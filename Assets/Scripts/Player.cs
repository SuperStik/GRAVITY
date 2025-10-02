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
    private InputAction move;
    private InputAction jump;
    
    private Rigidbody phys;
    private Collider collide;

    private Numerics.Vector2 pitchyaw;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        look = InputSystem.actions.FindAction("Look");
        pause = InputSystem.actions.FindAction("Pause");
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        
        phys = GetComponent<Rigidbody>();
        collide = GetComponent<Collider>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = look.ReadValue<Vector2>();
        pitchyaw += new Numerics.Vector2(-delta.y, delta.x);
        // fancy C# switch statement
        pitchyaw.X = pitchyaw.X switch {
            > 90.0f => 90.0f,
            < -90.0f => -90.0f,
            _ => pitchyaw.X
        };

        cam.transform.eulerAngles = new Vector3(pitchyaw.X, pitchyaw.Y);

        // trig to convert local control axis to world
        float yawradians = pitchyaw.Y * MathF.PI / 180;
        
        var yawsin = MathF.Sin(yawradians);
        var yawcos = MathF.Cos(yawradians);
        
        Vector2 localmovevec = move.ReadValue<Vector2>();
        Numerics.Vector2 movevec = new Numerics.Vector2(
            localmovevec.x * yawcos + localmovevec.y * yawsin,
            localmovevec.y * yawcos - localmovevec.x * yawsin);
        movevec *= 0.25f;
        
        float verticalspeed = 0.0f;
        if (jump.WasPressedThisFrame() && IsGrounded()) {
            verticalspeed = 4.0f;
        }
        
        phys.linearVelocity += (new Vector3(movevec.X, verticalspeed, movevec.Y));
        
        // get outta here when we escape
        if (pause.WasPressedThisFrame()) {
            Application.Quit();
            
#if UNITY_EDITOR
            // thanks Unity
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
    
    private bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, collide.bounds.extents.y + 0.1f);
    }
}
