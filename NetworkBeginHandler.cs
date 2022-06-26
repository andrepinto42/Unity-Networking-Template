using Unity.Netcode;
using UnityEngine;

public class NetworkBeginHandler : NetworkBehaviour
{
    public static NetworkBeginHandler Singleton;
    public Vector3[] positionsToSpawn;
    int currentPosition = 0;
    void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
        {
            Destroy(this);
        }
    }

    public Vector3 GetPositionToSpawn()
    {
        return positionsToSpawn[currentPosition++ % positionsToSpawn.Length];
    }

}
