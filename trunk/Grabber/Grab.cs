using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web;
using System.Xml;
using MediaPortal.Util;
using System.IO;
using System.Text;
using MediaPortal.GUI.Library;

using MediaPortal.Configuration;
using MediaPortal.Services;

using NLog;
using NLog.Config;
using NLog.Targets;

namespace Grabber
{
  using grabber;
  using MediaPortal.Video.Database;
  using System.Text.RegularExpressions;
  using NLog.Targets;

    public class GrabberScript
    {
      string m_strFilename = "";
      string m_strDBName = "";
      string m_strURLPrefix = "";
      string m_strLanguage = "";
      string m_strType = "";
      // XmlNode Node;
      string m_strVersion = "";
      string m_strEncoding = "";

      public void Load(string strFilename)
      {
        //Loading the configuration file
        XmlDocument doc = new XmlDocument();
        doc.Load(strFilename);
        XmlNode n = doc.ChildNodes[1].FirstChild;

        try { m_strDBName = n.SelectSingleNode("DBName").InnerText; }
        catch (Exception) {m_strURLPrefix = "";}
        
        try { m_strURLPrefix = n.SelectSingleNode("URLPrefix").InnerText; }
        catch (Exception) {m_strURLPrefix = "";}

        try { m_strLanguage = n.SelectSingleNode("Language").InnerText; }
        catch (Exception) {m_strLanguage = "";}

        try { m_strType = n.SelectSingleNode("Type").InnerText; }
        catch (Exception) {m_strType = "";}

        try { m_strVersion = n.SelectSingleNode("Version").InnerText; }
        catch (Exception) {m_strVersion = "";}

        try { m_strEncoding = n.SelectSingleNode("Encoding").InnerText; }
        catch (Exception) {m_strEncoding = "";}
      }
      
      public GrabberScript(string strFilename)
      {
        m_strFilename = strFilename;
        if (!string.IsNullOrEmpty(m_strFilename))
          Load(m_strFilename);
      }

      public string DBName
      {
        get { return m_strDBName; }
        set { m_strDBName = value; }
      }

      public string URLPrefix
      {
        get { return m_strURLPrefix; }
        set { m_strURLPrefix = value; }
      }

      public string Language
      {
        get { return m_strLanguage; }
        set { m_strLanguage = value; }
      }

      public string Type
      {
        get { return m_strType; }
        set { m_strType = value; }
      }

      public string Version
      {
        get { return m_strVersion; }
        set { m_strVersion = value; }
      }

      public string Encoding
      {
        get { return m_strEncoding; }
        set { m_strEncoding = value; }
      }

      //public XmlNode URLSearch
      //{
      //  get { return Node; }
      //  set { Node = value; }
      //}

      //public XmlNode Details
      //{
      //  get { return Node; }
      //  set { Node = value; }
      //}

      //public XmlNode Mapping
      //{
      //  get { return Node; }
      //  set { Node = value; }
      //}
    };
  
    public class Grabber_URLClass
    {
        private static Logger LogMyFilmsGrabber = NLog.LogManager.GetCurrentClassLogger();  //log

        ArrayList elements = new ArrayList();

        public enum Grabber_Output
        {
            OriginalTitle = 0,
            TranslatedTitle = 1,
            PicturePathLong = 2,
            Description = 3,
            Rating = 4,
            Actors = 5,
            Director = 6,
            Producer = 7,
            Year = 8,
            Country = 9,
            Category = 10,
            URL = 11,
            PicturePathShort = 12,
            Writer = 13,
            Comments = 14,
            Language = 15,
            Tagline = 16,
            Certification = 17,
            IMDB_Id = 18,
            IMDBrank = 19,
            Studio = 20,
            Edition = 21,
            Fanart = 22,
            Generic1 = 23,
            Generic2 = 24,
            Generic3 = 25,
            TranslatedTitleAllNames = 26,
            TranslatedTitleAllValues = 27,
            CertificationAllNames = 28,
            CertificationAllValues = 29,

            MultiPosters = 30,
            Photos = 31,
            PersonImages = 32,
            MultiFanart = 33,
            Trailer = 34,
            Empty35 = 35,
            Empty36 = 36,
            Empty37 = 37,
            Empty38 = 38,
            Empty39 = 39,

            OriginalTitle_Transformed = 40,
            TranslatedTitle_Transformed = 41,
            PicturePathLong_Transformed = 42,
            Description_Transformed = 43,
            Rating_Transformed = 44,
            Actors_Transformed = 45,
            Director_Transformed = 46,
            Producer_Transformed = 47,
            Year_Transformed = 48,
            Country_Transformed = 49,
            Category_Transformed = 50,
            URL_Transformed = 51,
            PicturePathShort_Transformed = 52,
            Writer_Transformed = 53,
            Comments_Transformed = 54,
            Language_Transformed = 55,
            Tagline_Transformed = 56,
            Certification_Transformed = 57,
            IMDB_Id_Transformed = 58,
            IMDBrank_Transformed = 59,
            Studio_Transformed = 60,
            Edition_Transformed = 61,
            Fanart_Transformed = 62,
            Generic1_Transformed = 63,
            Generic2_Transformed = 64,
            Generic3_Transformed = 65,
            TranslatedTitleAllNames_Transformed = 66,
            TranslatedTitleAllValues_Transformed = 67,
            CertificationAllNames_Transformed = 68,
            CertificationAllValues_Transformed = 69,
            MultiPosters_Transformed = 70,
            Photos_Transformed = 71,
            PersonImages_Transformed = 72,
            MultiFanart_Transformed = 73,
            Trailer_Transformed = 74,
            Empty35_Transformed = 75,
            Empty36_Transformed = 76,
            Empty37_Transformed = 77,
            Empty38_Transformed = 78,
            Empty39_Transformed = 79
        }

        #region internal vars

        // list of the search results, containts objects of IMDBUrl
        private ArrayList _elements = new ArrayList();

        private List<IMDB.MovieInfoDatabase> _databaseList = new List<IMDB.MovieInfoDatabase>();

        private string BodyDetail2 = string.Empty;
        private string BodyLinkGeneric1 = string.Empty;
        private string BodyLinkGeneric2 = string.Empty;
        private string BodyLinkImg = string.Empty;
        private string BodyLinkPersons = string.Empty;
        private string BodyLinkTitles = string.Empty;
        private string BodyLinkCertification = string.Empty;
        private string BodyLinkComment = string.Empty;
        private string BodyLinkSyn = string.Empty;
        private string BodyLinkMultiPosters = string.Empty;
        private string BodyLinkPhotos = string.Empty;
        private string BodyLinkPersonImages = string.Empty;
        private string BodyLinkMultiFanart = string.Empty;
        private string BodyLinkTrailer = string.Empty;

        private string BodyDetail = string.Empty;

        #endregion

        public ArrayList ReturnURL(string strSearch, string strConfigFile, int strPage)
        {
          return ReturnURL(strSearch, strConfigFile, strPage, true);
        }
        public ArrayList ReturnURL(string strSearch, string strConfigFile, int strPage, bool AlwaysAsk)
        {
          return ReturnURL(strSearch, strConfigFile, strPage, true, string.Empty);
        }
        public ArrayList ReturnURL(string strSearch, string strConfigFile, int strPage, bool AlwaysAsk, string strMediaPath)
        {
          InitLogger();
          if (!string.IsNullOrEmpty(strMediaPath) || strSearch.Contains("\\")) // if a mediapath is given and file name is part of the search expression... assume it's nfo/xml/xbmc reader request and return the proper file to read in details
          {
            if (string.IsNullOrEmpty(strMediaPath) && strSearch.Contains("\\")) strMediaPath = strSearch;
            string strURLFile = string.Empty;
            string strDBName = string.Empty;
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;
            try { strDBName = XmlConvert.DecodeName(n.SelectSingleNode("DBName").InnerText); }
            catch { strDBName = "ERROR"; }
            try { strURLFile = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").InnerText); }
            catch { strURLFile = ""; }

            elements.Clear();
            if (!string.IsNullOrEmpty(strURLFile))
            {
              string directory = System.IO.Path.GetDirectoryName(strMediaPath); // get directory name of media file
              string filename = System.IO.Path.GetFileNameWithoutExtension(strMediaPath); // get filename without extension
              //strSearch = GrabUtil.encodeSearch(strSearch);
              strURLFile = strURLFile.Replace("#Filename#", filename);
              try
              {
                string[] files = Directory.GetFiles(directory, strURLFile, SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                  string fileShortname = System.IO.Path.GetFileName(file);
                  IMDBUrl url = new IMDBUrl(file, fileShortname, strDBName, n, "1", "", "", "", "", "", "", "", "");
                  elements.Add(url);
                }
              }
              catch (Exception e)
              {
                Log.Debug("MyFilms - GrabUtil - Catched Exception: " + e.ToString());
              }
              
            }
            return elements;
          }

          // if no local grabbing, do web grabbing:
          if (strPage == -1)
          {
            // First run, finding the key starting page number
            //Loading the configuration file
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;
            //Gets Key to the first page if it exists (not required)
            try
            { strPage = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartPage").InnerText)); }
            catch
            { strPage = 1; }
          }
          ArrayList listUrl = new ArrayList();
          Grabber_URLClass _Grab = new Grabber_URLClass();
          Int16 WIndex;
          do
          {
            if (strSearch.LastIndexOf(".") == strSearch.Length - 1)
              strSearch = strSearch.Substring(0, strSearch.Length - 1);
            else
              break;
          } while (true);
          _Grab.FindMovies(strSearch, strConfigFile, strPage, AlwaysAsk, out elements, out WIndex);
          if (WIndex >= 0)
          {
            Grabber_URLClass.IMDBUrl wurl = (Grabber_URLClass.IMDBUrl)elements[WIndex];
            elements.Clear();
            elements.Add(wurl);
          }
          return elements;
        }

        public class IMDBUrl
        {
            string m_strURL = "";
            string m_strTitle = "";
            string m_strDatabase = "";
            string m_strIMDBURL = "";
            XmlNode Node;
            // Guzzi Added for better matching internet searches:
            string m_strYear = "";
            string m_strDirector = "";
            string m_strIMDB_Id = "";
            string m_strTMDB_Id = "";
            string m_strID = "";
            string m_strOptions = "";
            string m_strAkas = "";
            string m_strThumb = "";

            public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode)
            {
                m_strURL = strURL;
                m_strTitle = strTitle;
                m_strDatabase = strDB;
                Node = pNode;
                m_strIMDBURL = string.Empty;
            }
            public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode, string strIMDBURL)
            {
              m_strURL = strURL;
              m_strTitle = strTitle;
              m_strDatabase = strDB;
              Node = pNode;
              m_strIMDBURL = strIMDBURL;
            }

            public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode, string strIMDBURL, string strYear, string strDirector, string strIMDB_Id, string strTMDB_Id, string strID, string strOptions, string strAkas, string strThumb)
            {
              m_strURL = strURL;
              m_strTitle = strTitle;
              m_strDatabase = strDB;
              Node = pNode;
              m_strIMDBURL = strIMDBURL;
              m_strYear = strYear;
              m_strDirector = strDirector;
              m_strIMDB_Id = strIMDB_Id;
              m_strTMDB_Id = strTMDB_Id;
              m_strID = strID;
              m_strOptions = strOptions;
              m_strAkas = strAkas;
              m_strThumb = strThumb;
            }

            public IMDBUrl(string strURL, string strTitle, string strDB)
            {
                URL = strURL;
                Title = strTitle;
                Database = strDB;
            }

            public string URL
            {
                get { return m_strURL; }
                set { m_strURL = value; }
            }

            public string Title
            {
                get { return m_strTitle; }
                set { m_strTitle = value; }
            }

            public string Database
            {
                get { return m_strDatabase; }
                set { m_strDatabase = value; }
            }

            public string IMDBURL
            {
                get { return m_strIMDBURL; }
                set { m_strIMDBURL = value; }
            }

            public XmlNode CurNode
            {
              get { return Node; }
              set { Node = value; }
            }

            public string Year
            {
              get { return m_strYear; }
              set { m_strYear = value; }
            }

            public string Director
            {
              get { return m_strDirector; }
              set { m_strDirector = value; }
            }

            public string IMDB_ID
            {
              get { return m_strIMDB_Id; }
              set { m_strIMDB_Id = value; }
            }

            public string TMDB_ID
            {
              get { return m_strTMDB_Id; }
              set { m_strTMDB_Id = value; }
            }
            public string ID
            {
              get { return m_strID; }
              set { m_strID = value; }
            }
            public string Options
            {
              get { return m_strOptions; }
              set { m_strOptions = value; }
            }
            public string Akas
            {
              get { return m_strAkas; }
              set { m_strAkas = value; }
            }
            public string Thumb
            {
              get { return m_strThumb; }
              set { m_strThumb = value; }
            }
        };

