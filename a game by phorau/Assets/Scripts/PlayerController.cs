using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Camera C_MainCamera;
    [Header("Character Settings")]
    [SerializeField] float P_WalkSpeed = 2f;
    [SerializeField] float P_RunSpeed = 3f;
    [SerializeField] float P_TurnSpeed = 1000f;
    public GameVariables GV;
    [SerializeField] LayerMask groundMask;
    InputActions playerInput;
    private Rigidbody P_RB;
    Vector3 P_Inputs;
    
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
        P_RB = GetComponent<Rigidbody>();
        C_MainCamera = Camera.main;
        GV = GetComponent<GameVariables>();
    }

    private void Update()
    {
        GetMovementDirection();
        SetBoolIsAiming();
        SetBoolIsRunning();
        SetLookDirection();
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetMovementDirection()
    {
        Vector2 moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
        P_Inputs = new Vector3(moveDirection.x, 0, moveDirection.y);
    }

    private void Move()
    {
        if (GV.isCharacterRunning)
            P_RB.MovePosition
            (transform.position + P_Inputs.ToIso().normalized * P_RunSpeed * Time.deltaTime);
        else
            P_RB.MovePosition
            (transform.position + P_Inputs.ToIso().normalized * P_WalkSpeed * Time.deltaTime);
    }

    private void Look()
    {
    if (P_Inputs == Vector3.zero) return;

    Quaternion rotation = Quaternion.LookRotation(P_Inputs.ToIso(), Vector3.up);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, P_TurnSpeed * Time.deltaTime);
    }

    private void SetBoolIsRunning() {
        if (playerInput.Player.Run.IsPressed() && !GV.isCharacterAiming)
            GV.isCharacterRunning = true;
        else
            GV.isCharacterRunning = false;
    }

    private void SetBoolIsAiming()
    {
        if (playerInput.Player.Aim.IsPressed())
            GV.isCharacterAiming = true;
        else
            GV.isCharacterAiming = false;
    }

    private void SetLookDirection()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            Vector3 direction = position - transform.position;

            // Ignore the height difference.
            direction.y = 0;
            if (GV.isCharacterAiming)
            {
                // Quaternion for a smooth rotation towards the pointer
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, P_TurnSpeed * Time.deltaTime);
            }
            else
            {
                if (P_Inputs == Vector3.zero) return;
                Quaternion rotation = Quaternion.LookRotation(P_Inputs.ToIso(), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, P_TurnSpeed * Time.deltaTime);
            }

            // Make the transform look in the direction.
            // transform.forward = direction;
            
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        Ray ray = C_MainCamera.ScreenPointToRay(Input.mousePosition);

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
