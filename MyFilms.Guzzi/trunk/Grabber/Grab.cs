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

namespace Grabber
{
  using grabber;

  using MediaPortal.Video.Database;
    using System.Text.RegularExpressions;

    public enum optimizeOption { optimizeDisabled };
    //public enum optimizeDisabled;

    public class Grabber_URLClass
    {
        ArrayList elements = new ArrayList();
        #region internal vars

        // list of the search results, containts objects of IMDBUrl
        private ArrayList _elements = new ArrayList();

        private List<IMDB.MovieInfoDatabase> _databaseList = new List<IMDB.MovieInfoDatabase>();

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
          if (!string.IsNullOrEmpty(strMediaPath)) // if a mediapath is given assume it's nfo/xml/xbmc reader request and return the proper file to read in details
          {
            string strFILE = string.Empty;
            string directory = System.IO.Path.GetDirectoryName(strMediaPath); // get directory name of media file
            string strDBName = string.Empty;
            //Loading the configuration file to get details about file to search
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;
            //Gets Key to the first page if it exists (not required)
            try { strDBName = XmlConvert.DecodeName(n.SelectSingleNode("DBName").InnerText); }
            catch { strDBName = "ERROR"; }
            //Gets Key to get the path to search reader file if it exists (not required)
            try { strFILE = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch").InnerText); }
            catch {  }
            elements.Clear();
            if (string.IsNullOrEmpty(strFILE))
            {
              //Try to get the "real" local file
              //List<FileInfo> fileList = new List<FileInfo>();
              //DirectoryInfo[] subdirectories = new DirectoryInfo[] { };
              try
              {
                string[] files = Directory.GetFiles(directory, strSearch, SearchOption.TopDirectoryOnly);
                //fileList.AddRange(directory.GetFiles("*"));
                //subdirectories = directory.GetDirectories();
                foreach (var file in files)
                {
                  string extension = file.Substring(file.LastIndexOf(".")).ToLower();
                  if (extension == "xml" || extension == "nfo" || extension.StartsWith("ht"))
                  {
                    strFILE = file;
                    IMDBUrl urlsuite = new IMDBUrl(strFILE, strSearch, strDBName);
                    elements.Add(urlsuite);
                  }
                }
              }
              catch (Exception e)
              {
                Log.Debug("MyFilms - GrabUtil - Catched Exception: " + e.ToString());
              }
              
            }
            return elements;
          }
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
            // Guzzi Added for metter matching internet searches:
            string m_strYear = "";
            string m_strDirector = "";
            string m_strIMDB_Id = "";
            string m_strTMDB_Id = "";

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

            public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode, string strIMDBURL, string strYear, string strDirector, string strIMDB_Id, string strTMDB_Id)
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
        };

