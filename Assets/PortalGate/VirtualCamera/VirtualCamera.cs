using UnityEngine;

namespace PortalGateSystem
{
    [RequireComponent(typeof(Camera))]
    public class VirtualCamera : MonoBehaviour
    {
        public Camera cam;

        public PortalGate parentGate;
        public Camera rootCamera;
        public Camera parentCamera;

        public int generation;

        bool currentTex0;
        protected RenderTexture tex0;
        protected RenderTexture tex1;


        public RenderTexture targetTexture => currentTex0 ? tex0 : tex1;

        public RenderTexture lastTex => currentTex0 ? tex1 : tex0;

        private void Start()
        {
            cam = GetComponent<Camera>();

            cam.CopyFrom(rootCamera);
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
            // PreviewCameraなどはこのタイミングでnullになっているようなのでチェック
            if (parentCamera == null)
            {
                Destroy(gameObject);
                return;
            }

            var parentCamTrans = parentCamera.transform;
            var parentGateTrans = parentGate.transform;

            parentGate.UpdateTransformOnPair(transform, parentCamTrans.position, parentCamTrans.rotation);

            cam.enabled = IsPairGateVisible();

            if (cam.enabled)
            {
                UpdateProjectionMantrix();
            }
        }

        bool IsPairGateVisible()
        {
            var pairTrans = parentGate.pair.transform;
            return (transform.InverseTransformPoint(pairTrans.position).z >= 0f) // gate is front of camera
                && (Vector3.Dot(transform.forward, pairTrans.forward) <= 0f); // camera is back of gate
        }

        void UpdateProjectionMantrix()
        {
            // pairGateの奥しか描画しない = nearClipPlane を pairGateと一致させる
            var pairGateTrans = parentGate.pair.transform;
            var clipPlane = CalcPlane(cam, pairGateTrans.position, -pairGateTrans.forward);
            cam.ResetProjectionMatrix();
            cam.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);
        }

        Vector4 CalcPlane(Camera cam, Vector3 pos, Vector3 normal)
        {
            var viewMat = cam.worldToCameraMatrix;

            var normalOnView = viewMat.MultiplyVector(normal).normalized;
            var posOnView = viewMat.MultiplyPoint(pos);

            return new Vector4(
                normalOnView.x,
                normalOnView.y,
                normalOnView.z,
                -Vector3.Dot(normalOnView, posOnView)
                );
        }
    }
}