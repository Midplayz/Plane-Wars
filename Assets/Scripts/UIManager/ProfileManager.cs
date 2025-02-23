using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    [field: Header("------- Profile & Statistics -------")]
    [field: SerializeField] private TextMeshProUGUI userNameText;
    [field: SerializeField] private TextMeshProUGUI killsText;
    [field: SerializeField] private TextMeshProUGUI deathsText;
    [field: SerializeField] private TextMeshProUGUI winsText;
    [field: SerializeField] private TextMeshProUGUI rpText;

    public void SetPlayerStats()
    {
        userNameText.text = "Player";
        killsText.text = UserDataHandler.instance.ReturnSavedValues().killsCount.ToString();
        deathsText.text = UserDataHandler.instance.ReturnSavedValues().deathsCount.ToString();
        winsText.text = UserDataHandler.instance.ReturnSavedValues().winsCount.ToString();
        rpText.text = UserDataHandler.instance.ReturnSavedValues().rankPoints.ToString();
    }
}
