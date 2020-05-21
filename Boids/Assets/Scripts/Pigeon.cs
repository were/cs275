using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : MonoBehaviour
{
    Vector3 lShoulder, lElbow, lWrist, lHand;
    Vector3 rShoulder, rElbow, rWrist, rHand;
    Vector3 tail;

    Vector3 []tailFan = new Vector3[12];

    float tailSpread = 0;

    void randomLeft() {
        this.tail = transform.position + Neck2Shoulder * Random.onUnitSphere;
        this.lShoulder = transform.position + Neck2Shoulder * Random.onUnitSphere;
        Vector3 norm = Vector3.Normalize(Vector3.Cross(this.lShoulder, tail));
        this.lElbow = this.lShoulder + Vector3.Normalize(Vector3.ProjectOnPlane(Random.onUnitSphere, norm));
        this.lWrist = this.lElbow + Elbow2Wrist * Vector3.Normalize(Vector3.ProjectOnPlane(Random.onUnitSphere, norm));
        this.lHand = this.lWrist + Wrist2Hand * Vector3.Normalize(Vector3.ProjectOnPlane(Random.onUnitSphere, norm));
    }

    void calcTailFan() {
        Vector3 norm = Vector3.Normalize(Vector3.Cross(this.lShoulder, tail));
        Vector3 x = Vector3.Normalize(Vector3.Cross(tail, norm));
        Vector3 y = Vector3.Normalize(tail - transform.position);
        float angle = (Mathf.PI / 3.0F) * (1F + Mathf.Cos(tailSpread) / 10.0F);
        float delta = (Mathf.PI - angle * 2) / 12F;
        for (int i = 0; i < 12; ++i) {
            tailFan[i] = tail + (x * Mathf.Cos(angle) + y * Mathf.Sin(angle)) * TailLength;
            angle += delta;
        }
        tailSpread += 0.005F;
    }

    void reflectRight() {
        var norm = Vector3.Normalize(tail - transform.position - tail);
        rShoulder = lShoulder - 2 * Vector3.ProjectOnPlane(lShoulder, norm);
        rElbow = lElbow - 2 * Vector3.ProjectOnPlane(lElbow, norm);
        rWrist = lWrist - 2 * Vector3.ProjectOnPlane(lWrist, norm);
        rHand = lHand - 2 * Vector3.ProjectOnPlane(lHand, norm);
    }

    public void Start() {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<LineRenderer>();
        randomLeft();
        reflectRight();
        calcTailFan();
    }

    public void Update() {
        //randomLeft();
        reflectRight();
        //calcTailFan();
        var mesh = GetComponent<MeshFilter>().mesh;
        var lr = gameObject.GetComponent<LineRenderer>();
        mesh.Clear();
        //lr.startWidth = 1.0f;
        //lr.endWidth = 1.0f;
        //lr.positionCount = 11;
        //lr.SetPositions(new Vector3[]{lHand, lWrist, lElbow, lShoulder, transform.position, tail,
        //                              transform.position, rShoulder, rElbow, rWrist, rHand});
        List<Vector3> a = new List<Vector3>();
        lr.positionCount = 6;
        a.Add(transform.position);
        a.Add(tail);
        for (int i = 0; i < 12; ++i) {
            a.Add(tailFan[i]);
            a.Add(tail);
        }
        a.Add(transform.position);
        a.Add(lShoulder);
        a.Add(lElbow);
        a.Add(lWrist);
        a.Add(lHand);
        a.Add(lWrist);
        a.Add(lElbow);
        a.Add(lShoulder);
        a.Add(transform.position);
        a.Add(rShoulder);
        a.Add(rElbow);
        a.Add(rWrist);
        a.Add(rHand);

        lr.positionCount = a.Count;
        lr.SetPositions(a.ToArray());
    }

    static float Neck2Shoulder = 14;
    static float Shoulder2Elbow = 2;
    static float Elbow2Wrist = 5;
    static float Wrist2Hand = 2;
    static float TailLength = 10;
}
