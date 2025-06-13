using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    private int expCost;
    public TextMeshProUGUI healthField;
    public TextMeshProUGUI expField;
    public MagazineSpawn akMagazineSpawn;
    public MagazineSpawn FNMagazineSpawn;

    public int spawnCount = 20;

    [Header("Audio")]
    public List<AudioClip> grunts;
    private AudioSource audioSource;

    [Header("Canvas")]
    public GameObject pauseMenu;
    public List<GameObject> upgradeButtons;

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
    public GameObject rightPoke;
    public GameObject leftPoke;

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
        expCost = 100;
        rightWeapon = null;
        leftWeapon = null;
        navMeshAgent = GetComponent<NavMeshAgent>();
        updateHealth();
        audioSource = GetComponent<AudioSource>();
        isPaused = true;
        canMove = false;
    }
    
    void Update()
    {
        Movement();
        FireWeapon();
        Inventory();
        LevelUp();

        if (rightSecondary.action.WasPressedThisFrame())
        {
            TogglePause();
        }
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
            weapon.UpdateStats(damage, fireRate, accuracy, hipAccuracy, focusAccuracy);
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
        if (rightPrimary.action.WasPressedThisFrame() && !leftPrimary.action.IsPressed())
        {
            EnterInventoryMode();
            gunSocket.SetActive(true);
            UpdateZPosition(gunSocket.transform.parent.gameObject);
        }

        if (leftPrimary.action.WasPressedThisFrame() && !rightPrimary.action.IsPressed())
        {
            EnterInventoryMode();
            magazineSocket.SetActive(true);
            UpdateZPosition(gunSocket.transform.parent.gameObject);
        }
        if (rightPrimary.action.WasReleasedThisFrame() || leftPrimary.action.WasReleasedThisFrame())
        {
            magazineSocket.SetActive(false);
            gunSocket.SetActive(false);
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
        updateHealth();
        AudioClip clip = grunts[Random.Range(0, grunts.Count - 1)];
        audioSource.PlayOneShot(clip, 1);
        if (health < 0)
        {
            TogglePause();
            pauseMenu.SetActive(true);
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
        if (leftSecondary.action.WasPressedThisFrame() && experience > expCost)
        {
            experience = experience - expCost;
            TogglePause();
            HashSet<GameObject> selectedUpgrades = new HashSet<GameObject>();

            GameObject upgrade1;
            do
            {
                upgrade1 = getRandomUpgrade();
            } while (!selectedUpgrades.Add(upgrade1));
            setupUpgradeButton(upgrade1, new Vector3(0, 0, 0));

            GameObject upgrade2;
            do
            {
                upgrade2 = getRandomUpgrade();
            } while (!selectedUpgrades.Add(upgrade2));
            setupUpgradeButton(upgrade2, new Vector3(0, 60, 0));

            GameObject upgrade3;
            do
            {
                upgrade3 = getRandomUpgrade();
            } while (!selectedUpgrades.Add(upgrade3));
            setupUpgradeButton(upgrade3, new Vector3(0, 120, 0));
        }
    }

    private void setupUpgradeButton(GameObject buttonObj, Vector3 position)
    {
        buttonObj.SetActive(true);
        buttonObj.transform.localPosition = position;

        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();  // Avoid duplicates
            button.onClick.AddListener(OnUpgradeSelected);
        }
    }
    public void OnUpgradeSelected()
    {
        TogglePause();
        foreach (GameObject upgrade in upgradeButtons)
        {
            upgrade.SetActive(false);
        }
    }

    GameObject getRandomUpgrade()
    {
        int rand = Random.Range(0, upgradeButtons.Count);
        return upgradeButtons[rand];
    }

    public void TogglePause()
    {
       
        isPaused = !isPaused;

        if (isPaused)
        {
            rightPoke.SetActive(true);
            leftPoke.SetActive(true);
            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            canMove = false;
            if (rightSecondary.action.WasPressedThisFrame())
            {
                pauseMenu.SetActive(true);
            }
        }
        else
        {
            rightPoke.SetActive(false);
            leftPoke.SetActive(false);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            canMove = true; 
            pauseMenu.SetActive(false);
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

    public void updateHealth()
    {
        healthField.SetText(health.ToString());
    }

    public void UpgradeHealth()
    {
        health += 10;
        updateHealth();
    }

    public void UpgradeDamage()
    {
        damage += 5;
    }

    public void UpgradeFireRate()
    {
        fireRate += 0.1f;
    }

    public void UpgradeHipFire()
    {
        hipAccuracy += 0.05f;
    }

    public void UpgradeFocusFire()
    {
        focusAccuracy += 0.05f;
    }

    public void UpgradeMagSize()
    {
        akMagazineSpawn.maxAmmo += 5;
        FNMagazineSpawn.maxAmmo += 5;
    }
}
