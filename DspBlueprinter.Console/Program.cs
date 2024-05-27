using DspBlueprinter;

Console.WriteLine("DSP Blueprinter Console");

// List blueprints:
//foreach (var bpFile in FileManager.BlueprintFiles())
//{
//    Console.WriteLine(bpFile);
//    var bp = Blueprint.ReadFromFile(bpFile);
//    Console.WriteLine(bp);
//}

// Create variants
VariantFactory.CreateVariants(VariantDescripion.BuiltinVariants());