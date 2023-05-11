using UnityEngine;
using UnityEngine.UI;
public class Controller
{
    int button;
    Vector3 stick;
    readonly Vector2 a, b;
    Vector2 v;
    readonly int w;
    public Controller()
    {
        this.w = Screen.width;

        Image i;
        i = GameObject.Find("A").GetComponent<Image>();
        i.rectTransform.position = new Vector2(this.w * 0.95f, this.w * 0.05f);
        i.rectTransform.sizeDelta = new Vector2(this.w * 0.1f, this.w * 0.1f);
        i.color = Color.HSVToRGB(240f / 360f, 0.5f, 1f);
        this.a = i.rectTransform.position;

        i = GameObject.Find("B").GetComponent<Image>();
        i.rectTransform.position = new Vector2(this.w * 0.85f, this.w * 0.05f);
        i.rectTransform.sizeDelta = new Vector2(this.w * 0.1f, this.w * 0.1f);
        i.color = Color.HSVToRGB(120f / 360f, 0.5f, 1f);
        this.b = i.rectTransform.position;

        this.v = Vector2.zero;
    }
    public void update()
    {
        this.button = 0;
        this.stick = Vector3.zero;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                if (i == 0) this.v = t.position;

                if ((t.position - this.a).sqrMagnitude < (this.w * 0.05) * (this.w * 0.05)) this.button = 1;

                if ((t.position - this.b).sqrMagnitude < (this.w * 0.05) * (this.w * 0.05)) this.button = 2;

            }
            if (i == 0)
            {
                Vector2 v = t.position - this.v;
                if (v.sqrMagnitude > (this.w * 0.025) * (this.w * 0.025)) this.stick = new Vector3(v.x, 0, v.y);
            }
        }
        if (Input.GetKey(KeyCode.X)) this.button = 1;
        if (Input.GetKey(KeyCode.Z)) this.button = 2;
    }

    public Vector3 getStick() { return this.stick; }
    public int getButton() { return this.button; }
}