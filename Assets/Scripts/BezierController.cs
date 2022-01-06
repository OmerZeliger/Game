using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierController : MonoBehaviour
{
    public LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3[] trail(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        //lr.enabled = true;
        //lr.positionCount = 20;

        Vector3[] positions = new Vector3[20];
        for (int i = 0; i < 20; i++)
        {
            Vector2 temp = Bezier(a, b, c, d, (float)i / 19);
            positions[i] = new Vector3(temp.x, temp.y, 0);
        }
        //lr.SetPositions(positions);
        return positions;
    }


    private Vector2 Bezier(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    private Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    private Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }
}
