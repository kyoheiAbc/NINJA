using UnityEngine;
public class Ai
{
    bool atk;
    int mv;
    Ninja ninja;
    Ninja target;
    public Ai(Ninja n)
    {
        this.atk = false;
        this.mv = 0;
        this.ninja = n;
        this.target = Main.instance.nearestNinja(this.ninja);
    }
    public void update()
    {
        Vector3 old = this.ninja.getPos();

        if (this.ninja.getStun() > 0) this.atk = false;

        if (this.ninja.getHp() < 0 || this.ninja.getStun() > 0) return;

        this.target = Main.instance.nearestNinja(this.ninja);

        Vector3 v = this.target.getPos() - this.ninja.getPos();
        v.y = 0;
        if (v != Vector3.zero) this.ninja.setRot(Quaternion.LookRotation(v));

        mv();
        void mv()
        {
            if (this.mv == 0)
            {
                if (Random.Range(0, 60) == 0) this.mv = Random.Range(30, 60);
                return;
            }
            if (v.sqrMagnitude < Mathf.Pow(2f + 2 * this.ninja.getRandom(), 2))
            {
                this.mv = 0;
                return;
            }
            this.ninja.addPos(v.normalized * 0.1f);
            this.mv--;
        }

        atk();
        void atk()
        {
            if (this.atk) return;
            if (this.ninja.attack.getI() != 0) return;
            if (Random.Range(0, 60) != 0) return;
            if (!this.ninja.jump((this.ninja.getRot() * new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2))))) return;
            this.atk = true;
        }
        if (this.atk && this.ninja.attack.getI() == 0 && this.ninja.getVec().y == 0) this.ninja.attack.exe();
        if (this.ninja.attack.getI() % 100 == 15) this.atk = this.ninja.attack.exe();
        if (this.atk) this.ninja.setPos(old + v.normalized * 0.1f);
    }
}
