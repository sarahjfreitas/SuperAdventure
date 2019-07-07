using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player player;
        public SuperAdventure()
        {
            InitializeComponent();

            player = new Player(10,10,20,0,1);

            lblExperience.Text = player.ExperiencePoints.ToString();
            lblGold.Text = player.Gold.ToString();
            lblLevel.Text = player.Level.ToString();
            lblHitPoints.Text = player.CurrentHitPoints.ToString();

            Location location = new Location(1, "Home", "This is your house.");

        }

        private void SuperAdventure_Load(object sender, EventArgs e)
        {

        }

    }
}
