using UnityEngine;

// Bu scripti mühür, kalem, dosya gibi tüm tutulabilir nesnelere ekleyeceðiz.
public class PickupItem : MonoBehaviour, IPickable
{
    private Rigidbody rb;

    private void Awake()
    {
        // Component Caching
        rb = GetComponent<Rigidbody>();
    }

    // IPickable arayüzünden gelen zorunlu metodlar
    public void OnPickedUp()
    {
        Debug.Log(gameObject.name + " tutuldu.");
        // Ýleride buraya eþya tutulduðunda çýkacak bir "hýþýrtý" sesi ekleyebiliriz.
    }

    public void OnDropped()
    {
        Debug.Log(gameObject.name + " býrakýldý.");
        // Eþya býrakýldýðýnda fiziksel kýsýtlamalarý kaldýrýyoruz
        rb.constraints = RigidbodyConstraints.None;
    }
}