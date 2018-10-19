using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Nucleon : MonoBehaviour 
{
    [SerializeField] private float attractionForce;

    Rigidbody rb;

	//========================================================

	void Awake () 
	{
        rb = GetComponent<Rigidbody>();
	}
	

	void FixedUpdate () 
	{
        rb.AddForce(transform.localPosition * -attractionForce);
	}

	//========================================================

	
}
