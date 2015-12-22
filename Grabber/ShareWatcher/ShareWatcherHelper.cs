#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Linq;

namespace ShareWatcherHelper
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Timers;
  using Timer = System.Timers.Timer;

  public class ShareWatcherHelper
  {
    #region Variables

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    private bool bMonitoring;
//    public static MusicDatabase MyFilmsDB = null;
    private ArrayList m_Shares = new ArrayList();
    private ArrayList m_Watchers = new ArrayList();

    // Lock order is _enterThread, _events.SyncRoot
    private object m_EnterThread = new object(); // Only one timer event is processed at any given moment
    private ArrayList m_Events = null;

    private Timer m_Timer = null;

    private const int m_TimerInterval = 2000; // milliseconds

    public static event MediaFoundEventDelegate MediaFound;
    public delegate void MediaFoundEventDelegate(List<string> mediafiles, bool removeorphaned);



    #endregion

    #region Constructors/Destructors

    public ShareWatcherHelper(List<string> watchdirectories)
    {

//      MyFilmsDB = MusicDatabase.Instance;
      LoadShares(watchdirectories);
      LogMyFilms.Info("ShareWatcher starting up!");
    }

    #endregion

    #region Main

    public void StartMonitor()
    {
      if (bMonitoring)
      {
        LogMyFilms.Info("Starting up a worker thread...");
        Thread workerThread = new Thread(WatchShares);
        workerThread.IsBackground = true;
        workerThread.Name = "ShareWatcher";
        workerThread.Start();
      }
    }

    public void SetMonitoring(bool status)
    {
      bMonitoring = status;
    }

    public void ChangeMonitoring(bool status)
    {
      if (status)
      {
        bMonitoring = true;
        foreach (DelayedFileSystemWatcher watcher in m_Watchers)
        {
          watcher.EnableRaisingEvents = true;
        }
        m_Timer.Start();
        LogMyFilms.Info("Monitoring of shares enabled");
      }
      else
      {
        bMonitoring = false;
        foreach (DelayedFileSystemWatcher watcher in m_Watchers)
        {
          watcher.EnableRaisingEvents = false;
        }
        if (m_Timer != null)
        {
          m_Timer.Stop();
          m_Events.Clear();
        }
        LogMyFilms.Info("Monitoring of shares disabled");
      }
    }

    private void WatchShares()
    {
      LogMyFilms.Info("Monitoring active for following shares:");
      LogMyFilms.Info("---------------------------------------");

      // Release existing FSW Objects first
      foreach (DelayedFileSystemWatcher watcher in m_Watchers)
      {
        watcher.EnableRaisingEvents = false;
        watcher.Dispose();
      }
      m_Watchers.Clear();
      foreach (String sharename in m_Shares)
      {
        try
        {
          m_Events = ArrayList.Synchronized(new ArrayList(64));
          // Create the watchers. 
          //I need 2 type of watchers. 1 for files and the other for directories
          // Reason is that i don't know if the event occured on a file or directory.
          // For a Create / Change / Rename i could figure that out using FileInfo or DirectoryInfo,
          // but when something gets deleted, i don't know if it is a File or directory
          DelayedFileSystemWatcher watcherFile = new DelayedFileSystemWatcher();
          DelayedFileSystemWatcher watcherDirectory = new DelayedFileSystemWatcher();
          watcherFile.Path = sharename;
          watcherDirectory.Path = sharename;
          /* Watch for changes in LastWrite times, and the renaming of files or directories. */
          watcherFile.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Attributes;
          watcherDirectory.NotifyFilter = NotifyFilters.DirectoryName;
          // Monitor all Files and subdirectories.
          watcherFile.Filter = "*.*";
          watcherFile.IncludeSubdirectories = true;
          watcherDirectory.Filter = "*.*";
          watcherDirectory.IncludeSubdirectories = true;

          // Add event handlers.
          watcherFile.Changed += new FileSystemEventHandler(OnChanged);
          watcherFile.Created += new FileSystemEventHandler(OnCreated);
          watcherFile.Deleted += new FileSystemEventHandler(OnDeleted);
          watcherFile.Renamed += new RenamedEventHandler(OnRenamed);
          // For directories, i'm only interested in a Delete event
          watcherDirectory.Deleted += new FileSystemEventHandler(OnDirectoryDeleted);
          watcherDirectory.Renamed += new RenamedEventHandler(OnRenamed);

          // Begin watching.
          watcherFile.EnableRaisingEvents = true;
          watcherDirectory.EnableRaisingEvents = true;
          m_Watchers.Add(watcherFile);
          m_Watchers.Add(watcherDirectory);

          // Start Timer for processing events
          m_Timer = new Timer(m_TimerInterval);
          m_Timer.Elapsed += new ElapsedEventHandler(ProcessEvents);
          m_Timer.AutoReset = true;
          m_Timer.Enabled = watcherFile.EnableRaisingEvents;
          LogMyFilms.Info(sharename);
        }
        catch (ArgumentException ex)
        {
          LogMyFilms.Info("Unable to turn on monitoring for: {0} Exception: {1}", sharename, ex.Message);
        }
      }
      LogMyFilms.Info("---------------------------------------");
      LogMyFilms.Info("Note: Errors reported for CD/DVD drives can be ignored.");
    }

    #endregion Main

    #region EventHandlers

    // Event handler for Create of a file
    private void OnCreated(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("Add Song Fired: {0}", e.FullPath);
      m_Events.Add(new ShareWatcherEvent(ShareWatcherEvent.EventType.Create, e.FullPath));
    }

    // Event handler for Change of a file
    private void OnChanged(object source, FileSystemEventArgs e)
    {
      FileInfo fi = new FileInfo(e.FullPath);
      // A Change event occured.
      // Was it on a file? Ignore change events on directories
      if (fi.Exists)
      {
        LogMyFilms.Debug("Change Song Fired: {0}", e.FullPath);
        m_Events.Add(new ShareWatcherEvent(ShareWatcherEvent.EventType.Change, e.FullPath));
      }
    }

    // Event handler handling the Delete of a file
    private void OnDeleted(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("Delete Song Fired: {0}", e.FullPath);
      m_Events.Add(new ShareWatcherEvent(ShareWatcherEvent.EventType.Delete, e.FullPath));
    }

    // Event handler handling the Delete of a directory
    private void OnDirectoryDeleted(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("Delete Directory Fired: {0}", e.FullPath);
      m_Events.Add(new ShareWatcherEvent(ShareWatcherEvent.EventType.DeleteDirectory, e.FullPath));
    }

    // Event handler handling the Rename of a file/directory
    private void OnRenamed(object source, RenamedEventArgs e)
    {
      LogMyFilms.Debug("Rename File/Directory Fired: {0}", e.FullPath);
      m_Events.Add(new ShareWatcherEvent(ShareWatcherEvent.EventType.Rename, e.FullPath, e.OldFullPath));
    }

    #endregion EventHandlers

    #region Private Methods

    private void ProcessEvents(object sender, ElapsedEventArgs e)
    {
      // Allow only one Timer event to be executed.
      if (Monitor.TryEnter(m_EnterThread))
      {
        // Only one thread at a time is processing the events                
        try
        {
          // Lock the Collection, while processing the Events
          lock (m_Events.SyncRoot)
          {
            ShareWatcherEvent currentEvent = null;
            List<string> newfiles = new List<string>();
            for (int i = 0; i < m_Events.Count; i++)
            {
              currentEvent = m_Events[i] as ShareWatcherEvent;
              switch (currentEvent.Type)
              {
                case ShareWatcherEvent.EventType.Create:
                case ShareWatcherEvent.EventType.Change:
                  newfiles.Add(currentEvent.FileName);
                  //AddUpdateSong(currentEvent.FileName);
                  break;
                case ShareWatcherEvent.EventType.Delete:
                  //MyFilmsDB.DeleteSong(currentEvent.FileName, true);
                  LogMyFilms.Info("Deleted Song: {0}", currentEvent.FileName);
                  break;
                case ShareWatcherEvent.EventType.DeleteDirectory:
                  //MyFilmsDB.DeleteSongDirectory(currentEvent.FileName);
                  LogMyFilms.Info("Deleted Directory: {0}", currentEvent.FileName);
                  break;
                case ShareWatcherEvent.EventType.Rename:
                  //if (MyFilmsDB.RenameSong(currentEvent.OldFileName, currentEvent.FileName))
                  //{
                  //  Log.Info(LogType.MusicShareWatcher, "Song / Directory {0} renamed to {1]", currentEvent.OldFileName,
                  //           currentEvent.FileName);
                  //}
                  //else
                  //{
                  //  Log.Info(LogType.MusicShareWatcher, "Song / Directory rename failed: {0}", currentEvent.FileName);
                  //}
                  break;
              }
              m_Events.RemoveAt(i);
              i--; // Don't skip next event
            }
            if (newfiles.Count > 0)
            {
              if (MediaFound != null) // trigger AMCupdater to check and/or add file
              {
                MediaFound(newfiles, false);
                newfiles.Clear();
              }
            }
          }
        }
        finally
        {
          Monitor.Exit(m_EnterThread);
        }
      }
    }

    private static void AddUpdateSong(string strFileName) // Method used by OnCreated / Onchanged to add / update the song structure
    {
      //if (MyFilmsDB.SongExists(strFileName))
      //{
      //  MyFilmsDB.UpdateSong(strFileName);
      //  return;
      //}
      // For some reason the Create is fired already by windows while the file is still copied.
      // This happens especially on large songs copied via WLAN.
      // The result is that MP Readtag is throwing an IO Exception.
      // I'm trying to open the file here and in case of an exception i'll put it on a thread to be
      // processed 5 seconds later again.
      try
      {
        FileInfo file = new FileInfo(strFileName);
        Stream s = file.OpenRead();
        s.Close();
      }
      catch (IOException)
      {
        // The file is not closed yet. Ignore the event, it will be processed by the Change event
      }

      //MyFilmsDB.AddSong(strFileName);
      // Check for Various Artists
      //MyFilmsDB.CheckVariousArtists(song.Album);
    }

    #endregion

    #region Common Methods

    // Retrieve the Music Shares that should be monitored
    private int LoadShares(IList<string> watchdirectories)
    {
      foreach (string sharePath in watchdirectories.Where(sharePath => sharePath.Length > 0))
      {
        m_Shares.Add(sharePath);
      }
      return 0;
    }

    #endregion
  }
}