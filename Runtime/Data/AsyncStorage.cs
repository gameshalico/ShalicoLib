using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ShalicoLib.Data
{
    public class AsyncStorage<TContent> : IReadOnlyAsyncStorage<TContent>
    {
        private readonly UniTaskCompletionSource<TContent> _completionSource = new();

        public bool HasContent { get; private set; }

        public async UniTask<TContent> GetAsync(CancellationToken cancellationToken = default)
        {
            return await _completionSource.Task.AttachExternalCancellation(cancellationToken);
        }

        public bool TryGet(out TContent content)
        {
            if (!HasContent || _completionSource.Task.Status != UniTaskStatus.Succeeded)
            {
                content = default;
                return false;
            }

            content = _completionSource.Task.GetAwaiter().GetResult();
            return true;
        }

        public bool TrySet(TContent content)
        {
            if (HasContent) return false;
            HasContent = true;
            return _completionSource.TrySetResult(content);
        }

        public bool TrySetException(Exception exception)
        {
            if (HasContent) return false;
            HasContent = true;
            return _completionSource.TrySetException(exception);
        }

        public bool TryCancel()
        {
            if (HasContent) return false;
            HasContent = true;
            return _completionSource.TrySetCanceled();
        }
    }
}