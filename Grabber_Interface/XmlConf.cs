using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MediaPortal.Configuration;

namespace Grabber_Interface
{
  class XmlConf
  {

    // Lists that contain all nodes
    public List<ListNode> ListGen = new List<ListNode>();
    public List<ListNode> ListSearch = new List<ListNode>();
    public List<ListNode> ListDetail = new List<ListNode>();
    public List<ListNode> ListMapping = new List<ListNode>();

    // Contructeur
    public XmlConf(string configFile)
    {
      if (File.Exists(configFile))
        Init(configFile);
      else
        InitNew(configFile);
    }

    private void InitNew(string configFile)
    {
      //Assembly _ass = Assembly.GetExecutingAssembly();
      //Stream _stream = _ass.GetManifestResourceStream(_ass.GetName().Name + ".MyFilmsSample.xml");

      string MyFilmsSampleGrabber = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsSampleGrabber.xml";

      var xDoc = new XmlDocument();
      var str = new StreamReader(MyFilmsSampleGrabber, Encoding.UTF8);
      //StreamReader _str = new StreamReader(_stream, System.Text.Encoding.UTF8);
      string xmlStrings = string.Empty;

      while (str.Peek() > 0)
      {
        xmlStrings += str.ReadLine();
      }

      File.WriteAllText(configFile, xmlStrings, Encoding.UTF8);
      Init(configFile);
    }

    private void Init(string configFile)
    {
      //Loading conf file
      var doc = new XmlDocument();
      doc.Load(configFile);

      XmlNode n = doc.ChildNodes[1].FirstChild;
      XmlNodeList l = n.ChildNodes;

      for (int i = 0; i < l.Count; i++)
      {
        if (l.Item(i).ParentNode.Name == "Section"
            && !l.Item(i).Name.Equals("URLSearch") && !l.Item(i).Name.Equals("Details") && !l.Item(i).Name.Equals("Mapping"))
          SetList(1, l.Item(i));
      }

      l = n.SelectNodes("URLSearch/*");

      for (int i = 0; i < l.Count; i++)
      {
        if (l.Item(i).ParentNode.Name == "URLSearch")
          SetList(2, l.Item(i));
      }

      l = n.SelectNodes("Details/*");

      for (int i = 0; i < l.Count; i++)
      {
        if (l.Item(i).ParentNode.Name == "Details")
          SetList(3, l.Item(i));
      }

      l = n.SelectNodes("Mapping/*");

      for (int i = 0; i < l.Count; i++)
      {
        if (l.Item(i).ParentNode.Name == "Mapping")
          SetList(4, l.Item(i));
      }

    }

