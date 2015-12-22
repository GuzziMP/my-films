using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Video.Database;
using GUILocalizeStrings = MyFilmsPlugin.Utils.GUILocalizeStrings;

namespace MyFilmsPlugin.MyFilmsGUI
{
  public static class GUIUtils
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger(); //log

    private delegate bool ShowCustomYesNoDialogDelegate(string heading, string lines, string yesLabel, string noLabel, bool defaultYes, int timeout);
    private delegate void ShowOkDialogDelegate(string heading, string lines);
    private delegate void ShowNotifyDialogDelegate(string heading, string text, string image, string buttonText, int timeOut);
    private delegate int ShowMenuDialogDelegate(string heading, List<GUIListItem> items);
    private delegate List<MultiSelectionItem> ShowMultiSelectionDialogDelegate(string heading, List<MultiSelectionItem> items);
    private delegate void ShowTextDialogDelegate(string heading, string text);
    private delegate string ShowRateDialogDelegate<T>(T rateObject);
    private delegate bool GetStringFromKeyboardDelegate(ref string strLine, bool isPassword);
    private delegate bool GetDirectoryDelegate(ref string strLine);

    private static readonly string MyFilmsLogo = GUIGraphicsContext.Skin + "\\Media\\MyFilms\\myfilms.png";

    public static string PluginName()
    {
      return "MyFilms";
    }

    #region wrapper for MP1.2-MP1.3 compatibility in VideoDatabase

    public static bool GetActorDetails(IMDB imdb, object imdbActorUrl, object director, out IMDBActor imdbActor)
    {
      // MP1.3 _imdb.GetActorDetails(_imdb[0], out imdbActor);
      // MP1.2 _imdb.GetActorDetails(_imdb[0], false, out person);
      imdbActor = new IMDBActor();
      MethodInfo mi = imdb.GetType().GetMethod("GetActorDetails");
      var iParams = mi.GetParameters();
      switch (iParams.Length)
      {
        case 2:
          {
            //var paramTypes = new Type[] { typeof(IMDB.IMDBUrl), typeof(IMDBActor).MakeByRefType() }; // var paramTypes = new Type[] { typeof(IMDB.IMDBUrl) Type.GetType("MediaPortal.Video.Database.IMDB.IMDBUrl"), Type.GetType("MediaPortal.Video.Database.IMDBActor&") };
            //MethodInfo mi = imdb.GetType().GetMethod("GetActorDetails", paramTypes);
            object[] arrParms = new object[] { imdbActorUrl, imdbActor };
            bool returnValue = (bool)mi.Invoke(imdb, arrParms); // return (bool)mi.Invoke(imdb, new[] { imdbActorUrl, imdbActor });
            imdbActor = (IMDBActor)arrParms[1];
            return returnValue;
          }
        case 3:
          {
            //var paramTypes = new Type[] { typeof(IMDB.IMDBUrl), typeof(bool), typeof(IMDBActor).MakeByRefType() }; //var paramTypes = new Type[] { Type.GetType("MediaPortal.Video.Database.IMDB.IMDBUrl"), typeof(bool), Type.GetType("MediaPortal.Video.Database.IMDBActor&") };
            //MethodInfo mi = imdb.GetType().GetMethod("GetActorDetails", paramTypes);
            object[] arrParms = new object[] { imdbActorUrl, director, imdbActor };
            bool returnValue = (bool)mi.Invoke(imdb, arrParms);
            imdbActor = (IMDBActor)arrParms[2];
            return returnValue;
          }
      }
      throw new Exception("Error invoking VideoDatabase method 'GetActorDetails'");
    }

