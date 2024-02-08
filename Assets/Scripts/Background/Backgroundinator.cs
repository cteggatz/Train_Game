using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    private Rigidbody2D rb;
    private float deSpawn = -115f;
    public BoxCollider2D bc;
    private GameObject Train;
    void Start()
    {
        bc = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-5f, 0);
        Train = GameObject.FindWithTag("Train (primary)");
    }

    void FixedUpdate()
    {
        //Debug.Log(Train.GetComponent<Transform>().position.x);
        //Debug.Log(backSpeed);
        rb.velocity = new Vector2(-Train.GetComponent<Train_Controller>().GetSpeed(), 0);
        if (transform.position.x < deSpawn){
            Destroy(this.gameObject);
        }
    }
    
}
