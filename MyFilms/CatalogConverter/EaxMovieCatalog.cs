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

namespace MyFilmsPlugin.MyFilms.CatalogConverter
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Xml;
  using System.Globalization;

  class EaxMovieCatalog
  {
    public Dictionary<string, string> ProfilerDict;

    public EaxMovieCatalog()
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
      ProfilerDict.Add("Cast", "Actors");
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
      ProfilerDict.Add("Writer", "Writer");
      ProfilerDict.Add("MPAA", "Certification");
      ProfilerDict.Add("Tags", "Tags");
      //ProfilerDict.Add("Borrower", "Borrower");
      ProfilerDict.Add("TagLine", "TagLine");
      //ProfilerDict.Add("Trailer", "SourceTrailer");
    }
    public string ConvertEaxMovieCatalog(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, bool OnlyFile, string TitleDelim)
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
        XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/EaxMovieCatalog/Catalog/Contents/Movie");
        foreach (XmlNode nodeDVD in dvdList)
        {
          string Tags = string.Empty;
          const string Tagline = "(no Tagline supported by this catalog)";
          string Certification = string.Empty;
          CultureInfo ci = new CultureInfo("en-us");
          XmlNodeList DiscsList = nodeDVD.SelectNodes("Disc");
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
          string wID = nodeDVD.Attributes["Label"].Value;
          if (wID != null && wID.Length > 3 && wID.ToLower().Contains("no:"))
            WriteAntAtribute(destXml, "Number", wID.Substring(3));
          else
            if (!string.IsNullOrEmpty(wID))
              WriteAntAtribute(destXml, "Number", wID);
            else
              WriteAntAtribute(destXml, "Number", "9999");
          if (nodeDVD.Attributes["OriginalTitle"] != null && nodeDVD.Attributes["OriginalTitle"].Value.Length > 0)
            WriteAntAtribute(destXml, "OriginalTitle", nodeDVD.Attributes["OriginalTitle"].Value);
          else
            if (nodeDVD.Attributes["TranslatedTitle"] != null && nodeDVD.Attributes["TranslatedTitle"].Value.Length > 0)
              WriteAntAtribute(destXml, "OriginalTitle", nodeDVD.Attributes["TranslatedTitle"].Value);
          if (nodeDVD.Attributes["TranslatedTitle"] != null && nodeDVD.Attributes["TranslatedTitle"].Value.Length > 0)
            WriteAntAtribute(destXml, "TranslatedTitle", nodeDVD.Attributes["TranslatedTitle"].Value);
          else
            if (nodeDVD.Attributes["OriginalTitle"] != null && nodeDVD.Attributes["OriginalTitle"].Value.Length > 0)
              WriteAntAtribute(destXml, "TranslatedTitle", nodeDVD.Attributes["OriginalTitle"].Value);
          if (nodeDVD.Attributes["FormattedTitle"] != null && nodeDVD.Attributes["FormattedTitle"].Value.Length > 0)
            WriteAntAtribute(destXml, "FormattedTitle", nodeDVD.Attributes["FormattedTitle"].Value);
          //WriteAntAtribute(destXml, "Notes/File", File);
          if (nodeDVD.Attributes["User1Seen"] != null)
            WriteAntAtribute(destXml, "Checked", nodeDVD.Attributes["User1Seen"].Value.ToUpper() == "YES" ? "true" : "false");
          DateTime dt = new DateTime();
          if (nodeDVD.Attributes["ModifiedDate"] != null)
            try
            {
              dt = DateTime.Parse(nodeDVD.Attributes["ModifiedDate"].Value);
            }
            catch
            {
              try
              {
                dt = DateTime.Parse(nodeDVD.Attributes["ModifiedDate"].Value, ci);
              }
              catch
              {
                dt = DateTime.Today;
              }
            }
          WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
          if (nodeDVD.Attributes["Country"] != null)
            WriteAntAtribute(destXml, "Country", nodeDVD.Attributes["Country"].Value.StartsWith("|") ? nodeDVD.Attributes["Country"].Value.Substring(1) : nodeDVD.Attributes["Country"].Value);
          string wRating = "0";
          if (nodeDVD.Attributes["Rating"] != null)
            wRating = nodeDVD.Attributes["Rating"].Value;
          decimal wrating = 0;
          try { wrating = Convert.ToDecimal(wRating, ci); }
          catch
          {
            try { wrating = Convert.ToDecimal(wRating); }
            catch { }
          }
          WriteAntAtribute(destXml, "Rating", wrating.ToString().Replace(",", "."));
          string wYear = nodeDVD.Attributes["Year"].Value;
          if (!string.IsNullOrEmpty(wYear))
            WriteAntAtribute(destXml, "Year", wYear);
          if (nodeDVD.Attributes["RunningTime"] != null)
            WriteAntAtribute(destXml, "Length", nodeDVD.Attributes["RunningTime"].Value);
          if (nodeDVD.Attributes["Genre"] != null)
            WriteAntAtribute(destXml, "Category", nodeDVD.Attributes["Genre"].Value.StartsWith("|") ? nodeDVD.Attributes["Genre"].Value.Substring(1).Replace("|", "") : nodeDVD.Attributes["Genre"].Value.Replace("|", ","));
          if (nodeDVD.Attributes["Director"] != null)
            WriteAntAtribute(destXml, "Director", nodeDVD.Attributes["Director"].Value);
          if (nodeDVD.Attributes["Producer"] != null)
            WriteAntAtribute(destXml, "Producer", nodeDVD.Attributes["Producer"].Value);
          string Writer = string.Empty;
          if (nodeDVD.Attributes["Writer"] != null)
            Writer = nodeDVD.Attributes["Writer"].Value;
          //WriteAntAtribute(destXml, "Writer", Writer);
          if (nodeDVD.Attributes["MPAA"] != null)
          {
            Certification = nodeDVD.Attributes["MPAA"].Value;
            //WriteAntAtribute(destXml, "MPAA", Certification);
          }
          if (nodeDVD.Attributes["Tag"] != null)
          {
            Tags = nodeDVD.Attributes["Tag"].Value;
            //WriteAntAtribute(destXml, "Tag", Tags);
          }
          if (nodeDVD.Attributes["Cast"] != null)
            WriteAntAtribute(destXml, "Cast", nodeDVD.Attributes["Cast"].Value);
          if (nodeDVD.Attributes["Picture"] != null)
            WriteAntAtribute(destXml, "Picture", nodeDVD.Attributes["Picture"].Value);
          if (nodeDVD.Attributes["Format"] != null)
            WriteAntAtribute(destXml, "MediaType", nodeDVD.Attributes["Format"].Value);
          if (nodeDVD.Attributes["Media"] != null)
            WriteAntAtribute(destXml, "MediaLabel", nodeDVD.Attributes["Media"].Value);
          if (nodeDVD.Attributes["Website"] != null)
            WriteAntAtribute(destXml, "URL", nodeDVD.Attributes["Website"].Value);

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
          if (nodeDVD.Attributes["PlotOriginal"] != null && !string.IsNullOrEmpty(nodeDVD.Attributes["PlotOriginal"].Value))
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += nodeDVD.Attributes["PlotOriginal"].Value;
          }
          WriteAntAtribute(destXml, "Description", DescriptionMerged);

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
          if (nodeDVD.Attributes["Comments"] != null && !string.IsNullOrEmpty(nodeDVD.Attributes["Comments"].Value))
          {
            if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
            CommentsMerged += nodeDVD.Attributes["Comments"].Value;
          }
          WriteAntAtribute(destXml, "Comments", CommentsMerged);

          if (nodeDVD.Attributes["Language"] != null)
            WriteAntAtribute(destXml, "Languages", nodeDVD.Attributes["Language"].Value.StartsWith("|") ? nodeDVD.Attributes["Language"].Value.Substring(1) : nodeDVD.Attributes["Language"].Value);
          if (nodeDVD.Attributes["Subtitles"] != null)
            WriteAntAtribute(destXml, "Subtitles", nodeDVD.Attributes["Subtitles"].Value);
          WriteAntAtribute(destXml, "Source", wfile);
          if (wVideoCodec.Length > 0)
            WriteAntAtribute(destXml, "VideoFormat", wVideoCodec);
          if (wAudioCodec.Length > 0)
            WriteAntAtribute(destXml, "AudioFormat", wAudioCodec);
          if (wResolution.Length > 0)
            WriteAntAtribute(destXml, "Resolution", wResolution);
          if (wVideobitrate.Length > 0)
            WriteAntAtribute(destXml, "VideoBitrate", wVideobitrate.Substring(0, wVideobitrate.IndexOf("K") - 1).Trim().Replace(" ", ""));
          if (wAudiobitrate.Length > 0)
            WriteAntAtribute(destXml, "AudioBitrate", wAudiobitrate.Substring(0, wAudiobitrate.IndexOf("K") - 1).Trim().Replace(" ", ""));
          if (wFileSize.Length > 0)
            WriteAntAtribute(destXml, "Size", wFileSize);
          WriteAntAtribute(destXml, "Disks", DiscsList.Count.ToString());
          if (wFramerate.Length > 0)
            WriteAntAtribute(destXml, "Framerate", wFramerate);


          // Now writing MF extended attributes
          WriteAntElement(destXml, "MPAA", Certification);
          WriteAntElement(destXml, "TagLine", Tagline);
          WriteAntElement(destXml, "Tags", Tags);
          WriteAntElement(destXml, "Writer", Writer);

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
