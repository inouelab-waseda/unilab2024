using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;

namespace unilab2024
{
    public partial class Stage : Form
    {
        public Stage()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(ListBox_DragDrop);          //Form全体にDrop可能にする
            this.DragEnter += new DragEventHandler(ListBox_DragEnter);

            //pictureBoxの設定
            pictureBox2.Parent = pictureBox1;
            //pictureBox1.Location = new Point(600, 50);
            pictureBox2.Location = new Point(0, 0);
            pictureBox1.ClientSize = new Size(684, 684);
            pictureBox2.ClientSize = new Size(684, 684);
            pictureBox2.BackColor = Color.Transparent;

            bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            bmp3 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            //bmp4 = new Bitmap(pictureBox4.Width, pictureBox4.Height);
            pictureBox1.Image = bmp1;
            pictureBox2.Image = bmp2;
            pictureBox3.Image = bmp3;
            //pictureBox4.Image = bmp4;
            this.Load += Stage_Load;

        }

        #region メンバー変数定義
        private string _worldName;
        private int _worldNumber;
        private int _level;
        public int WorldNumber     //StageSelectからの呼び出し用
        {
            get { return _worldNumber; }
            set { _worldNumber = value; }
            //別フォームからのアクセス例
            //Stage form = new Stage();
            //form.StageName = "ステージ名";
        }
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public string WorldName
        {
            get { return _worldName; }
            set { _worldName = value; }
        }
        #endregion

        #region グローバル変数定義
        //ここに必要なBitmapやImageを作っていく
        Bitmap bmp1, bmp2, bmp3, bmp4;

        Brush goalBackgroundColor = new SolidBrush(Color.Yellow);
        Brush startBackgroundColor = new SolidBrush(Color.Blue);

        //Image[] img_otomo = new Image[4] {
        //    Image.FromFile("キャラ_たぬき.png"),
        //    Image.FromFile("キャラ_きつね.png"),
        //    Image.FromFile("キャラ_あざらし.png"),
        //    Image.FromFile("キャラ_ふくろう.png")
        //};

        Image character_me = Image.FromFile("忍者_正面.png");

        //Image[] character_enemy = new Image[6] {
        //    Image.FromFile("キャラ_一つ目小僧.png"),
        //    Image.FromFile("キャラ_唐傘一反.png"),
        //    Image.FromFile("キャラ_カッパ.png"),
        //    Image.FromFile("キャラ_てんぐ.png"),
        //    Image.FromFile("キャラ_赤鬼.png"),
        //    Image.FromFile("キャラ_ヤマタノオロチ.png")
        //};

        //Image img_maki = Image.FromFile("マップ_巻物.png");
        //Image img_way = Image.FromFile("マップ_草原.png");
        //Image img_noway = Image.FromFile("マップ_岩場.png");
        //Image img_ice = Image.FromFile("マップ_氷.png");
        //Image img_tree = Image.FromFile("マップ_木.png");
        //Image img_jump = Image.FromFile("マップ_ジャンプ1.png");
        //Image animatedImage_up = Image.FromFile("マップ_動く床_上.gif");
        //Image animatedImage_right = Image.FromFile("マップ_動く床_右.gif");
        //Image animatedImage_down = Image.FromFile("マップ_動く床_下.gif");
        //Image animatedImage_left = Image.FromFile("マップ_動く床_左.gif");
        //Image cloud_ul = Image.FromFile("マップ_雲_左上.png");
        //Image cloud_left = Image.FromFile("マップ_雲_左.png");
        //Image cloud_bl = Image.FromFile("マップ_雲_左下.png");
        //Image cloud_bottom = Image.FromFile("マップ_雲_下.png");
        //Image cloud_br = Image.FromFile("マップ_雲_右下.png");
        //Image cloud_right = Image.FromFile("マップ_雲_右.png");
        //Image cloud_ur = Image.FromFile("マップ_雲_右上.png");
        //Image cloud_upside = Image.FromFile("マップ_雲_上.png");

        Image[] pictures = new Image[10];

        public static int[,] map = new int[12, 12]; //map情報
        public static int x_start; //スタート位置ｘ
        public static int y_start; //スタート位置ｙ
        public static int x_goal; //ゴール位置ｘ
        public static int y_goal; //ゴール位置ｙ
        public static int x_now; //現在位置ｘ
        public static int y_now; //現在位置 y
        public static int extra_length = 7;
        public static int cell_length;

        public static int count = 0; //試行回数カウント
        public static int miss_count = 0; //ミスカウント

        public static int count_walk = 0; //歩数カウント

        public static List<int[]> move;  //プレイヤーの移動指示を入れるリスト

        //listBoxに入れられる行数の制限
        public static int limit_LB_Input;
        public static int limit_LB_A;
        public static int limit_LB_B;

        public static string hint;
        public static string hint_character;
        public static string hint_name;

        public static string grade;    //学年
        public static int gradenum;

        public static List<Conversation> Conversations = new List<Conversation>();  //会話文を入れるリスト
        //public class Global    //グローバル変数
        //{
        //    public static int[,] map = new int[12, 12]; //map情報
        //    public static int x_start; //スタート位置ｘ
        //    public static int y_start; //スタート位置ｙ
        //    public static int x_goal; //ゴール位置ｘ
        //    public static int y_goal; //ゴール位置ｙ
        //    public static int x_now; //現在位置ｘ
        //    public static int y_now; //現在位置 y
        //    public static int extra_length = 7;
        //    public static int cell_length;

        //    public static int count = 0; //試行回数カウント
        //    public static int miss_count = 0; //ミスカウント

        //    public static int count_walk = 0; //歩数カウント

        //    public static List<int[]> move;  //プレイヤーの移動指示を入れるリスト

        //    //listBoxに入れられる行数の制限
        //    public static int limit_LB_Input;
        //    public static int limit_LB_A;
        //    public static int limit_LB_B;

        //    public static string hint;
        //    public static string hint_character;
        //    public static string hint_name;

        //    public static string grade;    //学年

        //    public static List<Conversation> Conversations = new List<Conversation>();  //会話文を入れるリスト
        //}
        #endregion


