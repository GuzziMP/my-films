using System.Collections.Generic;

namespace Grabber.Data
{
  public class DbPersonInfo
  {
    public DbPersonInfo()
    {
      Images = new List<string>();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Job { get; set; }
    public string Biography { get; set; }
    public string Birthday { get; set; }
    public string Birthplace { get; set; }
    public string DetailsUrl { get; set; }
    public List<string> Images { get; set; }
  }
}