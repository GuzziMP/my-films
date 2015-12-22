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
using System.Linq;
using System.Text;
using System.Xml;
using Grabber;

namespace MyFilmsPlugin.CatalogConverter
{
  class DvdProfiler
  {
    public Dictionary<string, string> ProfilerDict;

    public DvdProfiler(string TagField)
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("Title", "OriginalTitle");
      ProfilerDict.Add("TTitle", "TranslatedTitle");
      ProfilerDict.Add("STitle", "FormattedTitle");
      ProfilerDict.Add("CollectionNumber", "Number");
      ProfilerDict.Add("MediaTypes", "MediaType");
      ProfilerDict.Add("UPC", "MediaLabel");
      ProfilerDict.Add("Review/ReviewFilm", "Rating");
      ProfilerDict.Add("Notes/File", "Source");
      ProfilerDict.Add("Notes/Country", "Country");
      ProfilerDict.Add("Year", "Year");
      ProfilerDict.Add("RunningTime", "Length");
      ProfilerDict.Add("Actors", "Actors");
      ProfilerDict.Add("Genres", "Category");
      ProfilerDict.Add("Credits", "Director");
      ProfilerDict.Add("Credits1", "Producer");
      ProfilerDict.Add("Credits2", "Writer");
      ProfilerDict.Add("Overview", "Description");
      ProfilerDict.Add("Comments", "Comments");
      ProfilerDict.Add("Picture", "Picture");
      ProfilerDict.Add("Date", "Date");
      ProfilerDict.Add("EventType", "Checked");
      ProfilerDict.Add("Tag", TagField);
      ProfilerDict.Add("Tags", "Tags");
      ProfilerDict.Add("AudioContent", "Languages");
      ProfilerDict.Add("Subtitle", "Subtitles");
      ProfilerDict.Add("Rating", "Certification");
      ProfilerDict.Add("LoanInfo/User", "Borrower");

    }
    public string TagFullName;

