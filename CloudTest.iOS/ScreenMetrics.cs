using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Essentials;
[assembly: Xamarin.Forms.Dependency(typeof(CloudTest.iOS.ScreenMetrics))]

namespace CloudTest.iOS
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
      return density;
    }
  }
}