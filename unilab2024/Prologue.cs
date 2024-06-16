using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static unilab2024.Stage;

namespace unilab2024
{
    public partial class Prologue : Form
    {

        #region メンバ変数の定義など
        Bitmap bmpPB1;
        List<Conversation> Conversations;
        int convIndex = 0;

        public Prologue()
        {
            InitializeComponent();

            bmpPB1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmpPB1;

            this.KeyDown += new KeyEventHandler(Prologue_KeyDown);
            this.KeyPreview = true;
        }
        #endregion

        #region 読み込み時
        private void Prologue_Load(object sender, EventArgs e)
        {
            Graphics g1 = Graphics.FromImage(bmpPB1);
            buttonToMap.Visible = false;
            buttonToMap.Enabled = false;

            Conversations = Func.LoadConversations("Conv_Story_Chapter0.csv");
            drawConversations();
        }
        #endregion

        #region 会話の表示（Form依存なのでこの位置）
        private void drawConversations()
        {
            Graphics g1 = Graphics.FromImage(bmpPB1);

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

            int adjust_y = 300;

            int lineHeight = fnt_dia.Height;

            g1.FillRectangle(Color_BackName, 15, adjust_y + face, name_x, name_y);
            //g1.DrawRectangle(pen, 15, adjust_y + face, name_x, name_y);

            g1.FillRectangle(Color_BackConv, 15, adjust_y + face + name_y, dia_x, dia_y);
            //g1.DrawRectangle(pen, 15, adjust_y + face + name_y, dia_x, dia_y);

            g1.DrawString(Conversations[convIndex].Character, fnt_name, Brushes.White, 15 + sp, adjust_y + face + sp);

            //改行の処理はこう書かないとうまくいかない
            char[] lineBreak = new char[]{ '\\' };
            string[] DialogueLines = Conversations[convIndex].Dialogue.Replace("\\n","\\").Split(lineBreak);
            for (int i = 0; i < DialogueLines.Length; i++)
            {
                g1.DrawString(DialogueLines[i], fnt_dia, Brushes.Black, 15 + sp, adjust_y + face + name_y + sp + i*lineHeight);
            }
            g1.DrawImage(Dictionaries.Img_Character[Conversations[convIndex].Img], 15, adjust_y, face, face);

            pictureBox1.Image = bmpPB1;
            g1.Dispose();

            if (convIndex < Conversations.Count - 1)
            {
                convIndex++;
            }
            else
            {
                buttonToMap.Visible = true;
                buttonToMap.Enabled = true;
                return;
            }
        }
        #endregion

        #region 諸々クリックの処理
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            drawConversations();
        }

        private void buttonToMap_Click(object sender, EventArgs e)
        {
            Func.CreateWorldMap(this);
        }
        #endregion

        #region ストーリースキップ用
        //public Prologue関数内に以下を追記
        //this.KeyDown += new KeyEventHandler(Prologue_KeyDown);
        //this.KeyPreview = true;
        private void Prologue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.M)
            {
                buttonToMap.Visible = true;
                buttonToMap.Enabled = true;
                return;
            }
        }
        #endregion
    }
}
