using System.Collections.Generic;
using UnityEngine;
public class Ai
{
    Ninja ninja;
    Ninja target;
    bool act;
    int coolTime;
    bool jump;
    bool last;
    int sleep;

    public Ai(Ninja n)
    {
        this.ninja = n;
        this.act = false;
        this.coolTime = 0;
        this.jump = false;
        this.last = false;
        this.sleep = 0;
        this.target = Main.instance.nearestNinja(this.ninja);
    }
    public void update()
    {

        Vector3 v = this.target.getPos() - this.ninja.getPos();
        v.y = 0;
        this.ninja.setRot(Quaternion.LookRotation(v));


        if (this.sleep > 0)
        {
            this.sleep--;
            if (sleep == 0) this.target = Main.instance.nearestNinja(this.ninja);
            return;
        }

        this.ninja.mv(v.normalized * 0.1f);

        if (Random.Range(0, 45) == 0) this.act = true;
        if (this.act)
        {
            if (this.ninja.attack.combo == 0) this.target = Main.instance.nearestNinja(this.ninja);

            if (!this.jump)
            {

                if (this.ninja.jump((this.ninja.getRot() * new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)))))
                {
                    this.jump = true;
                    this.coolTime = Random.Range(15, 26);
                }
                else return;
            }
            if (this.coolTime > 0)
            {
                this.coolTime--;
                return;
            }

            if (this.last)
            {
                this.act = false;
                this.coolTime = 0;
                this.jump = false;
                this.last = false;
                this.sleep = Random.Range(24, 37);
                return;
            }

            this.ninja.attack.exe();
            this.coolTime = Random.Range(15, 26);

            if (Random.Range(0, 3) == 0 || this.ninja.attack.combo > 2)
            {
                this.last = true;
            }

            if (this.last) this.coolTime = Random.Range(5, 15);


            return;
        }

        if (v.sqrMagnitude < 1.5 * 1.5)
        {
            this.act = true;
            this.coolTime = 0;
            this.jump = Random.Range(0, 2) == 0;
            return;
        }


        if (Random.Range(0, 90) == 0) this.sleep = Random.Range(30, 90);

    }

    public void setTarget(Ninja n)
    {
        this.target = n;

    }


}