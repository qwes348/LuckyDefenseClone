using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerLD
{
    private Define.Scene nextSceneType;
    
    public BaseScene CurrentScene => Object.FindObjectOfType<BaseScene>();
    public Define.Scene NextSceneType => nextSceneType;

    public void LoadScene(Define.Scene sceneType)
    {
        CurrentScene.Clear();
        SceneManager.LoadScene(GetSceneName(sceneType));
    }

    public void LoadSceneViaLoading(Define.Scene sceneType)
    {
        nextSceneType = sceneType;
        CurrentScene.Clear();
        LoadScene(Define.Scene.Loading);
    }

    public AsyncOperation LoadSceneAsync(Define.Scene sceneType)
    {
        CurrentScene.Clear();
        return SceneManager.LoadSceneAsync(GetSceneName(sceneType));
    }

    private string GetSceneName(Define.Scene sceneType)
    {
        return $"{sceneType.ToString()}Scene";
    }
}
