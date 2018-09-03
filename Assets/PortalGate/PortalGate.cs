using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalGateSystem
{
    public class PortalGate : MonoBehaviour
    {
        public static class ShaderParam
        {
            public const string MainCameraViewProj = "_MainCameraViewProj";
        }


        public GameObject virtualCameraPrefab;
        public PortalGate pair;

        public Quaternion gateRot { get; } = Quaternion.Euler(0f, 180f, 0f);

        Dictionary<Camera, VirtualCamera> virtualCameraTable = new Dictionary<Camera, VirtualCamera>();

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

        private void OnWillRenderObject()
        {
            var cam = Camera.current;
            var virtualCam = cam.gameObject.GetComponent<VirtualCamera>();
            var rootCam = cam;
            RenderTexture tex;
            


            // main camera
            if (virtualCam == null)
            {
                VirtualCamera vc;
                if (!virtualCameraTable.TryGetValue(cam, out vc))
                {
                    vc = virtualCameraTable[cam] = CreateVirtualCamera(cam);
                    return;
                }

                tex = vc.targetTexture;
            }
            // virtual camera
            else
            {
                rootCam = virtualCam.parentCamera;
                tex = virtualCam.lastTex;
            }

            Matrix4x4 projGPU = GL.GetGPUProjectionMatrix(rootCam.projectionMatrix, true) * rootCam.worldToCameraMatrix;

            material.mainTexture = tex;
            material.SetMatrix(ShaderParam.MainCameraViewProj, projGPU);
        }

        #endregion


        VirtualCamera CreateVirtualCamera(Camera cam)
        {
            var go = Instantiate(virtualCameraPrefab);
            go.name = cam.name + "_virtual";
            go.transform.SetParent(transform);

            var vc = go.GetComponent<VirtualCamera>();
            vc.parentCamera = cam;
            vc.parentGate = this;

            return vc;
        }


        public void PassTransform(Transform trans, Vector3 worldPos, Quaternion worldRot)
        {
            var localPos = transform.InverseTransformPoint(worldPos);
            var localRot = Quaternion.Inverse(transform.rotation) * worldRot;

            var pairGateTrans = pair.transform;
            var gateRot = pair.gateRot;
            var pos = pairGateTrans.TransformPoint(gateRot * localPos);
            var rot = pairGateTrans.rotation * gateRot * localRot;

            trans.SetPositionAndRotation(pos, rot);
        }

        public void PassRigidbody(Rigidbody rigidbody)
        {
            var rot = pair.transform.rotation * pair.gateRot * Quaternion.Inverse(transform.rotation);
            rigidbody.velocity = rot * rigidbody.velocity;
        }
    }
}