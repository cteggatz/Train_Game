using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Generic Template for all guns in game
/// </summary>
/// ----Chris Notes----
/// This needs the change! We need to implement an gun interface so we can have more then just basic guns!
/// ----Chris Notes----


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
    [SerializeField] private ParticleSystem shell;
    public float reloadSpeed; /**<summary>The time it takes to reload the gun</summary>*/
    [Min(0f)] public int burstAmmount; /**<summary>The ammount of bullets shot per click</summary>*/
    [Min(0f)] public float burstDelay; /**<summary>The time between bullet shots in a burst</summary>*/
    [Range(0f, 180f)] public float shotSpread; /**<summary>The angle between all the bullets in a shot</summary>*/

    // ---- Bullet Settings ----
    [Header("Bullet Settings")]
    public GameObject projectile; /// <summary> Is the projectile that is shot </summary>
    public int bulletDamage; /// <summary>The damage value passed to the bullet controller</summary>
    [Min(0)] public int thrust; /// <summary> The speed value passed to the bullet controller </summary>



    // ---- Methods


    /// <summary>
    /// Spawns a burst of bullets
    /// </summary>
    /// <param name="player"> The current bullet that is being shot in a burst </param>
    /// <param name="position"> The position at which the bullet is being instantiated </param>
    /// <param name="angle"> The angle at which the gun is being pointed </param>
    /// <param name="layer"> the layer that the bullet will be spawned in </param>
    /// --------
    /// just does a forloop of the burst ammount to spawn in bullets with helper function ShootWithDelay
    /// -------
    public override void use(Transform player, Vector3 position, float angle, int layer){
        for(int i = 0; i < burstAmmount; i++){
            player
                .GetComponent<PlayerInventory>()
                .StartCoroutine(ShootWithDelay(i, angle, position, layer, player.gameObject));
        }
    }

    
    /// <summary>
    /// Helper function that handles spawning bulletes at the correct angle with delay.
    /// </summary>
    /// <param name="index"> The current bullet that is being shot in a burst </param>
    /// <param name="angle"> The angle at which the gun is being pointed </param>
    /// <param name="position"> The position at which the bullet is being instantiated </param>
    /// <param name="layer"> the layer that the bullet will be spawned in </param>
    /// <param name="parent"> information about the game object that summoned the bullet</param>
    /// --------
    /// It is an IEnumerator so that it can be a co-rutine. 
    /// the corutine is so that the bullets are instanciated with a delay.
    /// -------
    private IEnumerator ShootWithDelay(int index, float angle, Vector3 position, int layer, GameObject parent){
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
        obj.GetComponent<ProjectileScript>().SetBulletArgs(layer, thrust, bulletDamage, parent);
    }


}
