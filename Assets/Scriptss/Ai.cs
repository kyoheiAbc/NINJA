using UnityEngine;
public class Ai
{
    bool atk;
    Ninja ninja;
    Ninja target;
    public Ai(Ninja n)
    {
        this.atk = false;
        this.ninja = n;
        this.target = Main.instance.nearestNinja(this.ninja);
    }
    public void update()
    {
        if (this.ninja.getStun() > 0) this.atk = false;

        if (this.ninja.getHp() < 0 || this.ninja.getStun() > 0) return;

        Vector3 v = this.target.getPos() - this.ninja.getPos();
        v.y = 0;
        if (v != Vector3.zero) this.ninja.setRot(Quaternion.LookRotation(v));

        if (this.atk) this.ninja.addVec(v.normalized * 0.03f);

        atk();
        void atk()
        {
            if (this.atk) return;
            if (this.ninja.attack.getI() != 0) return;
            if (Random.Range(0, 90) != 0) return;
            if (!this.ninja.jump((this.ninja.getRot() * new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2))))) return;
            this.atk = true;
        }

        if (this.atk && this.ninja.attack.getI() == 0 && this.ninja.getVec().y == 0) this.ninja.attack.exe();
        if (this.ninja.attack.getI() % 100 == 15) this.atk = this.ninja.attack.exe();
    }
}
