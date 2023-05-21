using UnityEngine;
using UnityEngine.UI;
public class Controller
{
    int button; public int getButton() { return this.button; }
    Vector2[] stick; public Vector3 getStick() { return new Vector3(this.stick[1].x, 0, this.stick[1].y); }
    Vector2 touchBegan; public Vector2 getTouchBegan() { return this.touchBegan; }

    public Controller()
    {
        this.stick = new Vector2[2];
    }
    public void update()
    {
        this.button = 0;
        float x = Main.instance.button[0].sizeDelta.x / 2f;
        x *= x;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Began) continue;
            for (int i_ = 0; i_ < Main.instance.button.Length; i_++)
            {
                if ((t.position - (Vector2)Main.instance.button[i_].position).sqrMagnitude < x) this.button |= (int)Mathf.Pow(2, i_);
            }
        }
        this.stick[1] = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x > 0.5f * Screen.width) continue;
            if (t.phase == TouchPhase.Began) this.stick[0] = t.position;
            this.stick[1] = t.position - this.stick[0];
        }
        this.touchBegan = new Vector2(0, 0);
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Began) continue;
            this.touchBegan = Main.instance.cam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -Main.instance.transform.position.z));
            break;
        }
        {
            if (Input.GetKeyDown(KeyCode.Z)) this.button |= 1;
            if (Input.GetKeyDown(KeyCode.X)) this.button |= 2;
            if (Input.GetKeyDown(KeyCode.C)) this.button |= 4;
            if (Input.GetKeyDown(KeyCode.V)) this.button |= 8;

        }
    }
}