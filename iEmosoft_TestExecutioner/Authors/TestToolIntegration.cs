using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aUI.Automation.Authors
{
    class TestToolIntegration
    {
        //file all api calls through the same method for throttling as needed
            //use var to manage the throttling time
            //look at pulling the 'level of parallelism' to dynamically set this value
            //possibly look at having a timer/counter/var to track the last time an endpoint was hit to delay only when needed

        //get list of test case names and ids from tool
            //Create an object to hold this data, other 'tag' data, current test steps, and possibly the new test steps

        //If new test, create the new test
            //do this in bunches if possible

        //Check if test needs to be updated
            //do this in bunches if possible

        //Update test results periodically during run
            //somehow check for 'low' times to make this call?

        //TestExecutioner needs to be able to send data in
            //ensure a thread-safe way of doing this


    }
}
