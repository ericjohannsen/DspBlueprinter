using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class FileManager
    {
        public static string BlueprintFolder => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Dyson Sphere Program", "Blueprint");

        public static IEnumerable<string> BlueprintFiles() =>
            Directory.EnumerateFiles(BlueprintFolder, "*.txt");

        internal static string VariantPath(string originalBlueprintPath, string subfolderName)
        {
            var originalFolder = Path.GetDirectoryName(originalBlueprintPath) ?? throw new ArgumentException($"Invalid path {originalBlueprintPath}");
            var originalFile = Path.GetFileName(originalBlueprintPath);
            return Path.Combine(originalFolder, subfolderName, originalFile);
        }
    }
}
