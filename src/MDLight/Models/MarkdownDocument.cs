
using System.IO;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;

using Markdig;

using Windows.Storage;

namespace MDLight.Models
{
    public class MarkdownDocument : ObservableObject
    {
        public string FileName { get; set; }

        public StorageFile File { get; set; }

        public string Title => string.IsNullOrEmpty(FileName) ? "Untitled" : Path.GetFileNameWithoutExtension(FileName);

        private string _contents;
        public string Contents { get => _contents; set => SetProperty(ref _contents, value); }

        public byte[] FileBytes => string.IsNullOrEmpty(Contents) ? System.Array.Empty<byte>() : Encoding.UTF8.GetBytes(Contents);

        public string MarkDown => Markdown.ToHtml(Contents);

        private bool _isEdit;
        public bool IsEdit { get => _isEdit; set => SetProperty(ref _isEdit, value); }
    }
}