        public void FindMovies(string strSearchInit, string strConfigFile, int strPage, bool AlwaysAsk, out ArrayList ListUrl, out short WIndex)
        {
            InitLogger();
  
            WIndex = -1;
            string strSearch = strSearchInit;
            string strTemp = string.Empty;
            string strBody = string.Empty;
            string strItem = string.Empty;
            string strURL;
            string strEncoding = string.Empty;
            string strSearchCleanup = string.Empty;
            string strAccept = string.Empty;
            string strUserAgent = string.Empty;
            string strHeaders = string.Empty;
            string strLanguage = string.Empty;
            string strType = string.Empty;
            string strVersion = string.Empty;
            string strStart = string.Empty;
            string strEnd = string.Empty;
            string strNext = string.Empty;
            string absoluteUri;
            string strStartItem = string.Empty; // selected item for grabbing
            string strStartTitle = string.Empty;
            string strEndTitle = string.Empty;
            string strStartYear = string.Empty;
            string strEndYear = string.Empty;
            string strStartDirector = string.Empty;
            string strEndDirector = string.Empty;
            string strStartLink = string.Empty;
            string strEndLink = string.Empty;
            string strStartID = string.Empty;
            string strEndID = string.Empty;
            string strStartOptions = string.Empty;
            string strEndOptions = string.Empty;
            string strStartAkas = string.Empty;
            string strEndAkas = string.Empty;
            string strKeyAkasRegExp = string.Empty;
            string strStartThumb = string.Empty;
            string strEndThumb = string.Empty;
          
            string strTitle = string.Empty;
            string strYear = string.Empty;
            string strDirector = string.Empty;
            string strID = string.Empty;
            string strOptions = string.Empty;
            string strAkas = string.Empty;
            string strThumb = string.Empty;
            
            string strIMDB_Id = string.Empty;
            string strTMDB_Id = string.Empty;
            string strLink = string.Empty;
            string strDBName;
            string strStartPage = string.Empty;
            int wStepPage = 0;
            int iFinItem = 0;
            int iStartItemLength = 0; // added to keep the length of the current item
            int iStartTitle = 0;
            int iStartYear = 0;
            int iStartDirector = 0;
            int iStartID = 0;
            int iStartOptions = 0;
            int iStartAkas = 0;
            int iStartThumb = 0;
            int iStartUrl = 0;
            int iStart = 0;
            int iEnd = 0;
            int iLength = 0;
            string strRedir = string.Empty;
            string strParam1 = string.Empty;
            string strParam2 = string.Empty;
            elements.Clear();

            MediaPortal.Util.Utils.RemoveStackEndings(ref strSearchInit);

            // Regex creation with name of movie file
            byte[] bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(strSearchInit.ToLower());
            string file = System.Text.Encoding.ASCII.GetString(bytes);
            file = MediaPortal.Util.Utils.FilterFileName(file);
            file = file.Replace("-", " ");
            file = file.Replace("+", " ");
            file = file.Replace("!", " ");
            file = file.Replace("#", " ");
            file = file.Replace(";", " ");
            file = file.Replace(".", " ");
            file = file.Replace(",", " ");
            file = file.Replace("=", " ");
            file = file.Replace("&", " ");
            file = file.Replace("(", " ");
            file = file.Replace(")", " ");
            file = file.Replace("@", " ");
            file = file.Replace("%", " ");
            file = file.Replace("$", " ");
            file = file.Replace(":", " ");
            file = file.Replace("_", " ");
            file = file.Trim();
            System.Text.RegularExpressions.Regex oRegex = new System.Text.RegularExpressions.Regex(" +");
            file = oRegex.Replace(file, ":");
            file = file.Replace(":", ".*");
            oRegex = new System.Text.RegularExpressions.Regex(file);

            //Loading the configuration file
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;

            strDBName = n.SelectSingleNode("DBName").InnerText;
            if (strDBName.ToLower().StartsWith("ofdb") && strSearchInit.Length > 3) // Optimization for searches with ofdb
            {
              string strLeft = "";
              strLeft = strSearchInit.Substring(0, 3);
              if (strLeft.ToLower().Contains("der") || strLeft.ToLower().Contains("die") || strLeft.ToLower().Contains("das") || strLeft.ToLower().Contains("the"))
              {
                strSearchInit = strSearchInit.Substring(3).Trim() + ", " + strLeft.Trim();
                strSearch = strSearchInit;
              }
            }

            // retrieve manual encoding override, if any
            try { strEncoding = n.SelectSingleNode("Encoding").InnerText; }
            catch (Exception) { strEncoding = ""; }

            try // retrieve language, if any
            { strLanguage = n.SelectSingleNode("Language").InnerText; }
            catch (Exception) { strLanguage = ""; }

            try // retrieve type, if any
            { strType = n.SelectSingleNode("Type").InnerText; }
            catch (Exception) { strType = ""; }

            try // retrieve version, if any
            { strVersion = n.SelectSingleNode("Version").InnerText; }
            catch (Exception) { strVersion = ""; }

            try // retrieve SearchCleanupDefinition, if any
            { strSearchCleanup = n.SelectSingleNode("SearchCleanup").InnerText; }
            catch (Exception) { strSearchCleanup = ""; }

            try // retrieve SearchCleanupDefinition, if any
            { strAccept = n.SelectSingleNode("Accept").InnerText; }
            catch (Exception) { strAccept = ""; }

            try // retrieve SearchCleanupDefinition, if any
            { strUserAgent = n.SelectSingleNode("UserAgent").InnerText; }
            catch (Exception) { strUserAgent = ""; }

            try // retrieve SearchCleanupDefinition, if any
            { strHeaders = n.SelectSingleNode("Headers").InnerText; }
            catch (Exception) { strHeaders = ""; }

            //Retrieves the URL
            strURL = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").InnerText);
            strRedir = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").Attributes["Param1"].InnerText);

            strSearch = GrabUtil.CleanupSearch(strSearch, strSearchCleanup); // process SearchCleanup
            strSearch = GrabUtil.encodeSearch(strSearch, strEncoding); // Process Encoding of Search Expression

            strURL = strURL.Replace("#Search#", strSearch);

            //Retrieves the identifier of the next page
            strNext = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyNextPage").InnerText);
            strNext = strNext.Replace("#Search#", strSearch);
            //Récupère Le n° de la première page
            strStartPage = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartPage").InnerText);
            //Récupère Le step de page
            try { wStepPage = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStepPage").InnerText)); }
            catch { wStepPage = 1; }

            int wpage = strPage;
            int wpagedeb;
            int wpageprev = 0;
            //Fetch The No. of the first page
            try { wpagedeb = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartPage").InnerText)); }
            catch { wpagedeb = 1; }
            if (wpage - wStepPage < wpagedeb)
                wpageprev = -1;
            else
                wpageprev = wpage - wStepPage;
            /******************************/
            /* Search titles and links
            /******************************/

            //Gets Key to the first page if it exists (not required)...
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartList").InnerText);
            //Récupère La clé de fin de liste
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndList").InnerText);

            //Récupère La clé de début du titre KeyStartTitle
            strStartTitle = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartTitle").InnerText);
            //Récupère La clé de fin du titre KeyEndTitle
            strEndTitle = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndTitle").InnerText);
            //Récupère La clé de début de l'année KeyStartYear
            strStartYear = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartYear").InnerText);
            //Récupère La clé de fin de l'année KeyEndYear
            strEndYear = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndYear").InnerText);
            //Récupère La clé de début du réalisateur KeyStartDirector
            strStartDirector = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartDirector").InnerText);
            //Récupère La clé de fin du réalisateur KeyEndDirector
            strEndDirector = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndDirector").InnerText);
            //Récupère La clé de début du lien KeyStartLink
            strStartLink = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartLink").InnerText);
            //Récupère La clé de fin du lien KeyEndLink
            strEndLink = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndLink").InnerText);
            // Load startkey and endkey for ID
            strStartID = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartID").InnerText);
            strEndID = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndID").InnerText);
            // Load startkey and endkey for Options
            strStartOptions = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartOptions").InnerText);
            strEndOptions = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndOptions").InnerText);
            // Load startkey and endkey for Akas
            strStartAkas = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartAkas").InnerText);
            strEndAkas = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndAkas").InnerText);
            strKeyAkasRegExp = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyAkasRegExp").InnerText);
            // Load startkey and endkey for Thumb
            strStartThumb = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartThumb").InnerText);
            strEndThumb = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyEndThumb").InnerText);

            CookieContainer cook = new CookieContainer();

            //Récupère la page wpage
            strBody = GrabUtil.GetPage(strURL.Replace("#Page#", wpage.ToString()), strEncoding, out absoluteUri, cook, strHeaders, strAccept, strUserAgent);
            //redirection auto : 1 résult
            if (!absoluteUri.Equals(strURL.Replace("#Page#", wpage.ToString())))
            {
                IMDBUrl url = new IMDBUrl(absoluteUri, strSearchInit + " (AutoRedirect)", strDBName, null, wpage.ToString());
                elements.Add(url);
                ListUrl = elements;
                WIndex = 0;
                return;
            }

            if (strRedir.Length > 0)
              strBody = GrabUtil.GetPage(strRedir, strEncoding, out absoluteUri, cook, strHeaders, strAccept, strUserAgent);

            wpage += wStepPage;


            /******************************/
            /* Cutting the list
            /******************************/
            // If you have at least the key to start, we cut strBody
            iStart = 0;
            iEnd = 0;
            iLength = 0;

            if (strStart.Length > 0)
            {
                iStart = GrabUtil.FindPosition(strBody, strStart, iStart, ref iLength, true, true);
                if (iStart < 0) iStart = 0;
            }
            if (strEnd.Length > 0)
            {
                iEnd = GrabUtil.FindPosition(strBody, strEnd, iStart, ref iLength, true, false);
                if (iEnd <= 0) iEnd = strBody.Length;
            }

            // Cutting the body
            try { strBody = strBody.Substring(iStart, iEnd - iStart); }
            catch { }

            // Now grab the search data from stripped search page !
            iStart = 0;
            iFinItem = 0;
            iStartTitle = 0;
            iStartUrl = 0;
            iLength = 0;
            IMDBUrl urlprev = new IMDBUrl(strURL.Replace("#Page#", wpageprev.ToString()), "---", strDBName, n, wpageprev.ToString());

            if (strBody != "")
            {
                // Comparison between the position of URL and title to boundary elements //if (strBody.IndexOf(strStartTitle, 0) > strBody.IndexOf(strStartLink, 0))
                int iPosStartTitle = 0; iPosStartTitle = GrabUtil.FindPosition(strBody, strStartTitle, iPosStartTitle, ref iLength, true, false); 
                int iPosStartLink = 0; iPosStartLink = GrabUtil.FindPosition(strBody, strStartLink, iPosStartLink, ref iLength, true, false);
                if (iPosStartTitle > iPosStartLink) strStartItem = strStartLink;
                else strStartItem = strStartTitle;

                // set start position for all elements (lowest possible position found)
                iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem, ref iLength, true, false);
                iStartItemLength = iLength;
                
                // iFinItem += strStartItem.Length;
                iStartYear = iStartDirector = iStartUrl = iStartTitle = iStartID = iStartOptions = iStartAkas = iStartThumb = iFinItem;

                while (true)
                {
                    // determining the end of nth Item (if the index fields are higher then found => no info for this item
                    if (iFinItem <= 0) break;
                    //iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + strStartItem.Length, ref iLength, true, false);
                    iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + iStartItemLength, ref iLength, true, false);
                    iStartItemLength = iLength;
                    // Initialisation 

                    // Read Movie Title
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartTitle").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartTitle").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                        strTitle = GrabUtil.FindWithAction(strBody, strStartTitle, ref iStartTitle, strEndTitle, strParam1, strParam2).Trim();
                    else
                        strTitle = GrabUtil.Find(strBody, strStartTitle, ref iStartTitle, strEndTitle).Trim();

                    if (strTitle.Length == 0)
                        break;
                    
                    // Reorder article for ofdb to beginning
                    if (strDBName.ToLower().StartsWith("ofdb") && strTitle.Length > 3) // Optimization for searches with ofdb
                    {
                      string strRight = "";
                      strRight = strTitle.Substring(strTitle.Length -3);
                      if (strRight.ToLower().Contains("der") || strRight.ToLower().Contains("die") || strRight.ToLower().Contains("das") || strRight.ToLower().Contains("the"))
                      {

                        strTitle = strRight.Trim() + " " + strTitle.Substring(0, strTitle.Length - 3).Trim().Trim(',');
                      }
                    }
                    
                    // Title outbound range Item re-delimit range item
                    if ((iStartTitle > iFinItem) && (iFinItem != -1))
                    {
                      iStartYear = iStartDirector = iStartUrl = iStartTitle = iStartID = iStartOptions = iStartAkas = iStartThumb = iFinItem;
                        //iFinItem = strBody.IndexOf(strStartItem, iFinItem + strStartItem.Length);
                      //iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + strStartItem.Length, ref iLength, true, false);
                      iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + iStartItemLength, ref iLength, true, false);
                      iStartItemLength = iLength;
                      if (iFinItem <= 0)
                            break;
                    }

                    // read movie year
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartYear").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartYear").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                        strYear = GrabUtil.FindWithAction(strBody, strStartYear, ref iStartYear, strEndYear, strParam1, strParam2).Trim();
                    else
                        strYear = GrabUtil.Find(strBody, strStartYear, ref iStartYear, strEndYear).Trim();

                    if ((strYear.Length == 0) || ((iStartYear > iFinItem) && !(iFinItem == -1)))
                        strYear = string.Empty;