    /// <summary>
    /// Adds a new element in the specified list.
    /// Parameter List : 1 for ListGen, 2 for ListSearch, 3 for ListDetail
    /// </summary>
    private void SetList(int list, XmlNode node)
    {
      XmlNode att1 = null;
      XmlNode att2 = null;
      XmlNode att3 = null;
      XmlNode att4 = null;
      XmlNode att5 = null;
      XmlNode att6 = null;
      XmlNode att7 = null;

      for (int i = 0; i < node.Attributes.Count; i++)
      {
        switch (i)
        {
          case 0:
            att1 = node.Attributes.Item(i);
            break;
          case 1:
            att2 = node.Attributes.Item(i);
            break;
          case 2:
            att3 = node.Attributes.Item(i);
            break;
          case 3:
            att4 = node.Attributes.Item(i);
            break;
          case 4:
            att5 = node.Attributes.Item(i);
            break;
          case 5:
            att6 = node.Attributes.Item(i);
            break;
          case 6:
            att7 = node.Attributes.Item(i);
            break;
        }
        //if (i == 0)
        //      att1 = node.Attributes.Item(i);
        //  else
        //      att2 = node.Attributes.Item(i);
      }
      switch (list)
      {
        case 1:
          ListGen.Add(new ListNode(node.Name,
            XmlConvert.DecodeName(node.InnerText),
            att1 == null ? null : XmlConvert.DecodeName(att1.InnerText),
            att2 == null ? null : XmlConvert.DecodeName(att2.InnerText),
            att3 == null ? null : XmlConvert.DecodeName(att3.InnerText),
            att4 == null ? null : XmlConvert.DecodeName(att4.InnerText),
            att5 == null ? null : XmlConvert.DecodeName(att5.InnerText),
            att6 == null ? null : XmlConvert.DecodeName(att6.InnerText),
            att7 == null ? null : XmlConvert.DecodeName(att7.InnerText)));

          break;
        case 2:
          ListSearch.Add(new ListNode(node.Name,
            XmlConvert.DecodeName(node.InnerText),
            att1 == null ? null : XmlConvert.DecodeName(att1.InnerText),
            att2 == null ? null : XmlConvert.DecodeName(att2.InnerText),
            att3 == null ? null : XmlConvert.DecodeName(att3.InnerText),
            att4 == null ? null : XmlConvert.DecodeName(att4.InnerText),
            att5 == null ? null : XmlConvert.DecodeName(att5.InnerText),
            att6 == null ? null : XmlConvert.DecodeName(att6.InnerText),
            att7 == null ? null : XmlConvert.DecodeName(att7.InnerText)));
          break;
        case 3:
          ListDetail.Add(new ListNode(node.Name,
            XmlConvert.DecodeName(node.InnerText),
            att1 == null ? null : XmlConvert.DecodeName(att1.InnerText),
            att2 == null ? null : XmlConvert.DecodeName(att2.InnerText),
            att3 == null ? null : XmlConvert.DecodeName(att3.InnerText),
            att4 == null ? null : XmlConvert.DecodeName(att4.InnerText),
            att5 == null ? null : XmlConvert.DecodeName(att5.InnerText),
            att6 == null ? null : XmlConvert.DecodeName(att6.InnerText),
            att7 == null ? null : XmlConvert.DecodeName(att7.InnerText)));
          break;
        case 4:
          ListMapping.Add(new ListNode(node.Name,
            XmlConvert.DecodeName(node.InnerText),
            att1 == null ? null : XmlConvert.DecodeName(att1.InnerText),
            att2 == null ? null : XmlConvert.DecodeName(att2.InnerText),
            att3 == null ? null : XmlConvert.DecodeName(att3.InnerText),
            att4 == null ? null : XmlConvert.DecodeName(att4.InnerText),
            att5 == null ? null : XmlConvert.DecodeName(att5.InnerText),
            att6 == null ? null : XmlConvert.DecodeName(att6.InnerText),
            att7 == null ? null : XmlConvert.DecodeName(att7.InnerText)));
          break;
      }
    }

    /// <summary>
    /// Recherche un élément dans la liste et le retourne
    /// Paramètre list : 1 pour ListGen, 2 pour ListSearch, 3 pour ListDetail
    /// </summary>
    public ListNode find(List<ListNode> list, string name)
    {
      ListNode listNode = list.Find(l => l.Tag.Equals(name));
      if (listNode != null)
        return listNode;
      listNode = new ListNode(name, "", "", "", "", "", "", "", "");
      list.Add(listNode);
      return listNode;
    }

  }


/*
 * Représente un noeud du fichier de conf
 * Contient le nom du tag, sa valeur, et 2 attributs.
 * 
 */
  class ListNode
  {
    private string tag = string.Empty;
    private string value = string.Empty;
    private string param1 = string.Empty;
    private string param2 = string.Empty;
    private string param3 = string.Empty;
    private string param4 = string.Empty;
    private string param5 = string.Empty;
    private string param6 = string.Empty;
    private string param7 = string.Empty;

    public ListNode(string tag, string value, string param1, string param2, string param3, string param4, string param5, string param6, string param7)
    {
      this.tag = tag;
      this.value = value;
      if (param1 == null)
        param1 = "";
      this.param1 = param1;
      if (param2 == null)
        param2 = "";
      this.param2 = param2;

      if (param3 == null)
        param3 = "";
      this.param3 = param3;
      if (param4 == null)
        param4 = "";
      this.param4 = param4;
      if (param5 == null)
        param5 = "";
      this.param5 = param5;
      if (param6 == null)
        param6 = "";
      this.param6 = param6;
      if (param7 == null)
        param7 = "";
      this.param7 = param7;
    }

