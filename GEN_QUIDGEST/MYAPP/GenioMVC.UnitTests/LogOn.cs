using GenioMVC.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.UnitTests;

public class LogOn : NoUserTestFixture
{


    [Test]
    public void LogOnGet()
    {
        AccountController controller = new AccountController(_userContextService);

        var result = controller.LogOn();

        JsonUtils.AssertJsonHasValue("Success", result, true);
    }


       
    [Test] 
    public void InvalidModelFails()
    {
        AccountController controller = new AccountController(_userContextService);
        var model = new GenioMVC.Models.LogOnModel();
        //Set an error in the model state
        controller.ModelState.AddModelError("ERROR", "Invalid Model");           
            
        //Act
        var result = controller.LogOn(model, "");

        //This returning structure should be refactored asap! The returning structure is something like:
        //    Success = true, Data = { Success = false, Errors = Count = 1 }, Errors = Count = 1,
        Assert.IsInstanceOf<JsonResult>(result);
        var json = result as JsonResult;
        Assert.IsNotNull(json);
        var success = JsonUtils.GetPropertyObject("Success", json.Value);
        Assert.That(success, Is.EqualTo(false));
    }
}