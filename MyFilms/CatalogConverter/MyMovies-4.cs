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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MyFilmsPlugin.CatalogConverter
{
  class MyMovies4
  {
    public Dictionary<string, string> ProfilerDict;

    public MyMovies4()
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
      ProfilerDict.Add("IMDB_Id", "IMDB_Id");
      //ProfilerDict.Add("TMDB_Id", "TMDB_Id");
    }
    public string ConvertMyMovies(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, bool OnlyFile)
    {
      string wStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = wStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + "_tmp.xml";
      XmlTextWriter destXml = new XmlTextWriter(destFile, Encoding.Default) {Formatting = Formatting.Indented};
      destXml.WriteStartDocument();
      destXml.WriteStartElement("AntMovieCatalog");
      destXml.WriteStartElement("Catalog");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      try
      {
        //XElement doc = XElement.Load(source);
        XDocument xml = new XDocument();

        xml = XDocument.Load(source);
        IEnumerable<XElement> dvdList = xml.Element("Titles").Elements("Title");

        //XmlDocument doc = new XmlDocument();
        //doc.Load(source);

        //// Get movies matching the ID not null
        //var dvdList = from movie in doc.Elements("/Titles/Title")
        //where !String.IsNullOrEmpty(movie.Element("ID").Value)
        //select movie;

        //// Get category of first page matched.
        //string c = v.First().Element("Category").Value;

        //// Count number of elements with that category element value.
        //int count = (from p in _x.Elements("SitePage")
        // where p.Element("Category").Value == c &&
        // p.Element("Visibility").Value == "Regular"
        // select p).Count();              

        foreach (XElement nodeDVD in dvdList)
        {
          destXml.WriteStartElement("Movie");

          string nodeID = nodeDVD.Element("ID").Value;
          XElement nodeNumber = nodeDVD.Element("CollectionNumber");
          XElement nodeFormat = nodeDVD.Element("Type");
          XElement nodeLabel = nodeDVD.Element("Barcode");
          XElement nodeTitle = nodeDVD.Element("LocalTitle");
          XElement nodeOTitle = nodeDVD.Element("OriginalTitle");
          XElement nodeSTitle = nodeDVD.Element("SortTitle");
          XElement nodeYear = nodeDVD.Element("ProductionYear");
          XElement nodeIMDB = nodeDVD.Element("IMDB");
          IEnumerable<XElement> LinksKist = nodeDVD.Elements("Discs/Disc");

          string Source = String.Empty;
          foreach (XElement nodeFile in LinksKist.Where(nodeFile => nodeFile.Element("LocationSideA").Value.Length > 0))
          {
            if (Source.Length > 0) Source += ";";
            Source += nodeFile.Element("LocationSideA").Value;
          }

          IEnumerable<XElement> trailerList = nodeDVD.Elements("LocalTrailer/URL");
          string SourceTrailer = String.Empty;
          foreach (XElement nodeTrailer in trailerList.Where(nodeTrailer => !string.IsNullOrEmpty(nodeTrailer.Value)))
          {
            if (SourceTrailer.Length > 0) SourceTrailer += "; ";
            SourceTrailer += nodeTrailer.Value;
          }

          XElement nodeDuration = nodeDVD.Element("RunningTime");
          XElement nodeChecked = nodeDVD.Element("Watched");
          XElement nodeCountry = nodeDVD.Element("Country");
          XElement nodeOverview = nodeDVD.Element("Description");
          string Overview = string.Empty;
          if (nodeOverview != null && nodeOverview.Value != null)
            Overview = nodeOverview.Value;

          string genre = String.Empty;
          IEnumerable<XElement> genreList = nodeDVD.Elements("Genres/Genre");
          foreach (XElement nodeGenre in genreList)
          {
            if (genre.Length > 0) genre += ", ";
            genre += nodeGenre.Value;
          }

          XElement nodeCertification = nodeDVD.Element("ParentalRating/Description");
          string Certification = string.Empty;
          if (nodeCertification != null && nodeCertification.Value.Length > 0)
            Certification = nodeCertification.Value;

          XElement nodeTagLine = nodeDVD.Element("TagLine");
          string Tagline = string.Empty;
          if (nodeTagLine != null && nodeTagLine.Value.Length > 0)
            Tagline = nodeTagLine.Value;

          string Tags = String.Empty;
          IEnumerable<XElement> categoryList = nodeDVD.Elements("Categories/Category");
          foreach (XElement nodeCategory in categoryList)
          {
            if (Tags.Length > 0) Tags += ", ";
            Tags += nodeCategory.Value;
          }
          string Actor = String.Empty;
          string Director = String.Empty;
          string Producer = String.Empty;
          string Writer = string.Empty;
          IEnumerable<XElement> creditsList = nodeDVD.Elements("Persons/Person");
          foreach (XElement nodeCredit in creditsList)
          {
            string line = String.Empty;
            if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Actor")
            {
              if (nodeCredit.Element("Name") != null)
                line = nodeCredit.Element("Name").Value;
              if ((nodeCredit.Element("Role") != null) && (nodeCredit.Element("Role").Value.Length > 0))
                line += " (" + nodeCredit.Element("Role").Value + ")";
              if (line.Length > 0)
              {
                if (Actor.Length > 0) Actor += ", ";
                Actor += line;
              }
            }
            else
              if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Director")
              {
                if (nodeCredit.Element("Name") != null)
                  line = nodeCredit.Element("Name").Value;
                if (line.Length > 0)
                {
                  if (Director.Length > 0) Director += ", ";
                  Director += line;
                }
              }
              else
                if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Producer")
                {
                  if (nodeCredit.Element("Name") != null)
                    line = nodeCredit.Element("Name").Value;
                  if (line.Length > 0)
                  {
                    if (Producer.Length > 0) Producer += ", ";
                    Producer += line;
                  }
                }
                else
                  if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Writer")
                  {
                    if (nodeCredit.Element("Name") != null)
                      line = nodeCredit.Element("Name").Value;
                    if (line.Length > 0)
                    {
                      if (Writer.Length > 0) Writer += ", ";
                      Writer += line;
                    }
                  }
          }

          string languages = string.Empty;
          IEnumerable<XElement> LanguagesList = nodeDVD.Elements("AudioTracks/AudioTrack");
          foreach (XElement nodeLanguage in LanguagesList)
          {
            if (nodeLanguage.Attribute("Language") != null && nodeLanguage.Attribute("Language").Value != null)
            {
              if (languages.Length > 0) languages += ", ";
              languages += nodeLanguage.Attribute("Language").Value;
              if (nodeLanguage.Attribute("Type").Value != null)
                languages += " (" + nodeLanguage.Attribute("Type").Value + ")";
            }
          }

          string subtitles = String.Empty;
          IEnumerable<XElement> subtitleList = nodeDVD.Elements("Subtitles/Subtitle");
          foreach (XElement nodeSubtitle in subtitleList)
          {
            if (nodeSubtitle.Attribute("Language") != null && nodeSubtitle.Attribute("Language").Value != null)
            {
              if (subtitles.Length > 0) subtitles += ", ";
              subtitles += nodeSubtitle.Attribute("Language").Value;
            }
          }

          string Image = String.Empty;
          if (nodeDVD.Element("Covers/FrontMedium") != null) // try FrontMedium first, as better resolution
            Image = nodeDVD.Element("Covers/FrontMedium").Value;
          if (string.IsNullOrEmpty(Image) && nodeDVD.Element("Covers/Front") != null) // Front
            Image = nodeDVD.Element("Covers/Front").Value;
          string Rating = string.Empty;
          decimal wrating = 0;
          CultureInfo ci = new CultureInfo("en-us");
          XElement nodeRating = nodeDVD.Element("Rating");
          if (nodeRating != null && nodeRating.Value != null)
          {
            try { wrating = Convert.ToDecimal(nodeRating.Value); }
            catch
            {
              try { wrating = Convert.ToDecimal(nodeRating.Value, ci); }
              catch { }
            }
          }
          Rating = wrating.ToString("0.0", ci);
          if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.Value))
            WriteAntAtribute(destXml, "CollectionNumber", nodeNumber.Value);
          else
            WriteAntAtribute(destXml, "CollectionNumber", "9999");
          
          if (nodeOTitle != null && nodeOTitle.Value.Length > 0)
            WriteAntAtribute(destXml, "Title", nodeOTitle.Value);
          else
            WriteAntAtribute(destXml, "Title", nodeTitle.Value);
          
          WriteAntAtribute(destXml, "TTitle", nodeTitle.Value);
          
          if (nodeSTitle != null && nodeSTitle.Value.Length > 0)
            WriteAntAtribute(destXml, "STitle", nodeSTitle.Value);
          else
            WriteAntAtribute(destXml, "STitle", nodeTitle.Value);
          
          XElement nodeDate = nodeDVD.Element("Added");

          string strDateAdded = string.Empty;
          IFormatProvider culture = new CultureInfo("en-US", true);
          if (nodeDate != null && nodeDate.Value.Length > 0)
          {
            strDateAdded = nodeDate.Value;
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

          if (nodeChecked != null && nodeChecked.Value.Length > 0 && nodeChecked.Value.ToLower() == "true")
          {
            WriteAntAtribute(destXml, "Watched", "true");
          }
          else
          {
            WriteAntAtribute(destXml, "Watched", "false");
          }

          if (nodeCountry != null)
            WriteAntAtribute(destXml, "Country", nodeCountry.Value);
          WriteAntAtribute(destXml, "Rating", Rating);
          if (nodeYear != null)
            WriteAntAtribute(destXml, "Year", nodeYear.Value);
          if (nodeDuration != null && nodeDuration.Value.Length > 0)
            WriteAntAtribute(destXml, "RunningTime", nodeDuration.Value);
          if (nodeFormat != null)
            WriteAntAtribute(destXml, "Format", nodeFormat.Value);
          if (nodeLabel != null)
            WriteAntAtribute(destXml, "Barcode", nodeLabel.Value);

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
          {
            WriteAntAtribute(destXml, "IMDB", nodeIMDB.Value); // goes into "URL"
            WriteAntAtribute(destXml, "IMDB_Id", nodeIMDB.Value); // goes into "IMDB_Id" field
          }
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
