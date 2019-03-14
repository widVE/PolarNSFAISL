using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadNewScene()
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = Application.LoadLevelAsync(1);
        while (!async.isDone)
        {
            yield return null;
        }

    }
}
