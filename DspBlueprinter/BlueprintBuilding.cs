using DspBlueprinter.Diagnosis;
using DspBlueprinter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
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
                    return (DysonSphereItem)Enum.Parse(typeof(DysonSphereItem), _fields["item_id"]?.ToString() ?? "Unknown");
                }
                catch
                {
                    throw new Exception($"Invalid item_id: {_fields["item_id"]}");
                }
            }
        }

        public void ChangeBuilding(DysonSphereItem updatedBuilding) =>
            _fields["item_id"] = (int)updatedBuilding;

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

            result["item_id"] = Item.ToString();

            result["parameters"] = Parameters is List<int> parameters ? parameters : ((StationParameters)Parameters).ToDict();
            return result;
        }

        public static BlueprintBuilding Deserialize(byte[] data, int offset)
        {
            var fields = _BLUEPRINT_BUILDING.UnpackHead(data, offset);
            offset += _BLUEPRINT_BUILDING.Size;

            var parameters = new List<int>();
            for (ushort i = 0; i < (ushort)fields["parameter_count"]; i++)
            {
                parameters.Add(BitConverter.ToInt32(data, offset + (4 * i)));
            }
            return new BlueprintBuilding(fields, parameters);
        }
        public override string ToString() => Item.ToString();

    }

}
