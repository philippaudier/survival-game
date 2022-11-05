using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    #region serialized fields
	    [SerializeField] LayerMask groundMask;
        [SerializeField] float initialSpeed = 0f;
	    [SerializeField] float walkSpeed = 2.5f;
        [SerializeField] float runSpeed = 5f;
	    [SerializeField] float turnSpeed = 1000f;
    #endregion

    #region private variables
	    InputActions playerInput;
	    Rigidbody playerRigidbody;
	    Vector3 input;
	    Vector2 moveDirection;
	    Camera mainCamera;
	    bool isAiming = false;
        bool isRunning = false;
    #endregion
    

    private void Awake()
    {
        playerInput = new InputActions();

    }

    private void OnEnable()
    {
        playerInput.Enable();

    }

    private void OnDisable()
    {
        playerInput.Disable();

    }

    private void Start()
    {
        mainCamera = Camera.main;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInput();
        CheckAiming();
        if (!isAiming) {
            Look();
        } else {
            Aim();
        }
        Run();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInput()
    {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
        input = new Vector3(moveDirection.x, 0, moveDirection.y);

    }

    private void Move()
    {
        playerRigidbody.MovePosition(transform.position + input.ToIso() * input.normalized.magnitude * initialSpeed * Time.deltaTime);
    }
    private void Look()
    {
    if (input == Vector3.zero) return;

    Quaternion rotation = Quaternion.LookRotation(input.ToIso(), Vector3.up);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed * Time.deltaTime);
    }

    private void Run() {
        if (playerInput.Player.Run.IsPressed() && !isAiming)
        {
            isRunning = true;
            initialSpeed = runSpeed;
        }
        else
        {
            isRunning = false;
            initialSpeed = walkSpeed;
        }
    }

    private void CheckAiming()
    {
        if (playerInput.Player.Aim.IsPressed()) {
            isAiming = true;
        }
        else {
            isAiming = false;
        }
        
    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            Vector3 direction = position - transform.position;

            // Ignore the height difference.
            direction.y = 0;

            // Quaternion for a smooth rotation towards the pointer
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Make the transform look in the direction.
            // transform.forward = direction;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed * Time.deltaTime);
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundMask))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
        }
    }

}
