using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.LevelObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelState : MonoBehaviour
{
    [SerializeField] private FinishDoor[] doors = new FinishDoor[2];
    [SerializeField] private UIPanelControll winControl;
    [SerializeField] private UIPanelControll pauseControl;
    [SerializeField] private UIPanelControll loseControl;

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
            Time.timeScale = 1;
        }
        else
        {
            Pause = true;
            pauseControl.gameObject.SetActive(true);
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
            winControl.gameObject.SetActive(true);
            Time.timeScale = 0;
            winControl.Stars = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
