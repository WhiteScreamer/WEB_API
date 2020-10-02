using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SlotMachine
{
    class Program
    {
        static private Timer rollTimer;
        static private IEnumerator snapshotEnumerator;
        static void Main(string[] args)
        {
            var m = SlotMachiteBuilder.CreateMachine();
            LoadView();

            var snapshots = m.Roll(DateTime.Now.Millisecond);
            snapshotEnumerator = snapshots.GetEnumerator();
            rollTimer = new Timer(50);
            rollTimer.Elapsed += Timer_Elapsed;
            rollTimer.Start();

            Console.ReadLine();
        }

        private static void LoadView()
        {
            Console.Write(File.ReadAllText("View.txt"));
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!snapshotEnumerator.MoveNext())
            {
                rollTimer.Stop();
            }
            else
            {                
                WriteMatrix(((SlotMachineSnapshot)snapshotEnumerator.Current).Values,0,0);
            }
        }
        private static void WriteMatrix(Array array,int x0, int y0)
        {
            Console.CursorLeft = y0;
            Console.CursorTop = x0;
            for (var x = 0; x < ((Array)array.GetValue(0)).Length; x++)
            {
                for (var y = 0; y < array.Length; y++)
                {
                    Console.Write(((Array)array.GetValue(y)).GetValue(x));
                }
                Console.WriteLine();
            }
        }
    }
}
