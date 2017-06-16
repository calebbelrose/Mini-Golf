using UnityEngine;
using System.Collections;

public class windmillRotation : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
		// Rotates the windmill
		transform.Rotate(0.0f, 0.0f, Mathf.LerpAngle(0.0f, 45.0f, Time.deltaTime));
    }
}
