#region GNU license
// MyFilms - Plugin for Mediaportal
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
using System.Globalization;

namespace MyFilmsPlugin.Utils
{
  /// <summary>
  /// Implementing String Methods
  /// </summary>
  public class NewString
  {
    string myString;

    #region ctor
    public NewString() { myString = String.Empty; }
    public NewString(string value) { myString = value; }
    #endregion

    #region Left, Right & Mid
    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string Left(int length) { return Left(myString, length); }
    /// <summary>Return left hand part of a string</summary>
    /// <param name="source">string to split</param>
    /// <param name="length">Number of characters to return</param>
    /// <returns>Returns left part of the string OR complete original string if length less than 1 or greater than string length</returns>
    public static string Left(string source, int length)
    {
      if (source == null) throw new ArgumentNullException("source");
      return length > 0 && length < source.Length ? source.Substring(0, length) : source;
    }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string Right(int length) { return Right(myString, length); }
    /// <summary>Returns right part of a string</summary>
    /// <param name="source">string to split</param>
    /// <param name="length">Number of characters to return</param>
    /// <returns>Returns right part of the string OR complete original string if length less than 1 or greater than string length</returns>
    public static string Right(string source, int length)
    {
      return length > 0 && length < source.Length ? source.Substring(source.Length - length) : source;
    }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string Mid(int index) { return Mid(myString, index); }


    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string Mid(int index, int length) { return Mid(myString, index, length); }
    /// <summary>Returns middle part of a string</summary>
    /// <param name="source">string to split</param>
    /// <param name="index">Position in string to start extract</param>
    /// <param name="length">Number of characters to extract. If 0 extract all to end</param>
    /// <returns>Returns middle part of string</returns>
    public static string Mid(string source, int index, int length = 0)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (index >= 0 && index < source.Length)
      {
        if (length == 0) length = source.Length;
        length = Min(length, source.Length - index + 1);
        return source.Substring(index, length);
      }
      else
        return "";
    }
    #endregion

    #region Positional functions
    /// <summary>Searches for substring in string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <returns>Position in string of first match</returns>
    public static int Pos(string findText, string source)
    {
      if (source == null) throw new ArgumentNullException("source");

      return source.IndexOf(findText, StringComparison.Ordinal);
    }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public int PPos(string findText, int index) { return myString.IndexOf(findText, index); }
    /// <summary>Searches for substring in string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <param name="index">Position in string to begin searching from</param>
    /// <returns>Position in string of next match</returns>
    public static int PPos(string findText, string source, int index)
    {
      if (findText == null) throw new ArgumentNullException("findText");
      if (source == null) throw new ArgumentNullException("source");
      return source.IndexOf(findText, index);
    }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public int NPos(string findText, int count, bool asChars) { return NPos(findText, myString, count, asChars); }
    /// <summary>Searches for position of nth substring in string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <param name="count">find n'th match in string. If negative search backwards from end of string for -n'th match</param>
    /// <param name="asChars">If true treat findText as a character array, matching on ANY char</param>
    /// <returns>Position in string of n'th match</returns>
    public static int NPos(string findText, string source, int count, bool asChars)
    {
      int index;
      int iLen;
      char[] sFindArray;

      if (findText == null) throw new ArgumentNullException("findText");
      if (source == null) throw new ArgumentNullException("source");

      iLen = (asChars) ? 1 : findText.Length;
      sFindArray = findText.ToCharArray();

      if (count < 0)
      {
        index = NPos(Reverse(findText), Reverse(source), count * -1, asChars);
        if (index != -1) index = source.Length - index - iLen;
        return index;
      }

      index = -iLen;
      do
      {
        index = (asChars) ? source.IndexOfAny(sFindArray, index + 1) : source.IndexOf(findText, index + iLen);
        count--;
      } while (index >= 0 && count > 0);
      if (count > 0) index = -1; //not found enough
      return index;
    }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public int PosCount(string findText, bool asChars) { return PosCount(findText, myString, asChars); }
    /// <summary>count how many substrings are present in the string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <param name="asChars">If true treat findText as a character array, matching on ANY char</param>
    /// <returns>Count of substrings found</returns>
    public static int PosCount(string findText, string source, bool asChars)
    {
      int index;
      int count = -1;
      int iLen;
      char[] sFindArray;

      if (findText == null) throw new ArgumentNullException("findText");
      if (source == null) throw new ArgumentNullException("source");

      iLen = (asChars) ? 1 : findText.Length;
      sFindArray = findText.ToCharArray();

      index = -iLen;
      do
      {
        index = asChars ? source.IndexOfAny(sFindArray, index + 1) : source.IndexOf(findText, index + iLen);
        count++;
      } while (index >= 0);
      return count;
    }
    #endregion

