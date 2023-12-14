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

    void Start()
    {
       // _camera = transform.GetComponent<CinemachineBrain>();
    }

    void Update(){
        if(Input.GetKeyDown("e") == true && canMoveOutside){
            SwitchLayer();       
        }
    }

    
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

    public void SwitchLayer(){
        this.gameObject.layer = ((this.gameObject.layer == 6) ? 7 : 6);
        //flipping the bit of the camera culling mask.
        //IDK WTF this code does but its what I was told to do lol.
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Outside_Train");
        cam.cullingMask ^= 1 << LayerMask.NameToLayer("Inside_Train");   
    }

    //adding layer changes
    void OnTriggerEnter2D(Collider2D other){
        if(other.transform.tag == "Train_Exit"){
            canMoveOutside = true;
        }
    }
    //
    void OnTriggerExit2D(Collider2D other){
        if(other.transform.tag == "Train_Exit"){
            canMoveOutside = false;
        }
    }
}
