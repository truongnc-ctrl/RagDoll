using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;

public class Close_buttons : MonoBehaviour
{
    public GameObject Coins;
    public void CloseThisButton()
    {
        this.gameObject.SetActive(false);
        Coins.SetActive(false);

    }
}
