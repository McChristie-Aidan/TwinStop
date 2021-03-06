using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Range(5f, 30f)]
    public float b_Speed = 10; //Bullet's Speed
    [SerializeField]
    GameObject ExplosionPrefab;
    #endregion
    Vector3 direction;
    string projectileUser;
    bool dontPlay = true;
    Rigidbody rb;
    public AudioClip audio;
    //ObjectPool_Projectiles opP;

    void Start()
    {
        //opP = new ObjectPool_Projectiles();
        rb = GetComponent<Rigidbody>();
        //projectileSound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (!dontPlay)
        {
            AudioManager.Instance.PlaySound(this.audio.name, this.transform.position, true);
        }
        dontPlay = false;
        if (GetComponent<ParticleSystem>() != null)
        {
            this.GetComponent<ParticleSystem>().Play();
        }
        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>())
        {
            if (item != null)
            {
                item.Play();
            }
        }
        
    }
    private void OnDisable()
    {
        this.direction = Vector3.zero;
        this.rb.velocity = Vector3.zero;
        if (GetComponent<ParticleSystem>() != null)
        {
            this.GetComponent<ParticleSystem>().Stop();
        }

        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>())
        {
            item.Stop();
        }
        

        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            GetComponentInChildren<TrailRenderer>().Clear();
        }
    }
    public void SetUp(Vector3 direction, Vector3 position, string projectileUser)
    {
        this.direction = direction;
        this.transform.position = position;
        this.projectileUser = projectileUser;
        transform.forward = direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseScript.Instance.isPaused)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        if (projectileUser == "Player")
        {
            //use unscaled delta time if unaffected by time stop
            //use delta time if affected by time stop
            rb.velocity = (direction * b_Speed)/Time.timeScale;
        }
        else
        {
            //transform.position += direction * b_Speed * Time.deltaTime;
            rb.velocity = direction * b_Speed;
        }

        
    }

    void IgnoreProjectiles()
    {
        if (GameObject.FindWithTag(gameObject.transform.tag))
            foreach (var item in GameObject.FindGameObjectsWithTag(gameObject.transform.tag))
            {
                Physics.IgnoreCollision(item.GetComponent<Collider>(), this.gameObject.GetComponent<Collider>(), true);
            }
        //    if (GameObject.FindWithTag("PlayerBullet"))
        //        Physics.IgnoreCollision(GameObject.FindWithTag("PlayerBullet").GetComponent<Collider>(), this.gameObject.GetComponent<Collider>(), true);
        //    if (GameObject.FindWithTag("EnemyBullet"))
        //        Physics.IgnoreCollision(GameObject.FindWithTag("EnemyBullet").GetComponent<Collider>(), this.gameObject.GetComponent<Collider>(), true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        /*
         * If we have an explosion prefab, use it
         */
        if (ExplosionPrefab != null)
        {
            var hitEffect = Instantiate(ExplosionPrefab, this.transform.position, this.transform.rotation);
            AudioManager.Instance.PlaySound("SpellImpact", this.transform.position, true);
        }
        this.gameObject.SetActive(false);
    }
}
