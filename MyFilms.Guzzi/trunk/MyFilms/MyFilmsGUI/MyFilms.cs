﻿#region GNU license
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

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;

  using MediaPortal.Configuration;
  using MediaPortal.Dialogs;
  using MediaPortal.GUI.Library;
  using MediaPortal.Services;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin.MyFilms.Configuration;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.MyFilms.Utils.Cornerstone.MP;

  using NLog;
  using NLog.Config;
  using NLog.Targets;

  using Trakt;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;
  using ImageFast = MyFilmsPlugin.MyFilms.Utils.ImageFast;

  /// <summary>
    /// Summary description for GUIMyFilms.
    /// </summary>
    //[PluginIcons("MesFilms.MyFilms.Resources.clapperboard-128x128.png", "MesFilms.MyFilms.Resources.clapperboard-128x128-faded.png")]
    //[PluginIcons("MesFilms.MyFilms.Resources.logo_mesfilms.png", "MesFilms.MyFilms.Resources.logo_mesfilms-faded.png")]
    [PluginIcons("MyFilmsPlugin.MyFilms.Resources.film-reel-128x128.png", "MyFilmsPlugin.MyFilms.Resources.film-reel-128x128-faded.png")]
    public class MyFilms : GUIWindow, ISetupForm
    {
      #region Constructor
      public MyFilms()
      {
        // create Backdrop image swapper
        backdrop = new ImageSwapper();
        backdrop.ImageResource.Delay = 250;
        backdrop.PropertyOne = "#myfilms.fanart";
        backdrop.PropertyTwo = "#myfilms.fanart2";
        backdrop.LoadingImage = loadingImage;
        backdrop.Active = false;

        // create Cover image swapper
        cover = new AsyncImageResource();
        cover.Property = "#myfilms.coverimage";
        cover.Delay = 125;
      }
      #endregion

      #region ISetupForm Members

      // Returns the name of the plugin which is shown in the plugin menu
      public string PluginName()
      {
        return "MyFilms";
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
        string wPluginName = strPluginName;
        using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
        {
          wPluginName = xmlreader.GetValueAsString("MyFilms", "PluginName", "MyFilms");
        }

        strButtonText = wPluginName;
        strButtonImage = String.Empty;
        strButtonImageFocus = String.Empty;
        strPictureImage = String.Format("hover_{0}.png", "MyFilms");
        string strBtnFile = String.Format(@"{0}\media\{1}", GUIGraphicsContext.Skin, strPictureImage);
        if (!System.IO.File.Exists(strBtnFile))
          strPictureImage = string.Empty;
        return true;
      }
      #endregion


      /*
       * Log declarations
       */
        private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();  //log
        private const string LogFileName = "MyFilms.log";  //log's filename
        private const string OldLogFileName = "MyFilms.old.log";  //log's old filename
        
        //private BaseMesFilms films;
        #region Descriptif zones Ecran

        public const int ID_MyFilms = 7986;
        public const int ID_MyFilmsDetail = 7987;
        public const int ID_MyFilmsDialogRating = 7988;
        public const int ID_MyFilmsActors = 7989;
        public const int ID_MyFilmsThumbs = 7990;
        public const int ID_MyFilmsActorsInfo = 7991;
        public const int ID_BrowseTheWeb = 54537689;
        public const int ID_OnlineVideos = 4755;

        public const string ImdbBaseUrl = "http://www.imdb.com/";

        enum Controls : int
        {
            CTRL_BtnSrtBy = 2,
            CTRL_BtnViewAs = 3,
            CTRL_BtnSearchT = 4,
            CTRL_BtnOptions = 5,
            CTRL_BtnLayout = 6,
            CTRL_GlobalOverlayFilter = 7,
            CTRL_ToggleGlobalUnwatchedStatus = 8,
            //CTRL_TxtSelect = 12,
            CTRL_Fanart = 11,
            CTRL_Fanart2 = 21,
            CTRL_LoadingImage = 22,
            CTRL_Image = 1020,
            CTRL_Image2 = 1021,
            CTRL_List = 50, // Changed from 1026 to 50 due to meeting MePo Standards
            CTRL_logos_id2001 = 2001,
            CTRL_logos_id2002 = 2002,
            CTRL_logos_id2003 = 2003,
            CTRL_logos_id2012 = 2012,
            CTRL_GuiWaitCursor = 3004,
        }

        #region Skin Variables
        //[SkinControlAttribute((int)Controls.CTRL_TxtSelect)]
        //protected GUIFadeLabel TxtSelect;

        [SkinControlAttribute((int)Controls.CTRL_BtnSrtBy)]
        protected GUISortButtonControl BtnSrtBy;

        [SkinControlAttribute((int)Controls.CTRL_GlobalOverlayFilter)]
        protected GUIButtonControl BtnGlobalOverlayFilter;

        //[SkinControlAttribute((int)Controls.CTRL_ToggleGlobalUnwatchedStatus)]
        //protected GUIButtonControl BtnToggleGlobalWatched;

        [SkinControlAttribute((int)Controls.CTRL_List)]
        protected GUIFacadeControl facadeView;

        [SkinControlAttribute((int)Controls.CTRL_Image)]
        protected GUIImage ImgLstFilm;

        [SkinControlAttribute((int)Controls.CTRL_Image2)]
        protected GUIImage ImgLstFilm2;

        [SkinControlAttribute((int)Controls.CTRL_logos_id2001)]
        protected GUIImage ImgID2001;

        [SkinControlAttribute((int)Controls.CTRL_logos_id2002)]
        protected GUIImage ImgID2002;

        [SkinControlAttribute((int)Controls.CTRL_logos_id2003)]
        protected GUIImage ImgID2003;

        [SkinControlAttribute((int)Controls.CTRL_logos_id2012)]
        protected GUIImage ImgID2012;

        [SkinControlAttribute((int)Controls.CTRL_Fanart)]
        protected GUIImage ImgFanart;

        [SkinControlAttribute((int)Controls.CTRL_Fanart2)]
        protected GUIImage ImgFanart2;

        [SkinControlAttribute((int)Controls.CTRL_LoadingImage)]
        protected GUIImage loadingImage;

        [SkinControlAttribute((int)Controls.CTRL_GuiWaitCursor)]
        protected GUIAnimation m_SearchAnimation;
        #endregion

        public int Layout = 0;
        public static int Prev_ItemID = -1;
        //Added to jump back to correct Menu (Either Basichome or MyHome - or others...)
        public static int Prev_MenuID = -1;
        public bool Context_Menu = false;
        public static Configuration conf;
        public static Logos confLogos;
        //private string currentConfig;
        private string strPluginName = "MyFilms";
        public static DataRow[] r; // will hold current recordset to traverse

        //Imageswapperdefinitions for fanart and cover
        private ImageSwapper backdrop;
        private AsyncImageResource cover = null;

        // Guzzi: Added from TV-Series for Fanarttoggling
        private System.Threading.Timer m_FanartTimer = null;
        // private System.Threading.Timer m_TraktSyncTimer = null;

        private bool m_bFanartTimerDisabled = false;

        //Guzzi Addons for Global nonpermanent Trailer and MinRating Filters
        public bool GlobalFilterTrailersOnly = false;
        public bool GlobalFilterMinRating = false;
        //public string GlobalFilterString = string.Empty;
        public string GlobalFilterStringTrailersOnly = string.Empty;
        public string GlobalFilterStringUnwatched = string.Empty;
        public string GlobalFilterStringMinRating = string.Empty;
        //public string GlobalUnwatchedFilterString = string.Empty;
        public bool MovieScrobbling = false;
        public int actorID = 0;
        public static string CurrentMovie;
        //public static string CurrentFanartDir;
        public enum optimizeOption { optimizeDisabled };
        public static bool InitialStart = false; //Added to implement InitialViewSetup, ToDo: Add Logic
        private bool LoadWithParameterSupported = false;

        public bool BrowseTheWebRightPlugin = false;
        public bool BrowseTheWebRightVersion = false;
        public static bool OnlineVideosRightPlugin = false;
        public static bool OnlineVideosRightVersion = false;
        private double lastPublished = 0;
        private Timer publishTimer;


        #endregion


        #region handler and backgroundworker
  
        public delegate void FilmsStoppedHandler(int stoptime, string filename);
        public delegate void FilmsEndedHandler(string filename);   
        System.ComponentModel.BackgroundWorker bgUpdateDB = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker bgUpdateFanart = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker bgUpdateTrailer = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker bgLoadMovieList = new System.ComponentModel.BackgroundWorker();

        #endregion


        #region Base Overrides

        public override int GetID
        {
            get {return ID_MyFilms;}
        }

        public override string GetModuleName()
        {
          return GUILocalizeStrings.Get(ID_MyFilms); // return localized string for Module ID
        }

        public override bool Init()
        //This Method is only loaded ONCE when starting Mediaportal !!!
        {
            InitLogger(); // Initialize Logger 
            Log.Info("MyFilms.Init() started. See MyFilms.log for further Details.");
            LogMyFilms.Debug("MyFilms.Init() started.");

            // Set Variable for FirstTimeView Setup
            InitialStart = true;

            //Add localized labels for DB Columns
            InitGUIPropertyLabels();

            // check if running version of mediaportal support loading with parameter           
            //if (typeof(GUIWindow).GetField("_loadParameter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null)
            //{
            //  LoadWithParameterSupported = true;
            //}

            #region Trakt
            //TraktAPI.Username = conf.StrTraktUsername;
            //TraktAPI.Password = conf.StrTraktPassword;
            //TraktAPI.UserAgent = MyFilmsSettings.UserAgent;

            //LogMyFilms.Debug("MF: Trakt Usersettings loaded - UserName : '" + TraktAPI.Username.ToString() + "'");
            //LogMyFilms.Debug("MF: Trakt Usersettings loaded - Useragent: '" + TraktAPI.UserAgent.ToString() + "'");

            // Timer to process episodes to send to trakt, will also be called after new episodes are added to library
            //m_TraktSyncTimer = new System.Threading.Timer(new TimerCallback(TraktSynchronize), null, 15000, Timeout.Infinite);
            #endregion

            LogMyFilms.Debug("MyFilms.Init() completed. Loading main skin file.");

            return Load(GUIGraphicsContext.Skin + @"\MyFilms.xml");
        }

        public override void DeInit()
        {
          base.DeInit();
          // Add Other Classes here, if necessary
        }

        protected override void OnPageLoad() //This is loaded each time, the plugin is entered - can be used to reset certain settings etc.
        {
            LogMyFilms.Debug("MyFilms.OnPageLoad() started.");
            Log.Debug("MyFilms.OnPageLoad() started. See MyFilms.log for further Details.");

            // Support for StartParameters - ToDo: Add start view options (implementation)
            string jumpToViewName = null;
            if (LoadWithParameterSupported)
            {
              jumpToViewName = GetJumpToViewName();
            }

            // (re)link our backdrop image controls to the backdrop image swapper
            backdrop.GUIImageOne = ImgFanart;
            backdrop.GUIImageTwo = ImgFanart2;
            backdrop.LoadingImage = loadingImage;  // --> Do NOT activate - otherwise coverimage flickers and goes away !!!!

            // Setup Random Fanart Timer
            //m_FanartTimer = new System.Threading.Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);
            //m_bFanartTimerDisabled = true;
            //m_FanartTimer.Change(0,10000);

            //MyFilmsDetail.clearGUIProperty("picture");
            if (!BrowseTheWebRightPlugin || !BrowseTheWebRightVersion)
            {
              BrowseTheWebRightPlugin = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb");
              BrowseTheWebRightVersion = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb" && plugin.GetType().Assembly.GetName().Version.Minor >= 0);
              LogMyFilms.Debug("MyFilms.Init() - BrowseTheWebRightVersion = '" + BrowseTheWebRightVersion + "', BrowseTheWebRightVersion = '" + BrowseTheWebRightVersion + "'");
            }
            if (!OnlineVideosRightPlugin || !OnlineVideosRightVersion)
            {
              OnlineVideosRightPlugin = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos");
              OnlineVideosRightVersion = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos" && plugin.GetType().Assembly.GetName().Version.Minor > 27);
              LogMyFilms.Debug("MyFilms.Init() - OnlineVideosRightPlugin = '" + OnlineVideosRightPlugin + "', OnlineVideosRightVersion = '" + OnlineVideosRightVersion + "'");
            }
            LogMyFilms.Debug("MyFilms.OnPageLoad() completed.");
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            LogMyFilms.Debug("MyFilms.OnPageDestroy() started.");

            // Disable Random Fanart Timer
            //m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
            //m_bFanartTimerDisabled = true;

            base.OnPageDestroy(new_windowId);

            LogMyFilms.Debug("MyFilms.OnPageDestroy() completed.");
            Log.Debug("MyFilms.OnPageDestroy() completed. See MyFilms.log for further Details.");
        }

        #endregion

        #region Action
        
        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------

        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            LogMyFilms.Debug("MF: OnAction " + actionType.wID.ToString());
            //LogMyFilms.Debug("MF: : PreviousWindowId: '" + PreviousWindowId.ToString() + "' - Prev_MenuID: '" + Prev_MenuID.ToString() + "'");
            //LogMyFilms.Debug("MF: : Select: '" + conf.Boolselect + "' - View: '" + conf.Boolview + "' - Return: '" + conf.Boolreturn + "'");
            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR)
                if (GetPrevFilmList()) return;

            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) && (conf.Boolselect || conf.Boolview))
            {
                Change_LayOut(MyFilms.conf.StrLayOut);
                if (GetPrevFilmList()) return;
            }
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) && (conf.Boolreturn))
            {
                conf.Boolreturn = false;
                if (conf.WStrSort.ToString() == "ACTORS") // Removed "ToUpper"
                    if (GetPrevFilmList())
                        return;
                    else
                        base.OnAction(actionType);
                Change_view(conf.WStrSort.ToLower());
                return;
            }
            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                if (GetPrevFilmList())
                    return;
                else
                {
                    //GUIWindowManager.ShowPreviousWindow();
                    //Fix to not only always return to MyHome, e.g. when coming from Basichome...
                    //GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                    //if (Prev_MenuID != -1)
                    LogMyFilms.Debug("MF: (GuzziFix for Previous Window - Prev_MenuID: '" + Prev_MenuID + "'");
                    GUIWindowManager.ActivateWindow(Prev_MenuID); 
                    return;
                }
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
                if (facadeView.SelectedListItemIndex > -1)
                    {
                        if (!(facadeView.Focus)) GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
                        return;
                    }

            if (actionType.m_key != null)
            {
                if ((actionType.m_key.KeyChar == 112) && facadeView.Focus && !facadeView.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
                {
                    MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                }
                if ((actionType.m_key.KeyChar == 120) && Context_Menu)
                {
                    Context_Menu = false;
                    return;
                }
                if (actionType.m_key.KeyChar == 120 && facadeView.Focus && !facadeView.SelectedListItem.IsFolder)
                {
                    // context menu for update or suppress entry
                    Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
                    return;
                }
            }

            if (actionType.wID.ToString().Substring(0, 6) == "REMOTE")
                return;
            base.OnAction(actionType);
        }
        //---------------------------------------------------------------------------------------
        //   Handle posted Messages
        //---------------------------------------------------------------------------------------
        public override bool OnMessage(GUIMessage messageType)
        {
            int dControl = messageType.TargetControlId;
            int iControl = messageType.SenderControlId;
            switch (messageType.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    //---------------------------------------------------------------------------------------
                    // Windows Init
                    //---------------------------------------------------------------------------------------
                    LogMyFilms.Debug("MF: GUIMessage: GUI_MSG_WINDOW_INIT - Start");
                    base.OnMessage(messageType);
                    //Hier muß irgendwie gemerkt werden, daß eine Rückkehr vom TrailerIsAvailable erfolgt - CheckAccess WIndowsID des Conterxts via LOGs
                    if ((PreviousWindowId != ID_MyFilmsDetail) && !MovieScrobbling && (PreviousWindowId != ID_MyFilmsActors) && (PreviousWindowId != ID_OnlineVideos) && (PreviousWindowId != ID_BrowseTheWeb))
                    {
                        Prev_MenuID = PreviousWindowId;
                        InitMainScreen();
                        Configuration.Current_Config();
                        Load_Config(Configuration.CurrentConfig, true);
                        if (MyFilms.conf.StrFanart)
                            backdrop.Active = true;
                        else
                            backdrop.Active = false;
                    }
                    if ((Configuration.CurrentConfig == null) || (Configuration.CurrentConfig.Length == 0))
                    {
                      GUIWindowManager.ShowPreviousWindow();
                      GUIWaitCursor.Hide();
                      return true;
                    }

                    // Originally Deactivated by Zebons    
                    // ********************************
                    // ToDo: Crash on Details to be fixed (make it threadsafe !!!!!!!)
                    //if (!bgLoadMovieList.IsBusy)
                    //{
                    //  LogMyFilms.Debug("MF: Launching AsynLoadMovieList");
                    //  AsynLoadMovieList();
                    //}
                    // ********************************
                    // Originally Deactivated by Zebons    
                    
                    
                    GUIControl.ShowControl(GetID, 34);
                    GUIWaitCursor.Hide();

                    if (((conf.AlwaysDefaultView) || (InitialStart)) && (PreviousWindowId != ID_MyFilmsDetail) && !MovieScrobbling && (PreviousWindowId != ID_MyFilmsActors) && (PreviousWindowId != ID_OnlineVideos) && (PreviousWindowId != ID_BrowseTheWeb))
                        Fin_Charge_Init(true,false);
                    else
                        Fin_Charge_Init(false, false);

                    LogMyFilms.Debug("MF: GUIMessage: GUI_MSG_WINDOW_INIT - End");
                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    LogMyFilms.Debug("MF: GUIMessage: GUI_MSG_WINDOW_DEINIT - Start");
                    GUITextureManager.CleanupThumbs();

                    if (Configuration.CurrentConfig != "")
                    {
                        if (facadeView == null || facadeView.SelectedListItemIndex == -1)
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
                        else
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                    }
                    //ImgFanart.SetFileName(string.Empty);
                    //ImgFanart2.SetFileName(string.Empty);

#if MP11
                    facadeView.Resources.Clear();
#else
                    facadeView.Clear();
#endif
                    facadeView.Clear();
                    //backdrop.PropertyOne = " ";
                    // added from MoPic
                    backdrop.Filename = string.Empty;
                    cover.Filename = string.Empty;

                    //Disable FanartTimer - already done on pagedestroy ...
                    //m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    //m_bFanartTimerDisabled = true;

                    LogMyFilms.Debug("MF: GUIMessage: GUI_MSG_WINDOW_DEINIT - End");
                    return true; // fall through to call base class?

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    //---------------------------------------------------------------------------------------
                    // Mouse/Keyboard Clicked
                    //---------------------------------------------------------------------------------------
                    LogMyFilms.Debug("MF: GUI_MSG_CLICKED recognized !"); 
                    if ((iControl == (int)Controls.CTRL_BtnSrtBy) && (conf.Boolselect))
                        // No change sort method and no searchs during select
                        return true;
                    if ((iControl == (int)Controls.CTRL_BtnSearchT) && (conf.Boolselect))
                        conf.Boolselect = false;
                    if (iControl == (int)Controls.CTRL_BtnSearchT)
                    // Search dialog search
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlg == null) return true;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
                        //Add Menuentries here

                        if (MyFilmsDetail.ExtendedStartmode("Global Random Movie Search"))
                        {
                          //Guzzi: RandomMovie Search added
                          dlg.Add(GUILocalizeStrings.Get(10798621));//Search global movies by randomsearch (singlesearch, areasearch)
                          choiceSearch.Add("randomsearch");
                        }

                        if (MyFilms.conf.StrSearchList[0].Length > 0)
                        {
                            dlg.Add(GUILocalizeStrings.Get(10798615));//Search global movies by property
                            choiceSearch.Add("globalproperty");
                        }

                        if (MyFilmsDetail.ExtendedStartmode("Global Search Movies by Areas"))
                        {
                          dlg.Add(GUILocalizeStrings.Get(10798645)); //Search global movies by areas
                          choiceSearch.Add("globalareas");
                        }

                        dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(342));//Title
                        choiceSearch.Add("title");

                        dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(344));//Actors
                        choiceSearch.Add("actors");
                        for (int i = 0; i < 2; i++)
                        {
                            if (MyFilms.conf.StrSearchItem[i] != "(none)" && MyFilms.conf.StrSearchItem[i].Length > 0)
                            {
                                if (MyFilms.conf.StrSearchText[i].Length == 0)
                                    dlg.Add(GUILocalizeStrings.Get(137) + " " + MyFilms.conf.StrSearchItem[i]);//Specific search with no text
                                else
                                    dlg.Add(GUILocalizeStrings.Get(137) + " " + MyFilms.conf.StrSearchText[i]);//Specific search  text
                                choiceSearch.Add(string.Format("search{0}", i.ToString()));
                            }
                        }
                        if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                        {
                            dlg.Add(GUILocalizeStrings.Get(1079866));//Search related movies by persons
                            choiceSearch.Add("analogyperson");
                        }
                        if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder && MyFilms.conf.StrSearchList[0].Length > 0)
                        {
                            dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
                            choiceSearch.Add("analogyproperty");
                        }
						
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        if (choiceSearch[dlg.SelectedLabel] == "analogyperson")
                        {
                            SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }

                        if (choiceSearch[dlg.SelectedLabel] == "analogyproperty")
                        {
							              //SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId, MesFilms.conf.StrSearchList); // This version takes properties from config - but should be all anyway ...
                            // Define Search Properties here (hardcoded)
                            //string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Source", "Borrower" };
                            //SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId, PropertyList);
                            SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId);
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        if (choiceSearch[dlg.SelectedLabel] == "randomsearch")
                        {
                            SearchMoviesbyRandomWithTrailer();
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        if (choiceSearch[dlg.SelectedLabel] == "globalareas")
                        {
                            SearchMoviesbyAreas();
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        if (choiceSearch[dlg.SelectedLabel] == "globalproperty")
                        {
							              SearchMoviesbyProperties(MyFilms.conf.StrSearchList);
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                        if (null == keyboard) return true;
                        keyboard.Reset();
                        keyboard.Text = "";
                        keyboard.DoModal(GetID);
                        if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                        {
                            switch (choiceSearch[dlg.SelectedLabel])
                            {
                                case "title":
                                    if (control_searchText(keyboard.Text))
                                    {
                                        conf.StrSelect = conf.StrTitle1.ToString() + " like '*" + keyboard.Text + "*'";
                                        //conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(369) + " [*" + keyboard.Text + @"*]";
                                        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(369) + " [*" + keyboard.Text + @"*]";
                                        conf.StrTitleSelect = "";
                                        GetFilmList();
                                    }
                                    else
                                        return false;
                                    break;
                                case "actors":
                                    if (control_searchText(keyboard.Text))
                                    {
                                        conf.WStrSort = "ACTORS";
                                        conf.Wselectedlabel = "";
                                        conf.WStrSortSens = " ASC";
                                        BtnSrtBy.IsAscending = true;
                                        conf.StrActors = keyboard.Text;
                                        getSelectFromDivx("Actors like '*" + keyboard.Text + "*'", conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");
                                    }
                                    else
                                        return false;
                                    break;
                                case "search0":
                                case "search1":
                                    int i = 0;
                                    if (choiceSearch[dlg.SelectedLabel] == "search1")
                                        i = 1;
                                    AntMovieCatalog ds = new AntMovieCatalog();
                                    if (control_searchText(keyboard.Text))
                                    {
                                        if (ds.Movie.Columns[conf.StrSearchItem[i].ToString()].DataType.Name == "string")
                                          conf.StrSelect = conf.StrSearchItem[i].ToString() + " like '*" + keyboard.Text + "*'";
                                        else
                                            conf.StrSelect = conf.StrSearchItem[i].ToString() + " = '" + keyboard.Text + "'";
                                        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + conf.StrSearchText[i] + " [*" + keyboard.Text + @"*]";
                                        conf.StrTitleSelect = "";
                                        GetFilmList();
                                    }
                                    else
                                        return false;
                                    break;
                            }
                        }
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();
                    }
                    if (iControl == (int)Controls.CTRL_BtnSrtBy)
                    // Choice of Sort Method
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlg == null) return true;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        System.Collections.Generic.List<string> choiceSort = new System.Collections.Generic.List<string>();
                        dlg.Add(GUILocalizeStrings.Get(103));//Title
                        dlg.Add(GUILocalizeStrings.Get(366));//Year
                        dlg.Add(GUILocalizeStrings.Get(104));//Date
                        dlg.Add(GUILocalizeStrings.Get(367));//Rating
                        choiceSort.Add("title");
                        choiceSort.Add("year");
                        choiceSort.Add("date");
                        choiceSort.Add("rating");
                        for (int i = 0; i < 2; i++)
                        {
                            if (conf.StrSort[i] != "(none)" && conf.StrSort[i].Length > 0)
                            {
                                dlg.Add(GUILocalizeStrings.Get(1079893) + " " + conf.StrTSort[i]);//Specific sort i
                                choiceSort.Add(string.Format("sort{0}",i.ToString()));
                            }
                        }
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        conf.StrIndex = 0;
                        switch (choiceSort[dlg.SelectedLabel])
                        {
                            case "title":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(103);
                                conf.StrSorta = conf.StrSTitle;
                                conf.StrSortSens = " ASC";
                                break;
                            case "year":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(366);
                                conf.StrSorta = "YEAR";
                                conf.StrSortSens = " DESC";
                                break;
                            case "date":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(621);
                                conf.StrSorta = "DateAdded";
                                conf.StrSortSens = " DESC";
                                break;
                            case "rating":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(367);
                                conf.StrSorta = "RATING";
                                conf.StrSortSens = " DESC";
                                break;
                            case "sort0":
                            case "sort1":
                                int i = 0;
                                if (choiceSort[dlg.SelectedLabel] == "sort1")
                                    i = 1;
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(1079893) + " " + conf.StrTSort[i];
                                conf.StrSorta = conf.StrSort[i];
                                conf.StrSortSens = " ASC";
                                break;
                        }
                        dlg.DeInit();
                        BtnSrtBy.Label = conf.CurrentSortMethod;
                        if (!conf.Boolselect)
                            GetFilmList();
                        else
                            getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.StrSorta, conf.StrSortSens, "*", true, "");
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnViewAs)
                    // Change Selected View
                    {
                        Change_Selection_type_Video();
                        return base.OnMessage(messageType);
                    }
                    if (iControl == (int)Controls.CTRL_BtnOptions)
                    // Change Selected Option
                    {
                        Change_Option();
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // Added to return to facade
                        return base.OnMessage(messageType);
                    }
                    if (iControl == (int)Controls.CTRL_GlobalOverlayFilter)
                    // Change Selected Option - call global filters directly
                    {
                        Change_Global_Filters();
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // Added to return to facade
                        return base.OnMessage(messageType);
                    }
                    if (iControl == (int)Controls.CTRL_ToggleGlobalUnwatchedStatus)
                    // Toggle global unwatched filter directly
                    {
                        MyFilms.conf.GlobalUnwatchedOnly = !MyFilms.conf.GlobalUnwatchedOnly;
                        if (conf.GlobalUnwatchedOnly)
                        {
                          GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
                          MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
                        }
                        else
                        {
                          GlobalFilterStringUnwatched = String.Empty;
                          MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
                        }
                        LogMyFilms.Info("MF: Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
                        Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                        // use this code later to set the label of the button
                        
                        //if (MyFilms.conf.GlobalUnwatchedOnly)
                        //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628));
                        //else
                        //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));

                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // Added to return to facade
                        return base.OnMessage(messageType);
                    } 
                    if ((iControl == (int)Controls.CTRL_BtnLayout) && !conf.Boolselect)
                    // Change Layout View
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlg == null) return true;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        dlg.Add(GUILocalizeStrings.Get(101));//List
                        dlg.Add(GUILocalizeStrings.Get(100));//Icons
                        dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
                        dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
