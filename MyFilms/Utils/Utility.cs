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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DirectShowLib;
using DirectShowLib.Dvd;
using MediaPortal.GUI.Library;

namespace MyFilmsPlugin.Utils
{
  class Utility
  {
    #region Ctor / Private variables
    private Utility()
    {

    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #endregion

    #region Enums
    public enum VideoDiscType
    {
      [Description(@"\video_ts\video_ts.ifo")]
      DVD,
      [Description(@"\bdmv\index.bdmv")]
      Bluray,
      [Description(@"\adv_obj\discid.dat")] // or adv_obj\vplst000.xpl ?
      HDDVD,
      [Description("")]
      UnknownFormat
    }

    /// <summary>
    /// Video Formats
    /// </summary>
    public enum VideoFormat
    {
      NotSupported, // used for file types that are not supported
      Unknown, // used for images
      DVD,
      Bluray,
      HDDVD,
      SVCD,
      File, // used for all valid 'standalone' video files that do not have a specific video format
    }


    #endregion

    #region Enum Helper Methods

    public static List<T> EnumToList<T>()
    {
      Type enumType = typeof(T);
      if (enumType.BaseType != typeof(Enum))
        throw new ArgumentException("T must be of type System.Enum");

      return new List<T>(Enum.GetValues(enumType) as IEnumerable<T>);
    }

    public static string GetEnumValueDescription(object value)
    {
      Type objType = value.GetType();
      FieldInfo fieldInfo = objType.GetField(Enum.GetName(objType, value));
      DescriptionAttribute attribute = (DescriptionAttribute)
      fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];

      // Return the description.
      return attribute.Description;
    }

    #endregion

    #region FileSystem Methods

    /// <summary>
    /// Get all files from directory and it's subdirectories.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static IEnumerable<FileInfo> GetFilesRecursive(DirectoryInfo directory)
    {
      var fileList = new List<FileInfo>();
      var subdirectories = new DirectoryInfo[] { };

      try
      {
        fileList.AddRange(directory.GetFiles("*"));
        subdirectories = directory.GetDirectories();
      }
      catch (Exception e)
      {
        if (e is ThreadAbortException)
          throw e;

        Log.Error("Error while retrieving files/directories for: " + directory.FullName, e);
      }

      foreach (DirectoryInfo subdirectory in subdirectories)
      {
        try
        {
          if ((subdirectory.Attributes & FileAttributes.System) == 0)
            fileList.AddRange(GetFilesRecursive(subdirectory));
          else
            Log.Debug("Rejecting directory " + subdirectory.FullName + " because it is flagged as a System folder.");
        }
        catch (Exception e)
        {
          if (e is ThreadAbortException)
            throw e;
          Log.Error("Error during attribute check for: " + subdirectory.FullName, e);
        }
      }

      return fileList;
    }

    /// <summary>
    /// Get all video files from directory and it's subdirectories.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static List<FileInfo> GetVideoFilesRecursive(DirectoryInfo directory)
    {
      IEnumerable<FileInfo> fileList = GetFilesRecursive(directory);
      return fileList.Where(IsVideoFile).ToList();
    }

    /// <summary>
    /// This method will create a string that can be safely used as a filename.
    /// </summary>
    /// <param name="subject">the string to process</param>
    /// <returns>the processed string</returns>
    public static string CreateFilename(string subject)
    {
      if (String.IsNullOrEmpty(subject))
        return string.Empty;
      string rtFilename = subject;
      char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
      return invalidFileChars.Aggregate(rtFilename, (current, invalidFileChar) => current.Replace(invalidFileChar, '_'));
    }

    /// <summary>
    /// Removes the extension from a filename
    /// @todo: remove this method
    /// </summary>
    /// <param name="file"></param>
    /// <returns>filename without extension</returns>
    public static string RemoveFileExtension(FileInfo file)
    {
      return Path.GetFileNameWithoutExtension(file.Name);
    }

    public static string RemoveFileExtension(string filename)
    {
      return RemoveFileExtension(new FileInfo(filename));
    }

