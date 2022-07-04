using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using TMPro;
public class PlayerGravity : MonoBehaviour
{
    public float gravityForce = 9.81f;
    public float timeToLand = 10f;
    public float maxGravity = 4f;
    public float jumpHeight = 0.4f;
    public bool isGrounded = false;
    public bool isJumping = false;
    public bool isFalling = false;
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
        
        //Just negate the value
        maxGravity = -maxGravity;
        gravityForce = -gravityForce;

        //Just to make sure
        isJumping = false;
        isGrounded = false;
        isFalling = false;
    }

    public virtual void Update()
    {
        ManageGravity(); 
    }
    private void ManageGravity()
    {
        isGrounded = CheckIfGrounded(); 

        //If the player just found ground floor then he is grounded
        if (isGrounded && isFalling)
        {            
            isFalling = false;

            if(_animator)
            _animator.SetBool("isFalling",false);

        }
        //Player just fall down a ledge
        else if (!isGrounded && !isJumping)
        {
            isFalling = true;
            if(_animator)
            _animator.SetBool("isFalling",true);
        }

        if (currentY_Velocity <=0f)
        {
            //Player now has negative velocity
            currentY_Velocity = maxGravity * Time.deltaTime;
        }
        else if (currentY_Velocity > 0f)
        {
            //Start decreasing the velocity when the player has a positive y Velocity
            currentY_Velocity += (gravityForce * Time.deltaTime) / timeToLand;
        }

        if (isJumping)
        {            
            //Upon reaching a certain threshold the player is considered to not be jumping no more
            //And it just in free fall mode
            if ( currentY_Velocity <= 0f)
            {
                isJumping = false;
                isFalling = true;
                if (_animator)
                {
                _animator.SetBool("isJumping",false);
                _animator.SetBool("isFalling",true);

                }
            }
        }
        
        _controller.Move(currentY_Velocity  * Vector3.up );
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            isJumping = true;
            if (_animator)
            {
                _animator.SetBool("isJumping",true);
            }

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