
using System.IO;
using System.Text;

namespace MDLight.Models
{
    internal class Document
    {
        public string FileName { get; set; }

        public string Title => string.IsNullOrEmpty(FileName) ? "Untitled" : Path.GetFileNameWithoutExtension(FileName);

        public string Contents { get; set; }

        public byte[] FileBytes => string.IsNullOrEmpty(Contents)? System.Array.Empty<byte>() : Encoding.UTF8.GetBytes(Contents);
    }
}
