using System;
using Cysharp.Threading.Tasks;
using Game;
using Utils.Coordinates;

namespace Observer
{
    public class ImitationController
    {
        private ObserverComponent _observer;
        private MoveManagerObservable _observable;

        public ImitationController(ObserverComponent component, MoveManagerObservable observable)
        {
            _observer = component;
            _observable = observable;
        }

        public async UniTask Logic(string what, string action, PairOfCoordinates coordinates)
        {
            switch (what)
            {
                case "cell":
                {
                    if (action == "click")
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(_observer.CellClickDelay));
                        _observable?.ImitateCellClick(coordinates);
                    }

                    break;
                }
                case "chip":
                {
                    if (action == "click")
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(_observer.ChipClickDelay));
                        _observable?.ImitateChipClick(coordinates);
                    }

                    if (action == "ate")
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(_observer.ChipRemoveDelay));
                        _observable?.ImitateChipRemove(coordinates);
                    }

                    break;
                }
            }
        }
    
}
}
