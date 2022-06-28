using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using TMPro;
public class PlayerGravity : MonoBehaviour
{
    public float gravityForce = -9.81f;
    public float timeToLand = 2f;
    public float maxGravity = 4f;
    public float jumpHeight = 10f;
    public bool isGrounded = false;
    public bool isJumping = false;
    [SerializeField] LayerMask LayerToCollide;
    [Header("Player Configurations")]
    public float maxDistanceToGround = 0.2f;
    public float offsetPlayerFeet=0.8f;
    
    
    float SphereRadiusCollision;
    float currentY_Velocity = 0f;
    CharacterController _controller;
    CapsuleCollider capsuleCollider;
    Animator _animator;
    Vector3 vectorSizeFeet;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();

        SphereRadiusCollision = capsuleCollider.radius;
        // 0.1f is the padding so it doenst line perfectly with the mesh
        vectorSizeFeet = new Vector3(SphereRadiusCollision*1.25f,0.2f,SphereRadiusCollision*1.25f) ;
        
        //Just to make sure
        isJumping = false;
        isGrounded = false;
    }

    public virtual void Update()
    {
        ManageGravity(); 
    }
    private void ManageGravity()
    {
        isGrounded = CheckIfGrounded(); 

        //Case exist a animator
        if (_animator)
        {
            _animator.SetBool("isJumping",isJumping);
        }

        //Decrease the current y velocity over time
        if (currentY_Velocity < maxGravity)
            currentY_Velocity += (gravityForce * Time.deltaTime) / timeToLand;

        if (isJumping)
        {            
            //Upon reaching a certain threshold the player is considered to not be jumping no more
            //And it just in free fall mode
            if ( currentY_Velocity <= 0f)
            {
                isJumping = false;
            }
        }
        
        _controller.Move(currentY_Velocity  * Vector3.up );
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            isJumping = true;
            AddJumpForce();
        }
    }

    private bool CheckIfGrounded()
    {
        Vector3 feetPlayer = FindFeetPlayer();        
        return  Physics.Raycast(feetPlayer,Vector3.down,maxDistanceToGround,LayerToCollide) ;
    }

    private Vector3 FindFeetPlayer()
    {
        return new Vector3
                (transform.position.x,
                transform.position.y - offsetPlayerFeet,
                transform.position.z);
    }
    protected virtual void AddJumpForce()
    {       
        _controller.Move(jumpHeight * Vector3.up);
        currentY_Velocity = jumpHeight;
    }

    //Debug
    private void OnDrawGizmos()
    {
        Vector3 feetPlayer = FindFeetPlayer();
      
        Gizmos.DrawRay(feetPlayer,Vector3.down * maxDistanceToGround);
    }

}