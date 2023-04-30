using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneIndicatorScript : MonoBehaviour
{
    static Vector2 MARGIN = new Vector2(100, 100);

    public GameObject prefabOminoExample;

    public CanvasGroup canvasGroup;
    public RectTransform rt;

    RectTransform rtCanvas;
    Camera cam;
    GameObject deliveryZone;
    float radius;
    float vAlpha;

    public void Init(DeliveryZoneScript deliveryZoneScript) {
        canvasGroup.alpha = 0;
        rtCanvas = transform.parent.GetComponent<RectTransform>();
        cam = Camera.main;
        deliveryZone = deliveryZoneScript.gameObject;
        IEnumerable<Vector2Int> coors = deliveryZoneScript.coorsAndColors.Select(c => new Vector2Int(c.x, c.y));
        Vector2Int dimensions = Util.GetCoorsDimensions(coors);
        radius = Mathf.Sqrt(dimensions.x * dimensions.x + dimensions.y * dimensions.y) * 3f;
        Instantiate(prefabOminoExample, rt).GetComponent<OminoExampleScript>().Init(deliveryZoneScript.coorsAndColors, true);
    }
    
    void Update() {
        if (deliveryZone == null) {
            Destroy(gameObject);
            return;
        }
        Vector2 zonePos = deliveryZone.transform.position;
        Vector2 camPos = cam.transform.position;
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, Util.IsPointOnCamera(zonePos, radius) ? 0 : 1, ref vAlpha, .1f);

        Vector2 normalizedDelta = (zonePos - camPos).normalized;
        Vector2 cornerVector = (rtCanvas.sizeDelta / 2) - MARGIN;
        float xScale = Mathf.Abs(cornerVector.x / normalizedDelta.x);
        float yScale = Mathf.Abs(cornerVector.y / normalizedDelta.y);
        float scaleFactor = Mathf.Min(xScale, yScale);
        rt.anchoredPosition = normalizedDelta * scaleFactor;
        rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(normalizedDelta.y, normalizedDelta.x) * Mathf.Rad2Deg);
    }
}
