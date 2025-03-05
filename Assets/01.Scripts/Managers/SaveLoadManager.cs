using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[Serializable]
public class SaveLoadManager
{
    [SerializeField]
    public SaveData localSaveData;
    
    private static string SavePath => Path.Combine(Application.persistentDataPath, "Save.json");
    
    public void Init()
    {
        Load();
    }

    public void Save()
    {
        localSaveData ??= new SaveData();
        
        JObject jobj = JObject.FromObject(localSaveData);
        
        string json = JsonConvert.SerializeObject(jobj, Formatting.Indented);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Save();
            return;
        }
        
        JObject jobj = JObject.Parse(File.ReadAllText(SavePath));
        localSaveData = jobj.ToObject<SaveData>();
    }

    [MenuItem("Oniboogie/로컬저장 파일 열기")]
    public static void OpenLocalSaveFile()
    {
        if(Directory.Exists(Application.persistentDataPath))
        {
            Process.Start(SavePath);
        }
        else
        {
            Debug.Log("폴더 없음");
        }
    }
    
    [Serializable]
    public class SaveData
    {
        private int highScore = 0;
        private float bgmVolume = Define.DEFAULT_BGM_VOLUME;
        private float sfxVolume = Define.DEFAULT_SFX_VOLUME;

        #region 프로퍼티
        public int HighScore
        {
            get => highScore;
            set
            {
                highScore = value;
            }
        }
        public float BGMVolume
        {
            get => bgmVolume;
            set
            {
                bgmVolume = value;
                Managers.Audio.SetBgmVolume(value);
            }
        }
        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = value;
                Managers.Audio.SetSfxVolume(value);
            }
        }
        #endregion
    }
}
