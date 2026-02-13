using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    public int _savedScore = 0;
    public List<int> _ballsCollected = new List<int>();
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

        Load();
    }

    public void Save(Vector3 position, Quaternion rotation, int score)
    {
        SaveInformation saveInformation = new SaveInformation()
        {
            _position = position,
            _rotation = rotation,
            _scoreCount = score,
            _collectedBalls = _ballsCollected
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

            Rigidbody player_rb = _player.GetComponent<Rigidbody>();

            Debug.Log(loadedInformation._position);

            player_rb.position = new Vector3(loadedInformation._position.x, loadedInformation._position.y, loadedInformation._position.z);
            player_rb.rotation = loadedInformation._rotation;
            _savedScore = loadedInformation._scoreCount;
            _ballsCollected = loadedInformation._collectedBalls;
            Debug.Log($"LOAD FILE AFTER CHANGING POSITION: {_player.transform.position}");
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
    public int _scoreCount;
    public List<int> _collectedBalls = new List<int>();
}