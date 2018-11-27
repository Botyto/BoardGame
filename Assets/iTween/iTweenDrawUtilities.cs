using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class iTweenDrawUtilities
{
    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Vector3s.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawLine(Vector3[] line)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Vector3s.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLine(Vector3[] line, Color color)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Transforms.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawLine(Transform[] line)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }
            DrawLineHelper(suppliedLine, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Transforms.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLine(Transform[] line, Color color)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }

            DrawLineHelper(suppliedLine, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawLineGizmos(Vector3[] line)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineGizmos(Vector3[] line, Color color)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawLineGizmos(Transform[] line)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }
            DrawLineHelper(suppliedLine, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineGizmos(Transform[] line, Color color)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }

            DrawLineHelper(suppliedLine, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawLineHandles(Vector3[] line)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, iTween.Defaults.color, "handles");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineHandles(Vector3[] line, Color color)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, color, "handles");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawLineHandles(Transform[] line)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }
            DrawLineHelper(suppliedLine, iTween.Defaults.color, "handles");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineHandles(Transform[] line, Color color)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }

            DrawLineHelper(suppliedLine, color, "handles");
        }
    }

    /// <summary>
    /// Returns a Vector3 position on a path at the provided percentage  
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    /// <returns>
    /// A <see cref="Vector3"/>
    /// </returns>
    public static Vector3 PointOnPath(Vector3[] path, float percent)
    {
        return iTween.Interp(iTween.PathControlPointGenerator(path), percent);
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Vector3s.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawPath(Vector3[] path)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Vector3s.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPath(Vector3[] path, Color color)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Transforms.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawPath(Transform[] path)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Transforms.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPath(Transform[] path, Color color)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawPathGizmos(Vector3[] path)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathGizmos(Vector3[] path, Color color)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawPathGizmos(Transform[] path)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, iTween.Defaults.color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathGizmos(Transform[] path, Color color)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawPathHandles(Vector3[] path)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, iTween.Defaults.color, "handles");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathHandles(Vector3[] path, Color color)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, color, "handles");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawPathHandles(Transform[] path)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, iTween.Defaults.color, "handles");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathHandles(Transform[] path, Color color)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, color, "handles");
        }
    }

    private static void DrawLineHelper(Vector3[] line, Color color, string method)
    {
        Gizmos.color = color;
        for (int i = 0; i < line.Length - 1; i++)
        {
            if (method == "gizmos")
            {
                Gizmos.DrawLine(line[i], line[i + 1]); ;
            }
            else if (method == "handles")
            {
                Debug.LogError("iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
                //UnityEditor.Handles.DrawLine(line[i], line[i+1]);
            }
        }
    }

    private static void DrawPathHelper(Vector3[] path, Color color, string method)
    {
        Vector3[] vector3s = iTween.PathControlPointGenerator(path);

        var points = new Vector3[vector3s.Length - 2];
        for (int i = 0; i < vector3s.Length - 2; ++i)
        {
            points[i] = vector3s[i + 1];
        }
        
        //Line Draw:
        Vector3 prevPt = iTween.Interp(points, 0);
        Gizmos.color = color;
        int SmoothAmount = path.Length * 20;
        for (int i = 1; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = iTween.Interp(points, pm);
            if (method == "gizmos")
            {
                Gizmos.DrawLine(currPt, prevPt);
            }
            else if (method == "handles")
            {
                //Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
              //  UnityEditor.Handles.DrawLine(currPt, prevPt);
            }
            prevPt = currPt;
        }
    }
}
