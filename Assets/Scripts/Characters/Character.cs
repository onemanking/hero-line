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
		[Header("STAT")]
		protected FloatReactiveProperty Hp = new FloatReactiveProperty();
		protected FloatReactiveProperty Atk = new FloatReactiveProperty();
		protected FloatReactiveProperty Def = new FloatReactiveProperty();
		protected Type Type;

		private void Start()
		{
			GenarateStats();
		}

		private void GenarateStats()
		{
			Hp.Value = GlobalUtility.RandomHp();
			Atk.Value = GlobalUtility.RandomAtk();
			Def.Value = GlobalUtility.RandomDef();
			Type = GlobalUtility.RandomType();
		}

		protected void TakeDamage(float _damage) => Hp.Value -= _damage;
	}
}