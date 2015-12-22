using System;


namespace MyFilmsPlugin.MyFilmsGUI
{
  public class MFView
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public MFView()
    {
      CurrentContext = MyFilms.ViewContext.Menu;
    }

    public MyFilms.ViewContext CurrentContext { get; set; }  // "Boolselect" - or Films, Views, Persons, Herarchies ... GetSelectFromDivX or GetFilmList ...
    public int ID { get; set; }
    public int PrevID { get; set; }  // to access  a state via "PrevID"
    public View StartSettings { get; set; }
    public View CurrentSettings { get; set; }
  }

  public class View
  {
    public View()
    {
      FilmsSortDirection = string.Empty;
      FilmsSortItemFriendlyName = string.Empty;
      FilmsSortItem = string.Empty;
      FilmsLayout = 0;
      HierarchySortDirection = string.Empty;
      HierarchySortItemFriendlyName = string.Empty;
      HierarchySortItem = string.Empty;
      HierarchyLayout = 0;
      PersonsSortDirection = string.Empty;
      PersonsSortItemFriendlyName = string.Empty;
      PersonsSortType = MyFilms.ViewSortType.Name;
      PersonsLayout = 0;
      ViewFilter = string.Empty;
      ViewSortDirection = string.Empty;
      ViewSortType = MyFilms.ViewSortType.Name;
      ViewLayout = 0;
      ViewDBItemValue = string.Empty;
      ViewDBItem = string.Empty;
      ViewDisplayName = string.Empty;
      ViewContext = MyFilms.ViewContext.Menu;
      ID = -1;
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region public vars

    public int ID { get; set; }
    public MyFilms.ViewContext ViewContext { get; set; }
    public string ViewDisplayName { get; set; }
    public string ViewDBItem { get; set; }
    public string ViewDBItemValue { get; set; }
    public MyFilms.Layout ViewLayout { get; set; }
    public MyFilms.ViewSortType ViewSortType { get; set; }
    public string ViewSortDirection { get; set; }
    public string ViewFilter { get; set; }
    public MyFilms.Layout PersonsLayout { get; set; }
    public MyFilms.ViewSortType PersonsSortType { get; set; }
    public string PersonsSortItemFriendlyName { get; set; }
    public string PersonsSortDirection { get; set; }
    public MyFilms.Layout HierarchyLayout { get; set; }
    public string HierarchySortItem { get; set; }
    public string HierarchySortItemFriendlyName { get; set; }
    public string HierarchySortDirection { get; set; }
    public MyFilms.Layout FilmsLayout { get; set; }
    public string FilmsSortItem { get; set; }
    public string FilmsSortItemFriendlyName { get; set; }
    public string FilmsSortDirection { get; set; }

    public void InitDefaults()
    {
      ID = -1;
      ViewContext = MyFilms.ViewContext.Menu;
      ViewDisplayName = "Films";
      ViewDBItem = "OriginalTitle";
      ViewDBItemValue = "";
      ViewLayout = MyFilms.Layout.List;
      ViewSortType = MyFilms.ViewSortType.Name;
      ViewSortDirection = " ASC";
      ViewFilter = string.Empty;
      PersonsLayout = MyFilms.Layout.List;
      PersonsSortType = MyFilms.ViewSortType.Name;
      PersonsSortItemFriendlyName = string.Empty;
      PersonsSortDirection = " ASC";
      HierarchyLayout = MyFilms.Layout.List;
      HierarchySortItem = "OriginalTitle";
      HierarchySortItemFriendlyName = string.Empty;
      HierarchySortDirection = " ASC";
      FilmsLayout = MyFilms.Layout.List;
      FilmsSortItem = "SortTitle";
      FilmsSortItemFriendlyName = string.Empty;
      FilmsSortDirection = " ASC";
    }

    public string SaveToString()
    {
      string savestring = ID + "|" + ViewDisplayName + "|" + ViewDBItem + "|" + ViewDBItemValue + "|" +
                          Enum.GetName(typeof(MyFilms.Layout), ViewLayout) + "|" + Enum.GetName(typeof(MyFilms.ViewSortType), ViewSortType) + "|" + ViewSortDirection + "|" + ViewFilter + "|" + 
                          Enum.GetName(typeof(MyFilms.Layout), PersonsLayout) + "|" + Enum.GetName(typeof(MyFilms.ViewSortType), PersonsSortType) + "|" + PersonsSortItemFriendlyName + "|" + PersonsSortDirection + "|" +
                          Enum.GetName(typeof(MyFilms.Layout), HierarchyLayout) + "|" + HierarchySortItem + "|" + HierarchySortItemFriendlyName + "|" + HierarchySortDirection + "|" +
                          Enum.GetName(typeof(MyFilms.Layout), FilmsLayout) + "|" + FilmsSortItem + "|" + FilmsSortItemFriendlyName + "|" + FilmsSortDirection;
      LogMyFilms.Debug("SaveToString() - output = '" + savestring + "'");
      return savestring;
    }

    public void LoadFromString(string inputstring)
    {
      int i = 0;
      string[] split = inputstring.Split(new char[] { '|' }, StringSplitOptions.None);
      LogMyFilms.Debug("LoadFromString() - parsed '" + split.Length + "' elements from inputstring = '" + inputstring + "'");
      foreach (string s in split)
      {
        LogMyFilms.Debug("LoadFromString() - Parsed Value [" + i + "] = '" + s + "'");
        i++;
      }
      ID = int.Parse(split[0]);
      // viewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), split[2], true);
      ViewDisplayName = split[1];
      ViewDBItem = split[2];
      ViewDBItemValue = split[3];
      ViewLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[4], true);
      ViewSortType = (MyFilms.ViewSortType)Enum.Parse(typeof(MyFilms.ViewSortType), split[5], true);
      ViewSortDirection = split[6];
      ViewFilter = split[7];
      PersonsLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[8], true);
      PersonsSortType = (MyFilms.ViewSortType)Enum.Parse(typeof(MyFilms.ViewSortType), split[9], true);
      PersonsSortItemFriendlyName = split[10];
      PersonsSortDirection = split[11];
      HierarchyLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[12], true);
      HierarchySortItem = split[13];
      HierarchySortItemFriendlyName = split[14];
      HierarchySortDirection = split[15];
      FilmsLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[16], true);
      FilmsSortItem = split[17];
      FilmsSortItemFriendlyName = split[18];
      FilmsSortDirection = split[19];
    }

    #endregion
  }

  public class CoverState
  {
    public CoverState(){}

    public CoverState(string menucover, string filmcover, string viewcover, string personcover, string groupcover)
    {
      MenuCover = menucover;
      FilmCover = filmcover;
      ViewCover = viewcover;
      PersonCover = personcover;
      GroupCover = groupcover;
    }

    #region public vars

    public string MenuCover { get; set; }
    public string FilmCover { get; set; }
    public string ViewCover { get; set; }
    public string PersonCover { get; set; }
    public string GroupCover { get; set; }

    #endregion
  }

  
  public class ViewState
  {
    public ViewState()
    {
      StrSelect = string.Empty;
      StrPersons = string.Empty;
      StrTitleSelect = string.Empty;
      StrFilmSelect = string.Empty;
      ViewContext = MyFilms.ViewContext.Menu;
      CurrentView = string.Empty;
      StrTxtView = string.Empty;
      StrTxtSelect = string.Empty;
      WStrSort = string.Empty;
      TitleItem = string.Empty;
      IndexItem = 0;
      LastID = 0;
      StrLayOutInHierarchies = 0;
      WStrLayOut = 0;
      StrLayOut = 0;
      Wstar = string.Empty;
      WStrSortSensCount = string.Empty;
      Wselectedlabel = string.Empty;
      IndexedChars = 0;
      BoolSkipViewState = false;
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region public vars
    public string StrSelect { get; set; }
    public string StrPersons { get; set; }
    public string StrTitleSelect { get; set; }
    public string StrFilmSelect { get; set; }
    public MyFilms.ViewContext ViewContext { get; set; }
    public string CurrentView { get; set; }
    public string StrTxtView { get; set; }
    public string StrTxtSelect { get; set; }
    public bool Boolselect { get; set; }
    public bool Boolreturn { get; set; }
    public bool Boolcollection { get; set; }
    public bool Boolindexed { get; set; }
    public bool Boolindexedreturn { get; set; }
    public int IndexedChars { get; set; }
    public bool BoolReverseNames { get; set; }
    public bool BoolVirtualPathBrowsing { get; set; }
    public bool BoolShowEmptyValuesInViews { get; set; }
    public bool BoolSkipViewState { get; set; }
    public string Wselectedlabel { get; set; }
    public string WStrSort { get; set; }
    public string WStrSortSensCount { get; set; }
    public bool BoolSortCountinViews { get; set; }
    public string Wstar { get; set; }
    public int StrLayOut { get; set; }
    public int WStrLayOut { get; set; }
    public int StrLayOutInHierarchies { get; set; }
    public int LastID { get; set; }
    public int IndexItem { get; set; }
    public string TitleItem { get; set; }
    // CurrentView", MyFilms.conf.CurrentView.SaveToString());
    #endregion

    public void InitDefaults()
    {
      StrSelect = string.Empty;
      StrPersons = string.Empty;
      StrTitleSelect = string.Empty;
      StrFilmSelect = string.Empty;
      ViewContext = MyFilms.ViewContext.Menu;
      StrTxtView = string.Empty;
      StrTxtSelect = string.Empty;
      Boolselect = false;
      Boolreturn = false;
      Boolcollection = false;
      Boolindexed = false;
      Boolindexedreturn = false;
      IndexedChars = 0;
      BoolReverseNames = false;
      BoolVirtualPathBrowsing = false;
      BoolShowEmptyValuesInViews = false;
      Wselectedlabel = string.Empty;
      WStrSort = string.Empty;
      WStrSortSensCount = string.Empty;
      BoolSortCountinViews = false;
      Wstar = string.Empty;
      StrLayOut = 0;
      WStrLayOut = 0;
      StrLayOutInHierarchies = 0;
      LastID = 0;
      IndexItem = 0;
      TitleItem = string.Empty;
    }

    public string SaveToString()
    {
      string savestring =
        StrSelect + "|" +
        StrPersons + "|" +
        StrTitleSelect + "|" +
        StrFilmSelect + "|" +
        Enum.GetName(typeof(MyFilms.Layout), ViewContext) + "|" +
        StrTxtView + "|" +
        StrTxtSelect + "|" +
        Boolselect + "|" +
        Boolreturn + "|" +
        Boolcollection + "|" +
        Boolindexed + "|" +
        Boolindexedreturn + "|" +
        IndexedChars + "|" +
        BoolReverseNames + "|" +
        BoolShowEmptyValuesInViews + "|" +
        Wselectedlabel + "|" +
        WStrSort + "|" +
        WStrSortSensCount + "|" +
        BoolSortCountinViews + "|" +
        Wstar + "|" +
        StrLayOut + "|" +
        WStrLayOut + "|" +
        StrLayOutInHierarchies + "|" +
        LastID + "|" +
        IndexItem + "|" +
        TitleItem + "|" +
        BoolVirtualPathBrowsing ;
      LogMyFilms.Debug("SaveToString() - output = '" + savestring + "'");
      return savestring;
    }

    public void LoadFromString(string inputstring)
    {
      int i = 0;
      string[] split = inputstring.Split(new[] { '|' }, StringSplitOptions.None);
      LogMyFilms.Debug("LoadFromString() - parsed '" + split.Length + "' elements from inputstring = '" + inputstring + "'");
      foreach (string s in split)
      {
        LogMyFilms.Debug("LoadFromString() - Parsed Value [" + i + "] = '" + s + "'");
        i++;
      }
      StrSelect = split[0];
      StrPersons = split[1];
      StrTitleSelect = split[2];
      StrFilmSelect = split[3];
      ViewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), split[4], true); // MyFilms.ViewContext.Menu;
      StrTxtView = split[5];
      StrTxtSelect = split[6];
      Boolselect = bool.Parse(split[7]);
      Boolreturn = bool.Parse(split[8]);
      Boolcollection = bool.Parse(split[9]);
      Boolindexed = bool.Parse(split[10]);
      Boolindexedreturn = bool.Parse(split[11]);
      IndexedChars = int.Parse(split[12]);
      BoolReverseNames = bool.Parse(split[13]);
      BoolShowEmptyValuesInViews = bool.Parse(split[14]);
      Wselectedlabel = split[15];
      WStrSort = split[16];
      WStrSortSensCount = split[17];
      BoolSortCountinViews = bool.Parse(split[18]);
      Wstar = split[19];
      StrLayOut = int.Parse(split[20]);
      WStrLayOut = int.Parse(split[21]);
      StrLayOutInHierarchies = int.Parse(split[22]);
      LastID = int.Parse(split[23]);
      IndexItem = int.Parse(split[24]);
      TitleItem = split[25];
      BoolVirtualPathBrowsing = bool.Parse(split[26]);
    }
  }

  public class CustomFilter
  {

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    
    public CustomFilter()
    {
      Field = "";
      Operator = "like";
      Value = "";
    }

    #region public vars

    public string Field { get; set; }
    public string Operator { get; set; }
    public string Value { get; set; }

    #endregion

    public string SaveToString()
    {
      string savestring = Field + "|" + Operator + "|" + Value;
      LogMyFilms.Debug("SaveToString() - output = '" + savestring + "'");
      return savestring;
    }

    public void LoadFromString(string inputstring)
    {
      int i = 0;
      string[] split = inputstring.Split(new[] { '|' }, StringSplitOptions.None);
      LogMyFilms.Debug("LoadFromString() - parsed '" + split.Length + "' elements from inputstring = '" + inputstring + "'");
      foreach (string s in split)
      {
        LogMyFilms.Debug("LoadFromString() - Parsed Value [" + i + "] = '" + s + "'");
        i++;
      }
      Field = split[0];
      Operator = split[1];
      Value = split[2];
    }


  }

}
