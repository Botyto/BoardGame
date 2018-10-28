using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public new FollowCamera camera = null;

    [Header("Players")]
    public GameObject playerPrefab = null;
    public int playersCount = 2;
    public Player[] players = null;

    [Header("Turns")]
    public int currentPlayerIndex = 0;
    public Player currentPlayer { get { return players[currentPlayerIndex % players.Length]; } }

    [HideInInspector]
    public Dictionary<string, Deck> decks;
    
    private void OnEnable()
    {
        //Gather all decks into a dictionary
        decks = new Dictionary<string, Deck>();
        foreach (var deck in FindObjectsOfType<Deck>())
        {
            if (!decks.ContainsKey(deck.id))
            {
                decks.Add(deck.id, deck);
            }
        }

        SpawnPlayers();
    }

    private UnityCoroutine m_GameRoutine;

    private void Start()
    {
        for (int i = 0; i < players.Length; ++i)
        {
            if (i == currentPlayerIndex) { continue; }
            players[i].StartCoroutine(players[i].Park());
        }

        --currentPlayerIndex;
        BeginTurn();
    }

    private void BeginTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        m_GameRoutine = new UnityCoroutine(TurnRountine());
        StartCoroutine(m_GameRoutine.StartSafe());
    }

    private void Update()
    {
        switch (m_GameRoutine.state)
        {
            case UnityCoroutine.CoroutineState.Finished:
                BeginTurn();
                break;

            case UnityCoroutine.CoroutineState.Failed:
                Debug.Log("<color=red>[Error]</color> Falied running turn routine!");
                BeginTurn();
                break;
        }
    }

    private IEnumerator TurnRountine()
    {
        camera.PushTarget(currentPlayer.gameObject.transform);
        yield return currentPlayer.BeginTurn();
        camera.PopTarget();
    }
    
    public int RollDice(int n = 1)
    {
        //TODO - make more elegant solution later?
        var cheat = FindObjectOfType<DiceCheat>();
        if (cheat != null)
        {
            return cheat.RollDice(n);
        }
        
        int total = 0;
        for (int i = 0; i < n; ++i)
        {
            total += Random.Range(1, 6);
        }

        return total;
    }

    private void SpawnPlayers()
    {
        if (playerPrefab == null)
        {
            Debug.Log("<color=red>[Error]</color> Missing player prefab");
            return;
        }

        var firstWaypoint = Board.instance.GetWaypoint(0);
        var spawnPosition = firstWaypoint.transform.TransformPoint(Vector3.forward * 20);
        var spacing = Vector3.right * 2.0f;

        playersCount = Mathf.Max(playersCount, 1);
        players = new Player[playersCount];
        for (int i = 0; i < playersCount; ++i)
        {
            var newPlayerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity) as GameObject;
            newPlayerObj.name = string.Format("Player {0}", i);
            var newPlayer = newPlayerObj.GetComponent<Player>();
            newPlayer.playerNumber = i;
            newPlayer.color = new Color(Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f), Random.Range(0.5f, 1.8f));

            players[i] = newPlayer;
            spawnPosition += spacing;
        }
    }

    #region Singleton

    public static GameController instance { get; private set; }

    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    void OnDestroy()
    {
        Debug.Assert(instance == this);
        instance = null;
    }

    #endregion
}
