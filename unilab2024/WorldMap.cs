using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace unilab2024
{
    public partial class WorldMap : Form
    {
        #region ボタンの定義
        CustomButton buttonToAnotherWorld;
        public WorldMap()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Prologue_KeyDown);
            this.KeyPreview = true;

            buttonToAnotherWorld = new CustomButton
            {
                Size = new Size(200, 60),
                Location = new Point(1189, 537),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Text = "外の世界へ",
                Font = new Font("游ゴシック", 21.75f, FontStyle.Bold),
                //ForeImage = Dictionaries.Img_Button["Lock"],
                Cursor = Cursors.No
            };

            this.Controls.Add(buttonToAnotherWorld);
            buttonToAnotherWorld.BringToFront();
        }
        #endregion

        #region 読み込み時
        private void WorldMap_Load(object sender, EventArgs e)
        {
            if (ClearCheck.IsCleared[3, 1])
            {
                buttonToAnotherWorld.ForeImage = null;
                buttonToAnotherWorld.Cursor = Cursors.Hand;
                buttonToAnotherWorld.Enabled = true;
            }
            else
            {
                buttonToAnotherWorld.ForeImage = Dictionaries.Img_Button["Lock"];
                buttonToAnotherWorld.Cursor = Cursors.No;
                buttonToAnotherWorld.Enabled = false;
                //Enable = falseにするとCursors.Noの設定が意味をなさないっぽい。ボタン押したときの処理変えるのはあり
            }
        }
        #endregion

        #region StageSelectフォームの呼び出し
        private void button1_Click(object sender, EventArgs e)
        {
            Func.CreateStageSelect(this, button1.Text,1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Func.CreateStageSelect(this, button2.Text,2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Func.CreateStageSelect(this, button3.Text,3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Func.CreateStageSelect(this, button4.Text,4);
        }
        #endregion

        #region クリアチェックスキップ用
        private void Prologue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.M)
            {
                for (int i = 0; i < (int)ConstNum.numWorlds; i++)
                {
                    for (int j = 0; j < (int)ConstNum.numStages; j++)
                    {
                        ClearCheck.IsCleared[i, j] = true;
                    }
                }
                Func.CreateWorldMap(this);
            }
        }
        #endregion
    }
}
