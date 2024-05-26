/*
    DspBlueprinter - Dyson Sphere Program Blueprinter
    Copyright (C) 2024 Eric Johannsen

    This file is part of DspBlueprinter.

    DspBlueprinter is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; this program is ONLY licensed under
    version 3 of the License, later versions are explicitly excluded.

    DspBlueprinter is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    Eric Johannsen <eric@johannsen.us>

    This file based on the outstanding re-enginering work of Johannes Bauer 
    https://github.com/johndoe31415/dspbptk/blob/master/MD5.py
*/

using System.Security.Cryptography;
using System.Text;

namespace DspBlueprinter.Cryptography
{
    /// <summary>
    /// This is a standard MD5 implementation, but with the ability to override some of the constants used in the algorithm.
    /// </summary>
    public class DspMd5
    {
        // Constants for MD5Transform routine.
        private const int S11 = 7;
        private const int S12 = 12;
        private const int S13 = 17;
        private const int S14 = 22;
        private const int S21 = 5;
        private const int S22 = 9;
        private const int S23 = 14;
        private const int S24 = 20;
        private const int S31 = 4;
        private const int S32 = 11;
        private const int S33 = 16;
        private const int S34 = 23;
        private const int S41 = 6;
        private const int S42 = 10;
        private const int S43 = 15;
        private const int S44 = 21;

        private readonly uint[] state = new uint[4]; // state (ABCD)
        private readonly uint[] count = new uint[2]; // number of bits, modulo 2^64 (lsb first)
        private readonly byte[] buffer = new byte[64]; // input buffer

