#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using System.Collections.Generic;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Ripper;
using System.IO;
using System.Xml;

namespace Grabber.Importer.Helpers
{
  using System.Linq;

  #region Date/Time extension Methods

  public static class DateExtensions
  {
    /// <summary>
    /// Date Time extension method to return a unix epoch
    /// time as a long
    /// </summary>
    /// <returns> A long representing the Date Time as the number
    /// of seconds since 1/1/1970</returns>
    public static long ToEpoch(this DateTime dt)
    {
      return (long)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    /// <summary>
    /// Long extension method to convert a Unix epoch
    /// time to a standard C# DateTime object.
    /// </summary>
    /// <returns>A DateTime object representing the unix
    /// time as seconds since 1/1/1970</returns>
    public static DateTime FromEpoch(this long unixTime)
    {
      return new DateTime(1970, 1, 1).AddSeconds(unixTime);
    }
  }

  #endregion

  public class Helper
  {
    #region List<T> Methods
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public static List<T> inverseList<T>(List<T> input)
    {
      List<T> result = new List<T>(input.Count);
      for (int i = input.Count - 1; i >= 0; i--)
        result.Add(input[i]);
      return result;
    }

    public static T getElementFromList<T, P>(P currPropertyValue, string PropertyName, int indexOffset, List<T> elements)
    {
      // takes care of "looping"
      if (elements.Count == 0) return default(T);
      int indexToGet = 0;
      P value = default(P);
      for (int i = 0; i < elements.Count; i++)
      {
        try
        {
          value = (P)elements[i].GetType().InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elements[i], null);
          if (value.Equals(currPropertyValue))
          {
            indexToGet = i + indexOffset;
            break;
          }
        }
        catch (Exception x)
        {
          LogMyFilms.Debug("Wrong call of getElementFromList<T,P>: the Type " + elements[i].GetType().Name + " - " + x.Message);
          return default(T);
        }
      }
      if (indexToGet < 0) indexToGet = elements.Count + indexToGet;
      if (indexToGet >= elements.Count) indexToGet = indexToGet - elements.Count;
      return elements[indexToGet];
    }

    public static List<P> getPropertyListFromList<T, P>(string PropertyNameToGet, List<T> elements)
    {
      List<P> results = new List<P>();
      foreach (T elem in elements)
      {
        try
        {
          results.Add((P)elem.GetType().InvokeMember(PropertyNameToGet, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elem, null));
        }
        catch (Exception)
        {
          LogMyFilms.Debug("Wrong call of getPropertyListFromList<T,P>: Type " + elem.GetType().Name);
        }
      }
      return results;
    }

    public delegate void ForEachOperation<T>(T element, int currIndex);
    /// <summary>
    /// Performs an operation for each element in the list, by starting with a specific index and working its way around it (eg: n, n+1, n-1, n+2, n-2, ...)
    /// </summary>
    /// <typeparam name="T">The Type of elements in the IList</typeparam>
    /// <param name="elements">All elements, this value cannot be null</param>
    /// <param name="startElement">The starting point for the operation (0 operates like a traditional foreach loop)</param>
    /// <param name="operation">The operation to perform on each element</param>
    public static void ProximityForEach<T>(IList<T> elements, int startElement, ForEachOperation<T> operation)
    {
      if (elements == null)
        throw new ArgumentNullException("elements");
      if ((startElement >= elements.Count && elements.Count > 0) || startElement < 0)
        throw new ArgumentOutOfRangeException("startElement", startElement, "startElement must be > 0 and <= elements.Count (" + elements.Count + ")");
      if (elements.Count > 0)                                      // if empty list, nothing to do, but legal, so not an exception
      {
        T item;
        for (int lower = startElement, upper = startElement + 1; // start with the selected, and then go down before going up
             upper < elements.Count || lower >= 0;               // only exit once both ends have been reached
             lower--, upper++)
        {
          if (lower >= 0)                                      // are lower elems left?
          {
            item = elements[lower];
            operation(item, lower);
            elements[lower] = item;
          }
          if (upper < elements.Count)                          // are higher elems left?
          {
            item = elements[upper];
            operation(item, upper);
            elements[upper] = item;
          }
        }
      }
    }

    #endregion

    #region XML File Cache

    public static void SaveXmlCache(string filename, XmlNode node)
    {
      // create cached document
      try
      {
        LogMyFilms.Debug("Saving xml to file cache: " + filename);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(node.OuterXml);
        if (!Directory.Exists(Path.GetDirectoryName(filename)))
          Directory.CreateDirectory(Path.GetDirectoryName(filename));
        doc.Save(filename);
      }
      catch (Exception e)
      {
        LogMyFilms.Debug("Failed to save xml to cache: {0}", filename);
        LogMyFilms.Debug("Exception: {0}", e.Message);
      }
    }

