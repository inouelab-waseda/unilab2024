﻿using System;
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
using System.Drawing.Drawing2D;

namespace unilab2024
{
    public partial class Stage : Form
    {
        public Stage()
        {
            InitializeComponent();
            //this.AllowDrop = true;
            //this.DragDrop += new DragEventHandler(ListBox_DragDrop);          //Form全体にDrop可能にする
            //this.DragEnter += new DragEventHandler(ListBox_DragEnter);

            this.KeyDown += new KeyEventHandler(Stage_KeyDown);
            this.KeyPreview = true;

            #region ボタン表示(開発中)
            uiButtonObject_up.Size = new Size(80, 80);
            uiButtonObject_left.Size = new Size(80, 80);
            uiButtonObject_right.Size = new Size(80, 80);
            uiButtonObject_down.Size = new Size(80, 80);
            #endregion

            //pictureBoxの設定
            pictureBox2.Parent = pictureBox1;
            //pictureBox1.Location = new Point(600, 50);
            pictureBox2.Location = new Point(0, 0);
            pictureBox1.ClientSize = new Size(684, 684);
            pictureBox2.ClientSize = new Size(684, 684);
            pictureBox2.BackColor = Color.Transparent;

            bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            bmpConv = new Bitmap(pictureBoxConv.Width, pictureBoxConv.Height);
            //bmp4 = new Bitmap(pictureBox4.Width, pictureBox4.Height);
            pictureBox1.Image = bmp1;
            pictureBox2.Image = bmp2;
            pictureBoxConv.Image = bmpConv;
            //pictureBox4.Image = bmp4;
            //this.Load += Stage_Load;
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
        Bitmap bmp1, bmp2, bmpConv, bmp4;

        Brush goalBackgroundColor = new SolidBrush(Color.Yellow);
        Brush startBackgroundColor = new SolidBrush(Color.Blue);

        Image character_me = Dictionaries.Img_DotPic["魔法使いサンプル"];

        public static int[,] map = new int[12, 12]; //map情報
        public static int x_start; //スタート位置ｘ
        public static int y_start; //スタート位置ｙ
        public static int x_goal; //ゴール位置ｘ
        public static int y_goal; //ゴール位置ｙ
        public static int x_now; //現在位置ｘ
        public static int y_now; //現在位置 y
        public static int extra_length = 7;
        public static int cell_length ;
        public static int For_count = 1; //for文のループ回数を保存
        public static bool isEndfor = true; //forの最後が存在するか→ない場合はError

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

        //public static List<Conversation> Conversations = new List<Conversation>();  //会話文を入れるリスト
        List<Conversation> StartConv; 
        List<Conversation> EndConv;
        bool isStartConv;
        int convIndex;
        public Graphics g1;
        public Graphics g2;
        #endregion


        public void Stage_Load(object sender, EventArgs e)        //StageのFormの起動時処理
        {
            //button5.Visible = false;
            //_stageName = "stage2-3";
            string stageName = "stage"+_worldNumber + "-" + _level;
            map = CreateStage(stageName); //ステージ作成

            grade = Regex.Replace(stageName, @"[^0-9]", "");
            int chapter_num = int.Parse(grade) / 10;        

            #region リストボックス・ボタンの設定
            // 1行文の高さ
            int ItemHeight = 20;
            listBox_Input.ItemHeight = ItemHeight;
            //listBox_Options.ItemHeight = ItemHeight;
            listBox_A.ItemHeight = ItemHeight;
            listBox_B.ItemHeight = ItemHeight;

            int element_height = listBox_Input.ItemHeight;

            // それぞれの枠の高さ
            int height_LB_Input = 5;
            int height_LB_A = 5;
            int height_LB_B = 5;

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

            limit_LB_Input = 8;
            limit_LB_A = 8;
            limit_LB_B = 8;
            cell_length = pictureBox1.Width / 12;

            if (height_LB_Input == 1)
            {
                //listBox_Input.Visible = false;
                
                //listBox_SelectAB.Items.Remove("A");
                button_ResetInput.Visible = false;
                button_ResetInput.Enabled = false;

            }

            if (height_LB_A == 1)
            {
                //listBox_A.Visible = false;
                //label_B.Visible = false;
                //listBox_B.Items.Remove("B");
                button_ResetInput.Visible = false;
                button_ResetInput.Enabled = false;
            }

            if (height_LB_Input == 1 && height_LB_A == 1)
            {
                //listBox_SelectAB.Visible = false;
            }

            //hint = null;
            //hint_character = null;
            //hint_name = null;

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
            //listBox_Input.SelectionMode = SelectionMode.One;
            //listBox_Input.DragEnter += new DragEventHandler(ListBox_DragEnter);
            //listBox_Input.DragDrop += new DragEventHandler(ListBox_DragDrop);
            //listBox_Options.MouseDown += new MouseEventHandler(ListBox_MouseDown);
            //listBox_A.SelectionMode = SelectionMode.One;
            //listBox_A.DragEnter += new DragEventHandler(ListBox_DragEnter);
            //listBox_A.DragDrop += new DragEventHandler(ListBox_DragDrop);
            //listBox_B.SelectionMode = SelectionMode.One;
            //listBox_B.DragEnter += new DragEventHandler(ListBox_DragEnter);
            //listBox_B.DragDrop += new DragEventHandler(ListBox_DragDrop);
            //listBox_SelectAB.MouseDown += new MouseEventHandler(ListBox_MouseDown);

            //    //ヒントを教えるキャラのアイコンを表示
            //    Graphics g3 = Graphics.FromImage(bmp3);
            //    Bitmap bmp = new Bitmap(1, 1);
            //    bmp.SetPixel(0, 0, Color.White);
            //    g3.DrawImage(bmp, 0, 0, bmp3.Height - 1, bmp3.Height - 1);
            //    g3.DrawRectangle(Pens.Black, 0, 0, bmp3.Height - 1, bmp3.Height - 1);
            //    g3.Dispose();

            //チュートリアルステージでは、マップに戻るボタンを消す。ゴールしたら見える
            //if (stageName == "stage1-1")
            //{
            //    button_ToMap.Visible = false;
            //}
            ////for文をステージ1-1,1-2で消す
            //if (stageName == "stage1-1" || stageName == "stage1-2")
            //{
            //    listBox_options.Items.Remove("連チャンの術 (1)");
            //    listBox_options.Items.Remove("連チャンの術おわり");
            //}

            //ストーリー強制視聴
            //listBox_Options.Visible = false;
            //listBox_SelectAB.Visible = false;
            button_Hint.Visible = false;
            button_Retry.Visible = false;
            button_ToMap.Visible = true;
            label_Error.Visible = false;
            label_Result.Visible = false;
            //drawConversation();
            #endregion

            string convFileName = "Story_Chapter" + _worldNumber + "-" + _level + ".csv";
            (StartConv, EndConv) = Func.LoadStories(convFileName); //会話読み込み
            pictureBoxConv.Visible = true;
            pictureBoxConv.Enabled = true;
            pictureBoxConv.BringToFront();
            convIndex = 0;
            isStartConv = true;
            drawConversations(isStartConv);
        }

        #region リセット関連
        public static void ResetListBox(ListBox listbox)   //ListBoxの中身消去
        {
            if (listbox.SelectedIndex > -1)
            {
                listbox.Items.RemoveAt(listbox.SelectedIndex);
            }
            else
            {
                listbox.Items.Clear();
            }
        }
        private void button_ResetInput_Click(object sender, EventArgs e)        //起動部分リセット
        {
            ResetListBox(listBox_Input);                    //Program.CSに処理記載
        } 
        private void button_ResetA_Click(object sender, EventArgs e)           //Aの魔法リセット
        {
            ResetListBox(listBox_A);
        }       
        private void button_ResetB_Click(object sender, EventArgs e)           //Bの魔法リセット
        {
            ResetListBox(listBox_B);
        }
        public void resetStage(string type) // ステージリセットまとめ
        {
            switch (type)
            {
                case "quit":
                    Func.CreateStageSelect(this, _worldName, _worldNumber);
                    break; ;
                case "miss_out":
                    label_Error.Text = "そこは止まれないよ！やり直そう！";
                    label_Error.Visible = true;
                    Thread.Sleep(300);
                    button_Retry.Visible = true;
                    button_Retry.Enabled = true;
                    miss_count += 1;
                    break;
                case "miss_countover":
                    label_Error.Text = "これ以上は移動できない！やり直そう！";
                    label_Error.Visible = true;
                    button_Retry.Visible = true;
                    button_Retry.Enabled = true;
                    Thread.Sleep(300);
                    miss_count += 1;
                    break;
                case "miss_end":
                    label_Error.Text = "ゴールまで届いてないね！やり直そう！";
                    label_Error.Visible = true;
                    button_Retry.Visible = true;
                    button_Retry.Enabled = true;
                    Thread.Sleep(300);
                    miss_count += 1;
                    break;
                case "retry":
                    //初期位置に戻す
                    x_now = x_start;
                    y_now = y_start;

                    //初期位置に書き換え
                    Graphics g2 = Graphics.FromImage(bmp2);
                    g2.Clear(Color.Transparent);
                    //int cell_length = pictureBox1.Width / 12;
                    //character_me = Image.FromFile("忍者_正面.png");
                    g2.DrawImage(Dictionaries.Img_DotPic["魔法使いサンプル"], x_now * cell_length - extra_length, y_now * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);

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
                    isEndfor = true;
                    label_Error.Text = "ミス！";
                    label_Error.Visible = false;
                    label_Result.Visible = false;
                    
                    break;
                default: break;
            }
            #region 削除候補
            //if (type == "quit")
            //{
            //    Func.CreateStageSelect(this, _worldName, _worldNumber);
            //    return;
            //}

            //// 曇から落ちたミス
            //else if (type == "miss_out")
            //{
            //    label_Error.Text = "そこは止まれないよ！やり直そう！";
            //    label_Error.Visible = true;
            //    Thread.Sleep(300);
            //    miss_count += 1;
            //}

            ////木に刺されたミス
            //else if (type == "miss_tree")
            //{
            //    label_Error.Text = "木に刺された！やり直そう！";
            //    label_Error.Visible = true;
            //    Thread.Sleep(300);
            //    miss_count += 1;
            //}

            ////無限ループの時のミス
            //else if (type == "miss_countover")
            //{
            //    label_Error.Text = "これ以上は移動できない！やり直そう！";
            //    label_Error.Visible = true;
            //    Thread.Sleep(300);
            //    miss_count += 1;
            //}

            ////止まった時ゴール到着してないミス
            //else if (type == "miss_end")
            //{
            //    label_Error.Text = "ゴールまで届いてないね！やり直そう！";
            //    label_Error.Visible = true;
            //    Thread.Sleep(300);
            //    miss_count += 1;
            //}

            //// リトライボタン
            //else if (type == "retry")
            //{
            //    //初期位置に戻す
            //    x_now = x_start;
            //    y_now = y_start;

            //    //初期位置に書き換え
            //    Graphics g2 = Graphics.FromImage(bmp2);
            //    g2.Clear(Color.Transparent);
            //    int cell_length = pictureBox1.Width / 12;
            //    //character_me = Image.FromFile("忍者_正面.png");
            //    //g2.DrawImage(character_me, Global.x_now * cell_length - Global.extra_length, Global.y_now * cell_length - 2 * Global.extra_length, cell_length + 2 * Global.extra_length, cell_length + 2 * Global.extra_length);

            //    //g2.DrawImage(goal_obj(_stageName), Global.x_goal * cell_length - Global.extra_length, Global.y_goal * cell_length - 2 * Global.extra_length, cell_length + 2 * Global.extra_length, cell_length + 2 * Global.extra_length);
            //    this.Invoke((MethodInvoker)delegate
            //    {
            //        // pictureBox2を同期的にRefreshする
            //        pictureBox2.Refresh();
            //    });

            //    //初期設定に戻す
            //    button_Start.Visible = true;
            //    button_Start.Enabled = true;
            //    label_Error.Visible = false;
            //    count = 0;
            //    miss_count = 0;
            //    label_Error.Text = "ミス！";
            //    label_Error.Visible = false;
            //    label_Result.Visible = false;
            //    label_Info.Visible = false;
            //}
            #endregion
        }
        #endregion

        #region ボタン押下時処理(UI開発中)
        private void button_Start_Click(object sender, EventArgs e)  //出発ボタン押下時処理
        {
            button_Start.Visible = false;
            button_Start.Enabled = false;
            label_Error.Visible = false;
            move = Movement(); //ユーザーの入力を読み取る
            if (!isEndfor)
            {
                resetStage("retry");
                return;
            }
            SquareMovement(x_now, y_now, map, move); //キャラ動かす
            count += 1;
            if (x_goal == x_now && y_goal == y_now)
            {
                label_Result.Text = "クリア！！";
                label_Result.Visible = true;
                button_ToMap.Enabled = true;
                button_Retry.Enabled = false;
                button_ToMap.Visible = true;
                button_ToMap.Location = new Point(800, 600);
                button_ToMap.Size = new Size(200, 50);
                
                //pictureBox4.Visible = false;
                //pictureBox5.Visible = false;
                //pictureBox6.Visible = false;
                //pictureBox7.Visible = false;
                ClearCheck.IsCleared[_worldNumber, _level] = true;    //クリア状況管理
                if (_level == 3)
                {
                    ClearCheck.IsCleared[_worldNumber,0] = true;
                    switch (_worldNumber)
                    {
                        case 1:
                        case 2:
                        case 3:
                            ClearCheck.IsButtonEnabled[_worldNumber + 1, 0] = true;
                            ClearCheck.IsButtonEnabled[_worldNumber + 1, 1] = true;
                            break;
                        case 4:
                            for (int i = _worldNumber + 1; i < (int)ConstNum.numWorlds; i++)
                            {
                                for (int j = 0; j <= 1; j++)
                                {
                                    ClearCheck.IsButtonEnabled[i, j] = true;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    ClearCheck.IsButtonEnabled[_worldNumber, _level + 1] = true;
                }
            }
            //else
            //{
            //    resetStage("miss_end");
            //}
        }

        private void button_Retry_Click(object sender, EventArgs e)  //リトライボタン押下時処理
        {
            resetStage("retry");
        }

        private void button_ToMap_Click(object sender, EventArgs e)  //マップに戻るボタン押下時処理
        {
            resetStage("quit");
        }

        void uiButtonObject_up_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("↑");
        }
        void uiButtonObject_left_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("←");
        }
        void uiButtonObject_right_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("→");
        }
        void uiButtonObject_down_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("↓");
        }
        void uiButtonObject_A_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("A");
        }
        void uiButtonObject_B_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("B");
        }
        void uiButtonObject_for_Click(object sender, EventArgs e)
        {
            label_ForCount.Visible = true;
            label_ForCount.Text = "ループ回数を入力してね!";
            textBox_ForCount.Visible = true;
            textBox_ForCount.Focus();
            //listBox_Input.Items.Add($"反復魔法({For_count})");
        }
        private void textBox_ForCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字キーが押された場合
            if (int.TryParse(e.KeyChar.ToString(), out For_count))
            {
                For_count = int.Parse(e.KeyChar.ToString());
                // リストボックスに "Input(数字)" 形式で追加
                listBox_Input.Items.Add($"反復魔法({For_count})");
                e.Handled = true;
                textBox_ForCount.Visible = false;
                label_ForCount.Visible = false;
            }
            else
            {
                label_ForCount.Text = "数字を入力してね!";
                e.Handled = true;
            }
        }
        void uiButtonObject_endfor_Click(object sender, EventArgs e)
        {
            listBox_Input.Items.Add("反復魔法終わり");
        }

        void Stage_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    listBox_Input.Items.Add("↑");
                    break;
                case Keys.Left:
                    listBox_Input.Items.Add("←");
                    break;
                case Keys.Right:
                    listBox_Input.Items.Add("→");
                    break;
                case Keys.Down:
                    listBox_Input.Items.Add("↓");
                    break;
                case Keys.A:
                    listBox_Input.Items.Add("A");
                    break;
                case Keys.B:
                    listBox_Input.Items.Add("B");
                    break;
            }
        }
        #endregion

        private int[,] CreateStage(string stageName)     //ステージ作成
        {
            //string stagenum = _worldNumber + "-" + _level;
            using (StreamReader sr = new StreamReader($"Map\\{stageName}.csv"))
            {
                int x;
                int y = 0;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');

                    x = 0;

                    foreach (var value in values)
                    {
                        // enum 使ったほうが分かりやすそう
                        map[x, y] = int.Parse(value);
                        switch (map[x,y])
                        {
                            case 0:
                                x_start = x;
                                y_start = y;
                                x_now = x;
                                y_now = y;
                                break;
                            case 1:
                                x_goal = x;
                                y_goal = y;
                                break;
                            default:
                                break;
                        }
                        x++;
                    }
                    y++;
                }
            }

            Graphics g1 = Graphics.FromImage(bmp1);
            Graphics g2 = Graphics.FromImage(bmp2);
            //label_Info.BackgroundImage = Image.FromFile("focus.png");


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
                    g1.DrawImage(Dictionaries.Img_Object[map[x,y]], placeX, placeY, cell_length, cell_length);
                    switch (map[x, y])
                    {
                        case 0:
                            x_start = x;
                            y_start = y;
                            x_now = x;
                            y_now = y;
                            g2.DrawImage(character_me, placeX - extra_length, placeY - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                            break;
                        case 1:
                            x_goal = x;
                            y_goal = y;
                            break;
                        default:
                            break;
                    }
                    /*switch (map[y, x]) //配列に画像を保存し表示で十分
                    {

                        case 0:
                            g1.DrawImage(img_noway, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 1:
                            g1.DrawImage(img_way, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 2:
                            g1.DrawImage(img_ice, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 3:
                            g1.DrawImage(img_jump, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 4:
                            ImageAnimator.UpdateFrames(animatedImage_up);
                            g1.DrawImage(animatedImage_up, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 5:
                            ImageAnimator.UpdateFrames(animatedImage_right);
                            g1.DrawImage(animatedImage_right, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 6:
                            ImageAnimator.UpdateFrames(animatedImage_down);
                            g1.DrawImage(animatedImage_down, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 7:
                            ImageAnimator.UpdateFrames(animatedImage_left);
                            g1.DrawImage(animatedImage_left, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 8:
                            g1.DrawImage(img_tree, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 20:
                            g1.DrawImage(cloud_ul, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 21:
                            g1.DrawImage(cloud_left, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 22:
                            g1.DrawImage(cloud_bl, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 23:
                            g1.DrawImage(cloud_bottom, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 24:
                            g1.DrawImage(cloud_br, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 25:
                            g1.DrawImage(cloud_right, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 26:
                            g1.DrawImage(cloud_ur, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 27:
                            g1.DrawImage(cloud_upside, placeX, placeY, Global.cell_length, Global.cell_length);
                            break;
                        case 100:
                            g1.FillRectangle(startBackgroundColor, placeX, placeY, Global.cell_length, Global.cell_length);
                            Global.x_start = x;
                            Global.y_start = y;
                            Global.x_now = x;
                            Global.y_now = y;
                            g2.DrawImage(character_me, placeX - Global.extra_length, placeY - 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length);
                            break;
                        case 101:
                            g1.FillRectangle(goalBackgroundColor, placeX, placeY, Global.cell_length, Global.cell_length);
                            //ステージごとにゴールのキャラを変えたい
                            g2.DrawImage(goal_obj(_stageName), placeX - Global.extra_length, placeY - 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length, Global.cell_length + 2 * Global.extra_length);
                            Global.x_goal = x;
                            Global.y_goal = y;
                            break;
                    }*/  //削除候補
                }
            }
            this.Invoke((MethodInvoker)delegate
            {
                // pictureBox2を同期的にRefreshする
                pictureBox2.Refresh();
            });
            return map;
        }

        #region 動作関連

        /// <summary>
        /// 入力(direction)に応じてマップ上で動きを実装
        /// </summary>
        /// <param name="movelist">キャラクターがどう動くかのリスト</param>
        /// <param name="direction">動く方向</param>
        public void MoveTo(List<int[]> movelist, string direction)    //指定方向に移動する関数
        {
            int Direction_Index = 10;
            switch (direction)
            {
                case "↑":
                    Direction_Index = 0;
                    break;
                case "→":
                    Direction_Index = 1;
                    break;
                case "↓":
                    Direction_Index = 2;
                    break;
                case "←":
                    Direction_Index = 3;
                    break;
                default:
                    break;
            }
            int[][] move = new int[4][];       // up,right.down,leftの順
            move[0] = new int[] { 0, -1 };     //up
            move[1] = new int[] { 1, 0 };      //right
            move[2] = new int[] { 0, 1 };      //down 
            move[3] = new int[] { -1, 0 };     //left
            if (Direction_Index < 4) movelist.Add(move[Direction_Index]);
        }

        /// <summary>
        /// ユーザーの入力Listを作成する関数(ほかの関数を呼び出す場合などの処理)
        /// </summary>
        /// <param name="A">自作関数Aの動き</param>
        /// <param name="B">自作関数Bの動き</param>
        /// <param name="Move_Input">出力先の動き</param>
        /// <returns></returns>
        public List<string> MakeMoveList(string[] A, string[] B, List<string> Move_Input)     //入力A・B(Move_Input)が再起の場合の処理
        {
            List<string> Return_List = new List<string>();          //戻り値用のList
            for (int i = 0; i < Move_Input.Count; i++)　　　　　　　//A・Bの中身だけ探索
            {

                if (Move_Input[i] == "B")
                {
                    Return_List.AddRange(B);                        //Bが含まれていたらBの操作を追加

                }
                else if (Move_Input[i] == "A")
                {
                    Return_List.AddRange(A);                        //Aが含まれていたらAの操作を追加

                }
                else
                {
                    Return_List.Add(Move_Input[i]);                 //そのまま追加
                }
            }
            return Return_List;
        }

        /// <summary>
        /// for文での処理を行う関数、多重のfor文の場合にも対応
        /// </summary>
        /// <param name="move">キャラクターのマップ上での動き</param>
        /// <param name="Move_Input">ユーザーの入力した方向</param>
        /// <param name="Move_A">自作関数Aを呼び出す場合の処理</param>
        /// <param name="Move_B">自作関数Bを呼び出す場合の処理</param>
        /// <param name="i"></param>
        /// <returns></returns>
        public (List<int[]>, int a) ForLoop(List<int[]> move, List<string> Move_Input, List<int[]> Move_A, List<int[]> Move_B, int i)     //for文処理
        {
            int trial;                                                                                                //反復回数
            int Now;                                                                                                  //入力したListのうち何番目の処理か
            //List<int[]> Return_List = new List<int[]>();                                                              //出力の配列(動きを[x,y]として保存)
            int Return_Num = i;                                                                                       //何番目までfor文処理が続いているか
            if (Move_Input[i].StartsWith("反復魔法("))
            {
                trial = int.Parse(Regex.Replace(Move_Input[i], @"[^0-9]", ""));                                       //処理回数をtrialに設定
                for (int j = 0; j < trial; j++)
                {
                    Now = i + 1;                                                                                          //for文の処理内容はi+1行目から
                    while (true)
                    {
                        if (Now >= Move_Input.Count)                                                                 //for文の終わりが存在しない場合、エラー表示
                        {
                            MessageBox.Show("「反復魔法」と「反復魔法おわり」はセットで使ってください");
                            isEndfor = false;
                            return (move, i);
                        }
                        (move, Now) = ForLoop(move, Move_Input, Move_A, Move_B, Now);                                  //二重ループの探索
                        if (Move_Input[Now] == "反復魔法終わり") break;                                                       //for文終わりが存在したら処理終了
                        else
                        {
                            if (Move_Input[Now] == "A") move.AddRange(Move_A);                            //Aの魔法が入力されている場合、Aの処理内容をListに追加
                            else if (Move_Input[Now] == "B") move.AddRange(Move_B);                        //Bの魔法の際も同様                     
                            else MoveTo(move, Move_Input[Now]);                                        //動く方向が指定されている場合、その方向への動きをListに追加
                            Now++;
                        }
                    }
                    Return_Num = Now;
                }
            }
            return (move, Return_Num);                                                                          //動きの内容(List)とどこまで処理したかを返却
        }

        /// <summary>
        /// 実際にユーザーの入力を読み取り、動きのListを作成
        /// </summary>
        /// <returns></returns>
        public List<int[]> Movement()      //動作の関数
        {
            var Move_A = new List<int[]>();                                                           //Aでの動きを保存
            var Move_B = new List<int[]>();                                                           //Bでの動きを保存
            string[] Get_Input_A = this.listBox_A.Items.Cast<string>().ToArray();                     //AのListへの入力を保存
            string[] Get_Input_B = this.listBox_B.Items.Cast<string>().ToArray();                     //BのListへの入力を保存
            //Get_Input_A = exchange_move(Get_Input_A);                             //AのListへの入力を動きに変換
            //Get_Input_B = exchange_move(Get_Input_B);                             //BのListへの入力を動きに変換
            var Move_A_List = new List<string> (Get_Input_A);
            var Move_B_List = new List<string>(Get_Input_B);

            //get_move_a_list.AddRange(get_move_a);
            //get_move_b_list.AddRange(get_move_b);

            int loop_count = 0;
            while (Move_A_List.Count <= 30 || Move_B_List.Count <= 30)
            {
                Move_A_List = MakeMoveList(Get_Input_A, Get_Input_B, Move_A_List);
                Move_B_List = MakeMoveList(Get_Input_A, Get_Input_B, Move_B_List);
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
                    (Move_A, i) = ForLoop(Move_A,Move_A_List,Move_A,Move_B, i);
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
                    MoveTo(Move_A, Get_Input_A[i]);
                }
            }

            if (Get_Input_B.Length != 0)
            {
                //string[] get_move_b = this.listBox3.Items.Cast<string>().ToArray();

                for (int i = 0; i < Move_B_List.Count; i++)
                {
                    (Move_B, i) = ForLoop(Move_B,Move_B_List, Move_A, Move_B, i);
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
                    MoveTo(Move_B, Get_Input_B[i]);
                }
            }

            string[] Get_Input_Main = this.listBox_Input.Items.Cast<string>().ToArray();
            //Get_Input_Main = exchange_move(Get_Input_Main);
            List<string> Move_Main_List = new List<string>(Get_Input_Main);
            List<int[]> move = new List<int[]>();

            if (Get_Input_Main.Length != 0)
            {
                for (int i = 0; i < Move_Main_List.Count; i++)
                {
                    (move, i) = ForLoop(move,Move_Main_List,Move_A,Move_B, i);
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
                    if (Move_Main_List[i] == "A") move.AddRange(Move_A);
                    else if(Move_Main_List[i] == "B") move.AddRange(Move_B);
                    else MoveTo(move, Move_Main_List[i]);
                }
            }
            return move;
        }

        /// <summary>
        /// 動いた先の衝突検知(入れない部分に侵入した場合)
        /// </summary>
        /// <param name="x">現在位置x座標</param>
        /// <param name="y">現在位置y座標</param>
        /// <param name="Map">ステージのマップ情報</param>
        /// <param name="move">動いた先の座標</param>
        /// <returns></returns>
        public bool Colision_detection(int x, int y, int[,] Map, List<int[]> move)
        {
            int max_x = Map.GetLength(0);
            int max_y = Map.GetLength(1);

            int new_x = x + move[0][0];
            int new_y = y + move[0][1];

            if (new_x <= 0 || (max_x - new_x) <= 1 || new_y <= 0 || (max_y - new_y) <= 1) return false;
            else if (Map[new_x,new_y] == 2) return false;
            else
            {
                //move.RemoveAt(0);
                return true;
            }
        }

        /// <summary>
        /// 動きに応じたキャラクターの画像を表示
        /// </summary>
        /// <param name="x">動きのx方向</param>
        /// <param name="y">動きのy方向</param>
        /// <param name="steps">歩きの差分(右足→左足)</param>
        /// <param name="jump">飛んでいる場合</param>
        /// <param name="Chara">出力画像へのポインタ</param>
        /// <returns></returns>

        Image Character_Image(int x, int y, int steps, bool jump, Image Chara) 
        {
            int a = steps % 2;//歩き差分を識別
            int direction = x * 10 + y;
            int type = a + 1;
            //if (a % 2 == 0) return Dictionaries.Img_DotPic["魔法使いサンプル"];   //左右図でき次第差し替え
            switch (direction)
            {
                case 10:
                    Chara = Dictionaries.Img_DotPic[$"歩き右{type}"];
                    break;
                case -10:
                    Chara = Dictionaries.Img_DotPic[$"歩き左{type}"];
                    break;
                case 1:
                    Chara = Dictionaries.Img_DotPic[$"歩き{type}"];
                    break;
                case -1:
                    Chara = Dictionaries.Img_DotPic[$"歩き後ろ{type}"];
                    break;
            }
            //if (a == 1)
            //{
            //    if (x == -1) Ninja = Image.FromFile("忍者_左面_右足.png");
            //    if (x == 1) Ninja = Image.FromFile("忍者_右面_右足.png");
            //    if (y == -1) Ninja = Image.FromFile("忍者_背面_右足.png");
            //    if (y == 1) Ninja = Image.FromFile("忍者_正面_右足.png");
            //}
            //else if (a == 3)
            //{
            //    if (x == -1) Ninja = Image.FromFile("忍者_左面_左足.png");
            //    if (x == 1) Ninja = Image.FromFile("忍者_右面_左足.png");
            //    if (y == -1) Ninja = Image.FromFile("忍者_背面_左足.png");
            //    if (y == 1) Ninja = Image.FromFile("忍者_正面_左足.png");

            //}
            //else
            //{
            //    if (x == -1) Ninja = Image.FromFile("忍者_左面.png");
            //    if (x == 1) Ninja = Image.FromFile("忍者_右面.png");
            //    if (y == -1) Ninja = Image.FromFile("忍者_背面.png");
            //    if (y == 1) Ninja = Image.FromFile("忍者_正面.png");
            //}


            //if (jump)
            //{
            //    if (x == -1) Ninja = Image.FromFile("忍者_左面_ジャンプ.png");
            //    if (x == 1) Ninja = Image.FromFile("忍者_右面_ジャンプ.png");
            //    if (y == -1) Ninja = Image.FromFile("忍者_背面_ジャンプ.png");
            //    if (y == 1) Ninja = Image.FromFile("忍者_正面_ジャンプ.png");
            //}
            return Chara;
        }

        /// <summary>
        /// 動く際の描画や処理を行う関数
        /// </summary>
        /// <param name="x">現在地のx座標</param>
        /// <param name="y">現在地のy座標</param>
        /// <param name="Map">ステージのマップ情報</param>
        /// <param name="move">動きのリスト</param>
        public void SquareMovement(int x, int y, int[,] Map, List<int[]> move)
        {
            Graphics g2 = Graphics.FromImage(bmp2);
            //cell_length = pictureBox1.Width / 12;
            if (move.Count == 0) //ゴールについていない場合(入力がない)場合のエラー処理
            {
                resetStage("miss_end");
                return;
            }

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

            void DrawCharacter(int a, int b, ref Image character_me)
            {
                //character_me = Dictionaries.Img_DotPic["魔法使いサンプル"];
                g2.DrawImage(character_me, a * cell_length - extra_length, b * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
            }

            (int,int) draw_move(int a,int b,ref List<int[]> move_next)
            {
                (x_now, y_now) = place_update(a, b, move_next);
                character_me = Character_Image(move_copy[0][0], move_copy[0][1], count_walk, jump, character_me);
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
                    //int max_x = Map.GetLength(0);
                    //int max_y = Map.GetLength(1);
                    //int new_x = x + move[0][0];
                    //int new_y = y + move[0][1];
                    //bool a = Colision_detection(x, y, Map, move_copy);
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
                    if (jump && Map[x + move_copy[0][1] * 2, y + move_copy[0][0] * 2] == 8) //jumpの時着地先が木の場合、ゲームオーバー
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
                        DrawCharacter(x, y, ref character_me);
                        //character_me = Image.FromFile("忍者_正面.png");
                        //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
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
                if (!jump && Map[x + move_copy[0][0], y + move_copy[0][1]] == 2)
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

                if (Map[x, y] == 101 && Map[x - move_copy[0][1], y - move_copy[0][0]] != 3)     //何の判定???
                {
                    DrawCharacter(x, y, ref character_me);
                    //character_me = Image.FromFile("忍者_正面.png");
                    //g2.DrawImage(character_me, x * cell_length - extra_length, y * cell_length - 2 * extra_length, cell_length + 2 * extra_length, cell_length + 2 * extra_length);
                    break;
                }

                //移動先が氷の上なら同じ方向にもう一回進む
                if (!jump && Map[x, y] == 8)                         //氷ステージ出来たら数字きめる(いまは一旦8)
                {
                    //500ミリ秒=0.5秒待機する
                    Thread.Sleep(waittime);
                    continue;
                }

                //移動先がジャンプ台なら同じ方向に二回進む（１個先の障害物は無視）
                if (Map[x, y] == 9 || jump)                           //ジャンプ台の番号も決める(いまは一旦9)
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

                switch (Map[x, y])       //動く床
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

        #region 会話の表示（Form依存なのでこの位置）
        private void drawConversations(bool isStart)
        {
            Graphics gConv = Graphics.FromImage(bmpConv);

            //Pen pen = new Pen(Color.FromArgb(100, 255, 100), 2);
            Font fnt_name = new Font("游ゴシック", 33, FontStyle.Bold);
            Font fnt_dia = new Font("游ゴシック", 33);
            Brush Color_BackConv = new SolidBrush(ColorTranslator.FromHtml("#f8e58c"));
            Brush Color_BackName = new SolidBrush(ColorTranslator.FromHtml("#856859"));
            int sp = 5;

            int face = 300;
            int name_x = 300;
            int name_y = 60;

            int dia_x = 1500;
            int dia_y = 200;

            int adjust_y = 0;

            int lineHeight = fnt_dia.Height;

            gConv.FillRectangle(Color_BackName, 15, adjust_y + face, name_x, name_y);
            //g1.DrawRectangle(pen, 15, adjust_y + face, name_x, name_y);

            gConv.FillRectangle(Color_BackConv, 15, adjust_y + face + name_y, dia_x, dia_y);
            //g1.DrawRectangle(pen, 15, adjust_y + face + name_y, dia_x, dia_y);

            List<Conversation> Conversations = new List<Conversation>();
            if (isStart)
            {
                Conversations = StartConv;
            }
            else
            {
                Conversations = EndConv;
            }

            if (convIndex >= Conversations.Count)
            {
                pictureBoxConv.Visible = false;
                pictureBoxConv.Enabled = false;
                pictureBoxConv.SendToBack();
                return;
            }

            string charaName = "";
            if (Conversations[convIndex].Character == "主人公")
            {
                if (MainCharacter.isBoy)
                {
                    charaName = "タロウ";
                }
                else
                {
                    charaName = "ハナコ";
                }
            }

            gConv.DrawString(charaName, fnt_name, Brushes.White, 15 + sp, adjust_y + face + sp);

            //改行の処理はこう書かないとうまくいかない
            char[] lineBreak = new char[] { '\\' };
            string[] DialogueLines = Conversations[convIndex].Dialogue.Replace("\\n", "\\").Split(lineBreak);
            for (int i = 0; i < DialogueLines.Length; i++)
            {
                gConv.DrawString(DialogueLines[i], fnt_dia, Brushes.Black, 15 + sp, adjust_y + face + name_y + sp + i * lineHeight);
            }

            Image charaImage = null;
            if (Conversations[convIndex].Img == "Main")
            {
                if (MainCharacter.isBoy)
                {
                    charaImage = Dictionaries.Img_Character["Boy"];
                }
                else
                {
                    charaImage = Dictionaries.Img_Character["Girl"];
                }
            }

            gConv.DrawImage(charaImage, 15, adjust_y, face, face);

            pictureBoxConv.Image = bmpConv;
            gConv.Dispose();

            if (convIndex < Conversations.Count)
            {
                convIndex++;
            }
           
        }

        private void pictureBoxConv_Click(object sender, EventArgs e)
        {
            drawConversations(isStartConv);
        }
        #endregion
    }

    #region カスタム設定
    public class UIButtonObject : UserControl
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            GraphicsPath graphic = new GraphicsPath();
            graphic.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new Region(graphic);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillEllipse(new SolidBrush(this.BackColor), 0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }
        
    }
    
    #endregion
}
