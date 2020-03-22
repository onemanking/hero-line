using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour
{
	private static PlayerController _instance = null;
	public static PlayerController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(PlayerController)) as PlayerController;
				if (_instance == null)
				{
					GameObject go = new GameObject();
					_instance = go.AddComponent<PlayerController>();
					go.name = "PlayerController";
				}
			}
			return _instance;
		}
	}

	public List<Hero> HeroList { get; private set; }
	public List<Vector2> HeroLinePositionList { get; private set; }
	public Hero ControllHero { get; private set; }
	public int HeroCount => HeroList.Count;

	void Start()
	{
		HeroList = new List<Hero>();
		HeroLinePositionList = new List<Vector2>();

		Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Z)).Subscribe(_ => RotateHeroLeader(true)).AddTo(this);
		// Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.X)).Subscribe(_ => RotateHeroLeader(false)).AddTo(this); 
	}

	public void SetControllHero(Hero _hero)
	{
		ControllHero = _hero;
		AddHero(_hero);
		_hero.SetControllable(true);
	}

	public void AddHero(Hero _hero)
	{
		if (!HeroList.Contains(_hero)) HeroList.Add(_hero);

		if (HeroList.Count == 0)
			_hero.transform.position = transform.position;
		else
			_hero.transform.position = HeroList[HeroList.Count - 1].transform.position;
	}

	internal void UpdateHeroLine(Vector3 _position)
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
				HeroList[i].SetFacing(ControllHero.Direction);
			}
		}
	}

	internal void RemoveHeroLeader()
	{
		HeroList.Remove(ControllHero);
		Destroy(ControllHero.gameObject);

		UpdateNewLeader();

		void UpdateNewLeader()
		{
			if (HeroList.Count <= 0) return;

			var newLeader = HeroList.First();
			newLeader.InvertDirection();
			SetControllHero(newLeader);

			UpdateHeroLine(newLeader.transform.position);
		}
	}

	private void RotateHeroLeader(bool _rotateForward)
	{
		if (HeroList.Count <= 1) return;

		var oldLeader = ControllHero;
		var newLeader = HeroList[1];

		oldLeader.SetControllable(false);

		HeroList.Rotate(_rotateForward);

		SetControllHero(newLeader);
	}

}
