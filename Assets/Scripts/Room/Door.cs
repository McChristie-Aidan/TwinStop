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

    [SerializeField]
    GameObject doorModels;
    public bool IsOpen { get; set; }
    AudioSource[] doorSounds;
    AudioSource doorOpenSound;
    AudioSource doorCloseSound;
    AudioSource hitDoorSound;
    Animator Animator;

    Collider doorCollider;
    Renderer renderer;

    bool doorOverride;

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
    }
    void Start()
    {
        this.doorCollider = this.gameObject.GetComponent<BoxCollider>();
        this.Animator = this.GetComponent<Animator>();
        //this.renderer = this.gameObject.GetComponent<MeshRenderer>();
        IsOpen = false;
        doorSounds = GetComponents<AudioSource>();
        doorOpenSound = doorSounds[0];
        doorCloseSound = doorSounds[1];
        hitDoorSound = doorSounds[2];
        //cam = GameObject.Find("2Dcam");
    }
    private void Update()
    {
        if (!EnemyManager.Instance.isInCombat && !this.IsOpen && !doorOverride)
        {
            if (!IsLocked)
            {
                OpenDoor();
            }
        }
        if (EnemyManager.Instance.isInCombat && this.IsOpen && doorOverride)
        {
            CloseDoor();
        }
        if (RoomManager.Instance.CurrentRoom != null)
        {
            if (!RoomManager.Instance.CurrentRoom.NoMoreWaves)
            {
                doorOverride = true;
            }
            else
            {
                doorOverride = false;
            }
        }
    }

    /*
     * Used to open the door. turns off the renderer and collider but still allows it to update if need be -A
     */
    public void OpenDoor()
    {
        if (this.IsLocked == false)
        {
            this.IsOpen = true;
            doorOpenSound.Play();

            if (Animator != null)
            {
                //Animator.Play("OpenDoor", 0, 0.0f);
                Animator.SetBool("IsOpen", true);
            }
            else
            {
                this.doorModels.SetActive(false);
                this.doorCollider.enabled = false;
            }
        }
    }
    /*
     * Closes the door. doesnt check if the door is locked because open doors are by definition unlocked -A
     */
    public void CloseDoor()
    {
        this.IsOpen = false;
        doorCloseSound.Play();

        if (Animator != null)
        {
            //Animator.Play("CloseDoor", 0, 0.0f);
            Animator.SetBool("IsOpen", false);
        }
        else
        {
            this.doorModels.SetActive(true);
            this.doorCollider.enabled = true;
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        hitDoorSound.PlayOneShot(hitDoorSound.clip);
    }
}
