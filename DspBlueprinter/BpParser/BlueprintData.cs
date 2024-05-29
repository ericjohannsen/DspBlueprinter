using System;
using System.Collections.Generic;

namespace DspBlueprintParser
{
    public class BlueprintData
    {
        public DateTime Time { get; set; }
        public string GameVersion { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int IconLayout { get; set; }
        public int Icon0 { get; set; }
        public int Icon1 { get; set; }
        public int Icon2 { get; set; }
        public int Icon3 { get; set; }
        public int Icon4 { get; set; }
        public int CursorOffsetX { get; set; }
        public int CursorOffsetY { get; set; }
        public int CursorTargetArea { get; set; }
        public int DragBoxSizeX { get; set; }
        public int DragBoxSizeY { get; set; }
        public int PrimaryAreaIdx { get; set; }
        public List<Area> Areas { get; set; } = new List<Area>();
        public List<Building> Buildings { get; set; } = new List<Building>();
        public int Version { get; set; }
    }
}
