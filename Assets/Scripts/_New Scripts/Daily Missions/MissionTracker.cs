using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTracker : MonoBehaviour
{
    public static MissionTracker instance;

    private void Awake()
    {
        instance = this;
    }

    public void AdjustValues(Quest typeOfQuest)
    {
        if (!MissionsDataHandler.instance.ReturnSavedValues().completedMission1 || !MissionsDataHandler.instance.ReturnSavedValues().completedMission2 || !MissionsDataHandler.instance.ReturnSavedValues().completedMission3)
        {
            switch (typeOfQuest)
            {
                case Quest.PlayGames:
                    MissionsDataHandler.instance.ReturnSavedValues().currentNumberOfMatches += 1;
                    break;
                case Quest.Top3:
                    MissionsDataHandler.instance.ReturnSavedValues().currentNumberOfWins += 1;
                    break;
                case Quest.Bottom5:
                    MissionsDataHandler.instance.ReturnSavedValues().currentNumberOfLosses += 1;
                    break;
                case Quest.ShootDownPlanes:
                    MissionsDataHandler.instance.ReturnSavedValues().currentNumberOfADs += 1;
                    break;
            }
            MissionsDataHandler.instance.SaveMissionsData();
        }
    }
}
