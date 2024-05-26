using System.Security.Cryptography;
using DspBlueprinter;
using DspBlueprinter.Cryptography;

namespace DspBlueprinter.Tests
{
    [TestClass]
    public class BlueprintTest
    {
        [DataTestMethod]
        [DataRow("BLUEPRINT:0,10,0,0,0,0,0,0,638522303192714946,0.10.29.22015,New%20Blueprint,\"H4sIAAAAAAAAC2NkQAWMUAxh/2dgOAFlMsKFGVapy5uDBQ9IbkNmw+RNOYwZ/kMBHz8/H0Q0j2EUDE0AAJ8sp/MkAgAA\"9158A128E539992F9F499CA4A7D8B93F")]
        public void Storage(string blueprintData)
        {
            // Act
            var blueprint = Blueprint.FromBlueprintString(blueprintData, validateHash: true);

            // Assert
            
        }
    }
}