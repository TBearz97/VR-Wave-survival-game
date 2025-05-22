using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Tower : MonoBehaviour
{
    public float health;

    public void TakeDamage(float damage)
    {
        health = health - damage;

        if (health < 0)
        {
            Debug.Log("Game Over");
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
