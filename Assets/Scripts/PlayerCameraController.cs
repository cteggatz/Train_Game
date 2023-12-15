using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform cameraFollowObj;
    [SerializeField] private Camera cam;

    [Header("Camera Look Settings")]
    [SerializeField] private float lookThreshHold = 3f;
    [SerializeField] private Vector3 offset;
    //private CinemachineBrain _camera;


    private bool canMoveOutside;
  
    void FixedUpdate()
    {
        /*
        ---- Camera Positioning Relative to the Player ----
        */
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = (this.transform.position + mousePos) / 2f;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x, 
            this.transform.position.x - lookThreshHold , 
            this.transform.position.x + lookThreshHold
        );
        targetPosition.y = Mathf.Clamp(
            targetPosition.y, 
            this.transform.position.y - lookThreshHold , 
            this.transform.position.y + lookThreshHold
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

    public void SwitchLayer(){
        Debug.Log("switch");
        this.gameObject.layer = ((this.gameObject.layer == 6) ? 7 : 6);
        //flipping the bit of the camera culling mask.
        //IDK WTF this code does but its what I was told to do lol.
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Outside_Train");
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Inside_Train");   
    }
}
