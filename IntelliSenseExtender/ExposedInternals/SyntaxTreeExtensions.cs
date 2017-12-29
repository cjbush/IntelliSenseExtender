﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace IntelliSenseExtender.ExposedInternals
{
    /// <summary>
    /// Exposing some methods from internal class
    /// <see cref="Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery.SyntaxTreeExtensions"/>
    /// </summary>
    public static class SyntaxTreeExtensions
    {
        private static readonly Type _contextQueryInternalType;
        private static readonly IsTypeContextHandler _isTypeContextMethod;
        private static readonly IsAttributeNameContextHandler _isAttributeNameContextMethod;

        private static readonly Type _sharedInternalType;
        private static readonly FindTokenOnLeftOfPositionHandler _findTokenOnLeftOfPositionMethod;

        private delegate bool IsTypeContextHandler(SyntaxTree syntaxTree, int position, CancellationToken cancellationToken, SemanticModel semanticModelOpt = null);

        private delegate bool IsAttributeNameContextHandler(SyntaxTree syntaxTree, int position, CancellationToken cancellationToken);

        private delegate SyntaxToken FindTokenOnLeftOfPositionHandler(
            SyntaxTree syntaxTree,
            int position,
            CancellationToken cancellationToken,
            bool includeSkipped = true,
            bool includeDirectives = false,
            bool includeDocumentationComments = false);

        static SyntaxTreeExtensions()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var cSharpWorkspacesAssembly = assemblies.First(a => a.FullName.Contains("Microsoft.CodeAnalysis.CSharp.Workspaces"));

            _contextQueryInternalType = cSharpWorkspacesAssembly.GetType("Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery.SyntaxTreeExtensions");
            _isTypeContextMethod = (IsTypeContextHandler)_contextQueryInternalType.GetMethod("IsTypeContext").CreateDelegate(typeof(IsTypeContextHandler));
            _isAttributeNameContextMethod = (IsAttributeNameContextHandler)_contextQueryInternalType.GetMethod("IsAttributeNameContext").CreateDelegate(typeof(IsAttributeNameContextHandler));

            var workspacesAssembly = assemblies.First(a => a.FullName.Contains("Microsoft.CodeAnalysis.Workspaces"));
            _sharedInternalType = workspacesAssembly.GetType("Microsoft.CodeAnalysis.Shared.Extensions.SyntaxTreeExtensions");
            _findTokenOnLeftOfPositionMethod = (FindTokenOnLeftOfPositionHandler)_sharedInternalType.GetMethod("FindTokenOnLeftOfPosition").CreateDelegate(typeof(FindTokenOnLeftOfPositionHandler));
        }

        public static bool IsTypeContext(
            this SyntaxTree syntaxTree, int position, CancellationToken cancellationToken, SemanticModel semanticModelOpt = null)
        {
            var result = _isTypeContextMethod(syntaxTree, position, cancellationToken, semanticModelOpt);
            return result;
        }

        public static bool IsAttributeNameContext(this SyntaxTree syntaxTree, int position, CancellationToken cancellationToken)
        {
            var result = _isAttributeNameContextMethod(syntaxTree, position, cancellationToken);
            return result;
        }

        public static SyntaxToken FindTokenOnLeftOfPosition(
            this SyntaxTree syntaxTree,
            int position,
            CancellationToken cancellationToken,
            bool includeSkipped = true,
            bool includeDirectives = false,
            bool includeDocumentationComments = false)
        {
            var result = _findTokenOnLeftOfPositionMethod(syntaxTree, position, cancellationToken, includeSkipped, includeDirectives, includeDocumentationComments);
            return result;
        }
    }
}
