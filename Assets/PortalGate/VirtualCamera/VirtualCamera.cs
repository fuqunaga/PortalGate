using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalGateSystem
{
    [RequireComponent(typeof(Camera))]
    public class VirtualCamera : MonoBehaviour
    {
        public Transform zBarrier;

        public Camera cam;

        public PortalGate parentGate;
        public Camera parentCamera;

        public RenderTexture targetTexture => (cam != null) ? cam.targetTexture : null;

        private void Start()
        {
            cam = GetComponent<Camera>();

            cam.CopyFrom(parentCamera);
            cam.depth = parentCamera.depth - 1;

            var baseTex = parentCamera.targetTexture;
            var size = (baseTex != null)
                ? new Vector2Int(baseTex.width, baseTex.height)
                : new Vector2Int(Screen.width, Screen.height);

            var tex = new RenderTexture(size.x, size.y, 24);

            cam.targetTexture = tex;
        }


        private void LateUpdate()
        {
            var parentGateTrans = parentGate.transform;
            var parentCamTrans = parentCamera.transform;
            var pairGateTrans = parentGate.pair.transform;

            var localPos = parentGateTrans.InverseTransformPoint(parentCamTrans.position);
            var localRot = Quaternion.Inverse(parentGateTrans.rotation) * parentCamTrans.rotation;

            var gateRot = parentGate.gateRot;
            var pos = pairGateTrans.TransformPoint(gateRot * localPos);
            var rot = pairGateTrans.rotation * gateRot * localRot;

            transform.SetPositionAndRotation(pos, rot);
        }
    }
}