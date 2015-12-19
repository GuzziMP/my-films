using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFilmsPlugin.DataBase
{
  using System.Globalization;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  public class MultiUserData
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();
    private string MultiUserStatesValue { get; set; }

    public const decimal NoRating = -1;
    public const decimal FavoriteRating = 7;
    public static readonly DateTime NoWatchedDate = DateTime.MinValue;

    private List<UserState> MultiUserStates { get; set; }
    
    public MultiUserData(string value)
    {
      MultiUserStatesValue = value;
      // if (!string.IsNullOrEmpty(value)) LogMyFilms.Debug("MultiUserData() - loaded with value = '" + value + "'");
      LoadUserStates();
    }

    public UserState GetUserState(string username)
    {
      UserState userstate;
      if (MultiUserStates.Count(userState => userState.UserName == username) == 0)
      {
        userstate = new UserState(username);
        MultiUserStates.Add(userstate);
      }
      userstate = MultiUserStates.First(userState => userState.UserName == username);
      return userstate;
    }

    public UserState GetGlobalState()
    {
      var global = new UserState(MyFilms.GlobalUsername)
        { WatchedCount = 0, UserRating = NoRating, WatchedDate = NoWatchedDate, ResumeData = 0 };

      foreach (UserState userState in MultiUserStates.FindAll(x => x.UserName != MyFilms.GlobalUsername))
      {
        global.WatchedCount += userState.WatchedCount;
        if (userState.WatchedDate > global.WatchedDate) global.WatchedDate = userState.WatchedDate;
        if (userState.UserRating > global.UserRating) global.UserRating = userState.UserRating;
        if (userState.ResumeData > global.ResumeData) global.ResumeData = userState.ResumeData;
      }
      global.Watched = global.WatchedCount > 0;
      return global;
    }

    public void SetWatchedWithoutDate(string username, bool watched)
    {
      var userstate = GetUserState(username);

      userstate.WatchedCount = (watched) ? 1 : 0;
      userstate.Watched = watched;
      userstate.WatchedDate = NoWatchedDate;
    }
    
    public void SetWatched(string username, bool watched)
    {
      var userstate = GetUserState(username);
      userstate.WatchedCount = (watched) ? 1 : 0;
      userstate.Watched = watched;
      userstate.WatchedDate = (watched) ? DateTime.Today : NoWatchedDate; // DateTime.Parse(DateTime.Now.ToShortDateString()); // make sure, only the date will be used with time to 00:00
    }

    public void SetWatchedCount(string username, int watchedcount)
    {
      var userstate = GetUserState(username);
      userstate.WatchedCount = watchedcount;
      userstate.Watched = (watchedcount > 0);
      userstate.WatchedDate = (watchedcount > 0) ? DateTime.Today : NoWatchedDate;
    }

    public void SetRating(string username, decimal rating)
    {
      var userstate = GetUserState(username);
      userstate.UserRating = (rating < 0) ? NoRating : rating;
    }

    public void AddWatchedCountByOne(string username)
    {
      var userstate = GetUserState(username);
      userstate.WatchedCount += 1;
      userstate.WatchedDate = DateTime.Today; // DateTime.Parse(DateTime.Now.ToShortDateString());
      userstate.Watched = true;
    }

    public void SetResumeData(string username, int resume)
    {
      var userstate = GetUserState(username);
      userstate.ResumeData = resume;
    }

    public string ResultValueString()
    {
      string resultValueString = string.Empty;
      foreach (UserState state in MultiUserStates.AsQueryable().OrderBy(x => x.UserName))
      {
        // LogMyFilms.Debug("LoadUserStates() - return state for user '" + state.UserName + "', rating = '" + state.UserRating + "', count = '" + state.WatchedCount + "', watched = '" + state.Watched + "', watcheddate = '" + state.WatchedDate + "'");
        // var sNew = ((!string.IsNullOrEmpty(state.UserName)) ? state.UserName : "*") + ":" + state.WatchedCount + ":" + state.UserRating.ToString(CultureInfo.InvariantCulture) + ":" + ((state.WatchedDate > DateTime.Parse("01/01/1900")) ? state.WatchedDate.ToShortDateString() : "");  // short date as invariant culture // var sNew = state.UserName + ":" + state.WatchedCount + ":" + state.UserRating.ToString(CultureInfo.InvariantCulture) + ":" + state.WatchedDate.ToString("d", invC);  // short date as invariant culture
        string sNew = state.UserName + ":" + state.WatchedCount + ":" + state.UserRating.ToString(CultureInfo.InvariantCulture) + ":" + ((state.WatchedDate > DateTime.Parse("01/01/1900")) ? state.WatchedDate.ToShortDateString() : "") + ":" + state.ResumeData;  // short date as invariant culture // var sNew = state.UserName + ":" + state.WatchedCount + ":" + state.UserRating.ToString(CultureInfo.InvariantCulture) + ":" + state.WatchedDate.ToString("d", invC);  // short date as invariant culture
        if (resultValueString.Length > 0) resultValueString += "|";
        resultValueString += sNew;
      }
      // if (MultiUserStatesValue != resultValueString) LogMyFilms.Debug("ResultValueString() - modified MUS value = '" + resultValueString + "'");
      return resultValueString;
    }

    public void DeleteUser(string username)
    {
      MultiUserStates.RemoveAll(x => x.UserName == username);
    }

    private enum Type
    {
      Username,
      Count,
      Rating,
      Datewatched,
      Resume
    }
    
    private void LoadUserStates()
    {
      if (MultiUserStates == null) MultiUserStates = new List<UserState>();
      MultiUserStates.Clear();
      string[] split = MultiUserStatesValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in split)
      {
        if (s.Contains(":"))
        {
          var userstate = new UserState((string)EnhancedWatchedValue(s, Type.Username));
          userstate.UserRating = (decimal)EnhancedWatchedValue(s, Type.Rating);
          userstate.WatchedCount = (int)EnhancedWatchedValue(s, Type.Count);
          userstate.Watched = (userstate.WatchedCount > 0);
          userstate.WatchedDate = (DateTime)(EnhancedWatchedValue(s, Type.Datewatched));
          userstate.ResumeData = (int)(EnhancedWatchedValue(s, Type.Resume));
          // LogMyFilms.Debug("LoadUserStates() - loading state for user '" + userstate.UserName + "', rating = '" + userstate.UserRating + "', count = '" + userstate.WatchedCount + "', watched = '" + userstate.Watched + "', watcheddate = '" + userstate.WatchedDate + "'");
          MultiUserStates.Add(userstate);
        }
      }
    }

    private static object EnhancedWatchedValue(string s, Type type)
    {
      // "*:0:-1|MikePlanet:0:-1" or "Global:0:-1|MikePlanet:0:-1:2011-12-24" "Global:0:-1|MikePlanet:0:-1" or "Global:0:-1|MikePlanet:0:-1:2011-12-24"
      object value = "";
      string[] split = s.Split(new Char[] { ':' });
      switch (type)
      {
        case Type.Username:
          // value = (split.Length > 0 && split.ToString() != "*") ? split[0] : ""; // "*" == default user
          value = split[0];
          break;
        case Type.Count:
          int count = 0;
          value = (split.Length > 1 && int.TryParse(split[1], out count)) ? count : 0;
          break;
        case Type.Rating:
          decimal rating;
          value = (split.Length > 2 && decimal.TryParse(split[2], NumberStyles.Any, CultureInfo.InvariantCulture, out rating)) ? rating : NoRating;
          break;
        case Type.Datewatched:
          DateTime datewatched;
          value = (split.Length > 3 && DateTime.TryParse(split[3], out datewatched)) ? datewatched : NoWatchedDate;
          break;
        case Type.Resume:
          int resume;
          value = (split.Length > 4 && int.TryParse(split[4], out resume)) ? resume : 0;
          break;
      }
      return value;
    }

    internal static string Remove(string input, string toRemove)
    {
      string output = "";
      string[] split = input.Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
      List<string> itemList = split.Select(x => x.Trim()).Distinct().ToList();
      if (itemList.Contains(toRemove)) itemList.Remove(toRemove);
      itemList.Sort();
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }

    internal static string Add(string input, string toAdd)
    {
      string output = "";
      string[] split = input.Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
      List<string> itemList = split.Select(x => x.Trim()).Distinct().ToList();
      if (!itemList.Contains(toAdd)) itemList.Add(toAdd);
      itemList.Sort();
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }
  }

  public class UserState
  {
    public UserState(string username)
    {
      UserName = username;
      UserRating = MultiUserData.NoRating;
      Watched = false;
      WatchedCount = 0;
      WatchedDate = MultiUserData.NoWatchedDate;
      ResumeData = 0;
    }

    public string UserName { get; private set; }
    public decimal UserRating { get; set; }
    public bool Watched { get; set; }
    public int WatchedCount { get; set; }
    public DateTime WatchedDate { get; set; }
    public int ResumeData { get; set; }
  }
}