    public static XmlNode LoadXmlCache(string filename)
    {
      if (!File.Exists(filename)) return null;

      // Load cache
      XmlDocument doc = new XmlDocument();
      try
      {
        LogMyFilms.Debug("Loading xml from file cache: " + filename);
        doc.Load(filename);
        return doc.FirstChild;
      }
      catch (Exception e)
      {
        LogMyFilms.Debug("Failed to load xml from file cache: {0}", filename);
        LogMyFilms.Debug("Exception: {0}", e.Message);
        return null;
      }
    }

    #endregion

    #region Other Public Methods

    /// <summary>
    /// Convertes a given amount of Milliseconds into humanly readable MM:SS format
    /// </summary>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public static System.String MSToMMSS(double milliseconds)
    {
      TimeSpan t = new TimeSpan(0, 0, 0, 0, (int)milliseconds);
      //cs1 anomalies or no disc/data available -> -:- 
      if (milliseconds <= 0)
      { return ("-- : --"); }
      //cs1 playtimes >= 1 hour -> 1:MM:SS 
      else if (milliseconds >= 3600000)
      { return t.Hours.ToString("0") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00"); }
      //cs1 playtimes < 1 hour -> MM:SS 
      else { return t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00"); }
    }

    /// <summary>
    /// Joins two parts of a path
    /// </summary>
    /// <param name="path1"></param>
    /// <param name="path2"></param>
    /// <returns></returns>
    public static string PathCombine(string path1, string path2)
    {
      if (path1 == null && path2 == null) return string.Empty;
      if (path1 == null) return path2;
      if (path2 == null) return path1;
      if (path2.Length > 0 && (path2[0] == '\\' || path2[0] == '/')) path2 = path2.Substring(1);
      return System.IO.Path.Combine(path1, path2);
    }

    /// <summary>
    /// Cleans the path by removing invalid characters
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string cleanLocalPath(string path)
    {
      path = Path.GetInvalidFileNameChars().Aggregate(path, (current, c) => current.Replace(c, invalidCharReplacement));
      // Also remove trailing dots and spaces            
      return path.TrimEnd(new char[] { '.' }).Trim();
    }

    const char invalidCharReplacement = '_';


    /// <summary>
    /// Removes duplicate items from a list
    /// </summary>
    /// <param name="inputList"></param>
    /// <returns>A list with unique items</returns>
    public static List<string> RemoveDuplicates(IEnumerable<string> inputList)
    {
      var uniqueStore = new Dictionary<string, int>();
      var finalList = new List<string>();
      foreach (string currValue in inputList)
      {
        if (!uniqueStore.ContainsKey(currValue))
        {
          uniqueStore.Add(currValue, 0);
          finalList.Add(currValue);
        }
      }
      return finalList;
    }

    /// <summary>
    /// Returns a limited list of items
    /// </summary>
    /// <param name="list"></param>
    /// <param name="limit"></param>
    public static void LimitList(ref List<string> list, int limit)
    {
      if (limit >= list.Count) return;
      list.RemoveRange(list.Count - (list.Count - limit), (list.Count - limit));
    }

    /// <summary>
    /// Checks if a filename is an Image file e.g. ISO
    /// </summary>
    /// <param name="filename"></param>
    /// <returns>Returns true if file is a image</returns>
    public static bool IsImageFile(string filename)
    {
      string extension = System.IO.Path.GetExtension(filename).ToLower();
      return VirtualDirectory.IsImageFile(extension);
    }

    /// <summary>
    /// Checks if Fullscreen video is active
    /// </summary>
    /// <returns></returns>
    public static bool IsFullscreenVideo
    {
      get
      {
        bool isFullscreen = (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
        return isFullscreen;
      }

    }

    public static void disableNativeAutoplay()
    {
      LogMyFilms.Debug("Disabling native autoplay.");
      AutoPlay.StopListening();
    }

    public static void enableNativeAutoplay()
    {
      if (GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
      {
        LogMyFilms.Debug("Re-enabling native autoplay.");
        AutoPlay.StartListening();
      }
    }

    public static string UppercaseFirst(string s)
    {
      if (string.IsNullOrEmpty(s))
      {
        return string.Empty;
      }
      char[] a = s.ToCharArray();
      a[0] = char.ToUpper(a[0]);
      return new string(a);
    }

    public static void GetEpisodeIndexesFromComposite(string compositeID, out int seasonIndex, out int episodeIndex)
    {
      seasonIndex = 0;
      episodeIndex = 0;

      if (string.IsNullOrEmpty(compositeID)) return;

      string[] splits = compositeID.Split(new char[] { '_' });
      string[] epComp = splits[1].Split(new char[] { 'x' });

      int.TryParse(epComp[0], out seasonIndex);
      int.TryParse(epComp[1], out episodeIndex);
    }

    public static bool IsNullOrWhiteSpace(string value)
    {
      return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
    }

    #endregion

  }
}

