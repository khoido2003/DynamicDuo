using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class ApplicationController : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
