using UnityEngine;
public class Ai
{
    Ninja ninja;
    Ninja target;

    int sleep;
    int atkCD;

    public Ai(Ninja n)
    {
        this.ninja = n;
        this.target = Main.instance.nearestNinja(this.ninja);
        this.sleep = -1;
        this.atkCD = -1;
    }
    public void update()
    {
        Vector3 v = this.target.getPos() - this.ninja.getPos();
        v.y = 0;
        if (v != Vector3.zero) this.ninja.setRot(Quaternion.LookRotation(v));

        if (this.atkCD != -1)
        {
            this.atkCD--;
        }

        if (this.sleep != -1)
        {
            this.sleep--;
            return;
        }

        this.ninja.mv(v.normalized * 0.15f);

        setAtkCD();
        void setAtkCD()
        {
            if (this.atkCD != -1) return;
            if (Random.Range(0, 30) != 0) return;
            if (!this.ninja.jump((this.ninja.getRot() * new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2))))) return;
            this.atkCD = Random.Range(10, 20);
        }

        attack();
        void attack()
        {
            if (this.atkCD != 0) return;
            if (this.ninja.attack.exe()) this.atkCD = Random.Range(10, 15);
            else this.sleep = 30;
        }

        sleep();
        void sleep()
        {
            if (this.ninja.getPos().y != 0) return;
            if (this.atkCD != -1 && this.ninja.attack.getCombo() == 0) return;
            if (Random.Range(0, 60) != 0) return;
            this.sleep = Random.Range(30, 60);
            this.atkCD = -1;
        }


    }
}

