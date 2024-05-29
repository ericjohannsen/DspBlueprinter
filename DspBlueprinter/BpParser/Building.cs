using System.Collections.Generic;

namespace DspBlueprintParser
{
    public class Building
    {
        public int Index { get; set; }
        public int AreaIndex { get; set; }
        public float LocalOffsetX { get; set; }
        public float LocalOffsetY { get; set; }
        public float LocalOffsetZ { get; set; }
        public float LocalOffsetX2 { get; set; }
        public float LocalOffsetY2 { get; set; }
        public float LocalOffsetZ2 { get; set; }
        public float Yaw { get; set; }
        public float Yaw2 { get; set; }
        public int ItemId { get; set; }
        public int ModelIndex { get; set; }
        public int TempOutputObjIdx { get; set; }
        public int TempInputObjIdx { get; set; }
        public int OutputToSlot { get; set; }
        public int InputFromSlot { get; set; }
        public int OutputFromSlot { get; set; }
        public int InputToSlot { get; set; }
        public int OutputOffset { get; set; }
        public int InputOffset { get; set; }
        public int RecipeId { get; set; }
        public int FilterFd { get; set; }
        public List<int> Parameters { get; set; } = new List<int>();
    }
}
