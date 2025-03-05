using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AudioManager
{
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    public void Init()
    {
        bgmSource.volume = Managers.SaveLoad.localSaveData.BGMVolume;
        bgmSource.loop = true;
        bgmSource.playOnAwake = true;
        
        sfxSource.volume = Managers.SaveLoad.localSaveData.SFXVolume;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    public void SetAudioSource(AudioSource bgm, AudioSource sfx)
    {
        bgmSource = bgm;
        sfxSource = sfx;
    }

    public async UniTask PlayBgm(Define.Bgm bgm)
    {
        string address = new StringBuilder(bgm.ToString()).Insert(0,"Bgm/").ToString();
        var clip = await Managers.Resource.LoadAsset<AudioClip>(address);

        if (bgmSource.isPlaying)
        {
            await bgmSource.DOFade(0f, 0.5f);
            bgmSource.clip = clip;
            bgmSource.Play();
            await bgmSource.DOFade(Managers.SaveLoad.localSaveData.BGMVolume, 0.5f);
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public async UniTask PlaySfx(Define.Sfx sfx)
    {
        string address = new StringBuilder(sfx.ToString()).Insert(0, "Sfx/").ToString();
        var clip = await Managers.Resource.LoadAsset<AudioClip>(address);
        
        sfxSource.PlayOneShot(clip);
    }

    public void SetBgmPitch(int pitchLevel)
    {
        bgmSource.pitch = Define.BGM_PITCH[pitchLevel];
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