    public static void GetFilesForMovie(int id, ref ArrayList movies)
    {
      // MP1.2 VideoDatabase.GetFiles(iidMovie, ref movies);
      // MP1.3 VideoDatabase.GetFilesForMovie(iidMovie, ref movies);

      //System.Type[] arrTypes = new System.Type[2];
      //arrTypes.SetValue(Type.GetType("System.Int32"), 0);
      //arrTypes.SetValue(Type.GetType("System.ArrayList&"), 1);
      //MethodInfo miCusNm = imdb.GetType().GetMethod("GetFiles", arrTypes);

      // package parameters
      object[] arrParms = new object[2];
      arrParms.SetValue(id, 0);
      arrParms.SetValue(movies, 1);
      // bool success = (bool)miCusNm.Invoke(oClsX, arrParms);

      Type[] paramTypes = new Type[] { typeof(int), typeof(ArrayList).MakeByRefType() }; // Type[] paramTypes = new Type[] { typeof(int), Type.GetType("System.ArrayList&") };

      // Get methods.
      MethodInfo[] methods = typeof(VideoDatabase).GetMethods();
      bool success = false;
      foreach (MethodInfo info in methods)
      {
        switch (info.Name)
        {
          case "GetFiles":
            {
              MethodInfo mi = typeof(VideoDatabase).GetMethod("GetFiles", paramTypes);
              mi.Invoke(null, arrParms);
              success = true;
            }
            break;
          case "GetFilesForMovie":
            {
              MethodInfo mi = typeof(VideoDatabase).GetMethod("GetFilesForMovie", paramTypes);
              mi.Invoke(null, arrParms);
              success = true;
            }
            break;
        }
      }
      if (!success)
      {
        throw new Exception("Error invoking GetFiles() form Videodatabase !");
      }
    }

    public static int AddActor(object role, object name)
    {
      // int iiActor = VideoDatabase.AddActor(string.Empty, "");
      var mi = typeof(VideoDatabase).GetMethod("AddActor");
      var iParams = mi.GetParameters();
      switch (iParams.Length)
      {
        case 1:
          return (int)mi.Invoke(null, new[] { name });
        case 2:
          return (int)mi.Invoke(null, new[] { role, name });
        default:
          throw new Exception("Error invoking VideoDatabase method 'AddActor'");
      }
    }

    public static void AddActorToMovie(object iMovieId, object iActorId, object role)
    {
      // VideoDatabase.AddActorToMovie(iidmovie, iiActor, "actor");
      // public static void AddActorToMovie(int lMovieId, int lActorId, string role)
      var mi = typeof(VideoDatabase).GetMethod("AddActorToMovie");
      var iParams = mi.GetParameters();
      switch (iParams.Length)
      {
        case 2:
          mi.Invoke(null, new[] { iMovieId, iActorId });
          break;
        case 3:
          mi.Invoke(null, new[] { iMovieId, iActorId, role });
          break;
        default:
          throw new Exception("Error invoking VideoDatabase method 'AddActorToMovie'");
      }
    }

    #endregion

    #region GUI property setter

    public static void SetProperty(string property, string value, bool log = false)
    {
      // prevent ugly display of property names
      if (string.IsNullOrEmpty(value)) value = " ";

      GUIPropertyManager.SetProperty(property, value);

      if (log)
      {
        if (GUIPropertyManager.Changed) LogMyFilms.Debug("Set property \"" + property + "\" to \"" + value + "\" successful");
        else LogMyFilms.Warn("Set property \"" + property + "\" to \"" + value + "\" failed");
      }
    }
    #endregion

    // sample code from offbyone to launch GUI activities in main thread via message queue
    //public void SetFanartToGUI(string header, string message, string value, bool isImportant)
    //{
    //  GUIWindowManager.SendThreadCallback((p1, p2, o) =>
    //  {
    //    string[] inputData = o as string[];

    //    // code to create a Dialog or do UI stuff goes here
    //    var dlgNotificationDialog = CreateNotificationDialog();
    //    if (dlgNotificationDialog != null)
    //    {
    //      dlgNotificationDialog.SetHeading(inputData[0]);
    //      dlgNotificationDialog.SetText(inputData[2] != null ? inputData[1].Replace("%1", inputData[2]) : inputData[1]);
    //      dlgNotificationDialog.TimeOut = showNotificationTimeout;
    //      dlgNotificationDialog.DoModal(GetActiveWindow());
    //    }
    //    // return an integer (required)
    //    return 0;
    //  }, 0, 0, new string[3] { header, message, value });
    //}