                    // read movie director
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartDirector").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartDirector").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                        strDirector = GrabUtil.FindWithAction(strBody, strStartDirector, ref iStartDirector, strEndDirector, strParam1, strParam2).Trim();
                    else
                        strDirector = GrabUtil.Find(strBody, strStartDirector, ref iStartDirector, strEndDirector).Trim();

                    if ((strDirector.Length == 0) || ((iStartDirector > iFinItem) && !(iFinItem == -1)))
                        strDirector = string.Empty;

                    // read movie ID
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartID").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartID").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                      strID = GrabUtil.FindWithAction(strBody, strStartID, ref iStartID, strEndID, strParam1, strParam2).Trim();
                    else
                      strID = GrabUtil.Find(strBody, strStartID, ref iStartID, strEndID).Trim();

                    if ((strID.Length == 0) || ((iStartID > iFinItem) && !(iFinItem == -1)))
                      strID = string.Empty;

                    // read movie Options
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartOptions").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartOptions").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                      strOptions = GrabUtil.FindWithAction(strBody, strStartOptions, ref iStartOptions, strEndOptions, strParam1, strParam2).Trim();
                    else
                      strOptions = GrabUtil.Find(strBody, strStartOptions, ref iStartOptions, strEndOptions).Trim();

                    if ((strOptions.Length == 0) || ((iStartOptions > iFinItem) && iFinItem != -1))
                      strOptions = string.Empty;

                    // read movie Akas
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartAkas").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartAkas").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                      strAkas = GrabUtil.FindWithAction(strBody, strStartAkas, ref iStartAkas, strEndAkas, strParam1, strParam2).Trim();
                    else
                      strAkas = GrabUtil.Find(strBody, strStartAkas, ref iStartAkas, strEndAkas).Trim();

                    if ((strAkas.Length == 0) || ((iStartAkas > iFinItem) && !(iFinItem == -1)))
                      strAkas = string.Empty;

                    // read movie Thumb
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartThumb").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartThumb").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                      strThumb = GrabUtil.FindWithAction(strBody, strStartThumb, ref iStartThumb, strEndThumb, strParam1, strParam2).Trim();
                    else
                      strThumb = GrabUtil.Find(strBody, strStartThumb, ref iStartThumb, strEndThumb).Trim();

                    if ((strThumb.Length == 0) || ((iStartThumb > iFinItem) && iFinItem != -1))
                      strThumb = string.Empty;

                    // create akas string with titles
                    if (!String.IsNullOrEmpty(strKeyAkasRegExp)) // strKeyAkasRegExp = @"aka." + "\"" + ".*?" + "\"" + ".-";
                    {
                      strTemp = strAkas;
                      strTemp = HttpUtility.HtmlDecode(strTemp);
                      strTemp = HttpUtility.HtmlDecode(strTemp).Replace("\n", "");
                      Regex p = new Regex(strKeyAkasRegExp, RegexOptions.Singleline);
                      iLength = 0;

                      MatchCollection MatchList = p.Matches(strTemp);
                      if (MatchList.Count > 0)
                      {
                        string matchstring = "";
                        foreach (Match match in MatchList)
                        {
                          if (matchstring.Length > 0) matchstring += " | " + match.Groups["aka"].Value;
                          else matchstring = match.Groups["aka"].Value;
                        }
                        if (matchstring.Length > 0) 
                          strAkas = matchstring;
                      }
                      // else strAkas = ""; 
                    }
                  
                    // read movie url
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartLink").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartLink").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                        strLink = GrabUtil.FindWithAction(strBody, strStartLink, ref iStartUrl, strEndLink, strParam1, strParam2).Trim();
                    else
                        strLink = GrabUtil.Find(strBody, strStartLink, ref iStartUrl, strEndLink).Trim();

                    if (strLink.Length != 0)
                    {
                        // break;
                        // check, if IMDB id is existing
                        if (strLink.Contains(@"/tt"))
                        {
                          strIMDB_Id = strLink.Substring(strLink.IndexOf(@"/tt") + 1);
                          strIMDB_Id = strIMDB_Id.Substring(0, strIMDB_Id.IndexOf(@"/"));
                          // Fix for redirection on AKAS site on IMDB
                          if (strLink.Contains("onclick"))
                            strLink = strLink.Substring(0, strLink.IndexOf("onclick"));
                        }
                        // check, if TMDB id is existing
                        if (strLink.Contains(@"themoviedb.org/movie/"))
                        {
                          strIMDB_Id = strLink.Substring(strLink.IndexOf(@"themoviedb.org/movie/") + 20); // assign TMDB ID
                        }
                        if (!strLink.StartsWith("http://") && !strLink.StartsWith("www."))
                        {
                            //si les liens sont relatifs on rajoute le préfix (domaine)
                            strLink = XmlConvert.DecodeName(n.SelectSingleNode("URLPrefix").InnerText + strLink);
                        }

                        //Ajout http:// s'il n'y est pas (Pour GetPage)
                        if (strLink.StartsWith("www."))
                            strLink = "http://" + strLink;

                        //Added new element, we pass the node of xml file to find the details

                        //IMDBUrl url = new IMDBUrl(strLink, strTitle + " (" + strYear + ") " + strDirector, strDBName, n, wpage.ToString(), strYear, strDirector, strIMDB_Id, strTMDB_Id) ;
                        IMDBUrl url = new IMDBUrl(strLink, strTitle, strDBName, n, wpage.ToString(), strYear, strDirector, strIMDB_Id, strTMDB_Id, strID, strOptions, strAkas, strThumb);  
                        bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(strTitle.ToLower());
                        System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(System.Text.Encoding.ASCII.GetString(bytes));
                        if (oMatches.Count > 0)
                            if (AlwaysAsk)
                                WIndex = -2;
                            else
                                if (WIndex == -1)
                                    WIndex = (short)elements.Count;
                                else
                                    WIndex = -2;

                        if ((elements.Count == 0) && (strNext.Length > 0) && (strBody.Contains(strNext.Replace("#Page#", wpageprev.ToString()))) && !(wpageprev < 0))
                            elements.Add(urlprev);
                        elements.Add(url);
                    }
                    // init new search indexes
                    iStartYear = iStartDirector = iStartUrl = iStartTitle = iStartID = iStartOptions = iStartAkas = iStartThumb = iFinItem;
                }
            }
            IMDBUrl urlsuite = new IMDBUrl(strURL.Replace("#Page#", wpage.ToString()), "+++", strDBName, n, wpage.ToString());

            if ((strBody.Contains(strNext.Replace("#Page#", wpage.ToString()))) && (strNext.Length > 0))
                elements.Add(urlsuite);

            ListUrl = elements;
        }
        
        public string[] GetDetail(string strURL, string strPathImg, string strConfigFile)
        {
            return GetDetail(strURL, strPathImg, strConfigFile, true);
        }
        public string[] GetDetail(string strURL, string strPathImg, string strConfigFile, bool SaveImage)
        {
          return GetDetail(strURL, strPathImg, strConfigFile, true, string.Empty, string.Empty, string.Empty, string.Empty);
        }
        public string[] GetDetail(string strURL, string strPathImg, string strConfigFile, bool SaveImage, string PreferredLanguage, string PersonLimit, string TitleLimit, string GetRoles)
        {
            InitLogger();

            string[] datas = new string[80]; // 0-39 = original fields, 40-79 mapped fields
            elements.Clear();
            string strTemp = string.Empty;
            //string strBody = string.Empty;
            //string strBodyDetails2 = string.Empty;
            //string strBodyPersons = string.Empty;
            //string strBodyTitles = string.Empty;
            //string strBodyCertification = string.Empty;
            //string strBodyComment = string.Empty;
            //string strBodyDescription = string.Empty;
            //string strBodyCover = string.Empty;

            string strEncoding = string.Empty; // added for manual encoding override (global setting for all webgrabbing)

            string strAccept = string.Empty;
            string strUserAgent = string.Empty;
            string strHeaders = string.Empty;
          
            string strStart = string.Empty;
            string strEnd = string.Empty;
            string strIndex = string.Empty;
            string strPage = string.Empty;

            string strActivePage = string.Empty;
            string absoluteUri;
            string strStartTitle = string.Empty;
            string strEndTitle = string.Empty;
            string strStartLink = string.Empty;
            string strEndLink = string.Empty;
            string strTitle = string.Empty;
            string strLink = string.Empty;
            string strRate = string.Empty;
            string strRate2 = string.Empty;
            string strBasedRate = string.Empty;
            string strParam1 = string.Empty;
            string strParam3 = string.Empty;
            string strParam2 = string.Empty;
            string strMaxItems = string.Empty;
            string strLanguage = string.Empty;
            string strGrabActorRoles = string.Empty;
            bool boolGrabActorRoles = false;
            string strLinkCover = string.Empty;
            string allNames = string.Empty;
            string allRoles = string.Empty;
            int iStart = 0;
            int iEnd = 0;

            // Reset all webpage content
            BodyDetail = string.Empty;
            BodyDetail2 = string.Empty;
            BodyLinkGeneric1 = string.Empty;
            BodyLinkGeneric2 = string.Empty;
            BodyLinkImg = string.Empty;
            BodyLinkPersons = string.Empty;
            BodyLinkTitles = string.Empty;
            BodyLinkCertification = string.Empty;
            BodyLinkComment = string.Empty;
            BodyLinkSyn = string.Empty;
            BodyLinkMultiPosters = string.Empty;
            BodyLinkPhotos = string.Empty;
            BodyLinkPersonImages = string.Empty;
            BodyLinkMultiFanart = string.Empty;
            BodyLinkTrailer = string.Empty;

            // Recovery parameters
            // Load the configuration file
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;

            try { strEncoding = n.SelectSingleNode("Encoding").InnerText; }
            catch (Exception) { strEncoding = ""; }  
            try { strAccept = n.SelectSingleNode("Accept").InnerText; }
            catch (Exception) { strAccept = ""; }  
            try { strUserAgent = n.SelectSingleNode("UserAgent").InnerText; }
            catch (Exception) { strUserAgent = ""; }  
            try { strHeaders = n.SelectSingleNode("Headers").InnerText; }
            catch (Exception) { strHeaders = ""; }  

            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartBody").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndBody").InnerText);


            //Fetch the basic Details page

            if (strURL.ToLower().StartsWith("http"))
              BodyDetail = GrabUtil.GetPage(strURL, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            else
              BodyDetail = this.ReadFile(strURL); // Read page from file !
            
            HTMLUtil htmlUtil = new HTMLUtil();

            //Si on a au moins la clé de début, on découpe StrBody
            if (strStart != "")
            {
              iStart = BodyDetail.IndexOf(strStart);

                //Si la clé de début a été trouvé
                if (iStart > 0)
                {

                    //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
                    if (strEnd != "")
                    {
                      iEnd = BodyDetail.IndexOf(strEnd, iStart);
                    }
                    else
                      iEnd = BodyDetail.Length;

                    //Découpage du body
                    iStart += strStart.Length;
                    if (iEnd - iStart > 0)
                      BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
                }
            }

            // ***** Load Sub Pages into Memory ***** // Will be used for Secondary Website Infos!

            // ***** URL Redirection Details 2 Base Page ***** // Will be used for Secondary Website Infos!
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartDetails2").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndDetails2").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartDetails2").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartDetails2").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyDetails2Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyDetails2Page").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
              {
                if (strParam1.Length > 0)
                  strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
                else
                  strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
                BodyDetail2 = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
              }
            else
              BodyDetail2 = "";

            // ***** URL Redirection Generic 1 *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkGeneric1").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkGeneric1").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkGeneric1").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkGeneric1").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkGeneric1Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkGeneric1Page").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkGeneric1 = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkGeneric1 = "";

            // ***** URL Redirection Generic 2 *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkGeneric2").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkGeneric2").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkGeneric2").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkGeneric2").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkGeneric2Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkGeneric2Page").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkGeneric2 = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkGeneric2 = "";

            // ***** URL Redirection IMG *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkImg").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkImg").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkImg").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkImg").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkImgIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkImgPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkImg = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkImg = "";

            // ***** URL Redirection Persons ***** // Will be used for persons, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersons").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkPersons").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersons").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersons").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPersonsIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPersonsPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
              {
                if (strParam1.Length > 0)
                  strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
                else
                  strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
                BodyLinkPersons = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
              }
            else
              BodyLinkPersons = "";

            // ***** URL Redirection Titles ***** // Will be used for TTitle, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTitles").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkTitles").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTitles").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTitles").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkTitlesIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkTitlesPage").InnerText);
            strActivePage = this.LoadPage(strPage); 
            if (strStart.Length > 0)
              {
                if (strParam1.Length > 0)
                  strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
                else
                  strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
                BodyLinkTitles = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
              }
            else
              BodyLinkTitles = "";

            // ***** URL Redirection Certification ***** // Will be used for Certification Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkCertification").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkCertification").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkCertification").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkCertification").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkCertificationIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkCertificationPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkCertification = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkCertification = "";

            // ***** URL Redirection Comment ***** // Will be used for Comment Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkComment").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkComment").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkComment").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkComment").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkCommentIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkCommentPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkComment = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkComment = "";

            // ***** URL Redirection Description ***** // Will be used for Description Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkSyn").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkSyn").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkSyn").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkSyn").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkSynIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkSynPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkSyn = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkSyn = "";

            // ***** URL Redirection MultiPosters ***** // Will be used for MultiPosters Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkMultiPosters").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkMultiPosters").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkMultiPosters").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkMultiPosters").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkMultiPostersIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkMultiPostersPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkMultiPosters = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkMultiPosters = "";

            // ***** URL Redirection Photos ***** // Will be used for Photos Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPhotos").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkPhotos").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPhotos").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPhotos").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPhotosIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPhotosPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkPhotos = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkPhotos = "";

            // ***** URL Redirection PersonImages ***** // Will be used for PersonImages Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersonImages").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkPersonImages").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersonImages").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersonImages").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPersonImagesIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPersonImagesPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkPersonImages = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkPersonImages = "";

            // ***** URL Redirection MultiFanart ***** // Will be used for MultiFanart Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkMultiFanart").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkMultiFanart").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkMultiFanart").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkMultiFanart").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkMultiFanartIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkMultiFanartPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkMultiFanart = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkMultiFanart = "";

            // ***** URL Redirection Trailer ***** // Will be used for Trailer Details, if available !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTrailer").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkTrailer").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTrailer").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTrailer").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkTrailerIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkTrailerPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
              BodyLinkTrailer = GrabUtil.GetPage(strTemp, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
            }
            else
              BodyLinkTrailer = "";


            // ************* Now get the detail fields ***************************

            // ***** Original TITLE *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndOTitle").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyOTitleIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyOTitlePage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strParam1.Length > 0)
              strTitle = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTitle = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTitle = strTitle.Replace("\n", "");

            if (strTitle.Length > 0)
              datas[(int)Grabber_Output.OriginalTitle] = strTitle;
            else
              datas[(int)Grabber_Output.OriginalTitle] = "";


            // ***** Translated TITLE *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTTitle").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitlePage").InnerText);
            try
            {strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleMaxItems").InnerText);}
            catch (Exception) {strMaxItems = "";}
            try
            {strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleLanguage").InnerText);}
            catch (Exception) {strLanguage = "";}
            strActivePage = this.LoadPage(strPage);
            
            // Overrides from MyFilms plugin:
            if (!string.IsNullOrEmpty(PreferredLanguage)) 
              strLanguage = PreferredLanguage;
            if (!string.IsNullOrEmpty(TitleLimit)) 
              strMaxItems = TitleLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTitle = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
            else
              strTitle = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTitle = strTitle.Replace("\n", "");
            if (strTitle.Length > 0)
                //Translated Title
              datas[(int)Grabber_Output.TranslatedTitle] = strTitle;
            else
              datas[(int)Grabber_Output.TranslatedTitle] = "";
            //else
            //    datas[(int)Grabber_Output.TranslatedTitle] = datas[(int)Grabber_Output.OriginalTitle];
            //if (datas[(int)Grabber_Output.OriginalTitle].Length == 0)
            //    datas[(int)Grabber_Output.OriginalTitle] = datas[(int)Grabber_Output.TranslatedTitle];
            datas[(int)Grabber_Output.TranslatedTitleAllNames] = allNames;
            datas[(int)Grabber_Output.TranslatedTitleAllValues] = allRoles;

            // ***** URL for Image *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndImg").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyImgIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyImgPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
            {
                //datas[(int)Grabber_Output.URL] = strTemp;
                //Picture
                if (SaveImage == true)
                {
                  if (string.IsNullOrEmpty(datas[(int)Grabber_Output.OriginalTitle]))
                    GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[(int)Grabber_Output.TranslatedTitle], out strTemp);
                    else
                    GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[(int)Grabber_Output.OriginalTitle], out strTemp);
                    // strTemp = MediaPortal.Util.Utils.FilterFileName(strTemp); // Guzzi: removed, as it could change the filename to an already loaded image, thus breaking the "link".
                  datas[(int)Grabber_Output.PicturePathLong] = strPathImg + "\\" + strTemp;
                }
                datas[(int)Grabber_Output.PicturePathShort] = strTemp;
            }

            // ***** Synopsis ***** Description
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndSyn").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeySynIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeySynPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            // strTemp = strTemp.Replace("\"", "\'");
            if (strTemp.Length > 0)
              //Description
              datas[(int)Grabber_Output.Description] = strTemp;

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ",";

            // ***** Base rating *****
            strBasedRate = XmlConvert.DecodeName(n.SelectSingleNode("Details/BaseRating").InnerText);
            decimal wRate = 0;
            decimal wRate2 = 0;
            decimal wBasedRate = 10;
            try
            { wBasedRate = Convert.ToDecimal(strBasedRate, provider); }
            catch
            { }
            // ***** NOTE 1 ***** Rating 1
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndRate").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRateIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRatePage").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strParam1.Length > 0) 
              strRate = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else 
              strRate = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strRate = GrabUtil.convertNote(strRate);
            try
            { wRate = (Convert.ToDecimal(strRate, provider) / wBasedRate) * 10; }
            catch
            { }

            // ***** NOTE 2 ***** Rating 2
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate2").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndRate2").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate2").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate2").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRate2Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRate2Page").InnerText);
            strActivePage = this.LoadPage(strPage);
            if (strParam1.Length > 0)
              strRate2 = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strRate2 = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strRate2 = GrabUtil.convertNote(strRate2);

            try
            { wRate2 = (Convert.ToDecimal(strRate2, provider) / wBasedRate) * 10; }
            catch
            { }

            //Calcul de la moyenne des notes.
            decimal resultRate;
            if (wRate > 0 && wRate2 > 0)
              resultRate = ((wRate + wRate2) / 2);
            else
                if (wRate == 0 && wRate2 == 0)
                  resultRate = -1;
                else
                  resultRate = ((wRate + wRate2));

            resultRate = Math.Round(resultRate, 1);
            if (resultRate == -1) strRate = "";
            else strRate = Convert.ToString(resultRate);

            //Rating (calculated from Rating 1 and 2)
            strRate = strRate.Replace(",", ".");
            datas[(int)Grabber_Output.Rating] = strRate.Replace(",", ".");

            // ***** Acteurs ***** Actors
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCredits").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsMaxItems").InnerText);
            strGrabActorRoles = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsGrabActorRoles").InnerText);
            if (strGrabActorRoles == "true") boolGrabActorRoles = true;
            else boolGrabActorRoles = false;
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            strLanguage = "";

            // Overrides from MyFilms plugin:
            if (!string.IsNullOrEmpty(PersonLimit))
              strMaxItems = PersonLimit.ToString();
            if (!string.IsNullOrEmpty(GetRoles))
            {
              if (GetRoles.ToLower() == "true") boolGrabActorRoles = true;
              if (GetRoles.ToLower() == "false") boolGrabActorRoles = false;
            }

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0) // 
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out allNames, out allRoles, boolGrabActorRoles).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            strTemp = GrabUtil.trimSpaces(strTemp);
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Actors] = strTemp;

            // ***** Réalisateur ***** = Director 
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndRealise").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseMaxItems").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealisePage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            if (!string.IsNullOrEmpty(PersonLimit))
              strMaxItems = PersonLimit.ToString();

            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Director] = strTemp;

            // ***** Producteur ***** Producer // Producers also using MiltiPurpose Secondary page !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndProduct").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductMaxItems").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            if (!string.IsNullOrEmpty(PersonLimit))
              strMaxItems = PersonLimit.ToString();

            if (strParam1.Length > 0 || strParam3.Length > 0) // Guzzi: Added param3 to execute matchcollections also !
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Producer] = strTemp;

            // ***** Année ***** Year
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndYear").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyYearIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyYearPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              if (strTemp.Length >= 4)
                datas[(int)Grabber_Output.Year] = strTemp.Substring(strTemp.Length - 4, 4);
              else
                datas[(int)Grabber_Output.Year] = strTemp; // fallback, if scraping failed

            // ***** Pays ***** Country
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCountry").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCountryRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCountryIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCountryPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
            {
              strTemp = strTemp.Replace(".", " ");
              strTemp = GrabUtil.transformCountry(strTemp);
              datas[(int)Grabber_Output.Country] = strTemp; 
            }


            // ***** Genre *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndGenre").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGenreRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGenreIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGenrePage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Category] = strTemp;

            // ***** URL *****
            datas[(int)Grabber_Output.URL] = strURL;


            // ***** Writer ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartWriter").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndWriter").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartWriter").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartWriter").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterMaxItems").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            if (!string.IsNullOrEmpty(PersonLimit))
              strMaxItems = PersonLimit.ToString();

            if (strParam1.Length > 0 || strParam3.Length > 0) // Guzzi: Added param3 to execute matchcollections also !
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Writer] = strTemp;

            // ***** Comment *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndComment").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCommentRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCommentIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCommentPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", " "); // Guzzi: Replace linebreaks with space
            // strTemp = strTemp.Replace("\"", "\'");

            if (strTemp.Length > 0)
              //Comment
              datas[(int)Grabber_Output.Comments] = strTemp;

            // ***** Language *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLanguage").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLanguageIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLanguagePage").InnerText); 
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Language] = strTemp;

            // ***** Tagline *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTagline").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTaglineIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTaglinePage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Tagline] = strTemp;

          
            // ***** Certification *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCertification").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationPage").InnerText);
            strActivePage = this.LoadPage(strPage);
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }

            // Overrides from MyFilms plugin:
            if (!string.IsNullOrEmpty(PreferredLanguage))
              strLanguage = PreferredLanguage;
          
            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, "", strLanguage, out allNames, out allRoles).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Certification] = strTemp;
            datas[(int)Grabber_Output.CertificationAllNames] = allNames;
            datas[(int)Grabber_Output.CertificationAllValues] = allRoles;

            // ***** IMDB_Id *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartIMDB_Id").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndIMDB_Id").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartIMDB_Id").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartIMDB_Id").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyIMDB_IdIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyIMDB_IdPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.IMDB_Id] = strTemp;

            // ***** TMDB_Id *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTMDB_Id").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTMDB_Id").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTMDB_Id").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTMDB_Id").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTMDB_IdIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTMDB_IdPage").InnerText); 
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0 && string.IsNullOrEmpty(datas[(int)Grabber_Output.IMDB_Id]))
              datas[(int)Grabber_Output.IMDB_Id] = strTemp;

            // ***** IMDB_Rank *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartIMDB_Rank").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndIMDB_Rank").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartIMDB_Rank").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartIMDB_Rank").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyIMDB_RankIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyIMDB_RankPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.IMDBrank] = strTemp;


            // ***** Studio *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartStudio").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndStudio").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartStudio").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartStudio").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStudioIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStudioPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Studio] = strTemp;

            // ***** Edition *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartEdition").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndEdition").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartEdition").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartEdition").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEditionIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEditionPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Edition] = strTemp;

            // ***** Fanart ***** //
            datas[(int)Grabber_Output.Fanart] = ""; // Fanart - only for file grabber
          
            // ***** Generic Field 1 ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric1").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndGeneric1").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric1").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric1").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric1RegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric1MaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric1Language").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric1Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric1Page").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Generic1] = strTemp;
            else
              datas[(int)Grabber_Output.Generic1] = "";

            // ***** Generic Field 2 ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric2").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndGeneric2").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric2").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric2").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric2RegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric2MaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric2Language").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric2Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric2Page").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Generic2] = strTemp;
            else
              datas[(int)Grabber_Output.Generic2] = "";


            // ***** Generic Field 3 ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric3").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndGeneric3").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric3").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGeneric3").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric3RegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric3MaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric3Language").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric3Index").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGeneric3Page").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Generic3] = strTemp;
            else
              datas[(int)Grabber_Output.Generic3] = "";


            // ***********************************
            // new key-value listingoutputs
            // ***********************************

            // ***** MultiPosters ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartMultiPosters").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndMultiPosters").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartMultiPosters").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartMultiPosters").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiPostersRegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiPostersMaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiPostersLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiPostersIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiPostersPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.MultiPosters] = strTemp;
            else
              datas[(int)Grabber_Output.MultiPosters] = "";

            // ***** Photos ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartPhotos").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndPhotos").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartPhotos").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartPhotos").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPhotosRegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPhotosMaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPhotosLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPhotosIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPhotosPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Photos] = strTemp;
            else
              datas[(int)Grabber_Output.Photos] = "";

            // ***** PersonImages ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartPersonImages").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndPersonImages").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartPersonImages").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartPersonImages").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPersonImagesRegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPersonImagesMaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPersonImagesLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPersonImagesIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyPersonImagesPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.PersonImages] = strTemp;
            else
              datas[(int)Grabber_Output.PersonImages] = "";

            // ***** MultiFanart ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartMultiFanart").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndMultiFanart").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartMultiFanart").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartMultiFanart").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiFanartRegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiFanartMaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiFanartLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiFanartIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyMultiFanartPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.MultiFanart] = strTemp;
            else
              datas[(int)Grabber_Output.MultiFanart] = "";

            // ***** Trailer ***** //
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTrailer").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTrailer").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTrailer").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTrailer").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTrailerRegExp").InnerText);
            try
            { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTrailerMaxItems").InnerText); }
            catch (Exception) { strMaxItems = ""; }
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTrailerLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTrailerIndex").InnerText);
            strPage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTrailerPage").InnerText);
            strActivePage = this.LoadPage(strPage);

            // Overrides from MyFilms plugin:
            //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
            //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
            //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

            allNames = string.Empty;
            allRoles = string.Empty;
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              datas[(int)Grabber_Output.Trailer] = strTemp;
            else
              datas[(int)Grabber_Output.Trailer] = "";

          
            // **********************************************************************************************************
            // Do mapping, if any configured...
            // **********************************************************************************************************
            string[] source = new string[40];
            string[] destination = new string[40];
            bool[] replace = new bool[40];
            bool[] addStart = new bool[40];
            bool[] addEnd = new bool[40];
            bool[] mergePreferSource = new bool[40];
            bool[] mergePreferDestination = new bool[40];
            // Read Config
            for (int t = 0; t < 39; t++)
            {
              source[t] = XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["source"].InnerText);
              destination[t] = XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["destination"].InnerText);
              replace[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["replace"].InnerText));
              addStart[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["addstart"].InnerText));
              addEnd[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["addend"].InnerText));
              mergePreferSource[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["mergeprefersource"].InnerText));
              mergePreferDestination[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNode("Mapping/Field_" + t.ToString()).Attributes["mergepreferdestination"].InnerText));
            }

            for (int t = 0; t < 39; t++) // set default values = source
            {
              if (!string.IsNullOrEmpty(datas[t])) 
                datas[t + 40] = datas[t]; // set destination = soutrce as default (base if no other transformations)
              else 
                datas[t + 40] = string.Empty;
            }
            for (int t = 0; t < 39; t++) // replace values if configured
            {
              for (int i = 0; i < 39; i++) // search for mapping destination
              {
                if (destination[t] == source[i]) // found mapping destination -> datas[i] is destination object !
                {
                  if (replace[t]) // replace action
                    if (!String.IsNullOrEmpty(datas[t]))
                      datas[i + 40] = datas[t];
                    else
                      datas[i + 40] = string.Empty;
                }
              }
            }
            for (int t = 0; t < 39; t++) // merge prefer source - replace values only if source is not empty
            {
              for (int i = 0; i < 39; i++)
              {
                if (destination[t] == source[i])
                {
                  if (mergePreferSource[t] && !string.IsNullOrEmpty(datas[t])) // replace action
                    datas[i + 40] = datas[t];
                }
              }
            }
            for (int t = 0; t < 39; t++) // merge prefer destination - replace values only if destination empty
            {
              for (int i = 0; i < 39; i++)
              {
                if (destination[t] == source[i])
                {
                  if (mergePreferDestination[t] && string.IsNullOrEmpty(datas[i])) // replace action
                    datas[i + 40] = datas[t];
                }
              }
            }
            for (int t = 0; t < 39; t++) // insert or append values if configured
            {
              for (int i = 0; i < 39; i++) // search for mapping destination
              {
                if (destination[t] == source[i]) // found mapping destination -> datas[i] is destination object !
                {
                  if (addStart[t] && !string.IsNullOrEmpty(datas[t])) // addStart action - only of not empty (avoid empty new line)
                    datas[i + 40] = datas[t] + System.Environment.NewLine + datas[i + 40];
                  if (addEnd[t] && !string.IsNullOrEmpty(datas[t])) // addEnd action - only if not empty (avoid empty new line)
                    datas[i + 40] = datas[i + 40] + System.Environment.NewLine + datas[t];
                }
              }
            }
          
          return datas;
        }


        ///<summary>
        /// Liefert den Inhalt der Datei zurück.
        ///</summary>
        ///<param name="sFilename">Dateipfad</param>
        public string ReadFile(String sFilename)
        {
          string sContent = "";

          if (File.Exists(sFilename))
          {
            StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
            sContent = myFile.ReadToEnd();
            myFile.Close();
          }
          return sContent;
        }

        public List<grabber.DBMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string imdbid, string fanartPath, bool multiImage, bool choose, string MasterTitle, string personArtworkPath)
        {
          return GetFanart(otitle, ttitle, year, director, imdbid, fanartPath, multiImage, choose, MasterTitle, "", 0, string.Empty, string.Empty);
        }
        public List<grabber.DBMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string imdbid, string fanartPath, bool multiImage, bool choose, string MasterTitle, string personArtworkPath, int downloadlimit, string resolutionMin, string resolutionMax)
        {
          List<grabber.DBMovieInfo> listemovies = new List<grabber.DBMovieInfo>();
          if (otitle.Length == 0)
            return listemovies;
          if (ttitle.Length == 0)
            ttitle = otitle;
          string wtitle1 = otitle;
          string wtitle2 = ttitle;
          if (otitle.IndexOf("\\") > 0)
            wtitle1 = wtitle1.Substring(wtitle1.IndexOf("\\") + 1);
          if (ttitle.IndexOf("\\") > 0)
            wtitle2 = wtitle2.Substring(wtitle2.IndexOf("\\") + 1);
          grabber.TheMoviedb TheMoviedb = new grabber.TheMoviedb();
          listemovies = TheMoviedb.getMoviesByTitles(wtitle1, wtitle2, year, director, imdbid, choose);

          string filename = string.Empty;
          string filename1 = string.Empty;
          string filename2 = string.Empty;
          if (MasterTitle == "OriginalTitle")
            wtitle2 = wtitle1;
          if (listemovies.Count == 1 && listemovies[0].Backdrops != null && listemovies[0].Backdrops.Count > 0 && !choose)
          {
            // Download Fanart !!!
            bool first = true;
            foreach (string backdrop in listemovies[0].Backdrops)
            {
              // old: filename1 = GrabUtil.DownloadBacdropArt(fanartPath, backdrop, wtitle2, multiImage, first, out filename);
              filename1 = GrabUtil.DownloadBacdropArt(fanartPath, backdrop, wtitle2, multiImage, first, out filename, downloadlimit, resolutionMin, resolutionMax);
              //if (filename2 == string.Empty)
              //    filename2 = filename1;
              if ((filename2 != "added") && (filename1 != "already") && !filename1.StartsWith("numberlimit") && !filename1.StartsWith("resolution"))
              {
                filename2 = "added";
              }
              else
              {
                if (filename1.StartsWith("numberlimit")) 
                  filename2 = "numberlimit";
                else if (filename1.StartsWith("resolution"))
                {
                  filename2 = "resolution";
                }
                else
                {
                  filename2 = "already"; 
                  first = false;
                }
              }
              
            }
            listemovies[0].Name = filename2;

            // Download PersonArtwork //
            // Get Actors from TMDB
            string filenameperson = string.Empty;
            string filename1person = string.Empty;
            string filename2person = string.Empty;
            //string ImdbBaseUrl = "http://www.imdb.com/";
            if (!string.IsNullOrEmpty(personArtworkPath) && listemovies[0].Persons != null && listemovies[0].Persons.Count > 0)
            {
              List<grabber.DBPersonInfo> listepersons = listemovies[0].Persons;
              foreach (grabber.DBPersonInfo person in listepersons)
              {
                bool firstpersonimage = true;
                grabber.DBPersonInfo persondetails = new DBPersonInfo();
                persondetails = TheMoviedb.getPersonsById(person.Id, string.Empty);
                foreach (var image in persondetails.Images)
                {
                  filename1person = GrabUtil.DownloadPersonArtwork(personArtworkPath, image, persondetails.Name, multiImage, firstpersonimage, out filenameperson);
                  if ((filename2person != "added") && (filename1person != "already"))
                    filename2person = "added";
                  else
                    filename2person = "already";
                  firstpersonimage = false;
                }
                //// Get further IMDB images
                //Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                //Grabber.MyFilmsIMDB.IMDBUrl wurl;
                //_imdb.FindActor(persondetails.Name);
                //IMDBActor imdbActor = new IMDBActor();

                //if (_imdb.Count > 0)
                //{
                //  string url = string.Empty;
                //  wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                //  if (wurl.URL.Length != 0)
                //  {
                //    url = wurl.URL;
                //    //url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                //    //url = ImdbBaseUrl + url.Substring(url.IndexOf("name"));
                //    this.GetActorDetails(url, persondetails.Name, false, out imdbActor);
                //    filename1person = GrabUtil.DownloadPersonArtwork(personArtworkPath, imdbActor.ThumbnailUrl, persondetails.Name, multiImage, firstpersonimage, out filenameperson);
                //    firstpersonimage = false;
                //  }
                //}
              }
              //// Get further Actors from IMDB
              //IMDBMovie MPmovie = new IMDBMovie();
              //MPmovie.Title = listemovies[0].Name;
              //MPmovie.IMDBNumber = listemovies[0].ImdbID;
              //FetchActorsInMovie(MPmovie, personArtworkPath);
            }
          }
          else if (listemovies.Count > 1)

            //listemovies[0].Name = "(toomany)";
            listemovies[0].Name = "(toomany) - (" + listemovies.Count.ToString() + " results) - " + listemovies[0].Name;

          return listemovies;
        }

        public List<grabber.DBMovieInfo> GetTMDBinfos(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle)
        {
          return GetTMDBinfos(otitle, ttitle, year, director, fanartPath, multiImage, choose, MasterTitle, "en");
        }
        public List<grabber.DBMovieInfo> GetTMDBinfos(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle, string language)
        {

            List<grabber.DBMovieInfo> listemovies = new List<grabber.DBMovieInfo>();
            if (otitle.Length == 0)
                return listemovies;
            string wtitle1 = otitle;
            string wtitle2 = ttitle;
            if (otitle.IndexOf("\\") > 0)
                wtitle1 = wtitle1.Substring(wtitle1.IndexOf("\\") + 1);
            if (ttitle.IndexOf("\\") > 0)
                wtitle2 = wtitle2.Substring(wtitle2.IndexOf("\\") + 1);
            if (ttitle.Length == 0)
                ttitle = otitle;
            grabber.TheMoviedb TheMoviedb = new grabber.TheMoviedb();
            listemovies = TheMoviedb.getMoviesByTitles(wtitle1, wtitle2, year, director, "", choose, language);

            string filename = string.Empty;
            string filename1 = string.Empty;
            string filename2 = string.Empty;
            if (MasterTitle == "OriginalTitle")
                wtitle2 = wtitle1;
            if (listemovies.Count == 1 && listemovies[0].Posters != null && listemovies[0].Posters.Count > 0 && !choose)
            {
                bool first = true;
                foreach (string backdrop in listemovies[0].Posters)
                {
                    filename1 = GrabUtil.DownloadCovers(fanartPath, backdrop, wtitle2, multiImage, first, out filename);
                    //if (filename2 == string.Empty)
                    //    filename2 = filename1;
                    if ((filename2 != "added") && (filename1 != "already"))
                        filename2 = "added";
                    else
                        filename2 = "already";
                    first = false;
                }
                listemovies[0].Name = filename2;
            }
            else if (listemovies.Count > 1)

                //listemovies[0].Name = "(toomany)";
                listemovies[0].Name = "(toomany) - (" + listemovies.Count.ToString() + " results) - " + listemovies[0].Name;

            return listemovies;

        }
        public static string ExtractBody(string Body, string ParamStart, XmlNode n)
        {
            string strStart = string.Empty;
            switch (ParamStart)
            {
                case "Original Title":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").InnerText);
                    break;
                case "Translated Title":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").InnerText);
                    break;
                case "URL cover":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").InnerText);
                    break;
                case "Rate 1":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate").InnerText);
                    break;
                case "Rate 2":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRate2").InnerText);
                    break;
                case "Synopsys":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").InnerText);
                    break;
                case "Director":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").InnerText);
                    break;
                case "Producer":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").InnerText);
                    break;
                case "Actors":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").InnerText);
                    break;
                case "Country":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").InnerText);
                    break;
                case "Genre":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").InnerText);
                    break;
                case "Year":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").InnerText);
                    break;
                case "Comment":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").InnerText);
                    break;
                case "Language":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").InnerText);
                    break;
                case "Tagline":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").InnerText);
                    break;
                case "Certification":
                    strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").InnerText);
                    break;
                default:
                    break;
            }
            if (strStart.Length > 0 && Body.Length > 0) // Guzzi: Fix for exception when returning wrong webpage without content
                return Body.Substring(Body.IndexOf(strStart) + strStart.Length - 1);
            else
                return Body;

        }

        /// <summary>
        /// count the elements
        /// </summary>
        public int Count
        {
            get { return _elements.Count; }
        }

        public IMDBUrl this[int index]
        {
            get { return (IMDBUrl)_elements[index]; }
        }

        #region helper methods to get infos

        /// <summary>
        /// trys to get a webpage from the specified url and returns the content as string
        /// </summary>
        private string GetPage(string strURL, string strEncode, out string absoluteUri)
        {
            string strBody = "";
            absoluteUri = string.Empty;
            Stream receiveStream = null;
            StreamReader sr = null;
            WebResponse result = null;
            try
            {
                // Make the Webrequest
                //Log.Info("IMDB: get page:{0}", strURL);
                WebRequest req = WebRequest.Create(strURL);
                try
                {
                    // Use the current user in case an NTLM Proxy or similar is used.
                    // wr.Proxy = WebProxy.GetDefaultProxy();
                    req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                }
                catch (Exception) { }
                result = req.GetResponse();
                receiveStream = result.GetResponseStream();

                // Encoding: depends on selected page
                Encoding encode = Encoding.GetEncoding(strEncode);
                using (sr = new StreamReader(receiveStream, encode))
                {
                    strBody = sr.ReadToEnd();
                }


                absoluteUri = result.ResponseUri.AbsoluteUri;
            }
            catch (Exception)
            {
                //Log.Error("Error retreiving WebPage: {0} Encoding:{1} err:{2} stack:{3}", strURL, strEncode, ex.Message, ex.StackTrace);
            }
            finally
            {
                if (sr != null)
                {
                    try
                    {
                        sr.Close();
                    }
                    catch (Exception) { }
                }
                if (receiveStream != null)
                {
                    try
                    {
                        receiveStream.Close();
                    }
                    catch (Exception) { }
                }
                if (result != null)
                {
                    try
                    {
                        result.Close();
                    }
                    catch (Exception) { }
                }
            }
            return strBody;
        }

        /// <summary>
        /// cuts end of sting after strWord
        /// </summary>
        private void RemoveAllAfter(ref string strLine, string strWord)
        {
            int iPos = strLine.IndexOf(strWord);
            if (iPos > 0)
            {
                strLine = strLine.Substring(0, iPos);
            }
        }

        /// <summary>
        /// make a searchstring out of the filename
        /// </summary>
        private string GetSearchString(string strMovie)
        {
            string strURL = strMovie;
            strURL = strURL.ToLower();
            strURL = strURL.Trim();

            RemoveAllAfter(ref strURL, "divx");
            RemoveAllAfter(ref strURL, "xvid");
            RemoveAllAfter(ref strURL, "dvd");
            RemoveAllAfter(ref strURL, " dvdrip");
            RemoveAllAfter(ref strURL, "svcd");
            RemoveAllAfter(ref strURL, "mvcd");
            RemoveAllAfter(ref strURL, "vcd");
            RemoveAllAfter(ref strURL, "cd");
            RemoveAllAfter(ref strURL, "ac3");
            RemoveAllAfter(ref strURL, "ogg");
            RemoveAllAfter(ref strURL, "ogm");
            RemoveAllAfter(ref strURL, "internal");
            RemoveAllAfter(ref strURL, "fragment");
            RemoveAllAfter(ref strURL, "proper");
            RemoveAllAfter(ref strURL, "limited");
            RemoveAllAfter(ref strURL, "rerip");
            RemoveAllAfter(ref strURL, "bluray");
            RemoveAllAfter(ref strURL, "brrip");
            RemoveAllAfter(ref strURL, "hddvd");
            RemoveAllAfter(ref strURL, "x264");
            RemoveAllAfter(ref strURL, "mbluray");
            RemoveAllAfter(ref strURL, "1080p");
            RemoveAllAfter(ref strURL, "720p");
            RemoveAllAfter(ref strURL, "480p");
            RemoveAllAfter(ref strURL, "r5");

            RemoveAllAfter(ref strURL, "+divx");
            RemoveAllAfter(ref strURL, "+xvid");
            RemoveAllAfter(ref strURL, "+dvd");
            RemoveAllAfter(ref strURL, "+dvdrip");
            RemoveAllAfter(ref strURL, "+svcd");
            RemoveAllAfter(ref strURL, "+mvcd");
            RemoveAllAfter(ref strURL, "+vcd");
            RemoveAllAfter(ref strURL, "+cd");
            RemoveAllAfter(ref strURL, "+ac3");
            RemoveAllAfter(ref strURL, "+ogg");
            RemoveAllAfter(ref strURL, "+ogm");
            RemoveAllAfter(ref strURL, "+internal");
            RemoveAllAfter(ref strURL, "+fragment");
            RemoveAllAfter(ref strURL, "+proper");
            RemoveAllAfter(ref strURL, "+limited");
            RemoveAllAfter(ref strURL, "+rerip");
            RemoveAllAfter(ref strURL, "+bluray");
            RemoveAllAfter(ref strURL, "+brrip");
            RemoveAllAfter(ref strURL, "+hddvd");
            RemoveAllAfter(ref strURL, "+x264");
            RemoveAllAfter(ref strURL, "+mbluray");
            RemoveAllAfter(ref strURL, "+1080p");
            RemoveAllAfter(ref strURL, "+720p");
            RemoveAllAfter(ref strURL, "+480p");
            RemoveAllAfter(ref strURL, "+r5");
            return strURL;
        }

        private string LoadPage(string Page)
        {
          string strActivePage = string.Empty;
          switch (Page)
          {
            case "URL Gateway":
              strActivePage = BodyDetail2;
              break;
            case "URL Redirection Generic 1":
              strActivePage = BodyLinkGeneric1;
              break;
            case "URL Redirection Generic 2":
              strActivePage = BodyLinkGeneric2;
              break;
            case "URL Redirection Cover":
              strActivePage = BodyLinkImg;
              break;
            case "URL Redirection Persons":
              strActivePage = BodyLinkPersons;
              break;
            case "URL Redirection Title":
              strActivePage = BodyLinkTitles;
              break;
            case "URL Redirection Certification":
              strActivePage = BodyLinkCertification;
              break;
            case "URL Redirection Comment":
              strActivePage = BodyLinkComment;
              break;
            case "URL Redirection Description":
              strActivePage = BodyLinkSyn;
              break;
            case "URL Redirection Multi Posters":
              strActivePage = BodyLinkMultiPosters;
              break;
            case "URL Redirection Photos":
              strActivePage = BodyLinkPhotos;
              break;
            case "URL Redirection PersonImages":
              strActivePage = BodyLinkPersonImages;
              break;
            case "URL Redirection Multi Fanart":
              strActivePage = BodyLinkMultiFanart;
              break;
            case "URL Redirection Trailer":
              strActivePage = BodyLinkTrailer;
              break;
            default:
              strActivePage = BodyDetail;
              break;
          }
          return strActivePage;
        }

        private static void InitLogger()
        {
          LoggingConfiguration config = LogManager.Configuration ?? new LoggingConfiguration();
          string LogFileName = "MyFilmsGrabber.log";
          string OldLogFileName = "MyFilmsGrabber-old.log";

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
          fileTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff} " + "${level:fixedLength=true:padding=5} " +
                              "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " +
                              "${exception:format=tostring}";
          //"${qpc}";
          //${qpc:normalize=Boolean:difference=Boolean:alignDecimalPoint=Boolean:precision=Integer:seconds=Boolean}

          config.AddTarget("file", fileTarget);

          // Get current Log Level from MediaPortal 
          NLog.LogLevel logLevel;
          MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

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

        #endregion

        #region methods to get movie infos from different databases

        /// <summary>
        /// this method switches between the different databases to get the search results
        /// </summary>
        /// 
        // Changed
        public void Find(string strMovie)
        {
            try
            {
                // getting searchstring
                string strSearch = HttpUtility.UrlEncode(GetSearchString(strMovie));

                // be aware of german special chars äöüß Ă¤Ă¶ĂĽĂź %E4%F6%FC%DF %c3%a4%c3%b6%c3%bc%c3%9f
                strSearch = strSearch.Replace("%c3%a4", "%E4");
                strSearch = strSearch.Replace("%c3%b6", "%F6");
                strSearch = strSearch.Replace("%c3%bc", "%FC");
                strSearch = strSearch.Replace("%c3%9f", "%DF");
                // be aware of spanish special chars ńáéíóúÁÉÍÓÚ %E4%F6%FC%DF %c3%a4%c3%b6%c3%bc%c3%9f
                strSearch = strSearch.Replace("%c3%b1", "%F1");
                strSearch = strSearch.Replace("%c3%a0", "%E0");
                strSearch = strSearch.Replace("%c3%a1", "%E1");
                strSearch = strSearch.Replace("%c3%a8", "%E8");
                strSearch = strSearch.Replace("%c3%a9", "%E9");
                strSearch = strSearch.Replace("%c3%ac", "%EC");
                strSearch = strSearch.Replace("%c3%ad", "%ED");
                strSearch = strSearch.Replace("%c3%b2", "%F2");
                strSearch = strSearch.Replace("%c3%b3", "%F3");
                strSearch = strSearch.Replace("%c3%b9", "%F9");
                strSearch = strSearch.Replace("%c3%ba", "%FA");
                // Extra Codes
                strSearch = strSearch.Replace("%c3%b8", "%F8"); //ø
                strSearch = strSearch.Replace("%c3%98", "%D8"); //ø
                strSearch = strSearch.Replace("%c3%86", "%C6"); //Æ
                strSearch = strSearch.Replace("%c3%a6", "%E6"); //æ
                strSearch = strSearch.Replace("%c2%bd", "%BD"); //½
                // CRO
                strSearch = strSearch.Replace("%c4%86", "%0106"); //Č
                strSearch = strSearch.Replace("%c4%87", "%0107"); //č
                strSearch = strSearch.Replace("%c4%8c", "%010C"); //Ć
                strSearch = strSearch.Replace("%c4%8d", "%010D"); //ć
                strSearch = strSearch.Replace("%c4%90", "%0110"); //Đ
                strSearch = strSearch.Replace("%c4%91", "%0111"); //đ
                strSearch = strSearch.Replace("%c5%a0", "%0160"); //Š
                strSearch = strSearch.Replace("%c5%a1", "%0161"); //š
                strSearch = strSearch.Replace("%c5%bc", "%017c"); //Ž
                strSearch = strSearch.Replace("%c5%bd", "%017d"); //ž

                _elements.Clear();

                // search the desired databases
                foreach (IMDB.MovieInfoDatabase db in _databaseList)
                {
                    // only do a search if requested
                    if (db.Limit <= 0)
                    {
                        continue;
                    }
                    if (db.Grabber == null)
                    {
                        continue;
                    }

                    try
                    {
                        db.Grabber.FindFilm(strSearch, db.Limit, _elements);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Movie database lookup Find() - grabber: {0}, message : {1}", db.ID, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info("Movie database lookup Find() - Exception: {0}", ex.Message);
            }
        }
        #endregion

        #region methods to get actor infos

        public void FindActor(string strActor)
        //public Array FindActor(string strActor)
        {
            // getting searchstring
            string strSearch = HttpUtility.UrlEncode(GetSearchString(strActor));

            // be aware of german special chars äöüß Ă¤Ă¶ĂĽĂź %E4%F6%FC%DF %c3%a4%c3%b6%c3%bc%c3%9f
            strSearch = strSearch.Replace("%c3%a4", "%E4");
            strSearch = strSearch.Replace("%c3%b6", "%F6");
            strSearch = strSearch.Replace("%c3%bc", "%FC");
            strSearch = strSearch.Replace("%c3%9f", "%DF");
            // be aware of spanish special chars ńáéíóúÁÉÍÓÚ %E4%F6%FC%DF %c3%a4%c3%b6%c3%bc%c3%9f
            strSearch = strSearch.Replace("%c3%b1", "%F1");
            strSearch = strSearch.Replace("%c3%a0", "%E0");
            strSearch = strSearch.Replace("%c3%a1", "%E1");
            strSearch = strSearch.Replace("%c3%a8", "%E8");
            strSearch = strSearch.Replace("%c3%a9", "%E9");
            strSearch = strSearch.Replace("%c3%ac", "%EC");
            strSearch = strSearch.Replace("%c3%ad", "%ED");
            strSearch = strSearch.Replace("%c3%b2", "%F2");
            strSearch = strSearch.Replace("%c3%b3", "%F3");
            strSearch = strSearch.Replace("%c3%b9", "%F9");
            strSearch = strSearch.Replace("%c3%ba", "%FA");
            strSearch = strSearch.Replace("%c3%b8", "%F8"); //ø
            strSearch = strSearch.Replace("%c3%98", "%D8"); //ø
            strSearch = strSearch.Replace("%c3%86", "%C6"); //Æ
            strSearch = strSearch.Replace("%c3%a6", "%E6"); //æ

            _elements.Clear();

            string strURL = String.Format("http://akas.imdb.com/find?s=nm&q=" + strSearch, strSearch);
            FindIMDBActor(strURL);
        }

        private void FetchActorsInMovie(IMDBMovie _movieDetails, string personartworkpath)
        {
          bool director = false; // Actor is director
          bool byImdbId = true;
          // Lookup by movie IMDBid number from which will get actorIMDBid, lookup by name is not so db friendly

          if (_movieDetails == null)
          {
            return;
          }
          ArrayList actors = new ArrayList();
          // Try first by IMDBMovieId to find IMDBactorID (100% accuracy)
          IMDBSearch actorlist = new IMDBSearch();
          // New actor search method
          actorlist.SearchActors(_movieDetails.IMDBNumber, ref actors);

          // If search by IMDBid fails try old fetch method (by name, less accurate)
          if (actors.Count == 0)
          {
            byImdbId = false;
            string cast = _movieDetails.Cast + "," + _movieDetails.Director;
            char[] splitter = { '\n', ',' };
            string[] temp = cast.Split(splitter);

            foreach (string element in temp)
            {
              string el = element.Trim();
              if (el != string.Empty)
              {
                actors.Add(el);
              }
            }
          }

          if (actors.Count > 0)
          {
            for (int i = 0; i < actors.Count; ++i)
            {
              // Is actor movie director??
              switch (byImdbId) // True-new method, false-old method
              {
                case true:
                  {
                    // Director
                    if (actors[i].ToString().Length > 1 && actors[i].ToString().Substring(0, 2) == "*d")
                    {
                      director = true;
                      // Remove director prefix (came from IMDBmovieID actor search)
                      actors[i] = actors[0].ToString().Replace("*d", string.Empty);
                    }
                    else
                    {
                      director = false;
                    }
                    break;
                  }
                case false:
                  {
                    // from old method (just comparing name with dbmoviedetail director name)
                    if (actors[i].ToString().Contains(_movieDetails.Director))
                    {
                      director = true;
                    }
                    else
                    {
                      director = false;
                    }
                    break;
                  }
              }
              string actor = (string)actors[i];
              string role = string.Empty;

              if (byImdbId == false)
              {
                int pos = actor.IndexOf(" as ");
                if (pos >= 0)
                {
                  role = actor.Substring(pos + 4);
                  actor = actor.Substring(0, pos);
                }
              }

              //Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
              MediaPortal.Video.Database.IMDB _imdb = new MediaPortal.Video.Database.IMDB();
              actor = actor.Trim();
              _imdb.FindActor(actor);
              IMDBActor imdbActor = new IMDBActor();

              if (_imdb.Count > 0)
              {
                //int index = FuzzyMatch(actor);
                int index = 0;
                if (index == -1)
                {
                  index = 0;
                }

                //Log.Info("Getting actor:{0}", _imdb[index].Title);
                _imdb.GetActorDetails(_imdb[index], director, out imdbActor);
                //Log.Info("Adding actor:{0}({1}),{2}", imdbActor.Name, actor, percent);
                int actorId = VideoDatabase.AddActor(imdbActor.Name);
                if (actorId > 0)
                {
                  VideoDatabase.SetActorInfo(actorId, imdbActor);
                  VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);

                  if (imdbActor.ThumbnailUrl != string.Empty)
                  {
                    string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                    string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                    Utils.FileDelete(largeCoverArt);
                    Utils.FileDelete(coverArt);
                    string filename = string.Empty;
                    GrabUtil.DownloadPersonArtwork(personartworkpath, imdbActor.ThumbnailUrl, imdbActor.Name, true, true, out filename);
                  }
                }
              }
              else
              {
                int actorId = VideoDatabase.AddActor(actor);
                imdbActor.Name = actor;
                IMDBActor.IMDBActorMovie imdbActorMovie = new IMDBActor.IMDBActorMovie();
                imdbActorMovie.MovieTitle = _movieDetails.Title;
                imdbActorMovie.Year = _movieDetails.Year;
                imdbActorMovie.Role = role;
                imdbActor.Add(imdbActorMovie);
                VideoDatabase.SetActorInfo(actorId, imdbActor);
                VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
              }
            }
          }
        }

        // Changed - IMDB changed HTML code
        private void FindIMDBActor(string strURL)
        {
            try
            {
                string absoluteUri;
                // UTF-8 have problem with special country chars, default IMDB enc is used
                string strBody = GetPage(strURL, "utf-8", out absoluteUri);
                string value = string.Empty;
                HTMLParser parser = new HTMLParser(strBody);
                if ((parser.skipToEndOf("<title>")) &&
                    (parser.extractTo("</title>", ref value)) && !value.ToLower().Equals("imdb name search"))
                {
                    value = new HTMLUtil().ConvertHTMLToAnsi(value);
                    value = Utils.RemoveParenthesis(value).Trim();
                    IMDBUrl oneUrl = new IMDBUrl(absoluteUri, value, "IMDB");
                    _elements.Add(oneUrl);
                    return;
                }
                parser.resetPosition();
                // while (parser.skipToEndOfNoCase("found the following results"))
                int exact = 0;
                int popular = 0;
                try
                {
                    exact = strBody.LastIndexOf("Exact Matches");
                }
                catch (Exception) { }
                try
                {
                    popular = strBody.LastIndexOf("Popular Names");
                }
                catch (Exception) { }
                if ((exact > 0) & (exact < popular) & (popular >= 0) | (popular < 0))
                {
                    while (parser.skipToEndOfNoCase("Exact Matches"))
                    {
                        string url = string.Empty;
                        string name = string.Empty;
                        //<a href="/name/nm0000246/" onclick="set_args('nm0000246', 1)">Bruce Willis</a>
                        if (parser.skipToStartOf("href=\"/name/"))
                        {
                            parser.skipToEndOf("href=\"");
                            parser.extractTo("\"", ref url);
                            parser.skipToEndOf(">");
                            parser.extractTo("</a>", ref name);
                            name = new HTMLUtil().ConvertHTMLToAnsi(name);
                            name = Utils.RemoveParenthesis(name).Trim();
                            IMDBUrl newUrl = new IMDBUrl("http://akas.imdb.com" + url, name, "IMDB");
                            _elements.Add(newUrl);
                        }
                        else
                        {
                            parser.skipToEndOfNoCase("</a>");
                        }
                    }
                }
                else
                {
                    while (parser.skipToEndOfNoCase("Popular Names"))
                    {
                        string url = string.Empty;
                        string name = string.Empty;
                        //<a href="/name/nm0000246/" onclick="set_args('nm0000246', 1)">Bruce Willis</a>
                        if (parser.skipToStartOf("href=\"/name/"))
                        {
                            parser.skipToEndOf("href=\"");
                            parser.extractTo("\"", ref url);
                            parser.skipToEndOf(">");
                            parser.extractTo("</a>", ref name);
                            name = new HTMLUtil().ConvertHTMLToAnsi(name);
                            name = Utils.RemoveParenthesis(name).Trim();
                            IMDBUrl newUrl = new IMDBUrl("http://akas.imdb.com" + url, name, "IMDB");
                            _elements.Add(newUrl);
                        }
                        else
                        {
                            parser.skipToEndOfNoCase("</a>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("exception for imdb lookup of {0} err:{1} stack:{2}", strURL, ex.Message, ex.StackTrace);
            }
        }

        // Changed - parsing all actor DB fields through HTML (IMDB changed HTML code)
        public bool GetActorDetails(string url, string name, bool director, out MediaPortal.Video.Database.IMDBActor actor)
        {
            actor = new IMDBActor();
            try
            {
                string absoluteUri;
                string strBody = GetPage(url, "utf-8", out absoluteUri);
                if (strBody == null)
                {
                    return false;
                }
                if (strBody.Length == 0)
                {
                    return false;
                }
                // IMDBActorID
                try
                {
                    int pos = url.LastIndexOf("nm");
                    string id = url.Substring(pos).Replace("/", string.Empty);
                    actor.IMDBActorID = id;
                }
                catch (Exception) { }

                HTMLParser parser = new HTMLParser(strBody);
                string strThumb = string.Empty;
                string value = string.Empty;
                string value2 = string.Empty;
                // Actor name
                if ((parser.skipToEndOf("<title>")) &&
                    (parser.extractTo("- IMDb</title>", ref value)))
                {
                    value = new HTMLUtil().ConvertHTMLToAnsi(value);
                    value = Utils.RemoveParenthesis(value).Trim();
                    actor.Name = HttpUtility.HtmlDecode(value.Trim());
                }
                if (actor.Name == string.Empty)
                {
                    actor.Name = name;
                }

                // Photo
                string parserTxt = parser.Content;
                string photoBlock = string.Empty;
                if (parser.skipToStartOf("<td id=\"img_primary\"") &&
                    (parser.extractTo("</td>", ref photoBlock)))
                {
                  parser.Content = photoBlock;
                  if ((parser.skipToEndOf("<img src=\"")) &&
                      (parser.extractTo("\"", ref strThumb)))
                  {
                    actor.ThumbnailUrl = strThumb;
                  }
                  parser.Content = parserTxt;
                }
                // Birth date
                if ((parser.skipToEndOf("Born:")) &&
                    (parser.skipToEndOf("birth_monthday=")) &&
                    (parser.skipToEndOf(">")) &&
                    (parser.extractTo("<", ref value)) &&
                    (parser.skipToEndOf("year=")) &&
                    (parser.extractTo("\"", ref value2)))
                {
                  actor.DateOfBirth = value + " " + value2;
                }
                // Birth place
                if ((parser.skipToEndOf("birth_place=")) &&
                    (parser.skipToEndOf(">")) &&
                    (parser.extractTo("<", ref value)))
                {
                  actor.PlaceOfBirth = HttpUtility.HtmlDecode(value);
                }
                //Mini Biography
                parser.resetPosition();
                if ((parser.skipToEndOf("<td id=\"overview-top\">")) &&
                    (parser.skipToEndOf("<p>")) &&
                    (parser.extractTo("See full bio</a>", ref value)))
                {
                  value = new HTMLUtil().ConvertHTMLToAnsi(value);
                  actor.MiniBiography = Utils.stripHTMLtags(value);
                  actor.MiniBiography = actor.MiniBiography.Replace("See full bio »", string.Empty).Trim();
                  actor.MiniBiography = HttpUtility.HtmlDecode(actor.MiniBiography); // Remove HTML entities like &#189;
                  if (actor.MiniBiography != string.Empty)
                  {
                    // get complete biography
                    string bioURL = absoluteUri;
                    if (!bioURL.EndsWith("/"))
                    {
                      bioURL += "/bio";
                    }
                    else
                      bioURL += "bio";
                    string strBioBody = GetPage(bioURL, "utf-8", out absoluteUri);
                    if (!string.IsNullOrEmpty(strBioBody))
                    {
                      HTMLParser parser1 = new HTMLParser(strBioBody);
                      if (parser1.skipToEndOf("<h5>Mini Biography</h5>") &&
                          parser1.extractTo("</p>", ref value))
                      {
                        value = new HTMLUtil().ConvertHTMLToAnsi(value);
                        actor.Biography = Utils.stripHTMLtags(value).Trim();
                        actor.Biography = HttpUtility.HtmlDecode(actor.Biography); // Remove HTML entities like &#189;
                      }
                    }
                  }
                }

                // Person is movie director or an actor/actress
                bool isActorPass = false;
                bool isDirectorPass = false;
                parser.resetPosition();

                if (director)
                {
                  if ((parser.skipToEndOf("name=\"Director\">Director</a>")) &&
                      (parser.skipToEndOf("</div>")))
                  {
                    isDirectorPass = true;
                  }
                }
                else
                {
                  if (parser.skipToEndOf("name=\"Actress\">Actress</a>") || parser.skipToEndOf("name=\"Actor\">Actor</a>"))
                  {
                    isActorPass = true;
                  }
                }
                // Get filmography
                if (isDirectorPass | isActorPass)
                {
                  string movies = string.Empty;
                  // Get films and roles block
                  if (parser.extractTo("<div id", ref movies))
                  {
                    parser.Content = movies;
                  }
                  // Parse block for evey film and get year, title and it's imdbID and role
                  while (parser.skipToStartOf("<span class=\"year_column\""))
                  {
                    string movie = string.Empty;
                    if (parser.extractTo("<div class", ref movie))
                    {
                      movie += "</li>";
                      HTMLParser movieParser = new HTMLParser(movie);
                      string title = string.Empty;
                      string strYear = string.Empty;
                      string role = string.Empty;
                      string imdbID = string.Empty;
                      // IMDBid
                      movieParser.skipToEndOf("title/");
                      movieParser.extractTo("/", ref imdbID);
                      // Title
                      movieParser.resetPosition();
                      movieParser.skipToEndOf("<a");
                      movieParser.skipToEndOf(">");
                      movieParser.extractTo("<br/>", ref title);
                      title = Utils.stripHTMLtags(title);
                      title = title.Replace("\n", " ").Replace("\r", string.Empty);
                      title = HttpUtility.HtmlDecode(title.Trim()); // Remove HTML entities like &#189;
                      // Year
                      movieParser.resetPosition();
                      if (movieParser.skipToStartOf(">20") && movieParser.skipToEndOf(">"))
                      {
                        movieParser.extractTo("<", ref strYear);
                      }
                      else if (movieParser.skipToStartOf(">19") && movieParser.skipToEndOf(">"))
                      {
                        movieParser.extractTo("<", ref strYear);
                      }
                      // Roles
                      if ((director == false) && (movieParser.skipToEndOf("<br/>"))) // Role case 1, no character link
                      {
                        movieParser.extractTo("<", ref role);
                        role = Utils.stripHTMLtags(role).Trim();
                        role = HttpUtility.HtmlDecode(role.Replace("\n", " ").Replace("\r", string.Empty).Trim());
                        if (role == string.Empty) // Role case 2, with character link
                        {
                          movieParser.resetPosition();
                          movieParser.skipToEndOf("<br/>");
                          movieParser.extractTo("</a>", ref role);
                          role = Utils.stripHTMLtags(role).Trim();
                          role = HttpUtility.HtmlDecode(role.Replace("\n", " ").Replace("\r", string.Empty).Trim());
                        }
                      }
                      else
                      {
                        // Just director
                        if (director) role = "Director";
                      }

                      int year = 0;
                      try
                      {
                        year = Int32.Parse(strYear.Substring(0, 4));
                      }
                      catch (Exception)
                      {
                        year = 1900;
                      }
                      IMDBActor.IMDBActorMovie actorMovie = new IMDBActor.IMDBActorMovie();
                      actorMovie.MovieTitle = title;
                      actorMovie.Role = role;
                      actorMovie.Year = year;
                      actorMovie.imdbID = imdbID;
                      actor.Add(actorMovie);
                    }
                  }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("IMDB.GetActorDetails({0} exception:{1} {2} {3}", url, ex.Message, ex.Source, ex.StackTrace);
            }
            return false;
        }

        public string[] GetNfoDetail(string strNfoFile, string strPathImg, string strPathImgActor, string strConfigFile)
        {
            return GetNfoDetail(strNfoFile, strPathImg, strPathImgActor, strConfigFile, true);
        }

        public string[] GetNfoDetail(string strNfoFile, string strPathImg, string strPathImgActor, string strConfigFile, bool SaveImage)
        {
            string[] datas = new string[80];
            elements.Clear();

            // Temporary Methods to check XML File
            string[] Result = new string[80]; // Array für die nfo-grabberresults - analog dem internetgrabber
            string[] ResultName = new string[80];
            string[] ActorsName = new string[100]; //(Actors Name)
            string[] ActorsRole = new string[100]; //(Actors Role)
            string[] ActorsThumb = new string[100]; //(Actors Thumblink)

            string ActorName = "";
            string ActorRole = "";
            string ActorThumb = "";
            int Actornumber = 0;


            XmlTextReader reader = new XmlTextReader(strNfoFile);
            string element = "";
            string value = "";
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
                switch (nodeType)
                {
                    case XmlNodeType.Element:
                        element = reader.Name;
                        //Log.Debug("MyFilms Grabber - nfoReader: Element name is '" + reader.Name + "'");
                        if (reader.HasAttributes)
                        {
                            Log.Debug("MyFilms Grabber - nfoReader:  *** " + reader.AttributeCount + " Attributes found! ***");
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                Log.Debug("MyFilms Grabber - nfoReader (attribute-value): Attribute is '" + reader.Name + "' with Value '" + reader.Value + "'");
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        value = reader.Value;
                        //Log.Debug("Value is: " + reader.Value);
                        break;
                }
                if (element.Length > 0 && value.Length > 0)
                {
                    Log.Debug("MyFilms Grabber - nfoReader (element-value): Element is '" + element + "' with Value '" + value + "'");
                    switch (element)
                    {
                        case "title":
                            Result[(int)Grabber_Output.TranslatedTitle] = value;
                            ResultName[(int)Grabber_Output.TranslatedTitle] = "translatedtitle";
                            // ***** Translated TITLE *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Translated Title
                              datas[(int)Grabber_Output.TranslatedTitle] = value;
                            else
                                if (!string.IsNullOrEmpty(datas[0]))
                                  datas[(int)Grabber_Output.TranslatedTitle] = datas[(int)Grabber_Output.OriginalTitle];
                            if (string.IsNullOrEmpty(datas[(int)Grabber_Output.OriginalTitle]) && datas[(int)Grabber_Output.TranslatedTitle].Length > 0)
                              datas[(int)Grabber_Output.OriginalTitle] = datas[(int)Grabber_Output.TranslatedTitle];
                            break;

                        case "originaltitle":
                            Result[(int)Grabber_Output.OriginalTitle] = value;
                            ResultName[(int)Grabber_Output.OriginalTitle] = "originaltitle";
                            value = value.Replace("\n", "");
                            //Original Title
                            if (value.Length > 0)
                              datas[(int)Grabber_Output.OriginalTitle] = value;
                            else
                                if (!string.IsNullOrEmpty(datas[1]))
                                  datas[(int)Grabber_Output.OriginalTitle] = datas[(int)Grabber_Output.TranslatedTitle];
                            if (string.IsNullOrEmpty(datas[(int)Grabber_Output.TranslatedTitle]) && datas[(int)Grabber_Output.OriginalTitle].Length > 0)
                              datas[(int)Grabber_Output.TranslatedTitle] = datas[(int)Grabber_Output.OriginalTitle];
                            break;
                        case "sorttitle": //XBMC
                            break;
                        case "set": // XBMC - BoxSet
                            break;

                        case "id":
                            Result[(int)Grabber_Output.URL] = value;
                            ResultName[(int)Grabber_Output.URL] = "url";
                            // URL
                            datas[(int)Grabber_Output.URL] = @"http://www.imdb.de/" + value;
                            break;
                        case "year":
                            Result[(int)Grabber_Output.Year] = value.ToString();
                            ResultName[(int)Grabber_Output.Year] = "year";
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                              datas[(int)Grabber_Output.Year] = value.Substring(value.Length - 4, 4);
                            break;
                        case "releasedate":
                            //Result[13] = value;
                            //ResultName[13] = "date";
                            break;
                        case "rating":
                            Result[(int)Grabber_Output.Rating] = value;
                            ResultName[(int)Grabber_Output.Rating] = "rating";
                            //Rating
                            decimal wRate = 0; 
                            decimal wBasedRate = 10;
                            NumberFormatInfo provider = new NumberFormatInfo();
                            provider.NumberDecimalSeparator = ",";
                            //try
                            //{ wBasedRate = Convert.ToDecimal(strBasedRate, provider); }
                            //catch
                            //{ }

                            value = GrabUtil.convertNote(value);
                            try
                            { wRate = (Convert.ToDecimal(value, provider) / wBasedRate) * 10; }
                            catch
                            {}
                            value = Convert.ToString(wRate);
                            datas[(int)Grabber_Output.Rating] = value.Replace(",", ".");
                            break;
                        case "votes":
                            Result[(int)Grabber_Output.IMDBrank] = value;
                            ResultName[(int)Grabber_Output.IMDBrank] = "votes";
                            break;
                        case "mpaa":
                            //Result[15] = value;
                            //ResultName[15] = "mpaa";
                            break;
                        case "certification":
                            Result[(int)Grabber_Output.Certification] = value;
                            ResultName[(int)Grabber_Output.Certification] = "certification";
                            break;
                        case "genre":
                            Result[(int)Grabber_Output.Category] = value;
                            ResultName[(int)Grabber_Output.Category] = "category";
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Genre
                              datas[(int)Grabber_Output.Category] = value.Replace("/", ",");
                            break;
                        case "studio":
                            Result[(int)Grabber_Output.Studio] = value;
                            ResultName[(int)Grabber_Output.Studio] = "studio";
                            break;
                        case "director":
                            Result[(int)Grabber_Output.Director] = value;
                            ResultName[(int)Grabber_Output.Director] = "director";
                            // ***** Réalisateur *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Director
                              datas[(int)Grabber_Output.Director] = value;
                            break;
                        case "credits":
                            Result[(int)Grabber_Output.Producer] = value;
                            ResultName[(int)Grabber_Output.Producer] = "producer (credits)";
                            string Producers = "";
                            ArrayList Items = new ArrayList();
                            Items = GetProducers(value, 3, false);
                            foreach (string producer in Items)
                            {
                                if (Producers == "")
                                    Producers = producer.Replace("(producer)", "").Trim();
                                else
                                    Producers = Producers + ", " + producer.Replace("(producer)", "").Trim();
                            }
                            // ***** Producteur *****
                            Producers = Producers.Replace("\n", "");
                            if (Producers.Length > 0)
                                //Producer
                              datas[(int)Grabber_Output.Producer] = Producers;
                            break;
                        case "tagline":
                            Result[(int)Grabber_Output.Tagline] = value;
                            ResultName[(int)Grabber_Output.Tagline] = "tagline";
                            break;
                        case "outline":
                            //Result[19] = value;
                            //ResultName[19] = "outline";
                            break;
                        case "plot":
                            Result[(int)Grabber_Output.Description] = value;
                            ResultName[(int)Grabber_Output.Description] = "description";
                            // ***** Synopsis *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Description
                              datas[(int)Grabber_Output.Description] = value;
                            break;
                        case "runtime":
                            value = value.Replace("min", "").Trim();
                            break;
                        case "name": // actor infos
                            if (ActorName.Length == 0)
                                ActorName = value;
                            else
                            {
                                ActorsName[Actornumber] = ActorName;
                                ActorsRole[Actornumber] = ActorRole;
                                ActorsThumb[Actornumber] = ActorThumb;
                                Actornumber = Actornumber + 1;
                                ActorName = "";
                                ActorRole = "";
                                ActorThumb = "";
                            }
                            break;
                        case "role":  // actor infos
                            {
                                ActorRole = value;
                            }
                            break;
                        case "thumb": // actor thumbs or cover thumbs (if actorname is "")
                            {
                                if (ActorName.Length > 0)
                                {
                                    ActorThumb = value;
                                    ActorsName[Actornumber] = ActorName;
                                    ActorsRole[Actornumber] = ActorRole;
                                    ActorsThumb[Actornumber] = ActorThumb;
                                    Actornumber = Actornumber + 1;
                                    ActorName = "";
                                    ActorRole = "";
                                    ActorThumb = "";
                                    // ToDo: Add code here to load actorthumbs in Actordirectory !
                                    if (SaveImage == true)
                                    {
                                        GrabUtil.DownloadCoverArt(strPathImgActor, value, ActorsName[Actornumber], out value);
                                        value = MediaPortal.Util.Utils.FilterFileName(value);
                                        //datas[14] = strPathImg + "\\" + value; // remarked, as there is multiple actor images
                                    }
                                }
                                else
                                {
                                    // ToDo: Add Code here to load pictures/Backdrops
                                    if (value.Contains("posters") && !value.Contains("cover") && !value.Contains("thumb") && !value.Contains("mid"))
                                    {
                                        // ***** URL IMG *****
                                        value = value.Replace("\n", "");
                                        if (value.Length > 0)
                                        {
                                            //datas[11] = strTemp;
                                            //Picture
                                            if (SaveImage == true)
                                            {
                                                GrabUtil.DownloadCoverArt(strPathImg, value, datas[0], out value);
                                                value = MediaPortal.Util.Utils.FilterFileName(value);
                                                datas[(int)Grabber_Output.PicturePathLong] = strPathImg + "\\" + value;
                                            }
                                            datas[(int)Grabber_Output.PicturePathShort] = value;
                                        }
                                    }

                                }
                            }
                            break;
                        
                        case "country": // Attention: is NOT included in NFO format of Ember Mediamanager !!!)
                            // ***** Pays *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                            {
                                value = value.Replace(".", " ");
                                value = GrabUtil.transformCountry(value);
                                //Country
                                datas[(int)Grabber_Output.Country] = value;
                            }
                            break;
                        case "channels":
                            break;
                        case "codec": // Audiocodec if after "channels"
                            break;
                        case "longlanguage":
                            break;
                        case "aspect":
                            break;
                        //case "codec": // Videocodec if after "aspect"
                        //    break;
                        case "duration":
                            break;
                        case "height":
                            break;
                        case "with":
                            break;
                        case "scantype":
                            break;

                        case "playcount": // XBMC
                            break;
                        case "watched": //XBMC
                            break;
                        case "filenameandpath": //XBMC
                            break;
                        case "trailer":
                            break;
                        

                        default:
                            break;
                    }
                    element = "";
                    value = "";
                }
                //Log.Debug("MyFilms Grabber - nfoReader: Attribute is '" + element + "' with Value '" + value + "'");
            }

            Log.Debug("MyFilms Grabber - nfoReader: Actors found: '" + (Actornumber).ToString() + "'");
            ResultName[5] = "actors";
            if (Actornumber > 0)
                Result[5] = ActorsName[0] + " (als " + ActorsRole[0].Replace("(uncredited)", "").Trim() + ")";
            if (Actornumber > 1)
            {
                for (int wi = 1; wi < Actornumber - 1; ++wi)
                {
                  Result[(int)Grabber_Output.Actors] = Result[(int)Grabber_Output.Actors] + ", " + ActorsName[wi] + " (als " + ActorsRole[wi].Replace("(uncredited)", "").Trim() + ")";
                    //Log.Debug("MyFilms Grabber - nfoReader: Actors: '" + ActorsName[wi] + "' (als " + ActorsRole[wi] + ") - Thumb = '" + ActorsThumb[wi] + "'");
                }
            }

            // ***** Acteurs *****
            if (Actornumber > 0)
                //Actors
                datas[5] = Result[5];

            for (int i = 0; i < 20; ++i)
            {
                try
                {
                    Log.Debug("MyFilms Grabber - nfoReader: Summary (" + i.ToString() + ", " + Result[i].Length.ToString() + "): " + ResultName[i] + " = '" + Result[i] + "'");
                }
                catch
                {
                }
            }

            //char[] charsToTrim = { '|' };
            //string str1 = details.Genre.TrimEnd(charsToTrim);
            //string str2 = str1.TrimStart(charsToTrim);
            //details.Genre = str2.Replace("|", ", ");
            //str1 = //details.Director.TrimEnd(charsToTrim);
            //str2 = str1.TrimStart(charsToTrim);
            //details.Director = str2.Replace("|", ", ");
            for (int i = 0; i < 20; ++i)
            {
                try
                {
                    Log.Debug("MyFilms Grabber - nfoReader: Summary (" + i.ToString() + ", " + Result[i].Length.ToString() + "): " + ResultName[i] + " = '" + Result[i] + "'");
                }
                catch
                {
                }
            }
            
            // datas[13] = strNfoFile;
            return datas;

        }

        public static ArrayList GetProducers(string champselect, int minchars, bool filter)
        {
            //Log.Debug("MyFilms (SubWordGrabbing): InputString: '" + champselect + "'");
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
                //Log.Debug("MyFilms (SubWordGrabbing): RegExReplace: '" + champselect + "'");
            }

            string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ", "- ", " -", " AT ", "in " };
            int i = 0;
            for (i = 0; i < 13; i++)
            {
                if ((CleanerList[i].Length > 0) && (filter = true))
                {
                    champselect = champselect.Replace(CleanerList[i], " ");
                    //Log.Debug("MyFilms (SubWordGrabbing): CleanerListItem: '" + CleanerList[i] + "'");
                }
            }
            
            ArrayList wtab = new ArrayList();

            int wi = 0;
            //string[] Sep = conf.ListSeparator;
            //string[] Sep = new string[] { " ", ",", ";", "|", "/", "(", ")", ".", @"\", ":" };
            string[] Sep = new string[] { ",", ";", "|", "/", ".", @"\", ":" };
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
                    //Log.Debug("MyFilms (SubWordGrabbing): wzone: '" + wzone + "'");
                    if (wzone.Length >= minchars)//Only words with minimum 4 letters!
                    {
                        if ((wzone.Contains("(producer)") && (!wzone.Contains("producer "))))
                        {
                            wtab.Add(wzone.Trim());
                            //Log.Debug("MyFilms Grabber (SubItems): AddWordToList: '" + wzone.Trim() + "'");
                        }
                    }
                    wzone = string.Empty;
                }
            }
            return wtab;
        }
                
        #endregion

    }
}
