using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private const int FIRST_PERSON = 0;
    private const int BIRDS_EYE_LEFT = 1;
    private const int BIRDS_EYE_RIGHT = 2;

    public Transform objectToFollow;
    public Vector3 offset;
    public float followSpeed = 10;
    public float lookSpeed = 10;
    private int pos;

    public void LookAtTarget()
    {
        Vector3 _lookDirection = objectToFollow.position - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
    }

    public void MoveToTarget()
    {
        Vector3 _targetPos = objectToFollow.position + 
                             objectToFollow.forward * offset.z + 
                             objectToFollow.right * offset.x + 
                             objectToFollow.up * offset.y;

        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
    }

    public void FollowBirdsEyeRight()
    {
        transform.position = Vector3.Lerp(transform.position, objectToFollow.transform.position + new Vector3(50, 50, 50), followSpeed * Time.deltaTime); ;
    }

    public void FollowBirdsEyeLeft()
    {
        transform.position = Vector3.Lerp(transform.position, objectToFollow.transform.position + new Vector3(-50, 50, 50), followSpeed * Time.deltaTime); ;
    }

    public void ChooseTarget()
    {
        Simulation simulation = GameObject.FindObjectOfType<Simulation>();
        Structure bestStructure = simulation.GetBestCar();

        objectToFollow = bestStructure.GetCar().gameObject.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        pos = 0;
    }


    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(pos);
            pos = (pos + 1) % 3;
        }
    }

    public void FixedUpdate()
    {
        Object[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (allPlayers.Length > 0)
        {
            if (pos == FIRST_PERSON)
            {
                ChooseTarget();
                LookAtTarget();
                MoveToTarget();
            }
            else if (pos == BIRDS_EYE_RIGHT)
            {
                ChooseTarget();
                LookAtTarget();
                FollowBirdsEyeRight();
            }
            else
            {
                ChooseTarget();
                LookAtTarget();
                FollowBirdsEyeLeft();
            }
        }
        else
        {
            GameObject startingLine = GameObject.Find("Starting Line");
            objectToFollow = startingLine.gameObject.transform;
        }
    }
}
