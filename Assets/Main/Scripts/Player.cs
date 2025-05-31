using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public float health;
    public TextMeshProUGUI healthField;


    [Header("Inputs")]
    public InputActionProperty leftJoy;
    public InputActionProperty rightJoy;
    public InputActionProperty rightPrimary;
    public InputActionProperty leftPrimary;
    public InputActionProperty triggerRight;

    [Header("Controllers")]
    public XRDirectInteractor rightHand;
    public XRDirectInteractor leftHand;

    [Header("Sockets")]
    public GameObject gunSocket;
    public GameObject magazineSocket;

    [Header("Headset Transform")]
    public Transform headset;
    private NavMeshAgent navMeshAgent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthField.SetText(health.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        FireWeapon();
        Inventory();
    }

    void FireWeapon()
    {
        Weapon weapon = null;
        if (rightHand.GetOldestInteractableSelected() != null)
        {
            GameObject parent = (rightHand.GetOldestInteractableSelected() as MonoBehaviour)?.gameObject;
            weapon = parent.GetComponentInChildren<Weapon>();
        } else if (leftHand.GetOldestInteractableSelected() != null)
        {
            GameObject parent = (leftHand.GetOldestInteractableSelected() as MonoBehaviour)?.gameObject;
            weapon = parent.GetComponentInChildren<Weapon>(); ;
        }


        if (weapon != null && triggerRight.action.IsPressed())
        {
            weapon.Fire();
        }
    }

    void Movement()
    {
        Vector2 inputVec = leftJoy.action.ReadValue<Vector2>();

        // Only move if input is non-zero
        if (inputVec.sqrMagnitude > 0.01f)
        {
            // Get headset forward and right vectors, projected onto the horizontal plane
            Vector3 forward = headset.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = headset.right;
            right.y = 0;
            right.Normalize();

            // Move in the direction relative to headset orientation
            Vector3 moveVec = (forward * inputVec.y + right * inputVec.x) * 0.1f;

            navMeshAgent.Move(moveVec);
        }
    }

    void Inventory()
    {
        if (rightPrimary.action.WasPressedThisFrame() && !magazineSocket.activeSelf)
        {
            gunSocket.SetActive(true);
        }
        else if (rightPrimary.action.WasReleasedThisFrame())
        {
            gunSocket.SetActive(false);
        }
        if (leftPrimary.action.WasPressedThisFrame() && !gunSocket.activeSelf)
        {
            magazineSocket.SetActive(true);
        }
        else if (leftPrimary.action.WasReleasedThisFrame())
        {
            magazineSocket.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {

        health = health - damage;
        healthField.SetText(health.ToString());
        if (health < 0)
        {
            healthField.SetText("Game Over");
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
