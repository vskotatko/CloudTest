using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
[assembly: Xamarin.Forms.Dependency(typeof(CloudTest.UWP.ScreenMetrics))]

namespace CloudTest.UWP
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
