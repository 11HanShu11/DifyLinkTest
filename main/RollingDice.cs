using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

namespace Sugoroku
{
    public class RollingDice : MonoBehaviour
    {
        private const float ROLL_TIME = 1f; //rサイコロの回転を見せる時間
        private const float RESULT_TIME = 1f; //rサイコロの結果を見せる時間


        [SerializeField] private List<Sprite> _diceSprites; // 0〜5: サイコロ目1〜6
        [SerializeField] private Image _diceDisplay;
        [SerializeField] private Button _diceBtn;



        [SerializeField] private Button _testBtn;


        private readonly Subject<int> _diceResult = new Subject<int>();
        public IObservable<int> DiceResult => _diceResult;

        private readonly Subject<Unit> _startDiceRoll = new Subject<Unit>();
        public IObservable<Unit> StartDiceRoll => _startDiceRoll;

        private void Start()
        {
            _diceDisplay.enabled = false;
            _diceBtn.onClick.AddListener(() => _ = StartDiceRollAsync());
        }

        /// <summary>
        /// サイコロのロール全体処理
        /// </summary>
        private async UniTaskVoid StartDiceRollAsync()
        {
            _diceDisplay.enabled = true;
            _startDiceRoll.OnNext(Unit.Default);

            int result = await AnimateDiceRollingAsync(TimeSpan.FromSeconds(ROLL_TIME));
            await ShowResultAsync(result, TimeSpan.FromSeconds(RESULT_TIME));

            _diceDisplay.enabled = false;
            _diceResult.OnNext(result);
        }

        /// <summary>
        /// 指定時間、ランダムにサイコロの目を変化させる
        /// </summary>
        /// <param name="duration">アニメーション時間</param>
        /// <returns　int = currentResult> サイコロの目の結果</returns>
        private async UniTask<int> AnimateDiceRollingAsync(TimeSpan duration)
        {
            float endTime = Time.time + (float)duration.TotalSeconds;
            int currentResult = 1;

            while (Time.time < endTime)
            {
                currentResult = UnityEngine.Random.Range(1, 7);
                SetDiceFace(currentResult);
                await UniTask.Delay(TimeSpan.FromSeconds(0.1));
            }

            return currentResult;
        }

        /// <summary>
        /// 指定された目を表示したまま一定時間表示する
        /// </summary>
        /// <param name="diceNumber">出目</param>
        /// <param name="duration">公開時間</param>
        private async UniTask ShowResultAsync(int diceNumber, TimeSpan duration)
        {
            SetDiceFace(diceNumber);
            await UniTask.Delay(duration);
        }

        /// <summary>
        ///  サイコロのスプライトを変更する（1〜6を0〜5に変換）
        /// </summary>
        /// <param name="diceNumber">出目</param>
        private void SetDiceFace(int diceNumber)
        {
            int index = Mathf.Clamp(diceNumber - 1, 0, _diceSprites.Count - 1);
            _diceDisplay.sprite = _diceSprites[index];
        }
    }
}
