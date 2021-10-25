using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class Door : MonoBehaviour
{
    [SerializeField]
    public bool isBossDoor = false;
    [SerializeField]
    public bool IsLocked = false;
    public bool IsOpen { get; set; }

    Collider doorCollider;
    Renderer renderer;

    private void Awake()
    {
        /*
         * adds This door to the door manager on awake. If there is no doorManager, make one -A
         */
        if (DoorManager.Instance == null)
        {
            DoorManager.CreateDoorManager();
        }
        DoorManager.Instance.doors.Add(this);
        Debug.Log("added door to door manager");
    }
    void Start()
    {
        this.doorCollider = this.gameObject.GetComponent<BoxCollider>();
        this.renderer = this.gameObject.GetComponent<MeshRenderer>();
        IsOpen = false;

        //cam = GameObject.Find("2Dcam");
    }

    /*
     * Used to open the door. turns off the renderer and collider but still allows it to update if need be -A
     */
    public void OpenDoor()
    {
        if (this.IsLocked == false)
        {
            this.IsOpen = true;
            this.renderer.enabled = false;
            this.doorCollider.enabled = false;
        }
    }
    /*
     * Closes the door. doesnt check if the door is locked because open doors are by definition unlocked -A
     */
    public void CloseDoor()
    {
        this.IsOpen = false;
        this.renderer.enabled = true;
        this.doorCollider.enabled = true;
    }
    /*
     * Unlocks the door. -A
     */
    public void UnlockDoor()
    {
        this.IsLocked = false;
    }
    /*
     * Locks the door. Locked doors can only be opened if the player has keys -A
     */
    public void LockDoor()
    {
        this.IsLocked = true;
    }
}
