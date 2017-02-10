using System;

namespace PMSoft.Caching {
    public interface IAsyncTokenProvider {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}