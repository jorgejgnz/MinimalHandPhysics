using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JorgeJGnz.MinimalHandPhysics
{
    [Serializable]
    public class RigMap
    {
        public Transform master;
        public SlaveBone slave;
    }

    public class ProxyHand : MonoBehaviour
    {
        public OVRSkeleton dataProvider;
        public Collider palmCollider;
        public RigMap[] rigMaps = new RigMap[24];

        Collider[] colliders;

        private void Awake()
        {
            for (int b = 0; b < rigMaps.Length; b++)
            {
                // Set slave targets
                if (rigMaps[b].master && rigMaps[b].slave)
                    rigMaps[b].slave.target = rigMaps[b].master;
            }

            // Ignore phalange-palm collisions
            colliders = rigMaps[0].slave.GetComponentsInChildren<Collider>();
            for (int c = 0; c < colliders.Length; c++)
            {
                Physics.IgnoreCollision(palmCollider, colliders[c], true);
            }
        }

        private void Update()
        {
            if (dataProvider.Bones.Count != rigMaps.Length)
                return;

            // Update wrist position and rotation in world space
            rigMaps[0].master.position = dataProvider.Bones[0].Transform.position;
            rigMaps[0].master.rotation = dataProvider.Bones[0].Transform.rotation;

            // Update phalanges rotations in local space
            for (int b = 1; b < dataProvider.Bones.Count; b++)
            {
                rigMaps[b].master.localRotation = dataProvider.Bones[b].Transform.localRotation;
            }
        }
    }
}
