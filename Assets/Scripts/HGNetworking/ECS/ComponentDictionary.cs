using ECSSkeleton;
using System;
using System.Collections.Generic;

public class ComponentDictionary
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

    public void AddComponentType<T>()
    {
        dict[typeof(T)] = new Dictionary<int, T>();
    }

    public void Delete<T>(int index)
    {
        ((Dictionary<int, T>)dict[typeof(T)]).Remove(index);
    }
    public bool Contains(Type type, int index)
    {
        switch (type)
        {
            case Type RigidBodyComponent:
                return Contains<RigidBodyComponent>(index);
            default:
                throw new ArgumentException($"Type is not currently handled by Contains: {type}");
        }
    }

    public bool Contains<T>(int index)
    {
        return ((Dictionary<int, T>)dict[typeof(T)]).ContainsKey(index);
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