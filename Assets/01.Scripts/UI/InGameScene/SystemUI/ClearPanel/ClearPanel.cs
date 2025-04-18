﻿using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _servivedText;
    [SerializeField] private TextMeshProUGUI _rankText;
    
    [SerializeField] private Button _gameEndButton;
    
    [SerializeField] private Transform _upgradesParent;
    [SerializeField] private BGMController _bgmController;
    [SerializeField] private FeedbackPlayer _feedbackPlayer;

    protected override void Awake()
    {
        base.Awake();
        _gameEndButton.onClick.AddListener(GameEnd);
    }

    private void GameEnd()
    {
        TimeManager.PlayTime();
        ResourceManager.SaveCoin();
        SceneLoadingManager.LoadScene("LobbyScene");
    }

    public void SetUI()
    {
        _levelText.text = $"Level {XPManager.GetLevel.ToString()}";
        _coinText.text = ResourceManager.GetCoin().ToString();
        _servivedText.text = TimeManager.CurrentGlobalTimerString;
    }

    public IEnumerator OpenUIBGM()
    {
        _bgmController.EndAudio();
        yield return new WaitForSeconds(3f);
        _feedbackPlayer.PlayFeedback();
        yield return new WaitForSecondsRealtime(13f);
        base.Open();
        SetUI();
        TimeManager.PauseTime();
    }

    public override void Open()
    {
        StartCoroutine(OpenUIBGM());
    }
    
    
}