    /// <summary>
    /// Remove extension and stackmarkers from a filename
    /// </summary>
    /// <param name="file">target file</param>
    /// <returns>the filename without stackmarkers and extension</returns>
    public static string RemoveFileStackMarkers(FileInfo file)
    {
      // Remove the file extension from the filename
      string fileName = RemoveFileExtension(file);

      // If file is classified as multipart clean the stack markers.
      if (isFileMultiPart(fileName))
      {
        Regex expr = new Regex(rxFileStackMarkers, RegexOptions.IgnoreCase);
        Match match = expr.Match(fileName);
        // if we have a match on this expression we will remove the complete match.
        fileName = match.Success ? expr.Replace(fileName, "") : fileName.Substring(0, (fileName.Length - 1));
      }

      // Return the cleaned filename
      return fileName;
    }

    public static string RemoveFileStackMarkers(string filename)
    {
      return RemoveFileStackMarkers(new FileInfo(filename));
    }

    // Regular expression patterns used by the multipart detection and cleaning methods
    private const string rxFileStackMarkers = @"([\s\-]*(cd|disk|disc|part)[\s\-]*([a-c]|\d+|i+))|[\(\[]\d(of|-)\d[\)\]]$";

    /// <summary>
    /// Checks if a file has stack markers (and is multi-part)
    /// </summary>
    /// <param name="file"></param>
    /// <returns>true if multipart, false if not</returns>
    public static bool isFileMultiPart(FileInfo file)
    {
      return isFileMultiPart(RemoveFileExtension(file));
    }

    /// <summary>
    /// Checks if a filename has stack markers (and is multi-part)
    /// </summary>
    /// <param name="filename"></param>
    /// <returns>true if multipart, false if not</returns>
    public static bool isFileMultiPart(string filename)
    {
      Regex expr = new Regex(rxFileStackMarkers + @"|[^\s\d](\d+)$|([a-c])$", RegexOptions.IgnoreCase);
      return expr.Match(filename).Success;
    }

    #endregion

    #region String Modification / Regular Expressions Methods

    // Regular expression pattern that matches an "article" that need to be moved for title conversions
    // todo: the articles should really be a user definable setting in the future
    private const string rxTitleSortPrefix = "(the|a|an|ein|das|die|der|les|la|le|el|une|de|het)";

    /// <summary>
    /// Converts a movie title to the display name.
    /// </summary>
    /// <example>
    /// Changes "Movie, The" into "The Movie"
    /// </example>
    /// <param name="title"></param>
    /// <returns>display name</returns>
    public static string TitleToDisplayName(string title)
    {
      Regex expr = new Regex(@"(.+?)(?:, " + rxTitleSortPrefix + @")?\s*$", RegexOptions.IgnoreCase);
      return expr.Replace(title, "$2 $1").Trim();
    }

    /// <summary>
    /// Converts a title to the archive name (sortable title)
    /// </summary>
    /// <example>
    /// Changes "The Movie" into "Movie, The"
    /// </example>
    /// <param name="title"></param>
    /// <returns>archive name</returns>
    public static string TitleToArchiveName(string title)
    {
      Regex expr = new Regex(@"^" + rxTitleSortPrefix + @"\s(.+)", RegexOptions.IgnoreCase);
      return expr.Replace(title, "$2, $1").Trim();
    }

    /// <summary>
    /// Converts a title string to a common format to be used in comparison.
    /// </summary>
    /// <param name="title">the original title</param>
    /// <returns>the normalized title</returns>
    public static string normalizeTitle(string title)
    {
      // Convert title to lowercase culture invariant
      string newTitle = title.ToLowerInvariant();

      // Swap article
      newTitle = TitleToDisplayName(newTitle);

      // Replace non-descriptive characters with spaces
      newTitle = Regex.Replace(newTitle, @"[\.:;\+\-\–\—\―\˜\*]", " ");

      // Remove other non-descriptive characters completely
      newTitle = Regex.Replace(newTitle, @"[\(\)\[\]'`,""\#\$\?]", "");

      // Equalize: Convert to base character string
      newTitle = RemoveDiacritics(newTitle);

      // Equalize: Common characters with words of the same meaning
      newTitle = Regex.Replace(newTitle, @"\b(and|und|en|et|y)\b", " & ");

      // Equalize: Roman Numbers To Numeric
      newTitle = Regex.Replace(newTitle, @"\si(\b)", @" 1$1");
      newTitle = Regex.Replace(newTitle, @"\sii(\b)", @" 2$1");
      newTitle = Regex.Replace(newTitle, @"\siii(\b)", @" 3$1");
      newTitle = Regex.Replace(newTitle, @"\siv(\b)", @" 4$1");
      newTitle = Regex.Replace(newTitle, @"\sv(\b)", @" 5$1");
      newTitle = Regex.Replace(newTitle, @"\svi(\b)", @" 6$1");
      newTitle = Regex.Replace(newTitle, @"\svii(\b)", @" 7$1");
      newTitle = Regex.Replace(newTitle, @"\sviii(\b)", @" 8$1");
      newTitle = Regex.Replace(newTitle, @"\six(\b)", @" 9$1");

      // Remove the number 1 from the end of a title string
      newTitle = Regex.Replace(newTitle, @"\s(1)$", "");

      // Remove double spaces and trim
      newTitle = trimSpaces(newTitle);

      // return the cleaned title
      return newTitle;
    }

