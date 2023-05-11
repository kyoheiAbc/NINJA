
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

        Vector3 past = this.transform[0].position;

        // basic
        {
            this.transform[0].position = this.ninja.getPos();
            this.transform[0].localRotation = this.ninja.getRot();
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

            float sin = Mathf.Sin(2 * Mathf.PI * (Time.time + this.ninja.getRandom())) * 0.5f + 0.5f;
            this.transform[1].localRotation = Quaternion.Lerp(this.walk[0], this.walk[1], sin);
            this.transform[2].localRotation = Quaternion.Lerp(this.walk[1], this.walk[0], sin);
            this.transform[3].localRotation = Quaternion.Lerp(this.walk[1], this.walk[0], sin);
            this.transform[4].localRotation = Quaternion.Lerp(this.walk[0], this.walk[1], sin);
        }

        // Attack
        {
            if (this.ninja.attack.combo > 0)
            {
                float f = (this.ninja.attack.inc - 1) / 4f;
                f = Mathf.Clamp(f, 0, 1);
                if (this.ninja.attack.combo == 2) this.transform[3].localRotation = this.attack[this.ninja.attack.combo - 1] * Quaternion.AngleAxis(-180 * f, new Vector3(0, 0, 1));
                else this.transform[3].localRotation = this.attack[this.ninja.attack.combo - 1] * Quaternion.AngleAxis(-180 * f, new Vector3(1, 0, 0));
            }
        }
    }



}
