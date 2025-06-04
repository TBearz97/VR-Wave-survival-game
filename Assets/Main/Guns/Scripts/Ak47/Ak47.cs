using UnityEngine;

public class Ak47 : Weapon
{

    void Awake()
    {
        Initialize();
    } 

    void Update()
    {
        int handCount = base.XRGrabInteractable.interactorsSelecting.Count;

        if (handCount == 2)
        {
            accuracy = focusAccuracy;
        }
        else if (handCount == 1)
        {
            accuracy = hipfireAccuracy;
        }

        base.Despawn();
    }

    public override void Despawn()
    {
        Destroy(gameObject);
    }
}
