﻿namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class GenreMovie
    {
        public string backdrop_path { get; set; }
        public int id { get; set; }
        public string original_title { get; set; }
        public string release_date { get; set; }
        public string poster_path { get; set; }
        public string title { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class TmdbGenreMovies
    {
        public int id { get; set; }
        public int page { get; set; }
        public List<GenreMovie> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}