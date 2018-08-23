using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class IKAttackTarget : MonoBehaviour {

    public Transform _attackTarget;
    public Transform _hitpoint;
    FullBodyBipedIK ik;
    public AimIK aimIk;
    public float weight;
    public FullBodyBipedEffector effector;
    [Tooltip("Weight of aiming the body to follow the target")]
    public AnimationCurve aimWeight;

    public PlayerBlackboard _blackboard;

    private void Start()
    {
        ik = GetComponent<FullBodyBipedIK>();
        _blackboard = GetComponent<PlayerBlackboard>();
    }

    void LateUpdate()
    {
        // Getting the weight of pinning the fist to the target
        float hitWeight = 1; // _blackboard.animator.GetFloat("HitWeight");

        // Pinning the first with FBIK
        ik.solver.GetEffector(effector).position = _attackTarget.position;
        ik.solver.GetEffector(effector).positionWeight = hitWeight * weight;

        // Aiming the body with AimIK to follow the target
        if (aimIk != null)
        {
            // Make the aim transform always look at the pin. This will normalize the default aim diretion to the animated pose.
            aimIk.solver.transform.LookAt(_hitpoint.position);

            // Set aim target
            aimIk.solver.IKPosition = _attackTarget.position;

            // Setting aim weight
            aimIk.solver.IKPositionWeight = aimWeight.Evaluate(hitWeight) * weight;
        }
    }

}
