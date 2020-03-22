using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Type = Characters.Type;

namespace Utilities
{
	public static class GlobalUtility
	{
		public const float LOWEST_DAMAGE = 1f;

		private const float _MIN_HP = 50f;
		private const float _MIN_ATK = 50f;
		private const float _MIN_DEF = 50f;
		private const float _MAX_HP = 100f;
		private const float _MAX_ATK = 100f;
		private const float _MAX_DEF = 100f;
		private static Sprite[] _SPRITE_RESOURCES;


		public static float RandomHp() => Random.Range(_MIN_HP, _MAX_HP);
		public static float RandomAtk() => Random.Range(_MIN_ATK, _MAX_ATK);
		public static float RandomDef() => Random.Range(_MIN_DEF, _MAX_DEF);
		public static Type RandomType()
		{
			var v = Enum.GetValues(typeof(Type));
			return (Type)v.GetValue(new System.Random().Next(v.Length));
		}
		public static Sprite RandomSprite()
		{
			if (_SPRITE_RESOURCES == null) LoadSprites();

			return _SPRITE_RESOURCES[Random.Range(0, _SPRITE_RESOURCES.Length)];

			void LoadSprites()
			{
				_SPRITE_RESOURCES = Resources.LoadAll("Characters", typeof(Sprite)).Select(x => (Sprite)x).ToArray();
			}
		}
	}
}
