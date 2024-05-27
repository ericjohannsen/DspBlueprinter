using DspBlueprinter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class VariantDescripion
    {
        public string SubfolderName { get; set; } = "SETME";
        public List<(List<DysonSphereItem> find, DysonSphereItem replace)> BuildingReplacements { get; private set; } 
            = new List<(List<DysonSphereItem> find, DysonSphereItem replace)>();

        public IReadOnlyDictionary<DysonSphereItem, DysonSphereItem> BuildingReplacementMap() =>
            BuildingReplacements.SelectMany(b => b.find, (k, b) => new { Replace = k.replace, With = b })
            .ToDictionary(k => k.With, v => v.Replace);

        public static IEnumerable<VariantDescripion> BuiltinVariants()
        {
            yield return new VariantDescripion
            {
                SubfolderName = "Mk1BeltSorter",
                BuildingReplacements = new List<(List<DysonSphereItem> find, DysonSphereItem replace)>()
                {
                    (new List<DysonSphereItem>() { DysonSphereItem.ConveyorBeltMKII, DysonSphereItem.ConveyorBeltMKIII }, 
                        DysonSphereItem.ConveyorBeltMKI),
                    (new List<DysonSphereItem>() { DysonSphereItem.SorterMKII, DysonSphereItem.SorterMKIII },
                        DysonSphereItem.SorterMKI),
                }
            };
            yield return new VariantDescripion
            {
                SubfolderName = "Mk2BeltSorter",
                BuildingReplacements = new List<(List<DysonSphereItem> find, DysonSphereItem replace)>()
                {
                    (new List<DysonSphereItem>() { DysonSphereItem.ConveyorBeltMKI, DysonSphereItem.ConveyorBeltMKIII },
                        DysonSphereItem.ConveyorBeltMKII),
                    (new List<DysonSphereItem>() { DysonSphereItem.SorterMKI, DysonSphereItem.SorterMKIII },
                        DysonSphereItem.SorterMKII),
                }
            };
            yield return new VariantDescripion
            {
                SubfolderName = "Mk3BeltSorter",
                BuildingReplacements = new List<(List<DysonSphereItem> find, DysonSphereItem replace)>()
                {
                    (new List<DysonSphereItem>() { DysonSphereItem.ConveyorBeltMKI, DysonSphereItem.ConveyorBeltMKII },
                        DysonSphereItem.ConveyorBeltMKIII),
                    (new List<DysonSphereItem>() { DysonSphereItem.SorterMKI, DysonSphereItem.SorterMKII },
                        DysonSphereItem.SorterMKIII),
                }
            };
        }
    }
}
