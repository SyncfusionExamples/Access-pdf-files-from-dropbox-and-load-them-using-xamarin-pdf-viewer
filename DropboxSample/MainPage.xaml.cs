using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Syncfusion.ListView.XForms;

namespace DropboxSample
{
    public partial class MainPage : ContentPage
    {
        private PopupListView popupListView;
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pdfViewerBindingContext.CheckAuthorization();
            if (pdfViewerBindingContext.IsAuthorized)
            {
                ShowFilesDirectory();
            }
        }

        private async void ShowFilesDirectory()
        {
            if (popupListView == null)
            {
                popupListView = new PopupListView(this, PopupLayout);
                PopupLayout.PopupView.ContentTemplate = new DataTemplate(() => { return popupListView; });
                PopupLayout.PopupView.ShowCloseButton = true;
                PopupLayout.PopupView.ShowFooter = false;
                PopupLayout.PopupView.ShowHeader = true;
                PopupLayout.PopupView.HeaderTitle = "Choose a File";

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    PopupLayout.PopupView.WidthRequest = this.Width - 50;
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    PopupLayout.PopupView.HeightRequest = this.Height / 2;
                    PopupLayout.PopupView.WidthRequest = this.Width / 1.5;
                    PopupLayout.ShowOverlayAlways = true;
                    PopupLayout.Show();
                }
                else if (Device.Idiom == TargetIdiom.Desktop)
                {
                    PopupLayout.PopupView.WidthRequest = 750;
                }
            }

            List<ListViewItem> listViewItems = new List<ListViewItem>();
            var filesCollection = await pdfViewerBindingContext.GetPdfFiles();
            PopupLayout.Show();
            foreach (var file in filesCollection)
            {
                listViewItems.Add(
                    new ListViewItem
                    {
                        FileName = file,
                        Icon = "PDF.png"
                    });
            }
            popupListView.SetItemSourceForListView(listViewItems);
        }

        private void FilesDirectoryButton_Clicked(object sender, EventArgs e)
        {
            ShowFilesDirectory();
        }

        private async void pdfViewerControl_DocumentSaveInitiated(object sender, Syncfusion.SfPdfViewer.XForms.DocumentSaveInitiatedEventArgs args)
        {
            args.SaveStream.Position = 0;
            string fileName = await DisplayPromptAsync("Save As", "Enter the file name", "OK", "Cancel", maxLength: 250, keyboard: Keyboard.Text);
            if (fileName != null)
            {
                pdfViewerBindingContext.UploadPdfStream(args.SaveStream as MemoryStream, fileName);
                await this.DisplayAlert("Success", "File saved successfully.", "OK");
            }
        }
    }
}
