using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    using System;

    public static class DateTimeTools
    {
        // Returns the current time in the format expected by the blueprint.
        public static DateTime CSharpNow()
        {
            return DateTime.UtcNow;
        }

        // Converts a timestamp (assumed to be in seconds since epoch) to a DateTime object.
        public static DateTime CSharpToDateTime(long timestamp)
        {
            // return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            // Getting invalid data here, so using the following instead
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        // Converts a DateTime object to a timestamp (seconds since epoch).
        public static long DateTimeToCSharp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }
    }

}
