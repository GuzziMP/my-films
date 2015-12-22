using System.Collections.Generic;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using GUILocalizeStrings = MyFilmsPlugin.Utils.GUILocalizeStrings;

namespace MyFilmsPlugin.MyFilmsGUI
{
  public class GUIDialogImageSelect : GUIDialogSelect2
  {
    #region Skin attributes
    [SkinControl(3)]
    public GUIListControl SelectionList = null;

    [SkinControl(10)]
    protected GUIButtonControl btnOK = null;

    [SkinControl(11)]
    protected GUIButtonControl btnCancel = null;
    #endregion

    #region Attributes
    public ModalResult DialogModalResult = ModalResult.None;
    protected bool m_bRunning = true;
    public List<GUIListItem> ListItems = new List<GUIListItem>();
    #endregion

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    private enum Controls
    {
      CONTROL_BACKGROUND = 1,
      CONTROL_NUMBEROFFILES = 2,
      CONTROL_LIST = 3,
      CONTROL_HEADING = 4,
      CONTROL_BUTTON = 5,
      CONTROL_BACKGROUNDDLG = 6
    } ;

    #region Overrides
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\MyFilmsDialogImageSelect.xml");
    }

    public override int GetID
    {
      get
      {
        return 2009; // this is original GUIDialogSelect2
      }
    }

    public override string GetModuleName()
    {
      return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsDialogImageSelect);
    }

    protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    {
      if (control == btnOK)
      {
        DialogModalResult = ModalResult.OK;
        Close();
      }

      if (control == btnCancel)
      {
        DialogModalResult = ModalResult.Cancel;
        Close();
      }

      if (control == SelectionList && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM)
      {
        if (SelectionList.SelectedListItem != null)
        {
          SelectionList.SelectedListItem.Selected = !SelectionList.SelectedListItem.Selected;
          DialogModalResult = ModalResult.OK;
          Close();
        }
      }

      base.OnClicked(controlId, control, actionType);
    }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
          {
            int iControl = message.SenderControlId;
            if ((int)Controls.CONTROL_LIST == iControl)
            {
              if (SelectionList.SelectedListItem != null)
              {
                LogMyFilms.Debug("OnItemSelected - set '#myfilms.selectedthumb' to '" + (SelectionList.SelectedListItem.ThumbnailImage ?? "") + "'");
                MyFilmsDetail.setGUIProperty("dialogimageselect.selectedlabel", SelectionList.SelectedListItem.Label);
                MyFilmsDetail.setGUIProperty("dialogimageselect.selectedthumb", SelectionList.SelectedListItem.ThumbnailImage);
              }
            }
          }
          break;
      }
      return base.OnMessage(message);
    }

    public new void DoModal(int dwParentId)
    {
      m_bRunning = true;
      DialogModalResult = ModalResult.None;
      base.DoModal(dwParentId);
    }

    public new void Reset()
    {
      ListItems.Clear();
      base.Reset();
    }

    public new void Add(string strLabel)
    {
      int iItemIndex = ListItems.Count + 1;
      GUIListItem pItem = new GUIListItem {ItemId = iItemIndex};
      ListItems.Add(pItem);

      base.Add(strLabel);
    }

    public new void Add(GUIListItem pItem)
    {
      ListItems.Add(pItem);
      base.Add(pItem);
    }

    #endregion

    #region Virtual methods

    protected virtual void Close()
    {
      if (m_bRunning == false) return;
      m_bRunning = false;
      GUIWindowManager.IsSwitchingToNewWindow = true;
      lock (this)
      {
        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, GetID, 0, 0, 0, 0, null);
        base.OnMessage(msg);

        GUIWindowManager.UnRoute();
        m_bRunning = false;
      }
      GUIWindowManager.IsSwitchingToNewWindow = false;
    }
    #endregion
  }
}