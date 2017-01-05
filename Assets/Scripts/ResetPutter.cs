using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResetPutter : MonoBehaviour
{

    public GameObject putter, ballObject, camera;							// Putter, golf ball and camera objects
    private GameObject hole;												// Hole object
    private Rigidbody rb;													// Rigid body for the golf ball
    int holeNumber = 0;														// The current hole number
    private Vector3 cameraOffset, prevPos;									// Offset for the camera and position of the ball before it was shot
    bool needReset = true, inTheHole = false;								// Whether the ball needs to be reset and whether it's in the hole
    float[] holeLocation = new float[] { 0, -23, -86, -147, -172, -186 };	// Location of each hole
	PlayerController pc;													// Player controller

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
		pc = putter.GetComponent<PlayerController>();
        NewHole();
        prevPos = transform.position;

		// Allows the putter to only collide with the golf ball
		Physics.IgnoreLayerCollision(8, 10, true);
	}

    // Update is called once per frame
    void Update()
    {
		// Moves the camera if the ball is hit and determines that the ball needs to be reset
        if (transform.position != prevPos)
        {
            camera.transform.position = transform.position + cameraOffset;
            needReset = true;
        }
		// Resets the golf ball if it was shot and has stopped moving
        else if (rb.IsSleeping() && needReset)
        {
            putter.SetActive(true);
			if (inTheHole) 
			{
				inTheHole = false;
				pc.scoreboard.SetActive(true);
				GameObject.Find("txtScore" + holeNumber).GetComponent<Text> ().text = pc.strokes.ToString();
				NewHole();
			}
			else if (pc.strokes == 10)
			{
				pc.scoreboard.SetActive(true);
				GameObject.Find("txtScore" + holeNumber).GetComponent<Text> ().text = 10.ToString();
				NewHole();
			}
            NewShot();
        }
        prevPos = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
		// Determines whether the ball is in the hole or not
        if (collision.collider.gameObject.name == ("hole" + holeNumber))
            inTheHole = true;
        else
            inTheHole = false;
    }

	// Moves the ball to the start of the next hole and resets the strokes
    public void NewHole()
    {
		pc.scoreboard.SetActive(false);
        transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        ballObject.transform.position = new Vector3(holeLocation[holeNumber], 0f, -44.5f);
        holeNumber++;
        hole = GameObject.Find("hole" + holeNumber);
		pc.strokes = 0;
    }
    
	// Moves the putter and camera to line up with the golf ball and hole
    public void NewShot()
    {
        float numerator1, numerator2, denominator, xDistance, zDistance;

		needReset = false;
        ballObject.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        numerator1 = hole.transform.position.x - transform.position.x;
        numerator2 = hole.transform.position.z - transform.position.z;
        denominator = Mathf.Abs((float)System.Math.Sqrt(numerator1 * numerator1 + numerator2 * numerator2));
        xDistance = numerator1 / denominator;
        zDistance = numerator2 / denominator;
        putter.transform.position = new Vector3(transform.position.x - 0.5f * xDistance, ballObject.transform.position.y, transform.position.z - 0.5f * zDistance);
        camera.transform.position = new Vector3(transform.position.x - 35.5f * xDistance, ballObject.transform.position.y + 20f, transform.position.z - 35.5f * zDistance);
        putter.transform.LookAt(hole.transform);
        putter.transform.rotation = new Quaternion(0f, putter.transform.rotation.y, putter.transform.rotation.z, putter.transform.rotation.w);
        camera.transform.LookAt(ballObject.transform);
        camera.transform.Rotate(-19.378f, 0f, 0f);
        cameraOffset = camera.transform.position - transform.position;
        pc.SetClamps(xDistance, zDistance);
		pc.target = hole.transform.position;
    }
}