namespace ThorClient.Clients.Base
{
    public interface ISubscribingCallback<in T>
    {
        void OnSubscribe(T response);
    }
}
