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

namespace ShareWatcherHelper
{
  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Timers;

  using Timer = System.Timers.Timer;

  /// <summary>
  /// The standard FileSystemWatcher raises numerous change events for a single change.
  /// To prevent that, this class is being used instead of the standard FSW.
  /// 
  /// This class wraps FileSystemEventArgs and RenamedEventArgs
  /// objects and detection of duplicate events.
  /// 
  ///  Code published by Adrian Hamza on
  ///  http://blogs.gotdotnet.com/ahamza/archive/2006/02/04/FileSystemWatcher_Duplicate_Events.aspx
  /// 
  /// </summary>
  internal class DelayedEvent
  {
    private readonly FileSystemEventArgs _args;

    public DelayedEvent(FileSystemEventArgs args)
    {
      Delayed = false;
      _args = args;
    }

    public FileSystemEventArgs Args
    {
      get { return _args; }
    }

    public bool Delayed { get; set; }

    public virtual bool IsDuplicate(object obj)
    {
      DelayedEvent delayedEvent = obj as DelayedEvent;
      if (delayedEvent == null)
      {
        return false; // this is not null so they are different 
      }
      else
      {
        FileSystemEventArgs eO1 = _args;
        RenamedEventArgs reO1 = _args as RenamedEventArgs;
        FileSystemEventArgs eO2 = delayedEvent._args;
        RenamedEventArgs reO2 = delayedEvent._args as RenamedEventArgs;
        // The events are equal only if they are of the same type (reO1 and reO2
        // are both null or NOT NULL) and have all properties equal.         
        // We also eliminate Changed events that follow recent Created events
        // because many apps create new files by creating an empty file and then 
        // they update the file with the file content.
        return ((eO1 != null && eO2 != null && eO1.ChangeType == eO2.ChangeType
                 && eO1.FullPath == eO2.FullPath && eO1.Name == eO2.Name) &&
                ((reO1 == null & reO2 == null) || (reO1 != null && reO2 != null &&
                                                   reO1.OldFullPath == reO2.OldFullPath && reO1.OldName == reO2.OldName))) ||
               (eO1 != null && eO2 != null && eO1.ChangeType == WatcherChangeTypes.Created
                && eO2.ChangeType == WatcherChangeTypes.Changed
                && eO1.FullPath == eO2.FullPath && eO1.Name == eO2.Name);
      }
    }
  }

  /// <summary>
  /// This class wraps a FileSystemWatcher object. The class is not derived
  /// from FileSystemWatcher because most of the FileSystemWatcher methods 
  /// are not virtual. The class was designed to resemble FileSystemWatcher class
  /// as much as possible so that you can use DelayedFileSystemWatcher instead 
  /// of FileSystemWatcher objects. 
  /// DelayedFileSystemWatcher will capture all events from the FileSystemWatcher object.
  /// The captured events will be delayed by at least ConsolidationInterval milliseconds in order
  /// to be able to eliminate duplicate events. When duplicate events are found, the last event
  /// is droped and the first event is fired (the reverse is not recomended because it could
  /// cause some events not be fired at all since the last event will become the first event and
  /// it won't fire a if a new similar event arrives imediately afterwards).
  /// </summary>
  public class DelayedFileSystemWatcher
  {
    private FileSystemWatcher _fileSystemWatcher;

    // Lock order is _enterThread, _events.SyncRoot
    private object _enterThread = new object(); // Only one timer event is processed at any given moment
    private ArrayList _events = null;

    private Timer _serverTimer = null;
    private int _msConsolidationInterval = 1000; // milliseconds

    #region Delegate to FileSystemWatcher

    public DelayedFileSystemWatcher()
    {
      _fileSystemWatcher = new FileSystemWatcher();
      Initialize();
    }

    public DelayedFileSystemWatcher(string path)
    {
      _fileSystemWatcher = new FileSystemWatcher(path);
      Initialize();
    }

