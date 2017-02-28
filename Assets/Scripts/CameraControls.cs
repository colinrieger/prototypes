using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [HideInInspector] public GameObject m_Target;
    Vector3 m_Offset = new Vector3(0f, -2f, 6f);

    private void FixedUpdate()
    {
        if (!m_Target)
            return;
        Quaternion rotation = Quaternion.Euler(0, m_Target.transform.eulerAngles.y, 0);
        transform.position = m_Target.transform.position - (rotation * m_Offset);

        transform.LookAt(m_Target.transform);
    }
}
