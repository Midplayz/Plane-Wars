using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [field: Header("------ Winning Percentage ------")]
    [field: SerializeField][field: Range(0, 100)] int playerWinPercentage = 75;
    [field: SerializeField] int currentWinningPercentage;
    [field: SerializeField] int playersPosition;
    int multiplier;

    [field: Header("------ Scoreboard Content ------")]
    [field: SerializeField] List<TextMeshProUGUI> scoreboardNames;
    [field: SerializeField] List<TextMeshProUGUI> scoreboardScores;
    [field: SerializeField] int minimumOffsetForTopThree;
    [field: SerializeField] int scoreMaxOffsetMultiplier =  10;
    int scoreMaxOffset = 50;
    private List<int> aiScore = new List<int> { 0, 0, 0 };

    [field: Header("------ Player Specific Content ------")]
    [field: SerializeField] TextMeshProUGUI playerKills;
    [field: SerializeField] TextMeshProUGUI playerDeaths;
    [field: SerializeField] TextMeshProUGUI playerCoins;
    [field: SerializeField] TextMeshProUGUI playerPositionText;
    [field: SerializeField] int coinsPerKill = 5;

    [field: Header("------ Miscellaneous ------")]
    [field: SerializeField] int gamesTillAD = 3;
    [field: SerializeField] Button rewardedButton;
    [field: SerializeField] GameObject exitConfirmationPanel;
    public int amountOfCoinsWon { get; private set; }

    private void Start()
    {
        currentWinningPercentage = Random.Range(0, 101);
        multiplier = ScoreTracker.Instance.ReturnScorePerKill();
        scoreMaxOffset = multiplier * scoreMaxOffsetMultiplier;
        minimumOffsetForTopThree = multiplier * 3;
    }
    public void DecideResult()
    {
        for(int i =0; i < GameManager.Instance.allPlanesInGame.Count; i++)
        {
            Destroy(GameManager.Instance.allPlanesInGame[i]);
        }

        if(IronSource.Agent.isRewardedVideoAvailable())
        {
            rewardedButton.interactable = true;
        }
        else
        {
            rewardedButton.interactable = false;
        }
        int playerScore = ScoreTracker.Instance.ReturnPlayerScore();
        if(playerScore < (minimumOffsetForTopThree + multiplier))
        {
            playersPosition = Random.Range(4, GameManager.Instance.allPlanesInGame.Count+1);
        }
        else
        {
            int playerLosingPercentage = 100 - playerWinPercentage;
            if(currentWinningPercentage >= 0 && currentWinningPercentage <= playerLosingPercentage)
            {
                playersPosition = Random.Range(4, GameManager.Instance.allPlanesInGame.Count + 1);
            }
            else
            {
                playersPosition = Random.Range(1, 4);
            }
        }
        GetAIScores();
        PopulateScoreboard();
        UpdateUserDataHandler();
    } 

    private void PopulateScoreboard()
    {
        for(int i = 0; i < 3; i ++)
        {
            if(i == playersPosition-1)
            {
                scoreboardNames[i].text = UserDataHandler.instance.ReturnSavedValues().username;
                scoreboardScores[i].text = ScoreTracker.Instance.ReturnPlayerScore().ToString();
            }
            else
            {
                scoreboardNames[i].text = AINamesGenerator.Utils.GetRandomName();
                scoreboardScores[i].text = aiScore[i].ToString();
            }
        }
        playerKills.text = (ScoreTracker.Instance.ReturnPlayerScore() / multiplier).ToString();
        playerDeaths.text = ScoreTracker.Instance.ReturnPlayerDeaths().ToString();
        amountOfCoinsWon = ((ScoreTracker.Instance.ReturnPlayerScore() / multiplier) * coinsPerKill);
        playerCoins.text = amountOfCoinsWon.ToString();
        playerPositionText.text = playersPosition.ToString();
    }

    private void GetAIScores()
    {
        int playerScore = ScoreTracker.Instance.ReturnPlayerScore();
        if (playersPosition == 1)
        {
           
            aiScore[1] = ReturnValue((0 + (multiplier * 2)), playerScore - multiplier);
            aiScore[2] = ReturnValue((0 + multiplier),(aiScore[1] - multiplier));
        }
        else if(playersPosition == 2)
        {
            aiScore[0] = ReturnValue((playerScore + multiplier), (playerScore + multiplier + scoreMaxOffset));
            aiScore[2] = ReturnValue((0 + multiplier) , (playerScore - multiplier));
        }
        else if(playersPosition == 3)
        {
            aiScore[0] = ReturnValue((playerScore + (multiplier * 2)), ((playerScore + (multiplier * 2)) + scoreMaxOffset));
            aiScore[1] = ReturnValue((playerScore + multiplier), (aiScore[0] - multiplier));
        }
        else
        {
            aiScore[2] = ReturnValue((playerScore + (multiplier * playersPosition)), (playerScore + (multiplier * playersPosition)) + scoreMaxOffset);
            aiScore[1] = ReturnValue((aiScore[2] + multiplier), ((aiScore[2] + multiplier) + scoreMaxOffset));
            aiScore[0] = ReturnValue((aiScore[1] + multiplier), ((aiScore[1] + multiplier) + scoreMaxOffset));
        }
    }

    private int ReturnValue(int startValue, int endValue)
    {
        List<int> scoreList = new List<int>();
        for (int i = startValue; i <= endValue; i++)
        {
            if (i % multiplier == 0)
            {
                scoreList.Add(i);
            }
        }
        return scoreList[Random.Range(0, scoreList.Count)];
    }

    private void UpdateUserDataHandler()
    {
        UserDataHandler.instance.ReturnSavedValues().killsCount += (ScoreTracker.Instance.ReturnPlayerScore() / multiplier);
        UserDataHandler.instance.ReturnSavedValues().deathsCount += ScoreTracker.Instance.ReturnPlayerDeaths();

        if (playersPosition == 1)
        {
            UserDataHandler.instance.ReturnSavedValues().winsCount++;
        }
        UserDataHandler.instance.SaveUserData();
    }
    public void GoToMainMenu()
    {
        if (UserDataHandler.instance.ReturnSavedValues().gameOversTillAD++ == gamesTillAD)
        {
            ADManager.Instance.LoadInterstitialAd();
            UserDataHandler.instance.ReturnSavedValues().gameOversTillAD = 0;
            UserDataHandler.instance.SaveUserData();
        }
        else
        {
            UserDataHandler.instance.ReturnSavedValues().gameOversTillAD++;
            UserDataHandler.instance.SaveUserData();
            GameManager.Instance.ChangingScenes = true;
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    public void ExitConfirmation()
    {
        exitConfirmationPanel.SetActive(!exitConfirmationPanel.activeInHierarchy);
    }
}