        private static readonly byte[] PADDING = {
        0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

        public DspMd5()
        {
            Initialize();
        }

        virtual protected uint S0() => 0x67452301;
        virtual protected uint S1() => 0xefcdab89;
        virtual protected uint S2() => 0x98badcfe;
        virtual protected uint S3() => 0x10325476;

        // F, G, H and I are basic MD5 functions.
        private static uint F(uint x, uint y, uint z) => (x & y) | (~x & z);
        private static uint G(uint x, uint y, uint z) => (x & z) | (y & ~z);
        private static uint H(uint x, uint y, uint z) => x ^ y ^ z;
        private static uint I(uint x, uint y, uint z) => y ^ (x | ~z);

        // Rotates x left n bits.
        private static uint RotateLeft(uint x, int n) => (x << n) | (x >> (32 - n));

        // FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4.
        // Rotation is separate from addition to prevent recomputation.
        private static void FF(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            // DUMP(a, b, c, d, x, s, ac);
            a += F(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
            // DUMP(a);
        }

        private static void GG(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += G(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }

        private static void HH(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += H(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }

        private static void II(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
        {
            a += I(b, c, d) + x + ac;
            a = RotateLeft(a, s) + b;
        }

#if DEBUG
        static int step = 1;
        static void DUMP(uint a)
            => Console.WriteLine($"State: {a:x}");

        static void DUMP(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
            => Console.WriteLine($"Step {step++}: {a:x} {b:x} {c:x} {d:x} {x:x} {s:x} {ac:x}");
#else
        static void DUMP(uint a) {}

        static void DUMP(uint a, uint b, uint c, uint d, uint x, int s, uint ac) {}
#endif

        // MD5 initialization. Begins an MD5 operation, writing a new context.
        public virtual void Initialize()
        {
            count[0] = count[1] = 0;

            // Load magic initialization constants. DSP has different values, so get them from a virtual property.
            state[0] = S0();
            state[1] = S1();
            state[2] = S2();
            state[3] = S3();
        }

        // MD5 block update operation. Continues an MD5 message-digest operation, processing another message block
        public void Update(byte[] input, int offset, int updateCount)
        {
            var inputLen = updateCount;
            var index = (int)((count[0] >> 3) & 0x3F);

            // Update number of bits
            if ((count[0] += (uint)(inputLen << 3)) < (uint)(inputLen << 3))
                count[1]++;
            count[1] += (uint)(inputLen >> 29);

            var partLen = 64 - index;

            int i;
            if (inputLen >= partLen)
            {
                Buffer.BlockCopy(input, offset, buffer, index, partLen);
                Transform(buffer, 0);

                for (i = partLen; i + 63 < inputLen; i += 64)
                    Transform(input, offset + i);
                index = 0;
            }
            else
                i = 0;

            // Buffer remaining input
            Buffer.BlockCopy(input, offset + i, buffer, index, inputLen - i);
        }

        // MD5 finalization. Ends an MD5 message-digest operation, writing the message digest and zeroizing the context.
        public byte[] FinalizeHash()
        {
            var bits = Encode(count, 8);

            // Pad out to 56 mod 64.
            var index = (int)((count[0] >> 3) & 0x3f);
            var padLen = (index < 56) ? (56 - index) : (120 - index);
            Update(PADDING, 0, padLen);

            // Append length (before padding)
            Update(bits, 0, 8);

            // Store state in digest
            var digest = Encode(state, 16);

            // Zeroize sensitive information.
            Initialize();

            return digest;
        }

        // DSP uses different constants in some rounds, so we need to override them.
        protected virtual uint OP2() => 0xe8c7b756;
        protected virtual uint OP7() => 0xa8304613;
        protected virtual uint OP13() => 0x6b901122;
        protected virtual uint OP16() => 0x49b40821;
        protected virtual uint OP20() => 0xe9b6c7aa;
        protected virtual uint OP22() => 0x02441453;
        protected virtual uint OP25() => 0x21e1cde6;
        protected virtual uint OP28() => 0x455a14ed;
        protected virtual void Transform(byte[] block, int offset)
        {
            uint a = state[0], b = state[1], c = state[2], d = state[3];
            var x = Decode(block, offset, 64);

            // Round 1
            FF(ref a, b, c, d, x[0], S11, 0xd76aa478); // 1
            FF(ref d, a, b, c, x[1], S12, OP2()); // 2
            FF(ref c, d, a, b, x[2], S13, 0x242070db); // 3
            FF(ref b, c, d, a, x[3], S14, 0xc1bdceee); // 4
            FF(ref a, b, c, d, x[4], S11, 0xf57c0faf); // 5
            FF(ref d, a, b, c, x[5], S12, 0x4787c62a); // 6
            FF(ref c, d, a, b, x[6], S13, OP7()); // 7
            FF(ref b, c, d, a, x[7], S14, 0xfd469501); // 8
            FF(ref a, b, c, d, x[8], S11, 0x698098d8); // 9
            FF(ref d, a, b, c, x[9], S12, 0x8b44f7af); // 10
            FF(ref c, d, a, b, x[10], S13, 0xffff5bb1); // 11
            FF(ref b, c, d, a, x[11], S14, 0x895cd7be); // 12
            FF(ref a, b, c, d, x[12], S11, OP13()); // 13
            FF(ref d, a, b, c, x[13], S12, 0xfd987193); // 14
            FF(ref c, d, a, b, x[14], S13, 0xa679438e); // 15
            FF(ref b, c, d, a, x[15], S14, OP16()); // 16

            // Round 2
            GG(ref a, b, c, d, x[1], S21, 0xf61e2562); // 17
            GG(ref d, a, b, c, x[6], S22, 0xc040b340); // 18
            GG(ref c, d, a, b, x[11], S23, 0x265e5a51); // 19
            GG(ref b, c, d, a, x[0], S24, OP20()); // 20
            GG(ref a, b, c, d, x[5], S21, 0xd62f105d); // 21
            GG(ref d, a, b, c, x[10], S22, OP22()); // 22
            GG(ref c, d, a, b, x[15], S23, 0xd8a1e681); // 23
            GG(ref b, c, d, a, x[4], S24, 0xe7d3fbc8); // 24
            GG(ref a, b, c, d, x[9], S21, OP25()); // 25
            GG(ref d, a, b, c, x[14], S22, 0xc33707d6); // 26
            GG(ref c, d, a, b, x[3], S23, 0xf4d50d87); // 27
            GG(ref b, c, d, a, x[8], S24, OP28()); // 28
            GG(ref a, b, c, d, x[13], S21, 0xa9e3e905); // 29
            GG(ref d, a, b, c, x[2], S22, 0xfcefa3f8); // 30
            GG(ref c, d, a, b, x[7], S23, 0x676f02d9); // 31
            GG(ref b, c, d, a, x[12], S24, 0x8d2a4c8a); // 32

            // Round 3
            HH(ref a, b, c, d, x[5], S31, 0xfffa3942); // 33
            HH(ref d, a, b, c, x[8], S32, 0x8771f681); // 34
            HH(ref c, d, a, b, x[11], S33, 0x6d9d6122); // 35
            HH(ref b, c, d, a, x[14], S34, 0xfde5380c); // 36
            HH(ref a, b, c, d, x[1], S31, 0xa4beea44); // 37
            HH(ref d, a, b, c, x[4], S32, 0x4bdecfa9); // 38
            HH(ref c, d, a, b, x[7], S33, 0xf6bb4b60); // 39
            HH(ref b, c, d, a, x[10], S34, 0xbebfbc70); // 40
            HH(ref a, b, c, d, x[13], S31, 0x289b7ec6); // 41
            HH(ref d, a, b, c, x[0], S32, 0xeaa127fa); // 42
            HH(ref c, d, a, b, x[3], S33, 0xd4ef3085); // 43
            HH(ref b, c, d, a, x[6], S34, 0x04881d05); // 44
            HH(ref a, b, c, d, x[9], S31, 0xd9d4d039); // 45
            HH(ref d, a, b, c, x[12], S32, 0xe6db99e5); // 46
            HH(ref c, d, a, b, x[15], S33, 0x1fa27cf8); // 47
            HH(ref b, c, d, a, x[2], S34, 0xc4ac5665); // 48

            // Round 4
            II(ref a, b, c, d, x[0], S41, 0xf4292244); // 49
            II(ref d, a, b, c, x[7], S42, 0x432aff97); // 50
            II(ref c, d, a, b, x[14], S43, 0xab9423a7); // 51
            II(ref b, c, d, a, x[5], S44, 0xfc93a039); // 52
            II(ref a, b, c, d, x[12], S41, 0x655b59c3); // 53
            II(ref d, a, b, c, x[3], S42, 0x8f0ccc92); // 54
            II(ref c, d, a, b, x[10], S43, 0xffeff47d); // 55
            II(ref b, c, d, a, x[1], S44, 0x85845dd1); // 56
            II(ref a, b, c, d, x[8], S41, 0x6fa87e4f); // 57
            II(ref d, a, b, c, x[15], S42, 0xfe2ce6e0); // 58
            II(ref c, d, a, b, x[6], S43, 0xa3014314); // 59
            II(ref b, c, d, a, x[13], S44, 0x4e0811a1); // 60
            II(ref a, b, c, d, x[4], S41, 0xf7537e82); // 61
            II(ref d, a, b, c, x[11], S42, 0xbd3af235); // 62
            II(ref c, d, a, b, x[2], S43, 0x2ad7d2bb); // 63
            II(ref b, c, d, a, x[9], S44, 0xeb86d391); // 64

            state[0] += a;
            state[1] += b;
            state[2] += c;
            state[3] += d;

            // Zeroize sensitive information.
            Array.Clear(x, 0, x.Length);
        }

        private static uint[] Decode(byte[] input, int offset, int len)
        {
            var output = new uint[len / 4];
            for (int i = 0, j = offset; i < output.Length; i++, j += 4)
            {
                output[i] = (uint)(input[j] | (input[j + 1] << 8) | (input[j + 2] << 16) | (input[j + 3] << 24));
            }
            return output;
        }

        private static byte[] Encode(uint[] input, int len)
        {
            var output = new byte[len];
            for (int i = 0, j = 0; j < len; i++, j += 4)
            {
                output[j] = (byte)(input[i] & 0xff);
                output[j + 1] = (byte)((input[i] >> 8) & 0xff);
                output[j + 2] = (byte)((input[i] >> 16) & 0xff);
                output[j + 3] = (byte)((input[i] >> 24) & 0xff);
            }
            return output;
        }

        public virtual string GetHash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            Update(data, 0, data.Length);
            var hash = FinalizeHash();
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}