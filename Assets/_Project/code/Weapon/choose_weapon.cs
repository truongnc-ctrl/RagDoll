using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class choose_weapon : MonoBehaviour
{
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;

    void Start()
    {
        this.gameObject.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            gameObject.SetActive(true);
            Debug.Log("ok");
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
