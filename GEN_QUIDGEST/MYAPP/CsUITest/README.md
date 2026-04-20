## Running tests

You can run the tests by opening the project in Visual Studio 2022 and executing __"Run all tests"__ or, having .Net 8 installed, on a command line:
```
dotnet test
```
The tests will establish a browser session to the configured Url. The framework does __not__ start the web server, the person testing is responsible for that. The web server should be started manually or by Continuous Integration automation pipeline. E2E tests assume the server is fully configured and its environment is complete with other dependency services like databases.

## Configuration

The configuration file `SeleniumWebTest.json.example` is located at the root of the test project. Rename this file to `SeleniumWebTest.json` before use.

Configuration properties:

* Browser - _(chrome, firefox, edge)_ The browser engine used to simulate the web session.
* BaseUrl - The base Url to the home page of the running web application.
* Headless - _(true, false)_ True if the browser window should not be visible on screen.
* ImplicitWait - Number of miliseconds that selenium will wait at most when trying to find elements in a page, in case the page is still loading or javascript is still updating the DOM.
* ExplicitWait - Number of miliseconds that selenium will wait at most during an explicit wait.Until().

## Directory structure
```
project           - Solutions files and configuration
├───controls      - Component elements
├───core          - Drivers and factory
├───pages
│   ├───common    - Layout and reusable pages
│   └───forms     - Generated forms
└───tests         - Generated test battery
```

The API is structured into pages and controls. Each page represents an entire dialog in the web application that the user navigates to. Each page will make available a property for each component it contains. These properties will in turn return the corresponding control object.

Both pages and controls have actions that can be done to them. These are represented as methods of these objects. Some actions can cause the application to navigate as they are invoked. It up to the tester to assert the expected page explicitly by alocating a new page of the corresponding type. This helps to validate that the flow of the test replicates the expected flow of the application.

The user created test need to be placed in the test directory within a [NUnit](https://nunit.org/) test class. A superclass `BaseSeleniumTest` is provided that already has all the driver initialization logic.

## Generation

The test solution is generated together with the MVC and VueJs targets. To add tests to this solution you will need to add manual code with the contents of the test (in the future there will be a test modeling language). The tag to use in MANMVC is:

```
/[MANUAL SIS UITEST testname]/
```

Each section will create a new test class file with a single test method in it. Your manual code should be constituted only by the inside of that method.

If for some reason the test requires some other namespace you also have available a tag to add using clauses to that test:

```
/[MANUAL SIS UITESTIMPORTS testname]/
```

Since many tests will benefit from reusing certain sequences steps, like for example login, going to a certain form, etc, the project is prepared for class files placed in the directory automatically being taken into account. NUnit has many useful features like parametrized tests, setup and teardown phases, that can be critical in keeping tests maintainable.

## Test examples

Authenticate into the application:

```csharp
var a = new AppPage(Driver);
a.ClickLogin();

var p = new LoginPage(Driver);
p.Login("quidgest", "zph2lab");

Assert.That(a.IsAuthenticated());
```

Go into a menu, search a row, then click it:

```csharp
a.Menu.ActivateModule(Module.GQT);
a.Menu.ActivateMenu(Module.GQT, "291");

var list = new MenuListPage(Driver, Module.GQT, "2911").List;
list.Search.Search("Tools");
Assert.That(list.GetValue(0, "tpequ.tipoequi"), Is.EqualTo("Tools"));

list.ClickRow(0);
```

Set values and verify them in a form, then save it:

```csharp
var form = new TpequForm(Driver, FORM_MODE.EDIT);

//Lookup
form.FamilFamily.SetValue("Tools");
Assert.That(form.FamilFamily.GetValue(), Is.EqualTo("Tools"));

//checkbox
Assert.That(form.TpequKit.GetValue(), Is.False);
form.TpequKit.Toggle();
Assert.That(form.TpequKit.GetValue(), Is.True);

//dates
DateTime now = DateTime.Now;
form.CamdateFldsDate.SetValue(now); //Date
var y = form.CamdateFldsDate.GetValue();
Assert.That(y, Is.EqualTo(now.Date));

form.CamdateFldsDatetime.SetValue(now); //DateTime
y = form.CamdateFldsDatetime.GetValue();
Assert.That(y, Is.EqualTo(now.Date.AddMinutes(Math.Floor(now.TimeOfDay.TotalMinutes))));

form.CamdateFldsDateseco.SetValue(now); //DateTimeSeconds
y = form.CamdateFldsDateseco.GetValue();
Assert.That(y, Is.EqualTo(now.Date.AddSeconds(Math.Floor(now.TimeOfDay.TotalSeconds))));

form.Save();
```

Set a lookup through its SeeMore dialog:

```csharp
var form = new TpequForm(Driver, FORM_MODE.EDIT);

form.FamilFamily.SeeMore();
var sml = form.FamilFamilySeeMorePage.List;
sml.Search.Search("Tools");
sml.ClickRow(0);
Assert.That(form.FamilFamily.GetValue(), Is.EqualTo("Tools"));
```

Canceling a dirty form through the confirmation popup:

```csharp
var form = new TpequForm(Driver, FORM_MODE.EDIT);
form.TpequTpequcod.SetValue("00");
form.Cancel();
var p = new ConfirmationPopup(Driver);
p.Confirm();
```

Editing data through a Grid control:

```csharp
var form = new GrpbForm(Driver, FORM_MODE.EDIT);
var grid = form.PseudTblb;
grid.SetCurrentRow(0);
grid.TblbText.SetValue("xpto");
grid.SetCurrentRow(1);
grid.DeleteRow();
grid.SetInsertRow();
grid.TblbText.SetValue("new record");
```