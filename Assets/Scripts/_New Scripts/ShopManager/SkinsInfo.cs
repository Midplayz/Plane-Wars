using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkinsInfo
{
    public string skinName;
    public int skinID;
    public GameObject skinObject;
    public int skinPrice;
    public bool isOwned = false;
    public bool isEquipped = false;
    public bool isGems = true;
}
