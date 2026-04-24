using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("Item Prefabs")]
    [SerializeField] private List<GameObject> itemPrefabs;

    [Header("Spawn Area Settings")]
    [SerializeField] private BoxCollider2D spawnArea; // ลาก Collider ที่กำหนดขอบเขตการเกิดมาใส่
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxItemsInScene = 3;

    private List<GameObject> currentItems = new List<GameObject>();
    private float spawnTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.IsBattleActive)
        {
            HandleSpawning();
        }
    }

    private void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            currentItems.RemoveAll(item => item == null);

            if (currentItems.Count < maxItemsInScene)
            {
                SpawnRandomItem();
            }
        }
    }

    public void SpawnRandomItem()
    {
        if (itemPrefabs.Count == 0 || spawnArea == null) return;

        // 1. สุ่มเลือกไอเทม
        GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

        // 2. คำนวณหาตำแหน่งสุ่มจากขนาดของ Collider
        Vector2 randomPosition = GetRandomPositionInCollider(spawnArea);

        // 3. สร้างไอเทม
        GameObject spawnedItem = Instantiate(randomPrefab, randomPosition, Quaternion.identity);
        currentItems.Add(spawnedItem);
    }

    private Vector2 GetRandomPositionInCollider(BoxCollider2D collider)
    {
        // ดึงขอบเขตของ Collider (Bounds)
        Bounds bounds = collider.bounds;

        // สุ่มค่า X ระหว่างขอบซ้ายและขอบขวา
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        // สุ่มค่า Y ระหว่างขอบบนและขอบล่าง
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }

    public void ClearAllItems()
    {
        foreach (var item in currentItems)
        {
            if (item != null) Destroy(item);
        }
        currentItems.Clear();
    }
}