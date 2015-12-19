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
  using System.Linq;
  using System.Text;
  using System.Xml;
  using System.Globalization;

  using Grabber;

  class MovieCollector
  {
    public Dictionary<string, string> ProfilerDict;

    public MovieCollector()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("index", "Number");
      ProfilerDict.Add("Format", "MediaType");
      ProfilerDict.Add("Storage", "MediaLabel");
      ProfilerDict.Add("Title", "OriginalTitle");
      ProfilerDict.Add("TTitle", "TranslatedTitle");
      ProfilerDict.Add("STitle", "FormattedTitle");
      ProfilerDict.Add("imdbrating", "Rating");
      ProfilerDict.Add("MovieFile", "Source");
      ProfilerDict.Add("Country", "Country");
      ProfilerDict.Add("Year", "Year");
      ProfilerDict.Add("RunningTime", "Length");
      ProfilerDict.Add("imdburl", "URL");
      ProfilerDict.Add("Actors", "Actors");
      ProfilerDict.Add("Genres", "Category");
      ProfilerDict.Add("Credits", "Director");
      ProfilerDict.Add("Credits1", "Producer");
      ProfilerDict.Add("Credits2", "Writer");
      ProfilerDict.Add("Overview", "Description");
      ProfilerDict.Add("Comments", "Comments");
      ProfilerDict.Add("Picture", "Picture");
      ProfilerDict.Add("Date", "Date");
      ProfilerDict.Add("Viewed", "Checked");
      ProfilerDict.Add("Borrower", "Borrower");
      ProfilerDict.Add("mpaarating", "Certification");
      ProfilerDict.Add("Tags", "Tags");
    }
    public string ConvertMovieCollector(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, bool OnlyFile, string TitleDelim)
    {
      if (TitleDelim.Length == 0)
        TitleDelim = "\\";
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
        XmlDocument doc = new XmlDocument();
        doc.Load(source);
        XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/movieinfo/movielist/movie");
        foreach (XmlNode nodeDVD in dvdList)
        {
          const string Tagline = "(no Tagline supported)";

          XmlNode nodeID = nodeDVD.SelectSingleNodeFast("id");
          XmlNode nodeMediaType = nodeDVD.SelectSingleNodeFast("format/displayname");
          XmlNode nodeNumber = null;
          try
          {
            nodeNumber = nodeDVD.SelectSingleNodeFast("index");
          }
          catch
          {
          }
          XmlNode nodeFormat = nodeDVD.SelectSingleNodeFast("format/displayname");
          XmlNode nodeTitle = nodeDVD.SelectSingleNodeFast("title");
          XmlNode nodeOTitle = nodeDVD.SelectSingleNodeFast("originaltitle");
          XmlNode nodeSTitle = nodeDVD.SelectSingleNodeFast("titlesort");
          XmlNode nodeViewed = nodeDVD.SelectSingleNodeFast("seenit");
          XmlNode nodeYear = nodeDVD.SelectSingleNodeFast("releasedate/year/displayname");
          XmlNodeList LinksKist = nodeDVD.SelectNodes("links/link");
          string url = String.Empty;
          foreach (XmlNode nodeFile in LinksKist.Cast<XmlNode>().Where(nodeFile => nodeFile.SelectSingleNodeFast("urltype").InnerText == "Movie"))
          {
            if (url.Length > 0) url += ";";
            url += nodeFile.SelectSingleNodeFast("url").InnerText;
          }
          XmlNode nodeBorrower = nodeDVD.SelectSingleNodeFast("loan/loanedto/displayname");
          XmlNode nodeDuration = nodeDVD.SelectSingleNodeFast("runtime");
          XmlNode nodeDurationMinutes = nodeDVD.SelectSingleNodeFast("runtimeminutes");
          XmlNode nodeURL = nodeDVD.SelectSingleNodeFast("imdburl");
          XmlNode nodeCountry = nodeDVD.SelectSingleNodeFast("country");
          XmlNode nodeOverview = nodeDVD.SelectSingleNodeFast("plot");
          string genre = String.Empty;
          XmlNodeList genreList = nodeDVD.SelectNodes("genres/genre");
          foreach (XmlNode nodeGenre in genreList)
          {
            if (nodeGenre.SelectSingleNodeFast("displayname") != null)
              if (genre.Length > 0) genre += ", ";
            genre += nodeGenre.SelectSingleNodeFast("displayname").InnerText;
          }
          string cast = String.Empty;
          XmlNodeList actorsList = nodeDVD.SelectNodes("cast/star");
          foreach (XmlNode nodeActor in actorsList)
          {
            string line = String.Empty;
            if (nodeActor.SelectSingleNodeFast("person/displayname") != null)
              line = nodeActor.SelectSingleNodeFast("person/displayname").InnerText;
            if (nodeActor.SelectSingleNodeFast("character") != null && nodeActor.SelectSingleNodeFast("character").InnerText.Length > 0)
              line += " (" + nodeActor.SelectSingleNodeFast("character").InnerText + ")";
            if (line.Length > 0)
            {
              if (cast.Length > 0) cast += ", ";
              cast += line;
            }
          }
          string Director = String.Empty;
          string Producer = String.Empty;
          string Writer = String.Empty;
          XmlNodeList creditsList = nodeDVD.SelectNodes("crew/crewmember");
          foreach (XmlNode nodeCredit in creditsList)
          {
            string line = String.Empty;
            if (nodeCredit.SelectSingleNodeFast("roleid") != null && nodeCredit.SelectSingleNodeFast("roleid").InnerText == "dfDirector")
            {
              if (nodeCredit.SelectSingleNodeFast("person/displayname") != null)
                line = nodeCredit.SelectSingleNodeFast("person/displayname").InnerText;
              if (line.Length > 0)
              {
                if (Director.Length > 0) Director += ", ";
                Director += line;
              }
            }
            else
              if (nodeCredit.SelectSingleNodeFast("roleid") != null && nodeCredit.SelectSingleNodeFast("roleid").InnerText == "dfProducer")
              {
                if (nodeCredit.SelectSingleNodeFast("person/displayname") != null)
                  line = nodeCredit.SelectSingleNodeFast("person/displayname").InnerText;
                if (line.Length > 0)
                {
                  if (Producer.Length > 0) Producer += ", ";
                  Producer += line;
                }
              }
              else
                if (nodeCredit.SelectSingleNodeFast("roleid") != null && nodeCredit.SelectSingleNodeFast("roleid").InnerText == "dfWriter")
                {
                  if (nodeCredit.SelectSingleNodeFast("person/displayname") != null)
                    line = nodeCredit.SelectSingleNodeFast("person/displayname").InnerText;
                  if (line.Length > 0)
                  {
                    if (Writer.Length > 0) Writer += ", ";
                    Writer += line;
                  }
                }
          }

          string Tags = string.Empty;
          XmlNodeList tagsList = nodeDVD.SelectNodes("tags/tag");
          foreach (XmlNode nodeTag in tagsList)
          {
            if (nodeTag.SelectSingleNodeFast("displayname") != null)
              if (Tags.Length > 0) Tags += ", ";
            Tags += nodeTag.SelectSingleNodeFast("displayname").InnerText;
          }

          string Image = String.Empty;
          if (nodeDVD.SelectSingleNodeFast("coverfront") != null)
            Image = nodeDVD.SelectSingleNodeFast("coverfront").InnerText;

          string Rating = string.Empty;
          decimal wrating = 0;
          CultureInfo ci = new CultureInfo("en-us");
          XmlNode nodeRating = nodeDVD.SelectSingleNodeFast("myrating");
          if (nodeRating != null && nodeRating.InnerText != null)
          {
            try { wrating = Convert.ToDecimal(nodeRating.InnerText); }
            catch
            {
              try { wrating = Convert.ToDecimal(nodeRating.InnerText, ci); }
              catch { }
            }
          }
          if (wrating == 0)
          {
            nodeRating = nodeDVD.SelectSingleNodeFast("imdbrating");
            if (nodeRating != null && nodeRating.InnerText != null)
            {
              try { wrating = Convert.ToDecimal(nodeRating.InnerText); }
              catch
              {
                try { wrating = Convert.ToDecimal(nodeRating.InnerText, ci); }
                catch { }
              }
            }
          }
          Rating = wrating.ToString("0.0", ci);

          string Certification = string.Empty;
          XmlNode nodeCertification = nodeDVD.SelectSingleNodeFast("mpaarating/displayname");
          if (nodeCertification != null && nodeCertification.InnerText != null)
            Certification = nodeCertification.InnerText;

          XmlNodeList DiscsList = nodeDVD.SelectNodes("discs/disc");
          string medialabel = String.Empty;
          int nodisc = 0;

          foreach (XmlNode nodeDisc in DiscsList)
          {
            destXml.WriteStartElement("Movie");
            nodisc++;
            string wmedialabel = string.Empty;
            if (nodeDisc.SelectSingleNodeFast("storagedevice/displayname") != null)
              wmedialabel = nodeDisc.SelectSingleNodeFast("storagedevice/displayname").InnerText;
            if (nodeDisc.SelectSingleNodeFast("storageslot") != null)
              if (wmedialabel.Length > 0)
                wmedialabel = wmedialabel + "\\" + nodeDisc.SelectSingleNodeFast("storageslot").InnerText;
              else
                wmedialabel = nodeDisc.SelectSingleNodeFast("storageslot").InnerText;

            if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.InnerText))
              WriteAntAtribute(destXml, "index", nodeNumber.InnerText + nodisc.ToString());
            else
              WriteAntAtribute(destXml, "index", "9999");
            if (DiscsList.Count > 1)
            {
              if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
              else
                WriteAntAtribute(destXml, "Title", nodeTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
              if (nodeTitle != null && nodeTitle.InnerText.Length > 0)
                WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
              if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
                WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
              else
                WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
            }
            else
            {
              if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText);
              else
                WriteAntAtribute(destXml, "Title", nodeTitle.InnerText);
              if (nodeTitle != null && nodeTitle.InnerText.Length > 0)
                WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText);
              if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
                WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
              else
                WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText);
            }
            //WriteAntAtribute(destXml, "Notes/File", File);
            if (wmedialabel.Length > 0)
              WriteAntAtribute(destXml, "Storage", wmedialabel);
            if (nodeViewed.Attributes != null)
              WriteAntAtribute(destXml, "Viewed", nodeViewed.Attributes["boolvalue"].Value == "1" ? "true" : "false");
            else
              WriteAntAtribute(destXml, "Viewed", "false");
            XmlNode nodeDate = nodeDVD.SelectSingleNodeFast("lastmodified/date");
            try
            {
              DateTime dt = new DateTime();
              dt = DateTime.Parse(nodeDate.InnerText);
              WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
            }
            catch
            {
            }
            if (nodeCountry.SelectSingleNodeFast("displayname") != null)
              WriteAntAtribute(destXml, "Country", nodeCountry.SelectSingleNodeFast("displayname").InnerText);
            WriteAntAtribute(destXml, "imdbrating", Rating);
            //WriteAntAtribute(destXml, "mpaarating", Certification);
            if (nodeYear != null && nodeYear.InnerText != null)
              WriteAntAtribute(destXml, "Year", nodeYear.InnerText);
            // Old code:
            //if (nodeDuration != null && nodeDuration.InnerText.Substring(0, nodeDuration.InnerText.IndexOf(" ")).Length > 0)
            //{
            //  if (nodeDuration.InnerText.IndexOf(" hr ") != -1)
            //  {
            //    int duree = int.Parse(nodeDuration.InnerText.Substring(0, nodeDuration.InnerText.IndexOf(" hr ")).Trim()) * 60 + int.Parse(nodeDuration.InnerText.Substring(nodeDuration.InnerText.IndexOf(" hr ") + 4, 2).Trim());
            //    WriteAntAtribute(destXml, "RunningTime", duree.ToString());
            //  }
            //  else
            //    WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText.Substring(0, nodeDuration.InnerText.IndexOf(" ")).ToString());
            //}
            // New code:
            if (nodeDuration != null && !string.IsNullOrEmpty(nodeDuration.InnerText))
            {
              WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText);
            }
            if (nodeURL != null && !string.IsNullOrEmpty(nodeURL.InnerText))
            {
              WriteAntAtribute(destXml, "imdburl", nodeURL.InnerText);
            }

            if (nodeBorrower != null && nodeBorrower.InnerText != null)
              WriteAntAtribute(destXml, "Borrower", nodeBorrower.InnerText);
            if (nodeFormat != null && nodeFormat.InnerText != null)
              WriteAntAtribute(destXml, "Format", nodeFormat.InnerText);
            WriteAntAtribute(destXml, "Genres", genre);
            //WriteAntAtribute(destXml, "Tags", Tags);
            WriteAntAtribute(destXml, "Credits", Director);
            WriteAntAtribute(destXml, "Credits1", Producer);
            //WriteAntAtribute(destXml, "Credits2", Writer);
            WriteAntAtribute(destXml, "Actors", cast);
            WriteAntAtribute(destXml, "Picture", Image);
            WriteAntAtribute(destXml, "MovieFile", url);

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
            if (nodeOverview != null && nodeOverview.InnerText != null)
            {
              if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
              DescriptionMerged += nodeOverview.InnerText;
            }
            WriteAntAtribute(destXml, "Overview", DescriptionMerged);

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

            // Now writing MF extended attributes
            WriteAntElement(destXml, "mpaarating", Certification);
            //WriteAntElement(destXml, "TAGLINE", Tagline);
            WriteAntElement(destXml, "Tags", Tags);
            WriteAntElement(destXml, "Credits2", Writer);

            destXml.WriteEndElement();
          }
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
