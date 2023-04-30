using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    static float PIECE_SPAWN_DISTANCE_MULT = 3;
    static float ZONE_SPAWN_DISTANCE_MULT = 5;
    static Vector3Int[] STARTING_TARGET = new Vector3Int[] { new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0) };
    static Vector3Int[] MONOMINO = new Vector3Int[] { new Vector3Int(0, 0, 0) };

    public static SpawnerScript instance;

    public GameObject prefabZone, prefabOmino;
    public LayerMask layerMaskSpawn, layerMaskZone, layerMaskBlock;

    List<GameObject> deliveryZones;
    int maxZoneCount;

    void Start() {
        instance = this;
        deliveryZones = new List<GameObject>();
        GameObject zone = Instantiate(prefabZone);
        zone.transform.position = new Vector3(-25, 0, 0);
        zone.GetComponent<DeliveryZoneScript>().Init(STARTING_TARGET);
        deliveryZones.Add(zone);
        GameObject omino = Instantiate(prefabOmino);
        omino.transform.position = new Vector3(20, -5, 0);
        omino.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        omino.GetComponent<OminoScript>().Init(MONOMINO);
        omino = Instantiate(prefabOmino);
        omino.transform.position = new Vector3(27, 6, 0);
        omino.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        omino.GetComponent<OminoScript>().Init(MONOMINO);
    }

    void Update() {
        float area = Mathf.PI * Mathf.Pow(GameHelper.instance.arenaRadius, 2);
        maxZoneCount = Mathf.RoundToInt(area / 12000);
        TrySpawnZone();
        TrySpawnOmino();
    }
    void TrySpawnZone() {
        for (int i = deliveryZones.Count - 1; i >= 0; i--) {
            if (deliveryZones[i] == null) {
                deliveryZones.RemoveAt(i);
            }
        }
        if (deliveryZones.Count >= maxZoneCount) {
            return;
        }
        Vector2 position = Util.GetRandomPointWithinRadius(GameHelper.instance.arenaRadius);
        Vector2 playerPosition = PlayerScript.instance.transform.position;
        if ((position - playerPosition).magnitude < GameHelper.instance.arenaRadius * .33f) {
            return;
        }
        float maxSize = 4.5f + GameHelper.instance.timePassed / 30;
        int size = Mathf.RoundToInt(Random.Range(4, maxSize));
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
            zone.GetComponent<DeliveryZoneScript>().Init(AddOminoColors(coors));
            deliveryZones.Add(zone);
        }
    }
    void TrySpawnOmino() {
        if (deliveryZones.Count < maxZoneCount) {
            return;
        }
        Vector2 position = Util.GetRandomPointWithinRadius(GameHelper.instance.arenaRadius);
        int size = -1;
        float max = 8;
        while (size <= 0) {
            size = Mathf.RoundToInt(Random.Range(-max, max) + Random.Range(-max, max));
        }
        List<Vector2Int> coors = GetRandomOmino(size);
        Vector2Int dimensions = Util.GetCoorsDimensions(coors);
        float hypot = Mathf.Sqrt(dimensions.x * dimensions.x + dimensions.y * dimensions.y);
        if (PieceCanSpawnHere(position, hypot)) {
            GameObject omino = Instantiate(prefabOmino);
            omino.transform.position = position;
            omino.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            omino.GetComponent<OminoScript>().Init(AddOminoColors(coors));
        }
    }
    bool ZoneCanSpawnHere(Vector2 position, float checkRadius) {
        if (Util.IsPointOnCamera(position, checkRadius)) {
            return false;
        }
        Collider2D c2d = Physics2D.OverlapCircle(position, checkRadius * ZONE_SPAWN_DISTANCE_MULT, layerMaskZone);
        return c2d == null;
    }
    bool PieceCanSpawnHere(Vector2 position, float hypot) {
        float checkRadius = hypot * OminoScript.INTERBLOCK_DISTANCE;
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
        return omino.ToList();
    }
    List<Vector3Int> AddOminoColors(IEnumerable<Vector2Int> coors) {
        float goldChance = .5f;// Mathf.Pow(Mathf.Max(0, GameHelper.instance.timePassed) / 250, .5f) * .2f;
        return coors.Select(c => new Vector3Int(c.x, c.y, Random.value < goldChance ? 1 : 0)).ToList();
    }
}
