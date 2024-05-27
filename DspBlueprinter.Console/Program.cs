using DspBlueprinter;

Console.WriteLine("DSP Blueprinter Console");

const string blueprintData = "BLUEPRINT:0,10,0,0,0,0,0,0,638522303192714946,0.10.29.22015,New%20Blueprint,\"H4sIAAAAAAAAC2NkQAWMUAxh/2dgOAFlMsKFGVapy5uDBQ9IbkNmw+RNOYwZ/kMBHz8/H0Q0j2EUDE0AAJ8sp/MkAgAA\"9158A128E539992F9F499CA4A7D8B93F";

var blueprint = Blueprint.FromBlueprintString(blueprintData, validateHash: true);

Console.WriteLine(blueprint.ToString());