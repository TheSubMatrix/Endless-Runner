using System.Collections.Generic;
using AudioSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundProxy : MonoBehaviour
{
    [FormerlySerializedAs("sounds")] [SerializeField]List<SoundData>  m_sounds = new List<SoundData>();

    public void PlaySound(int index)
    {
        SoundManager.Instance?.CreateSound().WithSoundData(m_sounds[index]).WithPosition(transform.position).WithRandomPitch().Play();
    }
}
