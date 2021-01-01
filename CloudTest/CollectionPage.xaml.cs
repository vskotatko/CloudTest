using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CloudTest.Nodes;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CloudTest
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CollectionPage : ContentPage
  {
    //-----------------------------------------------------------------------------
    public ObservableCollection<NodeData> nodes = new ObservableCollection<NodeData>();

    //-----------------------------------------------------------------------------
    public CollectionPage ()
    {
      InitializeComponent ();

      nodes.Add (new ImageData { FileName = "image_chair_pk.jpg" });
      nodes.Add (new ImageData { FileName = "image_chanty.jpg" });
      nodes.Add (new ImageData { FileName = "beach.jpg" });
      nodes.Add (new ImageData { FileName = "covid_wedding.jpg" });
      nodes.Add (new ImageData { FileName = "sidekick.png" });
      nodes.Add (new ImageData { FileName = "smoke.jpg" });

      DetailGrid.ItemsSource = nodes;
    }

    //-----------------------------------------------------------------------------
    async void HandleItemTapped (object sender, ItemTappedEventArgs e)
    {
      if (e.Item == null)
        return;
      
      await DisplayAlert ("Item " + e.ItemIndex + " Tapped", "An item was tapped.", "OK");
      
      //Deselect Item
//      ((ListView)sender).SelectedItem = null;
    }

    //-----------------------------------------------------------------------------
    async void OnAddClicked (object sender, EventArgs args)
    {
      await DisplayAlert("Add Tapped", "An button was tapped.", "OK");
//      nodes.Add (new NoteData { Note = DateTime.Now.ToString() });
    }

    //-----------------------------------------------------------------------------
    async void OnBackClicked (object sender, EventArgs args)
    {
      await DisplayAlert ("Back", "Back", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnPageClicked (object sender, EventArgs args)
    {
      await Navigation.PushAsync(new ListPage());
    }

    //-----------------------------------------------------------------------------
    async void OnResetClicked(object sender, EventArgs args)
    {
      await DisplayAlert("Menu", "Reset clicked", "OK");
    }

    //-----------------------------------------------------------------------------
    async void OnMenuClicked (object sender, EventArgs args)
    {
      await DisplayAlert("Menu", "Menu", "OK");
    }
  }
}
