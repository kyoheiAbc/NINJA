using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    Camera cam;
    Controller controller;
    int frame; public int getFrame() { return this.frame / 12; }
    static public Main instance;
    List<Ninja> list;
    Ninja player; public Ninja getPlayer() { return this.player; }
    int stage;
    void Awake()
    {
        this.gameObject.transform.position = new Vector3(0, 8, -8 * Mathf.Sqrt(3));
        this.gameObject.transform.eulerAngles = new Vector3(30, 0, 0);

        this.cam = this.gameObject.AddComponent<Camera>();
        this.cam.clearFlags = CameraClearFlags.SolidColor;
        this.cam.orthographic = true;
        this.cam.backgroundColor = Color.HSVToRGB(120 / 360f, 0.3f, 0.5f);

        this.gameObject.AddComponent<Light>().type = LightType.Directional;

        this.controller = new Controller();
        this.list = new List<Ninja>();

        Main.instance = this;

        Application.targetFrameRate = 30;
    }

    void Start()
    {
        this.reset();
    }

    private void reset()
    {
        this.frame = 0;

        for (int i = 0; i < this.list.Count; i++) Destroy(this.list[i].renderer.getGameObject());
        this.list.Clear();

        this.player = null;

        this.stage = 0;

        for (int i = 0; i < 4; i++)
        {
            Ninja n = new Ninja(i);
            n.setPos(new Vector3(2 * i, 0, 0));
            n.setRot(Quaternion.identity * Quaternion.AngleAxis(180, new Vector3(0, 1, 0)));
            n.setAi(null);
            this.list.Add(n);
        }
    }

    void Update()
    {
        this.frame += 12;
        if (this.frame == 12 * 60 * 3) this.frame = 0;

        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i].getHp() < -60)
            {
                Destroy(this.list[i].renderer.getGameObject());
                this.list.RemoveAt(i);
                i--;
                continue;
            }
            this.list[i].update();
        }

        control();
        void control()
        {
            this.controller.update();

            if (this.player == null)
            {
                Vector3 gtpb = this.cam.ScreenToWorldPoint(this.controller.getTouchPhaseBegan());
                if (gtpb == Vector3.zero) return;
                Quaternion r = Quaternion.Euler(30, 0, 0);
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (Main.DoesLineIntersectSphere(gtpb, gtpb + Main.forward(r).normalized * 50, this.list[i].getPos() + Vector3.up, 0.499f))
                    {
                        Debug.Log(this.list[i].getPos());
                        break;
                    }
                }
                return;
            }
            if (this.player.getHp() < 0 || this.player.getStun() > 0) return;

            Vector3 s = this.controller.getStick().normalized;
            this.player.mv(s * 0.1f);
            int b = this.controller.getButton();
            if (b == 1) this.player.attack.exe();
            if (b == 4) this.player.jump(this.controller.getDeltaPosition().normalized);
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].renderer.update();
        }

    }


    public List<Ninja> getList(Ninja n, float d, float a)
    {
        List<Ninja> ret = new List<Ninja>();
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i] == n) continue;
            if ((this.list[i].getPos() - n.getPos()).sqrMagnitude > d) continue;
            if (Vector3.Angle(Main.forward(n.getRot()), this.list[i].getPos() - n.getPos()) > a) continue;
            ret.Add(this.list[i]);
        }
        return ret;
    }

    public List<Ninja> getList(Vector3 p, Quaternion r, Vector3 s)
    {
        List<Ninja> ret = new List<Ninja>();
        for (int i = 0; i < this.list.Count; i++)
        {
            if (!Main.inCube(this.list[i].getPos(), p, r, s)) continue;
            ret.Add(this.list[i]);
        }
        return ret;
    }

    private static bool inCube(Vector3 p_, Vector3 p, Quaternion r, Vector3 s)
    {
        p_ = Quaternion.Inverse(r) * (p_ - p);
        p_.Scale(new Vector3(1 / s.x, 1 / s.y, 1 / s.z));
        return (Mathf.Abs(p_.x) <= 0.5f && Mathf.Abs(p_.y) <= 0.5f && Mathf.Abs(p_.z) <= 0.5f);
    }

    public Ninja nearestNinja(Ninja n)
    {
        Ninja ret = n;
        float min = float.MaxValue;
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i] == n) continue;
            if ((this.list[i].getPos() - n.getPos()).sqrMagnitude < min)
            {
                min = (this.list[i].getPos() - n.getPos()).sqrMagnitude;
                ret = this.list[i];
            }
        }
        return ret;
    }
    private static bool DoesLineIntersectSphere(Vector3 lineStart, Vector3 lineEnd, Vector3 sphereCenter, float sphereRadius)
    {
        Vector3 closestPoint = ClosestPointOnLine(lineStart, lineEnd, sphereCenter);
        return (closestPoint - sphereCenter).sqrMagnitude < sphereRadius * sphereRadius;
    }

    private static Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        float projectMagnitude = Vector3.Dot(point - lineStart, lineDirection);
        projectMagnitude = Mathf.Clamp(projectMagnitude, 0f, lineLength);

        Vector3 closestPoint = lineStart + lineDirection * projectMagnitude;
        return closestPoint;
    }

    static public Vector3 forward(Quaternion q) { return (q * Vector3.forward).normalized; }
}