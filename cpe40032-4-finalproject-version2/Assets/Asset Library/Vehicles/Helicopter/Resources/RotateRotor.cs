using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRotor : MonoBehaviour
{

    private float rotationSpeed = 1000;

    void Update()
    {
        // Rotates the rotor
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
