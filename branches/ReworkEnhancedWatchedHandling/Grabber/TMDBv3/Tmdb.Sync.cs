﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Net;
using WatTmdb;

namespace WatTmdb.V3
{
    public partial class Tmdb
    {
        private T ProcessRequest<T>(RestRequest request)
            where T : new()
        {
            var client = new RestClient(BASE_URL);
            client.AddHandler("application/json", new WatJsonDeserializer());

            if (Timeout.HasValue)
                client.Timeout = Timeout.Value;

#if !WINDOWS_PHONE
            if (Proxy != null)
                client.Proxy = Proxy;
#endif

            Error = null;

            request.AddHeader("Accept", "application/json");
            request.AddParameter("api_key", ApiKey);

            var resp = client.Execute<T>(request);

            ResponseContent = resp.Content;
            ResponseHeaders = resp.Headers.ToDictionary(k => k.Name, v => v.Value);

            if (resp.ResponseStatus == ResponseStatus.Completed)
                return resp.Data;
            else
            {
                if (resp.Content.Contains("status_message"))
                    Error = jsonDeserializer.Deserialize<TmdbError>(resp);
                else if (resp.ErrorException != null)
                    throw resp.ErrorException;
                else
                    Error = new TmdbError { status_message = resp.ErrorMessage };
            }

            return default(T);
        }

        private string ProcessRequestETag(RestRequest request)
        {
            var client = new RestClient(BASE_URL);
            if (Timeout.HasValue)
                client.Timeout = Timeout.Value;

#if !WINDOWS_PHONE
            if (Proxy != null)
                client.Proxy = Proxy;
#endif
            Error = null;

            request.Method = Method.HEAD;
            request.AddHeader("Accept", "application/json");
            request.AddParameter("api_key", ApiKey);

            var resp = client.Execute(request);
            ResponseContent = resp.Content;
            ResponseHeaders = resp.Headers.ToDictionary(k => k.Name, v => v.Value);

            if (resp.ResponseStatus != ResponseStatus.Completed && resp.ErrorException != null)
                throw resp.ErrorException;

            return this.ResponseETag;
        }


        #region Configuration
        /// <summary>
        /// Retrieve configuration data from TMDB
        /// (http://help.themoviedb.org/kb/api/configuration)
        /// </summary>
        /// <returns></returns>
        public TmdbConfiguration GetConfiguration()
        {
            return ProcessRequest<TmdbConfiguration>(BuildGetConfigurationRequest(null));
        }

        public string GetConfigurationETag()
        {
            return ProcessRequestETag(BuildGetConfigurationRequest(null));
        }

        #endregion


        #region Authentication
        //public TmdbAuthToken GetAuthToken()
        //{
        //    var request = new RestRequest("authentication/token/new", Method.GET);
        //    return ProcessRequest<TmdbAuthToken>(request);
        //}

        //public TmdbAuthSession GetAuthSession(string RequestToken)
        //{
        //    var request = new RestRequest("authentication/session/new", Method.GET);
        //    request.AddParameter("request_token", RequestToken);
        //    return ProcessRequest<TmdbAuthSession>(request);
        //}
        #endregion


        #region Search
        /// <summary>
        /// Search for movies that are listed in TMDB
        /// (http://help.themoviedb.org/kb/api/search-movies)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="includeAdult">optional - include adult items in your search, (Default=false)</param>
        /// <param name="year">optional - to get a closer result</param>
        /// <returns></returns>
        public TmdbMovieSearch SearchMovie(string query, int page, string language, bool? includeAdult, int? year)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Search must be supplied");

            return this.ProcessRequest<TmdbMovieSearch>(BuildSearchMovieRequest(query, page, language, includeAdult, year, null));
        }

        /// <summary>
        /// Search for movies that are listed in TMDB
        /// (http://help.themoviedb.org/kb/api/search-movies)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbMovieSearch SearchMovie(string query, int page)
        {
            return this.SearchMovie(query, page, this.Language, null, null);
        }

        /// <summary>
        /// Search for people that are listed in TMDB.
        /// (http://help.themoviedb.org/kb/api/search-people)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbPersonSearch SearchPerson(string query, int page, string language)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Search must be supplied");

