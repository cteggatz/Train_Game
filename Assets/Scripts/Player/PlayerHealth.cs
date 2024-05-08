using DataSaving;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, ISavable
{
    public float health;
    // Start is called before the first frame update
    public void Save(ref GameData data){

    }
    public void Load(ref GameData data){
        if(data.playerInitData.playerHealth == false){
            this.health = 100f;
            data.playerHealth = (int)health;
            data.playerInitData.playerHealth = true;
        }
        this.health = data.playerHealth;
    }
}
