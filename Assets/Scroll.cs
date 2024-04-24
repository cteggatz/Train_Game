using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Scroll : MonoBehaviour
{
    private Rigidbody2D rb;
    private float deSpawn = -115f, spawnpos = 100f;
    private GameObject Train;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-5f, 0);
        Train = GameObject.FindWithTag("Train (primary)");
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(-Train.GetComponent<Train_Controller>().GetSpeed(), 0);
        if(transform.position.x == spawnpos)
        {
            Instantiate(gameObject, transform.position, transform.rotation);
        }
        if (transform.position.x < deSpawn)
        {
            Destroy(this.gameObject);
        }
    }
}
