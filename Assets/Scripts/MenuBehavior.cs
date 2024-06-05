using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalWinsText;
    [SerializeField] Image vibrationButton;
    [SerializeField] Image soundsButton;
    [SerializeField] Slider difficultySlider;
    [SerializeField] LeanWindow exitPopup;
    AudioSource myAudio;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        LoadMenu();
        Vibration.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            exitPopup.On = true;
        }
    }

    void LoadMenu()
    {
        SetupTotalWinsText();
        SetupSoundsButton();
        SetupVibrationButton();
        SetupGameDifficultySlider();
        //sound gets enabled after setting up the menu because the slider plays sound when its value gets set
        myAudio.enabled = true;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ClearSaveData()
    {
        PlayerPrefs.SetInt("TotalWins",0);
        PlayerPrefs.SetInt("TotalLosses",0);
        PlayerPrefs.SetInt("TotalDraws",0);
        totalWinsText.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnSoundsButtonClick()
    {
        PlayerPrefs.SetInt("Sounds",1-PlayerPrefs.GetInt("Sounds",1));
        SetupSoundsButton();
    }

    void SetupSoundsButton()
    {
        soundsButton.color = PlayerPrefs.GetInt("Sounds",1)==1?new Color32(208,208,208,255):new Color32(0,0,0,76);
    }
    public void OnVibrationButtonClick()
    {
        PlayerPrefs.SetInt("Vibration",1-PlayerPrefs.GetInt("Vibration",1));
        SetupVibrationButton();
    }

    void SetupVibrationButton()
    {
        vibrationButton.color = PlayerPrefs.GetInt("Vibration",1)==1?new Color32(208,208,208,255):new Color32(0,0,0,76);
    }

    void SetupTotalWinsText()
    {
        int wins = PlayerPrefs.GetInt("TotalWins",0);
        totalWinsText.text = "total wins\n"+wins;
        if(wins>0) totalWinsText.gameObject.SetActive(true);
    }

    void SetupGameDifficultySlider()
    {
        difficultySlider.value = PlayerPrefs.GetInt("GameDifficulty",0);
    }

    public void OnGameDifficultyChange()
    {
        PlayerPrefs.SetInt("GameDifficulty",(int)difficultySlider.value);
    }

    public void PlayClickSound()
    {
        if(PlayerPrefs.GetInt("Sounds",1)==1) myAudio.Play();
    }

    public void PlaySmallVibration()
    {
        if(PlayerPrefs.GetInt("Vibration",1)==0) return;

        #if UNITY_IPHONE
            Vibration.VibratePop();    
        #endif

        #if UNITY_ANDROID
            Vibration.Vibrate (15);
        #endif
    }
}