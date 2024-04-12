using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.ShalicoLib.Model;
using VContainer;
using VContainer.Unity;

namespace ShalicoLib.Presentation
{
    public abstract class SceneStarter<TContext> : IAsyncStartable
    {
        private readonly AsyncStorage<TContext> _contextStorage;

        [Inject]
        protected SceneStarter(AsyncStorage<TContext> contextStorage)
        {
            _contextStorage = contextStorage;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            if (_contextStorage.HasContent)
            {
                var context = await _contextStorage.Get(cancellation);

                await StartWithContextAsync(context, cancellation);
            }
            else
            {
                await StartWithoutContextAsync(cancellation);
            }
        }

        protected abstract UniTask StartWithContextAsync(TContext context, CancellationToken cancellation);
        protected abstract UniTask StartWithoutContextAsync(CancellationToken cancellation);
    }
}