    public string Tag
    {
      get { return tag; }
      set { tag = value; }
    }

    public string Value
    {
      get { return this.value; }
      set { this.value = value; }
    }

    public string Param1
    {
      get { return param1; }
      set { param1 = value; }
    }

    public string Param2
    {
      get { return param2; }
      set { param2 = value; }
    }

    public string Param3
    {
      get { return param3; }
      set { param3 = value; }
    }

    public string Param4
    {
      get { return param4; }
      set { param4 = value; }
    }

    public string Param5
    {
      get { return param5; }
      set { param5 = value; }
    }

    public string Param6
    {
      get { return param6; }
      set { param6 = value; }
    }

    public string Param7
    {
      get { return param7; }
      set { param7 = value; }
    }
  }


//public class LinkPage
//{
//  public static int Index = 1;
//  public string DisplayName;
//  public static string LinkPage = "LinkPage" + Index;
//  public static string Start = "Start" + Index;
//  public static string End = "End" + Index;
//  public static string PageIndex = "PageIndex" + Index;
//  public static string Page = "Page" + Index;
//  public static string Encoding = "Encoding" + Index;
//  public static string Result = null;
//}

//public class GrabElement
//{
//  public static int Index;
//  public string DisplayName;
//  public static string LinkPage = "LinkPage" + Index;
//  public static string Start = "Start" + Index;
//  public static string End = "End" + Index;
//  public static string PageIndex = "PageIndex" + Index;
//  public static string Page = "Page" + Index;
//  public static string Encoding = "Encoding" + Index;
//  // inner matchgroups
//  public static string RegExp = "RegExp" + Index;
//  public static string MaxItems = "MaxItems" + Index;
//  public static string Filter = "Filter" + Index;
  
//  public static string Result = null;

//}


//Class qui contient la liste des tags des fichiers de conf
//Permet d'éviter d'aller chercher dans les fichiers pour se souvenir du nom ...
// A compléter si ajout de tag.
  public class TagName
  {

    public static string DBName = "DBName";
    public static string URLPrefix = "URLPrefix";
    public static string URL = "URL";
    public static string Language = "Language";                   // Added Language to filter grabber scripts in choice menus
    public static string Type = "Type";                           // Added Type to filter grabber scripts in choice menus
    public static string Version = "Version";                     // Added Version for future use
    public static string Encoding = "Encoding";                   // Added Encoding as override options for webpages not properly posting page encoding
    public static string SearchCleanup = "SearchCleanup";         // Added Encoding as override options for webpages not properly posting page encoding
    public static string FileBasedReader = "FileBasedReader";     // Added FileBasedReader as option to read data from filesystem instead of web

    public static string Accept = "Accept";                       // Added Accept as override options for web requests
    public static string UserAgent = "UserAgent";                 // Added Useragent as override options for web requests
    public static string Headers = "Headers";                     // Added Headers as override options for web requests

    public static string KeyStartThumb = "KeyStartThumb";         // Start / End Thumb for cover image in list result view
    public static string KeyEndThumb = "KeyEndThumb";
    public static string KeyStartList = "KeyStartList";           // List
    public static string KeyEndList = "KeyEndList";
    public static string KeyNextPage = "KeyNextPage";             // Page
    public static string KeyStartPage = "KeyStartPage";
    public static string KeyStepPage = "KeyStepPage";
    public static string KeyStartTitle = "KeyStartTitle";         // Title
    public static string KeyEndTitle = "KeyEndTitle";
    public static string KeyStartDirector = "KeyStartDirector";   // Director
    public static string KeyEndDirector = "KeyEndDirector";
    public static string KeyStartYear = "KeyStartYear";           // Year
    public static string KeyEndYear = "KeyEndYear";
    public static string KeyYearIndex = "KeyYearIndex";
    public static string KeyYearPage = "KeyYearPage";
    public static string KeyStartLink = "KeyStartLink";           // Start / End Grabber Detail page
    public static string KeyEndLink = "KeyEndLink";
    public static string KeyStartID = "KeyStartID";           // Start / End ID (IMDB_Id)
    public static string KeyEndID = "KeyEndID";
    public static string KeyStartOptions = "KeyStartOptions";           // Start / End Options
    public static string KeyEndOptions = "KeyEndOptions";
    public static string KeyStartAkas = "KeyStartAkas";           // Start / End AKAs
    public static string KeyEndAkas = "KeyEndAkas";
    public static string KeyAkasRegExp = "KeyAkasRegExp";

