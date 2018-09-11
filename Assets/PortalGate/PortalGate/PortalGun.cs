﻿using System;
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
            if (Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, LayerMask.GetMask(new[] { "Stage" })))
            {
                var gate = gatePair[idx];
                if (gate == null)
                {
                    var go = Instantiate(gatePrefab);
                    gate = gatePair[idx] = go.GetComponent<PortalGate>();

                    var pair = gatePair[(idx + 1) % 2];
                    gate.pair = pair;
                    if (pair != null) pair.pair = gate;
                }

                var trans = gate.transform;
                var normal = hit.normal;
                trans.position = hit.point + normal * 0.01f;
                trans.rotation = Quaternion.LookRotation(-normal, transform.up);
            }
        }
    }
}