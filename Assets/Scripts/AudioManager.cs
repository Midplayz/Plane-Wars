using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [field: Header("------- Background Music -------")]
    [field: SerializeField] private AudioSource audioSource;
    [field: SerializeField] bool isMusic = true;
    [field: SerializeField] private List<AudioClip> clipList;

    void Start()
    {
        if(isMusic && SettingsDataHandler.instance.ReturnSavedValues().musicMuted)
        {
            audioSource.volume = 0f;
        }
        else if (!isMusic && SettingsDataHandler.instance.ReturnSavedValues().soundMuted)
        {
            audioSource.volume = 0f;
        }

                audioSource.clip = clipList[Random.Range(0, clipList.Count)];
        audioSource.Play();
        
    }

}
