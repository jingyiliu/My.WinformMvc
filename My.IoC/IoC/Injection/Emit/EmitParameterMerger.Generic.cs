using My.Exceptions;
using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Injection.Emit
{
    public sealed class EmitParameterMerger<T0> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 1);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
        }

        public void Merge(InjectionContext context, out T0 param0)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                return;
            }

            var paramLength = myParams.Length;
            if (paramLength != 1)
                throw ParameterNumberExceeds(context, 1, paramLength);

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 2);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                return;
            }

            var paramLength = myParams.Length;
            if (paramLength > 2)
                throw ParameterNumberExceeds(context, 2, paramLength);

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                    if (paramLength == 1)
                        _depProvider1.CreateObject(context, out param1);
                    else
                        param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1, T2> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;
        readonly DependencyProvider<T2> _depProvider2;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 3);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
            _depProvider2 = (DependencyProvider<T2>)dependencyProviders[2];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1, out T2 param2)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                _depProvider2.CreateObject(context, out param2);
                return;
            }

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    switch (myParams.Length)
                    {
                        case 1:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            _depProvider1.CreateObject(context, out param1);
                            _depProvider2.CreateObject(context, out param2);
                            break;
                        case 2:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            _depProvider2.CreateObject(context, out param2);
                            break;
                        case 3:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            break;
                        default:
                            throw ParameterNumberExceeds(context, 3, myParams.Length);
                    }
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    param2 = GetNamedDependencyObject(_depProvider2, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1, T2, T3> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;
        readonly DependencyProvider<T2> _depProvider2;
        readonly DependencyProvider<T3> _depProvider3;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 4);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
            _depProvider2 = (DependencyProvider<T2>)dependencyProviders[2];
            _depProvider3 = (DependencyProvider<T3>)dependencyProviders[3];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1, out T2 param2, out T3 param3)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                _depProvider2.CreateObject(context, out param2);
                _depProvider3.CreateObject(context, out param3);
                return;
            }

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    switch (myParams.Length)
                    {
                        case 1:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            _depProvider1.CreateObject(context, out param1);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            break;
                        case 2:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            break;
                        case 3:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            _depProvider3.CreateObject(context, out param3);
                            break;
                        case 4:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            break;
                        default:
                            throw ParameterNumberExceeds(context, 4, myParams.Length);
                    }
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    param2 = GetNamedDependencyObject(_depProvider2, myParams, context);
                    param3 = GetNamedDependencyObject(_depProvider3, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1, T2, T3, T4> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;
        readonly DependencyProvider<T2> _depProvider2;
        readonly DependencyProvider<T3> _depProvider3;
        readonly DependencyProvider<T4> _depProvider4;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 5);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
            _depProvider2 = (DependencyProvider<T2>)dependencyProviders[2];
            _depProvider3 = (DependencyProvider<T3>)dependencyProviders[3];
            _depProvider4 = (DependencyProvider<T4>)dependencyProviders[4];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1, out T2 param2, out T3 param3, out T4 param4)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                _depProvider2.CreateObject(context, out param2);
                _depProvider3.CreateObject(context, out param3);
                _depProvider4.CreateObject(context, out param4);
                return;
            }

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    switch (myParams.Length)
                    {
                        case 1:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            _depProvider1.CreateObject(context, out param1);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            break;
                        case 2:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            break;
                        case 3:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            break;
                        case 4:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            _depProvider4.CreateObject(context, out param4);
                            break;
                        case 5:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            break;
                        default:
                            throw ParameterNumberExceeds(context, 5, myParams.Length);
                    }
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    param2 = GetNamedDependencyObject(_depProvider2, myParams, context);
                    param3 = GetNamedDependencyObject(_depProvider3, myParams, context);
                    param4 = GetNamedDependencyObject(_depProvider4, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1, T2, T3, T4, T5> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;
        readonly DependencyProvider<T2> _depProvider2;
        readonly DependencyProvider<T3> _depProvider3;
        readonly DependencyProvider<T4> _depProvider4;
        readonly DependencyProvider<T5> _depProvider5;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 6);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
            _depProvider2 = (DependencyProvider<T2>)dependencyProviders[2];
            _depProvider3 = (DependencyProvider<T3>)dependencyProviders[3];
            _depProvider4 = (DependencyProvider<T4>)dependencyProviders[4];
            _depProvider5 = (DependencyProvider<T5>)dependencyProviders[5];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1, out T2 param2, out T3 param3, out T4 param4, out T5 param5)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                _depProvider2.CreateObject(context, out param2);
                _depProvider3.CreateObject(context, out param3);
                _depProvider4.CreateObject(context, out param4);
                _depProvider5.CreateObject(context, out param5);
                return;
            }

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    switch (myParams.Length)
                    {
                        case 1:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            _depProvider1.CreateObject(context, out param1);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            break;
                        case 2:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            break;
                        case 3:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            break;
                        case 4:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            break;
                        case 5:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            _depProvider5.CreateObject(context, out param5);
                            break;
                        case 6:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            param5 = GetPositionalDependencyObject(_depProvider5, myParams[5], context);
                            break;
                        default:
                            throw ParameterNumberExceeds(context, 6, myParams.Length);
                    }
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    param2 = GetNamedDependencyObject(_depProvider2, myParams, context);
                    param3 = GetNamedDependencyObject(_depProvider3, myParams, context);
                    param4 = GetNamedDependencyObject(_depProvider4, myParams, context);
                    param5 = GetNamedDependencyObject(_depProvider5, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1, T2, T3, T4, T5, T6> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;
        readonly DependencyProvider<T2> _depProvider2;
        readonly DependencyProvider<T3> _depProvider3;
        readonly DependencyProvider<T4> _depProvider4;
        readonly DependencyProvider<T5> _depProvider5;
        readonly DependencyProvider<T6> _depProvider6;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 7);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
            _depProvider2 = (DependencyProvider<T2>)dependencyProviders[2];
            _depProvider3 = (DependencyProvider<T3>)dependencyProviders[3];
            _depProvider4 = (DependencyProvider<T4>)dependencyProviders[4];
            _depProvider5 = (DependencyProvider<T5>)dependencyProviders[5];
            _depProvider6 = (DependencyProvider<T6>)dependencyProviders[6];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1, out T2 param2, out T3 param3, out T4 param4, out T5 param5, out T6 param6)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                _depProvider2.CreateObject(context, out param2);
                _depProvider3.CreateObject(context, out param3);
                _depProvider4.CreateObject(context, out param4);
                _depProvider5.CreateObject(context, out param5);
                _depProvider6.CreateObject(context, out param6);
                return;
            }

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    switch (myParams.Length)
                    {
                        case 1:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            _depProvider1.CreateObject(context, out param1);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            break;
                        case 2:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            break;
                        case 3:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            break;
                        case 4:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            break;
                        case 5:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            break;
                        case 6:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            param5 = GetPositionalDependencyObject(_depProvider5, myParams[5], context);
                            _depProvider6.CreateObject(context, out param6);
                            break;
                        case 7:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            param5 = GetPositionalDependencyObject(_depProvider5, myParams[5], context);
                            param6 = GetPositionalDependencyObject(_depProvider6, myParams[6], context);
                            break;
                        default:
                            throw ParameterNumberExceeds(context, 7, myParams.Length);
                    }
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    param2 = GetNamedDependencyObject(_depProvider2, myParams, context);
                    param3 = GetNamedDependencyObject(_depProvider3, myParams, context);
                    param4 = GetNamedDependencyObject(_depProvider4, myParams, context);
                    param5 = GetNamedDependencyObject(_depProvider5, myParams, context);
                    param6 = GetNamedDependencyObject(_depProvider6, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }

    public sealed class EmitParameterMerger<T0, T1, T2, T3, T4, T5, T6, T7> : EmitParameterMerger
    {
        readonly DependencyProvider<T0> _depProvider0;
        readonly DependencyProvider<T1> _depProvider1;
        readonly DependencyProvider<T2> _depProvider2;
        readonly DependencyProvider<T3> _depProvider3;
        readonly DependencyProvider<T4> _depProvider4;
        readonly DependencyProvider<T5> _depProvider5;
        readonly DependencyProvider<T6> _depProvider6;
        readonly DependencyProvider<T7> _depProvider7;

        public EmitParameterMerger(DependencyProvider[] dependencyProviders)
        {
            VerfiyConstructorParameterLength(dependencyProviders, 8);
            _depProvider0 = (DependencyProvider<T0>)dependencyProviders[0];
            _depProvider1 = (DependencyProvider<T1>)dependencyProviders[1];
            _depProvider2 = (DependencyProvider<T2>)dependencyProviders[2];
            _depProvider3 = (DependencyProvider<T3>)dependencyProviders[3];
            _depProvider4 = (DependencyProvider<T4>)dependencyProviders[4];
            _depProvider5 = (DependencyProvider<T5>)dependencyProviders[5];
            _depProvider6 = (DependencyProvider<T6>)dependencyProviders[6];
            _depProvider7 = (DependencyProvider<T7>)dependencyProviders[7];
        }

        public void Merge(InjectionContext context, out T0 param0, out T1 param1, out T2 param2, out T3 param3, out T4 param4, out T5 param5, out T6 param6, out T7 param7)
        {
            var myParams = context.Parameters;
            if (myParams == null || myParams.Length == 0)
            {
                _depProvider0.CreateObject(context, out param0);
                _depProvider1.CreateObject(context, out param1);
                _depProvider2.CreateObject(context, out param2);
                _depProvider3.CreateObject(context, out param3);
                _depProvider4.CreateObject(context, out param4);
                _depProvider5.CreateObject(context, out param5);
                _depProvider6.CreateObject(context, out param6);
                _depProvider7.CreateObject(context, out param7);
                return;
            }

            switch (myParams.ParameterKind)
            {
                case ParameterKind.Positional:
                    switch (myParams.Length)
                    {
                        case 1:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            _depProvider1.CreateObject(context, out param1);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 2:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            _depProvider2.CreateObject(context, out param2);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 3:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            _depProvider3.CreateObject(context, out param3);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 4:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            _depProvider4.CreateObject(context, out param4);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 5:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            _depProvider5.CreateObject(context, out param5);
                            _depProvider6.CreateObject(context, out param6);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 6:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            param5 = GetPositionalDependencyObject(_depProvider5, myParams[5], context);
                            _depProvider6.CreateObject(context, out param6);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 7:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            param5 = GetPositionalDependencyObject(_depProvider5, myParams[5], context);
                            param6 = GetPositionalDependencyObject(_depProvider6, myParams[6], context);
                            _depProvider7.CreateObject(context, out param7);
                            break;
                        case 8:
                            param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                            param1 = GetPositionalDependencyObject(_depProvider1, myParams[1], context);
                            param2 = GetPositionalDependencyObject(_depProvider2, myParams[2], context);
                            param3 = GetPositionalDependencyObject(_depProvider3, myParams[3], context);
                            param4 = GetPositionalDependencyObject(_depProvider4, myParams[4], context);
                            param5 = GetPositionalDependencyObject(_depProvider5, myParams[5], context);
                            param6 = GetPositionalDependencyObject(_depProvider6, myParams[6], context);
                            param7 = GetPositionalDependencyObject(_depProvider7, myParams[7], context);
                            break;
                        default:
                            throw ParameterNumberExceeds(context, 8, myParams.Length);
                    }
                    break;
                case ParameterKind.Named:
                    param0 = GetNamedDependencyObject(_depProvider0, myParams, context);
                    param1 = GetNamedDependencyObject(_depProvider1, myParams, context);
                    param2 = GetNamedDependencyObject(_depProvider2, myParams, context);
                    param3 = GetNamedDependencyObject(_depProvider3, myParams, context);
                    param4 = GetNamedDependencyObject(_depProvider4, myParams, context);
                    param5 = GetNamedDependencyObject(_depProvider5, myParams, context);
                    param6 = GetNamedDependencyObject(_depProvider6, myParams, context);
                    param7 = GetNamedDependencyObject(_depProvider7, myParams, context);
                    break;
                default:
                    throw new ImpossibleException();
            }
        }
    }
}
