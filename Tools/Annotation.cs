using Avalonia;
using System;
using System.Collections.Generic;

namespace ASTEM_DB.Tools;

public class Annotation
{
    public string Id { get; }

    public string Type { get; set; }

    public List<Point> Bounds { get; set; }

    public List<Point> Points { get; set; }

    public string Name { get; set; }

    public Annotation(
        string type,
        IEnumerable<Point> bounds,
        IEnumerable<Point> points,
        string name)
    {
        Id = Guid.NewGuid().ToString();

        Type = type;
        Bounds = new List<Point>(bounds);
        Points = new List<Point>(points);
        Name = name;
    }
}
