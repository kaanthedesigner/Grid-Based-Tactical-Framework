using UnityEngine;
using System.Collections.Generic;
public class PathFinding
{
    private GridManager gridManager;

    public PathFinding(GridManager manager)
    {
        gridManager = manager;
    }

    // Baþlangýç noktasýndan hedef noktaya olan en kýsa yolu koordinat listesi olarak döner
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        // Bilgisayarýn yollarý geriye doðru takip edebilmesi için kimin nereden geldiðini tutan Sözlük
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Gezeceðimiz kareleri sýraya koyduðumuz kuyruk yapýsý (Queue)
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        bool pathFound = false;

        // Kuyrukta kare olduðu sürece haritayý tara
        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            // Hedefe ulaþtýysak aramayý durdur!
            if (current == target)
            {
                pathFound = true;
                break;
            }

            // Þu anki karenin komþularýný GridManager'dan istiyoruz
            foreach (Node neighbor in gridManager.GetNeighbors(current))
            {
                // Eðer bu komþuyu daha önce gezmediysek
                if (!cameFrom.ContainsKey(neighbor.coordinates))
                {
                    frontier.Enqueue(neighbor.coordinates);
                    cameFrom[neighbor.coordinates] = current; // " neighbor'a, current karesinden geldik" diye kaydet
                }
            }
        }

        // Eðer hedef bulunamadýysa (Örn: Etrafý tamamen duvarla kaplýysa) boþ dön
        if (!pathFound) return null;

        // --- Yolu Geriye Doðru Ýnþa Etme ---
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int currentStep = target;

        while (currentStep != start)
        {
            path.Add(currentStep);
            currentStep = cameFrom[currentStep]; // Bir önceki adýma geri git
        }

        path.Reverse(); // Yol tersten kurulduðu için listeyi düzeltiyoruz (Baþlangýçtan hedefe)
        return path;
    }
}
