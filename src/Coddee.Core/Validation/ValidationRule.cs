﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq.Expressions;

namespace Coddee.Validation
{
    public class ValidationRule : IValidationRule
    {
        public static ValidationRule CreateWarningRule(Validator validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Warning, validator, message);
        }
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField, Validator<T> validator)
        {
            return new ValidationRule(ValidationType.Warning, e => validator((T)e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField, Validator<T> validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Warning, e => validator((T)e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField, Func<string> message)
        {
            return new ValidationRule(ValidationType.Warning, e => Validators.GetValidator(typeof(T))(e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField)
        {
            return new ValidationRule(ValidationType.Warning, e => Validators.GetValidator(typeof(T))(e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        public static ValidationRule CreateErrorRule(Validator validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Error, validator, message);
        }

        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField, Validator<T> validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Error, e => validator((T)e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }
        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField, Validator<T> validator)
        {
            return new ValidationRule(ValidationType.Error, e => validator((T)e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }
        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField, Func<string> message)
        {
            return new ValidationRule(ValidationType.Error, e => Validators.GetValidator(typeof(T))(e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField)
        {
            return new ValidationRule(ValidationType.Error, e => Validators.GetValidator(typeof(T))(e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        private ValidationRule(ValidationType validationType, Validator validator, Func<string> message)
        {
            ValidationType = validationType;
            Validator = validator;
            Message = message;
        }

        public ValidationRule(ValidationType validationType, Validator validator, Func<string> message, string fieldName) : this(validationType, validator, message)
        {
            FieldName = fieldName;
        }

        public ValidationRule(ValidationType validationType, Validator validator, Func<string> message, string fieldName, Func<object> validatedField) : this(validationType, validator, message, fieldName)
        {
            ValidatedField = validatedField;
        }
        public ValidationRule(ValidationType validationType, Validator validator, string fieldName, Func<object> validatedField) : this(validationType, validator, () => DefaultMessages.ValidationMessage.Replace("$Field$", fieldName), fieldName)
        {
            ValidatedField = validatedField;
        }

        public ValidationType ValidationType { get; }
        public Validator Validator { get; }
        public Func<string> Message { get; set; }
        public string FieldName { get; set; }

        public bool Validate()
        {
            if (ValidatedField != null)
            {
                var value = ValidatedField();
                return Validator(value);
            }
            return Validator(null);
        }

        public string GetMessage()
        {
            return Message();
        }

        public Func<object> ValidatedField { get; set; }
    }
}
