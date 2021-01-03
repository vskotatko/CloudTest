using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CloudTest.Controls
{
  class StretchImage : SKCanvasView
  {
    //    public static readonly BindableProperty SourceProperty =
    //      BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(StretchImage),
    //                              null, Xamarin.Forms.BindingMode.OneWay);

    //-----------------------------------------------------------------------------
    public static readonly BindableProperty LabelProperty =
      BindableProperty.Create(nameof(Label), typeof(string), typeof(StretchImage),
                              null, Xamarin.Forms.BindingMode.OneWay,
                              propertyChanged: OnLabelPropertyChanged);

    //-----------------------------------------------------------------------------
    public static readonly BindableProperty SourceProperty =
      BindableProperty.Create(nameof(Source), typeof(string), typeof(StretchImage),
                              null, Xamarin.Forms.BindingMode.OneWay,
                              propertyChanged: OnSourcePropertyChanged);

    //-----------------------------------------------------------------------------
    // We need this because when XAML is "inflated", only SetValue() gets called, not Label.set().
    private static void OnLabelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
      StretchImage si = (StretchImage)bindable;
      si.SetElementWidth();
    }

    //-----------------------------------------------------------------------------
    // We need this because when XAML is "inflated", only SetValue() gets called, not Source.set().
    private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
      StretchImage si = (StretchImage)bindable;
      si.LoadBmpSrc();
      si.SetElementWidth();
    }

    //-----------------------------------------------------------------------------
    public string Label
    {
      get { return (string)GetValue(LabelProperty); }
      set 
      { 
        SetValue(LabelProperty, value);
//        SetElementWidth();

        // refresh
        InvalidateSurface();
      }
    }

    //-----------------------------------------------------------------------------
    public string Source
    {
      get { return (string)GetValue(SourceProperty); }
      set
      {
        SetValue(SourceProperty, value);
//        LoadBmpSrc();
      }
    }

    private float textCoreHeight = 0; // in actual device pixels
    private SKRect textBounds = new SKRect(); // device pixels
    private SKBitmap bmpSrc = null; // use this instead of Source

    //-----------------------------------------------------------------------------
    private void LoadBmpSrc()
    {
      Assembly assembly = GetType().GetTypeInfo().Assembly;
      using (Stream stream = assembly.GetManifestResourceStream(Source))
      {
        bmpSrc = SKBitmap.Decode(stream);
      }
    }

    //-----------------------------------------------------------------------------
    // Call this whenever either the Source or the Label changes
    private void SetElementWidth()
    {
      if (Label == null)
        return; // this will get called again when the Label is set.

      // determine space needed for text -- units are actual device pixels
      using (SKPaint textPaint = new SKPaint())
      {
        textPaint.TextSize = textPaint.TextSize * (float)DeviceDisplay.MainDisplayInfo.Density;
        textPaint.MeasureText("X", ref textBounds);
        textCoreHeight = textBounds.Height;
        textPaint.MeasureText(Label, ref textBounds);
      }
      float minWidth = textBounds.Width + DiuToPixels(40); // in actual device pixels

      // make sure minWidth is >= source bitmap width
      if (bmpSrc != null)
      {
        var h = DiuToPixels((float)this.Height);
        var scale = (float)h / (float)bmpSrc.Height;
        var scaledBmpWidth = scale * bmpSrc.Width;

        if (bmpSrc != null)
          if (scaledBmpWidth > minWidth)
            minWidth = scaledBmpWidth;
      }

      // set element width
      textBounds.Left = 0;
      textBounds.Right = minWidth;
      var w = PixelsToDiu(minWidth);
      WidthRequest = w;
    }

    //-----------------------------------------------------------------------------
    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
      if (bmpSrc == null)
        return;

      // identify left, right halves and a 10px wide swath of the middle of the source bitmap
      SKRect rectSrcLeft = new SKRect(0, 0, bmpSrc.Width / 2, bmpSrc.Height);
      SKRect rectSrcRight = new SKRect(bmpSrc.Width / 2, 0, bmpSrc.Width, bmpSrc.Height);
      SKRect rectSrcMid = new SKRect(bmpSrc.Width / 2 - 5, 0, bmpSrc.Width / 2 + 5, bmpSrc.Height);

      // create a new bitmap containing a 10 pixel wide swatch from middle of bmpSrc
      SKBitmap bmpSrcMid = new SKBitmap(10, bmpSrc.Height);
      using (SKCanvas tempCanvas = new SKCanvas(bmpSrcMid))
      {
        SKRect rectDest = new SKRect(0, 0, rectSrcMid.Width, rectSrcRight.Height);
        tempCanvas.DrawBitmap(bmpSrc, rectSrcMid, rectDest);
      }

      var canvas = e.Surface.Canvas;

      // scale canvas so that bmpSrc matches in height
      canvas.Save();
      float hDest = canvas.DeviceClipBounds.Height;
      float scale = hDest / (float)bmpSrc.Height;
      canvas.Scale(scale);

      // determine dest rect for middle section
      float rightDest = (float)textBounds.Width / scale; // rightmost point of whole target area
      SKRect rectDestMid = new SKRect(rectSrcLeft.Width, 0, rightDest - rectSrcRight.Width, rectSrcRight.Height);

      // left part of tab
      canvas.DrawBitmap(bmpSrc, rectSrcLeft, rectSrcLeft);
  
      // right part of tab
      {
        SKRect rectDest = new SKRect(rectDestMid.Right, 0, rightDest, rectSrcRight.Height);
        canvas.DrawBitmap(bmpSrc, rectSrcRight, rectDest);
      }

      // mid part of tab
      using (SKPaint paint = new SKPaint())
      {
        paint.Shader = SKShader.CreateBitmap(bmpSrcMid,
                                              SKShaderTileMode.Repeat,
                                              SKShaderTileMode.Repeat);
        // Draw background
        canvas.DrawRect(rectDestMid, paint);
      }

      canvas.Restore(); // back to orig scale

      using (SKPaint paint = new SKPaint { Color = SKColors.Black })
      {
        paint.TextSize = paint.TextSize * (float)DeviceDisplay.MainDisplayInfo.Density;
        float leftText = DiuToPixels(20); // matches padding in ListPage.xaml
        float topText = canvas.DeviceClipBounds.Height / 2 + textCoreHeight / 2;
        Debug.WriteLine("SI: text LT " + leftText + ", " + topText);
        canvas.DrawText(Label, new SKPoint(leftText, topText), paint);
      }
    }

    //-----------------------------------------------------------------------------
    float DiuToPixels(float diu)
    {
      float pixels = (float)DeviceDisplay.MainDisplayInfo.Density * diu;
      return pixels;
    }

    //-----------------------------------------------------------------------------
    float PixelsToDiu(float pixels)
    {
      var diu = pixels / (float)DeviceDisplay.MainDisplayInfo.Density;
      return diu;
    }

  }
}
