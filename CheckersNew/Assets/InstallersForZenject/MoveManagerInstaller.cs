using Game;
using UnityEngine;
using Zenject;

namespace InstallersForZenject
{
    public class MoveManagerInstaller : MonoInstaller
    {
        [SerializeField] private MoveManager _moveManager;
        private MoveManagerObservable _moveManagerObservable;
        public override void InstallBindings()
        {
            _moveManagerObservable = new MoveManagerObservable(_moveManager);
            Container.Bind<MoveManager>().FromInstance(_moveManager).AsSingle();
            Container.BindInterfacesAndSelfTo<MoveManagerObservable>().FromInstance(_moveManagerObservable);
        }
    }
    
}