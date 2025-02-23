using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [field: Header("------ Winning Percentage ------")]
    [field: SerializeField] int playersPosition;
    int multiplier;

    [field: Header("------ Scoreboard Content ------")]
    [field: SerializeField] List<TextMeshProUGUI> scoreboardNames;
    [field: SerializeField] List<TextMeshProUGUI> scoreboardScores;

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

    private List<int> allPlanesScore;
    public int amountOfCoinsWon { get; private set; }

    private void Start()
    {
        multiplier = ScoreTracker.Instance.ReturnScorePerKill();
    }
    public void ShowResults()
    {
        //Get Scores
        allPlanesScore = new List<int>();
        for (int i = 0; i < GameManager.Instance.allPlanesInGame.Count; i++)
        {
            if (GameManager.Instance.allPlanesInGame[i].TryGetComponent<PlaneMovement>(out PlaneMovement planeMovement))
            {
                allPlanesScore.Add(ScoreTracker.Instance.ReturnPlayerScore());
                continue;
            }
            allPlanesScore.Add(GameManager.Instance.allPlanesInGame[i].GetComponent<MLAgentController>().currentScore);
        }

        //Arrange
        allPlanesScore = allPlanesScore.OrderByDescending(_score => _score).ToList();

        //Get Player Position
        for (int i = 0; i < allPlanesScore.Count; i++)
        {
            if (allPlanesScore[i] == ScoreTracker.Instance.ReturnPlayerScore())
            {
                playersPosition = i + 1;
                break;
            }
        }

        //Populate Scoreboard
        for (int i = 0; i < 3; i++)
        {
            if (i == playersPosition - 1)
            {
                scoreboardNames[i].text = "You";
                scoreboardScores[i].text = ScoreTracker.Instance.ReturnPlayerScore().ToString();
            }
            else
            {
                scoreboardNames[i].text = AINamesGenerator.Utils.GetRandomName();
                scoreboardScores[i].text = allPlanesScore[i].ToString();
            }
        }
        if (playersPosition <= 3)
        {
            MissionTracker.instance.AdjustValues(Quest.Top3);
        }
        else if(playersPosition <= allPlanesScore.Count && playersPosition >= allPlanesScore.Count - 5)
        {
            MissionTracker.instance.AdjustValues(Quest.Bottom5);
        }

        playerKills.text = (ScoreTracker.Instance.ReturnPlayerScore() / multiplier).ToString();
        playerDeaths.text = ScoreTracker.Instance.ReturnPlayerDeaths().ToString();
        amountOfCoinsWon = ((ScoreTracker.Instance.ReturnPlayerScore() / multiplier) * coinsPerKill);
        playerCoins.text = amountOfCoinsWon.ToString();
        playerPositionText.text = playersPosition.ToString();
        UpdateUserDataHandler();
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
        CurrencyManager.instance.AdjustCurrency(amountOfCoinsWon);
        GameManager.Instance.ChangingScenes = true;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ExitConfirmation()
    {
        exitConfirmationPanel.SetActive(!exitConfirmationPanel.activeInHierarchy);
    }
}
