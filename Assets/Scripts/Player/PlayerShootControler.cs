using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    
    [Header("Projectile")]
    [SerializeField] private GameObject projectile;

    [Header("Gun")]
    [SerializeField] private GameObject gun;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(
                transform.position.y - mousePos.y,
                transform.position.x - mousePos.x
            );
            


            GameObject obj = (GameObject) Instantiate(
                projectile,
                new Vector3(
                    transform.position.x - Mathf.Cos(angle),
                    transform.position.y  - Mathf.Sin(angle),
                    transform.position.z
                ),
                quaternion.Euler(0,0,angle)
            );
            obj.GetComponent<ProjectileScript>().SetLayers(gameObject.layer);
        }
        
    }
}
