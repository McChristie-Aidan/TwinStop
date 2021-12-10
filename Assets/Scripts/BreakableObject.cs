using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BreakableObject : MonoBehaviour, IDamageFlash
{
    [SerializeField] private GameObject ItemToSpawn;

    AudioSource crateBreak;
    AudioClip clip;
    bool isBroken;

    float clipLength;
    float clipTimer;

    /*
     * Damage Flash variables
     */
    Renderer renderer;
    public Renderer FlashRenderer 
    {
        get => renderer;
        set => renderer = value;
    }
    Material defaultMat;
    public Material hurtMat;
    public Material HurtMat => hurtMat;
    public float flashDuration = 0.2f;
    public float FlashDuration { get => flashDuration; set => flashDuration = value; }
    public float FlashTimer { get; set; }
    public float health;
    public float Health { get => health; set => health = value; }

    // Start is called before the first frame update
    void Start()
    {
        crateBreak = GetComponent<AudioSource>();
        clip = GetComponent<AudioSource>().clip;
        renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        defaultMat = renderer.material;

        isBroken = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBroken)
        {
            clipLength = clip.length;
            if (clipTimer >= clipLength)
                Destroy(this.gameObject);
            else
                clipTimer += Time.deltaTime;
        }
        FlashCoolDown();
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PlayerBullet" || other.gameObject.tag == "EnemyBullet" || other.gameObject.GetComponent<Charger>() != null)
        {
            Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
            if (other.gameObject.GetComponent<Charger>() != null)
            {
                this.TakeDamage(5);
            }
            else
            {
                this.TakeDamage();
            }
            crateBreak.Play();
            if (this.Health <= 0)
            {
                if (ItemToSpawn != null)
                    Instantiate(ItemToSpawn, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
                foreach (var item in this.gameObject.GetComponents<BoxCollider>())
                {
                    item.enabled = false;
                }
            }
        }
    }
    public void TakeDamage()
    {
        TakeDamage(1);
    }
    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        FlashTimer = FlashDuration;
        if (Health <= 0)
        {
            this.isBroken = true;
        }
    }
    void FlashCoolDown()
    {
        FlashTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(FlashTimer / FlashDuration);

        if (FlashTimer >= 0 && FlashRenderer.material != hurtMat)
        {
            FlashRenderer.material = HurtMat;
        }
        else if (FlashTimer <= 0 && FlashRenderer.material != defaultMat)
        {
            FlashRenderer.material = defaultMat;
        }
    }
}