        public void FindMovies(string strSearchInit, string strConfigFile, int strPage, bool AlwaysAsk, out ArrayList ListUrl, out short WIndex)
        {
            WIndex = -1;
            string strSearch = strSearchInit;
            string strTemp = string.Empty;
            string strBody = string.Empty;
            string strItem = string.Empty;
            string strURL;
            string strStart = string.Empty;
            string strEnd = string.Empty;
            string strNext = string.Empty;
            string absoluteUri;
            string strStartItem = string.Empty;
            string strStartTitle = string.Empty;
            string strEndTitle = string.Empty;
            string strStartYear = string.Empty;
            string strEndYear = string.Empty;
            string strStartDirector = string.Empty;
            string strEndDirector = string.Empty;
            string strStartLink = string.Empty;
            string strEndLink = string.Empty;
            string strTitle = string.Empty;
            string strYear = string.Empty;
            string strDirector = string.Empty;
            string strIMDB_Id = string.Empty;
            string strTMDB_Id = string.Empty;
            string strLink = string.Empty;
            string strDBName;
            string strStartPage = string.Empty;
            int wStepPage = 0;
            int iFinItem = 0;
            int iStartTitle = 0;
            int iStartYear = 0;
            int iStartDirector = 0;
            int iStartUrl = 0;
            int iStart = 0;
            int iEnd = 0;
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

            //Retrieves the URL
            strURL = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").InnerText);
            strRedir = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").Attributes["Param1"].InnerText);

            strSearch = GrabUtil.encodeSearch(strSearch);

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

            CookieContainer cook = new CookieContainer();

            //Récupère la page wpage
            strBody = GrabUtil.GetPage(strURL.Replace("#Page#", wpage.ToString()), null, out absoluteUri, cook);
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
                strBody = GrabUtil.GetPage(strRedir, null, out absoluteUri, cook);

            wpage += wStepPage;


            /******************************/
            /* Cutting the list
            /******************************/
            //Si on a au moins la clé de début, on découpe StrBody
            if (strStart != "")
            {
                iStart = strBody.IndexOf(strStart);
                //Si la clé de début a été trouvé
                if (iStart > 0)
                {
                    //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
                    if (strEnd != "")
                        iEnd = strBody.IndexOf(strEnd, iStart);
                    else
                        iEnd = strBody.Length;

                    //Découpage du body
                    iStart += strStart.Length;
                    try { strBody = strBody.Substring(iStart, iEnd - iStart); }
                    catch { }
                }
            }

            iStart = 0;
            iFinItem = 0;
            iStartTitle = 0;
            iStartUrl = 0;
            IMDBUrl urlprev = new IMDBUrl(strURL.Replace("#Page#", wpageprev.ToString()), "---", strDBName, n, wpageprev.ToString());

            if (strBody != "")
            {

                // Comparaison entre la posution de URL et Titre pour bornage des éléments
                if (strBody.IndexOf(strStartTitle, 0) > strBody.IndexOf(strStartLink, 0))
                    strStartItem = strStartLink;
                else
                    strStartItem = strStartTitle;
                iFinItem = strBody.IndexOf(strStartItem, iFinItem);
                //                iFinItem += strStartItem.Length;
                iStartYear = iStartDirector = iStartUrl = iStartTitle = iFinItem;
                while (true)
                {

                    // détermination de la fin de nième Item (si les index des champs trouvés ensuite sont supérieurs => pas d'infos pour cet item
                    if (iFinItem < 0)
                        break;
                    iFinItem = strBody.IndexOf(strStartItem, iFinItem + strStartItem.Length);

                    // Initialisation 
                    // REad Movie Title
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
                        iStartYear = iStartDirector = iStartUrl = iStartTitle = iFinItem;
                        iFinItem = strBody.IndexOf(strStartItem, iFinItem + strStartItem.Length);
                        if (iFinItem < 0)
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
                        // check, if IMDB id is existing
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

                        IMDBUrl url = new IMDBUrl(strLink, strTitle + " (" + strYear + ") " + strDirector, strDBName, n, wpage.ToString(), strYear, strDirector, strIMDB_Id, strTMDB_Id) ;
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
                    iStartYear = iStartDirector = iStartUrl = iStartTitle = iFinItem;
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
            string[] datas = new string[20];
            elements.Clear();
            string strTemp = string.Empty;
            string strBody = string.Empty;
            string strBodyPersons = string.Empty;
            string strBodyTitles = string.Empty;
            string strBodyCertification = string.Empty;

            string strStart = string.Empty;
            string strEnd = string.Empty;
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
            string strBodyCover = string.Empty;

            int iStart = 0;
            int iEnd = 0;


            //Fetch the page
            strBody = GrabUtil.GetPage(strURL, null, out absoluteUri, new CookieContainer());
            HTMLUtil htmlUtil = new HTMLUtil();

            //Recovery parameters
            // Load the configuration file
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;


            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartBody").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndBody").InnerText);

            //Si on a au moins la clé de début, on découpe StrBody
            if (strStart != "")
            {
                iStart = strBody.IndexOf(strStart);

                //Si la clé de début a été trouvé
                if (iStart > 0)
                {

                    //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
                    if (strEnd != "")
                    {
                        iEnd = strBody.IndexOf(strEnd, iStart);
                    }
                    else
                        iEnd = strBody.Length;

                    //Découpage du body
                    iStart += strStart.Length;
                    if (iEnd - iStart > 0)
                        strBody = strBody.Substring(iStart, iEnd - iStart);

                }
            }

            string strIndex = string.Empty;
            // ***** Original TITLE *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndOTitle").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartOTitle").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyOTitleIndex").InnerText);

            if (strParam1.Length > 0)
                strTitle = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTitle = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTitle = strTitle.Replace("\n", "");

            if (strTitle.Length > 0)
                //Original Title
                datas[0] = strTitle;
            else
                datas[0] = "";

            // ***** URL Redirection Titles ***** // Will be used for TTitle, if available !
            strTemp = "";
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTitles").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkTitles").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTitles").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkTitles").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkTitlesIndex").InnerText);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();
              strBodyTitles = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
            }
            else
              strBodyTitles = strBody;
            datas[18] = strTemp;          

            // ***** Translated TITLE *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTTitle").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleRegExp").InnerText); 
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleIndex").InnerText);
            try
            {strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleMaxItems").InnerText);}
            catch (Exception) {strMaxItems = "";}
            try
            {strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleLanguage").InnerText);}
            catch (Exception) {strLanguage = "";}
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTitle = GrabUtil.FindWithAction(ExtractBody(strBodyTitles, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage).Trim();
            else
              strTitle = GrabUtil.Find(ExtractBody(strBodyTitles, strIndex, n), strStart, strEnd).Trim();
            strTitle = strTitle.Replace("\n", "");
            if (strTitle.Length > 0)
                //Translated Title
                datas[1] = strTitle;
            else
              datas[1] = "";
            //else
            //    datas[1] = datas[0];
            //if (datas[0].Length == 0)
            //    datas[0] = datas[1];

            // ***** URL Redirection IMG *****
            strTemp = "";
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkImg").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkImg").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkImg").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkImg").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkImgIndex").InnerText);
            // ***** If Details/KeyStartLinkImg filled => there is a redirection Cover's Page. Get it before searching Cover
            if (strStart.Length > 0)
            {
                if (strParam1.Length > 0)
                    strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
                else
                    strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();
                strBodyCover = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
            }
            else
                strBodyCover = strBody;

            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndImg").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartImg").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyImgIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBodyCover, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBodyCover, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
            {
                //datas[11] = strTemp;
                //Picture
                if (SaveImage == true)
                {
                    if (string.IsNullOrEmpty(datas[0]))
                      GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[1], out strTemp);
                    else
                      GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[0], out strTemp);
                    strTemp = MediaPortal.Util.Utils.FilterFileName(strTemp);
                    datas[2] = strPathImg + "\\" + strTemp;
                }
                datas[12] = strTemp;
            }

            // ***** URL Redirection Persons ***** // Will be used for persons, if available !
            strTemp = "";
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersons").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkPersons").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersons").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkPersons").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkPersonsIndex").InnerText);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();
              strBodyPersons = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
            }
            else
              strBodyPersons = strBody;
            datas[13] = strTemp;

            // ***** Synopsis ***** Description
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndSyn").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartSyn").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeySynIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Description
                datas[3] = strTemp;

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

            if (strParam1.Length > 0)
                strRate = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strRate = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

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

            if (strParam1.Length > 0)
                strRate2 = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strRate2 = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strRate2 = GrabUtil.convertNote(strRate2);

            try
            { wRate2 = (Convert.ToDecimal(strRate2, provider) / wBasedRate) * 10; }
            catch
            { }

            //Calcul de la moyenne des notes.
            if (wRate > 0 && wRate2 > 0)
                strRate = Convert.ToString((wRate + wRate2) / 2);
            else
                if (wRate == 0 && wRate2 == 0)
                    strRate = "";
                else
                    strRate = Convert.ToString((wRate + wRate2));

            //Rating (calculated from Rating 1 and 2)
            datas[4] = strRate.Replace(",", ".");

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
            strLanguage = "";

            if (strParam1.Length > 0 || strParam3.Length > 0) // 
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, boolGrabActorRoles).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            strTemp = GrabUtil.trimSpaces(strTemp);
            if (strTemp.Length > 0)
                //Actors
                datas[5] = strTemp;

            // ***** Réalisateur ***** = Director 
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndRealise").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseMaxItems").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseIndex").InnerText);

            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Director
                datas[6] = strTemp;

            // ***** Producteur ***** Producer // Producers also using MiltiPurpose Secondary page !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndProduct").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductMaxItems").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductIndex").InnerText);

            if (strParam1.Length > 0 || strParam3.Length > 0) // Guzzi: Added param3 to execute matchcollections also !
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Producer
                datas[7] = strTemp;

            // ***** Année ***** Year
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndYear").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartYear").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyYearIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Year
                datas[8] = strTemp.Substring(strTemp.Length - 4, 4);


            // ***** Pays ***** Country
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCountry").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCountryRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCountryIndex").InnerText);

            if (strParam1.Length > 0 || strParam3.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
            {
                strTemp = strTemp.Replace(".", " ");
                strTemp = GrabUtil.transformCountry(strTemp);
                //Country
                datas[9] = strTemp;
            }


            // ***** Genre *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndGenre").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartGenre").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGenreRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGenreIndex").InnerText);

            if (strParam1.Length > 0 || strParam3.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Genre
                datas[10] = strTemp;

            // ***** URL *****
            datas[11] = strURL;

            // ***** Comment *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndComment").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartComment").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCommentRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCommentIndex").InnerText);

            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", " "); // Guzzi: Replace linebreaks with space
            if (strTemp.Length > 0)
              //Comment
              datas[14] = strTemp;

            // ***** Language *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLanguage").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLanguage").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLanguageIndex").InnerText);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              //Language
              datas[15] = strTemp;

            // ***** Tagline *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTagline").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTagline").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTaglineIndex").InnerText);

            if (strParam1.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              //Tagline
              datas[16] = strTemp;

            // ***** URL Redirection Certification ***** // Will be used for Certification Details, if available !
            strTemp = "";
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkCertification").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndLinkCertification").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkCertification").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartLinkCertification").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyLinkCertificationIndex").InnerText);
            if (strStart.Length > 0)
            {
              if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
              else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();
              strBodyCertification = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
            }
            else
              strBodyCertification = strBody;
            //datas[19] = strTemp; // Field 19 is used for Writer !

          
            // ***** Certification *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCertification").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCertification").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationIndex").InnerText);
            try
            { strLanguage = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCertificationLanguage").InnerText); }
            catch (Exception) { strLanguage = ""; }
            if (strParam1.Length > 0 || strParam3.Length > 0)
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBodyCertification, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, "", strLanguage).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBodyCertification, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              //Certification
              datas[17] = strTemp;

            // ***** Writer ***** // Writers also using MiltiPurpose Secondary page !
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartWriter").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndWriter").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartWriter").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartWriter").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterRegExp").InnerText);
            strMaxItems = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterMaxItems").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyWriterIndex").InnerText);

            if (strParam1.Length > 0 || strParam3.Length > 0) // Guzzi: Added param3 to execute matchcollections also !
              strTemp = GrabUtil.FindWithAction(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
            else
              strTemp = GrabUtil.Find(ExtractBody(strBodyPersons, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
              //Producer
              datas[19] = strTemp;

          return datas;
        }

        public List<grabber.DBMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle)
        {
          return GetFanart(otitle, ttitle, year, director, fanartPath, multiImage, choose, MasterTitle, "");
        }
        public List<grabber.DBMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle, string personArtworkPath)
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
          listemovies = TheMoviedb.getMoviesByTitles(wtitle1, wtitle2, year, director, choose);

          string filename = string.Empty;
          string filename1 = string.Empty;
          string filename2 = string.Empty;
          if (MasterTitle == "OriginalTitle")
            wtitle2 = wtitle1;
          if (listemovies.Count == 1 && listemovies[0].Backdrops != null && listemovies[0].Backdrops.Count > 0 &&
              !choose)
          {
            // Download Fanart !!!
            bool first = true;
            foreach (string backdrop in listemovies[0].Backdrops)
            {
              filename1 = GrabUtil.DownloadBacdropArt(fanartPath, backdrop, wtitle2, multiImage, first, out filename);
              //if (filename2 == string.Empty)
              //    filename2 = filename1;
              if ((filename2 != "added") && (filename1 != "already"))
                filename2 = "added";
              else
                filename2 = "already";
              first = false;
            }
            listemovies[0].Name = filename2;

            // Download PersonArtwork //
            // Get Actors from TMDB
            string filenameperson = string.Empty;
            string filename1person = string.Empty;
            string filename2person = string.Empty;
            string ImdbBaseUrl = "http://www.imdb.com/";
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
                // Get further IMDB images
                Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                Grabber.MyFilmsIMDB.IMDBUrl wurl;
                _imdb.FindActor(persondetails.Name);
                //Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();
                IMDBActor imdbActor = new IMDBActor();

                if (_imdb.Count > 0)
                {
                  string url = string.Empty;
                  wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                  if (wurl.URL.Length != 0)
                  {
                    url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                    url = ImdbBaseUrl + url.Substring(url.IndexOf("name"));
                    this.GetActorDetails(url, persondetails.Name, false, out imdbActor);
                    filename1person = GrabUtil.DownloadPersonArtwork(personArtworkPath, imdbActor.ThumbnailUrl, persondetails.Name, multiImage, firstpersonimage, out filenameperson);
                    firstpersonimage = false;
                  }
                }
              }
              // Get further Actors from IMDB
              IMDBMovie MPmovie = new IMDBMovie();
              MPmovie.Title = listemovies[0].Name;
              MPmovie.IMDBNumber = listemovies[0].ImdbID;
              FetchActorsInMovie(MPmovie, personArtworkPath);
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
            listemovies = TheMoviedb.getMoviesByTitles(wtitle1, wtitle2, year, director, choose, language);

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
            string[] datas = new string[20];
            elements.Clear();

            // Temporary Methods to check XML File
            string[] Result = new string[20]; // Array für die nfo-grabberresults - analog dem internetgrabber
            string[] ResultName = new string[20];
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
                        //Log.Debug("                Value is: " + reader.Value);
                        break;
                }
                if (element.Length > 0 && value.Length > 0)
                {
                    Log.Debug("MyFilms Grabber - nfoReader (element-value): Element is '" + element + "' with Value '" + value + "'");
                    switch (element)
                    {
                        case "title":
                            Result[1] = value;
                            ResultName[1] = "translatedtitle";
                            // ***** Translated TITLE *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Translated Title
                                datas[1] = value;
                            else
                                if (!string.IsNullOrEmpty(datas[0]))
                                    datas[1] = datas[0];
                            if (string.IsNullOrEmpty(datas[0]) && datas[1].Length > 0)
                                datas[0] = datas[1];
                            break;

                        case "originaltitle":
                            Result[0] = value;
                            ResultName[0] = "originaltitle";
                            value = value.Replace("\n", "");
                            //Original Title
                            if (value.Length > 0)
                                datas[0] = value;
                            else
                                if (!string.IsNullOrEmpty(datas[1]))
                                    datas[0] = datas[1];
                            if (string.IsNullOrEmpty(datas[1]) && datas[0].Length > 0)
                                datas[1] = datas[0];
                            break;
                        case "sorttitle": //XBMC
                            break;
                        case "set": // XBMC - BoxSet
                            break;

                        case "id":
                            Result[11] = value;
                            ResultName[11] = "url";
                            // URL
                            datas[11] = @"http://www.imdb.de/" + value;
                            break;
                        case "year":
                            Result[8] = value.ToString();
                            ResultName[8] = "year";
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                datas[8] = value.Substring(value.Length - 4, 4);
                            break;
                        case "releasedate":
                            Result[13] = value;
                            ResultName[13] = "date";
                            break;
                        case "rating":
                            Result[4] = value;
                            ResultName[4] = "rating";
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
                            datas[4] = value.Replace(",", ".");
                            break;
                        case "votes":
                            Result[14] = value;
                            ResultName[14] = "votes";
                            break;
                        case "mpaa":
                            Result[15] = value;
                            ResultName[15] = "mpaa";
                            break;
                        case "certification":
                            Result[16] = value;
                            ResultName[16] = "certification";
                            break;
                        case "genre":
                            Result[10] = value;
                            ResultName[10] = "category";
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Genre
                                datas[10] = value.Replace("/", ",");
                            break;
                        case "studio":
                            Result[17] = value;
                            ResultName[17] = "studio";
                            break;
                        case "director":
                            Result[6] = value;
                            ResultName[6] = "director";
                            // ***** Réalisateur *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Director
                                datas[6] = value;
                            break;
                        case "credits":
                            Result[7] = value;
                            ResultName[7] = "producer (credits)";
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
                                datas[7] = Producers;
                            break;
                        case "tagline":
                            Result[18] = value;
                            ResultName[18] = "tagline";
                            break;
                        case "outline":
                            Result[19] = value;
                            ResultName[19] = "outline";
                            break;
                        case "plot":
                            Result[3] = value;
                            ResultName[3] = "description";
                            // ***** Synopsis *****
                            value = value.Replace("\n", "");
                            if (value.Length > 0)
                                //Description
                                datas[3] = value;
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
                                                datas[2] = strPathImg + "\\" + value;
                                            }
                                            datas[12] = value;
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
                                datas[9] = value;
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
                    Result[5] = Result[5] + ", " + ActorsName[wi] + " (als " + ActorsRole[wi].Replace("(uncredited)", "").Trim() + ")";
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
            
            datas[13] = strNfoFile;
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
