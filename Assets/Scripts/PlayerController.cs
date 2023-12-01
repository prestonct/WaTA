using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // variables to tune:
    public float currentMoveSpeed = 0; // from 0 to 1, based on max speed
    public float moveAcceleration = 50;
    public float moveMaxSpeed = 10;
    public float sprintAccelerationMultiplier = 2;
    public float fallAcceleration = 50;
    public float fallMaxSpeed = 3;
    public float fallDragForce = 1f;
    public float jumpForce = 2000;
    public float rollForce = 1000;
    public AnimationCurve rollForceCurve;
    // data:
    public bool isGrounded = false;
    public bool isSprinting = false;
    public Vector2 inputMovement = Vector2.zero;
    public Vector3 currentForce = Vector3.zero;
    // references:
    public UIManager uIManager;
    public Camera mainCam;
    public Animator animator;
    public Collider playerCollider;
    public Rigidbody rb;
    // jumping:
    private bool didJump = false;
    public bool canDoubleJump = true;
    private bool didDoubleJump = false;
    private bool jumpAnimate = false;

    public float slideTime = 0.5f;
    // rolling:
    public bool rollAnimate = false;
    public bool didRoll = false;
    public float rollCoyoteTime = 0.5f;
    public float rollTime = 1f;
    private float rollTimer = 0;
    private Vector3 rollDirection = Vector3.zero;

    // attacking
    public bool didAttack = false;
    public float attackComboTime = 0.5f;
    public float attackSpeed = 1;
    public float attackMoveAcceleration = 50;
    public float attackMaxMoveSpeed = 3;
    public float attackCancelTime = 0.2f;
    public float attackOneTime = .9f;
    public float attackTwoTime = .65f;
    public float attackThreeTime = 1.53f;
    public GameObject attackObjectOne;
    public GameObject attackObjectTwo;
    public GameObject attackObjectThree;
    private float attackTimer = 0f;
    private int currentAttackNumber = 0;
    private bool attackAnimateOne = false;
    private bool attackAnimateTwo = false;
    private bool attackAnimateThree = false;
    private bool didCancelAttack = false;
    private Vector3 attackDirection = Vector3.zero;
    private int startLines = 0;

    private AudioManager audioManager;
    [SerializeField]
    private ParticleSystem particles1;
    [SerializeField]
    private ParticleSystem particles2;
    [SerializeField]
    private ParticleSystem particles3;
    [SerializeField] private ParticleSystem particlesOnHit;
    public enum State
    {
        Walking,
        Falling,
        Wallrunning,
        Rolling,
        Attacking
    }
    public State currentState = State.Falling;
    public State lastState = State.Falling;

    private void ChangeStateTo(State newState)
    {
        if (currentState == newState) return;

        lastState = currentState;
        currentState = newState;
        if(lastState == State.Walking)
        {
            audioManager.PauseWalkSound();
        }
        if (newState == State.Walking)
        {
            rb.drag = 4;
            canDoubleJump = true;
        }
        else if (newState == State.Falling)
        {
            rb.drag = fallDragForce;
            isGrounded = false;
        }
        else if (newState == State.Rolling)
        {
            rb.drag = 0;
            rollTimer = 0;
            rollCoyoteTime = 0;
        }
        else if (newState == State.Attacking)
        {
            rb.drag = 4;
            rollTimer = 0;
            rollCoyoteTime = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        if(currentState == State.Rolling)
        {
            return; // no damage during roll
        }
        uIManager.playerHUD.TakeDamage(damage);
        particlesOnHit.Play();
        animator.SetTrigger("isHit");
        int rand = UnityEngine.Random.Range(0, 10);
        if(rand < 8)
        {
            audioManager.OnWillowStruckSounds(transform.position);
        }
        else
        {

            audioManager.OnWillowStruckQuips(transform.position);
        }
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        mainCam = FindFirstObjectByType<Camera>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        animator.SetFloat("Attack 1 Time", 2f / attackOneTime);
        animator.SetFloat("Attack 2 Time", 2f / attackTwoTime);
        animator.SetFloat("Attack 3 Time", 2f / attackThreeTime);
        //attackObjectOne = GameObject.Find("Attack 1");
        //attackObjectTwo = GameObject.Find("Attack 2");
        //attackObjectThree = GameObject.Find("Attack 3");
    }


    private void Update()
    {
        // handle animations
        if (jumpAnimate)
        {
            animator.SetTrigger("Jump");
            audioManager.OnWillowJumpSounds(transform.position);
            jumpAnimate = false;
        }
        if (rollAnimate)
        {
            animator.SetTrigger("Roll");
            rollAnimate = false;
        }
        if(didCancelAttack)
        {
            animator.SetTrigger("Cancel Attack");
            animator.SetBool("Attack 1", attackAnimateOne);
            animator.SetBool("Attack 2", attackAnimateTwo);
            animator.SetBool("Attack 3", attackAnimateThree);
            didCancelAttack = false;
        }
        switch (currentState)
        {
            case State.Walking:
                if(currentMoveSpeed > 0.05f)
                {
                    audioManager.PlayWalkSound();
                }
                else
                {
                    audioManager.PauseWalkSound();
                }
                if(lastState == State.Attacking)
                {
                    animator.SetBool("Attack 1", attackAnimateOne);
                    animator.SetBool("Attack 2", attackAnimateTwo);
                    animator.SetBool("Attack 3", attackAnimateThree);
                }
                animator.SetBool("isWalking", currentMoveSpeed > 0.05 && inputMovement.magnitude > 0.1);
                animator.SetFloat("walkSpeed", currentMoveSpeed);
                animator.SetBool("isGrounded", true);
                // animation for walking
                break;
            case State.Falling:
                animator.SetBool("isGrounded", false);
                animator.SetBool("isWalking", false);
                // animation for Falling
                break;
            case State.Wallrunning:
                animator.SetBool("isWalking", false);
                // animation for wallrunning
                break;
            case State.Rolling:
                animator.SetBool("isWalking", false);
                animator.SetBool("isGrounded", true);
                // animation for sliding
                break;
            case State.Attacking:
                animator.SetBool("Attack 1", attackAnimateOne);
                animator.SetBool("Attack 2", attackAnimateTwo);
                animator.SetBool("Attack 3", attackAnimateThree);
                animator.SetBool("isWalking", currentMoveSpeed > 0.05 && inputMovement.magnitude > 0.1);
                animator.SetFloat("walkSpeed", currentMoveSpeed);
                animator.SetBool("isGrounded", true);
                break;
        }
    }

    private void FixedUpdate()
    {
        //if (rb.velocity.y <= Physics.gravity.y && currentState != State.Falling)
        //{
        //    // the player character is falling
        //    ChangeStateTo(State.Falling);
        //}
        // depending on the state, figure out what to do next
        switch (currentState)
        {
            case State.Walking:
                // check if we aren't on the ground anymore
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
                {
                    if (hit.distance >= 1.02f)
                    {
                        ChangeStateTo(State.Falling); break;
                    }
                }
                // check roll, jump, and attack
                if (didRoll)
                {
                    if (rollCoyoteTime < 0)
                    {
                        // we waited too long, reset, don't roll
                        didRoll = false;
                        rollCoyoteTime = 0;
                    }
                    else if (inputMovement.magnitude > 0.2)
                    {
                        // roll in the inputmovement direction
                        rollDirection = Vector3.zero;
                        rollDirection += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z));
                        rollDirection += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z));
                    }
                    else
                    {
                        // roll in whatever direction the player is facing
                        ChangeStateTo(State.Rolling);
                        rollDirection = Vector3.zero;
                        rollDirection = transform.forward;
                    }
                    if (didRoll) // do calculations if we are still able to roll
                    {
                        ChangeStateTo(State.Rolling);
                        rollDirection.Normalize();
                        transform.forward = rollDirection;
                        currentForce += rollDirection * rollForce;
                        rb.AddForce(currentForce);
                        currentForce = Vector3.zero;
                        rollAnimate = true;
                        didRoll = false;
                        didJump = false;
                        didAttack = false;
                        break;
                    }
                }
                if (didJump)
                {
                    // add jumping force, change to falling state
                    currentForce += jumpForce * new Vector3(0, 1, 0);
                    rb.AddForce(currentForce);
                    currentForce = Vector3.zero;
                    jumpAnimate = true;
                    ChangeStateTo(State.Falling);
                    didJump = false;
                    didAttack = false;
                }
                else if (didAttack)
                {
                    ChangeStateTo(State.Attacking); // change to grounded attack mode
                    didAttack = false;
                    break;
                }
                // otherwise add walking force based on input
                if (Math.Abs(inputMovement.y) > 0.25)
                {
                    currentForce += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z)) * moveAcceleration
                        * (isSprinting ? sprintAccelerationMultiplier : 1);
                }
                if (Math.Abs(inputMovement.x) > 0.25)
                {
                    currentForce += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z)) * moveAcceleration
                        * (isSprinting ? sprintAccelerationMultiplier : 1);
                }
                rb.AddForce(currentForce);
                if (currentForce.magnitude > 1)
                {
                    transform.forward = currentForce.normalized;
                }
                currentForce = Vector3.zero;
                if (rb.velocity.magnitude > moveMaxSpeed)
                {
                    rb.velocity = new Vector3(rb.velocity.normalized.x * moveMaxSpeed, rb.velocity.y, rb.velocity.normalized.z * moveMaxSpeed);
                }
                currentMoveSpeed = rb.velocity.magnitude / moveMaxSpeed;
                break;

            case State.Falling:
                // in the air
                if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
                {
                    // we have hit the ground, change to walking
                    if (hit.distance < 1.02f)
                    {
                        ChangeStateTo(State.Walking); break;
                    }
                }
                if (rollCoyoteTime >= 0)
                {
                    rollCoyoteTime -= Time.fixedDeltaTime;
                }
                // air movement
                if (Math.Abs(inputMovement.y) > 0.25)
                {
                    Vector3 camForward = new Vector3(mainCam.transform.forward.x * 0, mainCam.transform.forward.y * 1, mainCam.transform.forward.z * 0);
                    currentForce += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z)) * fallAcceleration;
                }
                if (Math.Abs(inputMovement.x) > 0.25)
                {
                    currentForce += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z)) * fallAcceleration;
                }
                rb.AddForce(currentForce);
                if (currentForce.magnitude > 1)
                {
                    transform.forward = currentForce.normalized;
                }
                if (rb.velocity.magnitude > fallMaxSpeed)
                {
                    rb.velocity = new Vector3(rb.velocity.normalized.x * fallMaxSpeed, rb.velocity.y, rb.velocity.normalized.z * fallMaxSpeed);
                }
                currentMoveSpeed = rb.velocity.magnitude / fallMaxSpeed;
                currentForce = Vector3.zero;
                // check didJump
                if (didDoubleJump)
                {
                    // add jumping force, change to falling state
                    currentForce += jumpForce * 1.6f * new Vector3(0, 1, 0);
                    rb.AddForce(currentForce);
                    currentForce = Vector3.zero;
                    jumpAnimate = true;
                    didDoubleJump = false;
                    canDoubleJump = false;
                }
                if (didAttack)
                {
                    // in air attack, don't switch to the attack state but run the first animation
                    didAttack = false;
                }
                break;
            case State.Wallrunning:
                // TODO
                break;
            case State.Rolling:
                // wait until the roll is over

                // roll in whatever direction the player is facing
                currentForce += rollDirection * rollForce;
                currentForce *= rollForceCurve.Evaluate(rollTimer / rollTime);
                rb.AddForce(currentForce);
                currentForce = Vector3.zero;

                rollTimer += Time.fixedDeltaTime;
                if (rollTimer > rollTime)
                {
                    // end the roll
                    rollTimer = 0;
                    ChangeStateTo(State.Walking);
                }
                break;
            case State.Attacking:
                // we started attacking, check for the player to bail in a certain amount of time
                // check for the next attack input, cache that until the current attack is over
                // 
                attackTimer += Time.fixedDeltaTime;
                if (currentAttackNumber == 0)
                {
                    // we haven't started attacking, start the first one
                    attackAnimateOne = true;
                    currentAttackNumber++;
                    attackTimer = 0;
                    int rand = UnityEngine.Random.Range(0, 10);
                    if (rand < 7)
                    {
                        audioManager.OnWillowAttackQuips(transform.position);
                    }
                }
                else if(currentAttackNumber == 1)
                {
                    if(didRoll)
                    {
                        if(attackTimer < attackCancelTime)
                        {
                            // we canceled, go out
                            didAttack = false;
                            currentAttackNumber = 0;
                            attackAnimateOne = false;
                            attackObjectOne.SetActive(false);
                            didCancelAttack = true;

                            if (inputMovement.magnitude > 0.2)
                            {
                                // roll in the inputmovement direction
                                rollDirection = Vector3.zero;
                                rollDirection += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z));
                                rollDirection += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z));
                            }
                            else
                            {
                                // roll in whatever direction the player is facing
                                ChangeStateTo(State.Rolling);
                                rollDirection = Vector3.zero;
                                rollDirection = transform.forward;
                            }
                            ChangeStateTo(State.Rolling);
                            rollDirection.Normalize();
                            transform.forward = rollDirection;
                            currentForce += rollDirection * rollForce;
                            rb.AddForce(currentForce);
                            currentForce = Vector3.zero;
                            rollAnimate = true;
                            didRoll = false;
                            didJump = false;
                            didAttack = false;
                            break;

                        }
                        else
                        {
                            didRoll = false;
                        }
                    }
                    // we are in the first attack
                    if (attackTimer > (attackOneTime * 0.5f))
                    {
                        if (!attackObjectOne.activeSelf)
                        {
                            // play particles, create hitbox
                            particles1.Play();
                            audioManager.OnWillowAttackSounds(transform.position);
                            attackObjectOne.SetActive(true);
                        }

                        // prepare to move on to the next attack
                        if (didAttack)
                        {
                            attackAnimateTwo = true;
                            didAttack = false;
                        }
                    }
                    if(attackTimer > attackOneTime)
                    {
                        if (attackAnimateTwo)
                        {
                            // move on to the next one
                            currentAttackNumber++;
                            attackTimer = 0;
                            attackObjectOne.SetActive(false);
                            int rand = UnityEngine.Random.Range(0, 10);
                            if (rand < 7)
                            {
                                audioManager.OnWillowAttackQuips(transform.position);
                            }
                            //particles1.Pause();
                        }
                        else
                        {
                            // we are done, timer is over
                            attackAnimateOne = false;
                            attackObjectOne.SetActive(false);
                            //particles1.Pause();
                            didAttack = false;
                            currentAttackNumber = 0;
                            attackTimer = 0;
                            ChangeStateTo(State.Walking);
                        }
                        
                    }
                }
                else if(currentAttackNumber == 2)
                {
                    if (didRoll)
                    {
                        if (attackTimer < attackCancelTime)
                        {
                            // we canceled, go out
                            didAttack = false;
                            currentAttackNumber = 0;
                            attackAnimateOne = false;
                            attackAnimateTwo = false;
                            attackObjectTwo.SetActive(false);
                            //particles2.Pause();
                            didCancelAttack = true;

                            if (inputMovement.magnitude > 0.2)
                            {
                                // roll in the inputmovement direction
                                rollDirection = Vector3.zero;
                                rollDirection += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z));
                                rollDirection += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z));
                            }
                            else
                            {
                                // roll in whatever direction the player is facing
                                ChangeStateTo(State.Rolling);
                                rollDirection = Vector3.zero;
                                rollDirection = transform.forward;
                            }
                            ChangeStateTo(State.Rolling);
                            rollDirection.Normalize();
                            transform.forward = rollDirection;
                            currentForce += rollDirection * rollForce;
                            rb.AddForce(currentForce);
                            currentForce = Vector3.zero;
                            rollAnimate = true;
                            didRoll = false;
                            didJump = false;
                            didAttack = false;
                            break;
                        }
                        else
                        {
                            didRoll = false;
                        }
                    }
                    if (attackTimer > (attackTwoTime * 0.5f))
                    {
                        if (!attackObjectTwo.activeSelf)
                        {
                            attackObjectTwo.SetActive(true);
                            particles2.Play();

                            audioManager.OnWillowAttackSounds(transform.position);
                        }
                        if (didAttack)
                        {
                            // prepare to move on to the next attack
                            attackAnimateThree = true;
                            didAttack = false;
                        }
                    }
                    if (attackTimer > attackTwoTime)
                    {
                        if (attackAnimateThree)
                        {
                            // move on to the next one
                            currentAttackNumber++;
                            attackTimer = 0;
                            attackObjectTwo.SetActive(false);
                            int rand = UnityEngine.Random.Range(0, 10);
                            if (rand < 6)
                            {
                                audioManager.OnWillowAttackQuips(transform.position);
                            }
                            //particles2.Pause();
                        }
                        else
                        {
                            // we are done
                            attackAnimateOne = false;
                            attackAnimateTwo = false;
                            didAttack = false;
                            attackObjectTwo.SetActive(false);
                            //particles2.Pause();
                            currentAttackNumber = 0;
                            attackTimer = 0;
                            ChangeStateTo(State.Walking);
                        }
                    }
                }
                else if(currentAttackNumber == 3)
                {
                    if (didRoll)
                    {
                        if (attackTimer < attackCancelTime)
                        {
                            // we canceled, go out
                            didAttack = false;
                            currentAttackNumber = 0;
                            attackAnimateOne = false;
                            attackAnimateTwo = false;
                            attackAnimateThree = false;
                            attackObjectThree.SetActive(false);
                            //particles3.Pause();
                            didCancelAttack = true;

                            if (inputMovement.magnitude > 0.2)
                            {
                                // roll in the inputmovement direction
                                rollDirection = Vector3.zero;
                                rollDirection += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z));
                                rollDirection += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z));
                            }
                            else
                            {
                                // roll in whatever direction the player is facing
                                ChangeStateTo(State.Rolling);
                                rollDirection = Vector3.zero;
                                rollDirection = transform.forward;
                            }
                            ChangeStateTo(State.Rolling);
                            rollDirection.Normalize();
                            transform.forward = rollDirection;
                            currentForce += rollDirection * rollForce;
                            rb.AddForce(currentForce);
                            currentForce = Vector3.zero;
                            rollAnimate = true;
                            didRoll = false;
                            didJump = false;
                            didAttack = false;
                            break;
                        }
                        else
                        {
                            didRoll = false;
                        }
                    }
                    // finish attacking
                    if(attackTimer > attackThreeTime * 0.5f)
                    {
                        if (!attackObjectThree.activeSelf)
                        {
                            attackObjectThree.SetActive(true);
                            particles3.Play();
                            
                            audioManager.OnWillowAttackSounds(transform.position);
                        }
                    }
                    if (attackTimer > attackThreeTime)
                    {
                        attackAnimateOne = false; attackAnimateTwo = false; attackAnimateThree = false;
                        attackObjectThree.SetActive(false);
                        didAttack = false;
                        currentAttackNumber = 0;
                        ChangeStateTo(State.Walking);
                    }
                }
                // attack movement
                if (Math.Abs(inputMovement.y) > 0.25)
                {
                    Vector3 camForward = new Vector3(mainCam.transform.forward.x * 0, mainCam.transform.forward.y * 1, mainCam.transform.forward.z * 0);
                    currentForce += (inputMovement.y * new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z)) * attackMoveAcceleration;
                }
                if (Math.Abs(inputMovement.x) > 0.25)
                {
                    currentForce += ((inputMovement.x) * new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z)) * attackMoveAcceleration;
                }
                rb.AddForce(currentForce);
                if (currentForce.magnitude > 1)
                {
                    transform.forward = currentForce.normalized;
                }
                if (rb.velocity.magnitude > attackMaxMoveSpeed)
                {
                    rb.velocity = new Vector3(rb.velocity.normalized.x * attackMaxMoveSpeed, rb.velocity.y, rb.velocity.normalized.z * attackMaxMoveSpeed);
                }
                currentMoveSpeed = rb.velocity.magnitude / moveMaxSpeed;
                currentForce = Vector3.zero;
                break;
        }
    }

    #region CollisionDetection

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Player Collision with Layer " + collision.gameObject.layer);
    //    if (collision.gameObject.layer == 6) // hit the ground
    //    {
    //        switch (currentState)
    //        {
    //            case State.Walking:
    //                // maybe wallrunning?
    //                break;
    //            case State.Falling:
    //                // change out of falling, into either wallrunning or walking, depending on the collision
    //                ChangeStateTo(State.Walking);
    //                break;
    //            case State.Wallrunning:
    //                // maybe back to walking, landing?
    //                break;
    //            case State.Rolling:
    //                // maybe stop rolling?
    //                break;
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 12:
                // the start line
                if (startLines == 0)
                {
                    audioManager.OnWillowGameStart(transform.position);
                    startLines = 1;
                }
                else if (startLines == 1)
                {
                    startLines = 2;
                    audioManager.OnWillowMusings(transform.position);
                }
                break;
        }
    }

        //}

        #endregion

        #region InputFunctions
        void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (currentState == State.Walking)
        {
            didJump = true;
        }
        else if (currentState == State.Falling && canDoubleJump)
        {
            didDoubleJump = true;
        }
    }

    void OnAttack(InputValue value)
    {
        if (currentState == State.Walking || currentState == State.Falling)
        {
            didAttack = true;
        }
        if (currentState == State.Attacking)
        {
            didAttack = true;
        }
    }

    void OnRoll(InputValue value)
    {
        if (currentState == State.Walking)
        {
            didRoll = true;
            rollTimer = 0;
            rollCoyoteTime = 0;
        }
        if (currentState == State.Falling)
        {
            didRoll = true;
            rollTimer = 0;
            rollCoyoteTime = 0.5f;
        }
        if(currentState == State.Attacking)
        {
            didRoll = true;
            rollTimer = 0;
            rollCoyoteTime = 0;
        }
    }

    void OnPause()
    {
        uIManager.pauseMenu.onPause();
    }

    void OnSprint(InputValue value)
    {
        isSprinting = value.Get<float>() > 0f;
        Debug.Log(isSprinting);
    }

    void OnInventory()
    {
        uIManager.inventory.OpenInventory();
    }

    #endregion
}
