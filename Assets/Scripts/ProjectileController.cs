using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// [Train Game] | Controls the properties of bullet prefabs
/// </summary>
public class ProjectileScript : MonoBehaviour
{
    

    [SerializeField]private float thrust = 10;/// <summary> How fast the bullet is moving </summary>

    public float damage; /// <summary> The damage that is passed to the hit object </summary>

    private GameObject spawnedParent; /// <summary> Parent gameobject that spawned the bullet, so the bullet doesn't collide with its owner</summary>

    private Rigidbody2D _rb;
    [SerializeField] private AudioClip ghurt, bang, s_wall;
    [SerializeField] private ParticleSystem p_wall, p_fire;

    private void Awake(){
        AudioSource.PlayClipAtPoint(bang, transform.position);
        Instantiate(p_fire).transform.position = gameObject.transform.position;
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Update(){
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(_rb.position);
        if(screenPosition.x > Screen.width || screenPosition.y > Screen.height || screenPosition.x < 0 || screenPosition.y < 0) {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.GetComponent<PlatformEffector2D>() != null)return;
        if(other.gameObject.GetComponent<ProjectileScript>() != null) return;
        if(other.gameObject.GetComponent<Furnace>() != null){
            other.gameObject.GetComponent<Furnace>().DamageTrain((int)this.damage);
            Destroy(gameObject);
            return;
        }

        if(other.gameObject.tag == "Interactable")return;
        if(other.gameObject == spawnedParent)return;
        if (other.GetType() == typeof(CircleCollider2D)) return;
        if(other.gameObject.GetComponent<Igiveup>() != null){
            if ((other.gameObject.GetComponent<Igiveup>().health -= damage) > 0)
            {
                AudioSource.PlayClipAtPoint(ghurt, transform.position);
            }
            other.gameObject.GetComponent<Igiveup>().health -= damage;
        }
        AudioSource.PlayClipAtPoint(s_wall, transform.position);
        Instantiate(p_wall).transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }


    /// <summary>
    /// Is called by the source of the bullet to set the paramaters of the bullet once instanciated.
    /// </summary>
    /// <param name="layer">the layer in which the bullet was called</param>
    /// <param name="thrust">how fast the bullet is going</param>
    /// <param name="damage">the integer value that will be provided when something takes damage</param>
    /// <param name="spawnedParent">the game object that called the for bullet to be spawned</param>
    public void SetBulletArgs(int layer, int thrust, int damage, GameObject spawnedParent){

        this.spawnedParent = spawnedParent;
        LayerHelper.SwitchLayers((LayerHelper.TrainLayer)layer, gameObject);
        
        this.damage = damage;
        this.thrust = thrust;

        _rb.gravityScale = 0;
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        _rb.velocity = new Vector2(
            -Mathf.Cos(angle) * thrust,
            -Mathf.Sin(angle) * thrust
        );
    }
}
