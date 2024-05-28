using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
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

        private Dictionary<string, (object data, int offset, int width)> _fields;

        public BlueprintArea(Dictionary<string, (object data, int offset, int width)> fields)
        {
            _fields = fields;
        }

        public int Size => _BLUEPRINT_AREA.Size;

        public Dictionary<string, (object data, int offset, int width)> ToDict() => _fields;

        public static BlueprintArea Deserialize(byte[] data, int offset)
        {
            var fields = _BLUEPRINT_AREA.UnpackHead(data, offset);
            return new BlueprintArea(fields);
        }
    }

}
