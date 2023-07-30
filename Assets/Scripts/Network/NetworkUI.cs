using Unity.Netcode;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class NetworkUI : NetworkBehaviour
    {
        [SerializeField] private Button HostButton;
        [SerializeField] private Button ClientButton;
        [SerializeField] private TextMeshProUGUI PlayersCountText;
        [SerializeField] private GameObject chatUI;
        [SerializeField] private TMP_Text chatText;
        [SerializeField] private TMP_InputField inputChat;
        [SerializeField] private TMP_InputField inputName;

        private NetworkVariable<int> playerNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

        private static event Action<string> OnMessage;

        private void Awake()
        {
            HostButton.onClick.AddListener(() => {
                NetworkManager.Singleton.StartHost();
            });

            ClientButton.onClick.AddListener(() => {
                NetworkManager.Singleton.StartClient();
            });

            OnMessage += HandleNewMessage;
        }

        private void Update()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                playerNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
                PlayersCountText.text = "ServerMode, NumPlayer: " + playerNum.Value.ToString();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                PlayersCountText.text = "ClientMode";
            }
        }

        public override void OnDestroy()
        {
            if (!IsOwner) { return; }

            OnMessage -= HandleNewMessage;
        }

        private void HandleNewMessage(string message)
        {
            chatText.text += message;
        }

        [ClientRpc]
        private void ReceiveMessageFromServerClientRpc(string message)
        {
            HandleMessage(message);
        }

        public void SendClientMessage()
        {
            string message = inputChat.text.Trim();
            string name = inputName.text.Trim();
            if (string.IsNullOrWhiteSpace(message)||string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            // Send the message to the server
            SendServerMessageToAllClientsServerRpc(message, name);

            inputChat.text = string.Empty;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendServerMessageToAllClientsServerRpc(string message, string name)
        {
            // Process the message on the server and relay it to all clients
            //HandleMessage($"[]: {message}");
            ReceiveMessageFromServerClientRpc($"[{name}]: {message}");
        }

        private void HandleMessage(string message)
        {
            OnMessage?.Invoke($"\n{message}");
        }
    }
}
