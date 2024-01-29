using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    
    public void SpawnThing(bool isPlayer)
    {
        float x = Random.Range(-30, 125);
        float z = Random.Range(-30, 125);
        transform.position = new Vector3(x, transform.position.y, z);

        while(true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    Vector3 pos = hit.point;
                    pos.y = transform.position.y;
                    Instantiate(isPlayer ? player : enemy, pos, Quaternion.identity);
                    return;
                }
            }
        }
    }
}