    #region dialogs

    #region YesNoDialogs
    /// <summary>
    /// Displays a yes/no dialog.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowYesNoDialog(string heading, string lines)
    {
      return ShowCustomYesNoDialog(heading, lines, null, null);
    }

    /// <summary>
    /// Displays a yes/no dialog.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowYesNoDialog(string heading, string lines, bool defaultYes)
    {
      return ShowCustomYesNoDialog(heading, lines, null, null, defaultYes);
    }

    /// <summary>
    /// Displays a yes/no dialog with custom labels for the buttons.
    /// This method may become obsolete in the future if media portal adds more dialogs.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowCustomYesNoDialog(string heading, string lines, string yesLabel, string noLabel, bool defaultYes = false, int timeout = 0)
    {
      bool result = false;
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowCustomYesNoDialogDelegate d = ShowCustomYesNoDialog;
        return (bool)GUIGraphicsContext.form.Invoke(d, heading, lines, yesLabel, noLabel, defaultYes, timeout);
      }

      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);

      try
      {
        dlgYesNo.Reset();
        dlgYesNo.SetHeading(heading);
        string[] linesArray = lines.Split(new string[] { "\\n", "\n" }, StringSplitOptions.None);
        if (linesArray.Length == 1)
        {
          dlgYesNo.SetLine(1, string.Empty);
          dlgYesNo.SetLine(2, linesArray[0]);
          dlgYesNo.SetLine(3, string.Empty);
        }
        else
        {
        if (linesArray.Length > 0) dlgYesNo.SetLine(1, linesArray[0]);
        if (linesArray.Length > 1) dlgYesNo.SetLine(2, linesArray[1]);
        if (linesArray.Length > 2) dlgYesNo.SetLine(3, linesArray[2]);
        if (linesArray.Length > 3) dlgYesNo.SetLine(4, linesArray[3]);
        }
        dlgYesNo.SetDefaultToYes(defaultYes);

        foreach (GUIButtonControl btn in dlgYesNo.GetControlList().OfType<GUIButtonControl>())
        {
          if (btn.GetID == 11 && !string.IsNullOrEmpty(yesLabel)) // Yes button
            btn.Label = yesLabel;
          else if (btn.GetID == 10 && !string.IsNullOrEmpty(noLabel)) // No button
            btn.Label = noLabel;
        }
        if (timeout > 0) dlgYesNo.TimeOut = timeout;
        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
        result = dlgYesNo.IsConfirmed;
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("DialogYesNo - error: " + ex.Message);
        LogMyFilms.Debug("DialogYesNo - error: " + ex.StackTrace);
      }
      finally
      {
        // set the standard yes/no dialog back to it's original state (yes/no buttons)
        if (dlgYesNo != null)
        {
          dlgYesNo.ClearAll();
        }
      }
      LogMyFilms.Debug("DialogYesNo returning result = '" + result + "'");
      return result;
    }
    #endregion

    /// <summary>
    /// Same as Children and controlList but used for backwards compatibility between mediaportal 1.1 and 1.2
    /// </summary>
    /// <param name="self"></param>
    /// <returns>IEnumerable of GUIControls</returns>
    public static IEnumerable GetControlList(this GUIWindow self)
    {
      PropertyInfo property = GetPropertyInfo<GUIWindow>("Children", null);
      return (IEnumerable)property.GetValue(self, null);
    }

    private static Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();

    /// <summary>
    /// Gets the property info object for a property using reflection.
    /// The property info object will be cached in memory for later requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newName">The name of the property in 1.2</param>
    /// <param name="oldName">The name of the property in 1.1</param>
    /// <returns>instance PropertyInfo or null if not found</returns>
    public static PropertyInfo GetPropertyInfo<T>(string newName, string oldName)
    {
      PropertyInfo property = null;
      Type type = typeof(T);
      string key = type.FullName + "|" + newName;

      if (!propertyCache.TryGetValue(key, out property))
      {
        property = type.GetProperty(newName) ?? type.GetProperty(oldName);

        propertyCache[key] = property;
      }

      return property;
    }

    #region ShowOKDialog
    /// <summary>
    /// Displays a OK dialog with heading set to default error message and 1 line on position line2.
    /// </summary>
    public static void ShowErrorDialog(string line)
    {
      ShowOKDialog(GUILocalizeStrings.Get(10798624) + " - Error", line);
    }

    /// <summary>
    /// Displays a OK dialog with default heading and 1 line on position line2.
    /// </summary>
    public static void ShowOKDialog(string line)
    {
      ShowOKDialog(string.Empty, string.Empty, line, string.Empty, string.Empty);
    }

    /// <summary>
    /// Displays a OK dialog with default heading and up to 4 lines.
    /// </summary>
    public static void ShowOKDialog(string line1, string line2, string line3, string line4)
    {
      // ShowOKDialog("", string.Concat(line1, line2, line3, line4));
      ShowOKDialog("", (line1 + @"\n" + line2 + @"\n" + line3 + @"\n" + line4));
    }

    /// <summary>
    /// Displays a OK dialog with heading and up to 4 lines.
    /// </summary>
    public static void ShowOKDialog(string heading, string line1, string line2, string line3, string line4)
    {
      // ShowOKDialog(heading, string.Concat(line1, line2, line3, line4));
      ShowOKDialog(heading, (line1 + @"\n" + line2 + @"\n" + line3 + @"\n" + line4));
    }

    /// <summary>
    /// Displays a OK dialog with heading and up to 4 lines split by \n in lines string.
    /// </summary>
    public static void ShowOKDialog(string heading, string lines)
    {
      heading = (string.IsNullOrEmpty(heading) ? GUILocalizeStrings.Get(10798624) : heading); // use MyFilms default heading, if  none set - id="10798624" "MyFilms System Information"

      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowOkDialogDelegate d = ShowOKDialog;
        GUIGraphicsContext.form.Invoke(d, heading, lines);
        return;
      }

      GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);

      dlgOk.Reset();
      dlgOk.SetHeading(heading);

      string[] dialoglines = lines.Split(new string[] { "\\n", "\n" }, StringSplitOptions.None);
      switch (dialoglines.Length)
      {
        case 1:
          dlgOk.SetLine(1, string.Empty);
          dlgOk.SetLine(2, dialoglines[0]);
          dlgOk.SetLine(3, string.Empty);
          dlgOk.SetLine(4, string.Empty);
          break;
        case 2:
          dlgOk.SetLine(1, string.Empty);
          dlgOk.SetLine(2, dialoglines[0]);
          dlgOk.SetLine(3, dialoglines[1]);
          dlgOk.SetLine(4, string.Empty);
          break;
        default:
          {
            int lineid = 1;
            foreach (string line in dialoglines)
            {
              dlgOk.SetLine(lineid, line);
              lineid++;
            }
            for (int i = lineid; i <= 4; i++) dlgOk.SetLine(i, string.Empty);
          }
          break;
      }
      dlgOk.DoModal(GUIWindowManager.ActiveWindow);
    }
    #endregion

    #region ShowNotifyDialog
    /// <summary>
    /// Displays a notification dialog.
    /// heading will be autoset to id="10798624" "MyFilms System Information"
    /// </summary>
    public static void ShowNotifyDialog(string text)
    {
      ShowNotifyDialog(string.Empty, text, MyFilmsLogo, MediaPortal.GUI.Library.GUILocalizeStrings.Get(186), -1); // "Ok" in MP Translation
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text)
    {
      ShowNotifyDialog(heading, text, MyFilmsLogo, MediaPortal.GUI.Library.GUILocalizeStrings.Get(186), -1); // "Ok" in MP Translation
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, int timeOut)
    {
      ShowNotifyDialog(heading, text, MyFilmsLogo, MediaPortal.GUI.Library.GUILocalizeStrings.Get(186), timeOut);
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, string image)
    {
      ShowNotifyDialog(heading, text, image, MediaPortal.GUI.Library.GUILocalizeStrings.Get(186), -1);
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, string image, string buttonText, int timeout)
    {
      image = (string.IsNullOrEmpty(image) ? GetMyFilmsDefaultLogo() : image); // use MyFilms defult image, if  none set
      heading = (string.IsNullOrEmpty(heading) ? GUILocalizeStrings.Get(10798624) : heading); // use MyFilms default heading, if  none set - id="10798624" "MyFilms System Information"

      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowNotifyDialogDelegate d = ShowNotifyDialog;
        GUIGraphicsContext.form.Invoke(d, heading, text, image, buttonText, timeout);
        return;
      }

      var pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
      if (pDlgNotify == null) return;

      try
      {
        pDlgNotify.Reset();
        pDlgNotify.SetHeading(heading);
        pDlgNotify.SetImage(image);
        pDlgNotify.SetText(text);
        if (timeout >= 0) pDlgNotify.TimeOut = timeout;

        foreach (GUIButtonControl btn in pDlgNotify.GetControlList().OfType<GUIButtonControl>().Where(btn => btn.GetID == 4 && !string.IsNullOrEmpty(buttonText) && !string.IsNullOrEmpty(btn.Label)))
        {
          // Only if ID is 4 and we have our custom text and if button already has label (in case the skin "hides" the button by emtying the label)
          btn.Label = buttonText;
        }
        pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
      }
      finally
      {
        if (pDlgNotify != null) pDlgNotify.ClearAll();
      }
    }
    #endregion

    #region ShowMenuDialogs
    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    /// <returns>Selected item index, -1 if exited</returns>
    public static int ShowMenuDialog(string heading, List<GUIListItem> items)
    {
      return ShowMenuDialog(heading, items, -1);
    }

    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    /// <returns>Selected item index, -1 if exited</returns>
    public static int ShowMenuDialog(string heading, List<GUIListItem> items, int selectedItemIndex)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowMenuDialogDelegate d = ShowMenuDialog;
        return (int)GUIGraphicsContext.form.Invoke(d, heading, items);
      }

      GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

      dlgMenu.Reset();
      dlgMenu.SetHeading(heading);

      foreach (GUIListItem item in items)
      {
        dlgMenu.Add(item);
      }

      if (selectedItemIndex >= 0) dlgMenu.SelectedLabel = selectedItemIndex;

      dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

      if (dlgMenu.SelectedLabel < 0)
      {
        return -1;
      }

      return dlgMenu.SelectedLabel;
    }
    #endregion

    #region MultiSelectMenuDialogs
    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    public static List<MultiSelectionItem> ShowMultiSelectionDialog(string heading, List<MultiSelectionItem> items)
    {
      List<MultiSelectionItem> result = new List<MultiSelectionItem>();
      if (items == null) return result;

      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowMultiSelectionDialogDelegate d = ShowMultiSelectionDialog;
        return (List<MultiSelectionItem>)GUIGraphicsContext.form.Invoke(d, heading, items);
      }

      GUIWindow dlgMultiSelectOld = (GUIWindow)GUIWindowManager.GetWindow(2100);
      GUIDialogMultiSelect dlgMultiSelect = new GUIDialogMultiSelect();
      dlgMultiSelect.Init();
      GUIWindowManager.Replace(2100, dlgMultiSelect);

      try
      {
        dlgMultiSelect.Reset();
        dlgMultiSelect.SetHeading(heading);

        foreach (MultiSelectionItem multiSelectionItem in items)
        {
          GUIListItem item = new GUIListItem
          {
            Label = multiSelectionItem.ItemTitle,
            Label2 = multiSelectionItem.ItemTitle2,
            MusicTag = multiSelectionItem.Tag,
            Selected = multiSelectionItem.Selected
          };

          dlgMultiSelect.Add(item);
        }

        dlgMultiSelect.DoModal(GUIWindowManager.ActiveWindow);

        if (dlgMultiSelect.DialogModalResult == ModalResult.OK)
        {
          for (int i = 0; i < items.Count; i++)
          {
            MultiSelectionItem item = items[i];
            MultiSelectionItem newMultiSelectionItem = new MultiSelectionItem
            {
              ItemTitle = item.ItemTitle,
              ItemTitle2 = item.ItemTitle2,
              ItemID = item.ItemID,
              Tag = item.Tag
            };
            try
            {
              newMultiSelectionItem.Selected = dlgMultiSelect.ListItems[i].Selected;
            }
            catch
            {
              newMultiSelectionItem.Selected = item.Selected;
            }

            result.Add(newMultiSelectionItem);
          }
        }
        else return null;

        return result;
      }
      finally
      {
        GUIWindowManager.Replace(2100, dlgMultiSelectOld);
      }
    }
    #endregion

    /// <summary>
    /// Displays a text dialog.
    /// </summary>
    public static void ShowTextDialog(string heading, List<string> text)
    {
      if (text == null || text.Count == 0) return;
      ShowTextDialog(heading, string.Join("\n", text.ToArray()));
    }

    /// <summary>
    /// Displays a text dialog.
    /// </summary>
    public static void ShowTextDialog(string heading, string text)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowTextDialogDelegate d = ShowTextDialog;
        GUIGraphicsContext.form.Invoke(d, heading, text);
        return;
      }

      GUIDialogText dlgText = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);

      dlgText.Reset();
      dlgText.SetHeading(heading);
      dlgText.SetText(text);

      dlgText.DoModal(GUIWindowManager.ActiveWindow);
    }

    public static bool GetStringFromKeyboard(ref string strLine)
    {
      return GetStringFromKeyboard(ref strLine, false);
    }

    /// <summary>
    /// Gets the input from the virtual keyboard window.
    /// </summary>
    public static bool GetStringFromKeyboard(ref string strLine, bool isPassword)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        GetStringFromKeyboardDelegate d = GetStringFromKeyboard;
        object[] args = { strLine, isPassword };
        bool result = (bool)GUIGraphicsContext.form.Invoke(d, args);
        strLine = (string)args[0];
        return result;
      }

      VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (keyboard == null) return false;

      keyboard.Reset();
      keyboard.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
      keyboard.Text = strLine;
      keyboard.Password = isPassword;
      keyboard.DoModal(GUIWindowManager.ActiveWindow);

      if (keyboard.IsConfirmed)
      {
        strLine = keyboard.Text;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Gets the input from the virtual keyboard window.
    /// </summary>
    public static bool GetDirectoryDialog(ref string strLine)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        GetDirectoryDelegate d = GetDirectoryDialog;
        object[] args = { strLine };
        bool result = (bool)GUIGraphicsContext.form.Invoke(d, args);
        strLine = (string)args[0];
        return result;
      }

      GUIDialogFile dialogFile = (GUIDialogFile)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILE);
      if (dialogFile == null) return false;

      dialogFile.Init();
      dialogFile.SetHeading(strLine);
      dialogFile.DoModal(GUIWindowManager.ActiveWindow);

      if (!dialogFile.IsCanceled)
      {
        strLine = dialogFile.GetDestinationDir();
        return true;
      }

      return false;
    }
    #endregion

    private static string GetMyFilmsDefaultLogo()
    {
      // first check subfolder of current skin (allows skinners to use custom icons)
      string image = string.Format(@"{0}\Media\MyFilms\MyFilms.png", GUIGraphicsContext.Skin);
      if (!System.IO.File.Exists(image))
      {
        // use png in thumbs folder
        image = string.Format(@"{0}\MyFilms\DefaultImages\MyFilms.png", Config.GetFolder(Config.Dir.Thumbs));
      }
      return image;
    }

    public static void SetImageToGui(this GUIListItem item, string imageFilePath)
    {
      if (item == null) return;
      if (string.IsNullOrEmpty(imageFilePath)) return;

      string texture = GetTextureFromFile(imageFilePath);

      if (GUITextureManager.LoadFromMemory(MediaPortal.Util.ImageFast.FromFile(imageFilePath), texture, 0, 0, 0) > 0)
      {
        item.ThumbnailImage = texture;
        item.IconImage = texture;
        item.IconImageBig = texture;
      }

      //// if selected and GUIActors is current window force an update of thumbnail
      //GUIActors actorWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow) as GUIActors;
      //if (actorWindow != null)
      //{
      //  GUIListItem selectedItem = GUIControl.GetSelectedListItem(9816, 50);
      //  if (selectedItem == this)
      //  {
      //    GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
      //  }
      //}
    }

    private static string GetTextureFromFile(string filename)
    {
      return "[MyFilms:" + filename.GetHashCode() + "]";
    }

  }


  internal class Gui2UtilConnector
  {

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    # region Singleton
    protected Gui2UtilConnector()
    {
      timeoutTimer.Elapsed += TaskWatcherTimerElapsed;
    }
    protected static Gui2UtilConnector instance = null;
    internal static Gui2UtilConnector Instance
    {
      get { return instance ?? (instance = new Gui2UtilConnector()); }
    }
    #endregion

    internal bool IsBusy { get; private set; }

    Action<bool, object> _CurrentResultHandler = null;
    object _CurrentResult = null;
    bool? _CurrentTaskSuccess = null;
    Exception _CurrentError = null;
    string _CurrentTaskDescription = null;
    Thread backgroundThread = null;
    bool abortedByUser = false;
    System.Timers.Timer timeoutTimer = new System.Timers.Timer(10 * 1000) { AutoReset = false }; // ToDo: get timeout from settings

    public void StopBackgroundTask()
    {
      StopBackgroundTask(true);
    }

    void StopBackgroundTask(bool byUserRequest)
    {
      if (IsBusy && _CurrentTaskSuccess == null && backgroundThread != null && backgroundThread.IsAlive)
      {
        LogMyFilms.Info("Aborting background thread{0}.", byUserRequest ? " by User Request" : "");
        backgroundThread.Abort();
        abortedByUser = byUserRequest;
      }
    }

    void TaskWatcherTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      StopBackgroundTask(false);
    }

    /// <summary>
    /// This method should be used to call methods from siteutils that might take a few seconds.
    /// It makes sure only on thread at a time executes and has a timeout for the execution.
    /// It also catches Exceptions from the utils and writes errors to the log, and show a message on the GUI.
    /// The Wait Cursor will be shown on while executing the task and the resultHandler will be called on the MPMain thread.
    /// </summary>
    /// <param name="task">method to invoke on a background thread</param>
    /// <param name="resultHandler">method to invoke on the GUI Thread with the result of the task</param>
    /// <param name="taskDescription">description of the tak to be invoked - will be shown in the error message if execution fails or times out</param>
    /// <param name="timeout">true: use the timeout, or false: wait forever</param>
    /// <returns>true, if the task could be successfully started in the background</returns>
    internal bool ExecuteInBackgroundAndCallback(Func<object> task, Action<bool, object> resultHandler, string taskDescription, bool timeout)
    {
      if (Thread.CurrentThread.ManagedThreadId != 1)
      {
        LogMyFilms.Error("OnlineVideos not called on the MPMain thread - not executing any background action!");
        return false;
      }

      // make sure only one background task can be executed at a time
      if (!IsBusy && Monitor.TryEnter(this))
      {
        try
        {
          IsBusy = true;
          abortedByUser = false;
          _CurrentResultHandler = resultHandler;
          _CurrentTaskDescription = taskDescription;
          _CurrentResult = null;
          _CurrentError = null;
          _CurrentTaskSuccess = null;// while this is null the task has not finished (or later on timeouted), true indicates successfull completion and false error
          GUIWaitCursor.Init(); GUIWaitCursor.Show(); // init and show the wait cursor in MediaPortal
          backgroundThread = new Thread(delegate()
          {
            try
            {
              _CurrentResult = task.Invoke();
              _CurrentTaskSuccess = true;
            }
            catch (ThreadAbortException)
            {
              if (!abortedByUser) LogMyFilms.Warn("Timeout waiting for results.");
              Thread.ResetAbort();
            }
            catch (Exception threadException)
            {
              _CurrentError = threadException as Exception;
              LogMyFilms.Warn(threadException.ToString());
              _CurrentTaskSuccess = false;
            }
            timeoutTimer.Stop();
            // hide the wait cursor
            GUIWaitCursor.Hide();
            // execute the ResultHandler on the Main Thread
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, o) => { ExecuteTaskResultHandler(); return 0; }, 0, 0, null);
          }) { Name = "OnlineVideos", IsBackground = true };
          // disable timeout when debugging
          if (timeout && !System.Diagnostics.Debugger.IsAttached) timeoutTimer.Start();
          backgroundThread.Start();
          // successfully started the background task
          return true;
        }
        catch (Exception ex)
        {
          LogMyFilms.Error(ex);
          IsBusy = false;
          _CurrentResultHandler = null;
          GUIWaitCursor.Hide(); // hide the wait cursor
          return false; // could not start the background task
        }
      }
      else
      {
        LogMyFilms.Error("Another thread tried to execute a task in background.");
        return false;
      }
    }

    void ExecuteTaskResultHandler()
    {
      if (!IsBusy) return;

      // show an error message if task was not completed successfully
      if (_CurrentTaskSuccess != true)
      {
        if (_CurrentError != null)
        {
          MediaPortal.Dialogs.GUIDialogOK dlg_error = (MediaPortal.Dialogs.GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
          if (dlg_error != null)
          {
            dlg_error.Reset();
            dlg_error.SetHeading(GUIUtils.PluginName());
            dlg_error.SetLine(1, string.Format("{0} {1}", "Error", _CurrentTaskDescription)); 
            dlg_error.SetLine(2, _CurrentError.Message);
            dlg_error.DoModal(GUIWindowManager.ActiveWindow);
          }
        }
        else
        {
          GUIDialogNotify dlg_error = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
          if (dlg_error != null)
          {
            dlg_error.Reset();
            // dlg_error.SetImage();
            dlg_error.SetHeading(GUIUtils.PluginName());
            dlg_error.SetText(_CurrentTaskSuccess.HasValue
              ? string.Format("{0} {1}", "Error", _CurrentTaskDescription)
              : string.Format("{0} {1}", "Timeout", _CurrentTaskDescription));
            if (!abortedByUser) dlg_error.DoModal(GUIWindowManager.ActiveWindow);
          }
        }
      }

      // store info needed to invoke the result handler
      bool storedTaskSuccess = _CurrentTaskSuccess == true;
      Action<bool, object> storedHandler = _CurrentResultHandler;
      object storedResultObject = _CurrentResult;

      // clear all fields and allow execution of another background task 
      // before actually executing the result handler -> this way a result handler can also inovke another background task)
      _CurrentResultHandler = null;
      _CurrentResult = null;
      _CurrentTaskSuccess = null;
      _CurrentError = null;
      backgroundThread = null;
      abortedByUser = false;
      IsBusy = false;
      timeoutTimer.Stop();
      Monitor.Exit(this);

      // execute the result handler
      if (storedHandler != null) storedHandler.Invoke(storedTaskSuccess, storedResultObject);
    }
  }


}