    public DelayedFileSystemWatcher(string path, string filter)
    {
      _fileSystemWatcher = new FileSystemWatcher(path, filter);
      Initialize();
    }

    // Summary:
    //     Gets or sets a value indicating whether the component is enabled.
    //
    // Returns:
    //     true if the component is enabled; otherwise, false. The default is false.
    //     If you are using the component on a designer in Visual Studio 2005, the default
    //     is true.
    public bool EnableRaisingEvents
    {
      get { return _fileSystemWatcher.EnableRaisingEvents; }
      set
      {
        _fileSystemWatcher.EnableRaisingEvents = value;
        if (value)
        {
          _serverTimer.Start();
        }
        else
        {
          _serverTimer.Stop();
          _events.Clear();
        }
      }
    }

    //
    // Summary:
    //     Gets or sets the filter string, used to determine what files are monitored
    //     in a directory.
    //
    // Returns:
    //     The filter string. The default is "*.*" (Watches all files.)
    public string Filter
    {
      get { return _fileSystemWatcher.Filter; }
      set { _fileSystemWatcher.Filter = value; }
    }

    //
    // Summary:
    //     Gets or sets a value indicating whether subdirectories within the specified
    //     path should be monitored.
    //
    // Returns:
    //     true if you want to monitor subdirectories; otherwise, false. The default
    //     is false.
    public bool IncludeSubdirectories
    {
      get { return _fileSystemWatcher.IncludeSubdirectories; }
      set { _fileSystemWatcher.IncludeSubdirectories = value; }
    }

    //
    // Summary:
    //     Gets or sets the size of the internal buffer.
    //
    // Returns:
    //     The internal buffer size. The default is 8192 (8K).
    public int InternalBufferSize
    {
      get { return _fileSystemWatcher.InternalBufferSize; }
      set { _fileSystemWatcher.InternalBufferSize = value; }
    }

    //
    // Summary:
    //     Gets or sets the type of changes to watch for.
    //
    // Returns:
    //     One of the System.IO.NotifyFilters values. The default is the bitwise OR
    //     combination of LastWrite, FileName, and DirectoryName.
    //
    // Exceptions:
    //   System.ArgumentException:
    //     The value is not a valid bitwise OR combination of the System.IO.NotifyFilters
    //     values.
    public NotifyFilters NotifyFilter
    {
      get { return _fileSystemWatcher.NotifyFilter; }
      set { _fileSystemWatcher.NotifyFilter = value; }
    }

    //
    // Summary:
    //     Gets or sets the path of the directory to watch.
    //
    // Returns:
    //     The path to monitor. The default is an empty string ("").
    //
    // Exceptions:
    //   System.ArgumentException:
    //     The specified path contains wildcard characters.-or- The specified path contains
    //     invalid path characters.
    public string Path
    {
      get { return _fileSystemWatcher.Path; }
      set { _fileSystemWatcher.Path = value; }
    }

    //
    // Summary:
    //     Gets or sets the object used to marshal the event handler calls issued as
    //     a result of a directory change.
    //
    // Returns:
    //     The System.ComponentModel.ISynchronizeInvoke that represents the object used
    //     to marshal the event handler calls issued as a result of a directory change.
    //     The default is null.
    public ISynchronizeInvoke SynchronizingObject
    {
      get { return _fileSystemWatcher.SynchronizingObject; }
      set { _fileSystemWatcher.SynchronizingObject = value; }
    }

    // Summary:
    //     Occurs when a file or directory in the specified System.IO.FileSystemWatcher.Path
    //     is changed.
    public event FileSystemEventHandler Changed;
    //
    // Summary:
    //     Occurs when a file or directory in the specified System.IO.FileSystemWatcher.Path
    //     is created.
    public event FileSystemEventHandler Created;
    //
    // Summary:
    //     Occurs when a file or directory in the specified System.IO.FileSystemWatcher.Path
    //     is deleted.
    public event FileSystemEventHandler Deleted;

