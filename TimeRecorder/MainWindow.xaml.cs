using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string path = "";
        static string update_path = "";

        public MainWindow()
        {

            InitializeComponent();

            StreamReader readerPath = new StreamReader("path.inf");
            path = readerPath.ReadLine();
            readerPath.Close();

            StreamReader readerUpdate = new StreamReader("update path.inf");
            update_path = readerUpdate.ReadLine();
            readerUpdate.Close();

            //10秒後カウントする
            System.Timers.Timer timer = new System.Timers.Timer(10000);
            timer.Start();
            timer.Elapsed += timer_Elapsed;

            Visibility = Visibility.Hidden;
        }

        //10秒後にログを残す
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();

            //更新プログラムの有無を確認
            Check_Update();

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("ログイン:" + DateTime.Now);
            writer.Close();


        }
        

        //ログオフ、シャットダウンしようとしているとき
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            string s = "";
            if (e.Reason == SessionEndReasons.Logoff)
            {
                s = "　ログオフ：" + DateTime.Now;
            }
            else if (e.Reason == SessionEndReasons.SystemShutdown)
            {
                s = "　ログオフ：" + DateTime.Now;
            }

            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(s);
            writer.Close();
        }

        private void Check_Update()
        {
            if (update_path != null)
            {
                FileInfo updateFile = new FileInfo(update_path);
                FileInfo currentFile = new FileInfo("TimeRecorder.exe");

                if (updateFile.Exists)
                    if (DateTime.Compare(updateFile.LastWriteTime, currentFile.LastWriteTime) != 0)
                    {
                        {
                            if (File.Exists("TimeRecorder.exe.old"))
                            {
                                File.Delete("TimeRecorder.exe.old");
                            }
                            currentFile.MoveTo("TimeRecorder.exe.old");
                            updateFile.CopyTo("TimeRecorder.exe");

                            StreamWriter writer = new StreamWriter("UpdateLog.txt", true);
                            writer.WriteLine(DateTime.Now + " アップデートしました。");
                            writer.Close();

                            MessageBox.Show("プログラムを更新しました", "タイムレコーダー");
                        }
                    }
            }
        }

        private void hideMainWindow()
        {
            ShowInTaskbar = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //イベントをイベントハンドラに関連付ける
            //フォームコンストラクタなどの適当な位置に記述してもよい
            SystemEvents.SessionEnding +=
                new SessionEndingEventHandler(SystemEvents_SessionEnding);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //イベントを解放する
            //フォームDisposeメソッド内の基本クラスのDisposeメソッド呼び出しの前に
            //記述してもよい
            SystemEvents.SessionEnding -=
                new SessionEndingEventHandler(SystemEvents_SessionEnding);
        }
    }
}
