using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerController : MonoBehaviour
{
    //Camera rotation clamps
    public float minX = -70f;
    public float maxX = 70f;

    //Camera look sensitivity
    public float sensitivity;

    //local player rotation floats
    float rotX;
    float rotY;

    //Camera reference
    public Camera playerCam;

    //Player Movement reference
    PlayerMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        //Locks and hides cursor in standalone builds
        LockCursor();

        movement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuManager.gameIsPaused)
        {
            //Handles camera rotation based on mouse movement
            rotY += Input.GetAxis("Mouse X") * sensitivity;
            rotX += Input.GetAxis("Mouse Y") * sensitivity;

            //Handles clamping look rotation
            rotX = Mathf.Clamp(rotX, minX, maxX);

            //Actual rotation of player and camera
            transform.localEulerAngles = new Vector3(0, rotY, 0);
            playerCam.transform.localEulerAngles = new Vector3(-rotX, 0, movement.tilt); //We're assigning tilt here based on the
        }                                                                            //player movement script
    }
    //Lock Cursor function
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
