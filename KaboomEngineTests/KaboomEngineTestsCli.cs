using System;
using System.Linq;
using Com.Revo.Games.KaboomEngine;

namespace KaboomEngineTests
{
    static class KaboomEngineTestsCli
    {
        static void Main()
        {
            var engine = new KaboomEngineFactory().CreateEngine(31, 16, 20);
            while (engine.State == KaboomEngineState.Sweeping)
            {
                for (int y = 0; y < engine.Height; y++)
                {
                    for (int x = 0; x < engine.Width; x++)
                    {
                        var cell = engine.Cells[x, y];
                        Console.Write(cell.IsOpen
                                          ? cell.AdjacentMines == 0 ? " " : cell.AdjacentMines.ToString()
                                          : "X");
                    }
                    Console.WriteLine();
                }

                // ReSharper disable once PossibleNullReferenceException
                int[] c = Console.ReadLine().Split(',').Select(s => int.Parse(s.Trim())).ToArray();
                engine.Open(c[0], c[1]);
            }

            Console.WriteLine(engine.State);
        }
    }
}
