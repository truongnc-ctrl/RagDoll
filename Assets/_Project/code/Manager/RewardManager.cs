using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [SerializeField] private GameObject PileOfCoinsParent;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private GameObject Coin_VFX;
    public Button NextButton;
    public TextMeshProUGUI count_text;
    public TextMeshProUGUI Reward_text;
    public int amount = 1000;

    private const string COIN_KEY = "coin";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (PileOfCoinsParent != null)
        {
            foreach (Transform child in PileOfCoinsParent.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        NextButton.gameObject.SetActive(false);
        UpdateCoinText(PlayerPrefs.GetInt(COIN_KEY, 0));
        Reward_text.text = amount.ToString();
        Coin_VFX.SetActive(false);

    }

    public void RewardPileOfCoins()
    {
        PileOfCoinsParent.SetActive(true);
        Coin_VFX.SetActive(true);
        StartCoroutine(CountCoinsRoutine(amount));
    }

    IEnumerator CountCoinsRoutine(int amountToAdd)
    {
        yield return new WaitForSecondsRealtime(2.2f);
        int currentCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
        int targetCoins = currentCoins + amountToAdd;
        PlayerPrefs.SetInt(COIN_KEY, targetCoins);
        PlayerPrefs.Save();
        UpdateCoinText(targetCoins);
        NextButton.gameObject.SetActive(true);
    }

    private void UpdateCoinText(int value)
    {
        if (count_text != null )
        {
            count_text.text = value.ToString();
        }
    }
}