using UnityEngine;

namespace Utilities
{
	public class Singleton<T> : MonoBehaviour
	{
		public static T Instance => _Instance;
		private static T _Instance;

		protected virtual void Awake() => _Instance = GetComponent<T>();
	}
}