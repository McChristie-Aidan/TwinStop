﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
class PlayerStats : MonoBehaviour
{
    private static int numOfKilledEnemies;
    public static int NumOfKilledEnemies { get; private set; }

    public int health; // Total amount of health left
    public int numOfHearts; // Max amount of hearts a player can have, should be 3

    public int keys; //number of keys the player has -A
    public int bossKeys; //number of bossKeys the player has -A


    public Image[] hearts; // all heart UI game objects go here
    public Sprite fullHeart; // sprite of full heart here
    public Sprite emptyHeart; // sprite of empty heart here

    void Update()
    {
        // Health Lock
        if (health > numOfHearts)
        {
            health = numOfHearts; // this makes sure that players can never go over the set amount of hearts

        }

        // system for turning full hearts to empty hearts
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

        Debug.Log(health);
        if (health <= 0)
        {
            PlayerDead();
        }
    }

    public static void ResetKillCount()
    {
        NumOfKilledEnemies = 0;
    }

    public static void AddToKillCount()
    {
        NumOfKilledEnemies++;
        //Debug.Log(NumOfKilledEnemies);
    }

    // TODO add code for projectiles
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyBullet")
        {
            health--;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyBullet")
        {
            health--;
        }
    }

    void PlayerDead()
    {
        FindObjectOfType<SceneManagement>().LoadCurrentLevel();
    }
}
