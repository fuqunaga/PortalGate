using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace PortalGateSystem
{
    [RequireComponent(typeof(Collider))]
    public class PortalObj : MonoBehaviour
    {
        public Transform center;
        protected Rigidbody rigidbody_;
        protected Collider collider_;
        protected SimpleFPController fpController;

        protected HashSet<PortalGate> touchingGates = new HashSet<PortalGate>();

        private void Start()
        {
            if (center == null) center = transform;
            rigidbody_ = GetComponent<Rigidbody>();
            collider_ = GetComponent<Collider>();
            fpController = GetComponent<SimpleFPController>();
        }

        private void Update()
        {
            var passedGate = touchingGates.FirstOrDefault(gate =>
            {
                var posOnGate = gate.transform.InverseTransformPoint(center.position);
                return posOnGate.z > 0f;
            });


            if (passedGate != null)
            {
                PassGate(passedGate);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var gate = other.GetComponent<PortalGate>();
            if (gate != null)
            {
                touchingGates.Add(gate);
                Physics.IgnoreCollision(gate.hitColl, collider_, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var gate = other.GetComponent<PortalGate>();
            if (gate != null)
            {
                touchingGates.Remove(gate);
                Physics.IgnoreCollision(gate.hitColl, collider_, false);
            }
        }


        void PassGate(PortalGate gate)
        {
            gate.UpdateTransformOnPair(transform);

            if (rigidbody_ != null)
            {
                gate.UpdateRigidbodyOnPair(rigidbody_);
            }

            if (fpController != null)
            {
                fpController.InitMouseLook();
            }

            Physics.IgnoreCollision(gate.pair.hitColl, collider_);
        }
    }
}