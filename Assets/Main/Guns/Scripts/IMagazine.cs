using UnityEngine;

public interface IMagazine
{
    int ammoCount { get; set; }
    int maxAmmo {  get; set; }

    void Initialize(int ammoCount, int maxAmmo);
    void ConsumeAmmo();
    void Despawn();
}
