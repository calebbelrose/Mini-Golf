using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
	private Rigidbody rb;
	private Vector3 torque = new Vector3(0.0f, 5.0f, 0.0f);
	private bool flipping = false;
	private Vector3 maxAngle = new Vector3(0.0f, 45.0f, 0.0f);

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		Physics.IgnoreLayerCollision(10, 10, true);
		rb.centerOfMass = Vector3.zero;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 9)
		{
			rb.AddTorque (torque, ForceMode.VelocityChange);
			flipping = true;
		}
	}

	void Update()
	{
		
		if (Input.GetKey (KeyCode.Z))
		{
			rb.AddTorque (torque, ForceMode.VelocityChange);
			flipping = true;
		}
		else if (transform.eulerAngles.y > 45.0f)
		{
			rb.Sleep ();
			transform.rotation = Quaternion.identity;
			flipping = false;
		}

		if (flipping)
		{
			transform.rotation = new Quaternion (0.0f, transform.rotation.y, 0.0f, transform.rotation.w);

			if (transform.eulerAngles.y > 45.0f)
				transform.eulerAngles = maxAngle;
		}
	}
}
