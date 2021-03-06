using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    PlayerStats playerStats;
    ScreenFade fade;
    Animator anim;
    float animationTime = 0;
    float time2Reset = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = this.gameObject.GetComponent<PlayerStats>();
        anim = this.GetComponent<Animator>();
        fade = GameObject.Find("GameUI").GetComponentInChildren<ScreenFade>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats.isDead)
        {
            this.GetComponent<Collider>().enabled = false;
            DeathAnimation();
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            foreach(AnimationClip clip in clips)
            {
                if(clip.name == "PlayerDeathAni")
                    animationTime = clip.length;
            }
        }
        if(animationTime != 0)
        {
            if(time2Reset >= animationTime)
            {
                fade.ChangeScene(true);
            }
            else
            {
                time2Reset += Time.deltaTime;
            }

        }
    }

    void DeathAnimation()
    {
        anim.SetBool("isDead", true);
    }

}
