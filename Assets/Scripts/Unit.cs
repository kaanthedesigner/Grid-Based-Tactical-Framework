using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int gridPosition;
    public int movementRange = 3;

    // Karakterin yürüme hýzý (Sahnede ne kadar hýzlý kayacak?)
    public float moveSpeed = 5f;

    private GridManager gridManager;

    // YENÝ: Yol bulma sýnýfýmýzdan bir referans
    private PathFinding pathfinder;

    // YENÝ: Karakter þu an yürüyor mu kontrolü (Yürürken yeni týklamalarý engellemek için)
    private bool isMoving = false;

    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();

        // YENÝ: Yol bulucu beynimizi gridManager ile tanýþtýrarak baþlatýyoruz
        pathfinder = new PathFinding(gridManager);

        SnapToGridPosition();
    }

    public void SnapToGridPosition()
    {
        if (gridManager == null) return;

        float cellSize = gridManager.cellSize;
        transform.position = new Vector3(gridPosition.x * cellSize, 0.8f, gridPosition.y * cellSize);

        Node currentNode = gridManager.GetNodeFromGrid(gridPosition);
        if (currentNode != null)
        {
            currentNode.characterOnNode = this.gameObject;
        }
    }

    // --- YENÝLENEN HAREKET FONKSÝYONU ---
    public void MoveToTargetGrid(Vector2Int targetCoords)
    {
        // Eðer karakter zaten yürüyorsa yeni emir alma
        if (isMoving || gridManager == null) return;

        // 1. KONTROL: Hedef kare duvar mý?
        Node targetNode = gridManager.GetNodeFromGrid(targetCoords);
        if (targetNode != null && !targetNode.isWalkable)
        {
            Debug.LogWarning("Olamaz! Bu kare bir duvar!");
            return;
        }

        // 2. KONTROL: Yol bulma algoritmasýný çalýþtýr ve rotayý al
        List<Vector2Int> calculatedPath = pathfinder.FindPath(gridPosition, targetCoords);

        // Eðer yol bulunamadýysa veya yolun uzunluðu menzilimizden büyükse gidemeyiz!
        if (calculatedPath == null || calculatedPath.Count > movementRange)
        {
            Debug.LogWarning("Hedef çok uzak veya yol kapalý!");
            return;
        }

        // Kontrollerden geçtik! Eski durduðumuz kareyi hafýzada boþaltýyoruz
        Node oldNode = gridManager.GetNodeFromGrid(gridPosition);
        if (oldNode != null) oldNode.characterOnNode = null;

        // Adým adým yürüme sürecini (Coroutine) baþlatýyoruz
        StartCoroutine(FollowPathRoutine(calculatedPath));
    }

    // YENÝ: Listeyle gelen kareleri sýrayla yürüten Coroutine algoritmasý
    private IEnumerator FollowPathRoutine(List<Vector2Int> path)
    {
        isMoving = true; // Yürüme baþladý, girdileri kilitle

        float cellSize = gridManager.cellSize;

        // Listenin içindeki her bir hedef koordinat için sýrayla dön
        foreach (Vector2Int nextCoord in path)
        {
            // Gideceðimiz bir sonraki karenin 3D dünyadaki gerçek pozisyonu
            Vector3 targetWorldPos = new Vector3(nextCoord.x * cellSize, 0.8f, nextCoord.y * cellSize);

            // Karakter o kareye tamamen varana kadar döngüde kal
            while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
            {
                // Vector3.MoveTowards: Karakteri mevcut yerinden hedef yere, hýzýmýza göre yumuþakça kaydýrýr
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

                // Bir sonraki kareye (Frame) kadar bekle, sonra while döngüsüne devam et
                yield return null;
            }

            // Kareye tam vardýk! Mantýksal pozisyonumuzu güncelliyoruz
            gridPosition = nextCoord;
        }

        // Tüm yol bitti! En son vardýðýmýz kareyi hafýzada iþgal et
        Node finalNode = gridManager.GetNodeFromGrid(gridPosition);
        if (finalNode != null) finalNode.characterOnNode = this.gameObject;

        isMoving = false; // Yürüme bitti, kilit açýldý
        Debug.Log("Hedefe baþarýyla adým adým ulaþýldý.");
    }
}
