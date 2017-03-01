Hexasoft.BasicAuthentication
===============

Hexasoft.BasicAuthentication is a Http Module that adds Basic Authentication to your ASP.NET web application. Ideal for securing WebApi projects deployed as Azure Web Apps behind an Azure API Management.

See: [http://stackoverflow.com/questions/33975280/simplest-way-to-add-basic-authentication-to-web-config-with-user-pass](http://stackoverflow.com/questions/33975280/simplest-way-to-add-basic-authentication-to-web-config-with-user-pass)

Inspired by [Devbridge.BasicAuthentication](https://github.com/devbridge/AzurePowerTools/tree/master/Devbridge.BasicAuthentication).


Download
--------
Hexasoft.BasicAuthentication is available as a NuGet package at [https://www.nuget.org/packages/Hexasoft.BasicAuthentication](https://www.nuget.org/packages/Hexasoft.BasicAuthentication)


Usage
-----
After installing the package you will see 3 new settings in the `appSettings` section of your `Web.config`:

    <add key="BasicAuthentication.Required" value="true" />
    <add key="BasicAuthentication.Username" value="testuser" />
    <add key="BasicAuthentication.Password" value="testpass" />

Use the `BasicAuthentication.Required` to quickly turn the authentication on or off while the username/password settings are self explanatory. Username is case-insensitive, password is case-sensitive.

As an alternative to securing the entire site, you can specify a regular expression to match against the URL Path by replacing the `"BasicAuthentication.Required"` appSetting with something like the following:

    <add key="BasicAuthentication.RequiredOnPathRegex" value="^\/my-protected-path" />
    <add key="BasicAuthentication.RequiredOnPathRegex.IgnoreCase" value="true" />

As this module was meant to secure WebApi's behind an Azure API Management, it only supports one username/password combination. No support for multiple users,


Version history
---------------

- 1.0.0 Initial public release
