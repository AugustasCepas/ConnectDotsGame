using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
public class PointsScript : MonoBehaviour
{
    [SerializeField] private GameScript gameScript = null;
    private Level level = null;
    private LinkedList<GameObject> pointsList = null;
    private GameObject currentPoint = null;
    private GameObject currentRope = null;

    void OnEnable()
    {
        PrepareVariables();
        StartLevel();
    }

    #region Level Start Functions

    //Function To Check If Required GameObjects Are Atttached to Script GameObject & Prepare Other Variables
    void PrepareVariables()
    {
        level = new Level();
        pointsList = new LinkedList<GameObject>();

        if (gameScript != null)
        {
            if (gameScript.currentLevel != null)
            {
                level = gameScript.currentLevel;
            }
            else
            {
                Debug.Log("Error! Current Level Is Not Set!");
            }
        }
        else
        {
            gameScript = gameObject.GetComponent<GameScript>();
        }

        if (Resources.Load<GameObject>("Prefab/Point") == null)
        {
            Debug.Log("Error! Point prefab can not be accesed.");
        }
    }

    // Function To Read Level's Points And Set Them In The Game
    public void StartLevel()
    {
        Point point; GameObject childButton;

        if (level.GetPointsCount() >= 1)
        {
            for (int i = 0; i < level.GetPointsCount(); i++)
            {
                if (level.GetPoint(i) != null)
                {
                    point = level.GetPoint(i);
                    childButton = CreatePoint(i + 1, point.GetX(), point.GetY());
                    childButton.GetComponent<PointScript>().SetNotClickable();
                    pointsList.AddLast(childButton);
                }
                else Debug.LogError("Point can't be accesed!");
            }

            currentPoint = pointsList.First.Value;
            currentRope = currentPoint;
            currentPoint.SetActive(true);
            currentPoint.GetComponent<PointScript>().SetClickable();
        }
        else Debug.LogError("Error! There must be more than one point in a game!");
    }

    /// Function To Create Point
    public GameObject CreatePoint(int childNumber, int x, int y)
    {
        GameObject newPoint; PointScript newPointScript;
        var prefabObject = Resources.Load<GameObject>("Prefab/Point");
        newPoint = (GameObject)Instantiate(prefabObject);

        newPoint.name = childNumber.ToString();
        newPoint.transform.parent = gameObject.transform;

        newPointScript = newPoint.GetComponent<PointScript>();
        newPointScript.MovePoint(x, y);

        return newPoint;
    }

    #endregion

    #region Interaction With Points Functions 

    public void OnCorrectPointClicked()
    {
        PointScript currentPointScript;
        PointScript previousPointScript;

        currentPointScript = currentPoint.GetComponent<PointScript>();
        currentPointScript.SetNotClickable();

        LinkedListNode<GameObject> currentPointNode = pointsList.Find(currentPoint);

        // If Point Clicked Is Not First
        if (currentPointNode.Previous != null)
        {
            previousPointScript = currentPointNode.Previous.Value.GetComponent<PointScript>();
            StartCoroutine(DrawRope(previousPointScript, currentPointScript, currentPointNode, false));
        }
        // If Point Clicked Is Not Last
        if (currentPointNode.Next != null)
        {
            currentPoint = currentPointNode.Next.Value;
            currentPoint.GetComponent<PointScript>().SetClickable();
        }
        else
        {
            previousPointScript = currentPointNode.Previous.Value.GetComponent<PointScript>();
            StartCoroutine(DrawRope(previousPointScript, currentPointScript, currentPointNode, true));
        }
    }

    // IEnumerator For Rope Drawing If Next Point Clicked Before Previous Point Rope Drawn
    IEnumerator DrawRope(PointScript previousPointScript, PointScript currentPointScript, LinkedListNode<GameObject> currentPointNode, bool isLast)
    {
        PointScript previousPreviousPointScript;
        RopeScript currentRopeScript = previousPointScript.GetRope().GetComponent<RopeScript>();

        // If Not First Rope Being Drawn
        if (currentPointNode.Previous.Value != pointsList.First.Value && !isLast)
        {
            previousPreviousPointScript = currentPointNode.Previous.Previous.Value.GetComponent<PointScript>();

            RopeScript ropeScript = previousPreviousPointScript.GetRope().GetComponent<RopeScript>();
            while (!ropeScript.IsDrawingFinished())
            {
                yield return null;
            }
        }

        //If Last Point Clicked (Last-1 Rope Being Drawn)
        if (isLast)
        {
            while (!currentRopeScript.IsDrawingFinished())
            {
                yield return null;
            }
            // Set Rope (currentPointScript) From currentRope To pointsList.First.Value 
            TransformRope(pointsList.First.Value, currentPointScript, currentRope, true);

            while (!currentPointScript.GetRope().GetComponent<RopeScript>().IsDrawingFinished())
            {
                yield return null;

            }

            StartCoroutine(EndLevel());
        }
        else
        {
            RopeScript ropeScript = previousPointScript.GetRope().GetComponent<RopeScript>();
            LinkedListNode<GameObject> currentRopeNode = pointsList.Find(currentRope);
            currentRope = currentRopeNode.Next.Value;

            // Set Rope (previousPointScript) From currentPointNode.Previous.Value To currentRope
            TransformRope(currentPointNode.Previous.Value, previousPointScript, currentRope, false);
        }
    }

    IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(2);

        DeletePoints();
        gameScript.SelectGameLevel();

    }

    public void DeletePoints()
    {
        if (gameObject.transform.childCount > 0)
        {
            foreach (Transform child in gameObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    #endregion

    public LinkedList<GameObject> GetPointsList()
    {
        return pointsList;
    }

    //Function To Transform Rope's Angle Length And Position
    public void TransformRope(GameObject nextPoint, PointScript currentPointScript, GameObject currentPoint, bool last)
    {

        Vector2 middlePosition = new Vector2();
        float ropeLength = 0;
        float angle = 0;
        Calculations calculations = new Calculations();
        RopeScript currentRopeScript = currentPointScript.GetRope().GetComponent<RopeScript>();

        // Set Rope Middle Position
        middlePosition = calculations.GetMiddlePosition(currentPoint.transform.position, nextPoint.transform.position, last);
        currentRopeScript.SetMiddle(middlePosition);

        // Set Rope Length
        SpriteRenderer spriteRender = currentPointScript.GetRope().GetComponent<SpriteRenderer>();
        ropeLength = calculations.GetRopeLength(middlePosition, currentPoint.transform);
        currentRopeScript.SetRopeLength(ropeLength);

        // Set Rope Angle
        angle = calculations.GetRopeAngle(currentPoint.transform, nextPoint.transform.position);
        currentRopeScript.SetRopeAngle(angle);
        currentRopeScript.gameObject.SetActive(true);
    }
}