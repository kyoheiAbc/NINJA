using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    static public Main instance;
    Controller controller;
    int frame; public int getFrame() { return this.frame / 12; }
    List<Ninja> list;
    List<Renderer> rList;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        this.controller = new Controller();
        this.list = new List<Ninja>();
        this.rList = new List<Renderer>();
        this.reset();
    }

    public void reset()
    {
        this.frame = 0;

        this.list.Clear();

        for (int i = 0; i < this.rList.Count; i++)
        {
            if (this.rList[i].getGameObject() != null) Destroy(this.rList[i].getGameObject());
        }
        this.rList.Clear();

        this.newNinja(0);
        this.list[0].setAi(null);

        this.newNinja(0);
    }


    void Update()
    {

        this.frame += 12;
        if (this.frame == 12 * 60 * 3) this.frame = 0;


        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].update();
        }


        for (int i = 0; i < this.rList.Count; i++)
        {
            this.rList[i].update();
        }


        this.controller.update();

        Vector3 s = this.controller.getStick().normalized;
        this.list[0].addVec(s * 0.03f);
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

    private void newNinja(int i)
    {
        Ninja n = new Ninja();

        GameObject gameObject = new GameObject();
        gameObject.transform.position = n.getPos();
        gameObject.transform.localRotation = n.getRot();

        GameObject g = (GameObject)Instantiate(Resources.Load("human"), Vector3.zero, Quaternion.identity);
        g.transform.parent = gameObject.transform;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.Euler(0, 180, 0);

        this.setTexture(gameObject.transform, (Texture)Resources.Load("lloyd_movie"));

        GameObject sword = (GameObject)Instantiate(Resources.Load("Sword/Cube"), Vector3.zero, Quaternion.identity);
        sword.transform.parent = gameObject.transform.GetChild(0).GetChild(4).transform;
        sword.transform.localPosition = new Vector3(-0.05f, -0.8f, -0.25f);
        sword.transform.localRotation = Quaternion.Euler(45, 0, 0);

        this.list.Add(n);
        this.rList.Add(new Renderer(this.list[this.list.Count - 1], gameObject));
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