    /// <summary>
    /// Translates characters to their base form.
    /// </summary>
    /// <example>
    /// characters: ë, é, è
    /// result: e
    /// </example>
    /// <remarks>
    /// source: http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
    /// </remarks>
    public static string RemoveDiacritics(string title)
    {
      string stFormD = title.Normalize(NormalizationForm.FormD);
      StringBuilder sb = new StringBuilder();

      foreach (char t in stFormD)
      {
        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
        if (uc != UnicodeCategory.NonSpacingMark)
        {
          sb.Append(t);
        }
      }

      return sb.ToString().Normalize(NormalizationForm.FormC);
    }


    /// <summary>
    /// Returns true, if all strings or delimited parts of checkExpressions are contained in input
    /// </summary>
    /// <param name="input"></param>
    /// <param name="checkExpression"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static bool ContainsAll(string input, string checkExpression, string delimiter)
    {
      if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(checkExpression))
        return false;
      if (input.ToLower().Contains(checkExpression.ToLower()))
        return true;
      if (!string.IsNullOrEmpty(delimiter) && delimiter.Length > 0 && checkExpression.Contains(delimiter))
      {
        string[] split = checkExpression.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
        bool matches = true;
        foreach (string s in split.Where(s => !input.ToLower().Contains(s.ToLower())))
        {
          matches = false;
        }
        return matches;
      }
      return false;
    }

    /// <summary>
    /// Removes multiple spaces and replaces them with one space   
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string trimSpaces(string input)
    {
      return Regex.Replace(input, @"\s{2,}", " ").Trim();
    }

    public static bool HasAsciiSubstring(string str, string sub)
    {
      char[] ss = sub.ToCharArray();

      // Similar stupid small optimizations bring 30% speeding-up.
      int ssLen = ss.Length;
      for (int j = 0; j < ssLen; ++j)
        ss[j] = (char)((byte)ss[j] | 32);

      byte ss0 = (byte)ss[0];
      int len = str.Length - ssLen;
      for (int i = 0; i < len; ++i)
      {
        if ((str[i] | 32) == ss0)
        {
          bool bOk = true;
          for (int j = 1; j < ssLen; ++j)
          {
            if ((str[i + j] | 32) != ss[j])
            {
              bOk = false;
              break;
            }
          }
          if (bOk)
            return true;
        }
      }
      return false;
    }

    #endregion

    #region General Methods (Unsorted)


    /// <summary>
    /// Checks if the foldername is a multipart marker.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool isFolderMultipart(string name)
    {
      Regex expr = new Regex(@"^(cd|dvd)\s*\d+$", RegexOptions.IgnoreCase);
      return expr.Match(name).Success;
    }

