using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ShalicoLib.Presentation
{
    public class AsyncStorage<TContent>
    {
        private readonly UniTaskCompletionSource<TContent> _completionSource = new();

        public bool HasContent { get; private set; }

        public async UniTask<TContent> Get(CancellationToken cancellationToken = default)
        {
            var result = await UniTask.WhenAny(_completionSource.Task, cancellationToken.ToUniTask().Item1);
            if (result.hasResultLeft) return result.result;

            throw new OperationCanceledException();
        }

        public void Set(TContent content)
        {
            if (HasContent) throw new InvalidOperationException("Content is already set");
            HasContent = true;
            _completionSource.TrySetResult(content);
        }
    }
}