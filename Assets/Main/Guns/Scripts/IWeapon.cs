using UnityEngine.XR.Interaction.Toolkit.Interactors;

public interface IWeapon
{
    public float damage { get; set; }
    public float fireRate { get; set; }
    public float accuracy { get; set; }
    public float hipfireAccuracy { get; set; }
    public float focusAccuracy { get; set; }

    public XRSocketInteractor magazineSlot { get; set; }
    public Magazine currMag { get; set; }

    public void Initialize();
    public void Fire();
    public void Despawn();
}
