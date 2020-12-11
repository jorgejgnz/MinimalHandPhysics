using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JorgeJGnz.MinimalHandPhysics
{
    [RequireComponent(typeof(ConfigurableJoint))]
    public class SlaveBone : MonoBehaviour
    {
        ConfigurableJoint joint;

        public Transform target;
        public bool followTargetPosition = false;
        public bool followTargetRotation = false;

        Vector3 xAxis, zAxis, yAxis;
        Quaternion q;

        Quaternion initialGlobalRotation;
        Quaternion initialLocalRotation;
        Quaternion resultRotation;

        void Start()
        {
            if (!joint)
                joint = GetComponent<ConfigurableJoint>();

            xAxis = joint.axis;
            zAxis = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
            yAxis = Vector3.Cross(zAxis, xAxis).normalized;

            // Joint frame rotation
            q = Quaternion.LookRotation(zAxis, yAxis);

            if (joint.connectedBody != null && !joint.configuredInWorldSpace)
            {
                initialLocalRotation = joint.connectedBody.transform.localRotation;
            }
            else
            {
                initialGlobalRotation = joint.transform.rotation;
            }
        }

        void FixedUpdate()
        {
            if (followTargetPosition && target)
            {
                // Local
                if (joint.connectedBody != null && !joint.configuredInWorldSpace)
                {
                    // Phalanges don't follow position
                }
                // World
                else
                {
                    // We should be able to move connectedAnchor in space
                    if (joint.autoConfigureConnectedAnchor)
                        joint.autoConfigureConnectedAnchor = false;

                    // Joint will move the object towards connectedAnchor
                    if (joint.targetPosition != Vector3.zero)
                        joint.targetPosition = Vector3.zero;

                    // Update the center of the joint frame
                    joint.connectedAnchor = target.position;
                }

            }

            if (followTargetRotation && target)
            {
                // Local
                if (joint.connectedBody != null && !joint.configuredInWorldSpace)
                {
                    // Target local rotation relative to the joint frame + initial offset (local)
                    resultRotation = Quaternion.Inverse(q);
                    resultRotation *= Quaternion.Inverse(target.localRotation);
                    resultRotation *= initialLocalRotation;
                    resultRotation *= q;

                    joint.targetRotation = resultRotation;
                }
                // World
                else
                {
                    // Target world rotation relative to joint frame + initial offset (global)
                    resultRotation = Quaternion.Inverse(q);
                    resultRotation *= initialGlobalRotation;
                    resultRotation *= Quaternion.Inverse(target.rotation);
                    resultRotation *= q;

                    joint.targetRotation = resultRotation;
                }
            }
        }
    }
}
