extern alias ExternalPlugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ExternalPlugins::OnlineVideos;
using ExternalPlugins::OnlineVideos.MediaPortal1;
using ExternalPlugins::OnlineVideos.MediaPortal1.Player;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using System.Threading;
using MediaPortal.Dialogs;
using MyFilmsPlugin.MyFilmsGUI;

namespace MyFilmsPlugin.Utils
{
  using PlayerFactory = ExternalPlugins::OnlineVideos.MediaPortal1.Player.PlayerFactory;

  public class OVplayer
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region Static Methods

    public static bool IsOnlineVideosReady()
    {
      try
      {
        string strUrl = ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetHoster("Youtube").GetHosterUrl();
        return !String.IsNullOrEmpty(strUrl);
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug(ex, "OnlineVideos is not ready!");
        return false;
        // throw;
      }
    }

    public static Dictionary<string, string> GetYoutubeDownloadUrls(string stream)
    {
      var downloadUrls = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(stream)) return downloadUrls;

      // use onlinevideo youtube siteutils to get
      // playback option urls as myfilms only gives us the html page
      if (stream.Contains("youtube.com"))
      {
        // get playback url from streamURL
        LogMyFilms.Debug("Getting playback options from page '{0}'", stream);

        try
        {
          //OnlineVideosHosterProxy ovHosterProxy = ExternalPlugins::OnlineVideos.CrossDomain.OnlineVideosAppDomain.Domain.CreateInstanceAndUnwrap(typeof(OnlineVideosHosterProxy).Assembly.FullName, typeof(OnlineVideosHosterProxy).FullName) as OnlineVideosHosterProxy;
          //downloadUrls = ovHosterProxy.GetDownloadUrls(stream);
          // var hosterBase = ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetHoster("Youtube");
          // downloadUrls = hosterBase.GetPlaybackOptions(stream);
          downloadUrls = ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetHoster("Youtube").GetPlaybackOptions(stream);
        }
        catch (Exception ex)
        {
          // LogMyFilms.Debug("playback option exception: '{0}' - '{1}'", ex.Message, ex.StackTrace);
          LogMyFilms.Debug("playback option exception: '{0}'", ex.Message);
          return downloadUrls;
        }

        if (downloadUrls != null)
        {
          LogMyFilms.Info("Found playback options: '{0}' for link '{1}'", downloadUrls.Count, stream);
          foreach (KeyValuePair<string, string> downloadUrl in downloadUrls) 
            LogMyFilms.Debug("playback option: '{0}'", downloadUrl.Key); // LogMyFilms.Info("playback option: '{0}' - '{1}'", downloadUrl.Key, downloadUrl.Value);
        }
        else
        {
          LogMyFilms.Info("No playback options found - request returned 'null'");
        }
      }
      return downloadUrls;
    }
    
    public static bool Play(string stream, bool showPlaybackQualitySelectionDialog, GUIAnimation searchanimation)
    {
      if (string.IsNullOrEmpty(stream)) return false;

      // use onlinevideo youtube siteutils to get playback urls as myfilms only gives us the html page
      if (stream.Contains("youtube.com"))
      {
        // get playback url from streamURL
        LogMyFilms.Info("Getting playback url from page '{0}'", stream);

        try
        {
          bool interactiveSuccessful = false;
          if (showPlaybackQualitySelectionDialog)
          {
            Dictionary<string, string> availableTrailerFiles = GetYoutubeDownloadUrls(stream);
            List<string> choiceView = new List<string>();

            //GUIWindowManager.SendThreadCallbackAndWait((p1, p2, o) =>
            //{
            //  {
                
            //  }
            //  return 0;
            //}, 0, 0, null);
            
            GUIDialogMenu dlg = (MediaPortal.Dialogs.GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return false;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798994)); // Select quality ...
            foreach (KeyValuePair<string, string> availableTrailerFile in availableTrailerFiles)
            {
              dlg.Add(availableTrailerFile.Key);
              choiceView.Add(availableTrailerFile.Value); // this is the download URL
            }
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedLabel == -1) return false;
            stream = choiceView[dlg.SelectedLabel];
            interactiveSuccessful = true;
          }

          if (!interactiveSuccessful)
          {
            // OnlineVideosHosterProxy ovHosterProxy = ExternalPlugins::OnlineVideos.CrossDomain.OnlineVideosAppDomain.Domain.CreateInstanceAndUnwrap(typeof(OnlineVideosHosterProxy).Assembly.FullName, typeof(OnlineVideosHosterProxy).FullName) as OnlineVideosHosterProxy;
            // stream = ovHosterProxy.GetVideoUrls(stream);
            stream = ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetHoster("Youtube").GetVideoUrl(stream);
          }
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("playback url exception: '{0}'", ex.Message);
          return false;
        }

        if (string.IsNullOrEmpty(stream))
        {
          LogMyFilms.Info("Unable to find playback url from stream!", stream);
          return false;
        }
        LogMyFilms.Info("Found playback url: '{0}'", stream);
      }

      LogMyFilms.Info("Preparing graph for playback of '{0}'", stream);

      PlayerFactory factory = new PlayerFactory(PlayerType.Internal, stream);
      bool? prepareResult = ((OnlineVideosPlayer)factory.PreparedPlayer).PrepareGraph();

      if (prepareResult != true)
      {
        LogMyFilms.Info("Failed to create Player graph.");
        return false;
      }

      GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
      {
        LogMyFilms.Info("OnlineVideo Pre-Buffering Started");
        if (((OnlineVideosPlayer)factory.PreparedPlayer).BufferFile(YouTubeSiteUtil))
        {
          LogMyFilms.Info("OnlineVideo Pre-Buffering Complete");
          return true;
        }
        else
        {
          return null;
        }

      },
      delegate(bool success, object result)
      {
        if (result is bool?)
        {
          (factory.PreparedPlayer as OVSPLayer).GoFullscreen = true;

          IPlayerFactory savedFactory = g_Player.Factory;
          g_Player.Factory = factory;
          try
          {
            g_Player.Play(stream, g_Player.MediaType.Video);
          }
          catch (Exception e)
          {
            LogMyFilms.Warn("Exception while playing stream: {0}", e.Message);
          }
          g_Player.Factory = savedFactory;
        }
        else
        {
          LogMyFilms.Error("Failed to Buffer stream.");
          factory.PreparedPlayer.Dispose();
        }
      },
      "PlayTrailerStream", false, searchanimation);

      return true;
    }

    /// <summary>
    /// returns the highest quality download options for a given preferred quality
    /// </summary>
    internal static KeyValuePair<string, string> GetPreferredQualityOption(Dictionary<string, string> downloadOptions, string preferredQuality)
    {
      // available mp4 options are:
      // 1. 320x240
      // 2. 426x240
      // 3. 640x360
      // 4. 1280x720
      // 5. 1920x1080 (currently no high resolutions supported by OV due to split video/audio and missing support in URLsplitter)

      IEnumerable<KeyValuePair<string, string>> options;
      switch (preferredQuality)
      {
        case "FHD":
          options = downloadOptions.Where(o => o.Key.Contains("1080"));
          if (options.Any())
          {
            return new KeyValuePair<string, string>(options.First().Key, options.First().Value);
          }
          else
          {
            return GetPreferredQualityOption(downloadOptions, "HD");
          }
        case "HD":
          options = downloadOptions.Where(o => o.Key.Contains("720"));
          if (options.Any())
          {
            return new KeyValuePair<string, string>(options.First().Key, options.First().Value);
          }
          else
          {
            return GetPreferredQualityOption(downloadOptions, "HQ");
          }
        case "HQ":
          options = downloadOptions.Where(o => o.Key.Contains("640x360 | mp4"));
          if (options.Any())
          {
            return new KeyValuePair<string, string>(options.First().Key, options.First().Value);
          }
          else
          {
            return GetPreferredQualityOption(downloadOptions, "LQ");
          }
        case "LQ":
          options = downloadOptions.Where(o => o.Key.Contains("426x240 | mp4"));
          if (options.Any())
          {
            return new KeyValuePair<string, string>(options.First().Key, options.First().Value);
          }
          else
          {
            return GetPreferredQualityOption(downloadOptions, "BadQ");
          }
        case "BadQ":
          options = downloadOptions.Where(o => o.Key.Contains("320x240 | mp4"));
          if (options.Any())
          {
            return new KeyValuePair<string, string>(options.First().Key, options.First().Value);
          }
          else
          {
            // use any available ...
            return GetPreferredQualityOption(downloadOptions, "");
          }

        default:
          // return any trailer, take last in the list, assuming that is best quality
          if (downloadOptions.Any())
          {
            return new KeyValuePair<string, string>(downloadOptions.Last().Key, downloadOptions.Last().Value);
          }
          break;
      }

      // no videos available
      return new KeyValuePair<string, string>(null, null);
    }

    static ExternalPlugins::OnlineVideos.Sites.SiteUtilBase _youTubeSiteUtil = null;
    private static ExternalPlugins::OnlineVideos.Sites.SiteUtilBase YouTubeSiteUtil
    {
      get
      {
        if (_youTubeSiteUtil == null)
        {
          LogMyFilms.Info("Getting YouTube site util from OnlineVideos.");
          ExternalPlugins::OnlineVideos.Sites.SiteUtilBase siteUtil;
          OnlineVideoSettings.Instance.SiteUtilsList.TryGetValue("YouTube", out siteUtil);
          _youTubeSiteUtil = siteUtil;

          LogMyFilms.Info("Finished getting YouTube site util, Success = '{0}'", siteUtil != null);
        }
        return _youTubeSiteUtil;
      }
    }
    
    #endregion
  }

  /// <summary>
  /// this class is needed because the Hoster class lives in the second appdomain, 
  /// and is not marked Serialiable and does not inherit from MarshalByRefObject, 
  /// so the object cannot cross appdomains
  /// </summary>
  class OnlineVideosHosterProxy : MarshalByRefObject
  {
    public OnlineVideosHosterProxy() { }

    public Dictionary<string, string> GetDownloadUrls(string url)
    {
      return ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetHoster("Youtube").GetPlaybackOptions(url);
    }

    public string GetVideoUrls(string url)
    {
      return ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetHoster("Youtube").GetVideoUrl(url);
    }

    // not yet used
    public List<ExternalPlugins::OnlineVideos.Hoster.HosterBase> GetAllHoster()
    {
      return ExternalPlugins::OnlineVideos.Hoster.HosterFactory.GetAllHosters();
    }

  }

}