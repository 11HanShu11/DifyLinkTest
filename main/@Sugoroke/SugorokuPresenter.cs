using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

namespace Sugoroku
{
  public class SugorokuPresenter : MonoBehaviour
  {
    [SerializeField] private SugorokuModel _sugorokuModel;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private SugorokuAnime _sugorokuAnime;
    [SerializeField] private RollingDice _rollingDice;
    [SerializeField] private PieceMoving _pieceMoving;
    [SerializeField] private CameraWorking _cameraWorking;
    [SerializeField] private GameOpening _gameOpening;

    void Start()
    {
      //スタート演出でのタイル更新
      _gameOpening.TileChange.Subscribe(_ =>
      {
        Debug.Log("enter1");
        _tileManager.UpdateTiles();
      }).AddTo(this);

      //発進合図
      _gameOpening.GoSignal.Subscribe(async _ =>
      {
        Debug.Log("enter2");
        _tileManager.RoadMoving(true);
        await _sugorokuAnime.StartAnimation();
      }).AddTo(this);

      //ゲームのスタート演出終了合図
      _gameOpening.OpeningEnd.Subscribe(_ =>
      {
        Debug.Log("enter3");
        // _cameraWorking.StartStandbyCameraLoop();
        _cameraWorking.BeginningWork();
      }).AddTo(this);


      //サイコロを振り始める
      _rollingDice.StartDiceRoll.Subscribe(_ =>
      {
        Debug.Log("enter4");
        _sugorokuModel.PieceTileIndex = _pieceMoving.CurrentTileIndex;
        _cameraWorking.BeginDiceRollEvent(_sugorokuModel.PieceTileIndex);
      }).AddTo(this);

      //Pieceの動き終了
      _pieceMoving.IsPieceMove.Skip(1).Subscribe(value =>
      {
        Debug.Log("enter5");
        if (!value)
        {
          _cameraWorking.EndDiceRollEvent();
        }
      }).AddTo(this);

      //サイコロの目を取得
      _rollingDice.DiceResult.Subscribe(number =>
      {
        Debug.Log("enter6");
        _sugorokuModel.DiceValue = number;
        _sugorokuModel.DiceTotal += _sugorokuModel.DiceValue;

        _pieceMoving.ProceedPiece(_sugorokuModel.DiceValue);
      }).AddTo(this);

      //コマ位置での、中心床のマテリアル変化合図
      _pieceMoving.RoadChange.Subscribe(value =>
      {
        Debug.Log("enter7");
        _tileManager.ChangeRoadVisual(value.Item1, value.Item2);
        _cameraWorking.MoveToPiecePosition(value.Item2);
      }).AddTo(this);

      //次に進むTileを取得
      _pieceMoving.GetTile.Subscribe(_ =>
      {
        Debug.Log("enter8");
        _pieceMoving.Tiles = _tileManager.Tiles;
      }).AddTo(this);

    }
  }

}