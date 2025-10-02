using System;
using NUnit.Framework.Constraints;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
using Numerics = System.Numerics;

public class Player : MonoBehaviour {
    public Camera cam;
    private InputAction look;
	private InputAction pause;

    private Numerics.Vector2 pitchyaw;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        look = InputSystem.actions.FindAction("Look");
        pause = InputSystem.actions.FindAction("Pause");
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = look.ReadValue<Vector2>();
        pitchyaw += new Numerics.Vector2(-delta.y, delta.x);
        // fancy C# switch statement
        pitchyaw.X = pitchyaw.X switch {
            >= 90.0f => 90.0f,
            <= -90.0f => -90.0f,
            _ => pitchyaw.X
        };

        cam.transform.eulerAngles = new Vector3(pitchyaw.X, pitchyaw.Y);
        
        if (pause.WasPressedThisFrame()) {
            Application.Quit();
            
#if UNITY_EDITOR
            // thanks Unity
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
