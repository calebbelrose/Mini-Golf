using UnityEngine;
using System.Collections;

public class ResetPutter : MonoBehaviour
{

    public GameObject putter, ballObject, camera;
    private GameObject hole;
    private Rigidbody rb;
    int holeNumber = 5;
    private Vector3 cameraOffset, prevPos;
    bool needReset = true, inTheHole = false;
    float[] holeLocation = new float[] { 0, -23, -86, -147, -172, -186 };

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        NewHole();
        prevPos = transform.position;
        Physics.IgnoreLayerCollision(8, 10, true);
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
            putter.SetActive(true);
            if (inTheHole)
            {
                inTheHole = false;
                NewHole();
            }
            NewShot();
        }
        prevPos = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == ("hole" + holeNumber))
            inTheHole = true;
        else
            inTheHole = false;
    }

    public void NewHole()
    {
        transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        ballObject.transform.position = new Vector3(holeLocation[holeNumber], 0f, -44.5f);
        holeNumber++;
        hole = GameObject.Find("hole" + holeNumber);
    }
    
    public void NewShot()
    {
        needReset = false;
        float numerator1, numerator2, denominator, xDistance, zDistance;

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
        putter.GetComponent<PlayerController>().SetClamps(xDistance, zDistance);
    }
}