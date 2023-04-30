using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    static float ARENA_RADIUS = 100;
    static int MAX_ZONE_COUNT = 3;
    static float PIECE_SPAWN_DISTANCE_MULT = 3;
    static float ZONE_SPAWN_DISTANCE_MULT = 5;

    public static SpawnerScript instance;

    public GameObject prefabZone, prefabOmino;
    public LayerMask layerMaskSpawn, layerMaskZone, layerMaskBlock;

    List<GameObject> deliveryZones;

    void Start() {
        instance = this;
        deliveryZones = new List<GameObject>();
    }

    void Update() {
        TrySpawnZone();
        TrySpawnOmino();
    }
    void TrySpawnZone() {
        for (int i = deliveryZones.Count - 1; i >= 0; i--) {
            if (deliveryZones[i] == null) {
                deliveryZones.RemoveAt(i);
            }
        }
        if (deliveryZones.Count >= MAX_ZONE_COUNT) {
            return;
        }
        Vector2 position = Util.GetRandomPointWithinRadius(ARENA_RADIUS);
        int size = Random.Range(4, 9);
        List<Vector2Int> coors = GetRandomOmino(size);
        Vector2Int dimensions = Util.GetCoorsDimensions(coors);
        float checkRadius = Mathf.Sqrt(Mathf.Pow(dimensions.x / 2f, 2) + Mathf.Pow(dimensions.y / 2f, 2)) * OminoScript.INTERBLOCK_DISTANCE;
        if (ZoneCanSpawnHere(position, checkRadius)) {
            // Remove all ominos that overlap.
            HashSet<OminoScript> ominos = new HashSet<OminoScript>(Physics2D.OverlapCircleAll(position, checkRadius * 3, layerMaskBlock).Select(c => c.GetComponent<BlockScript>().omino));
            foreach (OminoScript omino in ominos) {
                Destroy(omino.gameObject);
            }
            // Spawn zone.
            GameObject zone = Instantiate(prefabZone);
            zone.transform.position = position;
            zone.GetComponent<DeliveryZoneScript>().Init(coors);
            deliveryZones.Add(zone);
        }
    }
    void TrySpawnOmino() {
        if (deliveryZones.Count < MAX_ZONE_COUNT) {
            return;
        }
        Vector2 position = Util.GetRandomPointWithinRadius(ARENA_RADIUS);
        int size = -1;
        while (size <= 0) {
            size = Mathf.RoundToInt(Random.Range(-2.5f, 4.5f) + Random.Range(-2.5f, 4.5f));
        }
        if (PieceCanSpawnHere(position, size)) {
            GameObject omino = Instantiate(prefabOmino);
            omino.transform.position = position;
            omino.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            omino.GetComponent<OminoScript>().Init(GetRandomOmino(size));
        }
    }
    bool ZoneCanSpawnHere(Vector2 position, float checkRadius) {
        if (Util.IsPointOnCamera(position, checkRadius)) {
            return false;
        }
        Collider2D c2d = Physics2D.OverlapCircle(position, checkRadius * ZONE_SPAWN_DISTANCE_MULT, layerMaskZone);
        return c2d == null;
    }
    bool PieceCanSpawnHere(Vector2 position, int size) {
        float checkRadius = size * OminoScript.INTERBLOCK_DISTANCE;
        if (Util.IsPointOnCamera(position, checkRadius)) {
            return false;
        }
        Collider2D c2d = Physics2D.OverlapCircle(position, checkRadius * PIECE_SPAWN_DISTANCE_MULT, layerMaskSpawn);
        return c2d == null;
    }

    public OminoScript SpawnEmpty() {
        return Instantiate(prefabOmino).GetComponent<OminoScript>();
    }

    static List<Vector2Int> GetRandomOmino(int n) {
        HashSet<Vector2Int> omino = new HashSet<Vector2Int>();
        HashSet<Vector2Int> frontierSet = new HashSet<Vector2Int>();
        List<Vector2Int> frontierList = new List<Vector2Int>();
        frontierSet.Add(new Vector2Int(0, 0));
        frontierList.Add(new Vector2Int(0, 0));
        while (omino.Count < n) {
            int i = Random.Range(0, frontierList.Count);
            Vector2Int next = frontierList[i];
            frontierList.RemoveAt(i);
            frontierSet.Remove(next);
            omino.Add(next);
            foreach (Vector2Int direction in OminoScript.NEIGHBOR_ORDER) {
                Vector2Int neighbor = next + direction;
                if (!omino.Contains(neighbor) && !frontierSet.Contains(neighbor)) {
                    frontierSet.Add(neighbor);
                    frontierList.Add(neighbor);
                }
            }
        }
        return new List<Vector2Int>(omino);
    }
}
