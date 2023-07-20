using System;
using Cell;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.Coordinates;
using Zenject;

public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField]
    protected ColorType _color;

    [SerializeField] private MeshRenderer _meshRenderer;
    [Inject]
    protected MoveManager _moveManager;
    protected Observer.ObserverComponent _observer;
    public ColorType Color => _color;
    private BaseClickComponent _pair;

    public BaseClickComponent Pair
    {
        get => _pair;
        set
        {
            _pair = value;
            if (value == null) return;
            
            value._pair = this;
            
            CoordinatesOnField = value.CoordinatesOnField;
        }
    }

    public event ClickEventHandler OnClickEventHandler;

    public event FocusEventHandler OnFocusEventHandler;
    [NonSerialized] public Material PreviousMaterial;
    public PairOfCoordinates CoordinatesOnField;

    public abstract void OnPointerEnter(PointerEventData eventData);
    public abstract void OnPointerExit(PointerEventData eventData);


    public void OnPointerClick(PointerEventData eventData)
    {
        if (_observer != null && _observer.IsReplaying == false)
        {
            OnClickEventHandler?.Invoke(this);
        }
    }

    public void SetMaterial(Material newMaterial, bool setPreviousMaterial = true)
    {
        if (newMaterial.name.Contains(_meshRenderer.material.name)) return;
        if (setPreviousMaterial) PreviousMaterial = new Material(_meshRenderer.material);
        _meshRenderer.material = newMaterial;
    }

    public void SetPreviousMaterial()
    {
        if (PreviousMaterial == null || PreviousMaterial.name.Contains(_meshRenderer.material.name)) return;

        SetMaterial(PreviousMaterial, false);
    }

    protected void OnFocusEventInvoke(CellComponent target, bool isSelect)
    {
        if (_observer.IsReplaying) return;
        
        OnFocusEventHandler?.Invoke(target, isSelect);
    }

    protected virtual void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _observer = FindObjectOfType<Observer.ObserverComponent>();
    }
}

public enum ColorType
{
    White,
    Black
}

public delegate void ClickEventHandler(BaseClickComponent component);

public delegate void FocusEventHandler(BaseClickComponent component, bool isSelect);