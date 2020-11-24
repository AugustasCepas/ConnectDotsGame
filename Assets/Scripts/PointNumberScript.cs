using UnityEngine;
using System.Collections.Generic;

public class PointNumberScript : MonoBehaviour
{
    private float textDistFromPoint = 0;
    private GameObject thisPointObj;
    [SerializeField] private Transform closestObjTransform;
    [SerializeField] private Transform secondClosestObjTransform;
    [SerializeField] private bool doesNotCollideWithClosest;

    [Range(0.9f, 1f)] [SerializeField] private float screenUsage = (float)0.98; // 98% Of Screen Can Be Used To Place Point Number Text

    private void Start()
    {
        SetStartingPosition();
        MoveToScreenIfOut();
    }

    // Function To Get Closest Points And Position Text Accordingly
    private void SetStartingPosition()
    {
        PointScript thisPointScript = null;
        PointsScript thisPointsScript = null;
        LinkedList<GameObject> pointsList = null;

        thisPointScript = gameObject.GetComponentInParent(typeof(PointScript), true) as PointScript;
        thisPointObj = thisPointScript.gameObject;
        thisPointsScript = thisPointScript.GetPointsScript();
        pointsList = thisPointsScript.GetPointsList();

        GetTextDistanceFromPoint(thisPointsScript);
        GetClosestPoints(pointsList.First);
        SetTextPosition();
    }
    private void GetTextDistanceFromPoint(PointsScript ps)
    {
        textDistFromPoint = transform.parent.GetComponent<CircleCollider2D>().radius;
        textDistFromPoint *= ps.GetPointScale();
    }

    #region  GetClosestsPoints

    // Function To Get Two Closest Points, Second Is For A Reason If ClosestPoint.X|Y == ThisPoint.X|Y
    private void GetClosestPoints(LinkedListNode<GameObject> firstPointNode)
    {
        float distFromClosestObj = float.MaxValue;
        float distFromSecondClosestObj = float.MaxValue;
        float distFromCurrentObj = float.MinValue;

        while (firstPointNode != null)
        {
            if (firstPointNode.Value.gameObject != thisPointObj)
            {
                distFromCurrentObj = Vector3.Distance(firstPointNode.Value.transform.position, transform.position);

                // If Current Point Is Closer Than Closest One Yet
                if (distFromCurrentObj < distFromClosestObj)
                {
                    if (closestObjTransform != null)
                    {
                        if (ArePointsNotInSingleLine(firstPointNode.Value.transform.position))
                        {
                            secondClosestObjTransform = closestObjTransform;
                        }
                    }

                    closestObjTransform = firstPointNode.Value.transform;
                    distFromClosestObj = distFromCurrentObj;
                }

                // If Current Point Is Closer Than Closest One Yet, But Closer Then Second Closest Yet
                else if (distFromCurrentObj < distFromSecondClosestObj)
                {
                    if (ArePointsNotInSingleLine(firstPointNode.Value.transform.position))
                    {
                        secondClosestObjTransform = firstPointNode.Value.transform;
                        distFromSecondClosestObj = distFromCurrentObj;
                    }
                }
            }
            firstPointNode = firstPointNode.Next;
        }
    }

    // Function For Situation If Closest Point X|Y == This Point X|Y
    // To Check If ThisPoint, ClosestObject & 2ndClosestObject Are Not In One Line
    private bool ArePointsNotInSingleLine(Vector3 currentPointNodePos)
    {
        Vector3 closestObjectPos = closestObjTransform.transform.position;
        Vector3 thisPointObjPos = thisPointObj.transform.position;

        if (!AreEqual(closestObjectPos.x, currentPointNodePos.x) && !AreEqual(thisPointObjPos.y, currentPointNodePos.y))
        {
            return true;
        }
        else if (!AreEqual(closestObjectPos.y, currentPointNodePos.y) && !AreEqual(thisPointObjPos.x, currentPointNodePos.x))
        {
            return true;
        }
        return false;
    }

