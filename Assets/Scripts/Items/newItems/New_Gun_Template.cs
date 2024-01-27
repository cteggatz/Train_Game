using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New_Gun_Template", menuName = "ScriptableObjects/Gun/New_Gun_Template")]
public class New_Gun_Template : Usable_Item
{
    public enum GunType {Primary, Secondary}
    public enum ShootType{Single, Spread, Burst, All}
    
    // ---- Gun Settings ----
    [Header("Gun Settings")]
    public bool automatic; /**<summary> determines wether you can hold down click or you need to actively click </summary> */
    public GunType gunType; /**<summary> Determins what slot the player can equipt it too </summary> */
    public ShootType shootType; /**<summary> Determins how the gun shoots the projectiles </summary> */
    public float reloadSpeed;
    [SerializeField] private ParticleSystem shell;
    [Min(0f)] public int burstAmmount;
    [Min(0f)] public float burstDelay;
    [Range(0f, 180f)] public float shotSpread;

    // ---- Bullet Settings ----
    [Header("Bullet Settings")]
    public GameObject projectile; /**<summary> Is the projectile that is shot </summary> */
    public int bulletDamage; 
    [Min(0)] public int thrust;


    // ---- Methods



    /**
    <summary>
    public function that is called in order to spawn bullets with gun spesifications.
    </summary>

    --- parameters ---
    <param name="angle">
        is the angle at which the gun is facing. This is the direction the bulletes will be facing.
    </param>
    <param name="layer">
        The layer where the bullets will be spawned
    </param>
    <param name="player">
        the position of the player
    </param>
    <param name="position">
        the position at which the bullets will be spawned
    </param>
    */
    public override void use(Transform player, Vector3 position, float angle, int layer){
        for(int i = 0; i < burstAmmount; i++){
            player
                .GetComponent<PlayerInventory>()
                .StartCoroutine(ShootWithDelay(i, angle, position, layer));
            Instantiate(shell, position, player.rotation, player); //the rotation should be baised on the weapon direction, not the player- this is a cheap fix.
        }
    }

    /**
    ///<summary>
    ///    Helper function that handles spawning bulletes at the correct angle with delay.
    ///</summary>

    It is an IEnumerator so that it can be a co-rutine. 
    the corutine is so that the bullets are instanciated with a delay


    */
    private IEnumerator ShootWithDelay(int index, float angle, Vector3 position, int layer){
        yield return new WaitForSeconds(burstDelay * index);
        float spreadOffset = (angle * Mathf.Rad2Deg - shotSpread * Mathf.Floor((float)burstAmmount/2) + shotSpread*index) * Mathf.Deg2Rad;
            GameObject obj = (GameObject) Instantiate(
                projectile,
                new Vector3(
                    position.x - Mathf.Cos(spreadOffset) - Mathf.Cos(spreadOffset) * sprite.bounds.extents.x,
                    position.y  - Mathf.Sin(spreadOffset) - Mathf.Sin(spreadOffset) * sprite.bounds.extents.x,
                    position.z
                ),
                Quaternion.Euler(0,0,spreadOffset * Mathf.Rad2Deg)
            );
        obj.GetComponent<ProjectileScript>().SetBulletArgs(layer, thrust, bulletDamage);
    }


}
