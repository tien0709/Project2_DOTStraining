using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode.Samples
{
    public class Net : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsServer && IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
            {
                //client call
                TestServerRpc(0, NetworkObjectId);
            }
        }

        public struct PlayerData
        {
            private int playerId;
            private bool isReady;
        }

       

        [ClientRpc]
        void TestClientRpc(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
            {
                TestServerRpc(value + 1, sourceNetworkObjectId);
            }
        }

        [ServerRpc]
        void TestServerRpc(int value, ulong sourceNetworkObjectId)
        { 
            Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            TestClientRpc(value, sourceNetworkObjectId);
        }
    }
}
