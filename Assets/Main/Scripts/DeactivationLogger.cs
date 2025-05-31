using UnityEngine;

public class DeactivationLogger : MonoBehaviour
{
    void OnDisable()
    {
        Debug.LogWarning($"{gameObject.name} was disabled!", this);
        Debug.LogWarning("Stack trace:\n" + System.Environment.StackTrace);
    }
}
