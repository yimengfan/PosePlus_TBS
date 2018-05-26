using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    static public AudioMgr Inst
    {
        get;
        private set;
    }
    
    public List<AudioClip> AudioClips;

    private List<AudioSource> AudioSources;
    private void Awake()
    {
        Inst = this;
        AudioSources = this.GetComponents<AudioSource>().ToList();
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBGM(string name , bool loop =true)
    {
        var clip = AudioClips.Find((a) => a.name == name);
        var ac =  this.AudioSources[0];
        ac.clip = clip;
        ac.loop = loop;
        ac.Play();
    }

    /// <summary>
    /// 播放一声
    /// </summary>
    /// <param name="name"></param>
    public void PlayOneShot(string name)
    {
        var clip = AudioClips.Find((a) => a.name == name);
        var ac =  this.AudioSources[0];
        ac.PlayOneShot(clip);
    }
}
