using Avalonia;
using System;
using System.Collections.Generic;

namespace ASTEM_DB.Tools;

public readonly record struct ViewportState(
    double X,
    double Y,
    double Scale
);

public static class CoordinateHelpers
{
    /// <summary>
    /// Converts screen coordinates to world coordinates.
    /// Migrated from glazer_admin/src/tools/helpers.ts.
    /// </summary>
    public static Point ScreenToWorld(
        Point position,
        ViewportState viewport,
        Rect canvasRect)
    {
        if (viewport.Scale <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(viewport),
                "Viewport scale must be greater than zero.");
        }

        double x =
            (position.X - canvasRect.X) / viewport.Scale
            - viewport.X;

        double y =
            (position.Y - canvasRect.Y) / viewport.Scale
            - viewport.Y;

        return new Point(x, y);
    }

    /// <summary>
    /// Converts world coordinates to screen coordinates.
    /// Migrated from glazer_admin/src/tools/helpers.ts.
    /// </summary>
    public static Point WorldToScreen(
        Point world,
        ViewportState viewport,
        Rect canvasRect)
    {
        double x =
            (world.X + viewport.X) * viewport.Scale
            + canvasRect.X;

        double y =
            (world.Y + viewport.Y) * viewport.Scale
            + canvasRect.Y;

        return new Point(x, y);
    }

    /// <summary>
    /// Determines whether a world position lies inside a bounding box.
    /// The bounds may be supplied in any corner order.
    /// </summary>
    public static bool InBounds(
        Point position,
        IReadOnlyList<Point> bounds)
    {
        if (bounds.Count < 2)
        {
            return false;
        }

        double minX = Math.Min(bounds[0].X, bounds[1].X);
        double maxX = Math.Max(bounds[0].X, bounds[1].X);
        double minY = Math.Min(bounds[0].Y, bounds[1].Y);
        double maxY = Math.Max(bounds[0].Y, bounds[1].Y);

        return
            minX <= position.X &&
            position.X <= maxX &&
            minY <= position.Y &&
            position.Y <= maxY;
    }
}
