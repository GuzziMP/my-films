#region GNU license
// MyFilms - Plugin for Mediaportal
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

namespace MyFilmsPlugin.MyFilms
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Data.Linq;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Reflection;
  using System.Runtime.Serialization.Formatters.Binary;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Xml;

  using MediaPortal.Configuration;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.DataBase;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  public class BaseMesFilms
  {
    public BaseMesFilms()
    {
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    private static AntMovieCatalog data; // Ant compatible File - with temp extended fields and person infos

    internal static Queue<MFMovie> MovieUpdateQueue = new Queue<MFMovie>();
    internal static BackgroundWorker UpdateWorker = null;
    internal static AutoResetEvent UpdateWorkerDoneEvent = new AutoResetEvent(false);

    //private const int MaxWebDownloads = 4;
    //internal static WebClient[] WebClients = new WebClient[MaxWebDownloads];

    internal const string MultiUserStateField = "MultiUserState";

    internal const int TrakthandlerTimeout = 20000;
    private static TimerCallback traktUpdateQueueHandler = new TimerCallback(StartTraktUpdateHandler);
    public static readonly Timer TraktQueueTimer = new Timer(traktUpdateQueueHandler, null, Timeout.Infinite, Timeout.Infinite); // Create a Timer that that is inactive unless set by "Change() method initially // define timer without actions // new Timer(traktUpdateQueueHandler, "a state string", Timeout.Infinite, Timeout.Infinite); // define timer without actions

    // private static Dictionary<string, AntMovieCatalog> dataAllCatalogs = new Dictionary<string, AntMovieCatalog>(); // all data from all configs in a dictionary
    // private static XmlDataDocument xmlDoc; // XML Doc file for hierarchical access like XPath
    // private static Dictionary<string, ReaderWriterLockSlim> _lockDict = new Dictionary<string, ReaderWriterLockSlim>();
    // private static readonly object _locker = new object();
    // private static Dictionary<string, string> dataPath;

    private static readonly ReaderWriterLockSlim DataLock = new ReaderWriterLockSlim(); // private static ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    private static readonly Stopwatch Watch = new Stopwatch();
    private static DataRow[] movies; // selected movies with filters

    #region list of Custom fields, that will be added to existing catalogs, if missing
    internal static List<string[]> CustomFieldDefinitions = new List<string[]>
            {
              // Tag, Name, Type - // ftString, ftInteger, ftReal, ftBoolean, ftDate, ftList, ftText, ftUrl
              // <CustomField Tag="Edition" Name="Edition" Type="ftString" GUIProperties="rx6:ry51:rw526:rh25:aw1:ah0:lw94" />
              // CustomFieldList.Add(new string[] { "IndexedTitle", "IndexedTitle", "ftString" });
              new string[] { "Edition", "Edition", "ftString" },
              new string[] { "Studio", "Studio", "ftString" },
              new string[] { "Budget", "Budget", "ftString" },
              new string[] { "Fanart", "Fanart", "ftString" },
              new string[] { "Certification", "Certification", "ftString" },
              new string[] { "Writer", "Writer", "ftString" },
              new string[] { "Composer", "Composer", "ftString" },
              new string[] { "TagLine", "TagLine", "ftString" },
              new string[] { "Tags", "Tags", "ftString" },
              new string[] { "Aspectratio", "Aspectratio", "ftString" },
              new string[] { "CategoryTrakt", "CategoryTrakt", "ftString" },
              new string[] { "Watched", "Watched", "ftString" },
              new string[] { "Favorite", "Favorite", "ftString" },
              new string[] { "RatingUser", "RatingUser", "ftReal" },
              new string[] { "IMDB_Id", "IMDB_Id", "ftString" },
              new string[] { "TMDB_Id", "TMDB_Id", "ftString" },
              new string[] { "IMDB_Rank", "IMDB_Rank", "ftString" },
              new string[] { "Votes", "Votes", "ftString" },
              new string[] { "SourceTrailer", "SourceTrailer", "ftString" },
              new string[] { "IsOnline", "IsOnline", "ftString" },
              new string[] { "IsOnlineTrailer", "IsOnlineTrailer", "ftString" },
              new string[] { "LastPosition", "LastPosition", "ftString" },
              new string[] { "AudioChannelCount", "AudioChannelCount", "ftString" },
              new string[] { "AlternateTitles", "AlternateTitles", "ftString" },
              new string[] { "DateWatched", "DateWatched", "ftDate" },
              new string[] { "DateReleased", "DateReleased", "ftDate" },
              new string[] { "MultiUserState", "MultiUserState", "ftString" },
              new string[] { "S3D", "S3D", "ftString" },
              new string[] { "Collection", "Collection", "ftString" },
              new string[] { "CollectionImage", "CollectionImage", "ftString" },
              new string[] { "CollectionDetails", "CollectionDetails", "ftString" },
              new string[] { "CustomField1", "CustomField1", "ftString" },
              new string[] { "CustomField2", "CustomField2", "ftString" },
              new string[] { "CustomField3", "CustomField3", "ftString" }
            };
    #endregion

    public class MfConfig
    {
      public string Name { get; set; }
      public List<KeyValuePair<string, string>> ViewList { get; set; }
    }

    public enum MostRecentType
    {
      Watched,
      Added
    }

    #region ctor
    static BaseMesFilms()
    {
      // _lockDict = GetLockerList();
    }
    #endregion

    #region private static methods ...

    private static void StartTraktUpdateHandler(object state)
    {
      // nothing to update, abort
      if (MovieUpdateQueue.Count == 0) return;

      if (UpdateWorker != null && UpdateWorker.IsBusy)
      {
        TraktQueueTimer.Change(TrakthandlerTimeout, Timeout.Infinite); // retry in 20 seconds
        return;
      }
      UpdateWorker = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = false };
      UpdateWorker.DoWork += new DoWorkEventHandler(UpdateWorkerDoWork);
      // updateWorker.ProgressChanged += new ProgressChangedEventHandler(updateWorker_ProgressChanged);
      UpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateWorkerRunWorkerCompleted);
      UpdateWorker.RunWorkerAsync();
    }

    private static void UpdateWorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs) // The method that is executed when the update queue timer expires. // state is null, as we didn't hand over any ...
    {
      Thread.CurrentThread.Name = "DB Update Worker";

      LogMyFilms.Debug("TraktUpdateHandler() - has been called - processing queue with '" + MovieUpdateQueue.Count + "' items.");
      var movielist = new List<MFMovie>();
      var configs = new List<string>();
      lock (MovieUpdateQueue)
      {
        do
        {
          var movie = MovieUpdateQueue.Dequeue();
          movielist.Add(movie);
          if (!configs.Contains(movie.Config))
            configs.Add(movie.Config);
        }
        while (MovieUpdateQueue.Count > 0);
      }
      TraktQueueTimer.Change(Timeout.Infinite, Timeout.Infinite);
      foreach (string config in configs) // call updates per config
      {
        UpdateMovies(config, movielist.Where(x => x.Config == config).ToList());
      }
      UpdateWorkerDoneEvent.Set(); // send notification, that worker has completed!
    }

    private static void UpdateWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
        Thread.CurrentThread.Name = "DB Update Worker";
      LogMyFilms.Info("TraktUpdateHandler finished.");
    }

    private static Dictionary<string, ReaderWriterLockSlim> GetLockerList()
    {
      var lockDictionary = new Dictionary<string, ReaderWriterLockSlim>();
      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
        for (int i = 0; i < mesFilmsNbConfig; i++) lockDictionary.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty), new ReaderWriterLockSlim());
      }
      return lockDictionary;
    }

    private static void InitData()
    {
      var initDataWatch = new Stopwatch();
      initDataWatch.Reset(); initDataWatch.Start();
      bool success = LoadMyFilmsFromDisk(MyFilms.conf.StrFileXml);
      initDataWatch.Stop();
      LogMyFilms.Debug("initData() - End reading catalogfile '" + MyFilms.conf.StrFileXml + "' (success = '" + success + "') (" + (initDataWatch.ElapsedMilliseconds) + " ms)");
    }

    internal class DataColumnComparer : IEqualityComparer<DataColumn>
    {
      #region IEqualityComparer<DataColumn> Members

      public bool Equals(DataColumn x, DataColumn y)
      {
        return x.Caption == y.Caption;
      }

      public int GetHashCode(DataColumn obj)
      {
        return obj.Caption.GetHashCode();
      }

      #endregion
    }

    //private class ObjectArrayComparer : IEqualityComparer<object[]>
    //{
    //  #region IEqualityComparer<object[]> Members

    //  public bool Equals(object[] x, object[] y)
    //  {
    //    for (var i = 0; i < x.Length; i++)
    //    {
    //      if (!object.Equals(x[i], y[i]))
    //        return false;
    //    }

    //    return true;
    //  }

    //  public int GetHashCode(object[] obj)
    //  {
    //    return obj.Sum(item => item.GetHashCode());
    //  }

    //  #endregion
    //}

    private static void CreateEmptyDataFile(string datafile)
    {
      XmlTextWriter destXml = new XmlTextWriter(datafile, System.Text.Encoding.Default) {Formatting = Formatting.Indented};
      destXml.WriteStartDocument();
      destXml.WriteStartElement("MyFilmsData");
      destXml.WriteStartElement("Persons");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      destXml.WriteStartElement("History");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      destXml.Close();
    }

    private static void CopyExtendedFieldsToCustomFields(bool cleanfileonexit)
    {
      var saveDataWatch = new Stopwatch(); saveDataWatch.Reset(); saveDataWatch.Start();
      IEnumerable<DataColumn> commonColumns = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer()).Where(x => x.ColumnName != "Movie_Id").ToList();
      foreach (var movieRow in Enumerable.Where(data.Movie, movieRow => movieRow.RowState != DataRowState.Deleted))
      {
        movieRow.BeginEdit();
        AntMovieCatalog.CustomFieldsRow customFields = null;
        if (movieRow.GetCustomFieldsRows().Length == 0) // create CustomFields Element, if not existing ...
        {
          customFields = data.CustomFields.NewCustomFieldsRow();
          customFields.SetParentRow(movieRow);
          data.CustomFields.AddCustomFieldsRow(customFields);
          // LogMyFilms.Debug("LoadMyFilmsFromDisk() - created new CustomFieldsRow for movie ID '" + movieRow.Number + "', Title = '" + movieRow.OriginalTitle + "'");
        }
        customFields = movieRow.GetCustomFieldsRows()[0];
        foreach (DataColumn dc in commonColumns)
        {
          customFields[dc.ColumnName] = movieRow[dc.ColumnName];
          #region disabled conditional updates
          // object temp;
          // if (DBNull.Value != (temp = customFields[dc.ColumnName])) movieRow[dc.ColumnName] = temp; // diabled the copy from customfields to MyFilms rows - this is only when saving and we do not modify customfields in plugin !

          //if (DBNull.Value != (temp = movieRow[dc.ColumnName]))
          //{
          //  customFields[dc.ColumnName] = temp;
          //  if (cleanfileonexit)
          //  {
          //    movieRow[dc.ColumnName] = System.Convert.DBNull;
          //  }
          //}
          #endregion
        }
      }
      data.Movie.AcceptChanges();
      saveDataWatch.Stop();
      LogMyFilms.Debug("CopyExtendedFieldsToCustomFields() - Copy CustomFields from MovieRow's done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
    }

    private static void CreateOrUpdateCatalogProperties()
    {
      var saveDataWatch = new Stopwatch(); saveDataWatch.Reset(); saveDataWatch.Start();
      AntMovieCatalog.PropertiesRow[] propCollection = data.Catalog[0].GetPropertiesRows();
      if (propCollection.Length == 0)
      {
        //<Properties Owner="SampleOwner" Mail="myfilms@gmail.com" Site="http://www.team-mediaportal.com" Description="Sample description" />
        AntMovieCatalog.PropertiesRow prop = data.Properties.NewPropertiesRow();
        prop.SetParentRow(data.Catalog[0]);
        prop.Owner = "MyFilms";
        prop.Description = "AMC Movie Database";
        prop.Site = "http://www.team-mediaportal.com";
        data.Properties.AddPropertiesRow(prop);
      }
      saveDataWatch.Stop();
      LogMyFilms.Debug("CreateOrUpdateCatalogProperties() - add Properties done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");

      #region add CustomFields table, if missing
      saveDataWatch.Reset(); saveDataWatch.Start();
      AntMovieCatalog.CustomFieldsPropertiesRow[] cfpCollection = data.Catalog[0].GetCustomFieldsPropertiesRows();
      if (cfpCollection.Length == 0)
      {
        AntMovieCatalog.CustomFieldsPropertiesRow cfp = data.CustomFieldsProperties.NewCustomFieldsPropertiesRow();
        cfp.SetParentRow(data.Catalog[0]);
        data.CustomFieldsProperties.AddCustomFieldsPropertiesRow(cfp);
      }
      #endregion

      AntMovieCatalog.CustomFieldRow[] customFieldRows = data.CustomFieldsProperties[0].GetCustomFieldRows();

      var cFields = customFieldRows.Select(fieldRow => fieldRow.Tag).ToList();

      #region add missing custom field definitions to customfield table
      foreach (var customFieldDefinition in CustomFieldDefinitions)
      {
        if (!cFields.Contains(customFieldDefinition[0]))
        {
          AntMovieCatalog.CustomFieldRow cfr = data.CustomField.NewCustomFieldRow();
          cfr.SetParentRow(data.CustomFieldsProperties[0]);
          cfr.Tag = customFieldDefinition[0];
          cfr.Name = (!string.IsNullOrEmpty(TranslateColumn(customFieldDefinition[0]))) ? TranslateColumn(customFieldDefinition[0]) : customFieldDefinition[1];
          cfr.Type = customFieldDefinition[2];
          data.CustomField.AddCustomFieldRow(cfr);
        }
      }
      #endregion
      saveDataWatch.Stop();
      LogMyFilms.Debug("CreateOrUpdateCatalogProperties() - Adding CustomFields Definitions done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
    }

    private static void CreateMissingCustomFieldsEntries()
    {
      var loadDataWatch = new Stopwatch(); loadDataWatch.Reset(); loadDataWatch.Start();
      foreach (var movieRow in data.Movie)
      {
        var customFieldsRows = movieRow.GetCustomFieldsRows();
        if (customFieldsRows.Length == 0) // create CustomFields Element, if not existing ...
        {
          AntMovieCatalog.CustomFieldsRow customFields = data.CustomFields.NewCustomFieldsRow();
          customFields.SetParentRow(movieRow);
          data.CustomFields.AddCustomFieldsRow(customFields);
          // LogMyFilms.Debug("LoadMyFilmsFromDisk() - created new CustomFieldsRow for movie ID '" + movieRow.Number + "', Title = '" + movieRow.OriginalTitle + "'");
        }
      }
      loadDataWatch.Stop();
      LogMyFilms.Debug("CreateMissingCustomFieldsEntries() - done ... (" + (loadDataWatch.ElapsedMilliseconds) + " ms)");
    }

    private static bool SaveMyFilmsToDisk(string catalogfile)
    {
      Watch.Reset(); Watch.Start();
      bool success = false; // result of write operation
      const int maxretries = 5; // max retries 10 * 1000 = 10 seconds
      int i = 0;

      while (!success && i < maxretries)
      {
        if (!MyFilmsDetail.GlobalLockIsActive(catalogfile)) // first check, if there is a global manual lock
        {
          try
          {
            MyFilmsDetail.SetGlobalLock(true, catalogfile);

            CopyExtendedFieldsToCustomFields(false);

            using (var fsTmp = File.Create(catalogfile.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose)) // make sure, only one process is writing to file !
            {
              using (var fs = new FileStream(catalogfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
              {
                LogMyFilms.Debug("SaveMyFilmsToDisk()- opening '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                fs.SetLength(0); // do not append, owerwrite !
                using (var myXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                {
                  LogMyFilms.Debug("SaveMyFilmsToDisk()- writing '" + catalogfile + "' as MyXmlTextWriter in FileStream");
                  myXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                  myXmlTextWriter.WriteStartDocument();
                  data.WriteXml(myXmlTextWriter, XmlWriteMode.IgnoreSchema); myXmlTextWriter.Flush();
                  myXmlTextWriter.Close();
                }
                //xmlDoc.Save(fs);
                fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                LogMyFilms.Debug("SaveMyFilmsToDisk()- closing '" + catalogfile + "' FileStream and releasing file lock");
                success = true;
              }
            }
            Thread.Sleep(50);
          }
          catch (Exception ex)
          {
            LogMyFilms.DebugException("SaveMyFilmsToDisk()- error saving '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None", ex);
            success = false;
          }
          finally
          {
            MyFilmsDetail.SetGlobalLock(false, catalogfile);
          }
        }
        else
        {
          i += 1;
          LogMyFilms.Info("SaveMyFilmsToDisk() - Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
          Thread.Sleep(1000);
        }
      }
      Watch.Stop();
      MyFilms.LastDbUpdate = DateTime.Now;
      LogMyFilms.Debug("SaveMyFilmsToDisk() - Finished Saving ... (" + (Watch.ElapsedMilliseconds) + " ms)");
      return success;
    }

    private static bool LoadMyFilmsFromDisk(string catalogfile)
    {
      bool success;
      LogMyFilms.Debug("LoadMyFilmsFromDisk() - Current Readlocks: '" + DataLock.CurrentReadCount + "'");
      // if (_dataLock.CurrentReadCount > 0) return false;// might be opened by API as well, so count can be 2+

      DataLock.EnterWriteLock();
      try
      {
        #region load catalog from file into dataset
        Watch.Reset(); Watch.Start();
        const int _numberOfTries = 20;
        const int _timeIntervalBetweenTries = 500;
        var tries = 0;
        while (true)
        {
          try
          {
            using (var fsTmp = File.Create(catalogfile.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose)) // make sure, no process is writing to file !
            {
              using (var fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
              {
                data = new AntMovieCatalog();
                LogMyFilms.Debug("LoadMyFilmsFromDisk() - opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.ReadWrite");
                foreach (DataTable dataTable in data.Tables) dataTable.BeginLoadData(); // dataTable.Rows.Clear();
                //// synchronize dataset with hierarchical XMLdoc
                //xmlDoc = new XmlDataDocument(data);
                //xmlDoc.Load(fs);
                data.ReadXml(fs);
                foreach (DataTable dataTable in data.Tables) dataTable.EndLoadData();
                fs.Close();
                LogMyFilms.Debug("LoadMyFilmsFromDisk() - closing  '" + catalogfile + "' FileStream");
              }
            }
            success = true;
            break;
          }
          catch (Exception e) // catch (IOException e)
          {
            // if (!Helper.IsFileLocked(e)) throw;
            if (++tries > _numberOfTries)
            {
              // throw new MyCustomException("The file is locked too long: " + e.Message, e);
              LogMyFilms.Error("LoadMyFilmsFromDisk() - File locked too long ! Returning error !");
              return false;
            }
            else
            {
              LogMyFilms.Debug("LoadMyFilmsFromDisk() - File is locked on try '" + tries + "' of '" + _numberOfTries + "' - waiting another '" + _timeIntervalBetweenTries + "' ms. - reason: " + e.Message);
              Thread.Sleep(_timeIntervalBetweenTries);
            }
          }
        }

        //foreach (DataTable dataTable in data.Tables)
        //{
        //  LogMyFilms.Debug("initData() - loaded table '" + dataTable + "'");
        //  foreach (var childrelation in dataTable.ChildRelations) LogMyFilms.Debug("initData() - childrelation: '" + childrelation + "'");
        //}
        Watch.Stop();
        LogMyFilms.Debug("LoadMyFilmsFromDisk() - Finished  (" + (Watch.ElapsedMilliseconds) + " ms)");
        #endregion

        CreateOrUpdateCatalogProperties();

        // CreateMissingCustomFieldsEntries(); // we don't need them when reading - only when writing back to disk!

        #region calculate artificial columns like AgeAdded, IndexedTitle, Persons, etc. and CustomFields Copy ...
        IEnumerable<DataColumn> commonColumns = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer()).Where(x => x.ColumnName != "Movie_Id").ToList();
        var now = DateTime.Now;
        Watch.Reset(); Watch.Start();
        //data.Movie.BeginLoadData();
        //data.EnforceConstraints = false; // primary key uniqueness, foreign key referential integrity and nulls in columns with AllowDBNull = false etc...
        foreach (var movieRow in data.Movie)
        {
          movieRow.BeginEdit();
          //// Convert(Date,'System.DateTime')
          //int iAge = 9999; // set default to 9999 for those, where we do not have date(added) in DB ...
          //// CultureInfo ci = CultureInfo.CurrentCulture;
          //if (!movieRow.IsDateNull() && DateTime.TryParse(movieRow.Date, out added)) // CultureInfo.InvariantCulture ??? // else movieRow.DateAdded = DateTime.MinValue; ???
          //{
          //  // movieRow.DateAdded = Convert.ToDateTime(movieRow.Date); // is same as: movieRow.DateAdded = DateTime.Parse(movieRow.Date, CultureInfo.CurrentCulture);
          //  movieRow.DateAdded = added;
          //  iAge = (int)now.Subtract(added).TotalDays; // iAge = (!movieRow.IsDateAddedNull()) ? ((int)now.Subtract(movieRow.DateAdded).TotalDays) : 9999;
          //}
          //movieRow.AgeAdded = iAge; // sets integer value
          //movieRow.RecentlyAdded = MyFilms.GetDayRange(iAge);
          DateTime added;
          if (!movieRow.IsDateNull() && DateTime.TryParse(movieRow.Date, out added)) // CultureInfo.InvariantCulture ??? // else movieRow.DateAdded = DateTime.MinValue; ???
            movieRow.DateAdded = Convert.ToDateTime(movieRow.Date); // is same as: movieRow.DateAdded = DateTime.Parse(movieRow.Date, CultureInfo.CurrentCulture);
          else
            movieRow.DateAdded = DateTime.MinValue;
          // movieRow.DateAdded = Convert.ToDateTime(movieRow.Date); // is same as: movieRow.DateAdded = DateTime.Parse(movieRow.Date, CultureInfo.CurrentCulture);
          movieRow.AgeAdded = (int)now.Subtract(movieRow.DateAdded).TotalDays; // sets integer value
          movieRow.RecentlyAdded = MyFilms.GetDayRange(movieRow.AgeAdded);
          string index = movieRow[MyFilms.conf.StrTitle1].ToString();
          movieRow.IndexedTitle = (index.Length > 0) ? index.Substring(0, 1).ToUpper() : "";
          movieRow.Persons = (movieRow.Actors ?? " ") + ", " + (movieRow.Producer ?? " ") + ", " + (movieRow.Director ?? " ") + ", " + (movieRow.Writer ?? " "); // Persons: ISNULL(Actors,' ') + ', ' + ISNULL(Producer, ' ') + ', ' + ISNULL(Director, ' ') + ', ' + ISNULL(Writer, ' ')
          // if (!movieRow.IsLengthNull()) movieRow.Length_Num = Convert.ToInt32(movieRow.Length);
          if (!movieRow.IsSourceNull() && movieRow.Source.Length > 0)
          {
            string path = movieRow.Source;
            if (path.StartsWith(@"\\")) path = path.Substring(2); // cut the first 2 chars on network drives
            int pos = path.IndexOf(";", System.StringComparison.Ordinal);
            if (pos > 0) path = path.Substring(0, pos).Trim();
            movieRow.VirtualPathTitle = path.ToLower();
          }

          #region update Favorite from UserRating (disabled - should be done in trakt handler updates and manual updates)
          //if (!movieRow.IsRatingUserNull())
          //{
          //  //if (movieRow.RatingUser > 5)
          //  //  // add user to movieRow.Favorite
          //  //else
          //  //  // remove user from movieRow.Favorite
          //}
          //else
          //  movieRow.Favorite = null;
          #endregion

          #region Copy CustomFields data ....
          if (movieRow.GetCustomFieldsRows().Length > 0) // customfields are present - use it! (we only create them on saving)
          {
            var customFields = movieRow.GetCustomFieldsRows()[0]; // Relations["Movie_CustomFields"]
            foreach (DataColumn dc in commonColumns)
            {
              movieRow[dc.ColumnName] = customFields[dc.ColumnName];
              //object temp;
              //// only copy CustomFields, if not nothing, as user might have initial values in Elements!
              //if (DBNull.Value != (temp = customFields[dc.ColumnName]))
              //  movieRow[dc.ColumnName] = temp;
            }
          }
          #endregion
        }
        //data.EnforceConstraints = true;
        //data.Movie.EndLoadData();
        LogMyFilms.Debug("LoadMyFilmsFromDisk() - Calc PreAcceptChanges ... (" + (Watch.ElapsedMilliseconds) + " ms)");
        data.Movie.AcceptChanges();
        Watch.Stop();
        LogMyFilms.Debug("LoadMyFilmsFromDisk() - Calc & CustomField Copy Finished ... (" + (Watch.ElapsedMilliseconds) + " ms)");
        #endregion

        MyFilms.LastDbUpdate = DateTime.Now;
      }
      catch (Exception e)
      {
        success = false;
        LogMyFilms.Error("LoadMyFilmsFromDisk() : Error reading xml database after " + data.Movie.Count + " records; error : " + e.Message + ", " + e.StackTrace);
        if (data.Movie.Count > 0) LogMyFilms.Error("LoadMyFilmsFromDisk() : Last Record: '" + data.Movie[data.Movie.Count - 1].Number + "', title: '" + data.Movie[data.Movie.Count - 1].OriginalTitle + "'");
        string strOtitle = (data.Movie.Count > 0) ? data.Movie[data.Movie.Count - 1].OriginalTitle : "n/a";
        throw new Exception("Error reading xml database after " + data.Movie.Count + " records; movie: '" + strOtitle + "'; error : " + e.Message);
        //throw e;
      }
      finally
      {
        DataLock.ExitWriteLock();
      }

      return success;
    }

    private static ArrayList GetMoviesGlobal(string expression, string sort, bool traktOnly, bool mostRecentOnly)
    {
      var moviesGlobal = new ArrayList();
      moviesGlobal.Clear();
      using (var dataExport = new AntMovieCatalog())
      {
        using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
        {
          int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
          var configs = new ArrayList();
          for (int i = 0; i < mesFilmsNbConfig; i++) configs.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

          LogMyFilms.Debug("GetMoviesGlobal() - Configs found: '" + configs.Count + "' - now checking, if they are enabled ...");
          foreach (string config in configs)
          {
            dataExport.Clear();
            string StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
            string StrFileType = xmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
            bool AllowTraktSync = xmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
            bool AllowRecentlyAddedAPI = xmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);
            string StrUserProfileName = xmlConfig.ReadXmlConfig("MyFilms", config, "UserProfileName", ""); // MyFilms.DefaultUsername

            string catalogname = Enum.GetName(typeof(MyFilmsGUI.Configuration.CatalogType), Int32.Parse(StrFileType));

            if (AllowTraktSync || AllowRecentlyAddedAPI)
              LogMyFilms.Debug("Trakt|LMH = '" + AllowTraktSync + "|" + AllowRecentlyAddedAPI + "', Config = '" + config + "', CatalogType = '" + StrFileType + "|" + catalogname + "', DBFile = '" + StrFileXml + "'");
            else
              LogMyFilms.Debug("Trakt|LMH = '" + AllowTraktSync + "|" + AllowRecentlyAddedAPI + "', Config = '" + config + "', CatalogType = '" + StrFileType + "|" + catalogname + "'");

            if (File.Exists(StrFileXml) && ((!mostRecentOnly && AllowTraktSync) || (!traktOnly && AllowRecentlyAddedAPI)))
            {
              var tmpconf = new MyFilmsGUI.Configuration(config, false, true, null);
              if (StrFileType != "0") tmpconf.EnhancedWatchedStatusHandling = true;
              //if (!_lockDict.ContainsKey(tmpconf.strFileXml))_lockDict.Add(tmpconf.strFileXml, new ReaderWriterLockSlim());
              //_lockDict["string"].EnterWriteLock();
              DataLock.EnterReadLock();
              try
              {
                // dataExport.ReadXml(tmpconf.strFileXml);
                const int numberOfTries = 8;
                const int timeIntervalBetweenTries = 250;
                var tries = 0;
                while (true)
                {
                  try
                  {
                    using (var fs = new FileStream(tmpconf.StrFileXml, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                      foreach (DataTable dataTable in dataExport.Tables) dataTable.BeginLoadData(); // dataTable.Rows.Clear();
                      dataExport.ReadXml(fs);
                      foreach (DataTable dataTable in dataExport.Tables) dataTable.EndLoadData();
                      fs.Close();
                    }
                    break;
                  }
                  catch (IOException e)
                  {
                    if (!Helper.IsFileLocked(e)) throw;
                    if (++tries > numberOfTries)
                    {
                      // throw new MyCustomException("The file is locked too long: " + e.Message, e);
                      LogMyFilms.Error("GetMoviesGlobal() - File locked too long ! Returning error !");
                      break;
                    }
                    LogMyFilms.Debug("LoadMyFilmsFromDisk() - File '" + tmpconf.StrFileXml + "' is locked on try '" + tries + "' of '" + numberOfTries + "' - waiting another '" + timeIntervalBetweenTries + "' ms.");
                    Thread.Sleep(timeIntervalBetweenTries);
                  }
                }
              }
              catch (Exception e)
              {
                LogMyFilms.Error("GetMoviesGlobal() - Error reading xml database after " + dataExport.Movie.Count.ToString() + " records; error : " + e.Message + ", " + e.StackTrace);
              }
              finally
              {
                DataLock.ExitReadLock();
              }

              try
              {
                string sqlExpression = (!string.IsNullOrEmpty(expression)) ? expression : tmpconf.StrDfltSelect + tmpconf.StrTitle1 + " not like ''";
                string sqlSort = (!string.IsNullOrEmpty(sort)) ? sort : "OriginalTitle" + " " + "ASC";
                DataRow[] results = dataExport.Tables["Movie"].Select(sqlExpression, sqlSort);
                IEnumerable<DataColumn> commonColumns = dataExport.Movie.Columns.OfType<DataColumn>().Intersect(dataExport.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer()).Where(x => x.ColumnName != "Movie_Id").ToList();

                foreach (AntMovieCatalog.MovieRow sr in results)
                {
                  #region copy customfields
                  AntMovieCatalog.CustomFieldsRow customFields = null;
                  if (sr.GetCustomFieldsRows().Length > 0)
                  {
                    customFields = sr.GetCustomFieldsRows()[0]; // Relations["Movie_CustomFields"]
                    foreach (DataColumn dc in commonColumns)
                    {
                      sr[dc.ColumnName] = customFields[dc.ColumnName];
                    }
                  }
                  #endregion

                  try
                  {
                    var movie = new MFMovie { Config = config, Username = StrUserProfileName }; // MF config context
                    GetMovieDetails(sr, tmpconf, (!traktOnly && tmpconf.AllowRecentlyAddedApi), ref movie);
                    moviesGlobal.Add(movie);
                  }
                  catch (Exception mex)
                  {
                    LogMyFilms.Error("MyFilms videodatabase: add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
                  }
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.Error("MyFilms videodatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
              }
            }
          }
        }
      }
      return moviesGlobal;
    }

    internal static void GetMovieDetails(DataRow row, MyFilmsGUI.Configuration tmpconf, bool getDataRowDetailsForArtwork, ref MFMovie movie)
    {
      movie.ReadOnly = tmpconf.ReadOnly; // is true for readonly catalog types

      #region Number
      movie.ID = !string.IsNullOrEmpty(row["Number"].ToString()) ? Int32.Parse(row["Number"].ToString()) : 0;
      #endregion

      #region year
      int year = 1900;
      Int32.TryParse(row["Year"].ToString(), out year);
      movie.Year = year;
      #endregion

      #region Category
      movie.Category = row["Category"].ToString();
      #endregion

      #region length
      int length = 0;
      Int32.TryParse(row["Length"].ToString(), out length);
      movie.Length = length;
      #endregion

      #region OriginalTitle & TranslatedTitle & FormattedTitle
      movie.Title = row["OriginalTitle"].ToString();
      if (row[tmpconf.StrTitle1].ToString().Length > 0)
      {
        int titlePos = (row[tmpconf.StrTitle1].ToString().LastIndexOf(tmpconf.TitleDelim, System.StringComparison.Ordinal) > 0) ? row[tmpconf.StrTitle1].ToString().LastIndexOf(tmpconf.TitleDelim, System.StringComparison.Ordinal) + 1 : 0; //only display rest of title after selected part common to group
        movie.Title = row[tmpconf.StrTitle1].ToString().Substring(titlePos);
        movie.GroupName = (titlePos > 1) ? row[tmpconf.StrTitle1].ToString().Substring(titlePos - 1) : "";
      }
      if (Helper.FieldIsSet(tmpconf.StrTitle2))
      {
        int titlePos = (row[tmpconf.StrTitle2].ToString().LastIndexOf(tmpconf.TitleDelim, System.StringComparison.Ordinal) > 0) ? row[tmpconf.StrTitle2].ToString().LastIndexOf(tmpconf.TitleDelim, System.StringComparison.Ordinal) + 1 : 0;
        movie.TranslatedTitle = row[tmpconf.StrTitle2].ToString().Substring(titlePos);
      }
      if (getDataRowDetailsForArtwork)
      {
        movie.FormattedTitle = row["FormattedTitle"].ToString();
      }
      #endregion

      #region Edition
      movie.Edition = row["Edition"].ToString();
      #endregion

      #region internet site rating
      float rating = 0;
      if (!(float.TryParse(row["Rating"].ToString(), out rating))) rating = 0;
      movie.Rating = rating;
      #endregion

      #region watched status & user rating
      if (tmpconf.EnhancedWatchedStatusHandling)
      {
        MultiUserData multiUserData;
        if (row[MultiUserStateField] == System.Convert.DBNull) // not yet migrated - do it now
        {
          #region migration code for watched state - migrate status from configured (enhanced or standard)watched field to new MultiUserStates
          if (row[tmpconf.StrWatchedField].ToString().Contains(":"))
          {
            #region old field was already multiuserdata - use it!
            multiUserData = new MultiUserData(row[tmpconf.StrWatchedField].ToString());
            #endregion
          }
          else
          {
            #region old field was standard watched data - create MUS, add watched for current user and use it
            bool tmpwatched = (!string.IsNullOrEmpty(tmpconf.GlobalUnwatchedOnlyValue) &&
                          row[tmpconf.StrWatchedField].ToString().ToLower() != tmpconf.GlobalUnwatchedOnlyValue.ToLower() &&
                          row[tmpconf.StrWatchedField].ToString().Length > 0);
            multiUserData = new MultiUserData("");
            multiUserData.SetWatched(tmpconf.StrUserProfileName, tmpwatched);
            #endregion
          }
          #endregion
        }
        else // use existiung MUS data
        {
          multiUserData = new MultiUserData(row[MultiUserStateField].ToString());
        }
        UserState user = multiUserData.GetUserState(tmpconf.StrUserProfileName);
        movie.Watched = user.Watched;
        movie.WatchedCount = user.WatchedCount;
        movie.RatingUser = (float)user.UserRating;
        movie.WatchedCount = user.WatchedCount;

        //// check, if the direct DB fields have updated values - if not, post them, so they will get an update.
        //bool played = false;
        //if (tmpconf.GlobalUnwatchedOnlyValue != null && tmpconf.StrWatchedField.Length > 0)
        //  if (row[tmpconf.StrWatchedField].ToString().ToLower() != tmpconf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
        //    played = true;
        //movie.Watched = played;

        //rating = -1;
        //if (!(float.TryParse(row["RatingUser"].ToString(), out rating))) rating = -1;
        //movie.RatingUser = rating;
      }
      else
      {
        bool played = false;
        if (tmpconf.GlobalUnwatchedOnlyValue != null && tmpconf.StrWatchedField.Length > 0)
          if (row[tmpconf.StrWatchedField].ToString().ToLower() != tmpconf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
            played = true;
        movie.Watched = played;
        movie.WatchedCount = -1; // check against it, if value returns...

        rating = -1;
        if (!(float.TryParse(row["RatingUser"].ToString(), out rating))) rating = -1;
        movie.RatingUser = rating;
      }
      #endregion

      #region mediapath
      string mediapath = string.Empty;
      if (Helper.FieldIsSet(tmpconf.StrStorage))
      {
        mediapath = row[tmpconf.StrStorage].ToString();
        if (mediapath.Contains(";")) // take the first source file
          mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
        movie.File = mediapath;
      }
      if (string.IsNullOrEmpty(mediapath)) // e.g. offline media files
        movie.File = movie.Title + " {offline} [" + movie.ID + "]";

      string path = "";
      if (!string.IsNullOrEmpty(mediapath))
      {
        try
        {
          path = Path.GetDirectoryName(mediapath);
        }
        catch (Exception)
        {
          movie.Path = "{search}";
        }
        movie.Path = path;
      }
      else
        movie.Path = "{offline}";
      #endregion

      #region imdb
      string imdb = "";
      if (!string.IsNullOrEmpty(row["IMDB_Id"].ToString()))
        imdb = row["IMDB_Id"].ToString();
      else if (!string.IsNullOrEmpty(row["URL"].ToString()))
      {
        string urLstring = row["URL"].ToString();
        var cutText = new Regex("" + @"tt\d{7}" + "");
        var m = cutText.Match(urLstring);
        if (m.Success) imdb = m.Value;
      }
      movie.IMDBNumber = imdb;
      #endregion

      #region tmdb
      movie.TMDBNumber = row["TMDB_Id"].ToString();
      #endregion

      #region dateadded
      movie.DateAdded = row["Date"].ToString();
      try
      {
        DateTime wdate = Convert.ToDateTime(row["Date"]); //  = new DateTime(1900, 01, 01)
        movie.DateTime = wdate;
      }
      catch { }
      #endregion

      #region Trakt Categories
      movie.CategoryTrakt.Clear();
      if (!string.IsNullOrEmpty(row["CategoryTrakt"].ToString()))
        movie.CategoryTrakt = row["CategoryTrakt"].ToString().Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList(); //row["CategoryTrakt"].ToString();
      #endregion

      #region Cover
      if (getDataRowDetailsForArtwork)
      {
        movie.Picture = row["Picture"].ToString();
      }
      #endregion
    }

    private static void GetMovieArtworkDetails(ref MFMovie movie) // DataRow row, 
    {
      var tempconf = new MyFilmsGUI.Configuration(movie.Config, false, true, null);
      try
      {
        #region Cover
        string pictureFile;
        if (movie.Picture.Length > 0)
        {
          if ((movie.Picture.IndexOf(":\\", System.StringComparison.Ordinal) == -1) && (movie.Picture.Substring(0, 2) != "\\\\"))
            pictureFile = tempconf.StrPathImg + "\\" + movie.Picture;
          else
            pictureFile = movie.Picture;
        }
        else
          pictureFile = string.Empty;
        if (string.IsNullOrEmpty(pictureFile) || !File.Exists(pictureFile))
          pictureFile = tempconf.DefaultCover;
        movie.Picture = pictureFile;
        #endregion

        #region Fanart
        string fanartTitle;
        if (tempconf.StrTitle1 == "FormattedTitle") // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
        {
          if (!string.IsNullOrEmpty(movie.TranslatedTitle))
            fanartTitle = movie.TranslatedTitle;
          else if (!string.IsNullOrEmpty(movie.Title))
            fanartTitle = movie.Title;
          else
            fanartTitle = movie.FormattedTitle;
        }
        else
        {
          if (tempconf.StrTitle1 == "TranslatedTitle" && movie.TranslatedTitle.Length > 0)
            fanartTitle = movie.TranslatedTitle;
          else if (!string.IsNullOrEmpty(movie.Title))
            fanartTitle = movie.Title;
          else if (!string.IsNullOrEmpty(movie.FormattedTitle))
            fanartTitle = movie.FormattedTitle;
          else fanartTitle = "";
        }
        movie.Fanart = MyFilmsDetail.Search_Fanart(fanartTitle, false, "file", false, pictureFile, string.Empty, tempconf)[0];
        #endregion
      }
      catch (Exception ex)
      {
        LogMyFilms.DebugException("GetMovieArtworkDetails() - Error: " + ex.Message, ex);
      }
    }

    private static ArrayList BaseSearchString(string champselect, string[] listSeparator, string[] roleSeparator)
    {
      var oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      var oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
      {
        var oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty));
      }
      var wtab = new ArrayList();

      int wi;
      string[] sep = listSeparator;
      string[] arSplit = champselect.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        if (arSplit[wi].Length > 0)
        {
          // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary !
          string wzone = arSplit[wi].Replace("  ", " ").Trim();
          for (int i = 0; i <= 4; i++)
          {
            if (roleSeparator[i].Length > 0)
            {
              int wzoneIndexPosition = wzone.IndexOf(roleSeparator[i], StringComparison.Ordinal);
              if (wzoneIndexPosition == wzone.Length - 1)
              {
                wzone = string.Empty;
                break;
              }
              if (wzoneIndexPosition > 1 && wzoneIndexPosition < wzone.Length)
              {
                wzone = wzone.Substring(0, wzoneIndexPosition).Trim();
              }
            }
          }
          if (wzone.Length > 0)
            wtab.Add(wzone);
        }
      }
      return wtab;
    }

    /// <summary>
    /// Updates a list of movies for a given config
    /// </summary>
    /// <param name="config">MyFilms config name</param>
    /// <param name="movielist">List of movies to update in chosen config</param>
    static void UpdateMovies(string config, List<MFMovie> movielist)
    {
      LogMyFilms.Debug("UpdateMovies() - called for config = '" + config + "' with '" + movielist.Count + "' movies to update");
      if (movielist.Count == 0) return;

      using (var dataImport = new AntMovieCatalog())
      {
        using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
        {
          #region variables
          IEnumerable<DataColumn> commonColumns = dataImport.Movie.Columns.OfType<DataColumn>().Intersect(dataImport.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer()).Where(x => x.ColumnName != "Movie_Id").ToList();
          string Catalog = xmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
          string CatalogTmp = xmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalogTemp", string.Empty);
          string FileType = xmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
          bool TraktEnabled = xmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
          string StrDfltSelect = xmlConfig.ReadXmlConfig("MyFilms", config, "strDfltSelect", string.Empty);
          bool EnhancedWatchedStatusHandling = xmlConfig.ReadXmlConfig("MyFilms", config, "EnhancedWatchedStatusHandling", false);
          string GlobalUnwatchedOnlyValue = xmlConfig.ReadXmlConfig("MyFilms", config, "GlobalUnwatchedOnlyValue", "false");
          string WatchedField = xmlConfig.ReadXmlConfig("MyFilms", config, "WatchedField", "Checked");
          string UserProfileName = xmlConfig.ReadXmlConfig("MyFilms", config, "UserProfileName", "");
          var saveDataWatch = new Stopwatch();
          #endregion

          #region sanity checks
          if (FileType != "0" && FileType != "10")
          {
            if (!string.IsNullOrEmpty(CatalogTmp))
              Catalog = CatalogTmp;
            else if (!string.IsNullOrEmpty(Catalog) && Catalog.Contains("\\"))
            {
              string Path = System.IO.Path.GetDirectoryName(Catalog);
              Catalog = Path + "\\" + Catalog.Substring(Catalog.LastIndexOf(@"\") + 1, Catalog.Length - Catalog.LastIndexOf(@"\") - 5) + "_tmp.xml";
            }
            else
            {
              LogMyFilms.Debug("Catalog Type is readonly (EC) - tmp-Catalog not found - Update rejected ! - Config = '" + config + "', Catalogfile = '" + Catalog + "'");
              return;
            }
          }
          LogMyFilms.Debug("TraktSync = '" + TraktEnabled + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");

          if (!File.Exists(Catalog)) return;

          if (!TraktEnabled)
          {
            LogMyFilms.Debug("Trakt not enabled for this config - Update rejected ! - Config = '" + config + "', Catalogfile = '" + Catalog + "'");
            return;
          }
          #endregion

          #region save changes via memory update to disk
          const int maxretries = 10; // max retries 10 * 3000 = 30 seconds
          int i = 0;
          bool success = false; // result of update operation

          while (!success && i < maxretries)
          {
            if (!MyFilmsDetail.GlobalLockIsActive(Catalog))
            {
              MyFilmsDetail.SetGlobalLock(true, Catalog);
              if (DataLock.TryEnterWriteLock(10000))
              {
                try
                {
                  #region read xml file from disk to memory
                  saveDataWatch.Reset(); saveDataWatch.Start();
                  // dataImport.Clear();
                  using (var fs = new FileStream(Catalog, FileMode.Open, FileAccess.Read, FileShare.Read))
                  {
                    //LogMyFilms.Debug("Commit() - opening '" + Catalog + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
                    foreach (DataTable dataTable in dataImport.Tables) dataTable.BeginLoadData();
                    dataImport.ReadXml(fs);
                    foreach (DataTable dataTable in dataImport.Tables) dataTable.EndLoadData();
                    fs.Close();
                    //LogMyFilms.Debug("Commit()- closing  '" + Catalog + "' FileStream");
                  }
                  saveDataWatch.Stop();
                  LogMyFilms.Debug("UpdateMovies() - finished loading database  ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
                  #endregion

                  saveDataWatch.Reset(); saveDataWatch.Start();
                  foreach (var movie in movielist)
                  {
                    #region update all movie records in memory
                    string movieinfo = " - MovieID = '" + movie.ID + "', Movie = '" + movie.Title + "' (" + movie.Year + "), IMDB = '" + movie.IMDBNumber + "', Watched = '" + movie.Watched + "', Rating = '" + movie.Rating + "', RatingUser = '" + movie.RatingUser + "', CategoryTrakt = '" + movie.GetStringValue(movie.CategoryTrakt) + "'";
                    DataRow[] results = dataImport.Movie.Select(StrDfltSelect + "Number" + " = " + "'" + movie.ID + "'", "OriginalTitle" + " " + "ASC");
                    // AntMovieCatalog.MovieRow[] results = dataImport.Movie.Where(m => m.Number == _mID).ToList();

                    if (results.Length != 1) LogMyFilms.Warn("UpdateMovies() - Warning - Results found: '" + results.Length + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");

                    foreach (AntMovieCatalog.MovieRow sr in results)
                    {
                      #region Copy CustomFields data ....
                      AntMovieCatalog.CustomFieldsRow customFields = null;
                      if (sr.GetCustomFieldsRows().Length == 0)
                      {
                        customFields = dataImport.CustomFields.NewCustomFieldsRow();
                        customFields.SetParentRow(sr);
                        dataImport.CustomFields.AddCustomFieldsRow(customFields);
                      }
                      else
                      {
                        customFields = sr.GetCustomFieldsRows()[0];
                        foreach (var dc in commonColumns)
                        {
                          //object temp;
                          //if (DBNull.Value != (temp = customFields[dc.ColumnName]))
                          //  sr[dc.ColumnName] = temp;
                          sr[dc.ColumnName] = customFields[dc.ColumnName];
                        }
                      }
                      #endregion

                      #region CategoryTrakt
                      if (sr.IsCategoryTraktNull() || sr.CategoryTrakt != movie.GetStringValue(movie.CategoryTrakt))
                      {
                        LogMyFilms.Debug("UpdateMovies() - Updating Field 'CategoryTrakt' from '" + (sr.IsCategoryTraktNull() ? "" : sr.CategoryTrakt) + "' to '" + movie.GetStringValue(movie.CategoryTrakt) + "'" + movieinfo);
                        sr.CategoryTrakt = movie.GetStringValue(movie.CategoryTrakt);
                      }
                      #endregion

                      #region watched status and user rating
                      if (EnhancedWatchedStatusHandling)
                      {
                        #region update multi user status for current trakt user (should be same as myfilms user, but not necessarily)
                        MultiUserData multiUserData;
                        if (sr.IsMultiUserStateNull() || sr.MultiUserState.Length == 0) // not yet migrated - do it now
                        {
                          #region migration code for watched state - migrate status from configured (enhanced or standard)watched field to new MultiUserStates
                          if (sr[WatchedField].ToString().Contains(":"))
                          {
                            #region old field was already multiuserdata - migrate it!
                            multiUserData = new MultiUserData(sr[WatchedField].ToString());
                            sr[WatchedField] = multiUserData.GetUserState(UserProfileName).Watched ? "true" : GlobalUnwatchedOnlyValue.ToLower();
                            #endregion
                          }
                          else
                          {
                            #region old field was standard watched data - create MUS and add watched for current user
                            bool tmpwatched = (!string.IsNullOrEmpty(GlobalUnwatchedOnlyValue) &&
                                          sr[WatchedField].ToString().ToLower() != GlobalUnwatchedOnlyValue.ToLower() &&
                                          sr[WatchedField].ToString().Length > 0);
                            multiUserData = new MultiUserData("");
                            multiUserData.SetWatched(UserProfileName, tmpwatched);
                            if (sr["RatingUser"] != Convert.DBNull)
                              multiUserData.SetRating(UserProfileName, (decimal)sr["RatingUser"]);
                            #endregion
                          }
                          sr.MultiUserState = multiUserData.ResultValueString();
                          sr["DateWatched"] = multiUserData.GetUserState(MyFilms.conf.StrUserProfileName).WatchedDate;
                          sr["RatingUser"] = multiUserData.GetUserState(MyFilms.conf.StrUserProfileName).UserRating == MultiUserData.NoRating ? Convert.DBNull : multiUserData.GetUserState(MyFilms.conf.StrUserProfileName).UserRating;
                          #endregion
                        }
                        else
                        {
                          // use existiung MUS data
                          multiUserData = new MultiUserData(sr.MultiUserState);
                        }

                        // now update Trakt update requests to MUS data
                        var user = multiUserData.GetUserState(movie.Username);
                        string oldEnhancedWatchedValue = multiUserData.ResultValueString();
                        if (user.Watched != movie.Watched)
                        {
                          LogMyFilms.Debug("UpdateMovies() - Updating 'Watched' from '" + user.Watched + "' to '" + movie.Watched + "'" + movieinfo);
                          user.Watched = movie.Watched;
                          multiUserData.SetWatched(movie.Username, movie.Watched);
                        }
                        if (user.WatchedCount != movie.WatchedCount)
                        {
                          LogMyFilms.Debug("UpdateMovies() - Updating 'WatchedCount' from '" + user.WatchedCount + "' to '" + movie.WatchedCount + "'" + movieinfo);
                          user.WatchedCount = movie.WatchedCount;
                          multiUserData.SetWatchedCount(movie.Username, movie.WatchedCount);
                        }

                        if (user.UserRating != Convert.ToDecimal(movie.RatingUser))
                        {
                          LogMyFilms.Debug("UpdateMovies() - Updating 'UserRating' from '" + user.UserRating.ToString() + "' to '" + Convert.ToDecimal(movie.RatingUser).ToString() + "'" + movieinfo);
                          multiUserData.SetRating(movie.Username, Convert.ToDecimal(movie.RatingUser));
                        }
                        string newEnhancedWatchedValue = multiUserData.ResultValueString();
                        sr.MultiUserState = newEnhancedWatchedValue;
                        if (oldEnhancedWatchedValue != newEnhancedWatchedValue) LogMyFilms.Debug("UpdateMovies() - Updating 'MultiUserState' from '" + oldEnhancedWatchedValue + "' to '" + newEnhancedWatchedValue + "', WatchedCount = '" + movie.WatchedCount + "', (user)Rating = '" + movie.RatingUser + "', (site)Rating = '" + movie.Rating + "', User = '" + movie.Username + "'");
                        #endregion

                        #region  if updates are for current active user, update direct DB fields also!
                        if (movie.Username == UserProfileName)
                        {
                          // LogMyFilms.Debug("UpdateMovies() - Trakt updates are for current MyFilms user '" + movie.Username + "' - also updating DB fields directly");
                          sr["DateWatched"] = (user.WatchedDate == MultiUserData.NoWatchedDate || user.Watched == false) ? System.Convert.DBNull : user.WatchedDate;
                          sr["RatingUser"] = user.UserRating == MultiUserData.NoRating ? System.Convert.DBNull : user.UserRating;
                          if (WatchedField.Length > 0) sr[WatchedField] = user.Watched ? "true" : GlobalUnwatchedOnlyValue;
                          if (UserProfileName.Length > 0 && sr["RatingUser"] != System.Convert.DBNull && sr.RatingUser != MultiUserData.NoRating)
                          {
                            if (sr.RatingUser > MultiUserData.FavoriteRating)
                            {
                              LogMyFilms.Debug("UpdateMovies() - Adding user '" + UserProfileName + "' to 'Favorite' field (rating = '" + (sr.IsRatingUserNull() ? "null" : sr.RatingUser.ToString()) + "')." + movieinfo);
                              string newValue = MultiUserData.Add(sr["Favorite"].ToString(), UserProfileName);
                              sr["Favorite"] = (string.IsNullOrEmpty(newValue)) ? Convert.DBNull : newValue;
                              // sr.Favorite = MultiUserData.Add(sr["Favorite"].ToString(), UserProfileName);
                            }
                            else
                            {
                              LogMyFilms.Debug("UpdateMovies() - Remove user '" + UserProfileName + "' from 'Favorite' field (rating = '" + (sr.IsRatingUserNull() ? "null" : sr.RatingUser.ToString()) + "')." + movieinfo);
                              string newValue = MultiUserData.Remove(sr["Favorite"].ToString(), UserProfileName);
                              sr["Favorite"] = (string.IsNullOrEmpty(newValue)) ? Convert.DBNull : newValue;
                              // if (!sr.IsFavoriteNull()) sr.Favorite = MultiUserData.Remove(sr.Favorite, UserProfileName);
                            }
                          }
                        }
                        else
                        {
                          LogMyFilms.Warn("UpdateMovies() - Trakt updates are for user '" + movie.Username + "', but current MyFilms user is '" + UserProfileName + "' - no direct updates to DB fields - only updating MUS!");
                        }
                        #endregion
                      }
                      else
                      {
                        #region old non MUS handling for watched and user rating (AMC3.x)
                        // watched
                        string oldWatchedString = sr[WatchedField].ToString();
                        if (sr[WatchedField].ToString().ToLower() != oldWatchedString.ToLower())
                          LogMyFilms.Debug("UpdateMovies() - Updating Field '" + WatchedField + "' from '" + oldWatchedString + "' to '" + sr[WatchedField] + "'");
                        sr[WatchedField] = movie.Watched ? "true" : GlobalUnwatchedOnlyValue;

                        // user rating
                        if (movie.RatingUser > -1)
                        {
                          LogMyFilms.Debug("UpdateMovies() - Updating 'RatingUser' from '" + (sr.IsRatingUserNull() ? -1 : sr.RatingUser).ToString() + "' to '" + Convert.ToDecimal(movie.RatingUser).ToString() + "'");
                          sr.RatingUser = Convert.ToDecimal(movie.RatingUser);
                        }
                        #endregion
                      }
                      #endregion

                      #region imdb number
                      string oldImdb = (sr.IsIMDB_IdNull() ? "" : sr.IMDB_Id);
                      string newImdb = (!string.IsNullOrEmpty(movie.IMDBNumber) ? movie.IMDBNumber : "");
                      sr.IMDB_Id = newImdb;
                      if (newImdb != oldImdb)
                        LogMyFilms.Debug("UpdateMovies() - Updating 'IMDB_Id' from '" + oldImdb + "' to '" + newImdb + "'");
                      #endregion

                      #region tmdb number
                      string oldTmdb = (sr.IsTMDB_IdNull() ? "" : sr.TMDB_Id);
                      string newTmdb = (!string.IsNullOrEmpty(movie.TMDBNumber) ? movie.TMDBNumber : "");
                      sr.TMDB_Id = newTmdb;
                      if (newTmdb != oldTmdb)
                        LogMyFilms.Debug("UpdateMovies() - Updating 'TMDB_Id' from '" + oldTmdb + "' to '" + newTmdb + "'");
                      #endregion

                      #region copy data to customfields ...
                      foreach (var dc in commonColumns)
                      {
                        customFields[dc.ColumnName] = sr[dc.ColumnName];
                      }
                      #endregion
                    }
                    #endregion
                  }
                  saveDataWatch.Stop();
                  LogMyFilms.Debug("UpdateMovies() - finished updating requests ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");

                  #region check for inconsistencies in config and correct if possible
                  var allmovies = dataImport.Movie.Select().ToList();
                  int totalcount = allmovies.Count;
                  saveDataWatch.Reset(); saveDataWatch.Start();
                  LogMyFilms.Info("UpdateMovies() - Check and correct inconsistencies for '" + totalcount + "' movies, Config = '" + config + "', Catalogfile = '" + Catalog + "'");

                  foreach (AntMovieCatalog.MovieRow sr in allmovies)
                  {
                    #region Copy CustomFields data ....
                    AntMovieCatalog.CustomFieldsRow customFields = null;
                    if (sr.GetCustomFieldsRows().Length == 0)
                    {
                      customFields = dataImport.CustomFields.NewCustomFieldsRow();
                      customFields.SetParentRow(sr);
                      dataImport.CustomFields.AddCustomFieldsRow(customFields);
                    }
                    customFields = sr.GetCustomFieldsRows()[0]; // Relations["Movie_CustomFields"]
                    foreach (var dc in commonColumns)
                    {
                      //object temp;
                      //if (DBNull.Value != (temp = customFields[dc.ColumnName]))
                      //  sr[dc.ColumnName] = temp;
                      sr[dc.ColumnName] = customFields[dc.ColumnName];
                    }
                    #endregion

                    #region sync MUS state with direct DB fields for user rating, watched and Favorite (only when MUS already migrated and DB field exists)
                    if (EnhancedWatchedStatusHandling && sr[MultiUserStateField] != System.Convert.DBNull)
                    {
                      var states = new MultiUserData(sr.MultiUserState);
                      var user = states.GetUserState(UserProfileName);
                      sr["DateWatched"] = (user.WatchedDate == MultiUserData.NoWatchedDate || user.Watched == false) ? System.Convert.DBNull : user.WatchedDate;
                      sr["RatingUser"] = user.UserRating == -1 ? System.Convert.DBNull : user.UserRating;
                      if (WatchedField.Length > 0) sr[WatchedField] = user.Watched ? "true" : GlobalUnwatchedOnlyValue;
                      if (UserProfileName.Length > 0 && sr["RatingUser"] != System.Convert.DBNull && sr.RatingUser != MultiUserData.NoRating)
                      {
                        if (sr.RatingUser > MultiUserData.FavoriteRating)
                        {
                          string newValue = MultiUserData.Add(sr["Favorite"].ToString(), UserProfileName);
                          sr["Favorite"] = (string.IsNullOrEmpty(newValue)) ? Convert.DBNull : newValue;
                          // sr.Favorite = MultiUserData.Add(sr["Favorite"].ToString(), UserProfileName);
                        }
                        else
                        {
                          string newValue = MultiUserData.Remove(sr["Favorite"].ToString(), UserProfileName);
                          sr["Favorite"] = (string.IsNullOrEmpty(newValue)) ? Convert.DBNull : newValue;
                          // if (!sr.IsFavoriteNull()) sr.Favorite = MultiUserData.Remove(sr.Favorite, UserProfileName);
                        }
                      }
                    }
                    #endregion

                    #region copy data to customfields ...
                    foreach (var dc in commonColumns)
                    {
                      customFields[dc.ColumnName] = sr[dc.ColumnName];
                    }
                    #endregion
                  }

                  saveDataWatch.Stop();
                  LogMyFilms.Debug("UpdateMovies() - finished consistency check ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
                  #endregion

                  #region (re)enable filesystem watcher to notify myfilms on update and disable trakt message handler
                  MyFilms.SendTraktUpdateMessage = false;
                  try
                  {
                    if (MyFilms.FSwatcher.Path.Length > 0) MyFilms.FSwatcher.EnableRaisingEvents = true; // re enable watcher - as myfilms should auto update dataset for current config, if update is done from trakt 
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.DebugException("Commit()- FSwatcher - problem enabling Raisingevents - Message: '" + ex.Message + "'", ex);
                  }
                  #endregion

                  #region write xml file
                  saveDataWatch.Reset(); saveDataWatch.Start();
                  using (FileStream fsTmp = File.Create(Catalog.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose)) // make sure, only one process is writing to file !
                  {
                    using (var fs = new FileStream(Catalog, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
                    {
                      // LogMyFilms.Debug("Commit() - opening '" + Catalog + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                      fs.SetLength(0); // do not append, owerwrite !

                      using (var myXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                      {
                        LogMyFilms.Debug("UpdateMovies() - writing '" + Catalog + "' as MyXmlTextWriter in FileStream");
                        myXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                        myXmlTextWriter.WriteStartDocument();
                        dataImport.WriteXml(myXmlTextWriter, XmlWriteMode.IgnoreSchema);
                        myXmlTextWriter.Flush();
                        myXmlTextWriter.Close();
                      }
                      fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                      // LogMyFilms.Debug("Commit() - closing '" + Catalog + "' FileStream and releasing file lock");
                      success = true;
                    }
                  }
                  Thread.Sleep(50); // give time to release file handle
                  saveDataWatch.Stop();
                  LogMyFilms.Debug("UpdateMovies() - finished saving databse ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
                  #endregion
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("UpdateMovies() - failed saving data to disk - Catalog = '" + Catalog + "' - reason: " + ex.Message + ex.StackTrace);
                  success = false;
                }
                finally
                {
                  dataImport.Clear();
                  MyFilmsDetail.SetGlobalLock(false, Catalog);
                  DataLock.ExitWriteLock();
                }
              }
              else
              {
                LogMyFilms.Debug("UpdateMovies() - timeout when waiting for slim writelock - could not write data !");
              }
            }
            if (success)
            {
              continue;
            }
            i += 1;
            LogMyFilms.Info("UpdateMovies() - Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
            Thread.Sleep(3000);
          }
          if (!success) LogMyFilms.Error("UpdateMovies() - Error writing Movie Database after '" + maxretries + "' retries !");
          #endregion

          MyFilmsDetail.SetGlobalLock(false, Catalog);
        }
        dataImport.Clear(); // if (dataImport != null) dataImport.Dispose();
      }
    }
    #endregion

    #region accesseurs
    public static DataRow[] FilmsSelected
    {
      get { return movies; }
    }
    #endregion

    #region Public static methods ...

    public static DataRow[] ReadDataMovies(string strDfltSelect, string strSelect, string strSort, string strSortSens, bool all = false, bool doextrasort = false)
    {
      // LogMyFilms.Debug("ReadDataMovies() - strDfltSelect            = '" + strDfltSelect + "'");
      // LogMyFilms.Debug("ReadDataMovies() - strSelect                = '" + strSelect + "'");
      // LogMyFilms.Debug("ReadDataMovies() - strSort/strSortSens, all = '" + strSort + "/" + strSortSens + "', '" + all + "', cached = '" + (data != null) + "'");
      // LogMyFilms.Debug("ReadDataMovies() - Expression               = '" + strDfltSelect + strSelect + "|" + strSort + " " + strSortSens + "', all = '" + all + "', extrasort = '" + doextrasort + "', cached = '" + (data != null) + "'");
      LogMyFilms.Debug("ReadDataMovies() - Started with Expression = '" + strDfltSelect + strSelect + "|" + strSort + " " + strSortSens + "', all = '" + all + "', extrasort = '" + doextrasort + "', cached = '" + (data != null) + "'");
      var watchReadMovies = new Stopwatch(); watchReadMovies.Reset(); watchReadMovies.Start();

      if (strSelect.Length == 0) strSelect = MyFilms.conf.StrTitle1 + " not like ''";

      if (data == null) InitData();

      DataLock.EnterReadLock();
      try
      {
        // DB field replacements for sorting - currently only used for "Date" - in the future might be used for "DateFile" and "DateWatched" too ...
        switch (strSort)
        {
          case "Date":
            strSort = "DateAdded";
            LogMyFilms.Debug("ReadDataMovies() - Sort field replacement: Date -> DateAdded");
            break;
        }
        movies = data.Movie.Select(strDfltSelect + strSelect, strSort + " " + strSortSens);
        if (movies.Length == 0 && all)
        {
          strSelect = MyFilms.conf.StrTitle1 + " not like ''";
          LogMyFilms.Debug("ReadDataMovies() - Switching to full list ...");
          movies = data.Movie.Select(strDfltSelect + strSelect, strSort + " " + strSortSens);
        }
        #region Additional sorting ...
        if (doextrasort)
        {
          var watchReadMoviesSort = new Stopwatch(); watchReadMoviesSort.Reset(); watchReadMoviesSort.Start();
          MyFilms.FieldType fieldType = MyFilms.GetFieldType(strSort);
          Type columnType = MyFilms.GetColumnType(strSort);
          string strColumnType = (columnType == null) ? "<invalid>" : columnType.ToString();

          if (!string.IsNullOrEmpty(strSort) && columnType == typeof(string)) // don't apply special sorting on "native" types - only on string types !
          {
            LogMyFilms.Debug("ReadDataMovies() - sorting fieldtype = '" + fieldType + "', vartype = '" + strColumnType + "', sortfield = '" + strSort + "', sortascending = '" + strSortSens + "'");
            Watch.Reset(); Watch.Start();
            switch (fieldType)
            {
              case MyFilms.FieldType.AlphaNumeric:
                if (strSortSens == " ASC")
                {
                  IComparer myComparer = new MyFilms.AlphanumComparatorFast();
                  Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[strSort], b[strSort]));
                }
                else
                {
                  IComparer myComparer = new MyFilms.myReverserAlphanumComparatorFast();
                  Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[strSort], b[strSort]));
                  //r.Reverse();
                }
                break;
              #region Date and Decimal types are never used, as we do additional sorting for alphanumeric (string) values only !
              case MyFilms.FieldType.Date:
                if (strSortSens == " ASC")
                {
                  IComparer myComparer = new MyFilms.myDateComparer();
                  Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[strSort], b[strSort]));
                }
                else
                {
                  IComparer myComparer = new MyFilms.myDateReverseComparer();
                  Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[strSort], b[strSort]));
                  //IComparer myComparer = new myDateComparer();
                  //Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(b[strSort], a[strSort]));
                }
                break;
              case MyFilms.FieldType.Decimal:
                if (strSortSens == " ASC")
                {
                  IComparer myComparer = new MyFilms.myRatingComparer();
                  Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[strSort], b[strSort]));
                }
                else
                {
                  IComparer myComparer = new MyFilms.myRatingComparer();
                  Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(b[strSort], a[strSort]));
                  //r.Reverse();
                }
                break;
              #endregion
            }
            LogMyFilms.Debug("ReadDataMovies() - additional sorting finished (" + (watchReadMoviesSort.ElapsedMilliseconds) + " ms)");
          }
          else
          {
            LogMyFilms.Debug("ReadDataMovies() - additional sorting skipped - sorting fieldtype = '" + fieldType + "', vartype = '" + strColumnType + "', sortfield = '" + strSortSens + "', sortascending = '" + strSort + "'");
          }
        }
        #endregion
      }
      finally
      {
        DataLock.ExitReadLock();
      }
      watchReadMovies.Stop();

      LogMyFilms.Debug("ReadDataMovies() - Finished ...  returning '" + movies.Length + "' movies (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");
      return movies;
    }

    public static void LoadMyFilms(string strFileXml)
    {
      if (!File.Exists(strFileXml))
      {
        LogMyFilms.Error(string.Format("LoadMyFilms() - the DB file {0} does not exist !", strFileXml));
        throw new Exception(string.Format("The DB file {0} does not exist !", strFileXml));
      }
      var watchReadMovies = new Stopwatch(); watchReadMovies.Reset(); watchReadMovies.Start();
      var success = LoadMyFilmsFromDisk(strFileXml);
      watchReadMovies.Stop();
      LogMyFilms.Debug("LoadMyFilms() - Finished ... (success = '" + success + "') (" + watchReadMovies.ElapsedMilliseconds + " ms)");
    }

    public static void UnloadMyFilms()
    {
      if (data != null) data.Dispose();
    }

    public static bool SaveMyFilms(string catalogfile, int timeout)
    {
      if (data == null) return false;
      if (timeout == 0) timeout = 10000; // default is 10 secs
      LogMyFilms.Debug("TryEnterWriteLock(" + timeout + ") - CurrentReadCount = '" + DataLock.CurrentReadCount + "', RecursiveReadCount = '" + DataLock.RecursiveReadCount + "', RecursiveUpgradeCount = '" + DataLock.RecursiveUpgradeCount + "', RecursiveWriteCount = '" + DataLock.RecursiveWriteCount + "'");
      if (DataLock.TryEnterWriteLock(timeout))
      {
        LogMyFilms.Debug("TryEnterWriteLock successful! - CurrentReadCount = '" + DataLock.CurrentReadCount + "', RecursiveReadCount = '" + DataLock.RecursiveReadCount + "', RecursiveUpgradeCount = '" + DataLock.RecursiveUpgradeCount + "', RecursiveWriteCount = '" + DataLock.RecursiveWriteCount + "'");
        bool success;
        try
        {
          success = SaveMyFilmsToDisk(catalogfile);
        }
        catch (Exception ex)
        {
          success = false;
          LogMyFilms.DebugException("SaveMyFilms() - error saving data - exception: '" + ex.Message + "'", ex);
          // throw;
        }
        finally
        {
          DataLock.ExitWriteLock();
        }

        return success;
      }
      LogMyFilms.Info("SaveMyFilms() - Movie Database could not get slim writelock for '" + timeout + "' ms - returning 'false'");
      return false;
    }

    internal static void CancelMyFilms()
    {
      StopBackgroundWorker();
      
      LogMyFilms.Debug("CancelMyFilms() - disposing data ...");
      if (data != null) data.Clear();
    }

    internal static void StopBackgroundWorker()
    {
      #region stop trailer thread downloader
      MyFilms.bgDownloadTrailer.CancelAsync();
      for (int f = 0; f < MyFilms.maxThreads; f++) MyFilms.threadArray[f].CancelAsync();
      #endregion

      #region wait for background threads to finish before shutting down ...
      if (MyFilms.bgDownloadTrailer != null && MyFilms.bgDownloadTrailer.IsBusy)
      {
        LogMyFilms.Info("StopBackgroundWorker() - Trailer Download still active ! - waiting 120 sec. for background worker to complete ...");
        MyFilms.bgDownloadTrailerDoneEvent.WaitOne(120000);
        LogMyFilms.Info("StopBackgroundWorker() - Trailer Download in background worker thread finished");
      }

      for (int f = 0; f < MyFilms.maxThreads; f++)
      {
        if (MyFilms.threadArray[f] != null && MyFilms.threadArray[f].IsBusy)
        {
          LogMyFilms.Info("StopBackgroundWorker() - Trailer Download thread '" + f + "' still active ! - waiting 120 sec. for background worker to complete ...");
          MyFilms.threadDoneEventArray[f].WaitOne(120000);
          LogMyFilms.Info("StopBackgroundWorker() - Trailer Download in background worker thread '" + f + "' finished");
        }
      }

      if (UpdateWorker != null && UpdateWorker.IsBusy)
      {
        LogMyFilms.Info("StopBackgroundWorker() - DB updates still active ! - waiting for background worker to complete ...");
        UpdateWorkerDoneEvent.WaitOne(60000);
        LogMyFilms.Info("StopBackgroundWorker() - DB updates in background worker thread finished");
        UpdateWorkerDoneEvent.WaitOne(1000); // wait another second to finish log entries
      }
      #endregion
    }
    
    internal static void RestartBackgroundWorker()
    {
      for (int f = 0; f < MyFilms.maxThreads; f++)
      {
        if (MyFilms.threadArray[f] != null && !MyFilms.threadArray[f].IsBusy)
        {
          MyFilms.threadArray[f].RunWorkerAsync(f);
          LogMyFilms.Info("RestartBackgroundWorker() - restarted trailer background worker thread '" + f + "'");
        }
      }
    }

    public static void SaveQueueToDisk(string name, Queue<Trailer> q)
    {
      try
      {
        string file = Path.Combine(Config.GetFolder(Config.Dir.Config), ("MyFilms_Queue_" + name + ".dat"));
        if (q == null || q.Count == 0)
        {
          LogMyFilms.Info("SaveQueueToDisk() - nothing to save for queue '" + name + "' - deleteing file '" + file + "'");
          if (File.Exists(file)) File.Delete(file);
          return;
        }
        var fs = new FileStream(file, FileMode.Create, FileAccess.Write); // FileMode.OpenOrCreate, FileAccess.Write, FileShare.None
        var bf = new BinaryFormatter();
        bf.Serialize(fs, q);
        fs.Close();
        LogMyFilms.Info("SaveQueueToDisk() - saved queue '" + name + "' with '" + q.Count + "' elements to '" + file + "'");
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("SaveQueueToDisk() - error saving queue '" + name + "' to disk: " + ex.Message + ex.StackTrace);
      }
    }

    public static Queue<Trailer> LoadQueueFromDisk(string name)
    {
      var queue = new Queue<Trailer>();
      try
      {
        string file = Path.Combine(Config.GetFolder(Config.Dir.Config), ("MyFilms_Queue_" + name + ".dat"));
        if (!File.Exists(file))
        {
          LogMyFilms.Debug("LoadQueueFromDisk() - nothing to load for queue '" + name + "' - file does not exist: '" + file + "'");
        }
        else
        {
          var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
          var bf = new BinaryFormatter();
          queue = (Queue<Trailer>)bf.Deserialize(fs);
          fs.Close();
          LogMyFilms.Debug("LoadQueueFromDisk() - loaded queue '" + name + "' with '" + queue.Count + "' elements from '" + file + "'");
        }
        return queue;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("LoadQueueFromDisk() - error loading queue '" + name + "' from disk: " + ex.Message + ex.StackTrace);
      }
      return queue;
    }

    public static void GetMovies(ref ArrayList movies)
    {
      movies = GetMoviesGlobal("", "", true, false);
      LogMyFilms.Debug("GetMovies() - movies matched: '" + movies.Count + "'");
    }

    #region API for Basic Home Editors
    /// <summary>
    /// returns an array of KeyValue ViewLists and name for each config
    /// .Name = config name
    /// .ViewList = List of KeyValues with view name and pretty name
    /// </summary>        
    public static ArrayList GetConfigViewLists()
    {
      var configViewLists = new ArrayList();

      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
        var configs = new ArrayList();
        for (int i = 0; i < mesFilmsNbConfig; i++) configs.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

        foreach (string config in configs)
        {
          var viewList = new List<KeyValuePair<string, string>>();
          viewList.Clear();
          var configViewList = new MfConfig();

          string catalog = xmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
          //bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
          //bool RecentAddedAPIEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);

          if (File.Exists(catalog))
          {
            int iCustomViews = xmlConfig.ReadXmlConfig("MyFilms", config, "AntViewTotalCount", -1);
            if (iCustomViews == -1) // Customviews not yet present ... ToDo: Remove in later Version - only "migration code"
            {
              //viewList.Add(new KeyValuePair<string, string>("all", GUILocalizeStrings.Get(342)));
              viewList.Add(new KeyValuePair<string, string>("Year", GUILocalizeStrings.Get(345)));
              viewList.Add(new KeyValuePair<string, string>("Category", GUILocalizeStrings.Get(10798664)));
              viewList.Add(new KeyValuePair<string, string>("Country", GUILocalizeStrings.Get(200026)));
              viewList.Add(new KeyValuePair<string, string>("Actors", GUILocalizeStrings.Get(10798667)));
            }
            int index = 1;
            while (true) // for (int i = 1; i < iCustomViews + 1; i++)
            {
              string viewName = "", viewDisplayName = "";
              viewName = xmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewItem{0}", index), string.Empty);
              viewDisplayName = xmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewText{0}", index), string.Empty);
              if (string.IsNullOrEmpty(viewName)) break; // stop loading, if no View name is given

              if (string.IsNullOrEmpty(viewDisplayName))
                viewDisplayName = viewName;
              viewList.Add(new KeyValuePair<string, string>(viewName, viewDisplayName));
              index++;
            }
          }
          configViewList.Name = config;
          configViewList.ViewList = viewList;
          configViewLists.Add(configViewList);
        }
      }
      return configViewLists;
    }

    /// <summary>
    /// returns a string List of available values for a given config & view
    /// use GetConfigViewLists() to get valid values for config and view
    /// returns 'null' if no values can be evaluated - user should still be able to manually set a value for startparam
    /// </summary>        
    public static IEnumerable<string> GetViewListValues(string config, string view)
    {
      var values = new List<string>();
      var movies = new ArrayList();
      movies.Clear();
      var dataExport = new AntMovieCatalog();
      if (string.IsNullOrEmpty(config) || string.IsNullOrEmpty(view))
        return null;

      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        string catalog = xmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
        string[] listSeparator = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        string[] roleSeparator = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        //bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
        //bool RecentAddedAPIEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);
        string strDfltSelect = xmlConfig.ReadXmlConfig("MyFilms", config, "strDfltSelect", string.Empty);
        string strSelect = xmlConfig.ReadXmlConfig("MyFilms", config, "strSelect", string.Empty);
        string strTitle1 = xmlConfig.ReadXmlConfig("MyFilms", config, "AntTitle1", string.Empty);


        if (string.IsNullOrEmpty(strSelect))
          strSelect = strTitle1 + " not like ''";

        int j = 0;
        for (int i = 1; i <= 5; i++)
        {
          if (xmlConfig.ReadXmlConfig("MyFilms", config, "ListSeparator" + i, string.Empty).Length > 0)
          {
            listSeparator[j] = xmlConfig.ReadXmlConfig("MyFilms", config, "ListSeparator" + i, string.Empty);
            j++;
          }
        }
        j = 0;
        for (int i = 1; i <= 5; i++)
        {
          if (xmlConfig.ReadXmlConfig("MyFilms", config, "RoleSeparator" + i, string.Empty).Length > 0)
          {
            roleSeparator[j] = xmlConfig.ReadXmlConfig("MyFilms", config, "RoleSeparator" + i, string.Empty);
            j++;
          }
        }

        if (File.Exists(catalog))
        {
          string champselect = "";
          string wchampselect = "";
          var wTableau = new ArrayList();
          int wnbEnr = 0;

          DataLock.EnterReadLock();
          try
          {
            dataExport.ReadXml(catalog);
          }
          catch (Exception e)
          {
            LogMyFilms.Error(": Error reading xml database after " + dataExport.Movie.Count + " records; error : " + e.Message + ", " + e.StackTrace);
            throw e;
          }
          finally
          {
            DataLock.ExitReadLock();
          }
          DataRow[] results = dataExport.Tables["Movie"].Select(strDfltSelect + strSelect, view + " ASC");
          if (results.Length == 0) return null;

          foreach (DataRow enr in results)
          {
            try
            {
              bool isdate = (view == "Date" || view == "DateAdded");
              champselect = (isdate) ? string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]) : enr[view].ToString().Trim();

              ArrayList wtab = BaseSearchString(champselect, listSeparator, roleSeparator);
              for (int wi = 0; wi < wtab.Count; wi++)
              {
                wTableau.Add(wi.ToString().Trim());
              }
            }
            catch (Exception) { }
          }
          wTableau.Sort(0, wTableau.Count, null);

          for (int wi = 0; wi != wTableau.Count; wi++)
          {
            champselect = wTableau[wi].ToString();
            if (string.Compare(champselect, wchampselect, StringComparison.OrdinalIgnoreCase) == 0) // Are the strings equal? Then add count!
            {
              wnbEnr++; // count items of distinct property
            }
            else
            {
              if ((wnbEnr > 0) && (wchampselect.Length > 0))
              {
                values.Add(wchampselect);
              }
              wnbEnr = 1;
              wchampselect = champselect;
            }
          }

          if ((wnbEnr > 0) && (wchampselect.Length > 0))
          {
            values.Add(wchampselect);
            wnbEnr = 0;
          }
        }
      }
      return values;
    }
    #endregion

    #region API for Most Recent movies

    /// <summary>
    /// returns the most recent movies based on conditions
    /// </summary>
    /// <param name="type">most recent type</param>
    /// <param name="days">number of days to look back in database</param>
    /// <param name="limit">number of results to return</param>
    /// <param name="unwatchedOnly">only get unwatched episodes (only used with recent added type)</param>
    public static List<MFMovie> GetMostRecent(MostRecentType type, int days = 30, int limit = 3, bool unwatchedOnly = false)
    {
      string enumtype = Enum.GetName(typeof(MostRecentType), type);
      LogMyFilms.Debug("GetMostRecent() - Called with type = '" + enumtype + "', days = '" + days + "', limit = '" + limit + "', unwatchedonly = '" + unwatchedOnly + "'");

      // Create Time Span to lookup most recents
      var dateCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
      dateCompare = dateCompare.Subtract(new TimeSpan(days, 0, 0, 0, 0));
      //string date = dateCompare.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");

      // get all movies
      List<MFMovie> movielist = (from MFMovie movie in GetMoviesGlobal("", "", false, true) select movie).ToList();

      switch (type)
      {
        case MostRecentType.Added:
          // only within the specified timeframe:
          if (unwatchedOnly)
            movielist = movielist.Where(m => m.Watched == false).ToList();
          movielist = movielist.Where(m => m.DateTime > dateCompare).ToList();
          break;

        case MostRecentType.Watched:
          // get the movies that we have watched
          movielist = movielist.Where(m => m.Watched == true).ToList();
          break;
      }

      // remove films without date and sort descending by dateadded
      movielist = movielist.Where(m => (m.DateAdded ?? string.Empty) != string.Empty).OrderByDescending(x => x.DateTime).ToList();
      LogMyFilms.Debug("GetMostRecent() - result of nonlimited movies: '" + movielist.Count + "'");

      movielist = movielist.Distinct(new DistinctItemComparer()).ToList();
      LogMyFilms.Debug("GetMostRecent() - result of nonlimited movies without dupes: '" + movielist.Count + "'");

      // now apply the result count limit
      movielist = movielist.Take(limit).ToList();

      // get artwork
      var movielistwithartwork = new List<MFMovie>();
      foreach (MFMovie movie in movielist)
      {
        MFMovie tmpmovie = movie;
        GetMovieArtworkDetails(ref tmpmovie);
        movielistwithartwork.Add(tmpmovie);
      }

      if (movielistwithartwork.Count < 11)
        foreach (MFMovie movie in movielistwithartwork)
        {
          LogMyFilms.Debug("GetMostRecent() - Returning (limited): config = '" + movie.Config + "', title = '" + movie.Title + "', translatedtitle = '" + movie.TranslatedTitle + "', watched = '" + movie.Watched + "', added = '" + movie.DateAdded + "', datetime = '" + movie.DateTime.ToShortDateString() + "', length = '" + movie.Length.ToString() + "', Category = '" + movie.Category + "', cover = '" + movie.Picture + "', fanart = '" + movie.Fanart + "'");
        }
      LogMyFilms.Debug("GetMostRecent() - Returning '" + movielistwithartwork.Count + "' movies.");
      return movielistwithartwork;
    }

    class DistinctItemComparer : IEqualityComparer<MFMovie>
    {
      public bool Equals(MFMovie x, MFMovie y)
      {
        return x.Title == y.Title &&
            x.File == y.File;
      }
      public int GetHashCode(MFMovie obj)
      {
        return obj.Title.GetHashCode() ^
            obj.File.GetHashCode();
      }
    }
    #endregion

    public static bool IsValidDb(string catalogfile)
    {
      // check, if the catalog file is a vlaid db format (e.g. exclude AMC4.2 format, as we don't support it !

      //<AntMovieCatalog Format="42" Version="4.2.0 (2013-11-03)" Date="1/28/2014 3:39:00 PM">
      // <Catalog>
      //  <Properties Owner="SampleOwner" Mail="myfilms@gmail.com" Site="www.team-mediaportal.com" Description="Sample description"/>
      //  <Contents>

      bool isvaliddb = true;
      var stopwatch = new Stopwatch();
      stopwatch.Reset();
      stopwatch.Start();

      try
      {
        using (var fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          XmlReader rdr = XmlReader.Create(fs);
          while (rdr.Read())
          {
            //switch (rdr.NodeType)
            //{
            //  case XmlNodeType.Element:
            //    break;
            //  case XmlNodeType.Attribute:
            //    break;
            //  case XmlNodeType.Text:
            //    break;
            //}

            if (rdr.NodeType == XmlNodeType.Element && rdr.LocalName == "AntMovieCatalog" && rdr.HasAttributes)
            {
              for (int i = 0; i < rdr.AttributeCount; i++)
              {
                rdr.MoveToAttribute(i);
                if (rdr.Name == "Format" && rdr.HasValue)
                {
                  LogMyFilms.Debug("IsValidDB() - AMC version info found - Format = '" + rdr.Value + "'");

                  if (rdr.Value == "42")
                  {
                    LogMyFilms.Error("IsValidDB() - incompatible AMC4.2 format found!");
                    isvaliddb = false;
                  }
                  break;
                }
              }
              // rdr.MoveToElement();
              break;
            }

            // if end of header is reached, stop parsing to save time !
            if (rdr.NodeType == XmlNodeType.Element && rdr.LocalName == "Contents") // stream has reached movie content, so no info in header found
            {
              LogMyFilms.Debug("IsValidDB() - no AMC version info found!");
              break;
            }
          }
          rdr.Close();
          fs.Close();
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("IsValidDB() - Error: " + ex.Message);
      }
      stopwatch.Stop();
      LogMyFilms.Debug("IsValidDB(): db format validity check = '" + isvaliddb + "' (" + stopwatch.ElapsedMilliseconds + " ms.)");
      return isvaliddb;
    }



    internal static string TranslateColumn(string column)
    {
      if (string.IsNullOrEmpty(column)) return string.Empty;
      switch (column)
      {
        case "Number":
        case "number":
          return GUILocalizeStrings.Get(10798650);
        case "Checked":
        case "checked":
          return GUILocalizeStrings.Get(10798651);
        case "MediaLabel":
        case "medialabel":
          return GUILocalizeStrings.Get(10798652);
        case "MediaType":
        case "mediatype":
          return GUILocalizeStrings.Get(10798653);
        case "Source":
        case "source":
          return GUILocalizeStrings.Get(10798654);
        case "Date":
        case "date":
          return GUILocalizeStrings.Get(10798655);
        case "Borrower":
        case "borrower":
          return GUILocalizeStrings.Get(10798656);
        case "Rating":
        case "rating":
          return GUILocalizeStrings.Get(10798657);
        case "OriginalTitle":
        case "originaltitle":
          return GUILocalizeStrings.Get(10798658);
        case "TranslatedTitle":
        case "translatedtitle":
          return GUILocalizeStrings.Get(10798659);
        case "FormattedTitle":
        case "formattedtitle":
          return GUILocalizeStrings.Get(10798660);
        case "Director":
        case "director":
          return GUILocalizeStrings.Get(10798661);
        case "Producer":
        case "producer":
          return GUILocalizeStrings.Get(10798662);
        case "Country":
        case "country":
          return GUILocalizeStrings.Get(10798663);
        case "Category":
        case "category":
          return GUILocalizeStrings.Get(10798664);
        case "Year":
        case "year":
          return GUILocalizeStrings.Get(10798665);
        case "Length":
        case "length":
          return GUILocalizeStrings.Get(10798666);
        case "Actors":
        case "actors":
          return GUILocalizeStrings.Get(10798667);
        case "URL":
        case "url":
          return GUILocalizeStrings.Get(10798668);
        case "Description":
        case "description":
          return GUILocalizeStrings.Get(10798669);
        case "Comments":
        case "comments":
          return GUILocalizeStrings.Get(10798670);
        case "VideoFormat":
        case "videoformat":
          return GUILocalizeStrings.Get(10798671);
        case "VideoBitrate":
        case "videobitrate":
          return GUILocalizeStrings.Get(10798672);
        case "AudioFormat":
        case "audioformat":
          return GUILocalizeStrings.Get(10798673);
        case "AudioBitrate":
        case "audiobitrate":
          return GUILocalizeStrings.Get(10798674);
        case "Resolution":
        case "resolution":
          return GUILocalizeStrings.Get(10798675);
        case "Framerate":
        case "framerate":
          return GUILocalizeStrings.Get(10798676);
        case "Languages":
        case "languages":
          return GUILocalizeStrings.Get(10798677);
        case "Subtitles":
        case "subtitles":
          return GUILocalizeStrings.Get(10798678);
        case "DateAdded":
        case "dateadded":
          return GUILocalizeStrings.Get(10798679);
        case "Size":
        case "size":
          return GUILocalizeStrings.Get(10798680);
        case "Disks":
        case "disks":
          return GUILocalizeStrings.Get(10798681);
        case "Picture":
        case "picture":
          return GUILocalizeStrings.Get(10798682);
        case "Certification":
        case "certification":
          return GUILocalizeStrings.Get(10798683);
        case "Writer":
        case "writer":
          return GUILocalizeStrings.Get(10798684);
        case "Watched":
        case "watched":
          return GUILocalizeStrings.Get(10798685);
        case "DateWatched": // last seen
        case "datewatched":
          return GUILocalizeStrings.Get(10798686);
        case "IMDB_Id":
        case "imdb_id":
          return GUILocalizeStrings.Get(10798687);
        case "TMDB_Id":
        case "tmdb_id":
          return GUILocalizeStrings.Get(10798688);
        case "SourceTrailer":
        case "sourcetrailer":
          return GUILocalizeStrings.Get(10798940);
        case "TagLine":
        case "tagline":
          return GUILocalizeStrings.Get(10798941);
        case "Tags":
        case "tags":
          return GUILocalizeStrings.Get(10798942);
        case "Aspectratio":
        case "aspectratio":
          return GUILocalizeStrings.Get(10798943);
        case "RatingUser":
        case "ratinguser":
          return GUILocalizeStrings.Get(10798944);
        case "Fanart":
        case "fanart":
          return GUILocalizeStrings.Get(10798945);
        case "Studio":
        case "studio":
          return GUILocalizeStrings.Get(10798946);
        case "IMDB_Rank":
        case "imdb_rank":
          return GUILocalizeStrings.Get(10798947);
        case "IsOnline":
        case "isonline":
          return GUILocalizeStrings.Get(10798948);
        case "Edition":
        case "edition":
          return GUILocalizeStrings.Get(10798949);
        case "IsOnlineTrailer":
        case "isonlinetrailer":
          return GUILocalizeStrings.Get(10798950);
        case "Persons":
        case "persons":
          return GUILocalizeStrings.Get(10798951);
        case "CategoryTrakt":
        case "categorytrakt":
          return GUILocalizeStrings.Get(10798952);
        case "AudioChannelCount":
        case "audiochannelcount":
          return GUILocalizeStrings.Get(10798953);
        case "RecentlyAdded":
        case "recentlyadded":
          return GUILocalizeStrings.Get(10798954);
        case "IndexedTitle":
        case "indexedtitle":
          return GUILocalizeStrings.Get(10798955);
        case "AgeAdded":
        case "ageadded":
          return GUILocalizeStrings.Get(10798956);
        case "Favorite":
        case "favorite":
          return GUILocalizeStrings.Get(10798957);
        case "AlternateTitles":
        case "alternatetitles":
          return GUILocalizeStrings.Get(10798932);
        case "MultiUserState":
        case "multiusersdate":
          return GUILocalizeStrings.Get(10798930);
        case "VirtualPathTitle": // virtual path = directory path plus movie title
        case "virtualpathtitle":
          return GUILocalizeStrings.Get(10798933);

        case "Composer":
        case "composer":
          return GUILocalizeStrings.Get(10799701);
        case "DateReleased":
        case "datereleased":
          return GUILocalizeStrings.Get(10799702);
        case "Budget":
        case "budget":
          return GUILocalizeStrings.Get(10799703);
        case "Collection":
        case "collection":
          return GUILocalizeStrings.Get(10799704);
        case "CollectionImage":
        case "collectionimage":
          return GUILocalizeStrings.Get(10799705);
        case "CollectionDetails":
        case "collectiondetails":
          return GUILocalizeStrings.Get(10799706);
        case "S3D":
        case "s3d":
          return GUILocalizeStrings.Get(10799707);
        case "Votes":
        case "votes":
          return GUILocalizeStrings.Get(10799708);

        default:
          {
            // translation for "Views"
            string translation = ""; // string translation = "*** DB field translation missing ***";
            if (data != null)
            {
              foreach (AntMovieCatalog.CustomFieldRow customFieldRow in Enumerable.Where(data.CustomField, customFieldRow => customFieldRow.Tag.ToLower() == column.ToLower()))
              {
                translation = (!customFieldRow.IsNameNull()) ? customFieldRow.Name : customFieldRow.Tag;
              }
            }
            return translation;
          }
      }
    }
    #endregion
  }

  public static class DBNullableExtensions
  {
    public static object ToDbValue<T>(this T? value) where T : struct
    {
      return value.HasValue ? (object)value.Value : DBNull.Value;
    }
  }
}
