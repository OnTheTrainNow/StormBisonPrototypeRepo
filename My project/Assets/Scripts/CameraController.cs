using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField][Range(0, 3200)] int horizontalSensitivity; //mouse sensitivity for the horizontal axis
    [SerializeField][Range(0,3200)] int verticalSensitivity; //mouse sensitivity for the vertical axis
    [SerializeField][Range(-90, 0)] int verticalLockMin = -90; //the minimum angle the camera can look down
    [SerializeField][Range(0, 90)] int verticalLockMax = 90; //the maximum angle the camera can look up
    [SerializeField] bool invertY; //this bool inverts the camera movement when moving the mouse up or down

    float xRotation; //the rotate for the camera along the X-Axis (looking up/down)

    void Start()
    {
        Cursor.visible = false; //makes the cursor invisible in game
        Cursor.lockState = CursorLockMode.Locked; //locks the cursor to the application screen so it doesnt leave if the player moves their mouse far
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * horizontalSensitivity; //get the direction and float value for mouse movement from get axis
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * verticalSensitivity; //multiply it by the corresponding sensitivity value 

        if (invertY) //if the inverted controls bool is on
        {
            xRotation += mouseY; //add the mouseY value to the xRotation (mouse movement along the Y axis controls the camera rotation along the X axis)
        }
        else
        {
            xRotation -= mouseY; //other wise the mouseY value is substracted for default controls
        }

        xRotation = Mathf.Clamp(xRotation, verticalLockMin, verticalLockMax); //clamp the xRotation of the camera so its between the two lock values

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0); //change the local rotation of the camera object. (use a Quaternion value to rotate it along only the X axis)

        transform.parent.Rotate(Vector3.up * mouseX); //rotate the parent object on the Y axis based on the mouse X value. Vector3.up is shorthand for Y axis (0,1,0)
    }
}
