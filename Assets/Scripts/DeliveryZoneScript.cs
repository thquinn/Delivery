using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryZoneScript : MonoBehaviour
{
    public GameObject prefabOminoExample;

    public CircleCollider2D circleCollider;
    public GameObject circleObject;
    public Transform orbitalsTransform;
    public Color orbitalColor;

    Dictionary<Collider2D, OminoScript> colliders;
    List<Transform> orbitalTransforms;
    int targetID;

    void Start() {
        colliders = new Dictionary<Collider2D, OminoScript>();
    }
    public void Init(IEnumerable<Vector2Int> coors) {
        Vector2Int dimensions = Util.GetCoorsDimensions(coors);
        float hypot = Mathf.Sqrt(dimensions.x * dimensions.x + dimensions.y * dimensions.y);
        float scale = hypot * 6f / 5;
        circleCollider.radius = scale * 1.5f;
        circleObject.transform.localScale = new Vector3(scale, scale, 1);
        targetID = Util.GetCanonicalPolyominoID(coors);
        GameObject ominoExample = Instantiate(prefabOminoExample, transform);
        ominoExample.GetComponent<OminoExampleScript>().Init(coors);
        ominoExample.transform.localPosition = new Vector3(0, 0, 2.9f);
        ominoExample.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        // Orbitals.
        GameObject orbital = Instantiate(ominoExample, orbitalsTransform);
        orbital.transform.localScale = new Vector3(1 / hypot, 1 / hypot, 1);
        foreach (SpriteRenderer sr in orbital.GetComponentsInChildren<SpriteRenderer>()) {
            sr.color = orbitalColor;
        }
        float distance = scale * 2 + 7;
        int orbitalCount = Mathf.RoundToInt(scale);
        orbitalTransforms = new List<Transform>(orbitalCount);
        for (int i = 0; i < orbitalCount; i++) {
            float angle = Mathf.PI * 2 / orbitalCount * i;
            orbital.transform.localPosition = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
            orbitalTransforms.Add(orbital.transform);
            if (i != orbitalCount - 1) {
                orbital = Instantiate(orbital, orbitalsTransform);
            }
        }
    }

    void Update() {
        HashSet<OminoScript> ominos = new HashSet<OminoScript>(colliders.Values);
        foreach (OminoScript omino in ominos) {
            if (omino.ID == targetID && omino.ContainsAll(colliders)) {
                Destroy(omino.gameObject);
                Destroy(gameObject);
            }
        }
        // Orbitals.
        orbitalsTransform.Rotate(0, 0, Time.deltaTime * 20);
        foreach (Transform t in orbitalTransforms) {
            t.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        BlockScript blockScript = collision.GetComponent<BlockScript>();
        OminoScript ominoScript = blockScript.omino;
        colliders[collision] = ominoScript;
    }
    private void OnTriggerExit2D(Collider2D collision) {
        colliders.Remove(collision);
    }
}
