using System.Collections.Generic;
using UnityEngine;

public enum PlayerPartType
{
	TITAN = 0,
	BARBECUEOFFRIDAY,
	AMPERE,
	HOLEMAKER,
	FIRSTNOVA
}


public class PlayerPartController : MonoBehaviour
{
	[SerializeField] private PlayerPartDataListSO _playerPartListSO;
	[SerializeField] private Transform _partPoint;
    private static PlayerPart _currentPlayerPart;
	public static PlayerPart GetCurrentPlayerPart() => _currentPlayerPart;

	public PlayerPart Init(PlayerPartType playerPartType)
	{
		_currentPlayerPart = Instantiate(_playerPartListSO.GetData((int)playerPartType).partPrefab, _partPoint);
		_currentPlayerPart.transform.localPosition = Vector3.zero;
		_currentPlayerPart.transform.localScale = Vector3.one;

		return _currentPlayerPart;
	}
}
