using System.Threading;
using My.IoC.Helpers;

namespace My.Threading
{
    //see: http://www.adammil.net/blog/v111_Creating_High-Performance_Locks_and_Lock-free_Code_for_NET_.html
    static class Spin
    {
        const int MaxSpins = 20;

        public static void Wait(int spinCount)
        {
            if (spinCount < 5 && SystemHelper.MultiProcessors) Thread.SpinWait(MaxSpins * spinCount);
            else if (spinCount < MaxSpins - 3) Thread.Sleep(0);
            else Thread.Sleep(1);
        }
    }
}