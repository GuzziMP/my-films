using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Linq;
using System.Text;
//using Microsoft.WindowsAPICodePack.Shell;
using MediaPortal.Configuration;
using MediaPortal.Services;
using MediaPortal.Util;

namespace Grabber
{
  public class ThumbCreator
  {
    // updated mtn build: http://sourceforge.net/p/moviethumbnail/patches/5/

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    private const string ExtractApp = "mtn.exe";

    private static string ExtractorPath = Config.GetFile(Config.Dir.Base, "MovieThumbnailer", ExtractApp);
    // private static string ExtractorPath = Config.GetFolder(Config.Dir.Base) + @"\MyFilms\MTN\" + ExtractApp;
    private static int PreviewColumns = 2;
    private static int PreviewRows = 3;
    private static bool LeaveShareThumb = false;
    private static string IndividualShots = "";
    private static string LimitScanArea = "";
    private static int ArtworkWidth = 0;
    private static int ArtworkHeight = 0;
    private static bool BlacklistingIsEnabled = true;
    private static bool keepMainImage = false;

    #region Public methods

    //[MethodImpl(MethodImplOptions.Synchronized)]
    //public static bool CreateVideoThumb(string aVideoPath, bool aOmitCredits)
    //{
    //  string sharethumb = Path.ChangeExtension(aVideoPath, ".jpg");
    //  if (File.Exists(sharethumb))
    //    return true;
    //  else
    //    return CreateVideoThumb(aVideoPath, sharethumb, false, aOmitCredits);
    //}

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static bool CreateVideoThumbForAmCupdater(string aVideoPath, string aThumbPath, bool aOmitCredits, int columns, int rows, string imageType, bool saveIndividualShots, bool keepmainimage, int snapshotPosition)
    {
      PreviewColumns = columns;
      PreviewRows = rows;
      ArtworkWidth = 0;
      LimitScanArea = "";
      BlacklistingIsEnabled = true;
      IndividualShots = saveIndividualShots ? "-I " : "";

      if (imageType == "Cover")
      {
        ArtworkWidth = 400;
        ArtworkHeight = 600;
        BlacklistingIsEnabled = false;
      }
      if (imageType == "Fanart")
      {
        ArtworkWidth = 1920;
        ArtworkHeight = 1080;
        BlacklistingIsEnabled = false;
      }
      if (keepmainimage)
      {
        keepMainImage = true;
      }
      else
      {
        keepMainImage = false;
        ArtworkWidth = 0; // to get single images in original size
      }

      LogMyFilms.Debug("VideoThumbCreator: Settings loaded - using {0} columns and {1} rows.", PreviewColumns, PreviewRows);
      //if (NeedsConfigRefresh)

      if (String.IsNullOrEmpty(aVideoPath) || String.IsNullOrEmpty(aThumbPath))
      {
        LogMyFilms.Warn("VideoThumbCreator: Invalid arguments to generate thumbnails of your video!");
        return false;
      }
      if (!File.Exists(aVideoPath))
      {
        LogMyFilms.Warn("VideoThumbCreator: File {0} not found!", aVideoPath);
        return false;
      }
      if (!File.Exists(ExtractorPath))
      {
        LogMyFilms.Warn("VideoThumbCreator: No {0} found to generate thumbnails of your video!", ExtractApp);
        return false;
      }
      var blacklist = GlobalServiceProvider.Get<IVideoThumbBlacklist>();
      if (BlacklistingIsEnabled && blacklist != null && blacklist.Contains(aVideoPath))
      {
        LogMyFilms.Debug("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", aVideoPath);
        return false;
      }

      // Params for ffmpeg
      // string ExtractorArgs = string.Format(" -i \"{0}\" -vframes 1 -ss {1} -s {2}x{3} \"{4}\"", aVideoPath, @"00:08:21", (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, aThumbPath);

      // Params for mplayer (outputs 00000001.jpg in video resolution into working dir) -vf scale=600:-3
      //string ExtractorArgs = string.Format(" -noconsolecontrols -nosound -vo jpeg:quality=90 -vf scale -frames 1 -ss {0} \"{1}\"", "501", aVideoPath);

      // Params for mtm (http://moviethumbnail.sourceforge.net/usage.en.html)
      //   -D 8         : edge detection; 0:off >0:on; higher detects more; try -D4 -D6 or -D8
      //   -B 420/E 600 : omit this seconds from the beginning / ending TODO: use pre- / postrecording values
      //   -c 2 / r 2   : # of column / # of rows
      //   -b 0.60      : skip if % blank is higher; 0:skip all 1:skip really blank >1:off
      //   -h 100       : minimum height of each shot; will reduce # of column to fit
      //   -t           : time stamp off
      //   -i           : info text off
      //   -I           : save individual shots too
      //   -w 0         : width of output image; 0:column * movie width
      //   -n           : run at normal priority
      //   -W           : dont overwrite existing files, i.e. update mode
      //   -P           : dont pause before exiting; override -p

      const double flblank = 0.6;
      string blank = flblank.ToString("F", CultureInfo.CurrentCulture);

      int preGapSec = 5;
      int postGapSec = 5;
      if (aOmitCredits)
      {
        preGapSec = 420;
        postGapSec = 600;
      }
      if (snapshotPosition > 0) // this is for single picture snapshots as fanart !
      {
        if (snapshotPosition > 2)
          preGapSec = snapshotPosition - 2;
        else
          preGapSec = snapshotPosition;
        postGapSec = 0;
        PreviewColumns = 1;
        PreviewRows = 1;
        LimitScanArea = "-C 2 -z "; // to limit the snapshotarea to 2 seconds and used unecact searching for speedup
      }

      bool Success = false;
      string extractorArgs = string.Format(" -D 6 -B {0} {9}-E {1} -c {2} -r {3} -b {4} -t -i {8}-w {5} -n -O \"{6}\" -P \"{7}\"",
                                           preGapSec,
                                           postGapSec,
                                           PreviewColumns,
                                           PreviewRows,
                                           blank,
                                           0, //ArtworkWidth, 
                                           aThumbPath.Substring(0, aThumbPath.LastIndexOf("\\")),
                                           aVideoPath,
                                           IndividualShots,
                                           LimitScanArea);
      string extractorFallbackArgs = string.Format(" -D 8 -B {0} {9}-E {1} -c {2} -r {3} -b {4} -t -i {8}-w {5} -n -O \"{6}\" -P \"{7}\"",
                                           0,
                                           0,
                                           PreviewColumns,
                                           PreviewRows,
                                           blank,
                                           0, //ArtworkWidth, 
                                           aThumbPath.Substring(0, aThumbPath.LastIndexOf("\\")),
                                           aVideoPath,
                                           IndividualShots,
                                           LimitScanArea);
      // Honour we are using a unix app
      extractorArgs = extractorArgs.Replace('\\', '/');
      try
      {
        // Use this for the working dir to be on the safe side
        string tempPath = Path.GetTempPath();
        string outputThumbtmp = Path.Combine(Path.GetDirectoryName(aThumbPath), Path.GetFileName(string.Format("{0}_s{1}", Path.ChangeExtension(aVideoPath, null), ".jpg")));
        string outputDirectory = Path.GetDirectoryName(aThumbPath);
        string searchmask = Path.GetFileNameWithoutExtension(aVideoPath) + "_";
        // string OutputThumbtmp = string.Format("{0}_s{1}", Path.ChangeExtension(aThumbPath, null), Path.GetExtension(aThumbPath));
        string outputThumb = aThumbPath;
        string outputName = Path.GetFileNameWithoutExtension(aThumbPath);

        if (!File.Exists(outputThumb)) // No thumb in share although it should be there
        {
          LogMyFilms.Debug("VideoThumbCreator: No thumb in share {0} - trying to create one with arguments: {1}", outputThumb, extractorArgs);
          Success = GrabUtil.StartProcess(ExtractorPath, extractorArgs, tempPath, 60000, true, GetMtnConditions());
          if (Success)
          {
            LogMyFilms.Debug("First try successful !");
          }
          else
          {
            // Maybe the pre-gap was too large or not enough sharp & light scenes could be caught
            Thread.Sleep(250);
            LogMyFilms.Debug("First try failed - trying fallback with arguments: {0}", extractorFallbackArgs);
            Success = GrabUtil.StartProcess(ExtractorPath, extractorFallbackArgs, tempPath, 120000, true, GetMtnConditions());
            if (Success)
            {
              LogMyFilms.Debug("Second try successful !");
            }
            else
            {
              LogMyFilms.Info("VideoThumbCreator: {0} has not been executed successfully with arguments: {1}", ExtractApp, extractorFallbackArgs);
              //try
              //{
              //  using (ShellObject Item = ShellObject.FromParsingName(recFileName))
              //  {
              //    Item.Thumbnail.RetrievalOption = ShellThumbnailRetrievalOption.Default;
              //    Item.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
              //    Bitmap _bitmap;
              //    _bitmap = Item.Thumbnail.LargeBitmap;
              //    if (_bitmap != null)
              //    {
              //      _bitmap.Save(thumbNail, System.Drawing.Imaging.ImageFormat.Jpeg);
              //      LogMyFilms.Info("RecordedTV: Thumbnail successfully created for -- {0}", recFileName);
              //    }
              //  }
              //}
              //catch
              //{
              //  LogMyFilms.Info("RecordedTV: No thumbnail created for -- {0}", recFileName);
              //}              
            }
          }
          Thread.Sleep(250); // give the system a few IO cycles
          GrabUtil.KillProcess(Path.ChangeExtension(ExtractApp, null)); // make sure there's no process hanging
          try
          {
            // remove the _s which mtn appends to its files
            if (ArtworkWidth > 0 && keepMainImage && snapshotPosition == 0)
            {
              if (File.Exists(outputThumbtmp))
              {
                Picture.CreateThumbnail(outputThumbtmp, outputThumb, ArtworkWidth, ArtworkHeight, 0, false); // Create a smaller Thumb for proper dimensions //Picture.CreateThumbnail(OutputThumbtmp, OutputThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, false);
                File.Delete(outputThumbtmp);
                Thread.Sleep(50);
              }
            }
            else
            {
              File.Move(outputThumbtmp, outputThumb);
            }

            // rename the singleimages to destination names
            string[] files = Directory.GetFiles(outputDirectory, searchmask + "*.*", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
              string filenew = file.Replace(searchmask, outputName);
              File.Move(file, filenew);
            }
            if (!keepMainImage)
            {
              File.Delete(outputThumb);
              Thread.Sleep(50);
            }
          }
          catch (FileNotFoundException)
          {
            LogMyFilms.Debug("VideoThumbCreator: {0} did not extract a thumbnail to: {1}", ExtractApp, outputThumbtmp);
          }
          catch (Exception)
          {
            try
            {
              // Clean up
              File.Delete(outputThumbtmp);
              Thread.Sleep(50);
              if (!keepMainImage)
              {
                File.Delete(outputThumb);
                Thread.Sleep(50);
              }
            }
            catch (Exception eix)
            {
              LogMyFilms.Debug("VideoThumbCreator: {0} did not extract - error: '{1}'", ExtractApp, eix.Message);
            }
          }
        }
        Thread.Sleep(30);
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("VideoThumbCreator: Thumbnail generation failed - {0}!", ex.ToString());
      }
      if (File.Exists(aThumbPath))
      {
        // Create a smaller Thumb for proper dimensions
        //Picture.CreateThumbnail(aThumbPath, Utils.ConvertToLargeCoverArt(aThumbPath), (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, false);
        return true;
      }
      else
      {
        if (blacklist != null && BlacklistingIsEnabled)
        {
          blacklist.Add(aVideoPath);
        }
        return false;
      }
    }

