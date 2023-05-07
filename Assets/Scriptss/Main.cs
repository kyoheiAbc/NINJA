using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    static public Main instance;
    Controller controller;
    List<Ninja> list;
    List<Renderer> rList;
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
        this.stop = -1;

        this.newNinja(0);
        for (int i = 0; i < 126; i++)
        {
            this.newNinja(2);
        }
        this.newNinja(1);
    }

    void Update()
    {

        if (this.stop != -1)
        {
            this.stop++;
            if (this.stop == 5) this.stop = -1;
            else return;
        }

        this.controller.update();

        this.player().mv(this.controller.getStick().normalized * 0.1f);

        switch (this.controller.getButton())
        {
            case 1:
                this.player().jump(this.controller.getStick());
                break;
            case 2:
                this.player().attack();
                break;
            case 3:
                this.player().spin();
                break;
        }

        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].update();
            if (this.list[i].getHp() < -90)
            {
                this.list.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < this.rList.Count; i++)
        {
            if (this.rList[i].GetGameObject())
            {
                this.rList[i].update();
            }
            else
            {
                this.rList.RemoveAt(i);
                i--;
            }
        }
    }
    private void newNinja(int i)
    {
        Ninja n = new Ninja();
        this.list.Add(n);
        switch (i)
        {
            case 0:
                n.setHp(int.MaxValue);
                this.rList.Add(new Renderer(this.list[this.list.Count - 1], (GameObject)Instantiate(Resources.Load("Player"), n.getPos(), n.getRot())));
                break;
            case 1:
                n.setHp(64);
                this.rList.Add(new Renderer(this.list[this.list.Count - 1], (GameObject)Instantiate(Resources.Load("Master"), n.getPos(), n.getRot())));
                this.list[this.list.Count - 1].setAi(1);
                break;
            case 2:
                n.setHp(8);
                this.rList.Add(new Renderer(this.list[this.list.Count - 1], (GameObject)Instantiate(Resources.Load("Mob"), n.getPos(), n.getRot())));
                this.list[this.list.Count - 1].setAi(2);
                break;
        }
    }
    public Ninja player()
    {
        return this.list[0];
    }

    public List<Ninja> getList(Ninja n, float d, float a)
    {
        List<Ninja> r = new List<Ninja>();
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i] == n) continue;
            if ((this.list[i].getPos() - n.getPos()).sqrMagnitude > d) continue;
            if (Vector3.Angle(n.forward(), this.list[i].getPos() - n.getPos()) > a) continue;
            r.Add(this.list[i]);
        }
        return r;
    }

    public List<Ninja> getList(Vector3 p, float d)
    {
        List<Ninja> r = new List<Ninja>();
        for (int i = 0; i < this.list.Count; i++)
        {
            if ((this.list[i].getPos() - p).sqrMagnitude > d) continue;
            r.Add(this.list[i]);
        }
        return r;
    }
    public Ninja nearestNinja(Ninja n)
    {
        Ninja r = null;
        float min = float.MaxValue;
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i] == n) continue;
            if ((this.list[i].getPos() - n.getPos()).sqrMagnitude < min)
            {
                min = (this.list[i].getPos() - n.getPos()).sqrMagnitude;
                r = this.list[i];
            }
        }
        return r;
    }
    public void setStop(int i)
    {
        if (this.stop == -1) this.stop = i;
    }
}