            return this.ProcessRequest<TmdbPersonSearch>(BuildSearchPersonRequest(query, page, language, null));
        }

        /// <summary>
        /// Search for people that are listed in TMDB.
        /// (http://help.themoviedb.org/kb/api/search-people)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbPersonSearch SearchPerson(string query, int page)
        {
            return this.SearchPerson(query, page, this.Language);
        }

        /// <summary>
        /// Search for production companies that are part of TMDB.
        /// (http://help.themoviedb.org/kb/api/search-companies)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbCompanySearch SearchCompany(string query, int page)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Search must be supplied");

            return this.ProcessRequest<TmdbCompanySearch>(BuildSearchCompanyRequest(query, page, null));
        }

        #endregion


        #region Collections
        /// <summary>
        /// Get all of the basic information about a movie collection.
        /// (http://help.themoviedb.org/kb/api/collection-info)
        /// </summary>
        /// <param name="CollectionID">Collection ID, available in TmdbMovie::belongs_to_collection</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbCollection GetCollectionInfo(int CollectionID, string language)
        {
            return this.ProcessRequest<TmdbCollection>(BuildGetCollectionInfoRequest(CollectionID, language, null));
        }

        public string GetCollectionInfoETag(int CollectionID, string language)
        {
            return this.ProcessRequestETag(BuildGetCollectionInfoRequest(CollectionID, language, null));
        }

        /// <summary>
        /// Get all of the basic information about a movie collection.
        /// (http://help.themoviedb.org/kb/api/collection-info)
        /// </summary>
        /// <param name="CollectionID">Collection ID, available in TmdbMovie::belongs_to_collection</param>
        /// <returns></returns>
        public TmdbCollection GetCollectionInfo(int CollectionID)
        {
            return this.GetCollectionInfo(CollectionID, this.Language);
        }

        public string GetCollectionInfoETag(int CollectionID)
        {
            return this.GetCollectionInfoETag(CollectionID, this.Language);
        }

        /// <summary>
        /// Get all the images for a movie collection
        /// http://help.themoviedb.org/kb/api/collection-images
        /// </summary>
        /// <param name="CollectionID">Collection ID, available in TmdbMovie::belongs_to_collection</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbCollectionImages GetCollectionImages(int CollectionID, string language)
        {
            return ProcessRequest<TmdbCollectionImages>(BuildGetCollectionImagesRequest(CollectionID, language, null));
        }

        public string GetCollectionImagesETag(int CollectionID, string language)
        {
            return ProcessRequestETag(BuildGetCollectionImagesRequest(CollectionID, language, null));
        }

        /// <summary>
        /// Get all the images for a movie collection
        /// http://help.themoviedb.org/kb/api/collection-images
        /// </summary>
        /// <param name="CollectionID">Collection ID, available in TmdbMovie::belongs_to_collection</param>
        /// <returns></returns>
        public TmdbCollectionImages GetCollectionImages(int CollectionID)
        {
            return GetCollectionImages(CollectionID, Language);
        }

        public string GetCollectionImagesETag(int CollectionID)
        {
            return GetCollectionImagesETag(CollectionID, Language);
        }
        #endregion


        #region Movie Info
        /// <summary>
        /// Retrieve all the basic movie information for a particular movie by TMDB reference.
        /// (http://help.themoviedb.org/kb/api/movie-info)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbMovie GetMovieInfo(int MovieID, string language)
        {
            return this.ProcessRequest<TmdbMovie>(BuildGetMovieInfoRequest(MovieID, language, null));
        }

        public string GetMovieInfoETag(int MovieID, string language)
        {
            return this.ProcessRequestETag(BuildGetMovieInfoRequest(MovieID, language, null));
        }

        /// <summary>
        /// Retrieve all the basic movie information for a particular movie by TMDB reference.
        /// (http://help.themoviedb.org/kb/api/movie-info)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbMovie GetMovieInfo(int MovieID)
        {
            return this.GetMovieInfo(MovieID, this.Language);
        }

        public string GetMovieInfoETag(int MovieID)
        {
            return this.GetMovieInfoETag(MovieID, this.Language);
        }

        /// <summary>
        /// Retrieve all the basic movie information for a particular movie by IMDB reference.
        /// (http://help.themoviedb.org/kb/api/movie-info)
        /// </summary>
        /// <param name="IMDB_ID">IMDB movie id</param>
        /// <returns></returns>
        public TmdbMovie GetMovieByIMDB(string IMDB_ID)
        {
            if (string.IsNullOrEmpty(IMDB_ID))
                throw new ArgumentException("IMDB_ID must be supplied");

            return this.ProcessRequest<TmdbMovie>(BuildGetMovieByIMDBRequest(IMDB_ID, null));
        }

        /// <summary>
        /// Get list of all the alternative titles for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-alternative-titles)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="Country">ISO 3166-1 country code (optional)</param>
        /// <returns></returns>
        public TmdbMovieAlternateTitles GetMovieAlternateTitles(int MovieID, string Country)
        {
            return this.ProcessRequest<TmdbMovieAlternateTitles>(BuildGetMovieAlternateTitlesRequest(MovieID, Country, null));
        }

        public string GetMovieAlternateTitlesETag(int MovieID, string Country)
        {
            return this.ProcessRequestETag(BuildGetMovieAlternateTitlesRequest(MovieID, Country, null));
        }

        /// <summary>
        /// Get list of all the cast information for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-casts)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbMovieCast GetMovieCast(int MovieID)
        {
            return this.ProcessRequest<TmdbMovieCast>(BuildGetMovieCastRequest(MovieID, null));
        }

        public string GetMovieCastETag(int MovieID)
        {
          return this.ProcessRequestETag(BuildGetMovieCastRequest(MovieID, null));
        }

        /// <summary>
        /// Get list of all the images for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-images)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbMovieImages GetMovieImages(int MovieID, string language)
        {
          return this.ProcessRequest<TmdbMovieImages>(BuildGetMovieImagesRequest(MovieID, language, null));
        }

        public string GetMovieImagesETag(int MovieID, string language)
        {
          return this.ProcessRequestETag(BuildGetMovieImagesRequest(MovieID, language, null));
        }

        /// <summary>
        /// Get list of all the images for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-images)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbMovieImages GetMovieImages(int MovieID)
        {
            return this.GetMovieImages(MovieID, this.Language);
        }

        public string GetMovieImagesETag(int MovieID)
        {
            return this.GetMovieImagesETag(MovieID, this.Language);
        }

        /// <summary>
        /// Get list of all the keywords that have been added to a particular movie.  Only English keywords exist currently.
        /// (http://help.themoviedb.org/kb/api/movie-keywords)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbMovieKeywords GetMovieKeywords(int MovieID)
        {
          return this.ProcessRequest<TmdbMovieKeywords>(BuildGetMovieKeywordsRequest(MovieID, null));
        }

        public string GetMovieKeywordsETag(int MovieID)
        {
          return this.ProcessRequestETag(BuildGetMovieKeywordsRequest(MovieID, null));
        }

        /// <summary>
        /// Get all the release and certification data in TMDB for a particular movie
        /// (http://help.themoviedb.org/kb/api/movie-release-info)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbMovieReleases GetMovieReleases(int MovieID)
        {
          return this.ProcessRequest<TmdbMovieReleases>(BuildGetMovieReleasesRequest(MovieID, null));
        }

        public string GetMovieReleasesETag(int MovieID)
        {
          return this.ProcessRequestETag(BuildGetMovieReleasesRequest(MovieID, null));
        }

        /// <summary>
        /// Get list of trailers for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-trailers)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbMovieTrailers GetMovieTrailers(int MovieID, string language)
        {
          return this.ProcessRequest<TmdbMovieTrailers>(BuildGetMovieTrailersRequest(MovieID, language, null));
        }

        public string GetMovieTrailersETag(int MovieID, string language)
        {
          return this.ProcessRequestETag(BuildGetMovieTrailersRequest(MovieID, language, null));
        }

        /// <summary>
        /// Get list of trailers for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-trailers)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbMovieTrailers GetMovieTrailers(int MovieID)
        {
            return GetMovieTrailers(MovieID, Language);
        }

        public string GetMovieTrailersETag(int MovieID)
        {
            return GetMovieTrailersETag(MovieID, Language);
        }

        /// <summary>
        /// Get list of similar movies for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-similar-movies)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbSimilarMovies GetSimilarMovies(int MovieID, int page, string language)
        {
          return this.ProcessRequest<TmdbSimilarMovies>(BuildGetSimilarMoviesRequest(MovieID, page, language, null));
        }

        /// <summary>
        /// Get list of similar movies for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-similar-movies)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbSimilarMovies GetSimilarMovies(int MovieID, int page)
        {
            return this.GetSimilarMovies(MovieID, page, this.Language);
        }

        /// <summary>
        /// Get list of all available translations for a specific movie.
        /// (http://help.themoviedb.org/kb/api/movie-translations)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        public TmdbTranslations GetMovieTranslations(int MovieID)
        {
          return this.ProcessRequest<TmdbTranslations>(BuildGetMovieTranslationsRequest(MovieID, null));
        }

        public string GetMovieTranslationsETag(int MovieID)
        {
          return this.ProcessRequestETag(BuildGetMovieTranslationsRequest(MovieID, null));
        }
        #endregion


        #region Person Info
        /// <summary>
        /// Get all of the basic information for a person.
        /// (http://help.themoviedb.org/kb/api/person-info)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <returns></returns>
        public TmdbPerson GetPersonInfo(int PersonID)
        {
          return this.ProcessRequest<TmdbPerson>(BuildGetPersonInfoRequest(PersonID, null));
        }

        public string GetPersonInfoETag(int PersonID)
        {
          return this.ProcessRequestETag(BuildGetPersonInfoRequest(PersonID, null));
        }

        /// <summary>
        /// Get list of cast and crew information for a person.
        /// (http://help.themoviedb.org/kb/api/person-credits)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbPersonCredits GetPersonCredits(int PersonID, string language)
        {
          return this.ProcessRequest<TmdbPersonCredits>(BuildGetPersonCreditsRequest(PersonID, language, null));
        }

        public string GetPersonCreditsETag(int PersonID, string language)
        {
          return this.ProcessRequestETag(BuildGetPersonCreditsRequest(PersonID, language, null));
        }

        /// <summary>
        /// Get list of cast and crew information for a person.
        /// (http://help.themoviedb.org/kb/api/person-credits)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <returns></returns>
        public TmdbPersonCredits GetPersonCredits(int PersonID)
        {
            return this.GetPersonCredits(PersonID, this.Language);
        }

        public string GetPersonCreditsETag(int PersonID)
        {
            return this.GetPersonCreditsETag(PersonID, this.Language);
        }

        /// <summary>
        /// Get list of images for a person.
        /// (http://help.themoviedb.org/kb/api/person-images)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <returns></returns>
        public TmdbPersonImages GetPersonImages(int PersonID)
        {
            return this.ProcessRequest<TmdbPersonImages>(BuildGetPersonImagesRequest(PersonID, null));
        }

        public string GetPersonImagesETag(int PersonID)
        {
            return this.ProcessRequestETag(BuildGetPersonImagesRequest(PersonID, null));
        }
        #endregion


        #region Miscellaneous Movie
        /// <summary>
        /// Get the newest movie added to the TMDB.
        /// (http://help.themoviedb.org/kb/api/latest-movie)
        /// </summary>
        /// <returns></returns>
        public TmdbLatestMovie GetLatestMovie()
        {
            return ProcessRequest<TmdbLatestMovie>(BuildGetLatestMovieRequest(null));
        }

        /// <summary>
        /// Get the list of movies currently in theatres.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/now-playing-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbNowPlaying GetNowPlayingMovies(int page, string language)
        {
            return ProcessRequest<TmdbNowPlaying>(BuildGetNowPlayingMoviesRequest(page, language, null));
        }

        /// <summary>
        /// Get the list of movies currently in theatres.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/now-playing-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbNowPlaying GetNowPlayingMovies(int page)
        {
            return GetNowPlayingMovies(page, Language);
        }

        /// <summary>
        /// Get the daily popularity list of movies.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/popular-movie-list)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbPopular GetPopularMovies(int page, string language)
        {
          return this.ProcessRequest<TmdbPopular>(BuildGetPopularMoviesRequest(page, language, null));
        }

        /// <summary>
        /// Get the daily popularity list of movies.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/popular-movie-list)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbPopular GetPopularMovies(int page)
        {
            return GetPopularMovies(page, Language);
        }

        /// <summary>
        /// Get list of movies that have over 10 votes on TMDB.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/top-rated-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbTopRated GetTopRatedMovies(int page, string language)
        {
          return this.ProcessRequest<TmdbTopRated>(BuildGetTopRatedMoviesRequest(page, language, null));
        }

        /// <summary>
        /// Get list of movies that have over 10 votes on TMDB.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/top-rated-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbTopRated GetTopRatedMovies(int page)
        {
            return GetTopRatedMovies(page, Language);
        }

        /// <summary>
        /// Get list of movies that are arriving to theatres in the next few weeks.
        /// (http://help.themoviedb.org/kb/api/upcoming-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbUpcoming GetUpcomingMovies(int page, string language)
        {
          return this.ProcessRequest<TmdbUpcoming>(BuildGetUpcomingMoviesRequest(page, language, null));
        }

        /// <summary>
        /// Get list of movies that are arriving to theatres in the next few weeks.
        /// (http://help.themoviedb.org/kb/api/upcoming-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbUpcoming GetUpcomingMovies(int page)
        {
            return GetUpcomingMovies(page, Language);
        }
        #endregion


        #region Company Info
        /// <summary>
        /// Get basic information about a production company from TMDB.
        /// (http://help.themoviedb.org/kb/api/company-info)
        /// </summary>
        /// <param name="CompanyID">Company ID</param>
        /// <returns></returns>
        public TmdbCompany GetCompanyInfo(int CompanyID)
        {
          return this.ProcessRequest<TmdbCompany>(BuildGetCompanyInfoRequest(CompanyID, null));
        }

        public string GetCompanyInfoETag(int CompanyID)
        {
          return this.ProcessRequestETag(BuildGetCompanyInfoRequest(CompanyID, null));
        }

        /// <summary>
        /// Get list of movies associated with a company.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/company-movies)
        /// </summary>
        /// <param name="CompanyID">Company ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbCompanyMovies GetCompanyMovies(int CompanyID, int page, string language)
        {
          return this.ProcessRequest<TmdbCompanyMovies>(BuildGetCompanyMoviesRequest(CompanyID, page, language, null));
        }

        public string GetCompanyMoviesETag(int CompanyID, int page, string language)
        {
          return this.ProcessRequestETag(BuildGetCompanyMoviesRequest(CompanyID, page, language, null));
        }

        /// <summary>
        /// Get list of movies associated with a company.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/company-movies)
        /// </summary>
        /// <param name="CompanyID">Company ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbCompanyMovies GetCompanyMovies(int CompanyID, int page)
        {
            return GetCompanyMovies(CompanyID, page, Language);
        }

        public string GetCompanyMoviesETag(int CompanyID, int page)
        {
            return GetCompanyMoviesETag(CompanyID, page, Language);
        }
        #endregion


        #region Genre Info
        /// <summary>
        /// Get list of genres used in TMDB.  The ids will correspond to those found in movie calls.
        /// (http://help.themoviedb.org/kb/api/genre-list)
        /// </summary>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbGenre GetGenreList(string language)
        {
          return this.ProcessRequest<TmdbGenre>(BuildGetGenreListRequest(language, null));
        }

        public string GetGenreListETag(string language)
        {
          return this.ProcessRequestETag(BuildGetGenreListRequest(language, null));
        }

        /// <summary>
        /// Get list of genres used in TMDB.  The ids will correspond to those found in movie calls.
        /// (http://help.themoviedb.org/kb/api/genre-list)
        /// </summary>
        /// <returns></returns>
        public TmdbGenre GetGenreList()
        {
            return GetGenreList(Language);
        }

        public string GetGenreListETag()
        {
            return GetGenreListETag(Language);
        }

        /// <summary>
        /// Get list of movies in a Genre.  Note that only movies with more than 10 votes get listed.
        /// (http://help.themoviedb.org/kb/api/genre-movies)
        /// </summary>
        /// <param name="GenreID">TMDB Genre ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <returns></returns>
        public TmdbGenreMovies GetGenreMovies(int GenreID, int page, string language)
        {
          return this.ProcessRequest<TmdbGenreMovies>(BuildGetGenreMoviesRequest(GenreID, page, language, null));
        }

        public string GetGenreMoviesETag(int GenreID, int page, string language)
        {
          return this.ProcessRequestETag(BuildGetGenreMoviesRequest(GenreID, page, language, null));
        }

        /// <summary>
        /// Get list of movies in a Genre.  Note that only movies with more than 10 votes get listed.
        /// (http://help.themoviedb.org/kb/api/genre-movies)
        /// </summary>
        /// <param name="GenreID">TMDB Genre ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <returns></returns>
        public TmdbGenreMovies GetGenreMovies(int GenreID, int page)
        {
            return GetGenreMovies(GenreID, page, Language);
        }

        public string GetGenreMoviesETag(int GenreID, int page)
        {
            return GetGenreMoviesETag(GenreID, page, Language);
        }
        #endregion
    }
}