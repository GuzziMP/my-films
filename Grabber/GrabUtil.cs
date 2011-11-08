using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.IO;
using System.Reflection;
using System.Drawing;
using MediaPortal.Util;
using MediaPortal.GUI.Library;
using NLog;

namespace Grabber
{
  using System.Collections;
  using System.Threading;
  using Cornerstone.Tools;


  public class ArtworkInfo
  {
    public ArtworkInfo(string grabstring, string siteURL)
    {
      if (!string.IsNullOrEmpty(grabstring))
        Load(grabstring, siteURL);
    }
    public List<ArtworkItem> ArtworkList
    {
      get { return artworkList; }
      set { artworkList = value; }
    }
    private List<ArtworkItem> artworkList;


    private void Load(string grabstring, string siteURL)
    {
      if (ArtworkList == null)
        ArtworkList = new List<ArtworkItem>();
      ArtworkList.Clear();
      string[] SepElements = new string[] { "," };
      string[] Elements = grabstring.Split(SepElements, StringSplitOptions.RemoveEmptyEntries);
      int count = 0;
      foreach (string s in Elements)
      {
        count += 1;
        ArtworkItem item = new ArtworkItem();

        if (s.Contains("("))
        {
          item.URL = s.Substring(0, s.IndexOf("(")).Trim(new Char[] { '(', ')' }).Trim();
          item.Name = s.Substring(s.IndexOf("(")).Trim(new Char[] { '(', ')' }).Trim();
        }
        else
        {
          item.URL = s.Trim();
          item.Name = "";
        }
        if (!item.URL.StartsWith("http"))
        {
          if (siteURL.EndsWith(@"/")) siteURL.TrimEnd(new Char[] { '/' });
          if (item.URL.StartsWith(@"/"))
            item.URL = siteURL + item.URL;
          else
            item.URL = siteURL + @"/" + item.URL;
        }

        if (string.IsNullOrEmpty(item.Name))
          item.Name = "Image #" + count + "'";
        ArtworkList.Add(item);
      }
    }

    public class ArtworkItem
    {
      public string Name
      {
        get { return name; }
        set { name = value; }
      } private string name;

      public string URL
      {
        get { return url; }
        set { url = value; }
      } private string url;

      public string LocalPath
      {
        get { return localPath; }
        set { localPath = value; }
      } private string localPath;
    }
  }

  public class ArtworkInfoItem
  {
    public ArtworkInfoItem()
    {
      
    }
    
    public string Name
    {
      get { return name; }
      set { name = value; }
    } private string name;

    public string URL
    {
      get { return url; }
      set { url = value; }
    } private string url;

    public string LocalPath
    {
      get { return localPath; }
      set { localPath = value; }
    } private string localPath;
  }


  public class GrabUtil
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

        public enum Artwork_Thumb_Mode
        {
          Cover,
          Fanart
        }

        public enum Artwork_Fanart_Type
        {
          Multiimage,
          MultipleSingleImages,
          Snapshotimage,
          MultiImageWithMultipleSingleImages
        }

        private enum MediaInfo_Output : int
        {
            Filename = 0,
            VideoFormat = 1,
            VideoBitrate = 2,
            Framerate = 3,
            Resolution = 4,
            AudioBitrate = 5,
            AudioFormat = 6,
            Runtime = 7,
            Filesize = 8,
            Audiostreamcount = 9,
            Audiostreamcodeclist = 10,
            Audiostreamlanguagelist = 11,
            Textstreamcodeclist = 12,
            Textstreamlanguagelist = 13,
            AspectRatio = 14,
            Date = 15
        }

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
                iStart = FindRegEx(Body, KeyStart, iStart, ref iLength, true) + iStart;
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
                  iStart = FindRegEx(Body, KeyStart, iStart, ref iLength, true) + iStart;
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

