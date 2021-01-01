using CloudTest.DbAccess;
using System;
using System.Diagnostics;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CloudTest
{
  public partial class App : Application
  {
    //-----------------------------------------------------------------------------
    public HttpClient httpClient = null;

    //-----------------------------------------------------------------------------
    public App ()
    {
      httpClient = new HttpClient();
      Console.WriteLine("OnStart.begin");

      InitializeComponent();

      MainPage = new NavigationPage(new ListPage());
    }

    //-----------------------------------------------------------------------------
    protected override void OnStart()
    {
    }

    //-----------------------------------------------------------------------------
    protected override void OnSleep ()
    {
    }

    //-----------------------------------------------------------------------------
    protected override void OnResume ()
    {
    }
  }
}