    /// <summary>
    /// Checks if the foldername is ambigious (non descriptive)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool isFolderAmbiguous(string name)
    {
      string[] folders = new string[] { 
                "video_ts", "hvdvd_ts", "adv_obj", "bdmv", 
                "stream", "playlist", "clipinf", "backup"
            };

      // Name is too short or is marked as being multi-part
      if (name.Length == 1 || isFolderMultipart(name))
        return true;

      // Ignore specific names
      return folders.Any(folderName => name.Equals(folderName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if the file is a valid video file.
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    public static bool IsVideoFile(FileInfo fileInfo)
    {
      string fullPath = fileInfo.FullName;
      // Video Disc Standards Are OK
      if (IsVideoDiscPath(fullPath))
        return true;

      // Check files that pass the MediaPortal Video Extension list
      if (IsMediaPortalVideoFile(fileInfo))
      {
        string ext = fileInfo.Extension.ToLower();
        string name = fileInfo.Name.ToLower();

        // DVD: Non-Standalone content is invalid
        if (ext == ".vob" && Regex.Match(name, @"(video_ts|vts_).+", RegexOptions.IgnoreCase).Success)
          return false;

        // DVD: Filter ifo's that are not called video_ts.ifo
        // but allow them when we don't have a video_ts.ifo in the same folder
        if (ext == ".ifo" && name != "video_ts.ifo")
          if (File.Exists(fileInfo.FullName.ToLower().Replace(name, "video_ts.ifo")))
            return false;

        // Bluray: the only valid bluray file would already passed the method, we filter the rest
        if (ext == ".bdmv")
          return false;

        // Bluray: m2ts files sitting in a stream folder are part of a bluray disc
        if (ext == ".m2ts" && fileInfo.Directory.Name.Equals("stream", StringComparison.OrdinalIgnoreCase))
          return false;

        // HD-DVD: evo files sitting in a hvdvd_ts folder are part of a hddvd disc
        if (ext == ".evo" && fileInfo.Directory.Name.Equals("hvdvd_ts", StringComparison.OrdinalIgnoreCase))
          return false;

        // HD-DVD: .dat files other than discid.dat should be ignored
        if (ext == ".dat" && name != "discid.dat")
          return false;

        // if we made it this far we have a winner
        return true;
      }

      // we did not pass so return false
      return false;
    }

    /// <summary>
    /// Returns the Video Disc Type
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static VideoDiscType GetVideoDiscType(string path)
    {
      foreach (VideoDiscType format in EnumToList<VideoDiscType>().Where(format => format != VideoDiscType.UnknownFormat).Where(format => path.EndsWith(GetEnumValueDescription(format), StringComparison.OrdinalIgnoreCase)))
      {
        return format;
      }
      return VideoDiscType.UnknownFormat;
    }

    /// <summary>
    /// Check if the path specified is a video disc standard
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsVideoDiscPath(string path)
    {
      return EnumToList<VideoDiscType>().Where(format => format != VideoDiscType.UnknownFormat).Any(format => path.EndsWith(GetEnumValueDescription(format), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns the full path to the video disc or null if it doesn't find one.
    /// </summary>
    /// <param name="drive"></param>
    /// <returns></returns>
    public static string GetVideoDiscPath(string drive)
    {
      FileInfo discPath;
      foreach (VideoDiscType format in EnumToList<VideoDiscType>().Where(format => format != VideoDiscType.UnknownFormat))
      {
        discPath = new FileInfo(drive + GetEnumValueDescription(format));
        discPath.Refresh();
        if (discPath.Exists)
          return discPath.FullName;
      }
      return null;
    }

    /// <summary>
    /// Checks if the directory is a logical drive root
    /// </summary>
    /// <param name="directory"></param>
    /// <returns>True if this is a root</returns>
    public static bool IsDriveRoot(DirectoryInfo directory)
    {
      return IsDriveRoot(directory.FullName);
    }

    /// <summary>
    /// Checks if the given path is a logical drive root
    /// </summary>
    /// <param name="path"></param>
    /// <returns>True if this is a root</returns>
    public static bool IsDriveRoot(string path)
    {
      if (path.Length < 4)
        return (path.Substring(1, 1) == ":");

      return false;
    }

    /// <summary>
    /// Returns the base directory for the movie files that are part of the input directory
    /// </summary>
    /// <param name="directory">directory to start in</param>
    /// <returns>the base directory for the movie files</returns>
    public static DirectoryInfo GetMovieBaseDirectory(DirectoryInfo directory)
    {
      DirectoryInfo dirLevel = directory;
      while (isFolderAmbiguous(dirLevel.Name) && dirLevel.Root != dirLevel)
        dirLevel = dirLevel.Parent;

      return dirLevel;
    }

    ///// <summary>
    ///// Check if the file is classified as sample
    ///// </summary>
    ///// <param name="file">file to check</param>
    ///// <returns>True if file is a sample file</returns>
    //public static bool isSampleFile(FileInfo file) {
    //    // Set sample max size in bytes
    //    long sampleMaxSize = long.Parse(MovingPicturesCore.SettingsManager["importer_sample_maxsize"].Value.ToString()) * 1024 * 1024;
    //    // Create the sample filter regular expression
    //    Regex expr = new Regex(MovingPicturesCore.SettingsManager["importer_sample_keyword"].Value.ToString(), RegexOptions.IgnoreCase);
    //    // Return result of given conditions         
    //    return ((file.Length < sampleMaxSize) && expr.Match(file.Name).Success);
    //}

    ///// <summary>
    ///// Get the number of video files (excluding sample files) that are in a folder
    ///// </summary>
    ///// <param name="folder">the directory to count video files in</param>
    ///// <returns>total number of files found in the folder</returns>
    //public static int GetVideoFileCount(DirectoryInfo folder) {
    //    int rtn = 0;
    //    FileInfo[] fileList = folder.GetFiles("*");
    //    foreach (FileInfo currFile in fileList) {
    //        // count the number of non-sample video files in the folder
    //        if (IsVideoFile(currFile))
    //            if (!isSampleFile(currFile))
    //                rtn++;
    //    }

    //    // Return count
    //    return rtn;
    //}       

    ///// <summary>
    ///// Checks if a folder contains a maximum amount of video files
    ///// This is used to determine if a folder is dedicated to one movie
    ///// </summary>
    ///// <param name="folder">the directory to check</param>
    ///// <param name="expectedCount">maximum count</param>
    ///// <returns>True if folder is dedicated</returns>
    //public static bool isFolderDedicated(DirectoryInfo folder, int expectedCount) {
    //    return (GetVideoFileCount(folder) <= expectedCount);
    //}

    #endregion

    #region MediaPortal

    /// <summary>
    /// Checks if file has valid video Extensions (as specified by media portal
    /// </summary>
    /// <remarks>
    /// MediaPortal Dependency!
    /// </remarks>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsMediaPortalVideoFile(FileInfo file)
    {
      return MediaPortal.Util.Utils.VideoExtensions.Cast<string>().Any(ext => file.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region DirectShowLib

    /// <summary>
    /// Get Disc ID as a string
    /// </summary>
    /// <param name="path">CD/DVD path</param>
    /// <returns>Disc ID</returns>
    public static string GetDiscIdString(string path)
    {
      long id = GetDiscId(path);
      return id != 0 ? Convert.ToString(id, 16) : "";
    }

    /// <summary>
    /// Get Disc ID as 64-bit signed integer
    /// </summary>
    /// <param name="path">CD/DVD path</param>
    /// <returns>Disc ID</returns>
    public static long GetDiscId(string path)
    {
      long discID = 0;
      LogMyFilms.Debug("Generating DiscId for: " + path);
      try
      {
        // get the path to the video_ts folder
        string vtsPath = path.ToLower().Replace(@"\video_ts.ifo", @"\");
        // This will get the microsoft generated DVD Disc ID
        IDvdInfo2 dvdInfo = (IDvdInfo2)new DVDNavigator();
        dvdInfo.GetDiscID(vtsPath, out discID);
      }
      catch (Exception e)
      {
        LogMyFilms.Error(e, "Error while retrieving disc id for: " + path);
      }
      return discID;
    }

    #endregion


    /// <summary>
    /// Get a hash representing the standard identifier for this format.
    /// Currently supported are the DVD/Bluray Disc ID and the OpenSubtitles.org Movie Hash.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="videoPath">path to the main video file</param>
    /// <returns>Hexadecimal string representing the identifier or NULL</returns>
    public static string GetIdentifier(VideoFormat self, string videoPath)
    {
      string hashID = null;
      switch (self)
      {
        case VideoFormat.DVD:
          {
            // get the path to the video_ts folder
            string vtsPath = videoPath.ToLower().Replace(@"\video_ts.ifo", @"\");
            // This will get the microsoft generated DVD Disc ID
            try
            {
              // get the disc id using the DirectShowLib method
              IDvdInfo2 dvdInfo = (IDvdInfo2)new DVDNavigator();
              long discID = 0;
              dvdInfo.GetDiscID(vtsPath, out discID);
              // if we got a disc id, we convert it to a hexadecimal string
              if (discID != 0) hashID = Convert.ToString(discID, 16);
            }
            catch (Exception e)
            {
              if (e.GetType() == typeof(ThreadAbortException))
                throw e;

              LogMyFilms.Debug(e, "Disc ID: Failed, Path='" + vtsPath + "', Format='" + self.ToString() + "' ");
            }
          }
          break;
        case VideoFormat.Bluray:
          break;
        case VideoFormat.File:
          break;
      }

      if (String.IsNullOrEmpty(hashID))
      {
        LogMyFilms.Debug("Failed Identifier: Path='{0}', Format='{1}' ", videoPath, self);
      }
      else
      {
        LogMyFilms.Debug("Identifier: Path='{0}', Format='{1}', Hash='{2}' ", videoPath, self, hashID);
      }

      // Return the result
      return hashID;
    }

    #region MovieHash

    public static string GetMovieHashString(string filename)
    {
      string hash;
      try
      {
        Log.Debug("Generating FileHash for: " + filename);
        byte[] moviehash = ComputeMovieHash(filename);
        hash = ToHexadecimal(moviehash);
      }
      catch (Exception e)
      {
        Log.Error("Error while generating FileHash for: " + filename, e);
        hash = null;
      }
      return hash;
    }

    private static byte[] ComputeMovieHash(string filename)
    {
      byte[] result;
      using (Stream input = File.OpenRead(filename))
      {
        result = ComputeMovieHash(input);
      }
      return result;
    }

    private static byte[] ComputeMovieHash(Stream input)
    {
      ulong lhash;
      long streamsize;
      streamsize = input.Length;
      lhash = (ulong)streamsize;

      long i = 0;
      byte[] buffer = new byte[sizeof(long)];
      input.Position = 0;
      while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
      {
        i++;
        unchecked { lhash += BitConverter.ToUInt64(buffer, 0); }
      }

      input.Position = Math.Max(0, streamsize - 65536);
      i = 0;
      while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
      {
        i++;
        unchecked { lhash += BitConverter.ToUInt64(buffer, 0); }
      }
      byte[] result = BitConverter.GetBytes(lhash);
      Array.Reverse(result);
      return result;
    }

    private static string ToHexadecimal(byte[] bytes)
    {
      StringBuilder hexBuilder = new StringBuilder();
      foreach (byte t in bytes)
      {
        hexBuilder.Append(t.ToString("x2"));
      }
      return hexBuilder.ToString();
    }

    #endregion

    // Classes added by Guzzi for use in "Trailer Search & Register" (Rework)
    #region GetFiles (string Path)
    /// <summary>
    /// Gets all the files in the given folder path and all its subdirectories
    /// </summary>
    public static string[] GetFiles(string path)
    {
      ArrayList files = new ArrayList();
      getFiles(path, ref files);

      return (string[])files.ToArray(typeof(string));
    }
    private static void getFiles(string path, ref ArrayList files)
    {
      try
      {
        string[] folders = Directory.GetDirectories(path);
        foreach (string t in folders)
          getFiles(t, ref files);


        string[] curFiles = Directory.GetFiles(path);
        files.AddRange(curFiles);
      }
      catch
      { }
    }
    #endregion

    #region GetFiles (string path, string[] searchPatterns, bool includeSubFolders) +1 overload
    /// <summary>
    /// Gets all the files in the given folder path and all its subdirectories.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPatterns">search patterns (ie, "*.exe")</param>
    /// <param name="includeSubFolders"></param>
    public static string[] GetFiles(string path, string[] searchPatterns, bool includeSubFolders)
    {
      ArrayList files = new ArrayList();
      if (includeSubFolders)
      {
        getFiles(path, searchPatterns, ref files);
      }
      else
      {
        try
        {
          foreach (string t in searchPatterns)
          {
            string[] curFiles = Directory.GetFiles(path, t);
            files.AddRange(curFiles);
          }
        }
        catch
        { }
      }
      return (string[])files.ToArray(typeof(string));
    }


    /// <summary>
    /// Gets all the files in the given folder path and all its subdirectories.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern">A series of valid search patterns, separated
    /// by ";". For example "*.jpg;prog*.exe"</param>
    /// <param name="includeSubFolders"></param>
    public static string[] GetFiles(string path, string searchPattern, bool includeSubFolders)
    {
      string[] patterns = searchPattern.Split(';');
      return GetFiles(path, patterns, includeSubFolders);
    }

    private static void getFiles(string path, string[] searchPattern, ref ArrayList files)
    {
      // Try to get the current directory's folders
      try
      {
        string[] folders = Directory.GetDirectories(path);
        foreach (string t in folders)
          getFiles(t, searchPattern, ref files);


        foreach (string t in searchPattern)
        {
          files.AddRange(Directory.GetFiles(path, t));
        }
      }
      catch
      { }
    }
    #endregion

    #region AMCUpdater Utils

    public static string GetDVDFolderName(string FileName)
    {
      //'Function to try and guess the correct movie name for a DVD image stored in a folder.
      //'DVD files may be stored directly in a folder with the name of the movie.
      //'DVD files may also retain their structure so .vob files will be in \VIDEO_TS\ - assume parent of that is movie name.

      //'filename = DVDs\Shawshank Redemption, the\VIDEO_TS.IFO
      //'filename = DVDs\Shawshank Redemption, the\VIDEO_TS\VIDEO_TS.IFO

      string TempString = "";
      string ReturnValue = "";

      //'Get the file name itself off the end: \VIDEO_TS.IFO
      string FileNameEnd = FileName.Substring(FileName.LastIndexOf(@"\") - 1);

      //'This trims the filename and leaves DVDs\ShawshankRedemption, the"
      FileName = FileName.Replace(FileNameEnd, "");

      TempString = FileName.Contains(@"\") ? FileName.Substring(FileName.LastIndexOf(@"\")) : FileName;

      if (TempString.ToLower() == "video_ts")
      {
        TempString = FileName.Replace(TempString, "");
        //'Check that there isn't a trailing backslash (probably is)
        if (TempString.EndsWith(@"\")) TempString = TempString.Substring(0, (TempString.Length) - 1);
        //'Check to see if we've still got a nested path.  Take the next level up if so.
        ReturnValue = TempString.Contains(@"\") ? TempString.Substring(TempString.LastIndexOf(@"\")) : TempString;
      }
      else
        ReturnValue = TempString;

      return ReturnValue;
    }

    public static string GetBRFolderName(string FileName)
    {
      //'Function to try and guess the correct movie name for a BR image stored in a folder.
      //'filename = BRs\IRON MAN\BDMV\index.bdmv
      //'filename = BRs\IRON MAN\index.bdmv

      string TempString;
      string ReturnValue = "";

      //'Get the file name itself off the end: \index.bdmv
      string FileNameEnd = FileName.Substring(FileName.LastIndexOf(@"\") - 1);

      //'This trims the filename and leaves BRs\IRON MAN"
      FileName = FileName.Replace(FileNameEnd, "");

      TempString = FileName.Contains(@"\") ? FileName.Substring(FileName.LastIndexOf(@"\")) : FileName;

      if (TempString.ToLower() == "bdmv")
      {
        TempString = FileName.Replace(TempString, "");
        //'Check that there isn't a trailing backslash (probably is)
        if (TempString.EndsWith(@"\")) TempString = TempString.Substring(0, (TempString.Length) - 1);
        //'Check to see if we've still got a nested path.  Take the next level up if so.
        ReturnValue = TempString.Contains(@"\") ? TempString.Substring(TempString.LastIndexOf(@"\")) : TempString;
      }
      else
        ReturnValue = TempString;

      return ReturnValue;

    }

    public static string RemoveNastyCharacters(string strText)
    {
      Regex RegCheck;
      string NewText;
      const string RegexCleanFilters = @"\([0-9][0-9][0-9][0-9]\)|\(.*?\)|\[.*?\]|\{.*?\}|tt\d{7}|-\s+\d{4}$|\s+1$|\s\d{4}\.";

      foreach (string regexFilter in RegexCleanFilters.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Where(regexFilter => regexFilter.Length > 0))
      {
        if (regexFilter.Length == 1) //'Probably not a regex, due to complexity of any single character, just do a replace.
          NewText = strText.Replace(regexFilter, " ");
        else
        {
          //'This should work for any multi-character string:
          RegCheck = new Regex(regexFilter);
          NewText = RegCheck.Replace(strText, " ");
        }
        if (NewText.Trim().Length > 0) //'Check to ensure last operation didn't wipe the string out!
          strText = NewText;
        else //'If NewText is blank, then exit here with the previous value of strText
          continue;
      }

      //'Tidy up the beginning and end of the string:
      strText = strText.Trim();

      //'Loop through to remove any groups of spaces left by character replacement:
      if (strText.Contains("  "))
      {
        while (strText.Contains("  "))
        {
          strText = strText.Replace("  ", " ");
        }
      }
      return strText;
    }

    #endregion

  }
}
