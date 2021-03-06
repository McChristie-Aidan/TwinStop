using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChronoLordAttackPattern : MonoBehaviour
{
    Animator MyAnimator;

    [SerializeField]
    GameObject[] Chargers, TestEnemies, Turtles;

    GameObject[] waveToCheck;

    [SerializeField]
    public float attackRate = 1;

    bool IsVulnerable, HasIncremented, AmDead;

    [SerializeField]
    bool coolDown;

    int waveIndex;

    Cannon_SentryTurret MySentryTurret;

    float fireTimer = 0;

    string projectileType;

    float projectileSpeed = 12;

    string BoolToSet;

    [SerializeField]
    float Health;

    [SerializeField]
    Slider CLHealthBar;

    [SerializeField]
    GameObject BubbleShield, projectileStartPos, explosionvx;

    GameObject Player, projectile;

    public Renderer FlashRenderer { get; set; }
    public Material hurtMat;
    public Material HurtMat { get => hurtMat; }
    protected Material defaultMat;
    public float flashDuration;

    [SerializeField]
    ChronoLordStatus MyChronoLordStatus;
    public float FlashDuration
    {
        get => flashDuration;
        set => flashDuration = value;
    }
    public float FlashTimer { get; set; }
    Renderer[] FlashRenderers;

    public bool hasChildrenRender;
    public LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        MySentryTurret = GetComponent<Cannon_SentryTurret>();
        Player = GameObject.FindGameObjectWithTag("Player");
        HasIncremented = false;
        MyAnimator = GetComponent<Animator>();
        AmDead = IsVulnerable = false;
        BubbleShield.SetActive(true);
        //projectileStartPos.SetActive(false);
        waveIndex = 1;
        waveToCheck = Chargers;

        FlashTimer = 0;

        FlashRenderer = GetComponent<Renderer>();
        if (hasChildrenRender)
        {
            FlashRenderers = this.transform.GetComponentsInChildren<Renderer>();
        }
        if (FlashRenderer == null || FlashRenderer.enabled == false)
        {
            FlashRenderer = this.GetComponentInChildren<Renderer>();
        }
        defaultMat = FlashRenderer.material;

        MyChronoLordStatus.AmFiring();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWaveFinished(waveToCheck) && !IsVulnerable && !HasIncremented)
        {
            MakeVulnerable();
            MyAnimator.SetBool(BoolToSet, true);
        }
        //Debug.Log("baba"+FlashTimer);
    }

    private void FixedUpdate()
    {
        FlashCoolDown();
    }

    void FlashCoolDown()
    {
        FlashTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(FlashTimer / FlashDuration);

        if (hasChildrenRender)
        {
            if (FlashTimer >= 0 && FlashRenderer.material != hurtMat)
            {
                foreach (Renderer r in FlashRenderers)
                    r.material = HurtMat;
            }
            else if (FlashTimer <= 0 && FlashRenderer.material != defaultMat)
            {
                foreach (Renderer r in FlashRenderers)
                    r.material = defaultMat;
            }
            return;
        }

        if (FlashTimer >= 0 && FlashRenderer.material != hurtMat)
        {
            FlashRenderer.material = HurtMat;
        }
        else if (FlashTimer <= 0 && FlashRenderer.material != defaultMat)
        {
            FlashRenderer.material = defaultMat;
        }
    }

    public bool IsWaveFinished(GameObject[] enemiesInWave)
    {
        foreach (GameObject enemy in enemiesInWave)
        {
            if (enemy.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    void MakeInvulnerable()
    {
        HasIncremented = false;
        MyAnimator.SetBool("Vulnerable", false);
        IsVulnerable = false;
        BubbleShield.SetActive(true);
        projectileStartPos.SetActive(false);
        MyChronoLordStatus.NotVulnerable();
        MyChronoLordStatus.AmFiring();
    }

    void MakeVulnerable()
    {
        IncrementWave();
        MyAnimator.SetBool("Vulnerable", true);
        IsVulnerable = true;
        BubbleShield.SetActive(false);
        projectileStartPos.SetActive(true);
        MyChronoLordStatus.AmVulnerable();
        MyChronoLordStatus.NotFiring();
    }

    void IncrementWave()
    {
        if (waveIndex == 1)
        {
            BoolToSet = "ChargerWaveEnd";
            waveToCheck = TestEnemies;
        }
        else if (waveIndex > 1)
        {
            waveToCheck = Turtles;
            BoolToSet = "TestEnemyWaveEnd";
        }
        waveIndex++;
        HasIncremented = true;
        Debug.Log("wave index" + waveIndex);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "PlayerBullet" && IsVulnerable)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (!AmDead)
        {
            if (Time.timeScale < 1f)
            {
                Health -= .8f;
                Debug.Log("Half Damage. Health - " + Health);
            }
            else
            {
                Health--;
                Debug.Log("Full Damage. Health - " + Health);
            }

            CLHealthBar.value = Health;
            FlashTimer = FlashDuration;

            if (CheckAmIDead())
            {
                Die();
            }
        }
    }

    bool CheckAmIDead()
    {
        if (Health <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Die()
    {
        MyAnimator.SetBool("ImDead", true);
        AmDead = true;
        MySentryTurret.enabled = false;
        MyChronoLordStatus.Die();
    }

    void Despawn()
    {
        Instantiate(explosionvx);
        this.gameObject.SetActive(false);
    }
}
