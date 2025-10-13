using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows.Forms;

namespace LazyVoom.Hosting.Winform;

public class WinFormsHost
{
    /// <summary>앱 시작 시점 비동기 초기화 콜백</summary>
    public Func<IServiceProvider, Task>? OnStartUpAsync { get; set; }

    // Exit 시 실행할 비동기 작업
    public Func<IServiceProvider, Task>? OnExitAsync
    {
        get; set;
    }

    /// <summary>MainForm과 앱 전체 컨텍스트</summary>
    public ApplicationContext? Context { get; private set; }

    /// <summary>DI Host</summary>
    public IHost Host { get; }

    /// <summary>MainForm 타입</summary>
    private Type MainFormType { get; }
    public WinFormsHost(IHost host, Type formType)
    {
        Host = host;
        MainFormType = formType;
    }

    /// <summary>WinForms 앱 실행</summary>
    public async Task RunAsync()
    {
        var provider = Host.Services;
        ConfigureWinFormsEnvironment ();

        if (OnStartUpAsync != null)
            await OnStartUpAsync (provider);

        // 2️⃣ MainForm은 싱글톤으로 root provider에서 가져오기
        var mainForm = (Form)Host.Services.GetRequiredService (MainFormType);

        // 3️⃣ MainForm 종료 시 Host 정리
        mainForm.FormClosed += (s, e) =>
        {
            if (OnExitAsync != null)
            {
                try
                {
                    OnExitAsync (provider).GetAwaiter ().GetResult ();
                }
                catch (Exception ex)
                {
                    Console.WriteLine ($"[ERROR] OnExitAsync: {ex}");
                }
            }
            Host.Dispose ();
        };

        // 4️⃣ ApplicationContext에 MainForm 등록 후 실행
        Context = new ApplicationContext (mainForm);
        Application.Run (Context);
    }

    /// <summary>WinForms 환경 설정 (STA, HighDPI, VisualStyles)</summary>
    private static void ConfigureWinFormsEnvironment()
    {
        if (Thread.CurrentThread.GetApartmentState () != ApartmentState.STA)
        {
            Thread.CurrentThread.SetApartmentState (ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState (ApartmentState.STA);
        }

        SetHighDpiMode ();

        Application.OleRequired ();
        Application.EnableVisualStyles ();
        Application.SetCompatibleTextRenderingDefault (false);
    }

    /// <summary>High DPI 모드 설정</summary>
    private static void SetHighDpiMode()
    {
        try
        {
            var os = Environment.OSVersion.Version;
            if (os.Major >= 10 && os.Build >= 15063)
                Application.SetHighDpiMode (HighDpiMode.PerMonitorV2);
            else
                Application.SetHighDpiMode (HighDpiMode.SystemAware);
        }
        catch
        {
            Application.SetHighDpiMode (HighDpiMode.SystemAware);
        }
    }
}
