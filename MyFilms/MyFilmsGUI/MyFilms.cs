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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Grabber;
using Grabber.Data;
using Grabber.Importer;
using Grabber.Util;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.ExtensionMethods;
using MediaPortal.GUI.Library;
using MediaPortal.Services;
using MediaPortal.Util;
using MediaPortal.Video.Database;
using MyFilmsPlugin.Configuration;
using MyFilmsPlugin.DataBase;
using MyFilmsPlugin.Utils;
using MyFilmsPlugin.Utils.Cornerstone.MP;
using NLog;
using NLog.Config;
using NLog.Targets;
using WatTmdb.V3;
using GUILocalizeStrings = MyFilmsPlugin.Utils.GUILocalizeStrings;
using ImageFast = MyFilmsPlugin.Utils.ImageFast;
using Thumbs = MediaPortal.Util.Thumbs;
using WakeOnLanManager = MyFilmsPlugin.Utils.WakeOnLanManager;

namespace MyFilmsPlugin.MyFilmsGUI
{
  using Action = MediaPortal.GUI.Library.Action;
  using GUILocalizeStrings = GUILocalizeStrings;
  // using AntMovieCatalog = MyFilmsPlugin.MyFilms.AntMovieCatalog;
  // using TmdbPerson = Grabber.TheMovieDbAPI.TmdbPerson;

  using WakeOnLanManager = WakeOnLanManager;
  
  using TMDB = WatTmdb.V3;

  /// <summary>
  /// Summary description for GUIMyFilms.
  /// </summary>
  [PluginIcons("MyFilmsPlugin.Resources.film-reel-128x128.png", "MyFilmsPlugin.Resources.film-reel-128x128-faded.png")]

  public class MyFilms : GUIWindow, ISetupForm, IComparer<GUIListItem>
  {
    #region Constructor
    public MyFilms()
    {
      // create Backdrop image swapper
      backdrop = new ImageSwapper();
      backdrop.ImageResource.Delay = 250;
      backdrop.PropertyOne = "#myfilms.fanart";
      backdrop.PropertyTwo = "#myfilms.fanart2";

      // create Menu image swapper
      menucover = new AsyncImageResource
      {
        Property = "#myfilms.menuimage",
        Delay = 125
      };

      // create Film Cover image swapper
      filmcover = new AsyncImageResource
      {
        Property = "#myfilms.coverimage",
        Delay = 125
      };

      // create Group Cover image swapper
      viewcover = new AsyncImageResource
      {
        Property = "#myfilms.viewcoverimage",
        Delay = 75
      };

      // create Person Cover image swapper
      personcover = new AsyncImageResource
      {
        Property = "#myfilms.personcoverimage",
        Delay = 125
      };

      // create Group Cover image swapper
      groupcover = new AsyncImageResource
      {
        Property = "#myfilms.groupcoverimage",
        Delay = 50
      };

    }
    #endregion

    #region TMDB online movie variables

    DateTime lastRequestPopularMovies;
    IEnumerable<TmdbMovieSearchResult> PopularMovies
    {
      get
      {
        if (popularMovies == null || lastRequestPopularMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          popularMovies = GetPopularMovies(true);
          lastRequestPopularMovies = DateTime.UtcNow;
        }
        return popularMovies;
      }
    }
    private IEnumerable<TmdbMovieSearchResult> popularMovies;

    DateTime lastRequestNowPlayingMovies;
    IEnumerable<TmdbMovieSearchResult> NowPlayingMovies
    {
      get
      {
        if (nowPlayingMovies == null || lastRequestNowPlayingMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          nowPlayingMovies = GetNowPlayingMovies(true);
          lastRequestNowPlayingMovies = DateTime.UtcNow;
        }
        return nowPlayingMovies;
      }
    }
    private IEnumerable<TmdbMovieSearchResult> nowPlayingMovies;

    DateTime lastRequestTopRatedMovies;
    IEnumerable<TmdbMovieSearchResult> TopRatedMovies
    {
      get
      {
        if (topRatedMovies == null || lastRequestTopRatedMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          topRatedMovies = GetTopRatedMovies(true);
          lastRequestTopRatedMovies = DateTime.UtcNow;
        }
        return topRatedMovies;
      }
    }
    private IEnumerable<TmdbMovieSearchResult> topRatedMovies;

    DateTime lastRequestUpcomingMovies;
    IEnumerable<TmdbMovieSearchResult> UpcomingMovies
    {
      get
      {
        if (upcomingMovies == null || lastRequestUpcomingMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          upcomingMovies = GetUpcomingMovies(true);
          lastRequestUpcomingMovies = DateTime.UtcNow;
        }
        return upcomingMovies;
      }
    }
    private IEnumerable<TmdbMovieSearchResult> upcomingMovies;

    DateTime lastRequestImdbTop250;
    IEnumerable<TmdbMovieSearchResult> ImdbTop250Movies
    {
      get
      {
        if (imdbTop250Movies == null || lastRequestImdbTop250 < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          imdbTop250Movies = GetImdbTop250Movies(true);
          lastRequestImdbTop250 = DateTime.UtcNow;
        }
        return imdbTop250Movies;
      }
    }
    private IEnumerable<TmdbMovieSearchResult> imdbTop250Movies;

    #endregion

    #region MapSettings class
    [Serializable]
    public class MapSettings
    {
      protected int _SortBy;
      protected int _ViewAs;
      protected bool _SortAscending;

      public MapSettings()
      {
        // Set default layout
        _SortBy = 0;
        _ViewAs = (int)Layout.List;
        _SortAscending = true;
      }

      [XmlElement("SortBy")]
      public int SortBy
      {
        get { return _SortBy; }
        set { _SortBy = value; }
      }

      [XmlElement("ViewAs")]
      public int ViewAs
      {
        get { return _ViewAs; }
        set { _ViewAs = value; }
      }

      [XmlElement("SortAscending")]
      public bool SortAscending
      {
        get { return _SortAscending; }
        set { _SortAscending = value; }
      }
    }
    #endregion

    #region ISetupForm Members

    // Returns the name of the plugin which is shown in the plugin menu
    public string PluginName()
    {
      return "MyFilms";
    }

    private string PluginNameSetup()
    {
      if (strPluginName == null)
      {
        string wPluginName;
        using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
        {
          wPluginName = xmlreader.GetValueAsString("MyFilms", "PluginName", "MyFilms");
        }
        strPluginName = wPluginName;
      }
      return strPluginName;
    }

    // Returns the description of the plugin is shown in the plugin menu
    public string Description()
    {
      return "MyFilms Ant Movie Catalog - Guzzi Mod";
    }

    // Returns the author of the plugin which is shown in the plugin menu
    public string Author()
    {
      return "Zebons (Mod by Guzzi)";
    }

    // show the setup dialog
    public void ShowPlugin()
    {
      System.Windows.Forms.Form setup = new MyFilmsSetup();
      setup.ShowDialog();
    }

    // Indicates whether plugin can be enabled/disabled
    public bool CanEnable()
    {
      return true;
    }

    // get ID of windowplugin belonging to this setup
    public int GetWindowId()
    {
      return 7986;
    }

    // Indicates if plugin is enabled by default;
    public bool DefaultEnabled()
    {
      return true;
    }

    // indicates if a plugin has its own setup screen
    public bool HasSetup()
    {
      return true;
    }

    /// <summary>
    /// If the plugin should have its own button on the main menu of Media Portal then it
    /// should return true to this method, otherwise if it should not be on home
    /// it should return false
    /// </summary>
    /// <param name="strButtonText">text the button should have</param>
    /// <param name="strButtonImage">image for the button, or empty for default</param>
    /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
    /// <param name="strPictureImage">subpicture for the button or empty for none</param>
    /// <returns>true  : plugin needs its own button on home
    ///          false : plugin does not need its own button on home</returns>
    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
    {
      strButtonText = PluginNameSetup();
      strButtonImage = String.Empty;
      strButtonImageFocus = String.Empty;
      strPictureImage = String.Format("hover_{0}.png", "MyFilms");
      string strBtnFile = String.Format(@"{0}\media\{1}", GUIGraphicsContext.Skin, strPictureImage);
      if (!System.IO.File.Exists(strBtnFile))
        strPictureImage = string.Empty;
      return true;
    }
    #endregion

    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();
    // public XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true);

    private const string LogFileName = "MyFilms.log";  //log's filename
    private const string OldLogFileName = "MyFilms.old.log";  //log's old filename
    private const string LogFileNameGrabber = "MyFilmsGrabber.log";  //log's filename
    private const string OldLogFileNameGrabber = "MyFilmsGrabber.old.log";  //log's old filename

    // private BaseMesFilms films;

    #region Descriptif zones Ecran
    internal const int ID_MyFilms = 7986;
    internal const int ID_MyFilmsDetail = 7987;
    internal const int ID_MyFilmsDialogRating = 7988;
    internal const int ID_MyFilmsActors = 7989;
    // internal const int ID_MyFilmsThumbs = 7990;
    // internal const int ID_MyFilmsActorsInfo = 7991; // has been removed
    internal const int ID_MyFilmsCoverManager = 7992;
    internal const int ID_MyFilmsDialogMultiSelect = 7993;
    internal const int ID_MyFilmsFanartManager = 7994;
    internal const int ID_MyFilmsFullScreen = 7995;
    internal const int ID_MyFilmsTrailerOSD = 7996;
    internal const int ID_MyFilmsDialogImageSelect = 7997;

    internal const int cacheThumbWith = 400;
    internal const int cacheThumbHeight = 600;

    internal const int ItemPublishDelay = 350;

    internal const string ImdbBaseUrl = "http://www.imdb.com/";
    internal const string TmdbApiKey = "1e66c0cc99696feaf2ea56695e134eae";

    internal const int MaxTmdbResults = 299;
    internal const int MaxSearchHistoryLength = 30;

    internal const string GlobalUsername = "Global";
    internal const string DefaultUsername = "Default";

    internal enum Controls : int
    {
      CTRL_BtnLayout = 2,
      CTRL_BtnSortBy = 3,
      CTRL_BtnSearch = 4,
      CTRL_BtnViewAs = 5,
      CTRL_BtnOptions = 6,
      CTRL_BtnGlobalOverlayFilter = 7,
      CTRL_BtnGlobalUpdates = 8,
      CTRL_BtnToggleGlobalUnwatchedStatus = 9,
      CTRL_BtnOnlineMovieLists = 10,
      //CTRL_TxtSelect = 12,
      CTRL_BtnSelectRamdomMovie = 13,
      CTRL_Fanart = 11,
      CTRL_Fanart2 = 21,
      CTRL_LoadingImage = 22,
      CTRL_DummyFacadeFilm = 36,
      CTRL_DummyFacadeView = 37,
      CTRL_DummyFacadePerson = 38,
      CTRL_DummyFacadeHierarchy = 39,
      CTRL_DummyFacadeMenu = 40,
      CTRL_DummyFacadeIndex = 41,
      CTRL_ListFilms = 50, // Changed from 1026 to 50 due to meeting MePo Standards - holds film lists and hierarchies
      //CTRL_ListMenu = 51, // added as separate list control for new menu - holds menu and extended menu
      //CTRL_ListViews = 52, // added as separate list control for Views - holds normal and indexed views (groupings)
      CTRL_logos_id2001 = 2001,
      CTRL_logos_id2002 = 2002,
      CTRL_logos_id2003 = 2003,
      CTRL_logos_id2012 = 2012,
      CTRL_GuiWaitCursor = 2080,
    }

    #region Skin Variables
    //[SkinControlAttribute((int)Controls.CTRL_TxtSelect)]
    //protected GUIFadeLabel TxtSelect;

    [SkinControl((int)Controls.CTRL_BtnLayout)]
    protected GUIButtonControl BtnLayout; // ToDo: can be replaceed by "GUIMenuButton" when dropping MP1.2 compatibility - requires skin change too

    [SkinControl((int)Controls.CTRL_BtnSortBy)]
    protected GUISortButtonControl BtnSrtBy;

    [SkinControl((int)Controls.CTRL_BtnViewAs)]
    protected GUIButtonControl BtnViewAs; // ToDo: can be replaceed by "GUIMenuButton" when dropping MP1.2 compatibility - requires skin change too

    [SkinControl((int)Controls.CTRL_BtnGlobalOverlayFilter)]
    protected GUIButtonControl BtnGlobalOverlayFilter;

    [SkinControl((int)Controls.CTRL_BtnGlobalUpdates)]
    protected GUIButtonControl BtnGlobalUpdates;

    //[SkinControlAttribute((int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus)]
    //protected GUIButtonControl BtnToggleGlobalWatched;

    //[SkinControlAttribute((int)Controls.CTRL_BtnOnlineMovieLists)]
    //protected GUIButtonControl BtnOnlineMovieLists;

    [SkinControl((int)Controls.CTRL_ListFilms)]
    protected GUIFacadeControl facadeFilms;

    //[SkinControlAttribute((int)Controls.CTRL_ListMenu)]
    //protected GUIFacadeControl facadeMenu;

    //[SkinControlAttribute((int)Controls.CTRL_ListViews)]
    //protected GUIFacadeControl facadeViews;

    [SkinControl((int)Controls.CTRL_logos_id2001)]
    protected GUIImage ImgID2001;

    [SkinControl((int)Controls.CTRL_logos_id2002)]
    protected GUIImage ImgID2002;

    [SkinControl((int)Controls.CTRL_logos_id2003)]
    protected GUIImage ImgID2003;

    [SkinControl((int)Controls.CTRL_logos_id2012)]
    protected GUIImage ImgID2012;

    [SkinControl((int)Controls.CTRL_Fanart)]
    protected GUIImage ImgFanart;

    [SkinControl((int)Controls.CTRL_Fanart2)]
    protected GUIImage ImgFanart2;

    [SkinControl((int)Controls.CTRL_LoadingImage)]
    protected GUIImage loadingImage;

    [SkinControl((int)Controls.CTRL_GuiWaitCursor)]
    protected GUIAnimation m_SearchAnimation;

    [SkinControl((int)Controls.CTRL_DummyFacadeFilm)]
    protected GUILabelControl dummyFacadeFilm = null;

    [SkinControl((int)Controls.CTRL_DummyFacadeHierarchy)]
    protected GUILabelControl dummyFacadeHierarchy = null;

    [SkinControl((int)Controls.CTRL_DummyFacadeView)]
    protected GUILabelControl dummyFacadeView = null;

    [SkinControl((int)Controls.CTRL_DummyFacadePerson)]
    protected GUILabelControl dummyFacadePerson = null;

    [SkinControl((int)Controls.CTRL_DummyFacadeMenu)]
    protected GUILabelControl dummyFacadeMenu = null;

    [SkinControl((int)Controls.CTRL_DummyFacadeIndex)]
    protected GUILabelControl dummyFacadeIndex = null;

    #endregion

    #region Private/Public Properties

    private bool StopLoadingMenuDetails { get; set; } // to cancel menu counts, if user makes selection before it's finished ....
    private bool StopLoadingViewDetails { get; set; }
    private bool StopLoadingFilmlistDetails { get; set; }

    private bool StopAddingTrailers { get; set; }

    private bool doUpdateMainViewByFinishEvent = false;

    // private Layout CurrentLayout { get; set; }

    // public static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public static int Prev_ItemID = -1;
    public static string Prev_Label = string.Empty;

    public static bool animationStatus = false;

    public static GUIListItem itemToPublish = null;

    public static Configuration conf;
    //// keeps track of last loaded config to make sure that on page load the DB is reloaded, if it was changed by e.g. the default config
    //private static Configuration confPrevious = null;

    private string PreviousConfig = ""; // keeps last loaded config for checks in OnPageLoad()

    private static Logos confLogos;

    public static DataRow[] r; // will hold current recordset to traverse
    public static MFMovie currentMovie = new MFMovie(); // will hold current recordset to traverse

    internal static string movielink; // for trailer playback

    //Imageswapperdefinitions for fanart and cover
    private ImageSwapper backdrop;

    private AsyncImageResource menucover;
    private AsyncImageResource filmcover;
    private AsyncImageResource viewcover;
    private AsyncImageResource personcover;
    private AsyncImageResource groupcover;

    private Watcher watcherUpdater; // updater for scan directory watchers

    //Added to jump back to correct Menu (Either Basichome or MyHome - or others...)
    private bool Context_Menu;
    //private string currentConfig;
    private string strPluginName;

    private Timer _fanartTimer;

    //Guzzi Addons for Global nonpermanent Trailer and MinRating Filters
    public bool GlobalFilterTrailersOnly = false;
    public bool GlobalFilterMinRating = false;
    public bool GlobalFilterIsOnlineOnly = false;
    public string GlobalFilterMovieFormat3D = "";
    //public string GlobalFilterString = string.Empty;
    public string GlobalFilterStringTrailersOnly = string.Empty;
    public string GlobalFilterStringUnwatched = string.Empty;
    public string GlobalFilterStringMinRating = string.Empty;
    public string GlobalFilterStringIsOnline = string.Empty;
    //public string GlobalUnwatchedFilterString = string.Empty;
    //public string GlobalFilterStringCustomFilter = string.Empty;
    public string GlobalFilterStringMovieFormat3D = string.Empty;

    public static bool InitialStart;                  //Added to implement InitialViewSetup
    public static bool InitialIsOnlineScan;           //Added to implement switch if facade should display media availability

    // current Random Fanart
    public static List<string> currentFanartList = new List<string>();

    // current Trailer List for Scrobbling
    internal static List<MFMovie> currentTrailerMoviesList = new List<MFMovie>();
    internal static MFMovie currentTrailerPlayingItem;
    public static bool trailerscrobbleactive;

    //PlayList currentPlaylist = null;
    //PlayListItem currentPlayingItem = null;

    private double lastPublished = 0;
    private Timer publishTimer;

    private static Stopwatch watch = new Stopwatch();

    private const int RandomFanartDelay = 15000;

    // Version for Skin Interface
    internal const int SkinInterfaceVersionMajor = 1;

    internal const int SkinInterfaceVersionMinor = 0;

    public static bool DebugPropertyLogging { get; set; }

    public static bool SendTraktUpdateMessage { get; set; } // to tell FSwatcher, if Trakt message should be sent

    // keeps track of currently loaded skin name to (re)initiate skin interface check on pageload
    private string currentSkin;

    private static string[] PersonTypes = new string[] { "Persons", "Actors", "Producer", "Director", "Writer", "Borrower", "Composer" };

    private static string EmptyFacadeValue = "(empty)";

    // View History for facade navigation support
    // private static List<ViewState> ViewHistory = new List<ViewState>();
    private Stack ViewHistory = new Stack();

    // new method for back navigation
    private Stack _navigationStack = new Stack();
    MapSettings _mapSettings = new MapSettings();
    // static GUIDialogProgress dlgProgress;

    // cache to store viewstate params from current session per view
    Dictionary<string, ViewState> _viewStateCache = new Dictionary<string, ViewState>();  // public static Dictionary<string, ViewState> _viewStateCache = new Dictionary<string, ViewState>();

    // string list for search history
    public static List<string> SearchHistory = new List<string>();
    
    LoadParameterInfo loadParamInfo;

    internal static string internalLoadParam;

    // last update to catalog - used to know, if the backnavigation needs to reload the facade -  LoadFacade();
    public static DateTime LastDbUpdate { get; set; }

    // TMDB sort method
    private SortMethod _currentSortMethod = SortMethod.Date;
    private string _currentSortDirection = " DESC";

    // os handler variables

    #endregion

    #region Enums
    private enum SortMethod
    {
      Name = 0,
      Date = 1,
      Rating = 2,
      Votes = 3
    }

    private enum SortDirection
    {
      Asc = 0,
      Desc = 1
    }

    public enum FieldType
    {
      Title,
      Date,
      AlphaNumeric,
      Decimal,
      Person,
      Default
    }

    public enum ExternalPluginWindows : int
    {
      BrowseTheWeb = 54537689,
      OnlineVideos = 4755,
      SubCentral = 84623,
      BluRayPlayerLauncher = 8080,
      MpNzb = 3847
    }

    public enum ViewContext
    {
      Movie,
      MovieCollection,
      Group,
      Person,
      Menu,
      MenuAll,
      StartView,
      None,
      TmdbMovies
    }

    public enum ViewSortType
    {
      Name = 0,
      Frequency = 1
    }

    //enum of all possible layouts
    public enum Layout
    {
      List = 0,
      SmallIcons = 1,
      LargeIcons = 2,
      Filmstrip = 3,
      AlbumView = 4,
      Playlist = 5,
      CoverFlow = 6
    }

    public enum SetupType
    {
      Local = 0,
      CentralMaster = 1,
      CentralClient = 2
    }
    #endregion

    #endregion

    #region handler and backgroundworker

    public delegate void FilmsStoppedHandler(int stoptime, string filename);
    public delegate void FilmsEndedHandler(string filename);

    public delegate void EventHandler(object sender, EventArgs e);

    BackgroundWorker bgUpdateDB = new BackgroundWorker();
    BackgroundWorker bgUpdateFanart = new BackgroundWorker();
    BackgroundWorker bgUpdateActors = new BackgroundWorker();
    BackgroundWorker bgUpdateTrailer = new BackgroundWorker();
    BackgroundWorker bgLoadMovieList = new BackgroundWorker();
    BackgroundWorker bgIsOnlineCheck = new BackgroundWorker();

    internal static BackgroundWorker bgDownloadTrailer = new BackgroundWorker();
    static Queue<Trailer> TrailertoDownloadQueue = new Queue<Trailer>();
    internal static AutoResetEvent bgDownloadTrailerDoneEvent = new AutoResetEvent(false);

    // experimental BG worker array for downloaad parallism
    internal const int maxThreads = 4;

    internal static BackgroundWorker[] threadArray = new BackgroundWorker[maxThreads];
    // static int _numberBackGroundThreads;  //Just for fun
    internal static AutoResetEvent[] threadDoneEventArray = new AutoResetEvent[maxThreads];


    public static FileSystemWatcher FSwatcher = new FileSystemWatcher();

    public delegate void DelegateUpdateProgress(string message);
    public static void UpdateMyProgressbar(string message)
    {
      MyFilmsDetail.setGUIProperty("statusmessage", message);
    }

    public static event ImportCompleteEventDelegate ImportComplete;
    public delegate void ImportCompleteEventDelegate();


    //public static event WatchedEventDelegate WatchedItem;
    //public delegate void WatchedEventDelegate(MFMovie movie, bool watched, int count);

    //public static event RatingEventDelegate RateItem;
    //public delegate void RatingEventDelegate(MFMovie movie, string rating);

    //public event TrailerScrobbledEventDelegate TrailerScrobbled;
    //public delegate void TrailerScrobbledEventDelegate(MFMovie movie, bool watched, int count);
    //TrailerScrobbled += (OnPlayBackEnded);
    //private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
    //{
    //  LogMyFilms.Debug("OnPlayBackEnded was initiated, but has no relevant event data for MyFilms - filename: '" + filename + "'");
    //  if (MyFilms.trailerscrobbleactive == true)
    //  {
    //    MyFilms.trailerscrobbleactive = false;
    //    MyFilms.PlayRandomTrailer(true);
    //  }
    //  return;
    //}


    #endregion

    #region Base Overrides

    public override int GetID
    {
      get { return ID_MyFilms; }
      set { base.GetID = value; }
    }

    public override string GetModuleName()
    {
      string currentmodule = GUILocalizeStrings.Get(ID_MyFilms);
      string view = conf != null ? conf.StrTxtView : "";
      string select = conf != null ? conf.StrTxtSelect : "";
      if (conf != null)
      {
        currentmodule = GUILocalizeStrings.Get(ID_MyFilms) + "/" + view + (@select.Length > 0 ? "/" : "") + select;
      }
      LogMyFilms.Debug("GetModuleName()  - conf.StrTxtView = '" + view + "', conf.StrTxtSelect = '" + select + "', currentmodule = '" + currentmodule + "'");

      return currentmodule; // return localized string for Module ID
    }

    public override bool Init() //This Method is only loaded ONCE when starting Mediaportal !
    {
      InitLogger(); // Initialize Logger 
      Log.Info("MyFilms.Init() started. See MyFilms.log for further Details.");
      LogMyFilms.Info("MyFilms.Init() started on '" + Environment.MachineName + "'.");
      LogMyFilms.Info("MyFilms     Version: 'V" + MyFilmsSettings.Version + "', BuildDate: '" + MyFilmsSettings.BuildDate + "'");
      LogMyFilms.Info("MediaPortal Version: 'V" + MyFilmsSettings.MPVersion + "',    BuildDate: '" + MyFilmsSettings.MPBuildDate + "', SkinVersion: '" + MyFilmsSettings.MPSkinVersion + "'");
      LogMyFilms.Info("MyFilms Skin Interface Version: 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");

      bool result = Load(GUIGraphicsContext.Skin + @"\MyFilms.xml");

      // check, if remote config file should be copied to local MP data dir (MyFilms Server Setup)
      SyncConfigFromRemoteServer();

      // clean orphaned files for all configs if any leftovers
      CleanOrphanedDBlocks();

      // Fanart Timer
      _fanartTimer = new Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);

      // Set Variable for FirstTimeView Setup
      InitialStart = true;

      // by default, enable trakt message handling
      SendTraktUpdateMessage = true;

      //Add localized labels for DB Columns
      InitGUIPropertyLabels();

      // Init Eventhandler for Background Updates
      MyFilmsDetail.DetailsUpdated += OnDetailsUpdated;

      // Init Eventhandler for Trailer Scrobbling
      MyFilmsDetail.TrailerEnded += OnTrailerEnded;

      // Register Messagehandler for CD-Inserted-Messages
      //GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_OnNewMessage);

      // Register Eventhandler for AMCupdater Background progress reporting
      //AMCupdaterStartEventHandler();

      // register handlers for os powerevent and network availability messages
      System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
      // LogMyFilms.Debug("Init() - Successfully subscribed NetworkAvailabilityChanged Handler !");
      Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
      // LogMyFilms.Debug("Init() - Successfully subscribed PowerModeChanged Handler !");
      
      // Initialize Backgroundworker
      InitializeBackgroundWorker();
      InitFolders();

      // load trailer queue
      TrailertoDownloadQueue = BaseMesFilms.LoadQueueFromDisk("Trailer");
      InitializeQueuedTrailerDownloader(90); // initialize trailer download threads with delayed start (to let OV assembly initialize)

      #region launch TMDB data loader in background ... (disabled)
      //new System.Threading.Thread(delegate()
      //{
      //  {
      //    try
      //    {
      //      IEnumerable<PopularMovie> movies = PopularMovies;
      //    }
      //    catch (Exception)
      //    {
      //    }
      //  }
      //  GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
      //  {
      //    {
      //      // this after thread finished ...
      //    }
      //    return 0;
      //  }, 0, 0, null);
      //}) { Name = "MyFilmsTmdbDataLoader", IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start();
      #endregion

      // preload config and dataset
      InitConfigPreload();

      LogMyFilms.Debug("MyFilms.Init() completed. Loading main skin file.");
      return result;
    }

    public override void DeInit()
    {
      LogMyFilms.Debug("MyFilms.DeInit() - Saving Config changes ...");

      StopAddingTrailers = true;

      XmlSettings.SaveCache();

      BaseMesFilms.CancelMyFilms(); // also stops backgroundworker ...

      CleanOrphanedDBlocks(); // clean orphaned files for all configs if any leftovers

      // save trailer queue
      BaseMesFilms.SaveQueueToDisk("Trailer", TrailertoDownloadQueue);

      // deregister network and powerevent handlers
      System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;
      Microsoft.Win32.SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;

      LogMyFilms.Debug("MyFilms.DeInit() - Shutdown completed...");
      base.DeInit();
    }

    protected override void OnPageLoad()
    {
      LogMyFilms.Debug("MyFilms.OnPageLoad() started."); Log.Debug("MyFilms.OnPageLoad() started. See MyFilms.log for further Details.");

      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms));

      if (!CheckSkinInterfaceVersion())
      {
        GUIWindowManager.ShowPreviousWindow();
        return;
      }

      if (InitialStart)
      {
        CheckAndLogEnhancedSkinControls();
        InitMainScreen(false); // don't log to MyFilms.log Property clear
        //InitGlobalFilters(false);
      }

      #region Support for StartParameters - load  them ...

      if (PreviousWindowId != ID_MyFilms && PreviousWindowId != ID_MyFilmsDetail)
      {
        // check if running version of mediaportal support loading with parameter           
        //{
        //  LoadWithParameterSupported = true;
        //}

        loadParamInfo = null; // reset the LoadParameterInfo

        string loadParam = null;
        // check if running version of mediaportal supports loading with parameter and handle _loadParameter
        System.Reflection.FieldInfo fi = typeof(GUIWindow).GetField("_loadParameter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fi != null)
          loadParam = (string)fi.GetValue(this);

        // check for LoadParameters by GUIproperties if nothing was set by the _loadParameter
        if (string.IsNullOrEmpty(loadParam))
          loadParam = LoadParameterInfo.FromGuiProperties();
        if (!string.IsNullOrEmpty(loadParam))
          loadParamInfo = new LoadParameterInfo(loadParam);
      }
      #endregion

      //MyFilmsDetail.clearGUIProperty("picture");

      base.OnPageLoad(); // let animations run

      #region init buttons and register sort button handler ...
      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnSearch, GUILocalizeStrings.Get(137));
      if (BtnSrtBy != null)
      {
        BtnSrtBy.SortChanged -= SortChanged;
        BtnSrtBy.SortChanged += SortChanged;
      }

      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnGlobalOverlayFilter, GUILocalizeStrings.Get(10799402)); // BtnGlobalOverlayFilter.Label = GUILocalizeStrings.Get(10798714); // Global Filters
      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnGlobalUpdates, GUILocalizeStrings.Get(10799401)); // global updates
      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnOnlineMovieLists, GUILocalizeStrings.Get(10798825)); // online information

      //try { BtnGlobalUpdates.Label = GUILocalizeStrings.Get(10798690); } // Global Updates ...  
      //catch (Exception) { LogMyFilms.Warn("(InitMainScreen) - 'Global Updates' button label cannot be set - not defined in skin?"); }
      #endregion

      #region (re)link backdrop image controls to the backdrop image swapper
      backdrop.GUIImageOne = ImgFanart;
      backdrop.GUIImageTwo = ImgFanart2;
      //backdrop.LoadingImage = loadingImage;  // --> Not used - could be used to show other image while loading destination thumb

      if (!menucover.Active) menucover.Active = true;
      if (!filmcover.Active) filmcover.Active = true;
      if (!viewcover.Active) viewcover.Active = true;
      if (!personcover.Active) personcover.Active = true;
      if (!groupcover.Active) groupcover.Active = true;
      #endregion

      if (PreviousWindowId != ID_MyFilmsDetail && PreviousWindowId != ID_MyFilmsActors && PreviousWindowId != (int)ExternalPluginWindows.OnlineVideos
        && PreviousWindowId != (int)ExternalPluginWindows.BrowseTheWeb && PreviousWindowId != (int)Window.WINDOW_FULLSCREEN_VIDEO) // Fullscreen is ID 2005
      {
        #region Load or change MyFilms Config

        PreviousConfig = Configuration.CurrentConfig;
        bool isDefaultConfig = false;
        

        if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config)) // config given in load params
        {
          #region change config by load parameter overload

          string currentconfig = (!string.IsNullOrEmpty(Configuration.CurrentConfig)) ? Configuration.CurrentConfig : "";
          LogMyFilms.Debug("Load_Config() - LoadParams - try override current config '" + currentconfig + "' with LoadParameter config: '" + loadParamInfo.Config + "'");
          string newConfig = Configuration.ControlAccessConfig(loadParamInfo.Config, GetID);
          if (newConfig != string.Empty) // if user escapes dialog or bad value leave system unchanged
          {
            if (Configuration.CurrentConfig != newConfig)
            {
              if (!string.IsNullOrEmpty(Configuration.CurrentConfig) && conf != null && conf.StrSelect != null) // if there is an active config, save it !
              {
                if (facadeFilms == null || facadeFilms.SelectedListItemIndex == -1)
                  Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
                else
                  Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
              }
              // set new config
              Configuration.CurrentConfig = newConfig;
              // initialize ...
              InitMainScreen(false); // reset all properties and values
              InitGlobalFilters(false); // reset global filters, when loading new config !
              InitialIsOnlineScan = false; // set false, so facade does not display false media status !!!
              InitialStart = (string.IsNullOrEmpty(loadParamInfo.MovieID));
              //Set to true to make sure initial View is initialized for new DB view
            }
          }
          #endregion
        }
        else
        {
          isDefaultConfig = Configuration.Current_Config(true);
        }

        if (string.IsNullOrEmpty(Configuration.CurrentConfig))
        {
          LogMyFilms.Error("OnPageLoad(): Config is empty - returning calling ShowPreviousWindow() !");
          GUIWindowManager.ShowPreviousWindow();
          return;
        }

        bool isvalidDb = Load_Config(Configuration.CurrentConfig, true, loadParamInfo);

        // check if it is a valid DB
        if (!isvalidDb)
        {
          LogMyFilms.Error("OnPageLoad(): DB has unsupported AMC4.2 format - returning calling ShowPreviousWindow() !");
          GUIUtils.ShowErrorDialog("Movie DB has unsupported AMC4.2 format !");
          GUIWindowManager.ShowPreviousWindow();
          return;
        }

        InitFSwatcher(); // load DB watcher for multiseat
        if (InitialStart) InitAmcImporter(conf); // load watcher for movie directories

        #endregion

        Fanartstatus(conf.StrFanart);
        if (!InitialStart && isDefaultConfig) InitMainScreen(false); // clear all properties, if a defaultconfig is loaded - otherwise we might run into display problems due to old properties remaining
        if (Configuration.CurrentConfig != PreviousConfig) InitialStart = true; // if a default config is set, otherwise the DB gets not proerly initialized
        if (conf.AlwaysDefaultView) 
        {
          _navigationStack.Clear(); // if returning from foreign window make sure, default is loaded instead of cached state
          conf.StrSelect = ""; // reset movie context filter for person views
          conf.StrPersons = ""; // reset person list filter
          viewcover.Filename = "";
          personcover.Filename = "";
          groupcover.Filename = "";
        }
      }
      // base.OnPageLoad(); // let animations run

      if (internalLoadParam != null) // internal option call, just do it
      {
        #region check, if main view was called with internal start param option
        string option = internalLoadParam;
        internalLoadParam = null;
        LogMyFilms.Debug("OnPageload() launched with loadParam internal 'internalLoadParam' = '" + internalLoadParam + "'");

        switch (option)
        {
          case "moviepersonlist":
            {
              MoviePersonListLauncher(false, true);
              return;
            }
          default:
            return;
        }
        #endregion
      }

      if (InitialStart) _navigationStack.Clear();
      if (loadParamInfo != null) _navigationStack.Clear(); // clear navigation stack, if called via load params

      if (_navigationStack.Count > 0)
      {
        DoBack();
        GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        // GUIControl.FocusControl(GetID, facadeFilms.GetID);
        OnPageload_Step_2();
      }
      else
      {
        // Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart), (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config) || IsDefaultConfig)); // reloadFromDisk is true, if a config is set in MF setup (not default view!) or loadparams are set
        new Thread(delegate()
        {
          {
            #region load Fin_Charge_Init with waiting symbol or progress dialog
            MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
            // GUIWaitCursor.Init(); GUIWaitCursor.Show();
            //GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
            //if (InitialStart && GetID == ID_MyFilms && conf.StrFileXml.StartsWith(@"\\"))
            //{
            //  #region Show loading in progress dialog
            //  if (dlgPrgrs != null)
            //  {
            //    dlgPrgrs.Reset();
            //    dlgPrgrs.DisplayProgressBar = false;
            //    dlgPrgrs.ShowWaitCursor = true;
            //    dlgPrgrs.DisableCancel(true);
            //    dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Loading ..."));
            //    // dlgPrgrs.SetLine(1, "Loading DB ...");
            //    // dlgPrgrs.Percentage = 0;
            //    dlgPrgrs.NeedRefresh();
            //    dlgPrgrs.ShouldRenderLayer();
            //    dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
            //  }
            //  #endregion
            //}

            if (InitialStart)
            {
              conf.StrSelect = "";
              conf.StrFilmSelect = "";
              conf.StrViewSelect = "";
              // loaded already in FinChargeInit() with reload = true if InitialStart !
              // r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // if (InitialStart) BaseMesFilms.LoadMyFilms(conf.StrFileXml);
            }

            Fin_Charge_Init(conf.AlwaysDefaultView || InitialStart, (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config)) || conf.IsDbReloadRequired); // || InitialStart)); // reloadFromDisk is true, if the loaded config and it's used DB file is physically a different one than the previous one
            // Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart), ((loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config)))); // || InitialStart)); // reloadFromDisk is true, if a config is set in MF setup (not default view!) or loadparams are set
            // Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart || requireFullReload), requireFullReload || Configuration.Current_Config()); // reloadFromDisk is true, if a config is set in MF setup (not default view!) or loadparams are set

            // GUIWaitCursor.Hide();
            MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
            //GUIControl.ShowControl(GetID, 34);

            //if (GetID == ID_MyFilms)
            //{
            //  if (dlgPrgrs != null)
            //  {
            //    dlgPrgrs.ShowWaitCursor = false;
            //    dlgPrgrs.Close();
            //  }
            //}
            #endregion
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              GUIPropertyManager.SetProperty("#currentmodule", GetModuleName());  // reload current modulename, as otherwise it is reset to base one...
              OnPageload_Step_2();
            }
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

      }

      // OnPageload_Step_2();


      // base.OnPageLoad(); // let animations run  
    }

    private void OnPageload_Step_2()
    {
      LogMyFilms.Debug("OnPageload_Step_2() started.");
      // Launch Background availability scanner, if configured in setup
      if (conf.ScanMediaOnStart)
      {
        AsynIsOnlineCheck();
      }

      #region LoadParam processing for single movie and search
      if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID) && loadParamInfo.Config == Configuration.CurrentConfig && PreviousWindowId != ID_MyFilmsDetail) // movieID given in load params -> jump to details screen !
      {
        LogMyFilms.Debug("OnPageLoad() - LoadParams - try override loading movieid: '" + loadParamInfo.MovieID + "', play: '" + loadParamInfo.Play + "'");
        // facade index is alreadey set in filmlist loading - only launching details necessary !

        if (!string.IsNullOrEmpty(loadParamInfo.MovieID)) // if load params for movieid exist, set current index to the movie detected
        {
          if (r.Length == 0)
            GUIWindowManager.ShowPreviousWindow();
          if (loadParamInfo.Play == "true")
            MyFilmsDetail.Launch_Movie(conf.StrIndex, GetID, null, MyFilmsDetail.PlayerOption.Internal);
          else
            GUIWindowManager.ActivateWindow((int)ID_MyFilmsDetail, true);
        }
      }
      else if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Search)) // search expression given in load params -> do global search !
      {
        LogMyFilms.Debug("OnPageLoad() - LoadParams - try loading search with search expression: '" + loadParamInfo.Search + "'");
        conf.Boolselect = false;
        SearchMoviesbyProperties(false, loadParamInfo.Search, string.Empty);
      }
      #endregion

      MyFilmsDetail.setGUIProperty("user.onlinestatus", Helper.IsTraktAvailableAndEnabled ? Helper.GetUserOnlineStatus(conf.StrUserProfileName) : "local");

      loadParamInfo = null; // all done, so "null" it to allow "normal browsing" from now on ...

      if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      {
        // Originally Deactivated by Zebons    
        // ********************************
        //if (!bgLoadMovieList.IsBusy)
        //{
        //  LogMyFilms.Debug("Launching AsynLoadMovieList");
        //  AsynLoadMovieList();
        //}
        // ********************************

        int itemIndex = facadeFilms.SelectedListItemIndex;
        // LogMyFilms.Debug("MyFilms.OnPageLoad() - itemindex = '" + itemIndex + "', facadeCount = '" + facadeFilms.Count + "'");
        if (itemIndex > -1) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
        else if (facadeFilms.Count > 0) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0);

        LogMyFilms.Debug("MyFilms.OnPageLoad() completed.");
      }
    }

    private void InitConfigPreload()
    {
      bool preCachingEnabled;
      LogMyFilms.Debug("InitConfigPreload() started preloading Config and DB.");
      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        preCachingEnabled = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PreCaching", false);
      }
      if (preCachingEnabled)
      {
        LogMyFilms.Info("InitConfigPreload() - preloading enabled - try preloading config and database");
        var preloadWatch = new Stopwatch(); preloadWatch.Reset(); preloadWatch.Start();
        PreviousConfig = Configuration.CurrentConfig;
        bool isDefaultConfig = Configuration.Current_Config(false); // don't show selection menu
        bool isvalidDb = Load_Config(Configuration.CurrentConfig, true, null);
        if (string.IsNullOrEmpty(Configuration.CurrentConfig))
        {
          LogMyFilms.Error("InitConfigPreload(): Config is empty - exiting precaching!");
          return;
        }
        if (!isvalidDb)
        {
          LogMyFilms.Error("InitConfigPreload(): Config is valid, but catalog format is not valid (AMC42) - exiting precaching!");
          return;
        }
        InitFSwatcher(); // load DB watcher for multiseat
        r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // if (InitialStart) BaseMesFilms.LoadMyFilms(conf.StrFileXml);
        preloadWatch.Stop();
        LogMyFilms.Info("InitConfigPreload() finished preloading Config '" + Configuration.CurrentConfig + "' and DB (" + preloadWatch.ElapsedMilliseconds + " ms).");
      }
      else
      {
        LogMyFilms.Debug("InitConfigPreload() finished without preloading Config and DB.");
      }
    }

    private static void SyncConfigFromRemoteServer()
    {
      if (!File.Exists(Config.GetFolder(Config.Dir.Config) + @"\MyFilmsServer.xml"))
      {
        LogMyFilms.Warn("SyncConfigFromRemoteServer() - local file MyFilmsServer.xml not found - cannot read sync settings - exit sync.");
        return;
      }
      XmlConfig myFilmsServer = new XmlConfig();
      string myFilmsCentralConfigDir = myFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", "");
      bool syncFromServerOnStartup = myFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
      LogMyFilms.Info("SyncConfigFromRemoteServer() - SyncOnStartup = '" + syncFromServerOnStartup + "'");
      if (syncFromServerOnStartup && File.Exists(myFilmsCentralConfigDir + @"\MyFilms.xml"))
      {
        LogMyFilms.Info("SyncConfigFromRemoteServer() - Server Sync is enabled - remote directory: '" + myFilmsCentralConfigDir + "'");
        string serverConfigFile = myFilmsCentralConfigDir + @"\MyFilms.xml";
        string localConfigFile = Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml";
        if (!Directory.Exists(myFilmsCentralConfigDir))
        {
          LogMyFilms.Error("SyncConfigFromRemoteServer() - remote directory is not accessible !");
        }
        else if (!File.Exists(serverConfigFile))
        {
          LogMyFilms.Error("SyncConfigFromRemoteServer() - remote config file note found !");
        }
        else
        {
          //if (File.Exists(localConfigFile))
          //{
          //  try
          //  {
          //    string backupfile = localConfigFile.Replace(".xml", " - " + DateTime.Now.ToString("u").Replace(":", "-") + ".xml").Replace("/", "-");
          //    File.Copy(localConfigFile, backupfile, true);
          //  }
          //  catch (Exception)
          //  {
          //    LogMyFilms.Error("SyncConfigFromRemoteServer() - could not backup local MyFilms.xml config file !");
          //  }
          //}
          try
          {
            File.Copy(serverConfigFile, localConfigFile, true);
            LogMyFilms.Info("SyncConfigFromRemoteServer() - Successfully copied remote config to local config");
          }
          catch (Exception)
          {
            LogMyFilms.Error("SyncConfigFromRemoteServer() - could not copy remote config to local config file !");
          }
        }
      }
      else
      {
        LogMyFilms.Info("SyncConfigFromRemoteServer() - Server Sync is disabled - running on local config only !");
      }
    }

    private static void CleanOrphanedDBlocks()
    {
      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
        var configs = new List<string>();
        for (var i = 0; i < mesFilmsNbConfig; i++)
          configs.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));
        XmlSettings xmlSettings = xmlConfig;
        foreach (string strFileXml in configs.Select(config => xmlSettings.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty)))
        {
          try
          {
            MyFilmsDetail.SetGlobalLock(false, strFileXml); // release global lock, if there is any, after initializing (this is cleanup for older leftovers)
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("MyFilms.CleanOrphanedDBlocks - Error: Path = '" + strFileXml + "', -> " + ex.Message + ".");
          }
        }
      }
    }

    protected override void OnPageDestroy(int newWindowId)
    {
      LogMyFilms.Debug("MyFilms.OnPageDestroy(" + newWindowId + ") started.");

      // save current GUIlist in navigation cache, also stops any remaining facade background worker
      if (facadeFilms != null) SaveListState(false);

      GUIBackgroundTask.Instance.StopBackgroundTask();

      loadParamInfo.SafeDispose();
      //// Reset to force republishing details on reentering
      //Prev_ItemID = -1;
      //Prev_Label = string.Empty;
      //Thread.Sleep(5); // sleep 5 milliseconds
      // Set Facadevisibilities false ...
      //SetDummyControlsForFacade(ViewContext.None);

      //Disable Random Fanart Timer
      _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);

      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the next start to fix "visibility animations" ....

      if (!string.IsNullOrEmpty(Configuration.CurrentConfig) && conf.StrSelect != null)
      {
        if (facadeFilms == null || facadeFilms.SelectedListItemIndex == -1)
          Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
        else
          Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
      }

      GUITextureManager.CleanupThumbs();
      LogMyFilms.Debug("MyFilms.OnPageDestroy(" + newWindowId + ") completed.");
      Log.Debug("MyFilms.OnPageDestroy() completed. See MyFilms.log for further Details.");
      base.OnPageDestroy(newWindowId);
    }

    protected override void OnShowContextMenu()
    {
      LogMyFilms.Debug("OnShowContextMenu() started");
      if (facadeFilms.SelectedListItemIndex > -1)
      {
        if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
        return;
      }
      base.OnShowContextMenu();
    }
    #endregion

    #region Action

    //---------------------------------------------------------------------------------------
    //   Handle Keyboard Actions
    //---------------------------------------------------------------------------------------

    public override void OnAction(Action action)
    {
      LogMyFilms.Debug("OnAction() -  " + action.wID);
      if (action.m_key != null) LogMyFilms.Debug("action.m_key.KeyChar = '" + action.m_key.KeyChar + "'");

      switch (action.wID)
      {
        case Action.ActionType.ACTION_MOVE_LEFT:
        case Action.ActionType.ACTION_MOVE_RIGHT:
          base.OnAction(action);
          return;

        case Action.ActionType.ACTION_PARENT_DIR:
          if (GetPrevFilmList()) return;
          break;
        case Action.ActionType.ACTION_PREVIOUS_MENU:
          if (!facadeFilms.Focus)
          {
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // set focus to facade, if e.g. menu buttons had focus (after global options etc.)
            return;
          }

          // check, if it was an internal call 
          if (internalLoadParam != null)
          {
            internalLoadParam = null;
            GUIWindowManager.ShowPreviousWindow();
          }
        
          // LogStatusVars("PreviousMenu");
          string viewStateCacheName = null != GetCustomViewFromViewLabel(conf.CurrentView) ? "CustomView_" + conf.WStrSort : conf.WStrSort;
          if (!string.IsNullOrEmpty(viewStateCacheName))
            SaveLastView(viewStateCacheName);

          if (_navigationStack.Count > 0) //_backPos
          {
            DoBack(); // also stops facade background worker
            if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            //UpdateGui();
            return;
          }

          //if (ViewHistory.Count > 0)
          //{
          //  RestoreLastView();
          //  r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with restored former filters to get proper counts ...
          //  Refreshfacade();
          //  return;
          //}

          switch (conf.ViewContext)
          {
            case ViewContext.None: // do nothing, if no valid context is there (e.g. because there is still backgroundloading of menu active...)
            case ViewContext.StartView:
              #region None
              StopLoadingViewDetails = true;
              StopLoadingMenuDetails = true;
              StopLoadingFilmlistDetails = true;
              GUIWindowManager.ShowPreviousWindow();
              #endregion
              return;
            case ViewContext.MenuAll:
              #region MenuAll
              StopLoadingMenuDetails = true;
              // if there is no views defined :
              if (conf.CustomViews.View.Rows.Count == 0)
              {
                if (conf.AlwaysShowConfigMenu) // show config menu selection, if selected in setup on "leaving"
                {
                  if (!ChooseNewConfig()) GUIWindowManager.ShowPreviousWindow(); // if user "escapes", return to previous window / quit
                  return;
                }
                base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
                return;
              }
              conf.MenuSelectedId = -2; // -2 means coming from MenuAll
              GetSelectFromMenuView(false); // Call simple Menu ...
              #endregion
              break;
            case ViewContext.Menu:
              #region Menu
              StopLoadingMenuDetails = true;
              if (conf.AlwaysShowConfigMenu) // show config menu selection, if selected in setup on "leaving"
              {
                if (!ChooseNewConfig()) GUIWindowManager.ShowPreviousWindow(); // if user "escapes", return to previous window / quit
                return;
              }
              base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
              #endregion
              return;

            default:
              #region default backnavigation

              base.OnAction(action); // return to previous window, if no backnavigation rule applies
              return;

              // following code was deactivated to use "simplified" backnavigation, instead "enhanced" backnavigation on user request
              #region deactivatged backnavigation code
              //if (GetPrevFilmList()) // try moving up in film list hierarchy - is false, when already on top
              //  return;

              //if (conf.StrTxtSelect == "" || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) //"All" or "Global Filter"
              //{
              //  #region this was already "root" view - so jumping back should leave the plugin !
              //  base.OnAction(action); // GUIWindowManager.ShowPreviousWindow();
              //  return;
              //  #endregion
              //}

              //// if (conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect)) //original code block refactored // 1079870 = "Selection" 
              //if (conf.StrTxtSelect == (GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect) || (conf.Boolreturn && IsPersonField(conf.wStrSort)))
              //{
              //  #region switch to selected root node here ... full list or menu
              //  if (Helper.FieldIsSet(conf.StrViewDfltItem) && conf.StrViewDfltItem != GUILocalizeStrings.Get(1079819) && conf.AlwaysDefaultView) // 1079819 = Menu
              //  {
              //    // DisplayAllMovies();

              //    conf.Boolselect = false;
              //    conf.Boolreturn = false;
              //    conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
              //    conf.StrIndex = 0;
              //    conf.Boolindexed = false;
              //    conf.Boolindexedreturn = false;

              //    viewcover.Filename = "";
              //    personcover.Filename = "";
              //    groupcover.Filename = "";

              //    GetFilmList();
              //    SetLabelView("all");
              //    // SetDummyControlsForFacade(conf.ViewContext);

              //    // base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
              //  }
              //  else
              //  {
              //    GetSelectFromMenuView(conf.BoolMenuShowAll);  // Call Menu with last detail state ...
              //  }
              //  return;
              //  #endregion
              //}

              //if (conf.Boolreturn)
              //#region boolreturn returns to view here
              //{
              //  conf.Boolreturn = false;
              //  Change_View_Action(conf.wStrSort);
              //  return;
              //}
              //#endregion

              //if (conf.Boolselect)
              //#region boolselect returns to defaultview or menu here
              //{
              //  {
              //    // switch to selected root node here ...
              //    if (Helper.FieldIsSet(conf.StrViewDfltItem) && conf.AlwaysDefaultView)
              //    {
              //      SetLabelView("all");

              //      conf.ViewContext = MyFilms.ViewContext.StartView;
              //      conf.StrIndex = -1;
              //      conf.LastID = -1;
              //      conf.Wstar = "";
              //      conf.Boolreturn = false;
              //      conf.Boolselect = true;
              //      conf.Boolindexed = false;
              //      conf.Boolindexedreturn = false;

              //      viewcover.Filename = "";
              //      personcover.Filename = "";
              //      groupcover.Filename = "";

              //      conf.Wselectedlabel = conf.StrViewDfltText;
              //      Change_View_Action(conf.StrViewDfltItem);
              //    }
              //    else
              //    {
              //      GetSelectFromMenuView(conf.BoolMenuShowAll);
              //    }
              //    return;
              //  }
              //}
              //#endregion

              //// switch to selected root node here ...
              //if (Helper.FieldIsSet(conf.StrViewDfltItem) && conf.StrViewDfltItem != GUILocalizeStrings.Get(1079819) && conf.AlwaysDefaultView) // 1079819 = Menu
              //{
              //  base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
              //}
              //else
              //{
              //  GetSelectFromMenuView(conf.BoolMenuShowAll);  // Call Menu with last detail state ...
              //}
              //return;

              //// GetSelectFromMenuView(conf.BoolMenuShowAll);  // Call Menu with last detail state ...
              //// base.OnAction(action); // return to previous window, if no backnavigation rule applies
              #endregion
              #endregion
          }
          break;
        case Action.ActionType.ACTION_KEY_PRESSED:
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_MUSIC_PLAY:
        case Action.ActionType.ACTION_PLAY:
          // Play groups as playlist (ToDo)
          if (facadeFilms.Focus && !facadeFilms.SelectedListItem.IsFolder)
          {
            conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, null, MyFilmsDetail.PlayerOption.Internal);
          }
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_PREV_PICTURE:
        case Action.ActionType.ACTION_NEXT_PICTURE:
          // Cycle Artwork
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_CONTEXT_MENU:
          base.OnAction(action);
          break;
        default:
          if (action.m_key != null)
          {
            if (action.m_key.KeyChar == 114 && facadeFilms.Focus) // 114 = "r" -> select random movie in current list
            {
              SelectRandomMovie();
            }
            if (action.m_key.KeyChar == 112 && facadeFilms.Focus && !facadeFilms.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
            {
              conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
              conf.StrTIndex = facadeFilms.SelectedListItem.Label;
              MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, null, MyFilmsDetail.PlayerOption.Internal);
            }
            if ((action.m_key.KeyChar == 120) && Context_Menu)
            {
              Context_Menu = false;
              return;
            }
            if (action.m_key.KeyChar == 120 && facadeFilms.Focus && !facadeFilms.SelectedListItem.IsFolder)
            {
              Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId); // context menu for update or suppress entry
              return;
            }
          }
          if (action.wID.ToString().Substring(0, 6) == "REMOTE") return;
          base.OnAction(action);
          break;
      }
    }

    ////---------------------------------------------------------------------------------------
    ////   Handle Clicked Events
    ////---------------------------------------------------------------------------------------
    //protected override void OnClicked(int iControl, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    //{
    //  LogMyFilms.Debug("OnClicked() started");
    //  //if (control == this.viewMenuButton)
    //  //{
    //  //  showViewSwitchDialog();
    //  //  viewMenuButton.Focus = false;
    //  //  return;
    //  //}
    //  //---------------------------------------------------------------------------------------
    //  // Mouse/Keyboard Clicked
    //  //---------------------------------------------------------------------------------------
    //  if (iControl == (int)Controls.CTRL_ListFilms)
    //  #region Item selected in facade - do action ...
    //  {
    //    if (facadeFilms.SelectedListItemIndex > -1)
    //    {
    //      LogStatusVars("SelectItem");
    //      if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
    //      {
    //        switch (conf.ViewContext)
    //        {
    //          case ViewContext.None:
    //            break;
    //          case ViewContext.Menu:
    //          case ViewContext.MenuAll:
    //            if (this.facadeFilms.SelectedListItem.DVDLabel == "showall")
    //            {
    //              conf.MenuSelectedID = -1;
    //              GetSelectFromMenuView(true);
    //            }
    //            else
    //            {
    //              conf.MenuSelectedID = this.facadeFilms.SelectedListItemIndex; // remember last menu position ...
    //              Change_View_Action(this.facadeFilms.SelectedListItem.DVDLabel);
    //            }
    //            break;

    //          default:
    //            if (conf.IndexedChars > 0 && conf.Boolindexed && conf.Wstar == "*") // enter indexed view ...
    //            {
    //              conf.Boolindexed = false;
    //              conf.Boolindexedreturn = true;
    //              GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, this.facadeFilms.SelectedListItem.Label, true, ""); // conf.StrSelect = conf.wStrSort + " like '*" + conf.Wstar + "*'"; // only for actors ! - so do it later in method ...
    //            }
    //            else if (!this.facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect) // New Window for detailed selected item information
    //            {
    //              conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
    //              conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
    //              GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
    //            }
    //            else // View List as selected
    //            {
    //              conf.Wselectedlabel = (conf.BoolReverseNames && this.facadeFilms.SelectedListItem.Label != EmptyFacadeValue) ? ReReverseName(conf.Wselectedlabel) : this.facadeFilms.SelectedListItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value
    //              Change_Layout_Action(MyFilms.conf.StrLayOut);
    //              conf.Boolreturn = (!this.facadeFilms.SelectedListItem.IsFolder);
    //              do
    //              {
    //                if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
    //                conf.StrTitleSelect += conf.Wselectedlabel;
    //              }
    //              while (GetFilmList() == false); //keep calling while single folders found
    //            }
    //            break;
    //        }
    //      }
    //      else if(actionType == Action.ActionType.ACTION_MUSIC_PLAY || actionType == Action.ActionType.ACTION_PLAY)
    //      {
    //        // add filmstart option here
    //      }
    //    }
    //  }
    //  #endregion
    //  else if (iControl == (int)Controls.CTRL_BtnSearch)
    //  {
    //    if (conf.Boolselect) conf.Boolselect = false;
    //    Change_Search_Options();
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnSortBy)
    //  {
    //    this.Change_Sort_Option_Menu();
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnViewAs)
    //  {
    //    this.Change_View_Menu();
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnOptions)
    //  {
    //    Change_Option();
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnGlobalUpdates)
    //  {
    //    Change_Menu_Action("globalupdates");
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }

    //  if (iControl == (int)Controls.CTRL_BtnGlobalOverlayFilter)
    //  {
    //    Change_Global_Filters();
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }

    //  else if (iControl == (int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus)
    //  {
    //    ToggleGlobalUnwatched();
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnLayout)
    //  {
    //    Change_Layout();
    //    GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
    //  }
    //  if (actionType != MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
    //  base.OnClicked(iControl, control, actionType);
    //}

    //---------------------------------------------------------------------------------------
    //   Handle posted Messages
    //---------------------------------------------------------------------------------------
    public override bool OnMessage(GUIMessage message)
    {
      int dControl = message.TargetControlId;
      int iControl = message.SenderControlId;
      LogMyFilms.Debug("GUIMessage: " + message.Message + ", Param1: " + message.Param1 + ", Sender: " + iControl + ", Target: " + dControl + "");
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          return base.OnMessage(message);

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          return base.OnMessage(message);

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          // return base.OnMessage(message); // enable this, if clicked messages should be handled in OnClicked override ...
          #region MSG_CLICKED
          //---------------------------------------------------------------------------------------
          // Mouse/Keyboard Clicked
          //---------------------------------------------------------------------------------------
          // LogMyFilms.Debug("GUI_MSG_CLICKED recognized !");

          if (iControl == (int)Controls.CTRL_ListFilms && message.Param1 != 7) return true;  // we only handle "SELECT_ITEM" here - some other events raised onClicked too for some reason?

          switch (iControl)
          {
            case (int)Controls.CTRL_ListFilms:
              #region Item selected in facade - do action ...
              if (facadeFilms.SelectedListItemIndex > -1) // if (facadeFilms.SelectedListItemIndex > -1 && !bgOnPageLoad.IsBusy) // do not allow going to details when loading thread still active !!!
              {
                #region context dependant actions
                // LogStatusVars("SelectItem");
                switch (conf.ViewContext)
                {
                  case ViewContext.None:
                    break;
                  case ViewContext.Menu:
                  case ViewContext.MenuAll:
                    // if (conf.ViewContext == ViewContext.MenuAll) 
                    SaveListState(false); // save current display to navigation cache
                    switch (facadeFilms.SelectedListItem.DVDLabel)
                    {
                      case "showall":
                        conf.MenuSelectedId = -1;
                        GetSelectFromMenuView(true);
                        break;
                      case "onlineinfo":
                        GetSelectFromOnlineMenuView();
                        break;
                      case "TMDBaction":
                        GetSelectFromTmdb(facadeFilms.SelectedListItem.Label);
                        break;
                      default:
                        conf.MenuSelectedId = facadeFilms.SelectedListItemIndex; // remember last menu position ...
                        Change_View_Action(facadeFilms.SelectedListItem.DVDLabel);
                        break;
                    }
                    break;
                  case ViewContext.TmdbMovies:
                    #region TMDB online movies selection
                    Change_SelectTmdbEntry_Action(facadeFilms.SelectedListItem);
                    #endregion
                    break;

                  default:
                    if (conf.IndexedChars > 0 && conf.Boolindexed && conf.Wstar == "*" && !conf.BoolSkipViewState)
                    {
                      #region enter indexed view ...
                      SaveListState(false);
                      conf.Boolindexed = false;
                      conf.Boolindexedreturn = true;

                      new Thread(delegate()
                        {
                          MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show(); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
                          GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, facadeFilms.SelectedListItem.Label, true, ""); // conf.StrSelect = conf.wStrSort + " like '*" + conf.Wstar + "*'"; // only for actors ! - so do it later in method ...
                          MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation);  // GUIWaitCursor.Hide();
                          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => 0, 0, 0, null);
                        }) { Name = "GetSelectFromDivx", IsBackground = true }.Start();
                      #endregion
                    }
                    else if (!facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
                    {
                      #region open detailed window for selected item details information
                      // SaveListState(false); // already done on OnPageDestroy !
                      conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
                      conf.StrTIndex = facadeFilms.SelectedListItem.Label;
                      GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                      #endregion
                    }
                    else
                    {
                      #region  View List as selected
                      //conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
                      //conf.Boolselect = true;
                      //conf.Boolreturn = true;
                      //conf.Wstar = "*";
                      //if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
                      //// TxtSelect.Label = conf.StrTxtSelect;
                      //MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
                      //conf.StrSelect = conf.wStrSort;

                      //conf.StrFilmSelect = string.Empty;
                      //conf.Wselectedlabel = (selectedCustomView.Value != "*") ? selectedCustomView.Value : "";
                      //do
                      //{
                      //  if (conf.StrTitleSelect != string.Empty) conf.StrTitleSelect += conf.TitleDelim;
                      //  conf.StrTitleSelect += selectedCustomView.Value;
                      //}
                      //while (GetFilmList() == false); //keep calling while single folders found

                      SaveListState(false); // don't cache GetFilmList()
                      conf.Wselectedlabel = conf.BoolReverseNames && facadeFilms.SelectedListItem.Label != EmptyFacadeValue ? ReReverseName(facadeFilms.SelectedListItem.Label) : facadeFilms.SelectedListItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value
                      conf.Boolreturn = !facadeFilms.SelectedListItem.IsFolder;
                      new Thread(delegate()
                      {
                        do
                        {
                          if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                          conf.StrTitleSelect += conf.Wselectedlabel;
                        }
                        while (GetFilmList() == false); //keep calling while single folders found
                        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => { return 0; }, 0, 0, null);
                      }) { Name = "GetFilmList", IsBackground = true }.Start();
                      #endregion
                    }
                    break;
                }
                #endregion
              }
              #endregion
              break;
            case (int)Controls.CTRL_BtnSearch: // Search dialog search
              // conf.Boolselect = false; // removed, as it was breaking the back navigation stack !!!
              Change_Search_Options();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnSearch);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnSortBy:
              Change_Sort_Option_Menu();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnSortBy);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnViewAs:
              Change_View_Menu();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnViewAs);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnOptions:
              Change_Option();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnOptions);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnGlobalUpdates:
              Change_Menu_Action("globalupdates");
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnGlobalUpdates);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnOnlineMovieLists:
              GetSelectFromOnlineMenuView();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnOnlineMovieLists);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnGlobalOverlayFilter:
              Change_Global_Filters();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnGlobalOverlayFilter);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus:
              ToggleGlobalUnwatched();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnSelectRamdomMovie:
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnSelectRamdomMovie);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              SelectRandomMovie();
              break;
            case (int)Controls.CTRL_BtnLayout:
              Change_Layout();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnLayout);
              // GUIControl.RefreshControl(GetID, (int)Controls.CTRL_BtnLayout);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
          }
          #endregion
          return base.OnMessage(message);
      }
      return base.OnMessage(message);
    }
    #endregion

    #region TMDB methods
    private static IEnumerable<TmdbMovieSearchResult> GetPopularMovies(bool all)
    {
      var movies = new List<TmdbMovieSearchResult>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetPopularMovies - detected language = '" + language + "', all = '" + all + "'");
      var api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();
      watch.Reset(); watch.Start();
      int ipage = 1;
      while (true)
      {
        MovieSearchResults movieSearchResults = api.GetPopularMovies(ipage, language);
        LogMyFilms.Debug("GetPopularMovies() - Loaded Page: " + ipage + " (of " + movieSearchResults.total_pages + "), Total Results = '" + movieSearchResults.total_results + "', (" + watch.ElapsedMilliseconds + " ms)");
        movies.AddRange(movieSearchResults.results);
        ipage++;
        if (ipage > movieSearchResults.total_pages || !all || movies.Count > MaxTmdbResults) break;
      }
      // movies.OrderByDescending(x => x.vote_count).ThenByDescending(x => x.vote_average).ThenByDescending(x => x.release_date);
      watch.Stop();
      LogMyFilms.Debug("GetPopularMovies() - finished loading movies from TMDB (" + watch.ElapsedMilliseconds + " ms)");
      return movies.AsEnumerable().OrderByDescending(vc => vc.vote_count).ThenByDescending(va => va.vote_average).ThenByDescending(rd => rd.release_date);
    }

    private static IEnumerable<TmdbMovieSearchResult> GetNowPlayingMovies(bool all)
    {
      var movies = new List<TmdbMovieSearchResult>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetNowPlayingMovies - detected language = '" + language + "', all = '" + all + "'");
      var api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();
      watch.Reset(); watch.Start();
      int ipage = 1;
      while (true)
      {
        MovieSearchResults movieSearchResults = api.GetNowPlayingMovies(ipage, language);
        LogMyFilms.Debug("GetNowPlayingMovies() - Loaded Page: " + ipage + " (of " + movieSearchResults.total_pages + "), Total Results = '" + movieSearchResults.total_results + "', (" + watch.ElapsedMilliseconds + " ms)");
        movies.AddRange(movieSearchResults.results);
        ipage++;
        if (ipage > movieSearchResults.total_pages || !all || movies.Count > MaxTmdbResults) break;
      }
      // movies = movies.OrderByDescending(x => x.release_date).ToList();
      // movies.OrderByDescending(x => x.release_date).ThenByDescending(x => x.vote_average).ThenByDescending(x => x.vote_count);
      watch.Stop();
      LogMyFilms.Debug("GetNowPlayingMovies() - finished loading movies from TMDB (" + watch.ElapsedMilliseconds + " ms)");
      return movies.AsEnumerable().OrderByDescending(rd => rd.release_date).ThenByDescending(va => va.vote_average).ThenByDescending(vc => vc.vote_count);
    }

    private static IEnumerable<TmdbMovieSearchResult> GetTopRatedMovies(bool all)
    {
      var movies = new List<TmdbMovieSearchResult>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetTopRatedMovies - detected language = '" + language + "', all = '" + all + "'");
      var api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();

      watch.Reset(); watch.Start();
      int ipage = 1;
      while (true)
      {
        MovieSearchResults movieSearchResults = api.GetTopRatedMovies(ipage, language);
        LogMyFilms.Debug("GetTopRatedMovies() - Loaded Page: " + ipage + " (of " + movieSearchResults.total_pages + "), Total Results = '" + movieSearchResults.total_results + "', (" + watch.ElapsedMilliseconds + " ms)");
        movies.AddRange(movieSearchResults.results);
        ipage++;
        if (ipage > movieSearchResults.total_pages || !all || movies.Count > MaxTmdbResults) break;
      }
      // movies = movies.OrderByDescending(x => x.vote_average).ToList();
      // movies.OrderByDescending(x => x.vote_average).ThenByDescending(x => x.vote_count).ThenByDescending(x => x.release_date);
      watch.Stop();
      LogMyFilms.Debug("GetTopRatedMovies() - finished loading movies from TMDB (" + watch.ElapsedMilliseconds + " ms)");
      return movies.AsEnumerable().OrderByDescending(v => v.vote_average).ThenByDescending(vc => vc.vote_count).ThenByDescending(rd => rd.release_date);
    }

    private static IEnumerable<TmdbMovieSearchResult> GetUpcomingMovies(bool all)
    {
      var movies = new List<TmdbMovieSearchResult>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetUpcomingMovies - detected language = '" + language + "', all = '" + all + "'");
      var api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();

      watch.Reset(); watch.Start();
      int ipage = 1;
      while (true)
      {
        MovieSearchResults movieSearchResults = api.GetUpcomingMovies(ipage, language);
        LogMyFilms.Debug("GetUpcomingMovies() - Loaded Page: " + ipage + " (of " + movieSearchResults.total_pages + "), Total Results = '" + movieSearchResults.total_results + "', (" + watch.ElapsedMilliseconds + " ms)");
        movies.AddRange(movieSearchResults.results);
        ipage++;
        if (ipage > movieSearchResults.total_pages || !all || movies.Count > MaxTmdbResults) break;
      }
      // movies = movies.OrderBy(x => x.release_date).ToList();
      // movies.OrderBy(x => x.release_date).ThenByDescending(x => x.vote_average).ThenByDescending(x => x.release_date);
      watch.Stop();
      LogMyFilms.Debug("GetUpcomingMovies() - finished loading movies from TMDB (" + watch.ElapsedMilliseconds + " ms)");
      return movies.AsEnumerable().OrderBy(rd => rd.release_date).ThenByDescending(v => v.vote_average);
    }

    private static IEnumerable<TmdbMovieSearchResult> GetImdbTop250Movies(bool all)
    {
      List<TmdbMovieSearchResult> movies = new List<TmdbMovieSearchResult>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      var api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();
      LogMyFilms.Debug("GetImdbTop250Movies - detected language = '" + language + "', all = '" + all + "'");

      watch.Reset(); watch.Start();
      string wscript = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + "\\IMDB-Top250.xml";
      if (!File.Exists(wscript)) return movies;
      
      var grab = new GrabberUrlClass();
      var listUrl = new ArrayList();

      try
      {
        listUrl = grab.ReturnURL("top", wscript, 1, true, "");
      }
      catch (Exception ex)
      {
        LogMyFilms.Error(ex, "GetImdbTop250Movies() - exception = '" + ex.Message + "'");
      }

      LogMyFilms.Debug("GetImdbTop250Movies() - finished loading IMDB Top 250 movie list (" + watch.ElapsedMilliseconds + " ms)");

      foreach (object item in listUrl)
      {
        try
        {
          GrabberUrlClass.IMDBUrl wurl = (GrabberUrlClass.IMDBUrl)item;
          TmdbMovieSearchResult movie = new TmdbMovieSearchResult();
          TmdbMovie tmdbmovie = api.GetMovieByIMDB(wurl.ID);
          movie.id = tmdbmovie.id;
          movie.original_title = tmdbmovie.original_title;
          movie.release_date = tmdbmovie.release_date;
          movie.backdrop_path = tmdbmovie.backdrop_path;
          movie.poster_path = tmdbmovie.poster_path;

          movie.title = wurl.Title;
          // movie.id = -1; // tmdbmovie.id missing, get it later from IMDBid
          // movie.imdb_id = wurl.ID;
          // movie.original_title = wurl.Title;
          // movie.release_date = wurl.Year;
          int votecount;
          int.TryParse(wurl.Akas, out votecount);
          movie.vote_count = votecount; // int.Parse(wurl.Akas);
          Double voteaverage;
          Double.TryParse(wurl.Options.Replace(".", ","), out voteaverage);
          movie.vote_average = voteaverage;
          movies.Add(movie);
          LogMyFilms.Debug("GetImdbTop250Movies() - Rating: '" + movie.vote_average + "', Title: " + movie.title + "");
        }
        catch (Exception ex)
        {
          LogMyFilms.Error(ex, "GetImdbTop250Movies() - exception = '" + ex.Message + "'");
        }
      }
      // movies = movies.OrderBy(x => x.release_date).ToList();
      // movies.OrderBy(x => x.release_date).ThenByDescending(x => x.vote_average).ThenByDescending(x => x.release_date);
      watch.Stop();
      LogMyFilms.Debug("GetImdbTop250Movies() - finished loading IMDB Top 250 movies (" + watch.ElapsedMilliseconds + " ms)");
      return movies.AsEnumerable(); // .OrderBy(rd => rd.release_date).ThenByDescending(v => v.vote_average);
    }

    private static IEnumerable<TmdbMovieSearchResult> GetPersonMovies(string personname, bool all)
    {
      List<TmdbMovieSearchResult> movies = new List<TmdbMovieSearchResult>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetPersonMovies - detected language = '" + language + "', all = '" + all + "'");
      watch.Reset(); watch.Start();
      Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();

      TmdbPersonSearch tmdbPerson = api.SearchPerson(personname, 1);
      List<PersonResult> persons = tmdbPerson.results;

      if (persons != null && persons.Count > 0)
      {
        // PersonResult pinfo = persons[0];
        // TMDB.TmdbPerson singleperson = api.GetPersonInfo(pinfo.id);
        // TMDB.TmdbPersonImages images = api.GetPersonImages(pinfo.id);
        TmdbPersonCredits personFilmList = api.GetPersonCredits(persons[0].id, language);
        if (personFilmList.cast.Count == 0)
          personFilmList = api.GetPersonCredits(persons[0].id);

        movies.AddRange(personFilmList.cast.Select(credit => new TmdbMovieSearchResult
        {
          id = credit.id, title = credit.title, original_title = credit.character, poster_path = credit.poster_path, release_date = credit.release_date
          // ,person_credit = credit.original_title + "|" + credit.character
        }));
        movies.AddRange(personFilmList.crew.Select(credit => new TmdbMovieSearchResult
        {
          id = credit.id, title = credit.title, original_title = credit.job + " (" + credit.department + ")", poster_path = credit.poster_path, release_date = credit.release_date
          // ,person_credit = credit.original_title + "|" + credit.department
        }));
      }
      // movies = movies.OrderByDescending(x => x.release_date).ToList();
      // movies.OrderByDescending(x => x.release_date).ThenByDescending(x => x.vote_average).ThenByDescending(x => x.vote_count);
      watch.Stop();
      LogMyFilms.Debug("GetUpcomingMovies() - finished loading movies from TMDB, Total Results = '" + movies.Count + "', (" + watch.ElapsedMilliseconds + " ms)");

      return movies.AsEnumerable().OrderByDescending(r => r.release_date).ThenByDescending(v => v.vote_average).ThenByDescending(vc => vc.vote_count);
    }

    #endregion

    private void Change_Layout()
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      List<GUIFacadeControl.Layout> choiceLayoutMenu = new List<GUIFacadeControl.Layout>();

      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079901)); // View (Layout) ...
      dlg.Add(GUILocalizeStrings.Get(101));//List
      choiceLayoutMenu.Add(GUIFacadeControl.Layout.List);

      if (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll) // if (!conf.UseListViewForGoups || !conf.Boolselect)
      {
        dlg.Add(GUILocalizeStrings.Get(529));//Cover list - used as extended list
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.AlbumView);
        dlg.Add(GUILocalizeStrings.Get(100));//Icons
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.SmallIcons);
        dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.LargeIcons);
        dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.Filmstrip);
        dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.CoverFlow);
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;
      // int iLayout = (int)choiceLayoutMenu[dlg.SelectedLabel];

      if (facadeFilms.AlbumListLayout == null && dlg.SelectedLabel == 1) // return, if layout not supported by skin !
      {
        GUIUtils.ShowNotifyDialog("Your skin does not support this layout !");
        dlg.SelectedLabel = 0;
      }

      conf.StrIndex = 0; // reset movie index
      int wselectindex = facadeFilms.SelectedListItemIndex;

      Change_Layout_Action(dlg.SelectedLabel);

      switch (conf.ViewContext)
      {
        case ViewContext.TmdbMovies:
          conf.StrLayOut = dlg.SelectedLabel; // we share Layout for menu with Views ...
          break;
        case ViewContext.MenuAll:
        case ViewContext.Menu:
          conf.WStrLayOut = dlg.SelectedLabel; // we share Layout for menu with Views ...
          break;

        default:
          if (conf.Boolselect)
            conf.WStrLayOut = dlg.SelectedLabel;
          else if (conf.BoolCollection)
            conf.StrLayOutInHierarchies = dlg.SelectedLabel;
          else
            conf.StrLayOut = dlg.SelectedLabel;
          break;
      }
      dlg.DeInit();

      switch (conf.ViewContext)
      {
        case ViewContext.MenuAll:
        case ViewContext.Menu:
          GetSelectFromMenuView(conf.BoolMenuShowAll);
          break;
        case ViewContext.TmdbMovies:
          break;

        case ViewContext.Movie:
        case ViewContext.MovieCollection:
        case ViewContext.Group:
        case ViewContext.Person:
        case ViewContext.StartView:
        case ViewContext.None:
        default:
          if (conf.Boolselect)
            GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, "");
          else
            GetFilmList();
          break;
      }
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, wselectindex);
    }

    private void ToggleGlobalUnwatched()
    {
      conf.GlobalUnwatchedOnly = !conf.GlobalUnwatchedOnly;
      if (conf.GlobalUnwatchedOnly)
      {
        //GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
        GlobalFilterStringUnwatched = CreateGlobalUnwatchedFilter();
        MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
      }
      else
      {
        GlobalFilterStringUnwatched = String.Empty;
        MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
      }
      LogMyFilms.Info("Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
      Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
      // use this code later to set the label of the button
      //if (MyFilms.conf.GlobalUnwatchedOnly)
      //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628));
      //else
      //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));
    }

    private string ReverseName(string nonreversedName)
    {
      return nonreversedName.Contains(" ")
        ? nonreversedName.Substring(nonreversedName.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) + ", " + nonreversedName.Substring(0, nonreversedName.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase))
        : nonreversedName;
    }

    private string ReReverseName(string reversedName)
    {
      return reversedName.Contains(", ")
        ? reversedName.Substring(reversedName.LastIndexOf(", ", StringComparison.Ordinal) + 2) + " " + reversedName.Substring(0, reversedName.LastIndexOf(", ", StringComparison.Ordinal))
        : reversedName;
    }

    public static ArrayList GetDisplayItems(string displayoption)
    {
      var itemsToDisplay = new ArrayList();
      using (var ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          if (dc.ColumnName == null || dc.ColumnName == "Contents_Id" || dc.ColumnName == "Movie_Id" || dc.ColumnName == "IsOnline" || dc.ColumnName == "IsOnlineTrailer" || dc.ColumnName == "LastPosition" ||
              dc.ColumnName == BaseMesFilms.MultiUserStateField || dc.ColumnName == "VirtualPathTitle" ||
              (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 && !MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Restrict DB field Selection for AMC3") &&
               (dc.ColumnName == "IMDB_Id" || dc.ColumnName == "TMDB_Id" || dc.ColumnName == "Watched" ||
                dc.ColumnName == "Certification" || dc.ColumnName == "Writer" || dc.ColumnName == "SourceTrailer" ||
                dc.ColumnName == "TagLine" || dc.ColumnName == "Tags" || dc.ColumnName == "RatingUser" ||
                dc.ColumnName == "Studio" || dc.ColumnName == "IMDB_Rank" || dc.ColumnName == "Edition" ||
                dc.ColumnName == "Aspectratio" || dc.ColumnName == "AudioChannelCount" ||
                dc.ColumnName == "CategoryTrakt" || dc.ColumnName == "Favorite" || dc.ColumnName == "Fanart" ||
                dc.ColumnName == "AlternateTitles" || dc.ColumnName == "DateWatched"))) continue;
          switch (displayoption)
          {
            case "sort":
              if (//dc.ColumnName != "OriginalTitle" && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "FormattedTitle" && 
                dc.ColumnName != "IndexedTitle" && dc.ColumnName != "Comments" &&
                dc.ColumnName != "Description" && // dc.ColumnName != "URL" && 
                dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "AlternateTitles"
                )
                itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.TranslateColumn(dc.ColumnName) });
              break;
            case "view":
              if (
                //dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                dc.ColumnName != "Description" && dc.ColumnName != "Comments" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "AlternateTitles"
                )
                itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.TranslateColumn(dc.ColumnName) });
              break;
            case "deletedetails":
              if (
                dc.ColumnName != conf.StrStorage && dc.ColumnName != conf.StrStorageTrailer &&
                dc.ColumnName != conf.StrTitle1 && // dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                dc.ColumnName != "Number"
                )
                itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.TranslateColumn(dc.ColumnName) });
              break;
            case "titles":
              if (dc.ColumnName == "TranslatedTitle" || dc.ColumnName == "OriginalTitle" || dc.ColumnName == "FormattedTitle") // || dc.ColumnName == "VirtualPathTitle" || dc.ColumnName == "AlternateTitles"
                itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.TranslateColumn(dc.ColumnName) });
              break;
            case "viewitems":
              if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart") // added "DatedAdded" to remove filter
                itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.TranslateColumn(dc.ColumnName) });
              break;

            default:
              if (dc.ColumnName == "MediaLabel" || dc.ColumnName == "MediaType" || dc.ColumnName == "Source" || (dc.ColumnName == "SourceTrailer" && conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended) ||
                  dc.ColumnName == "URL" || dc.ColumnName == "Comments" || dc.ColumnName == "Borrower" ||
                  dc.ColumnName == "Languages" || dc.ColumnName == "Subtitles")
              {
                //AntStorage.Items.Add(dc.ColumnName);
                //AntStorageTrailer.Items.Add(dc.ColumnName);
              }

              if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded") // added "DatedAdded" to remove filter
              {
                //AntFilterItem1.Items.Add(dc.ColumnName);
                //AntFilterItem2.Items.Add(dc.ColumnName);
              }
              if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded")
              {
                //AntSearchField.Items.Add(dc.ColumnName);
                //AntUpdField.Items.Add(dc.ColumnName);
              }
              if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                  dc.ColumnName != "Actors" && dc.ColumnName != "DateAdded" &&
                  dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
              {
                //AntSearchItem1.Items.Add(dc.ColumnName);
                //AntSearchItem2.Items.Add(dc.ColumnName);
              }
              if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                  dc.ColumnName != "Year" && dc.ColumnName != "Date" && dc.ColumnName != "DateAdded" && // disabled for Doug testing
                  dc.ColumnName != "Length" && dc.ColumnName != "Rating" &&
                  dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
              {
                //AntSort1.Items.Add(dc.ColumnName);
                //AntSort2.Items.Add(dc.ColumnName);
                //AntIdentItem.Items.Add(dc.ColumnName);
              }
              if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
              {
                //AntUpdItem1.Items.Add(dc.ColumnName);
                //AntUpdItem2.Items.Add(dc.ColumnName);
                //cbfdupdate.Items.Add(dc.ColumnName);
                //cbWatched.Items.Add(dc.ColumnName);
                //CmdPar.Items.Add(dc.ColumnName);
              }
              break;
          }
        }
      }
      // foreach (string[] strings in ItemsToDisplay) LogMyFilms.Debug("GetDisplayItems() - Unsorted: '" + strings[0] + "', '" + strings[1] + "'");
      IComparer myComp = new myDBfieldComparer();
      itemsToDisplay.Sort(0, itemsToDisplay.Count, myComp);
      // foreach (string[] strings in ItemsToDisplay) LogMyFilms.Debug("GetDisplayItems() - Sorted  : '" + strings[0] + "', '" + strings[1] + "'");
      LogMyFilms.Debug("GetDisplayItems() - returning '" + itemsToDisplay.Count + "' display items (sorted)");
      return itemsToDisplay;
    }

    private void LogStatusVars(string caller)
    {
      LogMyFilms.Debug(caller + string.Format("() - Lst_Detail - Prev_ItemID - Prev_Label : '{0}' - '{1}'", Prev_ItemID, Prev_Label));

      LogMyFilms.Debug(caller + string.Format("() - StrSelect          : '{0}'", conf.StrSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrTitleSelect     : '{0}'", conf.StrTitleSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrFilmSelect      : '{0}'", conf.StrFilmSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrDfltSelect      : '{0}'", conf.StrDfltSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrPersons         : '{0}'", conf.StrPersons));

      LogMyFilms.Debug(caller + string.Format("() - ViewContext        : '{0}'", Enum.GetName(typeof(ViewContext), conf.ViewContext)));
      LogMyFilms.Debug(caller + string.Format("() - StrTxtView         : '{0}' (View)", conf.StrTxtView));
      LogMyFilms.Debug(caller + string.Format("() - StrTxtSelect       : '{0}' (Selection)", conf.StrTxtSelect));
      //LogMyFilms.Debug(caller + string.Format("() - CurrentView        : '{0}'", conf.CurrentView.SaveToString()));

      LogMyFilms.Debug(caller + string.Format("() - Boolselect         : '{0}'", conf.Boolselect));
      LogMyFilms.Debug(caller + string.Format("() - Boolreturn         : '{0}'", conf.Boolreturn));
      LogMyFilms.Debug(caller + string.Format("() - Boolindexed        : '{0}'", conf.Boolindexed));
      LogMyFilms.Debug(caller + string.Format("() - Boolindexedreturn  : '{0}'", conf.Boolindexedreturn));
      LogMyFilms.Debug(caller + string.Format("() - IndexedChars       : '{0}'", conf.IndexedChars));

      LogMyFilms.Debug(caller + string.Format("() - wStrSort           : '{0}'", conf.WStrSort));
      LogMyFilms.Debug(caller + string.Format("() - Wselectedlabel     : '{0}'", conf.Wselectedlabel));
      LogMyFilms.Debug(caller + string.Format("() - Wstar              : '{0}'", conf.Wstar));

      LogMyFilms.Debug(caller + string.Format("() - StrSorta                       : '{0}'", conf.StrSorta));
      LogMyFilms.Debug(caller + string.Format("() - StrSortSens                    : '{0}'", conf.StrSortSens));

      LogMyFilms.Debug(caller + string.Format("() - StrSortaInHierarchies          : '{0}'", conf.StrSortaInHierarchies));
      LogMyFilms.Debug(caller + string.Format("() - StrSortSensInHierarchies       : '{0}'", conf.StrSortSensInHierarchies));

      LogMyFilms.Debug(caller + string.Format("() - StrLayOut (Layout)             : '{0}'", conf.StrLayOut));
      LogMyFilms.Debug(caller + string.Format("() - WStrLayOut               : '{0}'", conf.WStrLayOut));
      LogMyFilms.Debug(caller + string.Format("() - StrLayOutInHierarchies         : '{0}'", conf.StrLayOutInHierarchies));

      //LogMyFilms.Debug(caller + string.Format("() - LastID            : '{0}'", conf.LastID));
      //LogMyFilms.Debug(caller + string.Format("() - IndexItem: '{0}'", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"));
      //LogMyFilms.Debug(caller + string.Format("() - TitleItem: '{0}'", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel) : string.Empty));

      // LogMyFilms.Debug(caller + string.Format("() - StrUserProfileName: '{0}'", conf.StrUserProfileName));
      // LogMyFilms.Debug(caller + string.Format("() - StrSearchHistory: '{0}'", conf.StrSearchHistory));
    }

    /// <summary>Jumps to prev folder in FilmList  by modifying Selects and calling GetFilmList</summary>
    /// <returns>If returns false means cannot jump back any further, so caller must exit plugin to main menu.</returns>
    bool GetPrevFilmList()
    {
      // LogStatusVars("GetPrevFilmList");
      Prev_ItemID = -1;
      Prev_Label = string.Empty;
      // lastPublished = 0; // set timer to 0 to make sure no delayed publish is done (instead immediately !)

      if (!string.IsNullOrEmpty(conf.StrTitleSelect))
      {
        #region moving back in film list hierarchies
        string selItem = NewString.NPosRight(conf.TitleDelim, conf.StrTitleSelect, -1, false, false);
        conf.StrTitleSelect = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) > 0 ? NewString.NPosLeft(conf.TitleDelim, conf.StrTitleSelect, -1, false, false) : "";
        if (GetFilmList(selItem) == false) // if single folder then call this func to jump back again
          return GetPrevFilmList();
        #endregion
      }
      else
      {
        return false;
        #region view handling (disabled for new backnavigation)
        //if (conf.StrTxtSelect == (GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect)) //original code block refactored // 1079870 = "Selection" // if (conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect)) //original code block refactored // 1079870 = "Selection"
        //{
        //  #region jump back to main full list or menu
        //  conf.Boolselect = false;
        //  conf.Boolreturn = false;
        //  conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
        //  conf.StrIndex = 0;
        //  conf.Boolindexed = false;
        //  conf.Boolindexedreturn = false;
        //  GetFilmList();
        //  SetLabelView("all"); // if back on "root", show view as "movies"
        //  SetLabelSelect("root");
        //  // SetDummyControlsForFacade(conf.ViewContext);
        //  #endregion
        //  return false;
        //}

        //if (conf.StrTxtSelect == "" || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) //"All" or "Global Filter"
        //{
        //  return false; //this was already "root" view - so jumping back should leave the plugin !
        //}
        //return false;
        //if (GetCustomViewFromViewLabel(conf.CurrentView) != null && GetCustomViewFromViewLabel(conf.CurrentView).Value.Length > 0) // if it's a custom view and a filter value is set
        //  return false;
        //else
        //{
        //  #region Jump back to prev view_display (categorised by year, genre etc) // Removed ACTORS special handling
        //  // selItem = NewString.StripChars(@"[]", conf.StrTxtSelect); // Moved one up to first set selItem to the actor and thus get back to correct facade position
        //  if (conf.StrTxtSelect.IndexOf(@"[") >= 0 && conf.StrTxtSelect.IndexOf(@"]") > 0)
        //  {
        //    selItem = conf.StrTxtSelect.Substring(conf.StrTxtSelect.IndexOf(@"["));
        //    selItem = selItem.Substring(0, selItem.IndexOf(@"]"));
        //    selItem = NewString.StripChars(@"[]", selItem);
        //    if (IsPersonField(conf.wStrSort) && conf.BoolReverseNames)
        //      selItem = ReverseName(selItem); // selItem = ReReverseName(selItem);
        //  }
        //  conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
        //  #endregion

        //  if (IsPersonField(conf.wStrSort) && !string.IsNullOrEmpty(conf.StrPersons)) // Specialhandling for Actors search view
        //  {
        //    conf.StrSelect = conf.wStrSort + " like '*" + StringExtensions.EscapeLikeValue(conf.StrPersons) + "*'";
        //    GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, conf.StrPersons, true, selItem);
        //    conf.StrPersons = "";
        //  }
        //  else
        //  {
        //    #region return indexed view
        //    if (conf.IndexedChars > 0 && conf.Boolindexedreturn)
        //    {
        //      if (conf.StrSelect == "")
        //      {
        //        conf.Boolindexedreturn = false;
        //        conf.Boolindexed = true;
        //        conf.Wstar = "*";
        //        // conf.Wstar = conf.wStrSort.Substring(0, 1); 
        //        selItem = NewString.StripChars(@"*", selItem); // remove stars, e.g. "K*" should be "K"
        //        // selItem = selItem.Substring(0, 1);
        //      }
        //      else
        //      {
        //        conf.StrSelect = ""; // reset view filter ...
        //      }
        //    }
        //    else
        //    {
        //      conf.StrSelect = ""; // reset view filter ...
        //      conf.Wstar = "*";
        //    }
        //    #endregion
        //    GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, conf.Wstar, true, selItem); // GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, "*", true, selItem);
        //  }
        //}
        #endregion
      }
      // SetDummyControlsForFacade(conf.ViewContext); // will be set in OnItemSelected handler / Lst_Details ...
      return true;
    }

    /// <summary>Sets StrFilmSelect up based on StrSelect, StrTitleSelect etc... </summary>
    static void SetFilmSelect()
    {
      string s = "";
      Prev_ItemID = -1;
      Prev_Label = string.Empty;
      if (conf.Boolselect)
      {
        string sLabel = conf.Wselectedlabel;
        bool labelNotEmpty = sLabel != "";
        Type columnType = GetColumnType(conf.WStrSort);
        string datatype;
        using (AntMovieCatalog ds = new AntMovieCatalog())
        {
          datatype = ds.Movie.Columns[conf.WStrSort].DataType.Name; // == "string"
        }

        LogMyFilms.Debug("(SetFilmSelect) - DB-Field: '{0}', datatype = '{1}', ColumnType = '{2}'", conf.WStrSort, datatype, columnType);
        if (columnType != typeof(string))
          conf.StrSelect = labelNotEmpty ? conf.WStrSort + " = '" + sLabel + "'" : conf.WStrSort + " is NULL";
        else switch (conf.WStrSort)
        {
          case "Date":
            conf.StrSelect = labelNotEmpty ? "DateAdded" + " = '" + DateTime.Parse(sLabel).ToShortDateString() + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
            break;
          case "DateWatched":
            conf.StrSelect = labelNotEmpty ? "DateWatched" + " = '" + DateTime.Parse(sLabel).ToShortDateString() + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
            break;
          default:
            if (IsDateField(conf.WStrSort))
              conf.StrSelect = labelNotEmpty ? conf.WStrSort + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
            else if (IsAlphaNumericalField(conf.WStrSort))
              conf.StrSelect = labelNotEmpty ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
            else
            {
              if (conf.BoolDontSplitValuesInViews)
                conf.StrSelect = labelNotEmpty ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
              else
                conf.StrSelect = labelNotEmpty ? conf.WStrSort + " like '*" + StringExtensions.EscapeLikeValue(sLabel) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
            }
            break;
        }

        conf.StrTxtSelect = labelNotEmpty ? "" + sLabel + "" : "" + EmptyFacadeValue + "";
        conf.StrTitleSelect = "";
        conf.Boolselect = false;
        if (conf.BoolSkipViewState)
        {
          conf.StrSelect = labelNotEmpty ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
          // conf.StrTxtSelect = (LabelNotEmpty) ? "[" + sLabel + "*]" : "[" + EmptyFacadeValue + "]";
        }
      }
      else
      {
        conf.StrTxtSelect = NewString.NPosLeft(@"\", conf.StrTxtSelect, 1, false, false); //strip old path if any
        if (conf.StrTitleSelect != "") conf.StrTxtSelect += @"\" + conf.StrTitleSelect; // append new path if any
      }

      if (conf.StrSelect != "") s = conf.StrSelect + " And ";
      if (conf.StrTitleSelect != "")
        s = s + String.Format("{0} like '{1}{2}*'", (conf.BoolVirtualPathBrowsing) ? "VirtualPathTitle" : conf.StrTitle1, StringExtensions.EscapeLikeValue(conf.StrTitleSelect), conf.TitleDelim);
      else
        s = s + conf.StrTitle1 + " not like ''";
      conf.StrFilmSelect = s;
      LogMyFilms.Debug("(SetFilmSelect) - StrFilmSelect: '" + s + "'");
    }

    bool GetFilmList() { return GetFilmList(-1); }
    /// <summary>
    /// Display List of titles that match based on current StrSelect & StrTitleSelect settings
    /// Titles are treated as though they were folder paths, using the delimeter (set in config) as if it were a filepath slash
    /// Once a title is the only item in that virtual subfolder, further path seperators are ignored and treated as regular chars
    /// </summary>
    /// <param name="gSelItem">If a string then a folder match is looked for. If an int the item with this id is looked for. If found the item is then made the currently selected item</param>
    /// <returns>If returns false means is currently displaying a single folder</returns>
    bool GetFilmList<T>(T gSelItem)
    {
      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the start ....
      GUIControl.ClearControl(GetID, facadeFilms.GetID); // ClearFacade(); // facadeFilms.Clear();

      SetFilmSelect();

      // set online filter only, if scan is done already ...
      conf.StrGlobalFilterString = InitialIsOnlineScan ? GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D : GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;

      LogMyFilms.Debug("(GetFilmList) - conf.GlobalFilterString:        '" + conf.StrGlobalFilterString + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrViewSelect:             '" + conf.StrViewSelect + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrDfltSelect:             '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrFilmSelect:             '" + conf.StrFilmSelect + "'");

      #region set status of Collection for global use ...
      conf.BoolCollection = conf.StrTitleSelect != "" && NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1 > 0; // here we set, if it is a collection or normal filmlist ...
      bool isvirtualpathview = conf.BoolVirtualPathBrowsing;
      string sortascending;
      string sortfield;
      if (isvirtualpathview) // special mode "virtual path title browsing"
      {
        sortfield = "VirtualPathTitle";
        sortascending = " ASC";
        conf.BoolCollection = false;
        LogMyFilms.Debug("(GetFilmList) - special case: set sort field to 'VirtualPathTitle' for virtual path browsing ! -> sortfield = '" + sortfield + "'");
      }
      else
      {
        if (conf.BoolCollection && Helper.FieldIsSet(conf.StrSortaInHierarchies)) // only use Collection sort, if there is a value - otherwise use default
        {
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortaInHierarchies:    '" + conf.StrSortaInHierarchies + "'");
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortSensInHierarchies: '" + conf.StrSortSensInHierarchies + "'");
          sortascending = conf.StrSortSensInHierarchies;
          sortfield = conf.StrSortaInHierarchies;
        }
        else
        {
          LogMyFilms.Debug("(GetFilmList) - conf.StrSorta:                  '" + conf.StrSorta + "'");
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortSens:               '" + conf.StrSortSens + "'");
          sortascending = conf.StrSortSens;
          sortfield = conf.StrSorta;

          // special handling for Box Sets ! -> use mastertitle for sorting to get proper grouping of the collections/groups/box-sets
          if (conf.StrViewSelect.Contains(@"*\*"))
          {
            sortfield = conf.StrTitle1;
            LogMyFilms.Debug("(GetFilmList) - special case: set sort field to master title for box set ! -> sortfield = '" + sortfield + "'");
          }
        }
      }

      if (string.IsNullOrEmpty(sortfield))
      {
        sortfield = conf.StrTitle1;
        LogMyFilms.Error("(GetFilmList) - sort field not properly set - resetting to mastertitle to avoid sort exception !");
      }
      #endregion

      #region Set sort buttons (movie vs. collection) and set Layout

      BtnSrtBy.IsEnabled = !isvirtualpathview;
      BtnSrtBy.Label = conf.BoolCollection ? GUILocalizeStrings.Get(96) + (conf.StrSortaInHierarchies == conf.StrSTitle ? GUILocalizeStrings.Get(103) : BaseMesFilms.TranslateColumn(conf.StrSortaInHierarchies)) : GUILocalizeStrings.Get(96) + (conf.StrSorta == conf.StrSTitle ? GUILocalizeStrings.Get(103) : BaseMesFilms.TranslateColumn(conf.StrSorta));
      BtnSrtBy.IsAscending = conf.BoolCollection ? conf.StrSortSensInHierarchies == " ASC" : conf.StrSortSens == " ASC";
      // BtnSrtBy.Disabled = conf.BoolCollection;

      Change_Layout_Action(conf.BoolCollection ? conf.StrLayOutInHierarchies : conf.StrLayOut);
      #endregion

      conf.DbSelection = new[] { conf.StrGlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false.ToString(), true.ToString() };
      r = BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false, true); // moved sorting to BaseFilms.ReadDataMovies - last boolean parameter added

      #region Additional sorting ... // MOVED TO BASE METHOD (READDATAMOVIES)
      //FieldType fieldType = GetFieldType(sortfield);
      //Type columnType = GetColumnType(sortfield);
      //string strColumnType = (columnType == null) ? "<invalid>" : columnType.ToString();

      //if (!string.IsNullOrEmpty(sortfield) && columnType == typeof(string)) // don't apply special sorting on "native" types - only on string types !
      //{
      //  LogMyFilms.Debug("GetFilmList() - sorting fieldtype = '" + fieldType + "', vartype = '" + strColumnType + "', sortfield = '" + sortfield + "', sortascending = '" + sortascending + "'");
      //  watch.Reset(); watch.Start();
      //  switch (fieldType)
      //  {
      //    case FieldType.Decimal:
      //      if (sortascending == " ASC")
      //      {
      //        IComparer myComparer = new myRatingComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //      }
      //      else
      //      {
      //        IComparer myComparer = new myRatingComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(b[sortfield], a[sortfield]));
      //        //r.Reverse();
      //      }
      //      break;
      //    case FieldType.AlphaNumeric:
      //      if (sortascending == " ASC")
      //      {
      //        IComparer myComparer = new AlphanumComparatorFast();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //      }
      //      else
      //      {
      //        IComparer myComparer = new myReverserAlphanumComparatorFast();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //        //r.Reverse();
      //      }
      //      break;
      //    case FieldType.Date:
      //      if (sortascending == " ASC")
      //      {
      //        IComparer myComparer = new myDateComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //      }
      //      else
      //      {
      //        IComparer myComparer = new myDateReverseComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //        //IComparer myComparer = new myDateComparer();
      //        //Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(b[sortfield], a[sortfield]));
      //      }
      //      break;
      //  }
      //  watch.Stop();
      //  LogMyFilms.Debug("GetFilmList() - additional sorting finished (" + (watch.ElapsedMilliseconds) + " ms)");
      //}
      #endregion

      int iCnt = 0;
      int delimCnt = 0;
      var item = new GUIListItem();
      string selItem = gSelItem.ToString();
      int iSelItem = typeof(T) == typeof(int) ? Int32.Parse(selItem) : -2;
      LogMyFilms.Debug("(GetFilmList) - selItem/iSelItem:               '" + selItem + "'/'" + iSelItem + "'");

      List<GUIListItem> facadeDownloadItems = new List<GUIListItem>();

      // setlabels
      // MyFilmsDetail.setGUIProperty("select", (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));// always show as though folder path using \ regardless what sep is used
      SetLabelSelect(conf.StrTxtSelect == "" ? "" : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));

      if (conf.StrTitleSelect != "") delimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title

      #region Load the facade
      int number = -1;
      string sPrevTitle = "";
      int wfacadewiew = 0;
      bool isdate = IsDateField(conf.WStrSort);
      bool isPinIconsAvailable = LoadWatchedFlagPossible(); // do it only once, as it requires 4 IO ops

      foreach (DataRow sr in r)
      {
        number++;
        #region filter list by Wselectedlabel, if user is coming from group view
        if (conf.Boolreturn) //in case of selection by view verify if value correspond excatly to the searched string
        {
          List<string> wTableau = Search_String(sr[conf.WStrSort].ToString());
          foreach (string s in wTableau)
          {
            if (isdate)
            {
              if (string.Format("{0:dd/MM/yyyy}", DateTime.Parse(s).ToShortDateString()) == string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
              {
                goto suite;
              }
            }
            else
            {
              if (s.IndexOf(conf.Wselectedlabel, StringComparison.OrdinalIgnoreCase) >= 0) // if (s.ToLower().Contains(conf.Wselectedlabel.Trim().ToLower())) // if (string.Compare(s, conf.Wselectedlabel.Trim(), true) >= 0) // string.Compare(champselect, wchampselect, true) == 0
              {
                goto suite;
              }
            }
          }
          goto fin;
        }
        #endregion

      suite:
        #region suite - process facade item ...
        string currentImage = "";
        string sTitle;
        string masterTitle = sr[conf.StrTitle1].ToString();
        string sFullTitle = sTitle = !isvirtualpathview ? masterTitle : sr["VirtualPathTitle"] + (masterTitle.Length > 0 ? @"\" : "") + (masterTitle.LastIndexOf(conf.TitleDelim, StringComparison.Ordinal) > 0 ? masterTitle.Substring(masterTitle.LastIndexOf(conf.TitleDelim, StringComparison.Ordinal) + 1) : masterTitle);

        int delimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
        if (delimCnt <= delimCnt2)
        {
          sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, delimCnt, delimCnt + 1, false, false); //get current substring (folder) within path
          sFullTitle = NewString.NPosRight(conf.TitleDelim, sFullTitle, delimCnt, false, false); //current rest of path (if only 1 entry in subfolders will present entry ignoring folders)
        }

        if (iCnt > 0 && delimCnt < delimCnt2 && sTitle == sPrevTitle) // don't stack items already at lowest folder level
        {
          iCnt++;
          if (!isvirtualpathview)
          {
            item.Label2 = "(" + iCnt + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count)
            if (iCnt == 2) // iCnt == 1 is never reached here - changes to be done in main item load below !
            {
              item.Label = sTitle; //reset to current single folder as > 1 entries
              item.IsFolder = true;
              item.ThumbnailImage = currentImage;
              item.IconImage = currentImage;
            }
          }
          else // virtual path titles
          {
            item.Label2 = "(" + iCnt + ")";
            if (iCnt == 2) // iCnt == 1 is never reached here - changes to be done in main item load below !
            {
              item.Label = sTitle; //reset to current single folder as > 1 entries
              item.IsFolder = true;
              item.ThumbnailImage = "";
              item.IconImage = "";
              item.PinImage = "";
              item.IsRemote = false;
              item.TVTag = ""; // setting to not null skips imagesearch
              MediaPortal.Util.Utils.SetDefaultIcons(item);
            }
          }
        }
        else
        {
          iCnt = 1;
          item = new GUIListItem {ItemId = number};
          if (!isvirtualpathview)
            item.Label = delimCnt < delimCnt2 ? sFullTitle.Substring(sFullTitle.LastIndexOf(conf.TitleDelim, StringComparison.Ordinal) + 1) : sFullTitle; // Set = full subfolders path initially - new: set only to last name
          else
            item.Label = delimCnt < delimCnt2 ? sTitle : sr[conf.StrSTitle].ToString(); //reset to current single folder as > 1 entries

          item.DVDLabel = sTitle; // used by background thread
          item.IsFolder = isvirtualpathview && delimCnt + 1 < delimCnt2; // show folders in virtual path browsing
          int year;
          if (int.TryParse(sr["Year"].ToString(), out year)) item.Year = year;

          #region Label2 ...
          if (!conf.OnlyTitleList)
          {
            switch (sortfield)
            {
              case "VirtualPathTitle":
                item.Label2 = delimCnt + 1 < delimCnt2 ? "(" + iCnt + ")" : "";
                break;
              case "TranslatedTitle":
              case "OriginalTitle":
              case "FormattedTitle":
                item.Label2 = sr["Year"].ToString();
                break;
              case "Year":
                item.Label2 = sr["Year"].ToString();
                break;
              case "Date":
              case "DateAdded":
                try
                {
                  item.Label2 = DateTime.Parse(sr["Date"].ToString()).ToShortDateString(); //item.Label2 = ((DateTime)sr["DateAdded"]).ToShortDateString();
                }
                catch { }
                break;
              case "DateWatched":
                try
                {
                  item.Label2 = DateTime.Parse(sr["DateWatched"].ToString()).ToShortDateString(); //item.Label2 = ((DateTime)sr["DateAdded"]).ToShortDateString();
                }
                catch { }
                break;
              case "Rating":
                try
                {
                  item.Label2 = decimal.Parse(sr["Rating"].ToString()).ToString("0.0");
                }
                catch (Exception)
                {
                  item.Label2 = sr["Rating"].ToString();
                }
                break;
              default:
                string label2 = sr[sortfield].ToString(); // string label2 = sr[conf.wStrSort].ToString();
                if (IsSplittableField(sortfield))
                {
                  List<string> wtab = Search_String(label2, false);
                  if (wtab.Count > 0) label2 = wtab[0];
                  if (wtab.Count > 1) label2 += " ...";
                }
                if (label2.Length > 34) label2 = label2.Substring(0, 30) + " ...";
                item.Label2 = label2;
                break;
            }
            if (!isvirtualpathview && delimCnt < delimCnt2) item.Label2 = "(" + iCnt + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count) //  && iCnt > 1 ??
          }
          #endregion
          if (!(isvirtualpathview && item.IsFolder))
          {
            #region Label3 ...

            item.Label3 = sr["Edition"].ToString() ?? "";

            #endregion

            #region Watched Status
            if (conf.EnhancedWatchedStatusHandling)
            {
              //if (!sr[BaseMesFilms.MultiUserStateField].ToString().Contains(":")) // not yet migrated/created!
              //{
              //  #region migrate status from configured (enhanced)watched field to new MultiUserStates
              //  MultiUserData userData;
              //  if (sr[MyFilms.conf.StrWatchedField].ToString().Contains(":"))
              //  {
              //    // old field was already multiuserdata - migrate it!
              //    userData = new MultiUserData(sr[conf.StrWatchedField].ToString());
              //    sr[MyFilms.conf.StrWatchedField] = userData.GetUserState(MyFilms.conf.StrUserProfileName).Watched ? "true" : MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
              //  }
              //  else
              //  {
              //    // old field was standard watched data - create MUS and add watched for current user
              //    bool tmpwatched = (!string.IsNullOrEmpty(conf.GlobalUnwatchedOnlyValue) &&
              //                  sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower() &&
              //                  sr[conf.StrWatchedField].ToString().Length > 0);
              //    userData = new MultiUserData("");
              //    userData.SetWatched(MyFilms.conf.StrUserProfileName, tmpwatched);
              //    if (sr["RatingUser"] != Convert.DBNull)
              //      userData.SetRating(MyFilms.conf.StrUserProfileName, (decimal)sr["RatingUser"]);
              //  }
              //  sr[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
              //  sr["DateWatched"] = userData.GetUserState(MyFilms.conf.StrUserProfileName).WatchedDate;
              //  sr["RatingUser"] = userData.GetUserState(MyFilms.conf.StrUserProfileName).UserRating == MultiUserData.NoRating ? Convert.DBNull : userData.GetUserState(MyFilms.conf.StrUserProfileName).UserRating;
              //  #endregion
              //}
              item.IsPlayed = EnhancedWatched(sr[BaseMesFilms.MultiUserStateField].ToString(), conf.StrUserProfileName);
            }
            else
            {
              //if (sr[conf.StrWatchedField].ToString().StartsWith("Global:"))
              //{
              //  string s = sr[conf.StrWatchedField].ToString();
              //  string count = s.Substring(s.IndexOf(":", System.StringComparison.Ordinal) + 1, 1);
              //  sr[conf.StrWatchedField] = (count == "0") ? conf.GlobalUnwatchedOnlyValue : "true";
              //}
              if (conf.GlobalUnwatchedOnlyValue != null && conf.StrWatchedField.Length > 0)
                if (sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                  item.IsPlayed = true;
            }
            if (conf.StrSuppressAutomatic && conf.StrSuppressField.Length > 0)
              if ((sr[conf.StrSuppressField].ToString() == conf.StrSuppressValue) && conf.StrSuppressPlayStopUpdateUserField) item.IsPlayed = true;
            #endregion

            #region Availability Status

            // set availability status // only display media status, if onlinescan was done // if its online, set IsRemote to false !
            if (InitialIsOnlineScan)
              item.IsRemote = !string.IsNullOrEmpty(sr["IsOnline"].ToString()) && !bool.Parse(sr["IsOnline"].ToString());
            // load special icons to indicate watched/available flags in listcontrol
            if (isPinIconsAvailable) LoadWatchedFlag(item, item.IsPlayed, !item.IsRemote);

            #endregion

            #region Cover Picture

            if (sr["Picture"].ToString().Length > 0)
            {
              if ((sr["Picture"].ToString().IndexOf(":\\", StringComparison.Ordinal) == -1) &&
                  (sr["Picture"].ToString().Substring(0, 2) != "\\\\")) currentImage = conf.StrPathImg + "\\" + sr["Picture"];
              else currentImage = sr["Picture"].ToString();
            }
            else
            {
              currentImage = string.Empty;
            }
            item.ThumbnailImage = currentImage;
            item.IconImageBig = currentImage;
            item.IconImage = currentImage;

            #endregion

            facadeDownloadItems.Add(item);
          }
          else
          {
            item.ThumbnailImage = "";
            item.IconImage = "";
            item.PinImage = "";
            item.IsRemote = false;
            item.TVTag = ""; // setting to not null skips imagesearch
            MediaPortal.Util.Utils.SetDefaultIcons(item);
          }

          item.OnItemSelected += item_OnItemSelected;
          facadeFilms.Add(item);

          if (iSelItem == -2) // selected item is passed as string in "selItem"
          {
            if (sTitle == selItem) wfacadewiew = facadeFilms.Count - 1; //test if this item is one to select
          }
        }

        if (iSelItem >= 0) //-1 = ignore, >=0 = itemID to locate (test every item in case item is from within a folder)
        {
          if (!(conf.StrTIndex.Length > 0))
          {
            if (number == iSelItem) wfacadewiew = facadeFilms.Count - 1; //test if this item is one to select
          }
          else
          {
            if (number == iSelItem && sFullTitle == conf.StrTIndex) wfacadewiew = facadeFilms.Count - 1; //test if this item is one to select
          }
        }
        sPrevTitle = sTitle;
        #endregion
      fin: ;
      }
      #endregion

      #region post facade settings ...
      if (facadeFilms.Count == 0)
      {
        item = new GUIListItem { Label = GUILocalizeStrings.Get(10798639), IsRemote = true };
        facadeFilms.Add(item);
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798639));
        conf.ViewContext = ViewContext.Menu;
        GUIControl.ShowControl(GetID, 34); // hides certain GUI elements
      }
      else
      {
        Fanartstatus(true);
        GUIControl.HideControl(GetID, 34);
      }
      item.FreeMemory();
      MyFilmsDetail.setGUIProperty("nbobjects.value", r.Length.ToString(CultureInfo.InvariantCulture));

      GUIPropertyManager.SetProperty("#itemcount", facadeFilms.Count.ToString(CultureInfo.InvariantCulture));
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select")); 
      if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
      //if (wfacadewiew == -1 && facadeFilms.Count > 0)
      //  GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, 0);
      //else
      //  GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)wfacadewiew);
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, wfacadewiew);

      if (facadeFilms.Count == 1 && item.IsFolder)
      {
        conf.Boolreturn = false;
        conf.Wselectedlabel = item.Label;
      }
      conf.ViewContext = conf.BoolCollection ? ViewContext.MovieCollection : ViewContext.Movie;
      LogMyFilms.Debug("GetFilmList finished!");

      GetOnlineMoviesFromTMDBforFilmList(); // add Online Movies from TMDB
      GetImagesFilmList(facadeDownloadItems);
      #endregion
      return !(facadeFilms.Count == 1 && item.IsFolder); //ret false if single folder found
    }

    private static bool EnhancedWatched(string strEnhancedWatchedValue, string strUserProfileName)
    {
      if (!strEnhancedWatchedValue.Contains(strUserProfileName + ":")) return false;
      if (strEnhancedWatchedValue.Contains(strUserProfileName + ":0")) return false;
      if (strEnhancedWatchedValue.Contains(strUserProfileName + ":-1")) return false;
      return true; // count > 0 -> return true
    }

    private void GetOnlineMoviesFromTMDBforFilmList()
    {
      return;
      #region add online movies from TMDB
      if (!MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "GetFilmlist() - Online info on person film list") || facadeFilms.Count >= 200 || !IsPersonsField(conf.WStrSort)) 
        return;

      var facadeDownloadItems = new List<GUIListItem>();
      bool isPinIconsAvailable = LoadWatchedFlagPossible(); // do it only once, as it requires 4 IO ops
      string currentImage = "";
      //sortascending = conf.StrSortSens;
      //sortfield = conf.StrSorta;
      string sortascending;
      string sortfield;
      if (conf.BoolCollection && Helper.FieldIsSet(conf.StrSortaInHierarchies)) // only use Collection sort, if there is a value - otherwise use default
      {
        LogMyFilms.Debug("(GetFilmList) - conf.StrSortaInHierarchies:    '" + conf.StrSortaInHierarchies + "'");
        LogMyFilms.Debug("(GetFilmList) - conf.StrSortSensInHierarchies: '" + conf.StrSortSensInHierarchies + "'");
        sortascending = conf.StrSortSensInHierarchies;
        sortfield = conf.StrSortaInHierarchies;
      }
      else
      {
        LogMyFilms.Debug("(GetFilmList) - conf.StrSorta:                  '" + conf.StrSorta + "'");
        LogMyFilms.Debug("(GetFilmList) - conf.StrSortSens:               '" + conf.StrSortSens + "'");
        sortascending = conf.StrSortSens;
        sortfield = conf.StrSorta;
        // special handling for Box Sets ! -> use mastertitle for sorting to get proper grouping of the collections/groups/box-sets
        if (conf.StrViewSelect.Contains(@"*\*"))
        {
          sortfield = conf.StrTitle1;
          LogMyFilms.Debug("(GetFilmList) - special case: set sort field to master title for box set ! -> sortfield = '" + sortfield + "'");
        }
      }
      if (string.IsNullOrEmpty(sortfield))
      {
        sortfield = MyFilms.conf.StrTitle1;
        LogMyFilms.Error("(GetFilmList) - sort field not properly set - resetting to mastertitle to avoid sort exception !");
      }

      IComparer myComparer = new AlphanumComparatorFast();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      TheMoviedb tmdbapi = new TheMoviedb();
      TMDB.Tmdb api = new TMDB.Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      TMDB.TmdbConfiguration tmdbConf = api.GetConfiguration();
      TmdbPersonSearch personSearch = api.SearchPerson(conf.Wselectedlabel, 1);
      GUIListItem item = null;
      LogMyFilms.Debug("GetFilmList() - OnlineInfo - '" + personSearch.results.Count + "' results found for '" + (conf.Wselectedlabel ?? "") + "' ('" + conf.WStrSort + "')!");
      if (personSearch.results.Count > 0)
      {
        TmdbPersonCredits personCredits = api.GetPersonCredits(personSearch.results[0].id);
        LogMyFilms.Debug("GetFilmList() - OnlineInfo - '" + personCredits.cast.Count + "' cast movies found for '" + (conf.Wselectedlabel ?? "") + "' ('" + conf.WStrSort + "')!");
        LogMyFilms.Debug("GetFilmList() - OnlineInfo - '" + personCredits.crew.Count + "' crew movies found for '" + (conf.Wselectedlabel ?? "") + "' ('" + conf.WStrSort + "')!");
        foreach (CastCredit personMovie in personCredits.cast)
        {
          bool toBeAddedToList = true;
          int iInsertAt = int.MaxValue;
          for (int i = 0; i < facadeFilms.Count; i++)
          {
            if (facadeFilms[i].Label == personMovie.title)
              toBeAddedToList = false;
            else
            {
              try
              {
                if (sortfield == "Year" && personMovie.release_date.Length > 3)
                {
                  if (sortascending == " ASC")
                  {
                    //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) > Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                    if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) < 0) iInsertAt = i;
                  }
                  else
                  {
                    //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) < Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                    if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) > 0) iInsertAt = i;
                  }
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("GetFilmList() - Error in year parsing: " + ex.Message + " - " + ex.StackTrace);
              }
            }
          }
          LogMyFilms.Debug("GetFilmList() - OnlineInfo - add = '" + toBeAddedToList + "' for '" + personMovie.title + "' (" + personMovie.release_date + ") to facade at position '" + ((iInsertAt != int.MaxValue) ? iInsertAt.ToString() : "end") + "'");
          if (!toBeAddedToList) continue;

          #region add item

          item = new GUIListItem
          {
            Label = personMovie.title,
            IsFolder = true,
            ThumbnailImage = conf.DefaultCover,
            IconImage = conf.DefaultCover
          };
          //reset to current single folder as > 1 entries

          #region Label2 ...
          if (!MyFilms.conf.OnlyTitleList)
          {
            switch (sortfield)
            {
              case "TranslatedTitle":
              case "OriginalTitle":
              case "FormattedTitle":
              case "Year":
                item.Label2 = (personMovie != null && personMovie.release_date.Length > 3) ? personMovie.release_date.Substring(0, 4) : (personMovie.release_date ?? "");
                break;
              case "Date":
              case "DateAdded":
                break;
              case "Rating":
                break;
            }
          }
          #endregion
          #region Label3 ...
          item.Label3 = personMovie.character + " (cast)";
          #endregion
          #region Watched Status
          item.IsPlayed = false;
          #endregion

          #region Availability Status
          item.IsRemote = true;
          // load special icons to indicate watched/available flags in listcontrol
          if (isPinIconsAvailable) LoadWatchedFlag(item, item.IsPlayed, !item.IsRemote);
          #endregion

          #region Cover Picture
          currentImage = string.Empty;
          currentImage = tmdbConf.images.base_url + "w500" + personMovie.poster_path;
          item.ThumbnailImage = currentImage;
          item.IconImageBig = currentImage;
          item.IconImage = currentImage;
          item.TVTag = personMovie;
          #endregion

          facadeDownloadItems.Add(item);
          item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
          if (iInsertAt == int.MaxValue)
            facadeFilms.Add(item);
          else
            facadeFilms.Insert(0, item);
          #endregion
        }
        foreach (CrewCredit personMovie in personCredits.crew)
        {
          bool toBeAddedToList = true;
          int iInsertAt = int.MaxValue;
          for (int i = 0; i < facadeFilms.Count; i++)
          {
            if (facadeFilms[i].Label == personMovie.title)
              toBeAddedToList = false;
            else
            {
              try
              {
                if (sortfield == "Year" && personMovie.release_date.Length > 3)
                {
                  if (sortascending == " ASC")
                  {
                    //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) > Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                    if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) < 0) iInsertAt = i;
                  }
                  else
                  {
                    //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) < Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                    if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) > 0) iInsertAt = i;
                  }
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("GetFilmList() - Error in year parsing: " + ex.Message + " - " + ex.StackTrace);
              }
            }
          }
          LogMyFilms.Debug("GetFilmList() - OnlineInfo - add = '" + toBeAddedToList + "' for '" + personMovie.title + "' (" + personMovie.release_date + ") to facade at position '" + ((iInsertAt != int.MaxValue) ? iInsertAt.ToString() : "end") + "'");
          if (!toBeAddedToList) continue;

          #region add item

          item = new GUIListItem
          {
            Label = personMovie.title,
            IsFolder = true,
            ThumbnailImage = conf.DefaultCover,
            IconImage = conf.DefaultCover
          };
          //reset to current single folder as > 1 entries

          #region Label2 ...
          if (!MyFilms.conf.OnlyTitleList)
          {
            switch (sortfield)
            {
              case "TranslatedTitle":
              case "OriginalTitle":
              case "FormattedTitle":
              case "Year":
                item.Label2 = (personMovie != null && personMovie.release_date.Length > 3) ? personMovie.release_date.Substring(0, 4) : (personMovie.release_date ?? "");
                break;
              case "Date":
              case "DateAdded":
                break;
              case "Rating":
                break;
            }
          }
          #endregion
          #region Label3 ...
          item.Label3 = personMovie.job + " (" + personMovie.department + ")";
          #endregion
          #region Watched Status
          item.IsPlayed = false;
          #endregion

          #region Availability Status
          item.IsRemote = true;
          // load special icons to indicate watched/available flags in listcontrol
          if (isPinIconsAvailable) LoadWatchedFlag(item, item.IsPlayed, !item.IsRemote);
          #endregion

          #region Cover Picture
          currentImage = string.Empty;
          currentImage = tmdbConf.images.base_url + "w500" + personMovie.poster_path;
          item.ThumbnailImage = currentImage;
          item.IconImageBig = currentImage;
          item.IconImage = currentImage;
          item.TVTag = personMovie;
          #endregion

          facadeDownloadItems.Add(item);
          item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
          facadeFilms.Insert(0, item);
          #endregion
        }
      }
      GetImagesFilmList(facadeDownloadItems);

      #endregion
    }

    private void GetImagesFilmList(List<GUIListItem> itemsWithThumbs)
    {
      StopLoadingFilmlistDetails = false;
      string coverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";

      // split the downloads in X+ groups and do multithreaded downloading
      int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // Guzzi: Set group to x to only allow x thread(s)
      int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);


      for (int i = 0; i < groups; i++)
      {
        var groupList = new List<GUIListItem>();
        for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
        {
          groupList.Add(itemsWithThumbs[j]);
        }

        new Thread(delegate(object o)
        {
          string language = null;
          Tmdb api = null;
          if (conf.BoolEnableOnlineServices)
          {
            language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
            LogMyFilms.Debug("GetImagesforTmdbFilmList - detected language = '" + language + "'");
            // grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb();
            api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
            TmdbConfiguration tmdbConf = api.GetConfiguration();
            foreach (string posterSize in tmdbConf.images.poster_sizes)
            {
              LogMyFilms.Debug("Available TMDB Poster Size: '" + posterSize + "'");
            }
          }
          
          var items = (List<GUIListItem>)o;
          foreach (GUIListItem item in items)
          {
            // stop download if we have exited window
            if (StopLoadingFilmlistDetails)
            {
              items.SafeDispose();
              return;
            }
            if (item.TVTag != null) continue; // skip for online videos and virtual path title browsing

            #region group-collection image handling
            //if (!File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
            if (item.IsFolder) // special handling for groups (movie collections - NOT views!)
            {
              string strThumbGroup = GetGroupImage(item); // thumbnail for Groups/collections
              item.IconImage = strThumbGroup;
              item.IconImageBig = strThumbGroup;
              item.ThumbnailImage = strThumbGroup;
            }
            #endregion

            #region create artifical covers with title - inactive
            if (item.ThumbnailImage == "" || !File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
            {
              //string strlabel = item.Label;
              //MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strlabel);
              //strThumb = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views\" + strlabel;
              //if (System.IO.File.Exists(strThumb + ".png"))
              //{
              //  conf.FileImage = strThumb + ".png"; 
              //}
              //else
              {
                //conf.FileImage = string.Empty;
                //try
                //{
                //    if (conf.StrPathViews.Length > 0)
                //        if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                //            //Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb + ".png", cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
                //            createCacheThumb(conf.StrPathViews + item.Label + ".png", strThumb + ".png");
                //        else
                //            //Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
                //            createCacheThumb(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png");
                //    // Disabled "pseudo covers with label name"
                //    //if (!System.IO.File.Exists(strThumb + ".png"))
                //    //    if (MyFilms.conf.StrViewsDflt && System.IO.File.Exists(MyFilms.conf.DefaultCoverViews))
                //    //        ImageFast.CreateImage(strThumb + ".png", item.Label);
                //    if (System.IO.File.Exists(strThumb + ".png"))                                
                //    conf.FileImage = strThumb + ".png"; 
                //}
                //catch
                //{
                //    conf.FileImage = string.Empty;
                //}
                //if (string.IsNullOrEmpty(conf.FileImage) && conf.DefaultCover.Length > 0)
                //if (conf.DefaultCover.Length > 0)
                //  conf.FileImage = conf.DefaultCover;
              }
            }
            #endregion

            item.MusicTag = item.ThumbnailImage; // keep Original one in music tag for big list thumb ...
            // strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, item.DVDLabel); // item.DVDLabel is sTitle
            string strThumb = MediaPortal.Util.Utils.GetCoverArtName(coverThumbDir, item.DVDLabel); // cached cover
            if (string.IsNullOrEmpty(strThumb) || !strThumb.Contains(".")) continue;
            string strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".")) + "_s" + Path.GetExtension(strThumb);  // cached cover for Icons - small resolution

            if (!string.IsNullOrEmpty(item.ThumbnailImage) && item.ThumbnailImage != conf.DefaultCover && !File.Exists(strThumb))
            {
              Picture.CreateThumbnail(item.ThumbnailImage, strThumbSmall, 100, 150, 0, Thumbs.SpeedThumbsSmall);
              Picture.CreateThumbnail(item.ThumbnailImage, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
              LogMyFilms.Debug("GetFimList: Background thread creating thumbimage for sTitle: '" + item.DVDLabel + "'");
            }
            if (File.Exists(strThumb))
            {
              item.IconImage = strThumbSmall;
              item.IconImageBig = strThumb;
              item.ThumbnailImage = strThumb;
            }
            else
            {
              if (conf.DefaultCover.Length > 0)
              {
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
              }
            }
            // Thread.Sleep(10);
          }

          if (conf.BoolEnableOnlineServices)
          {
            foreach (GUIListItem item in items)
            {
              #region update facade for tmdb trailer availability (requires two requests per movie (search and trailer) - slow!)
              try
              {
                if (StopLoadingFilmlistDetails) return; // stop download if we have exited window

                TmdbMovieSearch moviesfound = api.SearchMovie(item.Label, 1, "", null, item.Year);
                if (moviesfound.results.Count == 0) moviesfound = api.SearchMovie(item.Label, 1, null);
                if (moviesfound.results.Count != 1)
                {
                  LogMyFilms.Debug("GetImagesForFilmList() - Movie Search Results: '" + moviesfound.total_results.ToString() + "' for movie '" + item.Label + "'");
                }
                else
                {
                  TmdbMovieTrailers trailers = api.GetMovieTrailers(moviesfound.results[0].id, language);
                  if (trailers.youtube.Count == 0)
                  {
                    // LogMyFilms.Debug("GetTrailers - no trailer found for language '" + language + "' - try default");
                    trailers = api.GetMovieTrailers(moviesfound.results[0].id, null);
                  }
                  if (trailers.youtube.Count > 0)
                  {
                    item.IsDownloading = true;
                    item.IsRemote = true; // for testing only to get it marked in facade
                  }
                  else 
                    LogMyFilms.Debug("GetImagesForFilmList() - no trailers found");
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("GetImagesForFilmList() - Error: '" + ex.Message + "'");
                LogMyFilms.Debug("GetImagesForFilmList() - Exception: '" + ex.StackTrace + "'");
              }
              #endregion
            }
          }

        })
        {
          IsBackground = true,
          Priority = ThreadPriority.BelowNormal,
          Name = "MyFilms FilmList Image Detector " + i
        }.Start(groupList);
      }
    }

    private string GetGroupImage(GUIListItem item)
    {
      // LogMyFilms.Debug("GetGroupImage() - item.Label = '" + (item.Label ?? "") + "', conf.StrTitleSelect = '" + conf.StrTitleSelect + "'");

      string collectionSafeName = GrabUtil.CreateFilename(item.Label);

      string strThumbGroup = ""; // thumbnail for Groups/collections
      if (File.Exists(conf.StrPathImg + "\\" + conf.StrTitleSelect.Replace(conf.TitleDelim, ".") + "." + collectionSafeName + ".jpg")) // check for longnames for nested groups
      {
        strThumbGroup = conf.StrPathImg + "\\" + conf.StrTitleSelect.Replace(conf.TitleDelim, ".") + "." + collectionSafeName + ".jpg";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + conf.StrTitleSelect + "." + collectionSafeName + ".jpg")) // check for nested directories
      {
        strThumbGroup = conf.StrPathImg + "\\" + conf.StrTitleSelect + "." + collectionSafeName + ".jpg";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + collectionSafeName + ".jpg"))
      {
        strThumbGroup = conf.StrPathImg + "\\" + collectionSafeName + ".jpg";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + collectionSafeName + ".png"))
      {
        strThumbGroup = conf.StrPathImg + "\\" + collectionSafeName + ".png";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + conf.StrPicturePrefix + collectionSafeName + ".jpg")) // if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
      {
        strThumbGroup = conf.StrPathImg + "\\" + conf.StrPicturePrefix + collectionSafeName + ".jpg";
      }
      else if (conf.StrPicturePrefix.Contains("\\"))
      {
        if (File.Exists(conf.StrPathImg + "\\" + conf.StrPicturePrefix.Substring(0, conf.StrPicturePrefix.LastIndexOf("\\")) + "\\" + collectionSafeName + ".jpg"))
          strThumbGroup = conf.StrPathImg + "\\" + conf.StrPicturePrefix.Substring(0, conf.StrPicturePrefix.LastIndexOf("\\")) + "\\" + collectionSafeName + ".jpg";
        // if (strThumbGroup == "") strThumbGroup = MyFilms.conf.DefaultCover; // ToDo: Add default cover for hierarchies / box sets
      }
      LogMyFilms.Debug("GetGroupImage() - item.Label = '" + (item.Label ?? "") + "', conf.StrTitleSelect = '" + conf.StrTitleSelect + "', collectionSafeName = '" + collectionSafeName + "' - returning image = '" + strThumbGroup + "'");
      return strThumbGroup;
    }

    bool GetMoviesInGroup(ref ArrayList fanartItems)
    {
      LogMyFilms.Debug("GetRandomFanartForGroups started ...");
      fanartItems.Clear();
      Type columnType = GetColumnType(conf.WStrSort);
      string s = "";
      string strSelect = string.Empty;
      if (conf.Boolselect)
      {
        // string sLabel = facadeFilms.SelectedListItem.Label; // string sLabel = this.facadeFilms.SelectedListItem.Label;
        string sLabel = conf.BoolReverseNames && facadeFilms.SelectedListItem.Label != EmptyFacadeValue ? ReReverseName(facadeFilms.SelectedListItem.Label) : facadeFilms.SelectedListItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value

        bool labelNotEmpty = sLabel.Length > 0;

        if (columnType != typeof(string))
          strSelect = labelNotEmpty ? conf.WStrSort + " = '" + sLabel + "'" : conf.WStrSort + " is NULL";
        else if (conf.WStrSort == "Date")
          strSelect = labelNotEmpty ? "DateAdded" + " = '" + DateTime.Parse(sLabel).ToShortDateString() + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (IsDateField(conf.WStrSort))
          strSelect = labelNotEmpty ? conf.WStrSort + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (IsAlphaNumericalField(conf.WStrSort))
          strSelect = labelNotEmpty ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else
        {
          strSelect = conf.BoolDontSplitValuesInViews
            ? (labelNotEmpty
              ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'"
              : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')")
            : (labelNotEmpty
              ? conf.WStrSort + " like '*" + StringExtensions.EscapeLikeValue(sLabel) + "*'"
              : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')");
        }
      }

      if (strSelect != "") s = strSelect + " And ";
      if (conf.StrTitleSelect != "")
        s = s + String.Format("{0} like '{1}{2}*'", conf.StrTitle1, StringExtensions.EscapeLikeValue(conf.StrTitleSelect), conf.TitleDelim);
      else
        s = s + conf.StrTitle1 + " not like ''";
      string strFilmSelect = s;

      string globalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;
      DataRow[] rFanart = BaseMesFilms.ReadDataMovies(globalFilterString + conf.StrViewSelect + conf.StrDfltSelect, strFilmSelect, conf.StrSorta, conf.StrSortSens, false);
      LogMyFilms.Debug("(GetRandomFanartForGroups) - Count: '" + rFanart.Length + "'");
      int iCnt = 0;
      int delimCnt = 0, delimCnt2;
      var item = new GUIListItem();
      string sTitle;
      string sFullTitle;
      string sPrevTitle = "";

      if (conf.StrTitleSelect != "") delimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title
      int number = -1;
      bool isdate = IsDateField(conf.WStrSort);

      foreach (DataRow sr in rFanart)
      {
        number++;
        if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
        {
          List<string> wTableau = Search_String(sr[conf.WStrSort].ToString(), false);
          foreach (string val in wTableau)
          {
            if (isdate && string.Format("{0:dd/MM/yyyy}", DateTime.Parse(val).ToShortDateString()) == string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
              goto suite;
            if (val.IndexOf(conf.Wselectedlabel, StringComparison.OrdinalIgnoreCase) >= 0) // if (val.ToLower().Contains(conf.Wselectedlabel.Trim().ToLower()))
              goto suite;
          }
          goto fin;
        }
      suite:

        sFullTitle = sTitle = sr[conf.StrTitle1].ToString();
        delimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
        if (delimCnt <= delimCnt2)
        {
          sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, delimCnt, delimCnt + 1, false, false); //get current substring (folder) within path
          sFullTitle = NewString.NPosRight(conf.TitleDelim, sFullTitle, delimCnt, false, false); //current rest of path (if only 1 entry in subfolders will present entry ignoring folders)
        }
        if ((iCnt > 0) && (delimCnt < delimCnt2) && (sTitle == sPrevTitle)) // don't stack items already at lowest folder level
        {
          iCnt++;
          item.Label2 = "(" + iCnt + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count)
          if (iCnt == 2)
          {
            item.Label = sTitle; //reset to current single folder as > 1 entries
            item.IsFolder = true;
          }
        }
        else
        {
          iCnt = 1;
          item = new GUIListItem
          {
            Label = sFullTitle,
            ItemId = number
          };
          // Set = full subfolders path initially
          if (!item.IsFolder)
            fanartItems.Add(item);
        }
      fin: ;
      }
      return !(fanartItems.Count == 1 && item.IsFolder);
    }

    private bool GetRandomFanartForGroups(int limit)
    {
      ArrayList fanartItems = new ArrayList();
      int i = 0;
      try
      {
        GetMoviesInGroup(ref fanartItems);
      }
      catch (Exception ex)
      {
        //LogMyFilms.DebugException("GetRandomFanartForGroups(): exception on calling GetMoviesInGroup() - ", ex);
        LogMyFilms.Warn("GetRandomFanartForGroups(): exception on calling GetMoviesInGroup() - " + ex.Message);
        LogMyFilms.Debug(ex, "GetRandomFanartForGroups() - inner exception: ");
        fanartItems.SafeDispose();
        return false;
      }

      // sort randomize
      Random r = new Random();
      for (int cnt = 0; cnt < fanartItems.Count; cnt++)
      {
        object tmp = fanartItems[cnt];
        int idx = r.Next(fanartItems.Count - cnt) + cnt;
        fanartItems[cnt] = fanartItems[idx];
        fanartItems[idx] = tmp;
      }

      currentFanartList.Clear(); // clear list from former content

      // Random rnd = new Random(); // return source.OrderBy<T, int>((item) => rnd.Next());

      foreach (string[] wfanart in fanartItems.Cast<GUIListItem>().Select(randomFanartItem => MyFilmsDetail.Search_Fanart(randomFanartItem.Label, true, "file", false, string.Empty, string.Empty)).Where(wfanart => wfanart[0] != " " && wfanart[0] != conf.DefaultFanartImage && !currentFanartList.Contains(wfanart[0])))
      {
        currentFanartList.Add(wfanart[0]);
        // LogMyFilms.Debug("GetRandomFanartForGroups() - added fanart #" + currentFanartList.Count + " : '" + wfanart[0] + "'");
        i += 1;
        if (i >= limit && limit != 0)
        {
          return true;
        }
      }
      fanartItems.SafeDispose();
      return currentFanartList.Count > 0;
    }

    private bool GetRandomFanartForFilms(int limit)
    {
      int i = 0;
      string fanartDirectory = string.Empty;
      currentFanartList.Clear(); // clear list from former content
      string[] wfanart;
      int index = facadeFilms.SelectedListItem.ItemId;
      if (r == null || index > r.Length - 1)
      {
        LogMyFilms.Warn("GetRandomFanartForFilms() - Failed loading random fanart - index '" + index + "' not within current dataset ... ");
        return false;
      }
      string personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
      string fanartTitle = MyFilmsDetail.GetFanartTitle(r[index], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
      wfanart = MyFilmsDetail.Search_Fanart(fanartTitle, false, "dir", false, "false", string.Empty);

      if (wfanart[1] == "dir")
        fanartDirectory = wfanart[0];

      if (!string.IsNullOrEmpty(fanartDirectory) && Directory.Exists(fanartDirectory))
      {
        const string searchname = "*.jpg";
        string[] files = Directory.GetFiles(fanartDirectory, searchname, SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
          foreach (string file in files.Where(file => !currentFanartList.Contains(file)))
          {
            currentFanartList.Add(file);
            //LogMyFilms.Debug("GetRandomFanartForFilms() - added fanart #" + currentFanartList.Count + " : '" + file + "'");
            i += 1;
            if (i >= limit && limit != 0)
              return true;
          }
      }
      return currentFanartList.Count > 0;
    }

    private string GetNewRandomFanart(bool reset, bool movie)
    {
      string newFanart = " ";
      bool success = false;
      int errorcount = 0;

      if (reset)
      {
        currentFanartList.Clear();
        if (movie)
          GetRandomFanartForFilms(25);
        else
          GetRandomFanartForGroups(25); // Limit items for performance reasons...
      }
      switch (currentFanartList.Count)
      {
        case 0:
          newFanart = " ";
          break;
        case 1:
          newFanart = currentFanartList[0];
          break;
        default:
          if (currentFanartList.Count > 1)
          {
            Int32 randomFanartIndex = -1;
            while (!success && errorcount < 5)
            {
              try
              {
                //Choose Random Fanart from Resultlist
                var rnd = new Random();
                randomFanartIndex = rnd.Next(currentFanartList.Count);
                if (currentFanartList.Count <= randomFanartIndex)
                {
                  LogMyFilms.Debug("GetNewRandomFanart() - error, invalid index !  - Available: '" + currentFanartList.Count + "', selected ID: '" + randomFanartIndex + "' - returning empty fanart !");
                  return " ";
                }
                newFanart = currentFanartList[randomFanartIndex];
                if (newFanart != backdrop.Filename)
                {
                  success = true;
                  LogMyFilms.Debug("GetNewRandomFanart - total: '" + currentFanartList.Count + ", ID: '" + randomFanartIndex + "', Path: '" + newFanart + "'");
                }
                else
                {
                  errorcount += 1;
                  Thread.Sleep(50);
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.Warn("GetNewRandomFanart() - error, invalid index !  - Available: '" + currentFanartList.Count + "', selected ID: '" + randomFanartIndex + "' - " + ex.Message);
                errorcount += 1;
                success = false;
              }
            }
          }
          break;
      }
      return newFanart;
    }

    //----------------------------------------------------------------------------------------
    //    Display rating (Hide/show Star Images)
    //    altered to add half stars
    //   0-0.4=(0)=0s  | 0.5-1.4=(1)=0.5s | 1.5-2.4=(2)=1s   | 2.5-3.4=(3)=1.5s 
    // 3.5-4.4=(4)=2s  | 4.5-5.4=(5)=2.5s | 5.5-6.4=(6)=3s   | 6.5-7.4=(7)=3.5s
    // 7.5-8.4=(8)=4s  | 8.5-9.4=(9)=4.5s | 9.5-10=(10)=5s
    //----------------------------------------------------------------------------------------
    private void Load_Rating(decimal rating)
    {
      int r, i;
      r = Decimal.ToInt32(Decimal.Round(rating, MidpointRounding.AwayFromZero)); // by setting rating here can easily modify for diff effect
      if (r > 10) r = 10;
      for (i = 0; i < 10; i++)
      {
        if (r > i)
          GUIControl.ShowControl(GetID, 24 + i);
        else
          GUIControl.HideControl(GetID, 24 + i);
      }
    }

    static readonly Regex GroupCountMatches = new Regex(@"\([0-9]*\)", RegexOptions.Compiled);

    //----------------------------------------------------------------------------------------
    //    Display Detailed Info (Image, Description, Year, Category)
    //----------------------------------------------------------------------------------------
    private void Load_Lstdetail(GUIListItem currentItem, bool forceLoading)
    {
      #region sanity checks ...
      if (currentItem != facadeFilms.SelectedListItem)
      {
        LogMyFilms.Debug("Load_Lstdetail() - Skipping: currentItem is not the current facadelist item ! -> return");
        return;
      }
      if (currentItem == null)
      {
        LogMyFilms.Warn("Load_Lstdetail() - Skipping: currentItem is null ! -> return");
        return;
      }
      if (currentItem.ItemId == -1)
      {
        // reinit some fields //filmcover.Filename = ""; //backdrop.Filename = ""; //MyFilmsDetail.Init_Detailed_DB(false);
        LogMyFilms.Debug("Load_Lstdetail() - Skipping: ItemId == -1 -> return");
        return;
      }
      if ((r == null || currentItem.ItemId > r.Length - 1) && conf.ViewContext != ViewContext.TmdbMovies)
      {
        LogMyFilms.Warn("Load_Lstdetail() - Failed loading data row  - index not within current dataset !");
      }
      #endregion

      LogMyFilms.Debug("Load_Lstdetail() - Start: ItemId = '" + currentItem.ItemId + "', label = '" + currentItem.Label + "', ViewContext = '" + conf.ViewContext + "', forceLoading = '" + forceLoading + "'");

      if (currentItem.ItemId == Prev_ItemID && currentItem.Label == Prev_Label && !forceLoading)
      {
        LogMyFilms.Debug("Load_Lstdetail() - Skipping: ItemId == Prev_ItemID ('" + Prev_ItemID + "') AND  currentItem.Label == Prev_Label ('" + Prev_Label + "') -> return");
        return;
      }
      Prev_ItemID = currentItem.ItemId;
      Prev_Label = currentItem.Label;

      switch (conf.ViewContext)
      {
        case ViewContext.Menu:
        case ViewContext.MenuAll:
          #region menu types
          //conf.MenuSelectedID = currentItem.ItemId; // remember last menu position ...  
          MyFilmsDetail.Init_Detailed_DB(false);
          // GUIControl.ShowControl(GetID, 34);
          Clear_Logos();
          menucover.Filename = currentItem.ThumbnailImage;
          filmcover.Filename = "";
          viewcover.Filename = "";
          personcover.Filename = "";
          groupcover.Filename = "";
          SetDummyControlsForFacade(conf.ViewContext);

          MFview.ViewRow selectedView = GetCustomViewFromViewLabel(currentItem.Label);
          if (selectedView != null) MyFilmsDetail.setGUIProperty("index", selectedView.Index.ToString());
          else MyFilmsDetail.clearGUIProperty("index");

          if (conf.StrFanartDefaultViewsUseRandom)
          {
            string menuFanart = GetNewRandomFanart(true, false); // resets and populates fanart list and selects a random one
            if (menuFanart != " " && !string.IsNullOrEmpty(menuFanart))
            {
              backdrop.Filename = menuFanart;
              MyFilmsDetail.setGUIProperty("currentfanart", menuFanart);
              Fanartstatus(true);
            }
            else
            {
              Fanartstatus(false);
              backdrop.Filename = " ";
              MyFilmsDetail.setGUIProperty("currentfanart", " ");
            }
          }
          #endregion
          break;
        //case ViewContext.Movie:
        //case ViewContext.MovieCollection:
        //case ViewContext.Group:
        //case ViewContext.Person:
        //  break;
        case ViewContext.TmdbMovies:
          filmcover.Filename = currentItem.ThumbnailImage;
          Clear_Logos();
          #region set fanart
          new Thread(delegate()
          {
            {
              LogMyFilms.Debug("Load_Lstdetail() - Sleep 500 ms to let animations go ...");
              Thread.Sleep(500); // wait, so animations don't stutter
              try
              {
                if (conf.StrFanartDefaultViewsUseRandom) currentFanartList.Clear();
                string[] wfanart = MyFilmsDetail.Search_Fanart(currentItem.Label, true, "file", false, currentItem.ThumbnailImage, string.Empty);
                //if (conf.StrFanartDefaultViewsUseRandom) // not usable for TMDB film lists
                //{
                //  string FilmFanart = GetNewRandomFanart(true, true);
                //  // resets and populates fanart list and selects a random one
                //  if (FilmFanart != " ") wfanart[0] = FilmFanart;
                //}
                LogMyFilms.Debug("Load_Lstdetail() - Backdrops-File: wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                Fanartstatus(wfanart[0] != " ");
                backdrop.Filename = wfanart[0];
                MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
                LogMyFilms.Debug("Load_Lstdetail() - Fanart-Status: '" + backdrop.Active + "', Backdrops-File: backdrop.Filename = wfanart[X]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
              }
              catch (Exception ex)
              {
                LogMyFilms.Warn("Load_Lstdetail() - Fanart-exception: '" + ex.Message + "'");
              }
            }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => 0, 0, 0, null);
          }) { Name = "MyFilmsSetFanartOnPageLoadWorker", IsBackground = true }.Start();
          #endregion

          SetDummyControlsForFacade(conf.ViewContext);

          try
          {
            MyFilmsDetail.Load_Detailed_TMDB(currentItem);
          }
          catch (Exception e) { LogMyFilms.Error("Load_Lstdetail() - Error loading 'Load_Detailed_TMDB()': " + e.Message); }

          if (!currentItem.IsFolder) { }
          break;
        case ViewContext.None:
          break;
        default:
          if (currentItem.IsFolder && conf.Boolselect)
          #region Views with grouping ...
          {
            LogMyFilms.Debug("Load_Lstdetail() - Item is Folder and BoolSelect is true");
            MyFilmsDetail.Init_Detailed_DB(false);

            switch (conf.ViewContext)
            {
              case ViewContext.Person:
                personcover.Filename = currentItem.ThumbnailImage;
                //MatchCollection matchList = GroupCountMatches.Matches(currentItem.Label2);
                //if (matchList.Count > 0)
                //{
                //  Match matcher = matchList[0];
                //  string personcount = matcher.Value.Trim(new Char[] { '(', ')' }).Trim();
                //  MyFilmsDetail.setGUIProperty("user.mastertitle.groupcount", personcount, true);
                //}
                break;
              case ViewContext.Group:
                MyFilmsDetail.setGUIProperty("user.mastertitle.groupcount", currentItem.Label2, true);
                viewcover.Filename = currentItem.ThumbnailImage;
                break;
              default:
                //conf.FileImage = currentItem.ThumbnailImage;
                //viewcover.Filename = conf.FileImage;
                //personcover.Filename = conf.FileImage;
                //filmcover.Filename = conf.FileImage;
                //groupcover.Filename = conf.FileImage;
                break;
            }
            MyFilmsDetail.setGUIProperty("picture", currentItem.ThumbnailImage, true);

            #region fanart
            new Thread(delegate()
              {
                string[] wfanart;
                {
                  LogMyFilms.Debug("Load_Lstdetail() - Sleep 500 ms to let animations go ...");
                  Thread.Sleep(conf.ViewContext == ViewContext.Person ? 750 : 500);
                  try
                  {
                    wfanart = MyFilmsDetail.Search_Fanart(
                      currentItem.Label, true, "file", true, currentItem.ThumbnailImage, currentItem.Path);
                    if (conf.StrFanartDefaultViewsUseRandom)
                    {
                      string groupFanart = GetNewRandomFanart(true, false);
                        // resets and populates fanart list and selects a random one
                      if (groupFanart != " ") wfanart[0] = groupFanart;
                    }
                    Fanartstatus(wfanart[0] != " ");
                    backdrop.Filename = wfanart[0];
                    MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
                    LogMyFilms.Debug("Load_Lstdetail() - Backdrop status: '" + backdrop.Active + "', backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Warn("Load_Lstdetail() - Fanart-exception: '" + ex.Message + "'");
                  }
                }
              })
              {
                Name = "MyFilmsSetFanartOnPageLoadWorker", IsBackground = true, Priority = ThreadPriority.BelowNormal
              }.Start();
            #endregion

            // Load_Rating(0); // old method - nor more used
            Clear_Logos(); // reset logos

            GUIControl.ShowControl(GetID, 34);
            SetDummyControlsForFacade(conf.ViewContext);

            switch (conf.ViewContext)
            {
              case ViewContext.Person:
                MyFilmsDetail.clearGUIProperty("user.source.isonline");
                MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");

                // MyFilmsDetail.Init_Detailed_DB(false);
                // MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false);
                string personname = conf.BoolReverseNames && currentItem.Label != EmptyFacadeValue ? ReReverseName(currentItem.Label) : currentItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value
                MyFilmsDetail.Load_Detailed_PersonInfo(personname, currentItem);
                break;
              case ViewContext.Group:
                MyFilmsDetail.clearGUIProperty("user.source.isonline");
                MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");
                // MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false);
                break;
            }
          }
          #endregion
          else
          #region Movie display ...
          {
            LogMyFilms.Debug("Load_Lstdetail() - Item is Film List Item - contains hierarchy = '" + currentItem.IsFolder + "'");
            if (currentItem.IsFolder) // if it is a group/box-set, clear all properties
            {
              // GUIControl.ShowControl(GetID, 34);
              MyFilmsDetail.Init_Detailed_DB(false);
            }

            #region set groupcount for movies
            if (currentItem.IsFolder) // contains collection for a hierarchy
            {
              Regex p = new Regex(@"\([0-9]*\)"); // count in brackets
              MatchCollection matchList = p.Matches(currentItem.Label2);
              if (matchList.Count > 0)
              {
                Match matcher = matchList[0];
                string groupcount = matcher.Value.Trim('(', ')').Trim();
                MyFilmsDetail.setGUIProperty("user.mastertitle.groupcount", groupcount, true);
              }
            }
            else
              MyFilmsDetail.clearGUIProperty("user.mastertitle.groupcount");
            #endregion

            string currentFilmCover = "";
            if (currentItem.MusicTag != null && !string.IsNullOrEmpty(currentItem.MusicTag.ToString()))
              currentFilmCover = currentItem.MusicTag.ToString();
            else if (!string.IsNullOrEmpty(currentItem.ThumbnailImage))
              currentFilmCover = currentItem.ThumbnailImage;
            //currentFilmCover = (currentItem.MusicTag != null && !string.IsNullOrEmpty(currentItem.MusicTag.ToString()))
            //                            ? currentItem.MusicTag.ToString()
            //                            : ((!string.IsNullOrEmpty(currentItem.ThumbnailImage)) ? currentItem.ThumbnailImage : "");
            LogMyFilms.Debug("Load_Lstdetail() - currentFilmCover = '" + currentFilmCover + "'");

            if (currentItem.IsFolder) //  && conf.ViewContext == ViewContext.Movie
            {
              string currentGroupCover = GetGroupImage(currentItem); // HierarchyImage for selected item
              if (currentGroupCover.Length > 0) currentFilmCover = currentGroupCover;
              LogMyFilms.Debug("Load_Lstdetail() - currentFilmCover (GroupImage) = '" + currentFilmCover + "'");
            }
            filmcover.Filename = currentFilmCover;
            MyFilmsDetail.setGUIProperty("picture", currentFilmCover, true);
            if (currentItem.IsFolder && conf.ViewContext == ViewContext.Movie)
              groupcover.Filename = currentFilmCover;

            Load_Logos(r, currentItem.ItemId); // set logos

            #region set fanart
            new Thread(delegate()
            {
              {
                LogMyFilms.Debug("Load_Lstdetail() - Sleep 500 ms to let animations go ...");
                Thread.Sleep(500); // wait, so animations don't stutter
                try
                {
                  if (conf.StrFanartDefaultViewsUseRandom) currentFanartList.Clear();
                  string fanartTitle = currentItem.IsFolder ? currentItem.Label : MyFilmsDetail.GetSearchTitles(r[currentItem.ItemId], "").FanartTitle;
                  LogMyFilms.Debug("Load_Lstdetail() - FanartTitle from facadeview = '" + fanartTitle + "'");
                  #region disabled code
                  //string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
                  //fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeFilms.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
                  // fanartTitle = currentItem.Label;
                  //if (MyFilms.conf.StrTitle1 == "FormattedTitle") // added to get fanart displayed when mastertitle is set to formattedtitle
                  //{
                  //  fanartTitle = MyFilms.r[facadeFilms.SelectedListItem.ItemId]["OriginalTitle"].ToString();
                  //  //Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
                  //  if (MyFilms.conf.StrTitle1 != "OriginalTitle")
                  //  {

                  //    if (MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"] != null && MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"].ToString().Length > 0)
                  //      fanartTitle = MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"].ToString();
                  //  }
                  //}
                  #endregion
                  string[] wfanart = MyFilmsDetail.Search_Fanart(fanartTitle, true, "file", false, currentFilmCover, string.Empty);
                  if (conf.StrFanartDefaultViewsUseRandom)
                  {
                    string filmFanart = GetNewRandomFanart(true, true);
                    // resets and populates fanart list and selects a random one
                    if (filmFanart != " ") wfanart[0] = filmFanart;
                  }

                  LogMyFilms.Debug("Load_Lstdetail() - Backdrops-File: wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                  Fanartstatus(wfanart[0] != " ");
                  backdrop.Filename = wfanart[0];
                  MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
                  LogMyFilms.Debug("Load_Lstdetail() - Fanart-Status: '" + backdrop.Active + "', Backdrops-File: backdrop.Filename = wfanart[X]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                catch (Exception ex)
                {
                  LogMyFilms.Warn("Load_Lstdetail() - Fanart-exception: '" + ex.Message + "'");
                }
              }
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => 0, 0, 0, null);
            }) { Name = "MyFilmsSetFanartOnPageLoadWorker", IsBackground = true }.Start();
            #endregion

            SetDummyControlsForFacade(conf.ViewContext);
            // Load_Rating(conf.W_rating); // old method - no more used

            if (!currentItem.IsFolder)
            {
              // GUIControl.HideControl(GetID, 34);
              MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId); // load details, if it is not a hierarchyentry (folder)
            }
          }
          #endregion
          break;
      }
      // itemToPublish = null;
    }

    //-------------------------------------------------------------------------------------------
    //  Control search Text : no specials characters only alphanumerics
    //-------------------------------------------------------------------------------------------        
    private static bool control_searchText(string stext)
    {
      var maRegexp = new Regex("^[^*]+$");
      var regOk = maRegexp.IsMatch(stext);
      return regOk;
    }
    //-------------------------------------------------------------------------------------------
    //  Proc for Sort Button
    //-------------------------------------------------------------------------------------------        
    void SortChanged(object sender, SortEventArgs e)
    {
      LogMyFilms.Debug("SortChanged triggered: ViewConext = '" + conf.ViewContext + "'");
      switch (conf.ViewContext)
      {
        case ViewContext.MenuAll:
        case ViewContext.Menu:
          return; // no sorting for menu types !

        case ViewContext.TmdbMovies:
          try
          {
            //BtnSrtBy.Label = (conf.BoolCollection) ? (GUILocalizeStrings.Get(96) + ((conf.StrSortaInHierarchies == conf.StrSTitle) ? GUILocalizeStrings.Get(103) : BaseMesFilms.TranslateColumn(conf.StrSortaInHierarchies))) : (GUILocalizeStrings.Get(96) + ((conf.StrSorta == conf.StrSTitle) ? GUILocalizeStrings.Get(103) : BaseMesFilms.TranslateColumn(conf.StrSorta)));
            //BtnSrtBy.IsAscending = (conf.BoolCollection) ? (conf.StrSortSensInHierarchies == " ASC") : (conf.StrSortSens == " ASC");
            if (e.Order.ToString().Substring(0, 3).ToLower() == _currentSortDirection.Trim().Substring(0, 3).ToLower())
              return;
            _currentSortDirection = BtnSrtBy.IsAscending ? " ASC" : " DESC";

            SetLabels();
            facadeFilms.Sort(this);
            UpdateButtonStates();
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("SortChanged - TMDB Movies: Error sorting items - {0}", ex.ToString());
          }
          return;

        default:
          LogMyFilms.Debug("SortChanged() - handler called with order = '" + e.Order + "'");
          LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - WStrSortSensCount        : '{0}'", conf.WStrSortSensCount));
          LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - wStrSortSens             : '{0}'", conf.WStrSortSens));
          LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - StrSortSensInHierarchies : '{0}'", conf.StrSortSensInHierarchies));
          LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - StrSortSens              : '{0}'", conf.StrSortSens));
          if (conf.Boolselect) //in Views / Groupviews
          {
            if (conf.BoolSortCountinViews)
            {
              if (e.Order.ToString().Substring(0, 3).ToLower() == conf.WStrSortSensCount.Trim().Substring(0, 3).ToLower())
                return;
              conf.WStrSortSensCount = BtnSrtBy.IsAscending ? " ASC" : " DESC";
            }
            else // normal sort direction for views/groups
            {
              if (e.Order.ToString().Substring(0, 3).ToLower() == conf.WStrSortSens.Trim().Substring(0, 3).ToLower())
                return;
              conf.WStrSortSens = BtnSrtBy.IsAscending ? " ASC" : " DESC";
            }
          }
          else
          {
            if (conf.BoolCollection) // film groups/collections
            {
              if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSensInHierarchies.Trim().Substring(0, 3).ToLower())
                return;
              conf.StrSortSensInHierarchies = BtnSrtBy.IsAscending ? " ASC" : " DESC";
            }
            else // normal sortdirection in film groups/collections
            {
              if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSens.Trim().Substring(0, 3).ToLower())
                return;
              conf.StrSortSens = BtnSrtBy.IsAscending ? " ASC" : " DESC";
            }
          }

          if (conf.Boolselect)
            GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, "");
          else
            GetFilmList();
          break;
      }
    }

    private void UpdateButtonStates()
    {
      try
      {
        string strLine = string.Empty;
        if (BtnSrtBy != null)
        {
          switch (_currentSortMethod)
          {
            case SortMethod.Name:
              strLine = GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(103); //Sort by: Title
              break;
            case SortMethod.Date:
              strLine = GUILocalizeStrings.Get(96) + BaseMesFilms.TranslateColumn("Date"); //Sort by: Date
              break;
            case SortMethod.Rating:
              strLine = GUILocalizeStrings.Get(96) + BaseMesFilms.TranslateColumn("Rating"); //Sort by: Rating
              break;
            case SortMethod.Votes:
              strLine = GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(205); //Sort by: Votes
              break;
          }
          GUIControl.SetControlLabel(GetID, BtnSrtBy.GetID, strLine);
        }

        // if (null != facadeFilms) facadeFilms.EnableScrollLabel = _currentSortMethod == SortMethod.Name;

      }
      catch (Exception ex)
      {
        LogMyFilms.Warn("UpdateButtonStates: Error updating button states - {0}", ex.ToString());
      }
    }

    private void SetLabels()
    {
      SortMethod method = _currentSortMethod;
      bool bAscending = conf.StrSortSens == " ASC";

      for (int i = 0; i < facadeFilms.Count; ++i)
      {
        try
        {
          GUIListItem item = facadeFilms[i];
          OnlineMovie onlineMovie = (OnlineMovie)item.TVTag;

          if (onlineMovie == null) continue;

          if (conf.StrLayOut == 0) // if list layout
          {
            item.Label2 = "";
            // item.Label3 = ""; // do not change Label3 when sorting, keep original value !
          }

          switch (_currentSortMethod)
          {
            case SortMethod.Date:
              try
              {
                string label = DateTime.Parse(onlineMovie.MovieSearchResult.release_date).ToShortDateString();
                if (!String.IsNullOrEmpty(label))
                {
                  item.Label2 = label;
                }
              }
              catch (Exception) { }
              break;
            case SortMethod.Rating:
              try
              {
                string label = onlineMovie.MovieSearchResult.vote_average.ToString("0.0");
                if (!String.IsNullOrEmpty(label))
                {
                  item.Label2 = label;
                }
              }
              catch (Exception) { }
              break;
            case SortMethod.Votes:
              try
              {
                string label = onlineMovie.MovieSearchResult.vote_count.ToString();
                if (!String.IsNullOrEmpty(label))
                {
                  item.Label2 = label;
                }
              }
              catch (Exception) { }
              break;

            default:
              try
              {
                string label = DateTime.Parse(onlineMovie.MovieSearchResult.release_date).ToShortDateString();
                if (!String.IsNullOrEmpty(label))
                {
                  item.Label2 = label;
                }
                label = onlineMovie.MovieSearchResult.vote_average.ToString("0.0") + " (" + onlineMovie.MovieSearchResult.vote_count + ")";
                if (!String.IsNullOrEmpty(label))
                {
                  item.Label3 = label;
                }
              }
              catch (Exception) { }
              break;

          }

          //Log.Debug("SetLabels: SetLabels - 1: {0}, 2: {1}, 3: {2}", item1.Label, item1.Label2, item1.Label3);
        }
        catch (Exception ex)
        {
          LogMyFilms.Warn("SetLabels: error in SetLabels - {0}", ex.Message);
        }
      }
    }

    private void item_OnItemSelected(GUIListItem item, GUIControl parent)
    {
      // LogMyFilms.Debug("Call item_OnItemSelected()with options - item: '" + item.ItemId + "', SelectedListItemIndex: '" + facadeFilms.SelectedListItemIndex.ToString() + "', Label: '" + facadeFilms.SelectedListItem.Label + "', TVtag: '" + item.TVTag.ToString() + "'");

      if (item == null || parent == null)
      {
        LogMyFilms.Warn("item_OnItemSelected() - null detected - return !");
        return;
      }

      var filmstrip = parent as GUIFilmstripControl;
      if (filmstrip != null) filmstrip.InfoImageFileName = item.ThumbnailImage;

      MovieDetailsPublisher(item);

      #region old code
      //if (facadeFilms.SelectedListItem.ItemId == Prev_ItemID && facadeFilms.SelectedListItem.Label == Prev_Label)
      //{
      //  LogMyFilms.Debug("(item_OnItemSelected): ItemId == Prev_ItemID (" + Prev_ItemID + ") && label == Prev_Label (" + Prev_Label + ") -> return without action !");
      //  return;
      //}

      //if (!(conf.Boolselect || (facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder))) //xxxx
      //{
      //  if (facadeFilms.SelectedListItemIndex > -1)
      //    MovieDetailsPublisher(item, true); //Load_Lstdetail(facadeFilms.SelectedListItem.ItemId, true, facadeFilms.SelectedListItem.Label);
      //}
      //else
      //{
      //  if (facadeFilms.SelectedListItemIndex > -1 && !conf.Boolselect)
      //  {
      //    MovieDetailsPublisher(item, false); //Load_Lstdetail(facadeFilms.SelectedListItem.ItemId, false, facadeFilms.SelectedListItem.Label);
      //  }
      //  else
      //  {
      //    MovieDetailsPublisher(item, false); //Load_Lstdetail(facadeFilms.SelectedListItem.ItemId, false, facadeFilms.SelectedListItem.Label);
      //    //GUIControl.ShowControl(GetID, 34);
      //  }
      //}
      // Load_Lstdetail(item.ItemId, true, item.Label);
      #endregion
    }

    delegate void MovieDetailsPublisherWorker(GUIListItem item, bool wrep);

    private void MovieDetailsPublisher(GUIListItem item)
    {
      double tickCount = System.Windows.Media.Animation.AnimationTimer.TickCount;
      int diff = (int)(tickCount - lastPublished);
      LogMyFilms.Debug("MovieDetailsPublisher() - diff: '" + diff.ToString() + "', ItemId: '" + item.ItemId + "', label: '" + item.Label + "'");
      itemToPublish = item; // Update instance of delayed item with current position

      if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID)) // if loaded movie via loadparams, never use delayed loading ...
      {
        LogMyFilms.Debug("MovieDetailsPublisher() - direct call by load paramaters");
        Load_Lstdetail(itemToPublish, false);
      }
      else if ((ItemPublishDelay < diff) || loadParamInfo != null) // wait 350 ms to load details... if loaded via loadparams, never use delayed loading ...
      {
        // Publish instantly when previous request has passed the required delay
        // LogMyFilms.Debug("MovieDetailsPublisher() - timer elapsed - launch MovieDetailsPublisherWorker");
        lastPublished = tickCount;
        var publisher = new MovieDetailsPublisherWorker(Load_Lstdetail);
        publisher.BeginInvoke(itemToPublish, false, null, null);

        // Load_Lstdetail(itemToPublish, false);
        // Load_Lstdetail(ItemId, wlabel); 
      }
      else // Publish on timer using delay specified
      {
        lastPublished = tickCount;
        if (publishTimer == null)
        {
          // LogMyFilms.Info("MovieDetailsPublisher() - Create new timer with callback");
          publishTimer = new Timer(delegate { Load_Lstdetail(itemToPublish, false); }, null, ItemPublishDelay, Timeout.Infinite);
        }
        else
        {
          // LogMyFilms.Debug("MovieDetailsPublisher() - Update timer to 250 ms");
          publishTimer.Change(250, Timeout.Infinite);
        }
      }
    }

    #region Accès Données

    //--------------------------------------------------------------------------------------------
    //  Select View for Video
    //--------------------------------------------------------------------------------------------
    private void Change_View_Menu()
    {
      var choiceView = new List<string>();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079903)); // Change View ...
      #region old hardcoded entries - disabled
      //dlg.Add(GUILocalizeStrings.Get(342));//videos
      //choiceView.Add("All");
      //dlg.Add(GUILocalizeStrings.Get(345));//year
      //choiceView.Add("Year");
      //dlg.Add(GUILocalizeStrings.Get(10798664));//genre -> category
      //choiceView.Add("Category");
      //dlg.Add(GUILocalizeStrings.Get(200026));//countries
      //choiceView.Add("Country");
      //dlg.Add(GUILocalizeStrings.Get(10798954));//recentlyadded
      //choiceView.Add("RecentlyAdded");
      //dlg.Add(GUILocalizeStrings.Get(10798667));//actors 
      //choiceView.Add("Actors");
      //dlg1.Add(GUILocalizeStrings.Get(200027));//Watched

      // Commented, as we have replaced this feature with global overlay filter for "media available"
      //if (Helper.FieldIsSet(conf.StrStorage.Length))
      //{
      //    dlg1.Add(GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951));//storage
      //    SelectedView.Add("storage");
      //}
      #endregion

      if (conf.CustomViews.View.Rows.Count > 0)
      {
        foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => Helper.FieldIsSet(customView.DBfield) && customView.ViewEnabled))
        {
          choiceView.Add(customView.Label); //choiceView.Add(string.Format("View{0}", i));
          dlg.Add(string.IsNullOrEmpty(customView.Label) ? customView.DBfield : customView.Label);
        }

        dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
        choiceView.Add("showall");

        // add menu as option
        dlg.Add(GUILocalizeStrings.Get(924)); // menu
        choiceView.Add("Menu");

        // add online movie lists as option
        dlg.Add("*** " + GUILocalizeStrings.Get(10798825) + " ***"); // 10798825 online information
        choiceView.Add("onlineinfo");

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
      }

      // show all sort options, if selected or no custom views available ...
      if (conf.CustomViews.View.Rows.Count == 0 || choiceView[dlg.SelectedLabel] == "showall")
      {
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(1079903)); // Change View (films) ...
        choiceView.Clear();
        var displayItems = GetDisplayItems("view");
        foreach (string[] displayItem in displayItems)
        {
          string entry = string.IsNullOrEmpty(displayItem[1]) ? displayItem[0] : displayItem[1];
          dlg.Add(entry);
          choiceView.Add(displayItem[0]);
          LogMyFilms.Debug("View Menu - add '{0}' as '{1}'", displayItem[0], entry);
        }
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
      }
      conf.StrSelect = ""; // reset movie context filter for person views
      conf.StrPersons = ""; // reset person list filter
      viewcover.Filename = "";
      personcover.Filename = "";
      groupcover.Filename = "";

      Change_View_Action(choiceView[dlg.SelectedLabel]);
    }

    private static int CountViewItems(IEnumerable<DataRow> filmrows, string wStrSort)
    {
      int count = 0;
      HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //List<string> itemList = new List<string>();

      bool issplitfield = IsSplittableField(wStrSort);
      bool dontsplitvalues = conf.BoolDontSplitValuesInViews;
      bool showEmptyValues = conf.BoolShowEmptyValuesInViews;

      watch.Reset(); watch.Start();
      foreach (string champselect in filmrows.Select(row => row[wStrSort].ToString().Trim()))
      {
        if (issplitfield && !dontsplitvalues)
        {
          List<string> wtab = Search_String(champselect, false);
          if (wtab.Count > 0) // itemList.AddRange(wtab);
          {
            int count1 = wtab.Count(t => set.Add(t));
            count += count1;
          }
          else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
        else
        {
          if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
      }
      watch.Stop();
      LogMyFilms.Debug("CountViewItems - Finished Count ('" + count + "') (" + watch.ElapsedMilliseconds + " ms)  - Read View Names for '" + wStrSort + "' finished (splittable items = '" + issplitfield + "', dontsplitvalues = '" + dontsplitvalues + "')");
      return count;

      // int count = itemList.Distinct().Count();
      // int count = itemList.Distinct(MfStringComparer).Count();

      //HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      //foreach (string s in itemList) if (set.Add(s)) count++;

      //Dictionary<object, int> counts = new Dictionary<object, int>();
      //foreach (object item in ItemList)
      //{
      //  if (!counts.ContainsKey(item)) counts.Add(item, 1);
      //  // else counts[item]++; // we don't need the number of the items itself ...
      //}
      //int count = counts.Count;
    }

    private Dictionary<T, int> CountOccurences<T>(IEnumerable<T> items)
    {
      var occurences = new Dictionary<T, int>();
      foreach (T item in items)
      {
        if (occurences.ContainsKey(item))
        {
          occurences[item]++;
        }
        else
        {
          occurences.Add(item, 1);
        }
      }
      return occurences;
    }

    //--------------------------------------------------------------------------------------------
    //  Load Online Info Menu into facade list ...
    //--------------------------------------------------------------------------------------------
    private void GetSelectFromOnlineMenuView()
    {
      LogMyFilms.Debug("GetSelectFromOnlineMenuView() - launched ...");

      Change_Layout_Action(0); // always use list view // Change_Layout_Action(MyFilms.conf.WStrLayOut);  // we share the layout with Views ...

      BtnSrtBy.Label = GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(103); // sort: name
      //BtnSrtBy.IsAscending = true;
      BtnSrtBy.IsEnabled = false;

      // SetDummyControlsForFacade(ViewContext.Menu); // reset all covers ...
      GUIControl.ShowControl(GetID, 34); // hide film controls ...
      SetLabelView("online");
      SetLabelSelect("");

      conf.ViewContext = ViewContext.Menu;

      if (conf.StrFanart && conf.StrFanartDfltImage)
      {
        if (backdrop.Filename == "") backdrop.Filename = conf.DefaultFanartImage;
        Fanartstatus(true);
      }

      ClearFacade(); // facadeFilms.Clear();
      GUIListItem item;

      // Popular Movies - retrieve the daily movie popularity list
      item = new GUIListItem
      {
        Label = GUILocalizeStrings.Get(10798826),
        DVDLabel = "TMDBaction",
        IsFolder = true
      };
      // Popular Movies
      item.ThumbnailImage = GetImageforMenu(item);
      item.IconImage = item.ThumbnailImage;
      item.IconImageBig = item.ThumbnailImage;
      item.OnItemSelected += item_OnItemSelected;
      if (facadeFilms != null) facadeFilms.Add(item);

      //Now Playing Movies -  retrieve the movies currently in theatres
      item = new GUIListItem
      {
        Label = GUILocalizeStrings.Get(10798827),
        DVDLabel = "TMDBaction",
        IsFolder = true
      };
      // Now Playing
      item.ThumbnailImage = GetImageforMenu(item);
      item.IconImage = item.ThumbnailImage;
      item.IconImageBig = item.ThumbnailImage;
      item.OnItemSelected += item_OnItemSelected;
      if (facadeFilms != null) facadeFilms.Add(item);

      //Top Rated Movies -  retrieve the top rated movies that have over 10 votes on TMDb
      item = new GUIListItem
      {
        Label = GUILocalizeStrings.Get(10798828),
        DVDLabel = "TMDBaction",
        IsFolder = true
      };
      // Top Rated
      item.ThumbnailImage = GetImageforMenu(item);
      item.IconImage = item.ThumbnailImage;
      item.IconImageBig = item.ThumbnailImage;
      item.OnItemSelected += item_OnItemSelected;
      if (facadeFilms != null) facadeFilms.Add(item);

      //Upcoming Movies - retrieve the movies arriving to theatres within the next few weeks
      item = new GUIListItem
      {
        Label = GUILocalizeStrings.Get(10798829),
        DVDLabel = "TMDBaction",
        IsFolder = true
      };
      // Upcoming Movies
      item.ThumbnailImage = GetImageforMenu(item);
      item.IconImage = item.ThumbnailImage;
      item.IconImageBig = item.ThumbnailImage;
      item.OnItemSelected += item_OnItemSelected;
      if (facadeFilms != null) facadeFilms.Add(item);

      if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Online lists - IMDB Top250"))
      {
        //Upcoming Movies - retrieve IMDB Top250 movies (via grabber script)
        item = new GUIListItem
        {
          Label = GUILocalizeStrings.Get(10798824),
          DVDLabel = "TMDBaction",
          IsFolder = true
        };
        // IMDB Top250 movies
        item.ThumbnailImage = GetImageforMenu(item);
        item.IconImage = item.ThumbnailImage;
        item.IconImageBig = item.ThumbnailImage;
        item.OnItemSelected += item_OnItemSelected;
        if (facadeFilms != null) facadeFilms.Add(item);
      }

      if (facadeFilms == null) return;

      MyFilmsDetail.setGUIProperty("nbobjects.value", facadeFilms.Count.ToString());
      GUIPropertyManager.SetProperty("#itemcount", facadeFilms.Count.ToString());
      // GUIPropertyManager.SetProperty("#itemtype", "Views"); // disabled, as we otherwise have to set it in all facade listings ...
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select")); // will be set in SetLabel
      LogMyFilms.Debug("GetSelectFromOnlineMenuView() - end facade load ...");
    }

    //--------------------------------------------------------------------------------------------
    //  Load Views for Video from facade list ...
    //--------------------------------------------------------------------------------------------
    private void GetSelectFromMenuView(bool showall)
    {
      LogMyFilms.Debug("GetSelectFromMenuView() - launched with showall = '" + showall + "'");
      conf.BoolMenuShowAll = showall; // remember state
      if (conf.CustomViews.View.Rows.Count == 0) showall = true; // show generic views, if there is no Custom Views defined in config - but keep it as "base menu"

      Prev_ItemID = -1;
      Prev_Label = string.Empty;

      Change_Layout_Action(0); // always use list view // Change_Layout_Action(MyFilms.conf.WStrLayOut);  // we share the layout with Views ...

      BtnSrtBy.Label = GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(103); // sort: name
      //BtnSrtBy.IsAscending = true;
      BtnSrtBy.IsEnabled = false;

      // SetDummyControlsForFacade(ViewContext.Menu); // reset all covers ...
      GUIControl.ShowControl(GetID, 34); // hide film controls ...
      SetLabelView("menu");
      SetLabelSelect("");

      conf.ViewContext = showall ? ViewContext.MenuAll : ViewContext.Menu;
      // conf.StrSelect = ""; // reset movie filter for views
      // conf.StrFilmSelect = "";

      if (conf.StrFanart && conf.StrFanartDfltImage)
      {
        if (backdrop.Filename == "") backdrop.Filename = conf.DefaultFanartImage;
        Fanartstatus(true);
      }

      LogMyFilms.Debug("GetSelectFromMenuView() - start facade load ...");
      ClearFacade(); // facadeFilms.Clear();

      GUIListItem item;
      if (!showall)
      {
        #region old hardcoded entries
        //item = new GUIListItem(); 
        //item.Label = GUILocalizeStrings.Get(342);//videos
        //item.DVDLabel = "All";
        //item.Label2 = r.Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();  // Select(row => row.Field<int?>("F1")).Where(val => val.HasValue).Select(val => val.Value).Distinct()
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item); 

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(345);//year
        //item.DVDLabel = "Year";
        //// item.Label2 = r.Select(p => p["Year"] != DBNull.Value).Distinct().Count().ToString(); 
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(10798664);//genre -> category
        //item.DVDLabel = "Category";
        //// item.Label2 = r.Select(p => p["Category"] != DBNull.Value).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(200026);//countries
        //item.DVDLabel = "Country";
        //// item.Label2 = r.Select(p => (string)p["Country"]).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(10798954);//genre -> recentlyadded
        //item.DVDLabel = "RecentlyAdded";
        ////item.Label2 = r.Select(p => p["RecentlyAdded"]).Distinct().Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(10798667);//actors
        //item.DVDLabel = "Actors";
        //// item.Label2 = r.Select(p => (string)p["Actors"]).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);
        #endregion

        foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => Helper.FieldIsSet(customView.DBfield) && customView.ViewEnabled))
        {
          item = new GUIListItem
          {
            DVDLabel = customView.Label,
            MusicTag = customView.DBfield,
            Label = (string.IsNullOrEmpty(customView.Label)) ? customView.DBfield : customView.Label,
            IsFolder = true
          };
          // (string.Format("View{0}", i));
          if (!string.IsNullOrEmpty(customView.ImagePath))
          {
            item.ThumbnailImage = customView.ImagePath;
            item.IconImage = customView.ImagePath;
            item.IconImageBig = customView.ImagePath;
          }
          else
          {
            string menuimage = GetImageforMenu(item);
            item.ThumbnailImage = menuimage;
            item.IconImage = menuimage;
            item.IconImageBig = menuimage;
          }
          item.OnItemSelected += item_OnItemSelected;
          if (facadeFilms != null) facadeFilms.Add(item);
        }
        #region add showall entry

        item = new GUIListItem
        {
          Label = GUILocalizeStrings.Get(10798765),
          DVDLabel = "showall",
          IsFolder = true
        };
        item.ThumbnailImage = GetImageforMenu(item);
        item.IconImage = item.ThumbnailImage;
        item.IconImageBig = item.ThumbnailImage;
        item.OnItemSelected += item_OnItemSelected;
        if (facadeFilms != null) facadeFilms.Add(item);
        #endregion

        #region add online info entry

        item = new GUIListItem
        {
          Label = "*** " + GUILocalizeStrings.Get(10798825) + " ***", // 10798825 online information
          DVDLabel = "onlineinfo",
          IsFolder = true
        };
        item.ThumbnailImage = GetImageforMenu(item);
        item.IconImage = item.ThumbnailImage;
        item.IconImageBig = item.ThumbnailImage;
        item.OnItemSelected += item_OnItemSelected;
        if (facadeFilms != null) facadeFilms.Add(item);
        #endregion
      }
      else
      {
        #region show all view options, if selected ...
        ArrayList displayItems = GetDisplayItems("view");
        foreach (string[] displayItem in displayItems)
        {
          item = new GUIListItem
          {
            Label = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1],
            DVDLabel = displayItem[0],
            MusicTag = displayItem[0],
            IsFolder = true
          };
          // item.Label2 = (!conf.OnlyTitleList) ? r.Select(p => (string)p[item.DVDLabel]).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString() : "";
          // item.Label2 = CountViewItems(item.DVDLabel).ToString(); 
          string menuimage = GetImageforMenu(item);
          item.ThumbnailImage = menuimage;
          item.IconImage = menuimage;
          item.IconImageBig = menuimage;
          item.OnItemSelected += item_OnItemSelected;
          if (facadeFilms != null) facadeFilms.Add(item);
        }
        #endregion
      }

      if (facadeFilms == null) return;
      if (conf.MenuSelectedId == -2)
        conf.MenuSelectedId = facadeFilms.Count - 1; // if -2 means coming from details menu -> set to "show all"/last position
      else if ((conf.MenuSelectedId > facadeFilms.Count - 1) || (conf.MenuSelectedId < 0)) //check index within bounds
        conf.MenuSelectedId = 0;
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, conf.MenuSelectedId);

      MyFilmsDetail.setGUIProperty("nbobjects.value", (!showall ? facadeFilms.Count : facadeFilms.Count - 1).ToString());
      GUIPropertyManager.SetProperty("#itemcount", (!showall ? facadeFilms.Count : facadeFilms.Count - 1).ToString());
      // GUIPropertyManager.SetProperty("#itemtype", "Views"); // disabled, as we otherwise have to set it in all facade listings ...
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select")); // will be set in SetLabel
      LogMyFilms.Debug("GetSelectFromMenuView() - end facade load ...");

      GetCountsForMenuView();
    }

    private void GetCountsForMenuView()
    {
      // load dataset and counts threaded ...
      LogMyFilms.Debug("GetCountsForMenuView() - start thread for menu counts ...");
      StopLoadingMenuDetails = false;

      new Thread(delegate()
      {
        #region Load Counts threaded
        // set online filter only, if scan is done already ...
        conf.StrGlobalFilterString = InitialIsOnlineScan ? GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D : GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;

        // DataRow[] allrows = BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens); // load dataset without filters
        r = BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrDfltSelect, conf.StrSelect, conf.StrSorta, conf.StrSortSens);
        // r = BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false, true); 
        // DataRow[] r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters

        for (int i = 0; i < facadeFilms.Count; i++)
        {
          if (StopLoadingMenuDetails) break;
          try
          {
            #region count items
            bool success = false;
            GUIListItem countitem = facadeFilms[i];
            
            // make sure, only valid DB fields are passing counting
            if (countitem.MusicTag == null || !IsMyFilmsDbColumn(countitem.MusicTag.ToString())) continue;

            if (!conf.OnlyTitleList && string.IsNullOrEmpty(countitem.Label2)) // first try to get info for custom views ...
            {
              #region get counts for menu item
              string newLabel = countitem.Label2;
              foreach (MFview.ViewRow mfView in Enumerable.Where(conf.CustomViews.View, mfView => Helper.FieldIsSet(mfView.DBfield)).Where(mfView => countitem.DVDLabel == mfView.Label))
              {
                success = true;
                if (string.IsNullOrEmpty(mfView.Value)) // no "Value" filter
                {
                  newLabel = string.IsNullOrEmpty(mfView.Filter) 
                    ? CountViewItems(r, mfView.DBfield).ToString() 
                    : CountViewItems(BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, "", conf.StrSorta, conf.StrSortSens), mfView.DBfield).ToString();
                }
                else if (mfView.Value == "*") // filmlist show all (possible "Value" filter) -> Count films, as it jumps directly to films
                {
                  newLabel = string.IsNullOrEmpty(mfView.Filter) 
                    ? r.Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString() 
                    : BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, "", conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();
                }
                else // "Value" filter present - use it !
                {
                  string valueFilter;
                  if (GetColumnType(mfView.DBfield) != typeof(string))
                    valueFilter = mfView.DBfield + " = '" + mfView.Value + "'";
                  else if (IsDateField(mfView.DBfield))
                    valueFilter = mfView.DBfield + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(mfView.Value).ToShortDateString()) + "*'";
                  else if (IsAlphaNumericalField(mfView.DBfield))
                    valueFilter = mfView.DBfield + " like '" + mfView.Value + "'";
                  else
                    valueFilter = mfView.DBfield + " like '*" + mfView.Value + "*'";

                  if (string.IsNullOrEmpty(mfView.Filter))
                    newLabel = "(" + mfView.Value + ") " + BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrDfltSelect, valueFilter, conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count();
                  else
                    newLabel = "(" + mfView.Value + ") " + BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, valueFilter, conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count();

                  //newLabel = "(" + viewRow.Value + ")";
                  //newLabel = "(" + viewRow.Value + ") " + r.Select(p => p[viewRow.DBfield].Equals(viewRow.Value)).Count().ToString();
                  //var ttt = r.Select(p => p[viewRow.DBfield].Equals(viewRow.Value)).Count().ToString();
                  //where d.Element("ProductName").Value.IndexOf(textBox1.Text, StringComparison.InvariantCultureIgnoreCase) > 0
                  // movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens)
                }
                countitem.Label2 = newLabel;
                // mfView.Label2 = newLabel; // update view cache
                if (StopLoadingMenuDetails) break;
              }
              if (!success) // get standard count, if no custom views match ...
              {
                newLabel = CountViewItems(r, countitem.DVDLabel).ToString();
                countitem.Label2 = newLabel;
                if (StopLoadingMenuDetails) break;
              }
              #endregion
            }
            #endregion
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug(ex, "MyFilmsMenuCountWorker() - error setting counts to facadelist item '" + i + "': " + ex.Message);
          }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          // Do this after thread finished ...
          // GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)conf.MenuSelectedID);
          return 0;
        }, 0, 0, null);
        #endregion
      }) { Name = "MyFilmsMenuCountWorker", IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start();
    }

    private string GetFieldFromViewLabel(string viewlabel)
    {
      foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => viewRow.Label == viewlabel))
      {
        return viewRow.DBfield;
      }
      return viewlabel;
    }

    private string GetImageforMenu(GUIListItem item)
    {
      string menuimage = "";
      if (item == null) return "";
      try
      {
        if (string.IsNullOrEmpty(item.ThumbnailImage))
        {
          // string strThumbDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.thumbsGroups) + wStrSort.ToLower() + @"\";
          if (conf.StrPathViews.Length > 0)
          {
            // Check, if default group cover is present
            if (conf.StrViewsDflt)
            {
              string strMenuImage;
              // special handling for funxtion entries
              switch (item.DVDLabel)
              {
                case "showall":
                  strMenuImage = Path.Combine(MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages), "ShowAll.jpg");
                  break;
                case "onlineinfo":
                  strMenuImage = Path.Combine(MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages), "OnlineInfo.jpg");
                  break;
                default:
                  strMenuImage = conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\" ? conf.StrPathViews : conf.StrPathViews + "\\";
                  strMenuImage = strMenuImage + GetFieldFromViewLabel(item.DVDLabel).ToLower() + "\\" + "Default.jpg";
                  break;
              }
              if (!File.Exists(strMenuImage))
              {
                if (IsPersonField(GetFieldFromViewLabel(item.DVDLabel)))
                {
                  strMenuImage = conf.DefaultCoverArtist.Length > 0 ? conf.DefaultCoverArtist : conf.DefaultCover;
                }
                else
                {
                  strMenuImage = conf.DefaultCoverViews.Length > 0 ? conf.DefaultCoverViews : conf.DefaultCover; //MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + "DefaultArtist.jpg";
                }
              }
              if (File.Exists(strMenuImage))
              {
                menuimage = strMenuImage;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("GetImageforMenu() - Error: " + ex.Message);
      }
      return menuimage;
    }

    private MFview.ViewRow GetCustomViewFromViewLabel(string viewlabel)
    {
      foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => viewRow.Label == viewlabel))
      {
        return viewRow;
      }
      LogMyFilms.Debug("GetCustomViewFromViewLabel() - no customvie found for viewlabel '" + viewlabel + "' - returning 'null'");
      return null;
    }

    //--------------------------------------------------------------------------------------------
    //  Change Sort Option for films, views or collections/groups
    //--------------------------------------------------------------------------------------------
    private void Change_Sort_Option_Menu()
    {
      if (conf.ViewContext == ViewContext.TmdbMovies)
      {
        #region sort for Online Movie Lists
        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;

        dlg.Reset();
        dlg.SetHeading(495); // Sort options
        dlg.Add(GUILocalizeStrings.Get(103)); // title / name
        dlg.Add(BaseMesFilms.TranslateColumn("Date")); // date
        dlg.Add(BaseMesFilms.TranslateColumn("Rating")); // rating
        dlg.Add(GUILocalizeStrings.Get(205)); // votes

        // set the focus to currently used sort method
        dlg.SelectedLabel = (int)_currentSortMethod;

        // show dialog and wait for result
        dlg.DoModal(GetID);
        if (dlg.SelectedId == -1) return;

        _currentSortMethod = (SortMethod)dlg.SelectedLabel;

        switch (_currentSortMethod)
        {
          case SortMethod.Name:
          // case SortMethod.Date:
            _currentSortDirection = " ASC";
            break;
          default:
            _currentSortDirection = " DESC";
            break;
        }
        BtnSrtBy.IsAscending = _currentSortDirection == " ASC";

        try
        {
          SetLabels();

          facadeFilms.Sort(this);

          UpdateButtonStates();

        }
        catch (Exception ex)
        {
          LogMyFilms.Error("Change_Sort_Option_Menu: Error sorting items - {0}", ex.ToString());
        }
        return;
        #endregion
      }

      if (conf.Boolselect) // view sort (grouping) // No change of normal filmlist sort method and no searchs in grouped views (views, e.g. country, year, etc.) - change count sorting instead ...
      {
        #region sorting by counts
        // BtnSrtBy.Label = (conf.BoolSortCountinViews) ? (GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(1079910)) : (GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(103)); // sort: count / sort: name

        var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(98)); // "Sort By ..."
        var choiceSort = new List<string>();
        dlg.Add(GUILocalizeStrings.Get(103)); // Name
        choiceSort.Add("Name");
        dlg.Add(GUILocalizeStrings.Get(1079910)); // Count
        choiceSort.Add("Count");
        dlg.SelectedLabel = conf.BoolSortCountinViews ? 1 : 0; // set position to active sort type
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;

        // new sort was selected ...
        conf.StrIndex = 0; // reset facadeposition to first line, as after sort position isn't valid anymore ...

        bool boolSortCountInViews = conf.BoolSortCountinViews;
        switch (choiceSort[dlg.SelectedLabel])
        {
          case "Name":
            boolSortCountInViews = false;
            break;
          case "Count":
            boolSortCountInViews = true;
            break;
          default:
            boolSortCountInViews = false;
            break;
        }
        dlg.DeInit();
        if (conf.BoolSortCountinViews == boolSortCountInViews) return;

        conf.BoolSortCountinViews = !conf.BoolSortCountinViews;
        new Thread(delegate()
        {
          {
            MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
            GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, string.Empty); // GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, "*", true, string.Empty);
            MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // removed, as it was causing trouble with menu focus control ...
            }
            return 0;
          }, 0, 0, null);
        })
        {
          Name = "Change_Sort_Option_Menu",
          IsBackground = true,
          Priority = ThreadPriority.Normal // Priority = ThreadPriority.AboveNormal
        }.Start();
        // GetSelectFromDivxThreaded(); // GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, "*", true, "");
        #endregion
      }
      else
      {
        #region film list sorting (normal and collections)
        var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading(conf.BoolCollection ? GUILocalizeStrings.Get(1079905) : GUILocalizeStrings.Get(1079902));
        var choiceSort = new List<string>();
        dlg.Add(GUILocalizeStrings.Get(103));// Name (=Title)
        choiceSort.Add("Title");
        dlg.Add(GUILocalizeStrings.Get(366));//Year
        choiceSort.Add("Year");
        dlg.Add(GUILocalizeStrings.Get(104));//Date
        choiceSort.Add("Date");
        dlg.Add(GUILocalizeStrings.Get(367));//Rating
        choiceSort.Add("Rating");
        dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
        choiceSort.Add("showall");

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;

        if (choiceSort[dlg.SelectedLabel] == "showall") // show all sort options, if selected ...
        {
          dlg.Reset();
          dlg.SetHeading(conf.BoolCollection ? GUILocalizeStrings.Get(1079905) : GUILocalizeStrings.Get(1079902)); // Sort by (Colletion) ... // Sort by ... 
          choiceSort.Clear();
          ArrayList displayItems = GetDisplayItems("sort");
          foreach (string[] displayItem in displayItems)
          {
            string entry = string.IsNullOrEmpty(displayItem[1]) ? displayItem[0] : displayItem[1];
            dlg.Add(entry);
            choiceSort.Add(displayItem[0]);
            LogMyFilms.Debug("Sort Menu - add '{0}' as '{1}'", displayItem[0], entry);
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;
        }

        // new sort was selected ...
        conf.StrIndex = 0; // reset facadeposition to first line, as after sort position isn't valid anymore ...
        string tmpStrSorta;
        string tmpStrSortSens;

        // set special handling for certain fields ...
        switch (choiceSort[dlg.SelectedLabel])
        {
          case "Title":
            //conf.StrSorta = conf.StrSTitle;
            //conf.StrSortSens = " ASC";
            tmpStrSorta = conf.StrSTitle;
            tmpStrSortSens = " ASC";
            break;
          case "Year":
            tmpStrSorta = "Year";
            tmpStrSortSens = " DESC";
            break;
          case "Date":
            tmpStrSorta = "Date"; // tmpStrSorta = "DateAdded";
            tmpStrSortSens = " DESC";
            break;
          case "Rating":
            tmpStrSorta = "Rating";
            tmpStrSortSens = " DESC";
            break;
          default:
            tmpStrSorta = choiceSort[dlg.SelectedLabel];
            tmpStrSortSens = " ASC";
            break;
        }
        dlg.DeInit();
        if (conf.BoolCollection)
        {
          conf.StrSortaInHierarchies = tmpStrSorta;
          conf.StrSortSensInHierarchies = tmpStrSortSens;
        }
        else
        {
          conf.StrSorta = tmpStrSorta;
          conf.StrSortSens = tmpStrSortSens;
        }
        if (conf.Boolselect) // cannot happen, as sorting for views only changes "counts" above ...
          GetSelectFromDivx(conf.StrTitle1 + " not like ''", conf.StrSorta, conf.StrSortSens, "*", true, ""); // GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, conf.Wstar, true, "");
        else
          GetFilmList();
        GUIWindowManager.Process();
        GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        #endregion
      }
    }

    public int Compare(GUIListItem item1, GUIListItem item2)
    {
      try
      {
        if (item1 == item2)
        {
          return 0;
        }
        if (item1 == null || item2 == null)
        {
          return -1;
        }
        if (item1.TVTag == null || item2.TVTag == null)
        {
          return -1;
        }

        OnlineMovie movie1 = item1.TVTag as OnlineMovie;
        OnlineMovie movie2 = item2.TVTag as OnlineMovie;

        DateTime dt1;
        DateTime dt2;

        if (movie1 == null || movie2 == null)
        {
          return -1;
        }
        switch (_currentSortMethod)
        {
          case SortMethod.Name:
            int iComp;
            if (_currentSortDirection == " ASC") // ToDo: replace with sortdirection variable
            {
              iComp = string.Compare(item1.Label, item2.Label, true);
              if (iComp == 0)
              {
                goto case SortMethod.Date;
              }
              else
              {
                return iComp;
              }
            }
            else
            {
              iComp = string.Compare(item2.Label, item1.Label, true);
              if (iComp == 0)
              {
                goto case SortMethod.Date;
              }
              else
              {
                return iComp;
              }
            }
          case SortMethod.Date:
            if (_currentSortDirection == " ASC")
            {
              if (movie1.MovieSearchResult.release_date == movie2.MovieSearchResult.release_date)
              {
                return 0;
              }

              // move films without date always to the end
              if (!DateTime.TryParse(movie1.MovieSearchResult.release_date, out dt1))
                return 1;
              if (!DateTime.TryParse(movie2.MovieSearchResult.release_date, out dt2))
                return 1;

              if (dt1 < dt2)
              {
                return -1;
              }
              return 1;
            }
            else
            {
              if (movie1.MovieSearchResult.release_date == movie2.MovieSearchResult.release_date)
              {
                return 0;
              }

              // move films without date always to the end
              if (!DateTime.TryParse(movie1.MovieSearchResult.release_date, out dt1))
                return 1;
              if (!DateTime.TryParse(movie2.MovieSearchResult.release_date, out dt2))
                return 1;

              if (dt1 > dt2)
              {
                return -1;
              }
              return 1;
            }
          case SortMethod.Rating:
            if (_currentSortDirection == " ASC")
            {
              if (movie1.MovieSearchResult.vote_average == movie2.MovieSearchResult.vote_average)
              {
                return 0;
              }
              if (movie1.MovieSearchResult.vote_average < movie2.MovieSearchResult.vote_average)
              {
                return -1;
              }
              return 1;
            }
            else
            {
              if (movie1.MovieSearchResult.vote_average == movie2.MovieSearchResult.vote_average)
              {
                return 0;
              }
              if (movie1.MovieSearchResult.vote_average > movie2.MovieSearchResult.vote_average)
              {
                return -1;
              }
              return 1;
            }
          case SortMethod.Votes:
            if (_currentSortDirection == " ASC")
            {
              if (movie1.MovieSearchResult.vote_count == movie2.MovieSearchResult.vote_count)
              {
                return 0;
              }
              if (movie1.MovieSearchResult.vote_count < movie2.MovieSearchResult.vote_count)
              {
                return -1;
              }
              return 1;
            }
            else
            {
              if (movie1.MovieSearchResult.vote_count == movie2.MovieSearchResult.vote_count)
              {
                return 0;
              }
              if (movie1.MovieSearchResult.vote_count > movie2.MovieSearchResult.vote_count)
              {
                return -1;
              }
              return 1;
            }
        }
        return 0;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error(ex, "MyFilms Sort(): Error comparing GUIListitems - {0}", ex.Message);
        return 0;
      }
    }

    //--------------------------------------------------------------------------------------------
    //  Select main Options
    //--------------------------------------------------------------------------------------------
    private void Change_Option()
    {
      var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      List<string> choiceView = new List<string>();
      dlg1.Reset();
      dlg1.SetHeading(GUILocalizeStrings.Get(10798701)); // Options ...
      if (Configuration.NbConfig > 1)
      {
        dlg1.Add(GUILocalizeStrings.Get(6022));   // Change Config 
        //dlg1.Add(GUILocalizeStrings.Get(6029) + " " + GUILocalizeStrings.Get(6022));   // Change Config 
        choiceView.Add("config");
        // choiceView.Add(MenuAction.Config);
      }

      if (conf.StrCheckWOLenable) // Show current NAS server status
      {
        dlg1.Add(GUILocalizeStrings.Get(10798727)); // Show NAS Server Status
        choiceView.Add("nasstatus");
      }

      // Add Submenu for Global Settings 
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798689)));
      choiceView.Add("globaloptions");

      // Add Submenu for Global Updates
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798690)));
      choiceView.Add("globalupdates");

      // Add Submenu for useritemx mapping
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798771)));
      choiceView.Add("globalmappings");

      // Add Submenu for Wiki Online Help
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798699)));
      choiceView.Add("globalwikihelp");

      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798700))); // About ...
      choiceView.Add("about");

      dlg1.DoModal(GetID);

      if (dlg1.SelectedLabel == -1)
      {
        return;
      }
      Change_Menu_Action(choiceView[dlg1.SelectedLabel].ToLower());
    }

    //--------------------------------------------------------------------------------------------
    //  Select main global filters
    //--------------------------------------------------------------------------------------------
    private void Change_Global_Filters()
    {
      Context_Menu = true; // make sure, it's set, as otherwise we fall back to first menu level ...
      var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      dlg1.Reset();
      dlg1.SetHeading(GUILocalizeStrings.Get(10798714)); // Global Filters ...
      var choiceViewGlobalOptions = new List<string>();

      // Change global Unwatchedfilteroption
      // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSuppressPlayStopUpdateUserField))// Make it conditoional, so only displayed, if options enabled in setup !
      if (conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
      if (!conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
      choiceViewGlobalOptions.Add("globalunwatchedfilter");


      // Change global MovieFilter (Only Movies with media files reachable 
      if (InitialIsOnlineScan) // (requires at least initial scan! - //  || MyFilms.conf.ScanMediaOnStart -> or enabled startup option)
      {
        if (GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798628)));
        if (!GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798629)));
        choiceViewGlobalOptions.Add("filterdbisonline");
      }

      // Change global MovieFilter (Only Movies with Trailer)
      if (Helper.FieldIsSet(conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
      {
        if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
        if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
        choiceViewGlobalOptions.Add("filterdbtrailer");
      }

      // Change global MovieFilter (Only Movies with highRating)
      if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
      if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
      choiceViewGlobalOptions.Add("filterdbrating");

      // Change Value for global MovieFilter (Only Movies with highRating)
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), conf.StrAntFilterMinRating));
      choiceViewGlobalOptions.Add("filterdbsetrating");

      //if (MyFilms.conf.BoolShowEmptyValuesInViews) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798628))); // show empty values in views
      //if (!MyFilms.conf.BoolShowEmptyValuesInViews) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798629)));
      //choiceViewGlobalOptions.Add("showemptyvaluesinviews");

      //// Select or disable a custom filter (e.g. category value, year value, etc.)
      //if (GlobalFilterStringCustomFilter.Length > 0) dlg1.Add(string.Format(GUILocalizeStrings.Get(0), GUILocalizeStrings.Get(0))); // disable user filter 'field-filter'
      //if (GlobalFilterStringCustomFilter.Length == 0) dlg1.Add(GUILocalizeStrings.Get(0)); // select user filter
      //choiceViewGlobalOptions.Add("filterdbcustomfilter");

      // Change global Movie type Filter (all, 2d, 3d)
      if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Global Filter 3D movies"))
      {
        dlg1.Add(string.Format(GUILocalizeStrings.Get(10798972), GlobalFilterMovieFormat3D));
        choiceViewGlobalOptions.Add("filter3d");
      }

      dlg1.DoModal(GetID);
      if (dlg1.SelectedLabel == -1) return;
      LogMyFilms.Debug("Call change_global_filters menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel] + "'");
      Change_Menu_Action(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
    }

    private void Change_Search_Options()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return; // true;

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(137) + " ..."); // Search ...
      var choiceSearch = new List<string>();

      dlg.Add(GUILocalizeStrings.Get(10798615)); //Search global movies by property 
      choiceSearch.Add("globalproperty");

      dlg.Add(GUILocalizeStrings.Get(10798608)); //Search global persons by role
      choiceSearch.Add("globalpersons");

      if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Global Random Movie Search"))
      {
        //Guzzi: RandomMovie Search added
        dlg.Add(GUILocalizeStrings.Get(10798621)); //Search global movies by randomsearch (singlesearch, areasearch)
        choiceSearch.Add("randomsearch");
      }

      if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Global Search Movies by Areas"))
      {
        dlg.Add(GUILocalizeStrings.Get(10798645)); //Search global movies by areas
        choiceSearch.Add("globalareas");
      }

      dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(369)); //Title
      choiceSearch.Add("title");

      dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(344)); //Actors
      choiceSearch.Add("actors");

      if (facadeFilms.SelectedListItemIndex > -1 && !facadeFilms.SelectedListItem.IsFolder)
      {
        dlg.Add(GUILocalizeStrings.Get(1079866)); //Search related movies by persons
        choiceSearch.Add("analogyperson");

        dlg.Add(GUILocalizeStrings.Get(10798614)); //Search related movies by property
        choiceSearch.Add("analogyproperty");
      }

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return; // true;

      string menuselection = choiceSearch[dlg.SelectedLabel];

      switch (menuselection)
      {
        case "analogyperson":
          SearchRelatedMoviesbyPersons(facadeFilms.SelectedListItem.ItemId, false);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          dlg.DeInit();
          return;
        case "analogyproperty":
          SearchRelatedMoviesbyProperties(facadeFilms.SelectedListItem.ItemId, false);
          dlg.DeInit();
          return;
        case "randomsearch":
          SearchMoviesbyRandomWithTrailerOriginalVersion(false);
          dlg.DeInit();
          return;
        case "globalareas":
          SearchMoviesbyAreas(false);
          dlg.DeInit();
          return;
        case "globalproperty":
          SearchMoviesbyProperties(false, string.Empty, string.Empty);
          dlg.DeInit();
          return;
        case "globalpersons":
          SearchMoviesbyPersons(false, string.Empty, string.Empty);
          dlg.DeInit();
          return;

        default:

          #region show last searches or new dialog
          dlg.Reset();
          choiceSearch.Clear();
          switch (menuselection)
          {
            case "title":
              dlg.SetHeading(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(369) + " ..."); //Title
              break;
            case "actors":
              dlg.SetHeading(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(344) + " ..."); //Actors
              break;
            default:
              dlg.SetHeading(GUILocalizeStrings.Get(137) + " ..."); // Search ...
              break;
          }

          dlg.Add("<" + GUILocalizeStrings.Get(10798633) + ">"); // New Search
          choiceSearch.Add("");

          if (SearchHistory.Count > 0)
          {
            for (int i = SearchHistory.Count; i > 0; i--)
            {
              dlg.Add(SearchHistory[i - 1]);
              choiceSearch.Add(SearchHistory[i - 1]);
            }
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
          {
            Change_Search_Options();
            return;
          }
          #endregion

          var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)Window.WINDOW_VIRTUAL_KEYBOARD);
          if (null == keyboard) return; //if (null == keyboard) return true;
          keyboard.Reset();
          keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
          keyboard.Text = choiceSearch[dlg.SelectedLabel];
          keyboard.DoModal(GetID);

          if (!keyboard.IsConfirmed || keyboard.Text.Length == 0)
          {
            dlg.DeInit();
            Change_Search_Options(); // recall search menu
          }
          else
          {
            #region do search as requested
            UpdateRecentSearch(keyboard.Text, MaxSearchHistoryLength);
            switch (menuselection)
            {
              case "title":
                if (control_searchText(keyboard.Text))
                {
                  SaveListState(false);
                  conf.Boolselect = false; // switch to film list view
                  conf.ViewContext = ViewContext.Movie;
                  conf.StrSelect = conf.StrTitle1 + " like '*" + keyboard.Text + "*'";
                  conf.StrTxtSelect = GUILocalizeStrings.Get(369) + " *" + keyboard.Text + @"*"; // selection ...
                  conf.StrTitleSelect = "";
                  SetLabelView("search"); // show "search"
                  conf.CurrentView = "MovieNameSearch";
                  GetFilmList();
                }
                else return; // false;
                break;
              case "actors":
                if (control_searchText(keyboard.Text))
                {
                  SaveListState(false);
                  conf.Boolselect = true;
                  conf.CurrentView = "ActorNameSearch";
                  conf.StrTitleSelect = "";
                  conf.StrViewSelect = "";
                  conf.IndexedChars = 0;
                  conf.Boolindexed = false;
                  conf.BoolSkipViewState = false;

                  filmcover.Filename = "";
                  viewcover.Filename = "";
                  personcover.Filename = "";
                  groupcover.Filename = "";

                  conf.ViewContext = ViewContext.Person;
                  conf.WStrSort = "Actors";
                  conf.WStrSortSens = " ASC";
                  conf.Wselectedlabel = "";
                  conf.StrPersons = ""; // conf.StrPersons = keyboard.Text;
                  conf.StrTxtSelect = GUILocalizeStrings.Get(344) + " *" + keyboard.Text + @"*"; // selection ...
                  conf.StrTitleSelect = "";
                  SetLabelSelect(conf.StrTxtSelect);
                  SetLabelView("search"); // show "search"
                  conf.StrSelect = "Actors like '*" + keyboard.Text + "*'";
                  GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");
                }
                else return; // false;
                break;
            }
            #endregion
          }
          break;
      }
    }

    public static string GetDayRange(int age)
    {
      if (age < 0) return "7" + GUILocalizeStrings.Get(10798960); //unknown
      if (age == 0) return "0" + GUILocalizeStrings.Get(10798961); //today
      if (age <= 1) return "1" + GUILocalizeStrings.Get(10798962); //yesterday
      if (age <= 7) return "2" + GUILocalizeStrings.Get(10798963); //last week
      if (age <= 30) return "3" + GUILocalizeStrings.Get(10798964); //last month
      if (age <= 90) return "4" + GUILocalizeStrings.Get(10798965); //last 3 month
      if (age <= 365) return "5" + GUILocalizeStrings.Get(10798966); //last year
      if (age <= 3 * 365) return "6" + GUILocalizeStrings.Get(10798967); //last 3 years
      return "7" + GUILocalizeStrings.Get(10798968); //older than 3 years
    }

    private static int GetValueRange(int iValue)
    {
      if (iValue == 0) return 0;
      if (iValue <= 1) return 1;
      if (iValue <= 5) return 5;
      if (iValue <= 10) return 10;
      if (iValue <= 20) return 20;
      if (iValue <= 50) return 50;
      if (iValue <= 100) return 100;
      if (iValue <= 200) return 200;
      if (iValue <= 500) return 500;
      if (iValue <= 1000) return 1000;
      if (iValue <= 2000) return 2000;
      if (iValue <= 5000) return 5000;
      if (iValue <= 10000) return 10000;
      if (iValue <= 20000) return 20000;
      return iValue;
    }

    public static Type GetColumnType(string fieldname)
    {
      using (var ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName == fieldname))
        {
          return dc.DataType;
        }
      }
      return null;
    }

    private static bool IsMyFilmsDbColumn(string fieldname)
    {
      if (string.IsNullOrEmpty(fieldname)) return false;

      using (AntMovieCatalog ds = new AntMovieCatalog())
      {
        return ds.Movie.Columns.Cast<DataColumn>().Any(dc => dc.ColumnName == fieldname);
      }
    }

    private string Equalexpression(string columnname, string comparevalue)
    {
      string expression;
      Type columnType = GetColumnType(columnname);
      if (columnType == typeof(decimal))
      {
        expression = columnname + " = " + Convert.ToDecimal(comparevalue);
      }
      else if (columnType == typeof(int))
      {
        expression = columnname + " = " + Convert.ToInt32(comparevalue);
      }
      else if (columnType == typeof(DateTime))
      {
        expression = columnname + " = " + Convert.ToDateTime(comparevalue);
      }
      else
      {
        expression = columnname + " like '*" + StringExtensions.EscapeLikeValue(comparevalue) + "*'";
      }
      LogMyFilms.Debug("Equalexpression() - called with columnname = '" + columnname + "', comparevalue = '" + comparevalue + "', resulting expression = '" + expression + "'");
      return expression;
    }

    public static FieldType GetFieldType(string fieldname)
    {
      if (IsDateField(fieldname)) return FieldType.Date;
      if (IsAlphaNumericalField(fieldname)) return FieldType.AlphaNumeric;
      if (IsDecimalField(fieldname)) return FieldType.Decimal;
      if (IsPersonField(fieldname)) return FieldType.Person;
      if (IsTitleField(fieldname)) return FieldType.Title;
      return FieldType.Default;
    }

    private static bool IsPersonField(FieldType fieldtype)
    {
      return fieldtype == FieldType.Person;
    }
    private static bool IsPersonField(string fieldname)  // "Persons", "Actors", "Producer", "Director", "Writer", "Borrower", "Composer"
    {
      if (string.Compare(fieldname, "Persons", true) == 0) return true;
      if (string.Compare(fieldname, "Actors", true) == 0) return true;
      if (string.Compare(fieldname, "Producer", true) == 0) return true;
      if (string.Compare(fieldname, "Director", true) == 0) return true;
      if (string.Compare(fieldname, "Writer", true) == 0) return true;
      if (string.Compare(fieldname, "Borrower", true) == 0) return true;
      if (string.Compare(fieldname, "Composer", true) == 0) return true;
      return false;
    }

    private static bool IsCategoryYearCountryField(string fieldname)  // "Category", "Year", "Country"
    {
      if (string.Compare(fieldname, "Category", true) == 0) return true;
      if (string.Compare(fieldname, "Year", true) == 0) return true;
      if (string.Compare(fieldname, "Country", true) == 0) return true;
      return false;
    }

    private static bool IsPersonsField(string fieldname)
    {
      return PersonTypes.Any(personField => string.Compare(fieldname, personField, true) == 0); // "Persons", "Actors", "Producer", "Director", "Writer", "Borrower", "Composer"
    }

    private static bool IsAlphaNumericalField(FieldType fieldtype)
    {
      return fieldtype == FieldType.AlphaNumeric;
    }
    private static bool IsSplittableField(string fieldname)
    {
      if (string.Compare(fieldname, "OriginalTitle", true) == 0) return false;
      if (string.Compare(fieldname, "TranslatedTitle", true) == 0) return false;
      if (string.Compare(fieldname, "FormattedTitle", true) == 0) return false;
      if (string.Compare(fieldname, "VirtualPathTitle", true) == 0) return false;
      if (string.Compare(fieldname, "Description", true) == 0) return false;
      if (string.Compare(fieldname, "Comment", true) == 0) return false;
      if (string.Compare(fieldname, "Source", true) == 0) return false;
      if (string.Compare(fieldname, "SourceTrailer", true) == 0) return false;
      if (string.Compare(fieldname, "Collection", true) == 0) return false;
      if (string.Compare(fieldname, "CollectionImage", true) == 0) return false;
      if (string.Compare(fieldname, "CollectionDetails", true) == 0) return false;
      if (string.Compare(fieldname, "Date", true) == 0) return false;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return false;
      if (string.Compare(fieldname, "DateReleased", true) == 0) return false;
      if (string.Compare(fieldname, "DateAdded", true) == 0) return false;
      if (string.Compare(fieldname, "Year", true) == 0) return false;
      if (string.Compare(fieldname, "RecentlyAdded", true) == 0) return false;
      if (string.Compare(fieldname, "Budget", true) == 0) return false;
      if (string.Compare(fieldname, "Votes", true) == 0) return false;
      if (IsDecimalField(fieldname)) return false;
      if (IsAlphaNumericalField(fieldname)) return false;
      return true;
    }

    private static bool IsAlphaNumericalField(string fieldname)
    {
      // if (string.Compare(fieldname, "Year", true) == 0) return true;
      if (string.Compare(fieldname, "Length", true) == 0) return true;
      if (string.Compare(fieldname, "Size", true) == 0) return true;
      if (string.Compare(fieldname, "Disks", true) == 0) return true;
      if (string.Compare(fieldname, "AudioChannelCount", true) == 0) return true;
      if (string.Compare(fieldname, "AudioBitrate", true) == 0) return true;
      if (string.Compare(fieldname, "VideoBitrate", true) == 0) return true;
      if (string.Compare(fieldname, "Aspectratio", true) == 0) return true;
      if (string.Compare(fieldname, "Framerate", true) == 0) return true;
      if (string.Compare(fieldname, "Resolution", true) == 0) return true;
      if (string.Compare(fieldname, "Number", true) == 0) return true;
      if (string.Compare(fieldname, "LastPosition", true) == 0) return true;
      if (string.Compare(fieldname, "Votes", true) == 0) return true;
      //if (string.Compare(fieldname, "CustomField1", true) == 0) return true;
      //if (string.Compare(fieldname, "CustomField2", true) == 0) return true;
      //if (string.Compare(fieldname, "CustomField3", true) == 0) return true;
      // if (string.Compare(fieldname, "Languages", true) == 0) return true; // removed, as it was not splitted otherwise ...
      return false;
    }

    private static bool IsDecimalField(string fieldname)
    {
      if (string.Compare(fieldname, "AgeAdded", true) == 0) return true;
      if (string.Compare(fieldname, "Rating", true) == 0) return true;
      if (string.Compare(fieldname, "RatingUser", true) == 0) return true;
      return false;
    }

    private static bool IsTitleField(FieldType fieldtype)
    {
      return fieldtype == FieldType.Title;
    }
    private static bool IsTitleField(string fieldname) // AlternativeTitles NOT added, is only used internally
    {
      if (string.Compare(fieldname, "OriginalTitle", true) == 0) return true;
      if (string.Compare(fieldname, "TranslatedTitle", true) == 0) return true;
      if (string.Compare(fieldname, "FormattedTitle", true) == 0) return true;
      // if (string.Compare(fieldname, "VirtualPathTitle", true) == 0) return true;
      return false;
    }

    private static bool IsDateField(FieldType fieldtype)
    {
      return fieldtype == FieldType.Date;
    }
    private static bool IsDateField(string fieldname)
    {
      if (string.Compare(fieldname, "Date", true) == 0) return true;
      if (string.Compare(fieldname, "DateAdded", true) == 0) return true;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return true;
      if (string.Compare(fieldname, "DateReleased", true) == 0) return true;
      return false;
    }

    private static bool IsDateTimeField(string fieldname)
    {
      if (string.Compare(fieldname, "DateAdded", true) == 0) return true;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return true;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return true;
      if (string.Compare(fieldname, "DateReleased", true) == 0) return true;
      return false;
    }


    static readonly Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]", RegexOptions.Compiled);
    static readonly Regex oRegexReplace = new Regex("[,;]", RegexOptions.Compiled);

    static MyFilms()
    {
      IsNetworkAvailable = true;
      IsResumeFromStandby = false;
    }

    public static List<string> Search_String(string champselect, bool reverseNames = false)
    {
      // Match theMatch = Regex.Match(source, pattern, RegexOptions.Compiled);
      // MatchCollection theMatches = Regex.Matches(source, pattern, RegexOptions.Compiled);
      // Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]", RegexOptions.Compiled);
      // MatchCollection oMatches = Regex.Matches(champselect, "\\([^\\)]*?[,;].*?[\\(\\)]", RegexOptions.Compiled);

      MatchCollection oMatches = oRegex.Matches(champselect);
      champselect = oMatches.Cast<Match>().Aggregate(champselect, (current, oMatch) => current.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty)));
      List<string> wtab = new List<string>();

      // char[] SepAsChars; string tS = String.Empty; for (int i = 0; i <= Sep.Length; i++) tS += Sep[i]; SepAsChars = tS.ToCharArray(); string[] arSplit = champselect.Split(SepAsChars, StringSplitOptions.RemoveEmptyEntries);
      string[] arSplit = champselect.Split(conf.ListSeparator, StringSplitOptions.RemoveEmptyEntries);
      foreach (string substring in arSplit)
      {
        if (substring.Length > 0)
        {
          // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary and was VERY slow!
          string wzone = substring.Replace("  ", " ").Trim();
          string[] roleSep = conf.RoleSeparator;
          foreach (string roleSeparator in roleSep.Where(roleSeparator => roleSeparator.Length > 0))
          {
            int wzoneIndexPosition = wzone.IndexOf(roleSeparator, StringComparison.OrdinalIgnoreCase);

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

          if (reverseNames && wzone.Contains(" ")) // Reverse Names "Bruce Willis" -> "Willis, Bruce"
          {
            wzone = wzone.Substring(wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) + ", " + wzone.Substring(0, wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase));
          }

          if (wzone.Length > 0) wtab.Add(wzone);
        }
      }
      return wtab;
    }

    public static IEnumerable<DbPersonInfo> Search_String_Persons(string champselect, bool reverseNames)
    {

      MatchCollection oMatches = oRegex.Matches(champselect);
      champselect = oMatches.Cast<Match>().Aggregate(champselect, (current, oMatch) => current.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty)));
      List<DbPersonInfo> wtab = new List<DbPersonInfo>();

      string[] sep = conf.ListSeparator;
      string[] roleSep = conf.RoleSeparator;
      //char[] SepAsChars; string tS = String.Empty; for (int i = 0; i <= Sep.Length; i++) tS += Sep[i]; SepAsChars = tS.ToCharArray(); string[] arSplit = champselect.Split(SepAsChars, StringSplitOptions.RemoveEmptyEntries);
      string[] arSplit = champselect.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      string wzoneRole = string.Empty;
      foreach (string substring in arSplit.Where(substring => substring.Length > 0))
      {
        // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary and was VERY slow!
        string wzone = substring.Replace("  ", " ").Trim();
        for (int i = 0; i <= 4; i++)
        {
          if (roleSep[i].Length > 0)
          {
            int wzoneIndexPosition = wzone.IndexOf(roleSep[i], StringComparison.OrdinalIgnoreCase);

            if (wzoneIndexPosition == wzone.Length - 1)
            {
              wzone = string.Empty;
              wzoneRole = string.Empty;
              break;
            }
            if (wzoneIndexPosition > 1 && wzoneIndexPosition < wzone.Length)
            {
              wzoneRole = wzone.Substring(wzoneIndexPosition + 1).Trim().Trim('(').Trim(')').Replace("as", "").Replace("  ", " ").Trim();
              wzone = wzone.Substring(0, wzoneIndexPosition).Trim();
            }
          }
        }
        if (reverseNames && wzone.Contains(" ")) // Reverse Names "Bruce Willis" -> "Willis, Bruce"
        {
          wzone = wzone.Substring(wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) + ", " + wzone.Substring(0, wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase));
        }

        if (wzone.Length > 0)
        {
          DbPersonInfo person = new DbPersonInfo { Name = wzone, Job = wzoneRole };
          wtab.Add(person);
        }
      }
      return wtab;
    }

    private static IEnumerable<string> SubItemGrabbing(string champselect)
    {
      Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
      {
        Regex oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
      }
      List<string> wtab = new List<string>();

      string[] sep = conf.ListSeparator;
      string[] arSplit = champselect.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      foreach (string substring in arSplit.Where(substring => substring.Length > 0))
      {
        string wzone = HTMLParser.removeHtml(substring.Trim());
        foreach (string roleSeparator in conf.RoleSeparator.Where(roleSeparator => roleSeparator.Length > 0))
        {
          if (wzone.Trim().IndexOf(roleSeparator) == wzone.Trim().Length - 1)
          {
            wzone = string.Empty;
            break;
          }
          if (wzone.Trim().IndexOf(roleSeparator) > 1 && wzone.Trim().IndexOf(roleSeparator) < wzone.Trim().Length)
          {
            wzone = wzone.Substring(0, wzone.IndexOf(roleSeparator)).Trim();
          }
        }
        if (wzone.Length > 0) wtab.Add(wzone.Trim());
      }
      return wtab.AsEnumerable();
    }

    public static IEnumerable<string> SubTitleGrabbing(string champselect)
    {
      Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
      {
        Regex oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
      }

      List<string> wtab = new List<string>();

      int wi;
      string[] sep = { " - ", ":" }; //Only Dash as Separator for Movietitles !!!
      //string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ","- "," -"," AT "};
      string[] arSplit = champselect.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        if (arSplit[wi].Length > 0)
        {
          string wzone = HTMLParser.removeHtml(arSplit[wi].Trim());
          for (int i = 0; i <= 4; i++)
          {
            if (conf.RoleSeparator[i].Length > 0)
            {
              if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) == wzone.Trim().Length - 1)
              {
                wzone = string.Empty;
                break;
              }
              if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) > 1 && wzone.Trim().IndexOf(conf.RoleSeparator[i]) < wzone.Trim().Length)
              {
                wzone = wzone.Substring(0, wzone.IndexOf(conf.RoleSeparator[i])).Trim();
              }
            }
          }
          if (wzone.Length > 0)
            wtab.Add(wzone.Trim());
        }
      }
      return wtab.AsEnumerable();
    }

    public static IEnumerable<string> SubWordGrabbing(string champselect, int minchars, bool filter)
    {
      LogMyFilms.Debug("(SubWordGrabbing): InputString: '" + champselect + "'");
      Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
      {
        Regex oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
        LogMyFilms.Debug("(SubWordGrabbing): RegExReplace: '" + champselect + "'");
      }

      string[] cleanerList = new[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ", "- ", " -", " AT ", "in " };
      champselect = cleanerList.Where(s => (s.Length > 0) && filter).Aggregate(champselect, (current, s) => current.Replace(s, " "));

      List<string> wtab = new List<string>();

      //string[] Sep = conf.ListSeparator;
      string[] sep = new[] { " ", ",", ";", "|", "/", "(", ")", ".", @"\", ":" };
      string[] arSplit = champselect.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in arSplit.Where(s => s.Length > 0))
      {
        string wzone = HTMLParser.removeHtml(s.Trim());
        LogMyFilms.Debug("(SubWordGrabbing): wzone: '" + wzone + "'");
        if (wzone.Length >= minchars)//Only words with minimum 4 letters!
        {
          wtab.Add(wzone.Trim());
          LogMyFilms.Debug("(SubWordGrabbing): AddWordToList: '" + wzone.Trim() + "'");
        }
      }
      return wtab.AsEnumerable();
    }

    private bool LoadWatchedFlagPossible()
    {
      // Available (Files are Local) Images
      string sWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlaywatched.png";
      string sUnWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayunwatched.png";

      // Not Available (Files are not Local) Images
      string sWatchedNaFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAwatched.png";
      string sUnWatchedNaFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAunwatched.png";

      // return if images do exist
      return File.Exists(sWatchedFilename) && File.Exists(sUnWatchedFilename) && File.Exists(sWatchedNaFilename) && File.Exists(sUnWatchedNaFilename);
    }

    private bool LoadWatchedFlag(GUIListItem item, bool bWatched, bool bAvailable)
    {
      // Available (Files are Local) Images
      string sWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlaywatched.png";
      string sUnWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayunwatched.png";

      // Not Available (Files are not Local) Images
      string sWatchedNaFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAwatched.png";
      string sUnWatchedNaFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAunwatched.png";

      item.PinImage = bWatched
                        ? (!bAvailable ? sWatchedNaFilename : sWatchedFilename)
                        : (!bAvailable ? sUnWatchedNaFilename : sUnWatchedFilename);
      return true;
    }

    private static bool LoadIndexSkinThumbs(GUIListItem item)
    {
      if (!File.Exists(GUIGraphicsContext.Skin + @"\Media\alpha\a.png")) return false; // return, if skin does not support index thumbs

      //item.ThumbnailImage = GUIGraphicsContext.Skin + @"\Media\alpha\" + "Logo leer.png";
      //return true;

      string strStartLetter = item.Label != EmptyFacadeValue && item.Label.Length > 0 ? item.Label.Substring(0, 1) : "Logo leer";
      if (strStartLetter.IsNumerical()) strStartLetter = "#";
      string indexThumb = GUIGraphicsContext.Skin + @"\Media\alpha\" + strStartLetter + ".png";
      if (!File.Exists(indexThumb)) indexThumb = GUIGraphicsContext.Skin + @"\Media\alpha\" + "Logo leer" + ".png";
      item.ThumbnailImage = indexThumb;
      // disable the following two, if you don't want index thumbs in list view
      item.IconImage = indexThumb;
      item.IconImageBig = indexThumb;
      return true;
    }

    /// <summary>Performs 'GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, "*", true, string.Empty)' in background thread</summary>
    private void GetSelectFromDivxThreaded()
    {
      new Thread(delegate()
      {
        {
          MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
          GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
          MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // removed, as it was causing trouble with menu focus control ...
            }
            return 0;
          }, 0, 0, null);
      })
        {
          Name = "MyFilmsgetSelectFromDivx",
          IsBackground = true,
          Priority = ThreadPriority.Normal // Priority = ThreadPriority.AboveNormal
        }.Start();

    }

    private void ClearFacade()
    {
      try
      {
        if (facadeFilms.ListLayout != null) facadeFilms.ListLayout.Clear();
        if (facadeFilms.AlbumListLayout != null) facadeFilms.AlbumListLayout.Clear();
        if (facadeFilms.ThumbnailLayout != null) facadeFilms.ThumbnailLayout.Clear();
        if (facadeFilms.FilmstripLayout != null) facadeFilms.FilmstripLayout.Clear();
        if (facadeFilms.CoverFlowLayout != null) facadeFilms.CoverFlowLayout.Clear();
        if (facadeFilms != null) facadeFilms.Clear();
        if (facadeFilms != null) facadeFilms.Focus = true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Error preparing Facade... " + ex.Message + ex.StackTrace);
      }
    }

    /// <summary>Loads online movie lists from TMDB and links them to local DB entries</summary>
    /// <param name="tmdBfunction">Select online movie list from TMDB to be loaded - e.g. latest movies, person movies, etc. or the movie of the person name given</param>
    /// 
    private void GetSelectFromTmdb(string tmdBfunction)
    {
      conf.WStrSort = conf.StrTitle1; // needed for forwardnavigation to local movies
      conf.ViewContext = ViewContext.TmdbMovies;

      LogMyFilms.Debug("(GetSelectFromTmdb) - CallerFeature           : '" + tmdBfunction + "'");
      LogMyFilms.Debug("(GetSelectFromTmdb) - conf.StrDfltSelect      : '" + conf.StrDfltSelect + "'");

      #region Setup variables and configure sorting and buttons
      Prev_ItemID = -1;
      Prev_Label = string.Empty;

      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the start ....
      GUIPropertyManager.SetProperty("#itemcount", "0");

      bool allowSortchanges = !(tmdBfunction != GUILocalizeStrings.Get(10798826) && tmdBfunction != GUILocalizeStrings.Get(10798827) &&
                                tmdBfunction != GUILocalizeStrings.Get(10798828) && tmdBfunction != GUILocalizeStrings.Get(10798829) && 
                                tmdBfunction != GUILocalizeStrings.Get(10798824) && !string.IsNullOrEmpty(tmdBfunction));

      BtnSrtBy.IsEnabled = allowSortchanges;
      _currentSortMethod = SortMethod.Date;
      _currentSortDirection = " DESC";
      BtnSrtBy.IsAscending = false; // (conf.StrSortSens == " ASC");
      UpdateButtonStates();

      conf.Boolselect = false;
      conf.Wselectedlabel = "";
      conf.StrTxtSelect = tmdBfunction;
      SetLabelSelect(tmdBfunction);
      GUIControl.ClearControl(GetID, facadeFilms.GetID);
      // ClearFacade(); // facadeFilms.Clear();
      Change_Layout_Action(conf.StrLayOut); // Change_Layout_Action(MyFilms.conf.WStrLayOut);
      #endregion

      #region get movie lists from TMDB ...
      watch.Reset(); watch.Start();
      bool ispersonmovie = false;

      GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
      {
        if (tmdBfunction == GUILocalizeStrings.Get(10798826))  // Popular Movies
          return PopularMovies;
        if (tmdBfunction == GUILocalizeStrings.Get(10798827))  // Now Playing
          return NowPlayingMovies;
        if (tmdBfunction == GUILocalizeStrings.Get(10798828))  // Top Rated
          return TopRatedMovies;
        if (tmdBfunction == GUILocalizeStrings.Get(10798829)) // Upcoming Movies
          return UpcomingMovies;
        if (tmdBfunction == GUILocalizeStrings.Get(10798824)) // IMDB Top250 movies
          return ImdbTop250Movies;

        ispersonmovie = true;
      return !string.IsNullOrEmpty(tmdBfunction) ? GetPersonMovies(tmdBfunction, true) : null;
      },
      delegate(bool success, object result)
      {
        if (!success) DoBack();
        else
        {
          IEnumerable<TmdbMovieSearchResult> movies = result as IEnumerable<TmdbMovieSearchResult>;

          // clear facade
          GUIControl.ClearControl(GetID, facadeFilms.GetID);

          IEnumerable<TmdbMovieSearchResult> tmdbMovieSearchResults = movies as IList<TmdbMovieSearchResult> ?? movies.ToList();
          if (movies == null || !tmdbMovieSearchResults.Any())
          {
            GUIUtils.ShowNotifyDialog("No Movie found !");
            DoBack(); return;
          }

          #region Populate the facade ...
          //string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
          //Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"

          int itemId = 0;
          foreach (TmdbMovieSearchResult movie in tmdbMovieSearchResults)
          {
            #region populate facade item
            GUIListItem item = new GUIListItem();
            OnlineMovie ovMovie = new OnlineMovie { MovieSearchResult = movie };

            // AntMovieCatalog.MovieRow AntMovie = new AntMovieCatalog.MovieDataTable().NewMovieRow();
            // string[] split = movie.original_title.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            // bool ispersoninfo = (split.Length > 1);


            //#region search for TMDB id if it is missing ...
            //if (ovMovie.MovieSearchResult.id == -1) // we have stored IMDBid in imdb_id, if id == -1
            //{
            //  TmdbMovie tmdbmovie = api.GetMovieByIMDB(ovMovie.MovieSearchResult.original_title);
            //  ovMovie.MovieSearchResult.id = tmdbmovie.id;
            //  ovMovie.MovieSearchResult.backdrop_path = tmdbmovie.backdrop_path;
            //  ovMovie.MovieSearchResult.poster_path = tmdbmovie.poster_path;
            //  ovMovie.MovieSearchResult.original_title = tmdbmovie.original_title;
            //  ovMovie.MovieSearchResult.release_date = tmdbmovie.release_date;
            //}
            //#endregion

            item.ItemId = Int32.MaxValue - itemId;
            item.TVTag = ovMovie;
            item.Label = movie.title.Length > 0 ? movie.title : movie.original_title;
            try
            {
              item.Label2 = DateTime.Parse(movie.release_date).ToShortDateString();
            }
            catch (Exception)
            {
              item.Label2 = movie.release_date;
            }
            item.Label3 = !ispersonmovie ? movie.vote_average.ToString("0.0") + " (" + movie.vote_count + ")" : movie.original_title;
            if (ispersonmovie) item.Label2 = movie.original_title + " - " + item.Label2;
            // if (ispersonmovie) item.Label2 = split[1] + " - " + item.Label2;
            item.Path = "";
            item.IsRemote = true;
            item.IsFolder = false;
            item.IconImage = conf.DefaultCover;
            item.IconImageBig = conf.DefaultCover;
            item.ThumbnailImage = conf.DefaultCover;
            // Utils.SetDefaultIcons(item);
            //item.Item = movie.Images;
            //item.IsPlayed = movie.Watched;
            item.OnItemSelected += item_OnItemSelected;
            facadeFilms.Add(item);
            itemId++;
            #endregion
          }
          #endregion

          #region do initial sorting
          if (tmdBfunction == GUILocalizeStrings.Get(10798826)) // Popular Movies
          {
            _currentSortMethod = SortMethod.Votes;
            _currentSortDirection = " DESC";
          }
          else if (tmdBfunction == GUILocalizeStrings.Get(10798827))  // Now Playing
          {
            _currentSortMethod = SortMethod.Date;
            _currentSortDirection = " DESC";
          }
          else if (tmdBfunction == GUILocalizeStrings.Get(10798828))  // Top Rated
          {
            _currentSortMethod = SortMethod.Rating;
            _currentSortDirection = " DESC";
          }
          else if (tmdBfunction == GUILocalizeStrings.Get(10798829))  // Upcoming Movies
          {
            _currentSortMethod = SortMethod.Date;
            _currentSortDirection = " ASC";
          }
          else if (tmdBfunction == GUILocalizeStrings.Get(10798824)) // IMDB Top250 movies
          {
            // no sorting, already in correct order
            _currentSortMethod = SortMethod.Rating;
            _currentSortDirection = " DESC";
          }
          else // it is a person movie list...
          {
            _currentSortMethod = SortMethod.Date;
            _currentSortDirection = " DESC";
          }

          // if (BtnSrtBy != null) BtnSrtBy.IsAscending = (conf.StrSortSens == " ASC");
          if (BtnSrtBy != null) BtnSrtBy.IsAscending = (_currentSortDirection == " ASC");

          try
          {
            SetLabels();

            // facadeFilms.Sort(this); // no sorting needed here, because list already sorted in pull method !

            UpdateButtonStates();

          }
          catch (Exception ex)
          {
            LogMyFilms.Error(ex, "Error sorting items - {0}", ex.Message);
          }
          #endregion

          MyFilmsDetail.setGUIProperty("nbobjects.value", facadeFilms.Count.ToString());
          GUIPropertyManager.SetProperty("#itemcount", facadeFilms.Count.ToString());
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...
          GetImagesAndUpdatesForTmdb();
        }
      }, "GettingTmdbMovies", true, m_SearchAnimation); // false = no timeout !

      GUIControl.HideControl(GetID, 34); // show film details
      watch.Stop();
      LogMyFilms.Debug("'loaded all movies from TMDB (" + watch.ElapsedMilliseconds + " ms)");
      #endregion
    }

    private void GetImagesAndUpdatesForTmdb()
    {
      StopLoadingFilmlistDetails = false;

      new Thread(delegate()
      {
        #region local availability
        Thread.Sleep(25);
        Stopwatch watch = new Stopwatch(); watch.Reset(); watch.Start();
        DataRow[] rtemp = BaseMesFilms.ReadDataMovies(GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D + conf.StrDfltSelect, "", conf.WStrSort, conf.WStrSortSens);
        List<GUIListItem> itemlist = new List<GUIListItem>();

        string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
        Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"

        for (int i = 0; i < facadeFilms.Count; i++)
        {
          #region check if local available ...
          if (StopLoadingFilmlistDetails) return; // stop download if we have exited window
          try
          {
            GUIListItem item = facadeFilms[i];
            itemlist.Add(item);
            OnlineMovie movie = item.TVTag as OnlineMovie;
            if (movie == null)
            {
              continue;
            }

            //#region search for TMDB id if it is missing ...
            //if (movie.MovieSearchResult.id == -1) // we have stored IMDBid in imdb_id, if id == -1
            //{
            //  TmdbMovie tmdbmovie = api.GetMovieByIMDB(movie.MovieSearchResult.original_title);
            //  movie.MovieSearchResult.id = tmdbmovie.id;
            //  movie.MovieSearchResult.backdrop_path = tmdbmovie.backdrop_path;
            //  movie.MovieSearchResult.poster_path = tmdbmovie.poster_path;
            //  movie.MovieSearchResult.original_title = tmdbmovie.original_title;
            //  movie.MovieSearchResult.release_date = tmdbmovie.release_date;
            //}
            //#endregion

            // iMoviesLocally = rtemp.Select("Year like '" + year + "' AND TranslatedTitle like '*" + item.Label + "*'", conf.StrSorta + conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count();
            int year = DateTime.Parse(movie.MovieSearchResult.release_date).Year;
            bool any = rtemp.Any(x => (x[conf.StrTitle1].ToString().Contains(movie.MovieSearchResult.title) && (string.IsNullOrEmpty(x["Year"].ToString()) || year == 0 || x["Year"].ToString().Contains(year.ToString()))) || (!string.IsNullOrEmpty(x["TMDB_Id"].ToString()) && int.Parse(x["TMDB_Id"].ToString()) == movie.MovieSearchResult.id));
            item.IsRemote = !any;
            // LogMyFilms.Debug("CountLocalItems - found '" + iMoviesLocally + "' items for (" + year.ToString() + ") '" + movie.MovieSearchResult.title + "' !");
          }
          catch (Exception ex) 
          {
            LogMyFilms.Warn("GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
          }
          #endregion
        }
        watch.Stop();
        LogMyFilms.Debug("GetImages() - finshed setting local availability after '" + facadeFilms.Count + "' items (" + watch.ElapsedMilliseconds + " ms)");
        #endregion

        #region counts for person lists (disabled)
        //if (rtemp != null && conf.StrPersons.Length > 0)
        //{
        //  Thread.Sleep(5);
        //  watch.Reset(); watch.Start();
        //  int[] facadeCounts = new int[facadeFilms.Count];
        //  try
        //  {
        //    for (int j = 0; j < facadeFilms.Count; j++) facadeCounts[j] = 0;

        //    for (int j = 0; j < rtemp.Length; j++)
        //    {
        //      if (StopLoadingViewDetails) break; // stop download if we have exited window
        //      string title = rtemp[j][conf.StrTitle1].ToString();
        //      for (int i = 0; i < facadeFilms.Count; i++)
        //      {
        //        var movie = facadeFilms[i].TVTag as OnlineMovie;
        //        if (title.Contains(movie.MovieSearchResult.title)) // if (value.IndexOf(facadeLabels[i], StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //          facadeCounts[i]++;
        //          // facadeFilms[i].Label3 = " (" + facadeCounts[i] + ")";
        //        }
        //      }
        //    }
        //    for (int j = 0; j < facadeFilms.Count; j++)
        //      facadeFilms[j].Label3 = (facadeFilms[j].Label3.Length > 0) ? facadeFilms[j].Label3 + " [" + facadeCounts[j] + "]" : "[" + facadeCounts[j] + "]";
        //  }
        //  catch (Exception ex)
        //  {
        //    LogMyFilms.Warn("GetCounts() - error setting facadelist item '" + i + "': " + ex.Message);
        //  }
        //  watch.Stop();
        //  LogMyFilms.Debug("GetCounts() - Threaded facade details loader exit after '" + i + "' items (" + (watch.ElapsedMilliseconds) + " ms)");
        //}
        #endregion

        #region call multithreaded thumbs loader for TMDB Items
        GetImagesforTmdbFilmList(itemlist);
        #endregion
      })
      {
        IsBackground = true,
        Name = "MyFilms Image Detector and Downloader",
        Priority = ThreadPriority.BelowNormal
      }.Start();
    }

    private void GetImagesforTmdbFilmList(List<GUIListItem> itemsWithThumbs)
    {
      StopLoadingFilmlistDetails = false;
      const string wStrSort = "TMDB";
      //  string CoverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";
      string strThumbDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views\" + wStrSort.ToLower(); // +@"\";
      if (!Directory.Exists(strThumbDirectory)) Directory.CreateDirectory(strThumbDirectory);

      // split the downloads in X+ groups and do multithreaded downloading
      int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // Guzzi: Set group to x to only allow x thread(s)
      int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

      for (int i = 0; i < groups; i++)
      {
        var groupList = new List<GUIListItem>();
        for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
        {
          groupList.Add(itemsWithThumbs[j]);
        }

        new Thread(delegate(object o)
        {
          try
          {
            string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
            LogMyFilms.Debug("GetImagesforTmdbFilmList - detected language = '" + language + "'");
            // grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb();
            Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
            TmdbConfiguration tmdbConf = api.GetConfiguration();
            foreach (string posterSize in tmdbConf.images.poster_sizes)
            {
              LogMyFilms.Debug("Available TMDB Poster Size: '" + posterSize + "'");
            }
            List<GUIListItem> items = (List<GUIListItem>)o;

            foreach (GUIListItem item in items)
            {
              #region load cover images and fanart
              try
              {
                if (StopLoadingFilmlistDetails) // stop download if we have exited window
                {
                  items.SafeDispose();
                  return;
                }
                // LogMyFilms.Debug("GetImagesforTmdbFilmList() - loading TMDB Details for '" + item.Label + "'");

                #region Poster
                // string localThumb = Path.Combine(strThumbDirectory, item.Label + ".jpg");
                string localThumb = MediaPortal.Util.Utils.GetCoverArtName(strThumbDirectory, item.Label); // cached cover                
                if (File.Exists(localThumb))
                {
                  item.IconImage = localThumb;
                  item.IconImageBig = localThumb;
                  item.ThumbnailImage = localThumb;
                }
                else
                {
                  #region sanity checks
                  OnlineMovie movie = item.TVTag as OnlineMovie;
                  if (movie == null)
                  {
                    LogMyFilms.Warn("GetImagesforTmdbFilmList() - OnlineMovie object is 'null' for movie '" + item.Label + "'");
                    continue;
                  }
                  #endregion

                  //#region search for TMDB id if it is missing ...
                  //if (movie.MovieSearchResult.id == 0) // we have stored IMDBid in original title
                  //{
                  //  TmdbMovie tmdbmovie = api.GetMovieByIMDB(movie.MovieSearchResult.original_title);
                  //  movie.MovieSearchResult.id = tmdbmovie.id;
                  //  movie.MovieSearchResult.backdrop_path = tmdbmovie.backdrop_path;
                  //  movie.MovieSearchResult.poster_path = tmdbmovie.poster_path;
                  //  movie.MovieSearchResult.original_title = tmdbmovie.original_title;
                  //  // movie.MovieSearchResult.release_date = tmdbmovie.release_date;
                  //}
                  //#endregion

                  movie.MovieImages = api.GetMovieImages(movie.MovieSearchResult.id, language);
                  if (movie.MovieImages.posters == null || movie.MovieImages.posters.Count == 0)
                  {
                    movie.MovieImages = api.GetMovieImages(movie.MovieSearchResult.id, null);
                    LogMyFilms.Debug("GetImagesTMDB() - no '" + language + "' posters found - used default and found '" + (movie.MovieImages.posters != null ? movie.MovieImages.posters.Count : 0) + "'");
                  }

                  LogMyFilms.Debug("GetImagesTMDB() - localThumb = '" + localThumb + "'");
                  string remoteThumb = movie.MovieImages.posters != null && movie.MovieImages.posters.Count > 0
                                         ? tmdbConf.images.base_url + "w500" + movie.MovieImages.posters[0].file_path
                                         : "";
                  LogMyFilms.Debug("GetImagesTMDB() - remoteThumb = '" + remoteThumb + "'");
                  // stop download if we have exited window
                  if (StopLoadingFilmlistDetails) return;

                  LogMyFilms.Debug("GetImagesTMDB() - loading TMDB Details for '" + item.Label + "'");

                  if (!string.IsNullOrEmpty(remoteThumb) && !string.IsNullOrEmpty(localThumb))
                  {
                    if (GrabUtil.DownloadImage(remoteThumb, localThumb))
                    {
                      //// notify that image has been downloaded
                      //item.NotifyPropertyChanged("PosterImageFilename");
                      item.IconImage = localThumb;
                      item.IconImageBig = localThumb;
                      item.ThumbnailImage = localThumb;
                    }
                  }
                }
                #endregion

                // send message to update facade
                // GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, (int)Controls.CTRL_ListFilms, ItemId, 0, null));

                #region disabled code
                // string strThumb; // cached cover
                // string strThumbSmall; // cached cover for Icons - small resolution

                //item.MusicTag = item.ThumbnailImage; // keep Original one in music tag for big list thumb ...
                //// strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, item.DVDLabel); // item.DVDLabel is sTitle
                //strThumb = MediaPortal.Util.Utils.GetCoverArtName(CoverThumbDir, item.DVDLabel); // item.DVDLabel is sTitle
                //strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".")) + "_s" + Path.GetExtension(strThumb);

                //if (!string.IsNullOrEmpty(item.ThumbnailImage) && item.ThumbnailImage != conf.DefaultCover && !File.Exists(strThumb))
                //{
                //  Picture.CreateThumbnail(item.ThumbnailImage, strThumbSmall, 100, 150, 0, Thumbs.SpeedThumbsSmall);
                //  Picture.CreateThumbnail(item.ThumbnailImage, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
                //  LogMyFilms.Debug("GetFimList: Background thread creating thumbimage for sTitle: '" + item.DVDLabel + "'");
                //}
                //if (File.Exists(strThumb))
                //{
                //  item.IconImage = strThumbSmall;
                //  item.IconImageBig = strThumb;
                //  item.ThumbnailImage = strThumb;
                //}
                //else
                //{
                //  if (conf.DefaultCover.Length > 0)
                //  {
                //    item.IconImage = conf.DefaultCover;
                //    item.IconImageBig = conf.DefaultCover;
                //    item.ThumbnailImage = conf.DefaultCover;
                //  }
                //}
                #endregion
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("GetImagesForTmdbFilmList() - Error: '" + ex.Message + "'");
                LogMyFilms.Debug("GetImagesForTmdbFilmList() - Exception: '" + ex.StackTrace + "'");
              }
              #endregion
            }


            foreach (GUIListItem item in items)
            {
              #region update facade for tmdb trailer availability (requires one request per movie - slow!)
              try
              {
                #region sanity checks
                if (StopLoadingFilmlistDetails) // stop download if we have exited window
                {
                  items.SafeDispose();
                  return;
                }
                OnlineMovie movie = item.TVTag as OnlineMovie;
                if (movie == null)
                {
                  LogMyFilms.Warn("GetImagesforTmdbFilmList() - OnlineMovie object is 'null' for movie '" + item.Label + "'");
                  continue;
                }
                #endregion

                if (movie.MovieSearchResult.TrailerStatus == TmdbMovieSearchResult.TrailerState.Unknown) // check trailers only, if not yet done in this session
                {
                  bool remotetraileravailable = false;
                  bool localtraileravailable = false;
                  #region load trailer info from TMDB
                  movie.Trailers = api.GetMovieTrailers(movie.MovieSearchResult.id, language);
                  if (movie.Trailers.youtube == null || movie.Trailers.youtube.Count == 0)
                  {
                    // LogMyFilms.Debug("GetTrailers - no trailer found for language '" + language + "' - try default");
                    movie.Trailers = api.GetMovieTrailers(movie.MovieSearchResult.id, null);
                  }
                  // LogMyFilms.Debug("SearchTrailers - found '" + movie.Trailers.youtube.Count + "' YouTube Trailers");

                  DateTime releasedate;
                  string year = DateTime.TryParse(movie.MovieSearchResult.release_date, out releasedate)
                                  ? releasedate.Year.ToString()
                                  : "";

                  if (movie.Trailers.youtube != null)
                  {
                    for (int j = 0; j < movie.Trailers.youtube.Count; j++)
                    {
                      LogMyFilms.Debug("GetTrailers - Trailer #'" + j + "', name = '" + movie.Trailers.youtube[j].name + "', size = '" + movie.Trailers.youtube[j].size + "', source = '" + movie.Trailers.youtube[j].source + "'");
                      if (j == 0)
                      {
                        item.Path = "http://www.youtube.com/watch?v=" + movie.Trailers.youtube[j].source; // http://www.youtube.com/watch?v=KwPTIEWTYEI
                        remotetraileravailable = true;
                      }

                      #region add trailers to trailer download queue for caching

                      if (conf.CacheOnlineTrailer && conf.StrDirStorTrailer.Length > 0) // if (MyFilms.conf.BoolEnableOnlineServices)
                      {
                        string destinationDirectory = Path.Combine(conf.StrDirStorTrailer + @"OnlineTrailerCache\", item.Label + (year.Length > 0 ? " (" + year + ")" : ""));
                        Trailer trailer = new Trailer
                        {
                          MovieTitle = item.Label,
                          Trailername = movie.Trailers.youtube[j].name,
                          OriginalUrl = "http://www.youtube.com/watch?v=" + movie.Trailers.youtube[j].source,
                          SourceUrl = null,
                          Quality = null,
                          DestinationDirectory = destinationDirectory
                        };
                        LogMyFilms.Debug("GetImagesForTmdbFilmList() - trailer caching - add trailer '#" + j + "', path '" + destinationDirectory + "'");
                        AddTrailerToDownloadQueue(trailer);
                      }

                      #endregion
                    }
                  } 
                  #endregion

                  #region search locally cached trailers
                  if (conf.StrDirStorTrailer.Length > 0)
                  {
                    try
                    {
                      string directoryname = Path.Combine(conf.StrDirStorTrailer + @"OnlineTrailerCache\", movie.MovieSearchResult.title + (year.Length > 0 ? " (" + year + ")" : "")); // string destinationDirectory = Path.Combine(MyFilms.conf.StrDirStorTrailer + @"OnlineTrailerCache\", (item.Label + ((year.Length > 0) ? (" (" + year + ")") : "")));
                      if (!string.IsNullOrEmpty(directoryname) && Directory.Exists(directoryname))
                      {
                        localtraileravailable = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories).Where(MediaPortal.Util.Utils.IsVideo).Any();
                      }
                    }
                    catch (Exception ex) { LogMyFilms.Error("GetImagesForTmdbFilmList - error searching cached trailer files: '" + ex.Message); }
                  }
                  #endregion
                  if (localtraileravailable)
                  {
                    movie.MovieSearchResult.TrailerStatus = remotetraileravailable ? TmdbMovieSearchResult.TrailerState.LocalAndRemote : TmdbMovieSearchResult.TrailerState.Local;
                  }
                  else
                  {
                    movie.MovieSearchResult.TrailerStatus = remotetraileravailable ? TmdbMovieSearchResult.TrailerState.Remote : TmdbMovieSearchResult.TrailerState.None;
                  }
                }
                item.IsDownloading = (movie.MovieSearchResult.TrailerStatus == TmdbMovieSearchResult.TrailerState.Remote || movie.MovieSearchResult.TrailerStatus == TmdbMovieSearchResult.TrailerState.LocalAndRemote);
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("GetImagesForTmdbFilmList() - Error: '" + ex.Message + "'");
                LogMyFilms.Debug(ex, "GetImagesForTmdbFilmList() - Exception: '" + ex.Message + "'");
              }
              #endregion
            }

            foreach (GUIListItem item in items)
            {
              #region load backdrops from tmdb
              try
              {
                if (StopLoadingFilmlistDetails) // stop download if we have exited window
                {
                  items.SafeDispose();
                  return;
                }

                #region Fanart
                if (StopLoadingFilmlistDetails) return;
                if (!conf.StrFanart)
                {
                  continue;
                }

                //string[] wfanart = MyFilmsDetail.Search_Fanart(item.Label, true, "file", false, item.ThumbnailImage, string.Empty);
                //if (!string.IsNullOrEmpty(wfanart[0]) && wfanart[0] != " " && File.Exists(wfanart[0]))
                //{
                //  continue;
                //}

                #region sanity checks
                OnlineMovie movie = item.TVTag as OnlineMovie;
                if (movie == null)
                {
                  LogMyFilms.Warn("GetImagesforTmdbFilmList() - OnlineMovie object is 'null' for movie '" + item.Label + "'");
                  continue;
                }
                if (string.IsNullOrEmpty(movie.MovieSearchResult.backdrop_path))
                {
                  LogMyFilms.Warn("GetImagesforTmdbFilmList() - backdrop_path is 'empty' for movie '" + item.Label + "'");
                  continue;
                }
                #endregion

                string fanartUrl = tmdbConf.images.base_url + "original" + movie.MovieSearchResult.backdrop_path;
                LogMyFilms.Debug("GetImagesforTmdbFilmList - start loading fanart for '{0}' from url = '{1}'", item.Label, fanartUrl);
                string filename;
                string filename1 = GrabUtil.DownloadBacdropArt(conf.StrPathFanart, fanartUrl, item.Label, true, true, out filename);
                LogMyFilms.Debug("GetImagesforTmdbFilmList - Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + item.Label);

                //movie.MovieImages = api.GetMovieImages(movie.MovieSearchResult.id, language);
                //if (movie.MovieImages.posters.Count == 0)
                //{
                //  movie.MovieImages = api.GetMovieImages(movie.MovieSearchResult.id, null);
                //  LogMyFilms.Debug("GetImagesTMDB() - no '" + language + "' posters found - used default and found '" + movie.MovieImages.posters.Count + "'");
                //}
                //int ii = 0;
                //foreach (Backdrop fanart in movie.MovieImages.backdrops)
                //{
                //  if (ii == 0)
                //  {
                //    string fanartUrl = tmdbConf.images.base_url + "original" + fanart.file_path;
                //    string filename;
                //    string filename1 = GrabUtil.DownloadBacdropArt(MyFilms.conf.StrPathFanart, fanartUrl, item.Label, true, (i == 0), out filename);
                //    LogMyFilms.Debug("Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + item.Label);
                //  }
                //  ii++;
                //}

                #endregion
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug(ex, "GetImagesForTmdbFilmList() - Error: '" + ex.Message + "'");
              }
              #endregion
            }
          }
          catch (Exception e) { LogMyFilms.Debug(e, "GetImagesforTmdbFilmList()"); }
          LogMyFilms.Debug("GetImagesforTmdbFilmList() - Finished Thread for loading TMDB Details");
        })
        {
          IsBackground = true,
          Priority = ThreadPriority.BelowNormal,
          Name = "MyFilms FilmList Image Detector " + i
        }.Start(groupList);
      }
    }

    private void Change_SelectTmdbEntry_Action(GUIListItem selItem)
    {
      movielink = null; // reset movielink

      new Thread(delegate()
      {
        {
          try
          {
            // MyFilmsPlugin.Utils.OVplayer.Play(facadeFilms.SelectedListItem.Path);
            string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
            Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
            OnlineMovie movie = facadeFilms.SelectedListItem.TVTag as OnlineMovie;
            if (movie == null) return;

            MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation);

            #region search locally cached trailers
            var cachedTrailerFiles = new List<string>();
            if (conf.StrDirStorTrailer.Length > 0)
            {
              try
              {
                DateTime releasedate;
                string year = DateTime.TryParse(movie.MovieSearchResult.release_date, out releasedate) ? releasedate.Year.ToString() : "";
                string directoryname = Path.Combine(conf.StrDirStorTrailer + @"OnlineTrailerCache\", movie.MovieSearchResult.title + (year.Length > 0 ? " (" + year + ")" : "")); // string destinationDirectory = Path.Combine(MyFilms.conf.StrDirStorTrailer + @"OnlineTrailerCache\", (item.Label + ((year.Length > 0) ? (" (" + year + ")") : "")));
                if (!string.IsNullOrEmpty(directoryname) && Directory.Exists(directoryname))
                {
                  cachedTrailerFiles.AddRange(Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories).Where(MediaPortal.Util.Utils.IsVideo)); // trailerFiles = trailerFiles.Distinct().ToList();
                  LogMyFilms.Debug("Change_SelectTmdbEntry_Action - found '" + cachedTrailerFiles.Count + "' locally cached trailer files");
                }
              }
              catch (Exception ex) { LogMyFilms.Error("Change_SelectTmdbEntry_Action - error searching cached trailer files: '" + ex.Message); }
            }
            #endregion

            #region search trailers
            var trailersfound = new List<Youtube>();
            int iLocalTrailers = 0;
            TmdbMovieTrailers trailers = api.GetMovieTrailers(movie.MovieSearchResult.id, language); // trailers in local language
            if (trailers.youtube.Count > 0)
            {
              trailersfound.AddRange(trailers.youtube);
              iLocalTrailers = trailersfound.Count;
            }
            trailers = api.GetMovieTrailers(movie.MovieSearchResult.id, null); // all trailers
            if (trailers.youtube.Count > 0) trailersfound.AddRange(trailers.youtube);
            LogMyFilms.Debug("Change_SelectTmdbEntry_Action - found '" + trailersfound.Count + "' YouTube Trailers");
            #endregion

            MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation);

            bool launchLocalMovies = false;
            if (trailersfound.Count == 0 && cachedTrailerFiles.Count == 0)
            {
              if (selItem.IsRemote)
              {
                GUIUtils.ShowNotifyDialog("No trailer available!");
              }
              else
              {
                launchLocalMovies = true;
              }
            }
            else if (trailersfound.Count == 1 && selItem.IsRemote && cachedTrailerFiles.Count == 0)
            {
              #region play single trailer in OV player factory
              if (Helper.IsOnlineVideosAvailableAndEnabled)
              {
                //bool success = MyFilmsPlugin.Utils.OVplayer.Play("http://www.youtube.com/watch?v=" + trailersfound[0].source, conf.BoolAskForPlaybackQuality, m_SearchAnimation);
                //if (!success) MyFilmsDetail.ShowNotificationDialog("Info", GUILocalizeStrings.Get(10798998)); // Cannot play online content !
                movielink = "http://www.youtube.com/watch?v=" + trailersfound[0].source;
              }
              else
              {
                GUIUtils.ShowNotifyDialog("OnlineVideos is not available!");
              }
              #endregion
            }
            else
            {
              #region build menu with local and remote trailers
              var choiceView = new List<string>();
              var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
              if (dlg == null) return;
              dlg.Reset();
              dlg.SetHeading(GUILocalizeStrings.Get(10798986)); // MyFilms Movie Preview

              #region movie catalog - local movies
              if (selItem.IsRemote == false)
              {
                dlg.Add(GUILocalizeStrings.Get(10798987)); // Movie Catalog ...
                choiceView.Add("videos");
              }
              #endregion

              #region add locally cached trailers
              foreach (string cachedTrailerFile in cachedTrailerFiles)
              {
                var trailerName = cachedTrailerFile;
                trailerName = trailerName.Contains("\\") ? trailerName.Substring(trailerName.LastIndexOf("\\", StringComparison.Ordinal) + 1) : trailerName;
                trailerName = trailerName.Substring(trailerName.IndexOf("(trailer)", StringComparison.Ordinal) + 10); // strip title and trailer prefix
                long wsize = File.Exists(cachedTrailerFile) ? new FileInfo(cachedTrailerFile).Length : 0;
                string entry = GUILocalizeStrings.Get(10798988) + trailerName + " (" + string.Format("{0} MB", wsize / 1048576) + ")"; // local: 
                using (var listItem = new GUIListItem { Label = entry, IsDownloading = false, IconImage = conf.DefaultCover, Path = cachedTrailerFile })
                {
                  dlg.Add(listItem);
                }
                choiceView.Add(cachedTrailerFile);
              }
              #endregion

              #region add online trailers
              for (int i = 0; i < trailersfound.Count; i++)
              {
                Youtube trailer = trailersfound[i];
                string entry = GUILocalizeStrings.Get(10798989) + trailer.name + " (" + trailer.size + ")"; // online: 
                if (i < iLocalTrailers) entry += " - (" + language + ")";
                using (var listItem = new GUIListItem { Label = entry, IsDownloading = true, IconImage = conf.DefaultCover, Path = "http://www.youtube.com/watch?v=" + trailer.source })
                {
                  dlg.Add(listItem);
                }
                choiceView.Add("http://www.youtube.com/watch?v=" + trailer.source);
              }
              #endregion

              #endregion

              dlg.DoModal(GetID);
              if (dlg.SelectedLabel == -1) return;

              if (choiceView[dlg.SelectedLabel] == "videos")
                launchLocalMovies = true;
              else
              {
                movielink = choiceView[dlg.SelectedLabel];
                //if (!choiceView[dlg.SelectedLabel].StartsWith("http"))
                //{
                //  #region play locally cached trailers locally
                //  var trailer = new ArrayList { choiceView[dlg.SelectedLabel] };
                //  // currentTrailerPlayingItem = choiceView[dlg.SelectedLabel];
                //  MyFilmsDetail.trailerPlayed = true;
                //  MyFilmsDetail.Launch_Movie_Trailer_Scrobbling(trailer, ID_MyFilms);
                //  #endregion
                //}
                //else
                //{
                //  #region play trailer in OV player factory
                //  if (Helper.IsOnlineVideosAvailableAndEnabled)
                //  {
                //    MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls(choiceView[dlg.SelectedLabel]);
                //    bool success = MyFilmsPlugin.Utils.OVplayer.Play(choiceView[dlg.SelectedLabel], MyFilms.conf.BoolAskForPlaybackQuality, m_SearchAnimation);
                //    if (!success) MyFilmsDetail.ShowNotificationDialog("Info", GUILocalizeStrings.Get(10798998)); // Cannot play online content !
                //  }
                //  else
                //  {
                //    MyFilmsDetail.ShowNotificationDialog("Info", "OnlineVideos is not available!");
                //  }
                //  #endregion
                //}
              }
            }

            if (launchLocalMovies)
            {
              movielink = null;
              #region jump to local movies
              SaveListState(false);
              conf.Wselectedlabel = selItem.Label;
              conf.Boolselect = true;
              conf.Boolreturn = !selItem.IsFolder;
              //conf.StrTxtSelect = "";
              //conf.StrTitleSelect = "";
              conf.StrSelect = ""; // reset movie context filter for person views
              conf.StrPersons = ""; // reset person list filter
              conf.StrViewSelect = ""; // reset view list filter
              viewcover.Filename = "";
              personcover.Filename = "";
              groupcover.Filename = "";

              new Thread(delegate()
              {
                do
                {
                  if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                  conf.StrTitleSelect += conf.Wselectedlabel;
                }
                while (GetFilmList() == false); //keep calling while single folders found
                GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => 0, 0, 0, null);
              }) { Name = "GetFilmList", IsBackground = true }.Start();
              #endregion
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("Change_SelectTmdbEntry_Action - error: " + ex.Message);
          }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            LogMyFilms.Debug("Change_SelectTmdbEntry_Action - try to start playback for link: '" + (movielink ?? "") + "'"); 
            if (movielink == null) return 0;
            if (string.IsNullOrEmpty(movielink))
            {
              GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798998)); // Cannot play online content !
              return 0;
            }
            if (!movielink.StartsWith("http"))
            {
              #region play locally cached trailers locally
              var trailer = new ArrayList { movielink };
              // currentTrailerPlayingItem = choiceView[dlg.SelectedLabel];
              MyFilmsDetail.trailerPlayed = true;
              MyFilmsDetail.Launch_Movie_Trailer_Scrobbling(trailer, ID_MyFilms);
              #endregion
            }
            else
            {
              #region play trailer in OV player factory
              if (Helper.IsOnlineVideosAvailableAndEnabled)
              {
                MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls(movielink);
                bool playbacksuccess = MyFilmsPlugin.Utils.OVplayer.Play(movielink, conf.BoolAskForPlaybackQuality, m_SearchAnimation);
                if (!playbacksuccess) GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798998)); // Cannot play online content !
              }
              else
              {
                GUIUtils.ShowNotifyDialog("OnlineVideos is not available!");
              }
              #endregion
            }
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

    }

    /// <summary>Selects records for display grouping them as required</summary>
    /// <param name="wstrSelect">Select this kind of records</param>
    /// <param name="wStrSort">Sort based on this</param>
    /// <param name="wStrSortSens">Asc/Desc. Ascending or descending sort order</param>
    /// <param name="newWstar">Entries must contain this string to be included</param>
    /// <param name="clearIndex">Reset Selected Item Index</param>
    /// <param name="selItem">Select entry matching this string if not empty</param>
    /// 
    private void GetSelectFromDivx(string wstrSelect, string wStrSort, string wStrSortSens, string newWstar, bool clearIndex, string selItem)
    {
      #region example
      //Example for Actors list:
      //conf.wStrSort = "ACTORS";
      //conf.Wselectedlabel = "";
      //conf.wStrSortSens = " ASC";
      //conf.StrPersons = keyboard.Text;
      //GetSelectFromDivx("Actors like '*" + keyboard.Text + "*'", conf.wStrSort, conf.wStrSortSens, keyboard.Text, true, "");
      #endregion

      string globalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;
      LogMyFilms.Debug("(GetSelectFromDivx) - GlobalFilterString      : '" + globalFilterString + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - conf.StrViewSelect      : '" + conf.StrViewSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - conf.StrDfltSelect      : '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - wstrSelect              : '" + wstrSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - wStrSort - wStrSortSens : '" + wStrSort + "' - '" + wStrSortSens + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - newWstar - clearIndex   : '" + newWstar + "' - '" + clearIndex + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - selItem  - StrPersons   : '" + selItem + "' - '" + conf.StrPersons + "'");

      #region Setup variables and configure sorting and buttons
      Prev_ItemID = -1;
      Prev_Label = string.Empty;

      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the start ....
      GUIPropertyManager.SetProperty("#itemcount", "0");

      GUIListItem item = null;
      string champselect;
      string wchampselect = "";
      string wchampselectIndexed = "";

      FieldType fieldType = GetFieldType(wStrSort);
      bool isperson = IsPersonField(fieldType);
      bool isdate = IsDateField(fieldType);
      bool issplitfield = IsSplittableField(wStrSort);
      bool istitlefield = IsTitleField(wStrSort);
      bool dontsplitvalues = conf.BoolDontSplitValuesInViews;
      bool showEmptyValues = conf.BoolShowEmptyValuesInViews;
      bool isrecentlyadded = wStrSort == "RecentlyAdded"; // calculate recently added fields
      bool isindexedtitle = wStrSort == "IndexedTitle"; // calculate recently added fields
      bool reversenames = isperson && conf.BoolReverseNames;
      // string defaultcover = (isperson) ? conf.DefaultCoverArtist : ((istitlefield) ? conf.DefaultCover : conf.DefaultCoverViews);

      //DateTime now = DateTime.Now;

      BtnSrtBy.IsEnabled = true;
      BtnSrtBy.Label = conf.BoolSortCountinViews ? GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(1079910) : GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(103); // sort: count / sort: name
      BtnSrtBy.IsAscending = conf.BoolSortCountinViews ? conf.WStrSortSensCount == " ASC" : wStrSortSens == " ASC";
      conf.ViewContext = isperson ? ViewContext.Person : ViewContext.Group;

      //if (isperson && !string.IsNullOrEmpty(conf.StrPersons))
      //{
      //  conf.StrSelect = conf.wStrSort + " like '*" + conf.StrPersons + "*'";
      //}

      if (conf.IndexedChars == 0) conf.Boolindexed = false;
      bool isindexed = conf.Boolindexed;
      int indexedChars = conf.IndexedChars;
      conf.Wstar = newWstar;
      conf.Boolselect = true;
      conf.Wselectedlabel = "";
      if (clearIndex) conf.StrIndex = 0;
      // if (conf.UseListViewForGoups) Change_Layout_Action(0); else 
      GUIControl.ClearControl(GetID, facadeFilms.GetID);
      // ClearFacade(); // facadeFilms.Clear();
      Change_Layout_Action(conf.WStrLayOut);
      #endregion

      #region Collection of all items or subitems
      // Collect List of all attributes in w_tableau
      List<string> wTableau = new List<string>();
      int wnbEnr = 0;
      int wnbEnrIndexed = 0; // counts for indexed views
      int wi;
      conf.DbSelection = new[] { globalFilterString + conf.StrViewSelect + conf.StrDfltSelect, wstrSelect, wStrSort, wStrSortSens, false.ToString(), false.ToString() };

      watch.Reset(); watch.Start();

      foreach (DataRow row in BaseMesFilms.ReadDataMovies(globalFilterString + conf.StrViewSelect + conf.StrDfltSelect, wstrSelect, wStrSort, wStrSortSens))
      {
        if (isdate)
        {
          // champselect = string.Format("{0:yyyy/MM/dd}", row["DateAdded"]);
          DateTime tmpdate;
          champselect = DateTime.TryParse(row[wStrSort].ToString(), out tmpdate) ? string.Format("{0:yyyy/MM/dd}", tmpdate.ToShortDateString()) : ""; // .ToString("o", CultureInfo.InvariantCulture)
          //try { champselect = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(row[wStrSort].ToString()).ToShortDateString()); }
          //catch (Exception) { champselect = ""; }
        }
        else
          champselect = row[wStrSort].ToString().Trim();

        if (issplitfield && !dontsplitvalues)
        {
          List<string> wtab = Search_String(champselect, reversenames); //ArrayList wtab = Search_String(champselect, isperson);
          if (wtab.Count > 0)
          {
            wTableau.AddRange(wtab);
          }
          else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            wTableau.Add(champselect);
          }
        }
        else
        {
          if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            wTableau.Add(champselect);
          }
        }
      }
      watch.Stop();
      LogMyFilms.Debug("(GetSelectFromDivx) - Read View Names for '" + wStrSort + "' finished (splittable items = '" + issplitfield + "', dontsplitvalues = '" + dontsplitvalues + "', reversenames = '" + reversenames + "') (" + watch.ElapsedMilliseconds + " ms)");
      #endregion

      #region Sorting based on FieldType using custom IComparer
      watch.Reset(); watch.Start();
      switch (fieldType)
      {
        case FieldType.AlphaNumeric:
        case FieldType.Decimal:
          if (wStrSortSens == " ASC")
          {
            wTableau.Sort(0, wTableau.Count, new AlphanumComparatorFastString());
          }
          else
          {
            wTableau.Sort(0, wTableau.Count, new AlphanumComparatorFastString());
            wTableau.Reverse();
          }
          break;

        default: // default sorter
          if (wStrSortSens == " ASC")
          {
            wTableau.Sort(0, wTableau.Count, StringComparer.OrdinalIgnoreCase);
            //w_tableau.Sort(0, w_tableau.Count, new myForwarderClass());
            //w_tableau.Sort(0, w_tableau.Count, null);
          }
          else
          {
            wTableau.Sort(0, wTableau.Count, StringComparer.OrdinalIgnoreCase);
            wTableau.Reverse();
            //w_tableau.Sort(0, w_tableau.Count, new myReverserClass());
          }
          break;
      }
      watch.Stop();
      LogMyFilms.Debug("(GetSelectFromDivx) - Base Sorting Finished (FieldType = '" + fieldType + "') (" + watch.ElapsedMilliseconds + " ms)");
      #endregion

      #region Sort by occurencies, if requested
      if (conf.BoolSortCountinViews) // if sorting by counts is selected ... do it...
      {
        watch.Reset(); watch.Start();
        int itemcount = 0;
        string currentitem = "";
        string itemValue = "";

        List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
        for (wi = 0; wi != wTableau.Count; wi++)
        {
          itemValue = wTableau[wi].ToString();
          if (!isindexed && string.Compare(currentitem, itemValue, StringComparison.OrdinalIgnoreCase) == 0) // Are the strings equal? Then add count!
            itemcount++;
          else if (isindexed && string.Compare(itemValue, 0, currentitem, 0, indexedChars, StringComparison.OrdinalIgnoreCase) == 0) //  CultureInfo.CurrentCulture, CompareOptions.OrdinalIgnoreCase
          {
            itemcount++; // count items of distinct property
            //if (string.Compare(champselect, wchampselectIndexed, StringComparison.OrdinalIgnoreCase) != 0)
            //{
            //  itemcountIndexed++;
            //  currentitemIndexed = itemValue;
            //}
          }
          else
          {
            if (itemcount > 0 && itemValue.Length > 0)
            {
              for (int i = wi - itemcount; i != wi; i++)
              {
                list.Add(new KeyValuePair<string, int>(currentitem, itemcount));
              }
            }
            // LogMyFilms.Debug("(GetSelectFromDivx) - Counting item: '" + currentitem + "', Count = '" + itemcount + "'");
            itemcount = 1;
            currentitem = wTableau[wi];
          }
        }
        if (itemcount > 0 && itemValue.Length > 0)
        {
          for (int i = wi - itemcount; i != wi; i++)
          {
            list.Add(new KeyValuePair<string, int>(currentitem, itemcount));
          }
        }
        watch.Stop();
        LogMyFilms.Debug("(GetSelectFromDivx) - Collecting Countlist finished (" + watch.ElapsedMilliseconds + " ms)");

        watch.Reset(); watch.Start();
        if (conf.WStrSortSensCount == " ASC") list.Sort(CompareCount);
        else list.Sort(CompareCountDescending);
        watch.Stop();
        LogMyFilms.Debug("(GetSelectFromDivx) - Sorting by ocurrencies finished (" + watch.ElapsedMilliseconds + " ms)");

        watch.Reset(); watch.Start();
        for (wi = 0; wi != wTableau.Count; wi++)
        {
          wTableau[wi] = list[wi].Key;
        }
        watch.Stop();
        LogMyFilms.Debug("(GetSelectFromDivx) - Copy back to display array finished (" + watch.ElapsedMilliseconds + " ms)");
      }
      #endregion

      #region count objects for indexed views ...
      //watch.Reset(); watch.Start();
      //Dictionary<object, int> counts = new Dictionary<object, int>();
      //foreach (object item in w_tableau)
      //{
      //  if (!counts.ContainsKey(item)) counts.Add(item, 1);
      //  else counts[item]++;
      //}
      //int count = counts.Count;
      //watch.Stop();
      //LogMyFilms.Debug("(GetSelectFromDivx) - Count objects for indexed view finished (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion

      #region directory handling
      string strThumbDirectory = isperson ? MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons\" : MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views\" + wStrSort.ToLower() + @"\";

      bool getThumbs = (conf.UseThumbsForPersons && isperson) || conf.UseThumbsForViews && (conf.StrViewsDfltAll || IsCategoryYearCountryField(wStrSort)) || conf.StrViewsShowIndexedImgInIndViews;
      bool createFanartDir = IsCategoryYearCountryField(wStrSort);

      if (!Directory.Exists(strThumbDirectory)) // Check groupview thumbs cache directories and create them
        try { Directory.CreateDirectory(strThumbDirectory); } catch (Exception) { }
      if (conf.StrPathViews.Length > 0 && !Directory.Exists(conf.StrPathViews + @"\" + wStrSort.ToLower())) // Check groupview thumbs (sub)directories and create them
        try { Directory.CreateDirectory(conf.StrPathViews + @"\" + wStrSort.ToLower()); } catch (Exception) { }
      #endregion

      // if (isperson) { AsynUpdateActors(w_tableau); } // Launch Backgroundworker to (off)-load actors artwork and create cache thumbs

      #region populating facade
      LogMyFilms.Debug("(GetSelectFromDivx) - Facadesetup Groups Started");
      //int number = -1;
      bool countItems = isperson && !wstrSelect.Contains("not like") && !string.IsNullOrEmpty(wstrSelect) && !wstrSelect.Contains("**"); // extensive counts only for conditional lists, otherwise takes too much time!
      watch.Reset(); watch.Start();
      for (wi = 0; wi != wTableau.Count; wi++)
      {
        champselect = wTableau[wi];
        if (!isindexed && string.Compare(champselect, wchampselect, StringComparison.OrdinalIgnoreCase) == 0)  // if (!MyFilms.conf.Boolindexed && string.Compare(champselect, wchampselect, true) == 0) // Are the strings equal? Then add count!
        {
          wnbEnr++; // count items of distinct property
        }
        else if (isindexed && string.Compare(champselect, 0, wchampselect, 0, indexedChars, StringComparison.OrdinalIgnoreCase) == 0) //  CultureInfo.CurrentCulture, CompareOptions.OrdinalIgnoreCase
        {
          wnbEnr++; // count items of distinct property
          if (string.Compare(champselect, wchampselectIndexed, StringComparison.OrdinalIgnoreCase) != 0)
          {
            wnbEnrIndexed++;
            wchampselectIndexed = champselect;
          }
        }
        else
        {
          if (conf.Wstar == "*" || ListConditionTrue(champselect, conf.Wstar, conf.Boolindexedreturn, reversenames))
          {
            if (wnbEnr > 0 && (wchampselect.Length > 0 || conf.BoolShowEmptyValuesInViews))
            {
              //number++;
              item = new GUIListItem();
              //item.ItemId = number;
              string label = isrecentlyadded ? wchampselect.Substring(1) : wchampselect;
              string label3 = istitlefield && !isindexed && label.IndexOf(conf.TitleDelim) > 0 ? label.Substring(0, label.LastIndexOf(conf.TitleDelim)) : ""; // loads title path
              if (istitlefield && label.IndexOf(conf.TitleDelim) > 0) label = label.Substring(label.LastIndexOf(conf.TitleDelim) + 1);
              if (isindexed && indexedChars > 0 && label.Length >= indexedChars) label = label.Substring(0, indexedChars).ToUpperInvariant();
              item.Label = label.Length == 0 ? EmptyFacadeValue : label; // show <empty> value if empty
              item.Label2 = isindexed ? wnbEnrIndexed + " (" + wnbEnr + ")" : wnbEnr.ToString(); // preset - will be repopulated with counts, if requested in thread ...
              item.Label3 = label3;
              item.Path = wStrSort.ToLower();
              //item.TVTag = (isperson) ? "person" : "group";
              item.IsFolder = true;
              //item.ThumbnailImage = defaultcover;
              //item.IconImage = defaultcover;
              //item.IconImageBig = defaultcover;
              item.OnItemSelected += item_OnItemSelected;
              facadeFilms.Add(item);
              if (selItem != "" && item.Label == selItem) conf.StrIndex = facadeFilms.Count - 1; //test if this item is one to select
            }
            wnbEnr = 1;
            wchampselect = champselect;
            wnbEnrIndexed = 1;
            wchampselectIndexed = champselect;
          }
        }
      }

      if (wnbEnr > 0 && (wchampselect.Length > 0 || conf.BoolShowEmptyValuesInViews))
      {
        //number++;
        item = new GUIListItem();
        //item.ItemId = number; // Only used in GetFilmList
        // item.Label = (wchampselect.Length == 0) ? EmptyFacadeValue : (isrecentlyadded) ? wchampselect.Substring(1) : wchampselect;
        string label = isrecentlyadded ? wchampselect.Substring(1) : wchampselect;
        string label3 = istitlefield && !isindexed && label.IndexOf(conf.TitleDelim) > 0 ? label.Substring(0, label.LastIndexOf(conf.TitleDelim)) : ""; // loads title path
        if (istitlefield && label.IndexOf(conf.TitleDelim) > 0) label = label.Substring(label.LastIndexOf(conf.TitleDelim) + 1);
        if (isindexed && indexedChars > 0 && label.Length >= indexedChars) label = label.Substring(0, indexedChars).ToUpperInvariant();
        item.Label = label.Length == 0 ? EmptyFacadeValue : label; // show <empty> value if empty
        item.Label2 = isindexed ? wnbEnrIndexed + " (" + wnbEnr + ")" : wnbEnr.ToString(); // preset - will be repopulated with counts, if requested in thread ...
        item.Label3 = label3;
        //item.Label3 = wStrSort;
        //if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) MediaPortal.Util.Utils.SetDefaultIcons(item);
        item.Path = wStrSort.ToLower();
        //item.TVTag = (isperson) ? "person" : "group";
        item.IsFolder = true;
        //item.ThumbnailImage = defaultcover;
        //item.IconImage = defaultcover;
        //item.IconImageBig = defaultcover;
        item.OnItemSelected += item_OnItemSelected;
        facadeFilms.Add(item);
        if (selItem != "" && item.Label == selItem) conf.StrIndex = facadeFilms.Count - 1; //test if this item is one to select
        wnbEnr = 0;
      }
      if (item != null) item.FreeMemory();
      watch.Stop();
      LogMyFilms.Debug("(GetSelectFromDivx) - Facadesetup Groups Finished (" + watch.ElapsedMilliseconds + " ms)");
      #endregion

      #region Final settings and thumbs
      conf.StrTxtSelect = string.IsNullOrEmpty(conf.StrSelect) || conf.StrSelect == conf.StrTitle1 + " not like ''" 
        ? GUILocalizeStrings.Get(1079870) 
        : conf.StrSelect.Substring(conf.StrSelect.IndexOf("'")).Trim(new Char[] { '\'', '*' });  // "Selection"
      if (conf.Wstar != "*" && !string.IsNullOrEmpty(conf.Wstar))
        conf.StrTxtSelect += " " + (conf.Wstar.Length > 1 ? "*" : "") + conf.Wstar + "*"; // conf.StrTxtSelect += " " + BaseMesFilms.Translate_Column(wStrSort) + (" [" + ((conf.Wstar.Length > 1) ? "*" : "") + conf.Wstar + "*]"); // conf.StrTxtSelect += " " + GUILocalizeStrings.Get(1079896) + " [*" + conf.Wstar + "*]"; // add to "Selection": Persons with Filter
      SetLabelSelect(conf.StrTxtSelect);
      conf.StrSelect = wstrSelect;
      // if (conf.StrPersons.Length == 0) conf.StrFilmSelect = string.Empty; // disabled, as otherwise we don't get filtered results for single movies ...

      if ((conf.StrIndex > facadeFilms.Count - 1) || (conf.StrIndex < 0)) //check index within bounds, will be unless xml file heavily edited
        conf.StrIndex = 0;

      // will be done in Lst_Details later ...GUIControl.ShowControl(GetID, 34); // hide film controls ....

      if (facadeFilms.Count == 0)
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798637), GUILocalizeStrings.Get(10798638), "", ""); //"no movies matching the view" - " show filmlist"
        DisplayAllMovies();
        GetFilmList();
        SetLabelView("all");
      }
      else
      {
        if (getThumbs)
        {
          #region load first image syncronously, as asyncloading might cause flicker or even let it disappear
          if (conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn && conf.StrViewsShowIndexedImgInIndViews)
            LoadIndexSkinThumbs(facadeFilms[conf.StrIndex]);
          else
          {
            // if there is a Default.jpg in the view subfolder
            string strPathViewsRoot = conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\" ? conf.StrPathViews : conf.StrPathViews + "\\";
            string strImageInViewsDefaultFolder = strPathViewsRoot + wStrSort.ToLower() + ".jpg";
            string defaultViewImage = File.Exists(strImageInViewsDefaultFolder) ? strImageInViewsDefaultFolder : null;
            string[] strActiveFacadeImages = SetViewThumbs(wStrSort, facadeFilms[conf.StrIndex].Label, strThumbDirectory, isperson, GetCustomViewFromViewLabel(conf.CurrentView), defaultViewImage, conf.BoolReverseNames);
            // string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
            facadeFilms[conf.StrIndex].ThumbnailImage = strActiveFacadeImages[0];
            facadeFilms[conf.StrIndex].IconImage = strActiveFacadeImages[1];
            facadeFilms[conf.StrIndex].IconImageBig = strActiveFacadeImages[0];
          }
          #endregion
          GetImages(wStrSort, strThumbDirectory, isperson, getThumbs, createFanartDir, countItems); // load the rest of images asynchronously!
        }
        //if (IsPersonField(fieldType)) MyFilmsDetail.Load_Detailed_PersonInfo(facadeFilms.SelectedListItem.Label, false);
        //else MyFilmsDetail.Load_Detailed_DB(0, false);
        //this.Load_Rating(0); // old method - nor more used
        MyFilmsDetail.setGUIProperty("nbobjects.value", facadeFilms.Count.ToString());
        GUIPropertyManager.SetProperty("#itemcount", facadeFilms.Count.ToString());
      }
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select"));
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, conf.StrIndex);
      if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
      // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...
      #endregion
    }

    private bool ListConditionTrue(string champselect, string filterlist, bool indexedview, bool reversenames) // checks, if a single string or comma separated strings do a matching for the list ...
    {
      if (filterlist.IndexOf(',') >= 0) // comma separated list ... so process it ...
      {
        foreach (string s in Search_String(filterlist, reversenames))
        {
          if (indexedview)
          {
            if (champselect.StartsWith(s, StringComparison.OrdinalIgnoreCase))
              return true;
          }
          else
          {
            // if (champselect.ToUpper().Contains(s.Trim().ToUpper())) return true;
            if (champselect.IndexOf(s.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
              return true;
          }
        }
      }
      else
      {
        if (indexedview) // if (filterlist.Length == 1) // if it's single character compare, use it as "first character"
        {
          return champselect.StartsWith(filterlist, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
          // if (champselect.ToUpper().Contains(filterlist.ToUpper()))
          return champselect.IndexOf(filterlist, StringComparison.OrdinalIgnoreCase) >= 0;
        }
      }
      return false;
    }

    private void GetImages(string wStrSort, string strThumbDirectory, bool isperson, bool getThumbs, bool createFanartDir, bool countItems)
    {
      StopLoadingViewDetails = false;
      new Thread(delegate()
          {
            #region images
            Thread.Sleep(25);
            int i;
            var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();

            MFview.ViewRow currentCustomView = GetCustomViewFromViewLabel(conf.CurrentView);

            // if there is a Default.jpg in the view subfolder
            string strPathViewsRoot = conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\"
                                        ? conf.StrPathViews
                                        : conf.StrPathViews + "\\";
            string strImageInViewsDefaultFolder = strPathViewsRoot + wStrSort.ToLower() + ".jpg";
            string defaultViewImage = (File.Exists(strImageInViewsDefaultFolder)) ? strImageInViewsDefaultFolder : null;
            bool reversenames = conf.BoolReverseNames;

            for (i = 0; i < facadeFilms.Count; i++)
            {
              if (StopLoadingViewDetails) return; // stop download if we have exited window
              try
              {
                GUIListItem item = facadeFilms[i];

                #region get thumbs

                if (getThumbs && facadeFilms[i] != null)
                {
                  if (string.IsNullOrEmpty(facadeFilms[i].ThumbnailImage))
                  {
                    if (conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn &&
                        conf.StrViewsShowIndexedImgInIndViews)
                    {
                      LoadIndexSkinThumbs(facadeFilms[i]);
                    }
                    else
                    {
                      string[] strActiveFacadeImages = SetViewThumbs(
                        wStrSort,
                        item.Label,
                        strThumbDirectory,
                        isperson,
                        currentCustomView,
                        defaultViewImage,
                        reversenames);
                      //string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
                      //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
                      //{
                      //  item.ThumbnailImage = texture;
                      //  item.IconImage = texture;
                      //  item.IconImageBig = texture;
                      //}

                      item.IconImage = strActiveFacadeImages[1];
                      item.IconImageBig = strActiveFacadeImages[0];
                      item.ThumbnailImage = strActiveFacadeImages[0];

                      // if selected force an update of thumbnail
                      //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
                      //if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
                    }
                  }
                }

                #endregion

                if (createFanartDir) MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, wStrSort.ToLower());

                #region thumb downloads (no active)

                // ToDo: Add downloader to SetViewThumbs - or here ...

                //string remoteThumb = item.ImageRemotePath;
                //if (string.IsNullOrEmpty(remoteThumb)) continue;

                //string localThumb = item.Image;
                //if (string.IsNullOrEmpty(localThumb)) continue;

                //if (Helper.DownloadFile(remoteThumb, localThumb))
                //{
                //  // notify that thumbnail image has been downloaded
                //  item.ThumbnailImage = localThumb;
                //  item.NotifyPropertyChanged("ThumbnailImage");
                //}

                #endregion
              }
              catch (Exception ex)
              {
                LogMyFilms.Warn("GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
              }
            }
            stopwatch.Stop();
            LogMyFilms.Debug("GetImages() - Threaded facade images loader finished after '" + i + "' items (" + stopwatch.ElapsedMilliseconds + " ms)");

            #endregion

            #region counts for person lists

            DataRow[] rtemp = null;
            if (conf.StrPersons.Length > 0) // reading full dataset only required, if personcounts are requested...
              rtemp = BaseMesFilms.ReadDataMovies(GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D + conf.StrViewSelect + conf.StrDfltSelect, "", wStrSort, conf.WStrSortSens);
            if (rtemp != null && conf.StrPersons.Length > 0)
            {
              Thread.Sleep(50);
              stopwatch.Reset(); stopwatch.Start();
              bool isReversed = conf.BoolReverseNames;
              string[] facadeLabel = new string[facadeFilms.Count];
              int[] facadeCounts = new int[facadeFilms.Count];
              // string label2NamePrefix = BaseMesFilms.TranslateColumn(wStrSort);

              try
              {
                #region update facade counts
                for (i = 0; i < facadeFilms.Count; i++)
                {
                  facadeCounts[i] = 0;
                  facadeLabel[i] = isReversed ? ReReverseName(facadeFilms[i].Label) : facadeFilms[i].Label;
                }

                foreach (string value in rtemp.Select(t => t[wStrSort].ToString()))
                {
                  for (i = 0; i < facadeFilms.Count; i++)
                  {
                    if (!value.Contains(facadeLabel[i])) continue;

                    facadeCounts[i]++;
                    facadeFilms[i].Label2 = facadeCounts[i].ToString(); // facadeFilms[i].Label2 = label2NamePrefix + " (" + facadeCounts[i] + ")";
                  }
                  if (StopLoadingViewDetails) return;
                }
                LogMyFilms.Debug("GetCounts() - facade counts finished  (" + stopwatch.ElapsedMilliseconds + " ms)");
                #endregion

                #region get counts (old and disabled)
                //for (i = 0; i < this.facadeFilms.Count; i++)
                //{
                //  string facadelabel = (isReversed) ? ReReverseName(facadeFilms[i].Label) : facadeFilms[i].Label;
                //  int count = rtemp.Count(x => x[wStrSort].ToString().Contains(facadelabel));
                //  facadeFilms[i].Label2 = count.ToString();
                //}

                //foreach (string role in PersonTypes)
                //{
                // if (rtemp != null && (conf.StrPersons.Length > 0 || countItems)) {}
                //  count = rtemp.Count(x => x[role].ToString().Contains(item.Label));
                //  // LogMyFilms.Debug("role: '" + role + "', count: '" + count + "'");
                //  if (count > 0) item.Label2 = (string.IsNullOrEmpty(item.Label2)) ? BaseMesFilms.Translate_Column(role) + " (" + count + ")" : item.Label2 + ", " + BaseMesFilms.Translate_Column(role) + " (" + count + ")";
                //}
                #endregion
              }
              catch (Exception ex)
              {
                LogMyFilms.Warn("GetCounts() - error setting facadelist item '" + i + "': " + ex.Message);
              }
              stopwatch.Stop();
              LogMyFilms.Debug("GetCounts() - Threaded facade details loader exit after '" + i + "' items (" + stopwatch.ElapsedMilliseconds + " ms)");
            }
            #endregion

            #region load actor details for person lists
            Thread.Sleep(25);
            stopwatch.Reset(); stopwatch.Start();
            if (Helper.IsConnectedToInternet() && conf.PersonsEnableDownloads && isperson && conf.StrPersons.Length > 0 && !(conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn && conf.StrViewsShowIndexedImgInIndViews))
            {
              for (i = 0; i < facadeFilms.Count; i++)
              {
                if (StopLoadingViewDetails) return; // stop download if we have exited window
                try
                {
                  GUIListItem item = facadeFilms[i];
                  string personname = conf.BoolReverseNames && item.Label != EmptyFacadeValue ? ReReverseName(item.Label) : item.Label.Replace(EmptyFacadeValue, "");

                  bool success = MyFilmsDetail.UpdatePersonDetails(personname, item, false, StopLoadingViewDetails);
                  if (success) // create small thumbs and assign to facade icons ...
                  {
                    string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson, currentCustomView, defaultViewImage, reversenames);
                    item.IconImage = strActiveFacadeImages[1];
                    item.IconImageBig = strActiveFacadeImages[0];
                    item.ThumbnailImage = strActiveFacadeImages[0];
                    // item.NotifyPropertyChanged("ThumbnailImage");
                  }
                  Thread.Sleep(5); // sleep a bit to let CPU for the GUI

                  #region thumb downloads (no active)

                  // ToDo: Add downloader to SetViewThumbs - or here ...
                  //string remoteThumb = item.ImageRemotePath;
                  //if (string.IsNullOrEmpty(remoteThumb)) continue;

                  //string localThumb = item.Image;
                  //if (string.IsNullOrEmpty(localThumb)) continue;

                  //if (Helper.DownloadFile(remoteThumb, localThumb))
                  //{
                  //  // notify that thumbnail image has been downloaded
                  //  item.ThumbnailImage = localThumb;
                  //  item.NotifyPropertyChanged("ThumbnailImage");
                  //}

                  #endregion
                }
                catch (Exception ex)
                {
                  LogMyFilms.Warn("GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
                  LogMyFilms.Debug(ex, "GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
                }
              }
            }

            stopwatch.Stop();
            LogMyFilms.Debug("GetImages() - Threaded person details loader finished after '" + i + "' items (" + stopwatch.ElapsedMilliseconds + " ms)");
            #endregion
          })
        {
          IsBackground = true,
          Name = "MyFilms Image Detector and Downloader",
          Priority = ThreadPriority.BelowNormal
        }.Start();
    }

    private void UpdatePersonsInFacade(bool showprogressdialog)
    {
      LogMyFilms.Debug("UpdatePersonsInFacade() - starting background updater thread ...");
      StopLoadingViewDetails = false;
      new Thread(delegate(object o)
      {
        try
        {
          if (facadeFilms != null && Helper.IsConnectedToInternet())
          {
            for (int i = 0; i < facadeFilms.Count; i++)
            {
              if (StopLoadingViewDetails) return; // stop download if we have exited window
              try
              {
                GUIListItem item = facadeFilms[i];
                string personname = conf.BoolReverseNames && item.Label != EmptyFacadeValue ? ReReverseName(item.Label) : item.Label.Replace(EmptyFacadeValue, "");

                LogMyFilms.Info("MyFilmsPersonsUpdater - updating person '" + personname + "'");
                bool success = MyFilmsDetail.UpdatePersonDetails(personname, item, showprogressdialog, StopLoadingViewDetails);

                //if (success) // create small thumbs and assign to facade icons ...
                //{
                //  //string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson, currentCustomView, defaultViewImage, reversenames);
                //  //item.IconImage = strActiveFacadeImages[1];
                //  //item.IconImageBig = strActiveFacadeImages[0];
                //  //item.ThumbnailImage = strActiveFacadeImages[0];
                //  //// item.NotifyPropertyChanged("ThumbnailImage");
                //}
                Thread.Sleep(20); // sleep a bit to let CPU for the GUI
              }
              catch (Exception ex)
              {
                LogMyFilms.Warn("UpdatePersonsInFacade() - error setting facadelist item '" + i + "': " + ex.Message);
                LogMyFilms.Debug(ex, "UpdatePersonsInFacade() - error setting facadelist item '" + i + "': " + ex.Message);
              }
            }
          }
          LogMyFilms.Debug("UpdatePersonsInFacade() - Threaded person details loader finished.");
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex, "Thread 'MyFilmsPersonsUpdater' - exception!");
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          // after background thread has finished !
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsPersonsUpdater", IsBackground = true }.Start();
    }

    private void AddAllMoviesToCollection(string groupname)
    {
      LogMyFilms.Debug("AddAllMoviesToCollection() - movies to add: '" + facadeFilms.Count + "'");
      try
      {
        if (facadeFilms != null)
        {
          for (int i = 0; i < facadeFilms.Count; i++)
          {
            try
            {
              GUIListItem item = facadeFilms[i];
              if (!item.IsFolder)
              {
                MyFilmsDetail.AddMovieToCollection(r[item.ItemId], groupname, false);
                LogMyFilms.Info("AddAllMoviesToCollection - adding movie '" + item.Label + "' to collection '" + groupname + "'");
              }
              else
              {
                LogMyFilms.Info("AddAllMoviesToCollection - skip movie '" + item.Label + "' - it is already grouped !");
              }
            }
            catch (Exception ex)
            {
              LogMyFilms.Warn("AddAllMoviesToCollection() - error setting facadelist item '" + i + "': " + ex.Message);
              LogMyFilms.Debug(ex, "AddAllMoviesToCollection() - error setting facadelist item '" + i + "': " + ex.Message);
            }
          }
          MyFilmsDetail.Update_XML_database();
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex, "Thread 'AddAllMoviesToCollection' - exception!");
      }
    }

    private void RemoveAllMoviesFromCollection()
    {
      LogMyFilms.Debug("RemoveAllMoviesFromCollection() - movies to remove: '" + facadeFilms.Count + "'");
      try
      {
        if (facadeFilms != null)
        {
          for (int i = 0; i < facadeFilms.Count; i++)
          {
            try
            {
              GUIListItem item = facadeFilms[i];
              if (!item.IsFolder)
              {
                MyFilmsDetail.RemoveMovieFromCollection(r[item.ItemId], false);
                LogMyFilms.Info("RemoveAllMoviesFromCollection() - removing movie '" + item.Label + "'");
              }
              else
              {
                LogMyFilms.Info("RemoveAllMoviesFromCollection() - skip entry '" + item.Label + "' - it is a sub-group !");
              }
            }
            catch (Exception ex)
            {
              LogMyFilms.Warn("RemoveAllMoviesFromCollection() - error setting facadelist item '" + i + "': " + ex.Message);
              LogMyFilms.Debug(ex, "RemoveAllMoviesFromCollection() - error setting facadelist item '" + i + "': " + ex.Message);
            }
          }
          MyFilmsDetail.Update_XML_database();
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex, "Thread 'RemoveAllMoviesFromCollection' - exception!");
      }
    }

    private void UpdatePersons(List<string> persons, bool showprogressdialog)
    {
      LogMyFilms.Debug("UpdatePersons() - persons to update: '" + persons.Count + "'");
      new Thread(delegate(object o)
      {
        try
        {
          List<string> wTableau = (List<string>)o;
          GUIDialogProgress dlgPrgrs = showprogressdialog ? (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS) : null;
          if (dlgPrgrs != null)
          {
            dlgPrgrs.Reset();
            dlgPrgrs.DisplayProgressBar = true;
            dlgPrgrs.ShowWaitCursor = false;
            dlgPrgrs.DisableCancel(true);
            dlgPrgrs.SetHeading("MyFilms Person Updates");
            dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
            dlgPrgrs.Percentage = 0;

            int i = 0;
            foreach (string personsname in wTableau)
            {
              if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "Loading Updates for '" + personsname + "'");
              if (dlgPrgrs != null) dlgPrgrs.Percentage = i*100/wTableau.Count;
              LogMyFilms.Info("MyFilmsPersonsUpdater - updating person '" + personsname + "'");
              bool success = MyFilmsDetail.UpdatePersonDetails(personsname, null, true, false);
              i++;
            }
          }
          if (dlgPrgrs != null)
          {
            dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); dlgPrgrs.SetLine(2, ""); Thread.Sleep(50); dlgPrgrs.Close(); // Done...
          }
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex, "Thread 'MyFilmsPersonsUpdater' - exception!");
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => 0, 0, 0, null);
      }) { Name = "MyFilmsPersonsUpdater", IsBackground = true }.Start(persons);
    }

    private void GetImagesOld(List<GUIListItem> itemsWithThumbs, string wStrSort, string strThumbDirectory, bool isperson, bool getThumbs, bool createFanartDir, bool countItems)
    {
      //StopLoadingViewDetails = false;

      //var groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // split the downloads in X+ groups and do multithreaded downloading // Guzzi: Set group to x to only allow x thread(s)
      //var groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

      //for (int i = 0; i < groups; i++)
      //{
      //  var groupList = new List<GUIListItem>();
      //  for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
      //  {
      //    groupList.Add(itemsWithThumbs[j]);
      //  }

      //  new Thread(delegate(object o)
      //  {
      //    #region taskaction
      //    var items = (List<GUIListItem>)o;
      //    Thread.Sleep(25);
      //    DataRow[] rtemp = null;
      //    if (conf.StrPersons.Length > 0) // reading full dataset only required, if personcounts are requested...
      //      rtemp = BaseMesFilms.ReadDataMovies(GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + conf.StrDfltSelect, "", wStrSort, conf.wStrSortSens);
      //    // DataRow[] rtemp = r;

      //    foreach (GUIListItem item in items)
      //    {
      //      // stop download if we have exited window
      //      if (StopLoadingViewDetails)
      //        break;

      //      if (getThumbs)
      //      {
      //        string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson);
      //        //string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";

      //        //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
      //        //{
      //        //  item.ThumbnailImage = texture;
      //        //  item.IconImage = texture;
      //        //  item.IconImageBig = texture;
      //        //}
      //        item.ThumbnailImage = strActiveFacadeImages[0];
      //        item.IconImage = strActiveFacadeImages[1];
      //        item.IconImageBig = strActiveFacadeImages[0];

      //        // if selected force an update of thumbnail
      //        //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
      //        //if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
      //      }

      //      if (createFanartDir) MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, wStrSort.ToLower());

      //      if (conf.StrPersons.Length > 0 && rtemp != null)
      //      {
      //        int count = rtemp.Count(x => x[wStrSort].ToString().IndexOf(item.Label, StringComparison.OrdinalIgnoreCase) > 0);
      //        if (count > 0) item.Label2 = BaseMesFilms.Translate_Column(wStrSort) + " (" + count + ")"; // LogMyFilms.Debug("role: '" + wStrSort + "', count: '" + count + "'");

      //        //foreach (string role in PersonTypes)
      //        //{
      //        //  count = rtemp.Count(x => x[role].ToString().Contains(item.Label));
      //        //  // LogMyFilms.Debug("role: '" + role + "', count: '" + count + "'");
      //        //  if (count > 0) item.Label2 = (string.IsNullOrEmpty(item.Label2)) ? BaseMesFilms.Translate_Column(role) + " (" + count + ")" : item.Label2 + ", " + BaseMesFilms.Translate_Column(role) + " (" + count + ")";
      //        //}
      //      }

      //      // ToDo: Add downloader to SetViewThumbs - or here ...

      //      //string remoteThumb = item.ImageRemotePath;
      //      //if (string.IsNullOrEmpty(remoteThumb)) continue;

      //      //string localThumb = item.Image;
      //      //if (string.IsNullOrEmpty(localThumb)) continue;

      //      //if (Helper.DownloadFile(remoteThumb, localThumb))
      //      //{
      //      //  // notify that thumbnail image has been downloaded
      //      //  item.ThumbnailImage = localThumb;
      //      //  item.NotifyPropertyChanged("ThumbnailImage");
      //      //}
      //    }
      //    items.SafeDispose();
      //    items = null;
      //    #endregion
      //    return;
      //  })
      //  {
      //    IsBackground = true,
      //    Name = "MyFilms Image Detector and Downloader" + i,
      //    Priority = ThreadPriority.BelowNormal
      //  }.Start(groupList);
      //}
    }

    private string[] SetViewThumbs(string wStrSort, string itemlabel, string strThumbDirectory, bool isPerson, MFview.ViewRow currentCustomView, string defaultViewImage, bool reversenames)
    {
      if (isPerson && reversenames) itemlabel = ReReverseName(itemlabel);

      string[] thumbimages = new string[2];
      thumbimages[0] = string.Empty; // ThumbnailImage
      thumbimages[1] = string.Empty; //IconImage
      string strThumb = strThumbDirectory + itemlabel + ".png";
      string strThumbSource = string.Empty;

      if (isPerson)
      #region thumbs for person views
      {
        //strThumbSource = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieActors, itemlabel); // check for actors images in MyVideos...
        //LogMyFilms.Debug("Artist thumbs - GetCoverName(Thumbs, MovieActors) - strThumb = '" + strThumb + "'");
        //if (!System.IO.File.Exists(strThumb))
        //  strThumb = strThumbDirectory + itemlabel + ".png";
        //else
        //  return strThumb;

        if (File.Exists(strThumb)) // If there is thumbs in cache folder ...
        {
          thumbimages[0] = strThumb;
          thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
          return thumbimages;
        }
        if (conf.StrPathArtist.Length > 0)
        {
          string strPathArtist;
          if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\") strPathArtist = conf.StrPathArtist;
          else strPathArtist = conf.StrPathArtist + "\\";
          if (File.Exists(strPathArtist + itemlabel + "\\folder.jpg")) strThumbSource = strPathArtist + itemlabel + "\\folder.jpg";
          else if (File.Exists(strPathArtist + itemlabel + "\\folder.png")) strThumbSource = strPathArtist + itemlabel + "\\folder.png";
          else if (File.Exists(strPathArtist + itemlabel + "L" + ".jpg")) strThumbSource = strPathArtist + itemlabel + "L" + ".jpg";
          else if (File.Exists(strPathArtist + itemlabel + ".jpg")) strThumbSource = strPathArtist + itemlabel + ".jpg";
          else if (File.Exists(strPathArtist + itemlabel + ".png")) strThumbSource = strPathArtist + itemlabel + ".png";
          else
            #region perform catalog specific searches ...
            switch (conf.StrFileType)
            {
              case Configuration.CatalogType.AntMovieCatalog3:
                break;
              case Configuration.CatalogType.AntMovieCatalog4Xtended:
                break;
              case Configuration.CatalogType.DVDProfiler:
                break;
              case Configuration.CatalogType.MovieCollector:
                break;
              case Configuration.CatalogType.MyMovies:
                break;
              case Configuration.CatalogType.EaxMovieCatalog2:
              case Configuration.CatalogType.EaxMovieCatalog3:
                if (File.Exists(strPathArtist + itemlabel.Replace(" ", ".") + ".jpg")) strThumbSource = strPathArtist + itemlabel.Replace(" ", ".") + ".jpg";
                break;
              case Configuration.CatalogType.PersonalVideoDatabase: // PVD artist thumbs: e.g. Natalie Portman_1.jpg , then Natalie Portman_2.jpg 
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in PVD picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel); // replaces special character "á" and other special chars !
                  searchname = searchname + "*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Any())
                    strThumbSource = files[0];
                }
                break;
              case Configuration.CatalogType.eXtremeMovieManager: // XMM artist thumbs: e.g. Alex-Revan_101640.jpg
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in XMM picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel); // replaces special character "á" and other special chars !
                  searchname = searchname.Replace(" ", "-");
                  searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Any())
                    strThumbSource = files[0];
                }
                break;
              case Configuration.CatalogType.XBMC:
                break;
              case Configuration.CatalogType.MovingPicturesXML:
                break;
              case Configuration.CatalogType.XBMCnfoReader:
                break;
            }
            #endregion

          //if (strThumbSource == string.Empty)
          //{
          //  strThumbSource = MyFilmsDetail.CreateOrUpdateActor(itemlabel);
          //}

          if (strThumbSource != string.Empty)
          {
            //Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
            CreateCacheThumb(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, "small");
            //Picture.CreateThumbnail(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
            CreateCacheThumb(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, "large");
            //thumbimages[0] = strThumbSource;
            //thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
            //return thumbimages;

            if (File.Exists(strThumb)) // (re)check if thumbs exist...
            {
              thumbimages[0] = strThumb;
              thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
              return thumbimages;
            }
          }
        }

        //if ((!System.IO.File.Exists(strThumbLarge)) && (strThumbLarge != conf.DefaultCoverArtist) && (strThumbSource != string.Empty))
        //{
        //  Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "L.png", cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
        //  strThumbLarge = strThumbDirectory + itemlabel + "L.png";
        //}

        // if (!System.IO.File.Exists(strThumb) && conf.StrArtistDflt && conf.DefaultCoverArtist.Length > 0)

        // Check, if default person cover is present
        if (conf.StrArtistDflt)
        {
          string strPathArtist = conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\"
                                   ? conf.StrPathArtist
                                   : conf.StrPathArtist + "\\";

          //ImageFast.CreateImage(strThumb, item.Label); // this is to create a pseudo cover with name of label added to it
          //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
          //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);

          // if there is a Default.jpg in the view subfolder
          if (defaultViewImage != null)
          {
            thumbimages[0] = defaultViewImage;
            thumbimages[1] = defaultViewImage;
            return thumbimages;
          }

          // if there is an image defined in Custom View
          if (currentCustomView != null)
          {
            thumbimages[0] = currentCustomView.ImagePath;
            thumbimages[1] = currentCustomView.ImagePath;
            return thumbimages;
          }

          // Otherwise use Default image
          if (conf.DefaultCoverArtist.Length > 0)
          {
            thumbimages[0] = conf.DefaultCoverArtist;
            thumbimages[1] = conf.DefaultCoverArtist;
            return thumbimages;
          }
          thumbimages[0] = "";
          thumbimages[1] = "";
          return thumbimages;
        }
        else
        {
          thumbimages[0] = "";
          thumbimages[1] = "";
          return thumbimages;
        }
      }
      #endregion
      else if (conf.StrViewsDfltAll || IsCategoryYearCountryField(wStrSort))
      {
        if (File.Exists(strThumb)) // If there is thumbs in cache folder ...
        {
          thumbimages[0] = strThumb;
          thumbimages[1] = strThumb;
          return thumbimages;
        }

        if (conf.StrPathViews.Length > 0)
        {
          string strPathViews = conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\" ? conf.StrPathViews : conf.StrPathViews + "\\";
          string strPathViewsRoot = strPathViews;
          strPathViews = strPathViews + wStrSort.ToLower() + "\\"; // added view subfolder to searchpath
          if (File.Exists(strPathViews + itemlabel + ".jpg"))
            CreateCacheThumb(strPathViews + itemlabel + ".jpg", strThumb, cacheThumbWith, cacheThumbHeight, "large");
          else if (File.Exists(strPathViews + itemlabel + ".png"))
            CreateCacheThumb(strPathViews + itemlabel + ".png", strThumb, cacheThumbWith, cacheThumbHeight, "large");
          if (File.Exists(strThumb))
          {
            thumbimages[0] = strThumb;
            thumbimages[1] = strThumb;
            return thumbimages;
          }

          // Check, if default group cover is present
          if (conf.StrViewsDflt)
          {
            // if there is a Default.jpg in the view subfolder
            if (defaultViewImage != null)
            {
              thumbimages[0] = defaultViewImage;
              thumbimages[1] = defaultViewImage;
              return thumbimages;
            }
            // if there is an image defined in Custom View
            if (currentCustomView != null && !string.IsNullOrEmpty(currentCustomView.ImagePath))
            {
              thumbimages[0] = currentCustomView.ImagePath;
              thumbimages[1] = currentCustomView.ImagePath;
              return thumbimages;
            }

            // Otherwise use Default image
            if (File.Exists(strPathViews + "Default.jpg"))
            {
              thumbimages[0] = strPathViews + "Default.jpg";
              thumbimages[1] = strPathViews + "Default.jpg";
              return thumbimages;
            }
          }
        }

        // Use Default Cover if no specific Cover found:
        //  if (MyFilms.conf.StrViewsDflt && System.IO.File.Exists(MyFilms.conf.DefaultCover))
        if (conf.StrViewsDflt && (conf.DefaultCoverViews.Length > 0))
        {
          //ImageFast.CreateImage(strThumb, item.Label); // Disabled "old" method to use Defaultcover with embedded text of selected item ...
          //Picture.CreateThumbnail(strThumbLarge, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
          thumbimages[0] = conf.DefaultCoverViews;
          thumbimages[1] = conf.DefaultCoverViews;
          return thumbimages;
        }
      }
      return thumbimages;
    }

    internal static void CreateCacheThumb(string thumbSource, string thumbTarget, int thumbWidth, int thumbHeight, string speedThumbSize)
    {
      if (thumbSource == null) return;

      try
      {
        Bitmap bmp = new Bitmap(thumbSource);
        LogMyFilms.Debug("(SetViewThumb) - Image Width x Height = '" + bmp.Width + "x" + bmp.Height + "' (" + thumbSource + ")");
        if (bmp.Width < thumbWidth && bmp.Height < thumbHeight)
        {
          if (!SaveThumbnailFile(thumbSource, thumbTarget)) // if copy unsuccessful, try to create speedthumb
          {
            Picture.CreateThumbnail(
              thumbSource,
              thumbTarget,
              thumbWidth,
              thumbHeight,
              0,
              speedThumbSize == "small" ? Thumbs.SpeedThumbsSmall : Thumbs.SpeedThumbsLarge);
          }
        }
        else if (speedThumbSize == "small") Picture.CreateThumbnail(thumbSource, thumbTarget, thumbWidth, thumbHeight, 0, Thumbs.SpeedThumbsSmall);
        else Picture.CreateThumbnail(thumbSource, thumbTarget, thumbWidth, thumbHeight, 0, Thumbs.SpeedThumbsLarge);
        bmp.SafeDispose();
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("CreateCacheThumb() - Error creating thumb file: {0}", ex.Message);
      }
    }

    private static bool SaveThumbnailFile(string thumbSource, string thumbTarget)
    {
      try
      {
        File.Copy(thumbSource, thumbTarget, true);

        //FileStream fs = new FileStream(ThumbSource, FileMode.Open, FileAccess.Read, FileShare.Read);	
        //BinaryReader reader = new BinaryReader(fs);
        //byte[] bytes = new byte[fs.Length];
        //int read;
        //reader.Read(bytes, 0, bytes.Length)	
        //reader.Close(); 
        //fs.Close();

        //using (FileStream fs = new FileStream(ThumbTarget, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        //{
        //  using (FileStream fs = new FileStream(ThumbTarget, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        //  {
        //    bmp.Save(fs, Thumbs.ThumbCodecInfo, Thumbs.ThumbEncoderParams);
        //  }
        //  fs.Flush();
        //}

        File.SetAttributes(thumbTarget, File.GetAttributes(thumbTarget) | FileAttributes.Hidden);
        // even if run in background thread wait a little so the main process does not starve on IO
        Thread.Sleep(MediaPortal.Player.g_Player.Playing ? 100 : 1);
        return true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("SaveThumbnailFile() - Error saving new thumbnail {0} - {1}", thumbTarget, ex.Message);
        return false;
      }
    }

    #region Sorter and Comparer Classes
    //----------------------------------------------------------------------------------------------
    //  Normal Sort
    //----------------------------------------------------------------------------------------------
    static int NormalCompare(object x, object y)
    {
      return string.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase); // StringComparison.CurrentCultureIgnoreCase
    }


    //----------------------------------------------------------------------------------------------
    //  Forward Sort
    //----------------------------------------------------------------------------------------------
    public class myForwarderClass : IComparer
    {
      // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
      int IComparer.Compare(Object x, Object y)
      {
        //CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth | CompareOptions.IgnoreNonSpace;
        //string.Compare(strA, strB, CultureInfo.CurrentCulture, compareOptions) == 0
        //int i = String.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
        //if (i == 0) 
        //  return String.CompareOrdinal(x.ToString(), y.ToString());
        //else
        //  return i;  // return ((new CaseInsensitiveComparer(CultureInfo.InvariantCulture)).Compare(x, y));
        return String.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Reverse Sort
    //----------------------------------------------------------------------------------------------
    public class myReverserClass : IComparer
    {
      // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
      int IComparer.Compare(Object x, Object y)
      {
        return String.Compare(y.ToString(), x.ToString(), StringComparison.OrdinalIgnoreCase);  // return String.CompareOrdinal(y.ToString(), x.ToString());  //return ((new CaseInsensitiveComparer()).Compare(y, x));
      }
    }

    static int CompareCount(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
    {
      if (a.Value == b.Value) return a.Key.CompareTo(b.Key);
      return a.Value.CompareTo(b.Value);
    }

    static int CompareCountDescending(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
    {
      if (b.Value == a.Value) return b.Key.CompareTo(a.Key); // alphabetical here ...
      return b.Value.CompareTo(a.Value);
    }

    //----------------------------------------------------------------------------------------------
    //  GUIListItem Sort
    //----------------------------------------------------------------------------------------------
    public class GUIListLabelComparer : IComparer<GUIListItem>
    {
      public int Compare(GUIListItem x, GUIListItem y)
      {
        return x.Label.CompareTo(y.Label);
      }
    }

    public class GUIListLabel2Comparer : IComparer<GUIListItem>
    {
      public int Compare(GUIListItem x, GUIListItem y)
      {
        return x.Label2.CompareTo(y.Label2);
      }
    }

    private class MyDataRowComparer : IComparer<DataRow>
    {
      public int Compare(DataRow x, DataRow y)
      {
        return String.Compare(x.Field<string>("c1"), y.Field<string>("c1"));
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Sort for translated DB field lists
    //----------------------------------------------------------------------------------------------
    public class myDBfieldComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        int result;
        string[] s1 = x as string[];
        if (s1 == null) return 0;
        string[] s2 = y as string[];
        if (s2 == null) return 0;
        try
        {
          result = String.Compare(s1[1], s2[1], true);
        }
        catch
        {
          result = String.Compare(s1[0], s2[0], true);
        }
        return result;
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Date Sort for string types
    //----------------------------------------------------------------------------------------------
    public class myDateComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        int result;
        string s1 = x as string;
        string s2 = y as string;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        if (s1 == null)
          return s2 == null ? 0 : 1;
        if (s2 == null)
          return -1;

        try
        {
          result = Convert.ToDateTime(s1).CompareTo(Convert.ToDateTime(s2));
          // result = DateTime.Parse(s1).CompareTo(DateTime.Parse(s2));
        }
        catch // compare as strings, if no conversion possible ...
        {
          result = s1.CompareTo(s2);
        }
        return result;
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Reverse Date Sort for string types
    //----------------------------------------------------------------------------------------------
    public class myDateReverseComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        int result;
        string s1 = x as string;
        string s2 = y as string;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        if (s1 == null)
          return s2 == null ? 0 : 1;
        if (s2 == null)
          return -1;

        try
        {
          result = Convert.ToDateTime(s2).CompareTo(Convert.ToDateTime(s1));
          // result = DateTime.Parse(s1).CompareTo(DateTime.Parse(s2));
        }
        catch // compare as strings, if no conversion possible ...
        {
          result = s2.CompareTo(s1);
        }
        return result;
        // return (new myDateComparer()).Compare(y, x);
      }
    }

    public sealed class NullsLastComparer<T> : Comparer<T>
    {
      private readonly IComparer<T> _comparer;

      public NullsLastComparer() : this(null) { }

      public NullsLastComparer(IComparer<T> comparer)
      {
        _comparer = comparer ?? Default;
      }

      public override int Compare(T x, T y)
      {
        if (x == null)
          return y == null ? 0 : 1;

        if (y == null)
          return -1;

        return _comparer.Compare(x, y);
      }
    }

    ////----------------------------------------------------------------------------------------------
    ////  Rating Sort for decimal types
    ////----------------------------------------------------------------------------------------------
    //public class myRatingComparer : IComparer
    //{
    //  public int Compare(object x, object y)
    //  {
    //    int result;
    //    decimal s1 = x as Decimal;
    //    int s2 = y as string;

    //    if (s1 == null) return 0;
    //    if (s2 == null) return 0;

    //    //if (s1 == null)
    //    //  return (s2 == null) ? 0 : 1;
    //    //if (s2 == null)
    //    //  return -1;

    //    try
    //    {
    //      result = Convert.ToDateTime(s1).CompareTo(Convert.ToDateTime(s2));
    //      // result = DateTime.Parse(s1).CompareTo(DateTime.Parse(s2));
    //    }
    //    catch // compare as strings, if no conversion possible ...
    //    {
    //      // result = s1.CompareTo(s2);
    //      result = 0;
    //    }
    //    return result;
    //  }
    //}

    public class myRatingComparer : IComparer
    {

      // int IComparer.Compare(string a, string b)
      public int Compare(object a, object b)
      {
        decimal aDec;
        decimal bDec;
        if (decimal.TryParse(a.ToString(), out aDec) && decimal.TryParse(b.ToString(), out bDec))
        {
          return aDec.CompareTo(bDec);
        }
        else
        {
          // return a.ToString().CompareTo(b.ToString());
          return new AlphanumComparatorFast().Compare(a, b);
        }
      }

      //public static IComparer NumericStringSorter()
      //{
      //  return (IComparer)new myRatingComparer();
      //}
    }

    public class myRatingReverseComparer : IComparer
    {

      // int IComparer.Compare(string a, string b)
      public int Compare(object a, object b)
      {
        decimal aDec;
        decimal bDec;
        if (decimal.TryParse(b.ToString(), out aDec) && decimal.TryParse(a.ToString(), out bDec))
        {
          return bDec.CompareTo(aDec);
        }
        else
        {
          // return a.ToString().CompareTo(b.ToString());
          return new AlphanumComparatorFast().Compare(b, a);
        }
      }

      //public static IComparer NumericStringSorter()
      //{
      //  return (IComparer)new myRatingComparer();
      //}
    }

    //----------------------------------------------------------------------------------------------
    //  Date Sort for string types based on non DateTime Conversion
    //----------------------------------------------------------------------------------------------
    public class myDateStringComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        var s1 = x as string;
        if (s1 == null) return 0;
        var s2 = y as string;
        if (s2 == null) return 0;

        string date1 = string.Format("{0:dd/MM/yyyy}", s1);
        string date2 = string.Format("{0:dd/MM/yyyy}", s2);
        return date1.CompareTo(date2);
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Reverse Alphanumeric Sort
    //----------------------------------------------------------------------------------------------
    public class myoldReverserAlphanumComparatorFast : IComparer // Calls AlphanumComparatorFast.Compare with the parameters reversed.
    {
      int IComparer.Compare(Object x, Object y)
      {
        return new AlphanumComparatorFast().Compare(y, x);
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Alphanumeric Sort
    //----------------------------------------------------------------------------------------------
    public class AlphanumComparatorFast : IComparer
    {
      public int Compare(object x, object y)
      {
        //if (x == null)
        //  return (y == null) ? 0 : 1;
        //if (y == null)
        //  return -1;

        string s1 = x as string;
        string s2 = y as string;

        if (s1 == null)
          return s2 == null ? 0 : 1;
        if (s2 == null)
          return -1;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
          char ch1 = s1[marker1];
          char ch2 = s2[marker2];

          // Some buffers we can build up characters in for each chunk.
          char[] space1 = new char[len1];
          int loc1 = 0;
          char[] space2 = new char[len2];
          int loc2 = 0;

          // Walk through all following characters that are digits or
          // characters in BOTH strings starting at the appropriate marker.
          // Collect char arrays.
          do
          {
            space1[loc1++] = ch1;
            marker1++;

            if (marker1 < len1)
            {
              ch1 = s1[marker1];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

          do
          {
            space2[loc2++] = ch2;
            marker2++;

            if (marker2 < len2)
            {
              ch2 = s2[marker2];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

          // If we have collected numbers, compare them numerically.
          // Otherwise, if we have strings, compare them alphabetically.
          string str1 = new string(space1);
          string str2 = new string(space2);

          int result;

          if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
          {
            int thisNumericChunk = int.Parse(str1);
            int thatNumericChunk = int.Parse(str2);
            result = thisNumericChunk.CompareTo(thatNumericChunk);
          }
          else
          {
            result = str1.CompareTo(str2);
          }

          if (result != 0)
          {
            return result;
          }
        }
        return len1 - len2;
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Alphanumeric Sort for strings
    //----------------------------------------------------------------------------------------------
    public class AlphanumComparatorFastString : IComparer<string>
    {
      public int Compare(string x, string y)
      {
        //if (x == null)
        //  return (y == null) ? 0 : 1;
        //if (y == null)
        //  return -1;

        string s1 = x;
        string s2 = y;

        if (s1 == null)
          return s2 == null ? 0 : 1;
        if (s2 == null)
          return -1;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
          char ch1 = s1[marker1];
          char ch2 = s2[marker2];

          // Some buffers we can build up characters in for each chunk.
          char[] space1 = new char[len1];
          int loc1 = 0;
          char[] space2 = new char[len2];
          int loc2 = 0;

          // Walk through all following characters that are digits or
          // characters in BOTH strings starting at the appropriate marker.
          // Collect char arrays.
          do
          {
            space1[loc1++] = ch1;
            marker1++;

            if (marker1 < len1)
            {
              ch1 = s1[marker1];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

          do
          {
            space2[loc2++] = ch2;
            marker2++;

            if (marker2 < len2)
            {
              ch2 = s2[marker2];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

          // If we have collected numbers, compare them numerically.
          // Otherwise, if we have strings, compare them alphabetically.
          string str1 = new string(space1);
          string str2 = new string(space2);

          int result;

          if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
          {
            int thisNumericChunk = int.Parse(str1);
            int thatNumericChunk = int.Parse(str2);
            result = thisNumericChunk.CompareTo(thatNumericChunk);
          }
          else
          {
            result = str1.CompareTo(str2);
          }

          if (result != 0)
          {
            return result;
          }
        }
        return len1 - len2;
      }
    }

    public class myReverserAlphanumComparatorFast : IComparer
    {
      public int Compare(object x, object y)
      {
        //if (x == null)
        //  return (y == null) ? 0 : 1;
        //if (y == null)
        //  return -1;

        string s1 = y as string;
        string s2 = x as string;

        if (s2 == null)
          return s1 == null ? 0 : 1;
        if (s1 == null)
          return -1;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
          char ch1 = s1[marker1];
          char ch2 = s2[marker2];

          // Some buffers we can build up characters in for each chunk.
          char[] space1 = new char[len1];
          int loc1 = 0;
          char[] space2 = new char[len2];
          int loc2 = 0;

          // Walk through all following characters that are digits or
          // characters in BOTH strings starting at the appropriate marker.
          // Collect char arrays.
          do
          {
            space1[loc1++] = ch1;
            marker1++;

            if (marker1 < len1)
            {
              ch1 = s1[marker1];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

          do
          {
            space2[loc2++] = ch2;
            marker2++;

            if (marker2 < len2)
            {
              ch2 = s2[marker2];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

          // If we have collected numbers, compare them numerically.
          // Otherwise, if we have strings, compare them alphabetically.
          string str1 = new string(space1);
          string str2 = new string(space2);

          int result;

          if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
          {
            int thisNumericChunk = int.Parse(str1);
            int thatNumericChunk = int.Parse(str2);
            result = thisNumericChunk.CompareTo(thatNumericChunk);
          }
          else
          {
            result = str1.CompareTo(str2);
          }

          if (result != 0)
          {
            return result;
          }
        }
        return len1 - len2;
      }
    }

    public class AlphanumComparatorFastForList : IComparer
    {
      List<string> GetList(string s1)
      {
        List<string> SB1 = new List<string>();
        string st1, st2, st3;
        st1 = "";
        bool flag = char.IsDigit(s1[0]);
        foreach (char c in s1)
        {
          if (flag != char.IsDigit(c) || c == '\'')
          {
            if (st1 != "")
              SB1.Add(st1);
            st1 = "";
            flag = char.IsDigit(c);
          }
          if (char.IsDigit(c))
          {
            st1 += c;
          }
          if (char.IsLetter(c))
          {
            st1 += c;
          }


        }
        SB1.Add(st1);
        return SB1;
      }



      public int Compare(object x, object y)
      {
        string s1 = x as string;
        if (s1 == null)
        {
          return 0;
        }
        string s2 = y as string;
        if (s2 == null)
        {
          return 0;
        }
        if (s1 == s2)
        {
          return 0;
        }
        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        List<string> str1 = GetList(s1);
        List<string> str2 = GetList(s2);
        while (str1.Count != str2.Count)
        {
          if (str1.Count < str2.Count)
          {
            str1.Add("");
          }
          else
          {
            str2.Add("");
          }
        }
        int x1 = 0; int res = 0; int x2 = 0; string y2 = "";
        bool status = false;
        string y1 = ""; bool s1Status = false; bool s2Status = false;
        //s1status ==false then string ele int;
        //s2status ==false then string ele int;
        int result = 0;
        for (int i = 0; i < str1.Count && i < str2.Count; i++)
        {
          status = int.TryParse(str1[i].ToString(), out res);
          if (res == 0)
          {
            y1 = str1[i].ToString();
            s1Status = false;
          }
          else
          {
            x1 = Convert.ToInt32(str1[i].ToString());
            s1Status = true;
          }

          status = int.TryParse(str2[i].ToString(), out res);
          if (res == 0)
          {
            y2 = str2[i].ToString();
            s2Status = false;
          }
          else
          {
            x2 = Convert.ToInt32(str2[i].ToString());
            s2Status = true;
          }
          //checking --the data comparision
          if (!s2Status && !s1Status)    //both are strings
          {
            result = str1[i].CompareTo(str2[i]);
          }
          else if (s2Status && s1Status) //both are intergers
          {
            if (x1 == x2)
            {
              if (str1[i].ToString().Length < str2[i].ToString().Length)
              {
                result = 1;
              }
              else if (str1[i].ToString().Length > str2[i].ToString().Length)
                result = -1;
              else
                result = 0;
            }
            else
            {
              int st1ZeroCount = str1[i].ToString().Trim().Length - str1[i].ToString().TrimStart(new char[] { '0' }).Length;
              int st2ZeroCount = str2[i].ToString().Trim().Length - str2[i].ToString().TrimStart(new char[] { '0' }).Length;
              if (st1ZeroCount > st2ZeroCount)
                result = -1;
              else if (st1ZeroCount < st2ZeroCount)
                result = 1;
              else
                result = x1.CompareTo(x2);

            }
          }
          else
          {
            result = str1[i].CompareTo(str2[i]);
          }
          if (result == 0)
          {
            continue;
          }
          else
            break;

        }
        return result;
      }
    }

    public class AlphanumComparator : IComparer
    {
      private enum ChunkType { Alphanumeric, Numeric };
      private bool InChunk(char ch, char otherCh)
      {
        ChunkType type = ChunkType.Alphanumeric;

        if (char.IsDigit(otherCh))
        {
          type = ChunkType.Numeric;
        }

        if ((type == ChunkType.Alphanumeric && char.IsDigit(ch)) || (type == ChunkType.Numeric && !char.IsDigit(ch)))
        {
          return false;
        }

        return true;
      }

      public int Compare(object x, object y)
      {
        String s1 = x as string;
        String s2 = y as string;
        if (s1 == null || s2 == null)
        {
          return 0;
        }

        int thisMarker = 0, thisNumericChunk = 0;
        int thatMarker = 0, thatNumericChunk = 0;

        while ((thisMarker < s1.Length) || (thatMarker < s2.Length))
        {
          if (thisMarker >= s1.Length)
          {
            return -1;
          }
          else if (thatMarker >= s2.Length)
          {
            return 1;
          }
          char thisCh = s1[thisMarker];
          char thatCh = s2[thatMarker];

          StringBuilder thisChunk = new StringBuilder();
          StringBuilder thatChunk = new StringBuilder();

          while ((thisMarker < s1.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
          {
            thisChunk.Append(thisCh);
            thisMarker++;

            if (thisMarker < s1.Length)
            {
              thisCh = s1[thisMarker];
            }
          }

          while ((thatMarker < s2.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
          {
            thatChunk.Append(thatCh);
            thatMarker++;

            if (thatMarker < s2.Length)
            {
              thatCh = s2[thatMarker];
            }
          }

          int result = 0;
          // If both chunks contain numeric characters, sort them numerically
          if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
          {
            thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
            thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

            if (thisNumericChunk < thatNumericChunk)
            {
              result = -1;
            }

            if (thisNumericChunk > thatNumericChunk)
            {
              result = 1;
            }
          }
          else
          {
            result = thisChunk.ToString().CompareTo(thatChunk.ToString());
          }

          if (result != 0)
          {
            return result;
          }
        }

        return 0;
      }
    }

    public class AlphanumComparatorFastNew : IComparer
    {
      public int Compare(object x, object y)
      {
        string s1 = x as string;
        if (s1 == null)
        {
          return 0;
        }
        string s2 = y as string;
        if (s2 == null)
        {
          return 0;
        }

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
          char ch1 = s1[marker1];
          char ch2 = s2[marker2];

          // Some buffers we can build up characters in for each chunk.
          char[] space1 = new char[len1];
          int loc1 = 0;
          char[] space2 = new char[len2];
          int loc2 = 0;

          // Walk through all following characters that are digits or
          // characters in BOTH strings starting at the appropriate marker.
          // Collect char arrays.
          do
          {
            space1[loc1++] = ch1;
            marker1++;

            if (marker1 < len1)
            {
              ch1 = s1[marker1];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

          do
          {
            space2[loc2++] = ch2;
            marker2++;

            if (marker2 < len2)
            {
              ch2 = s2[marker2];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

          // If we have collected numbers, compare them numerically.
          // Otherwise, if we have strings, compare them alphabetically.
          string str1 = new string(space1);
          string str2 = new string(space2);

          int result;

          if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
          {
            int thisNumericChunk = int.Parse(str1);
            int thatNumericChunk = int.Parse(str2);
            result = thisNumericChunk.CompareTo(thatNumericChunk);
          }
          else
          {
            result = str1.CompareTo(str2);
          }

          if (result != 0)
          {
            return result;
          }
        }
        return len1 - len2;
      }
    }

    #endregion

    private static bool Load_Config(string currentConfig, bool createTemp, LoadParameterInfo loadParams)
    {
      Stopwatch stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      string oldXmlDb = conf != null ? conf.StrFileXml : "";
      conf = new Configuration(currentConfig, true, createTemp, loadParams);
      conf.IsDbReloadRequired = oldXmlDb != conf.StrFileXml;
      // set reload flag, if the underlying DB has changed (it might not, if user switches config using the same DB)
      stopwatch.Stop();
      LogMyFilms.Debug("Load_Config(): Finished loading config '" + currentConfig + "' (" + stopwatch.ElapsedMilliseconds + " ms)");

      // check, if the file is a valid DB type
      bool valid = BaseMesFilms.IsValidDb(conf.StrFileXml);
      if (!valid)
        return false;

      if (conf.Boolreturn && conf.Wselectedlabel == string.Empty)
      {
        conf.Boolselect = true;
        conf.Boolreturn = false;
      }
      if (conf.StrLogos)
        confLogos = new Logos();
      MyFilmsDetail.setGUIProperty("config.currentconfig", currentConfig);
      if (conf.EnhancedWatchedStatusHandling)
      {
        MyFilmsDetail.setGUIProperty("user.name.value", conf.StrUserProfileName != DefaultUsername ? conf.StrUserProfileName : "");
      }
      else
      {
        MyFilmsDetail.setGUIProperty("user.name.value", "");
      }

      if (conf.StrDfltSelect.Length > 0)
      {
        string userfilter = conf.StrDfltSelect.Substring(0, conf.StrDfltSelect.LastIndexOf("AND", StringComparison.Ordinal)).Trim();
        MyFilmsDetail.setGUIProperty("config.configfilter", userfilter);
        LogMyFilms.Debug("userfilter from setup: StrDfltSelect = '" + userfilter + "'");
      }
      else
        MyFilmsDetail.clearGUIProperty("config.configfilter");

      // check, if Traktuser has to be "switched"
      if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
      {
        if (conf.StrUserProfileName == Helper.GetTraktUser())
        {
          LogMyFilms.Debug("Load_Config(): Current MyFilms user '" + conf.StrUserProfileName + "' is already the active Trakt user - no switch!");
        }
        else if (Helper.GetTraktUserList().Contains(conf.StrUserProfileName))
        {
          if (Helper.ChangeTraktUser(conf.StrUserProfileName))
            LogMyFilms.Debug("Load_Config(): Successfully switched Trakt to user '" + conf.StrUserProfileName + "'");
          else
            LogMyFilms.Error("Load_Config(): An error occurred changing current Trakt user login credentials!");
        }
        else
        {
          LogMyFilms.Debug("Load_Config(): Current MyFilms user '" + conf.StrUserProfileName + "' is not a trakt user - cannot switch!");
        }
      }
      // set user online status
      MyFilmsDetail.setGUIProperty("user.onlinestatus", Helper.IsTraktAvailableAndEnabled ? Helper.GetUserOnlineStatus(conf.StrUserProfileName) : "local");

      // get the userdefined Views
      try
      {
        if (conf.CustomViews.View.Rows.Count > 0)
        {
          MyFilmsDetail.setGUIProperty("views.count", "0");
          int i = 1;
          foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => Helper.FieldIsSet(customView.DBfield) && customView.ViewEnabled))
          {
            // publish properties for skin menu
            MyFilmsDetail.setGUIProperty(string.Format("views.item.{0}.name", i), string.IsNullOrEmpty(customView.Label) ? customView.DBfield : customView.Label);
            MyFilmsDetail.setGUIProperty(string.Format("views.item.{0}.image", i), customView.ImagePath);
            MyFilmsDetail.setGUIProperty(string.Format("views.item.{0}.hyperlinkparameter", i), "config:" + currentConfig + "|view:" + (string.IsNullOrEmpty(customView.Label) ? customView.DBfield : customView.Label));
            i++;
          }
        }
        else
        {
          MyFilmsDetail.setGUIProperty("views.count", conf.CustomViews.View.Rows.Count.ToString());
        }

        #region show all view options, if selected ... (disabled)
        //ArrayList displayItems = GetDisplayItems("view");
        //foreach (string[] displayItem in displayItems)
        //{
        //  strin viewlabel = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
        //  string view = displayItem[0];
        //  string image = ""; // GetImageforMenu(item);
        //}
        #endregion

      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Load_Config(): Error setting views to GUIproperties: '" + ex.Message);
      }
      return true;
    }

    private void Refreshfacade()
    {
      // stop facade background worker
      StopLoadingViewDetails = true;
      StopLoadingMenuDetails = true;
      StopLoadingFilmlistDetails = true;

      new Thread(delegate()
      {
        {
          MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
          Fin_Charge_Init(false, false);
          MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // what to do after it has finished here ....
            }
            return 0;
          }, 0, 0, null);
      }) { Name = "MyFilmsBGWorker", IsBackground = true, Priority = ThreadPriority.Normal }.Start();

    }

    private void Worker_Refreshfacade()
    {
      MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
      Fin_Charge_Init(false, false);
      MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
    }

    private void Loadfacade()
    {
      new Thread(delegate()
      {
        {
          MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
          Fin_Charge_Init(false, true);
          MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // what to do after it has finished here ....
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsBGWorker", IsBackground = true }.Start();
    }

    private void Worker_Loadfacade()
    {
      MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
      Fin_Charge_Init(false, true);
      MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
    }

    //--------------------------------------------------------------------------------------------
    //  Initial Windows load. If loadDfltSlct = true => load default select if any
    //                           loadDfltSlct = false => return from  MyFilmsDetail
    //--------------------------------------------------------------------------------------------
    private void Fin_Charge_Init(bool loadDfltSlct, bool reload)
    {
      LogMyFilms.Debug("Fin_Charge_Init() called with loadDfltSlct = '" + loadDfltSlct + "', reload = '" + reload + "', currentViewContext = '" + conf.ViewContext + "'");
      //if (publishTimer != null) publishTimer.SafeDispose();

      // reset view
      if (conf.ViewContext == ViewContext.StartView) // if View or Movie_ID loadparams are present, it is already set to "Startview"
      {
        conf.Boolselect = false; // Groupviews = false
        conf.StrSelect = ""; // reset filter, if there is one left from before ...
        ResetGlobalFilters();
      }

      #region (re)load Dataset ...
      if (reload || (PreviousWindowId != ID_MyFilmsDetail && PreviousWindowId != ID_MyFilmsActors))
      {
        if (reload) // ReLoad MyFilms DB from disk 
        {
          currentFanartList.Clear(); // as items do change, reset fanart list ...
          conf.DbSelection = new[] { conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false.ToString(), false.ToString() };
          BaseMesFilms.LoadMyFilms(conf.StrFileXml); // Will be automatically loaded, if not yet done - save time on reentering MyFilms GUI !!!
          // r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
        }
        r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
        if (reload)
        {
          #region inform Trakt ro start a sync ...
          {
            if (conf.StrFileType != Configuration.CatalogType.AntMovieCatalog3 && conf.StrFileType != Configuration.CatalogType.AntMovieCatalog4Xtended)
            {
              if (ImportComplete != null && conf.AllowTraktSync) // trigger sync to trakt page after importer finished
              {
                ImportComplete();
                LogMyFilms.Debug("Fin_Charge_Init(): Fired 'ImportCompleted' event to trigger sync to trakt page after initial DB import of external Catalog is finished !");
              }
            }
          }
          #endregion
        }
      }

      #region (re)load Dataset and check if migration for MUS is required ...
      if (conf.EnhancedWatchedStatusHandling && conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended && r.Length > 0)
      {
        if (!r[0][BaseMesFilms.MultiUserStateField].ToString().Contains(":")) // check, if the first record contains MUS data - if not, call the migration method !
        {
          // DataRow[] allmovies = BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens); // load all records into datraset
          MigrateToMus();
          r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // reload the dataset to reflect updated MUS data
        }
      }
      #endregion
      #endregion

      #region Configure Default Button Labels ...
      if (BtnSrtBy != null) BtnSrtBy.Label = conf.BoolCollection ? GUILocalizeStrings.Get(96) + (conf.StrSortaInHierarchies == conf.StrSTitle ? GUILocalizeStrings.Get(103) : BaseMesFilms.TranslateColumn(conf.StrSortaInHierarchies)) : GUILocalizeStrings.Get(96) + (conf.StrSorta == conf.StrSTitle ? GUILocalizeStrings.Get(103) : BaseMesFilms.TranslateColumn(conf.StrSorta));
      // BtnToggleGlobalWatched.Label = (MyFilms.conf.GlobalUnwatchedOnly) ? string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628)) : string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));
      #endregion

      InitialStart = false; // Guzzi: Set to false after first initialization to be able to return to noninitialized View - Make sure to set true if changing DB config

      LogMyFilms.Debug("Fin_Charge_Init(): (re)load done - now do action for currentViewContext = '" + conf.ViewContext + "'");

      switch (conf.ViewContext)
      {
        case ViewContext.None:
          #region Default, if nothing is configured
          conf.StrSelect = conf.StrTitle1 + " not like ''";
          conf.Boolselect = false;
          conf.Boolindexed = false;
          conf.Boolindexedreturn = false;
          SetLabelView("all");
          Change_Layout_Action(0);
          GetFilmList(conf.StrIndex);
          #endregion
          break;

        case ViewContext.Menu:
        case ViewContext.MenuAll:
          GetSelectFromMenuView(conf.ViewContext == ViewContext.MenuAll); // load views into facade ...
          break;

        case ViewContext.StartView:
          if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID) && loadParamInfo.Config == Configuration.CurrentConfig) // movieID given in load params -> set index to selected film!
          #region If LoadParams - Check and set single movie in current config ...
          {
            LogMyFilms.Debug("Fin_Charge_Init() - LoadParams - try override loading movieid: '" + loadParamInfo.MovieID + "', play: '" + loadParamInfo.Play + "'");
            r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with default filters
            // facade index is set in filmlist loading - only launching details necessary !
            Change_Layout_Action(conf.StrLayOut);
            if (!string.IsNullOrEmpty(loadParamInfo.MovieID)) // if load params for movieid exist, set current index to the movie detected
            {
              for (int index = 0; index < r.Length; index++)
              {
                DataRow sr = r[index];
                if (loadParamInfo.MovieID == Int32.Parse(sr["Number"].ToString()).ToString())
                {
                  // bool success = int.TryParse(loadParamInfo.MovieID, out index);
                  conf.StrIndex = index;
                  conf.StrTIndex = sr[conf.StrTitle1].ToString();
                  LogMyFilms.Debug("Fin_Charge_Init(): loadParam - set movie '" + conf.StrTIndex + "' by index '" + conf.StrIndex + "'");
                }
              }
            }
            GetFilmList(conf.StrIndex);
            SetLabelView(conf.StrTxtView); // Reload view name from configfile...
          }
          #endregion
          else
          #region Load Default View via config or LoadParameter ...
          {
            LogMyFilms.Debug("Fin_Charge_Init() - load default view - (config or loadparameter)");
            Change_Layout_Action(conf.StrLayOut);
            if (!Helper.FieldIsSet(conf.StrViewDfltItem) || conf.StrViewDfltItem == GUILocalizeStrings.Get(342)) // no Defaultitem defined for defaultview or "films" -> normal movielist
            {
              conf.StrSelect = conf.StrTitle1 + " not like ''";
              conf.StrTxtSelect = "";
              conf.StrTitleSelect = "";
              conf.Boolselect = false;
              conf.Boolindexed = false;
              conf.Boolindexedreturn = false;
              conf.ViewContext = ViewContext.Movie;
              SetLabelView("all");
              GetFilmList(conf.StrIndex);
              // if (conf.StrIndex == -1 && facadeFilms.Count > 0) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0); // make sure, selecteditem is initialized !
            }
            else // called with userdefined views - so launch them ...
            {
              #region Set and Call userdefined Views
              if (conf.StrViewDfltItem == GUILocalizeStrings.Get(1079819)) // Views Menu
              {
                GetSelectFromMenuView(conf.BoolMenuShowAll);
              }
              else if (string.IsNullOrEmpty(conf.StrViewDfltText)) // no filteritem defined for the defaultview
              {
                if (IsCategoryYearCountryField(conf.StrViewDfltItem) || conf.StrViewDfltItem == "Storage" || conf.StrViewDfltItem == "Actors" || conf.StrViewDfltItem == "RecentlyAdded")
                  Change_View_Action(conf.StrViewDfltItem);
                else
                {
                  // MFview.ViewRow customView = this.GetCustomViewFromViewLabel(conf.StrViewDfltItem);
                  foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => conf.StrViewDfltItem == customView.Label || conf.StrViewDfltItem == customView.DBfield))
                  {
                    Change_View_Action(customView.Label);
                    break; // stop, if match was found - avoids reprocessing, if multiple matches
                  }
                }
              }
              else // filteritem IS defined for the defaultview
              {
                string wStrViewDfltItem = conf.StrViewDfltItem;
                foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => conf.StrViewDfltItem == customView.Label))
                {
                  wStrViewDfltItem = customView.DBfield;
                  SetLabelView(customView.Label);
                  break;
                }
                conf.Boolselect = true;
                conf.Boolreturn = true;
                conf.WStrSort = wStrViewDfltItem;
                string sLabel = conf.Wselectedlabel;
                if (GetColumnType(wStrViewDfltItem) != typeof(string))
                  conf.StrSelect = wStrViewDfltItem + " = '" + conf.StrViewDfltText + "'";
                else if (IsDateField(wStrViewDfltItem))
                  conf.StrSelect = wStrViewDfltItem + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.StrViewDfltText).ToShortDateString()) + "*'";
                else if (IsAlphaNumericalField(wStrViewDfltItem))
                  conf.StrSelect = wStrViewDfltItem + " like '" + conf.StrViewDfltText + "'";
                else
                  conf.StrSelect = wStrViewDfltItem + " like '*" + conf.StrViewDfltText + "*'";
                // TxtSelect.Label = conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";
                conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";

                if (wStrViewDfltItem.Length > 0) SetLabelView(wStrViewDfltItem);
                GetFilmList(conf.StrIndex);
              }
              #endregion
            }
          }
          #endregion
          break;

        default:
          #region Normally load or refresh the facade ...
          {
            LogMyFilms.Debug("Fin_Charge_Init() - normal load, conf.Boolselect = '" + conf.Boolselect + "'");
            if (conf.Boolselect) // Groupviews / Persons
            {
              Change_Layout_Action(conf.WStrLayOut);
              SetLabelView(conf.StrTxtView); // Reload view name from configfile...
              GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, false, ""); // preserve index from last time
              LogMyFilms.Debug("Fin_Charge_Init() - Boolselect = true -> StrTxtSelect = '" + conf.StrTxtSelect + "', StrTxtView = '" + conf.StrTxtView + "'");
            }
            else
            {
              Change_Layout_Action(conf.StrLayOut);
              SetLabelView(conf.StrTxtView); // Reload view name from configfile...
              GetFilmList(conf.StrIndex);
              if (facadeFilms.Count == 0)
              {
                LogMyFilms.Error("Fin_Charge_Init(): Movie list seems empty - return!");
                return;
              }
            }
          }
          #endregion
          break;
      }

      LogMyFilms.Debug("Fin_Charge_Init: StrSelect = '" + conf.StrSelect + "', StrTxtSelect = '" + conf.StrTxtSelect + "'");

      //if (string.IsNullOrEmpty(conf.StrTxtSelect) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) // empty or starts with "all" or "filtered" ... 
      //  SetLabelSelect("");
      //else
      //  SetLabelSelect(conf.StrTxtSelect);

      switch (conf.LastID)
      {
        case ID_MyFilmsDetail:
          GUIWindowManager.ActivateWindow(ID_MyFilmsDetail); // if last window in use was detailed one display that one again
          break;
        case ID_MyFilmsActors:
          GUIWindowManager.ActivateWindow(ID_MyFilmsActors); // if last window in use was actor one display that one again
          break;
        default:
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          // if (facadeFilms.SelectedListItemIndex < 0 && facadeFilms.Count > 0) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0); // make sure, selecteditem is initialized !
          //int itemIndex = facadeFilms.SelectedListItemIndex;
          //if (itemIndex > -1)
          //  GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
          //else if (facadeFilms.Count > 0)
          //  GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0);
          break;
      }
    }

    private static void MigrateToMus()
    {
      LogMyFilms.Debug("MigrateToMus() - starting migration to MUS data");

      #region create a backup copy of DB file
      string backupcopy = conf.StrFileXml.Replace(".xml", " - MUS-backup - " + DateTime.Now.ToString().Replace(":", "-") + ".xml").Replace("/", "-");
      try
      {
        File.Copy(conf.StrFileXml, backupcopy, true);
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("MigrateToMus() - error creating backup copy - cancelling MUS migration ! - Exception: ", ex.Message);
      }
      LogMyFilms.Debug("MigrateToMus() - finished creating a DB backupcopy to '" + backupcopy + "'");
      #endregion

      DataRow[] allmovies = BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens); // load all records into datraset
      foreach (DataRow sr in allmovies.Where(sr => conf.EnhancedWatchedStatusHandling).Where(sr => !sr[BaseMesFilms.MultiUserStateField].ToString().Contains(":")))
      {
        #region migrate records
        MultiUserData userData;
        bool setwatcheddate = false;
        if (sr[conf.StrWatchedField].ToString().Contains(":")) // old field was already multiuserdata - migrate it!
        {
          userData = new MultiUserData(sr[conf.StrWatchedField].ToString());
          sr[conf.StrWatchedField] = userData.GetUserState(conf.StrUserProfileName).Watched ? "true" : conf.GlobalUnwatchedOnlyValue.ToLower();
          setwatcheddate = true;
        }
        else // old field was standard watched data - create MUS and add watched for current user
        {
          bool tmpwatched = !string.IsNullOrEmpty(conf.GlobalUnwatchedOnlyValue) &&
                            sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower() &&
                            sr[conf.StrWatchedField].ToString().Length > 0;
          userData = new MultiUserData("");
          userData.SetWatchedWithoutDate(conf.StrUserProfileName, tmpwatched);
          if (sr["RatingUser"] != Convert.DBNull)
            userData.SetRating(conf.StrUserProfileName, (decimal)sr["RatingUser"]);
        }
        sr[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
        sr["DateWatched"] = setwatcheddate && userData.GetUserState(conf.StrUserProfileName).WatchedDate != MultiUserData.NoWatchedDate ? userData.GetUserState(conf.StrUserProfileName).WatchedDate : Convert.DBNull;
        sr["RatingUser"] = userData.GetUserState(conf.StrUserProfileName).UserRating == MultiUserData.NoRating ? Convert.DBNull : userData.GetUserState(conf.StrUserProfileName).UserRating;
        #endregion
      }
      LogMyFilms.Debug("MigrateToMus() - finished migration to MUS data for '" + allmovies.Length.ToString() + "' DB entries");
      MyFilmsDetail.Update_XML_database();
      LogMyFilms.Debug("MigrateToMus() - finished saving DB with updated MUS data");
    }

    private void ResetGlobalFilters()
    {
      //Reset Global Filters !
      GlobalFilterTrailersOnly = false;
      GlobalFilterStringTrailersOnly = String.Empty;
      MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly");

      GlobalFilterMinRating = false;
      GlobalFilterStringMinRating = String.Empty;
      MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
      MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");

      if (conf.GlobalUnwatchedOnly) // Reset GlobalUnwatchedFilter to the setup default (can be changed via GUI menu)
      {
        GlobalFilterStringUnwatched = CreateGlobalUnwatchedFilter();
      }
      else
      {
        GlobalFilterStringUnwatched = String.Empty;
        MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
      }

      GlobalFilterIsOnlineOnly = conf.GlobalAvailableOnly;

      if (GlobalFilterIsOnlineOnly)
      {
        GlobalFilterStringIsOnline = "IsOnline like 'true' AND ";
        MyFilmsDetail.setGUIProperty("globalfilter.isonline", "true");
      }
      else
      {
        GlobalFilterStringIsOnline = String.Empty;
        MyFilmsDetail.clearGUIProperty("globalfilter.isonline");
      }

      GlobalFilterMovieFormat3D = GUILocalizeStrings.Get(10798973); // all
      GlobalFilterStringMovieFormat3D = string.Empty;
      MyFilmsDetail.clearGUIProperty("globalfilter.movieformat");

    }

    //--------------------------------------------------------------------------------------------
    //   Change LayOut 
    //--------------------------------------------------------------------------------------------
    private void Change_Layout_Action(int wLayout)
    {
      LogMyFilms.Debug("Change_Layout_Action() - change facade layout to '" + wLayout + "'");
      if (facadeFilms == null)
      {
        LogMyFilms.Error("Change_Layout_Action() - facade control is null - return without action");
        return;
      }
      int itemIndex = facadeFilms.SelectedListItemIndex;
      switch (wLayout)
      {
        // MP:
        //List = 0,
        //SmallIcons = 1,
        //LargeIcons = 2,
        //Filmstrip = 3,
        //AlbumView = 4,
        //Playlist = 5,
        //CoverFlow = 6
        //MyFilms:
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.List);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.AlbumView);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.SmallIcons);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.LargeIcons);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.Filmstrip);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.CoverFlow);

        case 0:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101)); // "Layout: " + "List"
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
        case 1:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(529));
          facadeFilms.CurrentLayout = facadeFilms.AlbumListLayout != null ? GUIFacadeControl.Layout.AlbumView : GUIFacadeControl.Layout.List;
          break;
        case 2:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(100));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
          break;
        case 3:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(417));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
        case 4:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(733));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
          break;
        case 5:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(791));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
          break;

        default:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
      }
      if (itemIndex > -1) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
    }

    //--------------------------------------------------------------------------------------------
    //   Change View Response  (and process corresponding filter list)
    //--------------------------------------------------------------------------------------------
    private void Change_View_Action(string selectedView)
    {
      LogMyFilms.Debug("Change_View_Action called with '" + selectedView + "'");

      #region reset parameters 
      internalLoadParam = null; // clear internal start params
      conf.CurrentView = selectedView;
      conf.StrSelect = ""; // reset view filter
      conf.StrFilmSelect = ""; // reset film list filter
      conf.StrViewSelect = "";
      conf.StrPersons = ""; // reset person filter
      conf.IndexedChars = 0;
      conf.Boolindexed = false;
      conf.BoolSkipViewState = false;
      MyFilmsDetail.clearGUIProperty("moviecontext.value");
      #endregion

      MFview.ViewRow selectedCustomView = conf.CustomViews.View.NewViewRow();

      //if (MyFilms.conf.CustomViews.View.Any(view => view.Label == selectedView)) // if it is a custom view ...
      //{
      //    selectedView = "CustomView"; // if a CustomView definition is found ...
      //    selectedCustomView = MyFilms.conf.CustomViews.View.First(view => view.Label == selectedView);
      //}
      //else if (selectedView == GUILocalizeStrings.Get(1079819)) // "menu"
      //{
        
      //}

      foreach (MFview.ViewRow customView in conf.CustomViews.View)
      {
        if (selectedView == customView.Label)
        {
          selectedView = "CustomView"; // if a CustomView definition is found ...
          selectedCustomView = customView;
          break;
        }
      }

      if (selectedView == GUILocalizeStrings.Get(1079819)) selectedView = "Menu"; // Views Menu
      switch (selectedView)
      {
        #region Special Views ...
        case "All": //  Change View All Films - this is not a "DB field", but a synthetic view ...
          conf.ViewContext = ViewContext.Movie;
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;
          LogMyFilms.Debug("Change_View filter - " + "StrSelect: " + conf.StrSelect + " | wStrSort: " + conf.WStrSort);
          SetLabelView("all");
          GetFilmList();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          SetDummyControlsForFacade(conf.ViewContext);
          break;

        case "Storage":
          #region storage currently not used ...
          // Change View by "Storage":
          conf.ViewContext = ViewContext.Movie;
          conf.StrSelect = "((" + conf.StrTitle1 + " not like '') and (" + conf.StrStorage + " not like ''))";
          conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
          conf.Boolselect = false;
          conf.Boolreturn = false;
          conf.WStrSort = conf.StrSTitle;
          GetFilmList();
          SetLabelView("storage");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion
        #endregion

        case "Menu": //  Change View to "Menu"
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;
          GetSelectFromMenuView(conf.BoolMenuShowAll); // load views into facade ...
          break;

        case "onlineinfo":
          GetSelectFromOnlineMenuView();
          break;

        case "CustomView":
          #region New CustomViews ...
          if (selectedCustomView == null) return;

          switch (selectedCustomView.DBfield)
          {
            case "Producer":
            case "Director":
            case "Writer":
            case "Actors":
            case "Borrower":
              conf.ViewContext = ViewContext.Person;
              break;
            case "":
            case "OriginalTitle":
            case "TranslatedTitle":
            case "FormattedTitle":
              conf.ViewContext = ViewContext.Movie;
              break;
            default:
              conf.ViewContext = ViewContext.Group;
              break;
          }

          conf.WStrSort = selectedCustomView.DBfield;
          conf.BoolSortCountinViews = selectedCustomView.SortFieldViewType == "Count";
          if (conf.BoolSortCountinViews)
            conf.WStrSortSensCount = selectedCustomView.SortDirectionView;
          else
            conf.WStrSortSens = selectedCustomView.SortDirectionView;

          conf.IndexedChars = selectedCustomView.Index;
          //int iLayout = 0;
          //int.TryParse(selectedCustomView.LayoutView, out iLayout);
          //conf.WStrLayOut = iLayout;
          conf.WStrLayOut = int.Parse(selectedCustomView.LayoutView);

          if (selectedCustomView.Filter.Length > 0)
          {
            //if (conf.StrDfltSelect.Length == 0) conf.StrDfltSelect = selectedCustomView.Filter + " AND ";
            //else conf.StrDfltSelect += selectedCustomView.Filter + " AND ";
            conf.StrViewSelect = selectedCustomView.Filter + " AND ";
          }

          conf.BoolSkipViewState = selectedCustomView.Value.Length > 0 && IsTitleField(conf.WStrSort);

          // RestoreLastView(conf.CurrentView); // restore saved settings, if there are any - might be "more" than the ones defined in Custom Views

          // now view film list, filtered list or view ...
          if (conf.IndexedChars == 0 && selectedCustomView.Value == "*") // Film list view without value filter - but only, if no indexed view is selected
          {
            conf.ViewContext = ViewContext.Movie;
            conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
            conf.WStrSort = conf.StrSTitle;
            conf.Boolselect = false;
            conf.Boolreturn = false;
            LogMyFilms.Debug("Change_View filter - " + "StrSelect: " + conf.StrSelect + " | wStrSort: " + conf.WStrSort);
            SetLabelView("all");
            GetFilmList();
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            SetDummyControlsForFacade(conf.ViewContext);
          }
          else if (conf.IndexedChars == 0 && selectedCustomView.Value.Length > 0 && !selectedCustomView.Value.Contains(",")) // Film list view with value filter
          {
            conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
            conf.Boolselect = true;
            conf.Boolreturn = false;
            conf.Wstar = "*";
            if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " *" + conf.Wstar + "*";
            // TxtSelect.Label = conf.StrTxtSelect;
            conf.StrSelect = conf.WStrSort;

            conf.StrFilmSelect = string.Empty;
            conf.Wselectedlabel = selectedCustomView.Value != "*" ? selectedCustomView.Value : "";
            do
            {
              if (conf.StrTitleSelect != string.Empty) conf.StrTitleSelect += conf.TitleDelim;
              conf.StrTitleSelect += selectedCustomView.Value;
            }
            while (GetFilmList() == false); //keep calling while single folders found
          }
          else
          // Indexed Views
          {
            if (conf.IndexedChars > 0) // if indexed view is enabled ...
            {
              conf.Boolindexed = true;
              conf.Boolindexedreturn = false;
            }
            string listfilter = selectedCustomView.Value.Length == 0 ? "*" : selectedCustomView.Value;
            if (selectedCustomView.Value.Length == 0)
              GetSelectFromDivxThreaded();
            else
              GetSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, listfilter, true, string.Empty);
          }

          SetLabelView(!string.IsNullOrEmpty(selectedCustomView.Label) ? selectedCustomView.Label : selectedCustomView.DBfield);
          GUIWindowManager.Process(); // required for the next statement to work correctly, so the skinengine has correct state for visibility and focus
          // GUIControl.FocusControl(GetID, facadeFilms.GetID);
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        #region OLD Userdefined Views ... (disabled)
        //case "View0":
        //case "View1":
        //case "View2":
        //case "View3":
        //case "View4":
        //  // specific user View
        //  int i = 0;
        //  if (selectedView == "View1") i = 1;
        //  if (selectedView == "View2") i = 2;
        //  if (selectedView == "View3") i = 3;
        //  if (selectedView == "View4") i = 4;

        //  conf.wStrSort = conf.StrViewItem[i];
        //  SetLabelView(selectedView.ToLower());
        //  conf.wStrSortSens = " ASC";
        //  if (conf.StrViewValue[i].Length > 0 && !conf.StrViewValue[i].Contains(","))
        //  {
        //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
        //    conf.Boolselect = true;
        //    conf.Wstar = "*";
        //    if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
        //    // TxtSelect.Label = conf.StrTxtSelect;
        //    MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
        //    conf.StrSelect = conf.wStrSort;

        //    conf.StrFilmSelect = string.Empty;
        //    conf.Wselectedlabel = conf.StrViewValue[i];
        //    conf.Boolreturn = true;
        //    do
        //    {
        //      if (conf.StrTitleSelect != string.Empty) conf.StrTitleSelect += conf.TitleDelim;
        //      conf.StrTitleSelect += conf.StrViewValue[i];
        //    }
        //    while (GetFilmList() == false); //keep calling while single folders found
        //  }
        //  else
        //  {
        //    if (conf.IndexedChars > 0) // if indexed view is enabled ...
        //    {
        //      conf.Boolindexed = true;
        //      conf.Boolindexedreturn = false;
        //    }
        //    string listfilter = (conf.StrViewValue[i].Length == 0) ? "*" : conf.StrViewValue[i];
        //    if (conf.StrViewValue[i].Length == 0)
        //      GetSelectFromDivxThreaded();
        //    else
        //      GetSelectFromDivx(conf.StrTitle1 + " not like ''", conf.wStrSort, conf.wStrSortSens, listfilter, true, string.Empty);
        //  }

        //  //if ((conf.StrViewText[i] == null) || (conf.StrViewText[i].Length == 0))
        //  //{
        //  //    MyFilmsDetail.setGUIProperty("view", conf.StrViewItem[i]);   // specific user View1
        //  //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewItem[i]);
        //  //}
        //  //else
        //  //{
        //  //    MyFilmsDetail.setGUIProperty("view", conf.StrViewText[i]);   // specific Text for View1
        //  //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewText[i]);
        //  //}
        //  GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        //  break;
        #endregion

        default: // generic views ...
          #region Default Generic View Handling ...
          {
            conf.WStrSort = selectedView;
            conf.WStrSortSens = " ASC";
            SetLabelView(selectedView.ToLower());

            switch (selectedView)
            {
              case "Producer":
              case "Director":
              case "Writer":
              case "Actors":
              case "Borrower":
              case "Persons":
                conf.ViewContext = ViewContext.Person;
                break;
              case "OriginalTitle":
              case "TranslatedTitle":
              case "FormattedTitle":
                conf.ViewContext = ViewContext.Movie;
                break;
              case "Category":
              case "RecentlyAdded":
              case "Country":
                conf.ViewContext = ViewContext.Group;
                break;
              case "Year":
              case "Date":
              case "DateAdded":
                conf.ViewContext = ViewContext.Group;
                conf.WStrSortSens = " DESC";
                break;
              default:
                conf.ViewContext = ViewContext.Group;
                break;
            }

            if (conf.IndexedChars > 0) // if indexed view is enabled ...
            {
              conf.Boolindexed = true;
              conf.Boolindexedreturn = false;
            }

            RestoreLastView(conf.CurrentView);
            GetSelectFromDivxThreaded(); //GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, "*", true, string.Empty);
            break;
          }
          #endregion
      }
      // SetDummyControlsForFacade(conf.ViewContext); will be set in Lst_Details later ...
      LogMyFilms.Debug("Change_View_Action(" + selectedView + ") -> End");
    }

    //--------------------------------------------------------------------------------------------
    //   Menu actions ...
    //--------------------------------------------------------------------------------------------
    private void Change_Menu_Action(string choiceView)
    {
      LogMyFilms.Debug("Change_View called with '" + choiceView + "'");
      XmlConfig amcXmlConfig = new XmlConfig(); // no more used - replaced by using ... (was not compatible to cached writing in other places), only used for AMCupdater settings
      switch (choiceView.ToLower())
      {
        case "config":
          #region Choose Database
          ChooseNewConfig();
          break;
          #endregion

        case "nasstatus": //Check and show status of NAS Servers
          #region nasstatus
          //First check status of configured NAS-Servers
          WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
          int intTimeOut = conf.StrWOLtimeout; //Timeout für WOL

          //GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...

          GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
          //if (dlg == null) return;
          //dlg.Reset();
          List<string> choiceSearch = new List<string>();
          dlg.SetHeading(GUILocalizeStrings.Get(10798727)); // show nas server status
          dlg.Add(GUILocalizeStrings.Get(712)); //Return
          choiceSearch.Add("BACK");

          if (conf.StrNasName1.Length > 0)
          {
            if (WakeOnLanManager.Ping(conf.StrNasName1, intTimeOut))
              dlg.Add("'" + conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798741));
            // srv name + " - (active)" 
            else
              dlg.Add("'" + conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798742));
            // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS1");
          }

          if (conf.StrNasName2.Length > 0)
          {
            if (WakeOnLanManager.Ping(conf.StrNasName2, intTimeOut))
              dlg.Add("'" + conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798741));
            // srv name + " - (active)" 
            else
              dlg.Add("'" + conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798742));
            // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS2");
          }

          if (conf.StrNasName3.Length > 0)
          {
            if (WakeOnLanManager.Ping(conf.StrNasName3, intTimeOut))
              dlg.Add("'" + conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798741));
            // srv name + " - (active)" 
            else
              dlg.Add("'" + conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798742));
            // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS3");
          }

          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;

          switch (choiceSearch[dlg.SelectedLabel])
          {
            case "BACK":
              return;

            default:

              string nasServerName;
              string nasMacAddress;
              switch (choiceSearch[dlg.SelectedLabel])
              {
                case "NAS1":
                  nasServerName = conf.StrNasName1;
                  nasMacAddress = conf.StrNasMAC1;
                  break;
                case "NAS2":
                  nasServerName = conf.StrNasName2;
                  nasMacAddress = conf.StrNasMAC2;
                  break;
                case "NAS3":
                  nasServerName = conf.StrNasName3;
                  nasMacAddress = conf.StrNasMAC3;
                  break;
                default:
                  nasServerName = String.Empty;
                  nasMacAddress = String.Empty;
                  break;
              }

              if (nasMacAddress.Length > 0)
              {
                if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(nasMacAddress), nasServerName, intTimeOut))
                  GUIUtils.ShowNotifyDialog("'" + nasServerName + "' " + GUILocalizeStrings.Get(10798743)); //successfully started 
                else
                  GUIUtils.ShowOKDialog("'" + nasServerName + "' " + GUILocalizeStrings.Get(10798744)); // could not be started 
              }
              else
              {
                GUIUtils.ShowOKDialog("Servername: '" + nasServerName + "'", "MAC: '" + nasMacAddress + "'", GUILocalizeStrings.Get(10798745), ""); // start not possible - check config !
              }
              break;
          }
          return;
          #endregion

        case "globaloptions":
          #region Submenu Globaloptions
          LogMyFilms.Debug("Building (sub)menu globaloptions");

          var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg1 == null) return;
          dlg1.Reset();
          dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
          var choiceViewGlobalOptions = new List<string>();

          // Choose UserProfileName
          if (conf.EnhancedWatchedStatusHandling)
          {
            dlg1.Add(string.Format(GUILocalizeStrings.Get(1079840), conf.StrUserProfileName));
            choiceViewGlobalOptions.Add("userprofilename");
          }

          // Delete UserProfileName
          if (conf.EnhancedWatchedStatusHandling)
          {
            dlg1.Add(GUILocalizeStrings.Get(1079818));
            choiceViewGlobalOptions.Add("userprofilenamedelete");
          }

          // Choose grabber script for that session
          if (conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798628)));
          if (!conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("choosescript");

          // Change grabber find trying best match option
          if (conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798628)));
          if (!conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("findbestmatch");

          // Change AMCupdater option "import if no auto match" in background mode for myfilms
          bool importonfail = (amcXmlConfig.ReadAMCUXmlConfig(conf.StrAMCUpd_cnf, "Import_File_On_Internet_Lookup_Failure", "").ToLower() == "true");
          if (importonfail) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079810), GUILocalizeStrings.Get(10798628))); // Import/Update AMCUpdater - import if internet lookup fails ({0})
          if (!importonfail) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079810), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("amcimportifinternetlookupfails");

          // Change AMCupdater option "purge missing files"
          bool purgeorphans = (amcXmlConfig.ReadAMCUXmlConfig(conf.StrAMCUpd_cnf, "Purge_Missing_Files", "").ToLower() == "true");
          if (purgeorphans) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079811), GUILocalizeStrings.Get(10798628))); // Import/Update AMCUpdater - purge mising db entries after import finished ({0})
          if (!purgeorphans) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079811), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("amcpurgeorphans");

          if (conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
          if (!conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("alwaysdefaultview");

          dlg1.Add(string.Format(GUILocalizeStrings.Get(1079841), conf.StrViewDfltItem)); // change default view
          choiceViewGlobalOptions.Add("changedefaultview");

          //if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "wtf ..."))
          //{
          //}

          if (conf.AlwaysShowConfigMenu) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079920), GUILocalizeStrings.Get(10798628)));
          if (!conf.AlwaysShowConfigMenu) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079920), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("alwaysdefaultconfig");

          string cDefaultConfig = "";
          using (XmlSettings xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            cDefaultConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
          }
          dlg1.Add(string.Format(GUILocalizeStrings.Get(1079842), cDefaultConfig)); // change default start config
          choiceViewGlobalOptions.Add("changedefaultconfig");

          if (conf.BoolAskForPlaybackQuality) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079919), GUILocalizeStrings.Get(10798628))); // Always ask for playback quality for online content ({0})
          if (!conf.BoolAskForPlaybackQuality) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079919), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("askforplaybackquality");

          if (conf.CacheOnlineTrailer) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079918), GUILocalizeStrings.Get(10798628))); // Cache trailers for online content ({0})
          if (!conf.CacheOnlineTrailer) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079918), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("cacheonlinetrailer");

          //if (conf.UseListViewForGoups) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079897), GUILocalizeStrings.Get(10798628)));
          //if (!conf.UseListViewForGoups) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079897), GUILocalizeStrings.Get(10798629)));
          //choiceViewGlobalOptions.Add("alwayslistforgroups");

          if (conf.StrCheckWOLenable)
          {
            if (conf.StrCheckWOLuserdialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079898), GUILocalizeStrings.Get(10798628)));
            if (!conf.StrCheckWOLuserdialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079898), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("woluserdialog");
          }

          dlg1.DoModal(GetID);
          if (dlg1.SelectedLabel == -1) return;

          LogMyFilms.Debug("Call global menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel] + "'");
          Change_Menu_Action(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          return;
          #endregion

        case "globalupdates":
          #region Submenu GlobalUpdates
          var dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg2 == null) return;
          dlg2.Reset();
          dlg2.SetHeading(GUILocalizeStrings.Get(10798690)); // Global Updates ...
          var choiceViewGlobalUpdates = new List<string>();

          dlg2.Add(GUILocalizeStrings.Get(1079850)); // Update Online status - to check availability if media files
          choiceViewGlobalUpdates.Add("isonlinecheck");

          if (conf.StrAMCUpd)
          {
            #region global update optiones (AMCupdater)
            if (bgUpdateDB.IsBusy && bgUpdateDB.WorkerSupportsCancellation)
            {
              // stop background worker
              dlg2.Add(GUILocalizeStrings.Get(1079855)); // Stop Database Updater (active)
              choiceViewGlobalUpdates.Add("cancelupdatedb");
            }
            else
            {
              dlg2.Add(GUILocalizeStrings.Get(1079861)); // Update Database with external AMCupdater
              choiceViewGlobalUpdates.Add("updatedb");

              if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Global AMCU Custom Update")) // check if specialmode is configured for disabled features
              {
                dlg2.Add(GUILocalizeStrings.Get(1079843)); // Userdefined DB Update (AMCUpdater)
                // Search all personinfos
                choiceViewGlobalUpdates.Add("updatedbselect");
              }
            }
            #endregion
          }

          if (conf.StrFanart)
          {
            dlg2.Add(GUILocalizeStrings.Get(4514)); // Download all Fanart
            choiceViewGlobalUpdates.Add("downfanart");
          }

          if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Global Update all PersonInfos"))
          {
            dlg2.Add(GUILocalizeStrings.Get(10798715)); // Load Person infos - all persons
            // Search all personinfos
            choiceViewGlobalUpdates.Add("personinfos-all");
          }

          if (Helper.FieldIsSet(conf.StrStorageTrailer))
          {
            dlg2.Add(GUILocalizeStrings.Get(10798694)); // Search and register all trailers for all movies in DB
            choiceViewGlobalUpdates.Add("trailer-all");
          }

          // Global Update - Download all (missing) trailers"
          if (conf.BoolEnableOnlineServices)
          {
            dlg2.Add("Download missing Trailers (TMDB)");
            choiceViewGlobalUpdates.Add("trailer-all-download");
          }

          dlg2.Add(GUILocalizeStrings.Get(10798717)); // incomplete movie data
          // Search records with missing movie data, e.g. after import when internet data loading failed
          choiceViewGlobalUpdates.Add("incomplete-movie-data");

          dlg2.DoModal(GetID);

          if (dlg2.SelectedLabel == -1)
          {
            return;
          }
          Change_Menu_Action(choiceViewGlobalUpdates[dlg2.SelectedLabel].ToLower());
          return;
          #endregion

        case "globalmappings": // map useritems from GUI
          #region globalmappings
          var dlg3 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg3 == null) return;
          dlg3.Reset();
          dlg3.SetHeading(GUILocalizeStrings.Get(10798771)); // Display options ...
          var choiceGlobalMappings = new List<string>();

          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 1 (" + conf.Stritem1 + "-" + conf.Strlabel1 + ")"); // Display Item ....
          choiceGlobalMappings.Add("useritem1");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 2 (" + conf.Stritem2 + "-" + conf.Strlabel2 + ")");
          choiceGlobalMappings.Add("useritem2");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 3 (" + conf.Stritem3 + "-" + conf.Strlabel3 + ")");
          choiceGlobalMappings.Add("useritem3");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 4 (" + conf.Stritem4 + "-" + conf.Strlabel4 + ")");
          choiceGlobalMappings.Add("useritem4");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 5 (" + conf.Stritem5 + "-" + conf.Strlabel5 + ")");
          choiceGlobalMappings.Add("useritem5");

          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 1 (" + conf.StritemDetails1 + "-" + conf.StrlabelDetails1 + ")"); // Details Display Item ....
          choiceGlobalMappings.Add("useritemdetails1");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 2 (" + conf.StritemDetails2 + "-" + conf.StrlabelDetails2 + ")");
          choiceGlobalMappings.Add("useritemdetails2");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 3 (" + conf.StritemDetails3 + "-" + conf.StrlabelDetails3 + ")");
          choiceGlobalMappings.Add("useritemdetails3");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 4 (" + conf.StritemDetails4 + "-" + conf.StrlabelDetails4 + ")");
          choiceGlobalMappings.Add("useritemdetails4");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 5 (" + conf.StritemDetails5 + "-" + conf.StrlabelDetails5 + ")");
          choiceGlobalMappings.Add("useritemdetails5");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 6 (" + conf.StritemDetails6 + "-" + conf.StrlabelDetails6 + ")");
          choiceGlobalMappings.Add("useritemdetails6");

          // master-, secondary-  and sorttitle
          //dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + conf.StrTitle1 + "-" + BaseMesFilms.Translate_Column(conf.StrTitle1) + ")"); // mastertitle
          dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + conf.StrTitle1 + "-" + BaseMesFilms.TranslateColumn(conf.StrTitle1) + ")"); // mastertitle
          choiceGlobalMappings.Add("mastertitle");
          dlg3.Add(GUILocalizeStrings.Get(10798791) + " (" + conf.StrTitle2 + "-" + BaseMesFilms.TranslateColumn(conf.StrTitle2) + ")"); // secondary title
          choiceGlobalMappings.Add("secondarytitle");
          dlg3.Add(GUILocalizeStrings.Get(10798792) + " (" + conf.StrSTitle + "-" + BaseMesFilms.TranslateColumn(conf.StrSTitle) + ")"); // sort title
          choiceGlobalMappings.Add("sorttitle");

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1) return;

          int selection = dlg3.SelectedLabel;
          string strUserItemSelection = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();

          dlg3.SetHeading(GUILocalizeStrings.Get(10798772)); // Choose field ...
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
          if (selection > 10) // title fields
          {
            ArrayList displayItems = GetDisplayItems("titles");
            foreach (string[] displayItem in displayItems)
            {
              dlg3.Add(displayItem[0] + "-" + displayItem[1]);
              choiceGlobalMappings.Add(displayItem[0]);
            }
          }
          else // display item fields
          {
            ArrayList displayItems = GetDisplayItems("viewitems");
            foreach (string[] displayItem in displayItems)
            {
              dlg3.Add(displayItem[1] + " (" + displayItem[0] + ")");
              choiceGlobalMappings.Add(displayItem[0]);
            }
          }

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1) return;
          string wproperty = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();
          LogMyFilms.Debug("Display Options - new field: '" + wproperty + "', new Label: '" + BaseMesFilms.TranslateColumn(wproperty) + "'.");
          UpdateUseritemWithValue(strUserItemSelection, wproperty);
          UpdateUserItems(); // save to currentconfig - save time for WinDeInit
          //Configuration.SaveConfiguration(Configuration.currentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
          //Load_Config(Configuration.currentConfig, true);
          MyFilmsDetail.Init_Detailed_DB(false); // clear properties 
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          return;
          #endregion

        case "userprofilename":
          #region choose user profile name
          Change_UserProfileName();
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "UserProfileName", MyFilms.conf.StrUserProfileName);
          using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UserProfileName", conf.StrUserProfileName);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Debug("UserProfileName - change to '" + conf.StrUserProfileName + "'");
          //Configuration.SaveConfiguration(Configuration.currentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
          //Load_Config(Configuration.currentConfig, true);
          MyFilmsDetail.Init_Detailed_DB(false); // clear properties 
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          MyFilmsDetail.setGUIProperty("user.onlinestatus", Helper.GetUserOnlineStatus(conf.StrUserProfileName));
          return;
          #endregion

        case "userprofilenamedelete":
          #region delete user profile name
          if (Delete_UserProfileName()) return;
          break;
          #endregion

        case "globalwikihelp":
          #region Wiki Help
          if (Helper.IsBrowseTheWebAvailableAndEnabled)
          {
            //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser
            //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            const string url = "http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films";
            const string zoom = "150";

            //Load Webbrowserplugin with the URL
            LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url + "'");
            GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
            MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation);
            GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false);
            MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation);
            GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
          }
          else
          {
            GUIUtils.ShowOKDialog("BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.BrowseTheWeb), "", "");
          }
          break;
          #endregion

        case "about":
          #region About Box
          string infoBackgroundProcess = string.Empty;
          infoBackgroundProcess = bgUpdateFanart.IsBusy ? "running (fanart & artwork)" : "not active";
          System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
          GUIUtils.ShowOKDialog(
            "MyFilms Version = 'V" + asm.GetName().Version + "'", 
            "MyFilms Operations Mode = '" + Configuration.PluginMode + "'", 
            "MyFilms Background Process = '" + infoBackgroundProcess + "'", 
            "");
          break;
          #endregion

        case "globalunwatchedfilter":
          #region Global overlayfilter for unwatched movies ...
          conf.GlobalUnwatchedOnly = !conf.GlobalUnwatchedOnly;
          if (conf.GlobalUnwatchedOnly)
          {
            GlobalFilterStringUnwatched = CreateGlobalUnwatchedFilter();
            MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
          }
          else
          {
            GlobalFilterStringUnwatched = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
          }
          LogMyFilms.Info("Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbisonline":
          #region GlobalFilterIsOnline
          GlobalFilterIsOnlineOnly = !GlobalFilterIsOnlineOnly;
          LogMyFilms.Info("Global filter for IsOnline available media files is now set to '" + GlobalFilterIsOnlineOnly + "'");
          if (GlobalFilterIsOnlineOnly)
          {
            GlobalFilterStringIsOnline = "IsOnline like 'true' AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.isonline", "true");
          }
          else
          {
            GlobalFilterStringIsOnline = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.isonline");
          }
          LogMyFilms.Info("(SetGlobalFilterString IsOnline) - 'GlobalFilterStringIsOnline' = '" + GlobalFilterStringIsOnline + "'");
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbtrailer":
          #region GlobalFilterTrailersOnly
          GlobalFilterTrailersOnly = !GlobalFilterTrailersOnly;
          LogMyFilms.Info("Global filter for Trailers Only is now set to '" + GlobalFilterTrailersOnly + "'");
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          if (GlobalFilterTrailersOnly)
          {
            GlobalFilterStringTrailersOnly = conf.StrStorageTrailer + " not like '' AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.trailersonly", "true");
          }
          else
          {
            GlobalFilterStringTrailersOnly = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly");
          }
          LogMyFilms.Info("(SetGlobalFilterString Trailers) - 'GlobalFilterStringTrailersOnly' = '" + GlobalFilterStringTrailersOnly + "'");
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbrating":
          #region GlobalFilterMinRating
          GlobalFilterMinRating = !GlobalFilterMinRating;
          LogMyFilms.Info("Global filter for MinimumRating is now set to '" + GlobalFilterMinRating + "'");
          if (GlobalFilterMinRating)
          {
            GlobalFilterStringMinRating = "Rating > " + conf.StrAntFilterMinRating.Replace(",", ".") + " AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.minrating", "true");
            MyFilmsDetail.setGUIProperty("globalfilter.minratingvalue", conf.StrAntFilterMinRating);
          }
          else
          {
            GlobalFilterStringMinRating = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
            MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
          }
          LogMyFilms.Info(
            "(SetGlobalFilterString MinRating) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbsetrating":
          #region Set global value for minimum Rating to restrict movielist
          LogMyFilms.Info("(FilterDbSetRating) - 'AntFilterMinRating' current setting = '" + conf.StrAntFilterMinRating + "', current decimalseparator: '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + "'");
          MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(ID_MyFilmsDialogRating);
          if (conf.StrAntFilterMinRating.Length > 0)
          {
            decimal wrating = Convert.ToDecimal(conf.StrAntFilterMinRating.Replace(",", "."), CultureInfo.InvariantCulture);
            dlgRating.Rating = wrating;
          }
          else dlgRating.Rating = 0;
          dlgRating.SetTitle(GUILocalizeStrings.Get(1079881));
          dlgRating.DoModal(GetID);

          conf.StrAntFilterMinRating = dlgRating.Rating.ToString("0.0", CultureInfo.InvariantCulture);
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "AntFilterMinRating", conf.StrAntFilterMinRating);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", conf.StrAntFilterMinRating);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("(FilterDbSetRating) - 'AntFilterMinRating' changed to '" + conf.StrAntFilterMinRating + "'");
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          if (GlobalFilterMinRating)
          {
            GlobalFilterStringMinRating = "Rating > " + conf.StrAntFilterMinRating.Replace(",", ".") + " AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.minrating", "true");
            MyFilmsDetail.setGUIProperty("globalfilter.minratingvalue", conf.StrAntFilterMinRating);
          }
          else
          {
            GlobalFilterStringMinRating = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
            MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
          }
          LogMyFilms.Info("(SetGlobalFilterString) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filter3d":
          #region GlobalFilter3D
          LogMyFilms.Info("(filter3d) - 'GlobalFilterMovieFormat3D' current setting = '" + GlobalFilterMovieFormat3D + "'");
          var dlg3d = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg3d == null) return;
          dlg3d.Reset();
          dlg3d.SetHeading(string.Format(GUILocalizeStrings.Get(10798972), GlobalFilterMovieFormat3D)); // current setting....
          dlg3d.Add(GUILocalizeStrings.Get(10798973)); //all
          dlg3d.Add(GUILocalizeStrings.Get(10798974)); //2D only
          dlg3d.Add(GUILocalizeStrings.Get(10798975)); //3D only
          dlg3d.DoModal(GetID);

          switch (dlg3d.SelectedLabel)
          {
            case -1:
              return;
            case 0:
              GlobalFilterStringMovieFormat3D = string.Empty;
              GlobalFilterMovieFormat3D = GUILocalizeStrings.Get(10798973); // all
              MyFilmsDetail.setGUIProperty("globalfilter.movieformat", GlobalFilterMovieFormat3D);
              break;
            case 1:
              GlobalFilterStringMovieFormat3D = "(S3D is NULL OR S3D like '') AND ";
              GlobalFilterMovieFormat3D = GUILocalizeStrings.Get(10798974); // 2D only
              MyFilmsDetail.setGUIProperty("globalfilter.movieformat", GlobalFilterMovieFormat3D);
              break;
            case 2:
              GlobalFilterStringMovieFormat3D = "(S3D is NOT NULL AND S3D NOT like '') AND ";
              GlobalFilterMovieFormat3D = GUILocalizeStrings.Get(10798975); // 3D only
              MyFilmsDetail.setGUIProperty("globalfilter.movieformat", GlobalFilterMovieFormat3D);
              break;
          }
          LogMyFilms.Info("(filter3d) - 'GlobalFilterMovieFormat3D' changed to '" + GlobalFilterMovieFormat3D + "'");
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          LogMyFilms.Info("(SetGlobalFilterString) - 'GlobalFilterStringMovieFormat3D' = '" + GlobalFilterStringMovieFormat3D + "'");
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "isonlinecheck":
          #region Launch IsOnlineCheck in batch mode
          if (bgIsOnlineCheck.IsBusy)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079850), GUILocalizeStrings.Get(875)  + @"\n" + GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          AsynIsOnlineCheck();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "updatedb":
          #region Launch AMCUpdater in batch mode
          if (bgUpdateDB.IsBusy)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(875) + @"\n" + GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          if (MyFilmsDetail.GlobalLockIsActive(conf.StrFileXml))
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(1079854) + @"\n" + GUILocalizeStrings.Get(330)); //movie db already in use (locked)
            break;
          }
          AsynUpdateDatabase("");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "updatedbselect":
          #region Launch AMCUpdater in batch mode with selection of profile - e.g. to only update values etc.
          if (bgUpdateDB.IsBusy)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(875) + @"\n" + GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          if (MyFilmsDetail.GlobalLockIsActive(conf.StrFileXml))
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(1079854) + @"\n" + GUILocalizeStrings.Get(330)); //movie db already in use (locked)
            break;
          }
          string selectedprofile = "";

          GUIDialogMenu dlgprofile = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgprofile == null) return;
          dlgprofile.Reset();
          dlgprofile.SetHeading(GUILocalizeStrings.Get(1079843)); // Userdefined DB Update (AMCUpdater)
          var choiceAMCconfig = new List<string>();

          var dirsInf = new DirectoryInfo(conf.StrAMCUpd_cnf.Substring(0, conf.StrAMCUpd_cnf.LastIndexOf("\\")));
          bool isMePoDataDir = (conf.StrAMCUpd_cnf.Substring(0, conf.StrAMCUpd_cnf.LastIndexOf("\\")) == Config.GetDirectoryInfo(Config.Dir.Config).ToString());
          FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();

          foreach (FileSystemInfo sfi in sfiles.Where(sfi => sfi.Extension.ToLower() == ".xml" && (sfi.Name.StartsWith("MyFilmsAMCSettings")) || !isMePoDataDir))
          {
            dlgprofile.Add(sfi.Name);
            choiceAMCconfig.Add(sfi.FullName);
            LogMyFilms.Info("Custom AMCupdater update) - Add config: '" + sfi.FullName + "'");
          }

          dlgprofile.DoModal(GetID);
          if (dlgprofile.SelectedLabel == -1) return;
          //int selection = dlgprofile.SelectedLabel;
          selectedprofile = choiceAMCconfig[dlgprofile.SelectedLabel];
          dlgprofile.Reset();
          choiceAMCconfig.Clear();

          AsynUpdateDatabase(selectedprofile);
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "cancelupdatedb":
          #region stop background worker for AMCupdater
          if (bgUpdateDB.IsBusy && bgUpdateDB.WorkerSupportsCancellation)
          {
            bgUpdateDB.CancelAsync();
          }
          else
          {
            GUIUtils.ShowOKDialog("AMC Updater cannot be stopped!"); // AMC Updater is stopping!
          }
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "downfanart":
          #region Launch Fanart download in batch mode
          if (bgUpdateFanart.IsBusy)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079862), GUILocalizeStrings.Get(921) + @"\n" + GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          AsynUpdateFanart();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "personinfos-all":
          // Search and update all personinfos from IMDB
          // Todo: Call Singlepersonupdate - maybe this function should also be available for single movies ? (less traffic)
          // ToDo: - first implement singlepersonupdate ...
          break;

        case "trailer-all":
          #region Launch "Search and register all trailers for all movies in DB" in batch mode
          //if (bgUpdateTrailer.IsBusy)
          //{
          //    ShowMessageDialog(GUILocalizeStrings.Get(10798694), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
          //    break;
          //}
          //AsynUpdateTrailer();
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          //break;

          // AntMovieCatalog ds = new AntMovieCatalog();

          DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
          //Now build a list of valid movies in w_index with Number registered
          List<string[]> movielist = wr.Select(wsr => new string[] {wsr["Number"].ToString(), wsr["TranslatedTitle"].ToString()}).ToList();
          int wIndexCount = movielist.Count;
          LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Number of Records found: " + wIndexCount);

          bool doExtendedSearch = false;
          // if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(10798940), GUILocalizeStrings.Get(10798803))) doExtendedSearch = true; // Trailer // Include extended directories ?
          // if (!(GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(10798800), GUILocalizeStrings.Get(10798801) + "\n" + string.Format(GUILocalizeStrings.Get(10798802), wr.Length.ToString()), true))) break; // Warning: Long runtime ! // //should really the trailer search be started // for <xx> movies ?
          GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798940)); // Trailer
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798803)); // Include extended directories ?
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed) doExtendedSearch = true;

          dlgYesNo.Reset();
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798800)); // Warning: Long runtime !
          dlgYesNo.SetLine(1, GUILocalizeStrings.Get(10798801)); //should really the trailer search be started
          dlgYesNo.SetLine(2, string.Format(GUILocalizeStrings.Get(10798802), wr.Length)); // for <xx> movies ?
          dlgYesNo.DoModal(GetID);
          if (!(dlgYesNo.IsConfirmed)) break;

          GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
          if (dlgPrgrs != null)
          {
            dlgPrgrs.Reset();
            dlgPrgrs.DisplayProgressBar = true;
            dlgPrgrs.ShowWaitCursor = false;
            dlgPrgrs.DisableCancel(true);
            dlgPrgrs.SetHeading("MyFilms Trailer Registration");
            dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
            //dlgPrgrs.SetLine(1, "Register Trailers ...");
            dlgPrgrs.Percentage = 0;
          }
          new Thread(delegate()
              {
                for (int i = 0; i < wIndexCount; i++)
                {
                  try
                  {
                    string title = wr[i][conf.StrTitle1].ToString();
                    LogMyFilms.Debug("GlobalSearchTrailerLocal - Number: '" + i + "' - Index to search: '" + movielist[i][0] + "'");
                    // MyFilmsDetail.setGUIProperty("statusmessage", "Register trailer for '" + title + "'");
                    if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Register trailer for '" + title + "'");
                    int iprogress = i * 100 / wIndexCount;
                    if (iprogress > 100) iprogress = 100;
                    if (dlgPrgrs != null) dlgPrgrs.Percentage = iprogress;
                    MyFilmsDetail.SearchTrailerLocal(r, Convert.ToInt32(i), doExtendedSearch, false);
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("GlobalSearchTrailerLocal - index: '" + i + "', Exception: '" + ex.Message + "', Stacktrace: '" + ex.StackTrace + "'");
                  }
                }
                MyFilmsDetail.Update_XML_database();
                
                if (dlgPrgrs != null) dlgPrgrs.Percentage = 100;
                dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846));
                Thread.Sleep(50);
                dlgPrgrs.Close(); // Done...
                GUIWindowManager.SendThreadCallbackAndWait(
                  (p1, p2, data) =>
                  {
                    // this will be executed after background thread finished
                    Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    MyFilmsDetail.clearGUIProperty("statusmessage");
                    // MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(10798624), "Trailer Search finished");
                    GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798695));
                    return 0;
                  },
                  0,
                  0,
                  null);
              }) { Name = "GlobalTrailerUpdate", IsBackground = true }.Start();
          return;
          #endregion

        case "trailer-all-download":
          // Todo: Implement loop to load all trailers
          break;

        case "incomplete-movie-data":
          SearchIncompleteMovies();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // set focus to list
          break;

        case "showemptyvaluesinviews":
          #region showemptyvaluesinviews
          conf.BoolShowEmptyValuesInViews = !conf.BoolShowEmptyValuesInViews;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "ShowEmptyValuesInViews", conf.StrGrabber_ChooseScript); // disabled, as we do not keep the setting persistant in config
          LogMyFilms.Info("Grabber Option 'show empty values in views' changed to " + conf.BoolShowEmptyValuesInViews);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          Refreshfacade();
          Change_Menu_Action("globaloptions");
          #endregion
          break;
        case "choosescript":
          #region choosescript
          conf.StrGrabber_ChooseScript = !conf.StrGrabber_ChooseScript;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "Grabber_ChooseScript", conf.StrGrabber_ChooseScript);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_ChooseScript", conf.StrGrabber_ChooseScript);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Grabber Option 'use always that script' changed to " + conf.StrGrabber_ChooseScript);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          Change_Menu_Action("globaloptions");
          #endregion
          break;
        case "findbestmatch":
          #region findbestmatch
          conf.StrGrabber_Always = !conf.StrGrabber_Always;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "Grabber_Always", conf.StrGrabber_Always);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_Always", conf.StrGrabber_Always);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Grabber Option 'try to find best match...' changed to " + conf.StrGrabber_Always);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "amcimportifinternetlookupfails":
          #region amcimportifinternetlookupfails
          bool importonfailvalue = (amcXmlConfig.ReadAMCUXmlConfig(conf.StrAMCUpd_cnf, "Import_File_On_Internet_Lookup_Failure", "").ToLower() == "true");
          importonfailvalue = !importonfailvalue;
          amcXmlConfig.WriteAMCUXmlConfig(conf.StrAMCUpd_cnf, "Import_File_On_Internet_Lookup_Failure", importonfailvalue ? "True" : "False");
          LogMyFilms.Info("AMCupdater import option 'Import_File_On_Internet_Lookup_Failure' changed to " + importonfailvalue);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "amcpurgeorphans":
          #region amcpurgeorphans
          bool amcpurgeorphans = (amcXmlConfig.ReadAMCUXmlConfig(conf.StrAMCUpd_cnf, "Purge_Missing_Files", "").ToLower() == "true");
          amcpurgeorphans = !amcpurgeorphans;
          amcXmlConfig.WriteAMCUXmlConfig(conf.StrAMCUpd_cnf, "Purge_Missing_Files", (amcpurgeorphans) ? "True" : "False");
          LogMyFilms.Info("AMCupdater import option 'Purge_Missing_Files' changed to " + amcpurgeorphans);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        //case "alwayslistforgroups":
        //  MyFilms.conf.UseListViewForGoups = !MyFilms.conf.UseListViewForGoups;
        //  XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "UseListviewForGroups", MyFilms.conf.UseListViewForGoups);
        //  LogMyFilms.Info("Update Option 'use list view for groups ...' changed to " + MyFilms.conf.UseListViewForGoups);
        //  this.Change_Menu_Action("globaloptions");
        //  break;

        case "alwaysdefaultview":
          #region alwaysdefaultview
          conf.AlwaysDefaultView = !conf.AlwaysDefaultView;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "AlwaysDefaultView", conf.AlwaysDefaultView);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AlwaysDefaultView", conf.AlwaysDefaultView);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'use always default view...' changed to " + conf.AlwaysDefaultView);
          Change_Menu_Action("globaloptions");
          #endregion
          break;
        case "changedefaultview":
          #region change default view
          GUIDialogMenu dlgdef = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgdef == null) return;
          dlgdef.Reset();

          dlgdef.SetHeading(string.Format(GUILocalizeStrings.Get(1079841), conf.StrViewDfltItem)); // change default view (current default vew)
          List<string> choiceViewDefaultItems = new List<string>();
          if (conf.StrViewDfltItem == GUILocalizeStrings.Get(1079819))
            dlgdef.Add(GUILocalizeStrings.Get(1079819) + " (*)"); // menu
          else
            dlgdef.Add(GUILocalizeStrings.Get(1079819)); // menu
          choiceViewDefaultItems.Add(GUILocalizeStrings.Get(1079819));
          int position = 0;
          foreach (MFview.ViewRow customView in conf.CustomViews.View)
          {
            dlgdef.Add(customView.Label);
            choiceViewDefaultItems.Add(customView.Label);
            position++;
            if (conf.StrViewDfltItem == customView.Label)
              dlgdef.SelectedLabel = position;
          }
          
          dlgdef.DoModal(GetID);
          if (dlgdef.SelectedLabel == -1) return;

          if (choiceViewDefaultItems[dlgdef.SelectedLabel] == GUILocalizeStrings.Get(1079819)) // Views Menu
          {
            conf.StrViewDfltItem = choiceViewDefaultItems[dlgdef.SelectedLabel];
            conf.StrViewDfltText = "";
          }
          else
            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => choiceViewDefaultItems[dlgdef.SelectedLabel] == viewRow.Label))
            {
              conf.StrViewDfltItem = choiceViewDefaultItems[dlgdef.SelectedLabel];
              conf.StrViewDfltText = (viewRow.Value.Length > 0) ? viewRow.Value : "";
            }

          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "ViewDfltItem", conf.StrViewDfltItem);
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "ViewDfltText", conf.StrViewDfltText);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'change default view' changed to " + conf.StrViewDfltItem);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "alwaysdefaultconfig":
          #region alwaysdefaultconfig
          conf.AlwaysShowConfigMenu = !conf.AlwaysShowConfigMenu;
          // AlwaysShowConfigMenu = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false);
          // XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", conf.AlwaysShowConfigMenu);
          using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", conf.AlwaysShowConfigMenu);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'use always default config...' changed to " + conf.AlwaysShowConfigMenu);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "askforplaybackquality":
          #region askforplaybackquality
          conf.BoolAskForPlaybackQuality = !conf.BoolAskForPlaybackQuality;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "AlwaysDefaultView", conf.AlwaysDefaultView);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AskForPlaybackQuality", conf.BoolAskForPlaybackQuality);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'Always ask for playback quality for online content...' changed to " + conf.BoolAskForPlaybackQuality);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "cacheonlinetrailer":
          #region cacheonlinetrailer
          conf.CacheOnlineTrailer = !conf.CacheOnlineTrailer;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "CacheOnlineTrailer", conf.CacheOnlineTrailer);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "CacheOnlineTrailer", conf.CacheOnlineTrailer);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'Cache trailers for online content' changed to " + conf.CacheOnlineTrailer);

          if (!conf.CacheOnlineTrailer && TrailertoDownloadQueue.Count > 0)
          {
            GUIDialogYesNo dlgYN = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (dlgYN != null)
            {
              dlgYN.SetHeading(GUILocalizeStrings.Get(1079701) + " (" + TrailertoDownloadQueue.Count + ")"); // Delete Trailer Download Queue ... 
              dlgYN.SetLine(1, GUILocalizeStrings.Get(927));// warning
              dlgYN.SetLine(2, GUILocalizeStrings.Get(1079702)); //Delete all trailers from download queue ?
              dlgYN.DoModal(GetID);
              if (dlgYN.IsConfirmed)
              {
                lock (TrailertoDownloadQueue)
                {
                  TrailertoDownloadQueue.Clear();
                }
              }
            }
          }
          
          Change_Menu_Action("globaloptions");
          #endregion
          break;
        
        case "changedefaultconfig":
          #region changedefaultconfig
          string currentDefaultConfig = "";
          using (XmlSettings xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            currentDefaultConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
          }
          var dlgcfg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgcfg == null) return;
          dlgcfg.Reset();
          dlgcfg.SetHeading(string.Format(GUILocalizeStrings.Get(1079842), currentDefaultConfig)); // change default config (current default config)

          using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            for (int i = 0; i < (int)mesFilmsNbConfig; i++)
            {
              dlgcfg.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));
              if (dlgcfg.SelectedLabelText == currentDefaultConfig) dlgcfg.SelectedLabel = i;
            }

            // add empty value to be able to remove start config
            dlgcfg.Add("<" + GUILocalizeStrings.Get(10798774) + ">"); // empty

            dlgcfg.DoModal(GetID);
            if (dlgcfg.SelectedLabel == -1) return;

            if (dlgcfg.SelectedLabelText == ("<" + GUILocalizeStrings.Get(10798774) + ">"))
            {
              currentDefaultConfig = "";
            }
            else if (dlgcfg.SelectedLabelText.Length > 0)
            {
              string catalog = xmlConfig.ReadXmlConfig("MyFilms", dlgcfg.SelectedLabelText, "AntCatalog", string.Empty);
              if (!File.Exists(catalog))
              {
                GUIUtils.ShowOKDialog("Cannot set this Configuration:", "'" + dlgcfg.SelectedLabelText + "'", "Verify your settings !", "");
                return;
              }
              else
              {
                currentDefaultConfig = dlgcfg.SelectedLabelText;
              }
            }
          }

          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            if (string.IsNullOrEmpty(currentDefaultConfig))
            {
              xmlSettings.RemoveEntry("MyFilms", "MyFilms", "Default_Config");
            }
            else
            {
              xmlSettings.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", currentDefaultConfig);
            }
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'change default config' changed to '" + currentDefaultConfig + "'");
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "woluserdialog":
          #region woluserdialog
          conf.StrCheckWOLuserdialog = !conf.StrCheckWOLuserdialog;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, "WOL-Userdialog", MyFilms.conf.StrCheckWOLuserdialog);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WOL-Userdialog", conf.StrCheckWOLuserdialog);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'use WOL userdialog ...' changed to " + conf.StrCheckWOLuserdialog);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        default: // other nonspecific views ...
          {
            LogMyFilms.Error("Change_View(" + choiceView + ") is not a known Menu Action !!!");
            break;
          }
      }
    }

    private string CreateGlobalUnwatchedFilter()
    {
      string username = conf.StrUserProfileName;
      string filter = (conf.EnhancedWatchedStatusHandling) ? BaseMesFilms.MultiUserStateField + " like '*" + StringExtensions.EscapeLikeValue(username) + ":0*" + "' AND " : GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
      LogMyFilms.Debug("CreateGlobalUnwatchedFilter() - filter = '" + filter + "'");
      return filter;
    }

    private bool ChooseNewConfig()
    {
      GUIControl.ShowControl(GetID, 34); // hide elements in skin
      currentFanartList.Clear();
      Fanartstatus(false); // disable Fanart
      string oldconfig = Configuration.CurrentConfig; // save current config, in case we need to abort and return to this
      string newConfig = Configuration.ChoiceConfig(GetID);
      newConfig = Configuration.ControlAccessConfig(newConfig, GetID);
      if (string.IsNullOrEmpty(newConfig)) // if user escapes dialog or bad value leave system unchanged
      {
        GUIControl.HideControl(GetID, 34); // show elements in skin
        return false;
      }
      else
      {
        //Change "Config":
        if (facadeFilms.SelectedListItem != null) 
          Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
        else 
          Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
        Configuration.CurrentConfig = newConfig;
        
        bool isvalidDb = Load_Config(newConfig, true, null);
        if (!isvalidDb) // abort, if it is invalid DB
        {
          LogMyFilms.Error("ChooseNewConfig(): DB has unsupported AMC4.2 format - returning to previous config !");
          GUIUtils.ShowErrorDialog("Movie DB has invalud AMC4.2 format - returning !");
          Configuration.CurrentConfig = oldconfig;
          Load_Config(oldconfig, true, null);
          GUIControl.HideControl(GetID, 34); // show elements in skin
          return false;
        }

        currentFanartList.Clear(); // clear fanart list
        _navigationStack.Clear(); // clear navigation stack
        internalLoadParam = null; // clear internal start params
        InitMainScreen(false); // reset all properties and values
        InitGlobalFilters(false); // reset global filters, when loading new config !

        ClearFacade(); // facadeFilms.Clear();        facadeFilms.ListLayout.Clear();
        InitialIsOnlineScan = false; // set false, so facade does not display false media status !!!
        InitialStart = true; //Set to true to make sure initial View is initialized for new DB view

        new Thread(delegate()
        {
          {
            MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();

            if (InitialStart)
            {
              Fin_Charge_Init(true, true); //Guzzi: need to always load default view on initial start, even if always default view is disabled ...
              //Loadfacade(); // load facade threaded...

              if (conf.StrFileType != Configuration.CatalogType.AntMovieCatalog3 && conf.StrFileType != Configuration.CatalogType.AntMovieCatalog4Xtended)
              {
                if (ImportComplete != null && conf.AllowTraktSync) // trigger sync to trakt page after importer finished
                {
                  ImportComplete();
                  LogMyFilms.Debug("Change_View(): Fired 'ImportCompleted' event to trigger sync to trakt page after initial DB import of external Catalog is finished !");
                }
              }
            }
            else
            {
              Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
              //if (conf.AlwaysDefaultView)
              //  Loadfacade(); // load threaded Fin_Charge_Init(true, true)
              //else
              //  Refreshfacade(); // load threaded Fin_Charge_Init(false, true)
            }

            // launch DB watcher for multiseat
            InitFSwatcher();
            InitAmcImporter(conf); // load watcher for movie directories

            // Launch Background availability scanner, if configured in setup
            if (conf.ScanMediaOnStart && InitialStart)
            {
              LogMyFilms.Debug("Launching Availabilityscanner - Initialstart = '" + InitialStart + "'");
              AsynIsOnlineCheck();
            }
            InitialStart = false; // Guzzi: Set InitialStart to false after initialization done

            Fanartstatus(conf.StrFanart);

            MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
            //GUIControl.ShowControl(GetID, 34);
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // after thread finished ...
            }
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

      }
      return true;
    }

    private void Change_UserProfileName()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(string.Format(GUILocalizeStrings.Get(1079840), conf.StrUserProfileName)); // Choose User Profile Name ... GUILocalizeStrings.Get(1079840), conf.StrUserProfileName)
      List<string> choiceGlobalUserProfileName = new List<string>();
      string oldUserProfileName = conf.StrUserProfileName;

      dlg.Add("<" + GUILocalizeStrings.Get(10798630) + ">"); // Add User ...
      choiceGlobalUserProfileName.Add("");

      // add Trakt user, if there is any configured:
      if (Helper.IsTraktAvailableAndEnabled)
      {
        if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
        {
          var userlist = Helper.GetTraktUserList();

          // Show List of users to login as
          foreach (var userlogin in userlist)
          {
            dlg.Add(userlogin + " (Trakt)");
            choiceGlobalUserProfileName.Add(userlogin);
          }
        }
        else
        {
          string currentTraktuser = Helper.GetTraktUser();
          if (!string.IsNullOrEmpty(currentTraktuser))
          {
            dlg.Add(currentTraktuser + " (Trakt)");
            choiceGlobalUserProfileName.Add(currentTraktuser);
          }
        }
      }

      // Add already existing UserProfileNames - example of string value: "Global:3|Mike:0|Sandy:1"
      foreach (var userprofilename in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens).Select(sr => sr[BaseMesFilms.MultiUserStateField].ToString().Trim()).Select(strEnhancedWatchedValue => strEnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).SelectMany(split1 => split1.Where(s => s.Contains(":")).Select(s => s.Substring(0, s.IndexOf(":"))).Where(userprofilename => !choiceGlobalUserProfileName.Contains(userprofilename) && userprofilename != GlobalUsername)))
      {
        if (userprofilename == "")
        {
          dlg.Add("<" + GUILocalizeStrings.Get(10798774) + ">");
          choiceGlobalUserProfileName.Add("<" + GUILocalizeStrings.Get(10798774) + ">");
        }
        else
        {
          dlg.Add(userprofilename);
          choiceGlobalUserProfileName.Add(userprofilename);
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;

      string strUserProfileNameSelection = choiceGlobalUserProfileName[dlg.SelectedLabel];
      switch (strUserProfileNameSelection)
      {
        case "": // new value
          var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
          if (null == keyboard) return;
          keyboard.Reset();
          keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
          keyboard.Text = ""; // Default string is empty
          keyboard.DoModal(GetID);
          if (keyboard.IsConfirmed && !string.IsNullOrEmpty(keyboard.Text))
          {
            #region create a backup copy of DB file
            string backupcopy = conf.StrFileXml.Replace(".xml", " - backup - " + DateTime.Now.ToString().Replace(":", "-") + ".xml").Replace("/", "-");
            try
            {
              File.Copy(conf.StrFileXml, backupcopy, true);
            }
            catch (Exception ex)
            {
              LogMyFilms.Error("Change_UserProfileName() - error creating backup copy - Exception: ", ex.Message);
            }
            LogMyFilms.Debug("Change_UserProfileName() - finished creating a DB backupcopy to '" + backupcopy + "'");
            #endregion
            conf.StrUserProfileName = keyboard.Text;
          }
          else
            return;
          break;
        default:
          conf.StrUserProfileName = (strUserProfileNameSelection != ("<" + GUILocalizeStrings.Get(10798774) + ">")) ? strUserProfileNameSelection : "";
          // check, if Traktuser has to be "switched"
          if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
          {
            if (conf.StrUserProfileName != Helper.GetTraktUser())
            {
              bool success = Helper.ChangeTraktUser(conf.StrUserProfileName);
              if (!success) LogMyFilms.Info("An error occurred changing current Trakt user login credentials!");
            }
          }
          break;
      }

      #region save new userprofilename to config
      using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
      {
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UserProfileName", conf.StrUserProfileName);
      }
      // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
      LogMyFilms.Info("Update Config 'UserProfileName' changed to " + conf.StrUserProfileName);
      #endregion

      #region switch user data to dedicated fields (WatchedDate, RatingUser, userdefined 'Watched' field)
      LogMyFilms.Debug("Change_UserProfileName() - Switch user data to dedicated fields (WatchedDate, RatingUser, userdefined 'Watched' field)");
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();

      foreach (AntMovieCatalog.MovieRow sr in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens))
      {
        #region sync MUS state with direct DB fields for user rating, watched and Favorite
        if (conf.EnhancedWatchedStatusHandling)
        {
          MultiUserData userData;
          if (sr[BaseMesFilms.MultiUserStateField] != Convert.DBNull) userData = new MultiUserData(sr.MultiUserState);
          else
          {
            #region if the former user was the default user, migrate it to MUS!
            if (oldUserProfileName == DefaultUsername)
            {
              #region migrate status from configured (enhanced)watched field to new MultiUserStates
              if (sr[conf.StrWatchedField].ToString().Contains(":"))
              {
                // old field was already multiuserdata - migrate it!
                userData = new MultiUserData(sr[conf.StrWatchedField].ToString());
                sr[conf.StrWatchedField] = userData.GetUserState(DefaultUsername).Watched ? "true" : conf.GlobalUnwatchedOnlyValue.ToLower();
              }
              else
              {
                // old field was standard watched data - create MUS and add watched for current user
                bool tmpwatched = (!string.IsNullOrEmpty(conf.GlobalUnwatchedOnlyValue) &&
                              sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower() &&
                              sr[conf.StrWatchedField].ToString().Length > 0);
                userData = new MultiUserData(string.Empty);
                userData.SetWatched(DefaultUsername, tmpwatched);
                if (sr["RatingUser"] != Convert.DBNull)
                  userData.SetRating(DefaultUsername, (decimal)sr["RatingUser"]);
              }
              sr[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
              sr["DateWatched"] = (userData.GetUserState(DefaultUsername).WatchedDate == MultiUserData.NoWatchedDate || userData.GetUserState(DefaultUsername).Watched == false) ? Convert.DBNull : userData.GetUserState(DefaultUsername).WatchedDate;
              sr["RatingUser"] = (userData.GetUserState(DefaultUsername).UserRating == MultiUserData.NoRating) ? Convert.DBNull : userData.GetUserState(DefaultUsername).UserRating;
              #endregion
            }
            #endregion
            else
            {
              userData = new MultiUserData(string.Empty);
            }
          }

          var user = userData.GetUserState(conf.StrUserProfileName);
          sr["DateWatched"] = (user.WatchedDate == MultiUserData.NoWatchedDate || user.Watched == false) ? System.Convert.DBNull : user.WatchedDate;
          sr["RatingUser"] = user.UserRating == -1 ? Convert.DBNull : user.UserRating;
          if (conf.StrWatchedField.Length > 0) sr[conf.StrWatchedField] = user.Watched ? "true" : conf.GlobalUnwatchedOnlyValue;
          if (conf.StrUserProfileName.Length > 0 && sr["RatingUser"] != Convert.DBNull && sr.RatingUser != MultiUserData.NoRating)
          {
            string newValue = sr.RatingUser > MultiUserData.FavoriteRating ? MultiUserData.Add(sr["Favorite"].ToString(), conf.StrUserProfileName) : MultiUserData.Remove(sr["Favorite"].ToString(), conf.StrUserProfileName);
            sr["Favorite"] = string.IsNullOrEmpty(newValue) ? Convert.DBNull : newValue;
          }
          sr[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
        }
        #endregion
      }
      stopwatch.Stop();
      LogMyFilms.Debug("Change_UserProfileName() - finished updating DB fields ... (" + stopwatch.ElapsedMilliseconds + " ms)");
      #endregion

      #region save xml DB
      stopwatch.Reset(); stopwatch.Start();
      MyFilmsDetail.Update_XML_database();
      stopwatch.Stop();
      LogMyFilms.Debug("Change_UserProfileName() - finished saving updated DB ... (" + stopwatch.ElapsedMilliseconds + " ms)");
      #endregion
    }

    private bool Delete_UserProfileName()
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return false;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079818)); // Delete User Profile Name
      List<string> choiceGlobalUserProfileName = new List<string>();

      // add Trakt user, if there is any configured:
      if (Helper.IsTraktAvailableAndEnabled)
      {
        if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
        {
          List<string> userlist = Helper.GetTraktUserList();

          // Show List of users to login as
          foreach (var userlogin in userlist.Where(userlogin => userlogin != conf.StrUserProfileName))
          {
            dlg.Add(userlogin + " (Trakt)");
            choiceGlobalUserProfileName.Add(userlogin);
          }
        }
        else
        {
          string currentTraktuser = Helper.GetTraktUser();
          if (!string.IsNullOrEmpty(currentTraktuser) && currentTraktuser != conf.StrUserProfileName)
          {
            dlg.Add(currentTraktuser + " (Trakt)");
            choiceGlobalUserProfileName.Add(currentTraktuser);
          }
        }
      }

      // Add already existing UserProfileNames - example of string value: "Global:3|Mike:0|Sandy:1"
      foreach (var userprofilename in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens).Select(sr => sr[BaseMesFilms.MultiUserStateField].ToString().Trim()).Select(strEnhancedWatchedValue => strEnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).SelectMany(split1 => split1.Where(s => s.Contains(":")).Select(s => s.Substring(0, s.IndexOf(":"))).Where(userprofilename => !choiceGlobalUserProfileName.Contains(userprofilename) && userprofilename != GlobalUsername)).Where(userprofilename => userprofilename != conf.StrUserProfileName))
      {
        if (userprofilename == "")
        {
          dlg.Add("<" + GUILocalizeStrings.Get(10798774) + ">"); // empty
        }
        else
        {
          dlg.Add(userprofilename);
        }
        choiceGlobalUserProfileName.Add(userprofilename);
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return false;

      string strUserProfileNameSelection = choiceGlobalUserProfileName[dlg.SelectedLabel];
      // if (string.IsNullOrEmpty(strUserProfileNameSelection)) return false; // do we allow selection of empty userprofilename? It should not happen ....

      #region create a backup copy of DB file
      string backupcopy = conf.StrFileXml.Replace(".xml", " - backup - " + DateTime.Now.ToString().Replace(":", "-") + ".xml").Replace("/", "-");
      try
      {
        File.Copy(conf.StrFileXml, backupcopy, true);
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Delete_UserProfileName() - error creating backup copy - Exception: ", ex.Message);
      }
      LogMyFilms.Debug("Delete_UserProfileName() - finished creating a DB backupcopy to '" + backupcopy + "'");
      #endregion

      #region delete all userdata for that selected user profile name
      LogMyFilms.Debug("Delete_UserProfileName() - Delete username '" + strUserProfileNameSelection + "' from DB");
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      foreach (AntMovieCatalog.MovieRow sr in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens))
      {
        if (conf.EnhancedWatchedStatusHandling)
        {
          if (sr[BaseMesFilms.MultiUserStateField] != Convert.DBNull)
          {
            var userData = new MultiUserData(sr.MultiUserState);
            userData.DeleteUser(strUserProfileNameSelection);
            sr[BaseMesFilms.MultiUserStateField] = userData.ResultValueString();
          }
        }
      }
      stopwatch.Stop();
      LogMyFilms.Debug("Delete_UserProfileName() - finished removing UserProfileName '" + strUserProfileNameSelection + "' from  DB fields... (" + stopwatch.ElapsedMilliseconds + " ms)");
      #endregion

      #region update xml db
      MyFilmsDetail.Update_XML_database();
      #endregion
      return true;
    }

    private string Change_MovieGroupName(string movieTitle)
    {
      var choiceMovieGroupNames = new List<string>();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return "";
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079836) + " - " + movieTitle); // Add to box set ...
      dlg.Add("<" + GUILocalizeStrings.Get(1079838) + ">"); //     <String id="1079838">Enter new box set name ...</String> // choiceMovieGroupNames.Add("");

      foreach (DataRow sr in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens)) // Add already existing Movie Group Names - example of string value: "24\Season 1"
      {
        string sFullTitle = sr[conf.StrTitle1].ToString();
        int delimCnt = NewString.PosCount(conf.TitleDelim, sFullTitle, false);
        if (delimCnt > 0)
        {
          for (int i = 1; i < delimCnt + 1; i++)
          {
            string strMovieGroupName = NewString.NPosLeft(conf.TitleDelim, sFullTitle, i, false, false);
            if (!choiceMovieGroupNames.Contains(strMovieGroupName))
              choiceMovieGroupNames.Add(strMovieGroupName);
          }
        }
      }
      choiceMovieGroupNames.Sort();
      foreach (string choiceMovieGroupName in choiceMovieGroupNames)
      {
        dlg.Add(choiceMovieGroupName);
      }
      dlg.DoModal(GetID);

      if (dlg.SelectedLabel == -1) return "";
      if (dlg.SelectedLabel == 0) // new value
      {
        var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
        if (null == keyboard) return "";
        keyboard.Reset();
        keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
        keyboard.Text = ""; // Default string is empty
        keyboard.DoModal(GetID);
        if (keyboard.IsConfirmed && (!string.IsNullOrEmpty(keyboard.Text))) return keyboard.Text;
        else return "";
      }
      else
        return dlg.SelectedLabelText;
    }

    #region Main Context Menu (inactive)
    //protected override void OnShowContextMenu()
    //{
    //  try
    //  {
    //    GUIListItem currentitem = this.facadeFilms.SelectedListItem;
    //    if (currentitem == null) return;

    //    var dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
    //    if (dlg == null) return;

    //    do
    //    {
    //      dlg.Reset();
    //      GUIListItem pItem = null;
    #endregion

    public enum Menu
    {
      MainMenu,
      HiddenMenuMain,
      ContextMenuMain,
      SubmenuOptions,
      ClearMovieCover
    }

    private void ShowContextMenu(int menuaction = (int)Menu.MainMenu, Stack menunavigationstack = null, int lastposition = -1, bool iscontextmenu = true)
    {
      try
      {
        if (menunavigationstack == null) menunavigationstack = new Stack();
        GUIListItem currentitem = facadeFilms.SelectedListItem;
        var dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        GUIListItem pItem; // menu entries
        if (dlg == null) return;
        dlg.Reset();

        switch (menuaction)
        {
          case (int)Menu.MainMenu:
            #region main menu
            dlg.SetHeading(GUILocalizeStrings.Get(10799201)); // MyFilms Cover Manager

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799204)); // Download Covers ...
            pItem.ItemId = (int)Menu.SubmenuOptions;
            dlg.Add(pItem);

            #endregion
            break;

          case (int)Menu.SubmenuOptions:
            #region submenu options ...
            dlg.SetHeading(GUILocalizeStrings.Get(10799204)); // Download Covers ...

            pItem = new GUIListItem(GUILocalizeStrings.Get(10798766));  // Load single Cover ...
            pItem.ItemId = (int)Menu.ClearMovieCover;
            dlg.Add(pItem);

            #endregion
            break;
        }

        dlg.DoModal(GUIWindowManager.ActiveWindow);

        if (dlg.SelectedLabel == -1)
        {
          #region conditional return to main menu
          if (!iscontextmenu) return;
          else
          {
            if (menunavigationstack.Count > 0)
            {
              var obj = _navigationStack.Pop() as MyFilmsPlugin.Utils.MenuNavigationObject;
              ShowContextMenu((int)obj.MenuItem, menunavigationstack, (int)(Menu)obj.Position, iscontextmenu);
            }
            else
            {
              switch (menuaction)
              {
                case (int)Menu.MainMenu:
                  dlg.Reset();
                  return;
                case (int)Menu.SubmenuOptions:
                  menunavigationstack = null;
                  ShowContextMenu((int)Menu.MainMenu, menunavigationstack, -1, true);
                  break;
              }
            }
          }
          #endregion
        }

        // MyFilmsDetail.Searchtitles sTitles;
        menunavigationstack.Push(new MenuNavigationObject((Menu)menuaction, dlg.SelectedId));
        switch (dlg.SelectedId) // what was chosen?
        {
          case (int)Menu.SubmenuOptions:
            dlg.Reset();
            ShowContextMenu((int)Menu.MainMenu, menunavigationstack, dlg.SelectedId, true);
            break;
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Exception in Menu Processor: " + ex.Message);
      }
    }

    //--------------------------------------------------------------------------------------------
    //   Display Context Menu for Movie 
    //--------------------------------------------------------------------------------------------
    private void Context_Menu_Movie(int selecteditem, string contextmenuselection = "")
    {
      LogMyFilms.Debug("Context_Menu_Movie() - context = '" + conf.ViewContext + "', contextmenuselection = '" + contextmenuselection + "'");

      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      List<string> choice = new List<string>();
      MyFilmsDetail.Searchtitles sTitles;

      Context_Menu = true;
      if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) conf.MenuSelectedId = facadeFilms.SelectedListItemIndex; // remember current facade position for Menu refresh

      if (string.IsNullOrEmpty(contextmenuselection)) // if called as main menu, first build main menu
      {
        if (dlg == null) return;
        dlg.Reset();
        choice.Clear();
        dlg.SetHeading(GUILocalizeStrings.Get(1079904)); // Context options ...

        #region Moviecontext - add context searches
        if (facadeFilms.SelectedListItemIndex > -1 && !facadeFilms.SelectedListItem.IsFolder && conf.ViewContext != ViewContext.TmdbMovies)
        {
          dlg.Add(GUILocalizeStrings.Get(1079866)); //Search related movies by persons
          choice.Add("analogyperson");

          dlg.Add(GUILocalizeStrings.Get(10798614)); //Search related movies by property
          choice.Add("analogyproperty");
        }
        #endregion

        #region Moviecontext and Groupcontext and TMDBonline context - add OnlineVideos and MyFilms search
        if (facadeFilms.SelectedListItemIndex > -1 && (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll && conf.ViewContext != ViewContext.Group))
        {
          if (Helper.IsOnlineVideosAvailableAndEnabled)
          {
            dlg.Add(GUILocalizeStrings.Get(10798600)); // Search OnlineVideos ... TMDB Context Search for OnlineVideos
            choice.Add("searchonlinevideos");
          }

          if (conf.ViewContext == ViewContext.TmdbMovies || conf.ViewContext == ViewContext.Person)
          {
            dlg.Add(GUILocalizeStrings.Get(10798601)); // Search MyFilms ... TMDB Context Search for MyFilms (local)
            choice.Add("searchmyfilms");
          }

          if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Context: NZB Context Search for MyFilms (local)"))
          {
            if (Helper.IsMpNzbAvailableAndEnabled)
            {
              dlg.Add("Search NZBs ... "); // Search NZBs ... 
              choice.Add("searchnzbs");
            }
          }
        }

        #endregion

        #region Moviecontext
        if (facadeFilms.SelectedListItemIndex > -1 && !facadeFilms.SelectedListItem.IsFolder && conf.ViewContext != ViewContext.TmdbMovies)
        {
          dlg.Add(GUILocalizeStrings.Get(1079879)); //Search Infos to related persons (load persons in facadeview) - only available in filmlist
          choice.Add("moviepersonlist");

          if (facadeFilms.Focus && !facadeFilms.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
          {
            dlg.Add(GUILocalizeStrings.Get(10798709)); //play movie 
            choice.Add("playmovie");

            if (conf.ExternalPlayerPath.Length > 0 && File.Exists(conf.ExternalPlayerPath))
            {
              dlg.Add(GUILocalizeStrings.Get(10798500)); //play movie external player
              choice.Add("playmovieexternal");
            }
            if (Helper.IsBluRayPlayerLauncherAvailableAndEnabled)
            {
              dlg.Add(GUILocalizeStrings.Get(10798501)); //play movie BD player launcher
              choice.Add("playmoviebrplayerlauncher");
            }
          }

          if (Helper.FieldIsSet(conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
          {
            string trailercount = "";
            if (string.IsNullOrEmpty(r[facadeFilms.SelectedListItem.ItemId][conf.StrStorageTrailer].ToString().Trim()))
              trailercount = "0";
            else
            {
              string[] split1 = r[facadeFilms.SelectedListItem.ItemId][conf.StrStorageTrailer].ToString().Trim().Split(new Char[] { ';' });
              trailercount = split1.Length.ToString();
              if (trailercount != "0")
              {
                dlg.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")");//play trailer (<number trailers present>)
                choice.Add("playtrailer");
              }
            }

            // play random movies or trailers in "view context" (selected group)
            dlg.Add(GUILocalizeStrings.Get(10798980)); // play random trailers
            choice.Add("playrandomtrailers");
          }

          if (conf.GlobalUnwatchedOnlyValue != null && conf.StrWatchedField.Length > 0)
          {
            // set unwatched // set watched
            dlg.Add(facadeFilms.SelectedListItem.IsPlayed ? GUILocalizeStrings.Get(1079895) : GUILocalizeStrings.Get(1079894));
            choice.Add("togglewatchedstatus");
          }

          if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Context: TMDB API play trailer - not yet implemented!")) // if (conf.ViewContext != ViewContext.TmdbMovies)
          {
            //// not yet implemented
            //dlg.Add(GUILocalizeStrings.Get(10798710) + " (TMDB)");//play trailer
            //choice.Add("playtrailertmdb");

            //dlg.Add(GUILocalizeStrings.Get(10798710) + " - All (TMDB)");//play trailer
            //choice.Add("playtrailertmdball");
          }

          if (facadeFilms.Count > 1) // not for single movie display ...
          {
            dlg.Add(GUILocalizeStrings.Get(10798979)); // select random movie
            choice.Add("selectrandommovie");
            //if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.test, "Context: select random movie"))
            //{
            //}
          }

          if (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended) // add or remove movie from/to box set (hierarchy)
          {
            if (r[facadeFilms.SelectedListItem.ItemId][conf.StrTitle1].ToString().IndexOf(conf.TitleDelim, StringComparison.Ordinal) > 0)
            {
              dlg.Add(GUILocalizeStrings.Get(1079837)); // Remove from box set
              choice.Add("removefromcollection");
            }
            else
            {
              dlg.Add(GUILocalizeStrings.Get(1079836)); // Add to box set ...
              choice.Add("addtocollection");
            }

            if (facadeFilms.Count > 1) // options only available, if there is multiple entries in facade list
            {
              if (conf.ViewContext == ViewContext.MovieCollection)
              {
                dlg.Add(GUILocalizeStrings.Get(1079802)); // Remove all films from box set
                choice.Add("removefromcollectionall");
              }
              else
              {
                dlg.Add(GUILocalizeStrings.Get(1079801)); // Add all films to box set ...
                choice.Add("addtocollectionall");
              }
            }
          }

          if (conf.StrSuppressAutomatic || conf.StrSuppressManual) // delete options
          {
            dlg.Add(GUILocalizeStrings.Get(1079830));
            choice.Add("suppress");
          }

          // Enable/disable global overlay filter could be added here for faster access ?
          // ...
          //dlg.Add(GUILocalizeStrings.Get(10798714)); // "global filters ..."
          //upd_choice[ichoice] = "globalfilters";

          dlg.Add(GUILocalizeStrings.Get(10799501)); // "Show web details ..."
          choice.Add("submenuwebdetails");

          dlg.Add(GUILocalizeStrings.Get(10798702)); // "Updates ..."
          choice.Add("submenuupdates");

          if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml"))
          {
            dlg.Add(GUILocalizeStrings.Get(10798763)); // Cover Manager ...
            choice.Add("covermanager");
          }

          if (conf.StrFanart)
          {
            dlg.Add(GUILocalizeStrings.Get(10798767)); // Fanart Manager ...
            choice.Add("submenufanartmanager");
          }

          if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Context: Fanartmanager (new) -> new window!"))
          {
            if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsFanartManager.xml"))
            {
              dlg.Add(GUILocalizeStrings.Get(10798767) + " (new & wip)"); // Fanart Manager ... (new & wip)
              choice.Add("fanartmanager");
            }
          }

          dlg.Add(GUILocalizeStrings.Get(10799502)); // View Options ...
          choice.Add("submenuoptions");
        }
        else if (facadeFilms.SelectedListItemIndex > -1 && (conf.ViewContext == ViewContext.Movie || conf.ViewContext == ViewContext.MovieCollection)) // when films with active movie facade
        {
          // Movie list with a group/collection selected ->  show update submenu!
          dlg.Add(GUILocalizeStrings.Get(10798702)); // "Updates ..."
          choice.Add("submenuupdates");

          dlg.Add(GUILocalizeStrings.Get(10799502)); // View Options ...
          choice.Add("submenuoptions");
        }
        #endregion

        #region Personcontext
        if (facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder && IsPersonField(conf.WStrSort) && (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll))
        {
          //if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Context Artist: Show MP person dialog"))
          //{
          //  //dlg.Add(GUILocalizeStrings.Get(1079884)); //Show Infos of person (load persons detail dialog - MesFilmsActorDetails) - only available in personlist
          //  //choice.Add("artistdetail");
          //}

          dlg.Add(GUILocalizeStrings.Get(1079893)); // Filmographie (TMDB)
          choice.Add("artistdetailtmdb");

          dlg.Add(GUILocalizeStrings.Get(10799501)); // "Show web details ..."
          choice.Add("submenuwebdetails");

          dlg.Add(GUILocalizeStrings.Get(10798702)); // "Updates ..."
          choice.Add("submenuupdates");

          dlg.Add(GUILocalizeStrings.Get(10799502)); // View Options ...
          choice.Add("submenuoptions");

        }
        #endregion

        #region Add menu add and save options in Views and Movie Context
        if (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.TmdbMovies && conf.ViewContext != ViewContext.MovieCollection)
        {
          dlg.Add(GUILocalizeStrings.Get(1079823)); // Add to Menu as Custom View
          choice.Add("menuadd");

          if (null != GetCustomViewFromViewLabel(conf.CurrentView))
          {
            dlg.Add(GUILocalizeStrings.Get(1079829)); // Save current custom view settings
            choice.Add("menusavecurrentsettingstoview");
          }
        }
        #endregion

        #region add start settings save option (film list and group, saves current layout, sortfield and sort direction to start settings)
        if (facadeFilms.SelectedListItemIndex > -1 && (conf.ViewContext == ViewContext.Movie || conf.ViewContext == ViewContext.MovieCollection))
        {
          if (conf.ViewContext == ViewContext.Movie)
          {
            dlg.Add(GUILocalizeStrings.Get(1079816)); // Save current settings as default layout and sort for film lists
            choice.Add("savecurrentsettingsasstartsettings");
          }

          if (conf.ViewContext == ViewContext.MovieCollection)
          {
            dlg.Add(GUILocalizeStrings.Get(1079817)); // Save current settings as default layout and sort for box-sets
            choice.Add("savecurrentsettingsasstartsettings");
          }
        }

        #endregion

        #region View context
        if (conf.ViewContext == ViewContext.Group)
        {
          dlg.Add(GUILocalizeStrings.Get(10799502)); // View Options ...
          choice.Add("submenuoptions");
        }
        #endregion

        #region TMDB online content context
        if (conf.ViewContext == ViewContext.TmdbMovies)
        {
          dlg.Add(GUILocalizeStrings.Get(10799502)); // View Options ...
          choice.Add("submenuoptions");
        }
        #endregion

        #region Add View Action Options ...
        if ((facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder && conf.Boolselect) || conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll)
        {
          if (conf.ViewContext == ViewContext.Menu)
          {
            dlg.Add(GUILocalizeStrings.Get(1079827)); // Disable Menu Entry
            choice.Add("menudisable");

            if (conf.CustomViews.View.Any(viewRow => !viewRow.ViewEnabled))
            {
              dlg.Add(GUILocalizeStrings.Get(1079828)); // Enable Menu Entry
              choice.Add("menuenable");
            }

            dlg.Add(GUILocalizeStrings.Get(1079820)); // Delete Menu Entry
            choice.Add("menudelete");
            dlg.Add(GUILocalizeStrings.Get(1079821)); // Move Menu Entry Up
            choice.Add("menumoveup");
            dlg.Add(GUILocalizeStrings.Get(1079822)); // Move Menu Entry Down
            choice.Add("menumovedown");
            dlg.Add(GUILocalizeStrings.Get(1079824)); // Rename Menu Entry
            choice.Add("menurename");
            if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Context Menu: Edit Value and Filter via GUI")) // check if specialmode is configured for disabled features
            {
              dlg.Add(GUILocalizeStrings.Get(1079825)); // Set Film Filter Value
              choice.Add("menusetvalue");
              dlg.Add(GUILocalizeStrings.Get(1079826)); // Set View Filter Expression
              choice.Add("menusetfilter");
            }
          }
        }
        #endregion

        #region Menu options
        if (facadeFilms.SelectedListItemIndex > -1 && (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll))
        {
          dlg.Add(GUILocalizeStrings.Get(10799502)); // View Options ...
          choice.Add("submenuoptions");
        }
        #endregion

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
        contextmenuselection = choice[dlg.SelectedLabel];
      }

      if (string.IsNullOrEmpty(contextmenuselection)) return;

      if (conf.ViewContext == ViewContext.Movie || conf.ViewContext == ViewContext.MovieCollection)
      {
        conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
        conf.StrTIndex = facadeFilms.SelectedListItem.Label;
      }

      switch (contextmenuselection)
      {
        #region Sub Menus
        case "submenuwebdetails":
          {
            #region SubMenu Web Details
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10799501)); // "Show web details ..."
            choice.Clear();

            #region Moviecontext
            if (facadeFilms.SelectedListItemIndex > -1 && !facadeFilms.SelectedListItem.IsFolder && conf.ViewContext != ViewContext.TmdbMovies)
            {
              dlg.Add(GUILocalizeStrings.Get(1079889));
              choice.Add("movieimdbinternet");

              dlg.Add(GUILocalizeStrings.Get(1079888));
              choice.Add("movieimdbbilder");

              dlg.Add(GUILocalizeStrings.Get(1079930));
              choice.Add("movieimdbfullcredits");

              dlg.Add(GUILocalizeStrings.Get(1079887));
              choice.Add("movieimdbtrailer");

              //if (MyFilmsDetail.ExtendedStartmode("Context: web infos via grabber script search"))
              //{
              //  dlg.Add("Movie Details (Web) ...");
              //  updChoice.Add("moviedetailswebgrabber");
              //}
            }
            #endregion

            #region Personcontext
            if (facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder && IsPersonField(conf.WStrSort) && conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll)
            {
              dlg.Add(GUILocalizeStrings.Get(1079886)); // http://www.imdb.com/name/nm0000288/
              choice.Add("artistimdbinternet");

              dlg.Add(GUILocalizeStrings.Get(1079885)); // http://www.imdb.com/name/nm0000288/filmoyear
              choice.Add("artistimdbfilmlist");

              dlg.Add(GUILocalizeStrings.Get(1079891)); // http://www.imdb.com/name/nm0000288/mediaindex
              choice.Add("artistimdbbilder");

              dlg.Add(GUILocalizeStrings.Get(1079890)); // http://www.imdb.com/name/nm0000288/videogallery
              choice.Add("artistimdbclips");
            }
            #endregion

            dlg.DoModal(GetID);
            Context_Menu_Movie(selecteditem, dlg.SelectedLabel == -1 ? string.Empty : choice[dlg.SelectedLabel].ToLower());
            return;
            //Change_Menu_Action(updChoice[dlg.SelectedLabel].ToLower());
            #endregion
          }

        case "submenuupdates":
          {
            #region SubMenu Updates
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798702)); // "Updates ..."
            choice.Clear();

            #region Moviecontext
            if (facadeFilms.SelectedListItemIndex > -1 && !facadeFilms.SelectedListItem.IsFolder && conf.ViewContext != ViewContext.TmdbMovies)
            {
              if (!facadeFilms.SelectedListItem.IsFolder && conf.ViewContext == ViewContext.MovieCollection) // inside a collection a selected collection movie
              {
                dlg.Add(GUILocalizeStrings.Get(10798760)); // Load collection cover (Tmdb)
                choice.Add("collectionimages");
              }

              dlg.Add(GUILocalizeStrings.Get(10798990)); // Load single trailer (TMDB)
              choice.Add("downloadtrailertmdb");

              dlg.Add(GUILocalizeStrings.Get(10798991)); // Load all trailers for these movies (TMDB) (in a view)
              choice.Add("downloadtrailertmdball");

              if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist))
              {
                dlg.Add(GUILocalizeStrings.Get(1079883)); // update personinfos for all involved persons of a selected movie from IMDB and/or TMDB
                choice.Add("updatepersonmovie");
              }

              if (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended)
              {
                dlg.Add(GUILocalizeStrings.Get(5910));  //Update Internet Movie Details
                choice.Add("grabber");
              }
            }
            else if (facadeFilms.SelectedListItem.IsFolder && (conf.ViewContext == ViewContext.Movie || conf.ViewContext == ViewContext.MovieCollection)) // in a film list, a "collection folder"
            {
              // TMDB Collection images download
              dlg.Add(GUILocalizeStrings.Get(10798760)); // Load collection cover (Tmdb)
              choice.Add("collectionimages");
            }
            #endregion

            #region Personcontext
            if (facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder && IsPersonField(conf.WStrSort) && conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll)
            {
              dlg.Add(GUILocalizeStrings.Get(1079882)); // update personinfo from IMDB/TMDB
              choice.Add("updateperson");

              if (facadeFilms.Count > 1 && !(conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn))
              {
                dlg.Add(GUILocalizeStrings.Get(1079883)); // update personinfos for all involved persons of a selected movie from IMDB and/or TMDB
                choice.Add("updatepersonfacadelist");
              }

              if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Context Artist: grabber scripts based person info update"))
              {
                if (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended)
                {
                  dlg.Add(GUILocalizeStrings.Get(1079899)); //Update Internet Person Details
                  choice.Add("grabber-person");
                }
              }
            }
            #endregion

            dlg.DoModal(GetID);
            Context_Menu_Movie(selecteditem, dlg.SelectedLabel == -1 ? string.Empty : choice[dlg.SelectedLabel].ToLower());
            return;
            #endregion
          }

        case "submenufanartmanager":
          {
            #region SubMenu Fanart Manager ...
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798767)); // Fanart Manager ...
            choice.Clear();

            dlg.Add(GUILocalizeStrings.Get(1079862));
            choice.Add("fanart");
            dlg.Add(GUILocalizeStrings.Get(1079874));
            choice.Add("deletefanart");

            dlg.DoModal(GetID);
            Context_Menu_Movie(selecteditem, dlg.SelectedLabel == -1 ? string.Empty : choice[dlg.SelectedLabel].ToLower());
            return;
            #endregion
          }

        case "submenuoptions":
          {
            #region SubMenu Options ...
            if (dlg == null) return;
            Context_Menu = true;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10799502)); // View Options ...
            choice.Clear();

            if (facadeFilms.SelectedListItemIndex > -1 && conf.ViewContext != ViewContext.TmdbMovies && conf.ViewContext != ViewContext.Movie && conf.ViewContext != ViewContext.MovieCollection)
            {
              if (conf.BoolShowEmptyValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798628))); // show empty values in views
              if (!conf.BoolShowEmptyValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798629)));
              choice.Add("showemptyvaluesinviewscontext");
            }

            #region Views context - option settings
            if ((facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder && conf.Boolselect) && conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll)
            {
              if (conf.IndexedChars > 0) dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + conf.IndexedChars)); // Show only indexed items in view
              if (conf.IndexedChars == 0) dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798629)));
              choice.Add("showindexedvalues");

              if (conf.BoolDontSplitValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079845), GUILocalizeStrings.Get(10798628))); // Don't split values
              if (!conf.BoolDontSplitValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079845), GUILocalizeStrings.Get(10798629)));
              choice.Add("dontsplitvaluesinviews");
            }
            #endregion

            #region Moviecontext
            if (facadeFilms.SelectedListItemIndex > -1 && !facadeFilms.SelectedListItem.IsFolder && conf.ViewContext != ViewContext.TmdbMovies)
            {
              if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "Context: TMDB API trailer and download entries")) // if (conf.ViewContext != ViewContext.TmdbMovies)
              {
                if (conf.BoolEnableOnlineServices) dlg.Add(string.Format(GUILocalizeStrings.Get(1079921), GUILocalizeStrings.Get(10798628))); // Get Online Info
                if (!conf.BoolEnableOnlineServices) dlg.Add(string.Format(GUILocalizeStrings.Get(1079921), GUILocalizeStrings.Get(10798629)));
                choice.Add("showonlineinfo");
              }
            }
            #endregion

            #region Personcontext
            if (facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder && IsPersonField(conf.WStrSort) && (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll))
            {
              if (conf.BoolReverseNames) dlg.Add(string.Format(GUILocalizeStrings.Get(1079839), GUILocalizeStrings.Get(10798628))); // Reverse names
              if (!conf.BoolReverseNames) dlg.Add(string.Format(GUILocalizeStrings.Get(1079839), GUILocalizeStrings.Get(10798629)));
              choice.Add("reversenames");
            }
            #endregion

            #region Moviecontext or Views
            if (facadeFilms.SelectedListItemIndex > -1 && conf.ViewContext != ViewContext.TmdbMovies)
            {
              // Use Virtual Path Browsing
              if (conf.BoolVirtualPathBrowsing) dlg.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798628))); // virtual path browsing
              if (!conf.BoolVirtualPathBrowsing) dlg.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798629)));
              choice.Add("virtualpathbrowsing");
            }
            #endregion

            #region TMDB online content context
            if (conf.ViewContext == ViewContext.TmdbMovies)
            {
              if (conf.BoolAskForPlaybackQuality) dlg.Add(string.Format(GUILocalizeStrings.Get(1079919), GUILocalizeStrings.Get(10798628))); // Always ask for playback quality for online content ({0})
              if (!conf.BoolAskForPlaybackQuality) dlg.Add(string.Format(GUILocalizeStrings.Get(1079919), GUILocalizeStrings.Get(10798629)));
              choice.Add("askforplaybackquality");
            }
            #endregion

            dlg.DoModal(GetID);
            Context_Menu_Movie(selecteditem, dlg.SelectedLabel == -1 ? string.Empty : choice[dlg.SelectedLabel].ToLower());
            return;
            #endregion
          }
        #endregion

        #region Menu Actions
        case "playmovie":
          MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.Internal);
          break;

        case "playmovieexternal":
          MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.External);
          break;

        case "playmoviebrplayerlauncher":
          MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.BluRayPlayerLauncher);
          break;

        case "playtrailer":
          #region first check, if trailer files are available, offer options, if not ...
          // if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer) // StrDirStorTrailer only required for extended search
          if (!string.IsNullOrEmpty(r[conf.StrIndex][conf.StrStorageTrailer].ToString().Trim()))
          {
            MyFilmsDetail.trailerPlayed = true;
            MyFilmsDetail.Launch_Movie_Trailer(conf.StrIndex, GetID, m_SearchAnimation);
          }
          else
          {
            // ToDo: Can add autosearch&register logic here before try starting trailers
            // if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(10798704), r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle].ToString() + "\n" + GUILocalizeStrings.Get(10798737) + "\n" + GUILocalizeStrings.Get(10798739))) //trailer //video title //no video found locally // Search local trailers  and update DB ?
            var dlgYesNotrailersearch = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNotrailersearch.SetHeading(GUILocalizeStrings.Get(10798704));//trailer
            dlgYesNotrailersearch.SetLine(1, r[conf.StrIndex][conf.StrSTitle].ToString());//video title
            dlgYesNotrailersearch.SetLine(2, GUILocalizeStrings.Get(10798737));//no video found locally
            dlgYesNotrailersearch.SetLine(3, GUILocalizeStrings.Get(10798739)); // Search local trailers  and update DB ?
            dlgYesNotrailersearch.DoModal(GetID); //dlgYesNotrailersearch.DoModal(GUIWindowManager.ActiveWindow);
            if (dlgYesNotrailersearch.IsConfirmed)
            {
              //setProcessAnimationStatus(true, m_SearchAnimation);
              //LogMyFilms.Debug("SearchTrailerLocal() SelectedItemInfo from (MyFilms.r[conf.StrIndex][conf.StrTitle1].ToString(): '" + (r[conf.StrIndex][conf.StrTitle1].ToString() + "'"));
              LogMyFilms.Debug("(Auto search trailer after selecting PLAY) title: '" + (r[conf.StrIndex] + "'"));
              MyFilmsDetail.SearchTrailerLocal(r, conf.StrIndex, true, true);
              //afficher_detail(true);
              //setProcessAnimationStatus(false, m_SearchAnimation);
              MyFilmsDetail.trailerPlayed = true;
              MyFilmsDetail.Launch_Movie_Trailer(conf.StrIndex, GetID, m_SearchAnimation);
            }
          }
          #endregion
          break;

        case "playrandomtrailers":  // only in views and intended for "multiple movies" // dlg.Add(GUILocalizeStrings.Get(10798980)); // play random trailers
          PlayRandomTrailersInit(facadeFilms.SelectedListItem.Label, "");
          break;

        case "selectrandommovie":  // only in views // dlg.Add(GUILocalizeStrings.Get(10798979)); // select random movie
          SelectRandomMovie();
          break;

        case "downloadtrailertmdb":
          if (!Helper.IsOnlineVideosAvailableAndEnabled)
          {
            GUIUtils.ShowNotifyDialog("OnlineVideos is not available!");
            return;
          }
          MyFilmsDetail.SearchAndDownloadTrailerOnlineTmdb(r, conf.StrIndex, false, true, (conf.StrDirStorTrailer.Length > 0) ? conf.StrDirStorTrailer : null); // MyFilmsDetail.SearchAndDownloadTrailerOnlineTMDB(r, conf.StrIndex, false, true, null);
          break;

        case "downloadtrailertmdball":
          {
            if (!Helper.IsOnlineVideosAvailableAndEnabled)
            {
              GUIUtils.ShowNotifyDialog("OnlineVideos is not available!");
              return;
            }
            #region add trailers for selected films to trailer download queue
            StopAddingTrailers = false;
            new Thread(delegate()
            {
              {
                try
                {
                  if (!Helper.IsConnectedToInternet()) return;

                  for (int i = 0; i < r.Length; i++)
                  {
                    if (StopAddingTrailers) return;
                    try
                    {
                      MyFilmsDetail.SearchAndDownloadTrailerOnlineTmdb(r, i, true, false, (conf.StrDirStorTrailer.Length > 0) ? conf.StrDirStorTrailer : null);
                      MyFilmsDetail.setGUIProperty("statusmessage", "adding TMDB trailers to download queue for '" + r[i][conf.StrTitle1] + "'", false);
                    }
                    catch (Exception ie)
                    {
                      LogMyFilms.Warn("Error adding trailers to download queue: " + ie.Message);
                      LogMyFilms.Warn("Stacktrace: " + ie.StackTrace);
                      MyFilmsDetail.setGUIProperty("statusmessage", "", false);
                    }
                  }
                  MyFilmsDetail.setGUIProperty("statusmessage", "", false);
                }
                catch (Exception ex)
                {
                  LogMyFilms.Error("Error adding trailers to download queue: " + ex.StackTrace);
                  MyFilmsDetail.setGUIProperty("statusmessage", "global update active", false);
                }
              }
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
              {
                {
                  MyFilmsDetail.setGUIProperty("statusmessage", "", false);
                }
                return 0;
              }, 0, 0, null);
            }) { Name = "MyFilmsAddTrailerDownloadsToQueue", IsBackground = true }.Start();
            #endregion
          }
          break;

        case "showemptyvaluesinviewscontext":
          {
            conf.BoolShowEmptyValuesInViews = !conf.BoolShowEmptyValuesInViews;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'show empty values' changed to " + conf.BoolShowEmptyValuesInViews);
            Refreshfacade();
            break;
          }

        case "showindexedvalues":
          {
            #region show indexed values
            if (dlg == null) return;
            dlg.Reset();
            choice.Clear();
            MFview.ViewRow currentCustomView = null;
            int currentIndex = conf.IndexedChars;

            if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) currentCustomView = GetCustomViewFromViewLabel(facadeFilms.SelectedListItem.Label);
            else currentCustomView = GetCustomViewFromViewLabel(conf.CurrentView); // Views - check, which one is active
            if (currentCustomView != null) currentIndex = currentCustomView.Index;

            string headline = (currentIndex == 0) ? (string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798629))) : (string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + currentIndex));
            dlg.SetHeading(headline);

            dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798629))); // disabled indexed view
            choice.Add("0");

            dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + "1"));
            choice.Add("1");

            dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + "2"));
            choice.Add("2");

            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
            {
              break;
            }
            switch (choice[dlg.SelectedLabel])
            {
              case "0":
                if (currentIndex == 0) return;
                currentIndex = 0;
                break;
              case "1":
                if (currentIndex == 1) return;
                currentIndex = 1;
                break;
              case "2":
                if (currentIndex == 2) return;
                currentIndex = 2;
                break;
              default:
                return;
            }

            //// if it is a view, save the settings for it
            //if (currentCustomView != null)
            //{
            //  currentCustomView.Index = currentIndex;
            //  LogMyFilms.Debug("Context_Menu_Movie() : Option 'show indexed values' changed for Custom View '" + currentCustomView.Label + "' to '" + currentIndex + "'");
            //  SaveCustomViews();
            //}

            // now change settings for GUI
            conf.IndexedChars = currentIndex;
            if (conf.IndexedChars > 0)
            {
              conf.Boolindexed = true;
              conf.Boolindexedreturn = false;
            }
            else
            {
              conf.Boolindexed = false;
              conf.Boolindexedreturn = false;
              conf.Wstar = "*";
            }
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'show indexed values' changed to '" + conf.IndexedChars + "'");
            Refreshfacade();
            #endregion
            break;
          }

        case "dontsplitvaluesinviews":
          {
            conf.BoolDontSplitValuesInViews = !conf.BoolDontSplitValuesInViews;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Don't split values' changed to " + conf.BoolDontSplitValuesInViews);
            Refreshfacade();
            break;
          }

        case "virtualpathbrowsing":
          {
            conf.BoolVirtualPathBrowsing = !conf.BoolVirtualPathBrowsing;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Use virtual Path Browsing' changed to " + conf.BoolVirtualPathBrowsing);
            if (!conf.BoolVirtualPathBrowsing) conf.StrTitleSelect = "";
            Refreshfacade();
            break;
          }

        case "reversenames":
          {
            conf.BoolReverseNames = !conf.BoolReverseNames;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Reverse Names' changed to '" + conf.BoolReverseNames + "'");
            Refreshfacade();
            break;
          }

        case "showonlineinfo":
          {
            conf.BoolEnableOnlineServices = !conf.BoolEnableOnlineServices;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Show Online Info' changed to '" + conf.BoolEnableOnlineServices + "'");
            Refreshfacade();
            break;
          }

        case "askforplaybackquality":
          {
            conf.BoolAskForPlaybackQuality = !conf.BoolAskForPlaybackQuality;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Always ask for playback quality for online content...' changed to '" + conf.BoolAskForPlaybackQuality + "'");
            break;
          }

        case "savecurrentsettingsasstartsettings":
          {
            #region save current settingsas start settings for layout and sort (film list or groups)
            LogMyFilms.Debug("Context_Menu_Movie() - Update default start settings from current view");
            using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
            {
              if (conf.ViewContext == ViewContext.Movie)
              {
                xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntDfltLayOut", conf.StrLayOut);
                xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntDfltStrSort", conf.StrSorta);
                xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntDfltStrSortSens", conf.StrSortSens);
                LogMyFilms.Debug("Context_Menu_Movie() - saved default start settings for film lists: Layout = '" + conf.StrLayOut + "', Sort Field = '" + conf.StrSorta + "', Sort Direction = '" + conf.StrSortSens + "'");
              }

              if (conf.ViewContext == ViewContext.MovieCollection)
              {
                xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntDfltLayOutInHierarchies", conf.StrLayOutInHierarchies);
                xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntDfltStrSortInHierarchies", conf.StrSortaInHierarchies);
                xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntDfltStrSortSensInHierarchies", conf.StrSortSensInHierarchies);
                LogMyFilms.Debug("Context_Menu_Movie() - saved default start settings for box sets: Layout = '" + conf.StrLayOutInHierarchies + "', Sort Field = '" + conf.StrSortaInHierarchies + "', Sort Direction = '" + conf.StrSortSensInHierarchies + "'");
              }
            }
            // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
            
            // _navigationStack.Clear();
            break;
            #endregion
          }

        #region Views Menu Actions

        case "menusavecurrentsettingstoview":
          {
            #region Menu - save settings
            MFview.ViewRow newRow = GetCustomViewFromViewLabel(conf.CurrentView);
            newRow.SortDirectionView = (conf.BoolSortCountinViews) ? conf.WStrSortSensCount : conf.WStrSortSens;
            newRow.SortFieldViewType = (conf.BoolSortCountinViews) ? "Count" : "Name";
            newRow.Index = conf.IndexedChars;
            newRow.LayoutView = conf.WStrLayOut.ToString();
            LogMyFilms.Debug("Context_Menu_Movie() - Update Custom View - DB Field '" + newRow.DBfield + "', Label '" + newRow.Label + "', Value '" + newRow.Value + "'");
            SaveCustomViews();
            _navigationStack.Clear();
            break;
            #endregion
          }

        case "menuadd":
          {
            #region Menu - add item
            MFview.ViewRow newRow = conf.CustomViews.View.NewViewRow();
            newRow.ViewEnabled = true;
            //newRow.ShowEmpty = conf.BoolShowEmptyValuesInViews;
            switch (conf.ViewContext)
            {
              case ViewContext.MenuAll:
                newRow.Label = facadeFilms.SelectedListItem.Label + " *";
                newRow.ImagePath = facadeFilms.SelectedListItem.ThumbnailImage;
                if (!File.Exists(newRow.ImagePath)) newRow.ImagePath = conf.DefaultCoverViews;
                newRow.DBfield = facadeFilms.SelectedListItem.DVDLabel;
                newRow.SortDirectionView = " ASC";
                newRow.SortFieldViewType = "Name";
                newRow.Index = 0;
                newRow.LayoutView = "0"; // List view
                newRow.Value = "";
                newRow.Filter = "";
                break;
              case ViewContext.Movie:
              case ViewContext.MovieCollection:
              case ViewContext.Group:
              case ViewContext.Person:
                newRow.Label = BaseMesFilms.TranslateColumn(conf.WStrSort) + " - " + conf.StrTxtSelect + " *";
                newRow.DBfield = conf.WStrSort;
                newRow.SortDirectionView = (conf.BoolSortCountinViews) ? conf.WStrSortSensCount : conf.WStrSortSens;
                newRow.SortFieldViewType = (conf.BoolSortCountinViews) ? "Count" : "Name";
                newRow.Index = conf.IndexedChars;
                newRow.LayoutView = conf.WStrLayOut.ToString();
                switch (conf.ViewContext)
                {
                  case ViewContext.Movie:
                    // newRow.Value = Prev_Label;
                    // newRow.Value = conf.Wselectedlabel;
                    newRow.Value = "*";
                    newRow.Filter = conf.StrSelect;
                    if (IsPersonField(newRow.DBfield))
                      newRow.ImagePath = !string.IsNullOrEmpty(personcover.Filename) ? personcover.Filename : conf.DefaultCoverArtist;
                    else
                      newRow.ImagePath = !string.IsNullOrEmpty(viewcover.Filename) ? viewcover.Filename : conf.DefaultCoverViews;
                    break;
                  case ViewContext.MovieCollection:
                    //newRow.Value = Prev_Label;
                    newRow.Value = conf.Wselectedlabel;
                    newRow.Filter = conf.StrSelect;
                    newRow.ImagePath = !string.IsNullOrEmpty(groupcover.Filename) ? groupcover.Filename : !string.IsNullOrEmpty(filmcover.Filename) ? filmcover.Filename : conf.DefaultCover;
                    break;
                  case ViewContext.Group:
                    newRow.Value = "";
                    newRow.Filter = conf.StrViewSelect;
                    newRow.ImagePath = conf.DefaultCoverViews;
                    break;
                  case ViewContext.Person:
                    newRow.Value = "";
                    newRow.Filter = conf.StrViewSelect;
                    newRow.ImagePath = conf.DefaultCoverArtist;
                    break;
                }
                break;
              default:
                return;
            }

            if (newRow.DBfield.Length == 0 || newRow.Label.Length == 0) return;
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
            keyboard.Text = newRow.Label;
            keyboard.DoModal(GetID);
            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            {
              newRow.Label = keyboard.Text;
              conf.CustomViews.View.AddViewRow(newRow);
              LogMyFilms.Debug("Context_Menu_Movie() - Add View with DB Field '" + newRow.DBfield + "', Label '" + newRow.Label + "', Value '" + newRow.Value + "'");
              SaveCustomViews();
              GetSelectFromMenuView(conf.BoolMenuShowAll);
              _navigationStack.Clear();
            }
            break;
            #endregion
          }

        case "menudisable":
          {
            #region Menu - disable item
            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              viewRow.ViewEnabled = false;
              break;
            }
            SaveCustomViews();
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            _navigationStack.Clear();
            break;
            #endregion
          }

        case "menuenable":
          {
            #region Menu - enable item
            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

            if (dlgmenu == null) return;
            dlgmenu.Reset();
            dlgmenu.SetHeading(GUILocalizeStrings.Get(1079828)); // Enable Menu Entry

            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => viewRow.ViewEnabled == false))
            {
              dlgmenu.Add(viewRow.Label);
            }

            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1) break;

            foreach (MFview.ViewRow viewRow in conf.CustomViews.View)
            {
              if (dlgmenu.SelectedLabelText == viewRow.Label && viewRow.ViewEnabled == false)
              {
                viewRow.ViewEnabled = true;
                break;
              }
            }
            SaveCustomViews();
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            _navigationStack.Clear();
            break;
            #endregion
          }

        case "menudelete":
          {
            #region Menu - delete item
            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              conf.CustomViews.View.RemoveViewRow(viewRow);
              break;
            }
            SaveCustomViews();
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            _navigationStack.Clear();
            break;
            #endregion
          }

        case "menumoveup":
          {
            #region Menu - movie item up
            int rowIndex = -1;
            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              rowIndex = conf.CustomViews.View.Rows.IndexOf(viewRow);
              break;
            }
            if (rowIndex > 0)
            {
              DataRow selectedRow = conf.CustomViews.View.Rows[rowIndex];
              DataRow newRow = conf.CustomViews.View.NewRow();
              newRow.ItemArray = selectedRow.ItemArray; // copy data
              conf.CustomViews.View.Rows.Remove(selectedRow);
              conf.CustomViews.View.Rows.InsertAt(newRow, rowIndex - 1);
            }
            SaveCustomViews();
            conf.MenuSelectedId--;
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            _navigationStack.Clear();
            break;
            #endregion
          }

        case "menumovedown":
          {
            #region Menu - movie item down
            int rowIndex = -1;
            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              rowIndex = conf.CustomViews.View.Rows.IndexOf(viewRow);
              break;
            }
            if (rowIndex < conf.CustomViews.View.Rows.Count - 1)
            {
              DataRow selectedRow = conf.CustomViews.View.Rows[rowIndex];
              DataRow newRow = conf.CustomViews.View.NewRow();
              newRow.ItemArray = selectedRow.ItemArray; // copy data
              conf.CustomViews.View.Rows.Remove(selectedRow);
              conf.CustomViews.View.Rows.InsertAt(newRow, rowIndex + 1);
            }
            SaveCustomViews();
            conf.MenuSelectedId++;
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            _navigationStack.Clear();
            break;
            #endregion
          }

        case "menurename":
          {
            #region Menu - rename tem
            foreach (MFview.ViewRow viewRow in conf.CustomViews.View)
            {
              if (facadeFilms.SelectedListItem.Label == viewRow.Label)
              {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard) return;
                keyboard.Reset();
                keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
                keyboard.Text = viewRow.Label;
                keyboard.DoModal(GetID);
                if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                {
                  viewRow.Label = keyboard.Text;
                  SaveCustomViews();
                  GetSelectFromMenuView(conf.BoolMenuShowAll);
                  _navigationStack.Clear();
                }
                break;
              }
            }
            break;
            #endregion
          }

        case "menusetvalue":
          {
            #region Menu - set value
            foreach (MFview.ViewRow viewRow in conf.CustomViews.View)
            {
              if (facadeFilms.SelectedListItem.Label == viewRow.Label)
              {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard) return;
                keyboard.Reset();
                keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
                keyboard.Text = viewRow.Value;
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed && keyboard.Text != null)
                {
                  viewRow.Value = keyboard.Text;
                  SaveCustomViews();
                  GetSelectFromMenuView(conf.BoolMenuShowAll);
                  _navigationStack.Clear();
                }
                break;
              }
            }
            break;
            #endregion
          }

        case "menusetfilter":
          {
            #region Menu - set filter
            foreach (MFview.ViewRow viewRow in conf.CustomViews.View)
            {
              if (facadeFilms.SelectedListItem.Label == viewRow.Label)
              {
                // ToDo:
                // ändern
                // löschen
                // auswählen via "field" und "wert - wert wahlweise manuell

                // offer menu to user about actions
                GUIListItem item;
                List<string> filteritems = new List<string>(); 

                GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlgmenu == null) return;
                dlgmenu.Reset();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(1079826)); // set view filter expression
                item = new GUIListItem();
                item.Label = "remove all filter";
                dlgmenu.Add(item);
                filteritems.Add(item.Label);
                item = new GUIListItem();
                item.Label = "manual edit filter";
                dlgmenu.Add(item);
                filteritems.Add(item.Label);

                #region add all field options ...
                ArrayList displayItems = GetDisplayItems("view");
                foreach (string[] displayItem in displayItems)
                {
                  item = new GUIListItem
                  {
                    Label = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1],
                    DVDLabel = displayItem[0]
                  };
                  dlgmenu.Add(item);
                  filteritems.Add(displayItem[0]);
                }
                #endregion
                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1) break;

                switch (dlgmenu.SelectedLabel)
                {
                  case 0:
                    viewRow.Filter = "";
                    SaveCustomViews();
                    GetSelectFromMenuView(conf.BoolMenuShowAll);
                    _navigationStack.Clear();
                    break;
                  case 1:
                    VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                    if (null == keyboard) return;
                    keyboard.Reset();
                    keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
                    keyboard.Text = viewRow.Filter;
                    keyboard.DoModal(GetID);
                    if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                    {
                      viewRow.Filter = keyboard.Text;
                      SaveCustomViews();
                      GetSelectFromMenuView(conf.BoolMenuShowAll);
                      _navigationStack.Clear();
                    }
                    break;

                  default:
                    string antFilteritem = filteritems[dlgmenu.SelectedLabel];
                    List<GUIListItem> filteroptions = new List<GUIListItem>(); 
                    
                    dlgmenu.Reset();
                    dlgmenu.SetHeading("add filter - choose operator");

                    #region add filter options
                    item = new GUIListItem
                    {
                      Label = "Contains ...",
                      DVDLabel = "like"
                    };
                    dlgmenu.Add(item);
                    filteroptions.Add(item);
                    item = new GUIListItem
                    {
                      Label = "Contains not ...",
                      DVDLabel = "not like"
                    };
                    dlgmenu.Add(item);
                    filteroptions.Add(item);
                    item = new GUIListItem
                    {
                      Label = "Equals ...",
                      DVDLabel = "="
                    };
                    dlgmenu.Add(item);
                    filteroptions.Add(item);
                    item = new GUIListItem
                    {
                      Label = "Equals not ...",
                      DVDLabel = "<>"
                    };
                    dlgmenu.Add(item);
                    filteroptions.Add(item);
                    #endregion

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1) break;

                    string filterSign = filteroptions[dlgmenu.SelectedLabel].DVDLabel;

                    List<string> wTableau = new List<string>();
                    // var w_tableau = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    // var w_tableau = new List<string>();

                    #region get possible values
                    string champselect = "";
                    FieldType fieldType = GetFieldType(antFilteritem);
                    bool isperson = IsPersonField(fieldType);
                    bool isdate = IsDateField(fieldType);
                    bool issplitfield = IsSplittableField(antFilteritem);
                    bool dontsplitvalues = conf.BoolDontSplitValuesInViews;
                    bool showEmptyValues = conf.BoolShowEmptyValuesInViews;
                    bool reversenames = isperson && conf.BoolReverseNames;

                    foreach (DataRow row in r) // foreach (DataRow row in BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, wstrSelect, wStrSort, wStrSortSens))
                    {
                      if (isdate)
                      {
                        DateTime tmpdate;
                        champselect = (DateTime.TryParse(row[antFilteritem].ToString(), out tmpdate)) ? string.Format("{0:yyyy/MM/dd}", tmpdate.ToShortDateString()) : "";
                      }
                      else
                        champselect = row[antFilteritem].ToString().Trim();

                      if (issplitfield && !dontsplitvalues)
                      {
                        List<string> wtab = Search_String(champselect, reversenames); //ArrayList wtab = Search_String(champselect, isperson);
                        if (wtab.Count > 0)
                        {
                          wTableau.AddRange(wtab);
                        }
                        else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
                        {
                          wTableau.Add(champselect);
                        }
                      }
                      else
                      {
                        if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
                        {
                          wTableau.Add(champselect);
                        }
                      }
                    }

                    wTableau = wTableau.Distinct().ToList();   //wTableau = Helper.RemoveDuplicate(wTableau);

                    switch (fieldType)
                    {
                      case FieldType.AlphaNumeric:
                      case FieldType.Decimal:
                        wTableau.Sort(0, wTableau.Count, new AlphanumComparatorFastString());
                        break;

                      default: // default sorter
                        wTableau.Sort(0, wTableau.Count, StringComparer.OrdinalIgnoreCase);
                        break;
                    }
                    #endregion

                    dlgmenu.Reset();
                    dlgmenu.SetHeading("add filter - choose operator");
                    foreach (string value in wTableau)
                    {
                      item = new GUIListItem {Label = value};
                      dlgmenu.Add(item);
                    }
                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1) break;

                    string sValue = dlgmenu.SelectedLabelText;

                    // create the filter rule
                    string strViewFilterSelect = string.Empty;

                    switch (filterSign)
                    {
                      case "not like":
                      case "#":
                        strViewFilterSelect = "(" + antFilteritem + " " + filterSign + " #" + Convert.ToDateTime(sValue) + "# or " + antFilteritem + " is null) ";
                        break;
                      //case "not in":
                      //case "in":
                      //  strViewFilterSelect = "(" + AntFilteritem + " " + FilterSign + " (" + DBitemList(sValuet, true) + ")) ";
                      //  break;
                      //case "like in":
                      //  strViewFilterSelect = "(" + TransformedLikeIn(FilterSign, sValue, true) + ") ";
                      //  break;
                      default:
                        strViewFilterSelect = "(" + antFilteritem + " " + filterSign + " #" + Convert.ToDateTime(sValue) + "# ) ";
                        break;
                    }
                    LogMyFilms.Debug("New filter expression = '" + strViewFilterSelect + "'");

                    if (viewRow.Filter.Length > 0)
                    {
                      viewRow.Filter += " AND " + strViewFilterSelect;
                    }
                    else
                    {
                      viewRow.Filter = strViewFilterSelect;
                    }
                    LogMyFilms.Debug("Resulting view filter expression = '" + viewRow.Filter + "'");
                    SaveCustomViews();
                    GetSelectFromMenuView(conf.BoolMenuShowAll);
                    _navigationStack.Clear();
                    break;
                }

                break;
              }
            }
            break;
            #endregion
          }
        #endregion

        case "analogyperson":
          {
            SearchRelatedMoviesbyPersons(facadeFilms.SelectedListItem.ItemId, Context_Menu);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
          }

        case "analogyproperty":
          {
            SearchRelatedMoviesbyProperties(facadeFilms.SelectedListItem.ItemId, Context_Menu);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
          }

        case "searchonlinevideos":
          {
            OnlineVideosSearchLauncher(facadeFilms.SelectedListItem.Label);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
          }

        case "searchmyfilms":
          {
            #region Search Myfilms by search expression from selecteditemlabel
            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

            if (dlgmenu == null) return;
            dlgmenu.Reset();
            dlgmenu.SetHeading(GUILocalizeStrings.Get(10798601)); // Search MyFilms ... 
            
            dlgmenu.Add(facadeFilms.SelectedListItem.Label); // add item Label
            var searchitems = facadeFilms.SelectedListItem.Label.Split(new char[] { ' ' });
            foreach (string searchitem in searchitems)
            {
              dlgmenu.Add(searchitem);
            }
            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1) break;

            SearchMoviesbyProperties(true, dlgmenu.SelectedLabelText, null);

            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
            #endregion
          }

        case "searchnzbs":
          {
            #region NZB Menu
            string searchObj = facadeFilms.SelectedListItem.Label;
            LogMyFilms.Debug("Starting mpNZB with '" + searchObj + "'");
            GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.MpNzb, searchObj);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
            #endregion
          }

        case "movieimdbinternet":  // Example: (http://www.imdb.com/title/tt0438488)
          {
            BrowseTheWebLauncher("movie", "", facadeFilms.SelectedListItem.Label);
            break;
          }
        case "movieimdbbilder": // example: http://www.imdb.com/title/tt0133093/mediaindex
          {
            BrowseTheWebLauncher("movie", "mediaindex", facadeFilms.SelectedListItem.Label);
            break;
          }

        case "movieimdbfullcredits": // example: http://www.imdb.com/title/tt0133093/fullcredits
          {
            BrowseTheWebLauncher("movie", "fullcredits", facadeFilms.SelectedListItem.Label);
            break;
          }

        case "movieimdbtrailer": // Example: (http://www.imdb.com/title/tt0438488/videogallery)
          {
            BrowseTheWebLauncher("movie", "videogallery", facadeFilms.SelectedListItem.Label);
            break;
          }

        case "moviedetailswebgrabber": // search movie URL via grabber scripts and show webpage
          {
            // ToDo: modify grabber to support launch of detals URL via BrowseTheWeb browser
            //conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            //string title = MyFilmsDetail.GetSearchTitle(r, this.facadeFilms.SelectedListItem.ItemId, "titleoption");
            //string mediapath = MyFilms.r[this.facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrStorage].ToString();
            //if (mediapath.Contains(";")) mediapath = mediapath.Substring(0, mediapath.IndexOf(";")); // take the forst source file
            //sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
            //MyFilmsDetail.grabb_Internet_Informations(title, GetID, MyFilms.conf.StrGrabber_ChooseScript, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.All, false, sTitles);
            break;
          }

        case "artistimdbclips": // Example: http://www.imdb.com/name/nm0000206/videogallery
          {
            BrowseTheWebLauncher("person", "videogallery", facadeFilms.SelectedListItem.Label);
            break;
          }

        case "artistimdbbilder": // Example: http://www.imdb.com/name/nm0000206/mediaindex
          {
            BrowseTheWebLauncher("person", "mediaindex", facadeFilms.SelectedListItem.Label);
            break;
          }

        case "artistimdbinternet": // Example: http://www.imdb.com/name/nm0000206/
          {
            BrowseTheWebLauncher("person", "", facadeFilms.SelectedListItem.Label);
            break;
          }

        case "artistimdbfilmlist": // http://www.imdb.com/name/nm0000288/filmoyear
          {
            BrowseTheWebLauncher("person", "filmoyear", facadeFilms.SelectedListItem.Label);
            break;
          }
        case "moviepersonlist":
          {
            if (!facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
            {
              //string persontype = SelectPersonType((int)facadeFilms.SelectedListItem.ItemId);
              //if (string.IsNullOrEmpty(persontype)) return;
              MoviePersonListLauncher(true, true);
            }
            break;
          }

        case "artistdetailtmdb":
          LogMyFilms.Debug("artistdetailtmdb: launching TMDB online infos for '" + facadeFilms.SelectedListItem.Label + "'");
          {
            string actorSearchname = Helper.DatabaseUtility.RemoveInvalidChars(facadeFilms.SelectedListItem.Label);
            SaveListState(false); // save current display to navigation cache
            GetSelectFromTmdb(actorSearchname);
            break;
          }

        case "updateperson":
          {
            #region update person info from internet
            // UpdatePersons(new ArrayList { this.facadeFilms.SelectedListItem.Label }, true); // alternative approach, using ProgressDialog

            new Thread(delegate(object o)
            {
              try 
              {
                bool success = MyFilmsDetail.UpdatePersonDetails((string)o, facadeFilms.SelectedListItem, true, false);
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug(ex, "Thread 'MyFilmsPersonUpdater' - exception!");
              }
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
              {
                // after background thread has finished !
                return 0;
              }, 0, 0, null);
            }) { Name = "MyFilmsPersonUpdater", IsBackground = true }.Start(facadeFilms.SelectedListItem.Label);
            
            // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            #endregion
            break;
          }

        case "updatepersonmovie":
          {
            UpdatePersons(Search_String(r[facadeFilms.SelectedListItem.ItemId]["Persons"].ToString()), true);
            break;
          }

        case "updatepersonfacadelist":
          {
            UpdatePersonsInFacade(true);
            break;
          }

        case "collectionimages":
          {
            MyFilmsDetail.LoadCollectionImages(r, conf.StrIndex, true, m_SearchAnimation);
            break;
          }

        case "covermanager":
          {
            LogMyFilms.Debug("Switching to Cover Manager Window");
            MyFilmsCoverManager cm = null;
            if (cm == null)
            {
              cm = new MyFilmsCoverManager();
              var cmwindow = (GUIWindow)cm;
              GUIWindowManager.Add(ref cmwindow);
              cm.Init();
            }
            // cm.MovieID = (int)facadeFilms.SelectedListItem.ItemId; // will be set later in cm class
            cm.setPageTitle("Cover Manager");
            //sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex));
            //cm.ArtworkFileName = GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);
            GUIWindowManager.ActivateWindow(cm.GetID, false);
            break;
          }

        //case "fanartmanager":
        //  {
        //    LogMyFilms.Debug("Switching to Fanart Manager Window");
        //    MyFilmsFanartManager fm = null;
        //    if (fm == null)
        //    {
        //      fm = new MyFilmsFanartManager();
        //      var cmwindow = (GUIWindow)fm;
        //      GUIWindowManager.Add(ref cmwindow);
        //      fm.Init();
        //    }
        //    fm.setPageTitle("Fanart Manager"); // GUILocalizeStrings.Get(10798767) Fanart Manager ...
        //    GUIWindowManager.ActivateWindow(fm.GetID, false);
        //    break;
        //  }

        case "suppress":
          {
            if (dlg == null) return;
            dlg.Reset();
            choice.Clear();
            dlg.SetHeading(GUILocalizeStrings.Get(1079830)); // Delete movie ...

            dlg.Add(GUILocalizeStrings.Get(1079831)); // Remove movie from catalog
            choice.Add("removefromdb");

            dlg.Add(GUILocalizeStrings.Get(1079832)); // Delete movie file(s) from disk
            choice.Add("deletefromdisk");

            dlg.Add(GUILocalizeStrings.Get(1079833)); // Delete from catalog and disk
            choice.Add("deletefromdbanddisk");

            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1) Context_Menu_Movie(selecteditem, "");

            switch (choice[dlg.SelectedLabel].ToLower())
            {
              case "removefromdb":
                
                // if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(1079831), GUILocalizeStrings.Get(433))) //Remove movie from catalog //confirm suppression
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079831));//Remove movie from catalog
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  MyFilmsDetail.ManualDelete(r[facadeFilms.SelectedListItem.ItemId], true, false);
                  Loadfacade(); //Fin_Charge_Init(false, true);
                }
                break;
              case "deletefromdisk":
                // if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(1079832), GUILocalizeStrings.Get(927) + "\n" + GUILocalizeStrings.Get(1079834) + "\n" + GUILocalizeStrings.Get(1079835))) //Delete movie file(s) from disk // warning //If you confirm, you media files will physically be deleted ! //Are you sure you want to delete movie ?
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079832));//Delete movie file(s) from disk
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927));// warning
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));//If you confirm, you media files will physically be deleted !
                dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835));//Are you sure you want to delete movie ?
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)                {
                  MyFilmsDetail.ManualDelete(r[facadeFilms.SelectedListItem.ItemId], false, true);
                  Loadfacade(); //Fin_Charge_Init(false, true);
                }
                break;
              case "deletefromdbanddisk":
                // if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(1079833), GUILocalizeStrings.Get(927) + "\n" + GUILocalizeStrings.Get(1079834) + "\n" + GUILocalizeStrings.Get(1079835))) //Delete from catalog and disk // warning //If you confirm, you media files will physically be deleted ! //Are you sure you want to delete movie ?
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079833));//Delete from catalog and disk
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927));// warning
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));//If you confirm, you media files will physically be deleted !
                dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835));//Are you sure you want to delete movie ?
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)                {
                  MyFilmsDetail.ManualDelete(r[facadeFilms.SelectedListItem.ItemId], true, true);
                  Loadfacade(); //Fin_Charge_Init(false, true);
                }
                break;
            }
            break;
          }

        case "grabber":
          string title = MyFilmsDetail.GetSearchTitle(r, facadeFilms.SelectedListItem.ItemId, "titleoption");
          string mediapath = r[facadeFilms.SelectedListItem.ItemId][conf.StrStorage].ToString();
          if (mediapath.Contains(";")) mediapath = mediapath.Substring(0, mediapath.IndexOf(";", StringComparison.Ordinal)); // take the first source file
          sTitles = MyFilmsDetail.GetSearchTitles(r[conf.StrIndex], mediapath);

          doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
          MyFilmsDetail.grabb_Internet_Informations(title, GetID, conf.StrGrabber_ChooseScript, conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.All, false, sTitles, m_SearchAnimation);
          // Fin_Charge_Init(false, true); // Guzzi: This might be required to reload facade and details ?
          // this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          // RefreshFacade() will be initiated by OnDetailsUpdated handler, if threaded loading is finished ! - so should NOT be loaded here ...
          break;

        case "grabber-person":
          doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
          MyFilmsDetail.grabb_Internet_Informations(facadeFilms.SelectedListItem.Label, GetID, conf.StrGrabber_ChooseScript, "", "", MyFilmsDetail.GrabType.Person, false, null, m_SearchAnimation);
          // Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          // RefreshFacade() will be initiated by OnDetailsUpdated handler, if threaded loading is finished ! - so should NOT be loaded here ...
          break;

        case "addtocollection":
          string groupname = Change_MovieGroupName(conf.StrTIndex);
          if (groupname == "") break;
          MyFilmsDetail.AddMovieToCollection(r[conf.StrIndex], groupname, true);
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "addtocollectionall":
          string groupnameall = Change_MovieGroupName(conf.StrTIndex);
          if (groupnameall == "") break;
          AddAllMoviesToCollection(groupnameall);
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "removefromcollection":
          MyFilmsDetail.RemoveMovieFromCollection(r[conf.StrIndex], true);
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "removefromcollectionall":
          RemoveAllMoviesFromCollection();
          GetPrevFilmList(); // go up in hierarchy, as collection is empty anyway ...
          // Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "fanart":
          if (!Helper.IsConnectedToInternet())
          {
            GUIUtils.ShowErrorDialog(GUILocalizeStrings.Get(703));
            break; // stop, if no internet available
          }
          else
          {
            // Grabber.GrabberUrlClass Grab = new Grabber.GrabberUrlClass();

            string personartworkpath = string.Empty;
            string imdbid = MyFilmsDetail.GetIMDB_Id(r[conf.StrIndex]);
            sTitles = MyFilmsDetail.GetSearchTitles(r[conf.StrIndex], "");
            //string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
            //fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeFilms.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
            if (!string.IsNullOrEmpty(sTitles.FanartTitle) && conf.StrFanart)
            {
              LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - (started from main menu)");
              if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist))
              {
                personartworkpath = conf.StrPathArtist;
                LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
              }
              doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
              MyFilmsDetail.Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director, imdbid, sTitles.Year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, true, false, m_SearchAnimation);
            }
          }
          break;

        case "deletefanart":
          // if (GUIUtils.ShowYesNoDialog(GUILocalizeStrings.Get(1079874), GUILocalizeStrings.Get(433))) //delete fanart (current film) //confirm suppression
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079874)); //delete fanart (current film)
          dlgYesNo.SetLine(1, "");
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433)); //confirm suppression
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            //MyFilmsDetail.Remove_Backdrops_Fanart(MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"].ToString(), false);
            //string fanartTitle = MyFilms.r[facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString();  // Fixed, as it should delete content of master title...
            string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
            fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeFilms.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
            MyFilmsDetail.Remove_Backdrops_Fanart(fanartTitle, false);

            // reload fanart
            string[] wfanart = MyFilmsDetail.Search_Fanart(facadeFilms.SelectedListItem.Label, true, "file", false, facadeFilms.SelectedListItem.ThumbnailImage, string.Empty);
            backdrop.Filename = wfanart[0];
            MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
          }
          break;

        case "togglewatchedstatus":
          if (facadeFilms.SelectedListItem.IsPlayed)
          {
            facadeFilms.SelectedListItem.IsPlayed = false;
            MyFilmsDetail.Watched_Toggle(r[facadeFilms.SelectedListItem.ItemId], false);
          }
          else
          {
            facadeFilms.SelectedListItem.IsPlayed = true;
            MyFilmsDetail.Watched_Toggle(r[facadeFilms.SelectedListItem.ItemId], true);
          }
          //Fin_Charge_Init(true, true);
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "globalfilters":
          var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg1 == null) return;
          dlg1.Reset();
          dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
          var choiceViewGlobalOptions = new List<string>();

          // Change global Unwatchedfilteroption
          // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSuppressPlayStopUpdateUserField))// Make it conditoional, so only displayed, if options enabled in setup !
          if (conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
          if (!conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("globalunwatchedfilter");

          // Change global MovieFilter (Only Movies with media files reachable (requires at least initial scan!)
          if (InitialIsOnlineScan)
          {
            if (GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("filterdbisonline");
          }

          // Change global MovieFilter (Only Movies with Trailer)
          if (Helper.FieldIsSet(conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
          {
            if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("filterdbtrailer");
          }

          // Change global MovieFilter (Only Movies with highRating)
          if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
          if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("filterdbrating");

          //// Change Value for global MovieFilter (Only Movies with highRating)
          //dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MyFilms.conf.StrAntFilterMinRating.ToString()));
          //choiceViewGlobalOptions.Add("filterdbsetrating");

          //if (conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
          //if (!conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
          //choiceViewGlobalOptions.Add("alwaysdefaultview");

          // Change global Movie type Filter (all, 2d, 3d)
          if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Global Filter 3D movies"))
          {
            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798972), GlobalFilterMovieFormat3D));
            choiceViewGlobalOptions.Add("filter3d");
          }

          dlg1.DoModal(GetID);
          if (dlg1.SelectedLabel == -1)
          {
            return;
          }

          Change_Menu_Action(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          //Context_Menu = false;
          return;

        #endregion
      }
    }

    private void OnlineVideosSearchLauncher(string searchexpression)
    {
      string path = "";
      if (r != null && facadeFilms.SelectedListItemIndex > -1 && conf.ViewContext != ViewContext.TmdbMovies)
      {
        path = r[facadeFilms.SelectedListItemIndex][conf.StrStorage].ToString();
        if (path.Contains(";")) path = path.Substring(0, path.IndexOf(";", StringComparison.Ordinal));
        if (path.Contains("\\")) path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));

        path = Path.Combine(path, "Trailer");
      }
      if (!Directory.Exists(path))
        try
        {
          Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("Error creating downloadpath: '" + ex.Message + "'");
          path = "";
        }

      var choiceViewMenu = new List<string>(); // choiceViewMenu.Clear();
      var dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlgmenu == null) return;
      dlgmenu.Reset();
      dlgmenu.SetHeading(GUILocalizeStrings.Get(10798600)); // Search OnlineVideos ...

      dlgmenu.Add(GUILocalizeStrings.Get(10798711)); //search youtube trailer with onlinevideos
      choiceViewMenu.Add("YouTube");

      dlgmenu.Add(GUILocalizeStrings.Get(10798712)); //search apple itunes trailer with onlinevideos
      choiceViewMenu.Add("iTunes Movie Trailers");

      dlgmenu.Add(GUILocalizeStrings.Get(10798716)); //search IMDB trailer with onlinevideos
      choiceViewMenu.Add("IMDb Movie Trailers");

      dlgmenu.Add("FilmStarts.de (OnlineVideos)"); // FilmStarts.de and optional all OnlineVideoSites menu ...
      choiceViewMenu.Add("FilmStarts.de Trailer");

      //dlgmenu.Add("*** show all ***");
      //choiceViewMenu.Add("showall");

      //if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Test, "Details context: Bla !!!"))
      //{
      //}

      dlgmenu.DoModal(GetID);
      if (dlgmenu.SelectedLabel == -1) return;

      string site = choiceViewMenu[dlgmenu.SelectedLabel];
      dlgmenu.DeInit();

      string oVstartparams = "site:" + site + "|category:|search:" + searchexpression + "|return:Locked" + "|downloaddir:" + path + "|downloadmenuentry:" + GUILocalizeStrings.Get(10798749) + " (" + searchexpression + ")"; // MyFilms: Download to movie directory
      LogMyFilms.Debug("Starting OnlineVideos with '" + oVstartparams + "'");
      GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.OnlineVideos, oVstartparams);
    }

    private void BrowseTheWebLauncher(string type, string extension, string searchexpression)
    {
      //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser 
      //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
      if (!Helper.IsBrowseTheWebAvailableAndEnabled)
      {
        GUIUtils.ShowOKDialog("BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.BrowseTheWeb), "", "");
        return;
      }

      string url = string.Empty;
      const string zoom = "150";

      new Thread(delegate()
      {
        {
          try
          {
            MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation);

            #region First search corresponding URL for the movie/person ...
            switch (type)
            {
              case "movie":
                {
                  var imdb = new IMDB();
                  imdb.Find(searchexpression);
                  foreach (IMDB.IMDBUrl t in imdb) LogMyFilms.Debug("movie imdb internet search - found: '" + t.Title + "', URL = '" + t.URL + "'");
                  if (imdb.Count > 0)
                  {
                    var wurl = imdb[0];
                    if (wurl.URL.Length != 0)
                    {
                      url = wurl.URL + extension; // Assign proper Webpage for infos
                      url = ImdbBaseUrl + url.Substring(url.IndexOf("title", StringComparison.Ordinal)); // redirect to base www.imdb.com server and remove localized returns...
                    }
                  }
                  break;
                }
              case "person":
                {
                  //var imdb = new IMDB();
                  //imdb.FindActor(searchexpression);
                  //if (imdb.Count > 0)
                  //{
                  //  IMDB.IMDBUrl wurl = imdb[0];
                  //  if (wurl.URL.Length != 0)
                  //  {
                  //    url = wurl.URL + extension; // Assign proper Webpage for Actorinfos
                  //    url = ImdbBaseUrl + url.Substring(url.IndexOf("name", StringComparison.Ordinal)); // redirect to base www.imdb.com server and remove localized returns...
                  //  }
                  //  //_imdb.GetActorDetails(_imdb[0], false, out imdbActor); // Details here not needed - we just want the URL !
                  //}
                  string grabberscript = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB-Person.xml";
                  if (File.Exists(grabberscript))
                  {
                    ArrayList personUrls = MyFilmsDetail.FindActor(searchexpression, grabberscript);
                    if (personUrls.Count > 0)
                    {
                      var wurl = (GrabberUrlClass.IMDBUrl)personUrls[0];
                      if (wurl.URL.Length != 0)
                      {
                        url = wurl.URL + extension; // Assign proper Webpage for Actorinfos
                        url = ImdbBaseUrl + url.Substring(url.IndexOf("name", StringComparison.Ordinal)); // redirect to base www.imdb.com server and remove localized returns...
                      }
                    }
                  }
                  break;
                }
            }
            #endregion

            MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation);
          }
          catch (Exception ex) { LogMyFilms.Debug("BrowseTheWebLauncher - error searching actor: " + ex.Message); }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            if (!string.IsNullOrEmpty(url))
            {
              #region Load Webbrowserplugin with the URL
              LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url + "'");
              GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
              GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
              MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation);
              GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
              MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation);
              GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
              GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
              #endregion
            }
            else GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798640)); // no result found
            return 0;
          }, 0, 0, null);
      }) { Name = "MyFilmsBrowseTheWebLauncher", IsBackground = true }.Start();
    }

    private void MoviePersonListLauncher(bool savestate, bool askforpersontype)
    {
      #region display person list of current movie
      string persontype = "Persons";
      if (askforpersontype) persontype = SelectPersonType(conf.StrIndex);
      if (string.IsNullOrEmpty(persontype)) return;

      if (savestate) SaveListState(false);

      conf.BoolSkipViewState = false;
      conf.StrPersons = r[conf.StrIndex][persontype].ToString();
      //conf.StrFilmSelect = conf.StrTitle1 + " not like ''";
      conf.StrSelect = conf.StrTitle1 + " like '*" + StringExtensions.EscapeLikeValue(conf.StrTIndex) + "'"; // set view filter to current movie name - use "*" at the beginning to include movies with hierarchies !
      //conf.StrSelect = "";

      conf.WStrSort = persontype;
      conf.WStrSortSens = " ASC";
      SetLabelView(persontype.ToLower());
      conf.ViewContext = ViewContext.Person;
      conf.CurrentView = "MoviePersons";

      MyFilmsDetail.setGUIProperty("moviecontext.value", conf.StrTIndex, true);

      // Change_View_Action(persontype);
      // GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, "*", true, string.Empty);
      // GetSelectFromDivxThreaded();
      // GetSelectFromDivx(conf.StrTitle1 + " not like ''", persontype, conf.wStrSortSens, conf.StrPersons, true, string.Empty);
      // GetSelectFromDivx(conf.StrFilmSelect, conf.wStrSort, conf.wStrSortSens, conf.StrPersons, true, string.Empty);

      GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrPersons, true, string.Empty);
      //new Thread(delegate()
      //{
      //  {
      //    // MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
      //    GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, conf.StrPersons, true, string.Empty);
      //    // MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
      //  }
      //  GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
      //  {
      //    {
      //      GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // removed, as it was causing trouble with menu focus control ...
      //    }
      //    return 0;
      //  }, 0, 0, null);
      //})
      //{
      //  Name = "MyFilmsMoviePersonList",
      //  IsBackground = true,
      //  Priority = ThreadPriority.Normal
      //}.Start();
      GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
      #endregion
    }
    
    //*****************************************************************************************
    //*  select person type dialog
    //*****************************************************************************************
    private string SelectPersonType(int index)
    {
      var choiceSearch = new List<string>();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return string.Empty;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079909)); // select role

      foreach (string personfield in PersonTypes.Where(personfield => r[index][personfield].ToString().Length > 0))
      {
        // IEnumerable<string> wTableau = Search_String(System.Web.HttpUtility.HtmlDecode(HTMLParser.removeHtml(r[index][personfield].ToString()))); var wTableau = new List<string>(wTableau.ToArray().Distinct().ToList()); // make list unique/distinct
        IEnumerable<string> wTableau = Search_String(System.Web.HttpUtility.HtmlDecode(HTMLParser.removeHtml(r[index][personfield].ToString()))).Distinct();

        dlg.Add(BaseMesFilms.TranslateColumn(personfield) + " (" + wTableau.Count() + ")");
        choiceSearch.Add(personfield);
      }
      if (choiceSearch.Count == 0)
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079909), GUILocalizeStrings.Get(10798641));
        return string.Empty;
      }

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return string.Empty;

      string persontype = choiceSearch[dlg.SelectedLabel];
      return persontype;
    }

    //*****************************************************************************************
    //*  search related movies by persons                                                     *
    //*****************************************************************************************
    private void SearchRelatedMoviesbyPersons(int index, bool returnToContextmenu)
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
      string[] roles = { "Actors", "Producer", "Director", "Writer" }; // , "Persons"

      new Thread(delegate()
      {
        {
          if (dlg == null) return;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
          List<string> choiceSearch = new List<string>();
          string personartworkpath = conf.StrPathArtist;

          #region show available persons
          foreach (string role in roles.Where(role => r[index][role].ToString().Length > 0))
          {
            IEnumerable<string> wTableau = Search_String(System.Web.HttpUtility.HtmlDecode(HTMLParser.removeHtml(r[index][role].ToString())));
            foreach (string t in wTableau)
            {
              GUIListItem item = new GUIListItem { Label = BaseMesFilms.TranslateColumn(role) + " : " + t };
              if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist))
              {
                item.IconImage = File.Exists(personartworkpath + "\\" + t + ".jpg") ? personartworkpath + "\\" + t + ".jpg" : conf.DefaultCoverArtist;
              }
              dlg.Add(item);
              choiceSearch.Add(t);
            }
          }

          if (choiceSearch.Count == 0)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079867), GUILocalizeStrings.Get(10798641));
            return;
          }
          #endregion

          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
          {
            if (returnToContextmenu) // only call if it was called from context menu
              Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
            else
              Change_Search_Options();
            return;
          }

          if (dlgPrgrs != null)
          {
            dlgPrgrs.Reset();
            dlgPrgrs.DisplayProgressBar = false;
            dlgPrgrs.ShowWaitCursor = true;
            dlgPrgrs.DisableCancel(true);
            dlgPrgrs.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
            dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
            dlgPrgrs.Percentage = 0;
          }

          string wperson = choiceSearch[dlg.SelectedLabel];
          dlg.Reset();
          choiceSearch.Clear();
          dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson); // function selection (actor, director, producer)

          //First add general option to show MP Actor Infos
          if (wperson.Length > 0)
          {
            if (dlgPrgrs != null)
            {
              dlgPrgrs.Percentage = 19;
              dlgPrgrs.SetLine(1, "search person ...");
            }

          }

          LogMyFilms.Debug("Adding search results for roles to menu ...");
          foreach (string role in roles)
          {
            if (dlgPrgrs != null)
            {
              dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(137) + " " + BaseMesFilms.TranslateColumn(role) + " ..."); // Search <persontype> ...
              dlgPrgrs.Percentage = (dlgPrgrs.Percentage + 20 < 100) ? dlgPrgrs.Percentage + 20 : 100;
            }
            DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, role + " like '*" + wperson + "*'", conf.StrSorta, conf.StrSortSens, false);
            if (wr.Length > 0)
            {
              dlg.Add(GUILocalizeStrings.Get(10798610) + BaseMesFilms.TranslateColumn(role) + "  (" + wr.Length + ")"); // movies as <persontype> (<count>)
              choiceSearch.Add(role);
            }
          }

          if (dlgPrgrs != null)
          {
            dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false;
            dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846));
            Thread.Sleep(10); dlgPrgrs.Close(); // Done ...
          }

          if (choiceSearch.Count == 0)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079867), GUILocalizeStrings.Get(10798640));
            return;
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;

          SaveListState(false); // SaveLastView(); // to restore to current context after search results ...

          conf.StrSelect = choiceSearch[dlg.SelectedLabel] + " like '*" + wperson + "*'"; // conf.StrFilmSelect = choiceSearch[dlg.SelectedLabel] + " like '*" + wperson + "*'"; 
          if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist)) personcover.Filename = File.Exists(personartworkpath + "\\" + wperson + ".jpg") ? personartworkpath + "\\" + wperson + ".jpg" : conf.DefaultCoverArtist;

          #region set localized label
          switch (choiceSearch[dlg.SelectedLabel])
          {
            case "Actors":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079868) + " *" + wperson + @"*"; // "Selection"
              break;
            case "Producer":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798612) + " *" + wperson + @"*";
              break;
            case "Director":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079869) + " *" + wperson + @"*";
              break;
            case "Writer":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798684) + " *" + wperson + @"*";
              break;
            case "Persons":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798951) + " *" + wperson + @"*";
              break;
            default:
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798951) + " *" + wperson + @"*";
              break;
          }
          #endregion
          conf.StrTitleSelect = "";
          conf.StrViewSelect = "";
          // conf.StrFilmSelect = "";
          // conf.ViewContext = ViewContext.Person;
          //conf.wStrSort = choiceSearch[dlg.SelectedLabel];
          //conf.Wselectedlabel = "";
          //conf.wStrSortSens = " ASC";
          //conf.StrPersons = wperson;
          //conf.StrTxtSelect = BaseMesFilms.Translate_Column(choiceSearch[dlg.SelectedLabel]) + " [*" + wperson + @"*]";

          SetLabelView("search"); // show "search"
          GetFilmList();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // this after thread finished ...
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "SearchRelatedMoviesbyPersons", IsBackground = true }.Start();
    }

    private void GetAndStorePersonInfo(string personName, ref IMDBActor imdbActor)
    {
      string grabberscript = MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\IMDB-Person.xml";
      if (File.Exists(grabberscript))
      {
        ArrayList personUrls = MyFilmsDetail.FindActor(personName, grabberscript);
        LogMyFilms.Debug("IMDB - " + personUrls.Count + " person(s) found for '" + personName + "' with person grabber script '" + grabberscript + "'");
        if (personUrls.Count > 0)
        {
          var wurl = (GrabberUrlClass.IMDBUrl)personUrls[0];
          if (wurl.URL.Length != 0)
          {
            MyFilmsDetail.GrabActorDetails(wurl.URL, grabberscript, out imdbActor);
          }
        }
      }

      if (!string.IsNullOrEmpty(imdbActor.ThumbnailUrl))
      {
        if (!string.IsNullOrEmpty(conf.StrPathArtist))
        {
          string filename1person = GrabUtil.DownloadPersonArtwork(conf.StrPathArtist, imdbActor.ThumbnailUrl, personName, true, true, out filename1person);
          LogMyFilms.Info("Person Artwork '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + personName + "', path = '" + filename1person + "'");
        }
        else
        {
          LogMyFilms.Debug("IMDBFetcher single actor fetch could not be done - no person artwork path defined in MyFilms config");
        }
      }
      else
      {
        Log.Debug("IMDBFetcher single actor fetch: url=null or empty for actor {0}", personName);
      }
      int actorId = GUIUtils.AddActor(imdbActor.IMDBActorID, imdbActor.Name);
      if (actorId > 0)
      {
        VideoDatabase.SetActorInfo(actorId, imdbActor);
      }
    }

    private IMDBActor grabb_Person_Details(string name)
    {
      var person = new IMDBActor();
      MyFilmsDetail.grabb_Internet_Informations_Search(name, GetID, conf.StrGrabber_cnf, "", MyFilmsDetail.GrabType.Person, null, m_SearchAnimation);
      return person;
    }

    #region Old Zebons methods (disabled)
    ////*****************************************************************************************
    ////*  search related movies by properties  (From ZebonsMerge, renamed to ...Zebons)		  *
    ////*****************************************************************************************
    //private void SearchRelatedMoviesbyPropertiesZebons(int Index, IEnumerable<string> wSearchList)
    //{
    //    // first select the property to be searching on

    //    AntMovieCatalog ds = new AntMovieCatalog();
    //    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
    //    System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
    //    GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
    //    ArrayList w_tableau = new ArrayList();
    //    if (dlg == null) return;
    //    dlg.Reset();
    //    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
    //    foreach (string wSearch in wSearchList)
    //    {
    //        dlg.Add(GUILocalizeStrings.Get(10798617) + "'" + BaseMesFilms.Translate_Column(wSearch) + "'");
    //        choiceSearch.Add(wSearch);
    //    }
    //    dlg.DoModal(GetID);
    //    if (dlg.SelectedLabel == -1)
    //        return;
    //    string wproperty = choiceSearch[dlg.SelectedLabel];
    //    dlg.Reset();
    //    if (choiceSearch.Count == 0)
    //    {
    //        if (dlg1 == null) return;
    //        dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
    //        dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
    //        dlg1.SetLine(2, BaseMesFilms.Translate_Column(wproperty));
    //        dlg1.DoModal(GUIWindowManager.ActiveWindow);
    //        return;
    //    }
    //    choiceSearch.Clear();
    //    if (ds.Movie.Columns[wproperty].DataType.Name == "string")
    //        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MesFilms.r[Index][wproperty].ToString())));
    //    else
    //        w_tableau.Add(MesFilms.r[Index][wproperty].ToString());
    //    foreach (object t in w_tableau)
    //    {
    //        dlg.Add(wproperty + " : " + t);
    //        choiceSearch.Add(t.ToString());
    //    }
    //    if (choiceSearch.Count == 0)
    //    {
    //        if (dlg1 == null) return;
    //        dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
    //        dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
    //        dlg1.SetLine(2, BaseMesFilms.Translate_Column(wproperty));
    //        dlg1.DoModal(GUIWindowManager.ActiveWindow);
    //        return;
    //    }
    //    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
    //    dlg.DoModal(GetID);
    //    if (dlg.SelectedLabel == -1)
    //        return;
    //    if (ds.Movie.Columns[wproperty].DataType.Name == "string")
    //    {
    //        conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
    //        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
    //    }
    //    else
    //    {
    //        conf.StrSelect = wproperty + " = '" + choiceSearch[dlg.SelectedLabel].ToString() + "'";
    //        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [" + choiceSearch[dlg.SelectedLabel].ToString() + @"]";
    //    }

    //    conf.StrTitleSelect = "";
    //    GetFilmList();
    //}


    //*****************************************************************************************
    //*  Global search movies by properties (ZebonsMerge - Renamed to ....Zebons)             *
    //*****************************************************************************************
    //     private void SearchMoviesbyPropertiesZebons(IEnumerable<string> wSearchList)
    //     {
    //         // first select the property to be searching on

    //         AntMovieCatalog ds = new AntMovieCatalog();
    //         GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
    //         GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
    //         System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
    //         ArrayList w_tableau = new ArrayList();
    //         ArrayList w_tablabel = new ArrayList();
    //         if (dlg == null) return;
    //         dlg.Reset();
    //         dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu
    //         dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
    //         choiceSearch.Add("all");
    //         foreach (string wSearch in wSearchList)
    //         {
    //             dlg.Add(GUILocalizeStrings.Get(10798617) + "'" + BaseMesFilms.Translate_Column(wSearch) + "'");
    //             choiceSearch.Add(wSearch);
    //         }
    //         dlg.DoModal(GetID);
    //         if (dlg.SelectedLabel == -1)
    //             return;
    //         string wproperty = choiceSearch[dlg.SelectedLabel];
    //         VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
    //         if (null == keyboard) return;
    //         keyboard.Reset();
    //         keyboard.Text = "";
    //         keyboard.DoModal(GetID);
    //         if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
    //         {
    //             switch (choiceSearch[dlg.SelectedLabel])
    //             {
    //                 case "all":
    //                     ArrayList w_count = new ArrayList();
    //                     if (dlg == null) return;
    //                     dlg.Reset();
    //                     dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
    //                     DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
    //                     foreach (DataRow wsr in wr)
    //                     {
    //                         foreach (string wsearch in wSearchList)
    //                         {
    //                             if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[wsearch].ToString().ToLower())).Contains(keyboard.Text.ToLower()))
    //                                 // column contains text serached on : added to w_tableau + w_count
    //                                 if (w_tableau.Contains(wsearch))
    //                                 // search position in w_tableau for adding +1 to w_count
    //                                 {
    //                                     for (int i = 0; i < w_tableau.Count; i++)
    //                                     {
    //                                         if (w_tableau[i].ToString() == wsearch)
    //                                         {
    //                                             w_count[i] = (int)w_count[i] + 1;
    //                                             break;
    //                                         }
    //                                     }
    //                                 }
    //                                 else
    //                                 // add to w_tableau and move 1 to w_count
    //                                 {
    //                                     w_tableau.Add(wsearch.ToString());
    //                                     w_count.Add(1);
    //                                 }
    //                         }
    //                         //foreach (DataColumn dc in ds.Movie.Columns)
    //                         //{
    //                         //    if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[dc.ColumnName].ToString())).Contains(keyboard.Text))
    //                         //        // column contains text serached on : added to w_tableau + w_count
    //                         //        if (w_tableau.Contains(dc.ColumnName))
    //                         //        // search position in w_tableau for adding +1 to w_count
    //                         //        {
    //                         //            for (int i = 0; i < w_tableau.Count; i++)
    //                         //            {
    //                         //                if (w_tableau[i].ToString() == dc.ColumnName)
    //                         //                {
    //                         //                    w_count[i] = (int)w_count[i] + 1;
    //                         //                    break;
    //                         //                }
    //                         //            }
    //                         //        }
    //                         //        else
    //                         //        // add to w_tableau and move 1 to w_count
    //                         //        {
    //                         //            w_tableau.Add(dc.ColumnName.ToString());
    //                         //            w_count.Add(1);
    //                         //        }
    //                         //}
    //                     }
    //                     if (w_tableau.Count == 0)
    //                     {
    //                         if (dlg1 == null) return;
    //                         dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
    //                         dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
    //                         dlg1.SetLine(2, keyboard.Text);
    //                         dlg1.DoModal(GUIWindowManager.ActiveWindow);
    //                         return;
    //                     }
    //                     dlg.Reset();
    //                     dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618),keyboard.Text)); // menu
    //                     choiceSearch.Clear();
    //                     for (int i = 0; i < w_tableau.Count; i++)
    //                     {
    //                         dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), w_count[i], BaseMesFilms.Translate_Column(w_tableau[i].ToString())));
    //                         choiceSearch.Add(w_tableau[i].ToString());
    //                     }
    //                     dlg.DoModal(GetID);
    //                     if (dlg.SelectedLabel == -1)
    //                         return;
    //                     wproperty = choiceSearch[dlg.SelectedLabel];
    //                     if (control_searchText(keyboard.Text))
    //                     {
    //                         if (ds.Movie.Columns[wproperty].DataType.Name == "string")
    //                         {
    //                             conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
    //                             conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
    //                         }
    //                         else
    //                         {
    //                             conf.StrSelect = wproperty + " = '" + keyboard.Text + "'";
    //                             conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [" + keyboard.Text + @"]";
    //                         }
    //                     //}
    //                     //if (control_searchText(keyboard.Text))
    //                     //{
    //                     //    if (wproperty == "Rating")
    //                     //        conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                     //    else
    //                     //        if (wproperty == "Number")
    //                     //            conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                     //        else
    //                     //            conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
    //                         //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + keyboard.Text + @"*]";
    //                         conf.StrTitleSelect = "";
    //                         //                         GetSelectFromDivx(conf.StrSelect, wproperty, conf.wStrSortSens, keyboard.Text, true, "");
    //                         GetFilmList();
    //                     }
    //                     break;
    //                 default:
    //                     if (control_searchText(keyboard.Text))
    //                     {
    //                         switch (wproperty)
    //                         {
    //                             case "Rating":
    //                                 conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                                 break;
    //                             case "Number":
    //                                 conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                                 break;
    //                             default:
    //                                 conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
    //                                 break;
    //                         }
    //                         conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]";
    //                         conf.StrTitleSelect = "";
    ////                         GetSelectFromDivx(conf.StrSelect, wproperty, conf.wStrSortSens, keyboard.Text, true, "");
    //                         GetFilmList();
    //                     }
    //                     break;
    //             }
    //         }
    //     }
    #endregion

    //*****************************************************************************************
    //*  search related movies by properties  (Guzzi Version)                                 *
    //*****************************************************************************************
    private void SearchRelatedMoviesbyProperties(int index, bool returnToContextmenu) // (int Index, IEnumerable<string> wSearchList)
    {
      // first select the property to be searching on
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      List<string> choiceSearch = new List<string>();

      int minChars = 2;
      const bool filter = true;
      if (dlg == null) return;

      conf.StrSelect = conf.StrViewSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;

      #region build menu
      ArrayList displayItems = GetDisplayItems("view");

      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // Choose property for search

      dlg.Add(BaseMesFilms.TranslateColumn(conf.StrTitle1));
      choiceSearch.Add(conf.StrTitle1);

      if (Helper.FieldIsSet(conf.StrTitle2))
      {
        dlg.Add(BaseMesFilms.TranslateColumn(conf.StrTitle2));
        choiceSearch.Add(conf.StrTitle2);
      }

      string[] defaultupdateitems = { "Category", "Year", "Date", "Country", "Rating" };
      foreach (string wupd in defaultupdateitems)
      {
        dlg.Add(BaseMesFilms.TranslateColumn(wupd));
        choiceSearch.Add(wupd);
      }

      dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
      choiceSearch.Add("showall");

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu) // only call if it was called from context menu
          Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
        else
          Change_Search_Options();
        return;
      }

      // show all search fields, if selected ...
      if (choiceSearch[dlg.SelectedLabel] == "showall")
      {
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
        choiceSearch.Clear();
        foreach (string[] displayItem in displayItems)
        {
          if (!string.IsNullOrEmpty(r[index][displayItem[0]].ToString()))
          {
            string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
            dlg.Add(entry);
            choiceSearch.Add(displayItem[0]);
            LogMyFilms.Debug("search related movie properties menu - add '{0}' as '{1}'", displayItem[0], entry);
          }
          else
            LogMyFilms.Debug("SearchRelatedMoviesByProperties: Property '" + displayItem[0] + "' not added to menu list, as it is null or empty !");
        }
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1)
        {
          if (returnToContextmenu) // only call if it was called from context menu
            Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
          else
            Change_Search_Options();
          return;
        }
      }
      #endregion
      string wproperty = choiceSearch[dlg.SelectedLabel];

      #region perform search action for related properties and show items (and optional subitems)
      dlg.Reset();
      choiceSearch.Clear();
      LogMyFilms.Debug("(RelatedPropertySearch): Searchstring in Property: '" + r[index][wproperty] + "'");
      
      //PersonTitle Grabbing (Double Words)
      IEnumerable<string> wTableau = new List<string>();
      if (wproperty != "Description" && wproperty != "Comments" && wproperty != "Rating")
      {
        wTableau = Search_String(r[index][wproperty].ToString());
        foreach (string t in wTableau)
        {
          foreach (string entry in from string[] displayItem in displayItems where wproperty.Equals(displayItem[0]) select (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1])
          {
            dlg.Add(entry + ": " + t);
            choiceSearch.Add(t);
            LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
            break;
          }
        }
      }

      //SubWordGrabbing for more Details, if necessary
      minChars = wproperty != "Description" && wproperty != "Comments" ? 2 : 5;

      if (wproperty == "Rating")
      {
        dlg.Add(GUILocalizeStrings.Get(10798657) + ": = " + r[index][wproperty].ToString().Replace(",", "."));
        choiceSearch.Add("RatingExact");
        dlg.Add(GUILocalizeStrings.Get(10798657) + ": > " + r[index][wproperty].ToString().Replace(",", "."));
        choiceSearch.Add("RatingBetter");
      }
      else
      {
        LogMyFilms.Debug("(RelatedPropertySearch): Length: '" + r[index][wproperty].ToString().Length + "'");
        if (r[index][wproperty].ToString().Length > 0)
        {
          foreach (string value in SubWordGrabbing(r[index][wproperty].ToString(), minChars, filter))
          {
            if (wTableau.Contains(value)) // Only Add SubWordItems if not already present in SearchStrin Table
            {
              LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result already Present: '" + value + "'");
              break;
            }
            else
            {
              foreach (string entry in from string[] displayItem in displayItems where wproperty.ToLower().Equals(displayItem[0].ToLower()) select (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1])
              {
                dlg.Add(entry + " (" + GUILocalizeStrings.Get(10798627) + "): '" + value + "'"); // property (singleword): 'value'
                choiceSearch.Add(value);
                LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result Add: '" + value + "'");
                break;
              }
            }
          }
        }
      }
      if (choiceSearch.Count == 0)
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798625));
        return;
      }
      #endregion

      #region now search the user selection (value)
      SaveListState(false);

      LogMyFilms.Debug("(Related Search by properties - ChoiceSearch.Count: " + choiceSearch.Count);
      if (choiceSearch.Count > 0)
      {
        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
      }
      else dlg.SelectedLabel = 0;
      LogMyFilms.Debug("(Related Search by properties - Selected wproperty: " + wproperty + "'");
      string wRating = "0";

      if (choiceSearch.Count == 0) //Use Special "is NULL" handling if property is empty ...
        conf.StrSelect = wproperty + " is NULL";
      else
        wRating = r[index][wproperty].ToString().Replace(",", ".");
      if (wproperty == "Rating" && choiceSearch[dlg.SelectedLabel] == "RatingExact")
        conf.StrSelect = wproperty + " = " + wRating;
      else if (wproperty == "Rating" && choiceSearch[dlg.SelectedLabel] == "RatingBetter")
        conf.StrSelect = wproperty + " > " + wRating;
      else
        {
          UpdateRecentSearch(choiceSearch[dlg.SelectedLabel], MaxSearchHistoryLength);
          // conf.StrSelect = (wproperty == "Number") ? wproperty + " = " + choiceSearch[dlg.SelectedLabel] : wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
          conf.StrSelect = Equalexpression(wproperty, choiceSearch[dlg.SelectedLabel]);
        }
      if (choiceSearch.Count == 0)
        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " (none)";
      else
        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " *" + choiceSearch[dlg.SelectedLabel] + @"*";
      conf.StrTitleSelect = string.Empty;
      #endregion
      SetLabelView("search"); // show "search"
      GetFilmList();
    }

    //******************************************************************************************************
    //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
    //******************************************************************************************************
    private void SearchMoviesbyRandomWithTrailer(bool returnToContextmenu)
    {
      currentTrailerMoviesList.Clear();
      // first select the area where to make random search on - "all", "category", "year", "country" - then populate the currentTrailerMovieList
      AntMovieCatalog ds = new AntMovieCatalog();
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      List<string> choiceSearch = new List<string>();
      ArrayList wTableau = new ArrayList();

      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798621)); // menu for random search
      dlg.Add(GUILocalizeStrings.Get(10798622)); // random search on all
      choiceSearch.Add("randomall");
      dlg.Add(GUILocalizeStrings.Get(10798664)); // random search on Genre
      choiceSearch.Add("Category");
      dlg.Add(GUILocalizeStrings.Get(10798665)); // random search on year
      choiceSearch.Add("Year");
      dlg.Add(GUILocalizeStrings.Get(10798663)); // random search on country
      choiceSearch.Add("Country");
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu)
          Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
        else
          Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];

      ArrayList wCount = new ArrayList();
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;
      DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
      wTableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
      wCount.Add(0);
      foreach (DataRow wsr in wr)
      {
        foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ToString().Contains(wproperty)))
        {
          if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
          {
            wCount[0] = (int)wCount[0] + 1;
            break;
          }
          else
          {
            foreach (string t in SubItemGrabbing(wsr[dc.ColumnName].ToString()))
            {
              if (wTableau.Contains(t)) // search position in w_tableau for adding +1 to w_count
              {
                for (int i = 0; i < wTableau.Count; i++)
                {
                  if (wTableau[i].ToString() == t)
                  {
                    wCount[i] = (int)wCount[i] + 1;
                    break;
                  }
                }
              }
              else // add to w_tableau and move 1 to w_count
              {
                LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
                wTableau.Add(t);
                wCount.Add(1);
              }
            }
          }
        }
      }
      if (wTableau.Count == 0) LogMyFilms.Debug("PropertyClassCount is 0");

      string wproperty2;

      if (wproperty != "randomall")
      {
        dlg.Reset();
        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
        choiceSearch.Clear();
        //w_tableau = Array.Sort(w_tableau);
        for (int i = 0; i < wTableau.Count; i++)
        {
          dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), wCount[i], wTableau[i]));
          choiceSearch.Add(wTableau[i].ToString());
        }
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
        wproperty2 = choiceSearch[dlg.SelectedLabel];
      }
      else
        wproperty2 = "*";

      LogMyFilms.Debug("(RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
      switch (wproperty)
      {
        case "randomall":
          conf.StrSelect = string.Empty;
          conf.StrTxtSelect = "Selection *";
          conf.StrTitleSelect = string.Empty;
          SetLabelView("search"); // show "search"
          break;
        case "Rating":
          conf.StrSelect = wproperty + " = " + wproperty2;
          break;
        case "Number":
          conf.StrSelect = wproperty + " = " + wproperty2;
          break;
        default:
          if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
            conf.StrSelect = wproperty + " like ''";
          else
            conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
          break;
      }

      GetFilmList(); // load facade with selected group of movies

      LogMyFilms.Debug("(RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
      conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " *" + wproperty2 + @"*";
      conf.StrTitleSelect = string.Empty;

      // we have now: wproperty = selected category (randomall for all) and wproperty2 = value to search after

      foreach (DataRow sr in r.Where(sr => Helper.FieldIsSet(conf.StrStorageTrailer) && sr[conf.StrStorageTrailer].ToString().Trim() != ""))
      {
        try
        {
          var movie = new MFMovie();
          movie.Config = Configuration.CurrentConfig; // MF config context
          movie.ID = !string.IsNullOrEmpty(sr["Number"].ToString()) ? Int32.Parse(sr["Number"].ToString()) : 0;
          movie.Year = Int32.Parse(sr["Year"].ToString());
          movie.Title = sr["OriginalTitle"].ToString();

          string mediapath = string.Empty;
          if (Helper.FieldIsSet(conf.StrStorageTrailer))
          {
            mediapath = sr[conf.StrStorageTrailer].ToString();
            if (mediapath.Contains(";")) // take the first source file
              mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
          }
          movie.File = mediapath;
          movie.DateAdded = sr["Date"].ToString();

          currentTrailerMoviesList.Add(movie);
        }
        catch (Exception mex)
        {
          LogMyFilms.Error("add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
        }
      }
      // Remove any blocked movies
      //moviesToPlay.RemoveAll(movie => TraktSettings.BlockedFolders.Any(f => movie.Path.Contains(f)));

      //// get the movies that we have watched
      //List<MFMovie> MovieList = (from MFMovie movie in allmovies select movie).ToList();
      //List<MFMovie> movielist = moviesToPlay.Where(m => m.Watched == true).ToList();

      //// Populte currentmovieslist as GUIListItems
      //for (int i = 0; i < facadeFilms.Count; i++)
      //{
      //}

      // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
      LogMyFilms.Debug("(ResultBuildIndex) Found " + currentTrailerMoviesList.Count + " Records matching '" + wproperty2 + "' in '" + wproperty + "'");
      for (int i = 0; i < currentTrailerMoviesList.Count; i++)
        LogMyFilms.Debug("(ResultList) - Index: '" + i + "' - Number: '" + currentTrailerMoviesList[i].ID + "'");
      if (currentTrailerMoviesList.Count == 0)
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0" + @"\n" + "Keine Filme in der Auswahl vorhanden"); // menu for random search
        return;
      }

      PlayRandomTrailer(false);

      //////Choose Random Movie from Resultlist
      ////System.Random rnd = new System.Random();
      ////Int32 RandomNumber = rnd.Next(currentTrailerMoviesList.Count);
      ////LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "', Record: '" + currentTrailerMoviesList[RandomNumber].ID + "', RandomTitle: '" + currentTrailerMoviesList[RandomNumber].Title + "'");

      //////Set Filmlist to random Movie:
      ////conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
      ////conf.wStrSort = conf.StrSTitle;
      ////conf.Boolselect = false;
      ////conf.Boolreturn = false;

      ////conf.StrSelect = "number = " + Convert.ToInt32(currentTrailerMoviesList[RandomNumber].ID);
      ////conf.StrTxtSelect = "Selection number [" + Convert.ToInt32(currentTrailerMoviesList[RandomNumber].ID) + "]";
      ////conf.StrTitleSelect = string.Empty;
      //////GetSelectFromDivx(conf.StrSelect, wproperty, conf.wStrSortSens, keyboard.Text, true, "");
      ////LogMyFilms.Debug("(Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | wStrSort: " + conf.wStrSort);
      ////SetLabelView("search"); // show "search"
      ////GetFilmList(); // Added to update facade

      //////Set Context to first and only title in facadeview
      ////facadeFilms.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
      ////if (!facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
      ////// New Window for detailed selected item information
      ////{
      ////  conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
      ////  conf.StrTIndex = facadeFilms.SelectedListItem.Label;
      ////  GUITextureManager.CleanupThumbs();
      ////  //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
      ////}
      ////else
      ////// View List as selected
      ////{
      ////  conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
      ////  Change_Layout_Action(MyFilms.conf.StrLayOut);
      ////  if (facadeFilms.SelectedListItem.IsFolder)
      ////    conf.Boolreturn = false;
      ////  else
      ////    conf.Boolreturn = true;
      ////  do
      ////  {
      ////    if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
      ////    conf.StrTitleSelect += conf.Wselectedlabel;
      ////  } while (GetFilmList() == false); //keep calling while single folders found
      ////}

      ////PlayRandomTrailer(false);

      ////LogMyFilms.Debug("(SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
    }

    private void PlayRandomTrailersInit(string currentLabel, string categoryfilter)
    {
      LogMyFilms.Debug("PlayRandomTrailersInit() - currentLabel = '" + currentLabel + "', showCategorySelection = '" + categoryfilter + "'");
      currentTrailerMoviesList.Clear();

      #region Collect films with trailers in active view
      foreach (DataRow sr in r.Where(sr => Helper.FieldIsSet(conf.StrStorageTrailer) && sr[conf.StrStorageTrailer].ToString().Trim() != ""))
      {
        try
        {
          MFMovie movie = new MFMovie();
          movie.Config = Configuration.CurrentConfig; // MF config context
          movie.ID = !string.IsNullOrEmpty(sr["Number"].ToString()) ? Int32.Parse(sr["Number"].ToString()) : 0;
          int year;
          movie.Year = Int32.TryParse(sr["Year"].ToString(), out year) ? year : 1900;
          movie.Title = Helper.TitleWithoutGroupName(sr["OriginalTitle"].ToString());
          movie.TranslatedTitle = Helper.TitleWithoutGroupName(sr["TranslatedTitle"].ToString());

          string mediapath = string.Empty;
          if (Helper.FieldIsSet(conf.StrStorageTrailer))
          {
            mediapath = sr[conf.StrStorageTrailer].ToString();
            if (mediapath.Contains(";")) mediapath = mediapath.Substring(0, mediapath.IndexOf(";", StringComparison.Ordinal)).Trim();  // take the first source file
          }
          movie.File = mediapath;
          movie.Trailer = mediapath;
          movie.DateAdded = sr["Date"].ToString();

          currentTrailerMoviesList.Add(movie);
        }
        catch (Exception mex)
        {
          LogMyFilms.Error("add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
        }
      }
      #endregion

      #region Remove any blocked movies (disabled)
      //moviesToPlay.RemoveAll(movie => TraktSettings.BlockedFolders.Any(f => movie.Path.Contains(f)));

      //// get the movies that we have watched
      //List<MFMovie> MovieList = (from MFMovie movie in allmovies select movie).ToList();
      //List<MFMovie> movielist = moviesToPlay.Where(m => m.Watched == true).ToList();

      //// Populte currentmovieslist as GUIListItems
      //for (int i = 0; i < facadeFilms.Count; i++)
      //{
      //}
      #endregion

      LogMyFilms.Debug("PlayRandomTrailersInit() - Found " + currentTrailerMoviesList.Count + " Records with trailers in current view");
      if (currentTrailerMoviesList.Count == 0)
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0" + @"\n" + "Keine Filme in der Auswahl vorhanden"); // menu for random search
        return;
      }
      PlayRandomTrailer(false);
    }

    private void SelectRandomMovie()
    {
      if (facadeFilms.SelectedListItemIndex == -1 || facadeFilms.Count < 2 ||
          !(conf.ViewContext == ViewContext.Movie || conf.ViewContext == ViewContext.MovieCollection))
      {
        LogMyFilms.Warn("SelectRandomMovie() - invalid context - return without action!");
        return;
      }
      
      string currentLabel = conf.StrTIndex;
      int currentIndex = conf.StrIndex;
      // int iMoviesInList = r.Count();
      int iMoviesInList = facadeFilms.Count;
      LogMyFilms.Debug("SelectRandomMovie() - currentLabel = '" + currentLabel + "', Items in current View = '" + iMoviesInList + "'");

      //Choose Random Movie from Resultlist
      var rnd = new Random();
      Int32 randomNumber = rnd.Next(iMoviesInList);
      string randomTitle = facadeFilms[randomNumber].Label;
      LogMyFilms.Debug("RandomNumber: '" + randomNumber + "', RandomTitle: '" + randomTitle + "'");

      // set the focus to random selected movie ...
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, randomNumber);

      // update current values
      conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
      conf.StrTIndex = facadeFilms.SelectedListItem.Label;

      //if (randomTitle.Length > 0 && randomTitle.Contains(MyFilms.conf.TitleDelim))
      //{
      //  randomTitle = randomTitle.Substring(randomTitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
      //}
      //GetFilmList(randomTitle);
    }

    private void OnTrailerEnded(string filename)
    {
      LogMyFilms.Debug("OnTrailerEnded(): Received TrailerEnded event with filename '" + filename + "', trailerscrobbleactive = '" + trailerscrobbleactive + "'");
      if (trailerscrobbleactive) trailerscrobbleactive = false;

      new Thread(delegate()
      {
        {
          Thread.Sleep(2000);
          // GUIControl.ShowControl(GetID, 34);
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            PlayRandomTrailer(true);
            return 0;
          }, 0, 0, null);
      }) { Name = "MyFilmsLaunchPlayRandomTrailer", IsBackground = true }.Start();
    }

    internal void PlayRandomTrailer(bool showMenu)
    {
      LogMyFilms.Debug("PlayRandomTrailer() - showMenu = '" + showMenu + "'");
      if (showMenu)
      {
        //bool dontplaynexttrailer = GUIUtils.ShowCustomYesNoDialog(
        //  GUILocalizeStrings.Get(10798981), // Trailer Scrobbling ...
        //  GUILocalizeStrings.Get(10798982), // Play next Trailer ?
        //  GUILocalizeStrings.Get(10798983), // Cancel
        //  GUILocalizeStrings.Get(10798984), // Next Trailer
        //  false,
        //  10);
        //if (dontplaynexttrailer)
        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
        dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798981)); // Trailer Scrobbling ...
        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798982)); // Play next Trailer ?
        dlgYesNo.TimeOut = 10;
        dlgYesNo.SetYesLabel(GUILocalizeStrings.Get(10798983)); // Cancel
        dlgYesNo.SetNoLabel(GUILocalizeStrings.Get(10798984)); // Next Trailer
        dlgYesNo.SetDefaultToYes(false);
        dlgYesNo.DoModal(ID_MyFilms);
        if (dlgYesNo.IsConfirmed)
        {
          trailerscrobbleactive = false;
          // TrailerScrobbleOptionsMenu(0);
          // MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
          return;
        }
      }

      //Choose Random Movie from Resultlist
      Random rnd = new Random();
      Int32 randomNumber = rnd.Next(currentTrailerMoviesList.Count);
      LogMyFilms.Debug("RandomNumber: '" + randomNumber + "', Record: '" + currentTrailerMoviesList[randomNumber].ID + "', RandomTitle: '" + currentTrailerMoviesList[randomNumber].Title + "'");

      ArrayList trailer = new ArrayList { currentTrailerMoviesList[randomNumber].File };
      currentTrailerPlayingItem = currentTrailerMoviesList[randomNumber];

      // set the active movie in facade
      for (int i = 0; i < r.Length; i++)
      {
        DataRow sr = r[i];
        if (!string.IsNullOrEmpty(sr["Number"].ToString()) && currentTrailerPlayingItem != null && currentTrailerPlayingItem.ID == Int32.Parse(sr["Number"].ToString()))
        {
          conf.StrIndex = i;
          conf.StrTIndex = sr[conf.StrTitle1].ToString();
          try
          {
            for (int ii = 0; i < facadeFilms.Count; ii++)
            {
              if (facadeFilms[ii].Label == conf.StrTIndex)
              {
                GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, ii);
                // facadeFilms.SelectedListItemIndex = ii;
                break;
              }
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("PlayRandonTrailer() - error: " + ex.StackTrace);
          }
        }
      }
      trailerscrobbleactive = true;

      MyFilmsDetail.trailerPlayed = true;
      MyFilmsDetail.Launch_Movie_Trailer_Scrobbling(trailer, ID_MyFilms);
    }

    private void TrailerScrobbleOptionsMenu(int currentNumber)
    {
      LogMyFilms.Debug("TrailerScrobbleOptionsMenu() - currentNumber = '" + currentNumber + "'");
      //// Exit fullscreen Video so we can see main facade again			
      //if (GUIGraphicsContext.IsFullScreenVideo) {
      //  GUIGraphicsContext.IsFullScreenVideo = false;
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      List<string> choiceSearch = new List<string>();

      while (dlg.SelectedLabel != -1)
      {
        dlg.Reset();
        choiceSearch.Clear();
        //dlg.SetHeading(string.Format(MesFilms.r[Convert.ToInt32(w_index[currentNumber])]["Originaltitle"].ToString())); // menu
        dlg.SetHeading(string.Format("Wählen Sie eine Aktion aus")); // menu
        //if (TrailerIsAvailable) // Später den Eintrag nur anzeigen, wenn auch ein Trailer verfügbar ist (siehe Klassen option)
        dlg.Add(string.Format("Hauptfilm spielen"));
        choiceSearch.Add("PlayMovie");
        dlg.Add(string.Format("Trailer spielen"));
        choiceSearch.Add("PlayMovieTrailer");
        dlg.Add(string.Format("Zeige Filmdetails")); //(goes to MesFilmsDetails ID7987 with selected record to show DetailScreen)
        choiceSearch.Add("ShowMovieDetails");
        dlg.Add(string.Format("Zeige alle Filme des gewählten Bereiches"));
        choiceSearch.Add("ShowMovieList");
        dlg.Add(string.Format("Neue Zufallssuche in gleicher Kategorie"));
        choiceSearch.Add("RepeatSearch");
        dlg.Add(string.Format("Neue globale Zufallssuche"));
        choiceSearch.Add("NewSearch");
        dlg.Add(string.Format("Zurück zum Hauptmenü", "Back"));
        choiceSearch.Add("Back");
        dlg.DoModal(ID_MyFilms);
        if ((dlg.SelectedLabel == -1) || (dlg.SelectedLabel == 6)) return;

        switch (choiceSearch[dlg.SelectedLabel])
        {
          case "PlayMovie":
            MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, null, MyFilmsDetail.PlayerOption.Internal);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            return;
          case "PlayMovieTrailer":
            //Hier muß irgendwie sichergestellt werden, daß nach Rückkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            //conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
            //Change_Layout_Action(MesFilms.conf.StrLayOut);
            //conf.Boolreturn = true;
            //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
            //conf.StrTitleSelect += conf.Wselectedlabel;
            //while (GetFilmList() == false) ; //keep calling while single folders found

            MyFilmsDetail.trailerPlayed = true;
            MyFilmsDetail.Launch_Movie_Trailer(facadeFilms.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
            // MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);    
            // if (GUIUtils.ShowYesNoDialog("Wollen Sie den Hauptfilm sehen?", MyFilms.r[Convert.ToInt32(currentTrailerMoviesList[currentNumber].ID)]["Originaltitle"].ToString() + "\n" + "Current ID = '" + currentTrailerMoviesList[currentNumber].ID + "'"))
            var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
            dlgYesNo.SetLine(1, r[Convert.ToInt32(currentTrailerMoviesList[currentNumber].ID)]["Originaltitle"].ToString());
            dlgYesNo.SetLine(2, "Current ID = '" + currentTrailerMoviesList[currentNumber].ID + "'");
            dlgYesNo.DoModal(GetID);
            if (dlgYesNo.IsConfirmed)
              MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, MyFilmsDetail.PlayerOption.Internal);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            break;
          case "ShowMovieDetails":
            // New Window for detailed selected item information
            //conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            GUITextureManager.CleanupThumbs();
            GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
            return;

          case "ShowMovieList":
            //GetFilmList(); //Is this necessary????
            //conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            GUIControl.FocusControl(ID_MyFilms, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            return;

          case "RepeatSearch":
            PlayRandomTrailer(false);
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            // if (GUIUtils.ShowYesNoDialog("Wollen Sie den Hauptfilm sehen?", GUILocalizeStrings.Get(219) + "\n" + "Zufällige Film ID = '" + currentTrailerMoviesList[currentNumber].ID + "'"))
            var dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
            dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
            dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + currentTrailerMoviesList[currentNumber].ID + "'");
            dlg1YesNo.DoModal(ID_MyFilms);
            if (dlg1YesNo.IsConfirmed)              //Launch_Movie(select_item, GetID, m_SearchAnimation);
              MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, null, MyFilmsDetail.PlayerOption.Internal);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            break;
          case "NewSearch":
            SearchMoviesbyRandomWithTrailer(false);
            return;
          case "Back":
            return;
        }
      }
    }

    //******************************************************************************************************
    //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
    //******************************************************************************************************
    private void SearchMoviesbyRandomWithTrailerOriginalVersion(bool returnToContextmenu)
    {
      // first select the area where to make random search on - "all", "category", "year", "country"
      var ds = new AntMovieCatalog();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceSearch = new List<string>();
      var wTableau = new ArrayList();
      bool GetItems = false;
      bool GetSubItems = true;
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798621)); // menu for random search
      dlg.Add(GUILocalizeStrings.Get(10798622)); // random search on all
      choiceSearch.Add("randomall");
      dlg.Add(GUILocalizeStrings.Get(10798664)); // random search on Genre
      choiceSearch.Add("Category");
      dlg.Add(GUILocalizeStrings.Get(10798665)); // random search on year
      choiceSearch.Add("Year");
      dlg.Add(GUILocalizeStrings.Get(10798663)); // random search on country
      choiceSearch.Add("Country");
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu)
          Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
        else
          Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];
      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (null == keyboard) return;
      keyboard.Reset();
      keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
      keyboard.Text = "";
      //            keyboard.DoModal(GetID);
      //            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
      switch (choiceSearch[dlg.SelectedLabel])
      {
        //case "randomall":
        //    break;

        default:
          var w_count = new ArrayList();
          if (dlg == null) return;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
          string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;
          DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
          wTableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
          w_count.Add(0);
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ToString().Contains(wproperty)))
            {
              if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
              {
                w_count[0] = (int)w_count[0] + 1;
                break;
              }
              else
              {
                //LogMyFilms.Debug("(Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                // column Name contains propertyname : added to w_tableau + w_count
                if (GetSubItems)
                {
                  LogMyFilms.Debug("SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                  foreach (string t in SubItemGrabbing(wsr[dc.ColumnName].ToString()))
                  {
                    LogMyFilms.Debug("SubItemGrabber: Output: " + t);
                    {
                      if (wTableau.Contains(t)) // search position in w_tableau for adding +1 to w_count
                      {
                        //if (!w_index.Contains(
                        for (int i = 0; i < wTableau.Count; i++)
                        {
                          if (wTableau[i].ToString() == t)
                          {
                            w_count[i] = (int)w_count[i] + 1;
                            //LogMyFilms.Debug("SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                            break;
                          }
                        }
                      }
                      else
                      // add to w_tableau and move 1 to w_count
                      {
                        LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
                        wTableau.Add(t);
                        w_count.Add(1);
                      }
                    }
                  }

                }
                if (GetItems)
                {
                  if (wTableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                  {
                    for (int i = 0; i < wTableau.Count; i++)
                    {
                      if (wTableau[i].ToString() == wsr[dc.ColumnName].ToString())
                      {
                        w_count[i] = (int)w_count[i] + 1;
                        //LogMyFilms.Debug("(Guzzi) Class already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                        break;
                      }
                    }
                  }
                  else
                  // add to w_tableau and move 1 to w_count
                  {
                    //LogMyFilms.Debug("AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
                    wTableau.Add(wsr[dc.ColumnName].ToString());
                    w_count.Add(1);
                  }
                }
              }
            }
          }
          if (wTableau.Count == 0)
          {
            LogMyFilms.Debug("PropertyClassCount is 0");
            break;
          }


          string wproperty2 = "";

          if (wproperty != "randomall")
          {
            dlg.Reset();
            dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
            choiceSearch.Clear();
            //w_tableau = Array.Sort(w_tableau);
            for (int i = 0; i < wTableau.Count; i++)
            {
              dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], wTableau[i]));
              choiceSearch.Add(wTableau[i].ToString());
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
              return;
            string t_wproperty2 = choiceSearch[dlg.SelectedLabel];
            wproperty2 = t_wproperty2;
          }
          else
            wproperty2 = "*";

          LogMyFilms.Debug("(RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
          switch (wproperty)
          {
            case "Rating":
              conf.StrSelect = wproperty + " = " + wproperty2;
              break;
            case "Number":
              conf.StrSelect = wproperty + " = " + wproperty2;
              break;
            default:
              if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                conf.StrSelect = wproperty + " like ''";
              else
                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
              break;
          }
          LogMyFilms.Debug("(RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " *" + wproperty2 + @"*";
          conf.StrTitleSelect = string.Empty;

          // we have: wproperty = selected category (randomall for all) and wproperty2 = value to search after

          var w_index = new ArrayList();
          int w_index_count = 0;
          string t_number_id = string.Empty;
          //Now build a list of valid movies in w_index with Number registered
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName == "Number"))
            {
              t_number_id = wsr[dc.ColumnName].ToString();
              //LogMyFilms.Debug("Movienumber stored as '" + t_number_id + "'");
            }
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              if ((wproperty == "randomall") && (dc.ColumnName.ToLower() == "translatedtitle"))
              {
                w_index.Add(t_number_id);
                LogMyFilms.Debug("(RamdomSearch - randomall!!!) - Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                w_index_count = w_index_count + 1;
              }
              else
                if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                {
                  if ((dc.ColumnName == wproperty) && (wsr[dc.ColumnName].ToString().Length == 0)) // column Name contains propertyname : add movie number (for later selection) to w_index
                  {
                    w_index.Add(t_number_id);
                    LogMyFilms.Debug("(RamdomSearch - (none)!!!) Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                    w_index_count = w_index_count + 1;
                  }
                }
                else
                {
                  //LogMyFilms.Debug("(searchmatches) - dc '" + dc.ToString() + "' - dc.ColumnName '" + dc.ColumnName.ToString() + "' - wproperty '" + wproperty + "' and Number(ID): '" + t_number_id + "'");
                  if (dc.ColumnName == wproperty)
                  {
                    //LogMyFilms.Debug("(searfhmatches with subitems) property2: '" + wproperty2 + "' - DB-Content: '" + wsr[dc.ColumnName].ToString() + "'"); 
                    if (wsr[dc.ColumnName].ToString().Contains(wproperty2)) // column Name contains propertyname : add movie number (for later selection) to w_index
                    {
                      w_index.Add(t_number_id);
                      LogMyFilms.Debug("(RamdomSearch - Standard) Counter '" + w_index_count + "' Added as '" + w_index[w_index_count] + "'");
                      w_index_count = w_index_count + 1;
                    }
                  }
                }
            }
          }
          // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
          LogMyFilms.Debug("(ResultBuildIndex) Found " + w_index.Count + " Records matching '" + wproperty2 + "' in '" + wproperty + "'");
          for (int i = 0; i < w_index.Count; i++)
            LogMyFilms.Debug("(ResultList) - Index: '" + i + "' - Number: '" + w_index[i] + "'");
          if (w_index.Count == 0)
          {
            GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0" + @"\n" + "Keine Filme in der Auswahl vorhanden"); // menu for random search
            return;
          }

          //Choose Random Movie from Resultlist
          var rnd = new Random();
          Int32 randomNumber = rnd.Next(w_index.Count + 1);
          LogMyFilms.Debug("RandomNumber: '" + randomNumber + "'");
          LogMyFilms.Debug("RandomTitle: '" + randomNumber + "'");

          //Set Filmlist to random Movie:
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;

          conf.StrSelect = "number = " + Convert.ToInt32(w_index[randomNumber]);
          conf.StrTxtSelect = "Selection number " + Convert.ToInt32(w_index[randomNumber]) + "";
          conf.StrTitleSelect = string.Empty;
          //GetSelectFromDivx(conf.StrSelect, wproperty, conf.wStrSortSens, keyboard.Text, true, "");
          LogMyFilms.Debug("(Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | wStrSort: " + conf.WStrSort);
          SetLabelView("search"); // show "search"
          GetFilmList(); // Added to update view ????

          //Set Context to first and only title in facadeview
          facadeFilms.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
          if (!facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
          // New Window for detailed selected item information
          {
            conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            GUITextureManager.CleanupThumbs();
            //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
          }
          else
          // View List as selected
          {
            conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
            Change_Layout_Action(conf.StrLayOut);
            conf.Boolreturn = !facadeFilms.SelectedListItem.IsFolder;
            do
            {
              if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
              conf.StrTitleSelect += conf.Wselectedlabel;
            } while (GetFilmList() == false); //keep calling while single folders found
          }

          //Before showing menu, first play the trailer
          //conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
          //Change_Layout_Action(MesFilms.conf.StrLayOut);
          //conf.Boolreturn = true;
          //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
          //conf.StrTitleSelect += conf.Wselectedlabel;
          //while (GetFilmList() == false) ; //keep calling while single folders found
          //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);

          while (dlg.SelectedLabel != -1)
          {
            dlg.Reset();
            choiceSearch.Clear();
            //dlg.SetHeading(string.Format(MesFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString())); // menu
            dlg.SetHeading(string.Format("Wählen Sie eine Aktion aus")); // menu
            //if (TrailerIsAvailable) // Später den Eintrag nur anzeigen, wenn auch ein Trailer verfügbar ist (siehe Klassen option)
            dlg.Add(string.Format("Hauptfilm spielen"));
            choiceSearch.Add("PlayMovie");
            dlg.Add(string.Format("Trailer spielen"));
            choiceSearch.Add("PlayMovieTrailer");
            dlg.Add(string.Format("Zeige Filmdetails")); //(goes to MesFilmsDetails ID7987 with selected record to show DetailScreen)
            choiceSearch.Add("ShowMovieDetails");
            dlg.Add(string.Format("Zeige alle Filme des gewählten Bereiches"));
            choiceSearch.Add("ShowMovieList");
            dlg.Add(string.Format("Neue Zufallssuche in gleicher Kategorie"));
            choiceSearch.Add("RepeatSearch");
            dlg.Add(string.Format("Neue globale Zufallssuche"));
            choiceSearch.Add("NewSearch");
            dlg.Add(string.Format("Zurück zum Hauptmenü", "Back"));
            choiceSearch.Add("Back");
            dlg.DoModal(GetID);
            if ((dlg.SelectedLabel == -1) || (dlg.SelectedLabel == 6))
              return;

            switch (choiceSearch[dlg.SelectedLabel])
            {
              case "PlayMovie":
                MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, null, MyFilmsDetail.PlayerOption.Internal);
                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                return;
              case "PlayMovieTrailer":
                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                //conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
                //Change_Layout_Action(MesFilms.conf.StrLayOut);
                //conf.Boolreturn = true;
                //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                //conf.StrTitleSelect += conf.Wselectedlabel;
                //while (GetFilmList() == false) ; //keep calling while single folders found

                MyFilmsDetail.trailerPlayed = true;
                MyFilmsDetail.Launch_Movie_Trailer(facadeFilms.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
                //// MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
                ////MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);    
                ////GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                //GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                //dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                //dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
                //dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
                //dlgYesNo.DoModal(GetID);
                //if (dlgYesNo.IsConfirmed)
                //  MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation);
                ////MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                break;
              case "ShowMovieDetails":
                // New Window for detailed selected item information
                conf.StrIndex = facadeFilms.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                conf.StrTIndex = facadeFilms.SelectedListItem.Label;
                GUITextureManager.CleanupThumbs();
                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                return;

              case "ShowMovieList":
                //GetFilmList(); //Is this necessary????
                conf.StrIndex = facadeFilms.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                conf.StrTIndex = facadeFilms.SelectedListItem.Label;
                GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
                dlg.DeInit();
                return;

              case "RepeatSearch":
                randomNumber = rnd.Next(w_index.Count + 1);
                LogMyFilms.Debug("RandomNumber: '" + randomNumber + "'");
                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);

                // if (GUIUtils.ShowYesNoDialog("Wollen Sie den Hauptfilm sehen?", GUILocalizeStrings.Get(219) + "\n" + "Zufällige Film ID = '" + w_index[randomNumber] + "'"))
                var dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
                dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + w_index[randomNumber] + "'");
                dlg1YesNo.DoModal(GetID);
                if (dlg1YesNo.IsConfirmed)                  //Launch_Movie(select_item, GetID, m_SearchAnimation);
                  MyFilmsDetail.Launch_Movie(facadeFilms.SelectedListItem.ItemId, GetID, null, MyFilmsDetail.PlayerOption.Internal);
                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                break;
              case "NewSearch":
                GUIUtils.ShowErrorDialog("Not yet implemented - be patient ....");
                SearchMoviesbyRandomWithTrailer(false);
                return;

              case "Back":
                return;
            }
          }
          break;
      }
      LogMyFilms.Debug("(SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
    }

    private void SearchMoviesbyAreas(bool returnToContextmenu)
    {
      // first select the area where to make random search on - "all", "category", "year", "country"
      AntMovieCatalog ds = new AntMovieCatalog();
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      List<string> choiceSearch = new List<string>();
      ArrayList wTableau = new ArrayList();
      bool getItems = false;
      bool getSubItems = true;
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798645)); // menu for random search
      dlg.Add(GUILocalizeStrings.Get(10798622)); // random search on all
      choiceSearch.Add("randomall");
      dlg.Add(GUILocalizeStrings.Get(10798664)); // random search on Genre
      choiceSearch.Add("Category");
      dlg.Add(GUILocalizeStrings.Get(10798665)); // random search on year
      choiceSearch.Add("Year");
      dlg.Add(GUILocalizeStrings.Get(10798663)); // random search on country
      choiceSearch.Add("Country");
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu)
          Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
        else
          Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];
      switch (choiceSearch[dlg.SelectedLabel])
      {
        case "randomall":
          SaveListState(false);
          conf.StrSelect = string.Empty;
          conf.StrTxtSelect = " " + GUILocalizeStrings.Get(10798621); // search random films
          conf.StrTitleSelect = string.Empty;
          SetLabelView("search"); // show "search"
          GetFilmList();
          break;

        default:
          var wCount = new ArrayList();
          if (dlg == null) return;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
          //Modified to checked for GlobalFilterString
          string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;
          DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
          //DataColumn[] wc = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
          wTableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
          wCount.Add(0);
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ToString().Contains(wproperty)))
            {
              if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
              {
                wCount[0] = (int)wCount[0] + 1;
                break;
              }
              else
              {
                //LogMyFilms.Debug("(Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                // column Name contains propertyname : added to w_tableau + w_count
                if (getSubItems)
                {
                  LogMyFilms.Debug("SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                  foreach (string t in SubItemGrabbing(wsr[dc.ColumnName].ToString()))
                  {
                    LogMyFilms.Debug("SubItemGrabber: Output: " + t);
                    {
                      if (wTableau.Contains(t)) // search position in w_tableau for adding +1 to w_count
                      {
                        //if (!w_index.Contains(
                        for (int i = 0; i < wTableau.Count; i++)
                        {
                          if (wTableau[i].ToString() == t)
                          {
                            wCount[i] = (int)wCount[i] + 1;
                            //LogMyFilms.Debug("SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                            break;
                          }
                        }
                      }
                      else
                      // add to w_tableau and move 1 to w_count
                      {
                        LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
                        wTableau.Add(t);
                        wCount.Add(1);
                      }
                    }
                  }

                }
                if (getItems)
                {
                  if (wTableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                  {
                    for (int i = 0; i < wTableau.Count; i++)
                    {
                      if (wTableau[i].ToString() == wsr[dc.ColumnName].ToString())
                      {
                        wCount[i] = (int)wCount[i] + 1;
                        //LogMyFilms.Debug("(Guzzi) Clas already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                        break;
                      }
                    }
                  }
                  else
                  // add to w_tableau and move 1 to w_count
                  {
                    //LogMyFilms.Debug("(Guzzi) AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
                    wTableau.Add(wsr[dc.ColumnName].ToString());
                    wCount.Add(1);
                  }
                }
              }
            }
          }
          if (wTableau.Count == 0)
          {
            LogMyFilms.Debug("PropertyClassCount is 0");
            break;
          }

          string wproperty2 = "";

          if (wproperty != "randomall")
          {
            dlg.Reset();
            dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
            choiceSearch.Clear();
            //w_tableau = Array.Sort(w_tableau);
            for (int i = 0; i < wTableau.Count; i++)
            {
              dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), wCount[i], wTableau[i]));
              choiceSearch.Add(wTableau[i].ToString());
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
              return;
            string t_wproperty2 = choiceSearch[dlg.SelectedLabel];
            wproperty2 = t_wproperty2;
          }
          else
            wproperty2 = "*";

          LogMyFilms.Debug("(RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
          SaveListState(false);
          switch (wproperty)
          {
            case "Rating":
            case "Number":
              conf.StrSelect = wproperty + " = " + wproperty2;
              conf.StrSelect = wproperty + " = " + wproperty2;
              break;
            default:
              if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                conf.StrSelect = wproperty + " is NULL";
              else
                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
              break;
          }
          LogMyFilms.Debug("(RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " *" + wproperty2 + @"*"; // selection ...

          conf.StrTitleSelect = string.Empty;

          SetLabelView("search"); // show "search"
          GetFilmList();
          break;
      }
    }

    //*****************************************************************************************
    //*  Global search persons
    //*****************************************************************************************
    private void SearchMoviesbyPersons(bool returnToContextmenu, string searchExpression, string searchField)
    {
      LogMyFilms.Debug("SearchMoviesbyPersons() - started");
      string wperson = "";
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      var choiceSearch = new List<string>();

      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798608)); //Search global persons by role

      dlg.Add("<" + GUILocalizeStrings.Get(10798633) + ">"); // New Search
      choiceSearch.Add("");

      if (SearchHistory.Count > 0)
      {
        for (int i = SearchHistory.Count; i > 0; i--)
        {
          dlg.Add(SearchHistory[i - 1]);
          choiceSearch.Add(SearchHistory[i - 1]);
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu) Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
        else Change_Search_Options();
        return;
      }

      if (null == keyboard) return;
      keyboard.Reset();
      keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
      keyboard.Text = choiceSearch[dlg.SelectedLabel];
      keyboard.DoModal(GetID);
      if (!keyboard.IsConfirmed || keyboard.Text.Length == 0) return;

      UpdateRecentSearch(keyboard.Text, MaxSearchHistoryLength);
      if (control_searchText(keyboard.Text))
        wperson = keyboard.Text;
      else
        return;

      #region function selection (actor, director, producer)

      string[] roles = { "Producer", "Director", "Writer", "Actors", "Persons" };
      dlg.Reset();
      choiceSearch.Clear();
      dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson);

      LogMyFilms.Debug("Adding search results for roles to menu ...");
      foreach (string role in roles)
      {
        DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, role + " like '*" + wperson + "*'", conf.StrSorta, conf.StrSortSens, false);
        if (wr.Length > 0)
        {
          int personCount = CountPersonsFound(wr, role, wperson);
          dlg.Add(BaseMesFilms.TranslateColumn(role) + " - " + personCount + " in " + wr.Length + " movie(s)");
          choiceSearch.Add(role);
        }
      }
      if (choiceSearch.Count == 0)
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(1079867), GUILocalizeStrings.Get(10798640));
        return;
      }
      #endregion

      dlg.DoModal(GetID);

      if (dlg.SelectedLabel == -1) return;

      SaveListState(false);

      conf.Boolselect = true;
      conf.CurrentView = "PersonsSearch";
      conf.StrSelect = ""; // reset view filter
      conf.StrViewSelect = "";
      conf.StrPersons = ""; // reset person filter
      conf.IndexedChars = 0;
      conf.Boolindexed = false;
      conf.BoolSkipViewState = false;


      conf.StrSelect = choiceSearch[dlg.SelectedLabel] + " like '*" + wperson + "*'";


      conf.StrTitleSelect = "";
      conf.ViewContext = ViewContext.Person;
      conf.WStrSort = choiceSearch[dlg.SelectedLabel];
      conf.WStrSortSens = " ASC";
      conf.Wselectedlabel = "";
      conf.StrPersons = ""; // conf.StrPersons = wperson;
      SetLabelView("search"); // show "search" // SetLabelView(persontype.ToLower());
      // this.Change_View_Action(persontype);
      GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, wperson, true, ""); // GetSelectFromDivx(conf.StrSelect, conf.wStrSort, conf.wStrSortSens, conf.StrPersons, true, string.Empty);
    }

    private int CountPersonsFound(DataRow[] wr, string wStrSort, string searchexpression)
    {
      int count = 0;
      var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //List<string> itemList = new List<string>();

      bool issplitfield = IsSplittableField(wStrSort);
      bool dontsplitvalues = conf.BoolDontSplitValuesInViews;
      bool showEmptyValues = conf.BoolShowEmptyValuesInViews;

      watch.Reset(); watch.Start();
      foreach (string champselect in wr.Select(row => row[wStrSort].ToString().Trim()))
      {
        if (issplitfield && !dontsplitvalues)
        {
          List<string> wtab = Search_String(champselect, false).ToList();
          if (wtab.Any()) // itemList.AddRange(wtab);
          {
            count += wtab.Count(t => set.Add(t));
          }
          else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
        else
        {
          if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
      }
      LogMyFilms.Debug("CountViewItems - Finished Count ('" + count + "') (" + (watch.ElapsedMilliseconds) + " ms)  - Read View Names for '" + wStrSort + "' finished (splittable items = '" + issplitfield + "', dontsplitvalues = '" + dontsplitvalues + "')");
      count = set.Count(t => t.IndexOf(searchexpression, StringComparison.OrdinalIgnoreCase) >= 0); // count = set.Count(t => t.ToUpperInvariant().Contains(searchexpression.ToUpperInvariant()));
      watch.Stop();
      LogMyFilms.Debug("CountViewItems - '" + count + "' Items found for SearchExpression '" + searchexpression + "' (" + (watch.ElapsedMilliseconds) + " ms)");
      return count;
    }

    //*****************************************************************************************
    //*  Global search movies by properties     (Guzzi Version)                               *
    //*****************************************************************************************
    private void SearchMoviesbyProperties(bool returnToContextmenu, string searchExpression, string searchField)
    {
      LogMyFilms.Debug("SearchMoviesbyProperties() - started - returnToContextmenu = '" + returnToContextmenu + "', searchExpression = '" + (searchExpression ?? "") + "'");
      var ds = new AntMovieCatalog();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      var choiceSearch = new List<string>();
      var wTableau = new List<string>();
      string searchstring = string.Empty;
      string wproperty = "all";

      if (string.IsNullOrEmpty(searchExpression)) // if no search startparameter is set ...
      {
        #region first select the function or property to be searching on
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // global search (films)

        if (SearchHistory.Count > 0)
        {
          dlg.Add(GUILocalizeStrings.Get(10798609));//last recent searches
          choiceSearch.Add("recentsearch");
        }

        dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
        choiceSearch.Add("all");

        dlg.Add(BaseMesFilms.TranslateColumn(conf.StrTitle1));
        choiceSearch.Add(conf.StrTitle1);

        if (Helper.FieldIsSet(conf.StrTitle2))
        {
          dlg.Add(BaseMesFilms.TranslateColumn(conf.StrTitle2));
          choiceSearch.Add(conf.StrTitle2);
        }

        dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
        choiceSearch.Add("showall");

        dlg.DoModal(GetID);
        
        if (dlg.SelectedLabel == -1)
        {
          if (returnToContextmenu) Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
          else Change_Search_Options();
          return;
        }

        // show all search fields, if selected ...
        if (choiceSearch[dlg.SelectedLabel] == "showall")
        {
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu
          choiceSearch.Clear();
          var displayItems = GetDisplayItems("view");
          foreach (string[] displayItem in displayItems)
          {
            string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
            dlg.Add(entry);
            choiceSearch.Add(displayItem[0]);
            LogMyFilms.Debug("Search Menu - add '{0}' as '{1}'", displayItem[0], entry);
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
          {
            if (returnToContextmenu)
              Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
            else
              Change_Search_Options();
            return;
          }
        }
        wproperty = choiceSearch[dlg.SelectedLabel];
        #endregion

        #region let user select the search option
        if (wproperty == "recentsearch") // if user choose recent search, set searchname = choice of user instead of asking via vkeyboard
        {
          var dlgrecent = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgrecent == null) return;
          dlgrecent.Reset();
          dlgrecent.SetHeading(GUILocalizeStrings.Get(10798609)); // Last Searches ...
          for (int i = SearchHistory.Count; i > 0; i--)
          {
            dlg.Add(SearchHistory[i - 1]);
          }
          dlgrecent.DoModal(GetID);
          if (dlg.SelectedId == -1)
          {
            if (returnToContextmenu) Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
            else Change_Search_Options();
            return;
          }
          searchstring = dlgrecent.SelectedLabelText;
        }

        // load search string into keyboard
        if (keyboard == null) return;
        keyboard.Reset();
        keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
        keyboard.Text = searchstring;
        keyboard.DoModal(GetID);
        if (keyboard.IsConfirmed) searchstring = keyboard.Text;
        else
        {
          if (returnToContextmenu) Context_Menu_Movie(facadeFilms.SelectedListItem.ItemId);
          else Change_Search_Options();
          return;
        }
        #endregion
      }
      else
      {
        searchstring = searchExpression;
      }

      #region if all prerequisites are met, do the search itself
      if (!string.IsNullOrEmpty(searchstring))
      {
        new Thread(delegate()
        {
          {
            UpdateRecentSearch(searchstring, MaxSearchHistoryLength);
            var wCount = new List<int>();
            string globalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D;
            switch (wproperty)
            {
              case "all":
              case "recentsearch":

                #region all and recent search
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu

                #region do the search
                #region log details about search
                LogMyFilms.Debug("(GlobalSearchAll) - GlobalFilterString: '" + globalFilterString + "' (INACTIVE for SEARCH !!!)");
                LogMyFilms.Debug("(GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
                LogMyFilms.Debug("(GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
                LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
                LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
                LogMyFilms.Debug("(GlobalSearchAll) - wproperty         : '" + wproperty + "'");
                LogMyFilms.Debug("(GlobalSearchAll) - searchstring      : '" + searchstring + "'");
                #endregion
                MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
                DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                foreach (DataColumn dc in from wsr in wr from DataColumn dc in ds.Movie.Columns where wsr[dc.ColumnName].ToString().ToLower().Contains(searchstring.ToLower()) select dc)
                {
                  if (wTableau.Contains(dc.ColumnName)) // search position in w_tableau for adding +1 to w_count
                  {
                    for (int i = 0; i < wTableau.Count; i++)
                    {
                      if (wTableau[i] == dc.ColumnName)
                      {
                        wCount[i]++;
                        break;
                      }
                    }
                  }
                  else
                  // add to w_tableau and move 1 to w_count
                  {
                    wTableau.Add(dc.ColumnName);
                    wCount.Add(1);
                    //LogMyFilms.Debug("(GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                  }
                }
                MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
                #endregion

                LogMyFilms.Debug("(GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + wTableau.Count + "'");
                if (wTableau.Count == 0) // No Results found
                {
                  GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798625));
                  return;
                }
                dlg.Reset();
                dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), searchstring)); // menu & SearchString
                choiceSearch.Clear();
                var propertyList = new string[]
                      {
                        "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year",
                        "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked",
                        "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate",
                        "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Borrower"
                      };

                foreach (string property in propertyList)
                {
                  for (int i = 0; i < wTableau.Count; i++)
                  {
                    if (wTableau[i].Equals(property))
                    {
                      dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), wCount[i], BaseMesFilms.TranslateColumn(property)));
                      choiceSearch.Add(wTableau[i]);
                    }
                  }
                }

                dlg.DoModal(GetID);
                if (dlg.SelectedLabel == -1) return;
                wproperty = choiceSearch[dlg.SelectedLabel];

                if (control_searchText(searchstring))
                {
                  SaveListState(false);

                  conf.Boolselect = false; // required to get film list results
                  conf.StrSelect = Equalexpression(wproperty, searchstring);
                  conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " *" + searchstring + @"*";
                  conf.StrTitleSelect = string.Empty;
                  SetLabelView("search"); // show "search"
                  GetFilmList();
                }
                break;

                #endregion

              case "all-Zebons":

                #region All-Zebons

                //ArrayList w_count = new ArrayList();
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                DataRow[] wrz = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                foreach (string wsearch in from wsr in wrz from string wsearch in GetDisplayItems("view") where System.Web.HttpUtility.HtmlDecode(HTMLParser.removeHtml(wsr[wsearch].ToString().ToLower())).Contains(keyboard.Text.ToLower()) select wsearch)
                {
                  if (wTableau.Contains(wsearch)) // search position in w_tableau for adding +1 to w_count
                  {
                    for (int i = 0; i < wTableau.Count; i++)
                    {
                      if (wTableau[i] == wsearch)
                      {
                        wCount[i] = (int)wCount[i] + 1;
                        break;
                      }
                    }
                  }
                  else
                  // add to w_tableau and move 1 to w_count
                  {
                    wTableau.Add(wsearch);
                    wCount.Add(1);
                  }
                }
                if (wTableau.Count == 0)
                {
                  GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798613), GUILocalizeStrings.Get(10798640) + @"\n" + searchstring);
                  return;
                }
                dlg.Reset();
                dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), searchstring)); // menu
                choiceSearch.Clear();
                for (int i = 0; i < wTableau.Count; i++)
                {
                  dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), wCount[i], BaseMesFilms.TranslateColumn(wTableau[i])));
                  choiceSearch.Add(wTableau[i]);
                }
                dlg.DoModal(GetID);
                if (dlg.SelectedLabel == -1) return;
                wproperty = choiceSearch[dlg.SelectedLabel];
                if (control_searchText(searchstring))
                {
                  SaveListState(false);
                  if (ds.Movie.Columns[wproperty].DataType.Name == "string")
                  {
                    conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
                    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " *" + choiceSearch[dlg.SelectedLabel] + @"*";
                  }
                  else
                  {
                    conf.StrSelect = wproperty + " = '" + searchstring + "'";
                    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " " + searchstring + @"";
                  }
                  conf.StrTitleSelect = string.Empty;
                  conf.Boolselect = false; // required to get film list results
                  SetLabelView("search"); // show "search"
                  GetFilmList();
                }
                break;
                #endregion

              default:

                #region default search handling
                LogMyFilms.Debug("(GlobalSearchAll) - ChosenProperty: wproperty  is '" + wproperty + "'");
                LogMyFilms.Debug("(GlobalSearchAll) - ChosenProperty: SearchTest is '" + searchstring + "'");
                if (control_searchText(searchstring))
                {
                  MyFilmsDetail.SetProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
                  DataRow[] wdr = BaseMesFilms.ReadDataMovies(globalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                  LogMyFilms.Debug("(GlobalSearchAll) - GlobalFilterString: '" + globalFilterString + "' (INACTIVE for SEARCH !!!)");
                  LogMyFilms.Debug("(GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
                  LogMyFilms.Debug("(GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
                  LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
                  LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
                  LogMyFilms.Debug("(GlobalSearchAll) - searchStringKBD   : '" + searchstring + "'");
                  foreach (DataColumn dc in wdr.SelectMany(wsr => (from DataColumn dc in ds.Movie.Columns where dc.ColumnName == wproperty where wsr[dc.ColumnName].ToString().ToLower().Contains(searchstring.ToLower()) select dc)))
                  {
                    if (wTableau.Contains(dc.ColumnName)) // search position in w_tableau for adding +1 to w_count
                    {
                      for (int i = 0; i < wTableau.Count; i++)
                      {
                        if (wTableau[i] == dc.ColumnName)
                        {
                          wCount[i]++;
                          break;
                        }
                      }
                    }
                    else
                    {
                      wTableau.Add(dc.ColumnName);
                      wCount.Add(1);
                    }
                  }
                  MyFilmsDetail.SetProcessAnimationStatus(false, m_SearchAnimation);

                  LogMyFilms.Debug("(GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + wTableau.Count + "'");
                  if (wTableau.Count == 0) // No Results found
                  {
                    GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798625));
                    return;
                  }

                  SaveListState(false);

                  switch (wproperty)
                  {
                    case "Rating":
                      //conf.StrSelect = wproperty + " = " + keyboard.Text; // Zebons version
                      conf.StrSelect = wproperty + " = " + Convert.ToInt32(searchstring);
                      break;
                    case "Number":
                    case "AgeAdded":
                      //conf.StrSelect = wproperty + " = " + keyboard.Text; // Zebons Version
                      conf.StrSelect = wproperty + " = " + Convert.ToInt32(searchstring);
                      break;
                    default:
                      conf.StrSelect = wproperty + " like '*" + searchstring + "*'";
                      break;
                  }

                  conf.Boolselect = false; // required to get film list results
                  conf.StrTxtSelect = dlg.SelectedLabelText + " *" + searchstring + @"*"; // conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]"; // Zebons Version
                  conf.StrTitleSelect = "";
                  SetLabelView("search");
                  GetFilmList();
                }
                break;
                #endregion
            }
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // after thread finished ...
            }
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsSearchMoviesByProperties", IsBackground = true }.Start();
      }
      #endregion
    }

    //*****************************************************************************************
    //*  Global search movies with missing/empty properties                                   *
    //*****************************************************************************************
    private void SearchIncompleteMovies()
    {
      var ds = new AntMovieCatalog();
      var choiceSearch = new List<string>();
      var wTableau = new List<string>();
      var wCount = new List<int>();
      string wproperty = string.Empty;
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798717)); // incomplete movie data
      DataRow[] dataRows = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
      foreach (DataColumn dc in dataRows.SelectMany(dataRow => ds.Movie.Columns.Cast<DataColumn>().Where(dc => string.IsNullOrEmpty(dataRow[dc.ColumnName].ToString()))))
      {
        if (wTableau.Contains(dc.ColumnName))
        {
          for (int i = 0; i < wTableau.Count; i++) // search position in w_tableau for adding +1 to w_count
          {
            if (wTableau[i] == dc.ColumnName)
            {
              wCount[i]++;
              break;
            }
          }
        }
        else // new item - add it to w_tableau and with count 1 to w_count
        {
          wTableau.Add(dc.ColumnName);
          wCount.Add(1);
        }
      }
      LogMyFilms.Debug("SearchIncompleteMovies() - Result of Search in all properties (w_tableau.Count): '" + wTableau.Count + "'");
      if (wTableau.Count == 0) // no results found
      {
        GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10798625));  // no result found for searched items
        return;
      }
      dlg.Reset();
      dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798717))); // incomplete movie data
      choiceSearch.Clear();
      var propertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Edition", "Description", "Comments", "Certification", "TagLine", "Actors", "Director", "Producer", "Writer", "Year", "Date", "Category", "Country", "Studio", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "AudioChannelCount", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "IMDB_Id", "IMDB_Rank", "TMDB_Id", "Picture", "Fanart" };
      foreach (string property in propertyList)
      {
        for (int i = 0; i < wTableau.Count; i++)
        {
          //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
          if (wTableau[i].Equals(property))
          {
            dlg.Add(string.Format(GUILocalizeStrings.Get(10798718), wCount[i], BaseMesFilms.TranslateColumn(property)));
            choiceSearch.Add(wTableau[i]);
          }
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;
      wproperty = choiceSearch[dlg.SelectedLabel];
      LogMyFilms.Debug("SearchIncompleteMovies() - ChosenProperty is '" + wproperty + "'");

      SaveListState(false);
      conf.ViewContext = ViewContext.Movie;
      conf.StrSelect = conf.StrTitleSelect = conf.StrFilmSelect = conf.StrTxtSelect = ""; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;

      conf.StrSelect = "(" + wproperty + " is NULL OR " + wproperty + " like '')";
      // conf.StrTxtSelect = "Selection " + wproperty + " [*empty*]";
      conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.TranslateColumn(wproperty) + " " + EmptyFacadeValue + "";
      conf.StrTitleSelect = string.Empty;
      SetLabelView("search"); // show "search"
      GetFilmList();
    }

    //*****************************************************************************************
    //*  Update recent used search terms
    //*****************************************************************************************
    private void UpdateRecentSearch(string newsearchstring, int maxItems)
    {
      if (SearchHistory.Contains(newsearchstring.Trim()))
        SearchHistory.Remove(newsearchstring.Trim());
      SearchHistory.Add(newsearchstring.Trim());

      while (SearchHistory.Count > maxItems)
      {
        LogMyFilms.Debug("UpdateRecentSearch: Item limit is: '" + maxItems + "', remove '" + SearchHistory[0] + "' from list");
        SearchHistory.RemoveAt(0);
      }

      string his = "";
      foreach (string s in SearchHistory)
      {
        if (String.IsNullOrEmpty(his))
          his = s;
        else
          his += "|" + s;
      }
      conf.StrSearchHistory = his;

      XmlConfig xmlConfig = new XmlConfig();
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "SearchHistory", conf.StrSearchHistory);
    }

    internal static void UpdateUseritemWithValue(string strUserItemSelection, string wproperty)
    {
      switch (strUserItemSelection)
      {
        case "useritem1":
          conf.Stritem1 = wproperty;
          conf.Strlabel1 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem1 + "', Label: '" + conf.Strlabel1 + "'.");
          break;
        case "useritem2":
          conf.Stritem2 = wproperty;
          conf.Strlabel2 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem2 + "', Label: '" + conf.Strlabel2 + "'.");
          break;
        case "useritem3":
          conf.Stritem3 = wproperty;
          conf.Strlabel3 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem3 + "', Label: '" + conf.Strlabel3 + "'.");
          break;
        case "useritem4":
          conf.Stritem4 = wproperty;
          conf.Strlabel4 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem4 + "', Label: '" + conf.Strlabel4 + "'.");
          break;
        case "useritem5":
          conf.Stritem5 = wproperty;
          conf.Strlabel5 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem5 + "', Label: '" + conf.Strlabel5 + "'.");
          break;
        case "useritemdetails1":
          conf.StritemDetails1 = wproperty;
          conf.StrlabelDetails1 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails1 + "', Label: '" + conf.StrlabelDetails1 + "'.");
          break;
        case "useritemdetails2":
          conf.StritemDetails2 = wproperty;
          conf.StrlabelDetails2 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails2 + "', Label: '" + conf.StrlabelDetails2 + "'.");
          break;
        case "useritemdetails3":
          conf.StritemDetails3 = wproperty;
          conf.StrlabelDetails3 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails3 + "', Label: '" + conf.StrlabelDetails3 + "'.");
          break;
        case "useritemdetails4":
          conf.StritemDetails4 = wproperty;
          conf.StrlabelDetails4 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails4 + "', Label: '" + conf.StrlabelDetails4 + "'.");
          break;
        case "useritemdetails5":
          conf.StritemDetails5 = wproperty;
          conf.StrlabelDetails5 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails5 + "', Label: '" + conf.StrlabelDetails5 + "'.");
          break;
        case "useritemdetails6":
          conf.StritemDetails6 = wproperty;
          conf.StrlabelDetails6 = BaseMesFilms.TranslateColumn(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails6 + "', Label: '" + conf.StrlabelDetails6 + "'.");
          break;
        case "mastertitle":
          conf.StrTitle1 = wproperty;
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StrTitle1 + "'.");
          break;
        case "secondarytitle":
          conf.StrTitle2 = wproperty;
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StrTitle2 + "'.");
          break;
        case "sorttitle":
          conf.StrSTitle = wproperty;
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StrSTitle + "'.");
          break;
      }
    }

    //*****************************************************************************************
    //*  Update userdefined mappings
    //*****************************************************************************************
    internal static void UpdateUserItems()
    {
      using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)) // Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)
      {
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem1", conf.Stritem1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel1", conf.Strlabel1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem2", conf.Stritem2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel2", conf.Strlabel2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem3", conf.Stritem3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel3", conf.Strlabel3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem4", conf.Stritem4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel4", conf.Strlabel4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem5", conf.Stritem5);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel5", conf.Strlabel5);

        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails1", conf.StritemDetails1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails1", conf.StrlabelDetails1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails2", conf.StritemDetails2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails2", conf.StrlabelDetails2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails3", conf.StritemDetails3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails3", conf.StrlabelDetails3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails4", conf.StritemDetails4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails4", conf.StrlabelDetails4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails5", conf.StritemDetails5);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails5", conf.StrlabelDetails5);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails6", conf.StritemDetails6);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails6", conf.StrlabelDetails6);

        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntTitle1", conf.StrTitle1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntTitle2", conf.StrTitle2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntSTitle", conf.StrSTitle);
      }
      //XmlConfig.SafeDispose();
    }

    //*****************************************************************************************
    //*  Update userdefined views
    //*****************************************************************************************
    private void SaveCustomViews()
    {
      using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)) // Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)
      {
        int iViewsCount = conf.CustomViews.View.Count;
        LogMyFilms.Debug("SaveCustomViews() - Current Total Views: '" + iViewsCount + "'");
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntViewTotalCount", iViewsCount);
        int index = 1;
        foreach (MFview.ViewRow viewRow in conf.CustomViews.View)
        {
          LogMyFilms.Debug("SaveCustomViews() - Saving view #'" + index + "' of " + iViewsCount + ", ViewLabel '" + viewRow.Label + "'");
          SaveView(xmlSettings, index, viewRow);
          index++;
        }
        for (int i = index; i < index + 2; i++)
        {
          LogMyFilms.Debug("SaveCustomViews() - Try removing view #'" + i + "'");
          RemoveView(xmlSettings, i);  // cleanup config file by removing unused view entries
        }
      }
      // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
    }

    private void RemoveView(XmlSettings xmlConfig, int index)
    {
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewItem{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewEnabled{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewImagePath{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewText{0}", index));
      // XmlConfig.RemoveEntry("MyFilms", Configuration.currentConfig, string.Format("AntViewLabel2{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewValue{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewFilter{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewIndex{0}", index));
      //XmlConfig.RemoveEntry("MyFilms", Configuration.currentConfig, string.Format("AntViewShowEmpty{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortFieldViewType{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortDirectionView{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewLayoutView{0}", index));
    }

    private void SaveView(XmlSettings xmlConfig, int index, MFview.ViewRow viewRow)
    {
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewText{0}", index), viewRow.Label);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewEnabled{0}", index), viewRow.ViewEnabled);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewImagePath{0}", index), viewRow.ImagePath);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewItem{0}", index), viewRow.DBfield);
      // XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, string.Format("AntViewLabel2{0}", index), viewRow.Label2);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewValue{0}", index), viewRow.Value);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewFilter{0}", index), viewRow.Filter);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewIndex{0}", index), viewRow.Index);
      //XmlConfig.WriteXmlConfig("MyFilms", Configuration.currentConfig, string.Format("AntViewShowEmpty{0}", index), viewRow.ShowEmpty);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortFieldViewType{0}", index), viewRow.SortFieldViewType);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortDirectionView{0}", index), viewRow.SortDirectionView);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewLayoutView{0}", index), GetLayoutFromName(viewRow.LayoutView));
    }

    private int GetLayoutFromName(string layoutname)
    {
      int wLayout = 0;
      if (layoutname == "List") wLayout = 0;
      if (layoutname == "Small Icons") wLayout = 1;
      if (layoutname == "Large Icons") wLayout = 2;
      if (layoutname == "Filmstrip") wLayout = 3;
      if (layoutname == "Cover Flow") wLayout = 4;
      return wLayout;
    }

    //*****************************************************************************************
    //*  No Movie found to display. Display all movies
    //*****************************************************************************************
    private static void DisplayAllMovies()
    {
      conf.StrFilmSelect = conf.StrTitle1 + " not like ''";
      conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;
      r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, true);
    }

    //*****************************************************************************************
    //*  Initialize Fields on Main screen                                                     *
    //*****************************************************************************************
    private void InitMainScreen(bool log)
    {
      LogMyFilms.Debug("(InitMainScreen) - Initialize all properties !!!");

      MyFilmsDetail.Init_Detailed_DB(log);  // Includes clear of db & user properties

      backdrop.Filename = String.Empty;
      menucover.Filename = String.Empty;
      filmcover.Filename = String.Empty;
      viewcover.Filename = String.Empty;
      personcover.Filename = String.Empty;
      groupcover.Filename = String.Empty;

      MyFilmsDetail.clearGUIProperty("logos_id2001", log);
      MyFilmsDetail.clearGUIProperty("logos_id2002", log);
      MyFilmsDetail.clearGUIProperty("logos_id2003", log);
      MyFilmsDetail.clearGUIProperty("logos_id2012", log); // Combined Logo
      MyFilmsDetail.clearGUIProperty("nbobjects.value", log);
      MyFilmsDetail.clearGUIProperty("Fanart", log);
      MyFilmsDetail.clearGUIProperty("Fanart2", log);
      MyFilmsDetail.clearGUIProperty("db.rating", log);
      MyFilmsDetail.clearGUIProperty("view", log); // Try to properly clean main view when entering
      MyFilmsDetail.clearGUIProperty("select", log);
      GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnViewAs, GUILocalizeStrings.Get(97) + "Home"); // "View by: " + <view>
      LogMyFilms.Debug("(InitMainScreen) - Initialize all properties - Finished !");
    }

    //*****************************************************************************************
    //*  Reset global Filters
    //*****************************************************************************************
    private void InitGlobalFilters(bool log)
    {
      LogMyFilms.Debug("(InitGlobalFilters) - Reset global Filters ...");
      GlobalFilterStringUnwatched = string.Empty;
      // Will be later initialized from setting MyFilms.conf.GlobalUnwatchedOnly
      MyFilmsDetail.clearGUIProperty("globalfilter.unwatched", log);
      if (!GlobalFilterTrailersOnly)
      {
        GlobalFilterStringTrailersOnly = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly", log);
      }

      if (!GlobalFilterIsOnlineOnly)
      {
        GlobalFilterStringIsOnline = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.isonline", log);
      }

      if (!GlobalFilterMinRating)
      {
        GlobalFilterStringMinRating = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.minrating", log);
        MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue", log);
      }

      GlobalFilterMovieFormat3D = GUILocalizeStrings.Get(10798973); // all
      GlobalFilterStringMovieFormat3D = string.Empty;
      MyFilmsDetail.clearGUIProperty("globalfilter.movieformat", log);
    }

    private void InitFolders()
    {
      // Check and create Group thumb folder ...
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsThumbsPath) + @"\MovieThumbs")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsThumbsPath) + @"\MovieThumbs");
    }

    private void InitializeBackgroundWorker()
    {
      if (bgUpdateDB == null) bgUpdateDB = new BackgroundWorker();
      bgUpdateDB.DoWork += new DoWorkEventHandler(bgUpdateDB_DoWork);
      bgUpdateDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateDB_RunWorkerCompleted);
      bgUpdateDB.WorkerSupportsCancellation = true;
      bgUpdateDB.WorkerReportsProgress = true;
      // bgUpdateDB.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bgUpdateDB_ProgressChanged);

      if (bgUpdateFanart == null) bgUpdateFanart = new System.ComponentModel.BackgroundWorker();
      bgUpdateFanart.DoWork += new DoWorkEventHandler(bgUpdateFanart_DoWork);
      bgUpdateFanart.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateFanart_RunWorkerCompleted);

      if (bgUpdateActors == null) bgUpdateActors = new System.ComponentModel.BackgroundWorker();
      bgUpdateActors.DoWork += new DoWorkEventHandler(bgUpdateActors_DoWork);
      bgUpdateActors.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateActors_RunWorkerCompleted);

      if (bgUpdateTrailer == null) bgUpdateTrailer = new System.ComponentModel.BackgroundWorker();
      bgUpdateTrailer.DoWork += new DoWorkEventHandler(bgUpdateTrailer_DoWork);
      bgUpdateTrailer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateTrailer_RunWorkerCompleted);

      if (bgLoadMovieList == null) bgLoadMovieList = new System.ComponentModel.BackgroundWorker();
      bgLoadMovieList.DoWork += new DoWorkEventHandler(bgLoadMovieList_DoWork);
      bgLoadMovieList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgLoadMovieList_RunWorkerCompleted);

      if (bgIsOnlineCheck == null) bgIsOnlineCheck = new System.ComponentModel.BackgroundWorker();
      bgIsOnlineCheck.DoWork += new DoWorkEventHandler(bgIsOnlineCheck_DoWork);
      bgIsOnlineCheck.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgIsOnlineCheck_RunWorkerCompleted);

      if (bgDownloadTrailer == null) bgDownloadTrailer = new System.ComponentModel.BackgroundWorker();
      bgDownloadTrailer.WorkerSupportsCancellation = true;
      bgDownloadTrailer.DoWork += new DoWorkEventHandler(bgDownloadTrailer_DoWork);
      bgDownloadTrailer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgDownloadTrailer_RunWorkerCompleted);

      for (int f = 0; f < maxThreads; f++)
      {
        threadArray[f] = new BackgroundWorker();
        threadArray[f].DoWork += new DoWorkEventHandler(threadArray_DoWork);
        threadArray[f].RunWorkerCompleted += new RunWorkerCompletedEventHandler(threadArray_RunWorkerCompleted);
        // threadArray[f].ProgressChanged += new ProgressChangedEventHandler(threadArray_ProgressChanged);
        threadArray[f].WorkerReportsProgress = false;
        threadArray[f].WorkerSupportsCancellation = true;
        threadDoneEventArray[f] = new AutoResetEvent(false); // to signal worker finished ...
      }
    }

    private void InitializeQueuedTrailerDownloader(int delayToStart)
    {
      new Thread(delegate()
      {
        {
          try
          {
            LogMyFilms.Info("InitializeQueuedTrailerDownloader() - waiting '{0}' seconds to try starting '{1}' trailer download threads for '{2}' queue entries", delayToStart, maxThreads, TrailertoDownloadQueue.Count);

            if (!Helper.IsConnectedToInternet())
            {
              LogMyFilms.Warn("InitializeQueuedTrailerDownloader() - No Internet connection available - aborting start of Trailer download threads!");
              return;
            }

            if (!conf.CacheOnlineTrailer)
            {
              LogMyFilms.Info("InitializeQueuedTrailerDownloader() - trailer caching is disabled - exiting !");
              return;
            }

            Thread.Sleep(delayToStart * 1000); // wait specified delay to let OV instance initialize

            int retry = 0;
            while (retry < 5)
            {
              if (conf == null)
              {
                LogMyFilms.Info("InitializeQueuedTrailerDownloader() - MyFilms config not yet loaded - waiting 10 seconds!");
                retry++;
                Thread.Sleep(10000);
              }
              else if (!OVplayer.IsOnlineVideosReady())
              {
                LogMyFilms.Info("InitializeQueuedTrailerDownloader() - retry '" + retry + "', OnlineVideos not yet loaded - waiting 10 seconds!");
                retry++;
                Thread.Sleep(10000);
              }
              else break;
            }

            if (conf == null)
            {
              LogMyFilms.Info("InitializeQueuedTrailerDownloader() - No MyFilms config loaded - exiting!");
              return;
            }

            // start the parallel downloader, of there is anything in queue
            if (TrailertoDownloadQueue.Count > 0)
            {
              // bgDownloadTrailer.RunWorkerAsync(); // single worker thread
              // start multiple worker threads
              for (int f = 0; f < maxThreads; f++)
              {
                LogMyFilms.Debug("InitializeQueuedTrailerDownloader() - starting trailer download thread '" + f + "'"); 
                threadArray[f].RunWorkerAsync(f);
              }
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("InitializeQueuedTrailerDownloader() - Error starting trailer download threads: " + ex.Message);
          }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // this after thread finished ...
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "InitializeQueuedTrailerDownloader", IsBackground = true }.Start();
    }

    private delegate void UpdateWatchTextDelegate(string newText);

    public static void UpdateWatchText(string newText)
    {
      MyFilmsDetail.setGUIProperty("statusmessage", newText, true);
    }

    private void InitAmcImporter(Configuration mfConf)
    {
      LogMyFilms.Debug("InitAmcImporter() - Starting initial import run in: {0} secs", mfConf.AMCUscanStartDelay);

      // do a local scan when starting up the app if enabled - later on the watcher will monitor changes            
      if (mfConf.AMCUscanOnStartup)
      {
        new Thread(delegate(object o)
        {
          try
          {
            var startdelay = (int)o;
            Thread.Sleep(startdelay * 1000);
            AsynUpdateDatabase("");
          }
          catch (Exception ex) { LogMyFilms.Warn("InitAmcImporter - Error: {0}", ex.ToString()); }
        }) { IsBackground = true, Name = "InitAmcImporter" }.Start(mfConf.AMCUscanStartDelay);
      }

      // Setup Disk Watcher (DeviceManager) and Folder/File Watcher
      if (mfConf.AMCUwatchScanFolders)
      {
        // DeviceManager.StartMonitor(); // we disable device monitoring for now
        SetUpFolderWatches(mfConf);
      }
    }

    private void SetUpFolderWatches(Configuration mfConf)
    {
      const int filescanintervalinminutes = 120;
      Regex splitRegex = new Regex(";");
      List<string> importFolders = new List<String>();
      XmlConfig amcXmlConfig = new XmlConfig();

      //// Go through all myfilms search folders, and add a watchfolder on it - we should NOT watch them for imports !
      //searchDir = splitRegex.Split(MyFilms.conf.StrDirStor); // this is the MyFilms search directories - we shouldn't watch them for AMCU importer !
      //importFolders.AddRange(searchDir.Select(path => path.LastIndexOf(@"\", StringComparison.Ordinal) != path.Length - 1 ? path + "\\" : path).Where(Directory.Exists));

      // Go through all AMCU import folders, and add a watchfolder on it
      string[] searchDir = splitRegex.Split(amcXmlConfig.ReadAMCUXmlConfig(mfConf.StrAMCUpd_cnf, "Movie_Scan_Path", ""));
      importFolders.AddRange(searchDir.Select(path => path.LastIndexOf(@"\", StringComparison.Ordinal) != path.Length - 1 ? path + "\\" : path).Where(Directory.Exists));

      LogMyFilms.Debug("setUpFolderWatches() - Found '{0}' folders to add to watcher", searchDir.Length);

      watcherUpdater = new Watcher(importFolders, filescanintervalinminutes);
      watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
      watcherUpdater.StartFolderWatch();
    }

    private void stopFolderWatches()
    {
      if (watcherUpdater == null) return; // in case the watcher feature was not enabled in setup, there was no instance created.

      watcherUpdater.StopFolderWatch();
      watcherUpdater.WatcherProgress -= new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
      watcherUpdater = null;
    }

    private void watcherUpdater_WatcherProgress(int nProgress, List<WatcherItem> modifiedFilesList)
    {
      var filesAdded = new List<PathPair>();
      var filesRemoved = new List<PathPair>();

      // go over the modified files list once in a while & update
      foreach (WatcherItem item in modifiedFilesList)
      {
        switch (item.m_type)
        {
          case WatcherItemType.Added:
            filesAdded.Add(new PathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
            break;

          case WatcherItemType.Deleted:
            filesRemoved.Add(new PathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
            break;
        }
      }

      // with out list of files, start the parsing process
      if (filesAdded.Count > 0)
      {
        GUIUtils.ShowNotifyDialog("MyFilms Media Watcher", "'" + filesAdded.Count + "' file(s) found !");
        LogMyFilms.Debug("watcherUpdater_WatcherProgress() - '" + filesAdded.Count + "' file(s) found !");
        // queue it
        //lock (m_parserUpdaterQueue)
        //{
        //  m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Add, filesAdded, false, false));
        //}
      }

      if (filesRemoved.Count > 0)
      {
        GUIUtils.ShowNotifyDialog("MyFilms Media Watcher", "'" + filesRemoved.Count + "' file(s) removed !");
        LogMyFilms.Debug("watcherUpdater_WatcherProgress() - '" + filesRemoved.Count + "' file(s) removed !");
        // queue it
        //lock (m_parserUpdaterQueue)
        //{
        //  m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Remove, filesRemoved, false, false));
        //}
      }

      if (filesAdded.Count > 0 || filesRemoved.Count > 0)
      {
        if (!bgUpdateDB.IsBusy && !bgUpdateDB.CancellationPending)
        {
          LogMyFilms.Debug("watcherUpdater_WatcherProgress() - file changes detected - launching AMCUpdater !");
          AsynUpdateDatabase("");
        }
        else
        {
          LogMyFilms.Debug("watcherUpdater_WatcherProgress() - AMCUpdater already running !");
        }
      }
    }

    private void InitFSwatcher()
    {
      if (FSwatcher.EnableRaisingEvents && FSwatcher.Filter == System.IO.Path.GetFileName(conf.StrFileXml))
        return; // return, if it's already enabled and DB name has not changed
      else
      {
        FSwatcher.EnableRaisingEvents = false;
        FSwatcher.Changed -= new FileSystemEventHandler(FSwatcherChanged);
        FSwatcher.Error -= new ErrorEventHandler(FSwatcherError);
        //FSwatcher.Created -= new FileSystemEventHandler(FSwatcherCreated);
        //FSwatcher.Deleted -= new FileSystemEventHandler(FSwatcherDeleted);
        //FSwatcher.Renamed -= new RenamedEventHandler(FSwatcherRenamed);
      }

      // Init FileSystem Watcher

      // ***** Change this as required
      string path = Path.GetDirectoryName(conf.StrFileXml);
      string filename = Path.GetFileName(conf.StrFileXml);

      FSwatcher.Path = path;
      FSwatcher.IncludeSubdirectories = false;
      FSwatcher.Filter = filename; // FSwatcher.Filter = "*.xml";
      FSwatcher.NotifyFilter = NotifyFilters.LastWrite; // FSwatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
      //FSwatcher.InternalBufferSize = 64;

      // Add event handlers: 1 for the event raised when a file is created, and 1 for when it detects an error.
      FSwatcher.Changed += new FileSystemEventHandler(FSwatcherChanged);
      FSwatcher.Error += new ErrorEventHandler(FSwatcherError);
      //FSwatcher.Created += new FileSystemEventHandler(FSwatcherCreated);
      //FSwatcher.Deleted += new FileSystemEventHandler(FSwatcherDeleted);
      //FSwatcher.Renamed += new RenamedEventHandler(FSwatcherRenamed);

      FSwatcher.EnableRaisingEvents = true; // Begin watching.
      LogMyFilms.Info("InitFSwatcher() - FSwatcher started watching - DB-file: '" + conf.StrFileXml + "'");
    }

    private void FSwatcherChanged(object source, FileSystemEventArgs e)
    {
      if (FSwatcher.EnableRaisingEvents == false) // ignore event, if notification is switched off
        return;
      FSwatcher.EnableRaisingEvents = false;
      LogMyFilms.Info("WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

      Thread.Sleep(3000);
      //FileInfo objFileInfo = new FileInfo(e.FullPath);
      //if (!objFileInfo.Exists) 
      //  return; // ignore the file changed event

      bool success = false; // result of try open for read
      const int maxretries = 10; // max retries 10 * 3000 = 30 seconds
      int i = 0;

      while (!success && i < maxretries)
      {
        success = !Helper.IsFileUsedbyAnotherProcess(conf.StrFileXml);
        i += 1;
        if (!success) LogMyFilms.Debug("FSwatcherChanged() - Attempt '" + i + "'to open Movie Database in read mode unsuccessful, waiting for next retry");
        Thread.Sleep(3000);
      }

      if (success)
      {
        if (GUIWindowManager.ActiveWindow != ID_MyFilms)
        {
          // load dataset
          BaseMesFilms.LoadMyFilms(conf.StrFileXml);
          // disabled - we do cleanup on init phase now - and there might be writings in progress by Trakt API !
          // MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // make sure, no global lock is left
          // (re)populate films
          r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
        }
        else
        {
          // save current position of the facade
          if (facadeFilms != null)
          {
            conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = facadeFilms.SelectedListItem.Label;
          }
          // alternatively RefreshFacade() to be called to also update facade (only when main window is active)
          Loadfacade(); // loading threaded : Fin_Charge_Init(false, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
        }

        // this.BeginInvoke(new UpdateWatchTextDelegate(UpdateWatchText), "WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

        try
        {
          if (FSwatcher.Path.Length > 0) FSwatcher.EnableRaisingEvents = true;
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug(ex, "FSwatcherChanged()- FSwatcher - problem enabling Raisingevents - Message: '" + ex.Message + "'");
        }

        if (SendTraktUpdateMessage)
        {
          if (ImportComplete != null && conf.AllowTraktSync) // trigger sync to trakt page after importer finished
          {
            ImportComplete();
            LogMyFilms.Debug("FSwatcherChanged(): Fired 'ImportCompleted' event to trigger sync to trakt page after reloading database content !");
          }
        }
        else
        {
          LogMyFilms.Debug("FSwatcherChanged(): No 'ImportCompleted' event sent (SendTraktUpdateMessage = '" + SendTraktUpdateMessage + "')");
          SendTraktUpdateMessage = true;
        }
      }
      else
      {
        LogMyFilms.Debug("FSwatcherChanged(): Reloading data not possible - cannot open file for reading ! No Update of dataset done.");
      }
    }

    private void FSwatcherCreated(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("WatcherCreated() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
    }

    private void FSwatcherDeleted(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("WatcherDeleted() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
    }

    private void FSwatcherRenamed(object source, FileSystemEventArgs e)
    {
      LogMyFilms.Debug("WatcherRenamed() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
    }

    private void FSwatcherError(object source, ErrorEventArgs e) // The error event handler
    {
      Exception watchException = e.GetException();
      LogMyFilms.Debug("WatcherError() - A FileSystemWatcher error has occurred: " + watchException.Message);
      // We need to create new version of the object because the old one is now corrupted
      FSwatcher = new FileSystemWatcher();
      while (!FSwatcher.EnableRaisingEvents)
      {
        try
        {
          InitFSwatcher(); // This will throw an error at the watcher.NotifyFilter line if it can't get the path.
          LogMyFilms.Debug("WatcherError() - FSwatcher restarted after error !");
        }
        catch
        {
          Thread.Sleep(5000); // Sleep for a bit; otherwise, it takes a bit of processor time
        }
      }
    }

    #region GetSet for Network and Powermode
    private static bool IsResumeFromStandby { get; set; }
    private static bool IsNetworkAvailable { get; set; }
    #endregion

    void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
    {
      LogMyFilms.Debug("MyFilms received power event {0}", e.Mode);
      switch (e.Mode)
      {
        case Microsoft.Win32.PowerModes.Resume:
        {
          LogMyFilms.Info("PowerModeChanged() - MyFilms is resuming from standby");
          IsResumeFromStandby = true;

          Thread.Sleep(250);

          const int maxretries = 10; // max retries 10 * 500 = 5 seconds
          int i = 0;
          bool success = false; // result of update operation

          while (!success && i < maxretries)
          {
            // first check, if the network is ready and DB is accessible
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && File.Exists(conf.StrFileXml))
            {
              LogMyFilms.Debug("PowerModeChanged() - MyFilms is reloading movie data to memory cache.");
              FSwatcher.EnableRaisingEvents = false;
              if (GUIWindowManager.ActiveWindow != ID_MyFilms)
              {
                // reload data, as it might have changed while MF/MP was sleeping
                BaseMesFilms.LoadMyFilms(conf.StrFileXml); // load dataset
                // disabled, we do it only on init phase!
                // MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // make sure, no global lock is left
                r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
                // (re)populate films
              }
              else
              {
                // loading threaded : Fin_Charge_Init(false, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
                Loadfacade(); 
              }
              LogMyFilms.Debug("PowerModeChanged() - MyFilms finished reloading movie data to memory cache.");

              LogMyFilms.Debug("PowerModeChanged() - restarting backgroundworker for trailer queue...");
              BaseMesFilms.RestartBackgroundWorker();
              success = true;
            }
            else
            {
              i += 1;
              LogMyFilms.Debug("PowerModeChanged() - Network not yet ready or DB file not accessible on try '" + i + " of " + maxretries + "' to reload - waiting for next retry");
              Thread.Sleep(2000);
            }
          }

          LogMyFilms.Debug("PowerModeChanged() - restarting DB file watcher to monitor for changes ...");
          try
          {
            FSwatcher.EnableRaisingEvents = true;
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("PowerModeChanged()- FSwatcher - problem enabling Raisingevents - Message:  '" + ex.Message);
          }

          try
          {
            if (conf != null && conf.AMCUwatchScanFolders)
            {
              LogMyFilms.Debug("PowerModeChanged() - starting folders watchers ...");
              // DeviceManager.StartMonitor(); // we disable device monitoring for now
              SetUpFolderWatches(conf);
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("PowerModeChanged() - Error when attempting to start folder watchers: " + ex.Message);
          }

          LogMyFilms.Debug("PowerModeChanged() - MyFilms finished Resuming.");
        }
          break;

        case Microsoft.Win32.PowerModes.Suspend:
          try
          {
            LogMyFilms.Debug("PowerModeChanged() - stopping folders watchers ...");
            stopFolderWatches();
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("PowerModeChanged() - Error when attempting to stop folder watchers: " + ex.Message);
          }

          LogMyFilms.Debug("PowerModeChanged() - Checking for ongoing DB update ...");
          if (BaseMesFilms.UpdateWorker != null && BaseMesFilms.UpdateWorker.IsBusy)
          {
            LogMyFilms.Debug("PowerModeChanged() - DB updates still active ! - waiting for background worker to complete ...");
            BaseMesFilms.UpdateWorkerDoneEvent.WaitOne(60000);
            LogMyFilms.Debug("PowerModeChanged() - DB updates in background worker thread finished");
            BaseMesFilms.UpdateWorkerDoneEvent.WaitOne(1000); // wait another second to finish log entries
          }

          try
          {
            LogMyFilms.Debug("PowerModeChanged() - stopping backgroundworker for trailer queue ...");
            BaseMesFilms.StopBackgroundWorker();
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("PowerModeChanged() - Error when attempting to stop backgroundworker for trailer queue: " + ex.Message);
          }
          LogMyFilms.Info("PowerModeChanged() - MyFilms is entering standby");
          break;
        default:
          LogMyFilms.Info("PowerModeChanged() - MyFilms detected unhandled PowerModeChanged event ('" + e.Mode + "') - no action.");
          break;
      }
    }

    void NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
    {
      if (e.IsAvailable)
      {
        LogMyFilms.Debug("MyFilms is connected to the network");
        IsNetworkAvailable = true;
      }
      else
      {
        LogMyFilms.Debug("MyFilms is disconnected from the network");
        // DBOnlineMirror.IsMirrorsAvailable = false; // Force to recheck later
        IsNetworkAvailable = false;
      }
    }

    private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
    {
      LogMyFilms.Debug("OnPlayBackEnded was initiated - filename: '" + filename + "'");
      OnPlayEndAction(type, 0, filename, true, false);
    }
    private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
    {
      LogMyFilms.Debug("OnPlayBackStopped was initiated - filename: '" + filename + "'");
      OnPlayEndAction(type, timeMovieStopped, filename, false, true);
    }
    private void OnPlayEndAction(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended, bool stopped)
    {
      if (currentTrailerMoviesList.Count > 0 && currentTrailerPlayingItem != null)
      {
        if (filename == currentTrailerPlayingItem.File && type == MediaPortal.Player.g_Player.MediaType.Video) // is it the last played trailer?
        {
          LogMyFilms.Debug("OnPlayEndAction is launching PlayRandomTrailer()");
          currentTrailerPlayingItem = null;
          PlayRandomTrailer(true); // show GUI dialog for user selection
        }
        else
        {
          // some other playback ended, and a playlist is still set here -> clear it
          currentTrailerMoviesList.Clear();
          currentTrailerPlayingItem = null;
        }
      }
      else
      {
        currentTrailerPlayingItem = null;
      }
    }


    //*****************************************************************************************
    //*  Update Database in batch mode                                                        *
    //*****************************************************************************************
    private void AsynUpdateDatabase(string config)
    {
      LogMyFilms.Info("AsynUpdateDatabase() - Launch global AMCU update with config = '" + config + "'");
      if (!bgUpdateDB.IsBusy && !bgUpdateDB.CancellationPending)
      {
        MyFilmsDetail.SetGlobalLock(true, conf.StrFileXml); // also disabled local FSwatcher
        bgUpdateDB.RunWorkerAsync(config); // bgUpdateDB.RunWorkerAsync(MyFilms.conf.StrTIndex);
        MyFilmsDetail.setGUIProperty("statusmessage", "global update active", false);
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail) GUIUtils.ShowNotifyDialog("Global Update started !");
        LogMyFilms.Info("AsynUpdateDatabase() - Launching AMCUpdater in batch mode");
      }
      else
      {
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail) GUIUtils.ShowNotifyDialog("Global Update already running !");
        LogMyFilms.Info("AsynUpdateDatabase() - AMCUpdater cannot be started in batch mode - either already running or cancellation pending");
      }
    }

    void bgUpdateDB_DoWork(object sender, DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      // MyFilmsDetail.RunAMCupdater(Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe", "\"" + MyFilms.conf.StrAMCUpd_cnf + "\" \"" + MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Log) + "\""); // Add Logpath to commandlineparameters
      string exeName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe";
      string amcConfig = (string.IsNullOrEmpty(e.Argument.ToString()))
                   ? "\"" + conf.StrAMCUpd_cnf + "\""
                   : "\"" + e.Argument + "\"";
      string argsLine = amcConfig + " " + "\"" + Config.GetDirectoryInfo(Config.Dir.Log) + "\"";
      //static public void RunAMCupdater(string exeName, string argsLine)
      if (exeName.Length > 0)
      {
        using (var p = new Process())
        {
          var psi = new ProcessStartInfo();
          psi.FileName = exeName;
          // psi.UseShellExecute = true;
          psi.UseShellExecute = false; // try to avoid UAC popup ...
          //psi.WindowStyle = ProcessWindowStyle.Minimized;
          psi.WindowStyle = ProcessWindowStyle.Hidden;
          psi.Arguments = argsLine;
          psi.ErrorDialog = false;
          psi.CreateNoWindow = true;
          // psi.RedirectStandardOutput = true; // redirect output to streamreader - ToDo: add reader to use and add console output to AMCU !
          // psi.RedirectStandardError = true;

          if (OSInfo.OSInfo.VistaOrLater())
          {
            psi.Verb = "runas";
          }

          p.StartInfo = psi;
          LogMyFilms.Debug("RunAMCupdater - Starting external command: {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);

          try
          {
            p.Start();
            while (!p.HasExited)
            {
              if (bgUpdateDB.CancellationPending)
              {
                LogMyFilms.Debug("RunAMCupdater - Cancellation requested by user - try to kill process 'AMCupdater.exe' !");
                e.Cancel = true;
                p.Kill();
                p.WaitForExit();
                //Process[] aProc = System.Diagnostics.Process.GetProcessesByName("AMCupdater");
                //if (aProc.Length > 0) {  Process myprc = aProc[0]; myprc.Kill(); }
              }
              Thread.Sleep(1000);
              //string output = p.StandardOutput.ReadToEnd();
              //string error = p.StandardError.ReadToEnd();
            }
          }
          catch (System.ComponentModel.Win32Exception)
          {
            // Person denied UAC escallation
            LogMyFilms.Debug("RunAMCupdater - UAC was denied ! - end");
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug(ex.Message);
          }
          LogMyFilms.Debug("RunAMCupdater - External command finished");
          //string output = p.StandardOutput.ReadToEnd();
          //string error = p.StandardError.ReadToEnd();
        }
      }
    }

    void bgUpdateDB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Cancelled)
      {
        LogMyFilms.Info("RunAMCupdater - Update database with AMCUpdater cancelled by user request. (GetID = '" + GetID + "')");
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
        {
          GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(1079856));
          // ShowMessageDialog(GUILocalizeStrings.Get(1079861), "", GUILocalizeStrings.Get(1079856)); // Global Update was cancelled !
        }
      }
      else
      {
        LogMyFilms.Info("RunAMCupdater - Update database with AMCUpdater sucessfully finished. (GetID = '" + GetID + "')");
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
        {
          GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(10798748));
          // ShowMessageDialog(GUILocalizeStrings.Get(1079861), "", GUILocalizeStrings.Get(10798748)); // Global Update finished !
        }
      }
      MyFilmsDetail.clearGUIProperty("statusmessage");

      if (GetID == ID_MyFilms)
      {
        //if (Configuration.currentConfig != "")
        //{
        //  if (this.facadeFilms == null || this.facadeFilms.SelectedListItemIndex == -1)
        //    Configuration.SaveConfiguration(Configuration.currentConfig, -1, "");
        //  else
        //    Configuration.SaveConfiguration(Configuration.currentConfig, this.facadeFilms.SelectedListItem.ItemId, this.facadeFilms.SelectedListItem.Label);
        //}

        // Load_Config(Configuration.currentConfig, true, null);
        Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
      }

      MyFilmsDetail.SetGlobalLock(false, conf.StrFileXml);
      if (ImportComplete != null && conf.AllowTraktSync) // trigger sync to trakt page after importer finished
      {
        ImportComplete();
        LogMyFilms.Debug("bgUpdateDB_RunWorkerCompleted(): Fired 'ImportCompleted' event to trigger sync to trakt page after importer finished !");
      }
    }

    //*****************************************************************************************
    //*  Download Backdrop Fanart in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateFanart()
    {
      if (!bgUpdateFanart.IsBusy)
      {
        bgUpdateFanart.RunWorkerAsync(r);
        LogMyFilms.Info("Downloading backdrop fanart in batch mode");
      }
    }

    static void bgUpdateFanart_DoWork(object sender, DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      var grab = new GrabberUrlClass();
      string personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty, wimdbid = string.Empty;
      int wyear = 0;
      if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist)) // if persoin artwork path present and person thumbs enabled, also load person images
      {
        personartworkpath = conf.StrPathArtist;
        LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
      }
      foreach (DataRow movieRecord in r)
      {
        wimdbid = MyFilmsDetail.GetIMDB_Id(movieRecord);
        string fanartTitle = MyFilmsDetail.GetFanartTitle(movieRecord, out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
        if (fanartTitle.Length <= 0)
        {
          continue;
        }
        MyFilmsDetail.setGUIProperty("statusmessage", "Updating TMDB Fanart for '" + fanartTitle + "'");
        var listemovies = grab.GetFanart(wtitle, fanartTitle, wyear, wdirector, wimdbid, conf.StrPathFanart, true, false, conf.StrTitle1, personartworkpath);
      }
    }

    static void bgUpdateFanart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Backdrop Fanart download finished");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798757), "Fanart Updates finished");
    }

    //*****************************************************************************************
    //*  Download Actors Artwork in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateActors(ArrayList actors)
    {
      if (!bgUpdateActors.IsBusy)
      {
        bgUpdateActors.RunWorkerAsync(actors);
        LogMyFilms.Info("AsynUpdateActors() : Downloading actors artwork in batch mode");
      }
    }

    void bgUpdateActors_DoWork(object sender, DoWorkEventArgs e)
    {
      ////BackgroundWorker worker = sender as BackgroundWorker;
      ////Grabber.GrabberUrlClass Grab = new Grabber.GrabberUrlClass();
      //ArrayList persons = e.Argument as ArrayList;
      //Grabber.TheMovieDb.TmdbAPI api = new TmdbAPI(TmdbApiKey);


      //foreach (string person in persons)
      //{
      //  // ToDo: Reenable AddActor() to scrape actor infos - make sure, it's not causing too much load...
      //  // AddActor(person);
      //  TmdbPerson[] personsearchresult = api.PersonSearch(person);
      //  foreach (TmdbPerson tmdbPerson in personsearchresult)
      //  {
      //  }
      //}
    }

    static void bgUpdateActors_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("actors artwork download finished");
    }

    private void AddActor(string name)
    {
      var ds = new AntMovieCatalog();

      //List<AntMovieCatalog.PersonRow> persons = new List<AntMovieCatalog.PersonRow>();
      //persons = (from AntMovieCatalog.PersonRow person in ds.Person select person).ToList();
      //persons = ds.Person.Where(x => x.Name == name).ToList();
      if (ds.Person.Count(x => x.Name == name) == 0)
      {
        AntMovieCatalog.PersonRow person = ds.Person.NewPersonRow();
        person.Name = name;
        //ToDo: grab data here ...
        ds.Person.AddPersonRow(person);
        LogMyFilms.Info("AddActor() - Added '" + name + "' to Person table");
      }
      else
        LogMyFilms.Info("AddActor() - Not Added '" + name + "' already exists");

    }

    //*****************************************************************************************
    //*  Load List movie file in batch mode                                                   *
    //*****************************************************************************************
    public void AsynLoadMovieList()
    {
      if (!bgLoadMovieList.IsBusy)
      {
        LogMyFilms.Info("AsynLoadMovieList() - Loading Movie List in batch mode");
        bgLoadMovieList.RunWorkerAsync();
      }
      else
        LogMyFilms.Debug(("AsynLoadMovieList() could not be launched because already running !"));
    }

    static void bgLoadMovieList_DoWork(object sender, DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      string searchrep = conf.StrDirStor;
      var allDrives = DriveInfo.GetDrives();

      searchrep = allDrives.Where(d => (d.DriveType.ToString() == "CDRom") && d.IsReady).Aggregate(searchrep, (current, d) => current.Length > 0 ? current + ";" + d.Name : d.Name);
      var oRegex = new Regex(";");
      string[] searchDir = oRegex.Split(searchrep);
      foreach (string path in searchDir)
      {
        conf.MovieList.Add(Directory.GetFiles(path));
        if (conf.SearchSubDirs == false || !Directory.Exists(path)) continue;
        foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
        {
          conf.MovieList.Add(Directory.GetFiles(sFolderSub));
        }
      }
    }

    static void bgLoadMovieList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Loading Movie List in batch mode finished");
    }

    public void AMCupdaterStartEventHandler()
    {
      // AMCUpdater.AntProcessor.bgwFolderScanUpdate.ProgressChanged += new ProgressChangedEventHandler(AMCupdater_ProgressChanged);
      // AMCUpdater.AntProcessor.bgwFolderScanUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AMCupdater_RunWorkerCompleted);
    }
    void AMCupdater_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      LogMyFilms.Info("AMCupdater_ProgressChanged. (GetID = '" + GetID + "', Message: " + e.UserState + ", Progress: " + e.ProgressPercentage + ")");
      if (GetID == ID_MyFilms)
      {
        MyFilmsDetail.setGUIProperty("statusmessage", "AMCupdater Running - Importing Movies");
      }
    }
    void AMCupdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("AMCupdater RunWorkerCompleted");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      {
        GUIUtils.ShowNotifyDialog("Global Update finished !");
      }
    }

    //*****************************************************************************************
    //*  Check availability status of media files in batch mode                                                   *
    //*****************************************************************************************
    public void AsynIsOnlineCheck()
    {
      LogMyFilms.Debug("AsynIsOnlineCheck() - Try Launching Availabilityscanner - InitialIsOnlineScan = '" + InitialIsOnlineScan + "'");
      if (!InitialIsOnlineScan)
      {
        if (!bgIsOnlineCheck.IsBusy) // only launch, if not yet completed and not already running
        {
          LogMyFilms.Info("AsynIsOnlineCheck() - Check IsOnline  started in batch mode");
          MyFilmsDetail.setGUIProperty("statusmessage", "Availability Scanner started ...", false);
          bgIsOnlineCheck.RunWorkerAsync(r);
        }
        else
          LogMyFilms.Debug(("AsynIsOnlineCheck() could not be launched because already running !"));
      }
      else
        LogMyFilms.Debug(("AsynIsOnlineCheck() not launched because 'InitialIsOnlineScan' is already done ...!"));
    }

    static void bgIsOnlineCheck_DoWork(object sender, DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      var startTime = DateTime.Now;
      var oRegex = new Regex(";");
      LogMyFilms.Info("bgIsOnlineCheck_DoWork: Now checking Online Status - Source field <film> is: '" + conf.StrStorage + "' - Source field <trailer> is: '" + conf.StrStorageTrailer + "'");

      // Check Config
      bool filmSource = Helper.FieldIsSet(conf.StrStorage);
      bool filmSearch = !(string.IsNullOrEmpty(conf.ItemSearchFile) || string.IsNullOrEmpty(conf.SearchFile) || conf.SearchFile == "False" || conf.SearchFile == "no");
      bool trailerSource = Helper.FieldIsSet(conf.StrStorageTrailer);
      bool trailerSearch = !(string.IsNullOrEmpty(conf.ItemSearchFileTrailer) || !conf.SearchFileTrailer);
      LogMyFilms.Debug("bgIsOnlineCheck_DoWork: filmSource = '" + filmSource + "', filmSearch = '" + filmSearch + "', trailerSource = '" + trailerSource + "', trailerSearch = '" + trailerSearch + "'");

      // Build MovieList (files)
      if (filmSearch) // if search by filename is activated in setup ...
      {
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Now checking Searchpathes, adding CDrom(s) and build MovieList ...");
        string searchrep = conf.StrDirStor; // Searchpath for movies 
        var allDrives = DriveInfo.GetDrives(); // get local drives to find CDrom(s)
        foreach (DriveInfo d in allDrives.Where(d => (d.DriveType.ToString() == "CDRom") && d.IsReady))
        {
          if (searchrep.Length > 0) searchrep = searchrep + ";" + d.Name;
          else searchrep = d.Name;
        }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Resulting Searchpathes after adding active CDrom(s): '" + searchrep + "'");

        try
        {
          string[] searchDir = oRegex.Split(searchrep);
          foreach (string path in searchDir)
          {
            if (Directory.Exists(path))
            {
              MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - GetDir: '" + path + "'");
              conf.MovieList.Add(Directory.GetFiles(path));
              if (conf.SearchSubDirs == false) continue;
              foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
              {
                MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - GetDir: '" + sFolderSub + "'");
                conf.MovieList.Add(Directory.GetFiles(sFolderSub));
              }
            }
            else
            {
              LogMyFilms.Error("bgIsOnlineCheck_DoWork: Search Directory (Movie) not found: '" + path + "' - check your config!");
            }
          }
        }
        catch (Exception ex) { LogMyFilms.Debug("bgIsOnlineCheck_DoWork: SearchDir exception: " + ex.Message + ", " + ex.StackTrace); }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Movie Files found: '" + conf.MovieList.Count + "'");
      }

      // Build TrailerList (files)
      if (trailerSearch) // if search by filename is activated in setup ...
      {
        LogMyFilms.Info("bgIsOnlineCheck_DoWork: Now checking Searchpathes for Trailers and build TrailerList ...");
        var searchrepTrailer = conf.StrDirStorTrailer; // Searchpath for trailer 
        var searchDirTrailer = oRegex.Split(searchrepTrailer);
        foreach (string path in searchDirTrailer)
        {
          if (Directory.Exists(path))
          {
            conf.TrailerList.Add(Directory.GetFiles(path));
            foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
              conf.TrailerList.Add(Directory.GetFiles(sFolderSub));
            }
          }
          else
          {
            LogMyFilms.Error("bgIsOnlineCheck_DoWork: Search Directory (Trailer) not found: '" + path + "' - check your config!");
          }
        }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Trailer Files found: '" + conf.TrailerList.Count + "'");
      }

      LogMyFilms.Info("bgIsOnlineCheck_DoWork: Read MyFilms Database ...");
      DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens, true);
      string totalRecords = wr.Length.ToString();
      LogMyFilms.Debug("bgIsOnlineCheck_DoWork: DB-Records found: '" + totalRecords + "'");

      // Now scan for availability
      int counter = 0;
      foreach (DataRow t in wr)
      {
        counter++;
        // Check Movie availability
        if (!filmSource && !filmSearch)
        {
          //LogMyFilms.Error("bgIsOnlineCheck_DoWork: Error checking Movie Online Status - Source or Search not properly defined in setup!");
        }
        else
        {
          bool isonline = true; // set true as default
          string fileName = string.Empty;

          if (filmSource)
          {
            try
            {
              fileName = t[conf.StrStorage].ToString().Trim();
            }
            catch (Exception ex)
            {
              LogMyFilms.Error("bgIsOnlineCheck_DoWork: Error getting source media files from DB - exception: " + ex.Message);
              fileName = string.Empty;
            }
            //fileName = fileName.Substring(0, fileName.LastIndexOf(";")).Trim();
            string[] Mediafiles = oRegex.Split(fileName);
            foreach (string mediafile in Mediafiles)
            {
              if (mediafile.Length > 0 && File.Exists(mediafile))
              {
                // isonline = true;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SOURCE check   media AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
              else
              {
                isonline = false;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SOURCE check   media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
            }
          }
          if (!isonline && filmSearch) // if movie not found via source and search is enabled...
          {
            string movieName = t[conf.ItemSearchFile].ToString();
            movieName = movieName.Substring(movieName.LastIndexOf(conf.TitleDelim, StringComparison.Ordinal) + 1).Trim().ToLower();
            string[] result = conf.MovieList.Find(files => files.Count(n => n.ToLower().Contains(@"\" + movieName + @".") || n.ToLower().Contains(@"\" + movieName + @"\")) > 0);
            if (result != null)
            {
              foreach (string file in result.Where(file => !string.IsNullOrEmpty(file.Trim())).Where(MediaPortal.Util.Utils.IsVideo))
              {
                isonline = true;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   file FOUND      for title '" + t[conf.StrTitle1] + "' - file found: '" + file + "'");
              }
            }
            else
            {
              isonline = false;
              LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file found: (none)");
            }

            //MyFilmsDetail.result = new ArrayList();
            //MyFilmsDetail.SearchFiles(movieName, conf.StrDirStor, false, false);
            //if (MyFilmsDetail.result.Count != 0) // found!
            //{
            //  // isonline = true;
            //  LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   file FOUND      for title '" + t[conf.StrTitle1] + "' - file found: '" + MyFilmsDetail.result[0].ToString().Trim() + "'");
            //}
            //else
            //{
            //  isonline = false;
            //  LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file found: (none)");
            //}
          }
          t["IsOnline"] = isonline.ToString();
          MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - (" + counter + " of " + totalRecords + ") - Film: '" + t[conf.StrTitle1] + "'", false);
        }

        // Check Trailer availability
        if (!trailerSource && !trailerSearch)
        {
          //LogMyFilms.Error("bgIsOnlineCheck_DoWork: Error checking Trailer Online Status - Source or Search not properly defined in setup!");
        }
        else
        {
          bool isonline = true; // set true as default - even if it might not be like that ...
          string fileName = string.Empty;

          if (trailerSource)
          {
            try { fileName = t[conf.StrStorageTrailer].ToString().Trim(); }
            catch { fileName = string.Empty; }

            string[] mediafiles = oRegex.Split(fileName);
            foreach (string mediafile in mediafiles)
            {
              if (mediafile.Length > 0 && File.Exists(mediafile))
              {
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - trailer (" + counter + ") media AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
              else
              {
                isonline = false;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - trailer (" + counter + ") media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
            }
          }
          if (!isonline && trailerSearch)
          {
            // Currently no autosearch implemented
          }
          t["IsOnlineTrailer"] = isonline.ToString();
          MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner (" + counter + " of " + totalRecords + " - film): '" + t[conf.StrTitle1] + "'", false);
        }
      }
      DateTime stopTime = DateTime.Now;
      TimeSpan duration = stopTime - startTime;
      LogMyFilms.Info("bgIsOnlineCheck_DoWork - total runtime was: '" + duration + "', runtime in seconds: '" + duration.TotalSeconds + "'");
      MyFilmsDetail.Update_XML_database();
    }

    void bgIsOnlineCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Check IsOnline in batch mode finished. (GetID = '" + GetID + "')");
      //if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      //{
      //  MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(10798948), "Finished !");
      //}
      if (GetID == ID_MyFilms)
      {
        //Configuration.SaveConfiguration(Configuration.currentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
        //Load_Config(Configuration.currentConfig, true);
        InitialIsOnlineScan = true; // let MF know, the status has been retrieved !
        MyFilmsDetail.clearGUIProperty("statusmessage");
        GUIUtils.ShowNotifyDialog(GUILocalizeStrings.Get(10798948), "Online Check finished");
        // Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position

        // save current position of the facade
        if (facadeFilms != null)
        {
          conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = facadeFilms.SelectedListItem.Label;
        }
        Refreshfacade(); // Fin_Charge_Init(false, true); //need to reload the facade, but NOT default select, as it otherwise will reset global filters the user might have set...
      }
      if (ImportComplete != null && conf.AllowTraktSync) // trigger sync to trakt page after importer finished
      {
        ImportComplete();
        LogMyFilms.Debug("bgIsOnlineCheck_RunWorkerCompleted(): Fired 'ImportCompleted' event to trigger sync to trakt page after importer finished !");
      }
    }


    //*****************************************************************************************
    //*  Search and register Trailers in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateTrailer()
    {
      if (!bgUpdateTrailer.IsBusy)
      {
        bgUpdateTrailer.RunWorkerAsync(r);
        LogMyFilms.Info("starting 'Search and register Trailer' in batch mode");
      }
    }

    static void bgUpdateTrailer_DoWork(object sender, DoWorkEventArgs e)
    {
      // ToDo: Check fanart worker thread to implement same way !!!
    }

    static void bgUpdateTrailer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("'Search and register Trailer' Thread finished");
    }

    //*****************************************************************************************
    //*  Download Trailers from worker queue
    //*****************************************************************************************
    public void AsynDownloadTrailer()
    {
      if (!bgDownloadTrailer.IsBusy)
      {
        bgDownloadTrailer.RunWorkerAsync();
        LogMyFilms.Info("starting 'Download Trailer Worker' in batch mode");
      }
    }

    void bgDownloadTrailer_DoWork(object sender, DoWorkEventArgs e)
    {
      //// init WebClients
      //for (int i = 0; i < BaseMesFilms.WebClients.Length; i++)
      //{
      //  BaseMesFilms.WebClients[i].DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
      //  BaseMesFilms.WebClients[i].DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
      //  // Starts the download
      //  BaseMesFilms.WebClients[i].DownloadFileAsync(new Uri("SomeURLToFile"), "SomePlaceOnLocalHardDrive");
      //}

      do
      {
        if (bgDownloadTrailer.CancellationPending)
        {
          LogMyFilms.Debug("cancel trailer download...");
          return;
        }

        Trailer f;
        // setDownloadStatus();
        lock (TrailertoDownloadQueue)
        {
          f = TrailertoDownloadQueue.Dequeue();
          LogMyFilms.Debug("bgDownloadTrailer_DoWork() - start loading trailer '" + f.Trailername + "' for movie '" + f.MovieTitle + "', remaining trailers in queue: '" + TrailertoDownloadQueue.Count + "'");
        }

        try
        {
          #region recheck download URL
          if (f.SourceUrl == null)
          {
            Dictionary<string, string> availableTrailerFiles = MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls(f.OriginalUrl);
            f.SourceUrl = availableTrailerFiles.Last().Value;
            f.Quality = availableTrailerFiles.Last().Key;
            
          }
          #endregion

          #region download trailer
          string extension = (f.SourceUrl.Contains(".")) ? f.SourceUrl.Substring(f.SourceUrl.LastIndexOf(".")) : ".err";
          string destinationfile = Path.Combine(f.DestinationDirectory, (MediaPortal.Util.Utils.FilterFileName(f.MovieTitle + " (trailer) " + f.Trailername + " (" + f.Quality.Replace(" ", "") + ")" + extension)));
          if (!File.Exists(destinationfile))
          {
            MyFilmsDetail.setGUIProperty("statusmessage", "Downloading  TMDB trailer '" + f.Trailername + "' for '" + f.MovieTitle + "'");
            bool bDownloadSuccess = Updater.DownloadFile(f.SourceUrl, destinationfile);
            if (!bDownloadSuccess) // refresh urls for download - might have timed out ....
            {
              LogMyFilms.Debug("bgDownloadTrailer_DoWork() - no success - retry with refreshed web link !");
              Dictionary<string, string> availableTrailerFiles = MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls(f.OriginalUrl);
              KeyValuePair<string, string> highestQualitySelection = MyFilmsPlugin.Utils.OVplayer.GetPreferredQualityOption(availableTrailerFiles, "FHD");
              string newUrl;
              f.SourceUrl = (availableTrailerFiles.TryGetValue(f.Quality, out newUrl)) ? newUrl : highestQualitySelection.Value;
              bDownloadSuccess = Updater.DownloadFile(f.SourceUrl, destinationfile);
            }
            LogMyFilms.Debug("bgDownloadTrailer_DoWork() - success = '" + bDownloadSuccess + "' for movie '" + f.MovieTitle + "' - trailer = '" + f.Trailername + "', trailerpath = '" + destinationfile + "'");
            MyFilmsDetail.setGUIProperty("statusmessage", "");
          }
          else
          {
            LogMyFilms.Debug("bgDownloadTrailer_DoWork() - file already exists - skip! for movie '" + f.MovieTitle + "' - trailer = '" + f.Trailername + "', trailerpath = '" + destinationfile + "'");
            Thread.Sleep(3000);
          }
          #endregion
        }
        catch (Exception ex)
        {
          // LogMyFilms.DebugException("Error loading trailer: '" + ex.Message + "'", ex);
          LogMyFilms.Debug("Error loading trailer: '" + ex.Message);
          MyFilmsDetail.setGUIProperty("statusmessage", "");
        }
      }
      while (TrailertoDownloadQueue.Count > 0 && !bgDownloadTrailer.CancellationPending); //  && MyFilms.conf.CacheOnlineTrailer (removed from consition, as this does not alloow siongle trailer downloads, when caching is disabled !
      bgDownloadTrailerDoneEvent.Set();
    }

    static void bgDownloadTrailer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("'Download Trailer Worker' thread finished");
    }

    static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      double bytesIn = double.Parse(e.BytesReceived.ToString());
      double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
      double percentage = bytesIn / totalBytes * 100;

      int progresspercent = int.Parse(Math.Truncate(percentage).ToString());
    }

    void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
      LogMyFilms.Info("client_DownloadFileCompleted() - trailer download finished");
    }

    void threadArray_DoWork(object sender, DoWorkEventArgs e)
    {
      int threadId = (int)e.Argument;
      do
      {
        if (threadArray[threadId].CancellationPending)
        {
          LogMyFilms.Debug("cancel trailer image download in thread '" + threadId + "' ...");
          return;
        }

        Trailer f;
        lock (TrailertoDownloadQueue)
        {
          f = TrailertoDownloadQueue.Dequeue();
          LogMyFilms.Debug("Download Thread '" + threadId + "' - start loading trailer '" + f.Trailername + "' for movie '" + f.MovieTitle + "', remaining trailers in queue: '" + TrailertoDownloadQueue.Count + "'");
        }
        try
        {
          #region recheck download URL
          if (f.SourceUrl == null)
          {
            Dictionary<string, string> availableTrailerFiles = OVplayer.GetYoutubeDownloadUrls(f.OriginalUrl);
            KeyValuePair<string, string> highestQualitySelection = OVplayer.GetPreferredQualityOption(availableTrailerFiles, "FHD");
            f.SourceUrl = highestQualitySelection.Value;
            f.Quality = highestQualitySelection.Key;
          }
          #endregion

          #region download trailer
          string extension = (f.SourceUrl.Contains(".")) ? f.SourceUrl.Substring(f.SourceUrl.LastIndexOf(".")) : ".err";
          string destinationfile = Path.Combine(f.DestinationDirectory, (MediaPortal.Util.Utils.FilterFileName(f.MovieTitle + " (trailer) " + f.Trailername + " (" + f.Quality.Replace(" ", "") + ")" + extension)));
          if (!File.Exists(destinationfile))
          {
            //bool stopThread = false;
            //while (stopThread == false)
            //{
            //  //DoWork();
            //}
            
            ////// async webclient
            //var client = new WebClient();
            //var uri = new Uri(address);
            //client.DownloadFileCompleted += (sender, e) => Console.WriteLine("Finished");
            //client.DownloadFileAsync(uri, "Hamsters.txt");

            //WebClient client = new WebClient();
            //Uri uri = new Uri(address);
            //client.DownloadFileCompleted += (sender, e) =>
            //{
            //  //inspect e here:
            //  //e.Error
            //};
            //client.DownloadProgressChanged += (sender, e) =>
            //{
            //  //e.ProgressPercentage
            //};
            //client.DownloadFileAsync(uri, "blabla");
            //client.CancelAsync(); // stop download
            
            //////token for cancellation
            //CancellationTokenSource cts = new CancellationTokenSource();
            //CancellationToken token = cts.Token;
            //WebClient wc = new WebClient();

            //// To request cancellation on the token
            //// will call CancelAsync on the WebClient.
            //token.Register(() => wc.CancelAsync());

            //Console.WriteLine("Starting request");
            //wc.DownloadStringAsync(new Uri("http://www.contoso.com"));

            ////// origihnal method code
            //try
            //{
            //  Directory.CreateDirectory(Path.GetDirectoryName(localFile));
            //  if (!File.Exists(localFile))
            //  {
            //    LogMyFilms.Debug("DownloadFile() - downloading file from: {0}", url);
            //    wc.DownloadFileAsync(new Uri(url), localFile);
            //    wc.DownloadFile(url, localFile);
            //  }
            //  else
            //  {
            //    LogMyFilms.Debug("DownloadFile() - file already exists - {0}", localFile);
            //  }
            //  return;
            //}
            //catch (Exception ex)
            //{
            //  LogMyFilms.Error("Download failed from '{0}' to '{1}' - Error: '{2}'", url, localFile);
            //  LogMyFilms.Debug("Error Reason: '{0}'", ex.Message);
            //  try
            //  {
            //    if (File.Exists(localFile)) File.Delete(localFile);
            //  }
            //  catch
            //  {
            //  }
            //}


            //WebClient wc = new WebClient();
            //Task<bool> downloadTask = Grabber.Updater.DownloadFileAsyncCore(wc, new Uri("request.Address"), "request.FileName");
            //bool success = true;
            //success = downloadTask.Result;
            //(downloadTask.AsyncState as WebClient).Dispose();
        

            bool bDownloadSuccess = Updater.DownloadFile(f.SourceUrl, destinationfile);
            if (!bDownloadSuccess) // refresh urls for download - might have timed out ....
            {
              LogMyFilms.Debug("Download Thread '" + threadId + "' - no success - retry with refreshed web link !");
              Dictionary<string, string> availableTrailerFiles = MyFilmsPlugin.Utils.OVplayer.GetYoutubeDownloadUrls(f.OriginalUrl);
              KeyValuePair<string, string> highestQualitySelection = MyFilmsPlugin.Utils.OVplayer.GetPreferredQualityOption(availableTrailerFiles, "FHD");
              string newUrl;
              f.SourceUrl = (availableTrailerFiles.TryGetValue(f.Quality, out newUrl)) ? newUrl : highestQualitySelection.Value;
              bDownloadSuccess = Updater.DownloadFile(f.SourceUrl, destinationfile);
            }
            LogMyFilms.Debug("Download Thread '" + threadId + "' - success = '" + bDownloadSuccess + "' for movie '" + f.MovieTitle + "' - trailer = '" + f.Trailername + "', trailerpath = '" + destinationfile + "'");
          }
          else
          {
            LogMyFilms.Debug("Download Thread '" + threadId + "' - file already exists - skip! for movie '" + f.MovieTitle + "' - trailer = '" + f.Trailername + "', trailerpath = '" + destinationfile + "'");
            Thread.Sleep(3000);
          }
          #endregion
        }
        catch (Exception ex)
        {
          // LogMyFilms.DebugException("Error loading trailer: '" + ex.Message + "'", ex);
          LogMyFilms.Warn("Error loading trailer: " + ex.Message);
        }
      }
      while (TrailertoDownloadQueue.Count > 0 && !threadArray[threadId].CancellationPending); // && MyFilms.conf.CacheOnlineTrailer);
      threadDoneEventArray[threadId].Set();
    }

    static void threadArray_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("threadArray_RunWorkerCompleted() - trailer worker thread finished");
    }

    private void FanartTimerEvent(object state)
    {
      if (backdrop.Filename != null && GUIWindowManager.ActiveWindow == ID_MyFilms)
      {
        //LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered for '" + facadeFilms.SelectedListItem.Label + "' !");
        bool success = ToggleFanart();
        if (!success)
          LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered, but not executed due to backdrop not enabled or set properly !");
      }
      else
      {
        LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered, but NOT executed due to MyFilms Main window not active (or backdrop filename not set)!");
      }
    }

    private bool ToggleFanart()
    {
      if (backdrop == null)
      {
        Fanartstatus(false);
        return false; // Fanart not supported by skin, exit now
      }
      string newfanart = GetNewRandomFanart(false, true);
      if (currentFanartList.Count > 1)
      {
        backdrop.Filename = newfanart;
        // LogMyFilms.Debug("(ToggleFanart): Loaded fanart is: '" + backdrop.Filename + "'");
        return true;
      }
      else
      {
        _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
        return false;
      }
    }

    private void Fanartstatus(bool status)
    {
      if (backdrop == null)
        backdrop = new ImageSwapper();

      if (status)
      {
        if (!backdrop.Active)
          backdrop.Active = true;
        if (currentFanartList.Count > 0)
          _fanartTimer.Change(0, RandomFanartDelay); //default 15000 = 15 secs
        else
          _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);

        GUIWindowManager.SendThreadCallback((p1, p2, o) =>
        {
          GUIControl.ShowControl(GetID, 35);
          return 0;
        }, 0, 0, null);
      }
      else
      {
        GUIWindowManager.SendThreadCallback((p1, p2, o) =>
        {
          GUIControl.HideControl(GetID, 35);
          MyFilmsDetail.clearGUIProperty("currentfanart");
          return 0;
        }, 0, 0, null);

        if (backdrop.Active)
          backdrop.Active = false;
        // Disable Random Fanart Timer
        _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
        // clear current fanarts
        currentFanartList.Clear();
        // Disable Fanart                
        backdrop.Filename = String.Empty;
      }
      LogMyFilms.Debug("Fanartstatus switched to '" + status + "'");
    }

    private void OnDetailsUpdated(bool searchPicture)
    {
      LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "', doUpdateMainViewByFinishEvent '" + doUpdateMainViewByFinishEvent + "'");
      if (GetID == ID_MyFilms && doUpdateMainViewByFinishEvent)
      {
        LogMyFilms.Debug("OnDetailsUpdated(): now reloading Details in Main screen");
        doUpdateMainViewByFinishEvent = false;
        Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
      }
      else
        LogMyFilms.Debug("OnDetailsUpdated(): Skipping reloading Details");
    }

    //*****************************************************************************************
    //*  set the #myfilms.view label
    //*****************************************************************************************
    private static void SetLabelView(string viewLabel)
    {
      string newViewLabel = viewLabel; // use the parameter as default ...
      string txtSelect = GUIPropertyManager.GetProperty("#myfilms.select");
      // LogMyFilms.Debug("SetLabelView() - called with parameter '" + viewLabel + "', txtSelect = '" + txtSelect + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
      if (viewLabel == GUILocalizeStrings.Get(342) || string.IsNullOrEmpty(viewLabel)) // case "films" or empty should show "all" = "filmes"
        viewLabel = "all";
      switch (viewLabel.ToLower())
      {
        case "search":
          newViewLabel = GUILocalizeStrings.Get(137);// "search"
          break;
        case "menu":
          newViewLabel = GUILocalizeStrings.Get(924);// "menu"
          break;
        case "online":
          newViewLabel = GUILocalizeStrings.Get(10798825); //online information
          break;
        case "all":
          newViewLabel = GUILocalizeStrings.Get(342); //videos
          break;
        case "storage":
          conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
          newViewLabel = GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951); //storage
          break;

        default:
          if (BaseMesFilms.TranslateColumn(viewLabel) != "")
            newViewLabel = BaseMesFilms.TranslateColumn(viewLabel);
          break;
      }
      MyFilmsDetail.setGUIProperty("view", newViewLabel);
      conf.StrTxtView = newViewLabel;
      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + ((txtSelect.Length > 0) ? "/" : "") + txtSelect); // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view"));
      GUIControl.SetControlLabel(ID_MyFilms, (int)Controls.CTRL_BtnViewAs, GUILocalizeStrings.Get(97) + newViewLabel); // "View by: " + <view>
      LogMyFilms.Debug("SetLabelView has been called with '" + viewLabel + "' -> set view to '" + newViewLabel + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
    }

    //*****************************************************************************************
    //*  set the #myfilms.select label
    //*****************************************************************************************
    private void SetLabelSelect(string selectLabel)
    {
      string initialSelectLabel = selectLabel;
      if (selectLabel == null) selectLabel = "";
      // LogMyFilms.Debug("SetLabelSelect() - called with parameter  '" + selectLabel + "', conf.StrTxtSelect = '" + conf.StrTxtSelect + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");

      if (selectLabel == GUILocalizeStrings.Get(10798632) || selectLabel == GUILocalizeStrings.Get(10798622)) // 10798622 all films // 10798632 (global filter active) 
        selectLabel = ""; // will reassign proper value

      switch (selectLabel.ToLower())
      {
        case "": // also includes "all" and "filtered"
        case "root":
          //conf.StrTxtSelect = GUILocalizeStrings.Get(10798622) + " " + GUILocalizeStrings.Get(10798632); // 10798622 all films // 10798632 (global filter active) 
          // 10798632 (global filter active) / Filtered  //10798622 All
          conf.StrTxtSelect = GUILocalizeStrings.Get((GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + GlobalFilterStringMovieFormat3D).Length > 0 ? 10798632 : 10798622);
          break;

        case "menu":
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079819); // views menu
          break;

        default:
          conf.StrTxtSelect = selectLabel;
          break;
      }
      MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + (conf.StrTxtSelect.Length > 0 ? "/" : "") + conf.StrTxtSelect);
      // LogMyFilms.Debug("SetLabelSelect has been called with '" + initialSelectLabel + "', processed as '" + selectLabel + "' -> set select to '" + conf.StrTxtSelect + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
    }

    /// <summary>
    /// Setup logger. This funtion made by the team behind Moving Pictures 
    /// (http://code.google.com/p/moving-pictures/)
    /// </summary>
    private static void InitLogger()
    {
      // LoggingConfiguration config = new LoggingConfiguration();
      // Fix suggested in forum to avoid breaking other plugins logging...
      LoggingConfiguration config = LogManager.Configuration ?? new LoggingConfiguration();

      try
      {
        // myfilms log
        FileInfo logFile = new FileInfo(Config.GetFile(Config.Dir.Log, LogFileName));
        if (logFile.Exists)
        {
          if (File.Exists(Config.GetFile(Config.Dir.Log, OldLogFileName)))
            File.Delete(Config.GetFile(Config.Dir.Log, OldLogFileName));

          logFile.CopyTo(Config.GetFile(Config.Dir.Log, OldLogFileName));
          logFile.Delete();
        }

        // grabber log
        FileInfo logFile2 = new FileInfo(Config.GetFile(Config.Dir.Log, LogFileNameGrabber));
        if (logFile2.Exists)
        {
          if (File.Exists(Config.GetFile(Config.Dir.Log, OldLogFileNameGrabber)))
            File.Delete(Config.GetFile(Config.Dir.Log, OldLogFileNameGrabber));

          logFile2.CopyTo(Config.GetFile(Config.Dir.Log, OldLogFileNameGrabber));
          logFile2.Delete();
        }
      }
      catch (Exception) { }

      FileTarget fileTarget = new FileTarget();
      // Filter logFilter = new Filter(); // use to only log MyFilms messages ...
      fileTarget.FileName = Config.GetFile(Config.Dir.Log, LogFileName);
      fileTarget.Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss,fff} " +  // "${date:format=yyyy-mm-dd HH\\:mm\\:ss,fff} " + 
        // "| ${qpc:normalize=true:difference=true:alignDecimalPoint=true:precision=3:seconds=true} " + 
                          "${level:fixedLength=true:padding=5} [" +
        //"[${threadname:fixedLength=true:padding=10} | " + 
                          "${threadid:fixedLength=true:padding=3} | ${logger:fixedLength=true:padding=13:shortName=true} ]: " +
                          "${message} ${exception:format=tostring}";
      //"${qpc}";
      // ${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff}
      //${qpc:normalize=Boolean:difference=Boolean:alignDecimalPoint=Boolean:precision=Integer:seconds=Boolean}
      // ${threadid} - The identifier of the current thread.
      // ${threadname} - The name of the current thread.

      config.AddTarget("file", fileTarget);

      FileTarget fileTarget2 = new FileTarget();
      // Filter logFilter = new Filter(); // use to only log MyFilms messages ...
      fileTarget2.FileName = Config.GetFile(Config.Dir.Log, LogFileNameGrabber);
      fileTarget2.Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss,fff} " +  // "${date:format=yyyy-mm-dd HH\\:mm\\:ss,fff} " + 
        // "| ${qpc:normalize=true:difference=true:alignDecimalPoint=true:precision=3:seconds=true} " + 
                          "${level:fixedLength=true:padding=5} [" +
        //"[${threadname:fixedLength=true:padding=10} | " + 
                          "${threadid:fixedLength=true:padding=3} | ${logger:fixedLength=true:padding=13:shortName=true} ]: " +
                          "${message} ${exception:format=tostring}";
      //"${qpc}";
      // ${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff}
      //${qpc:normalize=Boolean:difference=Boolean:alignDecimalPoint=Boolean:precision=Integer:seconds=Boolean}
      // ${threadid} - The identifier of the current thread.
      // ${threadname} - The name of the current thread.

      config.AddTarget("file", fileTarget2);



      // Get current Log Level from MediaPortal 
      NLog.LogLevel logLevel;
      var xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

      //string myThreadPriority = xmlreader.GetValue("general", "ThreadPriority");

      //if (myThreadPriority != null && myThreadPriority.Equals("Normal", StringComparison.CurrentCulture))
      //{
      //    FHThreadPriority = "Lowest";
      //}
      //else if (myThreadPriority != null && myThreadPriority.Equals("BelowNormal", StringComparison.CurrentCulture))
      //{
      //    FHThreadPriority = "Lowest";
      //}
      //else
      //{
      //    FHThreadPriority = "BelowNormal";
      //}

      switch ((Level)xmlreader.GetValueAsInt("general", "loglevel", 0))
      {
        case Level.Error:
          logLevel = LogLevel.Error;
          break;
        case Level.Warning:
          logLevel = LogLevel.Warn;
          break;
        case Level.Information:
          logLevel = LogLevel.Info;
          break;
        case Level.Debug:
        default:
          logLevel = LogLevel.Debug;
          break;
      }

#if DEBUG
      logLevel = LogLevel.Debug;
#endif

      LoggingRule rule = new LoggingRule("MyFilms*", logLevel, fileTarget); // only push logging from namespace "MyFilms*" to log file
      config.LoggingRules.Add(rule);
      LoggingRule rule2 = new LoggingRule("Grabber*", logLevel, fileTarget2); // add Grabber classes to logging
      config.LoggingRules.Add(rule2);
      LoggingRule rule3 = new LoggingRule("grabber*", logLevel, fileTarget2); // add TMDB grabber classes to logging
      config.LoggingRules.Add(rule3);

      LogManager.Configuration = config;
    }

    private void CheckAndLogEnhancedSkinControls()
    {
      if (dummyFacadeMenu == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Menu'");
      if (dummyFacadeView == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'View'");
      if (dummyFacadePerson == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Person'");
      if (dummyFacadeIndex == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Index'");
      if (dummyFacadeFilm == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Film'");
      if (dummyFacadeHierarchy == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Hierarchy'");
    }

    private static void InitGUIPropertyLabels()
    {
      using (var xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        string pluginname = xmlreader.GetValueAsString("MyFilms", "PluginName", "Films");
        string pluginmode = xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal");
        if (pluginmode != "normal") pluginname += " (" + pluginmode + ")";

        MyFilmsDetail.setGUIProperty("config.pluginname", pluginname);
        MyFilmsDetail.setGUIProperty("config.pluginmode", pluginmode);
        MyFilmsDetail.setGUIProperty("config.version", MyFilmsSettings.Version.ToString());
        MyFilmsDetail.setGUIProperty("config.mpversion", MyFilmsSettings.MPVersion.ToString());
        MyFilmsDetail.setGUIProperty("config.country", MyFilmsSettings.MPLanguage);

        LogMyFilms.Info("Startmode: '" + pluginmode + "'");
        DebugPropertyLogging = (xmlreader.GetValueAsString("MyFilms", "PropertyLogging", "false").ToLower() == "true");
        LogMyFilms.Info("Property Logging: '" + DebugPropertyLogging + "'");
      }

      EmptyFacadeValue = "(<" + GUILocalizeStrings.Get(10798774) + ">)";
      using (var ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          MyFilmsDetail.setGUIProperty("db." + dc.ColumnName.ToLower() + ".label", BaseMesFilms.TranslateColumn(dc.ColumnName));
        }
      }

      GUIPropertyManager.SetProperty("#btWeb.startup.link", "");
      GUIPropertyManager.SetProperty("#btWeb.link.zoom", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");

      MyFilmsDetail.setGUIProperty("views.count", "0");

      MyFilmsDetail.setGUIProperty("db.calc.aspectratio.label", GUILocalizeStrings.Get(10798697));
      MyFilmsDetail.setGUIProperty("db.calc.imageformat.label", GUILocalizeStrings.Get(10798698));
      MyFilmsDetail.setGUIProperty("user.sourcetrailer.label", GUILocalizeStrings.Get(10798649));
      MyFilmsDetail.setGUIProperty("user.source.label", GUILocalizeStrings.Get(10798648));
      MyFilmsDetail.setGUIProperty("nbobjects.unit", GUILocalizeStrings.Get(127));
      MyFilmsDetail.setGUIProperty("db.length.unit", GUILocalizeStrings.Get(2998));

      MyFilmsDetail.setGUIProperty("user.name.label", GUILocalizeStrings.Get(10799310));
      MyFilmsDetail.setGUIProperty("user.rating.label", GUILocalizeStrings.Get(10798944));
      MyFilmsDetail.setGUIProperty("user.watched.label", GUILocalizeStrings.Get(200027));
      MyFilmsDetail.setGUIProperty("user.watchedcount.label", GUILocalizeStrings.Get(1079910));
      MyFilmsDetail.setGUIProperty("user.watcheddate.label", GUILocalizeStrings.Get(10798686));

      MyFilmsDetail.setGUIProperty("person.name.label", GUILocalizeStrings.Get(10799301));
      MyFilmsDetail.setGUIProperty("person.dateofbirth.label", GUILocalizeStrings.Get(10799302));
      MyFilmsDetail.setGUIProperty("person.placeofbirth.label", GUILocalizeStrings.Get(10799303));
      MyFilmsDetail.setGUIProperty("person.biography.label", GUILocalizeStrings.Get(10799304));

      MyFilmsDetail.clearGUIProperty("user.name.value");
      MyFilmsDetail.clearGUIProperty("user.onlinestatus");
      MyFilmsDetail.clearGUIProperty("user.rating.value");
      MyFilmsDetail.clearGUIProperty("user.watched.value");
      MyFilmsDetail.clearGUIProperty("user.watchedcount.value");
      MyFilmsDetail.clearGUIProperty("user.watchedcountglobal.value");
      MyFilmsDetail.clearGUIProperty("user.watcheddate.value");

      MyFilmsDetail.clearGUIProperty("user.source.isonline");
      MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");
      MyFilmsDetail.clearGUIProperty("logos_id2001");
      MyFilmsDetail.clearGUIProperty("logos_id2002");
      MyFilmsDetail.clearGUIProperty("logos_id2003");
      MyFilmsDetail.clearGUIProperty("logos_id2012"); // Combined Logo
      MyFilmsDetail.clearGUIProperty("nbobjects.value");
      MyFilmsDetail.clearGUIProperty("Fanart");
      MyFilmsDetail.clearGUIProperty("Fanart2");
      MyFilmsDetail.clearGUIProperty("config.currentconfig");
      MyFilmsDetail.clearGUIProperty("config.configfilter");
      MyFilmsDetail.clearGUIProperty("view");
      MyFilmsDetail.clearGUIProperty("select");
      MyFilmsDetail.clearGUIProperty("index");
      MyFilmsDetail.clearGUIProperty("isplaying");
      MyFilmsDetail.clearGUIProperty("picture");
      MyFilmsDetail.clearGUIProperty("currentfanart");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      MyFilmsDetail.clearGUIProperty("details.downloads.status");
      MyFilmsDetail.clearGUIProperty("details.downloads.count");
      MyFilmsDetail.clearGUIProperty("details.downloads.name");

      MyFilmsDetail.clearGUIProperty("moviecontext.value");

      // init Details GUIproperties
      MyFilmsDetail.setGUIProperty("menu.overview", GUILocalizeStrings.Get(10798751));
      MyFilmsDetail.setGUIProperty("menu.description", GUILocalizeStrings.Get(10798752));
      MyFilmsDetail.setGUIProperty("menu.comments", GUILocalizeStrings.Get(10798753));
      MyFilmsDetail.setGUIProperty("menu.actors", GUILocalizeStrings.Get(10798754));
      MyFilmsDetail.setGUIProperty("menu.techinfos", GUILocalizeStrings.Get(10798755));
      MyFilmsDetail.setGUIProperty("menu.extradetails", GUILocalizeStrings.Get(10798756));
      MyFilmsDetail.setGUIProperty("menu.fanart", GUILocalizeStrings.Get(10798757));
    }

    private void Clear_Logos()
    {
      LogMyFilms.Debug("Clear_Logos() - clearing logos ...");
      MyFilmsDetail.clearGUIProperty("logos_id2001");
      MyFilmsDetail.clearGUIProperty("logos_id2002");
      MyFilmsDetail.clearGUIProperty("logos_id2003");
      MyFilmsDetail.clearGUIProperty("logos_id2012");
    }

    private void Load_Logos(DataRow[] dr, int index)
    {
      LogMyFilms.Debug("Load_Logos() - Using Logos -> '" + conf.StrLogos + "'");
      if (dr == null || index > dr.Length - 1)
      {
        LogMyFilms.Warn("Load_Logos() - Failed loading Logos - index not within current dataset ...");
        Clear_Logos();
        return;
      }

      DataRow row = dr[index];
      if (ImgID2001 != null && conf.StrLogos)
      {
        try { MyFilmsDetail.setGUIProperty("logos_id2001", Logos.BuildLogos(row, "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, GetID)); }
        catch (Exception e) { LogMyFilms.Error("" + e.Message); }
      }
      else MyFilmsDetail.clearGUIProperty("logos_id2001");

      if (ImgID2002 != null && conf.StrLogos)
      {
        try { MyFilmsDetail.setGUIProperty("logos_id2002", Logos.BuildLogos(row, "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, GetID)); }
        catch (Exception e) { LogMyFilms.Error("" + e.Message); }
      }
      else MyFilmsDetail.clearGUIProperty("logos_id2002");

      if (ImgID2003 != null && conf.StrLogos)
      {
        try { MyFilmsDetail.setGUIProperty("logos_id2003", Logos.BuildLogos(row, "ID2003", ImgID2003.Height, ImgID2003.Width, ImgID2003.XPosition, ImgID2003.YPosition, GetID)); }
        catch (Exception e) { LogMyFilms.Error("" + e.Message); }
      }
      else MyFilmsDetail.clearGUIProperty("logos_id2003");

      if (ImgID2012 != null && conf.StrLogos)
      {
        try { MyFilmsDetail.setGUIProperty("logos_id2012", Logos.BuildLogos(row, "ID2012", ImgID2012.Height, ImgID2012.Width, ImgID2012.XPosition, ImgID2012.YPosition, GetID)); }
        catch (Exception e) { LogMyFilms.Error("" + e.Message); }
      }
      else MyFilmsDetail.clearGUIProperty("logos_id2012");
    }

    public class FswHandler
    {
      public void OnEvent(Object source, FileSystemEventArgs Args)
      {
        // ToDo: Add required actions here ?
        //Console.Out.WriteLine(Args.ChangeType.ToString());
      }
    }

    delegate void SetDummyControlsForFacadeWorker(ViewContext viewContext);
    private void SetDummyControlsForFacade(ViewContext viewContext)
    {
      SetDummyControlsForFacadeWorker listlevelsetter = SetDummyControls;
      listlevelsetter.BeginInvoke(viewContext, null, null);
      // SetDummyControls(ViewContext);
    }

    private void SetDummyControls(ViewContext viewContext)
    {
      LogMyFilms.Debug("SetDummyControlsForFacade() setting ViewContext to '" + viewContext + "'");
      //if (dummyFacadeFilm == null || dummyFacadeHierarchy == null || dummyFacadeView == null || dummyFacadePerson == null || dummyFacadeIndex == null)
      //{
      //  LogMyFilms.Warn("SetDummyControlsForFacade() - : ViewContext = '" + viewContext + "' - Warning ! - null detected (controls missing in skin?)");
      //  // return;
      //}

      //if (viewContext == ViewContext.Movie) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeFilm);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeFilm);
      //if (viewContext == ViewContext.MovieCollection) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeHierarchy);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeHierarchy);
      //if (viewContext == ViewContext.Group) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeView);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeView);
      //if (viewContext == ViewContext.Person) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadePerson);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadePerson);
      //if (viewContext == ViewContext.Menu || viewContext == ViewContext.None)
      //{
      //  // Do Nothing ...
      //}

      switch (viewContext)
      {
        case ViewContext.Menu:
        case ViewContext.MenuAll:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && !dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = true;
          break;
        case ViewContext.Movie:
        case ViewContext.TmdbMovies:
          if (dummyFacadeFilm != null && !dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = true;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.MovieCollection:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && !dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = true;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.Group:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && !dummyFacadeView.Visible) dummyFacadeView.Visible = true;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.Person:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && !dummyFacadePerson.Visible) dummyFacadePerson.Visible = true;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.None:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          if (dummyFacadeIndex != null && dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = false;
          break;
        default:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting ViewContext to 'Default' (all false)");
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          if (dummyFacadeIndex != null && dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = false;
          break;
      }
      if (dummyFacadeIndex != null)
      {
        bool indexed = (conf.Boolindexed && !conf.Boolindexedreturn);
        if (!indexed && dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = false;
        if (indexed && !dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = true;
        // LogMyFilms.Debug("SetDummyControlsForFacade() setting dummyFacadeIndex to '" + indexed + "'");
      }
      if (viewContext != ViewContext.Person) MyFilmsDetail.clearGUIProperty("moviecontext.value");
    }

    #endregion

    private bool CheckSkinInterfaceVersion() // returns false, if plugin should not start, true, if start can continue
    {
      string skin = GUIGraphicsContext.Skin.Substring(GUIGraphicsContext.Skin.LastIndexOf("\\") + 1);
      if (currentSkin == null || currentSkin != skin)
      {
        #region check skin version and existance
        int versionMajor;
        int versionMinor;
        int versionBuild;
        int versionRevision;
        bool success = GetSkinInterfaceVersion(skin, out versionMajor, out versionMinor, out versionBuild, out versionRevision);
        if (success)
        {
          LogMyFilms.Info("CheckSkinInterfaceVersion(): Current  Skin Interface Version = 'V" + versionMajor + "." + versionMinor + "' for skin '" + skin + "'");
          LogMyFilms.Info("CheckSkinInterfaceVersion(): Required Skin Interface Version = 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");
          if (versionMajor != SkinInterfaceVersionMajor || versionMajor == 0)
          {
            InitMainScreen(false);
            GUIUtils.ShowOKDialog(
              GUILocalizeStrings.Get(10799611), // Your MyFilms skin is not compatible!
              GUILocalizeStrings.Get(10799612) + "'V" + versionMajor + "." + versionMinor + "'", // Current Version: 
              GUILocalizeStrings.Get(10799613) + "'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'", // Required Version: 
              "");
            if (versionMajor > 0)
            {
              return false; // leave plugin
            }
          }
          else if (versionMinor < SkinInterfaceVersionMinor)
          {
            InitMainScreen(false);
            GUIUtils.ShowOKDialog(
              versionMinor < SkinInterfaceVersionMinor ? GUILocalizeStrings.Get(10799614) : GUILocalizeStrings.Get(10799615), // "Your MyFilms skin should be updated to support all features !" : "Your MyFilms skin should be downgraded to properly work with this version !",
              GUILocalizeStrings.Get(10799612) + "'V" + versionMajor + "." + versionMinor + "'", // Current Version: 
              GUILocalizeStrings.Get(10799613) + "'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'", // Required Version: 
              "");
          }
        }
        else
        {
          if (!File.Exists(GUIGraphicsContext.Skin + @"\MyFilms.xml"))
          {
            GUIUtils.ShowOKDialog(
              GUILocalizeStrings.Get(10799616), // "Your current skin does not support MyFilms!"
              GUILocalizeStrings.Get(10799617), // "Please ask skin author to add MyFilms support."
              GUILocalizeStrings.Get(10799613) + "'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'", // Required Version: 
              "");
            return false; // leave plugin
          }
          else
          {
            LogMyFilms.Info("CheckSkinInterfaceVersion(): Cannot read Current Skin Interface Version for skin '" + skin + "'");
            InitMainScreen(false);
            GUIUtils.ShowOKDialog(
              GUILocalizeStrings.Get(10799614), // "Your MyFilms skin should be updated to support all features !"
              "",
              GUILocalizeStrings.Get(10799613) + "'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'", // Required Version: 
              "");
          }
        }
        currentSkin = skin; // all ok, so make sure this is remembered for next time
        #endregion
      }
      else
      {
        LogMyFilms.Info("CheckSkinInterfaceVersion(): skipping check - no skin change - Current Skin is still '" + currentSkin + "'");
      }
      return true;
    }

    private bool GetSkinInterfaceVersion(string currentskin, out int iVersionMajor, out int iVersionMinor, out int iVersionBuild, out int iVersionRevision)
    {
      string versionMajor = "";
      string versionMinor = "";
      string versionBuild = "";
      string versionRevision = "";
      bool success = true;
      iVersionMajor = 0;
      iVersionMinor = 0;
      iVersionBuild = 0;
      iVersionRevision = 0;
      string name = GUIGraphicsContext.Skin.Substring(GUIGraphicsContext.Skin.LastIndexOf("\\") + 1);
      var doc = new XmlDocument();
      try
      {
        string file = Config.GetFile(Config.Dir.Skin, name, @"MyFilms.xml");
        doc.PreserveWhitespace = true;
        doc.Load(file);
        XmlElement root = doc.DocumentElement;
        if (root != null)
        {
          XmlNode controlsNode = GenericExtensions.SelectSingleNodeFast(root, "controls");
          if (controlsNode != null)
          {
            XmlNode settingsNode = GenericExtensions.SelectSingleNodeFast(controlsNode, "settings");
            if (settingsNode != null)
            {
              XmlNode skininterfaceversionNode = GenericExtensions.SelectSingleNodeFast(settingsNode, "skininterfaceversion");
              if (skininterfaceversionNode != null)
              {
                XmlNode versionNode = GenericExtensions.SelectSingleNodeFast(skininterfaceversionNode, "version");
                if (versionNode != null)
                {
                  XmlNode majorVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "major");
                  if (majorVersionNode != null)
                    versionMajor = majorVersionNode.InnerText;
                  XmlNode minorVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "minor");
                  if (minorVersionNode != null)
                    versionMinor = minorVersionNode.InnerText;
                  XmlNode buildVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "build");
                  if (buildVersionNode != null)
                    versionBuild = buildVersionNode.InnerText;
                  XmlNode revisionVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "revision");
                  if (revisionVersionNode != null)
                    versionRevision = revisionVersionNode.InnerText;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("OnPageLoad(): Cannot read Current Skin Interface Version for skin '" + currentskin + "' - exception: " + ex.Message);
        versionMajor = "";
        versionMinor = "";
        versionBuild = "";
        versionRevision = "";
        iVersionMajor = 0;
        iVersionMinor = 0;
        iVersionBuild = 0;
        iVersionRevision = 0;
        success = false;
      }
      finally
      {
        doc = null;
      }
      if (!success) return false;
      if (!int.TryParse(versionMajor.Trim(), out iVersionMajor) || !int.TryParse(versionMinor.Trim(), out iVersionMinor)) return false;
      success = int.TryParse(versionBuild.Trim(), out iVersionBuild);
      success = int.TryParse(versionRevision.Trim(), out iVersionRevision);
      return true;
    }

    //private void GUIWindowManager_OnNewMessage(GUIMessage message)
    //{
    //  switch (message.Message)
    //  {
    //    case GUIMessage.MessageType.GUI_MSG_AUTOPLAY_VOLUME:
    //      if (message.Param1 == (int)Ripper.AutoPlay.MediaType.VIDEO)
    //      {
    //        if (message.Param2 == (int)Ripper.AutoPlay.MediaSubType.DVD)
    //          OnPlayDVD(message.Label, GetID);
    //        else if (message.Param2 == (int)Ripper.AutoPlay.MediaSubType.VCD ||
    //                  message.Param2 == (int)Ripper.AutoPlay.MediaSubType.FILES)
    //          OnPlayFiles((System.Collections.ArrayList)message.Object);
    //      }
    //      break;

    //    case GUIMessage.MessageType.GUI_MSG_VOLUME_REMOVED:
    //      if (g_Player.Playing && g_Player.IsVideo && message.Label.Equals(g_Player.CurrentFile.Substring(0, 2), StringComparison.InvariantCultureIgnoreCase))
    //      {
    //        Log.Info("GUIVideoFiles: Stop since media is ejected");
    //        g_Player.Stop();
    //        _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP).Clear();
    //      }
    //      break;
    //  }
    //}

    private bool SaveLastView()
    { return SaveLastView(string.Empty); }
    private bool SaveLastView(string viewname)
    {
      if (null == viewname) return false;

      LogMyFilms.Debug("SaveLastView() called with '" + viewname + "', current ViewHistory stack count is: '" + ViewHistory.Count + "'");
      // Configuration conf = new Configuration();
      var state = new ViewState();

      state.Boolselect = conf.Boolselect;
      state.Boolreturn = conf.Boolreturn;
      state.Boolindexed = conf.Boolindexed;
      state.Boolindexedreturn = conf.Boolindexedreturn;
      state.IndexedChars = conf.IndexedChars;
      state.BoolReverseNames = conf.BoolReverseNames;
      state.BoolVirtualPathBrowsing = conf.BoolVirtualPathBrowsing;
      state.BoolShowEmptyValuesInViews = conf.BoolShowEmptyValuesInViews;

      state.StrSelect = conf.StrSelect;
      state.StrPersons = conf.StrPersons;
      state.StrTitleSelect = conf.StrTitleSelect;
      state.StrFilmSelect = conf.StrFilmSelect;
      state.ViewContext = conf.ViewContext;
      state.StrTxtView = conf.StrTxtView;
      state.StrTxtSelect = conf.StrTxtSelect;

      state.Wselectedlabel = conf.Wselectedlabel;
      state.WStrSort = conf.WStrSort;
      state.WStrSortSensCount = conf.WStrSortSensCount;
      state.BoolSortCountinViews = conf.BoolSortCountinViews;
      state.Wstar = conf.Wstar;

      state.StrLayOut = conf.StrLayOut;
      state.WStrLayOut = conf.WStrLayOut;
      state.StrLayOutInHierarchies = conf.StrLayOutInHierarchies;
      state.LastID = conf.LastID;

      try
      {
        state.IndexItem = facadeFilms != null && facadeFilms.SelectedItem > -1 ? (conf.Boolselect ? facadeFilms.SelectedListItemIndex : 0) : -1; //may need to check if there is no item selected and so save -1
        state.TitleItem = facadeFilms != null && facadeFilms.SelectedItem > -1 ? (conf.Boolselect ? facadeFilms.SelectedItem.ToString() : facadeFilms.SelectedListItem.Label) : string.Empty; //may need to check if there is no item selected and so save ""
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("SaveLastView() - exception on facade values: " + ex.Message);
        state.IndexItem = -1;
        state.TitleItem = string.Empty;
      }
      if (!string.IsNullOrEmpty(viewname))
      {
        if (_viewStateCache.ContainsKey(viewname)) _viewStateCache.Remove(viewname);
        _viewStateCache.Add(viewname, state);
        LogMyFilms.Debug("SaveLastView() - saved viewstate for '" + viewname + "'");
      }
      else
      {
        // ViewHistory.Add(state);
        ViewHistory.Push(state);
        LogMyFilms.Debug("SaveLastView() - saved state to history stack - stack count is now: '" + ViewHistory.Count + "'");
      }
      return true;
    }

    private bool RestoreLastView(string viewname = "")
    {
      LogMyFilms.Debug("RestoreLastView() called with '" + viewname + "', current ViewHistory stack count is: '" + ViewHistory.Count + "'");
      ViewState state = new ViewState();
      if (!string.IsNullOrEmpty(viewname))
      {
        if (_viewStateCache.TryGetValue(viewname, out state))
        {
          conf.Boolselect = state.Boolselect;
          conf.Boolreturn = state.Boolreturn;
          conf.Boolindexed = state.Boolindexed;
          conf.Boolindexedreturn = state.Boolindexedreturn;
          conf.IndexedChars = state.IndexedChars;
          // conf.BoolReverseNames = state.BoolReverseNames; don't restore for Generic view state cache, as it is more a "global" setting
          // conf.BoolShowEmptyValuesInViews = state.BoolShowEmptyValuesInViews;

          //conf.StrSelect = state.StrSelect;
          //conf.StrPersons = state.StrPersons;
          //conf.StrTitleSelect = state.StrTitleSelect;
          //conf.StrFilmSelect = state.StrFilmSelect;
          //conf.ViewContext = state.ViewContext;
          //conf.StrTxtView = state.StrTxtView;
          //conf.StrTxtSelect = state.StrTxtSelect;

          conf.Wselectedlabel = state.Wselectedlabel;
          conf.WStrSort = state.WStrSort;
          conf.WStrSortSensCount = state.WStrSortSensCount;
          conf.BoolSortCountinViews = state.BoolSortCountinViews;
          conf.Wstar = state.Wstar;

          conf.StrLayOut = state.StrLayOut;
          conf.WStrLayOut = state.WStrLayOut;
          conf.StrLayOutInHierarchies = state.StrLayOutInHierarchies;
          conf.LastID = state.LastID;

          int IndexItem = state.IndexItem;
          string TitleItem = state.TitleItem;

          //IndexItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"); //may need to check if there is no item selected and so save -1
          //TitleItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel) : string.Empty); //may need to check if there is no item selected and so save ""
          LogMyFilms.Debug("SaveLastView() - restored viewstate for '" + viewname + "'");
          return true;
        }
        else return false;
      }
      else if (ViewHistory.Count > 0)
      {
        // state = ViewHistory.Last();
        state = ViewHistory.Pop() as ViewState;
        conf.Boolselect = state.Boolselect;
        conf.Boolreturn = state.Boolreturn;
        conf.Boolindexed = state.Boolindexed;
        conf.Boolindexedreturn = state.Boolindexedreturn;
        conf.IndexedChars = state.IndexedChars;
        conf.BoolReverseNames = state.BoolReverseNames;
        conf.BoolVirtualPathBrowsing = state.BoolVirtualPathBrowsing;
        conf.BoolShowEmptyValuesInViews = state.BoolShowEmptyValuesInViews;

        conf.StrSelect = state.StrSelect;
        conf.StrPersons = state.StrPersons;
        conf.StrTitleSelect = state.StrTitleSelect;
        conf.StrFilmSelect = state.StrFilmSelect;
        conf.ViewContext = state.ViewContext;
        conf.StrTxtView = state.StrTxtView;
        conf.StrTxtSelect = state.StrTxtSelect;

        conf.Wselectedlabel = state.Wselectedlabel;
        conf.WStrSort = state.WStrSort;
        conf.WStrSortSensCount = state.WStrSortSensCount;
        conf.BoolSortCountinViews = state.BoolSortCountinViews;
        conf.Wstar = state.Wstar;

        conf.StrLayOut = state.StrLayOut;
        conf.WStrLayOut = state.WStrLayOut;
        conf.StrLayOutInHierarchies = state.StrLayOutInHierarchies;
        conf.LastID = state.LastID;

        try { conf.StrIndex = state.IndexItem; }
        catch { conf.StrIndex = -1; }
        conf.StrTIndex = state.TitleItem;

        // ViewHistory.Remove(ViewHistory.Last());
        LogMyFilms.Debug("RestoreLastView() ViewHistory after restore is: '" + ViewHistory.Count + "'");
        return true;
      }
      return false;
    }

    private void DoBack()
    {
      LogMyFilms.Debug("DoBack() - Called with '" + _navigationStack.Count + "' items in navigation stack.");
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();

      // stop facade background worker
      StopLoadingViewDetails = true;
      StopLoadingMenuDetails = true;
      StopLoadingFilmlistDetails = true;

      if (_navigationStack.Count > 0)
      {
        GUIControl.ClearControl(GetID, facadeFilms.GetID);
        //Fanartstatus(false);
        //currentFanartList.Clear();
        MyFilmsDetail.Init_Detailed_DB(false); // clear all GUI properties
        Clear_Logos();

        NavigationObject obj = _navigationStack.Pop() as NavigationObject;


        #region restore state properties
        _mapSettings.ViewAs = (int)obj.CurrentView;
        // obj.SetCoverStatus(ref menucover, ref filmcover, ref viewcover, ref personcover, ref groupcover);
        menucover.Filename = obj.CoverStatus.MenuCover;
        // filmcover.Filename = obj.CoverStatus.FilmCover; // do not reset filmcover, as this "breaks" animations 
        viewcover.Filename = obj.CoverStatus.ViewCover;
        personcover.Filename = obj.CoverStatus.PersonCover;
        groupcover.Filename = obj.CoverStatus.GroupCover;
        conf.DbSelection = new string[] { obj.DbDfltSelect, obj.DbSelect, obj.DbField, obj.DbSort, obj.DbShowAll.ToString(), obj.DbExtraSort.ToString() };
        // Change_Layout_Action((int)obj.CurrentView); // switch here already the layout to BEFORE facade is populated !
        ShowPanel();  // switches to proper layout

        obj.SetViewStatus(conf); // sets the context environment
        obj.SetItems(facadeFilms); // populate facade with former content

        
        BtnSrtBy.IsEnabled = obj.SortButtonEnabled;
        BtnSrtBy.IsAscending = obj.SortButtonAsc;
        BtnSrtBy.Label = obj.SortButtonLabel;

        BtnViewAs.Label = obj.ViewAsButtonLabel;

        BtnLayout.Label = obj.LayoutButtonLabel;

        SetDummyControlsForFacade(conf.ViewContext); // set here in advance to make visibility conditions cahnge faster on back navigation

        facadeFilms.SelectedListItemIndex = obj.Position;
        Prev_ItemID = (facadeFilms == null || facadeFilms.SelectedListItemIndex == -1) ? -1 : facadeFilms.SelectedListItem.ItemId;
        Prev_Label = (facadeFilms == null || facadeFilms.SelectedListItemIndex == -1) ? string.Empty : facadeFilms.SelectedListItem.Label;
        LogMyFilms.Debug("DoBack() - restore facade position - SelectedListItemIndex = '" + obj.Position + "', Prev_ItemID = '" + Prev_ItemID + "', Prev_Label = '" + Prev_Label + "'");
        //Prev_ItemID = -1;
        //Prev_Label = string.Empty;

        // GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, obj.Position);
        GUIPropertyManager.SetProperty("#currentmodule", obj.Title);
        GUIPropertyManager.SetProperty("#itemtype", obj.ItemType);
        GUIPropertyManager.SetProperty("#itemcount", (facadeFilms != null) ? facadeFilms.Count.ToString() : "");
        GUIPropertyManager.SetProperty("#myfilms.nbobjects.value", obj.NbObjects);
        GUIPropertyManager.SetProperty("#myfilms.moviecontext.value", obj.MovieContext);

        SetLabelView(conf.StrTxtView);
        SetLabelSelect(conf.StrTxtSelect);

        if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll || conf.Boolselect)
          GUIControl.ShowControl(GetID, 34);
        else
          GUIControl.HideControl(GetID, 34);
        #endregion

        #region reload dataset threaded
        if (obj.LastDbUpdate < LastDbUpdate && conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll) // check if a full reload of the facade is required, e.g. due to watched status changes
        {
          #region restore facade by reloading facade
          LogMyFilms.Debug("DoBack() - DB was updated ! - cached date: '" + obj.LastDbUpdate.ToLongTimeString() + "', last update: '" + LastDbUpdate.ToLongTimeString() + "'");
          new Thread(delegate()
          {
            {
              //Loadfacade();
              //this.Refreshfacade();
              if (conf.Boolselect) // Groupviews / Persons
              {
                // Change_Layout_Action(MyFilms.conf.WStrLayOut);
                // SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
                GetSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, false, Prev_Label); // preserve index from last time
              }
              else
              {
                // Change_Layout_Action(MyFilms.conf.StrLayOut);
                // SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
                // conf.ViewContext = ViewContext.Movie;
                GetFilmList(Prev_Label);
              }
            }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              {
                Fanartstatus(true);
                //Prev_ItemID = -1;
                //Prev_Label = string.Empty;
                //ShowPanel();
                if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
                stopwatch.Stop();
                LogMyFilms.Debug("DoBack() - finished reloading dataset (" + stopwatch.ElapsedMilliseconds + " ms)");
              }
              return 0;
            }, 0, 0, null);
          }) { Name = "DoBackReloadDataset", IsBackground = true }.Start();
          #endregion
        }
        else
        {
          #region restore facade from navigation cache
          new Thread(delegate()
          {
            {
              if (Helper.FieldIsSet(obj.DbField))
                r = BaseMesFilms.ReadDataMovies(obj.DbDfltSelect, obj.DbSelect, obj.DbField, obj.DbSort, obj.DbShowAll, obj.DbExtraSort);
              // r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
            }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              {
                Fanartstatus(true);
                Prev_ItemID = -1;
                Prev_Label = string.Empty;
                ShowPanel();
                if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // make sure, the details publisher is called !
                stopwatch.Stop();
                LogMyFilms.Debug("DoBack() - finished reloading dataset (" + stopwatch.ElapsedMilliseconds + " ms)");
                if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) GetCountsForMenuView();
              }
              return 0;
            }, 0, 0, null);
          }) { Name = "DoBackReloadDataset", IsBackground = true }.Start();
          #endregion
        }
        #endregion

      }
      // stopwatch.Stop();
      LogMyFilms.Debug("DoBack() - finished restoring previous navigation cache (" + stopwatch.ElapsedMilliseconds + " ms)");
    }

    void ShowPanel()
    {
      //  case 0:
      //    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
      //    this.facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
      //    break;

      //List = 0,
      //SmallIcons = 1,
      //LargeIcons = 2,
      //Filmstrip = 3,
      //AlbumView = 4,
      //Playlist = 5,
      //CoverFlow = 6


      int itemIndex = facadeFilms.SelectedListItemIndex;
      switch (_mapSettings.ViewAs)
      {
        case (int)Layout.LargeIcons:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
        case (int)Layout.AlbumView:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.AlbumView;
          break;
        case (int)Layout.SmallIcons:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
          break;
        case (int)Layout.List:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
        case (int)Layout.Filmstrip:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
          break;
        case (int)Layout.CoverFlow:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
          break;
        default:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
      }
      if (itemIndex > -1)
      {
        GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
      }
    }

    private void SaveListState(bool clear)
    {
      var stopwatch = new Stopwatch(); stopwatch.Reset(); stopwatch.Start();
      // stop all background worker, as we save current state now
      StopLoadingMenuDetails = true;
      StopLoadingViewDetails = true;
      StopLoadingFilmlistDetails = true;
      
      if (facadeFilms.ListLayout.ListItems.Count > 0)
      {
        _mapSettings.ViewAs = (int)(Layout)facadeFilms.CurrentLayout;
        _navigationStack.Push(
          new MyFilmsPlugin.Utils.NavigationObject(
            facadeFilms.ListLayout,
            GUIPropertyManager.GetProperty("#currentmodule"),
            GUIPropertyManager.GetProperty("#itemtype"),
            GUIPropertyManager.GetProperty("#myfilms.nbobjects.value"),
            GUIPropertyManager.GetProperty("#myfilms.moviecontext.value"),
            facadeFilms.SelectedListItemIndex,
            (Layout)_mapSettings.ViewAs,
            conf,
            BtnSrtBy,
            BtnViewAs,
            BtnLayout,
            new CoverState(menucover.Filename, filmcover.Filename, viewcover.Filename, personcover.Filename, groupcover.Filename),
            LastDbUpdate
            ));
      }
      if (clear)
      {
        // ClearLabels("Curent");
        GUIControl.ClearControl(GetID, facadeFilms.GetID);
        //MyFilms.temp_player.Reset();
        //MyFilms.temp_player.GetPlaylist(PlayListType.PLAYLIST_MUSIC_VIDEO).Clear();
      }
      stopwatch.Stop();
      LogMyFilms.Debug("SaveListState() - finished saving current state to navigation cache (clear = '" + clear + "') (" + (stopwatch.ElapsedMilliseconds) + " ms)");
    }

    internal static bool AddTrailerToDownloadQueue(Trailer trailer)
    {
      bool added = false;
      // we need to get it, let's queue them up and download in the background
      lock (TrailertoDownloadQueue)
      {
        if (TrailertoDownloadQueue.Any(trl => trl.OriginalUrl == trailer.OriginalUrl && trl.DestinationDirectory == trailer.DestinationDirectory))
        {
          LogMyFilms.Debug("AddTrailerToDownloadQueue() - items = '" + TrailertoDownloadQueue.Count + "' - not added trailer for movie (dupe) '" + trailer.MovieTitle + "', DestinationDir = '" + trailer.DestinationDirectory + "'");
          return false;
        }
        if (!TrailertoDownloadQueue.Contains(trailer))
        {
          TrailertoDownloadQueue.Enqueue(trailer);
          LogMyFilms.Debug("AddTrailerToDownloadQueue() - items = '" + TrailertoDownloadQueue.Count + "' - added trailer for movie '" + trailer.MovieTitle + "', DestinationDir = '" + trailer.DestinationDirectory + "'"); // , sourceUrl = '" + trailer.SourceUrl + "'
          added = true;
        }
      }
      if (!bgDownloadTrailer.IsBusy) bgDownloadTrailer.RunWorkerAsync();
      return added;
    }
  
  }


  //public class GUIMyFilmsListItem : GUIListItem
  //{
  //  #region Facade Item
  //  public GUIMyFilmsListItem(string strLabel) : base(strLabel) { }

  //  public object Item
  //  {
  //    get { return _Item; }
  //    set
  //    {
  //      _Item = value;
  //      INotifyPropertyChanged notifier = value as INotifyPropertyChanged;
  //      if (notifier != null)
  //      {
  //        notifier.PropertyChanged += (s, e) =>
  //        {
  //          if (s is DBActor && e.PropertyName == "ThumbnailImage")
  //            SetImageToGui((s as DBActor).ThumbnailImage);
  //        };
  //      }
  //    }
  //  } protected object _Item;

  //  protected void SetImageToGui(string imageFilePath)
  //  {
  //    if (string.IsNullOrEmpty(imageFilePath)) return;

  //    string texture = GetTextureFromFile(imageFilePath);

  //    if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(imageFilePath), texture, 0, 0, 0) > 0)
  //    {
  //      ThumbnailImage = texture;
  //      IconImage = texture;
  //      IconImageBig = texture;
  //    }

  //    // if selected and GUIActors is current window force an update of thumbnail
  //    GUIActors actorWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow) as GUIActors;
  //    if (actorWindow != null)
  //    {
  //      GUIListItem selectedItem = GUIControl.GetSelectedListItem(9816, 50);
  //      if (selectedItem == this)
  //      {
  //        GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
  //      }
  //    }
  //  }

  //  private string GetTextureFromFile(string filename)
  //  {
  //    return "[MyFilms:" + filename.GetHashCode() + "]";
  //  }

  //  #endregion
  //}


  public class GUIMovieListItem : GUIListItem
  {
    public GUIMovieListItem(string strLabel) : base(strLabel) { }

    public object Item
    {
      get { return _Item; }
      set
      {
        _Item = value;
        var notifier = value as INotifyPropertyChanged;
        if (notifier != null) notifier.PropertyChanged += (s, e) =>
        {
          //if (s is TraktMovie.MovieImages && e.PropertyName == "PosterImageFilename")
          //  SetImageToGui((s as TraktMovie.MovieImages).PosterImageFilename);
          //if (s is TraktMovie.MovieImages && e.PropertyName == "FanartImageFilename")
          //  UpdateCurrentSelection();
        };
      }
    } protected object _Item;

    /// <summary>
    /// Loads an Image from memory into a facade item
    /// </summary>
    /// <param name="imageFilePath">Filename of image</param>
    protected void SetImageToGui(string imageFilePath)
    {
      if (string.IsNullOrEmpty(imageFilePath)) return;

      // determine the overlay to add to poster
      // [...]
      // get a reference to a MediaPortal Texture Identifier
      //string suffix = mainOverlay.ToString().Replace(", ", string.Empty) + Enum.GetName(typeof(RatingOverlayImage), ratingOverlay);
      //string texture = GUIImageHandler.GetTextureIdentFromFile(imageFilePath, suffix);

      // build memory image
      //Image memoryImage = null;
      //if (mainOverlay != MainOverlayImage.None || ratingOverlay != RatingOverlayImage.None)
      //{
      //  memoryImage = GUIImageHandler.DrawOverlayOnPoster(imageFilePath, mainOverlay, ratingOverlay);
      //  if (memoryImage == null) return;

      //  // load texture into facade item
      //  if (GUITextureManager.LoadFromMemory(memoryImage, texture, 0, 0, 0) > 0)
      //  {
      //    ThumbnailImage = texture;
      //    IconImage = texture;
      //    IconImageBig = texture;
      //  }
      //}
      //else
      //{
      //  ThumbnailImage = imageFilePath;
      //  IconImage = imageFilePath;
      //  IconImageBig = imageFilePath;
      //}

      // if selected and is current window force an update of thumbnail
      UpdateCurrentSelection();
    }

    protected void UpdateCurrentSelection()
    {
      var window = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
      if (window != null)
      {
        GUIListItem selectedItem = GUIControl.GetSelectedListItem(MyFilms.ID_MyFilms, 50);
        if (selectedItem == this)
        {
          GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
        }
      }
    }
  }

}