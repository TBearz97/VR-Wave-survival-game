using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class Magazine : MonoBehaviour, IMagazine
{
    [field: SerializeField] public int ammoCount { get; set; }
    [field: SerializeField] public int maxAmmo { get; set; }

    private XRGrabInteractable grab;

    public virtual void Initialize (int count, int max)
    {
        ammoCount = count;
        maxAmmo = max;
        grab = gameObject.GetComponent<XRGrabInteractable>();
    }

    public virtual void ConsumeAmmo()
    {
        if (ammoCount > 0)
        {
            ammoCount--;
        }
    }

    public virtual void Despawn ()
    {
        if (!grab.isSelected)
        {
            Destroy(gameObject, 5f);
        }
    }
}
