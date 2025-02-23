using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePitch : MonoBehaviour
{
    [field: SerializeField] float minPitch = 0.8f;  // Minimum pitch value
    [field: SerializeField] float maxPitch = 1.2f;  // Maximum pitch value

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(SettingsDataHandler.instance.ReturnSavedValues().soundMuted)
        {
            audioSource.enabled = false;
        }
        audioSource.pitch = Random.Range(minPitch, maxPitch);
    }
}
