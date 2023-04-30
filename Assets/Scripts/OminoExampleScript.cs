using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OminoExampleScript : MonoBehaviour
{
    public GameObject prefabBlockExample, prefabBlockExampleImage;

    SpriteRenderer[] renderers;

    public void Init(IEnumerable<Vector2Int> coors, bool canvas = false) {
        int minX = coors.Min(c => c.x);
        int maxX = coors.Max(c => c.x);
        int minY = coors.Min(c => c.y);
        int maxY = coors.Max(c => c.y);
        float cx = (maxX + minX) / 2f;
        float cy = (maxY + minY) / 2f;
        foreach (Vector2Int coor in coors) {
            GameObject block = Instantiate(canvas ? prefabBlockExampleImage : prefabBlockExample, transform);
            block.transform.localPosition = new Vector3((coor.x - cx) * OminoScript.INTERBLOCK_DISTANCE,
                                                        (coor.y - cy) * OminoScript.INTERBLOCK_DISTANCE);
        }
        if (canvas) {
            Vector2Int dimensions = Util.GetCoorsDimensions(coors);
            float hypot = Mathf.Sqrt(dimensions.x * dimensions.x + dimensions.y * dimensions.y);
            float scale = 10 / hypot;
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }

    public void DecrementAlpha(float f) {
        if (renderers == null) {
            renderers = GetComponentsInChildren<SpriteRenderer>();
        }
        foreach (SpriteRenderer sr in renderers) {
            Color c = sr.color;
            c.a -= f;
            sr.color = c;
        }
    }
}
