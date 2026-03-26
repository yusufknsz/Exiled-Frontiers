using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkPos;
    public List<GameObject> spawnedResources = new List<GameObject>();
    public bool isGenerated = false;

    // Uzaklaşıldığında veya yaklaşıldığında kaynakları uyut/uyandır
    public void SetActive(bool active)
    {
        foreach(var res in spawnedResources)
        {
            // Eğer kaynak oyuncu tarafından toplanmışsa (Destroy edilmişse) null olur, onu geç
            if (res != null) 
            {
                res.SetActive(active);
            }
        }
    }
}

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    [Header("Referanslar")]
    [Tooltip("Takip edilecek hedef (örneğin Settler veya Camera)")]
    public Transform playerTransform; 
    [Tooltip("Zemini çizeceğimiz Unity Tilemap objesi")]
    public Tilemap groundTilemap;     
    [Tooltip("Zemine fırça olarak vurulacak Tile (Çimen vb.)")]
    public TileBase groundTile;       

    [Header("Chunk Ayarları")]
    [Tooltip("Bir Chunk kaç x kaç kareden (tile) oluşsun?")]
    public int chunkSizeInCells = 10; 
    [Tooltip("Karakterin etrafında kaç chunklık bir alan aktif kalsın? (1 = etrafındaki 8 chunk)")]
    public int renderDistance = 1;

    [Header("Rastgele Üretim (Procedural)")]
    public GameObject[] resourcePrefabs; 
    [Tooltip("Perlin Noise ölçeği. Düşük değerler devasa ormanlar, yüksek değerler dağınık ağaçlar yapar.")]
    public float noiseScale = 0.15f;     
    [Tooltip("Hangi gürültü seviyesinin üzerinde kaynak çıksın? (0.0 ile 1.0 arası)")]
    [Range(0f, 1f)] public float spawnThreshold = 0.6f; 

    private Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();
    private Vector2Int currentChunkPos = new Vector2Int(-999, -999);
    private float seedOffsetX;
    private float seedOffsetY;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Dünyanın her seferinde (veya sabit kalsın isterseniz belirlediğiniz) farklı olması için
        seedOffsetX = Random.Range(-10000f, 10000f);
        seedOffsetY = Random.Range(-10000f, 10000f);
    }

    void Start()
    {
        if (playerTransform == null && Camera.main != null) 
            playerTransform = Camera.main.transform;
            
        UpdateChunks();
    }

    void Update()
    {
        if (playerTransform == null || groundTilemap == null) return;
        
        // Oyuncunun bulunduğu tile'ı tam olarak bul
        Vector3Int playerCell = groundTilemap.WorldToCell(playerTransform.position);
        
        // Tile'ı chunk koordinatına çevir
        Vector2Int newChunkPos = new Vector2Int(
            Mathf.FloorToInt((float)playerCell.x / chunkSizeInCells),
            Mathf.FloorToInt((float)playerCell.y / chunkSizeInCells)
        );

        // Eğer yeni bir Chunk'a girdiysek sınırları güncelle
        if (newChunkPos != currentChunkPos)
        {
            currentChunkPos = newChunkPos;
            UpdateChunks();
        }
    }

    void UpdateChunks()
    {
        List<Vector2Int> activeChunksThisFrame = new List<Vector2Int>();

        // Oyuncunun etrafındaki X ve Y eksenindeki chunkları tara
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int pos = currentChunkPos + new Vector2Int(x, y);
                activeChunksThisFrame.Add(pos);

                // Eğer o bölgeye ilk kez giriliyorsa üretimi (Generate) yap
                if (!chunks.ContainsKey(pos))
                {
                    GenerateChunk(pos);
                }
                
                // Bölgeyi uykudan uyandır
                chunks[pos].SetActive(true);
            }
        }

        // Kapsam (Görüş / Render Mesafesi) dışında kalan bölgeleri uyut
        foreach (var kv in chunks)
        {
            if (!activeChunksThisFrame.Contains(kv.Key))
            {
                kv.Value.SetActive(false);
            }
        }
    }

    void GenerateChunk(Vector2Int chunkPos)
    {
        ChunkData chunk = new ChunkData { chunkPos = chunkPos, isGenerated = true };
        
        int startX = chunkPos.x * chunkSizeInCells;
        int startY = chunkPos.y * chunkSizeInCells;

        for (int x = 0; x < chunkSizeInCells; x++)
        {
            for (int y = 0; y < chunkSizeInCells; y++)
            {
                int cellX = startX + x;
                int cellY = startY + y;
                Vector3Int cellPos = new Vector3Int(cellX, cellY, 0);

                // 1. ZEMİN ÇİZİMİ (Kullanıcının Tilemap'ine belirlenen çim/toprak Tile'ını yerleştirir)
                if (groundTile != null)
                {
                    groundTilemap.SetTile(cellPos, groundTile);
                }

                // 2. RASTGELE KAYNAK SPASWN (Perlin Noise ile doğal ada/orman görünümü)
                if (resourcePrefabs.Length > 0)
                {
                    // Doğal kümeler oluşturmak için Perlin Noise kullanıyoruz
                    float perlin = Mathf.PerlinNoise(
                        (cellX + seedOffsetX) * noiseScale, 
                        (cellY + seedOffsetY) * noiseScale
                    );

                    // Eğer perlin haritasında ormanlık bir bölgeye (threshold üstüne) denk geldiysek
                    if (perlin > spawnThreshold)
                    {
                        // Üretilecek objenin üzerine veya sağına tam yapışmaması için %30 şansla boş bırakalım (Seyreltme)
                        if (Random.value > 0.3f) 
                        {
                            Vector3 worldPos = groundTilemap.GetCellCenterWorld(cellPos);
                            worldPos.z = 0; 
                            
                            // Eklenen Prefablar arasından (Taş, Odun vb) rastgele birini seç
                            GameObject prefabToSpawn = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
                            GameObject spawned = Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
                            
                            // Editor temizliği için
                            spawned.transform.SetParent(this.transform); 
                            
                            chunk.spawnedResources.Add(spawned);
                        }
                    }
                }
            }
        }
        
        chunks.Add(chunkPos, chunk);
    }
}
