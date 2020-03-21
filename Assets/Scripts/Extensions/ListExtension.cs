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
}
