using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinsOwnershipDataHandler : MonoBehaviour
{
    public static SkinsOwnershipDataHandler instance;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private SkinsOwnershipData skinsOwnershipData;
    private void Awake()
    {
        instance = this;

        skinsOwnershipData = SaveLoadManager.LoadData<SkinsOwnershipData>();
        if (skinsOwnershipData == null)
        {
            skinsOwnershipData = new SkinsOwnershipData();
        }
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            EditSkinData();
        }
    }

    private void EditSkinData()
    {
        if (skinsOwnershipData.skinsInfo == null)
        {
            for (int i = 0; i < shopManager.infoOnSkins.Count; i++)
            {
                SavedSkinsInfo tempSkinsInfo = new SavedSkinsInfo();
                tempSkinsInfo.isOwned = shopManager.infoOnSkins[i].isOwned;
                tempSkinsInfo.isEquipped = shopManager.infoOnSkins[i].isEquipped;
                tempSkinsInfo.skinID = shopManager.infoOnSkins[i].skinID;
                skinsOwnershipData.skinsInfo.Add(tempSkinsInfo);
            }
            SaveSkinData();
        }
        else if (skinsOwnershipData.skinsInfo.Count != shopManager.infoOnSkins.Count)
        {
            for (int i = skinsOwnershipData.skinsInfo.Count; i < shopManager.infoOnSkins.Count; i++)
            {
                SavedSkinsInfo tempSkinsInfo = new SavedSkinsInfo();
                tempSkinsInfo.isOwned = shopManager.infoOnSkins[i].isOwned;
                tempSkinsInfo.isEquipped = shopManager.infoOnSkins[i].isEquipped;
                tempSkinsInfo.skinID = shopManager.infoOnSkins[i].skinID;
                skinsOwnershipData.skinsInfo.Add(tempSkinsInfo);
            }
            SaveSkinData();
        }
        else
        {
            HelperClass.DebugMessage("No Changes Required for Skins :D");
        }
    }

    #region Return All Data
    public SkinsOwnershipData ReturnSavedValues()
    {
        return skinsOwnershipData;
    }
    #endregion

    #region Saving
    public void SaveSkinData()
    {
        SaveLoadManager.SaveData(skinsOwnershipData);
    }
    private void OnDisable()
    {
        SaveSkinData();
    }
    #endregion
}

[System.Serializable]
public class SkinsOwnershipData
{
    public List<SavedSkinsInfo> skinsInfo = new List<SavedSkinsInfo>();
}