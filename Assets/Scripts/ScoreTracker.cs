using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker Instance;

    [field: Header("------ Score Tracking ------")]
    [field: SerializeField] int scorePerKill = 1;
    [field: SerializeField] TextMeshProUGUI scoreText;

    [field: Header("------ Damage Indicator ------")]
    [field: SerializeField] GameObject plusHair;
    [field: SerializeField] float timeToDisappear = 1f;

    private int currentScore = 0;
    private int playerDeaths = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        plusHair.SetActive(false);
        currentScore = 0;
        scoreText.text = "Score: " + currentScore;
    }

    public void UpdateScore()
    {
        currentScore += scorePerKill;
        scoreText.text = "Score: " + currentScore;
    }

    public void UpdateDeaths()
    {
        playerDeaths++;
    }

    public int ReturnPlayerScore()
    {
        return currentScore;
    }

    public int ReturnPlayerDeaths()
    {
        return playerDeaths;
    }

    public int ReturnScorePerKill()
    {
        return scorePerKill;
    }

    public void VisualizeImpact()
    {
        StartCoroutine(ShowImpact());
    }

    private IEnumerator ShowImpact()
    {
        plusHair.SetActive(true);
        yield return new WaitForSeconds(timeToDisappear);
        plusHair.SetActive(false);
    }
}
