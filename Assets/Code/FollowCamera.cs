using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : Singleton<FollowCamera>
{
    public new Camera camera;
    public float minSize = 5.0f;

    [Header("Follow")]
    public float moveSpeed = 5000f;
    public float zoomSpeed = 0.05f;
    public float additionalDistance = 10.0f;
    public List<Transform[]> followedObjectsStack = new List<Transform[]>() { new Transform[0] };

    public Transform[] followedObjects { get { return followedObjectsStack[followedObjectsStack.Count - 1]; } }

    private float m_PreviousStepSize = 0.0f;
    public float previousStepSize { get { return m_PreviousStepSize; } }
    public bool isMoving { get { return previousStepSize > 0.01f; } }

    public float lastSqrMag = Mathf.Infinity;
    public Vector3 desiredVelocity;
    Rigidbody body;

    private void Start()
    {
        body = this.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        var bounds = GetObjectsBounds(); //TODO - can we avoid calculating this bounding box every frame? But it will still need to be updated when something moves.
        if (bounds.size.sqrMagnitude < 0.01f)
        {
            return;
        }

        var boundsSize = bounds.extents.magnitude;

        var targetSize = Mathf.Max(minSize, boundsSize);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, 1.0f / zoomSpeed);

        var desiredPosition = bounds.center - transform.forward * (boundsSize * Mathf.Sqrt(2) + additionalDistance);
        var direction = desiredPosition - transform.position;
        var smoothedPosition = transform.position + (direction * Time.deltaTime) * moveSpeed;

        m_PreviousStepSize = Vector3.Distance(transform.position, smoothedPosition);

        float distanceToTarget = direction.magnitude;

        var directionalVector = direction.normalized * distanceToTarget * moveSpeed;
        desiredVelocity = directionalVector;

        if (Vector3.Dot(desiredPosition - transform.position, desiredVelocity) < 0)
        {
            desiredVelocity *= -1;
        }

        body.AddForce(desiredPosition - body.velocity);
        var sqrMag = (desiredPosition - transform.position).sqrMagnitude;

        if (sqrMag > lastSqrMag)
        {
            desiredVelocity = Vector3.zero;
        }
        lastSqrMag = sqrMag;

    }

    void FixedUpdate()
    {
        body.velocity = desiredVelocity;
    }

    public static void Push<T>(T newTarget) where T : Component
    {
        instance.PushTarget(newTarget.transform);
    }

    public static void Push<T>(T[] newTarget) where T : Component
    {
        var transforms = new Transform[newTarget.Length];
        for (int i = 0; i < newTarget.Length; ++i)
        {
            transforms[i] = newTarget[i].transform;
        }

        instance.PushTarget(transforms);
    }

    public void PushTarget(Transform newTarget)
    {
        PushTarget(new Transform[1] { newTarget });
    }

    public void PushTarget(Transform[] newTarget)
    {
        followedObjectsStack.Add(newTarget);
        m_PreviousStepSize = float.PositiveInfinity;
    }

    public static void Pop()
    {
        instance.PopTarget();
    }

    public void PopTarget()
    {
        followedObjectsStack.RemoveAt(followedObjectsStack.Count - 1);
        m_PreviousStepSize = float.PositiveInfinity;
    }

    public Bounds GetObjectsBounds()
    {
        var result = new Bounds(transform.position, Vector3.zero);
        if (followedObjects == null || followedObjects.Length == 0)
        {
            return result;
        }

        //find first renderer object
        int i;
        for (i = 0; i < followedObjects.Length; ++i)
        {
            Bounds objectBounds;
            if (GetBounds(followedObjects[i], out objectBounds))
            {
                result = objectBounds;
                break;
            }
        }

        //No renderers
        if (i == followedObjects.Length)
        {
            return result;
        }

        //include the rest of the renderers
        for (++i; i < followedObjects.Length; ++i)
        {
            Bounds objectBounds;
            if (GetBounds(followedObjects[i], out objectBounds))
            {
                result.Encapsulate(objectBounds);
            }
        }

        return result;
    }

    public static bool GetBounds(Transform obj, out Bounds bounds)
    {
        if (obj == null)
        {
            bounds = default(Bounds);
            return false;
        }

        var collider = obj.GetComponent<Collider>();
        if (collider != null && collider.enabled)
        {
            bounds = collider.bounds;
            return true;
        }

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null && renderer.enabled)
        {
            bounds = renderer.bounds;
            return true;
        }

        bounds = default(Bounds);
        return false;
    }
}
