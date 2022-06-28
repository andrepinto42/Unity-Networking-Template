using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using ParrelSync;

public class ManagerInterface : NetworkBehaviour
{
    public  Button buttonJoinLobby;
    public  Button buttonHostLobby;
    [SerializeField]private TMP_Text currentPlayersInGameText;
    public int playersInGame = 1;


    
    void Start()
    {
        buttonJoinLobby.onClick.AddListener( OnClickJoinLobby);

        
        buttonHostLobby.onClick.AddListener(onClickHostLobby);


        NetworkManager.Singleton.OnClientConnectedCallback += ((id) => 
        {
            if(IsServer)
                ChangePlayersInGameServerRpc(1);

            Debug.Log($"Just connected {id} !");
        });

        
        NetworkManager.Singleton.OnClientDisconnectCallback += ((id) => 
        {
            if(IsServer)
                ChangePlayersInGameServerRpc(-1);
            
            Debug.Log($"Just disconnected {id} !");
        });

        if (ClonesManager.IsClone())
        {
            OnClickJoinLobby();
        }
        else
        {
            //Only for Debug Purposes
            // onClickHostLobby();
        }
    }

    private void onClickHostLobby()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Sucess on Host");
        }
        else
        {
            Debug.Log("Client could not been started!");
        }
    }

    private void OnClickJoinLobby()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Sucess on Client");
        }
        else
        {
            Debug.Log("Host could not been started!");
        }
    }

    [ServerRpc]
    private void ChangePlayersInGameServerRpc(int number)
    {
        Debug.Log("Increasing number of Players");


        playersInGame += number;
        AdjustInterfaceClientRpc($"Number of Players {playersInGame}");
    }

    [ClientRpc]
    private void AdjustInterfaceClientRpc(string v)
    {
        currentPlayersInGameText.SetText(v);
    }
}
