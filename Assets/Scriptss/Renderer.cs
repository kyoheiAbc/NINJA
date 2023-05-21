
using UnityEngine;
public class Renderer
{
    Ninja ninja;
    GameObject gameObject; public GameObject getGameObject() { return this.gameObject; }
    Transform[] transform;
    Quaternion[] attack;
    Quaternion walk;
    bool walkEn; public void setWalkEn(bool s) { this.walkEn = s; }

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

        this.transform[0].position = this.ninja.getPos();
        this.transform[0].localRotation = this.ninja.getRot();

        this.walk = Quaternion.Euler(30, 0, 0);
        this.walkEn = false;

        this.attack = new Quaternion[2] { Quaternion.Euler(90, 0, 90), Quaternion.Euler(180, 0, 0) };
    }

    public void update()
    {
        float frame = (Main.instance.getFrame() % 30) / 30f;
        Vector3 old = this.transform[0].position;

        this.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            this.transform[i].localRotation = Quaternion.identity;
        }

        this.transform[0].position = this.ninja.getPos();
        this.transform[0].localRotation = this.ninja.getRot();

        walk();
        void walk()
        {
            if (this.ninja.getPos() == old && !this.walkEn) return;
            float sin = Mathf.Sin(2f * Mathf.PI * (frame + this.ninja.getRandom())) * 0.5f + 0.5f;
            this.transform[1].localRotation = Quaternion.Lerp(this.walk, Quaternion.Inverse(this.walk), sin);
            this.transform[2].localRotation = Quaternion.Lerp(Quaternion.Inverse(this.walk), this.walk, sin);
            this.transform[3].localRotation = Quaternion.Lerp(Quaternion.Inverse(this.walk), this.walk, sin);
            this.transform[4].localRotation = Quaternion.Lerp(this.walk, Quaternion.Inverse(this.walk), sin);
        }

        attack();
        void attack()
        {
            int i = this.ninja.attack.getI();
            if (i == 0) return;
            float f = (30 - i % 100) / 4f;
            f = Mathf.Clamp(f, 0, 1);
            float cos = 0.5f - Mathf.Cos(Mathf.PI * f) * 0.5f;
            int c = i / 100;
            this.transform[3].localRotation = this.attack[1 - c % 2] * Quaternion.AngleAxis(-180 * cos, new Vector3(1 - c % 2, 0, c % 2));
        }

        if (this.ninja.getStun() > 0 || this.ninja.getHp() < 0)
        {
            this.gameObject.SetActive(Mathf.Sin(16 * Mathf.PI * (frame + this.ninja.getRandom())) > 0);
        }
    }
}
