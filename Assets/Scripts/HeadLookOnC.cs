using UnityEngine;

public class HeadLookOnC : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode headKey = KeyCode.C;

    [Header("Bones (optional if Humanoid)")]
    [SerializeField] Animator animator;
    [SerializeField] Transform neckBone;
    [SerializeField] Transform headBone;

    [Header("Rotation")]
    [SerializeField] float yawAngle = 45f;          // 좌/우 회전 각도
    [SerializeField] float turnSpeed = 240f;        // 각도 변화 속도(도/초)
    [SerializeField, Range(0f, 1f)] float neckShare = 0.6f; // 목에 분배 비율(0.6 추천)

    float currentYaw;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        // Humanoid면 자동으로 Head/Neck 잡기
        if (animator != null && animator.isHuman)
        {
            if (neckBone == null) neckBone = animator.GetBoneTransform(HumanBodyBones.Neck);
            if (headBone == null) headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        }
    }

    void LateUpdate()
    {
        if (headBone == null && neckBone == null) return;

        // C 누르면 "왼쪽"으로만 돌리기 (원하면 아래 조건 바꿔)
        float targetYaw = Input.GetKey(headKey) ? -yawAngle : 0f;

        currentYaw = Mathf.MoveTowards(currentYaw, targetYaw, turnSpeed * Time.deltaTime);

        float neckYaw = currentYaw * neckShare;
        float headYaw = currentYaw * (1f - neckShare);

        // neck 없으면 head에 전부
        if (neckBone == null && headBone != null)
        {
            headYaw = currentYaw;
            neckYaw = 0f;
        }

        if (neckBone != null)
            neckBone.localRotation = neckBone.localRotation * Quaternion.Euler(0f, neckYaw, 0f);

        if (headBone != null)
            headBone.localRotation = headBone.localRotation * Quaternion.Euler(0f, headYaw, 0f);
    }
}
