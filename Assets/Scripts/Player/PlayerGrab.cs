using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Point where a grabed object will teleport")] [SerializeField] private Transform grabPoint;
    [Tooltip("Point where the ray is casted from")] [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDistance; //This should be self explanitory
    [SerializeField]  private GameObject coalPrefab,  grabbedObject; //... the current grabbed object
    
    /*
        Note for Tino:
            I have changed the grab script to work with the weapon script.
            Your logic is still here, I just moved somethings around, added some stuff, and simplified it.
            I added:
            [1] ability to detect when things are destroyed by furnace
            [2] stopped script from casting rays 2047 for performance
            [3] event handler for when you pick up and drop stuff.

            Basically when you pick up stuff, the event will trigger and the Item Controller script will know
            that you picked up something. Then the Item Controller will basically stop doing anything until the item is 
            put down or destroyed.

            If you dont like it, I kept your old implementation of the grab script and feel free to edit it to how you like.
    */
    private bool isGrabbing;
    public event EventHandler<GrabArgs> OnGrabInteraction;
    public class GrabArgs{public bool isGrabbed;}

    private void SetGrabbing(bool state){
        OnGrabInteraction(this, new GrabArgs{isGrabbed = state});
            isGrabbing = state;
    }
    void Update()
    {
        /*
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

                isGrabbing = true;
            }
            //SHITTY LOGIC TO SPAWN A ITEM
            if (Input.GetKey("f") && grabbedObject == null && hitinfo.collider.gameObject.name == "Grabbable - Spawner")
            {
                GameObject coal = Instantiate(coalPrefab);
                grabbedObject = coal;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.position = grabPoint.position;
                grabbedObject.transform.SetParent(transform);

                isGrabbing = true;
            }
        }
        if (Input.GetKey("f") == false && grabbedObject != null)
        {
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
            grabbedObject.transform.SetParent(null);
            grabbedObject = null;
            isGrabbing = false;
        } 
        //Debug.DrawRay(rayPoint.position, transform.right * rayDistance);
        */
    
        if(Input.GetKey("f") && !isGrabbing)
        {
            //casts ray at foot to see if then can grab anything
            RaycastHit2D hitinfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
            if(hitinfo.collider == null)return;


            //checks if the object is one of the grabbable things, if not then function will return;
            if(hitinfo.collider.gameObject.tag == "Fuel") { 
                grabbedObject = hitinfo.collider.gameObject; 
            } 
            else if(hitinfo.collider.gameObject.name == "Grabbable - Spawner") { 
                grabbedObject = Instantiate(coalPrefab); 
            }
            else return;
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
            grabbedObject.transform.position = grabPoint.position;
            grabbedObject.transform.SetParent(transform);

            SetGrabbing(true);
        }else if ( isGrabbing && (Input.GetKeyUp(KeyCode.F) == true || grabbedObject == null)){
            //if item exits, gets rid of it and puts it on ground
            if(grabbedObject != null){
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                grabbedObject.transform.SetParent(null);
                grabbedObject = null;
            }
            SetGrabbing(false);
        } 
    }
}
