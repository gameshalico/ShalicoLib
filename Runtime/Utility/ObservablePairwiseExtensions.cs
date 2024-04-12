using System.Collections.Generic;
using R3;

namespace ShalicoLib.Utility
{
    public static class ObservablePairwiseExtensions
    {
        public static Observable<(T Previous, T Current)> TrackUpdates<T>(this Observable<T> source)
        {
            return source.Pairwise().Where(pair => !EqualityComparer<T>.Default.Equals(pair.Previous, pair.Current));
        }
    }
}