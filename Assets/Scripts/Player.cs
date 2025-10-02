using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public Camera cam;
    private InputAction look;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        look = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = look.ReadValue<Vector2>();
        cam.transform.eulerAngles += new Vector3(-delta.y, delta.x);
    }
}
