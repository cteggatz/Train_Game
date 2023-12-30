using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerCameraController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform cameraFollowObj;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject BackgroundBlur;

    [Header("Camera Look Settings")]
    [SerializeField] private float lookThreshHold = 3f;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float MinYLevel = -1f;
    [SerializeField] private float MaxYLevel = 10f;


    private bool canMoveOutside;

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
    public Vector3 GetCameraOffset(){
        return offset;
    }

    public void SwitchLayer(){
        //switches the layer of the player between the two layers
        //this.gameObject.layer = ((this.gameObject.layer == 6) ? 7 : 6);

        if(this.gameObject.layer == 6){
            this.gameObject.layer = 7;
            BackgroundBlur.SetActive(false);
        } else {
            this.gameObject.layer = 6;
            BackgroundBlur.SetActive(true);
        }
        //IDK WTF this code does but its what I was told to do lol.
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Outside_Train");
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Inside_Train");

        if(OnLayerChange != null) OnLayerChange(this, new LayerChangeArgs{layer = gameObject.layer});   
    }
}
