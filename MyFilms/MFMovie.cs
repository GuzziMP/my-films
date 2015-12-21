namespace MyFilmsPlugin.MyFilms
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using MediaPortal.GUI.Library;

  /// <summary>
  /// Movie object
  /// </summary>  
  public class MFMovie
  {
    public MFMovie()
    {
      //MovieRow = null;
      //AllowLatestMediaAPI = false;
      //AllowTrakt = false;
      Category = string.Empty;
      Year = 1900;
      TMDBNumber = string.Empty;
      IMDBNumber = string.Empty;
      Path = string.Empty;
      Trailer = string.Empty;
      File = string.Empty;
      Edition = string.Empty;
      GroupName = string.Empty;
      FormattedTitle = string.Empty;
      TranslatedTitle = string.Empty;
      Title = string.Empty;
      WatchedCount = -1;
      CategoryTrakt = new List<string>();
      Length = 0;
      DateTime = DateTime.Today;
      DateAdded = string.Empty;
      Picture = string.Empty;
      Fanart = string.Empty;
      Config = string.Empty;
      Username = string.Empty;
      ReadOnly = false;
      ID = -1;
      Actors = new List<string>();
      Directors = new List<string>();
      Writers = new List<string>();
      Producers = new List<string>();
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #region public vars

    public int ID { get; set; }

    public bool IsEmpty
    {
      get
      {
        return (Title == string.Empty) || (Title == Strings.Unknown);
      }
    }

    public bool Watched { get; set; }
    public int WatchedCount { get; set; }
    public string Title { get; set; }
    public string TranslatedTitle { get; set; }
    public string FormattedTitle { get; set; }
    public string GroupName { get; set; }
    public string Edition { get; set; }
    public string File { get; set; }
    public string Trailer { get; set; }
    public string Path { get; set; }
    public string IMDBNumber { get; set; }
    public string TMDBNumber { get; set; }
    public int Year { get; set; }
    public string Category { get; set; }
    /// <summary>
    /// entries for watchlist, recommendations and user lists.
    /// </summary>
    public List<string> CategoryTrakt { get; set; }
    /// <summary>
    /// Runtime in minutes.
    /// </summary>
    public int Length { get; set; }
    public float Rating { get; set; }
    public float RatingUser { get; set; }
    public DateTime DateTime { get; set; }
    public string DateAdded { get; set; }
    public string Picture { get; set; }
    public string Fanart { get; set; }
    public string Config { get; set; }
    public string Username { get; set; }
    public bool ReadOnly { get; set; }
    public List<string> Actors { get; set; }
    public List<string> Directors { get; set; }
    public List<string> Writers { get; set; }
    public List<string> Producers { get; set; }

    //public bool AllowTrakt { get; set; }
    //public bool AllowLatestMediaAPI { get; set; }
    //public DataRow MovieRow { get; set; }

    #endregion

    public void Reset()
    {
      Title = string.Empty;
      TranslatedTitle = string.Empty;
      FormattedTitle = string.Empty;
      GroupName = string.Empty;
      CategoryTrakt.Clear();
      Edition = string.Empty;
      IMDBNumber = string.Empty;
      TMDBNumber = string.Empty;
      Year = 1900;
      Category = string.Empty;
      Length = 0;
      Rating = 0.0f;
      RatingUser = 0.0f;
      Watched = false;
      WatchedCount = -1;
      DateTime = DateTime.Today;
      DateAdded = string.Empty;
      File = string.Empty;
      Trailer = string.Empty;
      Path = string.Empty;
      Picture = string.Empty;
      Fanart = string.Empty;
      Config = string.Empty;
      Username = string.Empty;
      ReadOnly = false;
      Actors.Clear();
      Directors.Clear();
      Writers.Clear();
      Producers.Clear();
      //AllowTrakt = false;
      //AllowLatestMediaAPI = false;
      //MovieRow = null;
    }

    private MFMovie GetCurrentMovie()
    {
      var movie = new MFMovie
        {
          ID = ID,
          Title = Title,
          TranslatedTitle = TranslatedTitle,
          FormattedTitle = FormattedTitle,
          GroupName = GroupName,
          CategoryTrakt = CategoryTrakt,
          Edition = Edition,
          IMDBNumber = IMDBNumber,
          TMDBNumber = TMDBNumber,
          Year = Year,
          Category = Category,
          Length = Length,
          Rating = Rating,
          RatingUser = RatingUser,
          Watched = Watched,
          WatchedCount = WatchedCount,
          DateTime = DateTime,
          DateAdded = DateAdded,
          File = File,
          Trailer = Trailer,
          Path = Path,
          Picture = Picture,
          Fanart = Fanart,
          Config = Config,
          Username = Username,
          ReadOnly = ReadOnly
        };
      return movie;
    }

    public void Commit()
    {
      lock (BaseMesFilms.MovieUpdateQueue)
      {
        MFMovie movie = GetCurrentMovie();
        lock (BaseMesFilms.MovieUpdateQueue)
        {
          BaseMesFilms.MovieUpdateQueue.Enqueue(movie);
        }
        BaseMesFilms.TraktQueueTimer.Change(BaseMesFilms.TrakthandlerTimeout, Timeout.Infinite);
        // LogMyFilms.Debug("Commit() - #" + BaseMesFilms.MovieUpdateQueue.Count + ", conf '" + movie.Config + "', user '" + movie.Username + "', title '" + movie.Title + "' (" + movie.ID + ", " + movie.Year + ", " + movie.IMDBNumber + "), rating/userrating '" + movie.Rating + "/" + movie.RatingUser + "', Wacthed '" + movie.Watched + "' (" + movie.WatchedCount + ")'");
      }
    }

    internal string GetStringValue(List<string> input)
    {
      string output = string.Empty;
      var itemList = input.Select(x => x.Trim()).Where(x => x.Length > 0).Distinct().ToList();
      itemList.Sort();
      foreach (var s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }

    public void AddCategoryTrakt(string toAdd)
    {
      CategoryTrakt.Add(toAdd);
    }

    public void RemoveCategoryTrakt(string toRemove)
    {
      CategoryTrakt.Remove(toRemove);
    }
  }
}