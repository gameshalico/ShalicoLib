﻿using System.Threading;
using Cysharp.Threading.Tasks;
using ShalicoLib.Data;
using VContainer;
using VContainer.Unity;

namespace ShalicoLib.Presentation
{
    public abstract class SceneStarter<TContext> : IAsyncStartable
    {
        private readonly IReadOnlyStorage<TContext> _contextStorage;

        [Inject]
        protected SceneStarter(IReadOnlyStorage<TContext> contextStorage)
        {
            _contextStorage = contextStorage;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            if (_contextStorage.TryGet(out var context))
                await StartWithContextAsync(context, cancellation);
            else
                await StartWithoutContextAsync(cancellation);
        }

        protected abstract UniTask StartWithContextAsync(TContext context, CancellationToken cancellation);
        protected abstract UniTask StartWithoutContextAsync(CancellationToken cancellation);
    }
}