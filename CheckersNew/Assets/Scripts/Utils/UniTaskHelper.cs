using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
using UnityEngine;

namespace Utils
{
    public static class UniTaskHelper
    {
        public static async UniTask DoAfterSeconds(Action action, float seconds)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
            action?.Invoke();
        }

        public static async UniTask CameraRotate(float timeForMove)
        {
            GameCharacteristics.IsCameraMoving = true;

            var camera = Camera.main;
            if (camera != null)
            {
                var parent = camera.transform.parent;
                var rotation = parent.eulerAngles;
                var endRotation = rotation + new Vector3(0, 180, 0);
                    
                var tween = parent.DORotate(endRotation, timeForMove);
                await tween.AsyncWaitForCompletion();
            }

            GameCharacteristics.IsCameraMoving = false;
        }
    }
}