using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    private bool canMove;
    private int experience;
    public TextMeshProUGUI healthField;
    public TextMeshProUGUI expField;

    public int spawnCount = 20;

    [Header("Audio")]
    public List<AudioClip> grunts;
    private AudioSource audioSource;

    [Header("Canvas")]
    public GameObject levelUpScreen;
    public GameObject pauseMenu;

    [Header("Inputs")]
    public InputActionProperty leftJoy;
    public InputActionProperty rightJoy;
    public InputActionProperty rightPrimary;
    public InputActionProperty rightSecondary;
    public InputActionProperty leftPrimary;
    public InputActionProperty leftSecondary;
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
    private bool isPaused;

    
    void Start()
    {
        rightWeapon = null;
        leftWeapon = null;
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthField.SetText(health.ToString());
        audioSource = GetComponent<AudioSource>();
    }

    void StartUp()
    {
        Time.timeScale = 0f;
        canMove = false;
        pauseMenu.SetActive(true);
        isPaused = true;
    }
    
    void Update()
    {
        Movement();
        FireWeapon();
        Inventory();
        LevelUp();
        TogglePause();
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

    public void LevelUp()
    {
        if (leftSecondary.action.WasPressedThisFrame())
        {
            //levelUpScreen.SetActive(true);
        }
    }

    public void TogglePause()
    {
        if (rightSecondary.action.WasPressedThisFrame())
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0f;
                canMove = false;
                pauseMenu.SetActive(true);
                Debug.Log("Paused");
            }
            else
            {
                Time.timeScale = 1f;
                canMove= true;
                pauseMenu.SetActive(false);
                Debug.Log("Unpaused");
            }
        }
    }

    public void GainExperience(int health)
    {
        experience = (int)(experience + (health * 0.1f));
        expField.text = experience.ToString();
    }

    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
