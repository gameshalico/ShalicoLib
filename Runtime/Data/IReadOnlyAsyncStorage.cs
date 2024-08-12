using System.Threading;
using Cysharp.Threading.Tasks;

namespace ShalicoLib.Data
{
    public interface IReadOnlyAsyncStorage<TContent>
    {
        bool HasContent { get; }
        UniTask<TContent> GetAsync(CancellationToken cancellationToken = default);
        bool TryGet(out TContent content);
    }
}