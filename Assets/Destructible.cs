using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float health, maxhealth;
    [SerializeField] private bool immortal;
    [SerializeField] private ParticleSystem distruct_particle;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<ProjectileScript>() != null) health -= other.gameObject.GetComponent<ProjectileScript>().damage;
        if(health < 0)
        {
            Instantiate(distruct_particle).transform.position = gameObject.transform.position;
            if (immortal == false) Destroy(gameObject);
            else health = maxhealth;
        }
        Destroy(other.gameObject);
    }
}
