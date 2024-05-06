using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject[] weapons;
    [SerializeField] private SpriteRenderer sr;
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    private SpriteRenderer spriteRenderer;
    [SerializeField] float horizontalInput;
    PlayerInput playerInput;

    [Header("Player Direction")]
    [SerializeField] bool isFacingRight = true;
    [SerializeField] Vector2 moveAxis;

    [Header("Movement")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float groundedDrag;
    [SerializeField] private float midAirDrag;
    private float appliedDrag;
    private bool changingDirection => ((rb.velocity.x > 0f && moveAxis.x < 0f) || (rb.velocity.x < 0f && moveAxis.x > 0f));

    [Header("Ground Check")]
    [SerializeField] float groundedAreaLength;
    [SerializeField] float groundedAreaHeight;
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowFallMultiplier;
    [SerializeField] private float jumpForce;

    [Header("Dash")]
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashStrength;
    [SerializeField] private float dashesAvailable;
    [SerializeField] private float maxDashes;
    [SerializeField] private Color dashColor;
    [SerializeField] private float dashColorAmount;
    private Vector2 lastDashAxis;
    private bool isDashing;

    private void Awake()
    {
        //set current health
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        GetInput();
        FacingDirection();

        if (isGrounded) { dashesAvailable = maxDashes; }
    
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        Movement();
        if (!isDashing)
        {
            ApplyLinearDrag();
            FallMultiplier();
            LinearDrag();
        }
    }

    void CheckCollisions()
    {
        //GROUNDCHECK
        isGrounded = Physics2D.OverlapArea(
                        new Vector2(groundCheck.position.x - (groundedAreaLength / 2),
                                    groundCheck.position.y - groundedAreaHeight),
                        new Vector2(groundCheck.position.x + (groundedAreaLength / 2),
                                    groundCheck.position.y + 0.01f),
                                    groundLayer);

    }

    private void Movement()
    {
        if (moveAxis.x > 0 && rb.velocity.x < maxMoveSpeed)
        {
            rb.AddForce(new Vector2(moveAxis.x, 0f) * movementAcceleration, ForceMode2D.Impulse);
        }
        if (moveAxis.x < 0 && rb.velocity.x > -maxMoveSpeed)
        {
            rb.AddForce(new Vector2(moveAxis.x, 0f) * movementAcceleration, ForceMode2D.Impulse);
        }
    }

    private void LinearDrag()
    {
        float linearDrag;
        if (isGrounded)
        { linearDrag = groundedDrag; }
        else
        { linearDrag = midAirDrag; }

        if (Mathf.Abs(moveAxis.x) < 0.4f || changingDirection)
        {
            appliedDrag = linearDrag;
        }
        else
        {
            appliedDrag = 0f;
        }
    }

    private void ApplyLinearDrag()
    {
        rb.velocity = new Vector2((rb.velocity.x / (1f + appliedDrag / 50)), rb.velocity.y);
    }

    private void FallMultiplier()
    {
        if (rb.velocity.y < 0.6f)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0f && playerInput.actions["Jump"].ReadValue<float>() == 0)
        {
            rb.gravityScale = lowFallMultiplier;
        }
        else
        {
            rb.gravityScale = 2f;
        }
    }

    //INPUTS

    private void GetInput()
    {
        moveAxis = playerInput.actions["Movement"].ReadValue<Vector2>();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            //anim.SetTrigger("jump");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void ResetGravityScale()
    {
        rb.gravityScale = 1f;
        rb.velocity = new Vector2(maxMoveSpeed * lastDashAxis.x, rb.velocity.y * .5f);
        isDashing = false;
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && dashesAvailable > 0)
        {
            Debug.Log("dashed");
            dashesAvailable -= 1;
            isDashing = true;
            //sfx[0].Play();
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            lastDashAxis = moveAxis;
            lastDashAxis.Normalize();
            rb.AddForce(lastDashAxis * dashStrength, ForceMode2D.Impulse);
            StartCoroutine(FlashColor(dashColor, dashDuration, dashColorAmount));
            Invoke("ResetGravityScale", dashDuration);

        }
    }



    private IEnumerator FlashColor(Color color, float duration, float amount)
    {
        sr.material.color = color;
        float elapsedTime = 0f;
        while (elapsedTime < duration + .1f)
        {
            elapsedTime += Time.deltaTime;
            float mult = Mathf.Lerp(amount, 0, elapsedTime / .2f);

            mult = Mathf.Abs(mult);

            // Set the multiplier in the shader
            sr.material.SetFloat("_Multiplier", mult);
            yield return null; // Wait for the next frame
        }
        sr.material.SetFloat("_Multiplier", 0);
        Debug.Log("coloreset");
    }

    //ANIMATIONS


    // -- Animations method --

    private void FacingDirection()
    {
        if (!isFacingRight & rb.velocity.x > 0)
        { Flip(); }
        else if (isFacingRight & rb.velocity.x < 0)
        { Flip(); }
    }

    void Flip()
    {
        Vector2 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

}
