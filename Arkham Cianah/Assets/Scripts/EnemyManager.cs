using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    Transform cam = null;
    public GameObject indicator;
    Animator enemyAnimator = null;

    float health;
    bool canDamage;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        enemyAnimator = GetComponentInChildren<Animator>();

        ResetEnemy();
    }

    void ResetEnemy()
    {
        health = 100f;
        canDamage = true;
    }

    public void turnOnIndicator()
    {
        indicator.SetActive(true);
    }

    public void turnOffIndicator()
    {
        indicator.SetActive(false);
    }

    void Update()
    {
        indicator.transform.rotation = cam.rotation;
    }

    public void TakeDamage(float damage)
    {
        if(canDamage)
        {
            enemyAnimator.SetTrigger("Hit");
            FindAnyObjectByType<AudioManager>().PlaySound("HitReaction");
            health -= damage;
            if (health <= 0)
            {
                PlayDeathAnimation();
            }
        }
    }

    void PlayDeathAnimation()
    {
        enemyAnimator.SetTrigger("Dead");
        canDamage = false;
    }

    public void KillEnemy()
    {
        //disable the enemy
        gameObject.SetActive(false);

        //move him to another location
        float x = Random.Range(-30, 125);
        float z = Random.Range(-30, 125);
        transform.position = new Vector3(x, 19f, z);

        ResetEnemy();

        //reset and enable him again
        gameObject.SetActive(true);
    }
}
