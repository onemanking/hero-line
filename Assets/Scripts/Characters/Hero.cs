using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UniRx;
using UnityEngine;

namespace Characters
{
	public class Hero : Character
	{
		private const string _AXIS_HORIZONTAL = "Horizontal";
		private const string _AXIS_VERTICAL = "Vertical";

		public Vector2 Direction { get; private set; }
		public float Speed { get; private set; } = 1f;

		private bool _Controllable;
		private SpriteRenderer _SpriteRenderer;

		protected override void Start()
		{
			base.Start();

			_SpriteRenderer = GetComponent<SpriteRenderer>();

			SetStartDirection(Vector2.right);

			CheckForInput();

			Move();

			CheckFacing();

			void CheckForInput()
			{
				var inputHorizontal = Observable.EveryUpdate()
					.Where(x => Input.GetAxis(_AXIS_HORIZONTAL) != 0 && _Controllable && !IsInCombat)
					.Select(_ => Input.GetAxis(_AXIS_HORIZONTAL));
				var inputVertical = Observable.EveryUpdate()
					.Where(x => Input.GetAxis(_AXIS_VERTICAL) != 0 && _Controllable && !IsInCombat)
					.Select(_ => Input.GetAxis(_AXIS_VERTICAL));

				inputHorizontal.Where(x => Direction != Vector2.right && Direction.x > x || Direction != Vector2.left && Direction.x < x)
					.Subscribe(_horizontal =>
					{
						Direction = _horizontal > 0 ? Vector2.right : Vector2.left;
					}).AddTo(this);

				inputVertical.Where(y => Direction != Vector2.up && Direction.y > y || Direction != Vector2.down && Direction.y < y)
					.Subscribe(_vertical =>
					{
						Direction = _vertical > 0 ? Vector2.up : Vector2.down;
					}).AddTo(this);
			}

			void SetStartDirection(Vector2 _dir) => Direction = _dir;

			void CheckFacing()
			{
				Observable.EveryUpdate().Where(x => Direction.x != 0)
									.Select(_ => Direction.x >= 1)
									.Subscribe(_isFacingRight => _SpriteRenderer.flipX = !_isFacingRight)
									.AddTo(this);
			}
		}

		public void SetFacing(Vector2 _direction) => Direction = _direction;

		public void SetControllable(bool _controllable) => _Controllable = _controllable;

		private IDisposable _MoveDisposable;
		private void Move()
		{
			_MoveDisposable?.Dispose();
			_MoveDisposable = Observable.Interval(TimeSpan.FromSeconds(1f / Speed))
			.Where(x => _Controllable && !IsInCombat && GameManager.Instance.IsPlaying).Subscribe
			(
				_ =>
				{
					transform.Translate(Direction);

					PlayerController.Instance.UpdateHeroLine(transform.position);
				}
			).AddTo(this);
		}

		private void OnTriggerEnter2D(Collider2D _other)
		{
			if (!_Controllable) return;

			if (_other.tag == GameManager.TagHero)
				PlayerController.Instance.AddHero(_other.GetComponent<Hero>());
			else if (_other.tag == GameManager.TagEnemy)
				GameManager.Instance.Combat(this, _other.GetComponent<Enemy>());
			else
				HitWall();
		}

		private void HitWall()
		{
			Hp.Value = 0;
			PlayerController.Instance.HitWallRemoveLeader(this);
		}

		public void IncreaseSpeed(float _value)
		{
			Speed += _value;
			Move();
		}
		public void SetSpeed(float _value)
		{
			Speed = _value;
			Move();
		}

		public void InvertDirection() => Direction *= -1;

		public void SetPosition(Vector2 _pos) => transform.position = _pos;

		protected override void Die()
		{
			PlayerController.Instance.RotateHeroLeader();
			PlayerController.Instance.RemoveHero(this);
			base.Die();
		}
	}
}
