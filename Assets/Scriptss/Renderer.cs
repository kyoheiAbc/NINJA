
using UnityEngine;
public class Renderer
{
    Ninja ninja;
    GameObject gameObject;
    Transform[] transform;
    Quaternion[] attack;
    Quaternion[] walk;
    GameObject damage;
    GameObject spin;



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

        this.damage = this.gameObject.transform.GetChild(1).gameObject;
        this.damage.SetActive(false);

        if (this.ninja == Main.instance.player())
        {
            this.spin = this.gameObject.transform.GetChild(2).gameObject;
            this.spin.SetActive(false);
        }
    }

    public void update()
    {
        if (this.ninja.getHp() < -90)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        Vector3 past = this.transform[0].position;

        // basic
        {
            this.transform[0].position = this.ninja.getPos();
            this.transform[0].localRotation = this.ninja.getRot();
        }


        if (this.ninja.getHp() <= 0)
        {
            this.gameObject.SetActive(Mathf.Sin(4 * Mathf.PI * (Time.time + this.ninja.getRandom())) >= 0);
            // to-do, adjust center point
            this.transform[0].localRotation *= Quaternion.AngleAxis(-90, new Vector3(1, 0, 0));
            return;
        }


        if (this.ninja.getPos() == past)
        {
            this.transform[1].localRotation = Quaternion.identity;
            this.transform[2].localRotation = Quaternion.identity;
            this.transform[3].localRotation = Quaternion.identity;
            this.transform[4].localRotation = Quaternion.identity;
        }
        else
        {
            if (this.ninja.getInc() == -1)
            {
                float sin = Mathf.Sin(2 * Mathf.PI * (Time.time + this.ninja.getRandom())) * 0.5f + 0.5f;
                this.transform[1].localRotation = Quaternion.Lerp(this.walk[0], this.walk[1], sin);
                this.transform[2].localRotation = Quaternion.Lerp(this.walk[1], this.walk[0], sin);
                this.transform[3].localRotation = Quaternion.Lerp(this.walk[1], this.walk[0], sin);
                this.transform[4].localRotation = Quaternion.Lerp(this.walk[0], this.walk[1], sin);
            }
        }




        // Attack
        {
            int a = ninja.getAttackInc();
            int combo = a >> 6;
            int inc = a & 0b_0011_1111;
            if (combo > 0)
            {
                float f = (inc - 1) / 4f;
                f = Mathf.Clamp(f, 0, 1);
                if (combo == 2) this.transform[3].localRotation = this.attack[combo - 1] * Quaternion.AngleAxis(-180 * f, new Vector3(0, 0, 1));
                else this.transform[3].localRotation = this.attack[combo - 1] * Quaternion.AngleAxis(-180 * f, new Vector3(1, 0, 0));
                // this.transform[3].localRotation = Quaternion.Lerp(this.attack[combo], this.attack[combo + 1], f);
            }
        }

        // Spin
        {
            if (this.spin != null)
            {
                if (this.ninja.getInc() != -1)
                {
                    this.spin.SetActive(true);
                    this.transform[0].position += Vector3.up;
                }
                else
                {
                    this.spin.SetActive(false);
                }
            }
        }

        // Damage
        {
            if (this.ninja.getDamageInc() == -1)
            {
                this.damage.SetActive(false);
            }
            else
            {
                this.damage.SetActive(Mathf.Sin(16 * Mathf.PI * (Time.time + this.ninja.getRandom())) >= 0);
            }
        }

        // int i = this.ninja.getAi().getInc();
        // if (i >> 8 == 2)
        // {
        //     if ((i & 0b_0000_0000_1111_1111) > 40)
        //     {
        //         this.bullet.localScale = Vector3.one;
        //         this.bullet.position = this.ninja.getAi().getBullet() + 1.5f * Vector3.up;
        //     }
        // }
        // else
        // {
        //     this.bullet.localScale = Vector3.zero;
        // }
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

}
