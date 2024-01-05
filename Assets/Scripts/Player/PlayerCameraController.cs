using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerCameraController : MonoBehaviour
{
    /** --- Player Camera Notes ----
    This class controls the camera relative to the players position!

    it achieves two things 
        - It switches the layers of the player
        - It which layers the camera sees
        - Broadcasts layer changes to any class that subscribes to the event listener


    Side note:
    I think this could use a refactor since it does stuff for both the player and the camera.
    Maybe switch it into a camera controller class and a player camera controller
    **/


    [Header("References")]
    [SerializeField] private Transform cameraFollowObj;
    [SerializeField] private Camera cam;



    [Header("Camera Look Settings")]
    [SerializeField] private float lookThreshHold = 3f;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float MinYLevel = -1f;
    [SerializeField] private float MaxYLevel = 10f;


    private bool canMoveOutside;

    /** 
    <summary>
    Broadcasts player layer change to subscribing classes.
    </summary>

    <returns>
    Returns a <c> LayerChangeArgs </c> Event arg which has a <c>layer</c> field that represents the player's current layer
    </returns>
    */
    
    public event EventHandler<LayerChangeArgs> OnLayerChange;
    public class LayerChangeArgs : EventArgs{
        public int layer;
    }
  
    void FixedUpdate()
    {
        /*
        ---- Camera Positioning Relative to the Player ----
        */
        Vector3 playerPos = new Vector3(
            transform.position.x + offset.x,
            transform.position.y + offset.y,
            transform.position.z + offset.y
        );
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = (playerPos + mousePos) / 2f;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x, 
            playerPos.x - lookThreshHold , 
            playerPos.x + lookThreshHold
        );
        targetPosition.y = Math.Clamp(
            Mathf.Clamp(targetPosition.y, playerPos.y - lookThreshHold , playerPos.y + lookThreshHold),
            MinYLevel,
            MaxYLevel
        );
        
        //Debug.Log(targetPosition.x);
        targetPosition.z = 0;

        cameraFollowObj.position = targetPosition;
    }


    void Update(){
        if(Input.GetKeyDown("e") && canMoveOutside){
            SwitchLayer(); //help
        }

    }

    public void SetMoveOutside(bool state){
        canMoveOutside = state;
    }
    
    /**
    <summary>

    </summary>
    <returns>
    <c>offset</c>
    </returns>
    */
    public Vector3 GetCameraOffset(){
        return offset;
    }

    public void SwitchLayer(){
        //switches the layer of the player between the two layers
        this.gameObject.layer = (this.gameObject.layer == 6) ? 7 : 6;

        //This switches off the binary bits of the camera culling mask
        //essentually, it is turning switching these layers from visibile to invisible
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Outside_Train");
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Inside_Train");


        //broadcasts newest changes to subscribers
        if(OnLayerChange != null) OnLayerChange(this, new LayerChangeArgs{layer = gameObject.layer});   
    }
}
