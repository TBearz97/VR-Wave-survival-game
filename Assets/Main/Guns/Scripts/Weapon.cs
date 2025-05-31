using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public abstract class  Weapon : MonoBehaviour, IWeapon
{
    public float damage { get; set; }
    public float fireRate { get; set; }
    public float range { get; set; }
    public float armorPen { get; set; }
    public float accuracy { get; set; }
    public float hipfireAccuracy { get; set; }
    public float bulletForce { get; set; }

    [field: Header("References")]
    [field: SerializeField] public GameObject bulletPrefab;
    [field: SerializeField] public Transform barrelEnd;
    [field: SerializeField] public XRSocketInteractor magazineSlot { get; set; }
    [field: SerializeField] public XRDirectInteractor rightHand { get; set; }
    [field: SerializeField] public XRDirectInteractor leftHand { get; set; }
    [field: SerializeField] public XRGrabInteractable XRGrabInteractable { get; set; }
    [field: SerializeField] public TextMeshProUGUI ammoField {  get; set; }

    private XRGrabInteractable grab;
    public Magazine currMag { get; set; }

    [field: Header("Input")]
    [field: SerializeField] public InputActionProperty triggerInput { get; set; }

    [field: Header("Audio Resources")]
    [field: SerializeField] public AudioSource audioSource { get; set; }
    [field: SerializeField] public AudioResource gunShot { get; set; }
    [field: SerializeField] public AudioResource magIn { get; set; }
    [field: SerializeField] public AudioResource magOut { get; set; }

    public float nextShotTime { get; set; }

    public virtual void Initialize(float dam, float fRate, float rang, float pen, float acc, float hipacc, float bullForce)
    {
        damage = dam;
        fireRate = fRate;
        range = rang;
        armorPen = pen;
        accuracy = acc;
        hipfireAccuracy = hipacc;
        bulletForce = bullForce;
        grab = gameObject.GetComponentInParent<XRGrabInteractable>();
    }

    public virtual void Fire()
    {
        if (triggerInput.action.IsPressed() && Time.time >= nextShotTime)
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
                Destroy(bullet, 5f);
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
