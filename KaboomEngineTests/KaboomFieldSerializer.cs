using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Com.Revo.Games.KaboomEngine.Kaboom;

namespace KaboomEngineTests
{
    [ExcludeFromCodeCoverage]
    static class KaboomFieldSerializer
    {
        public static string Serialize(this KaboomField field)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{field.Width} {field.Height} {field.NumberOfMines}");

            var lines = Enumerable.Range(0, field.Height)
                                  .Select(row =>
                                              new string(Enumerable.Range(0, field.Width)
                                                                   .Select(col => field.Cells[col, row])
                                                                   .Select(cell => cell.IsOpen
                                                                                       ? cell.AdjacentMines.ToString()[0]
                                                                                       : cell.IsMine || cell.State == KaboomState.Mine
                                                                                           ? '!'
                                                                                           : cell.State switch
                                                                                           {
                                                                                               KaboomState.Indeterminate => '?',
                                                                                               KaboomState.Mine => '!',
                                                                                               KaboomState.Free => '_',
                                                                                               _ => '.'
                                                                                           })
                                                                   .ToArray()));

            foreach (string line in lines)
                sb.AppendLine(line);
            return sb.ToString();
        }
        public static KaboomField ToKaboomField(this string s, ISolveKaboomField solver)
        {
            string[] lines = s.Trim().Split('\n');
            string[] infos = lines[0].Split();
            KaboomField field = new KaboomField(Convert.ToInt32(infos[0]), Convert.ToInt32(infos[1]), Convert.ToInt32(infos[2]), solver);

            for (int row = 0; row < field.Height; row++)
            for (int col = 0; col < field.Width; col++)
            {
                var cell = field.Cells[col, row];
                char c = lines[row + 1][col];
                if (char.IsDigit(c))
                {
                    cell.State = KaboomState.None;
                    cell.IsOpen = true;
                    cell.IsMine = false;
                    cell.AdjacentMines = Convert.ToInt32(c.ToString());
                    continue;
                }

                cell.IsOpen = false;
                cell.AdjacentMines = 0;

                switch (c)
                {
                    case '.':
                        cell.State = KaboomState.None;
                        cell.IsMine = false;
                        break;
                    case '_':
                        cell.State = KaboomState.Free;
                        cell.IsMine = false;
                        break;
                    case '?':
                        cell.State = KaboomState.Indeterminate;
                        cell.IsMine = false;
                        break;
                    case '!':
                        cell.State = KaboomState.Mine;
                        cell.IsMine = true;
                        break;
                }
            }

            return field;
        }
    }
}
