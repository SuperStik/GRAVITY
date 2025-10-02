using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private CanvasGroup mainMenuGroup;

    public void StartButtonOnClick() {
        mainMenuGroup.interactable = false;
        StartCoroutine(FadingRoutine());
    }

    private void Start() {
        mainMenuGroup = GetComponent<CanvasGroup>();
    }

    private IEnumerator FadingRoutine() {
        for (var i = 0; i < 100; ++i) {
            mainMenuGroup.alpha -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