#if MP11
#else
                        dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
#endif
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        conf.StrIndex = 0;
                        int wselectindex = facadeView.SelectedListItemIndex;
                        Change_LayOut(dlg.SelectedLabel);
                        MyFilms.conf.StrLayOut = dlg.SelectedLabel;
                        dlg.DeInit();
                        GetFilmList();
                        GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wselectindex);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        return base.OnMessage(messageType);
                    }

                    if (iControl == (int)Controls.CTRL_List)
                    {
                        if (facadeView.SelectedListItemIndex > -1)
                        {
                            if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
                            // New Window for detailed selected item information
                            {
                                conf.StrIndex = facadeView.SelectedListItem.ItemId;
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();

                                //Start: Added for MovieThumbs
                                CurrentMovie = String.Empty;
                                try
                                { CurrentMovie = (string)MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorage].ToString().Trim(); }
                                catch
                                { CurrentMovie = String.Empty; }
                                LogMyFilms.Debug("MF: PrepareThumbView: CurrentMovie = '" + CurrentMovie + "'");
                                //End: Added for MovieThumbs

                                //Start: Added for Timed Imagerswapper in Main View
                                //CurrentFanartDir = "";
                                //try
                                //{ CurrentFanartDir = (string)MesFilms.r[facadeView.SelectedListItem.ItemId][MesFilms.conf.StrStorage].ToString().Trim(); }
                                //catch
                                //{ CurrentFanartDir = ""; }
                                //LogMyFilms.Debug("MF: - Set CurrentFanartDir: = '" + CurrentFanartDir + "'");

                                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                            }
                            else
                            // View List as selected
                            {
                                conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                                Change_LayOut(MyFilms.conf.StrLayOut);
                                if (facadeView.SelectedListItem.IsFolder)
                                    conf.Boolreturn = false;
                                else
                                    conf.Boolreturn = true;
                                do
                                {
                                    if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                                    conf.StrTitleSelect += conf.Wselectedlabel;
                                } while (GetFilmList() == false); //keep calling while single folders found
                            }
                        }
                    }
                    return base.OnMessage(messageType);
            }
            return base.OnMessage(messageType);
        }
        #endregion


        /// <summary>Jumps to prev folder in FilmList  by modifying Selects and calling GetFilmList</summary>
        /// <returns>If returns false means cannot jump back any further, so caller must exit plugin to main menu.</returns>
        bool GetPrevFilmList()
        {
            Prev_ItemID = -1;
            string SelItem;
            if (conf.StrTitleSelect == "")
            {
              if (conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect) || conf.Boolview) //original code block refactored // 1079870 = "Selection"
                {
                    conf.Boolselect = false;
                    conf.Boolview = false;
                    conf.Boolreturn = false;
                    conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
                    conf.StrIndex = 0;
                    GetFilmList();
                    SetLabelView("all"); // if back on "root", show view as "movies"
                    SetLabelSelect("root");
                    return true; //jump back to main full list  
                }

              if (conf.StrTxtSelect == "" || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) //"All" or "Global Filter"
                {
                  return false; //this was already "root" view - so jumping back should leave the plugin !
                }
                else
                {   // Jump back to prev view_display (categorised by year, genre etc) // Removed ACTORS special handling
                  if (conf.WStrSort == "ACTORS")
                  {
                    conf.StrSelect = "Actors like '*" + conf.StrActors + "*'";
                    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
                    SelItem = NewString.StripChars(@"[]", conf.StrTxtSelect);
                    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, SelItem);
                  }
                    //else if (conf.WStrSort == "PRODUCER")
                    //  {
                    //    conf.StrSelect = "Producer like '*" + conf.StrActors + "*'";
                    //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
                    //    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, "");
                    //  }
                    //else if (conf.WStrSort == "DIRECTOR")
                    //  {
                    //    conf.StrSelect = "Director like '*" + conf.StrActors + "*'";
                    //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
                    //    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, "");
                    //  }
                    else
                      {
                      SelItem = NewString.StripChars(@"[]", conf.StrTxtSelect);
                      if (conf.WStrSort == "DateAdded")
                          getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", "Date", " DESC", "*", true, SelItem);
                      else
                          getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, SelItem);
                      conf.StrSelect = "";
                      }
                }
            }
            else
            {
                SelItem = NewString.NPosRight(conf.TitleDelim, conf.StrTitleSelect, -1, false, false); // get last substring
                if (NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) > 0)
                    conf.StrTitleSelect = NewString.NPosLeft(conf.TitleDelim, conf.StrTitleSelect, -1, false, false); //jump back a delim
                else
                    conf.StrTitleSelect = "";
                if (GetFilmList(SelItem) == false) // if single folder then call this func to jump back again
                    return GetPrevFilmList();
            }
            return true;
        }

        /// <summary>Sets StrFilmSelect up based on StrSelect, StrTitleSelect etc... </summary>
        static void SetFilmSelect()
        {
            string s = "";
            Prev_ItemID = -1;
            if (conf.Boolselect)
            {
                string sLabel = conf.Wselectedlabel;
                if ((conf.WStrSort == "Date") || (conf.WStrSort == "DateAdded"))
                    conf.StrSelect = "Date" + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'";
                else
                {
                    if (sLabel == "")
                        conf.StrSelect = conf.WStrSort + " is NULL";
                    else
                        conf.StrSelect = conf.WStrSort + " like '*" + sLabel.Replace("'", "''") + "*'";
                }
                conf.StrTxtSelect = "[" + sLabel + "]";
                conf.StrTitleSelect = "";
                conf.Boolselect = false;
            }
            else
            {
                conf.StrTxtSelect = NewString.NPosLeft(@"\", conf.StrTxtSelect, 1, false, false); //strip old path if any
                if (conf.StrTitleSelect != "") conf.StrTxtSelect += @"\" + conf.StrTitleSelect; // append new path if any
            }

            if (conf.StrSelect != "")
                s = conf.StrSelect + " And ";
            if (conf.StrTitleSelect != "") //' in blake's seven causes fuckup
                s = s + String.Format("{0} like '{1}{2}*'", conf.StrTitle1.ToString(), conf.StrTitleSelect.Replace("'", "''"), conf.TitleDelim);
            else
                s = s + conf.StrTitle1.ToString() + " not like ''";
            conf.StrFilmSelect = s;
            LogMyFilms.Debug("MF: (SetFilmSelect) - StrFilmSelect: '" + s + "'");
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
            SetFilmSelect();
            //ImgLstFilm.SetFileName("#myfilms.picture"); // Disabled because replaced by SpeedLoader
            //ImgLstFilm2.SetFileName("#myfilms.picture");
            string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
            r = BaseMesFilms.LectureDonnées(GlobalFilterString + conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false);
			      //r = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false);
            LogMyFilms.Debug("MF: (GetFilmList) - GlobalFilterString:          '" + GlobalFilterString + "'");
            LogMyFilms.Debug("MF: (GetFilmList) - conf.StrDfltSelect:          '" + conf.StrDfltSelect + "'");
            LogMyFilms.Debug("MF: (GetFilmList) - conf.StrFilmSelect:          '" + conf.StrFilmSelect + "'");
            LogMyFilms.Debug("MF: (GetFilmList) - conf.StrSorta:               '" + conf.StrSorta + "'");
            LogMyFilms.Debug("MF: (GetFilmList) - conf.StrSortSens:            '" + conf.StrSortSens + "'");
            //if (r.Length == 0)
            //{
            //    //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            //    //dlgOk.ClearAll();
            //    //dlgOk.SetLine(1, GUILocalizeStrings.Get(10798620));
            //    //dlgOk.SetLine(2, GUILocalizeStrings.Get(10798621));
            //    //dlgOk.DoModal(GetID);
            //    //DisplayAllMovies();
            //    GUIControl.HideControl(GetID, 34);
            //    InitMainScreen();
            //}

            int iCnt = 0;
            int DelimCnt = 0, DelimCnt2;
            GUIListItem item = new GUIListItem();
            string sTitle;
            string sFullTitle;
            string sPrevTitle = "";
            string SelItem = gSelItem.ToString();
            int iSelItem = -2;
            if (typeof(T) == typeof(int)) iSelItem = Int32.Parse(SelItem);

            // setlabels
            // TxtSelect.Label = (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"); // always show as though folder path using \ regardless what sep is used
            MyFilmsDetail.setGUIProperty("select", (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));// always show as though folder path using \ regardless what sep is used

            BtnSrtBy.IsAscending = (conf.StrSortSens == " ASC");
            BtnSrtBy.Label = conf.CurrentSortMethod;

            if (conf.StrTitleSelect != "") DelimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title
            facadeView.Clear();
            //----------------------------------------------------------------------------------------
            // Load the DataSet.
            int number = -1;
            int wfacadewiew = 0;
            ArrayList w_tableau = new ArrayList();


            //// Guzzi: Testloop for Debugging ...
            //LogMyFilms.Debug("MF: (GuzziPerfCount) - Start");
            //int i = 0;
            //int ii = 0;
            //string strThumbT;
            //foreach (DataRow sr in r)
            //{
            //    sTitle = sr[conf.StrTitle1].ToString();
            //    DelimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
            //    if (DelimCnt <= DelimCnt2)
            //    {
            //        sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, DelimCnt, DelimCnt + 1, false, false); //get current substring (folder) within path
            //    }
            //    conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
            //    ii = ii + 1;
            //    strThumbT = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, sTitle);
            //    if (!System.IO.File.Exists(conf.FileImage))
            //    {
            //        i = i + 1;
            //    }
            //}
            //LogMyFilms.Debug("MF: (GuzziPerfCount) - End (" + ii.ToString() + " Records with " + i.ToString() + " Lookups)");

            // Check and create Group thumb folder ...
            if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups"))
              System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups");

            foreach (DataRow sr in r)
            {
                number++;
                if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
                {
                    w_tableau = Search_String(sr[conf.WStrSort].ToString());
                    foreach (object t in w_tableau)
                    {
                        if ((conf.WStrSort == "Date") || (conf.WStrSort == "DateAdded"))
                        {
                            if (string.Format("{0:dd/MM/yyyy}", DateTime.Parse(t.ToString()).ToShortDateString()) == string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
                                goto suite;
                        }
                        else
                        {
                            if (t.ToString().ToLower().Contains(conf.Wselectedlabel.Trim().ToLower()))
                                goto suite;
                        }
                    }
                    goto fin;
                }
            suite:

                sFullTitle = sTitle = sr[conf.StrTitle1].ToString();
                //LogMyFilms.Debug("MF: (GetFilmList) - BuildDisplaylist - FullTitle: '" + sFullTitle + "'");

                DelimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
                if (DelimCnt <= DelimCnt2)
                {
                    sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, DelimCnt, DelimCnt + 1, false, false); //get current substring (folder) within path
                    sFullTitle = NewString.NPosRight(conf.TitleDelim, sFullTitle, DelimCnt, false, false); //current rest of path (if only 1 entry in subfolders will present entry ignoring folders)
                }

                if ((iCnt > 0) && (DelimCnt < DelimCnt2) && (sTitle == sPrevTitle)) // don't stack items already at lowest folder level
                {
                    iCnt++;
                    item.Label2 = "(" + iCnt.ToString() + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count)
                    if (iCnt == 2)
                    {
                        item.Label = sTitle; //reset to current single folder as > 1 entries
                        item.IsFolder = true;

                        item.ThumbnailImage = conf.FileImage;
                        item.IconImage = conf.FileImage;
                    }
                }
                else
                {
                    iCnt = 1;
                    item = new GUIListItem();
                    item.Label = sFullTitle; // Set = full subfolders path initially
                    if (!MyFilms.conf.OnlyTitleList)
                    {
                        switch (conf.StrSorta)
                        {
                            case "TranslatedTitle":
                            case "OriginalTitle":
                            case "FormattedTitle":
                                item.Label2 = sr["Year"].ToString();
                                break;
                            case "YEAR":
                                item.Label2 = sr["Year"].ToString();
                                break;
                            case "DateAdded":
                                try
                                {
                                    item.Label2 = sr["Date"].ToString();
                                }
                                catch
                                {
                                }
                                break;
                            case "RATING":
                                item.Label2 = sr["Rating"].ToString();
                                break;
                            default:
                                if (conf.StrSorta == conf.StrSTitle)
                                    item.Label2 = sr["Year"].ToString();
                                else
                                    item.Label2 = sr[conf.StrSorta].ToString();
                                break;
                        }
                    }

                    //if (sr["Checked"].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue) // changed to take setup config into consideration
                    if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
                      if (sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                        item.IsPlayed = true;
                    if (MyFilms.conf.StrSuppress && MyFilms.conf.StrSuppressField.Length > 0)
                        if ((sr[MyFilms.conf.StrSuppressField].ToString() == MyFilms.conf.StrSuppressValue.ToString()) && (MyFilms.conf.StrSupPlayer))
                            item.IsPlayed = true;
                    if (sr["Picture"].ToString().Length > 0)
                    {
                        if ((sr["Picture"].ToString().IndexOf(":\\") == -1) && (sr["Picture"].ToString().Substring(0, 2) != "\\\\"))
                            conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
                        else
                            conf.FileImage = sr["Picture"].ToString();
                    }
                    else
                        conf.FileImage = string.Empty;
                    string strThumb = string.Empty;
                    if (string.IsNullOrEmpty(conf.FileImage) || !System.IO.File.Exists(conf.FileImage)) // No Coverart in DB - so handle it !
                    {
                        string strlabel = item.Label;
                        MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strlabel);
                        strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups\" + strlabel;
                        if (System.IO.File.Exists(strThumb + ".png"))
                        {
                          conf.FileImage = strThumb + ".png"; 
                        }
                        else
                        {
                            try
                            {
                                if (conf.StrPathViews.Length > 0)
                                    if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                                        Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                    else
                                        Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                // Disabled "pseudo covers with label name"
                                //if (!System.IO.File.Exists(strThumb + ".png"))
                                //    if (MyFilms.conf.StrViewsDflt && System.IO.File.Exists(MyFilms.conf.DefaultCoverViews))
                                //        ImageFast.CreateImage(strThumb + ".png", item.Label);
                                if (System.IO.File.Exists(strThumb + ".png"))                                
                                conf.FileImage = strThumb + ".png"; 
                            }
                            catch
                            {
                                conf.FileImage = string.Empty;
                            }
                            if (string.IsNullOrEmpty(conf.FileImage) && conf.DefaultCover.Length > 0)
                              conf.FileImage = conf.DefaultCover;
                        }
                    }
                    item.ThumbnailImage = conf.FileImage;
                    strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, sTitle);
                    //Guzzi: Added for Debugging DeDa Changes of MyVideos...
                    //LogMyFilms.Debug("MF: (GetFilmList) : strThumb: '" + strThumb + "'");
                    if ((!System.IO.File.Exists(strThumb)) && (conf.FileImage != conf.DefaultCover))
                      Picture.CreateThumbnail(conf.FileImage, strThumb, 100, 150, 0, Thumbs.SpeedThumbsSmall);
                    if (conf.FileImage == conf.DefaultCover)
                      item.IconImage = conf.DefaultCover;
                    else
                      item.IconImage = strThumb;
                    item.ItemId = number;
                    item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                    facadeView.Add(item);

                    if (iSelItem == -2) //set selected item = passed in string?
                    {
                        if (sTitle == SelItem)
                        {
                            wfacadewiew = facadeView.Count - 1; //test if this item is one to select
                        }
                    }
                }
                if (iSelItem >= 0) //-1 = ignore, >=0 = itemID to locate (test every item in case item is from within a folder)
                {
                    if (!(conf.StrTIndex.Length > 0))
                    {
                        if (number == iSelItem)
                        {
                            wfacadewiew = facadeView.Count - 1; //test if this item is one to select
                        }
                    }
                    else
                    {
                        if ((number == iSelItem) && (sFullTitle == conf.StrTIndex))
                        {
                         wfacadewiew = facadeView.Count - 1; //test if this item is one to select
                        }
                    }
                }

                sPrevTitle = sTitle;
            fin: ;
            }
            if (facadeView.Count == 0)
            {
                ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798639));
                GUIWaitCursor.Show();
                DisplayAllMovies();
                InitMainScreen();
                GUIControl.ShowControl(GetID, 34); // hides certain GUI elements
            }
            else
            {
                //ImgFanart.SetVisibleCondition(1, true);  ->> This fucked up the fanart swapper !!!!!
                if (!backdrop.Active) backdrop.Active = true;
                GUIControl.HideControl(GetID, 34);
            }
            MyFilmsDetail.setGUIProperty("nbobjects.value", facadeView.Count.ToString());
            GUIPropertyManager.SetProperty("#itemcount", facadeView.Count.ToString());
            GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wfacadewiew);
            if (facadeView.Count == 1 && item.IsFolder)
            {
                conf.Boolreturn = false;
                conf.Wselectedlabel = item.Label;
            }
            return !(facadeView.Count == 1 && item.IsFolder); //ret false if single folder found
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

        //----------------------------------------------------------------------------------------
        //    Display Detailed Info (Image, Description, Year, Category)
        //----------------------------------------------------------------------------------------
        private void Load_Lstdetail(int ItemId, bool wrep, string wlabel)//wrep = false display only image
        {
            //if (facadeView.SelectedListItem.ItemId == Prev_ItemID)
            //    return;
            LogMyFilms.Debug("MF: (Load_Lstdetail): ItemId = " + ItemId.ToString() + ", wrep = " + wrep.ToString() + ", wlabel = " + wlabel);
            if (ItemId == -1)
            {
                // reinit some fields
                return;
            }
            if ((facadeView.SelectedListItem.IsFolder) && (MyFilms.conf.Boolselect))
            {
                LogMyFilms.Debug("MF: (Load_Lstdetail): Item is Folder and BoolSelect is true");
                string[] wfanart = MyFilmsDetail.Search_Fanart(wlabel, true, "file", true, facadeView.SelectedListItem.ThumbnailImage.ToString(), facadeView.SelectedListItem.Path);
                if (wfanart[0] == " ")
                {
                    backdrop.Active = false;
                    GUIControl.HideControl(GetID, 35);
                    LogMyFilms.Debug("MF: (Load_Lstdetail): INACTIVE backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                else
                {
                    backdrop.Active = true;
                    GUIControl.ShowControl(GetID, 35);
                    LogMyFilms.Debug("MF: (Load_Lstdetail): ACTIVE backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                backdrop.Filename = wfanart[0];

                cover.Filename = facadeView.SelectedListItem.ThumbnailImage.ToString();
                //if (!backdrop.Active)
                //    backdrop.Active = true;
                //GUIControl.ShowControl(GetID, 34);
                Prev_ItemID = facadeView.SelectedListItem.ItemId;
                MyFilmsDetail.setGUIProperty("picture", facadeView.SelectedListItem.ThumbnailImage.ToString());
                // this.Load_Rating(0); // old method - nor more used
                MyFilmsDetail.clearGUIProperty("logos_id2001");
                MyFilmsDetail.clearGUIProperty("logos_id2002");
                MyFilmsDetail.clearGUIProperty("logos_id2003");
                MyFilmsDetail.clearGUIProperty("logos_id2012");
            }
            else
            {
                LogMyFilms.Debug("MF: (Load_Lstdetail): Item is Movie itself!");
                
                //ImageSwapper backdrop = new ImageSwapper();
                string[] wfanart = new string[2];
                wfanart = MyFilmsDetail.Search_Fanart(wlabel, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
                LogMyFilms.Debug("MyFilm (Load_Lstdetail): Backdrops-File: wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                //if (wfanart[0] == " ")
                //{
                //    wfanart = MyFilmsDetail.Search_Fanart(wlabel, true, "dir", true, facadeView.SelectedListItem.ThumbnailImage.ToString(), facadeView.SelectedListItem.Path);
                // }
                if (wfanart[0] == " ")
                    {
                        if (backdrop.Active)
                            backdrop.Active = false;
                        if (!cover.Active)
                            cover.Active = true;
                        GUIControl.HideControl(GetID, 35);
                        LogMyFilms.Debug("MyFilm (Load_Lstdetail): Fanart-Status: '" + backdrop.Active + "'");    
                    }
                else
                    {
                        if (!backdrop.Active)
                            backdrop.Active = true;
                        if (!cover.Active)
                            cover.Active = true;
                        GUIControl.ShowControl(GetID, 35);
                        LogMyFilms.Debug("MyFilm (Load_Lstdetail): Fanart-Status: '" + backdrop.Active + "'");
                    }
                LogMyFilms.Debug("MyFilm (Load_Lstdetail): Backdrops-File: backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                backdrop.Filename = wfanart[0];
                if (facadeView.SelectedListItem.IsFolder)
                    Prev_ItemID = facadeView.SelectedListItem.ItemId;
                Prev_ItemID = facadeView.SelectedListItem.ItemId;
                //if (!System.IO.File.Exists(facadeView.SelectedListItem.ThumbnailImage))
                //{
                //  conf.FileImage = MyFilms.conf.DefaultCover;
                //  //conf.FileImage = MyFilms.conf.DefaultCoverArtist;
                //  //conf.FileImage = MyFilms.conf.DefaultCoverViews;
                //  MyFilmsDetail.setGUIProperty("picture", MyFilms.conf.FileImage);
                //  cover.Filename = MyFilms.conf.FileImage;
                //}
                //else
                {
                  conf.FileImage = facadeView.SelectedListItem.ThumbnailImage;
                  MyFilmsDetail.setGUIProperty("picture", MyFilms.conf.FileImage);
                  cover.Filename = MyFilms.conf.FileImage;
                }

                //m_FanartTimer.Change(0, 10000); // 10000 = 10 sek. // Added to immediately change Fanart - activate to enable timer and reset it !

                //XmlConfig XmlConfig = new XmlConfig();
                //string logo_type = string.Empty;
                //string wlogos = string.Empty;
                Load_Logos(MyFilms.r[ItemId]);
            }

            //Make a difference between movies and persons -> Load_Detailed_DB or Load_Detailed_PersonInfo
            MyFilmsDetail.Load_Detailed_DB(ItemId, wrep);
            if ((conf.WStrSort.ToLower().Contains("actors")) || (conf.WStrSort.ToLower().Contains("producer")) || (conf.WStrSort.ToLower().Contains("director")))
                MyFilmsDetail.Load_Detailed_PersonInfo(facadeView.SelectedListItem.Label, wrep);

          // Load_Rating(conf.W_rating); // old method - nor more used
        }

        //-------------------------------------------------------------------------------------------
        //  Control search Text : no specials characters only alphanumerics
        //-------------------------------------------------------------------------------------------        
        private static bool control_searchText(string stext)
        {
            Regex maRegexp = new Regex("^[^*]+$");
            bool regOK = maRegexp.IsMatch(stext);
            return regOK;
        }
        //-------------------------------------------------------------------------------------------
        //  Proc for Sort Button
        //-------------------------------------------------------------------------------------------        
        void SortChanged(object sender, SortEventArgs e)
        {
            if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSens.Substring(1,3).ToLower())
                return;
            if (BtnSrtBy.IsAscending)
                conf.StrSortSens = " ASC";
            else
                conf.StrSortSens = " DESC";
            if (!conf.Boolselect)
                GetFilmList();
            else
                getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.StrSortSens, conf.Wstar, true, "");
            return;
        }

        private void item_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            GUIFilmstripControl filmstrip = parent as GUIFilmstripControl;
            //if (filmstrip == null) return;
            //filmstrip.InfoImageFileName = item.ThumbnailImage;
            if (filmstrip != null)
                filmstrip.InfoImageFileName = item.ThumbnailImage;
            if (!(conf.Boolselect || (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder))) //xxxx
            {
                if (facadeView.SelectedListItemIndex > -1)
                    this.Load_Lstdetail(facadeView.SelectedListItem.ItemId, true, facadeView.SelectedListItem.Label);
                    //this.MovieDetailsPublisher(true);
            }
            else
            {
                if (facadeView.SelectedListItemIndex > -1 && !conf.Boolselect)
                {
                  this.Load_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                  //this.MovieDetailsPublisher(false);
                  //LogMyFilms.Debug("MF: OnItemSelected - ItemIndex: '" + facadeView.SelectedListItemIndex.ToString() + "', Boolselect: '" + conf.Boolselect.ToString() + "', Details: 'false'");
                }
                else
                {
                    this.Load_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                    //LogMyFilms.Debug("MF: OnItemSelected - ItemIndex: '" + facadeView.SelectedListItemIndex.ToString() + "', Boolselect: '" + conf.Boolselect.ToString() + "', Details: 'false'");
                    //this.MovieDetailsPublisher(false);
                    GUIControl.ShowControl(GetID, 34);
                    //this.Load_Rating(0); // old method - nor more used
                }
            } 
            //Load_Lstdetail(item.ItemId, true, item.Label);
        }

        private void MovieDetailsPublisher(bool wrep)
        {
          double tickCount = System.Windows.Media.Animation.AnimationTimer.TickCount;

          // Publish instantly when previous request has passed the required delay
          if (150 < (int)(tickCount - lastPublished)) // wait 100 ms to load details...
          {
            lastPublished = tickCount;
            Load_Lstdetail(facadeView.SelectedListItem.ItemId, wrep, facadeView.SelectedListItem.Label);
            return;
          }
          else // Publish on timer using the delay specified in settings
          {
            lastPublished = tickCount;
            if (publishTimer == null)
            {
              publishTimer = new Timer(delegate { Load_Lstdetail(facadeView.SelectedListItem.ItemId, wrep, facadeView.SelectedListItem.Label); }, null, 150, Timeout.Infinite);
            }
            else
            {
              publishTimer.Change(150, Timeout.Infinite);
            }
          }
        }


        #region Accès Données

        //--------------------------------------------------------------------------------------------
        //  Select View for Video
        //--------------------------------------------------------------------------------------------
        private void Change_Selection_type_Video()
        {
            GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg1 == null) return;
            dlg1.Reset();
            dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
            dlg1.Add(GUILocalizeStrings.Get(342));//videos
            dlg1.Add(GUILocalizeStrings.Get(345));//year
            dlg1.Add(GUILocalizeStrings.Get(10798664));//genre -> category
            dlg1.Add(GUILocalizeStrings.Get(200026));//countries
            dlg1.Add(GUILocalizeStrings.Get(10798667));//actors 
            //dlg1.Add(GUILocalizeStrings.Get(200027));//Watched
            System.Collections.Generic.List<string> choiceView = new System.Collections.Generic.List<string>();
            choiceView.Add("all");
            choiceView.Add("year");
            choiceView.Add("category");
            choiceView.Add("country");
            choiceView.Add("actors");
            
            if (conf.StrStorage.Length != 0 && conf.StrStorage != "(none)")
            {
                dlg1.Add(GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951));//storage
                choiceView.Add("storage");
            }

            for (int i = 0; i < 5; i++)
            {
                if (conf.StrViewItem[i] != null && conf.StrViewItem[i] != "(none)" && (conf.StrViewItem[i].Length > 0))
                {
                    choiceView.Add(string.Format("view{0}",i));
                    if ((conf.StrViewText[i] == null) || (conf.StrViewText[i].Length == 0))
                        dlg1.Add(conf.StrViewItem[i]);   // specific user View1
                    else
                        dlg1.Add(conf.StrViewText[i]);   // specific Text for View1
                }
            }
            dlg1.DoModal(GetID);

            if (dlg1.SelectedLabel == -1)
            {
                return;
            }
            Change_view(choiceView[dlg1.SelectedLabel].ToLower());
            return;
        }
        
        //--------------------------------------------------------------------------------------------
        //  Select main Options
        //--------------------------------------------------------------------------------------------
        private void Change_Option()
        {
            GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg1 == null) return;
            System.Collections.Generic.List<string> choiceView = new System.Collections.Generic.List<string>();
            dlg1.Reset();
            dlg1.SetHeading(GUILocalizeStrings.Get(10798701)); // Options Menu
            if (Configuration.NbConfig > 1)
            {
                dlg1.Add(GUILocalizeStrings.Get(6022));   // Change Config 
                //dlg1.Add(GUILocalizeStrings.Get(6029) + " " + GUILocalizeStrings.Get(6022));   // Change Config 
                choiceView.Add("config");
            }

            if (MyFilms.conf.StrCheckWOLenable) // Show current NAS server status
            {
                dlg1.Add(GUILocalizeStrings.Get(10798727));   // Show NAS Server Status
                choiceView.Add("nasstatus");
            }

            // Add Submenu for Global Settings 
            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798689)));
            choiceView.Add("globaloptions");

            // Add Submenu for Global Updates
            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798690)));
            choiceView.Add("globalupdates");

            // Add Submenu for Wiki Online Help
            if (MyFilmsDetail.ExtendedStartmode("Contextmenu for Wiki Onlinehelp")) // check if specialmode is configured for disabled features
            {
              dlg1.Add(string.Format(GUILocalizeStrings.Get(10798699)));
              choiceView.Add("globalwikihelp");
            }

            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798700))); // About ...
            choiceView.Add("about");

            dlg1.DoModal(GetID);

            if (dlg1.SelectedLabel == -1)
            {
                return;
            }
            Change_view(choiceView[dlg1.SelectedLabel].ToLower());
            return;
        }



        //--------------------------------------------------------------------------------------------
        //  Select main Options
        //--------------------------------------------------------------------------------------------
        private void Change_Global_Filters()
        {
          Context_Menu = true; // make sure, it's set, as otherwise we fall back to first menu level ...
          GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg1 == null) return;
          dlg1.Reset();
          dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
          System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

          // Change global Unwatchedfilteroption
          // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
          if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("globalunwatchedfilter");

          // Change global MovieFilter (Only Movies with Trailer)
          if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
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
          dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MyFilms.conf.StrAntFilterMinRating.ToString()));
          choiceViewGlobalOptions.Add("filterdbsetrating");

          dlg1.DoModal(GetID);
          if (dlg1.SelectedLabel == -1)
            return;
          LogMyFilms.Debug("MF: Call change_global_filters menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");
          Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          return;
        }

    public static ArrayList Search_String(string champselect)
        {
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value,""));
             }
            ArrayList wtab = new ArrayList();

            int wi = 0;
            string[] Sep = conf.ListSeparator;
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
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
                    wzone = string.Empty;
                }
            }
            return wtab;
        }

        public static ArrayList SubItemGrabbing(string champselect)
        {
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
            }
            ArrayList wtab = new ArrayList();

            int wi = 0;
            string[] Sep = conf.ListSeparator;
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
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
                    wzone = string.Empty;
                }
            }
            return wtab;
        }

        public static ArrayList SubTitleGrabbing(string champselect)
        {
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
            }
            ArrayList wtab = new ArrayList();

            int wi = 0;
            string[] Sep = {" - ", ":"}; //Only Dash as Separator for Movietitles !!!
            //string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ","- "," -"," AT "};
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
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
                    wzone = string.Empty;
                }
            }
            return wtab;
        }

        public static ArrayList SubWordGrabbing(string champselect, int minchars, bool filter)
        {
            LogMyFilms.Debug("MF: (SubWordGrabbing): InputString: '" + champselect + "'"); 
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
                LogMyFilms.Debug("MF: (SubWordGrabbing): RegExReplace: '" + champselect + "'");
            }

            string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ","- "," -"," AT ", "in "};
            int i = 0;
            for (i = 0; i <  13; i++)
            {
                if ((CleanerList[i].Length > 0) && (filter = true))
                {
                    champselect = champselect.Replace(CleanerList[i], " ");
                    //LogMyFilms.Debug("MF: (SubWordGrabbing): CleanerListItem: '" + CleanerList[i] + "'");
                }
            }
            
            ArrayList wtab = new ArrayList();

            int wi = 0;
            //string[] Sep = conf.ListSeparator;
            string[] Sep = new string[]
                {" ",",",";","|","/","(",")",".",@"\",":"};
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
                    LogMyFilms.Debug("MF: (SubWordGrabbing): wzone: '" + wzone + "'"); 
                    if (wzone.Length >= minchars)//Only words with minimum 4 letters!
                    {
                        wtab.Add(wzone.Trim());
                        LogMyFilms.Debug("MF: (SubWordGrabbing): AddWordToList: '" + wzone.Trim() + "'");
                    }
                    wzone = string.Empty;
                }
            }
            return wtab;
        }


        /// <summary>Selects records for display grouping them as required</summary>
        /// <param name="WstrSelect">Select this kind of records</param>
        /// <param name="WStrSort">Sort based on this</param>
        /// <param name="WStrSortSens">Asc/Desc. Ascending or descending sort order</param>
        /// <param name="NewWstar">Entries must contain this string to be included</param>
        /// <param name="ClearIndex">Reset Selected Item Index</param>
        /// <param name="SelItem">Select entry matching this string if not empty</param>
        /// 
        //Example for Actors list:
            //conf.WStrSort = "ACTORS";
            //conf.Wselectedlabel = "";
            //conf.WStrSortSens = " ASC";
            //BtnSrtBy.IsAscending = true;
            //conf.StrActors = keyboard.Text;
            //getSelectFromDivx("Actors like '*" + keyboard.Text + "*'", conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");

        public void getSelectFromDivx(string WstrSelect, string WStrSort, string WStrSortSens, string NewWstar, bool ClearIndex, string SelItem)
        {
            GUIListItem item = new GUIListItem();
            Prev_ItemID = -1;
            string champselect = "";
            string wchampselect = "";
            ArrayList w_tableau = new ArrayList();
            int Wnb_enr = 0;

            conf.Wstar = NewWstar;
            BtnSrtBy.Label = GUILocalizeStrings.Get(103);
            conf.Boolselect = true;
            conf.Wselectedlabel = "";
            if (ClearIndex) 
                conf.StrIndex = 0;
            Change_LayOut(0); 
            facadeView.Clear();
            int wi = 0;

            string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - GlobalFilterString          : '" + GlobalFilterString + "'");
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - conf.StrDfltSelect          : '" + conf.StrDfltSelect + "'");
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - WstrSelect                  : '" + WstrSelect + "'");
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - WStrSort                    : '" + WStrSort + "'");
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - WStrSortSens                : '" + WStrSortSens + "'");
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - NewWstar                    : '" + NewWstar + "'");
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - Read movie DB Started (LectureDonnées)");

            // Collect List of all attributes in w_tableau
            foreach (DataRow enr in BaseMesFilms.LectureDonnées(GlobalFilterString + conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
                {
                if ((WStrSort == "Date") || (WStrSort == "DateAdded"))
                    champselect = string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]);
                else
                    champselect = enr[WStrSort].ToString().Trim();
                ArrayList wtab = Search_String(champselect);
                //string wAddList = "";
                //if ((NewWstar.Length < 3) && ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director"))))
                if ((NewWstar.Length < 0) && ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director"))))
                    for (wi = 0; wi < wtab.Count; wi++)
                        {
                        //wAddList = wtab[wi].ToString().Trim().Substring(1,1) + "\\" + wtab[wi].ToString().Trim().Substring(2);
                        if (wtab[wi].ToString().Trim().Length > 1)
                            //w_tableau.Add(wtab[wi].ToString().Trim().Substring(0, 1) + @"\" + wtab[wi].ToString().Trim().Substring(1));
                            w_tableau.Add(wtab[wi].ToString().Trim().Substring(0,1) + @"\" + wtab[wi].ToString().Trim());
                        else
                            w_tableau.Add(wtab[wi].ToString().Trim());
                        }
                else
                    for (wi = 0; wi < wtab.Count; wi++)
                    {
                        w_tableau.Add(wtab[wi].ToString().Trim());
                    }
                }

            LogMyFilms.Debug("MF: (GetSelectFromDivx) - Read movie DB Finished (LectureDonnées)");
            if (WStrSortSens == " ASC")
                w_tableau.Sort(0, w_tableau.Count, null);
            else
            {
                IComparer myComparer = new myReverserClass();
                w_tableau.Sort(0, w_tableau.Count, myComparer);
            }
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - Sorting Finished");

            if (MyFilms.conf.StrViews || MyFilms.conf.StrPersons) // Check if Thumbs directories exist or create them
            {
                if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups")) System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups");
                if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons")) System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons");
            }

            // setting up thumbs directory configuration
            string strThumbDirectory;
            string [] strActiveFacadeImages;
            if (WStrSort.ToLower().Contains("actors") || WStrSort.ToLower().Contains("producer") || WStrSort.ToLower().Contains("director") || WStrSort.ToLower().Contains("borrower"))
              strThumbDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons\";
            else
              strThumbDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups\";
            bool isperson = false;
            if (WStrSort.ToLower().Contains("actors") || WStrSort.ToLower().Contains("producer") || WStrSort.ToLower().Contains("director") || WStrSort.ToLower().Contains("borrower"))
              isperson = true;
            bool getThumbs = false;
            if (MyFilms.conf.StrViews && (MyFilms.conf.StrViewsDfltAll || (WStrSort.ToLower().Contains("category") || WStrSort.ToLower().Contains("year") || WStrSort.ToLower().Contains("country"))))
              getThumbs = true;
            if (MyFilms.conf.StrPersons && (isperson))
              getThumbs = true;
            bool createFanartDir = false;
            if (WStrSort.ToLower() == "category" || WStrSort.ToLower() == "year" || WStrSort.ToLower() == "country") 
              createFanartDir = true;

            LogMyFilms.Debug("MF: (GetSelectFromDivx) - Facadesetup Groups Started");
            //item = new GUIListItem();
            for (wi = 0; wi != w_tableau.Count; wi++)
            {
                champselect = w_tableau[wi].ToString();
                if (string.Compare(champselect, wchampselect, true) == 0) // Are the strings equal? Then add count!
                {
                  Wnb_enr++; // count items of distinct property
                }
                else
                {
                    if (conf.Wstar == "*" || champselect.ToUpper().Contains(conf.Wstar.ToUpper()))
                    {
                      if ((Wnb_enr > 0) && (wchampselect.Length > 0))
                      {
                        item = new GUIListItem();
                        item.Label = wchampselect;
                        item.Label2 = Wnb_enr.ToString();
                        if (getThumbs)
                        {
                          strActiveFacadeImages = SetViewThumbs(WStrSort, item.Label, strThumbDirectory, isperson);
                          item.ThumbnailImage = strActiveFacadeImages[0];
                          item.IconImage = strActiveFacadeImages[1];
                        }
                        if (createFanartDir)
                        {
                          string[] wfanart;
                          wfanart = MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, WStrSort.ToLower());
                        }
                        item.IsFolder = true;
                        item.Path = WStrSort.ToLower();
                        item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                        facadeView.Add(item);
                        if (SelItem != "" && item.Label == SelItem) 
                          conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                      }
                      Wnb_enr = 1;
                      wchampselect = champselect;
                    }
                }
            }

            if ((Wnb_enr > 0) && (wchampselect.Length > 0))
            {
                item = new GUIListItem();
                item.Label = wchampselect;
                item.Label2 = Wnb_enr.ToString();
                if (getThumbs)
                {
                  strActiveFacadeImages = SetViewThumbs(WStrSort, item.Label, strThumbDirectory, isperson);
                  item.ThumbnailImage = strActiveFacadeImages[0];
                  item.IconImage = strActiveFacadeImages[1];
                }
                string[] wfanart;
                if (createFanartDir)
                  wfanart = MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, WStrSort.ToLower());
                item.IsFolder = true;
                item.Path = WStrSort.ToLower(); 
                //item.ItemId = number; // Only used in GetFilmList
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                facadeView.Add(item);
                if (SelItem != "" && item.Label == SelItem) 
                  conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                Wnb_enr = 0;
            }
            LogMyFilms.Debug("MF: (GetSelectFromDivx) - Facadesetup Groups Finished");

            item.FreeMemory();
            conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
            if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(1079896) + " [*" + conf.Wstar + "*]"; // add to "Selection": Persons with Filter
            MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect); 
            //TxtSelect.Label = conf.StrTxtSelect;
            conf.StrSelect = WstrSelect;
            conf.StrFilmSelect = string.Empty;

            if ((conf.StrIndex > facadeView.Count - 1) || (conf.StrIndex < 0)) //check index within bounds, will be unless xml file heavily edited
                conf.StrIndex = 0;
            if (facadeView.Count == 0)
            {
              ShowMessageDialog(GUILocalizeStrings.Get(10798624), GUILocalizeStrings.Get(10798637), GUILocalizeStrings.Get(10798638));
                GUIWaitCursor.Show();
                DisplayAllMovies();
                GetFilmList();
                GUIControl.ShowControl(GetID, 34);
            }
            else
            {
                //ImgFanart.SetVisibleCondition(1, true);  ->> This fucked up the fanart swapper !!!!!
                if (!backdrop.Active) 
                    backdrop.Active = true;
                GUIControl.ShowControl(GetID, 34);
                //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
                //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
                
                //Make a difference between movies and persons -> Load_Detailed_DB or Load_Detailed_PersonInfo
                MyFilmsDetail.Load_Detailed_DB(0, false);
                if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                    MyFilmsDetail.Load_Detailed_PersonInfo(facadeView.SelectedListItem.Label, false);

                // Disabled because replaced by SpeedLoader
                //ImgLstFilm.SetFileName("#myfilms.picture");
                //ImgLstFilm2.SetFileName("#myfilms.picture");
                // this.Load_Rating(0); // old method - nor more used
            }
            MyFilmsDetail.setGUIProperty("nbobjects.value", facadeView.Count.ToString());
            GUIPropertyManager.SetProperty("#itemcount", facadeView.Count.ToString());
 
            //MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
            //GUIWaitCursor.Hide();
            GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)conf.StrIndex);

        }

        private static string[] SetViewThumbs(string WStrSort, string itemlabel, string strThumbDirectory, bool isPerson)
        {
          string[] thumbimages = new string[2];
          thumbimages[0] = string.Empty; // ThumbnailImage
          thumbimages[1] = string.Empty; //IconImage
          string strThumb = strThumbDirectory + itemlabel + ".png";
          //string strThumbLarge = string.Empty;
          string strThumbSource = string.Empty;

          if (isPerson)
          {
            //strThumbSource = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieActors, itemlabel); // check for actors images in MyVideos...
            //LogMyFilms.Debug("MF: Artist thumbs - GetCoverName(Thumbs, MovieActors) - strThumb = '" + strThumb + "'");
            //if (!System.IO.File.Exists(strThumb))
            //  strThumb = strThumbDirectory + itemlabel + ".png";
            //else
            //  return strThumb;

            if (System.IO.File.Exists(strThumb)) // If there is missing thumbs in cache folder ...
            {
              thumbimages[0] = strThumb;
              thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
              return thumbimages;
            }
            if (conf.StrPathArtist.Length > 0)
            {
              string strPathArtist = String.Empty;
              if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\") strPathArtist = conf.StrPathArtist;
              else strPathArtist = conf.StrPathArtist + "\\";
              if (System.IO.File.Exists(strPathArtist + itemlabel + "\\folder.jpg")) strThumbSource = strPathArtist + itemlabel + "\\folder.jpg";
              else if (System.IO.File.Exists(strPathArtist + itemlabel + "\\folder.png")) strThumbSource = strPathArtist + itemlabel + "\\folder.png";
              else if (System.IO.File.Exists(strPathArtist + itemlabel + "L" + ".jpg")) strThumbSource = strPathArtist + itemlabel + "L" + ".jpg";
              else if (System.IO.File.Exists(strPathArtist + itemlabel + ".jpg")) strThumbSource = strPathArtist + itemlabel + ".jpg";
              else if (System.IO.File.Exists(strPathArtist + itemlabel + ".png")) strThumbSource = strPathArtist + itemlabel + ".png";
              else if (conf.StrFileType == "5") // XMM artist thumbs: e.g. Alex-Revan_101640.jpg
              {
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in XMM picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel).Replace(" ", "-"); // replaces special character "á" and other special chars !
                  searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Count() > 0) 
                    strThumbSource = files[0];
                }
              }
              if (strThumbSource != string.Empty)
              {
                Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
                Picture.CreateThumbnail(strThumbSource, strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                thumbimages[0] = strThumbSource;
                thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
                return thumbimages;
              }
            }

            //if ((!System.IO.File.Exists(strThumbLarge)) && (strThumbLarge != conf.DefaultCoverArtist) && (strThumbSource != string.Empty))
            //{
            //  Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "L.png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
            //  strThumbLarge = strThumbDirectory + itemlabel + "L.png";
            //}

            // if (!System.IO.File.Exists(strThumb) && conf.StrArtistDflt && conf.DefaultCoverArtist.Length > 0)
            if (conf.StrArtistDflt && conf.DefaultCoverArtist.Length > 0)
              {
              //ImageFast.CreateImage(strThumb, item.Label); // this is to create a pseudo cover with name of label added to it
              //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
              //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
              thumbimages[0] = conf.DefaultCoverArtist;
              thumbimages[1] = conf.DefaultCoverArtist;
              return thumbimages;
            }
            else
            {
              thumbimages[0] = "";
              thumbimages[1] = "";
              return thumbimages;
            }

          }
          else if (MyFilms.conf.StrViewsDfltAll || (WStrSort.ToLower().Contains("country") || WStrSort.ToLower().Contains("category") || WStrSort.ToLower().Contains("year")))
          {
            if (System.IO.File.Exists(strThumb)) // If there is missing thumbs in cache folder ...
            {
              thumbimages[0] = strThumb;
              thumbimages[1] = strThumb;
              return thumbimages;
            }

            if (conf.StrPathViews.Length > 0)
            {
              string strPathViews = String.Empty;
              if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                strPathViews = conf.StrPathViews;
              else
                strPathViews = conf.StrPathViews + "\\";
              if (System.IO.File.Exists(strPathViews + itemlabel + ".jpg"))
                Picture.CreateThumbnail(strPathViews + itemlabel + ".jpg", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
              else if (System.IO.File.Exists(strPathViews + itemlabel + ".png"))
                Picture.CreateThumbnail(strPathViews + itemlabel + ".png", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
              if (System.IO.File.Exists(strThumb))
              {
                thumbimages[0] = strThumb;
                thumbimages[1] = strThumb;
                return thumbimages;
              }
            }
            // Use Default Cover if no specific Cover found:
            //  if (MyFilms.conf.StrViewsDflt && System.IO.File.Exists(MyFilms.conf.DefaultCover))
            if (MyFilms.conf.StrViewsDflt && (MyFilms.conf.DefaultCoverViews.Length > 0))
            {
              //ImageFast.CreateImage(strThumb, item.Label); // Disabled "old" method to use Defaultcover with embedded text ofg selected item ...
              //Picture.CreateThumbnail(strThumbLarge, strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
              thumbimages[0] = conf.DefaultCoverViews;
              thumbimages[1] = conf.DefaultCoverViews;
              return thumbimages;
            }
          }
          return thumbimages;
        }
      
      
        //----------------------------------------------------------------------------------------------
        //  Reverse Sort
        //----------------------------------------------------------------------------------------------
        public class myReverserClass : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                return ((new CaseInsensitiveComparer()).Compare(y, x));
            }
        }

        private static void Load_Config(string CurrentConfig, bool create_temp)
        {
            conf = new Configuration(CurrentConfig, create_temp);
            if ((conf.Boolreturn) && (conf.Wselectedlabel == string.Empty))
            {
                conf.Boolselect = true;
                conf.Boolreturn = false;
            }
            if (conf.StrLogos)
                confLogos = new Logos();
            MyFilmsDetail.setGUIProperty("config.currentconfig", CurrentConfig);

            if (conf.StrDfltSelect.Length > 0)
            {
              string userfilter = conf.StrDfltSelect.Substring(0, conf.StrDfltSelect.LastIndexOf("AND")).Trim();
              MyFilmsDetail.setGUIProperty("config.configfilter", userfilter);
              LogMyFilms.Debug("MF: userfilter from setup: StrDfltSelect = '" + userfilter + "'");
            }
            else
              MyFilmsDetail.clearGUIProperty("config.configfilter");
        }

         //--------------------------------------------------------------------------------------------
        //  Initial Windows load. If LoadDfltSlct = true => load default select if any
        //                           LoadDfltSlct = false => return from  MyFilmsDetail
        //--------------------------------------------------------------------------------------------
        private void Fin_Charge_Init(bool LoadDfltSlct, bool reload)
        {
            if (LoadDfltSlct)
            {
                conf.Boolselect = false;
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
                  GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
                  MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
                }
                else
                {
                  GlobalFilterStringUnwatched = String.Empty;
                  MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
                }
                MovieScrobbling = false; // reset scrobbler filter setting
            }
            if (((PreviousWindowId != ID_MyFilmsDetail) && (PreviousWindowId != ID_MyFilmsActors)) || (reload))
            {
                //chargement des films
                BaseMesFilms.LoadFilm(conf.StrFileXml);
                r = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
            }
            //Layout = conf.StrLayOut;

            //if (MyFilms.conf.GlobalUnwatchedOnly)
            //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628));
            //else
            //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));
            if (string.IsNullOrEmpty(conf.CurrentSortMethod))
                conf.CurrentSortMethod = GUILocalizeStrings.Get(103);
            else
                BtnSrtBy.Label = conf.CurrentSortMethod;
            string BtnSearchT = GUILocalizeStrings.Get(137);

            GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnSearchT, BtnSearchT);
            BtnSrtBy.SortChanged += new SortEventHandler(SortChanged);
            InitialStart = false; // Guzzi: Set to false after first initialization to be able to return to noninitialized View - Make sure to set true if changing DB config

            if (conf.Boolselect)
            {
              // Hack to get persons in ASC order after returning from external plugins ...
              if (conf.WStrSort.ToUpper() == "ACTORS" || conf.WStrSort.ToUpper() == "PRODUCER" || conf.WStrSort.ToUpper() == "DIRECTOR")
                getSelectFromDivx(conf.StrSelect, conf.WStrSort, " ASC", conf.Wstar, false, ""); // preserve index from last time
              else
                getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.StrSortSens, conf.Wstar, false, ""); // preserve index from last time
              LogMyFilms.Debug("MF: (Fin_Charge_Init) - Boolselect = true -> StrTxtSelect = '" + MyFilms.conf.StrTxtSelect + "', StrTxtView = '" + MyFilms.conf.StrTxtView + "'");
              //if (string.IsNullOrEmpty(conf.StrTxtView)) // make sure it's set to "Films" if not yet initialized
              //  conf.StrTxtView = "all";
              SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
            }
            else
            {
                Change_LayOut(MyFilms.conf.StrLayOut);
                if (!(LoadDfltSlct))                       // Defaultview not selected
                {
                  GetFilmList(conf.StrIndex);
                  SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
                }
                else                                        // Defaultview selected
                {
                  if (string.IsNullOrEmpty(conf.StrViewDfltItem) || conf.StrViewDfltItem == "(none)" || conf.StrViewDfltItem == GUILocalizeStrings.Get(342)) // no Defaultitem defined for defaultview or "films" -> normal movielist
                    {
                        conf.StrSelect = conf.StrTitle1 + " not like ''";
//                        TxtSelect.Label = conf.StrTxtSelect = "";
                        conf.Boolselect = false;
                        if (conf.StrSortSens == " ASC")
                            BtnSrtBy.IsAscending = true;
                        else
                            BtnSrtBy.IsAscending = false;
                        GetFilmList(conf.StrIndex);
                        SetLabelView("all");
                        SetLabelSelect("root");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(conf.StrViewDfltText)) // no filteritem defined for the defaultview
                        {
                          if (conf.StrViewDfltItem.ToLower() == "year" || conf.StrViewDfltItem.ToLower() == "category" || conf.StrViewDfltItem.ToLower() == "country" || conf.StrViewDfltItem.ToLower() == "storage" || conf.StrViewDfltItem.ToLower() == "actors")
                                Change_view(conf.StrViewDfltItem.ToLower());
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (conf.StrViewDfltItem == conf.StrViewText[i])
                                        Change_view(string.Format("View{0}", i).ToLower());
                                }
                            }
                          // SetLabelView and SetLabelSelect will be set by "Change_view"
                        }
                        else
                        // View List as selected - filteritem IS defined for the defaultview
                        {
                            string wStrViewDfltItem = conf.StrViewDfltItem.ToLower();
                            for (int i = 0; i < 5; i++)
                            {
                                if (conf.StrViewDfltItem == conf.StrViewText[i])
                                {
                                    wStrViewDfltItem = conf.StrViewItem[i];
                                    SetLabelView("view" + i.ToString());
                                    break;
                                }
                            } 
                            conf.Boolselect = true;
                            conf.Boolreturn = true;
                            conf.Boolview = true;
                            conf.WStrSort = wStrViewDfltItem;
                            if (wStrViewDfltItem == "DateAdded")
                                conf.StrSelect = "Date" + " like '" + DateTime.Parse(conf.StrViewDfltText).ToShortDateString() + "'";
                            else
                                conf.StrSelect = wStrViewDfltItem + " like '*" + conf.StrViewDfltText + "*'";
//                            TxtSelect.Label = conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";
                            conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";
                            SetLabelSelect(conf.StrTxtSelect);

                            if (wStrViewDfltItem.Length > 0)
                                SetLabelView(wStrViewDfltItem); // replaces st with localized set - old: MyFilmsDetail.setGUIProperty("view", conf.StrViewDfltItem); // set default view config to #myfilms.view
                            MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
                            BtnSrtBy.Label = conf.CurrentSortMethod;
                            if (conf.StrSortSens == " ASC")
                                BtnSrtBy.IsAscending = true;
                            else
                                BtnSrtBy.IsAscending = false;
                            GetFilmList(conf.StrIndex);
                        }
                    }
                }
            }
            LogMyFilms.Debug("MF: Fin_Charge_Init: StrSelect = '" + conf.StrSelect + "', StrTxtSelect = '" + conf.StrTxtSelect + "'");
            if (string.IsNullOrEmpty(conf.StrTxtSelect) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)))
              SetLabelSelect("root");
            else
              SetLabelSelect(conf.StrTxtSelect);
            MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); 
            GUIWaitCursor.Hide();
            if (conf.LastID == ID_MyFilmsDetail)
                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail); // if last window in use was detailed one display that one again
            if (conf.LastID == ID_MyFilmsActors)
                GUIWindowManager.ActivateWindow(ID_MyFilmsActors); // if last window in use was actor one display that one again
        }
        //--------------------------------------------------------------------------------------------
        //   Change LayOut 
        //--------------------------------------------------------------------------------------------
        private void Change_LayOut(int wLayOut)
        {

#if MP11
            switch (wLayOut)
            {
                case 1:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(100));
                    facadeView.View = GUIFacadeControl.ViewMode.SmallIcons;
                    break;
                case 2:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(417));
                    facadeView.View = GUIFacadeControl.ViewMode.LargeIcons;
                    break;
                case 3:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(733));
                    facadeView.View = GUIFacadeControl.ViewMode.Filmstrip;
                    break;
                default:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
                    facadeView.View = GUIFacadeControl.ViewMode.List;
                    break;
            }
