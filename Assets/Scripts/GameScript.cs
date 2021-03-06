﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class GameScript : MonoBehaviour
{
    [SerializeField] private TextAsset dataFile = null;
    [SerializeField] private GameObject pointsParent = null;
    [SerializeField] private TMP_Dropdown levelsDropdown = null;
    [SerializeField] private GameObject levelSelectionScene = null;
    [SerializeField] private GameObject gameScene = null;
    [SerializeField] private GameObject UIObject = null;
    [SerializeField] private GameObject gameBackground = null;
    [SerializeField] private GameObject menuBackground = null;
    [Range(0f, 0.4f)] [SerializeField] private float phoneBezzlesWidth = 0.1f;
    [Range(0.6f, 1.2f)] [SerializeField] private float pointScale = 0.7f;



    private Levels levelsList = null;
    public Level currentLevel = null;

    void Start()
    {
        levelsList = new Levels();
        currentLevel = new Level();
        ValidateGameData();
        PrepareScreen();
        ProcessJSONData();
        PrepareDropdown();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SelectGameLevel();
    }

    #region On Start Functions 

    private void ValidateGameData()
    {
        if (dataFile == null)
        {
            Debug.LogError("JSON Reading Error! No data file attached.");
        }
        if (pointsParent == null)
        {
            Debug.LogError("Error! Points parrent not attached.");
        }
        if (levelsDropdown == null)
        {
            Debug.LogError("Error! Levels dropdown not attached.");
        }
        if (levelSelectionScene == null)
        {
            Debug.LogError("Error! Level selection scene not attached.");
        }
        if (gameScene == null)
        {
            Debug.LogError("Error! Game scene not attached.");
        }
        if (UIObject == null)
        {
            Debug.LogError("Error! UI scene not attached.");
        }

        if (gameBackground == null)
        {
            Debug.LogError("Error! Game background not attached.");
        }
        if (menuBackground == null)
        {
            Debug.LogError("Error! Menu background not attached.");
        }
    }

    /// Function To Prepare Game That Top Left Corner Of The Game Would be (0,0)
    private void PrepareScreen()
    {
        if (!levelSelectionScene.activeSelf)
        {
            Debug.LogError("Level Selection Scene Not Active!");
            levelSelectionScene.SetActive(true);
        }

        if (!menuBackground.activeSelf)
        {
            Debug.LogError("Menu background Is Not Active!");
            menuBackground.SetActive(true);
        }

        if (gameScene.activeSelf)
        {
            Debug.LogError("Game Scene Is Active!");
            gameScene.SetActive(false);
        }

        if (UIObject.activeSelf)
        {
            Debug.LogError("UI Scene Is Active!");
            UIObject.SetActive(false);
        }

        if (gameBackground.activeSelf)
        {
            Debug.LogError("Game background Is Active!");
            gameBackground.SetActive(false);
        }

        //Prepare Screen That Top Left Corner Would be (0,0) 
        Vector2 TRSPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)); // Get top right screen position and move game screen accordingly
        transform.position = TRSPosition;

        //Move Points To Top Left Corner
        Vector2 BLSPosition = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)); // Get bottom left screen position
        pointsParent.transform.position = BLSPosition;
    }

    /// Function To Read JSON File And Process Levels Data
    private void ProcessJSONData()
    {
        Levels readData = new Levels();
        readData = JsonConvert.DeserializeObject<Levels>(dataFile.text);

        if (readData.GetLevelsCount() > 0)
        {
            levelsList.ProcessLevelsData(readData);
        }
        else
        {
            Debug.LogError("JSON Reading Error! Levels Count <= 0, check data file.");
        }
    }


    // Function To Prepare Dropdown With Levels Data
    private void PrepareDropdown()
    {
        if (levelsList.GetLevelsCount() > 0)
        {
            List<string> levelsNames = new List<string>();

            for (int i = 0; i < levelsList.GetLevelsCount(); i++)
            {
                string level = "Level " + (i + 1);
                levelsNames.Add(level);
            }
            levelsDropdown.AddOptions(levelsNames);
        }
        else
        {
            Debug.LogError("Preparing dropdown error! Game does not contain levels.");
        }
    }

    #endregion

    // Function To Start The Game
    public void StartGame()
    {
        currentLevel = levelsList.GetLevelsList()[levelsDropdown.value];
        levelsList.SetCurrentLevel(levelsDropdown.value);

        gameScene.SetActive(true);
        levelSelectionScene.SetActive(false);
        menuBackground.SetActive(false);
        gameBackground.SetActive(true);
    }

    // Function To Return To Level Selection Scene
    public void SelectGameLevel()
    {
        levelSelectionScene.SetActive(true);
        gameScene.SetActive(false);
        UIObject.SetActive(false);
        menuBackground.SetActive(true);
        gameBackground.SetActive(false);
    }

    public void LoadNextLevel()
    {
        if (levelsList.GetNextLevel() != currentLevel)
            currentLevel = levelsList.GetNextLevel();

        gameScene.SetActive(false);
        gameScene.SetActive(true);
        levelSelectionScene.SetActive(false);
    }

    public void SetActiveUI(bool isTrue)
    {
        menuBackground.SetActive(isTrue);
        gameBackground.SetActive(!isTrue);
        UIObject.SetActive(isTrue);
    }

    public float GetPhoneBezzlesWidth()
    {
        return phoneBezzlesWidth;
    }
    public float GetPointScale()
    {
        return pointScale;
    }
}