using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [field: Header("Loading Screen Stuff")]
    [field: SerializeField] private GameObject loadingScenePanel;

    [field: Header("On Screen Stuff")]
    [field: SerializeField] TextMeshProUGUI tipsText;
    [field: SerializeField] List<string> loadingScreenText;
    [field: SerializeField] int timeBetweenText;
    [field: SerializeField] List<Sprite> loadingScreenSprites;
    [field: SerializeField] Image loadingScreenImage;

    private int randomNumberOfText;
    private int counterOfText = 0;

    float timer = 0;
    bool startTimer = false;

    private void Start()
    {
        tipsText.text = loadingScreenText[Random.Range(0, loadingScreenText.Count)];
        loadingScreenImage.sprite = loadingScreenSprites[Random.Range(0, loadingScreenSprites.Count)];
        startTimer = true;
        randomNumberOfText = Random.Range(1, 3);
        counterOfText = 0;
    }

    public void CheckWhatToDo()
    {
        loadingScenePanel.SetActive(false);
        startTimer = false;
    }

    private void Update()
    {
        if(startTimer)
        {
            timer += Time.deltaTime;
            if(timer >= timeBetweenText) 
            { 
                tipsText.text = loadingScreenText[Random.Range(0, loadingScreenText.Count)];
                timer = 0;
                counterOfText++;
                if(counterOfText >= randomNumberOfText)
                {
                    CheckWhatToDo();
                }
            }
        }
    }
}
