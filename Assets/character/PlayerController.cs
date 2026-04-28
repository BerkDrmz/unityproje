using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private CharacterController karakterKontrolcu; 
    
    public float hareketHizi = 350f; 
    public float donusHizi = 720f; 
    
    public float yercekimi = -15f; 
    private Vector3 dususHizi; // Sadece Y eksenini tutacak

    private string mevcutAnimasyon;
    private bool aksiyonVarMi = false;

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
            FizikVeHareketKontrolu(); // Hareketi ve yerçekimini birleştirdik
        }
    }

    // --- RAMPALAR İÇİN KUSURSUZ FİZİK MOTORU ---
    void FizikVeHareketKontrolu()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 yatayHareket = new Vector3(moveX, 0f, moveZ).normalized * hareketHizi;
        bool isMoving = yatayHareket.magnitude > 0.1f;

        // --- KRİTİK DÜZELTME: YERÇEKİMİ SIFIRLAMA ---
        // Karakter yerdeyse ivmeyi birikmemesi için hemen resetliyoruz.
        if (karakterKontrolcu.isGrounded && dususHizi.y < 0)
        {
            // Neden 0 değil de -2? Çünkü karakterin yere 'yapışık' kalması gerekir, 
            // yoksa bir sonraki karede Unity 'bu karakter havada' diyebilir.
            dususHizi.y = -2f; 
        }
        else
        {
            // Karakter gerçekten havadaysa (veya rampadan aşağı iniyorsa) yerçekimi biriksin
            dususHizi.y += yercekimi * Time.deltaTime;
        }

        // Hareket vektörünü birleştiriyoruz
        Vector3 sonHareket = yatayHareket;
        sonHareket.y = dususHizi.y;

        // Tek bir hamleyle hareket ettir
        karakterKontrolcu.Move(sonHareket * Time.deltaTime);

        // Karakter dönme ve animasyon kısmı
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
        if (Input.GetMouseButton(1)) 
        {
            aksiyonVarMi = true;
            AnimasyonDegistir("Block", 0.05f);
            
            // Blok yaparken geri kayma (Bunu da tek Move içine alıyoruz)
            Vector3 geriKayma = transform.TransformDirection(Vector3.back) * (hareketHizi * 0.2f);
            geriKayma.y = -5f; // Geri kayarken de yere yapışsın
            karakterKontrolcu.Move(geriKayma * Time.deltaTime);
        }
        else if (Input.GetMouseButtonDown(0)) 
        {
            aksiyonVarMi = true;
            AnimasyonDegistir("Punch Combo", 0.05f);
            Invoke("AksiyonuBitir", 2.18f); 
        }
        else if (!Input.GetMouseButton(1) && mevcutAnimasyon != "Punch Combo")
        {
            aksiyonVarMi = false;
        }
    }

    void AksiyonuBitir()
    {
        aksiyonVarMi = false;
        mevcutAnimasyon = "";
    }

    void AnimasyonDegistir(string yeniAnimasyon, float gecisSuresi)
    {
        if (mevcutAnimasyon == yeniAnimasyon) return;
        animator.CrossFadeInFixedTime(yeniAnimasyon, gecisSuresi);
        mevcutAnimasyon = yeniAnimasyon;
    }
}