    public string ConvertProfiler(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, string TagField, bool OnlyFile)
    {
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
        var doc = new XmlDocument();
        doc.Load(source);
        var dvdList = doc.DocumentElement.SelectNodes("/Collection/DVD");
        foreach (XmlNode nodeDvd in dvdList)
        {
          destXml.WriteStartElement("Movie");
          XmlNode nodeID = nodeDvd.SelectSingleNodeFast("ID");
          XmlNode nodeMediaType = nodeDvd.SelectSingleNodeFast("MediaTypes/DVD");
          XmlNode nodeNumber = null;
          try
          {
            nodeNumber = nodeDvd.SelectSingleNodeFast("CollectionNumber");
          }
          catch
          {
          }
          XmlNode nodeTitle = nodeDvd.SelectSingleNodeFast("Title");
          XmlNode nodeOTitle = nodeDvd.SelectSingleNodeFast("OriginalTitle");
          XmlNode nodeSTitle = nodeDvd.SelectSingleNodeFast("SortTitle");

          string mediatype = String.Empty;
          XmlNode mediaTypes = nodeDvd.SelectSingleNodeFast("MediaTypes");
          foreach (XmlElement type in mediaTypes.Cast<XmlElement>().Where(type => type.InnerText == "true"))
          {
            if (mediatype.Length > 0) mediatype += ", ";
            mediatype += type.Name;
          }

          string medialabel = String.Empty;
          XmlNode nodeMediaLabel = nodeDvd.SelectSingleNodeFast("UPC");
          if (nodeMediaLabel != null && nodeMediaLabel.InnerText.Length > 0) medialabel = nodeMediaLabel.InnerText;

          XmlNode nodeNotes = nodeDvd.SelectSingleNodeFast("Notes");
          XmlNode nodeYear = nodeDvd.SelectSingleNodeFast("ProductionYear");
          XmlNode nodeDuration = nodeDvd.SelectSingleNodeFast("RunningTime");
          string Overview = string.Empty;
          XmlNode nodeOverview = nodeDvd.SelectSingleNodeFast("Overview");
          if (nodeOverview != null && nodeOverview.InnerText.Length > 0)
          {
            //Encoding encoding = Encoding.GetEncoding("windows-1252");
            //Overview = System.Web.HttpUtility.UrlDecode ( nodeOverview.InnerText, encoding );
            Overview = nodeOverview.InnerText;
            Overview = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(Overview));
            //StringWriter myWriter = new StringWriter();
            //// Decode the encoded string.
            //HttpUtility.HtmlDecode(nodeOverview.InnerText, myWriter);
            //Overview = myWriter.ToString();
          }

          string File = null;
          string Rating = null;
          string Country = null;
          if (nodeNotes != null && nodeNotes.InnerText != null)
          {
            if (nodeNotes.InnerText.IndexOf("<File>") != -1)
              File = nodeNotes.InnerText.Substring(nodeNotes.InnerText.IndexOf("<File>") + 6, nodeNotes.InnerText.IndexOf("</File>") - nodeNotes.InnerText.IndexOf("<File>") - 6);
            else
              if (OnlyFile)
                File = nodeNotes.InnerText.Trim();
            if (nodeNotes.InnerText.IndexOf("<Country>") != -1)
              Country = nodeNotes.InnerText.Substring(nodeNotes.InnerText.IndexOf("<Country>") + 9, nodeNotes.InnerText.IndexOf("</Country>") - nodeNotes.InnerText.IndexOf("<Country>") - 9);
          }

          if (nodeMediaType != null)
          {
            XmlNode nodeCountryOfOrigin = nodeDvd.SelectSingleNodeFast("CountryOfOrigin");
            Country = nodeCountryOfOrigin.InnerText;
          }
          string genre = String.Empty;
          XmlNodeList genreList = nodeDvd.SelectNodes("Genres/Genre");
          foreach (XmlNode nodeGenre in genreList)
          {
            if (genre.Length > 0) genre += ", ";
            genre += nodeGenre.InnerText;
          }

          const string Tagline = "(no Tagline supported by this catalog)";

          XmlNodeList TagList = nodeDvd.SelectNodes("Tags/Tag");
          TagFullName = string.Empty;

          foreach (XmlNode nodeTag in TagList.Cast<XmlNode>().Where(nodeTag => nodeTag.Attributes["FullName"] != null && nodeTag.Attributes["FullName"].Value != null))
          {
            if (TagFullName.Length > 0) TagFullName += ", ";
            TagFullName += nodeTag.Attributes["FullName"].Value;
          }

          if (TagField.Length > 0)
          {
            if (TagField == "Category")
            {
              if (genre.Length > 0) genre += ", ";
              genre += TagFullName;
            }
          }

          string languages = string.Empty;
          XmlNodeList LanguagesList = nodeDvd.SelectNodes("Audio/AudioTrack");
          foreach (XmlNode nodeLanguage in LanguagesList.Cast<XmlNode>().Where(nodeLanguage => nodeLanguage.SelectSingleNodeFast("AudioContent") != null && nodeLanguage.SelectSingleNodeFast("AudioContent").InnerText != null))
          {
            if (languages.Length > 0) languages += ", ";
            languages += nodeLanguage.SelectSingleNodeFast("AudioContent").InnerText;
          }

          string subtitles = String.Empty;
          XmlNodeList subtitleList = nodeDvd.SelectNodes("Subtitles/Subtitle");
          foreach (XmlNode nodeSubtitle in subtitleList)
          {
            if (subtitles.Length > 0) subtitles += ", ";
            subtitles += nodeSubtitle.InnerText;
          }

          string cast = String.Empty;
          XmlNodeList actorsList = nodeDvd.SelectNodes("Actors/Actor");
          foreach (XmlNode nodeActor in actorsList)
          {
            string firstname = String.Empty;
            string lastname = String.Empty;
            string role = String.Empty;
            if (nodeMediaType != null)
            {
              if (nodeActor.Attributes["FirstName"] != null && nodeActor.Attributes["FirstName"].Value != null) firstname = nodeActor.Attributes["FirstName"].Value;
              if (nodeActor.Attributes["LastName"] != null && nodeActor.Attributes["LastName"].Value != null) lastname = nodeActor.Attributes["LastName"].Value;
              if (nodeActor.Attributes["Role"] != null && nodeActor.Attributes["Role"].Value != null) role = nodeActor.Attributes["Role"].Value;
            }
            else
            {
              XmlNode nodeFirstName = nodeActor.SelectSingleNodeFast("FirstName");
              XmlNode nodeLastName = nodeActor.SelectSingleNodeFast("LastName");
              XmlNode nodeRole = nodeActor.SelectSingleNodeFast("Role");
              if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
              if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
              if (nodeRole != null && nodeRole.InnerText != null) role = nodeRole.InnerText;
            }
            string line;
            line = role.Length == 0 ? String.Format("{0} {1}", firstname, lastname) : String.Format("{0} {1} ({2})", firstname, lastname, role);
            if (cast.Length > 0) cast += ", ";
            cast += line;
          }
          string Director = String.Empty;
          string Producer = String.Empty;
          string Writer = String.Empty;
          XmlNodeList creditsList = nodeDvd.SelectNodes("Credits/Credit");
          foreach (XmlNode nodeCredit in creditsList)
          {
            string firstname = String.Empty;
            string lastname = String.Empty;
            if (nodeMediaType != null)
            {
              firstname = nodeCredit.Attributes["FirstName"].Value;
              lastname = nodeCredit.Attributes["LastName"].Value;
              if (nodeCredit.Attributes["CreditSubtype"] != null && nodeCredit.Attributes["CreditSubtype"].Value != null && nodeCredit.Attributes["CreditSubtype"].Value == "Director")
              {
                if (Director.Length > 0) Director += ", ";
                Director += String.Format("{0} {1}", firstname, lastname);
              }
              if (nodeCredit.Attributes["CreditSubtype"] != null && nodeCredit.Attributes["CreditSubtype"].Value != null && nodeCredit.Attributes["CreditSubtype"].Value == "Producer")
              {
                if (Producer.Length > 0) Producer += ", ";
                Producer += String.Format("{0} {1}", firstname, lastname);
              }
              if (nodeCredit.Attributes["CreditSubtype"] != null && nodeCredit.Attributes["CreditSubtype"].Value != null && nodeCredit.Attributes["CreditSubtype"].Value == "Screenwriter")
              {
                if (Writer.Length > 0) Writer += ", ";
                Writer += String.Format("{0} {1}", firstname, lastname);
              }
            }
            else
            {
              XmlNode nodeFirstName = nodeCredit.SelectSingleNodeFast("FirstName");
              XmlNode nodeLastName = nodeCredit.SelectSingleNodeFast("LastName");
              XmlNode nodeType = nodeCredit.SelectSingleNodeFast("CreditSubtype");
              if (nodeType != null && nodeType.InnerText != null && nodeType.InnerText == "Director")
              {
                if (Director.Length > 0) Director += ", ";
                if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                Director += String.Format("{0} {1}", firstname, lastname);
              }
              if (nodeType != null && nodeType.InnerText != null && nodeType.InnerText == "Producer")
              {
                if (Producer.Length > 0) Producer += ", ";
                if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                Producer += String.Format("{0} {1}", firstname, lastname);
              }
              if (nodeType != null && nodeType.InnerText != null && nodeType.InnerText == "Screenwriter")
              {
                if (Writer.Length > 0) Writer += ", ";
                if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                Writer += String.Format("{0} {1}", firstname, lastname);
              }
            }
          }
          string Image = folderimage + @"\" + nodeID.InnerText.Trim() + "f.jpg";
          if (nodeMediaType != null)
          {
            XmlNode nodeRating = nodeDvd.SelectSingleNodeFast("Review");
            if (nodeRating.Attributes["Film"] != null && nodeRating.Attributes["Film"].Value != null)
              Rating = nodeRating.Attributes["Film"].Value + ".0";
            else
              Rating = "0.0";
          }
          else
          {
            XmlNode nodeRating = nodeDvd.SelectSingleNodeFast("Review/ReviewFilm");
            if (nodeRating != null && nodeRating.InnerText != null)
              Rating = nodeRating.InnerText + ".0";
            else
              Rating = "0.0";
          }

          string Certification = string.Empty;
          XmlNode nodeCertification = nodeDvd.SelectSingleNodeFast("Rating");
          if (nodeCertification != null && nodeCertification.InnerText != null)
            Certification = nodeCertification.InnerText;

          string borrower = String.Empty;
          XmlNodeList borrowerList = nodeDvd.SelectNodes("LoanInfo/User");
          foreach (XmlNode nodeBorrower in borrowerList)
          {
            string firstname = String.Empty;
            string lastname = String.Empty;
            if (nodeBorrower.Attributes["FirstName"] != null && nodeBorrower.Attributes["FirstName"].Value != null) firstname = nodeBorrower.Attributes["FirstName"].Value;
            if (nodeBorrower.Attributes["LastName"] != null && nodeBorrower.Attributes["LastName"].Value != null) lastname = nodeBorrower.Attributes["LastName"].Value;
            string line;
            line = String.Format("{0} {1}", firstname, lastname);
            if (borrower.Length > 0) borrower += ", ";
            borrower += line;
          }

          if (nodeNumber != null && nodeNumber.InnerText != null && nodeNumber.InnerText.Length > 0)
            WriteAntAtribute(destXml, "CollectionNumber", nodeNumber.InnerText);
          else
            WriteAntAtribute(destXml, "CollectionNumber", "9999");
          if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
            WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText);
          else
            WriteAntAtribute(destXml, "Title", nodeTitle.InnerText);
          WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText);
          if (nodeSTitle != null && !string.IsNullOrEmpty(nodeSTitle.InnerText))
            WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
          else
            WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText);
          bool boolWatched = false;
          XmlNodeList eventList = nodeDvd.SelectNodes("Events/Event/EventType");
          foreach (XmlNode eventType in eventList.Cast<XmlNode>().Where(eventType => eventType.InnerText == "Watched"))
          {
            boolWatched = true;
          }
          WriteAntAtribute(destXml, "EventType", boolWatched.ToString());
          //if (nodeDVD.SelectSingleNodeFast("EventType") != null && nodeDVD.SelectSingleNodeFast("EventType").InnerText.Length > 0)
          //  WriteAntAtribute(destXml, "EventType", nodeDVD.SelectSingleNodeFast("EventType").InnerText);

          WriteAntAtribute(destXml, "MediaTypes", mediatype);
          WriteAntAtribute(destXml, "UPC", medialabel);
          WriteAntAtribute(destXml, "Notes/File", File);
          WriteAntAtribute(destXml, "Notes/Country", Country);
          WriteAntAtribute(destXml, "Review/ReviewFilm", Rating);
          if (nodeYear != null)
            WriteAntAtribute(destXml, "Year", nodeYear.InnerText);
          if (nodeDuration != null)
            WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText);
          WriteAntAtribute(destXml, "Genres", genre);
          WriteAntAtribute(destXml, "Credits", Director);
          WriteAntAtribute(destXml, "Credits1", Producer);
          //WriteAntAtribute(destXml, "Credits2", Writer);
          WriteAntAtribute(destXml, "Actors", cast);
          XmlNode nodeDate = nodeDvd.SelectSingleNodeFast("PurchaseInfo/PurchaseDate");
          try
          {
            DateTime dt = new DateTime();
            dt = DateTime.Parse(nodeDate.InnerText);
            WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
          }
          catch
          {
          }
          WriteAntAtribute(destXml, "LoanInfo/User", borrower);
          WriteAntAtribute(destXml, "Picture", Image);
          if ((TagField.Length > 0) && (TagField != "Category") && (TagFullName.Length > 0))
            WriteAntAtribute(destXml, "Tag", TagFullName);

          //WriteAntAtribute(destXml, "Tags", TagFullName);
          WriteAntAtribute(destXml, "AudioContent", languages);
          WriteAntAtribute(destXml, "Subtitle", subtitles);
          //WriteAntAtribute(destXml, "Rating", Certification);

          string DescriptionMerged = string.Empty;
          if (DestinationTagline == "Description")
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += Tagline;
          }
          if (DestinationTags == "Description")
          {
            if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
            DescriptionMerged += TagFullName;
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
            CommentsMerged += TagFullName;
          }
          if (DestinationCertification == "Comments")
          {
            if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
            CommentsMerged += Certification;
          }
          WriteAntAtribute(destXml, "Comments", CommentsMerged);

          // Now writing MF extended attributes
          WriteAntElement(destXml, "Rating", Certification);
          WriteAntElement(destXml, "TAGLINE", Tagline);
          WriteAntElement(destXml, "Tags", TagFullName);
          WriteAntElement(destXml, "Credits2", Writer);

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
