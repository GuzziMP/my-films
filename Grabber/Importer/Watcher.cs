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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;

class MPR
{
  const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
  const int REMOTE_NAME_INFO_LEVEL = 0x00000002;

  const int ERROR_MORE_DATA = 234;
  const int NOERROR = 0;

  [DllImport("mpr.dll")]
  [return: MarshalAs(UnmanagedType.U4)]
  static extern int WNetGetUniversalName(
      string lpLocalPath,
      [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
      IntPtr lpBuffer,
      [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);

  public static string GetUniversalName(string localPath)
  {
    string retVal = null;

    // The pointer in memory to the structure.
    IntPtr buffer = IntPtr.Zero;

    // Wrap in a try/catch block for cleanup.
    try
    {
      // First, call WNetGetUniversalName to get the size.
      int size = 0;

      // Make the call.
      // Pass IntPtr.Size because the API doesn't like null, even though
      // size is zero.  We know that IntPtr.Size will be
      // aligned correctly.
      int apiRetVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);

      // If the return value is not ERROR_MORE_DATA, then
      // raise an exception.
      if (apiRetVal != ERROR_MORE_DATA)
        // Throw an exception.
        throw new Win32Exception(apiRetVal);

      // Allocate the memory.
      buffer = Marshal.AllocCoTaskMem(size);

      // Now make the call.
      apiRetVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref size);

      // If it didn't succeed, then throw.
      if (apiRetVal != NOERROR)
        // Throw an exception.
        throw new Win32Exception(apiRetVal);

      // Now get the string.  It's all in the same buffer, but
      // the pointer is first, so offset the pointer by IntPtr.Size
      // and pass to PtrToStringAuto.
      retVal = Marshal.PtrToStringAnsi(new IntPtr(buffer.ToInt64() + IntPtr.Size));
    }
    finally
    {
      // Release the buffer.
      Marshal.FreeCoTaskMem(buffer);
    }

    // First, allocate the memory for the structure.

    // That's all folks.
    return retVal;
  }
}


namespace Grabber.Importer
{

  public enum WatcherItemType
  {
    Added,
    Deleted
  }

  public class WatcherItem
  {
    public String m_sFullPathFileName;
    public String m_sParsedFileName;
    public WatcherItemType m_type;
    private FileInfo fileInfo = null;

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public WatcherItem(FileSystemWatcher watcher, RenamedEventArgs e, bool bOldName)
    {
      if (bOldName)
      {
        m_sFullPathFileName = e.OldFullPath;
        m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
        m_type = WatcherItemType.Deleted;
        LogMyFilms.Debug("File monitor: " + m_sParsedFileName + " " + m_type);
      }
      else
      {
        m_sFullPathFileName = e.FullPath;
        m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
        fileInfo = new FileInfo(m_sFullPathFileName);
        m_type = WatcherItemType.Added;
        LogMyFilms.Debug("File monitor: " + m_sParsedFileName + " " + m_type);
      }
    }

    public WatcherItem(FileSystemWatcher watcher, FileSystemEventArgs e)
    {
      m_sFullPathFileName = e.FullPath;
      m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
      switch (e.ChangeType)
      {
        case WatcherChangeTypes.Deleted:
          m_type = WatcherItemType.Deleted;
          break;

        default:
          m_type = WatcherItemType.Added;
          fileInfo = new FileInfo(m_sFullPathFileName);
          break;
      }
      LogMyFilms.Debug("File monitor: " + m_sParsedFileName + " " + m_type);
    }

    public WatcherItem(PathPair file, WatcherItemType type)
    {
      m_type = type;
      m_sFullPathFileName = file.m_sFull_FileName;
      m_sParsedFileName = file.m_sMatch_FileName;
      if (type == WatcherItemType.Added)
        fileInfo = new FileInfo(file.m_sFull_FileName);
      LogMyFilms.Debug("File monitor: " + m_sParsedFileName + " " + m_type);
    }

    bool IsLocked(FileInfo fileInfo)
    {
      FileStream stream = null;
      try
      {
        stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
      }
      catch (IOException)
      {
        return true;
      }
      finally
      {
        if (stream != null) stream.Close();
      }

      return false;
    }

    public bool IsLocked()
    {
      return fileInfo != null && IsLocked(fileInfo);
    }
  };

