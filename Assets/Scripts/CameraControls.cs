using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject Target { get; set; }

    private Vector3 m_PositionOffset = new Vector3(0f, -2f, 8f);

    private void FixedUpdate()
    {
        if (!Target)
            return;

        Quaternion rotation = Quaternion.Euler(0, Target.transform.eulerAngles.y, 0);
        transform.position = Target.transform.position - (rotation * m_PositionOffset);

        transform.LookAt(Target.transform.position);
    }
}
