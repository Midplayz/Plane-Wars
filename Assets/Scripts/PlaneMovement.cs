using Unity.Cinemachine;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlaneMovement : MonoBehaviour
{
    [field: Header("------ Plane Movement ------")]
    [field: SerializeField] float speed = 20;
    [field: SerializeField] float pitchSpeed = 50;
    [field: SerializeField] float yawSpeed = 50;
    [field: SerializeField] float rollSpeed = 50;
    [field: SerializeField] float returnSpeed = 2.0f;
    [field: SerializeField] FloatingJoystick joystick;

    [field: Header("------ Plane Destruction ------")]
    [field: SerializeField] GameObject explosionEffect;
    [field: SerializeField] List<GameObject> planeparts;
    [field: SerializeField] GameObject replayButton;

    [field: Header("------ Things Go Boom! Section ------")]
    [field: SerializeField] GameObject missilePrefab;
    [field: SerializeField] GameObject missileSpawnPoint;
    [field: SerializeField] Image crossHair;
    public bool shootBullets = true;

    [field: Header("------ Health & Damage ------")]
    public int playerHealth;
    [field: SerializeField] TextMeshProUGUI healthText;
    [field: SerializeField] CinemachineCamera virtualCamera;
    [field: SerializeField] float screenShakeDuration;
    [field: SerializeField] GameObject damageImage;
    CinemachineBasicMultiChannelPerlin noise;

    [field: Header("------ Spawn Bullets ------")]
    [field: SerializeField] GameObject bulletPrefab;
    [field: SerializeField] float spawnInterval = 1f;
    [field: SerializeField] float bulletLifeTime = 1f;
    private float timer;

    [field: Header("------ Respawn ------")]
    [field: SerializeField] int respawnTime = 2;
    [field: SerializeField] int spawnProtectionTime = 2;
    [field: SerializeField] bool move = true;
    private float xMin = -120f;
    private float xMax = 120f;
    private float zMin = -120f;
    private float zMax = 120f;

    [field: Header("------ Music ------")]
    [field: SerializeField] AudioSource audioSource;

    private float yaw, roll, pitch;
    private float horizontal, vertical;
    private float maxHorizontal = 1f;
    private float maxVertical = 1f;

    public GameObject cube { get; private set; }
    public GameObject followTarget;

    bool isGameOverNow = false;

    private void Start()
    {
        GameManager.Instance.onMatchFinished += () => { shootBullets = false; joystick.gameObject.SetActive(false); isGameOverNow = true; };

        audioSource = GetComponent<AudioSource>();
        if (SettingsDataHandler.instance.ReturnSavedValues().soundMuted)
        {
            audioSource.enabled = false;
        }

        noise = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;

        speed = GetComponent<PlaneModelSpecs>().movementSpeed;
        pitch = GetComponent<PlaneModelSpecs>().maneuverability;
        roll = GetComponent<PlaneModelSpecs>().maneuverability;
        yaw = GetComponent<PlaneModelSpecs>().maneuverability;
        spawnInterval = GetComponent<PlaneModelSpecs>().fireRate;
        playerHealth = GetComponent<PlaneModelSpecs>().health;

        healthText.text = playerHealth + "%";

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().enabled = false;
        followTarget = cube;

    }
    private void Update()
    {
        if (move)
        {
            cube.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(crossHair.GetComponent<RectTransform>().position.x, crossHair.GetComponent<RectTransform>().position.y, 300f));
            horizontal = Mathf.Clamp(joystick.Horizontal, -maxHorizontal, maxHorizontal);
            vertical = Mathf.Clamp(joystick.Vertical, -maxVertical, maxVertical);

            float translation = speed;
            if (Mathf.Abs(vertical) > 0.05f)
            {
                pitch = -vertical * pitchSpeed;
            }
            else
            {
                vertical = 0;
            }

            if (!isGameOverNow)
            {
                yaw = horizontal * yawSpeed;
                roll = horizontal * -rollSpeed;
            }
            else
            {
                yaw = 0;
                roll = 0;
                pitch = 0;
            }

            translation *= Time.deltaTime;
            pitch *= Time.deltaTime;
            yaw *= Time.deltaTime;
            roll *= Time.deltaTime;
            transform.Translate(Vector3.forward * translation, Space.Self);
            //if (transform.localPosition.y >= 110f && pitch <= 0)
            //{
            //    pitch = 0;
            //}
            if (vertical == 0 && horizontal != 0)
            {
                transform.Rotate(pitch, yaw, 0);
            }
            if (vertical != 0 && horizontal != 0)
            {
                transform.Rotate(pitch, yaw, roll);
            }
            if (horizontal == 0 && vertical == 0)
            {
                Quaternion target = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * returnSpeed);
            }


            #region Raycasts
            if (!crossHair.GetComponent<LockIntoObject>().hasJustLocked)
            {
                RectTransform rectTransform = crossHair.GetComponent<RectTransform>();
                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);
                Vector3[] screenPositions = new Vector3[4];
                for (int i = 0; i < 4; i++)
                {
                    screenPositions[i] = new Vector3(corners[i].x, corners[i].y, 0);
                }

                RaycastHit[] hits = new RaycastHit[4];
                for (int i = 0; i < 4; i++)
                {
                    Ray ray = Camera.main.ScreenPointToRay(screenPositions[i]);
                    Debug.DrawLine(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(screenPositions[i]));
                    if (Physics.Raycast(ray, out hits[i]))
                    {
                        if (hits[i].transform.CompareTag("Target"))
                        {
                            crossHair.GetComponent<LockIntoObject>().targetObject = hits[i].transform.gameObject;
                            followTarget = hits[i].transform.gameObject;
                        }
                    }
                }
            }
            #endregion
            #region Spawning Rockets
            //if (Physics.Raycast(ray, out hit))
            //{
            //    if (hit.transform.CompareTag("Target"))
            //    {
            //        //hit.transform.GetComponent<OpponentAIScript>().hasBeenHit = true;
            //        crossHair.GetComponent<LockIntoObject>().targetObject = hit.transform.gameObject;
            //        followTarget = hit.transform.gameObject;
            //        //SpawnMissile(hit.transform);
            //    }
            //    else
            //    {
            //        followTarget = cube;
            //    }
            //}
            #endregion

            #region Spawning Bullets
            timer += Time.deltaTime;

            if (shootBullets && timer >= spawnInterval)
            {
                timer = 0f;
                for (int i = 0; i < GetComponent<PlaneModelSpecs>().pointsOfBullet.Count; i++)
                {
                    Vector3 position = GetComponent<PlaneModelSpecs>().pointsOfBullet[i].transform.position;
                    Quaternion rotation = GetComponent<PlaneModelSpecs>().pointsOfBullet[i].transform.rotation;
                    GameObject newObject = Instantiate(bulletPrefab, position, rotation);
                    newObject.GetComponent<BulletScript>().damage = GetComponent<PlaneModelSpecs>().bulletDamage;
                    newObject.GetComponent<BulletScript>().followObject = followTarget;
                    if (followTarget != cube)
                    {
                        newObject.GetComponent<BulletScript>().followCrosshair = false;
                    }
                    Destroy(newObject, bulletLifeTime);
                }
            }
            #endregion
        }
    }

    private void OnDisable()
    {
        if (!GameManager.Instance.ChangingScenes)
        {
            OnDisableActions();
        }
    }

    public void ReduceHealth(int reductionAmt)
    {
        TriggerScreenshake(screenShakeDuration);
        playerHealth -= reductionAmt;
        healthText.text = playerHealth + "%";
    }

    private void TriggerScreenshake(float duration)
    {
        noise.AmplitudeGain = 1f;
        noise.FrequencyGain = 10f;
        damageImage.SetActive(true);
        Handheld.Vibrate();

        Invoke("StopScreenshake", duration);
    }

    private void StopScreenshake()
    {
        damageImage.SetActive(false);
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    private void OnDisableActions()
    {
        healthText.text = "0%";
        TriggerScreenshake(screenShakeDuration);
        move = false;
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;
        GameObject newObject = Instantiate(explosionEffect, position, rotation);
        newObject.GetComponent<ParticleSystem>().Play();
        Destroy(newObject, 2f);

        shootBullets = false;

        Invoke("SetRandomPosition", respawnTime);
        Invoke("SpawnProtection", spawnProtectionTime);
    }

    private void SetRandomPosition()
    {
        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);
        transform.position = new Vector3(randomX, 70f, randomZ);
        transform.LookAt(Vector3.zero);
        followTarget = cube;
        playerHealth = GetComponent<PlaneModelSpecs>().health;
        healthText.text = playerHealth + "%";
        gameObject.SetActive(true);
        move = true;

        if(!isGameOverNow)
        {
            shootBullets = true;
        }
    }

    private void SpawnProtection()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Explode"))// || collision.transform.CompareTag("Target"))
        {
            gameObject.SetActive(false);
            if (!isGameOverNow)
            {
                ScoreTracker.Instance.UpdateDeaths();
            }
        }
    }
    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    private void SpawnMissile(Transform target)
    {
        Vector3 position = missileSpawnPoint.transform.position;
        Quaternion rotation = missileSpawnPoint.transform.rotation;
        GameObject newObject = Instantiate(missilePrefab, position, rotation);
        newObject.transform.GetComponent<HomingMissileScript>().target = target;
    }
}