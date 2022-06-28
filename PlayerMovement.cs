using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof( CharacterController),typeof(PlayerGravity))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Speed")]
    public float GroundSpeed = 30f;
    public float turnSmoothTime = 10f;    
    public float drag = 2f;
    public float runningAnimationSpeed = 1f;
    public Transform PlayerMainCamera;
    CharacterController _controller;
    Animator _animator;
    public bool canMove = true;

    private void Awake()
    {
        if (PlayerMainCamera == null)
            PlayerMainCamera = Camera.main.transform;
    
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canMove) return;
        
        MovePlayer();
        
        SpeedPlayer();
    }
    Vector3 moveDir;
    private void MovePlayer()
    {
        //Reading from the keyboard where to go
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        
        bool noInput = direction.magnitude < 0.1f;

        if (_animator)
        {
            _animator.SetBool("isRunning",!noInput);
        }
        
        if (noInput)
        {

            var negativeVelocity = new Vector3(-_controller.velocity.x * drag,0f,-_controller.velocity.z * drag);
            _controller.Move(negativeVelocity);
            return;
        }
        
        float targetAngle = FindNewRotationAngle(direction);
        
        //TODO
        //Find the normal of the ground below and add the force in the corresponding vector
        moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized *  Time.deltaTime * GroundSpeed;
        
        // float dot_Direction = Vector3.Dot(_rigidbody.velocity,moveDir);
        // if (dot_Direction <= 0.5)
        //     moveDir*= - Mathf.Abs(dot_Direction) * reverseAmplifier;

        _controller.Move(moveDir);
    }
    private float FindNewRotationAngle(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + PlayerMainCamera.eulerAngles.y;
        
        transform.rotation = Quaternion.Lerp(
        transform.rotation, 
        Quaternion.Euler(0f,targetAngle,0f),
        Time.deltaTime*turnSmoothTime);

        return targetAngle;
    }
    private void SpeedPlayer()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GroundSpeed *= 3;
            if (_animator)
                _animator.SetFloat("RunningMultiplier",runningAnimationSpeed * 3);

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            GroundSpeed /= 3;
            
            if(_animator)
                _animator.SetFloat("RunningMultiplier",runningAnimationSpeed);
        }            

    }
    
     private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position,moveDir*10);
    }

    public void StopMoving()
    {
        canMove = false;
    }
}