    public static string KeyStartBody = "KeyStartBody";
    public static string KeyEndBody = "KeyEndBody";
    public static string KeyStartOTitle = "KeyStartOTitle";       // Original Title
    public static string KeyEndOTitle = "KeyEndOTitle";
    public static string KeyOTitleIndex = "KeyOTitleIndex";
    public static string KeyOTitlePage = "KeyOTitlePage";
    public static string KeyStartTTitle = "KeyStartTTitle";       // TranslatedTitle
    public static string KeyEndTTitle = "KeyEndTTitle";
    public static string KeyTTitleIndex = "KeyTTitleIndex";
    public static string KeyTTitlePage = "KeyTTitlePage";
    public static string KeyTTitleRegExp = "KeyTTitleRegExp";
    public static string KeyTTitleMaxItems = "KeyTTitleMaxItems";
    public static string KeyTTitleLanguage = "KeyTTitleLanguage";
    public static string KeyTTitleLanguageAll = "KeyTTitleLanguageAll";
    public static string KeyStartImg = "KeyStartImg";             // Cover Image
    public static string KeyEndImg = "KeyEndImg";
    public static string KeyImgIndex = "KeyImgIndex";
    public static string KeyImgPage = "KeyImgPage";
    public static string KeyStartLinkImg = "KeyStartLinkImg";     // Linkpage for Image
    public static string KeyEndLinkImg = "KeyEndLinkImg";
    public static string KeyLinkImgIndex = "KeyLinkImgIndex";
    public static string KeyLinkImgPage = "KeyLinkImgPage";
    public static string KeyEncodingLinkImg = "KeyEncodingLinkImg"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartRate = "KeyStartRate";           // Rating
    public static string KeyEndRate = "KeyEndRate";
    public static string KeyRateIndex = "KeyRateIndex";
    public static string KeyRatePage = "KeyRatePage";
    public static string KeyStartRate2 = "KeyStartRate2";         // Rating 2
    public static string KeyEndRate2 = "KeyEndRate2";
    public static string KeyRate2Index = "KeyRate2Index";
    public static string KeyRate2Page = "KeyRate2Page";

