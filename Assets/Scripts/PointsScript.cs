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

    private string prefabPath = "Assets/Prefab/Point.prefab";

    void OnEnable()
    {
        PrepareVariables();
        StartLevel();
    }

    #region On Enable Functions

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

        if ((GameObject)AssetDatabase.LoadMainAssetAtPath(prefabPath) == null)
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

    #endregion

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

    IEnumerator DrawRope(PointScript previousPointScript, PointScript currentPointScript, LinkedListNode<GameObject> currentPointNode, bool isLast)
    {
        PointScript previousPreviousPointScript;
        RopeScript currentRopeScript = previousPointScript.GetRope().GetComponent<RopeScript>();

        // If Not First
        if (currentPointNode.Previous.Value != pointsList.First.Value)
        {
            previousPreviousPointScript = currentPointNode.Previous.Previous.Value.GetComponent<PointScript>();

            RopeScript ropeScript = previousPreviousPointScript.GetRope().GetComponent<RopeScript>();
            while (!ropeScript.IsDrawingFinished())
            {
                yield return null;
            }
        }

        //If Last
        if (isLast)
        {
            while (!currentRopeScript.IsDrawingFinished())
            {
                yield return null;
            }

            currentRopeScript.TransformRope(pointsList.First.Value, currentPointScript, currentRope, true);

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

            ropeScript.TransformRope(currentPointNode.Previous.Value, previousPointScript, currentRope, false);
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

    /// Function To Create Point
    public GameObject CreatePoint(int childNumber, int x, int y)
    {
        GameObject newPoint; PointScript newPointScript;
        var prefabObject = (GameObject)AssetDatabase.LoadMainAssetAtPath(prefabPath);
        newPoint = (GameObject)PrefabUtility.InstantiatePrefab(prefabObject);
        newPoint.name = childNumber.ToString();
        newPoint.transform.parent = gameObject.transform;

        newPointScript = newPoint.GetComponent<PointScript>();
        newPointScript.MovePoint(x, y);

        return newPoint;
    }
}