using System.Collections.Generic;
using UnityEngine;

public class Special
{
    int i; public int getI() { return this.i; }
    public void setI(int s) { this.i = s; }
    Ninja ninja;
    GameObject cube;

    public Special(Ninja n)
    {
        this.i = 0;
        this.ninja = n;
    }

    public void exe()
    {
        if (this.i > 0)
        {
            return;
        }

        i = 30;

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Main.Destroy(cube.GetComponent<Collider>());
        if (Main.instance.getFrame() % 3 == 0) cube.GetComponent<MeshRenderer>().material.color = Color.yellow;
        else if (Main.instance.getFrame() % 3 == 1) cube.GetComponent<MeshRenderer>().material.color = Color.red;
        else cube.GetComponent<MeshRenderer>().material.color = Color.blue;

        cube.transform.position = this.ninja.getPos() + Main.forward(this.ninja.getRot()).normalized * 3;
        cube.transform.rotation = this.ninja.getRot();
        cube.transform.localScale = new Vector3(1.5f, 1.5f, 6);


        List<Ninja> l = Main.instance.getList(cube.transform.position, cube.transform.rotation, cube.transform.localScale);
        for (int i = 0; i < l.Count; i++)
        {
            if (l[i] == this.ninja) continue;
            l[i].addVec(0.3f * Vector3.up);
            l[i].setStun(5);
            l[i].attack.setI(0);
        }
    }

    public void update()
    {
        if (this.i == 0) return;

        if (this.i == 15) Main.Destroy(cube);

        this.i--;

        // List<Ninja> l = Main.instance.getList(this.ninja, 1.5f * 1.5f, 180);
        // for (int i = 0; i < l.Count; i++)
        // {
        //     if (l[i].getStun() != 0) continue;
        //     l[i].addHp(-1);
        //     l[i].addVec(Vector3.up * 0.5f);
        //     l[i].setStun(5);
        // }
    }
}