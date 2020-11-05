using UnityEngine;

public class Calculations
{
    private Vector2 middlePosition;
    private float ropeLength = 0;
    private float angle = 0;


    // Function To Return Middle Position Between Objects
    public Vector2 GetMiddlePosition(Vector3 positionA, Vector3 positionB, bool last)
    {
        middlePosition = new Vector2();
        if (!last)
        {
            middlePosition = positionA - positionB;
        }
        else
        {
            middlePosition = positionB - positionA;
        }
        middlePosition *= 0.5f;

        return middlePosition;
    }

    // Function To Return Rope Length
    public float GetRopeLength(Vector3 middlePosition, Transform currentPoint)
    {
        ropeLength = Mathf.Sqrt(Mathf.Pow(middlePosition.x, 2) + Mathf.Pow(middlePosition.y, 2));

        if (currentPoint.localScale.x != currentPoint.localScale.y)
            Debug.LogError("Point X.Scale != Y.Scale");

        ropeLength /= currentPoint.localScale.x;
        ropeLength *= 2;

        return ropeLength;
    }

    // Function to Return Rope Angle
    public float GetRopeAngle(Transform currentPoint, Vector3 nextPosition)
    {
        Vector3 dir = currentPoint.position - nextPosition;
        dir = currentPoint.InverseTransformDirection(dir);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return angle;
    }
}
