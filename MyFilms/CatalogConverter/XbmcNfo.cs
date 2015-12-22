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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace MyFilmsPlugin.CatalogConverter
{
  class XbmcNfo
  {
    public Dictionary<string, string> ProfilerDict;

    public XbmcNfo()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("MediaType", "MediaType");
      ProfilerDict.Add("MediaLabel", "MediaLabel");
      ProfilerDict.Add("OriginalTitle", "OriginalTitle");
      ProfilerDict.Add("TranslatedTitle", "TranslatedTitle");
      ProfilerDict.Add("FormattedTitle", "FormattedTitle");
      ProfilerDict.Add("Number", "Number");
      ProfilerDict.Add("Rating", "Rating");
      ProfilerDict.Add("URL", "URL");
      ProfilerDict.Add("Country", "Country");
      ProfilerDict.Add("Year", "Year");
      ProfilerDict.Add("Length", "Length");
      ProfilerDict.Add("Actors", "Actors");
      ProfilerDict.Add("Category", "Category");
      ProfilerDict.Add("Director", "Director");
      ProfilerDict.Add("Producer", "Producer");
      ProfilerDict.Add("Description", "Description");
      ProfilerDict.Add("Picture", "Picture");
      ProfilerDict.Add("Date", "Date");
      ProfilerDict.Add("Checked", "Checked");
      ProfilerDict.Add("Source", "Source");
      ProfilerDict.Add("VideoFormat", "VideoFormat");
      ProfilerDict.Add("AudioFormat", "AudioFormat");
      ProfilerDict.Add("Resolution", "Resolution");
      ProfilerDict.Add("Size", "Size");
      ProfilerDict.Add("VideoBitrate", "VideoBitrate");
      ProfilerDict.Add("AudioBitrate", "AudioBitrate");
      ProfilerDict.Add("Disks", "Disks");
      ProfilerDict.Add("Framerate", "Framerate");
      ProfilerDict.Add("Comments", "Comments");
      ProfilerDict.Add("Languages", "Languages");
      ProfilerDict.Add("Subtitles", "Subtitles");
      //ProfilerDict.Add("Borrower", "Borrower");

    }
    public string ConvertXbmcNfo(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, string foldermovies, bool OnlyFile, string TitleDelim)
    {
      if (TitleDelim.Length == 0)
        TitleDelim = "\\";
      string[] wStrStorage = MyFilmsGUI.MyFilms.conf.StrDirStor.Split(new Char[] { ';' }); //The movie folders configured in setup to contain the movies ...
      string WStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = WStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + ".xml";
      XmlTextWriter destXml = new XmlTextWriter(destFile, Encoding.Default) {Formatting = Formatting.Indented};
      destXml.WriteStartDocument();
      destXml.WriteStartElement("AntMovieCatalog");
      destXml.WriteStartElement("Catalog");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      foreach (string wpath in wStrStorage)
      {
        try
        {
          XmlDocument doc = new XmlDocument();
          doc.Load(source);
          XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/EaxMovieCatalog/Catalog/Contents/Movie");
          foreach (XmlNode nodeDvd in dvdList)
          {
            CultureInfo ci = new CultureInfo("en-us");

            XmlNodeList DiscsList = nodeDvd.SelectNodes("Disc");
            string wfile = string.Empty;
            string wVideoCodec = string.Empty;
            string wAudioCodec = string.Empty;
            string wResolution = string.Empty;
            string wFileSize = string.Empty;
            string wVideobitrate = string.Empty;
            string wAudiobitrate = string.Empty;
            string wFramerate = string.Empty;

            if ((DiscsList != null) && (DiscsList.Count != 0))
            {
              foreach (XmlNode nodeDisc in DiscsList)
              {
                if (nodeDisc.Attributes["VideoCodec"] != null)
                  wVideoCodec = nodeDisc.Attributes["VideoCodec"].Value;
                if (nodeDisc.Attributes["AudioCodec"] != null)
                  wAudioCodec = nodeDisc.Attributes["AudioCodec"].Value;
                if (nodeDisc.Attributes["VideoResolution"] != null)
                  wResolution = nodeDisc.Attributes["VideoResolution"].Value;
                if (nodeDisc.Attributes["VideoFilesize"] != null)
                  if (wFileSize.Length > 0)
                    wFileSize = wFileSize + "+" + nodeDisc.Attributes["VideoFilesize"].Value;
                  else
                    wFileSize = nodeDisc.Attributes["VideoFilesize"].Value;
                if (nodeDisc.Attributes["VideoBitrate"] != null)
                  wVideobitrate = nodeDisc.Attributes["VideoBitrate"].Value;
                if (nodeDisc.Attributes["AudioBitrate"] != null)
                  wAudiobitrate = nodeDisc.Attributes["AudioBitrate"].Value;
                if (nodeDisc.Attributes["VideoFramerate"] != null)
                  wFramerate = nodeDisc.Attributes["VideoFramerate"].Value;
                if (nodeDisc.Attributes["VideoFullPath"] != null)
                  if (wfile.Length > 0)
                    wfile = wfile + ";" + nodeDisc.Attributes["VideoFullPath"].Value;
                  else
                    wfile = nodeDisc.Attributes["VideoFullPath"].Value;
              }
            }
            destXml.WriteStartElement("Movie");
            string wID = nodeDvd.Attributes["Label"].Value;
            if (wID != null && wID.Length > 3)
              WriteAntAtribute(destXml, "Number", wID.Substring(3));
            else
              WriteAntAtribute(destXml, "Number", "9999");
            if (nodeDvd.Attributes["OriginalTitle"] != null && nodeDvd.Attributes["OriginalTitle"].Value.Length > 0)
              WriteAntAtribute(destXml, "OriginalTitle", nodeDvd.Attributes["OriginalTitle"].Value);
            else
              if (nodeDvd.Attributes["TranslatedTitle"] != null && nodeDvd.Attributes["TranslatedTitle"].Value.Length > 0)
                WriteAntAtribute(destXml, "OriginalTitle", nodeDvd.Attributes["TranslatedTitle"].Value);
            if (nodeDvd.Attributes["TranslatedTitle"] != null && nodeDvd.Attributes["TranslatedTitle"].Value.Length > 0)
              WriteAntAtribute(destXml, "TranslatedTitle", nodeDvd.Attributes["TranslatedTitle"].Value);
            else
              if (nodeDvd.Attributes["OriginalTitle"] != null && nodeDvd.Attributes["OriginalTitle"].Value.Length > 0)
                WriteAntAtribute(destXml, "TranslatedTitle", nodeDvd.Attributes["OriginalTitle"].Value);
            if (nodeDvd.Attributes["FormattedTitle"] != null && nodeDvd.Attributes["FormattedTitle"].Value.Length > 0)
              WriteAntAtribute(destXml, "FormattedTitle", nodeDvd.Attributes["FormattedTitle"].Value);
            //WriteAntAtribute(destXml, "Notes/File", File);
            if (nodeDvd.Attributes["User1Seen"] != null)
              WriteAntAtribute(destXml, "Checked", nodeDvd.Attributes["User1Seen"].Value.ToUpper() == "YES" ? "true" : "false");
            DateTime dt = new DateTime();
            if (nodeDvd.Attributes["ModifiedDate"] != null)
              try
              {
                dt = DateTime.Parse(nodeDvd.Attributes["ModifiedDate"].Value, ci);
              }
              catch
              {
                dt = DateTime.Today;
              }
            WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
            if (nodeDvd.Attributes["Country"] != null)
              WriteAntAtribute(destXml, "Country", nodeDvd.Attributes["Country"].Value.StartsWith("|")
                  ? nodeDvd.Attributes["Country"].Value.Substring(1)
                  : nodeDvd.Attributes["Country"].Value);
            string wRating = "0";
            if (nodeDvd.Attributes["Rating"] != null)
              wRating = nodeDvd.Attributes["Rating"].Value;
            decimal wrating = 0;
            try { wrating = Convert.ToDecimal(wRating, ci); }
            catch
            {
              try { wrating = Convert.ToDecimal(wRating); }
              catch { }
            }
            WriteAntAtribute(destXml, "Rating", wrating.ToString().Replace(",", "."));
            string wYear = nodeDvd.Attributes["Year"].Value;
            if (!string.IsNullOrEmpty(wYear))
              WriteAntAtribute(destXml, "Year", wYear);
            if (nodeDvd.Attributes["RunningTime"] != null)
              WriteAntAtribute(destXml, "Length", nodeDvd.Attributes["RunningTime"].Value);
            if (nodeDvd.Attributes["Genre"] != null)
              WriteAntAtribute(destXml, "Category", nodeDvd.Attributes["Genre"].Value.StartsWith("|")
                ? nodeDvd.Attributes["Genre"].Value.Substring(1).Replace("|", "")
                : nodeDvd.Attributes["Genre"].Value.Replace("|", ","));
            if (nodeDvd.Attributes["Director"] != null)
              WriteAntAtribute(destXml, "Director", nodeDvd.Attributes["Director"].Value);
            if (nodeDvd.Attributes["Producer"] != null)
              WriteAntAtribute(destXml, "Producer", nodeDvd.Attributes["Producer"].Value);
            if (nodeDvd.Attributes["Cast"] != null)
              WriteAntAtribute(destXml, "Actors", nodeDvd.Attributes["Cast"].Value);
            if (nodeDvd.Attributes["Picture"] != null)
              WriteAntAtribute(destXml, "Picture", nodeDvd.Attributes["Picture"].Value);
            if (nodeDvd.Attributes["Format"] != null)
              WriteAntAtribute(destXml, "MediaType", nodeDvd.Attributes["Format"].Value);
            if (nodeDvd.Attributes["Media"] != null)
              WriteAntAtribute(destXml, "MediaLabel", nodeDvd.Attributes["Media"].Value);
            if (nodeDvd.Attributes["Website"] != null)
              WriteAntAtribute(destXml, "URL", nodeDvd.Attributes["Website"].Value);
            if (nodeDvd.Attributes["PlotOriginal"] != null)
              WriteAntAtribute(destXml, "Description", nodeDvd.Attributes["PlotOriginal"].Value);
            if (nodeDvd.Attributes["Comments"] != null)
              WriteAntAtribute(destXml, "Comments", nodeDvd.Attributes["Comments"].Value);
            if (nodeDvd.Attributes["Language"] != null)
              WriteAntAtribute(destXml, "Languages", nodeDvd.Attributes["Language"].Value.StartsWith("|")
                ? nodeDvd.Attributes["Language"].Value.Substring(1)
                : nodeDvd.Attributes["Language"].Value);
            if (nodeDvd.Attributes["Subtitles"] != null)
              WriteAntAtribute(destXml, "Subtitles", nodeDvd.Attributes["Subtitles"].Value);
            WriteAntAtribute(destXml, "Source", wfile);
            if (wVideoCodec.Length > 0)
              WriteAntAtribute(destXml, "VideoFormat", wVideoCodec);
            if (wAudioCodec.Length > 0)
              WriteAntAtribute(destXml, "AudioFormat", wAudioCodec);
            if (wResolution.Length > 0)
              WriteAntAtribute(destXml, "Resolution", wResolution);
            if (wVideobitrate.Length > 0)
              WriteAntAtribute(destXml, "VideoBitrate", wVideobitrate.Substring(0, wVideobitrate.IndexOf(" ") - 1));
            if (wAudiobitrate.Length > 0)
              WriteAntAtribute(destXml, "AudioBitrate", wAudiobitrate.Substring(0, wAudiobitrate.IndexOf(" ") - 1));
            if (wFileSize.Length > 0)
              WriteAntAtribute(destXml, "Size", wFileSize);
            WriteAntAtribute(destXml, "Disks", DiscsList.Count.ToString());
            if (wFramerate.Length > 0)
              WriteAntAtribute(destXml, "Framerate", wFramerate);


            // Now writing MF extended attributes
            //WriteAntElement(destXml, "mpaa", Certification);
            //WriteAntElement(destXml, "tagline", Tagline);
            //WriteAntElement(destXml, "tags", Tags);
            //WriteAntElement(destXml, "scenario", Writer);

            destXml.WriteEndElement();
          }

        }
        catch
        {
          return string.Empty;
        }
      }
      destXml.WriteEndElement();
      destXml.WriteEndElement();
      destXml.Close();
      return destFile;
    }

    private void WriteAntAtribute(XmlTextWriter tw, string key, string value)
    {
      string at = string.Empty;
      if (ProfilerDict.TryGetValue(key, out at))
      {
        tw.WriteAttributeString(at, value);
      }
    }
  }

}