        public void Stage_Load(object sender, EventArgs e)        //StageのFormの起動時処理
        {
            //button5.Visible = false;
            //_stageName = "stage2-3";
            string stageName = "stage"+_worldNumber + "-" + _level;
            map = CreateStage(stageName); //ステージ作成

            grade = Regex.Replace(stageName, @"[^0-9]", "");
            int chapter_num = int.Parse(grade) / 10;

            string file_name = "わせマジ" + grade.ToString() + "章.csv";

            //Global.Conversations = LoadConversation(file_name); //会話読み込み

            #region リストボックス・ボタンの設定
            // 1行文の高さ
            int ItemHeight = 20;
            listBox_Input.ItemHeight = ItemHeight;
            listBox_options.ItemHeight = ItemHeight;
            listBox_A.ItemHeight = ItemHeight;
            listBox_B.ItemHeight = ItemHeight;
            listBox_SelectAB.ItemHeight = ItemHeight;

            int element_height = listBox_Input.ItemHeight;

            // それぞれの枠の高さ
            int height_LB_Input = 0;
            int height_LB_A = 0;
            int height_LB_B = 0;

            //using (StreamReader sr = new StreamReader($"stage_frame.csv"))
            //{
            //    while (!sr.EndOfStream)
            //    {
            //        string line = sr.ReadLine();
            //        string[] values = line.Split(',');


            //        if (values[0] == _stageName)
            //        {
            //            Global.limit_LB_Input = int.Parse(values[1]);
            //            Global.limit_LB_A = int.Parse(values[2]);
            //            Global.limit_LB_B = int.Parse(values[3]);
            //            break;
            //        }
            //    }
            //}

            height_LB_Input = limit_LB_Input + 1;
            height_LB_A = limit_LB_A + 1;
            height_LB_B = limit_LB_B + 1;

            if (height_LB_Input == 1)
            {
                listBox_Input.Visible = false;
                label_Info.Visible = false;
                listBox_SelectAB.Items.Remove("A");
                button_ResetInput.Visible = false;
                button_ResetInput.Enabled = false;

            }

            if (height_LB_A == 1)
            {
                listBox_A.Visible = false;
                label_B.Visible = false;
                listBox_B.Items.Remove("B");
                button_ResetInput.Visible = false;
                button_ResetInput.Enabled = false;
            }

            if (height_LB_Input == 1 && height_LB_A == 1)
            {
                listBox_SelectAB.Visible = false;
            }

            hint = null;
            hint_character = null;
            hint_name = null;

            // CSVから読み込んだテキストを設定します。
            //using (StreamReader sr = new StreamReader($"hint.csv"))
            //{
            //    while (!sr.EndOfStream)
            //    {
            //        string line = sr.ReadLine();
            //        string[] values = line.Split(',');


            //        if (values[0] == _stageName)
            //        {
            //            Global.hint_character = values[1];
            //            Global.hint = values[2];
            //            Global.hint_name = values[3];
            //            break;
            //        }
            //    }
            //}

            if (hint == null)
            {
                button_Hint.Visible = false;
            }
            else
            {
                button_Hint.Visible = true;
            }

            listBox_Input.Height = element_height * height_LB_Input;
            listBox_A.Height = element_height * height_LB_A;
            listBox_B.Height = element_height * height_LB_B;

            //ListBox1のイベントハンドラを追加
            listBox_Input.SelectionMode = SelectionMode.One;
            listBox_Input.DragEnter += new DragEventHandler(ListBox_DragEnter);
            listBox_Input.DragDrop += new DragEventHandler(ListBox_DragDrop);
            listBox_options.MouseDown += new MouseEventHandler(ListBox_MouseDown);
            listBox_A.SelectionMode = SelectionMode.One;
            listBox_A.DragEnter += new DragEventHandler(ListBox_DragEnter);
            listBox_A.DragDrop += new DragEventHandler(ListBox_DragDrop);
            listBox_B.SelectionMode = SelectionMode.One;
            listBox_B.DragEnter += new DragEventHandler(ListBox_DragEnter);
            listBox_B.DragDrop += new DragEventHandler(ListBox_DragDrop);
            listBox_SelectAB.MouseDown += new MouseEventHandler(ListBox_MouseDown);

            //    //ヒントを教えるキャラのアイコンを表示
            //    Graphics g3 = Graphics.FromImage(bmp3);
            //    Bitmap bmp = new Bitmap(1, 1);
            //    bmp.SetPixel(0, 0, Color.White);
            //    g3.DrawImage(bmp, 0, 0, bmp3.Height - 1, bmp3.Height - 1);
            //    g3.DrawRectangle(Pens.Black, 0, 0, bmp3.Height - 1, bmp3.Height - 1);
            //    g3.Dispose();

            //    //チュートリアルステージでは、マップに戻るボタンを消す。ゴールしたら見える
            //    if (stageName == "stage1-1")
            //    {
            //        button5.Visible = false;
            //    }
            //    //for文をステージ1-1,1-2で消す
            //    if (stageName == "stage1-1" || stageName == "stage1-2")
            //    {
            //        listBox2.Items.Remove("連チャンの術 (1)");
            //        listBox2.Items.Remove("連チャンの術おわり");
            //    }

            //    //ストーリー強制視聴
            //    listBox2.Enabled = false;
            //    listBox5.Enabled = false;
            //    button1.Enabled = false;
            //    button6.Enabled = false;
            //    button8.Enabled = false;
            //    drawConversation();
            #endregion
        }

        #region リセット関連
        private void button_ResetInput_Click(object sender, EventArgs e)        //起動部分リセット
        {
            Func.ResetListBox(listBox_Input, listBox_Input);                    //Program.CSに処理記載
        } 
        private void button_ResetA_Click(object sender, EventArgs e)           //Aの魔法リセット
        {
           Func.ResetListBox(listBox_Input, listBox_A);
        }       

        private void button_ResetB_Click(object sender, EventArgs e)           //Bの魔法リセット
        {
            Func.ResetListBox(listBox_Input, listBox_B);
        }
        public void resetStage(string type) // ステージリセットまとめ
        {
            if (type == "quit")
            {
                Func.CreateStageSelect(this, _worldName, _worldNumber);
                return;
            }

            // 曇から落ちたミス
            else if (type == "miss_out")
            {
                label_Error.Text = "そこは止まれないよ！やり直そう！";
                label_Error.Visible = true;
                Thread.Sleep(300);
                miss_count += 1;
            }

            //木に刺されたミス
            else if (type == "miss_tree")
            {
                label_Error.Text = "木に刺された！やり直そう！";
                label_Error.Visible = true;
                Thread.Sleep(300);
                miss_count += 1;
            }

            //無限ループの時のミス
            else if (type == "miss_countover")
            {
                label_Error.Text = "これ以上は移動できない！やり直そう！";
                label_Error.Visible = true;
                Thread.Sleep(300);
                miss_count += 1;
            }

            //止まった時ゴール到着してないミス
            else if (type == "miss_end")
            {
                label_Error.Text = "ゴールまで届いてないね！やり直そう！";
                label_Error.Visible = true;
                Thread.Sleep(300);
                miss_count += 1;
            }

            // リトライボタン
            else if (type == "retry")
            {
                //初期位置に戻す
                x_now = x_start;
                y_now = y_start;

                //初期位置に書き換え
                Graphics g2 = Graphics.FromImage(bmp2);
                g2.Clear(Color.Transparent);
                int cell_length = pictureBox1.Width / 12;
                //character_me = Image.FromFile("忍者_正面.png");
                //g2.DrawImage(character_me, Global.x_now * cell_length - Global.extra_length, Global.y_now * cell_length - 2 * Global.extra_length, cell_length + 2 * Global.extra_length, cell_length + 2 * Global.extra_length);

                //g2.DrawImage(goal_obj(_stageName), Global.x_goal * cell_length - Global.extra_length, Global.y_goal * cell_length - 2 * Global.extra_length, cell_length + 2 * Global.extra_length, cell_length + 2 * Global.extra_length);
                this.Invoke((MethodInvoker)delegate
                {
                    // pictureBox2を同期的にRefreshする
                    pictureBox2.Refresh();
                });

                //初期設定に戻す
                button_Start.Visible = true;
                button_Start.Enabled = true;
                label_Error.Visible = false;
                count = 0;
                miss_count = 0;
                label_Error.Text = "ミス！";
                label_Error.Visible = false;
                label_Result.Visible = false;
                label_Info.Visible = false;
            }
        }
        #endregion

