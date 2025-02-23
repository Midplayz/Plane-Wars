using System;
using System.Collections;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurrencyDataHandler : MonoBehaviour
{
    public static CurrencyDataHandler instance;
    [SerializeField] private CurrencyData currencyData;
    private void Awake()
    {
        instance = this;

        currencyData = SaveLoadManager.LoadData<CurrencyData>();
        if (currencyData == null)
        {
            currencyData = new CurrencyData();
        }
    }

    #region Return All Data
    public CurrencyData ReturnSavedValues()
    {
        return currencyData;
    }
    #endregion

    #region Saving
    public void SaveCurrencyData()
    {
        SaveLoadManager.SaveData(currencyData);
    }
    private void OnDisable()
    {
        SaveCurrencyData();
    }
    #endregion
}


[System.Serializable]
public class CurrencyData
{
    public int amountOfCoins = 100;
    public bool hasAddedInitialDataToLeaderBoard = false;
    public string playerPFP;
    public string opponentPFP;
    public string opponentName;
}