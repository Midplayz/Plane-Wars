using UnityEngine;
using System.Threading.Tasks;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using System.Collections.Generic;

public class OpponentML : Agent
{
    [Header("------ Opponent Script ------")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float changeInterval = 2f;
    [SerializeField] private GameObject boomParticle;
    public int health = 100;
    [SerializeField] private bool move = true;
    [SerializeField] private GameObject targettedPlane;
    [field: SerializeField] float pitchSpeed = 50;
    [field: SerializeField] float yawSpeed = 50;
    [field: SerializeField] float rollSpeed = 50;
    [field: SerializeField] float returnSpeed = 2.0f;
    [field: SerializeField] AnimationCurve curve;
    [field: SerializeField] GameObject currentTarget;
    [field: SerializeField] LineRenderer LineRenderer;

    private float yaw, roll, pitch;
    private float maxHorizontal = 1f;
    private float maxVertical = 1f;

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
    private float newX;
    private float newY;
    [field: SerializeField] private Vector3 targetsPosition;


    private float xMin = -190f;
    private float xMax = 190f;
    private float zMin = -190f;
    private float zMax = 190f;
    private float yMin = 20f;
    private float yMax = 110f;

    private float minReward = -1.0f;  
    private float maxReward = 1.0f;
    private Quaternion targetRotation;

    float timer = 0f;
    private void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }
    public override void OnEpisodeBegin()
    {
        moveSpeed = GetComponent<PlaneModelSpecs>().movementSpeed;
        spawnInterval = GetComponent<PlaneModelSpecs>().fireRate;
        health = GetComponent<PlaneModelSpecs>().health;
        timer = 0f;
        RespawnPlane();
    }
    public void UpdateTarget(GameObject target)
    {
        targetsPosition = target.transform.position;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Transform closestEnemy = GetClosestEnemy(GameManager.Instance.allPlanesInGame);
        currentTarget = closestEnemy.gameObject;
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(closestEnemy.position);
        //sensor.AddObservation((closestEnemy.position - transform.position));
        //sensor.AddObservation(Vector3.Distance(transform.position, closestEnemy.position));
        sensor.AddObservation(Vector3.Dot(transform.forward, (GetClosestEnemy(GameManager.Instance.allPlanesInGame).position - transform.position).normalized));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (move)
        {
            newX = actions.ContinuousActions[0];
            newY = actions.ContinuousActions[1];

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
            if(newY != 0 && newX == 0)
            {
                transform.Rotate(pitch, 0, roll);
            }
            if (newY != 0 && newX != 0)
            {
                transform.Rotate(pitch, yaw, roll);
            }

            //New Dot Product Based System
            AdjustReward(DotProductRewardingSystem());

            //Dot Product based
            //float currentDotProduct = Vector3.Dot(transform.forward, (GetClosestEnemy(GameManager.Instance.allPlanesInGame).position - transform.position).normalized);
            //print(currentDotProduct);
            //AdjustReward(currentDotProduct * currentDotProduct* (currentDotProduct>0?1:-1));

            //Distance Based Rewarding
            //bool isFacingTarget = IsFollowingTarget(gameObject.transform, GetClosestEnemy(GameManager.Instance.allPlanesInGame).transform, ref previousPosition);
            //if (isFacingTarget)
            //{
            //    AdjustReward(+1);
            //}
            //else
            //{
            //    AdjustReward(-1f);
            //}

            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, transform.position);
            LineRenderer.SetPosition(1, GetClosestEnemy(GameManager.Instance.allPlanesInGame).transform.position);
        }
    }
    public void AdjustReward(float amount)
    {
        if(amount < 0)
        {
            //Debug.Log("Punished for: " + amount);
        }
        else if(amount > 0)
        {
            //Debug.Log("Rewarded for: " + amount);
        }
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
        timer += Time.deltaTime;
        if(timer> 10f)
        {
            EndEpisode();
        }
        if (move)
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
        Invoke("SetRandomPosition", respawnTime);
        Invoke("SpawnProtection", spawnProtectionTime);
    }

    private void SetRandomPosition()
    {
        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);
        transform.position = new Vector3(randomX, 70f, randomZ);
        health = GetComponent<PlaneModelSpecs>().health;
    }
    private void SpawnProtection()
    {
        GetComponent<Collider>().enabled = true;
    }
    public void PlaneDestroyed(float reward)
    {
        SetReward(reward);
        EndEpisode();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Explode") || collision.gameObject.CompareTag("Bounds"))
        {
            //PlaneDestroyed(-1f);
        }
    }

    private bool IsWithinBounds()
    {
        if (transform.position.x < xMin || transform.position.x > xMax || transform.position.y < yMin || transform.position.y > yMax || transform.position.z < zMin || transform.position.z > zMax)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    Transform GetClosestEnemy(List<GameObject> enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            if(t == gameObject)
            {
                continue;
            }
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    private bool IsFollowingTarget(Transform follower, Transform target, ref Vector3 previousPosition) //Using Distance is useless btw
    {
        float currentDistanceToTarget = Vector3.Distance(target.position, follower.position);
        float previousDistanceToTarget = Vector3.Distance(previousPosition, target.position);

        previousPosition = follower.position;
        return currentDistanceToTarget < previousDistanceToTarget? true: false;
    }

    private float DotProductRewardingSystem()
    {
        float currentDotProduct = Vector3.Dot(transform.forward, (GetClosestEnemy(GameManager.Instance.allPlanesInGame).position - transform.position).normalized);

        float curveValue = curve.Evaluate(currentDotProduct);
        float mappedValue = Mathf.Lerp(minReward, maxReward, curveValue);
        Debug.Log("Dot Prod: " + currentDotProduct + " Reward: " + mappedValue);
        return mappedValue;
    }
}
