using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollowMouse : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    [Header("Camera Look Settings")]
    [SerializeField] private float lookThreshHold = 3f;
    [SerializeField] private Vector3 offset;
    //private CinemachineBrain _camera;


    void Start()
    {
       // _camera = transform.GetComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = (player.position + mousePos) / 2f;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x, 
            player.position.x - lookThreshHold , 
            player.position.x + lookThreshHold
        );
        targetPosition.y = Mathf.Clamp(
            targetPosition.y, 
            player.position.y - lookThreshHold , 
            player.position.y + lookThreshHold
        );
        Debug.Log(targetPosition.x);
        targetPosition.z = 0;

        this.transform.position = targetPosition;
    }
}
