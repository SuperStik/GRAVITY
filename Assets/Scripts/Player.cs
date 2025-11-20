using System;
using NUnit.Framework.Constraints;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
using Numerics = System.Numerics;

public class Player : MonoBehaviour {
    public Camera cam;
    public float maxHealth = 100.0f;
    
    private Rigidbody phys;
    private Collider collide;

    private Numerics.Vector2 lookDelta;
    private Vector2 moveDirection;
    
    private Numerics.Vector2 pitchyaw;
    
    private bool didJump = false;

    private float health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        health = maxHealth;
        
        phys = GetComponent<Rigidbody>();
        collide = GetComponent<Collider>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update() {
        pitchyaw += lookDelta;
        // fancy C# switch statement
        pitchyaw.X = pitchyaw.X switch {
            > 90.0f => 90.0f,
            < -90.0f => -90.0f,
            _ => pitchyaw.X
        };

        Vector3 euler = new(pitchyaw.X, pitchyaw.Y);
        cam.transform.eulerAngles = euler;
    }

    private void FixedUpdate() {
        // trig to convert local control axis to world
        var yawradians = pitchyaw.Y * MathF.PI / 180;
        
        var yawsin = MathF.Sin(yawradians);
        var yawcos = MathF.Cos(yawradians);
        
        Numerics.Vector2 movevec = new (
            moveDirection.x * yawcos + moveDirection.y * yawsin,
            moveDirection.y * yawcos - moveDirection.x * yawsin);
        movevec *= 0.25f;
        
        var verticalSpeed = 0.0f;
        if (didJump && IsGrounded()) {
            verticalSpeed = 4.0f;
            didJump = false;
        }
        
        phys.linearVelocity += (new Vector3(movevec.X, verticalSpeed, movevec.Y));
    }

    private void OnLook(InputValue val) {
        var lookInput = val.Get<Vector2>();
        lookDelta = new Numerics.Vector2(-lookInput.y, lookInput.x);
    }

    private void OnMove(InputValue val) {
        moveDirection = val.Get<Vector2>();
    }

    private void OnJump() {
        didJump = true;
    }

    private void OnAttack() {
        var rot = Quaternion.Euler(cam.transform.eulerAngles);
        Physics.Raycast(cam.transform.position, rot * Vector3.forward, out var hit);
        Debug.DrawLine(cam.transform.position, hit.point, Color.red, 5.0f, false);
        print(cam.transform.position.ToString() + ';' + hit.point);
    }

    // get outta here when we pause
    private void OnPause() {
        Application.Quit();
            
#if UNITY_EDITOR
        // thanks Unity
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    private bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, collide.bounds.extents.y + 0.1f);
    }
}
