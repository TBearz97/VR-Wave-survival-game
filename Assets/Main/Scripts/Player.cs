using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public float health;
    public float damage;
    public float fireRate;
    public float armorPen;
    public float accuracy;
    public float hipAccuracy;
    public float focusAccuracy;
    public float moveSpeed = 0.04f;
    private bool canMove = true;
    public TextMeshProUGUI healthField;

    [Header("Audio")]
    public List<AudioClip> grunts;
    private AudioSource audioSource;

    [Header("Inputs")]
    public InputActionProperty leftJoy;
    public InputActionProperty rightJoy;
    public InputActionProperty rightPrimary;
    public InputActionProperty leftPrimary;
    public InputActionProperty triggerRight;
    public InputActionProperty triggerLeft;

    [Header("Controllers")]
    public XRDirectInteractor rightHand;
    public XRDirectInteractor leftHand;

    [Header("Sockets")]
    public GameObject gunSocket;
    public GameObject magazineSocket;

    [Header("Headset Transform")]
    public Transform headset;

    private NavMeshAgent navMeshAgent;
    private Weapon rightWeapon;
    private Weapon leftWeapon;

    
    void Start()
    {
        rightWeapon = null;
        leftWeapon = null;
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthField.SetText(health.ToString());
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        Movement();
        FireWeapon();
        Inventory();
    }

    void FireWeapon()
    {
        UpdateWeapon(ref rightWeapon, rightHand);
        UpdateWeapon(ref leftWeapon, leftHand);

        TryFireWeapon(rightWeapon, triggerRight);
        TryFireWeapon(leftWeapon, triggerLeft);
    }

    void UpdateWeapon(ref Weapon weapon, XRBaseInteractor hand)
    {
        var selected = hand.GetOldestInteractableSelected();
        if (selected != null && weapon == null)
        {
            var parent = (selected as MonoBehaviour)?.gameObject;
            weapon = parent?.GetComponentInChildren<Weapon>();
            weapon.UpdateStats(damage, fireRate, armorPen, accuracy, hipAccuracy, focusAccuracy);
        }
        else if (selected == null)
        {
            weapon = null;
        }
    }

    void TryFireWeapon(Weapon weapon, InputActionProperty trigger)
    {
        if (weapon == null) return;

        if (weapon.isAutomatic && trigger.action.IsPressed())
        {
            weapon.Fire();
        }
        else if (!weapon.isAutomatic && trigger.action.WasPressedThisFrame())
        {
            weapon.Fire();
        }
    }

    void Movement()
    {
        if (canMove)
        {
            Vector2 inputVec = leftJoy.action.ReadValue<Vector2>();

            if (inputVec.sqrMagnitude > 0.01f)
            {
                Vector3 forward = headset.forward;
                forward.y = 0;
                forward.Normalize();

                Vector3 right = headset.right;
                right.y = 0;
                right.Normalize();

                Vector3 moveVec = (forward * inputVec.y + right * inputVec.x) * moveSpeed;

                navMeshAgent.Move(moveVec);
            }
        }
    }

    void Inventory()
    {
        if (rightPrimary.action.IsPressed() && !leftPrimary.action.IsPressed())
        {
            EnterInventoryMode();
            gunSocket.SetActive(true);
            UpdateZPosition(gunSocket.transform.parent.gameObject);
        }
        else {
            gunSocket.SetActive(false);
        }

        if (leftPrimary.action.IsPressed() && !rightPrimary.action.IsPressed())
        {
            EnterInventoryMode();
            magazineSocket.SetActive(true);
            UpdateZPosition(gunSocket.transform.parent.gameObject);
        }
        else { 
            magazineSocket.SetActive(false);

        }
        if (!rightPrimary.action.IsPressed() && !leftPrimary.action.IsPressed())
        {
            ExitInventoryMode();
        }

    }

    void UpdateZPosition(GameObject socket)
    {
        float z = socket.transform.localPosition.z;
        z += rightJoy.action.ReadValue<Vector2>().y * 0.01f;
        z = Mathf.Clamp(z, -0.15f, 0.15f);

        socket.transform.localPosition = new Vector3(
            socket.transform.localPosition.x,
            socket.transform.localPosition.y,
            z
        );
    }


    public void TakeDamage(float damage)
    {

        health = health - damage;
        healthField.SetText(health.ToString());
        AudioClip clip = grunts[Random.Range(0, grunts.Count - 1)];
        audioSource.PlayOneShot(clip, 1);
        if (health < 0)
        {
            healthField.SetText("Game Over");
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void EnterInventoryMode()
    {
        canMove = false;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void ExitInventoryMode()
    {
        canMove = true;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
