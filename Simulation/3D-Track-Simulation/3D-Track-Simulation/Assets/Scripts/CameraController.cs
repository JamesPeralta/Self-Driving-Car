using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset;
    public float followSpeed = 10;
    public float lookSpeed = 10;

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

    public void ChooseTarget()
    {
        Object[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (allPlayers.Length > 0)
        {
            Simulation simulation = GameObject.FindObjectOfType<Simulation>();
            Structure bestStructure = simulation.GetBestCar();

            objectToFollow = bestStructure.GetCar().gameObject.transform;
        }
        else
        {
            GameObject startingLine = GameObject.Find("Starting Line");
            objectToFollow = startingLine.gameObject.transform;
        }
    }

    public void FixedUpdate()
    {
        ChooseTarget();
        LookAtTarget();
        MoveToTarget();
    }
}
