using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void OnPressed() {
        Application.Quit();
            
#if UNITY_EDITOR
        // thanks Unity
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
