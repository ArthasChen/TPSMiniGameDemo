using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotate : MonoBehaviour
{
    private float rotate = 0;
    private void Update()
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");
        rotate += 180 * axis;
        transform.Rotate(new Vector3(0, 0, rotate));
    }
}
