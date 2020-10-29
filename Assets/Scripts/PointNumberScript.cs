using UnityEngine;
using TMPro;

public class PointNumberScript : MonoBehaviour
{
    private TMP_Text textObject;
    private float distanceFromCenterX = 0;
    private float distanceFromCenterY = 0;
    private int halfWidth = 0;
    private int halfHeight = 0;
    private float pointRadius = 0;

    // Start is called before the first frame update
    void Start()
    {
        textObject = GetComponent<TMP_Text>();
        pointRadius = transform.parent.GetComponent<CircleCollider2D>().radius;
        transform.Translate(pointRadius, 0, 0);

        halfWidth = Screen.width / 2;
        halfHeight = Screen.height / 2;

        IsOutOfMap();
    }

    // Update is called once per frame
    void Update()
    {
        // IsOutOfMap();
    }
    void IsOutOfMap()
    {
        Vector3 coordinates = Camera.main.WorldToScreenPoint(transform.position);
        distanceFromCenterX = Vector3.Distance(new Vector3(halfWidth, 0f, 0f), new Vector3(coordinates.x, 0f, 0f));
        distanceFromCenterY = Vector3.Distance(new Vector3(0f, halfHeight, 0f), new Vector3(0f, coordinates.y, 0f));


        if (OutOfMap(distanceFromCenterX, halfWidth, distanceFromCenterY, halfHeight))
        {
            if (OutOfMapX(distanceFromCenterX, halfWidth))
            {
                if (OutOfMapAtLeft(coordinates, halfWidth))
                {
                    transform.Translate(pointRadius, 0, 0);
                }
                else
                {
                    transform.Translate(-pointRadius, 0, 0);
                }
            }
            else
            {
                if (OutOfMapAtBottom(coordinates, halfHeight))
                {
                    transform.Translate(0, -pointRadius, 0);
                }
                else
                {
                    transform.Translate(0, pointRadius, 0);
                }
            }
        }

    }
    static bool OutOfMap(float distanceFromCenterX, int halfWidth, float distanceFromCenterY, int halfHeight)
    {
        return distanceFromCenterX > halfWidth || distanceFromCenterY > halfHeight;
    }
    static bool OutOfMapX(float distanceFromCenterX, int halfWidth)
    {
        return distanceFromCenterX > halfWidth;
    }
    static bool OutOfMapAtLeft(Vector3 coordinates, int halfWidth)
    {
        return halfWidth - coordinates.x > halfWidth;
    }
    static bool OutOfMapAtBottom(Vector3 coordinates, int halfHeight)
    {
        return halfHeight - coordinates.y < halfHeight;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "PointNumber")
        {
            TMP_Text otherObject = other.gameObject.GetComponent<TMP_Text>();
            TMP_Text thisObject = this.gameObject.GetComponent<TMP_Text>();
            int otherObjectInt = int.Parse(otherObject.text);
            int thisObjectInt = int.Parse(thisObject.text);


            if (thisObjectInt < otherObjectInt)
            {
                transform.Translate((-2 * pointRadius), 0, 0);
            }
        }
    }
}
