#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//using MediaPortal.GUI.Library;

namespace MyFilmsPlugin.MyFilms.Utils
{
  using System.Globalization;

  using MediaPortal.Profile;

  ///// <summary>
  ///// MPSettings allows to read and write MediaPortal.xml configuration file
  ///// (wrapper class to unify path handling)
  ///// </summary>
  //public class MPSettings : Settings
  //{
  //  private static string _configPathName;

  //  public static string ConfigPathName
  //  {
  //    get
  //    {
  //      if (string.IsNullOrEmpty(_configPathName))
  //      {
  //        _configPathName = Configuration.Config.GetFile(Configuration.Config.Dir.Config, "MediaPortal.xml");
  //      }
  //      return _configPathName;
  //    }
  //    set
  //    {
  //      if (string.IsNullOrEmpty(_configPathName))
  //      {
  //        _configPathName = value;
  //        if (!Path.IsPathRooted(_configPathName))
  //        {
  //          _configPathName = Configuration.Config.GetFile(Configuration.Config.Dir.Config, _configPathName);
  //        }
  //      }
  //      else
  //      {
  //        throw new InvalidOperationException("ConfigPathName already has a value.");
  //      }
  //    }
  //  }

  //  private static MPSettings _instance;

  //  public static MPSettings Instance
  //  {
  //    get { return _instance ?? (_instance = new MPSettings()); }
  //  }

  //  // public constructor should be made/private protected, we should encourage the usage of Instance

  //  public MPSettings()
  //    : base(ConfigPathName) {}
  //}

  /// <summary>
  /// Settings allows to read and write any xml configuration file
  /// </summary>
  public class XmlSettings : IDisposable
  {
    public XmlSettings(string fileName, bool isCached = true)
    {
      xmlFileName = Path.GetFileName(fileName).ToLowerInvariant();

      _isCached = isCached;

      if (_isCached)
        xmlCache.TryGetValue(xmlFileName, out xmlDoc);

      if (xmlDoc == null)
      {
        xmlDoc = new CacheSettingsProvider(new XmlSettingsProvider(fileName));

        if (_isCached)
          xmlCache.Add(xmlFileName, xmlDoc);
      }
    }

    //// Recover install MediaPortal path
    //public string PathInstalMP()
    //{
    //  string path = Config.GetFolder(Config.Dir.Config);
    //  return path;
    //}

    //// Build entire filename of config file
    //public string EntireFilenameConfig(string FileName)
    //{
    //  if (FileName.Contains(":\\"))
    //    return FileName;
    //  string entirefilename = PathInstalMP() + @"\" + FileName + ".xml";
    //  return entirefilename;
    //}


    public string GetValue(string section, string entry)
    {
      object value = xmlDoc.GetValue(section, entry);
      return value == null ? string.Empty : value.ToString();
    }

    private T GetValueOrDefault<T>(string section, string entry, Func<string, T> conv, T defaultValue)
    {
      string strValue = GetValue(section, entry);
      return string.IsNullOrEmpty(strValue) ? defaultValue : conv(strValue);
    }

    //ReadXmlConfig

    public string GetValueAsString(string section, string entry, string strDefault)
    {
      return GetValueOrDefault(section, entry, val => val, strDefault);
    }

    public bool GetValueAsBool(string section, string entry, bool bDefault)
    {
      return GetValueOrDefault(section, entry,
                               val => val.Equals("yes", StringComparison.InvariantCultureIgnoreCase),
                               bDefault);
    }

    public int GetValueAsInt(string section, string entry, int iDefault)
    {
      return GetValueOrDefault(section, entry,
                               val =>
                               {
                                 int iVal;
                                 return Int32.TryParse(val, out iVal) ? iVal : iDefault;
                               }, iDefault);
    }

    public float GetValueAsFloat(string section, string entry, float fDefault)
    {
      object obj = xmlDoc.GetValue(section, entry);
      if (obj == null) return fDefault;
      string strValue = obj.ToString();
      if (strValue == null) return fDefault;
      if (strValue.Length == 0) return fDefault;
      try
      {
        float test = 123.456f;
        string tmp = test.ToString();
        bool useCommas = (tmp.IndexOf(",") >= 0);
        strValue = useCommas == false ? strValue.Replace(',', '.') : strValue.Replace('.', ',');

        float fRet = (float)System.Double.Parse(strValue, NumberFormatInfo.InvariantInfo);
        return fRet;
      }
      catch (Exception)
      {
      }
      return fDefault;
    }

    public string ReadXmlConfig(string FileName, string section, string entry, string strDefault)
    {
      return GetValueOrDefault(section, entry, val => val, strDefault);
    }

    public bool ReadXmlConfig(string FileName, string section, string entry, bool bDefault)
    {
      return GetValueOrDefault(section, entry,
                               val => val.Equals("yes", StringComparison.InvariantCultureIgnoreCase),
                               bDefault);
    }

    public int ReadXmlConfig(string FileName, string section, string entry, int iDefault)
    {
      return GetValueOrDefault(section, entry,
                               val =>
                               {
                                 int iVal;
                                 return Int32.TryParse(val, out iVal) ? iVal : iDefault;
                               }, iDefault);
    }

    public void WriteXmlConfig(string FileName, string section, string entry, object objValue)
    {
      SetValue(section, entry, objValue);
    }

    public void SetValue(string section, string entry, object objValue)
    {
      xmlDoc.SetValue(section, entry, objValue);
    }

    public void WriteXmlConfig(string FileName, string section, string entry, bool bValue)
    {
      SetValueAsBool(section, entry, bValue);
    }

    public void SetValueAsBool(string section, string entry, bool bValue)
    {
      SetValue(section, entry, bValue ? "yes" : "no");
    }

    public void RemoveEntry(string FileName, string section, string entry)
    {
      xmlDoc.RemoveEntry(section, entry);
    }

    public void RemoveEntry(string section, string entry)
    {
      xmlDoc.RemoveEntry(section, entry);
    }

    public static void ClearCache()
    {
      xmlCache.Clear();
    }

    //public void Save()
    //{
    //  xmlDoc.Save();
    //}

    #region IDisposable Members

    public void Dispose()
    {
      if (!_isCached)
      {
        xmlDoc.Save();
      }
    }

    public void Clear() { }

    public static void SaveCache()
    {
      foreach (var doc in xmlCache)
      {
        doc.Value.Save();
      }
    }

    #endregion

    #region Fields

    private bool _isCached;
    private static Dictionary<string, ISettingsProvider> xmlCache = new Dictionary<string, ISettingsProvider>();
    private string xmlFileName;
    private ISettingsProvider xmlDoc;

    #endregion Fields
  }
}