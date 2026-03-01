using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DynamicDuo/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private int m_tubeCapacity = 4;

        [SerializeField]
        private int m_emptyTubeCount = 2;

        public int TubeCapacity => m_tubeCapacity;
        public int EmptyTubeCount => m_emptyTubeCount;
    }
}
