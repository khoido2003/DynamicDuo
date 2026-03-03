using System.Collections.Generic;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class LoadingState : IGameState
    {
        readonly GameStateMachine m_stateMachine;
        readonly BoardModel m_boardModel;
        readonly LevelData m_levelData;
        readonly GameConfig m_gameConfig;
        readonly LevelGeneratorModel m_generator;

        public LoadingState(
            GameStateMachine stateMachine,
            BoardModel boardModel,
            LevelData levelData,
            GameConfig gameConfig,
            LevelGeneratorModel generator
        )
        {
            m_stateMachine = stateMachine;
            m_boardModel = boardModel;
            m_levelData = levelData;
            m_gameConfig = gameConfig;
            m_generator = generator;
        }

        public void Enter()
        {
            Debug.Log("[LoadingState] Enter");

            List<ColorSegment[]> segments;
            int tubeCount;
            int capacity;

            if (LevelSession.UseGenerator)
            {
                // Use auto generator
                capacity = LevelSession.TubeCapacity;
                var rng = new System.Random(LevelSession.Seed);

                segments = m_generator.Generate(
                    LevelSession.ColorCount,
                    capacity,
                    LevelSession.EmptyTubes,
                    LevelSession.Seed
                );

                tubeCount = LevelSession.ColorCount + LevelSession.EmptyTubes;

                Debug.Log($"[LoadingState] Generated level — seed: {LevelSession.Seed}");
            }
            else
            {
                //  Premade level
                if (!m_levelData.Validate(m_gameConfig, out string error))
                {
                    Debug.LogError($"[LoadingState] {error}");
                    return;
                }

                capacity = m_gameConfig.TubeCapacity;
                tubeCount = m_levelData.TubeCount;
                segments = new List<ColorSegment[]>();

                foreach (var tube in m_levelData.Tubes)
                    segments.Add(tube.ToSegments());
            }

            m_boardModel.Initialize(tubeCount, capacity, segments);
            Debug.Log($"[LoadingState] Board ready — {m_boardModel.Tubes.Count} tubes");
            m_stateMachine.TransitionTo<GameplayState>();
        }

        public void Exit()
        {
            Debug.Log("[LoadingState] Exit");
        }
    }
}
