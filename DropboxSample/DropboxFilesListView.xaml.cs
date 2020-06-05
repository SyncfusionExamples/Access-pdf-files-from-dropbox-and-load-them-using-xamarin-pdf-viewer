
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DropboxSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupListView : ContentView
    {
        private DropboxViewModel viewModel;
        private SfPopupLayout popupLayout;

         public PopupListView(MainPage mainPage, SfPopupLayout sfPopupLayout)
        {
            InitializeComponent();
            viewModel = mainPage.BindingContext as DropboxViewModel;
            popupLayout = sfPopupLayout;

            var gridLayout = new GridLayout();
            if (Device.Idiom == TargetIdiom.Phone)
            {
                gridLayout.SpanCount = 1;
            }
            else if(Device.Idiom == TargetIdiom.Desktop)
            {
                gridLayout.SpanCount = 2;
            }
            DropboxFilesListView.LayoutManager = gridLayout;           
        }

        internal void SetItemSourceForListView(List<ListViewItem> itemSource)
        {
            DropboxFilesListView.ItemsSource = itemSource;
        }

        private void DropboxFilesListView_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            ListViewItem selectedItem = e.ItemData as ListViewItem;
            viewModel.DownloadPdfStream(selectedItem.FileName);
            popupLayout.IsOpen = false;
        }
    }

    public class ListViewItem
    {
        public string Icon { get; set; }
        public string FileName { get; set; }

    }
}