using DspBlueprinter.Cryptography;
using DspBlueprinter.Enums;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;

namespace DspBlueprinter
{
    /// <summary>
    /// State management for the blueprint is currently messy. There's the byte[] representation, the convenience representation BlueprintData, and the string representation
    /// that is used by the game.
    /// </summary>
    public class Blueprint
    {
        public int Layout { get; private set; }
        public int Icon0 { get; private set; }
        public int Icon1 { get; private set; }
        public int Icon2 { get; private set; }
        public int Icon3 { get; private set; }
        public int Icon4 { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string GameVersion { get; private set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public byte[] Data { get; private set; }

        public Blueprint(
            string gameVersion,
            byte[] data,
            int layout = 10,
            int icon0 = 0,
            int icon1 = 0,
            int icon2 = 0,
            int icon3 = 0,
            int icon4 = 0,
            DateTime? timestamp = null,
            string shortDesc = "Short description",
            string longDesc = "Long description")
        {
            Layout = layout;
            Icon0 = icon0;
            Icon1 = icon1;
            Icon2 = icon2;
            Icon3 = icon3;
            Icon4 = icon4;
            Timestamp = timestamp ?? DateTime.Now;
            GameVersion = gameVersion;
            ShortDesc = shortDesc;
            LongDesc = longDesc;
            Data = data;
        }

        /// <summary>
        /// Get the Item ID of a building (e.g. SorterMkI)
        /// </summary>
        /// <param name="buildingIdx"></param>
        public DysonSphereItem GetBuildingItemId(int buildingIdx)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Game Version: {GameVersion}");
            sb.AppendLine($"Timestamp   : {Timestamp.ToLongTimeString()}");
            sb.AppendLine($"Short Descr : {ShortDesc}");
            sb.AppendLine($"Long Descr  : {LongDesc}");
            sb.AppendLine("Buildings:");
            foreach (var building in DecodedData.Buildings)
            {
                sb.AppendLine(building.Item.ToString());
            }
            return sb.ToString();
        }

        public Blueprint Clone() => FromBlueprintString(Serialize(), true);

        public BlueprintData DecodedData => BlueprintData.Deserialize(Data);

        public static Blueprint FromBlueprintString(string bpString, bool validateHash = true)
        {
            if (validateHash)
            {
                int index = bpString.LastIndexOf("\"", StringComparison.Ordinal);
                string hashedData = bpString[..index];
                string refValue = bpString[(index + 1)..].ToLower().Trim();
                string hash = new DspMd5F().GetHash(hashedData);


                if (refValue != hash)
                {
                    throw new InvalidHashValueException("Blueprint string has invalid hash value.");
                }
            }

            if (!bpString.StartsWith("BLUEPRINT:"))
            {
                throw new FormatException("Invalid blueprint string format.");
            }

            string[] components = bpString[10..].Split(',');

            if (components.Length != 12)
            {
                throw new FormatException("Invalid blueprint string format.");
            }

            (int fixed0_1, int layout, int icon0, int icon1, int icon2, int icon3, int icon4, int fixed0_2, long timestamp, string gameVersion, string shortDesc, string b64dataHash) =
                (int.Parse(components[0]), int.Parse(components[1]), int.Parse(components[2]), int.Parse(components[3]), int.Parse(components[4]), int.Parse(components[5]),
                 int.Parse(components[6]), int.Parse(components[7]), long.Parse(components[8]), components[9], HttpUtility.UrlDecode(components[10]), components[11]);

            if (fixed0_1 != 0 || fixed0_2 != 0)
            {
                throw new FormatException("Invalid fixed value in blueprint string.");
            }

            DateTime decodedTimestamp = new DateTime(timestamp);

            string[] b64dataHashSplit = b64dataHash.Split('\"');

            if (b64dataHashSplit.Length != 3)
            {
                throw new FormatException("Invalid blueprint string format.");
            }

            (string longDesc, string b64data, string hashValue) = (HttpUtility.UrlDecode(b64dataHashSplit[0]), b64dataHashSplit[1], b64dataHashSplit[2]);

            byte[] compressedData = Convert.FromBase64String(b64data);
            byte[] data = Decompress(compressedData);

            return new Blueprint(gameVersion, data, layout, icon0, icon1, icon2, icon3, icon4, decodedTimestamp, shortDesc, longDesc);
        }

        public string Serialize()
        {
            byte[] compressedData = Compress(Data);
            string b64Data = Convert.ToBase64String(compressedData);

            var components = new[]
            {
                "0",
                Layout.ToString(),
                Icon0.ToString(),
                Icon1.ToString(),
                Icon2.ToString(),
                Icon3.ToString(),
                Icon4.ToString(),
                "0",
                Timestamp.Ticks.ToString(),
                GameVersion,
                HttpUtility.UrlEncode(ShortDesc)
            };

            string header = "BLUEPRINT:" + string.Join(",", components);
            string hashedData = header + ",\"" + b64Data;
            string hashValue = new DspMd5F().GetHash(hashedData);

            return hashedData + "\"" + hashValue.ToUpper();
        }

        public Dictionary<string, object> ToDict()
        {
            return new()
            {
                { "icon", new { layout = Layout, images = new[] { Icon0, Icon1, Icon2, Icon3, Icon4 } } },
                { "timestamp", Timestamp.ToString("yyyy-MM-dd HH:mm:ss") },
                { "game_version", GameVersion },
                { "short_desc", ShortDesc },
                { "data", DecodedData.ToDict() }
            };
        }

        public static Blueprint ReadFromFile(string filename, bool validateHash = true)
        {
            string bpString = File.ReadAllText(filename);
            return FromBlueprintString(bpString, validateHash);
        }

        public void WriteToFile(string filename)
        {
            string serialized = Serialize();
            File.WriteAllText(filename, serialized);
        }

        private static byte[] Compress(byte[] data)
        {
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        private static byte[] Decompress(byte[] data)
        {
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            {
                gzip.CopyTo(output);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Data is always little endian.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="replaceWith"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal void WriteDataInt32(int startIndex, int replaceWith)
        {
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }

            if (startIndex < 0 || startIndex + 4 > Data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range or does not provide enough space for an Int32 value.");
            }
#if DEBUG
            var dbg = ReadDataInt32(Data, startIndex);
            throw new Exception("Index appears to be incorrect because the data isn't what we expect.");
#endif

            Data[startIndex] = (byte)(replaceWith & 0xFF);
            Data[startIndex + 1] = (byte)((replaceWith >> 8) & 0xFF);
            Data[startIndex + 2] = (byte)((replaceWith >> 16) & 0xFF);
            Data[startIndex + 3] = (byte)((replaceWith >> 24) & 0xFF);
        }

        public static int ReadDataInt32(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException(nameof(byteArray));
            }

            if (startIndex < 0 || startIndex + 4 > byteArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range or does not provide enough space to read an Int32 value.");
            }

            return byteArray[startIndex] |
                   (byteArray[startIndex + 1] << 8) |
                   (byteArray[startIndex + 2] << 16) |
                   (byteArray[startIndex + 3] << 24);
        }
    }
}
