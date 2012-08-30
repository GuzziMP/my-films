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
      this.Category = string.Empty;
      this.Year = 1900;
      this.TMDBNumber = string.Empty;
      this.IMDBNumber = string.Empty;
      this.Path = string.Empty;
      this.Trailer = string.Empty;
      this.File = string.Empty;
      this.Edition = string.Empty;
      this.GroupName = string.Empty;
      this.FormattedTitle = string.Empty;
      this.TranslatedTitle = string.Empty;
      this.Title = string.Empty;
      this.WatchedCount = -1;
      this.CategoryTrakt = new List<string>();
      this.Length = 0;
      this.DateTime = System.DateTime.Today;
      this.DateAdded = string.Empty;
      this.Picture = string.Empty;
      this.Fanart = string.Empty;
      this.Config = string.Empty;
      this.Username = string.Empty;
      this.ReadOnly = false;
      this.ID = -1;
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #region public vars

    public int ID { get; set; }

    public bool IsEmpty
    {
      get
      {
        return (this.Title == string.Empty) || (this.Title == Strings.Unknown);
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

    //public bool AllowTrakt { get; set; }
    //public bool AllowLatestMediaAPI { get; set; }
    //public DataRow MovieRow { get; set; }

    #endregion

    public void Reset()
    {
      this.Title = string.Empty;
      this.TranslatedTitle = string.Empty;
      this.FormattedTitle = string.Empty;
      this.GroupName = string.Empty;
      this.CategoryTrakt.Clear();
      this.Edition = string.Empty;
      this.IMDBNumber = string.Empty;
      this.TMDBNumber = string.Empty;
      this.Year = 1900;
      this.Category = string.Empty;
      this.Length = 0;
      this.Rating = 0.0f;
      this.RatingUser = 0.0f;
      this.Watched = false;
      this.WatchedCount = -1;
      this.DateTime = DateTime.Today;
      this.DateAdded = string.Empty;
      this.File = string.Empty;
      this.Trailer = string.Empty;
      this.Path = string.Empty;
      this.Picture = string.Empty;
      this.Fanart = string.Empty;
      this.Config = string.Empty;
      this.Username = string.Empty;
      this.ReadOnly = false;
      //this.AllowTrakt = false;
      //this.AllowLatestMediaAPI = false;
      //this.MovieRow = null;
    }

    private MFMovie GetCurrentMovie()
    {
      var movie = new MFMovie
        {
          ID = this.ID,
          Title = this.Title,
          TranslatedTitle = this.TranslatedTitle,
          FormattedTitle = this.FormattedTitle,
          GroupName = this.GroupName,
          CategoryTrakt = this.CategoryTrakt,
          Edition = this.Edition,
          IMDBNumber = this.IMDBNumber,
          TMDBNumber = this.TMDBNumber,
          Year = this.Year,
          Category = this.Category,
          Length = this.Length,
          Rating = this.Rating,
          RatingUser = this.RatingUser,
          Watched = this.Watched,
          WatchedCount = this.WatchedCount,
          DateTime = this.DateTime,
          DateAdded = this.DateAdded,
          File = this.File,
          Trailer = this.Trailer,
          Path = this.Path,
          Picture = this.Picture,
          Fanart = this.Fanart,
          Config = this.Config,
          Username = this.Username,
          ReadOnly = this.ReadOnly
        };
      return movie;
    }

    public void Commit()
    {
      lock (BaseMesFilms.MovieUpdateQueue)
      {
        MFMovie movie = this.GetCurrentMovie();
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
      this.CategoryTrakt.Add(toAdd);
    }

    public void RemoveCategoryTrakt(string toRemove)
    {
      this.CategoryTrakt.Remove(toRemove);
    }
  }
}