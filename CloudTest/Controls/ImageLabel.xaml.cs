using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CloudTest.Controls
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ImageLabel : Label
  {
    public ImageLabel()
    {
      InitializeComponent();
    }

    public static readonly BindableProperty LabelProperty =
      BindableProperty.Create(nameof(Source), typeof(string), typeof(ImageLabel),
                              default(string), Xamarin.Forms.BindingMode.OneWay);

    public string Source
    {
      get { return (string)GetValue(LabelProperty); }
      set { SetValue(LabelProperty, value); }
    }

  }
}