using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ListExtension
{
	public static void Swap<T>(this List<T> _list, int _indexA, int _indexB)
	{
		T tmp = _list[_indexA];
		_list[_indexA] = _list[_indexB];
		_list[_indexB] = tmp;
	}

	public static void Swap<T>(this List<T> _list, T _elementA, T _elementB)
	{
		var indexA = _list.FindIndex(x => x.Equals(_elementA));
		var indexB = _list.FindIndex(x => x.Equals(_elementB));
		_list.Swap(indexA, indexB);
	}

	public static void Rotate<T>(this List<T> _list, bool _forward = true)
	{
		if (_forward)
		{
			var temp = _list.First();
			for (var i = 0; i < _list.Count - 1; i++)
			{
				_list[i] = _list[i + 1];
			}
			_list[_list.Count - 1] = temp;
		}
		else
		{
			var temp = _list.Last();
			for (var i = _list.Count - 1; i > 0; i--)
			{
				_list[i] = _list[i - 1];
			}
			_list[0] = temp;
		}
	}

}
