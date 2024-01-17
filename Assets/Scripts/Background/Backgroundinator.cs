using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    private float backSpeed = 5.0f;
    private Rigidbody2D rb;
    private float deSpawn = -30f;
    public BoxCollider2D bc;
    void Start()
    {
        bc = this.GetComponent<BoxCollider2D>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-backSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
       if (transform.position.x < deSpawn){
            Destroy(this.gameObject);
       }
    }
}
