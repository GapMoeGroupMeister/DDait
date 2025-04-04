using Crogen.PowerfulInput;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }

	[SerializeField] private CutSceneManager _cutSceneManager;

	private void Start()
	{
		_cutSceneManager?.Init(Player);
		GameStart();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(2))
		{
			ZoomController.Instance.ForceZoomOut(60, 0.5f, 3);
		}

		if(Input.GetKey(KeyCode.I))
		{
			Vector3 ranPos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
			XPManager.CreateXP(ranPos, XPType.Red);
		}

		if (Input.GetKey(KeyCode.C))
		{
			Vector3 ranPos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
			ResourceManager.CreateCoin(ranPos);
		}
	}

	private void GameStart()
	{
		_cutSceneManager?.PlayGameStartCutScene();
		
	}

	public void GameExit()
	{
		SceneLoadingManager.LoadScene("LobbyScene");
	}
}
