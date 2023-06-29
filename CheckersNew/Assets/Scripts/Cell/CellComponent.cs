using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Cell
{

    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;
        private NeighboursController _neighboursController;
        private bool _isObserverNotReplaying => !(_observer != null && _observer.IsReplaying);
        public NeighboursController NeighboursController => _neighboursController;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_isObserverNotReplaying)
            {
                OnFocusEventInvoke(this, true);
            }
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_isObserverNotReplaying)
            {
                OnFocusEventInvoke(this, false);
            }
        }
        public void Start()
        {
            _neighboursController = new NeighboursController(this);
        }
    }
}