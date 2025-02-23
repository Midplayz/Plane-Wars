using System;
using UnityEngine;

public class MissionsDataHandler : MonoBehaviour
{
    public static MissionsDataHandler instance;
    [SerializeField] private MissionsData missionsData;
    private void Awake()
    {
        instance = this;

        missionsData = SaveLoadManager.LoadData<MissionsData>();
        if (missionsData == null)
        {
            missionsData = new MissionsData();
        }
    }

    #region Check If It's Next Day
    public bool CheckIfItsNextDay()
    {
        if (!string.IsNullOrEmpty(missionsData.DateAndTimeLastOpened))
        {
            DateTime lastOpened = DateTime.Parse(missionsData.DateAndTimeLastOpened);
            if (lastOpened.Date < GetInternetTime.Instance.GetCurrentDateTime().Date)
            {
                missionsData.DateAndTimeLastOpened = GetInternetTime.Instance.GetCurrentDateTime().ToString();
                SaveMissionsData();
                return true;
            }
            else
            {
                missionsData.DateAndTimeLastOpened = GetInternetTime.Instance.GetCurrentDateTime().ToString();
                SaveMissionsData();
                return false;
            }
        }
        else
        {
            missionsData.DateAndTimeLastOpened = GetInternetTime.Instance.GetCurrentDateTime().ToString();
            SaveMissionsData();
            return true;
        }
    }
    #endregion

    #region Reset Data
    public void ResetAllData()
    {
        missionsData.currentNumberOfMatches = 0;
        missionsData.currentNumberOfWins = 0;
        missionsData.currentNumberOfLosses = 0;
        missionsData.currentNumberOfADs = 0;

        missionsData.completedMission1 = false;
        missionsData.completedMission2 = false;
        missionsData.completedMission3 = false;

        missionsData.currentTargetForMatches = 0;
        missionsData.currentTargetForWins = 0;
        missionsData.currentTargetForLosses = 0;
        missionsData.currentTargetForADs = 0;
        SaveMissionsData();
}
    #endregion

    #region Return All Data
    public MissionsData ReturnSavedValues()
    {
        return missionsData;
    }
    #endregion

    #region Saving
    public void SaveMissionsData()
    {
        SaveLoadManager.SaveData(missionsData);
    }
    private void OnDisable()
    {
        missionsData.DateAndTimeLastOpened = GetInternetTime.Instance.GetCurrentDateTime().ToString();
        SaveMissionsData();
    }
    #endregion
}

[System.Serializable]
public class MissionsData
{
    public int currentNumberOfMatches = 0;
    public int currentNumberOfWins = 0;
    public int currentNumberOfLosses = 0;
    public int currentNumberOfADs = 0;

    public bool completedMission1 = false;
    public bool completedMission2 = false;
    public bool completedMission3 = false;

    public int currentTargetForMatches = 0;
    public int currentTargetForWins = 0;
    public int currentTargetForLosses = 0;
    public int currentTargetForADs = 0;

    public string DateAndTimeLastOpened = null;

    public int[] todaysMissions = new int[3];
}