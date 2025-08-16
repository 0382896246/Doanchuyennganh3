using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main_UI : MonoBehaviour
{

    private bool gamePause;
    private bool gameMuted;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGame;

    [Space]
    [SerializeField] private TextMeshProUGUI lastScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Volume Info")]
    [SerializeField] private UI_VolumeSlider[] slider;
    [SerializeField] private Image muteIcon;
    [SerializeField] private Image inGameMuteIcon;
    private void Start()
    {
        for (int i = 0; i < slider.Length; i++)
        {
            slider[i].SetupSlider();
        }

        SwitchMenuTo(mainMenu);
     

        lastScoreText.text= "Last Score: "+ PlayerPrefs.GetFloat("LastScore").ToString("#,#");
        highScoreText.text= "High Score: " + PlayerPrefs.GetFloat("HighScore").ToString("#,#");
    }

    public void MutedButton()
    {
        Color color = new Color(1, 1, 1, .5f);
        gameMuted =!gameMuted;

        if (gameMuted)
        {
            muteIcon.color = color;
            AudioListener.volume = 0;
        }
        else
        {
            muteIcon.color = Color.white;
            AudioListener.volume= 1;    
        }
    }

    public void SwitchSkyBox(int index)
    {
        AudioManager.instance.PlaySFX(4);
        GameManager.instance.SetupSkyBox(index);
    }
    public void SwitchMenuTo(GameObject uiMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        uiMenu.SetActive(true);

        AudioManager.instance.PlaySFX(4);
        coinsText.text= PlayerPrefs.GetInt("Coins").ToString("#,#");
    }

    public void StartGameButton()
    {
        muteIcon=inGameMuteIcon;
        if (gameMuted)
        
            muteIcon.color = new Color(1, 1, 1, .5f);
        GameManager.instance.UnlockPlayer(); 
    }

    public void PauseGameButton()
    {
        if (gamePause)
        {
            Time.timeScale = 1;
            gamePause = false;
        }
        else
        {
            Time.timeScale = 0;
            gamePause = true;
        }
    }

    public void RestartGameButton()=> GameManager.instance.RestartLevel();

    public void OpenEndGameUI()
    {
        SwitchMenuTo(endGame);
    }
       
}
