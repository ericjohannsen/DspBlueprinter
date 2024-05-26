using DspBlueprinter.Cryptography;

Console.WriteLine("DSP Blueprinter Console");

bool original = false;
if (original)
{
    Console.WriteLine("Original MD5");
    var hash = new DspMd5().GetHash("a");
    Console.WriteLine("MD5  " + hash);
}
else
{
    Console.WriteLine("Variant MD5F");
    var hash = new DspMd5F().GetHash("a");
    Console.WriteLine("MD5F " + hash);

}