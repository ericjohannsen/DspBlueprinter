using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DspBlueprintParser
{
    public class DataSections
    {
        private readonly string _strBlueprint;
        private readonly int _firstQuoteLoc;
        private readonly int _secondQuoteLoc;

        public DataSections(string strBlueprint)
        {
            _strBlueprint = strBlueprint.Trim();
            _firstQuoteLoc = _strBlueprint.IndexOf('"');
            _secondQuoteLoc = _strBlueprint.IndexOf('"', _firstQuoteLoc + 1);
        }

        public string[] HeaderSegments => _strBlueprint.Substring(10, _firstQuoteLoc - 10).Split(',');

        public string HashedString => _strBlueprint.Substring(0, _secondQuoteLoc);

        public string Hash => _strBlueprint.Substring(_secondQuoteLoc + 1);

        public byte[] DecompressedBody
        {
            get
            {
                var base64String = _strBlueprint.Substring(_firstQuoteLoc + 1, _secondQuoteLoc - _firstQuoteLoc - 1);
                var compressedBytes = Convert.FromBase64String(base64String);

                using (var inputStream = new MemoryStream(compressedBytes))
                using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var outputStream = new MemoryStream())
                {
                    gZipStream.CopyTo(outputStream);
                    return outputStream.ToArray();
                }
            }
        }
    }
}
