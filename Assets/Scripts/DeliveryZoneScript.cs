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

    Dictionary<Collider2D, OminoScript> colliders;
    int targetID;

    void Start() {
        colliders = new Dictionary<Collider2D, OminoScript>();
        Init(new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0), });
    }
    public void Init(IEnumerable<Vector2Int> coors) {
        float scale = coors.Count() * 6f / 5;
        circleCollider.radius = scale * 1.5f;
        circleObject.transform.localScale = new Vector3(scale, scale, 1);
        targetID = Util.GetCanonicalPolyominoID(coors);
        GameObject ominoExample = Instantiate(prefabOminoExample, transform);
        ominoExample.GetComponent<OminoExampleScript>().Init(coors);
        ominoExample.transform.localPosition = new Vector3(0, 0, 2.9f);
        ominoExample.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
    }

    void Update() {
        HashSet<OminoScript> ominos = new HashSet<OminoScript>(colliders.Values);
        foreach (OminoScript omino in ominos) {
            if (omino.ID == targetID && omino.ContainsAll(colliders)) {
                Destroy(omino.gameObject);
                Destroy(gameObject);
            }
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
