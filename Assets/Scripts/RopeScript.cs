using UnityEngine;
using System;

public class RopeScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float counter = 0;
    private float distanceBetweenPoints;
    private float drawSpeed = 1f;
    private float ropeLength;
    private Vector2 middle;
    private Vector2 startPosition;
    private Vector2 positionsDifference;
    private bool drawingFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetDesiredRopeData();
        SetSpriteSize(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!drawingFinished)
            DrawRope();
    }

    #region Get Desired Rope Functions 
    private void GetDesiredRopeData()
    {
        Vector2 finishPosition;

        distanceBetweenPoints = 2 * (Math.Abs(middle.x) + Math.Abs(middle.y));
        ropeLength /= distanceBetweenPoints;

        startPosition = transform.position;
        finishPosition = GetFinishPosition();

        positionsDifference = (finishPosition - startPosition) / distanceBetweenPoints;
    }

    private Vector2 GetFinishPosition()
    {
        GameObject ropeCopy = new GameObject(); Vector2 finishPosition;

        ropeCopy.transform.position = transform.position;
        ropeCopy.transform.Translate(middle, Space.World);

        finishPosition = ropeCopy.transform.position;
        Destroy(ropeCopy);

        return finishPosition;
    }

    #endregion

    #region Rope Drawing Functions

    private void DrawRope()
    {
        float x = Mathf.Lerp(0, distanceBetweenPoints, counter);
        if (x < distanceBetweenPoints)
        {
            counter += Time.deltaTime * drawSpeed;
            SetSpriteSize(ropeLength * x);
            transform.position = positionsDifference * x + startPosition;
        }
        else
        {
            drawingFinished = true;
        }
    }

    #endregion

    private void SetSpriteSize(float currentLength)
    {
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, currentLength);
    }

    public void SetRopeLength(float length)
    {
        ropeLength = length;
    }

    public void SetRopeAngle(float angle)
    {
        transform.Rotate(0f, 0f, angle + 90f);
    }

    public void SetMiddle(Vector2 m)
    {
        middle = m;
    }
    public bool IsDrawingFinished()
    {
        return drawingFinished;
    }
}
