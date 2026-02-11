using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    public static SaveSystem Instance { get; private set; }

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        _player = GameObject.Find("Player");

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public void Save(Vector3 position, Quaternion rotation)
    {
        SaveInformation saveInformation = new SaveInformation()
        {
            _position = position,
            _rotation = rotation
        };

        Debug.Log(saveInformation._position);

        string json = JsonUtility.ToJson(saveInformation);

        File.WriteAllText(Application.persistentDataPath + "/save.txt", json);
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.txt"))
        {
            string saveFile = File.ReadAllText(Application.persistentDataPath + "/save.txt");
            SaveInformation loadedInformation = JsonUtility.FromJson<SaveInformation>(saveFile);

            Debug.Log(loadedInformation._position);

            _player.transform.position = loadedInformation._position;
            _player.transform.rotation = loadedInformation._rotation;
        }
        else
        {
            Debug.LogError("NO SAVE FILE FOUND...");
        }
    }
}

public class SaveInformation
{
    public Vector3 _position;
    public Quaternion _rotation;
}