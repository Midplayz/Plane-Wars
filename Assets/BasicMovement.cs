using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float changeInterval = 2f;
    [SerializeField] private bool move = true;
    [field: SerializeField] float pitchSpeed = 50;
    [field: SerializeField] float yawSpeed = 50;
    [field: SerializeField] float rollSpeed = 50;
    [field: SerializeField] float returnSpeed = 2.0f;
    private float yaw, roll, pitch;
    float newX, newY;

    private float minX = -200f;
    private float maxX = 200f;
    private float minZ = -200f;
    private float maxZ = 200f;
    private float minY = 10f;
    private float maxY = 150f;
    private void Start()
    {
        StartCoroutine(ChangeDir());
    }
    // Update is called once per frame
    void Update()
    {
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
        Vector3 currentPosition = transform.position;
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minZ, maxZ);
        transform.position = currentPosition;
    }

    private IEnumerator ChangeDir()
    {
        while (true)
        {
            float targetX = Random.Range(-1f, 1f);
            targetX = (Mathf.Round(targetX * 10) / 10);
            float targetY = Random.Range(-1f, 1f);
            targetY = (Mathf.Round(targetY * 10) / 10);

            float elapsedTime = 0f;
            while (elapsedTime < changeInterval)
            {
                newX = Mathf.Lerp(newX, targetX, elapsedTime / changeInterval);
                newY = Mathf.Lerp(newY, targetY, elapsedTime / changeInterval);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            newX = targetX;
            newY = targetY;

            yield return new WaitForSeconds(changeInterval);
        }
    }
}
