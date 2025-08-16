using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ColorToSell
{
    public Color color;
    public float price;
}

public enum ColorType
{
    playerColor,
    platformColor
}

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI notifyText;
    [Space]

    [Header("Platform Color")]
    [SerializeField] private GameObject platformColorbutton;
    [SerializeField] private Transform platformParent;
    [SerializeField] private Image platformDisplay;
    [SerializeField] private ColorToSell[] platformColors;

    [Header("Player Color")]
    [SerializeField] private GameObject playerColorButton;
    [SerializeField] private Transform playerParent;
    [SerializeField] private Image playerDisplay;
    [SerializeField] private ColorToSell[] playerColors;


    void Start()
    {
        coinsText.text= PlayerPrefs.GetInt("Coins").ToString("#,#");

        for (int i = 0; i < platformColors.Length; i++)
        {
            Color color = platformColors[i].color;
            float price = platformColors[i].price;

            GameObject newButton = Instantiate(platformColorbutton,platformParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color=color ;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString("#,#");

            newButton.GetComponent<Button>().onClick.AddListener(() => PurchaseColor(color,price,ColorType.platformColor));
        }

        for (int i = 0; i < playerColors.Length; i++)
        {
            Color color = playerColors[i].color;
            float price = playerColors[i].price;

            GameObject newButton = Instantiate(playerColorButton, playerParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color = color;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString("#,#");

            newButton.GetComponent<Button>().onClick.AddListener(() => PurchaseColor(color, price, ColorType.playerColor));
        }
    }

   public void PurchaseColor(Color color, float price,ColorType colorType)
    {
        AudioManager.instance.PlaySFX(4);
        if (EnoughMoney(price))
        {
            if (colorType == ColorType.platformColor)
            {

                GameManager.instance.flatformColor = color;
                platformDisplay.color = color;
            }
            else if (colorType == ColorType.playerColor)
            {
                GameManager.instance.player.GetComponent<SpriteRenderer>().color= color;
                GameManager.instance.SaveColor(color.r, color.g, color.b);
                playerDisplay.color = color;
            }

            StartCoroutine(Notify("Purchased Successful", 1));
        }
        else
            StartCoroutine(Notify("Not enough money!", 1));

    }

    private bool EnoughMoney(float price)
    {
        int myCoins = PlayerPrefs.GetInt("Coins");

        if (myCoins >= price)
        {
            int newAmountOfCoins = myCoins - (int)price;
            PlayerPrefs.SetInt("Coins",newAmountOfCoins);
            coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");

            //    Debug.Log("Purchase Successful");
            return true;
        }
       // Debug.Log("not enough money");
        return false;
    }

    IEnumerator Notify(string text, float seconds)
    {
        notifyText.text = text;

        yield return new WaitForSeconds(seconds);

        notifyText.text = "Clicks to buy";
    }
}
