using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [field: Header("Play Sounds at Random Intervals")]
    [field: SerializeField] List<AudioClip> clip;
    [field: SerializeField] AudioSource audioSource;
    [field: SerializeField] int minimumDelay;
    [field: SerializeField] int maximumDelay;

    int timeToWaitFor;
    bool playSounds = true;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if(!SettingsDataHandler.instance.ReturnSavedValues().soundMuted)
        {
            playSounds = true;
            StartCoroutine(PlaySoundsAtRandomIntervals());
        }
    }

    private IEnumerator PlaySoundsAtRandomIntervals()
    {
        while(playSounds)
        {
            timeToWaitFor = Random.Range(minimumDelay, maximumDelay);
            audioSource.clip = clip[Random.Range(0, clip.Count)];
            audioSource.Play();
            yield return new WaitForSeconds(timeToWaitFor);
        }
    }

    private void OnDisable()
    {
        playSounds = false;
        StopCoroutine(PlaySoundsAtRandomIntervals());
    }
}
