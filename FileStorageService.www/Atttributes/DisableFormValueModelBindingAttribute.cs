using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FileStorageService.www.Atttributes;

/// <summary>
/// https://stackoverflow.com/a/62555240/13204730
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
{
	public void OnResourceExecuting(ResourceExecutingContext context)
	{
		var factories = context.ValueProviderFactories;
		// factories.RemoveType<FormValueProviderFactory>();
		factories.RemoveType<FormFileValueProviderFactory>();
		factories.RemoveType<JQueryFormValueProviderFactory>();
	}

	public void OnResourceExecuted(ResourceExecutedContext context)
	{
	}
}