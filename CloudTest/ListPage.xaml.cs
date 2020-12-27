using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    int parentId = -1; // -1 == undefined

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
        parentId = folder[0].id;

        // get items
        uri = new Uri("https://xamarin.perinote.com/children?parent=" + parentId);
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
    async void OnAddClicked (object sender, EventArgs args)
    {
      if (parentId == -1)
      {
        DisplayAlert("Error", "Can't add, parentId is -1.", "OK");
        return;
      }

      // create some text for the new note
      string description = DateTime.Now.ToString();

      // prepare data for the addnote endpoint
      Uri uri = new Uri("https://xamarin.perinote.com/addnote");
      MultipartFormDataContent content = new MultipartFormDataContent();
      content.Headers.ContentType.MediaType = "multipart/form-data";
      content.Add(new StringContent(parentId.ToString()), "parent");
      content.Add(new StringContent(description), "description");

      // post
      HttpClient httpClient = ((App)App.Current).httpClient;
      HttpResponseMessage response = null;
      response = await httpClient.PostAsync(uri, content);
      if (!response.IsSuccessStatusCode)
      {
        DisplayAlert("Error", response.StatusCode.ToString(), "OK");
        return;
      }

      // display
      nodes.Add(new NoteData { Note = description });
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
