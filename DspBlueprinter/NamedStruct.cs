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
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This is an AI translation of the Python code in the NamedStruct class.
    /// </summary>
    public class NamedStruct
    {
        private readonly List<(string Type, string Name)> _fields;
        private readonly int _size;

        public NamedStruct(IEnumerable<(string Type, string Name)> fields)
        {
            _fields = fields.ToList();
            _size = CalculateSize(_fields);
        }

        public int Size => _size;

        public Dictionary<string, (object data, int offset, int width)> UnpackHead(byte[] data, int offset = 0)
        {
            var result = new Dictionary<string, (object data, int offset, int width)>();

            foreach (var (type, name) in _fields)
            {
                // For an unknown reason, every element of the array ends up typed as Single with this code:
                //object value = type switch
                //{
                //    "b" => (byte)data[offset], // byte
                //    "H" => BitConverter.ToUInt16(data, offset), // ushort (2 bytes)
                //    "L" => BitConverter.ToUInt32(data, offset), // uint (4 bytes)
                //    "f" => BitConverter.ToSingle(data, offset), // float (4 bytes)
                //    _ => throw new InvalidOperationException($"Unknown type: {type}")
                //};

                (object data, int offset, int width) value;
                if (type == "b")
                    value = (data[offset], offset, 1);
                else if (type == "H")
                    value = (BitConverter.ToUInt16(data, offset), offset, 2); // ushort (2 bytes)
                else if (type == "L")
                    value = (BitConverter.ToUInt32(data, offset), offset, 4); // uint (4 bytes)
                else if (type == "f")
                    value = (BitConverter.ToSingle(data, offset), offset, 4); // float (4 bytes)
                else
                    throw new InvalidOperationException($"Unknown type: {type}");

                result[name] = value;
                offset += GetSize(type);
            }

            return result;
        }

        private int CalculateSize(List<(string Type, string Name)> fields)
        {
            return fields.Sum(field => GetSize(field.Type));
        }

        private int GetSize(string type)
        {
            return type switch
            {
                "b" => 1, // byte
                "H" => 2, // ushort (2 bytes)
                "L" => 4, // uint (4 bytes)
                "f" => 4, // float (4 bytes)
                _ => throw new InvalidOperationException($"Unknown type: {type}")
            };
        }
    }

}
