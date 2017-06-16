using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb, golfballRB;						// Rigidbody for the putter and golf ball
    public Vector3 startingPosition;						// Starting position
	public Vector3 target;									// Target for the putter
    float minX, maxX, minZ, maxZ;							// The minimum and maximum position for the putter while shooting
    float xDistance, zDistance;								// x and z offset for 1 unit away from the putter
	float power = 0.0f;										// Power of the shot
	bool newMax = false,									// Whether there's a new min/max x/z
	reversedX, reversedZ;									// Whether the min/max x and the min/max z are reversed
    GUI score;												// GUI used to display the score
    GUIStyle guiStyle;										// Style used to display the power, angle and number of strokes
    public int strokes;										// Number of strokes
    public GameObject golfball, scoreboard;					// Golf ball and scoreboard objects
	public Transform point;
	private Vector3 v;
	private BoxCollider coll;
	public GameObject cam;
	private Rect powerRect = new Rect(0, Screen.height - 30, 100, 30);
	private Rect angleRect = new Rect(0, Screen.height - 60, 100, 30);
	private Rect strokeRect = new Rect(0, Screen.height - 90, 100, 30);
	private ResetPutter rp;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 30;
        golfballRB = golfball.GetComponent<Rigidbody>();
		coll = GetComponent<BoxCollider> ();
		rp = golfball.GetComponent<ResetPutter> ();
    }

    void OnGUI()
    {
		// Displays the power, angle and number of strokes
		GUI.Label(powerRect, "Power: " + power.ToString("0.00"), guiStyle);
		GUI.Label(angleRect, "Angle: " + transform.eulerAngles.y + "°", guiStyle);
        GUI.Label(strokeRect, "Strokes: " + strokes, guiStyle);
    }

	void Update()
	{
		// Adds force to the putter to take the shot and increases the number of strokes
		if (Input.GetKeyDown ("space") && !newMax)
		{
			newMax = true;
			float x, z;
			Vector3 movement;

			if (reversedX)
			{
				x = -(minX - transform.position.x) * xDistance;
				minX += x;
			}
			else
			{
				x = (maxX - transform.position.x) * xDistance;
				maxX += x;
			}

			if (reversedZ)
			{
				z = -(minZ - transform.position.z) * zDistance;
				minZ += z;
			}
			else
			{
				z = (maxZ - transform.position.z) * zDistance;
				maxZ += z;
			}

			movement = new Vector3 (x, 0.0f, z);

			coll.enabled = true;
			rb.AddForce (movement * 1500);
			strokes++;
		}
		else
		{
			float moveHorizontal = Input.GetAxis ("Horizontal");		// How much the player wants the putter to move horizontally
			float moveVertical = Input.GetAxis ("Vertical");			// How much the player wants the ball to move vertically

			// Sets the power
			power -= moveVertical;

			// Makes sure the power stays within 0.0-100.0
			if (power > 100.0f)
				power = 100.0f;
			else if (power < 0.0f)
				power = 0.0f;


			// Moves the putter horizontally
			if (moveHorizontal != 0f)
			{
				float rotation = Time.deltaTime * moveHorizontal * 20f;
				float temp = power / 25f;
				float numerator1, numerator2, denominator, xDistance, zDistance;

				transform.position = startingPosition;

				v = Quaternion.AngleAxis (rotation, Vector3.up) * (transform.position - point.position);
				transform.position = point.position + v;
				transform.Rotate (0.0f, rotation, 0.0f);

				numerator1 = point.position.x - transform.position.x;
				numerator2 = point.position.z - transform.position.z;
				denominator = Mathf.Abs ((float)System.Math.Sqrt (numerator1 * numerator1 + numerator2 * numerator2));
				xDistance = numerator1 / denominator;
				zDistance = numerator2 / denominator;

				SetClamps (xDistance, zDistance);

				transform.position = new Vector3 (startingPosition.x - temp * xDistance, startingPosition.y, startingPosition.z - temp * zDistance);
				cam.transform.position = new Vector3(transform.position.x - 35.5f * xDistance, point.transform.position.y + 20f, transform.position.z - 35.5f * zDistance);
				cam.transform.rotation = transform.rotation;
				cam.transform.LookAt(point.transform);
			}
			// Moves the putter vertically
			else if (moveVertical != 0f && !newMax)
			{
				float temp = power / 25f;
				transform.position = new Vector3 (startingPosition.x - temp * xDistance, startingPosition.y, startingPosition.z - temp * zDistance);
			}
		}

		// Displays the scoreboard
		if(Input.GetKeyDown(KeyCode.Tab))
			scoreboard.SetActive(true);
		// Hides the scoreboard
		else if(Input.GetKeyUp(KeyCode.Tab))
			scoreboard.SetActive(false);
	}

    void FixedUpdate()
    {
		// Stops the putter if it's past the maximum x
        if (transform.position.x > maxX)
        {
			ManageShot ();
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
		// Stops the putter if it's past the minimum x
        else if (transform.position.x < minX)
        {
			ManageShot ();
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }

		// Stops the putter if it's past the maximum z
        if (transform.position.z > maxZ)
        {
			ManageShot ();
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
		// Stops the putter if it's past the minimum z
        else if (transform.position.z < minZ)
        {
			ManageShot ();
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
        }
    }

	void ManageShot()
	{
		if (newMax)
		{
			rb.Sleep();
			power = 0f;

			if (golfballRB.IsSleeping())
				rp.NewShot();
			else
				gameObject.SetActive(false);
		}
	}

    public void SetClamps(float xD, float zD)
    {
		// Sets the new min/max x/z
        newMax = false;
        startingPosition = transform.position;
        maxX = startingPosition.x;
        minX = maxX - 4f * xD;
        maxZ = startingPosition.z;
        minZ = maxZ - 4f * zD;

		// Determines whether the min/max x need to be reversed
        if (maxX < minX)
        {
            float temp = minX;
            minX = maxX;
            maxX = temp;
            reversedX = true;
        }
        else
            reversedX = false;

		// Determines whether the min/max z need to be reversed
        if (maxZ < minZ)
        {
            float temp = minZ;
            minZ = maxZ;
            maxZ = temp;
            reversedZ = true;
        }
        else
            reversedZ = false;

		// Sets the x and z offset and resets the angle
        xDistance = xD;
        zDistance = zD;
		coll.enabled = false;
    }
}