using System;
using System.Collections.Generic;

namespace DspBlueprintParser
{
    public class Parser
    {
        private const long SECONDS_AT_EPOCH = 62135596800;
        private readonly DataSections _dataSections;

        public Parser(string strBlueprint)
        {
            _dataSections = new DataSections(strBlueprint);
        }

        public BlueprintData Blueprint => _blueprint ??= ParseBlueprint();

        private BlueprintData _blueprint;

        private BlueprintData ParseBlueprint()
        {
            var blueprint = new BlueprintData();
            ParseMetadata(blueprint);
            ParseAreas(blueprint);
            ParseBuildings(blueprint);
            return blueprint;
        }

        private string[] HeaderSegments => _dataSections.HeaderSegments;

        private BinaryReader Reader => _reader ??= new BinaryReader(_dataSections.DecompressedBody);
        private BinaryReader _reader;

        private static DateTime TicksToEpoch(long ticks)
        {
            var seconds = ticks / 10000000;
            return DateTimeOffset.FromUnixTimeSeconds(seconds - SECONDS_AT_EPOCH).DateTime;
        }

        private void ParseMetadata(BlueprintData blueprint)
        {
            blueprint.IconLayout = int.Parse(HeaderSegments[1]);
            blueprint.Icon0 = int.Parse(HeaderSegments[2]);
            blueprint.Icon1 = int.Parse(HeaderSegments[3]);
            blueprint.Icon2 = int.Parse(HeaderSegments[4]);
            blueprint.Icon3 = int.Parse(HeaderSegments[5]);
            blueprint.Icon4 = int.Parse(HeaderSegments[6]);

            blueprint.Time = TicksToEpoch(long.Parse(HeaderSegments[8]));
            blueprint.GameVersion = HeaderSegments[9];
            blueprint.ShortDescription = Uri.UnescapeDataString(HeaderSegments[10]);
            blueprint.Description = Uri.UnescapeDataString(HeaderSegments[11]);

            blueprint.Version = Reader.ReadInt32();
            blueprint.CursorOffsetX = Reader.ReadInt32();
            blueprint.CursorOffsetY = Reader.ReadInt32();
            blueprint.CursorTargetArea = Reader.ReadInt32();
            blueprint.DragBoxSizeX = Reader.ReadInt32();
            blueprint.DragBoxSizeY = Reader.ReadInt32();
            blueprint.PrimaryAreaIdx = Reader.ReadInt32();
        }

        private void ParseAreas(BlueprintData blueprint)
        {
            var areaCount = Reader.ReadInt8();
            for (var i = 0; i < areaCount; i++)
            {
                var area = new Area
                {
                    Index = Reader.ReadInt8(),
                    ParentIndex = Reader.ReadInt8(),
                    TropicAnchor = Reader.ReadInt16(),
                    AreaSegments = Reader.ReadInt16(),
                    AnchorLocalOffsetX = Reader.ReadInt16(),
                    AnchorLocalOffsetY = Reader.ReadInt16(),
                    Width = Reader.ReadInt16(),
                    Height = Reader.ReadInt16()
                };
                blueprint.Areas.Add(area);
            }
        }

        private void ParseBuildings(BlueprintData blueprint)
        {
            var buildingCount = Reader.ReadInt32();
            for (var i = 0; i < buildingCount; i++)
            {
                var building = new Building
                {
                    Index = Reader.ReadInt32(),
                    AreaIndex = Reader.ReadInt8(),
                    LocalOffsetX = Reader.ReadSingle(),
                    LocalOffsetY = Reader.ReadSingle(),
                    LocalOffsetZ = Reader.ReadSingle(),
                    LocalOffsetX2 = Reader.ReadSingle(),
                    LocalOffsetY2 = Reader.ReadSingle(),
                    LocalOffsetZ2 = Reader.ReadSingle(),
                    Yaw = Reader.ReadSingle(),
                    Yaw2 = Reader.ReadSingle(),
                    ItemId = Reader.ReadInt16(),
                    ModelIndex = Reader.ReadInt16(),
                    TempOutputObjIdx = Reader.ReadInt32(),
                    TempInputObjIdx = Reader.ReadInt32(),
                    OutputToSlot = Reader.ReadInt8(),
                    InputFromSlot = Reader.ReadInt8(),
                    OutputFromSlot = Reader.ReadInt8(),
                    InputToSlot = Reader.ReadInt8(),
                    OutputOffset = Reader.ReadInt8(),
                    InputOffset = Reader.ReadInt8(),
                    RecipeId = Reader.ReadInt16(),
                    FilterFd = Reader.ReadInt16()
                };

                var paramCount = Reader.ReadInt16();
                for (var j = 0; j < paramCount; j++)
                {
                    building.Parameters.Add(Reader.ReadInt32());
                }

                blueprint.Buildings.Add(building);
            }
        }
    }
}
