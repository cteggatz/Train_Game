using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;




//i need to change this, this needs to be in its own namespace etc.
public class Furnace : MonoBehaviour, ISettableObject
{
    private GameObject trainController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fuel")
        {
            trainController.GetComponent<Train_Controller>().AddFuel(40f);
            Destroy(collision.gameObject);
        }
    }
    public void SetObject(GameObject train){
        trainController = train;
    }
}