    public static string KeyStartLinkSyn = "KeyStartLinkSyn"; // Synopsis/Description Link Page
    public static string KeyEndLinkSyn = "KeyEndLinkSyn";
    public static string KeyLinkSynIndex = "KeyLinkSynIndex";
    public static string KeyLinkSynPage = "KeyLinkSynPage";
    public static string KeyEncodingLinkSyn = "KeyEncodingLinkSyn"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartSyn = "KeyStartSyn";             // Synopsis / Description
    public static string KeyEndSyn = "KeyEndSyn";
    public static string KeySynIndex = "KeySynIndex";
    public static string KeySynPage = "KeySynPage";
    public static string KeyStartRealise = "KeyStartRealise";     // Director
    public static string KeyEndRealise = "KeyEndRealise";
    public static string KeyRealiseIndex = "KeyRealiseIndex";
    public static string KeyRealisePage = "KeyRealisePage";
    public static string KeyRealiseRegExp = "KeyRealiseRegExp";
    public static string KeyRealiseMaxItems = "KeyRealiseMaxItems";
    public static string KeyStartProduct = "KeyStartProduct";     // Producer
    public static string KeyEndProduct = "KeyEndProduct";
    public static string KeyProductIndex = "KeyProductIndex";
    public static string KeyProductPage = "KeyProductPage";
    public static string KeyProductRegExp = "KeyProductRegExp";
    public static string KeyProductMaxItems = "KeyProductMaxItems";
    public static string KeyStartWriter = "KeyStartWriter";     // Writer
    public static string KeyEndWriter = "KeyEndWriter";
    public static string KeyWriterIndex = "KeyWriterIndex";
    public static string KeyWriterPage = "KeyWriterPage";
    public static string KeyWriterRegExp = "KeyWriterRegExp";
    public static string KeyWriterMaxItems = "KeyWriterMaxItems";
    public static string KeyStartCredits = "KeyStartCredits";     // Credits / Actors
    public static string KeyEndCredits = "KeyEndCredits";
    public static string KeyCreditsIndex = "KeyCreditsIndex";
    public static string KeyCreditsPage = "KeyCreditsPage";
    public static string KeyCreditsRegExp = "KeyCreditsRegExp";
    public static string KeyCreditsMaxItems = "KeyCreditsMaxItems";
    public static string KeyCreditsGrabActorRoles = "KeyCreditsGrabActorRoles";
    public static string KeyStartCountry = "KeyStartCountry";     // Country
    public static string KeyEndCountry = "KeyEndCountry";
    public static string KeyCountryRegExp = "KeyCountryRegExp";
    public static string KeyCountryIndex = "KeyCountryIndex";
    public static string KeyCountryPage = "KeyCountryPage";
    public static string KeyStartGenre = "KeyStartGenre";         // Genre/Categories
    public static string KeyEndGenre = "KeyEndGenre";
    public static string KeyGenreRegExp = "KeyGenreRegExp";
    public static string KeyGenreIndex = "KeyGenreIndex";
    public static string KeyGenrePage = "KeyGenrePage";
    public static string BaseRating = "BaseRating";               // Baserating
    // Guzzi: Added to extend Grabber
    public static string KeyStartLinkPersons = "KeyStartLinkPersons";
    public static string KeyEndLinkPersons = "KeyEndLinkPersons";
    public static string KeyLinkPersonsIndex = "KeyLinkPersonsIndex";
    public static string KeyLinkPersonsPage = "KeyLinkPersonsPage";
    public static string KeyEncodingLinkPersons = "KeyEncodingLinkPersons"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartLinkTitles = "KeyStartLinkTitles";
    public static string KeyEndLinkTitles = "KeyEndLinkTitles";
    public static string KeyLinkTitlesIndex = "KeyLinkTitlesIndex";
    public static string KeyLinkTitlesPage = "KeyLinkTitlesPage";
    public static string KeyEncodingLinkTitles = "KeyEncodingLinkTitles"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartLinkCertification = "KeyStartLinkCertification";
    public static string KeyEndLinkCertification = "KeyEndLinkCertification";
    public static string KeyLinkCertificationIndex = "KeyLinkCertificationIndex";
    public static string KeyLinkCertificationPage = "KeyLinkCertificationPage";
    public static string KeyEncodingLinkCertification = "KeyEncodingLinkCertification"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartLinkComment = "KeyStartLinkComment"; // Comment Link Page
    public static string KeyEndLinkComment = "KeyEndLinkComment";
    public static string KeyLinkCommentIndex = "KeyLinkCommentIndex";
    public static string KeyLinkCommentPage = "KeyLinkCommentPage";
    public static string KeyEncodingLinkComment = "KeyEncodingLinkComment"; // Added Encoding as override options for webpages not properly posting page encoding


    public static string KeyStartComment = "KeyStartComment";     // Comment
    public static string KeyEndComment = "KeyEndComment";
    public static string KeyCommentRegExp = "KeyCommentRegExp";
    public static string KeyCommentIndex = "KeyCommentIndex";
    public static string KeyCommentPage = "KeyCommentPage";

    public static string KeyStartLanguage = "KeyStartLanguage";     // Language
    public static string KeyEndLanguage = "KeyEndLanguage";
    public static string KeyLanguageRegExp = "KeyLanguageRegExp";
    public static string KeyLanguageIndex = "KeyLanguageIndex";
    public static string KeyLanguagePage = "KeyLanguagePage";

