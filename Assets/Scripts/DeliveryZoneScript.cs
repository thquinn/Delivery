using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryZoneScript : MonoBehaviour
{
    public GameObject prefabOminoExample, prefabZoneIndicator, prefabOminoDisappearVFX;

    public CircleCollider2D circleCollider;
    public SpriteRenderer circleRenderer;
    public Transform orbitalsTransform;

    public IEnumerable<Vector3Int> coorsAndColors;
    Dictionary<Collider2D, OminoScript> colliders;
    List<Transform> orbitalTransforms;
    int targetID;
    bool destroying;
    OminoExampleScript ominoExample;
    List<OminoExampleScript> orbitalOminos;

    void Start() {
        colliders = new Dictionary<Collider2D, OminoScript>();
    }
    public void Init(IEnumerable<Vector3Int> coorsAndColors) {
        this.coorsAndColors = coorsAndColors;
        IEnumerable<Vector2Int> coors = coorsAndColors.Select(c => new Vector2Int(c.x, c.y));
        Vector2Int dimensions = Util.GetCoorsDimensions(coors);
        float hypot = Mathf.Sqrt(dimensions.x * dimensions.x + dimensions.y * dimensions.y);
        float scale = hypot * 6f / 5;
        circleCollider.radius = scale * 1.5f;
        circleRenderer.transform.localScale = new Vector3(scale, scale, 1);
        targetID = Util.GetCanonicalPolyominoID(coorsAndColors);
        ominoExample = Instantiate(prefabOminoExample, transform).GetComponent<OminoExampleScript>();
        ominoExample.Init(coorsAndColors);
        ominoExample.transform.localPosition = new Vector3(0, 0, 2.9f);
        ominoExample.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        // Indicator.
        RectTransform rtCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        Instantiate(prefabZoneIndicator, rtCanvas).GetComponent<ZoneIndicatorScript>().Init(this);
        // Orbitals.
        orbitalOminos = new List<OminoExampleScript>();
        GameObject orbital = Instantiate(ominoExample.gameObject, orbitalsTransform);
        orbital.transform.localScale = new Vector3(1 / hypot, 1 / hypot, 1);
        foreach (SpriteRenderer sr in orbital.GetComponentsInChildren<SpriteRenderer>()) {
            sr.color = Util.OrbitalColor(sr.color);
        }
        float distance = scale * 2 + 7;
        int orbitalCount = Mathf.RoundToInt(scale);
        orbitalTransforms = new List<Transform>(orbitalCount);
        for (int i = 0; i < orbitalCount; i++) {
            orbitalOminos.Add(orbital.GetComponent<OminoExampleScript>());
            float angle = Mathf.PI * 2 / orbitalCount * i;
            orbital.transform.localPosition = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
            orbital.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            orbitalTransforms.Add(orbital.transform);
            if (i != orbitalCount - 1) {
                orbital = Instantiate(orbital, orbitalsTransform);
            }
        }
    }

    void Update() {
        if (!destroying) {
            HashSet<OminoScript> ominos = new HashSet<OminoScript>(colliders.Values);
            foreach (OminoScript omino in ominos) {
                if (omino.ID == targetID && omino.ContainsAll(colliders)) {
                    GameHelper.instance.Deliver(omino);
                    Destroy(omino.gameObject);
                    Instantiate(prefabOminoDisappearVFX).GetComponent<OminoDisappearVFXScript>().Init(omino);
                    SFXHelper.instance.ZoneClear();
                    destroying = true;
                    break;
                }
            }
        }
        // Orbitals.
        orbitalsTransform.Rotate(0, 0, Time.deltaTime * 20);
        /*
        foreach (Transform t in orbitalTransforms) {
            t.rotation = Quaternion.identity;
        }
        */
        if (destroying) {
            circleRenderer.transform.localScale -= new Vector3(Time.deltaTime, Time.deltaTime, 0);
            Color c = circleRenderer.color;
            c.a -= Time.deltaTime * 2f;
            if (c.a <= 0) {
                Destroy(gameObject);
                return;
            }
            circleRenderer.color = c;
            ominoExample.DecrementAlpha(Time.deltaTime * 2f);
            orbitalsTransform.localScale += new Vector3(Time.deltaTime * .5f, Time.deltaTime * .5f, 0);
            foreach (OminoExampleScript orbital in orbitalOminos) {
                orbital.DecrementAlpha(Time.deltaTime * 2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (destroying) { return; }
        BlockScript blockScript = collision.GetComponent<BlockScript>();
        OminoScript ominoScript = blockScript.omino;
        colliders[collision] = ominoScript;
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (destroying) { return; }
        colliders.Remove(collision);
    }
}
