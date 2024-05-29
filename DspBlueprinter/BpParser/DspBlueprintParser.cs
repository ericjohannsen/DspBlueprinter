using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using DspBlueprinter;

namespace DspBlueprintParser
{
    public static class DspBlueprintParser
    {
        public class Error : Exception { }

        public static BlueprintData Parse(string strBlueprint)
        {
            if (strBlueprint.Length < 28 || !strBlueprint.StartsWith("BLUEPRINT:"))
                return null;

            var parser = new Parser(strBlueprint);
            return parser.Blueprint;
        }

        public static bool IsValid(string input)
        {
            if (input.Length < 28 || !input.StartsWith("BLUEPRINT:"))
                return false;

            var sections = new DataSections(input);
            var hash = MD5F.Compute(sections.HashedString);

            return sections.Hash == hash;
        }
    }
}
