using R3;
using ShalicoLib.Utility;

namespace ShalicoLib.Presentation
{
    public enum PointerButtonViewState
    {
        Normal,
        Disabled,
        Pressed,
        Hovered
    }

    public static class PointerButtonExtensions
    {
        public static PointerButtonViewState ToViewState(this PointerButton.ButtonState state)
        {
            return state switch
            {
                { IsInteractable: false } => PointerButtonViewState.Disabled,
                { IsAnyButtonPressed: true } => PointerButtonViewState.Pressed,
                { IsHovered: true } => PointerButtonViewState.Hovered,
                _ => PointerButtonViewState.Normal
            };
        }

        public static Observable<(PointerButtonViewState Previous, PointerButtonViewState Current)>
            TrackViewState(
                this PointerButton button)
        {
            return button.State.Select(state => state.ToViewState()).TrackUpdates();
        }
    }
}