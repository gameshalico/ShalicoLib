using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Packages.ShalicoLib.Model
{
    public interface IReadOnlyAsyncStorage<TContent>
    {
        bool HasContent { get; }
        UniTask<TContent> Get(CancellationToken cancellationToken = default);
        bool TryGet(out TContent content);
    }

    public class AsyncStorage<TContent> : IReadOnlyAsyncStorage<TContent>, IDisposable
    {
        private readonly UniTaskCompletionSource<TContent> _completionSource = new();

        public void Dispose()
        {
            if (!HasContent)
                _completionSource.TrySetException(new ObjectDisposedException(nameof(AsyncStorage<TContent>)));
        }

        public bool HasContent { get; private set; }

        public async UniTask<TContent> Get(CancellationToken cancellationToken = default)
        {
            var result = await UniTask.WhenAny(_completionSource.Task, cancellationToken.ToUniTask().Item1);
            if (result.hasResultLeft) return result.result;

            throw new OperationCanceledException();
        }

        public bool TryGet(out TContent content)
        {
            if (_completionSource.Task.Status != UniTaskStatus.Succeeded)
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
            _completionSource.TrySetResult(content);
            return true;
        }

        public bool TrySetException(Exception exception)
        {
            if (HasContent) return false;
            HasContent = true;
            _completionSource.TrySetException(exception);
            return true;
        }

        public bool TryCancel()
        {
            if (HasContent) return false;
            HasContent = true;
            _completionSource.TrySetCanceled();
            return true;
        }
    }
}