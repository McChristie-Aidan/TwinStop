using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject Door;

    Door targetDoor;

    [SerializeField]
    bool amIALargeRoom;

    [SerializeField]
    GameObject roomCenter;

    GameObject cameraFollowMe;

    FollowPlayer followPlayerScript; 

    private void Awake()
    {
        targetDoor = Door.GetComponent<Door>();       
    }

    private void Start()
    {
       cameraFollowMe = GameObject.FindWithTag("CamFollow");
       followPlayerScript = cameraFollowMe.GetComponent<FollowPlayer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && DoorManager.Instance.enemyManager.isInCombat == false) //
        {
            
            if (targetDoor.IsLocked == false)
            {
                targetDoor.OpenDoor();
                

                //Debug.Log("Door Opened");
            }
            else
            {
                PlayerStats ps = other.GetComponent<PlayerStats>();

                if (ps.keys >= 1)
                {
                    ps.keys--;
                    targetDoor.IsLocked = false;
                    /*
                     * play really cool animation here
                     */
                    this.OnTriggerEnter(other);
                }
            }
        }     
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            targetDoor.CloseDoor();
            followPlayerScript.Move(roomCenter);
            if(amIALargeRoom)
            {
                followPlayerScript.doIFollow = !followPlayerScript.doIFollow;
            }
            //Debug.Log("Door Closed");

        }
    }
}
