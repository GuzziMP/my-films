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
using MediaPortal.Util;

namespace MyFilmsPlugin.CatalogConverter
{
  class MovingPicturesXML
  {
    public Dictionary<string, string> ProfilerDict;

    public MovingPicturesXML()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("ID", "Number");
      ProfilerDict.Add("Checked", "Checked");
      ProfilerDict.Add("MediaLabel", "MediaLabel");
      ProfilerDict.Add("VIDEOFORMAT", "MediaType");
      ProfilerDict.Add("FULLPATH", "Source");
      ProfilerDict.Add("DATE_ADDED", "Date");
      ProfilerDict.Add("Loaner", "Borrower");
      ProfilerDict.Add("SCORE", "Rating");
      ProfilerDict.Add("TITLE", "OriginalTitle");
      ProfilerDict.Add("ALTERNATE_TITLES", "TranslatedTitle");
      ProfilerDict.Add("SORTBY", "FormattedTitle");
      ProfilerDict.Add("DIRECTORS", "Director");
      ProfilerDict.Add("Producer", "Producer");
      ProfilerDict.Add("Country", "Country");
      ProfilerDict.Add("GENRES", "Category");
      ProfilerDict.Add("YEAR", "Year");
      ProfilerDict.Add("RUNTIME", "Length");
      ProfilerDict.Add("ACTORS", "Actors");
      ProfilerDict.Add("DETAILS_URL", "URL");
      ProfilerDict.Add("SUMMARY", "Description");
      ProfilerDict.Add("Comments", "Comments");
      ProfilerDict.Add("VIDEOCODEC", "VideoFormat");
      ProfilerDict.Add("Bitrate", "VideoBitrate");
      ProfilerDict.Add("AUDIOCODEC", "AudioFormat");
      ProfilerDict.Add("AudioBitRate", "AudioBitrate");
      ProfilerDict.Add("VIDEORESOLUTION", "Resolution");
      ProfilerDict.Add("FPS", "Framerate");
      ProfilerDict.Add("LANGUAGE", "Languages");
      ProfilerDict.Add("HASSUBTITLES", "Subtitles");
      ProfilerDict.Add("Filesize", "Size");
      ProfilerDict.Add("Disks", "Disks");
      ProfilerDict.Add("COVERFULLPATH", "Picture");
      ProfilerDict.Add("WRITERS", "Writer");
      ProfilerDict.Add("CERTIFICATION", "Certification");
      ProfilerDict.Add("TAGLINE", "TagLine");
      ProfilerDict.Add("Tags", "Tags");
      ProfilerDict.Add("Trailer", "SourceTrailer");
      ProfilerDict.Add("BACKDROPFULLPATH", "Fanart");

      // POPULARITY - not mapped
      //ProfilerDict.Add("watched", "Watched");
      //ProfilerDict.Add("watcheddate", "WatchedDate");
      ProfilerDict.Add("IMDB_ID", "IMDB_Id");
      //ProfilerDict.Add("TMDB_Id", "TMDB_Id");

      //int Number, 
      //string Checked, 
      //string MediaLabel, 
      //string MediaType, 
      //string Source, 
      //string Date, 
      //string Borrower, 
      //decimal Rating, 
      //string OriginalTitle, 
      //string TranslatedTitle, 
      //string FormattedTitle, 
      //string Director, 
      //string Producer, 
      //string Country, 
      //string Category, 
      //string Year, 
      //string Length, 
      //string Actors, 
      //string URL, 
      //string Description, 
      //string Comments, 
      //string VideoFormat, 
      //string VideoBitrate, 
      //string AudioFormat, 
      //string AudioBitrate, 
      //string Resolution, 
      //string Framerate, 
      //string Languages, 
      //string Subtitles, 
      //string Size, 
      //string Disks, 
      //string Picture
    }
    public string ConvertMovingPicturesXML(string source, string folderimage, string destinationTagline, string destinationTags, string destinationCertification, string destinationWriter, bool onlyFile)
    {
      string wStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = wStrPath + "\\" +
                        source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) +
                        "_tmp.xml";

      XmlWriterSettings settings = new XmlWriterSettings
      {
        Indent = true,
        Encoding = Encoding.Default,
        NewLineOnAttributes = true,
        IndentChars = ("    ")
      };
      //          ' indent the output and insert line breaks
      //' start each attribute on a new line
      settings.NewLineOnAttributes = false;
      //settings.NewLineChars = ControlChars.CrLf & ControlChars.CrLf; //' use two Return characters instead of one
      //XmlWriter destXml = XmlWriter.Create(destFile, settings);

