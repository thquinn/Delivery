using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OminoExampleScript : MonoBehaviour
{
    public GameObject prefabBlockExample;

    public void Init(IEnumerable<Vector2Int> coors) {
        int minX = coors.Min(c => c.x);
        int maxX = coors.Max(c => c.x);
        int minY = coors.Min(c => c.y);
        int maxY = coors.Max(c => c.y);
        float cx = (maxX - minX) / 2f;
        float cy = (maxY - minY) / 2f;
        foreach (Vector2Int coor in coors) {
            GameObject block = Instantiate(prefabBlockExample, transform);
            block.transform.localPosition = new Vector3((coor.x - cx) * OminoScript.INTERBLOCK_DISTANCE,
                                                        (coor.y - cy) * OminoScript.INTERBLOCK_DISTANCE);
        }
    }
}
