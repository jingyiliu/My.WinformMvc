
using System.Reflection;

namespace My.IoC.Injection.Emit
{
    class EmitInjectorKey
    {
        protected readonly ConstructorInfo MyConstructor;

        public EmitInjectorKey(ConstructorInfo constructor)
		{
            MyConstructor = constructor;
		}

        public ConstructorInfo Constructor
		{
            get { return MyConstructor; }
		}

        public override int GetHashCode()
        {
            return MyConstructor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var key = obj as EmitInjectorKey;
            return key == null ? false : MyConstructor == key.MyConstructor;
        }
	}

    class EmitConstructorAndMemberInjectorKey : EmitInjectorKey
    {
        readonly MethodInfo[] _methods;

        public EmitConstructorAndMemberInjectorKey(ConstructorInfo constructor, MethodInfo[] methods)
            : base(constructor)
        {
            _methods = methods;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var key = obj as EmitConstructorAndMemberInjectorKey;
            if (key == null 
                || MyConstructor != key.MyConstructor 
                || _methods.Length != key._methods.Length)
                return false;

            for (int i = 0; i < _methods.Length; i++)
            {
                if (_methods[i] != key._methods[i])
                    return false;
            }

            return true;
        }
    }
}
