using aUI.Automation.HelperObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aUI.Automation.Elements
{
    public enum ElementAction
    {
        //active
        Click,
        EnterText,
        Dropdown,
        DropdownIndex,
        RadioBtn,
        MultiDropdown,
        Hover,

        //passive
        GetText,
        GetCheckbox,
        GetAttribute,
        GetCSS,
        GetProperty,
    }

    public enum ElementType
    {
        Id,
        Class,
        Name,
        Xpath,
        CSS,
        LinkText,
        Tag,
        PartialLinkText,
    }

    public enum Wait
    {
        Visible,
        Clickable,
        Selected,
        Invisible,
        ContainsText,
        Custom,
    }

    public class ElementActions
    {
        private TestExecutioner TE;
        private IWebDriver Driver;
        //TODO add & improve assertions at this level
        //this will require a class for customized assertions to track

        public ElementActions(TestExecutioner tE)
        {
            TE = tE ?? throw new ArgumentNullException(nameof(tE));
            Driver = TE.RawSeleniumWebDriver_AvoidCallingDirectly;

        }

        //general method to handle actions

        //general method to handle action on existing elements

        //helper method to complete the work/actions



        //ensure good assertions

        //https://stackoverflow.com/questions/2082615/pass-method-as-parameter-using-c-sharp

        public ElementResult ExecuteAction(ElementObject ele, ElementResult starter = null)
        {
            switch (ele.Action)
            {
                case ElementAction.Click:
                case ElementAction.EnterText:
                case ElementAction.Dropdown:
                case ElementAction.DropdownIndex:
                case ElementAction.RadioBtn:
                case ElementAction.MultiDropdown:
                case ElementAction.Hover:
                    TE.BeginTestCaseStep($"Execute action {ele.Action} on element: {ele.ElementName}",
                        ele.Random || ele.ProtectedValue ? "Random Value" : ele.Text);
                    break;
            }

            var element = starter;
            //check if 'ele' has an element in it or not.
            if (!string.IsNullOrEmpty(ele.EleRef))
            {
                var finder = ElementFinder(ele);
                element = FindElement(ele, finder, starter?.RawEle);
            }

            return CompleteAction(ele, element);
        }

        public List<ElementResult> ExecuteActions(ElementObject ele, ElementResult starter = null)
        {
            switch (ele.Action)
            {
                case ElementAction.Click:
                case ElementAction.EnterText:
                case ElementAction.Dropdown:
                case ElementAction.DropdownIndex:
                case ElementAction.RadioBtn:
                case ElementAction.MultiDropdown:
                case ElementAction.Hover:
                    TE.BeginTestCaseStep($"Execute action {ele.Action} on elements: {ele.ElementName}",
                        ele.Random || ele.ProtectedValue ? "Random Value" : ele.Text);
                    break;
            }

            var finder = ElementFinder(ele);
            var elements = FindElements(ele, finder, starter?.RawEle);

            elements.ForEach(x => CompleteAction(ele, x));
            return elements;
        }

        private ElementResult CompleteAction(ElementObject eleObj, ElementResult eleRes)
        {
            SelectElement select;
            var ele = eleRes.RawEle;
            var rsp = new ElementResult(TE) { Success = false, RawEle = ele };

            if (ele == null)
            {
                return rsp;
            }

            //TODO Implement 'random' for all other values beyond dropdown

            try
            {
                if (eleObj.Scroll)
                {
                    (((IWrapsDriver)ele).WrappedDriver as IJavaScriptExecutor).ScrollToElement(ele);
                }

                switch (eleObj.Action)
                {
                    case ElementAction.Click:
                        ele.Click();
                        break;
                    case ElementAction.Hover:
                        var act = new Actions(Driver);
                        act.MoveToElement(ele).Build().Perform();
                        TE.Pause(150);//default pause for hover to take effect
                        break;
                    case ElementAction.EnterText:
                        if (eleObj.Clear)
                        {
                            ele.Clear();
                        }
                        ele.SendKeys(eleObj.Text);
                        break;
                    case ElementAction.Dropdown:
                        if (eleObj.Random)
                        {
                            eleObj.Action = ElementAction.DropdownIndex;
                            return CompleteAction(eleObj, eleRes);
                        }

                        select = new SelectElement(ele);
                        if (select.IsMultiple && eleObj.Clear) 
                        {
                            select.DeselectAll();
                        }
                        rsp.Text = SetDropdown(select, eleObj.Text);
                        break;
                    case ElementAction.DropdownIndex:
                        select = new SelectElement(ele);
                        if (select.IsMultiple && eleObj.Clear)
                        {
                            select.DeselectAll(); 
                        }

                        var index = TE.Rand.Rnd.Next(0, select.Options.Count);
                        if (int.Parse(eleObj.Text) >= 0 || !eleObj.Random)
                        {
                            index = int.Parse(eleObj.Text);
                        }
                        select.SelectByIndex(index);
                        rsp.Text = select.Options[index].Text;
                        break;
                    case ElementAction.MultiDropdown:
                        select = new SelectElement(ele);
                        if (select.IsMultiple && eleObj.Clear)
                        {
                            select.DeselectAll();
                        }

                        foreach (var option in eleObj.Text.Split('|'))
                        {
                            SetDropdown(select, option);
                        }
                        break;
                    case ElementAction.RadioBtn:
                        if (!eleObj.Text.ToLower().Equals(ele.Selected.ToString().ToLower()))
                        {
                            ele.Click();
                        }
                        break;
                    case ElementAction.GetCheckbox:
                        rsp.Text = ele.Selected.ToString();
                        break;
                    case ElementAction.GetText:
                        rsp.Text = ele.Text;
                        break;
                    case ElementAction.GetAttribute:
                        rsp.Text = ele.GetAttribute(eleObj.Text);
                        break;
                    case ElementAction.GetCSS:
                        rsp.Text = ele.GetCssValue(eleObj.Text);
                        break;
                    case ElementAction.GetProperty:
                        rsp.Text = ele.GetProperty(eleObj.Text);
                        break;
                    default:
                        throw new NotImplementedException("This action has not been implemented. Please implement it.");
                }
            }
            catch (Exception)
            {
                rsp.Success = false;
                return rsp;
            }

            return rsp;
        }

        private string SetDropdown(SelectElement select, string desired)
        {
            var optionList = new List<string>();
            foreach (var option in select.Options)
            {
                optionList.Add(option.Text);
            }

            var found = optionList.Contains(desired);
            if (!found)
            {
                desired = optionList.FirstOrDefault(x => x.Contains(desired));
                found = !string.IsNullOrEmpty(desired);
            }

            if (found)
            {
                select.SelectByText(desired);
            }
            else
            {
                select.SelectByValue(desired);
            }

            return desired;
        }

        private By ElementFinder(ElementObject ele)
        {
            return ele.EleType switch
            {
                ElementType.Id => By.Id(ele.EleRef),
                ElementType.Class => By.ClassName(ele.EleRef),
                ElementType.Name => By.Name(ele.EleRef),
                ElementType.Xpath => By.XPath(ele.EleRef),
                ElementType.CSS => By.CssSelector(ele.EleRef),
                ElementType.LinkText => By.LinkText(ele.EleRef),
                ElementType.Tag => By.TagName(ele.EleRef),
                ElementType.PartialLinkText => By.PartialLinkText(ele.EleRef),
                _ => null,
            };
        }

        private ElementResult FindElement(ElementObject eleRef, By by, IWebElement starter = null)
        {
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, eleRef.MaxWait));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException),
                typeof(ElementNotInteractableException));

            IWebElement element = null;

            var sucess = wait.Until(condition =>
            {
                try
                {
                    if (starter == null)
                    {
                        //TODO Enable custom filter for elements from other elements
                        if (eleRef.WaitType == Wait.Custom)
                        {
                            if (eleRef.CustomCondition != null)
                            {
                                var rsp = eleRef.CustomCondition(Driver, by);
                                element = rsp.Item1;
                                return rsp.Item2;
                            }
                        }

                        element = Driver.FindElement(by);
                    }
                    else
                    {
                        element = starter.FindElement(by);
                    }

                    return eleRef.WaitType switch
                    {
                        Wait.Clickable => element.Displayed && element.Enabled,
                        Wait.Visible => element.Displayed,
                        Wait.Selected => element.Selected,
                        Wait.Invisible => !element.Displayed,
                        Wait.ContainsText => element.Text.Contains(eleRef.Text),
                        _ => true,
                    };
                }
                catch (Exception)
                {
                    return eleRef.WaitType == Wait.Invisible;
                }
            });

            return new ElementResult(TE) { RawEle = element, Success = sucess };
        }

        private List<ElementResult> FindElements(ElementObject eleRef, By by, IWebElement starter = null)
        {
            var retur = new List<ElementResult>();
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, eleRef.MaxWait));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException),
                typeof(ElementNotInteractableException));

            List<IWebElement> elements = null;

            var sucess = wait.Until(condition =>
            {
                try
                {
                    if (starter == null)
                    {
                        if (eleRef.WaitType == Wait.Custom)
                        {
                            if (eleRef.CustomConditionMulti != null)
                            {
                                var rsp = eleRef.CustomConditionMulti(Driver, by);
                                elements = rsp.Item1;
                                return rsp.Item2;
                            }
                        }

                        elements = Driver.FindElements(by).ToList();
                    }
                    else
                    {
                        elements = starter.FindElements(by).ToList();
                    }

                    var pass = true;

                    foreach (var element in elements)
                    {
                        var val = eleRef.WaitType switch
                        {
                            Wait.Clickable => element.Displayed && element.Enabled,
                            Wait.Visible => element.Displayed,
                            Wait.Selected => element.Selected,
                            Wait.Invisible => !element.Displayed,
                            Wait.ContainsText => element.Text.Contains(eleRef.Text),
                            _ => true,
                        };

                        if (!val)
                        {
                            pass = false;
                        }
                    }

                    return pass;
                }
                catch (Exception)
                {
                    return eleRef.WaitType == Wait.Invisible;
                }
            });

            elements.ForEach(x => retur.Add(new ElementResult(TE) { RawEle = x, Success = sucess }));

            return retur;
        }
    }

    public static class ElementActionExtender
    {
        public static ElementResult ExecuteAction(this ElementResult elementRef, ElementObject ele)
        {
            var ea = new ElementActions(elementRef.TE);
            return ea.ExecuteAction(ele, elementRef);
        }

        public static List<ElementResult> ExecuteActions(this ElementResult elementRef, ElementObject ele)
        {
            var ea = new ElementActions(elementRef.TE);
            return ea.ExecuteActions(ele, elementRef);
        }

        #region Single Element Actions
        public static ElementResult Click(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.Click;
            return elementRef.ExecuteAction(ele);
        }
        public static ElementResult Hover(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.Hover;
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult EnterText(this ElementResult elementRef, string text)
        {
            var ele = new ElementObject { Action = ElementAction.EnterText, Text = text };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult EnterText(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.EnterText;
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult SelectDropdown(this ElementResult elementRef, string option)
        {
            var ele = new ElementObject { Action = ElementAction.Dropdown, Text = option };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult SelectDropdown(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.Dropdown;
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult MultiDropdown(this ElementResult elementRef, string option)
        {
            var ele = new ElementObject { Action = ElementAction.MultiDropdown, Text = option };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult MultiDropdown(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.MultiDropdown;
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult DropdownIndex(this ElementResult elementRef, int index)
        {
            var ele = new ElementObject { Action = ElementAction.DropdownIndex, Text = index.ToString() };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult DropdownIndex(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.DropdownIndex;
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult RadioBtn(this ElementResult elementRef, bool selected = true)
        {
            var ele = new ElementObject { Action = ElementAction.RadioBtn, Text = selected.ToString() };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult RadioBtn(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.RadioBtn;
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult GetText(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetText };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult GetCheckbox(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetCheckbox };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult GetAttribute(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetAttribute };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult GetCSS(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetCSS };
            return elementRef.ExecuteAction(ele);
        }

        public static ElementResult GetProperty(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetProperty };
            return elementRef.ExecuteAction(ele);
        }
        #endregion

        #region Multi Element Actions

        public static List<ElementResult> ClickAll(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.Click;
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> EnterTexts(this ElementResult elementRef, string text)
        {
            var ele = new ElementObject { Action = ElementAction.EnterText, Text = text };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> EnterTexts(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.EnterText;
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> SelectDropdowns(this ElementResult elementRef, string option)
        {
            var ele = new ElementObject { Action = ElementAction.Dropdown, Text = option };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> SelectDropdowns(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.Dropdown;
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> MultiDropdowns(this ElementResult elementRef, string option)
        {
            var ele = new ElementObject { Action = ElementAction.MultiDropdown, Text = option };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> MultiDropdowns(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.MultiDropdown;
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> DropdownIndexes(this ElementResult elementRef, int index)
        {
            var ele = new ElementObject { Action = ElementAction.DropdownIndex, Text = index.ToString() };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> DropdownIndexes(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.DropdownIndex;
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> RadioBtns(this ElementResult elementRef, bool selected = true)
        {
            var ele = new ElementObject { Action = ElementAction.RadioBtn, Text = selected.ToString() };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> RadioBtns(this ElementResult elementRef, ElementObject ele = null)
        {
            ele.Action = ElementAction.RadioBtn;
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> GetTexts(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetText };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> GetCheckboxes(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetCheckbox };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> GetAttributes(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetAttribute };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> GetCSSs(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetCSS };
            return elementRef.ExecuteActions(ele);
        }

        public static List<ElementResult> GetProperties(this ElementResult elementRef)
        {
            var ele = new ElementObject { Action = ElementAction.GetProperty };
            return elementRef.ExecuteActions(ele);
        }
        #endregion
    }
}
