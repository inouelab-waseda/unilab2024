using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace unilab2024
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Func.LoadImg_Character();
            Func.LoadImg_DotPic();
            Func.LoadImg_Button();
            Func.InitializeIsCleared();

            Application.Run(new Title());
        }
    }

    #region フォーム呼び出し
    public static partial class Func
    {
        
        public static void CreatePrologue ()
        {
            Prologue form = new Prologue();
            form.Show();
        }
        
        public static void CreateWorldMap(Form currentForm) //呼び出し方: Func.CreateWorldMap(this);
        {
            WorldMap form = new WorldMap();
            form.Show();
            currentForm.Dispose();
        }

        public static void CreateStageSelect(Form currentForm,string worldName, int worldNumber) //呼び出し方: Func.CreateStageSelect(this,"1年生",1);
        {
            StageSelect form = new StageSelect();
            form.WorldName = worldName;
            form.WorldNumber = worldNumber;
            form.Show();
            currentForm.Dispose();
        }

        public static void CreateStage(Form currentForm, string worldName,int worldNumber, int level) //呼び出し方: Func.CreateStageSelect(this,"1");  各ステージどう名付けるか決めたい
        {
            Stage form = new Stage();
            form.WorldName = worldName;
            form.WorldNumber = worldNumber;
            form.Level = level;
            form.Show();
            currentForm.Dispose();
        }
        
    }
    #endregion

    #region Stage用関数
    public static partial class Func
    {
        public static void ResetListBox(ListBox listbox_Input, ListBox listbox)   //ListBoxの中身消去
        {
            if (listbox_Input.SelectedIndex > -1)
            {
                listbox.Items.RemoveAt(listbox.SelectedIndex);
            }
            else
            {
                listbox.Items.Clear();
            }
        }

        public static void ForLoopCount(ListBox listbox)                                 //for文回数読み込み処理
        {
            if (listbox.SelectedItem != null)                                            //ListBoxに入力が存在する場合                 
            {
                string command = listbox.SelectedItem.ToString();                        //文字として認識

                if (command == "反復魔法おわり")                                         
                {
                    return;
                }
                if (command.StartsWith("反復魔法"))
                {
                    string trial_str = Regex.Replace(command, @"[^0-9]", "");              //試行回数を抽出(string)
                    int trial = int.Parse(trial_str);                                      //試行回数をintに変換

                    int id = listbox.SelectedIndex;
                    listbox.Items[id] = "反復魔法 (" + (trial % 9 + 1).ToString() + ")";   //for文として表示

                    listbox.Refresh();
                }
            }
        }

        public static void Move(List<int[]> movelist,string direction)    //指定方向に移動する関数
        {
            int Direction_Index = int.Parse(direction);
            int[][] move = new int[4][];       // up,right.down,leftの順
            move[0] = new int[] { 0, -1 };     //up
            move[1] = new int[] { 1, 0 };      //right
            move[2] = new int[] { 0, 1 };      //down 
            move[3] = new int[] { -1, 0 };     //left
            movelist.Add(move[Direction_Index]);
        }

        public static List<string> MakeMoveList(string[] A, string[] B,List<string> Move_Input)     //入力A・B(Move_Input)が再起の場合の処理
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

        public static (List<int[]>,int a) ForLoop(List<string> Move_Input, List<int[]> Move_A, List<int[]> Move_B,int i)     //for文処理
        {
            int trial;                                                                                                //反復回数
            int Now;                                                                                                  //入力したListのうち何番目の処理か
            List<int[]> Return_List = new List<int[]>();                                                              //出力の配列(動きを[x,y]として保存)
            int Return_Num = i;                                                                                       //何番目までfor文処理が続いているか
            if (Move_Input[i].StartsWith("for"))
            {
                Now = i + 1;                                                                                          //for文の処理内容はi+1行目から
                trial = int.Parse(Regex.Replace(Move_Input[i], @"[^0-9]", ""));                                       //処理回数をtrialに設定
                for (int j = 0; j < trial; j++)
                {
                    while (true)
                    {
                        if (Now  >= Move_Input.Count)                                                                 //for文の終わりが存在しない場合、エラー表示
                        {
                            MessageBox.Show("「反復魔法」と「反復魔法おわり」はセットで使ってください");
                            return (Return_List, i);
                        }
                        (Return_List, Now) = ForLoop(Move_Input,Move_A,Move_B,Now );                                  //二重ループの探索
                        if (Move_Input[Now] == "endfor") break;                                                       //for文終わりが存在したら処理終了
                        else
                        {
                            if(int.Parse(Move_Input[Now]) < 4) Func.Move(Return_List, Move_Input[Now]);               //動く方向が指定されている場合、その方向への動きをListに追加
                            else if (Move_Input[Now] == "A") Return_List.AddRange(Move_A);                            //Aの魔法が入力されている場合、Aの処理内容をListに追加
                            else Return_List.AddRange(Move_B);                                                        //Bの魔法の際も同様
                            Now++;
                        }
                    }
                }
            }
            return (Return_List,Return_Num);                                                                          //動きの内容(List)とどこまで処理したかを返却
        }
    }
    #endregion

    #region 会話
    public static partial class Func
    {
        public static List<Conversation> LoadConversations(string ConvFileName)  //引数はConversationファイルの名前
        {
            List<Conversation> Conversations = new List<Conversation>();

            using (StreamReader sr = new StreamReader($"{ConvFileName}"))
            {
                bool isFirstRow = true;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');

                    if (isFirstRow) //escape 1st row
                    {
                        isFirstRow = false;
                        continue;
                    }

                    Conversations.Add(new Conversation(values[0], values[1], values[2]));
                }
            }

            return Conversations;
        }
    } 
    public struct Conversation
    {
        public string Character;
        public string Dialogue;
        public string Img;

        public Conversation(string character, string dialogue, string img)
        {
            Character = character;
            Dialogue = dialogue;
            Img = img;
        }
    }
    #endregion

    #region 各データのDictionaryと読み込み関数
    public partial class Dictionaries
    {
        //g1.DrawImage()の第1引数にDictionaries.Img_Character["画像名"]と入れて使う
        //使い方の例（in Prologue.cs）
        //g1.DrawImage(Dictionaries.Img_Character[Conversations[convIndex].Img], 15, adjust_y, face, face);
        //ここでのConversations[convIndex].Imgは"Teacher".
        public static Dictionary<string, Image> Img_Character = new Dictionary<string, Image>();
        public static Dictionary<string, Image> Img_DotPic = new Dictionary<string, Image>();
        public static Dictionary <string, Image> Img_Button = new Dictionary<string, Image>();
    }

    public partial class Func
    {
        //読み込みはProgram.csのMain関数内で行っている。以下の関数は他のFormで呼び出す必要はない。
        public static void LoadImg_Character()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "Img_Character_*.png");
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_Character_","");
                Dictionaries.Img_Character[key] = Image.FromFile(file);
            }
        }

        public static void LoadImg_DotPic()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "Img_DotPic_*.png");
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_DotPic_", "");
                Dictionaries.Img_DotPic[key] = Image.FromFile(file);
            }
        }

        public static void LoadImg_Button()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "Img_Button_*.png");
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_Button_", "");
                Dictionaries.Img_Button[key] = Image.FromFile(file);
            }
        }
    }
    #endregion

    #region 進行状況管理
    public enum ConstNum
    {
        numWorlds = 7,
        numStages = 3
    }
    public partial class ClearCheck
    {
        public static bool[,] IsCleared = new bool[(int)ConstNum.numWorlds, (int)ConstNum.numStages];
    }

    public partial class Func
    {
        public static void InitializeIsCleared()    //Main関数で呼び出す
        {
            for (int i = 0; i < (int)ConstNum.numWorlds; i++)
            {
                for(int j = 0; j < (int)ConstNum.numStages; j++)
                {
                    ClearCheck.IsCleared[i, j] = false;
                }
            }
        }
    }
    #endregion

    #region カスタムボタン（文字の上に画像を描画する）
    public class CustomButton : Button
    {
        private Image foreImage;

        public Image ForeImage
        {
            get { return foreImage; }
            set
            {
                foreImage = value;
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            //ボタンのベース描画
            base.OnPaint(pevent);

            //文字の描画
            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            //背景画像を文字の上に描画
            if (this.ForeImage != null)
            {
                //Zoomレイアウトで背景画像を描画
                //ボタンサイズ
                int buttonWidth = this.Width;
                int buttonHeight = this.Height;
                //画像サイズ
                int imageWidth = this.ForeImage.Width;
                int imageHeight = this.ForeImage.Height;

                //縦横比を保ちながらスケーリング
                float scale = Math.Min((float)buttonWidth / imageWidth, (float)buttonHeight / imageHeight);
                int scaleWidth = (int)(imageWidth * scale);
                int scaleHeight = (int)(imageHeight * scale);

                //位置調整
                int x = (buttonWidth - scaleWidth) / 2;
                int y = (buttonHeight - scaleHeight) / 2;
                Rectangle destRect = new Rectangle(x, y, scaleWidth, scaleHeight);
               
                pevent.Graphics.DrawImage(this.ForeImage, destRect);
            }
        }

        private Rectangle GetZoomedRectangle(Image img, Rectangle bounds)
        {
            float imageAspect = (float)img.Width / img.Height;
            float controlAspect = (float)bounds.Width / bounds.Height;

            if (imageAspect > controlAspect)
            {
                int newHeight = (int)(bounds.Width / imageAspect);
                int yOffset = (bounds.Height - newHeight) / 2;
                return new Rectangle(0, yOffset, bounds.Width, newHeight);
            }
            else
            {
                int newWidth = (int)(bounds.Height * imageAspect);
                int xOffset = (bounds.Width - newWidth) / 2;
                return new Rectangle(xOffset, 0, newWidth, bounds.Height);
            }
        }
    }
    #endregion
}
