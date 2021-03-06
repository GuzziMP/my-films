﻿#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// MyFilms - Plugin for Mediaportal
// Copyright (C) 2006-2012
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
using System.Text;
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;
using System.ComponentModel;
using System.Drawing;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System.IO;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.Utils;

  using WatTmdb.V3;

  using Translation = MyFilmsPlugin.MyFilms.Utils.Translation;

  class MyFilmsFanartManager : GUIWindow
  {
    #region skin controls
    [SkinControlAttribute(50)]
    protected GUIFacadeControl m_Facade = null;

    [SkinControlAttribute(2)]
    protected GUIButtonControl buttonLayouts = null;

    [SkinControlAttribute(11)]
    protected GUILabelControl labelResolution = null;

    [SkinControlAttribute(12)]
    protected GUIButtonControl buttonFilters = null;

    [SkinControlAttribute(13)]
    protected GUIToggleButtonControl togglebuttonRandom = null;

    [SkinControlAttribute(14)]
    protected GUILabelControl labelDisabled = null;

    [SkinControlAttribute(15)]
    protected GUILabelControl labelChosen = null;
    #endregion

    #region enums
    enum menuAction
    {
      use,
      download,
      delete,
      optionRandom,
      disable,
      enable,
      filters,
      interval,
      clearcache
    }

    enum menuFilterAction
    {
      all,
      hd,
      fullhd
    }

    enum menuIntervalAction
    {
      FiveSeconds,
      TenSeconds,
      FifteenSeconds,
      ThirtySeconds,
      FortyFiveSeconds,
      SixtySeconds
    }

    public enum View
    {
      List = 0,
      Icons = 1,
      LargeIcons = 2,
      FilmStrip = 3,
      AlbumView = 4,
      PlayList = 5
    }
    #endregion

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();
    int seriesID = -1;
    BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
    public static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the actual downloading
    static Queue<MFFanart> toDownload = new Queue<MFFanart>();
    private object locker = new object();
    int m_PreviousSelectedItem = -1;
    private View currentView = View.LargeIcons;
    bool m_bQuickSelect = false;

    # region DownloadWorker
    static MyFilmsFanartManager()
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
      if (loadingWorker != null && !loadingWorker.IsBusy)
      {
        m_PreviousSelectedItem = m_Facade.SelectedListItemIndex;

        if (m_Facade != null) m_Facade.Clear();
        loadingWorker.RunWorkerAsync(SeriesID);
      }
    }

    static void downloadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      setDownloadStatus();
    }

    static void downloadingWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      do
      {
        MFFanart f;
        setDownloadStatus();
        lock (toDownload)
        {
          f = toDownload.Dequeue();
        }

        bool bDownloadSuccess = true;
        if (f != null && !f.isAvailableLocally)
        {
          string filename = f.FullLocalPath;
          filename = filename.Replace("/", @"\");
          string fullURL = f.RemotePath;
          string filename1person = Grabber.GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathFanart, fullURL, "titlename", false, true, out filename);
          //int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.fanart, filename);
          //while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID))
          //{
          //  if (downloadingWorker.CancellationPending)
          //  {
          //    // Cancel, clean up pending download
          //    bDownloadSuccess = false;
          //    Online_Parsing_Classes.OnlineAPI.CancelFileDownload(nDownloadGUID);
          //    LogMyFilms.Debug("cancel Fanart download: " + f.FullLocalPath);
          //  }
          //  System.Windows.Forms.Application.DoEvents();
          //}
          // Download is either completed or canceled
          if (bDownloadSuccess)
          {
            //f[MFFanart.cLocalPath] = filename.Replace(Settings.GetPath(Settings.Path.fanart), string.Empty);
            //LogMyFilms.Debug("Successfully downloaded Fanart: " + f.FullLocalPath);
            //downloadingWorker.ReportProgress(0, f[DBFanart.cIndex]);
          }
          else
            LogMyFilms.Debug("Error downloading Fanart: " + f.FullLocalPath);
        }
      }
      while (toDownload.Count > 0 && !downloadingWorker.CancellationPending);
    }

    static void setDownloadStatus()
    {
      lock (toDownload)
      {
        if (toDownload.Count > 0)
        {
          MyFilmsDetail.setGUIProperty("FanArt.DownloadingStatus", string.Format("Translation.FanDownloadingStatus", toDownload.Count));
        }
        else
          MyFilmsDetail.setGUIProperty("FanArt.DownloadingStatus", " ");
      }
    }

    #endregion

    public static int GetWindowID
    { get { return MyFilms.ID_MyFilmsFanartManager; } }

    public override int GetID
    { get { return MyFilms.ID_MyFilmsFanartManager; } }

    public int GetWindowId()
    { return MyFilms.ID_MyFilmsFanartManager; }

    public override bool Init()
    {
      String xmlSkin = GUIGraphicsContext.Skin + @"\MyFilmsFanartManager.xml";
      return Load(xmlSkin);
    }

    /// <summary>
    /// MediaPortal will set #currentmodule with GetModuleName()
    /// </summary>
    /// <returns>Localized Window Name</returns>
    //public override string GetModuleName() {
    //	return Translation.FanArt;
    //}

    protected View CurrentView
    {
      get { return currentView; }
      set { currentView = value; }
    }

    protected override void OnPageLoad()
    {
      AllocResources();

      GUIPropertyManager.SetProperty("#currentmodule", "Translation.FanArt");

      loadingWorker = new BackgroundWorker();
      loadingWorker.WorkerReportsProgress = true;
      loadingWorker.WorkerSupportsCancellation = true;
      loadingWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
      loadingWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
      loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

      if (m_Facade != null)
      {
        int defaultView = 2;
        // ToDo: Add persistant setting for view in MyFilms config
        // if (int.TryParse(MyFilms.conf.FmDefaultView, out defaultView))
        {
          CurrentView = (View)defaultView;
        }
        m_Facade.CurrentLayout = (GUIFacadeControl.Layout)CurrentView;
      }

      base.OnPageLoad();

      Helper.disableNativeAutoplay();

      // update skin controls
      UpdateLayoutButton();
      if (labelResolution != null) labelResolution.Label = "Translation.LabelResolution";
      if (labelChosen != null) labelChosen.Label = "Translation.LabelChosen";
      if (labelDisabled != null) labelDisabled.Label = "Translation.LabelDisabled";
      if (buttonFilters != null) buttonFilters.Label = "Translation.FanArtFilter";
      if (togglebuttonRandom != null)
      {
        togglebuttonRandom.Label = "Translation.ButtonRandomFanart";
        togglebuttonRandom.Selected = true; //  MyFilms.conf.FmUseRandomFanart;
      }

      ClearProperties();
      UpdateFilterProperty(false);

      setDownloadStatus();

      LogMyFilms.Info("Fanart Chooser Window initializing");

      fetchList(SeriesID);
      loadingWorker.RunWorkerAsync(SeriesID);

      downloadingWorker.ProgressChanged += new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);

    }

    protected bool AllowView(View view)
    {
      if (view == View.List)
        return false;

      if (view == View.AlbumView)
        return false;

      if (view == View.PlayList)
        return false;

      return true;
    }

    private void UpdateLayoutButton()
    {
      string strLine = string.Empty;
      View view = CurrentView;
      switch (view)
      {
        case View.List:
          strLine = GUILocalizeStrings.Get(101);
          break;
        case View.Icons:
          strLine = GUILocalizeStrings.Get(100);
          break;
        case View.LargeIcons:
          strLine = GUILocalizeStrings.Get(417);
          break;
        case View.FilmStrip:
          strLine = GUILocalizeStrings.Get(733);
          break;
        case View.PlayList:
          strLine = GUILocalizeStrings.Get(101);
          break;
      }
      if (buttonLayouts != null)
        GUIControl.SetControlLabel(GetID, buttonLayouts.GetID, strLine);
    }

    private void ClearProperties()
    {
      MyFilmsDetail.setGUIProperty("FanArt.Count", " ");
      MyFilmsDetail.setGUIProperty("FanArt.LoadingStatus", " ");
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartInfo", " ");
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartResolution", " ");
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsChosen", " ");
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsDisabled", " ");
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartColors", " ");
    }

    private void UpdateFilterProperty(bool btnEnabled)
    {
      if (buttonFilters != null)
        buttonFilters.IsEnabled = btnEnabled;

      string resolution = string.Empty;
      // if (MyFilms.conf.FmFanartThumbnailResolutionFilter == "0")
      {
        resolution = "Translation.FanArtFilterAll";
      }
      //else if (MyFilms.conf.FmFanartThumbnailResolutionFilter == "1")
      //{
      //  resolution = "1280x720";
      //}
      //else
      //  resolution = "1920x1080";

      MyFilmsDetail.setGUIProperty("FanArt.FilterResolution", resolution);
    }

    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      MyFilmsDetail.setGUIProperty("FanArt.LoadingStatus", string.Empty);
      MyFilmsDetail.setGUIProperty("FanArt.Count", totalFanart.ToString());

      if (totalFanart == 0)
      {
        MyFilmsDetail.setGUIProperty("FanArt.LoadingStatus", "Translation.FanArtNoneFound");
        //// Enable Filters button in case fanart is filtered
        //if (MyFilms.conf.FmFanartThumbnailResolutionFilter != "0" && buttonFilters != null)
        //{
        //  OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
        //  OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
        //}
      }
      totalFanart = 0;

      // Load the selected facade so it's not black by default
      if (m_Facade != null && m_Facade.SelectedListItem != null && m_Facade.SelectedListItem.TVTag != null)
      {
        if (m_Facade.Count > m_PreviousSelectedItem)
        {
          if (m_PreviousSelectedItem <= 0)
            m_Facade.SelectedListItemIndex = 0;
          else
            m_Facade.SelectedListItemIndex = m_PreviousSelectedItem;

          // Work around for Filmstrip not allowing to programmatically select item
          if (m_Facade.CurrentLayout == GUIFacadeControl.Layout.Filmstrip)
          {
            m_bQuickSelect = true;
            for (int i = 0; i < m_PreviousSelectedItem; i++)
            {
              OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
            }
            m_bQuickSelect = false;
            // Note: this is better way, but Scroll offset wont work after set
            //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, m_Facade.WindowId, 0, m_Facade.FilmstripLayout.GetID, m_PreviousSelectedItem, 0, null);
            //GUIGraphicsContext.SendMessage(msg);
            //MPTVSeriesLog.Write("Sending a selection postcard to FilmStrip.", MPTVSeriesLog.LogLevel.Debug);
          }
          m_PreviousSelectedItem = -1;
        }


        MFFanart selectedFanart = m_Facade.SelectedListItem.TVTag as MFFanart;
        if (selectedFanart != null)
        {
          setFanartPreviewBackground(selectedFanart);
        }
      }
      UpdateFilterProperty(true);
    }

    protected override void OnPageDestroy(int new_windowId)
    {
      // MyFilms.conf.FmDefaultView = (int)CurrentView; / save curerent view to config

      if (loadingWorker.IsBusy)
        loadingWorker.CancelAsync();
      while (loadingWorker.IsBusy)
        System.Windows.Forms.Application.DoEvents();

      loadingWorker = null;

      Helper.enableNativeAutoplay();

      base.OnPageDestroy(new_windowId);
    }

    public void setPageTitle(string Title)
    {
      MyFilmsDetail.setGUIProperty("FanArt.PageTitle", Title);
    }

    #region Context Menu
    protected override void OnShowContextMenu()
    {
      try
      {
        GUIListItem currentitem = this.m_Facade.SelectedListItem;
        if (currentitem == null || !(currentitem.TVTag is MFFanart)) return;
        MFFanart selectedFanart = currentitem.TVTag as MFFanart;

        IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading("Translation.FanArt");

        GUIListItem pItem;
        if (false) // if (MyFilms.conf.MfUseRandomFanart)
        {
          // if random it doesnt make sense to offer an option to explicitally use an available fanart
          if (!selectedFanart.isAvailableLocally)
          {
            pItem = new GUIListItem("Translation.FanArtGetAndUse");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuAction.download;
          }
        }
        else
        {
          // if we are not random, we can choose available fanart
          if (selectedFanart.isAvailableLocally && !selectedFanart.Disabled)
          {
            pItem = new GUIListItem("Translation.FanArtUse");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuAction.use;
          }
          else if (!selectedFanart.isAvailableLocally)
          {
            pItem = new GUIListItem("Translation.FanArtGet");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuAction.download;
          }
        }

        if (selectedFanart.isAvailableLocally)
        {
          if (selectedFanart.Disabled)
          {
            pItem = new GUIListItem("Translation.FanartMenuEnable");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuAction.enable;
          }
          else
          {
            pItem = new GUIListItem("Translation.FanartMenuDisable");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuAction.disable;
          }
        }

        //pItem = new GUIListItem("Translation.FanArtRandom" + " (" + (MyFilms.conf.MfUseRandomFanart) ? "Translation.on" : "Translation.off") + ")");
        //dlg.Add(pItem);
        //pItem.ItemId = (int)menuAction.optionRandom;

        // Dont allowing filtering until DB has all data
        if (!loadingWorker.IsBusy)
        {
          pItem = new GUIListItem("Translation.FanArtFilter" + " ...");
          dlg.Add(pItem);
          pItem.ItemId = (int)menuAction.filters;
        }

        pItem = new GUIListItem("Translation.FanartRandomInterval" + " ...");
        dlg.Add(pItem);
        pItem.ItemId = (int)menuAction.interval;

        if (!loadingWorker.IsBusy)
        {
          pItem = new GUIListItem("Translation.ClearFanartCache");
          dlg.Add(pItem);
          pItem.ItemId = (int)menuAction.clearcache;
        }

        // lets show it
        dlg.DoModal(GUIWindowManager.ActiveWindow);
        switch (dlg.SelectedId) // what was chosen?
        {
          case (int)menuAction.delete:
            if (selectedFanart.isAvailableLocally)
            {
              selectedFanart.Delete();
              // and reinit the display to get rid of it
              m_Facade.Clear();
              loadingWorker.RunWorkerAsync(SeriesID);
            }
            break;
          case (int)menuAction.download:
            if (!selectedFanart.isAvailableLocally)
              downloadFanart(selectedFanart);
            break;
          case (int)menuAction.use:
            if (selectedFanart.isAvailableLocally)
            {
              MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsChosen", "Translation.Yes");
              SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);

              selectedFanart.Chosen = true;
              Fanart.RefreshFanart(SeriesID);
            }
            break;
          case (int)menuAction.optionRandom:
            // ToDo: set ramdon option setting
            bool toggleoption = false;
            toggleoption = !toggleoption;
            if (togglebuttonRandom != null) togglebuttonRandom.Selected = toggleoption;
            break;
          case (int)menuAction.disable:
            selectedFanart.Disabled = true;
            selectedFanart.Chosen = false;
            currentitem.Label = "Translation.FanartDisableLabel";
            MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsDisabled", "Translation.Yes");
            MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsChosen", "Translation.No");
            break;
          case (int)menuAction.enable:
            selectedFanart.Disabled = false;
            currentitem.Label = "Translation.FanArtLocal";
            MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsDisabled", "Translation.No");
            break;
          case (int)menuAction.filters:
            dlg.Reset();
            ShowFiltersMenu();
            break;
          case (int)menuAction.interval:
            dlg.Reset();
            ShowIntervalMenu();
            break;
          case (int)menuAction.clearcache:
            dlg.Reset();
            // ToDo: Fanart.ClearFanartCache(SeriesID);
            m_Facade.Clear();
            fetchList(SeriesID);
            loadingWorker.RunWorkerAsync(SeriesID);
            break;

        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Exception in Fanart Chooser Context Menu: " + ex.Message);
        return;
      }
    }
    #endregion

    #region Context Menu - Random Fanart Interval
    private void ShowIntervalMenu()
    {
      IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;

      dlg.Reset();
      dlg.SetHeading("Translation.FanartRandomInterval");

      GUIListItem pItem = new GUIListItem("Translation.FanartIntervalFiveSeconds");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuIntervalAction.FiveSeconds;

      pItem = new GUIListItem("Translation.FanartIntervalTenSeconds");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuIntervalAction.TenSeconds;

      pItem = new GUIListItem("Translation.FanartIntervalFifteenSeconds");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuIntervalAction.FifteenSeconds;

      pItem = new GUIListItem("Translation.FanartIntervalThirtySeconds");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuIntervalAction.ThirtySeconds;

      pItem = new GUIListItem("Translation.FanartIntervalFortyFiveSeconds");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuIntervalAction.FortyFiveSeconds;

      pItem = new GUIListItem("Translation.FanartIntervalSixtySeconds");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuIntervalAction.SixtySeconds;

      dlg.DoModal(GUIWindowManager.ActiveWindow);
      if (dlg.SelectedId >= 0)
      {
        switch (dlg.SelectedId)
        {
          case (int)menuIntervalAction.FiveSeconds:
            break;
          case (int)menuIntervalAction.TenSeconds:
            break;
          case (int)menuIntervalAction.FifteenSeconds:
            break;
          case (int)menuIntervalAction.ThirtySeconds:
            break;
          case (int)menuIntervalAction.FortyFiveSeconds:
            break;
          case (int)menuIntervalAction.SixtySeconds:
            break;
        }
      }
    }
    #endregion

    #region Context Menu - Filters
    private void ShowFiltersMenu()
    {
      IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;

      dlg.Reset();
      dlg.SetHeading("Translation.FanArtFilter");

      GUIListItem pItem = new GUIListItem("Translation.FanArtFilterAll");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuFilterAction.all;

      pItem = new GUIListItem("1280x720");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuFilterAction.hd;

      pItem = new GUIListItem("1920x1080");
      dlg.Add(pItem);
      pItem.ItemId = (int)menuFilterAction.fullhd;

      dlg.DoModal(GUIWindowManager.ActiveWindow);
      if (dlg.SelectedId >= 0)
      {
        switch (dlg.SelectedId)
        {
          case (int)menuFilterAction.all:
            // DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "0");
            break;
          case (int)menuFilterAction.hd:
            break;
          case (int)menuFilterAction.fullhd:
            break;
        }
        m_Facade.Clear();
        MFFanart.ClearAll();
        ClearProperties();

        UpdateFilterProperty(false);
        loadingWorker.RunWorkerAsync(SeriesID);
      }
    }
    #endregion

    int totalFanart;
    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      try
      {
        if (m_Facade != null)
        {
          GUIListItem loadedItem = e.UserState as GUIListItem;
          if (loadedItem != null)
          {
            m_Facade.Add(loadedItem);
            // we use this to tell the gui how many fanart we are loading
            MyFilmsDetail.setGUIProperty("FanArt.LoadingStatus", string.Format("Translation.FanArtOnlineLoading", e.ProgressPercentage, totalFanart));
            MyFilmsDetail.setGUIProperty("FanArt.Count", e.ProgressPercentage.ToString());
            if (m_Facade != null) this.m_Facade.Focus = true;
          }
          else if (e.ProgressPercentage > 0)
          {
            // we use this to tell the gui how many fanart we are loading
            MyFilmsDetail.setGUIProperty("FanArt.LoadingStatus", string.Format("Translation.FanArtOnlineLoading", 0, e.ProgressPercentage));
            totalFanart = e.ProgressPercentage;
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Error: Fanart Chooser worker_ProgressChanged() experienced an error: " + ex.Message);
      }
    }


    void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      loadThumbnails((int)e.Argument);
    }

    public int SeriesID
    {
      get { return seriesID; }
      set { seriesID = value; }
    }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        // Can't use OnMessage when using Filmstrip - it doesn't work!!
        case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
          {
            int iControl = message.SenderControlId;
            if (iControl == (int)m_Facade.GetID && m_Facade.SelectedListItem != null)
            {
              MFFanart selectedFanart = m_Facade.SelectedListItem.TVTag as MFFanart;
              if (selectedFanart != null)
              {
                setFanartPreviewBackground(selectedFanart);
              }
            }
            return true;
          }
        default:
          return base.OnMessage(message);
      }
    }

    // triggered when a selection change was made on the facade
    private void onFacadeItemSelected(GUIListItem item, GUIControl parent)
    {
      if (m_bQuickSelect) return;

      // if this is not a message from the facade, exit
      if (parent != m_Facade && parent != m_Facade.FilmstripLayout &&
          parent != m_Facade.ThumbnailLayout && parent != m_Facade.ListLayout)
        return;

      MFFanart selectedFanart = item.TVTag as MFFanart;
      if (selectedFanart != null)
      {
        setFanartPreviewBackground(selectedFanart);
      }

    }

    protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    {
      if (control == buttonFilters)
      {
        ShowFiltersMenu();
        buttonFilters.Focus = false;
        return;
      }

      if (control == togglebuttonRandom)
      {
        // DBOption.SetOptions(DBOption.cFanartRandom, togglebuttonRandom.Selected);
        togglebuttonRandom.Focus = false;
        return;
      }

      if (control == buttonLayouts)
      {
        bool shouldContinue = false;
        do
        {
          shouldContinue = false;
          switch (CurrentView)
          {
            case View.List:
              CurrentView = View.PlayList;
              if (!AllowView(CurrentView) || m_Facade.PlayListLayout == null)
              {
                shouldContinue = true;
              }
              else
              {
                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
              }
              break;

            case View.PlayList:
              CurrentView = View.Icons;
              if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
              {
                shouldContinue = true;
              }
              else
              {
                m_Facade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
              }
              break;

            case View.Icons:
              CurrentView = View.LargeIcons;
              if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
              {
                shouldContinue = true;
              }
              else
              {
                m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
              }
              break;

            case View.LargeIcons:
              CurrentView = View.FilmStrip;
              if (!AllowView(CurrentView) || m_Facade.FilmstripLayout == null)
              {
                shouldContinue = true;
              }
              else
              {
                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
              }
              break;

            case View.FilmStrip:
              CurrentView = View.List;
              if (!AllowView(CurrentView) || m_Facade.ListLayout == null)
              {
                shouldContinue = true;
              }
              else
              {
                m_Facade.CurrentLayout = GUIFacadeControl.Layout.List;
              }
              break;
          }
        } while (shouldContinue);
        UpdateLayoutButton();
        GUIControl.FocusControl(GetID, controlId);
      }

      if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
      if (control == this.m_Facade)
      {
        MFFanart chosen;
        if ((chosen = this.m_Facade.SelectedListItem.TVTag as MFFanart) != null)
        {
          if (chosen.isAvailableLocally && !chosen.Disabled)
          {
            MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsChosen", "Translation.Yes");
            SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);

            // if we already have it, we simply set the chosen property (will itself "unchoose" all the others)
            chosen.Chosen = true;
            // ZF: be sure to update the list of downloaded data in the cache - otherwise the selected fanart won't show up for new fanarts until restarted
            Fanart.RefreshFanart(SeriesID);

          }
          else if (!chosen.isAvailableLocally)
          {
            downloadFanart(chosen);
          }
        }
      }
    }

    void SetFacadeItemAsChosen(int iSelectedItem)
    {
      try
      {
        for (int i = 0; i < m_Facade.Count; i++)
        {
          if (i == iSelectedItem)
            m_Facade[i].IsRemote = true;
          else
          {
            m_Facade[i].IsRemote = false;
            MFFanart item;
            item = m_Facade[i].TVTag as MFFanart;
            item.Chosen = false;
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message);
      }
    }

    void downloadFanart(MFFanart fanart)
    {
      // we need to get it, let's queue them up and download in the background
      lock (toDownload)
      {
        toDownload.Enqueue(fanart);
      }
      setDownloadStatus();
      // don't return, user can queue up multiple fanart to download
      // the last he selects to download will be the chosen one by default

      // finally lets check if the downloader is already running, and if not start it
      if (!downloadingWorker.IsBusy)
        downloadingWorker.RunWorkerAsync();
    }

    void fetchList(int seriesID)
    {
      // Fetch a fresh list online and save info about them to the db 
      //GetFanart gf = new GetFanart(seriesID);
      //foreach (MFFanart f in gf.Fanart)
      //{
      //  f.Commit();
      //}
    }

    void loadThumbnails(int seriesID)
    {
      if (seriesID > 0)
      {
        if (loadingWorker.CancellationPending)
          return;

        GUIListItem item = null;
        List<MFFanart> onlineFanart = MFFanart.GetAll(seriesID, false);

        // sort fanart by highest rated
        onlineFanart.Sort();

        //// Filter Fanart Thumbnails to be displayed by resolution
        //if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) != 0)
        //{
        //  string filteredRes = (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) == "1" ? "1280x720" : "1920x1080");
        //  for (int j = onlineFanart.Count - 1; j >= 0; j--)
        //  {
        //    if (onlineFanart[j][DBFanart.cResolution] != filteredRes)
        //      onlineFanart.Remove(onlineFanart[j]);
        //  }
        //}

        // Inform skin message how many fanarts are online
        loadingWorker.ReportProgress(onlineFanart.Count < 100 ? onlineFanart.Count : 100);

        // let's get all the ones we have available locally (from online)
        int i = 0;
        foreach (MFFanart f in onlineFanart)
        {
          if (f.isAvailableLocally)
          {
            if (f.Disabled)
              item = new GUIListItem("Translation.FanartDisableLabel");
            else
              item = new GUIListItem("Translation.FanArtLocal");
            item.IsRemote = false;

            if (f.Chosen)
              item.IsRemote = true;
            else
              item.IsDownloading = false;
          }
          else
          {
            item = new GUIListItem("Translation.FanArtOnline");
            item.IsRemote = false;
            item.IsDownloading = true;
          }
          string filename = f.FullLocalPath;
          filename = filename.Replace("/", @"\");
          string fullURL = f.RemotePath;

          bool bDownloadSuccess = true;
          //int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, "Path.fanart", filename);
          //while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID))
          //{
          //  if (loadingWorker.CancellationPending)
          //  {
          //    // ZF: Cancel, clean up pending download
          //    bDownloadSuccess = false;
          //    // Online_Parsing_Classes.OnlineAPI.CancelFileDownload(nDownloadGUID);
          //    LogMyFilms.Debug("Cancelling fanart thumbnail download: " + filename);
          //  }
          //  System.Windows.Forms.Application.DoEvents();
          //}

          // ZF: should be downloaded now
          filename = Helper.PathCombine("Path.fanart", filename);
          if (bDownloadSuccess)
          {
            item.IconImage = item.IconImageBig = ImageAllocator.GetOtherImage(filename, new System.Drawing.Size(0, 0), false);
          }
          item.TVTag = f;

          // Subscribe to Item Selected Event
          item.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);

          // This will need to be tweaked for more than 100 fanarts
          loadingWorker.ReportProgress((i < 100 ? ++i : 100), item);

          if (loadingWorker.CancellationPending)
            return;
        }
      }
    }

    void setFanartPreviewBackground(MFFanart fanart)
    {
      string fanartInfo = fanart.isAvailableLocally ? "Translation.FanArtLocal" : "Translation.FanArtOnline";
      fanartInfo += Environment.NewLine;

      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartResolution", fanart.ImageResolutionClass);
      fanartInfo += "Resolution: " + fanart.ImageResolutionClass + Environment.NewLine;
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsChosen", fanart.Chosen.ToString());
      fanartInfo += "Chosen: " + fanart.Chosen + Environment.NewLine;
      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartIsDisabled", fanart.Disabled.ToString());
      fanartInfo += "Disabled: " + fanart.Disabled + Environment.NewLine;

      MyFilmsDetail.setGUIProperty("FanArt.SelectedFanartInfo", fanartInfo);

      string preview = string.Empty;

      lock (locker)
      {
        if (fanart.isAvailableLocally)
        {
          // Ensure Fanart on Disk is valid as well
          if (ImageAllocator.LoadImageFastFromFile(fanart.FullLocalPath) == null)
          {
            LogMyFilms.Debug("Fanart is invalid, deleting...");
            fanart.Delete();
            fanart.Chosen = false;
            m_Facade.SelectedListItem.Label = "Translation.FanArtOnline";
          }

          // Should be safe to assign fullsize fanart if available
          preview = fanart.isAvailableLocally ?
                    ImageAllocator.GetOtherImage(fanart.FullLocalPath, default(System.Drawing.Size), false) :
                    m_Facade.SelectedListItem.IconImageBig;
        }
        else
          preview = m_Facade.SelectedListItem.IconImageBig;

        MyFilmsDetail.setGUIProperty("FanArt.SelectedPreview", preview);
      }
    }

  }

  class MFFanart
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public MFFanart() { }

    public void Delete(List<string> titles)
    {
      try
      {
        File.Delete(LocalPath);
        LogMyFilms.Debug("Artwork Deleted: " + LocalPath);
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Failed to delete file: " + LocalPath + " (" + ex.Message + ")");
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

    public void Delete()
    {
      // first let's delete the physical file
      if (this.isAvailableLocally)
      {
        try
        {
          File.Delete(FullLocalPath);
          LogMyFilms.Debug("Fanart Deleted: " + FullLocalPath);
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("Failed to delete file: " + FullLocalPath + " (" + ex.Message + ")");
        }
      }
    }
   


    public string FileName { get; set; }

    public string LocalPath { get; set; }

    public string RemotePath { get; set; }

    public long ImageSize { get; set; }

    public string ImageSizeFriendly { get; set; }

    public int ImageWith { get; set; }

    public int ImageHeight { get; set; }

    public string ImageResolution { get; set; }

    public string ImageResolutionClass { get; set; }

    public bool Chosen { get; set; }

    public bool isAvailableLocally
    {
      get
      {
        if (String.IsNullOrEmpty(this.LocalPath)) return false;

        // Check if file in path exists, remove it from database if not
        if (File.Exists(this.LocalPath)) return true;
        return false;
      }
    }

    public bool Disabled
    {
      get
      {
        // Todo: use file attribute "hidden" ta handle "disabled" status for fanart files... might do this when initially read files from media and set Disabled property there (faster)
        // bool isHidden = ((File.GetAttributes(this.LocalPath) & FileAttributes.Hidden) == FileAttributes.Hidden);
        if (this.Disabled) return true;
        return false;
      }
      set
      {
        this.Disabled = value;
        if (value)
          File.SetAttributes(this.LocalPath, File.GetAttributes(this.LocalPath) | FileAttributes.Hidden); // set (add) hidden attribute (hide a file)
        else
          File.SetAttributes(this.LocalPath, File.GetAttributes(this.LocalPath) & ~FileAttributes.Hidden); // delete/clear hidden attribute      
      }
    }

    public string FullLocalPath
    {
      get
      {
        if (String.IsNullOrEmpty(this.LocalPath)) return string.Empty;
        return Helper.PathCombine(MyFilms.conf.StrPathFanart, this.FileName);
      }
    }

    public static void ClearAll()
    {
      cache.Clear();
    }

    static Dictionary<int, List<MFFanart>> cache = new Dictionary<int, List<MFFanart>>();

    public static List<MFFanart> GetAll(int SeriesID, bool availableOnly)
    {
      lock (cache)
      {
        if (SeriesID < 0) return new List<MFFanart>();

        if (cache == null || !cache.ContainsKey(SeriesID))
        {
          try
          {
            // make sure the table is created - create a dummy object
            MFFanart dummy = new MFFanart();

            // retrieve all fields in the table
            // ToDo: Get TMDB fanarts here ...
            var movies = new List<TmdbMovie>();
            var movie = new TmdbMovie();
            var results = movie.backdrop_path;
            if (results.Length > 0)
            {
              var ourFanart = new List<MFFanart>(results.Length);

              for (int index = 0; index < results.Length; index++)
              {
                ourFanart.Add(new MFFanart());
                // ourFanart[index].Read(ref results, index);
              }
              if (cache == null) cache = new Dictionary<int, List<MFFanart>>();
              cache.Add(SeriesID, ourFanart);
            }
            LogMyFilms.Debug("Found " + results.Length + " Fanart from TMDB");

          }
          catch (Exception ex)
          {
            LogMyFilms.Error("Error in MFFanart.Get (" + ex.Message + ").");
          }
        }
        List<MFFanart> faForSeries = null;
        if (cache != null && cache.TryGetValue(SeriesID, out faForSeries))
          return faForSeries;
        return new List<MFFanart>();
      }
    }

    public List<MFFanart> FanartsToDownload(int SeriesID)
    {
      // Only get a list of fanart that is available for download
      // String sqlQuery = "select * from ";
      // Get Preferred Resolution
      //int res = DBOption.GetOptions(DBOption.cAutoDownloadFanartResolution);

      //if (res == (int)FanartResolution.HD)
      //  sqlQuery += " and " + cResolution + " = " + "\"1280x720\"";
      //if (res == (int)FanartResolution.FULLHD)
      //  sqlQuery += " and " + cResolution + " = " + "\"1920x1080\"";

      //SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);

      //if (results.Rows.Count > 0)
      //{
      //  int iFanartCount = 0;
      //  List<MFFanart> AvailableFanarts = new List<MFFanart>(results.Rows.Count);
      //  for (int index = 0; index < results.Rows.Count; index++)
      //  {
      //    if (results.GetField(index, (int)results.ColumnIndices[cLocalPath]).Length > 0)
      //      iFanartCount++;
      //    else
      //    {
      //      // Add 'Available to Download' fanart to list
      //      AvailableFanarts.Add(new MFFanart());
      //      AvailableFanarts[AvailableFanarts.Count - 1].Read(ref results, index);
      //    }
      //  }

      //  // sort by highest rated
      //  AvailableFanarts.Sort();

      //  // Only return the fanarts that we want to download
      //  int AutoDownloadCount = DBOption.GetOptions(DBOption.cAutoDownloadFanartCount);

      //  for (int i = 0; i < AvailableFanarts.Count; i++)
      //  {
      //    // Dont get more than the user wants
      //    if (iFanartCount >= AutoDownloadCount)
      //      break;
      //    _FanartsToDownload.Add(AvailableFanarts[i]);
      //    iFanartCount++;
      //  }
      //}
      return _FanartsToDownload;

    } List<MFFanart> _FanartsToDownload = new List<MFFanart>();


  }

  class Fanart
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #region Static Vars

    static Dictionary<int, Fanart> fanartsCache = new Dictionary<int, Fanart>();
    static Random fanartRandom = new Random();

    #endregion


    #region Vars

    int _seriesID = -1;

    List<string> _fanArts = null;

    string _randomPick = null;

    MFFanart _dbchosenfanart = null;

    #endregion

    
    
    #region Properties

    public int SeriesID
    {
      get { return _seriesID; }
    }

    public bool Found
    {
      get
      {
        return _fanArts != null && _fanArts.Count > 0;
      }
    }
    #endregion
    
    
    #region Private Constructors

    Fanart(int seriesID)
    {
      _seriesID = seriesID;
      getFanart();
    }
    #endregion

    public static Fanart getFanart(int seriesID)
    {
      Fanart f = null;
      if (fanartsCache.ContainsKey(seriesID))
      {
        f = fanartsCache[seriesID];
        f.ForceNewPick();
      }
      else
      {
        f = new Fanart(seriesID); // this will get simple drop-ins (old method)
        fanartsCache.Add(seriesID, f);
      }
      return f;
    }

    public static bool RefreshFanart(int seriesID)
    {
      Fanart f = null;
      if (fanartsCache.ContainsKey(seriesID))
      {
        f = fanartsCache[seriesID];
        f.getFanart();
        return true;
      }
      else
        return false;
    }

    /// <summary>
    /// Creates a List of Fanarts in your Thumbs folder
    /// </summary>
    void getFanart()
    {
      string fanartFolder = MyFilms.conf.StrPathFanart;

      // Check if Fanart folder exists in MediaPortal's Thumbs directory
      if (Directory.Exists(fanartFolder))
      {
        LogMyFilms.Debug("Checking for Fanart on movie: ", _seriesID.ToString());
        try
        {
          _fanArts = new List<string>();
          // Store list of all fanart files found in all sub-directories of fanart thumbs folder
          _fanArts.AddRange(Directory.GetFiles(fanartFolder, "*.*", SearchOption.AllDirectories));

          // Remove any files that we dont want e.g. thumbnails in the _cache folder
          removeFromFanart("_cache");

          LogMyFilms.Debug("Number of fanart found on disk: ", _fanArts.Count.ToString());
        }
        catch (Exception ex)
        {
          LogMyFilms.Error("An error occured looking for fanart: " + ex.Message);
        }
      }
    }

    void removeFromFanart(string needle)
    {
      if (_fanArts == null) return;
      for (int i = 0; i < _fanArts.Count; i++)
      {
        if (_fanArts[i].Contains(needle))
        {
          _fanArts.Remove(_fanArts[i]);
          i--;
        }
      }
    }

    public void ForceNewPick()
    {
      _randomPick = null;
    }

  }

}
