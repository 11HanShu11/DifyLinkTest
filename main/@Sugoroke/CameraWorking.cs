using System.Collections.Generic;
using UnityEngine;
using System;
using Dreamteck.Splines;
using UnityEngine.Splines;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Sugoroku
{
    public class CameraWorking : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] Transform _mainCameraTrans;
        [SerializeField] GameObject CameraParent;

        private Vector3 _defaultPos;
        private SplineAnimate _splineAnime;
        private Transform _moveTrans;
        private CancellationTokenSource _standbyCts;

        void Start()
        {
            var obj = new GameObject();
            _moveTrans = obj.transform;
            _splineAnime = CameraParent.GetComponent<SplineAnimate>();

            _splineAnime.enabled = false;
        }

        /// <summary>
        /// スタート演出終了でスタンバイカメラワーク開始
        /// </summary>
        public void BeginningWork()
        {
            _mainCameraTrans.transform.parent = CameraParent.transform;
            _mainCameraTrans.localPosition = Vector3.zero;
            StartStandbyCameraLoop();
        }

        /// <summary>
        /// スタンバイ中のカメラモーション開始
        /// </summary>
        private void StartStandbyCameraLoop()
        {
            _splineAnime.enabled = true;
            _standbyCts?.Cancel();
            _standbyCts = new CancellationTokenSource();
            StandbyCameraLoopAsync(_standbyCts.Token).Forget();
        }

        /// <summary>
        /// スタンバイ中のカメラモーション停止
        /// </summary>
        public void StopStandbyCameraLoop()
        {
            _splineAnime.enabled = false;
            _standbyCts?.Cancel();
        }

        private async UniTaskVoid StandbyCameraLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_target != null)
                {
                    _mainCameraTrans.transform.LookAt(_target);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        /// <summary>
        /// サイコロを振った際に移動演出を開始する
        /// </summary>
        public void BeginDiceRollEvent(int pieceTileIndex)
        {
            _defaultPos = _mainCameraTrans.transform.position;
            StopStandbyCameraLoop();
            UpdateCameraTargetByTileIndex(pieceTileIndex);
            _mainCameraTrans.DOMove(_moveTrans.position, 1);
            _mainCameraTrans.DORotate(_moveTrans.eulerAngles, 1);
        }

        /// <summary>
        ///  ターンが終了した際にカメラを元の状態に戻す
        /// </summary>
        public void EndDiceRollEvent()
        {
            StartStandbyCameraLoop();
            _mainCameraTrans.DOMove(_defaultPos, 1);
        }

        /// <summary>
        /// 駒の位置に追従してカメラを移動する
        /// </summary>
        /// <param name="number"></param>
        public void MoveToPiecePosition(int number)
        {
            UpdateCameraTargetByTileIndex(number);
            _mainCameraTrans.DOMove(_moveTrans.position, .3f).SetEase(Ease.InQuad);
            _mainCameraTrans.DORotate(_moveTrans.eulerAngles, .3f).SetEase(Ease.InQuad);
        }

        /// <summary>
        /// 駒の位置からカメラの目標座標・角度を決定する
        /// </summary>
        /// <param name="number"></param>
        private void UpdateCameraTargetByTileIndex(int number)
        {
            Vector3 pos;
            int rotateY;

            const int rotateX = 35;

            if (number <= 9)
            {
                pos = new Vector3(12, 7, 0);
                rotateY = -90;
            }
            else if (number <= 18)
            {
                pos = new Vector3(0, 7, -12);
                rotateY = 0;
            }
            else if (number <= 27)
            {
                pos = new Vector3(-12, 7, 0);
                rotateY = 90;
            }
            else if (number <= 36)
            {
                pos = new Vector3(0, 7, 12);
                rotateY = 180;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(number), $"Invalid number: {number}");
            }

            _moveTrans.position = pos;
            _moveTrans.eulerAngles = new Vector3(rotateX, rotateY, 0);
        }
    }
}