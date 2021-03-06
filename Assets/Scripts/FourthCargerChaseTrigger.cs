using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthCargerChaseTrigger : MonoBehaviour
{
    [SerializeField]
    Animator Animator;

    [SerializeField]
    GameObject Bridge;

    GameObject Player;
    PlayerStats PlayerStatsScript;

    [SerializeField]
    Door DoorScript;

    [SerializeField]
    Animator LocksAnimator;

    Bridge BridgeBridgeScript;
    // Start is called before the first frame update
    void Start()
    {
        BridgeBridgeScript = Bridge.GetComponent<Bridge>();

        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerStatsScript = Player.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FourthCharger"))
        {
            Animator.SetTrigger("ReachedPit");
            if (BridgeBridgeScript.amIUp)
            {
                Animator.SetBool("IsBridgeUp", false);
            }
            else
            {
                Animator.SetBool("IsBridgeUp", true);
                DoorScript.IsLocked = false;
                LocksAnimator.SetBool("IsLocked", false);
            }
        }
    }
}
