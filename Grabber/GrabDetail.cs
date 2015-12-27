namespace Grabber
{
  public class GrabDetail
  {
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string GrabName { get; set; } //the defined detail definition
    public DetailType Type { get; set; }
    public GrabberUrlClass.Grabber_Output OutputMapping { get; set; }
    public string Value { get; set; }

    public GrabDetail(int id, string displayName, string grabName, DetailType type, GrabberUrlClass.Grabber_Output outputmapping)
    {
      Id = id;
      DisplayName = displayName;
      GrabName = grabName;
      Type = type;
      OutputMapping = outputmapping;
    }

    public override string ToString()
    {
      return Value;
    }
  }
}