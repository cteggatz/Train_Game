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
    /*
    // ---- Graphics Settings ----
    [Header("Graphics")]
    public Sprite sprite;
    public Vector3 sprite_Size = new Vector3(1,1,1);

    // ---- General setting ----
    [Header("General")]
    public new string name;
    public string description;

    */

    [Header("Bullet")]
    public GameObject projectile;
    public int bulletDamage;
    [Min(0)] public int bulletThrust;

    // ---- Gun Settings ----
    [Header("Gun")]
    //[Min(0)] public int magazineSize;
    //[Min(0f)] public float reloadTime;
    //[Min(0f)] public float shootCooldown;
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
    // -- magazine settings -- 
    // ---- Methods ----
    public override void Use(PlayerItemController itemController, float angle, Vector3 position, float spawnDistance, int layer){
        for(int i = 0; i < bulletNumber; i++){
            itemController.StartCoroutine(ShootWithDelay(i, angle, position, spawnDistance, layer));
        }
    }
    private IEnumerator ShootWithDelay(int index, float angle, Vector3 position, float spawnDistance, int layer){
        yield return new WaitForSeconds(bulletDelay * index);
        float spreadOffset = (angle * Mathf.Rad2Deg - shotSpread * Mathf.Floor((float)bulletNumber/2) + shotSpread*index) * Mathf.Deg2Rad;
            GameObject obj = (GameObject) Instantiate(
                projectile,
                new Vector3(
                    position.x - Mathf.Cos(spreadOffset) * spawnDistance,
                    position.y  - Mathf.Sin(spreadOffset) * spawnDistance,
                    position.z
                ),
                Quaternion.Euler(0,0,spreadOffset * Mathf.Rad2Deg)
            );
        obj.GetComponent<ProjectileScript>().SetBulletArgs(layer, bulletThrust, bulletDamage);
    }
}
