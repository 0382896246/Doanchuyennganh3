using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Main_UI ui;
    public Player player;
    public bool colorEntirePlatform;
    private PostProcessController postProcessController;

    [Header("SkyBox Material")]
    [SerializeField] private Material[] skyBoxMat;

    [Header("Purchased Color")]
    public Color flatformColor;
    
    

    [Header("Score Info")]
    public int coins;
    public float distance;
    public float score;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;

        SetupSkyBox(PlayerPrefs.GetInt("SkyBoxSettings"));
        //LoadColor();
        postProcessController = FindAnyObjectByType<PostProcessController>();
    }

    public void SaveColor(float r, float g, float b)
    {
        PlayerPrefs.SetFloat("ColorR", r);
        PlayerPrefs.SetFloat("ColorG", g);
        PlayerPrefs.SetFloat("ColorB", b);
    }
    private void LoadColor()
    {
       SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        Color newColor = new Color(PlayerPrefs.GetFloat("ColorR"),
                                   PlayerPrefs.GetFloat("ColorG"),
                                   PlayerPrefs.GetFloat("ColorB"),
                                   PlayerPrefs.GetFloat("ColorA",1));
        sr.color = newColor;    
    }

    public void SetupSkyBox(int i)
    {
        if (i <= 1)
        {

        RenderSettings.skybox= skyBoxMat[i];
        }
        else
        {
            RenderSettings.skybox = skyBoxMat[Random.Range(0,skyBoxMat.Length)];
        }

        PlayerPrefs.SetInt("SkyBoxSettings", i);
    }
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        if (player.transform.position.x > distance)
        {
            distance = player.transform.position.x;  // khoảng cách player đã đi
        }
    }

    public void UnlockPlayer()=> player.playerUnlocked = true;
    public void RestartLevel()
    {
        //SaveInfo();
        SceneManager.LoadScene(0);
    }
    
    public void SaveInfo()
    {
        int savedCoins = PlayerPrefs.GetInt("Coins");
        
        PlayerPrefs.SetInt("Coins",savedCoins+coins); 
        // lấy coin đã có từ những lần trc cộng với coin đã lưu từ lần chơi này

         score= distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

       
        // lấy số điểm lớn nhất 
        if (PlayerPrefs.GetFloat("HighScore") < score)
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
       
    }

    public void GameEnded()
    {
        SaveInfo() ;
        postProcessController.colorGradingOff = true;
        ui.OpenEndGameUI();
    }
}
