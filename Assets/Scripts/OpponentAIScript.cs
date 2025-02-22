using UnityEngine;
using System.Threading.Tasks;
public class OpponentAIScript : MonoBehaviour
{
    [Header("------ Opponent Script ------")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float changeInterval = 2f;
    [SerializeField] private GameObject boomParticle;
    public int health = 100;
    [SerializeField] private bool move = true;
    public GameObject target;

    [field: Header("------ Spawn Bullets ------")]
    [field: SerializeField] GameObject bulletPrefab;
    [field: SerializeField] float spawnInterval = 1f;
    [field: SerializeField] float bulletLifetime = 1f;
    public bool shootBullets = true;
    private float shootTimer;

    [field: Header("------ Spawn Bullets ------")]
    [field: SerializeField] int respawnTime = 2;
    [field: SerializeField] int spawnProtectionTime = 2;

    private float timer;
    private Vector3 direction;

    private float xMin = -120f;
    private float xMax = 120f;
    private float zMin = -120f;
    private float zMax = 120f;
    private float yMin = 20f;
    private float yMax = 110f;

    private Quaternion targetRotation;

    private void Start()
    {
        moveSpeed = GetComponent<PlaneModelSpecs>().movementSpeed;
        spawnInterval = GetComponent<PlaneModelSpecs>().fireRate;
        health = GetComponent<PlaneModelSpecs>().health;

        timer = changeInterval;
        direction = GetRandomDirection();
    }

    private void Update() 
    {
        if (move)
        {
            #region For Randomizing Movement
            if (target != null && !target.activeInHierarchy)
            {
                target = null;
            }
            if (target == null)
            {
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    direction = GetRandomDirection();
                    timer = Random.Range(changeInterval, changeInterval + 2f);
                }

                Vector3 newPosition = transform.position + direction;
                newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
                newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);
                newPosition.y = Mathf.Clamp(newPosition.y, yMin, yMax);
                transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);

                Quaternion currentRotation = transform.rotation;
                targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * 5f);
            }
            else
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                GetComponent<Rigidbody>().MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

                Quaternion currentRotation = transform.rotation;
                targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * 5f);
            }
            #endregion

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
                    newObject.GetComponent<BulletScript>().isAI = false;
                    Physics.IgnoreCollision(GetComponent<Collider>(), newObject.GetComponent<Collider>());
                    Destroy(newObject, bulletLifetime);
                }
            }
            #endregion
        }
    }

    private Vector3 GetRandomDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        return new Vector3(randomX, randomY, randomZ).normalized;
    }

    private void OnDestroy()
    {
        GameManager.Instance.allPlanesInGame.Remove(this.gameObject);
    }

    private void OnDisable()
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

    public void ReduceHealth(int reductionAmt)
    {
        health -= reductionAmt;
    }

    private void RespawnPlane()
    {   GetComponent<Collider>().enabled = false;
        move = false;
        shootBullets = false;
        target = null;

        Invoke("SetRandomPosition", respawnTime);
        Invoke("SpawnProtection", spawnProtectionTime);
    }

    private void SetRandomPosition()
    {
        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);
        transform.position = new Vector3(randomX, 70f, randomZ);
        health = GetComponent<PlaneModelSpecs>().health;
        gameObject.SetActive(true);
        move = true;
        shootBullets = true;
    }

    private void SpawnProtection ()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Explode"))// || collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
