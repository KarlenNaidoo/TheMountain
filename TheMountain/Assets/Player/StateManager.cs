using UnityEngine;
using System.Collections;
using Player.PlayerController;
using Player;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerBlackboard))]
[RequireComponent(typeof(CameraManager))]
public class StateManager : PlayerMotor
{

    CameraManager cameraManager;

    protected override void Start()
    {
        base.Start();
        cameraManager = GetComponent<CameraManager>();
        CalculatePhysicsMaterials();
        StartCoroutine(CharacterInit());
    }


    protected virtual IEnumerator CharacterInit() // TODO: Performance issues on this function
    {
        yield return new WaitForEndOfFrame();
        if (cameraManager.tpCamera == null)
        {
            cameraManager.tpCamera = FindObjectOfType<ThirdPersonCamera>();
            if (cameraManager.tpCamera && cameraManager.tpCamera.target != transform) cameraManager.tpCamera.SetMainTarget(this.transform);
        }
    }



    protected virtual void Update()
    {
        ControlCapsuleHeight();                   // checks the capsule height dependent on crouching, etc
    }


}
