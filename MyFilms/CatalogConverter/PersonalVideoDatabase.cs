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
using Grabber;

namespace MyFilmsPlugin.CatalogConverter
{
  class PersonalVideoDatabase
  {
    public Dictionary<string, string> ProfilerDict;

    public PersonalVideoDatabase()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("type", "MediaType");
      ProfilerDict.Add("label", "MediaLabel");
      ProfilerDict.Add("origtitle", "OriginalTitle");
      ProfilerDict.Add("title", "TranslatedTitle");
      ProfilerDict.Add("aka", "FormattedTitle");
      ProfilerDict.Add("num", "Number");
      ProfilerDict.Add("imdbrating", "Rating");
      ProfilerDict.Add("url", "URL");
      ProfilerDict.Add("country", "Country");
      ProfilerDict.Add("year", "Year");
      ProfilerDict.Add("length", "Length");
      ProfilerDict.Add("actors", "Actors");
      ProfilerDict.Add("genre", "Category");
      ProfilerDict.Add("director", "Director");
      ProfilerDict.Add("producer", "Producer");
      ProfilerDict.Add("description", "Description");
      ProfilerDict.Add("poster", "Picture");
      ProfilerDict.Add("dateadded", "Date");
      ProfilerDict.Add("viewed", "Checked");
      ProfilerDict.Add("path", "Source");
      ProfilerDict.Add("videocodec", "VideoFormat");
      ProfilerDict.Add("audiocodec", "AudioFormat");
      ProfilerDict.Add("resolution", "Resolution");
      ProfilerDict.Add("size", "Size");
      ProfilerDict.Add("videobitrate", "VideoBitrate");
      ProfilerDict.Add("audiobitrate", "AudioBitrate");
      ProfilerDict.Add("count", "Disks");
      ProfilerDict.Add("framerate", "Framerate");
      ProfilerDict.Add("comment", "Comments");
      ProfilerDict.Add("langs", "Languages");
      ProfilerDict.Add("subs", "Subtitles");
      ProfilerDict.Add("mpaa", "Certification");
      ProfilerDict.Add("tagline", "TagLine");
      ProfilerDict.Add("tags", "Tags");
      ProfilerDict.Add("scenario", "Writer");
      //ProfilerDict.Add("Borrower", "Borrower");
    }
    public string ConvertPersonalVideoDatabase(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, bool OnlyFile, string TitleDelim, bool AddTaglineToDescription)
    {
      if (TitleDelim.Length == 0)
        TitleDelim = "\\";
      string WStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = WStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + "_tmp.xml";
      XmlTextWriter destXml = new XmlTextWriter(destFile, Encoding.Default) {Formatting = Formatting.Indented};
      destXml.WriteStartDocument();
      destXml.WriteStartElement("AntMovieCatalog");
      destXml.WriteStartElement("Catalog");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(source);
        XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/xml/viddb/movie");
        foreach (XmlNode nodeDVD in dvdList)
        {

          //+		[0]	{Element, Name="num"}	object {System.Xml.XmlElement}
          //+		[1]	{Element, Name="title"}	object {System.Xml.XmlElement}
          //+		[2]	{Element, Name="origtitle"}	object {System.Xml.XmlElement}
          //+		[3]	{Element, Name="aka"}	object {System.Xml.XmlElement}
          //+		[4]	{Element, Name="year"}	object {System.Xml.XmlElement}
          //+		[5]	{Element, Name="genre"}	object {System.Xml.XmlElement}
          //+		[6]	{Element, Name="country"}	object {System.Xml.XmlElement}
          //+		[7]	{Element, Name="studio"}	object {System.Xml.XmlElement}
          //+		[8]	{Element, Name="release"}	object {System.Xml.XmlElement}
          //+		[9]	{Element, Name="mpaa"}	object {System.Xml.XmlElement}
          //+		[10]	{Element, Name="tags"}	object {System.Xml.XmlElement}
          //+		[11]	{Element, Name="director"}	object {System.Xml.XmlElement}
          //+		[12]	{Element, Name="producer"}	object {System.Xml.XmlElement}
          //+		[13]	{Element, Name="scenario"}	object {System.Xml.XmlElement}
          //+		[14]	{Element, Name="music"}	object {System.Xml.XmlElement}
          //+		[15]	{Element, Name="actors"}	object {System.Xml.XmlElement}
          //+		[16]	{Element, Name="description"}	object {System.Xml.XmlElement}
          //+		[17]	{Element, Name="comment"}	object {System.Xml.XmlElement}
          //+		[18]	{Element, Name="tagline"}	object {System.Xml.XmlElement}
          //+		[19]	{Element, Name="url"}	object {System.Xml.XmlElement}
          //+		[20]	{Element, Name="imdbrating"}	object {System.Xml.XmlElement}
          //+		[21]	{Element, Name="rating"}	object {System.Xml.XmlElement}
          //+		[22]	{Element, Name="orating"}	object {System.Xml.XmlElement}
          //+		[23]	{Element, Name="dateadded"}	object {System.Xml.XmlElement}
          //+		[24]	{Element, Name="path"}	object {System.Xml.XmlElement}
          //+		[25]	{Element, Name="count"}	object {System.Xml.XmlElement}
          //+		[26]	{Element, Name="type"}	object {System.Xml.XmlElement}
          //+		[27]	{Element, Name="rip"}	object {System.Xml.XmlElement}
          //+		[28]	{Element, Name="length"}	object {System.Xml.XmlElement}
          //+		[29]	{Element, Name="size"}	object {System.Xml.XmlElement}
          //+		[30]	{Element, Name="langs"}	object {System.Xml.XmlElement}
          //+		[31]	{Element, Name="subs"}	object {System.Xml.XmlElement}
          //+		[32]	{Element, Name="translation"}	object {System.Xml.XmlElement}
          //+		[33]	{Element, Name="resolution"}	object {System.Xml.XmlElement}
          //+		[34]	{Element, Name="framerate"}	object {System.Xml.XmlElement}
          //+		[35]	{Element, Name="videocodec"}	object {System.Xml.XmlElement}
          //+		[36]	{Element, Name="videobitrate"}	object {System.Xml.XmlElement}
          //+		[37]	{Element, Name="audiocodec"}	object {System.Xml.XmlElement}
          //+		[38]	{Element, Name="audiobitrate"}	object {System.Xml.XmlElement}
          //+		[39]	{Element, Name="label"}	object {System.Xml.XmlElement}
          //+		[40]	{Element, Name="features"}	object {System.Xml.XmlElement}
          //+		[41]	{Element, Name="viewed"}	object {System.Xml.XmlElement}
          //+		[42]	{Element, Name="viewdate"}	object {System.Xml.XmlElement}
          //+		[43]	{Element, Name="wish"}	object {System.Xml.XmlElement}
          //+		[44]	{Element, Name="bookmark"}	object {System.Xml.XmlElement}
          //+		[45]	{Element, Name="poster"}	object {System.Xml.XmlElement}

          CultureInfo ci = new CultureInfo("en-us");
          string Tags = string.Empty;
          string Tagline = string.Empty;
          string Certification = string.Empty;

          destXml.WriteStartElement("Movie");
          string wID = nodeDVD.SelectSingleNodeFast("num").InnerText;
          WriteAntAtribute(destXml, "num", !string.IsNullOrEmpty(wID) ? wID : "9999");
          if (nodeDVD.SelectSingleNodeFast("origtitle") != null && !string.IsNullOrEmpty(nodeDVD.SelectSingleNodeFast("origtitle").InnerText))
            WriteAntAtribute(destXml, "origtitle", nodeDVD.SelectSingleNodeFast("origtitle").InnerText);
          if (nodeDVD.SelectSingleNodeFast("title") != null && nodeDVD.SelectSingleNodeFast("title").InnerText.Length > 0)
            WriteAntAtribute(destXml, "title", nodeDVD.SelectSingleNodeFast("title").InnerText);
          if (nodeDVD.SelectSingleNodeFast("aka") != null && nodeDVD.SelectSingleNodeFast("aka").InnerText.Length > 0)
            WriteAntAtribute(destXml, "aka", nodeDVD.SelectSingleNodeFast("aka").InnerText);
          if (nodeDVD.SelectSingleNodeFast("viewed") != null)
            WriteAntAtribute(destXml, "viewed", nodeDVD.SelectSingleNodeFast("viewed").InnerText == "-1" ? "true" : "false");
          DateTime dt = new DateTime();
          if (nodeDVD.SelectSingleNodeFast("dateadded") != null)
            try
            {
              dt = DateTime.Parse(nodeDVD.SelectSingleNodeFast("dateadded").InnerText);
            }
            catch
            {
              try
              {
                dt = DateTime.Parse(nodeDVD.SelectSingleNodeFast("dateadded").InnerText, ci);
              }
              catch
              {
                dt = DateTime.Today;
              }
            }
          WriteAntAtribute(destXml, "dateadded", dt.ToShortDateString());
          if (nodeDVD.SelectSingleNodeFast("country") != null)
            WriteAntAtribute(destXml, "country", nodeDVD.SelectSingleNodeFast("country").InnerText.StartsWith("|")
                ? nodeDVD.SelectSingleNodeFast("country").InnerText.Substring(1)
                : nodeDVD.SelectSingleNodeFast("country").InnerText);
          string wRating = "0";
          if (nodeDVD.SelectSingleNodeFast("imdbrating") != null && !string.IsNullOrEmpty(nodeDVD.SelectSingleNodeFast("imdbrating").InnerText) && nodeDVD.SelectSingleNodeFast("imdbrating").InnerText != "0")
            wRating = nodeDVD.SelectSingleNodeFast("imdbrating").InnerText;
          else if (nodeDVD.SelectSingleNodeFast("rating") != null)
          {
            wRating = nodeDVD.SelectSingleNodeFast("rating").InnerText;
          }
          decimal wrating = 0;
          try { wrating = Convert.ToDecimal(wRating, ci); }
          catch
          {
            try { wrating = Convert.ToDecimal(wRating); }
            catch { }
          }
          WriteAntAtribute(destXml, "imdbrating", wrating.ToString().Replace(",", "."));
          string wYear = nodeDVD.SelectSingleNodeFast("year").InnerText;
          if (!string.IsNullOrEmpty(wYear))
            WriteAntAtribute(destXml, "year", wYear);
          if (nodeDVD.SelectSingleNodeFast("length") != null)
            WriteAntAtribute(destXml, "length", nodeDVD.SelectSingleNodeFast("length").InnerText);
          if (nodeDVD.SelectSingleNodeFast("genre") != null)
            WriteAntAtribute(destXml, "genre", nodeDVD.SelectSingleNodeFast("genre").InnerText.StartsWith("|")
                ? nodeDVD.SelectSingleNodeFast("genre").InnerText.Substring(1).Replace("|", "")
                : nodeDVD.SelectSingleNodeFast("genre").InnerText.Replace("|", ","));
          if (nodeDVD.SelectSingleNodeFast("director") != null)
            WriteAntAtribute(destXml, "director", nodeDVD.SelectSingleNodeFast("director").InnerText);
          if (nodeDVD.SelectSingleNodeFast("producer") != null)
            WriteAntAtribute(destXml, "producer", nodeDVD.SelectSingleNodeFast("producer").InnerText);
          string Writer = string.Empty;
          if (nodeDVD.SelectSingleNodeFast("scenario") != null) // Writer
            Writer = nodeDVD.SelectSingleNodeFast("scenario").InnerText;
          //WriteAntAtribute(destXml, "scenario", Writer);
          if (nodeDVD.SelectSingleNodeFast("actors") != null)
            WriteAntAtribute(destXml, "actors", nodeDVD.SelectSingleNodeFast("actors").InnerText);
          if (nodeDVD.SelectSingleNodeFast("poster") != null)
            WriteAntAtribute(destXml, "poster", nodeDVD.SelectSingleNodeFast("poster").InnerText);
          if (nodeDVD.SelectSingleNodeFast("type") != null)
            WriteAntAtribute(destXml, "type", nodeDVD.SelectSingleNodeFast("type").InnerText);
          if (nodeDVD.SelectSingleNodeFast("label") != null)
            WriteAntAtribute(destXml, "label", nodeDVD.SelectSingleNodeFast("label").InnerText);
          if (nodeDVD.SelectSingleNodeFast("url") != null)
            WriteAntAtribute(destXml, "url", nodeDVD.SelectSingleNodeFast("url").InnerText);
          if (nodeDVD.SelectSingleNodeFast("langs") != null)
            WriteAntAtribute(destXml, "langs", nodeDVD.SelectSingleNodeFast("langs").InnerText.StartsWith("|")
                ? nodeDVD.SelectSingleNodeFast("langs").InnerText.Substring(1)
                : nodeDVD.SelectSingleNodeFast("langs").InnerText);
          if (nodeDVD.SelectSingleNodeFast("subs") != null)
            WriteAntAtribute(destXml, "subs", nodeDVD.SelectSingleNodeFast("subs").InnerText);
          if (nodeDVD.SelectSingleNodeFast("videocodec") != null)
            WriteAntAtribute(destXml, "videocodec", nodeDVD.SelectSingleNodeFast("videocodec").InnerText);
          if (nodeDVD.SelectSingleNodeFast("audiocodec") != null)
            WriteAntAtribute(destXml, "audiocodec", nodeDVD.SelectSingleNodeFast("audiocodec").InnerText);
          if (nodeDVD.SelectSingleNodeFast("resolution") != null)
            WriteAntAtribute(destXml, "resolution", nodeDVD.SelectSingleNodeFast("resolution").InnerText);
          if (nodeDVD.SelectSingleNodeFast("videobitrate") != null)
            WriteAntAtribute(destXml, "videobitrate", nodeDVD.SelectSingleNodeFast("videobitrate").InnerText);
          if (nodeDVD.SelectSingleNodeFast("audiobitrate") != null)
            WriteAntAtribute(destXml, "audiobitrate", nodeDVD.SelectSingleNodeFast("audiobitrate").InnerText);
          if (nodeDVD.SelectSingleNodeFast("size") != null)
            WriteAntAtribute(destXml, "size", nodeDVD.SelectSingleNodeFast("size").InnerText);
          if (nodeDVD.SelectSingleNodeFast("framerate") != null)
            WriteAntAtribute(destXml, "framerate", nodeDVD.SelectSingleNodeFast("framerate").InnerText);
          if (nodeDVD.SelectSingleNodeFast("mpaa") != null)
          {
            Certification = nodeDVD.SelectSingleNodeFast("mpaa").InnerText;
            //WriteAntAtribute(destXml, "mpaa", Certification);
          }
          if (nodeDVD.SelectSingleNodeFast("tagline") != null)
          {
            Tagline = nodeDVD.SelectSingleNodeFast("tagline").InnerText;
            //WriteAntAtribute(destXml, "tagline", Tagline);
          }
          if (nodeDVD.SelectSingleNodeFast("tags") != null)
          {
            Tags = nodeDVD.SelectSingleNodeFast("tags").InnerText;
            //WriteAntAtribute(destXml, "tags", Tags);
          }
          if (nodeDVD.SelectSingleNodeFast("count") != null)
            WriteAntAtribute(destXml, "count", nodeDVD.SelectSingleNodeFast("count").InnerText);

          //string Description, 
          string DescriptionMerged = string.Empty;
          if (DestinationTagline == "Description")
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += Tagline;
          }
          if (DestinationTags == "Description")
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += Tags;
          }
          if (DestinationCertification == "Description")
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += Certification;
          }
          XmlNode nodePlot = nodeDVD.SelectSingleNodeFast("Plot");
          if (nodeDVD.SelectSingleNodeFast("description") != null && nodeDVD.SelectSingleNodeFast("description").InnerText != null)
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += nodeDVD.SelectSingleNodeFast("description").InnerText;
          }
          WriteAntAtribute(destXml, "description", DescriptionMerged);

          //string Comments, 
          string CommentsMerged = string.Empty;
          if (DestinationTagline == "Comments")
          {
            if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
            CommentsMerged += Tagline;
          }
          if (DestinationTags == "Comments")
          {
            if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
            CommentsMerged += Tags;
          }
          if (DestinationCertification == "Comments")
          {
            if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
            CommentsMerged += Certification;
          }
          if (nodeDVD.SelectSingleNodeFast("comment") != null && nodeDVD.SelectSingleNodeFast("comment").InnerText != null)
          {
            if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
            CommentsMerged += nodeDVD.SelectSingleNodeFast("comment").InnerText;
          }
          WriteAntAtribute(destXml, "comment", CommentsMerged);

          if (nodeDVD.SelectSingleNodeFast("path") != null)
            WriteAntAtribute(destXml, "path", nodeDVD.SelectSingleNodeFast("path").InnerText);

          // Now writing MF extended attributes
          WriteAntElement(destXml, "mpaa", Certification);
          WriteAntElement(destXml, "tagline", Tagline);
          WriteAntElement(destXml, "tags", Tags);
          WriteAntElement(destXml, "scenario", Writer);

          destXml.WriteEndElement();
        }
      }
      catch
      {
        return string.Empty;
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

    private void WriteAntElement(XmlWriter tw, string key, string value)
    {
      string at = string.Empty;
      if (ProfilerDict.TryGetValue(key, out at))
      {
        tw.WriteElementString(at, value);
        //LogMyFilms.Debug("XMM Importer: Writing Property '" + key + "' with Value '" + value.ToString() + "' to DB.");
      }
      else
      {
        //LogMyFilms.Debug("XMM Importer Property '" + key + "' not found in dictionary ! - Element not written to DB !");
      }
    }

  }

}
