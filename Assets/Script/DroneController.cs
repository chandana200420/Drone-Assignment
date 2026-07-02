using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Flight Settings")]
    public float throttleForce = 20f;
    public float pitchTorque = 8f;
    public float rollTorque = 8f;
    public float yawTorque = 5f;

    [Range(0.3f, 0.9f)] public float hoverDamping = 0.6f;

    
    [Range(0.85f, 0.99f)] public float angularSelfLevel = 0.95f;

    [Header("Input Keys")]
    public KeyCode throttleUpKey = KeyCode.Space;
    public KeyCode throttleDownKey = KeyCode.LeftControl;
    public KeyCode yawLeftKey = KeyCode.Q;
    public KeyCode yawRightKey = KeyCode.E;


    public bool isArmed = false;

    private Rigidbody rb;


    public float CurrentSpeed => rb != null ? rb.linearVelocity.magnitude : 0f;
    public float Altitude => transform.position.y;
    public bool IsArmed => isArmed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.useGravity = true;
        rb.linearDamping = 1f;
        rb.angularDamping = 2f;
    }

    void FixedUpdate()
    {
        if (!isArmed)
        {
            return;
        }

        float throttle = 0f;
        if (Input.GetKey(throttleUpKey)) throttle = 1f;
        else if (Input.GetKey(throttleDownKey)) throttle = -1f;

        float pitch = Input.GetAxis("Vertical");     
        float roll = Input.GetAxis("Horizontal");    

        float yaw = 0f;
        if (Input.GetKey(yawLeftKey)) yaw -= 1f;
        if (Input.GetKey(yawRightKey)) yaw += 1f;

        
        rb.AddForce(transform.up * throttle * throttleForce, ForceMode.Force);

        
        if (Mathf.Approximately(throttle, 0f))
        {
            Vector3 gravityCompensation = -Physics.gravity * rb.mass;
            rb.AddForce(gravityCompensation, ForceMode.Force);

            Vector3 vel = rb.linearVelocity;
            vel.y *= (1f - hoverDamping * Time.fixedDeltaTime * 10f);
            rb.linearVelocity = vel;
        }

        
        Vector3 torque = new Vector3(pitch * pitchTorque, yaw * yawTorque, -roll * rollTorque);
        rb.AddRelativeTorque(torque, ForceMode.Force);
        // Self-leveling
        Vector3 currentRotation = transform.eulerAngles;

        float pitchAngle = currentRotation.x;
        if (pitchAngle > 180f) pitchAngle -= 360f;

        float rollAngle = currentRotation.z;
        if (rollAngle > 180f) rollAngle -= 360f;

        Vector3 levelTorque = new Vector3(
            -pitchAngle,
            0f,
            -rollAngle
        ) * (1f - angularSelfLevel);

        rb.AddRelativeTorque(levelTorque, ForceMode.Force);



    }

    public void Arm()
    {
        isArmed = true;
    }

    public void Disarm()
    {
        isArmed = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ToggleArm()
    {
        if (isArmed) Disarm();
        else Arm();
    }
}