using System;
using System.Collections.Generic;
using System.Text;
using CloudTest.Nodes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CloudTest
{
  class NodeTemplateSelector : DataTemplateSelector
  {
    public DataTemplate NoteItemTemplate { get; set; }
    public DataTemplate ImageItemTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate (object item, BindableObject container)
    {
      ListView list = (ListView)container;
      if (item is NoteData)
        return NoteItemTemplate;
      else // item is ImageData
        return ImageItemTemplate;
    }
  }

  [ContentProperty (nameof(SourceFileName))]
  class ImageResourceExtension : IMarkupExtension
  {
    public string SourceFileName { get; set; }

    public object ProvideValue (IServiceProvider serviceProvider)
    {
      if (SourceFileName == null)
        return null;

      var imageSource = ImageSource.FromResource (SourceFileName);
      return imageSource;
    }
  }
}
