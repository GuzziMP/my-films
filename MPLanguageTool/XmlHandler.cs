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
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;

namespace MPLanguageTool
{
  using System.Windows.Forms;

  internal class XmlHandler
  {
    private static string MainNodeSelection = string.Empty;
    private static string SingleNodeSelection = string.Empty;
    private static string Field = string.Empty;
    private static string Prefix = string.Empty;

    private static string TextAttribute = string.Empty;
    // if the Text is not in Innertext, but in an attribute (MPII does it that way)

    public static string BuildFileName(string languageId, bool returnFullPath)
    {
      string langDefaultId = "en";
      string langPrefix = string.Empty;
      switch (FrmMain.LangType)
      {
        case FrmMain.StringsType.MovingPictures:
        case FrmMain.StringsType.TvSeries:
          langDefaultId = "en-US";
          break;
        case FrmMain.StringsType.MediaPortal_1:
        case FrmMain.StringsType.MediaPortal_II:
        case FrmMain.StringsType.MpTagThat:
        case FrmMain.StringsType.MyFilms:
          langPrefix = "strings_";
          break;
      }
      if (languageId != null)
      {
        langDefaultId = languageId;
      }
      return returnFullPath
               ? FrmMain.LanguagePath + "\\" + langPrefix + langDefaultId + ".xml"
               : langPrefix + langDefaultId + ".xml";
    }

    public static void InitializeXmlValues()
    {
      switch (FrmMain.LangType)
      {
        case FrmMain.StringsType.MediaPortal_1:
        case FrmMain.StringsType.MyFilms:
          MainNodeSelection = "/Language/section";
          SingleNodeSelection = "String";
          Field = "id";
          Prefix = "prefix";
          break;
        case FrmMain.StringsType.MovingPictures:
          MainNodeSelection = "/strings";
          SingleNodeSelection = "string";
          Field = "Field";
          break;
        case FrmMain.StringsType.TvSeries:
          MainNodeSelection = "/strings";
          SingleNodeSelection = "string";
          Field = "Field";
          Prefix = "Original";
          break;
      }
    }

    public static void InitializeXmlValues(string section)
    {
      switch (FrmMain.LangType)
      {
        case FrmMain.StringsType.MediaPortal_II:
          MainNodeSelection = "/Language/section[@Name='" + section + "']";
          SingleNodeSelection = "String";
          Field = "Name";
          TextAttribute = "Text";
          break;

        case FrmMain.StringsType.MpTagThat:
          MainNodeSelection = "/Language/section[@name='" + section + "']";
          SingleNodeSelection = "String";
          Field = "id";
          break;
      }
    }

    // Load Original Label to Translate
    public static DataTable Load(string languageId, out Dictionary<string, DataRow> originalMapping)
    {
      originalMapping = new Dictionary<string, DataRow>();
      string xml = BuildFileName(languageId, true);
      if (!File.Exists(xml))
      {
        return languageId == null ? null : new DataTable();
      }

      DataTable translations = new DataTable();

      DataColumn col0 = new DataColumn("id", Type.GetType("System.String"));
      DataColumn col1 = new DataColumn("Original", Type.GetType("System.String"));
      DataColumn col2 = new DataColumn("Translated", Type.GetType("System.String"));
      DataColumn col3 = new DataColumn("PrefixOriginal", Type.GetType("System.String"));
      DataColumn col4 = new DataColumn("PrefixTranslated", Type.GetType("System.String"));

      translations.Columns.Add(col0);
      translations.Columns.Add(col1);
      translations.Columns.Add(col2);
      translations.Columns.Add(col3);
      translations.Columns.Add(col4);


      XmlDocument doc = new XmlDocument();
      doc.Load(xml);
      if (doc.DocumentElement != null)
      {
        XmlNodeList nodes = doc.DocumentElement.SelectNodes(MainNodeSelection + "//" + SingleNodeSelection);

        if (nodes != null)
        {
          foreach (XmlNode keyNode in nodes)
          {
            string prefixValue = "";
            string nodeId = keyNode.Attributes[Field].Value;

            // MPII has always 2 attributes
            if (keyNode.Attributes.Count == 2 && FrmMain.LangType != FrmMain.StringsType.MediaPortal_II)
            {
              prefixValue = keyNode.Attributes[Prefix].Value;
            }

            DataRow row = translations.NewRow();
            row[0] = nodeId;
            if (FrmMain.LangType == FrmMain.StringsType.MediaPortal_II)
            {
              row[1] = keyNode.Attributes[TextAttribute].Value;
            }
            else
            {
              row[1] = keyNode.InnerText;
            }
            row[2] = "";
            row[3] = prefixValue;
            row[4] = "";


            translations.Rows.Add(row);
            originalMapping.Add(nodeId, row);
          }
        }
      }
      return translations;
    }

