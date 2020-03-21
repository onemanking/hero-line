using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UniRx;
using UnityEngine;

namespace Managers
{
	public class GameManager : MonoBehaviour
	{
		public enum GameState
		{
			Standby,
			Playing,
			Gameover
		}

		internal const string TagHero = "Hero";
		internal const string TagEnemy = "Enemy";
		internal const string TagWall = "Wall";

		private const double _SpawnRateSecond = 2.5f;

		private static GameManager _instance = null;
		public static GameManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType(typeof(GameManager)) as GameManager;
					if (_instance == null)
					{
						GameObject go = new GameObject();
						_instance = go.AddComponent<GameManager>();
						go.name = "GameManager";
					}
				}

				return _instance;
			}
		}
		[Header("LEVEL")]
		[SerializeField] private GameObject m_WallPrefab;
		[SerializeField] private int m_Row;
		[SerializeField] private int m_Colume;

		[Header("CHARACTER")]
		[SerializeField] private Hero m_AvatarPrefab;
		[SerializeField] private Character[] m_CharacterPrefabs;

		private List<Vector2> gridPosList = new List<Vector2>();
		private GameState _CurrentState;

		private void Start()
		{
			InitLevel();

			PlayerController.Instance.SetControllHero(SpawnAvatarHero());

			Observable.Interval(TimeSpan.FromSeconds(_SpawnRateSecond)).Subscribe(_ =>
			{
				SpawnCharacter();
			}).AddTo(this);

			Observable.EveryUpdate().Where(x => CheckShouldGameOver()).Subscribe(_ => GameOver()).AddTo(this);

			bool CheckShouldGameOver()
			{
				return PlayerController.Instance.HeroCount <= 0;
			}
		}

		private Hero SpawnAvatarHero()
		{
			var startPos = new Vector2(4, 7);
			return Instantiate(m_AvatarPrefab, startPos, Quaternion.identity);
		}

		private void InitLevel()
		{
			var pos = Vector2.zero;
			for (int y = 0; y < m_Colume; y++)
			{
				for (int x = 0; x < m_Row; x++)
				{
					gridPosList.Add(new Vector2(1 * x, 1 * y));
				}
			}

			var wallParent = new GameObject();
			wallParent.name = "BorderParent";
			for (int y = -1; y < m_Colume + 1; y++)
			{
				for (int x = -1; x < m_Row + 1; x++)
				{
					if (x == -1 || x == m_Row + 1 || y == -1 || y == m_Colume + 1 || x == m_Row || y == m_Row)
					{
						Instantiate(m_WallPrefab, new Vector2(1 * x, 1 * y), Quaternion.identity, wallParent.transform);
					}
				}
			}
		}

		private void SpawnCharacter()
		{
			var randomPosition = gridPosList[UnityEngine.Random.Range(0, gridPosList.Count)];
			var character = RandomCharacter();
			character.transform.position = randomPosition;

			Character RandomCharacter()
			{
				return Instantiate(m_CharacterPrefabs[UnityEngine.Random.Range(0, m_CharacterPrefabs.Length)]);
			}
		}

		public void GameOver()
		{
			_CurrentState = GameState.Gameover;
			// MenuManager.Instance.Gameover();
		}
	}
}