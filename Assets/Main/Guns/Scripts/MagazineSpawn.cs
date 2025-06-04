using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MagazineSpawn : MonoBehaviour
{
    public GameObject magazine;
    private XRSocketInteractor socket;

    private void Start()
    {
        socket = gameObject.GetComponent<XRSocketInteractor>();
    }

    private void Update()
    {
        if (!socket.hasSelection)
        {
            GameObject mag = Instantiate(magazine, gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
