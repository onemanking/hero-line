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
		protected FloatReactiveProperty Hp = new FloatReactiveProperty();
		protected FloatReactiveProperty Atk = new FloatReactiveProperty();
		protected FloatReactiveProperty Def = new FloatReactiveProperty();
		protected Type Type;

		protected virtual void Start()
		{
			GetComponent<SpriteRenderer>().sprite = GlobalUtility.RandomSprite();

			GenarateStats();
		}

		protected void FightEnemy(Character _other)
		{
			var damage = CalculateDamage(_other.Atk.Value);
			TakeDamage(damage);
		}

		private void GenarateStats()
		{
			Hp.Value = GlobalUtility.RandomHp();
			Atk.Value = GlobalUtility.RandomAtk();
			Def.Value = GlobalUtility.RandomDef();
			Type = GlobalUtility.RandomType();
		}

		protected void TakeDamage(float _damage) => Hp.Value -= _damage;

		private float CalculateDamage(float _atk)
		{
			return _atk - Def.Value;
		}
	}
}