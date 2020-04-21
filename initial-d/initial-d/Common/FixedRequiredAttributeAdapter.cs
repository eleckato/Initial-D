﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Common
{
    public class FixedRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        /* This is part of the insane boilerplate code to localize the form validation messages */
        public FixedRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Resources.ValidationResources.PropertyValueRequired;
            return new[] { new ModelClientValidationRequiredRule(errorMessage)  };
        }
    }
}