using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using CloudTest.Nodes;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CloudTest
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ListPage : ContentPage
  {
    //-----------------------------------------------------------------------------
    public ObservableCollection<NodeData> nodes = new ObservableCollection<NodeData>();

    //-----------------------------------------------------------------------------
    private class Item // members must be public for DeserializeObject.
    {
      public int id { get; set; }
      public String description { get; set; }
    }

    //-----------------------------------------------------------------------------
    public ListPage ()
    {
      InitializeComponent ();

      // toolbar
      ToolbarItem item = (ToolbarItem)FindByName ("back");
      item.IconImageSource = ImageSource.FromResource ("CloudTest.Assets.icons.arrow_back-24px.png"); 
      item = (ToolbarItem)FindByName ("page");
      item.IconImageSource = ImageSource.FromResource ("CloudTest.Assets.icons.crop_din-24px.png");

      LoadList();

      DetailList.ItemsSource = nodes;
    }

    //-----------------------------------------------------------------------------
    public async Task LoadList ()
    {
      try
      {
        HttpClient httpClient = ((App)App.Current).httpClient;
        HttpResponseMessage response = null;

        // get folder
        Uri uri = new Uri("https://xamarin.perinote.com/root");
        response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
          // exit and send notification to UI thread
          return;
        }
        string results = await response.Content.ReadAsStringAsync();
        var folder = JsonConvert.DeserializeObject<Item[]> (results);
        if (folder.Length != 1)
        {
          // exit and send notification to UI thread
          return;
        }

        // get items
        uri = new Uri("https://xamarin.perinote.com/children?parent=" + folder[0].id);
        response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
          // exit and send notification to UI thread
          return;
        }
        results = await response.Content.ReadAsStringAsync();
        var children = JsonConvert.DeserializeObject<Item[]>(results);

        Device.BeginInvokeOnMainThread (() => 
        {
          foreach (var child in children)
            nodes.Add (new NoteData { Note = child.description });
        });
      }
      catch (Exception e)
      {
        Debug.WriteLine("http exception: ", e.ToString());
      }
    }

    //-----------------------------------------------------------------------------
    async void HandleItemTapped (object sender, ItemTappedEventArgs e)
    {
      if (e.Item == null)
        return;
      
      await DisplayAlert ("Item " + e.ItemIndex + " Tapped", "An item was tapped.", "OK");
      
      //Deselect Item
      ((ListView)sender).SelectedItem = null;
    }

    //-----------------------------------------------------------------------------
    void OnAddClicked (object sender, EventArgs args)
    {
//      await DisplayAlert("Add Tapped", "An button was tapped.", "OK");
      nodes.Add (new NoteData { Note = DateTime.Now.ToString() });
    }

    //-----------------------------------------------------------------------------
    async void OnBackClicked (object sender, EventArgs args)
    {
      await DisplayAlert ("Back", "Back", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnPageClicked (object sender, EventArgs args)
    {
      await Navigation.PushAsync (new CollectionPage ());
    }

    //-----------------------------------------------------------------------------
    async void OnMenuClicked (object sender, EventArgs args)
    {
      await DisplayAlert("Menu", "Menu", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnLoadError (object sender, EventArgs args)
    {
      
    }
  }
}
