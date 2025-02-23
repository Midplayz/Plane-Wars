using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using System.Reflection;

public class MLAgentController : Agent
{
    [Header("------ Opponent Script ------")]
    public int health = 100;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float changeInterval = 2f;
    [SerializeField] private GameObject boomParticle;
    [SerializeField] private bool move = true;
    [SerializeField] private bool moveSideways = true;
    [field: SerializeField] float pitchSpeed = 50;
    [field: SerializeField] float yawSpeed = 50;
    [field: SerializeField] float rollSpeed = 50;
    [field: SerializeField] float returnSpeed = 2.0f;
    [field: SerializeField] AnimationCurve curve;
    public int currentScore = 0;

    private float yaw, roll, pitch;

    [field: Header("------ Spawn Bullets ------")]
    [field: SerializeField] GameObject bulletPrefab;
    [field: SerializeField] float spawnInterval = 1f;
    [field: SerializeField] float bulletLifetime = 1f;
    public bool shootBullets = true;
    private float shootTimer;

    [field: Header("------ Spawn Bullets ------")]
    [field: SerializeField] int respawnTime = 2;
    [field: SerializeField] int spawnProtectionTime = 2;

    [field: Header("------ ML Stuff ------")]
    [field: SerializeField][Range(0f, 1f)] float minimumDotProductForAutoAim;
    private float newX;
    private float newY;


    private float xMin = -190f;
    private float xMax = 190f;
    private float zMin = -190f;
    private float zMax = 190f;
    private float yMin = 20f;
    private float yMax = 110f;

    bool isGameOverNow = false;

    private void Start()
    {
        GameManager.Instance.onMatchFinished += () => { shootBullets = false; isGameOverNow = true; };
        currentScore = Random.Range(0, 4);
    }
    public override void OnEpisodeBegin()
    {
#if UNITY_EDITOR
        ClearLog();
#endif
        moveSpeed = GetComponent<PlaneModelSpecs>().movementSpeed;
        spawnInterval = GetComponent<PlaneModelSpecs>().fireRate;
        health = GetComponent<PlaneModelSpecs>().health;
        transform.LookAt(Vector3.zero);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Transform closestEnemy = GetClosestEnemy(GameManager.Instance.allPlanesInGame);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        Vector3 enemyPos = closestEnemy.position;
        sensor.AddObservation(enemyPos);
        sensor.AddObservation(Vector3.Dot(transform.forward, (GetClosestEnemy(GameManager.Instance.allPlanesInGame).position - transform.position).normalized));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (move)
        {
            if (moveSideways)
            {
                newX = actions.ContinuousActions[0];
                newY = actions.ContinuousActions[1];
            }
            else
            {
                newX = 0;
                newY = 0;
            }

            float translation = moveSpeed;
            if (Mathf.Abs(newY) > 0.05f)
            {
                pitch = -newY * pitchSpeed;
            }
            else
            {
                newY = 0;
            }

            yaw = newX * yawSpeed;
            roll = newX * -rollSpeed;

            translation *= Time.deltaTime;
            pitch *= Time.deltaTime;
            yaw *= Time.deltaTime;
            roll *= Time.deltaTime;
            transform.Translate(Vector3.forward * translation, Space.Self);
            if (newY == 0 && newX != 0)
            {
                transform.Rotate(pitch, yaw, 0);
            }
            if (newY != 0 && newX == 0)
            {
                transform.Rotate(pitch, 0, roll);
            }
            if (newY != 0 && newX != 0)
            {
                transform.Rotate(pitch, yaw, roll);
            }

            //Dot Product Based System
            AdjustReward(DotProductRewardingSystem());
        }
    }
    public void AdjustReward(float amount)
    {
        AddReward(amount);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
    private void Update()
    {
        if (shootBullets)
        {
            #region Spawning Bullets
            shootTimer += Time.deltaTime;

            if (shootBullets && shootTimer >= spawnInterval)
            {
                shootTimer = 0f;
                for (int i = 0; i < GetComponent<PlaneModelSpecs>().pointsOfBullet.Count; i++)
                {
                    Vector3 position = GetComponent<PlaneModelSpecs>().pointsOfBullet[i].transform.position;
                    Quaternion rotation = GetComponent<PlaneModelSpecs>().pointsOfBullet[i].transform.rotation;
                    GameObject newObject = Instantiate(bulletPrefab, position, rotation);
                    newObject.GetComponent<BulletScript>().damage = GetComponent<PlaneModelSpecs>().bulletDamage;
                    newObject.GetComponent<BulletScript>().isAI = true;
                    newObject.GetComponent<BulletScript>().whichAI = gameObject;

                    #region Assigning Target
                    float currentDotProduct = Vector3.Dot(transform.forward, (GetClosestEnemy(GameManager.Instance.allPlanesInGame).position - transform.position).normalized);
                    if (GetClosestEnemy(GameManager.Instance.allPlanesInGame).TryGetComponent<PlaneMovement>(out PlaneMovement planeMovement) && currentDotProduct >= minimumDotProductForAutoAim && currentDotProduct <= 1f)
                    {
                        newObject.GetComponent<BulletScript>().followObject = GetClosestEnemy(GameManager.Instance.allPlanesInGame).gameObject;
                    }
                    else if(GetClosestEnemy(GameManager.Instance.allPlanesInGame).TryGetComponent<MLAgentController>(out MLAgentController mLAgent) && currentDotProduct >= 0.25 && currentDotProduct <= 1f)
                    {
                        newObject.GetComponent<BulletScript>().followObject = GetClosestEnemy(GameManager.Instance.allPlanesInGame).gameObject;
                    }
                    #endregion

                    Physics.IgnoreCollision(GetComponent<Collider>(), newObject.GetComponent<Collider>());
                    Destroy(newObject, bulletLifetime);
                }
            }
            #endregion
        }
    }

    public void ReduceHealth(int reductionAmt)
    {
        health -= reductionAmt;
    }

    private void RespawnPlane()
    {
        GetComponent<Collider>().enabled = false;
        move = false;
        shootBullets = false;

        Invoke("SetRandomPosition", respawnTime);
    }

    private void SetRandomPosition()
    {
        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);
        transform.position = new Vector3(randomX, 70f, randomZ);
        health = GetComponent<PlaneModelSpecs>().health;
        gameObject.SetActive(true);
        move = true;
        if (!isGameOverNow)
        {
            shootBullets = true;
        }
        Invoke("SpawnProtection", spawnProtectionTime);
    }
    private void SpawnProtection()
    {
        GetComponent<Collider>().enabled = true;
    }
    public void PlaneDestroyed(float reward)
    {
        //SetReward(reward);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Explode") || collision.gameObject.CompareTag("Bounds"))
        {
            PlaneDestroyed(-1f);
        }
    }

