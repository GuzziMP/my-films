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
using System.Data;
using System.Linq;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MyFilmsPlugin.CatalogConverter;
using MyFilmsPlugin.DataBase;
using MyFilmsPlugin.Utils;
using GUILocalizeStrings = MyFilmsPlugin.Utils.GUILocalizeStrings;

namespace MyFilmsPlugin.MyFilmsGUI
{
  using GUILocalizeStrings = GUILocalizeStrings;

  public class Configuration
  {
    static Configuration()
    {
      CurrentConfig = string.Empty;
      PluginMode = string.Empty;
      NbConfig = int.MinValue;
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public enum CatalogType
    {
      AntMovieCatalog3 = 0,
      DVDProfiler = 1,
      MovieCollector = 2,
      MyMovies = 3,
      EaxMovieCatalog2 = 4,
      EaxMovieCatalog3 = 5,
      PersonalVideoDatabase = 6,
      eXtremeMovieManager = 7,
      XBMC = 8,
      MovingPicturesXML = 9,
      AntMovieCatalog4Xtended = 10,
      XBMCnfoReader = 11,
      MediaBrowser3 = 12
    }

    private bool IsExternalCatalog(CatalogType type)
    {
      return StrFileType != CatalogType.AntMovieCatalog3 && StrFileType != CatalogType.AntMovieCatalog4Xtended;
    }

    public Configuration(string currentConfig, bool setcurrentconfig, bool createTemp, LoadParameterInfo loadParams)
    {
      #region init variables
      CustomViews = new MFview();
      CurrentView = string.Empty;
      DbSelection = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
      ListSeparator = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
      RoleSeparator = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
      StrGlobalFilterString = string.Empty;
      WStrSortSensCount = string.Empty;
      WStrSortSens = string.Empty;
      TitleDelim = string.Empty;
      StrPlayedRow = null;
      StrSuppressPlayed = string.Empty;
      MovieList = new List<string[]>();
      TrailerList = new List<string[]>();
      MyFilmsPlaybackActive = false;
      StrArtist = false;
      BoolMenuShowAll = false;
      IsDbReloadRequired = false;
      BoolSortCountinViews = false;
      BoolDontSplitValuesInViews = false;
      BoolSkipViewState = false;
      MenuSelectedId = -1;
      BoolEnableOnlineServices = false;
      #endregion

      #region Load Config Parameters in MyFilms.xml file (section currentConfig)

      //LogMyFilms.Debug("MFC: Configuration loading started for '" + CurrentConfig + "'"); 
      //if (setcurrentconfig)
      //{
      //  XmlConfig xmlConfigSave = new XmlConfig();
      //  xmlConfigSave.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", CurrentConfig);
      //  // the xmlwriter caused late update on the file when leaving MP, thus overwriting MyFilms.xml and moving changes to MyFilms.bak !!! -> We write directly only!
      //  //using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      //  //{
      //  //  xmlwriter.SetValue("MyFilms", "Current_Config", CurrentConfig);
      //  //  xmlwriter.Dispose();
      //  //}
      //  //using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      //}
      using (XmlSettings xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)) // true = cached !
      {
        if (setcurrentconfig)
        {
          xmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", currentConfig);
          // XmlSettings.SaveCache(); // ToDo: Debug, if it is required here !
        }
        #region read xml data
        AlwaysShowConfigMenu = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false);
        StrStorage = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntStorage", string.Empty);
        StrDirStor = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "PathStorage", string.Empty);
        StrStorageTrailer = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntStorageTrailer", string.Empty);
        StrDirStorTrailer = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "PathStorageTrailer", string.Empty);
        StrDirStorActorThumbs = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "PathStorageActorThumbs", string.Empty);
        SearchFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SearchFileName", "False");
        SearchFileTrailer = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SearchFileNameTrailer", false);
        ItemSearchFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ItemSearchFileName", string.Empty);
        ItemSearchGrabber = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ItemSearchGrabberName", string.Empty);
        ItemSearchGrabberScriptsFilter = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ItemSearchGrabberScriptsFilter", string.Empty);
        GrabberOverrideLanguage = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GrabberOverrideLanguage", string.Empty);
        GrabberOverridePersonLimit = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GrabberOverridePersonLimit", string.Empty);
        GrabberOverrideTitleLimit = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GrabberOverrideTitleLimit", string.Empty);
        GrabberOverrideGetRoles = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GrabberOverrideGetRoles", string.Empty);
        PictureHandling = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "PictureHandling", string.Empty);
        ItemSearchFileTrailer = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ItemSearchFileNameTrailer", string.Empty);
        SearchSubDirs = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SearchSubDirs", false);
        SearchOnlyExactMatches = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SearchOnlyExactMatches", false);
        AutoRegisterTrailer = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "TrailerAutoregister", false);
        CacheOnlineTrailer = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "CacheOnlineTrailer", false);
        

        CheckWatched = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "CheckWatched", false);
        CheckWatchedPlayerStopped = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "CheckWatchedPlayerStopped", false);
        StrIdentItem = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntIdentItem", string.Empty);
        StrTitle1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntTitle1", string.Empty);
        StrTitle2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntTitle2", string.Empty);
        StrSTitle = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntSTitle", string.Empty);
        if (StrSTitle == string.Empty) StrSTitle = StrTitle1;

        StrViewDfltItem = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewDfltItem", string.Empty);
        if (loadParams != null && !string.IsNullOrEmpty(loadParams.View)) StrViewDfltItem = loadParams.View;

        StrViewDfltText = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewDfltText", string.Empty);
        if (loadParams != null && !string.IsNullOrEmpty(loadParams.ViewValue)) StrViewDfltText = loadParams.ViewValue;

        CustomViews.View.Clear();
        int iCustomViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntViewTotalCount", -1);

        #region New Customviews not yet present - add them ...
        if (iCustomViews == -1)
        {
          MFview.ViewRow newRow = CustomViews.View.NewViewRow();

          //Films (mastertitle)
          newRow.DBfield = StrTitle1;
          newRow.Label = GUILocalizeStrings.Get(342); // videos
          newRow.Value = "*";
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Films.jpg";
          CustomViews.View.Rows.Add(newRow);
          //year
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = "Year";
          newRow.SortDirectionView = " DESC";
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Year.jpg";
          CustomViews.View.Rows.Add(newRow);
          //Category
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = "Category";
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Category.jpg";
          CustomViews.View.Rows.Add(newRow);
          //Country
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = "Country";
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Country.jpg";
          CustomViews.View.Rows.Add(newRow);
          //RecentlyAdded
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = "RecentlyAdded";
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\RecentlyAdded.jpg";
          CustomViews.View.Rows.Add(newRow);
          //Indexed title view
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = StrTitle1;
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.Index = 1;
          newRow.Value = "*";
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\TitlesIndex.jpg";
          CustomViews.View.Rows.Add(newRow);
          //Box Sets view (mastertitle)
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = StrTitle1;
          newRow.Label = "Box Sets";
          newRow.Value = "*";
          newRow.Filter = @"(" + StrTitle1 + @" like '*\*') ";
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Films.jpg";
          CustomViews.View.Rows.Add(newRow);
          //Actors
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = "Actors";
          newRow.Index = 1;
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\PersonsIndex.jpg";
          CustomViews.View.Rows.Add(newRow);
          //Producer
          newRow = CustomViews.View.NewViewRow();
          newRow.DBfield = "Producer";
          newRow.Label = BaseMesFilms.TranslateColumn(newRow.DBfield);
          newRow.ImagePath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + @"\Persons.jpg";
          CustomViews.View.Rows.Add(newRow);
          iCustomViews = 5; // to load "old Custom Views"
        }
        #endregion

        #region load customviews from config
        int index = 1;
        for (int i = 1; i < iCustomViews + 1; i++)
        {
          if (string.IsNullOrEmpty(xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntViewText" + index, string.Empty)))
            break;
          MFview.ViewRow view = CustomViews.View.NewViewRow();
          view.Label = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewText{0}", index), string.Empty);
          // view.Label2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewLabel2{0}", index), string.Empty); // cached counts, if available
          view.ViewEnabled = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewEnabled{0}", index), true);
          view.ImagePath = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewImagePath{0}", index), string.Empty);
          view.DBfield = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewItem{0}", index), string.Empty);
          view.Value = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewValue{0}", index), string.Empty);
          view.Filter = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewFilter{0}", index), string.Empty);
          view.Index = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewIndex{0}", index), 0);
          view.SortFieldViewType = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewSortFieldViewType{0}", index), "Name");
          view.SortDirectionView = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewSortDirectionView{0}", index), " ASC");
          view.SortDirectionView = view.SortDirectionView.Contains("ASC") ? " ASC" : " DESC";
          view.LayoutView = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, string.Format("AntViewLayoutView{0}", index), "0");
          // LogMyFilms.Debug("Adding view - #: '" + index + "', DBitem: '" + view.DBfield + "', View Label: '" + view.Label + "'");
          if (view.DBfield.Length > 0) CustomViews.View.AddViewRow(view);
          index++;
        }
        #endregion

        StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalog", string.Empty);
        StrFileXmlTemp = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
        StrFileType = (CatalogType)int.Parse(xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "CatalogType", "0"));
        StrPathImg = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntPicture", string.Empty);
        StrArtistDflt = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ArtistDflt", false);
        StrPathFanart = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "FanartPicture", string.Empty);
        StrPathViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewsPicture", string.Empty);
        StrPathArtist = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ArtistPicturePath", string.Empty);

        #region user defined display items
        Stritem1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItem1", string.Empty);
        Stritem2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItem2", string.Empty);
        Stritem3 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItem3", string.Empty);
        Stritem4 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItem4", string.Empty);
        Stritem5 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItem5", string.Empty);
        Strlabel1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabel1", string.Empty);
        Strlabel2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabel2", string.Empty);
        Strlabel3 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabel3", string.Empty);
        Strlabel4 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabel4", string.Empty);
        Strlabel5 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabel5", string.Empty);

        StritemDetails1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItemDetails1", "Category");
        StritemDetails2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItemDetails2", "Country");
        StritemDetails3 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItemDetails3", "Director");
        StritemDetails4 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItemDetails4", "Producer");
        StritemDetails5 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItemDetails5", "Languages");
        StritemDetails6 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntItemDetails6", "Date");
        StrlabelDetails1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabelDetails1", BaseMesFilms.TranslateColumn(StritemDetails1));
        StrlabelDetails2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabelDetails2", BaseMesFilms.TranslateColumn(StritemDetails2));
        StrlabelDetails3 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabelDetails3", BaseMesFilms.TranslateColumn(StritemDetails3));
        StrlabelDetails4 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabelDetails4", BaseMesFilms.TranslateColumn(StritemDetails4));
        StrlabelDetails5 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabelDetails5", BaseMesFilms.TranslateColumn(StritemDetails5));
        StrlabelDetails6 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntLabelDetails6", BaseMesFilms.TranslateColumn(StritemDetails6));
        #endregion

        StrIdentLabel = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntIdentLabel", string.Empty);
        StrLogos = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Logos", false);
        StrSuppressAutomatic = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Suppress", false);
        StrSuppressManual = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SuppressManual", false);
        StrSuppressPlayStopUpdateUserField = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SuppressPlayed", false);
        StrSuppressAutomaticActionType = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SuppressType", string.Empty);
        StrWatchedField = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WatchedField", "Checked"); // Defaults to "Checked", if no value set, as it's most used in ANT like that
        StrSuppressField = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SuppressField", string.Empty);
        StrSuppressValue = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SuppressValue", string.Empty);

        EnhancedWatchedStatusHandling = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "EnhancedWatchedStatusHandling", false) || StrFileType == CatalogType.AntMovieCatalog4Xtended;  // always force MUS/EWS on AMC4 Catalog types
        StrUserProfileName = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "UserProfileName", MyFilms.DefaultUsername); // MyFilms.DefaultUsername

        StrECoptionStoreTaglineInDescription = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionStoreTaglineInDescription", false);
        #region Common EC options
        bool addTagline = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddTagline", false);
        bool addTags = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddTags", false);
        bool addCertification = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddCertification", false);
        bool addWriter = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddWriter", false);
        string destinationTagline = "";
        string destinationTags = "";
        string destinationCertification = "";
        string destinationWriter = "";

        if (IsExternalCatalog(StrFileType))
        {
          destinationTagline = addTagline ? xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddDestinationTagline", "") : "";
          destinationTags = addTags ? xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddDestinationTags", "") : "";
          destinationCertification = addCertification ? xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddDestinationCertification", "") : "";
          destinationWriter = addWriter ? xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ECoptionAddDestinationWriter", "") : "";
        }
        // LogMyFilms.Debug("MFC: switch (StrFileType) '" + StrFileType.ToString() + "'");
        #endregion

        ReadOnly = IsExternalCatalog(StrFileType);

        #region Catalog Specific Settings ...
        switch (StrFileType)
        {
          case CatalogType.AntMovieCatalog3:
          case CatalogType.AntMovieCatalog4Xtended:
            break;
          case CatalogType.DVDProfiler:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              string tagField = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "DVDPTagField", string.Empty);
              DvdProfiler cv = new DvdProfiler(tagField);
              StrFileXml = cv.ConvertProfiler(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, tagField, onlyFile);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.MovieCollector:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              MovieCollector mc = new MovieCollector();
              StrFileXml = mc.ConvertMovieCollector(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile, TitleDelim);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.MyMovies:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              MyMovies mm = new MyMovies();
              StrFileXml = mm.ConvertMyMovies(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.EaxMovieCatalog2:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              EaxMovieCatalog emc = new EaxMovieCatalog();
              StrFileXml = emc.ConvertEaxMovieCatalog(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile, TitleDelim);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.EaxMovieCatalog3:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              EaxMovieCatalog3 emc3 = new EaxMovieCatalog3();
              StrFileXml = emc3.ConvertEaxMovieCatalog3(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile, TitleDelim);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.PersonalVideoDatabase:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              PersonalVideoDatabase pvd = new PersonalVideoDatabase();
              StrFileXml = pvd.ConvertPersonalVideoDatabase(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile, TitleDelim, StrECoptionStoreTaglineInDescription);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.eXtremeMovieManager:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              XMM xmm = new XMM();
              StrFileXml = xmm.ConvertXMM(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.XBMC: // XBMC fulldb export (all movies in one DB)
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              XbmcNfo nfo = new XbmcNfo();
              StrFileXml = nfo.ConvertXbmcNfo(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, StrStorage, onlyFile, TitleDelim);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.MovingPicturesXML:
            if (createTemp)
            {
              string wStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
              string destFile = wStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
              if (System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml)))
              {
                StrFileXml = destFile;
                break;
              }
              bool onlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
              MovingPicturesXML mopi = new MovingPicturesXML();
              StrFileXml = mopi.ConvertMovingPicturesXML(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter, onlyFile);
            }
            else
              StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
            break;
          case CatalogType.XBMCnfoReader:
            break;
          //case CatalogType.MediaBrowser3:
          //  if (createTemp)
          //  {
          //    string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
          //    string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
          //    if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
          //    {
          //      StrFileXml = destFile;
          //      break;
          //    }
          //    bool OnlyFile = false;
          //    OnlyFile = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyFile", false);
          //    var mb3 = new Mediabrowser();
          //    StrFileXml = mb3.ConvertMediabrowser(StrFileXml, StrPathImg, destinationTagline, destinationTags, destinationCertification, destinationWriter);
          //  }
          //  else
          //    StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", string.Empty);
          //  break;

        }
        #endregion

        StrSelect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrSelect", string.Empty);
        StrPersons = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrPersons", string.Empty);
        StrTitleSelect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrTitleSelect", string.Empty);
        StrFilmSelect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrFilmSelect", string.Empty);
        StrDfltSelect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrDfltSelect", string.Empty);
        StrViewSelect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrViewSelect", string.Empty);

        #region experimental Viewmanager settings
        //StartView.InitDefaults();
        //CurrentView.InitDefaults();

        ////ViewManager.CurrentSettings.FilmsSortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSort", string.Empty);
        //CurrentView.FilmsSortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSort", string.Empty);
        //CurrentView.FilmsSortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSens", string.Empty);

        //CurrentView.HierarchySortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortInHierarchies", string.Empty);
        //CurrentView.HierarchySortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSensInHierarchies", string.Empty);

        //StartView.FilmsSortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSort", string.Empty);
        //StartView.FilmsSortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSens", string.Empty);

        //StartView.HierarchySortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortInHierarchies", string.Empty); // InHierarchies
        //StartView.HierarchySortItemFriendlyName = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSensInHierarchies", string.Empty);

        //string startview = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentView", string.Empty);
        //if (!string.IsNullOrEmpty(startview)) StartView.LoadFromString(startview);
        #endregion

        ViewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewContext", Enum.GetName(typeof(MyFilms.ViewContext), MyFilms.ViewContext.None)));
        StrTxtView = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "View", string.Empty);
        StrTxtSelect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Selection", string.Empty);
        try { StrIndex = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "IndexItem", -1); }
        catch { StrIndex = -1; }
        StrTIndex = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "TitleItem", string.Empty);
        Boolselect = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Boolselect", false);
        Boolreturn = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Boolreturn", false);
        Boolindexed = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Boolindexed", false);
        Boolindexedreturn = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Boolindexedreturn", false);
        IndexedChars = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "IndexedChars", 0);
        BoolReverseNames = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ReversePersonNames", false);
        BoolVirtualPathBrowsing = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "VirtualPathBrowsing", false);
        BoolAskForPlaybackQuality = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AskForPlaybackQuality", false);
        WStrSort = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WStrSort", string.Empty);
        Wselectedlabel = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WSelectedLabel", string.Empty);
        Wstar = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Wstar", string.Empty);
        LastID = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "LastID", -1);
        TitleDelim = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "TitleDelim", ".");
        DefaultCover = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "DefaultCover", string.Empty);
        DefaultCoverArtist = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "DefaultCoverArtist", string.Empty);
        DefaultCoverViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "DefaultCoverViews", string.Empty);
        DefaultFanartImage = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "DefaultFanartImage", string.Empty);
        StrAntFilterMinRating = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntFilterMinRating", "5");
        StrGrabber_cnf = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Grabber_cnf", string.Empty);
        StrPicturePrefix = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "PicturePrefix", string.Empty);
        StrGrabber_Always = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Grabber_Always", false);
        StrGrabber_ChooseScript = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Grabber_ChooseScript", false);
        StrAMCUpd = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AMCUpd", false);
        AMCUscanOnStartup = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AMCUscanOnStartup", false);
        AMCUwatchScanFolders = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AMCUwatchScanFolders", false);
        AMCUscanStartDelay = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AMCUscanStartDelay", 60);

        StrAMCUpd_cnf = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AMCUpd_cnf", string.Empty);
        StrFanart = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Fanart", false);
        StrFanartDefaultViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "FanartDefaultViews", false);
        StrFanartDefaultViewsUseRandom = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "FanartDefaultViewsUseRandom", false);
        StrFanartDflt = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "FanartDflt", false);
        StrFanartDfltImage = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "FanartDfltImage", false);
        StrFanartDfltImageAll = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "FanartDfltImageAll", false);
        UseThumbsForViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Views", false);
        UseThumbsForPersons = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "Persons", false);
        PersonsEnableDownloads = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "PersonsEnableDownloads", false);
        StrViewsDflt = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewsDflt", false);
        StrViewsDfltAll = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewsDfltAll", false);
        StrViewsShowIndexedImgInIndViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ViewsShowIndexedImages", false);

        #region WOL settings
        StrCheckWOLenable = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WOL-Enable", false);
        StrWOLtimeout = Convert.ToInt16(xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WOL-Timeout", "15"));
        StrCheckWOLuserdialog = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WOL-Userdialog", false);
        StrNasName1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "NAS-Name-1", string.Empty);
        StrNasMAC1 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "NAS-MAC-1", string.Empty);
        StrNasName2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "NAS-Name-2", string.Empty);
        StrNasMAC2 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "NAS-MAC-2", string.Empty);
        StrNasName3 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "NAS-Name-3", string.Empty);
        StrNasMAC3 = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "NAS-MAC-3", string.Empty);
        #endregion

        #region external player
        ExternalPlayerPath = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ExternalPlayerPath", string.Empty);
        ExternalPlayerStartParams = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ExternalPlayerStartParams", string.Empty);
        ExternalPlayerExtensions = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ExternalPlayerExtensions", string.Empty);
        #endregion

        #region read states vars for each possible view - do we also need states for "userdefined views"? (disabled)
        //DataBase.AntMovieCatalog ds = new DataBase.AntMovieCatalog();
        //MyFilms.ViewStateCache.Clear();
        //foreach (DataColumn dc in ds.Movie.Columns)
        //{
        //  ViewState tView = new ViewState();
        //  string viewstateName = "ViewState-" + dc.ColumnName;
        //  string viewstate = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, viewstateName, string.Empty);
        //  if (!string.IsNullOrEmpty(viewstate))
        //  {
        //    tView.LoadFromString(viewstate);
        //    MyFilms.ViewStateCache.Add(dc.ColumnName, tView);
        //  }
        //  //else
        //  //{
        //  //  tView.ViewDBItem = dc.ColumnName;
        //  //  CustomView.Add(tView);
        //  //}
        //}
        #endregion

        #region search history
        StrSearchHistory = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "SearchHistory", string.Empty);
        MyFilms.SearchHistory.Clear();
        foreach (string s in StrSearchHistory.Split('|').Where(s => !string.IsNullOrEmpty(s.Trim())))
        {
          MyFilms.SearchHistory.Add(s);
        }
        #endregion

        #region list and role separators
        int j = 0;
        for (int i = 1; i <= 5; i++)
        {
          if (xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ListSeparator" + i, string.Empty).Length > 0)
          {
            ListSeparator[j] = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ListSeparator" + i, string.Empty).Replace("#LF#", "\n"); // support for new line separation
            j++;
          }
        }
        j = 0;
        for (int i = 1; i <= 5; i++)
        {
          if (xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "RoleSeparator" + i, string.Empty).Length > 0)
          {
            RoleSeparator[j] = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "RoleSeparator" + i, string.Empty);
            j++;
          }
        }
        #endregion

        CmdPar = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "CmdPar", string.Empty);
        if (CmdPar == "(none)") CmdPar = "";
        OnlyTitleList = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "OnlyTitleList", false);
        GlobalAvailableOnly = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GlobalAvailableOnly", false);
        GlobalUnwatchedOnly = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GlobalUnwatchedOnly", false);
        GlobalUnwatchedOnlyValue = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "GlobalUnwatchedOnlyValue", "false");
        ScanMediaOnStart = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "CheckMediaOnStart", false);
        BoolShowEmptyValuesInViews = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "ShowEmpty", false);
        AllowTraktSync = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AllowTraktSync", false);
        AllowRecentlyAddedApi = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AllowRecentAddedAPI", false);

        #region layout settings
        StrLayOut = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "LayOut", 0);
        StrLayOutInHierarchies = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "LayOutInHierarchies", StrLayOut);
        WStrLayOut = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "WLayOut", StrLayOut);
        int iDfltLayOut = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntDfltLayOut", 0);
        int iDfltLayOutInHierarchies = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntDfltLayOutInHierarchies", 0);

        if (loadParams != null && !string.IsNullOrEmpty(loadParams.Layout))
        {
          #region override layout with start params, if available
          int iLayout = 0;
          if (int.TryParse(loadParams.Layout, out iLayout))
            if (iLayout >= 0 && iLayout <= 4)
            {
              StrLayOut = iLayout;
              WStrLayOut = iLayout;
              StrLayOutInHierarchies = iLayout;
            }
          #endregion
        }
        #endregion

        #region Sort Settings
        StrSorta = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrSort", string.Empty);
        StrSortSens = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrSortSens", string.Empty);
        StrSortaInHierarchies = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrSortInHierarchies", string.Empty);
        StrSortSensInHierarchies = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "StrSortSensInHierarchies", string.Empty);
        string wDfltSort = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntDfltStrSort", string.Empty);
        string wDfltSortSens = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntDfltStrSortSens", string.Empty);

        string wDfltSortInHierarchies = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntDfltStrSortInHierarchies", string.Empty); // InHierarchies
        string wDfltSortSensInHierarchies = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AntDfltStrSortSensInHierarchies", string.Empty);
        #endregion

        AlwaysDefaultView = xmlConfig.ReadXmlConfig("MyFilms", currentConfig, "AlwaysDefaultView", false);
        
        if (AlwaysDefaultView || MyFilms.InitialStart || (loadParams != null && (!string.IsNullOrEmpty(loadParams.View) || !string.IsNullOrEmpty(loadParams.MovieId))))
        {
          #region overrides for "AlwaysDefaultView"
          ViewContext = MyFilms.ViewContext.StartView;
          StrIndex = -1;
          LastID = -1;
          Wstar = "";
          Boolreturn = false;
          Boolselect = true;
          Boolindexed = false;
          Boolindexedreturn = false;

          Wselectedlabel = StrViewDfltText;
          if (loadParams == null || string.IsNullOrEmpty(loadParams.Layout))
          {
            StrLayOut = iDfltLayOut;
            StrLayOutInHierarchies = iDfltLayOutInHierarchies;
            WStrLayOut = iDfltLayOut; // also preset views layout to default - although it most probably will be overridden by views setting
          }
          if (Helper.FieldIsSet(wDfltSort))
          {
            StrSorta = wDfltSort;
            StrSortSens = wDfltSortSens;
          }
          if (Helper.FieldIsSet(wDfltSortInHierarchies)) // hierarchies sort settings
          {
            StrSortaInHierarchies = wDfltSortInHierarchies;
            StrSortSensInHierarchies = wDfltSortSensInHierarchies;
          }
          #endregion
        }
        xmlConfig.Dispose();
        #endregion
      }
      #endregion

      #region check and correct settings
      // if (string.IsNullOrEmpty(StrSelect)) StrSelect = StrTitle1 + " not like ''";

      if (string.IsNullOrEmpty(StrSorta)) StrSorta = StrSTitle;
      if (string.IsNullOrEmpty(StrSortaInHierarchies)) StrSortaInHierarchies = StrSTitle;

      // ToDo: Remove Migration Code
      if (StrSorta == "DateAdded") StrSorta = "Date";
      if (StrSortaInHierarchies == "DateAdded") StrSortaInHierarchies = "Date";

      if (string.IsNullOrEmpty(StrSortSens) || StrSortSens == "ASC") StrSortSens = " ASC";
      if (StrSortSens == "DESC") StrSortSens = " DESC";

      if (string.IsNullOrEmpty(StrSortSensInHierarchies) || StrSortSensInHierarchies == "ASC") StrSortSensInHierarchies = " ASC";
      if (StrSortSensInHierarchies == "DESC") StrSortSensInHierarchies = " DESC";

      if (string.IsNullOrEmpty(WStrSortSens) || WStrSortSens == "ASC") WStrSortSens = " ASC";
      if (WStrSortSens == "DESC") WStrSortSens = " DESC";

      if (string.IsNullOrEmpty(WStrSortSensCount) || WStrSortSensCount == "DESC") WStrSortSensCount = " DESC";
      if (WStrSortSensCount == "ASC") WStrSortSensCount = " ASC";

      if (StrFanart)
        if (!(StrPathFanart.Length > 0 && System.IO.Directory.Exists(StrPathFanart)))
        {
          LogMyFilms.Info("MyFilms : Fanart Path '" + StrPathFanart + "', doesn't exist. Fanart disabled ! ");
          StrFanart = false;
        }
      if (StrArtist)
        if (!(StrPathArtist.Length > 0 && System.IO.Directory.Exists(StrPathArtist)))
        {
          LogMyFilms.Info("MyFilms : Artist Path '" + StrPathArtist + "', doesn't exist. Artist Pictures disabled ! ");
          StrArtist = false;
        }
      #endregion
      // LogMyFilms.Debug("MFC: Configuration loading ended for '" + CurrentConfig + "'");
    }

    #region Getter/Setter - static values


    public static string CurrentConfig { get; set; }
    public string CurrentView { get; set; }
    public static string PluginMode { get; set; }
    public static int NbConfig { get; set; }
    public string[] DbSelection { get; set; }
    public bool AlwaysShowConfigMenu { get; set; }
    public MyFilms.ViewContext ViewContext { get; set; }
    public int MenuSelectedId { get; set; }
    public bool BoolMenuShowAll { get; set; }
    public bool Boolselect { get; set; }
    public bool Boolreturn { get; set; }
    public bool BoolSkipViewState { get; set; }
    public bool Boolindexed { get; set; }
    public int IndexedChars { get; set; }
    public bool Boolindexedreturn { get; set; }
    public bool BoolCollection { get; set; }
    public bool BoolVirtualPathBrowsing { get; set; }
    public bool AlwaysDefaultView { get; set; }
    public bool GlobalAvailableOnly { get; set; }
    public bool GlobalUnwatchedOnly { get; set; }
    public string GlobalUnwatchedOnlyValue { get; set; }
    public bool ScanMediaOnStart { get; set; }
    public bool AllowTraktSync { get; set; }
    public bool AllowRecentlyAddedApi { get; set; }
    public bool OnlyTitleList { get; set; }
    public bool StrLogos { get; set; }
    public bool StrSuppressAutomatic { get; set; }
    public bool StrSuppressManual { get; set; }
    public bool EnhancedWatchedStatusHandling { get; set; }
    public string StrUserProfileName { get; set; }
    public bool StrECoptionStoreTaglineInDescription { get; set; }
    public bool StrSuppressPlayStopUpdateUserField { get; set; }
    public string StrSelect { get; set; }
    public string StrDirStor { get; set; }
    public string StrDirStorTrailer { get; set; }
    public string StrDirStorActorThumbs { get; set; }
    public string StrIdentLabel { get; set; }
    public string Strlabel1 { get; set; }
    public string Strlabel2 { get; set; }
    public string Strlabel3 { get; set; }
    public string Strlabel4 { get; set; }
    public string Strlabel5 { get; set; }
    public string Stritem1 { get; set; }
    public string Stritem2 { get; set; }
    public string Stritem3 { get; set; }
    public string Stritem4 { get; set; }
    public string Stritem5 { get; set; }
    public string StrlabelDetails1 { get; set; }
    public string StrlabelDetails2 { get; set; }
    public string StrlabelDetails3 { get; set; }
    public string StrlabelDetails4 { get; set; }
    public string StrlabelDetails5 { get; set; }
    public string StrlabelDetails6 { get; set; }
    public string StritemDetails1 { get; set; }
    public string StritemDetails2 { get; set; }
    public string StritemDetails3 { get; set; }
    public string StritemDetails4 { get; set; }
    public string StritemDetails5 { get; set; }
    public string StritemDetails6 { get; set; }
    public string[] ListSeparator { get; set; }
    public string[] RoleSeparator { get; set; }
    public string StrGlobalFilterString { get; set; }
    public string StrDfltSelect { get; set; }
    public string StrViewSelect { get; set; }
    public string StrTitle1 { get; set; }
    public string StrTitle2 { get; set; }
    public string StrSTitle { get; set; }
    public string StrTitleSelect { get; set; }
    public string StrFilmSelect { get; set; }
    public string StrSorta { get; set; }
    public string StrSortaInHierarchies { get; set; }
    public bool BoolSortCountinViews { get; set; }
    public bool BoolShowEmptyValuesInViews { get; set; }
    public bool BoolDontSplitValuesInViews { get; set; }
    public bool BoolReverseNames { get; set; }
    public bool BoolAskForPlaybackQuality { get; set; }
    public string StrPersons { get; set; }
    public string StrTxtView { get; set; }
    public string StrTxtSelect { get; set; }
    public string StrStorage { get; set; }
    public string StrStorageTrailer { get; set; }
    public string SearchFile { get; set; }
    public bool SearchFileTrailer { get; set; }
    public bool AutoRegisterTrailer { get; set; }
    public bool CacheOnlineTrailer { get; set; }
    public string ItemSearchFile { get; set; }
    public string ItemSearchGrabber { get; set; }
    public string ItemSearchGrabberScriptsFilter { get; set; }
    public string GrabberOverrideLanguage { get; set; }
    public string GrabberOverridePersonLimit { get; set; }
    public string GrabberOverrideGetRoles { get; set; }
    public string GrabberOverrideTitleLimit { get; set; }
    public string PictureHandling { get; set; }
    public string ItemSearchFileTrailer { get; set; }
    public bool SearchSubDirs { get; set; }
    public bool SearchOnlyExactMatches { get; set; }
    public bool CheckWatched { get; set; }
    public bool CheckWatchedPlayerStopped { get; set; }
    public string StrIdentItem { get; set; }
    public MFview CustomViews { get; set; }
    public string StrViewDfltItem { get; set; }
    public string StrViewDfltText { get; set; }
    public string StrFileXml { get; set; }
    public string StrFileXmlTemp { get; set; }
    public CatalogType StrFileType { get; set; }
    public bool IsDbReloadRequired { get; set; }
    public bool ReadOnly { get; set; }
    public string StrPathImg { get; set; }
    public string StrPathFanart { get; set; }
    public string StrPathViews { get; set; }
    public string StrPathArtist { get; set; }
    public string StrSortSens { get; set; }
    public string StrSortSensInHierarchies { get; set; }
    public string WStrSortSensCount { get; set; }
    public string WStrSort { get; set; }
    public string WStrSortSens { get; set; }
    public string Wselectedlabel { get; set; }
    public string StrTIndex { get; set; }
    public string TitleDelim { get; set; }
    public string Wstar { get; set; }
    public string DefaultCover { get; set; }
    public string DefaultCoverArtist { get; set; }
    public string DefaultCoverViews { get; set; }
    public string DefaultFanartImage { get; set; }
    public string CmdExe { get; set; }
    public string CmdPar { get; set; }
    public int StrLayOut { get; set; }
    public int WStrLayOut { get; set; }
    public int StrLayOutInHierarchies { get; set; }
    public int LastID { get; set; }
    public int StrIndex { get; set; }
    public DataRow StrPlayedRow { get; set; }
    public bool StrFanart { get; set; }
    public bool StrFanartDefaultViews { get; set; }
    public bool StrFanartDefaultViewsUseRandom { get; set; }
    public bool StrArtist { get; set; }
    public bool StrFanartDflt { get; set; }
    public bool StrFanartDfltImage { get; set; }
    public bool StrFanartDfltImageAll { get; set; }
    public bool StrArtistDflt { get; set; }
    public bool UseThumbsForViews { get; set; }
    public bool UseThumbsForPersons { get; set; }
    public bool PersonsEnableDownloads { get; set; }
    public bool StrViewsDflt { get; set; }
    public bool StrViewsDfltAll { get; set; }
    public bool StrViewsShowIndexedImgInIndViews { get; set; }
    public bool StrCheckWOLenable { get; set; }
    public int StrWOLtimeout { get; set; }
    public bool StrCheckWOLuserdialog { get; set; }
    public string StrNasName1 { get; set; }
    public string StrNasMAC1 { get; set; }
    public string StrNasName2 { get; set; }
    public string StrNasMAC2 { get; set; }
    public string StrNasName3 { get; set; }
    public string StrNasMAC3 { get; set; }
    public string ExternalPlayerPath { get; set; }
    public string ExternalPlayerStartParams { get; set; }
    public string ExternalPlayerExtensions { get; set; }
    public string StrSearchHistory { get; set; }
    public string StrGrabber_cnf { get; set; }
    public string StrPicturePrefix { get; set; }
    public string StrAntFilterMinRating { get; set; }
    public bool StrGrabber_Always { get; set; }
    public bool StrGrabber_ChooseScript { get; set; }
    public bool StrAMCUpd { get; set; }
    public bool AMCUscanOnStartup { get; set; }
    public bool AMCUwatchScanFolders { get; set; }
    public int AMCUscanStartDelay { get; set; }
    public string StrAMCUpd_cnf { get; set; }
    public string StrSuppressPlayed { get; set; }
    public string StrSuppressAutomaticActionType { get; set; }
    public string StrSuppressField { get; set; }
    public string StrSuppressValue { get; set; }
    public string StrWatchedField { get; set; }
    public List<string[]> MovieList { get; set; }
    public List<string[]> TrailerList { get; set; }
    public bool MyFilmsPlaybackActive { get; set; }

    public bool BoolEnableOnlineServices { get; set; }

    #endregion

    public static void SaveConfiguration(string currentConfig, int selectedItem, string selectedItemLabel)
    {
      LogMyFilms.Debug("MFC: Configuration saving started for '" + currentConfig + "'");
      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        #region save xml data
        //XmlConfig XmlConfig = new XmlConfig();
        xmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", currentConfig);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSelect", MyFilms.conf.StrSelect);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrPersons", MyFilms.conf.StrPersons);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrTitleSelect", MyFilms.conf.StrTitleSelect);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrFilmSelect", MyFilms.conf.StrFilmSelect);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrViewSelect", MyFilms.conf.StrViewSelect); // Custom View filter
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrDfltSelect", MyFilms.conf.StrDfltSelect);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSort", MyFilms.conf.StrSorta);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortSens", MyFilms.conf.StrSortSens);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortInHierarchies", MyFilms.conf.StrSortaInHierarchies); //InHierarchies
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortSensInHierarchies", MyFilms.conf.StrSortSensInHierarchies);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "ViewContext", Enum.GetName(typeof(MyFilms.ViewContext), MyFilms.conf.ViewContext));
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "View", MyFilms.conf.StrTxtView);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Selection", MyFilms.conf.StrTxtSelect);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "IndexItem", selectedItem > -1 ? selectedItem.ToString() : "-1"); //may need to check if there is no item selected and so save -1
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "TitleItem", selectedItem > -1 ? (MyFilms.conf.Boolselect ? selectedItem.ToString() : selectedItemLabel) : string.Empty); //may need to check if there is no item selected and so save ""

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolselect", MyFilms.conf.Boolselect);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolreturn", MyFilms.conf.Boolreturn);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolindexed", MyFilms.conf.Boolindexed);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolindexedreturn", MyFilms.conf.Boolindexedreturn);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "IndexedChars", MyFilms.conf.IndexedChars);
        // XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "ReversePersonNames", MyFilms.conf.BoolReverseNames); // removed, to make it NonPersistant
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "VirtualPathBrowsing", MyFilms.conf.BoolVirtualPathBrowsing);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "AskForPlaybackQuality", MyFilms.conf.BoolAskForPlaybackQuality);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WSelectedLabel", MyFilms.conf.Wselectedlabel);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WStrSort", MyFilms.conf.WStrSort);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Wstar", MyFilms.conf.Wstar);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LayOut", MyFilms.conf.StrLayOut);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LayOutInHierarchies", MyFilms.conf.StrLayOutInHierarchies);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WLayOut", MyFilms.conf.WStrLayOut);

        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LastID", MyFilms.conf.LastID);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "SearchHistory", MyFilms.conf.StrSearchHistory);
        xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "UserProfileName", MyFilms.conf.StrUserProfileName);

        //foreach (KeyValuePair<string, ViewState> viewState in MyFilms.ViewStateCache)
        //{
        //  XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "ViewState-" + viewState.Key, viewState.Value.SaveToString());
        //}

        //foreach (View viewstate in MyFilms.conf.CustomView.View)
        //{
        //  string viewstateName = "ViewState-" + viewstate.ViewDBItem;
        //  XmlConfig.WriteXmlConfig("MyFilms", currentConfig, viewstateName, MyFilms.conf.CurrentView.SaveToString());
        //}

        //XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "CurrentView", MyFilms.conf.CurrentView.SaveToString());

        switch (MyFilms.conf.StrFileType)
        {
          case CatalogType.DVDProfiler:
            xmlConfig.WriteXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", MyFilms.conf.StrFileXml);
            break;
        }
        #endregion
        // XmlConfig.Dispose();
        // XmlSettings.SaveCache();
      }
      LogMyFilms.Debug("MFC: Configuration saving ended for '" + currentConfig + "'");
    }
    //--------------------------------------------------------------------------------------------
    //  Control Acces to asked configuration
    //--------------------------------------------------------------------------------------------
    public static string ControlAccessConfig(string configname, int getId)
    {
      if (configname.Length == 0)
        return "";

      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        string dwp = xmlConfig.ReadXmlConfig("MyFilms", configname, "Dwp", string.Empty);
        if (dwp.Length == 0)
        {
          return configname;
        }
        var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
        if (null == keyboard)
        {
          return string.Empty;
        }
        keyboard.Reset();
        keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
        keyboard.Text = string.Empty;
        keyboard.Password = true;
        keyboard.DoModal(getId);
        if (keyboard.IsConfirmed && (keyboard.Text.Length > 0))
        {
          var crypto = new Crypto();
          if (crypto.Decrypter(dwp) == keyboard.Text) return configname;
        }
        return string.Empty;
      }
    }

    //--------------------------------------------------------------------------------------------
    //  Choice Configuration
    //--------------------------------------------------------------------------------------------
    public static string ChoiceConfig(int getId)
    {
      var dlg = (MediaPortal.Dialogs.GUIDialogMenu)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null)
      {

        MyFilms.conf.StrFileXml = string.Empty;
        return string.Empty;
      }
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(6022)); // Choose MyFilms DB Config
      using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        //XmlConfig XmlConfig = new XmlConfig();
        int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
        for (int i = 0; i < mesFilmsNbConfig; i++) dlg.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

        dlg.DoModal(getId);
        if (dlg.SelectedLabel == -1)
        {
          try
          {
            MyFilms.conf.StrFileXml = string.Empty;
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("MF: Error resetting config, as not yet loaded into memory - exception: " + ex);
          }
          return string.Empty;
        }
        if (dlg.SelectedLabelText.Length > 0)
        {
          string catalog = xmlConfig.ReadXmlConfig("MyFilms", dlg.SelectedLabelText, "AntCatalog", string.Empty);
          if (!System.IO.File.Exists(catalog))
          {
            GUIUtils.ShowOKDialog("Cannot load Configuration:", "'" + dlg.SelectedLabelText + "'", "Verify your settings !", "");
            return string.Empty;
          }
          else
          {
            return dlg.SelectedLabelText;
          }
        }
      }
      return string.Empty;
    }
    //--------------------------------------------------------------------------------------------
    //  Return Current Configuration
    //--------------------------------------------------------------------------------------------
    public static bool Current_Config(bool showmenu) // returns true, if a default config is set (requires full reload on entering plugin)
    {
      CurrentConfig = null;
      bool isDefaultConfig = false;

      using (XmlSettings xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        NbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", 0);
        PluginMode = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PluginMode", "normal");
        // Reads Plugin start mode and sets to normal if not present
        LogMyFilms.Info("MyFilms ********** OperationsMode (PluginMode): '" + PluginMode + "' **********");
        if (NbConfig == 0)
        {
          GUIUtils.ShowOKDialog(GUILocalizeStrings.Get(10799601), GUILocalizeStrings.Get(10799602), GUILocalizeStrings.Get(10799603), ""); // No configuration found ! // please enter setup first // to create a MyFilms config.
          return false;

          // return NewConfigWizard();
        }

        bool boolchoice = true;
        if (CurrentConfig == null)
          CurrentConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", string.Empty).Length > 0 ? xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", string.Empty) : string.Empty;

        if (!xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false) &&
            (xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty).Length > 0))
        {
          CurrentConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
          isDefaultConfig = true;
        }
        if (showmenu)
        {
          if (CurrentConfig == string.Empty || xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", true))
          {
            boolchoice = false;
            CurrentConfig = ChoiceConfig(MyFilms.ID_MyFilms);
            // "" => user esc's dialog on plugin startup so exit plugin unchanged
          }
          CurrentConfig = ControlAccessConfig(CurrentConfig, MyFilms.ID_MyFilms);
          if ((CurrentConfig == "") && (NbConfig > 1) && boolchoice) //error password ? so if many config => choice config menu
            CurrentConfig = ChoiceConfig(MyFilms.ID_MyFilms);
        }
      }
      return isDefaultConfig;
    }

    private static bool NewConfigWizard()
    {
      bool result = GUIUtils.ShowYesNoDialog("New Config Wizard", "Do you want to create a new MF config?");
      if (!result) return false;
      // GUIUtils.ShowTextDialog("textheading", "söhgösdhgkösdgh hsudgö uögusioö ghisuöhg suioöhioöushgiosh<i öghsdöuiogh isöu<ghuiöh suiögh ösusöh uiö<h gusi<gh suiöhg  uiö<hö<");

      //string scandirectory = "";
      //GUIUtils.GetDirectoryDialog(ref scandirectory);

      try
      {
        System.IO.FileStream fs = System.IO.File.Create(Config.GetFile(Config.Dir.Config, "MyFilms.xml"));
        fs.Close();
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("CreateNewMyFilmsConfigFile - error creating file: : '" + ex.Message + "'");
        return false;
      }

      const string newConfigName = "Default";
      Configuration tmpconf = new Configuration(newConfigName, true, true, null);
      CurrentConfig = newConfigName;
      MyFilms.Load_Config(newConfigName, true, null);
      SaveConfiguration(CurrentConfig, -1, string.Empty);


      GUIDialogMenu mnu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      mnu.Reset();
      mnu.SetHeading("Select drive");

      foreach (Helper.Drive d in Helper.GetDrivesList())
      {
        mnu.Add(d.Path + " " + d.TypeDescription);
      }

      mnu.DoModal(GUIWindowManager.ActiveWindow);

      int i = 0;
      foreach (Helper.Drive d in Helper.GetDrivesList())
      {
        if (i == mnu.SelectedLabel)
        {
          string currentDrive = d.Path;
        }

        i++;
      }

      // Todo: add dialog to select path for movie source, set config accordingly, etc.
      return true;
    }

  }

}
