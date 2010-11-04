using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;
using MediaPortal.Util;
using MediaPortal.Configuration;
using System.Globalization;



namespace Grabber
{

    public class Grabber_URLClass
    {
        ArrayList elements = new ArrayList();

        public ArrayList ReturnURL(string strSearch, string strConfigFile, int strPage)
        {
            return ReturnURL(strSearch, strConfigFile, strPage, true);
        }
        public ArrayList ReturnURL(string strSearch, string strConfigFile, int strPage, bool AlwaysAsk)
        {
            if (strPage == -1)
            {
                // Premier lancement, recherche de la clé n° de page départ
                //Chargement du fichier de configuration
                XmlDocument doc = new XmlDocument();
                doc.Load(strConfigFile);
                XmlNode n = doc.ChildNodes[1].FirstChild;
                //Récupère La clé de premiere page si elle existe (non obligatoire)
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

            // création Regex avec nom du fichier film
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

            //Chargement du fichier de configuration
            XmlDocument doc = new XmlDocument();
            doc.Load(strConfigFile);
            XmlNode n = doc.ChildNodes[1].FirstChild;

            strDBName = n.SelectSingleNode("DBName").InnerText;

            //Récupère l'URL
            strURL = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").InnerText);
            strRedir = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/URL").Attributes["Param1"].InnerText);

            strSearch = GrabUtil.encodeSearch(strSearch);

            strURL = strURL.Replace("#Search#", strSearch);

            //Récupère L'identifieur de page suivante
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
            //Récupère Le n° de la première page
            try { wpagedeb = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartPage").InnerText)); }
            catch { wpagedeb = 1; }
            if (wpage - wStepPage < wpagedeb)
                wpageprev = -1;
            else
                wpageprev = wpage - wStepPage;
            /******************************/
            /* Recherche des titres et des liens
            /******************************/

            //Récupère La clé de début de liste si elle existe (non obligatoire)
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
            /* Découpage de la liste
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
                    //Title
                    strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartTitle").Attributes["Param1"].InnerText);
                    strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("URLSearch/KeyStartTitle").Attributes["Param2"].InnerText);

                    if (strParam1.Length > 0)
                        strTitle = GrabUtil.FindWithAction(strBody, strStartTitle, ref iStartTitle, strEndTitle, strParam1, strParam2).Trim();
                    else
                        strTitle = GrabUtil.Find(strBody, strStartTitle, ref iStartTitle, strEndTitle).Trim();

                    if (strTitle.Length == 0)
                        break;
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
                        //                        break;

                        if (!strLink.StartsWith("http://") && !strLink.StartsWith("www."))
                        {
                            //si les liens sont relatifs on rajoute le préfix (domaine)
                            strLink = XmlConvert.DecodeName(n.SelectSingleNode("URLPrefix").InnerText + strLink);
                        }

                        //Ajout http:// s'il n'y est pas (Pour GetPage)
                        if (strLink.StartsWith("www."))
                            strLink = "http://" + strLink;

                        //Ajout du nouvel élement, on passe le noeud du fichier xml, pour retrouver le détail

                        IMDBUrl url = new IMDBUrl(strLink, strTitle + " (" + strYear + ") " + strDirector, strDBName, n, wpage.ToString());
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
            string strLinkCover = string.Empty;
            string strBodyCover = string.Empty;

            int iStart = 0;
            int iEnd = 0;


            //Récupère la page
            strBody = GrabUtil.GetPage(strURL, null, out absoluteUri, new CookieContainer());
            HTMLUtil htmlUtil = new HTMLUtil();

            //Récupération des paramètres
            //Chargement du fichier de configuration
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

            // ***** Translated TITLE *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndTTitle").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartTTitle").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyTTitleIndex").InnerText);

            if (strParam1.Length > 0)
                strTitle = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTitle = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();


            strTitle = strTitle.Replace("\n", "");
            if (strTitle.Length > 0)
                //Translated Title
                datas[1] = strTitle;
            else
                datas[1] = datas[0];
            if (datas[0].Length == 0)
                datas[0] = datas[1];

            // ***** URL IMG *****
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
                    GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[0], out strTemp);
                    strTemp = MediaPortal.Util.Utils.FilterFileName(strTemp);
                    datas[2] = strPathImg + "\\" + strTemp;
                }
                datas[12] = strTemp;
            }

            // ***** Synopsis *****
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
            // ***** NOTE 1 *****
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

            // ***** NOTE 2 *****
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

            //Rating
            datas[4] = strRate.Replace(",", ".");

            // ***** Acteurs *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCredits").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCredits").Attributes["Param2"].InnerText);
            strParam3 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsRegExp").InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCreditsIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            strTemp = GrabUtil.trimSpaces(strTemp);
            if (strTemp.Length > 0)
                //Actors
                datas[5] = strTemp;

            // ***** Réalisateur *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndRealise").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartRealise").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyRealiseIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Director
                datas[6] = strTemp;

            // ***** Producteur *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndProduct").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartProduct").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyProductIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Producer
                datas[7] = strTemp;


            // ***** Année *****
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


            // ***** Pays *****
            strStart = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").InnerText);
            strEnd = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyEndCountry").InnerText);
            strParam1 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").Attributes["Param1"].InnerText);
            strParam2 = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyStartCountry").Attributes["Param2"].InnerText);
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyCountryIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
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
            strIndex = XmlConvert.DecodeName(n.SelectSingleNode("Details/KeyGenreIndex").InnerText);

            if (strParam1.Length > 0)
                strTemp = GrabUtil.FindWithAction(ExtractBody(strBody, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim();
            else
                strTemp = GrabUtil.Find(ExtractBody(strBody, strIndex, n), strStart, strEnd).Trim();

            strTemp = strTemp.Replace("\n", "");
            if (strTemp.Length > 0)
                //Genre
                datas[10] = strTemp;
            datas[11] = strURL;
            return datas;

        }

        public List<grabber.DBMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle)
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
            if (listemovies.Count == 1 && listemovies[0].Backdrops != null && listemovies[0].Backdrops.Count > 0 && !choose)
            {
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
            }
            else if (listemovies.Count > 1)
                listemovies[0].Name = "toomany";

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
                default:
                    break;
            }
            if (strStart.Length > 0)
                return Body.Substring(Body.IndexOf(strStart) + strStart.Length - 1);
            else
                return Body;

        }

    }
}
