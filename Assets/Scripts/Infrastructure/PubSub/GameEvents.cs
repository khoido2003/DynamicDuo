namespace Unity.DynamicDuo.Infrastructure
{
    public struct TubeClickedEvent
    {
        public int TubeIndex;
    }

    public struct UndoRequestedEvent { }

    public struct PauseRequestedEvent { }

    public struct ResumeRequestedEvent { }

    public struct RestartRequestedEvent { }

    public struct NextLevelRequestedEvent { }

    public struct PourSucceededEvent
    {
        public int FromIndex;
        public int ToIndex;
    }

    public struct PourFailedEvent
    {
        public int TubeIndex;
    }

    public struct MoveCountChangedEvent
    {
        public int MoveCount;
    }

    public struct BoardRestoredEvent { }

    public struct WinEvent { }

    public struct LoseEvent { }

    public struct LevelLoadedEvent
    {
        public int LevelNumber;
    }

    public struct ReturnToMenuRequestedEvent { }
}
