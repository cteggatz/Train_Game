using UnityEngine;
using UnityEngine.UI;

public class BurnCoal : MonoBehaviour
{
    [SerializeField] GameObject trainController;

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fuel")
        {
            trainController.GetComponent<Train_Controller>().AddFuel(40f);
            Destroy(collision.gameObject);
        }
    }
}
