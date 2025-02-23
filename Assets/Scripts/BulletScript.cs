using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletScript : MonoBehaviour
{
    [field: Header("Bullet Specs")]
    [field: SerializeField] float speed = 300f;
    [field: SerializeField] GameObject boomParticle;
    public GameObject followObject;
    public GameObject whichAI;
    public int damage = 10;
    public bool followCrosshair = true;
    Vector3 targetDirection;

    [field: Header("For AI")]
    public bool isAI = false;

    private void Start()
    {
        if (!isAI && followObject != null && followCrosshair)
        {
            targetDirection = followObject.transform.position;
        }
    }
    private void Update()
    {
        #region Following Crosshair 
        if (!isAI)
        {
            if (!followCrosshair)
            {
                targetDirection = followObject.transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetDirection, speed * Time.deltaTime);
            if (Mathf.Approximately(Vector3.Distance(transform.position, targetDirection), 0))
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region For AI
        if (isAI && followObject == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, followObject.transform.position, speed * Time.deltaTime);
        }
        #endregion
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isAI && other.gameObject.CompareTag("Target"))
        {
            ScoreTracker.Instance.VisualizeImpact();
            if (other.gameObject.GetComponent<MLAgentController>().health - damage <= 0)
            {
                OnPlaneDestroyed(other.gameObject);
                ScoreTracker.Instance.UpdateScore();
                MissionTracker.instance.AdjustValues(Quest.ShootDownPlanes);
            }
            else
            {
                other.gameObject.GetComponent<MLAgentController>().ReduceHealth(damage);
            }
            Destroy(gameObject);
        }
        else if (isAI && other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PlaneMovement>().playerHealth - damage <= 0)
            {
                OnPlaneDestroyed(other.gameObject);
                ScoreTracker.Instance.UpdateDeaths();
                whichAI.GetComponent<MLAgentController>().currentScore += ScoreTracker.Instance.ReturnScorePerKill();
            }
            other.gameObject.GetComponent<PlaneMovement>().ReduceHealth(damage);
            Destroy(gameObject);
        }
        else if (isAI && other.gameObject.CompareTag("Target"))
        {
            if (other.gameObject.GetComponent<MLAgentController>().health - damage <= 0)
            {
                OnPlaneDestroyed(other.gameObject);
                whichAI.GetComponent<MLAgentController>().currentScore += ScoreTracker.Instance.ReturnScorePerKill();
            }
            other.gameObject.GetComponent<MLAgentController>().ReduceHealth(damage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Explode"))
        {
            Destroy(gameObject);
        }
    }

    private void OnPlaneDestroyed(GameObject target)
    {
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;
        GameObject newObject = Instantiate(boomParticle, position, rotation);
        newObject.GetComponent<ParticleSystem>().Play();
        Destroy(newObject, 2f);
        target.SetActive(false);
    }

}
