using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Dropbox.Api;
using Dropbox.Api.Files;

using Xamarin.Forms;

namespace DropboxSample
{
    public class DropBoxService
    {
        #region Constants

        private const string ClientId = "Your App secret key";

        private const string RedirectUri = "Your Redirect Uri";

        #endregion

        #region Fields

        /// <summary>
        ///     Occurs when the user was authenticated
        /// </summary>
        public Action OnAuthenticated;

        private string oauth2State;

        #endregion

        #region Properties

        public string AccessToken { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     <para>Runs the Dropbox OAuth authorization process if not yet authenticated.</para>
        ///     <para>Upon completion <seealso cref="OnAuthenticated"/> is called</para>
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        public async Task Authorize()
        {
            Application.Current.Properties.Clear();

            if (string.IsNullOrWhiteSpace(this.AccessToken) == false)
            {
                // Already authorized
                this.OnAuthenticated?.Invoke();
                return;
            }

            // Run Dropbox authentication
            this.oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ClientId, new Uri(RedirectUri), this.oauth2State);
            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += this.WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            await Application.Current.MainPage.Navigation.PushModalAsync(contentPage);
        }

        /// <summary>
        /// Gets the Dropbox file list
        /// </summary>
        /// <returns>An asynchronous task</returns>
        public async Task<IList<Metadata>> ListFiles()
        {
            try
            {
                using (var client = this.GetClient())
                {
                    var list = await client.Files.ListFolderAsync(string.Empty, true);
                    return list?.Entries;
                }
            }
            catch (Exception)
            {                
                return null;
            }
        }

        /// <summary>
        /// Reads the file from Dropbox and returns it as byte array
        /// </summary>
        /// <returns>An asynchronous task</returns>
        public async Task<byte[]> ReadFile(string filePath)
        {
            try
            {
                using (var client = this.GetClient())
                {
                    var response = await client.Files.DownloadAsync(filePath);
                    var bytes = response?.GetContentAsByteArrayAsync();
                    return bytes?.Result;
                }
            }
            catch (Exception)
            {                
                return null;
            }
        }

        /// <summary>
        /// Writes the file to the Dropbox
        /// </summary>
        /// <returns>An asynchronous task</returns>
        public async Task<FileMetadata> WriteFile(MemoryStream fileContent, string fileName)
        {
            try
            {
                var commitInfo = new CommitInfo(fileName, WriteMode.Overwrite.Instance, false, DateTime.Now);

                using (var client = this.GetClient())
                {
                    var metadata = await client.Files.UploadAsync(commitInfo, fileContent);
                    return metadata;
                }
            }
            catch (Exception)
            {                
                return null;
            }
        }

        #endregion

        #region Methods

        private DropboxClient GetClient()
        {
            return new DropboxClient(this.AccessToken);
        }

        private async void WebViewOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                return;
            }

            try
            {
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != this.oauth2State)
                {
                    return;
                }

                this.AccessToken = result.AccessToken;
                this.OnAuthenticated?.Invoke();
            }
            catch (ArgumentException)
            {
                // There was an error in the URI passed
            }
            finally
            {
                e.Cancel = true;
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        #endregion
    }
}