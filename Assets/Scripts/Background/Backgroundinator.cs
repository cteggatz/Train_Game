using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    private float backSpeed = 5f; 
    private Rigidbody2D rb;
    private float deSpawn = -115f;
    public BoxCollider2D bc;
    private GameObject Train;
    void Start()
    {
        bc = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-backSpeed, 0);
        Train = GameObject.FindWithTag("Train (primary)");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Train);
        backSpeed = Train.GetComponent<Train_Controller>().GetSpeed();
        if (transform.position.x < deSpawn){
            Destroy(this.gameObject);
        }
    }
    
}
