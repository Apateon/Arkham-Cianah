using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //get the 
    Transform cam = null;
    public GameObject indicator;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public void turnOnIndicator()
    {
        indicator.SetActive(true);
    }

    public void turnOffIndicator()
    {
        indicator.SetActive(false);
    }

    public float GetDistance(GameObject player)
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    void Update()
    {
        indicator.transform.rotation = cam.rotation;
    }
}
