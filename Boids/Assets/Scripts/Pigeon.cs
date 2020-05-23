using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : MonoBehaviour
{
    public Material material;
    Vector3 lShoulder, lElbow, lWrist, lHand;
    Vector3[][] lSecondaryFeather = new Vector3[8][];
    Vector3[] lPrimaryFeather = new Vector3[10];
    Vector3 rShoulder, rElbow, rWrist, rHand;
    Vector3[][] rSecondaryFeather = new Vector3[8][];
    Vector3[] rPrimaryFeather = new Vector3[10];
    Vector3 tail;

    Vector3 []tailFan = new Vector3[12];

    float DOF = 0;
    float tailSpread = 0;

    static Vector3 bodyRotation(Vector3 vec, Vector3 axis, float angle) {
        return Quaternion.AngleAxis(angle, axis) * vec;
    }

    void randomLeft() {
        this.tail = transform.position - new Vector3(0, Neck2Shoulder, 0);
        this.lShoulder = transform.position + Vector3.Normalize(new Vector3(-1, 1.7F, 0)) * Neck2Shoulder / 2.0F;
        this.rShoulder = transform.position + Vector3.Normalize(new Vector3(1, 1.7F, 0)) * Neck2Shoulder / 2.0F;

        this.lElbow = this.lShoulder + Vector3.Normalize(new Vector3(-1, -1, 0)) * Shoulder2Elbow;
        this.lWrist = this.lElbow + Vector3.Normalize(new Vector3(-1.7F, 1, 0)) * Elbow2Wrist;
        this.lHand = this.lWrist + Vector3.Normalize(new Vector3(-1, -1, 0)) * Wrist2Hand;

        DOF += 0.01F;
        if (DOF > 6.28) DOF = 0F;

        // TODO(@were): I am not sure this rotation is good.
        {
            var ry = (Mathf.Cos(DOF + Mathf.PI * 1.33F) + 0.8F) / 2F;
            var rx = (Mathf.Cos(DOF + Mathf.PI * 1.33F) + 1.1F) / 2F;
            var rz = (Mathf.Sin(DOF / 2) * Mathf.Sin(DOF / 4));
            var rot = Quaternion.Euler(rx / Mathf.PI * 180, ry / Mathf.PI * 180, rz / Mathf.PI * 180);
            var dir = rot * Vector3.Normalize(new Vector3(-1, -1, 0));
            this.lElbow = this.lShoulder +  dir * Shoulder2Elbow;
        }
        {
            var rx = Mathf.Sin(DOF - Mathf.PI / 2) * Mathf.PI / 4F + Mathf.PI * 0.85F;
            var ry = Mathf.Cos(DOF) * Mathf.PI / 1.8F + Mathf.PI / 2F;
            rx = rx / Mathf.PI * 180 / 2;
            ry = ry / Mathf.PI * 180 / 2;
            var rot = Quaternion.Euler(rx, 0, ry);
            var dir = rot * Vector3.Normalize(lElbow - lShoulder);
            this.lWrist = lElbow + dir * Elbow2Wrist;
        }
        {
            var rx = Mathf.Sin(DOF - Mathf.PI / 2) * Mathf.PI / 4F;
            rx = rx / Mathf.PI * 180;
            var rot = Quaternion.Euler(0, rx, 0);
            var dir = rot * Vector3.Normalize(lWrist - lElbow);
            this.lHand = lWrist + dir * Wrist2Hand;
        }
        //this.lWrist = this.lElbow + Vector3.Normalize(lElbow - lShoulder) * Elbow2Wrist;
        //this.lHand = this.lWrist + Vector3.Normalize(lWrist - lElbow) * Wrist2Hand;

        this.rShoulder = transform.position + Vector3.Normalize(new Vector3(1, 1.7F, 0)) * Neck2Shoulder / 2.0F;
        this.rElbow = this.rShoulder + Vector3.Normalize(new Vector3(1, -1, 0)) * Shoulder2Elbow;
        this.rWrist = this.rElbow + Vector3.Normalize(new Vector3(1.7F, 1, 0)) * Elbow2Wrist;
        this.rHand = this.rWrist + Vector3.Normalize(new Vector3(1, -1, 0)) * Wrist2Hand;

        //this.tail = transform.position + Neck2Shoulder * Random.onUnitSphere;
        //this.lShoulder = transform.position + Neck2Shoulder * Random.onUnitSphere;
        //Vector3 norm = Vector3.Normalize(Vector3.Cross(this.lShoulder, tail));
        //this.lElbow = this.lShoulder + Shoulder2Elbow * Vector3.Normalize(Vector3.ProjectOnPlane(Random.onUnitSphere, norm));
        //this.lWrist = this.lElbow + Elbow2Wrist * Vector3.Normalize(Vector3.ProjectOnPlane(Random.onUnitSphere, norm));
        //this.lHand = this.lWrist + Wrist2Hand * Vector3.Normalize(Vector3.ProjectOnPlane(Random.onUnitSphere, norm));
    }

    void calcMovement() {
        {
            var dir = lElbow - lShoulder;

        }
    }

    void calcTailFan() {
        Vector3 norm = Vector3.Normalize(Vector3.Cross(this.lShoulder, tail));
        Vector3 x = Vector3.Normalize(Vector3.Cross(tail, norm));
        Vector3 y = Vector3.Normalize(tail - transform.position);
        float angle = (Mathf.PI / 3.0F) * (1F + Mathf.Cos(tailSpread) / 10.0F);
        float delta = (Mathf.PI - angle * 2) / 12F;
        for (int i = 0; i < 12; ++i) {
            tailFan[i] = tail + Vector3.Normalize(x * Mathf.Cos(angle) + y * Mathf.Sin(angle) + norm * Mathf.Sin(tailSpread) / 5) * TailLength;
            angle += delta;
        }
        tailSpread += 0.005F;
    }

    void reflectRight() {
        //var norm = Vector3.Normalize(tail - transform.position);
        //rShoulder = lShoulder - 2 * Vector3.ProjectOnPlane(lShoulder, norm);
        //rElbow = lElbow - 2 * Vector3.ProjectOnPlane(lElbow, norm);
        //rWrist = lWrist - 2 * Vector3.ProjectOnPlane(lWrist, norm);
        //rHand = lHand - 2 * Vector3.ProjectOnPlane(lHand, norm);
    }

    private void buildSecondaryFeatherImpl(Vector3 elbow, Vector3 wrist, Vector3[][] feather) {
        var dy = Vector3.Normalize(tail - transform.position);
        var armDir = wrist - elbow;
        for (int i = 0; i < 8; ++i) {
            feather[i][0] = elbow + armDir / 8F * i;
            feather[i][1] = feather[i][0] + Vector3.Normalize(dy + armDir / 5 * Mathf.Sin(Mathf.PI / 20 * (i - 3.5F))) * SecondaryFeather;
        }
    }

    private void buildPrimaryFeatherImpl(Vector3 hand, Vector3 wrist, Vector3 []primary) {
        var norm = Vector3.Cross(hand, wrist);
        var dy = Vector3.Normalize(tail - transform.position);
        dy = Vector3.Normalize(Vector3.Cross(norm, hand - wrist));
        var dx = Vector3.Normalize(hand - wrist);
        for (int i = 0; i < 10; ++i) {
             primary[i] = wrist + (dx / 10F * i) + Vector3.Normalize(dy * Mathf.Sin(Mathf.PI / 20 * i) + dx * Mathf.Cos(Mathf.PI / 20 * i)) * (0.5F * i + 11F);
        }
    }
    public void buildFeather() {
        buildPrimaryFeatherImpl(lHand, lWrist, lPrimaryFeather);
        buildPrimaryFeatherImpl(rHand, rWrist, rPrimaryFeather);
        buildSecondaryFeatherImpl(lElbow, lWrist, lSecondaryFeather);
        buildSecondaryFeatherImpl(rElbow, rWrist, rSecondaryFeather);
        calcTailFan();
    }

    public void Start() {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<LineRenderer>();
        for (int i = 0; i < 8; ++i) {
            lSecondaryFeather[i] = new Vector3[2];
            rSecondaryFeather[i] = new Vector3[2];
        }
        randomLeft();
        reflectRight();
        buildFeather();
        buildFeather();
    }

    public void Update() {
        randomLeft();
        reflectRight();
        buildFeather();
        var mesh = GetComponent<MeshFilter>().mesh;
        var lr = gameObject.GetComponent<LineRenderer>();
        mesh.Clear();

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
        for (int i = 0; i < 8; ++i) {
            a.Add(lSecondaryFeather[i][0]);
            a.Add(lSecondaryFeather[i][1]);
            a.Add(lSecondaryFeather[i][0]);
        }
        a.Add(lElbow);
        a.Add(lWrist);
        for (int i = 0; i < 10; ++i) {
            a.Add(lPrimaryFeather[i]);
            a.Add(lWrist);
        }
        a.Add(lHand);
        a.Add(lWrist);
        a.Add(lElbow);
        a.Add(lShoulder);
        a.Add(transform.position);
        a.Add(rShoulder);
        for (int i = 1; i < 8; ++i) {
            a.Add(rSecondaryFeather[i][0]);
            a.Add(rSecondaryFeather[i][1]);
            a.Add(rSecondaryFeather[i][0]);
        }
        a.Add(rElbow);
        a.Add(rWrist);
        for (int i = 0; i < 10; ++i) {
            a.Add(rPrimaryFeather[i]);
            a.Add(rWrist);
        }
        a.Add(rHand);

        lr.material = material;
        lr.positionCount = a.Count;
        lr.SetPositions(a.ToArray());

    }

    static float Neck2Shoulder = 14;
    static float Shoulder2Elbow = 2;
    static float Elbow2Wrist = 5;
    static float Wrist2Hand = 2;
    static float TailLength = 10;
    static float SecondaryFeather = 11;
}
