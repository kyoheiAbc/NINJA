using System.Collections.Generic;
using UnityEngine;

public class Special
{
    int i; public int getI() { return this.i; }
    public void setI(int s) { this.i = s; }
    Ninja ninja;
    public Special(Ninja n)
    {
        this.i = 0;
        this.ninja = n;
    }
    public void update()
    {
        if (this.i == 0) return;
        this.i--;

        List<Ninja> l = Main.instance.getList(this.ninja, 1.5f * 1.5f, 180);
        for (int i = 0; i < l.Count; i++)
        {
            if (l[i].getStun() != 0) continue;
            l[i].addHp(-1);
            l[i].addVec(Vector3.up * 0.5f);
            l[i].setStun(5);
        }
    }
}