﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Atata
{
    public class ByChain : By
    {
        public ByChain(params By[] items)
            : this(items as IEnumerable<By>)
        {
        }

        public ByChain(IEnumerable<By> items)
        {
            Items = items.ToList().AsReadOnly();
            Description = $"By.Chain([{string.Join(", ", Items)}])";
        }

        public ReadOnlyCollection<By> Items { get; }

        public override IWebElement FindElement(ISearchContext context)
        {
            ReadOnlyCollection<IWebElement> elements = FindElements(context);

            return elements.FirstOrDefault()
                ?? throw ExceptionFactory.CreateForNoSuchElement(by: this, searchContext: context);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            if (!Items.Any())
                return new List<IWebElement>().AsReadOnly();

            List<IWebElement> elements = Items.First().FindElements(context).ToList();

            foreach (By by in Items.Skip(1))
            {
                elements = elements.SelectMany(x => x.FindElements(by)).ToList();
            }

            return elements.AsReadOnly();
        }
    }
}
