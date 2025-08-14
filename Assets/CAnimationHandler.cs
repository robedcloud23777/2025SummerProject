using UnityEngine;

public class CAnimationHandler : MonoBehaviour
{
    public NetworkManager networkManager;
    public void OnPlay()
    {
        networkManager.Play();
    }
}