using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadScenButton : MonoBehaviour
{
    public string sceneName = "";

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject
            )
        {
            LoadTargetScene();
        }
    }

    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
