using UnityEngine;

public class spawnOnTrain : MonoBehaviour
{
    public Vector3 spawn;
    [SerializeField] private Vector3 hoardpos;
    [SerializeField] private float speed;
    private Transform grunt;

    // Update is called once per frame
    private void Start()
    {
        gameObject.transform.position = hoardpos;
        grunt = gameObject.transform.GetChild(0);
        grunt.gameObject.SetActive(false);
    }
    void Update()
    {
        float smooth = speed * Time.deltaTime;
        if(transform.position.x != spawn.x){
            transform.position = new Vector3(transform.position.x + smooth, transform.position.y, transform.position.z);
        }
        if(transform.position.y != spawn.y){
            transform.position = new Vector3(transform.position.x, transform.position.y  + smooth, transform.position.z);
        }
        if(transform.position.x >= spawn.x){
            grunt.gameObject.SetActive(true);
            grunt.GetComponent<Igiveup>().target = GameObject.FindGameObjectWithTag("Player").transform;
            grunt.gameObject.SetActive(true);
            //gameObject.transform.GetChild(0).gameObject.transform.z = 1;
            grunt.parent = null;
            Destroy(gameObject);
        }
    }
}
