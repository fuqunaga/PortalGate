using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalGateSystem
{
    public class PortalGate : MonoBehaviour
    {
        public PortalGate pair;
        Quaternion rotY = Quaternion.Euler(0f, 180f, 0f);

        Dictionary<Camera, Camera> cameraTable = new Dictionary<Camera, Camera>();


        Material material;

        private void Start()
        {
            var renderer = GetComponent<Renderer>();
            material = renderer.material;
        }

        private void OnDestroy()
        {
            Destroy(material);
        }

        private void OnWillRenderObject()
        {
            var cam = Camera.current;

            var isPair = cameraTable.ContainsValue(cam);

            if (!isPair)
            {
                var camTrans = cam.transform;

                var localPos = transform.InverseTransformPoint(camTrans.position);
                var localRot = Quaternion.Inverse(transform.rotation) * camTrans.rotation;

                var pairTrans = pair.transform;
                var pos = pairTrans.TransformPoint(rotY * localPos);
                var rot = pairTrans.rotation * rotY * localRot;

                var pairCam = GetPairCamera(cam);
                pairCam.transform.SetPositionAndRotation(pos, rot);

                material.mainTexture = pairCam.targetTexture;
            }
        }

        Camera GetPairCamera(Camera cam)
        {
            Camera ret;
            if (!cameraTable.TryGetValue(cam, out ret))
            {
                ret = cameraTable[cam] = CreatePairCamera(cam);
            }

            return ret;
        }

        Camera CreatePairCamera(Camera cam)
        {
            var go = new GameObject(cam.name + "_pair");
            go.transform.SetParent(transform);

            var c = go.AddComponent<Camera>();
            c.CopyFrom(cam);
            c.depth = cam.depth - 1;

            var baseTex = cam.targetTexture;
            var size = (baseTex != null)
                ? new Vector2Int(baseTex.width, baseTex.height)
                : new Vector2Int(Screen.width, Screen.height);

            var tex = new RenderTexture(size.x, size.y, 24);

            c.targetTexture = tex;

            return c;
        }
    }
}