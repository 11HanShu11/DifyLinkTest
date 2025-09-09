using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using TMPro;

public class GameOpening : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTrans;
    [SerializeField] private TMP_Text _countTxt;

    private Vector3 _initialPos;
    private Vector3 _initialRot;

    private readonly Subject<Unit> _openingEnd = new();
    public IObservable<Unit> OpeningEnd => _openingEnd;

    private readonly Subject<Unit> _tileChange = new();
    public IObservable<Unit> TileChange => _tileChange;

    private readonly Subject<Unit> _goSignal = new();
    public IObservable<Unit> GoSignal => _goSignal;

    private void Start()
    {
        // 開始時の状態を保存
        _initialPos = _cameraTrans.position;
        _initialRot = _cameraTrans.localEulerAngles;

        // 非同期演出開始
        _ = RunOpeningAsync();
    }

    private async UniTask RunOpeningAsync()
    {
        await PlayOpeningIntroAsync();
        PlayCountdownSequence();
    }

    /// <summary>
    /// 演出前半（カメラの移動・回転）
    /// </summary>
    private async UniTask PlayOpeningIntroAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        _tileChange.OnNext(Unit.Default);

        _cameraTrans.DOMoveX(3, 2.9f);
        _cameraTrans.DOBlendableRotateBy(new Vector3(0, 40, 0), 2.9f);

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        _countTxt.enabled = true;
    }

    /// <summary>
    /// カウントダウン演出
    /// </summary>
    private void PlayCountdownSequence()
    {
        Sequence sequence = DOTween.Sequence();

        AddCountdownStep(sequence, "3",
            startPos: new Vector3(5, 3, 3),
            startRot: new Vector3(0, 220, 0),
            moveTarget: new Vector3(3.8f, 3, 4.5f));

        AddCountdownStep(sequence, "2",
            startPos: new Vector3(-2, 2, -6),
            startRot: new Vector3(0, 40, 0),
            moveTarget: new Vector3(-5, 2, -3));

        AddCountdownStep(sequence, "1",
            startPos: new Vector3(0, 2, 6),
            startRot: new Vector3(0, 180, 0));

        // GO!!
        sequence.AppendCallback(() => SetText("GO!!"));
        sequence.AppendCallback(() => _goSignal.OnNext(Unit.Default));
        sequence.AppendCallback(() => _countTxt.transform.localScale = Vector3.zero);
        sequence.Append(_countTxt.transform.DOScale(1, 1).SetEase(Ease.OutElastic));
        sequence.Join(_cameraTrans.DOMove(new Vector3(0, 21, 23), 1));
        sequence.Join(_cameraTrans.DOLocalRotate(new Vector3(30, 180, 0), 1));

        sequence.OnComplete(() =>
        {
            _countTxt.enabled = false;
            _openingEnd.OnNext(Unit.Default);
        });

        sequence.Play();
    }

    /// <summary>
    /// カウントダウンの1ステップを追加
    /// </summary>
    private void AddCountdownStep(Sequence seq, string text, Vector3 startPos, Vector3 startRot, Vector3? moveTarget = null)
    {
        seq.AppendCallback(() =>
        {
            SetText(text);
            SetCameraStart(startPos, startRot);
        });

        seq.Append(_countTxt.transform.DOScale(1, 1).SetEase(Ease.OutElastic));

        if (moveTarget.HasValue)
        {
            seq.Join(_cameraTrans.DOMove(moveTarget.Value, 1));
        }
    }

    private void SetCameraStart(Vector3 pos, Vector3 rot)
    {
        _countTxt.transform.localScale = Vector3.zero;
        _cameraTrans.position = pos;
        _cameraTrans.localEulerAngles = rot;
    }

    private void SetText(string txt) => _countTxt.text = txt;

    private void OnDestroy()
    {
        // カメラを初期状態に戻す
        _cameraTrans.position = _initialPos;
        _cameraTrans.localEulerAngles = _initialRot;
    }
}