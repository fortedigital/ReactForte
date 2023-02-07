# ForteReact

Library to render React library components on the server-side with C# as well as on the client.
ForteReact has two options to render components:
- ReactJS.NET https://reactjs.net/
- Wrapper for Javascript.NodeJS https://github.com/JeringTech/Javascript.NodeJS

## How to run

### 1. Add WebpackOptions to appsettings

#### For Development 

  "Webpack": {
    "OutputPath": "http://localhost:8080"
  }

#### For Production
  "Webpack": {
    "OutputPath": "./path/to/output"
  }

### 2. Modify Startup.cs

#### For ReactJS.NET
```
services.AddReact(_configuration, _webHostingEnvironment.ContentRootPath);
-------------
app.UseReact(config =>
    {
        ...       
    });
			
```
More info https://reactjs.net/tutorials/aspnetcore.html

#### For Wrapper 

```services.AddReact(_configuration, _webHostingEnvironment.ContentRootPath, options => {...})
-------------
app.UseReact(FilesToInclude, new Version("x.x.x"));
```
