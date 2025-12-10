using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    [Header("풀링할 프리팹")]
    [SerializeField] protected GameObject _prefab;

    [Header("초기 생성 개수")]
    [SerializeField] protected int _initialSize = 10;

    // 실제로 오브젝트를 저장하는 큐
    protected Queue<GameObject> _pool = new Queue<GameObject>();

    protected virtual void Awake()
    {
        InitPool();
    }

    // 처음에 미리 만들어 둠
    protected virtual void InitPool()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            GameObject newObject = CreateNewObject();
            newObject.SetActive(false);
            _pool.Enqueue(newObject);
        }
    }

    // 새 오브젝트를 만드는 부분 (필요하면 자식에서 override)
    protected virtual GameObject CreateNewObject()
    {
        GameObject newObject = Instantiate(_prefab, transform);
        return newObject;
    }

    // 풀에서 하나 꺼내오기
    public virtual GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject newObject;

        if (_pool.Count > 0)
        {
            newObject = _pool.Dequeue();
        }
        else
        {
            // 부족하면 새로 생성
            newObject = CreateNewObject();
        }

        newObject.transform.SetPositionAndRotation(position, rotation);
        newObject.SetActive(true);
        return newObject;
    }

    // 사용이 끝난 오브젝트 돌려보내기
    public virtual void ReturnToPool(GameObject returnObject)
    {
        returnObject.SetActive(false);
        _pool.Enqueue(returnObject);
    }
}
