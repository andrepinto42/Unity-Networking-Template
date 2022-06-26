using Unity.Netcode;
using UnityEngine;

public class PlayerNetHandler : NetworkBehaviour
{
    public NetworkVariable<Vector3> _playerPos = new NetworkVariable<Vector3>();

    void Awake() => _playerPos.OnValueChanged += UpdatePosition;
    public override void OnDestroy() => _playerPos.OnValueChanged -= UpdatePosition;

    private void UpdatePosition(Vector3 previousValue, Vector3 newValue)
    {
        transform.position = newValue;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            GetPositionToSpawnServerRpc();
        } 

    }
    [ServerRpc]
    private void GetPositionToSpawnServerRpc()
    {
        _playerPos.Value = NetworkBeginHandler.Singleton.GetPositionToSpawn();
    }
}
