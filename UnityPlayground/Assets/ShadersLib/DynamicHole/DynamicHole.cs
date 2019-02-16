using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicHole : MonoBehaviour
{

    [SerializeField] private DynHoleMaker m_holeMaker = null;

    static int m_holeMakerPositionID = Shader.PropertyToID("_HoleMakerPosition");
    static int m_holeMakerRadiusID = Shader.PropertyToID("_HoleMakerRadius");
    static MaterialPropertyBlock m_materialPropertyBlock = null;

    private MeshRenderer m_meshRenderer = null;

    void Awake()
    {
        OnValidate();
    }

	void Update ()
    {
        if(m_materialPropertyBlock == null)
        {
            m_materialPropertyBlock = new MaterialPropertyBlock();
        }

        m_materialPropertyBlock.SetVector(m_holeMakerPositionID, m_holeMaker.transform.position);
        m_materialPropertyBlock.SetFloat(m_holeMakerRadiusID, m_holeMaker.Radius);
        m_meshRenderer.SetPropertyBlock(m_materialPropertyBlock);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_materialPropertyBlock == null)
        {
            m_materialPropertyBlock = new MaterialPropertyBlock();
        }

        if(m_meshRenderer == null)
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
        }

        m_materialPropertyBlock.SetFloat(m_holeMakerRadiusID, m_holeMaker.Radius);
        m_meshRenderer.SetPropertyBlock(m_materialPropertyBlock);
    }
#endif
}
