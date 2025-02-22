using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void AdjustCurrency(int amountToAdjust)
    {
        UserDataHandler.instance.ReturnSavedValues().currency += amountToAdjust;
        UserDataHandler.instance.SaveUserData();
    }
}