    public static string KeyStartTagline = "KeyStartTagline";     // Tagline 
    public static string KeyEndTagline = "KeyEndTagline";
    public static string KeyTaglineIndex = "KeyTaglineIndex";
    public static string KeyTaglinePage = "KeyTaglinePage";

    public static string KeyStartCertification = "KeyStartCertification";     // Certification
    public static string KeyEndCertification = "KeyEndCertification";
    public static string KeyCertificationRegExp = "KeyCertificationRegExp";
    public static string KeyCertificationIndex = "KeyCertificationIndex";
    public static string KeyCertificationPage = "KeyCertificationPage";
    public static string KeyCertificationLanguage = "KeyCertificationLanguage";
    public static string KeyCertificationLanguageAll = "KeyCertificationLanguageAll";

    public static string KeyStartStudio = "KeyStartStudio";     // Studio
    public static string KeyEndStudio = "KeyEndStudio";
    public static string KeyStudioRegExp = "KeyStudioRegExp";
    public static string KeyStudioIndex = "KeyStudioIndex";
    public static string KeyStudioPage = "KeyStudioPage";

    public static string KeyStartEdition = "KeyStartEdition";     // Edition
    public static string KeyEndEdition = "KeyEndEdition";
    public static string KeyEditionIndex = "KeyEditionIndex";
    public static string KeyEditionPage = "KeyEditionPage";

    public static string KeyStartIMDB_Rank = "KeyStartIMDB_Rank";     // IMDB_Rank
    public static string KeyEndIMDB_Rank = "KeyEndIMDB_Rank";
    public static string KeyIMDB_RankIndex = "KeyIMDB_RankIndex";
    public static string KeyIMDB_RankPage = "KeyIMDB_RankPage";

    public static string KeyStartIMDB_Id = "KeyStartIMDB_Id";     // IMDB_Id
    public static string KeyEndIMDB_Id = "KeyEndIMDB_Id";
    public static string KeyIMDB_IdIndex = "KeyIMDB_IdIndex";
    public static string KeyIMDB_IdPage = "KeyIMDB_IdPage";

    public static string KeyStartTMDB_Id = "KeyStartTMDB_Id";     // TMDB_Id
    public static string KeyEndTMDB_Id = "KeyEndTMDB_Id";
    public static string KeyTMDB_IdIndex = "KeyTMDB_IdIndex";
    public static string KeyTMDB_IdPage = "KeyTMDB_IdPage";

    public static string KeyStartCollection = "KeyStartCollection";     // Collection (currently supported on TMDB API V3
    public static string KeyEndCollection = "KeyEndCollection";
    public static string KeyCollectionIndex = "KeyCollectionIndex";
    public static string KeyCollectionPage = "KeyCollectionPage";

    public static string KeyStartCollectionImageURL = "KeyStartCollectionImageURL";     // Collection (currently supported on TMDB API V3
    public static string KeyEndCollectionImageURL = "KeyEndCollectionImageURL";
    public static string KeyCollectionImageURLIndex = "KeyCollectionImageURLIndex";
    public static string KeyCollectionImageURLPage = "KeyCollectionImageURLPage";

    // secondary Details page - will be loaded first and is available for all fields as option
    public static string KeyStartDetails2 = "KeyStartDetails2";     // Details2
    public static string KeyEndDetails2 = "KeyEndDetails2";
    public static string KeyDetails2Index = "KeyDetails2Index";
    public static string KeyDetails2Page = "KeyDetails2Page";
    public static string KeyEncodingDetails2 = "KeyEncodingDetails2"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartLinkGeneric1 = "KeyStartLinkGeneric1"; // Generic Link Page 1
    public static string KeyEndLinkGeneric1 = "KeyEndLinkGeneric1";
    public static string KeyLinkGeneric1Index = "KeyLinkGeneric1Index";
    public static string KeyLinkGeneric1Page = "KeyLinkGeneric1Page";
    public static string KeyEncodingLinkGeneric1 = "KeyEncodingLinkGeneric1"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartLinkGeneric2 = "KeyStartLinkGeneric2"; // Generic Link Page 2
    public static string KeyEndLinkGeneric2 = "KeyEndLinkGeneric2";
    public static string KeyLinkGeneric2Index = "KeyLinkGeneric2Index";
    public static string KeyLinkGeneric2Page = "KeyLinkGeneric2Page";
    public static string KeyEncodingLinkGeneric2 = "KeyEncodingLinkGeneric2"; // Added Encoding as override options for webpages not properly posting page encoding

