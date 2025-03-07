using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;
    static Managers Instance { get { Init(); return instance; } }

    #region managers
    [SerializeField]
    private PoolManager pool = new PoolManager();
    public static PoolManager Pool => Instance.pool;

    GameManager game = new GameManager();
    public static GameManager Game => Instance.game;

    private ResourceManager resource = new ResourceManager();
    public static ResourceManager Resource => Instance.resource;

    AudioManager audio = new AudioManager();
    public static AudioManager Audio => Instance.audio;

    SceneManagerLD scene = new SceneManagerLD();
    public static SceneManagerLD Scene => Instance.scene;

    SaveLoadManager saveLoad = new SaveLoadManager();
    public static SaveLoadManager SaveLoad => Instance.saveLoad;

    #endregion

    void Start()
    {
        // Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);
        Init();
    }

    static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
                instance = go.GetComponent<Managers>();
            }
            else
            {
                instance = go.GetComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            GenerateAudioSource();

            instance.pool.Init();
            instance.saveLoad.Init();
            instance.audio.Init();
        }
    }

    public static void Clear()
    {
        
    }

    static void GenerateAudioSource()
    {
        var bgm = new GameObject("@BGM", typeof(AudioSource)).GetComponent<AudioSource>();
        var sfx = new GameObject("@SFX", typeof(AudioSource)).GetComponent<AudioSource>();
        bgm.transform.SetParent(instance.transform);
        sfx.transform.SetParent(instance.transform);
        
        instance.audio.SetAudioSource(bgm, sfx);
    }

    private void OnApplicationQuit()
    {
        saveLoad.Save();
    }
}
