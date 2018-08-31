using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalGateSystem
{
    public class PortalGate : MonoBehaviour
    {
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

            if ( cam.gameObject.GetComponent<VirtualCamera>() == null )
            {
                VirtualCamera vc;
                if ( !virtualCameraTable.TryGetValue(cam, out vc))
                {
                    vc = virtualCameraTable[cam] = CreateVirtualCamera(cam);
                }

                material.mainTexture = vc.targetTexture;
            }
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
    }
}