using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class SFXManager : MonoBehaviour
{
    private DialogueManager _dialogueManager;
    public EventInstance instance;
    public EventReference footStepEvent;
    public EventReference grabEvent;
    public EventReference slideEvent;
    public EventReference vaultEvent;
    public EventReference wallRunEvent;
    public EventReference jumpEvent;
    public EventReference landEvent;
    public EventReference catwalkEvent;
    public EventReference pantEvent;
    public EventReference deathEvent;
    private GameObject player;
    private bool catWalkOn = false;
    void Awake()
    {
        player = GameObject.Find("Player");
        _dialogueManager = FindObjectOfType<DialogueManager>();
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());
    }
    
    public void Death()
    {
        if (!deathEvent.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(deathEvent, player);
        }
    }
    public void Pant()
    {
        if (!pantEvent.IsNull)
        {
            if(!_dialogueManager.IsDialoguePlaying())
                RuntimeManager.PlayOneShotAttached(pantEvent, player);
        }
    }
    public void Step()
    {
        if (!catWalkOn)
        {
            if (!footStepEvent.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(footStepEvent, player);
            }
        }
        else
        {
            if (!catwalkEvent.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(catwalkEvent, player);
            }
        }
    }

    public void Grab()
    {
        if (!grabEvent.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(grabEvent, player);
        }
    }

    public void Slide()
    {

        if (!slideEvent.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(slideEvent, player);
        }
    }

    public void Vault()
    {
        if (!vaultEvent.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(vaultEvent, player);
        }
    }

    public void WallRun()
    {
        if (!catWalkOn)
        {
            if (!wallRunEvent.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(wallRunEvent, player);
            }
        }
        else
        {
            if (!catwalkEvent.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(catwalkEvent, player);
            }
        }
    }
    
    public void Jump()
    {
        if (!jumpEvent.IsNull)
        {
            if(!_dialogueManager.IsDialoguePlaying())
                RuntimeManager.PlayOneShotAttached(jumpEvent, player);
        }
    }
    
    public void Land()
    {
        if (!landEvent.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(landEvent, player);
        }
    }
    
    public void CatWalkOn()
    {
        catWalkOn = true;
    }

    public void CatWalkOff()
    {
        catWalkOn = false;
    }
}