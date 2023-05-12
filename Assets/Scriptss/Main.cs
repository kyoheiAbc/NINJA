using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    static public Main instance;
    Ninja player;
    Controller controller;
    List<Ninja> list;
    List<Renderer> rList;
    int frame;

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

        this.newNinja(0);
        this.newNinja(1);
        this.newNinja(1);
        this.newNinja(1);

        this.player = this.list[0];

        this.frame = 0;
    }

    void Update()
    {
        this.frame++;
        if (this.frame == 300) this.frame = 0;

        this.controller.update();

        if (this.player.getDamage() == 0)
        {
            this.player.mv(this.controller.getStick().normalized * 0.1f);

            switch (this.controller.getButton())
            {
                case 0b_01:
                    this.player.attackExe();
                    break;
                case 0b_10:
                    this.player.jump(this.controller.getStick().normalized);
                    break;
            }
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].update();
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
        Ninja n = new Ninja(i);
        this.list.Add(n);
        switch (i)
        {
            case 0:
                this.rList.Add(new Renderer(this.list[this.list.Count - 1], (GameObject)Instantiate(Resources.Load("Player"), n.getPos(), n.getRot())));
                break;
            case 1:
                this.rList.Add(new Renderer(this.list[this.list.Count - 1], (GameObject)Instantiate(Resources.Load("Mob"), n.getPos(), n.getRot())));
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
            if (Vector3.Angle(n.forward(), this.list[i].getPos() - n.getPos()) > a) continue;
            ret.Add(this.list[i]);
        }
        return ret;
    }

    public Ninja nearestNinja(Ninja n)
    {
        Ninja ret = null;
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

}