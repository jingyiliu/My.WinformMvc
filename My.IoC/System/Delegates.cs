#if NET20

namespace My.Foundation
{
    public delegate void Action();
    public delegate void Action<in T>(T arg1);
    public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T arg1);
    //public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    //public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
}

#endif
