using LinqUI.WPF;
using LinqUI.WPF.HotReload;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

HotReloadManager.Enable ();

var builder = Host.CreateApplicationBuilder ();
var app = builder.BuildApp<App,MainWindow> ();  // 🔥

app.Run ();


public class App : Application { }
public class MainWindow : Window, IRender
{
    public MainWindow()
    {
        this.Render ();
    }
    public void Render()
    {
        this.Content = new Grid ()
                            .Columns ("*, *, *")
                            .Children(
                                new Button()
                                    .Content("핫리로드!")
                                    .Foreground(Colors.White)
                                    .Background(Colors.Green),
                                new Button ()
                                    .Content ("둘째!!")
                                    .Column(1),
                                new Button ()
                                    .Content ("셋째!")
                                    .Column (2)
                            );
    }
}