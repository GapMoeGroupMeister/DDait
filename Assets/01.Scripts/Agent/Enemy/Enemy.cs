using System.Linq;
using UnityEngine;
using Crogen.CrogenPooling;
using Unity.Behavior;
using System.Collections;

public abstract class Enemy : Agent, IPoolingObject
{
    protected BehaviorGraphAgent _btAgent;

    [field: SerializeField]
    public Renderer RendererCompo { get; private set; }
    public EnemyMovement EnemyMovementCompo { get; private set; }
    public EnemyAnimatorTrigger AnimatorTriggerCompo { get; private set; }

    [Header("Common Setting")]
    public LayerMask whatIsPlayer;
    public LayerMask whatIsObstacle;

    [Header("Attack Setting")]
    public float attackDistance;
    public Transform targetTrm;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private DamageCaster[] _damageCasters;
    [HideInInspector]
    public Collider colliderCompo;

    public float StunTime { get; protected set; }
    public float Level { get; protected set; }

    protected Collider[] _enemyCheckColliders;

    public string OriginPoolType { get; set; }
    GameObject IPoolingObject.gameObject { get; set; }

    private readonly int _burnedID = Shader.PropertyToID("_Burned");

    private Coroutine _stunCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _btAgent = GetComponent<BehaviorGraphAgent>();
        RestartBehaviorTree();
        targetTrm = GameManager.Instance.Player.transform;
        HealthCompo.OnDieEvent.AddListener(OnDie);
        colliderCompo = GetComponent<Collider>();
        EnemyMovementCompo = MovementCompo as EnemyMovement;
        EnemyMovementCompo.Initialize(this);
        Transform visualTrm = transform.Find("Visual");
        AnimatorTriggerCompo = visualTrm.GetComponent<EnemyAnimatorTrigger>();
    }

    public void CastDamage()
    {
        for (int i = 0; i < _damageCasters.Length; ++i)
        {
            _damageCasters[i]?.CastDamage((int)Stat.GetValue(StatEnum.Attack));
        }
    }

    public virtual void OnDie()
    {
        colliderCompo.enabled = false;
        WaveManager.Instance.RemoveEnemy(this);
    }

    public void Stun(float duration)
    {
        if (_stunCoroutine != null)
            StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        EnemyMovementCompo.DisableNavAgent();
        yield return new WaitForSeconds(duration);
        EnemyMovementCompo.EnableNavAgent();
    }

    public virtual void OnPop()
    {
        colliderCompo.enabled = true;
        Level = WaveManager.Instance.CurrentWave / WaveManager.Instance.stageWaves.wavelist.Length;
        EnemyStatDataSO stat = Stat as EnemyStatDataSO;
        foreach (var key in stat.statModifierDictionary.Keys)
        {
            float currentStat = stat.GetValue(key);
            float statModifier = stat.GetModifierValue(key, Level);
            stat.SetValue(key, currentStat * statModifier);
        }
        HealthCompo.Initialize(this, (int)stat.GetValue(StatEnum.MaxHP));
        HealthCompo.TakeDamage(0);
        RendererCompo.materials[0].SetFloat(_burnedID, 0);
        EnemyMovementCompo.EnableNavAgent();
        RestartBehaviorTree();
        CanStateChangeable = true;
    }

    public virtual void OnPush()
    {
        XP xp = gameObject.Pop(OtherPoolType.XP, transform.position, Quaternion.identity) as XP;
        xp.SetGrade((XPType)(int)(Level * 3));

        int rand = Random.Range(0, 100);
        if (rand < 20)
        {
            gameObject.Pop(OtherPoolType.Coin, transform.position, Quaternion.identity);
        }
    }

    public BlackboardVariable<T> GetVariable<T>(string variableName)
    {
        if (_btAgent.GetVariable(variableName, out BlackboardVariable<T> variable))
        {
            return variable;
        }
        return null;
    }

    public void RestartBehaviorTree()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        _btAgent.End();
        yield return null;
        _btAgent.Graph.Restart();

    }
}
