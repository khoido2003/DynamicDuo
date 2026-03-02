using System.Collections.Generic;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField]
        private TubeView m_tubePrefab;

        [SerializeField]
        private float m_horizonalSpacing = 1.5f;

        [SerializeField]
        private float m_verticalSpacing = 2.5f;

        [SerializeField]
        private int m_tubesPerRow = 4;

        private readonly List<TubeView> m_tubeViews = new();

        public IReadOnlyList<TubeView> TubeViews => m_tubeViews;

        public void BuildBoard(IReadOnlyList<TubeModel> tubes, ColorPalette palette)
        {
            foreach (var view in m_tubeViews)
            {
                Destroy(view.gameObject);
            }
            m_tubeViews.Clear();

            for (int i = 0; i < tubes.Count; i++)
            {
                TubeView view = Instantiate(m_tubePrefab, transform);

                int row = i / m_tubesPerRow;
                int col = i % m_tubesPerRow;

                float totalWidth = (Mathf.Min(tubes.Count, m_tubesPerRow) - 1) * m_horizonalSpacing;

                float xOffset = col * m_horizonalSpacing - totalWidth / 2;
                float yOffset = -row * m_verticalSpacing;

                view.transform.localPosition = new Vector3(xOffset, yOffset, 0);

                view.Initialize(i);
                view.RefreshSegements(tubes[i].Segments, palette);

                if (tubes[i].IsComplete)
                    view.SetComplete(true);

                m_tubeViews.Add(view);
            }
        }
    }
}
