using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GunSpawn : MonoBehaviour
{
    public GameObject gun;
    private XRSocketInteractor socket;
    private int gunsCount = 1;
    private GameObject parent;

    private void Start()
    {
        socket = gameObject.GetComponent<XRSocketInteractor>();
        parent = Instantiate(new GameObject(), gameObject.transform);
        parent.name = "Parent";
    }

    private void Update()
    {
        if (!socket.hasSelection)
        {
            if (parent.transform.childCount < gunsCount)
            {
                GameObject spawn = GameObject.Instantiate(gun, gameObject.transform.position, gameObject.transform.rotation, parent.transform);
                spawn.SetActive(true);
            }
        }
    }
}
