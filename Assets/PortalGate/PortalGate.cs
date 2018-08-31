using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalGateSystem
{
    public class PortalGate : MonoBehaviour
    {
        static HashSet<Camera> virtualCameras = new HashSet<Camera>();

        public PortalGate pair;
        Quaternion rotY = Quaternion.Euler(0f, 180f, 0f);

        Dictionary<Camera, Camera> cameraTable = new Dictionary<Camera, Camera>();


        Material material;

        #region Unity

        private void Start()
        {
            var renderer = GetComponent<Renderer>();
            material = renderer.material;
        }

        private void OnDestroy()
        {
            Destroy(material);
        }

        private void LateUpdate()
        {
            cameraTable.ToList().ForEach(pair =>
            {
                var cam = pair.Key;
                var virtualCam = pair.Value;

                UpdatePairCamera(cam, virtualCam);
            });
        }

        private void OnWillRenderObject()
        {
            var cam = Camera.current;

            if ( !virtualCameras.Contains(cam))
            {
                if ( !cameraTable.ContainsKey(cam))
                {
                    cameraTable[cam] = CreatePairCamera(cam);
                }
            }
        }

        #endregion


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

            virtualCameras.Add(c);

            return c;
        }


        void UpdatePairCamera(Camera cam, Camera virtualCam)
        {
            var camTrans = cam.transform;

            var localPos = transform.InverseTransformPoint(camTrans.position);
            var localRot = Quaternion.Inverse(transform.rotation) * camTrans.rotation;

            var pairTrans = pair.transform;
            var pos = pairTrans.TransformPoint(rotY * localPos);
            var rot = pairTrans.rotation * rotY * localRot;

            virtualCam.transform.SetPositionAndRotation(pos, rot);

            material.mainTexture = virtualCam.targetTexture;
        }
    }
}