    // Load Translations
    public static DataTable Load_Translation(string languageId, DataTable originalTranslation, Dictionary<string, DataRow> originalMapping)
    {
      string xml = BuildFileName(languageId, true);
      DataTable translations = new DataTable();

      DataColumn col0 = new DataColumn("id", Type.GetType("System.String"));
      DataColumn col1 = new DataColumn("Original", Type.GetType("System.String"));
      DataColumn col2 = new DataColumn("Translated", Type.GetType("System.String"));
      DataColumn col3 = new DataColumn("PrefixOriginal", Type.GetType("System.String"));
      DataColumn col4 = new DataColumn("PrefixTranslated", Type.GetType("System.String"));

      translations.Columns.Add(col0);
      translations.Columns.Add(col1);
      translations.Columns.Add(col2);
      translations.Columns.Add(col3);
      translations.Columns.Add(col4);

      if (!File.Exists(xml))
      {
        if (languageId == null)
        {
          return null;
        }
        //
        // Create a empty xml file as placeholder
        //
        Save(languageId, "", translations);
      }

      Dictionary<string, DataRow> translationMapping = new Dictionary<string, DataRow>();
      XmlDocument doc = new XmlDocument();
      doc.Load(xml);
      if (doc.DocumentElement != null)
      {
        XmlNodeList nodes = doc.DocumentElement.SelectNodes(MainNodeSelection + "//" + SingleNodeSelection);

        if (nodes != null)
        {
          foreach (XmlNode keyNode in nodes)
          {
            string prefixValue = "";
            string nodeId = keyNode.Attributes[Field].Value;

            // MPII has always 2 attributes
            if (keyNode.Attributes.Count == 2 && FrmMain.LangType != FrmMain.StringsType.MediaPortal_II)
            {
              prefixValue = keyNode.Attributes[Prefix].Value;
            }

            DataRow row = translations.NewRow();
            row[0] = nodeId;
            if (FrmMain.LangType == FrmMain.StringsType.MediaPortal_II)
            {
              row[1] = keyNode.Attributes[TextAttribute].Value;
            }
            else
            {
              row[1] = keyNode.InnerText;
            }
            row[2] = "";
            row[3] = prefixValue;
            row[4] = "";

            //
            // If strings_XX.xml trow an expcetion, uncomment this line to find the offending line
            //
            //MessageBox.Show("Loading xml strings (node_id = " + node_id + ")");
            //
            translations.Rows.Add(row);
            if (translationMapping.ContainsKey(nodeId.Trim()))
            {
              MessageBox.Show("Error in translation file !\n ID = '" + nodeId + "', String = '" + row[1] + "'", "Language File Importer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            translationMapping.Add(nodeId.Trim(), row);
          }
        }
      }


      // Hope That indexes was syncronized
      foreach (String key in originalMapping.Keys)
      {
        if (originalMapping.ContainsKey(key) && translationMapping.ContainsKey(key))
        {
          originalMapping[key]["PrefixTranslated"] = translationMapping[key]["PrefixOriginal"].ToString();
          originalMapping[key]["Translated"] = translationMapping[key]["Original"].ToString();
        }
      }

      return originalTranslation;
    }

    // Get list of sections
    public static IEnumerable<string> ListSections(string strSection, string attribute)
    {
      string xml = BuildFileName(null, true);
      XmlDocument doc = new XmlDocument();
      doc.Load(xml);

      List<string> sections = new List<string>();

      if (doc.DocumentElement != null)
      {
        XmlNodeList nodes = doc.DocumentElement.SelectNodes(strSection);

        if (nodes != null)
        {
          sections.AddRange(from XmlNode keyNode in nodes where keyNode.Attributes.Count > 0 select keyNode.Attributes[attribute].Value);
        }
      }
      return sections;
    }

    /// <summary>
    /// Validates, if we have a valid XML Document
    /// </summary>
    /// <param name="languageId"></param>
    /// <returns></returns>
    private static bool IsValidDocument(string languageId)
    {
      string xml = BuildFileName(languageId, true);
      if (!File.Exists(xml))
      {
        return false;
      }

      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(xml);
        XmlElement root = doc.DocumentElement;
        return root != null;
      }
      catch (Exception)
      {
        return false;
      }
    }

    // Save file
    public static void Save(string languageId, string languageName, DataTable translations)
    {
      string xml = BuildFileName(languageId, true);
      // Don't init the Streamwriter here, as it will clear the file content.
      // When saving multiple sections language files like MP2 or MPTagThat this causes troubles
      StreamWriter writer;
      switch (FrmMain.LangType)
      {
        case FrmMain.StringsType.MediaPortal_1:
        case FrmMain.StringsType.MyFilms:
          writer = new StreamWriter(xml, false, Encoding.UTF8);
          writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
          writer.Write("<Language name=\"" + languageName + "\" characters=\"" + GetMaxCharsForLanguageId(languageId) +
                       "\">\n");
          writer.Write("  <section name=\"unmapped\">\n");
          writer.Write("  </section>\n");
          writer.Write("</Language>\n");
          writer.Close();
          break;
        case FrmMain.StringsType.MediaPortal_II:
          if (!IsValidDocument(languageId))
          {
            writer = new StreamWriter(xml, false, Encoding.UTF8);
            writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            writer.Write("<!-- MediaPortal II translation file -->\n");
            writer.Write("<!-- Note: English is the fallback for any strings not found in other languages -->\n");
            writer.Write("<Language>\n");
            writer.Write("</Language>\n");
            writer.Close();
          }
          break;
        case FrmMain.StringsType.MpTagThat:
          if (!IsValidDocument(languageId))
          {
            writer = new StreamWriter(xml, false, Encoding.UTF8);
            writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            // Write some placeholder comments to have the same amount of lines as in the english referebce file
            writer.Write("<!--\n");
            writer.Write("     MPTagThat translation file\n\n");
            writer.Write("     Note: English is the fallback for any strings not found in other languages\n\n");
            writer.Write("-->\n");
            writer.Write("<Language>\n");
            writer.Write("</Language>\n");
            writer.Close();
          }
          break;
        case FrmMain.StringsType.MovingPictures:
          writer = new StreamWriter(xml, false, Encoding.UTF8);
          writer.Write("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n");
          writer.Write("<!-- Moving Pictures translation file -->\n");
          writer.Write("<!-- " + languageName + " -->\n");
          writer.Write("<!-- Note: English is the fallback for any strings not found in other languages -->\n");
          writer.Write("  <strings>\n");
          writer.Write("  </strings>\n");
          writer.Close();
          break;
        case FrmMain.StringsType.TvSeries:
          writer = new StreamWriter(xml, false, Encoding.UTF8);
          writer.Write("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n");
          writer.Write("<!-- MP-TV-Series translation file -->\n");
          writer.Write("<!-- " + languageName + " -->\n");
          writer.Write("<!-- Note: English is the fallback for any strings not found in other languages -->\n");
          writer.Write("  <strings>\n");
          writer.Write("  </strings>\n");
          writer.Close();
          break;
      }

      XmlDocument doc = new XmlDocument();
      doc.Load(xml);
      XmlNode nRoot = doc.SelectSingleNode(MainNodeSelection);
      if (nRoot == null)
      {
        // we have a new node, which got never translated
        // so let's add it
        string attrName = string.Empty;
        switch (FrmMain.LangType)
        {
          case FrmMain.StringsType.MediaPortal_II:
            attrName = "Name";
            break;

          case FrmMain.StringsType.MpTagThat:
            attrName = "name";
            break;
        }

        int startIndex = MainNodeSelection.IndexOf("'") + 1;
        int endIndex = MainNodeSelection.LastIndexOf("'");
        string sectionName = MainNodeSelection.Substring(startIndex, endIndex - startIndex);
        nRoot = doc.CreateElement("section");
        XmlAttribute attr = nRoot.OwnerDocument.CreateAttribute(attrName);
        attr.InnerText = sectionName;
        nRoot.Attributes.Append(attr);
        doc.DocumentElement.AppendChild(nRoot);
      }

      // Clear the Innertext first, so that we sdon't have multiple entries
      nRoot.InnerText = "";

      foreach (DataRow row in translations.Rows)
      {
        XmlNode nValue = doc.CreateElement(SingleNodeSelection);

        // First place id, must be same key as original
        XmlAttribute attr = nValue.OwnerDocument.CreateAttribute(Field);
        attr.InnerText = row["id"].ToString();
        nValue.Attributes.Append(attr);

        if (!String.IsNullOrEmpty(row["PrefixTranslated"].ToString()))
        {
          attr = nValue.OwnerDocument.CreateAttribute(Prefix);
          attr.InnerText = row["PrefixTranslated"].ToString();
        }

        if (!String.IsNullOrEmpty(row["Translated"].ToString()))
        {
          // MPII does have the translation in an attribute and not inner text
          if (FrmMain.LangType == FrmMain.StringsType.MediaPortal_II)
          {
            attr = nValue.OwnerDocument.CreateAttribute(TextAttribute);
            attr.InnerText = row["Translated"].ToString();
          }
          else
          {
            nValue.InnerText = row["Translated"].ToString();
          }
          nValue.Attributes.Append(attr);
          nRoot.AppendChild(nValue);
        }
      }
      doc.Save(xml);
    }

    private static int GetMaxCharsForLanguageId(string languageId)
    {
      int maxChars;

      switch (languageId)
      {
        case "cs": //Czech
        case "hu": //Hungarian
        case "sl": //Slovenian
          maxChars = 512;
          break;
        case "el": //Greek
          maxChars = 974;
          break;
        case "he": //Hebrew
          maxChars = 1524;
          break;
        case "pl": //Polish
          maxChars = 380;
          break;
        case "ru": //Russian
          maxChars = 1105;
          break;
        case "uk": //Ukrainian
          maxChars = 1250;
          break;
        default:
          maxChars = 255;
          break;
      }

      return maxChars;
    }
  }
}