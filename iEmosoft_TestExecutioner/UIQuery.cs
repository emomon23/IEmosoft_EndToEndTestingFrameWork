namespace aUI.Automation
{
    public class UIQuery
    {
        public UIQuery(string elementId, string controlType = "input")
        {
            AttributeName = "id";
            AttributeValue = elementId;
            ControlTypeName = controlType;
        }

        public UIQuery(string controlTypeName, string attributeName, string attributeValue)
        {
            ControlTypeName = controlTypeName;
            AttributeValue = attributeValue;
            AttributeName = attributeName;
        }

        public UIQuery() { }

        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string ControlTypeName { get; set; }
    }
}