#else
            switch (wLayOut)
            {
                case 1:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(100));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                    break;
                case 2:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(417));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                    break;
                case 3:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(733));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
                    break;
                case 4:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(791));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
                    break;

                default:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.List;
                    break;
            }
#endif
        }
        //--------------------------------------------------------------------------------------------
        //   Change View Response  (and process corresponding filter list)
        //--------------------------------------------------------------------------------------------
        private void Change_view(string choiceView)
        {
            LogMyFilms.Debug("MF: Change_View called with '" + choiceView + "'");
            conf.Boolview = false;
            conf.Boolstorage = false;
            XmlConfig XmlConfig = new XmlConfig();
            switch (choiceView)
            {
                case "all":
                    //  Change View All Films
                    conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
                    conf.WStrSort = conf.StrSTitle;
                    conf.Boolselect = false;
                    conf.Boolreturn = false;
                    LogMyFilms.Debug("MF: Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
                    GetFilmList();
                    SetLabelView("all");
                    this.SetLabelSelect("root");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "year":
                    //  Change View by Year
                    conf.WStrSort = "YEAR";
                    conf.WStrSortSens = " DESC";
                    BtnSrtBy.IsAscending = false;
                    getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    SetLabelView("year"); 
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "category":
                //  Change View by  "Category":
                    conf.WStrSort = "CATEGORY";
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    SetLabelView("category");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "country":
                    //  Change View by "Country":
                    conf.WStrSort = "COUNTRY";
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    SetLabelView("country");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "actors":
                    //  Change View by "Actors":

                    conf.WStrSort = "ACTORS";
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
                    SetLabelView("actors");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "storage":
                //  Change View by "Storage":
                    conf.StrSelect = "((" + conf.StrTitle1.ToString() + " not like '') and (" + conf.StrStorage.ToString() + " not like ''))";
                    conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
//                    TxtSelect.Label = conf.StrTxtSelect;
                    MyFilmsDetail.clearGUIProperty("select");
                    conf.Boolselect = false;
                    conf.Boolreturn = false;
                    conf.Boolview = true;
                    conf.WStrSort = conf.StrSTitle;
                    BtnSrtBy.Label = conf.CurrentSortMethod;
                    if (conf.StrSortSens == " ASC")
                        BtnSrtBy.IsAscending = true;
                    else
                        BtnSrtBy.IsAscending = false;
                    GetFilmList();
                    SetLabelView("storage");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "view0":
                case "view1":
                case "view2":
                case "view3":
                case "view4":
                    // specific user View
                    int i = 0;
                    if (choiceView == "view1")
                        i = 1;
                    if (choiceView == "view2")
                        i = 2;
                    if (choiceView == "view3")
                        i = 3;
                    if (choiceView == "view4")
                        i = 4;

                    GUIWaitCursor.Show();
                    GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...
                    conf.WStrSort = conf.StrViewItem[i];
                    SetLabelView(choiceView);
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    if (conf.StrViewValue[i].Length > 0)
                    {
                        conf.Boolview = true;
                        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
                        conf.Boolselect = true;
                        conf.Wstar = "*";
                        if (conf.Wstar != "*")
                            conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
//                        TxtSelect.Label = conf.StrTxtSelect;
                        MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
                        if (conf.WStrSort == "DateAdded")
                            conf.StrSelect = "Date";
                        else
                            conf.StrSelect = conf.WStrSort;

                        conf.StrFilmSelect = string.Empty;
                        conf.Wselectedlabel = conf.StrViewValue[i];
                        conf.Boolreturn = true;
                        do
                        {
                          if (conf.StrTitleSelect != string.Empty)
                                conf.StrTitleSelect += conf.TitleDelim;
                            conf.StrTitleSelect += conf.StrViewValue[i];
                        } while (GetFilmList() == false); //keep calling while single folders found
                    }
                    else
                    {
                        if (conf.WStrSort == "DateAdded")
                          getSelectFromDivx(conf.StrTitle1 + " not like ''", "Date", " DESC", "*", true, string.Empty);
                        else
                          getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
                    }

                    //if ((conf.StrViewText[i] == null) || (conf.StrViewText[i].Length == 0))
                    //{
                    //    MyFilmsDetail.setGUIProperty("view", conf.StrViewItem[i]);   // specific user View1
                    //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewItem[i]);
                    //}
                    //else
                    //{
                    //    MyFilmsDetail.setGUIProperty("view", conf.StrViewText[i]);   // specific Text for View1
                    //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewText[i]);
                    //}
                    SetLabelView(choiceView);
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "config": //Choose Database
                    GUIControl.ShowControl(GetID, 34); // hide elements in skin
                    string newConfig = Configuration.Choice_Config(GetID);
                    newConfig = Configuration.Control_Access_Config(newConfig, GetID);
                    if (newConfig != string.Empty) // if user escapes dialog or bad value leave system unchanged
                    {
                        InitMainScreen(); // reset all properties and values
                        //Change "Config":
                        if (facadeView.SelectedListItem != null)
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                        else
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
                        Configuration.CurrentConfig = newConfig;
                        InitialStart = true; //Set to true to make sure initial View is initialized for new DB view
                        MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
                        Load_Config(newConfig, true);
                        if (InitialStart)
                            Fin_Charge_Init(true, true); //Guzzi: need to always load default view on initial start, even if always default view is disabled ...
                        else
                            Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position

                        InitialStart = false; // Guzzi: Set InitialStart to false after initialization done

                        if (MyFilms.conf.StrFanart)
                            backdrop.Active = true;
                        else
                            backdrop.Active = false;
                        MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
                    }
                    break;

                case "nasstatus": //Check and show status of NAS Servers
                    
                    //First check status of configured NAS-Servers
                    WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
                    int intTimeOut = conf.StrWOLtimeout; //Timeout für WOL

                    //GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...

                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    //if (dlg == null) return;
                    //dlg.Reset();
                    System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798727)); // show nas server status
                    dlg.Add(GUILocalizeStrings.Get(712)); //Return
                    choiceSearch.Add("BACK");

                    if (MyFilms.conf.StrNasName1.Length > 0)
                    {
                        if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName1, intTimeOut))
                          dlg.Add("'" + MyFilms.conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798741)); // srv name + " - (active)" 
                        else
                          dlg.Add("'" + MyFilms.conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
                        choiceSearch.Add("NAS1");
                    }

                    if (MyFilms.conf.StrNasName2.Length > 0)
                    {
                        if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName2, intTimeOut))
                          dlg.Add("'" + MyFilms.conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798741)); // srv name + " - (active)" 
                        else
                          dlg.Add("'" + MyFilms.conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
                        choiceSearch.Add("NAS2");
                    }

                    if (MyFilms.conf.StrNasName3.Length > 0)
                    {
                        if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName3, intTimeOut))
                          dlg.Add("'" + MyFilms.conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798741)); // srv name + " - (active)" 
                        else
                          dlg.Add("'" + MyFilms.conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
                        choiceSearch.Add("NAS3");
                    }

                    dlg.DoModal(GetID);
                    if (dlg.SelectedLabel == -1)
                        return;

                    switch (choiceSearch[dlg.SelectedLabel])
                    {
                      case "BACK":
                        return;

                      default:

                        string NasServerName;
                        string NasMACAddress;
                        if (choiceSearch[dlg.SelectedLabel] == "NAS1")
                        {
                          NasServerName = MyFilms.conf.StrNasName1;
                          NasMACAddress = MyFilms.conf.StrNasMAC1;
                        }
                        else if (choiceSearch[dlg.SelectedLabel] == "NAS2")
                        {
                          NasServerName = MyFilms.conf.StrNasName2;
                          NasMACAddress = MyFilms.conf.StrNasMAC2;
                        }
                        else if (choiceSearch[dlg.SelectedLabel] == "NAS3")
                        {
                          NasServerName = MyFilms.conf.StrNasName3;
                          NasMACAddress = MyFilms.conf.StrNasMAC3;
                        }
                        else
                        {
                          NasServerName = String.Empty;
                          NasMACAddress = String.Empty;
                        }

                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); // MyFilms System Information
                        dlgOk.SetLine(1, string.Empty);

                        if (NasMACAddress.Length > 0)
                        {
                          if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(NasMACAddress), NasServerName, intTimeOut))
                          {
                            dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798743)); //successfully started 
                          }
                          else dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798744)); // could not be started 
                        }
                        else
                        {
                          dlgOk.SetLine(1, "Servername: '" + NasServerName + "', MAC: '" + NasMACAddress + "'");
                          dlgOk.SetLine(2, GUILocalizeStrings.Get(10798745)); // start not possible - check config !
                        }
                        dlgOk.DoModal(GetID);
                        break;
                    }
                return;

                case "globaloptions":
                    LogMyFilms.Debug("MF: Building (sub)menu globaloptions");

                    GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg1 == null) return;
                    dlg1.Reset();
                    dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
                    System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

                    // Change global Unwatchedfilteroption
                    // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
                    if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
                    if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
                    choiceViewGlobalOptions.Add("globalunwatchedfilter");

                    // Change global MovieFilter (Only Movies with Trailer)
                    if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
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
                    dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MyFilms.conf.StrAntFilterMinRating.ToString()));
                    choiceViewGlobalOptions.Add("filterdbsetrating");

                    // From ZebonsMerge
                    //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), MesFilms.conf.StrGrabber_ChooseScript.ToString(), (!MesFilms.conf.StrGrabber_ChooseScript).ToString()));   // Choose grabber script for that session
                    if (MyFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798628)));   // Choose grabber script for that session (status on)
                    if (!MyFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798629)));   // Choose grabber script for that session (status off)
                    choiceViewGlobalOptions.Add("choosescript");

                    //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), MesFilms.conf.StrGrabber_Always.ToString(), (!MesFilms.conf.StrGrabber_Always).ToString()));   // Change grabber find trying best match option 
                    if (MyFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798628)));   // Change grabber find trying best match option (status on)
                    if (!MyFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798629)));   // Change grabber find trying best match option (status off)
                    choiceViewGlobalOptions.Add("findbestmatch");

                    //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), MesFilms.conf.WindowsFileDialog.ToString(), (!MesFilms.conf.WindowsFileDialog).ToString()));  // Using Windows File Dialog File for that session
                    if (MyFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798628)));   // Using Windows File Dialog File for that session (status on)
                    if (!MyFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798629)));   // Using Windows File Dialog File for that session (status off)
                    choiceViewGlobalOptions.Add("windowsfiledialog");

                    if (MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
                    if (!MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
                    choiceViewGlobalOptions.Add("alwaysdefaultview");

                    dlg1.DoModal(GetID);
                    if (dlg1.SelectedLabel == -1)
                    {
                        return;
                    }

                    LogMyFilms.Debug("MF: Call global menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");

                    Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
                    return;

                case "globalupdates":

                    GUIDialogMenu dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg2 == null) return;
                    dlg2.Reset();
                    dlg2.SetHeading(GUILocalizeStrings.Get(10798690)); // Global Updates ...
                    System.Collections.Generic.List<string> choiceViewGlobalUpdates = new System.Collections.Generic.List<string>();

                    if (MyFilms.conf.StrAMCUpd)
                    {
                        dlg2.Add(GUILocalizeStrings.Get(1079861));   // Update Database with external AMCupdater
                        choiceViewGlobalUpdates.Add("updatedb");
                    }

                    if (MyFilms.conf.StrFanart)
                    {
                        dlg2.Add(GUILocalizeStrings.Get(4514));   // Download all Fanart
                        choiceViewGlobalUpdates.Add("downfanart");
                    }

                    if (MyFilmsDetail.ExtendedStartmode("Global Update all PersonInfos")) // check if specialmode is configured for disabled features
                    {
                      dlg2.Add(GUILocalizeStrings.Get(10798715)); // Load Person infos - all persons
                        // Search all personinfos
                      choiceViewGlobalUpdates.Add("personinfos-all");
                    }

                    if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
                    {
                      dlg2.Add(GUILocalizeStrings.Get(10798694));
                        // Search and register all trailers for all movies in DB
                      choiceViewGlobalUpdates.Add("trailer-all");
                    }

                    dlg2.DoModal(GetID);

                    if (dlg2.SelectedLabel == -1)
                    {
                        return;
                    }
                    Change_view(choiceViewGlobalUpdates[dlg2.SelectedLabel].ToLower());
                    return;

                case "globalwikihelp":
                    var hasRightPlugin = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb");
                    var hasRightVersion = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb" && plugin.GetType().Assembly.GetName().Version.Minor >= 0);
                    if (hasRightPlugin && hasRightVersion)
                    {
                      //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser
                      //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
                      string url = "http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films";
                      string zoom = "150";

                      //Load Webbrowserplugin with the URL
                      LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                      GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                      GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                      GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                      GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                      GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                    }
                    else
                    {
                      ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0");
                    }
                    break;

                case "about":
                      GUIDialogOK dlgok = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                      if (dlgok == null) return;
                      dlgok.Reset();
                      dlgok.SetHeading(GUILocalizeStrings.Get(10798624)); // MyFilms System Information

                      System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                      dlgok.SetLine(1, "MyFilms Version = 'V" + asm.GetName().Version.ToString() + "'");
                      dlgok.SetLine(2, "MyFilms Operations Mode = '" + Configuration.PluginMode + "'");
                      dlgok.DoModal(GetID);
                    break;
              
                case "globalunwatchedfilter":
                    // Global overlayfilter for unwatched movies ...
                    MyFilms.conf.GlobalUnwatchedOnly = !MyFilms.conf.GlobalUnwatchedOnly;
                    if (conf.GlobalUnwatchedOnly)
                    {
                      GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
                      MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
                    }
                    else
                    {
                      GlobalFilterStringUnwatched = String.Empty;
                      MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
                    }
                    LogMyFilms.Info("MF: Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    if (!Context_Menu)
                      Change_view("globaloptions");
                    else 
                      Context_Menu = false;
                    break;

                case "filterdbtrailer":
                    // GlobalFilterTrailersOnly
                    GlobalFilterTrailersOnly = !GlobalFilterTrailersOnly;
                    LogMyFilms.Info("MF: Global filter for Trailers Only is now set to '" + GlobalFilterTrailersOnly + "'");
                    //if (GlobalFilterTrailersOnly) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                    //if (!GlobalFilterTrailersOnly) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
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
                    LogMyFilms.Info("MF: (SetGlobalFilterString Trailers) - 'GlobalFilterStringTrailersOnly' = '" + GlobalFilterStringTrailersOnly + "'");
                    GUIWaitCursor.Show();
                    //GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    if (!Context_Menu)
                      Change_view("globaloptions");
                    else
                      Context_Menu = false;
                    break;

                case "filterdbrating":
                    // GlobalFilterMinRating
                    GlobalFilterMinRating = !GlobalFilterMinRating;
                    LogMyFilms.Info("MF: Global filter for MinimumRating is now set to '" + GlobalFilterMinRating + "'");
                    if (GlobalFilterMinRating)
                    {
                      GlobalFilterStringMinRating = "Rating > " + MyFilms.conf.StrAntFilterMinRating.Replace(",", ".") + " AND ";
                      MyFilmsDetail.setGUIProperty("globalfilter.minrating", "true");
                      MyFilmsDetail.setGUIProperty("globalfilter.minratingvalue", MyFilms.conf.StrAntFilterMinRating);
                    }
                    else
                    {
                      GlobalFilterStringMinRating = String.Empty;
                      MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
                      MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
                    }
                    LogMyFilms.Info("MF: (SetGlobalFilterString MinRating) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
                    GUIWaitCursor.Show();
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    if (!Context_Menu)
                      Change_view("globaloptions");
                    else
                      Context_Menu = false;
                    break;

                case "filterdbsetrating":
                    // Set global value for minimum Rating to restrict movielist
                    LogMyFilms.Info("MF: (FilterDbSetRating) - 'AntFilterMinRating' current setting = '" + MyFilms.conf.StrAntFilterMinRating + "', current decimalseparator: '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString() + "'");
                    MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(ID_MyFilmsDialogRating);
                    if (MyFilms.conf.StrAntFilterMinRating.Length > 0)
                    {
                      decimal wrating = 0;
                      //NumberFormatInfo nfi = new NumberFormatInfo();
                      //nfi.NumberDecimalSeparator = ",";
                      //nfi.NumberGroupSeparator = "";
                      //if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." && MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Contains("."))
                      //{
                      //  wrating = Convert.ToDecimal(MyFilms.conf.StrAntFilterMinRating.Replace(",", "."));
                      //}
                      wrating = Convert.ToDecimal(MyFilms.conf.StrAntFilterMinRating);
                      //try { wrating = Convert.ToDecimal(MyFilms.conf.StrAntFilterMinRating); }
                      //    catch
                      //    {
                      //      try
                      //      {
                      //        wrating = Convert.ToDecimal(MyFilms.conf.StrAntFilterMinRating, CultureInfo.CurrentCulture);
                      //        LogMyFilms.Debug("MF: Filter Rating dialog using cultureinfo: '" + CultureInfo.CurrentCulture.ToString() + "'");
                      //      }
                      //        catch { }
                      //    }
                      dlgRating.Rating = wrating;
                    }
                    else
                        dlgRating.Rating = 0;
                    dlgRating.SetTitle(GUILocalizeStrings.Get(1079881));
                    dlgRating.DoModal(GetID);

                    //NumberFormatInfo nfiback = new NumberFormatInfo();
                    //nfiback.NumberDecimalSeparator = ".";
                    //nfiback.NumberGroupSeparator = "";
                    //MyFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString("0.0", nfiback);
                    MyFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString("0.0");
                    //LogMyFilms.Debug("MF: Storing StrAntFilterMinRating with formatted string value: '" + dlgRating.Rating.ToString("0.0") + "'");
                    //MyFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString().Replace("," , ".");
                    //LogMyFilms.Debug("MF: Rating dialog using cultureinfo: '" + CultureInfo.CurrentCulture.ToString() + "'");
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", MyFilms.conf.StrAntFilterMinRating);
                    LogMyFilms.Info("MF: (FilterDbSetRating) - 'AntFilterMinRating' changed to '" + MyFilms.conf.StrAntFilterMinRating + "'");
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    if (GlobalFilterMinRating)
                    {
                      GlobalFilterStringMinRating = "Rating > " + MyFilms.conf.StrAntFilterMinRating.Replace(",", ".") + " AND ";
                      MyFilmsDetail.setGUIProperty("globalfilter.minrating", "true");
                      MyFilmsDetail.setGUIProperty("globalfilter.minratingvalue", MyFilms.conf.StrAntFilterMinRating);
                    }
                    else
                    {
                      GlobalFilterStringMinRating = String.Empty;
                      MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
                      MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
                    }
                    LogMyFilms.Info("MF: (SetGlobalFilterString) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
                    GUIWaitCursor.Show();
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    if (!Context_Menu)
                      Change_view("globaloptions");
                    else
                      Context_Menu = false;
                    break;

                case "updatedb":
                    // Launch AMCUpdater in batch mode
                    if (bgUpdateDB.IsBusy)
                    {
                        ShowMessageDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(875), GUILocalizeStrings.Get(330)); //action already launched
                        break;
                    }
                    AsynUpdateDatabase();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "downfanart":
                    // Launch Fanart download in batch mode
                    if (bgUpdateFanart.IsBusy)
                    {
                        ShowMessageDialog(GUILocalizeStrings.Get(1079862), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
                        break;
                    }
                    AsynUpdateFanart();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "personinfos-all":
                    // Search and update all personinfos from IMDB
                    // Todo: Call Singlepersonupdate - maybe this function should also be available for single movies ? (less traffic)
                    // ToDo: - first implement singlepersonupdate ...
                    break;
                
                case "trailer-all":
                    // Launch "Search and register all trailers for all movies in DB" in batch mode
                    //if (bgUpdateTrailer.IsBusy)
                    //{
                    //    ShowMessageDialog(GUILocalizeStrings.Get(10798694), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
                    //    break;
                    //}
                    //AsynUpdateTrailer();
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    //break;

                    AntMovieCatalog ds = new AntMovieCatalog();
                    ArrayList w_index = new ArrayList();
                    int w_index_count = 0;
                    string t_number_id = "";
                    DataRow[] wr = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //Now build a list of valid movies in w_index with Number registered
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            //LogMyFilms.Debug("MF: (GlobalSearchTrailerLocal) - dc.ColumnName '" + dc.ColumnName.ToString() + "'");
                            if (dc.ColumnName.ToString() == "Number")
                            {
                                t_number_id = wsr[dc.ColumnName].ToString();
                                //LogMyFilms.Debug("MF: (GlobalSearchTrailerLocal) - Movienumber stored as '" + t_number_id + "'");
                            }
                        }
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if (dc.ColumnName.ToLower() == "translatedtitle")
                                {
                                    w_index.Add(t_number_id);
                                    LogMyFilms.Debug("MF: (GlobalSearchTrailerLocal) - Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                                    w_index_count = w_index_count + 1;
                                }
                        }
                    }
                    LogMyFilms.Debug("MF: (GlobalSearchTrailerLocal) - Number of Records found: " + w_index_count);

                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798800)); // Warning: Long runtime !
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(10798801)); //should really the trailer search be started
                    dlgYesNo.SetLine(2, string.Format(GUILocalizeStrings.Get(10798802), w_index_count.ToString())); // for <xx> movies ?
                    dlgYesNo.DoModal(GetID);
                    if (!(dlgYesNo.IsConfirmed))
                        break;
                    GUIWaitCursor.Show();
                    for (i = 0; i < w_index_count; i++)
                    {
                        LogMyFilms.Debug("MF: (GlobalSearchTrailerLocal) - Number: '" + i.ToString() + "' - Index to search: '" + w_index[i] + "'");
                        //MyFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(w_index[i]));
                        MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, Convert.ToInt32(i), false);
                    }
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798695)); //Traiersearch finished!
                    break;
                
                case "choosescript":
                    MyFilms.conf.StrGrabber_ChooseScript = !MyFilms.conf.StrGrabber_ChooseScript;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_ChooseScript", MyFilms.conf.StrGrabber_ChooseScript);
                    LogMyFilms.Info("MF: Grabber Option 'use always that script' changed to " + MyFilms.conf.StrGrabber_ChooseScript.ToString());
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    Change_view("globaloptions");
                    break;
                case "findbestmatch":
                    MyFilms.conf.StrGrabber_Always = !MyFilms.conf.StrGrabber_Always;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_Always", MyFilms.conf.StrGrabber_Always);
                    LogMyFilms.Info("MF: Grabber Option 'try to find best match...' changed to " + MyFilms.conf.StrGrabber_Always.ToString());
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    Change_view("globaloptions");
                    break;
                case "windowsfiledialog":
                    MyFilms.conf.WindowsFileDialog = !MyFilms.conf.WindowsFileDialog;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WindowsFileDialog", MyFilms.conf.WindowsFileDialog);
                    LogMyFilms.Info("MF: Update Option 'use Windows File Dialog...' changed to " + MyFilms.conf.WindowsFileDialog.ToString());
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    Change_view("globaloptions");
                    break;
                case "alwaysdefaultview":
                    MyFilms.conf.AlwaysDefaultView = !MyFilms.conf.AlwaysDefaultView;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AlwaysDefaultView", MyFilms.conf.AlwaysDefaultView);
                    LogMyFilms.Info("MF: Update Option 'use always default view...' changed to " + MyFilms.conf.AlwaysDefaultView.ToString());
                    GUIWaitCursor.Show();
                    //GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...

                    //if (MesFilms.conf.AlwaysDefaultView) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                    //if (!MesFilms.conf.AlwaysDefaultView) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                    
                    if (MyFilms.conf.AlwaysDefaultView)
                        Fin_Charge_Init(true, true); //DefaultSelect, reload
                    else
                        Fin_Charge_Init(true, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    Change_view("globaloptions");
                    break;
            }
        }
        //--------------------------------------------------------------------------------------------
        //   Display Context Menu for Movie 
        //--------------------------------------------------------------------------------------------
        // Changed from private to PUBLIC - GUZZI - Original ZebonsMerge was private
		// private void Context_Menu_Movie(int selecteditem)

        public void Context_Menu_Movie(int selecteditem)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;
            Context_Menu = true;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
            string[] upd_choice = new string[20];
            int ichoice = 0;

            // Moviecontext
            if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(1079866));//Search related movies by persons
                upd_choice[ichoice] = "analogyperson";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
                upd_choice[ichoice] = "analogyproperty";
                ichoice++;

                if (facadeView.Focus && !facadeView.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
                {
                  dlg.Add(GUILocalizeStrings.Get(10798709));//play movie 
                  upd_choice[ichoice] = "playmovie";
                  ichoice++;
                }
              
                if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
                {
                  string trailercount = "";
                  if (string.IsNullOrEmpty(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
                    trailercount = "0";
                  else
                  {
                    string[] split1 = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorageTrailer].ToString().Trim().Split(new Char[] { ';' });
                    trailercount = split1.Count().ToString();
                    if (trailercount != "0")
                    {
                    dlg.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")");//play trailer (<number trailers present>)
                    upd_choice[ichoice] = "playtrailer";
                    ichoice++;
                    }
                  }
                }

                if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
                {
                  if (facadeView.SelectedListItem.IsPlayed) // show only the required option
                    dlg.Add(GUILocalizeStrings.Get(1079895)); // set unwatched
                  else dlg.Add(GUILocalizeStrings.Get(1079894)); // set watched
                  upd_choice[ichoice] = "togglewatchedstatus";
                  ichoice++;
                }

                // Enable/disable global overlay filter could be added here for faster access ?
                // ...
                //dlg.Add(GUILocalizeStrings.Get(10798714)); // "global filters ..."
                //upd_choice[ichoice] = "globalfilters";
                //ichoice++;

                if (MyFilmsDetail.ExtendedStartmode("Context: IMDB Trailer and Pictures")) // check if specialmode is configured for disabled features
                {
                  dlg.Add(GUILocalizeStrings.Get(1079887));
                  upd_choice[ichoice] = "movieimdbtrailer";
                  ichoice++;
                }

                dlg.Add(GUILocalizeStrings.Get(1079888));
                upd_choice[ichoice] = "movieimdbbilder";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079889));
                upd_choice[ichoice] = "movieimdbinternet";
                ichoice++;

                if (MyFilmsDetail.ExtendedStartmode("Context: Personlist in facade")) // check if specialmode is configured for disabled features
                {
                  dlg.Add(GUILocalizeStrings.Get(1079879));//Search Infos to related persons (load persons in facadeview) - only available in filmlist
                  upd_choice[ichoice] = "moviepersonlist";
                  ichoice++;
                }

                if (MyFilmsDetail.ExtendedStartmode("Context: IMDB Update for all persons of movie")) // check if specialmode is configured for disabled features
                {
                  dlg.Add(GUILocalizeStrings.Get(1079883)); // update personinfos for all envolved persons of a selected movie from IMDB
                  upd_choice[ichoice] = "updatepersonmovie";
                  ichoice++;
                }
            }

            // Artistcontext
            if (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder && (conf.WStrSort.ToLower().Contains("actor") || conf.WStrSort.ToLower().Contains("director") || conf.WStrSort.ToLower().Contains("producer")))
            {
              if (MyFilmsDetail.ExtendedStartmode("Context Artist: Show Infos of person locally (load persons detailscreen or load facade with filmlists of actor")) // check if specialmode is configured for disabled features
              {
                dlg.Add(GUILocalizeStrings.Get(1079884));
                //Show Infos of person (load persons detailscreen - MesFilmsActor) - only available in personlist
                upd_choice[ichoice] = "artistdetail";
                ichoice++;
              }

              dlg.Add(GUILocalizeStrings.Get(1079886));//Show IMDB internetinfos http://www.imdb.com/name/nm0000288/
              upd_choice[ichoice] = "artistimdbinternet";
              ichoice++;
              
              dlg.Add(GUILocalizeStrings.Get(1079890));//Show IMDB clips http://www.imdb.com/name/nm0000288/videogallery
              upd_choice[ichoice] = "artistimdbclips";
              ichoice++;

              dlg.Add(GUILocalizeStrings.Get(1079891));//Show IMDB pictures http://www.imdb.com/name/nm0000288/mediaindex
              upd_choice[ichoice] = "artistimdbbilder";
              ichoice++;

              if (MyFilmsDetail.ExtendedStartmode("Context Artist: IMDB all sort of details and updates (several entries)")) // check if specialmode is configured for disabled features
              {
                dlg.Add(GUILocalizeStrings.Get(1079885));//Show IMDB filmlist in facadeview and add availabilityinformations to it
                upd_choice[ichoice] = "artistimdbfilmlist";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079882)); // update personinfo from IMDB and create actorthumbs - optional: load mediathek for person backdrops etc.
                upd_choice[ichoice] = "updateperson";
                ichoice++;
              }
            }

            if (MyFilms.conf.StrSuppress && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(432));
                upd_choice[ichoice] = "suppress";
                ichoice++;
            }
            //Disabled due to problems of updating DB (requires debugging before reenabling...)
            //if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            //{
            //    dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
            //    upd_choice[ichoice] = "grabber";
            //    ichoice++;
            //}
            if (MyFilms.conf.StrFanart && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(1079862));
                upd_choice[ichoice] = "fanart";
                ichoice++;
                dlg.Add(GUILocalizeStrings.Get(1079874));
                upd_choice[ichoice] = "deletefanart";
                ichoice++;
            }

            //if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            //{
            //    dlg.Add(GUILocalizeStrings.Get(1079892)); // Update ...
            //    upd_choice[ichoice] = "updatemenu";
            //}

            ichoice++;

            dlg.DoModal(GetID);

            if (dlg.SelectedLabel == -1)
                return;
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            switch (upd_choice[dlg.SelectedLabel])
            {
                case "playmovie":
                  MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                  break;

                case "playtrailer":
                  // first check, if trailer files are available, offer options
                  //if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
                  if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
                  {
                    MyFilmsDetail.trailerPlayed = true;
                    MyFilmsDetail.Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                  }
                  else
                  {
                    // Can add autosearch&register logic here before try starting trailers

                    GUIDialogYesNo dlgYesNotrailersearch = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    dlgYesNotrailersearch.SetHeading(GUILocalizeStrings.Get(10798704));//trailer
                    dlgYesNotrailersearch.SetLine(1, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle].ToString());//video title
                    dlgYesNotrailersearch.SetLine(2, GUILocalizeStrings.Get(10798737));//no video found locally
                    dlgYesNotrailersearch.SetLine(3, GUILocalizeStrings.Get(10798739)); // Search local trailers  and update DB ?
                    dlgYesNotrailersearch.DoModal(GetID);
                    //dlgYesNotrailersearch.DoModal(GUIWindowManager.ActiveWindow);
                    if (dlgYesNotrailersearch.IsConfirmed)
                    {
                      //setProcessAnimationStatus(true, m_SearchAnimation);
                      //LogMyFilms.Debug("MF: (SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "'"));
                      LogMyFilms.Debug("MF: (Auto search trailer after selecting PLAY) title: '" + (MyFilms.r[MyFilms.conf.StrIndex].ToString() + "'"));
                      MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                      //afficher_detail(true);
                      //setProcessAnimationStatus(false, m_SearchAnimation);
                      MyFilmsDetail.trailerPlayed = true;
                      MyFilmsDetail.Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                    }
                  }
                  break;

                case "analogyperson":
                    {
                        SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();
                        break;
                    }

                case "analogyproperty":
                    {
                        SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();
                        break;
                    }

                case "movieimdbtrailer":
                    // Example: (http://www.imdb.com/title/tt0438488/videogallery)
                    {
                      if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
                      {
                        string url = ImdbBaseUrl + "";
                        string zoom = "150";
                        IMDB _imdb = new IMDB();
                        IMDB.IMDBUrl wurl;
                        _imdb.Find(facadeView.SelectedListItem.Label);
                        IMDBMovie imdbMovie = new IMDBMovie();
                        if (_imdb.Count > 0)
                        {
                          wurl = (IMDB.IMDBUrl)_imdb[0];
                          if (wurl.URL.Length != 0)
                          {
                            url = wurl.URL + @"videogallery";
                            url = ImdbBaseUrl + url.Substring(url.IndexOf("title")); // redirect to base www.imdb.com server and remove localized returns...
                          }
                        
                          LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                          GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                        }
                        else
                        {
                          ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                          break;
                        }

                      }
                      else
                      {
                        ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
                      }
                      break;
                    }

                case "movieimdbbilder":
                    // example: http://www.imdb.com/title/tt0133093/mediaindex
                    {
                      if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
                      {
                        string url = ImdbBaseUrl + "";
                        string zoom = "150";
                        IMDB _imdb = new IMDB();
                        IMDB.IMDBUrl wurl;
                        _imdb.Find(facadeView.SelectedListItem.Label);
                        IMDBMovie imdbMovie = new IMDBMovie();
                        if (_imdb.Count > 0)
                        {
                          wurl = (IMDB.IMDBUrl)_imdb[0];
                          if (wurl.URL.Length != 0)
                          {
                            url = wurl.URL + @"mediaindex";
                            url = ImdbBaseUrl + url.Substring(url.IndexOf("title")); // redirect to base www.imdb.com server and remove localized returns...
                          }
                          LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                          GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                        }
                        else
                        {
                          ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                          break;
                        }
                      }
                      else
                      {
                        ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
                      }
                      break;
                    }

                case "movieimdbinternet":
                    if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
                    {
                        //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser
                        //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
                        string url = ImdbBaseUrl + "";
                        string zoom = "150";

                        //First search corresponding URL for the actor ...
                        IMDB _imdb = new IMDB();
                        IMDB.IMDBUrl wurl;

                        _imdb.Find(facadeView.SelectedListItem.Label);
                        IMDBMovie imdbMovie = new IMDBMovie();
                        for (int i = 0; i < _imdb.Count; i++)
                        {
                          LogMyFilms.Debug("MF: movie imdb internet search - found: '" + _imdb[i].Title + "', URL = '" + _imdb[i].URL + "'");
                        }
                        if (_imdb.Count > 0)
                        {
                            wurl = (IMDB.IMDBUrl)_imdb[0];
                            if (wurl.URL.Length != 0)
                            {
                              url = wurl.URL; // Assign proper Webpage for Actorinfos
                              //url = ImdbBaseUrl + url.Substring(url.IndexOf(".com") + 5); // redirect to base www.imdb.com server and remove localized returns...
                            }
                          //Load Webbrowserplugin with the URL
                          LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                          GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                        }
                        else
                        {
                          ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                          break;
                        }

                        }
                    else
                    {
                        ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
                    }
                    break;
                    
                case "moviepersonlist":
                    {
                        //To be modified to call new class with personlist by type and call MyFilmsActors with facade
                        //Temporarily disabled - will be required to create Person List later ....
                        //SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);
                        if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
                        // Load Facade with movie actors 
                        // ToDo: Load all artists, including producers and directors
                        {
                            conf.StrIndex = facadeView.SelectedListItem.ItemId;
                            conf.StrTIndex = facadeView.SelectedListItem.Label;

                            conf.WStrSort = "ACTORS";
                            conf.Wselectedlabel = string.Empty;
                            conf.WStrSortSens = " ASC";
                            BtnSrtBy.IsAscending = true;
                            conf.StrActors = "*";
                            getSelectFromDivx("TranslatedTitle like '*" + conf.StrTIndex + "*'", conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, string.Empty);
                        }
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        //dlg.DeInit();
                        break;
                    }

                case "artistdetail":
                    // ToDo: Launch MesFilmsActorinfo - load new facade for LOCAL (!) IMDB-mediaindex
                    // ToDo: Optional add switch to seitch between filmclips (IMDB-videogallery) and photos (IMDB-mediaindex) - launch Onlinevideos to play clips?
                    {
                        break;
                    }

                case "artistimdbclips": // Example: http://www.imdb.com/name/nm0000206/videogallery
                    {
                      if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
                      { //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
                        string url = ImdbBaseUrl + "";
                        string zoom = "150";
                        //First search corresponding URL for the actor ...
                        Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                        Grabber.MyFilmsIMDB.IMDBUrl wurl;
                        _imdb.FindActor(facadeView.SelectedListItem.Label);
                        Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();

                        if (_imdb.Count > 0)
                        {
                          wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                          if (wurl.URL.Length != 0)
                          {
                            url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                            url = ImdbBaseUrl + url.Substring(url.IndexOf("name")); // redirect to base www.imdb.com server and remove localized returns...
                          }
                          //_imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
                          LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                          GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                        }
                        else
                        {
                          ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                          break;
                        }
                      }
                      else
                      {
                        ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
                      }
                      break;
                    }

                case "artistimdbbilder": // Example: http://www.imdb.com/name/nm0000206/mediaindex
                    
                    {
                      if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
                      { //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
                        string url = ImdbBaseUrl + "";
                        string zoom = "150";
                        //First search corresponding URL for the actor ...
                        Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                        Grabber.MyFilmsIMDB.IMDBUrl wurl;
                        _imdb.FindActor(facadeView.SelectedListItem.Label);
                        Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();

                        if (_imdb.Count > 0)
                        {
                          wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                          if (wurl.URL.Length != 0)
                          {
                            url = wurl.URL + "mediaindex"; // Assign proper Webpage for Actorinfos
                            url = ImdbBaseUrl + url.Substring(url.IndexOf("name")); // redirect to base www.imdb.com server and remove localized returns...
                          }
                          //_imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
                          LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                          GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                          GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                          GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                        }
                        else
                        {
                          ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                          break;
                        }
                      }
                      else
                      {
                        ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
                      }
                      break;
                    }
                    
                case "artistimdbinternet": // Example: http://www.imdb.com/name/nm0000206/

                    if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
                    { //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
                      string url = ImdbBaseUrl + "";
                      string zoom = "150";

                      //First search corresponding URL for the actor ...
                      Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                      Grabber.MyFilmsIMDB.IMDBUrl wurl;

                      _imdb.FindActor(facadeView.SelectedListItem.Label);
                      Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();

                      if (_imdb.Count > 0)
                      {
                        //int index = IMDBFetcher.FuzzyMatch(actor);
                        //int index;
                        //for (index = 0; index < _imdb.Count; ++index)
                        //{
                        //    wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[index];
                        //    if (wurl.URL.Length != 0) url = wurl.URL; // Assign proper Webpage for Actorinfos
                        //    _imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
                        //}
                        wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                        if (wurl.URL.Length != 0)
                        {
                          url = wurl.URL; // Assign proper Webpage for Actorinfos
                          //url = ImdbBaseUrl + url.Substring(url.IndexOf(".com" + 4)); // redirect to base www.imdb.com server and remove localized returns...
                        }
                        //_imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
                        //Load Webbrowserplugin with the URL
                        LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                        GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                        GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                        GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                        GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                        GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
                      }
                      else
                      {
                        ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                        break;
                      }
                    }
                    else
                    {
                      ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
                    }
                    break;

                case "artistimdbfilmlist":
                    // ToDo: Launch IMDB-Internetpage via actor-URL in Webbrowserplugin - check InfoPlugin how to implement ...
                    // Share loadingclass with actorclips and use parameter "mode"
                    {
                        break;
                    }

                case "updateperson":
                    {
                        //Todo: add calls to update the personinfos from IMDB - use database and grabberclasses from MePo / Deda
                        ArtistIMDBpictures(facadeView.SelectedListItem.Label); // Call Updategrabber with Textlabel/Actorname
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();


                        //First search corresponding URL for the actor ...
                        // bool director = false; // Actor is director // Currently not used...
                        IMDB _imdb = new IMDB();
                        //IMDB.IMDBUrl wurl;
                        //newGrab.FindActor(facadeView.SelectedListItem.Label);
                        ArrayList actorList = new ArrayList();
                        //if (_imdb.Count > 0)
                        {
                            string actor = facadeView.SelectedListItem.Label;
                            //string test = _imdb[0].IMDBURL;
                            _imdb.FindActor(actor);
                            IMDBActor imdbActor = new IMDBActor();
                            string ttt = imdbActor.ThumbnailUrl;
                            if (_imdb.Count > 0)
                            {
                                //int index = IMDBFetcher.FuzzyMatch(actor);
                                int index;
                                int matchingIndex = -1;
                                int matchingDistance = int.MaxValue;
                                bool isAmbiguous = false;

                                for (index = 0; index < _imdb.Count; ++index)
                                {
                                    int distance = Levenshtein.Match(actor, _imdb[index].Title);

                                    if (distance == matchingDistance && matchingDistance != int.MaxValue)
                                    {
                                        isAmbiguous = true;
                                    }

                                    if (distance < matchingDistance)
                                    {
                                        isAmbiguous = false;
                                        matchingDistance = distance;
                                        matchingIndex = index;
                                    }
                                }

                                if (isAmbiguous)
                                {
                                    matchingIndex = 0;
                                }


                                //LogMyFilms.Info("Getting actor:{0}", _imdb[index].Title);
                                //_imdb.GetActorDetails(_imdb[index], director, out imdbActor);

#if MP11
                                //_imdb.GetActorDetails(_imdb[index], out imdbActor);
#else
                                //_imdb.GetActorDetails(_imdb[index], out imdbActor);
#endif

                                //LogMyFilms.Info("Adding actor:{0}({1}),{2}", imdbActor.Name, actor, percent);
                                int actorId = MediaPortal.Video.Database.VideoDatabase.AddActor(imdbActor.Name);
                                if (actorId > 0)
                                {
                                    MediaPortal.Video.Database.VideoDatabase.SetActorInfo(actorId, imdbActor);
                                    //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId); // Guzzi: Removed, only updating Actorinfos

                                    if (imdbActor.ThumbnailUrl != string.Empty)
                                    {
                                        string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                                        string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                                        Utils.FileDelete(largeCoverArt);
                                        Utils.FileDelete(coverArt);
                                        MediaPortal.Video.Database.IMDBFetcher.DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
                                    }
                                }
                            }
                            else
                            {
                                int actorId = MediaPortal.Video.Database.VideoDatabase.AddActor(actor);
                                imdbActor.Name = actor;
                                IMDBActor.IMDBActorMovie imdbActorMovie = new IMDBActor.IMDBActorMovie();
                                //imdbActorMovie.MovieTitle = _movieDetails.Title;
                                //imdbActorMovie.Year = _movieDetails.Year;
                                //imdbActorMovie.Role = role;
                                imdbActor.Add(imdbActorMovie);
                                MediaPortal.Video.Database.VideoDatabase.SetActorInfo(actorId, imdbActor);
                                //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
                            }
                        }

                        MyFilmsDetail.GetActorByName(facadeView.SelectedListItem.Label, actorList);
                        //MediaPortal.Video.Database.VideoDatabase.GetActorByName(facadeView.SelectedListItem.Label, actorList);

                        if (actorList.Count == 0)
                        {
                            GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                            dlgOk.SetHeading("Info");
                            dlgOk.SetLine(1, string.Empty);
                            dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
                            dlgOk.DoModal(GetID);
                            return;
                        }
                        actorID = 0;
                        string actorname = string.Empty;
                        char[] splitter = { '|' };
                        foreach (string act in actorList)
                        {
                            string[] strActor = act.Split(splitter);
                            actorID = Convert.ToInt32(strActor[0]);
                            actorname = strActor[1];
                        }

                        //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
                        //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
                        //if (actor != null)

                        //OnVideoArtistInfoGuzzi(actor);



                        IMDB GrabArtist = new IMDB();
                        //IMDB.IMDBUrl wwurl;

                        GrabArtist.FindActor(facadeView.SelectedListItem.Label);

                        //int listCount = listUrl.Count;

                        //url = new IMDBSearch();
                        //MediaPortal.Video.Database.IMDB.IMDBUrl .FindActor(facadeView.SelectedListItem.Label));
                        //Load Webbrowserplugin with the URL
                        //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
                        string url = ImdbBaseUrl + string.Empty;
                        string zoom = "100";
                        //value = value.Replace("%link%", url);
                        //value = value.Replace("%zoom%", zoom);
                        LogMyFilms.Debug("MF: Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                        GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                        GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                        GUIWindowManager.ActivateWindow(ID_BrowseTheWeb, false); //54537689
                        GUIPropertyManager.SetProperty("#btWeb.startup.link", "");
                        GUIPropertyManager.SetProperty("#btWeb.link.zoom", "");
                    }
                    break;

                case "updatepersonmovie":
                    // ToDo: Update personinfo for all involve artists (takes longer!)
                    {
                        break;
                    }

                case "suppress":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                    {
                        MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)facadeView.SelectedListItem.ItemId);
                        Fin_Charge_Init(true, true);
                    }
                    break;
                case "grabber":
                    string title = string.Empty;
                    if (!string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabber) && !string.IsNullOrEmpty(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.ItemSearchGrabber].ToString()))
                    {
                      title = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.ItemSearchGrabber].ToString(); // Configured GrabberTitle
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with '" + MyFilms.conf.ItemSearchGrabber + "' = '" + title.ToString() + "'");
                    }
                    else if (!string.IsNullOrEmpty(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString())) // Mastertitle
                    {
                      title = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString();
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with (master)title = '" + title.ToString() + "'");
                    }
                    else if (!string.IsNullOrEmpty(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrTitle2].ToString())) // Secondary title
                    {
                      title = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrTitle2].ToString();
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with (secondary)title = '" + title.ToString() + "'");
                    }
                    else if (!string.IsNullOrEmpty(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorage].ToString())) // Name from source (media)
                    {
                      title = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorage].ToString();
                      if (title.Contains(";")) title = title.Substring(0, title.IndexOf(";"));
                      if (title.Contains("\\")) title = title.Substring(title.LastIndexOf("\\") + 1);
                      if (title.Contains(".")) title = title.Substring(0, title.LastIndexOf("."));
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with (media source)name = '" + title.ToString() + "'");
                    }

                    if (title.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    MyFilmsDetail.grabb_Internet_Informations(title, GetID, MyFilms.conf.StrGrabber_ChooseScript, MyFilms.conf.StrGrabber_cnf);
                    Fin_Charge_Init(false, true); // Guzzi: This might be required to reload facade and details ?
                    break;

                case "fanart":
                    Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
                    string wtitle = string.Empty;
                    if (MyFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"] != null && MyFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"].ToString().Length > 0)
                        wtitle = MyFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"].ToString();
                    if (wtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        wtitle = wtitle.Substring(wtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    string wttitle = string.Empty;
                    if (MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"] != null && MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString().Length > 0)
                        wttitle = MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString();
                    if (wttitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        wttitle = wttitle.Substring(wttitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    if (wtitle.Length > 0 && MyFilms.conf.StrFanart)
                    {
                      MyFilmsDetail.Download_Backdrops_Fanart(wtitle, wttitle, MyFilms.r[facadeView.SelectedListItem.ItemId]["Director"].ToString(), MyFilms.r[facadeView.SelectedListItem.ItemId]["Year"].ToString(), true, GetID, wtitle);
                    }
                    //if (wttitle != null && wttitle.Length > 0)
                    //    wtitle = wttitle;
                    string[] wfanart = MyFilmsDetail.Search_Fanart(facadeView.SelectedListItem.Label, true, "file", false, facadeView.SelectedListItem.ThumbnailImage, string.Empty);
                    if (wfanart[0] == " ")
                    {
                        backdrop.Active = false;
                        GUIControl.HideControl(GetID, 35);
                    }
                    else
                    {
                        backdrop.Active = true;
                        GUIControl.ShowControl(GetID, 35);
                    }
                    LogMyFilms.Debug("MF: (Backdrops-NewfromContext): backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '"+ wfanart[1] + "'");
                    backdrop.Filename = wfanart[0];
                    break;
                case "deletefanart":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my videos
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                      //MyFilmsDetail.Remove_Backdrops_Fanart(MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString(), false);
                      MyFilmsDetail.Remove_Backdrops_Fanart(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString(), false); // Fixed, as it should delete content of mastertitle...
                    break;

                case "togglewatchedstatus":
                    if (facadeView.SelectedListItem.IsPlayed)
                    {
                      facadeView.SelectedListItem.IsPlayed = false;
                      MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)facadeView.SelectedListItem.ItemId, false);
                    }
                    else
                    {
                      facadeView.SelectedListItem.IsPlayed = true;
                      MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)facadeView.SelectedListItem.ItemId, true);
                    }
                    //Fin_Charge_Init(true, true);
                    break;

                case "updatemenu":
                    GUIDialogMenu dlgupdate = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlgupdate == null) return;
                    Context_Menu = true;
                    dlgupdate.Reset();
                    dlgupdate.SetHeading(GUILocalizeStrings.Get(1079892)); // Update ...


                    if (MyFilms.conf.StrSuppress && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                    {
                        dlg.Add(GUILocalizeStrings.Get(432));
                        upd_choice[ichoice] = "suppress";
                        ichoice++;
                    }
                    if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                    {
                        dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                        upd_choice[ichoice] = "grabber";
                        ichoice++;
                    }
                    if (MyFilms.conf.StrFanart && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                    {
                        dlg.Add(GUILocalizeStrings.Get(1079862));
                        upd_choice[ichoice] = "fanart";
                        ichoice++;
                        dlg.Add(GUILocalizeStrings.Get(1079874));
                        upd_choice[ichoice] = "deletefanart";
                        ichoice++;
                    }

                    dlg.DoModal(GetID);

                    if (dlg.SelectedLabel == -1)
                    {
                      Context_Menu = false;
                      return;
                    }
                    break;

              case "globalfilters":
                    GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg1 == null) return;
                    dlg1.Reset();
                    dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
                    System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

                    // Change global Unwatchedfilteroption
                    // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
                    if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
                    if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
                    choiceViewGlobalOptions.Add("globalunwatchedfilter");

                    // Change global MovieFilter (Only Movies with Trailer)
                    if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
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

                    //if (MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
                    //if (!MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
                    //choiceViewGlobalOptions.Add("alwaysdefaultview");

                    dlg1.DoModal(GetID);
                    if (dlg1.SelectedLabel == -1)
                    {
                      return;
                    }

                    LogMyFilms.Debug("MF: Call global menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");

                    Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
                    //Context_Menu = false;
                    return;

            }
        }

        //*****************************************************************************************
        //*  search related movies by persons                                                     *
        //*****************************************************************************************
        private void SearchRelatedMoviesbyPersons(int Index)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
            ArrayList w_tableau = new ArrayList();
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            if (MyFilms.r[Index]["Producer"].ToString().Length > 0)
            {
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Producer"].ToString())));
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(10798612) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (MyFilms.r[Index]["Director"].ToString().Length > 0)
            {
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Director"].ToString())));
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079869) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (MyFilms.r[Index]["Actors"].ToString().Length > 0)
            {
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Actors"].ToString())));
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079868) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (choiceSearch.Count == 0)
            {
                if (dlg1 == null) return;
                dlg1.SetHeading(GUILocalizeStrings.Get(1079867));
                dlg1.SetLine(1, GUILocalizeStrings.Get(10798641));
                dlg1.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wperson = choiceSearch[dlg.SelectedLabel];
            dlg.Reset();
            choiceSearch.Clear();
            dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson); // function selection (actor, director, producer)

            //First add general option to show MP Actor Infos
            if (wperson.Length > 0)
            {
                // First check if actror exists... - this only works with MePo V1.1.5+
                ArrayList actorList = new ArrayList(); // Search with searchName parameter which contain wanted actor name, result(s) is in array which conatin id and name separated with char "|"
                //System.Collections.Generic.List<string> actorList = new System.Collections.Generic.List<string>();
                try
                {
                    MyFilmsDetail.GetActorByName(wperson, actorList);
                }
                catch (Exception)
                {}
                
                dlg.Add(GUILocalizeStrings.Get(10798731) + " (" + actorList.Count.ToString() + ")");
                //dlg.Add("Person Infos");
                choiceSearch.Add("PersonInfo");
            }

            DataRow[] wr = BaseMesFilms.LectureDonnées(MyFilms.conf.StrDfltSelect, "Producer like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798612) + "  (" + wr.Length + ")");
                choiceSearch.Add("Producer");
            }
            wr = BaseMesFilms.LectureDonnées(MyFilms.conf.StrDfltSelect, "Director like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079869) + "  (" + wr.Length + ")");
                choiceSearch.Add("Director");
            }
            wr = BaseMesFilms.LectureDonnées(MyFilms.conf.StrDfltSelect, "Actors like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079868) + "  (" + wr.Length + ")");
                choiceSearch.Add("Actors");
            }
            if (choiceSearch.Count == 0)
            {
                if (dlg1 == null) return;
                dlg1.SetHeading(GUILocalizeStrings.Get(1079867));
                dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
                dlg1.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            if (choiceSearch[dlg.SelectedLabel] == "PersonInfo")
            {
                ArrayList actorList = new ArrayList();
                // Search with searchName parameter which contain wanted actor name, result(s) is in array
                // which conatin id and name separated with char "|"
                MyFilmsDetail.GetActorByName(wperson, actorList);

                // Check result
                if (actorList.Count == 0)
                {
                    LogMyFilms.Debug("MF: (Person Info): No ActorIDs found for '" + wperson + "'");
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK); 
                    dlgOk.SetHeading("Info");
                    dlgOk.SetLine(1, string.Empty);
                    dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
                    dlgOk.DoModal(GetID);
                    return;
                }
                LogMyFilms.Debug("MF: (Person Info): " + actorList.Count + " ActorID(s) found for '" + wperson + "'");
                //int actorID;
                actorID = 0;
                string actorname = string.Empty;
                // Define splitter for string
                char[] splitter = { '|' };
                // Iterate through list
                foreach (string act in actorList)
                {
                    // Split id from actor name (two substrings, [0] is id and [1] is name)
                    string[] strActor = act.Split(splitter);
                    // From here we have all what we want, now we can populate datatable, gridview, listview....)
                    // actorID originally is integer in the databse (it can be string in results but if we want get details from
                    // IMDBActor  GetActorInfo(int idActor) we need integer)
                    actorID = Convert.ToInt32(strActor[0]);
                    actorname = strActor[1];
                    LogMyFilms.Debug("MF: (Person Info): ActorID: '" + actorID + "' with ActorName: '" + actorname + "' found found for '" + wperson + "'");
                }
                
                MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
                //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
                //if (actor != null)

                OnVideoArtistInfoGuzzi(actor);
                //OnVideoArtistInfoGuzzi(wperson);
                return;
            }
            conf.StrSelect = choiceSearch[dlg.SelectedLabel].ToString() + " like '*" + wperson + "*'";
            switch (choiceSearch[dlg.SelectedLabel])
            {
                case "Actors":
                    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079868) + " [*" + wperson + @"*]"; // "Seletion"
                    break;
                case "Director":
                    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079869) + " [*" + wperson + @"*]";
                    break;
                default:
                    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798612) + " [*" + wperson + @"*]";
                    break;
            }
            conf.StrTitleSelect = "";
            SetLabelView("search"); // show "search"
            GetFilmList();
        }

        private void OnVideoArtistInfoGuzzi(MediaPortal.Video.Database.IMDBActor actor)
        {
            MyFilmsActorInfo infoDlg =
                (MyFilmsActorInfo)GUIWindowManager.GetWindow(ID_MyFilmsActorsInfo);
            if (infoDlg == null)
            {
                return;
                }
            if (actor == null)
            {
                return;
            }
            infoDlg.Actor = actor;
            infoDlg.DoModal(GetID);
        }

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
   //                     DataRow[] wr = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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
   //                         //                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
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
   ////                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
   //                         GetFilmList();
   //                     }
   //                     break;
   //             }
   //         }
   //     }

       
        //*****************************************************************************************
        //*  search related movies by properties  (Guzzi Version)                                 *
        //*****************************************************************************************
        private void SearchRelatedMoviesbyProperties(int Index) // (int Index, IEnumerable<string> wSearchList)
        {
            // first select the property to be searching on
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            //GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList wsub_tableau = new ArrayList();
            int MinChars = 2;
            bool Filter = true;
            if (dlg == null) return;

            conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
            conf.WStrSort = conf.StrSTitle;
            conf.Boolselect = false;
            conf.Boolreturn = false;

            if (dlg == null) return; 
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            //<entry name="SearchList">TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Year|Date|Category|Country|Rating|Checked|MediaLabel|MediaType|URL|Borrower|Length|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Size|Disks|Languages|Subtitles|Number</entry>
            //<entry name="UpdateList">TranslatedTitle|OriginalTitle|Category|Year|Date|Country|Rating|Checked|MediaLabel|MediaType|Actors|Director|Producer</entry>
            //<entry name="AllItems..">TranslatedTitle|OriginalTitle|FormattedTitle|Description|Comments|Actors|Director|Producer|Rating|Country|Category|Year|Checked|MediaLabel|MediaType|Source|Date|Borrower|Length|URL|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Languages|Subtitles|DateAdded|Size|Disks|Picture|Contents_Id|Number</entry>
            //Sorted lists - manually adding items to have them in right order
            //if (conf.StrTitle1 == "OriginalTitle")
            //{
            //}
            string[] PropertyList = new string[] { "OriginalTitle", "TranslatedTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Source", "Borrower" };
            string[] PropertyListLabel = new string[] { "10798658", "10798659", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798654", "10798656" };
            // Former order was translated title first ...
            //string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Source", "Borrower" };
            //string[] PropertyListLabel = new string[] { "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798654", "10798656" };
            for (int ii = 0; ii < 31; ii++)
            {
                dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))));
                choiceSearch.Add(PropertyList[ii]);
            }

            // Dont use the propertylist...
            //foreach (string wSearch in wSearchList)
            //{
            //    dlg.Add(GUILocalizeStrings.Get(10798617) + BaseMesFilms.Translate_Column(wSearch));
            //    choiceSearch.Add(wSearch);
            //}
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            dlg.Reset();
            choiceSearch.Clear();
            LogMyFilms.Debug("MF: (RelatedPropertySearch): Searchstring in Property: '" + MyFilms.r[Index][wproperty] + "'");
            //PersonTitle Grabbing (Double Words)
            if (wproperty.ToLower() != "description" && wproperty.ToLower() != "comments" && wproperty.ToLower() != "rating")
            {
                w_tableau = Search_String(MyFilms.r[Index][wproperty].ToString());
                foreach (object t in w_tableau)
                {
                    for (int ii = 0; ii < 30; ii++)
                    {
                        if (wproperty.ToLower().Equals(PropertyList[ii].ToLower()))
                        {
                            dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": " + t);
                            //dlg.Add(wproperty + " : " + w_tableau[wi]);
                            choiceSearch.Add(t.ToString());
                            LogMyFilms.Debug("MF: (RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
                            break;
                        }
                    }
                }
            }
            //SubWordGrabbing for more Details, if necessary
            if (wproperty.ToLower() != "description" && wproperty.ToLower() != "comments")
                MinChars = 2;
                else
                MinChars = 5;
            if (MyFilms.r[Index][wproperty].ToString().Length > 0) //To avoid exception in subgrabbing
                wsub_tableau = SubWordGrabbing(MyFilms.r[Index][wproperty].ToString(),MinChars,Filter);
            if ((wproperty.ToLower() == "rating"))
            {
                dlg.Add(GUILocalizeStrings.Get(10798657) + ": = " + MyFilms.r[Index][wproperty].ToString().Replace(",","."));
                choiceSearch.Add("RatingExact");
                dlg.Add(GUILocalizeStrings.Get(10798657) + ": > " + MyFilms.r[Index][wproperty].ToString().Replace(",","."));
                choiceSearch.Add("RatingBetter");
            }
            else
            {
                LogMyFilms.Debug("MF: (RelatedPropertySearch): Length: '" + MyFilms.r[Index][wproperty].ToString().Length.ToString() + "'");
                if (MyFilms.r[Index][wproperty].ToString().Length > 0)
                {
                    foreach (object t in wsub_tableau)
                    {
                        if (w_tableau.Contains(t)) // Only Add SubWordItems if not already present in SearchStrin Table
                        {
                            LogMyFilms.Debug("MF: (RelatedPropertySearch): Searchstring Result already Present: '" + t + "'");
                            break;
                        }
                        else
                        {
                            for (int ii = 0; ii < 30; ii++)
                            {
                                if (wproperty.ToLower().Equals(PropertyList[ii].ToLower()))
                                {
                                    dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + " (" + GUILocalizeStrings.Get(10798627) + "): '" + t + "'");
                                    //dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": '" + wsub_tableau[wi] + "'");
                                    choiceSearch.Add(t.ToString());
                                    LogMyFilms.Debug("MF: (RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if ((choiceSearch.Count == 0) && (1 == 2)) // Temporarily Disabled
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//InfoPanel
                dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                dlgOk.DoModal(GetID);
                if (dlg.SelectedLabel == -1)
                    return;
                //break;
            }
            
            //if (choiceSearch.Count > 1)
            LogMyFilms.Debug("MF: (Related Search by properties - ChoiceSearch.Count: " + choiceSearch.Count);
            if (choiceSearch.Count > 0)
            {
                dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
                dlg.DoModal(GetID);
                if (dlg.SelectedLabel == -1)
                    return;
            }
            else
            dlg.SelectedLabel = 0;
            LogMyFilms.Debug("MF: (Related Search by properties - Selected wproperty: " + wproperty + "'");
            string w_rating = "0";
            
            if (choiceSearch.Count == 0) //Use Special "is NULL" handling if property is empty ...
                conf.StrSelect = wproperty + " is NULL";
            else
                w_rating = MyFilms.r[Index][wproperty].ToString().Replace(",", "."); 
                if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel] == "RatingExact"))
                    conf.StrSelect = wproperty.ToString() + " = " + w_rating;
                else
                    if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel] == "RatingBetter"))
                        conf.StrSelect = wproperty + " > " + w_rating;
                        else
                            if (wproperty == "Number")
                                conf.StrSelect = wproperty + " = " + choiceSearch[dlg.SelectedLabel];
                                    else
                                        conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
            if (choiceSearch.Count == 0) 
                conf.StrTxtSelect = "Selection " + wproperty + "(none)";
            else
                conf.StrTxtSelect = "Selection " + wproperty + " [*" + choiceSearch[dlg.SelectedLabel] + @"*]";
            conf.StrTitleSelect = string.Empty;
            SetLabelView("search"); // show "search"
            GetFilmList();
        }

        //******************************************************************************************************
        //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
        //******************************************************************************************************
        private void SearchMoviesbyRandomWithTrailer()
        {
            // first select the area where to make random search on - "all", "category", "year", "country"
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList wsub_tableau = new ArrayList();
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
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            //            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = "";
            //            keyboard.DoModal(GetID);
            //            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            switch (choiceSearch[dlg.SelectedLabel])
            {
                //case "randomall":
                //    break;

                default:
                    ArrayList w_count = new ArrayList();
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                    string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
                    DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                    w_tableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
                    w_count.Add(0);
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if (dc.ToString().Contains(wproperty))  // Only count property chosen
                            {
                                if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
                                {
                                    w_count[0] = (int)w_count[0] + 1;
                                    break;
                                }
                                else
                                {
                                    //LogMyFilms.Debug("MF: (Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                    // column Name contains propertyname : added to w_tableau + w_count
                                    if (GetSubItems)
                                    {
                                        LogMyFilms.Debug("MF: SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                                        wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                                        foreach (object t in wsub_tableau)
                                        {
                                            LogMyFilms.Debug("MF: SubItemGrabber: Output: " + t);
                                            {
                                                if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                                                {
                                                    //if (!w_index.Contains(
                                                    for (int i = 0; i < w_tableau.Count; i++)
                                                    {
                                                        if (w_tableau[i].ToString() == t.ToString())
                                                        {
                                                            w_count[i] = (int)w_count[i] + 1;
                                                            //LogMyFilms.Debug("MF: SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                    // add to w_tableau and move 1 to w_count
                                                {
                                                    LogMyFilms.Debug("MF: SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
                                                    w_tableau.Add(t.ToString());
                                                    w_count.Add(1);
                                                }
                                            }
                                        }

                                    }
                                    if (GetItems)
                                    {
                                        if (w_tableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                                        {
                                            for (int i = 0; i < w_tableau.Count; i++)
                                            {
                                                if (w_tableau[i].ToString() == wsr[dc.ColumnName].ToString())
                                                {
                                                    w_count[i] = (int)w_count[i] + 1;
                                                    //LogMyFilms.Debug("MF: (Guzzi) Class already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        // add to w_tableau and move 1 to w_count
                                        {
                                            //LogMyFilms.Debug("MF: AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
                                            w_tableau.Add(wsr[dc.ColumnName].ToString());
                                            w_count.Add(1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (w_tableau.Count == 0)
                    {
                        LogMyFilms.Debug("MF: PropertyClassCount is 0");
                        break;
                    }


                    string wproperty2="";

                    if (wproperty != "randomall")
                        {
                            dlg.Reset();
                            dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
                            choiceSearch.Clear();
                            //w_tableau = Array.Sort(w_tableau);
                            for (int i = 0; i < w_tableau.Count; i++)
                            {
                                dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], w_tableau[i]));
                                choiceSearch.Add(w_tableau[i].ToString());
                            }
                            dlg.DoModal(GetID);
                            if (dlg.SelectedLabel == -1)
                                return;
                            string t_wproperty2 = choiceSearch[dlg.SelectedLabel];
                            wproperty2 = t_wproperty2;
                        }
                    else
                        wproperty2 = "*";

                    LogMyFilms.Debug("MF: (RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
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
                    LogMyFilms.Debug("MF: (RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
                    conf.StrTxtSelect = "Selection " + wproperty + " [*" + wproperty2 + @"*]";
                    conf.StrTitleSelect = string.Empty;

                    // Temporarily Enabled for Testing
                    // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    //GetFilmList();

                    // we have: wproperty = selected category (randomall for all) and wproperty2 = value to search after

                    //if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623)))
                    //    wproperty2 = "";                                                    // Should not be done, because it is already handled in searchroutine
                    ArrayList w_index = new ArrayList();
                    int w_index_count = 0;
                    string t_number_id = string.Empty;
                    //Now build a list of valid movies in w_index with Number registered
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            //LogMyFilms.Debug("dc.ColumnName '" + dc.ColumnName.ToString() + "'");
                            if (dc.ColumnName == "Number")
                            {
                                t_number_id = wsr[dc.ColumnName].ToString();
                                //LogMyFilms.Debug("Movienumber stored as '" + t_number_id + "'");
                            }
                        }
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if ((wproperty == "randomall") && (dc.ColumnName.ToLower() == "translatedtitle"))
                                {
                                    w_index.Add(t_number_id);
                                    LogMyFilms.Debug("MF: (RamdomSearch - randomall!!!) - Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                                    w_index_count = w_index_count + 1;
                                }
                            else
                                if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                                {
                                    if ((dc.ColumnName == wproperty) && (wsr[dc.ColumnName].ToString().Length == 0)) // column Name contains propertyname : add movie number (for later selection) to w_index
                                    {
                                        w_index.Add(t_number_id);
                                        LogMyFilms.Debug("MF: (RamdomSearch - (none)!!!) Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                                        w_index_count = w_index_count + 1;
                                    }
                                }
                                else
                                    {
                                        //LogMyFilms.Debug("MF: (searchmatches) - dc '" + dc.ToString() + "' - dc.ColumnName '" + dc.ColumnName.ToString() + "' - wproperty '" + wproperty + "' and Number(ID): '" + t_number_id + "'");
                                        if (dc.ColumnName == wproperty) 
                                        {
                                            //LogMyFilms.Debug("MF: (searfhmatches with subitems) property2: '" + wproperty2 + "' - DB-Content: '" + wsr[dc.ColumnName].ToString() + "'"); 
                                            if (wsr[dc.ColumnName].ToString().Contains(wproperty2)) // column Name contains propertyname : add movie number (for later selection) to w_index
                                                {
                                                    w_index.Add(t_number_id);
                                                    LogMyFilms.Debug("MF: (RamdomSearch - Standard) Counter '" + w_index_count + "' Added as '" + w_index[w_index_count] + "'");
                                                    w_index_count = w_index_count + 1;
                                                }
                                        }
                                    }
                        }
                    }
                    // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
                    LogMyFilms.Debug("MF: (ResultBuildIndex) Found " + w_index.Count + " Records matching '" + wproperty2 + "' in '" + wproperty + "'");
                    for (int i = 0; i < w_index.Count; i++)
                        LogMyFilms.Debug("MF: (ResultList) - Index: '" + i + "' - Number: '" + w_index[i] + "'");
                    if (w_index.Count == 0)
                    {
                        ShowMessageDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0", "Keine Filme in der Auswahl vorhanden"); // menu for random search
                        return;
                    }

                    //Choose Random Movie from Resultlist
                    System.Random rnd = new System.Random();
                    Int32 RandomNumber = rnd.Next(w_index.Count + 1);
                    LogMyFilms.Debug("MF: RandomNumber: '" + RandomNumber + "'");
                    LogMyFilms.Debug("MF: RandomTitle: '" + RandomNumber + "'");

                    //Set Filmlist to random Movie:
                    conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
                    conf.WStrSort = conf.StrSTitle;
                    conf.Boolselect = false;
                    conf.Boolreturn = false;
                    
                    conf.StrSelect = "number = " + Convert.ToInt32(w_index[RandomNumber]);
                    conf.StrTxtSelect = "Selection number [" + Convert.ToInt32(w_index[RandomNumber]) + "]";
                    conf.StrTitleSelect = string.Empty;
                    //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    LogMyFilms.Debug("MF: (Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
                    SetLabelView("search"); // show "search"
                    GetFilmList(); // Added to update view ????

                    //Set Context to first and only title in facadeview
                    facadeView.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
                    if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
                        // New Window for detailed selected item information
                        {
                            conf.StrIndex = facadeView.SelectedListItem.ItemId;
                            conf.StrTIndex = facadeView.SelectedListItem.Label;
                            GUITextureManager.CleanupThumbs();
                            //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                        }
                        else
                        // View List as selected
                        {
                            conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                            Change_LayOut(MyFilms.conf.StrLayOut);
                            if (facadeView.SelectedListItem.IsFolder)
                                conf.Boolreturn = false;
                            else
                                conf.Boolreturn = true;
                            do
                            {
                                if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                                conf.StrTitleSelect += conf.Wselectedlabel;
                            } while (GetFilmList() == false); //keep calling while single folders found
                        }
                    
                    //Before showing menu, first play the trailer
                    //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                    //Change_LayOut(MesFilms.conf.StrLayOut);
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
                                MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                return;
                            case "PlayMovieTrailer":
                                //Hier muß irgendwie sichergestellt werden, daß nach Rückkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
                                MovieScrobbling = true; //Set True to avoid reload menu after Return ...    
                                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                                //Change_LayOut(MesFilms.conf.StrLayOut);
                                //conf.Boolreturn = true;
                                //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                                //conf.StrTitleSelect += conf.Wselectedlabel;
                                //while (GetFilmList() == false) ; //keep calling while single folders found

                                MyFilmsDetail.trailerPlayed = true;
                                MyFilmsDetail.Launch_Movie_Trailer(facadeView.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
                                // MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
                                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);    
                                //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                                dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
                                dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
                                dlgYesNo.DoModal(GetID);
                                if (dlgYesNo.IsConfirmed)
                                    MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, m_SearchAnimation);
                                    //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                break;
                            case "ShowMovieDetails":
                                // New Window for detailed selected item information
                                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();
                                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                                return;

                            case "ShowMovieList":
                                //GetFilmList(); //Is this necessary????
                                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                                dlg.DeInit();
                                return;

                            case "RepeatSearch":
                                RandomNumber = rnd.Next(w_index.Count + 1);
                                LogMyFilms.Debug("MF: RandomNumber: '" + RandomNumber + "'");
                                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);


                                GUIDialogYesNo dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                                dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
                                dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + w_index[RandomNumber] + "'");
                                dlg1YesNo.DoModal(GetID);
                                if (dlg1YesNo.IsConfirmed)
                                    //Launch_Movie(select_item, GetID, m_SearchAnimation);
                                    MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                                    //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                break;
                            case "NewSearch":
                                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                dlgOk.SetLine(1, string.Empty);
                                dlgOk.SetLine(2, "Not yet implemented - be patient ....");
                                SearchMoviesbyRandomWithTrailer();
                                return;

                            case "Back":
                                return;

                            default:
                                {
                                    break;
                                }
                        }
                    }   
                    break;
            }
            LogMyFilms.Debug("MF: (SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
        }

        private void SearchMoviesbyAreas()
        {
            // first select the area where to make random search on - "all", "category", "year", "country"
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList wsub_tableau = new ArrayList();
            bool GetItems = false;
            bool GetSubItems = true;
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
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            //            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = string.Empty;
            //            keyboard.DoModal(GetID);
            //            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            switch (choiceSearch[dlg.SelectedLabel])
            {
                case "randomall":
                conf.StrSelect = string.Empty;
                    conf.StrTxtSelect = "Selection [*]";
                    conf.StrTitleSelect = string.Empty;


                    // Temporarily Enabled for Testing
                    //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    SetLabelView("search"); // show "search"
                    GetFilmList();
                    break;

                default:
                    ArrayList w_count = new ArrayList();
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                    //Modified to checked for GlobalFilterString
                    string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
                    DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //DataColumn[] wc = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    w_tableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
                    w_count.Add(0);
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if (dc.ToString().Contains(wproperty))  // Only count property chosen
                            {
                                if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
                                {
                                    w_count[0] = (int)w_count[0] + 1;
                                    break;
                                }
                                else
                                {
                                    //LogMyFilms.Debug("MF: (Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                    // column Name contains propertyname : added to w_tableau + w_count
                                    if (GetSubItems)
                                    {
                                        LogMyFilms.Debug("MF: SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                                        wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                                        foreach (object t in wsub_tableau)
                                        {
                                            LogMyFilms.Debug("MF: SubItemGrabber: Output: " + t);
                                            {
                                                if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                                                {
                                                    //if (!w_index.Contains(
                                                    for (int i = 0; i < w_tableau.Count; i++)
                                                    {
                                                        if (w_tableau[i].ToString() == t.ToString())
                                                        {
                                                            w_count[i] = (int)w_count[i] + 1;
                                                            //LogMyFilms.Debug("MF: SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                    // add to w_tableau and move 1 to w_count
                                                {
                                                    LogMyFilms.Debug("MF: SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
                                                    w_tableau.Add(t.ToString());
                                                    w_count.Add(1);
                                                }
                                            }
                                        }

                                    }
                                    if (GetItems)
                                    {
                                        if (w_tableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                                        {
                                            for (int i = 0; i < w_tableau.Count; i++)
                                            {
                                                if (w_tableau[i].ToString() == wsr[dc.ColumnName].ToString())
                                                {
                                                    w_count[i] = (int)w_count[i] + 1;
                                                    //LogMyFilms.Debug("MF: (Guzzi) Clas already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        // add to w_tableau and move 1 to w_count
                                        {
                                            //LogMyFilms.Debug("MF: (Guzzi) AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
                                            w_tableau.Add(wsr[dc.ColumnName].ToString());
                                            w_count.Add(1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (w_tableau.Count == 0)
                    {
                        LogMyFilms.Debug("MF: PropertyClassCount is 0");
                        break;
                    }


                    string wproperty2 = "";

                    if (wproperty != "randomall")
                    {
                        dlg.Reset();
                        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
                        choiceSearch.Clear();
                        //w_tableau = Array.Sort(w_tableau);
                        for (int i = 0; i < w_tableau.Count; i++)
                        {
                            dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], w_tableau[i]));
                            choiceSearch.Add(w_tableau[i].ToString());
                        }
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel == -1)
                            return;
                        string t_wproperty2 = choiceSearch[dlg.SelectedLabel];
                        wproperty2 = t_wproperty2;
                    }
                    else
                        wproperty2 = "*";

                    LogMyFilms.Debug("MF: (RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
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
                                conf.StrSelect = wproperty + " is NULL";
                            else
                                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
                            break;
                    }
                    LogMyFilms.Debug("MF: (RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
                    conf.StrTxtSelect = "Selection " + wproperty + " [*" + wproperty2 + @"*]";
                    conf.StrTitleSelect = string.Empty;


                    // Temporarily Enabled for Testing
                    //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    SetLabelView("search"); // show "search"
                    GetFilmList();
                    break;
            }
            // Select RandomMovie from Filmlist and process 
            // MyFilmsDetail.Launch_Movie_Trailer(facadeView.SelectedListItem.ItemId, GetID, null);


        }

        //*****************************************************************************************
        //*  Global search movies by properties     (Guzzi Version)                               *
        //*****************************************************************************************
        private void SearchMoviesbyProperties(IEnumerable<string> wSearchList) // Old hardcoded searchlist: "TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Rating|Year|Date|Category|Country"
        {
            // first select the property to be searching on
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            //ArrayList w_tablabel = new ArrayList();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu

            dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
            choiceSearch.Add("all");

            foreach (string wSearch in wSearchList)
            {
                dlg.Add(GUILocalizeStrings.Get(10798617) + BaseMesFilms.Translate_Column(wSearch.Trim()));
                choiceSearch.Add(wSearch.Trim());
            }

            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;

            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = "";
            keyboard.DoModal(GetID);
            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            {
            ArrayList w_count = new ArrayList();
            string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
            switch (choiceSearch[dlg.SelectedLabel])
                {
                    case "all":
                        if (dlg == null) return;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                        //DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                        DataRow[] wr = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - GlobalFilterString: '" + GlobalFilterString + "' (INACTIVE for SEARCH !!!)");
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'"); 
                        foreach (DataRow wsr in wr)
                        {
                            foreach (DataColumn dc in ds.Movie.Columns)
                            {
                                if (wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()))
                                    // column contains text searched on : added to w_tableau + w_count
                                    if (w_tableau.Contains(dc.ColumnName.ToLower()))
                                    // search position in w_tableau for adding +1 to w_count
                                    {
                                        for (int i = 0; i < w_tableau.Count; i++)
                                        {
                                            if (w_tableau[i].ToString() == dc.ColumnName.ToLower())
                                            {
                                                w_count[i] = (int)w_count[i] + 1;
                                                //LogMyFilms.Debug("MF: (GlobalSearchAll) - AddCount for: '" + i.ToString() + "' - '" + dc.ColumnName.ToString() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    // add to w_tableau and move 1 to w_count
                                    {
                                        w_tableau.Add(dc.ColumnName.ToLower());
                                        w_count.Add(1);
                                        //LogMyFilms.Debug("MF: (GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                    }
                            }
                        }
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'"); 
                        if (w_tableau.Count == 0) // NodeLabelEditEventArgs Results found
                        {
                            GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                            dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//InfoPanel
                            dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                            dlgOk.DoModal(GetID);
                            if (dlg.SelectedLabel == -1)
                                return;
                            break;
                        }
                        dlg.Reset();
                        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), keyboard.Text)); // menu & SearchString
                        choiceSearch.Clear();
                        string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Borrower" };
                        string[] PropertyListLabel = new string[] { "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798656" };
                        for (int ii = 0; ii < 30; ii++)
                        {
                            //LogMyFilms.Debug("MF: (GlobalSearchAll) - OutputSort: Property is '" + PropertyList[ii] + "' - '" + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + "' (" + PropertyListLabel[ii] + ")");
                            for (int i = 0; i < w_tableau.Count; i++)
                            {
                                //LogMyFilms.Debug("MF: (GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
                                if (w_tableau[i].ToString().ToLower().Equals(PropertyList[ii].ToLower()))
                                {
                                    dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), w_count[i], GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii])))));
                                    choiceSearch.Add(w_tableau[i].ToString());
                                }
                            }
                        }
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel == -1)
                            return;
                        wproperty = choiceSearch[dlg.SelectedLabel];
                        if (control_searchText(keyboard.Text))
                        {

                            //LogMyFilms.Debug("MF: (GlobalSearchAll) - ChosenProperty: wproperty is '" + wproperty + "'"); 
                            switch (wproperty)
                            {
                                case "rating":
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                case "number":
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                default:
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                                    break;
                            }
                            conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]";
                            conf.StrTitleSelect = string.Empty;
                            // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            SetLabelView("search"); // show "search"
                            GetFilmList();
                        }
                        break;

                    case "all-Zebons":
                        //ArrayList w_count = new ArrayList();
                        if (dlg == null) return;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                        DataRow[] wrz = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                        foreach (DataRow wsr in wrz)
                        {
                            foreach (string wsearch in wSearchList)
                            {
                                if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[wsearch].ToString().ToLower())).Contains(keyboard.Text.ToLower()))
                                    // column contains text serached on : added to w_tableau + w_count
                                    if (w_tableau.Contains(wsearch))
                                    // search position in w_tableau for adding +1 to w_count
                                    {
                                        for (int i = 0; i < w_tableau.Count; i++)
                                        {
                                            if (w_tableau[i].ToString() == wsearch)
                                            {
                                                w_count[i] = (int)w_count[i] + 1;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    // add to w_tableau and move 1 to w_count
                                    {
                                        w_tableau.Add(wsearch);
                                        w_count.Add(1);
                                    }
                            }
                        }
                        if (w_tableau.Count == 0)
                        {
                            if (dlg1 == null) return;
                            dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
                            dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
                            dlg1.SetLine(2, keyboard.Text);
                            dlg1.DoModal(GUIWindowManager.ActiveWindow);
                            return;
                        }
                        dlg.Reset();
                        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), keyboard.Text)); // menu
                        choiceSearch.Clear();
                        for (int i = 0; i < w_tableau.Count; i++)
                        {
                            dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), w_count[i], BaseMesFilms.Translate_Column(w_tableau[i].ToString())));
                            choiceSearch.Add(w_tableau[i].ToString());
                        }
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel == -1)
                            return;
                        wproperty = choiceSearch[dlg.SelectedLabel];
                        if (control_searchText(keyboard.Text))
                        {
                            if (ds.Movie.Columns[wproperty].DataType.Name == "string")
                            {
                                conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
                                conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel] + @"*]";
                            }
                            else
                            {
                                conf.StrSelect = wproperty + " = '" + keyboard.Text + "'";
                                conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [" + keyboard.Text + @"]";
                            }
                            conf.StrTitleSelect = string.Empty;
                            // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            SetLabelView("search"); // show "search"
                            GetFilmList();
                        }
                        break;
                    
                    default:
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - ChosenProperty: wproperty is '" + wproperty + "'");
                        LogMyFilms.Debug("MF: (GlobalSearchAll) - ChosenProperty: SearchTest is '" + keyboard.Text + "'"); 
                        if (control_searchText(keyboard.Text))
                        {
                            DataRow[] wdr = BaseMesFilms.LectureDonnées(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - GlobalFilterString: '" + GlobalFilterString + "' (INACTIVE for SEARCH !!!)");
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'");
                            foreach (DataRow wsr in wdr)
                            {
                                foreach (DataColumn dc in ds.Movie.Columns)
                                {
                                    if (dc.ColumnName.ToLower() == wproperty.ToLower())
                                    {
                                        if (wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()))
                                            // column contains text searched on : added to w_tableau + w_count
                                            if (w_tableau.Contains(dc.ColumnName.ToLower()))
                                            // search position in w_tableau for adding +1 to w_count
                                            {
                                                for (int i = 0; i < w_tableau.Count; i++)
                                                {
                                                    if (w_tableau[i].ToString() == dc.ColumnName.ToLower())
                                                    {
                                                        w_count[i] = (int)w_count[i] + 1;
                                                        //LogMyFilms.Debug("MF: (GlobalSearchAll) - AddCount for: '" + i.ToString() + "' - '" + dc.ColumnName.ToString() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            // add to w_tableau and move 1 to w_count
                                            {
                                                w_tableau.Add(dc.ColumnName.ToLower());
                                                w_count.Add(1);
                                                //LogMyFilms.Debug("MF: (GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                            }
                                    }
                                }
                            }
                            LogMyFilms.Debug("MF: (GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
                            if (w_tableau.Count == 0) // NodeLabelEditEventArgs Results found
                            {
                                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//InfoPanel
                                dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                                dlgOk.DoModal(GetID);
                                if (dlg.SelectedLabel == -1)
                                    return;
                                break;
                            }

                            switch (wproperty)
                            {
                                case "Rating":
                                    //conf.StrSelect = wproperty + " = " + keyboard.Text; // Zebons version
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                case "Number":
                                    //conf.StrSelect = wproperty + " = " + keyboard.Text; // Zebons Version
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                default:
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                                    break;
                            }

                            conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]"; // Zebons Version
                            //conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]"; // Guzzi Version
                            conf.StrTitleSelect = "";
                            // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            SetLabelView("search"); // show "search"
                            GetFilmList();
                        }
                        break;
                }
            }
        }

        //*****************************************************************************************
        //*  No Movie found to display. Display all movies
        //*****************************************************************************************
        private static void DisplayAllMovies()
        {
            MyFilms.conf.StrFilmSelect = MyFilms.conf.StrTitle1 + " not like ''";
            conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
            conf.WStrSort = conf.StrSTitle;
            conf.Boolselect = false;
            conf.Boolreturn = false;
            r = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, true);
        }

        //*****************************************************************************************
        //*  Initialize Fields on Main screen                                                     *
        //*****************************************************************************************
        private void InitMainScreen()
        {
            LogMyFilms.Debug("MF: (InitMainScreen) - Initialize all properties !!!");

            MovieScrobbling = false; //Reset MovieScrobbling
            MyFilmsDetail.Init_Detailed_DB();  // Includes clear of db & user properties

            //if (MesFilms.conf.StrFanart)
            //    backdrop.Active = true;
            //else
            //    backdrop.Active = false;

            backdrop.Filename = String.Empty;
            cover.Filename = String.Empty;

            // (re)link the backdrop image controls to the backdrop image swapper
            backdrop.GUIImageOne = ImgFanart;
            backdrop.GUIImageTwo = ImgFanart2;
            backdrop.LoadingImage = loadingImage;

            BtnGlobalOverlayFilter.Label = GUILocalizeStrings.Get(10798714); // Global Filters ...

            //ImgFanart.SetVisibleCondition(1, false); //Added by ZebonsMerge ->> This fucked up the fanart swapper !!!!!
            //ImgFanart2.SetFileName(string.Empty); //Added by ZebonsMerge

            MyFilmsDetail.clearGUIProperty("logos_id2001");
            MyFilmsDetail.clearGUIProperty("logos_id2002");
            MyFilmsDetail.clearGUIProperty("logos_id2003");
            MyFilmsDetail.clearGUIProperty("logos_id2012"); // Combined Logo
            MyFilmsDetail.clearGUIProperty("nbobjects.value");
            MyFilmsDetail.clearGUIProperty("Fanart");
            MyFilmsDetail.clearGUIProperty("Fanart2");
            MyFilmsDetail.clearGUIProperty("db.rating");
            MyFilmsDetail.clearGUIProperty("view"); // Try to properly clean main view when entering
            MyFilmsDetail.clearGUIProperty("select");

            GlobalFilterStringUnwatched = ""; // Will be later initialized from setting MyFilms.conf.GlobalUnwatchedOnly
            MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
            if (!GlobalFilterTrailersOnly)
            {
              GlobalFilterStringTrailersOnly = "";
              MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly");
            }
          
            if (!GlobalFilterMinRating)
            {
              GlobalFilterStringMinRating = "";
              MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
              MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
            }
            // this.Load_Rating(0); // old method - nor more used
            GUIWaitCursor.Hide();
            GUIControl.ShowControl(GetID, 34); // hide elements in skin
            LogMyFilms.Debug("MF: (InitMainScreen) - Initialize all properties - Finished !");
        }

        //*****************************************************************************************
        //*  Ask for Title search and grab information on the NET base on the grab configuration  *
        //*****************************************************************************************
        private void GetTitleGrab()
        {
        }

        //*****************************************************************************************
        //*  Update Database in batch mode                                                        *
        //*****************************************************************************************
        public void AsynUpdateDatabase()
        {
            if (!bgUpdateDB.IsBusy)
            {
                // moved here to avoid reinstantiating for each menu change.... thanks inker !
                bgUpdateDB.DoWork += new DoWorkEventHandler(bgUpdateDB_DoWork);
                bgUpdateDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateDB_RunWorkerCompleted);
                bgUpdateDB.RunWorkerAsync(MyFilms.conf.StrTIndex);
                LogMyFilms.Info("MF: Launching AMCUpdater in batch mode");

            }
        }

        static void bgUpdateDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            MyFilmsDetail.RunAMCupdater(Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe", "\"" + MyFilms.conf.StrAMCUpd_cnf + "\" \"" + MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Log) + "\""); // Add Logpath to commandlineparameters
        }

        void bgUpdateDB_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            LogMyFilms.Info("MF: : Update database with AMCUpdater finished. (GetID = '" + GetID + "')");
            if (GetID == 7986)
            {
                Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                Load_Config(Configuration.CurrentConfig, true);
                Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
            }
        }

        //*****************************************************************************************
        //*  Download Backdrop Fanart in Batch mode                                               *
        //*****************************************************************************************
        public void AsynUpdateFanart()
        {
          if (!bgUpdateFanart.IsBusy)
          {
            // moved here to avoid reinstantiating for each menu change.... thanks inker !
            bgUpdateFanart.DoWork += new DoWorkEventHandler(bgUpdateFanart_DoWork);
            bgUpdateFanart.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateFanart_RunWorkerCompleted);
            bgUpdateFanart.RunWorkerAsync(MyFilms.r);
            LogMyFilms.Info("MF: : Downloading backdrop fanart in batch mode");

          }
        }

        static void bgUpdateFanart_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string wtitle = string.Empty;
            foreach (DataRow t in MyFilms.r)
            {
                if (t["OriginalTitle"] != null && t["OriginalTitle"].ToString().Length > 0)
                    wtitle = t["OriginalTitle"].ToString();
                if (wtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                    wtitle = wtitle.Substring(wtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                string wttitle = string.Empty;
                if (t["TranslatedTitle"] != null && t["TranslatedTitle"].ToString().Length > 0)
                    wttitle = t["TranslatedTitle"].ToString();
                if (wttitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                    wttitle = wttitle.Substring(wttitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                if (t["OriginalTitle"].ToString().Length > 0)

                {
                    int wyear = 0;
                    try { wyear = System.Convert.ToInt16(t["Year"]); }
                    catch { }
                    string wdirector = string.Empty;
                    try { wdirector = (string)t["Director"]; }
                    catch { }
                    System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1.ToString());
                    //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, MesFilms.conf.StrPathFanart, true, false);
                }
            }
        }

        static void bgUpdateFanart_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            LogMyFilms.Info("MF: Backdrop Fanart download finished");
        }

        //*****************************************************************************************
        //*  Load List movie file in batch mode                                                   *
        //*****************************************************************************************
        public void AsynLoadMovieList()
        {
            if (!bgLoadMovieList.IsBusy)
            {
                LogMyFilms.Debug("MF: AsynLoadMovieList() started in background !");
                bgLoadMovieList.DoWork += new DoWorkEventHandler(bgLoadMovieList_DoWork);
                bgLoadMovieList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgLoadMovieList_RunWorkerCompleted);
                LogMyFilms.Info("MF: Loading Movie List in batch mode");
                bgLoadMovieList.RunWorkerAsync();
            }
            else
              LogMyFilms.Debug(("MF: AsynLoadMovieList() could not be launched because already running !"));
        }

        static void bgLoadMovieList_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string searchrep = MyFilms.conf.StrDirStor;
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if ((d.DriveType.ToString() == "CDRom") && d.IsReady)
                {
                    if (searchrep.Length > 0)
                        searchrep = searchrep + ";" + d.Name;
                    else
                        searchrep = d.Name;
                }
            }
            System.Text.RegularExpressions.Regex oRegex= new System.Text.RegularExpressions.Regex(";");
            string[] SearchDir = oRegex.Split(searchrep);
            foreach (string path in SearchDir)
            {
                MyFilms.conf.MovieList.Add(System.IO.Directory.GetFiles(path));
                if ((MyFilms.conf.SearchSubDirs == "no") || (!System.IO.Directory.Exists(path))) continue;
                foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                {
                    MyFilms.conf.MovieList.Add(System.IO.Directory.GetFiles(sFolderSub));
                }
            }
        }

        static void bgLoadMovieList_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            LogMyFilms.Info("MF: Loading Movie List in batch mode finished");
        }

        //*****************************************************************************************
        //*  Search and register Trailers in Batch mode                                               *
        //*****************************************************************************************
        public void AsynUpdateTrailer()
        {
          if (!bgUpdateTrailer.IsBusy)
          {
            // moved here to avoid reinstantiating for each menu change.... thanks inker !
            bgUpdateTrailer.DoWork += new DoWorkEventHandler(bgUpdateTrailer_DoWork);
            bgUpdateTrailer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateTrailer_RunWorkerCompleted);
            bgUpdateTrailer.RunWorkerAsync(MyFilms.r);
            LogMyFilms.Info("MF: starting 'Search and register Trailer' in batch mode");
          }
        }

        static void bgUpdateTrailer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
          // ToDo: Check fanart worker thread to implement same way !!!
        }

        static void bgUpdateTrailer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
          LogMyFilms.Info("MF: 'Search and register Trailer' Thread finished");
        }

        private void FanartTimerEvent(object state)
        {
            //LogMyFilms.Debug("MF: (FanartTimerEvent): FanartTimerEvent triggered !!!");
            //LogMyFilms.Debug("MF: (FanartTimerEvent): Current Setting of Backdrop: '" + backdrop.Filename.ToString() + "'");
            // ToDo: Load new Fanart here !!!
            //if ((CurrentMovie.Length > 0) && (backdrop.Filename.Length > 0) && (backdrop.Active == true))
            if (backdrop.Filename != null)
                {
                LogMyFilms.Debug("MF: (FanartTimerEvent): loadFanart triggered for '" + facadeView.SelectedListItem.Label + "' !");
                //LogMyFilms.Debug("MF: (FanartTimerEvent): loadFanart triggered for '" + CurrentMovie.ToString() + "' !");
                //LogMyFilms.Debug("MF: (FanartTimerEvent): loadFanart CurrentFanartDir '" + CurrentFanartDir.ToString() + "' !");
                //Disabled, because it's still not working ...
                loadFanart();
            }
            else
            {
                LogMyFilms.Debug("MF: (FanartTimerEvent): loadFanart NOT triggered !");
            }
            if (m_bFanartTimerDisabled) 
              m_bFanartTimerDisabled = false;
        }

        bool fanartSet = false;
        //Fanart currSeriesFanart = null;

        private bool loadFanart()
        //private bool loadFanart(DBTable item)
        {
          if (backdrop == null)
          {
            // Fanart not supported by skin, exit now
            fanartSet = false;
            return false;
          }
          if (fanartSet)
          {
            // Can be removed?
          }

            string fanart = string.Empty;
            string fanartdir = string.Empty;
            string wtitle2 = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();

            // Get FanartDirectory
            if (MyFilms.conf.StrTitle1 != "OriginalTitle")
            {
                if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                    wtitle2 = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            }

            if (wtitle2.Contains(MyFilms.conf.TitleDelim))
                wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            LogMyFilms.Debug("MF: (FindFanart) - wtitle name = '" + wtitle2 + "'");
            //wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');

            fanartdir = MyFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}";
            LogMyFilms.Debug("MF: (FindFanart) - fanartdir = '" + fanartdir + "'");
            
            
            
            try
            {
                //LogMyFilms.Debug("MF: (loadFanart): Load Fanart by Timer for activemovie: '" + "'");
                //Fanart fanart = currSeriesFanart;
                //DBSeries series = item as DBSeries;

                // Get a Fanart for selected movie
                
                
                //FindFanart(); // first modify the method before calling it here ....
                fanart = "test";

                if (fanart == null)
                {
                    // This shouldn't happen
                    LogMyFilms.Debug("MF: (loadFanart): Fanart is unavailable, disabling");
                    DisableFanart();
                    return false;
                }

                // Activate Backdrop in Image Swapper                
                if (!backdrop.Active) backdrop.Active = true;

                // Assign Fanart filename to Image Loader
                // Will display fanart in backdrop or reset to default background                
                // backdrop.Filename = "test";
                LogMyFilms.Debug("MF: (loadFanart): Loaded fanart is: '" + backdrop.Filename + "'");
                return fanartSet = true;
            }
            catch (Exception ex)
            {
                LogMyFilms.Debug("MF: (loadFanart): Failed to load Fanart: " + ex.Message);
                return fanartSet = false;
            }
        }
        
        private bool FindFanart()
        {
            LogMyFilms.Debug("MF: (FindFanart): Started FanartSearch");
            if (MyFilms.conf.StrFanart)
            {};
            string[] wfanart = new string[2];
            wfanart[0] = " ";
            wfanart[1] = " ";
            //Search Fanarts
            string wtitle2 = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
            LogMyFilms.Debug("MF: (FindFanart) - wtitle old = '" + wtitle2 + "'");
            //Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
            if (MyFilms.conf.StrTitle1 != "OriginalTitle")
                {
                    if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        wtitle2 = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                }

            if (wtitle2.Contains(MyFilms.conf.TitleDelim))
                wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            LogMyFilms.Debug("MF: (FindFanart) - wtitle name = '" + wtitle2 + "'");
            wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');
            LogMyFilms.Debug("MF: (FindFanart) - wtitle filename = '" + wtitle2 + "'");
            LogMyFilms.Debug("MF: (FindFanart) - MesFilms.conf.StrFanart = '" + MyFilms.conf.StrFanart + "'");

            string safeName = string.Empty;
            safeName = MyFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}";
            //LogMyFilms.Debug("MF: (FindFanart) - Directory (safename) = '" + safeName.ToString() + "'");
            FileInfo wfile = new FileInfo(safeName + "\\{" + wtitle2 + "}.jpg");
            //LogMyFilms.Debug("MF: (FindFanart) - FullPath (safename file&ext) = '" + wfile.ToString() + "'");

            // Single File
            //wfanart[0] = safeName + "\\{" + wtitle2 + "}.jpg";
            //wfanart[1] = "file";

            if (System.IO.Directory.Exists(safeName))
            {
                //file
                if (System.IO.Directory.GetFiles(safeName).Length > 0)
                    {
                        wfanart[0] = System.IO.Directory.GetFiles(safeName)[0];
                        wfanart[1] = "file";
                    }
                //dir    
                if (System.IO.Directory.GetFiles(safeName).Length > 0)
                    {
                        wfanart[0] = safeName;
                        wfanart[1] = "dir";
                    }
            }
            else
            {
                try { System.IO.Directory.CreateDirectory(safeName); }
                catch { }
            }

            ArrayList result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            Int64 wsize = 0; // Temporary Filesize detection
            //Search Files in Mediadirectory (used befor: SearchFiles("trailer", directoryname, true, true);)
            string[] files = Directory.GetFiles(MyFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}", "*.*", SearchOption.AllDirectories);
            foreach (string filefound in files)
            {
                LogMyFilms.Debug("MF: (FanartSearchLocal) - Processing FilesFound: '" + filefound + "'");
                if (Utils.IsPicture(filefound))
                {
                    wsize = new System.IO.FileInfo(filefound).Length;
                    result.Add(filefound);
                    resultsize.Add(wsize);
                    filesfound[filesfoundcounter] = filefound;
                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                    filesfoundcounter = filesfoundcounter + 1;
                    LogMyFilms.Debug("MF: (FanartSearchLocal) - FilesFound in FanartDirectory: Size-Name '" + wsize + "' - Name '" + filefound + "'");
                }
            }

            LogMyFilms.Debug("MF: (FindFanart): Results for wfanart[1,2]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            if (wfanart[0] == " ")
            {
                //No Fanart available ...
                return false;
            }
            else
            {
                //Choose random fanart and return it ....
                return false;
            }
        }

        private void DisableFanart()
        {
            // Disable Random Fanart Timer
            m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            // Disable Fanart                
            if (backdrop.Active) backdrop.Active = false;
            backdrop.Filename = String.Empty;
            LogMyFilms.Debug("MF: (DisableFanart): Fanart disabled !");
        }

        //-------------------------------------------------------------------------------------------
        //  Search All Fanart for a given movie
        //-------------------------------------------------------------------------------------------        
        public static void SearchFanart(DataRow[] r1, int Index, bool ExtendedSearch)
        {
            //Searchdirectory:
            LogMyFilms.Debug("MF: (SearchtrailerLocal) - StrDirStortrailer: " + MyFilms.conf.StrDirStorTrailer);
            //Title1 = Movietitle
            LogMyFilms.Debug("MF: (SearchTrailerLocal) - MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1] : '" + MyFilms.r[Index][MyFilms.conf.StrTitle1] + "'");
            //Title2 = Translated Movietitle
            LogMyFilms.Debug("MF: (SearchTrailerLocal) - MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle2] : '" + MyFilms.r[Index][MyFilms.conf.StrTitle2] + "'");
            //Cleaned Title
            LogMyFilms.Debug("MF: (SearchTrailerLocal) - Cleaned Title                                               : '" + MediaPortal.Util.Utils.FilterFileName(MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString().ToLower()) + "'");
            //Index of facadeview?
            LogMyFilms.Debug("MF: (SearchtrailerLocal) - Index: '" + Index + "'");
            //Full Path to Film
            LogMyFilms.Debug("MF: (SearchtrailerLocal) - FullMediasource: '" + (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim() + "'");

            ArrayList result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            string file = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            string titlename = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            string titlename2 = MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString();
            string directoryname = string.Empty;
            string movieName = string.Empty;
            Int64 wsize = 0; // Temporary Filesize detection
            // split searchpath information delimited by semicolumn (multiple searchpathes from config)
            string[] Trailerdirectories = MyFilms.conf.StrDirStorTrailer.Split(new Char[] { ';' });
            LogMyFilms.Debug("MF: (SearchtrailerLocal) Search for '" + file + "' in '" + MyFilms.conf.StrDirStorTrailer + "'");

            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            LogMyFilms.Debug("MF: (SearchtrailerLocal) splits media directory name: '" + movieName + "'");
            try
            { directoryname = System.IO.Path.GetDirectoryName(movieName); }
            catch
            { directoryname = string.Empty; }
            LogMyFilms.Debug("MF: (SearchtrailerLocal) get media directory name: '" + directoryname + "'");

            //Search Files in Mediadirectory (used befor: SearchFiles("trailer", directoryname, true, true);)
            if (!string.IsNullOrEmpty(directoryname))
            {
              string[] files = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories);
              foreach (string filefound in files)
              {
                if (((filefound.ToLower().Contains("trailer")) || (filefound.ToLower().Contains("trl"))) && (Utils.IsVideo(filefound)))
                {
                  wsize = new System.IO.FileInfo(filefound).Length;
                  result.Add(filefound);
                  resultsize.Add(wsize);
                  filesfound[filesfoundcounter] = filefound;
                  filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                  filesfoundcounter = filesfoundcounter + 1;
                  LogMyFilms.Debug("MF: (TrailersearchLocal) - FilesFound in MediaDir: Size '" + wsize + "' - Name '" + filefound + "'");
                }
              }
            }

            //Search Filenames with "title" in Trailer Searchpath
            if (ExtendedSearch)
            {
              string[] files;
              string[] directories;

              foreach (string storage in Trailerdirectories)
                {
                    LogMyFilms.Debug("MF: (TrailersearchLocal) - TrailerSearchDirectoriy: '" + storage + "'");

                    // search in root directory
                    files = Directory.GetFiles(storage, "*.*", SearchOption.TopDirectoryOnly);
                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Search for matching files in root directory: '" + storage + "'");
                    foreach (string filefound in files)
                    {
                      if (((filefound.ToLower().Contains(titlename.ToLower())) || (filefound.ToLower().Contains(titlename2.ToLower()))) && (Utils.IsVideo(filefound)))
                      {
                        wsize = new System.IO.FileInfo(filefound).Length;
                        result.Add(filefound);
                        resultsize.Add(wsize);
                        filesfound[filesfoundcounter] = filefound;
                        filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                        filesfoundcounter = filesfoundcounter + 1;
                        LogMyFilms.Debug("MF: (TrailersearchLocal) - Singlefiles found in Trailer root directory: Size '" + wsize + "' - Name '" + filefound + "'");
                      }
                    }

                    // search in subdirectories:
                    directories = Directory.GetDirectories(storage, "*.*", SearchOption.AllDirectories);
                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Search for matching (sub)directories ...");
                    foreach (string directoryfound in directories)
                    {
                        LogMyFilms.Debug("MF: (TrailersearchLocal) - directory to check: '" + directoryfound + "'");
                        if ((directoryfound.ToLower().Contains(titlename.ToLower())) || (directoryfound.ToLower().Contains(titlename2.ToLower())))
                        {
                            LogMyFilms.Debug("MF: (TrailersearchLocal) - Directory found: '" + directoryfound + "'");
                            files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                            foreach (string filefound in files)
                            {
                                if (Utils.IsVideo(filefound))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Files added matching Directory: Size '" + wsize + "' - Name '" + filefound + "'");
                                }
                            }

                        }
                        else
                        {
                            files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                            foreach (string filefound in files)
                            {
                                if (((filefound.ToLower().Contains(titlename.ToLower())) || (filefound.ToLower().Contains(titlename2.ToLower()))) && (Utils.IsVideo(filefound)))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Singlefiles found in TrailerDIR: Size '" + wsize + "' - Name '" + filefound + "'");
                                }
                            }
                        }
                    }
                }
            }

            var sort = from fn in filesfound
                       orderby new FileInfo(fn).Length descending
                       select fn;
            foreach (string n in filesfound)
                LogMyFilms.Debug("MF: (Sorted Trailerfiles) ******* : '" + n + "'");

            Array.Sort(filesfoundsize);
            for (int i = 0; i < result.Count; i++)
            {
                LogMyFilms.Debug("MF: (Sorted Trailerfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
            }

            string trailersourcepath = "";

            if (result.Count != 0)
            {
                //result.Sort();
                trailersourcepath = result[0].ToString();
                //ArrayList wresult = new ArrayList();
                //foreach (String s in result)
                if (result.Count > 1)
                {
                    for (int i = 1; i < result.Count; i++)
                    {
                        trailersourcepath = trailersourcepath + ";" + result[i];
                        LogMyFilms.Debug("MF: (SearchTrailerLocal) - Added Trailer to Trailersouce: '" + result[i] + "'");
                    }
                }
                LogMyFilms.Debug("MF: (SearchTrailerLocal) - Total Files found: " + result.Count);
                LogMyFilms.Debug("MF: (SearchTrailerLocal) - TrailerSourcePath: '" + trailersourcepath + "'");
            }
            else
                LogMyFilms.Debug("MF: (SearchTrailerLocal) - NO TRAILERS FOUND !!!!");

            if ((trailersourcepath.Length > 0) && (MyFilms.conf.StrStorageTrailer.Length != 0 && MyFilms.conf.StrStorageTrailer != "(none)"))
            {
                LogMyFilms.Debug("MF: (SearchTrailerLocal) - Old Trailersourcepath: '" + MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] + "'");
                MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] = trailersourcepath;
                LogMyFilms.Debug("MF: (SearchTrailerLocal) - New Trailersourcepath    : '" + MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] + "'");
                //Update_XML_database();
                LogMyFilms.Debug("MF: (SearchTrailerLocal) - Database Updated !!!!");
            }
        }

       private void ShowMessageDialog(string headline, string line1, string line2)
       {
          GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
          if (dlgOK != null)
          {
             dlgOK.SetHeading(headline);
             dlgOK.SetLine(1, line1);
             dlgOK.SetLine(2, line2);
             dlgOK.DoModal(GetID);
          }
       }

       //*****************************************************************************************
       //*  search and download artist imdb pictures from mediaindex                             *
       //*****************************************************************************************
       private void ArtistIMDBpictures(string wperson)
       {
           if (wperson.Length > 0)
           {
               // First check if actror exists...
               ArrayList actorList = new ArrayList();
               // Search with searchName parameter which contain wanted actor name, result(s) is in array which conatin id and name separated with char "|"
               MyFilmsDetail.GetActorByName(wperson, actorList);
               if (actorList.Count != 0)
               {
                   actorID = 0;
                   string actorname = string.Empty;
                   char[] splitter = { '|' };
                   foreach (string act in actorList)
                   {
                       string[] strActor = act.Split(splitter);
                       // Split id from actor name (two substrings, [0] is id and [1] is name)
                       actorID = Convert.ToInt32(strActor[0]); // IMDBActor  GetActorInfo(int idActor) we need integer)
                       actorname = strActor[1];
                   }

                   MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
               }

            }
       }

       //*****************************************************************************************
       //*  set the #myfilms.view label
       //*****************************************************************************************
       private static void SetLabelView(string viewLabel)
       {
         string newViewLabel = viewLabel; // use the parameter as default ...
         viewLabel = viewLabel.ToLower();
         if (viewLabel == GUILocalizeStrings.Get(342).ToLower() || string.IsNullOrEmpty(viewLabel)) // case "films" or empty should show "all" = "filmes"
           viewLabel = "all";
         switch (viewLabel)
         {
           case "search":
             newViewLabel = GUILocalizeStrings.Get(137);// "search"
             break;
           case "all":
             newViewLabel = GUILocalizeStrings.Get(342); //videos
             break;
           case "year":
             newViewLabel = GUILocalizeStrings.Get(345); //year
             break;
           case "category":
             newViewLabel = GUILocalizeStrings.Get(10798664); //category
             break;
           case "country":
             newViewLabel = GUILocalizeStrings.Get(200026); //country
             break;
           case "storage":
             conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
             newViewLabel = GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951); //storage
             break;
           case "actors":
             newViewLabel = GUILocalizeStrings.Get(10798667); //actors
             break;

           case "view0":
           case "view1":
           case "view2":
           case "view3":
           case "view4":
             // specific user View
             int i = -1;
             if (viewLabel == "view0")
               i = 0;
             if (viewLabel == "view1")
               i = 1;
             if (viewLabel == "view2")
               i = 2;
             if (viewLabel == "view3")
               i = 3;
             if (viewLabel == "view4")
               i = 4;

             //if (viewDefaultItem == conf.StrViewText[0].ToLower() || viewDefaultItem == conf.StrViewItem[0].ToLower())
             //  i = 0;
             //if (viewDefaultItem == conf.StrViewText[1].ToLower() || viewDefaultItem == conf.StrViewItem[1].ToLower())
             //  i = 1;
             //if (viewDefaultItem == conf.StrViewText[2].ToLower() || viewDefaultItem == conf.StrViewItem[2].ToLower())
             //  i = 2;
             //if (viewDefaultItem == conf.StrViewText[3].ToLower() || viewDefaultItem == conf.StrViewItem[3].ToLower())
             //  i = 3;
             //if (viewDefaultItem == conf.StrViewText[4].ToLower() || viewDefaultItem == conf.StrViewItem[4].ToLower())
             //  i = 4;

             if (i != -1)
             {
               if (string.IsNullOrEmpty(conf.StrViewText[i]))
                 newViewLabel = conf.StrViewItem[i];   // specific user View1
               else
                 newViewLabel = conf.StrViewText[i];   // specific Text for View1
             }
             break;
           
           default:
             break;
         }
         MyFilmsDetail.setGUIProperty("view", newViewLabel);
         MyFilms.conf.StrTxtView = newViewLabel;
         GUIPropertyManager.SetProperty("#currentmodule", newViewLabel);
         LogMyFilms.Debug("MF: SetLabelView has been called with '" + viewLabel + "' -> set view to '" + newViewLabel + "'");
       }

       //*****************************************************************************************
       //*  set the #myfilms.view label
       //*****************************************************************************************
       private void SetLabelSelect(string selectLabel)
       {
         string newSelectLabel = selectLabel;
         selectLabel = selectLabel.ToLower();
         switch (selectLabel)
         {
           case "root":

             if ((GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating).Length > 0)
             {
               //conf.StrTxtSelect = GUILocalizeStrings.Get(10798622) + " " + GUILocalizeStrings.Get(10798632); // 10798622 all films // 10798632 (global filter active) 
               conf.StrTxtSelect = GUILocalizeStrings.Get(10798632); // 10798622 all films // 10798632 (global filter active) / Filtered
             }
             else
             {
               conf.StrTxtSelect = GUILocalizeStrings.Get(10798622); //10798622 All 
             }
             MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
             break;

           default:
             conf.StrTxtSelect = newSelectLabel;
             MyFilmsDetail.setGUIProperty("select", newSelectLabel);
             break;
         }
       }

       /// <summary>
       /// Setup logger. This funtion made by the team behind Moving Pictures 
       /// (http://code.google.com/p/moving-pictures/)
       /// </summary>
       private static void InitLogger()
       {
           LoggingConfiguration config = new LoggingConfiguration();

           try
           {
               FileInfo logFile = new FileInfo(Config.GetFile(Config.Dir.Log, LogFileName));
               if (logFile.Exists)
               {
                   if (File.Exists(Config.GetFile(Config.Dir.Log, OldLogFileName)))
                       File.Delete(Config.GetFile(Config.Dir.Log, OldLogFileName));

                   logFile.CopyTo(Config.GetFile(Config.Dir.Log, OldLogFileName));
                   logFile.Delete();
               }
           }
           catch (Exception) { }

           FileTarget fileTarget = new FileTarget();
           fileTarget.FileName = Config.GetFile(Config.Dir.Log, LogFileName);
           fileTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss,f} " +
                               "${level:fixedLength=true:padding=5} " +
                               "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " +
                               "${exception:format=tostring}";

           config.AddTarget("file", fileTarget);

           // Get current Log Level from MediaPortal 
           NLog.LogLevel logLevel;
           MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

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

           LoggingRule rule = new LoggingRule("*", logLevel, fileTarget);
           config.LoggingRules.Add(rule);

           LogManager.Configuration = config;
       }

       private static void InitGUIPropertyLabels()
       {
         using (
           MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
         {
           MyFilmsDetail.setGUIProperty("config.pluginname", xmlreader.GetValueAsString("MyFilms", "PluginName", "Films"));
           MyFilmsDetail.setGUIProperty("config.pluginmode", xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal"));
           LogMyFilms.Info("Startmode: '" + xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal") + "'");
         }
         AntMovieCatalog ds = new AntMovieCatalog();
         foreach (DataColumn dc in ds.Movie.Columns)
         {
           MyFilmsDetail.setGUIProperty("db." + dc.ColumnName.ToLower() + ".label", BaseMesFilms.Translate_Column(dc.ColumnName));
         }

         GUIPropertyManager.SetProperty("#btWeb.startup.link", "");
         GUIPropertyManager.SetProperty("#btWeb.link.zoom", "");
         GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
         GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
         GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
         GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
         
         MyFilmsDetail.setGUIProperty("db.calc.aspectratio.label", GUILocalizeStrings.Get(10798697));
         MyFilmsDetail.setGUIProperty("db.calc.imageformat.label", GUILocalizeStrings.Get(10798698));
         MyFilmsDetail.setGUIProperty("user.sourcetrailer.label", GUILocalizeStrings.Get(10798649));
         MyFilmsDetail.setGUIProperty("user.source.label", GUILocalizeStrings.Get(10798648));
         MyFilmsDetail.setGUIProperty("nbobjects.unit", GUILocalizeStrings.Get(127));
         MyFilmsDetail.setGUIProperty("db.length.unit", GUILocalizeStrings.Get(2998));
         MyFilmsDetail.setGUIProperty("user.watched.label", GUILocalizeStrings.Get(200027));
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
         MyFilmsDetail.clearGUIProperty("picture");
       }

    private void Load_Logos(DataRow row)
       {
         LogMyFilms.Debug("MF: Using Logos -> '" + conf.StrLogos + "'");
         //if ((ImgID2001 != null) && (ImgID2002 != null) && (conf.StrLogos))
         //{
         //    if ((ImgID2001.XPosition == ImgID2002.XPosition) && (ImgID2001.YPosition == ImgID2002.YPosition))
         //    {
         //        logo_type = "ID2003";
         //        try
         //        {
         //            wlogos = Logos.Build_Logos(r[ItemId], logo_type, Math.Max(ImgID2001.Height, ImgID2002.Height), Math.Max(ImgID2001.Width, ImgID2002.Width), ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID);
         //        }
         //        catch
         //        {
         //        }
         //        //GUIControl.ClearControl(GetID, (int)Controls.CTRL_logos_id2001);
         //        LogMyFilms.Debug("MF: : Logo thumb assigned : " + wlogos);
         //        if (wlogos.Length == 0)
         //            wlogos = " ";
         //        MyFilmsDetail.setGUIProperty("logos_id2001", wlogos);
         //        MyFilmsDetail.clearGUIProperty("logos_id2002");

         //        //ImgID2001.DoUpdate();
         //        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
         //        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
         //    }
         //}
         //else
         //{
         //    if ((ImgID2001 != null) && (conf.StrLogos))
         //    {
         //        logo_type = "ID2001";
         //        try
         //        {
         //          MyFilmsDetail.setGUIProperty("logos_id2001", Logos.Build_Logos(MesFilms.r[ItemId], logo_type, ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID));  
         //        }
         //        catch (Exception e)
         //        {
         //          LogMyFilms.Error("MF: : " + e.Message);
         //        }
         //        if (wlogos.Length == 0)
         //            wlogos = string.Empty;
         //        //MyFilmsDetail.setGUIProperty("logos_id2001", wlogos);
         //        ////ImgID2001.DoUpdate();
         //        //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001); 
         //        //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
         //    }
         //    else
         //    {
         //      MyFilmsDetail.clearGUIProperty("logos_id2001");
         //    }

         //    if ((ImgID2002 != null) && (conf.StrLogos))
         //    {
         //        logo_type = "ID2002";
         //        try
         //        {
         //          MyFilmsDetail.setGUIProperty("logos_id2002", Logos.Build_Logos(MesFilms.r[ItemId], logo_type, ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID));  
         //          //wlogos = Logos.Build_Logos(r[ItemId], logo_type, ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID);
         //        }
         //        catch (Exception e)
         //        {
         //          LogMyFilms.Error("MF: : " + e.Message);
         //        }
         //        if (wlogos.Length == 0)
         //            wlogos = string.Empty;
         //        //MyFilmsDetail.setGUIProperty("logos_id2002", wlogos);
         //        //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002); 
         //        //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
         //    }
         //    else
         //    {
         //      MyFilmsDetail.clearGUIProperty("logos_id2002");
         //    }
         //    //if (wlogos.Length == 0)
         //    //{
         //    //    MyFilmsDetail.clearGUIProperty("logos_id2001");
         //    //    MyFilmsDetail.clearGUIProperty("logos_id2002");

         //    //}
         //}

         if ((ImgID2001 != null) && (MyFilms.conf.StrLogos))
         {
           try
           {
             MyFilmsDetail.setGUIProperty("logos_id2001", Logos.Build_Logos(row, "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, GetID));
           }
           catch (Exception e)
           {
             LogMyFilms.Error("MF: " + e.Message);
           }
         }
         else
           MyFilmsDetail.clearGUIProperty("logos_id2001");

         if ((ImgID2002 != null) && (MyFilms.conf.StrLogos))
         {
           try
           {
             MyFilmsDetail.setGUIProperty("logos_id2002", Logos.Build_Logos(row, "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, GetID));
           }
           catch (Exception e)
           {
             LogMyFilms.Error("MF: " + e.Message);
           }
         }
         else
           MyFilmsDetail.clearGUIProperty("logos_id2002");

         if ((ImgID2003 != null) && (MyFilms.conf.StrLogos))
         {
           try
           {
             MyFilmsDetail.setGUIProperty("logos_id2003", Logos.Build_Logos(row, "ID2003", ImgID2003.Height, ImgID2003.Width, ImgID2003.XPosition, ImgID2003.YPosition, GetID));
           }
           catch (Exception e)
           {
             LogMyFilms.Error("MF: " + e.Message);
           }
         }
         else
           MyFilmsDetail.clearGUIProperty("logos_id2003");

         if ((ImgID2012 != null) && (MyFilms.conf.StrLogos))
         {
           try
           {
             MyFilmsDetail.setGUIProperty("logos_id2012", Logos.Build_Logos(row, "ID2012", ImgID2012.Height, ImgID2012.Width, ImgID2012.XPosition, ImgID2012.YPosition, GetID));
           }
           catch (Exception e)
           {
             LogMyFilms.Error("MF: " + e.Message);
           }
         }
         else
           MyFilmsDetail.clearGUIProperty("logos_id2012");
       }

      private string GetJumpToViewName()
      {
        return "";
        //return GUIWindow._loadParameter; // Requires MePo 1.2+
      }

    #endregion

       //public void GetActorByName(string strActorName, ArrayList actors)
       //{
       //    SQLite.NET.SQLiteClient m_db = new SQLite.NET.SQLiteClient(Config.GetFile(Config.Dir.Database, @"VideoDatabaseV5.db3"));
       //    strActorName = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(strActorName);
       //    if (m_db == null)
       //    {
       //        return;
       //    }
       //    try
       //    {
       //        actors.Clear();
       //        SQLite.NET.SQLiteResultSet results = m_db.Execute("select * from Actors where strActor like '%" + strActorName + "%'");
       //        if (results.Rows.Count == 0)
       //        {
       //            return;
       //        }
       //        for (int iRow = 0; iRow < results.Rows.Count; iRow++)
       //        {
       //            actors.Add(MediaPortal.Database.DatabaseUtility.Get(results, iRow, "idActor") + "|" +
       //                       MediaPortal.Database.DatabaseUtility.Get(results, iRow, "strActor"));
       //        }
       //    }
       //    catch (Exception ex)
       //    {
       //        LogMyFilms.Error("MF: videodatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
       //    }
       //}

    }
}