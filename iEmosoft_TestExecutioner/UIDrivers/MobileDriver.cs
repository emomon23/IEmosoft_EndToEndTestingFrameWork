using aUI.Automation.HelperObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;

namespace aUI.Automation.UIDrivers
{
    public class MobileDriver : BrowserDriver
    {
        readonly AppiumLocalService Local = null;
        readonly BrowserDriverEnumeration BrowserVendor = BrowserDriverEnumeration.Android;
        public MobileDriver(IAutomationConfiguration configuration, BrowserDriverEnumeration browserVendor = BrowserDriverEnumeration.Android) : base(configuration, browserVendor)
        {
            BrowserVendor = browserVendor;
            var options = ConfigBuilder();

            var appiumServer = Config.GetConfigSetting("RemoteServer", "");
            var local = string.IsNullOrEmpty(appiumServer);
            if(browserVendor == BrowserDriverEnumeration.AndroidRemote)
            {
                var uri = Config.GetConfigSetting("SeleniumHubUrl");

                var capabilities = new DesiredCapabilities();
                capabilities.SetCapability(CapabilityType.BrowserName, "android");
                capabilities.SetCapability(CapabilityType.BrowserVersion, "10");
                capabilities.SetCapability(CapabilityType.Timeouts, 120);
                capabilities.SetCapability("newCommandTimeout", "120000");
                capabilities.SetCapability("screenResolution", "720x1280");
                capabilities.SetCapability("skin", "720x1280");


                options.AddAdditionalCapability(MobileCapabilityType.BrowserName, "android");
                options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "10.0");
                options.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, "120");
                options.AddAdditionalCapability("waitDuration", 120);


                RawWebDriver = new RemoteWebDriver(new Uri(uri), options);// capabilities, new TimeSpan(0,2,0)); //options);
            }
            else if (local)
            {
                Local = new AppiumServiceBuilder().UsingAnyFreePort().Build();
                Local.Start();

                switch (browserVendor)
                {
                    case BrowserDriverEnumeration.Windows:
                        RawWebDriver = new WindowsDriver<IWebElement>(Local, options);
                        break;
                    case BrowserDriverEnumeration.Android:
                        RawWebDriver = new AndroidDriver<IWebElement>(Local, options);
                        break;
                    case BrowserDriverEnumeration.IOS:
                        RawWebDriver = new IOSDriver<IWebElement>(Local, options);
                        break;
                }
            }
            else
            {
                var uri = new Uri(appiumServer);
                switch (browserVendor)
                {
                    case BrowserDriverEnumeration.Windows:
                        RawWebDriver = new WindowsDriver<IWebElement>(uri, options);
                        break;
                    case BrowserDriverEnumeration.Android:
                        RawWebDriver = new AndroidDriver<IWebElement>(uri, options);
                        break;
                    case BrowserDriverEnumeration.IOS:
                        RawWebDriver = new IOSDriver<IWebElement>(uri, options);
                        break;
                }
            }
        }

        private AppiumOptions ConfigBuilder()
        {
            var ops = new AppiumOptions();
            
            ops.AddAdditionalCapability(MobileCapabilityType.DeviceName, Config.GetConfigSetting("DeviceName", ""));
            ops.AddAdditionalCapability(MobileCapabilityType.PlatformName, Config.GetConfigSetting("PlatformName", ""));
            ops.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, Config.GetConfigSetting("PlatformVersion", ""));
            ops.AddAdditionalCapability(MobileCapabilityType.App, Config.GetConfigSetting("App", "")); //TODO update for pulling path dynamically
            ops.AddAdditionalCapability(MobileCapabilityType.BrowserName, Config.GetConfigSetting("BrowserName", ""));
            ops.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, "120");//unsure if this is enough/too much
            ops.AddAdditionalCapability(MobileCapabilityType.Orientation, Config.GetConfigSetting("Orientation", "PORTRAIT"));
            //build appium config
            /* 
             * language
             * locale
             * udid
             */
            return ops;
        }

        public override void Dispose()
        {
            try
            {
                ((AndroidDriver<IWebElement>)RawWebDriver).CloseApp();
            }
            catch {
                ((AndroidDriver<IWebElement>)RawWebDriver).Close();
            }
            if(Local != null)
            {
                Local.Dispose();
            }
            RawWebDriver.Dispose();
        }
    }
}
