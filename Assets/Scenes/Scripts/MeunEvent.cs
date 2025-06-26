using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeunEvent : MonoBehaviour
{
    [SerializeField] private GameObject settingsUI;

    [SerializeField] private GameObject developerUI;
    public void SettingsDown()
    {
        settingsUI.SetActive(true);
    }
    public void DeveloperDwon()
    {
        developerUI.SetActive(true);
    }
    public void ExitDown()
    {
        Application.Quit();
    }
    public void PlayDown()
    {
        StartCoroutine(LoadMainScene());
    }

    public IEnumerator LoadMainScene()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("MainScene");

        while(ao.progress < 0.9)
        {
            yield return null;
        }
    }
}