        #region ボタン押下時処理
        private void button_Start_Click(object sender, EventArgs e)  //出発ボタン押下時処理
        {
            button_Start.Visible = false;
            button_Start.Enabled = false;
            label_Error.Visible = false;
            move = Movement(); //ユーザーの入力を読み取る
            SquareMovement(x_now, y_now, map, move); //キャラ動かす
            count += 1;
            if (x_goal == x_now && y_goal == y_now)
            {
                label_Result.Text = "クリア！！";
                label_Result.Visible = true;
                button_ToMap.Enabled = false;
                button_Retry.Enabled = false;
                button_ToMap.Visible = true;
                button_ToMap.Location = new Point(800, 600);
                button_ToMap.Size = new Size(200, 50);
                label_Info.Visible = true;
                pictureBox4.Visible = false;
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                pictureBox7.Visible = false;
                ClearCheck.IsCleared[_worldNumber, _level] = true;    //クリア状況管理
            }
            else
            {
                resetStage("miss_end");
            }
        }

        private void button_Retry_Click(object sender, EventArgs e)  //リトライボタン押下時処理
        {
            resetStage("reset");
        }

        private void button_ToMap_Click(object sender, EventArgs e)  //マップに戻るボタン押下時処理
        {
            resetStage("quit");
        }
        #endregion

        # region ListBox要素操作
        bool isEnableDrop = true;
        private void ListBox_MouseDown(object sender, MouseEventArgs e)   //ドラッグ&ドロップ処理
        {
            //マウスの左ボタンだけが押されている時のみドラッグできるようにする
            if (e.Button == MouseButtons.Left)
            {
                //ドラッグの準備
                ListBox lbx = (ListBox)sender;
                //ドラッグするアイテムのインデックスを取得する
                int itemIndex = lbx.IndexFromPoint(e.X, e.Y);
                if (itemIndex < 0) return;
                //ドラッグするアイテムの内容を取得する
                string itemText = (string)lbx.Items[itemIndex];

                //ドラッグ&ドロップ処理を開始する
                DragDropEffects dde =
                    lbx.DoDragDrop(itemText, DragDropEffects.All);

                ////ドロップ効果がMoveの時はもとのアイテムを削除する
                //if (dde == DragDropEffects.Move)
                //    lbx.Items.RemoveAt(itemIndex);

                isEnableDrop = true;
            }
        }

        private ListBox GetNearestListBox(Point point)                    //ドラック先選択
        {
            // 3つのListBoxをリストに格納する
            List<ListBox> listBoxes = listBoxes = new List<ListBox> { listBox_Input, listBox_A, listBox_B };

            // Aボタンがないとき
            if (limit_LB_A == 0)
            {
                listBoxes = new List<ListBox> { listBox_Input, listBox_B };
            }

            // Bボタンがないとき
            if (limit_LB_Input == 0)
            {
                listBoxes = new List<ListBox> { listBox_A, listBox_B };
            }

            // A.Bボタンない時
            if (limit_LB_Input == 0 && limit_LB_A == 0)
            {
                listBoxes = new List<ListBox> { listBox_B };
            }
            double minDistance = double.MaxValue;
            ListBox nearestListBox = null;

            foreach (var listBox in listBoxes)
            {
                // ListBoxの中心座標を計算する
                Point listBoxCenter = new Point(listBox.Location.X + listBox.Width / 2, listBox.Location.Y + listBox.Height / 2);

                // ドラッグされたポイントとListBoxの中心との間の距離を計算する
                double distance = Math.Sqrt(Math.Pow(listBoxCenter.X - point.X, 2) + Math.Pow(listBoxCenter.Y - point.Y, 2));

                // これまでの最小距離よりも小さい場合は、更新する
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestListBox = listBox;
                }
            }

