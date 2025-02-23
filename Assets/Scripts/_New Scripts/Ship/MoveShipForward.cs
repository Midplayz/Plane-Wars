using UnityEngine;

public class MoveShipForward : MonoBehaviour
{
    [field: SerializeField] private float speed = 1.0f; 

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
