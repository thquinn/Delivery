using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public static SpawnerScript instance;

    public GameObject prefabOmino;

    void Start() {
        instance = this;
        Instantiate(prefabOmino).GetComponent<OminoScript>().Init(new Vector2Int[] {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(0, -1),
            new Vector2Int(0, -2),
        });
    }

    public OminoScript SpawnEmpty() {
        return Instantiate(prefabOmino).GetComponent<OminoScript>();
    }
}