    #endregion

    #region MoveTextAccordingToClosestPoints

    private void TranslateObjectXY(Transform objectToTranslate, float x, float y)
    {
        objectToTranslate.transform.Translate(x, y, 0);
    }
    // Function To Place Text Position According To Closest Points
    private void SetTextPosition()
    {
        float thisPointX = thisPointObj.transform.position.x;
        float thisPointY = thisPointObj.transform.position.y;

        float XDistanceBetweenThisAndClosest = XDifference(thisPointObj.transform, closestObjTransform);
        float XDistanceBetweenThisAndSecond = XDifference(thisPointObj.transform, secondClosestObjTransform);

        float YDistanceBetweenThisAndClosest = YDifference(thisPointObj.transform, closestObjTransform);
        float YDistanceBetweenThisAndSecond = YDifference(thisPointObj.transform, secondClosestObjTransform);

        if (!IsInLine(closestObjTransform.transform.position.x, thisPointX, secondClosestObjTransform.transform.position.x))
        {
            float xMovement = GetMovementPosititon(XDistanceBetweenThisAndClosest, XDistanceBetweenThisAndSecond, thisPointX);
            float yMovement = GetMovementPosititon(YDistanceBetweenThisAndClosest, YDistanceBetweenThisAndSecond, thisPointY);
            if (xMovement == 0)
                Debug.LogError("Error! Point number X movement can not be 0.");
            else
                MoveAtXAxis(xMovement);

            if (yMovement == 0)
                Debug.LogError("Error! Point number Y movement can not be 0.");
            else
                MoveAtYAxis(yMovement);
        }
    }

    private bool IsInLine(float a, float b, float c)
    {
        if ((a > b && b > c) || (a < b && b < c))
        {
            int i = 0;
            List<Transform> allObjects = new List<Transform>();

            foreach (Transform child in transform)
            {
                float xMovement = -textDistFromPoint;
                float yMovement = -textDistFromPoint;
                allObjects.Add(child);

                if (i % 2 == 1)
                    xMovement *= -1;

                if (i > 1)
                    yMovement *= -1;

                TranslateObjectXY(child, xMovement, yMovement);
                i++;
            }

            float minDistance = 0; int fahrestLocation = 0;
            for (i = 0; i < allObjects.Count; i++)
            {
                float distFromClosestObj = Vector3.Distance(allObjects[i].transform.position, closestObjTransform.position);
                float distFromSecondClosestObj = Vector3.Distance(allObjects[i].transform.position, secondClosestObjTransform.position);

                if (distFromClosestObj > minDistance && distFromSecondClosestObj > (textDistFromPoint * (float)1.1))
                {
                    minDistance = distFromClosestObj;
                    fahrestLocation = i;
                }
            }
            transform.position = allObjects[fahrestLocation].transform.position;

            return true;
        }
        return false;
    }
    private float GetMovementPosititon(float distFromThisToClosest, float distFromThisToSecond, float thisPointPosition)
    {
        float distFromThisToClosestABS = Mathf.Abs(distFromThisToClosest);
        float distFromThisToSecondABS = Mathf.Abs(distFromThisToSecond);

        // If ClosestPoint.X != ThisPoint.X
        if (distFromThisToClosest != 0)
        {
            if (secondClosestObjTransform != null)
            {
                if (distFromThisToClosest > 0 && distFromThisToSecond > 0)
                {
                    return textDistFromPoint;
                }
                else if (distFromThisToClosest < 0 && distFromThisToSecond < 0)
                {
                    return -textDistFromPoint;
                }

                // If Three Points Are In One Line
                else if (distFromThisToClosestABS < distFromThisToSecondABS)
                {
                    if (distFromThisToClosest > thisPointPosition)
                        return -textDistFromPoint;
                    else
                        return textDistFromPoint;

                }
                else
                {
                    if (distFromThisToSecond > thisPointPosition)
                        return -textDistFromPoint;
                    else
                        return textDistFromPoint;
                }
            }
        }

        // If Closest Object X/Y == This Object X/Y, Then Place Text X/Y According 2nd Closest
        else
        {
            if (secondClosestObjTransform != null)
            {
                if (distFromThisToSecond != 0)
                {
                    if (distFromThisToSecond > 0)
                    {
                        return textDistFromPoint;
                    }
                    else
                    {
                        return -textDistFromPoint;
                    }
                }
            }

        }
        return 0;
    }