      using (XmlWriter destXml = XmlWriter.Create(destFile, settings))
      {
        // Write XML data.

        //XmlTextWriter destXml = new XmlTextWriter(destFile, Encoding.Default);
        //destXml.Formatting = Formatting.Indented;
        destXml.WriteStartDocument();
        destXml.WriteStartElement("AntMovieCatalog");
        destXml.WriteStartElement("Catalog");
        destXml.WriteElementString("Properties", string.Empty);
        destXml.WriteStartElement("Contents");
        try
        {
          XmlDocument doc = new XmlDocument();
          doc.Load(source);
          XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/ROOT/MOVIES/MOVIE");
          foreach (XmlNode nodeDVD in dvdList)
          {
            //HtmlUtil htmlUtil = new HtmlUtil();

            destXml.WriteStartElement("Movie");

            //int Number, 
            XmlNode nodeID = nodeDVD.SelectSingleNodeFast("ID");
            //XmlNode nodeNumber = nodeDVD.SelectSingleNodeFast("MovieID");
            if (nodeID != null && !string.IsNullOrEmpty(nodeID.InnerText)) WriteAntAtribute(destXml, "ID", nodeID.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            else WriteAntAtribute(destXml, "ID", "99999");

            //string Checked, 
            //XmlNode nodeChecked = nodeDVD.SelectSingleNodeFast("Seen");
            //if (nodeChecked != null && nodeChecked.InnerText.Length > 0) WriteAntAtribute(destXml, "Seen", nodeChecked.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            WriteAntAtribute(destXml, "Checked", "false");

            //string MediaLabel, 
            XmlNode nodeMediaLabel = nodeDVD.SelectSingleNodeFast("VIDEOFORMAT");
            if (nodeMediaLabel != null && nodeMediaLabel.InnerText.Length > 0) WriteAntAtribute(destXml, "VIDEOFORMAT", nodeMediaLabel.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string MediaType, 
            XmlNode nodeMediaType = nodeDVD.SelectSingleNodeFast("Media");
            if (nodeMediaType != null && nodeMediaType.InnerText.Length > 0) WriteAntAtribute(destXml, "Media", nodeMediaType.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Source, 
            string strSource = String.Empty;
            XmlNode nodeSource = nodeDVD.SelectSingleNodeFast("FULLPATH");
            if (nodeSource != null && nodeSource.InnerText.Length > 0)
              strSource += nodeSource.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            strSource = strSource.Replace("<br/>", "; ");
            WriteAntAtribute(destXml, "FULLPATH", strSource);

            //string Trailer, 
            string strTrailer = String.Empty;
            for (int i = 1; i < 3; i++)
            {
              XmlNode nodeTrailer = nodeDVD.SelectSingleNodeFast("TrailerFile" + i.ToString());
              if (strTrailer.Length > 0 && nodeTrailer.InnerText.Length > 0) strTrailer += ";";
              if (nodeTrailer != null && nodeTrailer.InnerText.Length > 0) strTrailer += nodeTrailer.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            }
            //WriteAntAtribute(destXml, "Trailer", strTrailer);

            //string Date, 
            XmlNode nodeDate = nodeDVD.SelectSingleNodeFast("DATE_ADDED");
            string strDateAdded = string.Empty;
            IFormatProvider culture = new CultureInfo("en-US", true);
            if (nodeDate != null && nodeDate.InnerText.Length > 0)
            {
              strDateAdded = nodeDate.InnerText.Replace(char.ConvertFromUtf32(160), " ");
              if (strDateAdded.Contains(" "))
                strDateAdded = strDateAdded.Substring(0, strDateAdded.IndexOf(" ")); // Remove time...
            }
            try
            {
              DateTime dt = new DateTime();
              dt = DateTime.ParseExact(strDateAdded, "d.M.yyyy", System.Globalization.CultureInfo.InvariantCulture);
              WriteAntAtribute(destXml, "DATE_ADDED", dt.ToShortDateString());
            }
            catch
            {
              try
              {
                DateTime dt = new DateTime();
                dt = DateTime.Parse(strDateAdded);
                //dt = DateTime.Parse(strDateAdded, culture, DateTimeStyles.NoCurrentDateDefault);
                //dt = DateTime.ParseExact(strDateAdded, "MM/dd/yyyy hh:mm:ss tt", culture, DateTimeStyles.NoCurrentDateDefault);
                WriteAntAtribute(destXml, "DATE_ADDED", dt.ToShortDateString());
              }
              catch (Exception)
              {
                DateTime dt = new DateTime();
                dt = DateTime.Parse(strDateAdded, culture, DateTimeStyles.NoCurrentDateDefault);
                WriteAntAtribute(destXml, "DATE_ADDED", dt.ToShortDateString());
              }
            }

            //string Borrower, 

            //decimal Rating, 
            string Rating = string.Empty;
            decimal wrating = 0;
            CultureInfo ci = new CultureInfo("en-us");
            XmlNode nodeRating = nodeDVD.SelectSingleNodeFast("SCORE");
            if (nodeRating != null && nodeRating.InnerText != null)
            {
              try
              {
                wrating = Convert.ToDecimal(nodeRating.InnerText, ci);
              }
              catch
              {
                try
                {
                  wrating = Convert.ToDecimal(nodeRating.InnerText);
                }
                catch
                {
                }
              }
            }
            Rating = wrating.ToString("0.0", ci);
            //Rating = wrating.ToString().Replace(",", "."));
            WriteAntAtribute(destXml, "SCORE", Rating);

            //string OriginalTitle, 
            //string TranslatedTitle, 
            //string FormattedTitle, 
            XmlNode nodeOTitle = nodeDVD.SelectSingleNodeFast("TITLE");
            XmlNode nodeTitle = nodeDVD.SelectSingleNodeFast("ALTERNATE_TITLES");
            XmlNode nodeSTitle = nodeDVD.SelectSingleNodeFast("SORTBY");
            if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0) WriteAntAtribute(destXml, "TITLE", nodeOTitle.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }));
            else WriteAntAtribute(destXml, "TITLE", nodeTitle.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ", "));
            WriteAntAtribute(destXml, "ALTERNATE_TITLES", nodeTitle.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }));
            if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0) WriteAntAtribute(destXml, "SORTBY", nodeSTitle.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            else WriteAntAtribute(destXml, "SORTBY", nodeTitle.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }));

            //string Certification
            string Certification = string.Empty;
            XmlNode nodeCertification = nodeDVD.SelectSingleNodeFast("CERTIFICATION");
            if (nodeCertification != null && nodeCertification.InnerText.Length > 0) Certification = nodeCertification.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //WriteAntAtribute(destXml, "CERTIFICATION", Certification);

            //string TagLine
            XmlNode nodeTagLine = nodeDVD.SelectSingleNodeFast("TAGLINE");
            string Tagline = string.Empty;
            if (nodeTagLine != null && nodeTagLine.InnerText.Length > 0) Tagline = nodeTagLine.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //WriteAntAtribute(destXml, "TAGLINE", Tagline);

            //string Tags
            string Tags = string.Empty;
            XmlNode nodeTags = nodeDVD.SelectSingleNodeFast("Category");
            if (nodeTags != null && nodeTags.InnerText.Length > 0) Tags = nodeTags.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ", ");
            //WriteAntAtribute(destXml, "Tags", Tags);

            //string Borrower
            XmlNode nodeBorrower = nodeDVD.SelectSingleNodeFast("Loaner");
            if (nodeBorrower != null && nodeBorrower.InnerText.Length > 0) WriteAntAtribute(destXml, "Loaner", nodeBorrower.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Director, 
            XmlNode nodeDirector = nodeDVD.SelectSingleNodeFast("DIRECTORS");
            if (nodeDirector != null && nodeDirector.InnerText.Length > 0) WriteAntAtribute(destXml, "DIRECTORS", nodeDirector.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ", "));

            //string Producer, 
            XmlNode nodeProducer = nodeDVD.SelectSingleNodeFast("Producer");
            if (nodeProducer != null && nodeProducer.InnerText.Length > 0) WriteAntAtribute(destXml, "Producer", nodeProducer.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ", "));

            //string Writer, 
            string Writer = string.Empty;
            XmlNode nodeWriter = nodeDVD.SelectSingleNodeFast("WRITERS");
            if (nodeWriter != null && nodeWriter.InnerText.Length > 0)
              Writer = nodeWriter.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ",");
            //WriteAntAtribute(destXml, "WRITERS", Writer);

            //string Country, 
            XmlNode nodeCountry = nodeDVD.SelectSingleNodeFast("Country");
            if (nodeCountry != null && nodeCountry.InnerText.Length > 0) WriteAntAtribute(destXml, "Country", nodeCountry.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ","));

            //string Category, 
            XmlNode nodeGenre = nodeDVD.SelectSingleNodeFast("GENRES");
            if (nodeGenre != null && nodeGenre.InnerText.Length > 0) WriteAntAtribute(destXml, "GENRES", nodeGenre.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ", "));

            //string Year, 
            XmlNode nodeYear = nodeDVD.SelectSingleNodeFast("YEAR");
            if (nodeYear != null) WriteAntAtribute(destXml, "YEAR", nodeYear.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Length, 
            int strLengthnum = 0;
            XmlNode nodeDuration = nodeDVD.SelectSingleNodeFast("RUNTIME");

            try
            {
              strLengthnum = Int32.Parse(nodeDuration.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            }
            catch
            {
              strLengthnum = 0;
            }
            if (nodeDuration != null && nodeDuration.InnerText.Length > 0) WriteAntAtribute(destXml, "RUNTIME", strLengthnum.ToString());

            //string Actors, ACTORS
            XmlNode nodeActors = nodeDVD.SelectSingleNodeFast("ACTORS");
            if (nodeActors != null && nodeActors.InnerText.Length > 0)
              WriteAntAtribute(destXml, "ACTORS", nodeActors.InnerText.Replace(char.ConvertFromUtf32(160), " ").Trim(new Char[] { '|' }).Replace("|", ", "));

            //string Actor = String.Empty;
            //XmlNodeList creditsList = nodeDVD.SelectNodes("Actors/Actor");
            //foreach (XmlNode nodeCredit in creditsList)
            //{
            //  string line = String.Empty;
            //  if (nodeCredit.SelectSingleNodeFast("ActorName") != null) line = nodeCredit.SelectSingleNodeFast("ActorName").InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //  if ((nodeCredit.SelectSingleNodeFast("ActorRole") != null) &&
            //      (nodeCredit.SelectSingleNodeFast("ActorRole").InnerText.Length > 0)) line += " (" + nodeCredit.SelectSingleNodeFast("ActorRole").InnerText.Replace(char.ConvertFromUtf32(160), " ") + ")";
            //  if (line.Length > 0)
            //  {
            //    if (Actor.Length > 0) Actor += ", ";
            //    Actor += line;
            //  }
            //}
            //WriteAntAtribute(destXml, "Actors", Actor);

            //string URL, 
            string strURL = String.Empty;
            XmlNode nodeURL = nodeDVD.SelectSingleNodeFast("DETAILS_URL");
            if (nodeURL != null && nodeURL.InnerText != null) strURL = nodeURL.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            WriteAntAtribute(destXml, "DETAILS_URL", strURL);

            //string URL, 
            string strIMDB = String.Empty;
            XmlNode nodeIMDB = nodeDVD.SelectSingleNodeFast("IMDB_ID");
            if (nodeIMDB != null && nodeIMDB.InnerText != null)
              strIMDB = nodeIMDB.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //WriteAntAtribute(destXml, "IMDB_ID", strURL);

            //string Description, 
            XmlNode nodePlot = nodeDVD.SelectSingleNodeFast("SUMMARY");
            string DescriptionMerged = string.Empty;
            if (destinationTagline == "Description")
            {
              if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
              DescriptionMerged += Tagline;
            }
            if (destinationTags == "Description")
            {
              if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
              DescriptionMerged += Tags;
            }
            if (destinationCertification == "Description")
            {
              if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
              DescriptionMerged += Certification;
            }
            if (nodePlot != null && nodePlot.InnerText != null)
            {
              if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
              DescriptionMerged += nodePlot.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            }
            WriteAntAtribute(destXml, "SUMMARY", DescriptionMerged);

            //string Comments, 
            XmlNode nodeComments = nodeDVD.SelectSingleNodeFast("Comments");
            string CommentsMerged = string.Empty;
            if (destinationTagline == "Comments")
            {
              if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
              CommentsMerged += Tagline;
            }
            if (destinationTags == "Comments")
            {
              if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
              CommentsMerged += Tags;
            }
            if (destinationCertification == "Comments")
            {
              if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
              CommentsMerged += Certification;
            }
            if (nodeComments != null && nodeComments.InnerText != null)
            {
              if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
              CommentsMerged += nodeComments.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            }
            WriteAntAtribute(destXml, "Comments", CommentsMerged);

            //string VideoFormat, Codec
            XmlNode nodeVideoFormat = nodeDVD.SelectSingleNodeFast("VIDEOCODEC");
            if (nodeVideoFormat != null && nodeVideoFormat.InnerText.Length > 0) WriteAntAtribute(destXml, "VIDEOCODEC", nodeVideoFormat.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string VideoBitrate, Bitrate 
            XmlNode nodeVideoBitrate = nodeDVD.SelectSingleNodeFast("Bitrate");
            if (nodeVideoBitrate != null && nodeVideoBitrate.InnerText.Length > 0) WriteAntAtribute(destXml, "Bitrate", nodeVideoBitrate.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string AudioFormat, 
            XmlNode nodeAudioFormat = nodeDVD.SelectSingleNodeFast("AUDIOCODEC");
            if (nodeAudioFormat != null && nodeAudioFormat.InnerText.Length > 0) WriteAntAtribute(destXml, "AUDIOCODEC", nodeAudioFormat.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string AudioBitrate, 
            XmlNode nodeAudioBitrate = nodeDVD.SelectSingleNodeFast("AudioBitRate");
            if (nodeAudioBitrate != null && nodeAudioBitrate.InnerText.Length > 0) WriteAntAtribute(destXml, "AudioBitRate", nodeAudioBitrate.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Resolution, 
            XmlNode nodeResolution = nodeDVD.SelectSingleNodeFast("VIDEORESOLUTION");
            if (nodeResolution != null && nodeResolution.InnerText.Length > 0) WriteAntAtribute(destXml, "VIDEORESOLUTION", nodeResolution.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Framerate, 
            XmlNode nodeFramerate = nodeDVD.SelectSingleNodeFast("FPS");
            if (nodeFramerate != null && nodeFramerate.InnerText.Length > 0) WriteAntAtribute(destXml, "FPS", nodeFramerate.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Languages, 
            XmlNode nodeLanguages = nodeDVD.SelectSingleNodeFast("LANGUAGE");
            if (nodeLanguages != null && nodeLanguages.InnerText.Length > 0) WriteAntAtribute(destXml, "LANGUAGE", nodeLanguages.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Subtitles, 
            XmlNode nodeSubtitles = nodeDVD.SelectSingleNodeFast("HASSUBTITLES");
            if (nodeSubtitles != null && nodeSubtitles.InnerText.Length > 0) WriteAntAtribute(destXml, "HASSUBTITLES", nodeSubtitles.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Size, 
            XmlNode nodeSize = nodeDVD.SelectSingleNodeFast("Filesize");
            if (nodeSize != null && nodeSize.InnerText.Length > 0) WriteAntAtribute(destXml, "Filesize", nodeSize.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Disks, 
            XmlNode nodeDisks = nodeDVD.SelectSingleNodeFast("Disks");
            if (nodeDisks != null && nodeDisks.InnerText.Length > 0) WriteAntAtribute(destXml, "Disks", nodeDisks.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Picture
            XmlNode nodePicture = nodeDVD.SelectSingleNodeFast("COVERFULLPATH");
            if (nodePicture != null && nodePicture.InnerText.Length > 0)
              WriteAntAtribute(destXml, "COVERFULLPATH", nodePicture.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            //string Image = String.Empty;
            //if (nodeDVD.SelectSingleNodeFast("COVERFULLPATH") != null && nodeDVD.SelectSingleNodeFast("COVERFULLPATH").InnerText.Length > 0)
            //Image = nodeDVD.SelectSingleNodeFast("COVERFULLPATH").InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //Image = Image.Substring(0, Image.IndexOf("|"));
            //WriteAntAtribute(destXml, "COVERFULLPATH", Image);

            //string Fanart
            string Fanart = String.Empty;
            XmlNode nodeFanart = nodeDVD.SelectSingleNodeFast("BACKDROPFULLPATH");
            if (nodeFanart != null && nodeFanart.InnerText.Length > 0)
              Fanart = nodeFanart.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //WriteAntAtribute(destXml, "BACKDROPFULLPATH", Fanart);

            // Now writing MF extended attributes
            WriteAntElement(destXml, "IMDB_ID", strIMDB);
            WriteAntElement(destXml, "Trailer", strTrailer);
            WriteAntElement(destXml, "CERTIFICATION", Certification);
            WriteAntElement(destXml, "TAGLINE", Tagline);
            WriteAntElement(destXml, "Tags", Tags);
            WriteAntElement(destXml, "WRITERS", Writer);
            WriteAntElement(destXml, "BACKDROPFULLPATH", Fanart);

            destXml.WriteEndElement();
          }

        }
        catch (Exception)
        {
          //LogMyFilms.Debug("XMM Importer: Failed to import Catalog: " + ex.Message);
          return string.Empty;
        }
        destXml.WriteEndElement();
        destXml.WriteEndElement();
        destXml.Flush();
        destXml.Close();
        return destFile;
      }

    }

    private void WriteAntAtribute(XmlWriter tw, string key, string value)
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
