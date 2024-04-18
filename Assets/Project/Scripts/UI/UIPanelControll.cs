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
        
    }
    private void NextLvlBtnOnclicked()
    {
        SceneManager.LoadScene(transitionLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
