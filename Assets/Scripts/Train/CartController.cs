using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface ISettableObject{
        public void SetObject(GameObject train);
}

public class CartController : MonoBehaviour
{
    [SerializeField] public Vector3 cartSize;

    [SerializeField] private Tilemap outsideCollision;
    [SerializeField] private Tilemap outsideNonCollision;
    [SerializeField] private Tilemap insideCollision;
    [SerializeField] private Tilemap insideNonCollision;
    [SerializeField] private Tilemap universal;

    [SerializeField] private GameObject center;
    [SerializeField] private List<GameObject> cartObjectSetter;

    [SerializeField] public string prefabReference;



    public List<Tilemap> GetTilemaps(){
        List<Tilemap> tilemaps = new List<Tilemap>();
        tilemaps.Add(outsideCollision);
        tilemaps.Add(outsideNonCollision);
        tilemaps.Add(insideCollision);
        tilemaps.Add(insideNonCollision);
        tilemaps.Add(universal);
        return tilemaps;
    }

    void Awake()
    {
        Destroy(center);
        //Debug.Log(this.prefabReference);
    }

    public void SetCart(GameObject obj, GameObject cart, bool existing = false){
        foreach(GameObject cartObject in cartObjectSetter){
            foreach(ISettableObject comp in cartObject.GetComponents<MonoBehaviour>().OfType<ISettableObject>().ToArray()){
                comp.SetObject(obj);
            }
        }    
        if(PrefabUtility.IsPartOfPrefabAsset(cart)){
            prefabReference = AssetDatabase.GetAssetPath(cart); 
        }  
        else {
            //Debug.Log($" existing cart | {cart.name} | {cart.GetComponent<CartController>().prefabReference}");
            prefabReference = cart.GetComponent<CartController>().prefabReference;
        }
    }
}

[CustomEditor(typeof(CartController))]
public class CartControllerGUI : Editor{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CartController cart = (CartController) target;

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        //GUILayout.TextField($"Prefab Reference : {cart.prefabReference}");

        if(GUILayout.Button("Calabrate Size")){
            Vector3 newSize = new Vector3();
            List<Tilemap> tilemaps = cart.GetTilemaps();
            foreach(Tilemap map in tilemaps){
                map.CompressBounds();
                newSize = new Vector3(
                    (map.size.x > newSize.x)? map.size.x : newSize.x,
                    (map.size.y > newSize.y)? map.size.y : newSize.y,
                    (map.size.z > newSize.z)? map.size.z : newSize.z
                );
            }
            cart.cartSize = newSize;
        }
        this.Repaint();
    }
}