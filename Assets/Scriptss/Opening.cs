using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Opening : MonoBehaviour
{
    Camera cam;
    int i;
    List<Vector2> list;

    void Awake()
    {
        Application.targetFrameRate = 30;
        Static.texture();
    }

    void Start()
    {
        this.i = 0;
        this.list = new List<Vector2>();

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                GameObject gameObject = (GameObject)Instantiate(Resources.Load("human"), Vector3.zero, Quaternion.identity);
                gameObject.transform.position = new Vector3(2 * x, -3 * y, 0);
                gameObject.transform.localRotation = Quaternion.Euler(0, 30, 0);
                Static.setTexture(gameObject.transform, Static.texList[x + 4 * y]);
                this.list.Add(gameObject.transform.position + Vector3.up);
            }
        }

        this.cam = Camera.main;
        Vector2 v = Vector2.zero;
        foreach (Vector2 l in this.list) v += l;
        this.cam.gameObject.transform.position = (Vector3)v / (float)this.list.Count + new Vector3(0, 0, -400);

        GameObject.Find("Directional Light").transform.position = this.cam.gameObject.transform.position;
    }

    void Update()
    {
        bool b = false;

        if (Input.touchCount > 0)
        {
            Vector2 t = Input.GetTouch(0).position;
            Vector2 v = this.cam.ScreenToWorldPoint(new Vector3(t.x, t.y, -this.cam.gameObject.transform.position.z));
            for (int i = 0; i < this.list.Count; i++)
            {
                if ((v - this.list[i]).sqrMagnitude < 0.75f * 0.75f)
                {
                    Static.playerI = i;
                    b = true;
                    break;
                }
            }
        }

        if (b) this.i++;
        else this.i = 0;

        if (this.i > 30) SceneManager.LoadScene("Scene");
    }
}
