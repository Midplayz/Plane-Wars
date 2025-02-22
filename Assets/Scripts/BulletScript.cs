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
    public int damage = 10;
    public bool followCrosshair = true;
    Vector3 targetDirection;

    [field: Header("For AI")]
    public bool isAI = false;
    public GameObject whichAI;

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
            if(!followCrosshair) 
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
        if(isAI)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        #endregion
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Explode") && !other.gameObject.CompareTag("Bounds"))
        {
            if (whichAI == null)
            {
                print(other.gameObject.tag + " Name: " + other.gameObject.name + " from: Player");
            }
            else
            {
                print(other.gameObject.tag + " Name: " + other.gameObject.name + " from: " + whichAI.name);
            }
        }
        if (!isAI && other.gameObject.CompareTag("Target"))
        {
            ScoreTracker.Instance.VisualizeImpact();
            if (other.gameObject.GetComponent<OpponentML>().health - damage <=0)
            {
                Debug.Log("This is calledSS");
                other.gameObject.GetComponent<OpponentML>().PlaneDestroyed(-1f);
                ScoreTracker.Instance.UpdateScore();
            }
            else
            {
                other.gameObject.GetComponent<OpponentML>().ReduceHealth(damage);
            }
            Destroy(gameObject);
        }
        else if(isAI && other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<OpponentAIScript>().health - damage <= 0)
            {
                OnPlaneDestroyed(other.gameObject);
                //whichAI.GetComponent<OpponentML>().AdjustReward(2);
                ScoreTracker.Instance.UpdateDeaths();
            }
            else
            {
                other.gameObject.GetComponent<OpponentAIScript>().ReduceHealth(damage);
                //whichAI.GetComponent<OpponentML>().AdjustReward(1);
            }
            Destroy(gameObject);
        }
        else if (isAI && other.gameObject.CompareTag("Target"))
        {
            if (other.gameObject.GetComponent<OpponentML>().health - damage <= 0)
            {
                //OnPlaneDestroyed(other.gameObject);
                //whichAI.GetComponent<OpponentML>().AdjustReward(2);
            }
            {
                other.gameObject.GetComponent<OpponentML>().ReduceHealth(damage);
                //whichAI.GetComponent<OpponentML>().AdjustReward(1);
            }
            Destroy(gameObject);
        }
        if(other.gameObject.CompareTag("Explode"))
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
