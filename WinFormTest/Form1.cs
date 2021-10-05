using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormTest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // 버튼을 눌렀을때 3초 대기후 윈도우 타이틀에 글자 표시
        private void button1_Click(object sender, EventArgs e) {
            // 0. 동기: 원하는 대로 동작하지만 3초동안 UI가 반응을 안한다.
            //Thread.Sleep(3000);
            //Text = $"Thread.Sleep(3000), {DateTime.Now}";

            // 1. 바로 리턴하고 별도의 쓰레드에서 대기함 
            //Task.Delay(3000);
            //Text = $"Task.Delay(3000), {DateTime.Now}";

            // 2. 별도의 쓰레드에서 대기하는것을 완료 기다림, 기다리는 동안 UI반응 안함
            //var task = Task.Delay(3000);
            //task.Wait();
            //Text = $"Task.Delay(3000), {DateTime.Now}";

            // 3. 별도의 쓰레드에서 대기하는것 완료 후 글자 출력, 기다리는 동안 UI반응
            // 하지만 글자 출력을 별도 쓰레드에서 하기 때문에 Cross Thread 예외 발생
            //var task = Task.Delay(3000);
            //task.ContinueWith(t => Text = $"Task.Delay(3000), {DateTime.Now}");

            // 4. 별도의 쓰레드에서 대기하는것 완료 후 글자 출력, 기다리는 동안 UI반응
            // Cross Thread 예외 발생 방지 위해 Invoke 사용
            //var task = Task.Delay(3000);
            //task.ContinueWith(t => this.Invoke((Action)(() => Text = $"Task.Delay(3000), {DateTime.Now}")));

            // 5. 별도의 쓰레드에서 대기하는것 완료 후 글자 출력, 기다리는 동안 UI반응
            // Cross Thread 예외 발생 방지 위해 TaskScheduler.FromCurrentSynchronizationContext() 파라미터 추가
            var task = Task.Delay(3000);
            task.ContinueWith(t => Text = $"Task.Delay(3000), {DateTime.Now}", TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void button2_Click(object sender, EventArgs e) {
            // 6. async awiat 사용
            await Task.Delay(3000);
            Text = $"Thread.Sleep(3000), {DateTime.Now}";
        }

        private void button3_Click(object sender, EventArgs e) {
            Console.WriteLine("Hello Console!");
        }

        void ReLayout() {
            panel2.Controls.Clear();
            if (chkTabOrGrid.Checked) {
                var tab = new TabControl();
                tab.Dock = DockStyle.Fill;

                for (int i = 0; i < views.Count; i++) {
                    var tpg = new TabPage();
                    tpg.Name = i.ToString();
                    tpg.Text = i.ToString();
                    tpg.Controls.Add(views[i]);

                    tab.TabPages.Add(tpg);
                }

                panel2.Controls.Add(tab);
            } else {
                var pnl = new TableLayoutPanel();
                pnl.Dock = DockStyle.Fill;

                pnl.RowCount = 1;
                pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

                pnl.ColumnCount = views.Count;
                for (int i = 0; i < views.Count; i++) {
                    pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    pnl.Controls.Add(views[i]);
                }

                panel2.Controls.Add(pnl);
            }
        }

        List<Control> views = new List<Control>();

        private void btnAdd_Click(object sender, EventArgs e) {
            var tbx = new TextBox();
            tbx.Multiline = true;
            tbx.WordWrap = false;
            tbx.ScrollBars = ScrollBars.Both;
            tbx.Dock = DockStyle.Fill;
            tbx.Text = DateTime.Now.ToString();
            views.Add(tbx);
            ReLayout();
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (views.Count == 0)
                return;
            views.Remove(views.Last());
            ReLayout();
        }

        private void chkTabOrGrid_CheckedChanged(object sender, EventArgs e) {
            ReLayout();
        }
    }
}
