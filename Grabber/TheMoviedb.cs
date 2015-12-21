using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using WatTmdb.V3;
using NLog;

namespace Grabber
{
  public class TheMoviedb
  {
    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();

    private const string TmdbApiKey = "1e66c0cc99696feaf2ea56695e134eae";
    private const string ApiSearchMovie = "http://api.themoviedb.org/2.1/Movie.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    private const string ApiSearchMovieByImdb = "http://api.themoviedb.org/2.1/Movie.imdbLookup/en/xml/1e66c0cc99696feaf2ea56695e134eae/"; // add tt-nbr to search for movie ... tt0137523
    private const string ApiGetMovieInfo = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    private const string ApiGetPersonInfo = "http://api.themoviedb.org/2.1/Person.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";

    public List<DbMovieInfo> GetMoviesByTitles(string title, string ttitle, int year, string director, string imdbid, string tmdbid, bool choose, string language)
    {
      List<DbMovieInfo> results = GetMoviesByTitle(title, year, director, imdbid, tmdbid, choose, language);
      if (results.Count == 1)
        return results;
      List<DbMovieInfo> results2 = GetMoviesByTitle(ttitle, year, director, imdbid, tmdbid, choose, language);
      return results2.Count == 1 ? results2 : results;
    }

    private List<DbMovieInfo> GetMoviesByTitle(string title, int year, string director, string imdbid, string tmdbid, bool choose, string language)
    {
      LogMyFilms.Debug("GetMoviesByTitle - title = '" + title + "', year = '" + year + "', imdbid = '" + imdbid + "', tmdbid = '" + tmdbid + "', choose = '" + choose + "', language = '" + language + "'");

      //title = Grabber.GrabUtil.normalizeTitle(title);

      //string apiSearchLanguage = ApiSearchMovie;
      //string apiGetMovieInfoLanguage = ApiGetMovieInfo;
      //if (language.Length == 2)
      //{
      //  apiSearchLanguage = ApiSearchMovie.Replace("/en/", "/" + language + "/");
      //  apiGetMovieInfoLanguage = ApiGetMovieInfo.Replace("/en/", "/" + language + "/");
      //}
      //else
      //{
      //  apiSearchLanguage = CultureInfo.CurrentCulture.Name.Substring(0, 2); // use local language instead 
      //  apiGetMovieInfoLanguage = CultureInfo.CurrentCulture.Name.Substring(0, 2); // use local language instead 
      //  language = CultureInfo.CurrentCulture.Name.Substring(0, 2); // use local language instead 
      //}

      if (language.Length != 2)
        language = CultureInfo.CurrentCulture.Name.Substring(0, 2); // use local language instead 

      List<DbMovieInfo> results = new List<DbMovieInfo>();
      List<DbMovieInfo> resultsdet = new List<DbMovieInfo>();
      // XmlNodeList xml = null; // old API2.1 xml structure
      List<TmdbMovie> movies = new List<TmdbMovie>();


      //Tmdb api = new Tmdb(TmdbApiKey, CultureInfo.CurrentCulture.Name.Substring(0, 2)); // language is optional, default is "en"
      //TmdbConfiguration tmdbConf = api.GetConfiguration();
      //TmdbMovieSearch tmdbMovies = api.SearchMovie(searchname, 0, language, 2012);
      //TmdbPersonSearch tmdbPerson = api.SearchPerson(personname, 1);
      //List<MovieResult> persons = tmdbMovies.results;
      //if (persons != null && persons.Count > 0)
      //{
      //  PersonResult pinfo = persons[0];
      //  TmdbPerson singleperson = api.GetPersonInfo(pinfo.id);
      //  // TMDB.TmdbPersonImages images = api.GetPersonImages(pinfo.id);
      //  // TMDB.TmdbPersonCredits personFilmList = api.GetPersonCredits(pinfo.id);
      //}



      Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      // TmdbConfiguration tmdbConf = api.GetConfiguration();

      try
      {
        if (!string.IsNullOrEmpty(imdbid) && imdbid.Contains("tt"))
        {
          TmdbMovie movie = api.GetMovieByIMDB(imdbid);
          if (movie.id > 0)
          {
            results.Add(GetMovieInformation(api, movie, language));
            return results;
          }
        }

        TmdbMovieSearch moviesfound;
        if (year > 0)
        {
          moviesfound = api.SearchMovie(title, 1, "", null, year);
          if (moviesfound.results.Count == 0) moviesfound = api.SearchMovie(title, 1, language);
          movies.AddRange(moviesfound.results);
        }
        else
        {
          int ipage = 1;
          while (true)
          {
            moviesfound = api.SearchMovie(title, 1, null);
            movies.AddRange(moviesfound.results);
            ipage++;
            if (ipage > moviesfound.total_pages) break;
          }
          movies = movies.OrderBy(x => x.release_date).ToList(); // .AsEnumerable()
        }

        if (movies.Count == 1)
        {
          results.Add(GetMovieInformation(api, movies[0], language));
          return results;
        }
        else
        {
          foreach (TmdbMovie movieResult in movies)
          {
            DbMovieInfo movie = GetMovieInformation(api, movieResult, language);
            if (movie != null && GrabUtil.normalizeTitle(movie.Name.ToLower()).Contains(GrabUtil.normalizeTitle(title.ToLower())))
              if (year > 0 && movie.Year > 0 && !choose)
              {
                if ((year >= movie.Year - 2) && (year <= movie.Year + 2))
                  results.Add(movie);
              }
              else
                results.Add(movie);
          }
          return results;
        }

      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex.StackTrace);
      }

