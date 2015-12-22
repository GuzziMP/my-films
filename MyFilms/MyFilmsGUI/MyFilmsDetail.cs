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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Grabber;
using Grabber.Data;
using Grabber.Util;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Playlists;
using MediaPortal.Profile;
using MediaPortal.Ripper;
using MediaPortal.Util;
using MediaPortal.Video.Database;
using MyFilmsPlugin.DataBase;
using MyFilmsPlugin.Utils;
using WatTmdb.V3;
using GrabberScript = Grabber.GrabberScript;
using GUILocalizeStrings = MyFilmsPlugin.Utils.GUILocalizeStrings;
using MediaInfo = Grabber.Util.MediaInfo;
using Thumbs = MediaPortal.Util.Thumbs;
using WakeOnLanManager = MyFilmsPlugin.Utils.WakeOnLanManager;

namespace MyFilmsPlugin.MyFilmsGUI
{
  extern alias ExternalPlugins;
  using Action = MediaPortal.GUI.Library.Action;
  using GUILocalizeStrings = GUILocalizeStrings;
  using MediaInfo = MediaInfo;
  using Utils = MediaPortal.Util.Utils;
  using WakeOnLanManager = WakeOnLanManager;

  
  /// <summary>
  /// Summary description for GUIMesFilms.
  /// </summary>
  public class MyFilmsDetail : GUIWindow
  {
    #region Descriptif zones Ecran

    #region skin controls
    enum Controls : int
    {
      //CTRL_TxtSelect = 12,
      CTRL_DummyMovieThumbsAvailable = 45,
      CTRL_PersonFacade = 50,
      CTRL_MovieThumbsFacade = 51,
      CTRL_BtnPlay = 10000,
      CTRL_BtnPlay1Description = 10001,
      CTRL_BtnPlay2Comment = 10002,
      CTRL_BtnPlay3Persons = 10003,
      CTRL_BtnPlay4TecDetails = 10004,
      CTRL_BtnPlay5ExtraDetails = 10005,
      CTRL_ViewFanart = 10099,
      CTRL_ViewMovieThumbs = 10100,
      CTRL_BtnReturn = 102,
      CTRL_BtnNext = 103,
      CTRL_BtnPrior = 104,
      CTRL_BtnLast = 105,
      CTRL_BtnFirst = 106,
      CTRL_BtnMaj = 107,
      CTRL_BtnActors = 108,
      CTRL_BtnPlayTrailer = 109,
      CTRL_BtnTraktMenu = 110,
      CTRL_Fanart = 1000,
      CTRL_FanartDir = 1001,
      CTRL_MovieThumbsDir = 1002,
      CTRL_logos_id2001 = 2001,
      CTRL_logos_id2002 = 2002,
      CTRL_logos_id2003 = 2003,
      CTRL_logos_id2012 = 2012,
      CTRL_Title = 2025,
      CTRL_OTitle = 2026,
      CTRL_ImgDD = 2072,
      CTRL_GuiWaitCursor = 2080
    }

    [SkinControl((int)Controls.CTRL_PersonFacade)] // to allow facade view in virtual actor/cast/crew screen
    protected GUIFacadeControl facadePersons;

    [SkinControl((int)Controls.CTRL_MovieThumbsFacade)]
    protected GUIFacadeControl facadeMovieThumbs;
    [SkinControl((int)Controls.CTRL_MovieThumbsDir)]
    protected GUIMultiImage ImgMovieThumbsDir;
    [SkinControl((int)Controls.CTRL_DummyMovieThumbsAvailable)]
    protected GUILabelControl dummyFacadeMovieThumbsAvailable = null;

    [SkinControl((int)Controls.CTRL_BtnMaj)]
    protected GUIButtonControl BtnMaj;
    [SkinControl((int)Controls.CTRL_BtnFirst)]
    protected GUIButtonControl BtnFirst;
    [SkinControl((int)Controls.CTRL_BtnLast)]
    protected GUIButtonControl BtnLast;
    [SkinControl((int)Controls.CTRL_BtnTraktMenu)]
    protected GUIButtonControl BtnTraktMenu;
    [SkinControl(2024)]
    protected GUIImage ImgDetFilm;
    [SkinControl(2023)]
    protected GUIImage ImgDetFilm2;
    [SkinControl((int)Controls.CTRL_logos_id2001)]
    protected GUIImage ImgID2001;
    [SkinControl((int)Controls.CTRL_logos_id2002)]
    protected GUIImage ImgID2002;
    [SkinControl((int)Controls.CTRL_logos_id2003)]
    protected GUIImage ImgID2003;
    [SkinControl((int)Controls.CTRL_logos_id2012)]
    protected GUIImage ImgID2012;
    [SkinControl((int)Controls.CTRL_ImgDD)] // Indicates if it's a file existant for movie on HD
    protected GUIImage ImgDD;
    [SkinControl((int)Controls.CTRL_GuiWaitCursor)]
    protected GUIAnimation m_SearchAnimation;
    [SkinControl((int)Controls.CTRL_Fanart)]
    protected GUIImage ImgFanart;
    [SkinControl((int)Controls.CTRL_FanartDir)]
    protected GUIMultiImage ImgFanartDir;
    #endregion

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    static string wzone = null;
    int StrMax = 0;

    public class Searchtitles
    {
      public string SearchTitle;
      public string FanartTitle;
      public string MasterTitle;
      public string SecondaryTitle;
      public string SortTitle;
      public string TranslatedTitle;
      public string OriginalTitle;
      public string FormattedTitle;
      public string MovieDirectoryTitle;
      public string MovieFileTitle;

      public string Director;
      public int Year;
    } ;

    static PlayListPlayer playlistPlayer;
    static VirtualDirectory m_directory = new VirtualDirectory();
    BackgroundWorker bgPicture = new BackgroundWorker();

    static bool m_askBeforePlayingDVDImage = false;
    public static ArrayList result;
    public static string wsearchfile;
    public static int wGetID;
    public static bool trailerPlayed = false;
    private bool PlayBackEvents_Subscribed = false;
    private bool doUpdateDetailsViewByFinishEvent = false;

    #region Enums
    private enum TraktGuiWindows
    {
      Main = 87258,
      Calendar = 87259,
      Friends = 87260,
      Recommendations = 87261,
      RecommendationsShows = 87262,
      RecommendationsMovies = 87263,
      Trending = 87264,
      TrendingShows = 87265,
      TrendingMovies = 87266,
      WatchedList = 87267,
      WatchedListShows = 87268,
      WatchedListEpisodes = 87269,
      WatchedListMovies = 87270,
      Settings = 87271,
      SettingsAccount = 87272,
      SettingsPlugins = 87273,
      SettingsGeneral = 87274,
      Lists = 87275,
      ListItems = 87276,
      RelatedMovies = 87277,
      RelatedShows = 87278,
      Shouts = 87280,
      ShowSeasons = 87281,
      SeasonEpisodes = 87282,
      Network = 87283,
      RecentWatchedMovies = 87284,
      RecentWatchedEpisodes = 87285,
      RecentAddedMovies = 87286,
      RecentAddedEpisodes = 87287,
      RecentShouts = 87288,
      UserProfile = 87400,
      Search = 874001,
      SearchEpisodes = 874002,
      SearchShows = 874003,
      SearchMovies = 874004,
      SearchPeople = 874005,
      SearchUsers = 874006
    }

    public enum GrabType
    {
      All,
      Details,
      Cover,
      MultiCovers,
      Fanart,
      Photos,
      Person,
      Trailers
    }

    public enum PlayerOption
    {
      Internal,
      External,
      BluRayPlayerLauncher
    }

    public enum PluginMode
    {
      Normal = 0,
      Test = 1,
      Extended = 2
    }
    #endregion

    public static event WatchedEventDelegate WatchedItem;
    public delegate void WatchedEventDelegate(MFMovie movie, bool watched, int count);

    public static event RatingEventDelegate RateItem;
    public delegate void RatingEventDelegate(MFMovie movie, string rating);

    public static event MovieStartedEventDelegate MovieStarted;
    public delegate void MovieStartedEventDelegate(MFMovie movie);

    public static event MovieStoppedEventDelegate MovieStopped;
    public delegate void MovieStoppedEventDelegate(MFMovie movie);

    public static event MovieWatchedEventDelegate MovieWatched;
    public delegate void MovieWatchedEventDelegate(MFMovie movie);

    // MF event for async updates
    public static event DetailsUpdatedEventDelegate DetailsUpdated;
    public delegate void DetailsUpdatedEventDelegate(bool searchPicture);

    // MF event for trailer completed (to start next)
    public static event TrailerEndedEventDelegate TrailerEnded;
    public delegate void TrailerEndedEventDelegate(string filename);

    public static FileSystemWatcher Trailerwatcher = new FileSystemWatcher();

    static MyFilmsDetail()
    {
      playlistPlayer = PlayListPlayer.SingletonPlayer;
    }

    #endregion
    
    internal static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the  downloading from a queue for e.g. actor images

    static Queue<DbPersonInfo> PersonstoDownloadQueue = new Queue<DbPersonInfo>();

    internal static List<string> theOnlineVideosViews = new List<string>();
    internal static List<KeyValuePair<string, string>> onlineVideosViews = new List<KeyValuePair<string, string>>();

    public MyFilmsDetail()
    {
      // lets set up the downloader            
      downloadingWorker.WorkerSupportsCancellation = true;
      downloadingWorker.WorkerReportsProgress = true;
      downloadingWorker.DoWork += new DoWorkEventHandler(downloadingWorker_DoWork);
      downloadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(downloadingWorker_RunWorkerCompleted);
      setDownloadStatus();
    }

    void downloadingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      // ToDo: Add Updater logic here
    }

    static void downloadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Debug("downloadingWorker_RunWorkerCompleted() - Finished loading person  images !");
      setDownloadStatus();
    }

    static void downloadingWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      var tmdbapi = new TheMoviedb();
      do
      {
        if (downloadingWorker.CancellationPending)
        {
          LogMyFilms.Debug("cancel person info updater...");
          return;
        }

        DbPersonInfo f;
        setDownloadStatus();
        lock (PersonstoDownloadQueue)
        {
          f = PersonstoDownloadQueue.Dequeue();
        }
        bool bDownloadSuccess = true;

        try
        {
          #region download person image

          bDownloadSuccess = UpdatePersonDetails(f.Name, null, false, false);

          #region experimental TMDB v3 code...
          //Grabber.TMDBv3.Tmdb api = new Grabber.TMDBv3.Tmdb("apikey", "de"); // language is optional, default is "en"
          //TmdbConfiguration tmdbConf = api.GetConfiguration();
          //TmdbPersonSearch person = api.SearchPerson("name", 1);
          //List<PersonResult> persons = person.results;
          //PersonResult pinfo = persons[0];
          //TmdbPerson singleperson = api.GetPersonInfo(pinfo.id);
          //TmdbPersonCredits personFilmList = api.GetPersonCredits(pinfo.id);

          // Search
          //TmdbMovieSearch SearchMovie(string query, int page)
          //TmdbPersonSearch SearchPerson(string query, int page)
          //TmdbCompanySearch SearchCompany(string query, int page);             

          // Person Info
          //TmdbPerson GetPersonInfo(int PersonID)
          //TmdbPersonCredits GetPersonCredits(int PersonID)
          //TmdbPersonImages GetPersonImages(int PersonID)
          //Movie Info
          //TmdbMovie GetMovieInfo(int MovieID)
          //TmdbMovie GetMovieByIMDB(string IMDB_ID)
          //TmdbMovieAlternateTitles GetMovieAlternateTitles(int MovieID, string Country)
          //TmdbMovieCast GetMovieCast(int MovieID)
          //TmdbMovieImages GetMovieImages(int MovieID)
          //TmdbMovieKeywords GetMovieKeywords(int MovieID)
          //TmdbMovieReleases GetMovieReleases(int MovieID)
          //TmdbMovieTrailers GetMovieTrailers(int MovieID)
          //TmdbSimilarMovies GetSimilarMovies(int MovieID, int page)
          //TmdbTranslations GetMovieTranslations(int MovieID)

          // Social Movie Info
          //TmdbNowPlaying GetNowPlayingMovies(int page)
          //TmdbPopular GetPopularMovies(int page)
          //TmdbTopRated GetTopRatedMovies(int page)
          //TmdbUpcoming GetUpcomingMovies(int page)
          #endregion

          #region TMDBv3 loading (inactive)
          //string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
          //List<DbPersonInfo> personlist = tmdbapi.GetPersonsByName(f.Name, false, language);
          //if (personlist.Count == 0)
          //{
          //  LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' not found on TMDB, remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //  bDownloadSuccess = false;
          //}
          //else
          //{
          //  f = personlist[0];

          //  if (f != null && !File.Exists(Path.Combine(MyFilms.conf.StrPathArtist, f.Name)))
          //  {
          //    if (f.Images.Count == 0)
          //    {
          //      LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' found, but no images available on TMDB! - remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //      bDownloadSuccess = false;
          //    }
          //    else
          //    {
          //      string filename = Path.Combine(MyFilms.conf.StrPathArtist, f.Name);

          //      LogMyFilms.Debug("downloadingWorker_DoWork() - TMDB Image found for person '" + f.Name + "', URL = '" + f.Images[0] + "' - remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //      string filename1person = Grabber.GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, f.Images[0], f.Name, false, true, out filename);
          //      LogMyFilms.Debug("Person Image (TMDB) '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + f.Name + "', path = '" + filename1person + "', filename = '" + filename + "'");

          //      if (downloadingWorker.CancellationPending)
          //      {
          //        bDownloadSuccess = false;
          //        LogMyFilms.Debug("cancel person image download - last person: " + f.Name);
          //        lock (PersonstoDownloadQueue)
          //        {
          //          PersonstoDownloadQueue.Clear();
          //        }
          //        return;
          //      }
          //      Application.DoEvents();
          //    }
          //  }
          //}
          #endregion

          #region try IMDB, if TMDB was not successful ! (inactive)
          //if (!bDownloadSuccess)
          //{
          //  // LogMyFilms.Debug("downloadingWorker_DoWork() - TMDB unsuccessful - try IMDB ...");
          //  bDownloadSuccess = true;
          //  var imdb = new IMDB();
          //  imdb.FindActor(f.Name);

          //  if (imdb.Count == 0)
          //  {
          //    LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' not found on IMDB, remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //    bDownloadSuccess = false;
          //  }
          //  else
          //  {
          //    if (imdb[0].URL.Length != 0)
          //    {
          //      var imdbActor = new IMDBActor();
          //      //#if MP1X
          //      //                    _imdb.GetActorDetails(_imdb[0], out imdbActor);
          //      //#else
          //      //                    _imdb.GetActorDetails(_imdb[0], false, out imdbActor);
          //      //#endif
          //      GUIUtils.GetActorDetails(imdb, imdb[0], false, out imdbActor);
          //      if (imdbActor.ThumbnailUrl.Length > 0)
          //      {
          //        LogMyFilms.Debug("downloadingWorker_DoWork() - IMDB Image found for person '" + f.Name + "', URL = '" + imdbActor.ThumbnailUrl + "' - remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //        string filename = Path.Combine(MyFilms.conf.StrPathArtist, f.Name);
          //        string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, imdbActor.ThumbnailUrl, f.Name, false, true, out filename);
          //        LogMyFilms.Debug("Person Image (IMDB) '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + f.Name + "', path = '" + filename1person + "', filename = '" + filename + "'");
          //      }
          //      else
          //      {
          //        LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' found, but no images available on IMDB! - remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //        bDownloadSuccess = false;
          //      }
          //    }
          //    else
          //    {
          //      LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' found, but no images available on IMDB! - remaining items: '" + PersonstoDownloadQueue.Count + "'");
          //      bDownloadSuccess = false;
          //    }
          //  }
          //}
          #endregion

          if (downloadingWorker.CancellationPending)
          {
            LogMyFilms.Debug("cancel person info download - last person: " + f.Name);
            lock (PersonstoDownloadQueue)
            {
              PersonstoDownloadQueue.Clear();
            }
            return;
          }

          // LogMyFilms.Debug("Result of person info download for '" + f.Name + "': success = '" + bDownloadSuccess + "'");
          if (bDownloadSuccess) downloadingWorker.ReportProgress(0, f.Name);
          #endregion
        }
        catch (Exception ex) { LogMyFilms.Debug(ex, "Error loading person updates: '" + ex.Message + "'"); }
      }
      while (PersonstoDownloadQueue.Count > 0 && !downloadingWorker.CancellationPending);
    }

    static void setDownloadStatus()
    {
      // LogMyFilms.Debug("setDownloadStatus() - remaining items in queue = '" + PersonstoDownloadQueue.Count + "'");
      lock (PersonstoDownloadQueue)
      {
        if (PersonstoDownloadQueue.Count > 0)
        {
          DbPersonInfo f = PersonstoDownloadQueue.Peek();
          setGUIProperty("details.downloads.status", string.Format(GUILocalizeStrings.Get(10799230), PersonstoDownloadQueue.Count.ToString()));
          setGUIProperty("details.downloads.count", PersonstoDownloadQueue.Count.ToString());
          setGUIProperty("details.downloads.name", f.Name ?? "");
        }
        else
        {
          clearGUIProperty("details.downloads.status");
          clearGUIProperty("details.downloads.count");
          clearGUIProperty("details.downloads.name");
        }
      }
    }

    public override int GetID
    {
      get { return MyFilms.ID_MyFilmsDetail; }
      set { base.GetID = value; }
    }

    public override string GetModuleName()
    {
      return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsDetail) + "/" + GUILocalizeStrings.Get(10798751); // return localized string for Module ID -> MyFilms/Details/Overview
    }

    public override bool Init()
    {
      LogMyFilms.Debug("MyFilmsDetail.Init() started.");

      GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_OnNewMessage);

      //g_Player.PlayBackStarted -= new g_Player.StartedHandler(OnPlayBackStarted);
      //g_Player.PlayBackEnded -= new g_Player.EndedHandler(OnPlayBackEnded);
      //g_Player.PlayBackStopped -= new g_Player.StoppedHandler(OnPlayBackStopped);
      g_Player.PlayBackStarted += new g_Player.StartedHandler(OnPlayBackStarted);
      g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
      g_Player.PlayBackStopped += new g_Player.StoppedHandler(OnPlayBackStopped);

      bool success = Load(GUIGraphicsContext.Skin + @"\MyFilmsDetail.xml");
      LogMyFilms.Debug("MyFilmsDetail.Init() ended.");
      return success;
    }

    protected override void OnPageLoad()
    {
      LogMyFilms.Debug("OnPageLoad() started.");

      if (!PlayBackEvents_Subscribed)
      {
        // Subscribe to GUI Events
        DetailsUpdated += new MyFilmsDetail.DetailsUpdatedEventDelegate(OnDetailsUpdated);
        PlayBackEvents_Subscribed = true;
      }

      //bool result = base.OnMessage(messageType);
      if (ImgDetFilm != null)
        if (ImgDetFilm.IsVisible)
          ImgDetFilm.Refresh();
        else if (ImgDetFilm2 != null)
          if (ImgDetFilm2.IsVisible)
            ImgDetFilm2.Refresh();

      if (facadeMovieThumbs != null && facadeMovieThumbs.ThumbnailLayout != null)
      {
        try
        {
          facadeMovieThumbs.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          // facadeMovieThumbs.Clear();
          GUIControl.ClearControl(GetID, facadeMovieThumbs.GetID);
          GUIControl.HideControl(GetID, (int)Controls.CTRL_MovieThumbsFacade);
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("OnPageLoad() - skin facade control for movie thumbs not initialized - exception: " + ex.Message);
        }
      }
      // if (ImgMovieThumbsDir != null && ImgMovieThumbsDir.Visible) ImgMovieThumbsDir.Visible = false;

      if (BtnFirst != null) BtnFirst.Label = GUILocalizeStrings.Get(1079872); //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnFirst, GUILocalizeStrings.Get(1079872));
      if (BtnLast != null) BtnLast.Label = GUILocalizeStrings.Get(1079873); //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLast, GUILocalizeStrings.Get(1079873));

      wGetID = GetID;
      GUIControl.ShowControl(GetID, 35);

      m_directory.SetExtensions(MediaPortal.Util.Utils.VideoExtensions);
      if (MyFilms.conf.StrTxtSelect.Length == 0)
        clearGUIProperty("select"); //GUIControl.HideControl(GetID, (int)Controls.CTRL_TxtSelect);
      else
        setGUIProperty("select", MyFilms.conf.StrTxtSelect.Replace(MyFilms.conf.TitleDelim, @"\")); //GUIControl.ShowControl(GetID, (int)Controls.CTRL_TxtSelect);

      //if (ImgFanartDir != null) ImgFanartDir.TexturePath = "";
      //if (ImgFanart != null) ImgFanart.SetFileName(MyFilms.conf.DefaultFanartImage);

      base.OnPageLoad(); // let animations run and make sure visibility on secondary title can be set!

      StrMax = MyFilms.r.Length; // selects records and sets StrIndex based on ItemId (leaves unchanged if ItemId=-1)
      afficher_detail(true);

      MyFilms.conf.LastID = MyFilms.ID_MyFilmsDetail;

      if (MyFilms.conf.PersonsEnableDownloads)
      {
        AddPersonsToDownloadQueue(); // add persons of current movie to download queue
        downloadingWorker.ProgressChanged += new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);
      }

      SetProcessAnimationStatus(false, m_SearchAnimation);

      if (MyFilms.conf.AutoRegisterTrailer) AutoRegisterTrailer("");
      LogMyFilms.Debug("OnPageLoad() finished.");
    }

    private void AutoRegisterTrailer(string newTrailerFile)
    {
      LogMyFilms.Debug("AutoRegisterTrailer() - called - enabled = '" + MyFilms.conf.AutoRegisterTrailer + "', newTrailerFile: '" + newTrailerFile + "'");
      if (MyFilms.conf.AutoRegisterTrailer)
      {
        if (newTrailerFile.Length > 0)
        {
          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
          {
            LogMyFilms.Debug("AutoRegisterTrailer() - Old Trailersourcepath: '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer] + "'");
            string trailersourcepath = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString();
            if (!trailersourcepath.Contains(newTrailerFile))
            {
              if (trailersourcepath.Length > 0)
                trailersourcepath = trailersourcepath + ";" + newTrailerFile;
              else
                trailersourcepath = newTrailerFile;
              LogMyFilms.Debug("AutoRegisterTrailer() - Added Trailer to Trailersource: '" + newTrailerFile + "'");
            }
            MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer] = trailersourcepath;
            LogMyFilms.Debug("AutoRegisterTrailer() - New Trailersourcepath    : '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer] + "'");
            Update_XML_database();
            LogMyFilms.Debug("AutoRegisterTrailer() - Database Updated !!!!");
          }
        }
        else
        {
          new Thread(delegate()
          {
            {
              //GUIWaitCursor.Init(); GUIWaitCursor.Show();
              SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, false, true);
              // Todo: Update DB, if intended
              // GUIWaitCursor.Hide();
            }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              {
                // this after thread finished ...
              }
              return 0;
            }, 0, 0, null);
          }) { Name = "MyFilmsAutoRegisterTrailer", IsBackground = true }.Start();
        }
      }
    }

    protected override void OnPageDestroy(int newWindowId)
    {
      LogMyFilms.Debug("MyFilmsDetail.OnPageDestroy(" + newWindowId + ") started.");
      if (MyFilms.conf.PersonsEnableDownloads)
      {
        if (downloadingWorker.IsBusy) downloadingWorker.CancelAsync();
        // while (downloadingWorker.IsBusy) System.Windows.Forms.Application.DoEvents();
        // downloadingWorker = null;
        downloadingWorker.ProgressChanged -= new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);
      }
      if (MyFilmsPlugin.MyFilmsGUI.Configuration.CurrentConfig != "")
        MyFilmsPlugin.MyFilmsGUI.Configuration.SaveConfiguration(MyFilmsPlugin.MyFilmsGUI.Configuration.CurrentConfig, MyFilms.conf.StrIndex, MyFilms.conf.StrTIndex);
      using (var xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
      {
        string currentmoduleid = "7986";
        bool currentmodulefullscreen = (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_MUSIC || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_TELETEXT);
        string currentmodulefullscreenstate = GUIPropertyManager.GetProperty("#currentmodulefullscreenstate");
        // if MP was closed/hibernated by the use of remote control, we have to retrieve the fullscreen state in an alternative manner.
        if (!currentmodulefullscreen && currentmodulefullscreenstate == "True")
          currentmodulefullscreen = true;
        xmlreader.SetValue("general", "lastactivemodule", currentmoduleid);
        xmlreader.SetValueAsBool("general", "lastactivemodulefullscreen", currentmodulefullscreen);
        LogMyFilms.Debug("SaveLastActiveModule - module {0}", currentmoduleid);
        LogMyFilms.Debug("SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
      }
      LogMyFilms.Debug("MyFilms.OnPageDestroy(" + newWindowId + ") completed.");
      base.OnPageDestroy(newWindowId);
    }

    #region Action
    //---------------------------------------------------------------------------------------
    //   Handle Keyboard Actions
    //---------------------------------------------------------------------------------------
    public override void OnAction(MediaPortal.GUI.Library.Action actionType)
    {
      LogMyFilms.Debug("MyFilmsDetail: OnAction " + actionType.wID.ToString());
      switch (actionType.wID)
      {
        #region action handling
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR:
          //if (BtnMaj.Focus) // first switch to main details window before returning to main window ....
          //{
          //  GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
          //  return;
          //}
          MyFilms.conf.LastID = MyFilms.ID_MyFilms;
          GUIWindowManager.ShowPreviousWindow();
          return;
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU: // context menu for options  like PlayTrailers or Updates
          LogMyFilms.Debug("MyFilmsDetail : ACTION_CONTEXT_MENU detected ! ");
          if (BtnMaj.Focus)
          {
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
            Update_XML_Items(); // Call Update Menu
          }
          else
          {
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
            Update_XML_Items(); // Call Update Menu
          }
          return;
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
          Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.Internal);
          return;

        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PAGE_UP:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_ITEM:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_REWIND:
          if (MyFilms.conf.StrIndex == 0) return;
          MyFilms.conf.StrIndex = MyFilms.conf.StrIndex - 1;
          //GUITextureManager.CleanupThumbs();
          //if (ImgMovieThumbsDir != null) ImgMovieThumbsDir.PreAllocResources();
          afficher_detail(true);
          if (MyFilms.conf.PersonsEnableDownloads) AddPersonsToDownloadQueue(); // add persons of current movie to download queue
          return;
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PAGE_DOWN:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_ITEM:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_FORWARD:
          if (MyFilms.conf.StrIndex == StrMax - 1) return;
          MyFilms.conf.StrIndex = MyFilms.conf.StrIndex + 1;
          //GUITextureManager.CleanupThumbs();
          // if (ImgMovieThumbsDir != null) ImgMovieThumbsDir.PreAllocResources();
          afficher_detail(true);
          if (MyFilms.conf.PersonsEnableDownloads) AddPersonsToDownloadQueue(); // add persons of current movie to download queue
          return;
        #endregion
      }
      base.OnAction(actionType);
    }

    //---------------------------------------------------------------------------------------
    //   Handle posted Messages
    //---------------------------------------------------------------------------------------
    public override bool OnMessage(GUIMessage messageType)
    {
      int dControl = messageType.TargetControlId;
      int iControl = messageType.SenderControlId;
      LogMyFilms.Debug("OnMessage - MessageType: '" + messageType.Message.ToString() + "', TargetId: '" + dControl + "', SenderId: '" + iControl + "'");
      switch (messageType.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          base.OnMessage(messageType);
          return true;
        //break;

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          base.OnMessage(messageType);
          return true;
        //break;

        case GUIMessage.MessageType.GUI_MSG_CD_REMOVED:
          //---------------------------------------------------------------------------------------
          // Stop playing after eject Cd
          //---------------------------------------------------------------------------------------
          if (g_Player.Playing && g_Player.IsDVD)
            g_Player.Stop();
          return true;

        case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
          //---------------------------------------------------------------------------------------
          // Set Focus and set #currentmodule based on focus
          //---------------------------------------------------------------------------------------
          string basename = GUILocalizeStrings.Get(MyFilms.ID_MyFilmsDetail); // return localized string for Module ID "Films/Details"
          switch (dControl)
          {
            case (int)Controls.CTRL_BtnPlay:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798751));
              break;
            case (int)Controls.CTRL_BtnPlay1Description:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798752));
              break;
            case (int)Controls.CTRL_BtnPlay2Comment:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798753));
              break;
            case (int)Controls.CTRL_BtnPlay3Persons:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798754));
              break;
            case (int)Controls.CTRL_BtnPlay4TecDetails:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798755));
              break;
            case (int)Controls.CTRL_BtnPlay5ExtraDetails:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798756));
              break;
            case (int)Controls.CTRL_ViewFanart:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798757));
              break;
            case (int)Controls.CTRL_ViewMovieThumbs:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798758));
              break;
            default:
              GUIPropertyManager.SetProperty("#currentmodule", basename + "/" + GUILocalizeStrings.Get(10798751)); // GUIPropertyManager.SetProperty("#currentmodule", basename);
              break;
          }
          LogMyFilms.Debug("MSG_Setfocus: focused control = '" + dControl + "', set currentmodule to '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
          base.OnMessage(messageType);
          return true;

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          //---------------------------------------------------------------------------------------
          // Mouse/Keyboard Clicked
          //---------------------------------------------------------------------------------------
          switch (iControl)
          {
            #region Message clicked handling
            case (int)Controls.CTRL_BtnReturn: // Return Previous Menu
              MyFilms.conf.LastID = MyFilms.ID_MyFilms;
              GUITextureManager.CleanupThumbs();
              GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilms);
              return true;

            case (int)Controls.CTRL_BtnPlay: // Search File to play
            case (int)Controls.CTRL_BtnPlay1Description:
            case (int)Controls.CTRL_BtnPlay2Comment:
            case (int)Controls.CTRL_BtnPlay3Persons:
            case (int)Controls.CTRL_BtnPlay4TecDetails:
            case (int)Controls.CTRL_BtnPlay5ExtraDetails:
              Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.Internal);
              return true;
            case (int)Controls.CTRL_ViewFanart: // On Button goto MyFilmsThumbs // Changed to also launch player due to Ember Media Manager discontinued...
            case (int)Controls.CTRL_ViewMovieThumbs:
              //GUIWindowManager.ActivateWindow(ID_MyFilmsThumbs);
              Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.Internal);
              return true;

            case (int)Controls.CTRL_BtnPlayTrailer: // Search Trailer File to play
              if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
              {
                trailerPlayed = true;
                Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
              }
              else
                Change_Menu("trailermenu");
              return true;

            case (int)Controls.CTRL_BtnNext: // Display Next Film (If last do nothing)
              if (MyFilms.conf.StrIndex == StrMax - 1)
                return true;
              MyFilms.conf.StrIndex = MyFilms.conf.StrIndex + 1;
              GUITextureManager.CleanupThumbs();
              // if (ImgMovieThumbsDir != null) ImgMovieThumbsDir.PreAllocResources();
              afficher_detail(true);
              return true;
            case (int)Controls.CTRL_BtnPrior: // Display Prior Film (If first do nothing)
              if (MyFilms.conf.StrIndex == 0)
                return true;
              MyFilms.conf.StrIndex = MyFilms.conf.StrIndex - 1;
              GUITextureManager.CleanupThumbs();
              // if (ImgMovieThumbsDir != null) ImgMovieThumbsDir.PreAllocResources();
              afficher_detail(true);
              return true;
            case (int)Controls.CTRL_BtnLast: // Display Next Film (If last do nothing)
              if (MyFilms.conf.StrIndex == StrMax - 1)
                return true;
              MyFilms.conf.StrIndex = StrMax - 1;
              GUITextureManager.CleanupThumbs();
              afficher_detail(true);
              return true;
            case (int)Controls.CTRL_BtnFirst: // Display Next Film (If First do nothing)
              if (MyFilms.conf.StrIndex == 0)
                return true;
              MyFilms.conf.StrIndex = 0;
              GUITextureManager.CleanupThumbs();
              afficher_detail(true);
              return true;
            case (int)Controls.CTRL_BtnMaj: // Update items
              Change_Menu("mainmenu");  // was: Update_XML_Items();
              // GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay); // Added to return to main view after menu
              return true;
            case (int)Controls.CTRL_BtnActors:
              GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsActors);
              return true;
            case (int)Controls.CTRL_BtnTraktMenu:
              TraktInternalMenu(MyFilms.currentMovie);
              return true;
            #endregion
          }
          base.OnMessage(messageType);
          return true;
      }
      base.OnMessage(messageType);
      return true;
    }

    //--------------------------------------------------------------------------------------------
    //  Update specifics Infos - creates Dialogue to choose actions...
    //  choice: sets dialogmode depending from where it's called to display differently
    //  possible modes: "localupdates", "internetupdates", etc.
    //--------------------------------------------------------------------------------------------
    private void Update_XML_Items()
    {
      Change_Menu("mainmenu");
    }


    //--------------------------------------------------------------------------------------------
    //   Change Menu (and process corresponding actions)
    //--------------------------------------------------------------------------------------------
    private void Change_Menu(string choiceView)
    {
      if (downloadingWorker.IsBusy) downloadingWorker.CancelAsync(); // cancel person image download, if user does select any (other) action

      var ds = new AntMovieCatalog();
      // int ItemID = (int)MyFilms.r[MyFilms.conf.StrIndex]["Number"];
      // May wish to completely re-load the dataset before updating any fields if used in multi-user system, but would req concurrency locks etc so...

      var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceViewMenu = new List<string>();

      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (null == keyboard) return;
      keyboard.Reset();
      keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared

      string title = string.Empty; // variable for searchtitle creation
      string mediapath = string.Empty; // variable for searchpath creation (for nfo/xml/xbmc reader)
      Searchtitles sTitles; // variable to get all searchtitles 
      var movie = new MFMovie(); // movie object for rating and trakt calls

      switch (choiceView)
      {
        case "mainmenu":

          #region show root menu
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798701)); // Options ...

          dlgmenu.Add(GUILocalizeStrings.Get(10798704)); //trailer menu "Trailer ..."
          choiceViewMenu.Add("trailermenu");

          if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
          {
            if (MyFilms.conf.EnhancedWatchedStatusHandling)
            {
              int watchedCount = new MultiUserData(MyFilms.r[MyFilms.conf.StrIndex][BaseMesFilms.MultiUserStateField].ToString()).GetUserState(MyFilms.conf.StrUserProfileName).WatchedCount;
              dlgmenu.Add(watchedCount > 0 ? GUILocalizeStrings.Get(1079895) : GUILocalizeStrings.Get(1079894));
            }
            else
            {
              dlgmenu.Add(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrWatchedField].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower() ? GUILocalizeStrings.Get(1079895) : GUILocalizeStrings.Get(1079894));
            }
            choiceViewMenu.Add("togglewatchedstatus");
          }

          dlgmenu.Add(GUILocalizeStrings.Get(931)); //rating
          choiceViewMenu.Add("rating");

          if (MyFilms.conf.StrFileType == MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog4Xtended || MyFilms.conf.EnhancedWatchedStatusHandling) // user rating only for AMC4+ or when using enhanced watched handling
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798944)); // User Rating
            choiceViewMenu.Add("userrating");
          }

          if (MyFilms.conf.StrSuppressAutomatic || MyFilms.conf.StrSuppressManual)
          {
            dlgmenu.Add(GUILocalizeStrings.Get(1079830)); // Delete movie ...
            choiceViewMenu.Add("delete");
          }

          dlgmenu.Add(GUILocalizeStrings.Get(1079879)); //Search Infos to related persons (load persons in facadeview)
          choiceViewMenu.Add("moviepersonlist");

          dlgmenu.Add(GUILocalizeStrings.Get(10798702)); // Updates ...
          choiceViewMenu.Add("updatesmenu");

          // Add Submenu for useritemx mapping
          dlgmenu.Add(string.Format(GUILocalizeStrings.Get(10798771)));  // Global Mappings ...
          choiceViewMenu.Add("globalmappings");

          if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml"))
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798763)); // Cover Manager ...
            choiceViewMenu.Add("covermanager");
          }

          dlgmenu.Add(GUILocalizeStrings.Get(10798767)); // Fanart Manager ...
          choiceViewMenu.Add("fanartmanager");

          //dlgmenu.Add(GUILocalizeStrings.Get(10798703)); // Fanart & Cover ...
          //choiceViewMenu.Add("fanartcovermenu");

          if (Helper.IsTraktAvailableAndEnabled) //  && MyFilms.conf.AllowTraktSync
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798775)); // Trakt external menu...
            choiceViewMenu.Add("traktinternal");

            if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
            {
              if (ExtendedStartmode(PluginMode.Extended, "Detail context menu - old MF Trakt menu"))
              {
                dlgmenu.Add(GUILocalizeStrings.Get(10798775) + " (MF old menu)"); // Trakt ... (MF menu)
                choiceViewMenu.Add("trakt");
              }
            }
          }
          else LogMyFilms.Debug("trakt not found or wrong version - disabling context entry");

          if (Helper.IsSubCentralAvailableAndEnabled)
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798707)); // Subtitles ...
            choiceViewMenu.Add("subtitles");
          }
          else LogMyFilms.Debug("Subcentral not found or wrong version - disabling context entry");

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            // GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay); // Added to return to main view after menu // Removed as it's causing an exception
            // return;
            break;
          }
          Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
          #endregion
          break;

        case "trailermenu":
          #region trailer menu
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          choiceViewMenu.Clear();

          #region  local trailer ...
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704));

          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
          {
            string trailercount = "";
            if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
              trailercount = "0";
            else
            {
              string[] split1 = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim().Split(new Char[] { ';' });
              trailercount = split1.Length.ToString();
            }
            if (trailercount != "0")
            {
              dlgmenu.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")"); //play trailer (<number trailers present>)
              choiceViewMenu.Add("playtrailer");
            }
          }
          #endregion

          dlgmenu.Add(GUILocalizeStrings.Get(10798711)); //search youtube trailer with onlinevideos
          choiceViewMenu.Add("playtraileronlinevideos");

          dlgmenu.Add(GUILocalizeStrings.Get(10798712)); //search apple itunes trailer with onlinevideos
          choiceViewMenu.Add("playtraileronlinevideosappleitunes");

          dlgmenu.Add(GUILocalizeStrings.Get(10798716)); //search IMDB trailer with onlinevideos
          choiceViewMenu.Add("playtraileronlinevideosimdbtrailer");

          dlgmenu.Add("FilmStarts.de (OnlineVideos)");
          choiceViewMenu.Add("playtraileronlinevideosfilmstarts");

          dlgmenu.Add("OnlineVideos ...");
          choiceViewMenu.Add("playtraileronlinevideosall");

          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798723)); //Search local Trailer and Update DB (local)
            choiceViewMenu.Add("trailer-register");

            dlgmenu.Add(GUILocalizeStrings.Get(10798725)); //delete Trailer entries from DB record
            choiceViewMenu.Add("trailer-delete");

            dlgmenu.Add(GUILocalizeStrings.Get(10798724)); //load IMDB Trailer, store locally and update DB
            // dlgmenu.Add(GUILocalizeStrings.Get(10798990)); // Load single trailer (TMDB)
            choiceViewMenu.Add("trailer-tmdb");
          }

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
          #endregion
          break;

        case "playtraileronlinevideos":
          LaunchOnlineVideos("YouTube");
          break;
        case "playtraileronlinevideosappleitunes":
          LaunchOnlineVideos("iTunes Movie Trailers");
          break;
        case "playtraileronlinevideosimdbtrailer":
          LaunchOnlineVideos("IMDb Movie Trailers");
          break;
        case "playtraileronlinevideosfilmstarts":
          LaunchOnlineVideos("FilmStarts.de Trailer");
          break;

        case "playtraileronlinevideosall":
          #region show all OnlineVideos sites
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          choiceViewMenu.Clear();

          dlgmenu.SetHeading("OnlineVideos ...");

          try
          {
            LoadOnlineVideosViews();
            foreach (string theOnlineVideosView in theOnlineVideosViews)
            {
              dlgmenu.Add(theOnlineVideosView);
              choiceViewMenu.Add(theOnlineVideosView);
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Warn("Error when trying to load OnlineVideos site list - Exceptoion: " + ex.Message);
          }
          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          LaunchOnlineVideos(choiceViewMenu[dlgmenu.SelectedLabel]);
          #endregion
          break;

        case "playtrailer":
          #region play trailer
          // first check, if trailer files are available, offer options
          //if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
          if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
          {
            trailerPlayed = true;
            Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
          }
          else
          {
            // Can add autosearch&register logic here before try starting trailers
            if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(10798704), MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle] + "\n" + GUILocalizeStrings.Get(10798737) + "\n" + GUILocalizeStrings.Get(10798739))) //trailer ////no video found locally // Search local trailers  and update DB ?
            {
              SetProcessAnimationStatus(true, m_SearchAnimation);
              //LogMyFilms.Debug("SearchTrailerLocal() SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "'"));
              LogMyFilms.Debug("(Auto search trailer after selecting PLAY) title: '" + (MyFilms.r[MyFilms.conf.StrIndex] + "'"));
              SearchTrailerLocal(MyFilms.r, MyFilms.conf.StrIndex, true, true);
              afficher_detail(true);
              SetProcessAnimationStatus(false, m_SearchAnimation);
              trailerPlayed = true;
              Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
            }
          }
          #endregion
          break;

        case "togglewatchedstatus":
          #region toggle watched status
          if (MyFilms.conf.EnhancedWatchedStatusHandling)
          {
            int watchedCount = new MultiUserData(MyFilms.r[MyFilms.conf.StrIndex][BaseMesFilms.MultiUserStateField].ToString()).GetUserState(MyFilms.conf.StrUserProfileName).WatchedCount;
            Watched_Toggle(MyFilms.r[MyFilms.conf.StrIndex], watchedCount <= 0);
          }
          else
          {
            Watched_Toggle(MyFilms.r[MyFilms.conf.StrIndex], MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrWatchedField].ToString().ToLower() == MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower());
          }
          afficher_detail(true);
          #endregion
          break;

        case "rating":
          {
            #region DB rating
            MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(MyFilms.ID_MyFilmsDialogRating);
            if (MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Length > 0)
            {
              dlgRating.Rating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["Rating"];
              if (dlgRating.Rating > 10) dlgRating.Rating = 10;
            }
            else dlgRating.Rating = 0;

            dlgRating.SetTitle(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
            dlgRating.DoModal(GetID);
            // if (dlgRating.SelectedLabel == -1 || dlgmenu.SelectedLabel != 2)  // If "ESC" or not returning from "ok"
            if (dlgRating.Result == MyFilmsDialogSetRating.ResultCode.Cancel) // If "ESC" or not returning from "ok"
            {
              Change_Menu("mainmenu");
              return;
            }
            MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = dlgRating.Rating;

            Update_XML_database();
            afficher_detail(true);

            //// tell any listeners that user rated the movie
            //movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
            //string value = dlgRating.Rating.ToString();
            //if (RateItem != null) RateItem(movie, value);
            #endregion
          }
          break;

        case "userrating":
          {
            #region User rating
            var dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(MyFilms.ID_MyFilmsDialogRating);
            if (MyFilms.conf.EnhancedWatchedStatusHandling)
            {
              decimal wRating = new MultiUserData(MyFilms.r[MyFilms.conf.StrIndex][BaseMesFilms.MultiUserStateField].ToString()).GetUserState(MyFilms.conf.StrUserProfileName).UserRating;
              dlgRating.Rating = (wRating != decimal.MinValue) ? wRating : 5;
            }
            else
            {
              if (MyFilms.r[MyFilms.conf.StrIndex]["RatingUser"].ToString().Length > 0)
              {
                dlgRating.Rating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["RatingUser"];
              }
              else dlgRating.Rating = 5;
            }
            if (dlgRating.Rating > 10) dlgRating.Rating = 10;
            if (dlgRating.Rating < 0) dlgRating.Rating = 5;

            dlgRating.SetTitle(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
            dlgRating.DoModal(GetID);
            // if (dlgRating.SelectedLabel == -1 || dlgmenu.SelectedLabel != 2)  // If "ESC" or not returning from "ok"
            if (dlgRating.Result == MyFilmsDialogSetRating.ResultCode.Cancel) // If "ESC" or not returning from "ok"
            {
              Change_Menu("mainmenu");
              return;
            }
            if (MyFilms.conf.EnhancedWatchedStatusHandling)
            {
              var userData = new MultiUserData(MyFilms.r[MyFilms.conf.StrIndex][BaseMesFilms.MultiUserStateField].ToString());
              userData.SetRating(MyFilms.conf.StrUserProfileName, dlgRating.Rating);
              MyFilms.r[MyFilms.conf.StrIndex][BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
              SyncMusToExtendedFields(MyFilms.r[MyFilms.conf.StrIndex], userData, MyFilms.conf.StrUserProfileName);
            }
            else
            {
              MyFilms.r[MyFilms.conf.StrIndex]["RatingUser"] = dlgRating.Rating; // always set db value, so in enhanced wat hed mode it represents the lst chaned value ...
            }

            if (MyFilms.conf.StrUserProfileName.Length > 0)
            {
              if (dlgRating.Rating > MultiUserData.FavoriteRating)
                MyFilms.r[MyFilms.conf.StrIndex]["Favorite"] = MultiUserData.Add(MyFilms.r[MyFilms.conf.StrIndex]["Favorite"].ToString(), MyFilms.conf.StrUserProfileName);
              else
                MyFilms.r[MyFilms.conf.StrIndex]["Favorite"] = MultiUserData.Remove(MyFilms.r[MyFilms.conf.StrIndex]["Favorite"].ToString(), MyFilms.conf.StrUserProfileName);
            }

            Update_XML_database();
            afficher_detail(true);

            // tell any listeners that user rated the movie
            movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
            string value = dlgRating.Rating.ToString();
            if (RateItem != null) RateItem(movie, value);
            #endregion
          }
          break;

        case "updatesmenu":
          #region updates menu
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          choiceViewMenu.Clear();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798702)); // Updates ...

          if (MyFilms.conf.StrFileType == MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog3 ||
              MyFilms.conf.StrFileType == MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog4Xtended)
          {
            dlgmenu.Add(GUILocalizeStrings.Get(5910)); //Update Internet Movie Details
            choiceViewMenu.Add("grabber");
          }

          dlgmenu.Add(GUILocalizeStrings.Get(10798642)); // Update by Property (choosen within the UPdate List Property
          choiceViewMenu.Add("updproperty");

          if (ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Details context: remove all details"))
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798795)); // remove all movie details (selected film)
            choiceViewMenu.Add("updremovealldetails");
          }

          //if (Helper.FieldIsSet(MyFilms.conf.StrStorage) && (MyFilms.conf.WindowsFileDialog))
          if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798636)); //filename
            choiceViewMenu.Add("fileselect");
          }

          if (ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Details context: update mediainfos"))
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798708)); //update mediainfos
            choiceViewMenu.Add("updmediainfos");
          }

          if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
          {
            dlgmenu.Add(GUILocalizeStrings.Get(1079883)); // Update info for all involved persons (IMDb)
            choiceViewMenu.Add("personimages");
          }

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
          #endregion
          break;

        case "globalmappings": // map useritems from GUI
          #region globalmappings
          var dlg3 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg3 == null) return;
          dlg3.Reset();
          dlg3.SetHeading(GUILocalizeStrings.Get(10798771)); // Display options ...
          var choiceGlobalMappings = new List<string>();

          #region populate menu
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 1 (" + MyFilms.conf.StritemDetails1 + "-" + MyFilms.conf.StrlabelDetails1 + ")"); // Details Display Item ....
          choiceGlobalMappings.Add("useritemdetails1");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 2 (" + MyFilms.conf.StritemDetails2 + "-" + MyFilms.conf.StrlabelDetails2 + ")");
          choiceGlobalMappings.Add("useritemdetails2");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 3 (" + MyFilms.conf.StritemDetails3 + "-" + MyFilms.conf.StrlabelDetails3 + ")");
          choiceGlobalMappings.Add("useritemdetails3");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 4 (" + MyFilms.conf.StritemDetails4 + "-" + MyFilms.conf.StrlabelDetails4 + ")");
          choiceGlobalMappings.Add("useritemdetails4");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 5 (" + MyFilms.conf.StritemDetails5 + "-" + MyFilms.conf.StrlabelDetails5 + ")");
          choiceGlobalMappings.Add("useritemdetails5");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 6 (" + MyFilms.conf.StritemDetails6 + "-" + MyFilms.conf.StrlabelDetails6 + ")");
          choiceGlobalMappings.Add("useritemdetails6");

          // master-, secondary-  and sorttitle
          //dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + MyFilms.conf.StrTitle1 + "-" + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1) + ")"); // mastertitle
          dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + MyFilms.conf.StrTitle1 + "-" + BaseMesFilms.TranslateColumn(MyFilms.conf.StrTitle1) + ")"); // mastertitle
          choiceGlobalMappings.Add("mastertitle");
          dlg3.Add(GUILocalizeStrings.Get(10798791) + " (" + MyFilms.conf.StrTitle2 + "-" + BaseMesFilms.TranslateColumn(MyFilms.conf.StrTitle2) + ")"); // secondary title
          choiceGlobalMappings.Add("secondarytitle");
          dlg3.Add(GUILocalizeStrings.Get(10798792) + " (" + MyFilms.conf.StrSTitle + "-" + BaseMesFilms.TranslateColumn(MyFilms.conf.StrSTitle) + ")"); // sort title
          choiceGlobalMappings.Add("sorttitle");
          #endregion

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          int selection = dlg3.SelectedLabel;
          string strUserItemSelection = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();

          dlg3.SetHeading(GUILocalizeStrings.Get(10798772)); // Choose field ...
          #region populate menu with choices
          switch (strUserItemSelection)
          {
            case "mastertitle":
            case "sorttitle":
              break;
            case "secondarytitle":
              dlg3.Add("<" + GUILocalizeStrings.Get(10798774) + ">"); // empty
              choiceGlobalMappings.Add("(none)");
              break;
            default:
              dlg3.Add("<" + GUILocalizeStrings.Get(10798774) + ">"); // empty
              choiceGlobalMappings.Add("");
              break;
          }
          if (selection > 5) // title fields
          {
            ArrayList displayItems = MyFilms.GetDisplayItems("titles");
            foreach (string[] displayItem in displayItems)
            {
              dlg3.Add(displayItem[0] + "-" + displayItem[1]);
              choiceGlobalMappings.Add(displayItem[0]);
            }
          }
          else // display item fields
          {
            ArrayList displayItems = MyFilms.GetDisplayItems("viewitems");
            foreach (string[] displayItem in displayItems)
            {
              dlg3.Add(displayItem[1] + " (" + displayItem[0] + ")");
              choiceGlobalMappings.Add(displayItem[0]);
            }
          }
          #endregion

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }

          string selectionLabel = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();
          LogMyFilms.Debug("Display Options - new field: '" + selectionLabel + "', new Label: '" + BaseMesFilms.TranslateColumn(selectionLabel) + "'.");
          MyFilms.UpdateUseritemWithValue(strUserItemSelection, selectionLabel);
          MyFilms.UpdateUserItems(); // save to currentconfig - save time for WinDeInit
          //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
          //Load_Config(Configuration.CurrentConfig, true);
          Init_Detailed_DB(false); // clear properties 
          afficher_detail(true);
          return;
          #endregion

        case "fanartcovermenu":
          #region fanart and cover menu (currently not used)
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          choiceViewMenu.Clear();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798703)); // Fanart & Cover ...

          int iCovercount = 0;
          bool success = int.TryParse(ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, true), out iCovercount);
          if (iCovercount > 1)
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798762) + " " + ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, true)); // Change Cover
            choiceViewMenu.Add("changecover");
          }

          //dlgmenu.Add(GUILocalizeStrings.Get(10798763)); // Covermanager ...
          //choiceViewMenu.Add("covermanager");

          dlgmenu.Add(GUILocalizeStrings.Get(10798766)); // Load single Cover ...
          choiceViewMenu.Add("loadcover");

          dlgmenu.Add(GUILocalizeStrings.Get(10798764)); // Load Covers ...
          choiceViewMenu.Add("loadmulticover");

          dlgmenu.Add(GUILocalizeStrings.Get(10798761)); // Load Covers (TMDB)
          choiceViewMenu.Add("tmdbposter");

          if (ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Details context: Thumb creator)"))
          {
            dlgmenu.Add(GUILocalizeStrings.Get(10798728));
            //Create Thumb from movie - if no cover available, e.g. with documentaries
            choiceViewMenu.Add("cover-thumbnailer");
          }

          if (MyFilms.conf.StrFanart)
          {
            if (ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Details context: Fanart Manager ..."))
            {
              dlgmenu.Add(GUILocalizeStrings.Get(10798766)); // Fanart Manager ...
              choiceViewMenu.Add("fanartmanager");
            }

            dlgmenu.Add(GUILocalizeStrings.Get(1079874)); // Remove Fanart
            choiceViewMenu.Add("deletefanart");

            dlgmenu.Add(GUILocalizeStrings.Get(1079851));
            // Create single fanart from movie as multi image (local)
            choiceViewMenu.Add("createfanartmultiimage");

            dlgmenu.Add(GUILocalizeStrings.Get(1079849)); // create single images fanart from movie (local)
            choiceViewMenu.Add("createfanartsingleimages");

            dlgmenu.Add(GUILocalizeStrings.Get(1079853)); // create fanart from movie (local)
            choiceViewMenu.Add("createfanart");

            if (!g_Player.Stopped && g_Player.HasVideo && !string.IsNullOrEmpty(g_Player.CurrentFile))
            {
              dlgmenu.Add(GUILocalizeStrings.Get(1079852));
              // Create single fanart from movie on current position (local)
              choiceViewMenu.Add("createfanartonposition");
            }

            dlgmenu.Add(GUILocalizeStrings.Get(1079862)); // Download Fanart (online)
            choiceViewMenu.Add("fanart");
          }

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
          #endregion
          break;

        case "traktinternal":
          #region trakt internal menu - including context and noncontext items
          if (!TraktInternalMenu(MyFilms.currentMovie))
          {
            Change_Menu("mainmenu");
          }
          #endregion
          break;

        case "trakt":
          #region trakt main menu
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          choiceViewMenu.Clear();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798775)); // Trakt...

          dlgmenu.Add(GUILocalizeStrings.Get(10798776)); // 
          choiceViewMenu.Add("trakt-Main");

          //dlgmenu.Add(GUILocalizeStrings.Get(10798777)); // 
          //choiceViewMenu.Add("trakt-Calendar");

          dlgmenu.Add(GUILocalizeStrings.Get(10798778)); // Network
          choiceViewMenu.Add("trakt-Network");

          dlgmenu.Add(GUILocalizeStrings.Get(10798779)); // 
          choiceViewMenu.Add("trakt-RecommendationsMovies");

          dlgmenu.Add(GUILocalizeStrings.Get(10798780)); // 
          choiceViewMenu.Add("trakt-TrendingMovies");

          dlgmenu.Add(GUILocalizeStrings.Get(10798781)); // 
          choiceViewMenu.Add("trakt-WatchedListMovies");

          dlgmenu.Add(GUILocalizeStrings.Get(10798783)); // 
          choiceViewMenu.Add("trakt-AddToWatchedListMovies");

          dlgmenu.Add(GUILocalizeStrings.Get(10798786)); // 
          choiceViewMenu.Add("trakt-Lists");

          dlgmenu.Add(GUILocalizeStrings.Get(10798785)); // 
          choiceViewMenu.Add("trakt-AddRemoveMovieInUserlist");

          dlgmenu.Add(GUILocalizeStrings.Get(10798782)); // Shouts
          choiceViewMenu.Add("trakt-Shouts");

          dlgmenu.Add(GUILocalizeStrings.Get(10798784)); // Rate
          choiceViewMenu.Add("trakt-Rate");

          dlgmenu.Add(GUILocalizeStrings.Get(10798787)); // 
          choiceViewMenu.Add("trakt-RelatedMovies");

          dlgmenu.Add(GUILocalizeStrings.Get(10798788)); // Search people ...
          choiceViewMenu.Add("trakt-SearchPeople");

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel]);
          #endregion
          break;

        case "trakt-Main":
          GUIWindowManager.ActivateWindow((int)TraktGuiWindows.Main, "");
          break;

        case "trakt-Calendar":
          GUIWindowManager.ActivateWindow((int)TraktGuiWindows.Calendar, "");
          break;

        case "trakt-Network":
          GUIWindowManager.ActivateWindow((int)TraktGuiWindows.Network, "");
          break;

        case "trakt-RecommendationsMovies":
          GUIWindowManager.ActivateWindow((int)TraktGuiWindows.RecommendationsMovies, "");
          break;

        case "trakt-TrendingMovies":
          GUIWindowManager.ActivateWindow((int)TraktGuiWindows.TrendingMovies, "");
          break;

        case "trakt-WatchedListMovies":
          GUIWindowManager.ActivateWindow((int)TraktGuiWindows.WatchedListMovies, "");
          break;

        case "trakt-AddToWatchedListMovies":
          GUIUtils.ShowErrorDialog(Helper.IsTraktAvailableAndEnabledAndNewVersion
            ? "This feature is no more supported - use the Trakt menu instead!"
            : "Your installed Trakt Version does not allow this feature!");
          break;

        case "trakt-Lists":
          if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
          {
            GUIWindowManager.ActivateWindow((int)TraktGuiWindows.Lists, "");
          }
          else
          {
            GUIUtils.ShowErrorDialog("Your installed Trakt Version does not allow this feature!");
          }
          break;

        case "trakt-AddRemoveMovieInUserlist":
          if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
          {
            TraktAddRemoveMovieInUserlist(MyFilms.currentMovie, false);
          }
          else
          {
            GUIUtils.ShowErrorDialog("Your installed Trakt Version does not allow this feature!");
          }
          break;

        case "trakt-Shouts":
          movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
          TraktShout(movie);
          //TraktShout(MyFilms.currentMovie);
          //GUIWindowManager.ActivateWindow((int)TraktGUIWindows.Shouts);
          break;

        case "trakt-Rate":
          GUIUtils.ShowErrorDialog(Helper.IsTraktAvailableAndEnabled
            ? "This feature is no more supported - use the Trakt menu instead!"
            : "Your installed Trakt Version does not allow this feature!");
          break;

        case "trakt-RelatedMovies":
          if (Helper.IsTraktAvailableAndEnabled)
          {
            TraktRelatedMovies(MyFilms.currentMovie);
          }
          else
          {
            GUIUtils.ShowErrorDialog("Your installed Trakt Version does not allow this feature!");
          }
          break;

        case "trakt-SearchPeople":
          if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
          {
            TraktSearchPeople(MyFilms.currentMovie);
          }
          else
          {
            GUIUtils.ShowErrorDialog("Your installed Trakt Version does not allow this feature!");
          }
          break;

        case "subtitles":
          if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() != null) // ShowSubtitleMenu(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
          {
            if (Helper.IsSubCentralAvailableAndEnabled) GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.SubCentral);
          }
          break;

        case "fileselect":
          #region file selection (source)
          string wfile = string.Empty;
          string wdirectory = string.Empty;
          if (File.Exists(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString())) // Check if Sourcefile exists
          {
            wfile = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
            wdirectory = Path.GetDirectoryName(wfile);
          }
          keyboard.Reset();
          keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
          keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
          keyboard.DoModal(GetID);
          wfile = keyboard.IsConfirmed ? keyboard.Text : string.Empty;
          if (wfile != string.Empty)
          {
            MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage] = wfile;
            Update_XML_database();
            afficher_detail(true);
          }
          #endregion
          break;

        case "delete":
          {
            #region delete item
            if (dlgmenu == null) return;
            dlgmenu.Reset();
            choiceViewMenu.Clear();
            dlgmenu.SetHeading(GUILocalizeStrings.Get(1079830)); // Delete movie ...

            dlgmenu.Add(GUILocalizeStrings.Get(1079831)); // Remove movie from catalog
            choiceViewMenu.Add("removefromdb");

            dlgmenu.Add(GUILocalizeStrings.Get(1079832)); // Delete movie file(s) from disk
            choiceViewMenu.Add("deletefromdisk");

            dlgmenu.Add(GUILocalizeStrings.Get(1079833)); // Delete from catalog and disk
            choiceViewMenu.Add("deletefromdbanddisk");

            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1)
            {
              Change_Menu("mainmenu");
              return;
            }
            Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
            #endregion
            break;
          }

        case "moviepersonlist":
          {
            #region display person list of current movie in main screen
            MyFilms.internalLoadParam = "moviepersonlist";
            GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilms,false);
            #endregion
            break;
          }

        case "removefromdb":
          #region remove from DB
          
          dlgYesNo.Reset();
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079831)); //Remove movie from catalog
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433)); //confirm suppression
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            // MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex);
            ManualDelete(MyFilms.r[MyFilms.conf.StrIndex], true, false);
            // Update_XML_database();
            MyFilms.r = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
            afficher_detail(true);
            return;
          }
          if (dlgYesNo.SelectedLabel == -1)
          {
            Change_Menu("delete");
          }
          #endregion
          break;

        case "deletefromdisk":
          #region delete item from disk
          dlgYesNo.Reset();
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079832)); //Delete movie file(s) from disk
          dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927)); // warning
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));
          //If you confirm, you media files will physically be deleted !
          dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835)); //Are you sure you want to delete movie ?
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            ManualDelete(MyFilms.r[MyFilms.conf.StrIndex], false, true);
            MyFilms.r = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
            afficher_detail(true);
            return;
          }
          if (dlgYesNo.SelectedLabel == -1)
          {
            Change_Menu("delete");
          }
          #endregion
          break;

        case "deletefromdbanddisk":
          #region delete item from DB and disk
          dlgYesNo.Reset();
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079833)); //Delete from catalog and disk
          dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927)); // warning
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));
          //If you confirm, you media files will physically be deleted !
          dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835)); //Are you sure you want to delete movie ?
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            ManualDelete(MyFilms.r[MyFilms.conf.StrIndex], true, true);
            MyFilms.r = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
            afficher_detail(true);
            return;
          }
          if (dlgYesNo.SelectedLabel == -1)
          {
            Change_Menu("delete");
          }
          #endregion
          break;

        case "updproperty":
          {
            #region build menu
            List<string> choiceUpd = new List<string>();
            if (dlgmenu == null) return;
            dlgmenu.Reset();
            dlgmenu.SetHeading(GUILocalizeStrings.Get(10798643)); // menu

            dlgmenu.Add(" " + BaseMesFilms.TranslateColumn(MyFilms.conf.StrTitle1));
            choiceUpd.Add(MyFilms.conf.StrTitle1);

            if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
            {
              dlgmenu.Add(" " + BaseMesFilms.TranslateColumn(MyFilms.conf.StrTitle2));
              choiceUpd.Add(MyFilms.conf.StrTitle2);
            }

            if (Helper.FieldIsSet(MyFilms.conf.StrStorage)) // Source field / media file name
            {
              dlgmenu.Add(" " + BaseMesFilms.TranslateColumn(MyFilms.conf.StrStorage));
              choiceUpd.Add(MyFilms.conf.StrStorage);
            }

            string[] defaultupdateitems = new string[] { "Category", "Year", "Date", "Country", "Rating" };
            foreach (string wupd in defaultupdateitems)
            {
              dlgmenu.Add(" " + BaseMesFilms.TranslateColumn(wupd));
              choiceUpd.Add(wupd.Trim());
            }

            dlgmenu.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
            choiceUpd.Add("showall");

            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1) Change_Menu("mainmenu"); // go back to main contextmenu

            // show all search fields, if selected ...
            if (choiceUpd[dlgmenu.SelectedLabel] == "showall")
            {
              dlgmenu.Reset();
              dlgmenu.SetHeading(GUILocalizeStrings.Get(10798643)); // menu
              choiceUpd.Clear();
              ArrayList displayItems = MyFilms.GetDisplayItems("view");
              foreach (string[] displayItem in displayItems)
              {
                string entry = string.IsNullOrEmpty(displayItem[1]) ? displayItem[0] : displayItem[1];
                dlgmenu.Add(" " + entry);
                choiceUpd.Add(displayItem[0]);
                LogMyFilms.Debug("Update properties menu - add '{0}' as '{1}'", displayItem[0], entry);
              }
              dlgmenu.DoModal(GetID);
              if (dlgmenu.SelectedLabel == -1) Change_Menu("mainmenu"); // go back to main contextmenu
            }
            #endregion
            string wproperty = choiceUpd[dlgmenu.SelectedLabel];
            #region update selected property

            dlgmenu.Reset();
            keyboard.Reset();
            keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
            keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][wproperty].ToString();
            keyboard.DoModal(GetID);
            if (keyboard.IsConfirmed)
            {
              switch (ds.Movie.Columns[wproperty].DataType.Name)
              {
                case "Decimal":
                  try
                  {
                    MyFilms.r[MyFilms.conf.StrIndex][wproperty] = Convert.ToDecimal(keyboard.Text);
                  }
                  catch
                  {
                    GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798642), GUILocalizeStrings.Get(10798644)); // wrong input
                    return;
                  }
                  break;
                case "Int32":
                  try
                  {
                    MyFilms.r[MyFilms.conf.StrIndex][wproperty] = Convert.ToInt32(keyboard.Text);
                  }
                  catch
                  {
                    GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798642), GUILocalizeStrings.Get(10798644)); // wrong input
                    return;
                  }
                  break;
                default:
                  MyFilms.r[MyFilms.conf.StrIndex][wproperty] = keyboard.Text;
                  break;
              }
              Update_XML_database();
              afficher_detail(true);
            }
            break;

            #endregion
          }

        case "updremovealldetails":
          #region reset movie details
          ArrayList deleteItems = MyFilms.GetDisplayItems("deletedetails");
          foreach (string wproperty in deleteItems.Cast<string[]>().Select(displayItem => displayItem[0]).Where(wproperty => !string.IsNullOrEmpty(wproperty) && MyFilms.r[MyFilms.conf.StrIndex][wproperty] != DBNull.Value))
          {
            try
            {
              switch (ds.Movie.Columns[wproperty].DataType.Name)
              {
                case "DateTime":
                  MyFilms.r[MyFilms.conf.StrIndex][wproperty] = DateTime.Now;
                  break;
                case "Decimal":
                  MyFilms.r[MyFilms.conf.StrIndex][wproperty] = 0;
                  break;
                case "Int32":
                  MyFilms.r[MyFilms.conf.StrIndex][wproperty] = 0;
                  break;
                default:
                  MyFilms.r[MyFilms.conf.StrIndex][wproperty] = "";
                  break;
              }
              LogMyFilms.Debug("Delete value for property '" + wproperty + "'");
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("Error deleting value for property '" + wproperty + "', exception: ", ex.Message);
            }
          }
          Update_XML_database();
          afficher_detail(true);
          #endregion
          break;

        case "updmediainfos":
          {
            #region update mediainfo
            string FileDate = string.Empty;
            string VideoPlaytime = string.Empty;
            string VideoCodec = string.Empty;
            string VideoFormat = string.Empty;
            string VideoFormatProfile = string.Empty;
            string VideoBitRate = string.Empty;
            string VideoFrameRate = string.Empty;
            string VideoWidth = string.Empty;
            string VideoHeight = string.Empty;
            string VideoAspectRatio = string.Empty;

            string AudioCodec = string.Empty;
            string AudioFormat = string.Empty;
            string AudioFormatProfile = string.Empty;
            string AudioBitrate = string.Empty;
            string AudioChannels = string.Empty;
            string AudioTracks = string.Empty;

            string TextCount = string.Empty;

            long TotalSize = 0;
            int TotalRuntime = 0;

            string source = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
            string[] files = source.Split(new Char[] { ';' });

            foreach (string file in files)
            {
              LogMyFilms.Debug("Mediainfo - Filename: '" + file + "'");
              if (System.IO.File.Exists(file))
              {
                // Get File Date Added/Created
                FileDate = GetFileTimeStamps(file);
                LogMyFilms.Debug("Mediainfo - FileDate: '" + FileDate + "'");

                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                TotalSize += fi.Length;

                // ReadMediaInfo(file, ref mediainfo);
                MediaInfo mediainfo = MediaInfo.GetInstance();

                // MediaInfo Object could not be created
                if (null == mediainfo)
                  return;

                // Check if File Exists and is not an Image type e.g. ISO (we can't extract mediainfo from that)
                if (System.IO.File.Exists(file) && !Helper.IsImageFile(file))
                {
                  try
                  {
                    LogMyFilms.Debug("Attempting to read Mediainfo for ", file);

                    // open file in MediaInfo
                    mediainfo.Open(file);

                    // check number of failed attempts at mediainfo extraction                    
                    int noAttempts = 0;

                    // Get Playtime (runtime)
                    string result = mediainfo.VideoPlaytime;

                    string LocalPlaytime = result != "-1" ? result : noAttempts.ToString();

                    VideoPlaytime = mediainfo.VideoPlaytime;
                    LogMyFilms.Debug("Mediainfo - VideoPlaytime: '" + VideoPlaytime + "'");
                    TotalRuntime += Int32.Parse(VideoPlaytime);

                    VideoCodec = mediainfo.VideoCodec;
                    LogMyFilms.Debug("Mediainfo - Videocodec: '" + VideoCodec + "'");
                    VideoFormat = mediainfo.VideoCodecFormat;
                    LogMyFilms.Debug("Mediainfo - VideoCodecFormat: '" + VideoFormat + "'");
                    VideoFormatProfile = mediainfo.VideoFormatProfile;
                    LogMyFilms.Debug("Mediainfo - VideoFormatProfile: '" + VideoFormatProfile + "'");
                    VideoBitRate = mediainfo.VideoBitrate;
                    LogMyFilms.Debug("Mediainfo - VideoBitrate: '" + VideoBitRate + "'");
                    VideoFrameRate = mediainfo.VideoFramesPerSecond;
                    LogMyFilms.Debug("Mediainfo - VideoFramesPerSecond: '" + VideoFrameRate + "'");
                    VideoWidth = mediainfo.VideoWidth;
                    LogMyFilms.Debug("Mediainfo - VideoWidth: '" + VideoWidth + "'");
                    VideoHeight = mediainfo.VideoHeight;
                    LogMyFilms.Debug("Mediainfo - VideoHeight: '" + VideoHeight + "'");
                    VideoAspectRatio = mediainfo.VideoAspectRatio;
                    LogMyFilms.Debug("Mediainfo - VideoAspectRatio: '" + VideoAspectRatio + "'");

                    AudioCodec = mediainfo.AudioCodec;
                    LogMyFilms.Debug("Mediainfo - AudioCodec: '" + AudioCodec + "'");
                    AudioFormat = mediainfo.AudioCodecFormat;
                    LogMyFilms.Debug("Mediainfo - AudioCodecFormat: '" + AudioFormat + "'");
                    AudioFormatProfile = mediainfo.AudioFormatProfile;
                    LogMyFilms.Debug("Mediainfo - AudioFormatProfile: '" + AudioFormatProfile + "'");
                    AudioBitrate = mediainfo.AudioBitrate;
                    LogMyFilms.Debug("Mediainfo - AudioBitrate: '" + AudioBitrate + "'");
                    AudioChannels = mediainfo.AudioChannelCount;
                    LogMyFilms.Debug("Mediainfo - AudioChannelCount: '" + AudioChannels + "'");
                    AudioTracks = mediainfo.AudioStreamCount;
                    LogMyFilms.Debug("Mediainfo - AudioStreamCount: '" + AudioTracks + "'");

                    TextCount = mediainfo.SubtitleCount;
                    LogMyFilms.Debug("Mediainfo - SubtitleCount: '" + TextCount + "'");

                    // MediaInfo cleanup
                    mediainfo.Close();
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("Error reading MediaInfo: ", ex.Message);
                  }

                }
                LogMyFilms.Debug("File '" + file + "' does not exist or is an image file");


              }
              else
                LogMyFilms.Debug("Mediainfo - File '" + file + "' does not exist !");
            }
            int size = (int)(TotalSize / 1024 / 1024);
            string strSize = string.Format("{0}", size);
            //string humanKBSize = string.Format("{0} KB", size);
            //string humanMBSize = string.Format("{0} MB", size / 1024);
            //string humanGBSize = string.Format("{0} GB", size / 1024 / 1024);
            LogMyFilms.Debug("Mediainfo - TotalSize: '" + strSize + "'");
            LogMyFilms.Debug("Mediainfo - TotalRuntime: '" + TotalRuntime.ToString() + "'");

            //MyFilms.r[MyFilms.conf.StrIndex]["Length"] = TotalRuntime.ToString();
            //MyFilms.r[MyFilms.conf.StrIndex]["VideoFormat"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["VideoBitrate"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["AudioFormat"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["AudioBitrate"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["Resolution"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["Framerate"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["Languages"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["Subtitles"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["DateAdded"] = string.Empty;
            //MyFilms.r[MyFilms.conf.StrIndex]["Size"] = string.Format("{0}", size); ;
            //MyFilms.r[MyFilms.conf.StrIndex]["Disks"] = split.Count().ToString();
            //MyFilms.r[MyFilms.conf.StrIndex]["Aspectratio"] = string.Empty;
            //Update_XML_database();
            //afficher_detail(true);
            #endregion
          }
          break;

        case "grabber":
          #region grabber
          bool wChooseScript = MyFilms.conf.StrGrabber_ChooseScript;
          if (!System.IO.File.Exists(MyFilms.conf.StrGrabber_cnf) && !MyFilms.conf.StrGrabber_ChooseScript)
          {
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
            dlgYesNo.SetLine(1, string.Format(GUILocalizeStrings.Get(1079875), MyFilms.conf.StrGrabber_cnf));//File doesn't exist. Do you want to choose it ?
            dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079878));//confirm suppression
            dlgYesNo.DoModal(GetID);
            if (dlgYesNo.IsConfirmed)
              wChooseScript = true;
            else
            {
              LogMyFilms.Info("My Films : The grabber config file doesn't exists. No grab Information done !");
              break;
            }
          }
          title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
          mediapath = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
          sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);

          // this will be executed after background thread finished
          doUpdateDetailsViewByFinishEvent = true;
          grabb_Internet_Informations(title, GetID, wChooseScript, MyFilms.conf.StrGrabber_cnf, mediapath, GrabType.All, false, sTitles, m_SearchAnimation);
          // afficher_detail(true); // -> will be executes by OnDetailsUpdated message handler later ...
          SetProcessAnimationStatus(false, m_SearchAnimation); // make sure it's switched off
          #endregion
          break;

        case "ant-nfo-reader":
          GUIUtils.ShowOKDialog("Action not yet implemented");
          break;

        case "ant-nfo-writer":
          GUIUtils.ShowOKDialog("Action not yet implemented");
          break;

        case "trailer-tmdb":
          if (!Helper.IsOnlineVideosAvailableAndEnabled)
          {
            GUIUtils.ShowNotifyDialog("OnlineVideos is not available!");
            return;
          }
          SearchAndDownloadTrailerOnlineTMDB(MyFilms.r, MyFilms.conf.StrIndex, false, true, (MyFilms.conf.StrDirStorTrailer.Length > 0) ? MyFilms.conf.StrDirStorTrailer : null); // MyFilmsDetail.SearchAndDownloadTrailerOnlineTMDB(r, MyFilms.conf.StrIndex, false, true, null);
          break;

        case "cover-thumbnailer":
          CreateThumbFromMovie();
          GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(107986), "Cover created from movie");
          //ToDo: Add Dialog to let user choose, if cover should replace existing one (remove skip existing logic for that!)
          break;

        case "trailer-register":
          {
            SetProcessAnimationStatus(true, m_SearchAnimation);
            //Zuerst Pfad lesen, dann Dateien suchen, in liste packen, Auswahlmenü präsentieren und zum Schluß Update des Records 
            // Suchen nach Files mit folgendem Kriterium:
            // 1.) ... die den Filmnamen im Filenamen haben und im Trailerverzeichnis gefunden werden (wahrscheinlich HD, daher an 1. Stelle setzen)
            // 2.) Im Verzeichnis des Films suchen nach Filmdateien die das Wort "Trailer" im Namen haben (Endung beliebig: avi, mov, flv, etc.)
            // 3.) Im (Trailer)-Suchpfad nach Verzeichnissen, die nach dem Filmnamen benannt sind - dann alle Files darin registrien

            LogMyFilms.Debug("SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex]: '" + (MyFilms.r[MyFilms.conf.StrIndex] + "'"));
            SearchTrailerLocal(MyFilms.r, MyFilms.conf.StrIndex, true, true);
            SetProcessAnimationStatus(false, m_SearchAnimation);
            afficher_detail(true);
            GUIUtils.ShowOKDialog(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle].ToString(), result.Count.ToString() + " " + GUILocalizeStrings.Get(10798705), "", ""); // trailers found ! 
            break;
          }

        case "trailer-delete":
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
          dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            DeleteTrailerFromDB(MyFilms.r, MyFilms.conf.StrIndex);
            //MyFilms.r = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
            afficher_detail(true);
          }
          break;

        case "personimages":
          if (!Helper.IsConnectedToInternet())
          {
            GUIUtils.ShowErrorDialog(GUILocalizeStrings.Get(703));
            break; // stop, if no internet available
          }
          Menu_LoadPersonImages();
          break;

        case "fanart":
          if (!Helper.IsConnectedToInternet())
          {
            GUIUtils.ShowErrorDialog(GUILocalizeStrings.Get(703));
            break; // stop, if no internet available
          }
          // Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false); // do not remove, as user might only want to "update" fanart ...
          Menu_LoadFanart(false);
          break;

        case "createfanart": // create fanart from local media
          Menu_CreateFanart(GrabUtil.ArtworkFanartType.MultiImageWithMultipleSingleImages);
          break;

        case "createfanartsingleimages": // create single images fanart from local media
          Menu_CreateFanart(GrabUtil.ArtworkFanartType.MultipleSingleImages);
          break;

        case "createfanartmultiimage": // create single fanart from local media on pause position
          Menu_CreateFanart(GrabUtil.ArtworkFanartType.Multiimage);
          break;

        case "createfanartonposition": // create single fanart from local media on pause position
          Menu_CreateFanart_OnMoviePosition();
          afficher_detail(true);
          break;

        case "deletefanart":
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079874));//Delete fanart
          dlgYesNo.SetLine(1, "");
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
            Remove_Backdrops_Fanart(sTitles.FanartTitle, false);
            Remove_Backdrops_Fanart(sTitles.OriginalTitle, false);
            Remove_Backdrops_Fanart(sTitles.TranslatedTitle, false);
            Remove_Backdrops_Fanart(sTitles.FormattedTitle, false);
            Remove_Backdrops_Fanart(sTitles.MasterTitle, false);
            Remove_Backdrops_Fanart(sTitles.SecondaryTitle, false);
            Remove_Backdrops_Fanart(sTitles.SortTitle, false);
            Thread.Sleep(50);
            afficher_detail(true);
          }
          break;

        case "changecover":
          string covercount = ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, false);
          afficher_detail(true);
          SetProcessAnimationStatus(false, m_SearchAnimation);
          Change_Menu("fanartcovermenu"); // stay in cover toggle menu
          break;

        case "covermanager":
          LogMyFilms.Debug("Switching to Cover Manager Window");
          MyFilmsCoverManager cm = null;
          if (cm == null)
          {
            cm = new MyFilmsCoverManager();
            GUIWindow cmwindow = (GUIWindow)cm;
            GUIWindowManager.Add(ref cmwindow);
            cm.Init();
          }
          // cm.MovieID = MyFilms.conf.StrIndex; // will be set later in cm class
          cm.setPageTitle("Cover Manager");
          //sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex));
          //cm.ArtworkFileName = GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);
          GUIWindowManager.ActivateWindow(cm.GetID, false);
          break;

        case "fanartmanager":
          #region fanart manager
          LogMyFilms.Info("Fanart Manager : Not yet implemented - using old submenu instead!");
          if (dlgmenu == null) return;
          dlgmenu.Reset();
          choiceViewMenu.Clear();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798767)); // Fanart Manager ...

          if (MyFilms.conf.StrFanart)
          {
            dlgmenu.Add(GUILocalizeStrings.Get(1079874)); // Remove Fanart
            choiceViewMenu.Add("deletefanart");

            dlgmenu.Add(GUILocalizeStrings.Get(1079851)); // Create single fanart from movie as multi image (local)
            choiceViewMenu.Add("createfanartmultiimage");

            dlgmenu.Add(GUILocalizeStrings.Get(1079849)); // create single images fanart from movie (local)
            choiceViewMenu.Add("createfanartsingleimages");

            dlgmenu.Add(GUILocalizeStrings.Get(1079853)); // create fanart from movie (local)
            choiceViewMenu.Add("createfanart");

            if (!g_Player.Stopped && g_Player.HasVideo && !string.IsNullOrEmpty(g_Player.CurrentFile))
            {
              dlgmenu.Add(GUILocalizeStrings.Get(1079852)); // Create single fanart from movie on current position (local)
              choiceViewMenu.Add("createfanartonposition");
            }

            dlgmenu.Add(GUILocalizeStrings.Get(1079862)); // Download Fanart (online)
            choiceViewMenu.Add("fanart");
          }

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1)
          {
            Change_Menu("mainmenu");
            return;
          }
          Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
          #endregion
          break;

        case "loadcover":
          title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
          mediapath = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
          sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
          grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, GrabType.Cover, false, sTitles, m_SearchAnimation);
          afficher_detail(true);
          SetProcessAnimationStatus(false, m_SearchAnimation); // make sure it's switched off
          break;

        case "loadmulticover":
          title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
          mediapath = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
          sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
          grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, GrabType.MultiCovers, false, sTitles, m_SearchAnimation);
          afficher_detail(true);
          SetProcessAnimationStatus(false, m_SearchAnimation); // make sure it's switched off
          break;

        case "tmdbposter":
          Menu_LoadTMDBposter();
          afficher_detail(true);
          break;

        default: // Main Contextmenu
          GUIUtils.ShowOKDialog("Action not yet implemented");
          return;
      }
    }

    internal void LaunchOnlineVideos(string site)
    {
      string titleextension = string.Empty;
      string path = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
      if (path.Contains(";")) path = path.Substring(0, path.IndexOf(";", System.StringComparison.Ordinal));
      if (path.Contains("\\")) path = path.Substring(0, path.LastIndexOf("\\", System.StringComparison.Ordinal));

      path = Path.Combine(path, "Trailer");
      if (!Directory.Exists(path))
        try
        {
          Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("Error creating downloadpath: '" + ex.Message + "'");
        }

      switch (site)
      {
        case "YouTube":
          titleextension = " " + MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer" + (MyFilms.conf.GrabberOverrideLanguage.Length > 0 ? " " + MyFilms.conf.GrabberOverrideLanguage : "");
          break;
        case "YouTubeMore":
          titleextension = " trailer";
          break;
        case "iTunes Movie Trailers":
        case "IMDb Movie Trailers":
          break;
      }

      // OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>     downloaddir:<path>|downloadfilename:<filename>|downloadmenuentry:<menu text>
      if (Helper.IsOnlineVideosAvailableAndEnabled)
      {
        string title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");

        string oVstartparams = "site:" + site + "|category:|search:" + title + titleextension + "|return:Locked" +
                               "|downloaddir:" + path + "|downloadmenuentry:" + GUILocalizeStrings.Get(10798749) + " (" + title + ")"; // MyFilms: Download to movie directory

        InitTrailerwatcher(); // enable Trailerwatcher for the movie path, in case the user is downloading a trailer there ...
        if (Helper.IsOnlineVideosAvailableAndEnabled) InitOVEventHandler();
        else LogMyFilms.Error("Error subscribing to 'VideoDownloaded' event from OnlineVideos - you need OV V1.2+ installed and enabled !");

        LogMyFilms.Debug("Starting OnlineVideos with '" + oVstartparams + "'");
        // trailerPlayed = true; // should this be set here to make original movie doesn't get set to watched??

        GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.OnlineVideos, oVstartparams);
      }
      else
      {
        GUIUtils.ShowOKDialog("MyFilms", "OnlineVideo plugin not installed or wrong version" + @"\n" + "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.OnlineVideos));
      }
    }

    private void LoadOnlineVideosViews()
    {
      onlineVideosViews = GetOnlineVideosViews();
      theOnlineVideosViews.Clear();

      foreach (KeyValuePair<string, string> ovv in onlineVideosViews)
      {
        theOnlineVideosViews.Add(ovv.Value);
      }
    }

    private List<KeyValuePair<string, string>> GetOnlineVideosViews()
    {
      // check if we have already got themargument (filestorage)
      if (onlineVideosViews.Count == 0)
      {
        // set path of config file, so we load user settings
        ExternalPlugins::OnlineVideos.OnlineVideoSettings.Instance.ConfigDir = Config.GetDirectoryInfo(Config.Dir.Config).ToString();

        // load list of sites
        ExternalPlugins::OnlineVideos.OnlineVideoSettings onlineVideos = ExternalPlugins::OnlineVideos.OnlineVideoSettings.Instance;
        onlineVideos.LoadSites();

        foreach (var view in from site in onlineVideos.SiteSettingsList where site.IsEnabled select new KeyValuePair<string, string>(site.Name, site.Name))
        {
          onlineVideosViews.Add(view);
        }
      }
      return onlineVideosViews;
    }

    private void Menu_LoadTMDBposter()
    {
      //Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      Searchtitles stitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
      if (string.IsNullOrEmpty(stitles.FanartTitle) && MyFilms.conf.StrFanart)
        return;
      if (MyFilms.conf.StrFanart)
      {
        Download_TMDB_Posters(stitles.OriginalTitle, stitles.TranslatedTitle, stitles.Director, stitles.Year.ToString(), true, GetID, stitles.OriginalTitle, m_SearchAnimation);
      }
    }

    private void Menu_CreateFanart_OnMoviePosition()
    {
      int duration = (int)g_Player.Duration;
      int currentposition = (int)g_Player.CurrentPosition;
      string file = g_Player.CurrentFile;
      string title = g_Player.currentTitle;
      Menu_CreateFanart_OnMoviePosition_Parameterized(duration, currentposition, file, title);
    }

    private void Menu_CreateFanart_OnMoviePosition_Parameterized(int duration, int currentposition, string file, string title)
    {
      //int duration = (int)g_Player.Duration;
      //int currentposition = (int)g_Player.CurrentPosition;
      //string file = g_Player.CurrentFile;
      //string title = g_Player.currentTitle;

      if (!g_Player.Stopped && g_Player.HasVideo && !string.IsNullOrEmpty(g_Player.CurrentFile)) // g_Player.Paused && 
      {
        GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
        if (dlgPrgrs != null)
        {
          dlgPrgrs.Reset();
          dlgPrgrs.DisplayProgressBar = false;
          dlgPrgrs.ShowWaitCursor = true;
          dlgPrgrs.DisableCancel(true);
          dlgPrgrs.SetHeading(GUILocalizeStrings.Get(1079847)); // MyFilms Fanart Creator
          dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
          dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079848)); // Creating new Fanart from movie
          dlgPrgrs.Percentage = 0;
        }
        new Thread(delegate()
        {
          Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
          if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
          {
            // Remove_Backdrops_Fanart(fanartTitle, false); // old: // Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false);
            // Thread.Sleep(50);
            bool success = GrabUtil.GetFanartFromMovie(sTitles.FanartTitle, sTitles.Year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.ArtworkFanartType.Snapshotimage, file, "localfanart", currentposition);
          }
          if (dlgPrgrs != null)
            dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); Thread.Sleep(50); dlgPrgrs.Close(); // Done ...
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            // this will be executed after background thread finished
            doUpdateDetailsViewByFinishEvent = true;
            if (DetailsUpdated != null)
              DetailsUpdated(true); // will launch afficher_detail(true) via message handler
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsFanartCreator", IsBackground = true }.Start();
      }
      else
      {
        GUIUtils.ShowOKDialog("MyFilms - '" + title + "'", "Unable to create fanart from movie position !" + @"\n" + "No movie active ?"); // Show message with hints
      }
    }

    private void Menu_CreateFanart(GrabUtil.ArtworkFanartType fanartType)
    {
      {
        GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
        if (dlgPrgrs != null)
        {
          dlgPrgrs.Reset();
          dlgPrgrs.DisplayProgressBar = false;
          dlgPrgrs.ShowWaitCursor = true;
          dlgPrgrs.DisableCancel(true);
          dlgPrgrs.SetHeading(GUILocalizeStrings.Get(1079847)); // MyFilms Fanart Creator
          dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
          dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079848)); // Creating new Fanart from movie
          dlgPrgrs.Percentage = 0;
        }
        new Thread(delegate()
          {
            string movieFile = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);

            Searchtitles stitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
            LogMyFilms.Debug("Menu_CreateFanart(): movieFile: '" + movieFile + "', fanartTitle: '" + stitles.FanartTitle + "'");
            if (!string.IsNullOrEmpty(stitles.FanartTitle) && MyFilms.conf.StrFanart && !string.IsNullOrEmpty(movieFile))
            {
              // Remove_Backdrops_Fanart(fanartTitle, false); // old: Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false);
              // Thread.Sleep(50);
              bool success;
              switch (fanartType)
              {

                case GrabUtil.ArtworkFanartType.MultiImageWithMultipleSingleImages:
                  success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.Year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.ArtworkFanartType.MultiImageWithMultipleSingleImages, movieFile, "localfanart", 0);
                  break;
                case GrabUtil.ArtworkFanartType.Multiimage:
                  success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.Year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.ArtworkFanartType.Multiimage, movieFile, "localfanart", 0);
                  break;
                case GrabUtil.ArtworkFanartType.MultipleSingleImages:
                  success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.Year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.ArtworkFanartType.MultipleSingleImages, movieFile, "localfanart", 0);
                  break;
              }
            }

            if (dlgPrgrs != null)
              dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); Thread.Sleep(50); dlgPrgrs.Close(); // Done ...
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              // this will be executed after background thread finished
              doUpdateDetailsViewByFinishEvent = true;
              if (DetailsUpdated != null)
                DetailsUpdated(true); // will launch afficher_detail(true) via message handler
              return 0;
            }, 0, 0, null);
          }) { Name = "MyFilmsFanartCreator", IsBackground = true }.Start();
      }
    }

    private void Menu_LoadFanart(bool loadPersonImages)
    {
      Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      string personartworkpath = string.Empty;
      Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
      string imdbid = GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
      if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
      {
        LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - director: '" + sTitles.Director + "' - year: '" + sTitles.Year.ToString() + "'");
        if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
        {
          personartworkpath = MyFilms.conf.StrPathArtist;
          LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
        }
        doUpdateDetailsViewByFinishEvent = true;
        Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director, imdbid, sTitles.Year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, true, loadPersonImages, m_SearchAnimation);
      }
    }

    private void Menu_LoadPersonImages()
    {
      AddPersonsToDownloadQueue();

      #region old method via fanart loader framework (disabled)
      //Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      //string personartworkpath = string.Empty;
      //Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
      //string imdbid = GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
      //if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
      //{
      //  LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - director: '" + sTitles.Director + "' - year: '" + sTitles.Year.ToString() + "'");
      //  personartworkpath = MyFilms.conf.StrPathArtist;
      //  LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
      //  doUpdateDetailsViewByFinishEvent = true;
      //  Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director.ToString(), imdbid, sTitles.Year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, false, true);
      //}
      #endregion
    }

    private void TraktShout(MFMovie movie)
    {
      LogMyFilms.Debug("TraktShout(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
      ExternalPlugins::TraktPlugin.TraktHelper.ShowMovieShouts(movie.IMDBNumber, movie.Title, movie.Year.ToString(), movie.Fanart);
      // replaced by helper call
      //TraktPlugin.GUI.GUIShouts.ShoutType = TraktPlugin.GUI.GUIShouts.ShoutTypeEnum.movie;
      //TraktPlugin.GUI.GUIShouts.MovieInfo = new TraktPlugin.GUI.MovieShout { IMDbId = movie.IMDBNumber, TMDbId = "", Title = movie.Title, Year = movie.Year.ToString() };
      //TraktPlugin.GUI.GUIShouts.Fanart = movie.Fanart;
    }

    private bool TraktInternalMenu(MFMovie movie)
    {
      bool success = false;
      try
      {
        LogMyFilms.Debug("TraktInternalMenu(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "', tmdb = '" + movie.TMDBNumber + "', actors = '" + movie.Actors.Count + "', Persons total = '" + (movie.Actors.Count + movie.Producers.Count + movie.Directors.Count + movie.Writers.Count) + "'");
        //public static bool ShowTraktExtMovieMenu(string title, string year, string imdbid, string fanart)
        //{
        //    return ShowTraktExtMovieMenu(title, year, imdbid, fanart, false);
        //}
        //public static bool ShowTraktExtMovieMenu(string title, string year, string imdbid, string fanart, bool showAll)
        //{
        //    return ShowTraktExtMovieMenu(title, year, imdbid, fanart, null, showAll);
        //}
        //public static bool ShowTraktExtMovieMenu(string title, string year, string imdbid, string fanart, SearchPeople people, bool showAll)
        
        ExternalPlugins::TraktPlugin.GUI.SearchPeople people = new ExternalPlugins::TraktPlugin.GUI.SearchPeople();
        people.Actors.AddRange(movie.Actors);
        people.Directors.AddRange(movie.Directors);
        people.Writers.AddRange(movie.Writers);
        people.Producers.AddRange(movie.Producers);
        success = ExternalPlugins::TraktPlugin.GUI.GUICommon.ShowTraktExtMovieMenu(movie.Title, movie.Year.ToString(), movie.IMDBNumber, movie.Fanart, people, true);
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("TraktInternalMenu(): Error - Exception '" + ex.Message + "'");
      }
      return success;
    }

    private void TraktRelatedMovies(MFMovie movie)
    {
      LogMyFilms.Debug("TraktRelatedMovies(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
      ExternalPlugins::TraktPlugin.TraktHelper.ShowRelatedMovies(movie.IMDBNumber, movie.Title, movie.Year.ToString());
    }

    private void TraktSearchPeople(MFMovie movie)
    {
      LogMyFilms.Debug("TraktSearchPersons(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
      ExternalPlugins::TraktPlugin.GUI.SearchPeople people = new ExternalPlugins::TraktPlugin.GUI.SearchPeople();
      people.Actors.AddRange(movie.Actors);
      people.Directors.AddRange(movie.Directors);
      people.Writers.AddRange(movie.Writers);
      people.Directors.AddRange(movie.Producers);
      ExternalPlugins::TraktPlugin.GUI.GUICommon.ShowSearchByMenu(people, movie.Title, movie.Fanart);
    }

    private void TraktAddRemoveMovieInUserlist(MFMovie movie, bool remove)
    {
      LogMyFilms.Debug("TraktAddRemoveMovieInUserlist(): Call with 'remove = " + remove + "' - Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "', file = '" + movie.File + "', path = '" + movie.Path + "'");
      ExternalPlugins::TraktPlugin.TraktHelper.AddRemoveMovieInUserList(movie.Title, movie.Year.ToString(), movie.IMDBNumber, remove);
    }

    private static MFMovie GetMovieFromRecord(DataRow sr)
    {
      var movie = new MFMovie();
      BaseMesFilms.GetMovieDetails(sr, MyFilms.conf, false, ref movie);
      movie.Config = MyFilmsPlugin.MyFilmsGUI.Configuration.CurrentConfig;

      #region disabled old code
      //movie.ID = !string.IsNullOrEmpty(sr["Number"].ToString()) ? Int32.Parse(sr["Number"].ToString()) : 0;

      //movie.Title = sr["OriginalTitle"].ToString();

      //int year = 1900;
      //Int32.TryParse(sr["Year"].ToString(), out year);
      //movie.Year = year;

      //bool played = false;
      //if (MyFilms.conf.EnhancedWatchedStatusHandling)
      //{
      //  if (GetWatchedCount(MyFilms.conf.StrIndex, MyFilms.conf.StrUserProfileName) > 0)
      //    played = true;
      //}
      //else
      //{
      //  if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
      //    if (sr[MyFilms.conf.StrWatchedField].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
      //      played = true;
      //  //if (MyFilms.conf.StrSuppressAutomatic && MyFilms.conf.StrSuppressField.Length > 0)
      //  //  if ((sr[MyFilms.conf.StrSuppressField].ToString() == MyFilms.conf.StrSuppressValue.ToString()) && (MyFilms.conf.StrSuppressPlayStopUpdateUserField))
      //  //    played = true;
      //}
      //movie.Watched = played;

      //float rating = 0;
      //bool success = float.TryParse(sr["Rating"].ToString(), out rating);
      //if (!success) rating = 0;

      //movie.Rating = rating;
      //// movie.Rating = (float)Double.Parse(sr["Rating"].ToString());

      //// movie.RatingUser = ????;

      //string mediapath = string.Empty;
      //if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
      //{
      //  mediapath = sr[MyFilms.conf.StrStorage].ToString();
      //  if (mediapath.Contains(";")) // take the first source file
      //    mediapath = mediapath.Substring(0, mediapath.IndexOf(";", System.StringComparison.Ordinal));
      //}
      //movie.File = mediapath;

      //if (string.IsNullOrEmpty(mediapath)) // e.g. offline media files
      //  movie.File = movie.Title + " {offline} [" + movie.ID + "]";

      //string currentPlayerFile = g_Player.CurrentFile;
      //if (!string.IsNullOrEmpty(currentPlayerFile))
      //  movie.File = currentPlayerFile;

      //string path = "";
      //if (!string.IsNullOrEmpty(mediapath))
      //{
      //  try
      //  {
      //    path = System.IO.Path.GetDirectoryName(mediapath);
      //  }
      //  catch (Exception)
      //  {
      //    if (!string.IsNullOrEmpty(movie.File))
      //      try
      //      {
      //        path = System.IO.Path.GetDirectoryName(movie.File);
      //      }
      //      catch (Exception)
      //      {
      //        path = "{search}";
      //      }
      //  }
      //  movie.Path = path;
      //}
      //else
      //{
      //  movie.Path = "{offline}";
      //}

      //string IMDB = "";
      //if (!string.IsNullOrEmpty(sr["IMDB_Id"].ToString()))
      //  IMDB = sr["IMDB_Id"].ToString();
      //if (!string.IsNullOrEmpty(sr["URL"].ToString()) && string.IsNullOrEmpty(IMDB))
      //{
      //  string CleanString = sr["URL"].ToString();
      //  Regex CutText = new Regex("" + @"tt\d{7}" + "");
      //  Match m = CutText.Match(CleanString);
      //  if (m.Success)
      //    IMDB = m.Value;
      //}
      //movie.IMDBNumber = IMDB;

      //if (!string.IsNullOrEmpty(sr["TMDB_Id"].ToString()))
      //  movie.TMDBNumber = sr["TMDB_Id"].ToString();
      //movie.DateAdded = sr["Date"].ToString();
      #endregion

      LogMyFilms.Info("GetMovieFromRecord(): Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "', file = '" + movie.File + "', path = '" + movie.Path + "'");
      return movie;
    }

    //-------------------------------------------------------------------------------------------
    //  Suppress an entry from the database
    //-------------------------------------------------------------------------------------------        
    private static void SuppressEntry(DataRow row)
    {

      if (MyFilms.conf.StrSuppressAutomaticActionType == "2" || MyFilms.conf.StrSuppressAutomaticActionType == "4")
      {
        var newItems = new ArrayList();
        bool noResumeMovie = true;
        int movieIndex = 0;

        SearchAllFiles(row, true, ref noResumeMovie, ref newItems, ref movieIndex, false, "");
        foreach (object t in from object t in newItems where File.Exists(t.ToString()) select t)
        {
          try
          {
            File.Delete(t.ToString());
            LogMyFilms.Info("file deleted : " + t);
          }
          catch
          {
            LogMyFilms.Info("unable to delete file : " + t);
          }
        }
      }
      if (MyFilms.conf.StrSuppressAutomaticActionType == "1" || MyFilms.conf.StrSuppressAutomaticActionType == "2")
      {
        string wdelTitle = row[MyFilms.conf.StrTitle1].ToString();
        row.Delete();
        MyFilms.conf.StrIndex = -1;
        LogMyFilms.Info("Database movie deleted : " + wdelTitle);
      }
      // Update_XML_database(); // already done on calling method ...
    }

    //-------------------------------------------------------------------------------------------
    //  Suppress an entry from the database
    //-------------------------------------------------------------------------------------------        
    internal static void ManualDelete(DataRow row, bool removefromDb, bool deletefromStorage)
    {
      LogMyFilms.Info("Manual_Delete(): remove from DB = '" + removefromDb + "', delete from storage = '" + deletefromStorage + "'");
      if (deletefromStorage)
      {
        var newItems = new ArrayList();
        bool noResumeMovie = true;
        int movieIndex = 0;

        SearchAllFiles(row, true, ref noResumeMovie, ref newItems, ref movieIndex, false, "");
        foreach (object t in from object t in newItems where File.Exists(t.ToString()) select t)
        {
          // for each entry test if it's a file, a directory or a dvd copy
          // no action for files on removible media or image files
          try
          {
            File.Delete(t.ToString());
            LogMyFilms.Info("file deleted : " + t);
          }
          catch (Exception ex)
          {
            LogMyFilms.Info("unable to delete file : " + t);
            LogMyFilms.Info(ex, "Manual_Delete() - delete file exception: ");
          }
        }
      }
      if (removefromDb)
      {
        string wdelTitle = row[MyFilms.conf.StrTitle1].ToString();
        //AntMovieCatalog.MovieRow RowToDelete = row;
        //foreach (AntMovieCatalog.CustomFieldsRow customFieldsRow in RowToDelete.GetCustomFieldsRows())
        //{
        //  customFieldsRow.Delete();
        //}
        row.Delete();
        LogMyFilms.Info("Database movie deleted : " + wdelTitle);
        Update_XML_database();
      }
    }

    //-------------------------------------------------------------------------------------------
    //  Set an entry from the database to watched/unwatched
    //-------------------------------------------------------------------------------------------        
    // public static void Watched_Toggle(DataRow[] r1, int Index, bool watched)
    public static void Watched_Toggle(DataRow row, bool watched)
    {
      if (MyFilms.conf.EnhancedWatchedStatusHandling)
      {
        var userData = new MultiUserData(row[BaseMesFilms.MultiUserStateField].ToString());
        userData.SetWatched(MyFilms.conf.StrUserProfileName, watched);
        row[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
        SyncMusToExtendedFields(row, userData, MyFilms.conf.StrUserProfileName);
      }
      else
      {
        row[MyFilms.conf.StrWatchedField] = watched ? "true" : MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
      }
      LogMyFilms.Info("Database movie changed 'watchedstatus' by setting '" + MyFilms.conf.StrWatchedField + "' to '" + row[MyFilms.conf.StrWatchedField] + "' for movie: " + row[MyFilms.conf.StrTitle1]);

      Update_XML_database();

      // tell any listeners that user rated the movie
      MFMovie movie = GetMovieFromRecord(row);
      if (WatchedItem != null)
        WatchedItem(movie, watched, ((watched) ? 1 : 0));
    }

    //-------------------------------------------------------------------------------------------
    //  Add watch count by one
    //-------------------------------------------------------------------------------------------        
    private static void AddWatchedCount(DataRow row, string userprofilename)
    {
      //// ToDo: Could also populate "watched history" here (future version)
      var userData = new MultiUserData(row[BaseMesFilms.MultiUserStateField].ToString());
      userData.AddWatchedCountByOne(userprofilename);
      row[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
      SyncMusToExtendedFields(row, userData, userprofilename);

      // following is disabled, as we already send MovieWatched message, that is used by registered apps. We loose setting the number of watches for the external apps though
      //// tell any listeners that user watched the movie
      //MFMovie movie = GetMovieFromRecord(row);
      //if (WatchedItem != null && MyFilms.conf.AllowTraktSync)
      //  WatchedItem(movie, true, userData.GetUserState(userprofilename).WatchedCount);
    }

    private static void SyncMusToExtendedFields(DataRow row, MultiUserData userData, string userprofilename)
    {
      row["DateWatched"] = (userData.GetUserState(userprofilename).WatchedDate == MultiUserData.NoWatchedDate || userData.GetUserState(MyFilms.conf.StrUserProfileName).Watched == false) ? Convert.DBNull : userData.GetUserState(userprofilename).WatchedDate;
      row["RatingUser"] = (userData.GetUserState(userprofilename).UserRating == -1) ? Convert.DBNull : userData.GetUserState(userprofilename).UserRating;
      row[MyFilms.conf.StrWatchedField] = userData.GetUserState(userprofilename).Watched ? "true" : MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
      // row["LastPosition"] = (userData.GetUserState(userprofilename).ResumeData == -1) ? Convert.DBNull : userData.GetUserState(userprofilename).ResumeData;
      //if (MyFilms.conf.StrUserProfileName.Length > 0 && row["RatingUser"] != System.Convert.DBNull && row["RatingUser"] != MultiUserData.NoRating)
      //{
      //  string newValue = (row["RatingUser"] > MultiUserData.FavoriteRating) ? MultiUserData.Add(row["Favorite"].ToString(), MyFilms.conf.StrUserProfileName) : MultiUserData.Remove(row["Favorite"].ToString(), MyFilms.conf.StrUserProfileName);
      //  row["Favorite"] = (string.IsNullOrEmpty(newValue)) ? Convert.DBNull : newValue;
      //}
    }

    //-------------------------------------------------------------------------------------------
    //  Update the XML database and refresh screen
    //-------------------------------------------------------------------------------------------        
    public static void Update_XML_database()
    {
      //int maxretries = 10; // max retries 10 * 1000 = 10 seconds
      //int i = 0;
      //bool success = false; // result of update operation

      //while (!success && i < maxretries)
      //{
      //  // first check, if there is a global manual lock
      //  if (!GlobalLockIsActive(MyFilms.conf.StrFileXml))
      //  {
      //    SetGlobalLock(true, MyFilms.conf.StrFileXml);
      //    bool writesuccessful = BaseMesFilms.SaveMyFilms();
      //    SetGlobalLock(false, MyFilms.conf.StrFileXml);
      //    if (writesuccessful)
      //    {
      //      LogMyFilms.Info("Movie Database updated to filesystem!");
      //      success = true;
      //    }
      //  }
      //  else
      //  {
      //    i += 1;
      //    LogMyFilms.Info("Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
      //    Thread.Sleep(1000);
      //  }
      //}

      
      new Thread(delegate()
      {
        bool successwrite = BaseMesFilms.SaveMyFilms(MyFilms.conf.StrFileXml, 10000);

        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          // this will be executed after background thread finished
          if (!successwrite)
          {
            GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(1079861), "DB could not be updated (locked) !");
            LogMyFilms.Warn("Movie Database NOT updated due to GlobalLock ! - timeout passed.");
          }
          //else
          //  GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(1079861), "DB successfully updated !");
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsUpdateXML", IsBackground = true }.Start();
    }

    //-------------------------------------------------------------------------------------------
    //  Get Global Lock
    //-------------------------------------------------------------------------------------------        
    public static bool GlobalLockIsActive(string config)
    {
      string db = string.IsNullOrEmpty(config) ? MyFilms.conf.StrFileXml : config;
      if (string.IsNullOrEmpty(db))
      {
        db = "";
        LogMyFilms.Debug("GlobalLockIsActive() - No valid DB - returning 'false' (DB-Config: '" + db + "')");
        return false;
      }

      string path = Path.GetDirectoryName(db);
      string filename = Path.GetFileNameWithoutExtension(db);
      string machineName = System.Environment.MachineName;
      string[] files = Directory.GetFiles(path, filename + @"*.lck", SearchOption.TopDirectoryOnly);
      if (files.Length > 0)
      {
        LogMyFilms.Debug("GlobalLockIsActive() - Global Lock detected ! (DB-Config: '" + db + "') - First LockFile: '" + files[0] + "', Number LockFiles: '" + files.Length + "', Local MachineName: '" + machineName + "'");
        return true;
      }
      else
      {
        LogMyFilms.Debug("GlobalLockIsActive() - No Global Lock detected ! (DB-Config: '" + db + "')");
        return false;
      }
    }

    //-------------------------------------------------------------------------------------------
    //  Set Global Lock
    //-------------------------------------------------------------------------------------------        
    public static void SetGlobalLock(bool lockstate, string config)
    {
      string strLockFileName = LockFilename(config);
      if (lockstate)
      {

        MyFilms.FSwatcher.EnableRaisingEvents = false; // stop FSwatcher for local update, otherwise unneeded reread would be triggered

        try
        {
          File.Create(strLockFileName).Dispose();
          LogMyFilms.Debug("SetGlobalLock() - successfully created global lock ! - " + strLockFileName);
        }
        catch (Exception ex)
        {
          LogMyFilms.Fatal(ex, "SetGlobalLock() - Error creating Lockfile - check if file system rights properly set! - Lockfile: '" + strLockFileName + "', exception: " + ex.Message);
          // throw;
        }
      }
      else
      {
        if (File.Exists(strLockFileName))
        {
          try
          {
            File.Delete(strLockFileName);
            LogMyFilms.Debug("RemoveGlobalLock() - removed global lock ! - " + strLockFileName);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("RemoveGlobalLock() - there seems to be a problem removing global lock - Message: " + ex.Message);
          }
        }
        try
        {
          if (MyFilms.FSwatcher.Path.Length > 0) // only try enabling, if there is a path already set !
            MyFilms.FSwatcher.EnableRaisingEvents = true;
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("RemoveGlobalLock()- FSwatcher - problem enabling Raisingevents - Message (file not yet set?):  '" + ex.Message);
        }
      }
    }

    //-------------------------------------------------------------------------------------------
    //  Create machine specific Lock Filename
    //-------------------------------------------------------------------------------------------        
    private static string LockFilename(string config)
    {
      string path = Path.GetDirectoryName(config);
      string filename = Path.GetFileNameWithoutExtension(config);
      string machineName = Environment.MachineName;
      string lockfilename = path + @"\" + filename + "_" + machineName + ".lck";
      // LogMyFilms.Debug("LockFilename() - created lock file name is: '" + lockfilename + "'");
      return lockfilename;
    }

    public static bool AddMovieToCollection(DataRow movieRow, string newGroupName, bool save)
    {
      string oldtitle = movieRow[MyFilms.conf.StrTitle1].ToString();
      if (oldtitle.IndexOf(MyFilms.conf.TitleDelim, System.StringComparison.Ordinal) > 0) // already has a groupname
      {
        LogMyFilms.Debug("AddMovieToCollection() - cannot add movie to collection '" + newGroupName + "' - already part of collection - oldtitle = '" + oldtitle + "'");
        return false;
      }
      else
      {
        string newtitle = newGroupName + MyFilms.conf.TitleDelim + oldtitle;
        LogMyFilms.Debug("AddMovieToCollection() - changing title from '" + oldtitle + "' to '" + newtitle + "'");
        movieRow[MyFilms.conf.StrTitle1] = newtitle;
        if (save) Update_XML_database();
        return true;
      }
    }

    public static bool RemoveMovieFromCollection(DataRow movieRow, bool save)
    {
      string oldtitle = movieRow[MyFilms.conf.StrTitle1].ToString();
      if (oldtitle.IndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) > 0)
      {
        string newtitle = oldtitle.Substring(oldtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
        LogMyFilms.Debug("RemoveMovieFromCollection() - changing title from '" + oldtitle + "' to '" + newtitle + "'");
        movieRow[MyFilms.conf.StrTitle1] = newtitle;
        if (save) Update_XML_database();
        return true;
      }
      else
      {
        LogMyFilms.Debug("RemoveMovieFromCollection() - cannot remove from collection - oldtitle = '" + oldtitle + "'");
        return false;
      }
    }

    //-------------------------------------------------------------------------------------------
    //  Get Search title
    //-------------------------------------------------------------------------------------------        
    public static string GetSearchTitle(DataRow[] r1, int index, string titleoption)
    {
      string title = "";
      if (Helper.FieldIsSet(MyFilms.conf.ItemSearchGrabber) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.ItemSearchGrabber].ToString()))
      {
        title = r1[index][MyFilms.conf.ItemSearchGrabber].ToString(); // Configured GrabberTitle
        LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with '" + MyFilms.conf.ItemSearchGrabber + "' = '" + title + "'");
      }
      else if (Helper.FieldIsSet(MyFilms.conf.StrTitle1) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.StrTitle1].ToString())) // Master Title
      {
        title = r1[index][MyFilms.conf.StrTitle1].ToString();
        LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with (master)title = '" + title + "'");
      }
      else if (Helper.FieldIsSet(MyFilms.conf.StrTitle2) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.StrTitle2].ToString())) // Secondary title
      {
        title = r1[index][MyFilms.conf.StrTitle2].ToString();
        LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with (secondary)title = '" + title + "'");
      }
      else if (Helper.FieldIsSet(MyFilms.conf.StrStorage) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.StrStorage].ToString())) // Name from source (media)
      {
        title = r1[index][MyFilms.conf.StrStorage].ToString();
        if (title.Contains(";")) title = title.Substring(0, title.IndexOf(";", StringComparison.Ordinal));
        if (title.Contains("\\")) title = title.Substring(title.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        if (title.Contains(".")) title = title.Substring(0, title.LastIndexOf(".", StringComparison.Ordinal));
        LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with (media source)name = '" + title + "'");
      }

      if (title.IndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) > 0)
        title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) + 1);
      LogMyFilms.Debug("GetSearchTitle() - returning (search)title = '" + title + "'");
      return title;
    }

    public static string GetMediaPathOfFirstFile(DataRow[] r1, int index)
    {
      string mediapath = "";

      // for catalog using "storage field"
      if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
      {
        try
        {
          mediapath = r1[index][MyFilms.conf.StrStorage].ToString();
          if (mediapath.Contains(";"))
            mediapath = mediapath.Substring(0, mediapath.IndexOf(";", System.StringComparison.Ordinal));
        }
        catch
        {
          mediapath = string.Empty;
        }
      }

      // for catalog using "search" instead storage field
      if (string.IsNullOrEmpty(mediapath)) // use search method only if required...
      {
        if ((MyFilms.conf.SearchFile == "True") || (MyFilms.conf.SearchFile == "yes"))
        {
          string movieName = r1[index][MyFilms.conf.ItemSearchFile].ToString();
          movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim, System.StringComparison.Ordinal) + 1).Trim();
          if (MyFilms.conf.ItemSearchFile.Length > 0)
          {
            mediapath = SearchFileName(movieName, MyFilms.conf.StrDirStor).Trim();
          }
        }
      }

      return mediapath;
    }

    private static bool ShouldGrabberBeAdded(GrabberScript script, GrabType grabtype, bool showallLanguages)
    {
      bool add = false;
      string currentlanguagefilter = "";
      if (!string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabberScriptsFilter))
        currentlanguagefilter = MyFilms.conf.ItemSearchGrabberScriptsFilter;
      string[] sep = new string[] { ",", ";", "|", "/", ".", @"\", ":" };
      switch (grabtype)
      {
        case GrabType.All:
        case GrabType.Details:
        case GrabType.Cover:
        case GrabType.MultiCovers:
        case GrabType.Fanart:
        case GrabType.Person:
        case GrabType.Photos:
        case GrabType.Trailers:
          break;
      }

      // check, if it meets filter criteria
      string[] allowedlanguages = currentlanguagefilter.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      string[] supportedlanguages = script.Language.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      string[] supportedfunctions = script.Type.Split(sep, StringSplitOptions.RemoveEmptyEntries);

      if (string.IsNullOrEmpty(currentlanguagefilter) || currentlanguagefilter.Contains("*")) // if there is no filter set in config or override add script anyway...
      {
        showallLanguages = true;
      }

      if (supportedfunctions.Length == 0 && (grabtype == GrabType.Details || grabtype == GrabType.All)) // if there is no functions set and trying legacy menu ...
        add = true;
      else
      {
        foreach (string supportedfunction in supportedfunctions.Where(supportedfunction => supportedfunction.Trim().ToLower() == grabtype.ToString().ToLower() || (supportedfunction.Trim().ToLower() == "details" && grabtype.ToString().ToLower() == "all")))
        {
          if (showallLanguages)
            add = true;
          else
          {
            foreach (string allowedlanguage in from allowedlanguage in allowedlanguages from supportedlanguage in supportedlanguages where supportedlanguage.Trim().ToLower() == allowedlanguage.Trim().ToLower() select allowedlanguage)
            {
              add = true;
            }
          }
        }
      }
      return add;
    }

    //-------------------------------------------------------------------------------------------
    //  Grab URL Internet Movie Informations and update the XML database and refresh screen
    //  -> Selection of grabber script
    //-------------------------------------------------------------------------------------------        
    public static void grabb_Internet_Informations(string fullMovieName, int GetID, bool choosescript, string wscript, string fullMoviePath, GrabType grabtype, bool showAll, Searchtitles sTitles, GUIAnimation searchanimation)
    {
      LogMyFilms.Debug("(grabb_Internet_Informations) with grabtype = '" + grabtype + "', title = '" + fullMovieName + "', choosescript = '" + choosescript + "', grabberfile = '" + wscript + "'");
      if (choosescript)
      {
        if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts)))
        {
          GUIUtils.ShowOKDialog(string.Format(GUILocalizeStrings.Get(1079876), MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts)), GUILocalizeStrings.Get(1079877), "", "");
          LogMyFilms.Info("My Films : The Directory grabber config files doesn't exists. Verify your Configuration !");
          return;
        }

        if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\user"))
        {
          try { Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\user"); }
          catch (Exception ex) { LogMyFilms.Debug("Error creating user script directory: '" + ex.Message + "'"); }
        }

        // Grabber Directory filled, search for XML scripts files
        var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(10798706)); // "Choose internet grabber script"
        if (dlg == null) return;
        var scriptfile = new ArrayList();

        if (MyFilms.conf.StrGrabber_cnf.Length > 0 && (grabtype == GrabType.Details || grabtype == GrabType.All))
        {
          try
          {
            var defaultScript = new GrabberScript(MyFilms.conf.StrGrabber_cnf);
            defaultScript.Load(MyFilms.conf.StrGrabber_cnf);
            scriptfile.Add(MyFilms.conf.StrGrabber_cnf);
            dlg.Add(MyFilms.conf.StrGrabber_cnf.Substring(MyFilms.conf.StrGrabber_cnf.LastIndexOf("\\", System.StringComparison.Ordinal) + 1) + " (default)");
            dlg.SelectedLabel = 0;
          }
          catch (Exception)
          {
            LogMyFilms.Error("The default script is not compatible with current MyFilms version - please change your settings !");
          }
        }
        var dirsInf = new DirectoryInfo(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts));
        FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();

        foreach (FileSystemInfo sfi in sfiles)
        {
          if ((sfi.Extension.ToLower() == ".xml") && (sfi.FullName != MyFilms.conf.StrGrabber_cnf))
          {
            var script = new GrabberScript(sfi.FullName);
            script.Load(sfi.FullName);
            string displayName = "";
            string displayNamePost = "";
            switch (grabtype)
            {
              case GrabType.MultiCovers:
                displayNamePost = script.Type.ToLower().Contains("multicovers") ? " - (Multi Cover)" : " - (Single Cover)";
                break;
            }
            if (ShouldGrabberBeAdded(script, grabtype, showAll))
            {
              if (!string.IsNullOrEmpty(script.DBName))
              {
                displayName += script.DBName;
                if (!string.IsNullOrEmpty(script.Language)) displayName += " (" + script.Language + ")";
                displayName += displayNamePost;
              }
              else displayName += sfi.Name;
              dlg.Add(displayName);
              scriptfile.Add(sfi.FullName);
            }
          }
        }
        // add choice to show all languages
        if (!showAll && !string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabberScriptsFilter))
          dlg.Add(GUILocalizeStrings.Get(10798765)); // Show all

        if (scriptfile.Count > 0)
        {
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
            return;
          if (dlg.SelectedLabelText == GUILocalizeStrings.Get(10798765))
          {
            grabb_Internet_Informations(fullMovieName, GetID, true, wscript, fullMoviePath, grabtype, true, sTitles, searchanimation);
            return;
          }
          if (dlg.SelectedLabel > -1)
            wscript = scriptfile[dlg.SelectedLabel].ToString();
        }
      }
      grabb_Internet_Informations_Search(fullMovieName, GetID, wscript, fullMoviePath, grabtype, sTitles, searchanimation);
    }

    //-------------------------------------------------------------------------------------------
    //  Grab URL Internet Movie Informations and update the XML database and refresh screen
    //  -> Search and select matching movie
    //-------------------------------------------------------------------------------------------        
    public static void grabb_Internet_Informations_Search(string fullMovieName, int GetID, string wscript, string fullMoviePath, GrabType grabtype, Searchtitles sTitles, GUIAnimation searchanimation)
    {
      if (string.IsNullOrEmpty(wscript)) return;
      LogMyFilms.Debug("grabb_Internet_Informations_Search() with title = '" + fullMovieName + "', grabberfile = '" + wscript + "'");
      string movieName = fullMovieName;
      string movieHierarchy = string.Empty;
      string moviePath = fullMoviePath;
      if (MyFilms.conf.TitleDelim.Length > 0)
      {
        movieName = fullMovieName.Substring(fullMovieName.LastIndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) + 1).Trim();
        movieHierarchy = fullMovieName.Substring(0, fullMovieName.LastIndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) + 1).Trim();
      }
      var grab = new Grabber.Grabber_URLClass();
      Grabber_URLClass.IMDBUrl wurl;
      var listUrl = new ArrayList();

      new Thread(delegate()
        {
          SetProcessAnimationStatus(true, searchanimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
          try
          {
            // listUrl = Grab.ReturnURL(MovieName, wscript, 1, !MyFilms.conf.StrGrabber_Always, MoviePath); // MoviePath only when nfo reader used !!!
            listUrl = grab.ReturnURL(movieName, wscript, 1, !MyFilms.conf.StrGrabber_Always, "");
          }
          catch (Exception ex)
          {
            LogMyFilms.Error(ex, "grabb_Internet_Details_Search() - exception = '" + ex.Message + "'");
          }
          SetProcessAnimationStatus(false, searchanimation); // GUIWaitCursor.Hide();

          int listCount = listUrl.Count;
          if (!MyFilms.conf.StrGrabber_Always)
            listCount = 2;
          switch (listCount)
          {
            case 1: // only one match -> grab details without user interaction
              wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[0];
              grabb_Internet_Details_Informations(wurl.URL, movieHierarchy, wscript, GetID, false, grabtype, sTitles, searchanimation);
              break;
            case 0:
              break;
            default:
              #region manual choice
              var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
              var choiceViewMenu = new List<string>();
              choiceViewMenu.Clear();

              if (dlg == null) return;
              dlg.Reset();
              dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
              dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
              choiceViewMenu.Add("manual selection");

              for (int i = 0; i < listUrl.Count; i++)
              {
                wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[i];
                if (wurl.Director.Contains(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString()) && wurl.Year.Contains(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString()) && !MyFilms.conf.StrGrabber_Always)
                {
                  if (dlg.SelectedLabel == -1)
                    dlg.SelectedLabel = i + 1;
                  else
                    dlg.SelectedLabel = -2;
                }
                string viewTitle = wurl.Title;
                if (!string.IsNullOrEmpty(wurl.Year)) viewTitle += " (" + wurl.Year + ")";
                if (!string.IsNullOrEmpty(wurl.Options)) viewTitle += " - " + wurl.Options + "";
                dlg.Add(viewTitle);
                choiceViewMenu.Add(wurl.Title);
              }
              string[] split = movieName.Trim().Split(new Char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
              foreach (string s in split.Where(s => s.Length > 2))
              {
                dlg.Add(GUILocalizeStrings.Get(137) + " '" + s + "'"); // add words from title as search items
                choiceViewMenu.Add(s);
              }
              if (grabtype != GrabType.Person)
              {
                #region add all titles as alternative search expressions
                if (!string.IsNullOrEmpty(sTitles.OriginalTitle) && !choiceViewMenu.Contains(sTitles.OriginalTitle))
                {
                  dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.OriginalTitle + "'"); //search
                  choiceViewMenu.Add(sTitles.OriginalTitle);
                }
                if (!string.IsNullOrEmpty(sTitles.TranslatedTitle) && !choiceViewMenu.Contains(sTitles.TranslatedTitle))
                {
                  dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.TranslatedTitle + "'");
                  choiceViewMenu.Add(sTitles.TranslatedTitle);
                }
                if (!string.IsNullOrEmpty(sTitles.FormattedTitle) && !choiceViewMenu.Contains(sTitles.FormattedTitle))
                {
                  dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.FormattedTitle + "'");
                  choiceViewMenu.Add(sTitles.FormattedTitle);
                }
                if (!string.IsNullOrEmpty(sTitles.MovieDirectoryTitle) && !choiceViewMenu.Contains(sTitles.MovieDirectoryTitle))
                {
                  dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.MovieDirectoryTitle + "'");
                  choiceViewMenu.Add(sTitles.MovieDirectoryTitle);
                }
                if (!string.IsNullOrEmpty(sTitles.MovieFileTitle) && !choiceViewMenu.Contains(sTitles.MovieFileTitle))
                {
                  dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.MovieFileTitle + "'");
                  choiceViewMenu.Add(sTitles.MovieFileTitle);
                }
                #endregion
              }

              if (!(dlg.SelectedLabel > -1))
              {
                dlg.SelectedLabel = -1;
                dlg.DoModal(GetID);
              }
              if (dlg.SelectedLabel == 0)
              {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard) return;
                keyboard.Reset();
                keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
                keyboard.Text = movieName;
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed && keyboard.Text.Length > 0)
                  grabb_Internet_Informations_Search(keyboard.Text, GetID, wscript, moviePath, grabtype, sTitles, searchanimation);
                break;
              }
              if (dlg.SelectedLabel > 0 && dlg.SelectedLabel <= listUrl.Count)
              {
                wurl = (Grabber_URLClass.IMDBUrl)listUrl[dlg.SelectedLabel - 1];
                grabb_Internet_Details_Informations(wurl.URL, movieHierarchy, wscript, GetID, true, grabtype, sTitles, searchanimation);
                break;
              }
              if (dlg.SelectedLabel > listUrl.Count)
              {
                //VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                //if (null == keyboard) return;
                //keyboard.Reset();
                //keyboard.Text = dlg.SelectedLabelText;
                //// keyboard.Text = choiceViewMenu[dlg.SelectedLabel];
                //keyboard.DoModal(GetID);
                //if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                //  grabb_Internet_Informations_Search(keyboard.Text, GetID, wscript, MoviePath, grabtype, sTitles);
                string strChoice = choiceViewMenu[dlg.SelectedLabel];
                LogMyFilms.Debug("grabb_Internet_Informations_Search(): (re)search with new search expression: '" + strChoice + "'");
                grabb_Internet_Informations_Search(strChoice, GetID, wscript, moviePath, grabtype, sTitles, searchanimation);
              }
              break;
              #endregion
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => { return 0; }, 0, 0, null);
        }) { Name = "MyFilmsDetailsLoader", IsBackground = true }.Start();
    }

    //-------------------------------------------------------------------------------------------
    //  Grab Internet Movie Details Informations and update the XML database and refresh screen
    //-------------------------------------------------------------------------------------------        
    public static void grabb_Internet_Details_Informations(string url, string moviehead, string wscript, int GetID, bool interactive, GrabType grabtype, Searchtitles sTitles, GUIAnimation searchanimation)
    {
      LogMyFilms.Debug("launching (grabb_Internet_Details_Informations) with url = '" + url + "', moviehead = '" + moviehead + "', wscript = '" + wscript + "', GetID = '" + GetID + "', interactive = '" + interactive + "'");

      #region set environment
      var Grab = new Grabber_URLClass();
      var Result = new string[80];
      string title = string.Empty;
      string ttitle = string.Empty;
      string wtitle = string.Empty;
      int year = 0;
      string director = string.Empty;
      // Those settings were used in the past from AMCupdater settings - now they exist in MF config as primary source!
      // XmlConfig XmlConfig = new XmlConfig();
      // string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
      // string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");

      string downLoadPath; //downLoadPath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsPath);
      if (interactive)
        downLoadPath = Path.GetTempPath();
      else
      {
        downLoadPath = grabtype == GrabType.Person
                         ? MyFilms.conf.StrPathArtist
                         : MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
      }
      LogMyFilms.Debug("Grabber - GetDetail: OverrideLanguage = '" + MyFilms.conf.GrabberOverrideLanguage + "', OverridePersonLimit = '" + MyFilms.conf.GrabberOverridePersonLimit + "', OverrideTitleLimit = '" + MyFilms.conf.GrabberOverrideTitleLimit + "', Get Roles = '" + MyFilms.conf.GrabberOverrideGetRoles + "'");
      LogMyFilms.Debug("Grabber - GetDetail: script = '" + wscript + "', url = '" + url + "', download path = '" + downLoadPath + "'");
      #endregion

      new Thread(delegate()
          {
            #region load internet data
            var dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
            if (interactive)
            {
              if (dlgPrgrs != null)
              {
                dlgPrgrs.Reset();
                dlgPrgrs.DisplayProgressBar = false;
                dlgPrgrs.ShowWaitCursor = true;
                dlgPrgrs.DisableCancel(true);
                dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Internet Updates"));
                dlgPrgrs.SetLine(1, "Loading Details ...");
                dlgPrgrs.Percentage = 0;
                dlgPrgrs.NeedRefresh();
                dlgPrgrs.ShouldRenderLayer();
                dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
              }

            }
            try
            {
              Result = Grab.GetDetail(url, downLoadPath, wscript, true, MyFilms.conf.GrabberOverrideLanguage, MyFilms.conf.GrabberOverridePersonLimit, MyFilms.conf.GrabberOverrideTitleLimit, MyFilms.conf.GrabberOverrideGetRoles, null);
            }
            catch (Exception ex) { LogMyFilms.Error(ex, "grabb_Internet_Details_Information() - exception = '" + ex.Message + "'"); }

            if (interactive)
            {
              // SetProcessAnimationStatus(false, searchanimation);
              if (dlgPrgrs != null)
              {
                dlgPrgrs.ShowWaitCursor = false;
                dlgPrgrs.Close();
              }
            }

            // copy mapped values to original values
            for (int i = 0; i < 40; i++)
            {
              LogMyFilms.Debug("Grabber Details: original: '" + i + "' - '" + Result[i] + "'");
              Result[i] = Result[i + 40];
              LogMyFilms.Debug("Grabber Details: mapped  : '" + i + "' - '" + Result[i] + "'");
            }
            LogMyFilms.Debug("Grab Internet Information done for title/ttitle: " + MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] + "/" + MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"]);

            // string Title_Group = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Folder_Name_Is_Group_Name", "false");
            // string Title_Group_Apply = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Group_Name_Applies_To", "");
            #endregion

            if (grabtype == GrabType.Details || grabtype == GrabType.All) // grabtype "all" includes cover
            {
              #region Movie Details

              string strChoice = "all"; // defaults to "all", if no other choice
              bool onlyselected = false;
              bool onlymissing = false;
              bool onlynonempty = false;
              var choiceViewMenu = new List<string>();
              var updateItems = new List<string>(); // store properties to update for later use ...
              const int iPropertyLengthLimit = 33;
              var PropertyList = new string[] {
                        "OriginalTitle", "TranslatedTitle", "Picture", "Description", "Rating", "Actors", "Director",
                        "Producer", "Year", "Country", "Category", "URL", "ImageURL", "Writer", "Comments", "Languages",
                        "TagLine", "Certification", "IMDB_Id", "IMDB_Rank", "Studio", "Edition", "Fanart", "Generic1",
                        "Generic2", "Generic3", "TranslatedTitleAllNames", "TranslatedTitleAllValues",
                        "CertificationAllNames", "CertificationAllValues", "MultiPosters", "Photos", "PersonImages",
                        "MultiFanart", "Trailer", "TMDB_Id", "Runtime", "Collection", "CollectionImageURL", "PictureURL"
                      };

              if (interactive) // Dialog only in interactive mode
              {
                #region interactive selection dialog
                var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                bool returnToMainDialog = false; // by default do NOT repeat the dialog - only if returning from sub dialogs
                do
                {
                  #region set main selection dialog for options
                  returnToMainDialog = false;
                  choiceViewMenu.Clear();
                  dlgmenu.Reset();
                  dlgmenu.SetHeading(GUILocalizeStrings.Get(10798797)); // Choose update option ...
                  dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798734) + " *** ");
                  choiceViewMenu.Add("all");
                  dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798735) + " *** ");
                  choiceViewMenu.Add("missing");
                  dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798730) + " *** ");
                  choiceViewMenu.Add("all-onlynewdata");
                  // disabled, as we now have the multiselect dialog
                  //dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798798) + " *** "); // Select single field for update ...
                  //choiceViewMenu.Add("singlefield");
                  if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsDialogMultiSelect.xml"))
                  {
                    dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798799) + " *** "); // Select multiple fields for update ...
                    choiceViewMenu.Add("multiplefields");
                  }
                  dlgmenu.DoModal(GetID);
                  if (dlgmenu.SelectedLabel == -1) return;
                  strChoice = choiceViewMenu[dlgmenu.SelectedLabel];

                  #endregion

                  if (strChoice == "singlefield")
                  {
                    #region populate select menu, if user has chosen to ...

                    string strOldValue = "";
                    string strNewValue = "";

                    var dlgSelect = (GUIDialogSelect)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT); // GUIDialogSelect2 dlgmenu = (GUIDialogSelect2)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT2);  
                    if (dlgSelect == null) return;
                    choiceViewMenu.Clear();
                    dlgSelect.Reset();
                    dlgSelect.SetHeading(GUILocalizeStrings.Get(10798732)); // choose property to update
                    dlgSelect.SetButtonLabel("");

                    int i = 0;
                    foreach (string wProperty in PropertyList)
                    {
                      try
                      {
                        strOldValue = MyFilms.r[MyFilms.conf.StrIndex][wProperty].ToString() ?? "";
                        strNewValue = Result[i];
                        if (i == 2) strNewValue = Result[12];
                        if (strNewValue == null) strNewValue = "";

                        if ( // make sure, only supported fields are offered to user for update
                          wProperty != "ImageURL" && !wProperty.Contains("Sub") && !wProperty.Contains("All") &&
                          !wProperty.Contains("Generic") && !wProperty.Contains("Empty") &&
                          ((wProperty != "TagLine" && wProperty != "Certification" && wProperty != "Writer" &&
                            wProperty != "Studio" && wProperty != "Edition" && wProperty != "IMDB_Id" &&
                            wProperty != "IMDB_Rank" && wProperty != "TMDB_Id") ||
                           MyFilms.conf.StrFileType == MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog4Xtended) &&
                          wProperty != "Fanart" && wProperty != "Aspectratio" && wProperty != "MultiPosters"
                          // set to enabled to get proper selection - WIP
                          && wProperty != "Photos" && wProperty != "PersonImages" && wProperty != "MultiFanart" &&
                          wProperty != "Trailer") //  && wProperty != "Runtime" && wProperty != "Collection"
                        {
                          dlgSelect.Add(BaseMesFilms.TranslateColumn(wProperty) + ": '" + Helper.LimitString(strOldValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) + "' -> '" + Helper.LimitString(strNewValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) + "'");
                          choiceViewMenu.Add(wProperty);
                          LogMyFilms.Debug("GrabberUpdate - Add (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                        }
                        else
                        {
                          LogMyFilms.Debug("GrabberUpdate - not added (unsupported) - (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                        }
                      }
                      catch (Exception ex)
                      {
                        LogMyFilms.Debug("GrabberUpdate - cannot add Property '" + wProperty + "' to Selectionmenu - reason: " + ex.Message);
                      }
                      i = i + 1;
                    }

                    dlgSelect.DoModal(GetID);
                    if (dlgSelect.SelectedLabel == -1) returnToMainDialog = true; // Nothing was selected - return to main selection menu
                    strChoice = choiceViewMenu[dlgmenu.SelectedLabel];

                    #endregion
                  }

                  if (choiceViewMenu[dlgmenu.SelectedLabel] == "multiplefields")
                  {
                    #region populate multi selection menu, if user has chosen to ...

                    string strOldValue = "";
                    string strNewValue = "";

                    var dlgMultiSelectOld = (GUIWindow)GUIWindowManager.GetWindow(2100);
                    var dlgMultiSelect = new GUIDialogMultiSelect();
                    if (dlgMultiSelect == null) return;
                    dlgMultiSelect.Init();
                    GUIWindowManager.Replace(2100, dlgMultiSelect);
                    try
                    {
                      dlgMultiSelect.Reset();
                      dlgMultiSelect.SetHeading(GUILocalizeStrings.Get(10798732)); // choose property to update

                      int i = 0;
                      foreach (string wProperty in PropertyList)
                      {
                        try
                        {
                          strOldValue = MyFilms.r[MyFilms.conf.StrIndex][wProperty].ToString() ?? "";
                          strNewValue = Result[i];
                          if (i == (int)Grabber_URLClass.Grabber_Output.PicturePathLong) strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathShort];
                          if (strNewValue == null) strNewValue = "";

                          if ( // make sure, only supported fields are offered to user for update
                            wProperty != "ImageURL" && !wProperty.Contains("Sub") && !wProperty.Contains("All") &&
                            !wProperty.Contains("Generic") && !wProperty.Contains("Empty") &&
                            ((wProperty != "TagLine" && wProperty != "Certification" && wProperty != "Writer" &&
                              wProperty != "Studio" && wProperty != "Edition" && wProperty != "IMDB_Id" &&
                              wProperty != "IMDB_Rank" && wProperty != "TMDB_Id") ||
                             MyFilms.conf.StrFileType == MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog4Xtended) &&
                            wProperty != "Fanart" && wProperty != "Aspectratio" && wProperty != "MultiPosters"
                            // set to enabled to get proper selection - WIP
                            && wProperty != "Photos" && wProperty != "PersonImages" && wProperty != "MultiFanart" &&
                            wProperty != "Trailer") //  && wProperty != "Collection" && wProperty != "CollectionImageURL"
                          {
                            var pItem = new GUIListItem(wProperty);
                            pItem.TVTag = wProperty;
                            if (i == (int)Grabber_URLClass.Grabber_Output.PicturePathLong) pItem.IconImage = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong];
                            pItem.Selected = false;
                            pItem.Label = BaseMesFilms.TranslateColumn(wProperty) + ": '" + Helper.LimitString(strOldValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) + "' -> '" + Helper.LimitString(strNewValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) + "'";
                            dlgMultiSelect.Add(pItem);
                            LogMyFilms.Debug("GrabberUpdate - Add (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                          }
                          else
                          {
                            LogMyFilms.Debug("GrabberUpdate - not added (unsupported) - (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                          }
                        }
                        catch (Exception ex)
                        {
                          LogMyFilms.Debug("GrabberUpdate - cannot add Property '" + wProperty + "' to Selectionmenu - reason: " + ex.Message);
                        }
                        i = i + 1;
                      }

                      dlgMultiSelect.DoModal(GetID);
                      updateItems.AddRange(from t in dlgMultiSelect.ListItems where t.Selected select t.TVTag.ToString());
                      LogMyFilms.Debug("GrabberUpdate - '" + updateItems.Count + "' updateItems selected !");
                      if (dlgMultiSelect.DialogModalResult == ModalResult.Cancel) return; // user cancelled
                      if (dlgMultiSelect.DialogModalResult == ModalResult.OK && updateItems.Count == 0) return; // Nothing was selected
                      if (dlgMultiSelect.DialogModalResult != ModalResult.OK) returnToMainDialog = true; // user wants to return to options menu
                    }
                    finally
                    {
                      GUIWindowManager.Replace(2100, dlgMultiSelectOld);
                    }

                    #endregion
                  }

                  switch (strChoice) // either an update type - or a single property to update
                  {
                    #region switch update options

                    case "all":
                      onlyselected = false;
                      onlymissing = false;
                      onlynonempty = false;
                      break;
                    case "missing":
                      onlyselected = false;
                      onlymissing = true;
                      onlynonempty = false;
                      break;
                    case "all-onlynewdata":
                      onlyselected = false;
                      onlymissing = false;
                      onlynonempty = true;
                      break;
                    default:
                      onlyselected = true;
                      onlymissing = false;
                      onlynonempty = false;
                      break;

                    #endregion
                  }
                }
                while (returnToMainDialog);
                LogMyFilms.Debug("GrabInternetDetails - interactive choice: '" + strChoice + "', onlyselected = '" + onlyselected + "', onlymissing = '" + onlymissing + "', onlynonempty = '" + onlynonempty + "'");
                #endregion
              }

              #region load details data

              // ********************************** now load data, if requested ! ******************************
              if (IsUpdateRequired("OriginalTitle", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
              {
                title = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
                if (wtitle.Contains(MyFilms.conf.TitleDelim)) wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                if (wtitle != title) Remove_Backdrops_Fanart(wtitle, true);

                // Add Collection from internet grabber
                if (!string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.Collection]) && string.IsNullOrEmpty(moviehead))
                {
                  moviehead = Result[(int)Grabber_URLClass.Grabber_Output.Collection] + @"\";
                }
                if (MyFilms.conf.StrTitle1 == "OriginalTitle") MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = moviehead + title;
                else MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = title;
              }

              if (IsUpdateRequired("TranslatedTitle", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.TranslatedTitle], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
              {
                ttitle = Result[(int)Grabber_URLClass.Grabber_Output.TranslatedTitle];
                if ("TranslatedTitle" == strChoice)
                {
                  if (string.IsNullOrEmpty(ttitle) && MyFilms.conf.StrTitle1 == "TranslatedTitle" && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString())) // Added to fill ttitle with otitle in case ttitle is empty and mastertitle = ttitle and mastertitle is empty
                    ttitle = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                }
                wtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                if (wtitle.Contains(MyFilms.conf.TitleDelim)) wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                if (wtitle != ttitle) Remove_Backdrops_Fanart(wtitle, true);
                if (MyFilms.conf.StrTitle1 == "TranslatedTitle")
                {
                  // Add Collection from internet grabber
                  if (!string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.Collection]) && string.IsNullOrEmpty(moviehead))
                  {
                    moviehead = Result[(int)Grabber_URLClass.Grabber_Output.Collection] + @"\";
                  }
                  MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = moviehead + ttitle;
                }
                else MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = ttitle;
              }

              //if (IsUpdateRequired("Collection", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Collection"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Collection], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
              //  MyFilms.r[MyFilms.conf.StrIndex]["Collection"] = Result[(int)Grabber_URLClass.Grabber_Output.Collection];

              if (IsUpdateRequired("Description", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Description], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[(int)Grabber_URLClass.Grabber_Output.Description];

              if (IsUpdateRequired("Rating", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Rating], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
              {
                if (Result[(int)Grabber_URLClass.Grabber_Output.Rating].Length > 0)
                {
                  var provider = new NumberFormatInfo();
                  provider.NumberDecimalSeparator = ".";
                  provider.NumberDecimalDigits = 1;
                  decimal wnote = Convert.ToDecimal(Result[(int)Grabber_URLClass.Grabber_Output.Rating], provider);
                  if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString()) || !onlymissing)
                    MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = string.Format("{0:F1}", wnote);
                }
              }
              if (IsUpdateRequired("Actors", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Actors], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[(int)Grabber_URLClass.Grabber_Output.Actors];
              if (IsUpdateRequired("Director", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Director], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
              {
                director = Result[(int)Grabber_URLClass.Grabber_Output.Director];
                MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[(int)Grabber_URLClass.Grabber_Output.Director];
              }
              if (IsUpdateRequired("Producer", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Producer"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Producer], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[(int)Grabber_URLClass.Grabber_Output.Producer];
              if (IsUpdateRequired("Year", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Year], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
              {
                try { year = Convert.ToInt16(Result[(int)Grabber_URLClass.Grabber_Output.Year]); }
                catch { year = 1900; }
                MyFilms.r[MyFilms.conf.StrIndex]["Year"] = year.ToString();
              }

              if (IsUpdateRequired("Country", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Country"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Country], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[(int)Grabber_URLClass.Grabber_Output.Country];
              if (IsUpdateRequired("Category", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Category"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Category], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[(int)Grabber_URLClass.Grabber_Output.Category];
              if (IsUpdateRequired("URL", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.URL], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                if (MyFilms.conf.StrStorage != "URL")
                  MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[(int)Grabber_URLClass.Grabber_Output.URL];
              if (IsUpdateRequired("Comments", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Comments"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Comments], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                MyFilms.r[MyFilms.conf.StrIndex]["Comments"] = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
              if (IsUpdateRequired("Languages", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Languages"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Language], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                MyFilms.r[MyFilms.conf.StrIndex]["Languages"] =
                  Result[(int)Grabber_URLClass.Grabber_Output.Language];

              #region AMC4 extended fields
              if (MyFilms.conf.StrFileType == MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog4Xtended)
              {
                if (IsUpdateRequired("Writer", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Writer"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Writer], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["Writer"] = Result[(int)Grabber_URLClass.Grabber_Output.Writer];
                if (IsUpdateRequired("TagLine", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TagLine"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Tagline], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["TagLine"] = Result[(int)Grabber_URLClass.Grabber_Output.Tagline];
                if (IsUpdateRequired("Certification", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Certification"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Certification], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["Certification"] = Result[(int)Grabber_URLClass.Grabber_Output.Certification];
                if (IsUpdateRequired("IMDB_Id", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Id], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"] = Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Id];
                if (IsUpdateRequired("IMDB_Rank", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Rank"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Rank], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Rank"] = Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Rank];
                if (IsUpdateRequired("TMDB_Id", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TMDB_Id"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.TMDB_Id], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["TMDB_Id"] = Result[(int)Grabber_URLClass.Grabber_Output.TMDB_Id];
                if (IsUpdateRequired("Studio", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Studio"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Studio], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["Studio"] = Result[(int)Grabber_URLClass.Grabber_Output.Studio];
                if (IsUpdateRequired("Edition", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Edition"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Edition], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  MyFilms.r[MyFilms.conf.StrIndex]["Edition"] = Result[(int)Grabber_URLClass.Grabber_Output.Edition];
                //if (IsUpdateRequired("Fanart", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Fanart"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Fanart], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                //  MyFilms.r[MyFilms.conf.StrIndex]["Fanart"] = Result[(int)Grabber_URLClass.Grabber_Output.Fanart];
                //if (IsUpdateRequired("Trailer", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Trailer"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Trailer], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                //  MyFilms.r[MyFilms.conf.StrIndex]["Trailer"] = Result[(int)Grabber_URLClass.Grabber_Output.Trailer];
              }
              #endregion

              if (grabtype == GrabType.All && IsUpdateRequired("Picture", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

              #endregion

              #endregion
            }

            if (grabtype == GrabType.Person)
            {
              #region Person

              #region Load existing person info
              IMDBActor person = null;
              var actorList = new ArrayList();
              VideoDatabase.GetActorByName(MyFilms.conf.StrTIndex, actorList);
              LogMyFilms.Debug("GrabberUpdate - found '" + actorList.Count + "' results for '" + MyFilms.conf.StrTIndex + "'");
              if (actorList.Count > 0 && actorList.Count < 5)
              {
                LogMyFilms.Debug("IMDB first search result: '" + actorList[0] + "'");
                string[] strActor = actorList[0].ToString().Split(new char[] { '|' });
                // int actorID = (strActor[0].Length > 0 && strActor.Count() > 1) ? Convert.ToInt32(strActor[0]) : 0; // string actorname = strActor[1];
                int actorId;
                int.TryParse(strActor[0], out actorId);
                person = actorId > 0 ? VideoDatabase.GetActorInfo(actorId) : new IMDBActor();
              }
              #endregion

              string strChoice = "all"; // defaults to "all", if no other choice
              var imageUrls = new Dictionary<string, string>();

              if (interactive) // Dialog only in interactive mode
              #region interactive selection dialog
              {
                var choiceViewMenu = new List<string>();
                var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                dlgmenu.Reset();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798797)); // Choose update option ...
                dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798734) + " *** ");
                choiceViewMenu.Add("all");
                dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798735) + " *** ");
                choiceViewMenu.Add("missing");
                dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798730) + " *** ");
                choiceViewMenu.Add("all-onlynewdata");

                //if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsDialogMultiSelect.xml"))
                //{
                //  dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798799) + " *** "); // Select multiple fields for update ...
                //  choiceViewMenu.Add("multiplefields");
                //}
                #region populate selection menu

                string[] PropertyList = new string[]
                      {
                        // "TMDB_Id", "IMDB_Id",
                        "OriginalTitle", // Name
                        "TranslatedTitle", // also known as (currently not supported)
                        "Picture", 
                        "Description", // biographie
                        "Comments", 
                        "Rating", // rating (if available)
                        "Actors", // filmographie
                        "Year", 
                        "Country", // birthplace
                        "URL", // URL to webpage
                        "Generic1", "Generic2", "Generic3", 
                        // "Photos", 
                        "PersonImages", // all images, if available
                        "PictureURL" // webUrl to main picture
                      };
                string personProperty;
                string strOldValue = "";
                string strNewValue = "";

                try
                {
                  personProperty = "name";
                  strOldValue = person != null && person.Name.Length > 0 ? person.Name : "";
                  strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                  dlgmenu.Add(GUILocalizeStrings.Get(10799301) + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                  choiceViewMenu.Add(personProperty);
                  LogMyFilms.Debug("GrabberUpdate - Add to menu (" + personProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");

                  personProperty = "dateofbirth";
                  strOldValue = person != null && person.DateOfBirth.Length > 0 ? person.DateOfBirth : "";
                  strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  dlgmenu.Add(GUILocalizeStrings.Get(10799302) + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                  choiceViewMenu.Add(personProperty);
                  LogMyFilms.Debug("GrabberUpdate - Add to menu (" + personProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");

                  personProperty = "placeofbirth";
                  strOldValue = person != null && person.PlaceOfBirth.Length > 0 ? person.PlaceOfBirth : "";
                  strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  dlgmenu.Add(GUILocalizeStrings.Get(10799303) + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                  choiceViewMenu.Add(personProperty);
                  LogMyFilms.Debug("GrabberUpdate - Add to menu (" + personProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");

                  personProperty = "biography";
                  strOldValue = person != null && person.Biography.Length > 0 ? person.Biography : "";
                  strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.Description];
                  dlgmenu.Add(GUILocalizeStrings.Get(10799304) + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                  choiceViewMenu.Add(personProperty);
                  LogMyFilms.Debug("GrabberUpdate - Add to menu (" + personProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
#if MP13
                  strOldValue = (person != null && person.DateOfDeath.Length > 0) ? person.DateOfDeath : "";
                  strOldValue = (person != null && person.PlaceOfDeath.Length > 0) ? person.PlaceOfDeath : "";
                  strOldValue = (person != null && person.LastUpdate.Length > 0) ? person.LastUpdate : "";
#endif
                  // main image
                  personProperty = "coverimage";
                  strOldValue = person != null && person.ThumbnailUrl.Length > 0 ? person.ThumbnailUrl : "";
                  strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.PictureURL];
                  dlgmenu.Add(GUILocalizeStrings.Get(10798682) + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                  choiceViewMenu.Add(personProperty);
                  imageUrls.Add(personProperty, strNewValue);
                  LogMyFilms.Debug("GrabberUpdate - Add to menu (" + personProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");

                  // add additional images, if returned by grabber
                  string multipersonimages = Result[(int)Grabber_URLClass.Grabber_Output.PersonImages];
                  LogMyFilms.Debug("GrabberUpdate - multipersonimages = '" + multipersonimages + "'");
                  int i = 1;
                  string[] personimagesUrls = multipersonimages.Split(new char[] { ',', '|' });
                  Regex reg = new Regex(@"\(((?!\)).)*\)");
                  foreach (string personimagesUrl in personimagesUrls)
                  {
                    personProperty = "coverimage" + i.ToString();
                    strNewValue = reg.Replace(personimagesUrl, "").Trim();
                    if (strNewValue != Result[(int)Grabber_URLClass.Grabber_Output.PictureURL]) // do not add main image twice!
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798682) + " " + i.ToString() + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                      choiceViewMenu.Add(personProperty);
                      imageUrls.Add(personProperty, strNewValue);
                      LogMyFilms.Debug("GrabberUpdate - Add to menu (" + personProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                      i++;
                    }
                  }
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("GrabberUpdate - Error adding Person property to Selectionmenu: " + ex.Message);
                }
                #endregion

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1) return;
                strChoice = choiceViewMenu[dlgmenu.SelectedLabel];
                LogMyFilms.Debug("GrabInternetDetails - interactive choice: '" + strChoice + "'");
              }
              #endregion

              #region load details data for person
              switch (strChoice)
              {
                case "name":
                  person.Name = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                  break;
                case "dateofbirth":
                  person.DateOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  break;
                case "placeofbirth":
                  person.PlaceOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  break;
                case "biography":
                  person.Biography = Result[(int)Grabber_URLClass.Grabber_Output.Description];
                  break;
                case "coverimage":
                  person.ThumbnailUrl = Result[(int)Grabber_URLClass.Grabber_Output.PictureURL];
                  // ToDo: Download Image
                  break;

                case "all":
                  person.Name = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                  person.DateOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  person.PlaceOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  person.Biography = Result[(int)Grabber_URLClass.Grabber_Output.Description];
                  person.ThumbnailUrl = Result[(int)Grabber_URLClass.Grabber_Output.PictureURL];
                  // ToDo: Download Image
                  break;

                case "missing":
                  if (string.IsNullOrEmpty(person.Name)) person.Name = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                  if (string.IsNullOrEmpty(person.DateOfBirth)) person.DateOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  if (string.IsNullOrEmpty(person.PlaceOfBirth)) person.PlaceOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  if (string.IsNullOrEmpty(person.Biography)) person.Biography = Result[(int)Grabber_URLClass.Grabber_Output.Description];
                  if (string.IsNullOrEmpty(person.ThumbnailUrl)) person.ThumbnailUrl = Result[(int)Grabber_URLClass.Grabber_Output.PictureURL];
                  // ToDo: Download Image
                  break;

                case "all-onlynewdata":
                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle])) person.Name = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.Comments])) person.DateOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.Country])) person.PlaceOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.Description])) person.Biography = Result[(int)Grabber_URLClass.Grabber_Output.Description];
                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.PictureURL]) && Result[(int)Grabber_URLClass.Grabber_Output.PictureURL].ToLower().StartsWith("http")) person.ThumbnailUrl = Result[(int)Grabber_URLClass.Grabber_Output.PictureURL];
                  // ToDo: Download Image
                  break;

                default:
                  if (strChoice.StartsWith("coverimage"))
                  {
                    person.ThumbnailUrl = imageUrls[strChoice];
                    LogMyFilms.Debug("GrabInternetDetails - set person URL to: " + person.ThumbnailUrl);
                  }
                  break;
              }

              LogMyFilms.Debug("GrabInternetDetails - downloadimage: '" + Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong]);

              #region Add or update actor to video database
              try
              {
                //#if MP1X
                //                  int actorId = VideoDatabase.AddActor(person.Name);
                //#else
                //                  int actorId = VideoDatabase.AddActor(null, person.Name);
                //#endif
                int actorId = GUIUtils.AddActor(null, person.Name);
                if (actorId > 0)
                {
                  if (!string.IsNullOrEmpty(person.Biography)) // clean up before saving ...
                  {
                    if (person.Biography.StartsWith("From Wikipedia, the free encyclopedia")) person.Biography = person.Biography.Replace("From Wikipedia, the free encyclopedia", "").TrimStart(new char[] { '.' }).Trim(new char[] { ' ', '\r', '\n' });
                  }
                  VideoDatabase.SetActorInfo(actorId, person);
                  //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
                }
              }
              catch (Exception ex) { LogMyFilms.Debug("Error adding person to VDB: " + ex.Message, ex.StackTrace); }
              #endregion

              #region load missing images ...
              if (!string.IsNullOrEmpty(MyFilms.conf.StrPathArtist) && !string.IsNullOrEmpty(person.ThumbnailUrl))
              {
                string filename = MyFilms.conf.StrPathArtist + "\\" + person.Name + ".jpg";  // string filename = Path.Combine(MyFilms.conf.StrPathArtist, personname); //File.Exists(MyFilms.conf.StrPathArtist + "\\" + personsname + ".jpg")))
                if (person.ThumbnailUrl.Contains("http:") && (strChoice == "all" || strChoice == "all-onlynewdata" || (!File.Exists(filename) && strChoice == "missing") || strChoice.StartsWith("coverimage")))
                {
                  #region MP Thumb download deactivated, as downloading not yet working !!!
                  //if (person.ThumbnailUrl != string.Empty) // to update MP person thumb dir
                  //{
                  //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, person.Name);
                  //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, person.Name);
                  //  Utils.FileDelete(largeCoverArt);
                  //  Utils.FileDelete(coverArt);
                  //  IMDBFetcher.DownloadCoverArt(Thumbs.MovieActors, person.ThumbnailUrl, person.Name);
                  //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
                  //}
                  #endregion
                  string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, person.ThumbnailUrl, person.Name, false, true, out filename);
                }
              }
              #endregion
              // grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

              #endregion

              #endregion
            }

            if (grabtype == GrabType.Cover)
            {
              #region Cover

              if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong]))
              {
                if (interactive)
                {
                  GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798625));  // no results found
                  // grabb_Internet_Informations(FullMovieName, GetID, true, wscript, FullMoviePath, grabtype, showAll);
                }
                return;
              }
              grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

              #endregion
            }

            if (grabtype == GrabType.MultiCovers)
            {
              #region MultiCovers

              if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.MultiPosters]))
              {
                // no data found
                if (interactive)
                {
                  GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798625));  // no results found
                  // grabb_Internet_Informations(FullMovieName, GetID, true, wscript, FullMoviePath, grabtype, showAll);
                }
                return;
              }
              grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

              #endregion
            }

            Update_XML_database();
            LogMyFilms.Info("Database Updated for title/ttitle: " + title + "/" + ttitle);

            if (GetID != MyFilms.ID_MyFilmsCoverManager)
              if (title.Length > 0 && MyFilms.conf.StrFanart && grabtype != GrabType.Person) // Get Fanart
              {
                #region fanart

                // GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS); // already defined above !
                if (dlgPrgrs != null)
                {
                  dlgPrgrs.Reset();
                  dlgPrgrs.DisplayProgressBar = false;
                  dlgPrgrs.ShowWaitCursor = true;
                  dlgPrgrs.DisableCancel(true);
                  dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Artwork Updater"));
                  dlgPrgrs.SetLine(1, "Loading Artwork ...");
                  dlgPrgrs.Percentage = 0;
                  dlgPrgrs.NeedRefresh();
                  dlgPrgrs.ShouldRenderLayer();
                  dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);

                  new Thread(delegate()
                      {
                        string imdbid = GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
                        GrabArtwork(title, ttitle, year, director, imdbid, MyFilms.conf.StrTitle1, dlgPrgrs);
                        //  dlgPrgrs.Percentage = 100;
                        //  dlgPrgrs.SetLine(1, "Finished loading Movie Details ...");
                        //  dlgPrgrs.NeedRefresh();
                        //  dlgPrgrs.ShouldRenderLayer();
                        //  Thread.Sleep(500);
                        //  dlgPrgrs.ShowWaitCursor = false;
                        //  dlgPrgrs.Close();

                        //GrabArtwork(title, ttitle, (int)year, director, MyFilms.conf.StrTitle1.ToString(), r => 
                        //{
                        //  dlgPrgrs.Percentage = r;
                        //  return dlgPrgrs.ShouldRenderLayer();
                        //});
                        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                            { return 0; }, 0, 0, null);
                      }) { Name = "MyFilmsArtworkLoader", IsBackground = true }.Start();
                  return;
                }
                // System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1.ToString());

                #endregion
              }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                {
                  if (GetID != MyFilms.ID_MyFilmsCoverManager)
                    if (DetailsUpdated != null) DetailsUpdated(true); // will launch screen update with new data, if handler registered
                  return 0;
                }, 0, 0, null);
          }) { Name = "MyFilmsDetailsLoader", IsBackground = true }.Start();
    }

    private static void grabb_Internet_Details_Informations_Cover(string[] Result, bool interactive, int GetID, string wscript, GrabType grabtype, Searchtitles sTitles)
    {
      string tmpPicture = "";
      string tmpPicturename = ""; // picturename only
      string newPicture = ""; // full path to new picture
      string newPictureCatalogname = ""; // entry to be stored in catalog
      string oldPicture = getGUIProperty("picture"); // "save" current picture for later restore...
      string oldPictureCatalogname = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();

      // set defaults...
      switch (grabtype)
      {
        case GrabType.Cover:
        case GrabType.All:
          tmpPicture = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong];
          tmpPicturename = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong].Substring(Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong].LastIndexOf("\\") + 1);
          break;
        case GrabType.MultiCovers:
          // make difference between existing cover and new one
          //if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()))
          //{
          //  string tmp = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();
          //  if (tmp.Contains("\\"))
          //  {
          //    tmp = tmp.Substring(tmp.LastIndexOf("\\") + 1);
          //    if (tmp.Contains("["))
          //      tmpPicturename = tmp.Substring(0, tmp.LastIndexOf("[") - 1);
          //    else tmpPicturename = tmp;
          //  }
          //  else tmpPicturename = tmp;
          //}

          //if (title.EndsWith(".jpg"))
          //  title = title.Substring(0, title.Length - 4);
          //string safeName = CreateFilename(title);
          //string dirname = artFolder;
          //// string directory = "";
          ////if (dirname.Length > dirname.LastIndexOf("\\")) 
          ////  directory = dirname.Substring(dirname.LastIndexOf("\\"));
          ////if (!System.IO.Directory.Exists(directory))
          ////  System.IO.Directory.CreateDirectory(directory);
          //if (first && !System.IO.File.Exists(dirname + safeName + ".jpg"))
          //  filename = dirname + safeName + ".jpg";
          //else
          //  filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";
          //FileInfo newFile = new FileInfo(filename);
          //bool alreadyInFolder = newFile.Exists;

          if (string.IsNullOrEmpty(tmpPicturename))
          {
            tmpPicturename = sTitles.FanartTitle + ".jpg";
          }
          break;
      }

      if (MyFilms.conf.StrPicturePrefix.Length > 0)
        newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
      else
        newPicture = Path.Combine(MyFilms.conf.StrPathImg, tmpPicturename);

      newPictureCatalogname = GetPictureCatalogNameFromFilename(newPicture);

      LogMyFilms.Debug("Cover Image path : '" + MyFilms.conf.StrPathImg + "'");
      LogMyFilms.Debug("Picturehandling  : '" + MyFilms.conf.PictureHandling + "'");
      LogMyFilms.Debug("PicturePrefix    : '" + MyFilms.conf.StrPicturePrefix + "'");
      LogMyFilms.Debug("Temp Cover Image : '" + tmpPicture + "'");
      LogMyFilms.Debug("New  Cover Image : '" + newPicture + "'");
      LogMyFilms.Debug("New Catalog Entry: '" + newPictureCatalogname + "'");


      switch (grabtype)
      {
        case GrabType.MultiCovers:
          #region multiple cover images
          var script = new GrabberScript(wscript);
          var ArtworkImages = new ArtworkInfo(Result[(int)Grabber_URLClass.Grabber_Output.MultiPosters], script.URLPrefix);
          var testlist = new List<ArtworkInfoItem>();
          testlist = Grabber.GrabUtil.GetMultiImageList(script.URLPrefix, Result[(int)Grabber_URLClass.Grabber_Output.MultiPosters], "");

          var choiceViewMenu = new List<string>();
          var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          dlgmenu.Reset();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798764)); // Load Covers ...
          dlgmenu.Add(GUILocalizeStrings.Get(10798622) + " (" + testlist.Count.ToString() + ")"); //all
          choiceViewMenu.Add("all");

          int i = 0;
          foreach (Grabber.ArtworkInfoItem artworkImage in testlist)
          {
            try
            {
              dlgmenu.Add(artworkImage.Name);
              choiceViewMenu.Add(artworkImage.URL);
              LogMyFilms.Debug("Coverdownload - Add to menu (" + i + ": " + artworkImage.Name + "): '" + artworkImage.URL + "'");
            }
            catch { }
            i = i + 1;
          }

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1) return;
          string strChoice = choiceViewMenu[dlgmenu.SelectedLabel];
          LogMyFilms.Debug("GrabInternetDetails - interactive choice: '" + strChoice + "'");

          //GrabberScript script = new GrabberScript(wscript);
          var dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
          if (dlgPrgrs != null)
          {
            dlgPrgrs.Reset();
            dlgPrgrs.DisplayProgressBar = true;
            dlgPrgrs.ShowWaitCursor = false;
            dlgPrgrs.DisableCancel(true);
            dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Internet Details Grabber"));
            dlgPrgrs.SetLine(1, "Loading Cover(s) [" + script.DBName + "] ...");
            dlgPrgrs.Percentage = 0;
            dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);

            new System.Threading.Thread(delegate()
            {
              try
              {
                string filename = string.Empty;
                string filename1 = string.Empty;
                string filename2 = string.Empty;
                //if (MasterTitle == "OriginalTitle")
                //  wtitle2 = wtitle1;
                bool first = true;
                int a = 0;

                if (strChoice != "all")
                {
                  filename1 = Grabber.GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, strChoice, tmpPicturename, true, first, out filename);
                }
                else
                {
                  foreach (Grabber.ArtworkInfoItem artworkImage in testlist)
                  {
                    filename1 = GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, artworkImage.URL, tmpPicturename, true, first, out filename);
                    if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + System.IO.Path.GetFileName(filename) + "'");
                    if (dlgPrgrs != null) dlgPrgrs.Percentage = a * 100 / testlist.Count;
                    LogMyFilms.Info("Poster " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + newPictureCatalogname);
                    artworkImage.LocalPath = filename;
                    if (first)
                      newPicture = filename1;
                    if (filename == string.Empty)
                      filename = filename1;
                    if (!(filename == "already" && filename1 == "already"))
                      filename = "added";
                    first = false;
                    a++;
                  }
                }
              }
              catch (Exception ex) { LogMyFilms.Debug(ex, "Thread 'MyFilmsCoverLoader' - exception!"); }
              if (dlgPrgrs != null)
                dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); dlgPrgrs.SetLine(2, ""); Thread.Sleep(50); dlgPrgrs.Close(); // Done...
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
              {
                dlgPrgrs.ShowWaitCursor = false;
                // enter here what to load after background thread has finished !
                if (GetID == MyFilms.ID_MyFilmsCoverManager)
                {
                  if (DetailsUpdated != null) DetailsUpdated(true);
                }
                return 0;
              }, 0, 0, null);
            }) { Name = "MyFilmsCoverLoader", IsBackground = true }.Start();
            return;
          }
          #endregion
          break;
        case GrabType.Cover:
          #region single cover without confirmation dialog
          if (File.Exists(newPicture)) // if 'base file' already exists, create sub file name Result[(int)Grabber_URLClass.Grabber_Output.Country]
          {
            newPicture = Path.Combine(Path.GetDirectoryName(newPicture), Path.GetFileNameWithoutExtension(newPicture) + " [" + Result[(int)Grabber.Grabber_URLClass.Grabber_Output.PictureURL].GetHashCode() + "]" + System.IO.Path.GetExtension(newPicture));
            LogMyFilms.Debug("New Cover File Name = '" + newPicture + "'");
          }
          if (newPicture != tmpPicture)
          {
            if (!Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
            {
              try { Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\"))); }
              catch (Exception ex) { LogMyFilms.Debug("Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) + "' - Exception: " + ex); }
            }
            try
            {
              LogMyFilms.Debug("Copy '" + tmpPicture + "' to '" + newPicture + "'");
              File.Copy(tmpPicture, newPicture, true);
            }
            catch (Exception ex) { LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex); }
          }

          if (newPicture != tmpPicture)
          {
            try { File.Delete(tmpPicture); }
            catch (Exception ex) { LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex); }
          }
          #endregion
          break;
        case GrabType.All:
          #region single cover
          if (interactive)
          {
            setGUIProperty("picture", tmpPicture);
            GUIWindowManager.Process(); // To Update GUI display ...

            var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079870)); // choice
            dlgYesNo.SetLine(1, "");
            dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798733)); // Replace cover with new one
            dlgYesNo.SetLine(3, "");
            dlgYesNo.DoModal(GetID);
            if (!(dlgYesNo.IsConfirmed))
            {
              setGUIProperty("picture", oldPicture);
              GUIWindowManager.Process();
              return;
            }
            else
            {
              setGUIProperty("picture", "");
              GUIWindowManager.Process();
            }
          }
          if (newPicture != tmpPicture)
          {
            if (!Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
            {
              try { Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\"))); }
              catch (Exception ex) { LogMyFilms.Debug("Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) + "' - Exception: " + ex); }
            }
            try { File.Copy(tmpPicture, newPicture, true); }
            catch (Exception ex) { LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex); }
          }

          if (newPicture != tmpPicture)
          {
            try { File.Delete(tmpPicture); }
            catch (Exception ex) { LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex); }
          }
          #endregion
          #region collection image

          string tmpPictureCollection = Path.GetDirectoryName(tmpPicture) + @"\Collection_" + Path.GetFileName(tmpPicture);
          string newPictureCollection = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrTitleSelect.Replace(MyFilms.conf.TitleDelim, ".") + "." + sTitles.MasterTitle + ".jpg";
          if (System.IO.File.Exists(tmpPictureCollection))
          {
            try
            {
              File.Copy(tmpPictureCollection, newPictureCollection, true);
              LogMyFilms.Debug("Created Collection image '" + newPictureCollection + "'");
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("Error copy file: '" + tmpPictureCollection + "' - Exception: " + ex);
            }
          }
          else
          {
            LogMyFilms.Debug("Collection Cover '" + tmpPictureCollection + "' does not exists - do nothing.");
          }
          if (newPictureCollection != tmpPictureCollection)
          {
            try { File.Delete(tmpPictureCollection); }
            catch (Exception ex) { LogMyFilms.Debug("Error deleting tmp file: '" + tmpPictureCollection + "' - Exception: " + ex); }
          }
          #endregion
          break;
      }
      // update catalog entry in memory
      if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()))
        MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
      else if (interactive)
        MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
      else if (!oldPicture.Contains(oldPictureCatalogname))
        MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
      // set picture to new one (full path)
      setGUIProperty("picture", newPicture);
      GUIWindowManager.Process();
      if (GetID == MyFilms.ID_MyFilmsCoverManager)
      {
        if (DetailsUpdated != null) DetailsUpdated(true);
      }
    }

    private static bool IsUpdateRequired(string currentField, string selectedField, string oldvalue, string newvalue, GrabType grabtype, bool onlyselected, bool onlymissing, bool onlynonempty, List<string> updateitems)
    {
      bool updaterequired = false;
      if (currentField == selectedField || !onlyselected || updateitems.Contains(currentField))
      {
        if (onlymissing && string.IsNullOrEmpty(oldvalue) || !onlymissing)
        {
          if (onlynonempty && !string.IsNullOrEmpty(newvalue) || !onlynonempty)
          {
            if (!string.IsNullOrEmpty(newvalue)) updaterequired = true;
          }
        }
      }
      LogMyFilms.Debug("IsUpdateRequired(): return '" + updaterequired + "' for: currentField = '" + currentField + "', selectedField = '" + selectedField + "', onlyselected = '" + onlyselected + "', onlymissing = '" + onlymissing + "', onlynonempty = '" + onlynonempty + "'");
      return updaterequired;
    }

    private static void GrabArtwork(string title, string ttitle, int year, string director, string imdbid, string StrTitle1, GUIDialogProgress dlgPrgrs)
    {
      try
      {
        if (dlgPrgrs != null)
          dlgPrgrs.SetLine(1, "Now loading Fanart ...");

        if (dlgPrgrs != null && !dlgPrgrs.ShouldRenderLayer())
          return;

        if (dlgPrgrs != null)
          dlgPrgrs.Percentage = 10;

        var grab = new Grabber_URLClass();
        List<DbMovieInfo> listemovies = grab.GetFanart(title, ttitle, year, director, imdbid, MyFilms.conf.StrPathFanart, true, false, StrTitle1, string.Empty);
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("GrabArtwork() - Error getting fanart: " + ex.Message);
      }
      finally
      {
        if (dlgPrgrs != null)
        {
          dlgPrgrs.Percentage = 100;
          dlgPrgrs.SetLine(1, "Finished loading Fanart !");
          dlgPrgrs.ShowWaitCursor = false;
          Thread.Sleep(500);
          dlgPrgrs.Close();
        }
      }
    }

    public static ArrayList FindActor(string personname, string wscript)
    {
      var listUrl = new ArrayList();
      if (string.IsNullOrEmpty(wscript)) return listUrl;
      LogMyFilms.Debug("FindActor() with name = '" + personname + "', grabberfile = '" + wscript + "'");
      var grab = new Grabber_URLClass();
      try
      {
        listUrl = grab.ReturnURL(personname, wscript, 1, false, "");
      }
      catch (Exception ex)
      {
        LogMyFilms.Error(ex, "FindActor() - exception = '" + ex.Message + "'");
      }
      return listUrl;
    }

    public static bool GrabActorDetails(string url, string wscript, out IMDBActor person)
    {
      LogMyFilms.Debug("launching (GrabActorDetails) with url = '" + url + "', wscript = '" + wscript + "'");

      person = new IMDBActor();
      var Grab = new Grabber_URLClass();
      var Result = new string[80];

      #region load internet data
      try
      {
        Result = Grab.GetDetail(url, "", wscript, false, null, null, null, null);
        // Result = Grab.GetDetail(url, downLoadPath, wscript, true, MyFilms.conf.GrabberOverrideLanguage, MyFilms.conf.GrabberOverridePersonLimit, MyFilms.conf.GrabberOverrideTitleLimit, MyFilms.conf.GrabberOverrideGetRoles, null);
        for (int i = 0; i < 40; i++) // copy mapped values to original values
        {
          Result[i] = Result[i + 40];
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Error(ex, "GrabActorDetails() - exception = '" + ex.Message + "'");
        return false;
      }
      #endregion

      #region load details data for person
      person.Name = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
      person.DateOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
      person.PlaceOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Country];
      person.Biography = Result[(int)Grabber_URLClass.Grabber_Output.Description];
      person.ThumbnailUrl = Result[(int)Grabber_URLClass.Grabber_Output.PictureURL];
      #endregion
      LogMyFilms.Debug("GrabActorDetails() done for person : '" + person.Name + "'");
      return true;
    }


    //-------------------------------------------------------------------------------------------
    //  Create Thumb via MTN (MovieThumbNailer) from movie itself
    //-------------------------------------------------------------------------------------------        
    public static void CreateThumbFromMovie()
    {
      //XmlConfig XmlConfig = new XmlConfig();
      //string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
      //string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");


      string fileName = MyFilms.r[MyFilms.conf.StrIndex]["Source"].ToString();
      if (string.IsNullOrEmpty(fileName))
      {
        LogMyFilms.Debug("(CreateThumbFromMovie): Error - Moviefilesource is empty !");
        return;
      }
      if (fileName.Contains("VIDEO_TS\\VIDEO_TS.IFO")) // Do not try to create thumbnails for DVDs
      {
        LogMyFilms.Debug("(CreateThumbFromMovie): Moviesource is DVD - return without creating coverfile...");
        return;
      }
      if (fileName.Contains(";"))
        fileName = fileName.Substring(0, fileName.IndexOf(";")).Trim();
      if (!File.Exists(fileName))
      {
        LogMyFilms.Debug("(CreateThumbFromMovie): Error - Moviefilesource: '" + fileName + "' does not exist !");
        return;
      }
      string tempImage = System.IO.Path.GetTempPath() + "MovieThumb_" + MyFilms.r[MyFilms.conf.StrIndex]["Number"] + ".jpg";
      if (File.Exists(tempImage)) File.Delete(tempImage);
      //if (System.IO.File.Exists(tempImage + "L")) System.IO.File.Delete(tempImage + "L");
      string strThumb = MyFilms.conf.StrPathImg + "\\MovieThumb_" + MyFilms.r[MyFilms.conf.StrIndex]["Number"] + ".jpg";
      LogMyFilms.Debug("(CreateThumbFromMovie): Moviefilesource: '" + fileName + "', Covernamedestination: '" + strThumb + "', TempImage: '" + tempImage + "'");

      //if (Img_Path_Type.ToLower() == "true")
      //    MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;

      //if (File.Exists(strThumb))
      //{
      //  LogMyFilms.Debug("(CreateThumbFromMovie): Coverimagefile already exists - return without creating coverfile...");
      //  return ;
      //}

      //MediaPortal.Services.IVideoThumbBlacklist blacklist = MediaPortal.Services.GlobalServiceProvider.Get<MediaPortal.Services.IVideoThumbBlacklist>();
      //if (blacklist != null && blacklist.Contains(fileName))
      //{
      //  LogMyFilms.Debug("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", fileName);
      //    return ;
      //}

      string ar = GetAspectRatio(fileName);
      int columns = 4;
      int rows = 2;
      if (string.IsNullOrEmpty(ar))
        LogMyFilms.Debug("Failed getting aspectratio of movie");
      else
      {
        LogMyFilms.Debug("GetAspectratio: ar = '" + ar + "'");
      }
      //"fullscreen" : "widescreen"
      if (ar == "fullscreen")
      {
        columns = 2;
        rows = 4;
      }
      if (ar == "widescreen")
      {
        columns = 2;
        rows = 5;
      }
      LogMyFilms.Debug("GetAspectratio: ar = '" + ar + "', columns = " + columns.ToString() + ", Rows = " + rows + ".");

      //System.Drawing.Image thumb = null;))))
      try
      {
        // CreateVideoThumb(string aVideoPath, string aThumbPath, bool aCacheThumb, bool aOmitCredits);
        bool success = ThumbCreator.CreateVideoThumb(fileName, tempImage, true, false, columns, rows, false, "Cover");
        if (!success)
        {
          LogMyFilms.Debug("(CreateThumbFromMovie): 'CreateVideoThumb' was NOT successful!");
          return;
        }
        else
        {
          LogMyFilms.Debug("(CreateThumbFromMovie): 'CreateVideoThumb' was successful!");
          //return;
        }
      }
      catch (System.Runtime.InteropServices.COMException comex)
      {
        if (comex.ErrorCode == unchecked((int)0x8004B200))
        {
          LogMyFilms.Warn("Could not create thumbnail for {0} [Unknown error 0x8004B200]", fileName);
        }
        else
        {
          LogMyFilms.Error("Could not create thumbnail for {0}", fileName);
          LogMyFilms.Error(comex);
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Could not create thumbnail for {0}", fileName);
        LogMyFilms.Error(ex);
      }
      finally
      {
        //if (!File.Exists(strThumb) && blacklist != null)
        //{
        //  blacklist.Add(fileName);
        //}
      }

      string cleanedtitle = GetSearchString(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
      LogMyFilms.Debug("Clean title '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1] + "' to '" + cleanedtitle + "'");

      try
      {
        MyFilmsPlugin.Utils.ImageFast.CreateImage(
          tempImage.Substring(0, tempImage.LastIndexOf(".")) + "title.jpg", cleanedtitle, tempImage.Substring(0, tempImage.LastIndexOf(".")) + "L.jpg");
      }
      catch (Exception)
      {

        throw;
      }

      string newPicture = tempImage.Substring(0, tempImage.LastIndexOf(".")) + "L.jpg";
      string oldPicture = getGUIProperty("picture");
      if (oldPicture.Length == 0 || oldPicture == null)
        oldPicture = newPicture;
      LogMyFilms.Debug("Picture Grabber options: Old temp Cover Image: '" + oldPicture + "'");
      LogMyFilms.Debug("Picture Grabber options: New temp Cover Image: '" + newPicture + "'");
      setGUIProperty("picture", newPicture);
      GUIWindowManager.Process(); // To Update GUI display ...

      var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079870)); // choice
      dlgYesNo.SetLine(1, "");
      dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798733)); // Replace cover with new one
      dlgYesNo.SetLine(3, "");
      dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
      //dlgYesNo.DoModal(GetID);
      if (!(dlgYesNo.IsConfirmed))
      {
        setGUIProperty("picture", oldPicture);
        GUIWindowManager.Process();
        return;
      }
      try
      {
        string newFinalPicture = strThumb;
        File.Copy(newPicture, newFinalPicture, true);
        File.Delete(newPicture);
        //MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newFinalPicture; // will be done below ...
        //setGUIProperty("picture", newFinalPicture);
        //GUIWindowManager.Process();
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Error copy file: '" + newPicture + "' - Exception: " + ex);
      }
      MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;
      Update_XML_database();
      LogMyFilms.Info("(Update_XML_database()) - Database Updated for created PictureThumb: " + strThumb);
    }

    private static string GetAspectRatio(string FileName)
    {
      try
      {
        var minfo = new MediaInfoWrapper(FileName);

        LogMyFilms.Debug("Mediainfo width: " + minfo.Width);
        LogMyFilms.Debug("Mediainfo height: " + minfo.Height);
        //ToDo: Calculate aspect ratio here...

        return minfo.AspectRatio;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("GetAspectRatio: Error getting aspectratio via mediainfo.dll - Exception: " + ex.ToString());
      }
      return "";
    }

    //-------------------------------------------------------------------------------------------
    //  Returns existing or newly created filename for coverart
    //-------------------------------------------------------------------------------------------        
    public static string GetOrCreateCoverFilename(DataRow[] r1, int index, string mastertitle)
    {
      string file = string.Empty;
      if (r1[index]["Picture"].ToString().Length > 0)
      {
        if ((r1[index]["Picture"].ToString().IndexOf(":\\") == -1) && (r1[index]["Picture"].ToString().Substring(0, 2) != "\\\\"))
          file = MyFilms.conf.StrPathImg + "\\" + r1[index]["Picture"];
        else
          file = r1[index]["Picture"].ToString();
      }
      //if (!System.IO.File.Exists(file) || string.IsNullOrEmpty(file))
      //  file = MyFilms.conf.DefaultCover;
      if (string.IsNullOrEmpty(file))
      {
        // create new name, as none is existing yet ...
        string title = (mastertitle.EndsWith(".jpg")) ? mastertitle.Substring(0, mastertitle.Length - 4) : mastertitle;
        title = GrabUtil.CreateFilename(title) + ".jpg";
        //if (first && !System.IO.File.Exists(dirname + safeName + ".jpg"))
        //  filename = dirname + safeName + ".jpg";
        //else
        //  filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";


        if (MyFilms.conf.StrPicturePrefix.Length > 0)
          file = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + title;
        else
          file = MyFilms.conf.StrPathImg + "\\" + title;

        if (!Directory.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix))
          try
          {
            Directory.CreateDirectory(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("GetOrCreateCoverFilename() - Error with Exception  = '" + ex.Message + "'");
          }
      }
      LogMyFilms.Debug("GetOrCreateCoverFilename() - PictureFilename = '" + file + "'");
      return file;
    }

    //-------------------------------------------------------------------------------------------
    //  Returns filename to be stored in catalog, depending on settings
    //-------------------------------------------------------------------------------------------        
    public static string GetPictureCatalogNameFromFilename(string picturefilename)
    {
      string newPictureCatalogname = ""; // entry to be stored in catalog
      string tmpPicturename = picturefilename.Substring(picturefilename.LastIndexOf("\\") + 1);
      if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
      {
        newPictureCatalogname = MyFilms.conf.StrPicturePrefix.Substring(0, MyFilms.conf.StrPicturePrefix.LastIndexOf("\\") + 1) + tmpPicturename;
      }
      if (MyFilms.conf.PictureHandling == "Full Path")
      {
        newPictureCatalogname = picturefilename;
      }
      LogMyFilms.Debug("GetPictureCatalogNameFromFilename() - PictureCatalogName = '" + newPictureCatalogname + "' for Picture '" + picturefilename + "'");
      return newPictureCatalogname;
    }

    //-------------------------------------------------------------------------------------------
    //  Dowload TMDBinfos (Poster(s), Movieinfos) on theMovieDB.org
    //-------------------------------------------------------------------------------------------        
    public static void Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose, int wGetID, string savetitle, GUIAnimation searchanimation)
    {
      string oldPicture = getGUIProperty("picture");
      string newPicture = ""; // full path to new picture
      string newPictureCatalogname = ""; // entry to be stored in catalog
      if (string.IsNullOrEmpty(oldPicture)) 
        oldPicture = "";

      if (!Directory.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix))
        try
        {
          Directory.CreateDirectory(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix);
        }
        catch (Exception) { }
      var grab = new Grabber_URLClass();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      int wyear = 0;
      try { wyear = Convert.ToInt32(year); }
      catch { }
      new Thread(delegate()
        {
          try
          {
            SetProcessAnimationStatus(true, searchanimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
            List<DbMovieInfo> listemovies = grab.GetTMDBinfos(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, true, choose, MyFilms.conf.StrTitle1, language);
            SetProcessAnimationStatus(false, searchanimation); // GUIWaitCursor.Hide();
            LogMyFilms.Debug("(TMDB-Infos) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "', '" + language + "'");
            int listCount = listemovies.Count;
            LogMyFilms.Debug("(TMDB-Infos) - listemovies: Result Listcount: '" + listCount + "'");

            if (choose)
              listCount = 2;
            switch (listCount)
            {
              case 0:
                break;
              case 1:
                LogMyFilms.Info("Posters " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                break;
              default:

                var wotitle_tableau = new ArrayList();
                var wttitle_tableau = new ArrayList();
                var wotitle_sub_tableau = new ArrayList();
                var wttitle_sub_tableau = new ArrayList();
                const int MinChars = 2;
                bool Filter = true;

                var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(GUILocalizeStrings.Get(1079857)); // Load TMDB data (online)
                dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                foreach (DbMovieInfo t in listemovies)
                {
                  dlg.Add(t.Name + "  (" + t.Year + ") - Posters: " + t.Posters.Count + " - Id" + t.Identifier);
                  LogMyFilms.Debug("TMDB listemovies: " + t.Name + "  (" + t.Year + ") - Posters: " + t.Posters.Count + " - TMDB-Id: " + t.Identifier);
                }
                if (!(dlg.SelectedLabel > -1))
                {
                  dlg.SelectedLabel = -1;
                  dlg.DoModal(wGetID);
                }
                if (dlg.SelectedLabel == 0)
                {
                  #region Get SubTitles and Subwords from otitle and ttitle
                  wotitle_tableau = MyFilms.SubTitleGrabbing(wtitle);
                  wttitle_tableau = MyFilms.SubTitleGrabbing(wttitle);
                  wotitle_sub_tableau = MyFilms.SubWordGrabbing(wtitle, MinChars, Filter); // Min 3 Chars, Filter true (no der die das)
                  wttitle_sub_tableau = MyFilms.SubWordGrabbing(wttitle, MinChars, Filter); // Min 3 Chars, Filter true (no der die das)
                  //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
                  var dlgs = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                  if (dlgs == null) return;
                  dlgs.Reset();
                  dlgs.SetHeading(GUILocalizeStrings.Get(1079859)); // choose search expression
                  dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079858) + "  *****  "); //manual selection with keyboard
                  //dlgs.Add(wtitle); //Otitle
                  dlgs.Add(savetitle); //Otitle = savetitle
                  dlgs.Add(wttitle); //Ttitle
                  foreach (object t in from object t in wotitle_tableau where t.ToString().Length > 1 select t)
                  {
                    dlgs.Add(t.ToString());
                  }
                  foreach (object t in from object t in wttitle_tableau where t.ToString().Length > 1 select t)
                  {
                    dlgs.Add(t.ToString());
                  }
                  foreach (object t in from object t in wotitle_sub_tableau where t.ToString().Length > 1 select t)
                  {
                    dlgs.Add(t.ToString());
                  }
                  foreach (object t in from object t in wttitle_sub_tableau where t.ToString().Length > 1 select t)
                  {
                    dlgs.Add(t.ToString());
                  }
                  //Now all titles and Substrings listed in dialog !
                  //dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                  if (!(dlgs.SelectedLabel > -1))
                  {
                    dlgs.SelectedLabel = -1;
                    dlgs.DoModal(wGetID);
                  }
                  if (dlgs.SelectedLabel == 0)
                  {
                    var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                    if (null == keyboard) return;
                    keyboard.Reset();
                    keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
                    keyboard.Text = wtitle;
                    keyboard.DoModal(wGetID);
                    if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                    {
                      Download_TMDB_Posters(keyboard.Text, wttitle, string.Empty, string.Empty, true, wGetID, savetitle, searchanimation);
                    }
                    break;
                  }
                  if (dlgs.SelectedLabel > 0 && dlgs.SelectedLabel < 3) // if one of otitle or ttitle selected, keep year and director
                  {
                    Download_TMDB_Posters(dlgs.SelectedLabelText, wttitle, year, director, true, wGetID, savetitle, searchanimation);
                    //Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                    break;
                  }
                  if (dlgs.SelectedLabel > 2) // For subitems, search without year and director !
                  {
                    Download_TMDB_Posters(dlgs.SelectedLabelText, wttitle, string.Empty, string.Empty, true, wGetID, savetitle, searchanimation);
                    //Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                    break;
                  }
                  #endregion
                }
                if (dlg.SelectedLabel > 0)
                {
                  // Load Posters  -> show progress dialog !

                  var dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
                  if (dlgPrgrs != null)
                  {
                    dlgPrgrs.Reset();
                    dlgPrgrs.DisplayProgressBar = true;
                    dlgPrgrs.ShowWaitCursor = false;
                    dlgPrgrs.DisableCancel(true);
                    dlgPrgrs.SetHeading("MyFilms Artwork Download");
                    dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
                    dlgPrgrs.SetLine(1, "Loading Artwork ...");
                    dlgPrgrs.Percentage = 0;

                    bool first = true;
                    string filename = string.Empty;
                    string filename1 = string.Empty;
                    if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                      wttitle = savetitle; // Was wttitle = wtitle;
                    int i = 0;
                    if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading Poster for '" + savetitle + "'");
                    foreach (string poster in listemovies[dlg.SelectedLabel - 1].Posters)
                    {
                      filename1 = Grabber.GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, poster, wttitle, true, first, out filename);
                      if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + System.IO.Path.GetFileName(filename) + "'");
                      if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / listemovies[dlg.SelectedLabel - 1].Posters.Count;
                      LogMyFilms.Info("Poster " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                      if (first)
                        newPicture = filename1;
                      if (filename == string.Empty)
                        filename = filename1;
                      if (!(filename == "already" && filename1 == "already"))
                        filename = "added";
                      first = false;
                      i++;
                    }
                    foreach (string person in listemovies[dlg.SelectedLabel - 1].Actors)
                    {
                      // ToDo: Load Actorinfodetails (API Call to be written in grabber) and download Person Artwork
                    }
                    listemovies[0].Name = filename;

                    newPictureCatalogname = GetPictureCatalogNameFromFilename(newPicture);

                    if (wGetID != MyFilms.ID_MyFilmsCoverManager)
                    {
                      if (!choose)
                      {
                        MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                        Update_XML_database();
                        LogMyFilms.Info("(Update_XML_database()) - Database Updated for created Coverthumb: " + newPicture);
                      }
                      else
                      {
                        //TMDB_Details_Select();
                        setGUIProperty("picture", newPicture);
                        GUIWindowManager.Process(); // To Update GUI display ...

                        var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079870)); // choice
                        dlgYesNo.SetLine(1, "");
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798733)); // Replace cover with new one
                        dlgYesNo.SetLine(3, "");
                        dlgYesNo.DoModal(wGetID);
                        if (!(dlgYesNo.IsConfirmed))
                        {
                          setGUIProperty("picture", oldPicture);
                          GUIWindowManager.Process();
                          return;
                        }
                        MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                        Update_XML_database();
                        LogMyFilms.Info("(Update_XML_database()) - Database Updated with '" + newPictureCatalogname + "' for created Coverthumb '" + newPicture + "'");
                      }
                    }
                  }
                  if (dlgPrgrs != null) dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); dlgPrgrs.SetLine(2, ""); Thread.Sleep(50); dlgPrgrs.Close(); // Done...
                  // return;
                }
                break;
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug(ex, "Thread 'MyFilmsTMDBLoader' - exception!");
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              if (DetailsUpdated != null) DetailsUpdated(true);
              return 0;
            }, 0, 0, null);
        }) { Name = "MyFilmsTMDBLoader", IsBackground = true }.Start();
    }

    //-------------------------------------------------------------------------------------------
    //  Change local Cover Image
    //-------------------------------------------------------------------------------------------        
    private static string ChangeLocalCover(DataRow[] r1, int Index, bool interactive, bool onlyReturnCount)
    {
      result = new ArrayList();
      ArrayList resultsize = new ArrayList();
      string[] filesfound = new string[100];
      Int64[] filesfoundsize = new Int64[100];
      int filesfoundcounter = 0;
      //string file = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
      //string titlename = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
      //string titlename2 = MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString();
      //string file = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
      //string titlename = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
      //string titlename2 = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
      string directoryname = "";
      //string movieName = "";
      string[] files = null;
      Int64 wsize = 0; // Temporary Filesize detection
      string startPattern = "";
      string currentPicture = getGUIProperty("picture");
      //if ((sr["Picture"].ToString().IndexOf(":\\") == -1) && (sr["Picture"].ToString().Substring(0, 2) != "\\\\"))
      //  conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
      //else
      //  conf.FileImage = sr["Picture"].ToString();

      if (string.IsNullOrEmpty(currentPicture))
        return "";
      string currentPictureName = currentPicture.Substring(currentPicture.LastIndexOf("\\") + 1);
      string currentStorePath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
      string newPicture = ""; // full path to new picture
      string newPictureCatalogname = ""; // entry to be stored in catalog


      if (!currentPicture.StartsWith(currentStorePath))
        return "";
      startPattern = currentStorePath.EndsWith("\\")
                       ? ""
                       : currentPicture.Substring(currentPicture.LastIndexOf("\\") + 1);
      string searchPattern = currentPicture.Substring(currentStorePath.Length);

      int patternLength2 = 999;
      int patternLength = searchPattern.LastIndexOf(".");
      if (searchPattern.Contains("["))
        patternLength2 = searchPattern.LastIndexOf("[") - 1;
      if (patternLength2 < patternLength)
        patternLength = patternLength2;
      searchPattern = searchPattern.Substring(0, patternLength);

      LogMyFilms.Debug("(ChangeLocalCover) - startPattern = '" + startPattern + "', searchPattern = '" + searchPattern + "', currentStorePath = '" + currentStorePath + "'");

      directoryname = currentPicture.Substring(0, currentPicture.LastIndexOf("\\"));

      int nCurrent = -1;
      if (!string.IsNullOrEmpty(directoryname))
      {
        files = Directory.GetFiles(directoryname, @"*" + searchPattern + @"*.jpg", SearchOption.TopDirectoryOnly);
        foreach (string filefound in files)
        {
          wsize = new System.IO.FileInfo(filefound).Length;
          result.Add(filefound);
          resultsize.Add(wsize);
          filesfound[filesfoundcounter] = filefound;
          filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
          if (filefound.ToLower() == currentPicture.ToLower())
            nCurrent = filesfoundcounter;
          filesfoundcounter = filesfoundcounter + 1;
        }
      }

      if (onlyReturnCount)
        return filesfoundcounter.ToString();

      foreach (string n in filesfound.Where(n => !string.IsNullOrEmpty(n)))
      {
        LogMyFilms.Debug("(Sorted coverfiles) ******* : '" + n + "'");
      }

      Array.Sort(filesfoundsize);
      for (int i = 0; i < result.Count; i++)
      {
        if (!string.IsNullOrEmpty(filesfound[i]))
          LogMyFilms.Debug("(Sorted coverfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
      }

      if (filesfoundcounter == 1) return "(1)";
      if (result.Count > 0)
      {
        if (nCurrent == -1)
          return "(" + filesfoundcounter.ToString() + ")";
        nCurrent++;
        if (nCurrent >= result.Count)
          nCurrent = 0;
        newPicture = filesfound[nCurrent];

        newPictureCatalogname = GetPictureCatalogNameFromFilename(newPicture);
        if (!interactive)
        {
          // add confirmation dialog here....
        }
        else
        {
          MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
          LogMyFilms.Debug("(ChangeLocalCover) : NewCatalogName: '" + newPictureCatalogname + "'");
          Update_XML_database();
        }
        return "(" + filesfoundcounter + ")";
      }
      else
        LogMyFilms.Debug("MyFilmsDetails (LocalCoverChange) - NO COVERS FOUND !!!!");
      return "";
    }

    public static Searchtitles GetSearchTitles(DataRow movieRecord, string mediapath)
    {
      return GetSearchTitles(movieRecord, mediapath, MyFilms.conf);
    }
    public static Searchtitles GetSearchTitles(DataRow movieRecord, string mediapath, MyFilmsPlugin.MyFilmsGUI.Configuration tmpconf) // returns the first title name of the configured mastertitle field
    {
      var stitles = new Searchtitles();
      stitles.SearchTitle = "";
      stitles.FanartTitle = "";
      stitles.MasterTitle = "";
      stitles.SecondaryTitle = "";
      stitles.SortTitle = "";
      stitles.MovieDirectoryTitle = "";
      stitles.MovieFileTitle = "";
      stitles.OriginalTitle = "";
      stitles.TranslatedTitle = "";
      stitles.FormattedTitle = "";
      stitles.Year = 0;
      stitles.Director = "";

      if (movieRecord["OriginalTitle"] != null && movieRecord["OriginalTitle"].ToString().Length > 0)
        stitles.OriginalTitle = RemoveGroupNames(movieRecord["OriginalTitle"].ToString());
      if (tmpconf.StrTitle1 == "OriginalTitle")
        stitles.MasterTitle = stitles.OriginalTitle;
      if (tmpconf.StrTitle2 == "OriginalTitle")
        stitles.SecondaryTitle = stitles.OriginalTitle;
      if (tmpconf.StrSTitle == "OriginalTitle")
        stitles.SortTitle = stitles.OriginalTitle;

      if (movieRecord["TranslatedTitle"] != null && movieRecord["TranslatedTitle"].ToString().Length > 0)
        stitles.TranslatedTitle = RemoveGroupNames(movieRecord["TranslatedTitle"].ToString());
      if (tmpconf.StrTitle1 == "TranslatedTitle")
        stitles.MasterTitle = stitles.TranslatedTitle;
      if (tmpconf.StrTitle2 == "TranslatedTitle")
        stitles.SecondaryTitle = stitles.TranslatedTitle;
      if (tmpconf.StrSTitle == "TranslatedTitle")
        stitles.SortTitle = stitles.TranslatedTitle;

      if (movieRecord["FormattedTitle"] != null && movieRecord["FormattedTitle"].ToString().Length > 0)
        stitles.FormattedTitle = RemoveGroupNames(movieRecord["FormattedTitle"].ToString());
      if (tmpconf.StrTitle1 == "FormattedTitle")
        stitles.MasterTitle = stitles.FormattedTitle;
      if (tmpconf.StrTitle2 == "FormattedTitle")
        stitles.SecondaryTitle = stitles.FormattedTitle;
      if (tmpconf.StrSTitle == "FormattedTitle")
        stitles.SortTitle = stitles.FormattedTitle;

      if (movieRecord["OriginalTitle"].ToString().Length > 0 && tmpconf.StrFanart)
      {
        try
        {
          stitles.Year = System.Convert.ToInt16(movieRecord["Year"]);
        }
        catch
        {
          stitles.Year = 0;
        }
        try
        {
          stitles.Director = (string)movieRecord["Director"];
        }
        catch
        {
          stitles.Director = string.Empty;
        }
      }
      if (tmpconf.StrTitle1 == "FormattedTitle") // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
      {
        if (!string.IsNullOrEmpty(stitles.TranslatedTitle))
          stitles.FanartTitle = stitles.TranslatedTitle;
        else if (!string.IsNullOrEmpty(stitles.OriginalTitle))
          stitles.FanartTitle = stitles.OriginalTitle;
        else
          stitles.FanartTitle = stitles.FormattedTitle;
      }
      else
      {
        if (!string.IsNullOrEmpty(stitles.MasterTitle))
          stitles.FanartTitle = stitles.MasterTitle;
        else if (!string.IsNullOrEmpty(stitles.OriginalTitle))
          stitles.FanartTitle = stitles.OriginalTitle;
        else if (!string.IsNullOrEmpty(stitles.TranslatedTitle))
          stitles.FanartTitle = stitles.TranslatedTitle;
        else if (!string.IsNullOrEmpty(stitles.FormattedTitle))
          stitles.FanartTitle = stitles.FormattedTitle;
        else stitles.FanartTitle = "";
      }

      // if mediapath is given, create mediapath search titles
      if (!string.IsNullOrEmpty(mediapath) && mediapath.Contains(".") && mediapath.Contains(@"\"))
      {
        string CleanStringFile = "";
        string CleanStringDir = "";
        Regex CutText;
        Match m;

        // filename
        //Strip Path
        CleanStringFile = mediapath.Substring(mediapath.LastIndexOf(@"\") + 1);
        //Strip Extension
        CleanStringFile = CleanStringFile.Substring(0, CleanStringFile.LastIndexOf("."));
        CutText = new Regex(@"\(" + "[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]" + @"\)");
        m = CutText.Match(CleanStringFile);
        if (m.Success) //Finally remove anything which may be a multi-part indicator (e.g. 1of2)
          CleanStringFile = CutText.Replace(CleanStringFile, "");
        CutText = new Regex("[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]");
        m = CutText.Match(CleanStringFile);
        if (m.Success) CleanStringFile = CutText.Replace(CleanStringFile, "");
        stitles.MovieFileTitle = Utility.RemoveNastyCharacters(CleanStringFile.Trim());

        // foldername
        //Strip filename:
        CleanStringDir = mediapath.Substring(0, mediapath.LastIndexOf(@"\"));
        //Strip Path:
        CleanStringDir = CleanStringDir.Substring(CleanStringDir.LastIndexOf(@"\") + 1);
        CutText = new Regex(@"\(" + "[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]" + @"\)");
        m = CutText.Match(CleanStringDir);
        if (m.Success) //Finally remove anything which may be a multi-part indicator (e.g. 1of2)
          CleanStringDir = CutText.Replace(CleanStringDir, "");
        CutText = new Regex("[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]");
        m = CutText.Match(CleanStringDir);
        if (m.Success) CleanStringDir = CutText.Replace(CleanStringDir, "");
        stitles.MovieDirectoryTitle = Utility.RemoveNastyCharacters(CleanStringDir.Trim());

        // DVD_Folders
        if (mediapath.ToLower().Contains("video_ts"))
        {
          string CleanString = Utility.GetDVDFolderName(mediapath);
          stitles.MovieDirectoryTitle = Utility.RemoveNastyCharacters(CleanString.Trim());
        }
        if (mediapath.ToLower().Contains("index.bdmv"))
        {
          string CleanString = Utility.GetBRFolderName(mediapath);
          stitles.MovieDirectoryTitle = Utility.RemoveNastyCharacters(CleanString.Trim());
        }
      }

      if (MyFilms.conf == tmpconf) // only log if internal operation
        LogMyFilms.Debug("GetSearchTitles: returning Titles: '" + stitles.FanartTitle + "' - mastertitle (" + tmpconf.StrTitle1 + ") =  '" + stitles.MasterTitle + "' - originaltitle: '" + stitles.OriginalTitle + "' - translatedtitle: '" + stitles.TranslatedTitle + "' - formattedtitle: '" + stitles.FormattedTitle + "' - director: '" + stitles.Director + "' - year: '" + stitles.Year.ToString() + "'");
      return stitles;
    }

    // returns the first title name of the configured mastertitle field
    public static string GetFanartTitle(DataRow movieRecord, out string wtitle, out string wttitle, out string wftitle, out int wyear, out string wdirector)
    {
      string fanartTitle = "";
      string mastertitle = string.Empty;
      wtitle = wttitle = wftitle = wdirector = string.Empty;
      wyear = 0;

      if (movieRecord["OriginalTitle"] != null && movieRecord["OriginalTitle"].ToString().Length > 0)
        wtitle = RemoveGroupNames(movieRecord["OriginalTitle"].ToString());
      if (MyFilms.conf.StrTitle1 == "OriginalTitle")
        mastertitle = wtitle;

      if (movieRecord["TranslatedTitle"] != null && movieRecord["TranslatedTitle"].ToString().Length > 0)
        wttitle = RemoveGroupNames(movieRecord["TranslatedTitle"].ToString());
      if (MyFilms.conf.StrTitle1 == "TranslatedTitle")
        mastertitle = wttitle;

      if (movieRecord["FormattedTitle"] != null && movieRecord["FormattedTitle"].ToString().Length > 0)
        wftitle = RemoveGroupNames(movieRecord["FormattedTitle"].ToString());
      if (MyFilms.conf.StrTitle1 == "FormattedTitle")
        mastertitle = wftitle;

      if (movieRecord["OriginalTitle"].ToString().Length > 0 && MyFilms.conf.StrFanart)
      {
        try
        {
          wyear = System.Convert.ToInt16(movieRecord["Year"]);
        }
        catch
        {
          wyear = 0;
        }
        try
        {
          wdirector = (string)movieRecord["Director"];
        }
        catch
        {
          wdirector = string.Empty;
        }
      }
      if (MyFilms.conf.StrTitle1 == "FormattedTitle") // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
      {
        if (!string.IsNullOrEmpty(wttitle))
          fanartTitle = wttitle;
        else if (!string.IsNullOrEmpty(wtitle))
          fanartTitle = wtitle;
        else
          fanartTitle = wftitle;
      }
      else
      {
        if (!string.IsNullOrEmpty(mastertitle))
          fanartTitle = mastertitle;
        else if (!string.IsNullOrEmpty(wtitle))
          fanartTitle = wtitle;
        else if (!string.IsNullOrEmpty(wttitle))
          fanartTitle = wttitle;
        else if (!string.IsNullOrEmpty(wftitle))
          fanartTitle = wftitle;
        else fanartTitle = "";
      }

      LogMyFilms.Debug("GetFanartTitle: returning fanartTitle: '" + fanartTitle + "' - mastertitle (" + MyFilms.conf.StrTitle1 + ") =  '" + mastertitle + "' - originaltitle: '" + wtitle + "' - translatedtitle: '" + wttitle + "' - formattedtitle: '" + wftitle + "' - director: '" + wdirector + "' - year: '" + wyear.ToString() + "'");
      return fanartTitle;
    }

    // returns the first title name of the configured mastertitle field
    public static string GetIMDB_Id(DataRow movieRecord)
    {
      string imdb = "";
      if (!string.IsNullOrEmpty(movieRecord["IMDB_Id"].ToString()))
      {
        imdb = movieRecord["IMDB_Id"].ToString();
      }
      else if (!string.IsNullOrEmpty(movieRecord["URL"].ToString()))
      {
        string cleanString = movieRecord["URL"].ToString();
        var cutText = new Regex("" + @"tt\d{7}" + "");
        var m = cutText.Match(cleanString);
        if (m.Success)
          imdb = m.Value;
      }
      //LogMyFilms.Debug("GetIMDB_Id: returning IMDB_Id: '" + IMDB + "'");
      return imdb;
    }

    private static string RemoveGroupNames(string fullName)
    {
      string name = fullName;
      if (MyFilms.conf != null && !string.IsNullOrEmpty(MyFilms.conf.TitleDelim)) // if called from BaseFilms
      {
        if (fullName.IndexOf(MyFilms.conf.TitleDelim, System.StringComparison.Ordinal) > 0)
          name = fullName.Substring(fullName.IndexOf(MyFilms.conf.TitleDelim) + 1);
      }
      return name;
    }

    //-------------------------------------------------------------------------------------------
    //  Remove Old backdrops Fanart already downloaded (case in title change)
    //-------------------------------------------------------------------------------------------        
    public static void Remove_Backdrops_Fanart(string wtitle, bool suppressDir)
    {
      if (wtitle.Length > 0)
      {
        if (wtitle.Contains(MyFilms.conf.TitleDelim))
          wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
        wtitle = Grabber.GrabUtil.CreateFilename(wtitle.ToLower()).Replace(' ', '.');
        if (suppressDir)
        {
          try { Directory.Delete(MyFilms.conf.StrPathFanart + "\\{" + wtitle + "}"); }
          catch { }
        }
        else
        {
          try
          {
            var dirsInf = new DirectoryInfo(MyFilms.conf.StrPathFanart + "\\{" + wtitle + "}");
            FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
            foreach (FileSystemInfo sfi in sfiles)
            {
              try
              {
                System.IO.File.Delete(sfi.FullName);
              }
              catch (Exception ex)
              {
                LogMyFilms.Error("Remove_Backdrops_Fanart() - error on deletion in directory: '" + wtitle + "', file: '" + sfi.FullName + "', exception: '" + ex.Message + "'");
              }
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("Remove_Backdrops_Fanart() - error on getting directory info: '" + wtitle + "', exception: '" + ex.Message + "'");
          }
        }
      }
    }
    //-------------------------------------------------------------------------------------------
    //  Dowload backdrops on theMovieDB.org
    //-------------------------------------------------------------------------------------------        
    public static void Download_Backdrops_Fanart(string wtitle, string wttitle, string wftitle, string director, string imdbid, string year, bool choose, int wGetID, string savetitle, string personartworkpath, bool loadFanart, bool loadPersonImages, GUIAnimation searchanimation)
    {
      new Thread(delegate()
      {
        var grab = new Grabber_URLClass();
        int wyear = 0;
        try { wyear = Convert.ToInt32(year); }
        catch { }
        try
        {
          SetProcessAnimationStatus(true, searchanimation);  // GUIWaitCursor.Init(); GUIWaitCursor.Show();
          List<DbMovieInfo> listemovies = grab.GetFanart(
            wtitle,
            savetitle,
            wyear,
            director,
            imdbid,
            MyFilms.conf.StrPathFanart,
            true,
            choose,
            MyFilms.conf.StrTitle1,
            personartworkpath);
          SetProcessAnimationStatus(false, searchanimation);  //GUIWaitCursor.Hide();
          //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathFanart, true, choose);
          LogMyFilms.Debug("(DownloadBackdrops) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathFanart + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "'");
          int listCount = listemovies.Count;
          LogMyFilms.Debug("(DownloadBackdrops) - listemovies: Result Listcount: '" + listCount.ToString() + "'");

          if (choose) listCount = 2;
          switch (listCount)
          {
            case 0:
              break;
            case 1:
              LogMyFilms.Debug("Fanart " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
              if (listemovies[0].Persons.Count > 0)
              {
                LogMyFilms.Debug("PersonArtwork: " + listemovies[0].Persons.Count.ToString() + " Persons checked for " + wttitle);
                foreach (DbPersonInfo person in listemovies[0].Persons)
                {
                  LogMyFilms.Debug("PersonArtwork: " + person.Images.Count.ToString() + " images found for " + person.Name);
                }
              }
              break;
            default:

              var wotitle_tableau = new ArrayList();
              var wttitle_tableau = new ArrayList();
              var wotitle_sub_tableau = new ArrayList();
              var wttitle_sub_tableau = new ArrayList();
              const int MinChars = 2;
              bool Filter = true;

              var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
              if (dlg == null) return;
              dlg.Reset();
              dlg.SetHeading(loadFanart ? GUILocalizeStrings.Get(1079862) : GUILocalizeStrings.Get(1079900));  // Load fanart (online)  // Download person images (selected film)
              dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
              foreach (DbMovieInfo t in listemovies)
              {
                string dialoginfoline = t.Name + "  (" + t.Year + ")";
                if (loadFanart) dialoginfoline += " - Fanarts: " + t.Backdrops.Count;
                if (loadPersonImages) dialoginfoline += " - Persons: " + t.Persons.Count.ToString();
                dlg.Add(dialoginfoline);
                LogMyFilms.Debug("TMDB listemovies: " + t.Name + "  (" + t.Year + ") - Fanarts: " + t.Backdrops.Count + " - TMDB-Id: " + t.Identifier + " - Persons: " + t.Persons.Count.ToString());
              }
              if (!(dlg.SelectedLabel > -1))
              {
                dlg.SelectedLabel = -1;
                dlg.DoModal(wGetID);
              }
              if (dlg.SelectedLabel == 0)
              {
                #region Get SubTitles and Subwords from otitle and ttitle

                wotitle_tableau = MyFilms.SubTitleGrabbing(wtitle);
                wttitle_tableau = MyFilms.SubTitleGrabbing(wttitle);
                wotitle_sub_tableau = MyFilms.SubWordGrabbing(wtitle, MinChars, Filter);
                // Min 3 Chars, Filter true (no der die das)
                wttitle_sub_tableau = MyFilms.SubWordGrabbing(wttitle, MinChars, Filter);
                // Min 3 Chars, Filter true (no der die das)
                //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
                var dlgSearchFilm = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlgSearchFilm == null) return;
                dlgSearchFilm.Reset();
                dlgSearchFilm.SetHeading(GUILocalizeStrings.Get(1079859)); // choose search expression
                dlgSearchFilm.Add("  *****  " + GUILocalizeStrings.Get(1079858) + "  *****  ");
                //manual selection with keyboard
                //dlgs.Add(wtitle); //Otitle
                dlgSearchFilm.Add(savetitle); //Otitle = savetitle
                dlgSearchFilm.Add(wttitle); //Ttitle
                foreach (object t in from object t in wotitle_tableau where t.ToString().Length > 1 select t)
                {
                  dlgSearchFilm.Add(t.ToString());
                }
                foreach (object t in from object t in wttitle_tableau where t.ToString().Length > 1 select t)
                {
                  dlgSearchFilm.Add(t.ToString());
                }
                foreach (object t in from object t in wotitle_sub_tableau where t.ToString().Length > 1 select t)
                {
                  dlgSearchFilm.Add(t.ToString());
                }
                foreach (object t in from object t in wttitle_sub_tableau where t.ToString().Length > 1 select t)
                {
                  dlgSearchFilm.Add(t.ToString());
                }
                //Now all titles and Substrings listed in dialog !
                //dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                if (!(dlgSearchFilm.SelectedLabel > -1))
                {
                  dlgSearchFilm.SelectedLabel = -1;
                  dlgSearchFilm.DoModal(wGetID);
                }
                if (dlgSearchFilm.SelectedLabel == 0) // enter manual searchstring via VK
                {
                  var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                  if (null == keyboard) return;
                  keyboard.Reset();
                  keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
                  keyboard.Text = wtitle;
                  keyboard.DoModal(wGetID);
                  if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                  {
                    //Remove_Backdrops_Fanart(wtitle, true);
                    //Remove_Backdrops_Fanart(wttitle, true);
                    //Remove_Backdrops_Fanart(wftitle, true);
                    Download_Backdrops_Fanart(
                      keyboard.Text,
                      string.Empty,
                      string.Empty,
                      string.Empty,
                      string.Empty,
                      string.Empty,
                      true,
                      wGetID,
                      savetitle,
                      personartworkpath,
                      loadFanart,
                      loadPersonImages,
                      searchanimation);
                  }
                  break;
                }
                if (dlgSearchFilm.SelectedLabel > 0 && dlgSearchFilm.SelectedLabel < 3) // if one of otitle or ttitle selected, keep year and director
                {
                  Download_Backdrops_Fanart(
                    dlgSearchFilm.SelectedLabelText,
                    wttitle,
                    wftitle,
                    year,
                    director,
                    string.Empty,
                    true,
                    wGetID,
                    savetitle,
                    personartworkpath,
                    loadFanart,
                    loadPersonImages,
                    searchanimation);
                  //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                  break;
                }
                if (dlgSearchFilm.SelectedLabel > 2) // For subitems, search without year and director !
                {
                  Download_Backdrops_Fanart(
                    dlgSearchFilm.SelectedLabelText,
                    wttitle,
                    wftitle,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    true,
                    wGetID,
                    savetitle,
                    personartworkpath,
                    loadFanart,
                    loadPersonImages,
                    searchanimation);
                  //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                  break;
                }

                #endregion
              }
              if (dlg.SelectedLabel > 0)
              {
                // Load Fanart  -> show progress dialog !

                var dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
                if (dlgPrgrs != null)
                {
                  dlgPrgrs.Reset();
                  dlgPrgrs.DisplayProgressBar = true;
                  dlgPrgrs.ShowWaitCursor = false;
                  dlgPrgrs.DisableCancel(true);
                  dlgPrgrs.SetHeading("MyFilms Artwork Download");
                  dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
                  dlgPrgrs.SetLine(1, "Loading Artwork ...");
                  dlgPrgrs.Percentage = 0;

                  #region load fanarts ...
                  bool first = true;
                  string filename = string.Empty;
                  string filename1 = string.Empty;
                  //if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                  //  wttitle = savetitle; // Was wttitle = wtitle;
                  int i = 0;
                  if (loadFanart) // Download Fanart
                  {
                    if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading Fanart for '" + savetitle + "'");

                    foreach (string backdrop in listemovies[dlg.SelectedLabel - 1].Backdrops)
                    {
                      filename1 = Grabber.GrabUtil.DownloadBacdropArt(MyFilms.conf.StrPathFanart, backdrop, savetitle, true, first, out filename);
                      if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + System.IO.Path.GetFileName(filename) + "'");
                      if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / listemovies[dlg.SelectedLabel - 1].Backdrops.Count;
                      LogMyFilms.Debug("Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + savetitle);

                      if (filename == string.Empty) filename = filename1;
                      if (!(filename == "already" && filename1 == "already")) filename = "added";
                      first = false;
                      i++;
                    }
                  }
                  #endregion

                  listemovies[0].Name = filename;

                  if (loadPersonImages) // Download PersonArtwork
                  {
                    string filenameperson = string.Empty;
                    string filename1person = string.Empty;
                    string filename2person = string.Empty;
                    LogMyFilms.Debug(
                      "Person Artwork - " + listemovies[0].Persons.Count + " persons found - now loading artwork");
                    if (!string.IsNullOrEmpty(personartworkpath) && listemovies[0].Persons != null &&
                        listemovies[0].Persons.Count > 0)
                    {
                      if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading person images for '" + wttitle + "'");
                      if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "");

                      foreach (DbPersonInfo person in listemovies[0].Persons)
                      {
                        bool firstpersonimage = true;
                        bool onlysinglepersonimage = true;
                        var persondetails = new DbPersonInfo();
                        var theMoviedb = new TheMoviedb();
                        persondetails = theMoviedb.GetPersonsById(person.Id, string.Empty);
                        LogMyFilms.Debug("Person Artwork: found '" + persondetails.Images.Count + "' TMDB images for '" + persondetails.Name + "' in movie '" + savetitle + "'");
                        if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + persondetails.Name + "'");
                        if (dlgPrgrs != null) dlgPrgrs.Percentage = 0;

                        if (persondetails.Images.Count > 0)
                        {
                          i = 0;
                          foreach (var image in persondetails.Images)
                          {
                            filename1person = Grabber.GrabUtil.DownloadPersonArtwork(personartworkpath, image, persondetails.Name, true, firstpersonimage, out filenameperson);
                            if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + persondetails.Name + "' (TMDB - #" + i + ")");
                            if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / persondetails.Images.Count;

                            LogMyFilms.Debug("Person Artwork " + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + " downloaded for '" + persondetails.Name + "' in movie '" + savetitle + "', path='" + filename1person + "'");
                            if (filenameperson == string.Empty) filenameperson = filename1person;
                            if (!(filenameperson == "already" && filename1person == "already")) filenameperson = "added";
                            firstpersonimage = false;
                            i++;
                            if (onlysinglepersonimage) break;
                          }
                        }
                      }
                    }
                    else if (string.IsNullOrEmpty(personartworkpath)) LogMyFilms.Debug("No Personartwork loaded - Personartworkpath is not set in setup!");
                  }
                  if (dlgPrgrs != null) dlgPrgrs.Percentage = 100;
                  dlgPrgrs.ShowWaitCursor = false;
                  dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846));
                  dlgPrgrs.SetLine(2, "");
                  Thread.Sleep(50);
                  dlgPrgrs.Close(); // Done...
                  return;
                }
              }
              break;
          }
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex, "Thread 'MyFilmsTMDBLoader' - exception!");
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          //dlgPrgrs.ShowWaitCursor = false;
          // enter here what to load after background thread has finished !
          if (DetailsUpdated != null) DetailsUpdated(true);
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsTMDBLoader", IsBackground = true }.Start();
    }


    private static bool MovieThumbsExist(int index)
    {
      string movieThumbsDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsThumbsPath) + @"\MovieThumbs";
      string title = GetSearchTitles(MyFilms.r[index], "").FanartTitle;
      if (MyFilms.conf.StrFanart)
      {
        if (title.Contains(MyFilms.conf.TitleDelim)) title = title.Substring(title.LastIndexOf(MyFilms.conf.TitleDelim, System.StringComparison.Ordinal) + 1);
        title = Grabber.GrabUtil.CreateFilename(title.ToLower()).Replace(' ', '.');
        string safeName = movieThumbsDirectory + "\\{" + title + "}"; // string safeName = MyFilms.conf.StrPathFanart + "\\{" + title + "}";

        if (!Directory.Exists(safeName)) return false;
        return Directory.GetFiles(safeName, @"*[single]*", SearchOption.TopDirectoryOnly).Length > 0;
      }
      return false;
    }
    
    private static List<string> SearchMovieThumbs(int index, bool createmissingmoviethumbs) // searches moviethumbs for the current active movie in dataset
    {
      LogMyFilms.Debug("SearchMovieThumbs() - search or create movie thumbs started ...");
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      Searchtitles stitles = GetSearchTitles(MyFilms.r[index], "");
      string title = stitles.FanartTitle;

      string movieThumbsDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsThumbsPath) + @"\MovieThumbs";
      var moviethumbs = new List<string>();

      if (MyFilms.conf.StrFanart)
      {
        if (title.Contains(MyFilms.conf.TitleDelim)) title = title.Substring(title.LastIndexOf(MyFilms.conf.TitleDelim, System.StringComparison.Ordinal) + 1);
        title = Grabber.GrabUtil.CreateFilename(title.ToLower()).Replace(' ', '.');

        string safeName = movieThumbsDirectory + "\\{" + title + "}"; // string safeName = MyFilms.conf.StrPathFanart + "\\{" + title + "}";

        try //Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
        {
          var wfile = new FileInfo(safeName + "\\{" + title + "}.jpg");
        }
        catch (Exception e)
        {
          LogMyFilms.Error("Title too long to create moviethumb path/filename from it - not loading moviethumbs! - Exception: " + e.Message);
          return moviethumbs;
        }

        if (!Directory.Exists(safeName))
        {
          // GUIControl.HideControl(GUIWindowManager.ActiveWindow, (int)Controls.CTRL_MovieThumbsDir);
          try { Directory.CreateDirectory(safeName); }
          catch { }
        }
        
        if (createmissingmoviethumbs && Directory.GetFiles(safeName, @"*[single]*", SearchOption.TopDirectoryOnly).Length == 0)
        {
          string movieFile = GetMediaPathOfFirstFile(MyFilms.r, index);
          if (!string.IsNullOrEmpty(stitles.FanartTitle) && MyFilms.conf.StrFanart && !string.IsNullOrEmpty(movieFile))
          {
            try
            {
              bool success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.Year.ToString(), movieThumbsDirectory, GrabUtil.ArtworkFanartType.MultipleSingleImagesAsMovieThumbs, movieFile, "localfanart", 0);
            }
            catch (Exception)
            {
            }
          }
        }
        var validfiles = Directory.GetFiles(safeName, @"*[single]*", SearchOption.TopDirectoryOnly);
        moviethumbs.AddRange(validfiles);
      }
      stopwatch.Stop();
      LogMyFilms.Debug("SearchMovieThumbs() - found '" + moviethumbs.Count + "' moviethumbs for '" + title + "' (" + stopwatch.ElapsedMilliseconds + " ms).");
      return moviethumbs;
    }


    //-------------------------------------------------------------------------------------------
    //  Search Fanart Thumbs 
    //                          title = Translated Title if any or Original Title
    //                          main = true for main screen and false for detailed
    //                          searched = dir for Directory searched (detail screen and control multiImage)or file 
    //                          isGroupView = true if the selected Item is a grouped Item (country, genre, year...) on main screen
    //                          filecover = name of the file cover for using 'Default Cover for missing Fanart'
    //      value returned string[2]
    //                          [0] = file or directory found (if not " ")
    //                          [1] = file or dir
    //-------------------------------------------------------------------------------------------

    public static string[] Search_Fanart(string title, bool main, string searched, bool isGroupView, string filecover, string group)
    {
      return Search_Fanart(title, main, searched, isGroupView, filecover, group, MyFilms.conf);
    }
    public static string[] Search_Fanart(string title, bool main, string searched, bool isGroupView, string filecover, string group, MyFilmsPlugin.MyFilmsGUI.Configuration tmpconf)
    //                     Search_Fanart(wlabel, true, "file", false, facadeFilms.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
    {
      //if (MyFilms.conf == tmpconf) LogMyFilms.Debug("Search_Fanart(): Using '" + title + "'");
      var wfanart = new string[2];
      wfanart[0] = " ";
      wfanart[1] = " ";
      if (tmpconf.StrFanart)
      {
        if (title.Contains(tmpconf.TitleDelim))
          title = title.Substring(title.LastIndexOf(tmpconf.TitleDelim, System.StringComparison.Ordinal) + 1);
        title = Grabber.GrabUtil.CreateFilename(title.ToLower()).Replace(' ', '.');

        if (!tmpconf.StrFanart)
          return wfanart;

        string safeName = string.Empty;
        if (isGroupView)
        {
          if ((group == "country" || group == "year" || group == "category") && tmpconf.StrFanartDefaultViews) // Default views and fanart for group view enabled?
          {
            if (!Directory.Exists(tmpconf.StrPathFanart + "\\_View")) Directory.CreateDirectory(tmpconf.StrPathFanart + "\\_View");
            if (!Directory.Exists(tmpconf.StrPathFanart + "\\_View\\" + group)) Directory.CreateDirectory(tmpconf.StrPathFanart + "\\_View\\" + group);
            safeName = tmpconf.StrPathFanart + "\\_View\\" + group + "\\{" + title + "}";
          }
          else
            if (tmpconf.StrFanartDfltImageAll && (wfanart[0] == "" || wfanart[0] == " "))
            {
              wfanart[0] = tmpconf.DefaultFanartImage;
              wfanart[1] = "file";
              return wfanart;
            }
        }
        else

          if ((tmpconf.StrPathFanart + title + "\\{" + title + "}.jpg").Length > 259) // Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
          {
            return wfanart;
          }
          else
          {
            safeName = tmpconf.StrPathFanart + "\\{" + title + "}";
          }


        try //Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
        {
          var wfile = new FileInfo(safeName + "\\{" + title + "}.jpg");
        }
        catch (Exception e)
        {
          LogMyFilms.Error("Title too long to create fanart path/filename from it - not loading fanart! - Exception: " + e.Message);
          return wfanart;
        }

        //LogMyFilms.Debug("(SearchFanart) - safename(file) = '" + wfile + "'");
        //LogMyFilms.Debug("(SearchFanart) - safename(file&ext) = '" + (safeName + "\\{" + title + "}.jpg") + "'");
        if ((main || searched == "file") && File.Exists(safeName + "\\{" + title + "}.jpg"))
        {
          wfanart[0] = safeName + "\\{" + title + "}.jpg";
          wfanart[1] = "file";
          return wfanart;
        }

        if (Directory.Exists(safeName))
        {
          if (main || searched == "file")
          {
            if (Directory.GetFiles(safeName).Length > 0)
            {
              wfanart[0] = Directory.GetFiles(safeName)[0];
              wfanart[1] = "file";
              LogMyFilms.Debug("Search_Fanart(): File Mode - searchtitle = '" + title + "', safename = '" + safeName + "'wfanart[0,1]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
              return wfanart;
            }
          }
          else
          {
            if (Directory.GetFiles(safeName).Length > 0)
            {
              wfanart[0] = safeName;
              wfanart[1] = "dir";
              return wfanart;
            }
          }
        }
        else
        {
          try { Directory.CreateDirectory(safeName); }
          catch { }
        }

        // Added to support fanart for external catalogs
        switch (tmpconf.StrFileType)
        {
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog3:
            break;
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.AntMovieCatalog4Xtended:
            break;

          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.DVDProfiler:
            break;
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.MovieCollector:
            break;
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.MyMovies:
            break;
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.EaxMovieCatalog2:
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.EaxMovieCatalog3:
            if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in XMM fanart directory
            {
              string searchname = HTMLParser.removeHtml(title).Replace(" ", "."); // replaces special character "á" and other special chars !
              //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
              searchname = searchname + ".*.jpg";
              string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
              if (files.Any())
              {
                wfanart[0] = files[0];
                wfanart[1] = "file";
                return wfanart;
              }
            }
            break;
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.PersonalVideoDatabase: // PVD artist thumbs: e.g. Natalie Portman_1.jpg , then Natalie Portman_2.jpg 
            if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in XMM fanart directory
            {
              string searchname = HTMLParser.removeHtml(title); // replaces special character "á" and other special chars !
              //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
              searchname = searchname + "*.jpg";
              string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
              if (files.Any())
              {
                wfanart[0] = files[0];
                wfanart[1] = "file";
                return wfanart;
              }
            }
            break;
          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.eXtremeMovieManager:
            if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in XMM fanart directory
            {
              string searchname = HTMLParser.removeHtml(title).Replace(" ", "-"); // replaces special character "á" and other special chars !
              searchname = searchname.Replace(" ", "-");
              searchname = searchname.Replace(".", "-");
              searchname = searchname.Replace("'", "-");
              searchname = "*" + Regex.Replace(searchname, "[\n\r\t]", "-") + "_fanart*.jpg";
              string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
              LogMyFilms.Debug("Search_Fanart - XMM - search for '" + searchname + "'");
              if (files.Any())
              {
                wfanart[0] = files[0];
                wfanart[1] = "file";
                return wfanart;
              }
            }
            break;

          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.XBMC: // XBMC fulldb export (all movies in one DB)
            break;

          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.MovingPicturesXML:
            if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in MoPi fanart directory
            {
              string searchname = HTMLParser.removeHtml(title); // replaces special character "á" and other special chars !
              //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
              searchname = "{" + searchname + "}" + "*.jpg";
              string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
              if (files.Any())
              {
                wfanart[0] = files[0];
                wfanart[1] = "file";
                return wfanart;
              }
            }
            break;

          case MyFilmsPlugin.MyFilmsGUI.Configuration.CatalogType.XBMCnfoReader: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
            break;

        }
        if ((tmpconf.StrFanartDflt) && !(isGroupView) && System.IO.File.Exists(filecover))
        {
          wfanart[0] = filecover;
          wfanart[1] = "file";
          //Added Guzzi - Fix that no fanart was returned ...
          return wfanart;
        }
        if (tmpconf.StrFanartDfltImage && (wfanart[0] == "" || wfanart[0] == " "))
        {
          wfanart[0] = tmpconf.DefaultFanartImage;
          wfanart[1] = "file";
        }
      }

      // LogMyFilms.Debug("(SearchFanart) - Fanart config for '" + title + "': wfanart[0,1]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
      return wfanart;
    }

    //-------------------------------------------------------------------------------------------
    //  Control search Numeric : only numerics
    //-------------------------------------------------------------------------------------------        
    private string control_searchNum(string stext)
    {
      var maRegexp = new Regex("^[0-9]{1,2}[\\.,]?[0-9]?$");
      if (!maRegexp.IsMatch(stext))
      {
        GUIUtils.ShowErrorDialog("Syntax : nn.n"); //rating Integer from 0 to 10
        return null;
      }
      try
      {
        var wrating = Convert.ToDouble(stext);
        NumberFormatInfo nfi = new CultureInfo("en-US", true).NumberFormat;
        nfi.NumberDecimalDigits = 1;
        if ((wrating > 0) && (wrating < 11))
          return wrating.ToString("N", nfi);
        else
          return null;
      }
      catch
      {
        return null;
      }
    }

    private void OnDetailsUpdated(bool searchPicture)
    {
      LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "', doUpdateDetailsViewByFinishEvent '" + doUpdateDetailsViewByFinishEvent + "'");
      if (GetID == MyFilms.ID_MyFilmsDetail && doUpdateDetailsViewByFinishEvent)
      {
        LogMyFilms.Debug("OnDetailsUpdated(): now reloading Details");
        doUpdateDetailsViewByFinishEvent = false;
        afficher_detail(searchPicture, false);
      }
      else
        LogMyFilms.Debug("OnDetailsUpdated(): Skipping reloading Details");
    }

    private void afficher_detail(bool searchPicture, bool checkfileavailability = false)
    {
      //-----------------------------------------------------------------------------------------------------------------------
      //    Load Detailed Info
      //-----------------------------------------------------------------------------------------------------------------------
      MyFilms.currentMovie.Reset(); // clear currentmovie

      if (MyFilms.conf.StrIndex > MyFilms.r.Length - 1)
        MyFilms.conf.StrIndex = MyFilms.r.Length - 1;

      if (MyFilms.conf.StrIndex == -1)
      {
        Action actionType = new MediaPortal.GUI.Library.Action
          {
            wID = MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU
          };
        base.OnAction(actionType);
        return;
      }
      if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString().Length > 0)
      {
        // int TitlePos = (MyFilms.conf.StrTitleSelect.Length > 0) ? MyFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group
        // MyFilms.conf.StrTIndex = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString().Substring(TitlePos);
        string fullTitle = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
        MyFilms.conf.StrTIndex = (fullTitle.LastIndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) > 0
                                    ? fullTitle.Substring(fullTitle.LastIndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) + 1)
                                    : fullTitle);
        // ((masterTitle.LastIndexOf(conf.TitleDelim, System.StringComparison.Ordinal) > 0) ? masterTitle.Substring(masterTitle.LastIndexOf(conf.TitleDelim, System.StringComparison.Ordinal) + 1) : masterTitle)
        MyFilms.currentMovie.Title = MyFilms.conf.StrTIndex;
      }

      int year;
      Int32.TryParse(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString(), out year);
      MyFilms.currentMovie.Year = year;
      string imdb = "";
      if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"].ToString()))
        imdb = MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"].ToString();
      if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString()) && string.IsNullOrEmpty(imdb))
      {
        string cleanString = MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString();
        var cutText = new Regex("" + @"tt\d{7}" + "");
        var m = cutText.Match(cleanString);
        if (m.Success)
          imdb = m.Value;
      }
      MyFilms.currentMovie.IMDBNumber = imdb;

      // add persons to movie object
      IEnumerable<DbPersonInfo> actors = MyFilms.Search_String_Persons(MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString(), false);
      foreach (DbPersonInfo t in actors)
      {
        MyFilms.currentMovie.Actors.Add(t.Name);
      }
      IEnumerable<DbPersonInfo> producers = MyFilms.Search_String_Persons(MyFilms.r[MyFilms.conf.StrIndex]["Producer"].ToString(), false);
      foreach (DbPersonInfo t in producers)
      {
        MyFilms.currentMovie.Producers.Add(t.Name);
      }
      IEnumerable<DbPersonInfo> directors = MyFilms.Search_String_Persons(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString(), false);
      foreach (DbPersonInfo t in directors)
      {
        MyFilms.currentMovie.Directors.Add(t.Name);
      }
      IEnumerable<DbPersonInfo> writers = MyFilms.Search_String_Persons(MyFilms.r[MyFilms.conf.StrIndex]["Writer"].ToString(), false);
      foreach (DbPersonInfo t in writers)
      {
        MyFilms.currentMovie.Writers.Add(t.Name);
      }

      string file = "";
      string strThumb = MediaPortal.Util.Utils.GetCoverArtName((MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies"), MyFilms.conf.StrTIndex); // cached cover
      if (File.Exists(strThumb))
      {
        file = strThumb;
      }
      else
      {
        if (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Length > 0)
        {
          if (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().IndexOf(":\\", System.StringComparison.Ordinal) == -1 && MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Substring(0, 2) != "\\\\")
            file = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[MyFilms.conf.StrIndex]["Picture"];
          else
            file = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();
        }
        else
          file = string.Empty;
        if (!File.Exists(file) || string.IsNullOrEmpty(file))
          file = MyFilms.conf.DefaultCover;
      }

      setGUIProperty("picture", file);
      MyFilms.currentMovie.Picture = file;
      // if (ImgDetFilm != null) ImgDetFilm.FileName = file;

      // load the rest threaded - logos and fanart might take a bit ...
      new Thread(delegate()
      {
        {
          try
          {
            #region threaded loading of logos, fanart and file availability check ...
            // load detailed DB infos
            Load_Detailed_DB(MyFilms.conf.StrIndex);

            // Logos
            Load_Logos(MyFilms.r[MyFilms.conf.StrIndex]);

            //ImageSwapper backdrop = new ImageSwapper();
            var wfanart = new string[2];
            Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");

            if (ImgFanartDir != null)
            {
              wfanart = Search_Fanart(sTitles.FanartTitle, false, "dir", false, file, string.Empty);
              LogMyFilms.Debug("(afficher_detail): Backdrops-File (dir): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            }
            else
            {
              wfanart = Search_Fanart(sTitles.FanartTitle, false, "file", false, file, string.Empty);
              LogMyFilms.Debug("(afficher_detail): Backdrops-File (file): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            }

            if (wfanart[0] == " ")
            {
              if (ImgFanartDir != null) ImgFanartDir.PreAllocResources();
              // GUIControl.HideControl(GetID, 35);
            }
            else
            {
              // GUIControl.ShowControl(GetID, 35);
              if (wfanart[1] == "dir" && ImgFanartDir != null)
              {
                ImgFanartDir.TexturePath = wfanart[0];
                ImgFanartDir.PreAllocResources();
                ImgFanartDir.AllocResources();
                GUIControl.HideControl(GetID, (int)Controls.CTRL_Fanart);
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_FanartDir);
              }
              else
              {
                if (ImgFanartDir != null)
                {
                  ImgFanartDir.PreAllocResources();
                  GUIControl.HideControl(GetID, (int)Controls.CTRL_FanartDir);
                }
                ImgFanart.SetFileName(wfanart[0]);
                setGUIProperty("currentfanart", wfanart[0]);
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_Fanart);
              }
            }
            MyFilms.currentMovie.Fanart = Search_Fanart(sTitles.FanartTitle, false, "file", false, file, string.Empty)[0];

            if (Helper.FieldIsSet(MyFilms.conf.StrStorage) && checkfileavailability)
            {
              if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Length > 0)
              {
                int at = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().IndexOf(";", 0, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Length, System.StringComparison.Ordinal);
                file = SearchMovie(at == -1
                  ? MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Substring(0, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Length).Trim()
                  : MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Substring(0, at).Trim(), MyFilms.conf.StrDirStor);
              }
              else
                file = "false";
              if (file != "false" && (file.Length > 0))
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_ImgDD);
              else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_ImgDD);
            }
            else
              GUIControl.HideControl(GetID, (int)Controls.CTRL_ImgDD);

            if (MyFilms.conf.StrIndex == StrMax - 1)
            {
              GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnNext);
              GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnLast);
            }
            else
            {
              GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnNext);
              GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnLast);
            }
            if (MyFilms.conf.StrIndex == 0)
            {
              GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnPrior);
              GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnFirst);
            }
            else
            {
              GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnPrior);
              GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnFirst);
            }
            Load_Detailed_DB_PushPersonsToPersonFacade(MyFilms.conf.StrIndex);
            #endregion

            Load_Detailed_DB_PushMovieThumbsToMovieThumbsFacade(MyFilms.conf.StrIndex, true);

          }
          catch (Exception ex)
          {
            LogMyFilms.Debug(ex, "afficher_detail() - error: " + ex.Message);
          }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // Do this after thread finished ...
            if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString().Length > 0)
            {
              //int TitlePos = (MyFilms.conf.StrTitleSelect.Length > 0) ? MyFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group
              //MyFilms.conf.StrTIndex = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString().Substring(TitlePos);
              MyFilms.currentMovie.Title = MyFilms.conf.StrTIndex;
              GUIControl.ShowControl(GetID, (int)Controls.CTRL_Title);
            }
            else
              GUIControl.HideControl(GetID, (int)Controls.CTRL_Title);
            if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
            {
              if ((MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() == MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString()) || (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString().Length == 0))
                GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);
              else
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_OTitle);
            }
            else
              GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

    }

    private void Load_Logos(DataRow row)
    {
      //if ((ImgID2001 != null) && (ImgID2002 != null) && (MyFilms.conf.StrLogos))
      //{
      //if ((ImgID2001.XPosition == ImgID2002.XPosition) && (ImgID2001.YPosition == ImgID2002.YPosition))
      //{
      //    try
      //    {
      //        string wlogos = Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2003", ImgID2001.Height + ImgID2002.Height, ImgID2001.Width + ImgID2002.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID);
      //        setGUIProperty("logos_id2001", wlogos);
      //        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
      //        GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
      //        GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
      //        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
      //    }
      //    catch (Exception e)
      //    {
      //        LogMyFilms.Error("" + e.Message);
      //    }
      //}
      //else
      //    {
      //        try
      //        {
      //            setGUIProperty("logos_id2001", Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID));
      //            //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
      //            //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
      //            GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
      //        }
      //        catch (Exception e)
      //        {
      //            LogMyFilms.Error("" + e.Message);
      //        }
      //        try
      //        {
      //            setGUIProperty("logos_id2002", Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID));
      //            //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
      //            //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
      //        }
      //        catch (Exception e)
      //        {
      //            LogMyFilms.Error("" + e.Message);
      //        }
      //    }
      //}
      //else
      //{
      if ((ImgID2001 != null) && (MyFilms.conf.StrLogos))
      {
        try
        {
          setGUIProperty("logos_id2001", Logos.BuildLogos(row, "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, GetID));
          //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
          //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
          //GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
        }
        catch (Exception e)
        {
          LogMyFilms.Error("" + e.Message);
        }
      }
      else
      {
        clearGUIProperty("logos_id2001");
        //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
        //GUIControl.ShowControl(GetID, (int)Controls.CTRL_Format);
      }
      if ((ImgID2002 != null) && (MyFilms.conf.StrLogos))
      {
        try
        {
          setGUIProperty("logos_id2002", Logos.BuildLogos(row, "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, GetID));
          //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
          //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
        }
        catch (Exception e)
        {
          LogMyFilms.Error("" + e.Message);
        }
      }
      else
        clearGUIProperty("logos_id2002");
      //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
      //}

      if ((ImgID2003 != null) && (MyFilms.conf.StrLogos))
      {
        try
        {
          setGUIProperty("logos_id2003", Logos.BuildLogos(row, "ID2003", ImgID2003.Height, ImgID2003.Width, ImgID2003.XPosition, ImgID2003.YPosition, GetID));
        }
        catch (Exception e)
        {
          LogMyFilms.Error("" + e.Message);
        }
      }
      else
        clearGUIProperty("logos_id2003");

      if ((ImgID2012 != null) && (MyFilms.conf.StrLogos))
      {
        try
        {
          setGUIProperty("logos_id2012", Logos.BuildLogos(row, "ID2012", ImgID2012.Height, ImgID2012.Width, ImgID2012.XPosition, ImgID2012.YPosition, GetID));
        }
        catch (Exception e)
        {
          LogMyFilms.Error("" + e.Message);
        }
      }
      else
        clearGUIProperty("logos_id2012");
    }

    internal static void Load_Detailed_TMDB(GUIListItem item)
    {
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetImagesForTMDB - detected language = '" + language + "'");
      Tmdb api = new Tmdb(MyFilms.TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();
      string wstring = "";
      OnlineMovie movie = item.TVTag as OnlineMovie;

      if (movie == null)
      {
        LogMyFilms.Warn("Load_Detailed_TMDB() - Failed loading details ... now clearing properties ...");
        Init_Detailed_DB(false);
        return;
      }

      #region always clear person properties in film details ...
      clearGUIProperty("person.name.value");
      clearGUIProperty("person.dateofbirth.value");
      clearGUIProperty("person.placeofbirth.value");
      clearGUIProperty("person.biography.value");
      //clearGUIProperty("person.dateofdeath.value");
      //clearGUIProperty("person.placeofdeath.value");
      //clearGUIProperty("person.movies.value");
      //clearGUIProperty("person.lastupdate.value");
      #endregion

      movie.Movie = api.GetMovieInfo(movie.MovieSearchResult.id, language);
      if (string.IsNullOrEmpty(movie.Movie.overview))
        movie.Movie = api.GetMovieInfo(movie.MovieSearchResult.id, null);
      movie.MovieCast = api.GetMovieCast(movie.MovieSearchResult.id);

      setGUIProperty("user.mastertitle.value", movie.Movie.title);
      setGUIProperty("user.secondarytitle.value", movie.Movie.original_title);
      setGUIProperty("db.description.value", movie.Movie.overview);
      setGUIProperty("db.year.value", movie.Movie.release_date);
      setGUIProperty("db.length.value", movie.Movie.runtime.ToString());
      setGUIProperty("user.source.isonline", item.IsRemote ? "unavailable" : "available");
      
      //int trailers = 0;
      //try
      //{
      //  trailers = movie.Trailers.youtube.Count;
      //}
      //catch (Exception) {}
      //setGUIProperty("user.sourcetrailer.isonline", (trailers > 0) ? "available" : "unavailable");

      string trailerstatus = "unknown";
      switch (movie.MovieSearchResult.TrailerStatus)
      {
        case TmdbMovieSearchResult.TrailerState.Unknown:
          trailerstatus = "unknown";
          break;
        case TmdbMovieSearchResult.TrailerState.Local:
        case TmdbMovieSearchResult.TrailerState.LocalAndRemote:
          trailerstatus = "available";
          break;
        case TmdbMovieSearchResult.TrailerState.Remote:
          trailerstatus = "offline";
          break;
        case TmdbMovieSearchResult.TrailerState.None:
          trailerstatus = "unavailable";
          break;
      }
      setGUIProperty("user.sourcetrailer.isonline", trailerstatus);

      wstring = "";
      foreach (MovieGenre genre in movie.Movie.genres)
      {
        if (wstring.Length > 0) wstring += ", ";
        wstring += genre.name;
      }
      SetTmdbProperties("Category", wstring);

      wstring = "";
      foreach (ProductionCountry country in movie.Movie.production_countries)
      {
        if (wstring.Length > 0) wstring += ", ";
        wstring += country.name;
      }
      SetTmdbProperties("Country", wstring);

      wstring = "";
      foreach (Cast cast in movie.MovieCast.cast)
      {
        if (wstring.Length > 0) wstring += ", ";
        wstring += cast.name + " (" + cast.character + ")";
      }
      SetTmdbProperties("Actors", wstring);

      string producer = "";
      string director = "";
      string writer = "";
      foreach (Crew crew in movie.MovieCast.crew)
      {
        switch (crew.department)
        {
          case "Production":
            if (producer.Length > 0) producer += ", ";
            producer += crew.name + " (" + crew.job + ")";
            break;
          case "Directing":
            if (director.Length > 0) director += ", ";
            director += crew.name + " (" + crew.job + ")";
            break;
          case "Writing":
            if (writer.Length > 0) writer += ", ";
            writer += crew.name + " (" + crew.job + ")";
            break;
          case "Sound":
          case "Camera":
            break;
        }
      }
      SetTmdbProperties("Director", director);
      SetTmdbProperties("Producer", producer);
      SetTmdbProperties("Writer", writer);

      wstring = "";
      foreach (Cast cast in movie.MovieCast.cast)
      {
        if (wstring.Length > 0) wstring += ", ";
        wstring += cast.name + " (" + cast.character + ")";
      }
      SetTmdbProperties("Actors", wstring);

      SetTmdbProperties("Rating", Math.Round(movie.Movie.vote_average, 1).ToString("0.0"));

      stopwatch.Stop();
      LogMyFilms.Debug("Load_Detailed_TMDB() - load details finished (" + stopwatch.ElapsedMilliseconds + " ms).");
    }

    private static void SetTmdbProperties(string dbfield, string value)
    {
      setGUIProperty("db." + dbfield.ToLower() + ".value", value);
      
      if (MyFilms.conf.Stritem1.ToLower() == (dbfield.ToLower()))
      {
        setGUIProperty("user.item1.label", MyFilms.conf.Strlabel1);
        if (MyFilms.conf.Stritem1.ToLower() == "date")
          setGUIProperty("user.item1.field", "w" + MyFilms.conf.Stritem1.ToLower());
        else
          setGUIProperty("user.item1.field", MyFilms.conf.Stritem1.ToLower());
        setGUIProperty("user.item1.value", value);
      }
      if (MyFilms.conf.Stritem2.ToLower() == (dbfield.ToLower()))
      {
        setGUIProperty("user.item2.label", MyFilms.conf.Strlabel2);
        if (MyFilms.conf.Stritem2.ToLower() == "date")
          setGUIProperty("user.item2.field", "w" + MyFilms.conf.Stritem2.ToLower());
        else
          setGUIProperty("user.item2.field", MyFilms.conf.Stritem2.ToLower());
        setGUIProperty("user.item2.value", value);
      }
      if (MyFilms.conf.Stritem3.ToLower() == (dbfield.ToLower()))
      {
        setGUIProperty("user.item3.label", MyFilms.conf.Strlabel3);
        if (MyFilms.conf.Stritem3.ToLower() == "date")
          setGUIProperty("user.item3.field", "w" + MyFilms.conf.Stritem3.ToLower());
        else
          setGUIProperty("user.item3.field", MyFilms.conf.Stritem3.ToLower());
        setGUIProperty("user.item3.value", value);
      }
      if (MyFilms.conf.Stritem4.ToLower() == (dbfield.ToLower()))
      {
        setGUIProperty("user.item4.label", MyFilms.conf.Strlabel4);
        if (MyFilms.conf.Stritem4.ToLower() == "date")
          setGUIProperty("user.item4.field", "w" + MyFilms.conf.Stritem4.ToLower());
        else
          setGUIProperty("user.item4.field", MyFilms.conf.Stritem4.ToLower());
        setGUIProperty("user.item4.value", value);
      }
      if (MyFilms.conf.Stritem5.ToLower() == (dbfield.ToLower()))
      {
        setGUIProperty("user.item5.label", MyFilms.conf.Strlabel5);
        if (MyFilms.conf.Stritem5.ToLower() == "date")
          setGUIProperty("user.item5.field", "w" + MyFilms.conf.Stritem5.ToLower());
        else
          setGUIProperty("user.item5.field", MyFilms.conf.Stritem5.ToLower());
        setGUIProperty("user.item5.value", value);
      }
    }

    //-------------------------------------------------------------------------------------------
    //  initialize exported fields to skin as '#myfilms.<ant db column name> 
    //-------------------------------------------------------------------------------------------
    public static void Init_Detailed_DB(bool log)
    {
      LogMyFilms.Debug("Init_Detailed_DB() - log = '" + log + "'");
      using (AntMovieCatalog ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          clearGUIProperty("db." + dc.ColumnName.ToLower() + ".value", log);
        }
      }

      MyFilms.currentMovie.Reset();

      clearGUIProperty("db.calc.format.value", log);
      clearGUIProperty("db.calc.aspectratio.value", log);
      clearGUIProperty("db.calc.imageformat.value", log);

      clearGUIProperty("db.certification.logo.value", log);

      for (var j = 1; j < 7; j++)
      {
        clearGUIProperty("db.actors.actor" + j + ".name", log);
        clearGUIProperty("db.actors.actor" + j + ".role", log);
        clearGUIProperty("db.actors.actor" + j + ".image", log);
      }

      clearGUIProperty("user.mastertitle.label", log);
      clearGUIProperty("user.mastertitle.value", log);
      clearGUIProperty("user.mastertitle.groupname", log);
      clearGUIProperty("user.mastertitle.groupcount", log);
      clearGUIProperty("user.secondarytitle.label", log);
      clearGUIProperty("user.secondarytitle.value", log);
      clearGUIProperty("user.secondarytitle.groupname", log);

      for (var j = 1; j < 6; j++)
      {
        clearGUIProperty("user.item" + j + ".label", log);
        clearGUIProperty("user.item" + j + ".field", log);
        clearGUIProperty("user.item" + j + ".value", log);
      }

      for (var j = 1; j < 7; j++)
      {
        clearGUIProperty("user.detailsitem" + j + ".label", log);
        clearGUIProperty("user.detailsitem" + j + ".field", log);
        clearGUIProperty("user.detailsitem" + j + ".value", log);
      }

      clearGUIProperty("user.source.value", log);
      clearGUIProperty("user.source.filepath", log);
      clearGUIProperty("user.source.filename", log);
      clearGUIProperty("user.source.shortname", log);
      clearGUIProperty("user.source.count", log);
      clearGUIProperty("user.sourcetrailer.value", log);
      clearGUIProperty("user.sourcetrailer.filepath", log);
      clearGUIProperty("user.sourcetrailer.filename", log);
      clearGUIProperty("user.sourcetrailer.shortname", log);
      clearGUIProperty("user.sourcetrailer.count", log);
      clearGUIProperty("user.rating.value", log);
      clearGUIProperty("user.watched.value", log);
      clearGUIProperty("user.watchedcount.value", log);
      clearGUIProperty("user.watcheddate.value", log);
      clearGUIProperty("user.name.value", log);
      clearGUIProperty("user.watchedcountglobal.value", log);
      clearGUIProperty("user.source.isonline", log);
      clearGUIProperty("user.sourcetrailer.isonline", log);

      //Clear person properties
      clearGUIProperty("person.name.value", log);
      clearGUIProperty("person.dateofbirth.value", log);
      clearGUIProperty("person.placeofbirth.value", log);
      clearGUIProperty("person.biography.value", log);
      //clearGUIProperty("person.dateofdeath.value", log);
      //clearGUIProperty("person.placeofdeath.value", log);
      //clearGUIProperty("person.movies.value", log);
      //clearGUIProperty("person.lastupdate.value", log);
    }

    //-------------------------------------------------------------------------------------------
    //  Load detailed db fields : export fields to skin as '#myfilms.<ant db column name> 
    //-------------------------------------------------------------------------------------------
    public static void Load_Detailed_DB(int itemId)
    {
      LogMyFilms.Debug("Load_Detailed_DB() - ItemId: '" + itemId + "'");
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      string wstrformat = "";

      if (MyFilms.r == null || itemId > MyFilms.r.Length - 1)
      {
        LogMyFilms.Warn("Load_Detailed_DB() - Failed loading details - index '" + itemId + "' not within current dataset ... now clearing properties ...");
        Init_Detailed_DB(false);
        return;
      }
      #region always clear person properties in film details ...
      clearGUIProperty("person.name.value");
      clearGUIProperty("person.dateofbirth.value");
      clearGUIProperty("person.placeofbirth.value");
      clearGUIProperty("person.biography.value");
      //clearGUIProperty("person.dateofdeath.value");
      //clearGUIProperty("person.placeofdeath.value");
      //clearGUIProperty("person.movies.value");
      //clearGUIProperty("person.lastupdate.value");
      #endregion

      using (var ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          string wstring = "";
          string wstring2 = "";
          //LogMyFilms.Debug("PropertyManager: Set Properties for DB Column '" + dc.ColumnName + "' - '" + BaseMesFilms.Translate_Column(dc.ColumnName) + "'");

          if (MyFilms.r.Length > itemId && MyFilms.r[itemId][dc.ColumnName] != null) // make sure, it is a valid part of current loaded dataset "r"
          {
            #region set userdefined properties for main screen
            if (MyFilms.conf.Stritem1.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.item1.label", MyFilms.conf.Strlabel1);
              if (MyFilms.conf.Stritem1.ToLower() == "date")
                setGUIProperty("user.item1.field", "w" + MyFilms.conf.Stritem1.ToLower());
              else
                setGUIProperty("user.item1.field", MyFilms.conf.Stritem1.ToLower());
              setGUIProperty("user.item1.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.Stritem2.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.item2.label", MyFilms.conf.Strlabel2);
              if (MyFilms.conf.Stritem2.ToLower() == "date")
                setGUIProperty("user.item2.field", "w" + MyFilms.conf.Stritem2.ToLower());
              else
                setGUIProperty("user.item2.field", MyFilms.conf.Stritem2.ToLower());
              setGUIProperty("user.item2.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.Stritem3.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.item3.label", MyFilms.conf.Strlabel3);
              if (MyFilms.conf.Stritem3.ToLower() == "date")
                setGUIProperty("user.item3.field", "w" + MyFilms.conf.Stritem3.ToLower());
              else
                setGUIProperty("user.item3.field", MyFilms.conf.Stritem3.ToLower());
              setGUIProperty("user.item3.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.Stritem4.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.item4.label", MyFilms.conf.Strlabel4);
              if (MyFilms.conf.Stritem4.ToLower() == "date")
                setGUIProperty("user.item4.field", "w" + MyFilms.conf.Stritem4.ToLower());
              else
                setGUIProperty("user.item4.field", MyFilms.conf.Stritem4.ToLower());
              setGUIProperty("user.item4.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.Stritem5.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.item5.label", MyFilms.conf.Strlabel5);
              if (MyFilms.conf.Stritem5.ToLower() == "date")
                setGUIProperty("user.item5.field", "w" + MyFilms.conf.Stritem5.ToLower());
              else
                setGUIProperty("user.item5.field", MyFilms.conf.Stritem5.ToLower());
              setGUIProperty("user.item5.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            #endregion

            #region set userdefined properties for details screen
            if (MyFilms.conf.StritemDetails1.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.detailsitem1.label", MyFilms.conf.StrlabelDetails1);
              if (MyFilms.conf.StritemDetails1.ToLower() == "date")
                setGUIProperty("user.detailsitem1.field", "w" + MyFilms.conf.StritemDetails1.ToLower());
              else
                setGUIProperty("user.detailsitem1.field", MyFilms.conf.StritemDetails1.ToLower());
              setGUIProperty("user.detailsitem1.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.StritemDetails2.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.detailsitem2.label", MyFilms.conf.StrlabelDetails2);
              if (MyFilms.conf.Stritem2.ToLower() == "date")
                setGUIProperty("user.detailsitem2.field", "w" + MyFilms.conf.StritemDetails2.ToLower());
              else
                setGUIProperty("user.detailsitem2.field", MyFilms.conf.StritemDetails2.ToLower());
              setGUIProperty("user.detailsitem2.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.StritemDetails3.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.detailsitem3.label", MyFilms.conf.StrlabelDetails3);
              if (MyFilms.conf.Stritem3.ToLower() == "date")
                setGUIProperty("user.detailsitem3.field", "w" + MyFilms.conf.StritemDetails3.ToLower());
              else
                setGUIProperty("user.detailsitem3.field", MyFilms.conf.StritemDetails3.ToLower());
              setGUIProperty("user.detailsitem3.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.StritemDetails4.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.detailsitem4.label", MyFilms.conf.StrlabelDetails4);
              if (MyFilms.conf.Stritem4.ToLower() == "date")
                setGUIProperty("user.detailsitem4.field", "w" + MyFilms.conf.StritemDetails4.ToLower());
              else
                setGUIProperty("user.detailsitem4.field", MyFilms.conf.StritemDetails4.ToLower());
              setGUIProperty("user.detailsitem4.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.StritemDetails5.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.detailsitem5.label", MyFilms.conf.StrlabelDetails5);
              if (MyFilms.conf.Stritem5.ToLower() == "date")
                setGUIProperty("user.detailsitem5.field", "w" + MyFilms.conf.StritemDetails5.ToLower());
              else
                setGUIProperty("user.detailsitem5.field", MyFilms.conf.StritemDetails5.ToLower());
              setGUIProperty("user.detailsitem5.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            if (MyFilms.conf.StritemDetails6.ToLower() == (dc.ColumnName.ToLower()))
            {
              setGUIProperty("user.detailsitem6.label", MyFilms.conf.StrlabelDetails6);
              if (MyFilms.conf.Stritem5.ToLower() == "date")
                setGUIProperty("user.detailsitem6.field", "w" + MyFilms.conf.StritemDetails6.ToLower());
              else
                setGUIProperty("user.detailsitem6.field", MyFilms.conf.StritemDetails6.ToLower());
              setGUIProperty("user.detailsitem6.value", MyFilms.r[itemId][dc.ColumnName].ToString());
            }
            #endregion

            #region set userdefined watched and rating field
            if (MyFilms.conf.StrWatchedField.ToLower() == dc.ColumnName.ToLower())
            {
              if (MyFilms.conf.EnhancedWatchedStatusHandling)
              {
                var userData = new MultiUserData(MyFilms.r[itemId][BaseMesFilms.MultiUserStateField].ToString());
                UserState user = userData.GetUserState(MyFilms.conf.StrUserProfileName);
                setGUIProperty("user.watched.value", user.WatchedCount > 0 ? "true" : "");
                setGUIProperty("user.watcheddate.value", (user.WatchedCount > 0 && user.WatchedDate > MultiUserData.NoWatchedDate) ? user.WatchedDate.ToShortDateString() : "");
                setGUIProperty("user.watchedcount.value", user.WatchedCount.ToString());
                setGUIProperty("user.name.value", (user.UserName != MyFilms.DefaultUsername) ? user.UserName : "");
                setGUIProperty("user.watchedcountglobal.value", userData.GetGlobalState().WatchedCount.ToString());
                setGUIProperty("user.rating.value", (user.UserRating > MultiUserData.NoRating) ? Math.Round(user.UserRating, 1).ToString() : "");
              }
              else
              {
                setGUIProperty("user.name.value", "");
                setGUIProperty("user.watched.value", MyFilms.r[itemId][dc.ColumnName].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower() ? "true" : "");
                decimal userRating = MultiUserData.NoRating;
                if (MyFilms.r[itemId]["RatingUser"].ToString().Length > 0)
                {
                  if (!(decimal.TryParse(MyFilms.r[itemId]["RatingUser"].ToString(), out userRating))) userRating = MultiUserData.NoRating;
                }
                setGUIProperty("user.rating.value", (userRating > MultiUserData.NoRating) ? Math.Round(userRating, 1).ToString() : "");
              }
            }
            #endregion

            #region set userdefined source and sourcetrailer
            if (MyFilms.conf.StrStorage.ToLower() == dc.ColumnName.ToLower())
            {
              string sourceFull = MyFilms.r[itemId][dc.ColumnName].ToString();
              string name = "";
              string path = "";
              string longname = "";
              string count = "";
              setGUIProperty("user.source.value", sourceFull);
              MyFilms.currentMovie.File = sourceFull;
              if (!string.IsNullOrEmpty(sourceFull.Trim()))
              {
                string[] split = sourceFull.Trim().Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                count = split.Length.ToString();
                foreach (string s in split.Select(x => x.Trim()).Where(x => x.LastIndexOf("\\", StringComparison.Ordinal) > 0))
                {
                  if (name.Length > 0) name += "; ";
                  name += s.Substring(s.LastIndexOf("\\", System.StringComparison.Ordinal) + 1);
                  if (path.Length > 0) path += "; ";
                  path += s.Substring(0, s.LastIndexOf("\\", System.StringComparison.Ordinal));
                  if (longname.Length > 0) longname += "; ";
                  string temppath = s.Substring(0, s.LastIndexOf("\\", System.StringComparison.Ordinal));
                  longname += (temppath.LastIndexOf("\\", System.StringComparison.Ordinal) > 0) ? temppath.Substring(temppath.LastIndexOf("\\", System.StringComparison.Ordinal) + 1) + "\\" + name : s;
                }
              }
              setGUIProperty("user.source.count", count);
              setGUIProperty("user.source.filepath", path);
              setGUIProperty("user.source.filename", name);
              setGUIProperty("user.source.shortname", longname);
            }

            if (MyFilms.conf.StrStorageTrailer.ToLower() == dc.ColumnName.ToLower())
            {
              string sourceFull = MyFilms.r[itemId][dc.ColumnName].ToString();
              string name = "";
              string path = "";
              string longname = "";
              string count = "";
              setGUIProperty("user.sourcetrailer.value", sourceFull);
              MyFilms.currentMovie.Trailer = sourceFull;
              if (!string.IsNullOrEmpty(sourceFull.Trim()))
              {
                string[] split = sourceFull.Trim().Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                count = split.Length.ToString();
                foreach (string s in split.Select(x => x.Trim()).Where(x => x.LastIndexOf("\\", StringComparison.Ordinal) > 0))
                {
                  if (name.Length > 0) name += "; ";
                  name += s.Substring(s.LastIndexOf("\\", System.StringComparison.Ordinal) + 1);
                  if (path.Length > 0) path += "; ";
                  path += s.Substring(0, s.LastIndexOf("\\", System.StringComparison.Ordinal));
                  if (longname.Length > 0) longname += "; ";
                  string temppath = s.Substring(0, s.LastIndexOf("\\", System.StringComparison.Ordinal));
                  longname += (temppath.LastIndexOf("\\", System.StringComparison.Ordinal) > 0) ? temppath.Substring(temppath.LastIndexOf("\\", System.StringComparison.Ordinal) + 1) + "\\" + name : s;
                }
              }
              setGUIProperty("user.sourcetrailer.count", count);
              setGUIProperty("user.sourcetrailer.filepath", path);
              setGUIProperty("user.sourcetrailer.filename", name);
              setGUIProperty("user.sourcetrailer.shortname", longname);
            }
            #endregion

            #region set all db fields ...
            switch (dc.ColumnName.ToLower())
            {
              case "translatedtitle":
              case "originaltitle":
              case "formattedtitle":
                #region titles
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                  if (MyFilms.r[itemId][dc.ColumnName].ToString().Contains(MyFilms.conf.TitleDelim))
                  {
                    wstring = MyFilms.r[itemId][dc.ColumnName].ToString().Substring(MyFilms.r[itemId][dc.ColumnName].ToString().LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                    wstring2 = MyFilms.r[itemId][dc.ColumnName].ToString().Substring(0, MyFilms.r[itemId][dc.ColumnName].ToString().LastIndexOf(MyFilms.conf.TitleDelim));
                  }
                  else
                  {
                    wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                    wstring2 = "";
                  }

                if (MyFilms.conf.StrTitle1.ToLower() == (dc.ColumnName.ToLower()))
                {
                  setGUIProperty("user.mastertitle.value", wstring);
                  setGUIProperty("user.mastertitle.groupname", wstring2);
                }
                if (MyFilms.conf.StrTitle2.ToLower() == (dc.ColumnName.ToLower()))
                {
                  setGUIProperty("user.secondarytitle.value", wstring);
                  setGUIProperty("user.secondarytitle.groupname", wstring2);
                }
                #endregion
                break;
              case "length":
                #region length
                int length = 0;
                if (MyFilms.r[itemId]["Length"].ToString().Length > 0) wstring = MyFilms.r[itemId]["Length"].ToString();
                setGUIProperty("db.length.value", wstring);
                bool success = int.TryParse(wstring, out length);
                MyFilms.currentMovie.Length = (success) ? length : 0;
                break;
                #endregion
              case "actors":
                #region actors
                if (MyFilms.r[itemId]["Actors"].ToString().Length > 0)
                {
                  wstring = MyFilms.r[itemId]["Actors"].ToString().Replace('|', '\n');
                  wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring));

                  //IEnumerable<DbPersonInfo> persons = MyFilms.Search_String_Persons(wstring, false);
                  //foreach (DbPersonInfo t in persons)
                  //{
                  //  MyFilms.currentMovie.Actors.Add(t.Name);
                  //}
                }
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                Load_Detailed_DB_PushActorsToSkin(wstring);
                break;
                #endregion
              case "director":
              case "producer":
              case "writer":
                //wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                //if (wstring.Length > 0)
                //{
                //  wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring));
                //  IEnumerable<DbPersonInfo> persons = MyFilms.Search_String_Persons(wstring, false);
                //  foreach (DbPersonInfo t in persons)
                //  {
                //    switch (dc.ColumnName.ToLower())
                //    {
                //      case "director":
                //        MyFilms.currentMovie.Directors.Add(t.Name);
                //        break;
                //      case "producer":
                //        MyFilms.currentMovie.Producers.Add(t.Name);
                //        break;
                //      case "writer":
                //        MyFilms.currentMovie.Writers.Add(t.Name);
                //        break;
                //    }
                //  }
                //}
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0 ? MyFilms.r[itemId][dc.ColumnName].ToString() : "");
                break;
              
              case "description":
              case "comments":
                #region description & comment
                if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                {
                  wstring = System.Web.HttpUtility.HtmlEncode(MyFilms.r[itemId][dc.ColumnName].ToString().Replace('’', '\''));
                  wstring = wstring.Replace('|', '\n').Replace('…', '.');
                  wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring));
                }
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                break;
                #endregion
              case "date":
                #region date
                if (MyFilms.r[itemId]["Date"].ToString().Length > 0)
                  wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                break;
                #endregion
              case "videoformat":
                #region videoformat
                if (MyFilms.r[itemId]["VideoFormat"].ToString().Length > 0)
                  wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                wstrformat = "V:" + MyFilms.r[itemId]["VideoFormat"];
                break;
                #endregion
              case "audioformat":
                #region audioformat
                if (MyFilms.r[itemId]["AudioFormat"].ToString().Length > 0)
                {
                  wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                  if (wstrformat.Length > 1)
                    wstrformat = wstrformat + ", A:" + MyFilms.r[itemId]["AudioFormat"];
                  else
                    wstrformat = "A:" + MyFilms.r[itemId]["AudioFormat"];
                }
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                setGUIProperty("db.calc.format" + ".value", wstrformat);
                break;
                #endregion
              case "rating":
              case "ratinguser":
                #region rating
                wstring = "";
                if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                  wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                //try { MyFilms.conf.W_rating = (decimal)MyFilms.r[ItemId][dc.ColumnName]; }
                //catch { MyFilms.conf.W_rating = 0; }
                try
                {
                  wstring = ((decimal)MyFilms.r[itemId][dc.ColumnName]).ToString("0.0");
                }
                catch
                {
                  wstring = "";
                }
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                break;
                #endregion

              // fields to skip (do not publish)
              case "contents_id":
              case "dateadded":
              case "picture":
              case "watched":
                break;
              case "fanart":
                if (MyFilms.currentMovie.Fanart.Length == 0 && MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                  MyFilms.currentMovie.Fanart = MyFilms.r[itemId][dc.ColumnName].ToString();
                break;
              case "imdb_id":
                #region imdb_id
                MyFilms.currentMovie.IMDBNumber = MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0 ? MyFilms.r[itemId][dc.ColumnName].ToString() : "";
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0 ? MyFilms.r[itemId][dc.ColumnName].ToString() : "");
                break;
                #endregion
              case "isonline":
                #region set online status
                if (MyFilms.InitialIsOnlineScan) // if availability scanner did run - either by autostart or manually
                {
                  if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                  {
                    switch (MyFilms.r[itemId][dc.ColumnName].ToString())
                    {
                      case "True":
                        setGUIProperty("user.source.isonline", "available");
                        break;
                      case "False":
                        if (Helper.FieldIsSet(MyFilms.conf.StrStorage)) // if there is source field set
                        {
                          setGUIProperty("user.source.isonline", MyFilms.r[itemId][MyFilms.conf.StrStorage].ToString().Length > 0
                                                                   ? "offline"
                                                                   : "unavailable");
                        }
                        else
                          setGUIProperty("user.source.isonline", "unavailable");
                        break;
                    }
                  }
                  else
                    setGUIProperty("user.source.isonline", "unknown"); // should not happen, if scanner did run ...
                }
                else
                {
                  if (Helper.FieldIsSet(MyFilms.conf.StrStorage)) // if there is source field set
                  {
                    if (MyFilms.r[itemId][MyFilms.conf.StrStorage].ToString().Length > 0) // if there is source info available ...
                    {
                      setGUIProperty("user.source.isonline", MyFilms.conf.ScanMediaOnStart ? "unknown" : "available");
                    }
                    else
                      setGUIProperty("user.source.isonline", "unavailable");
                  }
                  else if (MyFilms.conf.SearchFile.ToLower() == "true" || MyFilms.conf.SearchFile.ToLower() == "yes") // if search is enabled in setup
                    setGUIProperty("user.source.isonline", "unknown");
                  else
                    setGUIProperty("user.source.isonline", "unavailable");
                }
                break;
                #endregion
              case "isonlinetrailer":
                #region set trailer online status
                if (MyFilms.InitialIsOnlineScan)
                {
                  if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                  {
                    if (MyFilms.r[itemId][dc.ColumnName].ToString() == "True")
                      setGUIProperty("user.sourcetrailer.isonline", "available");
                    else
                    {
                      if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
                      {
                        setGUIProperty("user.sourcetrailer.isonline", MyFilms.r[itemId][MyFilms.conf.StrStorageTrailer].ToString().Length > 0
                            ? "offline"
                            : "unavailable");
                      }
                      else
                        setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                    }
                  }
                  else
                    setGUIProperty("user.sourcetrailer.isonline", "unknown"); // should not happen, if scanner did run ...
                }
                else
                {
                  if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
                  {
                    if (MyFilms.r[itemId][MyFilms.conf.StrStorageTrailer].ToString().Length > 0)
                    {
                      setGUIProperty("user.sourcetrailer.isonline", MyFilms.conf.ScanMediaOnStart
                        ? "unknown"
                        : "available");
                    }
                    else
                      setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                  }
                  //else if (MyFilms.conf.SearchFileTrailer.ToLower() == "true" || MyFilms.conf.SearchFileTrailer.ToLower() == "yes") // if search is enabled in setup
                  //  setGUIProperty("user.sourcetrailer.isonline", "unknown");
                  else
                    setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                }
                break;
                #endregion
              case "resolution":
                #region set calculated aspectratio and image format
                string ar = "";
                if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                  try
                  {
                    decimal aspectratio;
                    wstring = MyFilms.r[itemId][dc.ColumnName].ToString();
                    setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                    if (!Decimal.TryParse(MyFilms.r[itemId]["Aspectratio"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out aspectratio)) // if no media info data available, calculate data from video resolution - might not be exact DAR (display aspect ratio) // if (!Decimal.TryParse(MyFilms.r[itemId]["Aspectratio"].ToString().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator), out aspectratio)) // if no media info data available, calculate data from video resolution - might not be exact DAR (display aspect ratio)
                    {
                      string[] arSplit = wstring.Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);
                      aspectratio = Math.Round(decimal.Divide(Convert.ToInt32(arSplit[0]), Convert.ToInt32(arSplit[1])), 2);
                    }
                    //Formats:
                    //1,33 -> 4:3
                    //1,78 -> 16:9 / widescreen
                    //1,85 -> widescreen
                    //2,35+ -> cinemascope
                    if (aspectratio < (decimal)(1.4)) ar = "4:3";
                    else if (aspectratio < (decimal)(1.9)) ar = "16:9";
                    else if (aspectratio >= (decimal)(1.9)) ar = "cinemascope";
                    wstring = aspectratio.ToString(CultureInfo.InstalledUICulture);
                  }
                  catch { LogMyFilms.Info("Error calculating aspectratio !"); }
                setGUIProperty("db.calc.aspectratio.value", wstring);
                setGUIProperty("db.calc.imageformat.value", ar);
                break;
                #endregion
              case "certification":
                #region set cleaned certification for logos
                wstring = MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0 ? MyFilms.r[itemId][dc.ColumnName].ToString() : "";
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                wstring = Regex.Replace(wstring, @"\(|\)", ""); // strip ( and )
                wstring = Regex.Replace(wstring, @"^.*\s", ""); // strip start until forst white char
                setGUIProperty("db.certification.logo.value", wstring.Trim());
                break;
                #endregion
              case "year":
                #region year
                if (MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0)
                {
                  int year = 0;
                  setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[itemId][dc.ColumnName].ToString());
                  Int32.TryParse(MyFilms.r[itemId][dc.ColumnName].ToString(), out year);
                  MyFilms.currentMovie.Year = year;
                }

                else
                {
                  setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", "");
                  MyFilms.currentMovie.Year = 0;
                }
                break;
                #endregion

              default:
                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[itemId][dc.ColumnName].ToString().Length > 0 ? MyFilms.r[itemId][dc.ColumnName].ToString() : "");
                break;

            }
            #endregion
          }
        }
      }
      //// for catalog using "search" instead storage field
      //if (string.IsNullOrEmpty(MyFilms.currentMovie.File)) // use search method only if required...
      //{
      //  if ((MyFilms.conf.SearchFile == "True") || (MyFilms.conf.SearchFile == "yes"))
      //  {
      //    string movieName = MyFilms.r[ItemId][MyFilms.conf.ItemSearchFile].ToString();
      //    movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
      //    if (MyFilms.conf.ItemSearchFile.Length > 0)
      //    {
      //      MyFilms.currentMovie.File = Search_FileName(movieName, MyFilms.conf.StrDirStor).Trim();
      //    }
      //  }
      //}

      stopwatch.Stop();
      LogMyFilms.Debug("Load_Detailed_DB() - load details finished (" + stopwatch.ElapsedMilliseconds + " ms).");
    }

    private static void Load_Detailed_DB_PushActorsToSkin(string personscontent)
    {
      for (int j = 1; j < 7; j++)
      {
        clearGUIProperty("db.actors.actor" + j + ".name");
        clearGUIProperty("db.actors.actor" + j + ".role");
        clearGUIProperty("db.actors.actor" + j + ".image");
      }

      IEnumerable<DbPersonInfo> wTableau = MyFilms.Search_String_Persons(personscontent, false);
      int i = 1;
      foreach (DbPersonInfo t in wTableau)
      {
        string actorname = t.Name;
        string actorrole = t.Job;
        if (i < 7)
        {
          if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
          {
            string personartworkpath = MyFilms.conf.StrPathArtist;
            if (File.Exists(personartworkpath + "\\" + actorname + ".jpg"))
            {
              setGUIProperty("db.actors.actor" + i + ".name", actorname);
              setGUIProperty("db.actors.actor" + i + ".role", actorrole);
              setGUIProperty("db.actors.actor" + i + ".image", personartworkpath + "\\" + actorname + ".jpg");
              i = i + 1;
            }
          }
          else
          {
            setGUIProperty("db.actors.actor" + i + ".name", actorname);
            setGUIProperty("db.actors.actor" + i + ".role", actorrole);
            i = i + 1;
          }
        }
      }
    }

    private void Load_Detailed_DB_PushPersonsToPersonFacade(int index)
    {
      if (facadePersons == null || index > MyFilms.r.Length - 1) return;

      string personscontent = MyFilms.r[index]["Actors"].ToString();
      facadePersons.CurrentLayout = GUIFacadeControl.Layout.AlbumView;
      GUIControl.ClearControl(GetID, facadePersons.GetID);

      string personartworkpath = MyFilms.conf.StrPathArtist;
      IEnumerable<DbPersonInfo> wTableau = MyFilms.Search_String_Persons(personscontent, false);
      foreach (DbPersonInfo actor in wTableau)
      {
        var item = new GUIListItem();
        item.Label = actor.Name;
        item.Label2 = actor.Job;
        item.Label3 = actor.Name + " (" + actor.Job + ")";
        if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
        {
          if (File.Exists(personartworkpath + "\\" + actor.Name + ".jpg"))
          {
            item.ThumbnailImage = personartworkpath + "\\" + actor.Name + ".jpg";
          }
          else
          {
            item.ThumbnailImage = MyFilms.conf.DefaultCoverArtist;
          }
          item.IconImage = item.ThumbnailImage;
          item.IconImageBig = item.ThumbnailImage;
        }
        facadePersons.Add(item);
      }
    }

    private void Load_Detailed_DB_PushMovieThumbsToMovieThumbsFacade(int index, bool createmissingmoviethumbs)
    {
      if (index > MyFilms.r.Length - 1 || ((facadeMovieThumbs == null || facadeMovieThumbs.ThumbnailLayout != null) && ImgMovieThumbsDir == null)) return;

      if (facadeMovieThumbs != null && facadeMovieThumbs.ThumbnailLayout != null) 
        GUIControl.ClearControl(GetID, facadeMovieThumbs.GetID);

      if (ImgMovieThumbsDir != null)
      {
        if (!MovieThumbsExist(index))
        {
          SetMovieThumbDummyControl(false);
        }
      }

      List<string> movieThumbs = SearchMovieThumbs(index, createmissingmoviethumbs);

      if (facadeMovieThumbs != null && facadeMovieThumbs.ThumbnailLayout != null)
      {
        foreach (var item in movieThumbs.Select(movieThumb => new GUIListItem { IconImageBig = movieThumb }))
        {
          facadeMovieThumbs.Add(item);
        }
      }

      if (ImgMovieThumbsDir != null)
      {
        if (movieThumbs.Count > 0)
        {
          ImgMovieThumbsDir.TexturePath = (movieThumbs[0].LastIndexOf("\\", StringComparison.Ordinal) > 0) ? movieThumbs[0].Substring(0, movieThumbs[0].LastIndexOf("\\", StringComparison.Ordinal)) : movieThumbs[0];
          ImgMovieThumbsDir.PreAllocResources();
          ImgMovieThumbsDir.AllocResources();
          SetMovieThumbDummyControl(true);
        }
        else
        {
          SetMovieThumbDummyControl(false);
          // ImgMovieThumbsDir.PreAllocResources();
        }
      }
    }

    private void SetMovieThumbDummyControl(bool visible)
    {
      if (dummyFacadeMovieThumbsAvailable != null)
      {
        if (visible)
        {
          if (!dummyFacadeMovieThumbsAvailable.Visible) dummyFacadeMovieThumbsAvailable.Visible = true;
        }
        else
        {
          if (dummyFacadeMovieThumbsAvailable.Visible) dummyFacadeMovieThumbsAvailable.Visible = false;
        }
      }
    }

    //-------------------------------------------------------------------------------------------
    //  Load detailed infos for persons: export fields to skin as '#myfilms.<ant db column name> 
    //-------------------------------------------------------------------------------------------
    public static void Load_Detailed_PersonInfo(string personname, GUIListItem item)
    {
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      LogMyFilms.Debug("Load_Detailed_PersonInfo for '" + personname + "'");
      IMDBActor person = null;

      if (null != item && item.MusicTag != null)
      {
        LogMyFilms.Debug("Load_Detailed_PersonInfo() - got cached person info from facade object (" + stopwatch.ElapsedMilliseconds + " ms)");
        person = (IMDBActor)item.MusicTag;
      }
      else
      {
        var actorList = new ArrayList();
        VideoDatabase.GetActorByName(personname, actorList);
        LogMyFilms.Debug("Load_Detailed_PersonInfo() - got '" + actorList.Count + "' results (" + stopwatch.ElapsedMilliseconds + " ms)");
        if (actorList.Count > 0 && actorList.Count < 3) // Do not proceed, of none or too many results !
        {
          string[] strActor = actorList[0].ToString().Split(new char[] { '|' });
          int actorId = (strActor[0].Length > 0) ? Convert.ToInt32(strActor[0]) : 0;
          // string actorname = strActor[1];

          if (actorId > 0)
          {
            LogMyFilms.Debug("load details for actor ID: '" + actorId.ToString() + "'");
            try
            {
              person = VideoDatabase.GetActorInfo(actorId);
            }
            catch (Exception ex)
            {
              person = null;
              LogMyFilms.Debug("Load_Detailed_PersonInfo() - Exception while loading person details: " + ex.Message);
            }
            finally
            {
              // cleanup here ...
            }
          }
        }
      }
      string value;

      value = (person != null && person.Name.Length > 0) ? person.Name : personname;
      setGUIProperty("person.name.value", value);
      //setGUIProperty("user.mastertitle.value", value);
      //setGUIProperty("user.secondarytitle.value", value);

      value = (person != null && person.Biography.Length > 0) ? person.Biography : "";
      setGUIProperty("person.biography.value", value);
      //setGUIProperty("db.description.value", value);

      value = (person != null && person.DateOfBirth.Length > 0) ? person.DateOfBirth : "";
      setGUIProperty("person.dateofbirth.value", value);
      //setGUIProperty("db.year.value", value);

      value = (person != null && person.PlaceOfBirth.Length > 0) ? person.PlaceOfBirth : "";
      setGUIProperty("person.placeofbirth.value", value);
      //setGUIProperty("db.category.value", value);

#if MP13
          value = (person != null && person.DateOfDeath.Length > 0) ? person.DateOfDeath : "";
          setGUIProperty("person.dateofdeath.value", value);

          value = (person != null && person.PlaceOfDeath.Length > 0) ? person.PlaceOfDeath : "";
          setGUIProperty("person.placeofdeath.value", value);

          value = (person != null && person.LastUpdate.Length > 0) ? person.LastUpdate : "";
          setGUIProperty("person.lastupdate.value", value);

          setGUIProperty("person.movies.value", value);
#endif

      stopwatch.Stop();
      LogMyFilms.Debug("Load_Detailed_PersonInfo() - load details finished (" + stopwatch.ElapsedMilliseconds + " ms).");
    }
    #endregion

    private void InitTrailerwatcher()
    {
      string directorypath = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
      if (directorypath.Contains(";")) directorypath = directorypath.Substring(0, directorypath.IndexOf(";"));
      if (directorypath.Contains("\\")) directorypath = directorypath.Substring(0, directorypath.LastIndexOf("\\"));

      if (!Directory.Exists(directorypath))
      {
        LogMyFilms.Warn("InitTrailerwatcher() - Trailerwatcher cannot initialize - path does not exist: '" + directorypath + "'");
        return;
      }
      if (Trailerwatcher.EnableRaisingEvents && Trailerwatcher.Path == directorypath)
        return; // return, if it's already enabled and file name has not changed
      else
      {
        Trailerwatcher.EnableRaisingEvents = false;
        Trailerwatcher.Changed -= new FileSystemEventHandler(TrailerwatcherChanged);
        Trailerwatcher.Error -= new ErrorEventHandler(TrailerwatcherError);
        Trailerwatcher.Created -= new FileSystemEventHandler(TrailerwatcherCreated);
        Trailerwatcher.Deleted -= new FileSystemEventHandler(TrailerwatcherDeleted);
        Trailerwatcher.Renamed -= new RenamedEventHandler(TrailerwatcherRenamed);
        if (directorypath == "") return;
      }

      // Init FileSystem Watcher
      // ***** Change this as required
      //string path = System.IO.Path.GetDirectoryName(conf.StrFileXml);
      //string filename = System.IO.Path.GetFileName(conf.StrFileXml);
      Trailerwatcher.Path = directorypath;
      Trailerwatcher.IncludeSubdirectories = true;
      Trailerwatcher.Filter = "*.*"; // Trailerwatcher.Filter = "*.xml";
      Trailerwatcher.NotifyFilter = NotifyFilters.LastWrite; // Trailerwatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
      //Trailerwatcher.InternalBufferSize = 64;

      // Add event handlers: 1 for the event raised when a file is created, and 1 for when it detects an error.
      Trailerwatcher.Changed += new FileSystemEventHandler(TrailerwatcherChanged);
      Trailerwatcher.Error += new ErrorEventHandler(TrailerwatcherError);
      //Trailerwatcher.Created += new FileSystemEventHandler(TrailerwatcherCreated);
      //Trailerwatcher.Deleted += new FileSystemEventHandler(TrailerwatcherDeleted);
      //Trailerwatcher.Renamed += new RenamedEventHandler(TrailerwatcherRenamed);

      Trailerwatcher.EnableRaisingEvents = true; // Begin watching.
      LogMyFilms.Debug("InitTrailerwatcher() - Trailerwatcher started watching - directory: '" + directorypath + "'");
    }

    private void DeInitTrailerwatcher()
    {
      Trailerwatcher.EnableRaisingEvents = false;
      Trailerwatcher.Changed -= new FileSystemEventHandler(TrailerwatcherChanged);
      Trailerwatcher.Error -= new ErrorEventHandler(TrailerwatcherError);
      Trailerwatcher.Created -= new FileSystemEventHandler(TrailerwatcherCreated);
      Trailerwatcher.Deleted -= new FileSystemEventHandler(TrailerwatcherDeleted);
      Trailerwatcher.Renamed -= new RenamedEventHandler(TrailerwatcherRenamed);
      LogMyFilms.Debug("DeInitTrailerwatcher() - disabled trailer watcher");
    }

    private void TrailerwatcherChanged(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("TrailerwatcherChanged() - New Trailerwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

      if (Trailerwatcher.EnableRaisingEvents == false) // ignore event, if notification is switched off
        return;

      Thread.Sleep(250);
      var objFileInfo = new FileInfo(e.FullPath);
      if (!objFileInfo.Exists) return; // ignore the file changed event

      // Trailerwatcher.EnableRaisingEvents = false;
      DeInitTrailerwatcher(); // reset trailer watcher
      AutoRegisterTrailer(e.FullPath);

      //// this.BeginInvoke(new UpdateWatchTextDelegate(UpdateWatchText), "WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");
      //Trailerwatcher.EnableRaisingEvents = true;
      //if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
      //{
      //  ImportComplete();
      //  LogMyFilms.Debug("FSwatcherChanged(): Fired 'ImportCompleted' event to trigger sync to trakt page after reloading database content !");
      //}
    }

    private void TrailerwatcherCreated(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("WatcherCreated() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
    }

    private void TrailerwatcherDeleted(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("WatcherDeleted() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
    }

    private void TrailerwatcherRenamed(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("WatcherRenamed() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
    }

    private void TrailerwatcherError(object source, ErrorEventArgs e) // The error event handler
    {
      Exception watchException = e.GetException();
      LogMyFilms.Debug("WatcherError() - A FileSystemWatcher error has occurred: " + watchException.Message);
      // We need to create new version of the object because the old one is now corrupted
      Trailerwatcher = new FileSystemWatcher();
      while (!Trailerwatcher.EnableRaisingEvents)
      {
        try
        {
          InitTrailerwatcher(); // This will throw an error at the watcher.NotifyFilter line if it can't get the path.
          LogMyFilms.Debug("WatcherError() - Trailerwatcher restarted after error !");
        }
        catch
        {
          Thread.Sleep(5000); // Sleep for a bit; otherwise, it takes a bit of processor time
        }
      }
    }

    #region  Lecture du film demandé

    public static void PlayMovie(string config, int movieid)
    //-------------------------------------------------------------------------------------------
    // Play Movie from external (wrapper to prepare environment)
    //-------------------------------------------------------------------------------------------
    {
      int getId = GUIWindowManager.GetPreviousActiveWindow();
      try
      {
        Launch_Movie(movieid, getId, null, MyFilmsDetail.PlayerOption.Internal);
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex, "PlayMovie() : Exception !");
        // throw;
      }
    }

    public static void Launch_Movie(int select_item, int GetID, GUIAnimation m_SearchAnimation, PlayerOption playeroption)
    //-------------------------------------------------------------------------------------------
    // Play Movie
    //-------------------------------------------------------------------------------------------
    {
      LogMyFilms.Debug("Launch_Movie() select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "', playeroption = '" + Enum.GetName(typeof(PlayerOption), playeroption) + "'");
      //enableNativeAutoplay(); // in case, other plugin disabled it - removed, as we now do start external player ourselves ...

      #region Version Select Dialog
      string filestorage = MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString();
      if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
      {
        Regex filmver = new Regex(@"\[\[([^\#]*)##([^\]]*)\]\]");
        MatchCollection filmverMatches = filmver.Matches(filestorage);
        if (filmverMatches.Count > 0)
        {
          GUIDialogMenu versionmenu =
            (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          versionmenu.Reset();
          versionmenu.SetHeading("Select Version");
          List<string> filestr = new List<string>();
          for (int i = 0; i < filmverMatches.Count; i++)
          {
            versionmenu.Add(filmverMatches[i].Groups[2].Value);
            filestr.Add(filmverMatches[i].Groups[1].Value);
          }
          versionmenu.DoModal(GetID);
          if (versionmenu.SelectedLabel == -1) return;
          filestorage = filestr[versionmenu.SelectedLabel];
        }
      }

      #endregion

      #region handle WOL
      // Guzzi: Added WOL to start remote host before playing the files
      // Wake up the TV server, if required
      // HandleWakeUpNas();
      // LogMyFilms.Info("Launched HandleWakeUpNas() to start movie'" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "'");

      if (MyFilms.conf.StrCheckWOLenable)
      {
        WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
        int wTimeout = MyFilms.conf.StrWOLtimeout;
        bool isActive;
        string UNCpath = filestorage;
        string NasServerName;
        string NasMACAddress;

        if (UNCpath.StartsWith("\\\\")) UNCpath = (UNCpath.Substring(2, UNCpath.Substring(2).IndexOf("\\") + 0)).ToLower();
        if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) ||
            (UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) ||
            (UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)))
        {
          isActive = WakeOnLanManager.Ping(UNCpath, wTimeout);

          if (!isActive) // Only if NAS server is not yet already rzunning !
          {
            if (MyFilms.conf.StrCheckWOLuserdialog)
            {
              if (!(GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(107986), "Film    : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "'" + "\n" + "Server  : '" + UNCpath + "'" + "\n" + "Status  : '" + GUILocalizeStrings.Get(10798742)))) // srv name + " - (offline) - start ?"
                return;
            }

            // Search the NAS where movie is located:
            if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) &&
                (MyFilms.conf.StrNasMAC1.Length > 1))
            {
              NasServerName = MyFilms.conf.StrNasName1;
              NasMACAddress = MyFilms.conf.StrNasMAC1;
            }
            else if ((UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) &&
                     (MyFilms.conf.StrNasMAC2.Length > 1))
            {
              NasServerName = MyFilms.conf.StrNasName2;
              NasMACAddress = MyFilms.conf.StrNasMAC2;
            }
            else if ((UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)) &&
                     (MyFilms.conf.StrNasMAC3.Length > 1))
            {
              NasServerName = MyFilms.conf.StrNasName3;
              NasMACAddress = MyFilms.conf.StrNasMAC3;
            }
            else
            {
              NasServerName = String.Empty;
              NasMACAddress = String.Empty;
            }

            // Start NAS Server

            bool successFulStart = wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(NasMACAddress), NasServerName, wTimeout);

            if (MyFilms.conf.StrCheckWOLuserdialog)
            {
              if (successFulStart)
                GUIUtils.ShowNotifyDialog("'" + NasServerName + "' " + GUILocalizeStrings.Get(10798743)); //successfully started 
              else
                GUIUtils.ShowOKDialog("'" + NasServerName + "' " + GUILocalizeStrings.Get(10798744)); // could not be started 
            }
          }
        }
        else
        {
          GUIUtils.ShowOKDialog("", ("Server '" + UNCpath + "' " + GUILocalizeStrings.Get(10798746)), GUILocalizeStrings.Get(10798747), ""); // Automatic NAS start not possible ... 
          return;
        }
      }

      #endregion

      SetProcessAnimationStatus(true, m_SearchAnimation);
      #region Run externaly Program before Playing if defined in setup
      if (Helper.FieldIsSet(MyFilms.conf.CmdPar))
        RunProgram(MyFilms.conf.CmdExe, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.CmdPar].ToString());
      #endregion

      if (g_Player.Playing) g_Player.Stop();

      // search all files
      var newItems = new ArrayList();
      bool noResumeMovie = true;
      int movieIndex = 0;

      SearchAllFiles(MyFilms.r[select_item], false, ref noResumeMovie, ref newItems, ref movieIndex, false, filestorage);
      
      //  ToDo: add code to call population of trailers array and insert num of configured trailers into playlist
      // newItems.Insert(0, <trailers>);
      
      if (newItems.Count > 20) // Maximum 20 entries (limitation for MP dialogFileStacking)
      {
        GUIUtils.ShowOKDialog(MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString(), "maximum 20 entries for the playlist", "", "");
        LogMyFilms.Info("Too many entries found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "', number of entries found = " + newItems.Count);
        return;
      }
      SetProcessAnimationStatus(false, m_SearchAnimation);

      #region optional part selection dialog
      if (newItems.Count > 1)
      {
        if (noResumeMovie)
        {
          var dlg = (GUIDialogFileStacking)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILESTACKING);
          if (null != dlg)
          {
            dlg.SetNumberOfFiles(newItems.Count);
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            int selectedFileIndex = dlg.SelectedFile;
            if (selectedFileIndex < 1) return;
            movieIndex = selectedFileIndex++;
          }
        }
      }
      #endregion

      if (newItems.Count > 0)
      {
        // Check, if the content returned is a BR playlist to supress internal player and dialogs
        bool isBRcontent = false;
        string mediapath = filestorage;
        if (newItems[0].ToString().ToLower().EndsWith("bdmv")) isBRcontent = true;
        bool isMpVideo = Utils.IsVideo(newItems[0].ToString());
        LogMyFilms.Info("Launch_Movie() - starting playback: isBRcontent = '" + isBRcontent + "', isMpVideo = '" + isMpVideo + "', #items = '" + newItems.Count + "', first item = '" + newItems[0] + "'), filestorage = '" + filestorage + "'");

        switch (playeroption)
        {
          case PlayerOption.External:
            try
            {
              LogMyFilms.Info("Launch_Movie() - start external player - path = '" + MyFilms.conf.ExternalPlayerPath + "', argument (filestorage) = '" + filestorage + "'");
              LaunchExternalPlayer(filestorage);
            } catch (Exception ex) { LogMyFilms.Info("Launch_Movie() - calling external player ended with exception: " + ex); }
            break;
          case PlayerOption.BluRayPlayerLauncher:
            LogMyFilms.Info("Launch_Movie() - activate blurayplayer plugin");
            GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.BluRayPlayerLauncher);
            break;
          case PlayerOption.Internal:
            bool externalplayerextensiondetected = MyFilms.conf.ExternalPlayerExtensions.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(s => filestorage.ToLower().Contains(s.ToLower()));
            if (externalplayerextensiondetected && MyFilms.conf.ExternalPlayerPath.Length > 0) // check, if this is configured to use external player via Extensions in setup
            {
              #region external player playback (myfilms)
              LogMyFilms.Info("Launch_Movie() - extension for external player detected! - start external player - path = '" + MyFilms.conf.ExternalPlayerPath + "', argument (filestorage) = '" + filestorage + "'");
              try
              {
                LaunchExternalPlayer(filestorage);
              } catch (Exception ex) { LogMyFilms.Info("Launch_Movie() - calling external player ended with exception: " + ex); }
              #endregion
            }
            else if (isMpVideo || (isBRcontent && Helper.IsBdHandlerAvailableAndEnabled)) // use internal playback, if suppoerted or BD handler is installed
            {
              #region internal playback
              LogMyFilms.Info("Launch_Movie() - start internal playback");
              playlistPlayer.Reset();
              playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
              PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
              playlist.Clear();

              foreach (object t in newItems)
              {
                var movieFileName = (string)t;
                // ToDo: Check, if that is necessary - from MP1.3.x to get e.g. Mediainfo registered AddFileToDatabase(movieFileName);
                var newitem = new PlayListItem();
                newitem.FileName = movieFileName;
                newitem.Type = PlayListItem.PlayListItemType.Video;
                playlist.Add(newitem);
              }
              // ask for start movie Index

              MyFilms.conf.MyFilmsPlaybackActive = true; // Set Playbackhandler to active
              PlayMovieFromPlayList(noResumeMovie, movieIndex - 1); // play movie...
              #endregion
            }
            else if (isBRcontent && Helper.IsBluRayPlayerLauncherAvailableAndEnabled) // use BRplayerLauncher fir BR content, if installed
            {
              #region  BluRayPlayerLauncher playback
              LogMyFilms.Info("Launch_Movie() - activate blurayplayer plugin");
              GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.BluRayPlayerLauncher);
              #endregion
            }
            else // playback not successful - notify user!
            {
              #region no playback option successful - notify user
              LogMyFilms.Info("Launch_Movie() - Internal Playback not successful - playback not started!");
              GUIUtils.ShowOKDialog("No playback possible, please check your setup. \nInstall BR Player Launcher, BDhandler or external player for unsupported media.");
              #endregion
            }
            break;
        }
      }
      else
      {
        //if (first)
        //// ask for mounting file first time
        //{
        var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
        dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986)); //my films
        dlgYesNo.SetLine(1, GUILocalizeStrings.Get(219)); //no disc
        if (Helper.FieldIsSet(MyFilms.conf.StrIdentItem))
          if (MyFilms.conf.StrIdentLabel.Length > 0)
            dlgYesNo.SetLine(2, MyFilms.conf.StrIdentLabel + " = " + MyFilms.r[select_item][MyFilms.conf.StrIdentItem]); //Label Identification for Media
          else
            dlgYesNo.SetLine(2, "'" + MyFilms.conf.StrIdentItem + "' = " + MyFilms.r[select_item][MyFilms.conf.StrIdentItem]); //ANT Item Identification for Media
        else
          dlgYesNo.SetLine(2, "' disc n° = " + MyFilms.r[select_item]["Number"]); //ANT Number for Identification Media 
        dlgYesNo.DoModal(GetID);
        if (dlgYesNo.IsConfirmed) 
          Launch_Movie(select_item, GetID, m_SearchAnimation, playeroption);
        //}
        else
        {
          GUIUtils.ShowOKDialog("", GUILocalizeStrings.Get(1036), MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString(), ""); //no video found
          LogMyFilms.Info("File not found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "'");
        }
      }
    }

    internal static void Launch_Movie_Trailer(int selectItem, int GetID, GUIAnimation m_SearchAnimation)
    //-------------------------------------------------------------------------------------------
    // Play Movie Trailers !!!
    //-------------------------------------------------------------------------------------------
    {
      // Run externaly Program before Playing if defined in setup
      SetProcessAnimationStatus(true, m_SearchAnimation);
      LogMyFilms.Debug("(Play Movie Trailer) selectItem = '" + selectItem + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");

      if (Helper.FieldIsSet(MyFilms.conf.CmdPar)) RunProgram(MyFilms.conf.CmdExe, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.CmdPar].ToString());

      if (g_Player.Playing) g_Player.Stop();
      // search all files
      var newItems = new ArrayList();
      int movieIndex = 0;
      bool noResumeMovie = true;

      LogMyFilms.Debug("MyFilmsDetails (Launch_Movie_Trailer): new do Moviesearch with '" + selectItem + "' (Selected_Item");
      //Change back, if method in original properly adapted with bool Trailer
      SearchAllFiles(MyFilms.r[selectItem], false, ref noResumeMovie, ref newItems, ref movieIndex, true, "");
      LogMyFilms.Debug("MyFilmsDetails (Launch_Movie_Trailer): newItems.Count: '" + newItems.Count + "'");
      if (newItems.Count > 20)
      // Maximum 20 entries (limitation for MP dialogFileStacking)
      {
        GUIUtils.ShowOKDialog("", MyFilms.r[selectItem][MyFilms.conf.StrSTitle].ToString(), "maximum 20 entries for the playlist", "");
        LogMyFilms.Info("Too many entries found for movie '" + MyFilms.r[selectItem][MyFilms.conf.StrSTitle] + "', number of entries found = " + newItems.Count);
        return;
      }
      SetProcessAnimationStatus(false, m_SearchAnimation);
      if (newItems.Count > 1)
      {
        var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgmenu.Reset();
        dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...
        dlgmenu.Add(GUILocalizeStrings.Get(10798740)); // play all trailers 

        foreach (object t in newItems)
        {
          var movieFileName = (string)t;
          long wsize = File.Exists(movieFileName) ? new FileInfo(movieFileName).Length : 0;
          string wsizeformatted = string.Format("{0} MB", wsize / 1048576);
          if (movieFileName.Contains("\\"))
            dlgmenu.Add(movieFileName.Substring(movieFileName.LastIndexOf("\\", StringComparison.Ordinal) + 1) + " (" + wsizeformatted + ")"); // add moviename to menu
          else
            dlgmenu.Add(movieFileName + " (" + wsizeformatted + ")"); // add moviename to menu
        }

        dlgmenu.DoModal(GetID);
        if (dlgmenu.SelectedLabel == -1) return;
        //if (dlgmenu.SelectedLabel > 0) // 0 = Play all trailers - so >0 -> a specific choice has been taken ...
        //IMovieIndex = dlgmenu.SelectedId; // is "1" - as "0" is used for play all movies ...
        movieIndex = dlgmenu.SelectedLabel;

        //GUIDialogFileStacking dlg = (GUIDialogFileStacking)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILESTACKING);
        //if (null != dlg)
        //{
        //    dlg.SetNumberOfFiles(newItems.Count);
        //    dlg.DoModal(GUIWindowManager.ActiveWindow);
        //    int selectedFileIndex = dlg.SelectedFile;
        //    if (selectedFileIndex < 1) return;
        //    IMovieIndex = selectedFileIndex++;
        //}
      }
      if (newItems.Count > 0)
      {
        playlistPlayer.Reset();
        playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
        PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
        playlist.Clear();

        if (movieIndex > 0)
        {
          var movieFileName = (string)newItems[movieIndex - 1]; // as in menu there is "all-option" as first index 0 ...
          var newitem = new PlayListItem();
          newitem.FileName = movieFileName;
          LogMyFilms.Info("Play specific movie trailer: '" + movieFileName + "'");
          newitem.Type = PlayListItem.PlayListItemType.Video;
          playlist.Add(newitem);
        }
        else // if play all trailers is chosen add all available trailers to playlist
        {
          foreach (object t in newItems)
          {
            var movieFileName = (string)t;
            var newitem = new PlayListItem();
            newitem.FileName = movieFileName;
            LogMyFilms.Info("Add trailer to playlist: '" + movieFileName + "'");
            newitem.Type = PlayListItem.PlayListItemType.Video;
            playlist.Add(newitem);
          }
        }
        // play movie...
        //PlayMovieFromPlayListTrailer(NoResumeMovie, IMovieIndex - 1);
        PlayMovieFromPlayListTrailer(0); // always start with first trailer ...
      }
      else
      {
        LogMyFilms.Info("File not found for movie '" + MyFilms.r[selectItem][MyFilms.conf.StrSTitle] + "'");
        bool tryOther = false;
        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
        dlgYesNo.Reset();
        dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986) + " " + MyFilms.r[selectItem][MyFilms.conf.StrSTitle]);
        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798737));
        dlgYesNo.SetLine(3, GUILocalizeStrings.Get(10798738));
        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
        if (dlgYesNo.IsConfirmed) tryOther = true;

        if (tryOther) //if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(107986) + " " + MyFilms.r[selectItem][MyFilms.conf.StrSTitle], GUILocalizeStrings.Get(10798737) + "\n" + GUILocalizeStrings.Get(10798738))) //my films & Titel //no video found locally // Try Youtube?
        {
          var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          var choiceViewMenu = new List<string>();
          dlgmenu.Reset();
          choiceViewMenu.Clear();
          dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...

          dlgmenu.Add(GUILocalizeStrings.Get(10798711)); //search youtube trailer with onlinevideos
          choiceViewMenu.Add("playtraileronlinevideos");
          dlgmenu.Add(GUILocalizeStrings.Get(10798712)); //search apple itunes trailer with onlinevideos 10798716
          choiceViewMenu.Add("playtraileronlinevideosappleitunes");
          dlgmenu.Add(GUILocalizeStrings.Get(10798716)); //search IMDB trailer with onlinevideos
          choiceViewMenu.Add("playtraileronlinevideosimdbtrailer");
          dlgmenu.Add(GUILocalizeStrings.Get(712) + " ..."); //Return ...
          choiceViewMenu.Add("return");

          dlgmenu.DoModal(GetID);
          if (dlgmenu.SelectedLabel == -1) return;

          string site;
          string titleextension = string.Empty;
          string path = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
          if (path.Contains(";"))
            path = path.Substring(0, path.IndexOf(";", StringComparison.Ordinal));
          if (path.Contains("\\"))
            path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));

          switch (choiceViewMenu[dlgmenu.SelectedLabel].ToLower())
          {
            case "playtraileronlinevideos":
              site = "YouTube";
              titleextension = " " + MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer" + ((MyFilms.conf.GrabberOverrideLanguage.Length > 0) ? (" " + MyFilms.conf.GrabberOverrideLanguage) : "");
              break;
            case "playtraileronlinevideosappleitunes":
              site = "iTunes Movie Trailers";
              break;
            case "playtraileronlinevideosimdbtrailer":
              site = "IMDb Movie Trailers";
              break;
            default:
              return;
          }
          // Load OnlineVideo Plugin with Searchparameters for YouTube and movie to Search ... OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>
          if (Helper.IsOnlineVideosAvailableAndEnabled)
          {
            string title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
            string oVstartparams = "site:" + site + "|category:|search:" + title + titleextension + "|return:Locked" + "|downloaddir:" + path + "|downloadmenuentry:" + GUILocalizeStrings.Get(10798749) + " (" + title + ")";
            //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", site);
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", title + titleextension);
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloaddir", path);
            //GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadfilename", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadmenuentry", GUILocalizeStrings.Get(10798749) + " (" + title + ")"); // download to movie directory
            LogMyFilms.Debug("Starting OnlineVideos with '" + oVstartparams + "'");
            // should this be set here to make original movie doesn't get set to watched??
            // trailerPlayed = true;

            GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.OnlineVideos, oVstartparams);

            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloaddir", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadfilename", "");
            GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadmenuentry", "");
          }
          else
          {
            // GUIUtils.ShowNotifyDialog("OnlineVideo plugin not installed or wrong version");
            GUIUtils.ShowOKDialog("OnlineVideo plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.OnlineVideos), "", "");
          }
        }
      }
    }

    public static void Launch_Movie_Trailer_Scrobbling(ArrayList newItems, int GetID) // newitems contains filenames ...
    //-------------------------------------------------------------------------------------------
    // Play Movie Trailers for Scrobbling !!!
    //-------------------------------------------------------------------------------------------
    {
      if (g_Player.Playing) g_Player.Stop();

      if (newItems.Count > 20)
      // Maximum 20 entries (limitation for MP dialogFileStacking)
      {
        LogMyFilms.Info("Too many entries found !', number of entries = " + newItems.Count);
        return;
      }
      if (newItems.Count > 0)
      {
        playlistPlayer.Reset();
        playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
        PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
        playlist.Clear();

        foreach (object t in newItems)
        {
          var movieFileName = (string)t;
          var newitem = new PlayListItem();
          newitem.FileName = movieFileName;
          LogMyFilms.Info("Add trailer to playlist: '" + movieFileName + "'");
          newitem.Type = PlayListItem.PlayListItemType.Video;
          playlist.Add(newitem);
        }
        MyFilms.trailerscrobbleactive = true; // needs to be set active again here
        // set OSD values
        try
        {
          SetDelayedGuiPropertiesNowPlayingTrailer(MyFilms.currentTrailerPlayingItem);
        }
        catch (Exception ex)
        {
          LogMyFilms.Info("Error setting OSD properties: " + ex.StackTrace);
        }
        PlayMovieFromPlayListTrailer(0);
      }
    }

    /// <summary>
    /// Adds file (full path) and it's stacked parts (method will search
    /// for them inside strFile folder) into videodatabase (movie table)
    /// This method will also fill mediaInfo data and file duration id
    /// if they are not exists.
    /// </summary>
    /// <param name="strFile"></param>
    /// <returns></returns>
    private static ArrayList AddFileToDatabase(string strFile)
    {
      var files = new ArrayList();
      // Stop is file is not video
      if (!Utils.IsVideo(strFile))
      {
        return files;
      }
      // ??
      if (PlayListFactory.IsPlayList(strFile))
      {
        return files;
      }
      // Don't add web streams (e.g.: from Online videos)
      if (strFile.StartsWith("http:"))
      {
        return files;
      }

      var allFiles = new ArrayList();

      // If this is database movie, get files and return
      if (VideoDatabase.HasMovieInfo(strFile))
      {
        IMDBMovie movie = new IMDBMovie();
        VideoDatabase.GetMovieInfo(strFile, ref movie);
        GUIUtils.GetFilesForMovie(movie.ID, ref allFiles); // replace MP13 call : VideoDatabase.GetFilesForMovie(movie.ID, ref allFiles);

        // Check if we have mediainfo for files and add it if necessary
#if MP13
        foreach (string file in allFiles)
        {
          if (!VideoDatabase.HasMediaInfo(file))
          {
            VideoDatabase.AddMovieFile(file);
          }
        }
#endif
        return allFiles;
      }
      // If file is not scanned (movie is not in the db ie. user video)
      int movieId = VideoDatabase.AddMovieFile(strFile);
      files.Add(strFile);

      // Check if file is stackable and get rest of the stack
#if MP13
      if (IsFileStackable(strFile))
      {
        List<GUIListItem> items = _virtualDirectory.GetDirectoryUnProtectedExt(_currentFolder, true);

        for (int i = 0; i < items.Count; ++i)
        {
          GUIListItem temporaryListItem = (GUIListItem)items[i];

          if (temporaryListItem.IsFolder)
          {
            if (!IsMovieFolder(temporaryListItem.Path))
            {
              continue;
            }
            else
            {
              temporaryListItem.IsBdDvdFolder = true;
            }
          }

          IMDBMovie movie = (IMDBMovie)temporaryListItem.AlbumInfoTag;

          if (temporaryListItem.AlbumInfoTag == null)
          {
            IMDBMovie.SetMovieData(temporaryListItem);
            movie = temporaryListItem.AlbumInfoTag as IMDBMovie;
          }

          if (movie == null)
          {
            continue;
          }

          if (movie.VideoFileName != strFile)
          {
            // Add rest of stackable items for main file (main file is already in array)
            if (!temporaryListItem.IsBdDvdFolder)
            {
              if (Util.Utils.ShouldStack(movie.VideoFileName, strFile))
              {
                allFiles.Add(items[i]);
              }
            }
            else
            {
              string strPath, strFileName;
              Util.Utils.Split(strFile, out strPath, out strFileName);

              if (Util.Utils.PathShouldStack(movie.VideoFilePath, strPath))
              {
                allFiles.Add(items[i]);
              }
            }
          }
        }

      // Add rest of the stackable files
        foreach (GUIListItem item in allFiles)
        {
          IMDBMovie movie = (IMDBMovie)item.AlbumInfoTag;
          int idFile = VideoDatabase.GetFileId(movie.VideoFileName);

          if (idFile == -1)
          {
            string path, filename;
            DatabaseUtility.Split(movie.VideoFileName, out path, out filename);
            int pathId = VideoDatabase.AddPath(path);
            VideoDatabase.AddFile(movieId, pathId, filename);
          }

          files.Add(movie.VideoFileName);
        }
      }
#endif

      return files;
    }

    protected static void Search_parts(string fileName, DataRow row, bool delete, ref bool noResumeMovie, ref ArrayList newItems, ref int idMovie, ref int movieIndex, ref int timeMovieStopped)
    {
      // if filename already in arraylist, return
      // search other parts belonging to the same movie
      var dirsInf = new DirectoryInfo(fileName.Substring(0, fileName.LastIndexOf("\\", StringComparison.Ordinal)));
      FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
      foreach (string wfile in sfiles.Select(sfi => dirsInf.FullName + "\\" + sfi.Name))
      {
        if (newItems.Contains(wfile))
          continue;
        if (MediaPortal.Util.Utils.ShouldStack(fileName, wfile) && MediaPortal.Util.Utils.IsVideo(wfile))
        {
          if (delete)
          {
            delete_movie(wfile, ref newItems);
          }
          else
          {
            idMovie = -1;
            add_update_movie(wfile, row, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
          }
        }
      }
    }

    protected static void delete_movie(string fileName, ref ArrayList newItems)
    {
      VideoDatabase.DeleteMovie(fileName); // Remove file from videodatabase, if any.
      newItems.Add(fileName);
    }
    protected static void add_update_movie(string fileName, DataRow row, ref bool noResumeMovie, ref ArrayList newItems, ref int idMovie, ref int movieIndex, ref int timeMovieStopped)
    {
      // if filename already in arraylist, return
      if (newItems.Contains(fileName))
        return;
      // search infos in the Video Database
      var movieDetails = new IMDBMovie();
      VideoDatabase.GetMovieInfo(fileName, ref movieDetails);
      if (idMovie == -1)
        idMovie = VideoDatabase.GetMovieId(fileName);
      int idFile = VideoDatabase.GetFileId(fileName);
      if (idMovie >= 0 && idFile >= 0)
      {
        //  movie database find=> ask for resume movie if any 
        byte[] resumeData;
        timeMovieStopped = VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
        if (timeMovieStopped > 0)
        {
          string title = Path.GetFileName(fileName);
          if (movieDetails.Title != String.Empty) title = movieDetails.Title;
          string mfTitle = row[MyFilms.conf.StrTitle1].ToString();
          if (mfTitle.Contains("\\")) mfTitle = mfTitle.Substring(mfTitle.LastIndexOf("\\") + 1);
          if (mfTitle != String.Empty) title = mfTitle;

          if (noResumeMovie)
          {
            // if (!(GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(900), title + "\n" + GUILocalizeStrings.Get(936) + MediaPortal.Util.Utils.SecondsToHMSString(timeMovieStopped) + "\n" + fileName))) //resume movie?
            var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (null == dlgYesNo) return;
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
            dlgYesNo.SetLine(1, title);
            dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + MediaPortal.Util.Utils.SecondsToHMSString(timeMovieStopped));
            dlgYesNo.SetLine(3, fileName);
            dlgYesNo.SetDefaultToYes(true);
            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
            if (!dlgYesNo.IsConfirmed)            {
              VideoDatabase.DeleteMovieStopTime(idFile);
              timeMovieStopped = 0;
            }
            else
            {
              movieIndex = newItems.Count + 1;
              noResumeMovie = false;
            }
          }
        }
      }
      // update the MP Video Database for OSD view during playing
      // UpdateDatabase(fileName, row, -1); // disabled, as we do not want to add/update VDB anymore from MyFilms
      newItems.Add(fileName);
    }

    bool PlayBackEventIsOfConcern(g_Player.MediaType type, string filename)
    {
      if (string.IsNullOrEmpty(filename)) return false;

      if (MyFilms.conf != null && MyFilms.currentMovie != null && type == g_Player.MediaType.Video && (MyFilms.currentMovie.File.Contains(filename) || MyFilms.conf.MyFilmsPlaybackActive))
        return true;
      
      // check, if an inserted disc matches the current movie for DVDs
      try
      {
        if (g_Player.IsDVD && MyFilms.conf != null && MyFilms.r != null && MyFilms.conf.StrIndex < MyFilms.r.Length)
        {
          string currentDiscId = Utility.GetDiscIdString(filename);
          string catalogDiscId = MyFilms.r[MyFilms.conf.StrIndex]["MediaLabel"].ToString();
          LogMyFilms.Debug("PlayBackEventIsOfConcern: check for matching discIDs: - currentDiscId = '{0}',  catalogDiscId (MediaLabel) = '{1}'", currentDiscId, catalogDiscId);
          if (!string.IsNullOrEmpty(catalogDiscId) && currentDiscId == catalogDiscId)
            return true;
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("PlayBackEventIsOfConcern: error checking discID: '{0}',  '{1}'", ex.Message, ex.StackTrace);
        return false;
      }
      return false;
    }

    bool PlayBackEventIsOfConcernAsTrailer(g_Player.MediaType type, string filename)
    {
      if (string.IsNullOrEmpty(filename)) 
        return false;

      try
      {
        if (MyFilms.conf != null && MyFilms.currentTrailerPlayingItem != null && type == g_Player.MediaType.Video && MyFilms.currentTrailerPlayingItem.Trailer.Contains(filename)) // if (MyFilms.currentMovie != null && type == g_Player.MediaType.Video && MyFilms.currentMovie.Trailer.Contains(filename)) // 
          return true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("PlayBackEventIsOfConcernAsTrailer: error : '{0}',  '{1}'", ex.Message, ex.StackTrace);
        return false;
      }

      return false;
    }

    private void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
    {
      if (!PlayBackEventIsOfConcern(type, filename))
      {
        if (PlayBackEventIsOfConcernAsTrailer(type, filename))
        {
          LogMyFilms.Debug("OnPlayBackEnded was initiated, identified that MyFilms Trailer was played - filename: '" + filename + "'");
          GUIWindowManager.OnNewAction -= new OnActionHandler(GUIWindowManager_OnNewAction); // make sure it doesn't register twice ....
          GUIWindowManager.OnNewAction += new OnActionHandler(GUIWindowManager_OnNewAction);
        }
        else
          LogMyFilms.Debug("OnPlayBackStarted was initiated, but has no relevant event data for MyFilms - filename: '" + filename + "'");
        return;
      }
      LogMyFilms.Debug("OnPlayBackStarted was initiated - filename: '" + filename + "'");

      // indicate to skin, that it is MyFilms playing a file
      setGUIProperty("isplaying", "true", true);
      // attach to global action event, to handle remote keys during playback - e.g. trailer previews
      GUIWindowManager.OnNewAction -= new OnActionHandler(GUIWindowManager_OnNewAction); // make sure it doesn't register twice ....
      GUIWindowManager.OnNewAction += new OnActionHandler(GUIWindowManager_OnNewAction);

      // tell any listeners that user started the movie
      if (MovieStarted != null && MyFilms.conf.AllowTraktSync)
      {
        MFMovie movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
        MovieStarted(movie);
        LogMyFilms.Debug("OnPlayBackStarted(): Fired 'MovieStarted' event with movie = '" + movie.Title + "'");
      }

      MyFilms.conf.StrPlayedRow = MyFilms.r[MyFilms.conf.StrIndex];
      string otitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
      string ttitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();

      LogMyFilms.Debug("OnPlayBackStarted() was initiated - MyFilms.conf.StrIndex = '" + MyFilms.conf.StrIndex + "', count = '" + MyFilms.r.Length + "', otitle = '" + otitle + "', ttitle = '" + ttitle + "'");
      
      int idFile = VideoDatabase.GetFileId(filename);
      if (idFile != -1)
      {
        int movieDuration = (int)g_Player.Duration;
        VideoDatabase.SetMovieDuration(idFile, movieDuration);
      }

      SetDelayedGuiProperties_NowPlaying(MyFilms.conf.StrPlayedRow);

      // Might require delay to wait until OSD is first (auto)updated from MyVideo database - we might want to override this after ...  
      // GUIPropertyManager.SetProperty("#Play.Current.Thumb", clear ? " " : osdImage);
      // Check, if property for OSD should be set (came from myvideo DB in the past) -> #Play.Current.Thumb
    }

    private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
    {
      if (!PlayBackEventIsOfConcern(type, filename))
      {
        if (PlayBackEventIsOfConcernAsTrailer(type, filename))
        {
          LogMyFilms.Debug("OnPlayBackEnded was initiated, identified that MyFilms Trailer was played - filename: '" + filename + "'");
          if (TrailerEnded != null)
          {
            TrailerEnded(filename); // LogMyFilms.Debug("OnPlayBackEnded(): Fired 'TrailerEnded' event with filename = '" + filename + "'");
          }
        }
        else LogMyFilms.Debug("OnPlayBackEnded was initiated, but has no relevant movie event data for MyFilms - filename: '" + filename + "'");
        return;
      }
      LogMyFilms.Debug("OnPlayBackEnded was initiated - filename: '" + filename + "'");
      UpdateOnPlayEnd(type, 0, filename, true, false);
    }

    private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
    {
      if (!PlayBackEventIsOfConcern(type, filename))
      {
        LogMyFilms.Debug("OnPlayBackStopped was initiated, but has no relevant movie event data for MyFilms - filename: '" + filename + "'");
        MyFilms.trailerscrobbleactive = false;
        return;
      }
      LogMyFilms.Debug("OnPlayBackStopped was initiated - filename: '" + filename + "'");
      UpdateOnPlayEnd(type, timeMovieStopped, filename, false, true);
    }

    private void UpdateOnPlayEnd(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended, bool stopped)
    {
      LogMyFilms.Debug("UpdateOnPlayEnd() was initiated - ended = '" + ended + "', stopped = '" + stopped + "', trailerPlayed = '" + trailerPlayed + "', filename: '" + filename + "'");

      // indicate to skin, that MyFilms isn't playing anymore
      setGUIProperty("isplaying", "false", true);
      MyFilms.conf.MyFilmsPlaybackActive = false;

      // detach from global action event, to handle remote keys during playback - e.g. trailer previews
      try { GUIWindowManager.OnNewAction -= new OnActionHandler(GUIWindowManager_OnNewAction); }
      catch (Exception) { }

      if (MyFilms.conf.StrPlayedRow == null) return;
      // if (MyFilms.conf.StrPlayedIndex == -1) return;
      if (type != g_Player.MediaType.Video || filename.EndsWith("&txe=.wmv")) return;
      if (HandleTrailer()) return;

      try
      {
        PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
        PlayListItem listItem = playlist.LastOrDefault();
        bool isLastPart = (listItem == null) || (filename == listItem.FileName);

        string otitle = MyFilms.conf.StrPlayedRow["OriginalTitle"].ToString();
        string ttitle = MyFilms.conf.StrPlayedRow["TranslatedTitle"].ToString();
        LogMyFilms.Debug("UpdateOnPlayEnd() was initiated - isLastPart = '" + isLastPart + "',  = '" + otitle + "', ttitle = '" + ttitle + "'");

        // Handle all movie files from idMovie
        var movies = new ArrayList();
        int playTimePercentage = 0; // Set watched flag after 80% of total played time

        int iidMovie = VideoDatabase.GetMovieId(filename);
        if (iidMovie >= 0)
        {
          #region get files from myvideo DB and update if necessary
          //#if MP1X
          //                    VideoDatabase.GetFiles(iidMovie, ref movies);
          //#else
          //                    VideoDatabase.GetFilesForMovie(iidMovie, ref movies);
          //#endif

          GUIUtils.GetFilesForMovie(iidMovie, ref movies);

          if (movies.Count <= 0)
          {
            LogMyFilms.Warn("GetFilesForMovie() - no movie files found in MyVideoDB for iidMovie = '" + iidMovie + "', filename = '" + filename + "'");            
            return;
          }

          #region disabled code
          //HashSet<string> watchedMovies = new HashSet<string>();

          //try
          //{
          //  if (file != string.Empty)
          //  {
          //    // Set new data
          //    MediaInfoWrapper x = new MediaInfoWrapper(file);
          //    x.VideoDuration
          //    //GUIPropertyManager.SetProperty("#VideoCodec", Util.Utils.MakeFileName(x.VideoCodec));
          //    //GUIPropertyManager.SetProperty("#VideoResolution", x.VideoResolution);
          //    //GUIPropertyManager.SetProperty("#AudioCodec", Util.Utils.MakeFileName(x.AudioCodec));
          //    //GUIPropertyManager.SetProperty("#AudioChannels", x.AudioChannelsFriendly);
          //    //GUIPropertyManager.SetProperty("#HasSubtitles", x.HasSubtitles.ToString());
          //    //GUIPropertyManager.SetProperty("#AspectRatio", x.AspectRatio);
          //  }
          //}
          //catch (Exception)
          //{
          //  throw;
          //}
          #endregion

          #region get runtime from MyFilms DB
          string runtimeFromDb = MyFilms.conf.StrPlayedRow["Length"].ToString();
          double totalRuntimeMovie;
          try
          {
            totalRuntimeMovie = 60 * double.Parse(runtimeFromDb);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("TotalRuntimeMovie - Parsing runtime from catalog failed: " + ex.Message);
            totalRuntimeMovie = 0.0;
          }
          LogMyFilms.Debug("TotalRuntimeMovie = '" + totalRuntimeMovie.ToString() + "', Runtime from DB = '" + runtimeFromDb + "', timeMovieStopped = '" + timeMovieStopped + "'");
          #endregion

          if (g_Player.Player.Duration >= 1)
          {
            int duration = 0;
            foreach (PlayListItem playListItem in playlist)
            {
              if (filename != playListItem.FileName)
              {
                int fileId = VideoDatabase.GetFileId(playListItem.FileName);
                duration += VideoDatabase.GetMovieDuration(fileId);
                continue;
              }
              break;
            }

            LogMyFilms.Debug("TotalRuntimeMovie = '" + totalRuntimeMovie + "', g_player.Player.Duration = '" + g_Player.Player.Duration + "', Duration of former playlist items = '" + duration.ToString() + "'");
            if (totalRuntimeMovie > g_Player.Player.Duration)
              playTimePercentage = (int)Math.Ceiling(((duration + timeMovieStopped) / totalRuntimeMovie) * 100);
            else
              playTimePercentage = (int)Math.Ceiling(((duration + timeMovieStopped) / g_Player.Player.Duration) * 100);
            LogMyFilms.Debug("Calculated playtimepercentage: '" + playTimePercentage + "' - g_player.Duration: '" + g_Player.Duration + "' - playlistPlayer.g_Player.Duration: '" + playlistPlayer.g_Player.Duration.ToString() + "'");
          }

          foreach (object moviepart in movies)
          {
            string strFilePath = (string)moviepart;
            int idFile = VideoDatabase.GetFileId(strFilePath);
            byte[] resumeData;
            if (idFile < 0) break;

            if (g_Player.IsDVDMenu)
            {
              VideoDatabase.SetMovieStopTimeAndResumeData(idFile, 0, null);
              // watchedMovies.Add(strFilePath);
            }
            else if (filename.Trim().ToLower().Equals(strFilePath.Trim().ToLower()) && timeMovieStopped > 0)
            {
              g_Player.Player.GetResumeState(out resumeData);
              LogMyFilms.Info("GUIVideoFiles: {0} idFile={1} timeMovieStopped={2} resumeData={3}", "MyFilms", idFile, timeMovieStopped, resumeData);
              VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
              LogMyFilms.Debug("GUIVideoFiles: {0} store resume time", "MyFilms");

              if (playTimePercentage >= 80) //Set file "watched" only if 80% or higher played time (share view)
              {
                //watchedMovies.Add(strFilePath);
              }
            }
            else
            {
              VideoDatabase.DeleteMovieStopTime(idFile);
            }
            if (ended)
            {
              // Set resumedata to zero
              VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
              VideoDatabase.SetMovieStopTimeAndResumeData(idFile, 0, resumeData);
              LogMyFilms.Info("GUIVideoFiles: OnPlayBackEnded idFile={0} resumeData={1}", idFile, resumeData);
            }
            else
            {
              // ToDo: Activate and modify code to set watched earlier than "ended" ...
              // Watched status for videos not in the movie database (sample code from MyVideos)
              //int playTimePercentage = 0; // Set watched flag after 80% of total played time
              //if (g_Player.Player.Duration >= 1)
              //  playTimePercentage = (int)Math.Ceiling((timeMovieStopped / g_Player.Player.Duration) * 100);

              if (filename == strFilePath && timeMovieStopped > 0)
              {
                g_Player.Player.GetResumeState(out resumeData);
                LogMyFilms.Info("GUIVideoFiles: OnPlayBackStopped idFile={0} timeMovieStopped={1} resumeData={2}", idFile, timeMovieStopped, resumeData);
                VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
                LogMyFilms.Debug("GUIVideoFiles: OnPlayBackStopped store resume time");
              }
              else
                VideoDatabase.DeleteMovieStopTime(idFile);
            }
          }

          //if (ended || (stopped && MyFilms.conf.CheckWatchedPlayerStopped))
          //{
          //  // Update db view watched status for played movie
          //  var details = new IMDBMovie();
          //  VideoDatabase.GetMovieInfo(filename, ref details);
          //  //VideoDatabase.GetMovieInfoById(iidMovie, ref details);
          //  if (!details.IsEmpty && (playTimePercentage >= 80 || g_Player.IsDVDMenu)) //Flag movie "watched" status only if 80% or higher played time (database view)
          //  {
          //    details.Watched = 1;
          //    //details.Watched++;
          //    VideoDatabase.SetWatched(details);
          //    //VideoDatabase.SetMovieInfoById(details.ID, ref details);
          //  }
          //}
          #endregion
        }

        if (trailerPlayed)
        {
          LogMyFilms.Debug("Skipping UpdateOnEnd - reason: trailerPlayed = true");
          trailerPlayed = false;
          return;
        }

        if (MyFilms.conf.CheckWatched || (MyFilms.conf.CheckWatchedPlayerStopped && ((ended && isLastPart) || (stopped && (playTimePercentage >= 80 || g_Player.IsDVDMenu)))))
        {
          #region update watched status
          if (MyFilms.conf.EnhancedWatchedStatusHandling)
          {
            AddWatchedCount(MyFilms.conf.StrPlayedRow, MyFilms.conf.StrUserProfileName);
          }
          else
          {
            MyFilms.conf.StrPlayedRow[MyFilms.conf.StrWatchedField] = "True";
          }
          LogMyFilms.Debug("Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie end'");
          #endregion
        }

        #region tell any listeners that movie is watched
        MFMovie movie = GetMovieFromRecord(MyFilms.conf.StrPlayedRow); // create movie before DB record is deleted ...
        if ((ended && isLastPart) || (stopped && (playTimePercentage >= 80 || g_Player.IsDVDMenu)))
        {
          if (MovieWatched != null && MyFilms.conf.AllowTraktSync)
          {
            MovieWatched(movie);
            LogMyFilms.Debug("UpdateOnPlayEnd(): Fired 'MovieWatched' event with movie = '" + movie.Title + "'");
          }
        }
        else
        {
          if (MovieStopped != null && MyFilms.conf.AllowTraktSync)
          {
            MovieStopped(movie);
            LogMyFilms.Debug("UpdateOnPlayEnd(): Fired 'MovieStopped' event with movie = '" + movie.Title + "'");
          }
        }
        #endregion

        if (ended)
        {
          #region do automatic updates & deletions & update on userdefined field with userdefined value
          if (MyFilms.conf.StrSuppressPlayStopUpdateUserField)
          {
            MyFilms.conf.StrPlayedRow[MyFilms.conf.StrSuppressField] = MyFilms.conf.StrSuppressValue;
            LogMyFilms.Info("Database movie updated for playbackstopped : " + MyFilms.conf.StrPlayedRow[MyFilms.conf.StrTitle1]);
          }
          if (MyFilms.conf.StrSuppressAutomatic)
          {
            SuppressEntry(MyFilms.conf.StrPlayedRow);
          }
          #endregion
        }

        if (MyFilms.conf.CheckWatched || MyFilms.conf.CheckWatchedPlayerStopped || MyFilms.conf.StrSuppressPlayStopUpdateUserField || MyFilms.conf.StrSuppressAutomatic)
        {
          Update_XML_database();
          if (GetID == MyFilms.ID_MyFilmsDetail)  // only update GUI, when currently active
            afficher_detail(true);
        }

        MyFilms.conf.StrPlayedRow = null;
      }
      catch (Exception ex)
      {
        LogMyFilms.Info("Error during PlayBackEnded - exception: " + ex.Message + ", stacktrace: " + ex.StackTrace);
      }
    }

    private void SetDelayedGuiProperties_NowPlaying(DataRow rowPlaying)
    {
      // start a thread that will set the properties in 2 seconds (otherwise MediaPortal core logic would overwrite them)
      if (rowPlaying == null) return;
      new Thread(delegate(object o)
      {
        try
        {
          var movie = o as DataRow;
          // string alternativeTitle = video["TranslatedTitle"].ToString();

          #region Cover
          string pictureFile = string.Empty;
          if (movie["Picture"].ToString().Length > 0)
          {
            if ((movie["Picture"].ToString().IndexOf(":\\", System.StringComparison.Ordinal) == -1) && (movie["Picture"].ToString().Substring(0, 2) != "\\\\"))
              pictureFile = MyFilms.conf.StrPathImg + "\\" + movie["Picture"];
            else
              pictureFile = movie["Picture"].ToString();
          }
          if (string.IsNullOrEmpty(pictureFile) || !File.Exists(pictureFile))
            pictureFile = MyFilms.conf.DefaultCover;
          #endregion

          Thread.Sleep(2000);

          string titleToShow = movie[MyFilms.conf.StrTitle1].ToString();
          if (titleToShow.Contains("\\")) titleToShow = titleToShow.Substring(titleToShow.LastIndexOf("\\") + 1); // strip group names ...

          LogMyFilms.Debug("Setting Video Properties for '{0}'", titleToShow);

          if (!string.IsNullOrEmpty(titleToShow)) GUIPropertyManager.SetProperty("#Play.Current.Title", titleToShow);
          if (!string.IsNullOrEmpty(movie["Description"].ToString())) GUIPropertyManager.SetProperty("#Play.Current.Plot", movie["Description"].ToString());
          if (!string.IsNullOrEmpty(movie["Year"].ToString())) GUIPropertyManager.SetProperty("#Play.Current.Year", movie["Year"].ToString());

          if (!string.IsNullOrEmpty(pictureFile)) GUIPropertyManager.SetProperty("#Play.Current.Thumb", pictureFile);
        }
        catch (Exception ex)
        {
          LogMyFilms.Warn("Error setting playing video properties: {0}", ex.ToString());
        }
      }) { IsBackground = true, Name = "MyFilmsSetNowPlayingProperties" }.Start(rowPlaying);
    }

    private static void SetDelayedGuiPropertiesNowPlayingTrailer(MFMovie rowPlaying)
    {
      // start a thread that will set the properties in 2 seconds (otherwise MediaPortal core logic would overwrite them)
      if (rowPlaying == null) return;
      new Thread(delegate(object o)
      {
        try
        {
          MFMovie movie = o as MFMovie;
          string pictureFile = Helper.PicturePath(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString(), MyFilms.conf.StrPathImg, MyFilms.conf.DefaultCover);

          Thread.Sleep(3000);
          string titleToShow = Helper.TitleWithoutGroupName(rowPlaying.TranslatedTitle) + " (" + Helper.TitleWithoutGroupName(rowPlaying.Title) + ") - " + rowPlaying.Year.ToString();
          LogMyFilms.Debug("Setting Video Properties for '{0}'", titleToShow);

          if (!string.IsNullOrEmpty(titleToShow)) GUIPropertyManager.SetProperty("#Play.Current.Title", "Trailer: " + titleToShow);
          if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString())) GUIPropertyManager.SetProperty("#Play.Current.Plot", MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString());
          if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString())) GUIPropertyManager.SetProperty("#Play.Current.Year", MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString());
          if (!string.IsNullOrEmpty(pictureFile)) GUIPropertyManager.SetProperty("#Play.Current.Thumb", pictureFile);

          Thread.Sleep(1000); // make sure, refreshrate notification is over ...
          GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798986), titleToShow); // MyFilms Movie Preview
        }
        catch (Exception ex)
        {
          LogMyFilms.Warn("Error setting playing video properties: {0}", ex.ToString());
        }
      }) { IsBackground = true, Name = "MyFilmsSetNowPlayingProperties" }.Start(rowPlaying);
    }

    private void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
    {
      LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Action detected - '" + action.wID + "'");
      LogMyFilms.Debug("GUIWindowManager_OnNewAction(): trailerscrobbleactive = '" + MyFilms.trailerscrobbleactive + "'");
      switch (action.wID)
      {
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_RECORD:
          {
            if (!MyFilms.trailerscrobbleactive)
            {
              int duration = (int)g_Player.Duration;
              int currentposition = (int)g_Player.CurrentPosition;
              string file = g_Player.CurrentFile;
              string title = g_Player.currentTitle;
              LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Movie Action: Get Fanart Snapshot from current position (position = '" + currentposition + "', duration = '" + duration + "')");

              // if (!(GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(1079847), GUILocalizeStrings.Get(1079852) + " ?"))) // MyFilms Fanart Creator // Create 'snapshot' fanart from current playback position
              //   return;
              var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
              dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079847)); // MyFilms Fanart Creator
              dlgYesNo.SetLine(1, ""); // dlgYesNo.SetLine(1, "-> " + currentposition + " s.");
              dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079852) + " ?"); // Create 'snapshot' fanart from current playback position
              dlgYesNo.SetLine(3, "");
              // dlgYesNo.SetNoLabel("Cancel");
              dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
              //dlgYesNo.DoModal(GetID);
              if (!(dlgYesNo.IsConfirmed)) return;
              Menu_CreateFanart_OnMoviePosition_Parameterized(duration, currentposition, file, title);
            }
            break;
          }
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM:
          {
            if (MyFilms.trailerscrobbleactive)
            {
              LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Trailer Action: Play main movie ?");
              var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
              dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798981)); // Trailer Scrobbling ...
              dlgYesNo.SetLine(1, GUILocalizeStrings.Get(10798985)); // Do you want to play the main movie ?
              dlgYesNo.SetLine(2, "'" + MyFilms.currentTrailerPlayingItem.TranslatedTitle + "'");
              dlgYesNo.SetLine(3, "(" + MyFilms.currentTrailerPlayingItem.Title + ") - " + MyFilms.currentTrailerPlayingItem.Year.ToString());
              dlgYesNo.TimeOut = 10;
              //dlgYesNo.SetYesLabel("Options");
              //dlgYesNo.SetNoLabel("Next Trailer");
              dlgYesNo.SetDefaultToYes(false);
              dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
              if (dlgYesNo.IsConfirmed) // if (GUIUtils.ShowCustomYesNoDialog(GUILocalizeStrings.Get(10798981), GUILocalizeStrings.Get(10798985) + "\n" + "'" + MyFilms.currentTrailerPlayingItem.TranslatedTitle + "'" + "\n" + "(" + MyFilms.currentTrailerPlayingItem.Title + ") - " + MyFilms.currentTrailerPlayingItem.Year.ToString(), null, null, false, 10, wGetID)) // Trailer Scrobbling ... // Do you want to play the main movie ?
              {
                Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.Internal);
              }
              else
                dlgYesNo.DeInit();
            }
            break;
          }
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
          {
            if (MyFilms.trailerscrobbleactive)
            {
              LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Trailer Action: Show Options menu");
              // ToDo: Show Options menu
            }
            break;
          }
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_CHAPTER:
          {
            if (MyFilms.trailerscrobbleactive)
            {
              // ToDo: Play next Trailer
              var rnd = new Random();
              Int32 randomNumber = rnd.Next(MyFilms.currentTrailerMoviesList.Count);
              LogMyFilms.Debug("RandomNumber: '" + randomNumber + "', Record: '" + MyFilms.currentTrailerMoviesList[randomNumber].ID + "', RandomTitle: '" + MyFilms.currentTrailerMoviesList[randomNumber].Title + "'");

              var trailer = new ArrayList { MyFilms.currentTrailerMoviesList[randomNumber].File };
              MyFilms.currentTrailerPlayingItem = MyFilms.currentTrailerMoviesList[randomNumber];

              // set the active movie in facade
              for (int i = 0; i < MyFilms.r.Length; i++)
              {
                DataRow sr = MyFilms.r[i];
                if (!string.IsNullOrEmpty(sr["Number"].ToString()) && MyFilms.currentTrailerPlayingItem.ID == Int32.Parse(sr["Number"].ToString()))
                {
                  MyFilms.conf.StrIndex = i;
                  MyFilms.conf.StrTIndex = sr[MyFilms.conf.StrTitle1].ToString();
                }
              }
              
              g_Player.Stop();
              MyFilms.trailerscrobbleactive = true;
              trailerPlayed = true;
              Launch_Movie_Trailer_Scrobbling(trailer, MyFilms.ID_MyFilmsDetail);
            }
            break;
          }
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_PICTURE:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_PICTURE:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU:
        case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_ITEM:
        default:
          break;
      }
    }

    private static string LoadPlaylist(string filename)
    {
      var playlist = new PlayList();
      IPlayListIO loader = PlayListFactory.CreateIO(filename);

      if (!loader.Load(playlist, filename))
      {
        LogMyFilms.Info("Playlist not found for movie : '" + filename + "'");
        return "";
      }
      filename = "";
      foreach (PlayListItem playListItem in playlist)
      {
        if (filename.Length == 0)
          filename = playListItem.FileName;
        else
          filename = filename + ";" + playListItem.FileName;
      }
      return filename;
    }

    private static int UpdateDatabase(string filename, DataRow row, int idMovie)
    {
      var movieDetails = new IMDBMovie();
      if (idMovie <= 0) // add new VDB movie
      {
        idMovie = VideoDatabase.AddMovie(filename, row["Subtitles"].ToString().Length > 0);
        string wdescription = System.Web.HttpUtility.HtmlEncode(row["Description"].ToString().Replace('’', '\''));
        wdescription = wdescription.Replace('|', '\n');
        movieDetails.PlotOutline = movieDetails.Plot = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wdescription));

        var title = row[MyFilms.conf.StrTitle1].ToString();
        movieDetails.Title = (title.LastIndexOf("\\", StringComparison.Ordinal) > 0) ? title.Substring(title.LastIndexOf("\\", StringComparison.Ordinal) + 1) : title;

        try
        { movieDetails.RunTime = Int32.Parse(row["Length"].ToString()); }
        catch
        { movieDetails.RunTime = 0; }
        try
        { movieDetails.Rating = (float)Double.Parse(row["Rating"].ToString()); }
        catch
        { movieDetails.Rating = 0; }
        try
        { movieDetails.Year = Int32.Parse(row["Year"].ToString()); }
        catch
        { movieDetails.Year = 0; }

        string titleExt = movieDetails.Title + "{" + idMovie + "}";

        //string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
        //string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
        string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, titleExt);
        string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, titleExt);

        try
        {
          string wImage;
          string dbPicture = row["Picture"].ToString();
          if (dbPicture.IndexOf(":\\", StringComparison.Ordinal) == -1 && (dbPicture.Length < 2 || dbPicture.Substring(0, 2) != "\\\\"))
            wImage = MyFilms.conf.StrPathImg + "\\" + dbPicture;
          else
            wImage = dbPicture;
          if (!File.Exists(wImage))
            wImage = Config.Dir.Skin + "\\media\\Films_No_Cover.png";
          if (File.Exists(wImage)) // && wImage.Length < 249 check pathes are not too long!
          {
            if (!File.Exists(strThumb)) Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, Thumbs.SpeedThumbsSmall);
            if (!File.Exists(LargeThumb)) Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, Thumbs.SpeedThumbsLarge);
          }
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex, "Error updating MyVideos DB!");
        }
        movieDetails.Director = row["Director"].ToString();
        wzone = null;
        Analyze(row["Category"].ToString(), idMovie, "Genre");
        movieDetails.Genre = wzone;
        wzone = null;
        Analyze(row["Actors"].ToString(), idMovie, "Actor");
        movieDetails.Cast = wzone;
        VideoDatabase.SetMovieInfoById(idMovie, ref movieDetails);
      }
      else // update existing VDB movie
      {
        int pathId, widMovie;
        widMovie = VideoDatabase.GetFile(filename, out pathId, out widMovie, true);
        if (widMovie != idMovie)
        {
          string strPath, strFileName;

          Helper.DatabaseUtility.Split(filename, out strPath, out strFileName);
          Helper.DatabaseUtility.RemoveInvalidChars(ref strPath);
          Helper.DatabaseUtility.RemoveInvalidChars(ref strFileName);
          pathId = VideoDatabase.GetPath(strPath);
          pathId = pathId == -1 ? VideoDatabase.AddPath(strPath) : VideoDatabase.GetPath(strPath);
          VideoDatabase.AddFile(idMovie, pathId, strFileName);
        }
      }
      return idMovie;
    }

    private static void Analyze(string champselect, int iidmovie, string field)
    {
      ArrayList wTableau = MyFilms.Search_String(champselect);
      foreach (object t in wTableau)
      {
        UpdateFieldDb(t.ToString(), iidmovie, field);
      }
    }

    private static void SearchAllFiles(DataRow row, bool delete, ref bool noResumeMovie, ref ArrayList newItems, ref int movieIndex, bool trailer, string overrideFileName)
    {
      string fileName = string.Empty;
      string strDir = string.Empty;
      int idMovie = -1;
      int timeMovieStopped = 0;
      if (trailer)
      {
        if (MyFilms.conf.StrDirStorTrailer.Length > 0)
          strDir = MyFilms.conf.StrDirStorTrailer;
      }
      else
        strDir = MyFilms.conf.StrDirStor;

      LogMyFilms.Debug("(Search_All_Files) - Modus 'Trailer' = '" + trailer + "' - Searchpath (strDir): '" + strDir + "'");
      // retrieve filename information stored in the DB
      if (trailer)
      {
        if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
        {
          LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - try filename rows[selectItem][MyFilms.conf.StrStorageTrailer]: '" + row[MyFilms.conf.StrStorageTrailer].ToString().Trim() + "' - ConfStorageTrailer: '" + MyFilms.conf.StrStorageTrailer + "'");
          try { fileName = row[MyFilms.conf.StrStorageTrailer].ToString().Trim(); }
          catch { fileName = string.Empty; }
        }
      }
      else
      {
        if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
        {
          LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - try filename rows[selectItem][MyFilms.conf.StrStorage]: '" + row[MyFilms.conf.StrStorage].ToString().Trim() + "' - ConfStorage: '" + MyFilms.conf.StrStorage + "'");
          try { fileName = row[MyFilms.conf.StrStorage].ToString().Trim(); }
          catch { fileName = string.Empty; }
        }
      }

      if (!string.IsNullOrEmpty(overrideFileName))
      {
        LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - override filename: '" + overrideFileName.Trim());
        fileName = overrideFileName.Trim();
      }

      if (string.IsNullOrEmpty(fileName) && !trailer)
      {
        // search filename by Title movie
        if (MyFilms.conf.SearchFile == "True" || MyFilms.conf.SearchFile == "yes")
        {
          string movieName = row[MyFilms.conf.ItemSearchFile].ToString();
          LogMyFilms.Debug("Search_All_Files: Search = active, movieName = '" + movieName + "'");
          movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim, StringComparison.Ordinal) + 1).Trim();
          LogMyFilms.Debug("Search_All_Files: Search = active, movieName (cleaned) = '" + movieName + "'");
          if (MyFilms.conf.ItemSearchFile.Length > 0)
          {
            LogMyFilms.Debug("Search_All_Files: Search = active, movieName (cleaned) = '" + movieName + "', SearchDirectory: '" + MyFilms.conf.StrDirStor + "'");
            fileName = SearchFileName(movieName, MyFilms.conf.StrDirStor).Trim();
          }

          if ((fileName.Length > 0) && Helper.FieldIsSet(MyFilms.conf.StrStorage))
          {
            row[MyFilms.conf.StrStorage] = fileName;
            Update_XML_database();
          }
        }
      }
      bool wisomounted = false;
      string wisofile = string.Empty;
      // split filename information delimited by semicolumn (multifile detection)
      string[] split1 = fileName.Split(new Char[] { ';' });
      foreach (string wfile in split1)
      {
        fileName = wfile.IndexOf("/") == -1 ? wfile.Trim() : wfile.Substring(0, wfile.IndexOf("/")).Trim();
        if (fileName.Length > 0)
        {
          // first verify if file exists and if it's a video file
          if (File.Exists(fileName) && MediaPortal.Util.Utils.IsVideo(fileName) && !VirtualDirectory.IsImageFile(Path.GetExtension(fileName)))
          {
            if (delete)
            {
              delete_movie(fileName, ref newItems);
              Search_parts(fileName, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
              continue;
            }
            else
            {
              idMovie = -1;
              add_update_movie(fileName, row, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
              Search_parts(fileName, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
              continue;
            }
          }

          if (!File.Exists(fileName) && !Directory.Exists(fileName))
          {
            fileName = SearchMovie(fileName, strDir);
            if (fileName == "false")
              break;
          }
          // for image disk file filenames can be designed after iso filename delimited by '/'    
          string[] split2 = wfile.Split(new Char[] { '/' });
          int wfin = split2.Length;
          // detect if first part is an iso file
          if (VirtualDirectory.IsImageFile(Path.GetExtension(split2[0].Trim())))
          {
            string wtomount = split2[0].Trim();
            if (!File.Exists(wtomount))
              wtomount = SearchMovie(wtomount, strDir);
            if (wtomount == "false")
            {
              // image file not found
              break;
            }
            // only one iso file can be mounted
            if (!wisomounted && (wisofile == wtomount || wisofile == string.Empty))
            {
              if (MountImageFile(wGetID, wtomount))
              {
                wisomounted = true;
                wisofile = wtomount;
                string wdir = DaemonTools.GetVirtualDrive();
                if (wdir.LastIndexOf("\\", StringComparison.Ordinal) != wdir.Length - 1)
                  wdir = wdir.Trim() + "\\";
                fileName = string.Empty;
                for (int wi = 1; wi == split2.Length - 1; wi++)
                {
                  if (fileName.Length > 0)
                    fileName = fileName + ";" + wdir + split2[wi].Trim();
                  else
                    fileName = wdir + split2[wi].Trim();
                }
                if (fileName == string.Empty)
                  fileName = wdir;
              }
            }
          }
          if (PlayListFactory.IsPlayList(fileName))
            fileName = LoadPlaylist(wfile.Trim());
          // complete path to file (exist '\\' or '[Drive letter]:') ?
          if (fileName.Substring(0, 2) != "\\\\" && fileName.Substring(1, 1) != ":")
          {
            // search file into StrDir (no complete path)
            // split StrDir information delimited by semicolumn (mulpath to search)
            fileName = SearchMovie(fileName, strDir);
            if (fileName == "false")
            {
              // file not found
              break;
            }
          }
          string[] split3 = fileName.Split(new Char[] { ';' });
          foreach (string wfile2 in split3)
          {
            string wfile3;
            if (!File.Exists(wfile2.Trim()) && !Directory.Exists(wfile2.Trim()))
            {
              wfile3 = SearchMovie(wfile2.Trim(), strDir);
              if (wfile3 == "false")
                break;
            }
            else
              wfile3 = wfile2.Trim();
            if (File.Exists(wfile3) && MediaPortal.Util.Utils.IsVideo(wfile3))
            {
              if (delete)
              {
                delete_movie(wfile3, ref newItems);
                Search_parts(wfile3, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                continue;
              }
              else
              {
                idMovie = -1;
                add_update_movie(wfile3, row, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                Search_parts(wfile3, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                continue;
              }
            }
            if (File.Exists(wfile3 + @"\VIDEO_TS\VIDEO_TS.IFO"))
            {
              if (delete)
              {
                delete_movie(wfile3 + @"\VIDEO_TS\VIDEO_TS.IFO", ref newItems);
                Search_parts(wfile3, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                continue;
              }
              else
              {
                idMovie = -1;
                add_update_movie(wfile3 + @"\VIDEO_TS\VIDEO_TS.IFO", row, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                Search_parts(fileName, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                continue;
              }
            }

            if (File.Exists(wfile3 + @"\BDMV\index.bdmv")) // check for bluray in iso ...
            {
              if (delete)
              {
                delete_movie(wfile3 + @"\BDMV\index.bdmv", ref newItems);
                Search_parts(wfile3, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                continue;
              }
              else
              {
                idMovie = -1;
                add_update_movie(wfile3 + @"\BDMV\index.bdmv", row, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                Search_parts(fileName, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                continue;
              }
            }

            if (Directory.Exists(wfile3))
            {
              // it's a Directory so all files included are added to playlist
              var dirsInf = new DirectoryInfo(wfile3);
              //On retourne une liste d'informations sur les fichiers contenus dans le répertoire
              FileSystemInfo[] filesInf = dirsInf.GetFileSystemInfos();
              foreach (FileSystemInfo fi in filesInf.Where(fi => MediaPortal.Util.Utils.IsVideo(fi.FullName)))
              {
                if (delete)
                {
                  delete_movie(fi.FullName, ref newItems);
                  Search_parts(fi.FullName, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                }
                else
                {
                  idMovie = -1;
                  add_update_movie(fi.FullName, row, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                  Search_parts(fi.FullName, row, delete, ref noResumeMovie, ref newItems, ref idMovie, ref movieIndex, ref timeMovieStopped);
                }
              }
            }
          }
        }
      }
    }

    private static string SearchFileName(string filename, string storage)
    {
      LogMyFilms.Debug("Search_FileName: Launched with parameters: filename = '" + filename + "', storage = '" + storage + "'");
      result = new ArrayList();
      string file = filename;

      SearchFiles(file, storage, false, false);

      //si des resultats existent on les affichent
      if (result.Count != 0)
      {
        if (result.Count == 1)
        {
          if (result[0].ToString().ToLower() == filename.ToLower() || MyFilms.conf.SearchOnlyExactMatches == false)
          {
            LogMyFilms.Debug("only one match found - return result: '" + result[0] + "'");
            return result[0].ToString();
          }
          else
          {
            LogMyFilms.Debug("only one match found - but no exact match: '" + result[0] + "'");
          }
        }
        string wfile = null;
        result.Sort();
        var wresult = new List<string>();
        foreach (String s in result)
        {
          LogMyFilms.Debug("Search_FileName - Searchresult: '" + s + "'");
          if (string.IsNullOrEmpty(wfile))
          {
            wresult.Add(s);
            wfile = s;
          }
          else if (!MediaPortal.Util.Utils.ShouldStack(s, wfile) && s.ToLower() != wfile.ToLower())
          {
            wresult.Add(s);
            wfile = s;
          }
        }
        LogMyFilms.Debug("Search_FileName - Total Searchresults: '" + wresult.Count + "'");
        if (wresult.Count == 1 && (wresult[0].ToLower() == filename.ToLower() || MyFilms.conf.SearchOnlyExactMatches == false))
          return wresult[0];
        else
        {
          string singlefilefound = string.Empty;
          int count = 0;
          foreach (string searchresult in wresult.Cast<string>().Where(searchresult => searchresult == filename || searchresult.ToLower().Contains(@"\" + filename.ToLower() + @".") || searchresult.ToLower().Contains(@"\" + filename.ToLower() + @"\")))
          {
            LogMyFilms.Debug("full match found via file/directory compare -> '" + searchresult + "'");
            count = count + 1;
            singlefilefound = searchresult;
          }
          if (count == 1)
          {
            LogMyFilms.Debug("Single match found via file/directory compare -> using: '" + singlefilefound + "'");
            return singlefilefound;
          }
          // else if (MyFilms.conf.SearchOnlyExactMatches.ToLower() != "no") return ""; // activate, if no "near match" should be displayed in exact match mode
        }
        {
          // Many files found; ask for the good file
          var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg == null) return string.Empty;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
          foreach (string moviefile in wresult)
            dlg.Add(moviefile);
          dlg.DoModal(wGetID);

          if (dlg.SelectedLabel == -1)
            return "";
          return wresult[dlg.SelectedLabel];
        }
      }
      else // No Movie File Found
      {
        LogMyFilms.Info("File not found for movie '" + filename + "'; string search '" + wsearchfile + "'");
        return string.Empty;
      }
    }

    /// <summary>
    /// Permet de rechercher un fichier dans un dossier et ces sous dossier
    /// </summary>
    /// <param name="fileName">le nom du fichier recherché</param>
    /// <param name="searchrep">le chemin du répertoire dans lequel chercher</param>
    /// <param name="recur">spécifie s'il s'agit d'une relance recursive</param>
    /// <param name="Trailer">if called for trailers</param>
    public static void SearchFiles(string fileName, string searchrep, bool recur, bool Trailer)
    {
      var oRegex = new Regex(";");
      string file = fileName;
      if (!recur)
      {
        file = MediaPortal.Util.Utils.FilterFileName(fileName.ToLower());
        string[] drives = Environment.GetLogicalDrives();
        searchrep = (from drive in drives where MediaPortal.Util.Utils.getDriveType(drive) == 5 select String.Format(@"{0}:\", drive.Substring(0, 1))).Aggregate(searchrep, (current, driverLetter) => current.Length > 0 ? current + ";" + driverLetter : driverLetter);
        file = ReplaceString(file);
        wsearchfile = file;
      }
      string[] searchDir = oRegex.Split(searchrep);
      oRegex = new Regex(file);
      foreach (string path in searchDir)
      {
        string wpath;
        if (path.LastIndexOf(@"\", StringComparison.Ordinal) != path.Length - 1)
          wpath = path + "\\";
        else
          wpath = path;
        if (Directory.Exists(wpath))
        {
          var dirsInf = new DirectoryInfo(wpath);
          //On retourne une liste d'informations sur les fichiers contenus dans le répertoire
          FileSystemInfo[] filesInf = dirsInf.GetFileSystemInfos();

          //Si le nom d'un fichier correspond avec le nom du fichier recherché 
          //on place son path dans la variable globale result.
          foreach (FileSystemInfo fi in from fi in filesInf let oMatches = oRegex.Matches(fi.Name.ToLower()) from Match oMatch in oMatches select fi)
          {
            if (MediaPortal.Util.Utils.IsVideo(fi.Name) || VirtualDirectory.IsImageFile(System.IO.Path.GetExtension(fi.Name)))
              result.Add(wpath + fi.Name);
            else
            {
              if (fi.Attributes.ToString() == "Directory")
                if (File.Exists(wpath.Trim() + fi.Name.Trim() + "\\VIDEO_TS\\VIDEO_TS.IFO"))
                  result.Add(wpath.Trim() + fi.Name.Trim() + "\\VIDEO_TS\\VIDEO_TS.IFO");
                else
                  if (MediaPortal.Util.Utils.IsVideo(fi.Name))
                    result.Add(wpath.Trim() + fi.Name.Trim());
            }
          }
          //Si le parametre SearchSubDirs vaut true on réalise une 
          //recherche récursive sur tous les sous-répertoires
          if (MyFilms.conf.SearchSubDirs == false) continue;
          foreach (DirectoryInfo dir in dirsInf.GetDirectories())
          {
            //On rappelle la méthode SearchFiles pour tous les sous-répertoires  
            SearchFiles(file, dir.FullName, true, false);
          }
        }
      }
    }

    private static void UpdateFieldDb(string champselect, int iidmovie, string field)
    {
      var movieDetails = new IMDBMovie();

      if (field == "Genre")
      {
        int iiGenre = VideoDatabase.AddGenre(champselect);
        VideoDatabase.AddGenreToMovie(iidmovie, iiGenre);
        if (wzone == null)
          wzone = champselect;
        else
          wzone = wzone + "/" + champselect;
      }
      else // for actors
      {
        VideoDatabase.GetMovieInfoById(iidmovie, ref movieDetails);
        var actorMovie = new IMDBActor.IMDBActorMovie();
        actorMovie.MovieTitle = movieDetails.Title;
        actorMovie.Year = movieDetails.Year;
        //#if MP1X
        //                int iiActor = VideoDatabase.AddActor(champselect);
        //                VideoDatabase.AddActorToMovie(iidmovie, iiActor);
        //#else
        //                int iiActor = VideoDatabase.AddActor(string.Empty, champselect);
        //                VideoDatabase.AddActorToMovie(iidmovie, iiActor, "actor");
        //#endif

        int iiActor = GUIUtils.AddActor(string.Empty, champselect);
        GUIUtils.AddActorToMovie(iidmovie, iiActor, "actor");

        VideoDatabase.AddActorInfoMovie(iiActor, actorMovie);
        wzone = wzone == null ? champselect : wzone + "\n" + champselect;
      }
    }

    static public string SearchMovie(string filename, string strDirStor)
    {
      //check if media file is on Disk
      if (File.Exists(filename) || Directory.Exists(filename))
      {
        return filename;
      }
      else
      {
        string searchrep = strDirStor;
        DriveInfo[] allDrives = DriveInfo.GetDrives();

        searchrep = allDrives.Where(d => (d.DriveType.ToString() == "CDRom") && d.IsReady).Aggregate(searchrep, (current, d) => current.Length > 0 ? current + ";" + d.Name : d.Name);
        string file = filename.Substring(filename.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
        var oRegex = new Regex(";");
        string[] searchDir = oRegex.Split(searchrep);
        foreach (string wpath in searchDir.Select(path => path.LastIndexOf(@"\", StringComparison.Ordinal) != path.Length - 1 ? path + "\\" : path))
        {
          if (File.Exists(wpath + file) || Directory.Exists(wpath + file)) return (wpath + file);
          if (MyFilms.conf.SearchSubDirs == false || !Directory.Exists(wpath)) continue;
          foreach (string sFolderSub in Directory.GetDirectories(wpath, "*", SearchOption.AllDirectories).Where(sFolderSub => (File.Exists(sFolderSub + "\\" + file)) || (Directory.Exists(sFolderSub + "\\" + file))))
          {
            return (sFolderSub + "\\" + file);
          }
        }
        return "false";
      }
    }

    static public void PlayMovieFromPlayList(bool noResumeMovie, int iMovieIndex)
    {
      string filename = iMovieIndex == -1 ? playlistPlayer.GetNext() : playlistPlayer.Get(iMovieIndex);
      var movieDetails = new IMDBMovie();
      VideoDatabase.GetMovieInfo(filename, ref movieDetails);
      int idFile = VideoDatabase.GetFileId(filename);
      int idMovie = VideoDatabase.GetMovieId(filename);
      int timeMovieStopped = 0;
      byte[] resumeData = null;
      if (idMovie >= 0 && idFile >= 0)
      {
        timeMovieStopped = VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
        if (timeMovieStopped > 0)
        {
          string title = Path.GetFileName(filename);
          VideoDatabase.GetMovieInfoById(idMovie, ref movieDetails);
          if (movieDetails.Title != String.Empty) title = movieDetails.Title;

          if (noResumeMovie)
          {
            var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (null == dlgYesNo) return;
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
            dlgYesNo.SetLine(1, title);
            dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + MediaPortal.Util.Utils.SecondsToHMSString(timeMovieStopped));
            dlgYesNo.SetDefaultToYes(true);
            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
            if (!dlgYesNo.IsConfirmed)
              timeMovieStopped = 0;
          }
        }
      }

      // might speed up playback start
      // GUIGraphicsContext.IsFullScreenVideo = true;
      // GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO);
      
      if (iMovieIndex == -1)
        playlistPlayer.PlayNext();
      else
        playlistPlayer.Play(iMovieIndex);

      if (g_Player.Playing && timeMovieStopped > 0)
      {
        if (g_Player.IsDVD)
          g_Player.Player.SetResumeState(resumeData);
        else
        {
          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SEEK_POSITION, 0, 0, 0, 0, 0, null)
            {
              Param1 = (int)timeMovieStopped
            };
          GUIGraphicsContext.SendMessage(msg);
        }
      }
    }

    static public void PlayMovieFromPlayListTrailer(int iMovieIndex)
    {
      trailerPlayed = true;
      string filename = iMovieIndex == -1 ? playlistPlayer.GetNext() : playlistPlayer.Get(iMovieIndex);
      var movieDetails = new IMDBMovie();
      VideoDatabase.GetMovieInfo(filename, ref movieDetails);
      if (iMovieIndex == -1)
        playlistPlayer.PlayNext();
      else
        playlistPlayer.Play(iMovieIndex);
    }

    static public bool MountImageFile(int windowId, string file)
    {
      LogMyFilms.Warn("MountImageFile() - called with WindowID = '" + windowId + "', file = '" + file + "'");
      var mpSettings = new MPSettings();
      m_askBeforePlayingDVDImage = mpSettings.GetValueAsBool("daemon", "askbeforeplaying", false);
      if (!DaemonTools.IsMounted(file))
      {
        if (m_askBeforePlayingDVDImage)
        {
          var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
          if (dlgYesNo != null)
          {
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(713));
            dlgYesNo.SetLine(1, GUILocalizeStrings.Get(531));
            dlgYesNo.DoModal(windowId);
            if (!dlgYesNo.IsConfirmed) return false;
          }
        }
        string vdrive;
        DaemonTools.Mount(file, out vdrive);
        if (vdrive == string.Empty && file != String.Empty) return false; // protected share, with wrong pincode
      }

      return DaemonTools.IsMounted(file);
    }

    static public string ReplaceString(string file)
    {
      file = file.Replace("-", " ");
      file = file.Replace("!", " ");
      file = file.Replace("#", " ");
      file = file.Replace(";", " ");
      file = file.Replace(":", " ");
      file = file.Replace("'", " ");
      file = file.Replace("=", " ");
      file = file.Replace("&", " ");
      file = file.Replace("(", " ");
      file = file.Replace(")", " ");
      file = file.Replace("@", " ");
      file = file.Replace("%", " ");
      file = file.Replace("$", " ");
      file = file.Replace("_", " ");
      file = file.Replace(".", " ");
      file = file.Replace(",", " ");
      file = file.Trim();

      var oRegex = new Regex(" +");
      file = oRegex.Replace(file, ":");

      file = file.Replace(":", ".*");
      file = file.Replace("é", "[eé]");
      file = file.Replace("è", "[eè]");
      file = file.Replace("ê", "[eê]");
      file = file.Replace("ë", "[eë]");
      file = file.Replace("ô", "[oô]");
      file = file.Replace("ö", "[oö]");
      file = file.Replace("à", "[aà]");
      file = file.Replace("â", "[aâ]");
      file = file.Replace("ä", "[aä]");
      file = file.Replace("ù", "[uù]");
      file = file.Replace("û", "[uû]");
      file = file.Replace("ü", "[uü]");
      file = file.Replace("î", "[iî]");
      file = file.Replace("ï", "[iï]");
      file = file.Replace("ç", "[cç]");
      return file;
    }

    ///// <summary>
    ///// Execute as administrator the cmd file that enable the login screen change if enable == true.
    ///// Otherwise, execute the cmd file that disable the login screen change.
    ///// Shows an UAC prompt.
    ///// </summary>
    //bool launchCmd(bool enable)
    //{
    //  string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    //  try
    //  {
    //    Process p = new Process();
    //    p.StartInfo.FileName = "cmd.exe";
    //    if (enable)
    //      p.StartInfo.Arguments = "/C \"" + path + "\\enable background change\"";
    //    else
    //      p.StartInfo.Arguments = "/C \"" + path + "\\disable background change\"";
    //    p.StartInfo.Verb = "runas"; //shows the uac prompt
    //    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    //    if (enable)
    //    {
    //      p.EnableRaisingEvents = true;
    //      p.Exited += new EventHandler(p_Exited);
    //    }
    //    p.Start();
    //    alreadyAskedPermission = true;
    //    return true;
    //  }
    //  catch (Exception)
    //  {
    //    showMessage(Util.getText("error"), Util.getText("operationCanceled"), "");
    //  }
    //  return false;
    //}

    static public void RunProgram(string exeName, string argsLine)
    {
      if (exeName.Length > 0)
      {
        // Use ProcessStartInfo class
        // ProcessStartInfo psI = new ProcessStartInfo(exeName, argsLine);
        //ProcessStartInfo startInfo = new ProcessStartInfo();
        LogMyFilms.Debug("Launching process with filename = '" + exeName + "' and argument = '" + argsLine + "'");
        var newProcess = new Process();

        try
        {
          newProcess.StartInfo.FileName = exeName;
          newProcess.StartInfo.Arguments = argsLine;
          newProcess.StartInfo.UseShellExecute = true;
          newProcess.StartInfo.CreateNoWindow = true;
          newProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
          //newProcess.StartInfo.WorkingDirectory = "C:\\Programme\\Team MediaPortal\\MediaPortal";
          if (OSInfo.OSInfo.VistaOrLater())
          {
            newProcess.StartInfo.Verb = "runas";
          }
          newProcess.Start();
          while (!newProcess.HasExited)
          {
          }
        }

        catch (Exception e)
        {
          throw e;
        }
      }
    }

    static public void RunAmCupdater(string exeName, string argsLine)
    {
      if (exeName.Length > 0)
      {
        using (var p = new Process())
        {
          var psi = new ProcessStartInfo();
          psi.FileName = exeName;
          psi.UseShellExecute = true;
          psi.WindowStyle = ProcessWindowStyle.Minimized;
          psi.Arguments = argsLine;
          psi.ErrorDialog = false;
          if (OSInfo.OSInfo.VistaOrLater())
          {
            psi.Verb = "runas";
          }
          // psi.RedirectStandardOutput = true; // redirect output to streamreader - ToDo: add reader to use and add console output to AMCU !
          p.StartInfo = psi;
          LogMyFilms.Debug("RunAMCupdater - Starting external command: {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
          try
          {
            p.Start();
            // p.Kill();
            p.WaitForExit();
          }
          catch (Exception e)
          {
            LogMyFilms.Debug(e.ToString());
          }
          LogMyFilms.Debug("RunAMCupdater - External command finished");
        }
      }
    }

    //-------------------------------------------------------------------------------------------
    //  Search and Download Trailerfiles via TMDB (mostly YouTube)
    //-------------------------------------------------------------------------------------------        
    internal static void SearchAndDownloadTrailerOnlineTMDB(DataRow[] r1, int index, bool loadAllTrailers, bool interactive, string overridestoragepath)
    {
      //LogMyFilms.Debug("(SearchAndDownloadTrailerOnlineTMDB) - mastertitle      : '" + MyFilms.r[index][MyFilms.conf.StrTitle1] + "'");
      //if (Helper.FieldIsSet(MyFilms.conf.StrTitle2)) LogMyFilms.Debug("(SearchAndDownloadTrailerOnlineTMDB) - secondary title  : '" + MyFilms.r[index][MyFilms.conf.StrTitle2] + "'");
      //LogMyFilms.Debug("(SearchAndDownloadTrailerOnlineTMDB) - Cleaned Title    : '" + MediaPortal.Util.Utils.FilterFileName(MyFilms.r[index][MyFilms.conf.StrTitle1].ToString().ToLower()) + "'");

      string titlename = Helper.TitleWithoutGroupName(r1[index][MyFilms.conf.StrTitle1].ToString());
      string titlename2 = (Helper.FieldIsSet(MyFilms.conf.StrTitle2)) ? Helper.TitleWithoutGroupName(r1[index][MyFilms.conf.StrTitle2].ToString()) : "";
      string collectionname = Helper.TitleFirstGroupName(r1[index][MyFilms.conf.StrTitle1].ToString());

      string path;
      #region Retrieve original directory of mediafiles
      try
      {
        path = r1[index][MyFilms.conf.StrStorage].ToString();
        if (path.Contains(";")) path = path.Substring(0, path.IndexOf(";", StringComparison.Ordinal));
        //path = Path.GetDirectoryName(path);
        //if (path == null || !Directory.Exists(path))
        //{
        //  LogMyFilms.Warn("Directory of movie '" + titlename + "' doesn't exist anymore - check your DB");
        //  return;
        //}
        if (path.Contains("\\")) path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
        
        // LogMyFilms.Debug("(SearchAndDownloadTrailerOnlineTMDB) get media directory name: '" + path + "'");
      }
      catch (Exception)
      {
        LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() error with directory of movie '" + titlename + "' - check your DB");
        return;
      }
      #endregion

      string imdb = "";
      #region get imdb number sor better search match
      if (!string.IsNullOrEmpty(r1[index]["IMDB_Id"].ToString()))
        imdb = r1[index]["IMDB_Id"].ToString();
      else if (!string.IsNullOrEmpty(r1[index]["URL"].ToString()))
      {
        string urLstring = r1[index]["URL"].ToString();
        var cutText = new Regex("" + @"tt\d{7}" + "");
        var m = cutText.Match(urLstring);
        if (m.Success) imdb = m.Value;
      }
      #endregion

      int year;

      #region get local language
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      // LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB - detected language = '" + language + "'");
      #endregion

      LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - movie '" + titlename + "', media directory '" + path + "', language '" + language + "'");
      
      var api = new Tmdb(MyFilms.TmdbApiKey, language);
      // var tmdbConf = api.GetConfiguration();

      int selectedMovieId = 0;

      #region search matching TMDB movie id
      if (imdb.Contains("tt"))
      {
        TmdbMovie movie = api.GetMovieByIMDB(imdb);
        if (movie.id > 0)
        {
          selectedMovieId = movie.id;
        }
      }

      if (selectedMovieId == 0) // no movie found by imdb search
      {
        TmdbMovieSearch moviesfound;
        if (int.TryParse(r1[index]["Year"].ToString(), out year))
        {
          moviesfound = api.SearchMovie(titlename, 1, "", null, year);
          if (moviesfound.results.Count == 0) moviesfound = api.SearchMovie(titlename, 1, null);
        }
        else
        {
          moviesfound = api.SearchMovie(r1[index][MyFilms.conf.StrTitle1].ToString(), 1, null);
          if (moviesfound.results.Count == 0 && titlename2.Length > 0)
          {
            if (int.TryParse(r1[index]["Year"].ToString(), out year))
            {
              moviesfound = api.SearchMovie(titlename2, 1, "", null, year);
              if (moviesfound.results.Count == 0) moviesfound = api.SearchMovie(titlename2, 1, null);
            }
          }
        }

        if (moviesfound.results.Count == 1)
        {
          selectedMovieId = moviesfound.results[0].id;
        }
        else
        {
          LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - Movie Search Results: '" + moviesfound.total_results.ToString() + "' for movie '" + titlename + "'");
          if (!interactive) return;
          else
          {
            if (moviesfound.results.Count == 0)
            {
              while(selectedMovieId == 0)
              {
                selectedMovieId = SearchTmdbMovie(titlename, titlename, titlename2, year, language, false);
                if (selectedMovieId == -1) return; // cancel search
              }
            }
            else
            {
              var choiceMovies = new List<TmdbMovie>();
              var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
              if (dlg == null) return;
              dlg.Reset();
              dlg.SetHeading(GUILocalizeStrings.Get(10798992)); // Select movie ...

              foreach (TmdbMovie movieResult in moviesfound.results)
              {
                dlg.Add(movieResult.title + " (" + movieResult.release_date + ")");
                choiceMovies.Add(movieResult);
              }
              dlg.DoModal(GUIWindowManager.ActiveWindow);
              if (dlg.SelectedLabel == -1) return;
              selectedMovieId = choiceMovies[dlg.SelectedLabel].id;
            }
          }
        }
      }
      #endregion

      if (selectedMovieId == 0)
      {
        LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB - no movie found - no trailers added to DL queue - returning");
        if (interactive) GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798995)); // No matching movie found !
        return;
      }

      #region search trailers for movie
      var trailersfound = new List<Youtube>();

      TmdbMovieTrailers trailers = api.GetMovieTrailers(selectedMovieId, language); // trailers in local language
      if (trailers.youtube.Count > 0) trailersfound.AddRange(trailers.youtube);
      LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - '" + trailers.youtube.Count + "' trailers found in local language ('" + language + "')");

      if (language != "en")
      {
        trailers = api.GetMovieTrailers(selectedMovieId, "en"); // engish trailers
        if (trailers.youtube.Count > 0) trailersfound.AddRange(trailers.youtube);
        LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - '" + trailers.youtube.Count + "' trailers found in language 'en'");
      }

      if (trailersfound.Count == 0)
      {
        LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - no trailers found - returning");
        if (interactive) GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798996)); // no trailers found !
      }
      else
      {
        LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - '" + trailersfound.Count + "' trailers found");
        var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;

        Youtube selectedTrailer = null;
        string selectedTrailerUrl = "";
        string selectedQuality = "";

        if (interactive)
        {
          #region build menu with available trailers
          var choiceView = new List<Youtube>();
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798993)); // Select trailer ...

          dlg.Add("<" + GUILocalizeStrings.Get(10798997) + ">"); // load all 
          choiceView.Add(new Youtube());

          foreach (Youtube trailer in trailersfound)
          {
            dlg.Add(trailer.name + " (" + trailer.size + ")");
            choiceView.Add(trailer);
          }

          //dlg.Add(GUILocalizeStrings.Get(10798711)); //search youtube trailer with onlinevideos
          //choiceView.Add("youtube");

          dlg.DoModal(GUIWindowManager.ActiveWindow);
          if (dlg.SelectedLabel == -1) return;
          if (dlg.SelectedLabel >  0)
          {
            selectedTrailer = choiceView[dlg.SelectedLabel];
            LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - selectedTrailer = '" + selectedTrailer.name + " (" + selectedTrailer.size + ")'");
          }
          #endregion
        }

        #region select trailer format and quality in OV player factory
        if (selectedTrailer != null)
        {
          Dictionary<string, string> availableTrailerFiles = MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls("http://www.youtube.com/watch?v=" + selectedTrailer.source);
          var choiceView = new List<string>();
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798994)); // Select quality ...
          foreach (KeyValuePair<string, string> availableTrailerFile in availableTrailerFiles)
          {
            dlg.Add(availableTrailerFile.Key);
            choiceView.Add(availableTrailerFile.Value); // this is the download URL
          }
          dlg.DoModal(GUIWindowManager.ActiveWindow);
          if (dlg.SelectedLabel == -1) return;
          selectedTrailerUrl = choiceView[dlg.SelectedLabel];
          selectedQuality = dlg.SelectedLabelText;
        }
        #endregion

        #region download trailer

        string destinationDirectory;
        if (overridestoragepath != null)
        {
          string newpath = Path.Combine(overridestoragepath + @"MyFilms\", path.Substring(path.LastIndexOf("\\") + 1));
          newpath = Path.Combine(newpath, "Trailer");
          destinationDirectory = newpath;
        }
        else
        {
          destinationDirectory = Path.Combine(path, "Trailer");
        }

        if (selectedTrailerUrl.Length > 0 && selectedTrailer != null)
        {
          var trailer = new Trailer();
          trailer.MovieTitle = titlename;
          trailer.Trailername = selectedTrailer.name;
          trailer.OriginalUrl = "http://www.youtube.com/watch?v=" + selectedTrailer.source;
          trailer.SourceUrl = selectedTrailerUrl;
          trailer.Quality = selectedQuality;
          trailer.DestinationDirectory = destinationDirectory; // Path.Combine(Path.Combine(path, "Trailer"), (MediaPortal.Util.Utils.FilterFileName(titlename + " (trailer) " + selectedTrailer.name + " (" + dlg.SelectedLabelText.Replace(" ", "") + ")" + extension)));
          MyFilms.AddTrailerToDownloadQueue(trailer);
          LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - start loading single trailer '" + selectedTrailer.name + "' from URL: '" + selectedTrailerUrl + "'");
          if (interactive) GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(1079703)); //  trailer addd to DL queue
        }
        else
        {
          for (int i = 0; i < trailersfound.Count; i++)
          {
            if (i < 2 || loadAllTrailers)
            {
              Dictionary<string, string> availableTrailerFiles = MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls("http://www.youtube.com/watch?v=" + trailersfound[i].source);
              string url = null;
              string quality = null;
              if (availableTrailerFiles != null && availableTrailerFiles.Count > 0)
              {
                //url = availableTrailerFiles.Last().Value;
                //quality = availableTrailerFiles.Last().Key;
                KeyValuePair<string, string> highestQualitySelection = MyFilmsPlugin.Utils.OVplayer.GetPreferredQualityOption(availableTrailerFiles, "FHD");
                url = highestQualitySelection.Value;
                quality = highestQualitySelection.Key;
                LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - selected trailer with quality = '" + quality ?? "" + "', url = '" + url ?? "" + "'");
              }
              else
              {
                LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - no download Url found - adding trailer without DL links for later processing from queue");
              }
              var trailer = new Trailer();
              trailer.MovieTitle = titlename;
              trailer.Trailername = trailersfound[i].name;
              trailer.OriginalUrl = "http://www.youtube.com/watch?v=" + trailersfound[i].source;
              trailer.SourceUrl = url;
              trailer.Quality = quality;
              trailer.DestinationDirectory = destinationDirectory;
              // filename: (MediaPortal.Util.Utils.FilterFileName(titlename + " (trailer) " + trailersfound[i].name + " (" + quality.Replace(" ", "") + ")" + extension))
              LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() - add trailer '#" + i + "'");
              MyFilms.AddTrailerToDownloadQueue(trailer);
            }
          }
        }
        #endregion
      }
      #endregion
    }

    private static int SearchTmdbMovie(string searchexpression, string title, string title2, int year, string language, bool collectionsearch)
    {
      LogMyFilms.Debug("SearchTmdbMovie() - title '" + title + "', title2 '" + title2 + "', year '" + year.ToString() + "'");
      var api = new Tmdb(MyFilms.TmdbApiKey, language);
      var tmdbConf = api.GetConfiguration();
      var allMoviesFound = new List<TmdbMovie>();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

      const int minChars = 2;
      const bool filter = true;

      if (dlg == null) return -1;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798646));  // Search Films
      dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
      allMoviesFound.Add(new TmdbMovie());

      foreach (TmdbMovie t in api.SearchMovie(searchexpression, 1, null).results)
      {
        LogMyFilms.Debug("TMDB - movie found '" + t.title + "'");
        var item = new GUIListItem();
        item.Label = t.title + "  (" + t.release_date + ")";
        item.IconImage = tmdbConf.images.base_url + "w500" + t.poster_path;
        item.ThumbnailImage = tmdbConf.images.base_url + "w500" + t.poster_path;
        if (collectionsearch)
        {
          TmdbMovie tmdbMovie = api.GetMovieInfo(t.id);
          TmdbCollection collection = api.GetCollectionInfo(tmdbMovie.belongs_to_collection.id);

          LogMyFilms.Debug("TMDB - Value found - movie   = '" + (tmdbMovie.title ?? "") + "'");
          LogMyFilms.Debug("TMDB - Value found - belongs to collection   = '" + (collection.name ?? "") + "'");

          item.Label += " - " + (collection.name ?? "<no collection>");
          item.IconImage = tmdbConf.images.base_url + "w500" + collection.poster_path;
        }
        dlg.Add(item);
        allMoviesFound.Add(t);
      }

      #region title2 search (disabled)
      //if (title2.Length > 0)
      //{
      //  foreach (MovieResult t in api.SearchMovie(title2, 1, null).results)
      //  {
      //    LogMyFilms.Debug("TMDB - movie found '" + t.title + "'");

      //    var item = new GUIListItem();
      //    item.Label = t.title + "  (" + t.release_date + ")";
      //    item.IconImage = tmdbConf.images.base_url + "w500" + t.poster_path;
      //    item.ThumbnailImage = tmdbConf.images.base_url + "w500" + t.poster_path;
      //    if (collectionsearch)
      //    {
      //      TmdbMovie tmdbMovie = api.GetMovieInfo(t.id);
      //      TmdbCollection collection = api.GetCollectionInfo(tmdbMovie.belongs_to_collection.id);

      //      LogMyFilms.Debug("TMDB - Value found - movie   = '" + (tmdbMovie.title ?? "") + "'");
      //      LogMyFilms.Debug("TMDB - Value found - belongs to collection   = '" + (collection.name ?? "") + "'");

      //      item.Label += " - " + (collection.name ?? "<no collection>");
      //      item.IconImage = tmdbConf.images.base_url + "w500" + collection.poster_path;
      //    }
      //    dlg.Add(item);
      //    allMoviesFound.Add(t);
      //  }
      //}
      #endregion

      if (allMoviesFound.Count > 0)
      {
        dlg.DoModal(wGetID);
      }
      else
      {
        dlg.SelectedLabel = 0;
      }
      if (dlg.SelectedLabel == -1) return -1;

      if (dlg.SelectedLabel == 0)
      {
        //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
        var dlgSearchFilm = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        if (dlgSearchFilm == null) return -1;
        dlgSearchFilm.Reset();
        dlgSearchFilm.SetHeading(GUILocalizeStrings.Get(1079859)); // choose search expression
        dlgSearchFilm.Add("  *****  " + GUILocalizeStrings.Get(1079858) + "  *****  ");
        dlgSearchFilm.Add(title);
        dlgSearchFilm.Add(title2);
        foreach (object t in from object t in MyFilms.SubTitleGrabbing(title) where t.ToString().Length > 1 select t) dlgSearchFilm.Add(t.ToString());
        foreach (object t in from object t in MyFilms.SubTitleGrabbing(title2) where t.ToString().Length > 1 select t) dlgSearchFilm.Add(t.ToString());
        foreach (object t in from object t in MyFilms.SubWordGrabbing(title, minChars, filter) where t.ToString().Length > 1 select t) dlgSearchFilm.Add(t.ToString()); // Min 3 Chars, Filter true (no der die das)
        foreach (object t in from object t in MyFilms.SubWordGrabbing(title2, minChars, filter) where t.ToString().Length > 1 select t) dlgSearchFilm.Add(t.ToString());
        dlgSearchFilm.DoModal(wGetID);

        if (dlgSearchFilm.SelectedLabel == 0) // enter manual searchstring via VK
        {
          var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
          if (null == keyboard) return -1;
          keyboard.Reset();
          keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
          keyboard.Text = title;
          keyboard.DoModal(wGetID);
          if (keyboard.IsConfirmed && keyboard.Text.Length > 0) return SearchTmdbMovie(keyboard.Text, title, title2, year, language, collectionsearch);
        }
        if (dlgSearchFilm.SelectedLabel > 0) return SearchTmdbMovie(dlgSearchFilm.SelectedLabelText, title, title2, year, language, collectionsearch);
      }

      if (dlg.SelectedLabel > 0) return allMoviesFound[dlg.SelectedLabel].id;

      return 0; // return 0 if no movie found or -1 if search was cancelled
    }

    //-------------------------------------------------------------------------------------------
    //  Search All Trailerfiles locally
    //-------------------------------------------------------------------------------------------        
    public static void SearchTrailerLocal(DataRow[] r1, int index, bool extendedSearch, bool saveresult)
    {
      var trailerFiles = new List<string>();
      var split = MyFilms.r[index][MyFilms.conf.StrStorageTrailer].ToString().Split(new Char[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
      var oldTrailerFiles = split.Distinct().ToList();

      string titlename = MyFilms.r[index][MyFilms.conf.StrTitle1].ToString();
      if (titlename.Contains("\\")) titlename = titlename.Substring(titlename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
      string titlename2 = (Helper.FieldIsSet(MyFilms.conf.StrTitle2)) ? MyFilms.r[index][MyFilms.conf.StrTitle2].ToString() : "";

      LogMyFilms.Debug("SearchTrailerLocal() - mastertitle      : '" + titlename + "'");
      LogMyFilms.Debug("SearchTrailerLocal() - secondary title  : '" + titlename2 + "'");
      LogMyFilms.Debug("SearchTrailerLocal() - Cleaned Title    : '" + Utils.FilterFileName(titlename.ToLower()) + "'");
      LogMyFilms.Debug("SearchTrailerLocal() - Extended Search '" + extendedSearch + "' for movie '" + titlename + "' in search directories: '" + MyFilms.conf.StrDirStorTrailer + "'");

      #region Get original directory of mediafiles
      string directoryname;
      try
      {
        string movieName = MyFilms.r[index][MyFilms.conf.StrStorage].ToString().Trim();
        movieName = movieName.Substring(movieName.LastIndexOf(";", StringComparison.Ordinal) + 1);
        directoryname = Path.GetDirectoryName(movieName);
        LogMyFilms.Debug("SearchTrailerLocal() - media directory name: '" + directoryname + "'");
        if (directoryname == null || !Directory.Exists(directoryname))
        {
          LogMyFilms.Warn("SearchTrailerLocal() directory of movie '" + titlename + "' doesn't exist anymore - check your DB");
          return;
        }
      }
      catch
      {
        LogMyFilms.Error("SearchTrailerLocal() error with directory of movie '" + titlename + "' - check your DB");
        return;
      }
      #endregion

      #region Search Files in movie directory
      if (!string.IsNullOrEmpty(directoryname))
      {
        string[] files = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories);
        trailerFiles.AddRange(files.Where(filefound => (filefound.ToLower().Contains("trailer") || filefound.ToLower().Contains("trl") || filefound.ToLower().Contains("clip")) && Utils.IsVideo(filefound)));
      }
      #endregion

      #region Search files in MyFilms subdirectory (used by TMDB trailer loader if not stored in movie dir)
      if (!string.IsNullOrEmpty(MyFilms.conf.StrDirStorTrailer))
      {
        string newpath = Path.Combine(Path.Combine(MyFilms.conf.StrDirStorTrailer + @"MyFilms\", directoryname.Substring(directoryname.LastIndexOf("\\", StringComparison.Ordinal) + 1)), "Trailer");
        LogMyFilms.Debug("SearchTrailerLocal() - search TMDB download trailer in '" + newpath + "'");
        if (Directory.Exists(newpath))
        {
          string[] files = Directory.GetFiles(newpath, "*.*", SearchOption.AllDirectories);
          trailerFiles.AddRange(files.Where(Utils.IsVideo));
        }
        else
        {
          LogMyFilms.Debug("SearchTrailerLocal() - path does not exist: '" + newpath + "'");
        }
      }
      #endregion

      #region Extended search - Search Filenames with "title" in extended Trailer Searchpathes
      if (extendedSearch && !string.IsNullOrEmpty(MyFilms.conf.StrDirStorTrailer))
      {
        LogMyFilms.Debug("SearchTrailerLocal - starting ExtendedSearch in Searchdirectory: '" + MyFilms.conf.StrDirStorTrailer + "'");
        // split searchpath information delimited by semicolumn (multiple searchpathes from config)
        string[] trailerSearchdirectories = MyFilms.conf.StrDirStorTrailer.Split(new Char[] { ';' });
        foreach (string storage in trailerSearchdirectories)
        {
          #region First search rootdirectory for matching files
          LogMyFilms.Debug("(TrailersearchLocal) - TrailerSearchDirectory: '" + storage + "', search title1: '" + titlename.ToLower() + "', search title2: '" + titlename2.ToLower() + "'");
          string[] files = Directory.GetFiles(storage, "*.*", SearchOption.TopDirectoryOnly);
          trailerFiles.AddRange(files.Where(MediaPortal.Util.Utils.IsVideo).Where(item => (!string.IsNullOrEmpty(titlename) && item.ToLower().Contains(titlename.ToLower())) || (!string.IsNullOrEmpty(titlename2) && item.ToLower().Contains(titlename2.ToLower()))));
          #endregion

          #region Now search for subdirectories matching the movie and add the content of them as result
          string[] subdirectories = Directory.GetDirectories(storage, "*.*", SearchOption.AllDirectories);
          foreach (string subdirectory in subdirectories)
          {
            #region check for directories with matching title name or files in subdirectory matching
            LogMyFilms.Debug("(TrailersearchLocal) - Directory found to check matching: '" + subdirectory + "'");
            if (Utility.ContainsAll(subdirectory, titlename, ":") || Utility.ContainsAll(subdirectory, titlename2, ":"))
            {
              #region check for directories with matching title name
              LogMyFilms.Debug("(TrailersearchLocal) - Matching Directory found : '" + subdirectory + "'");
              files = Directory.GetFiles(subdirectory, "*.*", SearchOption.AllDirectories);
              trailerFiles.AddRange(files.Where(MediaPortal.Util.Utils.IsVideo));
              #endregion
            }
            else
            {
              #region check for matching files inside subdirs
              files = Directory.GetFiles(subdirectory, "*.*", SearchOption.AllDirectories);
              trailerFiles.AddRange(files.Where(Utils.IsVideo).Where(filefound => Utility.ContainsAll(filefound, titlename, ":") || Utility.ContainsAll(filefound, titlename2, ":")));
              #endregion
            }
            #endregion
          }
          #endregion
        }
      }
      #endregion

      if (trailerFiles.Count > 0)
      {
        LogMyFilms.Debug("SearchTrailerLocal() - Total Files found: '" + trailerFiles.Count + "'");
        trailerFiles.AddRange(oldTrailerFiles);
        trailerFiles = trailerFiles.Select(x => x.Trim()).Distinct().ToList().Select(f => new FileInfo(f)).OrderByDescending(f => f.Length).Select(x => x.FullName).ToList();
        string newtrailerentry = "";
        foreach (string trailerFile in trailerFiles)
        {
          if (newtrailerentry.Length > 0) newtrailerentry += ";";
          newtrailerentry += trailerFile;
        }
        LogMyFilms.Debug("SearchTrailerLocal() - Old Trailersourcepath: '" + MyFilms.r[index][MyFilms.conf.StrStorageTrailer] + "'");
        MyFilms.r[index][MyFilms.conf.StrStorageTrailer] = newtrailerentry;
        LogMyFilms.Debug("SearchTrailerLocal() - New Trailersourcepath    : '" + MyFilms.r[index][MyFilms.conf.StrStorageTrailer] + "'");
        if (saveresult)
        {
          Update_XML_database();
          LogMyFilms.Debug("SearchTrailerLocal() - Database Updated !!!!");
        }
      }
      else
        LogMyFilms.Debug("SearchTrailerLocal() - NO TRAILERS FOUND !!!!");
    }

    //-------------------------------------------------------------------------------------------
    //  Delete Trailer Entries From DB
    //-------------------------------------------------------------------------------------------        
    public static void DeleteTrailerFromDB(DataRow[] r1, int index)
    {
      MyFilms.r[index][MyFilms.conf.StrStorageTrailer] = string.Empty;
      LogMyFilms.Debug("DeleteTrailerFromDB() - Trailer entries Deleted for Current Movie !");
      Update_XML_database();
      LogMyFilms.Debug("DeleteTrailerFromDB() - Database Updated !");
    }

    private static void HandleWakeUpNas()
    {
      string hostName;
      bool isWakeOnLanEnabled;
      bool isAutoMacAddressEnabled;
      int intTimeOut;

      //Get settings from MediaPortal.xml
      using (MediaPortal.Profile.Settings xmlreader = new MPSettings())
      //using (Settings xmlreader = new MediaPortal.Profile.MPSettings())
      {
        hostName = xmlreader.GetValueAsString("nas", "hostname", "");
        isWakeOnLanEnabled = xmlreader.GetValueAsBool("nas", "isWakeOnLanEnabled", false);
        isAutoMacAddressEnabled = xmlreader.GetValueAsBool("nas", "isAutoMacAddressEnabled", false);
        intTimeOut = xmlreader.GetValueAsInt("nas", "WOLTimeOut", 10);
      }

      //isWakeOnlanEnabled
      if (isWakeOnLanEnabled)
      {
        //Check for multi-seat installation
        //if (!IsSingleSeat())
        if (true)
        {
          var wakeOnLanManager = new WakeOnLanManager();

          //isAutoMacAddressEnabled
          byte[] hwAddress;
          String macAddress;
          if (isAutoMacAddressEnabled)
          {
            IPAddress ipAddress = null;

            //Check if we already have a valid IP address stored in RemoteControl.HostName,
            //otherwise try to resolve the IP address
            if (!IPAddress.TryParse(hostName, out ipAddress))
            {
              //Get IP address of the NAS server
              try
              {
                IPAddress[] ips = Dns.GetHostAddresses(hostName);

                LogMyFilms.Debug("(HandleWakeUpNas) : WOL - GetHostAddresses({0}) returns:", hostName);

                foreach (IPAddress ip in ips)
                {
                  LogMyFilms.Debug("    {0}", ip);
                }

                //Use first valid IP address
                ipAddress = ips[0];
              }
              catch (Exception ex)
              {
                LogMyFilms.Error("(HandleWakeUpNas) : WOL - Failed GetHostAddress - {0}", ex.Message);
              }
            }

            //Check for valid IP address
            if (ipAddress != null)
            {
              //Update the MAC address if possible
              hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

              if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
              {
                LogMyFilms.Debug("(HandleWakeUpNas) : WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                          , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                //Store MAC address
                macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                LogMyFilms.Debug("(HandleWakeUpNas) : WOL - Store MAC address: {0}", macAddress);

                using (
                  MediaPortal.Profile.Settings xmlwriter =
                    new MediaPortal.Profile.MPSettings())
                {
                  xmlwriter.SetValue("nas", "macAddress", macAddress);
                }
              }
            }
          }

          //Use stored MAC address
          using (MediaPortal.Profile.Settings xmlreader = new MPSettings())
          {
            macAddress = xmlreader.GetValueAsString("nas", "macAddress", null);
          }

          LogMyFilms.Debug("(HandleWakeUpNas) : WOL - Use stored MAC address: {0}", macAddress);

          try
          {
            hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

            //Finally, start up the TV server
            LogMyFilms.Info("(HandleWakeUpNas) : WOL - Start the NAS server");

            if (wakeOnLanManager.WakeupSystem(hwAddress, hostName, intTimeOut))
            {
              LogMyFilms.Info("(HandleWakeUpNas) : WOL - The NAS server started successfully!");
            }
            else
            {
              LogMyFilms.Error("(HandleWakeUpNas) : WOL - Failed to start the NAS server");
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("(HandleWakeUpNas) : WOL - Failed to start the NAS server - {0}", ex.Message);
          }
        }
      }
    }

    internal KeyValuePair<string, object>? ShowSelectionDialog(string headline, List<KeyValuePair<string, object>> itemsList)
    {
      var choiceView = new List<object>();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return null;
      dlg.Reset();
      dlg.SetHeading(headline);
      foreach (KeyValuePair<string, object> keyValuePair in itemsList)
      {
        dlg.Add(keyValuePair.Key);
        choiceView.Add(keyValuePair.Value);
      }
      dlg.DoModal(GUIWindowManager.ActiveWindow);
      if (dlg.SelectedLabel == -1) return null;
      return new KeyValuePair<string, object>(dlg.SelectedLabelText, choiceView[dlg.SelectedLabel]);
    }

    public static string getGUIProperty(string name)
    {
      return GUIPropertyManager.GetProperty("#myfilms." + name);
    }

    public static void setGUIProperty(string name, string value)
    {
      setGUIProperty(name, value, MyFilms.DebugPropertyLogging);
    }

    public static void setGUIProperty(string name, string value, bool log)
    {
      string property = "#myfilms." + name;
      string finalvalue = value.SanitizeXmlString().RemapHighOrderChars();
      if (log) LogMyFilms.Debug("setGuiProperty [{0}]: '{1}' - nonsanitized input: '{2}'", property, finalvalue, value);

      GUIPropertyManager.SetProperty(property, finalvalue);
    }

    public static void clearGUIProperty(string name)
    {
      setGUIProperty(name, string.Empty, MyFilms.DebugPropertyLogging);
    }

    public static void clearGUIProperty(string name, bool log)
    {
      setGUIProperty(name, string.Empty, log);
    }

    public static bool ExtendedStartmode(PluginMode pluginMode, string disabledfeature)
    {
      switch (pluginMode)
      {
        case (PluginMode.Test): // for public user testing
          if (MyFilmsPlugin.MyFilmsGUI.Configuration.PluginMode == "extended") return true; 
          if (MyFilmsPlugin.MyFilmsGUI.Configuration.PluginMode == "test") return true;
          else LogMyFilms.Debug("PluginMode: '{0}', disabled feature: '{1}", MyFilmsPlugin.MyFilmsGUI.Configuration.PluginMode, disabledfeature);
          return false;

        case (PluginMode.Extended): // for developer features only - set manually in config
          if (MyFilmsPlugin.MyFilmsGUI.Configuration.PluginMode == "extended") return true;
          else LogMyFilms.Debug("PluginMode: '{0}', disabled feature: '{1}", MyFilmsPlugin.MyFilmsGUI.Configuration.PluginMode, disabledfeature);
          return false;

        case (PluginMode.Normal):
        default:
          return true;
      }
    }

    /// <summary>
    /// make a cleaned title out of the filename/original title
    /// </summary>
    private static string GetSearchString(string strMovie)
    {
      string strUrl = strMovie;
      strUrl = strUrl.Replace(".", " ");
      strUrl = strUrl.Trim();

      RemoveAllAfter(ref strUrl, "divx");
      RemoveAllAfter(ref strUrl, "xvid");
      RemoveAllAfter(ref strUrl, "dvd");
      RemoveAllAfter(ref strUrl, " dvdrip");
      RemoveAllAfter(ref strUrl, "svcd");
      RemoveAllAfter(ref strUrl, "mvcd");
      RemoveAllAfter(ref strUrl, "vcd");
      RemoveAllAfter(ref strUrl, "cd");
      RemoveAllAfter(ref strUrl, "ac3");
      RemoveAllAfter(ref strUrl, "ogg");
      RemoveAllAfter(ref strUrl, "ogm");
      RemoveAllAfter(ref strUrl, "internal");
      RemoveAllAfter(ref strUrl, "fragment");
      RemoveAllAfter(ref strUrl, "proper");
      RemoveAllAfter(ref strUrl, "limited");
      RemoveAllAfter(ref strUrl, "rerip");
      RemoveAllAfter(ref strUrl, "bluray");
      RemoveAllAfter(ref strUrl, "brrip");
      RemoveAllAfter(ref strUrl, "hddvd");
      RemoveAllAfter(ref strUrl, "x264");
      RemoveAllAfter(ref strUrl, "mbluray");
      RemoveAllAfter(ref strUrl, "1080p");
      RemoveAllAfter(ref strUrl, "720p");
      RemoveAllAfter(ref strUrl, "480p");
      RemoveAllAfter(ref strUrl, "r5");

      RemoveAllAfter(ref strUrl, "+divx");
      RemoveAllAfter(ref strUrl, "+xvid");
      RemoveAllAfter(ref strUrl, "+dvd");
      RemoveAllAfter(ref strUrl, "+dvdrip");
      RemoveAllAfter(ref strUrl, "+svcd");
      RemoveAllAfter(ref strUrl, "+mvcd");
      RemoveAllAfter(ref strUrl, "+vcd");
      RemoveAllAfter(ref strUrl, "+cd");
      RemoveAllAfter(ref strUrl, "+ac3");
      RemoveAllAfter(ref strUrl, "+ogg");
      RemoveAllAfter(ref strUrl, "+ogm");
      RemoveAllAfter(ref strUrl, "+internal");
      RemoveAllAfter(ref strUrl, "+fragment");
      RemoveAllAfter(ref strUrl, "+proper");
      RemoveAllAfter(ref strUrl, "+limited");
      RemoveAllAfter(ref strUrl, "+rerip");
      RemoveAllAfter(ref strUrl, "+bluray");
      RemoveAllAfter(ref strUrl, "+brrip");
      RemoveAllAfter(ref strUrl, "+hddvd");
      RemoveAllAfter(ref strUrl, "+x264");
      RemoveAllAfter(ref strUrl, "+mbluray");
      RemoveAllAfter(ref strUrl, "+1080p");
      RemoveAllAfter(ref strUrl, "+720p");
      RemoveAllAfter(ref strUrl, "+480p");
      RemoveAllAfter(ref strUrl, "+r5");
      return strUrl;
    }
    /// <summary>
    /// cuts end of sting after strWord
    /// </summary>
    private static void RemoveAllAfter(ref string strLine, string strWord)
    {
      int iPos = strLine.IndexOf(strWord, System.StringComparison.Ordinal);
      if (iPos > 0)
      {
        strLine = strLine.Substring(0, iPos);
      }
    }

    private static bool HandleTrailer()
    {
      if (trailerPlayed)
      {

        // Set custom trailer played back to false
        trailerPlayed = false;

        // If a trailer was just played, we need to play the selected movie
        //playMovie(queuedMedia.AttachedMovies[0], queuedMedia.Part);
        return true;
      }
      return false;
    }

    public static int MovieDuration(ArrayList files)
    {
      int totalMovieDuration = 0;

      foreach (string file in files)
      {
        int fileId = VideoDatabase.GetFileId(file);
        int tempDuration = VideoDatabase.GetMovieDuration(fileId);

        if (tempDuration > 0)
        {
          totalMovieDuration += tempDuration;
        }
        else
        {
          var mInfo = new MediaInfoWrapper(file);

          if (fileId > -1)
            VideoDatabase.SetMovieDuration(fileId, mInfo.VideoDuration / 1000);
          totalMovieDuration += mInfo.VideoDuration / 1000;
        }
      }
      return totalMovieDuration;
    }

    public static MFMovie GetCurrentMovie()
    {
      return MyFilms.currentMovie;
    }

    public bool ReadMediaInfo(string file, ref MediaInfo mediainfo)
    {
      var MI = MediaInfo.GetInstance();

      // MediaInfo Object could not be created
      if (null == MI) return false;

      // Check if File Exists and is not an Image type e.g. ISO (we can't extract mediainfo from that)
      if (File.Exists(file) && !Helper.IsImageFile(file))
      {
        try
        {
          LogMyFilms.Debug("Attempting to read Mediainfo for ", file);

          // open file in MediaInfo
          MI.Open(file);

          // check number of failed attempts at mediainfo extraction                    
          int noAttempts = 0;

          // Get Playtime (runtime)
          string result = MI.VideoPlaytime;
          mediainfo = MI;

          string LocalPlaytime = result != "-1" ? result : noAttempts.ToString();

          bool failed = false;
          if (result != "-1")
          {
            string VideoCodec = MI.VideoCodec;
            string VideoFormat = MI.VideoCodecFormat;
            string VideoFormatProfile = MI.VideoFormatProfile;
            string VideoBitRate = MI.VideoBitrate;
            string VideoFrameRate = MI.VideoFramesPerSecond;
            string VideoWidth = MI.VideoWidth;
            string VideoHeight = MI.VideoHeight;
            string VideoAspectRatio = MI.VideoAspectRatio;

            string AudioCodec = MI.AudioCodec;
            string AudioFormat = MI.AudioCodecFormat;
            string AudioFormatProfile = MI.AudioFormatProfile;
            string AudioBitrate = MI.AudioBitrate;
            string AudioChannels = MI.AudioChannelCount;
            string AudioTracks = MI.AudioStreamCount;

            string TextCount = MI.SubtitleCount;

            // check for subtitles in mediainfo
            var files = new List<string>();
            files.Add(file);
            bool availableSubtitles = CheckHasSubtitles(files, true);
          }
          else
            failed = true;

          // MediaInfo cleanup
          MI.Close();

          if (failed)
          {
            // Get number of retries left to report to user
            int retries = 3 - (noAttempts * -1);

            string retriesLeft = retries > 0 ? retries.ToString() : "No";
            retriesLeft = string.Format("Problem parsing MediaInfo for: {0}, ({1} retries left)", file, retriesLeft);

            LogMyFilms.Debug(retriesLeft);
          }
          else
          {
            LogMyFilms.Debug("Succesfully read MediaInfo for ", file);
          }
          return true;
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("Error reading MediaInfo: ", ex.Message);
        }

      }
      LogMyFilms.Debug("File '" + file + "' does not exist or is an image file");
      return false;

    }

    private string GetFileTimeStamps(string file)
    {
      try
      {
        if (File.Exists(file))
        {
          string fileDateCreated = File.GetCreationTime(file).ToString("yyyy-MM-dd HH:mm:ss");
          return fileDateCreated;
        }
      }
      catch
      {
        LogMyFilms.Debug("Error: Unable to extract File Timestamps");
        string fileDateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return fileDateNow;
      }
      return string.Empty;
    }

    public bool CheckHasLocalSubtitles(List<string> filenames)
    {
      return CheckHasSubtitles(filenames, false);
    }

    public bool CheckHasSubtitles(List<string> filenames, bool useMediaInfo)
    {
      if (filenames.Count == 0)
        return false;

      int textCount = -1;
      if (useMediaInfo)
      {
        textCount = 0;
      }

      if (Helper.IsSubCentralAvailableAndEnabled)
      {
        return checkHasSubtitlesFromSubCentral(filenames, useMediaInfo, textCount);
      }

      return textCount > 0;
    }

    private bool checkHasSubtitlesFromSubCentral(List<string> files, bool useMediaInfo, int textCount)
    {
      LogMyFilms.Debug(string.Format("Using SubCentral for checkHasSubtitles(), useMediaInfo = {0}, textCount = {1}", useMediaInfo.ToString(), textCount.ToString()));
      List<FileInfo> fiFiles = files.Select(file => new FileInfo(file)).ToList();
      bool result = ExternalPlugins::SubCentral.Utils.SubCentralUtils.MediaHasSubtitles(fiFiles, false, textCount, !useMediaInfo);
      LogMyFilms.Debug(string.Format("SubCentral returned {0}", result));
      return result;
    }

    bool isWritable(FileInfo fileInfo)
    {
      FileStream stream = null;
      try
      {
        stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch
      {
        return true;
      }
      finally
      {
        if (stream != null) stream.Close();
      }
      return false;
    }

    public bool isWritable(string file)
    {
      if (string.IsNullOrEmpty(file)) return false;
      var fi = new FileInfo(file);
      return isWritable(fi);
    }

    private void GUIWindowManager_OnNewMessage(GUIMessage message)
    {
      //public enum MediaType // global category
      //{
      //  UNKNOWN = 0,
      //  PHOTO = 1,
      //  VIDEO = 2,
      //  AUDIO = 3
      //}
      //public enum MediaSubType // add here new formats
      //{
      //  UNKNOWN = 0,
      //  DVD = 1,
      //  AUDIO_CD = 2,
      //  BLURAY = 3,
      //  HDDVD = 4,
      //  VCD = 5,
      //  FILES = 6
      //}
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_AUTOPLAY_VOLUME:
          LogMyFilms.Debug("GUIWindowManager_OnNewMessage() - New Message received ! - MessageType = '" + message.Message + "', Param1 = '" + message.Param1 + "', Param2 = '" + message.Param2 + "', message label = '" + message.Label + "'");
          if (message.Param1 == (int)MediaPortal.Ripper.AutoPlay.MediaType.VIDEO)
          {
            switch (message.Param2)
            {
              case (int)AutoPlay.MediaSubType.AUDIO_CD:
              case (int)AutoPlay.MediaSubType.BLURAY:
                LogMyFilms.Debug("GUIWindowManager_OnNewMessage() - New Message received - 'AutoPlay.MediaSubType.BLURAY'");
                // GUIWindowManager.ActivateWindow(MyFilms.ID_BluRayPlayerLauncher);
                break;
              case (int)AutoPlay.MediaSubType.DVD:
                LogMyFilms.Debug("GUIWindowManager_OnNewMessage() - New Message received - 'AutoPlay.MediaSubType.DVD'");
                // OnPlayDVD(message.Label, GetID);
                break;
              case (int)AutoPlay.MediaSubType.FILES:
              case (int)AutoPlay.MediaSubType.VCD:
                LogMyFilms.Debug("GUIWindowManager_OnNewMessage() - New Message received - 'AutoPlay.MediaSubType.VCD' or 'AutoPlay.MediaSubType.FILES'");
                // OnPlayFiles((System.Collections.ArrayList)message.Object);
                break;
              case (int)AutoPlay.MediaSubType.HDDVD:
              case (int)AutoPlay.MediaSubType.UNKNOWN:
                break;
            }
          }
          break;
      }
    }

    #region External Player

    private static void LaunchExternalPlayer(string videoPath)
    {
      LogMyFilms.Debug("LaunchExternalPlayer() - Launching external player.");

      // First check if the user supplied executable for the external player is valid
      string execPath = MyFilms.conf.ExternalPlayerPath;
      if (!File.Exists(execPath))
      {
        // if it's not show a dialog explaining the error
        GUIUtils.ShowNotifyDialog("Error", "MissingExternalPlayerExe");
        LogMyFilms.Warn("The external player executable '{0}' is missing.", execPath);
        // do nothing
        resetPlayer();
        return;
      }

      // process the argument string and replace the 'filename' variable
      string arguments = MyFilms.conf.ExternalPlayerStartParams;
      string videoRoot = Utility.GetMovieBaseDirectory(new FileInfo(videoPath).Directory).FullName;
      string filename = Utility.IsDriveRoot(videoRoot) ? videoRoot : videoPath;
      double fps = GetFPS(videoPath);
      string virtualDrive = DaemonTools.GetVirtualDrive();
      if (virtualDrive.LastIndexOf("\\") != virtualDrive.Length - 1) virtualDrive = virtualDrive.Trim() + "\\";

      arguments = arguments.Replace("%filename%", filename);
      arguments = arguments.Replace("%fps%", fps.ToString(CultureInfo.InvariantCulture));
      arguments = arguments.Replace("%root%", videoRoot);
      arguments = arguments.Replace("%drive%", virtualDrive);

      LogMyFilms.Debug("External Player: Video='{0}', Root={1}, FPS={2}, ExecPath={3}, CommandLine={4}, VirtualDrive={5}", filename, videoRoot, fps.ToString(CultureInfo.InvariantCulture), execPath, arguments, virtualDrive);
      LogMyFilms.Debug("Command Line: '" + arguments + "'");

      // Set Refresh Rate Based On FPS if needed
      bool UseDynamicRefreshRateChangerWithExternalPlayer = true; // Todo: add setting "Settings.UseDynamicRefreshRateChangerWithExternalPlayer"
      if (fps > 0 && UseDynamicRefreshRateChangerWithExternalPlayer)
      {
        LogMyFilms.Info("Requesting new refresh rate: FPS={0}", fps.ToString(CultureInfo.InvariantCulture));
        RefreshRateChanger.SetRefreshRateBasedOnFPS(fps, filename, RefreshRateChanger.MediaType.Video);
        if (RefreshRateChanger.RefreshRateChangePending)
        {
          TimeSpan ts = DateTime.Now - RefreshRateChanger.RefreshRateChangeExecutionTime;
          if (ts.TotalSeconds > RefreshRateChanger.WAIT_FOR_REFRESHRATE_RESET_MAX)
          {
            LogMyFilms.Info("Refresh rate change failed. Please check your mediaportal log and configuration", RefreshRateChanger.WAIT_FOR_REFRESHRATE_RESET_MAX);
            RefreshRateChanger.ResetRefreshRateState();
          }
        }
      }

      // Setup the external player process
      ProcessStartInfo processinfo = new ProcessStartInfo();
      processinfo.FileName = execPath;
      processinfo.Arguments = arguments;

      Process hdPlayer = new Process();
      hdPlayer.StartInfo = processinfo;
      hdPlayer.Exited += OnHDPlayerExited;
      hdPlayer.EnableRaisingEvents = true;

      try
      {
        // start external player process
        hdPlayer.Start();

        // disable mediaportal input devices
        MediaPortal.InputDevices.InputDevices.Stop();

        // hide mediaportal and suspend rendering to save resources for the external player
        GUIGraphicsContext.BlankScreen = true;
        GUIGraphicsContext.form.Hide();
        GUIGraphicsContext.CurrentState = GUIGraphicsContext.State.SUSPENDING;

        LogMyFilms.Info("HD Playback: External player started.");
        // onMediaStarted(queuedMedia);
      }
      catch (Exception e)
      {
        LogMyFilms.Error(e, "HD Playback: Could not start the external player process.");
        resetPlayer();
      }
    }

    private static void OnHDPlayerExited(object obj, EventArgs e)
    {
      // Restore refresh rate if it was changed
      bool UseDynamicRefreshRateChangerWithExternalPlayer = true; // Todo: add setting "Settings.UseDynamicRefreshRateChangerWithExternalPlayer"
      if (UseDynamicRefreshRateChangerWithExternalPlayer && RefreshRateChanger.RefreshRateChangePending)
        RefreshRateChanger.AdaptRefreshRate();

      // enable mediaportal input devices
      MediaPortal.InputDevices.InputDevices.Init();

      // show mediaportal and start rendering
      GUIGraphicsContext.BlankScreen = false;
      GUIGraphicsContext.form.Show();
      GUIGraphicsContext.ResetLastActivity();
      GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GETFOCUS, 0, 0, 0, 0, 0, null);
      GUIWindowManager.SendThreadMessage(msg);
      GUIGraphicsContext.CurrentState = GUIGraphicsContext.State.RUNNING;
      LogMyFilms.Info("HD Playback: The external player has exited.");
    }

    private static void resetPlayer()
    {
      //// If we have an image mounted, unmount it
      //if (mountedPlayback)
      //{
      //  queuedMedia.UnMount();
      //  mountedPlayback = false;
      //}

      // reset player variables

      if (GUIGraphicsContext.IsFullScreenVideo)
        GUIGraphicsContext.IsFullScreenVideo = false;

      //activeMedia = null;
      //queuedMedia = null;
      //_playerState = MoviePlayerState.Idle;
      //_resumeActive = false;
      //listenToExternalPlayerEvents = false;
      //donePlayingCustomIntros = false;
      //customIntrosPlayed = 0;

      LogMyFilms.Debug("Reset.");
    }

    /// <summary>
    /// Disable MediaPortal AutoPlay
    /// </summary>
    private static void disableNativeAutoplay()
    {
      LogMyFilms.Info("Disabling native autoplay.");
      AutoPlay.StopListening();
    }

    /// <summary>
    /// Enable MediaPortal AutoPlay
    /// </summary>
    private static void enableNativeAutoplay()
    {
      if (GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
      {
        LogMyFilms.Info("Re-enabling native autoplay.");
        AutoPlay.StartListening();
      }
    }

    /// <summary>
    /// Get refresh rate for file
    /// </summary>
    private static double GetFPS(string file)
    {
      LogMyFilms.Debug("GetFPS - Get RefreshRate for file '" + file + "'");
      if (!File.Exists(file))
      {
        return -1;
      }

      double fps = -1;
      string bdFramerate;
      try
      {
        MediaInfo mi = new MediaInfo();
        mi.Open(file);
        bdFramerate = mi.Get(Grabber.Util.StreamKind.Video, 0, "FrameRate");

        LogMyFilms.Info("GetFPS() - Framerate via Mediainfo: '{0}'", bdFramerate);

        switch (bdFramerate)
        {
          case "23.976":
            fps = 23.976;
            break;
          case "24":
            fps = 24;
            break;
          case "24.000":
            fps = 24;
            break;
          case "25":
            fps = 25;
            break;
          case "25.000":
            fps = 25;
            break;
          case "29.970":
            fps = 29.97;
            break;
          case "50":
            fps = 50;
            break;
          case "50.000":
            fps = 50;
            break;
          case "59.94":
            fps = 59.94;
            break;
        }
      }
      catch (Exception e)
      {
        LogMyFilms.Error("GetFPS() - failed to get refresh rate from disk!");
        LogMyFilms.Error(e, "GetFPS() - exception {0}", e.Message);
        return -1;
      }
      LogMyFilms.Debug("GetFPS - bdFramerate = '" + bdFramerate + "', -> fps = '" + fps + "'");
      return fps;
    }

    #endregion

    public static void AddPersonsToDownloadQueue() // add persons of current movie to download queue
    {
      new Thread(delegate()
      {
        {
          try
          {
            if (!Helper.IsConnectedToInternet()) return;

            if (MyFilms.conf.UseThumbsForPersons && Directory.Exists(MyFilms.conf.StrPathArtist) && MyFilms.r != null && MyFilms.r.Length > MyFilms.conf.StrIndex)
            {
              string persons = MyFilms.r[MyFilms.conf.StrIndex]["Persons"].ToString();
              ArrayList wTableau = MyFilms.Search_String(persons);
              foreach (string personsname in from object t in wTableau select t.ToString())
              {
                if (!(File.Exists(MyFilms.conf.StrPathArtist + "\\" + personsname + ".jpg")))
                {
                  if (DownloadPersonImage(personsname))
                    LogMyFilms.Debug("Person '" + personsname + "' added to downloadQueue !");
                  else
                    LogMyFilms.Debug("Person '" + personsname + "' already in downloadQueue !");
                }
                else LogMyFilms.Debug("Person '" + personsname + "' NOT added to downloadQueue - image already exists !");
              }
              // if (PersonstoDownloadQueue.Count > 0 && !downloadingWorker.IsBusy) downloadingWorker.RunWorkerAsync(); // already doine in submethod !
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("AddPersonsToDownloadQueue() - Error adding persons to download queue: " + ex.Message);
          }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // this after thread finished ...
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsPersonToDLqueueLoader", IsBackground = true }.Start();
    }

    private static bool DownloadPersonImage(string personname) //void downloadPersonImage(DBPersonInfo person)
    {
      bool added = false;
      // we need to get it, let's queue them up and download in the background
      var person = new DbPersonInfo { Name = personname };
      lock (PersonstoDownloadQueue)
      {
        if (PersonstoDownloadQueue.Any(personInfo => personInfo.Name == personname))
        {
          return false;
        }
        if (!PersonstoDownloadQueue.Contains(person))
        {
          PersonstoDownloadQueue.Enqueue(person);
          added = true;
        }
      }
      setDownloadStatus();
      if (!downloadingWorker.IsBusy) downloadingWorker.RunWorkerAsync(); // finally lets check if the downloader is already running, and if not start it
      return added;
    }

    internal static bool UpdatePersonDetails(string personname, GUIListItem item, bool forceupdate, bool stopLoadingViewDetails)
    {
      if (!forceupdate && !Helper.PersonUpdateAllowed(MyFilms.conf.StrPathArtist, personname)) return false;

      string item1LabelOrg = (item != null) ? item.Label : "";
      string item3LabelOrg = (item != null) ? item.Label3 : "";
      string filename = MyFilms.conf.StrPathArtist + "\\" + personname + ".jpg";  // string filename = Path.Combine(MyFilms.conf.StrPathArtist, personname); //File.Exists(MyFilms.conf.StrPathArtist + "\\" + personsname + ".jpg")))

      IMDBActor person = null;
      bool vdBexists = false;

      if (item != null) item.Label = item1LabelOrg + " " + GUILocalizeStrings.Get(10799205); // (updating...)

      #region get person info from VDB
      if (item != null) item.Label3 = "Loading details from VDB ...";
      var actorList = new ArrayList();
      VideoDatabase.GetActorByName(personname, actorList);
      LogMyFilms.Debug("VDB - found '" + actorList.Count + "' local results for '" + personname + "'");
      if (actorList.Count > 0 && actorList.Count < 5)
      {
        LogMyFilms.Debug("VDB first search result: '" + actorList[0] + "'");
        string[] strActor = actorList[0].ToString().Split(new char[] { '|' });
        // int actorID = (strActor[0].Length > 0 && strActor.Count() > 1) ? Convert.ToInt32(strActor[0]) : 0; // string actorname = strActor[1];
        int actorId;
        int.TryParse(strActor[0], out actorId);
        if (actorId > 0)
        {
          person = VideoDatabase.GetActorInfo(actorId);
        }
        if (person != null)
        {
          if (item != null) item.Label3 = "ID = " + actorId + ", URL = " + person.ThumbnailUrl;
          vdBexists = true;
        }
        else
        {
          if (item != null) item.Label3 = "ID = " + actorId;
        }
      }
      #endregion

      if (person != null && File.Exists(filename) && !forceupdate && !person.Biography.ToLower().StartsWith("unknown") && !(person.Biography.Length > 8))
      {
        LogMyFilms.Debug("Skip update for '" + personname + "' - VDB entry and image already present !");
        if (item != null)
        {
          item.MusicTag = person;
          item.Label = item1LabelOrg;
          item.Label3 = item3LabelOrg;
        }
        if (stopLoadingViewDetails && item != null) return false; // stop download if we have exited window
        return true; // nothing to do
      }

      // region update person detail infos or load new ones ...
      if (person == null || person.DateOfBirth.Length < 1 || person.DateOfBirth.ToLower().StartsWith("unknown") || !File.Exists(filename) || forceupdate)
      {
        if (person == null) person = new IMDBActor();

        #region IMDB internet search
        if (item != null) item.Label3 = "Searching IMDB ...";

        string grabberscript = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB-Person.xml";
        if (File.Exists(grabberscript))
        {
          ArrayList personUrls = FindActor(personname, grabberscript);
          LogMyFilms.Debug("IMDB - " + personUrls.Count + " person(s) found for '" + personname + "' with person grabber script '" + grabberscript + "'"); 
          if (personUrls.Count > 0)
          {
            var wurl = (Grabber_URLClass.IMDBUrl)personUrls[0];
            if (wurl.URL.Length != 0)
            {
              if (item != null) item.Label3 = "Loading IMDB details ...";
              GrabActorDetails(wurl.URL, grabberscript, out person);
              LogMyFilms.Debug("IMDB - Value found - name       = '" + (person.Name ?? "") + "'");
              LogMyFilms.Debug("IMDB - Value found - birthday   = '" + (person.DateOfBirth ?? "") + "'");
              LogMyFilms.Debug("IMDB - Value found - birthplace = '" + (person.PlaceOfBirth ?? "") + "'");
              LogMyFilms.Debug("IMDB - Value found - biography  = '" + (person.Biography.Substring(0, Math.Min(person.Biography.Length, 100)) ?? "") + "'");
              LogMyFilms.Debug("IMDB - Value found - thumb url  = '" + (person.ThumbnailUrl ?? "") + "'");
            }
          }
        }
        else
        {
          LogMyFilms.Debug("IMDB - Default person grabber script not found (" + grabberscript + ")");
        }
        
        //var imdb = new IMDB();
        //imdb.FindActor(personname);
        //LogMyFilms.Debug("IMDB - " + imdb.Count + " person(s) found for '" + personname + "' with IMDB API");
        //if (imdb.Count > 0)
        //{
        //  if (imdb[0].URL.Length != 0)
        //  {
        //    if (item != null) item.Label3 = "Loading IMDB details ...";
        //    //#if MP1X
        //    // _imdb.GetActorDetails(_imdb[0], out person);
        //    //#else
        //    // _imdb.GetActorDetails(_imdb[0], false, out person);
        //    //#endif
        //    GUIUtils.GetActorDetails(imdb, imdb[0], false, out person);
        //    LogMyFilms.Debug("IMDB - Value found - name       = '" + (person.Name ?? "") + "'");
        //    LogMyFilms.Debug("IMDB - Value found - birthday   = '" + (person.DateOfBirth ?? "") + "'");
        //    LogMyFilms.Debug("IMDB - Value found - birthplace = '" + (person.PlaceOfBirth ?? "") + "'");
        //    LogMyFilms.Debug("IMDB - Value found - biography  = '" + (person.Biography.Substring(0, Math.Min(person.Biography.Length, 100)) ?? "") + "'");
        //  }
        //}

        if (stopLoadingViewDetails && item != null && !forceupdate) return false; // stop download if we have exited window

        #endregion

        #region TMDB V3 API description
        // Search
        //TmdbMovieSearch SearchMovie(string query, int page)
        //TmdbPersonSearch SearchPerson(string query, int page)
        //TmdbCompanySearch SearchCompany(string query, int page);             

        // Person Info
        //TmdbPerson GetPersonInfo(int PersonID)
        //TmdbPersonCredits GetPersonCredits(int PersonID)
        //TmdbPersonImages GetPersonImages(int PersonID)
        //Movie Info
        //TmdbMovie GetMovieInfo(int MovieID)
        //TmdbMovie GetMovieByIMDB(string IMDB_ID)
        //TmdbMovieAlternateTitles GetMovieAlternateTitles(int MovieID, string Country)
        //TmdbMovieCast GetMovieCast(int MovieID)
        //TmdbMovieImages GetMovieImages(int MovieID)
        //TmdbMovieKeywords GetMovieKeywords(int MovieID)
        //TmdbMovieReleases GetMovieReleases(int MovieID)
        //TmdbMovieTrailers GetMovieTrailers(int MovieID)
        //TmdbSimilarMovies GetSimilarMovies(int MovieID, int page)
        //TmdbTranslations GetMovieTranslations(int MovieID)

        // Social Movie Info
        //TmdbNowPlaying GetNowPlayingMovies(int page)
        //TmdbPopular GetPopularMovies(int page)
        //TmdbTopRated GetTopRatedMovies(int page)
        //TmdbUpcoming GetUpcomingMovies(int page)
        #endregion

        #region TMDB v3 infos ...
        try
        {
          if (item != null) item.Label3 = "Searching TMDB ...";

          // grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb(); // we're using new v3 api here
          var api = new Tmdb(MyFilms.TmdbApiKey, CultureInfo.CurrentCulture.Name.Substring(0, 2)); // language is optional, default is "en"
          TmdbConfiguration tmdbConf = api.GetConfiguration();
          TmdbPersonSearch tmdbPerson = api.SearchPerson(personname, 1);
          List<PersonResult> persons = tmdbPerson.results;
          LogMyFilms.Debug("TMDB - " + persons.Count + " person(s) found for '" + personname + "' with TMDB API");
          if (persons != null && persons.Count > 0)
          {
            PersonResult pinfo = persons[0];
            if (item != null) item.Label3 = "Loading TMDB details ...";
            TmdbPerson singleperson = api.GetPersonInfo(pinfo.id);
            
            // TMDB.TmdbPersonImages images = api.GetPersonImages(pinfo.id);
            // TMDB.TmdbPersonCredits personFilmList = api.GetPersonCredits(pinfo.id);

            LogMyFilms.Debug("TMDB - Value found - birthday   = '" + (singleperson.birthday ?? "") + "'");
            LogMyFilms.Debug("TMDB - Value found - birthplace = '" + (singleperson.place_of_birth ?? "") + "'");
            LogMyFilms.Debug("TMDB - Value found - biography  = '" + ((!string.IsNullOrEmpty(singleperson.biography)) ? singleperson.biography.Substring(0, Math.Min(singleperson.biography.Length, 100)) : "") + "'");

            SetActorDetailsFromTmdb(singleperson, tmdbConf, ref person);

            if (!string.IsNullOrEmpty(singleperson.profile_path) && !File.Exists(filename))
            {
              if (item != null) item.Label3 = "Loading TMDB image ...";
              string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, person.ThumbnailUrl, personname, false, true, out filename);
              LogMyFilms.Debug("Person Image (TMDB) '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + personname + "', path = '" + filename1person + "', filename = '" + filename + "'");
              if (item != null)
              {
                item.IconImage = filename;
                item.IconImageBig = filename;
                item.ThumbnailImage = filename;
                item.Label3 = "TMDB ID = " + singleperson.id + ", URL = " + singleperson.profile_path;
              }
            }
          }
        }
        catch (Exception tex)
        {
          LogMyFilms.Debug(tex, "UpdatePersonDetails() - error in TMDB grabbing person '" + personname + "': " + tex.Message);
        }
        if (stopLoadingViewDetails && item != null && !forceupdate) return false; // stop download if we have exited window
        #endregion

        #region Add actor to database to get infos in person facades later...
        if (item != null) item.Label3 = "Save detail info to VDB ...";
        try
        {
          //#if MP1X
          //                  int actorId = VideoDatabase.AddActor(person.Name);
          //#else
          //                  int actorId = VideoDatabase.AddActor(null, person.Name);
          //#endif
          int actorId = GUIUtils.AddActor(null, person.Name);
          if (actorId > 0)
          {
            if (!string.IsNullOrEmpty(person.Biography)) // clean up before saving ...
            {
              if (person.Biography.Contains("Wikipedia"))
              {
                string startwiki = "From Wikipedia, the free encyclopedia";
                string endwiki = "Description above from the Wikipedia article";
                person.Biography = person.Biography.TrimStart('?', '.', ' ', '\r', '\n').TrimStart();
                if (person.Biography.Contains(startwiki)) person.Biography = person.Biography.Substring(person.Biography.IndexOf(startwiki) + startwiki.Length + 1);
                if (person.Biography.Contains(endwiki)) person.Biography = person.Biography.Substring(0, person.Biography.LastIndexOf(endwiki));
                person.Biography = person.Biography.Trim().TrimStart('?', '.', ' ', '\r', '\n').TrimEnd('\r', '\n', ' ').Trim(new char[] { ' ', '\r', '\n' }).Trim();
                LogMyFilms.Debug("Biography - cleaned value = '" + person.Biography + "'");
              }
            }
            VideoDatabase.SetActorInfo(actorId, person);
            //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
            if (item != null) item.Label3 = (vdBexists) ? ("Updated ID" + actorId + ", URL = " + person.ThumbnailUrl) : ("Added ID" + actorId + ", URL = " + person.ThumbnailUrl);
          }
        }
        catch (Exception ex)
        {
          if (item != null) item.Label = item1LabelOrg;
          LogMyFilms.Debug("Error adding person to VDB: " + ex.Message, ex.StackTrace);
        }
        if (stopLoadingViewDetails && item != null && !forceupdate) return false; // stop download if we have exited window
        #endregion

        #region load missing images ...
        if ((person.ThumbnailUrl.Contains("http:") && !File.Exists(filename)) || forceupdate)
        {
          #region MP Thumb download deactivated, as downloading not yet working !!!
          //if (person.ThumbnailUrl != string.Empty) // to update MP person thumb dir
          //{
          //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, person.Name);
          //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, person.Name);
          //  Utils.FileDelete(largeCoverArt);
          //  Utils.FileDelete(coverArt);
          //  IMDBFetcher.DownloadCoverArt(Thumbs.MovieActors, person.ThumbnailUrl, person.Name);
          //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
          //}
          #endregion
          if (item != null) item.Label3 = "Loading image ...";
          LogMyFilms.Debug(" Image found for person '" + personname + "', URL = '" + person.ThumbnailUrl + "'");
          string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, person.ThumbnailUrl, personname, false, true, out filename);
          LogMyFilms.Debug("Person Image '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + personname + "', path = '" + filename1person + "', filename = '" + filename + "'");

          string strThumbDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons\";
          string strThumb = strThumbDirectory + personname + ".png";
          string strThumbSmall = strThumbDirectory + personname + "_s.png";
          if (File.Exists(filename) && (forceupdate || !File.Exists(strThumbSmall)))
          {
            if (item != null) item.Label3 = "Creating cache image ...";
            //Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
            MyFilms.CreateCacheThumb(filename, strThumbSmall, 100, 150, "small");
            //Picture.CreateThumbnail(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
            MyFilms.CreateCacheThumb(filename, strThumb, MyFilms.cacheThumbWith, MyFilms.cacheThumbHeight, "large");
          }

          if (item != null)
          {
            if (File.Exists(strThumb)) // (re)check if thumbs exist...
            {
              item.IconImage = strThumbSmall;
              item.IconImageBig = strThumb;
              item.ThumbnailImage = strThumb;
            }
            else if (File.Exists(filename))
            {
              item.IconImage = filename;
              item.IconImageBig = filename;
              item.ThumbnailImage = filename;
            }
            else
            {
              {
                item.IconImage = MyFilms.conf.DefaultCoverArtist;
                item.IconImageBig = MyFilms.conf.DefaultCoverArtist;
                item.ThumbnailImage = MyFilms.conf.DefaultCoverArtist;
              }
            }

            item.Label3 = "URL = " + person.ThumbnailUrl;
            // item.NotifyPropertyChanged("ThumbnailImage");
          }
        }
        #endregion

        if (item != null)
        {
          item.MusicTag = person;
        }

        #region old stuff deactivated
        //string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson, currentCustomView, defaultViewImage, reversenames);
        ////string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
        ////if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
        ////{
        ////  item.ThumbnailImage = texture;
        ////  item.IconImage = texture;
        ////  item.IconImageBig = texture;
        ////}

        //item.IconImage = strActiveFacadeImages[1];
        //item.IconImageBig = strActiveFacadeImages[0];
        //item.ThumbnailImage = strActiveFacadeImages[0];

        //// if selected force an update of thumbnail
        ////GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
        ////if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
        #endregion
      }

      if (item != null) // reset to original values
      {
        item.Label = item1LabelOrg;
        item.Label3 = item3LabelOrg;
      }
      return true;
    }

    private static void SetActorDetailsFromTmdb(TmdbPerson tmdbPerson, TmdbConfiguration conf, ref IMDBActor imdbPerson)
    {
      if (tmdbPerson == null)
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - TMDB person is 'null' - return");
        return;
      }
      if (imdbPerson == null)
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - IMDB person is 'null' - return");
        return;
      }
      string tmdbProfileSize = "original";
      foreach (string profileSize in conf.images.profile_sizes.Where(profileSize => profileSize == "h632"))
      {
        tmdbProfileSize = profileSize;
      }

      // imdbPerson.IMDBActorID = 
      // imdbPerson.Name = tmdbPerson.name;
      // imdbPerson.MiniBiography = tmdbPerson.biography;
      // LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB name     - old : '" + imdbPerson.Name + "', new: '" + tmdbPerson.name + "'");
      if (!string.IsNullOrEmpty(tmdbPerson.biography) && tmdbPerson.biography.Length > imdbPerson.Biography.Length)
      {
        // LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB bio      - old : '" + imdbPerson.Biography + "', new: '" + tmdbPerson.biography + "'");
        imdbPerson.Biography = tmdbPerson.biography;
      }

      if (!string.IsNullOrEmpty(tmdbPerson.birthday))
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB birthday - old : '" + imdbPerson.DateOfBirth + "', new: '" + tmdbPerson.birthday + "'");
        imdbPerson.DateOfBirth = tmdbPerson.birthday + ((!string.IsNullOrEmpty(tmdbPerson.deathday)) ? " (" + tmdbPerson.deathday + ")" : "");
      }
      if (!string.IsNullOrEmpty(tmdbPerson.place_of_birth))
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB b-place  - old : '" + imdbPerson.PlaceOfBirth + "', new: '" + tmdbPerson.place_of_birth + "'");
        imdbPerson.PlaceOfBirth = tmdbPerson.place_of_birth;
      }
      if (!string.IsNullOrEmpty(tmdbPerson.profile_path))
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB thumb    - old : '" + imdbPerson.ThumbnailUrl + "', new: '" + conf.images.base_url + tmdbProfileSize + tmdbPerson.profile_path + "'");
        imdbPerson.ThumbnailUrl = conf.images.base_url + tmdbProfileSize + tmdbPerson.profile_path;
      }
    }

    internal static void LoadCollectionImages(DataRow[] r1, int index, bool interactive, GUIAnimation animation)
    {
      if (!File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsDialogImageSelect.xml"))
      {
        if (interactive) GUIUtils.ShowNotifyDialog("Missing Skin File");
        return;
      }
      // var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var dlgMenuOrg = (GUIWindow)GUIWindowManager.GetWindow(2012);
      var dlg = new GUIDialogImageSelect();
      if (dlg == null) return;
      dlg.Init();
      GUIWindowManager.Replace(2009, dlg);

      //var dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      //var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      //var dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);

      new Thread(delegate(object o)
      {
        try
        {
          SetProcessAnimationStatus(true, animation);
          #region select and load collection image
          string titlename = Helper.TitleWithoutGroupName(r1[index][MyFilms.conf.StrTitle1].ToString());
          string titlename2 = (Helper.FieldIsSet(MyFilms.conf.StrTitle2)) ? Helper.TitleWithoutGroupName(r1[index][MyFilms.conf.StrTitle2].ToString()) : "";
          string collectionname = Helper.TitleFirstGroupName(r1[index][MyFilms.conf.StrTitle1].ToString());
          string collectionSafeName = GrabUtil.CreateFilename(collectionname);

          string path;
          #region Retrieve original directory of mediafiles
          try
          {
            path = r1[index][MyFilms.conf.StrStorage].ToString();
            if (path.Contains(";")) path = path.Substring(0, path.IndexOf(";", StringComparison.Ordinal));
            //path = Path.GetDirectoryName(path);
            //if (path == null || !Directory.Exists(path))
            //{
            //  LogMyFilms.Warn("Directory of movie '" + titlename + "' doesn't exist anymore - check your DB");
            //  return;
            //}
            if (path.Contains("\\")) path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));

            // LogMyFilms.Debug("(SearchAndDownloadTrailerOnlineTMDB) get media directory name: '" + path + "'");
          }
          catch (Exception)
          {
            LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB() error with directory of movie '" + titlename + "' - check your DB");
            return;
          }
          #endregion

          string imdb = "";
          #region get imdb number for better search match
          if (!string.IsNullOrEmpty(r1[index]["IMDB_Id"].ToString()))
            imdb = r1[index]["IMDB_Id"].ToString();
          else if (!string.IsNullOrEmpty(r1[index]["URL"].ToString()))
          {
            string urLstring = r1[index]["URL"].ToString();
            var cutText = new Regex("" + @"tt\d{7}" + "");
            var m = cutText.Match(urLstring);
            if (m.Success) imdb = m.Value;
          }
          #endregion

          int year;

          LogMyFilms.Debug("LoadCollectionImages() - movietitle = '" + titlename + "', collectionname = '" + collectionSafeName + "', collectionSafeName = '" + collectionSafeName + "', interactive = '" + interactive + "'");

          string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
          var api = new Tmdb(MyFilms.TmdbApiKey, language); // language is optional, default is "en"
          TmdbConfiguration tmdbConf = api.GetConfiguration();
          foreach (string size in tmdbConf.images.poster_sizes) LogMyFilms.Debug("Available TMDB Poster Size: '" + size + "'");
          // foreach (string size in tmdbConf.images.backdrop_sizes) LogMyFilms.Debug("Available TMDB Backdrop Size: '" + size + "'");
          try
          {
            int selectedMovieId = 0;

            #region search matching TMDB movie id
            if (imdb.Contains("tt"))
            {
              TmdbMovie movie = api.GetMovieByIMDB(imdb);
              if (movie.id > 0)
              {
                selectedMovieId = movie.id;
              }
            }

            if (selectedMovieId == 0) // no movie found by tmdb search
            {
              TmdbMovieSearch moviesfound;
              if (int.TryParse(r1[index]["Year"].ToString(), out year))
              {
                moviesfound = api.SearchMovie(titlename, 1, "", null, year);
                if (moviesfound.results.Count == 0) moviesfound = api.SearchMovie(titlename, 1, null);
              }
              else
              {
                moviesfound = api.SearchMovie(r1[index][MyFilms.conf.StrTitle1].ToString(), 1, null);
                if (moviesfound.results.Count == 0 && titlename2.Length > 0)
                {
                  if (int.TryParse(r1[index]["Year"].ToString(), out year))
                  {
                    moviesfound = api.SearchMovie(titlename2, 1, "", null, year);
                    if (moviesfound.results.Count == 0) moviesfound = api.SearchMovie(titlename2, 1, null);
                  }
                }
              }
              SetProcessAnimationStatus(false, animation);

              if (moviesfound.results.Count == 1 && !interactive)
              {
                selectedMovieId = moviesfound.results[0].id;
              }
              else
              {
                LogMyFilms.Debug("LoadCollectionImages() - Movie Search Results: '" + moviesfound.total_results.ToString() + "' for movie '" + titlename + "'");
                if (!interactive) return;
                else
                {
                  if (moviesfound.results.Count == 0)
                  {
                    while (selectedMovieId == 0)
                    {
                      selectedMovieId = SearchTmdbMovie(titlename, titlename, titlename2, year, language, true);
                      if (selectedMovieId == -1) return; // cancel search
                    }
                  }
                  else
                  {
                    var choiceMovies = new List<TmdbMovie>();
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798992)); // Select movie ...

                    foreach (TmdbMovie movieResult in moviesfound.results)
                    {
                      dlg.Add(movieResult.title + " (" + movieResult.release_date + ")");
                      choiceMovies.Add(movieResult);
                    }
                    dlg.DoModal(GUIWindowManager.ActiveWindow);
                    if (dlg.SelectedLabel == -1) return;
                    selectedMovieId = choiceMovies[dlg.SelectedLabel].id;
                  }
                }
              }

              // now load the artwork
            }
            #endregion

            if (selectedMovieId == 0)
            {
              LogMyFilms.Debug("SearchAndDownloadTrailerOnlineTMDB - no movie found - no trailers added to DL queue - returning");
              if (interactive) GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798995)); // No matching movie found !
              return;
            }

            SetProcessAnimationStatus(true, animation);
            #region now load the details and collection images info
            TmdbMovie tmdbMovie = api.GetMovieInfo(selectedMovieId);
            TmdbCollection collection = api.GetCollectionInfo(tmdbMovie.belongs_to_collection.id);
            var collectionPosters = new List<CollectionPoster>();
            var backdrops = new List<CollectionBackdrop>();

            LogMyFilms.Debug("TMDB - Value found - movie   = '" + (tmdbMovie.title ?? "") + "'" + " (" + tmdbMovie.release_date + ")");
            LogMyFilms.Debug("TMDB - Value found - belongs to collection id = '" + collection.id.ToString() + "', name  = '" + (collection.name ?? "") + "'");

            TmdbCollectionImages collectionImages = api.GetCollectionImages(collection.id, language);
            LogMyFilms.Debug("TMDB - Collection Posters found for language = '" + language + "' : '" + collectionImages.posters.Count + "'");
            collectionPosters.AddRange(collectionImages.posters);
            backdrops.AddRange(collectionImages.backdrops);
            collectionImages = api.GetCollectionImages(collection.id, null);
            LogMyFilms.Debug("TMDB - Collection Posters found: '" + collectionImages.posters.Count + "'");
            collectionPosters.AddRange(collectionImages.posters);
            backdrops.AddRange(collectionImages.backdrops);

            collectionPosters.Distinct();
            backdrops.Distinct();

            //foreach (CollectionBackdrop backdrop in backdrops)
            //{
            //  string fanartUrl = tmdbConf.images.base_url + "original" + backdrop.file_path;
            //  LogMyFilms.Debug("TMDB - Backdrop found = '" + fanartUrl + "'");
            //}
            #endregion

            if (collectionPosters.Count == 0)
            {
              SetProcessAnimationStatus(false, animation);
              GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798760), GUILocalizeStrings.Get(10798625)); // load group covers, no result found
              return;
            }

            #region load collection images into selection menu
            var choicePosters = new List<CollectionPoster>();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798760)); // Load collection cover (Tmdb)
            foreach (CollectionPoster poster in collectionPosters)
            {
              string posterUrlSmall = tmdbConf.images.base_url + "w154" + poster.file_path;
              string posterUrl = tmdbConf.images.base_url + "w500" + poster.file_path;
              LogMyFilms.Debug("TMDB - Collection Poster found = '" + posterUrl + "'");
              var item = new GUIListItem();
              item.Label = poster.width + " x " + poster.height;
              item.IconImage = posterUrlSmall;
              item.ThumbnailImage = posterUrl;
              dlg.Add(item);
              choicePosters.Add(poster);
            }
            #endregion
            SetProcessAnimationStatus(false, animation);

            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedLabel == -1) return;
            CollectionPoster selectedPoster = choicePosters[dlg.SelectedLabel];

            #region load collection cover images and fanart
            SetProcessAnimationStatus(true, animation);
            try
            {
              #region Poster

              string localThumb = MyFilms.conf.StrPicturePrefix.Length > 0
                             ? MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + collectionSafeName + ".jpg"
                             : Path.Combine(MyFilms.conf.StrPathImg, collectionSafeName + ".jpg");
              string remoteThumb = tmdbConf.images.base_url + "w500" + selectedPoster.file_path;
              LogMyFilms.Debug("GetImagesTMDB() - localThumb = '" + localThumb + "'");
              LogMyFilms.Debug("GetImagesTMDB() - remoteThumb = '" + remoteThumb + "'");

              if (!string.IsNullOrEmpty(remoteThumb) && !string.IsNullOrEmpty(localThumb))
              {
                if (File.Exists(localThumb)) try { File.Delete(localThumb); }
                  catch (Exception) { }
                Thread.Sleep(10);
                if (GrabUtil.DownloadImage(remoteThumb, localThumb))
                {
                  string coverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";
                  string strThumb = MediaPortal.Util.Utils.GetCoverArtName(coverThumbDir, collectionSafeName); // cached cover
                  if (!string.IsNullOrEmpty(strThumb) && strThumb.Contains("."))
                  {
                    string strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".", StringComparison.Ordinal)) + "_s" + Path.GetExtension(strThumb);  // cached cover for Icons - small resolution
                    if (!string.IsNullOrEmpty(localThumb) && localThumb != MyFilms.conf.DefaultCover)
                    {
                      Picture.CreateThumbnail(localThumb, strThumbSmall, 100, 150, 0, Thumbs.SpeedThumbsSmall);
                      Picture.CreateThumbnail(localThumb, strThumb, MyFilms.cacheThumbWith, MyFilms.cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
                      LogMyFilms.Debug("Creating thumbimage for collection: '" + collection + "'");
                    }
                  }
                  //// notify that image has been downloaded
                  //item.NotifyPropertyChanged("PosterImageFilename");
                  SetProcessAnimationStatus(false, animation);
                  if (interactive) GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(1079846)); // Done !
                }
              }
              #endregion

              #region Fanart
              if (MyFilms.conf.StrFanart)
              {
                //string fanartUrl = tmdbConf.images.base_url + "original" + movie.MovieSearchResult.backdrop_path;
                //string filename;
                //string filename1 = GrabUtil.DownloadBacdropArt(MyFilms.conf.StrPathFanart, fanartUrl, item.Label, true, true, out filename);
                //// LogMyFilms.Debug("Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + item.Label);

                ////movie.MovieImages = api.GetMovieImages(movie.MovieSearchResult.id, language);
                ////if (movie.MovieImages.posters.Count == 0)
                ////{
                ////  movie.MovieImages = api.GetMovieImages(movie.MovieSearchResult.id, null);
                ////  LogMyFilms.Debug("GetImagesTMDB() - no '" + language + "' posters found - used default and found '" + movie.MovieImages.posters.Count + "'");
                ////}
                ////int ii = 0;
                ////foreach (Backdrop fanart in movie.MovieImages.backdrops)
                ////{
                ////  if (ii == 0)
                ////  {
                ////    string fanartUrl = tmdbConf.images.base_url + "original" + fanart.file_path;
                ////    string filename;
                ////    string filename1 = GrabUtil.DownloadBacdropArt(MyFilms.conf.StrPathFanart, fanartUrl, item.Label, true, (i == 0), out filename);
                ////    LogMyFilms.Debug("Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + item.Label);
                ////  }
                ////  ii++;
                ////}
              }
              #endregion

            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("GetImagesForTmdbCollection() - Error: '" + ex.Message + "'");
              LogMyFilms.Debug("GetImagesForTmdbCollection() - Exception: '" + ex.StackTrace + "'");
              SetProcessAnimationStatus(false, animation);
            }
            #endregion
          #endregion
          }
          catch (Exception tex)
          {
            LogMyFilms.Debug(tex, "CollectionArtwork() - error in TMDB grabbing movie '" + titlename + "': " + tex.Message);
            SetProcessAnimationStatus(false, animation);
          }
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex, "Thread 'LoadCollectionImages' - exception!");
        }
        finally
        {
          GUIWindowManager.Replace(2009, dlgMenuOrg);
          SetProcessAnimationStatus(false, animation);
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          // after background thread has finished !
          return 0;
        }, 0, 0, null);
      }) { Name = "LoadCollectionImages", IsBackground = true }.Start();
    }

    internal static void SetProcessAnimationStatus(bool enable, GUIAnimation guiSearchAnimation)
    {
      try
      {
        if (guiSearchAnimation != null)
        {
          if (enable)
            guiSearchAnimation.AllocResources();
          else
            guiSearchAnimation.Dispose();

          guiSearchAnimation.Visible = enable;
        }
        else
        {
          LogMyFilms.Warn("SetProcessAnimationStatus '" + enable + "' - skin control missing? - falling back to MP GUIWaitCursor()");
          if (enable && !MyFilms.animationStatus)
          {
            MyFilms.animationStatus = true;
            GUIWaitCursor.Init();
            GUIWaitCursor.Show();
          }
          else if (!enable && MyFilms.animationStatus)
          {
            MyFilms.animationStatus = false;
            GUIWaitCursor.Hide();
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("SetProcessAnimationStatus() -  '" + enable + "' - skin control missing? : " + ex.Message);
      }
    }

    #region GUI Events

    private void InitOVEventHandler()
    {
      // Subscribe to Event
      try
      {
        ExternalPlugins::OnlineVideos.MediaPortal1.GUIOnlineVideos OV = (ExternalPlugins::OnlineVideos.MediaPortal1.GUIOnlineVideos)GUIWindowManager.GetWindow((int)MyFilms.ExternalPluginWindows.OnlineVideos);
        OV.VideoDownloaded -= new ExternalPlugins::OnlineVideos.MediaPortal1.GUIOnlineVideos.VideoDownloadedHandler(OnVideoDownloaded);
        OV.VideoDownloaded += new ExternalPlugins::OnlineVideos.MediaPortal1.GUIOnlineVideos.VideoDownloadedHandler(OnVideoDownloaded);
        LogMyFilms.Info("Subscribed 'VideoDownloaded' event from OnlineVideos ...");
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Error subscribing to 'VideoDownloaded' event from OnlineVideos: " + ex.Message);
      }
    }

    private void OnVideoDownloaded(string file, string site, string categoryRecursiveName, string videoTitle)
    {
      LogMyFilms.Debug("OnVideoDownloaded() - file = '" + file + "', site = '" + site + "', categoryrecursivename = '" + categoryRecursiveName + "', videoTitle = '" + videoTitle + "'");
    }
    #endregion

  }

}
    #endregion
