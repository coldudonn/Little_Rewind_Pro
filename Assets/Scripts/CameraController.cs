using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] Transform followTarget;

    [Header("Pan Input (WASD + Arrow Keys)")]
    [SerializeField] float panSpeed = 2.5f; // units per second
    [SerializeField] Vector2 panLimit = new Vector2(1.2f, 0.8f); // max pan range (x,y)
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    [Header("Smoothing")]
    [SerializeField] float followSmooth = 12f;
    [SerializeField] bool recenterWhenNoInput = true;
    [SerializeField] float recenterSpeed = 8f;

    [Header("Cursor")]
    [SerializeField] bool lockCursor = false;

    // "지정한 뷰" 고정용
    Vector3 baseOffset;        // 시작 시점의 카메라-타겟 오프셋(거리/뷰 유지)
    Quaternion baseRotation;   // 시작 시점의 회전(뷰 고정)

    // 입력 패닝(좌/우/상/하)
    Vector2 panOffset;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (followTarget == null)
        {
            Debug.LogError("[CameraController] followTarget이 비어있습니다. Inspector에서 플레이어 Transform을 넣어주세요.");
            enabled = false;
            return;
        }

        // 씬에서 배치한 카메라 뷰(위치/각도)를 기준으로 고정
        baseOffset = transform.position - followTarget.position;
        baseRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (followTarget == null) return;

        // 회전 고정(지정 뷰 유지)
        transform.rotation = baseRotation;

        float ix = invertX ? -1f : 1f;
        float iy = invertY ? -1f : 1f;

        // WASD + 방향키 입력(디지털) → -1, 0, 1
        float inputX = 0f;
        float inputY = 0f;

        // Left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inputX -= 1f;
        // Right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inputX += 1f;
        // Down
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputY -= 1f;
        // Up
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputY += 1f;

        inputX *= ix;
        inputY *= iy;

        // 누적 패닝(시간 보정)
        panOffset.x += inputX * panSpeed * Time.deltaTime;
        panOffset.y += inputY * panSpeed * Time.deltaTime;

        // 범위 제한
        panOffset.x = Mathf.Clamp(panOffset.x, -panLimit.x, panLimit.x);
        panOffset.y = Mathf.Clamp(panOffset.y, -panLimit.y, panLimit.y);

        // 입력 없으면 중앙으로 복귀(선택)
        if (recenterWhenNoInput && inputX == 0f && inputY == 0f)
        {
            panOffset = Vector2.Lerp(panOffset, Vector2.zero, Time.deltaTime * recenterSpeed);
        }

        // X/Y만 이동(카메라 Right/Up 방향) → 전진/후진(Z, forward) 변화 없음
        Vector3 panWorld = transform.right * panOffset.x + transform.up * panOffset.y;

        Vector3 desiredPos = followTarget.position + baseOffset + panWorld;

        // 타겟 추적 스무딩
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmooth);
    }

    // PlayerController에서 카메라 기준 이동 방향 계산에 쓰는 경우를 위해 유지
    public Quaternion PlanarRotation => Quaternion.Euler(0f, baseRotation.eulerAngles.y, 0f);
}
