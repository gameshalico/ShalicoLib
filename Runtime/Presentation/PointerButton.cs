using System;
using R3;
using ShalicoUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShalicoLib.Presentation
{
    public class PointerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool _isDefaultInteractable = true;

        private readonly bool[] _isButtonsPressed = new bool[3];
        private readonly Subject<ButtonEvent> _onButtonClicked = new();
        private readonly Subject<ButtonEvent> _onButtonDown = new();
        private readonly Subject<ButtonEvent> _onButtonPointerEnter = new();
        private readonly Subject<ButtonEvent> _onButtonPointerExit = new();
        private readonly Subject<ButtonEvent> _onButtonUp = new();
        private readonly ReactiveProperty<ButtonState> _state = new();
        private bool _isHovered;
        public Observable<ButtonEvent> OnButtonClicked => _onButtonClicked;
        public Observable<ButtonEvent> OnButtonDown => _onButtonDown;
        public Observable<ButtonEvent> OnButtonUp => _onButtonUp;
        public Observable<ButtonEvent> OnButtonPointerEnter => _onButtonPointerEnter;
        public Observable<ButtonEvent> OnButtonPointerExit => _onButtonPointerExit;

        public ReadOnlyReactiveProperty<ButtonState> State => _state;
        public PriorityValueArbiter<bool> Interactable { get; } = new();

        private void Awake()
        {
            Interactable.DefaultValue = _isDefaultInteractable;
            UpdateState();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onButtonClicked.OnNext(new ButtonEvent(eventData, _state.Value));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isButtonsPressed[(int)eventData.button] = true;
            UpdateState();

            _onButtonDown.OnNext(new ButtonEvent(eventData, _state.Value));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateState();

            _onButtonPointerEnter.OnNext(new ButtonEvent(eventData, _state.Value));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateState();

            _onButtonPointerExit.OnNext(new ButtonEvent(eventData, _state.Value));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isButtonsPressed[(int)eventData.button] = false;
            UpdateState();

            _onButtonUp.OnNext(new ButtonEvent(eventData, _state.Value));
        }

        private void UpdateState()
        {
            _state.Value = new ButtonState(Interactable.Value, _isHovered, _isButtonsPressed);
        }

        [Flags]
        private enum ButtonFlags
        {
            Interactable = 1 << 0,
            Hovered = 1 << 1,

            LeftButtonPressed = 1 << 2,
            RightButtonPressed = 1 << 3,
            MiddleButtonPressed = 1 << 4,

            AnyButtonPressed = LeftButtonPressed | RightButtonPressed | MiddleButtonPressed
        }

        public readonly struct ButtonEvent : IEquatable<ButtonEvent>
        {
            public PointerEventData EventData { get; }
            public ButtonState State { get; }

            public ButtonEvent(PointerEventData eventData, ButtonState state)
            {
                EventData = eventData;
                State = state;
            }

            public bool Equals(ButtonEvent other)
            {
                return Equals(EventData, other.EventData) && State.Equals(other.State);
            }

            public override bool Equals(object obj)
            {
                return obj is ButtonEvent other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(EventData, State);
            }
        }


        public readonly struct ButtonState : IEquatable<ButtonState>
        {
            private readonly ButtonFlags _flags;

            public ButtonState(bool isInteractable, bool isHovered, bool[] isButtonsPressed)
            {
                _flags = 0;
                if (isInteractable) _flags |= ButtonFlags.Interactable;
                if (isHovered) _flags |= ButtonFlags.Hovered;
                if (isButtonsPressed[(int)PointerEventData.InputButton.Left]) _flags |= ButtonFlags.LeftButtonPressed;
                if (isButtonsPressed[(int)PointerEventData.InputButton.Right]) _flags |= ButtonFlags.RightButtonPressed;
                if (isButtonsPressed[(int)PointerEventData.InputButton.Middle])
                    _flags |= ButtonFlags.MiddleButtonPressed;
            }

            public bool IsInteractable => (_flags & ButtonFlags.Interactable) != 0;
            public bool IsHovered => (_flags & ButtonFlags.Hovered) != 0;
            public bool IsLeftButtonPressed => (_flags & ButtonFlags.LeftButtonPressed) != 0;
            public bool IsRightButtonPressed => (_flags & ButtonFlags.RightButtonPressed) != 0;
            public bool IsMiddleButtonPressed => (_flags & ButtonFlags.MiddleButtonPressed) != 0;
            public bool IsAnyButtonPressed => (_flags & ButtonFlags.AnyButtonPressed) != 0;

            public bool Equals(ButtonState other)
            {
                return _flags == other._flags;
            }

            public override bool Equals(object obj)
            {
                return obj is ButtonState other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (int)_flags;
            }
        }
    }
}