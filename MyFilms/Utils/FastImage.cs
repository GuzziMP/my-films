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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyFilmsPlugin.Utils
{
  public class ImageFast
  {
    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
    public static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

    private ImageFast()
    {
    }

    private static Type imageType = typeof(System.Drawing.Bitmap);

    public static Image FastFromFile(string filename)
    {
      filename = Path.GetFullPath(filename);
      IntPtr loadingImage = IntPtr.Zero;

      // We are not using ICM at all, fudge that, this should be FAAAAAST!
      if (GdipLoadImageFromFile(filename, out loadingImage) != 0)
      {
        throw new Exception("GDI+ threw a status error code.");
      }

      return (Bitmap)imageType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
    }

    public static Image CreateImage(string path, string texte, string sourceimage)
    {
      string loadimage = "";
      // Loading the image to draw 
      if (string.IsNullOrEmpty(sourceimage))
      {
        if (!System.IO.File.Exists(path.Substring(0, path.LastIndexOf("\\") + 1) + "Default.jpg"))
          MediaPortal.Util.Picture.CreateThumbnail(MyFilms.MyFilmsGUI.MyFilms.conf.DefaultCover, path.Substring(0, path.LastIndexOf("\\") + 1) + "\\Default.jpg", MyFilms.MyFilmsGUI.MyFilms.cacheThumbWith, MyFilms.MyFilmsGUI.MyFilms.cacheThumbHeight, 0, MediaPortal.Util.Thumbs.SpeedThumbsLarge);
        loadimage = path.Substring(0, path.LastIndexOf("\\") + 1) + "\\Default.jpg";
      }
      else loadimage = sourceimage;

      Image image = null;
      try
      { image = Image.FromFile(loadimage); }
      catch
      {
        return null;
      }

      // Creation of the bitmap to make the drawing 

      //          Bitmap bmp = new Bitmap(image.Width, image.Height);
      //Bitmap bmp = new Bitmap(150, 200);
      Bitmap bmp = new Bitmap(400, 600);
      // Creation of graphics to draw
      Graphics g = Graphics.FromImage(bmp);

      // It draws the image 
      g.DrawImage(image, 0, 0);

      // Draw rectangle for title text  
      // g.FillRectangle(Brushes.Gray, 50, 20, 300, 50);

      // the text is cut by separator space
      int wi = 0;
      string[] Sep = { " ", "-", "_", "&", "|", ",", ";" };
      string[] arSplit = texte.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
      float xfont = 0;
      float wfont = 0;
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        wfont = 340 / arSplit[wi].Length * 3 / 2;
        if (Equals(xfont, 0) || wfont < xfont)
          xfont = wfont;
      }
      // It adds a text 
      if (xfont > 64)
        xfont = 64;
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        g.DrawString(arSplit[wi].ToUpper(), new Font("Arial", xfont, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel), Brushes.White, new PointF(20, 30 + wi * xfont));
      }
      //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(1, 1));
      //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(1,image.Height - 64));
      //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(image.Width -64, 1));
      //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(image.Width-64, image.Height - 64));

      bmp.Save(path);
      return bmp;
    }

    /// <summary>
    /// Wandelt einen String in ein Bitmap (Image) um.
    /// </summary>
    /// <param name="Text">Beliebiger Text (auch mehrzeilig mit \n)</param>
    /// <param name="Schrift">Font als Objekt</param>
    /// <param name="ForeColor">Schriftfarbe</param>
    /// <param name="BackColor">Hintergrundfarbe</param>
    /// <returns></returns>
    public static Image Text2Bitmap(string Text, Font Schrift, Color ForeColor, Color BackColor)
    {
      int intWidth, intHeight;

      TextSize(Text, Schrift, out intWidth, out intHeight);

      SolidBrush objBrushForeColor = new SolidBrush(ForeColor);
      SolidBrush objBrushBackColor = new SolidBrush(BackColor);

      Point objPoint = new Point(0, 0);

      Bitmap objBitmap = new Bitmap(intWidth, intHeight);
      Graphics objGraphics = System.Drawing.Graphics.FromImage(objBitmap);

      objGraphics.FillRectangle(objBrushBackColor, 0, 0, intWidth, intHeight);
      objGraphics.DrawString(Text, Schrift, objBrushForeColor, objPoint);

      return objBitmap;
    }

    /// <summary>
    /// Übergibt die Größe eines Textes in Pixel
    /// </summary>
    /// <param name="Text">Text dessen Größe ermittelt werden soll (auch mehrzeilig mit \n)</param>
    /// <param name="Schrift">Font als Objet</param>
    /// <param name="Width">Ausgabe : Breite in Pixel</param>
    /// <param name="Height">Ausgabe : Höhe in Pixel</param>
    public static void TextSize(string Text, Font Schrift, out int Width, out int Height)
    {
      System.Drawing.Size objSize = TextRenderer.MeasureText(Text, Schrift);
      Width = objSize.Width;
      Height = objSize.Height;
    }

    // OPTIONALE ÜBERLADUNGEN:

    /// <summary>
    /// Wandelt einen String in ein Bitmap (Image) um.
    /// </summary>
    /// <param name="Text">Beliebiger Text (auch mehrzeilig mit \n)</param>
    /// <param name="FontName">Name des Font z.B. Arial</param>
    /// <param name="FontSize">Fontgröße</param>
    /// <param name="ForeColor">Schriftfarbe</param>
    /// <param name="BackColor">Hintergrundfarbe</param>
    /// <returns></returns>
    public static Image Text2Bitmap(string Text, string FontName, float FontSize, Color ForeColor, Color BackColor)
    {
      return Text2Bitmap(Text, new Font(FontName, FontSize), ForeColor, BackColor);
    }

    /// <summary>
    /// Wandelt einen String in ein Bitmap (Image) um. Schriftfarbe ist schwarz, Hintergrund ist weiss
    /// </summary>
    /// <param name="Text">Beliebiger Text (auch mehrzeilig mit \n)</param>
    /// <param name="FontName">Name des Font z.B. Arial</param>
    /// <param name="FontSize">Fontgröße</param>
    /// <returns></returns>
    public static Image Text2Bitmap(string Text, string FontName, float FontSize)
    {
      return Text2Bitmap(Text, new Font(FontName, FontSize), Color.Black, Color.White);
    }

  }
}
