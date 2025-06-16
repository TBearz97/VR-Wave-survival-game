using UnityEngine;

public class ReEnable : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
