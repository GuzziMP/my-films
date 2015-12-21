#region GNU license
// MyFilms - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System.ComponentModel;
using System.Diagnostics;
using MyFilmsPlugin.Utils;

namespace MyFilmsPlugin.MyFilms.Utils
{
  using System;
  using System.IO;
  using System.Collections;
  using System.Collections.Generic;
  using System.Data;
  using System.Drawing;
  using System.Linq;
  using System.Text.RegularExpressions;

  using MediaPortal.Configuration;
  using MediaPortal.GUI.Library;
  using MyFilmsGUI;

  public class Logos
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region Variables
    private static ArrayList ID2001Logos = new ArrayList();
    private static ArrayList ID2002Logos = new ArrayList();
    private static ArrayList ID2003Logos = new ArrayList();
    private static ArrayList ID2012Logos = new ArrayList();
    private static string LogosPath = string.Empty; // Source Path for Logos
    private static string LogosPathThumbs = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos\"; // Path for creating Thumbs - hardcoded.
    private static int Spacer = 1; // Added for Logo Spacing, set to 1 for default
    private static string LogoConfigOverride = String.Empty; // Added for differentiate cache files for override configs to be able to change skin from GUI having correct logos
    private static string Country = string.Empty; // Added for Logo subdirectories for e.g. certification - example: Thumbs\MyFilms\DefaultLogos\Certification\de\... (here Country = "de")
    private static bool UseCountryLogos = false;
    
    private static List<string> _logoFileList = null;
    private static List<string> LogoFileList
    {
      get
      {
        if (_logoFileList == null)
        {
          Stopwatch watch = new Stopwatch();
          watch.Restart();
          _logoFileList = new List<string>();
          // _logoFileList.AddRange(Directory.GetFiles(LogosPath, "\\" + Country + "\\", SearchOption.AllDirectories)); // get all files, that match the selected country code
          _logoFileList.AddRange(Directory.GetFiles(LogosPath, "*.*", SearchOption.AllDirectories));
          watch.Stop();
          LogMyFilms.Debug("Logos() - LogoFileList initialized ('" + watch.ElapsedMilliseconds + "' msec.) and loaded '" + _logoFileList.Count + "' elements from '" + LogosPath + "'");
          //foreach (string s in _logoFileList)
          //{
          //  LogMyFilms.Debug("Logos() - LogoFileList content: '" + s + "'");
          //}
        }
        return _logoFileList;
      }
      set
      {
        _logoFileList = value;
      }
    }
    #endregion

