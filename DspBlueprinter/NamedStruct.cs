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

        public Dictionary<string, object> UnpackHead(byte[] data, int offset = 0)
        {
            var result = new Dictionary<string, object>();

            foreach (var (type, name) in _fields)
            {
                object value = type switch
                {
                    "b" => data[offset], // byte
                    "H" => BitConverter.ToUInt16(data, offset), // ushort (2 bytes)
                    "L" => BitConverter.ToUInt32(data, offset), // uint (4 bytes)
                    "f" => BitConverter.ToSingle(data, offset), // float (4 bytes)
                    _ => throw new InvalidOperationException($"Unknown type: {type}")
                };

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
