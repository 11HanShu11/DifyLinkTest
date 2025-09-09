using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;

public class PieceMoving : MonoBehaviour
{
    private const float JUMP_POWER = 0.6f; //ジャンプ力
    private const float JUMP_TIME = 0.2f; //移動時間

    [SerializeField] private Transform _pieceTrans;

    public List<GameObject> Tiles { private get; set; }
    public int CurrentTileIndex => _currentTileIndex;
    public ReactiveProperty<bool> IsPieceMove = new ReactiveProperty<bool>(false);
    public IObservable<(float, int)> RoadChange => _roadChange;
    public IObservable<Unit> GetTile => _getTile;

    private int _currentTileIndex = 0;
    private readonly Subject<(float, int)> _roadChange = new Subject<(float, int)>();
    private readonly Subject<Unit> _getTile = new Subject<Unit>();

    void Start()
    {
        _getTile.OnNext(Unit.Default);
        // TimeScaleController.Instance.SetTimeScale("Slow", 0.2f);
        // DOVirtual.DelayedCall(5f, () => TimeScaleController.Instance.ClearTimeScale("Slow"));  
    }

    /// <summary>
    /// 移動とアニメーション
    /// </summary>
    /// <param name="number">進行数</param>
    public void ProceedPiece(int number)
    {
        Sequence sequence = DOTween.Sequence();

        IsPieceMove.Value = true;
        for (int i = 0; i < number; i++)
        {
            _currentTileIndex++;

            Vector3 nextPos = NextPos(_currentTileIndex);

            sequence.Append(_pieceTrans.transform.DOJump(nextPos, JUMP_POWER, 1, JUMP_TIME));
            sequence.AppendCallback(() => _roadChange.OnNext((JUMP_TIME, _currentTileIndex)));
        }

        sequence.OnComplete(() =>
            {
                Debug.Log("アニメーションが終了しました");
                IsPieceMove.Value = false;
            });

        sequence.Play();
    }

    /// <summary>
    /// 進行位置の決定
    /// </summary>
    /// <param name="number">歩数合計</param>
    /// <returns = Vector3> 進行位置</returns>
    private Vector3 NextPos(int tileIndex)
    {
        _currentTileIndex = tileIndex % (Tiles.Count);

        var tilePos = Tiles[_currentTileIndex].transform.position;
        tilePos.y += 1;

        return tilePos;
    }

}
