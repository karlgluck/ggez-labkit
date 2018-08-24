using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityObject = UnityEngine.Object;

namespace GGEZ.FullSerializer.Internal
{
    public class fsUnityReferenceManager
    {
        
        private class UnityObjectReferenceEqualityComparator : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return (x as UnityObject).GetInstanceID() == (y as UnityObject).GetInstanceID();
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return (obj as UnityObject).GetInstanceID();
            }

            public static readonly IEqualityComparer<object> Instance = new UnityObjectReferenceEqualityComparator();
        }

        private Dictionary<object, int> _objectIds = new Dictionary<object, int>(UnityObjectReferenceEqualityComparator.Instance);
        private List<UnityObject> _references = null;

        public bool Enabled
        {
            get
            {
                return References != null;
            }
        }

        public List<UnityObject> References
        {
            get { return _references; }
            set
            {
                _references = value;
                _objectIds.Clear();
                for (int i = _references.Count - 1; i >= 0; --i)
                {
                    if (_references[i] == null)
                    {
                        _references.RemoveAt(i);
                    }
                }
                for (int id = 0; id < _references.Count; ++id)
                {
                    _objectIds[_references[id]] = id;
                }
            }
        }

        public object GetUnityObject(int id)
        {
            if (id < 0 || id >= _references.Count)
            {
                return null;
            }
            return _references[id];
        }

        public int GetReferenceId(object item)
        {
            UnityObject unityObject = item as UnityObject;
            int id;
            if (_objectIds.TryGetValue(item, out id) == false)
            {
                id = _references.Count;
                _references.Add(unityObject);
                _objectIds[unityObject] = id;
            }
            return id;
        }

    }
}