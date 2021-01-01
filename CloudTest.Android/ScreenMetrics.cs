using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
[assembly: Xamarin.Forms.Dependency(typeof(CloudTest.Droid.ScreenMetrics))]

namespace CloudTest.Droid
{
  public class ScreenMetrics : IScreenMetrics
  {
    public ScreenMetrics()
    {
    }

    public double AdjustedDensity()
    {
      var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
      var density = mainDisplayInfo.Density;
      return density * 0.45; // emperically determined
    }
  }
}