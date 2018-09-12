using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace PortalGateSystem
{
    [RequireComponent(typeof(Collider))]
    public class PortalObj : MonoBehaviour
    {
        protected Rigidbody rigidbody_;
        protected Collider collider_;
        protected FirstPersonController fpController;

        private void Start()
        {
            rigidbody_ = GetComponent<Rigidbody>();
            collider_ = GetComponent<Collider>();
            fpController = GetComponent<FirstPersonController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var gate = other.GetComponent<PortalGate>();
            if (gate != null)
            {
                Physics.IgnoreCollision(gate.hitColl, collider_, true);
            }
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
                    gate.UpdateTransformOnPair(transform, transform.position, transform.rotation);
                    
                    if ( rigidbody_ != null)
                    {
                        gate.UpdateRigidbodyOnPair(rigidbody_);
                    }

                    if ( fpController != null)
                    {
                        fpController.InitMouseLook();
                    }
                }

                Physics.IgnoreCollision(gate.hitColl, collider_, false);
            }
        }
    }
}