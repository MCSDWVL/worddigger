using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	private Vector3 _originalPosition;
	private Quaternion _originalRotation;
	private float _intensity;
	private float _decay;

	private void Awake()
	{
		_originalPosition = transform.position;
		_originalRotation = transform.rotation;
	}

	private void Update()
	{
		if(_intensity > 0)
		{
			transform.position = _originalPosition + Random.insideUnitSphere * _intensity;
			_intensity -= _decay * Time.deltaTime;

			if(_intensity <= 0)
				transform.position = _originalPosition;
		}
	}

	public void Shake(float intensity = .3f, float decay = .6f)
	{
		_intensity = intensity;
		_decay = decay;
	}
}