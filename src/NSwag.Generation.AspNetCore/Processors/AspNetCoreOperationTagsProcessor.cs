﻿//-----------------------------------------------------------------------
// <copyright file="AspNetCoreOperationTagsProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#pragma warning disable IDE0005

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Namotion.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using NJsonSchema.Generation;

namespace NSwag.Generation.AspNetCore.Processors
{
    /// <summary>Processes the SwaggerTagsAttribute on the operation method.</summary>
    public class AspNetCoreOperationTagsProcessor : OperationTagsProcessor
    {
        /// <inheritdocs />
        public override bool Process(OperationProcessorContext context)
        {
#if NET6_0_OR_GREATER
            var aspNetCoreContext = (AspNetCoreOperationProcessorContext)context;
            var tagsAttributes = aspNetCoreContext
                .ApiDescription?
                .ActionDescriptor?
                .EndpointMetadata?
                .OfType<TagsAttribute>();

            if (tagsAttributes != null)
            {
                var tags = aspNetCoreContext.OperationDescription.Operation.Tags;
                foreach (var tag in tagsAttributes
                    .SelectMany(a => a.Tags)
                    .Where(t => !tags.Contains(t)))
                {
                    tags.Add(tag);
                }
            }
#endif

            return base.Process(context);
        }

        /// <summary>Adds the controller name as operation tag.</summary>
        /// <param name="context">The context.</param>
        protected override void AddControllerNameTag(OperationProcessorContext context)
        {
            var aspNetCoreContext = (AspNetCoreOperationProcessorContext)context;
            if (aspNetCoreContext.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var summary = controllerActionDescriptor.ControllerTypeInfo.GetXmlDocsSummary(context.Settings.SchemaSettings.GetXmlDocsOptions());
                aspNetCoreContext.OperationDescription.Operation.Tags.Add(controllerActionDescriptor.ControllerName);
                UpdateDocumentTagDescription(context, controllerActionDescriptor.ControllerName, summary);
            }
            else
            {
                base.AddControllerNameTag(context);
            }
        }
    }
}