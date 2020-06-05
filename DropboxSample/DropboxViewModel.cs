using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dropbox.Api;

namespace DropboxSample
{
    class DropboxViewModel : INotifyPropertyChanged
    {
        private Stream pdfDocumentStream;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly DropBoxService dropBoxService = new DropBoxService();        
        public bool IsAuthorized;

        public Stream PdfDocumentStream
        {
            get { return pdfDocumentStream; }
            set
            {
                //Check the value whether it is the same as the currently loaded stream
                if (value != pdfDocumentStream)
                {
                    pdfDocumentStream = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("PdfDocumentStream"));
                }
            }
        }

        /// <summary>
        /// Check whether the crendentials are authorized
        /// </summary>
        public async void CheckAuthorization()
        {
            if (this.dropBoxService.AccessToken == null)
            {
                await this.dropBoxService.Authorize();
            }
            else
            {
                IsAuthorized = true;
            }
        }

        /// <summary>
        /// Gets the Dropbox file list
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetPdfFiles()
        {
            List<string> dropboxPdfFiles = new List<string>();
            var list = await this.dropBoxService.ListFiles();
            foreach (var item in list.Where(i => i.IsFile))
            {
                if(item.Name.EndsWith(".pdf"))
                    dropboxPdfFiles.Add(item.PathDisplay);
            }
            return dropboxPdfFiles;
        }

        /// <summary>
        /// Gets the file/document stream from Dropbox
        /// </summary>
        /// <param name="fileName"></param>
        public async void DownloadPdfStream(string fileName)
        {
            var byteArray = await this.dropBoxService.ReadFile(fileName);
            PdfDocumentStream = new MemoryStream(byteArray);            
        }

        /// <summary>
        /// Uploads the file/document to the Dropbox
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public async void UploadPdfStream(MemoryStream stream, string fileName)
        {
            await this.dropBoxService.WriteFile(stream, fileName);
        }
    }
}
