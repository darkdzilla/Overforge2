using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Prefab & Spawn Points")]
    [SerializeField] private LoadInteractable loadInteractablePrefab;
    [Tooltip("Up to 4 points in the scene where orders can appear")]
    [SerializeField] private Transform[] spawnPoints = new Transform[4];

    [Header("Possible Products")]
    [Tooltip("Final products that can be requested as orders in this level")]
    [SerializeField] private ItemSO[] possibleProducts;

    [Header("Order Limits")]
    [SerializeField, Min(1)] private int minActiveOrders = 1;
    [SerializeField, Min(1)] private int maxActiveOrders = 4;

    [Header("Order Quantity")]
    [SerializeField, Min(1)] private int minQuantity = 1;
    [SerializeField, Min(1)] private int maxQuantity = 4;

    [Header("Timing")]
    [Tooltip("Ideal time the player takes to craft ONE unit of a product. " +
             "Multiplied by the order quantity to get its duration.")]
    [SerializeField] private float timePerProduct = 20f;

    [Tooltip("Random interval (seconds) between spawn attempts")]
    [SerializeField] private float minSpawnInterval = 6f;
    [SerializeField] private float maxSpawnInterval = 14f;

    // Index in this array maps 1:1 to spawnPoints[i]. null slot = free.
    private LoadInteractable[] activeOrders;
    private float nextSpawnTime;

    private void Start()
    {
        activeOrders = new LoadInteractable[spawnPoints.Length];

        // Spawn one right away so the player has something to do.
        TrySpawnOrder();
        ScheduleNextSpawn();
    }

    private void Update()
    {
        int activeCount = CountActive();

        // Guarantee the minimum: if we drop below, force a spawn right away.
        if (activeCount < minActiveOrders)
        {
            if (TrySpawnOrder()) ScheduleNextSpawn();
            return;
        }

        // Regular timed spawn up to the max cap.
        if (Time.time >= nextSpawnTime && activeCount < maxActiveOrders)
        {
            TrySpawnOrder();
            ScheduleNextSpawn();
        }
    }

    // -------------------------------------------------------------------------
    // Spawning
    // -------------------------------------------------------------------------

    private bool TrySpawnOrder()
    {
        if (loadInteractablePrefab == null)
        {
            Debug.LogWarning("LevelManager: LoadInteractable prefab not assigned.");
            return false;
        }

        if (possibleProducts == null || possibleProducts.Length == 0)
        {
            Debug.LogWarning("LevelManager: no possible products configured.");
            return false;
        }

        int spawnIndex = PickFreeSpawnIndex();
        if (spawnIndex == -1) return false;

        // Pick a random product and quantity.
        ItemSO product = possibleProducts[Random.Range(0, possibleProducts.Length)];
        int quantity   = Random.Range(minQuantity, maxQuantity + 1);
        float duration = timePerProduct * quantity;

        Transform point = spawnPoints[spawnIndex];
        LoadInteractable order = Instantiate(loadInteractablePrefab,
                                             point.position,
                                             point.rotation);

        // Capture spawnIndex in the closure so we free the right slot later.
        int capturedIndex = spawnIndex;
        order.Setup(
            product,
            quantity,
            duration,
            onCompleted: () => OnOrderFinished(capturedIndex, completed: true),
            onExpired:   () => OnOrderFinished(capturedIndex, completed: false)
        );

        activeOrders[spawnIndex] = order;
        return true;
    }

    private int PickFreeSpawnIndex()
    {
        List<int> free = new List<int>();
        for (int i = 0; i < activeOrders.Length; i++)
            if (activeOrders[i] == null) free.Add(i);

        if (free.Count == 0) return -1;
        return free[Random.Range(0, free.Count)];
    }

    private void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    // -------------------------------------------------------------------------
    // Callbacks
    // -------------------------------------------------------------------------

    private void OnOrderFinished(int spawnIndex, bool completed)
    {
        activeOrders[spawnIndex] = null;
        // TODO: scoring / penalties here based on `completed`.
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private int CountActive()
    {
        int count = 0;
        for (int i = 0; i < activeOrders.Length; i++)
            if (activeOrders[i] != null) count++;
        return count;
    }
}