    //
    // Summary:
    //     Occurs when the internal buffer overflows.
    public event ErrorEventHandler Error;

    //
    // Summary:
    //     Occurs when a file or directory in the specified System.IO.FileSystemWatcher.Path
    //     is renamed.
    public event RenamedEventHandler Renamed;

    // Summary:
    //     Begins the initialization of a System.IO.FileSystemWatcher used on a form
    //     or used by another component. The initialization occurs at run time.
    public void BeginInit()
    {
      _fileSystemWatcher.BeginInit();
    }

    //
    // Summary:
    //     Releases the unmanaged resources used by the System.IO.FileSystemWatcher
    //     and optionally releases the managed resources.
    //
    // Parameters:
    //   disposing:
    //     true to release both managed and unmanaged resources; false to release only
    //     unmanaged resources.
    public void Dispose()
    {
      Uninitialize();
    }

    //
    // Summary:
    //     Ends the initialization of a System.IO.FileSystemWatcher used on a form or
    //     used by another component. The initialization occurs at run time.
    public void EndInit()
    {
      _fileSystemWatcher.EndInit();
    }

    //
    // Summary:
    //     Raises the System.IO.FileSystemWatcher.Changed event.
    //
    // Parameters:
    //   e:
    //     A System.IO.FileSystemEventArgs that contains the event data.
    protected void OnChanged(FileSystemEventArgs e)
    {
      if (Changed != null)
      {
        Changed(this, e);
      }
    }

    //
    // Summary:
    //     Raises the System.IO.FileSystemWatcher.Created event.
    //
    // Parameters:
    //   e:
    //     A System.IO.FileSystemEventArgs that contains the event data.
    protected void OnCreated(FileSystemEventArgs e)
    {
      if (Created != null)
      {
        Created(this, e);
      }
    }

    //
    // Summary:
    //     Raises the System.IO.FileSystemWatcher.Deleted event.
    //
    // Parameters:
    //   e:
    //     A System.IO.FileSystemEventArgs that contains the event data.
    protected void OnDeleted(FileSystemEventArgs e)
    {
      if (Deleted != null)
      {
        Deleted(this, e);
      }
    }

    //
    // Summary:
    //     Raises the System.IO.FileSystemWatcher.Error event.
    //
    // Parameters:
    //   e:
    //     An System.IO.ErrorEventArgs that contains the event data.
    protected void OnError(ErrorEventArgs e)
    {
      if (Error != null)
      {
        Error(this, e);
      }
    }

    //
    // Summary:
    //     Raises the System.IO.FileSystemWatcher.Renamed event.
    //
    // Parameters:
    //   e:
    //     A System.IO.RenamedEventArgs that contains the event data.
    protected void OnRenamed(RenamedEventArgs e)
    {
      if (Renamed != null)
      {
        Renamed(this, e);
      }
    }

    //
    // Summary:
    //     A synchronous method that returns a structure that contains specific information
    //     on the change that occurred, given the type of change you want to monitor.
    //
    // Parameters:
    //   changeType:
    //     The System.IO.WatcherChangeTypes to watch for.
    //
    // Returns:
    //     A System.IO.WaitForChangedResult that contains specific information on the
    //     change that occurred.
    public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
    {
      //TODO
      throw new NotImplementedException();
    }

