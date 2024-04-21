using System;
using AttributesLibrary.SceneSelect;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPanelControll : MonoBehaviour
{
    [SerializeField] private bool OwnScene;
    [SerializeField,SceneSelect] private int transitionLevel;
    
    [SerializeField] private Button transitonLvlBtn;
    [SerializeField] private Button mainMenuBtn;
    
    private string path = "LevelData.txt";

    [SerializeField] private int Stars;

    [SerializeField] private bool Win;

    private void OnValidate()
    {
        if (OwnScene)
        {
            transitionLevel = SceneManager.GetActiveScene().buildIndex;
        }
    }

    void OnEnable()
    {
        
        transitonLvlBtn.onClick.AddListener(NextLvlBtnOnclicked);
        mainMenuBtn.onClick.AddListener(MainMenuBtnOnClicked);
    }

    

    private void OnDisable()
    {
        
        transitonLvlBtn.onClick.RemoveListener(NextLvlBtnOnclicked);
        mainMenuBtn.onClick.RemoveListener(MainMenuBtnOnClicked);
    }

    private void MainMenuBtnOnClicked()
    {
        if (Win)
        {
            AllLevelsObject levelsObject = SaveSystem.Load<AllLevelsObject>(path);
            for (var i = 0; i < levelsObject.levelDatas.Count; i++)
            {
                if (levelsObject.levelDatas[i].LvlName == SceneManager.GetActiveScene().name)
                {
                    LevelData currentLevel = levelsObject.levelDatas[i];
                    currentLevel.StarCount = Stars;
                    LevelData nextLevel = levelsObject.levelDatas[i+1];
                    nextLevel.locked = false;
                    levelsObject.levelDatas[i] = currentLevel;
                    levelsObject.levelDatas[i+1] = nextLevel;
                }
            }
            SaveSystem.Save<AllLevelsObject>(path, levelsObject);
        }
        SceneManager.LoadScene("MainMenu");
    }
    private void NextLvlBtnOnclicked()
    {
        if (Win)
        {
            AllLevelsObject levelsObject = SaveSystem.Load<AllLevelsObject>(path);
            for (var i = 0; i < levelsObject.levelDatas.Count; i++)
            {
                if (levelsObject.levelDatas[i].LvlName == SceneManager.GetActiveScene().name)
                {
                    LevelData currentLevel = levelsObject.levelDatas[i];
                    currentLevel.StarCount = Stars;
                    LevelData nextLevel = levelsObject.levelDatas[i+1];
                    nextLevel.locked = false;
                    levelsObject.levelDatas[i] = currentLevel;
                    levelsObject.levelDatas[i+1] = nextLevel;
                }
            }
            SaveSystem.Save<AllLevelsObject>(path, levelsObject);
        }
        SceneManager.LoadScene(transitionLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
