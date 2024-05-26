using System.Security.Cryptography;
using DspBlueprinter;
using DspBlueprinter.Cryptography;

namespace DspBlueprinter.Tests
{
    [TestClass]
    public class BlueprintMd5Test
    {
        [DataTestMethod]
        [DataRow("", "d41d8cd98f00b204e9800998ecf8427e")]
        [DataRow("a", "0cc175b9c0f1b6a831c399e269772661")]
        [DataRow("abc", "900150983cd24fb0d6963f7d28e17f72")]
        [DataRow("message digest", "f96b697d7cb7938d525a2f31aaf161d0")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "c3fcd3d76192e4007dfb496cca67e13b")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "d174ab98d277d9f5a5611c2c9f419d9f")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57edf4a22be3c955ac49da2e2107b67a")]
        public void MD5(string input, string expectedHash)
        {
            // Act
            string actualHash = new DspMd5().GetHash(input);

            // Assert
            Assert.AreEqual(expectedHash, actualHash);
        }

        [DataTestMethod]
        [DataRow("", "84d1ce3bd68f49ab26eb0f96416617cf")]
        [DataRow("a", "f10bddaecb62e5a92433757867ee06db")]
        [DataRow("abcd", "fa27c78b6ec31559f0e760ce3f2b03f6")]
        [DataRow("Why are you doing this, Youthcat Studio?", "13424e12890a3f50a1f8567c464fff8c")]
        public void MD5F(string input, string expectedHash)
        {
            // Act
            string actualHash = new DspMd5F().GetHash(input);

            // Assert
            Assert.AreEqual(expectedHash, actualHash);
        }
    }
}