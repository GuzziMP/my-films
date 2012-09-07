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

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System.Globalization;
  using MediaPortal.GUI.Library;
  //using MediaPortal.Dialogs;
  //using Action = MediaPortal.GUI.Library.Action;
  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;


  /// <summary>
  /// 
  /// </summary>
  public class MyFilmsDialogSetRating : MediaPortal.Dialogs.GUIDialogWindow
  {
    public decimal Rating { get; set; }
    public string FileName { get; set; }
    public ResultCode Result { get; private set; }
    
    public enum ResultCode
    {
      Close,
      Next,
      Previous,
      Cancel
    };

    [SkinControl(2)]
    protected GUIFadeLabel LblHeading = null;
    [SkinControlAttribute(4)]
    protected GUILabelControl LblName = null;
    [SkinControlAttribute(5)]
    protected GUILabelControl LblRating = null;
    [SkinControlAttribute(10)]
    protected GUIButtonControl BtnPlus = null;
    [SkinControlAttribute(11)]
    protected GUIButtonControl BtnMin = null;
    [SkinControlAttribute(12)]
    protected GUIButtonControl BtnOk = null;
    [SkinControlAttribute(100)]
    protected GUIImageList ImgStar = null;

    public MyFilmsDialogSetRating()
    {
      Rating = 1;
      GetID = MyFilms.ID_MyFilmsDialogRating;
    }

    public override int GetID
    {
      get { return MyFilms.ID_MyFilmsDialogRating; }
    }

    //public override int GetID
    //{
    //  get { return ID_MyFilmsDetail; }
    //  set { base.GetID = value; }
    //}

    /// <summary>
    /// MediaPortal will set #currentmodule with GetModuleName()
    /// </summary>
    /// <returns>Localized Window Name</returns>
    public override string GetModuleName()
    {
      return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsDialogRating); // return localized string for Module ID
    }

    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\MyFilmsDialogRating.xml");
    }

    protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    {
      base.OnClicked(controlId, control, actionType);
      if (control == this.BtnOk)
      {
        PageDestroy();
        this.Result = ResultCode.Close;
        return;
      }
      if (control == this.BtnMin)
      {
        if (this.Rating >= (decimal)0.1)
          this.Rating = this.Rating - (decimal)0.1;
        UpdateRating();
        return;
      }
      if (control == this.BtnPlus)
      {
        if (this.Rating < 10)
          this.Rating = this.Rating + (decimal)0.1;
        UpdateRating();
        return;
      }
    }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          {
            this.Result = ResultCode.Close;
            base.OnMessage(message);
            UpdateRating();
          }
          return true;
        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          {
            this.Result = ResultCode.Cancel;
            base.OnMessage(message);
          }
          return true;
      }

      return base.OnMessage(message);
    }

    public void SetHeading(string strLine)
    {
      LoadSkin();
      AllocResources();
      InitControls();

      this.LblHeading.Label = strLine;
    }

    public void SetHeading(int iString)
    {
      SetHeading(iString == 0 ? string.Empty : GUILocalizeStrings.Get(iString));
    }

    public void SetTitle(string title)
    {
      LoadSkin();
      AllocResources();
      InitControls();
      //lblName.Label = title;
    }

    void UpdateRating()
    {
      MyFilmsDetail.setGUIProperty("rating", Rating.ToString());
    }
  }
}
