using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb, golfballRB;
    public Vector3 startingPosition;
    float minX, maxX, minZ, maxZ;
    float xDistance, zDistance;
    bool newMax = false, reversedX, reversedZ;
    GUI score;
    float power = 0;
    GUIStyle guiStyle;
    public int strokes;
    public GameObject golfball, scoreboard;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 30;
        golfballRB = golfball.GetComponent<Rigidbody>();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 30, 100, 30), "Power: " + power.ToString("0.00"), guiStyle);
        GUI.Label(new Rect(0, Screen.height - 60, 100, 30), "Strokes: " + strokes, guiStyle);
    }

	void Update()
	{
		if (Input.GetKeyDown("space") && !newMax)
		{
			newMax = true;
			float x, z;

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

			Vector3 movement = new Vector3(x, 0.0f, z);

			rb.AddForce(movement * 1500);
			strokes++;
		}
		else if(Input.GetKeyDown(KeyCode.Tab))
			scoreboard.SetActive(true);
		else if(Input.GetKeyUp(KeyCode.Tab))
			scoreboard.SetActive(false);
	}

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        power -= moveVertical;

        if (power > 100f)
            power = 100f;
        else if (power < 0f)
            power = 0f;

        if (moveVertical != 0f && !newMax)
        {
            float temp = power / 25f;
            transform.position = new Vector3(startingPosition.x - temp * xDistance, startingPosition.y, startingPosition.z - temp * zDistance);
        }

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
        newMax = false;
        startingPosition = transform.position;
        maxX = startingPosition.x;
        minX = maxX - 4f * xD;
        maxZ = startingPosition.z;
        minZ = maxZ - 4f * zD;

        if (maxX < minX)
        {
            float temp = minX;
            minX = maxX;
            maxX = temp;
            reversedX = true;
        }
        else
            reversedX = false;

        if (maxZ < minZ)
        {
            float temp = minZ;
            minZ = maxZ;
            maxZ = temp;
            reversedZ = true;
        }
        else
            reversedZ = false;

        xDistance = xD;
        zDistance = zD;
    }
}
