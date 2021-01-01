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
using Xamarin.Essentials;
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

      // adjust size of icons in menu bar
      IScreenMetrics metrics = DependencyService.Get<IScreenMetrics>();
      double adjustedDensity = metrics.AdjustedDensity();

      StackLayout titlebar = (StackLayout)FindByName("titlebar");
      foreach (VisualElement child in titlebar.Children)
      {
//        if (child.GetType().Equals (typeof(ImageButton)))
        {
          if (child.HeightRequest != -1)
            child.HeightRequest = child.HeightRequest * adjustedDensity;
          if (child.WidthRequest != -1)
            child.WidthRequest = child.WidthRequest * adjustedDensity;
        }
      }
      titlebar.ForceLayout();

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

        // set folder description in title bar
        Folder.Text = folder[0].description;

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
      await DisplayAlert ("Menu", "Back clicked", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnPageClicked (object sender, EventArgs args)
    {
      await Navigation.PushAsync (new CollectionPage ());
    }

    //-----------------------------------------------------------------------------
    async void OnMenuClicked(object sender, EventArgs args)
    {
      // get bottom position of sender, which is the clicked buttoin
      View view = (View)sender;
      var senderBottom = view.Bounds.Bottom;

      // get popupMenu stacklayout
      VisualElement popupMenu = (VisualElement)FindByName("PopupMenu");

      // set popupLayout position; expecting X proportional, Y absolute, width proportional, height dynamic.
      var height = popupMenu.Bounds.Height;
      AbsoluteLayout.SetLayoutBounds(popupMenu, new Rectangle(0, senderBottom, 1, -1));

      // make the popupLayout layout visible (which contains the popupMenu)
      ShowPopupMenu(true);
      VisualElement popupLayout = (VisualElement)FindByName("PopupLayout");
      popupLayout.IsVisible = true;
    }

    //-----------------------------------------------------------------------------
    private void ShowPopupMenu (bool show)
    {
      VisualElement popupLayout = (VisualElement)FindByName("PopupLayout");
      popupLayout.IsVisible = show;
    }

    //-----------------------------------------------------------------------------
    async void OnOutsidePopupClicked(object sender, EventArgs args)
    {
      ShowPopupMenu(false);
    }

    //-----------------------------------------------------------------------------
    async void OnMenu1Clicked(object sender, EventArgs args)
    {
      ShowPopupMenu(false);
      await DisplayAlert("Menu", "Menu 1 clicked", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnMenu2Clicked(object sender, EventArgs args)
    {
      ShowPopupMenu(false);
      await DisplayAlert("Menu", "Menu 2 clicked", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnMenu3Clicked(object sender, EventArgs args)
    {
      ShowPopupMenu(false);
      await DisplayAlert("Menu", "Menu 3 clicked", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnMenu4Clicked(object sender, EventArgs args)
    {
      ShowPopupMenu(false);

      var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
      var density = mainDisplayInfo.Density;
      Debug.WriteLine("Screen Density: " + density);

      string msg = "Screen density: " + density;
      await DisplayAlert("Menu", msg, "OK");
    }
  }
}
