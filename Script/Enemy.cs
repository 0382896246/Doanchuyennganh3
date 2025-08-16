using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Player player;

    [Header("Movement Details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float distanceToRun;
    private float maxDistance;

    public bool canMove;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceillingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundForwardCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;

    private bool wallDetected;
    private bool isGrounded;
    private bool groundForward;
    private bool ceillingDectected;
    public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;// offset trc khi leo 
    [SerializeField] private Vector2 offset2;// offset sau khi leo 

    private Vector2 climBegunPosition;
    private Vector2 climbOverPostion;

    private bool canGrabLedge=true;
    private bool canClimb;
    private bool justRespawn = true;
    private float defaultGravityScale;

    private void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
        player = GameManager.instance.player;
        rb.velocity = new Vector2(10, 15);
        
        defaultGravityScale = rb.gravityScale;

        rb.gravityScale *=.6f ;

        maxDistance = transform.position.x+ distanceToRun;
    }

    private void Update()
    {
        if (justRespawn )
        {
            if (( rb.velocity.y < 0))
            {
                rb.gravityScale = defaultGravityScale*2;
                
            }
            if (isGrounded)
            {
                rb.velocity = new Vector2(0, 0);
            }
       
        }


        CheckCollision();
        AnimatorCotroller();

        Movement();
        CheckForLedge();
        SpeedControl();

        if (transform.position.x > maxDistance)
        {
            canMove = false;
            return;
        }

        if ( !groundForward || wallDetected)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }

    private void SpeedControl()
    {
        bool playerAhead= player.transform.position.x > transform.position.x;
        bool playerFarAway= Vector2.Distance(player.transform.position, transform.position)> 2.5f;

        if (playerAhead)
        {
            if (playerFarAway)
                moveSpeed = 24;
            else
                moveSpeed = 17;
        }
        else
        {
            if (playerFarAway)
                moveSpeed = 11;
            else
                moveSpeed = 14;
        }
    }

    private void Movement()
    {
        if (justRespawn) return;

        if (canMove)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);

    }

    private void AnimationTrigger()
    {
        rb.gravityScale= defaultGravityScale;
        justRespawn = false;
        canMove = true;
    }

    #region Ledge Climb region
    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;
            rb.gravityScale = 0;
            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climBegunPosition = ledgePosition + offset1; //khoảng cách tương đối từ mép đến vị trí bắt đầu leo
            climbOverPostion = ledgePosition + offset2; //khoảng cách tương đối đến vị trí kết thúc leo

            canClimb = true;
        }
        if (canClimb)
        {
            transform.position = climBegunPosition;
        }
    }

    private void LedgeClimbOver()
    {
        canClimb = false;
        rb.gravityScale = 5;
        transform.position = climbOverPostion;
        Invoke("AllowLedgeGrab", .1f);
    }
    private void AllowLedgeGrab() => canGrabLedge = true;

    #endregion

    private void AnimatorCotroller()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded",isGrounded);
        anim.SetBool("canClimb",canClimb);
        anim.SetBool("justRespawn",justRespawn); 
    }

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        groundForward = Physics2D.Raycast(groundForwardCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceillingDectected = Physics2D.Raycast(transform.position, Vector2.up, ceillingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
        //   Debug.Log(ledgeDetected);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundForwardCheck.position, new Vector2(groundForwardCheck.position.x, groundForwardCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceillingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
