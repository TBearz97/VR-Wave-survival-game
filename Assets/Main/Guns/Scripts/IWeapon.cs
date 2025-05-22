using UnityEngine;
using UnityEngine.InputSystem;          
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public interface IWeapon
{
    public float damage { get; set; }
    public float fireRate { get; set; }
    public float range { get; set; }
    public float armorPen {  get; set; }
    public float accuracy { get; set; }
    public float hipfireAccuracy { get; set; }

    public XRSocketInteractor magazineSlot { get; set; }
    public Magazine currMag { get; set; }

    public InputActionProperty triggerInput { get; set; }

    public void Initialize(float dam, float fRate, float rang, float pen, float acc, float hipacc, float bullForce);
    public void Fire();
    public void Despawn();
}
