namespace Com.Revo.Games.KaboomEngine.Kaboom 
{
    interface ISolveKaboomField
    {
        void Solve(KaboomField field, (int x, int y) coordinatesToOpen);
    }
}
