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
  class XbmcDb
  {
    public Dictionary<string, string> ProfilerDict;

    public XbmcDb()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("id", "Number");
      ProfilerDict.Add("lastplayed", "Checked");
      ProfilerDict.Add("originaltitle", "OriginalTitle");
      ProfilerDict.Add("title", "TranslatedTitle");
      ProfilerDict.Add("FormattedTitle", "FormattedTitle");
      ProfilerDict.Add("filenameandpath", "Source");
      ProfilerDict.Add("MediaType", "MediaType");
      ProfilerDict.Add("MediaLabel", "MediaLabel");
      ProfilerDict.Add("rating", "Rating");
      ProfilerDict.Add("InternetID", "URL");
      ProfilerDict.Add("country", "Country");
      ProfilerDict.Add("year", "Year");
      ProfilerDict.Add("runtime", "Length");
      ProfilerDict.Add("actor", "Actors");
      ProfilerDict.Add("genre", "Category");
      ProfilerDict.Add("director", "Director");
      ProfilerDict.Add("credits", "Producer");
      ProfilerDict.Add("plot", "Description");
      ProfilerDict.Add("Picture", "Picture");
      ProfilerDict.Add("Date", "Date");
      ProfilerDict.Add("VideoCodec", "VideoFormat");
      ProfilerDict.Add("AudioCodec", "AudioFormat");
      ProfilerDict.Add("Resolution", "Resolution");
      ProfilerDict.Add("Size", "Size");
      ProfilerDict.Add("VideoBitrate", "VideoBitrate");
      ProfilerDict.Add("AudioBitrate", "AudioBitrate");
      ProfilerDict.Add("Disks", "Disks");
      ProfilerDict.Add("Framerate", "Framerate");
      ProfilerDict.Add("Comments", "Comments");
      ProfilerDict.Add("language", "Languages");
      ProfilerDict.Add("Subtitles", "Subtitles");
      //ProfilerDict.Add("Borrower", "Borrower");

    }

    //public string ConvertXMM(string source, string folderimage, bool SortTitle, bool OnlyFile) // XMM Example
    public string ConvertXbmcDb(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, string foldermovies, bool OnlyFile, string TitleDelim)
    {
      if (TitleDelim.Length == 0)
        TitleDelim = "\\";
      //            string[] wStrStorage = MyFilms.conf.StrDirStor.ToString().Split(new Char[] { ';' });
      string WStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = WStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + "_tmp.xml"; // Write result to _tmp file
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
        XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/videodb/movie");
        foreach (XmlNode nodeDVD in dvdList)
        {
          destXml.WriteStartElement("Movie");

          //int Number, 
          string ID = string.Empty;
          XmlNode nodeNumber = nodeDVD.SelectSingleNodeFast("id");
          if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.InnerText))
            ID = nodeNumber.InnerText;
          if (ID.ToLower().StartsWith("tt"))
          {
            ID = ID.Substring(2);
            ID = ID.TrimStart('0');
          }
          WriteAntAtribute(destXml, "id", ID.Length > 0 ? ID : "9999");

          //string Checked, 
          XmlNode nodeChecked = nodeDVD.SelectSingleNodeFast("lastplayed");
          if (nodeChecked != null && nodeChecked.InnerText.Length > 0)
            WriteAntAtribute(destXml, "lastplayed", "true");
          else
            WriteAntAtribute(destXml, "lastplayed", "false");

          ////string MediaLabel, 
          //XmlNode nodeMediaLabel = nodeDVD.SelectSingleNodeFast("MediaLabel");
          //if (nodeMediaLabel != null && nodeMediaLabel.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "MediaLabel", nodeMediaLabel.InnerText);

          ////string MediaType, 
          //XmlNode nodeMediaType = nodeDVD.SelectSingleNodeFast("Media");
          //if (nodeMediaType != null && nodeMediaType.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "Media", nodeMediaType.InnerText);



          //string OriginalTitle, 
          //string TranslatedTitle, 
          //string FormattedTitle, 
          XmlNode nodeOTitle = nodeDVD.SelectSingleNodeFast("originaltitle");
          XmlNode nodeTitle = nodeDVD.SelectSingleNodeFast("title");
          //XmlNode nodeSTitle = nodeDVD.SelectSingleNodeFast("SortTitle");
          if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
            WriteAntAtribute(destXml, "title", nodeOTitle.InnerText);
          else
            WriteAntAtribute(destXml, "title", "no title found");
          if (nodeTitle != null && nodeTitle.InnerText.Length > 0)
            WriteAntAtribute(destXml, "originaltitle", nodeTitle.InnerText);
          else
            WriteAntAtribute(destXml, "originaltitle", nodeTitle.InnerText);

          //string Source, 
          string strSource = String.Empty;
          XmlNode nodeSource = nodeDVD.SelectSingleNodeFast("filenameandpath");
          if (nodeSource != null && nodeSource.InnerText.Length > 0)
            strSource += nodeSource.InnerText;
          strSource = strSource.StartsWith("stack://") ? strSource.Substring(9).Replace(" , ", "'; ") : strSource.Replace("/", @"\");
          WriteAntAtribute(destXml, "filenameandpath", strSource);


          ////string Date, 
          //XmlNode nodeDate = nodeDVD.SelectSingleNodeFast("DateInsert");
          //try
          //{
          //  DateTime dt = new DateTime();
          //  dt = DateTime.Parse(nodeDate.InnerText.ToString());
          //  WriteAntAtribute(destXml, "DateInsert", dt.ToShortDateString());
          //}
          //catch
          //{
          //}

          //string Borrower, 

          //decimal Rating, 
          string Rating = string.Empty;
          decimal wrating = 0;
          CultureInfo ci = new CultureInfo("en-us");
          XmlNode nodeRating = nodeDVD.SelectSingleNodeFast("rating");
          if (nodeRating != null && nodeRating.InnerText != null)
          {
            try { wrating = Convert.ToDecimal(nodeRating.InnerText); }
            catch
            {
              try { wrating = Convert.ToDecimal(nodeRating.InnerText, ci); }
              catch { }
            }
          }
          Rating = wrating.ToString("0.0", ci);
          WriteAntAtribute(destXml, "rating", Rating);


          //string Director, 
          XmlNode nodeDirector = nodeDVD.SelectSingleNodeFast("director");
          if (nodeDirector != null && nodeDirector.InnerText.Length > 0)
            WriteAntAtribute(destXml, "director", nodeDirector.InnerText);

          //string Country, 
          XmlNode nodeCountry = nodeDVD.SelectSingleNodeFast("country");
          if (nodeCountry != null && nodeCountry.InnerText.Length > 0)
            WriteAntAtribute(destXml, "country", nodeCountry.InnerText);

          //string Category, 
          string genre = String.Empty;
          XmlNodeList genreList = nodeDVD.SelectNodes("genre");
          foreach (XmlNode nodeGenre in genreList)
          {
            if (genre.Length > 0)
              genre += ", ";
            if (nodeGenre.InnerText != null)
              genre += nodeGenre.InnerText;
          }
          WriteAntAtribute(destXml, "genre", genre);

          //string Year, 
          XmlNode nodeYear = nodeDVD.SelectSingleNodeFast("year");
          if (nodeYear != null)
            WriteAntAtribute(destXml, "year", nodeYear.InnerText);

          //string Length, 
          int strLengthnum = 0;
          XmlNode nodeDuration = nodeDVD.SelectSingleNodeFast("runtime");

          try
          { strLengthnum = Int32.Parse(nodeDuration.InnerText); }
          catch
          { strLengthnum = 0; }
          if (nodeDuration != null && nodeDuration.InnerText.Length > 0)
            WriteAntAtribute(destXml, "runtime", strLengthnum.ToString());

          //string Producer, 
          string credits = String.Empty;
          XmlNodeList creditsList = nodeDVD.SelectNodes("credits");
          foreach (XmlNode nodeCredits in creditsList)
          {
            if (credits.Length > 0)
              credits += ", ";
            if (nodeCredits.InnerText != null)
              genre += nodeCredits.InnerText;
          }
          WriteAntAtribute(destXml, "credits", genre);

          //string Actors, 
          string Actor = String.Empty;
          XmlNodeList actorList = nodeDVD.SelectNodes("actor");
          foreach (XmlNode nodeActor in actorList)
          {
            string line = String.Empty;
            if (nodeActor.SelectSingleNodeFast("name") != null)
              line = nodeActor.SelectSingleNodeFast("name").InnerText;
            if ((nodeActor.SelectSingleNodeFast("role") != null) && (nodeActor.SelectSingleNodeFast("role").InnerText.Length > 0))
              line += " (" + nodeActor.SelectSingleNodeFast("role").InnerText + ")";
            if (line.Length > 0)
            {
              if (Actor.Length > 0) Actor += ", ";
              Actor += line;
            }
          }
          WriteAntAtribute(destXml, "actor", Actor);

          //string URL, 
          string strURL = String.Empty;
          XmlNode nodeURL = nodeDVD.SelectSingleNodeFast("id");
          if (nodeURL != null && nodeURL.InnerText != null)
            strURL = @"http://www.imdb.com/title/" + nodeURL.InnerText;
          WriteAntAtribute(destXml, "InternetID", strURL);

          //string Description, 
          XmlNode nodePlot = nodeDVD.SelectSingleNodeFast("plot");
          if (nodePlot != null && nodePlot.InnerText != null)
            WriteAntAtribute(destXml, "plot", nodePlot.InnerText);

          //string Comments, 
          //XmlNode nodeComments = nodeDVD.SelectSingleNodeFast("Comments");
          //if (nodeComments != null && nodeComments.InnerText != null)
          //  WriteAntAtribute(destXml, "Comments", nodeComments.InnerText);

          //string VideoFormat, Codec
          XmlNode nodeVideoFormat = nodeDVD.SelectSingleNodeFast("fileinfo/streamdetails/video/codec");
          if (nodeVideoFormat != null && nodeVideoFormat.InnerText.Length > 0)
            WriteAntAtribute(destXml, "VideoCodec", nodeVideoFormat.InnerText);

          ////string VideoBitrate, Bitrate 
          //XmlNode nodeVideoBitrate = nodeDVD.SelectSingleNodeFast("Bitrate");
          //if (nodeVideoBitrate != null && nodeVideoBitrate.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "Bitrate", nodeVideoBitrate.InnerText);

          //string AudioFormat, 
          XmlNode nodeAudioFormat = nodeDVD.SelectSingleNodeFast("fileinfo/streamdetails/audio/codec");
          if (nodeAudioFormat != null && nodeAudioFormat.InnerText.Length > 0)
            WriteAntAtribute(destXml, "AudioCodec", nodeAudioFormat.InnerText);

          ////string AudioBitrate, 
          //XmlNode nodeAudioBitrate = nodeDVD.SelectSingleNodeFast("AudioBitRate");
          //if (nodeAudioBitrate != null && nodeAudioBitrate.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "AudioBitRate", nodeAudioBitrate.InnerText);

          //string Resolution, 
          string resolution = String.Empty;
          XmlNode with = nodeDVD.SelectSingleNodeFast("fileinfo/streamdetails/video/width");
          XmlNode height = nodeDVD.SelectSingleNodeFast("fileinfo/streamdetails/audio/height");
          if (with != null && with.InnerText.Length > 0 && height != null && height.InnerText.Length > 0)
            resolution = with + "x" + height;
          WriteAntAtribute(destXml, "Resolution", resolution);

          ////string Framerate, 
          //XmlNode nodeFramerate = nodeDVD.SelectSingleNodeFast("FPS");
          //if (nodeFramerate != null && nodeFramerate.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "FPS", nodeFramerate.InnerText);

          //string Languages, 
          XmlNode nodeLanguages = nodeDVD.SelectSingleNodeFast("fileinfo/streamdetails/audio/language");
          if (nodeLanguages != null && nodeLanguages.InnerText.Length > 0)
            WriteAntAtribute(destXml, "language", nodeLanguages.InnerText);

          ////string Subtitles, 
          //XmlNode nodeSubtitles = nodeDVD.SelectSingleNodeFast("Subtitles");
          //if (nodeSubtitles != null && nodeSubtitles.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "Subtitles", nodeSubtitles.InnerText);

          ////string Size, 
          //XmlNode nodeSize = nodeDVD.SelectSingleNodeFast("Filesize");
          //if (nodeSize != null && nodeSize.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "Filesize", nodeSize.InnerText);

          ////string Disks, 
          //XmlNode nodeDisks = nodeDVD.SelectSingleNodeFast("Disks");
          //if (nodeDisks != null && nodeDisks.InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "Disks", nodeDisks.InnerText);

          ////string Picture
          //string Image = String.Empty;
          //if (nodeDVD.SelectSingleNodeFast("Cover") != null)
          //  Image = nodeDVD.SelectSingleNodeFast("Cover").InnerText;
          //WriteAntAtribute(destXml, "Cover", Image);



          //CultureInfo ci = new CultureInfo("en-us");

          //XmlNodeList DiscsList = nodeDVD.SelectNodes("Disc");
          //string wfile = string.Empty;
          //string wVideoCodec = string.Empty;
          //string wAudioCodec = string.Empty;
          //string wResolution = string.Empty;
          //string wFileSize = string.Empty;
          //string wVideobitrate = string.Empty;
          //string wAudiobitrate = string.Empty;
          //string wFramerate = string.Empty;

          //if ((DiscsList != null) && (DiscsList.Count != 0))
          //{
          //    foreach (XmlNode nodeDisc in DiscsList)
          //    {
          //        if (nodeDisc.Attributes["VideoCodec"] != null)
          //            wVideoCodec = nodeDisc.Attributes["VideoCodec"].Value;
          //        if (nodeDisc.Attributes["AudioCodec"] != null)
          //            wAudioCodec = nodeDisc.Attributes["AudioCodec"].Value;
          //        if (nodeDisc.Attributes["VideoResolution"] != null)
          //            wResolution = nodeDisc.Attributes["VideoResolution"].Value;
          //        if (nodeDisc.Attributes["VideoFilesize"] != null)
          //            if (wFileSize.Length > 0)
          //                wFileSize = wFileSize + "+" + nodeDisc.Attributes["VideoFilesize"].Value;
          //            else
          //                wFileSize = nodeDisc.Attributes["VideoFilesize"].Value;
          //        if (nodeDisc.Attributes["VideoBitrate"] != null)
          //            wVideobitrate = nodeDisc.Attributes["VideoBitrate"].Value;
          //        if (nodeDisc.Attributes["AudioBitrate"] != null)
          //            wAudiobitrate = nodeDisc.Attributes["AudioBitrate"].Value;
          //        if (nodeDisc.Attributes["VideoFramerate"] != null)
          //            wFramerate = nodeDisc.Attributes["VideoFramerate"].Value;
          //        if (nodeDisc.Attributes["VideoFullPath"] != null)
          //            if (wfile.Length > 0)
          //                wfile = wfile + ";" + nodeDisc.Attributes["VideoFullPath"].Value;
          //            else
          //                wfile = nodeDisc.Attributes["VideoFullPath"].Value;
          //    }
          //}
          //destXml.WriteStartElement("Movie");
          //string wID = nodeDVD.Attributes["Label"].Value;
          //if (wID != null && wID.Length > 3)
          //    WriteAntAtribute(destXml, "Number", wID.Substring(3));
          //else
          //    WriteAntAtribute(destXml, "Number", "9999");
          //if (nodeDVD.Attributes["OriginalTitle"] != null && nodeDVD.Attributes["OriginalTitle"].Value.Length > 0)
          //    WriteAntAtribute(destXml, "OriginalTitle", nodeDVD.Attributes["OriginalTitle"].Value);
          //else
          //    if (nodeDVD.Attributes["TranslatedTitle"] != null && nodeDVD.Attributes["TranslatedTitle"].Value.Length > 0)
          //        WriteAntAtribute(destXml, "OriginalTitle", nodeDVD.Attributes["TranslatedTitle"].Value);
          //if (nodeDVD.Attributes["TranslatedTitle"] != null && nodeDVD.Attributes["TranslatedTitle"].Value.Length > 0)
          //    WriteAntAtribute(destXml, "TranslatedTitle", nodeDVD.Attributes["TranslatedTitle"].Value);
          //else
          //    if (nodeDVD.Attributes["OriginalTitle"] != null && nodeDVD.Attributes["OriginalTitle"].Value.Length > 0)
          //        WriteAntAtribute(destXml, "TranslatedTitle", nodeDVD.Attributes["OriginalTitle"].Value);
          //if (nodeDVD.Attributes["FormattedTitle"] != null && nodeDVD.Attributes["FormattedTitle"].Value.Length > 0)
          //    WriteAntAtribute(destXml, "FormattedTitle", nodeDVD.Attributes["FormattedTitle"].Value);
          ////WriteAntAtribute(destXml, "Notes/File", File);
          //if (nodeDVD.Attributes["User1Seen"] != null)
          //    if (nodeDVD.Attributes["User1Seen"].Value.ToUpper() == "YES")
          //        WriteAntAtribute(destXml, "Checked", "true");
          //    else
          //        WriteAntAtribute(destXml, "Checked", "false");
          //DateTime dt = new DateTime();
          //if (nodeDVD.Attributes["ModifiedDate"] != null)
          //    try
          //    {
          //        dt = DateTime.Parse(nodeDVD.Attributes["ModifiedDate"].Value.ToString(), ci);
          //    }
          //    catch
          //    {
          //        dt = DateTime.Today;
          //    }
          //WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
          //if (nodeDVD.Attributes["Country"] != null)
          //    if (nodeDVD.Attributes["Country"].Value.StartsWith("|"))
          //        WriteAntAtribute(destXml, "Country", nodeDVD.Attributes["Country"].Value.Substring(1));
          //    else
          //        WriteAntAtribute(destXml, "Country", nodeDVD.Attributes["Country"].Value);
          //string wRating = "0";
          //if (nodeDVD.Attributes["Rating"] != null)
          //    wRating = nodeDVD.Attributes["Rating"].Value;
          //decimal wrating = 0;
          //try { wrating = Convert.ToDecimal(wRating, ci); }
          //catch
          //{
          //    try { wrating = Convert.ToDecimal(wRating); }
          //    catch { }
          //}
          //WriteAntAtribute(destXml, "Rating", wrating.ToString().Replace(",", "."));
          //string wYear = nodeDVD.Attributes["Year"].Value;
          //if (!string.IsNullOrEmpty(wYear))
          //    WriteAntAtribute(destXml, "Year", wYear);
          //if (nodeDVD.Attributes["RunningTime"] != null)
          //    WriteAntAtribute(destXml, "Length", nodeDVD.Attributes["RunningTime"].Value);
          //if (nodeDVD.Attributes["Genre"] != null)
          //    if (nodeDVD.Attributes["Genre"].Value.StartsWith("|"))
          //        WriteAntAtribute(destXml, "Category", nodeDVD.Attributes["Genre"].Value.Substring(1).Replace("|", ""));
          //    else
          //        WriteAntAtribute(destXml, "Category", nodeDVD.Attributes["Genre"].Value.Replace("|", ","));
          //if (nodeDVD.Attributes["Director"] != null)
          //    WriteAntAtribute(destXml, "Director", nodeDVD.Attributes["Director"].Value);
          //if (nodeDVD.Attributes["Producer"] != null)
          //    WriteAntAtribute(destXml, "Producer", nodeDVD.Attributes["Producer"].Value);
          //if (nodeDVD.Attributes["Cast"] != null)
          //    WriteAntAtribute(destXml, "Actors", nodeDVD.Attributes["Cast"].Value);
          //if (nodeDVD.Attributes["Picture"] != null)
          //    WriteAntAtribute(destXml, "Picture", nodeDVD.Attributes["Picture"].Value);
          //if (nodeDVD.Attributes["Format"] != null)
          //    WriteAntAtribute(destXml, "MediaType", nodeDVD.Attributes["Format"].Value);
          //if (nodeDVD.Attributes["Media"] != null)
          //    WriteAntAtribute(destXml, "MediaLabel", nodeDVD.Attributes["Media"].Value);
          //if (nodeDVD.Attributes["Website"] != null)
          //    WriteAntAtribute(destXml, "URL", nodeDVD.Attributes["Website"].Value);
          //if (nodeDVD.Attributes["PlotOriginal"] != null)
          //    WriteAntAtribute(destXml, "Description", nodeDVD.Attributes["PlotOriginal"].Value);
          //if (nodeDVD.Attributes["Comments"] != null)
          //    WriteAntAtribute(destXml, "Comments", nodeDVD.Attributes["Comments"].Value);
          //if (nodeDVD.Attributes["Language"] != null)
          //    if (nodeDVD.Attributes["Language"].Value.StartsWith("|"))
          //        WriteAntAtribute(destXml, "Languages", nodeDVD.Attributes["Language"].Value.Substring(1));
          //    else
          //        WriteAntAtribute(destXml, "Languages", nodeDVD.Attributes["Language"].Value);
          //if (nodeDVD.Attributes["Subtitles"] != null)
          //    WriteAntAtribute(destXml, "Subtitles", nodeDVD.Attributes["Subtitles"].Value);
          //WriteAntAtribute(destXml, "Source", wfile);
          //if (wVideoCodec.Length > 0)
          //    WriteAntAtribute(destXml, "VideoFormat", wVideoCodec);
          //if (wAudioCodec.Length > 0)
          //    WriteAntAtribute(destXml, "AudioFormat", wAudioCodec);
          //if (wResolution.Length > 0)
          //    WriteAntAtribute(destXml, "Resolution", wResolution);
          //if (wVideobitrate.Length > 0)
          //    WriteAntAtribute(destXml, "VideoBitrate", wVideobitrate.Substring(0, wVideobitrate.IndexOf(" ") - 1));
          //if (wAudiobitrate.Length > 0)
          //    WriteAntAtribute(destXml, "AudioBitrate", wAudiobitrate.Substring(0, wAudiobitrate.IndexOf(" ") - 1));
          //if (wFileSize.Length > 0)
          //    WriteAntAtribute(destXml, "Size", wFileSize);
          //WriteAntAtribute(destXml, "Disks", DiscsList.Count.ToString());
          //if (wFramerate.Length > 0)
          //    WriteAntAtribute(destXml, "Framerate", wFramerate);

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
        //LogMyFilms.Debug("XMM Importer: Writing Property '" + key + "' with Value '" + value.ToString() + "' to DB.");
      }
      else
      {
        //LogMyFilms.Debug("XMM Importer Property '" + key + "' not found in dictionary ! - Attribute not written to DB !");
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
