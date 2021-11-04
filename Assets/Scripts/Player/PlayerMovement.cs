using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody playerRigidbody;

    private Vector3 moveInput;
    private Vector3 moveVelocity;

    private  Vector3 currentMoveToTarget;

    private Camera mainCamera;

    private GunControl gunControlScript;

    //Turns true when special scenes happen like a door transition
    private bool freezeMovement;

    Animator anim;

    int moveFwd = Animator.StringToHash("Forward Ani");
    int moveBack = Animator.StringToHash("Backward Ani");
    int idle = Animator.StringToHash("Idle");
    int rightStrafe = Animator.StringToHash("RightStrafe");
    int leftStrafe = Animator.StringToHash("LeftStrafe");

    private void Start()
    {
        this.playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        anim = GetComponent<Animator>();
        freezeMovement = false;

        gunControlScript = this.GetComponent<GunControl>();

        //currentMoveToTarget = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!freezeMovement)
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            moveVelocity = moveInput.normalized * moveSpeed;

            float velocityZ = Vector3.Dot(moveInput.normalized, transform.forward);
            float velocityX = Vector3.Dot(moveInput.normalized, transform.right);

            anim.SetFloat("VelocityZ",velocityZ, 0.1f, Time.deltaTime);
            anim.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
            //if (Input.GetKey(KeyCode.W))
            //    anim.SetTrigger(moveFwd);
            //else if (Input.GetKey(KeyCode.S))
            //    anim.SetTrigger(moveBack);
            //else if (Input.GetKey(KeyCode.A))
            //    anim.SetTrigger(leftStrafe);
            //else if (Input.GetKey(KeyCode.D))
            //    anim.SetTrigger(rightStrafe);
            //else
            //    anim.SetTrigger(idle);
        }
        else
        {
            MovePlayerToTarget();
        }
        
        
        //this.playerRigidbody.MovePosition(playerRigidbody.position + (moveVelocity * Time.unscaledDeltaTime));

        //Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        //Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        //float rayLength; // Length of line from Camera to nearest ground

        //if(groundPlane.Raycast(cameraRay, out rayLength))
        //{
        //    Vector3 pointToLook = cameraRay.GetPoint(rayLength);
        //    Debug.DrawLine(cameraRay.origin, pointToLook, Color.green);

        //    transform.LookAt(pointToLook);
        //}
    }

    void FixedUpdate()
    {
        playerRigidbody.velocity = moveVelocity;
    }

    /// <summary>
    /// Called by door triggers to move the player to the correct spot when they enter a door.
    /// </summary>
    /// <param name="moveTo"></param>
    public void StartDoorTransition(Vector3 moveTo)
    {
        if (!freezeMovement)
        {
            Freeze();
            currentMoveToTarget = moveTo;
        }
       
    }

    /// <summary>
    /// The method used for moving a player to a certain point (without player input)
    /// </summary>
    void MovePlayerToTarget()
    {
        if (this.transform.position != currentMoveToTarget)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentMoveToTarget, 9 * Time.deltaTime);
        }
        else
        {
            //currentMoveToTarget = this.transform.position;
            UnFreeze();
        }
    }

    /// <summary>
    ///  Freezes the player
    /// </summary>
    void Freeze()
    {
        gunControlScript.FrezeFire();
        freezeMovement = true;
    }

    /// <summary>
    ///  UnFreezes the player
    /// </summary>
    void UnFreeze()
    {
        gunControlScript.UnFrezeFire();
        freezeMovement = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TransitionTrigger"))
        {
            Debug.Log("Duh?");
            if (freezeMovement)
            {
                currentMoveToTarget = this.transform.position;
                UnFreeze();

            }
        }
    }
}
