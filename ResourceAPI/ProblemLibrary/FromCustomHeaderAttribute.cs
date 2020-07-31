using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProblemLibrary
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class FromCustomHeaderAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        public BindingSource BindingSource => BindingSource.Header;
        public string Name { get; set; }
    }
}