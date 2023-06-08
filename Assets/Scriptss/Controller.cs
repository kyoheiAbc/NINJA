using UnityEngine;
public class Controller
{
    int button; public int getButton() { return this.button; }
    Vector2 deltaPosition; public Vector3 getDeltaPosition() { return new Vector3(this.deltaPosition.x, 0, this.deltaPosition.y); }
    Vector2[] stick; public Vector3 getStick() { return new Vector3(this.stick[1].x, 0, this.stick[1].y); }
    Vector3 touchPhaseBegan; public Vector3 getTouchPhaseBegan() { return this.touchPhaseBegan; }

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
            this.touchPhaseBegan = t.position;
            break;
        }

        this.stick[1] = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x > 0.5f * Screen.width) continue;
            if (t.phase == TouchPhase.Began) this.stick[0] = t.position;
            this.stick[1] = t.position - this.stick[0];
            break;
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
            if (t.position.x < 0.5f * Screen.width) continue;
            if (t.deltaPosition.sqrMagnitude < Mathf.Pow((Screen.width * 0.01f), 2)) continue;
            this.deltaPosition = t.deltaPosition;
            this.button |= 4;
            break;
        }

        if (Input.GetKeyDown(KeyCode.Z)) this.button |= 1;
        if (Input.GetKeyDown(KeyCode.X)) this.button |= 2;
        if (Input.GetKeyDown(KeyCode.C)) this.button |= 4;
    }
}