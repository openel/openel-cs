using System;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

class Demo : Form{
	private Label co2_label;
	private Label co2_value;
	private Label co2_text;
	private Label temperature_label;
	private Label temperature_value;
	private Label humidity_label;
	private Label humidity_value;
	private Label co2_label_other;
	private Label co2_label_a;
	private Label co2_value_a;
	private Label co2_label_b;
	private Label co2_value_b;
	private Button Btn1;
	private DiarkisTest diarkistest;
	private CancellationTokenSource tokenSource1;
	private CancellationTokenSource tokenSource2;
	private Task task1;
	private Task task2;

	static void Main(){
		Application.Run(new Demo());
		Environment.Exit(0);
	}

	public Demo(){
		//this.ControlBox = false;
		this.StartPosition = FormStartPosition.CenterScreen;
		this.Width = 800;
		this.Height = 480;
		this.Text = "分散型クラウドを活用したリアルタイムCO2濃度モニタリングシステム";

		co2_label = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "只今のCO2濃度[ppm]",
      			AutoSize = true,
			Location = new Point(10, 10),
			Parent=this,
		};

		co2_value = new Label(){
			Font = new Font("IPAexGothic", 180),
			ForeColor = Color.Green,
			Text = "0",
			AutoSize = true,
			Location = new Point(100, 40),
			Parent=this,
		};

		temperature_label = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "気温[℃]",
			AutoSize = true,
			Location = new Point(10, 320),
			Parent=this,
		};

		temperature_value = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "0",
			AutoSize = true,
			Location = new Point(200, 320),
			Parent=this,
		};

		humidity_label = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "湿度[%]",
			AutoSize = true,
			Location = new Point(400, 320),
			Parent=this,
		};

		humidity_value = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "0",
			AutoSize = true,
			Location = new Point(600, 320),
			Parent=this,
		};

		co2_text = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "",
			AutoSize = true,
			Location = new Point(10, 280),
			Parent=this,
		};

		co2_label_other = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "他の場所のCO2濃度[ppm]",
			AutoSize = true,
			Location = new Point(10, 360),
			Parent=this,
		};

		co2_label_a = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "東京都足立区",
			AutoSize = true,
			Location = new Point(10, 400),
			Parent=this,
		};

		co2_value_a = new Label(){
			Font = new Font("IPAexGothic", 24),
			ForeColor = Color.Green,
			Text = "0",
			AutoSize = true,
			Location = new Point(300, 400),
			Parent=this,
		};

		co2_label_b = new Label(){
			Font = new Font("IPAexGothic", 24),
			Text = "東京都世田谷区",
			AutoSize = true,
			Location = new Point(400, 400),
			Parent=this,
		};

		co2_value_b = new Label(){
			Font = new Font("IPAexGothic", 24),
			ForeColor = Color.Green,
			Text = "0",
			AutoSize = true,
			Location = new Point(700, 400),
			Parent=this,
		};

		Btn1 = new Button(){
			Font = new Font("IPAexGothic", 18),
			Text = "測定終了",
			Size = new Size(100, 35),
			Location = new Point(650, 10),
			Parent=this,
		};

		Btn1.Click += new EventHandler(this.Btn1_Click);

		diarkistest = new DiarkisTest();
		tokenSource1 = new CancellationTokenSource();
		var token1 = tokenSource1.Token;
		tokenSource2 = new CancellationTokenSource();
		var token2 = tokenSource2.Token;
		task1 = Task.Run(() => diarkistest.DiarkisMain(token1));
		task2 = Task.Run(() => measure(token2));
	}

	private void Btn1_Click(object sender, EventArgs e){
		Console.WriteLine("End");
		tokenSource1.Cancel();
		task1.Wait();
		Console.WriteLine("task1 End");
		tokenSource2.Cancel();
		task2.Wait();
		Console.WriteLine("task2 End");
		this.Close();
		Application.Exit();
	}

	public void measure(CancellationToken token) {
		while(true) { 
			co2_value.Text = diarkistest.valuelist0[0].ToString();
			co2_value_a.Text = diarkistest.valuelist1[0].ToString();
			co2_value_b.Text = diarkistest.valuelist2[0].ToString();
			temperature_value.Text = diarkistest.valuelist0[1].ToString();
			humidity_value.Text = diarkistest.valuelist0[2].ToString();
      			if(diarkistest.valuelist0[0] < 800) {
      				co2_value.ForeColor = Color.Green;
      				co2_text.ForeColor = Color.Green;
      				co2_text.Text = "正常レベルです。";
      			} else if(diarkistest.valuelist0[0] >= 800 && diarkistest.valuelist0[0] < 1000) {
      				co2_value.ForeColor = Color.Yellow;
      				co2_text.ForeColor = Color.Yellow;
      				co2_text.Text = "CO2濃度の上昇に注意してください！";
      			} else if(diarkistest.valuelist0[0] >= 1000 && diarkistest.valuelist0[0] < 1500) {
      				co2_value.ForeColor = Color.Orange;
      				co2_text.ForeColor = Color.Orange;
      				co2_text.Text = "換気が必要です！";
      			} else if(diarkistest.valuelist0[0] >= 1500) {
      				co2_value.ForeColor = Color.Red;
      				co2_text.ForeColor = Color.Red;
      				co2_text.Text = "過密状態です。すぐに換気してください！";
			}
      			if(diarkistest.valuelist1[0] < 800) {
      				co2_value_a.ForeColor = Color.Green;
      			} else if(diarkistest.valuelist1[0] >= 800 && diarkistest.valuelist0[0] < 1000) {
      				co2_value_a.ForeColor = Color.Yellow;
      			} else if(diarkistest.valuelist1[0] >= 1000 && diarkistest.valuelist0[0] < 1500) {
      				co2_value_a.ForeColor = Color.Orange;
      			} else if(diarkistest.valuelist1[0] >= 1500) {
      				co2_value_a.ForeColor = Color.Red;
			}

      			if(diarkistest.valuelist2[0] < 800) {
      				co2_value_b.ForeColor = Color.Green;
      			} else if(diarkistest.valuelist2[0] >= 800 && diarkistest.valuelist0[0] < 1000) {
      				co2_value_b.ForeColor = Color.Yellow;
      			} else if(diarkistest.valuelist2[0] >= 1000 && diarkistest.valuelist0[0] < 1500) {
      				co2_value_b.ForeColor = Color.Orange;
      			} else if(diarkistest.valuelist2[0] >= 1500) {
      				co2_value_b.ForeColor = Color.Red;
			}

			Task.Delay(1000);
			if (token.IsCancellationRequested) {
				Console.WriteLine("Cancelled");
				break;
			}
		}
	}
}

