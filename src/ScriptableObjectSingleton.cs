using UnityEngine;

namespace Unremove;

class ScriptableObjectSingleton<T>(System.Action<T> Setup)
where T : ScriptableObject
{
    T _obj = null!;
    internal T Instance => _obj ? _obj : Instantiate();

    T Instantiate()
    {
        _obj = ScriptableObject.CreateInstance<T>();

        Setup(_obj);

        return _obj;
    }
}
