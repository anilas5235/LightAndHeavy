using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.LevelObjects;
using Project.Scripts.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelState : MonoBehaviour
{
    [SerializeField] private FinishDoor[] doors = new FinishDoor[2];
    [SerializeField] private SettingsHandler handler;
    [SerializeField] private UIPanelControll winControl;
    [SerializeField] private UIPanelControll pauseControl;
    [SerializeField] private UIPanelControll loseControl;

    [SerializeField] private Timer timer;

    private bool Pause;

    private MainInput input;

    private void OnEnable()
    {
        foreach (FinishDoor finishDoor in doors)
        {
            finishDoor.onDoorOpened.AddListener(DoorOpenedCheck);
        }
        
        input.Enable();

    }

    private void Awake()
    {
        input = new MainInput();
        input.KeyBoardPlayer.Escape.performed += EscapeOnperformed;
    }

    private void EscapeOnperformed(InputAction.CallbackContext obj)
    {
        if (Pause)
        {
            Pause = false;
            pauseControl.gameObject.SetActive(false);
            handler.Deactivate();
            Time.timeScale = 1;
        }
        else
        {
            Pause = true;
            pauseControl.gameObject.SetActive(true);
            handler.Activate();
            Time.timeScale = 0;
        }
    }

    private void OnDisable()
    {
        foreach (FinishDoor finishDoor in doors)
        {
            finishDoor.onDoorOpened.RemoveListener(DoorOpenedCheck);
        }
        input.Disable();
    }

    void Start()
    {
        
    }

    private void DoorOpenedCheck()
    {
        bool allOpen = true;
        foreach (FinishDoor finishDoor in doors)
        {
            if (finishDoor.Open == false)
            {
                allOpen = false;
            }
        }

        if (allOpen)
        {
            bool[] stars = {true, false, false};
            winControl.gameObject.SetActive(true);
            Time.timeScale = 0;
            if (Time.timeSinceLevelLoad < timer.LevelTime)
            {
                stars[2] = true;
            }
            
            winControl.SetStars(stars);
        }
    }

    public void TriggerGameOver()
    {
        
        loseControl.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