    public Logos()
    {
      // BackgroundWorker bgLogos = new System.ComponentModel.BackgroundWorker();
      // string activeLogoConfigFile = Config.GetFile(Config.Dir.Config, "MyFilmsLogos_" + Configuration.CurrentConfig + ".xml"); // Default config customized logofile
      //string activeLogoConfig = "MyFilmsLogos_" + Configuration.CurrentConfig;
      string logoConfigPathSkin;
      string skinLogoPath;
      string activeLogoConfigFile = String.Empty;

      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
      {
        skinLogoPath = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "DefaultWide") + @"\Media\Logos"; // Get current path to logos in skindirectory
        logoConfigPathSkin = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "DefaultWide"); // Get current path to active skin directory
      }

      if (File.Exists(logoConfigPathSkin + @"\MyFilmsLogos.xml"))
      {
        try
        {
          activeLogoConfigFile = logoConfigPathSkin + @"\MyFilmsLogos.xml";
          LogoConfigOverride = "O";
          LogMyFilms.Debug("Using Skin specific logo config file: '" + activeLogoConfigFile + "'");
          //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + Configuration.CurrentConfig;
        }
        catch
        {
          LogMyFilms.Debug("Error copying config specific file from skin override file !");
        }
      }
      else
      {
        activeLogoConfigFile = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsLogos.xml";
        LogoConfigOverride = "";
        LogMyFilms.Debug("Using MyFilms default logo config file: '" + activeLogoConfigFile + "'");
      }

      using (var xmlConfig = new XmlSettings(activeLogoConfigFile))
      {
        // First check, if Config specific LogoConfig exists, if not create it from default file!
        LogosPath = xmlConfig.ReadXmlConfig(activeLogoConfigFile, "ID0000", "LogosPath", "");
        //Recreate the path to make it OS independant...
        if (LogosPath.Length < 1) // Fall back to default skin logos !
        {
          LogosPath = skinLogoPath;
        }
        else
        {
          if (LogosPath.ToLower().Contains(@"Team Mediaportal\Mediaportal".ToLower()))
          {
            int pos = LogosPath.ToLower().LastIndexOf(@"Team Mediaportal\Mediaportal".ToLower());
            LogosPath = LogosPath.Substring(pos + @"Team Mediaportal\Mediaportal".Length);
            LogosPath = Config.GetDirectoryInfo(Config.Dir.Config) + LogosPath;
          }
        }
        if (LogosPath.LastIndexOf("\\", System.StringComparison.Ordinal) != LogosPath.Length - 1)
          LogosPath = LogosPath + "\\";
        Spacer = xmlConfig.ReadXmlConfig(activeLogoConfigFile, "ID0000", "Spacing", 1);
        Country = xmlConfig.ReadXmlConfig(activeLogoConfigFile, "ID0000", "Country", string.Empty);

        // use Country setting of Logos if available
        if (Country.Length > 0)
        {
          UseCountryLogos = true;
          //if (Directory.Exists(LogosPath + Country) || LogoFileList.Any(x => x.IndexOf("\\" + Country + "\\", StringComparison.OrdinalIgnoreCase) >= 0))
          //{
          //}
        }
        else
        {
          // fallback to use MP language, if directory exists and no specific country setting in logo manager
          if (Directory.Exists(LogosPath + MyFilmsSettings.MPLanguage) || LogoFileList.Any(x => x.IndexOf("\\" + MyFilmsSettings.MPLanguage + "\\", StringComparison.OrdinalIgnoreCase) >= 0))
          {
            Country = MyFilmsSettings.MPLanguage;
            UseCountryLogos = true;
          }
        }

        if (UseCountryLogos) MyFilmsDetail.setGUIProperty("config.country", Country, true);

        LogMyFilms.Debug("Logo path for reading logos        : '" + LogosPath + "'");
        LogMyFilms.Debug("Logo path for storing cached logos : '" + LogosPathThumbs + "' with spacing = '" + Spacer + "'");
        LogMyFilms.Debug("Logo Country                       : '" + Country + "', MP language = '" + MyFilmsSettings.MPLanguage + "', UseCountryLogos = '" + UseCountryLogos + "'");

        #region read logo rules
        int i = 0;
        ID2001Logos.Clear();
        ID2002Logos.Clear();
        ID2003Logos.Clear();
        ID2012Logos.Clear();
        do
        {
          string wline = xmlConfig.ReadXmlConfig(activeLogoConfigFile, "ID2001", "ID2001_" + i, null);
          if (wline == null)
            break;
          ID2001Logos.Add(wline);
          i++;
        } while (true);

        i = 0;
        do
        {
          string wline = xmlConfig.ReadXmlConfig(activeLogoConfigFile, "ID2002", "ID2002_" + i, null);
          if (wline == null)
            break;
          ID2002Logos.Add(wline);
          i++;
        } while (true);
        i = 0;
        do
        {
          string wline = xmlConfig.ReadXmlConfig(activeLogoConfigFile, "ID2003", "ID2003_" + i, null);
          if (wline == null)
            break;
          ID2003Logos.Add(wline);
          i++;
        } while (true);
        #endregion
      }
    }

    public static string BuildLogos(DataRow r, string logoType, int imgHeight, int imgWidth, int xPos, int yPos, int id)
    {
      var imgs = new List<Image>();
      var imgSizes = new List<Size>();
      var listelogos = new List<string>();
      string fileLogoName = string.Empty;
      switch (logoType)
      {
        case "ID2001":
          fileLogoName = GetLogos(r, ref listelogos, ID2001Logos);
          break;
        case "ID2002":
          fileLogoName = GetLogos(r, ref listelogos, ID2002Logos);
          break;
        case "ID2003":
          fileLogoName = GetLogos(r, ref listelogos, ID2003Logos);
          break;
        case "ID2012":
          fileLogoName = GetLogos(r, ref listelogos, ID2001Logos);
          fileLogoName = fileLogoName + GetLogos(r, ref listelogos, ID2002Logos);
          break;
      }
      if (listelogos.Count > 0)
      {
        LogMyFilms.Debug("Logo picture to be added " + fileLogoName);
        string skinName = GUIGraphicsContext.Skin.Substring(GUIGraphicsContext.Skin.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        fileLogoName = id == MyFilms.ID_MyFilms
                         ? "MyFilms_" + skinName + "_M" + LogoConfigOverride + fileLogoName + ".png"
                         : "MyFilms_" + skinName + "_D" + LogoConfigOverride + fileLogoName + ".png";
        int wHeight = 0;
        int wWidth = 0;
        if (!Directory.Exists(LogosPathThumbs))
        {
          try { Directory.CreateDirectory(LogosPathThumbs); }
          catch { }
        }

        foreach (string listelogo in listelogos)
        {
          LogMyFilms.Debug("Logo picture content: '" + listelogo + "'");
        }
        try
        {
          // check if image exists and get its dimensions
          Image single = ImageFast.FastFromFile(LogosPathThumbs + fileLogoName);
          wHeight = single.Height;
          wWidth = single.Width;
          single.Dispose();
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("BuildLogos error: " + ex.Message);
        }

        if (!File.Exists(LogosPathThumbs + fileLogoName) || wHeight != imgHeight || wWidth != imgWidth)
        {
          Bitmap b = new Bitmap(imgWidth, imgHeight);
          Image img = b;
          Graphics g = Graphics.FromImage(img);
          AppendLogos(listelogos, ref g, imgHeight, imgWidth, Spacer);
          try
          {
            b.Save(LogosPathThumbs + fileLogoName);
            LogMyFilms.Debug("Concatenated Logo saved " + fileLogoName);
            System.Threading.Thread.Sleep(10);
          }
          catch (Exception e)
          {
            LogMyFilms.Error("Unable to save Logo file ! error : " + e.Message);
          }
        }
        return LogosPathThumbs + fileLogoName;
      }
      else
        return string.Empty;
    }

    static string GetLogos(DataRow r, ref List<string> listelogos, ArrayList rulesLogos)
    {
      string fileLogoName = string.Empty;
      // _logoFileList = null; // to reread logo files from disk in case users changed them

      foreach (string[] wtab in from string wline in rulesLogos select wline.Split(new Char[] { ';' }))
      {
        // reset the default logo for this rule
        string defaultlogo = null;
        string foundlogo = null;

        // value should be given to output directly - property like
        if (wtab[1] == "value")
        {
          #region search default logo and set search value to DB value
          defaultlogo = SearchLogoPath(wtab[7]); // return null, if nothing found

          string originalValue = r[wtab[0]].ToString();
          string cleanervalue = wtab[2].Replace("#REGEX#", "").Replace("#regex#", "");
          string cleanedValue = Regex.Replace(originalValue, cleanervalue, "");
          string defaultlogosetting = wtab[7];
          LogMyFilms.Debug("GetLogos() - raw value before cleaning : '" + originalValue + "', cleaner expression = '" + cleanervalue + "', DefaultLogo setting = '" + (defaultlogosetting ?? "") + "'");
          LogMyFilms.Debug("GetLogos() - value after regex cleaning: '" + cleanedValue + "', DefaultLogo found = '" + (defaultlogo ?? "<null>") + "'");
          wtab[7] = cleanedValue.Length > 0 ? cleanedValue + ".png" : "emptyvalue.png";
          #endregion
        }

        // LogMyFilms.Debug("searching for logo: '" + wtab[7] + "'");
        foundlogo = SearchLogoPath(wtab[7]);
        if (foundlogo != null)
        {
          wtab[7] = foundlogo;
          // LogMyFilms.Debug("GetLogos() - Logo found, target = '" + wtab[7] + "'");
        }
        else if (defaultlogo != null)
        {
          LogMyFilms.Debug("GetLogos() - Missing logo for '" + wtab[7] + "' - using default logo instead: '" + defaultlogo + "'");
          wtab[7] = defaultlogo;
        }
        else
        {
          LogMyFilms.Debug("GetLogos() - Logo NOT found for '" + (wtab[7]??"") + "'");
        }

        if (File.Exists(wtab[7]))
        {
          #region build combined logo based on the rules
          bool cond1 = GetRecordRule(r, wtab[0], wtab[1], wtab[2]);
          bool cond2 = GetRecordRule(r, wtab[4], wtab[5], wtab[6]);
          if (wtab[3].Length == 0 && cond1)
          {
            if (!listelogos.Contains(wtab[7]))
            {
              listelogos.Add(wtab[7]);
              fileLogoName = fileLogoName + "_" + Path.GetFileNameWithoutExtension(wtab[7]);
            }
            continue;
          }
          if (wtab[3] == "or" && (cond1 || cond2))
          {
            if (!listelogos.Contains(wtab[7]))
            {
              listelogos.Add(wtab[7]);
              fileLogoName = fileLogoName + "_" + Path.GetFileNameWithoutExtension(wtab[7]);
            }
            continue;
          }
          if (wtab[3] == "and" && (cond1 && cond2))
          {
            if (!listelogos.Contains(wtab[7]))
            {
              listelogos.Add(wtab[7]);
              fileLogoName = fileLogoName + "_" + Path.GetFileNameWithoutExtension(wtab[7]);
            }
          }
          #endregion
        }
        else
        {
          LogMyFilms.Debug("Missing logo for '" + wtab[7] + "'");
        }
      }
      return fileLogoName;
    }

    static string SearchLogoPath(string logo)
    {
      string logopath = null;
      #region First check, if there is country specific logos, if it is enabled
      if (UseCountryLogos)
      {
        logopath = LogoFileList.FirstOrDefault(logoFile => logoFile.IndexOf("\\" + Country + "\\", StringComparison.OrdinalIgnoreCase) >= 0 && logoFile.EndsWith("\\" + logo, StringComparison.OrdinalIgnoreCase));

        //// if no logo found in the configured country context, try to find in any other country context (pattern matching "\??\") - however, if you're missing a logo, you might get a wrong one from other directory !
        //if (logopath == null)
        //{
        //  logopath = LogoFileList.FirstOrDefault(logoFile => logoFile.IndexOf("\\us\\", StringComparison.OrdinalIgnoreCase) >= 0 && logoFile.EndsWith("\\" + logo, StringComparison.OrdinalIgnoreCase)); // fallback to us language folder(s)
        //  if (logopath != null) LogMyFilms.Debug("GetLogos() - no logo found in '" + Country + "'-context - but logo found in other language-context, using: '" + logopath + "'");
        //}
      }
      #endregion

      #region use standard search if either country logos disabled or no logos found in country subfolder(s)
      if (logopath == null)
      {
        if (File.Exists(logo))
        {
          logopath = logo;
        }

        // Added to also support Logo Mediafiles without path names - makes it independant from Skin also ...
        if (logopath == null && File.Exists(LogosPath + logo)) // Check, if logofile is present in logo directory of current skin
        {
          logopath = LogosPath + logo;
        }

        if (logopath == null)
        {
          if (UseCountryLogos)
          {
            // search for any default logo match but not in country folders
            logopath = LogoFileList.FirstOrDefault(logoFile => !Regex.IsMatch(logoFile, @"\\{1}\D{2}\\{1}") && logoFile.EndsWith("\\" + logo, StringComparison.OrdinalIgnoreCase));
            // if (logopath != null) LogMyFilms.Debug("GetLogos() - no logo found in '" + Country + "'-context - but logo found in non language-context, using: '" + logopath + "'");
          }
          else
          {
            //// search for any logo match in any folder
            //logopath = LogoFileList.FirstOrDefault(logoFile => logoFile.EndsWith("\\" + logo, StringComparison.OrdinalIgnoreCase));

            // general fallback only to "non-language-folder-logos" - so never put generic logos in folders with 2 letters or they might not be found !
            logopath = LogoFileList.FirstOrDefault(logoFile => !Regex.IsMatch(logoFile, @"\\{1}\D{2}\\{1}") && logoFile.EndsWith("\\" + logo, StringComparison.OrdinalIgnoreCase));
          }
        }
      }
      #endregion
      
      // finally logopath is null, if none found, otherwise the value for the logo found
      return logopath;
    }

    static bool GetRecordRule(DataRow r, string field, string compar, string value)
    {
      switch (compar)
      {
        case "value":
          return true;
        case "regex":
          Match match = Regex.Match(r[field].ToString(), value, RegexOptions.IgnoreCase);
          return match.Success;
        case "equal":
          return r[field].ToString().ToLower() == value.ToLower();
        case "not equal":
          return r[field].ToString().ToLower() != value.ToLower();
        case "contains":
          return r[field].ToString().ToLower().Contains(value.ToLower());
        case "not contains":
          return !(r[field].ToString().ToLower().Contains(value.ToLower()));
        case "greater":
          int number;
          int ivalue;
          bool isnumeric;
          bool isnumericref;
          switch (r[field].GetType().Name)
          {
            //case "Int32": // -> to be handled in "default" as string to int value ...
            //    if ((int)r[field].ToString().ToLower().CompareTo(value.ToLower()) > 0)
            //      return true;
            //    break;
            case "DateTime":
              break;
            default:
              isnumeric = int.TryParse(r[field].ToString(), out number);
              isnumericref = int.TryParse(value, out ivalue);
              if (isnumeric && isnumericref)
              {
                return number > ivalue;
              }
              else
              {
                // if (r[field].ToString().ToLower().CompareTo(value.ToLower()) > 0) return true;
                IComparer myComparer = new MyFilms.AlphanumComparatorFast();
                return myComparer.Compare(r[field].ToString().ToLower(), value.ToLower()) > 0;
              }
          }
          break;
        case "lower":
          switch (r[field].GetType().Name)
          {
            case "DateTime":
              break;
            default:
              isnumeric = int.TryParse(r[field].ToString(), out number);
              isnumericref = int.TryParse(value, out ivalue);
              if (isnumeric && isnumericref)
              {
                return number <= ivalue;
              }
              else
              {
                // if (!(r[field].ToString().ToLower().CompareTo(value.ToLower()) > 0)) return true;
                IComparer myComparer = new MyFilms.AlphanumComparatorFast();
                return myComparer.Compare(r[field].ToString().ToLower(), value.ToLower()) < 0;
              }
          }
          break;
        case "filled":
          return r[field].ToString().Length > 0;
        case "not filled":
          return r[field].ToString().Length == 0;
      }
      return false;
    }

    static void AppendLogos(List<string> listelogos, ref Graphics g, int totalHeight, int totalWidth, int spacer)
    {
      int noImgs = listelogos.Count;
      var imgs = new List<Image>();
      var imgSizes = new List<Size>();
      int checkWidth = 0, checkHeigth = 0;

      #region step one: get all sizes (not all logos are obviously square) and scale them to fit vertically
      // Create source
      float scale = 0;
      float totalWidthf = (float)totalWidth;
      float totalHeightf = (float)totalHeight;
      var tmp = default(Size);
      int xPos = 0, yPos = 0;
      foreach (string t in listelogos)
      {
        Image single = null;
        try
        {
          single = ImageFast.FastFromFile(t);
        }
        catch (Exception)
        {
          LogMyFilms.Error("Could not load Logo Image file... " + t);
          return;
        }
        if (totalWidth > totalHeight)
          scale = totalHeightf / (float)single.Size.Height;
        else
          scale = totalWidthf / (float)single.Size.Width;
        tmp = new Size((int)(single.Width * scale), (int)(single.Height * scale));
        checkWidth += tmp.Width;
        checkHeigth += tmp.Height;
        imgSizes.Add(tmp);
        imgs.Add(single);
      }
      #endregion

      #region step two: check if we are too big horizontally and if so scale again
      checkHeigth += imgSizes.Count * spacer;
      checkWidth += imgSizes.Count * spacer;
      if (totalWidth > totalHeight)
      {
        if (checkWidth > totalWidth)
        {
          scale = (float)checkWidth / (float)totalWidth;
          for (var i = 0; i < imgSizes.Count; i++)
          {
            imgSizes[i] = new Size((int)(imgSizes[i].Width / scale), (int)(imgSizes[i].Height / scale));
          }
        }
      }
      else
        if (checkHeigth > totalHeight)
        {
          scale = (float)checkHeigth / (float)totalHeight;
          for (var i = 0; i < imgSizes.Count; i++)
          {
            imgSizes[i] = new Size((int)(imgSizes[i].Width / scale), (int)(imgSizes[i].Height / scale));
          }
        }
      #endregion

      #region step three: finally draw them
      for (var i = 0; i < imgs.Count; i++)
      {
        if (totalWidth > totalHeight)
        {
          g.DrawImage(imgs[i], xPos, totalHeight - imgSizes[i].Height, imgSizes[i].Width, imgSizes[i].Height);
          xPos += imgSizes[i].Width + spacer;
        }
        else
        {
          g.DrawImage(imgs[i], totalWidth - imgSizes[i].Width, yPos, imgSizes[i].Width, imgSizes[i].Height);
          yPos += imgSizes[i].Height + spacer;
        }
      }
      #endregion
    }
  }
}
