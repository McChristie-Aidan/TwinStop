using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    [SerializeField]
    public float speed = 50;
    [SerializeField]
    public int health = 3;
    [SerializeField]
    public int damage = 1;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.Health = health;
        this.Speed = speed;
        this.Damage = damage;
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        DeathSoundClipTime();
        if (!isDead)
        {
            agent.SetDestination(target.transform.position);
            base.FixedUpdate();
        }
    }
}