    // Generic Fields
    public static string KeyStartGeneric1 = "KeyStartGeneric1";     // Generic Fields 1
    public static string KeyEndGeneric1 = "KeyEndGeneric1";
    public static string KeyGeneric1RegExp = "KeyGeneric1RegExp";
    public static string KeyGeneric1Index = "KeyGeneric1Index";
    public static string KeyGeneric1MaxItems = "KeyGeneric1MaxItems";
    public static string KeyGeneric1Language = "KeyGeneric1Language";
    public static string KeyGeneric1Page = "KeyGeneric1Page";
    public static string KeyStartGeneric2 = "KeyStartGeneric2";     // Generic Fields 2
    public static string KeyEndGeneric2 = "KeyEndGeneric2";
    public static string KeyGeneric2RegExp = "KeyGeneric2RegExp";
    public static string KeyGeneric2Index = "KeyGeneric2Index";
    public static string KeyGeneric2MaxItems = "KeyGeneric2MaxItems";
    public static string KeyGeneric2Language = "KeyGeneric2Language";
    public static string KeyGeneric2Page = "KeyGeneric2Page";
    public static string KeyStartGeneric3 = "KeyStartGeneric3";     // Generic Fields 3
    public static string KeyEndGeneric3 = "KeyEndGeneric3";
    public static string KeyGeneric3RegExp = "KeyGeneric3RegExp";
    public static string KeyGeneric3Index = "KeyGeneric3Index";
    public static string KeyGeneric3MaxItems = "KeyGeneric3MaxItems";
    public static string KeyGeneric3Language = "KeyGeneric3Language";
    public static string KeyGeneric3Page = "KeyGeneric3Page";

    // new added for extended scraping 2011-10-04
    //URL Redirection Multi Posters
    //URL Redirection Photos
    //URL Redirection PersonImages
    //URL Redirection Multi Fanart
    //URL Redirection Trailer

    //MultiPosters
    //Photos
    //PersonImages
    //MultiFanart
    //Trailer

    public static string KeyStartLinkMultiPosters = "KeyStartLinkMultiPosters"; // MultiPosters Link
    public static string KeyEndLinkMultiPosters = "KeyEndLinkMultiPosters";
    public static string KeyLinkMultiPostersIndex = "KeyLinkMultiPostersIndex";
    public static string KeyLinkMultiPostersPage = "KeyLinkMultiPostersPage";
    public static string KeyEncodingLinkMultiPosters = "KeyEncodingLinkMultiPosters"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartMultiPosters = "KeyStartMultiPosters";     // MultiPosters 
    public static string KeyEndMultiPosters = "KeyEndMultiPosters";
    public static string KeyMultiPostersIndex = "KeyMultiPostersIndex";
    public static string KeyMultiPostersRegExp = "KeyMultiPostersRegExp";
    public static string KeyMultiPostersMaxItems = "KeyMultiPostersMaxItems";
    public static string KeyMultiPostersLanguage = "KeyMultiPostersLanguage";
    public static string KeyMultiPostersPage = "KeyMultiPostersPage";

    public static string KeyStartLinkPhotos = "KeyStartLinkPhotos"; // Photos Link
    public static string KeyEndLinkPhotos = "KeyEndLinkPhotos";
    public static string KeyLinkPhotosIndex = "KeyLinkPhotosIndex";
    public static string KeyLinkPhotosPage = "KeyLinkPhotosPage";
    public static string KeyEncodingLinkPhotos = "KeyEncodingLinkPhotos"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartPhotos = "KeyStartPhotos";     // Photos 
    public static string KeyEndPhotos = "KeyEndPhotos";
    public static string KeyPhotosIndex = "KeyPhotosIndex";
    public static string KeyPhotosRegExp = "KeyPhotosRegExp";
    public static string KeyPhotosMaxItems = "KeyPhotosMaxItems";
    public static string KeyPhotosLanguage = "KeyPhotosLanguage";
    public static string KeyPhotosPage = "KeyPhotosPage";