    Transform GetClosestEnemy(List<GameObject> enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            if (t == gameObject)
            {
                continue;
            }
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist && Vector3.Dot(transform.forward, (t.transform.position - transform.position).normalized) > 0)
            {
                tMin = t.transform;
                minDist = dist;
            }
            else if(dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        if(tMin == null)
        {
            int randomEnemy = Random.Range(0, enemies.Count);
            while (enemies[randomEnemy] == gameObject)
            {
                randomEnemy = Random.Range(0, enemies.Count);
            }
                tMin = enemies[randomEnemy].transform;
        }
        return tMin;
    }

    private float DotProductRewardingSystem()
    {
        float currentDotProduct = Vector3.Dot(transform.forward, (GetClosestEnemy(GameManager.Instance.allPlanesInGame).position - transform.position).normalized);

        #region Assigning Target
        if (currentDotProduct >= 0.75f && currentDotProduct <= 1f)
        {

        }
        #endregion

        float curveValue = curve.Evaluate(currentDotProduct);
        return curveValue;
    }

    private float NormalizeFloatValues(float value, float min, float max)
    {
        float normalizedValue;
        normalizedValue = (value - min) / (max - min);
        return normalizedValue;
    }

#if UNITY_EDITOR
    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
#endif

    //OnDisable
    private  void OnDisable() 
    {
        if (!GameManager.Instance.ChangingScenes)
        {
            Vector3 position = gameObject.transform.localPosition;
            Quaternion rotation = gameObject.transform.rotation;
            GameObject newObject = Instantiate(boomParticle, position, rotation);
            newObject.GetComponent<ParticleSystem>().Play();
            Destroy(newObject, 2f);
            RespawnPlane();
        }
    }
}