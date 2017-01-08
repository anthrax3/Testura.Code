﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Testura.Code.Helper.Arguments;
using Testura.Code.Helper.Arguments.ArgumentTypes;

namespace Testura.Code.Helper.References
{
    public class Reference
    {
        /// <summary>
        /// Generate the code for a variable reference chain, for example: 
        /// 
        /// myVariable.SomeMethod().MyProperty
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static ExpressionSyntax Create(VariableReference reference)
        {
            var baseExpression = SyntaxFactory.IdentifierName(reference.Name);
            if (reference.Member == null)
            {
                return baseExpression;
            }

            return Generate(baseExpression, reference.Member);
        }

        /// <summary>
        /// Generate the code for a member chain. This method is used if you already have a variable, member or method invocation and want to 
        /// extend it with more references calls. 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static ExpressionSyntax Create(ExpressionSyntax invocation, MemberReference reference)
        {
           return Generate(invocation, reference);
        }

        /// <summary>
        /// Generate the code for a member reference
        /// </summary>
        /// <param name="expressionSyntax"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static ExpressionSyntax Generate(ExpressionSyntax expressionSyntax, MemberReference current)
        {
            if (current == null)
                return expressionSyntax;
            if (current.ReferenceType == MemberReferenceTypes.Field || current.ReferenceType == MemberReferenceTypes.Property)
            {
                expressionSyntax = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expressionSyntax, SyntaxFactory.IdentifierName(current.Name));
            }
            else if (current.ReferenceType == MemberReferenceTypes.Method)
            {
                IList<IArgument> arguments = new List<IArgument>();
                if (current is MethodReference)
                {
                    arguments = ((MethodReference) current).Arguments;
                }

                 expressionSyntax = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expressionSyntax,
                            SyntaxFactory.IdentifierName(current.Name))).WithArgumentList(
                        Argument.Create(arguments.ToArray()));
            }
            return Generate(expressionSyntax, current.Member);
        }

    }
}