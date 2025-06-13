using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MagazineSpawn : MonoBehaviour
{
    public GameObject magazine;
    private XRSocketInteractor socket;
    public int maxAmmo;
    public TextMeshProUGUI maxAmmoText;

    private void Start()
    {
        socket = gameObject.GetComponent<XRSocketInteractor>();
        //maxAmmo = magazine.GetComponent<Magazine>().maxAmmo;
    }

    private void Update()
    {
        if (!socket.hasSelection)
        {
            GameObject mag = Instantiate(magazine, gameObject.transform.position, gameObject.transform.rotation);
            //mag.GetComponent<Magazine>().maxAmmo = maxAmmo;
            //maxAmmoText.text = maxAmmo.ToString();
        }
    }
}
