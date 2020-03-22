using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Utilities;

public class CameraManager : Singleton<CameraManager>
{
	private Transform _CamTransform;
	private Vector3 _OriginalPos;
	private IDisposable _ShakeDisposable;

	protected override void Awake()
	{
		base.Awake();

		_CamTransform = GetComponent<Transform>();
	}

	public void Shake(float _shakeAmount, float _decreaseFactor, float _shakeDuration)
	{
		_ShakeDisposable?.Dispose();
		_ShakeDisposable = Observable.EveryUpdate().Subscribe(_ =>
		{
			_CamTransform.localPosition = _OriginalPos + UnityEngine.Random.insideUnitSphere * _shakeAmount;

			_shakeDuration -= Time.deltaTime * _decreaseFactor;

			if (_shakeDuration <= 0)
			{
				_CamTransform.localPosition = _OriginalPos;
				_ShakeDisposable.Dispose();
			}
		}).AddTo(this);
	}

	void OnEnable() => _OriginalPos = _CamTransform.localPosition;
}
