using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    static public Main instance;
    Controller controller;
    int frame; public int getFrame() { return this.frame / 12; }
    List<Ninja> list;
    Ninja player;
    int stage;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        this.controller = new Controller();
        this.list = new List<Ninja>();
        this.reset();
    }

    public void reset()
    {
        this.frame = 0;

        for (int i = 0; i < this.list.Count; i++) Destroy(this.list[i].renderer.getGameObject());
        this.list.Clear();

        this.player = null;

        this.stage = 0;
    }


    void Update()
    {

        this.frame += 12;
        if (this.frame == 12 * 60 * 3) this.frame = 0;

        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i].getHp() < -60)
            {
                if (i == 0) player = null;
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
            this.list[0].addPos(s * 0.1f);
            if (s != Vector3.zero) this.list[0].setRot(Quaternion.LookRotation(s));
            switch (this.controller.getButton())
            {
                case 0b_01:
                    this.list[0].attack.exe();
                    break;
                case 0b_10:
                    this.list[0].jump(this.controller.getStick().normalized);
                    break;
            }
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].renderer.update();
        }


        stage();
        void stage()
        {
            if (player != null && this.list.Count > 1) return;

            this.stage++;

            if (this.stage % 100 < 60) return;

            if (player == null)
            {
                this.reset();
                this.player = this.newNinja();
                this.player.setHp(8);
                this.player.setAi(null);
                return;
            }

            this.stage = 100 * (this.stage / 100) + 100;
            for (int i = 0; i < Mathf.Pow(2, this.stage / 100 - 1); i++) this.newNinja();
        }
    }


    public List<Ninja> getList(Ninja n, float d, float a)
    {
        List<Ninja> ret = new List<Ninja>();
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i] == n) continue;
            if ((this.list[i].getPos() - n.getPos()).sqrMagnitude > d) continue;
            if (Vector3.Angle(forward(n.getRot()), this.list[i].getPos() - n.getPos()) > a) continue;
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

    static public Vector3 forward(Quaternion q) { return (q * Vector3.forward).normalized; }

    private Ninja newNinja()
    {
        GameObject ninja = new GameObject();
        {
            GameObject gameObject = (GameObject)Instantiate(Resources.Load("human"), Vector3.zero, Quaternion.identity);
            gameObject.transform.parent = ninja.transform;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        this.setTexture(ninja.transform, (Texture)Resources.Load("lloyd_movie"));
        {
            GameObject gameObject = (GameObject)Instantiate(Resources.Load("Sword/Cube"), Vector3.zero, Quaternion.identity);
            gameObject.transform.parent = ninja.transform.GetChild(0).GetChild(4).transform;
            gameObject.transform.localPosition = new Vector3(-0.05f, -0.8f, -0.25f);
            gameObject.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
        this.list.Add(new Ninja(ninja));
        return this.list[this.list.Count - 1];
    }

    private void setTexture(Transform transform, Texture texture)
    {
        foreach (Transform t in transform)
        {
            if (t.childCount > 0) this.setTexture(t, texture);
            MeshRenderer r = t.GetComponent<MeshRenderer>();
            if (r != null) r.material.SetTexture("_MainTex", texture);
        }
    }
}
