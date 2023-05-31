using UnityEngine;
public class Controller
{
    int button; public int getButton() { return this.button; }
    Vector2 deltaPosition; public Vector3 getDeltaPosition() { return new Vector3(this.deltaPosition.x, 0, this.deltaPosition.y); }
    Vector2[] stick; public Vector3 getStick() { return new Vector3(this.stick[1].x, 0, this.stick[1].y); }
    Vector2 touchPhaseBegan; public Vector2 getTouchPhaseBegan() { return this.touchPhaseBegan; }

    public Controller()
    {
        this.stick = new Vector2[2];
    }
    public void update()
    {
        this.touchPhaseBegan = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Began) continue;
            this.touchPhaseBegan = Main.instance.cam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -Main.instance.transform.position.z));
            break;
        }

        this.stick[1] = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x > 0.5f * Screen.width) continue;
            if (t.phase == TouchPhase.Began) this.stick[0] = t.position;
            this.stick[1] = t.position - this.stick[0];
        }

        this.button = 0;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Began) continue;
            if (t.position.x < 0.75f * Screen.width) continue;
            if (t.position.y > 0.5f * Screen.height) continue;
            this.button |= 1;
            break;
        }

        int c = 0;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x < 0.5f * Screen.width) continue;
            c++;
            if (c < 2) continue;
            this.button |= 2;
            break;
        }

        this.deltaPosition = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase != TouchPhase.Ended) continue;
            if (t.deltaPosition.sqrMagnitude < Mathf.Pow((Screen.width * 0.03f), 2)) continue;
            this.deltaPosition = t.deltaPosition;
            this.button |= 4;
            break;
        }
    }
}