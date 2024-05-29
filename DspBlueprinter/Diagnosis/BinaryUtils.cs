using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter.Diagnosis
{
    public class BinaryUtils
    {
        static public void CompareArrays<T>(T[] a, T[] b)
        {
            if (a == null || b == null)
            {
                Console.WriteLine("ERROR: Array cannot be null.");
                return;
            }
            var len = Math.Min(a.Length, b.Length);
            if (a.Length != b.Length)
                Console.WriteLine($"WARNING: Different lengths ({a.Length} vs {b.Length}. Comparing {len} elements.");
            for (int i =  0; i < len; i++)
            {
                if (!a[i].Equals(b[i]))
                    Console.WriteLine($"MISMATCH at index {i} - {a[i]} vs {b[i]}");
            }
        }
        static public void CompareArraysInt16(int offset, params byte[][] arrays)
        {
            int i = 0;
            foreach (var array in arrays)
            {
                if (array.Length >= offset + 2)
                {
                    short value = BitConverter.ToInt16(array, offset);
                    Console.WriteLine($"Array {i} at offset {offset}: {value}");
                }
                else
                {
                    Console.WriteLine($"Array too short to read Int16 at offset {offset}");
                }
                i++;
            }
        }
    }
}
