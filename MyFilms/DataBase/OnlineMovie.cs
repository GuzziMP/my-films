using WatTmdb.V3;

namespace MyFilmsPlugin.DataBase
{
  

  public class OnlineMovie
  {
    public AntMovieCatalog.MovieRow AntMovie  { get; set; }
    public TmdbMovieSearchResult MovieSearchResult { get; set; }
    public TmdbMovie Movie { get; set; }
    public TmdbMovieTrailers Trailers { get; set; }
    public TmdbMovieAlternateTitles AlternateTitles { get; set; }
    public TmdbMovieImages MovieImages { get; set; }
    public TmdbMovieCast MovieCast { get; set; }



  }
}
