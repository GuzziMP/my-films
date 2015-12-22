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

using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.IO;

namespace MPLanguageTool
{
  internal class ResxHandler
  {
    private static string BuildFileName(string languageId)
    {
      if (languageId == null)
      {
        return FrmMain.LanguagePath + "\\MediaPortal.DeployTool.resx";
      }
      return FrmMain.LanguagePath + "\\MediaPortal.DeployTool." + languageId + ".resx";
    }

    public static NameValueCollection Load(string languageId)
    {
      string xml = BuildFileName(languageId);
      if (!File.Exists(xml))
      {
        return languageId == null ? null : new NameValueCollection();
      }
      NameValueCollection translations = new NameValueCollection();
      XmlDocument doc = new XmlDocument();
      doc.Load(xml);
      XmlNodeList nodes = doc.SelectNodes("/root/data");
      if (nodes != null)
      {
        foreach (XmlNode keyNode in nodes)
          translations.Add(keyNode.Attributes["name"].Value, keyNode.SelectSingleNode("value").InnerText);
      }
      return translations;
    }

    public static void Save(string languageID, NameValueCollection translations)
    {
      string stub = Resource1.ResourceManager.GetString("ResxTemplate");
      string xml = BuildFileName(languageID);
      StreamWriter writer = new StreamWriter(xml, false, Encoding.UTF8);
      writer.Write(stub);
      writer.Close();
      XmlDocument doc = new XmlDocument();
      doc.Load(xml);
      XmlNode nRoot = doc.SelectSingleNode("/root");
      foreach (string key in translations.Keys)
      {
        if (translations[key] == null) continue;
        XmlNode nValue = doc.CreateElement("value");

        nValue.InnerText = translations[key];
        XmlNode nKey = doc.CreateElement("data");
        XmlAttribute attr = nKey.OwnerDocument.CreateAttribute("name");
        attr.InnerText = key;
        nKey.Attributes.Append(attr);
        attr = nKey.OwnerDocument.CreateAttribute("xml:space");
        attr.InnerText = "preserve";
        nKey.Attributes.Append(attr);
        nKey.AppendChild(nValue);
        nRoot.AppendChild(nKey);
      }
      doc.Save(xml);
    }
  }
}