using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner networkRunner = null;
    [SerializeField]
    private NetworkPrefabRef playerPrefab;
    [SerializeField]
    private GameMode gameMode;
    [SerializeField]
    private NetworkPrefabRef soccer;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();
    public PlayerSubscriber playerSubscriber;
    private void Start()
    {
        StartGame(gameMode);
    }

    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition = new Vector3(0f, 1f, 32f);
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        playerList.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.x = playerSubscriber.x;
        data.y = playerSubscriber.y;
        data.z = playerSubscriber.z;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
}

internal struct NewStruct
{
    public float Item1;
    public float Item2;
    public float Item3;

    public NewStruct(float item1, float item2, float item3)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }

    public override bool Equals(object obj)
    {
        return obj is NewStruct other &&
               Item1 == other.Item1 &&
               Item2 == other.Item2 &&
               Item3 == other.Item3;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Item1, Item2, Item3);
    }

    public void Deconstruct(out float item1, out float item2, out float item3)
    {
        item1 = Item1;
        item2 = Item2;
        item3 = Item3;
    }

    public static implicit operator (float, float, float)(NewStruct value)
    {
        return (value.Item1, value.Item2, value.Item3);
    }

    public static implicit operator NewStruct((float, float, float) value)
    {
        return new NewStruct(value.Item1, value.Item2, value.Item3);
    }
}