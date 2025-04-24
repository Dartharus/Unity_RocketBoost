using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{   
    [SerializeField] float thrustForce;
    [SerializeField] float rotationForce;
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] AudioClip thrustSFX;
    [SerializeField] ParticleSystem mainThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    Rigidbody rb;
    AudioSource audioSource;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
        LockYRotation();
    }
    void LockYRotation()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = 0;
        transform.rotation = Quaternion.Euler(currentRotation);
    }
    void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrust();
        }
        else
        {
            audioSource.Stop();
            mainThrustParticles.Stop();
        }
    }

    private void StartThrust()
    {
        rb.AddRelativeForce(Vector3.up * thrustForce * Time.fixedDeltaTime);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(thrustSFX);
        if (!mainThrustParticles.isPlaying)
            mainThrustParticles.Play();
    }

    void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        if (rotationInput < 0)
        {
            RotateLeft();
        }
        else if (rotationInput > 0)
        {
            RotateRight();
        }
        else
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Stop();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(-rotationForce);
        if (!leftThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Play();
        }
    }

    private void RotateLeft()
    {
        ApplyRotation(rotationForce);
        if (!rightThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Play();
        }
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
}
