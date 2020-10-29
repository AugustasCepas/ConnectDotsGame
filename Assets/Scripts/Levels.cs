using System.Collections.Generic;
using UnityEngine;

public class Levels
{
    public List<Level> levels { get; private set; }

    public Levels()
    {
        levels = new List<Level>();
    }

    public void AddLevel(Level l)
    {
        levels.Add(l);
    }

    public List<Level> GetLevelsList()
    {
        return levels;
    }

    public int GetLevelsCount()
    {
        return levels.Count;
    }

    //Function To Process Data Document Points To Level Array And Then To Levels Array
    public void ProcessLevelsData(Levels readData)
    {
        const int dimensions = 2;
        foreach (Level level in readData.levels)
        {
            if (level.GetLevelDataCount() > 0 && level.GetLevelDataCount() % dimensions == 0)
            {
                level.CreatePointsList();
                for (int i = 0; i < level.GetLevelDataCount(); i += dimensions)
                {
                    if (IsWithin(level.level_data[i], 0, 1000) && IsWithin(level.level_data[i + 1], 0, 1000))
                    {
                        level.AddPoint(new Point(level.level_data[i], level.level_data[i + 1]));
                    }
                    else Debug.LogError("Level Data Processing Error! Level Point out of boundaries.");
                }
                AddLevel(level);
            }
            else Debug.LogError("Level Data Processing Error! Level points count <= 0 or %2 != 0, check data file.");
        }
    }

    public bool IsWithin(int value, int minimum, int maximum)
    {
        return value >= minimum && value <= maximum;
    }
}