    #region Positional extraction functions
    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string NPosMid(string findText, int startCount, int endCount, bool include, bool asChars)
    { return NPosMid(findText, myString, startCount, endCount, include, asChars); }
    /// <summary>Gets the middle string between 2 matches of a substring in a string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <param name="startCount">Find n'th match in string. If negative search back from end of string. If 0 or no match grab from string start</param>
    /// <param name="endCount">Find n'th match in string. If negative search back from end of string. If 0 or no match grab to string end</param>
    /// <param name="include">If true include the findText substring in returned string</param>
    /// <param name="asChars">If true treat findText as a character array, matching on ANY char</param>
    /// <returns>Middle string Between n'th and n'th match of findText in source</returns>
    public static string NPosMid(string findText, string source, int startCount, int endCount, bool include, bool asChars)
    {
      int iLen;

      if (findText == null) throw new ArgumentNullException("findText");
      if (source == null) throw new ArgumentNullException("source");

      iLen = (asChars) ? 1 : findText.Length;

      if (startCount != 0) //dont bother looking if 0
      {
        startCount = NPos(findText, source, startCount, asChars); //get pos of begin'th occurrence
        if (startCount >= 0 && !include) startCount += iLen;
      }
      // else (leave as 0 no need to = -1)

      if (endCount != 0)
      {
        endCount = NPos(findText, source, endCount, asChars); //get pos of end'th occurrence
        if (endCount >= 0 && include) endCount += iLen;
      }
      else
        endCount = -1; //dont bother looking if 0, set -1 for later

      if (startCount < 0) startCount = 0; //set startCount to start of str
      if (endCount < 0) endCount = source.Length; //set endCount to end of str+1

      return source.Substring(startCount, endCount - startCount);
    }


    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string NPosLeft(string findText, int count, bool include, bool asChars)
    { return NPosMid(findText, myString, 0, count, include, asChars); }
    /// <summary>Gets the left part of a string before the n'th match of a substring in a string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <param name="count">Find n'th match in string. If negative search back from end of string. If 0 or no match grab from string start</param>
    /// <param name="include">If true include the findText substring in returned string</param>
    /// <param name="asChars">If true treat findText as a character array, matching on ANY char</param>
    /// <returns>Left string before n'th match of findText in source</returns>
    public static string NPosLeft(string findText, string source, int count, bool include, bool asChars)
    { return NPosMid(findText, source, 0, count, include, asChars); }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string PosLeft(string findText)
    { return NPosMid(findText, myString, 0, 1, false, false); }
    /// <summary>Gets the left part of a string before the first match of a substring in a string</summary>
    /// <param name="findText">string to look for</param>
    /// <param name="source">string to search within</param>
    /// <returns>Left string before first match of findText in source</returns>
    public static string PosLeft(string findText, string source)
    { return NPosMid(findText, source, 0, 1, false, false); }


    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string NPosRight(string findText, int count, bool include, bool asChars)
    { return NPosMid(findText, myString, count, 0, include, asChars); }
    /// <summary>Gets the right part of a string after the n'th match of a substring in a string</summary>
    /// <param name="findText">String to look for</param>
    /// <param name="source">String to search within</param>
    /// <param name="count">Find n'th match in string. If negative search back from end of string. If 0 or no match grab to end of string</param>
    /// <param name="include">If true include the findText substring in returned string</param>
    /// <param name="asChars">If true treat findText as a character array, matching on ANY char</param>
    /// <returns>Right string after n'th match of findText in source</returns>
    public static string NPosRight(string findText, string source, int count, bool include, bool asChars)
    { return NPosMid(findText, source, count, 0, include, asChars); }

    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string PosRight(string findText)
    { return NPosMid(findText, myString, 1, 0, false, false); }
    /// <summary>Gets the right part of a string after the first match of a substring in a string</summary>
    /// <param name="findText">String to look for</param>
    /// <param name="source">String to search within</param>
    /// <returns>Right string after first match of findText in source</returns>
    public static string PosRight(string findText, string source)
    { return NPosMid(findText, source, 1, 0, false, false); }
    #endregion

