using System;
using System.Collections.Generic;

class ComponentDictionary
{
    Dictionary<Type, dynamic> dict;

    public ComponentDictionary()
    {
        dict = new Dictionary<Type, dynamic>();
    }

    public void Add<T>(int index, T component)
    {
        ((Dictionary<int, T>)dict[typeof(T)])[index] = component;
    }

    public void Delete<T>(int index)
    {
        ((Dictionary<int, T>)dict[typeof(T)]).Remove(index);
    }

    public void Contains<T>(int index)
    {
        ((Dictionary<int, T>)dict[typeof(T)]).ContainsKey(index);
    }

    public T GetValueAtIndex<T>(int index)
    {
        return ((Dictionary<int, T>)dict[typeof(T)])[index];
    }

    public Dictionary<int, T>.ValueCollection GetValues<T>()
    {
        Type t = typeof(T);
        return ((Dictionary<int, T>)dict[typeof(T)]).Values;
    }
}