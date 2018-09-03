using UnityEngine;

namespace PortalGateSystem
{
    [RequireComponent(typeof(Camera))]
    public class VirtualCamera : MonoBehaviour
    {
        public Camera cam;

        public PortalGate parentGate;
        public Camera parentCamera;

        bool currentTex0;
        protected RenderTexture tex0;
        protected RenderTexture tex1;


        public RenderTexture targetTexture => currentTex0 ? tex0 : tex1;

        public RenderTexture lastTex => currentTex0 ? tex1 : tex0;

        private void Start()
        {
            cam = GetComponent<Camera>();

            cam.CopyFrom(parentCamera);
            cam.depth = parentCamera.depth - 1;

            var baseTex = parentCamera.targetTexture;
            var size = (baseTex != null)
                ? new Vector2Int(baseTex.width, baseTex.height)
                : new Vector2Int(Screen.width, Screen.height);

            tex0 = new RenderTexture(size.x, size.y, 24);
            tex1 = new RenderTexture(size.x, size.y, 24);

            cam.targetTexture = tex0;
            currentTex0 = true;
        }

        private void Update()
        {
            // swap
            cam.targetTexture = lastTex;
            currentTex0 = !currentTex0;
        }

        private void LateUpdate()
        {
            var parentGateTrans = parentGate.transform;
            var parentCamTrans = parentCamera.transform;

            var localPos = parentGateTrans.InverseTransformPoint(parentCamTrans.position);
            var localRot = Quaternion.Inverse(parentGateTrans.rotation) * parentCamTrans.rotation;

            var pairGate = parentGate.pair;
            SetPairLocalToTransform(transform, pairGate, localPos, localRot);

            // TODO: pairGateが斜めってるとき正しくない
            // pairGateの奥しか描画しない。とりあえずnearClipPlaneでなんとなく対処
            var pairGatePosOnCamera = transform.InverseTransformPoint(pairGate.transform.position);
            cam.nearClipPlane = Mathf.Max(0f, pairGatePosOnCamera.z - 1f);
        }

        void SetPairLocalToTransform(Transform trans, PortalGate pairGate, Vector3 localPos, Quaternion localRot)
        {
            var pairGateTrans = pairGate.transform;
            var gateRot = pairGate.gateRot;
            var pos = pairGateTrans.TransformPoint(gateRot * localPos);
            var rot = pairGateTrans.rotation * gateRot * localRot;

            trans.SetPositionAndRotation(pos, rot);
        }
    }
}