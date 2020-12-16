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
  public partial class ListPage : ContentPage
  {
    //-----------------------------------------------------------------------------
    public ObservableCollection<NodeData> nodes = new ObservableCollection<NodeData>();

    //-----------------------------------------------------------------------------
    public ListPage ()
    {
      InitializeComponent ();

      // toolbar
      ToolbarItem item = (ToolbarItem)FindByName ("back");
      item.IconImageSource = ImageSource.FromResource ("CloudTest.Assets.icons.arrow_back-24px.png"); 
      item = (ToolbarItem)FindByName ("page");
      item.IconImageSource = ImageSource.FromResource ("CloudTest.Assets.icons.crop_din-24px.png");

      nodes.Add (new NoteData { Note = "As Donald Trump tried to claim victory before votes were tallied in critical battleground states, the Biden campaign was privately telling supporters not to panic, even as it prepared for pitched legal battles with the president." });
      nodes.Add (new NoteData { Note = "In a Zoom call with donors Wednesday, the aides told the group that Joe Biden was on pace to reach 270 electoral votes in short order, beaming over victories in the Midwestern states that Donald Trump flipped four years ago." });
      nodes.Add (new ImageData { FileName = "image_chair_pk.jpg" });
      nodes.Add (new ImageData { FileName = "image_chanty.jpg" });
      nodes.Add (new NoteData { Note = "Item 3" });
      nodes.Add (new NoteData { Note = "Item 4" });
      nodes.Add (new NoteData { Note = "Item 5" });
      nodes.Add (new NoteData { Note = "Item 6" });
      nodes.Add (new NoteData { Note = "Item 7" });
      nodes.Add (new NoteData { Note = "Item 8" });
      nodes.Add (new NoteData { Note = "Item 9" });

      DetailList.ItemsSource = nodes;
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
  }
}
