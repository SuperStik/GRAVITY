using System;
using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public GameObject gameMenu;
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
        
        gameObject.SetActive(false);
        gameMenu.SetActive(true);
    }
}
