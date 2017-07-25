using System.Text;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Core;

namespace My.IoC.Helpers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ExceptionFormatter
    {
        /// <summary>
        /// Formats the specified message based on the calling context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Format(string message, params object[] args)
        {
            return Format(null, message, args);
        }

        /// <summary>
        /// Formats the specified message based on the calling context.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Format(InjectionContext context, string message, params object[] args)
        {
            Requires.NotNullOrEmpty(message, "message");
            var strBuilder = new StringBuilder();
            if (args == null)
                strBuilder.Append(message);
            else
                strBuilder.AppendFormat(null, message, args);

            if (context == null)
                return strBuilder.ToString();

            var parentContexts = GetParentContexts(context);
            if (parentContexts == null)
                return strBuilder.ToString();

            // If the currentNode is not null, the rootNode will not be null.
            var requestedContext = parentContexts[parentContexts.Count - 1];
            strBuilder.AppendLine();
            strBuilder.AppendLine();
            AppendRequestedObjectDescription(strBuilder, requestedContext.ObjectDescription);

            if (parentContexts.Count == 1)
                return strBuilder.ToString();

            strBuilder.AppendLine();
            strBuilder.AppendLine();
            AppendCallPath(strBuilder, context, parentContexts);

            return strBuilder.ToString();
        }

        static List<InjectionContext> GetParentContexts(InjectionContext currentContext)
        {
            List<InjectionContext> parentContexts = null;
            var context = currentContext.ParentContext;
            while (context != null)
            {
                if (parentContexts == null)
                    parentContexts = new List<InjectionContext>();
                parentContexts.Add(context);
                context = context.ParentContext;
            }
            return parentContexts;
        }

        static void AppendRequestedObjectDescription(StringBuilder sb, ObjectDescription description)
        {
            sb.Append(Resources.BriefDescriptionOfTheRequestedServiceIs);
            sb.AppendLine();
            AppendObjectDescriptionDetail(sb, description);
        }

        static void AppendObjectDescriptionDetail(StringBuilder sb, ObjectDescription description)
        {
            sb.Append("ContractType = ");
            sb.Append(description.ContractType.ToTypeName());

            sb.AppendLine();
            sb.Append("ConcreteType = ");
            sb.Append(description.ConcreteType.ToTypeName());
        }

        static void AppendCallPath(StringBuilder sb, InjectionContext currentContext, List<InjectionContext> parentContexts)
        {
            sb.Append(Resources.TheCallPathIs);
            sb.AppendLine();

            var parentCount = parentContexts.Count;
            for (int i = parentContexts.Count - 1; i >= 0; i--)
            {
                var context = parentContexts[i];

                sb.Append(parentCount - i);
                sb.Append(") ");
                sb.Append(context.ObjectDescription.ConcreteType.ToTypeName());
                sb.AppendLine();
            }

            sb.Append(parentCount + 1);
            sb.Append(") ");
            sb.Append(currentContext.ObjectDescription.ConcreteType.ToTypeName());
        }
    }
}
