﻿using System;
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
    #region Main関数
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
            Func.LoadImg_Object();
            Func.LoadImg_Button();
            Func.LoadImg_Background();
            Func.InitializeClearCheck();

            Application.Run(new Title());
        }
    }
    #endregion

    #region フォーム呼び出し
    public static partial class Func
    {
        
        public static void CreatePrologue (Form currentForm)
        {
            CurrentFormState.FormName = "Prologue";
            CurrentFormState.StateData.Clear();

            Prologue form = new Prologue();
            form.Show();
            if (!(currentForm is Title))
            {
                currentForm.Dispose();
            }
        }
        
        public static void CreateWorldMap(Form currentForm) //呼び出し方: Func.CreateWorldMap(this);
        {
            CurrentFormState.FormName = "WorldMap";
            CurrentFormState.StateData.Clear();

            WorldMap form = new WorldMap();
            form.Show();
            if (!(currentForm is Title))
            {
                currentForm.Dispose();
            }
        }

        public static void CreateAnotherWorld(Form currentForm) 
        {
            CurrentFormState.FormName = "AnotherWorld";
            CurrentFormState.StateData.Clear();

            AnotherWorld form = new AnotherWorld();
            form.Show();
            if (!(currentForm is Title))
            {
                currentForm.Dispose();
            }
        }

        public static void CreateStageSelect(Form currentForm,string worldName, int worldNumber) //呼び出し方: Func.CreateStageSelect(this,"1年生",1);
        {
            CurrentFormState.FormName = "StageSelect";
            CurrentFormState.StateData.Clear();
            CurrentFormState.StateData["WorldName"] = worldName;
            CurrentFormState.StateData["WorldNumber"] = worldNumber;

            StageSelect form = new StageSelect();
            form.WorldName = worldName;
            form.WorldNumber = worldNumber;
            form.Show();
            if (!(currentForm is Title))
            {
                currentForm.Dispose();
            }
        }

        public static void CreateStage(Form currentForm, string worldName,int worldNumber, int level) //呼び出し方: Func.CreateStageSelect(this,"1");  各ステージどう名付けるか決めたい
        {
            CurrentFormState.FormName = "Stage";
            CurrentFormState.StateData.Clear();
            CurrentFormState.StateData["WorldName"] = worldName;
            CurrentFormState.StateData["WorldNumber"] = worldNumber;
            CurrentFormState.StateData["Level"] = level;

            Stage form = new Stage();
            form.WorldName = worldName;
            form.WorldNumber = worldNumber;
            form.Level = level;
            form.Show();
            if(!(currentForm is Title))
            {
                currentForm.Dispose();
            }
        }
        
    }
    #endregion

    #region 会話
    public static partial class Func
    {
        public static List<Conversation> LoadConversations(string ConvFileName)  //引数はConversationファイルの名前
        {
            List<Conversation> Conversations = new List<Conversation>();

            using (StreamReader sr = new StreamReader($"Story\\{ConvFileName}"))
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

        public static (List<Conversation>,List<Conversation>) LoadStories(string ConvFileName, string cutWord)
        {
            List<Conversation> StartConv = new List<Conversation>();
            List<Conversation> EndConv = new List<Conversation>();

            using (StreamReader sr = new StreamReader($"Story\\{ConvFileName}"))
            {
                bool isFirstRow = true;
                bool isBeforePlay = true;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');

                    if (isFirstRow) //escape 1st row
                    {
                        isFirstRow = false;
                        continue;
                    }
                    if (values[1] == cutWord)
                    {
                        isBeforePlay = false;
                        continue;
                    }

                    if (isBeforePlay)
                    {
                        StartConv.Add(new Conversation(values[0], values[1], values[2]));
                    }
                    else
                    {
                        EndConv.Add(new Conversation(values[0], values[1], values[2]));
                    }
                }
            }

            return (StartConv, EndConv);
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
        public static List<Image> Img_Object = new List<Image>();
        public static Dictionary <string, Image> Img_Button = new Dictionary<string, Image>();
        public static Dictionary <string, Image> Img_Background = new Dictionary<string, Image>();
    }

    public partial class Func
    {
        //読み込みはProgram.csのMain関数内で行っている。以下の関数は他のFormで呼び出す必要はない。
        public static void LoadImg_Character()
        {
            Dictionaries.Img_Character.Clear();
            string[] files = Directory.GetFiles(@"Image\\Character");
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_Character_","");
                Dictionaries.Img_Character[key] = Image.FromFile(file);
            }
        }

        public static void LoadImg_DotPic()
        {
            Dictionaries.Img_DotPic.Clear();
            string charaDirectory = "";
            if (MainCharacter.isBoy)
            {
                charaDirectory = @"Image\\DotPic\\Boy";
            }
            else
            {
                charaDirectory = @"Image\\DotPic\\Girl";
            }
            string[] files = Directory.GetFiles(charaDirectory);
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_DotPic_", "");
                Dictionaries.Img_DotPic[key] = Image.FromFile(file);
            }
        }
        public static void LoadImg_Object()
        {
            Dictionaries.Img_Object.Clear();
            string[] files = Directory.GetFiles(@"Object");
            foreach (string file in files)
            {
                //string File_Name = file.Replace("Object\\", "");
                Dictionaries.Img_Object.Add(Image.FromFile(file));
            }
        }

        public static void LoadImg_Button()
        {
            Dictionaries.Img_Button.Clear();
            string[] files = Directory.GetFiles(@"Image\\Button");
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_Button_", "");
                Dictionaries.Img_Button[key] = Image.FromFile(file);
            }
        }

        public static void LoadImg_Background()
        {
            Dictionaries.Img_Background.Clear();
            string[] files = Directory.GetFiles(@"Image\\Background");
            foreach (string file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file).Replace("Img_Background_", "");
                Dictionaries.Img_Background[key] = Image.FromFile(file);
            }
        }
    }
    #endregion

    #region キャラ選択結果
    public partial class MainCharacter
    {
        public static bool isBoy = true;
    }
    #endregion

    #region 進行状況管理
    public enum ConstNum
    {
        numWorlds = 7+1,
        numStages = 3+1
    }

    public partial class CurrentFormState
    {
        public static string FormName = "Prologue";
        public static Dictionary<string,object>StateData = new Dictionary<string,object>();
    }
    public partial class ClearCheck
    {
        //クリアチェック配列
        //0番目はそのWorldのレベル3つをすべてクリアしたらtrueにする。
        public static bool[,] IsCleared = new bool[(int)ConstNum.numWorlds, (int)ConstNum.numStages];

        //ボタン管理配列
        //0番目はWorldMapでそのボタンを押せるかどうか（押せる場合true）
        public static bool[,] IsButtonEnabled = new bool[(int)ConstNum.numWorlds, (int)ConstNum.numStages];

        //新ステージ出現チェック配列
        //0番目はWorldMapでそのワールドの中に新ステージがあるかどうか
        public static bool[,] IsNew = new bool[(int)ConstNum.numWorlds, (int)ConstNum.numStages];
    }

    public partial class Func
    {
        public static void InitializeClearCheck()    //Main関数で呼び出す
        {
            for (int i = 0; i < (int)ConstNum.numWorlds; i++)
            {
                for(int j = 0; j < (int)ConstNum.numStages; j++)
                {
                    ClearCheck.IsCleared[i, j] = false;
                    ClearCheck.IsButtonEnabled[i, j] = false;
                    ClearCheck.IsNew[i, j] = false;
                }
            }
            ClearCheck.IsButtonEnabled[1, 0] = true;
            ClearCheck.IsButtonEnabled[1, 1] = true;
        }

        public static void UpdateIsNew()    //IsNew配列の更新
        {
            for (int i = 1; i < (int)ConstNum.numWorlds; i++)
            {
                bool isNew0 = false;
                for (int j = 1; j < (int)ConstNum.numStages; j++)
                {
                    if (ClearCheck.IsNew[i, j])
                    {
                        isNew0 = true;
                        break;
                    }
                }
                ClearCheck.IsNew[i, 0] = isNew0;
            }
        }

        public static bool HasNewStageInWorld(bool isWorldMap)
        {
            // WorldMapまたはAnotherWorldに新ステージがあるかどうか
            bool hasNewStage = false;

            if (isWorldMap)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (ClearCheck.IsNew[i, 0])
                    {
                        hasNewStage = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 5; i <= 7; i++)
                {
                    if (ClearCheck.IsNew[i, 0])
                    {
                        hasNewStage = true;
                        break;
                    }
                }
            }

            return hasNewStage;
        }

        public static bool HasNewStageFromStageSelect(bool isWorldMap, int worldNumber)
        {
            // StageSelectからWorld選択に戻った時に新ステージがあるかどうか
            if (Func.HasNewStageInWorld(!isWorldMap)) return true;

            bool hasNewStage = false;

            if (isWorldMap)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (i == worldNumber) continue;
                    if (ClearCheck.IsNew[i, 0])
                    {
                        hasNewStage = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 5; i <= 7; i++)
                {
                    if (i == worldNumber) continue;
                    if (ClearCheck.IsNew[i, 0])
                    {
                        hasNewStage = true;
                        break;
                    }
                }
            }

            return hasNewStage;
        }

        public static bool HasNewStageInAllWorld()
        {
            // 新ステージがあるかどうか
            bool hasNewStage = false;

            for (int i = 1; i < (int)ConstNum.numWorlds; i++)
            {
                for (int j = 0; j < (int)ConstNum.numStages; j++)
                {
                    if (ClearCheck.IsNew[i, j])
                    {
                        hasNewStage = true;
                        break;
                    }
                }
            }

            return hasNewStage;
        }

        public static bool IsAllStageClearedInWorld(bool isWorldMap)
        {
            // WorldMapまたはAnotherWorldがすべてクリアされているかどうか
            bool isAllStageCleared = true;

            if (isWorldMap)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (!ClearCheck.IsCleared[i, 0])
                    {
                        isAllStageCleared = false;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 5; i <= 7; i++)
                {
                    if (!ClearCheck.IsCleared[i, 0])
                    {
                        isAllStageCleared = false;
                        break;
                    }
                }
            }

            return isAllStageCleared;
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

        private Image conditionImage;

        public Image ConditionImage
        {
            get { return conditionImage; }
            set
            {
                conditionImage = value;
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            //ボタンのベース描画
            base.OnPaint(pevent);

            //文字の描画
            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            //ボタンサイズ
            int buttonWidth = this.Width;
            int buttonHeight = this.Height;

            //背景画像を文字の上に描画
            if (this.ForeImage != null)
            {
                //Zoomレイアウトで背景画像を描画
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
            else if (this.ConditionImage != null)
            {
                //画像サイズ
                int imageWidth = this.ConditionImage.Width;
                int imageHeight = this.ConditionImage.Height;

                // 表示領域の大きさ指定
                int scaleHeight = buttonHeight / 4;
                double scale = (double)scaleHeight / imageHeight;
                int scaleWidth = (int)(scale * imageWidth);
                Rectangle destRect = new Rectangle(0, 0, scaleWidth, scaleHeight);

                pevent.Graphics.DrawImage(this.ConditionImage, destRect);
            }
        }
    }
    #endregion
}
