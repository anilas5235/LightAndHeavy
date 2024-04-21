using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private TextMeshProUGUI[] _textMeshProUGUI;

    [SerializeField] private Sprite StarEmpty;
    [SerializeField] private Sprite StarFull;

    [SerializeField] private Image[] StarImg = new Image[3];

    private string LevelName;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void SetLvl(LevelData data)
    {
        foreach (TextMeshProUGUI textMesh in _textMeshProUGUI)
        {
            textMesh.text = data.LvlName;
        }
        LevelName = data.LvlName;
        SetActive(data.locked);
        for (int i = 0; i < data.StarCount; i++)
        {
            StarImg[i].sprite = StarFull;
        }
        btn.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(LevelName);
    }
}
