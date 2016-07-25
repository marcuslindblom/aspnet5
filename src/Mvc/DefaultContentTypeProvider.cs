using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace src.Mvc
{
/*    public class DefaultContentTypeProvider : IContentTypeProvider
    {
        private readonly IAssemblyProvider _assemblyProvider;
        public DefaultContentTypeProvider(IAssemblyProvider assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
        }
        public IEnumerable<TypeInfo> ContentTypes
        {
            get
            {
                var candidateAssemblies = new HashSet<Assembly>(_assemblyProvider.CandidateAssemblies);
                var types = candidateAssemblies.SelectMany(a => a.DefinedTypes);
                return types.Where(HasContentTypeAttribute);
            }
        }
        private bool HasContentTypeAttribute(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttributes(typeof(ViewModelAttribute), false).Any();
        }


    }*/
    public interface IContentTypeProvider
    {
        IEnumerable<TypeInfo> ContentTypes { get; }
    }
}
