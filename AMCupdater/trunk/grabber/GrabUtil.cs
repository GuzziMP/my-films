using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Configuration;
using System.Reflection;
using System.Drawing;
using MediaPortal.Util;
using MediaPortal.GUI.Library;

namespace Grabber
{
    public class GrabUtil
    {
        public static string Find(string Body, string KeyStart, string KeyEnd)
        {
            int iStart = 0;
            int iEnd = 0;
            int iLength = 0;

            bool bregexs = false;
            bool bregexe = false;
            if (KeyStart.StartsWith("#REGEX#"))
                bregexs = true;
            if (KeyEnd.StartsWith("#REGEX#"))
                bregexe = true;
            string strTemp = String.Empty;
            HTMLUtil htmlUtil = new HTMLUtil();

            if (KeyStart != "" && KeyEnd != "")
            {
                iLength = KeyStart.Length;
                if (bregexs)
                    iStart = FindRegEx(Body, KeyStart, iStart, ref iLength, true);
                else
                    iStart = Body.IndexOf(KeyStart);
                if (iStart > 0)
                {
                    iStart += iLength;
                    if (bregexe)
                        iEnd = FindRegEx(Body, KeyEnd, iStart, ref iLength, true) + iStart;
                    else
                        iEnd = Body.IndexOf(KeyEnd, iStart);
                    if (iEnd > 0)
                    {
                        strTemp = Body.Substring(iStart, iEnd - iStart);
                        if (strTemp != "")
                        {
                            htmlUtil.RemoveTags(ref strTemp);
                            htmlUtil.ConvertHTMLToAnsi(strTemp, out strTemp);
                        }
                    }
                }
            }
            return strTemp.Trim();
        }
 
