using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iEmosoft.Automation
{
    public class UIQuery
    {
        public UIQuery(string elementId, string controlType = "input")
        {
            this.AttributeName = "id";
            this.AttributeValue = elementId;
            this.ControlTypeName = controlType;
        }

        public UIQuery(string controlTypeName, string attributeName, string attributeValue)
        {
            this.ControlTypeName = controlTypeName;
            this.AttributeValue = attributeValue;
            this.AttributeName = attributeName;
        }

        public UIQuery() { }

        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string ControlTypeName { get; set; }
    }
}
