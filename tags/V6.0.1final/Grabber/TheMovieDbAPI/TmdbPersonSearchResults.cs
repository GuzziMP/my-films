﻿namespace Grabber.TheMovieDbAPI
{
  using System.Xml.Serialization;

  [XmlRoot("OpenSearchDescription")]
    public class TmdbPersonSearchResults
    {
        [XmlArray("people")]
        public TmdbPerson[] People { get; set; }
    }
}