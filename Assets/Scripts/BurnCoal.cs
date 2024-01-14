using UnityEngine;
using UnityEngine.UI;

public class BurnCoal : MonoBehaviour
{
    private float timer, fuel;
    [SerializeField] private float delayAmount;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delayAmount)
        {
            timer = 0f;
            if(fuel >= 0){
                fuel -= 0.5f;
            }
        }
        Debug.Log($"Fuel : {fuel}");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fuel")
        {
            fuel += 10;
            Destroy(collision.gameObject);
        }
    }
}
