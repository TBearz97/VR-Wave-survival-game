using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GunSpawn : MonoBehaviour
{
    public GameObject gun;
    private XRSocketInteractor socket;

    private void Start()
    {
        socket = gameObject.GetComponent<XRSocketInteractor>();
    }

    private void Update()
    {
        if (!socket.hasSelection)
        {
            GameObject spawn = GameObject.Instantiate(gun, gameObject.transform.position, gameObject.transform.rotation);
            spawn.SetActive(true);
        }
    }
}
