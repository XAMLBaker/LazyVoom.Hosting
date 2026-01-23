using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LazyVoom.Hosting.Core;

namespace LazyVoom.Hosting.Winform;

public class WinFormsApp : VoomApp
{
    /// <summary>MainForm과 앱 전체 컨텍스트</summary>
    public static ApplicationContext? Context { get; private set; }

    /// <summary>MainForm 타입</summary>
    private Type MainFormType { get; }
    public WinFormsApp(IHost host, Type formType)
        :base(host)
    {
        MainFormType = formType;
    }

    /// <summary>WinForms 앱 실행</summary>
    public async Task Run()
    {
        var provider = Host.Services;
        ConfigureWinFormsEnvironment ();
        // MainForm은 싱글톤으로 root provider에서 가져오기
        var mainForm = (Form)Host.Services.GetRequiredService (MainFormType);

        // ApplicationContext에 MainForm 등록 후 실행
        Context = new ApplicationContext (mainForm);

        // 비동기 호스트 시작 (UI 스레드 블로킹하지 않음)
        _ = HostLifecycle.StartHostAsync (Host, provider, OnStartUpAsync);

        // MainForm 종료 시 Host 정리
        mainForm.FormClosed += (s, e) =>
        {
            // 공통 종료 처리 사용
            HostLifecycle.StopHostAndRunExit (Host, provider, OnExitAsync);
        };
        Application.Run (Context);
    }

    /// <summary>WinForms 환경 설정 (STA, HighDPI, VisualStyles)</summary>
    private static void ConfigureWinFormsEnvironment()
    {
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
