using UnityEngine;
using UnityEngine.UI;
public class Controller
{
    int button;
    Vector2[] stick;
    public Controller()
    {
        for (int i = 0; i < 2; i++)
        {
            Image image = GameObject.Find("A").GetComponent<Image>();
            if (i == 1) image = GameObject.Find("B").GetComponent<Image>();
            image.rectTransform.position = new Vector2((0.1f * i + 0.85f) * Screen.width, 0.05f * Screen.width);
            image.rectTransform.sizeDelta = new Vector2(0.1f * Screen.width, 0.1f * Screen.width);
            image.color = Color.HSVToRGB((i + 1) / 3f, 0.5f, 1f);
        }
        this.stick = new Vector2[2];
    }
    public void update()
    {
        this.button = 0;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Began) break;
            if ((t.position - (Vector2)GameObject.Find("A").transform.position).sqrMagnitude < (Screen.width * 0.05) * (Screen.width * 0.05)) this.button += 0b_01;
            if ((t.position - (Vector2)GameObject.Find("B").transform.position).sqrMagnitude < (Screen.width * 0.05) * (Screen.width * 0.05)) this.button += 0b_10;
        }
        this.stick[1] = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x > 0.5f * Screen.width) break;
            if (t.phase == TouchPhase.Began) this.stick[0] = t.position;
            this.stick[1] = t.position - this.stick[0];
        }
        // Debug
        {
            if (Input.GetKeyDown(KeyCode.Z)) this.button = 0b_01;
            if (Input.GetKeyDown(KeyCode.X)) this.button = 0b_10;
            if (Input.GetKeyDown(KeyCode.R)) this.button = 0b_100;
        }
    }
    public int getButton() { return this.button; }
    public Vector3 getStick() { return new Vector3(this.stick[1].x, 0, this.stick[1].y); }
}