using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    static public Main instance;
    Controller controller;
    List<Ninja> list;
    List<Renderer> rList;
    int frame;
    int stage;
    List<Texture> texture;
    int stop;
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

        {
            this.texture = new List<Texture>();
            this.texture.Add((Texture)Resources.Load("kai"));
            this.texture.Add((Texture)Resources.Load("jay"));
            this.texture.Add((Texture)Resources.Load("zane"));
            this.texture.Add((Texture)Resources.Load("cole"));

            this.texture.Add((Texture)Resources.Load("lloyd"));
            this.texture.Add((Texture)Resources.Load("nya"));
            this.texture.Add((Texture)Resources.Load("arin"));
            this.texture.Add((Texture)Resources.Load("sora"));

            this.texture.Add((Texture)Resources.Load("lloyd_movie"));
        }

        this.reset();

        {
            this.list.Clear();
            for (int i = 0; i < this.rList.Count; i++) if (this.rList[i].getGameObject() != null) Destroy(this.rList[i].getGameObject());
            this.rList.Clear();
        }

    }
    public void reset()
    {
        this.frame = 0;

        this.list.Clear();

        for (int i = 0; i < this.rList.Count; i++) if (this.rList[i].getGameObject() != null) Destroy(this.rList[i].getGameObject());
        this.rList.Clear();

        this.stop = 0;
        this.stage = 0;

        this.newNinja(4);
        this.list[0].disableAi();
        this.list[this.list.Count - 1].setHp(16);

    }

    void Update()
    {

        // Stop
        if (this.stop > 0)
        {
            this.stop--;
            return;
        }

        // Frame
        {
            this.frame++;
            if (this.frame == 300) this.frame = 0;
        }

        if (this.rList.Count <= 1) this.stage++;
        else if (this.list[0].getAi() != null) this.stage++;
        if (this.stage % 100 == 90)
        {
            if (this.rList.Count == 0) this.reset();
            else if (this.list[0].getAi() != null) this.reset();
            else
            {
                switch (this.stage / 100)
                {
                    case 0:
                        this.newNinja(0);
                        this.newNinja(5);
                        break;
                    case 1:
                        this.newNinja(1);
                        this.newNinja(2);
                        this.newNinja(3);
                        break;
                    case 3:
                        this.newNinja(6);
                        this.newNinja(7);
                        break;
                    case 4:
                        this.newNinja(8);
                        this.list[this.list.Count - 1].getAi().setLevel(1);
                        this.list[this.list.Count - 1].setHp(16);
                        break;
                    case 5:
                        this.reset();
                        break;
                }
                this.stage = 100 * (1 + this.stage / 100);
            }
            return;
        }

        if (this.list.Count == 0) return;

        this.controller.update();

        if (this.list[0].getDamage() == 0)
        {
            this.list[0].mv(this.controller.getStick().normalized * 0.15f);

            switch (this.controller.getButton())
            {
                case 0b_01:
                    this.list[0].attackExe();
                    break;
                case 0b_10:
                    this.list[0].jump(this.controller.getStick().normalized);
                    break;
            }
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].update();
            if (this.stop != 0) return;
            if (this.list[i].getHp() < 0 && this.list[i].getDamage() == 0)
            {
                this.list.RemoveAt(i);
                i--;
                continue;
            }
        }

        for (int i = 0; i < this.rList.Count; i++)
        {
            if (this.rList[i].getGameObject() == null)
            {
                this.rList.RemoveAt(i);
                i--;
                continue;
            }
            this.rList[i].update();
        }
    }
    public void destroy(GameObject g)
    {
        Destroy(g);
    }
    private void newNinja(int i)
    {
        Ninja n = new Ninja();
        this.list.Add(n);

        GameObject gameObject = new GameObject();
        gameObject.transform.position = n.getPos();
        gameObject.transform.localRotation = n.getRot();
        GameObject model = (GameObject)Instantiate(Resources.Load("human"), Vector3.zero, Quaternion.identity);
        model.transform.parent = gameObject.transform;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.Euler(0, 180, 0);
        this.setMaterial(gameObject.transform, this.texture[i]);

        GameObject sword = (GameObject)Instantiate(Resources.Load("Sword/Cube"), Vector3.zero, Quaternion.identity);
        sword.transform.parent = gameObject.transform.GetChild(0).GetChild(4).transform;
        sword.transform.localPosition = new Vector3(-0.05f, -0.8f, -0.25f);
        sword.transform.localRotation = Quaternion.Euler(45, 0, 0);

        this.rList.Add(new Renderer(this.list[this.list.Count - 1], gameObject));
    }

    public List<Ninja> getList(Ninja n, float d, float a)
    {
        List<Ninja> ret = new List<Ninja>();
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i] == n) continue;
            if ((this.list[i].getPos() - n.getPos()).sqrMagnitude > d) continue;
            if (Vector3.Angle(n.forward(), this.list[i].getPos() - n.getPos()) > a) continue;
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
    public int getFrame()
    {
        return this.frame;
    }

    private void setMaterial(Transform transform, Texture t)
    {
        foreach (Transform child in transform)
        {
            if (child.childCount > 0) this.setMaterial(child, t);
            MeshRenderer r = child.GetComponent<MeshRenderer>();
            if (r == null) continue;
            r.material.SetTexture("_MainTex", t);
        }
    }

    public void setStop(int i)
    {
        if (this.stop == 0) this.stop = i;
    }
}
