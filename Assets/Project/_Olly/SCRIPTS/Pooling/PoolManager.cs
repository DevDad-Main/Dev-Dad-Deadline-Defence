using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown {

    [System.Serializable]
    public class PoolObject
    {
        public string key;
        public GameObject prefab;
        public int initialSize = 10;
    }

    
    public class PoolManager : MonoBehaviour {
        public static PoolManager Instance;

        [SerializeField] private List<PoolObject> poolObjects;

        private Dictionary<string, Queue<GameObject>> _poolDictionary = new();

        private void Awake()
        {
            Instance = this;

            foreach (var obj in poolObjects)
            {
                var queue = new Queue<GameObject>();
                for (int i = 0; i < obj.initialSize; i++)
                {
                    var go = Instantiate(obj.prefab);
                    go.transform.SetParent(transform);
                    go.SetActive(false);
                    queue.Enqueue(go);
                }
                _poolDictionary.Add(obj.key, queue);
            }

            DontDestroyOnLoad(gameObject);
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning($"Pool with key {key} not found.");
                return null;
            }

            GameObject go = _poolDictionary[key].Count > 0
                ? _poolDictionary[key].Dequeue()
                : Instantiate(GetPrefabByKey(key));

            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.SetParent(transform);
            
            go.SetActive(true);

            if (go.TryGetComponent(out IPoolable poolable))
                poolable.OnSpawnFromPool();

            return go;
        }

        public void ReturnToPool(string key, GameObject go)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning($"Pool with key {key} not found.");
                Destroy(go);
                return;
            }

            if (go.TryGetComponent(out IPoolable poolable))
                poolable.OnReturnToPool();

            go.SetActive(false);
            _poolDictionary[key].Enqueue(go);
        }

        private GameObject GetPrefabByKey(string key)
        {
            foreach (var obj in poolObjects)
            {
                if (obj.key == key)
                    return obj.prefab;
            }
            return null;
        }
    }
}
