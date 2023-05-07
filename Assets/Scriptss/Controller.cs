using UnityEngine;
using UnityEngine.UI;
public class Controller
{
    int button;
    Vector3 stick;
    Vector2 a, b, c;
    Vector2 v;
    int w;
    public Controller()
    {
        this.w = Screen.width;

        Image i;
        i = GameObject.Find("A").GetComponent<Image>();
        i.rectTransform.position = new Vector2(this.w * 0.8f, this.w * 0.05f);
        i.rectTransform.sizeDelta = new Vector2(this.w * 0.09f, this.w * 0.09f);
        i.color = Color.HSVToRGB(240f / 360f, 0.5f, 1f);
        this.a = i.rectTransform.position;

        i = GameObject.Find("B").GetComponent<Image>();
        i.rectTransform.position = new Vector2(this.w * 0.9f, this.w * 0.05f);
        i.rectTransform.sizeDelta = new Vector2(this.w * 0.09f, this.w * 0.09f);
        i.color = Color.HSVToRGB(120f / 360f, 0.5f, 1f);
        this.b = i.rectTransform.position;

        i = GameObject.Find("C").GetComponent<Image>();
        i.rectTransform.position = new Vector2(this.w * 0.9f, this.w * 0.15f);
        i.rectTransform.sizeDelta = new Vector2(this.w * 0.09f, this.w * 0.09f);
        i.color = Color.HSVToRGB(60f / 360f, 0.5f, 1f);
        this.c = i.rectTransform.position;

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

                if ((t.position - this.c).sqrMagnitude < (this.w * 0.05) * (this.w * 0.05)) this.button = 3;
            }
            if (i == 0)
            {
                Vector2 v = t.position - this.v;
                if (v.sqrMagnitude > (this.w * 0.025) * (this.w * 0.025)) this.stick = new Vector3(v.x, 0, v.y);
            }
        }
        if (Input.GetKey(KeyCode.Z)) this.button = 1;
        if (Input.GetKey(KeyCode.X)) this.button = 2;
        if (Input.GetKey(KeyCode.C)) this.button = 3;

    }
    public Vector3 getStick()
    {
        return this.stick;
    }
    public int getButton()
    {
        return this.button;
    }
}