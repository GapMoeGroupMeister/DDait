using System.Collections;
using Crogen.CrogenPooling;
using EffectSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class OilObject : MonoBehaviour, IPoolingObject, IEffectable
{
    public string OriginPoolType { get; set; }
    GameObject IPoolingObject.gameObject { get; set; }

    private DecalProjector _decalCompo;
    private Material _decalMaterial;
    [SerializeField] private float _detectRange = 4f;
    private int setOilAmount = 0;
    public int leftOilAmount = 0;
    [SerializeField] private LayerMask _targetLayer;
    private bool _isFire;
    private Collider[] hits;
    private ParticleSystem _fireVFX;
    private Collider _collider;
    private int _fireLevel;
    private int _dissolveHash;
    private int _randomSeedHash;

    [SerializeField] private float _burnAroundDuration = 0.5f;
    private float _currentTime = 0;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        hits = new Collider[6];
        _dissolveHash = Shader.PropertyToID("_DissolveHeight");
        _randomSeedHash = Shader.PropertyToID("_RandomSeed");
        _decalCompo = GetComponentInChildren<DecalProjector>();
        _decalMaterial = Instantiate(_decalCompo.material); // sharedMaterial이 아닌데 모든 객체의 메테리얼에 참조된 속성의 값이 변경됨.
        _decalCompo.material = _decalMaterial;
        _fireVFX = transform.Find("FireVFX").GetComponent<ParticleSystem>();

    }
    //
    // private void Start()
    // {
    //     _decalMaterial = _decalCompo.material;
    //     Debug.Log(_decalMaterial);
    // }

    public void SetOil(int amount)
    {
        _decalCompo.material.SetFloat(_dissolveHash, 0.7f);
        leftOilAmount = amount;
        setOilAmount = amount;
        _isFire = false;
    }

    private void FixedUpdate()
    {
        if (_isFire)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _burnAroundDuration)
            {
                _currentTime = 0;
                BurnAround();
                
            }

        }
        
    }


    private void BurnAround()
    {
        int amount =  Physics.OverlapSphereNonAlloc(transform.position, _detectRange, hits, _targetLayer);
        if (amount == 0) return;
        leftOilAmount--;
        _decalCompo.material.SetFloat(_dissolveHash, Mathf.Lerp(0f, 0.7f, leftOilAmount/(float)setOilAmount));
        for (int i = 0; i < amount; i++)
        {
            if (hits[i].transform.TryGetComponent(out IEffectable effectTarget))
            {
                effectTarget.ApplyEffect(EffectStateTypeEnum.Burn, 10f,_fireLevel);
            }
        }

        if (leftOilAmount <= 0)
        {
            _isFire = false;
            StartCoroutine(SetOffFireCoroutine());
        }
    }


    private IEnumerator SetOffFireCoroutine()
    {
        _fireVFX.Stop();
        yield return new WaitForSeconds(2f);
        this.Push();
    }

    public void OnPop()
    {
        _isFire = false;
        _decalCompo.material.SetFloat(_randomSeedHash, Random.Range(-10f, 10f));
    }

    public void OnPush()
    {
        
        
        
    }
    
    public void ApplyEffect(EffectStateTypeEnum type, float duration, int level, float percent = 1f)
    {
        _fireLevel = level;
        if(_isFire || leftOilAmount <= 0) return;
        if (type == EffectStateTypeEnum.Burn)
        {
            _isFire = true;
            _fireVFX.Play();
        }
    }

   
}