        public static string Find(string Body, string KeyStart, ref int iStart, string KeyEnd)
        {
            int iEnd = 0;
            int iLength = 0;

            string strTemp = String.Empty;
            HTMLUtil htmlUtil = new HTMLUtil();
            bool bregexs = false;
            bool bregexe = false;
            if (KeyStart.StartsWith("#REGEX#"))
                bregexs = true;
            if (KeyEnd.StartsWith("#REGEX#"))
                bregexe = true;
            if (KeyStart != "" && KeyEnd != "")
            {
                iLength = KeyStart.Length;
                if (bregexs)
                    iStart = FindRegEx(Body, KeyStart,iStart, ref iLength,true);
                else
                    iStart = Body.IndexOf(KeyStart, iStart);
                
                if (iStart >= 0)
                {
                    iStart += iLength;
                    iLength = KeyEnd.Length;
                    if (bregexe)
                        iEnd = FindRegEx(Body, KeyEnd, iStart, ref iLength, true) + iStart;
                    else
                        iEnd = Body.IndexOf(KeyEnd, iStart);
                    if (iEnd > 0)
                    {
                        strTemp = Body.Substring(iStart, iEnd - iStart);
                        if (strTemp != "")
                        {
                            htmlUtil.RemoveTags(ref strTemp);
                            htmlUtil.ConvertHTMLToAnsi(strTemp, out strTemp);
                        }
                    }
                }

            }
            return strTemp.Trim();
        }
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2)
        {
            return FindWithAction(Body, KeyStart, KeyEnd, param1, param2, string.Empty);
        }

        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2, string param3)
        {
            int iStart = 0;
            int iEnd = 0;
            int iLength = 0;

            string strTemp = String.Empty;
            HTMLUtil htmlUtil = new HTMLUtil();
            bool bregexs = false;
            bool bregexe = false;
            if (KeyStart.StartsWith("#REGEX#"))
                bregexs = true;
            if (KeyEnd.StartsWith("#REGEX#"))
                bregexe = true;

            if (KeyStart != "" && KeyEnd != "")
            {
                iLength = KeyStart.Length;
                if (param1.StartsWith("#REVERSE#"))
                {
                    if (bregexs)
                        iStart = FindRegEx(Body, KeyStart, iStart, ref iLength, false);
                    else
                        iStart = Body.LastIndexOf(KeyStart);
                }
                else
                    if (bregexs)
                        iStart = FindRegEx(Body, KeyStart, iStart, ref iLength, true);
                    else
                        iStart = Body.IndexOf(KeyStart);

                if (iStart > 0)
                {
                    if (param1.StartsWith("#REVERSE#"))
                    {
                        iLength = KeyEnd.Length;
                        if (bregexe)
                            iEnd = FindRegEx(Body, KeyEnd, iStart, ref iLength, false) + iStart;
                        else
                            iEnd = Body.LastIndexOf(KeyEnd, iStart);
                    }
                    else
                    {
                        iStart += iLength;
                        if (bregexe)
                            iEnd = FindRegEx(Body, KeyEnd, iStart, ref iLength, true) + iStart;
                        else
                            iEnd = Body.IndexOf(KeyEnd, iStart);
                    }
                    if (iEnd > 0)
                    {
                        if (param1.StartsWith("#REVERSE#"))
                        {
                            param1 = param1.Substring(9); 
                            iEnd += iLength;
                            strTemp = Body.Substring(iEnd, iStart - iEnd);
                        }
                        else
                            strTemp = Body.Substring(iStart, iEnd - iStart);
                        if (strTemp != "")
                        {
                            if (param3.Length > 0)
                            {
                                Regex oRegex = new Regex(param3);
                                Regex oRegexReplace = new Regex(string.Empty);
                                System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(strTemp);
                                foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                                {
                                    if (param1.StartsWith("#REGEX#"))
                                    {
                                        oRegexReplace = new Regex(param1.Substring(7));
                                        strTemp = strTemp.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, param2));
                                    }
                                    else
                                        strTemp = strTemp.Replace(param1, param2);
                                }
                            }
                            else
                            {
                                if (param1.StartsWith("#REGEX#"))
                                    strTemp = Regex.Replace(strTemp, param1.Substring(7), param2);
                                else
                                    if (param1.Length > 0)
                                        strTemp = strTemp.Replace(param1, param2);
                            }
                            htmlUtil.RemoveTags(ref strTemp);
                            htmlUtil.ConvertHTMLToAnsi(strTemp, out strTemp);
                        } 
                    }
                }

            }
            return strTemp.Trim();
        }
        public static string FindWithAction(string Body, string KeyStart, ref int iStart, string KeyEnd, string param1, string param2)
        {
            return FindWithAction(Body,KeyStart,ref iStart,KeyEnd,param1,param2,string.Empty);
        }
        public static string FindWithAction(string Body, string KeyStart, ref int iStart, string KeyEnd, string param1, string param2, string param3)
        {
            int iEnd = 0;
            int iLength = 0;

            string strTemp = String.Empty;
            HTMLUtil htmlUtil = new HTMLUtil();
            bool bregexs = false;
            bool bregexe = false;
            if (KeyStart.StartsWith("#REGEX#"))
                bregexs = true;
            if (KeyEnd.StartsWith("#REGEX#"))
                bregexe = true;
            
            if (KeyStart != "" && KeyEnd != "")
            {
                iLength = KeyStart.Length;
                if (bregexs)
                    iStart = FindRegEx(Body, KeyStart, iStart, ref iLength, true) + iStart;
                else
                    iStart = Body.IndexOf(KeyStart, iStart);
                if (iStart > 0)
                {
                    iStart += iLength;
                    if (bregexe)
                        iEnd = FindRegEx(Body, KeyEnd,  iStart, ref iLength, true) + iStart;
                    else
                        iEnd = Body.IndexOf(KeyEnd, iStart);
                    if (iEnd > 0)
                    {
                        strTemp = Body.Substring(iStart, iEnd - iStart);
                        if (strTemp != "")
                        {
                            if (param3.Length > 0)
                            {
                                Regex oRegex = new Regex(param3);
                                Regex oRegexReplace = new Regex(string.Empty);
                                System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(strTemp);
                                foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                                {
                                    if (param1.StartsWith("#REGEX#"))
                                    {
                                        oRegexReplace = new Regex(param1.Substring(7));
                                        strTemp = strTemp.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, param2));
                                    }
                                    else
                                        strTemp = strTemp.Replace(param1, param2);
                                }
                            }
                            else
                            {
                                if (param1.StartsWith("#REGEX#"))
                                    strTemp = Regex.Replace(strTemp, param1.Substring(7), param2);
                                else
                                    if (param1.Length > 0)
                                        strTemp = strTemp.Replace(param1, param2);
                            }
                            htmlUtil.RemoveTags(ref strTemp);
                            htmlUtil.ConvertHTMLToAnsi(strTemp, out strTemp);
                        }
                    }
                }

            }
            return strTemp.Trim();
        }

        public static int FindRegEx(string text, string Key, int iStart, ref int iLength, bool first)
        {
            Regex p = new Regex(Key.Substring(7), RegexOptions.Singleline);
            int imatch = 0;
            text = text.Substring(iStart);
            iLength = 0;

            MatchCollection MatchList = p.Matches(text);
            if (MatchList.Count == 0)
                return 0;
            Match matcher = MatchList[0];           
            if (first)
                matcher = MatchList[MatchList.Count-1];                           

            imatch = matcher.Index;
            iLength = matcher.Length;
            return imatch;
        }

        public static string transformCountry(string strCountry)
        {
            strCountry = strCountry.ToLower();
            // Liste des nationalités trouvée sur moviecovers
            strCountry = strCountry.Replace("afghan", "Afghanistan");
            strCountry = strCountry.Replace("albanais", "Albanie");
            strCountry = strCountry.Replace("algérien", "Algérie");
            strCountry = strCountry.Replace("allemande", "Allemagne");
            strCountry = strCountry.Replace("allemand", "Allemagne");
            strCountry = strCountry.Replace("américaine", "USA");
            strCountry = strCountry.Replace("américain", "USA");
            strCountry = strCountry.Replace("argentin", "Argentine");
            strCountry = strCountry.Replace("arménien", "Arménie");
            strCountry = strCountry.Replace("australienne", "Australie");
            strCountry = strCountry.Replace("australien", "Australie");
            strCountry = strCountry.Replace("autrichien", "Autriche");
            strCountry = strCountry.Replace("bangladais", "Bangladesh");
            strCountry = strCountry.Replace("belge", "Belgique");
            strCountry = strCountry.Replace("beninois", "Benin");
            strCountry = strCountry.Replace("bosniaque", "Bosnie");
            strCountry = strCountry.Replace("botswanais", "Botswana");
            strCountry = strCountry.Replace("bouthanais", "Bouthan");
            strCountry = strCountry.Replace("brésilien", "Brésil");
            strCountry = strCountry.Replace("britannique", "Grande-Bretagne");
            strCountry = strCountry.Replace("bulgare", "Bulgarie");
            strCountry = strCountry.Replace("burkinabè", "Burkina Faso");
            strCountry = strCountry.Replace("cambodgien", "Cambodge");
            strCountry = strCountry.Replace("camerounais", "Cameroun");
            strCountry = strCountry.Replace("canadien", "Canada");
            strCountry = strCountry.Replace("chilien", "Chili");
            strCountry = strCountry.Replace("chinoise", "Chine");
            strCountry = strCountry.Replace("chinois", "Chine");
            strCountry = strCountry.Replace("colombien", "Colombie");
            strCountry = strCountry.Replace("congolais", "Congo");
            strCountry = strCountry.Replace("cubain", "Cuba");
            strCountry = strCountry.Replace("danois", "Danemark");
            strCountry = strCountry.Replace("ecossais", "Ecosse");
            strCountry = strCountry.Replace("egyptien", "Egypte");
            strCountry = strCountry.Replace("espagnole", "Espagne");
            strCountry = strCountry.Replace("espagnol", "Espagne");
            strCountry = strCountry.Replace("estonien", "Estonie");
            strCountry = strCountry.Replace("européen", "UE");
            strCountry = strCountry.Replace("finlandais", "Finlande");
            strCountry = strCountry.Replace("française", "France");
            strCountry = strCountry.Replace("français", "France");
            strCountry = strCountry.Replace("gabonais", "Gabon");
            strCountry = strCountry.Replace("georgien", "Géorgie");
            strCountry = strCountry.Replace("grec", "Grèce");
            strCountry = strCountry.Replace("guinéen", "Guinée");
            strCountry = strCountry.Replace("haïtien", "Haïti");
            strCountry = strCountry.Replace("hollandais", "Pays-Bas");
            strCountry = strCountry.Replace("néerlandais", "Pays-Bas");
            strCountry = strCountry.Replace("hong-kongais", "Hong-Kong");
            strCountry = strCountry.Replace("hongrois", "Hongrie");
            strCountry = strCountry.Replace("indien", "Inde");
            strCountry = strCountry.Replace("indonésien", "Indonésie");
            strCountry = strCountry.Replace("irakien", "Irak");
            strCountry = strCountry.Replace("iranien", "Iran");
            strCountry = strCountry.Replace("irlandais", "Irlande");
            strCountry = strCountry.Replace("islandais", "Islande");
            strCountry = strCountry.Replace("israélien", "Israël");
            strCountry = strCountry.Replace("italien", "Italie");
            strCountry = strCountry.Replace("ivoirien", "Côte d'Ivoire");
            strCountry = strCountry.Replace("jamaïcain", "Jamaïque");
            strCountry = strCountry.Replace("japonaise", "Japon");
            strCountry = strCountry.Replace("japonais", "Japon");
            strCountry = strCountry.Replace("kazakh", "Kazakhstan");
            strCountry = strCountry.Replace("kirghiz", "Kirghizistan");
            strCountry = strCountry.Replace("kurde", "Kurdistan");
            strCountry = strCountry.Replace("lettonien", "Lettonie");
            strCountry = strCountry.Replace("libanais", "Liban");
            strCountry = strCountry.Replace("liechtensteinois", "Liechtenstein");
            strCountry = strCountry.Replace("lituanien", "Lituanie");
            strCountry = strCountry.Replace("luxembourgeois", "Luxembourg");
            strCountry = strCountry.Replace("macédonien", "Macédoine");
            strCountry = strCountry.Replace("malaisien", "Malaisie");
            strCountry = strCountry.Replace("malien", "Mali");
            strCountry = strCountry.Replace("maltais", "Malte");
            strCountry = strCountry.Replace("marocain", "Maroc");
            strCountry = strCountry.Replace("mauritanien", "Mauritanie");
            strCountry = strCountry.Replace("mexicain", "Mexique");
            strCountry = strCountry.Replace("néo-zélandais", "Nouvelle-Zélande");
            strCountry = strCountry.Replace("nigérien", "Nigéria");
            strCountry = strCountry.Replace("nord-coréen", "Corée du Nord");
            strCountry = strCountry.Replace("norvégien", "Norvége");
            strCountry = strCountry.Replace("pakistanais", "Pakistan");
            strCountry = strCountry.Replace("palestinien", "Palestine");
            strCountry = strCountry.Replace("péruvien", "Pérou");
            strCountry = strCountry.Replace("philippiens", "Philippine");
            strCountry = strCountry.Replace("polonais", "Pologne");
            strCountry = strCountry.Replace("portugais", "Portugal");
            strCountry = strCountry.Replace("roumain", "Roumanie");
            strCountry = strCountry.Replace("russe", "Russie");
            strCountry = strCountry.Replace("sénégalais", "Sénégal");
            strCountry = strCountry.Replace("serbe", "Serbie");
            strCountry = strCountry.Replace("serbo-croate", "Serbie, Croatie");
            strCountry = strCountry.Replace("singapourien", "Singapour");
            strCountry = strCountry.Replace("slovaque", "Slovaquie");
            strCountry = strCountry.Replace("soviétique", "URSS");
            strCountry = strCountry.Replace("sri-lankais", "Sri-Lanka");
            strCountry = strCountry.Replace("sud-africain", "Afrique du Sud");
            strCountry = strCountry.Replace("sud-coréenne", "Corée du Sud");
            strCountry = strCountry.Replace("sud-coréen", "Corée du Sud");
            strCountry = strCountry.Replace("suédois", "Suède");
            strCountry = strCountry.Replace("suisse", "Suisse");
            strCountry = strCountry.Replace("tadjik", "Tadjikistan");
            strCountry = strCountry.Replace("taïwanais", "Taïwan");
            strCountry = strCountry.Replace("tchadien", "Tchad");
            strCountry = strCountry.Replace("tchèque", "République Tchèque");
            strCountry = strCountry.Replace("thaïlandais", "Thaïlande");
            strCountry = strCountry.Replace("tunisien", "Tunisie");
            strCountry = strCountry.Replace("turc", "Turquie");
            strCountry = strCountry.Replace("usa", "USA");
            strCountry = strCountry.Replace("ukranien", "Ukraine");
            strCountry = strCountry.Replace("uruguayen", "Uruguay");
            strCountry = strCountry.Replace("vénézuélien", "Vénézuéla");
            strCountry = strCountry.Replace("vietnamien", "Vietnam");
            strCountry = strCountry.Replace("yougoslave", "Yougoslavie");
            strCountry = strCountry.Replace("république République", "République");
            return strCountry;
        }
 
        /// <summary>
        /// This method will create a string that can be safely used as a filename.
        /// </summary>
        /// <param name="subject">the string to process</param>
        /// <returns>the processed string</returns>
        public static string CreateFilename(string subject)
        {
            if (String.IsNullOrEmpty(subject))
                return string.Empty;

            string rtFilename = subject;

            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char invalidFileChar in invalidFileChars)
                rtFilename = rtFilename.Replace(invalidFileChar, '_');

            return rtFilename;
        }
        public static void DownloadCoverArt(string stPath, string imageUrl, string title, out string filename)
        {
            filename = "";
            try
            {
                if (imageUrl.Length > 0 && stPath.Length > 0)
                {

                    string Extension = MediaPortal.Util.Utils.GetThumbExtension();
                    string coverArtImage = MediaPortal.Util.Utils.GetCoverArtName(stPath, title);

                    if (!System.IO.File.Exists(coverArtImage))
                    {
                        MediaPortal.Util.Utils.FileDelete(coverArtImage);
                    }

                    filename = MediaPortal.Util.Utils.MakeFileName(title + Extension);

                    MediaPortal.Util.Utils.DownLoadImage(imageUrl, stPath + '\\' + filename);


                }

            }
            catch (Exception)
            {
            }

        }

        public static string DownloadBacdropArt(string artFolder, string imageUrl, string title, bool multiImage, bool first, out string filename)
        {
            filename = "";
            
              // check if the image file is already in the backdrop folder
            string safeName = CreateFilename(title.ToLower()).Replace(' ', '.');
            string dirname = artFolder + "\\{" + safeName + "}";
            if (!System.IO.Directory.Exists(dirname))
                System.IO.Directory.CreateDirectory(dirname);
            if (first)
                filename = dirname + "\\{" + safeName + "}.jpg";
            else
                filename = dirname + "\\{" + safeName + "} [" + imageUrl.GetHashCode() + "].jpg";
            FileInfo newFile = new FileInfo(filename);
            bool alreadyInFolder = newFile.Exists;

            // if the file isnt in the backdrop folder, generate a name and save it there
            if (!alreadyInFolder)
            {
                try
                {
                    if (imageUrl.Length > 0 && artFolder.Length > 0)
                    {
                        System.Drawing.Image newBackdrop = GetImageFromUrl(imageUrl);

                        if (newBackdrop == null)
                            return string.Empty; 

                        newBackdrop.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                        newBackdrop.Dispose();
                        return filename;
                    }
                    else
                        return string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            else
                return "already";
        }


        // trys to get a webpage from the specified url and returns the content as string
        public static string GetPage(string strURL, string strEncode, out string absoluteUri, CookieContainer cookie)
        {
            string strBody = "";
            absoluteUri = string.Empty;
            Stream ReceiveStream = null;
            StreamReader sr = null;
            HttpWebResponse result = null;
            try
            {
                // Make the Webrequest
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strURL);
                req.CookieContainer = cookie;
                SetAllowUnsafeHeaderParsing();

                result = (HttpWebResponse)req.GetResponse();

                ReceiveStream = result.GetResponseStream();

                //                if (strEncode == null)
                strEncode = result.CharacterSet;
                if (strEncode == "ISO-8859-1")
                    strEncode = "Windows-1252";

                // Encoding: depends on selected page
                Encoding encode = System.Text.Encoding.GetEncoding(strEncode);
                sr = new StreamReader(ReceiveStream, encode);
                strBody = sr.ReadToEnd();

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
                    catch (Exception)
                    {
                    }
                }
                if (ReceiveStream != null)
                {
                    try
                    {
                        ReceiveStream.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
                if (result != null)
                {
                    try
                    {
                        result.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return strBody;
        } // END GetPage()


        public static string encodeSearch(string strSearch)
        {
            strSearch = strSearch.Replace("â", "a");
            strSearch = strSearch.Replace("ê", "e");
            strSearch = strSearch.Replace("î", "i");
            strSearch = strSearch.Replace("ô", "o");
            strSearch = strSearch.Replace("û", "u");
            strSearch = strSearch.Replace("´", " ");
            strSearch = HttpUtility.UrlEncode(strSearch);
            strSearch = strSearch.Replace("%c3%a4", "%E4");
            strSearch = strSearch.Replace("%c3%b6", "%F6");
            strSearch = strSearch.Replace("%c3%bc", "%FC");
            strSearch = strSearch.Replace("%c3%9f", "%DF");
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

            return strSearch;
        }

        public static string convertNote(string strNote)
        {
            string strTemp = strNote;

            strTemp = strTemp.Replace("-", ",");
            strTemp = strTemp.Replace(".", ",");

            return strTemp;

        }

        public static bool SetAllowUnsafeHeaderParsing()
        {

            Assembly aNetAssembly = Assembly.GetAssembly(
                typeof(System.Net.Configuration.SettingsSection));

            if (aNetAssembly != null)
            {

                Type aSettingsType = aNetAssembly.GetType(
                    "System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType != null)
                {
                    object anInstance = aSettingsType.InvokeMember("Section",
                       BindingFlags.Static | BindingFlags.GetProperty
                       | BindingFlags.NonPublic, null, null, new object[] { });
                    if (anInstance != null)
                    {
                        FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField(
                         "useUnsafeHeaderParsing",
                         BindingFlags.NonPublic | BindingFlags.Instance);

                        if (aUseUnsafeHeaderParsing != null)
                        {

                            Console.WriteLine(aUseUnsafeHeaderParsing.GetValue(anInstance).ToString());
                            aUseUnsafeHeaderParsing.SetValue(anInstance, true);

                            Console.WriteLine(aUseUnsafeHeaderParsing.GetValue(anInstance).ToString());
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        // given a URL, returns an image stored at that URL. Returns null if not 
        // an image or connection error.
        public static Image GetImageFromUrl(string url)
        {
            Image rtn = null;

            // pull in timeout settings
            int tryCount = 0;
            int maxRetries = 10;
            int timeout = 5000;
            int timeoutIncrement = 1000;

            while (rtn == null && tryCount < maxRetries)
            {
                try
                {
                    // try to grab the image
                    tryCount++;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = timeout + (timeoutIncrement * tryCount);
                    request.ReadWriteTimeout = 20000;
                    request.UserAgent = "Mozilla/5.0 (Windows; U; MSIE 7.0; Windows NT 6.0; en-US)";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // parse the stream into an image file
                    rtn = Image.FromStream(response.GetResponseStream());
                }
                catch (WebException e)
                {
                    // file doesnt exist
                    if (e.Message.Contains("404"))
                    {
                        // needs to be uncommented when backdrop provider is fleshed out and doesnt ask
                        // for 404 urls to be loaded
                        //logger.Warn("Failed retrieving artwork from " + url + ". File does not exist. (404)");
                        return null;
                    }

                    // if we timed out past our try limit
                    if (tryCount == maxRetries)
                    {
                        //logger.ErrorException("Failed to retrieve artwork from " + url + ". Reached retry limit of " + maxRetries, e);
                        return null;
                    }
                }
                catch (UriFormatException)
                {
                    //logger.Error("Bad URL format, failed loading image: " + url);
                    return null;
                }
                catch (ArgumentException)
                {
                    //logger.Error("URL does not point to an image: " + url);
                    return null;
                }
            }

            if (rtn == null)
            {
                //logger.Error("Failed loading image from url: " + url);
                return null;
            }

            return rtn;
        }

        /// <summary>
        /// Get all files from directory and it's subdirectories.
        /// </summary>
        /// <param name="inputDir"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFilesRecursive(DirectoryInfo directory)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            DirectoryInfo[] subdirectories = new DirectoryInfo[] { };

            try
            {
                fileList.AddRange(directory.GetFiles("*"));
                subdirectories = directory.GetDirectories();
            }
            catch (Exception e)
            {
                Log.Debug("MyFilms - GrabUtil - Catched Exception: " + e.ToString());
            }

            foreach (DirectoryInfo subdirectory in subdirectories)
            {
                try
                {
                    if ((subdirectory.Attributes & FileAttributes.System) == 0)
                        fileList.AddRange(GetFilesRecursive(subdirectory));
                }
                catch (Exception e)
                {
                    Log.Debug("MyFilms - GrabUtil - Catched Exception: " + e.ToString());
                }
            }

            return fileList;
        }
        
        #region String Modification / Regular Expressions Methods

        // Regular expression pattern that matches an "article" that need to be moved for title conversions
        // todo: the articles should really be a user definable setting in the future
        private const string rxTitleSortPrefix = "(the|a|an|ein|das|die|der|les|la|le|el|une|de|het)";

        /// <summary>
        /// Converts a movie title to the display name.
        /// </summary>
        /// <example>
        /// Changes "Movie, The" into "The Movie"
        /// </example>
        /// <param name="title"></param>
        /// <returns>display name</returns>
        public static string TitleToDisplayName(string title)
        {
            Regex expr = new Regex(@"(.+?)(?:, " + rxTitleSortPrefix + @")?\s*$", RegexOptions.IgnoreCase);
            return expr.Replace(title, "$2 $1").Trim();
        }

        /// <summary>
        /// Converts a title to the archive name (sortable title)
        /// </summary>
        /// <example>
        /// Changes "The Movie" into "Movie, The"
        /// </example>
        /// <param name="title"></param>
        /// <returns>archive name</returns>
        public static string TitleToArchiveName(string title)
        {
            Regex expr = new Regex(@"^" + rxTitleSortPrefix + @"\s(.+)", RegexOptions.IgnoreCase);
            return expr.Replace(title, "$2, $1").Trim();
        }

        /// <summary>
        /// Converts a title string to a common format to be used in comparison.
        /// </summary>
        /// <param name="title">the original title</param>
        /// <returns>the normalized title</returns>
        public static string normalizeTitle(string title)
        {
            // Convert title to lowercase culture invariant
            string newTitle = title.ToLowerInvariant();

            // Swap article
            newTitle = TitleToDisplayName(newTitle);

            // Replace non-descriptive characters with spaces
            newTitle = Regex.Replace(newTitle, @"[\.:;\+\-\–\—\―\˜\*]", " ");

            // Remove other non-descriptive characters completely
            newTitle = Regex.Replace(newTitle, @"[\(\)\[\]'`,""\#\$\?]", "");

            // Equalize: Convert to base character string
            newTitle = RemoveDiacritics(newTitle);

            // Equalize: Common characters with words of the same meaning
            newTitle = Regex.Replace(newTitle, @"\b(and|und|en|et|y)\b", " & ");

            // Equalize: Roman Numbers To Numeric
            newTitle = Regex.Replace(newTitle, @"\si(\b)", @" 1$1");
            newTitle = Regex.Replace(newTitle, @"\sii(\b)", @" 2$1");
            newTitle = Regex.Replace(newTitle, @"\siii(\b)", @" 3$1");
            newTitle = Regex.Replace(newTitle, @"\siv(\b)", @" 4$1");
            newTitle = Regex.Replace(newTitle, @"\sv(\b)", @" 5$1");
            newTitle = Regex.Replace(newTitle, @"\svi(\b)", @" 6$1");
            newTitle = Regex.Replace(newTitle, @"\svii(\b)", @" 7$1");
            newTitle = Regex.Replace(newTitle, @"\sviii(\b)", @" 8$1");
            newTitle = Regex.Replace(newTitle, @"\six(\b)", @" 9$1");

            // Remove the number 1 from the end of a title string
            newTitle = Regex.Replace(newTitle, @"\s(1)$", "");

            // Remove double spaces and trim
            newTitle = trimSpaces(newTitle);

            // return the cleaned title
            return newTitle;
        }

        /// <summary>
        /// Translates characters to their base form.
        /// </summary>
        /// <example>
        /// characters: ë, é, è
        /// result: e
        /// </example>
        /// <remarks>
        /// source: http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
        /// </remarks>
        public static string RemoveDiacritics(string title)
        {
            string stFormD = title.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }


        /// <summary>
        /// Removes multiple spaces and replaces them with one space   
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string trimSpaces(string input)
        {
            return Regex.Replace(input, @"\s{2,}", " ").Trim();
        }

        #endregion
    }
}   
