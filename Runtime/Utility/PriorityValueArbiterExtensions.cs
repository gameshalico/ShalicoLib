using R3;
using ShalicoUtils;

namespace ShalicoLib.Utility
{
    public static class PriorityValueArbiterExtensions
    {
        public static Observable<T> OnValueChangedAsObservable<T>(this PriorityValueArbiter<T> self)
        {
            return Observable.FromEvent<T>(
                handler => self.OnValueChanged += handler,
                handler => self.OnValueChanged -= handler
            );
        }

        public static ReactiveProperty<T> ToReactiveProperty<T>(this PriorityValueArbiter<T> self)
        {
            var rp = new ReactiveProperty<T>(self.Value);
            self.OnValueChanged += value => rp.Value = value;
            return rp;
        }
    }
}