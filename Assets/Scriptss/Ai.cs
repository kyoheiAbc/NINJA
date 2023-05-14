using UnityEngine;
public class Ai
{
    AttackFlow attackFlow;
    Ninja ninja;
    int sleep;
    Ninja target;
    int level;
    public Ai(Ninja n)
    {
        this.attackFlow = new AttackFlow(n);
        this.ninja = n;
        this.sleep = 0;
        this.target = Main.instance.nearestNinja(this.ninja);
        this.level = 0;
    }
    public void update()
    {

        if (this.ninja.getDamage() > 0)
        {
            if (this.level > 0) if (this.ninja.getDamage() == 0) this.attackFlow.start(Random.Range(0, 2) == 0);
            return;
        }

        Vector3 v = this.target.getPos() - this.ninja.getPos();
        v.y = 0;
        if (v != Vector3.zero) this.ninja.setRot(Quaternion.LookRotation(v));

        if (this.sleep > 0)
        {
            this.sleep--;
            if (sleep == 0) this.target = Main.instance.nearestNinja(this.ninja);
            return;
        }

        this.ninja.mv(v.normalized * 0.15f);
        if (this.level > 0) this.ninja.mv(v.normalized * 0.1f);

        // attack
        {
            if (v.sqrMagnitude < 2 * 2 && Random.Range(0, 10) == 0) this.attackFlow.start(Random.Range(0, 2) == 0);
            if (v.sqrMagnitude < 2 * 2 && this.level > 0) this.attackFlow.start(Random.Range(0, 2) == 0);

            if (Random.Range(0, 45) == 0) this.attackFlow.start(false);
            if (this.level > 0) if (Random.Range(0, 45) == 0) this.attackFlow.start(false);

            this.attackFlow.update();

            if (this.attackFlow.getDone()) this.sleep = Random.Range(24, 37);

            if (this.attackFlow.getEn())
            {
                this.target = Main.instance.nearestNinja(this.ninja);
                return;
            }
        }

        if (Random.Range(0, 90) == 0) this.sleep = Random.Range(30, 90 - 30 * this.level);
    }
    public void attackFlowStop()
    {
        this.attackFlow.stop();
    }
    public void setTarget(Ninja n)
    {
        this.target = n;

        Vector3 v = this.target.getPos() - this.ninja.getPos();
        v.y = 0;
        this.ninja.setRot(Quaternion.LookRotation(v));
    }
    public void setLevel(int i)
    {
        this.level = i;
    }
}








public class AttackFlow
{
    bool done;
    bool en;
    bool jump;
    bool last;
    Ninja ninja;
    int time;
    public AttackFlow(Ninja n)
    {
        this.done = false;
        this.en = false;
        this.ninja = n;
    }
    public void start(bool b)
    {
        if (this.en) return;
        this.en = true;
        this.jump = b;
        this.last = false;
        this.time = 0;
    }
    public void stop()
    {
        this.en = false;
    }
    public void update()
    {
        if (!this.en) return;
        if (!this.jump)
        {
            if (this.ninja.jump((this.ninja.getRot() * new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)))))
            {
                this.jump = true;
                this.time = Random.Range(10, 21);
            }
            else return;
        }
        if (this.time > 0)
        {
            this.time--;
            return;
        }
        if (this.last)
        {
            this.done = true;
            this.en = false;
            return;
        }
        this.ninja.attackExe();
        this.time = Random.Range(5, 15);
        if (Random.Range(0, 3) == 0 || this.ninja.getAttackCombo() > 2) this.last = true;
        if (this.last) this.time = Random.Range(10, 21);
    }
    public bool getEn()
    {
        return this.en;
    }
    public bool getDone()
    {
        if (!this.done) return false;
        this.done = false;
        return true;
    }
}