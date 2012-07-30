﻿#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *	Copyright (C) 2005-2008 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

//using Grabber.IMDB;
//using MediaPortal.Video.Database;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI

{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Data;

  using MediaPortal.Configuration;
  using MediaPortal.Dialogs;
  using MediaPortal.GUI.Library;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin.MyFilms;

  using NLog;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  /// <summary>
    /// Opens a separate page to display Actor Infos
    /// </summary>
    public class MyFilmsActors : GUIWindow
    {
        #region Skin ID descriptions

        enum Controls : int
        {
            CTRL_BtnReturn = 10402,
            CTRL_Fanart = 1000,
            CTRL_FanartDir = 1001,
            CTRL_MovieThumbs = 10201,
            CTRL_MovieThumbsDir = 10202,

            CTRL_BtnSrtBy = 2,
            CTRL_BtnViewAs = 3,
            CTRL_BtnSearchT = 4,
            CTRL_BtnOptions = 5,
            CTRL_BtnLayout = 6,
            CTRL_TxtSelect = 10412,
            //CTRL_TxtSelect = 12,
            CTRL_Fanart1 = 11,
            CTRL_Fanart2 = 21,
            CTRL_Image = 1020,
            CTRL_Image2 = 1021,
            CTRL_List = 10401,
            // ID 3004 aus MyFilms für Wait Symbol (Setvisilility)
        }

        [SkinControl(10101)]
        protected GUIButtonControl CTRL_TxtSelect;
        [SkinControl(10102)]
        protected GUISortButtonControl CTRL_BtnReturn;

        [SkinControlAttribute((int)Controls.CTRL_FanartDir)]
        protected GUIMultiImage ImgFanartDir;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbs)]
        //protected GUIImage ImgMovieThumbs = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbsDir)]
        //protected GUIMultiImage ImgMovieThumbsDir = null;

        [SkinControlAttribute((int)Controls.CTRL_BtnSrtBy)]
        protected GUISortButtonControl BtnSrtBy;

        [SkinControlAttribute((int)Controls.CTRL_List)]
        protected GUIFacadeControl facadeView;

        [SkinControlAttribute((int)Controls.CTRL_Image)]
        protected GUIImage ImgLstFilm;

        [SkinControlAttribute((int)Controls.CTRL_Image2)]
        protected GUIImage ImgLstFilm2;

        [SkinControlAttribute((int)Controls.CTRL_Fanart1)]
        protected GUIImage ImgFanart1;

        [SkinControlAttribute((int)Controls.CTRL_Fanart2)]
        protected GUIImage ImgFanart2;

        [SkinControlAttribute(3004)]
        protected GUIAnimation m_SearchAnimation;

        #endregion

        private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();  //log
  
        public static string wsearchfile;
        public static int wGetID;

        public int Layout = 0;
        public static int Prev_ItemID = -1;
        public bool Context_Menu = false;
        //public static Configuration conf;
        //public static Logos confLogos;
        //private string currentConfig;
        //private Cornerstone.MP.ImageSwapper backdrop;

        
        private List<string> list;

        enum View : int
        {
            List = 0,
            Icons = 1,
            LargeIcons = 2,
        }


        private enum ViewMode
        {
            Biography,
            Movies,
        }

        #region Base Dialog Variables

        private bool m_bRunning = false;
        private int m_dwParentWindowID = 0;
        private GUIWindow m_pParentWindow = null;

        #endregion

        private IMDBActor currentActor = null;
        private string imdbCoverArtUrl = string.Empty;

        //Pfad für ActorThumbs
        //MyFilms.conf.StrDirStorActorThumbs.ToString()
        //string strDir = MyFilms.conf.StrDirStorActorThumbs;


        public MyFilmsActors()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public override int GetID
        {
            get { return MyFilms.ID_MyFilmsActors; }
            set { base.GetID = value; }
        }

        public override string GetModuleName()
        {
          return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsActors); // return localized string for Modul ID
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\MyFilmsActors.xml");
        }

        public override void PreInit() { }


        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------
        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            LogMyFilms.Debug("MyFilmsActors: OnAction " + actionType.wID);
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) || (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR))
            {
                MyFilms.conf.LastID = MyFilms.ID_MyFilms;
                GUIWindowManager.ShowPreviousWindow();
                return;
            }

            base.OnAction(actionType);
            return;
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
                    base.OnMessage(messageType);
                    wGetID = GetID;
                    MyFilms.conf.LastID = MyFilms.ID_MyFilmsActors;


                    //Temporary set item in facadeview...
                    GUIListItem item = new GUIListItem();
                    Prev_ItemID = -1;
                    ArrayList w_tableau = new ArrayList();

                    BtnSrtBy.Label = GUILocalizeStrings.Get(103);
                    MyFilms.conf.Boolselect = true;
                    MyFilms.conf.Wselectedlabel = string.Empty;
                    Change_LayOut(0);
                    facadeView.Clear();

                    item = new GUIListItem();
                    //item.Label = wchampselect.ToString();
                    //item.Label2 = Wnb_enr.ToString();
                    item.Label = "Arnold Schwarzenegger";
                    item.Label2 = "als Terminator";
                    item.Label3 = "n/a";
                    item.IsFolder = true;
                    facadeView.Add(item);

                    item.FreeMemory();
                    item.Label = "Jim Knopf";
                    item.Label2 = "als Lukas, der Lokomotivführer";
                    item.Label3 = "n/a";
                    item.IsFolder = true;
                    item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                    facadeView.Add(item);
                    //End Testfacadeinfos

                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    if (Configuration.CurrentConfig != "")
                        Configuration.SaveConfiguration(Configuration.CurrentConfig, MyFilms.conf.StrIndex, MyFilms.conf.StrTIndex);
                    using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
                    {
                        string currentmoduleid = "7989";
                        bool currentmodulefullscreen = (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_MUSIC || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_TELETEXT);
                        string currentmodulefullscreenstate = GUIPropertyManager.GetProperty("#currentmodulefullscreenstate");
                        // if MP was closed/hibernated by the use of remote control, we have to retrieve the fullscreen state in an alternative manner.
                        if (!currentmodulefullscreen && currentmodulefullscreenstate == "True")
                            currentmodulefullscreen = true;
                        xmlreader.SetValue("general", "lastactivemodule", currentmoduleid);
                        xmlreader.SetValueAsBool("general", "lastactivemodulefullscreen", currentmodulefullscreen);
                        LogMyFilms.Debug("MyFilms : SaveLastActiveModule - module {0}", currentmoduleid);
                        LogMyFilms.Debug("MyFilms : SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
                    }
                    return true;

                case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
                    //---------------------------------------------------------------------------------------
                    // Set Focus
                    //---------------------------------------------------------------------------------------
                    base.OnMessage(messageType);
                    return true;

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    //---------------------------------------------------------------------------------------
                    // Mouse/Keyboard Clicked
                    //---------------------------------------------------------------------------------------
                    if (iControl == (int)Controls.CTRL_BtnReturn)
                    // Return Previous Menu
                    {
                        MyFilms.conf.LastID = MyFilms.ID_MyFilms;
                        GUITextureManager.CleanupThumbs();
                        GUIWindowManager.ShowPreviousWindow();
                        return true;
                    }

                    if ((iControl == (int)Controls.CTRL_BtnLayout) && !MyFilms.conf.Boolselect)
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
                        // dlg.Add(GUILocalizeStrings.Get(791));//Coverflow - add when Coverflow gets incorporated
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        //conf.StrIndex = 0;
                        int wselectindex = facadeView.SelectedListItemIndex;
                        Change_LayOut(dlg.SelectedLabel);
                        MyFilms.conf.StrLayOut = dlg.SelectedLabel;
                        dlg.DeInit();
                        //GetFilmList();
                        GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wselectindex);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        return base.OnMessage(messageType);
                    }

                    if (iControl == (int)Controls.CTRL_List)
                    {
                        if (facadeView.SelectedListItemIndex > -1)
                        {
                            if (!facadeView.SelectedListItem.IsFolder && !MyFilms.conf.Boolselect)
                            // New Window for detailed selected item information
                            {
                                MyFilms.conf.StrIndex = facadeView.SelectedListItem.ItemId;
                                MyFilms.conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();
                                GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsDetail);
                            }
                            else // View List as selected
                            {
                                MyFilms.conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                                Change_LayOut(MyFilms.conf.StrLayOut);
                                if (facadeView.SelectedListItem.IsFolder)
                                    MyFilms.conf.Boolreturn = false;
                                else
                                    MyFilms.conf.Boolreturn = true;
                                //do
                                //{
                                //    if (MyFilms.conf.StrTitleSelect != "") MyFilms.conf.StrTitleSelect += MyFilms.conf.TitleDelim;
                                //    MyFilms.conf.StrTitleSelect += MyFilms.conf.Wselectedlabel;
                                //} while (GetFilmList() == false); //keep calling while single folders found
                            }
                        }
                    }
                break;
            }
            base.OnMessage(messageType);
            return true;
        }
        
        
        
        #region Base Dialog Members

        public void DoModal(int dwParentId)
        {
            m_dwParentWindowID = dwParentId;
            m_pParentWindow = GUIWindowManager.GetWindow(m_dwParentWindowID);
            if (null == m_pParentWindow)
            {
                m_dwParentWindowID = 0;
                return;
            }

            GUIWindowManager.IsSwitchingToNewWindow = true;
            GUIWindowManager.RouteToWindow(GetID);

            // active this window...
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, GetID, 0, 0, 0, 0, null);
            OnMessage(msg);

            //GUILayerManager.RegisterLayer(this, GUILayerManager.LayerType.Dialog);

            GUIWindowManager.IsSwitchingToNewWindow = false;
            m_bRunning = true;
            while (m_bRunning && GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
            {
                GUIWindowManager.Process();
            }
            //GUILayerManager.UnRegisterLayer(this);
        }

        #endregion

        protected override void OnPageLoad()
        {
            list = new List<string>();

            base.OnPageLoad();
            Update();
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            if (m_bRunning)
            {
                // User probably pressed H (SWITCH_HOME)
                m_bRunning = false;
                GUIWindowManager.UnRoute();
                m_pParentWindow = null;
            }

            base.OnPageDestroy(newWindowId);
            currentActor = null;
        }


        public IMDBActor Actor
        {
            get { return currentActor; }
            set { currentActor = value; }
        }

        private void Update()
        {
            if (currentActor == null)
            {
                return;
            }

        }

        private int GetSelectedItemNo()
        {
            return facadeView.SelectedListItemIndex;
        }

        private GUIListItem GetSelectedItem()
        {
            return facadeView.SelectedListItem;
        }


        //--------------------------------------------------------------------------------------------
        //   Change LayOut 
        //--------------------------------------------------------------------------------------------
        private void Change_LayOut(int wLayOut)
        {
          switch (wLayOut)
            {
                case 1:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(100));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                    break;
                case 2:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(417));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                    break;
                case 3:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(733));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
                    break;
                case 4:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(791));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
                    break;
                default:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101));
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.List;
                    break;
            }
        }

        private void item_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            GUIFilmstripControl filmstrip = parent as GUIFilmstripControl;
            if (filmstrip != null)
                filmstrip.InfoImageFileName = item.ThumbnailImage;
            if (!(MyFilms.conf.Boolselect || (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder))) //xxxx
            {
                if (facadeView.SelectedListItemIndex > -1)
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, true, facadeView.SelectedListItem.Label);
            }
            else
            {
                if (facadeView.SelectedListItemIndex > -1 && !MyFilms.conf.Boolselect)
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                else
                {
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                    GUIControl.ShowControl(GetID, 34);
                    //affichage_rating(0);
                }
            }
            //affichage_Lstdetail(item.ItemId, true, item.Label);
        }

        private static void affichage_Lstdetail(int ItemId, bool wrep, string wlabel)//wrep = false display only image
        {
            return;
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


        private void personinfo(string wperson, int actorID)
            {
                ArrayList actorList = new ArrayList();
                // Search with searchName parameter which contain wanted actor name, result(s) is in array
                // which conatin id and name separated with char "|"
                //MediaPortal.Video.Database.VideoDatabase.GetActorByName(wperson, actorList);
                MyFilmsDetail.GetActorByName(wperson, actorList);
                // Check result

                if (actorList.Count == 0)
                {
                    LogMyFilms.Debug("MyFilms (Person Info): No ActorIDs found for '" + wperson + "'");
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK); 
                    dlgOk.SetHeading("Info");
                    dlgOk.SetLine(1, "");
                    dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
                    dlgOk.DoModal(GetID);
                    return;
                }
                LogMyFilms.Debug("MyFilms (Person Info): " + actorList.Count + " ActorID(s) found for '" + wperson + "'");
                //int actorID;
                actorID = 0;
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
                    string actorname = strActor[1];
                    LogMyFilms.Debug("MyFilms (ActorDetails - Person Info): ActorID: '" + actorID + "' with ActorName: '" + actorname + "' found found for '" + wperson + "'");
                }
                
                MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
                //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
                //if (actor != null)

                //OnVideoArtistInfoGuzzi(actor); // hier nicht erreichbar, Ersatz notwendig !!!
                return;
            }


    }
}