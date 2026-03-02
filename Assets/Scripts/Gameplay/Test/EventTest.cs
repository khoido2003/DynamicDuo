using Unity.DynamicDuo.Infrastructure;
using UnityEngine;
using VContainer;

public class EventTest : MonoBehaviour
{
    [Inject]
    IPublisher<TubeClickedEvent> m_tubeClickedPub;

    [Inject]
    IPublisher<UndoRequestedEvent> m_undoPub;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_tubeClickedPub.Publish(new TubeClickedEvent { TubeIndex = 0 });
            Debug.Log("Published TubeClicked 0");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_tubeClickedPub.Publish(new TubeClickedEvent { TubeIndex = 6 });
            Debug.Log("Published TubeClicked 6");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            m_undoPub.Publish(new UndoRequestedEvent());
            Debug.Log("Published Undo");
        }
    }
}
