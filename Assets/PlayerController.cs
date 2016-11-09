using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    public Vector3 startingPosition;
    float minX, maxX, minZ, maxZ;
    float xDistance, zDistance;
    bool newMax = false;
    GUI score;
    double power = 0;
    GUIStyle guiStyle;
    int strokes = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 30;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 30, 100, 30), "Power: " + power.ToString("0.00"), guiStyle);
        GUI.Label(new Rect(0, Screen.height - 60, 100, 30), "Strokes: " + strokes, guiStyle);
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        double x, z;

        if (moveVertical != 0f)
        {
            transform.position = new Vector3(transform.position.x + moveVertical * xDistance, transform.position.y, transform.position.z + moveVertical * zDistance);
        }

        if (transform.position.x > maxX)
        {
            if (newMax)
            {
                rb.Sleep();
                gameObject.SetActive(false);
            }
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < minX)
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);

        if (transform.position.z > maxZ)
        {
            if (newMax)
            {
                rb.Sleep();
                gameObject.SetActive(false);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
        else if (transform.position.z < minZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);

        x = (double)(transform.position.x - startingPosition.x);
        z = (double)(transform.position.z - startingPosition.z);
        power = System.Math.Sqrt(x * x + z * z) * 25;

        if (Input.GetKeyDown("space") && !newMax)
        {
            newMax = true;
            maxX = startingPosition.x + (startingPosition.x - transform.position.x) * xDistance;
            maxZ = startingPosition.z + (startingPosition.z - transform.position.z) * zDistance;
            Vector3 movement = new Vector3(startingPosition.x - transform.position.x, 0.0f, startingPosition.z - transform.position.z);
            rb.AddForce(movement * 1000);
            strokes++;
        }
    }

    public void SetClamps(float xD, float zD)
    {
        newMax = false;
        minX = transform.position.x - 4f * xD;
        maxX = startingPosition.x;
        minZ = transform.position.z - 4f * zD;
        maxZ = startingPosition.z;
        xDistance = xD;
        zDistance = zD;
    }
}
