using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] string SceneToGo;
    [SerializeField] string SpawnPointName;
    [SerializeField] string Area;
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.collider.tag == "Player")
    //    {
    //        Debug.Log("HALLO");
    //        GameManager.instance.ChangeScene(SceneToGo);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.instance.ChangeScene(SceneToGo, SpawnPointName, Area);
        }
    }
}
