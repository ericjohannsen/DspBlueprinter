using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class VariantFactory
    {
        static public void CreateVariants(IEnumerable<VariantDescripion> variants)
        {
            foreach (var variant in variants)
            {
                Console.WriteLine($"Creating variant: {variant.SubfolderName}");

                var folder = Path.Combine(FileManager.BlueprintFolder, variant.SubfolderName);
                Directory.CreateDirectory(folder);
                foreach (var bpFile in FileManager.BlueprintFiles())
                {
                    Console.WriteLine($"    Blueprint: {Path.GetFileName(bpFile)}");

                    var original = Blueprint.ReadFromFile(bpFile);
                    var modified = Modify(original, variant);
                    var saveAs = FileManager.VariantPath(bpFile, variant.SubfolderName);
                    modified.WriteToFile(saveAs);
                }
            }
        }

        private static Blueprint Modify(Blueprint original, VariantDescripion variant)
        {
            var replacementMap = variant.BuildingReplacementMap();
            var modified = original.Clone();
            for (int i = 0; i < original.DecodedData.Buildings.Count; i++)
            {
                var building = original.DecodedData.Buildings[i];
                if (replacementMap.TryGetValue(building.Item, out var replaceWith))
                {
                    Console.WriteLine($"        Replacing {modified.DecodedData.Buildings[i].Item} with {replaceWith}");
                    modified.DecodedData.Buildings[i].ChangeBuilding(replaceWith);
                }
            }

            return modified;
        }
    }
}
