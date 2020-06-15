using UnityEngine;

namespace tanks
{
    public class PlayerControls : MonoBehaviour
    {
        private float m_VerticalInputValue;
        private float m_HorizontalInputValue;
        private float m_MouseYInputValue;
        private float m_MouseXInputValue;

        private TankControls m_TankControls;
        private GameObject m_TankBarrel;
        private LineRenderer m_FiringArc;

        private const int c_FiringArcPositions = 1000;

        private void Awake()
        {
            m_TankControls = GetComponent<TankControls>();
            m_TankBarrel = transform.Find("Renderers/Turret/Barrel").gameObject;
            m_FiringArc = transform.Find("Renderers/Turret/Barrel/ShellOriginTransform/FiringArc").GetComponent<LineRenderer>();
        }

        private void Start()
        {
            m_FiringArc.positionCount = c_FiringArcPositions;
        }

        void Update()
        {
            if (Time.timeScale == 0)
                return;

            m_VerticalInputValue = Input.GetAxis("Vertical");
            m_HorizontalInputValue = Input.GetAxis("Horizontal");
            m_MouseYInputValue = Input.GetAxis("Mouse Y");
            m_MouseXInputValue = Input.GetAxis("Mouse X");

            if (Input.GetButtonDown("Fire1"))
                m_TankControls.Fire();
        }

        private void FixedUpdate()
        {
            Quaternion tankRotation = m_TankControls.RotateTank(m_HorizontalInputValue);

            m_TankControls.RotateTurret(m_MouseXInputValue, tankRotation);
            m_TankControls.RotateBarrel(m_MouseYInputValue);
            m_TankControls.MoveTank(m_VerticalInputValue);

            UpdateFiringArc();
        }

        private void UpdateFiringArc()
        {
            Vector3[] arcArray = new Vector3[c_FiringArcPositions + 1];

            float fireAngle = m_TankBarrel.transform.rotation.eulerAngles.x;
            if (fireAngle > 180)
                fireAngle -= 360;
            float radianAngle = Mathf.Deg2Rad * -fireAngle;
            float v = m_TankControls.ShellVelocity;

            for (int i = 0; i <= c_FiringArcPositions; i++)
            {
                float t = i * Time.fixedDeltaTime;
                float z = v * t * Mathf.Cos(radianAngle);
                float y = v * t * Mathf.Sin(radianAngle) - ((-Physics.gravity.y * t * t) / 2);
                arcArray[i] = new Vector3(0f, y, z);
            }

            m_FiringArc.SetPositions(arcArray);
            // arc positions are calculated locally, so counter the x barrel rotation on the firing arc
            m_FiringArc.transform.localRotation = Quaternion.Euler(-m_TankBarrel.transform.localEulerAngles.x, m_FiringArc.transform.localEulerAngles.y, m_FiringArc.transform.localEulerAngles.z);
        }
    }
}