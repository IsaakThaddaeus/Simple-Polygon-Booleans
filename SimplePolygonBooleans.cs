using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class SimplePolygonBooleans
{
    public static List<List<Vector2>> Add(List<Vector2> polygonA, List<Vector2> polygonB)
    {
        List<Vector2> verticesA = new List<Vector2>(polygonA);
        List<Vector2> verticesB = new List<Vector2>(polygonB);

        intersectPolygons(verticesA, verticesB);
        List<LineSegment>  segmentsA = getLineSegments(verticesA);
        List<LineSegment>  segmentsB = getLineSegments(verticesB);

        List<LineSegment> segments = selectEdges(segmentsA, segmentsB, polygonA, polygonB, true, true);
        removeDuplicateSegments(segments);


        return connectEdges(segments);
    }
    public static List<List<Vector2>> Subtract(List<Vector2> polygonA, List<Vector2> polygonB)
    {
        List<Vector2> verticesA = new List<Vector2>(polygonA);
        List<Vector2> verticesB = new List<Vector2>(polygonB);

        intersectPolygons(verticesA, verticesB);
        List<LineSegment> segmentsA = getLineSegments(verticesA);
        List<LineSegment> segmentsB = getLineSegments(verticesB);

        List<LineSegment> segments = selectEdges(segmentsA, segmentsB, polygonA, polygonB, true, false);
        removeDuplicateSegments(segments);


        return connectEdges(segments);
    }
    public static List<List<Vector2>> Union(List<Vector2> polygonA, List<Vector2> polygonB)
    {
        List<Vector2> verticesA = new List<Vector2>(polygonA);
        List<Vector2> verticesB = new List<Vector2>(polygonB);

        intersectPolygons(verticesA, verticesB);
        List<LineSegment> segmentsA = getLineSegments(verticesA);
        List<LineSegment> segmentsB = getLineSegments(verticesB);

        List<LineSegment> segments = selectEdges(segmentsA, segmentsB, polygonA, polygonB, false, false);
        removeDuplicateSegments(segments);


        return connectEdges(segments);
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
    static List<LineSegment> selectEdges(List<LineSegment> segmentsA, List<LineSegment> segmentsB, List<Vector2> polygonA, List<Vector2> polygonB, bool selectA, bool selectB)
    {
        for (int i = 0; i < segmentsA.Count; i++)
        {
            Vector2 midPoint = segmentsA[i].start + (segmentsA[i].end - segmentsA[i].start) * 0.5f;
            if (PointInPolygon.insideAngle(midPoint, polygonB, 0f) == selectA)
            {
                segmentsA.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < segmentsB.Count; i++)
        {
            Vector2 midPoint = segmentsB[i].start + (segmentsB[i].end - segmentsB[i].start) * 0.5f;
            if (PointInPolygon.insideAngle(midPoint, polygonA, 0f) == selectB)
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
        List<List<Vector2>> outputPolygon = new List<List<Vector2>>();
        
        while(segments.Count > 0)
        {
            List<Vector2> polygon = new List<Vector2>();
            LineSegment currentSegment = segments[0];
            segments.RemoveAt(0);
            polygon.Add(currentSegment.start);
            polygon.Add(currentSegment.end);

            while (currentSegment.end != polygon[0])
            {
                for (int i = 0; i < segments.Count; i++)
                {
                    if (Vector2.Distance(segments[i].start, currentSegment.end) < 0.001f)
                    {
                        currentSegment = segments[i];
                        segments.RemoveAt(i);
                        polygon.Add(currentSegment.end);
                        break;
                    }
                    else if (Vector2.Distance(segments[i].end, currentSegment.end) < 0.001f)
                    {
                        currentSegment = new LineSegment(segments[i].end, segments[i].start);
                        segments.RemoveAt(i);
                        polygon.Add(currentSegment.end);
                        break;
                    }
                }
            }

            outputPolygon.Add(polygon);
        }


        return outputPolygon;
    }

    static void removeDuplicateSegments(List<LineSegment> segments)
    {
        foreach(LineSegment ls in segments)
        {
            Debug.Log("Segment: " + ls.start + " " + ls.end);
        }

        Debug.Log("------------------------");

        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = i + 1; j < segments.Count; j++)
            {
                if ((segments[i].start == segments[j].end && segments[i].end == segments[j].start) ||
                    (segments[i].start == segments[j].start && segments[i].end == segments[j].end))
                {
                    Debug.Log("Duplicate: SegmentA = " + segments[i].start + " " + segments[i].end);
                    segments.RemoveAt(j);
                    break;
                }
            }
        }
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
