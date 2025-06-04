using UnityEngine;

public class FNFiveSeven : Weapon
{



    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        base.Despawn();
    }

    public override void Despawn()
    {
        Destroy(gameObject);
    }
}