            // 最も近いListBoxを返す
            return nearestListBox;
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)    //ドラックしたアイテムの中身確認
        {
            //ドラッグされているデータがstring型か調べ、
            //そうであればドロップ効果をMoveにする
            if (e.Data.GetDataPresent(typeof(string)))
                e.Effect = DragDropEffects.Move;
            else
                //string型でなければ受け入れない
                e.Effect = DragDropEffects.None;
        }

        private void DisplayImageAndTextOnPictureBox(PictureBox pictureBox, string image, string text)    //中身後で考える
        {
            // 画像ファイルを読み込む。
            Image img = Image.FromFile(image);

            Bitmap bmp = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            Graphics g = Graphics.FromImage(bmp);

            Font fnt = new Font("游明朝", 20);
            int sp = 8;

            g.DrawImage(img, 0, 0, bmp.Height - 1, bmp.Height - 1);
            g.DrawRectangle(Pens.Black, 0, 0, bmp.Height - 1, bmp.Height - 1);

            g.DrawRectangle(Pens.White, 100, 100, 100, 100);
            g.DrawString(text, fnt, Brushes.Black, bmp.Height + sp, 0 + sp);

            pictureBox.Image = bmp;
            //label2.Text = Global.hint_name;


            g.Dispose();
        }

        /*
        //listbox内の行数を制限しない場合
        //private void ListBox_DragDrop(object sender, DragEventArgs e)
        //{
        //    //ドロップされたデータがstring型か調べる
        //    if (e.Data.GetDataPresent(typeof(string)) && isEnableDrop)
        //    {
        //        ListBox target = (ListBox)sender;
        //        //ドロップされたデータ(string型)を取得
        //        string itemText =
        //            (string)e.Data.GetData(typeof(string));
        //        //ドロップされたデータをリストボックスに追加する
        //        target.Items.Add(itemText);

        //        isEnableDrop = false;
        //    }
        //}
        */

        private void ListBox_DragDrop(object sender, DragEventArgs e)     //ドロップ時処理
        {
            //ドロップされたデータがstring型か調べる
            if (e.Data.GetDataPresent(typeof(string)) && isEnableDrop)
            {
                Point point = this.PointToClient(new Point(e.X, e.Y));
                ListBox target = GetNearestListBox(point); // ここでマウス位置に最も近いリストボックスを取得

                if (target == null) // 最も近いリストボックスがない場合は何もしない
                    return;

                //listBoxの名前によって制限数を設定
                int limit = 0;
                switch (target.Name)
                {
                    case "listBox_Input":
                        limit = limit_LB_Input;
                        break;
                    case "listBox_A":
                        limit = limit_LB_A;
                        break;
                    case "listBox_B":
                        limit = limit_LB_B;
                        break;
                        //default:
                        //    throw new Exception("Unsupported ListBox name.");
                }

                // ドロップによってアイテム数が制限数を超える場合はドロップを拒否
                if (target.Items.Count >= limit)
                {
                    MessageBox.Show($"{target.Name} can only contain up to {limit} items.");
                    return;
                }

                //ドロップされたデータ(string型)を取得
                string itemText = (string)e.Data.GetData(typeof(string));

                //ドロップされたデータをリストボックスに追加する
                target.Items.Add(itemText);

                isEnableDrop = false;
            }
        }
        #endregion

        #region for文処理
        private void listBox_Input_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Func.ForLoopCount(listBox_Input);
        }
        private void listBox_A_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Func.ForLoopCount(listBox_A);
        }
        private void listBox_B_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Func.ForLoopCount(listBox_B);
        }
        #endregion

        private int[,] CreateStage(string stage_name)     //ステージ作成
        {
            //using (StreamReader sr = new StreamReader($"{_stageName}.csv"))
            //{
            //    int x;
            //    int y = 0;

            //    while (!sr.EndOfStream)
            //    {
            //        string line = sr.ReadLine();
            //        string[] values = line.Split(',');

            //        x = 0;

            //        foreach (var value in values)
            //        {
            //            // enum 使ったほうが分かりやすそう
            //            map[y, x] = int.Parse(value);
            //            x++;
            //        }
            //        y++;
            //    }
            //}

            Graphics g1 = Graphics.FromImage(bmp1);
            Graphics g2 = Graphics.FromImage(bmp2);
            label_Info.BackgroundImage = Image.FromFile("focus.png");
            label_Info.BackgroundImageLayout = ImageLayout.Stretch;

            cell_length = pictureBox1.Width / 12;

            //for (int y = 1; y < 11; y++)
            //{
            //    for (int x = 1; x < 11; x++)
            //    {
            //        g1.DrawImage(img_way, x * Global.cell_length, y * Global.cell_length, Global.cell_length, Global.cell_length);
            //    }
            //}

            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    int placeX = x * cell_length;
                    int placeY = y * cell_length;
                    g1.DrawImage(pictures[map[y,x]], placeX, placeY, cell_length, cell_length);

                    switch (map[y, x]) //配列に画像を保存し表示で十分
                    {

                        //case 0:
                        //    g1.DrawImage(img_noway, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 1:
                        //    g1.DrawImage(img_way, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 2:
                        //    g1.DrawImage(img_ice, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 3:
                        //    g1.DrawImage(img_jump, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 4:
                        //    ImageAnimator.UpdateFrames(animatedImage_up);
                        //    g1.DrawImage(animatedImage_up, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 5:
                        //    ImageAnimator.UpdateFrames(animatedImage_right);
                        //    g1.DrawImage(animatedImage_right, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 6:
                        //    ImageAnimator.UpdateFrames(animatedImage_down);
                        //    g1.DrawImage(animatedImage_down, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 7:
                        //    ImageAnimator.UpdateFrames(animatedImage_left);
                        //    g1.DrawImage(animatedImage_left, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 8:
                        //    g1.DrawImage(img_tree, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 20:
                        //    g1.DrawImage(cloud_ul, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 21:
                        //    g1.DrawImage(cloud_left, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 22:
                        //    g1.DrawImage(cloud_bl, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 23:
                        //    g1.DrawImage(cloud_bottom, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 24:
                        //    g1.DrawImage(cloud_br, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 25:
                        //    g1.DrawImage(cloud_right, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 26:
                        //    g1.DrawImage(cloud_ur, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 27:
                        //    g1.DrawImage(cloud_upside, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    break;
                        //case 100:
                        //    g1.FillRectangle(startBackgroundColor,placeX, placeY, Global.cell_length, Global.cell_length);
                        //    Global.x_start = x;
                        //    Global.y_start = y;
                        //    Global.x_now = x;
                        //    Global.y_now = y;
                        //    g2.DrawImage(character_me, placeX - Global.extra_length, placeY - 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length);
                        //    break;
                        //case 101:
                        //    g1.FillRectangle(goalBackgroundColor, placeX, placeY, Global.cell_length, Global.cell_length);
                        //    //ステージごとにゴールのキャラを変えたい
                        //    g2.DrawImage(goal_obj(_stageName), placeX - Global.extra_length, placeY - 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length);
                        //    Global.x_goal = x;
                        //    Global.y_goal = y;
                        //    break;
                    }
                }
            }


            //Bitmap bmp5, bmp6, bmp7, bmp8;
            //Image[] img_otomo = new Image[4] {
            //    Image.FromFile("キャラ_たぬき.png"),
            //    Image.FromFile("キャラ_きつね.png"),
            //    Image.FromFile("キャラ_あざらし.png"),
            //    Image.FromFile("キャラ_ふくろう.png")
            //};
            //bmp5 = new Bitmap(pictureBox4.Width, pictureBox4.Height);
            //bmp6 = new Bitmap(pictureBox5.Width, pictureBox5.Height);
            //bmp7 = new Bitmap(pictureBox6.Width, pictureBox6.Height);
            //bmp8 = new Bitmap(pictureBox7.Width, pictureBox7.Height);
            //pictureBox4.Image = bmp5;
            //pictureBox5.Image = bmp6;
            //pictureBox6.Image = bmp7;
            //pictureBox7.Image = bmp8;


            //this.Invoke((MethodInvoker)delegate
            //{
            //    // pictureBox2を同期的にRefreshする
            //    pictureBox2.Refresh();
            //});

            //if (stageClear[0])
            //{
            //    //pictureBox4.Visible = true;
            //    Graphics graphi = Graphics.FromImage(bmp5);
            //    graphi.Clear(Color.Transparent);
            //    graphi.DrawImage(img_otomo[0], 0, 0, 100, 100);

            //}
            //if (stageClear[1])
            //{
            //    //pictureBox5.Visible = true;
            //    Graphics graphi = Graphics.FromImage(bmp6);
            //    graphi.Clear(Color.Transparent);
            //    graphi.DrawImage(img_otomo[1], 0, 0, 100, 100);
            //}
            //if (stageClear[2])
            //{
            //    //pictureBox6.Visible = true;
            //    Graphics graphi = Graphics.FromImage(bmp7);
            //    graphi.Clear(Color.Transparent);
            //    graphi.DrawImage(img_otomo[2], 0, 0, 100, 100);
            //}
            //if (stageClear[3])
            //{
            //    //pictureBox7.Visible = true;
            //    Graphics graphi = Graphics.FromImage(bmp8);
            //    graphi.Clear(Color.Transparent);
            //    graphi.DrawImage(img_otomo[3], 0, 0, 100, 100);
            //}
            return map;
        }

        #region 動作関連
        public List<int[]> Movement()      //動作の関数
        {
            var Move_A = new List<int[]>();                                                           //Aでの動きを保存
            var Move_B = new List<int[]>();                                                           //Bでの動きを保存
            string[] Get_Input_A = this.listBox_A.Items.Cast<string>().ToArray();                     //AのListへの入力を保存
            string[] Get_Input_B = this.listBox_B.Items.Cast<string>().ToArray();                     //BのListへの入力を保存
            Get_Input_A = exchange_move(Get_Input_A, Get_Input_A.Length);                             //AのListへの入力を動きに変換
            Get_Input_B = exchange_move(Get_Input_B, Get_Input_B.Length);                             //BのListへの入力を動きに変換
            var Move_A_List = new List<string>(Get_Input_A);
            var Move_B_List = new List<string>(Get_Input_B);

            //get_move_a_list.AddRange(get_move_a);
            //get_move_b_list.AddRange(get_move_b);

            int loop_count = 0;
            while (Move_A_List.Count <= 30 || Move_B_List.Count <= 30)
            {
                Move_A_List = Func.MakeMoveList(Get_Input_A, Get_Input_B, Move_A_List);
                Move_B_List = Func.MakeMoveList(Get_Input_A, Get_Input_B, Move_B_List);
                /*var get_move_a_list_copy = new List<string>(Move_A_List);
                var get_move_b_list_copy = new List<string>(Move_B_List);
                Move_A_List.Clear();
                Move_B_List.Clear();


                for (int i = 0; i < get_move_a_list_copy.Count; i++)
                {

                    if (get_move_a_list_copy[i] == "B")
                    {
                        Move_A_List.AddRange(Get_Input_B);

                    }
                    else if (get_move_a_list_copy[i] == "A")
                    {
                        get_move_a_list.AddRange(get_move_a_list_copy);

                    }
                    else
                    {
                        get_move_a_list.Add(get_move_a_list_copy[i]);
                    }
                }

                for (int i = 0; i < get_move_b_list_copy.Count; i++)
                {

                    if (get_move_b_list_copy[i] == "B")
                    {
                        get_move_b_list.AddRange(get_move_b);

                    }
                    else if (get_move_b_list_copy[i] == "A")
                    {
                        get_move_b_list.AddRange(get_move_a_list_copy);

                    }
                    else
                    {
                        get_move_b_list.Add(get_move_b_list_copy[i]);
                    }
                }*/           //削除候補
                loop_count++;

                if (loop_count > 5)
                {
                    break;
                }
            }

            if (Get_Input_A.Length != 0)
            {
                //string[] get_move_a = this.listBox_Input.Items.Cast<string>().ToArray();

                for (int i = 0; i < Move_A_List.Count; i++)
                {
                    (Move_A, i) = Func.ForLoop(Move_A_List,Move_A,Move_B, i);
                    #region 削除候補
                    //if (get_move_a_list[i].StartsWith("for"))
                    //{
                    //    int start = i + 1;
                    //    int trial = int.Parse(Regex.Replace(get_move_a_list[i], @"[^0-9]", ""));

                    //    int goal = 0; //後で設定

                    //    for (int j = 0; j < trial; j++)
                    //    {
                    //        int k = start;
                    //        do
                    //        {
                    //            if (k >= get_move_a_list.Count)
                    //            {
                    //                MessageBox.Show("「反復魔法」と「反復魔法おわり」はセットで使ってください");
                    //                return new List<int[]>();
                    //            }
                    //            if (get_move_a_list[k].StartsWith("for")) //二重ループ
                    //            {
                    //                int trial2 = int.Parse(Regex.Replace(get_move_a_list[k], @"[^0-9]", ""));
                    //                for (int l = 0; l < trial2; l++)
                    //                {
                    //                    k = start + 1;
                    //                    do
                    //                    {
                    //                        if (get_move_a_list[k] == "endfor")
                    //                        {
                    //                            break;
                    //                        }

                    //                        else if (get_move_a_list[k] == "up")
                    //                        {
                    //                            move_a.Add(new int[2] { 0, -1 });
                    //                        }
                    //                        else if (get_move_a_list[k] == "down")
                    //                        {
                    //                            move_a.Add(new int[2] { 0, 1 });
                    //                        }
                    //                        else if (get_move_a_list[k] == "right")
                    //                        {
                    //                            move_a.Add(new int[2] { 1, 0 });
                    //                        }
                    //                        else if (get_move_a_list[k] == "left")
                    //                        {
                    //                            move_a.Add(new int[2] { -1, 0 });
                    //                        }
                    //                        k++;
                    //                    } while (true);
                    //                }
                    //            }
                    //            else if (get_move_a_list[k] == "endfor")
                    //            {
                    //                goal = k;
                    //                break;
                    //            }
                    //            else if (get_move_a_list[k] == "up")
                    //            {
                    //                move_a.Add(new int[2] { 0, -1 });
                    //            }
                    //            else if (get_move_a_list[k] == "down")
                    //            {
                    //                move_a.Add(new int[2] { 0, 1 });
                    //            }
                    //            else if (get_move_a_list[k] == "right")
                    //            {
                    //                move_a.Add(new int[2] { 1, 0 });
                    //            }
                    //            else if (get_move_a_list[k] == "left")
                    //            {
                    //                move_a.Add(new int[2] { -1, 0 });
                    //            }
                    //            k++;
                    //        } while (true);
                    //    }
                    //    i = goal;
                    //}
                    //else
                    //{
                    //    if (get_move_a_list[i] == "up")
                    //    {
                    //        move_a.Add(new int[2] { 0, -1 });
                    //    }
                    //    else if (get_move_a_list[i] == "down")
                    //    {
                    //        move_a.Add(new int[2] { 0, 1 });
                    //    }
                    //    else if (get_move_a_list[i] == "right")
                    //    {
                    //        move_a.Add(new int[2] { 1, 0 });
                    //    }
                    //    else if (get_move_a_list[i] == "left")
                    //    {
                    //        move_a.Add(new int[2] { -1, 0 });
                    //    }
                    //}
                    #endregion
                    Func.Move(Move_A, Get_Input_A[i]);
                }
            }

            if (Get_Input_B.Length != 0)
            {
                //string[] get_move_b = this.listBox3.Items.Cast<string>().ToArray();

                for (int i = 0; i < Move_B_List.Count; i++)
                {
                    (Move_B, i) = Func.ForLoop(Move_B_List, Move_A, Move_B, i);
                    #region 削除候補
                    //if (get_move_b_list[i].StartsWith("for"))
                    //{
                    //    int start = i + 1;
                    //    int trial = int.Parse(Regex.Replace(get_move_b_list[i], @"[^0-9]", ""));

                    //    int goal = 0; //後で設定

                    //    for (int j = 0; j < trial; j++)
                    //    {
                    //        int k = start;
                    //        do
                    //        {
                    //            if (k >= get_move_b_list.Count)
                    //            {
                    //                MessageBox.Show("「反復魔法」と「反復魔法おわり」はセットで使ってください");
                    //                return new List<int[]>();
                    //            }
                    //            if (get_move_b_list[k].StartsWith("for")) //二重ループ
                    //            {
                    //                int trial2 = int.Parse(Regex.Replace(get_move_b_list[k], @"[^0-9]", ""));
                    //                for (int l = 0; l < trial2; l++)
                    //                {
                    //                    k = start + 1;
                    //                    if (get_move_b_list[k] == "endfor") break;
                    //                    else
                    //                    {
                    //                        Func.Move(move_b, get_move_b_list[k]);
                    //                        k++;
                    //                    }
                    //                    //do
                    //                    //{
                    //                    //    if (get_move_b_list[k] == "endfor")
                    //                    //    {
                    //                    //        break;
                    //                    //    }

                    //                    //    else if (get_move_b_list[k] == "up")
                    //                    //    {
                    //                    //        move_b.Add(new int[2] { 0, -1 });
                    //                    //    }
                    //                    //    else if (get_move_b_list[k] == "down")
                    //                    //    {
                    //                    //        move_b.Add(new int[2] { 0, 1 });
                    //                    //    }
                    //                    //    else if (get_move_b_list[k] == "right")
                    //                    //    {
                    //                    //        move_b.Add(new int[2] { 1, 0 });
                    //                    //    }
                    //                    //    else if (get_move_b_list[k] == "left")
                    //                    //    {
                    //                    //        move_b.Add(new int[2] { -1, 0 });
                    //                    //    }
                    //                    //    k++;
                    //                    //} while (true);
                    //                }
                    //            }
                    //            else if (get_move_b_list[k] == "endfor")
                    //            {
                    //                goal = k;
                    //                break;
                    //            }
                    //            else
                    //            {
                    //                Func.Move(move_b, get_move_b_list[k]);
                    //                k++;
                    //            }
                    //            //else if (get_move_b_list[k] == "up")
                    //            //{
                    //            //    move_b.Add(new int[2] { 0, -1 });
                    //            //}
                    //            //else if (get_move_b_list[k] == "down")
                    //            //{
                    //            //    move_b.Add(new int[2] { 0, 1 });
                    //            //}
                    //            //else if (get_move_b_list[k] == "right")
                    //            //{
                    //            //    move_b.Add(new int[2] { 1, 0 });
                    //            //}
                    //            //else if (get_move_b_list[k] == "left")
                    //            //{
                    //            //    move_b.Add(new int[2] { -1, 0 });
                    //            //}
                    //            //k++;
                    //        } while (true);
                    //    }
                    //    i = goal;
                    //}
                    //else
                    //{
                    //    if (get_move_b_list[i] == "up")
                    //    {
                    //        move_b.Add(new int[2] { 0, -1 });
                    //    }
                    //    else if (get_move_b_list[i] == "down")
                    //    {
                    //        move_b.Add(new int[2] { 0, 1 });
                    //    }
                    //    else if (get_move_b_list[i] == "right")
                    //    {
                    //        move_b.Add(new int[2] { 1, 0 });
                    //    }
                    //    else if (get_move_b_list[i] == "left")
                    //    {
                    //        move_b.Add(new int[2] { -1, 0 });
                    //    }
                    //}
                    #endregion
                    Func.Move(Move_B, Get_Input_B[i]);
                }

            }

            string[] Get_Input_Main = this.listBox_B.Items.Cast<string>().ToArray();
            Get_Input_Main = exchange_move(Get_Input_Main, Get_Input_Main.Length);
            List<string> Move_Main_List = new List<string>(Get_Input_Main);
            var move = new List<int[]>();

            if (Get_Input_Main.Length != 0)
            {
                for (int i = 0; i < Move_Main_List.Count; i++)
                {
                    (move, i) = Func.ForLoop(Move_Main_List,Move_A,Move_B, i);
                    #region 削除候補
                    //if (get_move_main[i].StartsWith("for"))
                    //{
                    //    int start = i + 1;
                    //    int trial = int.Parse(Regex.Replace(get_move_main[i], @"[^0-9]", ""));

                    //    int goal = 0; //後で設定

                    //    for (int j = 0; j < trial; j++)
                    //    {
                    //        int k = start;
                    //        do
                    //        {
                    //            if (k >= get_move_main.Length)
                    //            {
                    //                MessageBox.Show("「反復魔法」と「反復魔法おわり」はセットで使ってください");
                    //                return new List<int[]>();
                    //            }
                    //            if (get_move_main[k].StartsWith("for")) //二重ループ
                    //            {
                    //                int trial2 = int.Parse(Regex.Replace(get_move_main[k], @"[^0-9]", ""));
                    //                for (int l = 0; l < trial2; l++)
                    //                {
                    //                    k = start + 1;
                    //                    if (get_move_main[k] == "endfor") break;
                    //                    else
                    //                    {
                    //                        Func.Move(move, get_move_main[k]);
                    //                        k++;
                    //                    }
                    //                    //do
                    //                    //{
                    //                    //    if (get_move_main[k] == "endfor")
                    //                    //    {
                    //                    //        break;
                    //                    //    }

                    //                    //    else if (get_move_main[k] == "up")
                    //                    //    {
                    //                    //        move.Add(new int[2] { 0, -1 });
                    //                    //    }
                    //                    //    else if (get_move_main[k] == "down")
                    //                    //    {
                    //                    //        move.Add(new int[2] { 0, 1 });
                    //                    //    }
                    //                    //    else if (get_move_main[k] == "right")
                    //                    //    {
                    //                    //        move.Add(new int[2] { 1, 0 });
                    //                    //    }
                    //                    //    else if (get_move_main[k] == "left")
                    //                    //    {
                    //                    //        move.Add(new int[2] { -1, 0 });
                    //                    //    }
                    //                    //    else if (get_move_main[k] == "A")
                    //                    //    {
                    //                    //        move.AddRange(move_a);
                    //                    //    }
                    //                    //    else if (get_move_main[k] == "B")
                    //                    //    {
                    //                    //        move.AddRange(move_b);
                    //                    //    }
                    //                    //    k++;
                    //                    //} while (true);
                    //                }
                    //            }
                    //            else if (get_move_main[k] == "endfor")
                    //            {
                    //                goal = k;
                    //                break;
                    //            }
                    //            else
                    //            {
                    //                Func.Move(move, get_move_main[k]);
                    //                k++;
                    //            }
                    //            //else if (get_move_main[k] == "up")
                    //            //{
                    //            //    move.Add(new int[2] { 0, -1 });
                    //            //}
                    //            //else if (get_move_main[k] == "down")
                    //            //{
                    //            //    move.Add(new int[2] { 0, 1 });
                    //            //}
                    //            //else if (get_move_main[k] == "right")
                    //            //{
                    //            //    move.Add(new int[2] { 1, 0 });
                    //            //}
                    //            //else if (get_move_main[k] == "left")
                    //            //{
                    //            //    move.Add(new int[2] { -1, 0 });
                    //            //}
                    //            //else if (get_move_main[k] == "A")
                    //            //{
                    //            //    move.AddRange(move_a);
                    //            //}
                    //            //else if (get_move_main[k] == "B")
                    //            //{
                    //            //    move.AddRange(move_b);
                    //            //}
                    //            //k++;
                    //        } while (true);
                    //    }
                    //    i = goal;
                    //}

                    //else
                    //{
                    //    if (get_move_main[i] == "up")
                    //    {
                    //        move.Add(new int[2] { 0, -1 });
                    //    }
                    //    else if (get_move_main[i] == "down")
                    //    {
                    //        move.Add(new int[2] { 0, 1 });
                    //    }
                    //    else if (get_move_main[i] == "right")
                    //    {
                    //        move.Add(new int[2] { 1, 0 });
                    //    }
                    //    else if (get_move_main[i] == "left")
                    //    {
                    //        move.Add(new int[2] { -1, 0 });
                    //    }
                    //    else if (get_move_main[i] == "A")
                    //    {
                    //        move.AddRange(move_a);
                    //    }
                    //    else if (get_move_main[i] == "B")
                    //    {
                    //        move.AddRange(move_b);
                    //    }
                    //}
                    #endregion
                    if (int.Parse(Move_Main_List[i]) < 4)
                    {
                        Func.Move(move, Move_Main_List[i]);
                    }
                    else if (Move_Main_List[i] == "A") move.AddRange(Move_A);
                    else move.AddRange(Move_B);

                }

            }
            return move;
        }

        public string[] exchange_move(string[] get_move, int l)     //矢印変換
        {    
            List<string> newget_move = get_move.ToList();
            for (int i = 0; i < l; i++)
            {
                if (newget_move[i] == "↑")
                {
                    newget_move[i] = "0";          // up → 0と変換
                }
                if (newget_move[i] == "→")
                {
                    newget_move[i] = "1";          // right → 1と変換
                }
                if (newget_move[i] == "↓")
                {
                    newget_move[i] = "2";          // down → 2と変換
                }
                if (newget_move[i] == "←")
                {
                    newget_move[i] = "3";          // left → 3と変換
                }
                if (newget_move[i].StartsWith("反復魔法 ("))
                {
                    string str_num = Regex.Replace(newget_move[i], @"[^0-9]", "");
                    int num = int.Parse(str_num);
                    newget_move[i] = "for (" + (num % 10).ToString() + ")";
                }
                if (newget_move[i].StartsWith("反復魔法"))
                {
                    newget_move[i] = "endfor";
                }
                if (newget_move[i] == "Aの術")
                {
                    newget_move[i] = "A";
                }
                if (newget_move[i] == "Bの術")
                {
                    newget_move[i] = "B";
                }
            }
            return newget_move.ToArray();
        }

        public bool Colision_detection(int x, int y, int[,] Map, List<int[]> move)
        {
            int max_x = Map.GetLength(0);
            int max_y = Map.GetLength(1);

            int new_x = x + move[0][0];
            int new_y = y + move[0][1];

            if (new_x <= 0 || (max_x - new_x) <= 1 || new_y <= 0 || (max_y - new_y) <= 1) return false;
            else if (Map[new_y, new_x] == 0) return false;
            else
            {
                //move.RemoveAt(0);
                return true;
            }
        }

        Image Ninja_Image(int x, int y, int steps, bool jump, Image Ninja)  //動きにあわせてキャラの画像を返す
        {
            int a = steps % 4;//歩き差分を識別
            if (a == 1)
            {
                if (x == -1) Ninja = Image.FromFile("忍者_左面_右足.png");
                if (x == 1) Ninja = Image.FromFile("忍者_右面_右足.png");
                if (y == -1) Ninja = Image.FromFile("忍者_背面_右足.png");
                if (y == 1) Ninja = Image.FromFile("忍者_正面_右足.png");
            }
            else if (a == 3)
            {
                if (x == -1) Ninja = Image.FromFile("忍者_左面_左足.png");
                if (x == 1) Ninja = Image.FromFile("忍者_右面_左足.png");
                if (y == -1) Ninja = Image.FromFile("忍者_背面_左足.png");
                if (y == 1) Ninja = Image.FromFile("忍者_正面_左足.png");

            }
            else
            {
                if (x == -1) Ninja = Image.FromFile("忍者_左面.png");
                if (x == 1) Ninja = Image.FromFile("忍者_右面.png");
                if (y == -1) Ninja = Image.FromFile("忍者_背面.png");
                if (y == 1) Ninja = Image.FromFile("忍者_正面.png");
            }


            if (jump)
            {
                if (x == -1) Ninja = Image.FromFile("忍者_左面_ジャンプ.png");
                if (x == 1) Ninja = Image.FromFile("忍者_右面_ジャンプ.png");
                if (y == -1) Ninja = Image.FromFile("忍者_背面_ジャンプ.png");
                if (y == 1) Ninja = Image.FromFile("忍者_正面_ジャンプ.png");
            }
            return Ninja;
        }

        public void SquareMovement(int x, int y, int[,] Map, List<int[]> move)
        {
            Graphics g2 = Graphics.FromImage(bmp2);
            cell_length = pictureBox1.Width / 12;
            if (move.Count == 0)  return;

            List<int[]> move_copy = new List<int[]>(move);
            
            bool jump = false;
            bool move_floor = false;
            int waittime = 250; //ミリ秒
            count_walk = 1;//何マス歩いたか、歩き差分用

            (int, int) place_update(int a, int b, List<int[]> move_next)
            {
                x += move_next[0][0];
                y += move_next[0][1];
                x_now = x;
                y_now = y;
                g2.Clear(Color.Transparent);
                return (x_now, y_now);
            }

            void DrawCharacter(int a, int b, ref Image character_me, string filePath = "忍者_正面.png")
            {
                character_me = Image.FromFile(filePath);
                g2.DrawImage(character_me, a * cell_length - extra_length, b * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
            }

            (int,int) draw_move(int a,int b,ref List<int[]> move_next, string filePath = "忍者_正面.png")
            {
                (x_now, y_now) = place_update(a, b, move_next);
                character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], count, jump, character_me);
                DrawCharacter(x_now, y_now, ref character_me);
                this.Invoke((MethodInvoker)delegate
                {
                    // pictureBox2を同期的にRefreshする
                    pictureBox2.Refresh();
                });
                return (x_now, y_now);
            }
            while (true)
            {
                if (move_copy.Count > 0)
                {
                    if (!Colision_detection(x, y, Map, move_copy) && !jump)
                    {
                        //忍者を動かしてからミスの表示を出す
                        (x_now, y_now) = draw_move(x, y, ref move_copy);
                        //x += move_copy[0][0];
                        //y += move_copy[0][1];
                        //x_now = x;
                        //y_now = y;
                        //g2.Clear(Color.Transparent);

                        //character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], count, jump, character_me);
                        //DrawCharacter(x, y, ref character_me);

                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        //ステージごとにゴールのキャラを変えたい
                        //g2.DrawImage(goal_obj(_stageName), x_goal * cell_length - extra_length, y_goal * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);

                        //pictureBoxの中身を塗り替える
                        //this.Invoke((MethodInvoker)delegate
                        //{
                        //    // pictureBox2を同期的にRefreshする
                        //    pictureBox2.Refresh();
                        //});
                        resetStage("miss_out");
                        //character_me = Image.FromFile("忍者_正面.png");
                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        DrawCharacter(x, y, ref character_me);
                        break;
                    }
                    if (jump && Map[y + move_copy[0][1] * 2, x + move_copy[0][0] * 2] == 8) //jumpの時着地先が木の場合、ゲームオーバー
                    {
                        (x_now, y_now) = draw_move(x, y, ref move_copy);
                        //x += move_copy[0][0];
                        //y += move_copy[0][1];
                        //x_now = x;
                        //y_now = y;
                        //g2.Clear(Color.Transparent);
                        //ステージごとにゴールのキャラを変えたい
                        //g2.DrawImage(goal_obj(_stageName), x_goal * cell_length - extra_length, y_goal * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);

                        //忍者の動きに合わせて向きが変わる
                        //character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], count, jump, character_me);

                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        //DrawCharacter(x, y, ref character_me);

                        //pictureBox2.Refresh();
                        Thread.Sleep(waittime);
                        //bool J = false;
                        //x += move_copy[0][0];
                        //y += move_copy[0][1];
                        //x_now = x;
                        //y_now = y;
                        //g2.Clear(Color.Transparent);
                        (x_now, y_now) = draw_move(x, y, ref move_copy);

                        //ステージごとにゴールのキャラを変えたい
                        //g2.DrawImage(goal_obj(_stageName), x_goal * cell_length - extra_length, y_goal * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        //忍者の動きに合わせて向きが変わる
                        //character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], count, J, character_me);
                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        //pictureBox2.Refresh();

                        resetStage("miss_tree");
                        character_me = Image.FromFile("忍者_正面.png");
                        g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        break;
                    }
                    if (count_walk > 50) //無限ループ対策
                    {
                        resetStage("miss_countover");
                        DrawCharacter(x, y, ref character_me);
                        //character_me = Image.FromFile("忍者_正面.png");
                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                        break;
                    }
                }
                else
                {
                    DrawCharacter(x, y, ref character_me);
                    //character_me = Image.FromFile("忍者_正面.png");
                    //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 *    extra_length);
                    break;
                }

                //jumpでない時移動先が木の場合、木の方向には進めない
                if (!jump && Map[y + move_copy[0][1], x + move_copy[0][0]] == 8)
                {
                    DrawCharacter(x, y, ref character_me);
                    //character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], count_walk, jump, character_me);
                    //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                    //move_copy[0] = new int[] { 0, 0 };
                    move_copy.RemoveAt(0);
                    //500ミリ秒=0.5秒待機する
                    Thread.Sleep(waittime);
                    continue;
                }

                (x_now, y_now) = draw_move(x, y, ref move_copy);
                //x += move_copy[0][0];
                //y += move_copy[0][1];
                //x_now = x;
                //y_now = y;
                //g2.Clear(Color.Transparent);

                //ステージごとにゴールのキャラを変えたい
                //g2.DrawImage(goal_obj(_stageName), x_goal * cell_length - extra_length, y_goal * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);

                //忍者の動きに合わせて向きが変わる
                //character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], count_walk, jump, character_me);
                //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);

                //Thread.Sleep(waittime);//マスの間に歩く差分を出そうとしたけど。。。
                //g2.Clear(Color.Transparent);
                //character_me = Ninja_Image(move_copy[0][0], move_copy[0][1], stepCount, jump, character_me);
                //g2.DrawImage(character_me, x * cell_length + move_copy[0][0] * cell_length / 2, y * cell_length + move_copy[0][1] * cell_length / 2, cell_length, cell_length);


                //pictureBoxの中身を塗り替える
                //this.Invoke((MethodInvoker)delegate
                //{
                //    // pictureBox2を同期的にRefreshする
                //    pictureBox2.Refresh();
                //});

                if (Map[y, x] == 101 && Map[y - move_copy[0][1], x - move_copy[0][0]] != 3)
                {
                    DrawCharacter(x, y, ref character_me);
                    //character_me = Image.FromFile("忍者_正面.png");
                    //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                    break;
                }

                //移動先が氷の上なら同じ方向にもう一回進む
                if (!jump && Map[y, x] == 2)
                {
                    //500ミリ秒=0.5秒待機する
                    Thread.Sleep(waittime);
                    continue;
                }

                //移動先がジャンプ台なら同じ方向に二回進む（１個先の障害物は無視）
                if (Map[y, x] == 3 || jump)
                {
                    if (move_floor)
                    {
                        move_floor = false;
                        move_copy.RemoveAt(0);
                    }

                    jump = !jump;
                    //if (jump) //次の移動で着地
                    //{
                    //    jump = false;
                    //}
                    //else //ジャンプ台の上（次の移動でジャンプ）
                    //{
                    //    jump = true;
                    //}

                    //500ミリ秒=0.5秒待機する
                    Thread.Sleep(waittime);
                    continue;
                }

                switch (Map[y, x])
                {
                    case 4: move_copy[0] = new int[2] { 0, -1 }; break;
                    case 5: move_copy[0] = new int[2] { 1, 0 }; break;
                    case 6: move_copy[0] = new int[2] { 0, 1 }; break;
                    case 7: move_copy[0] = new int[2] { -1, 0 }; break;
                }
                ////上に移動するマスを踏んだ場合1つ上に進む
                //if (Map[y, x] == 4)
                //{
                //    move_copy[0] = new int[2] { 0, -1 };
                //    Thread.Sleep(waittime);
                //    continue;
                //}

                ////右に移動するマスを踏んだ場合1つ右に進む
                //if (Map[y, x] == 5)
                //{
                //    move_copy[0] = new int[2] { 1, 0 };
                //    Thread.Sleep(waittime);
                //    continue;
                //}

                ////下に移動するマスを踏んだ場合1つ下に進む
                //if (Map[y, x] == 6)
                //{
                //    move_copy[0] = new int[2] { 0, 1 };
                //    Thread.Sleep(waittime);
                //    continue;
                //}

                ////左に移動するマスを踏んだ場合1つ左に進む
                //if (Map[y, x] == 7)
                //{
                //    move_copy[0] = new int[2] { -1, 0 };
                //    Thread.Sleep(waittime);
                //    continue;
                //}

                move_copy.RemoveAt(0);
                if (move_copy.Count == 0)//動作がすべて終了した場合
                {
                    if (x_now != x_goal || y_now != y_goal)
                    {
                        resetStage("miss_end");
                        Thread.Sleep(300);
                        DrawCharacter(x, y, ref character_me);
                        //character_me = Image.FromFile("忍者_正面.png");
                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                    }
                    break;
                }

                //500ミリ秒=0.5秒待機する
                Thread.Sleep(waittime);
                count_walk++;
            }
        }
        #endregion
    }
}
