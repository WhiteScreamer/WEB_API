using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SlotMachine
{
    public class SlotItem
    {
        public SlotItem Prev { get; set; }
        public SlotItem Next { get; set; }
        public byte Val { get; set; }
    }
    public static class SlotManager
    {
        public static SlotItem Insert(SlotItem curSlot, byte val)
        {
            var newSlot = new SlotItem() { Val = val };
            curSlot = curSlot ?? newSlot;
            newSlot.Prev = curSlot.Prev;
            curSlot.Prev = newSlot;
            newSlot.Next = curSlot;
            return newSlot;
        }
        public static SlotItem Roll(SlotItem slot)
        {
            return slot.Prev;
        }
        public static byte[] GetColumn(SlotItem slot)
        {
            return new byte[] { slot.Prev.Val, slot.Val, slot.Next.Val };
        }
    }
    public class Slot
    {
        private SlotItem curSlot = null;
        public void Add(byte val)
        {
            curSlot = SlotManager.Insert(curSlot, val);
        }
        public void Roll()
        {
            curSlot = SlotManager.Roll(curSlot);
        }
        public byte[] GetColumn()
        {
            return SlotManager.GetColumn(curSlot);
        }
    }
    public class SlotMachite
    {
        public SlotMachite(Slot[] _slots, int _minTurnCount, int _maxTurnCount, WinRule[] winRules)
        {
            slots = _slots;
            minTurnCount = _minTurnCount;
            maxTurnCount = _maxTurnCount;
            WinRules = winRules;
        }
        private Slot[] slots;
        private int minTurnCount = 0;
        private int maxTurnCount = 0;
        private byte[][] GetCells()
        {
            return slots.Select(o => o.GetColumn()).ToArray();
        }
        public SlotMachineSnapshot[] Roll(int seed)
        {
            var random = new Random(seed);
            var turnCount = slots.Select(o => random.Next(minTurnCount, maxTurnCount)).OrderBy(o => o).ToArray();
            var snapshots = new List<SlotMachineSnapshot>();
            do
            {
                var isStopped = new List<bool>();
                for(var i =0;i< slots.Length; i++)
                {
                    if (turnCount[i] > 0)
                    {
                        slots[i].Roll();
                        isStopped.Add(false);
                    }
                    else
                    {
                        isStopped.Add(true);
                    }
                }
                snapshots.Add(new SlotMachineSnapshot()
                {
                    IsStopped = isStopped.ToArray(),
                    Values = GetCells()
                });
                turnCount = turnCount.Select(o => o - 1).ToArray();
            } while (turnCount.Any(o => o > 0));
            return snapshots.ToArray();
        }
        public WinRule[] WinRules { get; private set; }
    }
    public class SlotMachineSnapshot
    {
        public byte[][] Values { get; set; }
        public bool[] IsStopped { get; set; }
    }
    public class SlotConfiguration
    {
        public int[][] slots { get; set; }
        public int minTurnCount { get; set; }
        public int maxTurnCount { get; set; }
        public WinRule[] winRules { get; set; }
    }

    public static class SlotMachiteBuilder
    {
        public static SlotMachite CreateMachine()
        {
            var jsonText = File.ReadAllText("SlotConfig.json");
            var config = JsonSerializer.Deserialize<SlotConfiguration>(jsonText);
            var slots = new List<Slot>();
            foreach (var slotValues in config.slots)
            {
                var newSlot = new Slot();
                foreach (var val in slotValues)
                {
                    newSlot.Add((byte)val);
                }
                slots.Add(newSlot);
            }
            return new SlotMachite(slots.ToArray(), config.minTurnCount, config.maxTurnCount,config.winRules);
        }
    }

    public class WinRuleCountKoef
    {
        public byte count { get; set; }
        public byte koef { get; set; }
    }

    public class WinRule
    {        
        public WinRuleCountKoef[] countKoef { get; set; }
        public byte value { get; set; }        
    }
}

