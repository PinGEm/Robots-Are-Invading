using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<ObjectPoolInfo> ObjectPools = new List<ObjectPoolInfo>();
    private GameObject _objectPoolEmptyHolder;

    private static GameObject _bulletEmpty;
    private static GameObject _enemyEmpty;
    private static GameObject _gameObjectsEmpty;

    public enum PoolType
    {
        Bullet,
        Enemy,
        Gameobject,
        None,
    }
    public static PoolType PoolingType;

    private void Awake()
    {
        SetupEmpties();
    }

    private void SetupEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Objects");

        _bulletEmpty = new GameObject("Bullet");
        _bulletEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _enemyEmpty = new GameObject("Enemy");
        _enemyEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _gameObjectsEmpty = new GameObject("GameObjects");
        _gameObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        ObjectPoolInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name); // Search through object pools
                                                                                           // Returns the name of what we want to spawn equal to the LookupString (Enemy)

        // If the pool doesn't exist, create it
        if(pool == null)
        {
            pool = new ObjectPoolInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if(spawnableObj == null)
        {
            // Find the parent of the empty object
            GameObject parentObject = SetParentObject(poolType);

            // If there are no inactive objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            if(parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            // If there is an inactive object, reactivate it
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length -7); // Removes the "(Clone)" bit in the inspector tab of the pool.
        ObjectPoolInfo pool = ObjectPools.Find(p => p.LookupString == goName);

        if(pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Bullet:
                return _bulletEmpty;
                
            case PoolType.Enemy:
                return _enemyEmpty;

            case PoolType.Gameobject:
                return _gameObjectsEmpty;

            case PoolType.None:
                return null;

            default:
                return null;
        }
    }
}

public class ObjectPoolInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}