    public static string KeyStartLinkPersonImages = "KeyStartLinkPersonImages"; // PersonImages Link
    public static string KeyEndLinkPersonImages = "KeyEndLinkPersonImages";
    public static string KeyLinkPersonImagesIndex = "KeyLinkPersonImagesIndex";
    public static string KeyLinkPersonImagesPage = "KeyLinkPersonImagesPage";
    public static string KeyEncodingLinkPersonImages = "KeyEncodingLinkPersonImages"; // Added Encoding as override options for webpages not properly posting page encoding

    public static string KeyStartPersonImages = "KeyStartPersonImages";     // PersonImages 
    public static string KeyEndPersonImages = "KeyEndPersonImages";
    public static string KeyPersonImagesIndex = "KeyPersonImagesIndex";
    public static string KeyPersonImagesRegExp = "KeyPersonImagesRegExp";
    public static string KeyPersonImagesMaxItems = "KeyPersonImagesMaxItems";
    public static string KeyPersonImagesLanguage = "KeyPersonImagesLanguage";
    public static string KeyPersonImagesPage = "KeyPersonImagesPage";

    public static string KeyStartLinkMultiFanart = "KeyStartLinkMultiFanart"; // MultiFanart Link
    public static string KeyEndLinkMultiFanart = "KeyEndLinkMultiFanart";
    public static string KeyLinkMultiFanartIndex = "KeyLinkMultiFanartIndex";
    public static string KeyLinkMultiFanartPage = "KeyLinkMultiFanartPage";
    public static string KeyEncodingLinkMultiFanart = "KeyEncodingLinkMultiFanart"; // Added Encoding as override options for webpages not properly posting page encoding
    public static string KeyStartMultiFanart = "KeyStartMultiFanart";     // MultiFanart 
    public static string KeyEndMultiFanart = "KeyEndMultiFanart";
    public static string KeyMultiFanartIndex = "KeyMultiFanartIndex";
    public static string KeyMultiFanartRegExp = "KeyMultiFanartRegExp";
    public static string KeyMultiFanartMaxItems = "KeyMultiFanartMaxItems";
    public static string KeyMultiFanartLanguage = "KeyMultiFanartLanguage";
    public static string KeyMultiFanartPage = "KeyMultiFanartPage";

    public static string KeyStartLinkTrailer = "KeyStartLinkTrailer"; // Trailer Link
    public static string KeyEndLinkTrailer = "KeyEndLinkTrailer";
    public static string KeyLinkTrailerIndex = "KeyLinkTrailerIndex";
    public static string KeyLinkTrailerPage = "KeyLinkTrailerPage";
    public static string KeyEncodingLinkTrailer = "KeyEncodingLinkTrailer"; // Added Encoding as override options for webpages not properly posting page encoding
    public static string KeyStartTrailer = "KeyStartTrailer";     // Trailer 
    public static string KeyEndTrailer = "KeyEndTrailer";
    public static string KeyTrailerIndex = "KeyTrailerIndex";
    public static string KeyTrailerRegExp = "KeyTrailerRegExp";
    public static string KeyTrailerMaxItems = "KeyTrailerMaxItems";
    public static string KeyTrailerLanguage = "KeyTrailerLanguage";
    public static string KeyTrailerPage = "KeyTrailerPage";

    public static string KeyStartRuntime = "KeyStartRuntime";     // Runtime
    public static string KeyEndRuntime = "KeyEndRuntime";
    public static string KeyRuntimeRegExp = "KeyRuntimeRegExp";
    public static string KeyRuntimeIndex = "KeyRuntimeIndex";
    public static string KeyRuntimeMaxItems = "KeyRuntimeMaxItems";
    public static string KeyRuntimeLanguage = "KeyRuntimeLanguage";
    public static string KeyRuntimePage = "KeyRuntimePage";
  }
}