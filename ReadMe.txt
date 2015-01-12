There are two pieces to this solution.  The main focus is the end to end testing framework using Selenium, Excel, and windows GDI api
to capture the screen to an image file (see the TestExecutioner.png image for a high level overview).

In order to demo the testing framework, we need something to test!  For this I've created the Patient Management System (PMS).  
An Angular website to manage hospitals, physicians, and patients. 

Begin:

- You must HAVE FIREFOX INSTALLED on the PC these tests will run.

- Create a folder on C: called iEmoSoft and clone this repository to that folder.

- Create a web application on your machine and point it to C:\iEmosoft\IEmosoft_EndToEndTestingFrameWork\PMS.  Verify
this site is running by going to http://localhost/PMS/default.html.  (You can enter any username and password to login.
If the password contains the string 'invalid', anywhere in the string, than it will be treated as an invalid username / password)

- Run visual studio as an administrator and open the C:\iEmosoft\IEmosoft_EndToEndTestingFrameWork\iEmosoft_TestExecutioner.sln solution

- Build the solution, then select 'Test -> Windows -> Text Explorer'

- Run all the tests (2 should pass, 1 should fail)

- See C:\PatientMgmtSystemTestsResults for the artifacts that were created.


