using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [Header("Açılacak Parıltı / Efekt Objesi")]
    public GameObject zaferPariltisi;

    void Start()
    {
        // Oyun başında parıltı açık unutulmuşsa diye önlem olarak kapatıyoruz
        if (zaferPariltisi != null)
        {
            zaferPariltisi.SetActive(false);
        }

        // Düşmanları her saniye kontrol et (Performans için Update yerine bunu kullanıyoruz!)
        InvokeRepeating("DusmanlariKontrolEt", 1f, 1f);
    }

    void DusmanlariKontrolEt()
    {
        // Sahnedeki "Enemy" etiketli tüm objeleri bul ve bir listeye koy
        GameObject[] dusmanlar = GameObject.FindGameObjectsWithTag("Enemy");

        // Eğer listede hiç eleman kalmadıysa (düşman sayısı 0 ise)
        if (dusmanlar.Length == 0)
        {
            // Lazer şovunu başlat!
            if (zaferPariltisi != null)
            {
                zaferPariltisi.SetActive(true);
            }

            // Artık düşman kalmadığı için sürekli kontrol etmeyi durdur
            CancelInvoke("DusmanlariKontrolEt"); 
        }
    }
}