using Crogen.CrogenPooling;
using System.Collections;
using UnityEngine;

public class EnergySphereLaser : MonoBehaviour, IPoolingObject
{
	[SerializeField] private int _damage = 1;

	[Header("Damage Caster")]
	[SerializeField] private Transform _damageCasterTrm;
	[SerializeField] private DamageCaster _damageCaster;

	[Space(25f)]
	[SerializeField] private LineRenderer _lineRenderer;
	private Transform _target;

	public string OriginPoolType { get; set; }
	GameObject IPoolingObject.gameObject { get; set; }
	[SerializeField] private HitEffectPoolType _hitEffectPoolType;

	public void Init(Transform target, int damage)
	{
		SetTarget(target);
		SetDamage(damage);
		StartCoroutine(CoroutineOnAttack());
	}

	private void SetTarget(Transform target)
	{
		this._target = target;
	}

	private void SetDamage(int damage)
	{
		_damage = damage;
	}

	public void OnPop()
	{
	}

	public void OnPush()
	{
		_target = null;
	}

	private IEnumerator CoroutineOnAttack()
	{
		yield return new WaitForSeconds(0.1f);
		if (_target != null)
		{
			Vector3 lineAttackPoint = _target.position - transform.position;
			lineAttackPoint.y = transform.position.y;
			Vector3 effectAttackPoint = _target.position;
			effectAttackPoint.y = transform.position.y;
			_lineRenderer.SetPosition(1, lineAttackPoint);
			_damageCasterTrm.position = _target.position;
			_damageCaster.CastDamage(_damage);
			this.Push();
			gameObject.Pop(_hitEffectPoolType, effectAttackPoint, Quaternion.identity);
		}
	}
}