    //
    // Summary:
    //     A synchronous method that returns a structure that contains specific information
    //     on the change that occurred, given the type of change you want to monitor
    //     and the time (in milliseconds) to wait before timing out.
    //
    // Parameters:
    //   timeout:
    //     The time (in milliseconds) to wait before timing out.
    //
    //   changeType:
    //     The System.IO.WatcherChangeTypes to watch for.
    //
    // Returns:
    //     A System.IO.WaitForChangedResult that contains specific information on the
    //     change that occurred.
    public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
    {
      //TODO
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation

    private void Initialize()
    {
      _events = ArrayList.Synchronized(new ArrayList(32));
      _fileSystemWatcher.Changed += new FileSystemEventHandler(FileSystemEventHandler);
      _fileSystemWatcher.Created += new FileSystemEventHandler(FileSystemEventHandler);
      _fileSystemWatcher.Deleted += new FileSystemEventHandler(FileSystemEventHandler);
      _fileSystemWatcher.Error += new ErrorEventHandler(ErrorEventHandler);
      _fileSystemWatcher.Renamed += new RenamedEventHandler(RenamedEventHandler);
      _serverTimer = new Timer(_msConsolidationInterval);
      _serverTimer.Elapsed += new ElapsedEventHandler(ElapsedEventHandler);
      _serverTimer.AutoReset = true;
      _serverTimer.Enabled = _fileSystemWatcher.EnableRaisingEvents;
    }

    private void Uninitialize()
    {
      if (_fileSystemWatcher != null)
      {
        _fileSystemWatcher.Dispose();
      }
      if (_serverTimer != null)
      {
        _serverTimer.Dispose();
      }
    }

    private void FileSystemEventHandler(object sender, FileSystemEventArgs e)
    {
      _events.Add(new DelayedEvent(e));
    }

    private void ErrorEventHandler(object sender, ErrorEventArgs e)
    {
      OnError(e);
    }

    private void RenamedEventHandler(object sender, RenamedEventArgs e)
    {
      _events.Add(new DelayedEvent(e));
    }

    private void ElapsedEventHandler(Object sender, ElapsedEventArgs e)
    {
      // We don't fire the events inside the lock. We will queue them here until
      // the code exits the locks.
      Queue eventsToBeFired = null;
      if (Monitor.TryEnter(_enterThread))
      {
        // Only one thread at a time is processing the events                
        try
        {
          eventsToBeFired = new Queue(32);
          // Lock the collection while processing the events
          lock (_events.SyncRoot)
          {
            DelayedEvent current = null;
            for (int i = 0; i < _events.Count; i++)
            {
              current = _events[i] as DelayedEvent;
              if (current.Delayed)
              {
                // This event has been delayed already so we can fire it
                // We just need to remove any duplicates
                for (int j = i + 1; j < _events.Count; j++)
                {
                  if (current.IsDuplicate(_events[j]))
                  {
                    // Removing later duplicates
                    _events.RemoveAt(j);
                    j--; // Don't skip next event
                  }
                }
                // Add the event to the list of events to be fired
                eventsToBeFired.Enqueue(current);
                // Remove it from the current list
                _events.RemoveAt(i);
                i--; // Don't skip next event
              }
              else
              {
                // This event was not delayed yet, so we will delay processing
                // this event for at least one timer interval
                current.Delayed = true;
              }
            }
          }
        }
        finally
        {
          Monitor.Exit(_enterThread);
        }
      }
      // else - this timer event was skipped, processing will happen during the next timer event

      // Now fire all the events if any events are in eventsToBeFired
      RaiseEvents(eventsToBeFired);
    }

    public int ConsolidationInterval
    {
      get { return _msConsolidationInterval; }
      set
      {
        _msConsolidationInterval = value;
        _serverTimer.Interval = value;
      }
    }

    protected void RaiseEvents(Queue deQueue)
    {
      if ((deQueue != null) && (deQueue.Count > 0))
      {
        DelayedEvent de = null;
        while (deQueue.Count > 0)
        {
          de = deQueue.Dequeue() as DelayedEvent;
          switch (de.Args.ChangeType)
          {
            case WatcherChangeTypes.Changed:
              OnChanged(de.Args);
              break;
            case WatcherChangeTypes.Created:
              OnCreated(de.Args);
              break;
            case WatcherChangeTypes.Deleted:
              OnDeleted(de.Args);
              break;
            case WatcherChangeTypes.Renamed:
              OnRenamed(de.Args as RenamedEventArgs);
              break;
          }
        }
      }
    }

    #endregion
  }
}