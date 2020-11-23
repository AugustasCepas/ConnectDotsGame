using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointScript : MonoBehaviour
{
    [SerializeField] private Sprite blueSprite = null;
    private bool isClickable = false;
    private GameObject parentObject = null;
    private TMP_Text textObject = null;
    private PointsScript pointsScript = null;
    private GameObject rope = null;
    private Animator animator;
    private AudioSource audioSource;
    private float phoneBezzlesWidth = 0;
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        ValidateVariables();
        Initialise();
    }

    void Update()
    {
        if (isClickable)
            OnMouseClick();
    }

    private void ValidateVariables()
    {
        if (blueSprite == null)
        {
            Debug.LogError("Error! Sprite not attached.");
        }
        if (textObject == null)
        {
            textObject = gameObject.GetComponentInChildren(typeof(TMP_Text), true) as TMP_Text;
        }
        if (parentObject == null)
        {
            parentObject = this.gameObject.transform.parent.gameObject;
        }
        if (pointsScript == null)
        {
            pointsScript = gameObject.GetComponentInParent(typeof(PointsScript), true) as PointsScript;
        }
        if (rope == null)
        {
            rope = this.transform.Find("Rope").gameObject;
        }
    }
    private void Initialise()
    {
        if (transform.parent != parentObject)
        {
            transform.parent = parentObject.transform;
        }

        textObject.text = gameObject.name;

        Renderer textRenderer = GetComponent<Renderer>();
        int.TryParse(gameObject.name, out int x);
        textRenderer.sortingOrder = -x;
    }

    void OnMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Point")
                {
                    if (hit.collider.gameObject.GetComponent<PointScript>().isClickable)
                    {
                        animator.SetTrigger("Disappear");
                        audioSource.Play();
                        Renderer textRenderer = GetComponent<Renderer>();
                        textRenderer.sortingLayerName = "ClickedPoints";
                        pointsScript.OnCorrectPointClicked();

                        if (hit.collider.GetComponent<CircleCollider2D>() != null)
                        {
                            Destroy(hit.collider.GetComponent<CircleCollider2D>());
                        }
                    }

                }
            }
        }
    }

    public void SetClickable()
    {
        isClickable = true;
    }

    public void SetNotClickable()
    {
        isClickable = false;
    }

    public GameObject GetRope()
    {
        return rope;
    }

    public void SetPhoneBezzlesWidth(float bezzleWidth)
    {
        phoneBezzlesWidth = bezzleWidth;
        // if (bezzleWidth >= 0)
        // else
        //             Debug.LogError("Phone bezzles width must be >= 0!");
    }

    /// Function To Move Point From Top Left Corner Accordingly To X And Y
    public void MovePoint(int xPos, int yPos)
    {
        float usableScreenWidth = 1f - (2 * phoneBezzlesWidth);
        float screenWidthStart = Screen.width * phoneBezzlesWidth;

        float x = (float)Screen.width * (float)xPos / 1000;
        float y = (float)Screen.height * (float)yPos / 1000;

        x *= usableScreenWidth;
        x += screenWidthStart;

        Vector3 BRSPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0)); // Get bottom right screen position and move pointsList accordingly

        transform.position = new Vector2(BRSPosition.x, BRSPosition.y);
    }

    public PointsScript GetPointsScript()
    {
        return pointsScript;
    }
}