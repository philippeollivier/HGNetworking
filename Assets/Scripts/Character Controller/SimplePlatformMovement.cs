using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlatformMovement : MonoBehaviour
{
    public bool rotating = false, moving = false, verticalMoving = false;
    public float rotSpeed = 1f, movSpeed = 1f;
    private Rigidbody rb;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (rotating)
        {
            rb.MoveRotation(new Quaternion());
        }
        if (moving)
        {
            rb.AddForce(transform.position + transform.forward * Mathf.Sin(Time.time) * movSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        if (verticalMoving)
        {
            rb.AddForce(transform.position + transform.up * Mathf.Sin(Time.time) * movSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
