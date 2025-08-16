
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private bool isDead;
    [HideInInspector] public bool playerUnlocked;
    [HideInInspector] public bool extraLife;


    [Header("KnockBack info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    [Header("VFX")]
    [SerializeField] private ParticleSystem dustVFX;
    [SerializeField] private ParticleSystem bloodVFX;

    [Header("Move Info")]
    [SerializeField] private float speedToSurvive = 18;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMax;
    [SerializeField] private float speedMutilier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float defaultMilestoneIncreaser;
    private float speedMilestone;

    private bool readyToLand;

    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTimer; // thời gian lướt 
    [SerializeField] private float slideCoolDown;
    [HideInInspector] public float slideCoolDownCounter;
    private float slideTimerCounter;
    private bool isSliding;

    [Header("Jump Info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDouleJump;



    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceillingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;

    private bool wallDetected;
    private bool isGrounded;
    private bool ceillingDectected;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;// offset trc khi leo 
    [SerializeField] private Vector2 offset2;// offset sau khi leo 

    private Vector2 climBegunPosition;
    private Vector2 climbOverPostion;

    private bool canGrabLedge = true;
    private bool canClimb;
  
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncreaser = milestoneIncreaser;
      
        // Debug.Log(rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckCollision();
        AnimatorController();

        extraLife = moveSpeed >= speedToSurvive;

        slideTimerCounter -= Time.deltaTime; // đếm thời gian 
        slideCoolDownCounter -= Time.deltaTime; // đếm thời gian hồi chiêu


        if (Input.GetKeyDown(KeyCode.K))
            KnockBack();

        if (Input.GetKeyDown(KeyCode.O) && !isDead)
            StartCoroutine(Die());

        if (isDead) return;


        if (isKnocked) return;

        if (playerUnlocked)
            SetMovement();

        if (isGrounded) canDouleJump = true;

        CheckForLanding();
        SpeedController();
        CheckForSlideCancle();
        CheckInput();
        CheckForLedge();

    }

    //khi rơi xuống thì có particle effect  
    private void CheckForLanding()
    {
        if (rb.velocity.y < -5 && !isGrounded)
        {
            readyToLand = true;
        }
        if (readyToLand && isGrounded)
        {
            dustVFX.Play();
            readyToLand = false;
        }
    }

    public void Damage()
    {
        bloodVFX.Play();
        if (extraLife)
            KnockBack();
        else
            StartCoroutine(Die());
    }
    private IEnumerator Die()
    {
        AudioManager.instance.PlaySFX(3);
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);
        Time.timeScale = .6f;
        yield return new WaitForSeconds(.5f);
        Time.timeScale = 1f;

        rb.velocity = new Vector2(0, 0);



        GameManager.instance.GameEnded();
    }
    #region KnockBack
    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

        canBeKnocked = false;
        sr.color = darkenColor;

        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.35f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.4f);

        sr.color = originalColor;
        canBeKnocked = true;

    }
    private void KnockBack()
    {
        if (!canBeKnocked) return;
        moveSpeed = defaultSpeed;
        StartCoroutine(Invincibility());
        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockBack() => isKnocked = false;

    #endregion

    #region Speed Control
    private void SpeedReset()
    {
        if (isSliding) return;
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncreaser;
    }

    private void SpeedController()
    {
        if (moveSpeed == speedMax) return;

        if (transform.position.x > speedMilestone)
        {
            speedMilestone += milestoneIncreaser;
            moveSpeed *= speedMutilier;
            milestoneIncreaser *= speedMutilier;

            if (moveSpeed > speedMax)
                moveSpeed = speedMax;
        }

    }
    #endregion

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
    private void CheckForSlideCancle()
    {
        if (slideTimerCounter < 0 && !ceillingDectected)
            isSliding = false;
    }

    private void SetMovement()
    {
        if (wallDetected)
        {
            //SpeedReset();
            return;
        }


        if (isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);

        else  // nếu ko có else thì slideSpeed sẽ bị ghi đè bởi moveSpeed
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }


    #region Inputs
    public void SlideButton()
    {
        if(isDead) return;  
        if (rb.velocity.x != 0 && slideCoolDownCounter < 0)
        {
            dustVFX.Play();
            // nếu mà đang di chuyển thì cho lướt 
            isSliding = true;
            slideTimerCounter = slideTimer;
            slideCoolDownCounter = slideCoolDown;
        }
    }

    public void JumpButton()
    {
        if (isSliding || isDead) return;

        RollAnimFinished();

        if (isGrounded)
        {
            Jump(jumpForce);
        }
        else if (canDouleJump)
        {
            canDouleJump = false;
            Jump(doubleJumpForce);


        }
    }

    private void Jump(float force)
    {
        dustVFX.Play();
        AudioManager.instance.PlaySFX(Random.Range(1, 2));
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    private void CheckInput()
    {
        //if (Input.GetButtonDown("Fire2")) // chuột phải để bắt đầu chạy

        //    playerUnlocked = true;

        if (Input.GetKeyDown(KeyCode.Space))
            JumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();
    }
    #endregion

    #region Animations

    private void AnimatorController()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("canDoubleJump", canDouleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);

        if (rb.velocity.y < -20)
        {
            anim.SetBool("canRoll", true);
        }
    }

    private void RollAnimFinished() => anim.SetBool("canRoll", false);
    #endregion

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceillingDectected = Physics2D.Raycast(transform.position, Vector2.up, ceillingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
        //   Debug.Log(ledgeDetected);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceillingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
