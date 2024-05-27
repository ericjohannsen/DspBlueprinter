using DspBlueprinter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class SlotItem
    {
        public LogisticsStationDirection Direction { get; set; }
        public int StorageIndex { get; set; }

        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
            {
                { "direction", Direction.ToString() },
                { "storage_index", StorageIndex }
            };
        }
        public static readonly SlotItem EmptySlotItem = new SlotItem()
        {
            Direction = LogisticsStationDirection.Input,
            StorageIndex = 0,
        };
    }


}
