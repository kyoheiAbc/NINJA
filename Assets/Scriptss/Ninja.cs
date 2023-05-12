using System.Collections.Generic;
using UnityEngine;
public class Ninja
{
    Ai ai;
    Attack attack;
    Vector3 pos;
    Quaternion rot;
    Vector3 vec;
    int type;
    int damage;
    float random;
    int hp;

    public Ninja(int i)
    {
        this.pos = new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10));
        this.rot = Quaternion.identity;
        this.vec = Vector3.zero;
        this.random = Random.Range(-1f, 1f);
        this.type = i;
        this.attack = new Attack(this);
        this.damage = 0;
        this.hp = 8;
        if (this.type == 0) this.hp = int.MaxValue;
        if (this.type != 0) this.ai = new Ai(this);
    }
    public void update()
    {
        // Physics
        {
            List<Ninja> l = Main.instance.getList(this, 1, 180);
            for (int i = 0; i < l.Count; i++)
            {
                Vector3 v = l[i].getPos() - this.getPos();
                v.y = 0;
                v = v.normalized;
                if (v == Vector3.zero) v = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
                this.addVec(v * -0.5f);
                l[i].addVec(v * 0.5f);
            }

            this.pos += this.vec;
            this.vec.x *= 0.75f;
            this.vec.z *= 0.75f;
            if (this.pos.y < 0.001f)
            {
                this.pos.y = 0;
                this.vec.y = 0;
            }
            else
            {
                this.vec.y -= 0.05f;
            }
        }

        // Ai
        {
            if (this.ai != null) this.ai.update();
        }

        // Attack
        {
            this.attack.update();
        }

        // Damage
        {
            if (this.damage > 0) this.damage--;
        }


        if (Mathf.Abs(this.pos.x) > 10) this.vec += 0.1f * Mathf.Sign(this.pos.x) * Vector3.left;
        if (Mathf.Abs(this.pos.z) > 10) this.vec += 0.1f * Mathf.Sign(this.pos.z) * Vector3.back;
        if (Mathf.Abs(this.pos.y) > 5) this.vec += 0.1f * Vector3.down;


    }

    public void mv(Vector3 v)
    {
        this.pos += v;
        if (v != Vector3.zero) this.rot = Quaternion.LookRotation(v);
    }
    public Vector3 getPos()
    {
        return this.pos;
    }
    public Quaternion getRot()
    {
        return this.rot;
    }
    public bool jump(Vector3 v)
    {
        if (pos.y != 0) return false;
        this.vec += Vector3.up * 0.35f;
        this.vec += v.normalized * 0.5f;
        return true;
    }
    public void addVec(Vector3 v)
    {
        this.vec += v;
    }

    public Vector3 forward()
    {
        return (this.rot * Vector3.forward).normalized;
    }
    public void setRot(Quaternion q)
    {
        this.rot = q;
    }

    public float getRandom()
    {
        return this.random;
    }

    public void attackExe()
    {
        this.attack.exe();
    }
    public int getAttackCombo()
    {
        return this.attack.getCombo();
    }

    public int getAttackInc()
    {
        return this.attack.getInc();
    }
    public int getDamage() { return this.damage; }
    public void setDamage(Ninja n, int d)
    {
        if (this.damage != 0) return;
        if (this.hp < 0) return;
        this.hp -= d;
        this.damage = 10;
        if (this.hp < 0) this.damage *= 3;
        this.attack.stop();
        if (this.ai == null) return;

        this.ai.attackFlowStop();
        this.ai.setTarget(n);
    }

    public int getHp()
    {
        return this.hp;
    }


}

public class Attack
{
    Ninja ninja;
    int combo;
    int inc;
    public Attack(Ninja n)
    {
        this.ninja = n;
        this.combo = 0;
        this.inc = 0;
    }
    public void exe()
    {
        switch (this.combo)
        {
            case 1:
            case 2:
                if (this.inc < 10) return;
                break;
            case 3:
                return;
        }
        this.combo++;
        this.inc = 0;
    }
    public void stop()
    {
        this.combo = 0;
        this.inc = 0;
    }
    public void update()
    {
        if (this.combo == 0) return;

        this.inc++;

        if (this.inc == 5)
        {
            List<Ninja> l = Main.instance.getList(this.ninja, 2.5f * 2.5f, 45);
            for (int i = 0; i < l.Count; i++)
            {
                l[i].addVec(this.ninja.forward().normalized * 0.3f * this.combo);
                l[i].setDamage(this.ninja, this.combo);
            }
        }

        if (this.inc > 30)
        {
            this.combo = 0;
            this.inc = 0;
        }

    }
    public int getCombo()
    {
        return this.combo;
    }

    public int getInc()
    {
        return this.inc;
    }
}





