using UnityEngine;

public class LocalRot : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0.0f, 50.0f, 0.0f);

    private Vector3 currentRotation;

    private bool startRotation = false;

    private void OnEnable()
    {
        startRotation = true;
    }

    private void OnDisable()
    {
        startRotation = false;
    }

    void Start()
    {
        currentRotation = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        if (startRotation)
        {
            currentRotation.x += rotationSpeed.x * Time.deltaTime;
            currentRotation.y += rotationSpeed.y * Time.deltaTime;
            currentRotation.z += rotationSpeed.z * Time.deltaTime;

            Quaternion newRotation = Quaternion.Euler(currentRotation);

            transform.localRotation = newRotation;
        }
    }
}
