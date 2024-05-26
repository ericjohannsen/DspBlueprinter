using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter.Cryptography
{
    public class DspMd5F : DspMd5
    {
        /*
            https://github.com/johndoe31415/dspbptk/blob/bb11f9b90109e738334f5453d92f34152e614897/MD5.py#L146
			int.from_bytes(bytes.fromhex("01 23 45 67"), byteorder = "little"),
			int.from_bytes(bytes.fromhex("89 ab dc ef"), byteorder = "little"),
			int.from_bytes(bytes.fromhex("fe dc ba 98"), byteorder = "little"),
			int.from_bytes(bytes.fromhex("46 57 32 10"), byteorder = "little"),       
         */
        protected override uint S0() => 0x67452301; 
        protected override uint S1() => 0xefdcab89;
        protected override uint S2() => 0x98badcfe;
        protected override uint S3() => 0x10325746;

        /*
            https://github.com/johndoe31415/dspbptk/blob/bb11f9b90109e738334f5453d92f34152e614897/MD5.py#L116C1-L124C94
         		Variant.MD5F: {
			1: _RoundOp(a = 3, b = 0, c = 1, d = 2, k =  1, s = 12, i =  2, T = 0xe8d7b756, op = _f),
			6: _RoundOp(a = 2, b = 3, c = 0, d = 1, k =  6, s = 17, i =  7, T = 0xa8304623, op = _f),
			12: _RoundOp(a = 0, b = 1, c = 2, d = 3, k = 12, s =  7, i = 13, T = 0x6b9f1122, op = _f),
			15: _RoundOp(a = 1, b = 2, c = 3, d = 0, k = 15, s = 22, i = 16, T = 0x39b40821, op = _f),
			19: _RoundOp(a = 1, b = 2, c = 3, d = 0, k =  0, s = 20, i = 20, T = 0xc9b6c7aa, op = _g),
			21: _RoundOp(a = 3, b = 0, c = 1, d = 2, k = 10, s =  9, i = 22, T = 0x2443453, op = _g),
			24: _RoundOp(a = 0, b = 1, c = 2, d = 3, k =  9, s =  5, i = 25, T = 0x21f1cde6, op = _g),
			27: _RoundOp(a = 1, b = 2, c = 3, d = 0, k =  8, s = 20, i = 28, T = 0x475a14ed, op = _g),
         */
        protected override uint OP2() => 0xe8d7b756;
        protected override uint OP7() => 0xa8304623;
        protected override uint OP13() => 0x6b9f1122;
        protected override uint OP16() => 0x39b40821;
        protected override uint OP20() => 0xc9b6c7aa;
        protected override uint OP22() => 0x2443453;
        protected override uint OP25() => 0x21f1cde6;
        protected override uint OP28() => 0x475a14ed;
    }
}
