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

namespace MyFilmsPlugin.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Reflection;
  using System.Runtime.InteropServices;
  using System.Text.RegularExpressions;

  using MediaPortal.GUI.Library;

  public class ImageAllocator
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    static String s_sFontName;
    static List<String> s_PosterImageList = new List<string>();
    static List<String> s_OtherPersistentImageList = new List<string>();
    static List<String> s_OtherDiscardableImageList = new List<string>();
    static Size DefPosterSize = new Size(680, 1000);

    static ImageAllocator()
    {
    }

    #region Helper

    /// <summary>
    /// Takes an Image sFileName and tries to load it into MP' graphics memory
    /// If the sFileName was already in memory, it will not be reloaded (basically it caches)
    /// </summary>
    /// <param name="sFileName">The sFileName of the image to load, fails silently if it cannot be loaded</param>
    /// <param name="size">size of image</param>
    /// <returns>memory identifier</returns>
    public static string buildMemoryImageFromFile(string sFileName, Size size)
    {
      try
      {
        if (String.IsNullOrEmpty(sFileName) || !System.IO.File.Exists(sFileName)) return string.Empty;
        string ident = buildIdentifier(sFileName);
        //MPTVSeriesLog.WriteMultiLine("AsyncImageResource LoadFromMemory - " + Environment.StackTrace, MPTVSeriesLog.LogLevel.Debug);
        if (GUITextureManager.LoadFromMemory(null, ident, 0, size.Width, size.Height) > 0) return ident;
        else return buildMemoryImage(LoadImageFastFromFile(sFileName), ident, size, false);
      }
      catch (Exception e)
      {
        LogMyFilms.Error("Unable to add to MP's Graphics memory: " + sFileName + " Error: " + e.Message);
        return string.Empty;
      }
    }

    static string buildIdentifier(string name)
    {
      // note: GetHashCode() experiences strangeness with dissappearing textures
      // replace ';' to avoid issues with mediaportal texture splitting code            
      return "[MyFilms:" + name.Replace(";", "-") + "]";
    }

    /// <summary>
    /// Takes an Image and tries to load it into MP' graphics memory
    /// If the sFileName was already in memory, it will not be reloaded (basically it caches)
    /// </summary>
    /// <param name="image">The System.Drawing.Bitmap to be loaded</param>
    /// <param name="identifier">A unique identifier for the image so it can be retrieved later on</param>
    /// <param name="size">Dimensions of the image</param>
    /// <param name="buildIdentifier">switch if new identifier should be created</param>
    /// <returns>memory identifier</returns>
    public static string buildMemoryImage(Image image, string identifier, Size size, bool buildIdentifier)
    {
      string name = buildIdentifier ? ImageAllocator.buildIdentifier(identifier) : identifier;
      try
      {
        // we don't have to try first, if name already exists mp will not do anything with the image
        if (size.Height > 0 && (size.Height != image.Size.Height || size.Width != image.Size.Width)) //resize
        {
          image = Resize(image, size);
        }
        PerfWatcher.GetNamedWatch("add to TextureManager").Start();
        //MPTVSeriesLog.WriteMultiLine("AsyncImageResource LoadFromMemory - " + Environment.StackTrace, MPTVSeriesLog.LogLevel.Debug);
        GUITextureManager.LoadFromMemory(image, name, 0, size.Width, size.Height);
        PerfWatcher.GetNamedWatch("add to TextureManager").Stop();
      }
      catch (Exception)
      {
        LogMyFilms.Debug("Unable to add to MP's Graphics memory: " + identifier);
        return string.Empty;
      }
      return name;
    }

    public static string ExtractFullName(string identifier)
    {
      const string RegExp = @"\[MyFilms:(.*)\]";
      Regex engine = new Regex(RegExp, RegexOptions.IgnoreCase);
      Match match = engine.Match(identifier);
      return match.Success ? match.Groups[1].Value : identifier;
    }

    public static void Flush(List<String> toFlush)
    {
      foreach (String sTextureName in toFlush)
      {
        Flush(sTextureName);
      }
      toFlush.Clear();
    }

    public static void Flush(string sTextureName)
    {
      GUITextureManager.ReleaseTexture(sTextureName);
    }
    #endregion

    /// <summary>
    /// Set the font name to be used to create dummy banners
    /// </summary>
    /// <param name="sFontName">Size of the image to be generated</param>
    /// <returns>nothing</returns>
    public static void SetFontName(String sFontName)
    {
      s_sFontName = sFontName;
    }

    public static String GetOtherImage(string sFileName, Size size, bool bPersistent)
    {
      return GetOtherImage(null, sFileName, size, bPersistent);
    }

    public static String GetOtherImage(Image i, string sFileName, Size size, bool bPersistent)
    {
      string sTextureName = i != null ? buildMemoryImage(i, sFileName, size, true) : buildMemoryImageFromFile(sFileName, size);
      if (bPersistent)
      {
        if (!s_OtherPersistentImageList.Contains(sTextureName))
          s_OtherPersistentImageList.Add(sTextureName);
      }
      else if (!s_OtherDiscardableImageList.Contains(sTextureName))
        s_OtherDiscardableImageList.Add(sTextureName);
      return sTextureName;
    }

    public static void FlushAll()
    {
      FlushPosters();
    }

    public static void FlushPosters()
    {
      Flush(s_PosterImageList);
    }

    #region FastBitmapLoading From File
    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
    private static extern int GdipLoadImageFromFile(string filename, out IntPtr image);
    private static Type imageType = typeof(System.Drawing.Bitmap);

    /// <summary>
    /// Loads an Image from a File by invoking GDI Plus instead of using build-in .NET methods, or falls back to Image.FromFile
    /// Can perform up to 10x faster
    /// </summary>
    /// <param name="filename">The filename to load</param>
    /// <returns>A .NET Image object</returns>
    public static Image LoadImageFastFromFile(string filename)
    {
      IntPtr image = IntPtr.Zero;
      Image i = null;
      try
      {
        // We are not using ICM at all, fudge that, this should be FAAAAAST!
        if (GdipLoadImageFromFile(filename, out image) != 0)
        {
          LogMyFilms.Debug("Reverting to slow ImageLoading for: " + filename);
          i = Image.FromFile(filename);
        }
        else i = (Image)imageType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { image });

      }
      catch (System.IO.FileNotFoundException fe)
      {
        LogMyFilms.Error("Image does not exist: " + filename + " - " + fe.Message);
      }
      catch (Exception e)
      {
        // this probably means the image is bad
        LogMyFilms.Error("Unable to load Imagefile (corrupt?): " + filename + " - " + e.Message);
        return null;
      }
      return i;
    }
    #endregion

    public static Bitmap Resize(Image img, Size size)
    {
      return new Bitmap(img, size);
    }
  }
}