      #region old TMDB APIv2.1 code
      //if (!string.IsNullOrEmpty(imdbid))
      //  xml = GetXml(ApiSearchMovieByImdb + imdbid);
      //if (xml == null) // if imdb search was unsuccessful use normal search...
      //  xml = GetXml(apiSearchLanguage + Grabber.GrabUtil.RemoveDiacritics(title.Trim().ToLower()).Replace(" ", "+"));

      //if (xml == null)
      //  return results;

      //XmlNodeList movieNodes = xml.Item(0).SelectNodes("//movie");
      //foreach (XmlNode node in movieNodes)
      //{
      //  DbMovieInfo movie = GetMovieInformation(node);
      //  if (movie != null && Grabber.GrabUtil.normalizeTitle(movie.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(title.ToLower())))
      //    if (year > 0 && movie.Year > 0 && !choose)
      //    {
      //      if ((year >= movie.Year - 2) && (year <= movie.Year + 2))
      //        results.Add(movie);
      //    }
      //    else
      //      results.Add(movie);
      //}


      //if (results.Count > 0)
      //{
      //  // Replace non-descriptive characters with spaces
      //  director = System.Text.RegularExpressions.Regex.Replace(director, "( et | and | & | und )", ",");
      //  if (director.IndexOf(",", System.StringComparison.Ordinal) > 0) 
      //    director = director.Substring(0, director.IndexOf(",", System.StringComparison.Ordinal));
      //  foreach (DbMovieInfo movie in results.Where(movie => movie.Identifier != null))
      //  {
      //    try { xml = GetXml(apiGetMovieInfoLanguage + movie.Identifier); }
      //    catch { xml = null; }
      //    if (xml != null)
      //    {
      //      movieNodes = xml.Item(0).SelectNodes("//movie");
      //      foreach (DbMovieInfo movie2 in from XmlNode node in movieNodes select GetMovieInformation(node))
      //      {
      //        if (movie2 != null && Grabber.GrabUtil.normalizeTitle(movie2.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(title.ToLower())) && movie2.Directors.Contains(director))
      //          if (year > 0 && movie2.Year > 0 && !choose)
      //          {
      //            if ((year >= movie2.Year - 2) && (year <= movie2.Year + 2))
      //              resultsdet.Add(movie2);
      //          }
      //          else
      //            resultsdet.Add(movie2);
      //        else
      //          if (choose)
      //            resultsdet.Add(movie2);
      //      }
      //    }
      //  }
      //}
      #endregion

      return resultsdet.Count > 0 ? resultsdet : results;
    }

    public DbPersonInfo GetPersonsById(string id, string language)
    {
      Tmdb api = new Tmdb(TmdbApiKey, language); // language is optional, default is "en"

      if (language.Length != 2)
        language = CultureInfo.CurrentCulture.Name.Substring(0, 2); // use local language instead 

      TmdbPerson singleperson = api.GetPersonInfo(int.Parse(id));

      DbPersonInfo result = GetPersonInformation(api, singleperson, language);
      return result;
      
      //string apiGetPersonInfoLanguage = language.Length == 2 ? ApiGetPersonInfo.Replace("/en/", "/" + language + "/") : ApiGetPersonInfo;
      
      //XmlNodeList xml = GetXml(apiGetPersonInfoLanguage + id);
      //if (xml == null) return result;

      //XmlNodeList personNodes = xml.Item(0).SelectNodes("//person");
      //foreach (DbPersonInfo person in personNodes.Cast<XmlNode>().Select(node => GetPersonInformation(node)).Where(person => person != null))
      //{
      //  result = person;
      //}
      //return result;
    }

