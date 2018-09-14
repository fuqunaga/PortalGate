using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PortalGateSystem
{
    public class PortalGun : MonoBehaviour
    {
        public KeyCode key0 = KeyCode.Mouse0;
        public KeyCode key1 = KeyCode.Mouse1;

        public GameObject gatePrefab;
        List<PortalGate> gatePair = new List<PortalGate>(2) { null, null };

        public float gatePosOffset = 0.02f;


        private void Update()
        {
            if (Input.GetKeyDown(key0))
            {
                Shot(0);
            }
            else if (Input.GetKeyDown(key1))
            {
                Shot(1);
            }
        }

        void Shot(int idx)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, LayerMask.GetMask(new[] { "StageColl" })))
            {
                var gate = gatePair[idx];
                if (gate == null)
                {
                    var go = Instantiate(gatePrefab);
                    gate = gatePair[idx] = go.GetComponent<PortalGate>();

                    var pair = gatePair[(idx + 1) % 2];
                    if (pair != null)
                    {
                        gate.SetPair(pair);
                        pair.SetPair(gate);
                    }
                }

                gate.hitColl = hit.collider;

                var trans = gate.transform;
                var normal = hit.normal;
                trans.position = hit.point + normal * gatePosOffset;
                trans.rotation = Quaternion.LookRotation(-normal, transform.up);

                gate.Open();
            }
        }
    }
}