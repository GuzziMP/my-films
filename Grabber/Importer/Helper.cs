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
using System.IO;
using System.Xml;
using System.Linq;

namespace Grabber.Importer.Helpers
{
  
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

  }
}

