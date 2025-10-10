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
    private InputAction attack;
    
    private Rigidbody phys;
    private Collider collide;

    private Numerics.Vector2 pitchyaw;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        look = InputSystem.actions.FindAction("Look");
        pause = InputSystem.actions.FindAction("Pause");
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        attack = InputSystem.actions.FindAction("Attack");
        
        phys = GetComponent<Rigidbody>();
        collide = GetComponent<Collider>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        var delta = look.ReadValue<Vector2>();
        pitchyaw += new Numerics.Vector2(-delta.y, delta.x);
        // fancy C# switch statement
        pitchyaw.X = pitchyaw.X switch {
            > 90.0f => 90.0f,
            < -90.0f => -90.0f,
            _ => pitchyaw.X
        };

        Vector3 euler = new(pitchyaw.X, pitchyaw.Y);
        cam.transform.eulerAngles = euler;

        // trig to convert local control axis to world
        var yawradians = pitchyaw.Y * MathF.PI / 180;
        
        var yawsin = MathF.Sin(yawradians);
        var yawcos = MathF.Cos(yawradians);
        
        var localmovevec = move.ReadValue<Vector2>();
        Numerics.Vector2 movevec = new (
            localmovevec.x * yawcos + localmovevec.y * yawsin,
            localmovevec.y * yawcos - localmovevec.x * yawsin);
        movevec *= 0.25f;
        
        var verticalspeed = 0.0f;
        if (jump.WasPressedThisFrame() && IsGrounded()) {
            verticalspeed = 4.0f;
        }

        if (attack.WasPressedThisFrame()) {
            var rot = Quaternion.Euler(euler);
            Physics.Raycast(cam.transform.position, rot * Vector3.forward, out var hit);
            Debug.DrawLine(cam.transform.position, hit.point, Color.red, 5.0f, false);
            print(cam.transform.position.ToString() + ';' + hit.point);
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
