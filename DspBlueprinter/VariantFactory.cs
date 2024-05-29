using DspBlueprinter.Enums;
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

        static Dictionary<DysonSphereItem, DysonSphereModel> itemToModelMap = new Dictionary<DysonSphereItem, DysonSphereModel>()
        {
            { DysonSphereItem.ConveyorBeltMKI, DysonSphereModel.ConveyorBeltMKI},
            { DysonSphereItem.ConveyorBeltMKII, DysonSphereModel.ConveyorBeltMkII },
            { DysonSphereItem.ConveyorBeltMKIII, DysonSphereModel.ConveyorBeltMkIII },
            { DysonSphereItem.SorterMKI, DysonSphereModel.SorterMKI },
            { DysonSphereItem.SorterMKII, DysonSphereModel.SorterMKII },
            { DysonSphereItem.SorterMKIII, DysonSphereModel.SorterMKIII },
            { DysonSphereItem.PileSorter, DysonSphereModel.PileSorter },
        };
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
                    var dataOffset = building.ItemIdOffset;
                    if (itemToModelMap.TryGetValue(replaceWith, out var model))
                    {
                        modified.WriteDataInt16(dataOffset, (short)replaceWith);
                        modified.WriteDataInt16(dataOffset + 2, (short)model);
                    }
                    else
                        throw new Exception($"Item ID {replaceWith} is not mapped to a model ID. Add a mapping.");                    
                }
            }

            return modified;
        }
    }
}
