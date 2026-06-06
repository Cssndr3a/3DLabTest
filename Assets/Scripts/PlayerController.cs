using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private Animator anim;

    private bool isGrounded;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip footstepSound;
    public AudioClip bgm;

    [Header("Particle")]
    public ParticleSystem jumpEffect;

    private float footstepTimer = 0f;
    public float footstepDelay = 0.4f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // Play BGM (3D optional OFF for cleaner)
        if (bgm != null)
        {
            audioSource.clip = bgm;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f; // 2D BGM
            audioSource.Play();
        }
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Use GetAxisRaw for snappier input detection (instantly drops to 0 when keys are released)
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // Raise the threshold from 0.1f to 0.15f to filter out tiny physics jitter
        bool isMoving = move.magnitude > 0.15f;

        if (isMoving)
        {
            // 1. Apply movement velocity only when actually pressing keys
            rb.linearVelocity = new Vector3(
                move.x * speed,
                rb.linearVelocity.y,
                move.z * speed
            );

            // 2. Smoothly rotate towards movement direction
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
        else
        {
            // 3. KILL RESIDUAL SPIN & HORIZONTAL VELOCITY WHEN IDLE
            // This stops the physics engine from sliding or spinning your player on collisions
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.angularVelocity = Vector3.zero;
        }

        anim.SetBool("isRunning", isMoving);

        // FOOTSTEP SOUND (anti spam)
        if (isMoving && isGrounded)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepDelay)
            {
                audioSource.PlayOneShot(footstepSound);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");

            audioSource.PlayOneShot(jumpSound);

            if (jumpEffect != null)
            {
                jumpEffect.Play();
            }

            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}