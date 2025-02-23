using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateComponent : MonoBehaviour
{
    [field: Header("Rotate Any Gameobject")]
    [field: SerializeField] float speed;

    void Update()
    {
      transform.Rotate(new Vector3(0,speed*Time.deltaTime,0));  
    }
}
