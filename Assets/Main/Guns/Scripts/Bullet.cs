using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (enemy.TakeDamage(damage))
            {
                Destroy(this.gameObject);
            }
        } else
        {
            Destroy(this.gameObject);
        }
    }
}
