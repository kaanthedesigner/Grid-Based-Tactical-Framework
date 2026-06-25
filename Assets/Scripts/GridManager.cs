using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // Haritanýn geniþlik ve yükseklik sýnýrlarý
    public int width = 10;
    public int height = 10;

    // --- YENÝ: Görsel küpümüzün boyutunu ve aralarýndaki boþluðu ayarlamak için mesafe
    public float cellSize = 1.1f;

    // Hafýzadaki tüm haritayý tutacak olan Sözlük (Dictionary)
    // <Anahtar Türü, Deðer Türü> -> <Koordinat, Hücre Verisi>
    private Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Awake()
    {
        GenerateGrid();
    }

    // Hafýzada haritayý oluþturan fonksiyon
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int currentCoord = new Vector2Int(x, y);

                // Baþlangýç noktasý (0,0) ve karakterin doðduðu (2,4) karelerini garanti açýk tutalým ki karakter sýkýþmasýn
                bool walkable = true;
                if (currentCoord != new Vector2Int(0, 0) && currentCoord != new Vector2Int(2, 4))
                {
                    // Random.value 0 ile 1 arasýnda sayý döner. 0.15'ten küçükse (%15 þansla) burasý DUWAR olsun!
                    if (Random.value < 0.25f)
                    {
                        walkable = false;
                    }
                }

                // 1. Önce 3D dünyadaki gerçek pozisyonu hesaplýyoruz
                Vector3 worldPosition = new Vector3(x * cellSize, 0, y * cellSize);

                // 2. Unity'nin ilkel 3D Küp objesini sahneye doðuruyoruz
                GameObject visualCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                visualCube.transform.position = worldPosition;
                visualCube.transform.parent = this.transform;
                visualCube.name = "Cell_" + x + "_" + y;
                // --- YENÝ: Küp ilk doðduðunda rengini net olarak BEYAZ yapýyoruz ---
                Renderer cubeRenderer = visualCube.GetComponent<Renderer>();
                if (cubeRenderer != null)
                {
                    // --- YENÝ: Eðer yürünemez bir kareyse rengini SÝYAH yap, yürünebilirse BEYAZ yap ---
                    if (!walkable)
                    {
                        cubeRenderer.material.color = Color.black;
                    }
                    else
                    {
                        cubeRenderer.material.color = Color.white;
                    }
                }


                // 3. ÝÞTE DEÐÝÞEN YER: Artýk Node oluþtururken, yukarýda yarattýðýmýz 'visualCube' objesini de içeri gönderiyoruz!
                Node newNode = new Node(currentCoord, walkable, visualCube);

                // 4. Sözlüðe ekliyoruz
                grid.Add(currentCoord, newNode);
            }
        }

        Debug.Log(width * height + " adet kare engellerle birlikte baþarýyla oluþturuldu!");
    }
    // Dýþarýdan bir koordinat verildiðinde, sözlükten o kareyi bulup veren fonksiyon
    public Node GetNodeFromGrid(Vector2Int coords)
    {
        // Eðer girilen koordinat sözlüðümüzde varsa onu dön
        if (grid.ContainsKey(coords))
        {
            return grid[coords];
        }

        // Eðer harita sýnýrlarý dýþýnda bir koordinat girildiyse boþ (null) dön
        return null;
    }
    // Dýþarýdan 3D dünya pozisyonu verildiðinde, onun hangi (X,Y) koordinatýna denk geldiðini hesaplar
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        // Formülümüz: Pozisyonu cellSize'a bölüp tam sayýya yuvarlýyoruz
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.z / cellSize); // Dünyadaki Z, hafýzadaki Y idi.

        return new Vector2Int(x, y);
    }
    // Bir karenin etrafýndaki (sað, sol, yukarý, aþaðý) yürünebilir komþularýný bulan fonksiyon
    public List<Node> GetNeighbors(Vector2Int coord)
    {
        List<Node> neighbors = new List<Node>();

        // Bakacaðýmýz 4 ana yön (Sað, Sol, Yukarý, Aþaðý)
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),  // Sað
        new Vector2Int(-1, 0), // Sol
        new Vector2Int(0, 1),  // Yukarý
        new Vector2Int(0, -1)  // Aþaðý
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborCoord = coord + dir;
            Node neighborNode = GetNodeFromGrid(neighborCoord);

            // Eðer komþu haritanýn içindeyse VE yürünebilirse (Duvar deðilse!) listeye ekle
            if (neighborNode != null && neighborNode.isWalkable)
            {
                neighbors.Add(neighborNode);
            }
        }

        return neighbors;
    }
    public void ResetAllCellColors()
    {
        foreach (KeyValuePair<Vector2Int, Node> pair in grid)
        {
            Node node = pair.Value;
            Renderer cubeRenderer = node.visualCubeObject.GetComponent<Renderer>();

            if (node.isWalkable)
            {
                cubeRenderer.material.color = Color.white;
            }
            else
            {
                cubeRenderer.material.color = Color.black;
            }
        }
    }
}
