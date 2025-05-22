using UnityEngine;

public class Ak47 : Weapon
{

    [Header("AK47 Default Stats")]
    [SerializeField] private float baseDamage = 33f;          
    [SerializeField] private float baseFireRate = 600f / 60f;   
    [SerializeField] private float baseRange = 150f;    
    [SerializeField] private float baseArmorPen = 0.40f;  
    [SerializeField] private float baseAccuracy = 0.85f;      
    [SerializeField] private float baseHipAccuracy = 0.30f;
    [SerializeField] private float baseBulletForce = 100f;



    void Awake()
    {
        Initialize(baseDamage,
                   baseFireRate,
                   baseRange,
                   baseArmorPen,
                   baseAccuracy,
                   baseHipAccuracy,
                   baseBulletForce);
    }


    void OnEnable() {
        triggerInput.action.Enable();
        test.action.Enable();
    } 
    void OnDisable() {
        triggerInput.action.Disable();
        test.action.Disable();
    } 

    void Update()
    {
        base.Despawn();
        if (triggerInput.action.IsPressed() && Time.time >= nextShotTime)
        {
            base.Fire();              
            nextShotTime = Time.time + 1f / baseFireRate;
        }
    }

    public override void Despawn()
    {
        Destroy(gameObject);
    }
}
