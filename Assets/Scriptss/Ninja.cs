using System.Collections.Generic;
using UnityEngine;
public class Ninja
{
    int attackInc;
    Vector3 pos;
    Quaternion rot;
    Vector3 vec;
    int hp;
    Ai ai;
    int damageInc;
    int inc;

    readonly float random;
    public Ninja()
    {
        this.attackInc = 0;
        this.pos = new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10));
        this.rot = Quaternion.identity;
        this.vec = Vector3.zero;
        this.random = Random.Range(-1f, 1f);
        this.hp = 16;
        this.damageInc = -1;
        this.inc = -1;
    }
    public void update()
    {

        // Physics
        {
            List<Ninja> l = Main.instance.getList(this, 1, 180);
            for (int i = 0; i < l.Count; i++)
            {
                Vector3 v = l[i].getPos() - this.getPos();
                v = v.normalized;
                if (v == Vector3.zero) v = Vector3.right;
                this.addVec(v * -0.1f);
                l[i].addVec(v * 0.1f);
            }

            this.pos += this.vec;
            if (this.inc != -1)
            {
                this.vec.x *= 0.1f;
                this.vec.z *= 0.1f;
            }
            else
            {
                this.vec.x *= 0.75f;
                this.vec.z *= 0.75f;
            }
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

        // Attack
        {
            int combo = this.attackInc >> 6;
            int inc = this.attackInc & 0b_0011_1111;
            if (combo > 0) this.attackInc += 1;
            if (inc == 30) this.attackInc = 0;
            if (inc == 3)
            {
                List<Ninja> l;
                l = Main.instance.getList(this, 2.5f * 2.5f, 45);

                for (int i = 0; i < l.Count; i++)
                {
                    if (this.ai != null && l[i] != Main.instance.player()) continue;
                    l[i].addVec((this.forward().normalized + new Vector3(0, 0.3f, 0)) * 0.3f * combo);
                    l[i].damage((int)Mathf.Pow(2, combo));
                    if (this.ai == null) Main.instance.setStop(0);
                }
            }
        }

        // skill
        if (this.inc != -1)
        {
            int skill = this.inc >> 8;
            int cnt = this.inc & 0b_0000_0000_1111_1111;
            cnt++;

            if (cnt == 60)
            {
                this.inc = -1;
            }
            else
            {

                List<Ninja> l = Main.instance.getList(this, 4, 180);
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].damage(2))
                    {
                        l[i].addVec(1f * Vector3.up);
                        // Main.instance.setStop(0);
                    }
                }

                this.inc = (skill << 8) + cnt;
            }
        }

        // Ai
        {
            if (this.ai != null) ai.update();
        }


        //
        if (this.damageInc == 15) this.damageInc = -1;
        if (this.damageInc != -1) this.damageInc++;

        if (this.hp <= 0) this.hp--;

        if (Mathf.Abs(this.pos.x) > 10) this.vec += 0.1f * Mathf.Sign(this.pos.x) * Vector3.left;
        if (Mathf.Abs(this.pos.z) > 10) this.vec += 0.1f * Mathf.Sign(this.pos.z) * Vector3.back;
        if (Mathf.Abs(this.pos.y) > 5) this.vec += 0.1f * Vector3.down;


    }

    public void mv(Vector3 v)
    {
        if (this.inc != -1)
        {
            this.vec += 3 * v;
        }
        else
        {
            this.pos += v;
        }
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
    public void attack()
    {
        int combo = this.attackInc >> 6;
        int inc = this.attackInc & 0b_0011_1111;

        if (combo > 0 && inc < 10) return;
        if (combo > 2) return;

        combo++;
        this.attackInc = combo << 6;
    }

    public void spin()
    {
        this.inc = 0;
    }
    public int getInc()
    {
        return this.inc;
    }
    public bool jump(Vector3 v)
    {
        if (pos.y > 0.001f) return false;
        this.vec += Vector3.up * 0.35f;
        this.vec += v.normalized * 0.5f;
        return true;
    }
    public void addVec(Vector3 v)
    {
        this.vec += v;
    }
    public bool damage(int d)
    {
        if (this.damageInc != -1) return false;
        this.hp -= d;
        this.damageInc = 0;
        if (this.ai != null) this.ai.setInc(0);
        return true;
    }
    public int getDamageInc()
    {
        return this.damageInc;
    }
    public int getHp()
    {
        return this.hp;
    }
    public void setHp(int i)
    {
        this.hp = i; ;
    }
    public int getAttackInc()
    {
        return this.attackInc;
    }
    public Vector3 forward()
    {
        return (this.rot * Vector3.forward).normalized;
    }
    public void setAi(int i)
    {
        this.ai = new Ai(this, i);
    }
    public void setRot(Quaternion q)
    {
        this.rot = q;
    }
    public Ai getAi()
    {
        return this.ai;
    }
    public float getRandom()
    {
        return random;
    }
}





