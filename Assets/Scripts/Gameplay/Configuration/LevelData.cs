using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    [CreateAssetMenu(fileName = "Level_01", menuName = "DynamicDuo/LevelData")]
    public class LevelData : ScriptableObject
    {
        [SerializeField]
        private int m_levelNumber = 1;

        [SerializeField]
        private TubeData[] m_tubes = new TubeData[0];

        public int LevelNumber => m_levelNumber;
        public TubeData[] Tubes => m_tubes;
        public int TubeCount => m_tubes.Length;

        public bool Validate(GameConfig config, out string error)
        {
            if (m_tubes.Length < 3)
            {
                error = $"Level {m_levelNumber}: need at least  3 tubes";
                return false;
            }

            foreach (var tube in m_tubes)
            {
                if (tube.Colors.Length > config.TubeCapacity)
                {
                    error =
                        $"Level {m_levelNumber}: a tube has more colors than capacity ({config.TubeCapacity}).";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
