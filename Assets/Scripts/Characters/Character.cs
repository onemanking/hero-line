using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UniRx;

namespace Characters
{
	public enum Type { Red, Green, Blue }

	public class Character : MonoBehaviour
	{
		public FloatReactiveProperty Hp { get; private set; } = new FloatReactiveProperty();
		public FloatReactiveProperty Atk { get; private set; } = new FloatReactiveProperty();
		public FloatReactiveProperty Def { get; private set; } = new FloatReactiveProperty();
		public Type Type { get; private set; }
		public bool IsInCombat { get; private set; }

		protected virtual void Start()
		{
			GetComponent<SpriteRenderer>().sprite = GlobalUtility.RandomSprite();

			GenarateStats();

			Hp.Where(x => x <= 0).Subscribe(_ => Die()).AddTo(this);
		}

		protected virtual void Die() => Destroy(gameObject);

		private void GenarateStats()
		{
			Hp.Value = GlobalUtility.RandomHp();
			Atk.Value = GlobalUtility.RandomAtk();
			Def.Value = GlobalUtility.RandomDef();
			Type = GlobalUtility.RandomType();
		}

		public void TakeDamage(float _atk)
		{
			Hp.Value -= CalculateDamage(_atk);

			float CalculateDamage(float _atkLocal)
			{
				return _atkLocal - Def.Value < GlobalUtility.LOWEST_DAMAGE ? GlobalUtility.LOWEST_DAMAGE : _atkLocal - Def.Value;
			}
		}
		public void SetIsInCombat(bool _isInCombat) => IsInCombat = _isInCombat;
	}
}