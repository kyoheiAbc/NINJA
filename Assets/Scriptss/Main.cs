using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    Camera cam; public Camera getCam() { return this.cam; }
    Controller controller;
    int frame; public int getFrame() { return this.frame / 12; }
    static public Main instance;
    List<Ninja> list;
    Ninja player; public Ninja getPlayer() { return this.player; }
    int stage; public int getStage() { return this.stage; }
    List<Texture> texList;

    void Start()
    {
        Application.targetFrameRate = 30;

        this.cam = GameObject.Find("Camera").GetComponent<Camera>();
        this.cam.clearFlags = CameraClearFlags.SolidColor;

        this.controller = new Controller();
        Main.instance = this;
        this.list = new List<Ninja>();
        this.texture();

        this.reset();
    }

    private void reset()
    {
        this.frame = 0;

        for (int i = 0; i < this.list.Count; i++) Destroy(this.list[i].renderer.getGameObject());
        this.list.Clear();

        this.player = null;

        this.stage = 0;

        this.cam.transform.parent.position = Vector3.zero;
        this.cam.transform.parent.localRotation = Quaternion.identity;
        this.cam.fieldOfView = 1;
        float f = 0;
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                f++;

                Ninja n = newNinja(x + 4 * y);
                n.setPos(new Vector3(2 * x, -3 * y, 0));
                n.setRot(Quaternion.Euler(0, 210, 0));
                n.setAi(null);
                this.list.Add(n);

                this.cam.transform.parent.position += n.getPos();
            }
        }
        this.cam.transform.parent.position /= f;
        this.cam.transform.parent.position += new Vector3(0, 1, -400);
        this.cam.backgroundColor = Color.HSVToRGB(1 / 6f, 0.5f, 0.8f);

        this.controller.reset();
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
            if (this.player.getHp() < 0 || this.player.getStun() > 0) return;

            Vector3 s = this.controller.getStick().normalized;
            this.player.addPos(s * 0.1f);
            if (s != Vector3.zero) this.player.setRot(Quaternion.LookRotation(s));
            switch (this.controller.getButton())
            {
                case 0b_0001:
                    this.player.attack.exe();
                    break;
                case 0b_0010:
                    this.player.jump(this.controller.getStick().normalized);
                    break;
                case 0b_0100:
                    break;
                case 0b_1000:
                    this.player.special.setI(60);
                    break;
            }
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
            if (Vector3.Angle(Static.forward(n.getRot()), this.list[i].getPos() - n.getPos()) > a) continue;
            ret.Add(this.list[i]);
        }
        return ret;
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
        for (int i = 0; i < this.list.Count; i++) this.list[i].addPos(new Vector3(0.001f, 0, 0));
        this.cam.transform.parent.transform.position += new Vector3(0.001f, 0, 0);

        Vector2 t = this.controller.getTouchBegan();
        if (t != new Vector2(0, 0))
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                Vector2 n = (Vector2)this.list[i].getPos() + new Vector2(0, 1);
                if ((t - n).sqrMagnitude > 0.75f * 0.75f) continue;
                this.list[i].attack.exe();
                break;
            }
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i].attack.getI() != 301) continue;

            this.player = this.list[i];
            this.player.setPos(new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10)));
            this.player.renderer.update();

            this.cam.transform.parent.position = new Vector3(0, 22.5f, -40);
            this.cam.transform.parent.eulerAngles = new Vector3(30, 0, 0);
            this.cam.fieldOfView = 15;
            this.cam.backgroundColor = Color.HSVToRGB(120 / 360f, 0.3f, 0.5f);

            for (int i_ = 0; i_ < this.list.Count; i_++)
            {
                if (this.list[i_] == this.player) continue;
                Destroy(this.list[i_].renderer.getGameObject());
                this.list.RemoveAt(i_);
                i_--;
            }

            this.controller.start();

            return;
        }
    }

    private void stageSel()
    {
        if (this.player.getHp() < -60 || this.list.Count == 1) this.stage++;

        if (this.stage % 100 < 60) return;

        if (this.player.getHp() < -60) this.reset();
        else
        {
            this.stage = 100 * (this.stage / 100) + 100;
            for (int i = 0; i < Mathf.Pow(2, this.stage / 100 - 1); i++) this.list.Add(newNinja(Random.Range(0, 8)));
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
        Static.setTexture(ninja.transform, this.texList[i]);
        {
            GameObject gameObject = (GameObject)Instantiate(Resources.Load("Sword/Cube"), Vector3.zero, Quaternion.identity);
            gameObject.transform.parent = ninja.transform.GetChild(0).GetChild(4).transform;
            gameObject.transform.localPosition = new Vector3(-0.05f, -0.8f, -0.25f);
            gameObject.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
        return new Ninja(ninja);
    }
    private void texture()
    {
        this.texList = new List<Texture>();

        this.texList.Add((Texture)Resources.Load("kai"));
        this.texList.Add((Texture)Resources.Load("jay"));
        this.texList.Add((Texture)Resources.Load("zane"));
        this.texList.Add((Texture)Resources.Load("cole"));

        this.texList.Add((Texture)Resources.Load("lloyd"));
        this.texList.Add((Texture)Resources.Load("nya"));
        this.texList.Add((Texture)Resources.Load("arin"));
        this.texList.Add((Texture)Resources.Load("sora"));

        this.texList.Add((Texture)Resources.Load("lloyd_movie"));
    }
}

public static class Static
{
    static public Vector3 forward(Quaternion q) { return (q * Vector3.forward).normalized; }

    static public void setTexture(Transform transform, Texture texture)
    {
        foreach (Transform t in transform)
        {
            if (t.childCount > 0) Static.setTexture(t, texture);
            MeshRenderer r = t.GetComponent<MeshRenderer>();
            if (r != null) r.material.SetTexture("_MainTex", texture);
        }
    }
}