    // X Difference Between Script's Object And Parameter Given Object 
    private float XDifference(Transform a, Transform b)
    {
        return a.position.x - b.position.x;
    }

    // Y Difference Between Script's Object And Parameter Given Object 
    private float YDifference(Transform a, Transform b)
    {
        return a.position.y - b.position.y;
    }

    private bool AreEqual(float a, float b)
    {
        return a == b;
    }

    #endregion


    #region MoveToScreenIfOut Functions

    // Function To Get If Text Is Out Of Map
    private void MoveToScreenIfOut()
    {
        float XDistanceFromMiddle = 0;
        float YDistanceFromMiddle = 0;
        float screenWidth = (float)Screen.width;
        float screenHeight = (float)Screen.height;

        Vector3 thisObjectCoords = Camera.main.WorldToScreenPoint(transform.position);

        XDistanceFromMiddle = Vector3.Distance(new Vector3((screenWidth / 2), 0f, 0f), new Vector3(thisObjectCoords.x, 0f, 0f));
        YDistanceFromMiddle = Vector3.Distance(new Vector3(0f, (screenHeight / 2), 0f), new Vector3(0f, thisObjectCoords.y, 0f));

        float maxWidth = screenWidth / 2 * screenUsage;
        float maxHeight = screenHeight / 2 * screenUsage;

        if (IsBigger(XDistanceFromMiddle, maxWidth) || IsBigger(YDistanceFromMiddle, maxHeight))
        {
            // Is Object At X Axis Is Out Of Screen
            if (IsBigger(XDistanceFromMiddle, maxWidth))
            {
                if (IsOutAtBegining(screenWidth, thisObjectCoords.x, maxWidth))
                {
                    MoveAtXAxis(2 * textDistFromPoint);
                }
                else if (IsOutAtFinish(thisObjectCoords.x, maxWidth))
                {
                    MoveAtXAxis(-2 * textDistFromPoint);
                }
            }

            // If Object At Y Axis Is Out Of Screen
            if (IsBigger(YDistanceFromMiddle, maxHeight))
            {
                if (IsOutAtBegining(screenHeight, thisObjectCoords.y, maxHeight))
                {
                    MoveAtYAxis(2 * textDistFromPoint);
                }
                else if (IsOutAtFinish(thisObjectCoords.y, maxHeight))
                {
                    MoveAtYAxis(-2 * textDistFromPoint);
                }
            }
        }
    }

    // Is Object Is Out Of Screen
    private bool IsBigger(float a, float b)
    {
        return a > b;
    }

    // Function To Check If Object Is Out Of Screen At Begining (Left/Bottom Corner)
    private bool IsOutAtBegining(float screenLimitation, float objectCoordinate, float textMaxPosition)
    {
        return screenLimitation - textMaxPosition > objectCoordinate;
    }

    // Function To Check If Object Is Out Of Screen At Finish (Right/Top Corner)
    private bool IsOutAtFinish(float objectCoordinate, float textMaxPosition)
    {
        return objectCoordinate > textMaxPosition;
    }

    #endregion

    private void MoveAtYAxis(float value)
    {
        transform.Translate(0, value, 0);
    }

    private void MoveAtXAxis(float value)
    {
        transform.Translate(value, 0, 0);
    }
}
