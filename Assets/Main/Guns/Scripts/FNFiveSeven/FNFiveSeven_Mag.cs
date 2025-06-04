using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FNFiveSeven_Mag : Magazine
{
    private int startAmmo = 30;
    private int capacity = 30;

    void Awake()
    {
        Initialize(startAmmo, capacity);
    }

    private void Update()
    {
        base.Despawn();
    }
}
