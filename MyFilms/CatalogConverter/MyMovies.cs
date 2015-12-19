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
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Xml;
  using System.Globalization;

  using Grabber;

  class MyMovies
  {
    public Dictionary<string, string> ProfilerDict;

    public MyMovies()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("CollectionNumber", "Number");
      ProfilerDict.Add("Watched", "Checked");
      ProfilerDict.Add("Barcode", "MediaLabel");
      ProfilerDict.Add("Format", "MediaType");
      ProfilerDict.Add("Title", "OriginalTitle");
      ProfilerDict.Add("TTitle", "TranslatedTitle");
      ProfilerDict.Add("STitle", "FormattedTitle");
      ProfilerDict.Add("Rating", "Rating");
      ProfilerDict.Add("MovieFile", "Source");
      ProfilerDict.Add("IMDB", "URL");
      ProfilerDict.Add("Country", "Country");
      ProfilerDict.Add("Year", "Year");
      ProfilerDict.Add("RunningTime", "Length");
      ProfilerDict.Add("Actors", "Actors");
      ProfilerDict.Add("Genres", "Category");
      ProfilerDict.Add("Credits", "Director");
      ProfilerDict.Add("Credits1", "Producer");
      ProfilerDict.Add("Credits2", "Writer");
      ProfilerDict.Add("Description", "Description");
      ProfilerDict.Add("Comments", "Comments"); // not exported by mymovies - only for merging infos
      ProfilerDict.Add("Picture", "Picture");
      ProfilerDict.Add("Date", "Date");
      //ProfilerDict.Add("Borrower", "Borrower");

      //ProfilerDict.Add("Codec", "VideoFormat");
      //ProfilerDict.Add("Bitrate", "VideoBitrate");
      //ProfilerDict.Add("AudioCodec", "AudioFormat");
      //ProfilerDict.Add("AudioBitRate", "AudioBitrate");
      //ProfilerDict.Add("Resolution", "Resolution");
      //ProfilerDict.Add("FPS", "Framerate");
      ProfilerDict.Add("AudioTracks", "Languages");
      ProfilerDict.Add("Subtitles", "Subtitles");
      //ProfilerDict.Add("Filesize", "Size");
      //ProfilerDict.Add("Disks", "Disks");

      ProfilerDict.Add("ParentalRating/Description", "Certification");
      ProfilerDict.Add("TagLine", "TagLine");
      ProfilerDict.Add("Categories", "Tags");
      ProfilerDict.Add("LocalTrailer/URL", "SourceTrailer");
      //ProfilerDict.Add("watched", "Watched");
      //ProfilerDict.Add("watcheddate", "WatchedDate");
      //ProfilerDict.Add("IMDB_Id", "IMDB_Id");
      //ProfilerDict.Add("TMDB_Id", "TMDB_Id");
    }
    public string ConvertMyMovies(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, bool OnlyFile)
    {
      string WStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = WStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + "_tmp.xml";
      XmlTextWriter destXml = new XmlTextWriter(destFile, Encoding.Default);
      destXml.Formatting = Formatting.Indented;
      destXml.WriteStartDocument();
      destXml.WriteStartElement("AntMovieCatalog");
      destXml.WriteStartElement("Catalog");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      try
      {
        //string Element = "";
        //string Text = "";
        //string XMLDeclaration = "";
        //string ProcessingInstruction = "";
        //string Comment = "";
        //string EndElement = "";
        //XmlReaderSettings settings = new XmlReaderSettings();
        //settings.ConformanceLevel = ConformanceLevel.Fragment;
        //using (XmlReader reader = XmlReader.Create(new StringReader(source), settings))
        //{
        //  // Parse the file and display each of the nodes.
        //  while (reader.Read())
        //  {
        //    try
        //    {
        //      switch (reader.NodeType)
        //      {
        //        case XmlNodeType.Element:
        //          Element = reader.Name;
        //          break;
        //        case XmlNodeType.Text:
        //          Text = reader.Value;
        //          break;
        //        case XmlNodeType.XmlDeclaration:
        //        case XmlNodeType.ProcessingInstruction:
        //          XMLDeclaration = reader.Name;
        //          ProcessingInstruction = reader.Value;
        //          break;
        //        case XmlNodeType.Comment:
        //          Comment = reader.Value;
        //          break;
        //        case XmlNodeType.EndElement:
        //          EndElement = "";
        //          break;
        //      }
        //    }
        //    catch (Exception ex)
        //    {

        //      //throw;
        //    }
        //  }
        //}


        XmlDocument doc = new XmlDocument();
        doc.Load(source);
        XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/Titles/Title");
        foreach (XmlNode nodeDVD in dvdList)
        {
          destXml.WriteStartElement("Movie");
          XmlNode nodeID = nodeDVD.SelectSingleNodeFast("ID");
          XmlNode nodeNumber = nodeDVD.SelectSingleNodeFast("CollectionNumber");
          XmlNode nodeFormat = nodeDVD.SelectSingleNodeFast("Type");
          XmlNode nodeLabel = nodeDVD.SelectSingleNodeFast("Barcode");
          XmlNode nodeTitle = nodeDVD.SelectSingleNodeFast("LocalTitle");
          XmlNode nodeOTitle = nodeDVD.SelectSingleNodeFast("OriginalTitle");
          XmlNode nodeSTitle = nodeDVD.SelectSingleNodeFast("SortTitle");
          XmlNode nodeYear = nodeDVD.SelectSingleNodeFast("ProductionYear");
          XmlNode nodeIMDB = nodeDVD.SelectSingleNodeFast("IMDB");
          XmlNodeList LinksKist = nodeDVD.SelectNodes("Discs/Disc");

          string Source = String.Empty;
          foreach (XmlNode nodeFile in LinksKist.Cast<XmlNode>().Where(nodeFile => nodeFile.SelectSingleNodeFast("LocationSideA").InnerText.Length > 0))
          {
            if (Source.Length > 0) Source += ";";
            Source += nodeFile.SelectSingleNodeFast("LocationSideA").InnerText;
          }

          XmlNodeList trailerList = nodeDVD.SelectNodes("LocalTrailer/URL");
          string SourceTrailer = String.Empty;
          foreach (XmlNode nodeTrailer in trailerList.Cast<XmlNode>().Where(nodeTrailer => !string.IsNullOrEmpty(nodeTrailer.InnerText)))
          {
            if (SourceTrailer.Length > 0) SourceTrailer += "; ";
            SourceTrailer += nodeTrailer.InnerText;
          }

          XmlNode nodeDuration = nodeDVD.SelectSingleNodeFast("RunningTime");
          XmlNode nodeChecked = nodeDVD.SelectSingleNodeFast("Watched");
          XmlNode nodeCountry = nodeDVD.SelectSingleNodeFast("Country");
          XmlNode nodeOverview = nodeDVD.SelectSingleNodeFast("Description");
          string Overview = string.Empty;
          if (nodeOverview != null && nodeOverview.InnerText != null)
            Overview = nodeOverview.InnerText;

          string genre = String.Empty;
          XmlNodeList genreList = nodeDVD.SelectNodes("Genres/Genre");
          foreach (XmlNode nodeGenre in genreList)
          {
            if (genre.Length > 0) genre += ", ";
            genre += nodeGenre.InnerText;
          }

          XmlNode nodeCertification = nodeDVD.SelectSingleNodeFast("ParentalRating/Description");
          string Certification = string.Empty;
          if (nodeCertification != null && nodeCertification.InnerText.Length > 0)
            Certification = nodeCertification.InnerText;

          XmlNode nodeTagLine = nodeDVD.SelectSingleNodeFast("TagLine");
          string Tagline = string.Empty;
          if (nodeTagLine != null && nodeTagLine.InnerText.Length > 0)
            Tagline = nodeTagLine.InnerText;

          string Tags = String.Empty;
          XmlNodeList categoryList = nodeDVD.SelectNodes("Categories/Category");
          foreach (XmlNode nodeCategory in categoryList)
          {
            if (Tags.Length > 0) Tags += ", ";
            Tags += nodeCategory.InnerText;
          }
          string Actor = String.Empty;
          string Director = String.Empty;
          string Producer = String.Empty;
          string Writer = string.Empty;
          XmlNodeList creditsList = nodeDVD.SelectNodes("Persons/Person");
          foreach (XmlNode nodeCredit in creditsList)
          {
            string line = String.Empty;
            if (nodeCredit.SelectSingleNodeFast("Type") != null && nodeCredit.SelectSingleNodeFast("Type").InnerText == "Actor")
            {
              if (nodeCredit.SelectSingleNodeFast("Name") != null)
                line = nodeCredit.SelectSingleNodeFast("Name").InnerText;
              if ((nodeCredit.SelectSingleNodeFast("Role") != null) && (nodeCredit.SelectSingleNodeFast("Role").InnerText.Length > 0))
                line += " (" + nodeCredit.SelectSingleNodeFast("Role").InnerText + ")";
              if (line.Length > 0)
              {
                if (Actor.Length > 0) Actor += ", ";
                Actor += line;
              }
            }
            else
              if (nodeCredit.SelectSingleNodeFast("Type") != null && nodeCredit.SelectSingleNodeFast("Type").InnerText == "Director")
              {
                if (nodeCredit.SelectSingleNodeFast("Name") != null)
                  line = nodeCredit.SelectSingleNodeFast("Name").InnerText;
                if (line.Length > 0)
                {
                  if (Director.Length > 0) Director += ", ";
                  Director += line;
                }
              }
              else
                if (nodeCredit.SelectSingleNodeFast("Type") != null && nodeCredit.SelectSingleNodeFast("Type").InnerText == "Producer")
                {
                  if (nodeCredit.SelectSingleNodeFast("Name") != null)
                    line = nodeCredit.SelectSingleNodeFast("Name").InnerText;
                  if (line.Length > 0)
                  {
                    if (Producer.Length > 0) Producer += ", ";
                    Producer += line;
                  }
                }
                else
                  if (nodeCredit.SelectSingleNodeFast("Type") != null && nodeCredit.SelectSingleNodeFast("Type").InnerText == "Writer")
                  {
                    if (nodeCredit.SelectSingleNodeFast("Name") != null)
                      line = nodeCredit.SelectSingleNodeFast("Name").InnerText;
                    if (line.Length > 0)
                    {
                      if (Writer.Length > 0) Writer += ", ";
                      Writer += line;
                    }
                  }
          }

          string languages = string.Empty;
          XmlNodeList LanguagesList = nodeDVD.SelectNodes("AudioTracks/AudioTrack");
          foreach (XmlNode nodeLanguage in LanguagesList.Cast<XmlNode>().Where(nodeLanguage => nodeLanguage.Attributes["Language"] != null && nodeLanguage.Attributes["Language"].Value != null))
          {
            if (languages.Length > 0) languages += ", ";
            languages += nodeLanguage.Attributes["Language"].Value;
            if (nodeLanguage.Attributes["Type"].Value != null)
              languages += " (" + nodeLanguage.Attributes["Type"].Value + ")";
          }

          string subtitles = String.Empty;
          XmlNodeList subtitleList = nodeDVD.SelectNodes("Subtitles/Subtitle");
          foreach (XmlNode nodeSubtitle in subtitleList.Cast<XmlNode>().Where(nodeSubtitle => nodeSubtitle.Attributes["Language"] != null && nodeSubtitle.Attributes["Language"].Value != null))
          {
            if (subtitles.Length > 0) subtitles += ", ";
            subtitles += nodeSubtitle.Attributes["Language"].Value;
          }

          string Image = String.Empty;
          if (nodeDVD.SelectSingleNodeFast("Covers/FrontMedium") != null) // try FrontMedium first, as better resolution
            Image = nodeDVD.SelectSingleNodeFast("Covers/FrontMedium").InnerText;
          if (string.IsNullOrEmpty(Image) && nodeDVD.SelectSingleNodeFast("Covers/Front") != null) // Front
            Image = nodeDVD.SelectSingleNodeFast("Covers/Front").InnerText;
          string Rating = string.Empty;
          decimal wrating = 0;
          CultureInfo ci = new CultureInfo("en-us");
          XmlNode nodeRating = nodeDVD.SelectSingleNodeFast("Rating");
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
          if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.InnerText))
            WriteAntAtribute(destXml, "CollectionNumber", nodeNumber.InnerText);
          else
            WriteAntAtribute(destXml, "CollectionNumber", "9999");

          string title = "";
          string otitle = "";
          if (nodeTitle != null && !string.IsNullOrEmpty(nodeTitle.InnerText))
            title = nodeTitle.InnerText;
          if (nodeOTitle != null && !string.IsNullOrEmpty(nodeOTitle.InnerText))
            otitle = nodeOTitle.InnerText;

          WriteAntAtribute(destXml, "Title", !string.IsNullOrEmpty(otitle) ? otitle : title);
          WriteAntAtribute(destXml, "TTitle", !string.IsNullOrEmpty(title) ? title : otitle);

          if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
            WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
          else
            WriteAntAtribute(destXml, "STitle", title);
          XmlNode nodeDate = nodeDVD.SelectSingleNodeFast("Added");

          string strDateAdded = string.Empty;
          IFormatProvider culture = new CultureInfo("en-US", true);
          if (nodeDate != null && nodeDate.InnerText.Length > 0)
          {
            strDateAdded = nodeDate.InnerText;
            if (strDateAdded.Contains(" "))
              strDateAdded = strDateAdded.Substring(0, strDateAdded.IndexOf(" ")); // Remove time...
          }

          try
          {
            DateTime dt = new DateTime();
            //dt = DateTime.Parse(strDateAdded);
            dt = DateTime.Parse(strDateAdded, culture, DateTimeStyles.NoCurrentDateDefault);
            //dt = DateTime.ParseExact(strDateAdded, "MM/dd/yyyy hh:mm:ss tt", culture, DateTimeStyles.NoCurrentDateDefault);
            WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
          }
          catch
          {
          }

          if (nodeChecked != null && nodeChecked.InnerText.Length > 0 && nodeChecked.InnerText.ToLower() == "true")
          {
            WriteAntAtribute(destXml, "Watched", "true");
          }
          else
          {
            WriteAntAtribute(destXml, "Watched", "false");
          }

          if (nodeCountry != null)
            WriteAntAtribute(destXml, "Country", nodeCountry.InnerText);
          WriteAntAtribute(destXml, "Rating", Rating);
          if (nodeYear != null)
            WriteAntAtribute(destXml, "Year", nodeYear.InnerText);
          if (nodeDuration != null && nodeDuration.InnerText.Length > 0)
            WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText);
          if (nodeFormat != null)
            WriteAntAtribute(destXml, "Format", nodeFormat.InnerText);
          if (nodeLabel != null)
            WriteAntAtribute(destXml, "Barcode", nodeLabel.InnerText);

          WriteAntAtribute(destXml, "Genres", genre);
          WriteAntAtribute(destXml, "Credits", Director);
          WriteAntAtribute(destXml, "Credits1", Producer);
          //WriteAntAtribute(destXml, "Credits2", Writer);
          WriteAntAtribute(destXml, "Actors", Actor);

          WriteAntAtribute(destXml, "AudioTracks", languages);
          WriteAntAtribute(destXml, "Subtitles", subtitles);
          //WriteAntAtribute(destXml, "ParentalRating/Description", Certification);
          //WriteAntAtribute(destXml, "TagLine", Tagline);
          //WriteAntAtribute(destXml, "Categories", Tags);
          if (nodeIMDB != null)
            WriteAntAtribute(destXml, "IMDB", nodeIMDB.InnerText);
          WriteAntAtribute(destXml, "MovieFile", Source);
          WriteAntAtribute(destXml, "LocalTrailer/URL", SourceTrailer);

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
          if (Overview.Length > 0)
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += Overview;
          }
          WriteAntAtribute(destXml, "Description", DescriptionMerged);

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
          WriteAntAtribute(destXml, "Comments", CommentsMerged);

          WriteAntAtribute(destXml, "Picture", Image);

          // Now writing MF extended attributes
          WriteAntElement(destXml, "ParentalRating/Description", Certification);
          WriteAntElement(destXml, "TagLine", Tagline);
          WriteAntElement(destXml, "Categories", Tags);
          WriteAntElement(destXml, "Credits2", Writer);

          destXml.WriteEndElement();
        }

      }
      catch (Exception)
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
