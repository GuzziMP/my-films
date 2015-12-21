using MediaPortal.ExtensionMethods;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.WindowsAPICodePack.Shell;
using OSInfo;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Grabber.Util
{
  public class Picture
  {
    private static Picture.ExifOrientations orientation = Picture.ExifOrientations.Normal;

    public static string MediaUrl { get; private set; }

    public static Texture Load(string strPic, int iRotate, int iMaxWidth, int iMaxHeight, bool bRGB, bool bZoom, out int iWidth, out int iHeight)
    {
      return Picture.Load(strPic, iRotate, iMaxWidth, iMaxHeight, bRGB, bZoom, false, out iWidth, out iHeight);
    }

    public static Texture Load(string strPic, int iRotate, int iMaxWidth, int iMaxHeight, bool bRGB, bool bZoom, bool bOversized, out int iWidth, out int iHeight)
    {
      iWidth = 0;
      iHeight = 0;
      if (strPic == null)
        return (Texture)null;
      if (strPic == string.Empty)
        return (Texture)null;
      Texture texture = (Texture)null;
      Image image = (Image)null;
      try
      {
        try
        {
          using (FileStream fileStream = new FileStream(strPic, FileMode.Open, FileAccess.Read))
          {
            using (image = Image.FromStream((Stream)fileStream, true, false))
            {
              Log.Debug("Picture: Fast loaded texture {0}", (object)strPic);
              if (image == null)
                return (Texture)null;
              if (iRotate > 0)
              {
                switch (iRotate)
                {
                  case 1:
                    RotateFlipType rotateFlipType1 = RotateFlipType.Rotate90FlipNone;
                    image.RotateFlip(rotateFlipType1);
                    break;
                  case 2:
                    RotateFlipType rotateFlipType2 = RotateFlipType.Rotate180FlipNone;
                    image.RotateFlip(rotateFlipType2);
                    break;
                  case 3:
                    RotateFlipType rotateFlipType3 = RotateFlipType.Rotate270FlipNone;
                    image.RotateFlip(rotateFlipType3);
                    break;
                }
              }
              iWidth = image.Size.Width;
              iHeight = image.Size.Height;
              int num1 = iWidth;
              int num2 = iHeight;
              bool flag = false;
              float num3;
              if (bZoom)
              {
                flag = true;
                num1 = iMaxWidth;
                num2 = iMaxHeight;
                while (iWidth < iMaxWidth || iHeight < iMaxHeight)
                {
                  iWidth *= 2;
                  iHeight *= 2;
                }
                int overScanLeft = GUIGraphicsContext.OverScanLeft;
                int overScanTop = GUIGraphicsContext.OverScanTop;
                int overScanWidth = GUIGraphicsContext.OverScanWidth;
                int overScanHeight = GUIGraphicsContext.OverScanHeight;
                float pixelRatio = GUIGraphicsContext.PixelRatio;
                num3 = (float)iWidth / (float)iHeight / pixelRatio;
              }
              else
                num3 = (float)iWidth / (float)iHeight;
              if (iWidth > iMaxWidth)
              {
                flag = true;
                iWidth = iMaxWidth;
                iHeight = (int)((double)iWidth / (double)num3);
              }
              if (iHeight > iMaxHeight)
              {
                flag = true;
                iHeight = iMaxHeight;
                iWidth = (int)((double)num3 * (double)iHeight);
              }
              int width;
              int height;
              if (!bOversized)
              {
                width = iWidth;
                height = iHeight;
              }
              else
              {
                width = iWidth + 2;
                height = iHeight + 2;
                flag = true;
              }
              if (flag)
              {
                using (Bitmap theImage = new Bitmap(width, height))
                {
                  using (Graphics graphics = Graphics.FromImage((Image)theImage))
                  {
                    graphics.CompositingQuality = Thumbs.Compositing;
                    graphics.InterpolationMode = Thumbs.Interpolation;
                    graphics.SmoothingMode = Thumbs.Smoothing;
                    if (bOversized)
                    {
                      int x = 1;
                      int y = 1;
                      graphics.DrawImage(image, new System.Drawing.Rectangle(x, y, iWidth, iHeight));
                    }
                    else
                      graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, iWidth, iHeight));
                  }
                  texture = Picture.ConvertImageToTexture(theImage, out iWidth, out iHeight);
                }
              }
              else
                texture = Picture.ConvertImageToTexture((Bitmap)image, out iWidth, out iHeight);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Warn("Picture: exception loading {0}", (object)strPic);
        }
      }
      catch (ThreadAbortException ex)
      {
        Log.Debug("Picture: exception loading {0} err:{1}", new object[2]
        {
          (object) strPic,
          (object) ex.Message
        });
      }
      catch (Exception ex)
      {
        Log.Warn("Picture: exception loading {0} err:{1}", new object[2]
        {
          (object) strPic,
          (object) ex.Message
        });
      }
      finally
      {
        if (image != null)
          ObjectMethods.SafeDispose((object)image);
      }
      return texture;
    }

    public static Texture ConvertImageToTexture(Bitmap theImage, out int iWidth, out int iHeight)
    {
      iWidth = 0;
      iHeight = 0;
      if (theImage == null)
        return (Texture)null;
      try
      {
        Texture texture = (Texture)null;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          ImageInformation srcInformation = new ImageInformation();
          theImage.Save((Stream)memoryStream, ImageFormat.Bmp);
          memoryStream.Flush();
          memoryStream.Seek(0L, SeekOrigin.Begin);
          texture = TextureLoader.FromStream(GUIGraphicsContext.DX9Device, (Stream)memoryStream, 0, 0, 1, Usage.None, Format.X8R8G8B8, GUIGraphicsContext.GetTexturePoolType(), Filter.None, Filter.None, 0, ref srcInformation);
          memoryStream.Close();
          iWidth = srcInformation.Width;
          iHeight = srcInformation.Height;
        }
        return texture;
      }
      catch (Exception ex)
      {
        Log.Info("Picture.ConvertImageToTexture( {0}x{1} ) exception err:{2} stack:{3}", (object)iWidth, (object)iHeight, (object)ex.Message, (object)ex.StackTrace);
      }
      return (Texture)null;
    }

    public static void RenderImage(Texture texture, float x, float y, float nw, float nh, float iTextureWidth, float iTextureHeight, float iTextureLeft, float iTextureTop, bool bHiQuality)
    {
      if (texture == (Texture)null || texture.Disposed || (GUIGraphicsContext.DX9Device == (Device)null || GUIGraphicsContext.DX9Device.Disposed) || ((double)x < 0.0 || (double)y < 0.0 || ((double)nw < 0.0 || (double)nh < 0.0)) || ((double)iTextureWidth < 0.0 || (double)iTextureHeight < 0.0 || ((double)iTextureLeft < 0.0 || (double)iTextureTop < 0.0)))
        return;
      VertexBuffer streamData = (VertexBuffer)null;
      try
      {
        streamData = new VertexBuffer(typeof(CustomVertex.TransformedColoredTextured), 4, GUIGraphicsContext.DX9Device, Usage.None, VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed, GUIGraphicsContext.GetTexturePoolType());
        SurfaceDescription levelDescription = texture.GetLevelDescription(0);
        float num1 = iTextureLeft / (float)levelDescription.Width;
        float num2 = iTextureTop / (float)levelDescription.Height;
        float num3 = iTextureWidth / (float)levelDescription.Width;
        float num4 = iTextureHeight / (float)levelDescription.Height;
        long num5 = (long)uint.MaxValue;
        if ((double)num1 < 0.0 || (double)num1 > 1.0 || ((double)num2 < 0.0 || (double)num2 > 1.0) || ((double)num3 < 0.0 || (double)num3 > 1.0 || ((double)num4 < 0.0 || (double)num4 > 1.0)) || ((double)iTextureWidth + (double)iTextureLeft < 0.0 || (double)iTextureWidth + (double)iTextureLeft > (double)levelDescription.Width || ((double)iTextureHeight + (double)iTextureTop < 0.0 || (double)iTextureHeight + (double)iTextureTop > (double)levelDescription.Height)))
          return;
        if ((double)x < 0.0)
          x = 0.0f;
        if ((double)x > (double)GUIGraphicsContext.Width)
          x = (float)GUIGraphicsContext.Width;
        if ((double)y < 0.0)
          y = 0.0f;
        if ((double)y > (double)GUIGraphicsContext.Height)
          y = (float)GUIGraphicsContext.Height;
        if ((double)nw < 0.0)
          nw = 0.0f;
        if ((double)nh < 0.0)
          nh = 0.0f;
        if ((double)x + (double)nw > (double)GUIGraphicsContext.Width)
          nw = (float)GUIGraphicsContext.Width - x;
        if ((double)y + (double)nh > (double)GUIGraphicsContext.Height)
          nh = (float)GUIGraphicsContext.Height - y;
        CustomVertex.TransformedColoredTextured[] transformedColoredTexturedArray = (CustomVertex.TransformedColoredTextured[])streamData.Lock(0, LockFlags.None);
        transformedColoredTexturedArray[0].X = x - 0.5f;
        transformedColoredTexturedArray[0].Y = (float)((double)y + (double)nh - 0.5);
        transformedColoredTexturedArray[0].Z = 0.0f;
        transformedColoredTexturedArray[0].Rhw = 1f;
        transformedColoredTexturedArray[0].Color = (int)num5;
        transformedColoredTexturedArray[0].Tu = num1;
        transformedColoredTexturedArray[0].Tv = num2 + num4;
        transformedColoredTexturedArray[1].X = x - 0.5f;
        transformedColoredTexturedArray[1].Y = y - 0.5f;
        transformedColoredTexturedArray[1].Z = 0.0f;
        transformedColoredTexturedArray[1].Rhw = 1f;
        transformedColoredTexturedArray[1].Color = (int)num5;
        transformedColoredTexturedArray[1].Tu = num1;
        transformedColoredTexturedArray[1].Tv = num2;
        transformedColoredTexturedArray[2].X = (float)((double)x + (double)nw - 0.5);
        transformedColoredTexturedArray[2].Y = (float)((double)y + (double)nh - 0.5);
        transformedColoredTexturedArray[2].Z = 0.0f;
        transformedColoredTexturedArray[2].Rhw = 1f;
        transformedColoredTexturedArray[2].Color = (int)num5;
        transformedColoredTexturedArray[2].Tu = num1 + num3;
        transformedColoredTexturedArray[2].Tv = num2 + num4;
        transformedColoredTexturedArray[3].X = (float)((double)x + (double)nw - 0.5);
        transformedColoredTexturedArray[3].Y = y - 0.5f;
        transformedColoredTexturedArray[3].Z = 0.0f;
        transformedColoredTexturedArray[3].Rhw = 1f;
        transformedColoredTexturedArray[3].Color = (int)num5;
        transformedColoredTexturedArray[3].Tu = num1 + num3;
        transformedColoredTexturedArray[3].Tv = num2;
        streamData.Unlock();
        GUIGraphicsContext.DX9Device.SetTexture(0, (BaseTexture)texture);
        int num6 = GUIGraphicsContext.DX9Device.DeviceCaps.MaxAnisotropy;
        float num7 = 0.0f;
        DXNative.FontEngineSetSamplerState(0U, 6, 2U);
        DXNative.FontEngineSetSamplerState(0U, 5, 2U);
        DXNative.FontEngineSetSamplerState(0U, 7, 2U);
        DXNative.FontEngineSetSamplerState(0U, 10, (uint)num6);
        DXNative.FontEngineSetSamplerState(0U, 8, (uint)num7);
        DXNative.FontEngineSetSamplerState(1U, 6, 2U);
        DXNative.FontEngineSetSamplerState(1U, 5, 2U);
        DXNative.FontEngineSetSamplerState(1U, 7, 2U);
        DXNative.FontEngineSetSamplerState(1U, 10, (uint)num6);
        DXNative.FontEngineSetSamplerState(1U, 8, (uint)num7);
        GUIGraphicsContext.DX9Device.SetStreamSource(0, streamData, 0);
        GUIGraphicsContext.DX9Device.VertexFormat = VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed;
        GUIGraphicsContext.DX9Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        GUIGraphicsContext.DX9Device.SetTexture(0, (BaseTexture)null);
      }
      finally
      {
        if ((Resource)streamData != (Resource)null)
          ObjectMethods.SafeDispose((object)streamData);
      }
    }

    public static void RenderImage(Texture texture, int x, int y, int nw, int nh, int iTextureWidth, int iTextureHeight, int iTextureLeft, int iTextureTop, bool bHiQuality)
    {
      if (texture == (Texture)null || texture.Disposed || (GUIGraphicsContext.DX9Device == (Device)null || GUIGraphicsContext.DX9Device.Disposed) || (x < 0 || y < 0 || (nw < 0 || nh < 0)) || (iTextureWidth < 0 || iTextureHeight < 0 || (iTextureLeft < 0 || iTextureTop < 0)))
        return;
      VertexBuffer streamData = (VertexBuffer)null;
      try
      {
        streamData = new VertexBuffer(typeof(CustomVertex.TransformedColoredTextured), 4, GUIGraphicsContext.DX9Device, Usage.None, VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed, GUIGraphicsContext.GetTexturePoolType());
        SurfaceDescription levelDescription = texture.GetLevelDescription(0);
        float num1 = (float)iTextureLeft / (float)levelDescription.Width;
        float num2 = (float)iTextureTop / (float)levelDescription.Height;
        float num3 = (float)iTextureWidth / (float)levelDescription.Width;
        float num4 = (float)iTextureHeight / (float)levelDescription.Height;
        long num5 = (long)uint.MaxValue;
        if ((double)num1 < 0.0 || (double)num1 > 1.0 || ((double)num2 < 0.0 || (double)num2 > 1.0) || ((double)num3 < 0.0 || (double)num3 > 1.0 || ((double)num4 < 0.0 || (double)num4 > 1.0)) || ((double)num3 + (double)num1 < 0.0 || (double)num3 + (double)num1 > 1.0 || ((double)num4 + (double)num2 < 0.0 || (double)num4 + (double)num2 > 1.0)))
          return;
        if (x < 0)
          x = 0;
        if (x > GUIGraphicsContext.Width)
          x = GUIGraphicsContext.Width;
        if (y < 0)
          y = 0;
        if (y > GUIGraphicsContext.Height)
          y = GUIGraphicsContext.Height;
        if (nw < 0)
          nw = 0;
        if (nh < 0)
          nh = 0;
        if (x + nw > GUIGraphicsContext.Width)
          nw = GUIGraphicsContext.Width - x;
        if (y + nh > GUIGraphicsContext.Height)
          nh = GUIGraphicsContext.Height - y;
        CustomVertex.TransformedColoredTextured[] transformedColoredTexturedArray = (CustomVertex.TransformedColoredTextured[])streamData.Lock(0, LockFlags.None);
        transformedColoredTexturedArray[0].X = (float)x - 0.5f;
        transformedColoredTexturedArray[0].Y = (float)(y + nh) - 0.5f;
        transformedColoredTexturedArray[0].Z = 0.0f;
        transformedColoredTexturedArray[0].Rhw = 1f;
        transformedColoredTexturedArray[0].Color = (int)num5;
        transformedColoredTexturedArray[0].Tu = num1;
        transformedColoredTexturedArray[0].Tv = num2 + num4;
        transformedColoredTexturedArray[1].X = (float)x - 0.5f;
        transformedColoredTexturedArray[1].Y = (float)y - 0.5f;
        transformedColoredTexturedArray[1].Z = 0.0f;
        transformedColoredTexturedArray[1].Rhw = 1f;
        transformedColoredTexturedArray[1].Color = (int)num5;
        transformedColoredTexturedArray[1].Tu = num1;
        transformedColoredTexturedArray[1].Tv = num2;
        transformedColoredTexturedArray[2].X = (float)(x + nw) - 0.5f;
        transformedColoredTexturedArray[2].Y = (float)(y + nh) - 0.5f;
        transformedColoredTexturedArray[2].Z = 0.0f;
        transformedColoredTexturedArray[2].Rhw = 1f;
        transformedColoredTexturedArray[2].Color = (int)num5;
        transformedColoredTexturedArray[2].Tu = num1 + num3;
        transformedColoredTexturedArray[2].Tv = num2 + num4;
        transformedColoredTexturedArray[3].X = (float)(x + nw) - 0.5f;
        transformedColoredTexturedArray[3].Y = (float)y - 0.5f;
        transformedColoredTexturedArray[3].Z = 0.0f;
        transformedColoredTexturedArray[3].Rhw = 1f;
        transformedColoredTexturedArray[3].Color = (int)num5;
        transformedColoredTexturedArray[3].Tu = num1 + num3;
        transformedColoredTexturedArray[3].Tv = num2;
        streamData.Unlock();
        GUIGraphicsContext.DX9Device.SetTexture(0, (BaseTexture)texture);
        int num6 = GUIGraphicsContext.DX9Device.DeviceCaps.MaxAnisotropy;
        float num7 = 0.0f;
        DXNative.FontEngineSetSamplerState(0U, 6, 2U);
        DXNative.FontEngineSetSamplerState(0U, 5, 2U);
        DXNative.FontEngineSetSamplerState(0U, 7, 2U);
        DXNative.FontEngineSetSamplerState(0U, 10, (uint)num6);
        DXNative.FontEngineSetSamplerState(0U, 8, (uint)num7);
        DXNative.FontEngineSetSamplerState(1U, 6, 2U);
        DXNative.FontEngineSetSamplerState(1U, 5, 2U);
        DXNative.FontEngineSetSamplerState(1U, 7, 2U);
        DXNative.FontEngineSetSamplerState(1U, 10, (uint)num6);
        DXNative.FontEngineSetSamplerState(1U, 8, (uint)num7);
        GUIGraphicsContext.DX9Device.SetStreamSource(0, streamData, 0);
        GUIGraphicsContext.DX9Device.VertexFormat = VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed;
        GUIGraphicsContext.DX9Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        GUIGraphicsContext.DX9Device.SetTexture(0, (BaseTexture)null);
      }
      finally
      {
        if ((Resource)streamData != (Resource)null)
          ObjectMethods.SafeDispose((object)streamData);
      }
    }

    public static void RenderImage(Texture texture, float x, float y, float nw, float nh, float iTextureWidth, float iTextureHeight, float iTextureLeft, float iTextureTop, long lColorDiffuse)
    {
      if (texture == (Texture)null || texture.Disposed || (GUIGraphicsContext.DX9Device == (Device)null || GUIGraphicsContext.DX9Device.Disposed) || ((double)x < 0.0 || (double)y < 0.0 || ((double)nw < 0.0 || (double)nh < 0.0)) || ((double)iTextureWidth < 0.0 || (double)iTextureHeight < 0.0 || ((double)iTextureLeft < 0.0 || (double)iTextureTop < 0.0)))
        return;
      VertexBuffer streamData = (VertexBuffer)null;
      try
      {
        streamData = new VertexBuffer(typeof(CustomVertex.TransformedColoredTextured), 4, GUIGraphicsContext.DX9Device, Usage.None, VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed, GUIGraphicsContext.GetTexturePoolType());
        SurfaceDescription levelDescription = texture.GetLevelDescription(0);
        float num1 = iTextureLeft / (float)levelDescription.Width;
        float num2 = iTextureTop / (float)levelDescription.Height;
        float num3 = iTextureWidth / (float)levelDescription.Width;
        float num4 = iTextureHeight / (float)levelDescription.Height;
        if ((double)num1 < 0.0 || (double)num1 > 1.0 || ((double)num2 < 0.0 || (double)num2 > 1.0) || ((double)num3 < 0.0 || (double)num3 > 1.0 || ((double)num4 < 0.0 || (double)num4 > 1.0)) || ((double)num3 + (double)num1 < 0.0 || (double)num3 + (double)num1 > 1.0 || ((double)num4 + (double)num2 < 0.0 || (double)num4 + (double)num2 > 1.0)))
          return;
        if ((double)x < 0.0)
          x = 0.0f;
        if ((double)x > (double)GUIGraphicsContext.Width)
          x = (float)GUIGraphicsContext.Width;
        if ((double)y < 0.0)
          y = 0.0f;
        if ((double)y > (double)GUIGraphicsContext.Height)
          y = (float)GUIGraphicsContext.Height;
        if ((double)nw < 0.0)
          nw = 0.0f;
        if ((double)nh < 0.0)
          nh = 0.0f;
        if ((double)x + (double)nw > (double)GUIGraphicsContext.Width)
          nw = (float)GUIGraphicsContext.Width - x;
        if ((double)y + (double)nh > (double)GUIGraphicsContext.Height)
          nh = (float)GUIGraphicsContext.Height - y;
        CustomVertex.TransformedColoredTextured[] transformedColoredTexturedArray = (CustomVertex.TransformedColoredTextured[])streamData.Lock(0, LockFlags.None);
        transformedColoredTexturedArray[0].X = x - 0.5f;
        transformedColoredTexturedArray[0].Y = (float)((double)y + (double)nh - 0.5);
        transformedColoredTexturedArray[0].Z = 0.0f;
        transformedColoredTexturedArray[0].Rhw = 1f;
        transformedColoredTexturedArray[0].Color = (int)lColorDiffuse;
        transformedColoredTexturedArray[0].Tu = num1;
        transformedColoredTexturedArray[0].Tv = num2 + num4;
        transformedColoredTexturedArray[1].X = x - 0.5f;
        transformedColoredTexturedArray[1].Y = y - 0.5f;
        transformedColoredTexturedArray[1].Z = 0.0f;
        transformedColoredTexturedArray[1].Rhw = 1f;
        transformedColoredTexturedArray[1].Color = (int)lColorDiffuse;
        transformedColoredTexturedArray[1].Tu = num1;
        transformedColoredTexturedArray[1].Tv = num2;
        transformedColoredTexturedArray[2].X = (float)((double)x + (double)nw - 0.5);
        transformedColoredTexturedArray[2].Y = (float)((double)y + (double)nh - 0.5);
        transformedColoredTexturedArray[2].Z = 0.0f;
        transformedColoredTexturedArray[2].Rhw = 1f;
        transformedColoredTexturedArray[2].Color = (int)lColorDiffuse;
        transformedColoredTexturedArray[2].Tu = num1 + num3;
        transformedColoredTexturedArray[2].Tv = num2 + num4;
        transformedColoredTexturedArray[3].X = (float)((double)x + (double)nw - 0.5);
        transformedColoredTexturedArray[3].Y = y - 0.5f;
        transformedColoredTexturedArray[3].Z = 0.0f;
        transformedColoredTexturedArray[3].Rhw = 1f;
        transformedColoredTexturedArray[3].Color = (int)lColorDiffuse;
        transformedColoredTexturedArray[3].Tu = num1 + num3;
        transformedColoredTexturedArray[3].Tv = num2;
        streamData.Unlock();
        GUIGraphicsContext.DX9Device.SetTexture(0, (BaseTexture)texture);
        DXNative.FontEngineSetTextureStageState(0U, 1, 4U);
        DXNative.FontEngineSetTextureStageState(0U, 2, 2U);
        DXNative.FontEngineSetTextureStageState(0U, 3, 0U);
        DXNative.FontEngineSetTextureStageState(0U, 4, 4U);
        DXNative.FontEngineSetTextureStageState(0U, 5, 2U);
        DXNative.FontEngineSetTextureStageState(0U, 6, 0U);
        DXNative.FontEngineSetTextureStageState(0U, 1, 4U);
        DXNative.FontEngineSetTextureStageState(0U, 2, 2U);
        DXNative.FontEngineSetTextureStageState(0U, 3, 0U);
        DXNative.FontEngineSetTextureStageState(0U, 4, 4U);
        DXNative.FontEngineSetTextureStageState(0U, 5, 2U);
        DXNative.FontEngineSetTextureStageState(0U, 6, 0U);
        DXNative.FontEngineSetTextureStageState(1U, 1, 1U);
        DXNative.FontEngineSetTextureStageState(1U, 4, 1U);
        int num5 = GUIGraphicsContext.DX9Device.DeviceCaps.MaxAnisotropy;
        float num6 = 0.0f;
        DXNative.FontEngineSetSamplerState(0U, 6, 2U);
        DXNative.FontEngineSetSamplerState(0U, 5, 2U);
        DXNative.FontEngineSetSamplerState(0U, 7, 2U);
        DXNative.FontEngineSetSamplerState(0U, 10, (uint)num5);
        DXNative.FontEngineSetSamplerState(0U, 8, (uint)num6);
        DXNative.FontEngineSetSamplerState(1U, 6, 2U);
        DXNative.FontEngineSetSamplerState(1U, 5, 2U);
        DXNative.FontEngineSetSamplerState(1U, 7, 2U);
        DXNative.FontEngineSetSamplerState(1U, 10, (uint)num5);
        DXNative.FontEngineSetSamplerState(1U, 8, (uint)num6);
        GUIGraphicsContext.DX9Device.SetStreamSource(0, streamData, 0);
        GUIGraphicsContext.DX9Device.VertexFormat = VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed;
        GUIGraphicsContext.DX9Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        GUIGraphicsContext.DX9Device.SetTexture(0, (BaseTexture)null);
      }
      finally
      {
        if ((Resource)streamData != (Resource)null)
          ObjectMethods.SafeDispose((object)streamData);
      }
    }

    public static bool ThumbnailCallback()
    {
      return false;
    }

    public static bool CreateThumbnail(string thumbnailImageSource, string thumbnailImageDest, int aThumbWidth, int aThumbHeight, int iRotate, bool aFastMode)
    {
      return Picture.CreateThumbnail(thumbnailImageSource, thumbnailImageDest, aThumbWidth, aThumbHeight, iRotate, aFastMode, true, true);
    }

    private static bool BitmapFromSource(BitmapSource bitmapsource, string thumbnailImageDest)
    {
      if (bitmapsource == null)
        return false;
      try
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          BitmapEncoder bitmapEncoder = (BitmapEncoder)new BmpBitmapEncoder();
          bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapsource));
          bitmapEncoder.Save((Stream)memoryStream);
          using (memoryStream)
          {
            Bitmap bitmap = new Bitmap(Image.FromStream((Stream)memoryStream));
            if (thumbnailImageDest != null && !File.Exists(thumbnailImageDest))
            {
              bitmap.Save(thumbnailImageDest, Thumbs.ThumbCodecInfo, Thumbs.ThumbEncoderParams);
              File.SetAttributes(thumbnailImageDest, File.GetAttributes(thumbnailImageDest) | FileAttributes.Hidden);
            }
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool CreateThumbnailVideo(string aInputFilename, string aThumbTargetPath, int iMaxWidth, int iMaxHeight, int iRotate, bool aFastMode)
    {
      bool flag = false;
      if (string.IsNullOrEmpty(aInputFilename) || string.IsNullOrEmpty(aThumbTargetPath) || (iMaxHeight <= 0 || iMaxHeight <= 0))
        return false;
      Image aDrawingImage = (Image)null;
      try
      {
        try
        {
          using (FileStream fileStream = new FileStream(aInputFilename, FileMode.Open, FileAccess.Read))
          {
            using (aDrawingImage = Image.FromStream((Stream)fileStream, true, false))
              flag = Picture.CreateThumbnail(aDrawingImage, aThumbTargetPath, iMaxWidth, iMaxHeight, iRotate, aFastMode);
          }
        }
        catch (FileNotFoundException ex)
        {
          flag = false;
        }
      }
      catch (Exception ex1)
      {
        Log.Warn("Picture: Fast loading of thumbnail {0} failed - trying safe fallback now", (object)aInputFilename);
        try
        {
          try
          {
            using (FileStream fileStream = new FileStream(aInputFilename, FileMode.Open, FileAccess.Read))
            {
              using (aDrawingImage = Image.FromStream((Stream)fileStream, true, false))
                flag = Picture.CreateThumbnail(aDrawingImage, aThumbTargetPath, iMaxWidth, iMaxHeight, iRotate, aFastMode);
            }
          }
          catch (Exception ex2)
          {
          }
        }
        catch (FileNotFoundException ex2)
        {
          flag = false;
        }
        catch (OutOfMemoryException ex2)
        {
          Log.Warn("Picture: Creating thumbnail failed - image format is not supported of {0}", (object)aInputFilename);
          flag = false;
        }
        catch (Exception ex2)
        {
          Log.Error("Picture: CreateThumbnail exception err:{0} stack:{1}", new object[2]
          {
            (object) ex2.Message,
            (object) ex2.StackTrace
          });
          flag = false;
        }
      }
      finally
      {
        if (aDrawingImage != null)
          ObjectMethods.SafeDispose((object)aDrawingImage);
      }
      return flag;
    }

    public static bool CreateThumbnail(string thumbnailImageSource, string thumbnailImageDest, int aThumbWidth, int aThumbHeight, int iRotate, bool aFastMode, bool autocreateLargeThumbs, bool fallBack)
    {
      return Picture.ReCreateThumbnail(thumbnailImageSource, thumbnailImageDest, aThumbWidth, aThumbHeight, iRotate, aFastMode, autocreateLargeThumbs, fallBack, false);
    }

    public static bool ReCreateThumbnail(string thumbnailImageSource, string thumbnailImageDest, int aThumbWidth, int aThumbHeight, int iRotate, bool aFastMode, bool autocreateLargeThumbs, bool fallBack, bool needOverride)
    {
      if (!needOverride && File.Exists(thumbnailImageDest) || (string.IsNullOrEmpty(thumbnailImageSource) || string.IsNullOrEmpty(thumbnailImageDest)) || (aThumbHeight <= 0 || aThumbWidth <= 0))
        return false;
      BitmapSource bitmapSource = (BitmapSource)null;
      Bitmap bitmap1 = (Bitmap)null;
      Bitmap bitmap2 = (Bitmap)null;
      BitmapFrame bitmapFrame = (BitmapFrame)null;
      Image aDrawingImage = (Image)null;
      TransformedBitmap transformedBitmap = (TransformedBitmap)null;
      TransformGroup transformGroup = (TransformGroup)null;
      bool flag = false;
      int num1 = (int)Thumbs.Quality;
      int num2 = aThumbWidth;
      Picture.MediaUrl = thumbnailImageSource;
      try
      {
        bitmapFrame = !fallBack ? BitmapFrame.Create(new Uri(Picture.MediaUrl), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None) : BitmapFrame.Create(new Uri(Picture.MediaUrl), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        if (bitmapFrame.Thumbnail == null)
        {
          using (ShellObject shellObject = ShellObject.FromParsingName(thumbnailImageSource))
          {
            shellObject.get_Thumbnail().set_RetrievalOption((ShellThumbnailRetrievalOption)0);
            shellObject.get_Thumbnail().set_FormatOption((ShellThumbnailFormatOption)8);
            switch (num1)
            {
              case 0:
                bitmap1 = shellObject.get_Thumbnail().get_MediumBitmap();
                break;
              case 1:
                bitmap1 = shellObject.get_Thumbnail().get_LargeBitmap();
                break;
              case 2:
                bitmap1 = shellObject.get_Thumbnail().get_LargeBitmap();
                break;
              case 3:
                bitmap1 = shellObject.get_Thumbnail().get_ExtraLargeBitmap();
                break;
              case 4:
                bitmap1 = shellObject.get_Thumbnail().get_ExtraLargeBitmap();
                break;
            }
            if (!OSInfo.Win8OrLater())
            {
              switch (iRotate)
              {
                case 1:
                  bitmap1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                  break;
                case 2:
                  bitmap1.RotateFlip(RotateFlipType.Rotate180FlipNone);
                  break;
                case 3:
                  bitmap1.RotateFlip(RotateFlipType.Rotate270FlipNone);
                  break;
              }
            }
            if (bitmap1 != null && !autocreateLargeThumbs)
            {
              int width = aThumbWidth;
              int height = aThumbHeight;
              double num3 = (double)bitmap1.Width / (double)bitmap1.Height;
              if (bitmap1.Width > bitmap1.Height)
                height = (int)Math.Floor((double)width / num3);
              else
                width = (int)Math.Floor(num3 * (double)height);
              try
              {
                GrabUtil.FileDelete(thumbnailImageDest);
              }
              catch (Exception ex)
              {
                Log.Error("Picture: Error deleting old thumbnail - {0}", (object)ex.Message);
              }
              bitmap2 = new Bitmap((Image)bitmap1, width, height);
              bitmap2.Save(thumbnailImageDest, Thumbs.ThumbCodecInfo, Thumbs.ThumbEncoderParams);
              File.SetAttributes(thumbnailImageDest, File.GetAttributes(thumbnailImageDest) | FileAttributes.Hidden);
              flag = true;
            }
            else
            {
              int width = aThumbWidth;
              int height = aThumbHeight;
              double num3 = (double)bitmap1.Width / (double)bitmap1.Height;
              if (bitmap1.Width > bitmap1.Height)
                height = (int)Math.Floor((double)width / num3);
              else
                width = (int)Math.Floor(num3 * (double)height);
              try
              {
                GrabUtil.FileDelete(thumbnailImageDest);
              }
              catch (Exception ex)
              {
                Log.Error("Picture: Error deleting old thumbnail - {0}", (object)ex.Message);
              }
              bitmap2 = new Bitmap((Image)bitmap1, width, height);
              bitmap2.Save(thumbnailImageDest, Thumbs.ThumbCodecInfo, Thumbs.ThumbEncoderParams);
              File.SetAttributes(thumbnailImageDest, File.GetAttributes(thumbnailImageDest) | FileAttributes.Hidden);
              flag = true;
            }
          }
        }
        else
        {
          BitmapMetadata meta = bitmapFrame.Metadata as BitmapMetadata;
          bitmapSource = bitmapFrame.Thumbnail;
          if (autocreateLargeThumbs)
          {
            if (bitmapSource != null)
            {
              transformedBitmap = new TransformedBitmap();
              transformedBitmap.BeginInit();
              transformedBitmap.Source = (BitmapSource)bitmapFrame;
              int pixelHeight = bitmapFrame.PixelHeight;
              int pixelWidth = bitmapFrame.PixelWidth;
              int num3 = bitmapFrame.PixelHeight * num2 / pixelWidth;
              double scaleX = (double)num2 / (double)pixelWidth;
              double scaleY = (double)num3 / (double)pixelHeight;
              transformGroup = new TransformGroup();
              transformGroup.Children.Add((Transform)new ScaleTransform(scaleX, scaleY));
              transformedBitmap.Transform = (Transform)transformGroup;
              transformedBitmap.EndInit();
              bitmapSource = (BitmapSource)transformedBitmap;
              bitmapSource = Picture.MetaOrientation(meta, bitmapSource);
              flag = Picture.BitmapFromSource(bitmapSource, thumbnailImageDest);
            }
          }
          else if (bitmapSource != null)
          {
            bitmapSource = Picture.MetaOrientation(meta, bitmapSource);
            flag = Picture.BitmapFromSource(bitmapSource, thumbnailImageDest);
          }
        }
      }
      catch (Exception ex1)
      {
        try
        {
          try
          {
            using (FileStream fileStream = new FileStream(thumbnailImageSource, FileMode.Open, FileAccess.Read))
            {
              using (aDrawingImage = Image.FromStream((Stream)fileStream, true, false))
                flag = Picture.CreateThumbnail(aDrawingImage, thumbnailImageDest, aThumbWidth, aThumbHeight, iRotate, aFastMode);
            }
          }
          catch (FileNotFoundException ex2)
          {
            flag = false;
          }
        }
        catch (Exception ex2)
        {
          Log.Warn("Picture: Fast loading of thumbnail {0} failed - trying safe fallback now", (object)thumbnailImageDest);
          try
          {
            try
            {
              using (FileStream fileStream = new FileStream(thumbnailImageDest, FileMode.Open, FileAccess.Read))
              {
                using (aDrawingImage = Image.FromStream((Stream)fileStream, true, false))
                  flag = Picture.CreateThumbnail(aDrawingImage, thumbnailImageDest, aThumbWidth, aThumbHeight, iRotate, aFastMode);
              }
            }
            catch (Exception ex3)
            {
            }
          }
          catch (FileNotFoundException ex3)
          {
            flag = false;
          }
          catch (OutOfMemoryException ex3)
          {
            Log.Warn("Picture: Creating thumbnail failed - image format is not supported of {0}", (object)thumbnailImageSource);
            flag = false;
          }
          catch (Exception ex3)
          {
            Log.Info("Pictures: No thumbnail created for -- {0}", (object)thumbnailImageSource);
            flag = false;
          }
        }
      }
      finally
      {
        if (bitmap1 != null)
          ObjectMethods.SafeDispose((object)bitmap1);
        if (bitmapSource != null)
          ObjectMethods.SafeDispose((object)bitmapSource);
        if (transformedBitmap != null)
          ObjectMethods.SafeDispose((object)transformedBitmap);
        if (transformGroup != null)
          ObjectMethods.SafeDispose((object)transformGroup);
        if (bitmap2 != null)
          ObjectMethods.SafeDispose((object)bitmap2);
        if (Picture.MediaUrl != null)
          ObjectMethods.SafeDispose((object)Picture.MediaUrl);
        if (bitmapFrame != null)
          ObjectMethods.SafeDispose((object)bitmapFrame);
        if (aDrawingImage != null)
          ObjectMethods.SafeDispose((object)aDrawingImage);
      }
      return flag;
    }

    public static bool CreateThumbnail(Image aDrawingImage, string aThumbTargetPath, int aThumbWidth, int aThumbHeight, int aRotation, bool aFastMode)
    {
      bool flag = false;
      if (string.IsNullOrEmpty(aThumbTargetPath) || aThumbHeight <= 0 || aThumbHeight <= 0)
        return false;
      Bitmap bitmap = (Bitmap)null;
      Image myImage = (Image)null;
      try
      {
        switch (aRotation)
        {
          case 1:
            aDrawingImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            break;
          case 2:
            aDrawingImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            break;
          case 3:
            aDrawingImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            break;
        }
        int num1 = aThumbWidth;
        int num2 = aThumbHeight;
        float num3 = (float)aDrawingImage.Width / (float)aDrawingImage.Height;
        if (aDrawingImage.Width > aDrawingImage.Height)
          num2 = (int)Math.Floor((double)num1 / (double)num3);
        else
          num1 = (int)Math.Floor((double)num3 * (double)num2);
        try
        {
          GrabUtil.FileDelete(aThumbTargetPath);
        }
        catch (Exception ex)
        {
          Log.Error("Picture: Error deleting old thumbnail - {0}", (object)ex.Message);
        }
        if (aFastMode)
        {
          Image.GetThumbnailImageAbort callback = new Image.GetThumbnailImageAbort(Picture.ThumbnailCallback);
          bitmap = new Bitmap(aDrawingImage, num1, num2);
          myImage = bitmap.GetThumbnailImage(num1, num2, callback, IntPtr.Zero);
        }
        else
        {
          System.Drawing.Imaging.PixelFormat format = aDrawingImage.PixelFormat;
          switch (format)
          {
            case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
            case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
            case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
            case System.Drawing.Imaging.PixelFormat.Undefined:
            case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
            case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
              format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
              break;
          }
          bitmap = new Bitmap(num1, num2, format);
          using (Graphics graphics = Graphics.FromImage((Image)bitmap))
          {
            graphics.CompositingQuality = Thumbs.Compositing;
            graphics.InterpolationMode = Thumbs.Interpolation;
            graphics.SmoothingMode = Thumbs.Smoothing;
            graphics.DrawImage(aDrawingImage, new System.Drawing.Rectangle(0, 0, num1, num2));
            myImage = (Image)bitmap;
          }
        }
        if (g_Player.Playing)
          Thread.Sleep(30);
        flag = Picture.SaveThumbnail(aThumbTargetPath, myImage);
      }
      catch (Exception ex)
      {
        flag = false;
      }
      finally
      {
        if (myImage != null)
          ObjectMethods.SafeDispose((object)myImage);
        if (bitmap != null)
          ObjectMethods.SafeDispose((object)bitmap);
      }
      return flag;
    }

    private static BitmapSource MetaOrientation(BitmapMetadata meta, BitmapSource ret)
    {
      double angle = 0.0;
      if (meta != null && ret != null)
      {
        if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
          Picture.orientation = (Picture.ExifOrientations)Enum.Parse(typeof(Picture.ExifOrientations), meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
        switch (Picture.orientation)
        {
          case Picture.ExifOrientations.Rotate180:
            angle = 180.0;
            break;
          case Picture.ExifOrientations.Rotate270:
            angle = 90.0;
            break;
          case Picture.ExifOrientations.Rotate90:
            angle = -90.0;
            break;
        }
        if (angle != 0.0)
        {
          ret = (BitmapSource)new TransformedBitmap(ret.Clone(), (Transform)new RotateTransform(angle));
          ret.Freeze();
        }
      }
      return ret;
    }

    public static bool SaveThumbnail(string aThumbTargetPath, Image myImage)
    {
      try
      {
        using (FileStream fileStream = new FileStream(aThumbTargetPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
          using (Bitmap bitmap = new Bitmap(myImage))
            bitmap.Save((Stream)fileStream, Thumbs.ThumbCodecInfo, Thumbs.ThumbEncoderParams);
          fileStream.Flush();
        }
        File.SetAttributes(aThumbTargetPath, File.GetAttributes(aThumbTargetPath) | FileAttributes.Hidden);
        if (g_Player.Playing)
          Thread.Sleep(100);
        else
          Thread.Sleep(1);
        return true;
      }
      catch (Exception ex)
      {
        Log.Error("Picture: Error saving new thumbnail {0} - {1}", new object[2]
        {
          (object) aThumbTargetPath,
          (object) ex.Message
        });
        return false;
      }
    }

    public static void DrawLine(int x1, int y1, int x2, int y2, long color)
    {
      Vector2[] vertexList = new Vector2[2];
      vertexList[0].X = (float)x1;
      vertexList[0].Y = (float)y1;
      vertexList[1].X = (float)x2;
      vertexList[1].Y = (float)y2;
      using (Line line = new Line(GUIGraphicsContext.DX9Device))
      {
        line.Begin();
        line.Draw(vertexList, (int)color);
        line.End();
      }
    }

    public static void DrawRectangle(System.Drawing.Rectangle rect, long color, bool fill)
    {
      if (fill)
      {
        System.Drawing.Rectangle[] regions = new System.Drawing.Rectangle[1]
        {
          rect
        };
        GUIGraphicsContext.DX9Device.Clear(ClearFlags.Target, (int)color, 1f, 0, regions);
      }
      else
      {
        Vector2[] vertexList = new Vector2[2];
        vertexList[0].X = (float)rect.Left;
        vertexList[0].Y = (float)rect.Top;
        vertexList[1].X = (float)(rect.Left + rect.Width);
        vertexList[1].Y = (float)rect.Top;
        using (Line line = new Line(GUIGraphicsContext.DX9Device))
        {
          line.Begin();
          line.Draw(vertexList, (int)color);
          vertexList[0].X = (float)(rect.Left + rect.Width);
          vertexList[0].Y = (float)rect.Top;
          vertexList[1].X = (float)(rect.Left + rect.Width);
          vertexList[1].Y = (float)(rect.Top + rect.Height);
          line.Draw(vertexList, (int)color);
          vertexList[0].X = (float)(rect.Left + rect.Width);
          vertexList[0].Y = (float)(rect.Top + rect.Width);
          vertexList[1].X = (float)rect.Left;
          vertexList[1].Y = (float)(rect.Top + rect.Height);
          line.Draw(vertexList, (int)color);
          vertexList[0].X = (float)rect.Left;
          vertexList[0].Y = (float)(rect.Top + rect.Height);
          vertexList[1].X = (float)rect.Left;
          vertexList[1].Y = (float)rect.Top;
          line.Draw(vertexList, (int)color);
          line.End();
        }
      }
    }

    public static int GetRotateByExif(string imageFile)
    {
      try
      {
        using (FileStream fileStream = new FileStream(imageFile, FileMode.Open, FileAccess.Read))
        {
          using (Image image = Image.FromStream((Stream)fileStream, true, false))
            return Picture.GetRotateByExif(image);
        }
      }
      catch
      {
        return 0;
      }
    }

    public static int GetRotateByExif(Image image)
    {
      foreach (PropertyItem propertyItem in image.PropertyItems)
      {
        if (propertyItem.Id == 274)
        {
          switch (Convert.ToInt16(propertyItem.Value[0]))
          {
            case (short)3:
              return 2;
            case (short)6:
              return 1;
            case (short)8:
              return 3;
            default:
              goto label_8;
          }
        }
      }
    label_8:
      return 0;
    }

    public enum ExifOrientations
    {
      None,
      Normal,
      HorizontalFlip,
      Rotate180,
      VerticalFlip,
      Transpose,
      Rotate270,
      Transverse,
      Rotate90,
    }
  }
}