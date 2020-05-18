using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : MonoBehaviour
{
    public Transform boidPrefab;
    public int swarmCount;
    private int maxDistance = 45;
    // Start is called before the first frame update
    private Vector3 origin;
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        var mf = GetComponent<MeshFilter>().mesh;
        mf.Clear();
        mf.vertices = new Vector3[] {new Vector3(0,0,0), new Vector3(0,8,0), new Vector3(8, 8, 0)};
        mf.uv = new Vector2[] {new Vector2(0, 0), new Vector2 (0, 0), new Vector2 (0, 0)};
        mf.triangles = new int[] {0, 1, 2};
    }

    // Update is called once per frame
    void Update()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] {new Vector3(0,0,0), new Vector3(0,8,0), new Vector3(8, 8, 0)};
        mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2 (0, 0), new Vector2 (0, 0)};
        mesh.triangles = new int[] {0, 1, 2};
        gameObject.AddComponent<LineRenderer>();
        var lr = gameObject.GetComponent<LineRenderer>();
        lr.startColor = Color.red;
        lr.endColor = Color.blue;
        lr.startWidth = 1.0f;
        lr.endWidth = 2.0f;
        lr.SetPosition(0, Random.insideUnitSphere * 15);
        lr.SetPosition(1, Random.onUnitSphere * 15f);
    }

}