    public static bool CreateVideoThumb(string aVideoPath, string aThumbPath, bool aCacheThumb, bool aOmitCredits, int columns, int rows, bool doShareThumb, string imageType)
    {

      PreviewColumns = columns;
      PreviewRows = rows;
      LeaveShareThumb = doShareThumb;
      if (imageType == "Cover")
      {
        // ToDo: Set Resolution Params to Coverimagesize
      }
      if (imageType == "Fanart")
      {
        // ToDo: Set Resolution Params to FullHD
      }

      LogMyFilms.Debug("VideoThumbCreator: Settings loaded - using {0} columns and {1} rows. Share thumb = {2}", PreviewColumns, PreviewRows, LeaveShareThumb);
      //if (NeedsConfigRefresh)

      if (String.IsNullOrEmpty(aVideoPath) || String.IsNullOrEmpty(aThumbPath))
      {
        LogMyFilms.Warn("VideoThumbCreator: Invalid arguments to generate thumbnails of your video!");
        return false;
      }
      if (!File.Exists(aVideoPath))
      {
        LogMyFilms.Warn("VideoThumbCreator: File {0} not found!", aVideoPath);
        return false;
      }
      if (!File.Exists(ExtractorPath))
      {
        LogMyFilms.Warn("VideoThumbCreator: No {0} found to generate thumbnails of your video!", ExtractApp);
        return false;
      }
      if (!LeaveShareThumb && !aCacheThumb)
      {
        LogMyFilms.Warn("VideoThumbCreator: No share thumbs wanted by config option AND no caching wanted - where should the thumb go then? Aborting..");
        return false;
      }

      var blacklist = GlobalServiceProvider.Get<IVideoThumbBlacklist>();
      if (blacklist != null && blacklist.Contains(aVideoPath))
      {
        LogMyFilms.Debug("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", aVideoPath);
        return false;
      }

      // Params for ffmpeg
      // string ExtractorArgs = string.Format(" -i \"{0}\" -vframes 1 -ss {1} -s {2}x{3} \"{4}\"", aVideoPath, @"00:08:21", (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, aThumbPath);

      // Params for mplayer (outputs 00000001.jpg in video resolution into working dir) -vf scale=600:-3
      //string ExtractorArgs = string.Format(" -noconsolecontrols -nosound -vo jpeg:quality=90 -vf scale -frames 1 -ss {0} \"{1}\"", "501", aVideoPath);

      // Params for mtm (http://moviethumbnail.sourceforge.net/usage.en.html)
      //   -D 8         : edge detection; 0:off >0:on; higher detects more; try -D4 -D6 or -D8
      //   -B 420/E 600 : omit this seconds from the beginning / ending TODO: use pre- / postrecording values
      //   -c 2 / r 2   : # of column / # of rows
      //   -b 0.60      : skip if % blank is higher; 0:skip all 1:skip really blank >1:off
      //   -h 100       : minimum height of each shot; will reduce # of column to fit
      //   -t           : time stamp off
      //   -i           : info text off
      //   -w 0         : width of output image; 0:column * movie width
      //   -n           : run at normal priority
      //   -W           : dont overwrite existing files, i.e. update mode
      //   -P           : dont pause before exiting; override -p

      const double flblank = 0.6;
      string blank = flblank.ToString("F", CultureInfo.CurrentCulture);

      int preGapSec = 5;
      int postGapSec = 5;
      if (aOmitCredits)
      {
        preGapSec = 420;
        postGapSec = 600;
      }
      bool success = false;
      string extractorArgs = string.Format(" -D 6 -B {0} -E {1} -c {2} -r {3} -b {4} -t -i -w {5} -n -O \"{6}\" -P \"{7}\"",
                                           preGapSec, postGapSec, PreviewColumns, PreviewRows, blank, 0, aThumbPath.Substring(0, aThumbPath.LastIndexOf("\\")), aVideoPath);
      string extractorFallbackArgs = string.Format(" -D 8 -B {0} -E {1} -c {2} -r {3} -b {4} -t -i -w {5} -n -O \"{6}\" -P \"{7}\"", 0, 0, PreviewColumns, PreviewRows, blank, 0, aThumbPath.Substring(0, aThumbPath.LastIndexOf("\\")), aVideoPath);
      // Honour we are using a unix app
      extractorArgs = extractorArgs.Replace('\\', '/');
      try
      {
        // Use this for the working dir to be on the safe side
        string tempPath = Path.GetTempPath();
        string t = string.Format("{0}_s{1}", Path.ChangeExtension(aVideoPath, null), ".jpg");
        string videoFilename = t.Substring(t.LastIndexOf("\\"));
        string outputThumb = aThumbPath.Substring(0, aThumbPath.LastIndexOf("\\") + 1) + videoFilename;
        string shareThumb = outputThumb.Replace("_s.jpg", ".jpg");

        if ((LeaveShareThumb && !File.Exists(shareThumb)) // No thumb in share although it should be there
            || (!LeaveShareThumb && !File.Exists(aThumbPath))) // No thumb cached and no chance to find it in share
        {
          //LogMyFilms.Debug("VideoThumbCreator: No thumb in share {0} - trying to create one with arguments: {1}", ShareThumb, ExtractorArgs);
          success = GrabUtil.StartProcess(ExtractorPath, extractorArgs, tempPath, 45000, true, GetMtnConditions());
          if (success)
          {
            LogMyFilms.Debug("First try successful !");
          }
          else
          {
            // Maybe the pre-gap was too large or not enough sharp & light scenes could be caught
            Thread.Sleep(250);
            success = GrabUtil.StartProcess(ExtractorPath, extractorFallbackArgs, tempPath, 90000, true, GetMtnConditions());
            if (success)
            {
              LogMyFilms.Debug("Second try successful !");
            }
            else
            {
              LogMyFilms.Info("VideoThumbCreator: {0} has not been executed successfully with arguments: {1}", ExtractApp, extractorFallbackArgs);
            }
          }
          // give the system a few IO cycles
          Thread.Sleep(250);
          // make sure there's no process hanging
          GrabUtil.KillProcess(Path.ChangeExtension(ExtractApp, null));
          try
          {
            // remove the _s which mdn appends to its files
            File.Move(outputThumb, shareThumb);
          }
          catch (FileNotFoundException)
          {
            LogMyFilms.Debug("VideoThumbCreator: {0} did not extract a thumbnail to: {1}", ExtractApp, outputThumb);
          }
          catch (Exception)
          {
            try
            {
              // Clean up
              File.Delete(outputThumb);
              Thread.Sleep(50);
            }
            catch (Exception) { }
          }
        }
        else
        {
          // We have a thumbnail in share but the cache was wiped out - make sure it is recreated
          if (LeaveShareThumb && File.Exists(shareThumb) && !File.Exists(aThumbPath))
            success = true;
        }

        Thread.Sleep(30);

        if (aCacheThumb && success)
        {
          if (Picture.CreateThumbnail(shareThumb, aThumbPath, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, false))
            Picture.CreateThumbnail(shareThumb, Utils.ConvertToLargeCoverArt(aThumbPath), (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, false);
        }

        if (!LeaveShareThumb)
        {
          try
          {
            File.Delete(shareThumb);
            Thread.Sleep(30);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("VideoThumbCreator: {0} did not extract - error: '{1}'", ExtractApp, ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("VideoThumbCreator: Thumbnail generation failed - {0}!", ex.ToString());
      }
      if (File.Exists(aThumbPath))
      {
        return true;
      }
      else
      {
        if (blacklist != null)
        {
          blacklist.Add(aVideoPath);
        }
        return false;
      }
    }

    public static string GetThumbExtractorVersion()
    {
      try
      {
        //System.Diagnostics.FileVersionInfo newVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(ExtractorPath);
        //return newVersion.FileVersion;
        // mtn.exe has no version info, so let's use "time modified" instead
        var fi = new FileInfo(ExtractorPath);
        return fi.LastWriteTimeUtc.ToString("s"); // use culture invariant format
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("GetThumbExtractorVersion failed:");
        LogMyFilms.Error(ex);
        return "";
      }
    }

    #endregion

    #region Private methods

    private static GrabUtil.ProcessFailedConditions GetMtnConditions()
    {
      var mtnStat = new GrabUtil.ProcessFailedConditions();
      // The input file is shorter than pre- and post-recording time
      mtnStat.AddCriticalOutString("net duration after -B & -E is negative");
      mtnStat.AddCriticalOutString("all rows're skipped?");
      mtnStat.AddCriticalOutString("step is zero; movie is too short?");
      mtnStat.AddCriticalOutString("failed: -");
      // unsupported video format by mtn.exe - maybe there's an update?
      mtnStat.AddCriticalOutString("couldn't find a decoder for codec_id");

      mtnStat.SuccessExitCode = 0;

      return mtnStat;
    }

    #endregion
  }

}
