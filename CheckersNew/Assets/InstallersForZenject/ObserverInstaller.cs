using Observer;
using UnityEngine;
using Zenject;

namespace InstallersForZenject
{
    public class ObserverInstaller : MonoInstaller
    {
        [SerializeField] public ObserverComponent _observer;

        public override void InstallBindings()
        {
            Container.Bind<ObserverComponent>().FromInstance(_observer).AsSingle();
        }
    }
}