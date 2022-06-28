using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerCameraMovement : MonoBehaviour
{
    public float radius = 0.05f;
    public float offsetY = 0.5f;
    public float sensitivy = 1f;
    public bool invertX = true;
    public bool canMove = true;

    [SerializeField]Transform playerMainCamera;
    private float invert = -1f;

    // Start is called before the first frame update
    void Awake()
    {
        if (playerMainCamera == null)
            playerMainCamera = Camera.main.transform;
    }
    void Start()
    {
        playerMainCamera.SetParent(null);
        
        //Ensure that if mouse goes from window start to finish then 1 lap will be made
        sensitivy *= (float) ((2f * Math.PI)/ (float) Screen.width);
        
        invert = (invertX) ? -1f : 1f;
    }

    void Update()
    {
        //This public variable could be changed
        if (!canMove)
            return;

        float mouseX = Input.mousePosition.x * sensitivy;
        var mouseRotateOffset = GetCirclePosition(mouseX);
        playerMainCamera.position = transform.position +  mouseRotateOffset;
        playerMainCamera.LookAt(transform,Vector3.up);
    }
    //SIN(between 0 and 2PI)
    //
    private Vector3 GetCirclePosition(float mouseX)
    {
        float y =(float) Math.Sin(mouseX * invert) * radius;
        float x =(float) Math.Cos(mouseX * invert) * radius;
        return new Vector3(x,offsetY,y);
    }

    public async Task MoveCamera(Vector3 v,Vector3 lookAt,float speed = 1)
    {
        var start = playerMainCamera.position;
        int frames = 100;
        float increase = (1f/(float)frames) * speed;
        for (float i = 0f; i < 1; i+= increase)
        {
            playerMainCamera.position = Vector3.Lerp(start,v,i);
            playerMainCamera.LookAt(lookAt,Vector3.up);
            await Task.Delay(20);
        }
    }

     public void MoveCameraInstant(Vector3 v,Vector3 lookAt)
    {
        playerMainCamera.position = v;
        playerMainCamera.LookAt(lookAt,Vector3.up);
    }

}
