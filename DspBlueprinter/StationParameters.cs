using DspBlueprinter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter
{
    public class StationParameters
    {
        private static readonly int _STORAGE_OFFSET = 0;
        private static readonly int _SLOTS_OFFSET = _STORAGE_OFFSET + 192;
        private static readonly int _PARAMETERS_OFFSET = _SLOTS_OFFSET + 128;

        public (int workEnergy, int droneRange, int vesselRange, bool orbitalCollector, int warpDistance, bool equipWarper, int droneCount, int vesselCount) Parameters { get; private set; }
        public List<StorageItem> Storage { get; private set; }
        public List<SlotItem> Slots { get; private set; }

        public StationParameters(List<int> parameters, int storageLen, int slotsLen)
        {
            Storage = ParseStorage(parameters, storageLen);
            Slots = ParseSlots(parameters, slotsLen);
            Parameters = ParseParameters(parameters);
        }

        private List<StorageItem> ParseStorage(List<int> parameters, int storageLen)
        {
            var storage = new List<StorageItem>();
            for (int offset = _STORAGE_OFFSET; offset < _STORAGE_OFFSET + (6 * storageLen); offset += 6)
            {
                int itemId = parameters[offset];
                if (itemId == 0)
                {
                    storage.Add(StorageItem.EmptyItem);
                }
                else
                {
                    storage.Add(new StorageItem
                    {
                        ItemId = parameters[offset],
                        LocalLogic = parameters[offset + 1],
                        RemoteLogic = parameters[offset + 2],
                        MaxCount = parameters[offset + 3]
                    });
                }
            }
            return storage;
        }

        private List<SlotItem> ParseSlots(List<int> parameters, int slotsLen)
        {
            var slots = new List<SlotItem>();
            for (int offset = _SLOTS_OFFSET; offset < _SLOTS_OFFSET + (4 * slotsLen); offset += 4)
            {
                int storageIndex = parameters[offset + 1];
                if (storageIndex == 0)
                {
                    slots.Add(SlotItem.EmptySlotItem);
                }
                else
                {
                    slots.Add(new SlotItem
                    {
                        Direction = (LogisticsStationDirection)parameters[offset],
                        StorageIndex = storageIndex
                    });
                }
            }
            return slots;
        }

        private (int workEnergy, int droneRange, int vesselRange, bool orbitalCollector, int warpDistance, bool equipWarper, int droneCount, int vesselCount) ParseParameters(List<int> parameters)
        {
            return (
                parameters[_PARAMETERS_OFFSET],
                parameters[_PARAMETERS_OFFSET + 1],
                parameters[_PARAMETERS_OFFSET + 2],
                parameters[_PARAMETERS_OFFSET + 3] == 1,
                parameters[_PARAMETERS_OFFSET + 4],
                parameters[_PARAMETERS_OFFSET + 5] == 1,
                parameters[_PARAMETERS_OFFSET + 6],
                parameters[_PARAMETERS_OFFSET + 7]
            );
        }

        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
        {
            { "storage", Storage.Select(s => s?.ToDict()).ToList() },
            { "slots", Slots.Select(s => s?.ToDict()).ToList() },
            { "parameters", new
                {
                    Parameters.workEnergy,
                    Parameters.droneRange,
                    Parameters.vesselRange,
                    Parameters.orbitalCollector,
                    Parameters.warpDistance,
                    Parameters.equipWarper,
                    Parameters.droneCount,
                    Parameters.vesselCount
                }
            }
        };
        }
    }
}
