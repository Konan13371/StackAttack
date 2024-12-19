using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stack_Attack
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Stack Attack - простая и увлекательная игра.\n \n" +
               "Цель игры состоит в том, чтобы набрать как можно больше очков.\n \n" +
               "Изначально создается случайное поле с определенным количеством кубиков. После этого сверху начнут падать кубики в случайные места. \n \n" +
               "Задача игрока - перемещать кубик строителем и выстраивать линии из 12 блоков по горизонтали. Человек может прыгать на уровень выше и перемещать по одному кубику за раз. \n \n" +
               "Игрок может разбивать кубики головой во время прыжков. \n" +
               "Конец игры наступает, когда кубик падает человеку на голову.", "Руководство пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void aboutrazrabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" Создатели: Тигин Егор Дмитриевич \n Студент группы РПС 21.", "О разработчиках", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите выйти?", "Информация", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                Application.Exit();
        }

        private void buttonGame_Click(object sender, EventArgs e)
        {
            this.Hide();
            Game game = new Game();
            game.ShowDialog();
        }
    }
}