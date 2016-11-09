using UnityEngine;
using System.Collections;

public class ResetPutter : MonoBehaviour
{

    public GameObject putter, ballObject, camera;
    private GameObject hole, wall;
    private Rigidbody rb;
    int holeNumber = 0;
    private Vector3 cameraOffset, prevPos;
    bool needReset = true;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        NewHole();
        cameraOffset = camera.transform.position - transform.position;
        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != prevPos)
        {
            camera.transform.position = transform.position + cameraOffset;
            needReset = true;
        }
        else if (rb.IsSleeping() && needReset)
        {
            needReset = false;
            float numerator1, numerator2, denominator, xDistance, zDistance;
            PlayerController pc;

            putter.SetActive(true);
            Physics.IgnoreCollision(wall.GetComponent<Collider>(), putter.GetComponent<Collider>());
            ballObject.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
            numerator1 = hole.transform.position.x - transform.position.x;
            numerator2 = hole.transform.position.z - transform.position.z;
            denominator = Mathf.Abs((float)System.Math.Sqrt(numerator1 * numerator1 + numerator2 * numerator2));
            xDistance = numerator1 / denominator;
            zDistance = numerator2 / denominator;
            putter.transform.position = new Vector3(transform.position.x - 0.5f * xDistance, ballObject.transform.position.y, transform.position.z - 0.5f * zDistance);
            camera.transform.position = new Vector3(transform.position.x - 35.5f * xDistance, ballObject.transform.position.y + 20f, transform.position.z - 35.5f * zDistance);
            putter.transform.LookAt(ballObject.transform);
            camera.transform.LookAt(ballObject.transform);
            camera.transform.Rotate(-19.378f, 0f, 0f);
            pc = putter.GetComponent<PlayerController>();
            pc.startingPosition = putter.transform.position;
            pc.SetClamps(xDistance, zDistance);
        }
        prevPos = transform.position;
    }

    public void NewHole()
    {
        holeNumber++;
        hole = GameObject.Find("hole" + holeNumber);
        wall = GameObject.Find("wall" + holeNumber);
        Physics.IgnoreCollision(wall.GetComponent<Collider>(), putter.GetComponent<Collider>());
    }
}