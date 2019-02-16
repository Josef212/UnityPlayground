using UnityEngine;

public class DynHoleMaker : MonoBehaviour
{
    public float Radius { get { return m_radius; } }


    [SerializeField] private float m_radius = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }
}
