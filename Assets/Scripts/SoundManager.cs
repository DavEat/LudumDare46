using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource m_sfx = null;
    [SerializeField] AudioSource m_sfx_walk = null;
    [SerializeField] AudioSource m_sfx_revers = null;
    [SerializeField] AudioSource m_sfx_jumps = null;
    [SerializeField] AudioSource m_sfx_rocks = null;

    [SerializeField] AudioClip[] m_walks = null;
    [SerializeField] AudioClip[] m_jumps = null;
    [SerializeField] AudioClip[] m_rocks = null;
    [SerializeField] AudioClip m_water = null;

    float m_rockVol = 1;

    void Start()
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_rocks == null) return;
        #endif

        m_rockVol = m_sfx_rocks.volume;
    }

    public void PlayFoot(float pitch)
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_walk == null) return;
        #endif

        m_sfx_walk.pitch = pitch;
        if (!m_sfx_walk.isPlaying)
            m_sfx_walk.PlayOneShot(m_walks[Random.Range(0, m_walks.Length)]);
    }
    public void PlayJump()
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_jumps == null) return;
        #endif

        m_sfx_jumps.PlayOneShot(m_jumps[Random.Range(0, m_jumps.Length)]);
    }
    public void PlayRocks(float volMul)
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_rocks == null) return;
        #endif

        m_sfx_rocks.volume = m_rockVol * volMul;
        m_sfx_rocks.PlayOneShot(m_rocks[Random.Range(0, m_rocks.Length)]);
    }
    public void PlayWater()
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx == null) return;
        #endif

        m_sfx.PlayOneShot(m_water);
    }
    public void PlayRevers()
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_revers == null) return;
        #endif

        m_sfx_revers.Play();
    }
    public bool ReversIsPlaying()
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_revers == null) return false;
        #endif

        return m_sfx_revers.isPlaying;
    }
    public void StopRevers()
    {
        #if UNITY_EDITOR
        if (inst == null || m_sfx_revers == null) return;
        #endif

        m_sfx_revers.Stop();
    }
}