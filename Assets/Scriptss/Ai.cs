using System.Collections.Generic;
using UnityEngine;
public class Ai
{
    int inc;
    int type;
    Ninja ninja;
    public Ai(Ninja n, int i)
    {
        this.inc = 0;
        this.ninja = n;
        this.type = i;
    }
    public void update()
    {
        if (this.ninja.getHp() <= 0) return;

        Vector3 v = Main.instance.player().getPos() - this.ninja.getPos();
        v.y = 0;
        this.ninja.setRot(Quaternion.LookRotation(v));

        int skill = this.inc >> 8;
        int cnt = this.inc & 0b_0000_0000_1111_1111;

        switch (skill)
        {
            case 0:
                if (this.type == 1) skill = 1;
                else
                {
                    if (Random.Range(0, 900) == 0)
                    {
                        skill = 2;
                    }
                }
                break;
            case 1:
                if (v.sqrMagnitude > Mathf.Pow(3.5f + this.ninja.getRandom(), 2))
                {
                    if (this.ninja.getPos().y == 0) this.ninja.mv(v.normalized * (0.03f * this.ninja.getRandom() + 0.06f));
                }
                switch (Random.Range(0, 90))
                {
                    case 0:
                        skill = 2;
                        break;
                    case 1:
                        skill = 3;
                        break;
                    case 2:
                        this.ninja.jump(this.ninja.getRot() * Vector3.right);
                        break;
                    case 3:
                        this.ninja.jump(this.ninja.getRot() * Vector3.left);
                        break;
                }
                break;

            case 2:

                switch (cnt)
                {
                    case 0:
                        if (this.ninja.jump(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)))) cnt++;
                        break;
                    case 20:
                        this.ninja.attack();
                        cnt++;
                        break;
                    case 40:
                        this.ninja.attack();
                        cnt++;
                        break;
                    case 60:
                        this.ninja.attack();
                        cnt++;
                        break;
                    case 80:
                        skill = 0;
                        cnt = 0;
                        break;
                    default:
                        if (this.ninja.getPos().y == 0) this.ninja.mv(v.normalized * (0.03f * this.ninja.getRandom() + 0.06f));
                        cnt++;
                        break;
                }
                break;

            case 3:
                switch (cnt)
                {
                    case 0:
                        if (this.ninja.jump(new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)).normalized)) cnt++;
                        break;
                    case 20:
                        if (this.ninja.jump(new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)).normalized)) cnt++;
                        break;
                    case 40:
                        if (this.ninja.jump(new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)).normalized)) cnt++;
                        break;
                    case 60:
                        this.ninja.addVec(2f * this.ninja.forward().normalized);
                        cnt++;
                        break;
                    case 90:
                        skill = 0;
                        cnt = 0;
                        break;
                    default:
                        cnt++;
                        break;
                }
                if (cnt > 60)
                {
                    List<Ninja> l = Main.instance.getList(this.ninja, 4, 180);
                    for (int i = 0; i < l.Count; i++)
                    {
                        if (l[i] != Main.instance.player()) continue;
                        if (l[i].damage(2))
                        {
                            l[i].addVec(1f * Vector3.up);
                            // Main.instance.setStop(0);
                        }
                    }
                }
                break;

        }
        this.inc = (skill << 8) + cnt;
    }

    public int getInc()
    {
        return this.inc;
    }
    public void setInc(int i)
    {
        this.inc = i;
    }

}