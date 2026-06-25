using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Sahnede duran ana kameramýz
    private Camera mainCamera;
    // --- YENÝ: Sahnede kontrol ettiðimiz karakterin referansý
    private Unit playerUnit;
    private GridManager gridManager;
    private Node lastHoveredNode;
    private PathFinding pathfinder;
    void Start()
    {
        // Sahnede duran "Main Camera" etiketli kamerayý otomatik buluyoruz
        mainCamera = Camera.main;
        // Sahnemizde duran GridManager'ý buluyoruz
        gridManager = FindAnyObjectByType<GridManager>();
        // ---YENÝ: Sahnede duran PlayerUnit(Unit scriptine sahip olan) objeyi buluyoruz
        playerUnit = FindAnyObjectByType<Unit>();
        // --- YENÝ: Pathfinder'ý burada da tanýmlýyoruz ---
        pathfinder = new PathFinding(gridManager);
    }

    void Update()
    {
        HandleMouseHover();
        // 1. Her karede (Frame) mouse'un sol týkýna basýlýp basýlmadýðýný kontrol ediyoruz (0 = Sol Týk)
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
        
    }
    void HandleMouseHover()
    {
        // Eðer karakter o esnada yürüyorsa haritayý boyama, kafasý karýþmasýn
        // (Unit sýnýfýndaki isMoving deðiþkenini public yapmadýðýmýz için þimdilik týklamayý engellediðimiz gibi koruyoruz)

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.name.StartsWith("Cell_"))
            {
                Vector3 hitWorldPos = clickedObject.transform.position;
                Vector2Int hoveredGridCoord = gridManager.GetGridPositionFromWorld(hitWorldPos);
                Node currentHoveredNode = gridManager.GetNodeFromGrid(hoveredGridCoord);

                if (currentHoveredNode != null)
                {
                    // Önce haritadaki tüm eski yeþillikleri/kýrmýzýlarý temizle pýrýl pýrýl yap
                    gridManager.ResetAllCellColors();

                    // 1. DURUM: Eðer baktýðýmýz yer zaten duvarsa, direkt KIRMIZI yap ve bitir
                    if (!currentHoveredNode.isWalkable)
                    {
                        currentHoveredNode.visualCubeObject.GetComponent<Renderer>().material.color = Color.red;
                        return;
                    }

                    // 2. DURUM: Karakterden mouse'un durduðu yere gerçek bir yol arýyoruz!
                    List<Vector2Int> previewPath = pathfinder.FindPath(playerUnit.gridPosition, hoveredGridCoord);
                    // --- YENÝ DOKUNUÞ: Karakterin bastýðý mevcut kareyi de listenin en baþýna (0. indekse) ekle ---
                    if (previewPath != null)
                    {
                        previewPath.Insert(0, playerUnit.gridPosition);
                    }
                    // Eðer gerçek bir yol yoksa (duvarlar kapatmýþsa) VEYA gerçek yol uzunluðu menzili aþýyorsa: KIRMIZI
                    if (previewPath == null || previewPath.Count > playerUnit.movementRange + 1)
                    {
                        currentHoveredNode.visualCubeObject.GetComponent<Renderer>().material.color = Color.red;
                    }
                    // Eðer yol temizse ve menzil içindeyse: TÜM ROTAYI YEÞÝL YAP!
                    else
                    {
                        foreach (Vector2Int pathCoord in previewPath)
                        {
                            Node pathNode = gridManager.GetNodeFromGrid(pathCoord);
                            if (pathNode != null)
                            {
                                pathNode.visualCubeObject.GetComponent<Renderer>().material.color = Color.green;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // Mouse boþluða bakýyorsa her þeyi temizle
            gridManager.ResetAllCellColors();
        }
    }

    void HandleMouseClick()
    {
        // 2. Ekrandaki mouse pozisyonundan 3D dünyaya doðru giden görünmez ýþýný (Ray) hazýrlýyoruz
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Çarpýþma bilgilerini saklayacak olan yapý

        // 3. Iþýný fýrlatýyoruz (Eðer bir þeye çarparsa bu if bloðu çalýþýr)
        if (Physics.Raycast(ray, out hit))
        {
            // Çarptýðýmýz objenin adýný kontrol ediyoruz
            GameObject clickedObject = hit.collider.gameObject;

            // Eðer çarptýðýmýz obje bizim oluþturduðumuz hücrelerden biriyse (Adý "Cell_" ile baþlýyorsa)
           if (clickedObject.name.StartsWith("Cell_"))
            {
                // --- YENÝ: Çarpma noktasýnýn 3D pozisyonunu alýyoruz
                Vector3 hitWorldPos = clickedObject.transform.position;

                // --- YENÝ: GridManager'a bu pozisyonun koordinatýný hesaplatýyoruz
                Vector2Int clickedGridCoord = gridManager.GetGridPositionFromWorld(hitWorldPos);

                // --- YENÝ: Karakterimize yeni koordinata gitme emri veriyoruz!
                if (playerUnit != null)
                {
                    playerUnit.MoveToTargetGrid(clickedGridCoord);

                }
                // Karakter yürümeye baþladýðý an haritadaki boyalarý sýfýrla
                gridManager.ResetAllCellColors();

            }
        }
    }
}
