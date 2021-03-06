using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;

    private Transform target;
    private bool skipMove;

    public AudioClip enemySound1;
    public AudioClip enemySound2;


    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        GameManager.Instance.AddEnemiesToList(this);

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("EnemyAttack");
        SoundManager.instance.RandomizeSfx(enemySound1, enemySound2);
    }
}
