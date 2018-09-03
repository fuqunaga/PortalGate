using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PortalGateSystem
{
    public class PortalObj : MonoBehaviour
    {
        protected Rigidbody rigidbody_;

        private void Start()
        {
            rigidbody_ = GetComponent<Rigidbody>();
        }

        private void OnTriggerExit(Collider other)
        {
            var gate = other.GetComponent<PortalGate>();
            if (gate != null)
            {
                var posOnGate = other.transform.InverseTransformPoint(transform.position);

                // z >= 0f ならGate通過
                if ( posOnGate.z >= 0f)
                {
                    gate.PassTransform(transform, transform.position, transform.rotation);
                    
                    if ( rigidbody_ != null)
                    {
                        gate.PassRigidbody(rigidbody_);
                    }
                }
            }
        }
    }
}