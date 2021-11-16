using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    AudioSource audio;
    AudioClip clip;
    bool isPickedUp;
    float clipLength;
    float clipTimer;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        isPickedUp = false;
        clip = audio.clip;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {
            if (clipTimer >= clipLength)
                this.gameObject.SetActive(false);
            else
                clipTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            audio.Play();
            isPickedUp = true;
            clipLength = clip.length;
            other.GetComponent<PlayerStats>().Health++;
        }
    }
}