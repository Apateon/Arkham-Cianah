using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventsManager : MonoBehaviour
{
    public EnemyManager enemy;

    public void DeadAnimationComplete()
    {
        enemy.KillEnemy();
    }
}
