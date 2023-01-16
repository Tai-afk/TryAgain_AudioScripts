using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using System.Runtime.InteropServices;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public struct DialogueEvent
{
    public string name;
    public EventInstance instance;
    public bool hasStarted;
}

public class DialogueManager : MonoBehaviour
{
    [ShowInInspector] private List<DialogueEvent> dialogueInstanceQueue;
    [SerializeField] private List<EventReference> dialogueEvents;
    [SerializeField] private List<string> dialogueStringReferences;
    private Dictionary<string, EventReference> dialogueDictionary;
    [SerializeField] private Transform player;
    private EventInstance _currentInstance;
    public DialogueEvent currentEvent;


    /// <summary>
    /// Get Spectrum data window
    /// </summary>
    public int _windowSize = 512;
    public FMOD.DSP_FFT_WINDOW _windowShape = FMOD.DSP_FFT_WINDOW.RECT;
    private FMOD.ChannelGroup _channelGroup;
    private FMOD.DSP _dsp;
    private FMOD.DSP_PARAMETER_FFT _fftparam;
    public float[] _samples;

    private TVManager tvManager;
    private void Start()
    {
        dialogueDictionary = new Dictionary<string, EventReference>();
        dialogueInstanceQueue = new List<DialogueEvent>();
        FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out _dsp);
        _dsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)_windowShape);
        _dsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, _windowSize * 2);
        _samples = new float[_windowSize];
        if (dialogueEvents.Count == dialogueStringReferences.Count)
        {
            for (int i = 0; i < dialogueEvents.Count; i++)
            {
                dialogueDictionary.Add(dialogueStringReferences[i], dialogueEvents[i]);
            }
        }
        else
        {
            Debug.Log("Wrong number of dialogue strings or events");
        }
        tvManager = (TVManager)FindObjectOfType(typeof(TVManager));
    }

    private void Update()
    {
        int i = 10;
        GetSpectrumData();
    }

    private void FixedUpdate()
    {
        if (dialogueInstanceQueue.IsNullOrEmpty())
        {
            return;
        }
        
        currentEvent = dialogueInstanceQueue[0];
            
        // Check if the current event has completed
        if (currentEvent.hasStarted)
        {
            currentEvent.instance.getChannelGroup(out _channelGroup);
            _channelGroup.addDSP(0, _dsp);
            if (HasInstanceStopped(currentEvent.instance))
            {
                Debug.Log("Finished dialogue event: " + currentEvent.name);
                currentEvent.instance.release();
                dialogueInstanceQueue.RemoveAt(0);
                
                // Try to start the next event
                TryStartNextEvent();
            }
        }
            
        // Try to start the current event
        else
        {
            TryStartNextEvent();
        }
    }

    public void PlayDialogue(string key, int type)
    {
        Debug.Log("Queueing dialogue event: " + key);
        if(currentEvent.hasStarted)
            currentEvent.instance.stop(STOP_MODE.ALLOWFADEOUT);
        dialogueInstanceQueue.Add(new DialogueEvent()
        {
            name = key,
            hasStarted = false
        });
    }

    private void TryStartNextEvent()
    {
        
        if (dialogueInstanceQueue.Count > 0)
        {
            DialogueEvent eventToStart = dialogueInstanceQueue[0];
            EventInstance instance = RuntimeManager.CreateInstance(dialogueDictionary[eventToStart.name]);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(player));
            instance.start();
            EventDescription eventDescription = new EventDescription();
            instance.getDescription(out eventDescription);
            //print(eventDescription.);
            eventToStart.hasStarted = true;
            eventToStart.instance = instance;
            dialogueInstanceQueue[0] = eventToStart;
            Debug.Log("Starting dialogue event: " + eventToStart.name);
        }
    }

    public bool IsDialoguePlaying()
    {
        if (dialogueInstanceQueue.Count > 0)
        {
            DialogueEvent dialogueEvent = dialogueInstanceQueue[0];
            if (dialogueEvent.hasStarted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    private bool HasInstanceStopped(EventInstance instance)
    {
        instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        return state == FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    private void GetSpectrumData()
    {
        System.IntPtr _data;
        uint _length;
        
        _dsp.getParameterData(2, out _data, out _length);
        _fftparam = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(_data, typeof(FMOD.DSP_PARAMETER_FFT));
        

        if (_fftparam.numchannels == 0)
        {
            currentEvent.instance.getChannelGroup(out _channelGroup);
            _channelGroup.addDSP(0, _dsp);
            //Debug.Log("wait I'm not ready yet!");
        }
        else if (_fftparam.numchannels >= 1)
        {
            for (int s = 0; s < _windowSize; s++)
            {
                float _totalChannelData = 0f;
                for (int c = 0; c < _fftparam.numchannels; c++)
                    _totalChannelData += _fftparam.spectrum[c][s];
                _samples[s] = _totalChannelData / _fftparam.numchannels;
                if (tvManager != null) tvManager.curAudioIntensity = _samples[s];
            }
            //Debug.Log("working with: " + fftparam.numchannels + " channels here baby!");
        }
    }
}

public class OldDialogueManager : MonoBehaviour
{
//     // private EVENT_CALLBACK dialogueCallBack;
//
//     // public EventReference eventDialogue;
//     // public EventReference eventBarks;
//     // private GameObject player;
//     // private BackgroundFade levelMusic;
//     // private EventInstance dialogueInstance;
//     [ShowInInspector] private List<string> dialogueQueue;
//
//     //private bool dialogueInstancePlaying;
// #if UNITY_EDITOR
//     void Reset()
//     {
//         // eventBarks = EventReference.Find("event:/Barks");
//         // eventDialogue = EventReference.Find("event:/Dialogue");
//     }
// #endif
//     // Start is called before the first frame update
//     void Start()
//     {
//         // levelMusic = GameObject.Find("AudioManager").GetComponent<BackgroundFade>();
//         // player = GameObject.Find("Player");
//         // dialogueCallBack = new EVENT_CALLBACK(DialogueEventCallback);
//         // dialogueQueue = new List<string>();
//     }
//
//
//     #region Old Dialogue Functions
//
//     // [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
//     // FMOD.RESULT DialogueEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
//     // {
//     //     EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);
//     //
//     //     // Retrieve the user data
//     //     IntPtr stringPtr;
//     //     instance.getUserData(out stringPtr);
//     //
//     //     // Get the string object
//     //     GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
//     //     String key = stringHandle.Target as String;
//     //     
//     //     switch (type)
//     //     {
//     //         case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
//     //         {
//     //             FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
//     //             var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
//     //
//     //             if (key.Contains("."))
//     //             {
//     //                 FMOD.Sound dialogueSound;
//     //                 var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
//     //                 if (soundResult == FMOD.RESULT.OK)
//     //                 {
//     //                     parameter.sound = dialogueSound.handle;
//     //                     parameter.subsoundIndex = -1;
//     //                     Marshal.StructureToPtr(parameter, parameterPtr, false);
//     //                     dialogueInstancePlaying = true;
//     //                 }
//     //             }
//     //             else
//     //             {
//     //                 FMOD.Studio.SOUND_INFO dialogueSoundInfo;
//     //                 var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
//     //                 if (keyResult != FMOD.RESULT.OK)
//     //                 {
//     //                     break;
//     //                 }
//     //                 FMOD.Sound dialogueSound;
//     //                 var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
//     //                 if (soundResult == FMOD.RESULT.OK)
//     //                 {
//     //                     parameter.sound = dialogueSound.handle;
//     //                     parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
//     //                     Marshal.StructureToPtr(parameter, parameterPtr, false);
//     //                     dialogueInstancePlaying = true;
//     //                 }
//     //             }
//     //             break;
//     //         }
//     //         case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
//     //         {
//     //             var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
//     //             var sound = new FMOD.Sound(parameter.sound);
//     //             sound.release();
//     //             
//     //             dialogueInstancePlaying = false;
//     //             PopDialogueQueue();
//     //
//     //             break;
//     //         }
//     //         case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
//     //         {
//     //             // Now the event has been destroyed, unpin the string memory so it can be garbage collected
//     //             stringHandle.Free();
//     //
//     //             break;
//     //         }
//     //     }
//     //     return FMOD.RESULT.OK;
//     // }
//     //
//     //
//     // /// <summary>
//     // /// key for the name of the dialogue in the audio table in fmod
//     // /// type is for either the event dialogue or the event barks
//     // /// </summary>
//     // /// <param name="key"></param>
//     // /// <param name="type"></param>
//     // public void PlayDialogue(string key, int type)
//     // {
//     //     dialogueQueue.Add(key);
//     //     if (dialogueInstancePlaying)
//     //     {
//     //         return;
//     //     }
//     //     dialogueQueue.RemoveAt(0);
//     //     PlaySoundInstance(key, type);
//     // }
//     //
//     // public void PlaySoundInstance(string key, int type, bool triggerWhenPreviousDialogueEnd = false)
//     // {
//     //     dialogueInstance = new EventInstance();
//     //     if (type == 0)
//     //     {
//     //         dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(eventDialogue);
//     //     }
//     //     else if (type == 1)
//     //     {
//     //         dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(eventBarks);
//     //     }
//     //
//     //     if (!triggerWhenPreviousDialogueEnd)
//     //     {
//     //         dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(player.transform));
//     //     }
//     //
//     //     // Pin the key string in memory and pass a pointer through the user data
//     //     GCHandle stringHandle = GCHandle.Alloc(key);
//     //     dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
//     //
//     //     dialogueInstance.setCallback(dialogueCallBack);
//     //     dialogueInstance.start();
//     //     dialogueInstance.release();
//     // }
//     //
//     // public void PopDialogueQueue(int type = 0)
//     // {
//     //     if (dialogueQueue.Count == 0) return;
//     //     var key = dialogueQueue[0];
//     //     dialogueQueue.RemoveAt(0);
//     //     PlaySoundInstance(key, type, true);
//     // }
//     //
//
//     #endregion
}