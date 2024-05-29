namespace DspBlueprintParser
{
    public class BinaryReader
    {
        private readonly byte[] _data;
        private int _position;

        public BinaryReader(byte[] data)
        {
            _data = data;
            _position = 0;
        }

        public int ReadInt32()
        {
            return GetInteger(4, BitConverter.ToInt32);
        }

        public short ReadInt16()
        {
            return GetInteger(2, BitConverter.ToInt16);
        }

        public byte ReadInt8()
        {
            return (byte)GetInteger(1, b => b[0]);
        }

        public float ReadSingle()
        {
            return GetInteger(4, BitConverter.ToSingle);
        }

        private T GetInteger<T>(int byteCount, Func<byte[], int, T> converter)
        {
            var value = converter(_data, _position);
            _position += byteCount;
            return value;
        }
    }
}
