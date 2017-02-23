using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject m_Target;
    Vector3 m_Offset;

    void Start()
    {
        m_Offset = m_Target.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        Quaternion rotation = Quaternion.Euler(0, m_Target.transform.eulerAngles.y, 0);
        transform.position = m_Target.transform.position - (rotation * m_Offset);

        transform.LookAt(m_Target.transform);
    }
}
