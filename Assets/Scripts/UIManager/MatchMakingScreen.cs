using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchMakingScreen : MonoBehaviour
{
    [field: Header("------ Match Making Screen ------")]
    [field: SerializeField] int maxTimeForSearching = 10;
    [field: SerializeField] TextMeshProUGUI mainContentText;
    [field: SerializeField] List<string> searchingText;

    float timer = 0f;
    float maxTimerValue;
    bool startTimer = false;
    int currentText = 0;

    private void OnEnable()
    { 
        maxTimerValue = Random.Range(2, maxTimeForSearching);
        startTimer = true;
        StartCoroutine(ChangeText());
    }

    private void Update()
    {
        if(startTimer)
        {
            timer += Time.deltaTime;
            if(timer >= maxTimerValue) 
            {
                StopCoroutine(ChangeText());
                mainContentText.text = "Match Found!";
                Invoke("ChangeScene", 1f);
                startTimer = false;
            }
        }
    }
    private IEnumerator ChangeText()
    {
        while(startTimer) 
        {
            mainContentText.text = searchingText[currentText];
            currentText++;
            if(currentText >= searchingText.Count)
            {
                currentText = 0;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private void ChangeScene()
    {
        SceneManager.LoadScene("MainGame");
    }
}
