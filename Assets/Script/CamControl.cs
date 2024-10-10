using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public GameObject target;
    private Vector3 offset;
    public float smooth;
    private Vector3 velocity = Vector3.zero;
    private Quaternion currentRotation;
    public float rotateSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        GameObject target = GameObject.Find("Player");
        offset = target.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        // update position
        // get mouse rotation
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        //convert quaternion rotation into Euler angles
        Vector3 currentRotationEuler = currentRotation.eulerAngles;
        //add to the rotation the horizontal movement
        currentRotationEuler.y += horizontal;
        /*
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed/2;          
        currentRotationEuler.x += horizontal;
        */
        //convert back to Quaternion
        currentRotation = Quaternion.Euler(currentRotationEuler);
        //update position
        Vector3 targetPosition = target.transform.position - (currentRotation * offset);
        this.transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smooth);

        // update rotation
        this.transform.LookAt(target.transform);


    }
}
