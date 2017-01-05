using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb, golfballRB;			// Rigidbody for the putter and golf ball
    public Vector3 startingPosition, target;	// Starting position and target for the putter
    float minX, maxX, minZ, maxZ;				// The minimum and maximum position for the putter while shooting
    float xDistance, zDistance;					// x and z offset for 1 unit away from the putter
	float power = 0.0f, angle = 0.0f;			// Power and angle of the shot
	bool newMax = false,						// Whether there's a new min/max x/z
	reversedX, reversedZ;						// Whether the min/max x and the min/max z are reversed
    GUI score;									// GUI used to display the score
    GUIStyle guiStyle;							// Style used to display the power, angle and number of strokes
    public int strokes;							// Number of strokes
    public GameObject golfball, scoreboard;		// Golf ball and scoreboard objects


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 30;
        golfballRB = golfball.GetComponent<Rigidbody>();
    }

    void OnGUI()
    {
		// Displays the power, angle and number of strokes
        GUI.Label(new Rect(0, Screen.height - 30, 100, 30), "Power: " + power.ToString("0.00"), guiStyle);
		GUI.Label(new Rect(0, Screen.height - 60, 100, 30), "Angle: " + angle + "°", guiStyle);
        GUI.Label(new Rect(0, Screen.height - 90, 100, 30), "Strokes: " + strokes, guiStyle);
    }

	void Update()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");		// How much the player wants the putter to move horizontally

		// Moves the putter horizontally
		if (moveHorizontal != 0f)
		{
			float numerator1, numerator2, denominator, temp;	// Offsets used to determine the new position of the putter
			float x1, x2, z1, z2;								// Used to calculate the new target position

			// Calculates the new target position
			x1 = target.x - golfball.transform.position.x;
			z1 = target.z - golfball.transform.position.z;
			x2 = x1 * Mathf.Cos (moveHorizontal) - z1 * Mathf.Sin (moveHorizontal);
			z2 = x1 * Mathf.Sin(moveHorizontal) + z1 * Mathf.Cos(moveHorizontal);

			// Sets the new target position
			target.x = x2 + golfball.transform.position.x;
			target.y = z2 + golfball.transform.position.z;

			// Sets the new angle
			angle += moveHorizontal;

			// Calculates the offsets for the new target
			numerator1 = target.x - golfball.transform.position.x;
			numerator2 = target.z - golfball.transform.position.z;
			denominator = Mathf.Abs((float)System.Math.Sqrt(numerator1 * numerator1 + numerator2 * numerator2));
			xDistance = numerator1 / denominator;
			zDistance = numerator2 / denominator;
			temp = power / 25f;

			// Sets the new position of the putter
			transform.position = new Vector3(startingPosition.x - temp * xDistance, startingPosition.y, startingPosition.z - temp * zDistance);
		}
		// Adds force to the putter to take the shot and increases the number of strokes
		else if (Input.GetKeyDown("space") && !newMax)
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

			movement = new Vector3(x, 0.0f, z);

			rb.AddForce(movement * 1500);
			strokes++;
		}
		// Displays the scoreboard
		else if(Input.GetKeyDown(KeyCode.Tab))
			scoreboard.SetActive(true);
		// Hides the scoreboard
		else if(Input.GetKeyUp(KeyCode.Tab))
			scoreboard.SetActive(false);
	}

    void FixedUpdate()
    {
		// How much the player wants the ball to move vertically
        float moveVertical = Input.GetAxis("Vertical");

		// Sets the power
        power -= moveVertical;

		// Makes sure the power stays within 0.0-100.0
        if (power > 100.0f)
            power = 100.0f;
        else if (power < 0.0f)
            power = 0.0f;

		// Moves the putter vertically
        if (moveVertical != 0f && !newMax)
        {
            float temp = power / 25f;
            transform.position = new Vector3(startingPosition.x - temp * xDistance, startingPosition.y, startingPosition.z - temp * zDistance);
        }

		// Stops the putter if it's past the maximum x
        if (transform.position.x > maxX)
        {
            if (newMax)
            {
                rb.Sleep();
                power = 0f;

                if (golfballRB.IsSleeping())
                    golfball.GetComponent<ResetPutter>().NewShot();
                else
                    gameObject.SetActive(false);
            }
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
		// Stops the putter if it's past the minimum x
        else if (transform.position.x < minX)
        {
            if (newMax)
            {
                rb.Sleep();
                power = 0f;

                if (golfballRB.IsSleeping())
                    golfball.GetComponent<ResetPutter>().NewShot();
                else
                    gameObject.SetActive(false);
            }
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }

		// Stops the putter if it's past the maximum z
        if (transform.position.z > maxZ)
        {
            if (newMax)
            {
                rb.Sleep();
                power = 0f;

                if (golfballRB.IsSleeping())
                    golfball.GetComponent<ResetPutter>().NewShot();
                else
                    gameObject.SetActive(false);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
		// Stops the putter if it's past the minimum z
        else if (transform.position.z < minZ)
        {
            if (newMax)
            {
                rb.Sleep();
                power = 0f;

                if (golfballRB.IsSleeping())
                    golfball.GetComponent<ResetPutter>().NewShot();
                else
                    gameObject.SetActive(false);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
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
		angle = 0;
    }
}