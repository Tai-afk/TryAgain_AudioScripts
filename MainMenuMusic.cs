using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using EventReference = Mono.Cecil.EventReference;

public class MainMenuMusic : MonoBehaviour
{
    public EventInstance musicInstance;

    public FMODUnity.EventReference musicReference;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicReference);
        musicInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
