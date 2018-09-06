using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PortalGateSystem
{
    public class FillTexture : MonoBehaviour
    {
        public VirtualCamera virtualCamera;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var tex = virtualCamera?.targetTexture ?? source;

            Graphics.Blit(tex, destination);   
        }
    }
}