    #region Search & Replace functions
    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string NPosReplaceString(string findText, string replaceText, int count)
    { return NPosReplaceString(findText, replaceText, myString, count); }
    /// <summary>Replaces all occurences of findText with replaceText in sStr from after n'th instance of findText</summary>
    /// <param name="findText">String to look for</param>
    /// <param name="replaceText">String to replace it with</param>
    /// <param name="source">String to search within</param>
    /// <param name="count">Find n'th match in string.</param>
    /// <returns>New string with replacements</returns>
    public static string NPosReplaceString(string findText, string replaceText, string source, int count)
    {
      if (findText == null) throw new ArgumentNullException("findText");
      if (replaceText == null) throw new ArgumentNullException("replaceText");
      if (source == null) throw new ArgumentNullException("source");

      string source2 = NPosRight(findText, source, count, false, false);
      return source2.Length != 0
        ? source.Substring(0, source.Length - source2.Length) + source2.Replace(findText, replaceText)
        : source;
    }


    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string PosReplaceString(string startText, string endText, string replaceText, int index)
    { return PosReplaceString(startText, endText, replaceText, myString, index); }
    /// <summary>Replaces all occurences of text between 2 different substrings with a 3rd substring</summary>
    /// <param name="startText">String to look for to begin replace</param>
    /// <param name="endText">String to look for to end replace. Can be empty "" to just replace sFindBegs not range between</param>
    /// <param name="replaceText">String to replace it with. Can be empty "" to just erase each match</param>
    /// <param name="source">String to search within</param>
    /// <param name="index">Position in string to begin search/replacing from</param>
    /// <returns>New string with replacements</returns>
    public static string PosReplaceString(string startText, string endText, string replaceText, string source, int index)
    {
      if (startText == null) throw new ArgumentNullException("startText");
      if (endText == null) throw new ArgumentNullException("endText");
      if (replaceText == null) throw new ArgumentNullException("replaceText");
      if (source == null) throw new ArgumentNullException("source");

      if (index < 0) index = 0;
      while (true)
      {
        index = source.IndexOf(startText, index);
        if (index == -1) break;

        int pEnd;
        if (endText.Length != 0)
        {
          pEnd = source.IndexOf(endText, index + 1);
          if (pEnd == -1) break;
          pEnd = pEnd + endText.Length;
        }
        else
          pEnd = index + startText.Length;

        source = source.Remove(index, pEnd - index);
        if (replaceText.Length != 0) source = source.Insert(index, replaceText);
        index = index + replaceText.Length;
        if (index > source.Length) break;
      }
      return source;
    }


    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string PosReplace(string replaceText, int index)
    { return PosReplace(replaceText, myString, index); }
    /// <summary>Replace text of equel length to new text with the new text</summary>
    /// <param name="replaceText">String to replace</param>
    /// <param name="index">Position in string to replace at</param>
    /// <param name="source">string to replace at</param>
    /// <returns>New string with replacements</returns>
    public static string PosReplace(string replaceText, string source, int index)
    {
      if (replaceText == null) throw new ArgumentNullException("replaceText");
      if (source == null) throw new ArgumentNullException("source");

      source = source.Remove(index, replaceText.Length);
      source = source.Insert(index, replaceText);
      return source;
    }


    /// <summary>Non-static version of function below. Operates on NewString object</summary>
    public string StripChars(string strip)
    { return StripChars(strip, myString); }
    /// <summary>Removes all chars in supplied string</summary>
    /// <param name="strip">String of chars to remove from string</param>
    /// <param name="source">String to search within</param>
    /// <returns>New string without strip within it</returns>
    public static string StripChars(string strip, string source)
    {
      if (strip == null) throw new ArgumentNullException("strip");
      if (source == null) throw new ArgumentNullException("source");

      for (int i = 0; i < strip.Length; i++)
        source = source.Replace(strip.Substring(i, 1), null);
      return source; // xxx check if original affected
    }
    #endregion

    #region Conversion functions
    public string ProperCase()
    {
      return ProperCase(myString);
    }

    /// <summary>Convert String to ProperCase</summary>
    /// <param name="source">String to be converted to ProperCase</param>
    /// <returns>The ProperCase String</returns>
    public static string ProperCase(string source)
    {
      char[] sFindArray = { ' ', '\t', '\n', '.' };
      int index = 0;
      char[] sArray;

      if (source == null) throw new ArgumentNullException("source");

      sArray = source.ToCharArray();

      do
      {
        sArray[index] = Char.ToUpper(sArray[index], CultureInfo.CurrentCulture);
        index = source.IndexOfAny(sFindArray, index) + 1;
      } while (index > 0 && index < source.Length);
      return new string(sArray);
    }


    public string Reverse()
    {
      return Reverse(myString);
    }

    /// <summary>Recursion Reverse function to Reverse a given String</summary>
    /// <param name="source">String to be Reversed</param>
    /// <returns>The Reversed string</returns>
    public static string Reverse(string source)
    {
      if (source == null) throw new ArgumentNullException("source");

      return source.Length == 1 ? source : Reverse(source.Substring(1)) + source.Substring(0, 1);
    }

    /// <summary>Converts any multiple adjacent spaces to a single space</summary>
    /// <returns>The trimmed string</returns>
    public string TrimToSingleSpace()
    {
      return TrimToSingleSpace(myString);
    }

    public static string TrimToSingleSpace(string source)
    {
      int intPos;

      if (source == null) throw new ArgumentNullException("source");

      intPos = source.IndexOf("  ");
      return intPos == -1 ? source : TrimToSingleSpace(source.Substring(0, intPos) + source.Substring(intPos + 1));
    }

    #endregion

    #region Non_String_functions
    // -------- NON-STRING BUT USEFUL FUNCS ADDED HERE FOR NOW ------------

    /// <summary>Returns the smaller of 2 values</summary>
    /// <param name="val1">First value to compare</param>
    /// <param name="val2">Second value to compare</param>
    /// <returns>Smaller value</returns>
    public static T Min<T>(T val1, T val2)
    {
      T retVal = val2;
      if (Comparer<T>.Default.Compare(val1, val2) < 0)
        retVal = val1;
      return retVal;
    }

    /// <summary>Returns the greater of 2 values</summary>
    /// <param name="val1">First value to compare</param>
    /// <param name="val2">Second value to compare</param>
    /// <returns>Greater value value</returns>
    public static T Max<T>(T val1, T val2)
    {
      T retVal = val1;
      if (Comparer<T>.Default.Compare(val1, val2) < 0)
        retVal = val2;
      return retVal;
    }
    #endregion

  }
}
