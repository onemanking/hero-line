using System.Collections.Generic;
using Characters;
using UnityEngine;
using UniRx;
using System.Linq;
using Utilities;

public class PlayerController : Singleton<PlayerController>
{
	public List<Hero> HeroList { get; private set; }
	public List<Vector2> HeroLinePositionList { get; private set; }
	public int HeroCount => HeroList.Count;
	public Hero CurrentLeader => HeroList.FirstOrDefault();
	private float CurrentSpeed;

	void Start()
	{
		HeroList = new List<Hero>();
		HeroLinePositionList = new List<Vector2>();

		Observable.EveryUpdate()
			.Where(_ => Input.GetKeyDown(KeyCode.Z) && HeroList.All(x => !x.IsInCombat))
			.Subscribe(_ => RotateHeroLeader(true))
			.AddTo(this);
	}

	public void IncreasePartySpeed(float _value)
	{
		CurrentSpeed += _value;
		foreach (var hero in HeroList)
		{
			hero.SetSpeed(CurrentSpeed);
		}
	}

	public void SetLeaderHero(Hero _hero, bool _isFirstStart = false)
	{
		AddHero(_hero);
		_hero.SetControllable(true);

		if (_isFirstStart)
			CurrentSpeed = _hero.Speed;
	}

	public void AddHero(Hero _hero)
	{
		if (!HeroList.Contains(_hero))
			HeroList.Add(_hero);
	}

	public void UpdateHeroLine(Vector3 _position)
	{
		HeroLinePositionList.Insert(0, _position);

		if (HeroLinePositionList.Count > HeroList.Count + 1)
		{
			HeroLinePositionList.RemoveAt(HeroLinePositionList.Count - 1);
		}

		if (HeroList.Count > 0)
		{
			for (int i = 0; i < HeroList.Count; i++)
			{
				HeroList[i].SetPosition(HeroLinePositionList[i]);
				HeroList[i].SetFacing(CurrentLeader.Direction);
			}
		}
	}

	public void RemoveHero(Hero _hero)
	{
		HeroList.Remove(_hero);
	}

	public void HitWallRemoveLeader(Hero _hero)
	{
		RemoveHero(_hero);

		UpdateHitWallNewLeader();

		void UpdateHitWallNewLeader()
		{
			if (HeroList.Count <= 0) return;

			HeroLinePositionList.Remove(_hero.transform.position);

			var newLeader = HeroList.First();
			newLeader.InvertDirection();
			SetLeaderHero(newLeader);
		}
	}

	public void RotateHeroLeader(bool _rotateForward = true)
	{
		if (HeroList.Count <= 1) return;

		var oldLeader = CurrentLeader;
		var newLeader = HeroList[1];

		oldLeader.SetControllable(false);

		HeroList.Rotate(_rotateForward);

		SetLeaderHero(newLeader);
	}

}
