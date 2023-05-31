using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public RectTransform[] button;
    public Camera cam;
    Controller controller;
    int fieldSize; public int getFieldSize() { return this.fieldSize; }
    int frame; public int getFrame() { return this.frame / 12; }
    static public Main instance;
    List<Ninja> list;
    Ninja player; public Ninja getPlayer() { return this.player; }
    int stage;
    Texture[] texList;

    void Awake()
    {
        this.cam = this.gameObject.AddComponent<Camera>();
        this.cam.clearFlags = CameraClearFlags.SolidColor;

        Light l = this.gameObject.AddComponent<Light>();
        l.type = LightType.Directional;

        this.texture();

        Main.instance = this;

        Application.targetFrameRate = 30;
    }

    void Start()
    {
        this.controller = new Controller();
        this.list = new List<Ninja>();
        this.reset();
    }

    private void reset()
    {
        this.fieldSize = int.MaxValue;

        this.frame = 0;

        for (int i = 0; i < this.list.Count; i++) Destroy(this.list[i].renderer.getGameObject());
        this.list.Clear();

        this.player = null;

        this.stage = 0;

        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.transform.localRotation = Quaternion.identity;
        this.cam.fieldOfView = 1;
        float f = 0;
        for (int y = 0; y < 1; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                f++;
                Ninja n = newNinja(x + y);
                n.setPos(new Vector3(100 * (x + y), 0, 0));
                n.setRot(Quaternion.identity);
                n.setAi(null);
                n.renderer.setWalkEn(true);
                this.list.Add(n);
                n.renderer.getGameObject().transform.GetChild(0).transform.localPosition = Vector3.zero;
                n.renderer.getGameObject().transform.GetChild(0).transform.localPosition = -n.getPos() + new Vector3(2 * x, y, 0);
                n.renderer.getGameObject().transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 30, 0);
                this.gameObject.transform.position += new Vector3(2 * x, y, 0);
            }
        }
        this.gameObject.transform.position /= f;
        this.gameObject.transform.position += new Vector3(0, 1, -400);
        this.cam.backgroundColor = Color.HSVToRGB(1 / 6f, 0.5f, 0.8f);

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
            if (this.player == null) return;
            if (this.cam.fieldOfView == 1) return;
            if (this.player.getHp() < 0 || this.player.getStun() > 0) return;
            if (this.player.special.getI() != 0) return;

            Vector3 s = this.controller.getStick().normalized;
            this.player.mv(s * 0.1f);
            if (s != Vector3.zero) this.player.setRot(Quaternion.LookRotation(s));
            int b = this.controller.getButton();
            if (b == 1) this.player.attack.exe();
            if (b == 2) this.player.special.exe();
            if (b == 4) this.player.jump(this.controller.getDeltaPosition().normalized);
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].renderer.update();
        }

        if (this.player == null) this.charSel();
        else stageSel();
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
    private void charSel()
    {
        Vector2 t = this.controller.getTouchPhaseBegan();
        if (t != new Vector2(0, 0))
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                Vector2 n = (Vector2)this.list[i].renderer.getGameObject().transform.GetChild(0).transform.position + new Vector2(0, 1);
                if ((t - n).sqrMagnitude > 0.75f * 0.75f) continue;
                this.list[i].attack.exe();
                break;
            }
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i].attack.getI() != 301) continue;
            this.list[i].addVec(0.75f * Vector3.up);
            this.player = list[i];
        }
    }

    private void stageSel()
    {
        if (this.cam.fieldOfView == 1 || this.player.getHp() < -60 || this.list.Count == 1) this.stage++;

        if (this.stage % 100 < 30) return;

        if (this.cam.fieldOfView == 1)
        {
            this.player.renderer.getGameObject().transform.GetChild(0).transform.localPosition = Vector3.zero;
            this.player.renderer.getGameObject().transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 180, 0);

            this.player.setPos(new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10)));
            this.player.renderer.setWalkEn(false);
            this.player.renderer.update();

            this.gameObject.transform.position = new Vector3(0, 22.5f, -40);
            this.gameObject.transform.eulerAngles = new Vector3(30, 0, 0);
            this.cam.fieldOfView = 15;
            this.cam.backgroundColor = Color.HSVToRGB(120 / 360f, 0.3f, 0.5f);

            for (int i_ = 0; i_ < this.list.Count; i_++)
            {
                if (this.list[i_] == this.player) continue;
                Destroy(this.list[i_].renderer.getGameObject());
                this.list.RemoveAt(i_);
                i_--;
            }

            this.fieldSize = 10;
            return;
        }

        if (this.player.getHp() < -60) this.reset();
        else
        {
            this.stage = 100 * (this.stage / 100) + 100;
            for (int i = 0; i < Mathf.Pow(2, this.stage / 100 - 1); i++) this.list.Add(newNinja(Random.Range(0, 6)));
        }
    }
    private Ninja newNinja(int i)
    {
        GameObject ninja = new GameObject();
        {
            GameObject gameObject = (GameObject)Instantiate(Resources.Load("human"), Vector3.zero, Quaternion.identity);
            gameObject.transform.parent = ninja.transform;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        Main.setTexture(ninja.transform, this.texList[i]);
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Main.Destroy(c.GetComponent<Collider>());
            c.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(0f, 0f, 0.5f);
            c.transform.parent = ninja.transform.GetChild(0).GetChild(4).transform;
            c.transform.localScale = new Vector3(0.1f, 1.5f, 0.1f);
            c.transform.localPosition = new Vector3(-0.05f, -0.8f, -0.25f);
            c.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
        return new Ninja(ninja);
    }
    private void texture()
    {
        this.texList = new Texture[9];

        this.texList[0] = (Texture)Resources.Load("kai");
        this.texList[1] = (Texture)Resources.Load("jay");
        this.texList[2] = (Texture)Resources.Load("zane");
        this.texList[3] = (Texture)Resources.Load("cole");

        this.texList[4] = (Texture)Resources.Load("lloyd");
        this.texList[5] = (Texture)Resources.Load("nya");
    }

    static public Vector3 forward(Quaternion q) { return (q * Vector3.forward).normalized; }

    static private void setTexture(Transform transform, Texture texture)
    {
        foreach (Transform t in transform)
        {
            if (t.childCount > 0) Main.setTexture(t, texture);
            MeshRenderer r = t.GetComponent<MeshRenderer>();
            if (r != null) r.material.SetTexture("_MainTex", texture);
        }
    }
}