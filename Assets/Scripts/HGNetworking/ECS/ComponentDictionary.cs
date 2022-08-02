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

    public bool Contains<T>(int index)
    {
        return ((Dictionary<int, T>)dict[typeof(T)]).ContainsKey(index);
    }

    public bool Contains(Type t, int index)
    {
        return true;
        //return ((Dictionary<int, (typeof(ComponentDictionary))>)dict[t]).ContainsKey(index);
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