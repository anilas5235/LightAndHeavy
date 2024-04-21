using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSelection : MonoBehaviour
{
   [SerializeField] private GameObject LevelSelect;
   [SerializeField] private bool CreateOrOverrideJson;
   private string path = "LevelData.txt";
   
   string fileName = Application.streamingAssetsPath + "/XML/ObjectData.xml";
    void Start()
    {
        LoadJson();
    }

    private void OnValidate()
    {
        if (CreateOrOverrideJson)
        {
            CreateOrOverrideJson = false;
            CreateJson();
        }
    }


    //Creates the Json with all levels with 0 Stars
    private AllLevelsObject CreateJson()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> levels = new List<string>();
        
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName.StartsWith("LVL_"))
            {
                levels.Add(sceneName);
            }
        }
        
        AllLevelsObject data = new AllLevelsObject();
        for (var i = 0; i < levels.Count; i++)
        {
            string level = levels[i];
            if (i == 0)
            {
                data.levelDatas.Add(new LevelData() { LvlName = level, StarCount = 0, locked = false });
            }
            else
            {
                data.levelDatas.Add(new LevelData() { LvlName = level, StarCount = 0, locked = true });
            }
            
        }

        SaveSystem.Save<AllLevelsObject>(path, data);
        return data;
    }

    private void LoadJson()
    {
       AllLevelsObject levelsObject = SaveSystem.Load<AllLevelsObject>(path) ?? CreateJson();
       foreach (LevelData levelObject in levelsObject.levelDatas)
       {
          GameObject level = Instantiate(LevelSelect, transform);
          LevelSelect levelSelect =  level.GetComponent<LevelSelect>();
         levelSelect.SetLvl(levelObject);
       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
