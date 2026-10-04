using System.Collections.Generic;

// This file is shared by the Unity client AND the game server.
// It lives in Assets so Unity compiles it, and Server/GameServerCore/GameServerCore.csproj links this exact file,
// so both sides always agree on what every packet ID means.
// Never add UnityEngine code here, the server can't see Unity and will fail to build.

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    welcome = 1,
    sendUpdatedPlayerPosition = 2,
    totalPlayers = 3,// Deprecated
    sendSelectionPacket = 4,
    sendUsername = 5,
    startTurn = 6,
    relayReadyUp = 7,
    syncTimers = 8,
    sendDamageToOpponent = 9,
    serverSendStatusEffectData = 10,
    serverSendCurrentStatusEffectDuration = 11,
    serverSendStoredMomentumValue = 12,
    serverSendOverrodePos = 13,
    serverSendWinStatus = 14,
    toggleCountdownTimer = 15,
    serverSendAnimationTrigger = 16,
    sendMovementPath = 17,       // Renamed from sendSegmentedMovementData; payload is now a full waypoint list
    // sendSegmentedRotationData = 18, -- RETIRED (facing derives from waypoints); number reserved
    sendNetworkedMethodIndex = 19,
    initalMatchDetails = 20,
    startMatch = 21,             // Sent to both players at once, only after both have said they're ready
}

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    updatePlayerCurrentPosition = 2,
    sendSelectionData = 3,
    endTurn = 4,
    sendReadyUp = 5,
    enterSyncTimerQueue = 6,
    requestToDamageOpponentsHealth = 7,
    clientSendStatusEffectData = 8,
    sendCurrentStatusEffectDuration = 9, // Deprecated
    sendStoredMomentumValue = 10,
    overrideOppositePlayersPos = 11,
    hasWonTheMatch = 12,
    toggleTimerCountdown = 13,
    clientSendAnimationTrigger = 14,
    sendMovementPath = 15,       // Renamed from sendSegmentedMovementData; payload is now a full waypoint list
    // sendSegmentedRotationData = 16, -- RETIRED (facing derives from waypoints); number reserved
    sendNetworkedMethodIndex = 17,
    readyToStartMatch = 18,      // The combat scene is loaded and the local character has spawned
}

// Protocol holds the rules both sides have to agree on, beyond just the packet IDs above.
// Keeping them here (instead of hand-copying them into the client and the server) means
// a change is made once and can't be forgotten on the other side.
public static class Protocol
{
    // Bump this whenever a packet's ID or contents change.
    // The client and server compare it when they connect and refuse to play together if it doesn't match,
    // so an out of date build fails loudly instead of quietly reading packets wrong.
    // 2: the movement path packet sends a movement state index instead of four pacing floats.
    // 3: added readyToStartMatch / startMatch so both players start the match at the same time.
    public const int Version = 3;

    // Packets the server doesn't need to understand, it just forwards them to the opponent untouched.
    // Left side is the ID the client sends, right side is the ID the opponent receives it as.
    // To network something new between players, add a pair here, the server needs no new code for it.
    public static readonly Dictionary<ClientPackets, ServerPackets> RelayedPackets = new Dictionary<ClientPackets, ServerPackets>()
    {
        { ClientPackets.updatePlayerCurrentPosition, ServerPackets.sendUpdatedPlayerPosition },
        { ClientPackets.sendSelectionData, ServerPackets.sendSelectionPacket },
        { ClientPackets.endTurn, ServerPackets.startTurn },
        { ClientPackets.sendReadyUp, ServerPackets.relayReadyUp },
        { ClientPackets.requestToDamageOpponentsHealth, ServerPackets.sendDamageToOpponent },
        { ClientPackets.clientSendStatusEffectData, ServerPackets.serverSendStatusEffectData },
        { ClientPackets.sendStoredMomentumValue, ServerPackets.serverSendStoredMomentumValue },
        { ClientPackets.overrideOppositePlayersPos, ServerPackets.serverSendOverrodePos },
        { ClientPackets.hasWonTheMatch, ServerPackets.serverSendWinStatus },
        { ClientPackets.clientSendAnimationTrigger, ServerPackets.serverSendAnimationTrigger },
        { ClientPackets.sendMovementPath, ServerPackets.sendMovementPath },
        { ClientPackets.sendNetworkedMethodIndex, ServerPackets.sendNetworkedMethodIndex },
    };
}
