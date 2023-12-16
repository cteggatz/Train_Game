using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Point where a grabed object will teleport")] [SerializeField] private Transform grabPoint;
    [Tooltip("Point where the ray is casted from")] [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDistance; //This should be self explanitory
    [SerializeField]  private GameObject grabbedObject, coalPrefab; //... the current grabbed object
    void Start()
    {
        //LayerMask mask = LayerMask.GetMask("Defult");
    }
    void Update()
    {
        //We should probobly remove all this kinematic stuff. And this script works by picking up a item that is at the base of the player
        RaycastHit2D hitinfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
        if (hitinfo.collider != null)
        {
            if (Input.GetKey("f") && grabbedObject == null && hitinfo.collider.gameObject.tag == "Fuel")
            {
                grabbedObject = hitinfo.collider.gameObject;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.position = grabPoint.position;
                grabbedObject.transform.SetParent(transform);
            }
            //SHITTY LOGIC TO SPAWN A ITEM
            if (Input.GetKey("f") && grabbedObject == null && hitinfo.collider.gameObject.name == "Grabbable - Spawner")
            {
                GameObject coal = Instantiate(coalPrefab);
                grabbedObject = coal;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.position = grabPoint.position;
                grabbedObject.transform.SetParent(transform);
            }
        }
        if (Input.GetKey("f") == false && grabbedObject != null)
        {
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
            grabbedObject.transform.SetParent(null);
            grabbedObject = null;
        }
        Debug.DrawRay(rayPoint.position, transform.right * rayDistance);
    }
}
