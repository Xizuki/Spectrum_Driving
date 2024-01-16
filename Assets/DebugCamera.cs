using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public float speed;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -transform.right * speed * Time.deltaTime;

        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * speed * Time.deltaTime;

        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= 1.5f;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            speed /= 1.5f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate the camera based on mouse input.
            transform.Rotate(Vector3.up * mouseX);
            transform.Rotate(Vector3.left * mouseY);

            // Optional: Limit the vertical rotation to a certain range to prevent flipping.
            float currentRotationX = transform.rotation.eulerAngles.x;
            if (currentRotationX > 180f)
                currentRotationX -= 360f;

            currentRotationX = Mathf.Clamp(currentRotationX, -90f, 90f);

            transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y, 0f);
        }
    }
}
