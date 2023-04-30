using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OminoExampleScript : MonoBehaviour
{
    public GameObject prefabBlockExample, prefabBlockExampleImage;

    SpriteRenderer[] renderers;

    public void Init(IEnumerable<Vector3Int> coorsAndColors, bool canvas = false) {
        IEnumerable<Vector2Int> coors = coorsAndColors.Select(c => new Vector2Int(c.x, c.y));
        int minX = coors.Min(c => c.x);
        int maxX = coors.Max(c => c.x);
        int minY = coors.Min(c => c.y);
        int maxY = coors.Max(c => c.y);
        float cx = (maxX + minX) / 2f;
        float cy = (maxY + minY) / 2f;
        foreach (Vector3Int c in coorsAndColors) {
            GameObject block = Instantiate(canvas ? prefabBlockExampleImage : prefabBlockExample, transform);
            block.transform.localPosition = new Vector3((c.x - cx) * OminoScript.INTERBLOCK_DISTANCE,
                                                        (c.y - cy) * OminoScript.INTERBLOCK_DISTANCE);
            if (canvas) {
                Image image = block.GetComponent<Image>();
                image.color = Util.HueshiftBlockColor(image.color, c.z);
            } else {
                SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
                sr.color = Util.HueshiftBlockColor(sr.color, c.z);
            }
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
