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
        if (bgm != null && audioSource != null)
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

        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        bool isMoving = move.magnitude > 0.15f;

        if (isMoving)
        {
            rb.linearVelocity = new Vector3(
                move.x * speed,
                rb.linearVelocity.y,
                move.z * speed
            );

            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.angularVelocity = Vector3.zero;
        }

        anim.SetBool("isRunning", isMoving);

        // --- SPATIAL AUDIO FOOTSTEPS ---
        if (isMoving && isGrounded)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepDelay)
            {
                if (footstepSound != null)
                {
                    // Spawns the footstep sound at the player's exact 3D feet coordinates
                    AudioSource.PlayClipAtPoint(footstepSound, transform.position);
                }
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

            // FIX: Play the jump sound at the player's exact 3D position instead of globally
            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position);
            }

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