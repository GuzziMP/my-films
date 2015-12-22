using System.Collections.Generic;

namespace Grabber.Data
{
  public class DbMovieInfo
  {
    public DbMovieInfo()
    {
      // initialize the List objects ...
      Posters = new List<string>();
      Backdrops = new List<string>();
      Languages = new List<string>();
      Producers = new List<string>();
      Directors = new List<string>();
      Writers = new List<string>();
      Actors = new List<string>();
      Country = new List<string>();
      Genres = new List<string>();
      Persons = new List<DbPersonInfo>();
    }

    public string Identifier { get; set; }
    public string Name { get; set; }
    public string TranslatedTitle { get; set; }
    public string AlternativeTitle { get; set; }
    public int Year { get; set; }
    public string ImdbId { get; set; }
    public string DetailsUrl { get; set; }
    public string Summary { get; set; }
    public List<string> Posters { get; set; }
    public List<string> Backdrops { get; set; }
    public float Score { get; set; }
    public string Certification { get; set; }
    public List<string> Languages { get; set; }
    public int Runtime { get; set; }
    public List<string> Producers { get; set; }
    public List<string> Directors { get; set; }
    public List<string> Writers { get; set; }
    public List<string> Actors { get; set; }
    public List<string> Country { get; set; }
    public List<string> Genres { get; set; }
    public List<DbPersonInfo> Persons { get; set; }
  }
}