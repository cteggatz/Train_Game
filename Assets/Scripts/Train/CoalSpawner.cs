using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---- Coal Spawner ----
[RequireComponent(typeof(BoxCollider2D), typeof(LayerHelper))]
public class CoalSpawner : MonoBehaviour
{
    [SerializeField] private  GameObject coal;

    private List<GameObject> GlobalCoalList = new List<GameObject>();

    [SerializeField] private int maxCoalCount = 5;

    void OnTriggerExit2D(Collider2D collider){
        PlayerInventory inventory = collider.gameObject.GetComponent<PlayerInventory>();
        if(collider.gameObject.GetComponent<PlayerInventory>() != null){
            inventory.canCreatCoal = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collider){
        PlayerInventory inventory = collider.gameObject.GetComponent<PlayerInventory>();
        if(collider.gameObject.GetComponent<PlayerInventory>() != null){
            inventory.canCreatCoal = true;
        }
    }

    public GameObject SpawnCoal(){
        for(int i = 0; i < GlobalCoalList.Count; i++){
            if(GlobalCoalList[i] == null)GlobalCoalList.RemoveAt(i);
        }
        if(GlobalCoalList.Count == maxCoalCount){
            Destroy(GlobalCoalList[0]);
            GlobalCoalList.RemoveAt(0);
        }

        GameObject obj = Instantiate(coal);
        LayerHelper.SwitchLayers(gameObject.GetComponent<LayerHelper>().GetLayer(), obj);

        GlobalCoalList.Add(obj);
        return obj;
    }

}