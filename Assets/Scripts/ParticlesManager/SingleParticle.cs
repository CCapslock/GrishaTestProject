using System;
using UnityEngine;

[Serializable]
public class SingleParticle
{
    public ParticleType Type;
    public ParticleSystem ParticlePrefab;
    public int PoolSize;
}