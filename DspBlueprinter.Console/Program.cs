using DspBlueprinter;
using DspBlueprinter.Diagnosis;

Console.WriteLine("DSP Blueprinter Console");


var smelterBp = "BLUEPRINT:0,10,0,0,0,0,0,0,638525923225190441,0.10.30.22243,New%20Blueprint,\"H4sIAAAAAAAAC4WMMQ6AMAwDnXaAlZHPFB7KO5g78ikwJUQhLGAp0slyTgBkPOnaibGAwGa16GwhqcXOadZNHdfIQYWjL6DlbpLbL5HSgOrPkb9FeIlSHMfnf1F20QnldBaxCgEAAA==\"0DE6C18708D5D03C8095B57E95C16487";
var bp = Blueprint.FromBlueprintString(smelterBp);
var decodedData = bp.DecodedData;

Console.WriteLine("DONE");

// List blueprints:
//foreach (var bpFile in FileManager.BlueprintFiles())
//{
//    Console.WriteLine(bpFile);
//    var bp = Blueprint.ReadFromFile(bpFile);
//    Console.WriteLine(bp);
//}

// Create variants
VariantFactory.CreateVariants(VariantDescripion.BuiltinVariants());

//var mk1Sorter = "BLUEPRINT:0,10,0,0,0,0,0,0,638525183453463859,0.10.29.22015,New%20Blueprint,\"H4sIAAAAAAAAC2NkYGBghGIQYAZiJiibkeE/A8MJqDATAwtUmOH9///2IZJ/rBqNz+6cXOVr/+DbZMvCZTq7FRjanV/8anO+za4JN4flP8QYkGlwe7gZGBzkGRrsGQ9IbkNmczKYOIPwRXZlsOb/QMCIcBvEYSANq9TlzRgL0rYjs7mAGrmgmv9DAZLHwC4CA5gGZPacZ5NNQdiUwxiumY+fnw+iI49hFAxNAAB38egL3wIAAA==\"49DE955DE7C9A5BE00AEAEBD23C32B12";
//var mk2Sorter = "BLUEPRINT:0,10,0,0,0,0,0,0,638525184202331261,0.10.29.22015,New%20Blueprint,\"H4sIAAAAAAAAC2NkYGBghGIQYAZiJiibkeE/A8MJqDATAwtUmOH9///2IZJ/rBqNz+6cXOVr/+DbZMvCZTq7FRjanV/8anO+w64FN4flP8QYkGlwe7gZGBzkGRrsGQ9IbkNmczKYOIPwRXZlsOb/QMCIcBvEYSANq9TlzRgL0rYjs7mAGrmgmv9DAZLHwC4CA5gGZPacZ5NNQdiUwxiumY+fnw+iI49hFAxNAAAMIKdC3wIAAA==\"337ABA0990AA9795705C3B742A4BF362";
//var mk3Sorter = "BLUEPRINT:0,10,0,0,0,0,0,0,638525184405155449,0.10.29.22015,New%20Blueprint,\"H4sIAAAAAAAAC2NkYGBghGIQYAZiJiibkeE/A8MJqDATAwtUmOH9///2IZJ/rBqNz+6cXOVr/+DbZMvCZTq7FRjanV/8anO+y64NN4flP8QYkGlwe7gZGBzkGRrsGQ9IbkNmczKYOIPwRXZlsOb/QMCIcBvEYSANq9TlzRgL0rYjs7mAGrmgmv9DAZLHwC4CA5gGZPacZ5NNQdiUwxiumY+fnw+iI49hFAxNAACDpJ9H3wIAAA==\"C1C37E0DAAE01D9452403A0FF0CB4F46";
//var pileSorter = "BLUEPRINT:0,10,0,0,0,0,0,0,638525184676989380,0.10.29.22015,New%20Blueprint,\"H4sIAAAAAAAAC2NkYGBghGIQYAZiJiibkeE/A8MJqDATAwtUmOH9///2IZJ/rBqNz+6cXOVr/+DbZMvCZTq7FRjanV/8anO+x/6YEWYOy3+IMSDT4PZwMzA4yDM02DMekNyGzOZkMHEG4YvsymDN/4GAEeE2iMNAGlapy5sxFqRtR2ZzATVyQTX/hwIkj4F9BgYwDcjsOc8mm4KwKYcxXDMfPz8fREcewygYmgAANfoW998CAAA=\"48827C1C91DA2B32D2B7440B940E99B3"; 

//var bp1 = Blueprint.FromBlueprintString(mk1Sorter);
//var bp2 = Blueprint.FromBlueprintString(mk2Sorter);
//var bp3 = Blueprint.FromBlueprintString(mk3Sorter);
//var bp4 = Blueprint.FromBlueprintString(pileSorter);

//// ofs 86 model ID
//BinaryUtils.CompareArraysInt16(84, bp1.Data, bp2.Data, bp3.Data, bp4.Data);

// BinaryUtils.CompareArrays(bp1.Data, bp2.Data);
//Console.WriteLine("DONE");
//Console.ReadKey();