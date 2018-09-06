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

        public int maxGeneration = 5;

        public Quaternion gateRot { get; } = Quaternion.Euler(0f, 180f, 0f);

        Dictionary<Camera, VirtualCamera> pairVCTable = new Dictionary<Camera, VirtualCamera>();

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
            var vc = cam.gameObject.GetComponent<VirtualCamera>();
            var hasVC = vc != null;
            var rootCam = cam;
            RenderTexture tex;

            VirtualCamera pairVC;
            if (!pairVCTable.TryGetValue(cam, out pairVC))
            {
                if (!hasVC || vc.generation < maxGeneration)
                {
                    pairVC = pairVCTable[cam] = CreateVirtualCamera(cam, vc);
                    return;
                }
            }


            if (pairVC != null)
            {
                tex = pairVC.targetTexture;
            }
            // last generation
            else
            {
                rootCam = vc.parentCamera;
                tex = vc.lastTex;
            }

            Matrix4x4 projGPU = GL.GetGPUProjectionMatrix(rootCam.projectionMatrix, true) * rootCam.worldToCameraMatrix;

            material.mainTexture = tex;
            material.SetMatrix(ShaderParam.MainCameraViewProj, projGPU);
        }

        #endregion


        VirtualCamera CreateVirtualCamera(Camera parentCam, VirtualCamera parentVC)
        {
            var rootCam = parentVC?.rootCamera ?? parentCam;
            var generation = parentVC?.generation+1 ?? 1;

            var go = Instantiate(virtualCameraPrefab);
            go.name = rootCam.name + "_virtual" + generation;
            go.transform.SetParent(transform);

            var vc = go.GetComponent<VirtualCamera>();
            vc.rootCamera = rootCam;
            vc.parentCamera = parentCam;
            vc.parentGate = this;
            vc.generation = generation;

            return vc;
        }


        public void UpdateTransformOnPair(Transform trans, Vector3 worldPos, Quaternion worldRot)
        {
            var localPos = transform.InverseTransformPoint(worldPos);
            var localRot = Quaternion.Inverse(transform.rotation) * worldRot;

            var pairGateTrans = pair.transform;
            var gateRot = pair.gateRot;
            var pos = pairGateTrans.TransformPoint(gateRot * localPos);
            var rot = pairGateTrans.rotation * gateRot * localRot;

            trans.SetPositionAndRotation(pos, rot);
        }

        public void UpdateRigidbodyOnPair(Rigidbody rigidbody)
        {
            var rot = pair.transform.rotation * pair.gateRot * Quaternion.Inverse(transform.rotation);
            rigidbody.velocity = rot * rigidbody.velocity;
        }
    }
}