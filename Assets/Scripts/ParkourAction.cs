using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [SerializeField] bool rotateToObstacle;
    [SerializeField] float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 matchPosWeight = new Vector3(0, 1, 0);

    [Header("Climb Offset")]
    [SerializeField] float matchPosYOffset = 0f;

    public Quaternion TargetRotation { get; set; }

    public Vector3 MatchPos { get; set; }
    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
        {
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
        }
        if (enableTargetMatching)
        {
            Vector3 pos = hitData.heightHit.point;
            pos.y += matchPosYOffset;
            MatchPos = pos;
        }

        return true;

    }

    public string AnimName => animName;

    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;

    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;

    public Vector3 MatchPosWeight => matchPosWeight;
}