/*                                                                                     *\
           _____ _____   _____   _      _              _____  _     _           
          / ____|  __ \ / ____| | |    (_)            |  __ \(_)   | |          
         | |  __| |  | | (___   | |     _ _ __   ___  | |__) |_  __| | ___ _ __ 
         | | |_ | |  | |\___ \  | |    | | '_ \ / _ \ |  _  /| |/ _` |/ _ \ '__|
         | |__| | |__| |____) | | |____| | | | |  __/ | | \ \| | (_| |  __/ |   
          \_____|_____/|_____/  |______|_|_| |_|\___| |_|  \_\_|\__,_|\___|_|   
      ˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽
        ©2016 Sionera Entertainment - Daniele Banovaz (daniele.banovaz@gmail.com)

        Developed as a tutorial for Game Developement Saturday #1, 2016-03-19, PN

\*                                                                                     */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Segment of the Track, designed by Player
/// </summary>
public class TrackSegment : MonoBehaviour
{
    /// <summary>
    /// List of vertices that define this segment
    /// </summary>
    private List<Vector2> _points = new List<Vector2>();

    /// <summary>
    /// Line Renderer used to draw this segment on screen
    /// </summary>
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Edge Collider used to support runnning Car
    /// </summary>
    private EdgeCollider2D _collider;

    /// <summary>
    /// Height of the Line Renderer
    /// </summary>
    private const float LineScale = 3;


    #region Collection Properties

    /// <summary>
    /// Indexed access to vertices
    /// </summary>
    public Vector2 this[int index]
    {
        get { return _points[index]; }
    }

    /// <summary>
    /// Last point of this segment
    /// </summary>
    public Vector2 LastPoint
    {
        get { return _points[_points.Count - 1]; }
    }

    /// <summary>
    /// Number of points this segment has
    /// </summary>
    public int PointCount
    {
        get { return _points.Count; }
    }

    /// <summary>
    /// Determine if it can be considered a valid segment
    /// (at least two points)
    /// </summary>
    public bool IsValid
    {
        get { return _points.Count > 1; }
    }

    #endregion Collection Properties

    /// <summary>
    /// Expose Material property of the Line Renderer
    /// (used to set terrain texture)
    /// </summary>
    public Material GroundMaterial
    {
        get { return _lineRenderer.material; }
        set { _lineRenderer.material = value; }
    }

    /// <summary>
    /// Called by Unity when this Component is created
    /// </summary>
    private void Awake()
    {
        // Create required components: Line Renderer for graphics...
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.SetWidth(LineScale, LineScale);

        // ...and Collider for physics
        _collider = gameObject.AddComponent<EdgeCollider2D>();
    }

    /// <summary>
    /// Adds a new point to the end of the segment
    /// </summary>
    /// <param name="point"></param>
    public void AddPoint(Vector2 point)
    {
        // Add to the collection
        _points.Add(point);

        // Update Line Renderer's vertices, adding an extra one to handle in-drawing point
        _lineRenderer.SetVertexCount(_points.Count);
        _lineRenderer.SetPosition(_points.Count - 1, point);
        _needToAddLineRendererVertex = true;

        // Update Edge Collider's vertices
        if (_points.Count > 1)
            _collider.points = _points.ToArray();
    }

    private bool _needToAddLineRendererVertex ;

    /// <summary>
    /// Set current position of the in-drawing point
    /// </summary>
    /// <param name="point"></param>
    public void SetCurrentPoint(Vector2 point)
    {
        if (_needToAddLineRendererVertex)
        {
            _lineRenderer.SetVertexCount(_points.Count + 1);
            _needToAddLineRendererVertex = false;
        }
        _lineRenderer.SetPosition(_points.Count, point);
    }
}
