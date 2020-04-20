using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource m_sfx = null;
    [SerializeField] AudioSource m_sfx_walk = null;
    [SerializeField] AudioSource m_sfx_revers = null;
    [SerializeField] AudioSource m_sfx_jumps = null;

    [SerializeField] AudioClip[] m_walks = null;
    [SerializeField] AudioClip[] m_jumps = null;
    [SerializeField] AudioClip[] m_rocks = null;
    [SerializeField] AudioClip m_water = null;

    public void PlayFoot()
    {
        if (!m_sfx_walk.isPlaying)
            m_sfx_walk.PlayOneShot(m_walks[Random.Range(0, m_walks.Length)]);
    }
    public void PlayJump()
    {
        m_sfx_jumps.PlayOneShot(m_jumps[Random.Range(0, m_jumps.Length)]);
    }
    public void PlayRocks()
    {
        m_sfx.PlayOneShot(m_rocks[Random.Range(0, m_rocks.Length)]);
    }
    public void PlayWater()
    {
        m_sfx.PlayOneShot(m_water);
    }
    public void PlayRevers()
    {
        m_sfx_revers.Play();
    }
    public bool ReversIsPlaying()
    {
        return m_sfx_revers.isPlaying;
    }
    public void StopRevers()
    {
        m_sfx_revers.Stop();
    }
}