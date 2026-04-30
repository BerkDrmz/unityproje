using UnityEngine;

public class DusmanCan : MonoBehaviour
{
    [Header("Düşman Ayarları")]
    public float can = 100f; // Düşmanın başlangıç canı

    // Dışarıdan hasar vermek için çağıracağımız fonksiyon
    public void HasarAl(float hasarMiktari)
    {
        can -= hasarMiktari; // Canı hasar kadar düşür
        
        // Konsola bilgi yazdırıyoruz ki çalıştığını görelim
        Debug.Log(gameObject.name + " vuruldu! Kalan Can: " + can);

        // Canı sıfırlandıysa objeyi yok et
        if (can <= 0)
        {
            Debug.Log(gameObject.name + " YOK OLDU!");
            Destroy(gameObject);
        }
    }
}