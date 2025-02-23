using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public GameObject[] objectList;
    public int maxSpawnAttempts = 10;
    public Transform planeTransform;
    public int numberOfObjectsToSpawn = 5; // Specify the number of objects to spawn

    private float planeWidth;
    private float planeLength;

    private void Start()
    {
        Renderer planeRenderer = planeTransform.GetComponent<Renderer>();
        planeWidth = planeRenderer.bounds.size.x;
        planeLength = planeRenderer.bounds.size.z;

        SpawnGameObjects();
    }

    private void SpawnGameObjects()
    {
        int objectsSpawned = 0;

        while (objectsSpawned < numberOfObjectsToSpawn)
        {
            GameObject randomObject = GetRandomObject();
            if (randomObject != null)
            {
                SpawnObject(randomObject);
                objectsSpawned++;
            }
            else
            {
                Debug.LogWarning("Unable to find a valid object to spawn");
                break;
            }
        }
    }

    private GameObject GetRandomObject()
    {
        if (objectList.Length == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, objectList.Length);
        return objectList[randomIndex];
    }

    private void SpawnObject(GameObject obj)
    {
        Vector3 spawnPosition = GetRandomPosition();

        bool isValidPosition = CheckValidPosition(spawnPosition, obj);
        int spawnAttempts = 0;

        while (!isValidPosition && spawnAttempts < maxSpawnAttempts)
        {
            spawnPosition = GetRandomPosition();
            isValidPosition = CheckValidPosition(spawnPosition, obj);
            spawnAttempts++;
        }

        if (isValidPosition)
        {
            var objSpawned = Instantiate(obj, spawnPosition, Quaternion.identity);
            objSpawned.transform.parent = this.transform;
            objSpawned.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            objSpawned.transform.position = new Vector3(objSpawned.transform.position.x, objSpawned.transform.position.y + objSpawned.transform.localScale.y, objSpawned.transform.position.z);
            if(!objSpawned.transform.CompareTag("Explode"))
            {
                objSpawned.transform.tag = "Explode";
            }
            if(!TryGetComponent<MeshCollider>(out MeshCollider collider))
            {
                objSpawned.AddComponent<MeshCollider>();
            }
        }
        else
        {
            Debug.LogWarning("Unable to find a valid position to spawn " + obj.name);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(planeTransform.position.x - planeWidth / 2f, planeTransform.position.x + planeWidth / 2f);
        float randomZ = Random.Range(planeTransform.position.z - planeLength / 2f, planeTransform.position.z + planeLength / 2f);
        float randomY = planeTransform.position.y;

        return new Vector3(randomX, randomY, randomZ);
    }

    private bool CheckValidPosition(Vector3 position, GameObject obj)
    {
        float radius;
        if(obj.transform.localScale.x > obj.transform.localScale.z)
        {
            radius = obj.transform.localScale.x;
        }
        else
        {
            radius = obj.transform.localScale.z;
        }
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Explode"))
            {
                return false;
            }
        }

        return true;
    }
}
