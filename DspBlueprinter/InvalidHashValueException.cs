using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class InvalidHashValueException : Exception
    {
        public InvalidHashValueException(string message) : base(message)
        {
        }
    }
}
