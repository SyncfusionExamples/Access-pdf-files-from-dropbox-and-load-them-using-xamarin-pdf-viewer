using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DropboxSample.UWP
{
    public sealed partial class MainPage
    {
        //static string[] Scopes = { DriveService.Scope.DriveReadonly };
        //static string ApplicationName = "Drive API .NET Quickstart";

        public MainPage()
       {
            this.InitializeComponent();
            LoadApplication(new DropboxSample.App());

            //UserCredential credential;

            //string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "token.json");
            //Stream stream1 = typeof(MainPage).GetType().Assembly.GetManifestResourceStream("LoadPDFFromURL.UWP.credentials.json");

            //credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream1).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore("token.json", true)).Result;
        }
    }
}
