using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
public class AudioBankManager : MonoBehaviour
{
    public float MasterVolume = 1f;
    public float DialogueVolume = 1f;
    public float SFXVolume = 1f;
    public float MusicVolume = 1f;

    private FMOD.Studio.Bus Master;
    private FMOD.Studio.Bus SFX;
    private FMOD.Studio.Bus Music;
    private FMOD.Studio.Bus Dialogue;
    void Awake()
    {
        Master = FMODUnity.RuntimeManager.GetBus("bus:/");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        Dialogue = FMODUnity.RuntimeManager.GetBus("bus:/Dialogue Group");
    }

    void Update()
    {
        Master.setVolume(MasterVolume);
        
        if (SFX.isValid())
        {
            SFX.setVolume(SFXVolume);
        }

        if (Music.isValid())
        {
            Music.setVolume(MusicVolume);
        }

        if (Dialogue.isValid())
        {
            Dialogue.setVolume(DialogueVolume);
        }
        
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
    }

    public void SFXVolumeLevel(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;

        FMOD.Studio.PLAYBACK_STATE pbState;
    }

    public void DialogueVolumeLevel(float newDialogueVolume)
    {
        DialogueVolume = newDialogueVolume;
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
    }
}
