using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HomingMissileScript : MonoBehaviour
{
    [field: Header("------ Homing Missile Controller ------")]
    [field: SerializeField] float speed = 5f;
    [field: SerializeField] float homingTime = 2f;
    [field: SerializeField] GameObject boomParticle;
    public Transform target;

    private float timer = 0f;
    private float distance;
    private float step;

    private void Update()
    {
        if (target != null)
        {
            timer += Time.deltaTime;

            //if (timer <= homingTime)
            //{
            Vector3 direction = target.position - transform.position;

            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * speed);

            distance = direction.magnitude;
            step = speed * Time.deltaTime;
            //}
            //else if( timer >= homingTime * 3)
            // {
            //Destroy(gameObject);
            //}

            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            Vector3 position = gameObject.transform.position;
            Quaternion rotation = gameObject.transform.rotation;
            GameObject newObject = Instantiate(boomParticle, position, rotation);
            newObject.GetComponent<ParticleSystem>().Play();
            Destroy(newObject, 2f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
