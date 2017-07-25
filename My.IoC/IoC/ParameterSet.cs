
using System.Collections;
using System.Collections.Generic;
using My.Helpers;

namespace My.IoC
{
    public enum ParameterKind
    {
        Positional = 0,
        Named = 1
    }

    public abstract class ParameterSet : IEnumerable<Parameter>
    {
        public abstract int Length { get; }
        public abstract ParameterKind ParameterKind { get; }
        public abstract Parameter this[int index] { get; }

        #region IEnumerable<Parameter> Members

        public abstract IEnumerator<Parameter> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PositionalParameterSet : ParameterSet
    {
        readonly IList<PositionalParameter> _positionalParameters;

        public PositionalParameterSet(params PositionalParameter[] positionalParameters)
        {
            Requires.NotNull(positionalParameters, "positionalParameters");
            _positionalParameters = positionalParameters;
        }

        public PositionalParameterSet(IList<PositionalParameter> positionalParameters)
        {
            Requires.NotNull(positionalParameters, "positionalParameters");
            _positionalParameters = positionalParameters;
        }

        public override ParameterKind ParameterKind
        {
            get { return ParameterKind.Positional; }
        }

        public override int Length
        {
            get { return _positionalParameters.Count; }
        }

        public override Parameter this[int index]
        {
            get { return _positionalParameters[index]; }
        }

        public override IEnumerator<Parameter> GetEnumerator()
        {
            foreach (var positionalParameter in _positionalParameters)
                yield return positionalParameter;
        }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NamedParameterSet : ParameterSet
    {
        readonly IList<NamedParameter> _namedParameters;

        public NamedParameterSet(params NamedParameter[] namedParameters)
        {
            Requires.NotNull(namedParameters, "namedParameters");
            _namedParameters = namedParameters;
        }

        public NamedParameterSet(IList<NamedParameter> namedParameters)
        {
            Requires.NotNull(namedParameters, "namedParameters");
            _namedParameters = namedParameters;
        }

        public override ParameterKind ParameterKind
        {
            get { return ParameterKind.Named; }
        }

        public override int Length
        {
            get { return _namedParameters.Count; }
        }

        public override Parameter this[int index]
        {
            get { return _namedParameters[index]; }
        }

        public override IEnumerator<Parameter> GetEnumerator()
        {
            foreach (var namedParameter in _namedParameters)
                yield return namedParameter;
        }
    }
}
