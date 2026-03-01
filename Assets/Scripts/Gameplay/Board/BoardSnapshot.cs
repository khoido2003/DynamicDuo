namespace Unity.DynamicDuo.Gameplay
{
    public readonly struct BoardSnapshot
    {
        public readonly TubeSnapshot[] TubeSnapshots;

        public BoardSnapshot(TubeSnapshot[] snapshots) => TubeSnapshots = snapshots;
    }
}
