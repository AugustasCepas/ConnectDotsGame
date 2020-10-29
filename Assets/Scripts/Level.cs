using System.Collections.Generic;

public class Level
{
    public List<int> level_data { get; set; }

    private List<Point> points;

    public void CreatePointsList()
    {
        points = new List<Point>();
    }

    public void AddPoint(Point p)
    {
        points.Add(p);
    }

    public Point GetPoint(int i)
    {
        return points[i];
    }

    public int GetPointsCount()
    {
        return points.Count;
    }

    public int GetLevelDataCount()
    {
        return level_data.Count;
    }
}