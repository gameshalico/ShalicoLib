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

    public class AsyncStorage<TContent> : IReadOnlyAsyncStorage<TContent>
    {
        private readonly UniTaskCompletionSource<TContent> _completionSource = new();

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