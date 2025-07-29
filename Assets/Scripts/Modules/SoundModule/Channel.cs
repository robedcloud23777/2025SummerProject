using UnityEngine;

public class Channel : MonoBehaviour
{
    private void OnDisable() => SoundManager.Instance.UnableChannel(GetComponent<AudioSource>());



}
