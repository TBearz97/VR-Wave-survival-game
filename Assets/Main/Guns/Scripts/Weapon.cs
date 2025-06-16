using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public abstract class  Weapon : MonoBehaviour, IWeapon
{
    [Header("Canvas")]
    [SerializeField] public TextMeshProUGUI damageText;
    [SerializeField] public TextMeshProUGUI fireRateText;
    [SerializeField] public TextMeshProUGUI hipAccuracyText;
    [SerializeField] public TextMeshProUGUI focusAccuracyText;

    [Header("Base Stats")]
    [SerializeField] public float baseDamage = 33f;
    [SerializeField] public float baseFireRate = 600f / 60f;
    [SerializeField] public float baseAccuracy = 0.85f;
    [SerializeField] public float baseHipAccuracy = 0.30f;
    [SerializeField] public float baseFocusAccuracy = 0.85f;
    [SerializeField] public float baseBulletForce = 100f;
    [SerializeField] public bool baseIsAutomatic = true;
    public float damage { get; set; }
    public float fireRate { get; set; }
    public float armorPen { get; set; }
    public float accuracy { get; set; }
    public float hipfireAccuracy { get; set; }
    public float focusAccuracy { get; set; }
    public float bulletForce { get; set; }

    public bool isAutomatic { get; set; }

    [field: Header("References")]
    [field: SerializeField] public GameObject bulletPrefab;
    [field: SerializeField] public Transform barrelEnd;
    [field: SerializeField] public XRSocketInteractor magazineSlot { get; set; }
    [field: SerializeField] public XRGrabInteractable XRGrabInteractable { get; set; }
    [field: SerializeField] public TextMeshProUGUI ammoField {  get; set; }

    private XRGrabInteractable grab;
    public Magazine currMag { get; set; }

    [field: Header("Audio Resources")]
    [field: SerializeField] public AudioSource audioSource { get; set; }
    [field: SerializeField] public AudioResource gunShot { get; set; }
    [field: SerializeField] public AudioResource magIn { get; set; }
    [field: SerializeField] public AudioResource magOut { get; set; }

    public float nextShotTime { get; set; }

    public virtual void Initialize()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        accuracy = baseAccuracy;
        hipfireAccuracy = baseHipAccuracy;
        focusAccuracy = baseFocusAccuracy;
        bulletForce = baseBulletForce;
        isAutomatic = baseIsAutomatic;
        grab = gameObject.GetComponentInParent<XRGrabInteractable>();
        UpdateCanvas();
    }

    public virtual void UpdateStats(float dam, float fRate, float acc, float hipacc, float focacc)
    {
        damage = dam + baseDamage;
        fireRate = fRate + baseFireRate;
        accuracy = acc + baseAccuracy;
        hipfireAccuracy = hipacc + baseHipAccuracy;
        focusAccuracy = focacc + baseFocusAccuracy;
        UpdateCanvas();
    }

    public virtual void UpdateCanvas()
    {
        damageText.text = ((int)(damage)).ToString();
        fireRateText.text = ((int)(fireRate)).ToString();
        hipAccuracyText.text = hipfireAccuracy.ToString("F2");
        focusAccuracyText.text = focusAccuracy.ToString("F2");
    }

    public virtual void Fire()
    {
        if (Time.time >= nextShotTime)
        {
            if (currMag != null && currMag.ammoCount > 0)
            {
                float spreadAngle = 5f * (1f - accuracy);

                Vector3 direction = barrelEnd.forward;
                direction = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0f) * direction;
                GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelEnd.position, Quaternion.LookRotation(barrelEnd.right));
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                Bullet script = bullet.GetComponent<Bullet>();
                script.damage = damage;
                rb.AddForce(direction * bulletForce);
                audioSource.resource = gunShot;
                audioSource.Play();
                currMag.ConsumeAmmo();
                ammoField.SetText(currMag.ammoCount.ToString());
            }
            nextShotTime = Time.time + 1f / fireRate;
        }
    }



    public virtual void Despawn() {
        if (!grab.isSelected)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    public virtual void MagEntered()
    {
        Magazine go = (magazineSlot.GetOldestInteractableSelected() as MonoBehaviour)?.GetComponent<Magazine>();
        currMag = go;
        ammoField.SetText(currMag.ammoCount.ToString());
        audioSource.resource = magIn;
        audioSource.Play();
    }

    public virtual void MagExited()
    {
        currMag = null;
        audioSource.resource = magOut;
        audioSource.Play();
    }
}
