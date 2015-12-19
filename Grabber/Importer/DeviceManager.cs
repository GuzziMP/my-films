#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

namespace Grabber.Importer
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;

  /// <summary>
  /// Device Manager - needs proper description
  /// </summary>
  public class DeviceManager
  {

    #region Private Variables

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    private static readonly object syncRoot = new object();
    private static Dictionary<string, DriveInfo> driveInfoPool;

    #endregion

    #region Events / Delegates

    public delegate void DeviceManagerEvent(string volume, string serial);
    public static event DeviceManagerEvent OnVolumeInserted;
    public static event DeviceManagerEvent OnVolumeRemoved;
    public static List<string> watchedDrives;

    private static Thread watcherThread;
    private static Dictionary<string, bool> driveStates;

    #endregion

    #region Public Properties

    /// <summary>
    /// Check if the monitor is started
    /// </summary>
    public static bool MonitorStarted
    {
      get
      {
        lock (syncRoot)
        {
          return (watcherThread != null && watcherThread.IsAlive);
        }
      }
    }

    #endregion

    #region Constructor

    private DeviceManager()
    {

    }

    static DeviceManager()
    {
      lock (syncRoot)
      {
        driveInfoPool = new Dictionary<string, DriveInfo>();
      }
    }

    ~DeviceManager()
    {
      StopDiskWatcher();
    }

    #endregion

    #region Monitoring Logic

    public static void StartMonitor()
    {
      if (MonitorStarted) return;
      // ToDo: Add searchpathes here ....
      //DBImportPath[] importPaths = DBImportPath.GetAll();
      //if (importPaths != null)
      //{
      //  foreach (DBImportPath currPath in importPaths)
      //  {
      //    try
      //    {
      //      AddWatchDrive(currPath[DBImportPath.cPath]);
      //    }
      //    catch (Exception e)
      //    {
      //      if (e is ThreadAbortException) throw e;
      //      LogMyFilms.Debug("Failed adding " + currPath + " to the Disk Watcher!");
      //    }
      //  }
      //}
    }

    public static void StopMonitor()
    {
      if (!MonitorStarted)
        return;

      ClearWatchDrives();
    }

    public static void AddWatchDrive(string path)
    {
      AddWatchDrive(path, false);
    }
    public static void AddWatchDrive(string path, bool localOnly)
    {
      try
      {
        // if the path does not point to logical volume do not add it to the drive watcher
        if (PathIsUnc(path))
          return;

        if (localOnly)
        {
          if (PathIsOnNetwork(path)) return;
        }

        // get the driveletter
        string driveLetter = GetDriveLetter(path);
        if (driveLetter == null)
          return;

        // add the drive to the drive watcher
        lock (syncRoot)
        {
          if (watchedDrives == null)
            watchedDrives = new List<string>();

          if (!watchedDrives.Contains(driveLetter))
          {
            watchedDrives.Add(driveLetter);
            StartDiskWatcher();
            LogMyFilms.Info("Added " + driveLetter + " to Disk Watcher");
          }
        }
      }
      catch (Exception e)
      {
        if (e is ThreadAbortException)
          throw e;

        LogMyFilms.Debug("Error adding \"" + path + "\" to Disk Watcher!!");
      }
    }

    public static void RemoveWatchDrive(string path)
    {
      string driveLetter = GetDriveLetter(path);
      if (driveLetter == null)
        return;

      lock (syncRoot)
      {
        if (watchedDrives != null && watchedDrives.Contains(driveLetter))
        {
          watchedDrives.Remove(driveLetter);
          LogMyFilms.Info("Removed " + driveLetter + " from Disk Watcher");
          if (watchedDrives.Count == 0)
            StopDiskWatcher();
        }
      }

    }

    public static void ClearWatchDrives()
    {
      lock (syncRoot)
      {
        if (watchedDrives != null && watchedDrives.Count > 0)
        {
          LogMyFilms.Info("Removing all drives from Disk Watcher");
          watchedDrives.Clear();
        }
        StopDiskWatcher();
      }
    }

    public static void StartDiskWatcher()
    {
      lock (syncRoot)
      {
        if (!MonitorStarted)
        {
          LogMyFilms.Info("Starting Disk Watcher...");
          watcherThread = new Thread(new ThreadStart(WatchDisks));
          watcherThread.Name = "DeviceManager.WatchDisks";
          watcherThread.Start();
          LogMyFilms.Info("Successfully started Disk Watcher.");
        }
      }
    }

    public static void StopDiskWatcher()
    {
      lock (syncRoot)
      {
        if (MonitorStarted)
        {
          LogMyFilms.Info("Stopping Disk Watcher...");
          watcherThread.Abort();
          LogMyFilms.Info("Successfully stopped Disk Watcher.");
        }
      }
    }

    private static void WatchDisks()
    {
      while (true)
      {
        lock (syncRoot)
        {
          if (driveStates == null)
            driveStates = new Dictionary<string, bool>();

          foreach (string currDrive in watchedDrives)
          {
            try
            {
              // check if the drive is available
              DriveInfo driveInfo = GetDriveInfo(currDrive);
              bool isAvailable = driveInfo != null && driveInfo.IsReady;

              // if the previous drive state is not stored, store it and continue
              if (!driveStates.ContainsKey(currDrive))
              {
                driveStates[currDrive] = isAvailable;
                continue;
              }

              // if a change has occured
              if (driveStates[currDrive] != isAvailable)
              {

                // update our state - BEFIRE notifiers
                driveStates[currDrive] = isAvailable;

                // notify any listeners
                if (isAvailable)
                {

                  if (driveInfo.DriveType != DriveType.Network)
                    LogMyFilms.Info("Volume Inserted: " + currDrive);
                  else
                    LogMyFilms.Info("Volume Online: " + currDrive);

                  if (OnVolumeInserted != null)
                    OnVolumeInserted(currDrive, driveInfo.GetVolumeSerial());
                }
                else
                {
                  // if a mapped network share gets disconnected it can either show Network or NoRootDirectory
                  if (driveInfo.DriveType != DriveType.Network && driveInfo.DriveType != DriveType.NoRootDirectory)
                    LogMyFilms.Info("Volume Removed: " + currDrive);
                  else
                    LogMyFilms.Info("Volume Offline: " + currDrive);

                  if (OnVolumeRemoved != null)
                    OnVolumeRemoved(currDrive, null);
                }
              }
            }
            catch (Exception e)
            {
              if (e is ThreadAbortException)
                throw e;

              LogMyFilms.Debug("Unexpected error in Disk Watcher thread!");
            }
          }
        }
        Thread.Sleep(5000);
      }
    }

    /*
    // Listens for new import paths and adds them to the DiskWatcher
    private static void onPathAdded(DatabaseTable obj) {
        // If this is not an import path object break
        if (obj.GetType() != typeof(DBImportPath))
            return;

        // Add the new import path to the watched drives
        AddWatchDrive(((DBImportPath)obj).FullPath);
    }
    */

    #endregion

    #region Public Static Methods

    // Grab the drive letter from a FileSystemInfo object.
    public static string GetDriveLetter(FileSystemInfo fsInfo)
    {
      return fsInfo != null ? GetDriveLetter(fsInfo.FullName) : null;
    }

    // Grab drive letter from string
    public static string GetDriveLetter(string path)
    {
      // if the path is UNC return null
      if (path != null && PathIsUnc(path))
        return null;

      // return the first 2 characters
      if (path.Length > 1)
        return path.Substring(0, 2).ToUpper();
      else // or if only a letter was given add colon
        return path.ToUpper() + ":";
    }

    /// <summary>
    /// Gets a value indicating whether the path is in UNC format.
    /// </summary>
    /// <param name="path">path to check</param>
    /// <returns>True, if it's a UNC path</returns>
    public static bool PathIsUnc(string path)
    {
      return (path != null && path.StartsWith(@"\\"));

    }

    /// <summary>
    /// Gets a value indicating whether volume is a drive on a network
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool PathIsOnNetwork(string path)
    {
      if (PathIsUnc(path)) return true;
      try
      {
        var info = new DriveInfo(path);
        if (info.DriveType == DriveType.Network)
          return true;
      }
      catch (ArgumentException)
      {
        // has to be unc path
        return true;
      }
      return false;
    }

    /// <summary>
    /// Get the VolumeInfo object where this FileSystemInfo object is located.
    /// </summary>
    /// <param name="fsInfo"></param>
    /// <returns></returns>
    public static DriveInfo GetDriveInfo(FileSystemInfo fsInfo)
    {
      return GetDriveInfo(fsInfo.FullName);
    }

    /// <summary>
    /// Gets the DriveInfo object for the given path 
    /// When the object was created before it will be returned from cache.
    /// </summary>
    /// <param name="path">ex. E:\ </param>
    /// <returns></returns>
    public static DriveInfo GetDriveInfo(string path)
    {
      if (!PathIsUnc(path))
      {
        string driveletter = GetDriveLetter(path);
        if (!driveInfoPool.ContainsKey(driveletter))
        {
          lock (syncRoot)
          {
            if (!driveInfoPool.ContainsKey(driveletter))
            {
              try
              {
                driveInfoPool.Add(driveletter, new DriveInfo(driveletter));
              }
              catch (Exception e)
              {
                LogMyFilms.Debug("Error adding drive object for '" + driveletter + "' to cache. " + e.Message);
                return null;
              }
            }
          }
        }
        return driveInfoPool[driveletter];
      }
      else
      {
        // not a logical volume (no driveinfo)
        return null;
      }
    }

    /// <summary>
    /// Gets a value indicating if the FileSystemInfo object is currently available
    /// </summary>
    /// <param name="fsInfo"></param>
    /// <returns></returns>
    public static bool IsAvailable(FileSystemInfo fsInfo)
    {
      return IsAvailable(fsInfo, null);
    }

    /// <summary>
    /// Gets a value indicating if the FileSystemInfo object is currently available
    /// </summary>
    /// <param name="fsInfo"></param>
    /// <param name="recordedSerial">if a serial is specified the return value is more reliable.</param>
    /// <returns></returns>
    public static bool IsAvailable(FileSystemInfo fsInfo, string recordedSerial)
    {
      // Get Drive Information
      DriveInfo driveInfo = GetDriveInfo(fsInfo);

      // Refresh the object information (important)
      fsInfo.Refresh();

      // Check if the file exists
      bool fileExists = fsInfo.Exists;

      // Do we have a logical volume?
      if (driveInfo != null && fileExists)
      {
        string currentSerial = driveInfo.GetVolumeSerial();
        // if we have both the recorded and the current serial we can do this very exact
        // by checking if the serials match
        if (!String.IsNullOrEmpty(recordedSerial) && !String.IsNullOrEmpty(currentSerial))
          // return the exact check result
          return (currentSerial == recordedSerial);
      }

      // return the simple check result
      return fileExists;
    }

    /// <summary>
    /// Gets a value indicating wether this FileSystemInfo object is located on a removable drive type.
    /// </summary>
    /// <param name="fsInfo"></param>
    /// <returns></returns>
    public static bool IsRemovable(FileSystemInfo fsInfo)
    {
      // UNC is always removable
      if (PathIsUnc(fsInfo.FullName))
        return true;

      if (fsInfo.Exists)
      {
        try
        {
          if ((fsInfo.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            return true;
        }
        catch (Exception)
        {
          LogMyFilms.Debug("Failed check if " + fsInfo.FullName + " is a reparse point");
        }
      }

      return IsRemovable(fsInfo.FullName);
    }

    /// <summary>
    /// Gets a value indicating wether this path is located on a removable drive type.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsRemovable(string path)
    {

      // UNC is always removable
      if (PathIsUnc(path))
        return true;

      DriveInfo driveInfo = GetDriveInfo(path);
      return driveInfo == null || driveInfo.IsRemovable();
    }

    public static bool IsOpticalDrive(string path)
    {
      DriveInfo driveInfo = DeviceManager.GetDriveInfo(path);
      return (driveInfo != null && driveInfo.IsOptical());
    }

    /// <summary>
    /// Gets the disk serial of the drive were the given FileSystemInfo object is located.
    /// </summary>
    /// <param name="fsInfo"></param>
    /// <returns></returns>
    public static string GetVolumeSerial(FileSystemInfo fsInfo)
    {
      return GetVolumeSerial(fsInfo.FullName);
    }

    /// <summary>
    /// Gets the disk serial of the drive were the path is located.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetVolumeSerial(string path)
    {
      DriveInfo driveInfo = GetDriveInfo(path);
      if (driveInfo != null && driveInfo.IsReady)
        return driveInfo.GetVolumeSerial();
      else
        return string.Empty;
    }

    /// <summary>
    /// Gets the volume label of the drive were the given FileSystemInfo object is located.
    /// </summary>
    /// <param name="fsInfo"></param>
    /// <returns></returns>
    public static string GetVolumeLabel(FileSystemInfo fsInfo)
    {
      return GetVolumeLabel(fsInfo.FullName);
    }

    /// <summary>
    /// Gets the volume label of the drive where the path is located.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetVolumeLabel(string path)
    {
      DriveInfo driveInfo = GetDriveInfo(path);
      if (driveInfo != null && driveInfo.IsReady)
        return driveInfo.VolumeLabel;
      else
        return string.Empty;
    }

    #endregion

  }
}
