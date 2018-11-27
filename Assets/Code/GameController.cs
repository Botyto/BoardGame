using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [Header("Players")]
    public GameObject playerPrefab = null;
    public int playersCount = 2;
    public Player[] players = null;

    [Header("Turns")]
    public int currentPlayerIndex = -1;
    public int turnDirection = 1;
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

        currentPlayerIndex = 0;
        BeginTurn();
    }

    private void BeginTurn()
    {
        while (true)
        {
            if (currentPlayer.numberOfTurns > 0)
            {
                --currentPlayer.numberOfTurns;
                break;
            }
            else
            {
                //This increment guarantees that at some point one of the players will have a positive number of turns.
                ++currentPlayer.numberOfTurns;
                currentPlayerIndex += turnDirection;

                while (currentPlayerIndex < 0)
                {
                    currentPlayerIndex += players.Length;
                }
                currentPlayerIndex %= players.Length;
            }
        }

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
        FollowCamera.Push(currentPlayer.gameObject.transform);
        yield return currentPlayer.BeginTurn();
        FollowCamera.Pop();
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
}
