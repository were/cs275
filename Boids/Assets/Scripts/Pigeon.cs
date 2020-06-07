using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : MonoBehaviour
{
    public Vector3 velocity;
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
        return Quaternion.AngleAxis(angle / Mathf.PI * 180, axis) * vec;
    }

    static Vector3 calcElbow(float DOF, Vector3 neck, Vector3 shoulder, Vector3 tail, bool isLeft) {
        var axisx = Vector3.Normalize(shoulder - neck);
        var axisy = Vector3.Normalize(Vector3.Cross(shoulder - neck, tail - neck));
        if (isLeft) axisy = -axisy;
        var axisz = Vector3.Normalize(Vector3.Cross(axisx, axisy));
        if (!isLeft) axisz = Vector3.Normalize(Vector3.Cross(axisy, axisx));


        var rx = (Mathf.Cos(DOF + Mathf.PI * 1.33F) + 1.1F) / 2F;
        var ry = (Mathf.Cos(DOF + Mathf.PI * 1.33F) + 0.8F) / 2F;
        var rz = Mathf.Sin(DOF / 2) * Mathf.Sin(DOF / 4);

        var coord = (Quaternion.AngleAxis(ry, axisy) * Quaternion.AngleAxis(rx, axisx) * Quaternion.AngleAxis(rz, axisz)).ToEuler();

        coord = Vector3.Normalize(coord) * Shoulder2Elbow;

        return shoulder + coord;
    }

    static Vector3 calcWrist(float DOF, Vector3 shoulder, Vector3 tail, Vector3 elbow, bool isLeft) {
        var axisx = Vector3.Normalize(elbow - shoulder);
        var axisy = Vector3.Normalize(Vector3.Cross(shoulder, tail));
        if (!isLeft) axisy = Vector3.Normalize(Vector3.Cross(tail, shoulder));


        var rx = Mathf.Sin(DOF - Mathf.PI / 2) * Mathf.PI / 4F + Mathf.PI * 0.85F / 2;
        var ry = Mathf.Cos(DOF) * Mathf.PI / 1.8F + Mathf.PI / 2F / 2;

        var coord = (Quaternion.AngleAxis(rx, axisx) * Quaternion.AngleAxis(ry, axisy)).ToEuler();
        coord = Vector3.Normalize(coord) * Elbow2Wrist;

        return elbow + coord;
    }

    static Vector3 calcHand(float DOF, Vector3 elbow, Vector3 wrist) {
        var coord = Vector3.Normalize(wrist - elbow);
        var rx = Mathf.Sin(DOF) * Mathf.PI / 4 + Mathf.PI / 4;
        var axisy = Vector3.Normalize(Vector3.Cross(wrist, elbow));
        coord = Vector3.Normalize(Quaternion.AngleAxis(rx, coord).ToEuler()) * Wrist2Hand;
        return wrist + coord;
    }

    static Vector3 Mirror(Vector3 a, Vector3 norm) {
        return a - 2 * Vector3.ProjectOnPlane(a, norm);
    }


    void randomLeft() {

        this.tail = transform.position - velocity.normalized * Neck2Shoulder;

        this.lShoulder = transform.position + Vector3.Normalize(new Vector3(-1, 0, 1.7F)) * Neck2Shoulder / 2.0F;
        this.lElbow = this.lShoulder + Vector3.Normalize(new Vector3(-1, 0, -1)) * Shoulder2Elbow;
        this.lWrist = this.lElbow + Vector3.Normalize(new Vector3(-1.7F, 0, 1)) * Elbow2Wrist;
        this.lHand = this.lWrist + Vector3.Normalize(new Vector3(-1, 0, -1)) * Wrist2Hand;

        this.rShoulder = transform.position + Vector3.Normalize(new Vector3(1, 0, 1.7F)) * Neck2Shoulder / 2.0F;
        this.rElbow = this.rShoulder + Vector3.Normalize(new Vector3(1, 0, -1)) * Shoulder2Elbow;
        this.rWrist = this.rElbow + Vector3.Normalize(new Vector3(1.7F, 0, 1)) * Elbow2Wrist;
        this.rHand = this.rWrist + Vector3.Normalize(new Vector3(1, 0, -1)) * Wrist2Hand;

        this.lShoulder = transform.position + (Quaternion.AngleAxis(130F, transform.rotation.eulerAngles) * tail).normalized * Neck2Shoulder;
        this.rShoulder = transform.position + (Quaternion.AngleAxis(130F, -transform.rotation.eulerAngles) * tail).normalized * Neck2Shoulder;

        DOF += 0.01F;
        if (DOF > 6.28) DOF = 0F;

        lElbow = calcElbow(DOF, transform.position, lShoulder, tail, false);
        rElbow = calcElbow(DOF, transform.position, rShoulder, tail, true);

        lWrist = calcWrist(DOF, lShoulder, tail, lElbow, false);
        rWrist = calcWrist(DOF, rShoulder, tail, rElbow, true);

        lHand = calcHand(DOF, lElbow, lWrist);
        lHand = calcHand(DOF, rElbow, rWrist);

        {
            var coord = Vector3.Normalize(lWrist - lElbow);
            var rx = Mathf.Sin(DOF) * Mathf.PI / 4 + Mathf.PI / 4 + 0.01F;
            var axisy = Vector3.Normalize(Vector3.Cross(lWrist, lElbow));
            coord = Vector3.Normalize(Quaternion.AngleAxis(rx, coord).ToEuler()) * Wrist2Hand;
            this.lHand = lWrist + coord;
        }
        {
            var coord = Vector3.Normalize(rWrist - rElbow);
            var rx = Mathf.Sin(DOF) * Mathf.PI / 4 + Mathf.PI / 4 + 0.01F;
            var axisy = Vector3.Normalize(Vector3.Cross(rWrist, rElbow));
            coord = Vector3.Normalize(Quaternion.AngleAxis(rx, coord).ToEuler()) * Wrist2Hand;
            this.rHand = rWrist + coord;
        }

    }

    void calcTailFan() {
        Vector3 norm = Vector3.Normalize(Vector3.Cross(this.lShoulder - transform.position, tail - transform.position));
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

    static void buildPrimaryFeatherImpl(Vector3 hand, Vector3 wrist, Vector3 elbow, Vector3 []primary, bool shouldNeg) {
        var norm = Vector3.Cross(hand, wrist);
        norm = Vector3.Cross(norm, hand);
        var wing = Vector3.Normalize(hand - wrist);
        for (int i = 0; i < 10; ++i) {
             primary[i] = hand + (wrist - hand) * i * 0.1F + wing * (-0.5F * i + 16F);
             wing = bodyRotation(wing, norm, Mathf.PI / 18 * (shouldNeg ? -1 : 1));
        }
    }
    public void buildFeather() {
        buildPrimaryFeatherImpl(lHand, lWrist, lElbow, lPrimaryFeather, false);
        buildPrimaryFeatherImpl(rHand, rWrist, lElbow, rPrimaryFeather, true);
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

        velocity = new Vector3(Mathf.Cos(DOF), Mathf.Cos(DOF) * Mathf.Sin(DOF), Mathf.Sin(DOF)) * 2F;

        transform.Translate(velocity);

        //transform.Translate(new Vector3(0, 0, 0.01F));
    }

    static float Neck2Shoulder = 14;
    static float Shoulder2Elbow = 2;
    static float Elbow2Wrist = 5;
    static float Wrist2Hand = 2;
    static float TailLength = 10;
    static float SecondaryFeather = 11;
}
