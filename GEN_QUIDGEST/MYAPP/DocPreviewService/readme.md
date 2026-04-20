# Description
This service provides conversion of many document formats to pdf.

# Install
The service must be installed in a IIS server with Asp.Net 4.8 capabilities.

Create a custom Application Pool to host this service:
* v4.0 CLR
* Integrated
* Change Identity to a local machine user with Office access permissions

Create a Application for the service:
* Choose the custom application pool
* In the windows filesystem give all file permissions to the Identity user
* Ensure there is a `temp` folder in the application directory

The service requires Microsoft Office to be installed locally on the same machine.

Due to the heavy dependency on DCOM interfaces its highly recomended to separate this service into and isolated virtual machine.

Configure DCOM permission using the 32bit console in administrator mode:

`mmc comexp.msc /32`

* Navigate to the `My Computer` node and right click to choose `Properties` option.
* In the tab `COM Security` add the user with Office permissions
	* In group `Access Permissions` press button `Edit Default...`
	* Click `Add...`
	* Select AD or Local computer in the `Locations...` button according to the user needed
	* Write the user name and validate it with `Check Names`
	* Press `Ok`
	* Mark the checkboxes for the permissions of `Local Access`
	* Repeat the steps for group `Launch and Activation Permissions` with button `Edit Default...`
	* Mark the checkboxes for the permissions of `Local Launch` and `Local Activation`

# Post install operations

Office interop is very interactive user dependent. There are a lot of variables that can go wrong during office instalation:

* Use Office Deployment Tool to install Office whenever possible to make instalation more predictable
* Ensure the following folders are created in the operating system:
  * C:\Windows\SysWOW64\config\systemprofile\Desktop
  * C:\Windows\System32\config\systemprofile\Desktop
* Before using the service make you the user can login into windows, open a document and convert it to pdf. If this operation is not performed at least once by the user then automation may not be able to run without errors.

# Usage

To convert a file call the `/Convert` uri relative to your base url, and use a `form-data` type body to attach a file to a POST request. 
The return will be a converted PDF as a FileStream response in case of sucess. 
In case of an error an Http status error will be return with the error message in the body formated as Text.

To invoke the service programatically from CSharp here is an example:
``` CSharp
public async Task<string> CallConvertPdf(string baseUrl, string filename, Stream inStream, Stream outStream)
{
    var http = new HttpClient();

    MultipartFormDataContent form = new MultipartFormDataContent();
    var streamContent = new StreamContent(inStream);
    form.Add(streamContent, "file", Path.GetFileName(filename));

    var resp = await http.PostAsync(new Uri(baseUrl + "/Convert"), form);

    if (!resp.IsSuccessStatusCode)
        return "";

    await resp.Content.CopyToAsync(outStream);

    if(resp.Content.Headers.TryGetValues("Content-Disposition", out var values))
    {
        Match match = new Regex(@"filename=""?([\w\.]*)""?").Match(values.First());
        if(match.Success)
            return match.Groups[1].Value;
    }
    return "preview.pdf";
}
```

# Notes

This service is designed as an internal process only. It has no built in security or authentication features of its own. **Do not expose it to public internet.**

Docker integration is not working at the moment until a base image with Microsoft Office properly configured can be found. Deploying to the cloud requires a full virtual machine at the moment.

The temp folder serves as a caching mechanism for the resource heavy conversion process. While there is a builtin cleanup mechanism, do ensure enough free disk space exists and is maintained.

Office automation is very error prone and does not have any support from Microsoft. Hanged processes may be left orphaned in the windows tasks. This and other cleanup tasks may be necessary periodically.