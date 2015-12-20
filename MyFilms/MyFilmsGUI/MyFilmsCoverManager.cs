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
using System.Collections.Generic;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using Action = MediaPortal.GUI.Library.Action;
using System.ComponentModel;
using System.Drawing;
using MyFilmsPlugin.MyFilms.Utils;
using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System.Data;
  using System.IO;
  using MyFilmsPlugin.Utils;

  class MyFilmsCoverManager : GUIWindow
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #region Skin Controls

    [SkinControlAttribute(50)]
    protected GUIFacadeControl MFacade = null;

    [SkinControlAttribute(2)]
    protected GUIButtonControl ButtonLayouts = null;

    [SkinControlAttribute(11)]
    protected GUILabelControl LabelResolution = null;

    [SkinControlAttribute(12)]
    protected GUIButtonControl ButtonFilters = null;

    [SkinControlAttribute(13)]
    protected GUIButtonControl ButtonDownloadCover = null;

    [SkinControlAttribute((int)MyFilms.Controls.CTRL_GuiWaitCursor)]
    protected GUIAnimation SearchAnimation = null;

    #endregion

    #region Enums

    enum MenuAction
    {
      LoadSingle,
      LoadMultiple,
      CreateFromMovie,
      CreateFromMovieAsMosaic,
      LoadFromTmdb,
      DeleteSelected,
      DeleteAllLow,
      DeleteAllMedium,
      DeleteAllHigh,
      DeleteAllExceptSelected,
      UseAsDefault,
      Filters,
      ClearMovieCover,
      MenuMain,
      MenuDownload,
      MenuCreate,
      MenuDelete,
      MenuFilter
    }

    enum MenuFilterAction
    {
      all,
      high,
      medium,
      low
    }

    protected enum View
    {
      List = 0,
      Icons = 1,
      LargeIcons = 2,
      FilmStrip = 3,
      AlbumView = 4,
      PlayList = 5,
      CoverFlow = 6
    }

    #endregion

    BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
    int mPreviousSelectedItem = -1;
    private View currentView = View.LargeIcons;
    private int initialView = -1;
    bool mBQuickSelect = false;

    // rivate ImageSwapper CovermanagerBackground;
    // private Utils.Cornerstone.MP.AsyncImageResource CovermanagerBackground = null;

    # region DownloadWorker
    static MyFilmsCoverManager()
    {
      // lets set up the downloader
      //// create Backdrop image swapper
      //CovermanagerBackground = new ImageSwapper();
      //CovermanagerBackground.ImageResource.Delay = 250;
      //CovermanagerBackground.PropertyOne = "#myfilms.cover.fanart";
      //CovermanagerBackground.PropertyTwo = "#myfilms.cover.fanart2";
    }

    public MyFilmsCoverManager()
    {
      DisplayFilter = string.Empty;
      NewArtworkFileName = string.Empty;
      ArtworkFileName = string.Empty;
      MovieLabel = string.Empty;
      MovieId = -1;
    }

    #endregion

    public override int GetID
    { get { return MyFilms.ID_MyFilmsCoverManager; } }

    public override bool Init()
    {
      String xmlSkin = GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml";
      return Load(xmlSkin);
    }

    /// <summary>
    /// MediaPortal will set #currentmodule with GetModuleName()
    /// </summary>
    /// <returns>Localized Window Name</returns>
    public override string GetModuleName()
    {
      return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsCoverManager);
    }

    protected View CurrentView
    {
      get { return currentView; }
      set { currentView = value; }
    }

    protected override void OnPageLoad()
    {
      LogMyFilms.Debug("OnPageLoad() - Cover Manager Window initialization started ...");
      AllocResources();
      MovieId = MyFilms.conf.StrIndex;
      MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, this.MovieId));
      ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);

      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(10799201)); // MyFilms Cover Manager

      loadingWorker = new BackgroundWorker
      {
        WorkerReportsProgress = true,
        WorkerSupportsCancellation = true
      };
      loadingWorker.DoWork += WorkerDoWork;
      loadingWorker.ProgressChanged += WorkerProgressChanged;
      loadingWorker.RunWorkerCompleted += WorkerRunWorkerCompleted;

      if (MFacade != null)
      {
        if (initialView == -1)
        {
          initialView = 2; // bigthumbs as default ...
          CurrentView = (View)initialView;
        }
        MFacade.CurrentLayout = (GUIFacadeControl.Layout)CurrentView;
      }

      MyFilmsDetail.DetailsUpdated += OnDetailsUpdated;

      base.OnPageLoad();

      // update skin controls
      UpdateLayoutButton();
      if (LabelResolution != null) LabelResolution.Label = GUILocalizeStrings.Get(10799202); //  resolution

      if (ButtonFilters != null) ButtonFilters.Label = GUILocalizeStrings.Get(10799206); //  Filter
      if (ButtonDownloadCover != null) ButtonDownloadCover.Label = GUILocalizeStrings.Get(10799207); //  Download Covers

      ClearProperties();
      UpdateFilterProperty(false);

      string movielabel = MyFilms.r[MovieId][MyFilms.conf.StrTitle1].ToString();
      if (!string.IsNullOrEmpty(MyFilms.r[MovieId][MyFilms.conf.StrTitle2].ToString())) movielabel += " (" + MyFilms.r[MovieId][MyFilms.conf.StrTitle2] + ")";
      if (!string.IsNullOrEmpty(MyFilms.r[MovieId]["Year"].ToString())) movielabel += " - [" + MyFilms.r[MovieId]["Year"] + "]";
      MovieLabel = movielabel;

      MyFilmsDetail.setGUIProperty("cover.currentmoviename", MovieLabel);
      if (!string.IsNullOrEmpty(MyFilms.r[MovieId]["Picture"].ToString())) MyFilmsDetail.setGUIProperty("picture", ArtworkFileName); // only set preview, if DB is not empty ...

      loadingWorker.RunWorkerAsync(MovieId);
      LogMyFilms.Debug("OnPageLoad() - Cover Manager Window initialization finished ...");
    }

    protected bool AllowView(View view)
    {
      switch (view)
      {
        case View.List:
        case View.AlbumView:
        case View.PlayList:
          return false;
        default:
          return true;
      }
    }

    private void UpdateLayoutButton()
    {
      string strLine = string.Empty;
      View view = CurrentView;
      switch (view)
      {
        case View.List:
          strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101); // strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(101); // translations from Mediaportal !
          break;
        case View.Icons:
          strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(100);
          break;
        case View.LargeIcons:
          strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(417);
          break;
        case View.FilmStrip:
          strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(733);
          break;
        case View.PlayList:
          strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101);
          break;
        case View.CoverFlow:
          strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(791);
          break;
      }
      if (this.ButtonLayouts != null)
        GUIControl.SetControlLabel(GetID, this.ButtonLayouts.GetID, strLine);
    }

    private static void ClearProperties()
    {
      MyFilmsDetail.clearGUIProperty("cover.currentmoviename");
      MyFilmsDetail.clearGUIProperty("cover.count");
      MyFilmsDetail.clearGUIProperty("cover.selectedcoverresolution");
      MyFilmsDetail.clearGUIProperty("cover.selectedcoverresolutionclass");
      MyFilmsDetail.clearGUIProperty("cover.selectedcovername");

      MyFilmsDetail.clearGUIProperty("cover.selectedcoversize");
      MyFilmsDetail.clearGUIProperty("cover.selectedcoversizenum");
      MyFilmsDetail.clearGUIProperty("cover.selectedpreview");

    }

    private void UpdateFilterProperty(bool btnEnabled)
    {
      if (ButtonFilters != null) ButtonFilters.IsEnabled = btnEnabled;
      string resolution = (string.IsNullOrEmpty(DisplayFilter)) ? "All" : DisplayFilter;
      MyFilmsDetail.setGUIProperty("cover.filterresolution", resolution);
    }

    void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      MyFilmsDetail.setGUIProperty("cover.currentmoviename", MovieLabel);
      MyFilmsDetail.setGUIProperty("cover.count", this.MFacade.Count.ToString());

      if (int.Parse(MFacade.Count.ToString()) == 0)
      {
        // Enable Filters button in case Artwork is filtered
        if (DisplayFilter != "All" && !string.IsNullOrEmpty(DisplayFilter) && ButtonFilters != null)
        {
          OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
          OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
        }
      }

      // Load the selected facade so it's not black by default
      if (MFacade != null && MFacade.SelectedListItem != null && MFacade.SelectedListItem.TVTag != null)
      {
        if (MFacade.Count > mPreviousSelectedItem)
        {
          MFacade.SelectedListItemIndex = mPreviousSelectedItem <= 0 ? 0 : mPreviousSelectedItem;

          // Work around for Filmstrip not allowing to programmatically select item
          if (MFacade.CurrentLayout == GUIFacadeControl.Layout.Filmstrip)
          {
            mBQuickSelect = true;
            for (int i = 0; i < mPreviousSelectedItem; i++)
            {
              LogMyFilms.Debug("worker_RunWorkerCompleted() - move right to focus correct item");
              OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
            }
            mBQuickSelect = false;
            // Note: this is better way, but Scroll offset wont work after set
            //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, m_Facade.WindowId, 0, m_Facade.FilmstripLayout.GetID, m_PreviousSelectedItem, 0, null);
            //GUIGraphicsContext.SendMessage(msg);
          }
          mPreviousSelectedItem = -1;
        }

        var selectedCover = MFacade.SelectedListItem.TVTag as MFCover;
        if (selectedCover != null) SetItemProperties(selectedCover);
      }
      UpdateFilterProperty(true);
    }

    protected override void OnPageDestroy(int new_windowId)
    {
      MyFilmsDetail.DetailsUpdated -= OnDetailsUpdated;
      // MFCover selectedFanart = m_Facade.SelectedListItem.TVTag as MFCover;
      if (!string.IsNullOrEmpty(NewArtworkFileName) && File.Exists(NewArtworkFileName) && MyFilmsDetail.getGUIProperty("picture") == NewArtworkFileName)
        SaveChangesToDb();
      else
        LogMyFilms.Debug("OnPageDestroy - saveChangesToDB() - Cover file does not exist - not saving Cover '" + NewArtworkFileName + "' to DB !");

      if (loadingWorker.IsBusy) loadingWorker.CancelAsync();
      while (loadingWorker.IsBusy)
        System.Windows.Forms.Application.DoEvents();

      loadingWorker = null;
      // Helper.enableNativeAutoplay();
      base.OnPageDestroy(new_windowId);
    }

    public void setPageTitle(string Title)
    {
      MyFilmsDetail.setGUIProperty("cover.pagetitle", Title);
    }

    protected override void OnShowContextMenu()
    {
      ShowContextMenu();
    }


    #region Context Menu
    private void ShowContextMenu()
    {
      ShowContextMenu((int)MenuAction.MenuMain, true); // calls main context menu
    }
    private void ShowContextMenu(int menuaction, bool iscontextmenu)
    {
      try
      {
        bool bCoverSelected = true;
        var selectedCover = new MFCover();
        GUIListItem currentitem = MFacade.SelectedListItem;
        if (currentitem == null || !(currentitem.TVTag is MFCover))
          bCoverSelected = false; //return;
        else
          selectedCover = currentitem.TVTag as MFCover;

        var dlg = (IDialogbox)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
        GUIListItem pItem;
        if (dlg == null) return;
        dlg.Reset();

        switch (menuaction)
        {
          case (int)MenuAction.MenuMain:
            #region main menu
            dlg.SetHeading(GUILocalizeStrings.Get(10799201)); // MyFilms Cover Manager

            //// disabled, can be used by just select the item
            //if (bCoverSelected)
            //{
            //  pItem = new GUIListItem(GUILocalizeStrings.Get(10798769)); // Select Cover as Default
            //  dlg.Add(pItem);
            //  pItem.ItemId = (int)MenuAction.UseAsDefault;
            //}

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799204)); // Download Covers ...
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuAction.MenuDownload;

            if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "CoverManager: Submenu 'create covers ...'"))
            {
              pItem = new GUIListItem(GUILocalizeStrings.Get(10799208)); // Create Covers ...
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.MenuCreate;
            }

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799209)); // Delete Cover ...
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuAction.MenuDelete;

            if (!loadingWorker.IsBusy) // don't allowing filtering until all data is loaded
            {
              pItem = new GUIListItem(GUILocalizeStrings.Get(10799210)); // Filter Cover ...
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.MenuFilter;
            }
            #endregion
            break;

          case (int)MenuAction.MenuDownload:
            #region submenu download covers ...
            dlg.SetHeading(GUILocalizeStrings.Get(10799204)); // Download Covers ...

            pItem = new GUIListItem(GUILocalizeStrings.Get(10798766));  // Load single Cover ...
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuAction.LoadSingle;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10798764)); // Load multiple Covers ...
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuAction.LoadMultiple;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10798761)); // Load Covers (TMDB)
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuAction.LoadFromTmdb;
            #endregion
            break;

          case (int)MenuAction.MenuCreate:
            #region submenu create covers ...
            dlg.SetHeading(GUILocalizeStrings.Get(10799208));  // Create Covers ...

            if (MyFilmsDetail.ExtendedStartmode(MyFilmsDetail.PluginMode.Extended, "CoverManager: Creation of Covers from Movie not yet supported"))
            {
              pItem = new GUIListItem(GUILocalizeStrings.Get(10798728)); // create cover from movie ...
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.CreateFromMovie;

              pItem = new GUIListItem(GUILocalizeStrings.Get(10798729)); // Create cover from film as mosaic
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.CreateFromMovieAsMosaic;
            }
            #endregion
            break;

          case (int)MenuAction.MenuDelete:
            #region submenu delete covers ...
            dlg.SetHeading(GUILocalizeStrings.Get(10799209));  // Delete Covers ...

            if (bCoverSelected)
            {
              if (!loadingWorker.IsBusy && !string.IsNullOrEmpty(MyFilms.r[this.MovieId]["Picture"].ToString()))
              {
                pItem = new GUIListItem(GUILocalizeStrings.Get(10798810)); // Delete Movie Cover from DB
                dlg.Add(pItem);
                pItem.ItemId = (int)MenuAction.ClearMovieCover;
              }

              pItem = new GUIListItem(GUILocalizeStrings.Get(10799212)); //"Delete all 'Low'"
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.DeleteAllLow;

              pItem = new GUIListItem(GUILocalizeStrings.Get(10799213)); // "Delete all 'Medium'"
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.DeleteAllMedium;

              pItem = new GUIListItem(GUILocalizeStrings.Get(10799214)); // "Delete all 'High'"
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.DeleteAllHigh;

              pItem = new GUIListItem(GUILocalizeStrings.Get(10799211)); // Delete selected Cover
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.DeleteSelected;

              pItem = new GUIListItem(GUILocalizeStrings.Get(10799215)); // "Delete all except currently selected"
              dlg.Add(pItem);
              pItem.ItemId = (int)MenuAction.DeleteAllExceptSelected;
            }
            #endregion
            break;

          case (int)MenuAction.MenuFilter:
            #region submenu filter covers ... (calls extra method)
            dlg.SetHeading(GUILocalizeStrings.Get(10799210)); // Filter Cover ...

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799221)); // All
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuFilterAction.all;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799222)); // High
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuFilterAction.high;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799223)); // Medium
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuFilterAction.medium;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799224)); // Low
            dlg.Add(pItem);
            pItem.ItemId = (int)MenuFilterAction.low;
            #endregion
            break;
        }

        dlg.DoModal(GUIWindowManager.ActiveWindow);

        if (dlg.SelectedLabel == -1)
        {
          #region conditional return to main menu
          if (iscontextmenu)
          {
            switch (menuaction)
            {
              case (int)MenuAction.MenuMain:
                dlg.Reset();
                return;
              case (int)MenuAction.MenuDownload:
              case (int)MenuAction.MenuCreate:
              case (int)MenuAction.MenuDelete:
              case (int)MenuAction.MenuFilter:
                ShowContextMenu((int)MenuAction.MenuMain, true);
                break;
            }
          }
          else
          {
            return;
          }
          #endregion
        }

        string title = "";
        string mediapath = "";
        MyFilmsDetail.Searchtitles sTitles;
        switch (dlg.SelectedId) // what was chosen?
        {
          case (int)MenuAction.MenuDownload:
            dlg.Reset();
            ShowContextMenu((int)MenuAction.MenuDownload, iscontextmenu);
            break;
          case (int)MenuAction.MenuCreate:
            dlg.Reset();
            ShowContextMenu((int)MenuAction.MenuCreate, iscontextmenu);
            break;
          case (int)MenuAction.MenuDelete:
            dlg.Reset();
            ShowContextMenu((int)MenuAction.MenuDelete, iscontextmenu);
            break;
          case (int)MenuAction.MenuFilter:
            dlg.Reset();
            ShowFiltersMenu();
            break;

          case (int)MenuAction.LoadSingle:
            //downloadFanart(selectedCover);
            title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
            mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
            sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
            MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.Cover, false, sTitles, SearchAnimation);
            // RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
            break;
          case (int)MenuAction.LoadMultiple:
            //downloadFanart(selectedCover);
            title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
            mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
            sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
            MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.MultiCovers, false, sTitles, SearchAnimation);
            // RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
            break;
          case (int)MenuAction.LoadFromTmdb:
            sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
            MyFilmsDetail.Download_TMDB_Posters(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.Director, sTitles.Year.ToString(), false, GetID, sTitles.OriginalTitle, SearchAnimation);
            // RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
            break;
          case (int)MenuAction.CreateFromMovie:
            //ToDo: Add Code for single image thumbnailer
            // RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
            break;
          case (int)MenuAction.CreateFromMovieAsMosaic:
            //downloadFanart(selectedCover);
            MyFilmsDetail.CreateThumbFromMovie();
            // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
            break;
          case (int)MenuAction.DeleteSelected:
            // delete from DB, if it is the one currently selected
            if (NewArtworkFileName == selectedCover.FullPath)
            {
              NewArtworkFileName = "";
              sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[this.MovieId], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, this.MovieId));
              ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, this.MovieId, sTitles.MasterTitle);
              MyFilmsDetail.clearGUIProperty("picture");
              SaveChangesToDb();
            }
            selectedCover.Delete(AllTitles());
            // and reinit the display to get rid of it
            RefreshFacade();
            break;
          case (int)MenuAction.DeleteAllLow:
            DeleteSelectedFromGroup("Low");
            RefreshFacade();
            break;
          case (int)MenuAction.DeleteAllMedium:
            DeleteSelectedFromGroup("Medium");
            RefreshFacade();
            break;
          case (int)MenuAction.DeleteAllHigh:
            DeleteSelectedFromGroup("High");
            RefreshFacade();
            break;
          case (int)MenuAction.DeleteAllExceptSelected:
            DeleteAllExceptSelected(MFacade.SelectedListItemIndex);
            RefreshFacade();
            break;
          case (int)MenuAction.UseAsDefault:
            SetFacadeItemAsChosen(MFacade.SelectedListItemIndex);
            selectedCover.Chosen = true;
            SetDefaultCover(selectedCover);
            break;
          case (int)MenuAction.ClearMovieCover:
            dlg.Reset();
            RemoveMovieCoverFromDb();
            ClearProperties();
            UpdateFilterProperty(false);
            RefreshFacade();
            break;
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Exception in Artwork Chooser Context Menu: " + ex.Message);
      }
    }
    #endregion


    #region Context Menu - Filters
    private void ShowFiltersMenu()
    {
      var dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10799203)); // Filter...

      var pItem = new GUIListItem(GUILocalizeStrings.Get(10799221)); // All
      dlg.Add(pItem);
      pItem.ItemId = (int)MenuFilterAction.all;

      pItem = new GUIListItem(GUILocalizeStrings.Get(10799222)); // High
      dlg.Add(pItem);
      pItem.ItemId = (int)MenuFilterAction.high;

      pItem = new GUIListItem(GUILocalizeStrings.Get(10799223)); // Medium
      dlg.Add(pItem);
      pItem.ItemId = (int)MenuFilterAction.medium;

      pItem = new GUIListItem(GUILocalizeStrings.Get(10799224)); // Low
      dlg.Add(pItem);
      pItem.ItemId = (int)MenuFilterAction.low;

      dlg.DoModal(GUIWindowManager.ActiveWindow);
      if (dlg.SelectedId >= 0)
      {
        switch (dlg.SelectedId)
        {
          case (int)MenuFilterAction.all:
            DisplayFilter = "All";
            break;
          case (int)MenuFilterAction.high:
            DisplayFilter = "High";
            break;
          case (int)MenuFilterAction.medium:
            DisplayFilter = "Medium";
            break;
          case (int)MenuFilterAction.low:
            DisplayFilter = "Low";
            break;
          default:
            DisplayFilter = "";
            break;
        }
        ClearProperties();
        UpdateFilterProperty(false);
        RefreshFacade();
      }
    }
    #endregion

    void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      try
      {
        if (MFacade != null)
        {
          GUIListItem loadedItem = e.UserState as GUIListItem;
          if (loadedItem != null)
          {
            MFacade.Add(loadedItem);
            LogMyFilms.Debug("worker_ProgressChanged() - item '" + loadedItem.Label + "' added to facade");

            // we use this to tell the gui how many Artwork we are loading
            MyFilmsDetail.setGUIProperty("cover.count", MFacade.Count.ToString());
            if (MFacade != null) MFacade.Focus = true;
          }
          else
          {
            LogMyFilms.Debug("worker_ProgressChanged() - item not added (null)");
          }
          //else if (e.ProgressPercentage > 0)
          //{
          //  // we use this to tell the gui how many Artwork we are loading
          //  MyFilmsDetail.setGUIProperty("cover.counttotal", e.ProgressPercentage.ToString()); 
          //}
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Error: Artwork Chooser worker_ProgressChanged() experienced an error: " + ex.Message);
      }
    }

    void WorkerDoWork(object sender, DoWorkEventArgs e)
    {
      LoadThumbnails((int)e.Argument);
    }

    public int MovieId { get; set; }

    public string MovieLabel { get; set; }

    public string ArtworkFileName { get; set; }

    public string NewArtworkFileName { get; set; }

    public string DisplayFilter { get; set; }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        // Can't use OnMessage when using Filmstrip - it doesn't work!!
        case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
          {
            int iControl = message.SenderControlId;
            if (iControl == MFacade.GetID && MFacade.SelectedListItem != null)
            {
              var selectedFanart = MFacade.SelectedListItem.TVTag as MFCover;
              if (selectedFanart != null)
              {
                SetItemProperties(selectedFanart);
              }
            }
            return true;
          }
        default:
          return base.OnMessage(message);
      }
    }

    private void OnFacadeItemSelected(GUIListItem item, GUIControl parent) // triggered when a selection change was made on the facade
    {
      if (this.mBQuickSelect) return;
      if (parent != MFacade && parent != MFacade.FilmstripLayout && parent != MFacade.ThumbnailLayout && parent != MFacade.ListLayout) return; // if this is not a message from the facade, exit
      var selectedCover = item.TVTag as MFCover;
      if (selectedCover != null) SetItemProperties(selectedCover);
    }

    protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
    {
      if (control == this.ButtonFilters)
      {
        ShowFiltersMenu();
        ButtonFilters.Focus = false;
        return;
      }

      if (control == ButtonLayouts)
      {
        ChangeLayout();
        UpdateLayoutButton();
        RefreshFacade();
        GUIControl.FocusControl(GetID, controlId);
        return;
      }


      if (control == ButtonDownloadCover)
      {
        ShowContextMenu((int)MenuAction.MenuDownload, false);
        ButtonDownloadCover.Focus = false;
        GUIControl.FocusControl(GetID, 50);
        return;
      }

      if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
      if (control == MFacade)
      {
        MFCover chosen;
        if ((chosen = MFacade.SelectedListItem.TVTag as MFCover) != null)
        {
          SetFacadeItemAsChosen(MFacade.SelectedListItemIndex);
          chosen.Chosen = true; // if we already have it, we simply set the chosen property (will itself "unchoose" all the others)
          SetDefaultCover(chosen);
        }
      }
    }

    private void ChangeLayout()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      var menuentries = new List<int>();

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079901)); // View (Layout) ...
      if (AllowView(View.List) && MFacade.ListLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(101));//List
        menuentries.Add((int)View.List);
      }
      if (AllowView(View.Icons) && MFacade.ThumbnailLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(100));//Icons
        menuentries.Add((int)View.Icons);
      }
      if (AllowView(View.LargeIcons) && MFacade.ThumbnailLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
        menuentries.Add((int)View.LargeIcons);
      }
      if (AllowView(View.FilmStrip) && MFacade.FilmstripLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
        menuentries.Add((int)View.FilmStrip);
      }
      if (AllowView(View.AlbumView) && MFacade.AlbumListLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(529));//Cover list - used as extended list
        menuentries.Add((int)View.AlbumView);
      }
      if (AllowView(View.PlayList) && MFacade.PlayListLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(529));//Cover list - used as extended list
        menuentries.Add((int)View.AlbumView);
      }
      if (AllowView(View.CoverFlow) && this.MFacade.CoverFlowLayout != null)
      {
        dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
        menuentries.Add((int)View.CoverFlow);
      }

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;

      ChangeLayoutAction(menuentries[dlg.SelectedLabel]);
      dlg.DeInit();
    }

    private void ChangeLayoutAction(int iLayout)
    {
      LogMyFilms.Debug("Change_Layout_Action() - change facade layout to '" + iLayout + "'");
      switch (iLayout)
      {
        case 0:
          CurrentView = View.List;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
        case 1:
          CurrentView = View.Icons;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
          break;
        case 2:
          CurrentView = View.LargeIcons;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
        case 3:
          CurrentView = View.FilmStrip;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
          break;
        case 4:
          CurrentView = View.AlbumView;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.AlbumView;
          break;
        case 5:
          CurrentView = View.PlayList;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
          break;
        case 6:
          CurrentView = View.CoverFlow;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
          break;

        default:
          CurrentView = View.LargeIcons;
          MFacade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
      }
    }

    private void RefreshFacade() // (re)load facade
    {
      if (loadingWorker.IsBusy)
      {
        loadingWorker.CancelAsync();
        System.Threading.Thread.Sleep(500);
      }
      if (!loadingWorker.IsBusy)
      {
        MFacade.Clear();
        loadingWorker.RunWorkerAsync(MovieId);
      }
    }

    private void OnDetailsUpdated(bool searchPicture)
    {
      LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "'");
      if (GetID == MyFilms.ID_MyFilmsCoverManager)
      {
        LogMyFilms.Debug("OnDetailsUpdated(): now reloading Covermanager Facade");
        RefreshFacade();
      }
      else
        LogMyFilms.Debug("OnDetailsUpdated(): Skipping reloading Details");
    }

    void RemoveMovieCoverFromDb()
    {
      NewArtworkFileName = "";
      MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MovieId], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MovieId));
      ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, MovieId, sTitles.MasterTitle);
      MyFilmsDetail.clearGUIProperty("picture");
      LogMyFilms.Debug("RemoveMovieCoverFromDB() - Removed Cover from DB - new Covername (empty) = '" + ArtworkFileName + "'");
      SaveChangesToDb();
    }

    void SetFacadeItemAsChosen(int iSelectedItem)
    {
      try
      {
        for (int i = 0; i < MFacade.Count; i++)
        {
          if (i == iSelectedItem)
            MFacade[i].IsRemote = true;
          else
          {
            MFacade[i].IsRemote = false;
            MFCover item = MFacade[i].TVTag as MFCover;
            item.Chosen = false;
          }
        }
      }
      catch (Exception ex) { LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message); }
    }

    void DeleteSelectedFromGroup(string strQuality)
    {
      try
      {
        for (int i = 0; i < MFacade.Count; i++)
        {
          MFCover item = MFacade[i].TVTag as MFCover;
          if (item != null && item.ImageResolutionClass == strQuality)
            item.Delete(AllTitles());
        }
      }
      catch (Exception ex) { LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message); }
    }

    void DeleteAllExceptSelected(int iSelectedItem)
    {
      try
      {
        for (int i = 0; i < MFacade.Count; i++)
        {
          if (i != iSelectedItem)
          {
            MFCover item = MFacade[i].TVTag as MFCover;
            if (item != null)
              item.Delete(AllTitles());
          }
        }
      }
      catch (Exception ex) { LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message); }
    }

    void LoadThumbnails(int movieId)
    {
      if (movieId > -1)
      {
        if (loadingWorker.CancellationPending) return;

        GUIListItem item;
        GUIWaitCursor.Init(); GUIWaitCursor.Show(); // show waitcursor while covers are searched on local drive...
        var covers = GetLocalCover(MyFilms.r, MyFilms.conf.StrIndex, ArtworkFileName);
        GUIWaitCursor.Hide();

        int i = 0;
        foreach (var mfCover in covers)
        {
          bool add = false;
          item = null; // reset item to null
          switch (DisplayFilter)
          {
            case "High":
              if (mfCover.ImageResolutionClass == "High")
                add = true;
              break;
            case "Medium":
              if (mfCover.ImageResolutionClass == "Medium")
                add = true;
              break;
            case "Low":
              if (mfCover.ImageResolutionClass == "Low")
                add = true;
              break;
            default:
              add = true;
              break;
          }

          if (add)
          {
            item = new GUIListItem(mfCover.ImageResolution);
            item.IsRemote = false;
            item.TVTag = mfCover;
            item.Path = mfCover.FullPath;
            item.IconImageBig = ImageAllocator.GetOtherImage(mfCover.FullPath, new System.Drawing.Size(400, 600), false);
            item.IconImage = item.IconImageBig;
            item.ThumbnailImage = item.IconImageBig;
            item.OnItemSelected += OnFacadeItemSelected;
          }
          loadingWorker.ReportProgress(i < 100 ? ++i : 100, item);
          if (loadingWorker.CancellationPending) return;
        }
      }
    }

    void SetItemProperties(MFCover cover)
    {
      MyFilmsDetail.setGUIProperty("cover.selectedcoverresolution", cover.ImageResolution);
      MyFilmsDetail.setGUIProperty("cover.selectedcoverresolutionclass", cover.ImageResolutionClass);
      MyFilmsDetail.setGUIProperty("cover.selectedcoversize", cover.ImageSizeFriendly);
      MyFilmsDetail.setGUIProperty("cover.selectedcoversizenum", cover.ImageSize.ToString());
      MyFilmsDetail.setGUIProperty("cover.selectedcovername", cover.FileName);
      MyFilmsDetail.setGUIProperty("cover.selectedpreview", MFacade.SelectedListItem.Path);

    }

    void SetDefaultCover(MFCover cover)
    {
      NewArtworkFileName = !File.Exists(cover.FullPath) ? "" : cover.FullPath;
      MyFilmsDetail.setGUIProperty("picture", NewArtworkFileName);

      //MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
      //string currentPicture = MyFilmsDetail.getGUIProperty("picture");
      ////if ((r1[Index]["Picture"].ToString().IndexOf(":\\") == -1) && (r1[Index]["Picture"].ToString().Substring(0, 2) != "\\\\"))
      ////  MyFilms.conf.FileImage = MyFilms.conf.StrPathImg + "\\" + r1[Index]["Picture"].ToString();
      ////else
      ////  MyFilms.conf.FileImage = r1[Index]["Picture"].ToString();
      //if ((MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().IndexOf(":\\") == -1) && (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Substring(0, 2) != "\\\\"))
      //  currentPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[MyFilms.conf.StrIndex]["Picture"];
      //else
      //  currentPicture = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();

      //if (string.IsNullOrEmpty(currentPicture))
      //  currentPicture = sTitles.FanartTitle + ".jpg";

      //LogMyFilms.Debug("setDefaultCover - currentPicture = '" + currentPicture + "'");
      //if (System.IO.File.Exists(currentPicture))
      //{
      //  //string safeName = Grabber.GrabUtil.CreateFilename(title);
      //  //filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";

      //  string newFile = System.IO.Path.GetFileNameWithoutExtension(currentPicture) + " [" + currentPicture.GetHashCode() + "]." + System.IO.Path.GetExtension(currentPicture);
      //  LogMyFilms.Debug("setDefaultCover - newFile = '" + newFile + "'");
      //  try
      //  {
      //    System.IO.File.Copy(currentPicture, newFile, true);
      //    File.Delete(currentPicture);
      //  }
      //  catch (Exception ex)
      //  {
      //    LogMyFilms.Debug("setDefaultCover - error renaming currentPicture: " + ex.Message + ex.StackTrace);
      //  }
      //}
      //try
      //{
      //  File.Copy(cover.FullPath, currentPicture, true);
      //  File.Delete(cover.FullPath);
      //  LogMyFilms.Debug("setDefaultCover - sucessfully set new Default Cover: '" + currentPicture + "'");
      //}
      //catch (Exception ex)
      //{
      //  LogMyFilms.Debug("setDefaultCover - error renaming currentPicture: " + ex.Message + ex.StackTrace);
      //}
    }

    void SaveChangesToDb()
    {
      string newPictureCatalogname = string.IsNullOrEmpty(NewArtworkFileName) ? "" : MyFilmsDetail.GetPictureCatalogNameFromFilename(NewArtworkFileName);
      MyFilms.r[MovieId]["Picture"] = newPictureCatalogname;
      MyFilmsDetail.Update_XML_database();
      LogMyFilms.Info("saveChangesToDB() - Database updated with '" + newPictureCatalogname + "' for Cover file '" + NewArtworkFileName + "'");
      //MyFilmsDetail.afficher_detail(true);
    }

    //-------------------------------------------------------------------------------------------
    //  Get local Cover Image
    //-------------------------------------------------------------------------------------------        
    public static IEnumerable<MFCover> GetLocalCover(DataRow[] r1, int Index, string currentPictureFilename)
    {
      List<MFCover> result = new List<MFCover>();
      string directoryname = "";
      string[] files = null;
      Int64 wsize = 0; // Temporary Filesize detection
      //string currentPicture = MyFilmsDetail.getGUIProperty("picture");

      //if ((r1[Index]["Picture"].ToString().IndexOf(":\\") == -1) && (r1[Index]["Picture"].ToString().Substring(0, 2) != "\\\\"))
      //  currentPictureFilename = MyFilms.conf.StrPathImg + "\\" + r1[Index]["Picture"];
      //else
      //  currentPictureFilename = r1[Index]["Picture"].ToString();

      if (string.IsNullOrEmpty(currentPictureFilename))
        return null;
      string searchPattern = GetCoverSearchPattern(currentPictureFilename);
      if (searchPattern == null)
        return null;

      directoryname = currentPictureFilename.Substring(0, currentPictureFilename.LastIndexOf("\\"));
      LogMyFilms.Debug("CoverManager - searchPattern = '" + searchPattern + "', directoryname = '" + directoryname + "'");

      if (!string.IsNullOrEmpty(directoryname))
      {
        files = Directory.GetFiles(directoryname, @"*" + searchPattern + @"*.jpg", SearchOption.TopDirectoryOnly);
        foreach (string filefound in files)
        {
          #region add found files
          wsize = new FileInfo(filefound).Length;
          var item = new MFCover();
          item.FileName = Path.GetFileNameWithoutExtension(filefound);
          item.FullPath = filefound;

          //if (!File.Exists(localFile) || ImageFast.FastFromFile(localFile) == null) {}
          var newCover = Image.FromFile(filefound);
          item.ImageResolution = newCover.Width + " x " + newCover.Height;
          item.ImageWith = newCover.Width;
          item.ImageHeight = newCover.Height;

          if (newCover.Height > 800) item.ImageResolutionClass = "High";
          else if (newCover.Height > 400) item.ImageResolutionClass = "Medium";
          else if (newCover.Height > 0) item.ImageResolutionClass = "Low";
          else item.ImageResolutionClass = "Unknown";
          newCover.Dispose();

          item.ImageSize = new FileInfo(filefound).Length;
          item.ImageSizeFriendly = Helper.GetFileSize(wsize);
          result.Add(item);
          #endregion
        }
      }
      return result;
    }

    private static string GetCoverSearchPattern(string currentPictureFilename)
    {
      string startPattern = "";
      string currentStorePath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;

      if (!currentPictureFilename.StartsWith(currentStorePath))
        return null;
      startPattern = currentStorePath.EndsWith("\\") ? "" : currentPictureFilename.Substring(currentPictureFilename.LastIndexOf("\\") + 1);
      string searchPattern = currentPictureFilename.Substring(currentStorePath.Length);

      int patternLength2 = 999;
      int patternLength = searchPattern.LastIndexOf(".");
      if (searchPattern.Contains("["))
        patternLength2 = searchPattern.LastIndexOf("[") - 1;
      if (patternLength2 < patternLength)
        patternLength = patternLength2;
      searchPattern = searchPattern.Substring(0, patternLength);
      return searchPattern;
    }

    private List<string> AllTitles()
    {
      var allTitles = new List<string>();
      string title = null;
      title = MyFilms.r[MovieId]["OriginalTitle"].ToString();
      if (!string.IsNullOrEmpty(title)) allTitles.Add(title);
      title = MyFilms.r[MovieId]["TranslatedTitle"].ToString();
      if (!string.IsNullOrEmpty(title)) allTitles.Add(title);
      title = MyFilms.r[MovieId]["FormattedTitle"].ToString();
      if (!string.IsNullOrEmpty(title)) allTitles.Add(title);
      return allTitles;
    }
  }

  public class MFCover
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public MFCover() { }

    public void Delete(List<string> titles)
    {
      try
      {
        File.Delete(FullPath);
        LogMyFilms.Debug("Artwork Deleted: " + FullPath);
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Failed to delete file: " + FullPath + " (" + ex.Message + ")");
      }

      try // also delete image from thumbscache directory
      {
        string CoverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";
        foreach (string title in titles)
        {
          string strThumb = MediaPortal.Util.Utils.GetCoverArtName(CoverThumbDir, title);
          if (File.Exists(strThumb)) File.Delete(strThumb);
          string strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".")) + "_s" + Path.GetExtension(strThumb);
          if (File.Exists(strThumbSmall)) File.Delete(strThumbSmall);
          LogMyFilms.Debug("Cached thumbs deleted for title = '" + title + "'");
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Failed to delete cache thumb file - exception: " + ex.Message + "");
      }
    }

    public string FullPath { get; set; }

    public string FileName { get; set; }

    public long ImageSize { get; set; }

    public string ImageSizeFriendly { get; set; }

    public int ImageWith { get; set; }

    public int ImageHeight { get; set; }

    public string ImageResolution { get; set; }

    public string ImageResolutionClass { get; set; }

    public bool Chosen { get; set; }
  }

}
