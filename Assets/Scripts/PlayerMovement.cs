using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Estadísticas de Movimiento")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("Estadísticas de Salto")]
    [SerializeField] private float jumpForce = 15f;
    [Range(0f, 1f)]
    [Tooltip("Multiplicador aplicado a la velocidad vertical al soltar el botón de salto.")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;


    [Header("Estadísticas de Escalada")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private LayerMask climbableLayer;


    [Header("Game Feel (Permisividad)")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Detección de Entorno")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;


    [Header("Referencias Visuales")]
    [Tooltip("Arrastra aquí el GameObject hijo que contiene el SpriteRenderer")]
    [SerializeField] private Transform characterVisuals;

    private Rigidbody2D rb;
    private float moveInputX;
    private float moveInputY;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float defaultGravity;

    private bool isClimbing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking)
        {
            moveInputX = 0f;
            moveInputY = 0f;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }


        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputY = Input.GetAxisRaw("Vertical");


        HandleFacingDirection();
        CheckClimbingState();

        if (IsGrounded()) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        jumpBufferCounter -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;


        if (jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || isClimbing))
        {
            ExecuteJump();
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();

        if (isClimbing)
        {
            ApplyClimbingMovement();
        }
        else
        {
            ApplyGravityModifiers();
        }
    }


    private void CheckClimbingState()
    {
        // Reutilizamos el groundCheck o el centro del jugador para detectar la pared/escalera
        bool touchingClimbable = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, climbableLayer);

        // Si tocamos algo escalable y presionamos Arriba/Abajo, entramos en estado de escalada
        if (touchingClimbable && Mathf.Abs(moveInputY) > 0.1f)
        {
            isClimbing = true;
        }
        // Si dejamos de tocar la pared, nos caemos
        else if (!touchingClimbable)
        {
            isClimbing = false;
        }
    }

    private void ApplyClimbingMovement()
    {
        // Anulamos la gravedad y aplicamos velocidad vertical pura
        rb.gravityScale = 0f;

        // Fricción horizontal extrema: si no te mueves hacia los lados, te quedas pegado a la pared
        float currentVelocityX = (Mathf.Abs(moveInputX) > 0.1f) ? rb.linearVelocity.x : 0f;

        rb.linearVelocity = new Vector2(currentVelocityX, moveInputY * climbSpeed);
    }



    private void ApplyHorizontalMovement()
    {
        float targetSpeed = moveInputX * maxSpeed;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void ApplyGravityModifiers()
    {
        if (rb.linearVelocity.y < 0) rb.gravityScale = fallGravityMultiplier;
        else rb.gravityScale = defaultGravity;
    }

    private void ExecuteJump()
    {
        isClimbing = false;
        rb.gravityScale = defaultGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    private bool IsGrounded()
    {
        // Detección mediante Box para evitar que el personaje se deslice de los bordes sin querer
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }




    private void HandleFacingDirection()
    {
        if (characterVisuals == null) return;

        if (moveInputX > 0.01f) characterVisuals.localScale = new Vector3(1f, 1f, 1f); // Mira a la derecha
        else if (moveInputX < -0.01f) characterVisuals.localScale = new Vector3(-1f, 1f, 1f); // Mira a la izquierda
    }





    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
