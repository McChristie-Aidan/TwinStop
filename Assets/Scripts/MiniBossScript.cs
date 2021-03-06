using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MiniBossScript : Sentinel
{
    enum MiniBossState {Attack, SpecialAttack, Dead, Spawn, offline }
    MiniBossState mState;
    [SerializeField] GameObject roomTrigger;
    [SerializeField] GameObject MeteorMarker;
    [SerializeField] float specialAttackRate;
    [SerializeField] float spawnRate;
    [SerializeField] int meteorCount;
    [SerializeField] bool isBlueGiantSkull; //Otherwise is Red Giant Skull
    [SerializeField] MiniBossEnemySpawn[] enemySpawn;
    [SerializeField] int[] spawnAtHealthPercent;

    float specialAttackTimer;
    float specialAttackInterval;
    float burstTimer;
    float waveTimer;
    int fireIntervals;
    int intervalCounter;
    int maxHealth;
    bool modify;
    bool isActive;

    Vector3[] randomLoc;
    List<GameObject> meteors;
    // Update is called once per frame
    public override void Start()
    {
        base.Start();
        mState = MiniBossState.offline;
        specialAttackRate = 20f;
        meteors = new List<GameObject>();
        CreateMeteorMarkers();
        MeteorMarker.SetActive(false);
        randomLoc = new Vector3[meteorCount];
        fireIntervals = 1;
        maxHealth = (int)Health;
        isActive = this.transform.Find("ActivationTrigger").GetComponent<SentryTrigger>().isTriggered;
        if (enemySpawn != null)
            TurnOffSpawners();
    }

    void TurnOffSpawners()
    {
        foreach (MiniBossEnemySpawn e in enemySpawn)
            e.gameObject.SetActive(false);
    }

    void CreateMeteorMarkers()
    {
        for(int i = 0; i < meteorCount; i++)
        {
            meteors.Add(Instantiate(MeteorMarker));
            meteors[i].SetActive(false);
        }
    }
    public override void FixedUpdate()
    {
        if (PauseScript.Instance.isPaused)
            return;
        SwitchState();
        SpecialAttackTimer();
        RotateToPlayer();
        ModTest();
        DamageFlash();
        GetActivation();
        CheckHealth(); Debug.Log(Health);
        EnemySpawnerSystem();
    }

    void GetActivation()
    {
        Debug.Log(isActive);
        if (!this.transform.Find("ActivationTrigger").GetComponent<SentryTrigger>().isTriggered)
            return;
        mState = MiniBossState.Attack;
    }

    void EnemySpawnerSystem()
    {
        if (!isBlueGiantSkull || mState != MiniBossState.Attack)
            return;
        SpawnOnHealthPercent();
        if(waveTimer >= spawnRate)
        {
            SpawnersActivation("Group");
            waveTimer = 0;
            return;
        }
        if (waveTimer >= 2.2f)
            TurnOffSpawners();
        waveTimer += Time.deltaTime;
    }

    void SpawnOnHealthPercent()
    {
        for(int i = 0; i < spawnAtHealthPercent.Length; i++)
        {
            if((int)PercentCalc() == spawnAtHealthPercent[i])
            {
                SpawnersActivation("Wave");
            }

        }
    }

    void SpawnersActivation(string type)
    {
        for (int i = 0; i < enemySpawn.Length; i++)
        {
            enemySpawn[i].gameObject.SetActive(true);
            if (type == "Group")
                SpawnGroup(enemySpawn[i]);
            else
                SpawnWave(enemySpawn[i]);
        }
    }

    void SpawnGroup(MiniBossEnemySpawn m)
    {
        m.SpawnEnemy();
    }

    void SpawnWave(MiniBossEnemySpawn m)
    {
        m.GetEnemyWave();
    }

    void ModTest()
    {
        if ((int)PercentCalc() == 60)
            fireCount = 5;
        if ((int)PercentCalc() == 30)
            fireCount = maxCount;
        if ((int)PercentCalc() == 50)
            fireIntervals = 2;
        if ((int)PercentCalc() == 40)
            fireIntervals = 3;
    }

    float PercentCalc()
    {
        return (this.Health / maxHealth) * 100;
    }

    void SpecialAttackTimer()
    {
        Debug.Log(isBlueGiantSkull);
        if (isBlueGiantSkull)
            return;
        if (mState == MiniBossState.SpecialAttack)
            return;
        if(specialAttackTimer >= specialAttackRate)
        {
            mState = MiniBossState.SpecialAttack;
            specialAttackTimer = 0;
            return;
        }
        specialAttackTimer += Time.deltaTime;
    }

    void GetMeteors()
    {
        for(int i = 0; i < meteorCount; i++)
        {
            if(i <= 0)
            {
                randomLoc[i] = target.transform.position;
                meteors[i].transform.position = target.transform.position + new Vector3(0, .1f, 0);
            }
            else
            {
                NavMeshHit hit;
                if(NavMesh.SamplePosition(target.transform.position + Random.insideUnitSphere * 5f,out hit, 5f, NavMesh.AllAreas))
                {
                    randomLoc[i] = hit.position;
                    meteors[i].transform.position = hit.position;
                }
            }
        }
    }

    void RotateToPlayer()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;
        Vector3 direction = Vector3.RotateTowards(this.transform.forward, targetDir, Time.deltaTime, 0.0f);
        this.transform.rotation = Quaternion.LookRotation(direction);
    }

    void MeteorShower()
    {
        if (specialAttackInterval < 3f)
        {
            specialAttackInterval += Time.deltaTime;
            return;
        }
        foreach(Vector3 v in randomLoc)
        {
            projectile = ObjectPool_Projectiles.Instance.GetProjectile(projectileType);
            projectile.GetComponent<Projectile>().b_Speed = 2f;
            projectile.GetComponent<Projectile>().SetUp(Vector3.down, new Vector3(v.x, v.y + 10f, v.z), "Enemy");
        }
        specialAttackInterval = 0;
        mState = MiniBossState.Attack;
    }

    void DisplayMeteorDestination()
    {
        foreach (GameObject g in meteors)
            g.SetActive(true);
    }

    void TurnOffMarkers()
    {
        if (!meteors[0].activeSelf)
            return;
        if (specialAttackTimer <= 5f)
            return;
        foreach (GameObject g in meteors)
            g.SetActive(false);

    }

    protected override void AttackTarget()
    {
        if (!coolDown && !target.GetComponent<PlayerStats>().isDead)
        {
            FireIntervals();
        }
    }

    void Shoot()
    {
        for (int i = 0; i < fireCount; i++)
        {
            projectile = ObjectPool_Projectiles.Instance.GetProjectile(projectileType); //Getting the projectile gameobject
            projectile.GetComponent<Projectile>().b_Speed = projectileSpeed;
            projectile.GetComponent<Projectile>().SetUp(projectileStartPos[i].forward, projectileStartPos[i].position, "Enemy");
        }
    }

    void FireIntervals()
    {
        if(intervalCounter >= fireIntervals)
        {
            coolDown = true;
            intervalCounter = 0;
            return;
        }
        if(burstTimer >= .2f)
        {
            Shoot();
            burstTimer = 0;
            intervalCounter++;
            return;
        }
        burstTimer += Time.deltaTime;

    }

    void SwitchState()
    {
        switch (mState)
        {
            case MiniBossState.Attack:
                AttackTarget();
                AttackCoolDown();
                TurnOffMarkers();
                break;
            case MiniBossState.Dead:
                break;
            case MiniBossState.SpecialAttack:  
                GetMeteors();
                DisplayMeteorDestination();
                MeteorShower();
                break;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }
}
