using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class PlaneModelSpecs : MonoBehaviour 
{
    public int health;
    public float fireRate;
    public int bulletDamage;
    public float movementSpeed;
    public float maneuverability;
    public List<GameObject> pointsOfBullet;
}
