namespace System.Reactive.Subjects
{
    public interface IAsyncSubject<in TInput, out TOutput> : IAsyncObservable<TOutput>, IAsyncObserver<TInput>
    {
    }

    public interface IAsyncSubject<T> : IAsyncSubject<T, T>
    {
    }
}