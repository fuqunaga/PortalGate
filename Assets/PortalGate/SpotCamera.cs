using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpotCamera : MonoBehaviour
{
    public MeshFilter target;
    Camera camera_;


    private void Start()
    {
        camera_ = GetComponent<Camera>();
    }

    void Update()
    {
        var mesh = target.sharedMesh;
        var posList = mesh.vertices
            .Select(vtx => target.transform.TransformPoint(vtx))
            .ToList();

        UpdateRect(posList);
        UpdateProjection(posList);

    }

    void UpdateRect(List<Vector3> posList)
    {
        camera_.ResetProjectionMatrix();
        camera_.rect = new Rect(Vector2.zero, Vector2.one);

        var vpPosList = posList
            .Select(wPos => camera_.WorldToViewportPoint(wPos))
            .ToList();


        var xList = vpPosList.Select(p => p.x);
        var yList = vpPosList.Select(p => p.y);

        var rect = new Rect()
        {
            xMin = xList.Min(),
            xMax = xList.Max(),
            yMin = yList.Min(),
            yMax = yList.Max(),
        };

        camera_.rect = rect;
    }

    void UpdateProjection(List<Vector3> posList)
    {
        var nearPosList = posList
            .Select(wPos => WorldPosToNearPos(wPos))
            .ToList();

        var xList = nearPosList.Select(p => p.x);
        var yList = nearPosList.Select(p => p.y);

        var frustum = new FrustumPlanes()
        {
            left = xList.Min(),
            right = xList.Max(),
            bottom = yList.Min(),
            top = yList.Max(),
            zNear = camera_.nearClipPlane,
            zFar = camera_.farClipPlane
        };

        //camera_.ResetProjectionMatrix();
        camera_.projectionMatrix = Matrix4x4.Frustum(frustum);
    }

    Vector3 WorldPosToNearPos(Vector3 pos)
    {
        var posLocal = camera_.transform.InverseTransformPoint(pos);
        var zRate = camera_.nearClipPlane / posLocal.z;
        return posLocal * zRate;
    }
}