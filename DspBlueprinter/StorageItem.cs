using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class StorageItem
    {
        public int ItemId { get; set; }
        public int LocalLogic { get; set; }
        public int RemoteLogic { get; set; }
        public int MaxCount { get; set; }

        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
            {
                { "item_id", ItemId },
                { "local_logic", LocalLogic },
                { "remote_logic", RemoteLogic },
                { "max_count", MaxCount }
            };
        }
        public static readonly StorageItem EmptyItem = new StorageItem()
        {
            ItemId = 0,
            LocalLogic = 0,
            RemoteLogic = 0,
            MaxCount = 0,
        };
    }
}
