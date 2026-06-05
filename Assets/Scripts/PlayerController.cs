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

        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;

        rb.linearVelocity = new Vector3(
            move.x * speed,
            rb.linearVelocity.y,
            move.z * speed
        );

        bool isMoving = move.magnitude > 0.1f;

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