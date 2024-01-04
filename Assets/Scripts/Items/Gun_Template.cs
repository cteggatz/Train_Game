using System.Collections;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Test_Gun", menuName = "ScriptableObjects/Gun/Test_Gun")]
public class Gun : Item_Template
{
    public enum GunType{SingleFire, SpreadFire, BurstFire, Custom}

    [Header("Bullet")]
    public GameObject projectile;
    public int bulletDamage;
    [Min(0)] public int bulletThrust;

    // ---- Gun Settings ----
    [Header("Gun")]
    public GunType gunType;

    [Min(0f)] int bulletNumber;
    [Range(0f, 180f)] float shotSpread;
    [Min(0f)] float bulletDelay;

    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(Gun))]
    public class GunEditor: Editor{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Gun gun = (Gun)target;

            EditorGUI.indentLevel ++;
            switch(gun.gunType){
                case GunType.SingleFire:
                    gun.bulletNumber = 1;
                    gun.shotSpread = 0;
                    gun.bulletDelay = 0;
                    break;
                case GunType.SpreadFire:
                    gun.bulletNumber = EditorGUILayout.IntField("Number of Bullets",gun.bulletNumber);
                    gun.shotSpread = EditorGUILayout.FloatField("Multi Shot Spread",gun.shotSpread);
                    gun.bulletDelay = 0;
                    break;
                case GunType.BurstFire:
                    gun.bulletNumber = EditorGUILayout.IntField("Number of Bullets",gun.bulletNumber);
                    gun.shotSpread = 0;
                    gun.bulletDelay = EditorGUILayout.FloatField("Burst Fire Delay",gun.bulletDelay);
                    break;
                default:
                    gun.bulletNumber = EditorGUILayout.IntField("Number of Bullets",gun.bulletNumber);
                    gun.shotSpread = EditorGUILayout.FloatField("Multi Shot Spread",gun.shotSpread);
                    gun.bulletDelay = EditorGUILayout.FloatField("Burst Fire Delay",gun.bulletDelay);
                    break;
            }
            gun.useIncrement = gun.bulletNumber;
            EditorGUI.indentLevel --;
        }
    }
#endif
    #endregion

    // ---- Methods ----
    
    //this handles the call to use the gun. Calls the Shoot function to spawn bullets.
    public override void Use(PlayerItemController itemController, float angle, Vector3 position, float spawnDistance, int layer){
        for(int i = 0; i < bulletNumber; i++){
            itemController.StartCoroutine(ShootWithDelay(i, angle, position, spawnDistance, layer));
        }
    }

    /*
    this handles spawning in the bullets for the gun. I know its confusing as shit
    Basically: 
        [*] in order to give the effect of a burst, I am creating corutines (simolar to loading the job onto another thread) and having those corutines start at different times.
        [*] in order to give the effect of a shotgun, I am taking the initial angle of the input handler and subtracting the angle offset of the particular bullet in the burst.
        [*] finally, when spawining the bullet, it is adding in half of the guns length in order for the bullet to look like its coming out of the gun.
    */
    private IEnumerator ShootWithDelay(int index, float angle, Vector3 position, float spawnDistance, int layer){
        yield return new WaitForSeconds(bulletDelay * index);
        float spreadOffset = (angle * Mathf.Rad2Deg - shotSpread * Mathf.Floor((float)bulletNumber/2) + shotSpread*index) * Mathf.Deg2Rad;
            GameObject obj = (GameObject) Instantiate(
                projectile,
                new Vector3(
                    position.x - Mathf.Cos(spreadOffset) * spawnDistance - Mathf.Cos(spreadOffset) * sprite.bounds.extents.x,
                    position.y  - Mathf.Sin(spreadOffset) * spawnDistance - Mathf.Sin(spreadOffset) * sprite.bounds.extents.x,
                    position.z
                ),
                Quaternion.Euler(0,0,spreadOffset * Mathf.Rad2Deg)
            );
        obj.GetComponent<ProjectileScript>().SetBulletArgs(layer, bulletThrust, bulletDamage);
    }
}
