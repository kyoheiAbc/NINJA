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
    public void start()
    {
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                Image image = GameObject.Find("A").GetComponent<Image>();
                if (x + 2 * y == 1) image = GameObject.Find("B").GetComponent<Image>();
                if (x + 2 * y == 2) image = GameObject.Find("C").GetComponent<Image>();
                if (x + 2 * y == 3) image = GameObject.Find("D").GetComponent<Image>();
                image.rectTransform.position = new Vector2((0.15f * x + 0.75f) * Screen.width, (0.15f * y + 0.1f) * Screen.width);
                image.rectTransform.sizeDelta = new Vector2(0.1f * Screen.width, 0.1f * Screen.width);
                image.color = Color.HSVToRGB((x + 2 * y) / 6f, 0.5f, 1f);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
            }
        }
    }
    public void reset()
    {
        GameObject.Find("A").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(0, 0);
        GameObject.Find("B").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(0, 0);
        GameObject.Find("C").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(0, 0);
        GameObject.Find("D").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(0, 0);
    }

    public void update()
    {
        this.button = 0;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Began) continue;
            if ((t.position - (Vector2)GameObject.Find("A").transform.position).sqrMagnitude < (Screen.width * 0.05) * (Screen.width * 0.05)) this.button |= 0b_0001;
            if ((t.position - (Vector2)GameObject.Find("B").transform.position).sqrMagnitude < (Screen.width * 0.05) * (Screen.width * 0.05)) this.button |= 0b_0010;
            if ((t.position - (Vector2)GameObject.Find("C").transform.position).sqrMagnitude < (Screen.width * 0.05) * (Screen.width * 0.05)) this.button |= 0b_0100;
            if ((t.position - (Vector2)GameObject.Find("D").transform.position).sqrMagnitude < (Screen.width * 0.05) * (Screen.width * 0.05)) this.button |= 0b_1000;
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
            this.touchBegan = Main.instance.getCam().ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -Main.instance.getCam().transform.parent.position.z));
            break;
        }
        {
            if (Input.GetKeyDown(KeyCode.Z)) this.button |= 0b_0001;
            if (Input.GetKeyDown(KeyCode.X)) this.button |= 0b_0010;
            if (Input.GetKeyDown(KeyCode.A)) this.button |= 0b_0100;
            if (Input.GetKeyDown(KeyCode.S)) this.button |= 0b_1000;

        }
    }
}