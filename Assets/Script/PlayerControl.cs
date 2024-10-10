using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float speed = 8;                 //variables de stats
    public float jumpForce = 10;
    public float gravityForce = 3;
    private float Base_speed = 0;           //les variables Base_ servent a conserver les valeurs de base pour les resets
    private float Base_jumpForce = 0;
    private float Base_gravityForce = 0;
    public bool Charge_On = true;           //Charge_On permet d'activer/desactiver le fait d'avoir un nombre de changement de stats limité ou non
    public float Charge_restantes = 6;      //Nombre de charge de base (a changer dans code pour le nb max)
    private float ySpeed;
    private float sec_value = 0;            //permet de compter les sec pour ajouter les charges
    private CharacterController characterController;
    public float rotationSpeed = 720;
    public float jumpButtonTolerancePeriod = 0.05f;         //temps pour le coyote time
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    public GameObject cam;
    public Vector3 tp;              //position de base pour s'y tp
    private Vector3 cp_tp;          //position de checkpoint pour s'y tp

    void Start()
    {
        Base_speed = speed;             //assignation des valeurs
        Base_jumpForce = jumpForce;
        Base_gravityForce = gravityForce;
        characterController = GetComponent<CharacterController>();
        tp = transform.position;        //assignation des CO pour se tp
        cp_tp = tp;
    }
    private void OnTriggerEnter(Collider other)         //permet de ne pas rester "collé" a un plafond en attendant que la yspeed se reduise (=rebondir sur un plafond)
    {
        if (other.tag == "Plafond")
        {
            if (ySpeed < 0)
            {
                ySpeed = -ySpeed;
            }
        }
    }

    void Update()
    {
        Physics.gravity = new Vector3(0, -(gravityForce * 7), 0);           ///pour la gestion de la gravité (le *7 pour que le changment ai un impact significatif)
        GameObject.Find("SpeedValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<PlayerControl>().speed.ToString();          //changer les stats dans l'UI
        GameObject.Find("JumpForceValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<PlayerControl>().jumpForce.ToString();
        GameObject.Find("GravityForceValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<PlayerControl>().gravityForce.ToString();
        if (Charge_On == true)      //uniqument si on le fait avec les charges
        {
            GameObject.Find("ChargeRestantesValue").GetComponent<TextMeshProUGUI>().text = GameObject.Find("Player").GetComponent<PlayerControl>().Charge_restantes.ToString();
        }
        sec_value += Time.deltaTime; //compter les secondes
        if (sec_value >= 3)         //augmenter le nombre de charges toutes les X secondes jusqu'a un max predefini
        {
            sec_value -= 3;
            if (Charge_restantes < 6)
            {
                Charge_restantes += 1;
            }

        }


        float horizontal; //Pour les deplacements
        float vertical;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontal, 0, vertical);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude);
        movementDirection.Normalize();

        Vector3 finalDirection = movementDirection * speed * 3 * magnitude; //le *3 pour que le changment soit significatif
        float cameraFacing = cam.transform.eulerAngles.y;
        finalDirection.y = ySpeed;
        finalDirection = Quaternion.Euler(0, cameraFacing, 0) * finalDirection;
        characterController.Move(finalDirection * Time.deltaTime);

        if (characterController.isGrounded == true)
        {
            ySpeed = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                ySpeed = jumpForce * 3;     //le *3 pour que le changment soit significatif
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
        if (characterController.isGrounded == true) // set le checkpoint si au sol
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
                ySpeed = jumpForce * 3;     //le *3 pour que le changment soit significatif et soit coherent au reste
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }



        if (Input.GetKeyUp("r"))        //reset les stats
        {
            speed = Base_speed;
            jumpForce = Base_jumpForce;
            gravityForce = Base_gravityForce;
            Console.WriteLine("Stats reset");

        }
        // dans la suite des input, conditions si on fait avec ou sans les charges
        if (Input.GetKeyUp("i"))        //augmentation speed
        {
            if (Charge_On == true)
            {
                if (Charge_restantes >= 1)
                {
                    if (jumpForce >= 1)
                    {
                        if (gravityForce >= 1)      //si on peut changer les stats sans jamais en avoir une a 0, alors change les stats (et en fonction si on a au moins 1 charge)
                        {
                            speed += 1;
                            jumpForce -= 0.5f;
                            gravityForce -= 0.5f;
                            Charge_restantes -= 1;
                        }
                    }
                }
            }
            else if (Charge_On == false)
            {
                if (gravityForce >= 1)
                {
                    if (jumpForce >= 1)
                    {
                        speed += 1;
                        jumpForce -= 0.5f;
                        gravityForce -= 0.5f;
                    }

                }
            }

        }
        if (Input.GetKeyUp("o"))        //reduction speed
        {
            if (Charge_On == true)
            {
                if (Charge_restantes >= 1)
                {

                    if (speed >= 1.5f)          //si on peut changer les stats sans jamais en avoir une a 0, alors change les stats (et en fonction si on a au moins 1 charge)
                    {
                        speed -= 1;
                        jumpForce += 0.5f;
                        gravityForce += 0.5f;
                        Charge_restantes -= 1;
                    }

                }
            }
            else if (Charge_On == false)
            {
                if (speed >= 1.5f)
                {
                    speed -= 1;
                    jumpForce += 0.5f;
                    gravityForce += 0.5f;

                }
            }
        }
        if (Input.GetKeyUp("k"))            //augmentation jump force
        {
            if (Charge_On == true)
            {
                if (Charge_restantes >= 1)
                {
                    if (gravityForce >= 1.5f)           //si on peut changer les stats sans jamais en avoir une a 0, alors change les stats (et en fonction si on a au moins 1 charge)
                    {
                        jumpForce += 1;
                        gravityForce -= 1;
                        Charge_restantes -= 1;
                    }
                }
            }

            else if (Charge_On == false)
            {
                if (gravityForce >= 1.5f)

                {
                    jumpForce += 1;
                    gravityForce -= 1;
                }

            }
        }
        if (Input.GetKeyUp("l"))        //reduction jump force
        {
            if (Charge_On == true)
            {
                if (Charge_restantes >= 1)
                {
                    if (jumpForce >= 1.5f)      //si on peut changer les stats sans jamais en avoir une a 0, alors change les stats (et en fonction si on a au moins 1 charge)
                    {
                        jumpForce -= 1;
                        gravityForce += 1;
                        Charge_restantes -= 1;

                    }
                }
            }

            else if (Charge_On == false)
            {
                if (jumpForce >= 1.5f)
                {
                    jumpForce -= 1;
                    gravityForce += 1;
                }
            }
        }
        if (Input.GetKeyUp("b"))            //augmentation gravity
        {
            if (Charge_On == true)
            {
                if (Charge_restantes >= 1)
                {
                    if (speed >= 1.5f)          //si on peut changer les stats sans jamais en avoir une a 0, alors change les stats (et en fonction si on a au moins 1 charge)
                    {
                        speed -= 1;
                        gravityForce += 1;
                    }
                }
            }
            else if (Charge_On == false)
            {
                if (speed >= 1.5f)
                {
                    speed -= 1;
                    gravityForce += 1;
                }
            }

        }
        if (Input.GetKeyUp("n"))        //reduction gravity
        {
            if (Charge_On == true)
            {
                if (Charge_restantes >= 1)
                    if (gravityForce >= 1.5f)           //si on peut changer les stats sans jamais en avoir une a 0, alors change les stats (et en fonction si on a au moins 1 charge)
                    {
                        gravityForce -= 1;
                        speed += 1;

                    }
            }
            else if (Charge_On == false)
            {
                if (gravityForce >= 1.5f)
                {
                    gravityForce -= 1;
                    speed += 1;

                }
            }
        }


        if (Input.GetKeyUp("p"))        //tp position de base 
        {

            GetComponent<CharacterController>().enabled = false;
            transform.position = tp;
            GetComponent<CharacterController>().enabled = true;
            if (ySpeed > 0)      //verifer que la speed verticale est positive pour l'inverser (et donc toucher le sol pour stopper le mouvement vertical)
            {
                ySpeed = -ySpeed;
            }

        }
        if (Input.GetKey("v"))      //tp position du check point
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = cp_tp;
            GetComponent<CharacterController>().enabled = true;
            if (ySpeed > 0)     //verifer que la speed verticale est positive pour l'inverser (et donc toucher le sol pour stopper le mouvement vertical)
            {
                ySpeed = -ySpeed;
            }
        }



    }
}




