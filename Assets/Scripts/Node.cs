using UnityEngine;

public class Node
{

    // 1. Deðiþkenler (Veriler)
    public Vector2Int coordinates; // Karenin X ve Y koordinatý
    public bool isWalkable;        // Bu kareye basýlabilir mi?
    public GameObject characterOnNode; // Bu karede þu an bir karakter var mý? (Yoksa null olur)
    public GameObject visualCubeObject;
    // 2. Constructor (Yapýcý Metot)
    // Bu sýnýf hafýzada her yaratýldýðýnda çalýþacak ve kareye kimliðini verecek.
    public Node(Vector2Int coords, bool walkable,GameObject visualCube)
    {
        coordinates = coords;
        isWalkable = walkable;
        characterOnNode = null; // Ýlk baþta kare boþtur
        // --- YENÝ: Görsel objeyi hafýzaya kaydediyoruz
        visualCubeObject = visualCube;
    }


}
