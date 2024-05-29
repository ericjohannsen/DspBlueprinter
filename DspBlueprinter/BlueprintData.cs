using DspBlueprinter.Diagnosis;
using DspBlueprinter.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DspBlueprinter
{


    public class BlueprintData
    {
        private static readonly NamedStruct _HEADER = new NamedStruct(new (string, string)[]
        {
        ("L", "version"),
        ("L", "cursor_offset_x"),
        ("L", "cursor_offset_y"),
        ("L", "cursor_target_area"),
        ("L", "dragbox_size_x"),
        ("L", "dragbox_size_y"),
        ("L", "primary_area_index"),
        ("b", "area_count")
        });

        private static readonly NamedStruct _BUILDING_HEADER = new NamedStruct(new (string, string)[]
        {
        ("L", "building_count")
        });

        private IReadOnlyDictionary<string, object> _header;
        private IReadOnlyList<BlueprintArea> _areas;
        private IReadOnlyList<BlueprintBuilding> _buildings;

        public BlueprintData(ImmutableDictionary<string, object> header, ImmutableList<BlueprintArea> areas, ImmutableList<BlueprintBuilding> buildings)
        {
            _header = header;
            _areas = areas;
            _buildings = buildings;
        }

        public IReadOnlyList<BlueprintBuilding> Buildings => _buildings;

        public ImmutableDictionary<string, object> ToDict()
        {
            var result = _header.ToDictionary(entry => entry.Key, entry => entry.Value);
            result["areas"] = _areas.Select(area => area.ToDict()).ToList();
            result["buildings"] = _buildings.Select(building => building.ToDict()).ToList();
            return result.ToImmutableDictionary();
        }

        public static BlueprintData Deserialize(byte[] data)
        {
            var header = _HEADER.UnpackHead(data);

            var areas = new List<BlueprintArea>();
            int offset = _HEADER.Size;

            for (int areaId = 0; areaId < (byte)header["area_count"].data; areaId++)
            {
                var area = BlueprintArea.Deserialize(data, offset);
                offset += area.Size;
                areas.Add(area);
            }

            var buildings = new List<BlueprintBuilding>();
            var buildingHeader = _BUILDING_HEADER.UnpackHead(data, offset);
            offset += _BUILDING_HEADER.Size;
            for (uint buildingId = 0; buildingId < (uint)buildingHeader["building_count"].data; buildingId++)
            {
                var building = BlueprintBuilding.Deserialize(data, offset);

                offset += building.Size;
                buildings.Add(building);
            }

            return new BlueprintData(
                header.Select(h => new { h.Key, Value = h.Value.data }).ToDictionary(k => k.Key, v => v.Value).ToImmutableDictionary(), 
                areas.ToImmutableList(), 
                buildings.ToImmutableList());
        }
    }
}
