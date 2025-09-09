using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SugorokuAnime : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons;
    [SerializeField] private Animator _anime;
    [SerializeField] private Transform _playerPos;

    private int _previousSign = 0; // -1, 0, 1 を格納
    private string[] _animationNames = { "Run2_R", "Run2_L", "Parkour001_L", "Drift_L", "GoalPerformance_Kawaii_A", "Brake"};
    private string[] _selectedAnimationName = { "Run_R", "Run_L" };
    private const float FADE_TIME = 0.2f;
    private bool _isRunning = false;

    void Start()
    {
        SettingButtons();
        _anime.Play(_animationNames[5]);
    }
    
    /// <summary>
    /// 開始アニメーション
    /// 開始カウントダウン終了で実行
    /// </summary>
    /// <returns></returns>
    public async UniTask StartAnimation()
    {
        DefaultAnimation(_previousSign);// 開始時にループ再生
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        StartRandomMovement().Forget(); // 非同期で動作開始
    }

    private void SettingButtons()
    {
        int smaller = Mathf.Min(_buttons.Count, _animationNames.Length);

        for (int i = 0; i < smaller; i++)
        {
            int index = i; // クロージャ対策
            _buttons[i].onClick.AddListener(() => _ = PlayTemporaryAnimation(_animationNames[index]));
        }
    }

    private async UniTaskVoid PlayTemporaryAnimation(string clipName)
    {
        StopAllCoroutines(); // 複数押されたときの処理衝突防止
        await PlayAndReturnToDefault(clipName);
    }

    private async UniTask PlayAndReturnToDefault(string clipName)
    {
        _anime.CrossFade(clipName, FADE_TIME);

        // CrossFadeによって状態が切り替わるのを待つ（次のフレーム）
        await UniTask.Yield();

        // 状態が正しく更新されるまで少し待機
        await UniTask.WaitUntil(() => _anime.GetCurrentAnimatorStateInfo(0).IsName(clipName));

        AnimatorStateInfo state = _anime.GetCurrentAnimatorStateInfo(0);
        float length = state.length;

        // 念のため、length分+α待つ（途中で切れるのを防ぐ）
        await UniTask.Delay(TimeSpan.FromSeconds(length + 0.1f));

        // 元のループアニメーションに戻す
        DefaultAnimation(_previousSign);
    }



    #region ループアニメ処理
    private async UniTaskVoid StartRandomMovement()
    {
        _isRunning = true;

        while (_isRunning)
        {
            // ランダムな方向を取得
            int rndmMoveX = UnityEngine.Random.Range(-3, 3);
            int rndmMoveZ = UnityEngine.Random.Range(-2, 2);

            _playerPos.DOMoveX(rndmMoveX, 2);
            _playerPos.DOMoveZ(rndmMoveZ, 2);
            DefaultAnimation(rndmMoveX);

            // ランダムな待機時間
            float waitTime = UnityEngine.Random.Range(3, 5);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }
    }

    /// <summary>
    /// プレイヤーのアニメーター実行
    /// </summary>
    /// <param name="newSign"></param>
    private void DefaultAnimation(int newSign)
    {
        if (newSign <= 0)
        {
            _anime.CrossFade(_selectedAnimationName[0], FADE_TIME);
        }
        else
        {
            _anime.CrossFade(_selectedAnimationName[1], FADE_TIME);
        }
    }
    #endregion

}
