using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    [Header("Efektler")]
    public GameObject yildirimPrefab;
    public GameObject vurusEfektiPrefab;

    [Header("Yetenek Ayarları")]
    public float saldiriMenzili = 15f;
    public float beklemeSuresi = 10f; // Buradaki rakamı Unity Inspector'dan istediğin gibi değiştirebilirsin
    
    // Oyun başlar başlamaz yeteneğin hazır olması için geçmiş bir zaman verdik
    private float sonKullanimZamani = -100f; 

    void Update()
    {
        // 'E' tuşuna basıldıysa
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Zaman kontrolü: Şimdiki zaman, bekleme süresini geçti mi?
            if (Time.time >= sonKullanimZamani + beklemeSuresi)
            {
                TopluYildirimCak();
                sonKullanimZamani = Time.time; // Saati sıfırla/güncelle
            }
            else
            {
                // Süre dolmadıysa konsola ne kadar kaldığını yazdır (Test için)
                float kalanSure = (sonKullanimZamani + beklemeSuresi) - Time.time;
                Debug.Log("Zeus gücü şarj oluyor! Kalan saniye: " + kalanSure.ToString("F1"));
            }
        }
    }

    void TopluYildirimCak()
    {
        Collider[] etraftakiler = Physics.OverlapSphere(transform.position, saldiriMenzili);

        foreach (Collider obje in etraftakiler)
        {
            if (obje.CompareTag("Enemy"))
            {
                Vector3 dusmanPozisyonu = obje.transform.position;

                if (yildirimPrefab != null)
                    Instantiate(yildirimPrefab, dusmanPozisyonu, Quaternion.identity);

                if (vurusEfektiPrefab != null)
                    Instantiate(vurusEfektiPrefab, dusmanPozisyonu + Vector3.up * 1f, Quaternion.identity);

                Destroy(obje.gameObject, 0.5f);
            }
        }
    }
}