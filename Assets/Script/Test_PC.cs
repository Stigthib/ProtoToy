using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Test_PC : MonoBehaviour
{
    public float speed = 8;
    public float jumpForce = 10;
    public float gravityForce = 3;
    private float Base_speed = 0;
    private float Base_jumpForce = 0;
    private float Base_gravityForce = 0;

    private float ySpeed;
    private CharacterController characterController;
    public float rotationSpeed = 720;
    public float jumpButtonTolerancePeriod = 0.05f;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    public GameObject cam;
    public Vector3 tp;
    private Vector3 cp_tp;
    // Start is called before the first frame update
    void Start()
    {
        Base_speed = speed;
        Base_jumpForce = jumpForce;
        Base_gravityForce = gravityForce;
        characterController = GetComponent<CharacterController>();
        tp = transform.position;
        cp_tp = tp;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.gravity = new Vector3(0, -(gravityForce * 7), 0);
        GameObject.Find("SpeedValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<Test_PC>().speed.ToString();
        GameObject.Find("JumpForceValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<Test_PC>().jumpForce.ToString();
        GameObject.Find("GravityForceValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<Test_PC>().gravityForce.ToString();
       
        float horizontal;
        float vertical;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontal, 0, vertical);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude);
        movementDirection.Normalize();

        //ySpeed += Physics.gravity.y * Time.deltaTime;
        Vector3 finalDirection = movementDirection * speed * 3 * magnitude;
        float cameraFacing = cam.transform.eulerAngles.y;
        finalDirection.y = ySpeed;
        finalDirection = Quaternion.Euler(0, cameraFacing, 0) * finalDirection;
        characterController.Move(finalDirection * Time.deltaTime);

        if (characterController.isGrounded == true)
        {
            ySpeed = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                ySpeed = jumpForce * 3;
            }
        }
        else
        {
            ySpeed = ySpeed + Physics.gravity.y * Time.deltaTime;
        }

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        if (characterController.isGrounded == true)
        {
            if (Input.GetKeyUp("c"))
            {
                cp_tp = transform.position;
            }
        }


        if (Time.time - lastGroundedTime <= jumpButtonTolerancePeriod)
        {
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonTolerancePeriod)
            {
                ySpeed = jumpForce * 3;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }



        if (Input.GetKeyUp("r"))
        {
            speed = Base_speed;
            jumpForce = Base_jumpForce;
            gravityForce = Base_gravityForce;
            Console.WriteLine("Stats reset");

        }
        if (Input.GetKeyUp("i"))
        {

            if (jumpForce >= 1) 
            {
                if (gravityForce >=1)
                {
                    speed += 1;
                    jumpForce -= 0.5f;
                    gravityForce -= 0.5f;
                }
            }

            
        }
        if (Input.GetKeyUp("o"))
        {
            if (speed >= 1.5f)
            {
                speed -= 1;
                jumpForce += 0.5f;
                gravityForce += 0.5f;  

            }
        }
        if (Input.GetKeyUp("k"))
        {
            if (gravityForce >= 1.5f)
            {
                jumpForce += 1;
                gravityForce -= 1;
            }
        }
        if (Input.GetKeyUp("l"))
        {
            if (jumpForce >= 1.5f)
            {
                jumpForce -= 1;
                gravityForce += 1;

            }

        }
        if (Input.GetKeyUp("b"))
        {
            if (speed >= 1.5f)
            {
                speed -= 1;
                gravityForce += 1;
            }

                


        }
        if (Input.GetKeyUp("n"))
        {
            if (gravityForce >= 1.5f)
            {
                gravityForce -= 1;
                speed += 1;

            }

        }
        if (Input.GetKeyUp("p"))
        {

            GetComponent<CharacterController>().enabled = false;
            transform.position = tp;
            GetComponent<CharacterController>().enabled = true;
            ySpeed = -ySpeed;
        }
        if (Input.GetKey("v"))
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = cp_tp;
            GetComponent<CharacterController>().enabled = true;
            ySpeed = -ySpeed;
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Plafond")
        {
            ySpeed = -ySpeed;

        }
    }
}

