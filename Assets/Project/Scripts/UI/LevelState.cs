using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.LevelObjects;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    [SerializeField] private FinishDoor[] doors = new FinishDoor[2];
    [SerializeField] private UIPanelControll winControll;
    void Start()
    {
        foreach (FinishDoor finishDoor in doors)
        {
            finishDoor.onDoorOpened.AddListener(DoorOpenedCheck);
        }
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
            winControll.gameObject.SetActive(true);
            Time.timeScale = 0;
            winControll.Stars = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