        public static int FindPosition(string Body, string strStartKey, int iStartIndex, ref int iLength, bool firstMatch, bool ReturnEndPositionOfMatch)
        {
          int iIndex = -1;
          bool bregexs = false;
          if (strStartKey.StartsWith("#REGEX#"))
            bregexs = true;
          if (strStartKey != "")
          {
            iLength = strStartKey.Length;
            if (bregexs)
            {
              iIndex = FindRegEx(Body, strStartKey, iStartIndex, ref iLength, firstMatch) + iStartIndex;
              if (iLength == 0) //  || iLength == Body.Length // if fill select is not intended
                iIndex = -1;
              if (iIndex >= 0 && ReturnEndPositionOfMatch) 
                iIndex += iLength;
            }
            else
            {
              iIndex = Body.IndexOf(strStartKey, iStartIndex, StringComparison.InvariantCulture);
              if (iIndex >= 0 && ReturnEndPositionOfMatch)
                iIndex += strStartKey.Length;
            }
          }
          return iIndex;
        }
    
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2)
        {
          return FindWithAction(Body, KeyStart, KeyEnd, param1, param2, string.Empty, string.Empty, string.Empty);
        }
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2, string param3)
        {
          return FindWithAction(Body, KeyStart, KeyEnd, param1, param2, param3, string.Empty);
        }
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2, string param3, string maxItems)
        {
          return FindWithAction(Body, KeyStart, KeyEnd, param1, param2, param3, maxItems, string.Empty);
        }
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2, string param3, string maxItems, string languages)
        {
          string allNames = string.Empty;
          string allRoles = string.Empty;
          return FindWithAction(Body, KeyStart, KeyEnd, param1, param2, param3, maxItems, languages, out allNames, out allRoles);
        }
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2, string param3, string maxItems, string languages, out string allNames, out string allRoles)
        {
          return FindWithAction(Body, KeyStart, KeyEnd, param1, param2, param3, maxItems, languages, out allNames, out allRoles, true);
        }
        public static string FindWithAction(string Body, string KeyStart, string KeyEnd, string param1, string param2, string param3, string maxItems, string languages, out string allNames, out string allRoles, bool grabActorRoles)
        {
            allNames = string.Empty;
            allRoles = string.Empty;
            int iStart = 0;
            int iEnd = 0;
            int iLength = 0;
            int maxItemsToAdd = 999; // Max number of items to add to matchgroup
            if (!string.IsNullOrEmpty(maxItems))
              maxItemsToAdd = Convert.ToInt32(maxItems);
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
                          //if (param3.Length > 0)
                          //{
                          //  Regex oRegex = new Regex(param3);
                          //  Regex oRegexReplace = new Regex(string.Empty);
                          //  System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(strTemp);
                          //  foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                          //  {
                          //    if (param1.StartsWith("#REGEX#"))
                          //    {
                          //      oRegexReplace = new Regex(param1.Substring(7));
                          //      strTemp = strTemp.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, param2));
                          //    }
                          //    else
                          //      strTemp = strTemp.Replace(param1, param2);
                          //  }
                          //}
                          //else
                          //{
                          //  if (param1.StartsWith("#REGEX#"))
                          //    strTemp = Regex.Replace(strTemp, param1.Substring(7), param2);
                          //  else
                          //    if (param1.Length > 0)
                          //      strTemp = strTemp.Replace(param1, param2);
                          //}
                          if (param3.Length > 0)
                          {
                            System.Text.RegularExpressions.RegexOptions regexoption = new RegexOptions();
                            regexoption = RegexOptions.Singleline;
                            if (param3.StartsWith("#MULTILINE#"))
                            {
                              regexoption = RegexOptions.Multiline;
                              param3 = param3.Substring(11);
                            }
                              
                            //Regex oRegex = new Regex(param3, RegexOptions.Singleline);
                            Regex oRegex = new Regex(param3, regexoption);
                            Regex oRegexReplace = new Regex(string.Empty);
                            strTemp = HttpUtility.HtmlDecode(strTemp);
                            if (regexoption != RegexOptions.Multiline)
                              strTemp = HttpUtility.HtmlDecode(strTemp).Replace("\n", "");
                            // System.Windows.Forms.Clipboard.SetDataObject(strTemp, false); // Must not be set when called by AMCupdater -> STAThread exception !
                            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(strTemp);

                            string strActor = string.Empty;
                            string strRole = string.Empty;

                            if (oMatches.Count > 0)
                            {
                              string strCastDetails = "";
                              int i = 0;
                              foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                              {
                                strActor = string.Empty;
                                strActor = oMatch.Groups["person"].Value;
                                strActor = Utils.stripHTMLtags(strActor).Trim().Replace("\n", "");
                                //strActor = HttpUtility.HtmlDecode(strActor).Replace(",", ";");

                                strRole = string.Empty;
                                strRole = oMatch.Groups["role"].Value;
                                strRole = Utils.stripHTMLtags(strRole).Trim().Replace("\n", "");
                                //strRole = HttpUtility.HtmlDecode(strRole).Replace(",", ";");

                                if (param1.Length > 0)
                                {
                                  if (param1.StartsWith("#REGEX#"))
                                  {
                                    oRegexReplace = new Regex(param1.Substring(7));
                                    if (!string.IsNullOrEmpty(strActor))
                                      strActor = strActor.Replace(strActor, oRegexReplace.Replace(strActor, param2)).Trim();
                                    if (!string.IsNullOrEmpty(strRole)) 
                                      strRole = strRole.Replace(strRole, oRegexReplace.Replace(strRole, param2)).Trim();
                                  }
                                  else if (param1.Length > 0)
                                  {
                                    if (!string.IsNullOrEmpty(strActor)) 
                                      strActor = strActor.Replace(param1, param2).Trim();
                                    if (!string.IsNullOrEmpty(strRole)) 
                                      strRole = strRole.Replace(param1, param2).Trim();
                                  }
                                }

                                // build allNames & allRoles strings for dropdowns
                                if (!string.IsNullOrEmpty(allNames)) allNames += ", ";
                                allNames += strActor;
                                if (!string.IsNullOrEmpty(allRoles)) allRoles += ", ";
                                allRoles += strRole;

                                if (i < maxItemsToAdd) // Limit number of items to add
                                {
                                  string[] langSplit = languages.Split(new Char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                  if (string.IsNullOrEmpty(languages))
                                  {
                                    if (!string.IsNullOrEmpty(strCastDetails)) strCastDetails += ", ";
                                    strCastDetails += strActor;
                                    if (strRole != string.Empty && grabActorRoles) 
                                      strCastDetails += " (" + strRole + ")";
                                    //strCastDetails += "\n";
                                    i = i + 1;
                                  }
                                  else
                                  {
                                    foreach (var s in langSplit)
                                    {
                                      if (strActor.ToLower().Contains(s.Trim().ToLower()) || strRole.ToLower().Contains(s.Trim().ToLower()))
                                      {
                                        if (!string.IsNullOrEmpty(strCastDetails)) strCastDetails += ", ";
                                        strCastDetails += strActor;
                                        if (strRole != string.Empty && grabActorRoles)
                                          strCastDetails += " (" + strRole + ")";
                                            // Don't add groupnames when adding TTitles
                                        //strCastDetails += "\n";
                                        i = i + 1;
                                      }
                                    }
                                    if (param1.StartsWith("#REGEX#")) strCastDetails = Regex.Replace(strCastDetails, param1.Substring(7), param2);
                                    else if (param1.Length > 0) strCastDetails = strCastDetails.Replace(param1, param2);
                                  }
                                }
                                //i = i + 1;
                              }
                              strTemp = strCastDetails;
                            }
                            else // no matchcollection found, do cleanup on original string
                            {
                              if (param1.StartsWith("#REGEX#")) strTemp = Regex.Replace(strTemp, param1.Substring(7), param2);
                              else if (param1.Length > 0) strTemp = strTemp.Replace(param1, param2);
                            }

                            string[] split = allNames.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
                            string strT = string.Empty;
                            string strTname = string.Empty;
                            string strTrole = string.Empty;
                            foreach (var str in split)
                            {
                              strT = str.Trim();
                              if (!string.IsNullOrEmpty(strTname)) strTname += ", ";
                              strTname += strT;
                            }
                            allNames = strTname;
                            
                            split = allRoles.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
                            strT = string.Empty;
                            strTname = string.Empty;
                            strTrole = string.Empty;
                            foreach (var str in split)
                            {
                              strT = str.Trim();
                              if (!string.IsNullOrEmpty(strTrole)) strTrole += ", ";
                              strTrole += strT;
                            }
                            allRoles = strTrole;
                          }
                          else
                          {
                            if (param1.StartsWith("#REGEX#"))
                              strTemp = Regex.Replace(strTemp, param1.Substring(7), param2);
                            else
                              if (param1.Length > 0)
                                strTemp = strTemp.Replace(param1, param2);
                            //System.Windows.Forms.Clipboard.SetDataObject(strTemp, false); // Must NOT be called, when using from AMCupdater, cause it's giving STAThread error !!!
                          }

                          // Added to enable the addition of surrounding chars for results...
                          if (param1.StartsWith("#ADD#") && param2.Length > 0)
                          {
                            string strExtend = param2;
                            string[] strExtendSplit = strExtend.Split(new Char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (strExtendSplit.Length == 2)
                            {
                              strTemp = strExtendSplit[0] + strTemp.Trim() + strExtendSplit[1];
                            }
                            else
                            {
                              strTemp = strExtend + strTemp.Trim() + strExtend;
                            }
                          }

                          strTemp = strTemp.Replace("„", "'").Replace("“", "'");
                          htmlUtil.RemoveTags(ref strTemp);
                          htmlUtil.ConvertHTMLToAnsi(strTemp, out strTemp);
                          // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                          // strTemp = strTemp.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        } 
                    }
                }

            }
            return strTemp.Trim();
        }

        // Method for movie Search (NOT Details...)
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
                            strTemp = strTemp.Replace("„", "'").Replace("“", "'");
                            htmlUtil.RemoveTags(ref strTemp);
                            htmlUtil.ConvertHTMLToAnsi(strTemp, out strTemp);
                            // strTemp = strTemp.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        }
                    }
                }

            }
            return strTemp.Trim();
        }

        public static int FindRegEx(string text, string Key, int iStart, ref int iLength, bool firstMatch)
        {
            int imatch = 0;
            try
            {
            Regex p = new Regex(Key.Substring(7), RegexOptions.Singleline);
            text = text.Substring(iStart);
            iLength = 0;

            MatchCollection MatchList = p.Matches(text);
            if (MatchList.Count == 0)
                return 0;
            Match matcher = MatchList[0];           
            if (!firstMatch)
                matcher = MatchList[MatchList.Count-1];                           

            imatch = matcher.Index;
            iLength = matcher.Length;
            }
            catch (Exception)
            {
            }
            return imatch;
        }

        public static string transformCountry(string strCountry)
        {
            // strCountry = strCountry.ToLower(); // Removed to keep countries in upper case
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

        // returns an array of the names and downloadlinks of images. Use filterstring to only add items where name contains filterstring (e.g. "german" for moviegoods only returns german covers)
        public static List<ArtworkInfoItem> GetMultiImageList(string siteURL, string GrabberImageList, string filterstring)
        {
          List<ArtworkInfoItem> imagelist = new List<ArtworkInfoItem>();
          string[] split = GrabberImageList.Trim().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          foreach (string s in split)
          {
            ArtworkInfoItem item = new ArtworkInfoItem();
            if (s.Contains("("))
            {
              item.URL = s.Substring(0, s.IndexOf("(")).Trim(new Char[] { '(',')' }).Trim();
              item.Name = s.Substring(s.IndexOf("(")).Trim(new Char[] { '(', ')' }).Trim();
            }
            else
            {
              item.URL = s.Trim();
            }
            if (!item.URL.StartsWith("http"))
            {
              if(siteURL.EndsWith(@"/")) siteURL.TrimEnd(new Char[] { '/' });
              if (item.URL.StartsWith(@"/")) 
                item.URL = siteURL + item.URL;
              else
                item.URL = siteURL + @"/" + item.URL;
            }

            if (string.IsNullOrEmpty(filterstring) || item.Name.Contains(filterstring))
              imagelist.Add(item);
          }
          return imagelist;
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

              // first load image to memory (allows download validation before deleting original image!)
              string resolutionMin = "";
              string resolutionMax = "";
              bool allowdownload = true;
              System.Drawing.Image newCoverImage = GetImageFromUrl(imageUrl);

              if (newCoverImage != null)
              {
                // check if resolution limit applies
                if (!string.IsNullOrEmpty(resolutionMin) || !string.IsNullOrEmpty(resolutionMax))
                {
                  int resX = 0;
                  int resY = 0;
                  if (!string.IsNullOrEmpty(resolutionMin) && resolutionMin.Contains("x"))
                  {
                    string[] Sep = { "x", "(", "]" };
                    string[] split = resolutionMin.Split(Sep, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (int.TryParse(split[0].Trim(), out resX)) if (newCoverImage.Width < resX) allowdownload = false;
                    if (int.TryParse(split[1].Trim(), out resY)) if (newCoverImage.Height < resY) allowdownload = false;
                  }
                  if (!string.IsNullOrEmpty(resolutionMax) && resolutionMax.Contains("x"))
                  {
                    string[] Sep = { "x", "(", "]" };
                    string[] split = resolutionMax.Split(Sep, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (int.TryParse(split[0].Trim(), out resX)) if (newCoverImage.Width > resX) allowdownload = false;
                    if (int.TryParse(split[1].Trim(), out resY)) if (newCoverImage.Height > resY) allowdownload = false;
                  }
                }
              }

              if (!allowdownload) filename = "";
              else
              {
                if (newCoverImage != null)
                {
                  if (System.IO.File.Exists(coverArtImage)) // Guzzi: changed to "if exist" - as it didn't delete existing images, thus preventing download ...
                  {
                    MediaPortal.Util.Utils.FileDelete(coverArtImage);
                  }
                  filename = MediaPortal.Util.Utils.MakeFileName(title + Extension);
                  //MediaPortal.Util.Utils.DownLoadImage(imageUrl, stPath + '\\' + filename);
                  //DownLoadImage(imageUrl, stPath + '\\' + filename);
                  newCoverImage.Save(stPath + '\\' + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                  newCoverImage.Dispose();
                }
              }

              //if (System.IO.File.Exists(coverArtImage)) // Guzzi: changed to "if exist" - as it didn't delete existing images, thus preventing download ...
              //{
              //    MediaPortal.Util.Utils.FileDelete(coverArtImage);
              //}

              //filename = MediaPortal.Util.Utils.MakeFileName(title + Extension);
              //// filename = CreateFilename(filename); // added by Guzzi to fix chars not handles by MP cleanup

              ////MediaPortal.Util.Utils.DownLoadImage(imageUrl, stPath + '\\' + filename);
              //DownLoadImage(imageUrl, stPath + '\\' + filename);
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("DownloadCoverart: Exception: " + ex.ToString());
            filename = "";
          }
          if (!System.IO.File.Exists(stPath + '\\' + filename)) // add check, if download was successful to avoid crashes in grabber clients
            filename = "";
        }


        public static bool GetCoverartFromMovie(string FileName, string OutputThumbName, Grabber.GrabUtil.Artwork_Thumb_Mode CoverType, bool CreateIndividualShots, bool KeepMainImage, int SnapshotPosition)
        {
          if (FileName.Contains("VIDEO_TS\\VIDEO_TS.IFO") || !System.IO.File.Exists(FileName)) // Do not try to create thumbnails for DVDs or nonexisting media files
            return false;
          bool success = false;
          string ar = "";
          int columns = 2;
          int rows = 4;
          double arValue = 0;
          string Type = "Cover";

          MediaInfo mediainfo = new MediaInfo();
          mediainfo.Open(FileName);
          if (mediainfo.VideoAspectRatio != null) 
            ar = mediainfo.VideoAspectRatio;
          else 
            ar = "";
          mediainfo.Close();

          try
          {
            arValue = Double.Parse(ar, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
          }
          catch (Exception) {}

          switch (CoverType)
          {
            case Grabber.GrabUtil.Artwork_Thumb_Mode.Cover:
              Type = "Cover";
                if (arValue < 1.4) // 4:3
                {
                  columns = 2;
                  rows = 4;
                }
                else if (arValue < 1.9) // 16:9
                {
                  columns = 2;
                  rows = 5;
                }
              else if (arValue >= 1.9) // cinemascope
              {
                columns = 2;
                rows = 6;
              }
              break;
            case Grabber.GrabUtil.Artwork_Thumb_Mode.Fanart:
            default:
              Type = "Fanart";
                if (arValue < 1.4) // 4:3
                {
                  columns = 4;
                  rows = 3;
                }
                else if (arValue < 1.9) // 16:9
                {
                  columns = 4;
                  rows = 4;
                }
              else if (arValue >= 1.9) // cinemascope
              {
                columns = 3;
                rows = 4;
              }
              break;
          }
          try
          {
            success = Grabber.ThumbCreator.CreateVideoThumbForAMCupdater(FileName, OutputThumbName, false, columns, rows, Type, CreateIndividualShots, KeepMainImage, SnapshotPosition);
          }
          catch (Exception)
          {
            //LogMyFilms.Error("Could not create thumbnail for {0}", MovieFile);
            //LogMyFilms.Error(ex);
          }
          return success;
        }

        public static bool GetFanartFromMovie(string title, string year, string artFolder, Artwork_Fanart_Type FanartType, string MovieFile, string newFanartThumbName, int SnapshotPosition)
        {
          string filename = string.Empty;

          // check if the image file is already in the backdrop folder
          string safeName = CreateFilename(title.ToLower()).Replace(' ', '.');
          string dirname = artFolder + "\\{" + safeName + "}";
          if (!System.IO.Directory.Exists(dirname))
            System.IO.Directory.CreateDirectory(dirname);

          filename = dirname + "\\{" + safeName + "}.jpg";
          switch (FanartType)
          {
            case Artwork_Fanart_Type.Multiimage:
              filename = dirname + "\\{" + safeName + "} [mosaic].jpg";
              break;
            case Artwork_Fanart_Type.Snapshotimage:
              filename = dirname + "\\{" + safeName + "} [snapshot-" + CreateFilename(SnapshotPosition.ToString().ToLower()).Replace(' ', '.') + "].jpg";
              //filename = dirname + "\\{" + safeName + "} - " + System.IO.Path.GetFileNameWithoutExtension(newFanartThumbName) + ".jpg";
              //filename = dirname + "\\{" + safeName + "}.jpg";
              break;
            case Artwork_Fanart_Type.MultipleSingleImages:
              filename = dirname + "\\{" + safeName + "} [single].jpg";
              break;
            case Artwork_Fanart_Type.MultiImageWithMultipleSingleImages:
              filename = dirname + "\\{" + safeName + "} [multi].jpg";
              break;
          }
          FileInfo newFile = new FileInfo(filename);
          bool alreadyInFolder = newFile.Exists;

          // if the file isnt in the backdrop folder, generate a name and save it there
          if (!alreadyInFolder)
            switch (FanartType)
            {
              case Artwork_Fanart_Type.Multiimage:
                GetCoverartFromMovie(MovieFile, filename, Artwork_Thumb_Mode.Fanart, false, true, SnapshotPosition);
                break;
              case Artwork_Fanart_Type.Snapshotimage:
                GetCoverartFromMovie(MovieFile, filename, Artwork_Thumb_Mode.Fanart, false, true, SnapshotPosition);
                break;
              case Artwork_Fanart_Type.MultipleSingleImages:
                GetCoverartFromMovie(MovieFile, filename, Artwork_Thumb_Mode.Fanart, true, false, SnapshotPosition);
                break;
              case Artwork_Fanart_Type.MultiImageWithMultipleSingleImages:
                GetCoverartFromMovie(MovieFile, filename, Artwork_Thumb_Mode.Fanart, true, true, SnapshotPosition);
                break;
            }
          if (File.Exists(filename))
            return true;
          else
            return false;
        }

        public static string DownloadBacdropArt(string artFolder, string imageUrl, string title, bool multiImage, bool first, out string filename)
        {
          return DownloadBacdropArt(artFolder, imageUrl, title, multiImage, first, out filename, 0, string.Empty, string.Empty);
        }
        public static string DownloadBacdropArt(string artFolder, string imageUrl, string title, bool multiImage, bool first, out string filename, int downloadlimit, string resolutionMin, string resolutionMax)
        {
          filename = string.Empty;
          bool allowdownload = true;

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

          if (downloadlimit > 0) // check if download limit applies
          {
            int imagecount = 0; // count existing fanarts
            System.IO.DirectoryInfo dir = new DirectoryInfo(dirname);
            string[] files = System.IO.Directory.GetFiles(dirname, @"*.jpg", SearchOption.AllDirectories);
            imagecount += files.Length;
            files = System.IO.Directory.GetFiles(dirname, @"*.png", SearchOption.AllDirectories);
            imagecount += files.Length;
            if (imagecount >= downloadlimit)
            {
              allowdownload = false; 
              return "numberlimit reached!";
            }
          }

          // if the file isnt in the backdrop folder and download is allowd (that is: not restriced tue to resolution or number limits), generate a name and save it there
          if (!alreadyInFolder && allowdownload)
          {
            try
            {
              if (imageUrl.Length > 0 && artFolder.Length > 0)
              {
                System.Drawing.Image newBackdrop = GetImageFromUrl(imageUrl);

                if (newBackdrop == null)
                  return string.Empty;

                if (!string.IsNullOrEmpty(resolutionMin) || !string.IsNullOrEmpty(resolutionMax)) // check if resolution limit applies
                {
                  int resX = 0;
                  int resY = 0;
                  if (!string.IsNullOrEmpty(resolutionMin) && resolutionMin.Contains("x"))
                  {
                    string[] Sep = { "x", "(", "]" };
                    string[] split = resolutionMin.Split(Sep, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (int.TryParse(split[0].Trim(), out resX))
                      if (newBackdrop.Width < resX) allowdownload = false;
                    if (int.TryParse(split[1].Trim(), out resY))
                      if (newBackdrop.Height < resY) allowdownload = false;
                  }
                  if (!string.IsNullOrEmpty(resolutionMax) && resolutionMax.Contains("x"))
                  {
                    string[] Sep = { "x", "(", "]" };
                    string[] split = resolutionMax.Split(Sep, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (int.TryParse(split[0].Trim(), out resX))
                      if (newBackdrop.Width > resX) allowdownload = false;
                    if (int.TryParse(split[1].Trim(), out resY))
                      if (newBackdrop.Height > resY) allowdownload = false;
                  }
                  if (!allowdownload)
                  {
                    newBackdrop.Dispose();
                    return "resolution requirements missed!";
                  }
                }
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

        public static string DownloadPersonArtwork(string artFolder, string imageUrl, string name, bool multiImage, bool first, out string filename)
        {
          filename = string.Empty;

          // check if the image file is already in the backdrop folder
          //string safeName = CreateFilename(name.ToLower()).Replace(' ', '.');
          string safeName = CreateFilename(name.ToLower());
          //string dirname = artFolder + "\\{" + safeName + "}";
          string dirname = artFolder;
          if (!System.IO.Directory.Exists(dirname))
            System.IO.Directory.CreateDirectory(dirname);
          if (first)
            //filename = dirname + "\\{" + safeName + "}.jpg";
            filename = dirname + "\\" + safeName + ".jpg";
          else
            //filename = dirname + "\\{" + safeName + "} [" + imageUrl.GetHashCode() + "].jpg";
            filename = dirname + "\\" + safeName + " [" + imageUrl.GetHashCode() + "].jpg";
          FileInfo newFile = new FileInfo(filename);
          bool alreadyInFolder = newFile.Exists;

          // if the file isnt in the backdrop folder, generate a name and save it there
          if (!alreadyInFolder)
          {
            try
            {
              if (imageUrl.Length > 0 && artFolder.Length > 0)
              {
                System.Drawing.Image newPersonImage = GetImageFromUrl(imageUrl);

                if (newPersonImage == null)
                  return string.Empty;

                newPersonImage.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                newPersonImage.Dispose();
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

        public static string DownloadCovers(string artFolder, string imageUrl, string title, bool multiImage, bool first, out string filename)
        {
          filename = string.Empty;

          // check if the image file is already in the cover folder
          //string safeName = CreateFilename(title.ToLower()).Replace(' ', '.');
          if (title.EndsWith(".jpg")) 
            title = title.Substring(0, title.Length - 4);
          string safeName = CreateFilename(title);
          string dirname = artFolder;
          // string directory = "";
          //if (dirname.Length > dirname.LastIndexOf("\\")) 
          //  directory = dirname.Substring(dirname.LastIndexOf("\\"));
          //if (!System.IO.Directory.Exists(directory))
          //  System.IO.Directory.CreateDirectory(directory);
          if (first && !System.IO.File.Exists(dirname + safeName + ".jpg"))
            filename = dirname + safeName + ".jpg";
          else
            filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";
          FileInfo newFile = new FileInfo(filename);
          bool alreadyInFolder = newFile.Exists;

          // if the file isnt in the cover folder, generate a name and save it there
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
            //return "already";
            return filename;
        }

        // gets string content from a web URL
        private static string RetrieveUrl(string url)
        {
          string pageContents = "";
          // Try to grab the document
          try
          {
            WebGrabber grabber = new WebGrabber(url);
            grabber.Request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            //grabber.UserAgent = parsedUserAgent;
            grabber.MaxRetries = 10;
            grabber.Timeout = 5000;
            grabber.TimeoutIncrement = 1000;
            //grabber.Encoding = Encoding.UTF8;
            //grabber.Encoding = encoding;
            //grabber.AllowUnsafeHeader = true;
            //grabber.CookieHeader = cookies;
            //grabber.Debug = ScriptSettings.DebugMode;

            // Retrieve the document
            if (grabber.GetResponse())
            {
              pageContents = grabber.GetString();
            }
          }
          catch (Exception e)
          {
            if (e is ThreadAbortException)
              throw e;
            //logger.Warn("Could not connect to " + parsedUrl + ". " + e.Message);
          }
          return pageContents;
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
                //WebRequest req = WebRequest.Create(strURL);
                //try
                //{
                //  req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                //}
                //catch (Exception) { }

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strURL);
                req.CookieContainer = cookie;
                // req.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
                // req.ProtocolVersion.

                SetAllowUnsafeHeaderParsing();

                // result = req.GetResponse();
                result = (HttpWebResponse)req.GetResponse();

                ReceiveStream = result.GetResponseStream();

                if (string.IsNullOrEmpty(strEncode)) // allows overriding manually and otherwise try to detect from content
                  strEncode = result.CharacterSet;
                if (strEncode == "ISO-8859-1")
                    strEncode = "Windows-1252";

                // Encoding: depends on selected page
                Encoding encode = System.Text.Encoding.GetEncoding(strEncode);

                if (strURL.Contains("ofdbgw.") || strURL.Contains("themoviedb.org") || strURL.Contains("cineol.net")) // special handling for those to force switch to UTF-8 encoding
                  encode = System.Text.Encoding.UTF8;
                
                //sr = new StreamReader(ReceiveStream, encode);
                //// sr = new StreamReader(new WebClient().OpenRead(URL));
                //strBody = sr.ReadToEnd();

                using (sr = new StreamReader(ReceiveStream, encode))
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
                    catch (Exception) {}
                }
                if (ReceiveStream != null)
                {
                    try
                    {
                        ReceiveStream.Close();
                    }
                    catch (Exception) {}
                }
                if (result != null)
                {
                    try
                    {
                        result.Close();
                    }
                    catch (Exception) {}
                }
            }

            return strBody;
            //return RetrieveUrl(strURL); // use Cornerstone WebGrabber to read url into string
        } // END GetPage()

        public static string encodeSearch(string strSearch, string encoding)
        {
            strSearch = strSearch.Replace("â", "a");
            strSearch = strSearch.Replace("ê", "e");
            strSearch = strSearch.Replace("î", "i");
            strSearch = strSearch.Replace("ô", "o");
            strSearch = strSearch.Replace("û", "u");
            strSearch = strSearch.Replace("´", " ");
            //strSearch = strSearch.Replace("Ä", " ");
            //strSearch = strSearch.Replace("ä", " ");
            //strSearch = strSearch.Replace("Ö", " ");
            //strSearch = strSearch.Replace("ö", " ");
            //strSearch = strSearch.Replace("Ü", " ");
            //strSearch = strSearch.Replace("ü", " ");
            //strSearch = strSearch.Replace("ß", " ");


            // getting searchstring
            strSearch = HttpUtility.UrlEncode(strSearch);

            if (encoding.ToLower() != "utf-8")
            {
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
            }

            //// Zebons original Cleanups:
            //strSearch = HttpUtility.UrlEncode(strSearch);
            //strSearch = strSearch.Replace("%c3%a4", "%E4");
            //strSearch = strSearch.Replace("%c3%b6", "%F6");
            //strSearch = strSearch.Replace("%c3%bc", "%FC");
            //strSearch = strSearch.Replace("%c3%9f", "%DF");
            //strSearch = strSearch.Replace("%c3%b1", "%F1");
            //strSearch = strSearch.Replace("%c3%a0", "%E0");
            //strSearch = strSearch.Replace("%c3%a1", "%E1");
            //strSearch = strSearch.Replace("%c3%a8", "%E8");
            //strSearch = strSearch.Replace("%c3%a9", "%E9");
            //strSearch = strSearch.Replace("%c3%ac", "%EC");
            //strSearch = strSearch.Replace("%c3%ad", "%ED");
            //strSearch = strSearch.Replace("%c3%b2", "%F2");
            //strSearch = strSearch.Replace("%c3%b3", "%F3");
            //strSearch = strSearch.Replace("%c3%b9", "%F9");
            //strSearch = strSearch.Replace("%c3%ba", "%FA");

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
        /// <param name="directory"></param>
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

        public string[] GetMediaInfo(string file)
        {
          string[] mediainfoData = new string[20];

          Grabber.MediaInfo MI = Grabber.MediaInfo.GetInstance();

          // MediaInfo Object could not be created
          if (null == MI || string.IsNullOrEmpty(file)) 
            return null;

          // Check if File Exists and is not an Image type e.g. ISO (we can't extract mediainfo from that)
          string extension = System.IO.Path.GetExtension(file).ToLower();
          bool isImageFile = VirtualDirectory.IsImageFile(extension);
          FileInfo f = new FileInfo(file);

          if (System.IO.File.Exists(file) && !isImageFile)
          {
            try
            {
              MI.Open(file); // open file in MediaInfo

              // check number of failed attempts at mediainfo extraction                    
              int noAttempts = 0;

              // Get Playtime (runtime)
              string result = MI.VideoPlaytime;
              string LocalPlaytime = result != "-1" ? result : noAttempts.ToString();
              string TempString = "";
              int TempInteger = 0;
              int i = 0;
              string ReturnValue = "";

              bool failed = false;
              if (result != "-1")
              {
                //TempString = f.Name;
                //TempString = TempString.Replace(f.Extension, "");
                //// Put this bit in here to remove the '1of2' type bits using the system variable regex expression.
                //Regex SplitText = new Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)");
                //TempString = SplitText.Replace(TempString, "");
                //Regex SplitText = new Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files);
                //TempString = SplitText.Replace(TempString, "");
                //ReturnValue = TempString;

                mediainfoData[(int)MediaInfo_Output.Filename] = f.Name;

                TempString = MI.VideoPlaytime; //TempString = MI.Get_(StreamKind.General, 0, "PlayTime"); // PlayTime value is in miliseconds!
                int.TryParse(TempString, out TempInteger);
                    if (TempInteger != 0) 
                      ReturnValue = ((long)TempInteger / 60000).ToString();
                    else 
                      ReturnValue = "";
                mediainfoData[(int)MediaInfo_Output.Runtime] = ReturnValue;
                mediainfoData[(int)MediaInfo_Output.VideoFormat] = MI.VideoCodecFormat;

                TempString = MI.VideoBitrate;
                int.TryParse(TempString, out TempInteger);
                        if (TempInteger != 0) ReturnValue = (TempInteger / 1000).ToString();
                        else ReturnValue = "";
                mediainfoData[(int)MediaInfo_Output.VideoBitrate] = ReturnValue;
                mediainfoData[(int)MediaInfo_Output.AudioFormat] = MI.AudioCodecFormat;
                mediainfoData[(int)MediaInfo_Output.Audiostreamcount] = MI.AudioStreamCount;
                mediainfoData[(int)MediaInfo_Output.Audiostreamcodeclist] = MI.AudioStreamCodecList;

                // First get the count if possible
                TempString = "";
                int.TryParse(MI.Get(StreamKind.General, 0, "AudioCount"), out TempInteger);
                if (TempInteger > 0)
                {
                  for (i = 0; i < TempInteger - 1; i++)
                  {
                    TempString = "";
                        // Try to get the 'proper' language for this stream:
                    TempString = MI.Get(StreamKind.Audio, i, "Language/String");
                        if (TempString == "")
                        {
                            // If not, check the IAS value - maybe has a language string there:
                          TempString = MI.Get(StreamKind.General, 0, "IAS" + (i + 1).ToString());
                        }
                        if (TempString != "")
                        {
                            // Build the list:
                            if (ReturnValue == "")
                              ReturnValue = TempString;
                            else 
                              ReturnValue += " / " + TempString;
                        }
                        if (ReturnValue == "")
                        {
                            // Still no value, maybe just put in the number of audio streams?
                            if (MI.Get(StreamKind.General, 0, "AudioCount") != "1") 
                              ReturnValue = MI.Get(StreamKind.General, 0, "AudioCount").ToString();
                        }
                  }  
                }
                else
                {
                    // 'Cannot even get the count of streams - return empty:
                  ReturnValue = "";
                }
                mediainfoData[(int)MediaInfo_Output.Audiostreamlanguagelist] = ReturnValue;

                TempString = MI.AudioBitrate; // TempString = MI.Get(StreamKind.Audio, 0, "BitRate");
                int.TryParse(TempString, out TempInteger);
                if (TempInteger != 0) ReturnValue = ((TempInteger / 1000)).ToString();
                else ReturnValue = "";
                mediainfoData[(int)MediaInfo_Output.AudioBitrate] = ReturnValue;

                ReturnValue = MI.Get(StreamKind.General, 0, "Text_Codec_List");
                mediainfoData[(int)MediaInfo_Output.Textstreamcodeclist] = ReturnValue;

                int.TryParse(MI.Get(StreamKind.General, 0, "TextCount"), out TempInteger);
                if (TempInteger != 0)
                {
                  for (i = 0; i < TempInteger - 1; i++)
                  {
                    TempString = "";
                    TempString = MI.Get(StreamKind.Text, i, "Language/String");
                        if (TempString != "")
                        {
                            // Build the string
                            if (ReturnValue == "") ReturnValue = TempString;
                            else ReturnValue += " / " + TempString;
                            // Check for a subtitle description:
                          TempString = MI.Get(StreamKind.Text, i, "Title");
                            if (TempString != "")
                            {
                              // Clean up the title a bit:
                              TempString = TempString.Replace("<", "");
                              TempString = TempString.Replace(">", "");
                              TempString = TempString.Replace("(", "");
                              TempString = TempString.Replace(")", "");
                              ReturnValue += " (" + TempString + ")";
                            }
                        }
                  }
                }
                mediainfoData[(int)MediaInfo_Output.Textstreamlanguagelist] = ReturnValue;

                TempString = MI.VideoWidth + "x" + MI.VideoHeight;
                if (TempString == "x") TempString = "";
                mediainfoData[(int)MediaInfo_Output.Resolution] = TempString;

                // TempString = MI.Get(StreamKind.Visual, 0, "FrameRate");
                mediainfoData[(int)MediaInfo_Output.Framerate] = MI.VideoFramesPerSecond;

                // "filesize" 'get in MB = divide by 1024 twice
                try
                {
                  TempString = f.Length.ToString();
                  TempInteger = int.Parse(TempString);
                  ReturnValue = (((long)TempInteger / 1048576)).ToString();
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("ERROR : " + ex.Message.ToString());
                  ReturnValue = "";
                }
                mediainfoData[(int)MediaInfo_Output.Filesize] = ReturnValue;

                //'File Created Date
                //'Current System Date
                //'No Date
                // Case "File Created Date"
                // ReturnValue = f.CreationTime.Date;
                // Case "File Modified Date"
                // ReturnValue = f.LastWriteTime.Date;
                // Case "Current System Date"
                ReturnValue = DateTime.Now.Date.ToString();
                // Case "No Date"
                // ReturnValue = String.Empty;
                // Case Else
                //     ReturnValue = String.Empty

                mediainfoData[(int)MediaInfo_Output.Date] = ReturnValue;
                // TempString = MI.Get(StreamKind.Visual, 0, "AspectRatio")
                mediainfoData[(int)MediaInfo_Output.AspectRatio] = MI.VideoAspectRatio;


                //string t;
                //t = MI.VideoPlaytime;
                //t = MI.VideoCodecFormat;
                //t = MI.VideoBitrate;
                //t = MI.AudioCodecFormat;
                //t = MI.AudioStreamCount;
                //t = MI.AudioStreamCodecList;
                //t = MI.AudioChannelCount;
                //t = MI.AudioBitrate;

                //t = MI.AudioCodec;
                //t = MI.AudioFormatProfile;
                //t = MI.SubtitleCount;
                //t = MI.VideoAspectRatio;
                //t = MI.VideoCodec;
                //t = MI.VideoFormatProfile;
                //t = MI.VideoFramesPerSecond;
                //t = MI.VideoHeight;
                //t = MI.VideoWidth;

            //Case "date"
            //    Try
            //        Select Case CurrentSettings.Date_Handling
            //            'File Created Date
            //            'Current System Date
            //            'No Date
            //            Case "File Created Date"
            //                ReturnValue = f.CreationTime.Date
            //            Case "File Modified Date"
            //                ReturnValue = f.LastWriteTime.Date
            //            Case "Current System Date"
            //                ReturnValue = My.Computer.Clock.LocalTime.Date
            //            Case "No Date"
            //                ReturnValue = String.Empty
            //            Case Else
            //                ReturnValue = String.Empty
            //        End Select
            //    Catch ex As Exception
            //        'Console.WriteLine(ex.Message)
            //        LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
            //        ReturnValue = ""
            //    End Try
              }
              else
                failed = true;

              // MediaInfo cleanup
              MI.Close();

              if (failed)
              {
                // Get number of retries left to report to user
                int retries = 3 - (noAttempts * -1);

                string retriesLeft = retries > 0 ? retries.ToString() : "No";
                retriesLeft = string.Format("Problem parsing MediaInfo for: {0}, ({1} retries left)", file.ToString(), retriesLeft);
                // LogMyFilms.Debug(retriesLeft);
              }
              else
              {
                // LogMyFilms.Debug("Succesfully read MediaInfo for ", file.ToString());
              }
              // return true;
              return null;
            }
            catch (Exception)
            {
              // LogMyFilms.Debug("Error reading MediaInfo: ", ex.Message);
            }
          }
          // LogMyFilms.Debug("File '" + file + "' does not exist or is an image file");
          // return false;
          return mediainfoData;
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

      
      
        private static void DownLoadImage(string strURL, string strFile)
        {
          if (string.IsNullOrEmpty(strURL) || string.IsNullOrEmpty(strFile))
            return;

          try
          {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(strURL);
            wr.Timeout = 20000; // Guzzi: changed from 5000 to 20.000 cause IMDB is slow and Pictures loading can be even slower ....
            try
            {
              // Use the current user in case an NTLM Proxy or similar is used.
              // wr.Proxy = WebProxy.GetDefaultProxy();
              wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }
            catch (Exception) { }
            HttpWebResponse ws = (HttpWebResponse)wr.GetResponse();
            try
            {

              using (Stream str = ws.GetResponseStream())
              {
                byte[] inBuf = new byte[900000];
                int bytesToRead = (int)inBuf.Length;
                int bytesRead = 0;

                DateTime dt = DateTime.Now;
                while (bytesToRead > 0)
                {
                  dt = DateTime.Now;
                  int n = str.Read(inBuf, bytesRead, bytesToRead);
                  if (n == 0)
                    break;
                  bytesRead += n;
                  bytesToRead -= n;
                  TimeSpan ts = DateTime.Now - dt;
                  if (ts.TotalSeconds >= 5)
                  {
                    throw new Exception("timeout");
                  }
                }
                using (FileStream fstr = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                  fstr.Write(inBuf, 0, bytesRead);
                  str.Close();
                  fstr.Close();
                }
              }
            }
            finally
            {
              if (ws != null)
              {
                ws.Close();
              }
            }
          }
          catch (Exception ex)
          {
            Log.Info("Utils: DownLoadImage {1} failed:{0}", ex.Message, strURL);
          }
        }



        #endregion
    }
}   