  public class Watcher
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log  
    public BackgroundWorker Worker = new BackgroundWorker();
    List<String> m_WatchedFolders = new List<String>();
    int m_nScanLapse; // number of minutes between scans

    List<String> m_ScannedFolders = new List<String>();
    List<PathPair> m_PreviousScan = new List<PathPair>();
    List<PathPair> m_PreviousScanRemovable = new List<PathPair>();

    List<FileSystemWatcher> m_watchersList = new List<FileSystemWatcher>();
    List<WatcherItem> m_modifiedFilesList = new List<WatcherItem>();
    volatile bool refreshWatchers = false;
    public delegate void WatcherProgressHandler(int nProgress, List<WatcherItem> modifiedFilesList);

    /// <summary>
    /// This will be triggered once all the movieinfo has been parsed completely.
    /// </summary>
    public event WatcherProgressHandler WatcherProgress;

    public Watcher(IEnumerable<string> watchedFolders, int nScanLapse)
    {
      LogMyFilms.Info("File Watcher: Creating new File System Watcher");

      m_nScanLapse = nScanLapse;

      foreach (String folder in watchedFolders)
      {
        string sRoot = Path.GetPathRoot(folder);
        try
        {
          var info = new DriveInfo(sRoot);

          switch (info.DriveType)
          {
            case DriveType.CDRom:
              LogMyFilms.Info("File Watcher: Skipping CD/DVD drive: {0}", sRoot);
              break;
            case DriveType.Network:
              LogMyFilms.Info("File Watcher: Adding watcher on folder: {0}", folder);
              m_ScannedFolders.Add(folder);
              break;
            default:
              LogMyFilms.Info("File Watcher: Adding watcher on folder: {0}", folder);
              m_WatchedFolders.Add(folder);
              break;
          }
        }
        catch (ArgumentException)
        {
          // this has to be a UNC path
          LogMyFilms.Info("File Watcher: Adding watcher on folder: {0}", folder);
          m_ScannedFolders.Add(folder);
        }
      }

      DeviceManager.OnVolumeInserted += OnVolumeInsertedRemoved;
      DeviceManager.OnVolumeRemoved += OnVolumeInsertedRemoved;

      Worker.WorkerReportsProgress = true;
      Worker.WorkerSupportsCancellation = true;
      Worker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);
      Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
      Worker.DoWork += new DoWorkEventHandler(WorkerWatcherDoWork);
    }

    void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (WatcherProgress != null) // only if any subscribers exist
        WatcherProgress.Invoke(e.ProgressPercentage, e.UserState as List<WatcherItem>);
    }

    void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Debug("Successfully stopped File System Watchers.");
    }

    public void StartFolderWatch()
    {
      // start the thread that is going to handle the addition in the db when files change
      Worker.RunWorkerAsync();
    }

    public void StopFolderWatch()
    {
      LogMyFilms.Debug("Stopping File System Watchers...");
      if (Worker.IsBusy)
        Worker.CancelAsync();
    }

    void RemoveFromModifiedFilesList(string filePath, WatcherItemType type, bool isFolder)
    {
      string completeFilePath = filePath;
      if (isFolder) completeFilePath = completeFilePath + "\\";

      var watcherItemsRemove = m_modifiedFilesList.Where(watcherItem => watcherItem.m_sFullPathFileName.StartsWith(filePath) && watcherItem.m_type == type).ToList();
      foreach (WatcherItem watcherItem in watcherItemsRemove)
        m_modifiedFilesList.Remove(watcherItem);
    }

    void WatcherRenamed(object sender, RenamedEventArgs e)
    {
      LogMyFilms.Debug("File Watcher: Renamed event: " + e.OldFullPath + " to " + e.FullPath);

      var filesToRemove = new List<PathPair>();
      var filesToAdd = new List<PathPair>();
      bool isDirectoryRename = false;

      if (Directory.Exists(e.FullPath))
      {
        isDirectoryRename = true;

        var folder = new List<string>();
        folder.Add(e.FullPath);
        filesToAdd = Filelister.GetFiles(folder);

        filesToRemove.AddRange(filesToAdd.Select(pathPair => new PathPair(pathPair.m_sMatch_FileName, pathPair.m_sFull_FileName.Replace(e.FullPath, e.OldFullPath))));
      }

      // rename: delete the old, add the new
      lock (m_modifiedFilesList)
      {
        if (isDirectoryRename)
        {
          foreach (PathPair pathPair in filesToRemove)
          {
            RemoveFromModifiedFilesList(pathPair.m_sFull_FileName, WatcherItemType.Added, false);
            m_modifiedFilesList.Add(new WatcherItem(pathPair, WatcherItemType.Deleted));
          }
          foreach (PathPair pathPair in filesToAdd)
          {
            RemoveFromModifiedFilesList(pathPair.m_sFull_FileName, WatcherItemType.Deleted, false);
            m_modifiedFilesList.Add(new WatcherItem(pathPair, WatcherItemType.Added));
          }
        }
        else
        {
          String sOldExtention = Path.GetExtension(e.OldFullPath);
          if (GrabUtil.Extensions.Contains(sOldExtention))
          {
            RemoveFromModifiedFilesList(e.OldFullPath, WatcherItemType.Added, false);
            m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e, true));
          }
          String sNewExtention = Path.GetExtension(e.FullPath);
          if (GrabUtil.Extensions.Contains(sNewExtention))
          {
            RemoveFromModifiedFilesList(e.FullPath, WatcherItemType.Deleted, false);
            m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e, false));
          }
        }
      }
    }

    void WatcherChanged(object sender, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("File Watcher: Changed event: " + e.FullPath);

      var filesChanged = new List<PathPair>();
      bool isDirectoryChange = false;

      if (Directory.Exists(e.FullPath))
      {
        isDirectoryChange = true;

        var folder = new List<string>();
        folder.Add(e.FullPath);
        filesChanged = Filelister.GetFiles(folder);
      }

      // a file has changed! created, not created, whatever. Just add it to our list. 
      // we only process this list once in a while
      lock (m_modifiedFilesList)
      {
        if (e.ChangeType == WatcherChangeTypes.Deleted)
        {
          RemoveFromModifiedFilesList(e.FullPath, WatcherItemType.Added, true);
          //ToDo: Add updates from MyFilms DB here ...
          //SQLCondition condition = new SQLCondition(new DBEpisode(), DBEpisode.cFilename, e.FullPath + "\\%", SQLConditionType.Like);
          //List<DBEpisode> dbepisodes = DBEpisode.Get(condition, false);
          //if (dbepisodes != null && dbepisodes.Count > 0)
          //{
          //    foreach (DBEpisode dbepisode in dbepisodes)
          //    {
          //        m_modifiedFilesList.Add(new WatcherItem(new PathPair(dbepisode[DBEpisode.cFilename].ToString().Substring(e.FullPath.Length).TrimStart('\\'), dbepisode[DBEpisode.cFilename]), WatcherItemType.Deleted));
          //    }
          //}
        }

        if (isDirectoryChange)
        {
          foreach (PathPair pathPair in filesChanged)
          {
            RemoveFromModifiedFilesList(pathPair.m_sFull_FileName, WatcherItemType.Deleted, false);
            m_modifiedFilesList.Add(new WatcherItem(pathPair, WatcherItemType.Added));
          }
        }
        else
        {
          /* duplicates are removed later
          foreach (WatcherItem item in m_modifiedFilesList)
          {
              if (item.m_sFullPathFileName == e.FullPath)
                  return;
          }
          */

          String sExtention = Path.GetExtension(e.FullPath);
          if (GrabUtil.Extensions.Contains(sExtention))
          {
            RemoveFromModifiedFilesList(e.FullPath, e.ChangeType == WatcherChangeTypes.Deleted ? WatcherItemType.Added : WatcherItemType.Deleted, false);
            m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e));
          }
        }
      }
    }

    void SetUpWatches()
    {
      if (m_watchersList.Count > 0)
      {
        LogMyFilms.Info("File Watcher: Cleaning up File System Watchers");

        // do some cleanup first, remove the existing watchers
        foreach (FileSystemWatcher watcher in m_watchersList)
        {
          watcher.EnableRaisingEvents = false;
          watcher.Changed -= new FileSystemEventHandler(WatcherChanged);
          watcher.Created -= new FileSystemEventHandler(WatcherChanged);
          watcher.Deleted -= new FileSystemEventHandler(WatcherChanged);
          watcher.Renamed -= new RenamedEventHandler(WatcherRenamed);
          watcher.Error -= new ErrorEventHandler(WatcherError);
        }
        m_watchersList.Clear();
      }

      // go through all enabled import folders, and add a watchfolder on it
      foreach (String sWatchedFolder in m_WatchedFolders)
      {
        if (Directory.Exists(sWatchedFolder))
        {
          var watcher = new FileSystemWatcher();
          // ZFLH watch for all types of file notification, filter in the event notification
          // from MSDN, filter doesn't change the amount of stuff looked at internally
          watcher.Path = sWatchedFolder;
          watcher.IncludeSubdirectories = true;
          watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
          watcher.Changed += new FileSystemEventHandler(WatcherChanged);
          watcher.Created += new FileSystemEventHandler(WatcherChanged);
          watcher.Deleted += new FileSystemEventHandler(WatcherChanged);
          watcher.Renamed += new RenamedEventHandler(WatcherRenamed);
          watcher.Error += new ErrorEventHandler(WatcherError);
          watcher.EnableRaisingEvents = true;
          m_watchersList.Add(watcher);
        }
      }
      refreshWatchers = false;
    }

    void WatcherError(object sender, ErrorEventArgs e)
    {
      LogMyFilms.Debug("File Watcher: Error event: " + e.GetException().Message);
      refreshWatchers = true;
    }

    static List<WatcherItem> removeDuplicates(List<WatcherItem> inputList)
    {
      var uniqueStore = new Dictionary<string, int>();
      var finalList = new List<WatcherItem>();

      LogMyFilms.Debug("Remove duplicates from inputList, starting count: " + inputList.Count);

      foreach (WatcherItem currValue in inputList)
      {
        string key = currValue.m_sFullPathFileName;
        switch (currValue.m_type)
        {
          case WatcherItemType.Deleted:
            key += " DELETED";
            break;
          case WatcherItemType.Added:
            key += " ADDED";
            break;
        }

        if (!uniqueStore.ContainsKey(key))
        {
          uniqueStore.Add(key, 0);
          finalList.Add(currValue);
        }
      }

      LogMyFilms.Debug("Removed duplicates from inputList, final count: " + inputList.Count);
      return finalList;
    }

    void SignalModifiedFiles()
    {
      try
      {
        lock (m_modifiedFilesList)
        {
          if (m_modifiedFilesList.Count > 0)
          {
            m_modifiedFilesList = removeDuplicates(m_modifiedFilesList);

            LogMyFilms.Debug("File Watcher: Signaling " + m_modifiedFilesList.Count + " modified files");
            var outList = new List<WatcherItem>();

            // leave locked files in the m_modifiedFilesList
            foreach (WatcherItem watcherItem in m_modifiedFilesList)
            {
              if (!watcherItem.IsLocked())
              {
                outList.Add(watcherItem);
              }
              else
              {
                LogMyFilms.Debug("File " + watcherItem.m_sParsedFileName + " is locked, leaving for later parsing!");
              }
            }
            // clear m_modifiedFilesList
            foreach (WatcherItem watcherItem in outList)
            {
              m_modifiedFilesList.Remove(watcherItem);
            }

            // go over the modified files list once in a while & update                        
            //outList.AddRange(m_modifiedFilesList);
            //m_modifiedFilesList.Clear();
            if (outList.Count > 0)
              Worker.ReportProgress(0, outList);
          }
        }
      }
      catch (Exception exp)
      {
        LogMyFilms.Debug("File Watcher: Exception happened in Signal Modified Files: " + exp.Message);
      }
    }

    List<String> GetDeviceManagerWatchedFolders()
    {
      List<String> result = new List<String>();
      if (DeviceManager.watchedDrives == null) return result;

      // ToDo: Add Import Pathes here ...
      //foreach (DBImportPath importPath in DBImportPath.GetAll())
      //{
      //    string sRoot = System.IO.Path.GetPathRoot(importPath[DBImportPath.cPath]);
      //    string importPathString = importPath[DBImportPath.cPath];
      //    if (!String.IsNullOrEmpty(importPathString) && (importPath[DBImportPath.cEnabled] != 0) && !String.IsNullOrEmpty(sRoot))
      //    {
      //        foreach (String drive in DeviceManager.watchedDrives)
      //        {
      //            if (importPathString.StartsWith(drive) && !result.Contains(importPathString))
      //                result.Add(importPathString);
      //        }
      //    }
      //}
      return result;
    }

    void OnVolumeInsertedRemoved(string volume, string serial)
    {
      LogMyFilms.Debug("On Volume Inserted or Removed: " + volume);

      List<String> folders = new List<String>();

      //foreach (DBImportPath importPath in DBImportPath.GetAll())
      //{
      //    string sRoot = System.IO.Path.GetPathRoot(importPath[DBImportPath.cPath]);
      //    if ((importPath[DBImportPath.cEnabled] != 0) && !String.IsNullOrEmpty(sRoot) && sRoot.ToLower().StartsWith(volume.ToLower()))
      //    {
      //      LogMyFilms.Debug("Adding for import or remove: " + importPath[DBImportPath.cPath]);
      //      folders.Add(importPath[DBImportPath.cPath]); 
      //    }
      //}

      if (folders.Count > 0)
      {
        var m_PreviousScanRemovableTemp = new List<PathPair>();
        m_PreviousScanRemovableTemp.AddRange(m_PreviousScanRemovable);
        foreach (String pair in folders)
        {
          m_PreviousScanRemovableTemp.RemoveAll(item => !item.m_sFull_FileName.StartsWith(pair));
        }
        foreach (PathPair pair in m_PreviousScanRemovableTemp)
        {
          m_PreviousScanRemovable.RemoveAll(item => item.m_sFull_FileName == pair.m_sFull_FileName);
        }

        DoFileScan(folders, ref m_PreviousScanRemovableTemp);

        m_PreviousScanRemovable.AddRange(m_PreviousScanRemovableTemp);
      }
    }

    void DoFileScan()
    {
      DoFileScan(m_ScannedFolders, ref m_PreviousScan);
    }

    void DoFileScan(List<String> scannedFolders, ref List<PathPair> previousScan)
    {
      LogMyFilms.Info("File Watcher: Performing File Scan on Import Paths for changes");
      bool isFullscreen = false;
      // Check if Fullscreen Video is active as this can cause stuttering/dropped frames
      // bool isFullscreen = (MediaPortal.GUI.Library.GUIWindowManager.ActiveWindow == (int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || MediaPortal.GUI.Library.GUIWindowManager.ActiveWindow == (int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_TVFULLSCREEN);
      if (isFullscreen)
      {
        LogMyFilms.Debug("File Watcher: Fullscreen Video has been detected, aborting file scan");
        return;
      }

      List<PathPair> newScan = Filelister.GetFiles(scannedFolders);

      var addedFiles = new List<PathPair>();
      addedFiles.AddRange(newScan);

      foreach (PathPair pair in previousScan)
      {
        addedFiles.RemoveAll(item => item.m_sFull_FileName == pair.m_sFull_FileName);
      }

      var removedFiles = new List<PathPair>();

      removedFiles.AddRange(previousScan);
      foreach (PathPair pair in newScan)
      {
        removedFiles.RemoveAll(item => item.m_sFull_FileName == pair.m_sFull_FileName);
      }

      lock (m_modifiedFilesList)
      {
        foreach (PathPair pair in addedFiles)
          m_modifiedFilesList.Add(new WatcherItem(pair, WatcherItemType.Added));

        foreach (PathPair pair in removedFiles)
          m_modifiedFilesList.Add(new WatcherItem(pair, WatcherItemType.Deleted));
      }

      previousScan = newScan;
    }

    void WorkerWatcherDoWork(object sender, DoWorkEventArgs e)
    {
      Thread.CurrentThread.Priority = ThreadPriority.Lowest;
      // delay the start of file monitoring for a small period (30 s.)
      Thread.Sleep(30 * 1000); // wait 30 seconds before starting watching ....

      LogMyFilms.Debug("File Watcher: Starting File System Watcher Background Task");
      SetUpWatches();

      // do the initial scan
      m_PreviousScan = Filelister.GetFiles(m_ScannedFolders);
      m_PreviousScanRemovable = Filelister.GetFiles(GetDeviceManagerWatchedFolders());

      DateTime timeLastScan = DateTime.Now;

      // then start the watcher loop
      while (!Worker.CancellationPending)
      {
        TimeSpan tsUpdate = DateTime.Now - timeLastScan;
        if ((int)tsUpdate.TotalMinutes > m_nScanLapse)
        {
          timeLastScan = DateTime.Now;
          DoFileScan();
        }

        SignalModifiedFiles();

        if (refreshWatchers)
          SetUpWatches();

        Thread.Sleep(1000);
      }
    }
  };

}

