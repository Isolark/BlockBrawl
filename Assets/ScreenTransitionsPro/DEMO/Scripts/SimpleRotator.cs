using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
	public Vector3 rotationPerAxis; 

    void Update()
    {
		transform.Rotate(rotationPerAxis * Time.deltaTime, Space.World);
    }
}
