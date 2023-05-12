
using UnityEngine;
public class Renderer
{
    Ninja ninja;
    GameObject gameObject;
    Transform[] transform;
    Quaternion[] attack;
    Quaternion[] walk;

    public Renderer(Ninja n, GameObject g)
    {
        this.ninja = n;
        this.gameObject = g;
        this.transform = new Transform[5];
        this.transform[0] = this.gameObject.transform;
        for (int i = 0; i < 4; i++)
        {
            this.transform[i + 1] = this.transform[0].GetChild(0).GetChild(i + 2).gameObject.transform;
        }

        this.walk = new Quaternion[2];
        this.walk[0] = Quaternion.Euler(45, 0, 0);
        this.walk[1] = Quaternion.Inverse(this.walk[0]);

        this.attack = new Quaternion[3];
        this.attack[0] = Quaternion.Euler(180, 0, -45);
        this.attack[1] = Quaternion.Euler(90, 0, 90);
        this.attack[2] = Quaternion.Euler(180, 0, 0);

    }

    public void update()
    {
        this.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            this.transform[i].localRotation = Quaternion.identity;
        }

        float frame = (Main.instance.getFrame() % 30) / 30f;

        Vector3 past = this.transform[0].position;

        // basic
        {
            this.transform[0].position = this.ninja.getPos();
            this.transform[0].localRotation = this.ninja.getRot();
        }

        // walk
        walkFunc();
        void walkFunc()
        {
            if (this.ninja.getDamage() > 0) return;
            if (this.ninja.getHp() < 0) return;
            if (this.ninja.getPos() == past) return;
            float sin = Mathf.Sin(2 * Mathf.PI * (frame + this.ninja.getRandom())) * 0.5f + 0.5f;
            this.transform[1].localRotation = Quaternion.Lerp(this.walk[0], this.walk[1], sin);
            this.transform[2].localRotation = Quaternion.Lerp(this.walk[1], this.walk[0], sin);
            this.transform[3].localRotation = Quaternion.Lerp(this.walk[1], this.walk[0], sin);
            this.transform[4].localRotation = Quaternion.Lerp(this.walk[0], this.walk[1], sin);
        }


        // Damage
        {
            if (this.ninja.getDamage() > 0)
            {
                this.gameObject.SetActive(Mathf.Sin(16 * Mathf.PI * (frame + this.ninja.getRandom())) > 0);
            }
        }


        // Attack
        {
            if (this.ninja.getAttackCombo() > 0)
            {
                float f = (this.ninja.getAttackInc() - 1) / 4f;
                f = Mathf.Clamp(f, 0, 1);
                if (this.ninja.getAttackCombo() == 2) this.transform[3].localRotation = this.attack[this.ninja.getAttackCombo() - 1] * Quaternion.AngleAxis(-180 * f, new Vector3(0, 0, 1));
                else this.transform[3].localRotation = this.attack[this.ninja.getAttackCombo() - 1] * Quaternion.AngleAxis(-180 * f, new Vector3(1, 0, 0));
            }
        }


        // Death
        {
            if (this.ninja.getHp() < 0)
            {
                if (this.ninja.getDamage() == 0)
                {
                    Main.instance.destroy(this.gameObject);
                    return;
                }
            }
        }
    }



    public GameObject getGameObject()
    {
        return this.gameObject;
    }



}
