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

  using MediaPortal.Util;

  class XMM
  {
    public Dictionary<string, string> ProfilerDict;

    public XMM()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("MovieID", "Number");
      ProfilerDict.Add("Seen", "Checked");
      ProfilerDict.Add("MediaLabel", "MediaLabel");
      ProfilerDict.Add("Media", "MediaType");
      ProfilerDict.Add("MovieFile", "Source");
      ProfilerDict.Add("DateInsert", "Date");
      ProfilerDict.Add("Loaner", "Borrower");
      ProfilerDict.Add("Rating", "Rating");
      ProfilerDict.Add("Title", "OriginalTitle");
      ProfilerDict.Add("TTitle", "TranslatedTitle");
      ProfilerDict.Add("STitle", "FormattedTitle");
      ProfilerDict.Add("Director", "Director");
      ProfilerDict.Add("Producer", "Producer");
      ProfilerDict.Add("Country", "Country");
      ProfilerDict.Add("Genre", "Category");
      ProfilerDict.Add("Year", "Year");
      ProfilerDict.Add("Length", "Length");
      ProfilerDict.Add("Actors", "Actors");
      ProfilerDict.Add("WebLinkScript", "URL");
      ProfilerDict.Add("Plot", "Description");
      ProfilerDict.Add("Comments", "Comments");
      ProfilerDict.Add("Codec", "VideoFormat");
      ProfilerDict.Add("Bitrate", "VideoBitrate");
      ProfilerDict.Add("AudioCodec", "AudioFormat");
      ProfilerDict.Add("AudioBitRate", "AudioBitrate");
      ProfilerDict.Add("Resolution", "Resolution");
      ProfilerDict.Add("FPS", "Framerate");
      ProfilerDict.Add("OriginalLanguage", "Languages");
      ProfilerDict.Add("Subtitles", "Subtitles");
      ProfilerDict.Add("Filesize", "Size");
      ProfilerDict.Add("Disks", "Disks");
      ProfilerDict.Add("Cover", "Picture");
      ProfilerDict.Add("Fanart", "Fanart");
      ProfilerDict.Add("Writer", "Writer");
      ProfilerDict.Add("MPAA", "Certification");
      ProfilerDict.Add("TagLine", "TagLine");
      ProfilerDict.Add("Tags", "Tags");
      ProfilerDict.Add("Trailer", "SourceTrailer");
      //ProfilerDict.Add("watched", "Watched");
      //ProfilerDict.Add("watcheddate", "WatchedDate");
      //ProfilerDict.Add("IMDB_Id", "IMDB_Id");
      //ProfilerDict.Add("TMDB_Id", "TMDB_Id");

      //case "Aspectratio":
      //case "RatingUser":
      //case "Fanart":
      //case "Studio":
      //case "IMDB_Rank":
      //case "IsOnline":
      //case "Edition":
      //case "IsOnlineTrailer":


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
    public string ConvertXMM(string source, string folderimage, string destinationTagline, string destinationTags, string destinationCertification, string destinationWriter, bool onlyFile)
    {
      string WStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = WStrPath + "\\" +
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
          XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/XMM_Movie_Database/Movie");
          foreach (XmlNode nodeDVD in dvdList)
          {
            // HtmlUtil htmlUtil = new HtmlUtil();

            destXml.WriteStartElement("Movie");

            //int Number, 
            XmlNode nodeID = nodeDVD.SelectSingleNodeFast("ID");
            XmlNode nodeNumber = nodeDVD.SelectSingleNodeFast("MovieID");
            if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.InnerText)) WriteAntAtribute(destXml, "MovieID", nodeNumber.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            else WriteAntAtribute(destXml, "MovieID", "9999");

            //string Checked, 
            XmlNode nodeChecked = nodeDVD.SelectSingleNodeFast("Seen");
            if (nodeChecked != null && nodeChecked.InnerText.Length > 0) WriteAntAtribute(destXml, "Seen", nodeChecked.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string MediaLabel, 
            XmlNode nodeMediaLabel = nodeDVD.SelectSingleNodeFast("MediaLabel");
            if (nodeMediaLabel != null && nodeMediaLabel.InnerText.Length > 0) WriteAntAtribute(destXml, "MediaLabel", nodeMediaLabel.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string MediaType, 
            XmlNode nodeMediaType = nodeDVD.SelectSingleNodeFast("Media");
            if (nodeMediaType != null && nodeMediaType.InnerText.Length > 0) WriteAntAtribute(destXml, "Media", nodeMediaType.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Source, 
            string strSource = String.Empty;
            for (int i = 1; i < 7; i++)
            {
              XmlNode nodeSource = nodeDVD.SelectSingleNodeFast("MovieFile" + i.ToString());
              if (strSource.Length > 0 && nodeSource.InnerText.Length > 0) strSource += ";";
              if (nodeSource != null && nodeSource.InnerText.Length > 0) strSource += nodeSource.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            }
            WriteAntAtribute(destXml, "MovieFile", strSource);

            //string Trailer, 
            string strTrailer = String.Empty;
            for (int i = 1; i < 3; i++)
            {
              XmlNode nodeTrailer = nodeDVD.SelectSingleNodeFast("TrailerFile" + i.ToString());
              if (strTrailer.Length > 0 && nodeTrailer.InnerText.Length > 0) strTrailer += ";";
              if (nodeTrailer != null && nodeTrailer.InnerText.Length > 0) strTrailer += nodeTrailer.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            }
            WriteAntAtribute(destXml, "Trailer", strTrailer);

            //string Date, 
            XmlNode nodeDate = nodeDVD.SelectSingleNodeFast("DateInsert");
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
              WriteAntAtribute(destXml, "DateInsert", dt.ToShortDateString());
            }
            catch
            {
              try
              {
                DateTime dt = new DateTime();
                dt = DateTime.Parse(strDateAdded);
                //dt = DateTime.Parse(strDateAdded, culture, DateTimeStyles.NoCurrentDateDefault);
                //dt = DateTime.ParseExact(strDateAdded, "MM/dd/yyyy hh:mm:ss tt", culture, DateTimeStyles.NoCurrentDateDefault);
                WriteAntAtribute(destXml, "DateInsert", dt.ToShortDateString());
              }
              catch (Exception)
              {
                DateTime dt = new DateTime();
                dt = DateTime.Parse(strDateAdded, culture, DateTimeStyles.NoCurrentDateDefault);
                WriteAntAtribute(destXml, "DateInsert", dt.ToShortDateString());
              }
            }

            //string Borrower, 

            //decimal Rating, 
            string Rating = string.Empty;
            decimal wrating = 0;
            CultureInfo ci = new CultureInfo("en-us");
            XmlNode nodeRating = nodeDVD.SelectSingleNodeFast("Rating");
            if (nodeRating != null && nodeRating.InnerText != null)
            {
              try
              {
                wrating = Convert.ToDecimal(nodeRating.InnerText.Replace(char.ConvertFromUtf32(160), " "));
              }
              catch
              {
                try
                {
                  wrating = Convert.ToDecimal(nodeRating.InnerText.Replace(char.ConvertFromUtf32(160), " "), ci);
                }
                catch
                {
                }
              }
            }
            Rating = wrating.ToString("0.0", ci);
            WriteAntAtribute(destXml, "Rating", Rating);

            //string OriginalTitle, 
            //string TranslatedTitle, 
            //string FormattedTitle, 
            XmlNode nodeOTitle = nodeDVD.SelectSingleNodeFast("OriginalTitle");
            XmlNode nodeTitle = nodeDVD.SelectSingleNodeFast("Title");
            XmlNode nodeSTitle = nodeDVD.SelectSingleNodeFast("SortTitle");
            if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0) WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            else WriteAntAtribute(destXml, "Title", nodeTitle.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0) WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            else WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Certification
            string Certification = string.Empty;
            XmlNode nodeCertification = nodeDVD.SelectSingleNodeFast("MPAA");
            if (nodeCertification != null && nodeCertification.InnerText.Length > 0) Certification = nodeCertification.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //WriteAntAtribute(destXml, "MPAA", Certification);

            //string TagLine
            XmlNode nodeTagLine = nodeDVD.SelectSingleNodeFast("TagLine");
            string Tagline = string.Empty;
            if (nodeTagLine != null && nodeTagLine.InnerText.Length > 0) Tagline = nodeTagLine.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //WriteAntAtribute(destXml, "TagLine", Tagline);

            //string Tags
            string Tags = string.Empty;
            XmlNode nodeTags = nodeDVD.SelectSingleNodeFast("Category");
            if (nodeTags != null && nodeTags.InnerText.Length > 0) Tags = nodeTags.InnerText.Replace(char.ConvertFromUtf32(160), " ").Replace("|", ",");
            //WriteAntAtribute(destXml, "Tags", Tags);

            //string Borrower
            XmlNode nodeBorrower = nodeDVD.SelectSingleNodeFast("Loaner");
            if (nodeBorrower != null && nodeBorrower.InnerText.Length > 0) WriteAntAtribute(destXml, "Loaner", nodeBorrower.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Director, 
            XmlNode nodeDirector = nodeDVD.SelectSingleNodeFast("Director");
            if (nodeDirector != null && nodeDirector.InnerText.Length > 0) WriteAntAtribute(destXml, "Director", nodeDirector.InnerText.Replace(char.ConvertFromUtf32(160), " ").Replace("|", ","));

            //string Producer, 
            XmlNode nodeProducer = nodeDVD.SelectSingleNodeFast("Producer");
            if (nodeProducer != null && nodeProducer.InnerText.Length > 0) WriteAntAtribute(destXml, "Producer", nodeProducer.InnerText.Replace(char.ConvertFromUtf32(160), " ").Replace("|", ","));

            //string Writer, 
            string Writer = string.Empty;
            XmlNode nodeWriter = nodeDVD.SelectSingleNodeFast("Writer");
            if (nodeWriter != null && nodeWriter.InnerText.Length > 0) Writer = nodeWriter.InnerText.Replace(char.ConvertFromUtf32(160), " ").Replace("|", ",");
            //WriteAntAtribute(destXml, "Writer", Writer);

            //string Country, 
            XmlNode nodeCountry = nodeDVD.SelectSingleNodeFast("Country");
            if (nodeCountry != null && nodeCountry.InnerText.Length > 0) WriteAntAtribute(destXml, "Country", nodeCountry.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Category, 
            string genre = String.Empty;
            XmlNode genreGenre = nodeDVD.SelectSingleNodeFast("Genre");
            XmlNode genreSubgenre = nodeDVD.SelectSingleNodeFast("Subgenre");
            XmlNode genreCategory = nodeDVD.SelectSingleNodeFast("Category");
            if (genreGenre != null && genreGenre.InnerText.Length > 0) genre = genreGenre.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            if (genreSubgenre != null && genreSubgenre.InnerText.Length > 0)
            {
              foreach (string subgenre in genreSubgenre.InnerText.Replace(char.ConvertFromUtf32(160), " ").Split(new Char[] { '/' }))
              {
                if (genre.Length > 0) genre += ", ";
                genre += subgenre;
              }
            }
            if (genre.Length == 0 && genreCategory != null && genreCategory.InnerText.Length > 0) genre = genreCategory.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            //XmlNodeList genreList = nodeDVD.SelectNodes("Genres/Genre");
            //foreach (XmlNode nodeGenre in genreList)
            //{
            //    if (genre.Length > 0) genre += ", ";
            //    genre += nodeGenre.InnerText;
            //}
            WriteAntAtribute(destXml, "Genre", genre);

            //string Year, 
            XmlNode nodeYear = nodeDVD.SelectSingleNodeFast("Year");
            if (nodeYear != null) WriteAntAtribute(destXml, "Year", nodeYear.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Length, 
            int strLengthnum = 0;
            XmlNode nodeDuration = nodeDVD.SelectSingleNodeFast("Length");

            try
            {
              strLengthnum = Int32.Parse(nodeDuration.InnerText.Replace(char.ConvertFromUtf32(160), " "));
            }
            catch
            {
              strLengthnum = 0;
            }
            if (nodeDuration != null && nodeDuration.InnerText.Length > 0) WriteAntAtribute(destXml, "Length", strLengthnum.ToString());

            //string Actors, 
            string Actor = String.Empty;
            XmlNodeList creditsList = nodeDVD.SelectNodes("Actors/Actor");
            foreach (XmlNode nodeCredit in creditsList)
            {
              string line = String.Empty;
              if (nodeCredit.SelectSingleNodeFast("ActorName") != null) line = nodeCredit.SelectSingleNodeFast("ActorName").InnerText.Replace(char.ConvertFromUtf32(160), " ");
              if ((nodeCredit.SelectSingleNodeFast("ActorRole") != null) &&
                  (nodeCredit.SelectSingleNodeFast("ActorRole").InnerText.Length > 0)) line += " (" + nodeCredit.SelectSingleNodeFast("ActorRole").InnerText.Replace(char.ConvertFromUtf32(160), " ") + ")";
              if (line.Length > 0)
              {
                if (Actor.Length > 0) Actor += ", ";
                Actor += line;
              }
            }
            WriteAntAtribute(destXml, "Actors", Actor);

            //string URL, 
            string strURL = String.Empty;
            XmlNode nodeURL = nodeDVD.SelectSingleNodeFast("WebLinkScript");
            if (nodeURL != null && nodeURL.InnerText != null) strURL = nodeURL.InnerText.Replace(char.ConvertFromUtf32(160), " ");
            WriteAntAtribute(destXml, "WebLinkScript", strURL);

            //string Description, 
            XmlNode nodePlot = nodeDVD.SelectSingleNodeFast("Plot");
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
            WriteAntAtribute(destXml, "Plot", DescriptionMerged);

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
            XmlNode nodeVideoFormat = nodeDVD.SelectSingleNodeFast("Codec");
            if (nodeVideoFormat != null && nodeVideoFormat.InnerText.Length > 0) WriteAntAtribute(destXml, "Codec", nodeVideoFormat.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string VideoBitrate, Bitrate 
            XmlNode nodeVideoBitrate = nodeDVD.SelectSingleNodeFast("Bitrate");
            if (nodeVideoBitrate != null && nodeVideoBitrate.InnerText.Length > 0) WriteAntAtribute(destXml, "Bitrate", nodeVideoBitrate.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string AudioFormat, 
            XmlNode nodeAudioFormat = nodeDVD.SelectSingleNodeFast("AudioCodec");
            if (nodeAudioFormat != null && nodeAudioFormat.InnerText.Length > 0) WriteAntAtribute(destXml, "AudioCodec", nodeAudioFormat.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string AudioBitrate, 
            XmlNode nodeAudioBitrate = nodeDVD.SelectSingleNodeFast("AudioBitRate");
            if (nodeAudioBitrate != null && nodeAudioBitrate.InnerText.Length > 0) WriteAntAtribute(destXml, "AudioBitRate", nodeAudioBitrate.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Resolution, 
            XmlNode nodeResolution = nodeDVD.SelectSingleNodeFast("Resolution");
            if (nodeResolution != null && nodeResolution.InnerText.Length > 0) WriteAntAtribute(destXml, "Resolution", nodeResolution.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Framerate, 
            XmlNode nodeFramerate = nodeDVD.SelectSingleNodeFast("FPS");
            if (nodeFramerate != null && nodeFramerate.InnerText.Length > 0) WriteAntAtribute(destXml, "FPS", nodeFramerate.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Languages, 
            XmlNode nodeLanguages = nodeDVD.SelectSingleNodeFast("OriginalLanguage");
            if (nodeLanguages != null && nodeLanguages.InnerText.Length > 0) WriteAntAtribute(destXml, "OriginalLanguage", nodeLanguages.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Subtitles, 
            XmlNode nodeSubtitles = nodeDVD.SelectSingleNodeFast("Subtitles");
            if (nodeSubtitles != null && nodeSubtitles.InnerText.Length > 0) WriteAntAtribute(destXml, "Subtitles", nodeSubtitles.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Size, 
            XmlNode nodeSize = nodeDVD.SelectSingleNodeFast("Filesize");
            if (nodeSize != null && nodeSize.InnerText.Length > 0) WriteAntAtribute(destXml, "Filesize", nodeSize.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Disks, 
            XmlNode nodeDisks = nodeDVD.SelectSingleNodeFast("Disks");
            if (nodeDisks != null && nodeDisks.InnerText.Length > 0) WriteAntAtribute(destXml, "Disks", nodeDisks.InnerText.Replace(char.ConvertFromUtf32(160), " "));

            //string Picture
            string Image = String.Empty;
            if (nodeDVD.SelectSingleNodeFast("Cover") != null) Image = nodeDVD.SelectSingleNodeFast("Cover").InnerText.Replace(char.ConvertFromUtf32(160), " ");
            WriteAntAtribute(destXml, "Cover", Image);

            //string Fanart
            string Fanart = String.Empty;
            if (nodeDVD.SelectSingleNodeFast("FanArt") != null) Fanart = nodeDVD.SelectSingleNodeFast("FanArt").InnerText.Replace(char.ConvertFromUtf32(160), " ");
            WriteAntAtribute(destXml, "Fanart", Fanart);

            // Now writing MF extended attributes
            WriteAntElement(destXml, "MPAA", Certification);
            WriteAntElement(destXml, "TagLine", Tagline);
            WriteAntElement(destXml, "Tags", Tags);
            WriteAntElement(destXml, "Writer", Writer);

            destXml.WriteEndElement();
          }

        }
        catch //(Exception ex)
        {
          //throw;
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
