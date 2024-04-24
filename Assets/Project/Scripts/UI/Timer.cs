using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;
    public int LevelTime = 30;
    private int seconds;
    private int minutes;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float timeSinceLevelLoad = Time.timeSinceLevelLoad;
        seconds = Mathf.FloorToInt(timeSinceLevelLoad % 60);
        minutes = Mathf.FloorToInt(timeSinceLevelLoad / 60);
        timer.text = $"{minutes:00}:{seconds:00}";
        if (timeSinceLevelLoad > LevelTime/2f)
        {
            timer.color = timeSinceLevelLoad > LevelTime ? Color.red : Color.Lerp(Color.white, Color.yellow, timeSinceLevelLoad/ LevelTime);
        }
       
    }
}
