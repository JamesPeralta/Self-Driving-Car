/* This class contains all of the logic related to the camera control in game view
 * It follows car with the highest fitness function and has three points of views.
 * Birds-eye from the right, birds-eye from the left, and third person. 
*/
using UnityEngine;


public class CameraController : MonoBehaviour
{
    // Constants
    private const int THIRD_PERSON = 0;
    private const int BIRDS_EYE_LEFT = 1;
    private const int BIRDS_EYE_RIGHT = 2;

    public Transform objectToFollow;
    public Vector3 offset;
    private int pos;
    public float followSpeed = 10;
    public float lookSpeed = 10;

    // Rotate camera towards the target
    public void LookAtTarget()
    {
        Vector3 _lookDirection = objectToFollow.position - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
    }

    #region Camera Angles
    public void FollowThirdPerson()
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
    #endregion

    // Finds the car with the best fitness and locks onto it
    public void ChooseTarget()
    {
        Simulation simulation = GameObject.FindObjectOfType<Simulation>();
        Structure bestStructure = simulation.GetBestCar();

        objectToFollow = bestStructure.GetCar().gameObject.transform;
    }

    // Start the camera in birds eye view
    void Start()
    {
        pos = 0;
    }

    // Check for camera angle changes from the user and update
    // camera position accordingly
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            pos = (pos + 1) % 3;
        }
    }

    // Update the cameras position and location on every frame
    public void FixedUpdate()
    {
        Object[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (allPlayers.Length > 0)
        {
            if (pos == THIRD_PERSON)
            {
                ChooseTarget();
                LookAtTarget();
                FollowThirdPerson();
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