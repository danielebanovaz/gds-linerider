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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manager class that handles track design
/// </summary>
public class TrackDesigner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Track parameters
    
    /// <summary>
    /// The very Track: a collection of Track Segments
    /// </summary>
    private List<TrackSegment> _track = new List<TrackSegment>();

    /// <summary>
    /// New segments starting in a distance from an existing point below this range,
    /// will have their first point 'snapped' on the nearest one
    /// </summary>
    public float SnapRange = 1f;

    /// <summary>
    /// Min distance required from previous point in order to create a new one
    /// </summary>
    public float MinDistance = 0.25f;

    /// <summary>
    /// Max distance from previous point: if current distance is greater than this,
    /// a new point will be automatically created
    /// </summary>
    public float MaxDistance = 2f;

    /// <summary>
    /// Max angular difference (in degrees) from previous point: if current difference is greater than this,
    /// a new point will be automatically created
    /// </summary>
    public float MaxAngleDifference = 10.0f;

    /// <summary>
    /// Material used by Line Renderer
    /// </summary>
    public Material GroundMaterial;

    #endregion Track parameters

    #region Camera parameters

    /// <summary>
    /// Nearer camera zoom allowed
    /// </summary>
    public float MaxZoom = 3f;

    /// <summary>
    /// Farthest camera zoom allowed
    /// </summary>
    public float MinZoom = 40f;

    /// <summary>
    /// Zoom speed: the higher it is, the less mouse scroll is required to (de)zoom
    /// </summary>
    public float ZoomSpeed = 1f;

    #endregion Camera parameters

    #region Cursors

    /// <summary>
    /// Default mouse pointer
    /// </summary>
    public Texture2D DefaultCursor;

    /// <summary>
    /// Mouse pointer used when drawing
    /// </summary>
    public Texture2D DrawingCursor;

    /// <summary>
    /// Mouse pointer used when panning scene
    /// </summary>
    public Texture2D MoveCursor;

    #endregion Cursors

    /// <summary>
    /// Called by Unity engine each time this Component is enabled
    /// </summary>
    private void OnEnable()
    {
        // Switch to drawing cursor
        Cursor.SetCursor(DrawingCursor, new Vector2(0, 64), CursorMode.ForceSoftware);
    }

    /// <summary>
    /// Called by Unity engine each time this Component is disabled
    /// </summary>
    private void OnDisable()
    {
        // Switch to default cursor
        Cursor.SetCursor(DefaultCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    /// <summary>
    /// Called by Unity engine once per frame
    /// </summary>
    public void Update()
    {
        // If user scroll with mouse wheel..
        if (Input.mouseScrollDelta.y != 0)
        {
            // ..change camera zoom accordingly
            Camera.main.orthographicSize -= Input.mouseScrollDelta.y * ZoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MaxZoom, MinZoom);
        }
    }

    /// <summary>
    /// Called by Unity engine when the player start mouse drag
    /// </summary>
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            // If Left mouse button, start a new Track Segment
            case PointerEventData.InputButton.Left:
            {
                // Convert position from Screen Coordinates into World ones
                Vector2 position = eventData.pressPosition.ScreenToWorld();

                // Check whether to snap to a previous point
                if (_track.Count > 0)
                {
                    for (int ti = _track.Count - 1; ti >= 0; ti--)
                    {
                        TrackSegment currentTrack = _track[ti];
                        for (int pi = currentTrack.PointCount - 1; pi >= 0; pi--)
                        {
                            if ((currentTrack[pi] - position).magnitude < SnapRange)
                            {
                                position = currentTrack[pi];
                                break;
                            }
                        }
                    }
                }

                // Create a new Game Object for the new Track Segment
                GameObject newTrackSegmentGo = new GameObject("Track segment");
               
                // Set it as a child of this one
                newTrackSegmentGo.transform.parent = transform;
                
                // Add a Track Segment component to the newly created Game Object
                TrackSegment newTrackSegment = newTrackSegmentGo.AddComponent<TrackSegment>();
                
                // Set Line Renderer material
                newTrackSegment.GroundMaterial = GroundMaterial;

                // Add starting point
                newTrackSegment.AddPoint(position);

                // Add this new Track Segment to the whole Track
                _track.Add(newTrackSegment);
                break;
            }

            // If right button is pressed, switch mouse cursor to move
            case PointerEventData.InputButton.Right:
            {
                Cursor.SetCursor(MoveCursor, new Vector2(32, 32), CursorMode.ForceSoftware);
                break;
            }
        }
    }

    /// <summary>
    /// Called by Unity engine when the player is dragging mouse
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            // If Left mouse button, look for new points
            case PointerEventData.InputButton.Left:
            {
                // Determine active Track Segment
                TrackSegment currentTrack = _track.Last();

                // Convert position from Screen Coordinates into World ones
                Vector2 newPosition = eventData.position.ScreenToWorld();

                // Update Line Renderer preview accordingly
                currentTrack.SetCurrentPoint(newPosition);

                // Check min distance has been passed
                Vector2 lastTrackPoint = currentTrack.LastPoint;
                float currentDistance = (lastTrackPoint - newPosition).magnitude;
                if (currentDistance < MinDistance)
                    return;

                // Boost first point assignment (it cannot snap due to angle comparison)
                if (currentTrack.PointCount <= 1)
                    currentDistance *= 3f;
                
                // Check if max distance has been passed..
                if (currentDistance > MaxDistance)
                {
                    // ..create a new point
                    currentTrack.AddPoint(newPosition);
                    return;
                }

                // Check angle is over min threshold..
                if (currentTrack.PointCount <= 1)
                    return;

                Vector2 currentDirection = newPosition - lastTrackPoint;
                Vector2 previousDirection = lastTrackPoint - currentTrack[currentTrack.PointCount - 2];
                float currentAngle = currentDirection.GetZAngle();
                float previousAngle = previousDirection.GetZAngle();
                if (Mathf.Abs(Mathf.DeltaAngle(previousAngle, currentAngle)) > MaxAngleDifference)
                        // ..create a new point
                        currentTrack.AddPoint(newPosition);

                break;
            }

            // If right button is pressed, drag camera
            case PointerEventData.InputButton.Right:
            {
                // Convert new and previous positions from Screen Coordinates into World ones
                Vector2 currentPosition = eventData.position.ScreenToWorld();
                Vector2 previousPosition = (eventData.position - eventData.delta).ScreenToWorld();

                // Drag camera based in their World-space distance
                Vector3 distance = currentPosition - previousPosition;
                Camera.main.transform.position -= distance;
                break;
            }
        }
    }

    /// <summary>
    /// Called by Unity engine when the player finish dragging mouse
    /// </summary>
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            // If Left mouse button, add last point and validate
            case PointerEventData.InputButton.Left:
            {
                // Determine active Track Segment
                TrackSegment currentTrack = _track.Last();

                // Add current point
                currentTrack.AddPoint(eventData.position.ScreenToWorld());

                // If new Track doesn't have enough points, remove it
                if (!currentTrack.IsValid)
                {
                    _track.Remove(currentTrack);
                    Destroy(currentTrack.gameObject);
                }

                break;
            }
            // If right button is pressed, switch back to default mouse cursor
            case PointerEventData.InputButton.Right:
            {
                Cursor.SetCursor(DrawingCursor, new Vector2(0, 64), CursorMode.ForceSoftware);
                break;
            }
        }
    }

    /// <summary>
    /// Remove last created Track Segment
    /// </summary>
    public void Undo()
    {
        if (_track.Count == 0)
            return;

        // Determine last created Track Segment
        TrackSegment lastSegment = _track.Last();

        // Remove it from Track
        _track.Remove(lastSegment);

        // Remove it from game
        Destroy(lastSegment.gameObject);
    }

    /// <summary>
    /// Calculate lower height reached by current Track
    /// </summary>
    /// <returns></returns>
    public float CalculateMinHeight()
    {
        float min = float.MaxValue;

        // Look for the minimum Y value in all Track points
        foreach (TrackSegment segment in _track)
        {
            for(int i = 0; i < segment.PointCount; i++)
            {
                if (segment[i].y < min)
                    min = segment[i].y;
            }
        }

        return min;
    }
}
