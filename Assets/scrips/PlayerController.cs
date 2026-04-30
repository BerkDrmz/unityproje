using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private CharacterController karakterKontrolcu; 
    
    public float hareketHizi = 350f; 
    public float donusHizi = 720f; 
    
    public float yercekimi = -15f; 
    private Vector3 dususHizi; 

    private string mevcutAnimasyon;
    private bool aksiyonVarMi = false;

    [Header("--- ZEUS GÜCÜ (E YETENEĞİ) ---")]
    public GameObject yildirimPrefab;
    public GameObject vurusEfektiPrefab;
    
    public float yetenekHasari = 50f; 
    public float saldiriMenzili = 15f;
    public float beklemeSuresi = 10f; 
    public float efektGecikmesi = 0.5f; 
    public float buyuAnimasyonSuresi = 1.5f; 
    private float sonKullanimZamani = -100f;

    void Start()
    {
        animator = GetComponent<Animator>();
        karakterKontrolcu = GetComponent<CharacterController>(); 
    }

    void Update()
    {
        SavasKontrolu();

        if (!aksiyonVarMi)
        {
            FizikVeHareketKontrolu(); 
        }
    }

    void FizikVeHareketKontrolu()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 yatayHareket = new Vector3(moveX, 0f, moveZ).normalized * hareketHizi;
        bool isMoving = yatayHareket.magnitude > 0.1f;

        if (karakterKontrolcu.isGrounded && dususHizi.y < 0)
        {
            dususHizi.y = -2f; 
        }
        else
        {
            dususHizi.y += yercekimi * Time.deltaTime;
        }

        Vector3 sonHareket = yatayHareket;
        sonHareket.y = dususHizi.y;

        karakterKontrolcu.Move(sonHareket * Time.deltaTime);

        if (isMoving)
        {
            Quaternion yeniYon = Quaternion.LookRotation(new Vector3(moveX, 0f, moveZ).normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, yeniYon, donusHizi * Time.deltaTime);
            AnimasyonDegistir("Fast Run", 0.1f);
        }
        else
        {
            AnimasyonDegistir("Bouncing Fight Idle", 0.1f);
        }
    }

   void SavasKontrolu()
    {
        bool eYeteneğiKullanildi = Input.GetKeyDown(KeyCode.E); 

        if (Input.GetMouseButton(1)) 
        {
            aksiyonVarMi = true;
            AnimasyonDegistir("Block", 0.05f);
            
            Vector3 geriKayma = transform.TransformDirection(Vector3.back) * (hareketHizi * 0.2f);
            geriKayma.y = -5f; 
            karakterKontrolcu.Move(geriKayma * Time.deltaTime);
        }
        else if (Input.GetMouseButtonDown(0)) 
        {
            aksiyonVarMi = true;
            AnimasyonDegistir("Punch Combo", 0.05f);
            Invoke("AksiyonuBitir", 2.18f); 
        }
        else if (eYeteneğiKullanildi) 
        {
            if (Time.time >= sonKullanimZamani + beklemeSuresi)
            {
                aksiyonVarMi = true;
                AnimasyonDegistir("BuyuAnim", 0.1f); 
                
                sonKullanimZamani = Time.time;
                
                Invoke("TopluYildirimCak", efektGecikmesi);
                Invoke("AksiyonuBitir", buyuAnimasyonSuresi); 
            }
            else
            {
                float kalanSure = (sonKullanimZamani + beklemeSuresi) - Time.time;
                Debug.Log("Zeus gücü şarj oluyor! Kalan saniye: " + kalanSure.ToString("F1"));
            }
        }
        else if (!Input.GetMouseButton(1) && mevcutAnimasyon != "Punch Combo" && mevcutAnimasyon != "BuyuAnim")
        {
            aksiyonVarMi = false;
        }
    }

    // --- GÜNCELLENEN KISIM: EFEKT YARATMA VE SİLME ---
    void TopluYildirimCak()
    {
        Collider[] etraftakiler = Physics.OverlapSphere(transform.position, saldiriMenzili);

        foreach (Collider obje in etraftakiler)
        {
            if (obje.CompareTag("Enemy"))
            {
                Vector3 dusmanPozisyonu = obje.transform.position;

                if (yildirimPrefab != null)
                {
                    // Efekti düşmanın biraz üzerinde yaratıyoruz (yere gömülmesin)
                    GameObject go = Instantiate(yildirimPrefab, dusmanPozisyonu + Vector3.up * 0.5f, Quaternion.identity);
                    // KRİTİK: Efekti 3 saniye sonra sahnenden siliyoruz!
                    Destroy(go, 3f); 
                }

                if (vurusEfektiPrefab != null)
                {
                    GameObject vfx = Instantiate(vurusEfektiPrefab, dusmanPozisyonu + Vector3.up * 1f, Quaternion.identity);
                    Destroy(vfx, 1.5f); // Vuruş kıvılcımını 1.5 saniye sonra sil
                }

                DusmanCan dusmaninCani = obje.GetComponent<DusmanCan>();
                if (dusmaninCani != null)
                {
                    dusmaninCani.HasarAl(yetenekHasari);
                }
            }
        }
    }

    // --- GÜNCELLENEN KISIM: ANİMASYONU RESETLEME ---
    void AksiyonuBitir()
    {
        aksiyonVarMi = false;
        mevcutAnimasyon = "";
        // Karakter büyü pozu biter bitmez otomatik Idle'a döner
        AnimasyonDegistir("Bouncing Fight Idle", 0.1f);
    }

    void AnimasyonDegistir(string yeniAnimasyon, float gecisSuresi)
    {
        if (mevcutAnimasyon == yeniAnimasyon) return;
        animator.CrossFadeInFixedTime(yeniAnimasyon, gecisSuresi);
        mevcutAnimasyon = yeniAnimasyon;
    }
}