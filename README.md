# LazyVoom.Hosting

게으른 개발자를 위한 Host 기반 앱 실행 라이브러리입니다.
WPF와 WinForms에서 쉽게 Host 기반으로 앱을 시작하고 DI/초기화 작업을 처리할 수 있습니다.

## 설치

템플릿을 먼저 설치해야 합니다:

**WinForms 용 템플릿**
```
dotnet new install LazyVoom.Hosting.Winform.Template
```

**WPF 용 템플릿**
```
dotnet new install LazyVoom.Hosting.WPF.Template
```

각각 설치 후 실행하면 바로 Host 기반 앱을 시작할 수 있습니다.


## WinForms 사용 예제

기본 실행:
```csharp
var builder = Host.CreateApplicationBuilder();
var app = builder.BuildApp();

app.OnStartUpAsync = async provider =>
{
    // DI 서비스 초기화 등
};

await app.RunAsync();
```

메인 폼이 Form1이 아닐 경우, 제네릭으로 지정 가능:
```csharp
var builder = Host.CreateApplicationBuilder();
var app = builder.BuildApp<Form2>();

app.OnStartUpAsync = async provider =>
{
    // DI 서비스 초기화 등
};

await app.RunAsync();
```
## WPF 사용 예제

기본 실행:
```csharp
var builder = Host.CreateApplicationBuilder();
var app = builder.BuildApp();  // App과 MainWindow 기본 사용

app.OnStartUpAsync = async provider =>
{
    // DI 서비스 초기화 등
};

app.Run();
```

App이나 MainWindow를 기본값이 아닌 다른 클래스로 지정하고 싶을 경우:

```csharp
var builder = Host.CreateApplicationBuilder();
var app = builder.BuildApp<App2, MainWindow2>();

app.OnStartUpAsync = async provider =>
{
    // DI 서비스 초기화 등
};

app.Run();
```

이런 식으로 Host 기반 실행 + DI 초기화 + 커스텀 App/MainWindow까지 깔끔하게 관리할 수 있습니다.