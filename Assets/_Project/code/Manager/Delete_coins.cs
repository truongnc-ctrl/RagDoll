using UnityEngine;

public class Delete_coins : MonoBehaviour
{
    void Update()
    {
        PlayerPrefs.DeleteKey("coin");
    }
}