    private XmlNodeList GetXml(string url)
    {
      // given a url, retrieves the xml result set and returns the nodelist of Item objects
      WebGrabber grabber = new WebGrabber(url)
      {
        MaxRetries = 10,
        Timeout = 5000,
        TimeoutIncrement = 1000,
        Encoding = Encoding.UTF8
      };
      return grabber.GetResponse() ? grabber.GetXML() : null;
    }

    private static DbMovieInfo GetMovieInformation(Tmdb api, TmdbMovie movieNode, string language)
    {
      LogMyFilms.Debug("GetMovieInformation() - language = '" + (language ?? "") + "'");

      if (movieNode == null) return null;
      DbMovieInfo movie = new DbMovieInfo();

      try
      {
        TmdbMovie m = api.GetMovieInfo(movieNode.id);

        movie.Identifier = m.id.ToString();
        movie.ImdbId = m.imdb_id;
        movie.Name = m.original_title;
        movie.TranslatedTitle = m.title;
        movie.AlternativeTitle = m.original_title;
        DateTime date;
        if (DateTime.TryParse(m.release_date, out date))
          movie.Year = date.Year;
        movie.DetailsUrl = m.homepage;
        movie.Summary = m.overview;
        movie.Score = (float)Math.Round(m.vote_average, 1);
        // movie.Certification = "";
        foreach (SpokenLanguage spokenLanguage in m.spoken_languages)
        {
          movie.Languages.Add(spokenLanguage.name);
        }
        movie.Runtime = m.runtime;

        TmdbMovieCast p = api.GetMovieCast(movieNode.id);

        foreach (Cast cast in p.cast)
        {
          string name = cast.name;
          string character = cast.character;
          DbPersonInfo personToAdd = new DbPersonInfo { Id = cast.id.ToString(), Name = cast.name, DetailsUrl = cast.profile_path };
          movie.Persons.Add(personToAdd);

          if (character.Length > 0) name = name + " (" + character + ")";
          movie.Actors.Add(name);
        }

        foreach (Crew crew in p.crew)
        {
          DbPersonInfo personToAdd = new DbPersonInfo { Id = crew.id.ToString(), Name = crew.name, DetailsUrl = crew.profile_path };
          movie.Persons.Add(personToAdd);
          switch (crew.department)
          {
            case "Production":
              movie.Producers.Add(crew.name);
              break;
            case "Directing":
              movie.Directors.Add(crew.name);
              break;
            case "Writing":
              movie.Writers.Add(crew.name);
              break;
            case "Sound":
            case "Camera":
              break;
          }
        }

        foreach (Cast cast in p.cast)
        {
          string name = cast.name;
          string character = cast.character;
          string thumb = cast.profile_path;
          string job = cast.character;
          string id = cast.id.ToString();
          string url = cast.profile_path;
          var personToAdd = new DbPersonInfo { Id = id, Name = name, DetailsUrl = url, Job = job };
          movie.Persons.Add(personToAdd);
          switch (job)
          {
            case "Producer":
              movie.Producers.Add(name);
              break;
            case "Director":
              movie.Directors.Add(name);
              break;
            case "Actor":
              if (character.Length > 0)
                name = name + " (" + character + ")";
              movie.Actors.Add(name);
              break;
            case "Screenplay":
              movie.Writers.Add(name);
              break;
          }
        }
        foreach (ProductionCountry country in m.production_countries)
        {
          movie.Country.Add(country.name);
        }
        foreach (MovieGenre genre in m.genres)
        {
          movie.Country.Add(genre.name);
        }

        TmdbConfiguration tmdbConf = api.GetConfiguration();

        // load posters
        TmdbMovieImages movieImages = api.GetMovieImages(movieNode.id, language);
        LogMyFilms.Debug("GetMovieInformation() - language = '" + (language ?? "") + "', Posters: '" + movieImages.posters.Count + "', Backdrops: '" + movieImages.backdrops.Count + "'");

        foreach (Poster poster in movieImages.posters)
        {
          movie.Posters.Add(tmdbConf.images.base_url + "w500" + poster.file_path);
        }
        foreach (Backdrop backdrop in movieImages.backdrops)
        {
          movie.Backdrops.Add(tmdbConf.images.base_url + "original" + backdrop.file_path);
        }

        // english posters and backdrops
        movieImages = api.GetMovieImages(movieNode.id, "en"); // fallback to en language images
        LogMyFilms.Debug("GetMovieInformation() - language = 'en', Posters: '" + movieImages.posters.Count + "', Backdrops: '" + movieImages.backdrops.Count + "'");
        if (movie.Posters.Count < 5)
        {
          foreach (Poster poster in movieImages.posters)
          {
            movie.Posters.Add(tmdbConf.images.base_url + "w500" + poster.file_path);
          }
        }
        foreach (Backdrop backdrop in movieImages.backdrops)
        {
          movie.Backdrops.Add(tmdbConf.images.base_url + "original" + backdrop.file_path);
        }

        // non language posters and backdrops
        movieImages = api.GetMovieImages(movieNode.id, null); // fallback to non language images
        LogMyFilms.Debug("GetMovieInformation() - language = 'null', Posters: '" + movieImages.posters.Count + "', Backdrops: '" + movieImages.backdrops.Count + "'");
        if (movie.Posters.Count < 11)
        {
          foreach (Poster poster in movieImages.posters)
          {
            movie.Posters.Add(tmdbConf.images.base_url + "w500" + poster.file_path);
          }
        }

        if (movie.Backdrops.Count < 11) // only load foreign backdrops, if not anough are availabole
        {
          foreach (Backdrop backdrop in movieImages.backdrops)
          {
            movie.Backdrops.Add(tmdbConf.images.base_url + "original" + backdrop.file_path);
          }
        }
        LogMyFilms.Debug("GetMovieInformation() - Totals added - Posters: '" + movie.Posters.Count + "', Backdrops: '" + movie.Backdrops.Count + "'");
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex.StackTrace);
      }
      return movie;
    }

    private static DbMovieInfo GetMovieInformation(XmlNode movieNode)
    {
      if (movieNode == null)
        return null;

      if (movieNode.ChildNodes.Count < 2 || movieNode.Name != "movie")
        return null;

      var producers = new List<string>();
      var directors = new List<string>();
      var writers = new List<string>();
      var actors = new List<string>();
      var backdrops = new List<string>();
      var posters = new List<string>();
      var persons = new List<DbPersonInfo>();
      var movie = new DbMovieInfo();
      foreach (XmlNode node in movieNode.ChildNodes)
      {
        string value = node.InnerText;
        switch (node.Name)
        {
          case "id":
            movie.Identifier = value;
            break;
          case "name":
          case "title":
            movie.Name = value;
            break;
          case "alternative_name":
            if (value.Trim() != "None found." && value.Trim().Length > 0)
              movie.AlternativeTitle = value;
            break;
          case "original_name":
            if (value.Trim() != "None found." && value.Trim().Length > 0)
              movie.TranslatedTitle = value;
            break;
          case "released":
          case "release":
            DateTime date;
            if (DateTime.TryParse(value, out date))
              movie.Year = date.Year;
            break;
          case "imdb":
          case "imdb_id":
            movie.ImdbId = value;
            break;
          case "url":
            movie.DetailsUrl = value;
            break;
          case "overview":
          case "short_overview":
            movie.Summary = value;
            break;
          case "rating":
          case "score":
            float rating;
            if (float.TryParse(value, out rating))
              movie.Score = rating;
            break;
          case "certification":
            movie.Certification = value;
            break;
          case "languages_spoken":
            foreach (XmlNode language in node.SelectNodes("language_spoken/name"))
            {
              movie.Languages.Add(language.InnerText.Trim());
            }
            break;
          case "runtime":
            int runtime;
            if (int.TryParse(value, out runtime))
              movie.Runtime = runtime;
            break;
          case "people":
          case "cast":
            foreach (XmlNode person in node.ChildNodes)
            {
              string name = person.Attributes["name"].Value;
              string character = person.Attributes["character"].Value;
              string thumb = person.Attributes["thumb"].Value;
              string job = person.Attributes["job"].Value;
              string id = person.Attributes["id"].Value;
              string url = person.Attributes["url"].Value;
              var personToAdd = new DbPersonInfo { Id = id, Name = name, DetailsUrl = url, Job = job };
              persons.Add(personToAdd);
              switch (job)
              {
                case "Producer":
                  producers.Add(name);
                  break;
                case "Director":
                  directors.Add(name);
                  break;
                case "Actor":
                  if (character.Length > 0)
                    name = name + " (" + character + ")";
                  actors.Add(name);
                  break;
                case "Screenplay":
                  writers.Add(name);
                  break;
              }
            }
            break;
          case "countries":
            foreach (XmlNode country in node.SelectNodes("country/name"))
            {
              movie.Country.Add(country.InnerText.Trim());
            }
            break;
          case "categories":
            foreach (XmlNode category in node.SelectNodes("category/name"))
            {
              movie.Genres.Add(category.InnerText.Trim());
            }
            break;
          case "poster":
            if (node.OuterXml.Contains("\"original\""))
              posters.Add(value);
            break;
          case "backdrop":
            if (node.OuterXml.Contains("\"original\""))
              backdrops.Add(value);
            break;
          case "images":
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"backdrop\"") && node.OuterXml.Contains("url="))
            {
              backdrops.AddRange(from XmlNode image in node.SelectNodes("image") where image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"backdrop\"") && image.OuterXml.Contains("url=") let start = image.OuterXml.IndexOf("url=") + 5 select image.OuterXml.Substring(start) into zvalue select zvalue.Substring(0, zvalue.IndexOf("\"")));
            }
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"poster\"") && node.OuterXml.Contains("url="))
            {
              posters.AddRange(from XmlNode image in node.SelectNodes("image") where image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"poster\"") && image.OuterXml.Contains("url=") let start = image.OuterXml.IndexOf("url=") + 5 select image.OuterXml.Substring(start) into zvalue select zvalue.Substring(0, zvalue.IndexOf("\"")));
            }
            break;
        }
      }
      movie.Producers = producers;
      movie.Directors = directors;
      movie.Writers = writers;
      movie.Actors = actors;
      movie.Backdrops = backdrops;
      movie.Posters = posters;
      movie.Persons = persons;
      return movie;
    }

    private static DbPersonInfo GetPersonInformation(Tmdb api, TmdbPerson personNode, string language)
    {
      LogMyFilms.Debug("GetPersonInformation()");

      if (personNode == null) return null;

      List<string> images = new List<string>();
      DbPersonInfo person = new DbPersonInfo();

      try
      {
        TmdbPerson m = api.GetPersonInfo(personNode.id);

        person.Id = m.id.ToString();
        person.Name = m.name;
        person.Biography = m.biography;
        person.Birthday = m.birthday;
        person.Birthplace = m.place_of_birth;
        person.DetailsUrl = m.homepage;

        TmdbPersonCredits p = api.GetPersonCredits(personNode.id);

        foreach (CastCredit cast in p.cast)
        {
        }

        foreach (CrewCredit crew in p.crew)
        {
        }

        TmdbConfiguration tmdbConf = api.GetConfiguration();

        TmdbPersonImages personImages = api.GetPersonImages(personNode.id);
        foreach (PersonImageProfile imageProfile in personImages.profiles)
        {
          person.Images.Add(tmdbConf.images.base_url + "w500" + imageProfile.file_path);
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex.StackTrace);
      }
      return person;
    }

    private static DbPersonInfo GetPersonInformation(XmlNode personNode)
    {
      if (personNode == null)
        return null;

      if (personNode.ChildNodes.Count < 2 || personNode.Name != "person")
        return null;

      List<string> images = new List<string>();
      DbPersonInfo person = new DbPersonInfo();
      foreach (XmlNode node in personNode.ChildNodes)
      {
        string value = node.InnerText;
        switch (node.Name)
        {
          case "id":
            person.Id = value;
            break;
          case "name":
            person.Name = value;
            break;
          case "birthday":
            person.Birthday = value;
            break;
          case "birthplace":
            person.Birthplace = value;
            break;
          case "url":
            person.DetailsUrl = value;
            break;
          case "biography":
            person.Biography = value;
            break;
          case "image":
            if (node.OuterXml.Contains("\"original\""))
              images.Add(value);
            break;
          case "version":
          case "last_modified_at":
            break;
          case "images":
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"profile\"") && node.OuterXml.Contains("url="))
            {
              images.AddRange(from XmlNode image in node.SelectNodes("image") where image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"profile\"") && image.OuterXml.Contains("url=") let start = image.OuterXml.IndexOf("url=") + 5 select image.OuterXml.Substring(start) into zvalue select zvalue.Substring(0, zvalue.IndexOf("\"")));
            }
            break;
        }
      }
      person.Images = images;
      return person;
    }
  }
}
