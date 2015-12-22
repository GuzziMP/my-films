#region Copyright (C) 2005-2009 Team MediaPortal

/* 
 *	Copyright (C) 2005-2009 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Localisation;

namespace MyFilmsPlugin.Utils
{
  /// <summary>
  /// This class will hold all text used in the application
  /// The text is loaded for the current language from
  /// the file language/[language]/strings.xml
  /// </summary>
  public class GUILocalizeStrings
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #region Variables
    static LocalisationProvider _stringProvider;
    static Dictionary<string, string> _cultures;
    static string[] _languages;
    #endregion

    #region Constructors/Destructors
    // singleton. Dont allow any instance of this class
    private GUILocalizeStrings()
    {
    }

    static public void Dispose()
    {
      if (_stringProvider != null)
        _stringProvider.Dispose();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Public method to load the text from a strings/xml file into memory
    /// </summary>
    /// <param name="language">language for which the file should be loaded</param>
    /// <returns>
    /// true when text is loaded
    /// false when it was unable to load the text
    /// </returns>
    //[Obsolete("This method has changed", true)]
    static public bool Load(string language)
    {
      bool isPrefixEnabled = true;

      using (var reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
      {
        // isPrefixEnabled = reader.GetValueAsBool("general", "myprefix", true); // setting moved in MP
        isPrefixEnabled = reader.GetValueAsBool("gui", "myprefix", true);
        if (language == null) language = reader.GetValueAsString("gui", "language", null); // try to get MP language from config
      }

      string directory = Config.GetSubFolder(Config.Dir.Language, "MyFilms");
      string cultureName = null;
      if (language != null) cultureName = GetCultureName(language);

      LogMyFilms.Info("Loading localised Strings - Culture: '{0}', Language: '{1}', Prefix: '{2}', Path: '{3}'", cultureName, language, isPrefixEnabled, directory);

      _stringProvider = new LocalisationProvider(directory, cultureName, isPrefixEnabled);

      GUIGraphicsContext.CharsInCharacterSet = _stringProvider.Characters;

      return true;
    }

    static public string CurrentLanguage()
    {
      //string Lang;
      //try
      //{ Lang = GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage()); }
      //catch (Exception)
      //{ Lang = CultureInfo.CurrentUICulture.Name; }
      //LogMyFilms.Info("Using language " + Lang);

      if (_stringProvider == null)
        Load(null);

      return _stringProvider.CurrentLanguage.EnglishName;
    }

    static public void ChangeLanguage(string language)
    {
      if (_stringProvider == null)
        Load(language);
      else
        _stringProvider.ChangeLanguage(GetCultureName(language));
    }

    /// <summary>
    /// Get the translation for a given id and format the sting with
    /// the given parameters
    /// </summary>
    /// <param name="dwCode">id of text</param>
    /// <param name="parameters">parameters used in the formating</param>
    /// <returns>
    /// string containing the translated text
    /// </returns>
    static public string Get(int dwCode, object[] parameters)
    {
      if (_stringProvider == null)
        Load(null);

      string translation = _stringProvider.GetString("unmapped", dwCode);
      // if parameters or the translation is null, return the translation.
      if ((translation == null) || (parameters == null))
      {
        return translation;
      }
      // return the formatted string. If formatting fails, log the error
      // and return the unformatted string.
      try
      {
        return String.Format(translation, parameters);
      }
      catch (FormatException e)
      {
        LogMyFilms.Error("Error formatting translation with id {0}", dwCode);
        LogMyFilms.Error("Unformatted translation: {0}", translation);
        LogMyFilms.Error(e);
        return translation;
      }
    }

    /// <summary>
    /// Get the translation for a given id
    /// </summary>
    /// <param name="dwCode">id of text</param>
    /// <returns>
    /// string containing the translated text
    /// </returns>
    static public string Get(int dwCode)
    {
      if (_stringProvider == null)
        Load(null);

      string translation = _stringProvider.GetString("unmapped", dwCode);

      if (translation == null)
      {
        LogMyFilms.Error("No translation found for id {0}", dwCode);
        return String.Empty;
      }

      return translation;
    }

    static public void LocalizeLabel(ref string strLabel)
    {
      if (_stringProvider == null)
        Load(null);

      if (strLabel == null) strLabel = String.Empty;
      if (strLabel == "-") strLabel = string.Empty;
      if (strLabel == string.Empty) return;
      // This can't be a valid string code if the first character isn't a number.
      // This check will save us from catching unnecessary exceptions.
      if (!char.IsNumber(strLabel, 0))
        return;

      int dwLabelID;

      try
      {
        dwLabelID = System.Int32.Parse(strLabel);
      }
      catch (FormatException e)
      {
        LogMyFilms.Error(e);
        strLabel = String.Empty;
        return;
      }

      strLabel = _stringProvider.GetString("unmapped", dwLabelID);
      if (strLabel == null)
      {
        LogMyFilms.Error("No translation found for id {0}", dwLabelID);
        strLabel = String.Empty;
      }
    }

    public static string LocalSupported()
    {
      if (_stringProvider == null)
        Load(null);

      CultureInfo culture = _stringProvider.GetBestLanguage();

      return culture.EnglishName;
    }

    public static string[] SupportedLanguages()
    {
      if (_languages == null)
      {
        if (_stringProvider == null)
          Load(null);

        CultureInfo[] cultures = _stringProvider.AvailableLanguages();

        SortedList sortedLanguages = new SortedList();
        foreach (CultureInfo culture in cultures)
          sortedLanguages.Add(culture.EnglishName, culture.EnglishName);

        _languages = new string[sortedLanguages.Count];

        for (int i = 0; i < sortedLanguages.Count; i++)
        {
          _languages[i] = (string)sortedLanguages.GetByIndex(i);
        }
      }

      return _languages;
    }

    internal static string GetCultureName(string language)
    {
      if (_cultures == null)
      {
        _cultures = new Dictionary<string, string>();

        CultureInfo[] cultureList = CultureInfo.GetCultures(CultureTypes.AllCultures);

        foreach (CultureInfo t in cultureList)
        {
          try
          {
            _cultures.Add(t.EnglishName, t.Name);
          }
          catch (Exception)
          {
            LogMyFilms.Warn("LocalizeStrings - duplicate culture found: " + t.EnglishName + " (" + t.Name + ")");
          }
        }
      }

      return _cultures.ContainsKey(language) ? _cultures[language] : null;
    }
    #endregion
  }
}