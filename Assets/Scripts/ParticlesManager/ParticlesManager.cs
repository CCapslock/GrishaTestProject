using UnityEngine;

public class ParticlesManager : MonoBehaviour
{

    [SerializeField] private SingleParticle[] _availableParticles;
    [SerializeField] private ParticleSystem[][] _particlesPool;

    private Transform _parentTransform;

    private Vector3 _poolPositionVector = new Vector3(0, -50f, -20f);
    private Vector3 _poolRotationVector = new Vector3(0, 0, 0);
    private void Awake()
    {
        _parentTransform = new GameObject("ParticlesPool").transform;
        InitiateParticlePool();
    }
    private void InitiateParticlePool()
    {
        _particlesPool = new ParticleSystem[_availableParticles.Length][];

        for (int i = 0; i < _availableParticles.Length; i++)
        {
            _particlesPool[i] = new ParticleSystem[_availableParticles[i].PoolSize];

            for (int j = 0; j < _particlesPool[i].Length; j++)
            {
                _particlesPool[i][j] = Instantiate(_availableParticles[i].ParticlePrefab, _poolPositionVector, Quaternion.Euler(_poolRotationVector), _parentTransform);
                _particlesPool[i][j].Stop();
            }
        }
    }
    public void MakeParticles(ParticleType type, Vector3 position)
    {
        Debug.Log("position = " + position);
        for (int i = 0; i < _particlesPool[(int)type].Length; i++)
        {
            if (!_particlesPool[(int)type][i].isPlaying || i == _particlesPool[(int)type].Length - 1)
            {
                _particlesPool[(int)type][i].transform.position = position;
                _particlesPool[(int)type][i].Play();
                break;
            }
        }
    }
    public void MakeParticles(ParticleType type, Transform parentTransform)
    {
        for (int i = 0; i < _particlesPool[(int)type].Length; i++)
        {
            if (!_particlesPool[(int)type][i].isPlaying || i == _particlesPool[(int)type].Length - 1)
            {
                _particlesPool[(int)type][i].transform.parent = parentTransform;
                _particlesPool[(int)type][i].transform.localPosition = Vector3.zero;
                _particlesPool[(int)type][i].Play();
                break;
            }
        }
    }
}