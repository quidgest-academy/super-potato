using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder.Extensions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace GenioMVC;

/// <summary>
/// Custom object validator for custom view models
/// </summary>
/// <remarks>
/// The default .Net Core validator iterates through every single property of a sucessfully binded model
///  and executes it immediately so it can check its value against any registered validators.
/// This creates not only a performance problem, because it will recursively iterate into any complex object,
///  but also a initialization problem, in that some properties intended for internal use or deserialization only
///  will be evaluated at a time where we had no oportunity to make any initializations on the viewModel/model.
///  
/// This custom validator allows us to override the default MVC behavior
/// that will reach max recursion on the validations.
///  
/// This code is based on the solution in this article:
/// https://www.dotnet-programming.com/post/2017/04/05/Model-binding-interfaces-fixing-server-side-validation.aspx
/// </remarks>
public class OnDemandObjectValidator : IObjectModelValidator
{
    //replicating what the frameword would do:
    //private readonly ObjectModelValidator _validator;
    public OnDemandObjectValidator(
        IModelMetadataProvider modelMetadataProvider,
        IList<IModelValidatorProvider> validatorProviders,
        MvcOptions mvcOptions)
    {
        //replicating what the frameword would do:
        //_validator = new InternalObjectValidator(modelMetadataProvider, validatorProviders, mvcOptions);
    }

    public void Validate(ActionContext actionContext, ValidationStateDictionary? validationState, string prefix, object? model)
    {
        // Empty
    }
}


/// <summary>
/// InternalObjectValidator
/// </summary>
/// <remarks>
/// DefaultObjectValidator is an internal class of Asp.Net Core so we dont have acess to it.
/// This is a copy of the same code, just to implement the abstract class ObjectModelValidator.
/// We cannot inherit OnDemandObjectValidator directly from ObjectValidator because of the implementation of
///  the ParameterBinder.BindModelAsync, that in case the object is recognized directy as a ObjectModelValidator
///  a different logic branch is used, calling a function called EnforceBindRequiredAndValidate which causes
///  the whole problem.
/// </remarks>
public class InternalObjectValidator : ObjectModelValidator
{
    private readonly MvcOptions _mvcOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="InternalObjectValidator"/>.
    /// </summary>
    /// <param name="modelMetadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
    /// <param name="validatorProviders">The list of <see cref="IModelValidatorProvider"/>.</param>
    /// <param name="mvcOptions">Accessor to <see cref="MvcOptions"/>.</param>
    public InternalObjectValidator(
        IModelMetadataProvider modelMetadataProvider,
        IList<IModelValidatorProvider> validatorProviders,
        MvcOptions mvcOptions)
        : base(modelMetadataProvider, validatorProviders)
    {
        _mvcOptions = mvcOptions;
    }

    public override ValidationVisitor GetValidationVisitor(
        ActionContext actionContext,
        IModelValidatorProvider validatorProvider,
        ValidatorCache validatorCache,
        IModelMetadataProvider metadataProvider,
        ValidationStateDictionary? validationState)
    {
        var visitor = new ValidationVisitor(
            actionContext,
            validatorProvider,
            validatorCache,
            metadataProvider,
            validationState)
        {
            MaxValidationDepth = _mvcOptions.MaxValidationDepth,
            ValidateComplexTypesIfChildValidationFails = _mvcOptions.ValidateComplexTypesIfChildValidationFails,
        };

        return visitor;
    }
}
