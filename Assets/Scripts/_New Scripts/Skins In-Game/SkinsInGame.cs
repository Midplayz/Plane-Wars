using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkinsInGame : MonoBehaviour
{
    [field: Header("Skins Chooser")]
    [field: SerializeField] bool isPlayer = false;
    [field: SerializeField] List<GameObject> skinsForPlane;
    [field: SerializeField] List<GameObject> bulletSpawnPoints;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            skinsForPlane.Add(child.gameObject);
        }
    }
    private void Start()
    {
        if (isPlayer)
        {
            ChooseSkinForPlayer();
        }
        else
        {
            ChooseSkinForAI();
        }
    }

    private void ChooseSkinForPlayer()
    {
        int skinChosen = 0;
        for(int i = 0; i < SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo.Count; i++)
        {
            if (SkinsOwnershipDataHandler.instance.ReturnSavedValues().skinsInfo[i].isEquipped)
            {
                skinChosen = i;
                break;
            }
        }
        WorkWithSkinValues(skinChosen);
    }

    private void ChooseSkinForAI()
    {
        int skinChosen = Random.Range(0, skinsForPlane.Count);
        WorkWithSkinValues(skinChosen);
    }

    private void WorkWithSkinValues(int skinChosen)
    {
        skinsForPlane[skinChosen].SetActive(true);
        //GetComponent<PlaneModelSpecs>().pointsOfBullet = new List<GameObject>();
        //foreach (GameObject go in GetComponent<SkinsPersonalInfo>().bulletInstantiatingPoint)
        //{
        //    GetComponent<PlaneModelSpecs>().pointsOfBullet.Add(go);
        //}
        FindBulletSpawnPoints(skinsForPlane[skinChosen]);
    }

    void FindBulletSpawnPoints(GameObject parentGameObject)
    {
        Transform[] childTransforms = parentGameObject.GetComponentsInChildren<Transform>();

        bulletSpawnPoints = new List<GameObject>();

        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.name.StartsWith("Bullet Spawn Point"))
            {
                bulletSpawnPoints.Add(childTransform.gameObject);
            }
        }

        if(bulletSpawnPoints.Count > 0)
        {
            GetComponent<PlaneModelSpecs>().pointsOfBullet = new List<GameObject>();
            foreach (GameObject go in bulletSpawnPoints)
            {
                GetComponent<PlaneModelSpecs>().pointsOfBullet.Add(go);
            }
        }
        else
        {
            HelperClass.DebugError("No Bullet Spawn Points Found in " + gameObject.name + "!");
        }
    }
}
