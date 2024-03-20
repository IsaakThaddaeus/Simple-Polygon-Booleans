using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class SimplePolygonBooleans
{
    public static List<List<Vector2>> Union(List<Vector2> polygonA, List<Vector2> polygonB, out List<LineSegment> segmentsA, out List<LineSegment> segmentsB)
    {
        List<Vector2> verticesA = new List<Vector2>(polygonA);
        List<Vector2> verticesB = new List<Vector2>(polygonB);

        intersectPolygons(verticesA, verticesB);
        segmentsA = getLineSegments(verticesA);
        segmentsB = getLineSegments(verticesB);

        List<LineSegment> segments = selectEdges(segmentsA, segmentsB, polygonA, polygonB);
        List<List<Vector2>> outputPolygon = connectEdges(segments);



        //DEBUGGING -- WILL BE REMOVED
        Debug.Log("--A--");
        foreach (LineSegment segmentA in segmentsA)
        {
            Debug.Log(segmentA.start + " " + segmentA.end);
        }
        Debug.Log("--B--");
        foreach (LineSegment segmentB in segmentsB)
        {
            Debug.Log(segmentB.start + " " + segmentB.end);
        }


        return null;
    }


    static void intersectPolygons(List<Vector2> verticesA, List<Vector2> verticesB)
    {
        for (int i = 0; i < verticesA.Count; i++)
        {
            for (int j = 0; j < verticesB.Count; j++)
            {
                bool inter = LineIntersector.intersectionPoint(verticesA[i], verticesA[(i + 1) % verticesA.Count], verticesB[j], verticesB[(j + 1) % verticesB.Count], out Vector2 intersection);

                if (inter == true && !verticesA.Any(v2 => v2 == intersection))
                {
                    verticesA.Insert(i + 1, intersection);
                }
            }
        }

        for (int i = 0; i < verticesB.Count; i++)
        {
            for (int j = 0; j < verticesA.Count; j++)
            {
                bool inter = LineIntersector.intersectionPoint(verticesB[i], verticesB[(i + 1) % verticesB.Count], verticesA[j], verticesA[(j + 1) % verticesA.Count], out Vector2 intersection);

                if (inter == true && !verticesB.Any(v2 => v2 == intersection))
                {
                    verticesB.Insert(i + 1, intersection);
                }
            }
        }

    }
    static List<LineSegment> selectEdges(List<LineSegment> segmentsA, List<LineSegment> segmentsB, List<Vector2> polygonA, List<Vector2> polygonB)
    {
        for (int i = 0; i < segmentsA.Count; i++)
        {
            Vector2 midPoint = segmentsA[i].start + (segmentsA[i].end - segmentsA[i].start) * 0.5f;
            if (PointInPolygon.insideAngle(midPoint, polygonB, 0f))
            {
                segmentsA.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < segmentsB.Count; i++)
        {
            Vector2 midPoint = segmentsB[i].start + (segmentsB[i].end - segmentsB[i].start) * 0.5f;
            if (PointInPolygon.insideAngle(midPoint, polygonA, 0f))
            {
                segmentsB.RemoveAt(i);
                i--;
            }
        }

        List<LineSegment> segments = new List<LineSegment>();
        segments.AddRange(segmentsA);
        segments.AddRange(segmentsB);
        return segments;
    }
    static List<List<Vector2>> connectEdges(List<LineSegment> segments)
    {
        return null;
    }
    static List<LineSegment> getLineSegments(List<Vector2> vertices)
    {
        List<LineSegment> segments = new List<LineSegment>();
        for(int i = 0; i < vertices.Count; i++)
        {
            segments.Add(new LineSegment(vertices[i], vertices[(i + 1) % vertices.Count]));
        }
        return segments;
    }
}

public class LineSegment
{
    public Vector2 start;
    public Vector2 end;

    public LineSegment(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
}
