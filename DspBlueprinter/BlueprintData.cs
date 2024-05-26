using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Web;

    public enum LogisticsStationDirection
    {
        Input = 0,
        Output = 1,
        Both = 2
    }

    public enum DysonSphereItem
    {
        Unknown = 0,
        PlanetaryLogisticsStation = 1,
        InterstellarLogisticsStation = 2,
        // Add other item types as needed
    }

    public class StationParameters
    {
        private static readonly int _STORAGE_OFFSET = 0;
        private static readonly int _SLOTS_OFFSET = _STORAGE_OFFSET + 192;
        private static readonly int _PARAMETERS_OFFSET = _SLOTS_OFFSET + 128;

        public (int workEnergy, int droneRange, int vesselRange, bool orbitalCollector, int warpDistance, bool equipWarper, int droneCount, int vesselCount) Parameters { get; private set; }
        public List<StorageItem> Storage { get; private set; }
        public List<SlotItem> Slots { get; private set; }

        public StationParameters(List<int> parameters, int storageLen, int slotsLen)
        {
            Storage = ParseStorage(parameters, storageLen);
            Slots = ParseSlots(parameters, slotsLen);
            Parameters = ParseParameters(parameters);
        }

        private List<StorageItem> ParseStorage(List<int> parameters, int storageLen)
        {
            var storage = new List<StorageItem>();
            for (int offset = _STORAGE_OFFSET; offset < _STORAGE_OFFSET + (6 * storageLen); offset += 6)
            {
                int itemId = parameters[offset];
                if (itemId == 0)
                {
                    storage.Add(null);
                }
                else
                {
                    storage.Add(new StorageItem
                    {
                        ItemId = parameters[offset],
                        LocalLogic = parameters[offset + 1],
                        RemoteLogic = parameters[offset + 2],
                        MaxCount = parameters[offset + 3]
                    });
                }
            }
            return storage;
        }

        private List<SlotItem> ParseSlots(List<int> parameters, int slotsLen)
        {
            var slots = new List<SlotItem>();
            for (int offset = _SLOTS_OFFSET; offset < _SLOTS_OFFSET + (4 * slotsLen); offset += 4)
            {
                int storageIndex = parameters[offset + 1];
                if (storageIndex == 0)
                {
                    slots.Add(null);
                }
                else
                {
                    slots.Add(new SlotItem
                    {
                        Direction = (LogisticsStationDirection)parameters[offset],
                        StorageIndex = storageIndex
                    });
                }
            }
            return slots;
        }

        private (int workEnergy, int droneRange, int vesselRange, bool orbitalCollector, int warpDistance, bool equipWarper, int droneCount, int vesselCount) ParseParameters(List<int> parameters)
        {
            return (
                parameters[_PARAMETERS_OFFSET],
                parameters[_PARAMETERS_OFFSET + 1],
                parameters[_PARAMETERS_OFFSET + 2],
                parameters[_PARAMETERS_OFFSET + 3] == 1,
                parameters[_PARAMETERS_OFFSET + 4],
                parameters[_PARAMETERS_OFFSET + 5] == 1,
                parameters[_PARAMETERS_OFFSET + 6],
                parameters[_PARAMETERS_OFFSET + 7]
            );
        }

        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
        {
            { "storage", Storage.Select(s => s?.ToDict()).ToList() },
            { "slots", Slots.Select(s => s?.ToDict()).ToList() },
            { "parameters", new
                {
                    Parameters.workEnergy,
                    Parameters.droneRange,
                    Parameters.vesselRange,
                    Parameters.orbitalCollector,
                    Parameters.warpDistance,
                    Parameters.equipWarper,
                    Parameters.droneCount,
                    Parameters.vesselCount
                }
            }
        };
        }
    }

    public class StorageItem
    {
        public int ItemId { get; set; }
        public int LocalLogic { get; set; }
        public int RemoteLogic { get; set; }
        public int MaxCount { get; set; }

        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
        {
            { "item_id", ItemId },
            { "local_logic", LocalLogic },
            { "remote_logic", RemoteLogic },
            { "max_count", MaxCount }
        };
        }
    }

    public class SlotItem
    {
        public LogisticsStationDirection Direction { get; set; }
        public int StorageIndex { get; set; }

        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
        {
            { "direction", Direction.ToString() },
            { "storage_index", StorageIndex }
        };
        }
    }

    public class BlueprintArea
    {
        private static readonly NamedStruct _BLUEPRINT_AREA = new NamedStruct(new (string, string)[]
        {
        ("b", "index"),
        ("b", "parent_index"),
        ("H", "tropic_anchor"),
        ("H", "area_segments"),
        ("H", "anchor_local_offset_x"),
        ("H", "anchor_local_offset_y"),
        ("H", "width"),
        ("H", "height")
        });

        private Dictionary<string, object> _fields;

        public BlueprintArea(Dictionary<string, object> fields)
        {
            _fields = fields;
        }

        public int Size => _BLUEPRINT_AREA.Size;

        public Dictionary<string, object> ToDict() => _fields;

        public static BlueprintArea Deserialize(byte[] data, int offset)
        {
            var fields = _BLUEPRINT_AREA.UnpackHead(data, offset);
            return new BlueprintArea(fields);
        }
    }

    public class BlueprintBuilding
    {
        private static readonly NamedStruct _BLUEPRINT_BUILDING = new NamedStruct(new (string, string)[]
        {
        ("L", "index"),
        ("b", "area_index"),
        ("f", "local_offset_x"),
        ("f", "local_offset_y"),
        ("f", "local_offset_z"),
        ("f", "local_offset_x2"),
        ("f", "local_offset_y2"),
        ("f", "local_offset_z2"),
        ("f", "yaw"),
        ("f", "yaw2"),
        ("H", "item_id"),
        ("H", "model_index"),
        ("L", "output_object_index"),
        ("L", "input_object_index"),
        ("b", "output_to_slot"),
        ("b", "input_from_slot"),
        ("b", "output_from_slot"),
        ("b", "input_to_slot"),
        ("b", "output_offset"),
        ("b", "input_offset"),
        ("H", "recipe_id"),
        ("H", "filter_id"),
        ("H", "parameter_count")
        });

        private Dictionary<string, object> _fields;
        private List<int> _parameters;

        public BlueprintBuilding(Dictionary<string, object> fields, List<int> parameters)
        {
            _fields = fields;
            _parameters = parameters;
        }

        public DysonSphereItem Item
        {
            get
            {
                try
                {
                    return (DysonSphereItem)Enum.Parse(typeof(DysonSphereItem), _fields["item_id"].ToString());
                }
                catch
                {
                    throw new Exception($"Invalid item_id: {_fields["item_id"]}");
                }
            }
        }

        public Dictionary<string, object> Data => _fields;

        public List<int> RawParameters => _parameters;

        public object Parameters
        {
            get
            {
                if (Item == DysonSphereItem.PlanetaryLogisticsStation)
                {
                    return new StationParameters(_parameters, storageLen: 3, slotsLen: 12);
                }
                else if (Item == DysonSphereItem.InterstellarLogisticsStation)
                {
                    return new StationParameters(_parameters, storageLen: 5, slotsLen: 12);
                }
                return _parameters;
            }
        }

        public int Size => _BLUEPRINT_BUILDING.Size + (_parameters.Count * 4);

        public Dictionary<string, object> ToDict()
        {
            var result = _fields.ToDictionary(entry => entry.Key, entry => entry.Value);
            if (Item != null)
            {
                result["item_id"] = Item.ToString();
            }
            result["parameters"] = Parameters is List<int> parameters ? parameters : ((StationParameters)Parameters).ToDict();
            return result;
        }

        public static BlueprintBuilding Deserialize(byte[] data, int offset)
        {
            var fields = _BLUEPRINT_BUILDING.UnpackHead(data, offset);
            offset += _BLUEPRINT_BUILDING.Size;

            var parameters = new List<int>();
            for (int i = 0; i < (int)fields["parameter_count"]; i++)
            {
                parameters.Add(BitConverter.ToInt32(data, offset + (4 * i)));
            }
            return new BlueprintBuilding(fields, parameters);
        }
    }

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
        ("B", "area_count")
        });

        private static readonly NamedStruct _BUILDING_HEADER = new NamedStruct(new (string, string)[]
        {
        ("L", "building_count")
        });

        private Dictionary<string, object> _header;
        private List<BlueprintArea> _areas;
        private List<BlueprintBuilding> _buildings;

        public BlueprintData(Dictionary<string, object> header, List<BlueprintArea> areas, List<BlueprintBuilding> buildings)
        {
            _header = header;
            _areas = areas;
            _buildings = buildings;
        }

        public List<BlueprintBuilding> Buildings => _buildings;

        public Dictionary<string, object> ToDict()
        {
            var result = _header.ToDictionary(entry => entry.Key, entry => entry.Value);
            result["areas"] = _areas.Select(area => area.ToDict()).ToList();
            result["buildings"] = _buildings.Select(building => building.ToDict()).ToList();
            return result;
        }

        public static BlueprintData Deserialize(byte[] data)
        {
            var header = _HEADER.UnpackHead(data);

            var areas = new List<BlueprintArea>();
            int offset = _HEADER.Size;
            for (int areaId = 0; areaId < (int)header["area_count"]; areaId++)
            {
                var area = BlueprintArea.Deserialize(data, offset);
                offset += area.Size;
                areas.Add(area);
            }

            var buildings = new List<BlueprintBuilding>();
            var buildingHeader = _BUILDING_HEADER.UnpackHead(data, offset);
            offset += _BUILDING_HEADER.Size;
            for (int buildingId = 0; buildingId < (int)buildingHeader["building_count"]; buildingId++)
            {
                var building = BlueprintBuilding.Deserialize(data, offset);
                offset += building.Size;
                buildings.Add(building);
            }

            return new BlueprintData(header, areas, buildings);
        }
    }

}
