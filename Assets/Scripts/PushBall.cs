using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBall : MonoBehaviour
{
	private Vector3 force = new Vector3 (0.0f, 1000.0f, 0.0f);

	void OnCollisionEnter(Collision coll)
	{
		if (coll.transform.gameObject.layer == 9)
			coll.transform.GetComponent<Rigidbody> ().AddForce (force);
	}
}
