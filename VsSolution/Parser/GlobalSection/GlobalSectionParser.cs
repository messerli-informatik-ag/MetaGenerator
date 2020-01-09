﻿using System;
using System.Collections.Generic;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    public class GlobalSectionParser
    {
        private readonly HashSet<string> _possibleLoadingOrders = new HashSet<string> { "preSolution", "postSolution" };

        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            tokenWalker.Consume<BeginGlobalSectionToken>();
            tokenWalker.Consume<OpenParenthesisToken>();
            var sectionType = Enum.Parse<GlobalSectionType>(tokenWalker.ConsumeWord());
            tokenWalker.Consume<ClosedParenthesisToken>();

            tokenWalker.ConsumeAllWhiteSpace();
            tokenWalker.Consume<AssignToken>();

            tokenWalker.ConsumeAllWhiteSpace();
            CheckLoadingOrder(tokenWalker.ConsumeWord());

            ParseConfigurations(sectionType, tokenWalker, solution);

            tokenWalker.Consume<EndGlobalSectionToken>();
            tokenWalker.ConsumeAllWhiteSpace();
        }

        private void CheckLoadingOrder(string loadingOrder)
        {
            if (_possibleLoadingOrders.Contains(loadingOrder) == false)
            {
                throw new ParseException($"Unknown loading Order '{loadingOrder}'");
            }
        }

        private void ParseConfigurations(GlobalSectionType sectionType, TokenWalker tokenWalker, Solution solution)
        {
            var globalSection = GlobalSectionTypeFactory.Create(sectionType);

            globalSection.Parse(tokenWalker, solution);
        }
    }
}