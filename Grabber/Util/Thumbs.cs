using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Grabber.Util
{
  public class Thumbs
  {
    private static readonly ImageFormat _currentThumbFormat = ImageFormat.Jpeg;

    public static ImageFormat ThumbFormat
    {
      get
      {
        return _currentThumbFormat;
      }
    }

    static Thumbs()
    {
    }
  }
}