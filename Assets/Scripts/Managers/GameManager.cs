using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Managers
{
	public class GameManager : Singleton<GameManager>
	{
		public enum GameState
		{
			Standby,
			Playing,
			GameOver
		}

		internal const string TagHero = "Hero";
		internal const string TagEnemy = "Enemy";
		internal const string TagWall = "Wall";

		private const double _SpawnRateSecond = 2.5f;
		private const string _STAND_BY_MESSAGE = "Press Z to Rotate Hero\n\nPress Space to Start Game";
		private const string _GAME_OVER_MESSAGE = "GameOver";

		[Header("CONFIG")]
		[SerializeField] private float m_SpeedIncreasement = 0.1f;
		[SerializeField] private float m_TypeMultiplier = 2f;
		[SerializeField] private double m_CombatResolveSec = 0.5f;


		[Header("LEVEL")]
		[SerializeField] private GameObject m_WallPrefab;
		[SerializeField] private int m_Row;
		[SerializeField] private int m_Colume;

		[Header("CHARACTER")]
		[SerializeField] private Hero m_AvatarPrefab;
		[SerializeField] private Character[] m_CharacterPrefabs;

		[Header("UI")]
		[SerializeField] private Text m_ScoreText;
		[SerializeField] private Text m_GameStateText;

		private GameState _GameState;
		public bool IsPlaying => _GameState == GameState.Playing;

		private FloatReactiveProperty _Score = new FloatReactiveProperty();
		private List<Vector2> _GridPosList = new List<Vector2>();

		private void Start()
		{
			InitLevel();

			StartGameInput();

			CheckSpawnCharacter();

			CheckGameOver();

			CheckScore();

			CheckStandbyMessage();

			CheckGameOverMessage();

			void StartGameInput()
			{
				Observable.EveryUpdate().Where(x => Input.GetKeyDown(KeyCode.Space) && _GameState == GameState.Standby)
							.Subscribe(_ => StartGame())
							.AddTo(this);
			}

			void CheckSpawnCharacter()
			{
				Observable.Interval(TimeSpan.FromSeconds(_SpawnRateSecond))
							.Where(x => _GameState == GameState.Playing && !PlayerController.Instance.CurrentLeader.IsInCombat)
							.Subscribe(_ => SpawnCharacter())
							.AddTo(this);
			}

			void CheckGameOver()
			{
				Observable.EveryUpdate().Where(x => CheckShouldGameOver()).Subscribe(_ => GameOver()).AddTo(this);

				bool CheckShouldGameOver()
				{
					return PlayerController.Instance.HeroCount <= 0 && _GameState == GameState.Playing && _GameState != GameState.GameOver;
				}
			}

			void CheckScore()
			{
				_Score.ObserveEveryValueChanged(_ => _.Value.ToString("0.##")).Subscribe(_score => m_ScoreText.text = _score).AddTo(this);
			}

			void CheckStandbyMessage()
			{
				Observable.EveryUpdate().Where(x => _GameState == GameState.Standby && m_GameStateText.text != _STAND_BY_MESSAGE)
							.Subscribe(_ => m_GameStateText.text = _STAND_BY_MESSAGE)
							.AddTo(this);
			}

			void CheckGameOverMessage()
			{
				Observable.EveryUpdate().Where(x => _GameState == GameState.GameOver && m_GameStateText.text != _GAME_OVER_MESSAGE)
							.Subscribe(_ => m_GameStateText.text = _GAME_OVER_MESSAGE)
							.AddTo(this);
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
					_GridPosList.Add(new Vector2(1 * x, 1 * y));
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
			var randomPosition = _GridPosList[UnityEngine.Random.Range(0, _GridPosList.Count)];
			var character = RandomCharacter();
			character.transform.position = randomPosition;

			Character RandomCharacter()
			{
				return Instantiate(m_CharacterPrefabs[UnityEngine.Random.Range(0, m_CharacterPrefabs.Length)]);
			}
		}

		private void StartGame()
		{
			_GameState = GameState.Playing;
			m_GameStateText.text = string.Empty;
			PlayerController.Instance.SetLeaderHero(SpawnAvatarHero(), true);
		}

		private void GameOver()
		{
			_GameState = GameState.GameOver;
			Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => Restart()).AddTo(this);
		}

		private void Restart()
		{
			_GameState = GameState.Standby;
			_Score.Value = 0f;
			foreach (var character in FindObjectsOfType<Character>())
			{
				Destroy(character.gameObject);
			}
		}

		private IDisposable _CombatDisposable;
		public void Combat(Hero _hero, Enemy _enemy)
		{
			_enemy.SetIsInCombat(true);
			_hero.SetIsInCombat(true);

			var heroDmg = GetHeroDmg(_hero, _enemy);

			_CombatDisposable?.Dispose();
			_CombatDisposable = Observable.Interval(TimeSpan.FromSeconds(m_CombatResolveSec)).Subscribe(_ =>
			{
				CameraManager.Instance.Shake(0.2f, 1f, 0.2f);

				_enemy.TakeDamage(heroDmg);

				if (_enemy.Hp.Value > 0) _hero.TakeDamage(_enemy.Atk.Value);

				if (_hero.Hp.Value <= 0 || _enemy.Hp.Value <= 0)
				{
					_enemy.SetIsInCombat(false);
					_hero.SetIsInCombat(false);

					_Score.Value += _hero.Hp.Value > 0 ? _hero.Hp.Value : 0;

					if (_hero.Hp.Value > 0)
						PlayerController.Instance.IncreasePartySpeed(m_SpeedIncreasement);

					_CombatDisposable.Dispose();
				}
			}).AddTo(this);

			float GetHeroDmg(Hero _heroLocal, Enemy _enemyLocal)
			{
				return _heroLocal.Type == _enemyLocal.Type ? _heroLocal.Atk.Value * m_TypeMultiplier : _heroLocal.Atk.Value;
			}
		}
	}
}